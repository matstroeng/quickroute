using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace QuickRoute.Controls
{
  public class DragDropManager
  {
    public List<string> GetDroppedFileNames(DragEventArgs e, IEnumerable<string> allowedFileExtensions = null)
    {
      var fileNames = new List<string>();
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        fileNames.AddRange((string[]) e.Data.GetData(DataFormats.FileDrop));
        fileNames = FilterFileNamesByExtension(fileNames, allowedFileExtensions);
      }
      if (fileNames.Count == 0 && e.Data.GetDataPresent(DataFormats.StringFormat))
      {
        var fileName = (string)e.Data.GetData(DataFormats.StringFormat);
        if (fileName.StartsWith("http") || File.Exists(fileName)) fileNames.Add(fileName);
      }
      if (fileNames.Count == 0 && e.Data.GetDataPresent("FileName"))
      {
        fileNames.AddRange((string[])e.Data.GetData("FileName"));
      }
      return fileNames;
    }

    private static List<string> FilterFileNamesByExtension(IEnumerable<string> fileNames, IEnumerable<string> allowedFileExtensions)
    {
      if (fileNames == null) return null;
      if (allowedFileExtensions == null) return new List<string>(fileNames);
      var result = new List<string>();
      // find first file with correct extension
      foreach (var fileName in fileNames)
      {
        foreach(var fileExtension in allowedFileExtensions)
        {
          if (fileName.EndsWith(fileExtension, StringComparison.InvariantCultureIgnoreCase))
          {
            result.Add(fileName);
            break;
          }
        }
      }
      return result;
    }
  }
}
