using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Wintellect.PowerCollections;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class Route : ISerializable
  {
    private readonly List<RouteSegment> segments = new List<RouteSegment>();
    private OrderedBag<DateTime> lapTimes;
    private Interval speedSmoothingInterval = new Interval(0, 0); // for backwards compatibility only, do not use in new code
    private Dictionary<WaypointAttribute, Interval> smoothingIntervals = SessionSettings.CreateDefaultSmoothingIntervals();
    private bool suppressWaypointAttributeCalculation;
    [NonSerialized] private Dictionary<WaypointAttribute, bool> waypointAttributeExists;

    public Route(List<RouteSegment> segments)
    {
      this.segments = segments;
    }

    protected Route(SerializationInfo info, StreamingContext context)
    {
      segments = (List<RouteSegment>)(info.GetValue("segments", typeof(List<RouteSegment>)));
    }

    #region ISerializable Members

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("segments", segments, typeof(List<RouteSegment>));
    }

    #endregion

    #region Public properties

    public List<RouteSegment> Segments
    {
      get { return segments; }
    }

    public Dictionary<WaypointAttribute, Interval> SmoothingIntervals
    {
      get
      {
        if (smoothingIntervals == null)
        {
          smoothingIntervals = SessionSettings.CreateDefaultSmoothingIntervals();
          smoothingIntervals[WaypointAttribute.Speed] = speedSmoothingInterval;
          smoothingIntervals[WaypointAttribute.Pace] = speedSmoothingInterval;
        }
        return smoothingIntervals;
      }
      set
      {
        smoothingIntervals = value;
        CalculateWaypointAttributes(true);
      }
    }

    public Waypoint FirstWaypoint
    {
      get { return segments.Count == 0 ? null : segments[0].Waypoints[0]; }
    }

    public Waypoint LastWaypoint
    {
      get
      {
        return segments.Count == 0
                 ? null
                 : segments[segments.Count - 1].Waypoints[segments[segments.Count - 1].Waypoints.Count - 1];
      }
    }

    public ParameterizedLocation FirstPL
    {
      get { return new ParameterizedLocation(0, 0); }
    }

    public ParameterizedLocation LastPL
    {
      get { return new ParameterizedLocation(segments.Count - 1, segments[segments.Count - 1].Waypoints.Count - 1); }
    }

    public OrderedBag<DateTime> LapTimes
    {
      get { return lapTimes; }
      set { lapTimes = value; }
    }

    public bool SuppressWaypointAttributeCalculation
    {
      get { return suppressWaypointAttributeCalculation; }
      set { suppressWaypointAttributeCalculation = value; }
    }

    #endregion

    #region  Public methods

    public void Initialize()
    {
      CalculateWaypointAttributes(true);
    }

    public void EnsureUtcTimes()
    {
      foreach (var segment in segments)
      {
        foreach (var waypoint in segment.Waypoints)
        {
          waypoint.Time = waypoint.Time.ToUniversalTime();
        }
      }
    }

    public LongLat CenterLongLat()
    {
      RectangleD boundingRectangle = BoundingRectangle();
      return new LongLat(boundingRectangle.Center.X, boundingRectangle.Center.Y);
    }

    public RectangleD BoundingProjectedRectangle(LongLat projectionOrigin)
    {
      double minX = 0;
      double minY = 0;
      double maxX = 0;
      double maxY = 0;

      for (int i = 0; i < segments.Count; i++)
      {
        for (int j = 0; j < segments[i].Waypoints.Count; j++)
        {
          PointD projectedLocation = segments[i].Waypoints[j].LongLat.Project(projectionOrigin);
          if (projectedLocation.X < minX || (i == 0 && j == 0)) minX = projectedLocation.X;
          if (projectedLocation.Y < minY || (i == 0 && j == 0)) minY = projectedLocation.Y;
          if (projectedLocation.X > maxX || (i == 0 && j == 0)) maxX = projectedLocation.X;
          if (projectedLocation.Y > maxY || (i == 0 && j == 0)) maxY = projectedLocation.Y;
        }
      }
      return new RectangleD(new PointD(minX, minY), new SizeD(maxX - minX, maxY - minY));
    }

    public RectangleD BoundingRectangle()
    {
      double minX = 0;
      double minY = 0;
      double maxX = 0;
      double maxY = 0;

      for (int i = 0; i < segments.Count; i++)
      {
        for (int j = 0; j < segments[i].Waypoints.Count; j++)
        {
          LongLat longLat = segments[i].Waypoints[j].LongLat;
          if (longLat.Longitude < minX || (i == 0 && j == 0)) minX = longLat.Longitude;
          if (longLat.Latitude < minY || (i == 0 && j == 0)) minY = longLat.Latitude;
          if (longLat.Longitude > maxX || (i == 0 && j == 0)) maxX = longLat.Longitude;
          if (longLat.Latitude > maxY || (i == 0 && j == 0)) maxY = longLat.Latitude;
        }
      }
      return new RectangleD(new PointD(minX, minY), new SizeD(maxX - minX, maxY - minY));
    }

    public PointD GetProjectedLocationFromParameterizedLocation(ParameterizedLocation parameterizedLocation,
                                                                LongLat projectionOrigin)
    {
      List<Waypoint> waypoints = segments[parameterizedLocation.SegmentIndex].Waypoints;
      if (parameterizedLocation.IsNode) return waypoints[(int)parameterizedLocation.Value].LongLat.Project(projectionOrigin);
      var i = (int)parameterizedLocation.Floor().Value;
      if (i >= waypoints.Count - 1) i = waypoints.Count - 2;
      if (waypoints.Count < 2) return waypoints[0].LongLat.Project(projectionOrigin);
      double d = parameterizedLocation.Value - i;

      PointD p0 = waypoints[i].LongLat.Project(projectionOrigin);
      PointD p1 = waypoints[i + 1].LongLat.Project(projectionOrigin);

      return new PointD(p0.X + d * (p1.X - p0.X), p0.Y + d * (p1.Y - p0.Y));
    }

    public LongLat GetLocationFromParameterizedLocation(ParameterizedLocation parameterizedLocation)
    {
      List<Waypoint> waypoints = segments[parameterizedLocation.SegmentIndex].Waypoints;
      if (parameterizedLocation.IsNode) return waypoints[(int)parameterizedLocation.Value].LongLat;
      var i = (int)parameterizedLocation.Floor().Value;
      if (i >= waypoints.Count - 1) i = waypoints.Count - 2;
      if (waypoints.Count < 2) return waypoints[0].LongLat;
      double d = parameterizedLocation.Value - i;
      LongLat p0 = waypoints[i].LongLat;
      LongLat p1 = waypoints[i + 1].LongLat;

      return new LongLat(p0.Longitude + d * (p1.Longitude - p0.Longitude), p0.Latitude + d * (p1.Latitude - p0.Latitude));
    }

    public DateTime GetTimeFromParameterizedLocation(ParameterizedLocation parameterizedLocation)
    {
      List<Waypoint> waypoints = segments[parameterizedLocation.SegmentIndex].Waypoints;
      if (parameterizedLocation.IsNode) return waypoints[(int)parameterizedLocation.Value].Time;
      var i = (int)parameterizedLocation.Floor().Value;
      if (i >= waypoints.Count - 1) i = waypoints.Count - 2;
      if (waypoints.Count < 2) return waypoints[0].Time;
      double d = parameterizedLocation.Value - i;

      DateTime dt0 = waypoints[i].Time;
      DateTime dt1 = waypoints[i + 1].Time;
      return new DateTime(dt0.Ticks + (long)(d * (dt1.Ticks - dt0.Ticks)), dt0.Kind);
    }

    public double? GetOriginalAttributeFromParameterizedLocation(WaypointAttribute attribute, ParameterizedLocation parameterizedLocation)
    {
      if (attribute != WaypointAttribute.HeartRate && attribute != WaypointAttribute.Altitude) throw new ArgumentException("The 'attribute' parameter must be either WaypointAttribute.HeartRate or WaypointAttribute.Altitude");

      List<Waypoint> waypoints = segments[parameterizedLocation.SegmentIndex].Waypoints;
      if (parameterizedLocation.IsNode) return waypoints[(int)parameterizedLocation.Value].GetOriginalAttribute(attribute);
      var i = (int)parameterizedLocation.Floor().Value;
      if (i >= waypoints.Count - 1) i = waypoints.Count - 2;
      if (waypoints.Count < 2) return waypoints[0].GetOriginalAttribute(attribute);
      double d = parameterizedLocation.Value - i;

      var v0 = waypoints[i].GetOriginalAttribute(attribute);
      var v1 = waypoints[i + 1].GetOriginalAttribute(attribute);
      if (v0.HasValue && v1.HasValue) return v0 + d * (v1 - v0);
      return null;
    }

    public double? GetAttributeFromParameterizedLocation(WaypointAttribute attribute, ParameterizedLocation parameterizedLocation)
    {
      List<Waypoint> waypoints = segments[parameterizedLocation.SegmentIndex].Waypoints;
      if (parameterizedLocation.IsNode)
      {
        return waypoints[(int)parameterizedLocation.Value].Attributes.ContainsKey(attribute) 
          ? waypoints[(int)parameterizedLocation.Value].Attributes[attribute] 
          : null;
      }
      var i = (int)parameterizedLocation.Floor().Value;
      if (i >= waypoints.Count - 1) i = waypoints.Count - 2;
      if (waypoints.Count < 2) return waypoints[0].Attributes[attribute];
      double d = parameterizedLocation.Value - i;

      return GetAttributeValueBetweenWaypoints(waypoints[i], waypoints[i + 1], d, attribute);
    }

    /// <summary>
    /// Relatively slow, use the three parameter overload if possible.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public ParameterizedLocation GetParameterizedLocationFromTime(DateTime time)
    {
      return GetParameterizedLocationFromTime(time, false);
    }

    /// <summary>
    /// Relatively slow, use the three parameter overload if possible.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="strict">If true, returns null if outside segments</param>
    /// <returns></returns>
    public ParameterizedLocation GetParameterizedLocationFromTime(DateTime time, bool strict)
    {
      if (time <= FirstWaypoint.Time) return FirstPL;
      if (time >= LastWaypoint.Time) return LastPL;
      for (int i = 0; i < segments.Count; i++)
      {
        RouteSegment segment = segments[i];
        if (!(strict && (time < segment.FirstWaypoint.Time || time > segment.LastWaypoint.Time))) // don't look outside segment if in strict mode
        {
          if (time <= segment.FirstWaypoint.Time) return new ParameterizedLocation(i, 0);
          if (time <= segment.LastWaypoint.Time)
          {
            int lo = 0;
            int hi = segment.Waypoints.Count - 2;
            while (hi >= lo)
            {
              int mi = (lo + hi) / 2;
              if (time < segment.Waypoints[mi].Time)
              {
                hi = mi - 1;
              }
              else if (time > segment.Waypoints[mi + 1].Time)
              {
                lo = mi + 1;
              }
              else
              {
                var delta = segment.Waypoints[mi + 1].Time.Ticks - segment.Waypoints[mi].Time.Ticks;
                if (delta == 0) return new ParameterizedLocation(i, mi);
                return new ParameterizedLocation(i,
                                                 mi +
                                                 (double)(time.Ticks - segment.Waypoints[mi].Time.Ticks) /
                                                 (segment.Waypoints[mi + 1].Time.Ticks -
                                                  segment.Waypoints[mi].Time.Ticks));
              }
            }
          }
        }
      }
      return null;
    }

    public ParameterizedLocation GetParameterizedLocationFromTime(DateTime time, ParameterizedLocation initialGuess,
                                                                  ParameterizedLocation.Direction direction)
    {
      if (time <= FirstWaypoint.Time) return FirstPL;
      if (time >= LastWaypoint.Time) return LastPL;
      if (initialGuess == null) return GetParameterizedLocationFromTime(time);
      ParameterizedLocation pl = new ParameterizedLocation(initialGuess).Floor();

      if (direction == ParameterizedLocation.Direction.Forward)
      {
        ParameterizedLocation lastPL = LastPL;
        while (pl < lastPL)
        {
          var w2pl = new ParameterizedLocation(pl.SegmentIndex, (int)pl.Value + 1);
          if (!IsLastPLInSegment(w2pl))
          {
            Waypoint w1 = segments[pl.SegmentIndex].Waypoints[(int)pl.Value];
            Waypoint w2 = segments[w2pl.SegmentIndex].Waypoints[(int)w2pl.Value];
            if (w2 != null && time >= w1.Time && time <= w2.Time)
            {
              if (w2.Time.Ticks == w1.Time.Ticks) return new ParameterizedLocation(pl.SegmentIndex, (int)pl.Value);
              return new ParameterizedLocation(pl.SegmentIndex,
                                               (int)pl.Value +
                                               (double)(time.Ticks - w1.Time.Ticks) / (w2.Time.Ticks - w1.Time.Ticks));
            }
          }
          pl = GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        }
      }
      else
      {
        ParameterizedLocation firstPL = FirstPL;
        while (pl > firstPL)
        {
          var w2pl = new ParameterizedLocation(pl.SegmentIndex, (int)pl.Value + 1);
          if (!IsLastPLInSegment(w2pl))
          {
            Waypoint w1 = segments[pl.SegmentIndex].Waypoints[(int)pl.Value];
            Waypoint w2 = segments[w2pl.SegmentIndex].Waypoints[(int)w2pl.Value];
            if (w2 != null && time >= w1.Time && time <= w2.Time)
            {
              return new ParameterizedLocation(pl.SegmentIndex,
                                               (int)pl.Value +
                                               (double)(time.Ticks - w1.Time.Ticks) / (w2.Time.Ticks - w1.Time.Ticks));
            }
          }
          pl = GetNextPLNode(pl, ParameterizedLocation.Direction.Backward);
        }
      }
      return GetParameterizedLocationFromTime(time);
    }

    /// <summary>
    /// Relatively slow.
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public ParameterizedLocation GetParameterizedLocationFromDistance(double distance)
    {
      if (distance <= FirstWaypoint.Attributes[WaypointAttribute.Distance].Value) return FirstPL;
      if (distance >= LastWaypoint.Attributes[WaypointAttribute.Distance].Value) return LastPL;
      for (int i = 0; i < segments.Count; i++)
      {
        RouteSegment segment = segments[i];
        for (int j = 0; j < segment.Waypoints.Count - 1; j++)
        {
          if (distance >= segment.Waypoints[j].Attributes[WaypointAttribute.Distance].Value && distance <= segment.Waypoints[j + 1].Attributes[WaypointAttribute.Distance].Value)
          {
            return new ParameterizedLocation(i,
                                             j +
                                             (distance - segment.Waypoints[j].Attributes[WaypointAttribute.Distance].Value) /
                                             (segment.Waypoints[j + 1].Attributes[WaypointAttribute.Distance].Value - segment.Waypoints[j].Attributes[WaypointAttribute.Distance].Value));
          }
          if (distance < segment.Waypoints[j].Attributes[WaypointAttribute.Distance].Value)
          {
            return new ParameterizedLocation(i, j);
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Only to be used inside a route segment!
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public List<ParameterizedLocation> GetEquallySpacedParameterizedLocations(DateTime startTime, DateTime endTime,
                                                                              TimeSpan interval)
    {
      return GetEquallySpacedParameterizedLocations(startTime, endTime, interval,
                                                    new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    /// <summary>
    /// Only to be used inside a route segment!
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="interval"></param>
    /// <param name="referenceTime"></param>
    /// <returns></returns>
    public List<ParameterizedLocation> GetEquallySpacedParameterizedLocations(DateTime startTime, DateTime endTime,
                                                                              TimeSpan interval, DateTime referenceTime)
    {
      decimal offset = Math.Ceiling(((decimal)startTime.Ticks - referenceTime.Ticks) / interval.Ticks);
      var time = new DateTime(Convert.ToInt64(referenceTime.Ticks + offset * interval.Ticks), startTime.Kind);
      var pls = new List<ParameterizedLocation>();
      ParameterizedLocation pl = GetParameterizedLocationFromTime(time);
      while (time <= endTime)
      {
        pls.Add(pl);
        time += interval;
        pl = GetParameterizedLocationFromTime(time, pl, ParameterizedLocation.Direction.Forward);
      }
      return pls;
    }

    /// <summary>
    /// Only to be used inside a route segment!
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public List<DateTime> GetEquallySpacedTimes(DateTime startTime, DateTime endTime, TimeSpan interval)
    {
      return GetEquallySpacedTimes(startTime, endTime, interval, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    /// <summary>
    /// Only to be used inside a route segment!
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="interval"></param>
    /// <param name="referenceTime"></param>
    /// <returns></returns>
    public List<DateTime> GetEquallySpacedTimes(DateTime startTime, DateTime endTime, TimeSpan interval, DateTime referenceTime)
    {
      decimal offset = Math.Ceiling(((decimal)startTime.Ticks - referenceTime.Ticks) / interval.Ticks);
      var time = new DateTime(Convert.ToInt64(referenceTime.Ticks + offset * interval.Ticks), startTime.Kind);
      var times = new List<DateTime>();
      while (time <= endTime)
      {
        times.Add(time);
        time += interval;
      }
      return times;
    }

    /// <summary>
    /// Only to be used inside a route segment!
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="interval"></param>
    /// <param name="referenceTime"></param>
    /// <returns></returns>
    public List<Waypoint> GetEquallySpacedWaypoints(DateTime startTime, DateTime endTime, TimeSpan interval,
                                                    DateTime referenceTime)
    {
      decimal offset = Math.Ceiling(((decimal)startTime.Ticks - referenceTime.Ticks) / interval.Ticks);
      var time = new DateTime(Convert.ToInt64(referenceTime.Ticks + offset * interval.Ticks), startTime.Kind);
      List<ParameterizedLocation> pls = GetEquallySpacedParameterizedLocations(startTime, endTime, interval,
                                                                               referenceTime);
      var waypoints = new List<Waypoint>();
      foreach (ParameterizedLocation pl in pls)
      {
        var w = CreateWaypointFromParameterizedLocation(pl);
        w.Time = time; // to prevent rounding errors
        waypoints.Add(w);
        time += interval;
      }
      return waypoints;
    }

    /// <summary>
    /// Only to be used inside a route segment!
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public List<Waypoint> GetEquallySpacedWaypoints(DateTime startTime, DateTime endTime, TimeSpan interval)
    {
      return GetEquallySpacedWaypoints(startTime, endTime, interval, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    /// <summary>
    /// Calculates elapsed time, distance, direction and speed for all waypoints in the route.
    /// </summary>
    public void CalculateWaypointAttributes(bool routeChanged)
    {
      if (suppressWaypointAttributeCalculation) return;
      // first pass: location, elapsed time, distance, direction

      if (routeChanged)
      {
        double distance = 0;
        double elapsedTime = 0;
        for (int i = 0; i < segments.Count; i++)
        {
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            Waypoint w = segments[i].Waypoints[j];
            double deltaDistance = 0;
            double deltaTime = 0;

            w.Attributes[WaypointAttribute.Longitude] = w.LongLat.Longitude;
            w.Attributes[WaypointAttribute.Latitude] = w.LongLat.Latitude;
            if (j > 0)
            {
              deltaDistance = LinearAlgebraUtil.DistancePointToPointLongLat(w.LongLat,
                                                                            segments[i].Waypoints[j - 1].LongLat);
              deltaTime = w.Time.Subtract(segments[i].Waypoints[j - 1].Time).TotalSeconds;
            }
            elapsedTime += deltaTime;
            distance += deltaDistance;
            w.Attributes[WaypointAttribute.Distance] = distance;
            w.Attributes[WaypointAttribute.ElapsedTime] = elapsedTime;
          }
        }

        // check existence of heart rate, altitude and map reading
        waypointAttributeExists = new Dictionary<WaypointAttribute, bool>();
        var attributesToCheckExistenceFor = new List<WaypointAttribute>
                                              {
                                                WaypointAttribute.HeartRate,
                                                WaypointAttribute.Altitude,
                                                WaypointAttribute.MapReadingDuration,
                                                WaypointAttribute.Cadence,
                                                WaypointAttribute.Power
                                              };
        foreach (var wa in attributesToCheckExistenceFor)
        {
          waypointAttributeExists[wa] = CheckIfWaypointAttributeExists(wa);
        }

        // second pass: the rest
        CalculateSpeeds();
        CalculateSlidingAverageAttributes(WaypointAttribute.HeartRate, SmoothingIntervals[WaypointAttribute.HeartRate]);
        CalculateSlidingAverageAttributes(WaypointAttribute.Altitude, SmoothingIntervals[WaypointAttribute.Altitude]);
        CalculateInclinations();
        CalculateMapReadings();
        CalculateCadences();
        CalculatePowers();
      }
      // CalculateDirectionDeviationsToNextLap is also dependent on laps, not just route change
      CalculateDirectionDeviationsToNextLap();
    }

    public Waypoint CreateWaypointFromParameterizedLocation(ParameterizedLocation parameterizedLocation)
    {
      var w = new Waypoint();
      List<Waypoint> waypoints = segments[parameterizedLocation.SegmentIndex].Waypoints;
      var i = (int)parameterizedLocation.Floor().Value;
      if (i >= waypoints.Count - 1) i = waypoints.Count - 2;
      if (waypoints.Count < 2) return waypoints[0];
      double d = parameterizedLocation.Value - i;

      // time
      DateTime dt0 = waypoints[i].Time;
      DateTime dt1 = waypoints[i + 1].Time;
      w.Time = new DateTime(dt0.Ticks + (long)(d * (dt1.Ticks - dt0.Ticks)), dt0.Kind);

      // location
      LongLat p0 = waypoints[i].LongLat;
      LongLat p1 = waypoints[i + 1].LongLat;
      w.LongLat = new LongLat(p0.Longitude + d * (p1.Longitude - p0.Longitude),
                              p0.Latitude + d * (p1.Latitude - p0.Latitude));

      // altitude
      if (waypoints[i].Altitude.HasValue && waypoints[i + 1].Altitude.HasValue)
      {
        w.Altitude = waypoints[i].Altitude + d * (waypoints[i + 1].Altitude - waypoints[i].Altitude);
      }

      // heart rate
      if (waypoints[i].HeartRate.HasValue && waypoints[i + 1].HeartRate.HasValue)
      {
        w.HeartRate = waypoints[i].HeartRate + d * (waypoints[i + 1].HeartRate - waypoints[i].HeartRate);
      }

      var attributes = new List<WaypointAttribute>
                         {
                           WaypointAttribute.Pace,
                           WaypointAttribute.HeartRate,
                           WaypointAttribute.Altitude,
                           WaypointAttribute.Speed,
                           WaypointAttribute.DirectionDeviationToNextLap,
                           WaypointAttribute.ElapsedTime,
                           WaypointAttribute.Distance,
                           WaypointAttribute.Inclination,
                           WaypointAttribute.Direction,
                           WaypointAttribute.Longitude,
                           WaypointAttribute.Latitude,
                           WaypointAttribute.Cadence,
                           WaypointAttribute.Power
                         };

      // map reading
      if(waypointAttributeExists[WaypointAttribute.MapReadingDuration])
      {
        if(parameterizedLocation.IsNode)
        {
          w.MapReadingState = waypoints[i].MapReadingState;
        }
        else
        {
          w.MapReadingState = waypoints[i].MapReadingState == MapReadingState.NotReading || waypoints[i + 1].MapReadingState == MapReadingState.NotReading
            ? MapReadingState.NotReading
            : MapReadingState.Reading;
        }
        attributes.AddRange(new[]
                              {
                                WaypointAttribute.MapReadingDuration,
                                WaypointAttribute.MapReadingState,
                                WaypointAttribute.PreviousMapReadingEnd,
                                WaypointAttribute.NextMapReadingStart,
                              });
      }
      
      foreach (var attribute in attributes)
      {
        w.Attributes[attribute] = GetAttributeValueBetweenWaypoints(waypoints[i], waypoints[i + 1], d, attribute);
      }

      return w;
    }

    public Waypoint CreateWaypointFromTime(DateTime time)
    {
      return CreateWaypointFromParameterizedLocation(GetParameterizedLocationFromTime(time));
    }

    public CutRouteData Cut(ParameterizedLocation parameterizedLocation, CutType cutType)
    {
      var cutRoute = new CutRouteData { CutType = cutType };
      switch (cutType)
      {
        case CutType.Before:
          cutRoute.Segments.AddRange(segments.GetRange(0, parameterizedLocation.SegmentIndex));
          if (!parameterizedLocation.IsNode)
          {
            // cut between two waypoints: create cut waypoint
            Waypoint cutWaypoint = CreateWaypointFromParameterizedLocation(parameterizedLocation);
            var rs = new RouteSegment();
            segments.RemoveRange(0, parameterizedLocation.SegmentIndex);
            rs.Waypoints.AddRange(segments[0].Waypoints.GetRange(0, (int)parameterizedLocation.Ceiling().Value));
            cutRoute.Segments.Add(rs);
            segments[0].Waypoints.RemoveRange(0, (int)parameterizedLocation.Ceiling().Value);
            segments[0].Waypoints.Insert(0, cutWaypoint);
            cutRoute.WaypointInsertedAtCut = cutWaypoint;
          }
          else
          {
            // cut exactly at a waypoint
            var rs = new RouteSegment();
            segments.RemoveRange(0, parameterizedLocation.SegmentIndex);
            rs.Waypoints.AddRange(segments[0].Waypoints.GetRange(0, (int)parameterizedLocation.Value));
            cutRoute.Segments.Add(rs);
            segments[0].Waypoints.RemoveRange(0, (int)parameterizedLocation.Value);
          }
          // handle map readings nicely
          if (waypointAttributeExists[WaypointAttribute.MapReadingDuration] &&
             segments[0].FirstWaypoint.MapReadingState == MapReadingState.Reading)
          {
            segments[0].FirstWaypoint.MapReadingState = MapReadingState.StartReading;
          }
          break;

        case CutType.After:
          if (parameterizedLocation.SegmentIndex < segments.Count - 1)
          {
            cutRoute.Segments.AddRange(segments.GetRange(parameterizedLocation.SegmentIndex + 1,
                                                         segments.Count - 1 - parameterizedLocation.SegmentIndex));
            segments.RemoveRange(parameterizedLocation.SegmentIndex + 1,
                                 segments.Count - 1 - parameterizedLocation.SegmentIndex);
          }
          var i = (int)parameterizedLocation.Ceiling().Value;
          int count = segments[parameterizedLocation.SegmentIndex].Waypoints.Count;
          if (!parameterizedLocation.IsNode)
          {
            // cut between two waypoints: create cut waypoint
            Waypoint cutWaypoint = CreateWaypointFromParameterizedLocation(parameterizedLocation);
            var rs = new RouteSegment();
            rs.Waypoints.AddRange(segments[parameterizedLocation.SegmentIndex].Waypoints.GetRange(i, count - i));
            cutRoute.Segments.Insert(0, rs);
            segments[parameterizedLocation.SegmentIndex].Waypoints.RemoveRange(i, count - i);
            segments[parameterizedLocation.SegmentIndex].Waypoints.Insert(i, cutWaypoint);
            cutRoute.WaypointInsertedAtCut = cutWaypoint;
          }
          else
          {
            // cut exactly at a waypoint
            var rs = new RouteSegment();
            rs.Waypoints.AddRange(segments[parameterizedLocation.SegmentIndex].Waypoints.GetRange(i + 1, count - 1 - i));
            cutRoute.Segments.Insert(0, rs);
            segments[parameterizedLocation.SegmentIndex].Waypoints.RemoveRange(i + 1, count - 1 - i);
          }
          // handle map readings nicely
          var lastWaypoint = segments[segments.Count - 1].LastWaypoint;
          if (waypointAttributeExists[WaypointAttribute.MapReadingDuration] &&
             lastWaypoint.MapReadingState == MapReadingState.Reading)
          {
            lastWaypoint.MapReadingState = MapReadingState.EndReading;
          }
          break;
      }
      return cutRoute;
    }

    public void UnCut(CutRouteData cutRouteData)
    {
      Waypoint cutWaypoint;
      switch (cutRouteData.CutType)
      {
        case CutType.Before:
          cutWaypoint = segments[0].FirstWaypoint;
          var beforeCutWaypoint = cutRouteData.Segments[cutRouteData.Segments.Count - 1].LastWaypoint;
          if (cutRouteData.WaypointInsertedAtCut != null)
          {
            segments[0].Waypoints.Remove(cutRouteData.WaypointInsertedAtCut);
          }
          segments.InsertRange(0, cutRouteData.Segments.GetRange(0, cutRouteData.Segments.Count - 1));
          segments[cutRouteData.Segments.Count - 1].Waypoints.
            InsertRange(0, cutRouteData.Segments[cutRouteData.Segments.Count - 1].Waypoints);
          // handle map reading
          if (cutRouteData.WaypointInsertedAtCut == null)
          {
            if (cutWaypoint.MapReadingState == MapReadingState.StartReading &&
               (beforeCutWaypoint.MapReadingState == MapReadingState.Reading || beforeCutWaypoint.MapReadingState == MapReadingState.StartReading))
            {
              cutWaypoint.MapReadingState = MapReadingState.Reading;
            }
          }
          break;

        case CutType.After:
          cutWaypoint = segments[segments.Count - 1].LastWaypoint;
          var afterCutWaypoint = cutRouteData.Segments[0].FirstWaypoint;
          if (cutRouteData.WaypointInsertedAtCut != null)
          {
            segments[segments.Count - 1].Waypoints.Remove(cutRouteData.WaypointInsertedAtCut);
          }
          segments[segments.Count - 1].Waypoints.AddRange(cutRouteData.Segments[0].Waypoints);
          segments.AddRange(cutRouteData.Segments.GetRange(1, cutRouteData.Segments.Count - 1));
          // handle map reading
          if (cutRouteData.WaypointInsertedAtCut == null)
          {
            if (cutWaypoint.MapReadingState == MapReadingState.EndReading &&
               (afterCutWaypoint.MapReadingState == MapReadingState.Reading || afterCutWaypoint.MapReadingState == MapReadingState.EndReading))
            {
              cutWaypoint.MapReadingState = MapReadingState.Reading;
            }
          }
          break;
      }
    }

    public List<double?> GetMomentaneousValueList(WaypointAttribute attribute, ParameterizedLocation startPL,
                                                  ParameterizedLocation endPL, TimeSpan samplingInterval)
    {
      DateTime time = GetTimeFromParameterizedLocation(startPL);
      DateTime endTime = GetTimeFromParameterizedLocation(endPL);
      ParameterizedLocation previousPL = GetParameterizedLocationOfPreviousWaypoint(startPL, false);
      ParameterizedLocation nextPL = GetParameterizedLocationOfNextWaypoint(startPL, true);
      Waypoint previousWaypoint = GetClosestWaypointFromParameterizedLocation(previousPL);
      Waypoint nextWaypoint = GetClosestWaypointFromParameterizedLocation(nextPL);
      var values = new List<double?>();

      while (time < endTime)
      {
        double t0 = time.Subtract(previousWaypoint.Time).TotalSeconds;
        double t1 = nextWaypoint.Time.Subtract(previousWaypoint.Time).TotalSeconds;
        double t = (t1 > 0 ? t0 / t1 : 0);
        double? value = GetAttributeValueBetweenWaypoints(previousWaypoint, nextWaypoint, 1 - t, attribute);
        values.Add(value);

        // is there a waypoint before next sample time?
        DateTime nextSampleTime = time.Add(samplingInterval);
        while (nextWaypoint.Time <= nextSampleTime && nextSampleTime < endTime)
        {
          previousPL = nextPL;
          nextPL = GetParameterizedLocationOfNextWaypoint(nextPL, true);
          if (previousPL.SegmentIndex != nextPL.SegmentIndex)
          {
            // moved to next segment
            previousPL = nextPL;
            nextPL = GetParameterizedLocationOfNextWaypoint(nextPL, true);
            previousWaypoint = GetClosestWaypointFromParameterizedLocation(previousPL);
            nextWaypoint = GetClosestWaypointFromParameterizedLocation(nextPL);
            nextSampleTime = previousWaypoint.Time;
          }
          else
          {
            previousWaypoint = nextWaypoint;
            nextWaypoint = GetClosestWaypointFromParameterizedLocation(nextPL);
          }
        }
        time = nextSampleTime;
      }
      return values;
    }

    /// <summary>
    /// Gets the values at the specified percentage when the attribute values are sorted.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="percentage"></param>
    /// <param name="samplingInterval"></param>
    /// <returns></returns>
    public double? GetOrderedValue(WaypointAttribute attribute, double percentage, TimeSpan samplingInterval)
    {
      List<double?> list = GetMomentaneousValueList(
        attribute,
        GetParameterizedLocationFromTime(FirstWaypoint.Time),
        GetParameterizedLocationFromTime(LastWaypoint.Time),
        samplingInterval);
      list.Sort();

      var i = (int)(percentage * list.Count);
      return list[Math.Min(i, list.Count - 1)];
    }

    /// <summary>
    /// Gets the values at each percentage specified in the percentages list when the attribute values are sorted.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="percentages"></param>
    /// <param name="samplingInterval"></param>
    /// <returns></returns>
    public List<double?> GetOrderedValues(WaypointAttribute attribute, List<double> percentages,
                                          TimeSpan samplingInterval)
    {
      List<double?> list = GetMomentaneousValueList(
        attribute,
        GetParameterizedLocationFromTime(FirstWaypoint.Time),
        GetParameterizedLocationFromTime(LastWaypoint.Time),
        samplingInterval);
      list.Sort();

      var values = new List<double?>();
      foreach (double percentage in percentages)
      {
        var i = (int)(percentage * list.Count);
        int index = Math.Min(i, list.Count - 1);
        if (index >= 0) values.Add(list[index]);
      }
      return values;
    }

    public double CalculatePaceStandardDeviation(ParameterizedLocation start, ParameterizedLocation end)
    {
      return CalculatePaceStandardDeviation(start, end, new Interval(0, 0));
    }

    public double CalculatePaceStandardDeviation(ParameterizedLocation start, ParameterizedLocation end,
                                                 Interval slidingAverageInterval)
    {
      // first collect all waypoints
      var nodes = new List<Pair<ParameterizedLocation, Waypoint>>();
      ParameterizedLocation adjustedStart = GetParameterizedLocationOfNextWaypoint(start, false);
      ParameterizedLocation adjustedEnd = GetParameterizedLocationOfPreviousWaypoint(end, false);
      var pl = new ParameterizedLocation(adjustedStart);

      if (!start.IsNode)
        nodes.Add(new Pair<ParameterizedLocation, Waypoint>(start, CreateWaypointFromParameterizedLocation(start)));
      while (pl != null && pl <= adjustedEnd)
      {
        if (pl.Value > segments[pl.SegmentIndex].Waypoints.Count - 1)
        {
          pl.SegmentIndex++;
          pl.Value = 0;
        }
        nodes.Add(new Pair<ParameterizedLocation, Waypoint>(pl, GetClosestWaypointFromParameterizedLocation(pl)));
        pl = GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
      }
      if (!end.IsNode)
        nodes.Add(new Pair<ParameterizedLocation, Waypoint>(end, CreateWaypointFromParameterizedLocation(end)));


      var paces = new List<StatisticsUtil.WeightedItem>();
      if (slidingAverageInterval.Length == 0)
      {
        for (int i = 0; i < nodes.Count; i++)
        {
          ParameterizedLocation p = nodes[i].First;
          Waypoint w = nodes[i].Second;
          double beforeWeight = (i == 0 || IsFirstPLInSegment(p)
                                   ? 0
                                   : (double)(w.Time.Ticks - nodes[i - 1].Second.Time.Ticks) / TimeSpan.TicksPerSecond / 2);
          double afterWeight = (i == nodes.Count - 1 || IsLastPLInSegment(p)
                                  ? 0
                                  : (double)(nodes[i + 1].Second.Time.Ticks - w.Time.Ticks) / TimeSpan.TicksPerSecond / 2);
          paces.Add(
            new StatisticsUtil.WeightedItem(
              beforeWeight + afterWeight,
              ConvertUtil.ToPace(segments[p.SegmentIndex].Waypoints[(int)p.Value].Attributes[WaypointAttribute.Speed].Value).TotalSeconds
              )
            );
        }
      }
      else
      {
        ParameterizedLocation siStartPL =
          GetParameterizedLocationFromTime(nodes[0].Second.Time.AddSeconds(slidingAverageInterval.Start));
        ParameterizedLocation siEndPL =
          GetParameterizedLocationFromTime(nodes[0].Second.Time.AddSeconds(slidingAverageInterval.End));
        for (int i = 0; i < nodes.Count; i++)
        {
          // start of sliding interval
          siStartPL =
            GetParameterizedLocationFromTime(nodes[i].Second.Time.AddSeconds(slidingAverageInterval.Start), siStartPL,
                                             ParameterizedLocation.Direction.Forward);
          // end of sliding interval
          siEndPL =
            GetParameterizedLocationFromTime(nodes[i].Second.Time.AddSeconds(slidingAverageInterval.End), siEndPL,
                                             ParameterizedLocation.Direction.Forward);
          if (siStartPL != null && siEndPL != null)
          {
            if (siStartPL.SegmentIndex < nodes[i].First.SegmentIndex)
              siStartPL = new ParameterizedLocation(nodes[i].First.SegmentIndex, 0);
            if (siEndPL.SegmentIndex > nodes[i].First.SegmentIndex)
              siEndPL = new ParameterizedLocation(nodes[i].First.SegmentIndex,
                                                  segments[nodes[i].First.SegmentIndex].Waypoints.Count - 1);
            ParameterizedLocation p = nodes[i].First;
            Waypoint w = nodes[i].Second;
            double pace = ConvertUtil.ToPace(w.Attributes[WaypointAttribute.Speed].Value).TotalSeconds;
            double siStartDistance = GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, siStartPL).Value;
            double siEndDistance = GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, siEndPL).Value;
            double siLength =
              GetTimeFromParameterizedLocation(siEndPL).Subtract(GetTimeFromParameterizedLocation(siStartPL)).
                TotalSeconds;
            double meanSpeed = (siLength == 0 ? 0 : (siEndDistance - siStartDistance) / siLength);
            double meanPace = ConvertUtil.ToPace(meanSpeed).TotalSeconds;

            double beforeWeight = (i == 0 || IsFirstPLInSegment(p)
                                     ? 0
                                     : (double)(w.Time.Ticks - nodes[i - 1].Second.Time.Ticks) / TimeSpan.TicksPerSecond /
                                       2);
            double afterWeight = (i == nodes.Count - 1 || IsLastPLInSegment(p)
                                    ? 0
                                    : (double)(nodes[i + 1].Second.Time.Ticks - w.Time.Ticks) / TimeSpan.TicksPerSecond / 2);
            paces.Add(new StatisticsUtil.WeightedItem((beforeWeight + afterWeight), pace - meanPace));
          }
        }
      }
      return StatisticsUtil.GetStandardDeviation(paces);
    }

    public double CalculateSpeedPercentualStandardDeviation(ParameterizedLocation start, ParameterizedLocation end)
    {
      return CalculateSpeedPercentualStandardDeviation(start, end, new Interval(0, 0));
    }

    public double CalculateSpeedPercentualStandardDeviation(ParameterizedLocation start, ParameterizedLocation end,
                                                            Interval slidingAverageInterval)
    {
      // first collect all waypoints
      var nodes = new List<Pair<ParameterizedLocation, Waypoint>>();
      ParameterizedLocation adjustedStart = GetParameterizedLocationOfNextWaypoint(start, false);
      ParameterizedLocation adjustedEnd = GetParameterizedLocationOfPreviousWaypoint(end, false);
      var pl = new ParameterizedLocation(adjustedStart);

      if (!start.IsNode)
        nodes.Add(new Pair<ParameterizedLocation, Waypoint>(start, CreateWaypointFromParameterizedLocation(start)));
      while (pl != null && pl <= adjustedEnd)
      {
        if (pl.Value > segments[pl.SegmentIndex].Waypoints.Count - 1)
        {
          pl.SegmentIndex++;
          pl.Value = 0;
        }
        nodes.Add(new Pair<ParameterizedLocation, Waypoint>(pl, GetClosestWaypointFromParameterizedLocation(pl)));
        pl = GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
      }
      if (!end.IsNode)
        nodes.Add(new Pair<ParameterizedLocation, Waypoint>(end, CreateWaypointFromParameterizedLocation(end)));


      var speeds = new List<StatisticsUtil.WeightedItem>();
      if (slidingAverageInterval.Length == 0)
      {
        double elapsedTime = GetAttributeFromParameterizedLocation(WaypointAttribute.ElapsedTime, LastPL).Value;
        double meanSpeed = (GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, LastPL).Value - GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, FirstPL).Value) /
                           (elapsedTime == 0 ? 0 : elapsedTime);
        for (int i = 0; i < nodes.Count; i++)
        {
          ParameterizedLocation p = nodes[i].First;
          Waypoint w = nodes[i].Second;
          double beforeWeight = (i == 0 || IsFirstPLInSegment(p)
                                   ? 0
                                   : (double)(w.Time.Ticks - nodes[i - 1].Second.Time.Ticks) / TimeSpan.TicksPerSecond / 2);
          double afterWeight = (i == nodes.Count - 1 || IsLastPLInSegment(p)
                                  ? 0
                                  : (double)(nodes[i + 1].Second.Time.Ticks - w.Time.Ticks) / TimeSpan.TicksPerSecond / 2);
          speeds.Add(
            new StatisticsUtil.WeightedItem(
              beforeWeight + afterWeight,
              (meanSpeed == 0 ? 0 : segments[p.SegmentIndex].Waypoints[(int)p.Value].Attributes[WaypointAttribute.Speed].Value / meanSpeed)
              )
            );
        }
      }
      else
      {
        ParameterizedLocation siStartPL =
          GetParameterizedLocationFromTime(nodes[0].Second.Time.AddSeconds(slidingAverageInterval.Start));
        ParameterizedLocation siEndPL =
          GetParameterizedLocationFromTime(nodes[0].Second.Time.AddSeconds(slidingAverageInterval.End));
        for (int i = 0; i < nodes.Count; i++)
        {
          // start of sliding interval
          siStartPL =
            GetParameterizedLocationFromTime(nodes[i].Second.Time.AddSeconds(slidingAverageInterval.Start), siStartPL,
                                             ParameterizedLocation.Direction.Forward);
          // end of sliding interval
          siEndPL =
            GetParameterizedLocationFromTime(nodes[i].Second.Time.AddSeconds(slidingAverageInterval.End), siEndPL,
                                             ParameterizedLocation.Direction.Forward);
          if (siStartPL != null && siEndPL != null)
          {
            if (siStartPL.SegmentIndex < nodes[i].First.SegmentIndex)
              siStartPL = new ParameterizedLocation(nodes[i].First.SegmentIndex, 0);
            if (siEndPL.SegmentIndex > nodes[i].First.SegmentIndex)
              siEndPL = new ParameterizedLocation(nodes[i].First.SegmentIndex,
                                                  segments[nodes[i].First.SegmentIndex].Waypoints.Count - 1);
            ParameterizedLocation p = nodes[i].First;
            Waypoint w = nodes[i].Second;
            double siStartDistance = GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, siStartPL).Value;
            double siEndDistance = GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, siEndPL).Value;
            double siLength =
              GetTimeFromParameterizedLocation(siEndPL).Subtract(GetTimeFromParameterizedLocation(siStartPL)).
                TotalSeconds;
            double meanSpeed = (siLength == 0 ? 0 : (siEndDistance - siStartDistance) / siLength);

            double beforeWeight = (i == 0 || IsFirstPLInSegment(p)
                                     ? 0
                                     : (double)(w.Time.Ticks - nodes[i - 1].Second.Time.Ticks) / TimeSpan.TicksPerSecond /
                                       2);
            double afterWeight = (i == nodes.Count - 1 || IsLastPLInSegment(p)
                                    ? 0
                                    : (double)(nodes[i + 1].Second.Time.Ticks - w.Time.Ticks) / TimeSpan.TicksPerSecond / 2);
            speeds.Add(new StatisticsUtil.WeightedItem((beforeWeight + afterWeight),
                                                       (meanSpeed == 0 ? 0 : w.Attributes[WaypointAttribute.Speed].Value / meanSpeed)));
          }
        }
      }
      return StatisticsUtil.GetStandardDeviation(speeds);
    }

    public double CalculateAverageDirectionDeviationToNextLap(ParameterizedLocation start, ParameterizedLocation end)
    {
      ParameterizedLocation adjustedStart = GetParameterizedLocationOfNextWaypoint(start, false);
      ParameterizedLocation adjustedEnd = GetParameterizedLocationOfPreviousWaypoint(end, false);
      DateTime previousTime = GetTimeFromParameterizedLocation(start);
      var pl = new ParameterizedLocation(adjustedStart);
      double sum = 0;
      double weightSum = 0;
      while (pl != null && pl <= adjustedEnd)
      {
        if (pl.Value > segments[pl.SegmentIndex].Waypoints.Count - 1)
        {
          pl.SegmentIndex++;
          pl.Value = 0;
          previousTime = GetTimeFromParameterizedLocation(pl);
        }
        DateTime time = GetTimeFromParameterizedLocation(pl);
        long weight = time.Ticks - previousTime.Ticks;
        sum += weight * GetAttributeFromParameterizedLocation(WaypointAttribute.DirectionDeviationToNextLap, pl).Value;
        weightSum += weight;
        previousTime = time;
        pl = GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
      }
      return (weightSum == 0 ? 0 : sum / weightSum);
    }

    /// <summary>
    /// Returns true if there are NO null values for the specified attributes in the segments.
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool ContainsWaypointAttribute(WaypointAttribute attribute)
    {
      return waypointAttributeExists[attribute];
    }

    public List<DateTime> GetMapReadingsList()
    {
      if(!waypointAttributeExists[WaypointAttribute.MapReadingDuration]) return null;
      var mapReadingsList = new List<DateTime>();
      for (var i = 0; i < segments.Count; i++)
      {
        for (var j = 0; j < segments[i].Waypoints.Count; j++)
        {
          var w = segments[i].Waypoints[j];
          if(w.MapReadingState == MapReadingState.StartReading || w.MapReadingState == MapReadingState.EndReading) mapReadingsList.Add(w.Time);
        }
      }
      return mapReadingsList.Count == 0 ? null : mapReadingsList;
    }

    #region  ParameterizedLocation helpers

    public bool IsFirstPLInSegment(ParameterizedLocation pl)
    {
      return (pl.Value <= 0);
    }

    public bool IsLastPLInSegment(ParameterizedLocation pl)
    {
      return (pl.Value >= segments[pl.SegmentIndex].Waypoints.Count - 1);
    }

    public bool IsFirstPL(ParameterizedLocation pl)
    {
      return (pl <= FirstPL);
    }

    public bool IsLastPL(ParameterizedLocation pl)
    {
      return (pl >= LastPL);
    }

    public ParameterizedLocation GetNextPLNode(ParameterizedLocation pl, ParameterizedLocation.Direction direction)
    {
      if (direction == ParameterizedLocation.Direction.Forward)
      {
        if (pl.Floor().Value + 1 > segments[pl.SegmentIndex].Waypoints.Count - 1)
        {
          if (pl.SegmentIndex + 1 > segments.Count - 1) return null;
          return new ParameterizedLocation(pl.SegmentIndex + 1, 0);
        }
        return new ParameterizedLocation(pl.SegmentIndex, pl.Floor().Value + 1);
      }
      else
      {
        if (pl.Ceiling().Value - 1 < 0)
        {
          if (pl.SegmentIndex <= 0) return null;
          return new ParameterizedLocation(pl.SegmentIndex - 1, segments[pl.SegmentIndex - 1].Waypoints.Count - 1);
        }
        return new ParameterizedLocation(pl.SegmentIndex, pl.Ceiling().Value - 1);
      }
    }

    public bool IsFirstSegment(ParameterizedLocation pl)
    {
      return (pl.SegmentIndex <= 0);
    }

    public bool IsLastSegment(ParameterizedLocation pl)
    {
      return (pl.SegmentIndex >= segments.Count - 1);
    }

    #endregion

    #endregion

    #region  Private methods

    private double GetAverageSpeed(int currentSegmentIndex, int currentWaypointIndex, DateTime startTime,
                                   DateTime endTime)
    {
      if (startTime == endTime)
      {
        // can't have zero-length interval, so make very small interval to calculate speed for
        startTime = startTime.AddMilliseconds(-0.5);
        endTime = endTime.AddMilliseconds(0.5);
      }
      if (startTime < segments[currentSegmentIndex].FirstWaypoint.Time)
        startTime = Segments[currentSegmentIndex].FirstWaypoint.Time;
      if (endTime > Segments[currentSegmentIndex].LastWaypoint.Time)
        endTime = Segments[currentSegmentIndex].LastWaypoint.Time;

      if (startTime == endTime) return 0; // same time for both first and last waypoint of this segment

      int i = currentWaypointIndex;
      List<Waypoint> waypoints = Segments[currentSegmentIndex].Waypoints;

      if (waypoints[i].Time < startTime)
      {
        while (waypoints[i].Time < startTime)
        {
          i += 1;
        }
      }
      else
      {
        while (waypoints[i].Time > startTime)
        {
          i -= 1;
        }
      }

      double distance = 0.0;
      while (waypoints[i].Time < endTime)
      {
        long ticks0 = Math.Max(startTime.Ticks, waypoints[i].Time.Ticks);
        long ticks1 = Math.Min(endTime.Ticks, waypoints[i + 1].Time.Ticks);
        long ticksDuration = waypoints[i + 1].Time.Ticks - waypoints[i].Time.Ticks;
        if (ticksDuration != 0)
        {
          double t = (ticks1 - ticks0) / (double)ticksDuration;
          distance += t * LinearAlgebraUtil.DistancePointToPointLongLat(waypoints[i].LongLat, waypoints[i + 1].LongLat);
        }
        i++;
      }

      TimeSpan timeSpan = endTime.Subtract(startTime);
      return distance / timeSpan.TotalSeconds;
    }

    private ParameterizedLocation GetParameterizedLocationOfPreviousWaypoint(
      ParameterizedLocation parameterizedLocation, bool forceAdvance)
    {
      if (!parameterizedLocation.IsNode)
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex, parameterizedLocation.Floor().Value);
      }
      else if (parameterizedLocation.Value > 0)
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex,
                                         parameterizedLocation.Value - (forceAdvance ? 1 : 0));
      }
      else if (forceAdvance && parameterizedLocation.SegmentIndex > 0)
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex - 1,
                                         segments[parameterizedLocation.SegmentIndex - 1].Waypoints.Count - 1);
      }
      else
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex, 0);
      }
    }

    private ParameterizedLocation GetParameterizedLocationOfNextWaypoint(ParameterizedLocation parameterizedLocation,
                                                                         bool forceAdvance)
    {
      if (!parameterizedLocation.IsNode)
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex, parameterizedLocation.Ceiling().Value);
      }
      else if (parameterizedLocation.Value < segments[parameterizedLocation.SegmentIndex].Waypoints.Count - 1)
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex,
                                         parameterizedLocation.Value + (forceAdvance ? 1 : 0));
      }
      else if (forceAdvance && parameterizedLocation.SegmentIndex < segments.Count - 1)
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex + 1, 0);
      }
      else
      {
        return new ParameterizedLocation(parameterizedLocation.SegmentIndex,
                                         segments[parameterizedLocation.SegmentIndex].Waypoints.Count - 1);
      }
    }

    private Waypoint GetClosestWaypointFromParameterizedLocation(ParameterizedLocation parameterizedLocation)
    {
      if (parameterizedLocation.SegmentIndex < 0 || parameterizedLocation.SegmentIndex > segments.Count - 1)
        return null;
      if (parameterizedLocation.Value < 0 ||
          parameterizedLocation.Value > segments[parameterizedLocation.SegmentIndex].Waypoints.Count - 1) return null;
      return segments[parameterizedLocation.SegmentIndex].Waypoints[(int)parameterizedLocation.Value];
    }

    private Waypoint GetNextWaypoint(ParameterizedLocation parameterizedLocation, bool forceAdvance)
    {
      return
        GetClosestWaypointFromParameterizedLocation(GetParameterizedLocationOfNextWaypoint(parameterizedLocation,
                                                                                           forceAdvance));
    }

    private Waypoint GetPreviousWaypoint(ParameterizedLocation parameterizedLocation, bool forceAdvance)
    {
      return
        GetClosestWaypointFromParameterizedLocation(GetParameterizedLocationOfPreviousWaypoint(parameterizedLocation,
                                                                                               forceAdvance));
    }

    // w0 and w1 are adjacent, t measured from w0
    private static double? GetAttributeValueBetweenWaypoints(Waypoint w0, Waypoint w1, double t, WaypointAttribute attribute)
    {
      switch(attribute)
      {
        case WaypointAttribute.MapReadingDuration:
          return w0.MapReadingState == MapReadingState.StartReading || w0.MapReadingState == MapReadingState.Reading 
            ? w0.Attributes[WaypointAttribute.MapReadingDuration]
            : null;
        case WaypointAttribute.MapReadingState:
          return GetAttributeValueBetweenWaypoints(w0, w1, t, WaypointAttribute.MapReadingDuration) != null ? 1 : 0;
        case WaypointAttribute.PreviousMapReadingEnd:
          return w1.Attributes[WaypointAttribute.PreviousMapReadingEnd] != null
          ? (double?)(w1.Attributes[WaypointAttribute.PreviousMapReadingEnd].Value - (1 - t) * (w1.Time - w0.Time).TotalSeconds)
          : null;
        case WaypointAttribute.NextMapReadingStart:
          return w0.Attributes[WaypointAttribute.NextMapReadingStart] != null
          ? (double?)(w0.Attributes[WaypointAttribute.NextMapReadingStart].Value - t * (w1.Time - w0.Time).TotalSeconds)
          : null;
        default:
          if (w0.Attributes.ContainsKey(attribute) && w1.Attributes.ContainsKey(attribute))
          {
            var d0 = w0.Attributes[attribute];
            var d1 = w1.Attributes[attribute];
            if (d0.HasValue && d1.HasValue) return d0 + t * (d1 - d0);
            if (!d0.HasValue && !d1.HasValue) return null;
            return (t < 0.5 ? d0 : d1);
          }
          return null;
      }
    }


    private void CalculateSlidingAverageAttributes(WaypointAttribute attribute, Interval smoothingInterval)
    {
      // using optimized but hard-to-understand algorithm
      if (attribute != WaypointAttribute.HeartRate && attribute != WaypointAttribute.Altitude) throw new ArgumentException("The 'attribute' parameter must be either WaypointAttribute.HeartRate or WaypointAttribute.Altitude");

      bool zeroLengthInterval = smoothingInterval.Length == 0;
      bool containsAttribute = ContainsWaypointAttribute(attribute);

      ParameterizedLocation siStartPL = null;
      ParameterizedLocation siEndPL = null;
      if (!zeroLengthInterval)
      {
        siStartPL = GetParameterizedLocationFromTime(FirstWaypoint.Time.AddSeconds(smoothingInterval.Start));
        siEndPL = GetParameterizedLocationFromTime(FirstWaypoint.Time.AddSeconds(smoothingInterval.End));
      }
      for (int i = 0; i < segments.Count; i++)
      {
        double[] valueSums = null;
        bool[] valueIsSet = null;
        DateTime[] valueTimes = null;
        var nullValueFound = false;
        if (containsAttribute && !zeroLengthInterval)
        {
          valueSums = new double[segments[i].Waypoints.Count];
          valueIsSet = new bool[segments[i].Waypoints.Count];
          valueTimes = new DateTime[segments[i].Waypoints.Count];
          valueSums[0] = 0;
          if (segments[i].Waypoints.Count > 0)
          {
            valueIsSet[0] = segments[i].Waypoints[0].GetOriginalAttribute(attribute).HasValue;
            valueTimes[0] = segments[i].Waypoints[0].Time;
            nullValueFound = !valueIsSet[0];
          }
          for (int j = 1; j < segments[i].Waypoints.Count; j++)
          {
            var previousWaypoint = segments[i].Waypoints[j - 1];
            var thisWaypoint = segments[i].Waypoints[j];
            var previousOriginalAttribute = previousWaypoint.GetOriginalAttribute(attribute);
            var thisOriginalAttribute = thisWaypoint.GetOriginalAttribute(attribute);
            valueIsSet[j] = thisOriginalAttribute.HasValue;
            valueTimes[j] = thisWaypoint.Time;
            nullValueFound = nullValueFound || !valueIsSet[j];
            if (valueIsSet[j - 1] && valueIsSet[j])
            {
              valueSums[j] = valueSums[j - 1] +
                             (valueTimes[j] - valueTimes[j - 1]).TotalSeconds *
                             (previousOriginalAttribute.Value + thisOriginalAttribute.Value) / 2;
            }
            else
            {
              valueSums[j] = valueSums[j - 1];
            }
          }
        }

        for (int j = 0; j < segments[i].Waypoints.Count; j++)
        {
          Waypoint w = segments[i].Waypoints[j];
          if (!containsAttribute)
          {
            w.Attributes[attribute] = null;
          }
          else if (zeroLengthInterval)
          {
            w.Attributes[attribute] = w.GetOriginalAttribute(attribute);
          }
          else
          {
            // start of sliding interval
            siStartPL =
              GetParameterizedLocationFromTime(w.Time.AddSeconds(smoothingInterval.Start), siStartPL,
                                               ParameterizedLocation.Direction.Forward);
            // end of sliding interval
            siEndPL =
              GetParameterizedLocationFromTime(w.Time.AddSeconds(smoothingInterval.End), siEndPL,
                                               ParameterizedLocation.Direction.Forward);
            if (siStartPL != null && siEndPL != null)
            {
              if (siStartPL.SegmentIndex < i) siStartPL = new ParameterizedLocation(i, 0);
              if (siEndPL.SegmentIndex > i) siEndPL = new ParameterizedLocation(i, segments[i].Waypoints.Count - 1);

              double startSum;
              double endSum;
              var adjustedIntervalLength = (GetTimeFromParameterizedLocation(siEndPL) - GetTimeFromParameterizedLocation(siStartPL)).TotalSeconds;
              int startIndex;
              int endIndex;
              if (siStartPL.IsNode)
              {
                startSum = valueSums[(int)siStartPL.Value];
                startIndex = (int)siStartPL.Value;
              }
              else
              {
                var d = siStartPL.Value - Math.Floor(siStartPL.Value);
                startSum = (1 - d) * valueSums[(int)siStartPL.Value] + d * valueSums[(int)siStartPL.Value + 1];
                startIndex = (int)siStartPL.Value;
              }
              if (siEndPL.IsNode)
              {
                endSum = valueSums[(int)siEndPL.Value];
                endIndex = (int)siEndPL.Value;
              }
              else
              {
                var d = siEndPL.Value - Math.Floor(siEndPL.Value);
                endSum = (1 - d) * valueSums[(int)siEndPL.Value] + d * valueSums[(int)siEndPL.Value + 1];
                endIndex = (int)siEndPL.Value + 1;
              }

              if (adjustedIntervalLength == 0)
              {
                w.Attributes[attribute] = w.GetOriginalAttribute(attribute);
              }
              else
              {
                // check if there are any null values in this interval
                bool nullValueFoundInInterval;
                if (!nullValueFound)
                {
                  // no null value in whole route, don't need to check this interval
                  nullValueFoundInInterval = false;
                }
                else
                {
                  // there is at least one null value in the route, need to check if there exists any in this interval
                  nullValueFoundInInterval = false;
                  for (var k = startIndex; k <= endIndex; k++)
                  {
                    if (!valueIsSet[k])
                    {
                      nullValueFoundInInterval = true;
                      break;
                    }
                  }
                }
                if (!nullValueFoundInInterval)
                {
                  // no null values in this interval, perform normal calculation
                  w.Attributes[attribute] = (endSum - startSum) / adjustedIntervalLength;
                }
                else
                {
                  // null value found, calculate based on each non-null value waypoint pair
                  adjustedIntervalLength = 0;
                  double sum = 0;
                  for (var k = startIndex; k < endIndex; k++)
                  {
                    if (valueIsSet[k] && valueIsSet[k + 1])
                    {
                      double start = Math.Max(k, siStartPL.Value);
                      double end = Math.Min(k + 1, siEndPL.Value);
                      adjustedIntervalLength += (end - start) * (valueTimes[k + 1] - valueTimes[k]).TotalSeconds;
                      sum += (end - start) * (valueSums[k + 1] - valueSums[k]);
                    }
                  }
                  w.Attributes[attribute] = (adjustedIntervalLength == 0 ? (double?)null : sum / adjustedIntervalLength);
                }
              }
            }
            else
            {
              w.Attributes[attribute] = null;
            }
          }
        }
      }
    }

    /// <summary>
    /// Calculates speeds and paces.
    /// </summary>
    private void CalculateSpeeds()
    {
      // using optimized but hard-to-understand algorithm
      var actualInterval = new Interval(SmoothingIntervals[WaypointAttribute.Speed]);
      if (actualInterval.Length == 0)
      {
        // need to have non-zero smothing interval when calculating speed, set it to a millisecond
        var center = actualInterval.Start;
        actualInterval = new Interval(center - 0.0005, center + 0.0005);
      }
      ParameterizedLocation siStartPL =
        GetParameterizedLocationFromTime(FirstWaypoint.Time.AddSeconds(actualInterval.Start));
      ParameterizedLocation siEndPL = GetParameterizedLocationFromTime(FirstWaypoint.Time.AddSeconds(actualInterval.End));
      for (int i = 0; i < segments.Count; i++)
      {
        for (int j = 0; j < segments[i].Waypoints.Count; j++)
        {
          Waypoint w = segments[i].Waypoints[j];
          // start of sliding interval
          siStartPL =
            GetParameterizedLocationFromTime(w.Time.AddSeconds(actualInterval.Start), siStartPL,
                                             ParameterizedLocation.Direction.Forward);
          // end of sliding interval
          siEndPL =
            GetParameterizedLocationFromTime(w.Time.AddSeconds(actualInterval.End), siEndPL,
                                             ParameterizedLocation.Direction.Forward);
          if (siStartPL != null && siEndPL != null)
          {
            if (siStartPL.SegmentIndex < i) siStartPL = new ParameterizedLocation(i, 0);
            if (siEndPL.SegmentIndex > i) siEndPL = new ParameterizedLocation(i, segments[i].Waypoints.Count - 1);
            double siStartDistance = GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, siStartPL).Value;
            double siEndDistance = GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, siEndPL).Value;
            double siLength = (GetTimeFromParameterizedLocation(siEndPL) - GetTimeFromParameterizedLocation(siStartPL)).TotalSeconds;
            w.Attributes[WaypointAttribute.Speed] = (siLength == 0 ? 0 : 3.6 * (siEndDistance - siStartDistance) / siLength);
          }
          else
          {
            w.Attributes[WaypointAttribute.Speed] = 0;
          }
          w.Attributes[WaypointAttribute.Pace] = ConvertUtil.ToPace(w.Attributes[WaypointAttribute.Speed].Value).TotalSeconds;
        }
      }
    }

    private void CalculateDirectionDeviationsToNextLap()
    {
      if (lapTimes.Count >= 2)
      {
        int lapIndex = 0;
        var lapStartPL = GetParameterizedLocationFromTime(lapTimes[lapIndex]);
        var lapEndPL = GetParameterizedLocationFromTime(lapTimes[lapIndex + 1]);
        // using optimized but hard-to-understand algorithm
        var actualInterval = new Interval(SmoothingIntervals[WaypointAttribute.DirectionDeviationToNextLap]);
        if (actualInterval.Length == 0)
        {
          // need to have non-zero smothing interval when calculating direction vectors, set it to a millisecond
          var center = actualInterval.Start;
          actualInterval = new Interval(center - 0.0005, center + 0.0005);
        }
        ParameterizedLocation siStartPL =
          GetParameterizedLocationFromTime(FirstWaypoint.Time.AddSeconds(actualInterval.Start));
        ParameterizedLocation siEndPL =
          GetParameterizedLocationFromTime(FirstWaypoint.Time.AddSeconds(actualInterval.End));
        for (int i = 0; i < segments.Count; i++)
        {
          // 1. calculate the direction angles in this segment, taking laps into account (never consider positions outside current lap)
          var directionAngles = new double[segments[i].Waypoints.Count];
          var startLapIndexInThisSegment = lapIndex;
          var lapStartPLInThisSegment = GetParameterizedLocationFromTime(lapTimes[lapIndex]);
          var lapEndPLInThisSegment = GetParameterizedLocationFromTime(lapTimes[lapIndex + 1]);
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            Waypoint w = segments[i].Waypoints[j];
            while (w.Time.ToUniversalTime() > lapTimes[lapIndex].ToUniversalTime() && lapIndex < lapTimes.Count - 1)
            {
              lapIndex++;
              lapStartPL = GetParameterizedLocationFromTime(lapTimes[lapIndex - 1]);
              lapEndPL = GetParameterizedLocationFromTime(lapTimes[lapIndex]);
            }

            // start of sliding interval
            siStartPL =
              GetParameterizedLocationFromTime(w.Time.AddSeconds(actualInterval.Start), siStartPL,
                                               ParameterizedLocation.Direction.Forward);
            // end of sliding interval
            siEndPL =
              GetParameterizedLocationFromTime(w.Time.AddSeconds(actualInterval.End), siEndPL,
                                               ParameterizedLocation.Direction.Forward);
            if (siStartPL != null && siEndPL != null)
            {
              if (siStartPL.SegmentIndex < i) siStartPL = new ParameterizedLocation(i, 0);
              if (siEndPL.SegmentIndex > i) siEndPL = new ParameterizedLocation(i, segments[i].Waypoints.Count - 1);
              if (siStartPL < lapStartPL) siStartPL = new ParameterizedLocation(lapStartPL);
              if (siEndPL > lapEndPL) siEndPL = new ParameterizedLocation(lapEndPL);
              var siStartLocation = GetLocationFromParameterizedLocation(siStartPL);
              var siEndLocation = GetLocationFromParameterizedLocation(siEndPL);
              var middle = (siStartLocation / 2 + siEndLocation / 2);
              var p0 = siStartLocation.Project(middle);
              var p1 = siEndLocation.Project(middle);
              directionAngles[j] = LinearAlgebraUtil.GetAngleD(LinearAlgebraUtil.Normalize(p1 - p0));
            }
            else
            {
              directionAngles[j] = 0;
            }
          }


          // 2. calculate the directions and direction deviations based on values achieved in step 1
          lapIndex = startLapIndexInThisSegment;
          lapStartPL = lapStartPLInThisSegment;
          lapEndPL = lapEndPLInThisSegment;
          var lapLongLat = GetLocationFromParameterizedLocation(GetParameterizedLocationFromTime(lapTimes[lapIndex + 1]));
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            Waypoint w = segments[i].Waypoints[j];

            // direction (clockwise from north: n = 0, e = 90, s = 180, w = 270)
            var direction = -directionAngles[j] + 90;
            if (direction < 0) direction += 360;
            w.Attributes[WaypointAttribute.Direction] = direction;

            // direction deviation to next lap
            while (w.Time.ToUniversalTime() > lapTimes[lapIndex].ToUniversalTime() && lapIndex < lapTimes.Count - 1)
            {
              lapIndex++;
              lapLongLat = GetLocationFromParameterizedLocation(GetParameterizedLocationFromTime(lapTimes[lapIndex]));
            }
            LongLat middle = (w.LongLat / 2 + lapLongLat / 2);
            PointD p0 = w.LongLat.Project(middle);
            PointD p1 = lapLongLat.Project(middle);
            PointD directionVectorToNextLap = LinearAlgebraUtil.Normalize(p1 - p0);
            w.Attributes[WaypointAttribute.DirectionDeviationToNextLap] = Math.Abs(LinearAlgebraUtil.GetAngleD(directionVectorToNextLap, LinearAlgebraUtil.CreateNormalizedVectorFromAngleD(directionAngles[j])));
          }
        }
      }
    }

    private void CalculateInclinations()
    {
      var containsAltitude = ContainsWaypointAttribute(WaypointAttribute.Altitude);
      var altitudeSmoothingInterval = new Interval(-0.005, 0.005);
      var distanceSmoothingInterval = SmoothingIntervals[WaypointAttribute.Altitude];
      if (distanceSmoothingInterval.Length == 0) distanceSmoothingInterval = altitudeSmoothingInterval;

      for (int i = 0; i < segments.Count; i++)
      {
        for (int j = 0; j < segments[i].Waypoints.Count; j++)
        {
          var waypoint = segments[i].Waypoints[j];
          if (containsAltitude)
          {
            var pl = new ParameterizedLocation(i, j);
            var startDirection = altitudeSmoothingInterval.Start < 0
                                   ? ParameterizedLocation.Direction.Backward
                                   : ParameterizedLocation.Direction.Forward;
            var endDirection = altitudeSmoothingInterval.End < 0
                                 ? ParameterizedLocation.Direction.Backward
                                 : ParameterizedLocation.Direction.Forward;

            // altitude calculations
            var altitudeStartTime = waypoint.Time.AddSeconds(altitudeSmoothingInterval.Start);
            var altitudeEndTime = waypoint.Time.AddSeconds(altitudeSmoothingInterval.End);
            var altitudeStartPL = GetParameterizedLocationFromTime(altitudeStartTime, pl, startDirection);
            var altitudeEndPL = GetParameterizedLocationFromTime(altitudeEndTime, pl, endDirection);
            if (altitudeStartPL.SegmentIndex < i)
            {
              altitudeStartPL = new ParameterizedLocation(i, 0);
              altitudeStartTime = segments[i].FirstWaypoint.Time;
            }
            if (altitudeEndPL.SegmentIndex > i)
            {
              altitudeEndPL = new ParameterizedLocation(i, segments[i].Waypoints.Count - 1);
              altitudeEndTime = segments[i].LastWaypoint.Time;
            }
            var altitudeDifference = GetAttributeFromParameterizedLocation(WaypointAttribute.Altitude, altitudeEndPL) -
                                     GetAttributeFromParameterizedLocation(WaypointAttribute.Altitude, altitudeStartPL);
            var altitudeDuration = (altitudeEndTime - altitudeStartTime).TotalSeconds;

            // distance calculations
            var distanceStartTime = waypoint.Time.AddSeconds(distanceSmoothingInterval.Start);
            var distanceEndTime = waypoint.Time.AddSeconds(distanceSmoothingInterval.End);
            var distanceStartPL = GetParameterizedLocationFromTime(distanceStartTime, pl, startDirection);
            var distanceEndPL = GetParameterizedLocationFromTime(distanceEndTime, pl, endDirection);
            if (distanceStartPL.SegmentIndex < i)
            {
              distanceStartPL = new ParameterizedLocation(i, 0);
              distanceStartTime = segments[i].FirstWaypoint.Time;
            }
            if (distanceEndPL.SegmentIndex > i)
            {
              distanceEndPL = new ParameterizedLocation(i, segments[i].Waypoints.Count - 1);
              distanceEndTime = segments[i].LastWaypoint.Time;
            }
            var distanceDifference = GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, distanceEndPL) -
                                     GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, distanceStartPL);
            var distanceDuration = (distanceEndTime - distanceStartTime).TotalSeconds;

            // calculate the inclination
            if (!distanceDifference.HasValue || !altitudeDifference.HasValue)
            {
              waypoint.Attributes[WaypointAttribute.Inclination] = null;
            }
            else if (altitudeDuration == 0 || distanceDuration == 0)
            {
              waypoint.Attributes[WaypointAttribute.Inclination] = 0;
            }
            else
            {
              waypoint.Attributes[WaypointAttribute.Inclination] = LinearAlgebraUtil.ToDegrees(Math.Atan2(altitudeDifference.Value / altitudeDuration, distanceDifference.Value / distanceDuration));
            }
          }
          else
          {
            waypoint.Attributes[WaypointAttribute.Inclination] = null;
          }
        }
      }
    }

    private void CalculateMapReadings()
    {
      if (waypointAttributeExists[WaypointAttribute.MapReadingDuration])
      {
        for (int i = 0; i < segments.Count; i++)
        {
          double? duration = null;
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            var waypoint = segments[i].Waypoints[j];
            if (waypoint.MapReadingState == MapReadingState.StartReading)
            {
              var k = j + 1;
              while (segments[i].Waypoints[k].MapReadingState == MapReadingState.Reading) k++;
              duration = (segments[i].Waypoints[k].Time - waypoint.Time).TotalSeconds;
            }
            waypoint.Attributes[WaypointAttribute.MapReadingDuration] = duration;
            waypoint.Attributes[WaypointAttribute.MapReadingState] = duration != null ? 1 : 0;
            if (waypoint.MapReadingState == MapReadingState.EndReading)
            {
              duration = null;
            }
          }

          DateTime? previousEndReadingTime = null;
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            var waypoint = segments[i].Waypoints[j];
            waypoint.Attributes[WaypointAttribute.PreviousMapReadingEnd] = previousEndReadingTime == null ? null : (double?)(waypoint.Time - previousEndReadingTime.Value).TotalSeconds;
            if (waypoint.MapReadingState == MapReadingState.EndReading)
            {
              previousEndReadingTime = waypoint.Time;
            }
          }

          DateTime? nextStartReadingTime = null;
          for (int j = segments[i].Waypoints.Count - 1; j >= 0; j--)
          {
            var waypoint = segments[i].Waypoints[j];
            waypoint.Attributes[WaypointAttribute.NextMapReadingStart] = nextStartReadingTime == null ? null : (double?)(nextStartReadingTime.Value - waypoint.Time).TotalSeconds; ;
            if (waypoint.MapReadingState == MapReadingState.StartReading)
            {
              nextStartReadingTime = waypoint.Time;
            }
          }

        }
      }
      else
      {
        for (int i = 0; i < segments.Count; i++)
        {
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            var waypoint = segments[i].Waypoints[j];
            waypoint.Attributes[WaypointAttribute.MapReadingState] = null;
            waypoint.Attributes[WaypointAttribute.MapReadingDuration] = null;
            waypoint.Attributes[WaypointAttribute.PreviousMapReadingEnd] = null;
            waypoint.Attributes[WaypointAttribute.NextMapReadingStart] = null;
          }
        }
      }
    }

    private void CalculateCadences()
    {
      if (waypointAttributeExists[WaypointAttribute.Cadence])
      {
        for (int i = 0; i < segments.Count; i++)
        {
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            var waypoint = segments[i].Waypoints[j];
            waypoint.Attributes[WaypointAttribute.Cadence] = waypoint.Cadence ?? 0;
          }
        }
      }
      else
      {
        for (int i = 0; i < segments.Count; i++)
        {
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            var waypoint = segments[i].Waypoints[j];
            waypoint.Attributes[WaypointAttribute.Cadence] = null;
          }
        }
      }
    }

    private void CalculatePowers()
    {
      if (waypointAttributeExists[WaypointAttribute.Power])
      {
        for (int i = 0; i < segments.Count; i++)
        {
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            var waypoint = segments[i].Waypoints[j];
            waypoint.Attributes[WaypointAttribute.Power] = waypoint.Power ?? 0;
          }
        }
      }
      else
      {
        for (int i = 0; i < segments.Count; i++)
        {
          for (int j = 0; j < segments[i].Waypoints.Count; j++)
          {
            var waypoint = segments[i].Waypoints[j];
            waypoint.Attributes[WaypointAttribute.Power] = null;
          }
        }
      }
    }

    private int GetNextLapIndexFromTime(DateTime time)
    {
      int lo = 0;
      int hi = lapTimes.Count - 1;
      while (hi >= lo)
      {
        int mi = (lo + hi) / 2;
        if (time < lapTimes[mi])
        {
          hi = mi - 1;
        }
        else if (time > lapTimes[mi + 1])
        {
          lo = mi + 1;
        }
        else
        {
          return Math.Min(mi + 1, lapTimes.Count - 1);
        }
      }
      return lo;
    }

    private bool CheckIfWaypointAttributeExists(WaypointAttribute attribute)
    {
      foreach (RouteSegment segment in Segments)
      {
        foreach (Waypoint waypoint in segment.Waypoints)
        {
          switch (attribute)
          {
            case WaypointAttribute.Altitude:
              if (waypoint.Altitude != null) return true;
              break;

            case WaypointAttribute.HeartRate:
              if (waypoint.HeartRate != null) return true;
              break;

            case WaypointAttribute.MapReadingDuration:
              if (waypoint.MapReadingState != null) return true;
              break;

            case WaypointAttribute.Cadence:
              if (waypoint.Cadence != null) return true;
              break;

            case WaypointAttribute.Power:
              if (waypoint.Power != null) return true;
              break;

          }
        }
      }
      return false;
    }
    #endregion

    #region Public static methods
    public static List<RouteSegment> AddMapReadingWaypoints(List<RouteSegment> routeSegments, List<DateTime> mapReadings)
    {
      if (mapReadings.Count == 0) return routeSegments;
      var newRouteSegments = new List<RouteSegment>();
      foreach (var routeSegment in routeSegments)
      {
        var newRouteSegment = new RouteSegment();
        var waypointCount = 0;
        var mapReadingIndex = -1; // just to be sure, could probably be removed
        foreach (var waypoint in routeSegment.Waypoints)
        {
          while (mapReadingIndex < mapReadings.Count - 1 && mapReadings[mapReadingIndex+1] <= waypoint.Time)
          {
            mapReadingIndex++;
          }
          var newWaypoint = waypoint.Clone();
          if (waypoint == routeSegment.FirstWaypoint)
          {
            newWaypoint.MapReadingState = mapReadingIndex % 2 == 0 ? MapReadingState.StartReading : MapReadingState.NotReading;
            newRouteSegment.Waypoints.Add(newWaypoint);
          }
          else if (waypoint == routeSegment.LastWaypoint)
          {
            newWaypoint.MapReadingState = newRouteSegment.LastWaypoint.MapReadingState == MapReadingState.StartReading || newRouteSegment.LastWaypoint.MapReadingState == MapReadingState.Reading
                                         ? MapReadingState.EndReading
                                         : MapReadingState.NotReading;
            newRouteSegment.Waypoints.Add(newWaypoint);
            break;
          }
          else
          {
            if (mapReadingIndex != -1 && mapReadings[mapReadingIndex] == newWaypoint.Time)
            {
              newWaypoint.MapReadingState = mapReadingIndex % 2 == 0 ? MapReadingState.StartReading : MapReadingState.EndReading;
            }
            else
            {
              newWaypoint.MapReadingState = mapReadingIndex % 2 == 0 ? MapReadingState.Reading : MapReadingState.NotReading;
            }
            newRouteSegment.Waypoints.Add(newWaypoint);
          }
          var nextWaypoint = routeSegment.Waypoints[waypointCount + 1];
          while (mapReadingIndex < mapReadings.Count - 1 && mapReadings[mapReadingIndex + 1] < nextWaypoint.Time)
          {
            var time = mapReadings[mapReadingIndex + 1];
            var t = (double)(time - waypoint.Time).Ticks / (nextWaypoint.Time - waypoint.Time).Ticks;
            var longLat = (1 - t) * waypoint.LongLat + t * nextWaypoint.LongLat;
            var altitude = waypoint.Altitude.HasValue && nextWaypoint.Altitude.HasValue
              ? (double?)((1 - t) * waypoint.Altitude.Value + t * nextWaypoint.Altitude.Value)
              : null;
            var heartRate = waypoint.HeartRate.HasValue && nextWaypoint.HeartRate.HasValue
              ? (double?)((1 - t) * waypoint.HeartRate.Value + t * nextWaypoint.HeartRate.Value)
              : null;
            var mapReadingState = (mapReadingIndex + 1) % 2 == 0 ? MapReadingState.StartReading : MapReadingState.EndReading;
            newRouteSegment.Waypoints.Add(new Waypoint(time, longLat, altitude, heartRate, mapReadingState, null, null));
            mapReadingIndex++;
          }
          waypointCount++;
        }
        newRouteSegments.Add(newRouteSegment);
      }
      return newRouteSegments;
    }

    #endregion

    #region Nested classes

    public class CutRouteData
    {
      private List<RouteSegment> segments = new List<RouteSegment>();

      public List<RouteSegment> Segments
      {
        get { return segments; }
        set { segments = value; }
      }

      public CutType CutType { get; set; }
      public Waypoint WaypointInsertedAtCut { get; set; }
    }

    private class TimedAttribute
    {
      public DateTime Time { get; set; }
      public double? Value { get; set; }
    }

    #endregion

    public void AddTimeOffset(TimeSpan offset)
    {
      foreach (RouteSegment segment in Segments)
      {
        foreach (Waypoint waypoint in segment.Waypoints)
        {
          waypoint.Time = waypoint.Time.Add(offset);
        }
      }
    }

    /// <summary>
    /// Returns the parameterized location of the start of the corresponding lap, given a parameterized location.
    /// </summary>
    /// <param name="pl">The parameterized location that defines the lap.</param>
    /// <returns></returns>
    public ParameterizedLocation GetLapStartParameterizedLocation(ParameterizedLocation pl)
    {
      var currentTime = GetTimeFromParameterizedLocation(pl);
      var lastLapTime = lapTimes[0];
      foreach (var lapTime in lapTimes)
      {
        if (lapTime > lastLapTime && lapTime < currentTime)
        {
          lastLapTime = lapTime;
        }
      }
      return GetParameterizedLocationFromTime(lastLapTime, pl, ParameterizedLocation.Direction.Backward);
    }

    /// <summary>
    /// Returns the parameterized location of the end of the corresponding lap, given a parameterized location.
    /// </summary>
    /// <param name="pl">The parameterized location that defines the lap.</param>
    /// <returns></returns>
    public ParameterizedLocation GetLapEndParameterizedLocation(ParameterizedLocation pl)
    {
      var currentTime = GetTimeFromParameterizedLocation(pl);
      var nextLapTime = lapTimes[lapTimes.Count - 1];
      foreach (var lapTime in lapTimes)
      {
        if (lapTime < nextLapTime && lapTime >= currentTime)
        {
          nextLapTime = lapTime;
        }
      }
      return GetParameterizedLocationFromTime(nextLapTime, pl, ParameterizedLocation.Direction.Forward);
    }
  }

  [Serializable]
  public class RouteSegment
  {
    private List<Waypoint> waypoints = new List<Waypoint>();

    public List<Waypoint> Waypoints
    {
      get { return waypoints; }
      set { waypoints = value; }
    }

    public Waypoint FirstWaypoint
    {
      get { return waypoints.Count == 0 ? null : waypoints[0]; }
    }

    public Waypoint LastWaypoint
    {
      get { return waypoints.Count == 0 ? null : waypoints[waypoints.Count - 1]; }
    }
  }

  [Serializable]
  public class Waypoint
  {
    // core data of a waypoint
    private double? altitude;
    private double? heartRate;
    private LongLat longLat;
    private DateTime time;
    private MapReadingState? mapReadingState;
    private double? cadence;
    private double? power;
    // dictionary for storing calculated (possibly smoothed) attribute values (speed, altitude, etc) of this waypoint.
    [NonSerialized]
    private Dictionary<WaypointAttribute, double?> attributes;

    public Waypoint()
    {
      attributes = new Dictionary<WaypointAttribute, double?>();
    }

    public Waypoint(DateTime time, LongLat longLat, double? altitude, double? heartRate, MapReadingState? mapReadingState, double? cadence, double? power)
      : this()
    {
      this.time = time;
      this.longLat = longLat;
      this.altitude = altitude;
      this.heartRate = heartRate;
      this.mapReadingState = mapReadingState;
      this.cadence = cadence;
      this.power = power;
    }

    public DateTime Time
    {
      get { return time; }
      set { time = value; }
    }

    public LongLat LongLat
    {
      get { return longLat; }
      set { longLat = value; }
    }

    public double? Altitude
    {
      get { return altitude; }
      set { altitude = value; }
    }

    public double? HeartRate
    {
      get { return heartRate; }
      set { heartRate = value; }
    }

    public MapReadingState? MapReadingState
    {
      get { return mapReadingState; }
      set { mapReadingState = value; }
    }

    public double? Cadence
    {
      get { return cadence; }
      set { cadence = value; }
    }

    public double? Power
    {
      get { return power; }
      set { power = value; }
    }

    public Waypoint Clone()
    {
      var newWaypoint = new Waypoint(time, longLat, altitude, heartRate, mapReadingState, cadence, power);
      foreach (var attributeKey in Attributes.Keys)
      {
        newWaypoint.Attributes[attributeKey] = Attributes[attributeKey];
      }
      return newWaypoint;
    }

    public Dictionary<WaypointAttribute, double?> Attributes
    {
      get
      {
        // need to ensure that this is not null due to backward compatibility
        if (attributes == null) attributes = new Dictionary<WaypointAttribute, double?>();
        return attributes;
      }
    }

    /// <summary>
    /// Returns the original heart rate or altitude.
    /// </summary>
    /// <param name="attribute">Must be WaypointAttribute.HeartRate or WaypointAttribute.Altitude</param>
    /// <returns></returns>
    public double? GetOriginalAttribute(WaypointAttribute attribute)
    {
      if (attribute != WaypointAttribute.HeartRate && attribute != WaypointAttribute.Altitude) throw new ArgumentException("The 'attribute' parameter must be either WaypointAttribute.HeartRate or WaypointAttribute.Altitude.");
      switch (attribute)
      {
        case WaypointAttribute.HeartRate:
          return HeartRate;
        case WaypointAttribute.Altitude:
          return Altitude;
      }
      return null;
    }

  }
}