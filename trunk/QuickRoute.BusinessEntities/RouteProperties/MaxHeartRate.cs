using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class MaxHeartRate : RouteSpanProperty
  {
    public MaxHeartRate(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public MaxHeartRate(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      double? max = null;
      var pl = new ParameterizedLocation(Start);

      // try to get value of close location from cache
      if (HasCache)
      {
        var closestProperty = CacheManager.GetLastAdded(GetType(), Locations);
        if (closestProperty != null)
        {
          if (closestProperty.Value != null) max = (double)closestProperty.Value;
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
          if (!max.HasValue || hr.Value > max.Value) max = hr.Value;
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

      if(max.HasValue) value = max.Value; 
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(MaxHeartRateFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if(format == null) format = "{0:n0}";
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

  public class MaxHeartRateFromStart : RouteFromStartProperty
  {
    public MaxHeartRateFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public MaxHeartRateFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new MaxHeartRate(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
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
