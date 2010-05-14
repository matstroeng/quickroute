using System;
using System.Collections.Generic;
using System.Text;
using QuickRoute.GPSDeviceReaders.SerialPortDeviceReader;

namespace QuickRoute.GPSDeviceReaders.GlobalSatGH615MReader
{

    class GH615MError : Exception
    {
        public GH615MError() { }
        public GH615MError(string message) : base(message) { }
        public GH615MError(string message, Exception e) : base(message, e) { }
    }

    class GH615MCommunicationError : GH615MError
    {
        public GH615MCommunicationError() { }
        public GH615MCommunicationError(string message) : base(message) { }
        public GH615MCommunicationError(string message, Exception e) : base(message, e) { }
    }

    public class CheckGH615MPortCommand : SerialPortCommand
    {
        private bool _status;
        private static readonly byte[] Header = new byte[] { 0xBF, 0x00, 0x08, 0x47, 0x48, 0x2D, 0x36, 0x31, 0x35, 0x4D, 0x00, 0x55 };

        public CheckGH615MPortCommand()
            : base(DefaultSleepTime, Header.Length)
        {
        }

        override protected byte[] GetCommandBuffer()
        {
            return new byte[] { 0x02, 0x00, 0x01, 0xBF, 0xBE };
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

    public class GetDeviceInfoCommand : SerialPortCommand
    {
        private GH615MDeviceInfo _info;
        private static readonly byte[] Header = new byte[] { 0x85, 0x00 };

        public GetDeviceInfoCommand() : base(1000) { }

        override protected byte[] GetCommandBuffer()
        {
            return new byte[] { 0x02, 0x00, 0x01, 0x85, 0x84 };
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (!CompareFirstN(buffer, Header, Header.Length))
            {
                throw new GH615MCommunicationError("Wrong Header in GetDeviceInfoCommand");
            }

            var zero = new String(new[] { '\0' });
            var deviceName = Encoding.UTF8.GetString(buffer, 3, 9).Replace(zero, "");
            var version = String.Format("{0}.{1}", Convert.ToString(buffer[26], 16).PadLeft(2, '0'), Convert.ToString(buffer[27], 16).PadLeft(2, '0'));
            var firmware = Encoding.UTF8.GetString(buffer, 28, 16);
            var name = Encoding.UTF8.GetString(buffer, 45, 9).Replace(zero, "");
            int waypointsCount = buffer[66];
            int tracksCount = buffer[67];
            int routesCount = buffer[68];

            _info = new GH615MDeviceInfo(deviceName, version, firmware, name, waypointsCount, tracksCount, routesCount);
        }

        public IGH615MDeviceInfo GetDeviceInfo()
        {
            return _info;
        }
    }

    public class GetTracksListCommand : SerialPortCommand
    {
        private GH615MTrackInfo[] _infos = new GH615MTrackInfo[] { };

        private static readonly byte[] Header = new byte[] { 0x78 };

        override protected byte[] GetCommandBuffer()
        {
            return new byte[] { 0x02, 0x00, 0x01, 0x78, 0x79 };
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (CompareFirstN(buffer, Header, Header.Length) && buffer.Length > 4 && (buffer.Length - 4) % 24 == 0)
            {
                int tracksCount = (buffer.Length - (Header.Length + 2 + 1)) / 24;
                _infos = new GH615MTrackInfo[tracksCount];
                int offset = Header.Length + 2;
                for (int i = 0; i < tracksCount; i++)
                {
                    GH615MTrackInfo info = GH615MTrackInfo.FromByteArray(buffer, offset, null);
                    _infos[i] = info;
                    offset += 24;
                }
            }
        }

        public IGH615MTrackInfo[] GetTracksInfo()
        {
            return _infos;
        }
    }

    public class StartLoadTrackCommand : SerialPortCommand
    {
        private static readonly byte[] End = new byte[] { 0x8A, 0x00, 0x00, 0x00 };
        private readonly LoadStatus _status;

        public StartLoadTrackCommand(Int16 id)
            : base(1000)
        {
            _status = new LoadStatus(id);
        }

        override protected byte[] GetCommandBuffer()
        {
            var commandHeader = new byte[] { 0x02, 0x00 };
            var command = new byte[commandHeader.Length /* header */ + 2 /* payload */ + 2 /* numberOfTracks */ + 2 /* id */ + 1 /* checkSum */];
            Array.Copy(commandHeader, 0, command, 0, commandHeader.Length);

            int offset = commandHeader.Length;

            byte[] payload = HexUtils.GetBytes((Int16)(512 + 896));
            Array.Copy(payload, 0, command, offset, payload.Length);
            offset += 2;

            byte[] numberOfTracks = HexUtils.GetBytes((Int16)1);
            Array.Copy(numberOfTracks, 0, command, offset, numberOfTracks.Length);
            offset += 2;

            byte[] idBytes = HexUtils.GetBytes(_status.Id);
            Array.Copy(idBytes, 0, command, offset, idBytes.Length);
            offset += 2;

            command[offset] = HexUtils.CheckSum(command, commandHeader.Length, 2 /* payload */ + 2 /* numberOfTracks */ + 2 /* id */ );
            return command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (CompareFirstN(buffer, End, End.Length))
            {
                _status.FirstLoadedPointNumber = 0;
                _status.LastLoadedPointNumber = -1;
                _status.IsFinished = true;
                return;
            }
            if (buffer.Length >= 3 + 22)
            {
                _status.TrackInfo = GH615MTrackInfo.FromByteArray(buffer, 3, _status.Id);

                Int16 from = HexUtils.ToInt16(buffer, 3 + 22);
                Int16 to = HexUtils.ToInt16(buffer, 3 + 22 + 2);
                if (_status.LastLoadedPointNumber + 1 != from)
                {
                    throw new GH615MCommunicationError(String.Format("Loading error! Expected points from {0}, but get from {1} to {2}!", _status.LastLoadedPointNumber + 1, from, to));
                }
                int offset = 3 + 22 + 4;
                DateTime timeOffset = _status.TrackInfo.Date;
                if (_status.TrackPoints.Count > 0)
                {
                    timeOffset = _status.TrackPoints[_status.TrackPoints.Count-1].Time;
                }
                for (int i = 0; i <= to - from; i++)
                {
                    GH615MTrackPoint point = GH615MTrackPoint.FromByteArray(buffer, offset, timeOffset);
                    _status.TrackPoints.Add(point);
                    timeOffset = point.Time;
                    offset += 15;
                }
                _status.FirstLoadedPointNumber = from;
                _status.LastLoadedPointNumber = to;
            }
        }

        public LoadStatus Status { get { return _status; } }
    }

    public interface ILoadStatus
    {
        IGH615MTrackInfo TrackInfo { get; }
        Int16 LastLoadedPointNumber { get; }
        Int32 LastLoadedPointsCount { get; }
    }

    public class LoadStatus : ILoadStatus
    {
        private Int16 _from;
        private Int16 _to = -1;
        private readonly List<IGH615MTrackPoint> _trackPoints = new List<IGH615MTrackPoint>();

        public LoadStatus(Int16 id)
        {
            Id = id;
        }

        public Int16 Id { get; private set; }
        public IGH615MTrackInfo TrackInfo { get; set; }
        public List<IGH615MTrackPoint> TrackPoints { get { return _trackPoints; } }
        public Int16 LastLoadedPointNumber { get { return _to; } set { _to = value; } }
        public Int16 FirstLoadedPointNumber { get { return _from; } set { _from = value; } }
        public Int32 LastLoadedPointsCount { get { return _to - _from + 1; } }
        public bool IsFinished { get; set; }
    }

    public class ContinueLoadTracksCommand : SerialPortCommand
    {
        private static readonly byte[] End = new byte[] { 0x8A, 0x00, 0x00, 0x00 };
        private static readonly byte[] Command = new byte[] { 0x02, 0x00, 0x01, 0x81, 0x80 };
        private readonly LoadStatus _status;

        public ContinueLoadTracksCommand(LoadStatus status)
            : base(1000)
        {
            _status = status;
        }

        public LoadStatus Status { get { return _status; } }

        override protected byte[] GetCommandBuffer()
        {
            return Command;
        }

        override protected void ProcessBuffer(byte[] buffer)
        {
            if (CompareFirstN(buffer, End, End.Length))
            {
                _status.FirstLoadedPointNumber = 0;
                _status.LastLoadedPointNumber = -1;
                _status.IsFinished = true;
                return;
            }
            var from = HexUtils.ToInt16(buffer, 3 + 22);
            var to = HexUtils.ToInt16(buffer, 3 + 22 + 2);
            if (_status.LastLoadedPointNumber + 1 != from)
            {
                throw new GH615MCommunicationError(String.Format("Loading error! Expected points from {0}, but get from {1} to {2}!", _status.LastLoadedPointNumber + 1, from, to));
            }
            int offset = 3 + 22 + 4;
            DateTime timeOffset = _status.TrackInfo.Date;
            if (_status.TrackPoints.Count > 0)
            {
                timeOffset = _status.TrackPoints[_status.TrackPoints.Count-1].Time;
            }
            for (int i = 0; i <= to - from; i++)
            {
                GH615MTrackPoint point = GH615MTrackPoint.FromByteArray(buffer, offset, timeOffset);
                _status.TrackPoints.Add(point);
                timeOffset = point.Time;
                offset += 15;
            }
            _status.FirstLoadedPointNumber = from;
            _status.LastLoadedPointNumber = to;
        }
    }

    public class DeleteAllTracksCommand : SerialPortCommand
    {
        private static readonly byte[] End = new byte[] { 0x79, 0x00, 0x00, 0x00 };
        private static readonly byte[] Command = new byte[] { 0x02, 0x00, 0x03, 0x79, 0x00, 0x64, 0x1E };

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
            if (CompareFirstN(buffer, End, End.Length) && buffer.Length == End.Length)
            {
                IsOk = true;
            }
        }
    }

    public class DeleteAllWayPointsCommand : SerialPortCommand
    {
        private static readonly byte[] End = new byte[] { 0x75, 0x00, 0x00, 0x00 };
        private static readonly byte[] Command = new byte[] { 0x02, 0x00, 0x03, 0x75, 0x00, 0x64, 0x12 };

        public DeleteAllWayPointsCommand()
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
            if (CompareFirstN(buffer, End, End.Length) && buffer.Length == End.Length)
            {
                IsOk = true;
            }
        }
    }
}
