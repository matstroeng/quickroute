namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public enum Program_Type
  {
    None = 0,
    Virtual_Partner = 1,    /* Completed with Virtual Partner */
    Workout = 2,            /* Completed as part of a workout */
    Auto_Multisport = 3     /* Completed as part of an auto MultiSport */
  };
}