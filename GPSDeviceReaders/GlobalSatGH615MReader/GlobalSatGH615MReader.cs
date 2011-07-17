using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using QuickRoute.GPSDeviceReaders.GlobalSatGH615MReader.Properties;
using QuickRoute.GPSDeviceReaders.SerialPortDeviceReader;

namespace QuickRoute.GPSDeviceReaders.GlobalSatGH615MReader
{
    public delegate void ProgressDelegate(string message, int percent);
    public delegate void ReadCompletedDelegate();
    public delegate void ReadErrorDelegate(Exception e);

    public class GlobalSatGH615MReader
    {
        public event ProgressDelegate ProgressChanged;
        public event ReadCompletedDelegate ReadCompleted;
        public event ReadErrorDelegate ReadError;

        public GlobalSatGH615MReader()
        {
            Port = Settings.Default.PortName;    
        }

        private Thread _thread;
        private String _portName;
        private readonly SerialPort _commport = new SerialPort();
        
        private IGH615MDeviceInfo _deviceInfo;
        private IGH615MTrackInfo[] _tracksInfo = new IGH615MTrackInfo[] {};
        private readonly IDictionary<Int16, IGH615MTrack> _tracks = new Dictionary<Int16, IGH615MTrack>();

        public bool IsConnected { get; private set; }

        public String Port
        {
            get
            {
                return _portName;
            }
            set 
            {
                if (_portName == value)
                {
                    return;
                }
                ClearCache();
                _portName = value;
                CheckPort();
            }
        }

        public IGH615MDeviceInfo GetDeviceInfo()
        {
            if (_deviceInfo == null)
            {
                if (!IsConnected)
                {
                    return null;
                }
                using (var session = new SerialPortSession(_commport))
                {
                    var cmd = new GetDeviceInfoCommand();
                    cmd.Execute(session.Port);
                    _deviceInfo = cmd.GetDeviceInfo();
                }
            }
            return _deviceInfo;
        }

        public IGH615MTrackInfo[] GetTracksInfo()
        {
            if (_tracksInfo == null) 
            {
                if (!IsConnected)
                {
                    return null;
                }
                using (var session = new SerialPortSession(_commport))
                {
                    var cmd = new GetTracksListCommand();
                    cmd.Execute(session.Port);
                    _tracksInfo = cmd.GetTracksInfo();
                    Array.Reverse(_tracksInfo);
                }
            }
            return _tracksInfo;
        }

        public IGH615MTrack GetTrack(IGH615MTrackInfo info)
        {
            return GetTrack(info.Id);
        }

        public IGH615MTrack GetTrack(Int16 id)
        {
            lock (this)
            {
                if (_tracks.ContainsKey(id))
                {
                    return _tracks[id];
                }
                if (!IsConnected)
                {
                    return null;
                }
                GH615MTrack result = null;
                using (var session = new SerialPortSession(_commport))
                {
                    var startLoadCommand = new StartLoadTrackCommand(id);
                    startLoadCommand.Execute(session.Port);
                    var total = startLoadCommand.Status.TrackInfo.TrackPointsCount;
                    var current = startLoadCommand.Status.LastLoadedPointsCount;
                    SendProgressChanged(String.Format("Read track data... ({0}/{1})", current, total), current, total);
                    if (startLoadCommand.Status.IsFinished)
                    {
                        if (startLoadCommand.Status.TrackInfo != null)
                        {
                            result = new GH615MTrack(startLoadCommand.Status.TrackInfo, startLoadCommand.Status.TrackPoints.ToArray());
                        }
                    }
                    else
                    {
                        var command = new ContinueLoadTracksCommand(startLoadCommand.Status);
                        do
                        {
                            command.Execute(session.Port);
                            current += startLoadCommand.Status.LastLoadedPointsCount;
                            SendProgressChanged(String.Format("Read track data... ({0}/{1})", current, total), current, total);
                        }
                        while (!command.Status.IsFinished);
                        result = new GH615MTrack(command.Status.TrackInfo, command.Status.TrackPoints.ToArray());
                    }
                    SendReadCompleted();
                }
                if (result != null)
                {
                    _tracks.Add(id, result);
                }
                return result;
            }
        }

