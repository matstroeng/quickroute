using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Importers.TCX
{
  public class TCXImporter : IRouteFileImporter
  {
    private string idToImport;
    private string fileName;
    private ImportResult importResult;

    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }

    public string IdToImport
    {
      get { return idToImport; }
      set { idToImport = value; }
    }

    public ImportResult ImportResult
    {
      get { return importResult; }
      set { importResult = value; }
    }

    public DialogResult ShowPreImportDialogs()
    {
      using (var dlg = new SessionSelector())
      {
        if (BeginWork != null) BeginWork(this, new EventArgs());
        dlg.Sessions = GetAllActivities();
        if (BeginWork != null) EndWork(this, new EventArgs());
        DialogResult result = dlg.ShowDialog();
        if (result == DialogResult.OK)
        {
          IdToImport = dlg.SelectedSession.ToString();
        }
        dlg.Dispose();
        return result;
      }
    }

    public void Import()
    {
      importResult = new ImportResult();

      if (BeginWork != null) BeginWork(this, new EventArgs());

      XmlTextReader reader = new XmlTextReader(FileName);
      XPathDocument doc = new XPathDocument(reader);
      XPathNavigator nav = doc.CreateNavigator();
      XmlNamespaceManager nsManager = new XmlNamespaceManager(nav.NameTable);
      nsManager.AddNamespace("ns", "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");
      XPathNodeIterator activities = nav.Select("/ns:TrainingCenterDatabase/ns:Activities/ns:Activity", nsManager);

      while (activities.MoveNext())
      {
        XPathNavigator id = activities.Current.SelectSingleNode("ns:Id", nsManager);
        if (id != null && DateTime.Parse(id.Value).ToString("yyyy-MM-dd HH:mm:ss") == IdToImport)
        {
          // the activity was found

          // the laps
          XPathNodeIterator lapNodes = activities.Current.Select("ns:Lap", nsManager);
          List<RouteSegment> routeSegments = new List<RouteSegment>();
          RouteSegment routeSegment = new RouteSegment();
          while (lapNodes.MoveNext())
          {
            // the tracks
            XPathNodeIterator trackNodes = lapNodes.Current.Select("ns:Track", nsManager);
            int trackCount = 0;
            while (trackNodes.MoveNext())
            {
              if (trackCount > 0)
              {
                if (routeSegment.Waypoints.Count > 1) routeSegments.Add(routeSegment);
                routeSegment = new RouteSegment();
              }
              XPathNodeIterator trackpointNodes = trackNodes.Current.Select("ns:Trackpoint", nsManager);
              DateTime lastTime = DateTime.MinValue;
              LongLat lastLongLat = null;
              int trackpointCount = 0;
              while (trackpointNodes.MoveNext())
              {
                Waypoint waypoint = new Waypoint();
                waypoint.Time = DateTime.Parse(trackpointNodes.Current.SelectSingleNode("ns:Time", nsManager).Value);
                XPathNavigator position = trackpointNodes.Current.SelectSingleNode("ns:Position", nsManager);
                if (position != null)
                {
                  waypoint.LongLat = new LongLat(
                    position.SelectSingleNode("ns:LongitudeDegrees", nsManager).ValueAsDouble,
                    position.SelectSingleNode("ns:LatitudeDegrees", nsManager).ValueAsDouble);
                }
                if (trackpointNodes.Current.SelectSingleNode("ns:AltitudeMeters", nsManager) != null)
                  waypoint.Altitude =
                    trackpointNodes.Current.SelectSingleNode("ns:AltitudeMeters", nsManager).ValueAsDouble;
                if (trackpointNodes.Current.SelectSingleNode("ns:HeartRateBpm/ns:Value", nsManager) != null)
                  waypoint.HeartRate =
                    trackpointNodes.Current.SelectSingleNode("ns:HeartRateBpm/ns:Value", nsManager).ValueAsDouble;

                // do not add waypoint if it has the same location or time as the previous one
                if (waypoint.LongLat != null && !waypoint.LongLat.Equals(lastLongLat) && waypoint.Time != lastTime)
                {
                  // special handling for positionless trackpoint in the beginning, use its time together with next position
                  if (trackpointCount == 1 && lastLongLat == null && routeSegment.Waypoints.Count == 0)
                    waypoint.Time = lastTime;
                  routeSegment.Waypoints.Add(waypoint);
                }
                lastLongLat = waypoint.LongLat;
                lastTime = waypoint.Time;
                trackpointCount++;
              }
              if (lastLongLat == null && routeSegment.Waypoints.Count > 1)
              {
                // special handling for positionless trackpoint in the end, use its time together with previous position
                routeSegment.Waypoints[routeSegment.Waypoints.Count - 1].Time = lastTime;
              }
              trackCount++;
            }
          }

          // add last route segment
          if (routeSegment.Waypoints.Count > 1) routeSegments.Add(routeSegment);

          // set position of all start and end waypoints of the route segments if they are null
          foreach (RouteSegment rs in routeSegments)
          {
            if (rs.FirstWaypoint.LongLat == null && rs.Waypoints.Count > 1) rs.Waypoints[1] = rs.Waypoints[1].Clone();
            if (rs.LastWaypoint.LongLat == null && rs.Waypoints.Count > 1)
              rs.Waypoints[rs.Waypoints.Count - 1] = rs.Waypoints[rs.Waypoints.Count - 2].Clone();
          }


          // the laps
          lapNodes = activities.Current.Select("ns:Lap", nsManager);
          // first store all elapsed times
          List<double> elapsedTimes = new List<double>();
          LapCollection laps = new LapCollection();
          if (lapNodes.MoveNext())
          {
            DateTime startTime = DateTime.Parse(lapNodes.Current.GetAttribute("StartTime", ""));
            double elapsedTime = 0;
            do
            {
              elapsedTimes.Add(elapsedTime);
              elapsedTime += lapNodes.Current.SelectSingleNode("ns:TotalTimeSeconds", nsManager).ValueAsDouble;
            } while (lapNodes.MoveNext());


            laps = RouteImporterUtil.CreateLapsFromElapsedTimes(startTime, elapsedTimes, routeSegments);
          }

          importResult.Route = new Route(routeSegments);
          importResult.Laps = laps;
          importResult.Succeeded = true;

          break;
        }
      }
      reader.Close();
      if (EndWork != null) EndWork(this, new EventArgs());
    }

    private List<object> GetAllActivities()
    {
      List<object> activities = new List<object>();

      XmlTextReader reader = new XmlTextReader(FileName);
      XPathDocument doc = new XPathDocument(reader);
      XPathNavigator nav = doc.CreateNavigator();
      XmlNamespaceManager nsManager = new XmlNamespaceManager(nav.NameTable);
      nsManager.AddNamespace("ns", "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");
      XPathNodeIterator activitiyNodes = nav.Select("/ns:TrainingCenterDatabase/ns:Activities/ns:Activity", nsManager);
      while (activitiyNodes.MoveNext())
      {
        XPathNavigator id = activitiyNodes.Current.SelectSingleNode("ns:Id", nsManager);
        activities.Add(DateTime.Parse(id.Value).ToString("yyyy-MM-dd HH:mm:ss"));
      }
      reader.Close();
      return activities;
    }


    #region IRouteImporter Members

    public event EventHandler<EventArgs> BeginWork;

    public event EventHandler<EventArgs> EndWork;

    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    #endregion
  }
}
