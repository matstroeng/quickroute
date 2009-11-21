using System;


namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D_Virtual_partner
    {
        private UInt32 _time;       /* Time result of virtual partner */
        private float _distance;    /* Distance result of virtual partner */

        public UInt32 Time
        {
            set { _time = value; }
            get { return _time; }
        }
        public float Distance
        {
            set { _distance = value; }
            get { return _distance; }
        }
    }
}
