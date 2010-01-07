using System;
using System.Collections.Generic;
using System.Text;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class CircleTimeInSpan : RouteSpanProperty
  {
    public CircleTimeInSpan(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public CircleTimeInSpan(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    private double? distanceThreshold;
    protected double DistanceThreshold 
    { 
      get
      {
        if(distanceThreshold == null && RetrieveExternalProperty != null) distanceThreshold = RetrieveExternalProperty("CircleTimeRadius");
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

      value = (TimeSpan)(new CircleTimeAtStartOfSpan(Session, Start, End, RetrieveExternalProperty).Value) +
              (TimeSpan)(new CircleTimeAtEndOfSpan(Session, Start, End, RetrieveExternalProperty).Value);

      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(CircleTimeInSpanFromStart);
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
      get { return ValueToString(new TimeSpan(29, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class CircleTimeInSpanFromStart : RouteFromStartProperty
  {
    public CircleTimeInSpanFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public CircleTimeInSpanFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
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

      value = (TimeSpan)(new CircleTimeInSpan(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value);

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
      get { return ValueToString(new TimeSpan(29, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
