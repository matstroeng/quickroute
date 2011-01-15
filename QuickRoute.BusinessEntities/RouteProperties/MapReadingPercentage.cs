using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class MapReadingPercentage : RouteSpanProperty
  {
    public MapReadingPercentage(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public MapReadingPercentage(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        value = 0.0;
      }
      else
      {
        var mapReadingDuration = (TimeSpan)new MapReadingDurationInSpan(Session, Locations, RetrieveExternalProperty).Value;
        var totalDuration = (TimeSpan)new ElapsedTime(Session, Locations, RetrieveExternalProperty).Value;
        value = mapReadingDuration.TotalSeconds/totalDuration.TotalSeconds;
      }
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(MapReadingPercentageFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (!ContainsValue) return "-"; 
      return string.Format(provider, format ?? "{0:p1}", Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(1); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }

  public class MapReadingPercentageFromStart : RouteFromStartProperty
  {
    public MapReadingPercentageFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public MapReadingPercentageFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      value = (double)new MapReadingPercentage(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value;

      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (!ContainsValue) return "-";
      return string.Format(provider, format ?? "{0:p1}", Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(1); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }
}