using System;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class MapReadingState : RouteMomentaneousProperty
  {
    public MapReadingState(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    public MapReadingState(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        value = false;
      }
      else
      {
        value = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.MapReadingState, Location) == 1;
      }
      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return (Convert.ToDouble(Value)).CompareTo(Convert.ToDouble(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (!ContainsValue) return "-";
      return Convert.ToDouble(v) == 0 ? Strings.No : Strings.Yes;
    }

    public override string MaxWidthString
    {
      get { return Strings.No.Length > Strings.Yes.Length ? Strings.No : Strings.Yes; }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }
}