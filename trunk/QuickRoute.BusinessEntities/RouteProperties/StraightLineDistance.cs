using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  // TODO: project position in the middle of a lap down on straight line
  public class StraightLineDistance : RouteSpanProperty
  {
    public StraightLineDistance(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public StraightLineDistance(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    protected override void Calculate()
    {
      var cachedProperty = GetFromCache();
      if (cachedProperty != null)
      {
        value = cachedProperty.Value;
        return;
      }
      double sum = 0;
      var spanStartTime = Session.Route.GetTimeFromParameterizedLocation(Start);
      var spanEndTime = Session.Route.GetTimeFromParameterizedLocation(End);
      int lapIndex = 0;
      // examine each lap until end time
      while (Session.Route.LapTimes[lapIndex] < spanEndTime && lapIndex < Session.Route.LapTimes.Count - 1)
      {
        var thisLapStartPL = Session.Route.GetParameterizedLocationFromTime(Session.Route.LapTimes[lapIndex]);
        var thisLapEndPL = Session.Route.GetParameterizedLocationFromTime(Session.Route.LapTimes[lapIndex+1]);

        ParameterizedLocation startPL = null;
        // span starts somewhere in this lap?
        if (spanStartTime >= Session.Route.LapTimes[lapIndex] && spanStartTime < Session.Route.LapTimes[lapIndex+1])
        {
          startPL = Start;
        }
        // span starts before this lap?
        else if (spanStartTime < Session.Route.LapTimes[lapIndex])
        {
          startPL = thisLapStartPL;
        }

        ParameterizedLocation endPL = null;
        // span ends somewhere in this lap?
        if (spanEndTime >= Session.Route.LapTimes[lapIndex] && spanEndTime < Session.Route.LapTimes[lapIndex + 1])
        {
          endPL = End;
        }
        // span ends after this lap?
        else if (spanEndTime >= Session.Route.LapTimes[lapIndex+1])
        {
          endPL = thisLapEndPL;
        }

        if(startPL != null && endPL != null)
        {
          // this lap contains some of the desired distance to be measured
          for (var i = startPL.SegmentIndex; i <= endPL.SegmentIndex; i++)
          {
            var thisSegmentStartPL = new ParameterizedLocation(i, 0);
            var thisSegmentEndPL = new ParameterizedLocation(i, Session.Route.Segments[i].Waypoints.Count - 1);
            if (thisSegmentStartPL < startPL) thisSegmentStartPL = startPL;
            if (thisSegmentEndPL > endPL) thisSegmentEndPL = endPL;

            var thisLapStartLocation = Session.Route.GetLocationFromParameterizedLocation(thisLapStartPL);
            var thisLapEndLocation = Session.Route.GetLocationFromParameterizedLocation(thisLapEndPL);
            var projectionOrigin = thisLapStartLocation / 2 + thisLapEndLocation / 2;
            var startLocationPoint = Session.Route.GetLocationFromParameterizedLocation(startPL).Project(projectionOrigin);
            var endLocationPoint = Session.Route.GetLocationFromParameterizedLocation(endPL).Project(projectionOrigin);
            var thisLapStartLocationPoint = thisLapStartLocation.Project(projectionOrigin);
            var thisLapEndLocationPoint = thisLapEndLocation.Project(projectionOrigin);

            double t0;
            double t1;
            // get start and end point parameterized lcoations (t0 and t1) on straight line
            LinearAlgebraUtil.ClosestDistancePointToLine(startLocationPoint, thisLapStartLocationPoint, thisLapEndLocationPoint, out t0);
            LinearAlgebraUtil.ClosestDistancePointToLine(endLocationPoint, thisLapStartLocationPoint, thisLapEndLocationPoint, out t1);

            t0 = Math.Max(0, Math.Min(1, t0));
            t1 = Math.Max(0, Math.Min(1, t1));
            sum += (t1-t0) * LinearAlgebraUtil.DistancePointToPointLongLat(thisLapStartLocation, thisLapEndLocation);
          }
        }
        lapIndex++;
      }
      value = sum;
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(StraightLineDistanceFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      return string.Format(provider, format, Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(99999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class StraightLineDistanceFromStart : RouteFromStartProperty
  {
    public StraightLineDistanceFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public StraightLineDistanceFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    protected override void Calculate()
    {
      var cachedProperty = GetFromCache();
      if (cachedProperty != null)
      {
        value = cachedProperty.Value;
        return;
      }
      value = (new StraightLineDistance(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      return string.Format(provider, format, Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(99999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
