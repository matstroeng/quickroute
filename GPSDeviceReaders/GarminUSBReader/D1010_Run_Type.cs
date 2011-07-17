using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  [Serializable]
  public class D1010_Run_Type
  {
    public UInt32 TrackIndex { get; set; }

    public UInt32 FirstLapIndex { get; set; }

    public UInt32 LastLapIndex { get; set; }

    public Sport_Type SportType { get; set; }

    public Program_Type ProgramType { get; set; }

    public Multisport Multisport { get; set; }

    public byte Unused { get; set; }

    public D_Virtual_partner VirtualPartner { get; set; }

    public D1002_Workout_Type Workout { get; set; }
  }
}
