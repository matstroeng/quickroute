using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AveragePaceWhenReadingMap : RouteSpanProperty
  {
    public AveragePaceWhenReadingMap(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AveragePaceWhenReadingMap(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        var previousStartReadingTime = default(DateTime);
        double previousStartReadingDistance = 0;
        var totalTime = TimeSpan.Zero;
        var totalDistance = 0.0;
        while (true)
        {
          var w = pl.IsNode
                    ? Session.Route.Segments[pl.SegmentIndex].Waypoints[(int)pl.Value]
                    : Session.Route.CreateWaypointFromParameterizedLocation(pl);
          if (w.MapReadingState == BusinessEntities.MapReadingState.StartReading ||
              (w.MapReadingState == BusinessEntities.MapReadingState.Reading && pl == Locations.Start))
          {
            previousStartReadingTime = w.Time;
            previousStartReadingDistance = (double)w.Attributes[WaypointAttribute.Distance];
          }
          if ((w.MapReadingState == BusinessEntities.MapReadingState.EndReading && pl != Locations.Start) ||
              (w.MapReadingState == BusinessEntities.MapReadingState.Reading && pl == Locations.End))
          {
            totalTime += w.Time - previousStartReadingTime;
            totalDistance += (double) w.Attributes[WaypointAttribute.Distance] - previousStartReadingDistance;
          }
          if (pl >= Locations.End) break;
          pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
          if (pl > Locations.End) pl = new ParameterizedLocation(Locations.End);
        }
        value = totalDistance == 0 ? (TimeSpan?)null : TimeSpan.FromSeconds(totalTime.TotalSeconds / ( totalDistance / 1000));
      }

      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AveragePaceWhenReadingMapFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan?)Value ?? TimeSpan.MaxValue).CompareTo((TimeSpan?)(((RouteProperty)obj).Value) ?? TimeSpan.MaxValue);
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
      get { return ValueToString(new TimeSpan(999, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }

  }


  public class AveragePaceWhenReadingMapFromStart : RouteFromStartProperty
  {
    public AveragePaceWhenReadingMapFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AveragePaceWhenReadingMapFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      value = (TimeSpan)(new AveragePaceWhenReadingMap(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value);

      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan?)Value ?? TimeSpan.MaxValue).CompareTo((TimeSpan?)(((RouteProperty)obj).Value) ?? TimeSpan.MaxValue);
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
      get { return ValueToString(new TimeSpan(999, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }

}