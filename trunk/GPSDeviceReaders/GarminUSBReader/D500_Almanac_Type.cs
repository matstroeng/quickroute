using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D500_Almanac_Type
    {
        private UInt16 _wn;           /* week number (weeks) */
        private float _toa;           /* almanac data reference time (s) */
        private float _af0;           /* clock correction coefficient (s) */
        private float _af1;           /* clock correction coefficient (s/s) */
        private float _e;             /* eccentricity (-) */
        private float _sqrta;         /* square root of semi-major axis (a)(m**1/2) */
        private float _m0;            /* mean anomaly at reference time (r) */
        private float _w;             /* argument of perigee (r) */
        private float _omg0;          /* right ascension (r) */
        private float _odot;          /* rate of right ascension (r/s) */
        private float _i;             /* inclination angle (r) */

        public UInt16 Wn
        {
            set { _wn = value; }
            get { return _wn; }
        }
        public float Toa
        {
            set { _toa = value; }
            get { return _toa; }
        }
        public float Af0
        {
            set { _af0 = value; }
            get { return _af0; }
        }
        public float Af1
        {
            set { _af1 = value; }
            get { return _af1; }
        }
        public float E
        {
            set { _e = value; }
            get { return _e; }
        }
        public float Sqrta
        {
            set { _sqrta = value; }
            get { return _sqrta; }
        }
        public float M0
        {
            set { _m0 = value; }
            get { return _m0; }
        }
        public float W
        {
            set { _w = value; }
            get { return _w; }
        }
        public float Omg0
        {
            set { _omg0 = value; }
            get { return _omg0; }
        }
        public float Odot
        {
            set { _odot = value; }
            get { return _odot; }
        }
        public float I
        {
            set { _i = value; }
            get { return _i; }
        }
    }
}
