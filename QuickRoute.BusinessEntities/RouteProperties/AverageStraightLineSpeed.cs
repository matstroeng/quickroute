using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageStraightLineSpeed : RouteSpanProperty
  {
    public AverageStraightLineSpeed(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
    }

    public AverageStraightLineSpeed(Session session, RouteLocations locations)
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
      var straightLineLength = (double)new StraightLineDistance(Session, Start, End).Value;
      var elapsedTime = (TimeSpan)new ElapsedTime(Session, Start, End).Value;
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
    public AverageStraightLineSpeedFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    public AverageStraightLineSpeedFromStart(Session session, RouteLocations locations)
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
      value = (new AverageStraightLineSpeed(Session, ParameterizedLocation.Start, Location) { CacheManager = CacheManager }).Value;
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
