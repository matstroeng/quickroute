using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Importers.GPX.GPX11;
using QuickRoute.BusinessEntities.Importers.Polar;
using QuickRoute.BusinessEntities.Importers.Polar.ProTrainer;

namespace QuickRoute.BusinessEntities.Importers.GPX
{
  public class GPXImporter : IRouteFileImporter
  {
    private string fileName;
    private ImportResult importResult;

    private const string garminTrackPointExtensionsURI = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";

    public ImportResult ImportResult
    {
      get { return importResult; }
      set { importResult = value; }
    }

    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }

    public DialogResult ShowPreImportDialogs()
    {
      return DialogResult.OK;
    }

    public void Import()
    {
      // A) The GPX 1.0 class generates an error when trying to run the ImportGPX10 method.
      //
      // B) Some GPX 1.0 files out there seem to contain <extension> elements, which they should not.

      // A) and B) made me decide to just replace the schema namespace in GPX 1.0 files from
      // xmlns="http://www.topografix.com/GPX/1/0"
      // to
      // xmlns="http://www.topografix.com/GPX/1/1"
      // and use GPX 1.1 import instead. 

      //if (GPXUtil.GetGPXVersion(fileName) == GPXVersion.GPX10)
      //{
      //  ImportGPX10();
      //}
      //else
      //{
      ImportGPX11();
      //}
    }

    /*
    private void ImportGPX10()
    {
      if (BeginWork != null) BeginWork(this, new EventArgs());

      NumberFormatInfo nfi = new NumberFormatInfo();
      nfi.NumberDecimalSeparator = ".";
      XmlTextReader sr = new XmlTextReader(fileName);
      XmlSerializer xSerializer = new XmlSerializer(typeof(gpx10Type));
      gpx10Type gpx = (gpx10Type)xSerializer.Deserialize(sr);
      sr.Close();
      XmlNamespaceManager nsManager = new XmlNamespaceManager(sr.NameTable);
      nsManager.AddNamespace("st", "urn:uuid:D0EB2ED5-49B6-44e3-B13C-CF15BE7DD7DD");

      importResult = new ImportResult();

      // route
      List<RouteSegment> routeSegments = new List<RouteSegment>();
      foreach (gpxTrk trk in gpx.trk)
      {
        for (int i = trk.trkseg.GetLowerBound(0); i <= trk.trkseg.GetUpperBound(0); i++)
        {
          RouteSegment routeSegment = new RouteSegment();
          for (int j = trk.trkseg.GetLowerBound(1); j <= trk.trkseg.GetUpperBound(1); j++)
          {
            gpxTrkTrksegTrkpt wpt = trk.trkseg[i][j];
            double lat = (double)wpt.lat;
            double lon = (double)wpt.lon;
            DateTime time = wpt.time;
            double? heartRate = null;
            double? altitude = null;
            routeSegment.Waypoints.Add(new Waypoint(time, new LongLat(lon, lat), altitude, heartRate));
          }
          routeSegments.Add(routeSegment);
        }
      }
      importResult.Route = new Route(routeSegments);

      // laps
      LapCollection laps = new LapCollection();
      importResult.Laps = laps;

      if (EndWork != null) EndWork(this, new EventArgs());
    }
    */

