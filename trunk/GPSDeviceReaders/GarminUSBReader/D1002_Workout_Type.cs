using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  [Serializable]
  public class D1002_Workout_Type
  {
    public uint NumValidSteps { get; set; }

    public Sport_Type SportType { get; set; }
  }
}
