using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  [Serializable]
  public class D303_Trk_Point_Type
  {
    public D_Position_Type Position { get; set; }

    public uint Time { get; set; }

    public float Altitude { get; set; }

    public byte HeartRate { get; set; }

    public DateTime TimeAsDateTime
    {
      get { return GarminUtil.GetDateTimeFromElapsedSeconds(Time); }
    }
  }
}
