using System;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class D1002_Workout_Type
    {
        UInt32 _num_valid_steps;                                                /* Number of valid steps (1-20) */
        D_Steps[] steps = new D_Steps[20];
        char[] _name = new char[16];                                            /* Null-terminated workout name */
        Sport_Type _sport_type;                                                 /* Same as D1000 */

        public UInt32 NumValidSteps
        {
            set { _num_valid_steps = value; }
            get { return _num_valid_steps; }
        }

        public Sport_Type SportType
        {
            set { _sport_type = value; }
            get { return _sport_type; }
        }
    }
}
