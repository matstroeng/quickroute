using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using QuickRoute.BusinessEntities.RouteProperties;

namespace QuickRoute.BusinessEntities.Exporters
{
  /// <summary>
  /// Creates an xml file with the route property data of all route segments in a session.
  /// </summary>
  public class RouteDataExporter
  {
    private readonly Session session;
    private readonly Stream outputStream;

    public Session Session
    {
      get { return session; }
    }

    public Stream OutputStream
    {
      get { return outputStream; }
    }

    public ExportRouteDataSettings Settings { get; set; }

    public RouteDataExporter(Session session, Stream outputStream)
    {
      this.session = session;
      this.outputStream = outputStream;
      Settings = new ExportRouteDataSettings();
    }

    public void Export()
    {
      var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, IndentChars = "  " };
      var writer = XmlWriter.Create(OutputStream, writerSettings);
      var formatter = new Formatter();
      var cacheManager = new RoutePropertyCacheManager();

      if (writer != null)
      {
        writer.WriteStartDocument();
        writer.WriteStartElement("Route");
        int segmentCount = 0;
        foreach (var segment in Session.Route.Segments)
        {
          writer.WriteStartElement("Segment");
          var waypoints = Session.Route.GetEquallySpacedWaypoints(segment.FirstWaypoint.Time, segment.LastWaypoint.Time, Settings.SamplingInterval, Settings.ZeroTime);
          var lastPl = new ParameterizedLocation(segmentCount, 0);

          foreach (var waypoint in waypoints)
          {
            writer.WriteStartElement("Sample");
            var pl = Session.Route.GetParameterizedLocationFromTime(waypoint.Time, lastPl, ParameterizedLocation.Direction.Forward);
            var locations = new RouteLocations(pl);
            foreach (var lpType in Settings.RoutePropertyTypes)
            {
              if (lpType.Selected)
              {
                RetrieveExternalPropertyDelegate dlg = new ExternalRoutePropertyRetriever(Session.Settings).RetrieveExternalProperty;
                var lp = Activator.CreateInstance(lpType.RoutePropertyType, Session, locations, dlg) as RouteProperty;

                if (lp != null)
                {
                  lp.CacheManager = cacheManager;
                  if (lp.ContainsValue)
                  {
                    var attributeName = GetCamelCaseString(lpType.RoutePropertyType.Name);
                    var attributeValue = lp.ToString(formatter);
                    writer.WriteAttributeString(attributeName, attributeValue);
                  }
                }
              }
            }

            writer.WriteEndElement();
            lastPl = pl;
          }
          writer.WriteEndElement();
          segmentCount++;
        }
        writer.WriteEndElement();
        writer.Close();
      }
    }

    private static string GetCamelCaseString(string s)
    {
      if (string.IsNullOrEmpty(s)) return s;
      return s.Substring(0, 1).ToLower() + s.Substring(1);
    }

    private class Formatter : IFormatProvider, ICustomFormatter
    {
      private static readonly IFormatProvider nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

      public string Format(string format, object arg, IFormatProvider formatProvider)
      {
        if (arg is double? && !((double?)arg).HasValue) return "-";
        if (arg is double || arg is double?)
        {
          return ((double)arg).ToString(nfi);
        }

        if (arg is DateTime)
        {
          var dt = (DateTime)arg;
          var timeString = dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
          return timeString.Replace("Z", dt.ToString(".fffffff").TrimEnd("0".ToCharArray()).TrimEnd(".".ToCharArray()) + "Z");
        }

        if (arg is TimeSpan)
        {
          var ts = (TimeSpan)arg;
          return Format(format, ts.TotalSeconds, formatProvider);
        }

        // format everything else normally
        if (arg is IFormattable)
        {
          return ((IFormattable)arg).ToString(format, formatProvider);
        }
        return arg.ToString();
      }

