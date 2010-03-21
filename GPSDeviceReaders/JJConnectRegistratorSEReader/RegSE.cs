using System;
using QuickRoute.GPSDeviceReaders.SerialPortDeviceReader;

namespace QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader
{
    public class RegSEDeviceInfo : IRegSEDeviceInfo
    {
        private readonly String _deviceName;
        private readonly String _name;
        private readonly string _softwareVersion;
        private readonly string _hardwareVersion;
        private readonly int _trackPointsCount;

        public RegSEDeviceInfo(String deviceName, String name, string softwareVersion, string hardwareVersion, int trackPointsCount)
        {
            _deviceName = deviceName;
            _trackPointsCount = trackPointsCount;
            _hardwareVersion = hardwareVersion;
            _softwareVersion = softwareVersion;
            _name = name;
        }

        public String DeviceName { get { return _deviceName; } }
        public String Name { get { return _name; } }
        public string SoftwareVersion { get { return _softwareVersion; } }
        public string HardwareVersion { get { return _hardwareVersion; } }
        public int TrackPointsCount { get { return _trackPointsCount; } }
    }

    public class RegSETrackPoint : IRegSETrackPoint
    {
        public RegSETrackPoint(DateTime time, Decimal latitude, Decimal longitude, int altitude, Int16 type)
        {
            _time = time;
            _latitude = latitude;
            _longitude = longitude;
            _altitude = altitude;
            _type = type;
        }
        private readonly Decimal _latitude;
        private readonly Decimal _longitude;
        private readonly int _altitude;
        private readonly DateTime _time;
        private readonly Int16 _type;

        public Decimal Latitude { get { return _latitude; } }
        public Decimal Longitude { get { return _longitude; } }
        public int Altitude { get { return _altitude; } }
        public Int16 Type { get { return _type; } }
        public DateTime Time { get { return _time; } }
        public bool HasMark(RegSEPointType type)
        {
            return (_type & (int) type) == 1;
        }


        public static RegSETrackPoint FromByteArray(byte[] buffer, int offset)
        {
            //* 16 Byte Blocke
            //* Byte 0-1: little endian 16bit, flags: 1=TRACK_START, 2=WAYPOINT,4=OVER_SPEED
            //* Byte 2-5: little endian 32bit: timestamp
            //* Byte 6-9: little endian 32bit: lattitude in Grad, wenn man es durch 10000000 dividiert
            //* Byte 10-13: little endian 32bit: longitude in Grad, wenn man es durch 10000000 dividiert
            //* Byte 14-15: little endian 16bit: altitude 

            var type = HexUtils.ToInt16LE(buffer, offset + 0);

            var timeCode = HexUtils.ToInt32LE(buffer, offset + 2);
            int sec = (((timeCode) >> 0) & 0x3F);
            int min = (((timeCode) >> 6) & 0x3F);
            int hours = (((timeCode) >> 12) & 0x1F);
            int day = (((timeCode) >> 17) & 0x1F);
            int month = (((timeCode) >> 22) & 0x0F);
            int year = (((timeCode) >> 26) & 0x3F);
            var time = new DateTime(2000 + year, month, day, hours, min, sec);
            
            var latitudeValue = HexUtils.ToInt32LE(buffer, offset + 6);
            var longitudeValue = HexUtils.ToInt32LE(buffer, offset + 10);
            var altitude = HexUtils.ToInt16LE(buffer, offset + 14);
            var latitude = new Decimal(latitudeValue) / new Decimal(10000000);
            var longitude = new Decimal(longitudeValue) / new Decimal(10000000);

            return new RegSETrackPoint(time, latitude, longitude, altitude, type);
        }
    }

    public class RegSETrack : IRegSETrack
    {
        private readonly IRegSETrackPoint[] _points;
        private readonly IRegSETrackInfo _info;

        public RegSETrack(IRegSETrackInfo info, IRegSETrackPoint[] points)
        {
            _info = info;
            _points = points;
        }

        public IRegSETrackPoint[] GetTrackPoints()
        {
            if (_points == null)
            {
                return new IRegSETrackPoint[] { };
            }
            return _points;
        }

        public IRegSETrackInfo GetTrackInfo()
        {
            return _info;
        }

        public override String ToString()
        {
            return _info.ToString();
        }
    }

    public class RegSETrackInfo : IRegSETrackInfo
    {
        private DateTime _date;
        private readonly int _duration;
        private readonly Int32 _numberOfTrackPoints;

        public RegSETrackInfo(DateTime date, int duration, int trackPointsCount)
        {
            _date = date;
            _duration = duration;
            _numberOfTrackPoints = trackPointsCount;
        }

        public DateTime Date { get { return _date; } }
        public int Duration { get { return _duration; } }
        public Int32 NumbetOfTrackPoints { get { return _numberOfTrackPoints; } }

        public override String ToString()
        {
            return String.Format("{0}, {1} min", _date.ToString("s"), (_duration / 60.0).ToString("F2"));
        }
    }

}
