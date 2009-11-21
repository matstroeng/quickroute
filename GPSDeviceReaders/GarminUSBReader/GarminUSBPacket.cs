using System;
using System.Runtime.InteropServices;
    
namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public struct GarminUSBPacket
    {
        // Byte Number  Byte Description    Notes
        // 0            Packet Type         USB Protocol Layer = 0, Application Layer = 20
        // 1-3          Reserved            Must be set to 0
        // 4-5          Packet ID
        // 6-7          Reserved            Must be set to 0
        // 8-11         Data Size
        // 12+          Data

        public byte type;
        public byte reserved1;
        public byte reserved2;
        public byte reserved3;
        public UInt16 id;
        public byte reserved4;
        public byte reserved5;
        public UInt32  size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] data;

        public GarminUSBPacket(byte[] buffer)
        {
            type = buffer[0];
            id = BitConverter.ToUInt16(buffer, 4);
            size = BitConverter.ToUInt32(buffer, 8);
            data = new byte[buffer.Length - 12];
            Array.Copy(buffer, 12, data, 0, buffer.Length - 12);
            reserved1 = 0;
            reserved2 = 0;
            reserved3 = 0;
            reserved4 = 0;
            reserved5 = 0;
        }
        public GarminUSBPacket(byte type, UInt16 id)
        {
            this.type = type;
            this.id = id;
            size = 0;
            data = null;
            reserved1 = 0;
            reserved2 = 0;
            reserved3 = 0;
            reserved4 = 0;
            reserved5 = 0;
        }
        public GarminUSBPacket(byte type, UInt16 id, UInt32 size, byte[] data)
        {
            this.type = type;
            this.id = id;
            this.size = size;
            this.data = data;
            reserved1 = 0;
            reserved2 = 0;
            reserved3 = 0;
            reserved4 = 0;
            reserved5 = 0;
        }

        public byte Type
        {
            set { type = value; }
            get { return type; }
        }
        public UInt16 Id
        {
            set { id = value; }
            get { return id; }
        }
        public UInt32 Size
        {
            set { size = value; }
            get { return size; }
        }
        public byte[] Data
        {
            set { data = value; }
            get { return data; }
        }
    }
}
