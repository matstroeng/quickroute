using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  [Serializable]
  public class D1001_Lap_Type
  {
    public uint Index { get; set; }

    public uint StartTime { get; set; }

    public uint TotalTime { get; set; }

    public float TotalDist { get; set; }

    public float MaxSpeed { get; set; }

    public D_Position_Type Begin { get; set; }

    public D_Position_Type End { get; set; }

    public ushort Calories { get; set; }

    public byte AvgHeartRate { get; set; }

    public byte MaxHeartRate { get; set; }

    public byte Intensity { get; set; }

    #region Derived properties

    public DateTime StartTimeAsDateTime
    {
      get { return GarminUtil.GetDateTimeFromElapsedSeconds(StartTime); }
    }

    public UInt32 FinishTime
    {
      get { return StartTime + TotalTime/100; }
    }

    public DateTime FinishTimeAsDateTime
    {
      get { return StartTimeAsDateTime.AddSeconds((int)(TotalTime/100)); }
    }

    #endregion

  }
}
