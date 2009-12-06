using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.IO;
using QuickRoute.BusinessEntities.Importers;
using QuickRoute.BusinessEntities.Importers.FRWD;
using QuickRoute.BusinessEntities.Importers.Garmin.Forerunner;
using QuickRoute.BusinessEntities.Importers.Garmin.ANTAgent;
using QuickRoute.BusinessEntities.Importers.GPX;
using QuickRoute.BusinessEntities.Importers.TCX;
using QuickRoute.Resources;
using QuickRoute.UI.Classes;

namespace QuickRoute.UI.Forms
{
  public partial class AddSessionsForm : Form
  {
    private readonly List<RouteFileFormat> supportedRouteFileFormats;
    private readonly List<GPSDevice> supportedGPSDevices;

    public AddSessionsForm(Document document)
    {
      InitializeComponent();

      supportedRouteFileFormats = SupportedImportFormatManager.GetSupportedRouteFileFormats(); 
      routeFileFormatComboBox.DataSource = supportedRouteFileFormats;

      supportedGPSDevices = SupportedImportFormatManager.GetSupportedGPSDevices();

      var gpsDevicesFound = SearchForGPSDevices();
      if (!gpsDevicesFound) routeFromFile.Checked = true;

      Document = document;
      Sessions = Document.Sessions.Copy();
      sessionGrid.DataSource = CreateBindingSource(Sessions);

      UpdateUI();
    }

    private static BindingSource CreateBindingSource(IEnumerable<Session> sessions)
    {
      var bindingSource = new BindingSource();
      foreach(var s in sessions)
      {
        bindingSource.Add(s);
      }
      return bindingSource;
    }

    #region Public properties

    public SessionCollection Sessions { get; set; }
    public Document Document { get; set; }

    #endregion

