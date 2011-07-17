using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public static class GarminUtil
  {
    public static DateTime GetDateTimeFromElapsedSeconds(uint secondsElapsed)
    {
      var d = new DateTime(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc);
      return d.AddSeconds(secondsElapsed);
    }
  }
}