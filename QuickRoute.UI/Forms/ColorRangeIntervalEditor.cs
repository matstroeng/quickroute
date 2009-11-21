using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.UI.Forms
{
  public partial class ColorRangeIntervalEditor : Form
  {
    private NumericConverter numericConverter;
    private double intervalStart;
    private double intervalEnd;

    public ColorRangeIntervalEditor(NumericConverter numericConverter, double intervalStart, double intervalEnd)
    {
      InitializeComponent();
      this.numericConverter = numericConverter;
      this.intervalStart = intervalStart;
      this.intervalEnd = intervalEnd;
      UpdateUI();
    }

    public NumericConverter NumericConverter
    {
      get { return numericConverter; }
      set
      {
        numericConverter = value;
        UpdateUI();
      }
    }

    public double IntervalStart
    {
      get { return intervalStart; }
      set 
      {
        intervalStart = Math.Min(value, intervalEnd);
        UpdateUI();
      }
    }

    public double IntervalEnd
    {
      get { return intervalEnd; }
      set
      {
        intervalEnd = Math.Max(value, intervalStart);
        UpdateUI();
      }
    }

    private void IntervalStartTextbox_Leave(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void IntervalEndTextbox_Leave(object sender, EventArgs e)
    {
      UpdateUI();
    }

    private void OK_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void Cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void UpdateUI()
    {
      intervalStartTextbox.Text = numericConverter.ToString(intervalStart);
      intervalEndTextbox.Text = numericConverter.ToString(intervalEnd);
    }

    private void IntervalStartTextbox_Enter(object sender, EventArgs e)
    {
      intervalStartTextbox.SelectAll();
    }

    private void IntervalEndTextbox_Enter(object sender, EventArgs e)
    {
      intervalEndTextbox.SelectAll();
    }

    private void intervalStartTextbox_TextChanged(object sender, EventArgs e)
    {
      double? value = numericConverter.ToNumeric(intervalStartTextbox.Text);
      if (value != null)
      {
        intervalStart = (double)value;
      }
    }

    private void intervalEndTextbox_TextChanged(object sender, EventArgs e)
    {
      double? value = numericConverter.ToNumeric(intervalEndTextbox.Text);
      if (value != null)
      {
        intervalEnd = (double)value;
      }
    }

  }
}