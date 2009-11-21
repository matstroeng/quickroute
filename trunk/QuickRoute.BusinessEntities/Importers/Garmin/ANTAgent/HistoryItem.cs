using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.Importers.Garmin.ANTAgent
{
  public class HistoryItem
  {
    private string displayName;
    private string id;
    private string fileName;

    public HistoryItem(string displayName, string id, string fileName)
    {
      this.displayName = displayName;
      this.id = id;
      this.fileName = fileName;
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

    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }

    public override string ToString()
    {
      return displayName + ": " + DateTime.Parse(id).ToString("yyyy-MM-dd HH:mm:ss");
    }

  }
}
