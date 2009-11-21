using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuickRoute.BusinessEntities.Importers.Polar
{
  public class PolarHRMImporter
  {
    /// <summary>
    /// Adds laps and heart rate and altitude data from a Polar HRM file. GPS locations and other data are split into two files, GPX and HRM.
    /// </summary>
    /// <param name="fileName">The file name of the HRM file</param>
    /// <param name="importResult">The object where e g the route and laps collection are stored</param>
    public void AddLapsAndHRData(string fileName, ImportResult importResult)
    {
      using (StreamReader sr = new StreamReader(fileName))
      {
        HRMSection currentSection = HRMSection.None;
        DateTime startTime = importResult.Route.FirstWaypoint.Time;
        List<double> heartRates = new List<double>();
        List<double> altitudes = new List<double>();
        int intTimesRowCount = 0;

        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();

          if (line.StartsWith("["))
          {
            // new section of file
            currentSection = StringToHRMSection(line);
          }
          else
          {
            string[] atoms;
            switch (currentSection)
            {
              case HRMSection.IntTimes:
                if (line != "")
                {
                  atoms = line.Split("\t".ToCharArray());
                  if (intTimesRowCount % 5 == 0 && atoms.Length == 5)
                  {
                    double lapTimeInSeconds = TimeStringToSeconds(atoms[0]);
                    DateTime lapTime = startTime.AddSeconds(lapTimeInSeconds);
                    // only add lap if it is within the time span of the session
                    if (lapTimeInSeconds != 0 &&
                        lapTime > importResult.Laps[0].Time &&
                        lapTime < importResult.Laps[importResult.Laps.Count - 1].Time)
                    {
                      importResult.Laps.Add(new Lap(lapTime, LapType.Lap));
                    }
                  }
                  intTimesRowCount += 1;
                }
                break;

              case HRMSection.HRData:
                atoms = line.Split("\t".ToCharArray());
                if (atoms.Length == 3)
                {
                  heartRates.Add(Convert.ToDouble(atoms[0]));
                  altitudes.Add(Convert.ToDouble(atoms[2]));
                }
                break;
            }
          }
        }

        // add heart rates and altitudes
        int count = 0;
        foreach (var segment in importResult.Route.Segments)
        {
          foreach (var waypoint in segment.Waypoints)
          {
            if (count < heartRates.Count)
            {
              waypoint.HeartRate = heartRates[count];
              waypoint.Altitude = altitudes[count];
              count++;
            }
          }
        }

        sr.Close();
      }
    }

    private static double TimeStringToSeconds(string value)
    {
      string[] atoms = value.Split(":".ToCharArray());
      if (atoms.Length == 3)
      {
        string[] secondAtoms = atoms[2].Split(".".ToCharArray());
        return 3600 * Convert.ToInt32(atoms[0]) +
               60 * Convert.ToInt32(atoms[1]) +
               Convert.ToInt32(secondAtoms[0]) +
               Convert.ToDouble(secondAtoms[1]) / 10;
      }
      return 0;
    }

    private static HRMSection StringToHRMSection(string s)
    {
      switch (s)
      {
        case "[Params]": return HRMSection.Params;
        case "[Note]": return HRMSection.Note;
        case "[IntTimes]": return HRMSection.IntTimes;
        case "[IntNotes]": return HRMSection.IntNotes;
        case "[ExtraData]": return HRMSection.ExtraData;
        case "[Summary-123]": return HRMSection.Summary_123;
        case "[Summary-TH]": return HRMSection.Summary_TH;
        case "[HRZones]": return HRMSection.HRZones;
        case "[SwapTimes]": return HRMSection.SwapTimes;
        case "[Trip]": return HRMSection.Trip;
        case "[HRData]": return HRMSection.HRData;
        default: return HRMSection.None;
      }
    }

    private enum HRMSection
    {
      None,
      Params,
      Note,
      IntTimes,
      IntNotes,
      ExtraData,
      Summary_123,
      Summary_TH,
      HRZones,
      SwapTimes,
      Trip,
      HRData
    }
  }
}
