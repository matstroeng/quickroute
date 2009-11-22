using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Ionic.Zip;

namespace QuickRoute.Tools.ResourceTranslationTool
{
  class Program
  {
    static void Main(string[] args)
    {
      var action = args[0];

      if (action == "-upload") Upload(args);
      if (action == "-download") Download(args);
    }

    private static void Upload(string[] args)
    {
      var solutionPath = args[1];
      var outputPath = args[2];

      var solution = new Solution(solutionPath);
      var languages = solution.GetLanguages();

      foreach (var language in languages)
      {
        var sb = new StringBuilder();
        sb.AppendLine("Created at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".");
        sb.AppendLine();

        foreach (var file in solution.GetLocalizedResourceFiles(language))
        {
          var fileNameWithoutSolutionPath = file.FileName.Replace(solutionPath, "");
          var untranslatedResources = file.GetUntranslatedResourceKeys();
          if (untranslatedResources.Count() > 0)
          {
            Console.WriteLine("Processing {0} ({1} untranslated resources)...", fileNameWithoutSolutionPath, untranslatedResources.Count());
            sb.AppendLine("=========================================================================");
            sb.AppendLine(fileNameWithoutSolutionPath);
            sb.AppendLine("=========================================================================");
            untranslatedResources.ToList().ForEach(o => sb.AppendLine(fileNameWithoutSolutionPath + " > " + o.Key));
            sb.AppendLine("=========================================================================");
            sb.AppendLine("");
          }
        }
        File.WriteAllText(Path.Combine(outputPath, language + ".txt"), sb.ToString());
      }
    }

    private static void Download(string[] args)
    {
      var solutionPath = args[1];
      var serverUrl = args[2];

      // get new files
      var client = new WebClient();
      var newFiles = client.DownloadString(serverUrl + "?action=getNewUploads");

      if(!string.IsNullOrEmpty(newFiles))
      {
        foreach (var data in newFiles.Split('\n'))
        {
          var atoms = data.Split(';');
          var id = atoms[0];
          var language = atoms[1];
          var extension = "." + language + ".resx";
          var zipFileData = client.DownloadData(serverUrl + "?action=downloadZipFile&id=" + id);
          var zipFile = ZipFile.Read(zipFileData);
          foreach (var file in zipFile)
          {
            // only extract files with the .[language].resx extension
            if (file.FileName.EndsWith(extension))
            {
              Console.WriteLine("Extracting {0}...", file.FileName);
              file.Extract(solutionPath, true);
              // make sure to add the file to the subversion repository
              SvnManager.AddFile(Path.Combine(solutionPath, file.FileName));
            }
          }
        }
      }
    }
  
  }
}
