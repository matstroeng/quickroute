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
  public class ToolstripNumericUpDown : ToolStripControlHost
  {
    public ToolstripNumericUpDown()
      : base(CreateControlInstance())
    {
      this.Enter += new EventHandler(ToolstripNumericUpDown_Enter);
    }

    void ToolstripNumericUpDown_Enter(object sender, EventArgs e)
    {
      NumericUpDownControl.Select(0, NumericUpDownControl.Text.Length);
      NumericUpDownControl.Focus();
    }

    /// <summary>
    /// Export a strongly typed property called colorRangeIntervalSlider - handy to prevent casting everywhere.
    /// </summary>
    public NumericUpDown NumericUpDownControl
    {
      get
      {
        return Control as NumericUpDown;
      }
    }

    private static Control CreateControlInstance()
    {
      NumericUpDown nud = new NumericUpDown();
      nud.AutoSize = false;
      nud.Size = new Size(40, 24);
      // Add other initialization code here.
      return nud;
    }

    /// <summary>
    /// Attach to events we want to re-wrap
    /// </summary>
    /// <param name="control"></param>
    protected override void OnSubscribeControlEvents(Control control)
    {
      base.OnSubscribeControlEvents(control);
      NumericUpDownControl.ValueChanged += new System.EventHandler(NumericUpDownControl_ValueChanged);
    }
    /// <summary>
    /// Detach from events.
    /// </summary>
    /// <param name="control"></param>
    protected override void OnUnsubscribeControlEvents(Control control)
    {
      base.OnUnsubscribeControlEvents(control);
      NumericUpDownControl.ValueChanged -= new System.EventHandler(NumericUpDownControl_ValueChanged);
    }

    void NumericUpDownControl_ValueChanged(object sender, EventArgs e)
    {
      if (this.ValueChanged != null) this.ValueChanged(sender, e);
    }
    // add events that are subscribable from the designer.
    public event EventHandler ValueChanged;

    // set other defaults that are interesting
    protected override Size DefaultSize
    {
      get
      {
        return new Size(40, 24);
      }
    }

  }

}