    #region Event handlers

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void routeFileNameBrowse_Click(object sender, EventArgs e)
    {
      var filterItems = new string[supportedRouteFileFormats.Count];
      for (var i = 0; i < supportedRouteFileFormats.Count; i++)
      {
        filterItems[i] = supportedRouteFileFormats[i].FileFilter;
      }
      using (var ofd = new OpenFileDialog
                  {
                    CheckFileExists = true,
                    Filter = string.Join("|", filterItems),
                    FilterIndex = (routeFileFormatComboBox.SelectedIndex + 1),
                    Title = Strings.SelectRouteFile,
                  })
      {
        if (Util.ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ImportRoute))
        {
          ofd.InitialDirectory = Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportRoute];
        }
        if (ofd.ShowDialog() == DialogResult.OK)
        {
          routeFileName.Text = ofd.FileName;
          routeFileFormatComboBox.SelectedIndex = ofd.FilterIndex - 1;
          Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportRoute] =
            Path.GetDirectoryName(ofd.FileName);
        }
      }
    }
    private void routeFromFile_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void routeFromGpsDevice_CheckedChanged(object sender, EventArgs e)
    {
      if (routeFromGpsDevice.Checked) SearchForGPSDevices();
      UpdateUI();
    }

    private void routeFileName_TextChanged(object sender, EventArgs e)
    {
      UpdateRouteFileFormat();
    }

    #endregion

    private void Import()
    {
      if (routeFromFile.Checked && !File.Exists(routeFileName.Text))
      {
        MessageBox.Show(Strings.RouteFileDoesNotExist, Strings.InvalidRoute, MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
        return;
      }

      IRouteImporter routeImporter = null;
      if (routeFromFile.Checked)
      {
        IRouteFileImporter routeFileImporter = ((RouteFileFormat)routeFileFormatComboBox.SelectedItem).Importer;
        routeFileImporter.FileName = routeFileName.Text;
        routeImporter = routeFileImporter;
      }
      else if (routeFromGpsDevice.Checked)
      {
        GPSDevice gpsDevice = routeGpsDevice.SelectedItem as GPSDevice;
        if (gpsDevice == null)
        {
          MessageBox.Show(Strings.NoGPSDevicesConnectedMessageBox, Strings.InvalidRoute, MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
          return;
        }
        routeImporter = gpsDevice.Importer;
      }

      routeImporter.BeginWork += routeImporter_BeginWork;
      routeImporter.WorkProgress += routeImporter_WorkProgress;
      routeImporter.EndWork += routeImporter_EndWork;

      if (routeImporter.ShowPreImportDialogs() == DialogResult.OK)
      {
        try
        {
          routeImporter.Import();
        }
        catch (Exception ex)
        {
          routeImporter.ImportResult.Succeeded = false;
          routeImporter.ImportResult.Error = ImportError.Unknown;
          routeImporter.ImportResult.ErrorMessage = ex.Message;
          routeImporter.ImportResult.Exception = ex;
        }

        if(!routeImporter.ImportResult.Succeeded)
        {
          // an error occured, show relevant error info and cancel creation of new session.
          switch (routeImporter.ImportResult.Error)
          {
            case ImportError.NoWaypoints:
              routeImporter.ImportResult.ErrorMessage = Strings.RouteImportError_NoWaypoints;
              break;

            case ImportError.NoWaypointTimes:
              routeImporter.ImportResult.ErrorMessage = Strings.RouteImportError_NoWaypointTimes;
              break;

          }
          Cursor = Cursors.Default;

          if (routeImporter.ImportResult.Exception != null)
          {
            Util.ShowExceptionMessageBox(routeImporter.ImportResult.Exception, Strings.InvalidRoute);
          }
          else
          {
            MessageBox.Show(routeImporter.ImportResult.ErrorMessage, Strings.InvalidRoute, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
          }
          
          return;
        }

        // add new session to session collection
        var monochromeColors = new Color[] {Color.Red, Color.Blue, Color.DarkGreen, Color.DarkOrange, Color.DarkGray};
        SessionSettings ss = Document.Settings.DefaultSessionSettings.Copy();
        ss.RouteLineSettingsCollection[WaypointAttribute.Pace].MonochromeColor = Color.FromArgb(160,
                                                                                                monochromeColors[
                                                                                                  Sessions.Count%
                                                                                                  monochromeColors.
                                                                                                    Length]);
        ss.RouteLineSettingsCollection[WaypointAttribute.Pace].MonochromeWidth = 3;
        Session s = new Session(
          routeImporter.ImportResult.Route, 
          routeImporter.ImportResult.Laps,
          Document.Map.Image.Size, Document.Sessions.CalculateAverageTransformation().TransformationMatrix, Document.ProjectionOrigin, ss);
        s.CreateAdjustedRoute();
        Sessions.Add(s);

        // update session grid
        sessionGrid.DataSource = CreateBindingSource(Sessions);
      }
    }

    void routeImporter_EndWork(object sender, EventArgs e)
    {
      Cursor = Cursors.Default;
    }

    void routeImporter_WorkProgress(object sender, WorkProgressEventArgs e)
    {
    }

    void routeImporter_BeginWork(object sender, EventArgs e)
    {
      Cursor = Cursors.WaitCursor;
    }

    private void UpdateUI()
    {
      routeFileName.Enabled = routeFromFile.Checked;
      routeFileNameBrowse.Enabled = routeFromFile.Checked;
      routeFileFormatComboBox.Enabled = routeFromFile.Checked;

      routeGpsDevice.Enabled = routeFromGpsDevice.Checked;
    }

    private bool SearchForGPSDevices()
    {
      routeGpsDevice.DataSource = null;
      routeGpsDevice.Items.Clear();
      var devices = SupportedImportFormatManager.SearchForGPSDevices(supportedGPSDevices);
      bool devicesFound = (devices.Count > 0);
      if (!devicesFound)
      {
        routeGpsDevice.Items.Add(Strings.NoGPSDevicesConnected);
      }
      else
      {
        routeGpsDevice.DataSource = devices;
      }
      routeGpsDevice.SelectedIndex = 0;
      return devicesFound;
    }

    private void UpdateRouteFileFormat()
    {
      bool found = false;
      foreach (RouteFileFormat ff in supportedRouteFileFormats)
      {
        foreach (string e in ff.Extensions)
        {
          if (routeFileName.Text.EndsWith(e))
          {
            routeFileFormatComboBox.SelectedItem = ff;
            found = true;
            break;
          }
        }
        if (found) break;
      }
    }

    private void addSessionButton_Click(object sender, EventArgs e)
    {
      Import();
    }

    private void sessionGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      switch (sessionGrid.Columns[e.ColumnIndex].DataPropertyName)
      {
        case "Name":
          break;
      }
    }
  }
}