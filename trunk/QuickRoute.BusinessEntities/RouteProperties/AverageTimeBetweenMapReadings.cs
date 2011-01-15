using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageTimeBetweenMapReadings : RouteSpanProperty
  {
    public AverageTimeBetweenMapReadings(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AverageTimeBetweenMapReadings(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
        DateTime? previousEndReadingTime = null;
        var totalTime = TimeSpan.Zero;
        var count = 0;
        while(true)
        {
          if(pl.Value == 0) previousEndReadingTime = null; // reset when entering new segment
          var w = pl.IsNode
                    ? Session.Route.Segments[pl.SegmentIndex].Waypoints[(int) pl.Value]
                    : Session.Route.CreateWaypointFromParameterizedLocation(pl);
          if(w.MapReadingState == BusinessEntities.MapReadingState.EndReading)
          {
            previousEndReadingTime = w.Time;
          }
          if (w.MapReadingState == BusinessEntities.MapReadingState.StartReading && previousEndReadingTime.HasValue)
          {
            totalTime += w.Time - previousEndReadingTime.Value;
            count++;
          }
          if (pl >= Locations.End) break;
          pl++;
          if (pl > Locations.End) pl = new ParameterizedLocation(Locations.End);
        }
        value = new TimeSpan(count == 0 ? 0 : totalTime.Ticks / count);
      }

      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageTimeBetweenMapReadingsFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
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
      get { return ValueToString(new TimeSpan(0, 9, 59)); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }

  public class AverageTimeBetweenMapReadingsFromStart : RouteFromStartProperty
  {
    public AverageTimeBetweenMapReadingsFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AverageTimeBetweenMapReadingsFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      value = (TimeSpan)(new AverageTimeBetweenMapReadings(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty).Value);

      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((int)Value).CompareTo((int)(((RouteProperty)obj).Value));
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
      get { return ValueToString(new TimeSpan(0, 9, 59)); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.MapReadingDuration); }
    }
  }
}