using System;
using QuickRoute.BusinessEntities.Numeric;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class Location : RouteMomentaneousProperty
  {
    public Location(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    public Location(Session session, ParameterizedLocation location)
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
      value = Session.Route.GetLocationFromParameterizedLocation(Location);
      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return 0; // can't compare coordinates
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (provider == null)
      {
        var formatter = new LongLatFormatter(Strings.NorthAbbreviated, Strings.SouthAbbreviated, Strings.EastAbbreviated,
                                             Strings.WestAbbreviated, true, 0);
        return ((LongLat) v).ToString(formatter);
      }
      return string.Format(provider, format ?? "{0}", (LongLat)v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new LongLat(179 + 59.0 / 60 + 59.0 / 3600, 89 + 59.0 / 60 + 59.0 / 3600)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}