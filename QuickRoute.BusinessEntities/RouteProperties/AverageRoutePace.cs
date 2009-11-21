using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageRoutePace : RouteSpanProperty
  {
    public AverageRoutePace(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
    }

    public AverageRoutePace(Session session, RouteLocations locations)
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
      value = ConvertUtil.ToPace((double)new AverageRouteSpeed(Session, Start, End).Value);
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageRoutePaceFromStart);
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
      get { return ValueToString(new TimeSpan(999,59,59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class AverageRoutePaceFromStart : RouteFromStartProperty
  {
    public AverageRoutePaceFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    public AverageRoutePaceFromStart(Session session, RouteLocations locations)
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
      value = (new AverageRoutePace(Session, ParameterizedLocation.Start, Location) { CacheManager = CacheManager }).Value;
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
      get { return ValueToString(new TimeSpan(999, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

}
