using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public abstract class RouteSpanProperty : RouteProperty
  {
    public ParameterizedLocation Start { get { return Locations.Start; } }
    public ParameterizedLocation End { get { return Locations.End; } }

    protected RouteSpanProperty(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    protected RouteSpanProperty(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, new RouteLocations(start, end))
    {
    }
    
    public abstract Type GetRouteFromStartPropertyType();
  }

}


