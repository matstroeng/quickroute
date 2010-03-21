using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.IO;
using QuickRoute.BusinessEntities.Importers;
using QuickRoute.Resources;
using QuickRoute.UI.Classes;

namespace QuickRoute.UI.Forms
{
  public partial class CreateNewForm : Form
  {
    private readonly List<FileFormat> supportedMapImageFileFormats;
    private readonly List<RouteFileFormat> supportedRouteFileFormats;
    private readonly List<GPSDevice> supportedGPSDevices;

    public CreateNewForm()
      : this(null)
    {

    }

    public CreateNewForm(string initialRouteFileName)
    {
      InitializeComponent();

      mapImageFileName.Items.AddRange(Util.ApplicationSettings.RecentMapImageFileNames.ToArray());
      routeFileName.Items.AddRange(Util.ApplicationSettings.RecentRouteFileNames.ToArray());

      supportedMapImageFileFormats = SupportedImportFormatManager.GetSupportedMapFileFormats();
      mapImageFileFormatComboBox.DataSource = supportedMapImageFileFormats;

      supportedRouteFileFormats = SupportedImportFormatManager.GetSupportedRouteFileFormats();
      routeFileFormatComboBox.DataSource = supportedRouteFileFormats;

      supportedGPSDevices = SupportedImportFormatManager.GetSupportedGPSDevices();

      bool gpsDevicesFound = SearchForGPSDevices();

      if (initialRouteFileName != null)
      {
        routeFileName.Text = initialRouteFileName;
        routeFromFile.Checked = true;
      }
      else
      {
        if (!gpsDevicesFound) routeFromFile.Checked = true;
      }

      InitialTransformation = new Transformation();

      UpdateUI();
    }

    #region Public properties

    public Map Map { get; private set; }

    public Route Route { get; private set; }

    public LapCollection Laps { get; private set; }

    public Transformation InitialTransformation { get; private set; }
    
    public SessionPerson Person
    {
      get
      {
        return persons.SelectedItem as SessionPerson ?? new SessionPerson() { Name = persons.Text };
      }
      set
      {
        persons.SelectedItem = value;
      }
    }

    #endregion

    #region Event handlers

    public string FileNameProposal
    {
      get
      {
        if (mapImageFromFile.Checked)
        {
          return Path.GetFileNameWithoutExtension(mapImageFileName.Text) + ".qrt";
        }
        if (mapImageFromUrl.Checked)
        {
          Uri uri = new Uri(mapImageUrl.Text);
          return Path.GetFileNameWithoutExtension(uri.Segments[uri.Segments.Length - 1]) + ".qrt";
        }
        return Strings.Untitled + ".qrt";
      }
    }

