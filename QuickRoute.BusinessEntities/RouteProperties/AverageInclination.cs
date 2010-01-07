using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageInclination : RouteSpanProperty
  {
    public AverageInclination(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AverageInclination(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      var routeLength = (double)new RouteDistance(Session, Start, End, RetrieveExternalProperty).Value;
      var altitudeDifference = (double?)new AltitudeDifference(Session, Start, End, RetrieveExternalProperty).Value;
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
    public AverageInclinationFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AverageInclinationFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new AverageInclination(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
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
