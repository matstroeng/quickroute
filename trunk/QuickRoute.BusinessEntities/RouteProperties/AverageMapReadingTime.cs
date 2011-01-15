using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageMapReadingTime : RouteSpanProperty
  {
    public AverageMapReadingTime(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AverageMapReadingTime(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      if (!Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration))
      {
        value = TimeSpan.Zero;
      }
      else
      {
        var mapReadingDuration = (TimeSpan)new MapReadingDurationInSpan(Session, Locations, RetrieveExternalProperty).Value;
        var numberOfMapReadings = (int)new NumberOfMapReadings(Session, Locations, RetrieveExternalProperty).Value;
        value = numberOfMapReadings == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(mapReadingDuration.TotalSeconds / numberOfMapReadings);
      }

      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageMapReadingTimeFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if ((TimeSpan)v == TimeSpan.Zero) return "-";
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime) {NoOfDecimals = 1};
        return tc.ToString((TimeSpan)v);
      }
      return string.Format(provider, format ?? "{0}", (TimeSpan)v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new TimeSpan(0, 9, 59)); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }


  public class AverageMapReadingTimeFromStart : RouteFromStartProperty
  {
    public AverageMapReadingTimeFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AverageMapReadingTimeFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      value = (TimeSpan)(new AverageMapReadingTime(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value);

      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((int)Value).CompareTo((int)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if ((TimeSpan)v == TimeSpan.Zero) return "-";
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime) { NoOfDecimals = 1 };
        return tc.ToString((TimeSpan)v);
      }
      return string.Format(provider, format ?? "{0}", (TimeSpan)v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new TimeSpan(0, 9, 59)); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }

}