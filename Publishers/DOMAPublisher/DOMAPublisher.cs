using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using QuickRoute.Common;
using QuickRoute.Publishers.DOMAPublisher.DOMA;
using Category = QuickRoute.Common.Category;
using MapInfo = QuickRoute.Common.MapInfo;

namespace QuickRoute.Publishers.DOMAPublisher
{
  public class DOMAPublisher : IMapPublisher
  {
    public string WebServiceUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    private string version;

    #region Interface methods
    public PublishResult Publish(MapInfo mapInfo)
    {
      if (version != null && version.CompareTo("3.0") >= 0)
      {
        return PublishWithPreUpload(mapInfo);
      }
      return PublishWithoutPreUpload(mapInfo);
    }

    public GetAllCategoriesResult GetAllCategories()
    {
      var service = new DOMAService();
      service.Url = WebServiceUrl;

      var request = new GetAllCategoriesRequest
      {
        Username = Username,
        Password = Password
      };

      var response = service.GetAllCategories(request);
      var result = new GetAllCategoriesResult
      {
        ErrorMessage = response.ErrorMessage,
        Success = response.Success,
      };
      if (response.Success)
      {
        result.Categories = new List<Category>();
        foreach (var category in response.Categories)
        {
          result.Categories.Add(TranslateCategory(category));
        }
      }
      return result;
    }

    public GetAllMapsResult GetAllMaps()
    {
      var service = new DOMAService();
      service.Url = WebServiceUrl;

      var request = new GetAllMapsRequest
      {
        Username = Username,
        Password = Password
      };

      var response = service.GetAllMaps(request);
      var result = new GetAllMapsResult
                     {
                       ErrorMessage = response.ErrorMessage,
                       Success = response.Success,
                     };
      if (response.Success)
      {
        result.Maps = new List<MapInfo>();
        foreach (var map in response.Maps)
        {
          result.Maps.Add(TranslateMapInfo(map));
        }
      }
      return result;
    }

    public ConnectResult Connect()
    {
      var service = new DOMAService();
      service.Url = WebServiceUrl;

      var request = new ConnectRequest
                      {
                        Username = Username,
                        Password = Password
                      };
      var response = service.Connect(request);

      version = response.Version;

      return new ConnectResult
      {
        Success = response.Success,
        ErrorMessage = response.ErrorMessage,
        Version = response.Version
      };
    }
    #endregion

    private PublishResult PublishWithoutPreUpload(MapInfo mapInfo)
    {
      var service = new DOMAService();
      service.Url = WebServiceUrl;

      var request = new PublishMapRequest
      {
        Username = Username,
        Password = Password,
        MapInfo = TranslateMapInfo(mapInfo),
      };

      try
      {
        request.MapInfo.BlankMapImageData = null; // this version of the webservice has not support for blank map images
        var response = service.PublishMap(request);
        return new PublishResult
        {
          Success = response.Success,
          ErrorMessage = response.ErrorMessage,
          URL = response.URL
        };
      }
      catch (Exception ex)
      {
        return new PublishResult
        {
          Success = false,
          ErrorMessage = ex.Message
        };
      }
    }

