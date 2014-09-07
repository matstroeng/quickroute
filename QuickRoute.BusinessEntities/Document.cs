using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using QuickRoute.Common;

namespace QuickRoute.BusinessEntities
{
  /// <summary>
  /// The container object for a QuickRoute document, holding the map, the sessions, and the document settings.
  /// </summary>
  [Serializable]
  public class Document : ISerializable, IDisposable
  {
    private const int exportImageHeaderHeight = 64;
    private const int exportImageBorderWidth = 1;

    private Map map;
    private DocumentSettings settings;
    private SessionCollection sessions = new SessionCollection();
    private LongLat projectionOrigin;
    private DocumentProperties properties;

    public event EventHandler<EventArgs> MapChanged;

    #region Constructors

    /// <summary>
    /// Creates a new document using the specified map and document settings. No sessions are added.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="settings"></param>
    public Document(Map map, DocumentSettings settings)
    {
      Map = map;
      Settings = settings;
    }

    /// <summary>
    /// Creating a new document using the specified map, route, laps, and document settings, and adding one new session with the specified route and laps.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="route"></param>
    /// <param name="laps"></param>
    /// <param name="settings"></param>
    public Document(Map map, Route route, LapCollection laps, DocumentSettings settings)
      : this(map, route, laps, null, settings)
    {
    }

    /// <summary>
    /// Creating a new document using the specified map, route, laps, initial transformation matrix and document settings, and adding one new session with the specified route and laps.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="route"></param>
    /// <param name="laps"></param>
    /// <param name="initialTransformationMatrix"></param>
    /// <param name="settings"></param>
    public Document(Map map, Route route, LapCollection laps, GeneralMatrix initialTransformationMatrix, DocumentSettings settings)
      : this(map, route, laps, initialTransformationMatrix, null, settings)
    {
    }

    /// <summary>
    /// Creating a new document using the specified map, route, laps, initial transformation matrix, projection origin and document settings, and adding one new session with the specified route and laps.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="route"></param>
    /// <param name="laps"></param>
    /// <param name="initialTransformationMatrix"></param>
    /// <param name="projectionOrigin"></param>
    /// <param name="settings"></param>
    public Document(Map map, Route route, LapCollection laps, GeneralMatrix initialTransformationMatrix, LongLat projectionOrigin, DocumentSettings settings)
    {
      Map = map;
      sessions.Add(new Session(route, laps, map.Image.Size, initialTransformationMatrix, projectionOrigin, settings.DefaultSessionSettings));
      this.settings = settings;
      UpdateDocumentToCurrentVersion(this);
    }

    protected Document(SerializationInfo info, StreamingContext context)
    {
      Map = (Map)(info.GetValue("map", typeof(Map)));
      settings = (DocumentSettings)(info.GetValue("settings", typeof(DocumentSettings)));
      sessions = (SessionCollection)(info.GetValue("sessions", typeof(SessionCollection)));
      projectionOrigin = (LongLat)(info.GetValue("projectionOrigin", typeof(LongLat)));
      // todo: how handle non-existing properties field?
      try
      {
        properties = (DocumentProperties)(info.GetValue("properties", typeof(DocumentProperties)));
      }
      catch (Exception)
      { }
    }

    #endregion

    #region ISerializable members

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("map", map, typeof(Map));
      info.AddValue("settings", settings, typeof(DocumentSettings));
      info.AddValue("sessions", sessions, typeof(List<Session>));
      info.AddValue("projectionOrigin", projectionOrigin, typeof(LongLat));
      info.AddValue("properties", properties, typeof(DocumentProperties));
    }

    #endregion

    #region Public properties

    /// <summary>
    /// The document's map.
    /// </summary>
    public Map Map
    {
      get { return map; }
      set
      {
        map = value;
        if (MapChanged != null) MapChanged(this, new EventArgs());
      }
    }

    /// <summary>
    /// The settings of the document.
    /// </summary>
    public DocumentSettings Settings
    {
      get { return settings; }
      set
      {
        settings = value;
        // apply to all sessions
        foreach (Session s in sessions)
        {
          s.Settings = settings.DefaultSessionSettings.Copy();
        }
      }
    }

