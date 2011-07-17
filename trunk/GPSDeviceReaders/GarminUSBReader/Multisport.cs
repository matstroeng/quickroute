using System;
using System.Collections.Generic;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public enum Multisport
  {
    No = 0,                 /* Not a MultiSport run */
    Yes = 1,                /* Part of a MultiSport session */
    YesAndLastInGroup = 2   /* The last of a MultiSport session */
  };
}
