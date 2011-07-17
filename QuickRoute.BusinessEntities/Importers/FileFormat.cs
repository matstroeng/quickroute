using System.Collections.Generic;

namespace QuickRoute.BusinessEntities.Importers
{
  public class FileFormat
  {
    private string fileFilter;

    public FileFormat(string fileFilter)
    {
      this.fileFilter = fileFilter;
    }

    public string FileFilter
    {
      get { return fileFilter; }
      set { fileFilter = value; }
    }

    public string[] Extensions
    {
      get
      {
        int pos = fileFilter.LastIndexOf("|");
        string[] extensionsArray = fileFilter.Substring(pos + 1).Split(";".ToCharArray());
        List<string> extensions = new List<string>();
        foreach (string e in extensionsArray)
        {
          extensions.Add(e.TrimStart("*".ToCharArray()));
        }
        return extensions.ToArray();
      }
    }

    public override string ToString()
    {
      int pos = fileFilter.LastIndexOf("|");
      return fileFilter.Substring(0, pos);
    }

  }
}