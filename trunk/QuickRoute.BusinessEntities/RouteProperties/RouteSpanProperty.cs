using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public abstract class RouteSpanProperty : RouteProperty
  {
    public ParameterizedLocation Start { get { return Locations.Start; } }
    public ParameterizedLocation End { get { return Locations.End; } }

    protected RouteSpanProperty(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    protected RouteSpanProperty(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, new RouteLocations(start, end), retrieveExternalProperty)
    {
    }
    
    public abstract Type GetRouteFromStartPropertyType();
  }

}