    /// <summary>
    /// The sessions included in the document.
    /// </summary>
    public SessionCollection Sessions
    {
      get { return sessions; }
      set { sessions = value; }
    }

    /// <summary>
    /// The longitude and latitude of the location that acts as the origin for the projection from 3D (earth) to 2D (map).
    /// </summary>
    public LongLat ProjectionOrigin
    {
      get { return projectionOrigin; }
      set { projectionOrigin = value; }
    }

    /// <summary>
    /// Saves the current document to a qrt file.
    /// </summary>
    /// <param name="fileName">The filename of the file.</param>
    public void Save(string fileName)
    {
      FileFormat = QuickRouteFileFormat.Qrt; 
      IFormatter formatter = new BinaryFormatter();
      Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
      try
      {
        formatter.Serialize(stream, this);
        FileName = fileName;
      }
      finally
      {
        stream.Close();
      }
    }

    public string FileName { get; set; }

    public DocumentProperties Properties
    {
      get 
      { 
        if(properties == null)
        {
          // create standard properties
          var dp = new DocumentProperties { Name = Path.GetFileNameWithoutExtension(FileName) };
          if(dp.Name == null && Sessions.Count > 0 && Sessions[0].Route.FirstWaypoint != null)
          {
            var dt = Sessions[0].Route.FirstWaypoint.Time.ToLocalTime();
            dp.Name = dt.ToLongDateString() + " " + dt.ToShortTimeString(); 
          }
          return dp;
        }
        return properties; 
      }
      set { properties = value; }
    }

    public QuickRouteFileFormat FileFormat { get; set; }

    #endregion

    #region Public methods

    public void Initialize()
    {
      foreach (var s in sessions)
      {
        s.Initialize(true);
      }
    }

    public Bitmap CreateMapImage(double zoom)
    {
      Size size = GetMapImageSize(zoom);
      Bitmap image = new Bitmap(size.Width, size.Height,
                                  PixelFormat.Format32bppPArgb);
      Graphics graphics = Graphics.FromImage(image);
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      var srcRect = new Rectangle(new Point(0, 0), Map.Image.Size);
      var dstRect = new Rectangle(new Point(0, 0), image.Size);
      graphics.DrawImage(Map.Image, dstRect, srcRect, GraphicsUnit.Pixel);
      graphics.Dispose();
      return image;
    }

    public void DrawRoutes(IEnumerable<Session> sessionsToDraw, double zoom, Graphics graphics, Image mapImage, RouteDrawingMode mode, WaypointAttribute colorCodingAttribute, WaypointAttribute? secondaryColorCodingAttribute, SessionSettings sessionSettings)
    {
      graphics.Clip = new Region(new Rectangle(0, 0, mapImage.Width, mapImage.Height));

      // copy map as a background to route
      graphics.DrawImage(mapImage, new Point(0, 0));

      // draw the routes
      foreach (var s in sessionsToDraw)
      {
        s.DrawRoute(zoom, graphics, mode, colorCodingAttribute, secondaryColorCodingAttribute, sessionSettings);
      }
    }

    public Size CalculateImageForExportSize(double zoomValue)
    {
      var size = GetMapImageSize(zoomValue);
      return new Size(size.Width + 2 * exportImageBorderWidth,
                      size.Height + 3 * exportImageBorderWidth + exportImageHeaderHeight);
    }

    public Bitmap CreateMapAndRouteImage(bool showMap, double zoomValue, SessionCollection sessionsToDraw, WaypointAttribute colorCodingAttribute, WaypointAttribute? secondaryColorCodingAttribute, RouteDrawingMode mode, SessionSettings sessionSettings)
    {
      Bitmap mapImage;
      if(showMap)
      {
        mapImage = CreateMapImage(zoomValue);
      }
      else
      {
        var size = GetMapImageSize(zoomValue);
        mapImage = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
      }

      if (mode == RouteDrawingMode.None) return mapImage;

      var mapAndRouteImage = new Bitmap(mapImage.Width, mapImage.Height, PixelFormat.Format32bppPArgb);
      var mapAndRouteGraphics = Graphics.FromImage(mapAndRouteImage);
      mapAndRouteGraphics.SmoothingMode = SmoothingMode.AntiAlias;

      DrawRoutes(sessionsToDraw, zoomValue, mapAndRouteGraphics, mapImage, mode, colorCodingAttribute, secondaryColorCodingAttribute, sessionSettings);

      mapAndRouteGraphics.Dispose();
      mapImage.Dispose();
      return mapAndRouteImage;
    }

