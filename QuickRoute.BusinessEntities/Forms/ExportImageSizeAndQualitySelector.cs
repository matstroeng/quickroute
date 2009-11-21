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
  public partial class ExportImageSizeAndQualitySelector : Form 
  {
    private const double minImageQuality = 0.1;
    private const double maxImageQuality = 1;
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

    public double ImageQuality
    {
      get
      {
        double quality;
        if (!double.TryParse(imageQualityComboBox.Text.Replace("%", ""), out quality))
        {
          quality = 80;
        }
        return Math.Min(maxImageQuality, Math.Max(minImageQuality, quality / 100));
      }
      set
      {
        imageQualityComboBox.Text = string.Format("{0:p0}", value);
      }
    }

    
    public ExportImageSizeAndQualitySelector()
    {
      InitializeComponent();
      percentualSizeComboBox.Text = string.Format("{0:p0}", PercentualImageSize);
      imageQualityComboBox.Text = string.Format("{0:p0}", ImageQuality);
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

    private void imageQualityComboBox_Leave(object sender, EventArgs e)
    {
      imageQualityComboBox.Text = string.Format("{0:p0}", ImageQuality);
    }


  }
}