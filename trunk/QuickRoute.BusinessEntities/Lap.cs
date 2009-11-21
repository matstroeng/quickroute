using System;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class Lap : IComparable<Lap>, ICloneable
  {
    private DateTime time;
    private LapType lapType;
    private string description;

    public Lap()
    {
    }

    public Lap(DateTime time, LapType lapType)
    {
      this.time = time;
      this.lapType = lapType;
    }

    public Lap(DateTime time, LapType lapType, string description)
    {
      this.time = time;
      this.lapType = lapType;
      this.description = description;
    }

    public DateTime Time
    {
      get { return time; }
      set { time = value; }
    }

    public LapType LapType
    {
      get { return lapType; }
      set { lapType = value; }
    }

    public string Description
    {
      get { return description; }
      set { description = value; }
    }

    public LongLat GetLocation(Route route)
    {
      ParameterizedLocation pl = route.GetParameterizedLocationFromTime(time);
      return route.GetLocationFromParameterizedLocation(pl);
    }

    #region IComparable<Lap> Members

    public int CompareTo(Lap other)
    {
      var compare = Math.Sign(time.Ticks - other.Time.Ticks);
      return compare == 0 ? lapType.CompareTo(other.lapType) : compare;
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      return this.MemberwiseClone();
    }

    #endregion
  }

  [Serializable]
  public enum LapType
  {
    Start,
    Lap,
    Stop
  }
}
