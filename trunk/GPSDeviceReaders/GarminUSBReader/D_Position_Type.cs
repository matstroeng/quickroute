using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  [Serializable]
  public class D_Position_Type
  {
    public int Latitude { get; set; }

    public int Longitude { get; set; }

    #region Derived properties
    
    public double LatitudeAsDegrees
    {
      get { return Latitude * (180 / Math.Pow(2, 31)); }
      set { throw new NotImplementedException(); }
    }

    public double LongitudeAsDegrees
    {
      get { return Longitude * (180 / Math.Pow(2, 31)); }
    }

    #endregion
  }
}
