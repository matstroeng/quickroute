using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Forms
{
  public partial class ExportImageSizeSelector : Form 
  {
    private const double minImagePercentualSize = 0.5;
    private const double maxImagePercentualSize = 2;
    private SizeCalculatorDelegate sizeCalculator;

    public SizeCalculatorDelegate SizeCalculator
    {
      get
      {
        return sizeCalculator;
      }
      set
      {
        sizeCalculator = value;
        CalculateSize();
      }
    }
    
    public double PercentualImageSize
    {
      get
      {
        double size;
        if(!double.TryParse(percentualSizeComboBox.Text.Replace("%", ""), out size))
        {
          size = 100;
        }
        return Math.Min(maxImagePercentualSize, Math.Max(minImagePercentualSize, size / 100));
      }
      set
      {
        percentualSizeComboBox.Text = string.Format("{0:p0}", value);
      }
    }

    public Size ImageSize
    {
      get
      {
        if (SizeCalculator != null)
        {
          return SizeCalculator(PercentualImageSize);
        }
        return new Size();
      }
    }

    public ExportImageSizeSelector()
    {
      InitializeComponent();
      percentualSizeComboBox.Text = string.Format("{0:p0}", PercentualImageSize);
    }

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void percentualSizeComboBox_TextChanged(object sender, EventArgs e)
    {
      CalculateSize();
    }

    private void CalculateSize()
    {
      if (SizeCalculator != null)
      {
        Size newSize = SizeCalculator(PercentualImageSize);
        sizeInPixels.Text = string.Format(Resources.Strings.SizeInPixels, newSize.Width, newSize.Height);
      }
      else
      {
        sizeInPixels.Text = "";
      }
    }

    private void percentualSizeComboBox_Leave(object sender, EventArgs e)
    {
      percentualSizeComboBox.Text = string.Format("{0:p0}", PercentualImageSize);
    }


  }
}