using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QuickRoute.GPSDeviceReaders.Polar
{
  public static class PolarRS800CXReader
  {
    public static List<DateTime> GetAllExercises()
    {
      var allExercises = new List<DateTime>();
      if (Connect())
      {
        POLAR_SSET_MONITORINFO monitorInfo;
        if (ReadMonitorInfo(out monitorInfo))
        {
          for (int i = 0; i < monitorInfo.iTotalFiles; i++)
          {
            POLAR_EXERCISEFILE exerciseHeader;
            if (GetExerciseHeader(i, out exerciseHeader))
            {
              DateTime dt = new DateTime(
                exerciseHeader.iDate / 10000,
                (exerciseHeader.iDate / 100) % 100,
                exerciseHeader.iDate % 100,
                exerciseHeader.iTime / 3600,
                (exerciseHeader.iTime / 60) % 60,
                exerciseHeader.iTime % 60);
              allExercises.Add(dt);
            }
          }
        }
        Disconnect();
      }
      return allExercises;
    }

    private static bool Connect()
    {
      return (API_ResetIRCommunication(0) != 0 && API_StartIRCommunication(32, "IR") != 0);
    }

    private static bool Disconnect()
    {
      return (API_EndIRCommunication(0) != 0);
    }

    private static bool ReadMonitorInfo(out POLAR_SSET_MONITORINFO monitorInfo)
    {
      POLAR_SSET_GENERAL psg = new POLAR_SSET_GENERAL();
      monitorInfo = new POLAR_SSET_MONITORINFO();
      // Fill general information
      psg.iConnection = 1;
      psg.hOwnerWnd = IntPtr.Zero;
      // Read monitor info from HR monitor
      return (API_ReadMonitorInfo(ref psg, ref monitorInfo) != 0);
    }

    private static bool GetExerciseHeader(int exerciseIndex, out POLAR_EXERCISEFILE exerciseHeader)
    {
      exerciseHeader = new POLAR_EXERCISEFILE();
      return (API_GetExeFileInfoEx(IntPtr.Zero, exerciseIndex, ref exerciseHeader) != 0);
    }

    public static bool GetExercise(int exerciseIndex, out PolarExercise exercise)
    {
      exercise = new PolarExercise();
      bool result = false;

      if (Connect())
      {
        POLAR_SSET_MONITORINFO monitorInfo;
        if (ReadMonitorInfo(out monitorInfo) && monitorInfo.iMonitorInUse == HRM_RS800CX)
        {
          // TODO: IntPtr.Zero or real window handle?
          if (API_ReadExerciseFile(IntPtr.Zero, exerciseIndex, 0) != 0)
          {
            int noOfSamples = API_GetNbrOfHRMSamples();
            exercise.HeartRates = new double[noOfSamples];
            exercise.Altitudes = new double[noOfSamples];
            for (int i = 0; i < noOfSamples; i++)
            {
              exercise.HeartRates[i] = Convert.ToDouble(API_GetHRMSamples(CC_HRATE, i));
              exercise.Altitudes[i] = Convert.ToDouble(API_GetHRMSamples(CC_ALT, i));
            }
            result = true;
          }
        }
        Disconnect();
      }
      return result;
    }

    public class PolarExercise
    {
      public double[] HeartRates { get; set; }
      public double[] Altitudes { get; set; }
    }



    #region Polar API

    private const string dllFileName = @"C:\dev\QuickRoute\GPSDeviceReaders\PolarRS800CXReader\Dependencies\hrmcom.dll";
    private const int MAX_PATH = 260;

    private const Int32 CC_HRATE = 1;				// heart rate values (bpm / msec)
    private const Int32 CC_SPEED = 2;				// speed values (10 * km/h / 10 * mph)
    private const Int32 CC_CAD = 3;				// cadence values (rpm)
    private const Int32 CC_ALT = 4;				// altitude values (m / ft)
    private const Int32 CC_POWER = 5;				// power values (Watts)
    private const Int32 CC_POWER_BALANCE = 6;				// power LR Balance (left%)
    private const Int32 CC_POWER_INDEX = 7;				// power pedalling index (%)
    private const Int32 CC_DIST = 8;				// distance values
    private const Int32 CC_AIRPR = 9;

    private const Int32 INT_INT_TIME = 601;				// Lap time in 1/10 seconds

    private const Int32 HRM_RS800CX = 38;			// Polar RS800CX

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_ResetIRCommunication@4")]
    private static extern Int32 API_ResetIRCommunication(Int32 iParam);

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_StartIRCommunication@8")]
    private static extern Int32 API_StartIRCommunication(Int32 iParam, string tcPort);

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_EndIRCommunication@4")]
    private static extern Int32 API_EndIRCommunication(Int32 iParam);

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_ReadMonitorInfo@8")]
    private static extern Int32 API_ReadMonitorInfo(ref POLAR_SSET_GENERAL psg, ref POLAR_SSET_MONITORINFO psmi);

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_SendMonitorToWatchMode@4")]
    private static extern Int32 API_SendMonitorToWatchMode(ref POLAR_SSET_GENERAL psg);

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_GetExeFileInfoEx@12")]
    private static extern Int32 API_GetExeFileInfoEx(IntPtr hOwnerWnd, Int32 iFileNumber, ref POLAR_EXERCISEFILE pef);

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_ReadExerciseFile@12")]
    private static extern Int32 API_ReadExerciseFile(IntPtr hOwnerWnd, Int32 iFileNumber, Int32 iParam);

    [DllImport(dllFileName, EntryPoint = "fnHRMCom_GetPolarHRMonitorName")]
    private static extern string API_GetPolarHRMonitorName(Int32 iMonitorInUse, Int32 iMonitorSubModel);

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_GetNbrOfHRMSamples@0")]
    private static extern Int32 API_GetNbrOfHRMSamples();

    [DllImport(dllFileName, EntryPoint = "_fnHRMCom_GetHRMSamples@8")]
    private static extern Int32 API_GetHRMSamples(Int32 iAttribute, Int32 iSampleIndex);


    private static POLAR_SSET_GENERAL Create_POLAR_SSET_GENERAL()
    {
      POLAR_SSET_GENERAL o = new POLAR_SSET_GENERAL();
      o.szWaveFile = new string(' ', 260);
      o.zDlgMsg = new string(' ', 50);
      o.iSize = 354;
      return o;
    }


    private struct POLAR_SSET_GENERAL
    {
      public Int32 iSize;				// Structure size for version control
      // Get using sizeof (STRUCTURE)

      public int iConnection;		// Connection method: HRMCOM_CONNECTION_UPLINK or HRMCOM_CONNECTION_IR
      // NOTE: Polar UpLink connection can be used only for writing information to HR monitor.

      public Int32 iMonitorID;			// Unique monitor ID, 0 = message to all monitors
      // Monitor will accept the messages if monitor id to send is same as already
      // set by User settings or if message was meant for all monitors available.
      // Other ID numbers used mainly with IR communication

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
      public string szWaveFile; // Wave file name, use NULL to create random file name
      // to Temp folder (MAX_PATH = 260)

      // EXCLUSION FLAGS
      // ---------------
      public Int32 bNoCreateWave;		// Don't create wave file at all, this allows testing of values in the data structure
      public Int32 bNoPlayWave;		// Don't play created Polar UpLink WAV file
      public Int32 bNoDeleteWave;		// Don't delete created Polar UpLink WAV file after it have been played

      // DATA FILE MANAGEMENT
      // --------------------
      public Int32 bLoadFromDataFile;	// Load information from binary data file, file name have to be at szWaveFile
      // If trying to load the data file with not the same data as data structure
      // specified in call, all the calling functions will return FALSE
      // When file will be loaded, other actions (create, play, delete wave) are not done.
      // If loaded file includes incorrect data, default values will be set automatically.

      public Int32 bSaveAsDataFile;	// Save information to binary data file, file name have to be at szWaveFile
      // When file will be saved, other actions (create, play, delete wave) are not done.

      // CONNECTION DIALOG
      // -----------------
      public Int32 bConnectionDlg;		// Usage of connection dialog to user. Dialog is shown always with IR connection.
      public IntPtr hOwnerWnd;			// Owner window handle to connection dialog
      // If connection dialog has been selected to be shown, owner window
      // handle have to be specified. If not, dialog won't be shown and connection fails.
      // If connection dialog is not in use, this parameter is ingnored.

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
      public string zDlgMsg;		// Connection dialog message to user, max 50 characters
      // If message text is not specified, default English texts will be used
      // If connection dialog is not in use, this parameter is ingnored.

      // MISC PARAMETERS
      // ---------------
      public Int32 bFixErrors;			// Errors in settings can be fixed automatically and error messages
      // are not returned in normal cases.

      public Int32 iParam;				// Parameter reserved for future usage, use zero
      public Int32 lParam;				// Parameter reserved for future usage, use zero

    }

    private struct POLAR_SSET_MONITORINFO
    {
      public Int32 iSize;					// Structure size for version control

      public Int32 iMonitorInUse;			// HR monitor in use: HRM_RS400, HRM_RS800, HRM_S610, HRM_S710, etc.
      public Int32 iDataVersion;			// HR monitor data version (for Int32ernal usage)

      public Int32 iTotalFiles;			// Total count of all files inside HR monitor
      public Int32 iFreeMemoryInBytes;		// Free memory inside HR monitor (in bytes)
      public Int32 iTotalMemoryInBytes;	// Total memory inside HR monitor (in bytes)

      public Int32 iLowBattery;			// Low battery indicator, see detailed information below

      public Int32 bHorseMode;				// Reserved for Equine usage
      public Int32 iSpeedSensor;			// Speed measurement type: 0 = bike speed sensor / foot pod (other values reserved)

      public Int32 iMonitorSubModel;		// HR Monitor sub model identifier (for Int32ernal usage):

      // HR Monitor		SubModel	Handled as
      // Polar S610		0x11		HRM_S610
      // Polar S610i		0x12		HRM_S610
      // Polar S625X		0x13		HRM_S625
      // Polar S710		0x22		HRM_S710
      // Polar S710i		0x23		HRM_S710
      // Polar S720i		0x24		HRM_S710
      // Polar S725		0x25		HRM_S725
      // Polar S725X		0x26		HRM_S625
      // Polar S810		0x33		HRM_S810
      // Polar S810i		0x34		HRM_S810

      // Note: Because of internal data similarity, all sub models within
      // one monitor type (iMonitorInUse) are handled in similar way.
    }

    private struct POLAR_EXERCISEFILE
    {
      public Int32 iSize;					// Structure size for version control

      public Int32 iTime;					// Start time of exercise in seconds
      public Int32 iDate;					// Start date of exercise in yyyymmdd
      public Int32 iDuration;				// Duration of exercise in seconds
      public Int32 bUSTimeMode;			// Usage of 12h time mode in exercise
      public Int32 iSamplingRate;			// Sampling rate of exercise
      public Int32 bRRRecording;			// RR-recording
      public Int32 bDeleted;				// Exercise has been marked to be deleted

      public Int32 bSpeed;					// Speed sensor data available
      public Int32 bCadence;				// Cadence sensor data available
      public Int32 bAltitude;				// Altitude sensor data available
      public Int32 bPower;					// Power sensor data available
      public Int32 bLocation;				// Location data available.	
      public Int32 bInterval;				// interval data available

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
      public string szName;				// Exercise set/profile name used in exercise
      // Max number of characters is 10 + ending zero

      public Int32 iBike;					// Bike used at exercise (0 = off, 1 = bike1, 2 = bike2)
      public Int32 iExerciseID;

    }

    #endregion
  }
}
