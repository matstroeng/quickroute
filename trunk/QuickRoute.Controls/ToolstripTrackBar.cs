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
  public class ToolstripTrackBar : ToolStripControlHost
  {
    public ToolstripTrackBar()
      : base(CreateControlInstance())
    {

    }

    /// <summary>
    /// Export a strongly typed property called colorRangeIntervalSlider - handy to prevent casting everywhere.
    /// </summary>
    public TrackBar TrackBarControl
    {
      get
      {
        return Control as TrackBar;
      }
    }

    private static Control CreateControlInstance()
    {
      TrackBar tb = new TrackBar();
      tb.AutoSize = false;
      tb.Size = new Size(60, 24);
      // Add other initialization code here.
      return tb;
    }

    /// <summary>
    /// Attach to events we want to re-wrap
    /// </summary>
    /// <param name="control"></param>
    protected override void OnSubscribeControlEvents(Control control)
    {
      base.OnSubscribeControlEvents(control);
      TrackBarControl.ValueChanged += new System.EventHandler(TrackBarControl_ValueChanged);
      TrackBarControl.MouseDown += new System.Windows.Forms.MouseEventHandler(TrackBarControl_MouseDown);
    }
    /// <summary>
    /// Detach from events.
    /// </summary>
    /// <param name="control"></param>
    protected override void OnUnsubscribeControlEvents(Control control)
    {
      base.OnUnsubscribeControlEvents(control);
      TrackBarControl.ValueChanged -= new System.EventHandler(TrackBarControl_ValueChanged);
      TrackBarControl.MouseDown -= new System.Windows.Forms.MouseEventHandler(TrackBarControl_MouseDown);
    }

    void TrackBarControl_ValueChanged(object sender, EventArgs e)
    {
      if (this.ValueChanged != null) this.ValueChanged(sender, e);
    }

    void TrackBarControl_MouseDown(object sender, MouseEventArgs e)
    {
      if (this.MouseDown != null) this.MouseDown(sender, e);
    }
    
    // add events that are subscribable from the designer.
    public event EventHandler ValueChanged;
    public new event MouseEventHandler MouseDown;

    // set other defaults that are interesting
    protected override Size DefaultSize
    {
      get
      {
        return new Size(60, 24);
      }
    }

  }

}
