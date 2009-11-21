using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace QuickRoute.BusinessEntities.Importers.Garmin.ANTAgent
{
  public class ANTDevice
  {
    private string id;
    private string displayName;
    private List<HistoryItem> historyItems = new List<HistoryItem>();

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
      id = idElement.Value;

      XPathNavigator displayNameElement = nav.SelectSingleNode("/ns:Device/ns:DisplayName", nsManager);
      displayName = displayNameElement.Value;

      reader.Close();

      // history items
      historyItems = new List<HistoryItem>();
      DirectoryInfo di = new DirectoryInfo(path + "History\\");

      foreach (FileInfo fi in di.GetFiles("*.TCX"))
      {
        reader = new XmlTextReader(fi.FullName);
        doc = new XPathDocument(reader);
        nav = doc.CreateNavigator();
        nsManager = new XmlNamespaceManager(nav.NameTable);
        nsManager.AddNamespace("ns", "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2");

        XPathNavigator historyItemId = nav.SelectSingleNode("/ns:TrainingCenterDatabase/ns:Activities/ns:Activity/ns:Id", nsManager);

        historyItems.Add(new HistoryItem(displayName, historyItemId.Value, fi.FullName));

        reader.Close();
      }
    }


    public string DisplayName
    {
      get { return displayName; }
      set { displayName = value; }
    }

    public string Id
    {
      get { return id; }
      set { id = value; }
    }

    public List<HistoryItem> HistoryItems
    {
      get { return historyItems; }
      set { historyItems = value; }
    }
  }
}