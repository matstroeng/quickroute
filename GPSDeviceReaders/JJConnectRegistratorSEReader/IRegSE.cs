using System;

namespace QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader
{
    public interface IRegSEDeviceInfo
    {
        string DeviceName { get; }
        string Name { get; }
        string SoftwareVersion { get; }
        string HardwareVersion { get; }
        int TrackPointsCount { get; }
    }

    public interface IRegSETrackInfo
    {
        DateTime Date { get; }
        int Duration { get; }
        Int32 NumbetOfTrackPoints { get; }
    }

    public interface IRegSETrack 
    {
        IRegSETrackPoint[] GetTrackPoints();
        IRegSETrackInfo GetTrackInfo();
    }

    public enum RegSEPointType
    {
        TrackPoint = 0,
        TrackStart = 1,
        WayPoint = 2,
        OverSpeed = 4
    }

    public interface IRegSETrackPoint
    {
        DateTime Time { get; }
        Decimal Latitude { get; }
        Decimal Longitude { get; }
        int Altitude { get; }
        Int16 Type { get; }
        bool HasMark(RegSEPointType type);
    }
}
