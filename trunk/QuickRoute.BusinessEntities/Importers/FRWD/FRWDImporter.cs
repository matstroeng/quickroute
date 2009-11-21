using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Importers.FRWD
{
  public class FRWDImporter : IRouteFileImporter
  {
    public event EventHandler<EventArgs> BeginWork;
    public event EventHandler<EventArgs> EndWork;
    public event EventHandler<WorkProgressEventArgs> WorkProgress;
    private ImportResult importResult;

    public ImportResult ImportResult
    {
      get { return importResult; }
      set { importResult = value; }
    }

    public string FileName { get; set; }

    public DialogResult ShowPreImportDialogs()
    {
      return DialogResult.OK;
    }

    public void Import()
    {
      importResult = new ImportResult();
      if (BeginWork != null) BeginWork(this, new EventArgs());
      StreamReader reader = new StreamReader(FileName);
      RouteSegment rs = new RouteSegment();
      LapCollection laps = new LapCollection();
      int i;
      string[] atoms;
      string line;

      for (i = 0; i < 3; i++) reader.ReadLine();

      // start time
      atoms = reader.ReadLine().Split("\t".ToCharArray());
      DateTime startTime = DateTime.MinValue;
      int firstColonPosition = atoms[1].IndexOf(":");
      int lastColonPosition = atoms[1].LastIndexOf(":");
      int length = lastColonPosition + 2 - firstColonPosition + 2 + 1;
      if (length > 0)
      {
        if(DateTime.TryParse(atoms[1].Substring(firstColonPosition - 2, lastColonPosition + 2 - firstColonPosition + 2 + 1), out startTime))
        {
          startTime = startTime.ToUniversalTime();
        }
      }

      // go to start of coordinates
      i = 0;
      while (i < 5 && !reader.EndOfStream)
      {
        line = reader.ReadLine();
        if (line == "") i++;
      }
      reader.ReadLine();
      reader.ReadLine();

      // read all coordinates
      while (!reader.EndOfStream)
      {
        line = reader.ReadLine();
        if (line == "") break;
        atoms = line.Split("\t".ToCharArray());
        Waypoint w = new Waypoint();
        w.Time = startTime.AddSeconds(Convert.ToInt32(atoms[0]));
        w.LongLat = new LongLat(ParseFRWDLongLatString(atoms[5]), ParseFRWDLongLatString(atoms[4]));
        double altitude;
        double.TryParse(LocalizeDecimalString(atoms[6]), out altitude);
        w.Altitude = altitude;
        double heartRate;
        double.TryParse(LocalizeDecimalString(atoms[9]), out heartRate);
        w.HeartRate = heartRate;
        rs.Waypoints.Add(w);

        if (atoms[1] != "")
        {
          // lap
          Lap lap = new Lap();
          lap.LapType = LapType.Lap;
          lap.Time = w.Time;
          laps.Add(lap);
        }
      }

      if (laps.Count > 0) laps[0].LapType = LapType.Start;
      if (laps.Count > 1) laps[laps.Count - 1].LapType = LapType.Stop;

      importResult.Laps = laps;
      List<RouteSegment> routeSegments = new List<RouteSegment>();
      if(rs.Waypoints.Count > 0)
      {
        routeSegments.Add(rs);
        importResult.Route = new Route(routeSegments);
        importResult.Succeeded = true;
      }
      else
      {
        importResult.Succeeded = false;
        importResult.Error = ImportError.NoWaypoints; 
      }

      reader.Close();
      if (EndWork != null) EndWork(this, new EventArgs());
    }

    private static double ParseFRWDLongLatString(string s)
    {
      double value;
      double.TryParse(LocalizeDecimalString(s), out value);
      bool isNegative = (value < 0);
      value = Math.Abs(value);

      return (isNegative ? -1 : 1) *
             (Math.Floor(value / 100) +
              (value % 100) / 60);
    }

    private static string LocalizeDecimalString(string s)
    {
      s = s.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
      s = s.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
      return s;
    }

  }
}
