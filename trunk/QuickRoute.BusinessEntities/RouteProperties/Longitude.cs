using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class Longitude : RouteMomentaneousProperty
  {
    public Longitude(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    public Longitude(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Longitude, Location);
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      return string.Format(provider, format ?? "{0}", v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(-999.999999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}