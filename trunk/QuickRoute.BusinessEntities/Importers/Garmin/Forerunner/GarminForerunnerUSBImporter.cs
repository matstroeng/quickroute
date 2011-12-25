using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.GPSDeviceReaders.GarminUSBReader;
using Timer = System.Threading.Timer;

namespace QuickRoute.BusinessEntities.Importers.Garmin.Forerunner
{
  public class GarminForerunnerUSBImporter : IGPSDeviceImporter
  {
    private readonly GarminUSBReader garminUSBReader = GarminUSBReader.Instance;

    private ImportResult importResult;
    private GarminSession sessionToImport;

    #region IGPSDeviceImporter Members

    public bool IsConnected
    {
      get
      {
        return garminUSBReader.IsConnected;
      }
    }

    public bool CachedDataExists
    {
      get
      {
        return garminUSBReader.CachedSessionsExists;
      }
    }

    public string DeviceName
    {
      get
      {
        return garminUSBReader.DeviceName;
      }
    }

    public void Refresh()
    {
      // do nothing
    }

    #endregion

    #region IRouteImporter Members

    public DialogResult ShowPreImportDialogs()
    {
      if (BeginWork != null) BeginWork(this, new EventArgs());

      DialogResult result;
      var showProgressIndicator = false;

      if (/*!garminUSBReader.CachedSessionsExists && */ !garminUSBReader.ReadingNow)
      {
        garminUSBReader.BeginReadData();
        showProgressIndicator = true;
      }
      else
      {
        showProgressIndicator = garminUSBReader.ReadingNow;
      }

      if (showProgressIndicator)
      {
        var progressIndicator = new ProgressIndicator(garminUSBReader);
        result = progressIndicator.ShowDialog();
        if (result != DialogResult.OK)
        {
          if (EndWork != null) EndWork(this, new EventArgs());
          return result;
        }
      }

      var sessionHeaders = new List<GarminSessionHeader>(garminUSBReader.GetSessionHeadersFromCache());
      sessionHeaders.Sort();
      sessionHeaders.Reverse();

      using (var dlg = new GarminSessionSelector(sessionHeaders))
      {
        result = dlg.ShowDialog();
        if (result == DialogResult.OK)
        {
          sessionToImport = garminUSBReader.GetSessionFromCache(dlg.SelectedSessionHeader);
        }
        dlg.Dispose();
      }

      if (EndWork != null) EndWork(this, new EventArgs());

      return result;
    }

    public ImportResult ImportResult
    {
      get { return importResult; }
      set { importResult = value; }
    }

    public void Import()
    {
      importResult = new ImportResult();
      if (BeginWork != null) BeginWork(this, new EventArgs());

      // The trackpoints
      List<RouteSegment> routeSegments = new List<RouteSegment>();
      bool lastTrackpointWasInvalid = false;
      bool thisTrackpointIsInvalid = false;
      RouteSegment rs = new RouteSegment();
      int current = 0;
      int total = sessionToImport.Trackpoints.Count;
      foreach (D303_Trk_Point_Type tp in sessionToImport.Trackpoints)
      {
        Waypoint waypoint = new Waypoint();
        waypoint.Time = tp.TimeAsDateTime;
        waypoint.LongLat = new LongLat(tp.Position.LongitudeAsDegrees, tp.Position.LatitudeAsDegrees);
        waypoint.Altitude = (double)tp.Altitude;
        waypoint.HeartRate = (double)tp.HeartRate;

        thisTrackpointIsInvalid = (tp.Position.Latitude == 2147483647 && tp.Position.Longitude == 2147483647);
        if (!thisTrackpointIsInvalid) rs.Waypoints.Add(waypoint);
        if (thisTrackpointIsInvalid && lastTrackpointWasInvalid && rs.Waypoints.Count > 0)
        {
          routeSegments.Add(rs);
          rs = new RouteSegment();
        }
        lastTrackpointWasInvalid = thisTrackpointIsInvalid;
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

      // The laps
      List<double> elapsedTimes = new List<double>();
      double elapsedTime = 0;
      DateTime startTime = DateTime.MinValue;
      foreach (D1001_Lap_Type xLap in sessionToImport.Laps)
      {
        if (startTime == DateTime.MinValue) startTime = xLap.StartTimeAsDateTime;
        elapsedTimes.Add(elapsedTime);
        elapsedTime += (double)xLap.TotalTime / 100;
      }
      LapCollection laps = RouteImporterUtil.CreateLapsFromElapsedTimes(startTime, elapsedTimes, routeSegments);

      importResult.Route = new Route(routeSegments);
      importResult.Laps = laps;
      importResult.Succeeded = true;
      if (EndWork != null) EndWork(this, new EventArgs());
    }

    public event EventHandler<EventArgs> BeginWork;

    public event EventHandler<EventArgs> EndWork;

    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    #endregion
  }

}