    private void ok_Click(object sender, EventArgs e)
    {
      if (Import() == DialogResult.OK)
      {
        DialogResult = DialogResult.OK;
        Close();
      }
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void mapImageFileNameBrowse_Click(object sender, EventArgs e)
    {
      using (var ofd = new OpenFileDialog { CheckFileExists = true })
      {
        var filterItems = new string[supportedMapImageFileFormats.Count];
        for (int i = 0; i < supportedMapImageFileFormats.Count; i++)
        {
          filterItems[i] = supportedMapImageFileFormats[i].FileFilter;
        }
        ofd.Filter = string.Join("|", filterItems);
        ofd.FilterIndex = mapImageFileFormatComboBox.SelectedIndex + 1;
        ofd.Title = Strings.SelectMapImageFile;
        if (Util.ApplicationSettings.InitialFolders.ContainsKey(ApplicationSettings.FileDialogType.ImportMapImage))
        {
          ofd.InitialDirectory =
            Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportMapImage];
        }

        if (ofd.ShowDialog() == DialogResult.OK)
        {
          mapImageFileName.Text = ofd.FileName;
          mapImageFileFormatComboBox.SelectedIndex = ofd.FilterIndex - 1;
          Util.ApplicationSettings.InitialFolders[ApplicationSettings.FileDialogType.ImportMapImage] =
            Path.GetDirectoryName(ofd.FileName);
        }
      }
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
                           Title = Strings.SelectRouteFile
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

    private void mapImageFromFile_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void mapImageFromUrl_CheckedChanged(object sender, EventArgs e)
    {
      UpdateUI();
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

    private void mapImageFileName_TextChanged(object sender, EventArgs e)
    {
      UpdateMapImageFileFormat();
    }

    #endregion

    public void SetPersons(List<SessionPerson> personList)
    {
      persons.Items.Clear();
      if (personList == null || personList.Count == 0) return;
      persons.Items.AddRange(personList.ToArray());
      persons.SelectedItem = personList[0];
    }

    private DialogResult Import()
    {
      // validate file names
      if (mapImageFromFile.Checked && !File.Exists(mapImageFileName.Text))
      {
        MessageBox.Show(Strings.MapImageFileDoesNotExist, Strings.InvalidMapImage, MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
        return DialogResult.Cancel;
      }
      if (routeFromFile.Checked && !File.Exists(routeFileName.Text))
      {
        MessageBox.Show(Strings.RouteFileDoesNotExist, Strings.InvalidRoute, MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
        return DialogResult.Cancel;
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
          return DialogResult.Cancel;
        }
        routeImporter = gpsDevice.Importer;
      }

      routeImporter.BeginWork += routeImporter_BeginWork;
      routeImporter.WorkProgress += routeImporter_WorkProgress;
      routeImporter.EndWork += routeImporter_EndWork;

      DialogResult result;
      try
      {
        result = routeImporter.ShowPreImportDialogs();
      }
      catch (Exception ex)
      {
        Cursor = Cursors.Default;
        Util.ShowExceptionMessageBox(ex, Strings.InvalidRoute);
        return DialogResult.Cancel;
      }
      if (result == DialogResult.OK)
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

        if (!routeImporter.ImportResult.Succeeded)
        {
          // an error occured, show relevant error info and cancel creation of new document.
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
          return DialogResult.Cancel;
        }

        try
        {
          if (mapImageFromFile.Checked)
          {
            switch (mapImageFileFormatComboBox.SelectedIndex)
            {
              case 0: // map image from image file
                Map = new Map(mapImageFileName.Text, MapSourceType.FileSystem, MapStorageType.Inline);
                // is it a QuickRoute image? if yes, use embedded transformation matrix
                var ed = QuickRouteJpegExtensionData.FromJpegFile(mapImageFileName.Text);
                if (ed != null && ed.Sessions != null && ed.Sessions.Count > 0)
                {
                  InitialTransformation = ed.Sessions.CalculateAverageTransformation();
                }
                break;

              case 1: // map image from QuickRoute file
                Document d = Document.OpenFromQrt(mapImageFileName.Text);
                if (d != null)
                {
                  Map = d.Map;
                  InitialTransformation = d.Sessions.CalculateAverageTransformation();
                }
                break;

              case 2: // map image from kmz file
                var kmz = new KmzDocument(mapImageFileName.Text);
                if (kmz.ImageStream != null)
                {
                  Map = new Map(kmz.ImageStream);
                  InitialTransformation = kmz.Transformation;
                }
                break;
            }
          }
          else if (mapImageBlank.Checked)
          {
            var blankMap = new Bitmap(1500, 1500);
            using (Graphics g = Graphics.FromImage(blankMap))
            {
              g.Clear(Color.White);
            }
            Map = new Map(blankMap);

          }
          else if (mapImageFromUrl.Checked)
          {
            Map = new Map(mapImageUrl.Text, MapSourceType.Url, MapStorageType.Inline);
          }
        }
        catch (Exception ex)
        {
          Cursor = Cursors.Default;
          Util.ShowExceptionMessageBox(ex, Strings.InvalidMapImage);
          return DialogResult.Cancel;
        }
        Route = routeImporter.ImportResult.Route;
        Laps = routeImporter.ImportResult.Laps;
        if (mapImageFromFile.Checked) Util.ApplicationSettings.AddRecentMapImageFileName(mapImageFileName.Text);
        if (routeFromFile.Checked) Util.ApplicationSettings.AddRecentRouteFileName(routeFileName.Text);
      }
      return result;
    }

    protected void routeImporter_EndWork(object sender, EventArgs e)
    {
      Cursor = Cursors.Default;
    }

    protected void routeImporter_WorkProgress(object sender, WorkProgressEventArgs e)
    {
    }

    protected void routeImporter_BeginWork(object sender, EventArgs e)
    {
      Cursor = Cursors.WaitCursor;
    }

    private void UpdateUI()
    {
      mapImageFileName.Enabled = mapImageFromFile.Checked;
      mapImageFileNameBrowse.Enabled = mapImageFromFile.Checked;
      mapImageFileFormatComboBox.Enabled = mapImageFromFile.Checked;
      mapImageUrl.Enabled = mapImageFromUrl.Checked;

      routeFileName.Enabled = routeFromFile.Checked;
      routeFileNameBrowse.Enabled = routeFromFile.Checked;
      routeFileFormatComboBox.Enabled = routeFromFile.Checked;

      routeGpsDevice.Enabled = routeFromGpsDevice.Checked;
      refreshButton.Enabled = routeFromGpsDevice.Checked;
    }

    private bool SearchForGPSDevices()
    {
      routeGpsDevice.DataSource = null;
      routeGpsDevice.Items.Clear();
      var devices = SupportedImportFormatManager.SearchForGPSDevices();
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

    private void UpdateMapImageFileFormat()
    {
      bool found = false;
      foreach (FileFormat ff in supportedMapImageFileFormats)
      {
        foreach (string e in ff.Extensions)
        {
          if (mapImageFileName.Text.EndsWith(e))
          {
            mapImageFileFormatComboBox.SelectedItem = ff;
            found = true;
            break;
          }
        }
        if (found) break;
      }

    }

    private void refreshButton_Click(object sender, EventArgs e)
    {
        using (var progressIndicator = new RefreshDevicesProgressIndicator())
        {
            routeGpsDevice.DataSource = null;
            routeGpsDevice.Items.Clear();
            SupportedImportFormatManager.StartRefreshGPSDevices();
            progressIndicator.ShowDialog();
            SearchForGPSDevices();
        }
    }
  }
}