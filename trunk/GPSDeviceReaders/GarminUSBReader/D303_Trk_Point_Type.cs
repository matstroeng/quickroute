using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D303_Trk_Point_Type
    {
        private D_Position_Type _posn;       /* position */
        private UInt32 _time;                /* time */
        private float _alt;                  /* altitude in meters */
        private byte _heart_rate;            /* heart rate in beats per minute */

        public D_Position_Type Position
        {
            set { _posn = value; }
            get { return _posn; }
        }
        public UInt32 Time
        {
            set { _time = value; }
            get { return _time; }
        }
        public DateTime TimeAsDateTime
        {
            get { return GarminDevice.GetDateTimeFromElapsedSeconds(_time); }
        }
        public float Altitude
        {
            set { _alt = value; }
            get { return _alt; }
        }
        public byte HeartRate
        {
            set { _heart_rate = value; }
            get { return _heart_rate; }
        }
    }
}
