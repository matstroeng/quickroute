using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class CircleTimeBackward : RouteMomentaneousProperty
  {
    public CircleTimeBackward(Session session, RouteLocations locations)
      : base(session, locations)
    {
      DistanceThreshold = 35; // TODO: set dynamically
    }

    public CircleTimeBackward(Session session, ParameterizedLocation location)
      : base(session, location)
    {
      DistanceThreshold = 35; // TODO: set dynamically
    }

    protected double DistanceThreshold { get; set; }

    protected override void Calculate()
    {
      var cachedProperty = GetFromCache();
      if (cachedProperty != null)
      {
        value = cachedProperty.Value;
        return;
      }

      var currentLongLat = Session.Route.GetLocationFromParameterizedLocation(Location);
      var lastPL = new ParameterizedLocation(Location);
      ParameterizedLocation thisPL;
      ParameterizedLocation thresholdPL;
      double lastNodeDistance = 0;
      double thisNodeDistance = 0;
      while (true)
      {
        thisPL = Session.Route.GetNextPLNode(lastPL, ParameterizedLocation.Direction.Backward);
        if(thisPL == null || Session.Route.IsFirstPLInSegment(thisPL)) break;
        var longLat = Session.Route.GetLocationFromParameterizedLocation(thisPL);
        thisNodeDistance = LinearAlgebraUtil.DistancePointToPointLongLat(longLat, currentLongLat);
        if (thisNodeDistance > DistanceThreshold) break;
        lastPL = thisPL;
        lastNodeDistance = thisNodeDistance;
      }
      if (thisPL == null) thisPL = new ParameterizedLocation(Location.SegmentIndex, 0);
      if (Session.Route.IsFirstPLInSegment(thisPL))
      {
        thresholdPL = thisPL;
      }
      else
      {
        var t = thisNodeDistance - lastNodeDistance == 0 ? 1 : (DistanceThreshold - lastNodeDistance) / (thisNodeDistance - lastNodeDistance);
        thresholdPL = new ParameterizedLocation(thisPL.SegmentIndex, lastPL.Value - t * (lastPL.Value - thisPL.Value));
      }
      value = Session.Route.GetTimeFromParameterizedLocation(Location) - Session.Route.GetTimeFromParameterizedLocation(thresholdPL);      
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
      get { return ValueToString(new TimeSpan(23, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
