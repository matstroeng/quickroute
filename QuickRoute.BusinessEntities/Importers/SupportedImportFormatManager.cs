using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Importers;
using QuickRoute.BusinessEntities.Importers.FRWD;
using QuickRoute.BusinessEntities.Importers.Garmin.Forerunner;
using QuickRoute.BusinessEntities.Importers.Garmin.ANTAgent;
using QuickRoute.BusinessEntities.Importers.Navilock;
using QuickRoute.BusinessEntities.Importers.Polar.ProTrainer;
using QuickRoute.BusinessEntities.Importers.GPX;
using QuickRoute.BusinessEntities.Importers.QuickRoute;
using QuickRoute.BusinessEntities.Importers.TCX;
using QuickRoute.BusinessEntities.Importers.GlobalSat.GH615M;
using QuickRoute.BusinessEntities.Importers.JJConnect.RegistratorSE;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.Importers
{
  public delegate void RefreshProgressDelegate(string message, int percent);
  public delegate void RefreshCompletedDelegate();
  
  public static class SupportedImportFormatManager
  {
      private static Thread _thread;
      public static event RefreshProgressDelegate RefreshProgressChanged;
    public static event RefreshCompletedDelegate RefreshCompleted;

    public static List<GPSDevice> GetSupportedGPSDevices()
    {
      var supportedGPSDevices = new List<GPSDevice>();

      // Garmin Forerunner
      supportedGPSDevices.Add(new GPSDevice(new GarminForerunnerImporter()));

      // GlobalSat GH-615M
      supportedGPSDevices.Add(new GPSDevice(new GlobalSatGH615MImporter()));

      // JJ-Connect Registrator SE
      supportedGPSDevices.Add(new GPSDevice(new JJConnectRegistratorSEImporter()));

      // Garmin ANT Agent
      GarminANTAgentImporter antImporter = new GarminANTAgentImporter();
      antImporter.Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GARMIN\Devices\";
      GPSDevice antDevice = new GPSDevice(antImporter);
      supportedGPSDevices.Add(antDevice);

      // Polar ProTrainer
      var polarProTrainerImporter = new PolarProTrainerImporter();
      polarProTrainerImporter.Paths = new List<string>
                                        {
                                          Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Polar\Polar ProTrainer\",
                                          Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\VirtualStore\Program Files\Polar\Polar ProTrainer\",
                                        };
      GPSDevice polarProTrainerDevice = new GPSDevice(polarProTrainerImporter);
      supportedGPSDevices.Add(polarProTrainerDevice);

      return supportedGPSDevices;
    }

    public static List<RouteFileFormat> GetSupportedRouteFileFormats()
    {
      var supportedRouteFileFormats = new List<RouteFileFormat>();
      supportedRouteFileFormats.Add(
        new RouteFileFormat(Strings.FileFilter_GpxFiles, new GPXImporter()));

      supportedRouteFileFormats.Add(
        new RouteFileFormat(Strings.FileFilter_AllQuickRouteFiles, new QuickRouteImporter()));

      supportedRouteFileFormats.Add(
        new RouteFileFormat(Strings.FileFilter_TcxFiles, new TCXImporter()));

      supportedRouteFileFormats.Add(
        new RouteFileFormat(Strings.FileFilter_FrwdTxtFiles, new FRWDImporter()));

      supportedRouteFileFormats.Add(
        new RouteFileFormat(Strings.FileFilter_NavilockActFiles, new NavilockImporter()));

      return supportedRouteFileFormats;
    }

    public static List<FileFormat> GetSupportedMapFileFormats()
    {
      var supportedMapImageFileFormats = new List<FileFormat>();
      supportedMapImageFileFormats.Add(
        new FileFormat(Strings.FileFilter_ImageFiles));
      supportedMapImageFileFormats.Add(
        new FileFormat(Strings.FileFilter_QuickRouteFiles));
      supportedMapImageFileFormats.Add(
        new FileFormat(Strings.FileFilter_KmzFiles));
      return supportedMapImageFileFormats;
    }

    public static List<GPSDevice> SearchForGPSDevices()
    {
      return SearchForGPSDevices(GetSupportedGPSDevices());
    }

    public static List<GPSDevice> SearchForGPSDevices(List<GPSDevice> devicesToSearchFor)
    {
      var foundDevices = new List<GPSDevice>();
      foreach (var device in devicesToSearchFor)
      {
        if (device.Importer.IsConnected)
        {
          foundDevices.Add(device);
        }
      }
      return foundDevices;
    }

    public static void StopRefreshThread()
    {
        try
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
            }
        }
        catch (ThreadAbortException)
        {
            if (_thread != null)
            {
                while (_thread.ThreadState == ThreadState.Running)
                {
                }
            }
            _thread = null;
        }
    }

    private static void StartReadThread(ThreadStart threadStartMethod)
    {
        StopRefreshThread();
        var myThread = new ThreadStart(threadStartMethod);
        _thread = new Thread(myThread);
        _thread.Start();
    }

    public static void StartRefreshGPSDevices()
    {
        StartReadThread(RefreshGPSDevices);
    }

    private static void RefreshGPSDevices()
    {
        List<GPSDevice> devices = GetSupportedGPSDevices();
        for (int i = 0; i < devices.Count; i++)
        {
            var device = devices[i];
            var percent = ((i / (Single)devices.Count) * 100);
            if (percent > 100)
            {
                percent = 100;
            }
            if (RefreshProgressChanged != null)
            {
                RefreshProgressChanged(String.Format("Looking for supported devices... ({0}/{1})", i, devices.Count), (int)percent);
            }
            device.Importer.Refresh();
        }
        if (RefreshCompleted != null)
        {
            RefreshCompleted();
        }
    }
  }

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

  public class RouteFileFormat : FileFormat
  {
    private IRouteFileImporter importer;

    public RouteFileFormat(string fileFilter, IRouteFileImporter importer)
      : base(fileFilter)
    {
      this.importer = importer;
    }

    public IRouteFileImporter Importer
    {
      get { return importer; }
      set { importer = value; }
    }
  }

  public class GPSDevice
  {
    private IGPSDeviceImporter importer;

    public GPSDevice(IGPSDeviceImporter importer)
    {
      this.importer = importer;
    }

    public IGPSDeviceImporter Importer
    {
      get { return importer; }
      set { importer = value; }
    }

    public override string ToString()
    {
      if (importer == null)
      {
        return base.ToString();
      }
      else
      {
        return importer.DeviceName;
      }
    }
  }

}
