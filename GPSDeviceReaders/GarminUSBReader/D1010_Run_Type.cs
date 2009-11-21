using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D1010_Run_Type
    {
        private UInt32 _track_index;                 /* Index of associated track */
        private UInt32 _first_lap_index;             /* Index of first associated lap */
        private UInt32 _last_lap_index;              /* Index of last associated lap */
        private Sport_Type _sport_type;              /* Sport type (same as D1000) */
        private Program_Type _program_type;          /* See below */
        private Multisport _multisport;              /* Same as D1009 */
        private byte _unused;                        /* Unused. Set to 0. */
        private D_Virtual_partner _virtual_partner;
        private D1002_Workout_Type _workout;         /* Workout */

        public D1010_Run_Type()
        {
            _unused = 0;
        }

        public UInt32 TrackIndex
        {
            set { _track_index = value; }
            get { return _track_index; }
        }
        public UInt32 FirstLapIndex
        {
            set { _first_lap_index = value; }
            get { return _first_lap_index; }
        }
        public UInt32 LastLapIndex
        {
            set { _last_lap_index = value; }
            get { return _last_lap_index; }
        }
        public Sport_Type SportType
        {
            set { _sport_type = value; }
            get { return _sport_type; }
        }
        public Program_Type ProgramType
        {
            set { _program_type = value; }
            get { return _program_type; }
        }
        public Multisport Multisport
        {
            set { _multisport = value; }
            get { return _multisport; }
        }
        public byte Unused
        {
            set { _unused = value; }
            get { return _unused; }
        }
        public D_Virtual_partner VirtualPartner
        {
            set { _virtual_partner = value; }
            get { return _virtual_partner; }
        }
        public D1002_Workout_Type Workout
        {
            set { _workout = value; }
            get { return _workout; }
        }
    }
}
