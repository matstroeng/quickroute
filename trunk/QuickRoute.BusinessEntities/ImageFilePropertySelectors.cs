using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ExportImageSizeSelector=QuickRoute.BusinessEntities.Forms.ExportImageSizeSelector;

namespace QuickRoute.BusinessEntities
{
  public delegate Size SizeCalculatorDelegate(double percentualSize);

  public class JpegEncodingInfo : IImageEncodingInfo
  {
    private readonly double quality;

    public JpegEncodingInfo(double quality)
    {
      this.quality = quality;
    }

    public ImageCodecInfo Encoder
    {
      get
      {
        var codecs = ImageCodecInfo.GetImageEncoders();
        foreach (var codec in codecs)
        {
          if (codec.MimeType == "image/jpeg") return codec;
        }
        return null;
      }
    }

    public EncoderParameters EncoderParams
    {
      get
      {
        var ep = new EncoderParameters(1);
        ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)(100 * quality));
        return ep;
      }
    }

    public double Quality
    {
      get
      {
        return quality;
      }
    }
  }

  public class PngEncodingInfo : IImageEncodingInfo
  {
    public ImageCodecInfo Encoder
    {
      get
      {
        var codecs = ImageCodecInfo.GetImageEncoders();
        foreach (var codec in codecs)
        {
          if (codec.MimeType == "image/png") return codec;
        }
        return null;
      }
    }

    public EncoderParameters EncoderParams
    {
      get
      {
        return null;
      }
    }
  }

  public class TiffEncodingInfo : IImageEncodingInfo
  {
    public ImageCodecInfo Encoder
    {
      get
      {
        var codecs = ImageCodecInfo.GetImageEncoders();
        foreach (var codec in codecs)
        {
          if (codec.MimeType == "image/tiff") return codec;
        }
        return null;
      }
    }

    public EncoderParameters EncoderParams
    {
      get { return null; }
    }
  }

  public interface IImageEncodingInfo  
  {
    ImageCodecInfo Encoder { get; }
    EncoderParameters EncoderParams { get; }
  }

  public class JpegPropertySelector : IMapImageFileExporterDialog
  {
    private readonly Forms.ExportImageSizeAndQualitySelector dlg = new Forms.ExportImageSizeAndQualitySelector();

    public double PercentualImageSize
    {
      get
      {
        return dlg.PercentualImageSize;
      }
      set
      {
        dlg.PercentualImageSize = value;
      }
    }
    
    public SizeCalculatorDelegate SizeCalculator { get; set; }

    public IImageEncodingInfo EncodingInfo
    {
      get { return new JpegEncodingInfo(Quality); }
    }

    public double Quality
    {
      get
      {
        return dlg.ImageQuality;
      }
      set
      {
        dlg.ImageQuality = value;
      }
    }

    public DialogResult ShowPropertyDialog()
    {
      dlg.SizeCalculator = SizeCalculator;
      DialogResult result = dlg.ShowDialog();
      return result;
    }
  }

  public class PngPropertySelector : IMapImageFileExporterDialog
  {
    private readonly ExportImageSizeSelector dlg = new ExportImageSizeSelector();

    public double PercentualImageSize
    {
      get
      {
        return dlg.PercentualImageSize;
      }
      set { dlg.PercentualImageSize = value; }
    }

    public SizeCalculatorDelegate SizeCalculator { get; set; }

    public IImageEncodingInfo EncodingInfo
    {
      get { return new PngEncodingInfo(); }
    }

    public DialogResult ShowPropertyDialog()
    {
      dlg.SizeCalculator = SizeCalculator;
      DialogResult result = dlg.ShowDialog();
      return result;
    }
  }

  public class TiffPropertySelector : IMapImageFileExporterDialog
  {
    private readonly ExportImageSizeSelector dlg = new ExportImageSizeSelector();

    public double PercentualImageSize
    {
      get
      {
        return dlg.PercentualImageSize;
      }
      set { dlg.PercentualImageSize = value; }
    }

    public SizeCalculatorDelegate SizeCalculator { get; set; }

    public IImageEncodingInfo EncodingInfo
    {
      get { return new TiffEncodingInfo(); }
    }


    public DialogResult ShowPropertyDialog()
    {
      dlg.SizeCalculator = SizeCalculator;
      DialogResult result = dlg.ShowDialog();
      return result;
    }
  }

  public interface IMapImageFileExporterDialog : IMapImageFileExporter
  {
    DialogResult ShowPropertyDialog();
  }

  public interface IMapImageFileExporter
  {
    IImageEncodingInfo EncodingInfo { get; }
    double PercentualImageSize { get; set; }
  }
}