    // todo: skip showRouteLine parameter, use RouteDrawingMode instead
    public Bitmap CreateMapAndRouteImage(bool showRouteLine, double zoomValue, Session sessionToDraw, List<int> legsToDraw, double frameWidth, WaypointAttribute colorCodingAttribute, WaypointAttribute? secondaryColorCodingAttribute, RouteDrawingMode mode, SessionSettings sessionSettings)
    {
      RectangleD frame = GetFrame(zoomValue, sessionToDraw, legsToDraw, frameWidth, colorCodingAttribute, mode);

      var sc = new SessionCollection();
      sc.Add(sessionToDraw);
      var wholeImage = CreateMapAndRouteImage(true, zoomValue, sc, colorCodingAttribute, secondaryColorCodingAttribute, mode, sessionSettings);

      AdjustFrameToImage(frame, wholeImage);

      var croppedImage = new Bitmap(
        Convert.ToInt32(Math.Ceiling(frame.Right) - Math.Floor(frame.Left)),
        Convert.ToInt32(Math.Ceiling(frame.Bottom) - Math.Floor(frame.Top)));
      var croppedImageGraphics = Graphics.FromImage(croppedImage);

      croppedImageGraphics.DrawImage(
        wholeImage,
        -Convert.ToInt32(Math.Floor(frame.Left)),
        -Convert.ToInt32(Math.Floor(frame.Top)));
      croppedImageGraphics.Dispose();
      wholeImage.Dispose();
      return croppedImage;
    }

    public LongLat[] GetImageCornersLongLat(Rectangle imageBounds, Rectangle mapBounds, double zoomValue)
    {
      var swPixel = new PointD(0, Map.Image.Height) +
                    new PointD(imageBounds.Left - mapBounds.Left,
                               imageBounds.Bottom - mapBounds.Bottom) /
                    zoomValue;
      var nePixel = new PointD(Map.Image.Width, 0) +
                    new PointD(imageBounds.Right - mapBounds.Right,
                               imageBounds.Top - mapBounds.Top) /
                    zoomValue;
      var sePixel = new PointD(nePixel.X, swPixel.Y);
      var nwPixel = new PointD(swPixel.X, nePixel.Y);

      var inverseAverageTransformationMatrix = Sessions.CalculateAverageTransformation().TransformationMatrix.Inverse();

      return new[] 
               { 
                 GetLongLatForMapImagePosition(swPixel, inverseAverageTransformationMatrix),
                 GetLongLatForMapImagePosition(nwPixel, inverseAverageTransformationMatrix),
                 GetLongLatForMapImagePosition(nePixel, inverseAverageTransformationMatrix),
                 GetLongLatForMapImagePosition(sePixel, inverseAverageTransformationMatrix)
               };
    }

    public LongLat[] GetMapCornersLongLat()
    {
      var swPixel = new PointD(0, Map.Image.Height);
      var nePixel = new PointD(Map.Image.Width, 0);
      var sePixel = new PointD(nePixel.X, swPixel.Y);
      var nwPixel = new PointD(swPixel.X, nePixel.Y);

      var inverseAverageTransformationMatrix = Sessions.CalculateAverageTransformation().TransformationMatrix.Inverse();

      return new[] 
               { 
                 GetLongLatForMapImagePosition(swPixel, inverseAverageTransformationMatrix),
                 GetLongLatForMapImagePosition(nwPixel, inverseAverageTransformationMatrix),
                 GetLongLatForMapImagePosition(nePixel, inverseAverageTransformationMatrix),
                 GetLongLatForMapImagePosition(sePixel, inverseAverageTransformationMatrix)
               };
    }

