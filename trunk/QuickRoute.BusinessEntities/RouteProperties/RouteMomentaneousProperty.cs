/*
 * FROM START OF SESSION TO CURRENT POSITION,
 * FROM START OF LAP TO CURRENT POSITION,
 * THIS LAP
 * (having start point and end point)
 * elapsed time
 * distance
 * average speed
 * average direction deviation
 * number of stops
 * 
 * 
 * MOMENTANEOUS VALUE (no dependency to start of session or any other point in time)
 * time of day
 * speed
 * heart rate
 * altitude
 * direction deviation
 * stop: yes or no
 * 
 */



using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public abstract class RouteMomentaneousProperty : RouteProperty
  {
    public ParameterizedLocation Location { get { return Locations.Location; } }

    protected RouteMomentaneousProperty(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    protected RouteMomentaneousProperty(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, new RouteLocations(location), retrieveExternalProperty)
    {
    }
  }
}


