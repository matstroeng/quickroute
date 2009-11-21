using System;
using System.Collections.Generic;

namespace QuickRoute.BusinessEntities
{
  /// <summary>
  /// Class containing route transformed to the map coordinate system.
  /// </summary>
  public class AdjustedRoute
  {
    private List<AdjustedRouteSegment> segments = new List<AdjustedRouteSegment>();

    public List<AdjustedRouteSegment> Segments
    {
      get { return segments; }
      set { segments = value; }
    }

    public AdjustedRouteSegment FirstSegment
    {
      get
      {
        return segments.Count == 0 ? null : segments[0];
      }
    }

    public AdjustedRouteSegment LastSegment
    {
      get
      {
        return segments.Count == 0 ? null : segments[segments.Count - 1];
      }
    }

    public RectangleD GetBoundingRectangle(ParameterizedLocation pl0, ParameterizedLocation pl1)
    {
      PointD p0 = GetLocationFromParameterizedLocation(pl0);
      PointD p1 = GetLocationFromParameterizedLocation(pl1);

      if (p0 == null || p1 == null) return null;

      double minX = Math.Min(p0.X, p1.X);
      double maxX = Math.Max(p0.X, p1.X);
      double minY = Math.Min(p0.Y, p1.Y);
      double maxY = Math.Max(p0.Y, p1.Y);

      ParameterizedLocation pl = new ParameterizedLocation(pl0.SegmentIndex, Math.Ceiling(pl0.Value));
      while (pl < pl1)
      {
        pl++;
        if ((int)pl.Value >= segments[pl.SegmentIndex].Waypoints.Count) pl = new ParameterizedLocation(pl.SegmentIndex + 1, 0);
        PointD p = segments[pl.SegmentIndex].Waypoints[(int)pl.Value].Location;

        if (p.X < minX) minX = p.X;
        if (p.X > maxX) maxX = p.X;
        if (p.Y < minY) minY = p.Y;
        if (p.Y > maxY) maxY = p.Y;
      }
      return new RectangleD(minX, minY, maxX - minX, maxY - minY);
    }

    public PointD GetLocationFromParameterizedLocation(ParameterizedLocation parameterizedLocation)
    {
      if (parameterizedLocation == null) return null;

      List<AdjustedWaypoint> waypoints = segments[parameterizedLocation.SegmentIndex].Waypoints;

      if (parameterizedLocation.Value <= waypoints[0].ParameterizedLocation.Value) return waypoints[0].Location;
      if (parameterizedLocation.Value >= waypoints[waypoints.Count - 1].ParameterizedLocation.Value) return waypoints[waypoints.Count - 1].Location;

      for (var i = (int)parameterizedLocation.Value; i < waypoints.Count; i++)
      {
        if (parameterizedLocation.Value <= waypoints[i].ParameterizedLocation.Value)
        {
          PointD p0 = waypoints[i - 1].Location;
          PointD p1 = waypoints[i].Location;
          double t = (parameterizedLocation.Value - waypoints[i - 1].ParameterizedLocation.Value) / (waypoints[i].ParameterizedLocation.Value - waypoints[i - 1].ParameterizedLocation.Value);
          return new PointD(p0.X + t * (p1.X - p0.X), p0.Y + t * (p1.Y - p0.Y));
        }
      }
      return waypoints[waypoints.Count - 1].Location;
    }
    
