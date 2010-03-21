using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using QuickRoute.GPSDeviceReaders.SerialPortDeviceReader;

namespace QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader
{

    class RegSEError : Exception
    {
        public RegSEError() { }
        public RegSEError(string message) : base(message) { }
        public RegSEError(string message, Exception e) : base(message, e) { }
    }

    class RegSECommunicationError : RegSEError
    {
        public RegSECommunicationError() { }
        public RegSECommunicationError(string message) : base(message) { }
        public RegSECommunicationError(string message, Exception e) : base(message, e) { }
    }

    public class CheckRegSEPortCommand : SerialPortCommand
    {
        private bool _status;

        private static readonly byte[] Command = ASCIIEnc.GetBytes("@AL\r\n@AL,2,3\r\n@AL,2,3\r\n");
        private static readonly byte[] Header = ASCIIEnc.GetBytes("@AL,LoginOK\n@AL,2,3\n");

        public CheckRegSEPortCommand()
            : base(DefaultSleepTime, Header.Length)
        {
        }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (CompareFirstN(buffer, Header, Header.Length))
            {
                _status = true;
            }
        }

        public bool IsOk()
        {
            return _status;
        }
    }

    public class DeviceNameCommand : SerialPortCommand
    {
        private String _deviceName;
        private static readonly byte[] Command = ASCIIEnc.GetBytes("@AL,7,1\r\n");
        private static readonly byte[] Header = ASCIIEnc.GetBytes("@AL,7,1,");

        public DeviceNameCommand() : base(500) { }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (!CompareFirstN(buffer, Header, Header.Length) || buffer.Length < Header.Length + 1)
            {
                throw new RegSECommunicationError("Wrong Header in NameCommand");
            }
            var zero = new String(new[] { '\0' });
            _deviceName = Encoding.UTF8.GetString(buffer, Header.Length, buffer.Length - (Header.Length + 1)).Replace(zero, "");
        }

        public String GetDeviceName()
        {
            return _deviceName;
        }
    }

    public class SoftwareVersionCommand : SerialPortCommand
    {
        private String _version;
        private static readonly byte[] Command = ASCIIEnc.GetBytes("@AL,8,2\r\n");
        private static readonly byte[] Header = ASCIIEnc.GetBytes("@AL,8,2,");

        public SoftwareVersionCommand() : base(500) { }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (!CompareFirstN(buffer, Header, Header.Length) || buffer.Length < Header.Length + 1)
            {
                throw new RegSECommunicationError("Wrong Header in HardwareVersionCommand");
            }
            var zero = new String(new[] { '\0' });
            _version = Encoding.UTF8.GetString(buffer, Header.Length, buffer.Length - (Header.Length + 1)).Replace(zero, "");
        }

        public String GetVersion()
        {
            return _version;
        }
    }

    public class HardwareVersionCommand : SerialPortCommand
    {
        private String _version;
        private static readonly byte[] Command = ASCIIEnc.GetBytes("@AL,8,1\r\n");
        private static readonly byte[] Header = ASCIIEnc.GetBytes("@AL,8,1,");

        public HardwareVersionCommand() : base(500) { }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (!CompareFirstN(buffer, Header, Header.Length) || buffer.Length < Header.Length + 1)
            {
                throw new RegSECommunicationError("Wrong Header in HardwareVersionCommand");
            }
            var zero = new String(new[] { '\0' });
            _version = Encoding.UTF8.GetString(buffer, Header.Length, buffer.Length - (Header.Length + 1)).Replace(zero, "");
        }

        public String GetVersion()
        {
            return _version;
        }
    }

    public class NameCommand : SerialPortCommand
    {
        private String _name;
        private static readonly byte[] Command = ASCIIEnc.GetBytes("@AL,7,2\r\n");
        private static readonly byte[] Header = ASCIIEnc.GetBytes("@AL,7,2,");

        public NameCommand() : base(500) { }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (!CompareFirstN(buffer, Header, Header.Length) || buffer.Length < Header.Length + 1)
            {
                throw new RegSECommunicationError("Wrong Header in NameCommand");
            }
            var zero = new String(new[] { '\0' });
            _name = Encoding.UTF8.GetString(buffer, Header.Length, buffer.Length - (Header.Length + 1)).Replace(zero, "");
        }

        public String GetName()
        {
            return _name;
        }
    }


    public class NumOfPointsCommand : SerialPortCommand
    {
        private int _numOfPoints;
        private static readonly byte[] Command = ASCIIEnc.GetBytes("@AL,5,2\n");
        private static readonly byte[] Header = ASCIIEnc.GetBytes("@AL,5,2,");
    
        public NumOfPointsCommand() : base(500) { }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (!CompareFirstN(buffer, Header, Header.Length) || buffer.Length < Header.Length + 1)
            {
                throw new RegSECommunicationError("Wrong Header in NumOfPointsCommand");
            }

            try
            {
                _numOfPoints = int.Parse((Encoding.UTF8.GetString(buffer, Header.Length, buffer.Length - (Header.Length + 1)))) / 16;
            }
            catch (Exception e)
            {
                throw new RegSECommunicationError("Wrong number of points in NumOfPointsCommand", e);
            }
        }

        public int GetNumOfPoints()
        {
            return _numOfPoints;
        }
    }


    public class LoadPointsCommand : SerialPortCommand
    {
        private readonly int _offset;
        private List<IRegSETrackPoint> _points = new List<IRegSETrackPoint>();

        public LoadPointsCommand(int offset)
            : base(800)
        {
            _offset = offset;
        }

        override protected byte[] GetCommandBuffer()
        {
            return ASCIIEnc.GetBytes("@AL,5,3," + _offset + "\r\n"); ;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            var tail = ASCIIEnc.GetBytes("@AL,5,3," + _offset + "\n");
            var pointsBufferLength = buffer.Length - (tail.Length + tail.Length + 2); // "@AL,CS,XX," + _offset + "\n" + "@AL,5,3," + _offset + "\n"
            if (!CompareLastN(buffer, tail, tail.Length) || pointsBufferLength % 16 != 0)
            {
                throw new RegSECommunicationError("Wrong data in LoadPointsCommand");
            }

            for (int offset = 0; offset < pointsBufferLength; offset += 16)
            {
                RegSETrackPoint point = RegSETrackPoint.FromByteArray(buffer, offset);
                _points.Add(point);
            }
        }

        public List<IRegSETrackPoint> GetPoints()
        {
            return _points;
        }
    }

    public class DeleteAllTracksCommand : SerialPortCommand
    {
        private static readonly byte[] Command = ASCIIEnc.GetBytes("@AL,5,6\r\n");
        private static readonly byte[] Header = ASCIIEnc.GetBytes("@AL,5,6\n");

        public DeleteAllTracksCommand()
            : base(3000)
        {
        }

        public bool IsOk { get; private set; }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (CompareFirstN(buffer, Header, Header.Length) && buffer.Length == Header.Length)
            {
                IsOk = true;
            }
        }
    }
}
