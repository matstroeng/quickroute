using System;

namespace QuickRoute.GPSDeviceReaders.GlobalSatGH615MReader
{
    public interface IGH615MDeviceInfo
    {
        string DeviceName { get; }
        string Firmware { get; }
        string Name { get; }
        int RoutesCount { get; }
        int TracksCount { get; }
        string Version { get; }
        int WayPointsCount { get; }
    }

    public interface IGH615MTrackInfo
    {
        Int16 Id { get; }
        DateTime Date { get; }
        int Duration { get; }
        int Distance { get; }
        int AvgPulse { get; }
        int MaxPulse { get; }
        int Calories { get; }
        int TopSpeed { get; }
        Int32 TrackPointsCount { get; }
    }

    public interface IGH615MTrack 
    {
        IGH615MTrackPoint[] GetTrackPoints();
        IGH615MTrackInfo GetTrackInfo();
    }

    public interface IGH615MTrackPoint
    {
        Decimal Latitude { get; }
        Decimal Longitude { get; }
        int Altitude { get; }
        int Speed { get; }
        int Pulse { get; }
        DateTime Time { get; }

    }

    public enum GH615MWayPointType
    {
        Dot = 0,
        House,
        Triangle,
        Tunnel,
        Cross,
        Fish,
        Light,
        Car,
        Comm,
        Redcross,
        Tree,
        Bus,
        Copcar,
        Trees,
        Restaurant,
        Seven,
        Parking,
        Repairs,
        Mail,
        Dollar,
        Govoffice,
        Church,
        Grocery,
        Heart,
        Book,
        Gas,
        Grill,
        Lookout,
        Flag,
        Plane,
        Bird,
        Dawn,
        Restroom,
        Wtf,
        Mantaray,
        Information,
        Blank
    }

    public interface IGH615MWayPoint
    {
        Decimal Latitude { get; }
        Decimal Longitude { get; }
        int Altitude { get; }
        GH615MWayPointType Type { get; }
        String Title{ get; }
    }
    
}
