using System;
using QuickRoute.BusinessEntities.Numeric;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class MapReadingDuration : RouteMomentaneousProperty
  {
    public MapReadingDuration(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    public MapReadingDuration(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        value = TimeSpan.Zero;
      }
      else
      {
        var w = Location.IsNode
                  ? Session.Route.Segments[Location.SegmentIndex].Waypoints[(int)Location.Value]
                  : Session.Route.CreateWaypointFromParameterizedLocation(Location);
        value = TimeSpan.FromSeconds(w.Attributes[WaypointAttribute.MapReadingDuration] ?? 0);
      }
      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return (Convert.ToDouble(Value)).CompareTo(Convert.ToDouble(((RouteProperty)obj).Value));
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
      get { return Strings.No.Length > Strings.Yes.Length ? Strings.No : Strings.Yes; }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }

  public class MapReadingDurationInSpan : RouteSpanProperty
  {
    public MapReadingDurationInSpan(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public MapReadingDurationInSpan(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        var pl = Locations.Start;
        DateTime previousStartReadingTime = default(DateTime);
        var totalTime = TimeSpan.Zero;
        while (true)
        {
          var w = pl.IsNode
                    ? Session.Route.Segments[pl.SegmentIndex].Waypoints[(int)pl.Value]
                    : Session.Route.CreateWaypointFromParameterizedLocation(pl);
          if (w.MapReadingState == BusinessEntities.MapReadingState.StartReading ||
              (w.MapReadingState == BusinessEntities.MapReadingState.Reading && pl == Locations.Start))
          {
            previousStartReadingTime = w.Time;
          }
          if ((w.MapReadingState == BusinessEntities.MapReadingState.EndReading && pl != Locations.Start) ||
              (w.MapReadingState == BusinessEntities.MapReadingState.Reading && pl == Locations.End))
          {
            totalTime += w.Time - previousStartReadingTime;
          }
          if (pl >= Locations.End) break;
          pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
          if (pl > Locations.End) pl = new ParameterizedLocation(Locations.End);
        }
        value = totalTime;
      }

      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(MapReadingDurationFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (!ContainsValue) return "-";
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
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

  public class MapReadingDurationFromStart : RouteFromStartProperty
  {
    public MapReadingDurationFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public MapReadingDurationFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      value = (TimeSpan)(new MapReadingDurationInSpan(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value);

      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (!ContainsValue) return "-"; 
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
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