using System;
using System.Collections.Generic;
using System.Xml;

namespace QuickRoute.BusinessEntities.Importers.Polar.ProTrainer
{
  public static class PolarProTrainerUtil
  {
    public static bool IsPolarProTrainerGPXFile(string fileName)
    {
      var doc = new XmlDocument();
      doc.Load(fileName);
      if (doc.DocumentElement == null) return false;
      XmlAttribute creatorAttribute = doc.DocumentElement.Attributes["creator"];
      if (creatorAttribute == null) return false;
      return creatorAttribute.Value.Contains("Polar ProTrainer");
    }

    private static ValidationStatus GetValidationStatus(string timeString)
    {
      int minute = GetMinute(timeString);
      if (minute >= 60) return ValidationStatus.Invalid;
      return (minute % 10 == 0 && minute != 0 ? ValidationStatus.InvalidCandidate : ValidationStatus.Valid);
    }

    public static void CorrectPolarProTrainerGPXFile(string inputFileName, string outputFileName)
    {
      var doc = new XmlDocument();
      doc.Load(inputFileName);
      var nsManager = new XmlNamespaceManager(doc.NameTable);
      nsManager.AddNamespace("gpx", "http://www.topografix.com/GPX/1/1");
      if (doc.DocumentElement != null)
      {
        XmlNodeList times = doc.DocumentElement.SelectNodes("/gpx:gpx/gpx:trk/gpx:trkseg/gpx:trkpt/gpx:time", nsManager);
        if (times != null)
        {
          var timeEntries = new List<TimeEntry>();
          foreach (XmlNode time in times)
          {
            timeEntries.Add(new TimeEntry { TimeNode = time, ValidationStatus = GetValidationStatus(time.InnerText) });
          }

          for (int i = 0; i < timeEntries.Count; i++)
          {
            TimeEntry te = timeEntries[i];
            if (te.ValidationStatus == ValidationStatus.InvalidCandidate)
            {
              // invalid time candidates that are valid:
              // times whose next neightbour's minute has the format x1
              for (int j = i + 1; j < timeEntries.Count; j++)
              {
                if (GetMinute(timeEntries[j].TimeNode.InnerText) != GetMinute(te.TimeNode.InnerText))
                {
                  ValidationStatus status = (GetMinute(timeEntries[j].TimeNode.InnerText) ==
                                             GetMinute(te.TimeNode.InnerText) + 1
                                               ? ValidationStatus.Valid
                                               : ValidationStatus.Invalid);
                  for (int k = i; k < j; k++)
                  {
                    timeEntries[k].ValidationStatus = status;
                  }
                  i = j - 1;
                  break;
                }
              }
            }
          }

          for (int i = timeEntries.Count - 1; i >= 0; i--)
          {
            TimeEntry te = timeEntries[i];
            if (te.ValidationStatus == ValidationStatus.InvalidCandidate)
            {
              // invalid time candidates that are valid:
              // times whose previous neightbour's minute has the format x9
              for (int j = i - 1; j >= 0; j--)
              {
                if (GetMinute(timeEntries[j].TimeNode.InnerText) != GetMinute(te.TimeNode.InnerText))
                {
                  ValidationStatus status = (GetMinute(timeEntries[j].TimeNode.InnerText) ==
                                             GetMinute(te.TimeNode.InnerText) - 1 ||
                                             (GetMinute(timeEntries[j].TimeNode.InnerText) == 0 &&
                                              GetMinute(te.TimeNode.InnerText) == 59)
                                               ? ValidationStatus.Valid
                                               : ValidationStatus.Invalid);
                  for (int k = i; k > j; k--)
                  {
                    timeEntries[k].ValidationStatus = status;
                  }
                  i = j + 1;
                  break;
                }
              }
            }
          }

          for (int i = 0; i < timeEntries.Count; i++)
          {
            TimeEntry te = timeEntries[i];
            if (te.ValidationStatus == ValidationStatus.Invalid)
            {
              te.TimeNode.InnerText = CorrectTimeString(te.TimeNode.InnerText);
            }
          }

          // correct Polar GPX bug when passing midnight - Polar's file starts over with the same date again
          // forum post describing the bug: http://www.matstroeng.se/quickroute/forum/index.php?mode=thread&cat=0&thread=292
          for (int i = 1; i < timeEntries.Count; i++)
          {
            var lastTime = DateTime.Parse(timeEntries[i - 1].TimeNode.InnerText).ToUniversalTime();
            var thisTime = DateTime.Parse(timeEntries[i].TimeNode.InnerText).ToUniversalTime();
            if (thisTime.AddHours(23) < lastTime && thisTime.AddHours(24) > lastTime)
            {
              timeEntries[i].TimeNode.InnerText = thisTime.AddHours(24).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
          }

        }

        // correct points that have the same location as previous point
        //XmlNodeList points = doc.DocumentElement.SelectNodes("/gpx:gpx/gpx:trk/gpx:trkseg/gpx:trkpt", nsManager);
        //if (points != null)
        //{
        //  LongLat previousPosition = GetLongLatFromTrkpt(points[0]);
        //  for(int i = 1; i<points.Count-1; i++)
        //  {
        //    LongLat currentPosition = GetLongLatFromTrkpt(points[i]);
        //    double dist = LinearAlgebraUtil.DistancePointToPointLongLat(previousPosition, currentPosition);
        //    if (dist < 0.1)
        //    {
        //      LongLat nextPosition = GetLongLatFromTrkpt(points[i+1]);
        //      DateTime previousTime = GetTimeFromTrkpt(points[i - 1], nsManager);
        //      DateTime currentTime = GetTimeFromTrkpt(points[i], nsManager);
        //      DateTime nextTime = GetTimeFromTrkpt(points[i + 1], nsManager);
        //      long w1 = currentTime.Subtract(previousTime).Ticks; 
        //      long w2 = nextTime.Subtract(currentTime).Ticks;
        //      currentPosition = ((double) w2/(w1 + w2))*previousPosition + ((double) w1/(w1 + w2))*nextPosition;
        //      points[i].Attributes["lon"].Value = currentPosition.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
        //      points[i].Attributes["lat"].Value = currentPosition.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
        //    }
        //    previousPosition = currentPosition;
        //  }
        //}

        doc.Save(outputFileName);
      }
    }

    private static int GetMinute(string timeString)
    {
      string[] atoms = timeString.Split(":".ToCharArray());
      return Convert.ToInt32(atoms[1]);
    }

    private static string CorrectTimeString(string timeString)
    {
      string[] atoms = timeString.Split(":".ToCharArray());
      atoms[1] = "0" + atoms[1].Substring(0, 1);
      return string.Join(":", atoms);
    }

    private class TimeEntry
    {
      public XmlNode TimeNode { get; set; }
      public ValidationStatus ValidationStatus { get; set; }
    }

    private enum ValidationStatus
    {
      Valid,
      InvalidCandidate,
      Invalid
    }

  }
}
