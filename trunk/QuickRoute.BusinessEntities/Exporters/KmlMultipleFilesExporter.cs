using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using QuickRoute.BusinessEntities.Exporters;
using QuickRoute.Common;

namespace QuickRoute.BusinessEntities.Exporters
{
  public class KmlMultipleFilesExporter
  {
    public Document Document { get; private set; }
    public SessionCollection Sessions { get; private set; }
    public KmlProperties KmlProperties { get; private set; }
    public KmlMultipleFileExporterProperties MultipleFileExporterProperties { get; set; }
    public List<string> InvalidFileNames { get; private set; }

    public KmlMultipleFilesExporter(IEnumerable<string> fileNames, KmlMultipleFileExporterProperties multipleFileProperties)
    {
      MultipleFileExporterProperties = multipleFileProperties;
      KmlProperties = new KmlProperties()
      {
        MapType = KmlExportMapType.Map,
        RouteType = (multipleFileProperties.IncludeRoutes ? KmlExportRouteType.Monochrome : KmlExportRouteType.None),
        ReplayType = (multipleFileProperties.IncludeReplay ? KmlExportReplayType.Monochrome : KmlExportReplayType.None),
        ReplayTimeInterval = multipleFileProperties.ReplayTimeInterval,
        ReplayTails = multipleFileProperties.ReplayTails,
        RouteLineStyle = new KmlLineStyle() { Width = multipleFileProperties.RouteLineWidth },
        ReplayMarkerStyle = new KmlMarkerStyle() { Size = 3 * multipleFileProperties.RouteLineWidth }
      };

      var massStartTime = DateTime.MaxValue;
      var allMapsHaveTheSameSize = true;
      Sessions = new SessionCollection();
      InvalidFileNames = new List<string>();
      foreach (var fileName in fileNames)
      {
        var d = Document.Open(fileName);
        if (d != null)
        {
          if (Document == null)
          {
            // first file name sets document
            Document = d;
          }
          if (d.Map.Image.Size != Document.Map.Image.Size) allMapsHaveTheSameSize = false;
          var s = d.Sessions[0];
          if (multipleFileProperties.IncludeReplay && s.Route.FirstWaypoint.Time < massStartTime) massStartTime = s.Route.FirstWaypoint.Time;
          Sessions.Add(s);
        }
        else
        {
          InvalidFileNames.Add(fileName);
        }
      }
      // if all map images have the same size, then assume that the very same map image is used and adjust the route to the document map
      // otherwise, adjust the route to the individual session map
      // todo: make it possible to choose KmlRouteAdaptationStyle.NoAdjustment in the GUI
      KmlProperties.RouteAdaptionStyle = (allMapsHaveTheSameSize
                                            ? KmlRouteAdaptationStyle.AdaptToDocumentMapImage
                                            : KmlRouteAdaptationStyle.AdaptToSessionMapImage);

      if (multipleFileProperties.IncludeReplay && multipleFileProperties.MassStart)
      {
        // adjust to mass start
        foreach (var s in Sessions)
        {
          s.AddTimeOffset(massStartTime.Subtract(s.Route.FirstWaypoint.Time));
        }

        // adjust routes to restart after each lap if necessary
        if (multipleFileProperties.ReplayRestartAfterEachLap)
        {
          var maxTimeDurations = GetMaxLapTimeDurations(Sessions);
          foreach (var s in Sessions)
          {
            Lap previousLap = null;
            var count = 0;

            // 1. Caclulate the durations and place in separate variable (since the session will be altered as time passes by)
            var durations = new List<TimeSpan>();
            foreach (var lap in s.Laps)
            {
              if (previousLap != null && (lap.LapType == LapType.Lap || lap.LapType == LapType.Stop))
              {
                durations.Add(lap.Time - previousLap.Time);
                count++;
              }
              previousLap = lap;
            }

            // 2. Add idle time
            previousLap = null;
            count = 0;
            foreach (var lap in s.Laps)
            {
              if (previousLap != null && (lap.LapType == LapType.Lap || lap.LapType == LapType.Stop))
              {
                if (durations[count] < maxTimeDurations[count])
                {
                  var timeToAdd = maxTimeDurations[count] - durations[count];
                  s.InsertIdleTime(lap.Time, timeToAdd);
                }
                count++;
              }
              previousLap = lap;
            }
          }
        }
      }
    }

    public void SetRouteLineSettingsForSession(Session session, Color color)
    {
      var rls = session.Settings.RouteLineSettingsCollection[WaypointAttribute.Pace];
      rls.MonochromeWidth = KmlProperties.RouteLineStyle.Width;
      rls.MonochromeColor = color;
    }

