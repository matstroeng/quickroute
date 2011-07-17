namespace QuickRoute.BusinessEntities.Importers
{
  public class RouteFileFormat : FileFormat
  {
    private IRouteFileImporter importer;

    public RouteFileFormat(string fileFilter, IRouteFileImporter importer)
      : base(fileFilter)
    {
      this.importer = importer;
    }

    public IRouteFileImporter Importer
    {
      get { return importer; }
      set { importer = value; }
    }
  }
}