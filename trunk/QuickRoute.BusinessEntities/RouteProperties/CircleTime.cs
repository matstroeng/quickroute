using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class CircleTime : RouteMomentaneousProperty
  {
    public CircleTime(Session session, RouteLocations locations)
      : base(session, locations)
    {
      DistanceThreshold = 35; // TODO: set dynamically
    }

    public CircleTime(Session session, ParameterizedLocation location)
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

      value = (TimeSpan)(new CircleTimeBackward(Session, Location).Value) +
              (TimeSpan)(new CircleTimeForward(Session, Location).Value);

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
