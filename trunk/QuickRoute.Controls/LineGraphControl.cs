using System;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using System.Drawing;

namespace QuickRoute.Controls
{
  public partial class LineGraphControl : UserControl
  {
    private LineGraph graph = new LineGraph();
    private Bitmap graphBitmap;
    private Bitmap hoverBitmap;
    private double? hoverXValue;
    private readonly Pen haircrossPen = new Pen(Color.FromArgb(128, Color.Red), 1);
    private readonly DiscAndCircleDrawer haircrossMarker =
      new DiscAndCircleDrawer(Color.FromArgb(192, Color.Red), 4,
                              Color.FromArgb(192, Color.Black), 6, 2);

    public event EventHandler<Canvas.RouteMouseHoverEventArgs> GraphMouseHover;
    public event EventHandler<Canvas.RouteMouseHoverEventArgs> GraphMouseDown;
    public event EventHandler<Canvas.RouteMouseHoverEventArgs> GraphMouseUp;

    public LineGraphControl()
    {
      InitializeComponent();
    }

    ~LineGraphControl()
    {
      haircrossPen.Dispose();
      if (graphBitmap != null) graphBitmap.Dispose();
      if (hoverBitmap != null) hoverBitmap.Dispose();
    }

    public LineGraph Graph
    {
      get { return graph; }
      set { graph = value; }
    }

    public double? HoverXValue
    {
      get { return hoverXValue; }
      set
      {
        hoverXValue = value;
        Draw();
      }
    }

    private void LineGraphControl_Paint(object sender, PaintEventArgs e)
    {
      Draw(e.Graphics);
    }

    private void LineGraphControl_Resize(object sender, System.EventArgs e)
    {
      Create();
      Draw();
    }

    public void Create()
    {
      if (graphBitmap != null) graphBitmap.Dispose();
      if (hoverBitmap != null) hoverBitmap.Dispose();
      if (Width > 0 && Height > 0)
      {
        graphBitmap = new Bitmap(Width, Height);
        hoverBitmap = new Bitmap(Width, Height);
        Graphics g = Graphics.FromImage(graphBitmap);
        g.Clear(BackColor);
        graph.Draw(g, new RectangleF(0, 0, Width, Height));
        g.Dispose();
      }
      else
      {
        graphBitmap = null;
        hoverBitmap = null;
      }
    }

    public void Draw(Graphics g)
    {
      if (graphBitmap == null) return;
      if (hoverXValue.HasValue && hoverXValue.Value >= graph.XAxisMinValue && hoverXValue.Value <= graph.XAxisMaxValue)
      {
        Graphics hoverGraphics = Graphics.FromImage(hoverBitmap);
        hoverGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        hoverGraphics.Clear(BackColor);
        hoverGraphics.DrawImage(graphBitmap, 0, 0);
        PointF centerF = new PointF(graph.ValueToX(hoverXValue.Value),
                                    graph.ValueToY(graph.GetYValueFromXValue(hoverXValue.Value)));
        Point center = new Point((int)centerF.X, (int)centerF.Y);
        center.Y = Math.Max(graph.GraphDrawingRectangle.Top, Math.Min(graph.GraphDrawingRectangle.Bottom, center.Y));
        SizeF size = new SizeF(10, 10);
        RectangleF rect = new RectangleF(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
        hoverGraphics.DrawLine(haircrossPen, center, new Point(center.X, graph.GraphDrawingRectangle.Bottom));
        hoverGraphics.DrawLine(haircrossPen, new Point(graph.GraphDrawingRectangle.Left, center.Y), new Point(graph.GraphDrawingRectangle.Right, center.Y));
        haircrossMarker.Draw(hoverGraphics, new PointD(center.X, center.Y), 1);
        g.DrawImage(hoverBitmap, 0, 0);
        hoverGraphics.Dispose();
      }
      else
      {
        g.DrawImage(graphBitmap, new Point(0, 0));
      }
    }

    public void Draw()
    {
      Draw(CreateGraphics());
    }

    private ParameterizedLocation GetParameterizedLocationFromMouseLocation(int x)
    {
      double? xValue = graph.XToValue(x);
      if (xValue < graph.XAxisMinValue || xValue > graph.XAxisMaxValue) xValue = null;

      ParameterizedLocation pl = null;
      if (xValue != null)
      {
        DateTime time;
        switch (graph.XAxisAttribute)
        {
          case DomainAttribute.TimeOfDay:
            time = new DateTime((long)(xValue.Value * TimeSpan.TicksPerSecond)).ToUniversalTime();
            pl = graph.Session.Route.GetParameterizedLocationFromTime(time, true);
            break;
          case DomainAttribute.ElapsedTime:
            time = graph.Session.Route.FirstWaypoint.Time.AddTicks((long)xValue.Value * TimeSpan.TicksPerSecond);
            pl = graph.Session.Route.GetParameterizedLocationFromTime(time);
            break;
          case DomainAttribute.Distance:
            pl = graph.Session.Route.GetParameterizedLocationFromDistance(xValue.Value);
            break;
        }
      }
      return pl;
    }

    private void LineGraphControl_MouseMove(object sender, MouseEventArgs e)
    {
      if (graph.Session == null) return;
      hoverXValue = graph.XToValue(e.X);
      var pl = GetParameterizedLocationFromMouseLocation(e.X);
      if (pl == null || (hoverXValue.HasValue && (hoverXValue.Value < graph.XAxisMinValue || hoverXValue.Value > graph.XAxisMaxValue))) hoverXValue = null;
      Draw();
      if (GraphMouseHover != null) GraphMouseHover(this, new Canvas.RouteMouseHoverEventArgs(pl, hoverXValue != null));
    }

    private void LineGraphControl_MouseDown(object sender, MouseEventArgs e)
    {
      if (graph.Session == null) return;
      hoverXValue = graph.XToValue(e.X);
      if (hoverXValue.HasValue && hoverXValue.Value >= graph.XAxisMinValue && hoverXValue.Value <= graph.XAxisMaxValue)
      {
        var pl = GetParameterizedLocationFromMouseLocation(e.X);
        if (pl == null) hoverXValue = null; // prevent marker hovering
        Draw();
        if (GraphMouseDown != null) GraphMouseDown(this, new Canvas.RouteMouseHoverEventArgs(pl, hoverXValue != null));
      }
    }

    private void LineGraphControl_MouseUp(object sender, MouseEventArgs e)
    {
      if (graph.Session == null) return;
      hoverXValue = graph.XToValue(e.X);
      if (hoverXValue.HasValue && hoverXValue.Value >= graph.XAxisMinValue && hoverXValue.Value <= graph.XAxisMaxValue)
      {
        var pl = GetParameterizedLocationFromMouseLocation(e.X);
        if (pl == null) hoverXValue = null; // prevent marker hovering
        Draw();
        if (GraphMouseUp != null) GraphMouseUp(this, new Canvas.RouteMouseHoverEventArgs(pl, hoverXValue != null));
      }
    }
  }

}
