using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuickRoute.Tools.ResourceTranslationTool
{
  class Program
  {
    static void Main(string[] args)
    {
      var solutionPath = args[0];
      var outputPath = args[1];

      var solution = new Solution(solutionPath);
      var languages = solution.GetLanguages();

      foreach(var language in languages)
      {
        var sb = new StringBuilder();
        sb.AppendLine("Created at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".");
        sb.AppendLine();

        foreach(var file in solution.GetLocalizedResourceFiles(language))
        {
          var fileNameWithoutSolutionPath = file.FileName.Replace(solutionPath, "");
          var untranslatedResources = file.GetUntranslatedResourceKeys();
          if(untranslatedResources.Count() > 0)
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
  }
}