      public object GetFormat(Type formatType)
      {
        return (formatType == typeof(ICustomFormatter)) ? this : null;
      }
    }
  }

  [Serializable]
  public class ExportRouteDataSettings
  {
    public ExportRouteDataSettings()
    {
      ZeroTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      SamplingInterval = new TimeSpan(0, 0, 1);
    }

    private static SelectableRoutePropertyTypeCollection AvailableRoutePropertyTypes
    {
      get
      {
        return new SelectableRoutePropertyTypeCollection()
              {
                new SelectableRoutePropertyType(typeof (Time), true),
                new SelectableRoutePropertyType(typeof (Longitude), true),
                new SelectableRoutePropertyType(typeof (Latitude), true),
                new SelectableRoutePropertyType(typeof (ElapsedTimeFromStart), true),
                new SelectableRoutePropertyType(typeof (CircleTimeBackward), true),
                new SelectableRoutePropertyType(typeof (CircleTimeForward), true),
                new SelectableRoutePropertyType(typeof (RouteDistanceFromStart), true),
                new SelectableRoutePropertyType(typeof (Pace), true),
                new SelectableRoutePropertyType(typeof (Speed), true),
                new SelectableRoutePropertyType(typeof (RouteProperties.HeartRate), true),
                new SelectableRoutePropertyType(typeof (RouteProperties.Altitude), true),
                new SelectableRoutePropertyType(typeof (DirectionDeviationToNextLap), true),
                new SelectableRoutePropertyType(typeof (Direction), true),                               
                new SelectableRoutePropertyType(typeof (Inclination), true),
                new SelectableRoutePropertyType(typeof (AscentFromStart), true),
                new SelectableRoutePropertyType(typeof (DescentFromStart), true),
                new SelectableRoutePropertyType(typeof (ImageX), true),
                new SelectableRoutePropertyType(typeof (ImageY), true),
                new SelectableRoutePropertyType(typeof (LapNumber), false),
                new SelectableRoutePropertyType(typeof (RouteProperties.ElapsedTime), false), 
                new SelectableRoutePropertyType(typeof (RouteDistance), false),
                new SelectableRoutePropertyType(typeof (StraightLineDistance), false), 
                new SelectableRoutePropertyType(typeof (StraightLineDistanceFromStart), false),
                new SelectableRoutePropertyType(typeof (RouteToStraightLine), false),
                new SelectableRoutePropertyType(typeof (RouteToStraightLineFromStart), false),
                new SelectableRoutePropertyType(typeof (AverageStraightLinePace), false),
                new SelectableRoutePropertyType(typeof (AverageStraightLinePaceFromStart), false),
                new SelectableRoutePropertyType(typeof (AverageRoutePace), false),
                new SelectableRoutePropertyType(typeof (AverageRoutePaceFromStart), false),
                new SelectableRoutePropertyType(typeof (AverageStraightLineSpeed), false),
                new SelectableRoutePropertyType(typeof (AverageStraightLineSpeedFromStart), false),
                new SelectableRoutePropertyType(typeof (AverageRouteSpeed), false),
                new SelectableRoutePropertyType(typeof (AverageRouteSpeedFromStart), false),
                new SelectableRoutePropertyType(typeof (AverageHeartRate), false),
                new SelectableRoutePropertyType(typeof (AverageHeartRateFromStart), false),
                new SelectableRoutePropertyType(typeof (MinHeartRate), false),
                new SelectableRoutePropertyType(typeof (MinHeartRateFromStart), false),
                new SelectableRoutePropertyType(typeof (MaxHeartRate), false),
                new SelectableRoutePropertyType(typeof (MaxHeartRateFromStart), false),
                new SelectableRoutePropertyType(typeof (AltitudeDifference), false),
                new SelectableRoutePropertyType(typeof (AltitudeDifferenceFromStart), false),
                new SelectableRoutePropertyType(typeof (Ascent), false),
                new SelectableRoutePropertyType(typeof (Descent), false),
                new SelectableRoutePropertyType(typeof (AverageInclination), false),
                new SelectableRoutePropertyType(typeof (AverageInclinationFromStart), false),
                new SelectableRoutePropertyType(typeof (AverageDirectionDeviationToNextLap), false),
                new SelectableRoutePropertyType(typeof (AverageDirectionDeviationToNextLapFromStart), false),
                new SelectableRoutePropertyType(typeof (Location), false),
                new SelectableRoutePropertyType(typeof (RouteProperties.MapReadingState), false),
                new SelectableRoutePropertyType(typeof (MapReadingDuration), false),
                new SelectableRoutePropertyType(typeof (AveragePaceWhenReadingMap), false),
                new SelectableRoutePropertyType(typeof (AveragePaceWhenReadingMapFromStart), false),
                new SelectableRoutePropertyType(typeof (AveragePaceWhenNotReadingMap), false),
                new SelectableRoutePropertyType(typeof (AveragePaceWhenNotReadingMapFromStart), false),
                new SelectableRoutePropertyType(typeof (PreviousMapReadingEnd), false),
                new SelectableRoutePropertyType(typeof (NextMapReadingStart), false),
              };
      }
    }

    private SelectableRoutePropertyTypeCollection routePropertyTypes;

    /// <summary>
    /// The time (eg midnight) that will be used as a reference time for the sampling interval.
    /// </summary>
    public DateTime ZeroTime { get; set; }

    /// <summary>
    /// The time between each pixel coordinate in the file
    /// </summary>
    public TimeSpan SamplingInterval { get; set; }

    [Obsolete("Replaced by RoutePropertyTypes, left for not breaking the serialization contract.")]
    public RouteDataExportAttributes IncludedAttributes { get; set; }

    /// <summary>
    /// The route property types that should be included when exporting data.
    /// </summary>
    public SelectableRoutePropertyTypeCollection RoutePropertyTypes
    {
      get
      {
        if (routePropertyTypes == null)
        {
          routePropertyTypes = AvailableRoutePropertyTypes;
        }
        else if (routePropertyTypes.Count < AvailableRoutePropertyTypes.Count)
        {
          // make sure new property types are propagated to the settings
          foreach (var rpt in AvailableRoutePropertyTypes)
          {
            if (!routePropertyTypes.ContainsRoutePropertyType(rpt.RoutePropertyType))
            {
              routePropertyTypes.Add(rpt);
            }
          }
        }
        return routePropertyTypes;
      }
      set
      {
        routePropertyTypes = value;
      }
    }
  }

  [Flags]
  [Serializable]
  [Obsolete]
  public enum RouteDataExportAttributes
  {
    None = 0,
    Time = 1,
    Longitude = 2,
    Latitude = 4,
    PixelX = 8,
    PixelY = 16,
    ElapsedTime = 32,
    Distance = 64,
    Pace = 128,
    Speed = 256,
    DirectionDeviation = 512,
    Altitude = 1024,
    HeartRate = 2048
  }

}
