using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Numeric;
using QuickRoute.Common;
using QuickRoute.Resources;
using QuickRoute.UI.Forms;
using QuickRoute.UI.QuickRouteServer;
using ExceptionMessageBox=QuickRoute.UI.Forms.ExceptionMessageBox;

namespace QuickRoute.UI.Classes
{
  public static class Util
  {
    public static ApplicationSettings applicationSettings;

    [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
    public static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

    public static ApplicationSettings ApplicationSettings
    {
      get
      {
        if (applicationSettings == null) applicationSettings = LoadSettings();
        return applicationSettings;
      }
      set
      {
        applicationSettings = value;
      }
    }
    
    public static void ShowHelp()
    {
      System.Diagnostics.Process.Start(Strings.HelpUrl);
    }

    public static string FormatTimeSpan(TimeSpan ts)
    {
      TimeConverter tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
      return tc.ToString(ts.TotalSeconds);
    }

    public static List<Gradient> GetGradientsInFolder(string path)
    {
      List<Gradient> gradients = new List<Gradient>();
      DirectoryInfo di = new DirectoryInfo(path);
      foreach (FileInfo fi in di.GetFiles())
      {
        Gradient g = Gradient.Load(fi.FullName);
        if (!gradients.Contains(g) && g != null) gradients.Add(g);
      }
      return gradients;
    }

    public static void SaveGradientsToFolder(IEnumerable<Gradient> gradients, string path)
    {
      foreach (Gradient g in gradients)
      {
        if (g != null)
        {
          if (g.Name == null) g.Name = "";
          string fileName = (path + "\\").Replace("\\\\", "\\") + CreateSafeFileName(g.Name) + ".gradient";
          fileName = CreateSequentialFileName(fileName);
          g.Save(fileName);
        }
      }
    }

    public static string CreateSafeFileName(string fileName)
    {
      char[] invalidPathChars = Path.GetInvalidPathChars();
      char[] invalidFileChars = Path.GetInvalidFileNameChars();

      foreach (char c in invalidPathChars)
      {
        fileName = fileName.Replace(c, Convert.ToChar("_"));
      }

      foreach (char c in invalidFileChars)
      {
        fileName = fileName.Replace(c, Convert.ToChar("_"));
      }
      return fileName;
    }

    public static string CreateSequentialFileName(string fileName)
    {
      if (File.Exists(fileName))
      {
        string fn = Path.GetFileNameWithoutExtension(fileName);
        string e = Path.GetExtension(fileName);
        string p = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
        int n = 1;
        while (File.Exists(p + fn + "(" + n + ")" + e))
        {
          n += 1;
        }
        return p + fn + "(" + n + ")" + e;
      }
      return fileName;
    }

    public static void SelectUICulture(bool showRestartInformation)
    {
      CultureInfo culture = ApplicationSettings.UiCulture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
      using(var uiCultureForm = new SelectUICultureForm { UiCulture = culture })
      {
        if (uiCultureForm.ShowDialog() != DialogResult.OK) return;
        if (ApplicationSettings.UiCulture != null && uiCultureForm.UiCulture.Name == ApplicationSettings.UiCulture.Name)
          return;
        ApplicationSettings.UiCulture = uiCultureForm.UiCulture;
      }
      if (showRestartInformation)
      {
        MessageBox.Show(Strings.UICultureChangeTakesEffectAtNextRestart, Strings.QuickRoute, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
      }
    }

    public static string PathShortener(string path, int length)
    {
      var sb = new StringBuilder();
      PathCompactPathEx(sb, path, length, 0);
      return sb.ToString();
    }

    public static void InsertIntoRecentDocumentsList(string fileName)
    {
      const int maxNoOfFiles = 10;
      ApplicationSettings.AddRecentDocumentFileName(fileName, maxNoOfFiles);
    }

    public static void UpdateApplicationSettingsToCurrentVersion()
    {
      var s = ApplicationSettings.DefaultDocumentSettings;

      // add some speed waypoint attribute settings, introduced in QR 2.1
      if (!s.ColorRangeIntervalSliderSettings.ContainsKey(WaypointAttribute.Speed))
      {
        var defaultCRISS = DocumentSettings.CreateDefaultColorRangeIntervalSliderSettings();
        s.ColorRangeIntervalSliderSettings.Add(WaypointAttribute.Speed, defaultCRISS[WaypointAttribute.Speed]);
      }

      if (!s.LapHistogramSettings.ContainsKey(WaypointAttribute.Speed))
      {
        var defaultLHS = DocumentSettings.CreateDefaultLapHistogramSettings();
        s.LapHistogramSettings.Add(WaypointAttribute.Speed, defaultLHS[WaypointAttribute.Speed]);
      }

      if (!s.DefaultSessionSettings.RouteLineSettingsCollection.ContainsKey(WaypointAttribute.Speed))
      {
        var defaultRLS = SessionSettings.CreateDefaultRouteLineSettingsCollection();
        s.DefaultSessionSettings.RouteLineSettingsCollection.Add(WaypointAttribute.Speed, defaultRLS[WaypointAttribute.Speed]);
      }

      // add some direction waypoint attribute settings, introduced in QR 2.2
      if (!s.ColorRangeIntervalSliderSettings.ContainsKey(WaypointAttribute.DirectionDeviationToNextLap))
      {
        var defaultCRISS = DocumentSettings.CreateDefaultColorRangeIntervalSliderSettings();
        s.ColorRangeIntervalSliderSettings.Add(WaypointAttribute.DirectionDeviationToNextLap, defaultCRISS[WaypointAttribute.DirectionDeviationToNextLap]);
      }

      if (!s.LapHistogramSettings.ContainsKey(WaypointAttribute.DirectionDeviationToNextLap))
      {
        var defaultLHS = DocumentSettings.CreateDefaultLapHistogramSettings();
        s.LapHistogramSettings.Add(WaypointAttribute.DirectionDeviationToNextLap, defaultLHS[WaypointAttribute.DirectionDeviationToNextLap]);
      }

      if (!s.DefaultSessionSettings.RouteLineSettingsCollection.ContainsKey(WaypointAttribute.DirectionDeviationToNextLap))
      {
        var defaultRLS = SessionSettings.CreateDefaultRouteLineSettingsCollection();
        s.DefaultSessionSettings.RouteLineSettingsCollection.Add(WaypointAttribute.DirectionDeviationToNextLap, defaultRLS[WaypointAttribute.DirectionDeviationToNextLap]);
      }

      // add some map reading duration attribute settings, introduced in QR 2.4
      if (!s.ColorRangeIntervalSliderSettings.ContainsKey(WaypointAttribute.MapReadingDuration))
      {
        var defaultCRISS = DocumentSettings.CreateDefaultColorRangeIntervalSliderSettings();
        s.ColorRangeIntervalSliderSettings.Add(WaypointAttribute.MapReadingDuration, defaultCRISS[WaypointAttribute.MapReadingDuration]);
      }

      if (!s.LapHistogramSettings.ContainsKey(WaypointAttribute.MapReadingDuration))
      {
        var defaultLHS = DocumentSettings.CreateDefaultLapHistogramSettings();
        s.LapHistogramSettings.Add(WaypointAttribute.MapReadingDuration, defaultLHS[WaypointAttribute.MapReadingDuration]);
      }

      if (!s.DefaultSessionSettings.RouteLineSettingsCollection.ContainsKey(WaypointAttribute.MapReadingDuration))
      {
        var defaultRLS = SessionSettings.CreateDefaultRouteLineSettingsCollection();
        s.DefaultSessionSettings.RouteLineSettingsCollection.Add(WaypointAttribute.MapReadingDuration, defaultRLS[WaypointAttribute.MapReadingDuration]);
      }
      
      if (ApplicationSettings.PublishMapSettings == null)
        ApplicationSettings.PublishMapSettings = new List<PublishMapSettingsItem>();

      // make sure new property types are propagated to the settings
      if (ApplicationSettings.LapPropertyTypes.Count < ApplicationSettings.AvailableLapPropertyTypes.Count)
      {
        foreach (var rpt in ApplicationSettings.AvailableLapPropertyTypes)
        {
          if (!ApplicationSettings.LapPropertyTypes.ContainsRoutePropertyType(rpt.RoutePropertyType))
          {
            ApplicationSettings.LapPropertyTypes.Add(rpt);
          }
        }
      }

      // make sure new property types are propagated to the settings
      if (ApplicationSettings.MomentaneousInfoPropertyTypes.Count < ApplicationSettings.AvailableMomentaneousInfoPropertyTypes.Count)
      {
        foreach (var rpt in ApplicationSettings.AvailableMomentaneousInfoPropertyTypes)
        {
          if (!ApplicationSettings.MomentaneousInfoPropertyTypes.ContainsRoutePropertyType(rpt.RoutePropertyType))
          {
            ApplicationSettings.MomentaneousInfoPropertyTypes.Add(rpt);
          }
        }
      }

    }

    public static string SettingsFileName
    {
      get
      {
        return Path.Combine(CommonUtil.GetApplicationDataPath(), "QuickRoute.settings");
      }
    } 

    public static void EnsureApplicationDataFolderExists()
    {
      // ensure that application data folder and subfolders exists
      string[] necessaryFolders = new[] { 
                                          CommonUtil.GetApplicationDataPath(),
                                          Path.Combine(CommonUtil.GetApplicationDataPath(), "Gradients"),
                                          CommonUtil.GetTempPath()
                                        };
      foreach (string folder in necessaryFolders)
      {
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
      }
    }

    public static ApplicationSettings LoadSettings()
    {
      if (!File.Exists(SettingsFileName))
      {
        return new ApplicationSettings();
      }

      try
      {
        return CommonUtil.DeserializeFromFile<ApplicationSettings>(SettingsFileName);
      }
      catch (Exception)
      {
        return new ApplicationSettings();
      }
    }

    public static void SaveSettings(ApplicationSettings s)
    {
      CommonUtil.SerializeToFile(s, SettingsFileName);
    }

    public static NumericConverter GetNumericConverterFromWaypointAttribute(WaypointAttribute wa)
    {
      switch (wa)
      {
        case WaypointAttribute.Pace:
        case WaypointAttribute.MapReadingDuration:
          return new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
        case WaypointAttribute.Speed:
          return new NumericConverter();
        case WaypointAttribute.DirectionDeviationToNextLap:
          return new NumericConverter { NoOfDecimals = 0 };
        default:
          return new IntConverter();
      }
    }

    public static DialogResult ShowExceptionMessageBox(string message, Exception exception, string title)
    {
      var topLevelException = new ApplicationException(message, exception);
      return ShowExceptionMessageBox(topLevelException, title);
    }

    public static DialogResult ShowExceptionMessageBox(Exception exception, string title)
    {
      var messageBox = new ExceptionMessageBox(title, exception);
      return messageBox.ShowDialog();
    }
  
    public static void CheckForNewVersion()
    {
      var languageString = (ApplicationSettings.UiCulture == null ? "" : ApplicationSettings.UiCulture.ToString());
      var request = new GetCurrentVersionRequest() { UserVersion = Application.ProductVersion, UserLanguage = languageString}; 
      var server = new QuickRouteServer.QuickRouteServer();
      server.GetCurrentVersionCompleted += GetCurrentVersionFromServerCompleted;
      try
      {
        server.GetCurrentVersionAsync(request);
      }
      catch(Exception)
      {
        // probably no connection to Internet
      }
    }

    private static void GetCurrentVersionFromServerCompleted(object sender, GetCurrentVersionCompletedEventArgs e)
    {
      try
      {
        var currentVersion = new Version(e.Result.CurrentVersion);
        if (currentVersion > new Version(Application.ProductVersion))
        {
          // there is a new version
          // display message?
          if (!ApplicationSettings.DontRemindAboutVersions.Contains(currentVersion.ToString()))
          {
            var message = string.Format(Strings.NewVersionAvailable, currentVersion) + "\r\n\r\n";
            if (e.Result.Features != null && e.Result.Features.Length > 0)
            {
              message += Strings.NewVersionFeatures;
              foreach (var feature in e.Result.Features)
              {
                var tagsStripped = Regex.Replace(feature, "<.*?>", string.Empty);
                message += "\r\n" + "* " + HttpUtility.UrlDecode(tagsStripped);
              }
            }

            using (var dlg = new NewVersionDialog()
                        {
                          Message = message,
                          DownloadUrl = e.Result.DownloadUrl
                        })
            {
              dlg.ShowDialog();
              if (dlg.DontRemindMe) ApplicationSettings.DontRemindAboutVersions.Add(currentVersion.ToString());
            }
          }
        }
      }
      catch(Exception)
      {
        // probably no connection to Internet
      }
    }
  }
}
