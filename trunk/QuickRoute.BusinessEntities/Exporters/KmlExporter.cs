using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zip;
using QuickRoute.BusinessEntities;
using QuickRoute.Common;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.Exporters
{
  /// <summary>
  /// Class for creating kml/kmz files from QuickRoute documents and sessions
  /// </summary>
  public class KmlExporter
  {
    private readonly List<KmlExportDocument> kmlExportDocuments;
    private readonly Stream outputStream;

    /// <summary>
    /// The stream to write the kml/kmz document to
    /// </summary>
    public Stream OutputStream
    {
      get { return outputStream; }
    }
    /// <summary>
    /// The properties used to create the kml/kmz file
    /// </summary>
    public KmlProperties KmlProperties { get; set; }
    private Dictionary<KmlExportDocument, KmlGroundOverlay> GroundOverlays { get; set; }
    private ZipFile kmzFile;
    private string temporaryBasePath;
    private int iconStyleCount;
    private readonly Dictionary<KmlLineStyle, string> routeLineStyles = new Dictionary<KmlLineStyle, string>();
    private readonly Dictionary<KmlMarkerStyle, string> markerStyles = new Dictionary<KmlMarkerStyle, string>();
    /// <summary>
    /// Route line styles for each individual session when in monochrome route line style mode. If not set, the KmlProperties.RouteLineStyle value is used.
    /// </summary>
    public Dictionary<Session, KmlLineStyle> RouteLineStyleForSessions { get; set; }
    /// <summary>
    /// Replay marker styles for each individual session when in monochrome replay marker style mode. If not set, the KmlProperties.ReplayMarkerStyle value is used.
    /// </summary>
    public Dictionary<Session, KmlMarkerStyle> ReplayMarkerStyleForSessions { get; set; }
    /// <summary>
    /// The number of possible colors when route line or replay markers are color-coded
    /// </summary>
    public int NoOfColorCodedSteps { get; set; }

    private readonly Dictionary<Session, Route> adaptedRoutes = new Dictionary<Session, Route>();

    // some constants
    private static readonly Size iconSize = new Size(16, 16);
    private static readonly Size replayMarkerIconSize = new Size(10, 10);
    private static readonly Pen replayMarkerBorderPen = new Pen(Color.FromArgb(64, Color.Black));
    private const double replayMarkerSizeFactor = 3;

    private KmlExporter()
    {
      RouteLineStyleForSessions = new Dictionary<Session, KmlLineStyle>();
      ReplayMarkerStyleForSessions = new Dictionary<Session, KmlMarkerStyle>();
      NoOfColorCodedSteps = 32;
    }

    /// <summary>
    /// Creates an exporter where data is taken from one single QuickRoute document and its sessions
    /// </summary>
    /// <param name="document">The QuickRoute document to export</param>
    /// <param name="imageExporter">The image exporter used to create the map image</param>
    /// <param name="outputStream">The stream to export to</param>
    public KmlExporter(Document document, ImageExporter imageExporter, Stream outputStream)
      : this()
    {
      kmlExportDocuments = new List<KmlExportDocument> { new KmlExportDocument(document, imageExporter) };
      this.outputStream = outputStream;
    }

    /// <summary>
    /// Creates an exporter where the map data is taken from one single QuickRoute document and the sessions are specified in a custom SessionCollection object
    /// </summary>
    /// <param name="document">The QuickRoute document that contains the map to export</param>
    /// <param name="imageExporter">The image exporter used to create the map image</param>
    /// <param name="sessions">The sessions to export</param>
    /// <param name="outputStream">The stream to export to</param>
    public KmlExporter(Document document, ImageExporter imageExporter, SessionCollection sessions, Stream outputStream)
      : this()
    {
      kmlExportDocuments = new List<KmlExportDocument> { new KmlExportDocument(document, sessions, imageExporter) };
      this.outputStream = outputStream;
    }

    /// <summary>
    /// Creates an exporter from the specified QuickRoute files.
    /// </summary>
    /// <param name="documentFileNames">The file names of the QuickRoute files to export. The map image of the first file in the collection will be used as the map image.</param>
    /// <param name="imageExporterProperties">The proerties of the exporter used to create the map image</param>
    /// <param name="outputStream">The stream to export to</param>
    public KmlExporter(IEnumerable<string> documentFileNames, ImageExporterProperties imageExporterProperties, Stream outputStream)
      : this()
    {
      kmlExportDocuments = new List<KmlExportDocument>();
      foreach (var fileName in documentFileNames)
      {
        kmlExportDocuments.Add(new KmlExportDocument(fileName, imageExporterProperties));
      }
      this.outputStream = outputStream;
    }

    /// <summary>
    /// Exports data to a KMZ file.
    /// </summary>
    /// <param name="tempBasePath">A temporary path where files creatred during the export will reside. This path should not exist before calling this method. The path is deleted when this method completes.</param>
    public void ExportKmz(string tempBasePath)
    {
      LogUtil.LogDebug("Started");
      // 1. create temp directories and the kmz file
      temporaryBasePath = tempBasePath;
      Directory.CreateDirectory(temporaryBasePath);
      Directory.CreateDirectory(temporaryBasePath + @"files\");
      kmzFile = new ZipFile();
      kmzFile.AddDirectoryByName("files");

      // 2. create ground overlay image streams for each document
      LogUtil.LogDebug("Creating overlays");
      var groundOverlayCount = 0;
      GroundOverlays = new Dictionary<KmlExportDocument, KmlGroundOverlay>();
      foreach (var document in kmlExportDocuments)
      {
        if (KmlProperties.MapType != KmlExportMapType.None)
        {
          var fileName = temporaryBasePath + @"files\o" + groundOverlayCount + "." + document.ImageExporter.Properties.EncodingInfo.Encoder.MimeType.Replace("image/", ""); // NOTE: this assumes that the mime type matches the file extension
          using (var imageStream = new FileStream(fileName, FileMode.Create))
          {
            document.ImageExporter.OutputStream = imageStream;
            switch (KmlProperties.MapType)
            {
              case KmlExportMapType.Map:
                document.ImageExporter.Properties.RouteDrawingMode = Document.RouteDrawingMode.None;
                break;
              case KmlExportMapType.MapAndRoute:
                document.ImageExporter.Properties.RouteDrawingMode = Document.RouteDrawingMode.Extended;
                break;
            }
            document.ImageExporter.Export();

            GroundOverlays.Add(document, new KmlGroundOverlay(fileName, KmlGroundOverlay.GroundOverlayType.File));
            groundOverlayCount++;
          }
        }
      }

      // 3. generate kml document
      LogUtil.LogDebug("Generating kml document");
      var tempKmlFileName = temporaryBasePath + "doc.kml";
      using (var kmlStream = new FileStream(tempKmlFileName, FileMode.Create))
      {
        var writerSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true, IndentChars = "  " };
        using (var writer = XmlWriter.Create(kmlStream, writerSettings))
        {
          if (writer != null)
          {
            CreateKml(writer);
            writer.Close();
            kmzFile.AddFile(tempKmlFileName, "");
          }
        }
      }
      kmzFile.Save(OutputStream);
      kmzFile.Dispose();

      // 4. delete temp directory and its content
      LogUtil.LogDebug("Deleting temp dir");
      Directory.Delete(temporaryBasePath, true);
      LogUtil.LogDebug("Finished");
    }

    private void CreateKml(XmlWriter writer)
    {
      var formatProvider = new NumberFormatInfo { NumberDecimalSeparator = "." };
      var isSingleDocument = (kmlExportDocuments.Count == 1);

      writer.WriteStartDocument();
      writer.WriteStartElement("kml");
      if (!isSingleDocument)
      {
        CreateDocumentElement(Strings.QuickRouteKmlData, true, writer, formatProvider);
        CreateStyles(writer, formatProvider);
      }

      var documentCount = 0;
      var sessionCount = 0;
      foreach (var kmlExportDocument in kmlExportDocuments)
      {
        if (isSingleDocument)
        {
          CreateDocumentElement(kmlExportDocument.Document.Properties.Name, false, writer, formatProvider);
          CreateStyles(writer, formatProvider);
        }
        else
        {
          CreateFolderElement(kmlExportDocument.Document.Properties.Name, false, writer, formatProvider);
        }

        // GroundOverlay
        if (KmlProperties.MapType != KmlExportMapType.None)
        {
          CreateGroundOverlay(kmlExportDocument, documentCount, writer, formatProvider);
        }

        var isSingleSession = (kmlExportDocument.Sessions.Count == 1);
        var count = 0;
        var untitledCount = 0;
        foreach (var session in kmlExportDocument.Sessions)
        {
          LogUtil.LogDebug("Creating session " + count);
          // create folder, but only if there are multiple sessions
          if (!isSingleSession)
          {
            var sessionName = session.SessionInfo.Person.ToString();
            if (sessionName == "")
            {
              sessionName = string.Format(Strings.UntitledX, untitledCount);
            }
            CreateFolderElement(session.SessionInfo.Person.ToString(), false, writer, formatProvider);
            writer.WriteElementString("styleUrl", "#si" + sessionCount);
          }

          // Route lines
          LogUtil.LogDebug("Creating route lines");
          CreateRouteLines(session, kmlExportDocument, sessionCount, writer, formatProvider);

          // Route replay
          LogUtil.LogDebug("Creating replays");
          CreateReplay(session, kmlExportDocument, sessionCount, writer, formatProvider);

          // add folder end tag, but only if there are multiple sessions
          if (!isSingleSession)
          {
            writer.WriteEndElement();
          }

          count++;
          sessionCount++;
        }
        writer.WriteEndElement(); // Folder / Document
        documentCount++;
      }

      if (!isSingleDocument) writer.WriteEndElement(); // Document

      writer.WriteEndElement(); // kml
      writer.WriteEndDocument();
      writer.Flush();
    }

    private static void CreateFolderElement(string name, bool openChildren, XmlWriter writer, IFormatProvider formatProvider)
    {
      writer.WriteStartElement("Folder");
      writer.WriteElementString("name", name);
      writer.WriteElementString("open", (openChildren ? "1" : "0"));
    }

    private static void CreateDocumentElement(string name, bool openChildren, XmlWriter writer, IFormatProvider formatProvider)
    {
      writer.WriteStartElement("Document");
      writer.WriteElementString("name", name);
      writer.WriteElementString("open", (openChildren ? "1" : "0"));
      writer.WriteElementString("styleUrl", "#quickRouteIcon");
    }

    #region Styles

    private void CreateStyles(XmlWriter writer, IFormatProvider formatProvider)
    {
      LogUtil.LogDebug("Creating styles");
      CreateGeneralStyles(writer);
      CreateIconStyles(writer, formatProvider);
      CreateRouteLineStyles(writer, formatProvider);
      CreateReplayStyles(writer, formatProvider);
    }

    private void CreateGeneralStyles(XmlWriter writer)
    {
      if (KmlProperties.RouteType != KmlExportRouteType.None || KmlProperties.ReplayType != KmlExportReplayType.None)
      {
        writer.WriteStartElement("Style");
        writer.WriteAttributeString("id", "hideChildren");
        writer.WriteStartElement("ListStyle");
        writer.WriteElementString("listItemType", "checkHideChildren");
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    private void CreateIconStyles(XmlWriter writer, IFormatProvider provider)
    {
      iconStyleCount = 0;
      CreateIconStyle(Images.HeaderLogo, "quickRouteIcon", true, writer, provider);
      var documentCount = 0;
      var sessionCount = 0;
      foreach (var document in kmlExportDocuments)
      {
        if (KmlProperties.MapType != KmlExportMapType.None)
        {
          // create map icon for the document
          using (var map = (Bitmap)Image.FromFile(GroundOverlays[document].Href))
          {
            using (var thumbnail = CreateMapThumbnail(map, iconSize))
            {
              CreateIconStyle(thumbnail, "oi" + documentCount, true, writer, provider);
            }
          }
          documentCount++;
        }

        // create icons for the sessions
        foreach (var session in document.Sessions)
        {
          var routeLineStyle = GetRouteLineStyleForSession(session);
          var replayMarkerStyle = GetReplayMarkerStyleForSession(session);

          if (document.Sessions.Count > 1)
          {
            // the session
            using (var thumbnail = CreateReplayMarkerThumbnail(routeLineStyle.Color, replayMarkerIconSize, iconSize))
            {
              CreateIconStyle(thumbnail, "si" + sessionCount, true, writer, provider);
            }
          }

          if (KmlProperties.RouteType != KmlExportRouteType.None)
          {
            // the route
            using (var thumbnail = CreateRouteThumbnail(session.Route, iconSize, routeLineStyle.Color, 0.5))
            {
              CreateIconStyle(thumbnail, "ri" + sessionCount, false, writer, provider);
            }
          }

          if (KmlProperties.ReplayType != KmlExportReplayType.None)
          {
            // the replay folder
            using (var thumbnail = CreateReplayThumbnail(routeLineStyle.Color, iconSize))
            {
              CreateIconStyle(thumbnail, "rfi" + sessionCount, KmlProperties.HasReplayTails, writer, provider);
            }

            if (KmlProperties.HasReplayTails)
            {
              // the replay marker
              using (var thumbnail = CreateReplayMarkerThumbnail(replayMarkerStyle.Color, replayMarkerIconSize, iconSize))
              {
                CreateIconStyle(thumbnail, "rmi" + sessionCount, false, writer, provider);
              }
              // the replay tails
              using (var thumbnail = CreateReplayTailThumbnail(iconSize, replayMarkerStyle.Color, 2))
              {
                CreateIconStyle(thumbnail, "rti" + sessionCount, false, writer, provider);
              }
            }
          }
          sessionCount++;
        }
      }
    }

    private void CreateRouteLineStyles(XmlWriter writer, IFormatProvider formatProvider)
    {
      // create line styles
      // take replay tail into consideration
      if (KmlProperties.RouteType == KmlExportRouteType.Monochrome || (KmlProperties.HasReplayTails && KmlProperties.ReplayType == KmlExportReplayType.Monochrome))
      {
        CreateMonochromeRouteLineStyles();
      }
      if (KmlProperties.RouteType == KmlExportRouteType.ColorCoded || (KmlProperties.HasReplayTails && KmlProperties.ReplayType == KmlExportReplayType.ColorCoded))
      {
        CreateColorCodedRouteLineStyles();
      }

      // write them to xml stream
      foreach (var routeLineStyle in routeLineStyles.Keys)
      {
        writer.WriteStartElement("Style");
        writer.WriteAttributeString("id", routeLineStyles[routeLineStyle]);
        writer.WriteStartElement("LineStyle");
        writer.WriteElementString("color", routeLineStyle.GetColorAsString());
        writer.WriteElementString("width", routeLineStyle.Width.ToString(formatProvider));
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    private void CreateMonochromeRouteLineStyles()
    {
      foreach (var kmlExportDocument in kmlExportDocuments)
      {
        foreach (var session in kmlExportDocument.Sessions)
        {
          var dummyId = GetLineStyleId(GetRouteLineStyleForSession(session)); // GetLineStyleId adds line style to collection
        }
      }
    }

    private void CreateColorCodedRouteLineStyles()
    {
      foreach (var kmlExportDocument in kmlExportDocuments)
      {
        foreach (var session in kmlExportDocument.Sessions)
        {
          var sessionSettings = kmlExportDocument.ImageExporter.Properties.SessionSettings ?? session.Settings;

          foreach (var segment in GetRouteForSession(session, kmlExportDocument).Segments)
          {
            foreach (var waypoint in segment.Waypoints)
            {
              var lineStyle = GetLineStyleFromWaypoint(waypoint, kmlExportDocument.ImageExporter, sessionSettings);
              var dummyId = GetLineStyleId(lineStyle); // GetLineStyleId adds line style to collection
            }
          }
        }
      }
    }

    private void CreateReplayStyles(XmlWriter writer, IFormatProvider formatProvider)
    {
      // create replay styles
      switch (KmlProperties.ReplayType)
      {
        case KmlExportReplayType.Monochrome:
          CreateMonochromeReplayStyles();
          break;
        case KmlExportReplayType.ColorCoded:
          CreateColorCodedReplayStyles();
          break;
      }

      // write them to xml stream
      foreach (var markerStyle in markerStyles.Keys)
      {
        var style = markerStyle;
        writer.WriteStartElement("Style");
        writer.WriteAttributeString("id", markerStyles[markerStyle]);
        writer.WriteStartElement("IconStyle");

        writer.WriteStartElement("Icon");
        var fileName = temporaryBasePath + @"files\" + markerStyles[markerStyle] + ".png";
        var href = "files/" + markerStyles[markerStyle] + ".png";
        var bitmap = new Bitmap(Convert.ToInt32(style.Size), Convert.ToInt32(style.Size));
        using (var g = Graphics.FromImage(bitmap))
        {
          g.SmoothingMode = SmoothingMode.AntiAlias;
          using (var brush = new SolidBrush(style.Color))
          {
            g.FillEllipse(brush, 0, 0, Convert.ToSingle(style.Size - 1), Convert.ToSingle(style.Size - 1));
            g.DrawEllipse(replayMarkerBorderPen, 0, 0, Convert.ToSingle(style.Size - 1), Convert.ToSingle(style.Size - 1));
          }
        }

        bitmap.Save(fileName, ImageFormat.Png);
        kmzFile.AddFile(fileName, "files");
        writer.WriteElementString("href", href);
        writer.WriteEndElement();  // Icon
        writer.WriteElementString("scale", "0.3");
        writer.WriteEndElement(); // IconStyle
        writer.WriteEndElement(); // Style
      }
    }

    private void CreateMonochromeReplayStyles()
    {
      foreach (var kmlExportDocument in kmlExportDocuments)
      {
        foreach (var session in kmlExportDocument.Sessions)
        {
          var dummyId = GetMarkerStyleId(GetReplayMarkerStyleForSession(session)); // GetMarkerStyleId adds marker style to collection
        }
      }
    }

    private void CreateColorCodedReplayStyles()
    {
      foreach (var kmlExportDocument in kmlExportDocuments)
      {
        foreach (var session in kmlExportDocument.Sessions)
        {
          var sessionSettings = kmlExportDocument.ImageExporter.Properties.SessionSettings ?? session.Settings;

          foreach (var segment in GetRouteForSession(session, kmlExportDocument).Segments)
          {
            foreach (var waypoint in segment.Waypoints)
            {
              var markerStyle = GetMarkerStyleFromWaypoint(waypoint, kmlExportDocument.ImageExporter, sessionSettings);
              var dummyId = GetMarkerStyleId(markerStyle); // GetMarkerStyleId adds marker style to collection
            }
          }
        }
      }
    }

    private KmlLineStyle GetLineStyleFromWaypoint(Waypoint waypoint, ImageExporter imageExporter, SessionSettings sessionSettings)
    {
      var attribute = imageExporter.Properties.ColorCodingAttribute;
      var rls = sessionSettings.RouteLineSettingsCollection[attribute];
      return new KmlLineStyle()
      {
        Color = rls.ColorRange.GetColor(waypoint.Attributes[attribute].GetValueOrDefault(0), NoOfColorCodedSteps),
        Width = (rls.MaskVisible ? 2 * rls.MaskWidth : 0) + rls.Width
      };
    }

    private KmlMarkerStyle GetMarkerStyleFromWaypoint(Waypoint waypoint, ImageExporter imageExporter, SessionSettings sessionSettings)
    {
      var attribute = imageExporter.Properties.ColorCodingAttribute;
      var rls = sessionSettings.RouteLineSettingsCollection[attribute];
      return new KmlMarkerStyle()
      {
        Color = rls.ColorRange.GetColor(waypoint.Attributes[attribute].GetValueOrDefault(0), NoOfColorCodedSteps),
        Size = replayMarkerSizeFactor * ((rls.MaskVisible ? 2 * rls.MaskWidth : 0) + rls.Width)
      };
    }

    private KmlLineStyle GetRouteLineStyleForSession(Session s)
    {
      if (RouteLineStyleForSessions.ContainsKey(s)) return RouteLineStyleForSessions[s];
      return KmlProperties.RouteLineStyle;
    }

    private KmlMarkerStyle GetReplayMarkerStyleForSession(Session s)
    {
      if (ReplayMarkerStyleForSessions.ContainsKey(s)) return ReplayMarkerStyleForSessions[s];
      return KmlProperties.ReplayMarkerStyle;
    }

    private string GetLineStyleId(KmlLineStyle style)
    {
      if (!routeLineStyles.ContainsKey(style)) routeLineStyles.Add(style, "l" + routeLineStyles.Count);
      return routeLineStyles[style];
    }

    private string GetMarkerStyleId(KmlMarkerStyle style)
    {
      if (!markerStyles.ContainsKey(style)) markerStyles.Add(style, "m" + markerStyles.Count);
      return markerStyles[style];
    }

    private Route GetRouteForSession(Session session, KmlExportDocument parentDocument)
    {
      if (KmlProperties.RouteAdaptionStyle != KmlRouteAdaptationStyle.NoAdaption && KmlProperties.MapType != KmlExportMapType.None)
      {
        if (!adaptedRoutes.ContainsKey(session))
        {
          var baseSession = parentDocument == null || parentDocument.Sessions.Count == 0 || KmlProperties.RouteAdaptionStyle == KmlRouteAdaptationStyle.AdaptToSessionMapImage 
                                       ? session
                                       : parentDocument.Sessions[0];
          adaptedRoutes[session] = session.CreateRouteAdaptedToSingleTransformationMatrix(baseSession);
        }
        return adaptedRoutes[session];
      }
      return session.Route;
    }

    #endregion

    #region Overlays

    private void CreateGroundOverlay(KmlExportDocument kmlExportDocument, int documentCount, XmlWriter writer, IFormatProvider formatProvider)
    {
      if (GroundOverlays[kmlExportDocument] == null) return;
      writer.WriteStartElement("GroundOverlay");
      writer.WriteElementString("name", Strings.Map);
      writer.WriteElementString("styleUrl", "#oi" + documentCount);

      writer.WriteStartElement("Icon");
      writer.WriteElementString("href", GetGroundOverlayUrl(kmlExportDocument));
      writer.WriteEndElement();

      var orthogonallyRotatedCorners = GetOrthogonallyRotatedCorners(kmlExportDocument);

      if (orthogonallyRotatedCorners != null)
      {
        writer.WriteStartElement("LatLonBox");
        writer.WriteElementString("north", Math.Max(orthogonallyRotatedCorners[1].Latitude, orthogonallyRotatedCorners[2].Latitude).ToString(formatProvider));
        writer.WriteElementString("south", Math.Min(orthogonallyRotatedCorners[0].Latitude, orthogonallyRotatedCorners[3].Latitude).ToString(formatProvider));
        writer.WriteElementString("east", Math.Max(orthogonallyRotatedCorners[2].Longitude, orthogonallyRotatedCorners[3].Longitude).ToString(formatProvider));
        writer.WriteElementString("west", Math.Min(orthogonallyRotatedCorners[0].Longitude, orthogonallyRotatedCorners[1].Longitude).ToString(formatProvider));
        writer.WriteElementString("rotation", GetImageRotationD(kmlExportDocument).ToString(formatProvider));
        writer.WriteEndElement();
      }
      writer.WriteEndElement(); // GroundOverlay
    }

    private string GetGroundOverlayUrl(KmlExportDocument kmlExportDocument)
    {
      if (GroundOverlays.ContainsKey(kmlExportDocument))
      {
        var go = GroundOverlays[kmlExportDocument];

        switch (go.Type)
        {
          case KmlGroundOverlay.GroundOverlayType.Url:
            return go.Href;
          case KmlGroundOverlay.GroundOverlayType.File:
            var href = Path.GetFileName(go.Href);
            kmzFile.AddFile(go.Href, "");
            return href;
        }
      }
      return null;
    }

    #endregion

    #region Lines

    private void CreateRouteLines(Session session, KmlExportDocument parentDocument, int sessionCount, XmlWriter writer, IFormatProvider formatProvider)
    {
      if (KmlProperties.RouteType == KmlExportRouteType.None) return;
      CreateFolderElement(Strings.Route, false, writer, formatProvider);
      writer.WriteElementString("styleUrl", "#ri" + sessionCount);
      switch (KmlProperties.RouteType)
      {
        case KmlExportRouteType.Monochrome:
          CreateMonochromeRouteLines(session, parentDocument, writer, formatProvider);
          break;
        case KmlExportRouteType.ColorCoded:
          CreateColorCodedRouteLines(session, parentDocument, writer, formatProvider);
          break;
      }
      writer.WriteEndElement();
    }

    private void CreateMonochromeRouteLines(Session session, KmlExportDocument parentDocument, XmlWriter writer, IFormatProvider formatProvider)
    {
      foreach (var segment in GetRouteForSession(session, parentDocument).Segments)
      {
        CreateLineStringPlacemark(segment.Waypoints, GetRouteLineStyleForSession(session), writer, formatProvider);
      }
    }

    private void CreateColorCodedRouteLines(Session session, KmlExportDocument parentDocument, XmlWriter writer, IFormatProvider formatProvider)
    {
      var sessionSettings = parentDocument.ImageExporter.Properties.SessionSettings ?? session.Settings;
      foreach (var segment in GetRouteForSession(session, parentDocument).Segments)
      {
        var lineWaypoints = new List<Waypoint>();
        Waypoint previousWaypoint = null;
        KmlLineStyle previousLineStyle = null;
        foreach (var waypoint in segment.Waypoints)
        {
          if (previousWaypoint == null)
          {
            previousWaypoint = waypoint;
            lineWaypoints.Add(waypoint);
            continue; // skip first waypoint
          }

          // continue with this line or create new line with different color?
          previousLineStyle = GetLineStyleFromWaypoint(previousWaypoint, parentDocument.ImageExporter, sessionSettings);
          var lineStyle = GetLineStyleFromWaypoint(waypoint, parentDocument.ImageExporter, sessionSettings);
          if (lineStyle.Equals(previousLineStyle))
          {
            // add line segment to current line
            lineWaypoints.Add(waypoint);
          }
          else
          {
            lineWaypoints.Add(waypoint);
            CreateLineStringPlacemark(lineWaypoints, previousLineStyle, writer, formatProvider);
            lineWaypoints = new List<Waypoint>() { waypoint }; // create new line with this waypoint as start point
          }
          previousWaypoint = waypoint;
        }
        // add last line
        CreateLineStringPlacemark(lineWaypoints, previousLineStyle, writer, formatProvider);
      }
    }

    private void CreateLineStringPlacemark(IEnumerable<Waypoint> waypoints, KmlLineStyle routeLineStyle, XmlWriter writer, IFormatProvider formatProvider)
    {
      CreateLineStringPlacemark(waypoints, routeLineStyle, null, null, writer, formatProvider);
    }

    private void CreateLineStringPlacemark(IEnumerable<Waypoint> waypoints, KmlLineStyle routeLineStyle, DateTime? appearTime, DateTime? disappearTime, XmlWriter writer, IFormatProvider formatProvider)
    {
      writer.WriteStartElement("Placemark");
      writer.WriteElementString("styleUrl", "#" + GetLineStyleId(routeLineStyle));
      if (appearTime.HasValue || disappearTime.HasValue)
      {
        writer.WriteStartElement("TimeSpan");
        if (appearTime.HasValue) writer.WriteElementString("begin", FormatTime(appearTime.Value.ToUniversalTime()));
        if (disappearTime.HasValue) writer.WriteElementString("end", FormatTime(disappearTime.Value.ToUniversalTime()));
        writer.WriteEndElement(); // TimeSpan
      }
      writer.WriteStartElement("LineString");
      writer.WriteElementString("extrude", "1");
      writer.WriteElementString("tessellate", "1");
      writer.WriteStartElement("coordinates");
      foreach (var waypoint in waypoints)
      {
        writer.WriteString(waypoint.LongLat.Longitude.ToString(formatProvider) + "," +
                           waypoint.LongLat.Latitude.ToString(formatProvider) + " ");
      }
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    #endregion

    #region Replay

    private void CreateReplay(Session session, KmlExportDocument parentDocument, int sessionCount, XmlWriter writer, IFormatProvider formatProvider)
    {
      if (KmlProperties.ReplayType == KmlExportReplayType.None) return;

      CreateFolderElement(Strings.Replay, false, writer, formatProvider);
      writer.WriteElementString("styleUrl", "#rfi" + sessionCount);

      // markers
      if (KmlProperties.HasReplayTails)
      {
        CreateFolderElement(Strings.ReplayPosition, false, writer, formatProvider);
        writer.WriteElementString("styleUrl", "#rmi" + sessionCount);
      }
      CreateReplayMarkers(session, parentDocument, writer, formatProvider);
      if (KmlProperties.HasReplayTails) writer.WriteEndElement();

      // tail(s)
      foreach (var tail in KmlProperties.ReplayTails)
      {
        CreateFolderElement(Strings.ReplayTail, false, writer, formatProvider);
        writer.WriteElementString("styleUrl", "#rti" + sessionCount);
        CreateReplayTails(session, parentDocument, tail, writer, formatProvider);
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }

    private void CreateReplayMarkers(Session session, KmlExportDocument parentDocument, XmlWriter writer, IFormatProvider formatProvider)
    {
      LogUtil.LogDebug("");
      var sessionSettings = parentDocument.ImageExporter.Properties.SessionSettings ?? session.Settings;
      KmlMarkerStyle markerStyle = null;
      if (KmlProperties.ReplayType == KmlExportReplayType.Monochrome)
      {
        markerStyle = GetReplayMarkerStyleForSession(session);
      }

      var route = GetRouteForSession(session, parentDocument);
      foreach (var segment in route.Segments)
      {
        var waypoints = route.GetEquallySpacedWaypoints(segment.FirstWaypoint.Time, segment.LastWaypoint.Time,
                                                                KmlProperties.ReplayTimeInterval);
        foreach (var waypoint in waypoints)
        {
          // when should the marker disappear?
          DateTime? markerDisappearTime = segment.LastWaypoint.Time; // default value
          if (waypoint.Time.Add(KmlProperties.ReplayTimeInterval) <= segment.LastWaypoint.Time)
          {
            // subtract a millisecond from the time to prevent double occurences of markers
            markerDisappearTime = waypoint.Time.Add(KmlProperties.ReplayTimeInterval).Subtract(new TimeSpan(TimeSpan.TicksPerMillisecond));
          }

          // create the marker
          if (KmlProperties.ReplayType == KmlExportReplayType.ColorCoded)
          {
            markerStyle = GetMarkerStyleFromWaypoint(waypoint, parentDocument.ImageExporter, sessionSettings);
          }
          CreateReplayPlacemark(waypoint, markerStyle, waypoint.Time, markerDisappearTime,
                                writer, formatProvider);
        }
      }
    }

    private void CreateReplayTails(Session session, KmlExportDocument parentDocument, KmlReplayTail templateTail, XmlWriter writer, IFormatProvider formatProvider)
    {
      LogUtil.LogDebug("start");
      var sessionSettings = parentDocument.ImageExporter.Properties.SessionSettings ?? session.Settings;
      KmlLineStyle routeLineStyle = null;
      if (KmlProperties.ReplayType == KmlExportReplayType.Monochrome)
      {
        routeLineStyle = templateTail.RouteLineStyle ?? GetRouteLineStyleForSession(session);
      }
      
      var route = GetRouteForSession(session, parentDocument);
      foreach (var segment in route.Segments)
      {
        var waypoints = route.GetEquallySpacedWaypoints(segment.FirstWaypoint.Time, segment.LastWaypoint.Time,
                                                                KmlProperties.ReplayTimeInterval);
        for (var i = 1; i < waypoints.Count; i++)
        {
          var lastWaypoint = waypoints[i - 1];
          var waypoint = waypoints[i];
          // create tail
          DateTime? lineAppearTime = null;
          DateTime? lineDisappearTime = null;
          if (templateTail.StartVisible.HasValue)
          {
            lineAppearTime = waypoint.Time.Add(templateTail.StartVisible.Value);
            if (lineAppearTime < segment.FirstWaypoint.Time.Add(templateTail.StartVisible.Value)) lineAppearTime = segment.FirstWaypoint.Time;
            if (lineAppearTime > segment.LastWaypoint.Time.Add(templateTail.StartVisible.Value)) lineAppearTime = segment.LastWaypoint.Time;
          }
          if (templateTail.EndVisible.HasValue)
          {
            // subtract a millisecond from the time to ensure tail disappears correctly
            lineDisappearTime = waypoint.Time.Add(templateTail.EndVisible.Value).Subtract(new TimeSpan(TimeSpan.TicksPerMillisecond));
            if (lineDisappearTime < segment.FirstWaypoint.Time.Add(templateTail.EndVisible.Value)) lineDisappearTime = segment.FirstWaypoint.Time;
            if (lineDisappearTime > segment.LastWaypoint.Time.Add(templateTail.EndVisible.Value)) lineDisappearTime = segment.LastWaypoint.Time;
          }

          if (KmlProperties.ReplayType == KmlExportReplayType.ColorCoded)
          {
            routeLineStyle = GetLineStyleFromWaypoint(waypoint, parentDocument.ImageExporter, sessionSettings);
          }
          CreateLineStringPlacemark(new[] { lastWaypoint, waypoint }, routeLineStyle,
                                    lineAppearTime, lineDisappearTime, writer, formatProvider);
        }
      }
      LogUtil.LogDebug("end");
    }

    private void CreateReplayPlacemark(Waypoint waypoint, KmlMarkerStyle style, XmlWriter writer, IFormatProvider formatProvider)
    {
      CreateReplayPlacemark(waypoint, style, null, null, writer, formatProvider);
    }

    private void CreateReplayPlacemark(Waypoint waypoint, KmlMarkerStyle style, DateTime? appearTime, DateTime? disappearTime, XmlWriter writer, IFormatProvider formatProvider)
    {
      writer.WriteStartElement("Placemark");
      writer.WriteElementString("styleUrl", "#" + GetMarkerStyleId(style));
      if (appearTime.HasValue || disappearTime.HasValue)
      {
        writer.WriteStartElement("TimeSpan");
        if (appearTime.HasValue) writer.WriteElementString("begin", FormatTime(appearTime.Value.ToUniversalTime()));
        if (disappearTime.HasValue) writer.WriteElementString("end", FormatTime(disappearTime.Value.ToUniversalTime()));
        writer.WriteEndElement(); // TimeSpan
      }
      else
      {
        writer.WriteStartElement("TimeStamp");
        writer.WriteElementString("when", FormatTime(waypoint.Time.ToUniversalTime()));
        writer.WriteEndElement(); // TimeStamp
      }
      writer.WriteStartElement("Point");
      writer.WriteElementString("coordinates",
        waypoint.LongLat.Longitude.ToString(formatProvider) + "," +
        waypoint.LongLat.Latitude.ToString(formatProvider));
      writer.WriteEndElement(); // Point
      writer.WriteEndElement(); // Placemark
    }

    #endregion

    #region Icons

    private void CreateIconStyle(Bitmap bitmap, string styleId, bool showChildren, XmlWriter writer, IFormatProvider provider)
    {
      writer.WriteStartElement("Style");
      writer.WriteAttributeString("id", styleId);
      writer.WriteStartElement("ListStyle");
      if (!showChildren) writer.WriteElementString("listItemType", "checkHideChildren");
      writer.WriteStartElement("ItemIcon");
      writer.WriteElementString("state", "open closed");
      writer.WriteElementString("href", "files/i" + iconStyleCount + ".png");
      SaveBitmapToKmz(bitmap, "files", "i" + iconStyleCount + ".png", ImageFormat.Png);
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
      iconStyleCount++;
    }

    private void SaveBitmapToKmz(Bitmap bitmap, string path, string fileName, ImageFormat imageFormat)
    {
      var fileNameOnDisk = temporaryBasePath + path + @"\" + fileName;
      bitmap.Save(fileNameOnDisk, imageFormat);
      kmzFile.AddFile(fileNameOnDisk, path);
    }

    private static Bitmap CreateMapThumbnail(Bitmap original, Size thumbnailSize)
    {
      var bmp = new Bitmap(thumbnailSize.Width, thumbnailSize.Height);
      using (var g = Graphics.FromImage(bmp))
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        var scale = Math.Min((double)thumbnailSize.Width / original.Size.Width,
                             (double)thumbnailSize.Height / original.Size.Height);
        var projectionSize = new Size(Convert.ToInt32(scale * original.Size.Width),
                                      Convert.ToInt32(scale * original.Size.Height));
        var destRect = new Rectangle(
          (thumbnailSize.Width - projectionSize.Width) / 2,
          (thumbnailSize.Height - projectionSize.Height) / 2,
          projectionSize.Width - 1,
          projectionSize.Height - 1);
        g.DrawImage(original, destRect, new Rectangle(0, 0, original.Size.Width - 1, original.Size.Height - 1), GraphicsUnit.Pixel);
        // draw border around map
        using (var pen = new Pen(Color.FromArgb(92, Color.Black)))
        {
          g.DrawRectangle(pen, destRect);
        }
      }
      return bmp;
    }

    private static Bitmap CreateRouteThumbnail(Route route, Size thumbnailSize, Color routeLineColor, double routeLineWidth)
    {
      var bitmap = new Bitmap(thumbnailSize.Width, thumbnailSize.Width);
      var projectionOrigin = route.CenterLongLat();

      // get coordinates of lines
      double minX = 0, maxX = 0, minY = 0, maxY = 0;
      var lines = new List<List<PointD>>();
      foreach (var segment in route.Segments)
      {
        var vertices = new List<PointD>();
        foreach (var waypoint in segment.Waypoints)
        {
          var vertex = waypoint.LongLat.Project(projectionOrigin);
          vertices.Add(vertex);
          // calculate bounds
          if (lines.Count == 0 && vertices.Count == 0)
          {
            minX = vertex.X;
            minY = vertex.Y;
            maxX = vertex.X;
            maxY = vertex.Y;
          }
          else
          {
            minX = Math.Min(minX, vertex.X);
            minY = Math.Min(minY, vertex.Y);
            maxX = Math.Max(maxX, vertex.X);
            maxY = Math.Max(maxY, vertex.Y);
          }
        }
        lines.Add(vertices);
      }

      var scale = Math.Min((thumbnailSize.Width - 1) / (maxX == minX ? 0 : maxX - minX),
                           (thumbnailSize.Height - 1) / (maxY == minY ? 0 : maxY - minY));
      var offsetX = (thumbnailSize.Width - (maxX - minX) * scale) / 2;
      var offsetY = (thumbnailSize.Height - (maxY - minY) * scale) / 2;

      // map coordinates onto thumbnail
      var mappedLines = new List<List<PointF>>();
      foreach (var line in lines)
      {
        var mappedVertices = new List<PointF>();
        foreach (var vertex in line)
        {
          mappedVertices.Add((PointF)new PointD(
            offsetX + scale * (vertex.X - minX),
            (thumbnailSize.Height - 1) - (offsetY + scale * (vertex.Y - minY))));
        }
        mappedLines.Add(mappedVertices);
      }

      using (var g = Graphics.FromImage(bitmap))
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        foreach (var mappedLine in mappedLines)
        {
          if(mappedLine.Count > 1) g.DrawLines(new Pen(routeLineColor, (float)routeLineWidth), mappedLine.ToArray());
        }
      }

      return bitmap;
    }

    private static Bitmap CreateReplayThumbnail(Color color, Size thumbnailSize)
    {
      var bitmap = new Bitmap(thumbnailSize.Width, thumbnailSize.Width);
      using (var g = Graphics.FromImage(bitmap))
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var brush = new SolidBrush(color);
        var fingerBrush = new SolidBrush(Color.White);
        var clockEllipse = new RectangleF(0.0f, 3.1f, 11.0f, 11.0f);
        var fingerPolygon = new[]
                              {
                                new PointF(3.5f, 4.9f),
                                new PointF(5.5f, 4.9f),
                                new PointF(5.5f, 8.3f),
                                new PointF(8.0f, 10.4f),
                                new PointF(7.1f, 11.6f),
                                new PointF(4.0f, 9.0f)
                              };
        var arrowPolygon = new[]
                             {
                               new PointF(11.0f, 0.0f),
                               new PointF(11.0f, 7.5f),
                               new PointF(15.0f, 3.5f)
                             };
        g.FillEllipse(brush, clockEllipse);
        g.DrawEllipse(replayMarkerBorderPen, clockEllipse);
        g.FillPolygon(fingerBrush, fingerPolygon);
        g.FillPolygon(brush, arrowPolygon);
        g.DrawPolygon(replayMarkerBorderPen, arrowPolygon);

        brush.Dispose();
        fingerBrush.Dispose();
      }
      return bitmap;
    }


    private static Bitmap CreateReplayMarkerThumbnail(Color color, Size markerSize, Size thumbnailSize)
    {
      var bitmap = new Bitmap(thumbnailSize.Width, thumbnailSize.Width);
      using (var g = Graphics.FromImage(bitmap))
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using (var brush = new SolidBrush(color))
        {
          var rect = new Rectangle((thumbnailSize.Width - markerSize.Width) / 2,
                                   (thumbnailSize.Height - markerSize.Height) / 2,
                                   markerSize.Width,
                                   markerSize.Height);
          g.FillEllipse(brush, rect);
          g.DrawEllipse(replayMarkerBorderPen, rect);
        }
      }
      return bitmap;
    }

    private static Bitmap CreateReplayTailThumbnail(Size thumbnailSize, Color lineColor, double lineWidth)
    {
      var bitmap = new Bitmap(thumbnailSize.Width, thumbnailSize.Width);
      var tailPoints = new[]
                          {
                            new PointF(1.3f, 10.7f),
                            new PointF(1.7f, 9.8f),
                            new PointF(2.4f, 9.0f),
                            new PointF(3.1f, 8.4f),
                            new PointF(3.8f, 7.9f),
                            new PointF(4.7f, 7.7f),
                            new PointF(5.6f, 7.4f),
                            new PointF(6.4f, 7.4f),
                            new PointF(7.2f, 7.4f),
                            new PointF(8.1f, 7.4f),
                            new PointF(9.0f, 7.3f),
                            new PointF(9.7f, 7.1f),
                            new PointF(10.4f, 6.7f),
                            new PointF(11.1f, 6.3f),
                            new PointF(11.7f, 5.6f)
                          };


      using (var g = Graphics.FromImage(bitmap))
      {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using (var pen = new Pen(lineColor, (float)lineWidth))
        {
          g.DrawLines(pen, tailPoints);
        }

        var rect = new RectangleF(10.0f, 1.4f, 5f, 5f);
        using (var brush = new SolidBrush(lineColor))
        {
          g.FillEllipse(brush, rect);
          g.DrawEllipse(replayMarkerBorderPen, rect);
        }
      }
      return bitmap;
    }

    #endregion

    /// <summary>
    /// Gets the corners of the map if the map has a rotation of 0, i e are aligned to the longitude and latitude axes.
    /// </summary>
    /// <param name="kmlExportDocument"></param>
    /// <returns></returns>
    private static LongLat[] GetOrthogonallyRotatedCorners(KmlExportDocument kmlExportDocument)
    {
      var document = kmlExportDocument.Document;
      var corners = document.GetImageCornersLongLat(kmlExportDocument.ImageExporter.ImageBounds, kmlExportDocument.ImageExporter.MapBounds,
                                                             kmlExportDocument.ImageExporter.Properties.PercentualSize);
      var rotationD = GetImageRotationD(kmlExportDocument);
      var rotationR = LinearAlgebraUtil.ToRadians(rotationD);
      if (corners != null)
      {
        // need to align map in north-south direction; rotate map around its center
        var center = (corners[0] / 4 + corners[1] / 4 + corners[2] / 4 + corners[3] / 4);
        var projectedCenter = center.Project(document.ProjectionOrigin);
        var projectedOrthogonallyRotatedCorners = new[]
                                          {
                                            LinearAlgebraUtil.Rotate(corners[0].Project(document.ProjectionOrigin), projectedCenter, rotationR),
                                            LinearAlgebraUtil.Rotate(corners[1].Project(document.ProjectionOrigin), projectedCenter, rotationR),
                                            LinearAlgebraUtil.Rotate(corners[2].Project(document.ProjectionOrigin), projectedCenter, rotationR),
                                            LinearAlgebraUtil.Rotate(corners[3].Project(document.ProjectionOrigin), projectedCenter, rotationR),
                                          };
        var orthogonallyRotatedCorners = new[]
                                 {
                                   LongLat.Deproject(projectedOrthogonallyRotatedCorners[0], document.ProjectionOrigin),
                                   LongLat.Deproject(projectedOrthogonallyRotatedCorners[1], document.ProjectionOrigin),
                                   LongLat.Deproject(projectedOrthogonallyRotatedCorners[2], document.ProjectionOrigin),
                                   LongLat.Deproject(projectedOrthogonallyRotatedCorners[3], document.ProjectionOrigin),
                                 };
        return orthogonallyRotatedCorners;
      }
      return null;
    }

    private static double GetImageRotationD(KmlExportDocument kmlExportDocument)
    {
      var document = kmlExportDocument.Document;
      var corners = document.GetImageCornersLongLat(kmlExportDocument.ImageExporter.ImageBounds, kmlExportDocument.ImageExporter.MapBounds,
                                                    kmlExportDocument.ImageExporter.Properties.PercentualSize);
      var sw = corners[0].Project(document.ProjectionOrigin);
      var se = corners[3].Project(document.ProjectionOrigin);
      return LinearAlgebraUtil.GetAngleD(se - sw);
    }

    private static string FormatTime(DateTime time)
    {
      var firstPart = time.ToString("yyyy-MM-ddTHH:mm:ss");
      return firstPart + time.ToString(".fffffff").TrimEnd("0".ToCharArray()).TrimEnd(".".ToCharArray()) + "Z";
    }

  }

  public class KmlGroundOverlay
  {
    private readonly string href;
    private readonly GroundOverlayType type;

    public KmlGroundOverlay(string href, GroundOverlayType type)
    {

      this.href = href;
      this.type = type;
    }

    public enum GroundOverlayType
    {
      File,
      Url
    }

    public GroundOverlayType Type
    {
      get { return type; }
    }

    public string Href
    {
      get { return href; }
    }

  }

  public class KmlExportDocument : IDisposable
  {
    private Document document;
    private SessionCollection sessions;
    private ImageExporter imageExporter;
    private readonly string fileName;
    private readonly KmlExportDocumentType type;
    private readonly ImageExporterProperties imageExporterProperties;

    public KmlExportDocument(string fileName, ImageExporterProperties imageExporterProperties)
    {
      this.fileName = fileName;
      this.imageExporterProperties = imageExporterProperties;
      this.type = KmlExportDocumentType.File;
    }

    public KmlExportDocument(Document document, ImageExporter imageExporter)
    {
      this.document = document;
      this.sessions = document.Sessions;
      this.imageExporter = imageExporter;
      this.type = KmlExportDocumentType.Document;
    }

    public KmlExportDocument(Document document, SessionCollection sessions, ImageExporter imageExporter)
    {
      this.document = document;
      this.sessions = sessions;
      this.imageExporter = imageExporter;
      this.type = KmlExportDocumentType.Document;
    }

    public Document Document
    {
      get
      {
        InitDocumentAndSessions();
        return document;
      }
    }

    public SessionCollection Sessions
    {
      get
      {
        InitDocumentAndSessions();
        return sessions;
      }
    }

    public ImageExporter ImageExporter
    {
      get
      {
        InitImageExporter();
        return imageExporter;
      }
    }

    public void Dispose()
    {
      if (type == KmlExportDocumentType.File)
      {
        document = null;
        sessions = null;
      }
    }

    private void InitDocumentAndSessions()
    {
      if (type == KmlExportDocumentType.File && document == null)
      {
        document = Document.Open(fileName);
        if (document != null)
        {
          document.Initialize();
          sessions = document.Sessions;
        }
      }
    }

    private void InitImageExporter()
    {
      if (imageExporter == null)
      {
        InitDocumentAndSessions();
        imageExporter = new ImageExporter(Document, Sessions) { Properties = imageExporterProperties };
      }
    }

    private enum KmlExportDocumentType
    {
      Document,
      File
    }
  }

  [Serializable]
  public class KmlProperties : ICloneable
  {
    public KmlExportMapType MapType { get; set; }
    public KmlExportRouteType RouteType { get; set; }
    public KmlExportReplayType ReplayType { get; set; }
    public KmlLineStyle RouteLineStyle { get; set; }
    public KmlMarkerStyle ReplayMarkerStyle { get; set; }
    public TimeSpan ReplayTimeInterval { get; set; }
    public List<KmlReplayTail> ReplayTails { get; set; }
    /// <summary>
    /// Gets or sets whether the route waypoint coordinates should be slightly transformed to exactly adapt to the map image rather than to the reality
    /// </summary>
    public KmlRouteAdaptationStyle RouteAdaptionStyle { get; set; }

    private const double replayMarkerSizeFactor = 3;

    public KmlProperties()
    {
      MapType = KmlExportMapType.MapAndRoute;
      RouteType = KmlExportRouteType.None;
      ReplayType = KmlExportReplayType.None;
      ReplayTimeInterval = new TimeSpan(0, 0, 5);
      RouteLineStyle = new KmlLineStyle();
      ReplayMarkerStyle = new KmlMarkerStyle();
      ReplayTails = new List<KmlReplayTail>() { new KmlReplayTail() { StartVisible = new TimeSpan(0), EndVisible = new TimeSpan(0, 0, 60) } };
    }

    public bool HasReplayTails
    {
      get
      {
        return ReplayTails.Count > 0;
      }
    }

    public KmlProperties(RouteLineSettings rls)
      : this()
    {
      RouteLineStyle.Color = rls.MonochromeColor;
      RouteLineStyle.Width = rls.MonochromeWidth;
      ReplayMarkerStyle.Color = rls.MonochromeColor;
      ReplayMarkerStyle.Size = replayMarkerSizeFactor * rls.MonochromeWidth;
    }

    public object Clone()
    {
      var clone = new KmlProperties
                    {
                      MapType = MapType,
                      RouteType = RouteType,
                      ReplayType = ReplayType,
                      RouteLineStyle = RouteLineStyle.Clone() as KmlLineStyle,
                      ReplayMarkerStyle = ReplayMarkerStyle.Clone() as KmlMarkerStyle,
                      ReplayTimeInterval = ReplayTimeInterval,
                      ReplayTails = new List<KmlReplayTail>(),
                      RouteAdaptionStyle = RouteAdaptionStyle
                    };
      foreach (var tail in ReplayTails)
      {
        clone.ReplayTails.Add(tail.Clone() as KmlReplayTail);
      }
      return clone;
    }
  }

  [Serializable]
  public class KmlLineStyle : ICloneable
  {
    public Color Color { get; set; }
    public double Width { get; set; }

    public KmlLineStyle()
    {
      Color = Color.FromArgb(160, Color.Red);
      Width = 4;
    }

    public string GetColorAsString()
    {
      return
        String.Format("{0:X2}", Color.A) +
        String.Format("{0:X2}", Color.B) +
        String.Format("{0:X2}", Color.G) +
        String.Format("{0:X2}", Color.R);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(KmlLineStyle)) return false;
      return Equals((KmlLineStyle)obj);
    }

    public bool Equals(KmlLineStyle obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.Color.Equals(Color) && obj.Width == Width;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Color.GetHashCode() * 397) ^ Width.GetHashCode();
      }
    }

    public object Clone()
    {
      return MemberwiseClone();
    }
  }

  [Serializable]
  public class KmlMarkerStyle : ICloneable
  {
    public Color Color { get; set; }
    public double Size { get; set; }

    public KmlMarkerStyle()
    {
      Color = Color.FromArgb(160, Color.Red);
      Size = 8;
    }

    public string GetColorAsString()
    {
      return
        String.Format("{0:X2}", Color.A) +
        String.Format("{0:X2}", Color.B) +
        String.Format("{0:X2}", Color.G) +
        String.Format("{0:X2}", Color.R);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(KmlMarkerStyle)) return false;
      return Equals((KmlMarkerStyle)obj);
    }

    public bool Equals(KmlMarkerStyle obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.Color.Equals(Color) && obj.Size == Size;
    }

    public object Clone()
    {
      return MemberwiseClone();
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (Color.GetHashCode() * 397) ^ Size.GetHashCode();
      }
    }
  }

  [Serializable]
  public class KmlReplayTail : ICloneable
  {
    /// <summary>
    /// Time relative to current time when the tail becomes visible, or null if always visible
    /// </summary>
    public TimeSpan? StartVisible { get; set; }
    /// <summary>
    /// Time relative to current time when the tail becomes hidden again, or null if always visible
    /// </summary>
    public TimeSpan? EndVisible { get; set; }
    public KmlLineStyle RouteLineStyle { get; set; }

    public object Clone()
    {
      var clone = new KmlReplayTail()
      {
        StartVisible = StartVisible,
        EndVisible = EndVisible
      };
      if (RouteLineStyle != null) clone.RouteLineStyle = RouteLineStyle.Clone() as KmlLineStyle;
      return clone;
    }
  }

  [Serializable]
  public enum KmlExportMapType
  {
    None,
    Map,
    MapAndRoute
  }

  [Serializable]
  public enum KmlExportRouteType
  {
    None,
    Monochrome,
    ColorCoded
  }

  [Serializable]
  public enum KmlExportReplayType
  {
    None,
    Monochrome,
    ColorCoded
  }

  [Serializable]
  public enum KmlRouteAdaptationStyle
  {
    NoAdaption,
    AdaptToSessionMapImage,
    AdaptToDocumentMapImage
  }

}
