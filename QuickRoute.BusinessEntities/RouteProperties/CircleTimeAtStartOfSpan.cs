using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class CircleTimeAtStartOfSpan : RouteSpanProperty
  {
    public CircleTimeAtStartOfSpan(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public CircleTimeAtStartOfSpan(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    private double? distanceThreshold;
    protected double DistanceThreshold
    {
      get
      {
        if (distanceThreshold == null && RetrieveExternalProperty != null) distanceThreshold = RetrieveExternalProperty("CircleTimeRadius");
        return distanceThreshold ?? 0;
      }
    }

    protected override void Calculate()
    {
      var cachedProperty = GetFromCache();
      if (cachedProperty != null)
      {
        value = cachedProperty.Value;
        return;
      }

      var currentLongLat = Session.Route.GetLocationFromParameterizedLocation(Start);
      var lastPL = new ParameterizedLocation(Start);
      ParameterizedLocation thisPL;
      ParameterizedLocation thresholdPL;
      double lastNodeDistance = 0;
      double thisNodeDistance = 0;
      while (true)
      {
        thisPL = Session.Route.GetNextPLNode(lastPL, ParameterizedLocation.Direction.Forward);
        if (thisPL == null || Session.Route.IsLastPLInSegment(thisPL) || thisPL > End) break;
        var longLat = Session.Route.GetLocationFromParameterizedLocation(thisPL);
        thisNodeDistance = LinearAlgebraUtil.DistancePointToPointLongLat(longLat, currentLongLat);
        if (thisNodeDistance > DistanceThreshold) break;
        lastPL = thisPL;
        lastNodeDistance = thisNodeDistance;
      }
      if (thisPL == null) thisPL = new ParameterizedLocation(Start.SegmentIndex, Session.Route.Segments[Start.SegmentIndex].Waypoints.Count - 1);
      if (Session.Route.IsLastPLInSegment(thisPL))
      {
        thresholdPL = thisPL;
      }
      else if (thisPL > End)
      {
        thresholdPL = new ParameterizedLocation(End);
      }
      else
      {
        var t = thisNodeDistance - lastNodeDistance == 0 ? 1 : (DistanceThreshold - lastNodeDistance) / (thisNodeDistance - lastNodeDistance);
        thresholdPL = new ParameterizedLocation(thisPL.SegmentIndex, lastPL.Value + t * (thisPL.Value - lastPL.Value));
      }
      value = Session.Route.GetTimeFromParameterizedLocation(thresholdPL) - Session.Route.GetTimeFromParameterizedLocation(Start);
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(CircleTimeAtStartOfSpanFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
        return tc.ToString((TimeSpan)v);
      }
      return string.Format(provider, format ?? "{0}", (TimeSpan)v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new TimeSpan(29, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class CircleTimeAtStartOfSpanFromStart : RouteFromStartProperty
  {
    public CircleTimeAtStartOfSpanFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public CircleTimeAtStartOfSpanFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    private double? distanceThreshold;
    protected double DistanceThreshold
    {
      get
      {
        if (distanceThreshold == null && RetrieveExternalProperty != null) distanceThreshold = RetrieveExternalProperty("CircleTimeRadius");
        return distanceThreshold ?? 0;
      }
    }

    protected override void Calculate()
    {
      var cachedProperty = GetFromCache();
      if (cachedProperty != null)
      {
        value = cachedProperty.Value;
        return;
      }

      var time = Session.Route.GetTimeFromParameterizedLocation(Location);
      var sum = new TimeSpan();
      for (var i = 1; i < Session.Laps.Count; i++)
      {
        if (Session.Laps[i].LapType != LapType.Start)
        {
          if (time >= Session.Laps[i].Time)
          {
            sum += (TimeSpan)new CircleTimeAtStartOfSpan(
              Session,
              Session.Route.GetParameterizedLocationFromTime(Session.Laps[i - 1].Time),
              Session.Route.GetParameterizedLocationFromTime(Session.Laps[i].Time),
              RetrieveExternalProperty).Value;
          }
        }
      }
      value = sum;

      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
        return tc.ToString((TimeSpan)v);
      }
      return string.Format(provider, format ?? "{0}", (TimeSpan)v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new TimeSpan(29, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
