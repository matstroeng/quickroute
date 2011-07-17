using QuickRoute.BusinessEntities.Importers;

namespace QuickRoute.BusinessEntities.Importers
{
  public interface IGPSDeviceImporter : IRouteImporter
  {
    bool IsConnected { get; }
    bool CachedDataExists { get; }
    string DeviceName { get; }
    void Refresh();
  }
}