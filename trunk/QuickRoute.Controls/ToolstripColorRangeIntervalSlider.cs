using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace QuickRoute.Controls
{
  [DesignerCategory("code")]
  [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
  public class ToolstripColorRangeIntervalSlider : ToolStripControlHost
  {
    // add events that are subscribable from the designer.
    public event EventHandler ColorRangeStartValueChanged;
    public event EventHandler ColorRangeEndValueChanged;
    public event EventHandler ColorRangeStartValueChanging;
    public event EventHandler ColorRangeEndValueChanging;
    public event MouseEventHandler ColorRangeClicked;

    public ToolstripColorRangeIntervalSlider()
      : base(CreateControlInstance())
    {

    }

    /// <summary>
    /// Export a strongly typed property called colorRangeIntervalSlider - handy to prevent casting everywhere.
    /// </summary>
    public ColorRangeIntervalSlider SliderControl
    {
      get
      {
        return Control as ColorRangeIntervalSlider;
      }
    }

    private static Control CreateControlInstance()
    {
      ColorRangeIntervalSlider cris = new ColorRangeIntervalSlider();
      cris.AutoSize = false;
      cris.Size = new Size(200, 24);
      // Add other initialization code here.
      return cris;
    }

    /// <summary>
    /// Attach to events we want to re-wrap
    /// </summary>
    /// <param name="control"></param>
    protected override void OnSubscribeControlEvents(Control control)
    {
      base.OnSubscribeControlEvents(control);
      SliderControl.ColorRangeStartValueChanged += new EventHandler(ColorRangeIntervalSlider_ColorRangeStartValueChanged);
      SliderControl.ColorRangeEndValueChanged += new EventHandler(ColorRangeIntervalSlider_ColorRangeEndValueChanged);
      SliderControl.ColorRangeStartValueChanging += new EventHandler(ColorRangeIntervalSlider_ColorRangeStartValueChanging);
      SliderControl.ColorRangeEndValueChanging += new EventHandler(ColorRangeIntervalSlider_ColorRangeEndValueChanging);
      SliderControl.ColorRangeClicked += new MouseEventHandler(ColorRangeIntervalSlider_ColorRangeClicked);
    }

    /// <summary>
    /// Detach from events.
    /// </summary>
    /// <param name="control"></param>
    protected override void OnUnsubscribeControlEvents(Control control)
    {
      base.OnUnsubscribeControlEvents(control);
      SliderControl.ColorRangeStartValueChanged -= new EventHandler(ColorRangeIntervalSlider_ColorRangeStartValueChanged);
      SliderControl.ColorRangeEndValueChanged -= new EventHandler(ColorRangeIntervalSlider_ColorRangeEndValueChanged);
      SliderControl.ColorRangeStartValueChanging -= new EventHandler(ColorRangeIntervalSlider_ColorRangeStartValueChanging);
      SliderControl.ColorRangeEndValueChanging -= new EventHandler(ColorRangeIntervalSlider_ColorRangeEndValueChanging);
      SliderControl.ColorRangeClicked -= new MouseEventHandler(ColorRangeIntervalSlider_ColorRangeClicked);
    }

    void ColorRangeIntervalSlider_ColorRangeStartValueChanged(object sender, EventArgs e)
    {
      if (this.ColorRangeStartValueChanged != null) this.ColorRangeStartValueChanged(sender, e);
    }

    void ColorRangeIntervalSlider_ColorRangeEndValueChanged(object sender, EventArgs e)
    {
      if (this.ColorRangeEndValueChanged != null) this.ColorRangeEndValueChanged(sender, e);
    }

    void ColorRangeIntervalSlider_ColorRangeStartValueChanging(object sender, EventArgs e)
    {
      if (this.ColorRangeStartValueChanging != null) this.ColorRangeStartValueChanging(sender, e);
    }

    void ColorRangeIntervalSlider_ColorRangeEndValueChanging(object sender, EventArgs e)
    {
      if (this.ColorRangeEndValueChanging != null) this.ColorRangeEndValueChanging(sender, e);
    }

    void ColorRangeIntervalSlider_ColorRangeClicked(object sender, MouseEventArgs e)
    {
      if (this.ColorRangeClicked != null) this.ColorRangeClicked(sender, e);
    }

    // set other defaults that are interesting
    protected override Size DefaultSize
    {
      get
      {
        return new Size(200, 24);
      }
    }

  }

}
