using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.Controls
{
  public partial class ColorRangeIntervalSlider : UserControl
  {
    private double minValue = 0.0;
    private double maxValue = 1.0;
    private ColorRange colorRange = new ColorRange(new Gradient(Color.White, Color.Black), 0.0, 1.0);
    private Rectangle colorRangeRectangle;
    private SliderType currentSlider = SliderType.None;
    private Bitmap backBufferBitmap;
    private Graphics backBufferGraphics;
    private ScaleCreatorBase scaleCreator = new DoubleScaleCreator(0, 1, 11, false);
    private const int MOUSE_CLOSENESS = 16;
    private Size sliderSize = new Size(11, 6);
    private NumericConverter numericConverter;
    private double alphaAdjustment;
    private bool preventRedraw;

    public event EventHandler ColorRangeStartValueChanged;
    public event EventHandler ColorRangeEndValueChanged;
    public event EventHandler ColorRangeStartValueChanging;
    public event EventHandler ColorRangeEndValueChanging;
    public event MouseEventHandler ColorRangeClicked;

    public ColorRangeIntervalSlider()
    {
      InitializeComponent();
      colorRangeRectangle = new Rectangle(new Point(0, 0), Size);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      CreateColorRangeRectangle();
      CreateBackBuffer();
    }

    ~ColorRangeIntervalSlider()
    {
      if (backBufferBitmap != null) backBufferBitmap.Dispose();
      if (backBufferGraphics != null) backBufferGraphics.Dispose();
    }

    public double MinValue
    {
      get { return minValue; }
      set { minValue = value; }
    }

    public double MaxValue
    {
      get { return maxValue; }
      set { maxValue = value; }
    }

    public ColorRange ColorRange
    {
      get { return colorRange; }
      set 
      {
        if (colorRange != null)
        {
          colorRange.StartValueChanged -= new EventHandler(ColorRange_StartValueChanged);
          colorRange.EndValueChanged -= new EventHandler(ColorRange_EndValueChanged);
        }
        colorRange = value;
        colorRange.StartValueChanged += new EventHandler(ColorRange_StartValueChanged);
        colorRange.EndValueChanged += new EventHandler(ColorRange_EndValueChanged);
        Draw();
      }
    }

    public ScaleCreatorBase ScaleCreator
    {
      get { return scaleCreator; }
      set 
      { 
        scaleCreator = value;
        Draw();
      }
    }

    public NumericConverter NumericConverter
    {
      get { return numericConverter; }
      set
      {
        numericConverter = value;
        Draw();
      }
    }

    public double AlphaAdjustment
    {
      get { return alphaAdjustment; }
      set { alphaAdjustment = Math.Min(1, Math.Max(-1, value)); }
    }

    public bool PreventRedraw
    {
      get { return preventRedraw; }
      set 
      { 
        preventRedraw = value;
        Draw();
      }
    }


    private void Draw()
    {
      if (backBufferGraphics == null || preventRedraw) return;

      // use backbuffering
      Graphics g = backBufferGraphics;
      // clear background
      g.Clear(BackColor);

      // gradient
      double intervalLength = (colorRange.EndValue - colorRange.StartValue);
      double startLocation = 0.0 - (colorRange.StartValue - minValue) / intervalLength;
      double endLocation = 1.0 + (maxValue - colorRange.EndValue) / intervalLength;
      Gradient.FillCheckerboardRectangle(g, colorRangeRectangle, 8);
      ColorRange.Gradient.Draw(g, colorRangeRectangle, startLocation, endLocation, Gradient.Direction.Horizontal, alphaAdjustment);

      // start value slider
      DrawSlider(new Point(ValueToX(colorRange.StartValue), colorRangeRectangle.Bottom + 1));

      // end value slider
      DrawSlider(new Point(ValueToX(colorRange.EndValue), colorRangeRectangle.Bottom + 1));

      // border
      g.DrawRectangle(Pens.Gray, new Rectangle(colorRangeRectangle.Left - 1, colorRangeRectangle.Top - 1, colorRangeRectangle.Width + 1, colorRangeRectangle.Height + 1));

      // marker lines and labels
      SizeF labelSize;
      int startY;
      if (NumericConverter == null)
      {
        labelSize = new SizeF(0F, 0F);
        startY = colorRangeRectangle.Top;
      }
      else
      {
        labelSize = g.MeasureString(NumericConverter.ToString(scaleCreator.LastMarkerValue) + "    ", Font);
        startY = colorRangeRectangle.Top + (int)labelSize.Height;
      }
      float lastLabelX = (float)colorRangeRectangle.Left - labelSize.Width / 2;
      for (double value = scaleCreator.FirstMarkerValue; value <= scaleCreator.LastMarkerValue; value += scaleCreator.MarkerInterval)
      {
        Pen p = new Pen(Color.FromArgb(192, Color.Black));
        g.DrawLine(
          p,
          (int)ValueToX(value),
          startY,
          ValueToX(value),
          (int)colorRangeRectangle.Bottom
        );
        p.Dispose();
        if (NumericConverter != null)
        {
          float labelX = (float)ValueToX(value);
          // only draw label if it is enough space to the last one
          if (labelX > lastLabelX + labelSize.Width && labelX + labelSize.Width / 2 < (float)colorRangeRectangle.Right) 
          {
            g.DrawString(
              "  " + NumericConverter.ToString(value),
              Font,
              Brushes.Black,
              new PointF(labelX -labelSize.Width / 2, (float)colorRangeRectangle.Top)
            );
            lastLabelX = labelX;
          }
        }
      }

      // copy to screen
      sliderPanel.CreateGraphics().DrawImageUnscaled(backBufferBitmap, new Point(0, 0));
    }

    private double XToValue(int x)
    {
      return minValue + (double)(x - colorRangeRectangle.Left) / (double)(colorRangeRectangle.Width) * (maxValue - minValue);
    }

    private int ValueToX(double value)
    {
      return colorRangeRectangle.Left + (int)((value - minValue) / (maxValue - minValue) * colorRangeRectangle.Width);
    }

    private void CreateBackBuffer()
    {
      if (Width > 0 && Height > 0)
      {
        if (backBufferBitmap != null) backBufferBitmap.Dispose();
        if (backBufferGraphics != null) backBufferGraphics.Dispose();
        backBufferBitmap = new Bitmap(Width, Height);
        backBufferGraphics = Graphics.FromImage(backBufferBitmap);
        backBufferGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      }
    }

    private void CreateColorRangeRectangle()
    {
      colorRangeRectangle = new Rectangle(
        new Point(sliderSize.Width / 2 + 1, 1),
        new Size(Width - 2 * (sliderSize.Width / 2 + 1), Height - sliderSize.Height - 2));
    }

    private void DrawSlider(Point p)
    {
      Point[] points = new Point[3];

      points[0] = new Point(p.X, p.Y);
      points[1] = new Point(p.X + sliderSize.Width / 2, p.Y + sliderSize.Height);
      points[2] = new Point(p.X - sliderSize.Width / 2, p.Y + sliderSize.Height);

      backBufferGraphics.FillPolygon(Brushes.Black, points);
    }

    #region Event handlers

    private void ColorRangeIntervalSlider_MouseDown(object sender, MouseEventArgs e)
    {
      if (colorRangeRectangle.Contains(e.Location))
      {
        if (ColorRangeClicked != null) ColorRangeClicked(this, e);
      }
      else
      {
        // get closest slider
        int startCloseness = Math.Abs(e.X - ValueToX(colorRange.StartValue));
        int endCloseness = Math.Abs(e.X - ValueToX(colorRange.EndValue));

        if (startCloseness < endCloseness && startCloseness < MOUSE_CLOSENESS)
        {
          currentSlider = SliderType.Start;
          colorRange.StartValue = Math.Max(minValue, Math.Min(maxValue, XToValue(e.X)));
          Draw();
        }
        else if (endCloseness < MOUSE_CLOSENESS)
        {
          currentSlider = SliderType.End;
          colorRange.EndValue = Math.Max(minValue, Math.Min(maxValue, XToValue(e.X)));
          Draw();
        }
        else
        {
          currentSlider = SliderType.None;
        }
      }
    }

    private void ColorRangeIntervalSlider_MouseMove(object sender, MouseEventArgs e)
    {
      switch (currentSlider)
      {
        case SliderType.Start:
          colorRange.StartValue = Math.Max(minValue, Math.Min(maxValue, XToValue(e.X)));
          Draw();
          break;
        case SliderType.End:
          colorRange.EndValue = Math.Max(minValue, Math.Min(maxValue, XToValue(e.X)));
          Draw();
          break;
      }
    }

    private void ColorRangeIntervalSlider_MouseUp(object sender, MouseEventArgs e)
    {
      if (currentSlider == SliderType.Start)
      {
        if (ColorRangeStartValueChanged != null) ColorRangeStartValueChanged(this, new EventArgs());
      }
      else if(currentSlider == SliderType.End)
      {
        if (ColorRangeEndValueChanged != null) ColorRangeEndValueChanged(this, new EventArgs());
      }
      currentSlider = SliderType.None;
    }

    private void ColorRangeIntervalSlider_Resize(object sender, EventArgs e)
    {
      CreateColorRangeRectangle();
      CreateBackBuffer();
    }

    private void ColorRange_StartValueChanged(object sender, EventArgs e)
    {
      Draw();
      if (currentSlider == SliderType.None)
      {
        if (ColorRangeStartValueChanged != null) ColorRangeStartValueChanged(this, new EventArgs());
      }
      else
      {
        if (ColorRangeStartValueChanging != null) ColorRangeStartValueChanging(this, new EventArgs());
      }
    }

    private void ColorRange_EndValueChanged(object sender, EventArgs e)
    {
      Draw();
      if (currentSlider == SliderType.None)
      {
        if (ColorRangeEndValueChanged != null) ColorRangeEndValueChanged(this, new EventArgs());
      }
      else
      {
        if (ColorRangeEndValueChanging != null) ColorRangeEndValueChanging(this, new EventArgs());
      }
    }

    private void SliderPanel_Paint(object sender, PaintEventArgs e)
    {
      Draw();
    }

    #endregion

    private enum SliderType
    {
      Start,
      End,
      None
    }

  }
}
