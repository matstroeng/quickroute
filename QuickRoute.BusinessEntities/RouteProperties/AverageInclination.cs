using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageInclination : RouteSpanProperty
  {
    public AverageInclination(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
    }

    public AverageInclination(Session session, RouteLocations locations)
      : base(session, locations)
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
      var routeLength = (double)new RouteDistance(Session, Start, End).Value;
      var altitudeDifference = (double?)new AltitudeDifference(Session, Start, End).Value;
      if(altitudeDifference.HasValue)
      {
        value = LinearAlgebraUtil.ToDegrees(Math.Atan2(altitudeDifference.Value, routeLength));
      }
      else
      {
        value = null;
      }
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageInclinationFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n1}";
      var d = ((double?)v);
      var s = d.HasValue ? string.Format(provider, format, d.Value) : "-";
      if (d.HasValue && provider == null) s = (d.Value > 0 ? "+" : "") + s;
      return s;
    }

    public override string MaxWidthString
    {
      get { return ValueToString(99.9); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.Altitude); }
    }
  }

  public class AverageInclinationFromStart : RouteFromStartProperty
  {
    public AverageInclinationFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    public AverageInclinationFromStart(Session session, RouteLocations locations)
      : base(session, locations)
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
      value = (new AverageInclination(Session, ParameterizedLocation.Start, Location) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n1}";
      var d = ((double?)v);
      var s = d.HasValue ? string.Format(provider, format, d.Value) : "-";
      if (d.HasValue && provider == null) s = (d.Value > 0 ? "+" : "") + s;
      return s;
    }

    public override string MaxWidthString
    {
      get { return ValueToString(99.9); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.Altitude); }
    }
  }

}
