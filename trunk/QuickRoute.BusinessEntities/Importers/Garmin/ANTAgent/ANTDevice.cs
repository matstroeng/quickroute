using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using QuickRoute.Common;

namespace QuickRoute.BusinessEntities.Importers.Garmin.ANTAgent
{
  public class ANTDevice
  {
    /// <summary>
    /// Initializes the device from data in the specified path.
    /// </summary>
    /// <param name="path">The path where the data resides. Shall end with a backslash.</param>
    public ANTDevice(string path)
    {
      // id and display name
      XmlTextReader reader = new XmlTextReader(path + "GarminDevice.XML");
      XPathDocument doc = new XPathDocument(reader);
      XPathNavigator nav = doc.CreateNavigator();
      XmlNamespaceManager nsManager = new XmlNamespaceManager(nav.NameTable);
      nsManager.AddNamespace("ns", "http://www.garmin.com/xmlschemas/GarminDevice/v2");

      XPathNavigator idElement = nav.SelectSingleNode("/ns:Device/ns:Id", nsManager);
      Id = idElement.Value;

      XPathNavigator displayNameElement = nav.SelectSingleNode("/ns:Device/ns:DisplayName", nsManager);
      DisplayName = displayNameElement.Value;

      reader.Close();

      // history items
      HistoryItems = new List<HistoryItem>();
      DirectoryInfo di = new DirectoryInfo(path + "History\\");

      var cache = ReadCache();
      foreach (FileInfo fi in di.GetFiles("*.TCX"))
      {
        var key = GetCacheKey(fi);
        if (!cache.ContainsKey(key))
        {
          var itemsInFile = ExtractHistoryItemsFromFile(fi);
          cache.Add(key, itemsInFile);
        }
        HistoryItems.AddRange(cache[key]);
      }
      SaveCache(cache);
    }
    
    public string DisplayName { get; set; }

    public string Id { get; set; }

    public List<HistoryItem> HistoryItems { get; set; }

    private IEnumerable<HistoryItem> ExtractHistoryItemsFromFile(FileInfo fi)
    {
      var items = new List<HistoryItem>();

      XPathNodeIterator historyItemIds;
      XmlNamespaceManager nsManager;
      XmlTextReader reader;
      XPathDocument doc;
      XPathNavigator nav;
      reader = new XmlTextReader(fi.FullName);
      doc = new XPathDocument(reader);
      nav = doc.CreateNavigator();
      nsManager = new XmlNamespaceManager(nav.NameTable);
      nsManager.AddNamespace("ns", "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");

      historyItemIds = nav.Select("//ns:Activity/ns:Id", nsManager);

      while (historyItemIds.MoveNext())
      {
        XPathNavigator historyItemId = historyItemIds.Current;
        if (historyItemId != null) items.Add(new HistoryItem(DisplayName, historyItemId.Value, fi));
      }
      reader.Close();
      return items;
    }

    private IDictionary<string, IEnumerable<HistoryItem>> ReadCache()
    {
      try
      {
        return CommonUtil.DeserializeFromFile<IDictionary<string, IEnumerable<HistoryItem>>>(CacheFileName);
      }
      catch (Exception)
      {
        return new Dictionary<string, IEnumerable<HistoryItem>>();
      }
    }

    private void SaveCache(IDictionary<string, IEnumerable<HistoryItem>> cache)
    {
      try
      {
        CommonUtil.SerializeToFile(cache, CacheFileName);
      }
      catch (Exception)
      {
      }
    }

    private string CacheFileName
    {
      get
      {
        return Path.Combine(CommonUtil.GetApplicationDataPath(), "ANTDeviceCache.bin");
      }
    }

    private string GetCacheKey(FileInfo fi)
    {
      return fi.FullName + " " + fi.LastWriteTimeUtc.Ticks.ToString();
    }
  }

}