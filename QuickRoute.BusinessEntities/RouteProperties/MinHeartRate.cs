using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class MinHeartRate : RouteSpanProperty
  {
    public MinHeartRate(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
    }

    public MinHeartRate(Session session, RouteLocations locations)
      : base(session, locations)
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

      double? min = null;
      var pl = new ParameterizedLocation(Start);

      // try to get value of close location from cache
      if (HasCache)
      {
        var closestProperty = CacheManager.GetLastAdded(GetType(), Locations);
        if (closestProperty != null)
        {
          if (closestProperty.Value != null) min = (double)closestProperty.Value;
          pl =
            new ParameterizedLocation(closestProperty.Locations.IsSpan
                                        ? closestProperty.Locations.End
                                        : closestProperty.Locations.Location);
          pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        }
      }
      
      while (pl <= End)
      {
        var hr = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.HeartRate, pl);
        if (hr.HasValue)
        {
          if (!min.HasValue || hr.Value < min.Value) min = hr.Value;
        }
        else
        {
          value = null;
            return;
        }
        if (pl >= End) break;
        pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        if (pl > End) pl = new ParameterizedLocation(End);
      }

      if (min.HasValue) value = min.Value; 
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(MinHeartRateFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      var d = ((double?)v);
      return d.HasValue ? string.Format(provider, format, d.Value) : "-";
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.HeartRate); }
    }
  }

  public class MinHeartRateFromStart : RouteFromStartProperty
  {
    public MinHeartRateFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    public MinHeartRateFromStart(Session session, RouteLocations locations)
      : base(session, locations)
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
      value = (new MinHeartRate(Session, ParameterizedLocation.Start, Location) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      var d = ((double?)v);
      return d.HasValue ? string.Format(provider, format, d.Value) : "-";
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.HeartRate); }
    }
  }

}
