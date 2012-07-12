using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Importers.TCX;

namespace QuickRoute.BusinessEntities.Importers.Garmin.ANTAgent
{
  public class GarminANTAgentImporter : IGPSDeviceImporter
  {
    private HistoryItem itemToImport;

    #region IRouteImporter Members
    public ImportResult ImportResult { get; set; }

    /// <summary>
    /// The ...Application Data\GARMIN\Devices\ path. Shall end with a backslash.
    /// </summary>
    public string Path { get; set; }

    public DialogResult ShowPreImportDialogs()
    {
      var historyItems = new List<object>();
      var baseDir = new DirectoryInfo(Path);
      if (baseDir.Exists)
      {
        foreach (DirectoryInfo di in baseDir.GetDirectories())
        {
          var antDevice = new ANTDevice(Path + di.Name + "\\");
          foreach (HistoryItem hi in antDevice.HistoryItems)
          {
            historyItems.Insert(0, hi);
          }
        }
      }

      using (var dlg = new SessionSelector())
      {
        if (BeginWork != null) BeginWork(this, new EventArgs());
        dlg.Sessions = historyItems;
        if (BeginWork != null) EndWork(this, new EventArgs());
        DialogResult result = dlg.ShowDialog();
        if (result == DialogResult.OK)
        {
          itemToImport = (HistoryItem)dlg.SelectedSession;
        }
        dlg.Dispose();

        return result;
      }
    }

    public void Import()
    {
      ImportResult = new ImportResult();
      var tcxImporter = new TCXImporter
                          {
                            FileName = itemToImport.FileInfo.FullName,
                            IdToImport = DateTime.Parse(itemToImport.Id).ToString("yyyy-MM-dd HH:mm:ss")
                          };
      tcxImporter.Import();
      ImportResult = tcxImporter.ImportResult;
    }

    public event EventHandler<EventArgs> BeginWork;
    public event EventHandler<EventArgs> EndWork;
    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    #endregion

    #region IGPSDeviceImporter Members

    public bool IsConnected
    {
      get
      {
        return Directory.Exists(Path);
      }
    }

    public bool CachedDataExists
    {
      get { return IsConnected; }
    }

    public string DeviceName
    {
      get
      {
        return "Garmin ANT Agent";
      }
    }

    public void Refresh()
    {
      // do nothing
    }

    #endregion
  }
}
