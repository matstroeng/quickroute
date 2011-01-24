using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AveragePaceWhenNotReadingMap : RouteSpanProperty
  {
    public AveragePaceWhenNotReadingMap(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AveragePaceWhenNotReadingMap(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        value = null;
      }
      else
      {
        var pl = Locations.Start;
        var previousEndReadingTime = default(DateTime);
        double previousEndReadingDistance = 0;
        var totalTime = TimeSpan.Zero;
        var totalDistance = 0.0;
        while (true)
        {
          var w = pl.IsNode
                    ? Session.Route.Segments[pl.SegmentIndex].Waypoints[(int)pl.Value]
                    : Session.Route.CreateWaypointFromParameterizedLocation(pl);
          if (w.MapReadingState == BusinessEntities.MapReadingState.EndReading ||
              (w.MapReadingState == BusinessEntities.MapReadingState.NotReading && pl == Locations.Start))
          {
            previousEndReadingTime = w.Time;
            previousEndReadingDistance = (double)w.Attributes[WaypointAttribute.Distance];
          }
          if ((w.MapReadingState == BusinessEntities.MapReadingState.StartReading && pl != Locations.Start) ||
              (w.MapReadingState == BusinessEntities.MapReadingState.NotReading && pl == Locations.End))
          {
            totalTime += w.Time - previousEndReadingTime;
            totalDistance += (double)w.Attributes[WaypointAttribute.Distance] - previousEndReadingDistance;
          }
          if (pl >= Locations.End) break;
          pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
          if (pl > Locations.End) pl = new ParameterizedLocation(Locations.End);
        }
        value = totalDistance == 0 ? (TimeSpan?)null : TimeSpan.FromSeconds(Math.Min(86400, totalTime.TotalSeconds) /* to prevent overflow */ / (totalDistance / 1000));
      }

      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AveragePaceWhenNotReadingMapFromStart);
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


  public class AveragePaceWhenNotReadingMapFromStart : RouteFromStartProperty
  {
    public AveragePaceWhenNotReadingMapFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AveragePaceWhenNotReadingMapFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      value = (TimeSpan?)(new AveragePaceWhenNotReadingMap(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value);

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