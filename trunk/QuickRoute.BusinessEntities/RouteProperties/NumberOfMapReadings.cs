using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class NumberOfMapReadings : RouteSpanProperty
  {
    public NumberOfMapReadings(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public NumberOfMapReadings(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        value = 0;
      }
      else
      {
        var count = 0;
        var pl = Locations.Start;
        while (true)
        {
          var w = pl.IsNode
                    ? Session.Route.Segments[pl.SegmentIndex].Waypoints[(int)pl.Value]
                    : Session.Route.CreateWaypointFromParameterizedLocation(pl);
          if (w.MapReadingState == BusinessEntities.MapReadingState.StartReading ||
              (w.MapReadingState == BusinessEntities.MapReadingState.Reading && pl == Locations.Start))
          {
            count++;
          }
          if (pl >= Locations.End) break;
          pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
          if (pl > Locations.End) pl = new ParameterizedLocation(Locations.End);
        }
        value = count;
        
      }

      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(NumberOfMapReadingsFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((int)Value).CompareTo((int)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (!ContainsValue) return "-";
      return string.Format(provider, format ?? "{0}", v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(9999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }

  public class NumberOfMapReadingsFromStart : RouteFromStartProperty
  {
    public NumberOfMapReadingsFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public NumberOfMapReadingsFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      value = new NumberOfMapReadings(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value;

      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((int)Value).CompareTo((int)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (!ContainsValue) return "-";
      return string.Format(provider, format ?? "{0}", v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(9999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }
}