        public IGH615MTrack[] GetTracks(IGH615MTrackInfo[] infos)
        {
            var ids = new Int16[infos.Length];
            for (Int16 i = 0; i < infos.Length; i++)
            {
                ids[i] = infos[i].Id;
            }
            return GetTracks(ids);
        }

        public IGH615MTrack[] GetTracks(Int16[] ids)
        {
            var tracks = new List<IGH615MTrack>();
            foreach (var id in ids)
            {
                var track = GetTrack(id);
                tracks.Add(track);
            }
            return tracks.ToArray();
        }

        public void DeleteAllTracks()
        {
            if (!IsConnected)
            {
                return;
            }
            using (var session = new SerialPortSession(_commport))
            {
                var cmd = new DeleteAllTracksCommand();
                cmd.Execute(session.Port);
            }
        }

        public IGH615MWayPoint[] GetWayPoints()
        {
            // TODO: load waypoints 
            // 'getWaypoints'                    : '0200017776',
            //def getWaypoints(self):
            //    response = self._querySerial('getWaypoints')            
            //    waypoints = Utilities.chop(response[6:-2], 36) #trim junk
            //    return [Waypoint().fromHex(waypoint) for waypoint in waypoints] 
            //
            //class Waypoint(Point):
                
            //    def __init__(self, latitude = 0, longitude = 0, altitude = 0, title = '', type = 0):
            //        self.altitude = altitude
            //        self.title = title
            //        self.type = type
            //        super(Waypoint, self).__init__(latitude, longitude) 
                            
            //    def __str__(self):
            //        return "%s (%f,%f)" % (self.title, self.latitude, self.longitude)
                    
            //    def __hex__(self):
            //        return "%(title)s00%(type)s%(altitude)s%(latitude)s%(longitude)s" % {
            //            'latitude'  : hex(self.latitude),
            //            'longitude' : hex(self.longitude),
            //            'altitude'  : Utilities.dec2hex(self.altitude,4),
            //            'type'      : Utilities.dec2hex(self.type,2),
            //            'title'     : Utilities.chr2hex(self.title.ljust(6)[:6])
            //        }
                    
            //    def fromHex(self, hex):
            //        if len(hex) == 36:            
            //            def safeConvert(c):
            //                #if hex == 00 chr() converts it to space, not \x00
            //                if c == '00':
            //                    return ' '
            //                else:
            //                    return Utilities.hex2chr(c)
                            
            //            self.latitude = Coordinate().fromHex(hex[20:28])
            //            self.longitude = Coordinate().fromHex(hex[28:36])
            //            self.altitude = Utilities.hex2dec(hex[16:20])
            //            self.title = safeConvert(hex[0:2])+safeConvert(hex[2:4])+safeConvert(hex[4:6])+safeConvert(hex[6:8])+safeConvert(hex[8:10])+safeConvert(hex[10:12])
            //            self.type = Utilities.hex2dec(hex[12:16])
                        
            //            return self
            //        else:
            //            raise GH600ParseException(self.__class__.__name__, len(hex), 36)

            return null;
        }

        public void DeleteAllWayPoints()
        {
            if (!IsConnected)
            {
                return;
            }
            using (var session = new SerialPortSession(_commport))
            {
                var cmd = new DeleteAllWayPointsCommand();
                cmd.Execute(session.Port);
            }
        }
        private void ClearCache()
        {
            _tracks.Clear();
            _deviceInfo = null;
            _tracksInfo = null;
        }

