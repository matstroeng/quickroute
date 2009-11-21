using System;
using System.Collections.Generic;
using System.Text;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{

  public class CircleTimeInSpan : RouteSpanProperty
  {
    public CircleTimeInSpan(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
      DistanceThreshold = 35; // TODO: set dynamically
    }

    public CircleTimeInSpan(Session session, RouteLocations locations)
      : base(session, locations)
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

      value = (TimeSpan)(new CircleTimeAtStartOfSpan(Session, Start, End).Value) +
              (TimeSpan)(new CircleTimeAtEndOfSpan(Session, Start, End).Value);

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
    public CircleTimeInSpanFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
      DistanceThreshold = 35; // TODO: set dynamically
    }

    public CircleTimeInSpanFromStart(Session session, RouteLocations locations)
      : base(session, locations)
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

      value = (TimeSpan)(new CircleTimeInSpan(Session, ParameterizedLocation.Start, Location).Value);

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
