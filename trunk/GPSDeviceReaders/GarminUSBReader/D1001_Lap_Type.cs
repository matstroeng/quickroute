using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D1001_Lap_Type
    {
        private UInt32 _index;
        private UInt32 _start_time;
        private UInt32 _total_time;
        private float _total_dist;
        private float _max_speed;
        private D_Position_Type _begin;
        private D_Position_Type _end;
        private UInt16 _calories;
        private byte _avg_heart_rate;
        private byte _max_heart_rate;
        private byte _intensity;

        public UInt32 Index
        {
            set { _index = value; }
            get { return _index; }
        }
        public UInt32 StartTime
        {
            set { _start_time = value; }
            get { return _start_time; }
        }
        public DateTime StartTimeAsDateTime
        {
            get { return GarminDevice.GetDateTimeFromElapsedSeconds(_start_time); }
        }
        public UInt32 TotalTime
        {
            set { _total_time = value; }
            get { return _total_time; }
        }
        public float TotalDist
        {
            set { _total_dist = value; }
            get { return _total_dist; }
        }
        public float MaxSpeed
        {
            set { _max_speed = value; }
            get { return _max_speed; }
        }
        public D_Position_Type Begin
        {
            set { _begin = value; }
            get { return _begin; }
        }
        public D_Position_Type End
        {
            set { _end = value; }
            get { return _end; }
        }
        public UInt16 Calories
        {
            set { _calories = value; }
            get { return _calories; }
        }
        public byte AvgHeartRate
        {
            set { _avg_heart_rate = value; }
            get { return _avg_heart_rate; }
        }
        public byte MaxHeartRate
        {
            set { _max_heart_rate = value; }
            get { return _max_heart_rate; }
        }
        public byte Intensity
        {
            set { _intensity = value; }
            get { return _intensity; }
        }
    }
}