    #endregion

    #region Private methods

    private static void UpdateDocumentToCurrentVersion(Document doc)
    {
      var defaultRLS = SessionSettings.CreateDefaultRouteLineSettingsCollection();

      // ensure MonochromeColor is non-invisible
      foreach (Session s in doc.sessions)
      {
        foreach (var rls in s.Settings.RouteLineSettingsCollection.Values)
        {
          if (rls.MonochromeColor == Color.FromArgb(0, 0, 0, 0))
          {
            rls.MonochromeColor = rls.ColorRange.Gradient.GetColor(1);
            rls.MonochromeWidth = rls.Width;
          }
        }
      }


      // add some speed waypoint attribute settings, introduced in QR 2.1
      if (!doc.Settings.ColorRangeIntervalSliderSettings.ContainsKey(WaypointAttribute.Speed))
      {
        var defaultCRISS = DocumentSettings.CreateDefaultColorRangeIntervalSliderSettings();
        doc.Settings.ColorRangeIntervalSliderSettings.Add(WaypointAttribute.Speed, defaultCRISS[WaypointAttribute.Speed]);
      }

      if (!doc.Settings.LapHistogramSettings.ContainsKey(WaypointAttribute.Speed))
      {
        var defaultLHS = DocumentSettings.CreateDefaultLapHistogramSettings();
        doc.Settings.LapHistogramSettings.Add(WaypointAttribute.Speed, defaultLHS[WaypointAttribute.Speed]);
      }

      foreach (Session s in doc.sessions)
      {
        if (!s.Settings.RouteLineSettingsCollection.ContainsKey(WaypointAttribute.Speed))
        {
          s.Settings.RouteLineSettingsCollection.Add(WaypointAttribute.Speed, defaultRLS[WaypointAttribute.Speed]);
        }
      }


      // add some direction waypoint attribute settings, introduced in QR 2.1-4
      if (!doc.Settings.ColorRangeIntervalSliderSettings.ContainsKey(WaypointAttribute.DirectionDeviationToNextLap))
      {
        var defaultCRISS = DocumentSettings.CreateDefaultColorRangeIntervalSliderSettings();
        doc.Settings.ColorRangeIntervalSliderSettings.Add(WaypointAttribute.DirectionDeviationToNextLap, defaultCRISS[WaypointAttribute.DirectionDeviationToNextLap]);
      }

      if (!doc.Settings.LapHistogramSettings.ContainsKey(WaypointAttribute.DirectionDeviationToNextLap))
      {
        var defaultLHS = DocumentSettings.CreateDefaultLapHistogramSettings();
        doc.Settings.LapHistogramSettings.Add(WaypointAttribute.DirectionDeviationToNextLap, defaultLHS[WaypointAttribute.DirectionDeviationToNextLap]);
      }

      foreach (Session s in doc.sessions)
      {
        if (!s.Settings.RouteLineSettingsCollection.ContainsKey(WaypointAttribute.DirectionDeviationToNextLap))
        {
          s.Settings.RouteLineSettingsCollection.Add(WaypointAttribute.DirectionDeviationToNextLap, defaultRLS[WaypointAttribute.DirectionDeviationToNextLap]);
        }
      }

      // add circle time radius, introduced in QR 2.4
      foreach (var s in doc.sessions)
      {
        if (s.Settings.CircleTimeRadius == 0) s.Settings.CircleTimeRadius = 45;
      }

      // add map reading duration settings, introduced in QR 2.4
      // add cadence and power, introduced September 2014
      var attributes = new[] { WaypointAttribute.MapReadingDuration, WaypointAttribute.Cadence, WaypointAttribute.Power };

      foreach (var attribute in attributes)
      {
        if (!doc.Settings.ColorRangeIntervalSliderSettings.ContainsKey(attribute))
        {
          var defaultCRISS = DocumentSettings.CreateDefaultColorRangeIntervalSliderSettings();
          doc.Settings.ColorRangeIntervalSliderSettings.Add(attribute, defaultCRISS[attribute]);
        }

        if (!doc.Settings.LapHistogramSettings.ContainsKey(attribute))
        {
          var defaultLHS = DocumentSettings.CreateDefaultLapHistogramSettings();
          doc.Settings.LapHistogramSettings.Add(attribute, defaultLHS[attribute]);
        }

        if (!doc.Settings.DefaultSessionSettings.SmoothingIntervals.ContainsKey(attribute))
        {
          doc.Settings.DefaultSessionSettings.SmoothingIntervals[attribute] = new Interval(0, 0);
        }
      }

      foreach (Session s in doc.sessions)
      {
        foreach (var attribute in attributes)
        {
          if (!s.Settings.RouteLineSettingsCollection.ContainsKey(attribute))
          {
            s.Settings.RouteLineSettingsCollection.Add(attribute, defaultRLS[attribute]);
          }
        }
      }
    }