    public ParameterizedLocation GetClosestParameterizedLocation(PointD location, out double distance)
    {
      if (location == null)
      {
        distance = 0;
        return null;
      }

      double closestDistance = 0.0;
      ParameterizedLocation closestDistanceParameterizedLocation = new ParameterizedLocation(0, 0);
      bool closestDistanceSet = false;
      double limit = 2*32;

      for (int i = 0; i < segments.Count; i++)
      {
        List<AdjustedWaypoint> waypoints = segments[i].Waypoints;
        PointD p0 = waypoints[0].Location;
        for (int j = 1; j < waypoints.Count; j++)
        {
          PointD p1 = waypoints[j].Location;

          // long distance between p0 and p1? then we need to check more time-consuming ClosestDistancePointToLine even if p0 is far from location
          bool isLongLineSegment = (LinearAlgebraUtil.DistancePointToPoint(p0, p1) > limit);

          if (LinearAlgebraUtil.DistancePointToPoint(location, p1) < limit || isLongLineSegment)
          {
            double t;
            double tmpDistance = LinearAlgebraUtil.ClosestDistancePointToLine(location, p0, p1, out t);
            if (tmpDistance < closestDistance || !closestDistanceSet)
            {
              closestDistance = tmpDistance;
              closestDistanceParameterizedLocation = 
                new ParameterizedLocation(i, 
                                          waypoints[j - 1].ParameterizedLocation.Value + t * (waypoints[j].ParameterizedLocation.Value - waypoints[j - 1].ParameterizedLocation.Value));
              closestDistanceSet = true;
            }
          }
          p0 = p1;
        }
      }

      distance = closestDistance;
      return (closestDistanceSet ? closestDistanceParameterizedLocation : null);
    }

    public bool IsStartOfSegment(ParameterizedLocation pl)
    {
      if (pl == null) return false;
      return (pl.Value == 0);
    }

    public bool IsEndOfSegment(ParameterizedLocation pl)
    {
      if (pl == null) return false;
      return (pl.Value == segments[pl.SegmentIndex].Waypoints.Count - 1);
    }

    public bool IsStartOfSegment(int segmentIndex, double waypointIndex)
    {
      return (waypointIndex == 0);
    }

    public bool IsEndOfSegment(int segmentIndex, double waypointIndex)
    {
      return (waypointIndex == segments[segmentIndex].Waypoints.Count - 1);
    }

    public ParameterizedLocation GetFirstParameterizedLocation()
    {
      return new ParameterizedLocation(0, 0);
    }

    public ParameterizedLocation GetLastParameterizedLocation()
    {
      return new ParameterizedLocation(segments.Count - 1, LastSegment.LastWaypoint.ParameterizedLocation.Value);
    }

    public double GetWaypointIndexFromParameterizedLocation(ParameterizedLocation pl)
    {
      if (pl == null) return 0;

      List<AdjustedWaypoint> waypoints = segments[pl.SegmentIndex].Waypoints;

      if (pl.Value <= waypoints[0].ParameterizedLocation.Value) return 0;
      if (pl.Value >= waypoints[waypoints.Count - 1].ParameterizedLocation.Value) return waypoints.Count - 1;

      for (int i = 1; i < waypoints.Count; i++)
      {
        if (pl.Value <= waypoints[i].ParameterizedLocation.Value)
        {
          double t = (pl.Value - waypoints[i - 1].ParameterizedLocation.Value) / (waypoints[i].ParameterizedLocation.Value - waypoints[i - 1].ParameterizedLocation.Value);
          return i - 1 + t;
        }
      }
      return waypoints.Count - 1;
    }

    public PointD GetDirectionVectorFromParameterizedLocation(ParameterizedLocation pl)
    {
      if (pl == null) return null;

      double wi = GetWaypointIndexFromParameterizedLocation(pl);
      if (wi == Math.Floor(wi))
      {
        // exactly at a waypoint
        if (wi == 0)
        {
          // first waypoint
          AdjustedWaypoint wp0 = segments[pl.SegmentIndex].Waypoints[0];
          AdjustedWaypoint wp1 = segments[pl.SegmentIndex].Waypoints[1];
          return LinearAlgebraUtil.Normalize(wp1.Location - wp0.Location);
        }
        else if ((int)wi == segments[pl.SegmentIndex].Waypoints.Count - 1)
        {
          // last waypoint
          int lastWaypointIndex = segments[pl.SegmentIndex].Waypoints.Count - 1;
          AdjustedWaypoint wp0 = segments[pl.SegmentIndex].Waypoints[lastWaypointIndex - 1];
          AdjustedWaypoint wp1 = segments[pl.SegmentIndex].Waypoints[lastWaypointIndex];
          return LinearAlgebraUtil.Normalize(wp1.Location - wp0.Location);
        }
        else
        {
          AdjustedWaypoint wp0 = segments[pl.SegmentIndex].Waypoints[(int)wi - 1];
          AdjustedWaypoint wp1 = segments[pl.SegmentIndex].Waypoints[(int)wi];
          AdjustedWaypoint wp2 = segments[pl.SegmentIndex].Waypoints[(int)wi + 1];
          return LinearAlgebraUtil.Normalize(
            LinearAlgebraUtil.Normalize(wp2.Location - wp1.Location) +
            LinearAlgebraUtil.Normalize(wp1.Location - wp0.Location)
            );
        }
      }
      else
      {
        AdjustedWaypoint wp0 = segments[pl.SegmentIndex].Waypoints[(int)Math.Floor(wi)];
        AdjustedWaypoint wp1 = segments[pl.SegmentIndex].Waypoints[(int)Math.Ceiling(wi)];
        return LinearAlgebraUtil.Normalize(wp1.Location - wp0.Location);
      }
    }

