using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class RouteDistance : RouteSpanProperty
  {
    public RouteDistance(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
    }

    public RouteDistance(Session session, RouteLocations locations)
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
      if (Start == ParameterizedLocation.Start)
      {
        value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, End).Value; // for faster calculation
      }
      else
      {
        value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, End).Value -
                Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, Start).Value;
      }
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(RouteDistanceFromStart);
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

  public class RouteDistanceFromStart : RouteFromStartProperty
  {
    public RouteDistanceFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    public RouteDistanceFromStart(Session session, RouteLocations locations)
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
      value = (new RouteDistance(Session, ParameterizedLocation.Start, Location) { CacheManager = CacheManager }).Value;
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
