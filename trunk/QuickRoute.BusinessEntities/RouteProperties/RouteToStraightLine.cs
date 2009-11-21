using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class RouteToStraightLine : RouteSpanProperty
  {
    public RouteToStraightLine(Session session, RouteLocations locations) : base(session, locations)
    {
    }

    public RouteToStraightLine(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
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
      var routeLength = (double)(new RouteDistance(Session, Start, End).Value);
      var straightLineLength = (double)(new StraightLineDistance(Session, Start, End).Value);
      value = straightLineLength != 0 ? (routeLength-straightLineLength)/straightLineLength : 0; 
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(RouteToStraightLineFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      return Convert.ToDouble(v).ToString("p1");
    }

    public override string MaxWidthString
    {
      get { return ValueToString(9.999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

  public class RouteToStraightLineFromStart : RouteFromStartProperty
  {
    public RouteToStraightLineFromStart(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    public RouteToStraightLineFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
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
      var routeLength = (double)(new RouteDistanceFromStart(Session, Location).Value);
      var straightLineLength = (double)(new StraightLineDistanceFromStart(Session, Location).Value);
      value = straightLineLength != 0 ? (routeLength - straightLineLength) / straightLineLength : 0;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      return Convert.ToDouble(v).ToString("p1");
    }

    public override string MaxWidthString
    {
      get { return ValueToString(9.999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }

}
