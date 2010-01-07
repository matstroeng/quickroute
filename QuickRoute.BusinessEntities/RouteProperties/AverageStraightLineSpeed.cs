using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageStraightLineSpeed : RouteSpanProperty
  {
    public AverageStraightLineSpeed(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AverageStraightLineSpeed(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      var straightLineLength = (double)new StraightLineDistance(Session, Start, End, RetrieveExternalProperty).Value;
      var elapsedTime = (TimeSpan)new ElapsedTime(Session, Start, End, RetrieveExternalProperty).Value;
      value = straightLineLength / elapsedTime.TotalSeconds;
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageStraightLineSpeedFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      // speed in km/h
      if (format == null) format = "{0:n1}";
      return string.Format(provider, format, 3.6 * Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(999.9 / 3.6); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class AverageStraightLineSpeedFromStart : RouteFromStartProperty
  {
    public AverageStraightLineSpeedFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AverageStraightLineSpeedFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new AverageStraightLineSpeed(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      // speed in km/h
      if (format == null) format = "{0:n1}";
      return string.Format(provider, format, 3.6 * Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(999.9 / 3.6); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
