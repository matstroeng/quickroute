using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class RouteDistance : RouteSpanProperty
  {
    public RouteDistance(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public RouteDistance(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
    public RouteDistanceFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public RouteDistanceFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new RouteDistance(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
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
