using QuickRoute.BusinessEntities.Importers;

namespace QuickRoute.BusinessEntities.Importers
{
  public interface IGPSDeviceImporter : IRouteImporter
  {
    bool IsConnected { get; }
    string DeviceName { get; }
    void Refresh();
  }
}