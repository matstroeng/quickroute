using System;
using System.Collections.Generic;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  [Serializable]
  public class GarminSession
  {
    public GarminSession(D1010_Run_Type run, IList<D1001_Lap_Type> laps, IList<D303_Trk_Point_Type> trackpoints)
    {
      Run = run;
      Laps = laps;
      Trackpoints = trackpoints;
    }

    public D1010_Run_Type Run { get; private set; }

    public IList<D1001_Lap_Type> Laps { get; private set; }

    public IList<D303_Trk_Point_Type> Trackpoints { get; private set; }

    public DateTime StartTime
    {
      get { return Laps[0].StartTimeAsDateTime; }
    }

    public DateTime FinishTime
    {
      get { return Laps[Laps.Count - 1].FinishTimeAsDateTime; }
    }

    public GarminSessionHeader GetHeader()
    {
      return new GarminSessionHeader()
               {
                 StartTime = StartTime,
                 FinishTime = FinishTime,
                 NumberOfLaps = Laps.Count
               };
    }
  }

  [Serializable]
  public class GarminSessionHeader : IComparable<GarminSessionHeader>, IComparable
  {
    public DateTime StartTime { get; set; }
    public DateTime FinishTime { get; set; }
    public int NumberOfLaps { get; set; }

    public string Key
    {
      get { return string.Format("{0:yyyyMMddHHmmss}-{1:yyyyMMddHHmmss}-{2}", StartTime, FinishTime, NumberOfLaps); }
    }

    #region IComparable<GarminSession> Members

    public int CompareTo(GarminSessionHeader other)
    {
      if (NumberOfLaps == 0 && other.NumberOfLaps == 0) return 0;
      if (NumberOfLaps == 0 && other.NumberOfLaps > 0) return 1;
      if (NumberOfLaps > 0 && other.NumberOfLaps == 0) return -1;

      var startTimeComparison = StartTime.CompareTo(other.StartTime);
      return startTimeComparison != 0
               ? startTimeComparison
               : FinishTime.CompareTo(other.FinishTime);
    }

    #endregion

    #region IComparable Members

    public int CompareTo(object obj)
    {
      var other = obj as GarminSessionHeader;
      if (other != null)
      {
        return CompareTo(other);
      }
      else
      {
        return -1;
      }

    }

    #endregion

    public override string ToString()
    {
      return NumberOfLaps > 0
               ? StartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
               : "";
    }
  }
}