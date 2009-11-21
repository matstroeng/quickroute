using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D_Position_Type
    {
        private Int32 _lat;       //4 bytes 
        private Int32 _lon;       //4 bytes 

        public Int32 Latitude
        {
            set { _lat = value; }
            get { return _lat; }
        }
        public Int32 Longitude
        {
            set { _lon = value; }
            get { return _lon; }
        }
        public double LatitudeAsDegrees
        {
            get { return _lat * (180 / Math.Pow(2,31)); }
        }
        public double LongitudeAsDegrees
        {
            get { return _lon * (180 / Math.Pow(2, 31)); }
        }
    }
}
