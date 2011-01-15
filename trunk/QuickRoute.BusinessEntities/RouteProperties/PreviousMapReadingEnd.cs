using System;
using QuickRoute.BusinessEntities.Numeric;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class PreviousMapReadingEnd : RouteMomentaneousProperty
  {
    public PreviousMapReadingEnd(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    public PreviousMapReadingEnd(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
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
        value = null;
      }
      else
      {
        var previousMapReadingEnd = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.PreviousMapReadingEnd, Location);
        value = previousMapReadingEnd == null ? null : (TimeSpan?)TimeSpan.FromSeconds(previousMapReadingEnd.Value);
      }
      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      if ((TimeSpan?)Value == null && (TimeSpan?)((RouteProperty)obj).Value == null) return 0;
      if ((TimeSpan?)Value != null && (TimeSpan?)((RouteProperty)obj).Value == null) return -1;
      if ((TimeSpan?)Value == null && (TimeSpan?)((RouteProperty)obj).Value != null) return 1;
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (v == null) return "-";
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
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }
}