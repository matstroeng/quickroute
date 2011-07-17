using System;


namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  [Serializable]
  public class D_Virtual_partner
  {
    public uint Time { get; set; }

    public float Distance { get; set; }
  }
}
