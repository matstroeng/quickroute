using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class Speed : RouteMomentaneousProperty
  {
    public Speed(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    public Speed(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Speed, Location);
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      // speed in km/h
      if (format == null) format = "{0:n1}";
      return string.Format(provider, format, 3.6 * Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(999.9 / 3.6); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}