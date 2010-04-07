using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageRouteSpeed : RouteSpanProperty
  {
    public AverageRouteSpeed(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AverageRouteSpeed(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      var elapsedTime = (TimeSpan)new ElapsedTime(Session, Start, End, RetrieveExternalProperty).Value;
      value = 3.6 * routeLength/elapsedTime.TotalSeconds;
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageRouteSpeedFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      // speed in km/h
      if (format == null) format = "{0:n1}";
      return string.Format(provider, format, Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(999.9); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class AverageRouteSpeedFromStart : RouteFromStartProperty
  {
    public AverageRouteSpeedFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AverageRouteSpeedFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new AverageRouteSpeed(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      // speed in km/h
      if (format == null) format = "{0:n1}";
      return string.Format(provider, format, Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(999.9); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

}