    public void Export(Stream stream)
    {
      // crrete kml exporter object
      var imageExporter = new ImageExporter(Document)
                            {
                              Properties = new ImageExporterProperties()
                            };

      var kmlExporter = new KmlExporter(Document, imageExporter, Sessions, stream)
                          {
                            KmlProperties = KmlProperties
                          };

      // set individual colors for the sessions
      var count = 0;
      foreach (var s in Sessions)
      {
        // create custom route line style for this session
        kmlExporter.RouteLineStyleForSessions[s] = new KmlLineStyle()
                                                     {
                                                       Color = MultipleFileExporterProperties.Colors[count % MultipleFileExporterProperties.Colors.Count],
                                                       Width = KmlProperties.RouteLineStyle.Width
                                                     };
        if (MultipleFileExporterProperties.IncludeReplay)
        {
          // create custom replay marker style for this session
          kmlExporter.ReplayMarkerStyleForSessions[s] = new KmlMarkerStyle()
          {
            Color = MultipleFileExporterProperties.Colors[count % MultipleFileExporterProperties.Colors.Count],
            Size = KmlProperties.ReplayMarkerStyle.Size
          };
        }
        count++;
      }

      // export
      kmlExporter.ExportKmz(CommonUtil.GetTempFileName() + @"\");
    }

    private static List<TimeSpan> GetMaxLapTimeDurations(IEnumerable<Session> sessions)
    {
      var maxLapTimeDurations = new List<TimeSpan>();
      foreach (var session in sessions)
      {
        var count = 0;
        Lap previousLap = null;
        foreach (var lap in session.Laps)
        {
          if (previousLap != null && (lap.LapType == LapType.Lap || lap.LapType == LapType.Stop))
          {
            var duration = lap.Time - previousLap.Time;
            if (maxLapTimeDurations.Count <= count)
            {
              maxLapTimeDurations.Add(duration);
            }
            else if (maxLapTimeDurations[count] < duration)
            {
              maxLapTimeDurations[count] = duration;
            }
            count++;
          }
          previousLap = lap;
        }
      }
      return maxLapTimeDurations;
    }

  }

  [Serializable]
  public class KmlMultipleFileExporterProperties : ICloneable
  {
    public bool IncludeRoutes { get; set; }
    public bool IncludeReplay { get; set; }
    public bool MassStart { get; set; }
    public bool CompactRouteSegments { get; set; }
    public List<Color> Colors { get; set; }
    public TimeSpan ReplayTimeInterval { get; set; }
    public List<KmlReplayTail> ReplayTails { get; set; }
    public double RouteLineWidth { get; set; }
    /// <summary>
    /// Gets or sets whether a replay with mass start should restart the masstart after each lap.
    /// </summary>
    public bool ReplayRestartAfterEachLap { get; set; }


    public KmlMultipleFileExporterProperties()
    {
      IncludeRoutes = true;
      MassStart = true;
      CompactRouteSegments = true;
      Colors = new List<Color>
                 {
                   Color.FromArgb(160, Color.Red),
                   Color.FromArgb(160, Color.Blue),
                   Color.FromArgb(160, Color.DarkGreen),
                   Color.FromArgb(160, Color.Purple),
                   Color.FromArgb(160, Color.DarkOrange),
                   Color.FromArgb(160, Color.Cyan),
                   Color.FromArgb(160, Color.Lime),
                   Color.FromArgb(160, Color.Chocolate)
                 };
      ReplayTimeInterval = new TimeSpan(0, 0, 5);
      ReplayTails = new List<KmlReplayTail> { new KmlReplayTail() { StartVisible = new TimeSpan(0), EndVisible = new TimeSpan(0, 0, 60) } };
      RouteLineWidth = 6;
    }

    public bool HasReplayTails
    {
      get { return ReplayTails.Count > 0; }
    }

    public object Clone()
    {
      var clone = new KmlMultipleFileExporterProperties
                    {
                      IncludeRoutes = IncludeRoutes,
                      IncludeReplay = IncludeReplay,
                      MassStart = MassStart,
                      CompactRouteSegments = CompactRouteSegments,
                      Colors = new List<Color>(Colors),
                      ReplayTimeInterval = ReplayTimeInterval,
                      RouteLineWidth = RouteLineWidth,
                      ReplayTails = new List<KmlReplayTail>(),
                      ReplayRestartAfterEachLap = ReplayRestartAfterEachLap
                    };

      foreach (var tail in ReplayTails)
      {
        clone.ReplayTails.Add(tail.Clone() as KmlReplayTail);
      }
      return clone;
    }
  }
}