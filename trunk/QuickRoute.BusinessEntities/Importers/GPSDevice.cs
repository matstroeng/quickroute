using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.Importers
{
  public class GPSDevice
  {
    private IGPSDeviceImporter importer;

    public GPSDevice(IGPSDeviceImporter importer)
    {
      this.importer = importer;
    }

    public IGPSDeviceImporter Importer
    {
      get { return importer; }
      set { importer = value; }
    }

    public override string ToString()
    {
      if (importer == null)
      {
        return base.ToString();
      }
      if (!importer.IsConnected && importer.CachedDataExists)
      {
        return string.Format("{0} ({1})", importer.DeviceName, Strings.NotConnected);
      }
      return importer.DeviceName;
    }
  }
}