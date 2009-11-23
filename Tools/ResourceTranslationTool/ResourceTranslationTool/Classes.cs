using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace QuickRoute.Tools.ResourceTranslationTool
{
  public class Solution
  {
    public string Path { get; private set; }
    public Solution(string path)
    {
      Path = path;
    }

    public IEnumerable<LocalizedResourceFile> GetLocalizedResourceFiles(string language)
    {
      return Directory.GetFiles(Path, "*.resx", SearchOption.AllDirectories)
        .Where(LocalizedResourceFile.IsBaseResourceFile)
        .Select(o => new LocalizedResourceFile(LocalizedResourceFile.GetLocalizedFileNameFromBaseFileName(o, language)));
    }

    public IEnumerable<string> GetLanguages()
    {
      var settingsFile = System.IO.Path.Combine(Path, "QuickRoute.UI\\app.config");
      var doc = new XmlDocument();
      doc.Load(settingsFile);
      var node = doc.SelectSingleNode("/configuration/appSettings/add[@key='cultures']");
      return node.Attributes["value"].Value.Split(';').Except(new[] { "en" });
    }

  }

  public class Project
  {
    public string ProjectFileName { get; private set; }
    
    public Project(string fileName)
    {
      ProjectFileName = fileName;
    }
    
    public void AddResourceFile(string fileName)
    {
      var relativeFileName = fileName.Replace(Path.GetDirectoryName(ProjectFileName), "");
      if (relativeFileName.StartsWith("\\")) relativeFileName = relativeFileName.Substring(1);
      var document = XDocument.Load(ProjectFileName);
      XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

      var resourceFiles = GetResourceFiles(ProjectFileName);

      if (!resourceFiles.Contains(fileName))
      {
        var resourceFileItemGroup = new XElement(ns + "ItemGroup");
        var embeddedResource = new XElement(ns + "EmbeddedResource");
        embeddedResource.SetAttributeValue("Include", relativeFileName);
        resourceFileItemGroup.Add(embeddedResource);
        document.Descendants(ns + "ItemGroup").Last().AddAfterSelf(resourceFileItemGroup);
        document.Save(ProjectFileName);
      }
    }

    private static List<string> GetResourceFiles(string projectFileName)
    {
      var document = XDocument.Load(projectFileName);
      XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

      var resourceFiles = from node in document.Descendants(ns + "ItemGroup").Descendants(ns + "EmbeddedResource")
                          where node.Attribute("Include") != null && node.Attribute("Include").Value.EndsWith(".resx")
                          select Path.Combine(Path.GetDirectoryName(projectFileName), node.Attribute("Include").Value);
      return resourceFiles.ToList();
    }

    public static string GetProjectFile(string fileName)
    {
      var path = Path.GetDirectoryName(fileName);
      if(string.IsNullOrEmpty(path)) return null;
      var projectFiles = Directory.GetFiles(path, "*.csproj");
      if (projectFiles.Length > 0)
      {
        return projectFiles[0];
      }
      return GetProjectFile(path);
    }
  }



  public class LocalizedResourceFile
  {
    public LocalizedResourceFile(string fileName)
    {
      FileName = fileName;
    }

    public string FileName { get; private set; }

    public string Language
    {
      get
      {
        var fileNameWithoutDirectory = Path.GetFileName(FileName);
        var atoms = fileNameWithoutDirectory.Split('.');
        if (atoms.Length < 3) return null;
        return atoms[atoms.Length - 2];
      }
    }

    public string BaseFileName
    {
      get
      {
        return FileName.Replace("." + Language + ".", ".");
      }
    }

    public static bool IsLocalizedResourceFile(string path)
    {
      var fileNameWithoutDirectory = Path.GetFileName(path);
      var atoms = fileNameWithoutDirectory.Split('.');
      if (atoms.Length < 3) return false;
      return atoms[atoms.Length - 1].ToLowerInvariant() == "resx";
    }

    public static bool IsBaseResourceFile(string path)
    {
      var fileNameWithoutDirectory = Path.GetFileName(path);
      var atoms = fileNameWithoutDirectory.Split('.');
      if (atoms.Length != 2) return false;
      return atoms[atoms.Length - 1].ToLowerInvariant() == "resx";
    }

    public static string GetLocalizedFileNameFromBaseFileName(string path, string language)
    {
      if(!IsBaseResourceFile(path)) return null;
      return path.Replace(".resx", string.Format(".{0}.resx", language));
    }


    private IEnumerable<Resource> GetBaseResourcesWithComments()
    {
      var set = new HashSet<Resource>();
      if (File.Exists(BaseFileName))
      {
        var doc = new XmlDocument();
        doc.Load(BaseFileName);
        var nodes = doc.SelectNodes("/root/data/comment");
        if (nodes != null)
        {
          foreach (XmlNode node in nodes)
          {
            set.Add(new Resource(node.ParentNode.Attributes["name"].Value.Trim(),
                                 node.ParentNode.SelectSingleNode("value").InnerText.Trim(),
                                 node.InnerText.Trim()));
          }
        }
      }
      return set;
    }

    private IEnumerable<Resource> GetResources()
    {
      var set = new HashSet<Resource>();
      if (File.Exists(FileName))
      {
        var doc = new XmlDocument();
        doc.Load(FileName);
        var nodes = doc.SelectNodes("/root/data");
        if (nodes != null)
        {
          foreach (XmlNode node in nodes)
          {
            set.Add(new Resource(node.Attributes["name"].Value.Trim(),
                                 node.SelectSingleNode("value").InnerText.Trim(),
                                 null));
          }
        }
      }
      return set;
    }

    public IEnumerable<Resource> GetUntranslatedResourceKeys()
    {
      var result = new HashSet<Resource>();
      var baseResourcesWithComments = GetBaseResourcesWithComments();
      var localizedResources = GetResources();

      foreach(var baseResource in baseResourcesWithComments)
      {
        if(!localizedResources.Any(o=>o.Key==baseResource.Key))
        {
          result.Add(baseResource);
        }
      }
      return result;
    }
  }

  public class Resource
  {
    public Resource(string key, string value, string comment)
    {
      Key = key;
      Value = value;
      Comment = comment;
    }

    public string Key { get; set; }
    public string Value { get; set; }
    public string Comment { get; set; }
  }

  public static class SvnManager
  {
    public static void AddFile(string path)
    {
      var svnPath = ConfigurationManager.AppSettings["SubversionPath"];
      Process.Start(svnPath, string.Format(@"add ""{0}""", path));
    }
  }

}
