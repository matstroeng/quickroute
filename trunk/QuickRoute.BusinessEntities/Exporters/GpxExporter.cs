using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using QuickRoute.BusinessEntities.Importers.GPX.GPX11;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.Exporters
{
  public class GpxExporter
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

    public GpxExporter(Session session, Stream outputStream)
    {
      this.session = session;
      this.outputStream = outputStream;
    }

    public void Export()
    {
      var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, IndentChars = "  " };
      var writer = XmlWriter.Create(OutputStream, writerSettings);
      var xmlSerializer = new XmlSerializer(typeof(gpx11Type));
      var stNs = "urn:uuid:D0EB2ED5-49B6-44e3-B13C-CF15BE7DD7DD";
      var mrNs = "http://www.matstroeng.se/quickroute/map-reading";
      var ns = new XmlSerializerNamespaces();
      ns.Add("st", stNs);
      if(Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration)) ns.Add("mr", mrNs);
      var xml = new XmlDocument();
      var nfi = new NumberFormatInfo();
      nfi.NumberDecimalSeparator = ".";

      var gpx11 = new gpx11Type();
      gpx11.creator = "QuickRoute";
      gpx11.trk = new trkType[] { new trkType() };

      gpx11.extensions = new extensionsType();
      var extensionElements = new List<XmlElement>();
      // TODO: add map-reading elements if exists
      XmlElement activityElement = xml.CreateElement("st", "activity", stNs);
      extensionElements.Add(activityElement);
      XmlElement heartRateTrackElement = xml.CreateElement("st", "heartRateTrack", stNs);
      activityElement.AppendChild(heartRateTrackElement);

      var trksegs = new List<trksegType>();
      foreach (var rs in Session.Route.Segments)
      {
        var wpts = new List<wptType>();
        foreach (var w in rs.Waypoints)
        {
          var wpt = new wptType();
          wpt.eleSpecified = (w.Altitude != null);
          if (wpt.eleSpecified)
          {
            wpt.ele = (decimal)w.Altitude;
          }
          wpt.lon = (decimal)w.LongLat.Longitude;
          wpt.lat = (decimal)w.LongLat.Latitude;
          wpt.time = w.Time.ToUniversalTime(); // use ToUniversalTime for backwards compatibility, QR <= v2.2 used local time internally
          wpt.timeSpecified = true;
          wpts.Add(wpt);

          if (w.HeartRate.HasValue)
          {
            // add heartrate
            var heartRateElement = xml.CreateElement("st", "heartRate", stNs);
            var timeAttribute = xml.CreateAttribute("time");
            timeAttribute.Value = w.Time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"); // use ToUniversalTime for backwards compatibility, QR <= v2.2 used local time internally
            var bpmAttribute = xml.CreateAttribute("bpm");
            bpmAttribute.Value = w.HeartRate.Value.ToString(nfi);
            heartRateElement.Attributes.Append(timeAttribute);
            heartRateElement.Attributes.Append(bpmAttribute);
            heartRateTrackElement.AppendChild(heartRateElement);
          }
        }
        var trkseg = new trksegType {trkpt = wpts.ToArray()};
        trksegs.Add(trkseg);
      }
      gpx11.trk[0].trkseg = trksegs.ToArray();

      // add laps as GPX st:split
      var splitsElement = xml.CreateElement("st", "splits", stNs);
      // first distances
      foreach (var lap in Session.Laps)
      {
        if (lap.LapType == LapType.Lap)
        {
          var splitElement = xml.CreateElement("st", "split", stNs);
          var distanceAttribute = xml.CreateAttribute("distance");
          distanceAttribute.Value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, Session.Route.GetParameterizedLocationFromTime(lap.Time)).Value.ToString(nfi);
          splitElement.Attributes.Append(distanceAttribute);
          splitsElement.AppendChild(splitElement);
        }
      }
      // then times
      foreach (var lap in Session.Laps)
      {
        if (lap.LapType == LapType.Lap)
        {
          var splitElement = xml.CreateElement("st", "split", stNs);
          var timeAttribute = xml.CreateAttribute("time");
          timeAttribute.Value = lap.Time.Subtract(Session.Route.FirstWaypoint.Time).TotalSeconds.ToString(nfi);
          splitElement.Attributes.Append(timeAttribute);
          splitsElement.AppendChild(splitElement);
        }
      }
      if (splitsElement.ChildNodes.Count > 0) activityElement.AppendChild(splitsElement);

      // add laps as GPX waypoint elements
      var lapWaypoints = new List<wptType>();
      foreach (var lap in Session.Laps)
      {
        if (lap.LapType == LapType.Lap)
        {
          var wpt = new wptType();
          wpt.time = lap.Time;
          wpt.timeSpecified = true;
          var qrWaypoint = session.Route.CreateWaypointFromTime(lap.Time);
          wpt.lat = (decimal)qrWaypoint.LongLat.Latitude;
          wpt.lon = (decimal)qrWaypoint.LongLat.Longitude;
          if(qrWaypoint.Altitude.HasValue)
          {
            wpt.ele = (decimal)qrWaypoint.Altitude.Value;
            wpt.eleSpecified = true;
          }
          wpt.name = string.Format(Strings.LapX, lapWaypoints.Count + 1);
          lapWaypoints.Add(wpt);
        }
      }
      if(lapWaypoints.Count > 0) gpx11.wpt = lapWaypoints.ToArray();

      // map reading
      if(Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration))
      {
        var mapReadingsList = Session.Route.GetMapReadingsList();
        for(var i=0; i<mapReadingsList.Count; i+=2)
        {
          var mapReadingElement = xml.CreateElement("mr", "map-reading", mrNs);
          mapReadingElement.SetAttribute("start", mapReadingsList[i].ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
          mapReadingElement.SetAttribute("end", mapReadingsList[i+1].ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
          extensionElements.Add(mapReadingElement);
        }
      }
      gpx11.extensions.Any = extensionElements.ToArray();

      xmlSerializer.Serialize(writer, gpx11, ns);
      writer.Close();
    }



  }
}
