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
  public class ToolstripCustomUpDown : ToolStripControlHost
  {
    public ToolstripCustomUpDown()
      : base(CreateControlInstance())
    {
      this.Enter += new EventHandler(ToolstripCustomUpDown_Enter);
    }

    void ToolstripCustomUpDown_Enter(object sender, EventArgs e)
    {
      CustomUpDownControl.Select(0, CustomUpDownControl.Text.Length);
      CustomUpDownControl.Focus();
    }

    public CustomUpDown CustomUpDownControl
    {
      get
      {
        return Control as CustomUpDown;
      }
    }

    private static Control CreateControlInstance()
    {
      CustomUpDown nud = new CustomUpDown();
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
      CustomUpDownControl.ValueChanged += new System.EventHandler(CustomUpDownControl_ValueChanged);
    }
    /// <summary>
    /// Detach from events.
    /// </summary>
    /// <param name="control"></param>
    protected override void OnUnsubscribeControlEvents(Control control)
    {
      base.OnUnsubscribeControlEvents(control);
      CustomUpDownControl.ValueChanged -= new System.EventHandler(CustomUpDownControl_ValueChanged);
    }

    void CustomUpDownControl_ValueChanged(object sender, EventArgs e)
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
