using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class RouteLocations : IComparable<RouteLocations>
  {
    /// <summary>
    /// Used for RouteSpanProperty.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public RouteLocations(ParameterizedLocation start, ParameterizedLocation end)
    {
      Start = start;
      End = end;
      Location = end;
    }

    /// <summary>
    /// Used for RouteMomentaneousProperty and RouteFromStartProperty.
    /// </summary>
    /// <param name="location"></param>
    public RouteLocations(ParameterizedLocation location)
    {
      Start = ParameterizedLocation.Start;
      End = location;
      Location = location;
    }

    /// <summary>
    /// Start of route span. Used for RouteSpanProperty.
    /// </summary>
    public ParameterizedLocation Start { get; set; }
    
    /// <summary>
    /// End of route span. Used for RouteSpanProperty.
    /// </summary>
    public ParameterizedLocation End { get; set; }
    
    /// <summary>
    /// Route location. Used for RouteMomentaneousProperty and RouteFromStartProperty.
    /// </summary>
    public ParameterizedLocation Location { get; set; }

    /// <summary>
    /// Returns true if this is a span location (i e the Start and End properties are set).
    /// </summary>
    public bool IsSpan
    {
      get { return Start != null && End != null; }
    }


    public int CompareTo(RouteLocations other)
    {
      if (Location != null && other.Location == null) return -1;
      if (Location == null && other.Location != null) return 1;
      if (Location != null && other.Location != null) return Location.CompareTo(other.Location);
      var startComparison = Start.CompareTo(other.Start);
      return startComparison != 0 ? startComparison : End.CompareTo(other.End);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (RouteLocations)) return false;
      return Equals((RouteLocations) obj);
    }

    public bool Equals(RouteLocations obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return Equals(obj.Start, Start) && Equals(obj.End, End) && Equals(obj.Location, Location);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = (Start != null ? Start.GetHashCode() : 0);
        result = (result*397) ^ (End != null ? End.GetHashCode() : 0);
        result = (result*397) ^ (Location != null ? Location.GetHashCode() : 0);
        return result;
      }
    }
  }
}