    private void ImportGPX11()
    {
      ImportResult = new ImportResult();
      var isGPX10 = (GPXUtil.GetGPXVersion(fileName) == GPXVersion.GPX10);
      string gpx10convertedFileName = null;
      string polarConvertedFileName = null;
      var originalFileName = fileName;
      if (BeginWork != null) BeginWork(this, new EventArgs());

      // check if the file is in gpx 1.0 format and convert it to gpx 1.1 if necessary
      if (isGPX10)
      {
        gpx10convertedFileName = Path.GetTempFileName();
        GPXUtil.ConvertGPX10ToGPX11(fileName, gpx10convertedFileName);
        fileName = gpx10convertedFileName;
      }

      // check if the file is an invalid Polar ProTrainer file and correct if necessary
      if (PolarProTrainerUtil.IsPolarProTrainerGPXFile(fileName))
      {
        polarConvertedFileName = Path.GetTempFileName();
        PolarProTrainerUtil.CorrectPolarProTrainerGPXFile(fileName, polarConvertedFileName);
        fileName = polarConvertedFileName;
      }

      var nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
      var sr = new XmlTextReader(fileName);
      var xSerializer = new XmlSerializer(typeof(gpx11Type));
      var gpx11 = (gpx11Type)xSerializer.Deserialize(sr);
      sr.Close();
      var nsManager = new XmlNamespaceManager(sr.NameTable);
      // add namespace for split times and heart rates (from SportsTracks software)
      nsManager.AddNamespace("st", "urn:uuid:D0EB2ED5-49B6-44e3-B13C-CF15BE7DD7DD");
      // add namespace for map reading information (QuickRoute native)
      nsManager.AddNamespace("mr", "http://www.matstroeng.se/quickroute/map-reading");

      // pre-store heart rates in dictionary (if present)
      var heartRates = new Dictionary<DateTime, double>();
      // pre-store map-readings in map reading collection (if present)
      var mapReadings = new List<DateTime>();
      if (gpx11.extensions != null && gpx11.extensions.Any != null)
      {
        foreach (var element in gpx11.extensions.Any)
        {
          if (element.Name == "st:activity")
          {
            var nodes = element.SelectNodes("st:heartRateTrack/st:heartRate[@time and @bpm]", nsManager);
            if (nodes != null)
            {
              foreach (XmlNode node in nodes)
              {
                DateTime time;
                double bpm;
                if (DateTime.TryParse(node.Attributes["time"].Value, out time) &&
                    double.TryParse(node.Attributes["bpm"].Value, NumberStyles.Any, nfi, out bpm))
                {
                  time = time.ToUniversalTime();
                  if(!heartRates.ContainsKey(time)) heartRates.Add(time, bpm);
                }
              }
            }
          }
          if(element.Name == "mr:map-reading")
          {
            DateTime start, end;
            if (DateTime.TryParse(element.Attributes["start"].Value, out start) &&
                DateTime.TryParse(element.Attributes["end"].Value, out end))
            {
              mapReadings.Add(start.ToUniversalTime());
              mapReadings.Add(end.ToUniversalTime());
            }
          }
        }
      }
      mapReadings = FilterMapReadings(mapReadings);

      // QuickRoute route
      var noOfWaypoints = 0;
      var noOfWaypointsWithTimes = 0;
      var routeSegments = new List<RouteSegment>();

      // first use GPX track
      if (gpx11.trk != null)
      {
        foreach (var trk in gpx11.trk)
        {
          // Garmin Training Center exports each lap as a separate trkseg with end time of trkseg n equal to start time of trkseg n+1
          // handle this issue

          foreach (var trkseg in trk.trkseg)
          {
            var routeSegment = new RouteSegment();
            wptType lastWpt = null;
            if (trkseg.trkpt != null)
            {
              foreach (var wpt in trkseg.trkpt)
              {
                if (lastWpt == null || wpt.time != lastWpt.time)
                {
                  if (wpt.extensions != null && wpt.extensions.Any[0].LocalName == "timerPaused")
                  {
                    // new route segment ahead
                    if (routeSegment.Waypoints.Count > 0) routeSegments.Add(routeSegment);
                    routeSegment = new RouteSegment();
                  }
                  else
                  {
                    var lat = (double)wpt.lat;
                    var lon = (double)wpt.lon;
                    double? heartRate = null;
                    double? altitude = null;
                    // check for heartrate in SportsTracks extensions
                    if (heartRates.ContainsKey(wpt.time)) heartRate = heartRates[wpt.time];
                    // check for heartrate in Garmin Trackpoint Extensions
                    heartRate = GetGarminHeartRateFromWaypoint(wpt) ?? heartRate;

                    if (wpt.eleSpecified)
                    {
                      altitude = (double?)wpt.ele;
                    }
                    if (wpt.timeSpecified)
                    {
                      routeSegment.Waypoints.Add(new Waypoint(wpt.time, new LongLat(lon, lat), altitude, heartRate, null));
                      noOfWaypointsWithTimes++;
                      lastWpt = wpt;
                    }
                  }
                  noOfWaypoints++;
                }
              }
            }
            if (routeSegment.Waypoints.Count > 0) routeSegments.Add(routeSegment);
          }
        }
      }

      // if no GPX track - use GPX route
      if (noOfWaypointsWithTimes == 0 && gpx11.rte != null)
      {
        foreach (var route in gpx11.rte)
        {
          var routeSegment = new RouteSegment();
          foreach (var rtept in route.rtept)
          {
            if (rtept.extensions != null && rtept.extensions.Any[0].LocalName == "timerPaused")
            {
              // new route segment ahead
              if (routeSegment.Waypoints.Count > 0) routeSegments.Add(routeSegment);
              routeSegment = new RouteSegment();
            }
            else
            {
              var lat = (double) rtept.lat;
              var lon = (double) rtept.lon;
              double? heartRate = null;
              double? altitude = null;
              if (heartRates.ContainsKey(rtept.time)) heartRate = heartRates[rtept.time];
              if (rtept.eleSpecified)
              {
                altitude = (double?) rtept.ele;
              }
              if (rtept.timeSpecified)
              {
                routeSegment.Waypoints.Add(new Waypoint(rtept.time, new LongLat(lon, lat), altitude, heartRate, null));
                noOfWaypointsWithTimes++;
              }
            }
            noOfWaypoints++;
          }
          if (routeSegment.Waypoints.Count > 0) routeSegments.Add(routeSegment);
        }
      }

      // add map reading waypoints
      routeSegments = Route.AddMapReadingWaypoints(routeSegments, mapReadings);

      // concat route segments if they are close enough (oddly enough, Garmin Training Center v2 seems to create a trkseg for each lap)
      var lapsFromConcatenatedRouteSegments = new List<Lap>();
      if (routeSegments.Count > 0)
      {
        var concatenatedRouteSegments = new List<RouteSegment>() { routeSegments[0] };
        for (var i = 1; i < routeSegments.Count; i++)
        {
          if (concatenatedRouteSegments[concatenatedRouteSegments.Count - 1].LastWaypoint.Time.AddSeconds(10) > routeSegments[i].FirstWaypoint.Time)
          {
            lapsFromConcatenatedRouteSegments.Add(new Lap(concatenatedRouteSegments[concatenatedRouteSegments.Count - 1].LastWaypoint.Time, LapType.Lap));
            concatenatedRouteSegments[concatenatedRouteSegments.Count - 1].Waypoints.AddRange(routeSegments[i].Waypoints);
          }
        }
        routeSegments = concatenatedRouteSegments;
      }

      importResult.Succeeded = (noOfWaypointsWithTimes > 0);

      if (ImportResult.Succeeded)
      {
        importResult.Route = new Route(routeSegments);

        // laps
        var laps = new LapCollection();
        var startTime = ImportResult.Route.FirstWaypoint.Time;

        // from GPX st:split
        if (gpx11.extensions != null && gpx11.extensions.Any != null)
        {
          foreach (var element in gpx11.extensions.Any)
          {
            if (element.Name == "st:activity")
            {
              var nodes = element.SelectNodes("st:splits/st:split[@time]", nsManager);
              if (nodes != null)
              {
                foreach (XmlNode node in nodes)
                {
                  var elapsedTime = double.Parse(node.Attributes["time"].Value, nfi);
                  var lap = new Lap(startTime.AddSeconds(elapsedTime), LapType.Lap);
                  laps.Add(lap);
                }
              }
            }
          }
        }

        // from GPX waypoints
        if (gpx11.wpt != null && laps.Count == 0)
        {
          foreach (var waypoint in gpx11.wpt)
          {
            if (waypoint.timeSpecified)
            {
              laps.Add(new Lap(waypoint.time, LapType.Lap));
            }
          }
        }

        laps.AddRange(lapsFromConcatenatedRouteSegments);

        foreach (var rs in routeSegments)
        {
          laps.Add(new Lap(rs.FirstWaypoint.Time, LapType.Start));
          laps.Add(new Lap(rs.LastWaypoint.Time, LapType.Stop));
        }
        importResult.Laps = laps;
      }
      else
      {
        if (noOfWaypoints == 0)
        {
          importResult.Error = ImportError.NoWaypoints;
        }
        else if (noOfWaypointsWithTimes == 0)
        {
          importResult.Error = ImportError.NoWaypointTimes;
        }
      }

      if (gpx10convertedFileName != null)
      {
        File.Delete(gpx10convertedFileName);
        fileName = originalFileName;
      }

      if (polarConvertedFileName != null)
      {
        File.Delete(polarConvertedFileName);
        fileName = originalFileName;
      }

      // import Polar HRM file with same base file name as the gpx file, if existing
      var extension = new FileInfo(fileName).Extension;
      if(extension != "") 
      {
        string hrmFileName = new FileInfo(fileName).FullName.Replace(extension, ".hrm");
        if(File.Exists(hrmFileName))
        {
          new PolarHRMImporter().AddLapsAndHRData(hrmFileName, importResult);
        }
      }

      if (EndWork != null) EndWork(this, new EventArgs());
    }