    public PointD GetLocationFromWaypointIndex(int segmentIndex, double waypointIndex)
    {
      List<AdjustedWaypoint> waypoints = segments[segmentIndex].Waypoints;

      if (waypointIndex <= 0) return waypoints[0].Location;
      if (waypointIndex >= waypoints.Count - 1) return waypoints[waypoints.Count - 1].Location;

      PointD p0 = waypoints[(int)Math.Floor(waypointIndex)].Location;
      PointD p1 = waypoints[(int)Math.Floor(waypointIndex) + 1].Location;
      double t = waypointIndex - Math.Floor(waypointIndex);

      return p0 + t * (p1 - p0);
    }
    
    public AdjustedWaypoint CreateWaypointFromParameterizedLocation(ParameterizedLocation pl, AdjustedWaypoint.AdjustedWaypointType type)
    {
      return new AdjustedWaypoint(
        GetLocationFromParameterizedLocation(pl),
        pl,
        type);
    }

    public IList<AdjustedWaypoint> GetAllWaypoints()
    {
      var adjustedWaypoints = new List<AdjustedWaypoint>();
      foreach (var segment in segments)
      {
        adjustedWaypoints.AddRange(segment.Waypoints);
      }
      return adjustedWaypoints;
    }
  }

  /// <summary>
  /// Class containing route segments transformed to the map coordinate system.
  /// </summary>
  public class AdjustedRouteSegment
  {
    private List<AdjustedWaypoint> waypoints = new List<AdjustedWaypoint>();

    public List<AdjustedWaypoint> Waypoints
    {
      get { return waypoints; }
      set { waypoints = value; }
    }

    public AdjustedWaypoint FirstWaypoint
    {
      get
      {
        return waypoints.Count == 0 ? null : waypoints[0];
      }
    }

    public AdjustedWaypoint LastWaypoint
    {
      get
      {
        return waypoints.Count == 0 ? null : waypoints[waypoints.Count - 1];
      }
    }
  
  }

  /// <summary>
  /// Class containing coordinates in the map coordinate system.
  /// </summary>
  public class AdjustedWaypoint : IComparable<AdjustedWaypoint>
  {
    /// <summary>
    /// Location in non-zoomed map coordinates.
    /// </summary>
    public PointD Location { get; set; }
    public ParameterizedLocation ParameterizedLocation { get; set; }
    public AdjustedWaypointType Type { get; set; }

    public AdjustedWaypoint(PointD location, ParameterizedLocation parameterizedLocation)
    {
      Location = location;
      ParameterizedLocation = parameterizedLocation;
    }

    public AdjustedWaypoint(PointD location, ParameterizedLocation parameterizedLocation, AdjustedWaypointType type)
    {
      Location = location;
      ParameterizedLocation = parameterizedLocation;
      Type = type;
    }

    public AdjustedWaypoint()
    {

    }

    public int CompareTo(AdjustedWaypoint other)
    {
      int result = ParameterizedLocation.CompareTo(other.ParameterizedLocation);
      return (result != 0 ? result : Type.CompareTo(other.Type));
      }

    public enum AdjustedWaypointType
    {
      Normal,
      Start,
      End,
      AlphaAdjustmentChange
    }
  }
}