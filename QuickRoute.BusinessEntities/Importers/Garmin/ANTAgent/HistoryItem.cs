using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuickRoute.BusinessEntities.Importers.Garmin.ANTAgent
{
  [Serializable]
  public class HistoryItem
  {
    public HistoryItem(string displayName, string id, FileInfo fileInfo)
    {
      DisplayName = displayName;
      Id = id;
      FileInfo = fileInfo;
    }

    public string DisplayName { get; set; }

    public string Id { get; set; }

    public FileInfo FileInfo { get; set; }

    public override string ToString()
    {
      return DisplayName + ": " + DateTime.Parse(Id).ToString("yyyy-MM-dd HH:mm:ss");
    }

  }
}
