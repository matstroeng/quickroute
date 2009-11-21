using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class Inclination : RouteMomentaneousProperty
  {
    public Inclination(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    public Inclination(Session session, ParameterizedLocation location)
      : base(session, location)
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
      value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Inclination, Location);
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n1}";
      var d = ((double?)v);
      var s = d.HasValue ? string.Format(provider, format, d.Value) : "-";
      if (d.HasValue && provider == null) s = (d.Value > 0 ? "+" : "") + s;
      return s;
    }

    public override string MaxWidthString
    {
      get { return ValueToString(99.9); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.Altitude); }
    }
  }
}