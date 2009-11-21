using System;
using System.Runtime.Serialization;
using System.Drawing;
using QuickRoute.BusinessEntities.GlobalizedProperties;

namespace QuickRoute.BusinessEntities
{
  public interface IMarkerDrawer
  {
    void Draw(Graphics g, PointD p, double zoom);
  }

  [Serializable]
  public class DiscDrawer : GlobalizedObject, IMarkerDrawer, ISerializable
  {
    private Color color = Color.Black;
    private double radius = 8.0;

    public DiscDrawer(Color color, double radius)
    {
      this.color = color;
      this.radius = radius;
    }

    protected DiscDrawer(SerializationInfo info, StreamingContext context)
    {
      color = Color.FromArgb(info.GetInt32("color"));
      radius = info.GetDouble("radius");
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("color", color.ToArgb());
      info.AddValue("radius", radius);
    }


    public void Draw(Graphics g, PointD p, double zoom)
    {
      Brush b = new SolidBrush(color);
      g.FillEllipse(b, (float)(p.X - zoom * radius), (float)(p.Y - zoom * radius), (float)(2.0 * zoom * radius), (float)(2.0 * zoom * radius));
      b.Dispose();
    }

    public Color Color
    {
      get { return color; }
      set { color = value; }
    }

    public double Radius
    {
      get { return radius; }
      set { radius = value; }
    }

  }

  [Serializable]
  public class DiscAndCircleDrawer : GlobalizedObject, IMarkerDrawer, ISerializable
  {
    private Color innerColor = Color.Black;
    private double innerRadius = 8.0;
    private double outerRadius = 12.0;
    private Color outerColor = Color.Red;
    private double outerLineWidth = 2.0;

    public DiscAndCircleDrawer(Color innerColor, double innerRadius, Color outerColor, double outerRadius, double outerLineWidth)
    {
      this.innerColor = innerColor;
      this.innerRadius = innerRadius;
      this.outerColor = outerColor;
      this.outerRadius = outerRadius;
      this.outerLineWidth = outerLineWidth;
    }

    protected DiscAndCircleDrawer(SerializationInfo info, StreamingContext context)
    {
      this.innerColor = Color.FromArgb(info.GetInt32("innerColor"));
      this.innerRadius = info.GetDouble("innerRadius");
      this.outerColor = Color.FromArgb(info.GetInt32("outerColor"));
      this.outerRadius = info.GetDouble("outerRadius");
      this.outerLineWidth = info.GetDouble("outerLineWidth");
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("innerColor", innerColor.ToArgb());
      info.AddValue("innerRadius", innerRadius);
      info.AddValue("outerColor", outerColor.ToArgb());
      info.AddValue("outerRadius", outerRadius);
      info.AddValue("outerLineWidth", outerLineWidth);
    }

    public void Draw(Graphics g, PointD p, double zoom)
    {
      Pen pen = new Pen(outerColor, (float) (zoom*outerLineWidth));
      Brush brush = new SolidBrush(innerColor);
      g.FillEllipse(brush, (float)(p.X - zoom * innerRadius), (float)(p.Y - zoom * innerRadius), (float)(2 * zoom * innerRadius), (float)(2 * zoom * innerRadius));
      g.DrawEllipse(pen, (float)(p.X - zoom * (outerRadius - outerLineWidth / 2)), (float)(p.Y - zoom * (outerRadius - outerLineWidth / 2)), (float)(2 * zoom * (outerRadius - outerLineWidth / 2)), (float)(2 * zoom * (outerRadius - outerLineWidth / 2)));
      pen.Dispose();
      brush.Dispose();
    }

    public Color InnerColor
    {
      get { return innerColor; }
      set { innerColor = value; }
    }

    public double InnerRadius
    {
      get { return innerRadius; }
      set { innerRadius = value; }
    }

    public double OuterRadius
    {
      get { return outerRadius; }
      set { outerRadius = value; }
    }

    public Color OuterColor
    {
      get { return outerColor; }
      set { outerColor = value; }
    }

    public double OuterLineWidth
    {
      get { return outerLineWidth; }
      set { outerLineWidth = value; }
    }
  }

  [Serializable]
  public class CircleDrawer : GlobalizedObject, IMarkerDrawer, ISerializable
  {
    private Color color = Color.Black;
    private double radius = 8.0;
    private double lineWidth = 2.0;

    public CircleDrawer(Color color, double radius, double lineWidth)
    {
      this.color = color;
      this.radius = radius;
      this.lineWidth = lineWidth;
    }

    protected CircleDrawer(SerializationInfo info, StreamingContext context)
    {
      this.Color = Color.FromArgb(info.GetInt32("color"));
      this.Radius = info.GetDouble("radius");
      this.LineWidth = info.GetDouble("lineWidth");
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("color", color.ToArgb());
      info.AddValue("radius", radius);
      info.AddValue("lineWidth", lineWidth);
    }

    public void Draw(Graphics g, PointD p, double zoom)
    {
      Pen pen = new Pen(color, (float) (zoom*lineWidth));
      g.DrawEllipse(pen, (float)(p.X - zoom * (radius - lineWidth / 2)), (float)(p.Y - zoom * (radius - lineWidth / 2)), (float)(2 * zoom * (radius - lineWidth / 2)), (float)(2 * zoom * (radius - lineWidth / 2)));
      pen.Dispose();
    }

    public Color Color
    {
      get { return color; }
      set { color = value; }
    }

    public double Radius
    {
      get { return radius; }
      set { radius = value; }
    }

    public double LineWidth
    {
      get { return lineWidth; }
      set { lineWidth = value; }
    }
  }

}