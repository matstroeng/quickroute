using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class Ascent : RouteSpanProperty
  {
    public Ascent(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public Ascent(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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

      var sum = 0.0;
      var pl = new ParameterizedLocation(Start);
      double? previousAltitude = 0;

      // try to get value of close location from cache
      if(HasCache)
      {
        var closestProperty = CacheManager.GetLastAdded(GetType(), Locations);
        if(closestProperty != null)
        {
          if(closestProperty.Value != null) sum = (double)closestProperty.Value;
          pl =
            new ParameterizedLocation(closestProperty.Locations.IsSpan
                                        ? closestProperty.Locations.End
                                        : closestProperty.Locations.Location);
          previousAltitude = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Altitude, pl);
          pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        }
      }

      while (pl != null && pl <= End)
      {
        var altitude = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Altitude, pl);
        if (pl != Start && !Session.Route.IsFirstPLInSegment(pl))
        {
          if (altitude.HasValue)
          {
            if (altitude > previousAltitude) sum += altitude.Value - previousAltitude.Value;
          }
          else
          {
            value = null;
            return;
          }
        }
        previousAltitude = altitude;
        if (pl >= End) break;
        pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        if (pl > End) pl = new ParameterizedLocation(End);
      }
      value = (double?)sum;      
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AscentFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if(format == null) format = "{0:n0}";
      var d = ((double?)v);
      return d.HasValue ? string.Format(provider, format, d.Value) : "-";
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)9999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.Altitude); }
    }
  }

  public class AscentFromStart : RouteFromStartProperty
  {
    public AscentFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AscentFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new Ascent(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if(format == null) format = "{0:n0}";
      var d = ((double?)v);
      return d.HasValue ? string.Format(provider, format, d.Value) : "-";
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)9999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.Altitude); }
    }
  }
}
