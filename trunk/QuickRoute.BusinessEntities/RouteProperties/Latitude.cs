using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class Latitude : RouteMomentaneousProperty
  {
    public Latitude(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    public Latitude(Session session, ParameterizedLocation location)
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
      value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Latitude, Location);
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      return string.Format(provider, format ?? "{0}", v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(-99.999999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}