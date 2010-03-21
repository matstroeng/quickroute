using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader.Properties;
using QuickRoute.GPSDeviceReaders.SerialPortDeviceReader;

namespace QuickRoute.GPSDeviceReaders.JJConnectRegistratorSEReader
{
    public delegate void ProgressDelegate(string message, int percent);
    public delegate void ReadCompletedDelegate();
    public delegate void ReadErrorDelegate(Exception e);

    public class JJConnectRegistratorSEReader
    {
        public event ProgressDelegate ProgressChanged;
        public event ReadCompletedDelegate ReadCompleted;
        public event ReadErrorDelegate ReadError;

        public JJConnectRegistratorSEReader()
        {
            Port = Settings.Default.PortName;    
        }

        private Thread _thread;
        private String _portName;
        private readonly SerialPort _commport = new SerialPort();

        private IRegSEDeviceInfo _deviceInfo;
        private List<IRegSETrack> _tracks;
        private List<IRegSETrackInfo> _trackInfos;
        private bool _loadCompleted;

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

        public IRegSEDeviceInfo GetDeviceInfo()
        {
            if (_deviceInfo == null)
            {
                if (!IsConnected)
                {
                    return null;
                }
                using (var session = new SerialPortSession(_commport))
                {
                    var deviceNameCommand = new DeviceNameCommand();
                    deviceNameCommand.Execute(session.Port);
                    var deviceName = deviceNameCommand.GetDeviceName();
                    var nameCommand = new NameCommand();
                    nameCommand.Execute(session.Port);
                    var name = nameCommand.GetName();
                    var numberOfPointsCommand = new NumOfPointsCommand(); ;
                    numberOfPointsCommand.Execute(session.Port);
                    var numberOfPoints = numberOfPointsCommand.GetNumOfPoints();

                    var softwareVersionCommand = new SoftwareVersionCommand(); ;
                    softwareVersionCommand.Execute(session.Port);
                    var softwareVersion = softwareVersionCommand.GetVersion();

                    var hardwareVersionCommand = new HardwareVersionCommand(); ;
                    hardwareVersionCommand.Execute(session.Port);
                    var hardwareVersion = hardwareVersionCommand.GetVersion();

                    _deviceInfo = new RegSEDeviceInfo(deviceName, name, softwareVersion, hardwareVersion, numberOfPoints);
                }
            }
            return _deviceInfo;
        }

        public List<IRegSETrackInfo> GetTracksInfo()
        {
            lock (this)
            {
                if (_tracks != null)
                {
                    return _trackInfos;
                }
                LoadTracks();
                return _trackInfos;
            }
        }

        public List<IRegSETrack> GetTracks()
        {
            if (_tracks != null)
            {
                return _tracks;
            }
            LoadTracks();
            return _tracks;
        }

        private void LoadTracks()
        {
            lock (this)
            {
                _loadCompleted = false;
                if (!IsConnected)
                {
                    SendReadError(new RegSEError("No connection with device!"));
                    SendReadCompleted();
                    return;
                }
                using (var session = new SerialPortSession(_commport))
                {
                    var getNumOfPointsCommand = new NumOfPointsCommand();
                    getNumOfPointsCommand.Execute(session.Port);
                    int total = getNumOfPointsCommand.GetNumOfPoints();
                    int current = 0;
                    var loadPointsCommand = new LoadPointsCommand(current*16);
                    loadPointsCommand.Execute(session.Port);
                    var points = new List<IRegSETrackPoint>();
                    points.AddRange(loadPointsCommand.GetPoints());
                    current += loadPointsCommand.GetPoints().Count;

                    SendProgressChanged(String.Format("Read track data... ({0}/{1})", current, total), current, total);
                    do
                    {
                        loadPointsCommand = new LoadPointsCommand(current*16);
                        loadPointsCommand.Execute(session.Port);
                        points.AddRange(loadPointsCommand.GetPoints());
                        current += loadPointsCommand.GetPoints().Count;
                        SendProgressChanged(String.Format("Read track data... ({0}/{1})", current, total), current,
                                            total);
                    } while (current < total);

                    // split into tracks
                    if (points.Count > 0)
                    {
                        _tracks = new List<IRegSETrack>();
                        _trackInfos = new List<IRegSETrackInfo>();

                        var startPoint = points[0];
                        if (!startPoint.HasMark(RegSEPointType.TrackStart))
                        {
                            SendReadError(new RegSEError("First point does not marked as track's start point!"));
                            SendReadCompleted();
                            return;
                        }
                        var startPointIndex = 0;
                        var numOfPoints = 0;

                        for (var i = 1; i < points.Count; i++)
                        {
                            numOfPoints++;
                            IRegSETrackPoint point = points[i];
                            if (point.HasMark(RegSEPointType.TrackStart))
                            {
                                var duration = (int) (points[i - 1].Time - startPoint.Time).TotalSeconds;
                                IRegSETrackInfo trackInfo = new RegSETrackInfo(startPoint.Time, duration, numOfPoints);
                                _trackInfos.Add(trackInfo);
                                _tracks.Add(new RegSETrack(trackInfo,
                                                           points.GetRange(startPointIndex, numOfPoints).ToArray()));
                                startPoint = point;
                                startPointIndex = i;
                                numOfPoints = 0;
                            }
                        }
                        {
                            var duration = (int) (points[points.Count - 1].Time - startPoint.Time).TotalSeconds;
                            IRegSETrackInfo trackInfo = new RegSETrackInfo(startPoint.Time, duration, numOfPoints);
                            _trackInfos.Add(trackInfo);
                            _tracks.Add(new RegSETrack(trackInfo,
                                                       points.GetRange(startPointIndex, numOfPoints).ToArray()));
                        }
                    }
                    _loadCompleted = true;
                    SendReadCompleted();
                }
            }
        }

        public void DeleteTracks()
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

        private void ClearCache()
        {
            _tracks = null;
            _trackInfos = null;
            _deviceInfo = null;
        }

        private void CheckPort()
        {
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
                    var checkCommand = new CheckRegSEPortCommand();
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
                    if (IsConnected)
                    {
                        SendProgressChanged(String.Format("Device found on {0}.", portName), total, total);
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

        public void StartLoadTracks()
        {
            StartReadThread(LoadTracks);
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

        public bool LoadCompleted
        {
            get { return _loadCompleted; }
        }

        public void Cancel()
        {
            StopReadPortThread();
        }
    }
}