    private static double? GetGarminHeartRateFromWaypoint(wptType wpt)
    {
      if (wpt.extensions != null)
      {
        foreach (var element in wpt.extensions.Any)
        {
          if (element.NamespaceURI == garminTrackPointExtensionsURI && element.LocalName == "TrackPointExtension")
          {
            foreach (XmlNode subElement in element.GetElementsByTagName("hr", garminTrackPointExtensionsURI))
            {
              double hr;
              if (double.TryParse(subElement.InnerText, out hr)) return hr;
            }
          }
        }
      }
      return null;
    }

    private static List<DateTime> FilterMapReadings(List<DateTime> mapReadings)
    {
      var filteredMapReadings = new List<DateTime>();
      for(var i=0; i<mapReadings.Count; i+=2)
      {
        if (mapReadings[i] < mapReadings[i + 1])
        {
          // only add non-zero-length intervals
          if (filteredMapReadings.Count == 0 || mapReadings[i] > filteredMapReadings[filteredMapReadings.Count - 1])
          {
            // simplest case
            filteredMapReadings.Add(mapReadings[i]);
            filteredMapReadings.Add(mapReadings[i + 1]);
          }
          else
          {
            // tricky case, loop through list
            // not very efficient, but will happen quite seldom so it should be fine
            var j = 0;
            while(j < filteredMapReadings.Count)
            {
              if (mapReadings[i] < filteredMapReadings[j])
              {
                filteredMapReadings[j] = mapReadings[i];
              }
              if(mapReadings[i] <= filteredMapReadings[j+1])
              {
                while(mapReadings[i+1] >= filteredMapReadings[j+1])
                {
                  filteredMapReadings.RemoveAt(j+1);
                }
                if(filteredMapReadings.Count % 2 == 1)
                {
                  filteredMapReadings.Insert(j + 1, mapReadings[i + 1]);
                }
                break;
              }
              j += 2;
            }
          }
        }
      }
      return filteredMapReadings;
    }

    #region IRouteImporter Members

    public event EventHandler<EventArgs> BeginWork;

    public event EventHandler<EventArgs> EndWork;

    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    #endregion
  }

}
