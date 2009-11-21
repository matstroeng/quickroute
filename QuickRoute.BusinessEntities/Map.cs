using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.Serialization;
using System.Net;
using System.IO;
using System.Drawing;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class Map : ISerializable 
  {
    private MapStorageType storageType;
    private MapSourceType sourceType;
    private Bitmap image;
    private string source;
    [NonSerialized] private byte[] rawData;

    public Map(string source, MapSourceType sourceType, MapStorageType storageType)
    {
      this.source = source;
      this.sourceType = sourceType;
      this.storageType = storageType;
      this.image = GetImage(source, sourceType);
    }

    private Bitmap GetImage(string source, MapSourceType sourceType)
    {
      Stream stream = null;
      Bitmap targetImage = null;
      switch (sourceType)
      {
        case MapSourceType.FileSystem:
          stream = new FileStream(source, FileMode.Open);
          break;

        case MapSourceType.Url:
          stream = GetImageStreamFromUrl(source);
          break;
      }
      
      if(stream != null)
      {
        // check if the image is an exported QuickRoute image; if yes, remove the border
        var ed = QuickRouteJpegExtensionData.FromStream(stream);
        var sourceImage = System.Drawing.Image.FromStream(stream);
        rawData = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(rawData, 0, (int)stream.Length);
        if (ed != null)
        {
          targetImage = new Bitmap(ed.MapLocationAndSizeInPixels.Width, ed.MapLocationAndSizeInPixels.Height);
          var g = Graphics.FromImage(targetImage);
          g.DrawImage(sourceImage, new Rectangle(new Point(0, 0), ed.MapLocationAndSizeInPixels.Size), ed.MapLocationAndSizeInPixels, GraphicsUnit.Pixel);
          g.Dispose();
        }
        else
        {
          targetImage = new Bitmap(sourceImage);
        }
        stream.Close();
        stream.Dispose();
      }
      return targetImage;
    }

    public Map(Bitmap image)
    {
      this.source = null;
      this.sourceType = MapSourceType.FileSystem;
      this.storageType = MapStorageType.Inline;
      this.image = image;
      this.rawData = null;
    }

    public Map(Stream stream)
    {
      this.source = null;
      this.sourceType = MapSourceType.FileSystem;
      this.storageType = MapStorageType.Inline;
      stream.Position = 0;
      this.rawData = new byte[stream.Length];
      stream.Read(this.rawData, 0, (int) stream.Length);
      this.image = (Bitmap)System.Drawing.Image.FromStream(stream);
    }

    protected Map(SerializationInfo info, StreamingContext context)
    {
      source = info.GetString("source");
      sourceType = (MapSourceType)(info.GetValue("sourceType", typeof(MapSourceType)));
      storageType = (MapStorageType)(info.GetValue("storageType", typeof(MapStorageType)));
      switch (storageType)
      {
        case MapStorageType.Inline:
          //rawData = (byte[])(info.GetValueNo("rawData", typeof(byte[])));
          var getValueNoThrowMethod = info.GetType().GetMethod("GetValueNoThrow", BindingFlags.Instance | BindingFlags.NonPublic);
          rawData = (byte[])getValueNoThrowMethod.Invoke(info, new object[] { "rawData", typeof(byte[]) });


          if (rawData != null)
          {
            // version 2.3 file format contains rawData field, create image from it
            using (var ms = new MemoryStream(rawData))
            {
              image = (Bitmap)System.Drawing.Image.FromStream(ms);
            }
          }
          else
          {
            // version 2.2 file format
            image = (Bitmap)(info.GetValue("image", typeof(Bitmap)));
          }
          break;

        case MapStorageType.Reference:
          switch (sourceType)
          {
            case MapSourceType.FileSystem:
              image = (Bitmap)System.Drawing.Image.FromFile(source);
              break;
            case MapSourceType.Url:
              image = GetImageFromUrl(source);
              break;
          }
          break;
      }
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("source", source);
      info.AddValue("sourceType", sourceType, typeof(MapSourceType));
      info.AddValue("storageType", storageType, typeof(MapStorageType));
      if(storageType == MapStorageType.Inline)
      {
        // saving bitmap object in version 2.2 and earlier
        if (rawData == null)
        {
          info.AddValue("image", image);
        }
        else
        {
          // from version 2.3, save byte array instead to decrease file size when map image is in jpeg format
          // this breaks backward compatibility, but the advantage of a small file size is more important in my opinion
          info.AddValue("rawData", rawData);
        }
      }
    }

    public MapStorageType StorageType
    {
      get { return storageType; }
      set { storageType = value; }
    }

    public MapSourceType SourceType
    {
      get { return sourceType; }
    }

    public Bitmap Image
    {
      get { return image; }
    }

    public string Source
    {
      get { return source; }
    }

    /// <summary>
    /// Rotates the map image
    /// </summary>
    /// <param name="angle">Rotation angle, radians counterclockwise</param>
    public void RotateImage(double angle)
    {
      Graphics g = Graphics.FromImage(image);
      Matrix t = new Matrix();
      t.RotateAt((float)(angle * Math.PI / 180), new PointF((float)image.Width / 2, (float)image.Height / 2));
      g.Transform = t;
      g.DrawImage(image, 0, 0); 
    }

    private static Bitmap GetImageFromUrl(string url)
    {
      var request = WebRequest.Create(url);
      var response = (HttpWebResponse)request.GetResponse();
      var dataStream = response.GetResponseStream();
      var imageFromUrl = (Bitmap)System.Drawing.Image.FromStream(dataStream);
      dataStream.Close();
      response.Close();
      return imageFromUrl;
    }

    private static Stream GetImageStreamFromUrl(string url)
    {
      var request = WebRequest.Create(url);
      var response = (HttpWebResponse)request.GetResponse();
      return response.GetResponseStream();
    }

  }

  /// <summary>
  /// The type of storage of the map image when the map object is serialized.
  /// </summary>
  public enum MapStorageType
  {
    /// <summary>
    /// The map image is stored as a byte array in the object itself
    /// </summary>
    Inline,
    /// <summary>
    /// The map image is stored as a reference to a file or url
    /// </summary>
    Reference
  }

  /// <summary>
  /// The location type of the map image file.
  /// </summary>
  public enum MapSourceType
  {
    /// <summary>
    /// The source is a file in the file system
    /// </summary>
    FileSystem,
    /// <summary>
    /// The source is a file on an internet url 
    /// </summary>
    Url
  }
}
