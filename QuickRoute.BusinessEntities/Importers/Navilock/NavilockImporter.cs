using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Importers.Navilock
{
  public class NavilockImporter : IRouteFileImporter
  {
    public event EventHandler<EventArgs> BeginWork;
    public event EventHandler<EventArgs> EndWork;
    public event EventHandler<WorkProgressEventArgs> WorkProgress;
    private ImportResult importResult;

    public ImportResult ImportResult
    {
      get { return importResult; }
      set { importResult = value; }
    }

    public string FileName { get; set; }

    public DialogResult ShowPreImportDialogs()
    {
      return DialogResult.OK;
    }

    public void Import()
    {
      importResult = new ImportResult();
      if (BeginWork != null) BeginWork(this, new EventArgs());

      var laps = new LapCollection();
      var routeSegment = new RouteSegment();

      XmlTextReader reader = new XmlTextReader(FileName);
      XPathDocument doc = new XPathDocument(reader);
      XPathNavigator nav = doc.CreateNavigator();

      // get all trackmaster nodes (contains the laps)
      XPathNodeIterator trackmaster = nav.Select("/NewDataSet/trackmaster");
      // get all trackpoint nodes (contains the waypoints)
      XPathNodeIterator trackPoints = nav.Select("/NewDataSet/TrackPoints");

      DateTime startTime = DateTime.MinValue;
      DateTime endTime = DateTime.MinValue;
      int current = 0;
      int total = trackmaster.Count + trackPoints.Count;  
      while (trackmaster.MoveNext())
      {
        if (startTime == DateTime.MinValue)
        {
          // get start time of session
          XPathNavigator startDateNode = trackmaster.Current.SelectSingleNode("TrackName");
          XPathNavigator startTimeNode = trackmaster.Current.SelectSingleNode("StartTime");
          XPathNavigator durationNode = trackmaster.Current.SelectSingleNode("Duration");
          DateTime d;
          TimeSpan st;
          TimeSpan et;
          DateTime.TryParse(startDateNode.Value, out d);
          TimeSpan.TryParse(startTimeNode.Value, out st);
          TimeSpan.TryParse(durationNode.Value, out et);
          startTime = d.Add(st);
          endTime = startTime.Add(et);
        }
        // add lap time
        XPathNavigator accruedTimeNode = trackmaster.Current.SelectSingleNode("AccruedTime");
        TimeSpan ts;
        TimeSpan.TryParse(accruedTimeNode.Value, out ts);
        DateTime lapTime = startTime.Add(ts);
        if (lapTime != endTime) // don't add finish time to laps at this point
        {
          laps.Add(new Lap(lapTime, LapType.Lap));
        }
        current++;
        if (WorkProgress != null && current % 10 == 0)
        {
          WorkProgress(this, new WorkProgressEventArgs((double)current / total));
        }
      }
      laps.Add(new Lap(startTime, LapType.Start));
      laps.Add(new Lap(endTime, LapType.Stop));


      DateTime currentTime = startTime;
      while (trackPoints.MoveNext())
      {
        XPathNavigator latitudeNode = trackPoints.Current.SelectSingleNode("Latitude");
        XPathNavigator longitudeNode = trackPoints.Current.SelectSingleNode("Longitude");
        XPathNavigator altitudeNode = trackPoints.Current.SelectSingleNode("Altitude");
        XPathNavigator heartRateNode = trackPoints.Current.SelectSingleNode("HeartRate");
        XPathNavigator intervalTimeNode = trackPoints.Current.SelectSingleNode("IntervalTime");
        if (latitudeNode != null && longitudeNode != null && intervalTimeNode != null)
        {
          Waypoint w = new Waypoint();
          w.LongLat = new LongLat(
            Convert.ToDouble(LocalizeDecimalString(longitudeNode.Value)),
            Convert.ToDouble(LocalizeDecimalString(latitudeNode.Value)));
          if (altitudeNode != null) w.Altitude = Convert.ToDouble(LocalizeDecimalString(altitudeNode.Value));
          if (heartRateNode != null) w.HeartRate = Convert.ToDouble(LocalizeDecimalString(heartRateNode.Value));
          currentTime = currentTime.AddSeconds(Convert.ToDouble(LocalizeDecimalString(intervalTimeNode.Value)));
          w.Time = currentTime;
          routeSegment.Waypoints.Add(w);
        }
        current++;
        if (WorkProgress != null && current % 10 == 0)
        {
          WorkProgress(this, new WorkProgressEventArgs((double)current / total));
        }
      }
      reader.Close();

      importResult.Laps = laps;
      importResult.Route = new Route(new List<RouteSegment> {routeSegment});
      importResult.Succeeded = true;

      if (EndWork != null) EndWork(this, new EventArgs());
    }


    private static string LocalizeDecimalString(string s)
    {
      s = s.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
      s = s.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
      return s;
    }

  }
}
