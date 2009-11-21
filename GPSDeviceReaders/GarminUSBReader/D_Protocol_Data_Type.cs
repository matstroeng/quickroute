using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D_Protocol_Data_Type
    {
        private byte _tag;
        private UInt16 _data;

        public byte Tag
        {
            set { _tag = value; }
            get { return _tag; }
        }
        public UInt16 Data
        {
            set { _data = value; }
            get { return _data; }
        }
    }
}
