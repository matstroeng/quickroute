using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.GPSDeviceReaders.GlobalSatGH615MReader;

namespace QuickRoute.BusinessEntities.Importers.GlobalSat.GH615M
{
    public class GlobalSatGH615MImporter : IGPSDeviceImporter
    {
        public GlobalSatGH615MImporter()
        {
            
        }

        private readonly GlobalSatGH615MReader _gsGH615MReader = new GlobalSatGH615MReader();
        private IGH615MTrackInfo _trackToImport;
        private ImportResult _importResult;

        public DialogResult ShowPreImportDialogs()
        {
            if (BeginWork != null) BeginWork(this, new EventArgs());

            DialogResult result;

            using (var dlg = new GlobalSatGH615MTrackSelector { DeviceReader = _gsGH615MReader })
            {
                result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _trackToImport = dlg.SelectedTrack;
                }
                dlg.Dispose();
            }

            if (EndWork != null) EndWork(this, new EventArgs());

            return result;
        }

        public event EventHandler<EventArgs> BeginWork;
        public event EventHandler<EventArgs> EndWork;
        public event EventHandler<WorkProgressEventArgs> WorkProgress;

        public bool IsConnected
        {
            get 
            {
                return _gsGH615MReader.IsConnected;
            }
        }

        public bool CachedDataExists
        {
          get { return false; }
        }

        public string DeviceName
        {
            get
            {
                if (_gsGH615MReader.IsConnected)
                {
                    return String.Format("GlobalSat GH-615M [{0}] on {1}", _gsGH615MReader.GetDeviceInfo().Name, _gsGH615MReader.Port);    
                }
                return "GlobalSat GH-615M device: not connected";
            }
        }

        public void Refresh()
        {
            _gsGH615MReader.ScanPorts();
        }

        public ImportResult ImportResult
        {
            get { return _importResult; }
            set { _importResult = value; }
        }

        public void Import()
        {
            _importResult = new ImportResult();
            if (BeginWork != null) BeginWork(this, new EventArgs());

            // The trackpoints
            var routeSegments = new List<RouteSegment>();
            var rs = new RouteSegment();
            var current = 0;
            var total = _trackToImport.TrackPointsCount;
            var track = _gsGH615MReader.GetTrack(_trackToImport);

            foreach (var tp in track.GetTrackPoints())
            {
                var waypoint = new Waypoint
                   {
                       Time = tp.Time,
                       LongLat = new LongLat((double) tp.Longitude, (double) tp.Latitude),
                       Altitude = tp.Altitude,
                       HeartRate = tp.Pulse
                   };

                rs.Waypoints.Add(waypoint);
                current++;
                if (WorkProgress != null && current % 10 == 0)
                {
                    WorkProgress(this, new WorkProgressEventArgs((double)current / total));
                }
            }
            if (rs.Waypoints.Count > 0)
            {
                routeSegments.Add(rs);
            }
            _importResult.Route = new Route(routeSegments);
            
            // create one lap (from start to finish)
            DateTime startTime = DateTime.MinValue;
            LapCollection laps = RouteImporterUtil.CreateLapsFromElapsedTimes(startTime, new List<double>(), routeSegments);
            _importResult.Laps = laps;

            _importResult.Succeeded = true;
            if (EndWork != null) EndWork(this, new EventArgs());
        }
    }
}