    private static Bitmap Base64StringToBitmap(string base64String)
    {
      byte[] b = Convert.FromBase64String(base64String);
      MemoryStream ms = new MemoryStream(b);
      Bitmap bmp = (Bitmap)Image.FromStream(ms);
      ms.Close();
      return bmp;
    }

    private static string BitmapToBase64String(Image bmp)
    {
      string str;
      MemoryStream ms = new MemoryStream();
      bmp.Save(ms, ImageFormat.Png);
      str = Convert.ToBase64String(ms.ToArray());
      ms.Close();
      return str;
    }

    private Size GetMapImageSize(double zoomValue)
    {
      return new Size((int)(zoomValue * Map.Image.Width), (int)(zoomValue * Map.Image.Height));
    }

    /// <summary>
    /// Converts a map image pixel coordinate to a longitude and latitude coordinate
    /// </summary>
    /// <param name="mapImagePosition">Map pixel coordinate, referring to unzoomed map without any borders and image header</param>
    /// <returns></returns>
    public LongLat GetLongLatForMapImagePosition(PointD mapImagePosition, GeneralMatrix averageTransformationMatrixInverse)
    {
      var projectedPosition = LinearAlgebraUtil.Transform(mapImagePosition, averageTransformationMatrixInverse);
      return LongLat.Deproject(projectedPosition, ProjectionOrigin);
    }

    private static void AdjustFrameToImage(RectangleD frame, Image image)
    {
      if (frame.Left < 0)
      {
        frame.Width += frame.Left;
        frame.Left = 0;
      }
      if (frame.Top < 0)
      {
        frame.Height += frame.Top;
        frame.Top = 0;
      }
      if (frame.Right > image.Width)
      {
        frame.Width -= frame.Right - image.Width;
      }
      if (frame.Bottom > image.Height)
      {
        frame.Height -= frame.Bottom - image.Height;
      }
    }

    private static RectangleD GetFrame(double zoomValue, Session sessionToDraw, IList<int> legsToDraw, double frameWidth, WaypointAttribute colorCodingAttribute, RouteDrawingMode mode)
    {
      var vertices = sessionToDraw.GetAdjustedWaypointLocations(legsToDraw);
      var rls = sessionToDraw.Settings.RouteLineSettingsCollection[colorCodingAttribute];
      double lineRadius = 0;
      switch(mode)
      {
        case RouteDrawingMode.Simple:
          lineRadius = rls.MonochromeWidth/2;
          break;
        case RouteDrawingMode.Extended:
          lineRadius = (rls.Width + (rls.MaskVisible ? 2 * rls.MaskWidth : 0)) / 2;
          break;
        case RouteDrawingMode.None:
          lineRadius = 0;
          break;
      }

      double minX = zoomValue * (vertices[0].X - lineRadius);
      double maxX = zoomValue * (vertices[0].X + lineRadius);
      double minY = zoomValue * (vertices[0].Y - lineRadius);
      double maxY = zoomValue * (vertices[0].Y + lineRadius);

      for (int i = 0; i < vertices.Count; i++)
      {
        minX = Math.Min(minX, zoomValue * (vertices[i].X - lineRadius));
        maxX = Math.Max(maxX, zoomValue * (vertices[i].X + lineRadius));
        minY = Math.Min(minY, zoomValue * (vertices[i].Y - lineRadius));
        maxY = Math.Max(maxY, zoomValue * (vertices[i].Y + lineRadius));
      }

      double maxWidth = Math.Max(maxX - minX, maxY - minY);
      return new RectangleD(new PointD(minX - frameWidth * maxWidth, minY - frameWidth * maxWidth),
                            new SizeD(maxX - minX + 2 * frameWidth * maxWidth, maxY - minY + 2 * frameWidth * maxWidth));
    }

