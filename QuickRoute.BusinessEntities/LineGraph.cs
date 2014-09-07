using System;
using System.Collections.Generic;
using QuickRoute.BusinessEntities.Numeric;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace QuickRoute.BusinessEntities
{
  public class LineGraph
  {
    private List<List<LineGraphPoint>> pointsList;
    private Session session;
    public ParameterizedLocation StartPL { get; set; }
    public ParameterizedLocation EndPL { get; set; }

    private DomainAttribute xAxisAttribute;
    private double xAxisMinValue;
    private double xAxisMaxValue;
    private NumericConverter xAxisNumericConverter;
    private ScaleCreatorBase xAxisScaleCreator;
    private string xAxisCaption;

    private WaypointAttribute yAxisAttribute;
    private double yAxisMinValue;
    private double yAxisMaxValue;
    private NumericConverter yAxisNumericConverter;
    private ScaleCreatorBase yAxisScaleCreator;
    private string yAxisCaption;

    private Rectangle graphDrawingRectangle;
    private readonly Font font = new Font("MS Sans Serif", 8.25F);
    private readonly Pen borderPen = new Pen(Color.Black, 1F);
    private readonly Pen gridPen = new Pen(Color.FromArgb(128, Color.Gray), 1F);
    private readonly Pen linePen = new Pen(Color.FromArgb(192, Color.Black), 1.5F);
    private readonly Pen lapPen = new Pen(Color.FromArgb(128, Color.Blue), 1.5F);
    private readonly Color fontColor = Color.Black;
    private readonly Brush lineGraphBackgroundBrush = Brushes.White;

    ~LineGraph()
    {
      font.Dispose();
      borderPen.Dispose();
      gridPen.Dispose();
      linePen.Dispose();
      lapPen.Dispose();
    }

    public string XAxisCaption
    {
      get { return xAxisCaption; }
      set { xAxisCaption = value; }
    }

    public string YAxisCaption
    {
      get { return yAxisCaption; }
      set { yAxisCaption = value; }
    }

    public Session Session
    {
      get { return session; }
      set { session = value; }
    }

    public DomainAttribute XAxisAttribute
    {
      get { return xAxisAttribute; }
      set { xAxisAttribute = value; }
    }

    public WaypointAttribute YAxisAttribute
    {
      get { return yAxisAttribute; }
      set { yAxisAttribute = value; }
    }

    public double XAxisMinValue
    {
      get { return xAxisMinValue; }
      set { xAxisMinValue = value; }
    }

    public double XAxisMaxValue
    {
      get { return xAxisMaxValue; }
      set { xAxisMaxValue = value; }
    }

    public NumericConverter XAxisNumericConverter
    {
      get { return xAxisNumericConverter; }
      set { xAxisNumericConverter = value; }
    }

    public ScaleCreatorBase XAxisScaleCreator
    {
      get { return xAxisScaleCreator; }
      set { xAxisScaleCreator = value; }
    }

    public double YAxisMinValue
    {
      get { return yAxisMinValue; }
      set { yAxisMinValue = value; }
    }

    public double YAxisMaxValue
    {
      get { return yAxisMaxValue; }
      set { yAxisMaxValue = value; }
    }

    public NumericConverter YAxisNumericConverter
    {
      get { return yAxisNumericConverter; }
      set { yAxisNumericConverter = value; }
    }

    public ScaleCreatorBase YAxisScaleCreator
    {
      get { return yAxisScaleCreator; }
      set { yAxisScaleCreator = value; }
    }

    public Rectangle GraphDrawingRectangle
    {
      get { return graphDrawingRectangle; }
    }

    public void Calculate()
    {
      pointsList = new List<List<LineGraphPoint>>();
      DateTime startTime = session.Route.GetTimeFromParameterizedLocation(StartPL);
      DateTime endTime = session.Route.GetTimeFromParameterizedLocation(EndPL);

      foreach (RouteSegment rs in session.Route.Segments)
      {
        var points = new List<LineGraphPoint>();
        for (var i = 0; i < rs.Waypoints.Count; i++)
        {
          var previousWaypoint = i > 0 ? rs.Waypoints[i - 1] : null;
          var thisWaypoint = rs.Waypoints[i];
          var nextWaypoint = i < rs.Waypoints.Count - 1 ? rs.Waypoints[i + 1] : null;

          if (thisWaypoint.Time >= startTime && thisWaypoint.Time <= endTime)
          {
            var p = new LineGraphPoint
                      {
                        X = GetXValue(thisWaypoint),
                        Y = GetYValue(thisWaypoint),
                        Type = (i == 0 ? LineGraphPointType.Start
                                       : (i == rs.Waypoints.Count - 1 ? LineGraphPointType.End : LineGraphPointType.Intermediate))
                      };

            // handle null values 
            List<LineGraphPoint> beforePoints = null;
            List<LineGraphPoint> afterPoints = null;
            if (previousWaypoint != null)
            {
              if (YAxisAttributeIsMapReadingAttribute && thisWaypoint.MapReadingState == MapReadingState.StartReading)
              {
                beforePoints = new List<LineGraphPoint>()
                                 {
                                   new LineGraphPoint() {X = p.X, Y = 0, Type = LineGraphPointType.Intermediate},
                                 };
              }
              else if (!previousWaypoint.Attributes[yAxisAttribute].HasValue && thisWaypoint.Attributes[yAxisAttribute].HasValue)
              {
                beforePoints = new List<LineGraphPoint>() 
                { 
                  new LineGraphPoint() {X = (GetXValue(previousWaypoint) + p.X)/2, Y = 0, Type = LineGraphPointType.Intermediate},
                  new LineGraphPoint() {X = (GetXValue(previousWaypoint) + p.X)/2, Y = p.Y, Type = LineGraphPointType.Intermediate}
                };
              }
            }
            if (nextWaypoint != null)
            {
              if (YAxisAttributeIsMapReadingAttribute && thisWaypoint.MapReadingState == MapReadingState.EndReading)
              {
                afterPoints = new List<LineGraphPoint>()
                                 {
                                   new LineGraphPoint() {X = p.X, Y = 0, Type = LineGraphPointType.Intermediate},
                                 };
              }
              else if (!nextWaypoint.Attributes[yAxisAttribute].HasValue && thisWaypoint.Attributes[yAxisAttribute].HasValue)
              {
                afterPoints = new List<LineGraphPoint>() 
                { 
                  new LineGraphPoint() {X = (GetXValue(nextWaypoint) + p.X)/2, Y = p.Y, Type = LineGraphPointType.Intermediate},
                  new LineGraphPoint() {X = (GetXValue(nextWaypoint) + p.X)/2, Y = 0, Type = LineGraphPointType.Intermediate}
                };
              }
            }
            if (beforePoints != null) points.AddRange(beforePoints);
            points.Add(p);
            if (afterPoints != null) points.AddRange(afterPoints);
          }
        }
        if (points.Count > 0)
        {
          points[0].Type = LineGraphPointType.Start;
          points[points.Count - 1].Type = LineGraphPointType.End;
          pointsList.Add(points);
        }
      }

      bool first = true;
      foreach (var points in pointsList)
      {
        foreach (var p in points)
        {
          if (first || p.X < xAxisMinValue) xAxisMinValue = p.X;
          if (first || p.X > xAxisMaxValue) xAxisMaxValue = p.X;
          //if (first || p.Y < yAxisMinValue) yAxisMinValue = p.Y;
          //if (first || p.Y > yAxisMaxValue) yAxisMaxValue = p.Y;
          first = false;
        }
      }
    }

    private double GetYValue(Waypoint w)
    {
      switch (yAxisAttribute)
      {
        case WaypointAttribute.Speed:
          return w.Attributes[WaypointAttribute.Speed].Value;
        case WaypointAttribute.HeartRate:
          return w.Attributes[WaypointAttribute.HeartRate].GetValueOrDefault(0);
        case WaypointAttribute.Altitude:
          return w.Attributes[WaypointAttribute.Altitude].GetValueOrDefault(0);
        case WaypointAttribute.DirectionDeviationToNextLap:
          return w.Attributes[WaypointAttribute.DirectionDeviationToNextLap].Value;
        case WaypointAttribute.MapReadingDuration:
          return w.Attributes[WaypointAttribute.MapReadingDuration].GetValueOrDefault(0);
        case WaypointAttribute.Cadence:
          return w.Attributes[WaypointAttribute.Cadence].GetValueOrDefault(0);
        case WaypointAttribute.Power:
          return w.Attributes[WaypointAttribute.Power].GetValueOrDefault(0);
        case WaypointAttribute.Pace:
        default:
          return ConvertUtil.ToPace(w.Attributes[WaypointAttribute.Speed].Value).TotalSeconds;
      }
    }

    private double GetXValue(Waypoint w)
    {
      switch (xAxisAttribute)
      {
        case DomainAttribute.ElapsedTime:
          return w.Attributes[WaypointAttribute.ElapsedTime].Value;
        case DomainAttribute.Distance:
          return w.Attributes[WaypointAttribute.Distance].Value;
        case DomainAttribute.TimeOfDay:
        default:
          return (double)w.Time.ToLocalTime().Ticks / TimeSpan.TicksPerSecond;
      }
    }

    public void Draw(Graphics g, RectangleF drawingRectangle)
    {
      if (yAxisNumericConverter == null) return;
      if (yAxisScaleCreator == null) return;

      Brush fontBrush = new SolidBrush(fontColor);

      g.SmoothingMode = SmoothingMode.AntiAlias;
      g.TextRenderingHint = TextRenderingHint.AntiAlias;

      // calculate caption sizes
      SizeF xAxisCaptionSize = g.MeasureString(xAxisCaption, font);
      SizeF yAxisCaptionSize = g.MeasureString(yAxisCaption, font);

      // calculate max x axis label size
      switch (xAxisAttribute)
      {
        case DomainAttribute.TimeOfDay:
          xAxisNumericConverter = new TimeConverter(TimeConverter.TimeConverterType.TimeOfDay);
          XAxisScaleCreator = new TimeScaleCreator(xAxisMinValue, xAxisMaxValue, 15, false);
          break;
        case DomainAttribute.ElapsedTime:
          xAxisNumericConverter = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
          XAxisScaleCreator = new TimeScaleCreator(xAxisMinValue, xAxisMaxValue, 15, false);
          break;
        case DomainAttribute.Distance:
          xAxisNumericConverter = new NumericConverter();
          XAxisScaleCreator = new DoubleScaleCreator(xAxisMinValue, xAxisMaxValue, 15, false);
          break;
      }

      SizeF maxXAxisLabelSize = SizeF.Empty;
      for (int i = 0; i < xAxisScaleCreator.NoOfMarkers; i++)
      {
        double value = xAxisScaleCreator.MarkerValue(i);
        string label = xAxisNumericConverter.ToString(value);
        SizeF labelSize = g.MeasureString(label, font);
        maxXAxisLabelSize.Width = Math.Max(maxXAxisLabelSize.Width, labelSize.Width);
        maxXAxisLabelSize.Height = Math.Max(maxXAxisLabelSize.Height, labelSize.Height);
      }

      // calculate max y axis label size
      SizeF maxYAxisLabelSize = SizeF.Empty;
      for (int i = 0; i < yAxisScaleCreator.NoOfMarkers; i++)
      {
        double value = yAxisScaleCreator.MarkerValue(i);
        string label = yAxisNumericConverter.ToString(value);
        SizeF labelSize = g.MeasureString(label, font);
        maxYAxisLabelSize.Width = Math.Max(maxYAxisLabelSize.Width, labelSize.Width);
        maxYAxisLabelSize.Height = Math.Max(maxYAxisLabelSize.Height, labelSize.Height);
      }

      // calculate graph drawing rectangle
      float leftMargin = Math.Max(1.5F * yAxisCaptionSize.Height + maxYAxisLabelSize.Width, maxXAxisLabelSize.Width / 2);
      int width = (int)(drawingRectangle.Width - leftMargin - maxXAxisLabelSize.Width / 2 - 1);
      int height = (int)(drawingRectangle.Height - maxYAxisLabelSize.Height / 2 - maxXAxisLabelSize.Height - xAxisCaptionSize.Height - 1);

      graphDrawingRectangle = new Rectangle(
        (int)(drawingRectangle.Left + leftMargin),
        (int)(drawingRectangle.Top + maxYAxisLabelSize.Height / 2),
        Math.Max(width, 1),
        Math.Max(height, 1));

      // draw graph background
      g.FillRectangle(lineGraphBackgroundBrush, GraphDrawingRectangle);

      // draw the axis labels
      // x axis
      for (int i = 0; i < xAxisScaleCreator.NoOfMarkers; i++)
      {
        double value = xAxisScaleCreator.MarkerValue(i);
        int x = (int)Math.Round(ValueToX(value));
        string label = xAxisNumericConverter.ToString(value);
        SizeF labelSize = g.MeasureString(label, font);
        g.DrawString(label, font, fontBrush, new PointF(x - labelSize.Width / 2, GraphDrawingRectangle.Bottom));
        g.DrawLine(gridPen, new Point(x, GraphDrawingRectangle.Top), new Point(x, GraphDrawingRectangle.Bottom));
      }
      // y axis
      for (int i = 0; i < yAxisScaleCreator.NoOfMarkers; i++)
      {
        double value = yAxisScaleCreator.MarkerValue(i);
        int y = (int)Math.Round(ValueToY(value));
        string label = yAxisNumericConverter.ToString(value);
        SizeF labelSize = g.MeasureString(label, font);
        g.DrawString(label, font, fontBrush, new PointF(GraphDrawingRectangle.Left - labelSize.Width, y - maxYAxisLabelSize.Height / 2));
        g.DrawLine(gridPen, new Point(GraphDrawingRectangle.Left, y), new Point(GraphDrawingRectangle.Right, y));
      }

      // draw the axis captions
      // x axis
      PointF xAxisCaptionLocation = new PointF(
        GraphDrawingRectangle.Left + (GraphDrawingRectangle.Width - xAxisCaptionSize.Width) / 2,
        GraphDrawingRectangle.Bottom + xAxisCaptionSize.Height);
      g.DrawString(xAxisCaption, font, fontBrush, xAxisCaptionLocation);
      // y axis, rotated 90 degrees
      PointF yAxisCaptionLocation = new PointF(
        GraphDrawingRectangle.Left - 3 * yAxisCaptionSize.Height,
        GraphDrawingRectangle.Bottom - (GraphDrawingRectangle.Height - yAxisCaptionSize.Width) / 2);
      g.ResetTransform();
      g.TranslateTransform(-yAxisCaptionLocation.X, -yAxisCaptionLocation.Y, MatrixOrder.Append);
      g.RotateTransform(-90F, MatrixOrder.Append);
      g.TranslateTransform(yAxisCaptionLocation.X, yAxisCaptionLocation.Y, MatrixOrder.Append);
      g.DrawString(yAxisCaption, font, fontBrush, yAxisCaptionLocation);
      g.ResetTransform();

      // create the line points
      var polygonList = new List<List<PointF>>();
      if (pointsList != null)
      {
        foreach (var points in pointsList)
        {
          GraphicsPath path = new GraphicsPath();
          var polygon = new List<PointF> { new PointF(ValueToX(points[0].X), GraphDrawingRectangle.Bottom) };
          foreach (LineGraphPoint p in points)
          {
            polygon.Add(new PointF(ValueToX(p.X), ValueToY(p.Y)));
          }
          polygon.Add(new PointF(ValueToX(points[points.Count - 1].X), GraphDrawingRectangle.Bottom));
          path.AddPolygon(polygon.ToArray());
          polygonList.Add(polygon);
          g.SetClip(path);

          RouteLineSettings rls = session.Settings.RouteLineSettingsCollection[yAxisAttribute];

          // draw the gradient filling
          Bitmap templateBitmap = new Bitmap(32, GraphDrawingRectangle.Height);
          Graphics templateGraphics = Graphics.FromImage(templateBitmap);
          double colorRangeInterval = rls.ColorRange.EndValue - rls.ColorRange.StartValue;
          if (colorRangeInterval == 0) colorRangeInterval = 0.00001;
          rls.ColorRange.Gradient.Draw(
            templateGraphics,
            new Rectangle(0, 0, templateBitmap.Width, templateBitmap.Height),
            (yAxisMinValue - rls.ColorRange.StartValue) / colorRangeInterval,
            1 + (yAxisMaxValue - rls.ColorRange.EndValue) / colorRangeInterval,
            Gradient.Direction.ReverseVertical);
          templateGraphics.Dispose();
          for (int i = 0; i < GraphDrawingRectangle.Width; i += templateBitmap.Width)
          {
            g.DrawImage(templateBitmap, GraphDrawingRectangle.Left + i, GraphDrawingRectangle.Top);
          }
        }
      }

      g.SetClip(GraphDrawingRectangle);

      // draw lap lines
      foreach (Lap lap in session.Laps)
      {
        double xValue = 0;
        switch (xAxisAttribute)
        {
          case DomainAttribute.TimeOfDay:
            xValue = (double)lap.Time.ToLocalTime().Ticks / TimeSpan.TicksPerSecond;
            break;
          case DomainAttribute.ElapsedTime:
            xValue =
              session.Route.GetAttributeFromParameterizedLocation(
                WaypointAttribute.ElapsedTime,
                session.Route.GetParameterizedLocationFromTime(lap.Time)).Value;
            break;
          case DomainAttribute.Distance:
            xValue =
              session.Route.GetAttributeFromParameterizedLocation(
                WaypointAttribute.Distance,
                session.Route.GetParameterizedLocationFromTime(lap.Time)).Value;
            break;
        }

        int x = (int)Math.Round(ValueToX(xValue));
        g.DrawLine(lapPen, x, graphDrawingRectangle.Top, x, graphDrawingRectangle.Bottom);
      }

      // draw the line above the gradient area
      foreach (var polygon in polygonList)
      {
        g.DrawLines(linePen, polygon.ToArray());
      }
      g.ResetClip();
      g.DrawRectangle(borderPen, GraphDrawingRectangle);

      // dispose drawing objects
      fontBrush.Dispose();
    }

    public float ValueToX(double value)
    {
      if (xAxisMaxValue == xAxisMinValue) return 0;
      return GraphDrawingRectangle.Left +
             (float)((value - xAxisMinValue) / (xAxisMaxValue - xAxisMinValue)) * GraphDrawingRectangle.Width;
    }

    public float ValueToY(double value)
    {
      if (yAxisMaxValue == yAxisMinValue) return 0;
      return GraphDrawingRectangle.Bottom -
             (float)((value - yAxisMinValue) / (yAxisMaxValue - yAxisMinValue)) * GraphDrawingRectangle.Height;
    }

    public double XToValue(int x)
    {
      if (GraphDrawingRectangle.Width == 0) return 0;
      return xAxisMinValue +
             ((double)x - GraphDrawingRectangle.Left) / GraphDrawingRectangle.Width * (xAxisMaxValue - xAxisMinValue);
    }

    public double YToValue(int y)
    {
      if (GraphDrawingRectangle.Height == 0) return yAxisMinValue;
      return yAxisMinValue +
             ((double)GraphDrawingRectangle.Bottom - y) / GraphDrawingRectangle.Height * (yAxisMaxValue - yAxisMinValue);
    }

    public double GetYValueFromXValue(double xValue)
    {
      if (pointsList == null) return 0;
      LineGraphPoint p0 = null;
      LineGraphPoint p1 = null;
      foreach (var points in pointsList)
      {
        foreach (var p in points)
        {
          if (p.X <= xValue && (p0 == null || p.X >= p0.X)) p0 = p;
          if (p.X >= xValue && (p1 == null || p.X < p1.X)) p1 = p;
        }
      }
      if (p0 == null || p1 == null) return 0;
      if (p0.X == p1.X) return p0.Y;
      return p0.Y + (xValue - p0.X) / (p1.X - p0.X) * (p1.Y - p0.Y);
    }

    private bool YAxisAttributeIsMapReadingAttribute
    {
      get { return YAxisAttribute == WaypointAttribute.MapReadingDuration; }
    }
  }

  public class LineGraphPoint : PointD
  {
    public LineGraphPointType Type { get; set; }
  }

  public enum LineGraphPointType
  {
    Start,
    Intermediate,
    End
  }

  public enum DomainAttribute
  {
    TimeOfDay,
    ElapsedTime,
    Distance
  }

}
