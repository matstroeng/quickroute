using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D600_Date_Time_Type
    {
        private byte _Month;            /* month (1-12) */
        private byte _Day;              /* day (1-31) */
        private UInt16 _Year;           /* year (1990 means 1990) */
        private UInt16 _Hour;           /* hour (0-23) */
        private byte _Minute;           /* minute (0-59) */
        private byte _Second;           /* second (0-59) */

        public byte Month
        {
            set { _Month = value; }
            get { return _Month; }
        }
        public byte Day
        {
            set { _Day = value; }
            get { return _Day; }
        }
        public UInt16 Year
        {
            set { _Year = value; }
            get { return _Year; }
        }
        public UInt16 Hour
        {
            set { _Hour = value; }
            get { return _Hour; }
        }
        public byte Minute
        {
            set { _Minute = value; }
            get { return _Minute; }
        }
        public byte Second
        {
            set { _Second = value; }
            get { return _Second; }
        }
    }
}
