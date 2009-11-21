namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D700_Position_Type
    {
        private double _lat;       //8 bytes 
        private double _lon;       //8 bytes 

        public double Latitude
        {
            set { _lat = value; }
            get { return _lat; }
        }
        public double Longitude
        {
            set { _lon = value; }
            get { return _lon; }
        }
    }
}
