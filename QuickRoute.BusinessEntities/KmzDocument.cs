using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using QuickRoute.Common;

namespace QuickRoute.BusinessEntities
{
  public class KmzDocument
  {
    public KmzDocument(string fileName)
    {
      CalculateImageAndTransformationMatrix(fileName);
    }

    public KmzDocument(Stream stream)
    {
      var tempFileName = CommonUtil.GetTempFileName();
      using (var fs = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
      {
        stream.Position = 0;
        CommonUtil.CopyStream(stream, fs);
      }
      CalculateImageAndTransformationMatrix(tempFileName);
      File.Delete(tempFileName);
    }

    public Stream ImageStream { get; private set; }

    public Transformation Transformation { get; private set; }

    private void CalculateImageAndTransformationMatrix(string fileName)
    {
      using (var zipFile = new Ionic.Zip.ZipFile(fileName))
      {
        var mapSize = new Size();
        Transformation = new Transformation();

        // get entry for kml file and image file
        KmlDocument kmlDocument = null;
        foreach (var entry in zipFile)
        {
          if (entry.FileName == entry.LocalFileName && Path.GetExtension(entry.FileName) == ".kml")
          {
            using (var kmlStream = new MemoryStream())
            {
              entry.Extract(kmlStream);
              kmlStream.Position = 0;
              kmlDocument = new KmlDocument(kmlStream);
            }
            if (kmlDocument.ImageFileName != null)
            {
              break;
            }
          }
        }

        if (kmlDocument != null)
        {
          // we have got a kml document, get map image file stream from it
            foreach (var entry in zipFile)
          {
            if (entry.FileName == kmlDocument.ImageFileName)
            {
              ImageStream = new MemoryStream();
              entry.Extract(ImageStream);
              ImageStream.Position = 0;
              // check if image is QR jpeg
              var ed = QuickRouteJpegExtensionData.FromStream(ImageStream);
              if (ed != null)
              {
                // get transformation matrix from QR jpeg metadata
                Transformation = ed.Sessions.CalculateAverageTransformation();
                ImageStream.Position = 0;
                return;
              }
              else
              {
                // it is not, use normal image bounds
                ImageStream.Position = 0;
                mapSize = Image.FromStream(ImageStream).Size; // need to get image object to get image size
              }
              ImageStream.Position = 0;
              break;
            }
          }
        }

        if (kmlDocument != null && ImageStream != null)
        {
          // finally, calculate the transformation
          Transformation = new Transformation(kmlDocument.LongLatBox, mapSize);
        }
      }
    }

    private class KmlDocument
    {
      public KmlDocument(Stream stream)
      {
        Parse(stream);
      }

      public string ImageFileName { get; private set; }

      public LongLatBox LongLatBox { get; private set; }

      private void Parse(Stream stream)
      {
        var doc = new XmlDocument();
        doc.Load(stream);
        var nsmgr = new XmlNamespaceManager(doc.NameTable);

        var ns = "";
        if(doc.DocumentElement != null && doc.DocumentElement.Attributes["xmlns"] != null)
        {
          nsmgr.AddNamespace("ns", doc.DocumentElement.Attributes["xmlns"].Value);
          ns = "ns:";
        }

        var groundOverlays = doc.SelectNodes(string.Format("//{0}GroundOverlay[1]", ns), nsmgr);
        if (groundOverlays != null && groundOverlays.Count > 0)
        {
          var iconNodes = groundOverlays[0].SelectNodes(string.Format("{0}Icon/{0}href", ns), nsmgr);
          if (iconNodes != null && iconNodes.Count > 0)
          {
            ImageFileName = iconNodes[0].InnerText;
          }

          var latLonBoxNodes = groundOverlays[0].SelectNodes(string.Format("{0}LatLonBox", ns), nsmgr);
          if (latLonBoxNodes != null && latLonBoxNodes.Count > 0)
          {
            LongLatBox = new LongLatBox()
            {
              North =
                Convert.ToDouble(latLonBoxNodes[0].SelectSingleNode(string.Format("{0}north", ns), nsmgr).InnerText,
                                 CultureInfo.InvariantCulture),
              South =
                Convert.ToDouble(latLonBoxNodes[0].SelectSingleNode(string.Format("{0}south", ns), nsmgr).InnerText,
                                 CultureInfo.InvariantCulture),
              West =
                Convert.ToDouble(latLonBoxNodes[0].SelectSingleNode(string.Format("{0}west", ns), nsmgr).InnerText,
                                 CultureInfo.InvariantCulture),
              East =
                Convert.ToDouble(latLonBoxNodes[0].SelectSingleNode(string.Format("{0}east", ns), nsmgr).InnerText,
                                 CultureInfo.InvariantCulture),
              Rotation =
                Convert.ToDouble(latLonBoxNodes[0].SelectSingleNode(string.Format("{0}rotation", ns), nsmgr).InnerText,
                                 CultureInfo.InvariantCulture) / 180.0 * Math.PI
            };
          }
        }
      }
    }

  }
}
