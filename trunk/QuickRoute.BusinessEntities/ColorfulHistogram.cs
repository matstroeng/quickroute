using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Numeric;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace QuickRoute.BusinessEntities
{
  public class ColorfulHistogram
  {
    private List<double> histogramData = new List<double>(new double[] { 1, 2, 3, 5, 9 });
    private ColorRange colorRange = new ColorRange(new Gradient(Color.Red, Color.Blue), 0, 10);
    private double barSpacing = 0;
    private Pen barBorderPen = new Pen(Color.FromArgb(128, Color.Black), 0.5F);
    private double startValue = 0;
    private double endValue = 10;
    private double binWidth = 2;
    private ScaleCreatorBase xAxisScaleCreator;
    private NumericConverter xAxisNumericConverter;
    private Pen borderPen = new Pen(Color.Black, 1F);
    private Pen gridPen = new Pen(Color.FromArgb(128, Color.Gray), 1F);
    private Color fontColor = Color.Black;
    private Color histogramBackColor = Color.White;
    private string xAxisCaption;
    private string yAxisCaption;
    private bool includeOutOfRangeValues;
    private Font font = new Font("MS Sans Serif", 8.25F);
    private Color backColor = SystemColors.Control;
    private bool cumulative;

    #region Public properties

    ~ColorfulHistogram()
    {
      barBorderPen.Dispose();
      borderPen.Dispose();
      gridPen.Dispose();
      font.Dispose();
    }

    [CategoryAttribute("Data"), DescriptionAttribute("The histogram data expressed as a list of doubles.")]
    public List<double> HistogramData
    {
      get { return histogramData; }
      set { histogramData = value; }
    }

    public ColorRange ColorRange
    {
      get { return colorRange; }
      set { colorRange = value; }
    }

    public double BarSpacing
    {
      get { return barSpacing; }
      set { barSpacing = value; }
    }

    public Pen BarBorderPen
    {
      get { return barBorderPen; }
      set { barBorderPen = value; }
    }

    public double StartValue
    {
      get { return startValue; }
      set { startValue = value; }
    }

    public double EndValue
    {
      get { return endValue; }
      set { endValue = value; }
    }

    public double BinWidth
    {
      get { return binWidth; }
      set { binWidth = value; }
    }

    public Pen BorderPen
    {
      get { return borderPen; }
      set { borderPen = value; }
    }

    public Pen GridPen
    {
      get { return gridPen; }
      set { gridPen = value; }
    }

    public Color FontColor
    {
      get { return fontColor; }
      set { fontColor = value; }
    }

    public Color HistogramBackColor
    {
      get { return histogramBackColor; }
      set { histogramBackColor = value; }
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

    public bool IncludeOutOfRangeValues
    {
      get { return includeOutOfRangeValues; }
      set { includeOutOfRangeValues = value; }
    }

    public ScaleCreatorBase XAxisScaleCreator
    {
      get { return xAxisScaleCreator; }
      set { xAxisScaleCreator = value; }
    }

    public NumericConverter XAxisNumericConverter
    {
      get { return xAxisNumericConverter; }
      set { xAxisNumericConverter = value; }
    }

    public Font Font
    {
      get { return font; }
      set { font = value; }
    }

    public Color BackColor
    {
      get { return backColor; }
      set { backColor = value; }
    }

    public bool Cumulative
    {
      get { return cumulative; }
      set { cumulative = value; }
    }

    #endregion

    public void Draw(Graphics g, RectangleF drawingRectangle)
    {
      if (NoOfBins <= 0) return;
      int[] bins = CreateBins();
      double maxBarValue = 0;
      g.SmoothingMode = SmoothingMode.AntiAlias;
      g.TextRenderingHint = TextRenderingHint.AntiAlias;

      // get max bar height
      for (int i = 0; i < NoOfBins; i++)
      {
        maxBarValue = Math.Max(maxBarValue, bins[i]);
      }
      if (maxBarValue == 0) return;
      // allow some extra space at top
      double spaceFactor = 1.1;
      double maxPercentage = Math.Min(1, spaceFactor * maxBarValue / histogramData.Count);
      DoubleScaleCreator yAxisScaleCreator = new DoubleScaleCreator(0, maxPercentage, 5, true);
      maxPercentage = yAxisScaleCreator.ScaleEndValue;

      // calculate max x axis label size
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
        string label = String.Format("{0:P0}", value);
        SizeF labelSize = g.MeasureString(label, font);
        maxYAxisLabelSize.Width = Math.Max(maxYAxisLabelSize.Width, labelSize.Width);
        maxYAxisLabelSize.Height = Math.Max(maxYAxisLabelSize.Height, labelSize.Height);
      }

      // calculate the graph rectangle
      SizeF xAxisCaptionSize = g.MeasureString(xAxisCaption, font);
      SizeF yAxisCaptionSize = g.MeasureString(yAxisCaption, font);
      float leftMargin = Math.Max(yAxisCaptionSize.Height + maxYAxisLabelSize.Width, maxXAxisLabelSize.Width / 2);
      RectangleF histogramRectangle = new RectangleF(
        (int)(drawingRectangle.Left + leftMargin),
        (int)(drawingRectangle.Top + maxYAxisLabelSize.Height / 2),
        Math.Max(1, (int)(drawingRectangle.Width - leftMargin - maxXAxisLabelSize.Width / 2 - 1)),
        Math.Max(1, (int)(drawingRectangle.Height - maxYAxisLabelSize.Height / 2 - maxXAxisLabelSize.Height - xAxisCaptionSize.Height - 1)));
      float barWidth = histogramRectangle.Width / (float)((endValue - startValue) / binWidth);


      // clear graphics
      g.Clear(BackColor);

      // draw the histogram background
      Brush b = new SolidBrush(histogramBackColor);
      g.FillRectangle(b, histogramRectangle);
      b.Dispose();

      // draw the axis labels
      // x axis
      Brush fontBrush = new SolidBrush(fontColor);
      for (int i = 0; i < xAxisScaleCreator.NoOfMarkers; i++)
      {
        double value = xAxisScaleCreator.MarkerValue(i);
        int x = (int)histogramRectangle.Left + (int)((float)(value - startValue) / (float)(endValue - startValue) * histogramRectangle.Width);
        string label = xAxisNumericConverter.ToString(value);
        SizeF labelSize = g.MeasureString(label, font);
        g.DrawString(label, font, fontBrush, new PointF(x - labelSize.Width / 2, histogramRectangle.Bottom));
        g.DrawLine(gridPen, new Point(x, (int)histogramRectangle.Top), new Point(x, (int)histogramRectangle.Bottom));
      }
      // y axis
      for (int i = 0; i < yAxisScaleCreator.NoOfMarkers; i++)
      {
        double value = yAxisScaleCreator.MarkerValue(i);
        int y = (int)histogramRectangle.Bottom - (int)((float)(value / maxPercentage) * histogramRectangle.Height);
        string label = String.Format("{0:P0}", value);
        SizeF labelSize = g.MeasureString(label, font);
        g.DrawString(label, font, fontBrush, new PointF(histogramRectangle.Left - labelSize.Width, y - maxYAxisLabelSize.Height / 2));
        g.DrawLine(gridPen, new Point((int)histogramRectangle.Left, y), new Point((int)histogramRectangle.Right, y));
      }



      // draw the axis captions
      // x axis
      PointF xAxisCaptionLocation = new PointF(
        histogramRectangle.Left + (histogramRectangle.Width - xAxisCaptionSize.Width) / 2,
        histogramRectangle.Bottom + xAxisCaptionSize.Height);
      g.DrawString(xAxisCaption, Font, fontBrush, xAxisCaptionLocation);
      // y axis, rotated 90 degrees
      PointF yAxisCaptionLocation = new PointF(
        histogramRectangle.Left - 3 * yAxisCaptionSize.Height,
        histogramRectangle.Bottom - (histogramRectangle.Height - yAxisCaptionSize.Width) / 2);
      g.ResetTransform();
      g.TranslateTransform(-yAxisCaptionLocation.X, -yAxisCaptionLocation.Y, MatrixOrder.Append);
      g.RotateTransform(-90F, MatrixOrder.Append);
      g.TranslateTransform(yAxisCaptionLocation.X, yAxisCaptionLocation.Y, MatrixOrder.Append);
      g.DrawString(yAxisCaption, Font, fontBrush, yAxisCaptionLocation);
      g.ResetTransform();


      // draw the bars
      g.SetClip(histogramRectangle);
      for (int i = 0; i < NoOfBins; i++)
      {
        float barHeight = bins[i] / (float)histogramData.Count / (float)maxPercentage * histogramRectangle.Height;
        RectangleF barRectangle = new RectangleF(
          histogramRectangle.Left + i * barWidth,
          histogramRectangle.Top + histogramRectangle.Height - barHeight,
          barWidth,
          barHeight
          );
        Color barColor = colorRange.GetColor(startValue + (i + 0.5) * binWidth);
        b = new SolidBrush(barColor);
        g.FillRectangle(b, barRectangle);
        b.Dispose();
        if (barBorderPen != null) g.DrawRectangle(barBorderPen, barRectangle.X, barRectangle.Y, barRectangle.Width, barRectangle.Height);
      }
      g.ResetClip();

      // draw the histogram border
      g.DrawRectangle(borderPen, histogramRectangle.X, histogramRectangle.Y, histogramRectangle.Width, histogramRectangle.Height);

      fontBrush.Dispose();
    }



    private int NoOfBins
    {
      get { return (int)Math.Ceiling((endValue - startValue) / binWidth); }
    }

    private int[] CreateBins()
    {
      int[] bins = new int[NoOfBins];

      foreach (double value in histogramData)
      {
        int binIndex = (int)((value - startValue) / binWidth);
        if (includeOutOfRangeValues || cumulative)
        {
          binIndex = Math.Max(Math.Min(binIndex, NoOfBins - 1), 0);
        }
        if (binIndex >= 0 && binIndex < NoOfBins)
        {
          bins[binIndex] += 1;
        }
      }

      if (cumulative)
      {
        int sum = 0;
        for (int i = 0; i < NoOfBins - 1; i++)
        {
          sum += bins[i];
          bins[i] = sum;
        }
      }

      return bins;
    }

  }
}
