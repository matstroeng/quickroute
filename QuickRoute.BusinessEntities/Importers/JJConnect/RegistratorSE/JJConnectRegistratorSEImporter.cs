using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader;

namespace QuickRoute.BusinessEntities.Importers.JJConnect.RegistratorSE
{
  public class JJConnectRegistratorSEImporter : IGPSDeviceImporter
  {
    private readonly JJConnectRegistratorSEReader _regSEReader = new JJConnectRegistratorSEReader();
    private IRegSETrack _trackToImport;
    private ImportResult _importResult;

    public DialogResult ShowPreImportDialogs()
    {
      if (BeginWork != null) BeginWork(this, new EventArgs());


      DialogResult result;
      if (!_regSEReader.LoadCompleted)
      {
        var progressIndicator = new ProgressIndicator(_regSEReader);
        _regSEReader.StartLoadTracks();
        result = progressIndicator.ShowDialog();
        if (result != DialogResult.OK)
        {
          if (EndWork != null) EndWork(this, new EventArgs());
          return result;
        }
      }

      using (var dlg = new JJConnectRegistratorSETrackSelector { DeviceReader = _regSEReader })
      {
        result = dlg.ShowDialog();
        if (result == DialogResult.OK)
        {
          _trackToImport = dlg.SelectedTrack;
        }
        dlg.Dispose();
      }

      if (EndWork != null) EndWork(this, new EventArgs());

      return result;
    }

    public event EventHandler<EventArgs> BeginWork;
    public event EventHandler<EventArgs> EndWork;
    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    public bool IsConnected
    {
      get
      {
        return _regSEReader.IsConnected;
      }
    }

    public bool CachedDataExists
    {
      get { return false; }
    }

    public string DeviceName
    {
      get
      {
        if (_regSEReader.IsConnected)
        {
          return String.Format("JJ-Connect Registrator SE [{0}] on {1}", _regSEReader.GetDeviceInfo().Name, _regSEReader.Port);
        }
        return "JJ-Connect Registrator SE device: not connected";
      }
    }

    public void Refresh()
    {
      _regSEReader.ScanPorts();
    }

    public ImportResult ImportResult
    {
      get { return _importResult; }
      set { _importResult = value; }
    }

    public void Import()
    {
      _importResult = new ImportResult();
      if (BeginWork != null) BeginWork(this, new EventArgs());

      // The trackpoints
      var routeSegments = new List<RouteSegment>();
      var rs = new RouteSegment();
      var current = 0;
      var total = _trackToImport.GetTrackInfo().NumbetOfTrackPoints;
      var elapsedTimes = new List<double>();
      DateTime startTime = DateTime.MinValue;
      foreach (var tp in _trackToImport.GetTrackPoints())
      {
        var waypoint = new Waypoint
                           {
                             Time = tp.Time,
                             LongLat = new LongLat((double)tp.Longitude, (double)tp.Latitude),
                             Altitude = tp.Altitude
                           };

        if (tp.HasMark(RegSEPointType.WayPoint))
        {
          elapsedTimes.Add((tp.Time - startTime).TotalSeconds);
        }

        rs.Waypoints.Add(waypoint);
        current++;
        if (WorkProgress != null && current % 10 == 0)
        {
          WorkProgress(this, new WorkProgressEventArgs((double)current / total));
        }
      }
      if (rs.Waypoints.Count > 0)
      {
        routeSegments.Add(rs);
      }
      _importResult.Route = new Route(routeSegments);

      // create one lap (from start to finish)
      LapCollection laps = RouteImporterUtil.CreateLapsFromElapsedTimes(startTime, elapsedTimes, routeSegments);
      _importResult.Laps = laps;

      _importResult.Succeeded = true;
      if (EndWork != null) EndWork(this, new EventArgs());
    }
  }
}