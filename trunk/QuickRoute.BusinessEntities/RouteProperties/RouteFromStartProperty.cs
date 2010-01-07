namespace QuickRoute.BusinessEntities.RouteProperties
{
  public abstract class RouteFromStartProperty : RouteProperty
  {
    public ParameterizedLocation Location { get { return Locations.Location; } }

    protected RouteFromStartProperty(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    protected RouteFromStartProperty(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, new RouteLocations(location), retrieveExternalProperty)
    {
    }
  }
}


