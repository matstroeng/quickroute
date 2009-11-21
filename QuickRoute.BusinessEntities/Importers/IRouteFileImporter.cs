using QuickRoute.BusinessEntities.Importers;

namespace QuickRoute.BusinessEntities.Importers
{
  public interface IRouteFileImporter : IRouteImporter
  {
    string FileName { get; set; }
  }
}