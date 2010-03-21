using System;
using QuickRoute.GPSDeviceReaders.SerialPortDeviceReader;

namespace QuickRoute.GPSDeviceReaders.GlobalSatGH615MReader
{
    public class GH615MDeviceInfo : IGH615MDeviceInfo
    {
        private readonly String _deviceName;
        private readonly String _version;
        private readonly String _firmware;
        private readonly String _name;
        private readonly int _waypointsCount;
        private readonly int _tracksCount;
        private readonly int _routesCount;

        public GH615MDeviceInfo(String deviceName, String version, String firmware, String name, int waypointsCount, int tracksCount, int routesCount)
        {
            _deviceName = deviceName;
            _version = version;
            _firmware = firmware;
            _name = name;
            _waypointsCount = waypointsCount;
            _tracksCount = tracksCount;
            _routesCount = routesCount;
        }

        public String DeviceName { get { return _deviceName; } }
        public String Version { get { return _version; } }
        public String Firmware { get { return _firmware; } }
        public String Name { get { return _name; } }
        public int WayPointsCount { get { return _waypointsCount; } }
        public int TracksCount { get { return _tracksCount; } }
        public int RoutesCount { get { return _routesCount; } }
    }

    public class GH615MTrackPoint : IGH615MTrackPoint
    {
        public GH615MTrackPoint(Decimal latitude, Decimal longitude, int altitude, int speed, int pulse, DateTime time)
        {
            _latitude = latitude;
            _longitude = longitude;
            _altitude = altitude;
            _speed = speed;
            _pulse = pulse;
            _time = time;
        }
        private readonly Decimal _latitude;
        private readonly Decimal _longitude;
        private readonly int _altitude;
        private readonly int _speed;
        private readonly int _pulse;
        private readonly DateTime _time;

        public Decimal Latitude { get { return _latitude; } }
        public Decimal Longitude { get { return _longitude; } }
        public int Altitude { get { return _altitude; } }
        public int Speed { get { return _speed; } }
        public int Pulse { get { return _pulse; } }
        public DateTime Time { get { return _time; } }

        public static GH615MTrackPoint FromByteArray(byte[] buffer, int offset, DateTime timeOffset)
        {
            Int32 latitudeValue = HexUtils.ToInt32(buffer, offset + 0);
            Int32 longitudeValue = HexUtils.ToInt32(buffer, offset + 4);
            Int16 altitude = HexUtils.ToInt16(buffer, offset + 8);
            Int16 speed = HexUtils.ToInt16(buffer, offset + 10);
            byte pulse = buffer[offset + 12];
            int interval = HexUtils.ToInt16(buffer, offset + 13);

            Decimal latitude = new Decimal(latitudeValue) / new Decimal(1000000);
            if (latitudeValue < 0)
            {
                latitude = new Decimal(latitudeValue) / new Decimal(1000000) - new Decimal(4294.967295D);
            }

            Decimal longitude = new Decimal(longitudeValue) / new Decimal(1000000);
            if (longitudeValue < 0)
            {
                longitude = new Decimal(longitudeValue) / new Decimal(1000000) - new Decimal(4294.967295D);
            }
            DateTime time = timeOffset.Add(new TimeSpan(0, 0, 0, 0, 100 * interval));
            return new GH615MTrackPoint(latitude, longitude, altitude, speed, pulse, time);
        }
    }

    public class GH615MTrack : IGH615MTrack
    {
        private readonly IGH615MTrackPoint[] _points;
        private readonly IGH615MTrackInfo _info;

        public GH615MTrack(IGH615MTrackInfo info, IGH615MTrackPoint[] points)
        {
            _info = info;
            _points = points;
        }

        public IGH615MTrackPoint[] GetTrackPoints()
        {
            if (_points == null)
            {
                return new IGH615MTrackPoint[] { };
            }
            return _points;
        }

        public IGH615MTrackInfo GetTrackInfo()
        {
            return _info;
        }
    }

    public class GH615MTrackInfo : IGH615MTrackInfo
    {
        private DateTime _date;
        private readonly int _duration;
        private readonly int _distance;
        private readonly int _calories;
        private readonly int _topSpeed;
        private readonly int _maxPulse;
        private readonly int _avgPulse;
        private readonly Int32 _trackPointsCount;
        private readonly Int16 _id;

        public GH615MTrackInfo(DateTime date, int duration, int distance, int calories, int topSpeed, int maxPuls, int avgPuls, int trackPointsCount, Int16 id)
        {
            _date = date;
            _duration = duration;
            _distance = distance;
            _calories = calories;
            _topSpeed = topSpeed;
            _maxPulse = maxPuls;
            _avgPulse = avgPuls;
            _trackPointsCount = trackPointsCount;
            _id = id;
        }

        public DateTime Date { get { return _date; } }
        public int Duration { get { return _duration; } }
        public int Distance { get { return _distance; } }
        public int Calories { get { return _calories; } }
        public int TopSpeed { get { return _topSpeed; } }
        public int MaxPulse { get { return _maxPulse; } }
        public int AvgPulse { get { return _avgPulse; } }
        public Int16 Id { get { return _id; } }
        public Int32 TrackPointsCount { get { return _trackPointsCount; } }

        public static GH615MTrackInfo FromByteArray(byte[] buffer, int offset, Int16? id)
        {
            int year = 2000 + buffer[offset + 0];
            int month = buffer[offset + 1];
            int day = buffer[offset + 2];
            int hour = buffer[offset + 3];
            int minute = buffer[offset + 4];
            int second = buffer[offset + 5];

            var date = new DateTime(year, month, day, hour, minute, second);
            int duration = HexUtils.ToInt32(buffer, offset + 6) / 10;
            int distance = HexUtils.ToInt32(buffer, offset + 10);
            int calories = HexUtils.ToInt16(buffer, offset + 14);
            int topSpeed = HexUtils.ToInt16(buffer, offset + 16);
            int maxPuls = buffer[offset + 18];
            int avgPuls = buffer[offset + 19];
            int trackpointCount = HexUtils.ToInt16(buffer, offset + 20);
            if (id == null)
            {
                id = HexUtils.ToInt16(buffer, offset + 22);
            }

            return new GH615MTrackInfo(date, duration, distance, calories, topSpeed, maxPuls, avgPuls, trackpointCount, id.Value);
        }

        public override String ToString() { return String.Format("{0}, {1} m, {2} min", _date.ToString("s"), _distance, (_duration / 60.0).ToString("F2")); }
    }

}
