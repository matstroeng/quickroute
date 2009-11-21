using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using QuickRoute.BusinessEntities.Numeric;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.Exporters
{
  public class ImageExporter
  {
    // INPUT FIELDS, to be set before calling the Export method
    private readonly Document document;
    public Document Document
    {
      get { return document; }
    }
    private readonly SessionCollection sessions;
    public SessionCollection Sessions
    {
      get { return sessions; }
    }
    public Stream OutputStream { get; set; }
    public ImageExporterProperties Properties { get; set; }

    // OUTPUT FIELDS, populated during the call to the Export method
    /// <summary>
    /// The bounding rectangle of the whole image
    /// </summary>
    public Rectangle ImageBounds
    {
      get { return Image == null ? Rectangle.Empty : new Rectangle(new Point(0, 0), Image.Size); }
    }

    /// <summary>
    /// The bounding rectangle of the map part of the image
    /// </summary>
    public Rectangle MapBounds { get; private set; }

    public Bitmap Image { get; private set; }

    private const int exportImageHeaderHeight = 64;
    private const int exportImageBorderWidth = 1;

    public ImageExporter(Document document) : this(document, document.Sessions, null)
    {
    }

    public ImageExporter(Document document, SessionCollection sessions) : this(document, sessions, null)
    {
    }

    public ImageExporter(Document document, Stream outputStream): this(document, document.Sessions, outputStream)
    {
    }

    public ImageExporter(Document document, SessionCollection sessions, Stream outputStream)
    {
      this.document = document;
      this.sessions = sessions;
      OutputStream = outputStream;
      // set default values for non-mandatory fields
      Properties = new ImageExporterProperties {SessionSettings = new SessionSettings()};
    }

    public void Export()
    {
      CreateImage();
      SetExifData();
      SetQuickRouteExtensionData();
    }

    private void CreateImage()
    {
      // todo: support for multiple sessions
      var sessionSettings = Properties.SessionSettings ?? Document.Sessions[0].Settings;

      var routeLineSettings = sessionSettings.RouteLineSettingsCollection[Properties.ColorCodingAttribute];

      // create the base image
      var mapAndRouteImage = document.CreateMapAndRouteImage(
        Properties.ShowMap, 
        Properties.PercentualSize, 
        Sessions, 
        Properties.ColorCodingAttribute, 
        Properties.RouteDrawingMode, 
        Properties.SessionSettings
      );

      // create a nice header with logo and color range
      var borderPen = new Pen(Color.FromArgb(255, 64, 64, 64));
      var captionFont = new Font("Calibri", 18.6F, FontStyle.Regular, GraphicsUnit.Pixel);
      Brush captionBrush = new SolidBrush(Color.FromArgb(192, Color.White));
      var versionFont = new Font("Calibri", 13.3F, FontStyle.Regular, GraphicsUnit.Pixel);
      Brush versionBrush = new SolidBrush(Color.FromArgb(128, Color.White));
      var urlFont = new Font("Calibri", 13.3F, FontStyle.Regular, GraphicsUnit.Pixel);
      Brush urlBrush = new SolidBrush(Color.FromArgb(128, Color.White));
      var colorRangeScaleFont = new Font("Calibri", 13.3F, FontStyle.Regular, GraphicsUnit.Pixel);
      Brush colorRangeScaleBrush = new SolidBrush(Color.FromArgb(192, Color.White));
      // copy route image to a new image
      Image = new Bitmap(mapAndRouteImage.Width + 2 * exportImageBorderWidth,
                                    mapAndRouteImage.Height + exportImageHeaderHeight + 3 * exportImageBorderWidth);
      var g = Graphics.FromImage(Image);
      MapBounds = new Rectangle(exportImageBorderWidth, 2 * exportImageBorderWidth + exportImageHeaderHeight, mapAndRouteImage.Width, mapAndRouteImage.Height);

      g.DrawImageUnscaled(mapAndRouteImage, exportImageBorderWidth, 2 * exportImageBorderWidth + exportImageHeaderHeight);
      mapAndRouteImage.Dispose();

      // draw the header

      // border
      g.DrawRectangle(borderPen, 0, 0, Image.Width - 1, Image.Height - 1);
      g.DrawLine(borderPen, 0, exportImageHeaderHeight + exportImageBorderWidth, Image.Width, exportImageHeaderHeight + exportImageBorderWidth);

      // black background
      g.FillRectangle(Brushes.Black, new Rectangle(exportImageBorderWidth, exportImageBorderWidth,
                                    Image.Width - 2 * exportImageBorderWidth, exportImageHeaderHeight));

      // header gradient
      g.FillRectangle(
        new LinearGradientBrush(new Point(0, 0), new Point(0, 28), Color.FromArgb(255, 64, 64, 64),
                                Color.FromArgb(255, 24, 24, 24)),
        new Rectangle(exportImageBorderWidth, exportImageBorderWidth, Image.Width - 2 * exportImageBorderWidth, 28));

      // Q logo
      g.DrawImage(Images.HeaderLogo, exportImageBorderWidth + 16, exportImageBorderWidth + 6);

      // application name, version and url
      g.TextRenderingHint = TextRenderingHint.AntiAlias;
      g.SmoothingMode = SmoothingMode.AntiAlias;
      g.DrawString(Strings.QuickRoute, captionFont, captionBrush, new PointF(exportImageBorderWidth + 74F, exportImageBorderWidth + 5F));
      g.DrawString(
        string.Format(Strings.Version, Document.GetVersionString()),
        versionFont, versionBrush,
        new PointF(exportImageBorderWidth + 76F, exportImageBorderWidth + 27F));
      g.DrawString(Strings.WebsiteShortUrl, urlFont, urlBrush, new PointF(exportImageBorderWidth + 76F, exportImageBorderWidth + 41F));

      if (Properties.RouteDrawingMode == Document.RouteDrawingMode.Extended && Properties.ColorRangeProperties != null)
      {
        DrawColorRange(g, colorRangeScaleFont, routeLineSettings, colorRangeScaleBrush);
      }

      // dispose objects
      borderPen.Dispose();
      captionFont.Dispose();
      captionBrush.Dispose();
      versionFont.Dispose();
      versionBrush.Dispose();
      urlFont.Dispose();
      urlBrush.Dispose();
      colorRangeScaleFont.Dispose();
      colorRangeScaleBrush.Dispose();
      g.Dispose();
    }

    private void DrawColorRange(Graphics graphics, Font colorRangeScaleFont, RouteLineSettings routeLineSettings, Brush colorRangeScaleBrush)
    {
      // draw the color range

      // calculate various sizes
      var sf = new StringFormat();
      var colorRangeSize = new Size(Math.Max(100, Math.Min(300, Image.Width - 300)), 16);
      var colorRangeScaleLabelMaxSize =
        graphics.MeasureString(Properties.ColorRangeProperties.NumericConverter.ToString((double?)Properties.ColorRangeProperties.ScaleCreator.LastMarkerValue),
                        colorRangeScaleFont);
      var colorRangeScaleUnitSize = graphics.MeasureString(" " + Properties.ColorRangeProperties.ScaleUnit, colorRangeScaleFont);
      var colorRangeRectangle =
        new Rectangle(
          new Point(
            Image.Width - exportImageBorderWidth - 16 - (int)colorRangeScaleUnitSize.Width -
            (int)colorRangeScaleLabelMaxSize.Width / 2 - colorRangeSize.Width, 20), colorRangeSize);

      // checkerboard background for color range
      Gradient.FillCheckerboardRectangle(graphics, colorRangeRectangle, 8);

      // color range
      routeLineSettings.ColorRange.Gradient.Draw(graphics, colorRangeRectangle, 0, 1, Gradient.Direction.Horizontal,
                                                 routeLineSettings.AlphaAdjustment);

      // color range scale markers and labels
      var lastX = 0;
      for (var value = Properties.ColorRangeProperties.ScaleCreator.FirstMarkerValue;
           value <= Properties.ColorRangeProperties.ScaleCreator.LastMarkerValue;
           value += Properties.ColorRangeProperties.ScaleCreator.MarkerInterval)
      {
        var x = colorRangeRectangle.Left +
                (int)
                ((value - Properties.ColorRangeProperties.ScaleCreator.FirstMarkerValue) /
                 (routeLineSettings.ColorRange.EndValue - routeLineSettings.ColorRange.StartValue) *
                 colorRangeRectangle.Width);
        var p = new Pen(Color.FromArgb(192, Color.Black));
        graphics.DrawLine(p, x, colorRangeRectangle.Top, x, colorRangeRectangle.Bottom);
        p.Dispose();

        // draw label string, but only if the space is sufficient
        if (value == Properties.ColorRangeProperties.ScaleCreator.FirstMarkerValue || (x - lastX) > colorRangeScaleLabelMaxSize.Width)
        {
          sf.Alignment = StringAlignment.Center;
          sf.LineAlignment = StringAlignment.Near;
          graphics.DrawString(Properties.ColorRangeProperties.NumericConverter.ToString((double?)value), colorRangeScaleFont, colorRangeScaleBrush, x,
                       colorRangeRectangle.Bottom + 2F, sf);
          lastX = x;
        }
      }

      // color range border
      graphics.DrawRectangle(Pens.Gray,
                      new Rectangle(colorRangeRectangle.Left - 1, colorRangeRectangle.Top - 1,
                                    colorRangeRectangle.Width + 2, colorRangeRectangle.Height + 1));

      // color range scale strings
      // caption
      sf.Alignment = StringAlignment.Far;
      sf.LineAlignment = StringAlignment.Near;
      graphics.DrawString(Properties.ColorRangeProperties.ScaleCaption + " ", colorRangeScaleFont, colorRangeScaleBrush,
                   colorRangeRectangle.Left - colorRangeScaleLabelMaxSize.Width / 2F, colorRangeRectangle.Bottom + 2F, sf);

      // unit
      sf.Alignment = StringAlignment.Near;
      sf.LineAlignment = StringAlignment.Near;
      graphics.DrawString(" " + Properties.ColorRangeProperties.ScaleUnit, colorRangeScaleFont, colorRangeScaleBrush,
                   colorRangeRectangle.Right + colorRangeScaleLabelMaxSize.Width / 2F, colorRangeRectangle.Bottom + 2F, sf);
    }

    private void SetExifData()
    {
      // GPS version
      var image = Image;
      var exif = new ExifWorks.ExifWorks(ref image);

      // center coordinate
      var center = new LongLat();
      foreach (var corner in Document.GetMapCornersLongLat())
      {
        center += corner / 4;
      }

      var ver = new byte[] { 2, 2, 0, 0 };
      var longitudeRef = new byte[] { Convert.ToByte(center.Longitude < 0 ? 'W' : 'E'), 0 };
      var longitude = ExifUtil.GetExifGpsCoordinate(center.Longitude);
      var latitudeRef = new byte[] { Convert.ToByte(center.Latitude < 0 ? 'S' : 'N'), 0 };
      var latitude = ExifUtil.GetExifGpsCoordinate(center.Latitude);
      exif.SetProperty((int)ExifWorks.ExifWorks.TagNames.GpsVer, ver, ExifWorks.ExifWorks.ExifDataTypes.UnsignedLong);
      exif.SetProperty((int)ExifWorks.ExifWorks.TagNames.GpsLongitudeRef, longitudeRef, ExifWorks.ExifWorks.ExifDataTypes.AsciiString);
      exif.SetProperty((int)ExifWorks.ExifWorks.TagNames.GpsLongitude, longitude, ExifWorks.ExifWorks.ExifDataTypes.UnsignedRational);
      exif.SetProperty((int)ExifWorks.ExifWorks.TagNames.GpsLatitudeRef, latitudeRef, ExifWorks.ExifWorks.ExifDataTypes.AsciiString);
      exif.SetProperty((int)ExifWorks.ExifWorks.TagNames.GpsLatitude, latitude, ExifWorks.ExifWorks.ExifDataTypes.UnsignedRational);
      if (Properties.EncodingInfo.Encoder.MimeType == "image/jpeg")
      {
        exif.SetProperty((int)ExifWorks.ExifWorks.TagNames.JPEGQuality, new byte[] {(byte)(100 * ((JpegEncodingInfo)Properties.EncodingInfo).Quality)}, ExifWorks.ExifWorks.ExifDataTypes.UnsignedByte);
      }

      exif.SetPropertyString((int)ExifWorks.ExifWorks.TagNames.SoftwareUsed, Strings.QuickRoute + " " + Document.GetVersionString());
    }

    private void SetQuickRouteExtensionData()
    {
      if (Properties.EncodingInfo.Encoder.MimeType == "image/jpeg")
      {
        using (var tmpStream = new MemoryStream())
        {
          Image.Save(tmpStream, Properties.EncodingInfo.Encoder, Properties.EncodingInfo.EncoderParams);
          var ed = QuickRouteJpegExtensionData.FromImageExporter(this);
          ed.EmbedDataInImage(tmpStream, OutputStream);
        }
      }
      else
      {
        Image.Save(OutputStream, Properties.EncodingInfo.Encoder, Properties.EncodingInfo.EncoderParams);
      }
    }

  }

  public class ImageExporterProperties
  {
    public SessionSettings SessionSettings { get; set; }
    public IImageEncodingInfo EncodingInfo { get; set; }
    public double PercentualSize { get; set; }
    public Document.RouteDrawingMode RouteDrawingMode { get; set; }
    public bool ShowMap { get; set; }
    public WaypointAttribute ColorCodingAttribute { get; set; }
    public ColorRangeProperties ColorRangeProperties { get; set; }

    public ImageExporterProperties()
    {
      EncodingInfo = new JpegEncodingInfo(0.8);
      PercentualSize = 1;
      RouteDrawingMode = Document.RouteDrawingMode.Extended;
      ShowMap = true;
      ColorCodingAttribute = WaypointAttribute.Pace;
    }
  }

  // todo: is this class really necessary? could be created on the fly
  public class ColorRangeProperties
  {
    public ScaleCreatorBase ScaleCreator { get; set; }
    public NumericConverter NumericConverter { get; set; }
    public string ScaleCaption { get; set; }
    public string ScaleUnit { get; set; }

  }
}
