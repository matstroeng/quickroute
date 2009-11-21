namespace QuickRoute.BusinessEntities.RouteProperties
{
  public abstract class RouteFromStartProperty : RouteProperty
  {
    public ParameterizedLocation Location { get { return Locations.Location; } }

    protected RouteFromStartProperty(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    protected RouteFromStartProperty(Session session, ParameterizedLocation location)
      : base(session, new RouteLocations(location))
    {
    }
  }
}


