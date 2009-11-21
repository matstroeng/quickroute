using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace QuickRoute.BusinessEntities.Importers.GPX
{
  public static class GPXUtil
  {
    public static GPXVersion GetGPXVersion(string fileName)
    {
      if (IsGPX10(fileName)) return GPXVersion.GPX10;
      if (IsGPX11(fileName)) return GPXVersion.GPX11;
      return GPXVersion.Unknown;
    }

    private static bool IsGPX10(string fileName)
    {
      var reader = new XmlTextReader(fileName);
      var doc = new XPathDocument(reader);
      var nav = doc.CreateNavigator();
      var nsManager = new XmlNamespaceManager(nav.NameTable);
      nsManager.AddNamespace("gpx10", "http://www.topografix.com/GPX/1/0");

      XPathNavigator gpxElement = nav.SelectSingleNode("/gpx10:gpx", nsManager);

      return (gpxElement != null);
    }

    private static bool IsGPX11(string fileName)
    {
      var reader = new XmlTextReader(fileName);
      var doc = new XPathDocument(reader);
      var nav = doc.CreateNavigator();
      var nsManager = new XmlNamespaceManager(nav.NameTable);
      nsManager.AddNamespace("gpx11", "http://www.topografix.com/GPX/1/1");

      XPathNavigator gpxElement = nav.SelectSingleNode("/gpx11:gpx", nsManager);

      return (gpxElement != null);
    }

    public static bool ConvertGPX10ToGPX11(string sourceFileName, string targetFileName)
    {
      bool result = false;
      TextReader reader = new StreamReader(sourceFileName);
      TextWriter writer = new StreamWriter(targetFileName);
      string data = reader.ReadToEnd();
      if (data.Contains("http://www.topografix.com/GPX/1/0"))
      {
        writer.Write(data.Replace("http://www.topografix.com/GPX/1/0", "http://www.topografix.com/GPX/1/1"));
        result = true;
      }
      reader.Close();
      writer.Close();
      return result;
    }

  }

  public enum GPXVersion
  {
    GPX10,
    GPX11,
    Unknown
  }

}
