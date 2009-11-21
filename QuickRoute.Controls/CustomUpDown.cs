using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.Controls
{
  public partial class CustomUpDown : NumericUpDown 
  {
    readonly double[] deltas = new double[] { 1, 10, 100 };
    double value = 0;
    NumericConverter numericConverter = new NumericConverter();

    public override void DownButton()
    {
      if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (Control.ModifierKeys & Keys.Shift) == Keys.Shift) value -= deltas[2];
      else if ((Control.ModifierKeys & Keys.Control) == Keys.Control) value -= deltas[1];
      else value -= deltas[0];
    }

    public override void UpButton()
    {
      if ((Control.ModifierKeys & Keys.Control) == Keys.Control && (Control.ModifierKeys & Keys.Shift) == Keys.Shift) value += deltas[2];
      else if ((Control.ModifierKeys & Keys.Control) == Keys.Control) value += deltas[1];
      else value += deltas[0];
    }

    protected override void UpdateEditText()
    {
      numericConverter = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
      double? tmpValue = numericConverter.ToNumeric(Text);
      if (tmpValue.HasValue) Value = tmpValue.Value;
    }

    new public double Value
    {
      get { return value; }
      set 
      {
        this.value = value;
        Text = numericConverter.ToString(this.value);
      }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
    }

  }
}
