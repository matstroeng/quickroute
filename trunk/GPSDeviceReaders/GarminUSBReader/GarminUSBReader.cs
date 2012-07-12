using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using QuickRoute.Common;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public delegate void ProgressDelegate(ReadType readType, int step, int maxSteps, double partCompleted);
  public delegate void CompletedDelegate();
  public delegate void ErrorDelegate(Exception ex);

  public class GarminUSBReader
  {
    // singleton implementation
    private static readonly GarminUSBReader instance = new GarminUSBReader();
    public static GarminUSBReader Instance { get { return instance; } }

    private readonly object locker = new object();
    private bool abortingThreadNow;

    private int step;
    private int maxSteps;
    private ReadType readType;

    private IntPtr handle;
    private Int16 usbPacketSize;

    private IDictionary<string, GarminSessionHeader> cachedSessionHeaders = new Dictionary<string, GarminSessionHeader>();

    private string CachedSessionsFileName
    {
      get { return Path.Combine(CacheDirectory, "CachedSessions.bin"); }
    }

    public string cacheDirectory;
    public string CacheDirectory
    {
      get { return cacheDirectory; }
      set
      {
        cacheDirectory = value;
        if (!Directory.Exists(cacheDirectory))
        {
          try
          {
            Directory.CreateDirectory(cacheDirectory);
          }
          catch (Exception)
          {
          }
        }

        if (File.Exists(CachedSessionsFileName))
        {
          try
          {
            cachedSessionHeaders = CommonUtil.DeserializeFromFile<IDictionary<string, GarminSessionHeader>>(CachedSessionsFileName);
          }
          catch (Exception)
          {
          }
        }
      }
    }

    private Thread workingThread;

    private GarminUSBReader()
    {
    }

    private bool readingNow;
    public bool ReadingNow
    {
      get
      {
        lock(locker)
        {
          return readingNow;
        }
      } 
      private set
      {
        lock (locker)
        {
          readingNow = value;
        }
      }
    }

    #region Events

    public event ProgressDelegate Progress;

    public event CompletedDelegate Completed;

    public event ErrorDelegate Error;

    #endregion

    #region Enums

    private enum Packet_Type
    {
      USB_Protocol_Layer = 0,
      Application_Layer = 20
    }

    private enum L000_Packet_Id
    {
      Pid_Protocol_Array = 253,
      Pid_Product_Rqst = 254,
      Pid_Product_Data = 255,
      Pid_Ext_Product_Data = 248
    }

    private enum L001_Packet_Id
    {
      Pid_Command_Data = 10,
      Pid_Xfer_Cmplt = 12,
      Pid_Date_Time_Data = 14,
      Pid_Position_Data = 17,
      Pid_Prx_Wpt_Data = 19,
      Pid_Records = 27,
      Pid_Rte_Hdr = 29,
      Pid_Rte_Wpt_Data = 30,
      Pid_Almanac_Data = 31,
      Pid_Trk_Data = 34,
      Pid_Wpt_Data = 35,
      Pid_Pvt_Data = 51,
      Pid_Rte_Link_Data = 98,
      Pid_Trk_Hdr = 99,
      Pid_FlightBook_Record = 134,
      Pid_Lap = 149,
      Pid_Wpt_Cat = 152,
      Pid_Run = 990,
      Pid_Workout = 991,
      Pid_Workout_Occurrence = 992,
      Pid_Fitness_User_Profile = 993,
      Pid_Workout_Limits = 994,
      Pid_Course = 1061,
      Pid_Course_Lap = 1062,
      Pid_Course_Point = 1063,
      Pid_Course_Trk_Hdr = 1064,
      Pid_Course_Trk_Data = 1065,
      Pid_Course_Limits = 1066
    }

    private enum A010_Command_Id_Type
    {
      Cmnd_Abort_Transfer = 0,                            /* abort current transfer */
      Cmnd_Transfer_Alm = 1,                              /* transfer almanac */
      Cmnd_Transfer_Posn = 2,                             /* transfer position */
      Cmnd_Transfer_Prx = 3,                              /* transfer proximity waypoints */
      Cmnd_Transfer_Rte = 4,                              /* transfer routes */
      Cmnd_Transfer_Time = 5,                             /* transfer time */
      Cmnd_Transfer_Trk = 6,                              /* transfer track log */
      Cmnd_Transfer_Wpt = 7,                              /* transfer waypoints */
      Cmnd_Turn_Off_Pwr = 8,                              /* turn off power */
      Cmnd_Start_Pvt_Data = 49,                           /* start transmitting PVT data */
      Cmnd_Stop_Pvt_Data = 50,                            /* stop transmitting PVT data */
      Cmnd_FlightBook_Transfer = 92,                      /* transfer flight records */
      Cmnd_Transfer_Laps = 117,                           /* transfer fitness laps */
      Cmnd_Transfer_Wpt_Cats = 121,                       /* transfer waypoint categories */
      Cmnd_Transfer_Runs = 450,                           /* transfer fitness runs */
      Cmnd_Transfer_Workouts = 451,                       /* transfer workouts */
      Cmnd_Transfer_Workout_Occurrences = 452,            /* transfer workout occurrences */
      Cmnd_Transfer_Fitness_User_Profile = 453,           /* transfer fitness user profile */
      Cmnd_Transfer_Workout_Limits = 454,                 /* transfer workout limits */
      Cmnd_Transfer_Courses = 561,                        /* transfer fitness courses */
      Cmnd_Transfer_Course_Laps = 562,                    /* transfer fitness course laps */
      Cmnd_Transfer_Course_Points = 563,                  /* transfer fitness course points */
      Cmnd_Transfer_Course_Tracks = 564,                  /* transfer fitness course tracks */
      Cmnd_Transfer_Course_Limits = 565                   /* transfer fitness course limits */
    }

    #endregion

    #region Constants

    private const int MAX_BUFFER_SIZE = 4096;

    private const int RECORDS_TYPE_LENGTH = 2;

    #endregion

    #region Public properties and methods

    public bool IsConnected
    {
      get
      {
        if (ReadingNow) return true;

        var connected = true;
        try
        {
          StartUSBCommunication();
        }
        catch
        {
          connected = false;
        }
        finally
        {
          StopUSBCommunication();
        }
        return connected;
      }
    }

    public string deviceName;
    public string DeviceName
    {
      get
      {
        return deviceName ?? "Garmin Forerunner";
      }
    }

    public void CancelRead()
    {
      try
      {
        if (handle != IntPtr.Zero)
        {
          // Send Cancel to device
          SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Abort_Transfer);
        }
      }
      catch (Exception)
      {
      }
      finally
      {
        StopUSBCommunication();
      }
      AbortThread();
    }

    public void BeginReadData()
    {
      if (ReadingNow) throw new ReadingNowException();
      abortingThreadNow = false;
      workingThread = new Thread(ReadData);
      workingThread.Start();
    }

    public bool CachedSessionsExists
    {
      get
      {
        return CacheDirectory != null && Directory.Exists(CacheDirectory) &&
               Directory.GetFiles(CacheDirectory).Length > 0;
      }
    }

    public IEnumerable<GarminSessionHeader> GetSessionHeadersFromCache()
    {
      return cachedSessionHeaders.Values;
    }

    public GarminSession GetSessionFromCache(GarminSessionHeader sessionHeader)
    {
      if (!cachedSessionHeaders.ContainsKey(sessionHeader.Key)) return null;
      try
      {
        return CommonUtil.DeserializeFromFile<GarminSession>(GetSessionFileName(sessionHeader));
      }
      catch (Exception)
      {
        return null;
      }
    }

    #endregion

    #region Private methods

    private void ReadData()
    {
      Exception exception = null;
      var completed = false;
      try
      {
        StartUSBCommunication();

        maxSteps = 4;
        // first get device information
        step = 1;
        readType = ReadType.ProductData;
        var deviceInfo = GetDeviceInformation();
        deviceName = deviceInfo == null ? "Garmin Forerunner" : deviceInfo.ProductDescription;

        // then get runs...
        step = 2;
        readType = ReadType.Runs;
        var runs = GetRuns();

        // ... and laps
        step = 3;
        readType = ReadType.Laps;
        var laps = GetLaps();

        // based on the runs and laps, check if there are any non-cached sessions present
        var newSessionFound = false;
        foreach (var session in CreateSessions(runs, laps, null, false))
        {
          var key = session.GetHeader().Key;
          if (!cachedSessionHeaders.ContainsKey(key))
          {
            newSessionFound = true;
            break;
          }
        }

        if (newSessionFound)
        {
          // there is at least one new session found, download the trackpoints for all sessions and save to cache
          step = 4;
          readType = ReadType.Tracks;
          var tracks = GetTracks();
          foreach (var session in CreateSessions(runs, laps, tracks, true))
          {
            var sessionHeader = session.GetHeader();
            CommonUtil.SerializeToFile(session, GetSessionFileName(sessionHeader));
            if (!cachedSessionHeaders.ContainsKey(sessionHeader.Key)) cachedSessionHeaders.Add(sessionHeader.Key, sessionHeader);
          }
          CommonUtil.SerializeToFile(cachedSessionHeaders, CachedSessionsFileName);
        }

        completed = true;
      }
      catch (Exception e)
      {
        exception = e;
      }
      finally
      {
        StopUSBCommunication();
        if (completed) SendCompleted();
        if (exception != null) SendError(exception);
      }
    }

    public void AbortThread()
    {
      abortingThreadNow = true;
      try
      {
        if (workingThread != null && workingThread.IsAlive)
        {
          workingThread.Abort();
        }
      }
      catch (ThreadAbortException)
      {
        while (workingThread.ThreadState == ThreadState.Running) { Thread.Sleep(100); }
        workingThread = null;
      }
    }

    private GarminUSBDeviceInformation GetDeviceInformation()
    {
      // A000 Product data and A001 Protocol Capability Protocol
      var p = new GarminUSBPacket((byte)Packet_Type.Application_Layer, (ushort)L000_Packet_Id.Pid_Product_Rqst);
      SendPacket(p);

      SendProgress(0);
      var packet = GetPacket();
      if (packet.Id == (short)L000_Packet_Id.Pid_Product_Data)
      {
        var c = new char[packet.Data.Length - 4];
        Array.Copy(packet.Data, 4, c, 0, packet.Data.Length - 4);
        return new GarminUSBDeviceInformation()
        {
          ProductId = BitConverter.ToUInt16(packet.Data, 0),
          SoftwareVersion = BitConverter.ToInt16(packet.Data, 2),
          ProductDescription = GetStringFromNullTerminatedCharArray(c)
        };
      }
      return null;
    }

    private IList<D1010_Run_Type> GetRuns()
    {
      var runs = new List<D1010_Run_Type>();

      // Retrive Runs
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Runs);

      SendProgress(0);
      var packet = GetPacket();
      UInt16 numberOfExpectedRecords = 0;
      UInt16 numberOfRecords = 0;
      while (packet.Id != 0 && packet.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        if (packet.Id == (short)L001_Packet_Id.Pid_Records)
        {
          numberOfExpectedRecords = BitConverter.ToUInt16(packet.Data, 0);
        }
        else if (packet.Id == (short)L001_Packet_Id.Pid_Run)
        {
          numberOfRecords++;
          SendProgress((double)numberOfRecords / numberOfExpectedRecords);
          runs.Add(CreateRun(packet.Data));
        }
        packet = GetPacket();
      }
      SendProgress(1);
      return runs;
    }

    private IList<D1001_Lap_Type> GetLaps()
    {
      var laps = new List<D1001_Lap_Type>();

      // Retrive Laps
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Laps);

      SendProgress(0);
      var packet = GetPacket();
      UInt16 numberOfExpectedRecords = 0;
      UInt16 numberOfRecords = 0;
      while (packet.Id != 0 && packet.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        if (packet.Id == (short)L001_Packet_Id.Pid_Records)
        {
          numberOfExpectedRecords = BitConverter.ToUInt16(packet.Data, 0);
        }
        else if (packet.Id == (short)L001_Packet_Id.Pid_Lap)
        {
          numberOfRecords++;
          SendProgress((double)numberOfRecords / numberOfExpectedRecords);
          laps.Add(CreateLap(packet.Data));
        }
        packet = GetPacket();
      }
      SendProgress(1);
      return laps;
    }

    private IDictionary<UInt32, IList<D303_Trk_Point_Type>> GetTracks()
    {
      var tracks = new Dictionary<UInt32, IList<D303_Trk_Point_Type>>();
      // Retrive Tracks
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Trk);

      SendProgress(0);
      var packet = GetPacket();
      UInt16 numberOfExpectedRecords = 0;
      UInt16 numberOfRecords = 0;
      UInt16 trackKey = 0;
      var validTrack = true;

      while ((numberOfRecords < numberOfExpectedRecords || numberOfExpectedRecords == 0) && packet.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        // would rather exit on thePacket.Id == 0, but there is something fishy with Garmin devices setting the id to 0 sometimes
        if (packet.Id == (short)L001_Packet_Id.Pid_Records)
        {
          numberOfExpectedRecords = BitConverter.ToUInt16(packet.Data, 0);
        }
        else if (packet.Id == (short)L001_Packet_Id.Pid_Trk_Hdr)
        {
          numberOfRecords++;
          // D311_Trk_Hdr_Type
          trackKey = BitConverter.ToUInt16(packet.Data, 0);
          if (!tracks.ContainsKey(trackKey))
          {
            tracks.Add(trackKey, new List<D303_Trk_Point_Type>());
            validTrack = true;
          }
          else
          {
            validTrack = false;
          }
        }
        else if (packet.Id == (short)L001_Packet_Id.Pid_Trk_Data)
        {
          numberOfRecords++;
          SendProgress((double)numberOfRecords / numberOfExpectedRecords);
          if (validTrack) tracks[trackKey].Add(CreateTrackpoint(packet.Data));
        }

        packet = GetPacket();
      }
      SendProgress(1);
      return tracks;
    }

    private void StartUSBCommunication()
    {
      ReadingNow = true;
      
      var guid = new Guid("2C9C45C2-8E7D-4C08-A12D-816BBAE722C0");
      string deviceId;
      const int portIndex = 0;

      IntPtr hDevInfoSet = APIs.SetupDiGetClassDevs(ref guid,
                            0,
                            IntPtr.Zero,
                            (int)(APIs.DeviceFlags.DigCDDeviceInterface | APIs.DeviceFlags.DigCFPresent));

      APIs.DeviceInterfaceDetailData mDetailedData = GetDeviceInfo(hDevInfoSet, guid, out deviceId, portIndex);

      handle = APIs.CreateFile(mDetailedData.DevicePath, FileAccess.Read | FileAccess.Write, FileShare.None, IntPtr.Zero, FileMode.Open, 0x00000080, IntPtr.Zero);

      // Did we get a handle?
      if ((int)handle < 0)
      {
        throw new GarminUsbException(Strings.ErrorFindingDevice);
      }

      IntPtr usbPacketSizePointer = Marshal.AllocHGlobal(sizeof(Int16));
      int bytesReturned;

      bool r = APIs.DeviceIoControl(
          handle.ToInt32(),
          CTL_CODE(0x00000022, 0x851, 0, 0),
          IntPtr.Zero,
          0,
          usbPacketSizePointer,
          (uint)sizeof(int),
          out bytesReturned,
          0);
      if (!r)
      {
        throw new GarminUsbException(Strings.ErrorCommunicatingWithDevice);
      }
      usbPacketSize = (Int16)Marshal.PtrToStructure(usbPacketSizePointer, typeof(Int16));

      Marshal.FreeHGlobal(usbPacketSizePointer);

      // Tell the device that we are starting a session.
      var startSessionPacket = new GarminUSBPacket {Id = 5};

      SendPacket(startSessionPacket);

      GarminUSBPacket packet;
      while (true)
      {
        packet = GetPacket();
        if (packet.Type == 0 && packet.Id == 6)
        {
          break;
        }
      }
    }

    private void StopUSBCommunication()
    {
      try
      {
        if (handle != IntPtr.Zero)
        {
          APIs.CloseHandle(handle);
          handle = IntPtr.Zero;
        }
      }
      finally
      {
        ReadingNow = false;
      }
    }

    private void SendA010Command(UInt16 command)
    {
      var data = BitConverter.GetBytes(command);
      var p = new GarminUSBPacket((byte)Packet_Type.Application_Layer, (UInt16)L001_Packet_Id.Pid_Command_Data, RECORDS_TYPE_LENGTH, data);
      SendPacket(p);
    }

    private void SendPacket(GarminUSBPacket aPacket)
    {
      var bytesToWrite = 12 + (int)aPacket.Size;
      int theBytesReturned;
      IntPtr pPacket = Marshal.AllocHGlobal(bytesToWrite);
      Marshal.StructureToPtr(aPacket, pPacket, false);

      bool r = APIs.WriteFile(handle, pPacket, bytesToWrite, out theBytesReturned, IntPtr.Zero);
      Marshal.FreeHGlobal(pPacket);
      if (!r)
      {
        throw new GarminUsbException(Strings.ErrorCommunicatingWithDevice);
      }

      // If the packet size was an exact multiple of the USB packet size, we must make a final write call with no data
      if (bytesToWrite % usbPacketSize == 0)
      {
        r = APIs.WriteFile(handle, IntPtr.Zero, 0, out theBytesReturned, IntPtr.Zero);
        if (!r)
        {
          throw new GarminUsbException(Strings.ErrorCommunicatingWithDevice);
        }
      }
    }

    private GarminUSBPacket GetPacket()
    {
      GarminUSBPacket packet;
      var bufferSize = 0;
      var buffer = new byte[0];

      while (true)
      {
        // Read async data until the driver returns less than the
        // max async data size, which signifies the end of a packet
        var tempBuffer = new byte[APIs.ASYNC_DATA_SIZE];
        int bytesReturned;

        IntPtr pInBuffer = Marshal.AllocHGlobal(0);
        IntPtr pOutBuffer = Marshal.AllocHGlobal(APIs.ASYNC_DATA_SIZE);

        bool r = APIs.DeviceIoControl(
              handle.ToInt32(),
              CTL_CODE(0x00000022, 0x850, 0, 0),
              pInBuffer,
              0,
              pOutBuffer,
              APIs.ASYNC_DATA_SIZE,
              out bytesReturned,
              0);

        Marshal.Copy(pOutBuffer, tempBuffer, 0, APIs.ASYNC_DATA_SIZE);
        Marshal.FreeHGlobal(pInBuffer);
        Marshal.FreeHGlobal(pOutBuffer);
        if (!r)
        {
          throw new GarminUsbException(Strings.ErrorCommunicatingWithDevice);
        }

        bufferSize += APIs.ASYNC_DATA_SIZE;
        var newBuffer = new byte[bufferSize];

        if (buffer.Length > 0) { Array.Copy(buffer, 0, newBuffer, 0, buffer.Length); }
        Array.Copy(tempBuffer, 0, newBuffer, bufferSize - APIs.ASYNC_DATA_SIZE, tempBuffer.Length);

        buffer = newBuffer;

        if (bytesReturned != APIs.ASYNC_DATA_SIZE)
        {
          packet = new GarminUSBPacket(buffer);
          break;
        }
      }

      // If this was a small "signal" packet, read a real
      // packet using ReadFile
      if (packet.Type == 0 && packet.Id == 2)
      {
        var newBuffer = new byte[MAX_BUFFER_SIZE];
        var bytesReturned = 0;

        // A full implementation would keep reading (and queueing)
        // packets until the driver returns a 0 size buffer.
        bool r = APIs.ReadFile(handle,
                  newBuffer,
                  MAX_BUFFER_SIZE,
                  ref bytesReturned,
                  IntPtr.Zero);
        if (!r)
        {
          throw new GarminUsbException(Strings.ErrorCommunicatingWithDevice);
        }
        return new GarminUSBPacket(newBuffer);
      }

      return packet;
    }

    private static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
    {
      return ((deviceType << 16) | (access << 14) | (function << 2) | method);
    }

    private static APIs.DeviceInterfaceDetailData GetDeviceInfo(IntPtr hDevInfoSet, Guid deviceGUID, out string deviceID, int devicePortIndex)
    {
      // Get a Device Interface Data structure.
      // --------------------------------------

      APIs.DeviceInterfaceData interfaceData = new APIs.DeviceInterfaceData();
      APIs.CRErrorCodes crResult;
      IntPtr startingDevice;
      APIs.DevinfoData infoData = new APIs.DevinfoData();
      APIs.DeviceInterfaceDetailData detailData = new APIs.DeviceInterfaceDetailData();

      bool result = true;
      IntPtr ptrInstanceBuf;
      uint requiredSize = 0;

      interfaceData.Init();
      infoData.Size = Marshal.SizeOf(typeof(APIs.DevinfoData));
      detailData.Size = Marshal.SizeOf(typeof(APIs.DeviceInterfaceDetailData));

      result = APIs.SetupDiEnumDeviceInterfaces(
              hDevInfoSet,
              0,
              ref deviceGUID,
              (uint)devicePortIndex,
              ref interfaceData);

      if (!result)
      {
        if (Marshal.GetLastWin32Error() == APIs.ERROR_NO_MORE_ITEMS)
        {
          deviceID = string.Empty;
          return new APIs.DeviceInterfaceDetailData();
        }
        else
        {
          throw new ApplicationException("[CCore::GetDeviceInfo] Error when retriving device info");
        }
      }


      //Get a DevInfoDetailData and DeviceInfoData
      // ------------------------------------------

      // First call to get the required size.
      result = APIs.SetupDiGetDeviceInterfaceDetail(
              hDevInfoSet,
              ref interfaceData,
              IntPtr.Zero,
              0,
              ref requiredSize,
              ref infoData);

      // Create the buffer.
      detailData.Size = 5;

      // Call with the correct buffer
      result = APIs.SetupDiGetDeviceInterfaceDetail(
            hDevInfoSet,
            ref interfaceData,
            ref detailData, // ref devDetailBuffer,
            requiredSize,
            ref requiredSize,
            ref infoData);

      if (!result)
      {
        System.Windows.Forms.MessageBox.Show(Marshal.GetLastWin32Error().ToString());
        throw new ApplicationException("[CCore::GetDeviceInfo] Can not get SetupDiGetDeviceInterfaceDetail");
      }

      startingDevice = infoData.DevInst;

      //Get current device data
      ptrInstanceBuf = Marshal.AllocHGlobal(1024);
      crResult = (APIs.CRErrorCodes)APIs.CM_Get_Device_ID(startingDevice, ptrInstanceBuf, 1024, 0);

      if (crResult != APIs.CRErrorCodes.CR_SUCCESS)
        throw new ApplicationException("[CCore::GetDeviceInfo] Error calling CM_Get_Device_ID: " + crResult);

      // We have the pnp string of the parent device.
      deviceID = Marshal.PtrToStringAuto(ptrInstanceBuf);

      Marshal.FreeHGlobal(ptrInstanceBuf);
      return detailData;
    }

    private void SendProgress(double partCompleted)
    {
      if (Progress != null) Progress(readType, step, maxSteps, partCompleted);
    }

    private void SendCompleted()
    {
      if (Completed != null) Completed();
    }

    private void SendError(Exception ex)
    {
      if (Error != null && !abortingThreadNow) Error(ex);
    }

    private static string GetStringFromNullTerminatedCharArray(char[] c)
    {
      string s = new string(c);
      return s.Remove(s.IndexOf("\0"));
    }

    private static D1010_Run_Type CreateRun(byte[] data)
    {
      return new D1010_Run_Type
      {
        TrackIndex = BitConverter.ToUInt16(data, 0),
        FirstLapIndex = BitConverter.ToUInt16(data, 2),
        LastLapIndex = BitConverter.ToUInt16(data, 4),
        SportType = GetSportType(data[6]),
        ProgramType = GetProgramType(data[7]),
        Multisport = GetMultisport(data[8]),
        VirtualPartner = new D_Virtual_partner
        {
          Time = BitConverter.ToUInt32(data, 16),
          Distance = BitConverter.ToSingle(data, 20)
        },
        Workout = new D1002_Workout_Type
        {
          NumValidSteps = BitConverter.ToUInt32(data, 24)
        }
      };
    }

    private static Sport_Type GetSportType(byte value)
    {
      switch (value)
      {
        case 0: return Sport_Type.Running;
        case 1: return Sport_Type.Biking;
        default: return Sport_Type.Other;
      }
    }

    private static Program_Type GetProgramType(byte value)
    {
      switch (value)
      {
        case 1: return Program_Type.Virtual_Partner;
        case 2: return Program_Type.Workout;
        case 3: return Program_Type.Auto_Multisport;
        default: return Program_Type.None;
      }
    }

    private static Multisport GetMultisport(byte value)
    {
      switch (value)
      {
        case 1: return Multisport.Yes;
        case 2: return Multisport.YesAndLastInGroup;
        default: return Multisport.No;
      }
    }

    private static D1001_Lap_Type CreateLap(byte[] data)
    {
      return new D1001_Lap_Type()
               {
                 Index = BitConverter.ToUInt32(data, 0),
                 StartTime = BitConverter.ToUInt32(data, 4),
                 TotalTime = BitConverter.ToUInt32(data, 8),
                 TotalDist = BitConverter.ToSingle(data, 12),
                 MaxSpeed = BitConverter.ToSingle(data, 16),
                 Begin = new D_Position_Type()
                           {
                             Latitude = BitConverter.ToInt32(data, 20),
                             Longitude = BitConverter.ToInt32(data, 24)
                           },
                 End = new D_Position_Type()
                         {
                           Latitude = BitConverter.ToInt32(data, 28),
                           Longitude = BitConverter.ToInt32(data, 32)
                         },
                 Calories = BitConverter.ToUInt16(data, 36),
                 AvgHeartRate = data[38],
                 MaxHeartRate = data[39],
                 Intensity = data[40]
               };
    }

    private static D303_Trk_Point_Type CreateTrackpoint(byte[] data)
    {
      return new D303_Trk_Point_Type()
               {
                 Position = new D_Position_Type()
                              {
                                Latitude = BitConverter.ToInt32(data, 0),
                                Longitude = BitConverter.ToInt32(data, 4)
                              },
                 Time = BitConverter.ToUInt32(data, 8),
                 Altitude = BitConverter.ToSingle(data, 12),
                 HeartRate = data[20]
               };
    }

    private string GetSessionFileName(GarminSessionHeader sessionHeader)
    {
      return Path.Combine(CacheDirectory, sessionHeader.Key + ".bin");
    }

    public List<GarminSession> CreateSessions(IList<D1010_Run_Type> runs, IList<D1001_Lap_Type> laps, IDictionary<UInt32, IList<D303_Trk_Point_Type>> tracks, bool excludeSessionsWithoutTrackpoints)
    {
      var sessions = new List<GarminSession>();
      foreach (var run in runs)
      {
        // No tracks if trackIndex == 65535
        if (run.TrackIndex != 65535 && (!excludeSessionsWithoutTrackpoints || (tracks != null && tracks.ContainsKey(run.TrackIndex))))
        {
          sessions.Add(new GarminSession(run, GetLapsForRun(run, laps), GetTrackpointsForRun(run, tracks)));
        }
      }
      return sessions;
    }

    private static IList<D1001_Lap_Type> GetLapsForRun(D1010_Run_Type run, IEnumerable<D1001_Lap_Type> laps)
    {
      var lapsForRun = new List<D1001_Lap_Type>();
      foreach (var lap in laps)
      {
        if (lap.Index >= run.FirstLapIndex && lap.Index <= run.LastLapIndex)
        {
          lapsForRun.Add(lap);
        }
      }
      return lapsForRun;
    }

    private static IList<D303_Trk_Point_Type> GetTrackpointsForRun(D1010_Run_Type run, IDictionary<UInt32, IList<D303_Trk_Point_Type>> tracks)
    {
      return tracks != null && tracks.ContainsKey(run.TrackIndex) ? tracks[run.TrackIndex] : new List<D303_Trk_Point_Type>();
    }

    #endregion

    private class GarminUSBDeviceInformation
    {
      public UInt16 ProductId { get; set; }
      public Int16 SoftwareVersion { get; set; }
      public string ProductDescription { get; set; }
    }
  }

  public class GarminUsbException : Exception
  {
    public GarminUsbException(string message) : base(message)
    {
    }
  }
}