    private PublishResult PublishWithPreUpload(MapInfo mapInfo)
    {
      var service = new DOMAService();
      service.Url = WebServiceUrl;

      var request = new PublishPreUploadedMapRequest
      {
        Username = Username,
        Password = Password,
        MapInfo = TranslateMapInfo(mapInfo),
      };

      try
      {
        // use partial upload
        // map image
        FileUploadResult mapFileUploadResult = null;
        if (request.MapInfo.MapImageData != null) mapFileUploadResult = PartialFileUpload(request.MapInfo.MapImageData, request.MapInfo.MapImageFileExtension);
        // blank map image
        FileUploadResult blankMapFileUploadResult = null;
        if (request.MapInfo.BlankMapImageData != null) blankMapFileUploadResult = PartialFileUpload(request.MapInfo.BlankMapImageData, request.MapInfo.MapImageFileExtension);
        // thumbnail
        FileUploadResult thumbnailFileUploadResult = null;
        var thumbnailImageData = CreateThumbnailImageData(mapInfo);
        if (thumbnailImageData != null) thumbnailFileUploadResult = PartialFileUpload(thumbnailImageData, "jpg");

        if (!(mapFileUploadResult != null && !mapFileUploadResult.Success) && !(blankMapFileUploadResult != null && !blankMapFileUploadResult.Success) && !(thumbnailFileUploadResult != null && !thumbnailFileUploadResult.Success))
        {
          if (mapFileUploadResult != null) request.PreUploadedMapImageFileName = mapFileUploadResult.FileName;
          if (blankMapFileUploadResult != null) request.PreUploadedBlankMapImageFileName = blankMapFileUploadResult.FileName;
          if (thumbnailFileUploadResult != null) request.PreUploadedThumbnailImageFileName = thumbnailFileUploadResult.FileName;
        }

        // reset image data as it already has been uploaded
        request.MapInfo.MapImageData = null;
        request.MapInfo.BlankMapImageData = null;
        var response = service.PublishPreUploadedMap(request);
        return new PublishResult
        {
          Success = response.Success,
          ErrorMessage = response.ErrorMessage,
          URL = response.URL
        };
      }
      catch (Exception ex)
      {
        return new PublishResult
        {
          Success = false,
          ErrorMessage = ex.Message
        };
      }
    }

    private FileUploadResult PartialFileUpload(byte[] imageData, string extension)
    {
      const int chunkSize = 512 * 1024; // 512 KB
      var service = new DOMAService { Url = WebServiceUrl };

      string fileName = new Random().Next(0, 100000000) + "." + extension;
      int position = 0;

      while (position < imageData.Length)
      {
        int length = Math.Min(chunkSize, imageData.Length - position);
        var buffer = new byte[length];
        Array.Copy(imageData, position, buffer, 0, length);
        position += length;
        var uploadPartialFileRequest = new UploadPartialFileRequest
        {
          Username = Username,
          Password = Password,
          FileName = fileName,
          Data = buffer
        };
        var uploadPartialFileResponse = service.UploadPartialFile(uploadPartialFileRequest);
        if (!uploadPartialFileResponse.Success)
        {
          return new FileUploadResult
          {
            ErrorMessage = uploadPartialFileResponse.ErrorMessage,
            Success = false
          };
        }
      }
      return new FileUploadResult
      {
        Success = true,
        FileName = fileName
      };
    }

    private static byte[] CreateThumbnailImageData(MapInfo mapInfo) //, ThumbnailProperties tp)
    {
      if (mapInfo.MapImageData == null && mapInfo.BlankMapImageData == null) return null;
      // get original image from byte array
      var ms = new MemoryStream(mapInfo.MapImageData ?? mapInfo.BlankMapImageData);
      var image = Image.FromStream(ms);
      ms.Close();
      ms.Dispose();

      // create blank thumbnail image
      // todo: get these values from webservice
      var thumbnailSize = new Size(400, 100);
      var thumbnailScale = 0.5;
      var thumbnailBitmap = new Bitmap(thumbnailSize.Width, thumbnailSize.Height);
      // the rectangle in the original image that corresponds to the thumbnail image
      var imageRectangle = new Rectangle((image.Width - thumbnailSize.Width) / 2, (image.Height - thumbnailSize.Height) / 2,
                                         Convert.ToInt32(thumbnailSize.Width / thumbnailScale), Convert.ToInt32(thumbnailSize.Height / thumbnailScale));
      // perform some resizing if the image is not large enough
      if (imageRectangle.Width > image.Width)
      {
        imageRectangle.X = 0;
        imageRectangle.Width = image.Width;
      }
      if (imageRectangle.Height > image.Height)
      {
        imageRectangle.Y = 0;
        imageRectangle.Height = image.Height;
      }

      // calculate actual thumbnail rectangle and draw the image to the thumbnail
      var thumbnailRectangle = new Rectangle(
        Convert.ToInt32((thumbnailSize.Width - thumbnailScale * imageRectangle.Width) / 2),
        Convert.ToInt32((thumbnailSize.Height - thumbnailScale * imageRectangle.Height) / 2),
        Convert.ToInt32(thumbnailScale * imageRectangle.Width),
        Convert.ToInt32(thumbnailScale * imageRectangle.Height));
      var thumbnailGraphics = Graphics.FromImage(thumbnailBitmap);
      thumbnailGraphics.SmoothingMode = SmoothingMode.AntiAlias;
      thumbnailGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      using (var b = new SolidBrush(Color.White))
      {
        thumbnailGraphics.FillRectangle(b, new Rectangle(new Point(0, 0), thumbnailSize));
      }
      thumbnailGraphics.DrawImage(image, thumbnailRectangle, imageRectangle, GraphicsUnit.Pixel);
      thumbnailGraphics.Dispose();
      image.Dispose();

      // create byte array from image
      var thumbnailStream = new MemoryStream();
      thumbnailBitmap.Save(thumbnailStream, GetJpegEncoder(), GetJpegEncoderParams(0.8));
      thumbnailBitmap.Dispose();
      var data = thumbnailStream.ToArray();
      thumbnailStream.Dispose();

      return data;
    }