    public static string GetVersionString()
    {
      Version version = Assembly.GetExecutingAssembly().GetName(false).Version;
      return version.Major + "." + version.Minor + (version.Build > 0 ? "." + version.Build : "");
    }

    #endregion

    #region Static methods

    public static Document Open(string fileName)
    {
      return Open(fileName, new DocumentSettings()); 
    }

    public static Document Open(string fileName, DocumentSettings defaultSettings)
    {
      fileName = CommonUtil.GetDownloadedFileName(fileName);
      switch(GetFileFormat(fileName))
      {
        case QuickRouteFileFormat.Qrt:
          return OpenFromQrt(fileName);
        case QuickRouteFileFormat.Jpeg:
          return OpenFromJpeg(fileName, defaultSettings);
        case QuickRouteFileFormat.Xml:
          return OpenFromXml(fileName, defaultSettings);
      }
      return null;
    }

    public static QuickRouteFileFormat GetFileFormat(string fileName)
    {
      // 1. check jpg
      var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      if(stream.Length >= 11)
      {
        var jpegHeader1 = new byte[4];
        var jpegHeader2 = new byte[5];
        stream.Read(jpegHeader1, 0, 4);
        stream.Position = 6;
        stream.Read(jpegHeader2, 0, 5);
        if (ArraysAreEqual(jpegHeader1, new byte[] { 0xff, 0xd8, 0xff, 0xe0 }) &&
            ArraysAreEqual(jpegHeader2, new byte[] {0x4a, 0x46, 0x49, 0x46, 0x00}))
        {
          stream.Close();
          return QuickRouteFileFormat.Jpeg;
        }
      }

      // 2. check xml
      stream.Position = 0;
      var textReader = new StreamReader(stream);
      var startChars = new char[300];
      textReader.Read(startChars, 0, 300);
      if(startChars.ToString().Contains("<QuickRoute>") && startChars.ToString().Contains("<Route>"))
      {
        textReader.Close();
        return QuickRouteFileFormat.Xml;
      }

      // 3. check qrt
      if (stream.Length >= 16)
      {
        stream.Position = 0;
        var qrtHeader = new byte[16];
        stream.Read(qrtHeader, 0, 16);
        if (ArraysAreEqual(qrtHeader, new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }))
        {
          stream.Close();
          return QuickRouteFileFormat.Qrt;
        }
      }
      stream.Close();
      return QuickRouteFileFormat.Unknown;
    }

    public static Document OpenFromQrt(string fileName)
    {
      using(var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      {
        return OpenFromQrt(fs, fileName);
      }
    }

    public static Document OpenFromQrt(Stream stream, string fileNameForDocument = null)
    {
      IFormatter formatter = new BinaryFormatter();
      Document doc;
      try
      {
        doc = (Document)formatter.Deserialize(stream);
        doc.FileName = fileNameForDocument;
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        stream.Close();
      }
      doc.FileFormat = QuickRouteFileFormat.Qrt;
      doc.Initialize();
      UpdateDocumentToCurrentVersion(doc);
      return doc;
    }

    /// <summary>
    /// Opens a document stored in the old QuickRoute XML file format. This version can't save documents in this file format.
    /// </summary>
    /// <param name="fileName">The file name of the QuickRoute 1.0 xml document.</param>
    /// <param name="settings">The document settings to apply.</param>
    /// <returns></returns>
    public static Document OpenFromXml(string fileName, DocumentSettings settings)
    {
      XmlTextReader reader = null;

      RouteSegment rs = new RouteSegment();
      HandleCollection handles = new HandleCollection();
      Map map;

      try
      {

        reader = new XmlTextReader(fileName);
        reader.WhitespaceHandling = WhitespaceHandling.None;

        reader.ReadStartElement("QuickRoute");

        reader.ReadStartElement("Route");

        while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
        {
          while (reader.NodeType != XmlNodeType.Element) reader.Read();
          Waypoint t = new Waypoint();
          t.Time = DateTime.Parse(reader.GetAttribute("time"));
          t.LongLat = new LongLat();
          t.LongLat.Longitude = double.Parse(reader.GetAttribute("longitude"));
          t.LongLat.Latitude = double.Parse(reader.GetAttribute("latitude"));
          t.Altitude = double.Parse(reader.GetAttribute("altitude"));
          t.HeartRate = int.Parse(reader.GetAttribute("heartRate"));
          rs.Waypoints.Add(t);
        }
        reader.ReadEndElement();

        reader.ReadStartElement("Markers");
        while (reader.Name == "Handle")
        {
          reader.Read();
          Handle h = new Handle();
          h.ParameterizedLocation = new ParameterizedLocation(0, double.Parse(reader.GetAttribute("value")));
          reader.Read();
          double x = double.Parse(reader.GetAttribute("x"));
          double y = double.Parse(reader.GetAttribute("y"));
          h.Location = new PointD(x, y);
          reader.Read();
          h.TransformationMatrix = new GeneralMatrix(3, 3);
          h.MarkerDrawer = (new ApplicationSettings()).DefaultDocumentSettings.DefaultSessionSettings.MarkerDrawers[MarkerType.Handle];
          for (int row = 0; row < 3; row++)
          {
            for (int col = 0; col < 3; col++)
            {
              reader.Read();
              h.TransformationMatrix.SetElement(row, col, double.Parse(reader.GetAttribute("value")));
            }
          }
          reader.Read();
          reader.ReadEndElement();
          reader.ReadEndElement();
          handles.Add(h);
        }
        reader.ReadEndElement();

        map = new Map(Base64StringToBitmap(reader.ReadElementContentAsString()));

      }
      catch (Exception ex)
      {
        if (reader != null) reader.Close();
        throw new Exception(ex.Message);
      }
      reader.Close();

      List<RouteSegment> routeSegments = new List<RouteSegment>();
      routeSegments.Add(rs);
      Document doc = new Document(map, new Route(routeSegments), new LapCollection(), null, settings);
      foreach (var h in handles)
      {
        doc.Sessions[0].AddHandle(h);
      }
      doc.FileFormat = QuickRouteFileFormat.Xml;
      doc.Initialize();
      UpdateDocumentToCurrentVersion(doc);
      return doc;
    }

    /// <summary>
    /// Opens a document from a jpeg file with embedded QuickRoute Jpeg Extension Data.
    /// </summary>
    /// <param name="fileName">The file name of the jpeg file.</param>
    /// <param name="settings">The document settings to apply.</param>
    /// <returns>A QuickRoute document if the jpeg file contains QuickRoute Jpeg Extension Data, otherwise null.</returns>
    public static Document OpenFromJpeg(string fileName, DocumentSettings settings)
    {
      var ed = QuickRouteJpegExtensionData.FromJpegFile(fileName);
      if (ed == null) return null;
      var mapAndBorderImage = (Bitmap)Image.FromFile(fileName);

      var mapImage = new Bitmap(ed.MapLocationAndSizeInPixels.Width, ed.MapLocationAndSizeInPixels.Height);
      using(var g = Graphics.FromImage(mapImage))
      {
        g.DrawImage(mapAndBorderImage, 
                    new Rectangle(0, 0, ed.MapLocationAndSizeInPixels.Width, ed.MapLocationAndSizeInPixels.Height),
                    ed.MapLocationAndSizeInPixels,
                    GraphicsUnit.Pixel);
      }
      foreach(var pi in mapAndBorderImage.PropertyItems)
      {
        mapImage.SetPropertyItem(pi);
      }

      var exif = new ExifWorks.ExifWorks(ref mapAndBorderImage);
      var qualityByteArray = exif.GetProperty((int)ExifWorks.ExifWorks.TagNames.JPEGQuality, new byte[] { 80 });

      var encodingInfo = new JpegEncodingInfo((double)qualityByteArray[0] / 100);
      using (var ms = new MemoryStream())
      {
        mapImage.Save(ms, encodingInfo.Encoder, encodingInfo.EncoderParams);
        var document = new Document(new Map(ms), settings) { Sessions = ed.Sessions };
        if (document.Sessions.Count > 0) document.ProjectionOrigin = document.Sessions[0].ProjectionOrigin;
        document.FileName = fileName;
        document.FileFormat = QuickRouteFileFormat.Jpeg;
        document.Initialize();
        mapAndBorderImage.Dispose();
        mapImage.Dispose();
        return document;
      }
    }

    private static bool ArraysAreEqual(byte[] array1, byte[] array2)
    {
      if (array1.Length != array2.Length) return false;
      for (var i = 0; i < array1.Length; i++)
      {
        if (array1[i] != array2[i]) return false;
      }
      return true;
    }

    #endregion

    #region Public enums

    public enum RouteDrawingMode
    {
      Simple,
      Extended,
      None
    }

    #endregion

    public void Dispose()
    {
      // todo: how does dispose work for fields? want to dispose map image, etc.
    }
  }

  [Serializable]
  public class DocumentProperties
  {
    public string Name { get; set; }
  }

  internal static class ExifUtil
  {
    public static byte[] GetExifGpsCoordinate(double coordinate)
    {
      var d = (uint)coordinate;
      var m = (uint)(60 * coordinate - 60 * d);
      var s = (uint)(1000000 * (3600 * coordinate - (3600 * d + 60 * m)));

      var result = new byte[24];
      BitConverter.GetBytes(d).CopyTo(result, 0);
      BitConverter.GetBytes((uint)1).CopyTo(result, 4);
      BitConverter.GetBytes(m).CopyTo(result, 8);
      BitConverter.GetBytes((uint)1).CopyTo(result, 12);
      BitConverter.GetBytes(s).CopyTo(result, 16);
      BitConverter.GetBytes((uint)1000000).CopyTo(result, 20);
      return result;
    }

    public static byte[] GetExifGpsPosition(LongLat longLat)
    {
      var lng = BitConverter.GetBytes(Convert.ToInt32(longLat.Longitude * 3600000));
      var lat = BitConverter.GetBytes(Convert.ToInt32(longLat.Latitude * 3600000));
      var x = BitConverter.GetBytes((int)int.MinValue + 1);
      var result = new byte[8];
      lng.CopyTo(result, 0);
      lat.CopyTo(result, 4);
      return result;
    }
  }

  /// <summary>
  /// Class for storing an interval, used e g for smoothing when calculating speeds.
  /// </summary>
  [Serializable]
  public class Interval
  {
    private double start;
    private double end;

    public Interval(double start, double end)
    {
      if (end < start) throw new Exception("End value must be less than start value.");
      this.start = start;
      this.end = end;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="i"></param>
    public Interval(Interval i)
    {
      start = i.start;
      end = i.end;
    }

    /// <summary>
    /// The start (relative to the origin) of the interval)
    /// </summary>
    public double Start
    {
      get { return start; }
      set
      {
        if (end < value) throw new Exception("Start value must be greater than end value.");
        start = value;
      }
    }

    /// <summary>
    /// The end (relative to the origin) of the interval)
    /// </summary>
    public double End
    {
      get { return end; }
      set
      {
        if (value < start) throw new Exception("End value must be less than start value.");
        end = value;
      }
    }

    /// <summary>
    /// Gets the length of the interval.
    /// </summary>
    public double Length
    {
      get { return end - start; }
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (Interval)) return false;
      return Equals((Interval) obj);
    }

    public bool Equals(Interval obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.start == start && obj.end == end;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (start.GetHashCode()*397) ^ end.GetHashCode();
      }
    }
  }

}
