using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class CircleTime : RouteMomentaneousProperty
  {
    public CircleTime(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    public CircleTime(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    private double? distanceThreshold;
    protected double DistanceThreshold
    {
      get
      {
        if (distanceThreshold == null && RetrieveExternalProperty != null) distanceThreshold = RetrieveExternalProperty("CircleTimeRadius");
        return distanceThreshold ?? 0;
      }
    }

    protected override void Calculate()
    {
      var cachedProperty = GetFromCache();
      if (cachedProperty != null)
      {
        value = cachedProperty.Value;
        return;
      }

      value = (TimeSpan)(new CircleTimeBackward(Session, Location, RetrieveExternalProperty).Value) +
              (TimeSpan)(new CircleTimeForward(Session, Location, RetrieveExternalProperty).Value);

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
