using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D_Steps
    {
        private char[] _custom_name = new char[16]; /* Null-terminated step name */
        private float _target_custom_zone_low;      /* See below */
        private float _target_custom_zone_high;     /* See below */
        private UInt16 _duration_value;             /* See below */
        private byte _intensity;                    /* Same as D1001 */
        private byte _duration_type;                /* See below */
        private byte _target_type;                  /* See below */
        private byte _target_value;                 /* See below */
        private UInt16 _unused;                     /* Unused. Set to 0. */

        public char[] CustomName
        {
            set { _custom_name = value; }
            get { return _custom_name; }
        }
        public float TargetCustomZoneLow
        {
            set { _target_custom_zone_low = value; }
            get { return _target_custom_zone_low; }
        }
        public float TargetCustomZoneHigh
        {
            set { _target_custom_zone_high = value; }
            get { return _target_custom_zone_high; }
        }
        public UInt16 DurationValue
        {
            set { _duration_value = value; }
            get { return _duration_value; }
        }
        public byte Intensity
        {
            set { _intensity = value; }
            get { return _intensity; }
        }
        public byte DurationType
        {
            set { _duration_type = value; }
            get { return _duration_type; }
        }
        public byte TargetType
        {
            set { _target_type = value; }
            get { return _target_type; }
        }
        public byte TargetValue
        {
            set { _target_value = value; }
            get { return _target_value; }
        }
        public UInt16 Unused
        {
            set { _unused = value; }
            get { return _unused; }
        }
    }
}
