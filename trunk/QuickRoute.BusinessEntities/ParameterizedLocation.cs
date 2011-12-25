using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities
{
  /// <summary>
  /// Class for representing a location on a route relative to the indexes of the segments and waypoints.
  /// </summary>
  [Serializable]
  public class ParameterizedLocation : IComparable<ParameterizedLocation>
  {
    private int segmentIndex;
    private double value;

    public ParameterizedLocation(int segmentIndex, double value)
    {
      this.segmentIndex = segmentIndex;
      this.value = value;
    }

    public ParameterizedLocation(ParameterizedLocation pl)
    {
      segmentIndex = pl.segmentIndex;
      value = pl.value;
    }

    public int SegmentIndex
    {
      get { return segmentIndex; }
      set { segmentIndex = value; }
    }

    public double Value
    {
      get { return value; }
      set { this.value = value; }
    }

    public int CompareTo(ParameterizedLocation other)
    {
      int indexSign = Math.Sign(segmentIndex - other.SegmentIndex);
      if (indexSign != 0) return indexSign;
      return Math.Sign(value - other.Value);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is ParameterizedLocation)) return false;
      return this == (ParameterizedLocation)obj;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return  (SegmentIndex.GetHashCode() * 397) ^ Value.GetHashCode();
      }
    }

    public static bool operator <(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      return pl0.CompareTo(pl1) < 0;
    }

    public static bool operator <=(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      return pl0.CompareTo(pl1) <= 0;
    }

    public static bool operator >(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      return pl0.CompareTo(pl1) > 0;
    }

    public static bool operator >=(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      return pl0.CompareTo(pl1) >= 0;
    }

    public static bool operator ==(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      if ((object)pl0 == null && (object)pl1 == null) return true;
      if ((object)pl0 == null || (object)pl1 == null) return false;
      return pl0.CompareTo(pl1) == 0;
    }

    public static bool operator !=(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      return !(pl0 == pl1);
    }

    public static ParameterizedLocation operator -(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      if (pl0 <= pl1)
      {
        return new ParameterizedLocation(0, 0);
      }
      else if (pl0.SegmentIndex == pl1.SegmentIndex)
      {
        return new ParameterizedLocation(0, pl0.Value - pl1.Value);
      }
      else
      {
        return new ParameterizedLocation(pl0.SegmentIndex - pl1.SegmentIndex, pl0.Value);
      }
    }

    public static ParameterizedLocation operator ++(ParameterizedLocation pl)
    {
      return new ParameterizedLocation(pl.segmentIndex, Math.Floor(pl.value + 1));
    }

    public ParameterizedLocation Floor()
    {
      return new ParameterizedLocation(segmentIndex, Math.Floor(value));
    }

    public ParameterizedLocation Ceiling()
    {
      return new ParameterizedLocation(segmentIndex, Math.Ceiling(value));
    }

    /// <summary>
    /// Decides whether this parameterized location has a waypoint, i e the value part is an integer
    /// </summary>
    public bool IsNode
    {
      get { return (value == Math.Floor(value)); }
    }

    public static ParameterizedLocation Start
    {
      get { return new ParameterizedLocation(0,0); }
    }

    public override string ToString()
    {
      return segmentIndex + ", " + value;
    }

    public enum Direction
    {
      Backward,
      Forward
    }
  }
}
