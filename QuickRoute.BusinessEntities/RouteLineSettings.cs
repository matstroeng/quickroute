using System;
using System.Runtime.Serialization;
using System.Drawing;
using QuickRoute.BusinessEntities.GlobalizedProperties;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class RouteLineSettings : GlobalizedObject, ISerializable
  {
    private ColorRange colorRange;
    private double width;
    private Color maskColor;
    private double maskWidth;
    private bool maskVisible;
    private double alphaAdjustment;
    private double monochromeWidth;
    private Color monochromeColor;

    public RouteLineSettings()
    {

    }

    public RouteLineSettings(ColorRange colorRange, double width, Color maskColor, double maskWidth, bool maskVisible, double alphaAdjustment)
    {
      this.colorRange = colorRange;
      this.width = width;
      this.maskColor = maskColor;
      this.maskWidth = maskWidth;
      this.maskVisible = maskVisible;
      AlphaAdjustment = alphaAdjustment;
    }

    protected RouteLineSettings(SerializationInfo info, StreamingContext context)
    {
      colorRange = (ColorRange)(info.GetValue("colorRange", typeof(ColorRange)));
      width = info.GetDouble("width");
      maskColor = Color.FromArgb(info.GetInt32("maskColor"));
      maskWidth = info.GetDouble("maskWidth");
      maskVisible = info.GetBoolean("maskVisible");
      alphaAdjustment = info.GetDouble("alphaAdjustment");
      if(info.MemberCount > 6)
      {
        monochromeColor = Color.FromArgb(info.GetInt32("monochromeColor"));
        monochromeWidth = info.GetDouble("monochromeWidth");
      }
      else
      {
        monochromeColor = Color.FromArgb(160, Color.Red);
        monochromeWidth = 3;
      }
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("colorRange", colorRange, typeof(ColorRange));
      info.AddValue("width", width);
      info.AddValue("maskColor", maskColor.ToArgb());
      info.AddValue("maskWidth", maskWidth);
      info.AddValue("maskVisible", maskVisible);
      info.AddValue("alphaAdjustment", alphaAdjustment);
      info.AddValue("monochromeWidth", monochromeWidth);
      info.AddValue("monochromeColor", monochromeColor.ToArgb());
    }

    public ColorRange ColorRange
    {
      get { return colorRange; }
      set { colorRange = value; }
    }

    public double Width
    {
      get { return width; }
      set { width = value; }
    }

    public Color MaskColor
    {
      get { return maskColor; }
      set { maskColor = value; }
    }

    public double MaskWidth
    {
      get { return maskWidth; }
      set { maskWidth = value; }
    }

    public bool MaskVisible
    {
      get { return maskVisible; }
      set { maskVisible = value; }
    }

    public double AlphaAdjustment
    {
      get { return alphaAdjustment; }
      set { alphaAdjustment = Math.Min(1, Math.Max(-1, value)); }
    }

    public double MonochromeWidth
    {
      get { return monochromeWidth; }
      set { monochromeWidth = value; }
    }

    public Color MonochromeColor
    {
      get { return monochromeColor; }
      set { monochromeColor = value; }
    }

  }
}