    private static QuickRoute.Common.MapInfo TranslateMapInfo(DOMA.MapInfo mapInfo)
    {
      return new QuickRoute.Common.MapInfo
               {
                 Comment = mapInfo.Comment,
                 Country = mapInfo.Country,
                 Date = mapInfo.Date,
                 ID = mapInfo.ID,
                 UserID = mapInfo.UserID,
                 CategoryID = mapInfo.CategoryID,
                 MapImageData = mapInfo.MapImageData,
                 BlankMapImageData = mapInfo.BlankMapImageData,
                 MapImageFileExtension = mapInfo.MapImageFileExtension,
                 MapName = mapInfo.MapName,
                 Name = mapInfo.Name,
                 Organiser = mapInfo.Organiser,
                 RelayLeg = mapInfo.RelayLeg,
                 ResultListUrl = mapInfo.ResultListUrl,
                 Discipline = mapInfo.Discipline
               };
    }

    private static DOMA.MapInfo TranslateMapInfo(QuickRoute.Common.MapInfo mapInfo)
    {
      return new DOMA.MapInfo
      {
        Comment = mapInfo.Comment,
        Country = mapInfo.Country,
        Date = mapInfo.Date,
        ID = mapInfo.ID,
        UserID = mapInfo.UserID,
        CategoryID = mapInfo.CategoryID,
        MapImageData = mapInfo.MapImageData,
        BlankMapImageData = mapInfo.BlankMapImageData,
        MapImageFileExtension = mapInfo.MapImageFileExtension,
        MapName = mapInfo.MapName,
        Name = mapInfo.Name,
        Organiser = mapInfo.Organiser,
        RelayLeg = mapInfo.RelayLeg,
        ResultListUrl = mapInfo.ResultListUrl,
        Discipline = mapInfo.Discipline
      };
    }

    private static QuickRoute.Common.Category TranslateCategory(DOMA.Category Category)
    {
      return new QuickRoute.Common.Category
      {
        ID = Category.ID,
        UserID = Category.UserID,
        Name = Category.Name,
      };
    }

    private static DOMA.Category TranslateCategory(QuickRoute.Common.Category Category)
    {
      return new DOMA.Category
      {
        ID = Category.ID,
        UserID = Category.UserID,
        Name = Category.Name,
      };
    }

    private static ImageCodecInfo GetJpegEncoder()
    {
      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
      foreach (ImageCodecInfo codec in codecs)
      {
        if (codec.MimeType == "image/jpeg") return codec;
      }
      return null;
    }

    private static EncoderParameters GetJpegEncoderParams(double quality)
    {
      var ep = new EncoderParameters(1);
      ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)(100 * quality));
      return ep;
    }

    private class FileUploadResult
    {
      public bool Success { get; set; }
      public string ErrorMessage { get; set; }
      public string FileName { get; set; }
    }

  }
}