        private void CheckPort()
        {
            if (string.IsNullOrEmpty(_portName))
            {
                IsConnected = false;
                return;
            }
            try
            {
                if (_commport.IsOpen)
                {
                    _commport.Close();
                }
                _commport.PortName = _portName;
                _commport.BaudRate = 57600;
                _commport.DataBits = 8;
                _commport.StopBits = StopBits.One;
                _commport.Parity = Parity.None;
                _commport.ReadTimeout = 500;
                _commport.WriteTimeout = 500;
                using (var session = new SerialPortSession(_commport))
                {
                    var checkCommand = new CheckGH615MPortCommand();
                    checkCommand.Execute(session.Port);
                    IsConnected = checkCommand.IsOk();
                    if (IsConnected)
                    {
                        Settings.Default.PortName = _portName;
                        Settings.Default.Save();
                    }
                }
            }
            catch (Exception)
            {
                IsConnected = false;
            }
        }

        public void RescanPort()
        {
            SendProgressChanged("Clear cache...", 0, 2);
            ClearCache();
            SendProgressChanged(String.Format("Re-read device on port {0}...", Port), 1, 2);
            CheckPort();
            try {
                var info = GetDeviceInfo();
                if (info != null)
                {
                    SendProgressChanged(String.Format("Device found on {0}.", Port), 2, 2);
                    SendReadCompleted();
                    return;
                }
            }
            catch (Exception e)
            {
                SendReadError(e);
            }
            SendProgressChanged("Device not found! Check connection and device mode.", 2, 2);
            SendReadCompleted();
        }

        public void ScanPorts()
        {
            var portNames = SerialPortUtil.GetLatestPortsList();
            if (portNames == null)
            {
                return;
            }
            var total = portNames.Length;
            for (var i = 0; i < portNames.Length; i++)
            {
                var portName = portNames[i];
                try
                {
                    SendProgressChanged(String.Format("Scaning port {0}...", portName), i, total);
                    Port = portName;
                    var info = GetDeviceInfo();
                    var tracksInfo = GetTracksInfo();
                    if (info != null)
                    {
                        SendProgressChanged(String.Format("Device found on {0}. Number of tracks: {1}", portName, tracksInfo.Length), total, total);
                        SendReadCompleted();
                        return;
                    }
                }
                catch (Exception e)
                {
                    SendReadError(e);
                }
            }
            SendProgressChanged("Device not found! Check connection and device mode.", total, total);
            SendReadCompleted();
        }

        private void StartReadThread(ThreadStart threadStartMethod)
        {
            StopReadPortThread();
            var myThread = new ThreadStart(threadStartMethod);
            _thread = new Thread(myThread);
            _thread.Start();
        }

        public void StartRescanPortThread()
        {
            StartReadThread(RescanPort);
        }

        public void StartReadTrackThread(IGH615MTrackInfo info)
        {
            var worker = new ReadTrackWorker(this, info);
            StartReadThread(worker.ReadTrack);
        }

        class ReadTrackWorker
        {
            private readonly IGH615MTrackInfo _info;
            private readonly GlobalSatGH615MReader _reader;

            public ReadTrackWorker(GlobalSatGH615MReader reader, IGH615MTrackInfo info)
            {
                _reader = reader;
                _info = info;    
            }

            public void ReadTrack()
            {
                _reader.GetTrack(_info);
            }
        }

        public void StopReadPortThread()
        {
            try
            {
                if (_thread != null && _thread.IsAlive)
                {
                    _thread.Abort();
                }
            }
            catch (ThreadAbortException)
            {
                if (_thread != null)
                {
                    while (_thread.ThreadState == ThreadState.Running)
                    {
                    }
                }
                _thread = null;
            }
        }

        private void SendProgressChanged(string message, int value, int max)
        {
            var percent = ((value / (Single)max) * 100);
            if (percent > 100)
            {
                percent = 100;
            }
            if (ProgressChanged != null)
            {
                ProgressChanged(message, (int)percent);
            }
        }

        private void SendReadCompleted()
        {
            if (ReadCompleted != null)
            {
                ReadCompleted();
            }
        }

        private void SendReadError(Exception e)
        {
            if (ReadError != null)
            {
                ReadError(e);
            }
        }

        public void Cancel()
        {
            StopReadPortThread();
        }
    }
}