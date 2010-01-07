using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class ElapsedTime : RouteSpanProperty
  {
    public ElapsedTime(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty) 
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public ElapsedTime(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      var sum = new TimeSpan();
      for (var i = Start.SegmentIndex; i <= End.SegmentIndex; i++)
      {
        var startPL = new ParameterizedLocation(i, 0);
        var endPL = new ParameterizedLocation(i, Session.Route.Segments[i].Waypoints.Count-1);
        if (startPL < Start) startPL = Start;
        if (endPL > End) endPL = End;
        sum += Session.Route.GetTimeFromParameterizedLocation(endPL) - Session.Route.GetTimeFromParameterizedLocation(startPL);
      }
      value = sum;
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(ElapsedTimeFromStart);
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
        return tc.ToString((TimeSpan)v);
      }
      return string.Format(provider, format ?? "{0}", (TimeSpan)v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new TimeSpan(29,59,59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class ElapsedTimeFromStart : RouteFromStartProperty
  {
    public ElapsedTimeFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public ElapsedTimeFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
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
      value = (new ElapsedTime(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((TimeSpan)Value).CompareTo((TimeSpan)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.ElapsedTime);
        return tc.ToString((TimeSpan) v);
      }
      return string.Format(provider, format ?? "{0}", (TimeSpan) v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new TimeSpan(29, 59, 59)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
