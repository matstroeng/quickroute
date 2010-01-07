using System;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AltitudeDifference : RouteSpanProperty
  {
    public AltitudeDifference(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AltitudeDifference(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      double? sum = 0;
      for (var i = Start.SegmentIndex; i <= End.SegmentIndex; i++)
      {
        var startPL = new ParameterizedLocation(i, 0);
        var endPL = new ParameterizedLocation(i, Session.Route.Segments[i].Waypoints.Count - 1);
        if (startPL < Start) startPL = Start;
        if (endPL > End) endPL = End;
        var startAltitude = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Altitude, startPL);
        var endAltitude = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Altitude, endPL);
        if (startAltitude == null || endAltitude == null)
        {
          sum = null;
          break;
        }
        sum += endAltitude.Value - startAltitude.Value;
      }
      value = sum;
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AltitudeDifferenceFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      var d = ((double?)v);
      var s = d.HasValue ? string.Format(provider, format, d.Value) : "-";
      if (d.HasValue && provider == null) s = (d.Value > 0 ? "+" : "") + s;
      return s;
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

  public class AltitudeDifferenceFromStart : RouteFromStartProperty
  {
    public AltitudeDifferenceFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AltitudeDifferenceFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new AltitudeDifference(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      var d = ((double?)v);
      var s = d.HasValue ? string.Format(provider, format, d.Value) : "-";
      if (d.HasValue && provider == null) s = (d.Value > 0 ? "+" : "") + s;
      return s;
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
