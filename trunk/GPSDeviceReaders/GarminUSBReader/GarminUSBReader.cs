using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public delegate void USBProgressDelegate(string type, int stepNr, int stepMax, int percent);
  public delegate void USBReadCompletedDelegate();
  public delegate void USBReadErrorDelegate(Exception e);

  public class GarminUSBReader
  {
    private IntPtr handle;
    private Int16 usbPacketSize;
    private Thread thread;
    private GarminDevice garminDevice;
    private string readType;
    private int readStep;
    private int readStepsNrOf;
    private readonly string error_Finding_Device;
    private readonly string error_Communicating_With_Device;
    private bool readCompleted;

    public event USBProgressDelegate USBProgressChanged;

    public event USBReadCompletedDelegate USBReadCompleted;
    
    public event USBReadErrorDelegate USBReadError;
    

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

    private const int MAX_BUFFER_SIZE = 4096;

    private const int RECORDS_TYPE_LENGTH = 2;

    public GarminUSBReader()
    {
      usbPacketSize = 0;
      readStep = 0;
      readStepsNrOf = 0;
      error_Finding_Device = Strings.ErrorFindingDevice;
      error_Communicating_With_Device = Strings.ErrorCommunicatingWithDevice;
    }

    private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
    {
      return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
    }
 
    private static APIs.DeviceInterfaceDetailData GetDeviceInfo(IntPtr hDevInfoSet, Guid DeviceGUID, out string DeviceID, int DevicePortIndex)
    {
      try
      {
        // Get a Device Interface Data structure.
        // --------------------------------------

        APIs.DeviceInterfaceData interfaceData = new APIs.DeviceInterfaceData();
        APIs.CRErrorCodes CRResult;
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
                ref DeviceGUID,
                (uint)DevicePortIndex,
                ref interfaceData);

        if (!result)
        {
          if (Marshal.GetLastWin32Error() == APIs.ERROR_NO_MORE_ITEMS)
          {
            DeviceID = string.Empty;
            return new APIs.DeviceInterfaceDetailData();
          }
          else
            throw (new ApplicationException("[CCore::GetDeviceInfo] Error when retriving device info"));
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
          throw (new ApplicationException("[CCore::GetDeviceInfo] Can not get SetupDiGetDeviceInterfaceDetail"));
        }

        startingDevice = infoData.DevInst;

        //Get current device data
        ptrInstanceBuf = Marshal.AllocHGlobal(1024);
        CRResult = (APIs.CRErrorCodes)APIs.CM_Get_Device_ID(startingDevice, ptrInstanceBuf, 1024, 0);

        if (CRResult != APIs.CRErrorCodes.CR_SUCCESS)
          throw (new ApplicationException("[CCore::GetDeviceInfo] Error calling CM_Get_Device_ID: " + CRResult.ToString()));

        // We have the pnp string of the parent device.
        DeviceID = Marshal.PtrToStringAuto(ptrInstanceBuf);

        Marshal.FreeHGlobal(ptrInstanceBuf);
        return detailData;
      }
      catch (Exception e)
      {
        throw (e);
      }
    }

    private void StartUSBCommunication()
    {
      try
      {
        Guid guid = new Guid("2C9C45C2-8E7D-4C08-A12D-816BBAE722C0");
        string mDeviceID;
        int mPortIndex = 0;

        IntPtr hDevInfoSet = APIs.SetupDiGetClassDevs(ref guid
                              , 0
                              , IntPtr.Zero
                              , (int)(APIs.DeviceFlags.DigCDDeviceInterface | APIs.DeviceFlags.DigCFPresent));

        APIs.DeviceInterfaceDetailData mDetailedData = GetDeviceInfo(hDevInfoSet, guid, out mDeviceID, mPortIndex);

        handle = APIs.CreateFile(mDetailedData.DevicePath, FileAccess.Read | FileAccess.Write, FileShare.None, IntPtr.Zero, FileMode.Open, 0x00000080, IntPtr.Zero);

        // Did we get a handle?
        if ((int)handle < 0)
        {
          throw (new Exception(error_Finding_Device));
        }

        IntPtr pUSBPacketSize = Marshal.AllocHGlobal(sizeof(Int16));
        int bytesReturned = 0;

        bool r = APIs.DeviceIoControl(
            handle.ToInt32(),
            CTL_CODE(0x00000022, 0x851, 0, 0),
            IntPtr.Zero,
            0,
            pUSBPacketSize,
            (uint)sizeof(int),
            out bytesReturned,
            0);
        if (r == false)
        {
          throw (new Exception());
        }
        usbPacketSize = (Int16)Marshal.PtrToStructure(pUSBPacketSize, typeof(Int16));

        Marshal.FreeHGlobal(pUSBPacketSize);

        // Tell the device that we are starting a session.
        GarminUSBPacket theStartSessionPacket = new GarminUSBPacket();
        theStartSessionPacket.Id = 5;

        SendPacket(theStartSessionPacket);

        GarminUSBPacket thePacket;
        while (true)
        {
          thePacket = GetPacket();
          if (thePacket.Type == 0 && thePacket.Id == 6)
          {
            break;
          }
        }
      }
      catch (Exception e)
      {
        throw (e);
      }
    }
   
    private void StopUSBCommunication()
    {
      try
      {
        if (handle != null)
        {
          int i = APIs.CloseHandle(handle);
          handle = IntPtr.Zero;
        }
      }
      catch (Exception)
      { }
    }

    private void SendA010Command(UInt16 command)
    {
      try
      {
        byte[] data = BitConverter.GetBytes(command);
        GarminUSBPacket p = new GarminUSBPacket((byte)Packet_Type.Application_Layer, (UInt16)L001_Packet_Id.Pid_Command_Data, RECORDS_TYPE_LENGTH, data);
        SendPacket(p);
      }
      catch (Exception e)
      {
        throw (e);
      }
    }
  
    private void SendPacket(GarminUSBPacket aPacket)
    {
      try
      {
        int theBytesToWrite = 12 + (int)aPacket.Size;
        int theBytesReturned = 0;
        IntPtr pPacket = Marshal.AllocHGlobal(theBytesToWrite);
        Marshal.StructureToPtr(aPacket, pPacket, false);

        bool r = APIs.WriteFile(handle, pPacket, theBytesToWrite, out theBytesReturned, IntPtr.Zero);
        Marshal.FreeHGlobal(pPacket);
        if (r == false) { throw (new Exception(error_Communicating_With_Device)); }

        // If the packet size was an exact multiple of the USB packet size, we must make a final write call with no data
        if (theBytesToWrite % usbPacketSize == 0)
        {
          r = APIs.WriteFile(handle, IntPtr.Zero, 0, out theBytesReturned, IntPtr.Zero);
          if (r == false) { throw (new Exception(error_Communicating_With_Device)); }
        }
      }
      catch (Exception e)
      {
        throw (e);
      }
    }
   
    private GarminUSBPacket GetPacket()
    {
      try
      {
        GarminUSBPacket thePacket = new GarminUSBPacket();
        int theBufferSize = 0;
        byte[] theBuffer = new byte[0];

        while (true)
        {
          // Read async data until the driver returns less than the
          // max async data size, which signifies the end of a packet
          byte[] theTempBuffer = new byte[APIs.ASYNC_DATA_SIZE];
          byte[] theNewBuffer;
          int theBytesReturned = 0;

          IntPtr pInBuffer = Marshal.AllocHGlobal(0);
          IntPtr pOutBuffer = Marshal.AllocHGlobal(APIs.ASYNC_DATA_SIZE);

          bool r = APIs.DeviceIoControl(
                handle.ToInt32(),
                CTL_CODE(0x00000022, 0x850, 0, 0),
                pInBuffer,
                0,
                pOutBuffer,
                APIs.ASYNC_DATA_SIZE,
                out theBytesReturned,
                0);

          Marshal.Copy(pOutBuffer, theTempBuffer, 0, APIs.ASYNC_DATA_SIZE);
          Marshal.FreeHGlobal(pInBuffer);
          Marshal.FreeHGlobal(pOutBuffer);
          if (r == false) { throw (new Exception(error_Communicating_With_Device)); }

          theBufferSize += APIs.ASYNC_DATA_SIZE;
          theNewBuffer = new byte[theBufferSize];

          if (theBuffer.Length > 0) { Array.Copy(theBuffer, 0, theNewBuffer, 0, theBuffer.Length); }
          Array.Copy(theTempBuffer, 0, theNewBuffer, theBufferSize - APIs.ASYNC_DATA_SIZE, theTempBuffer.Length);

          theBuffer = theNewBuffer;

          if (theBytesReturned != APIs.ASYNC_DATA_SIZE)
          {
            thePacket = new GarminUSBPacket(theBuffer);
            break;
          }
        }

        // If this was a small "signal" packet, read a real
        // packet using ReadFile
        if (thePacket.Type == 0 && thePacket.Id == 2)
        {
          byte[] theTempBuffer = new byte[MAX_BUFFER_SIZE];
          byte[] theNewBuffer = new byte[MAX_BUFFER_SIZE];
          int theBytesReturned = 0;

          thePacket = new GarminUSBPacket();

          // A full implementation would keep reading (and queueing)
          // packets until the driver returns a 0 size buffer.
          bool r = APIs.ReadFile(handle,
                    theNewBuffer,
                    MAX_BUFFER_SIZE,
                    ref theBytesReturned,
                    IntPtr.Zero);
          if (r == false) { throw (new Exception(error_Communicating_With_Device)); }
          return new GarminUSBPacket(theNewBuffer);
        }
        else
        {
          return thePacket;
        }
      }
      catch (Exception e)
      {
        throw (e);
      }
    }

    private void SendUSBProgressChanged(int value, int max)
    {
      Single percent = (((Single)value / (Single)max) * 100);
      if (percent > 100) { percent = 100; }
      if (USBProgressChanged != null) { USBProgressChanged(readType, readStep, readStepsNrOf, (int)percent); }
    }
   
    private void SendUSBReadCompleted()
    {
      if (USBReadCompleted != null) { USBReadCompleted(); }
    }
   
    private void SendUSBReadError(Exception e)
    {
      if (USBReadError != null) { USBReadError(e); }
    }

    public void StartReadA1000Protocol()
    {
      StartReadThread(new ThreadStart(ReadA1000Protocol));
    }
   
    public void StartReadProductData()
    {
      StartReadThread(new ThreadStart(ReadA000Protocol));
    }
   
    public void ReadProductData()
    {
      ReadA000Protocol();
    }

    public bool IsConnected()
    {
      bool connected = true;
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

    public void StopReadGarminDevice()
    {
      try
      {
        if (thread != null && thread.IsAlive)
        {
            thread.Abort();
        }
      }
      catch (ThreadAbortException)
      {
        while (thread.ThreadState == ThreadState.Running) { }
        thread = null;
      }
    }
    
    public void Cancel()
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
        StopReadGarminDevice();
        readCompleted = false;
      }
    }

    private void StartReadThread(ThreadStart threadStartMethod)
    {
      StopReadGarminDevice();
      ThreadStart MyThread = new ThreadStart(threadStartMethod);
      thread = new Thread(MyThread);
      thread.Start();
    }
   
    private void ReadAllProtocols()
    {
      try
      {
        StartUSBCommunication();

        readStepsNrOf = 6;

        readStep = 1;
        readType = "product data";
        GetProductData_A000_And_A001();

        readStep = 2;
        readType = "date and time";
        GetDateTime();

        readStep = 3;
        readType = "almanac";
        GetAlmanac();

        readStep = 4;
        readType = "position";
        GetPosition();

        readStep = 5;
        readType = "runs";
        GetRuns();

        readStep = 6;
        readType = "laps";
        GetLaps();

        readStep = 7;
        readType = "tracks";
        GetTracks();

        SendUSBReadCompleted();
      }
      catch (Exception e)
      {
        SendUSBReadError(e);
      }
      finally
      {
        StopUSBCommunication();
        StopReadGarminDevice();
      }
    }
   
    private void ReadA1000Protocol()
    {
      try
      {
        StartUSBCommunication();

        readStepsNrOf = 3;
        readCompleted = false;

        readStep = 1;
        readType = "runs";
        GetRuns();

        readStep = 2;
        readType = "laps";
        GetLaps();

        readStep = 3;
        readType = "tracks";
        GetTracks();

        readCompleted = true;

        SendUSBReadCompleted();
      }
      catch (Exception e)
      {
        SendUSBReadError(e);
      }
      finally
      {
        StopUSBCommunication();
        StopReadGarminDevice();
      }
    }
  
    private void ReadA000Protocol()
    {
      try
      {
        StartUSBCommunication();

        GetProductData_A000_And_A001();

        SendUSBReadCompleted();
      }
      catch (Exception e)
      {
        SendUSBReadError(e);
      }
      finally
      {
        StopUSBCommunication();
        StopReadGarminDevice();
      }
    }
   
    private void GetProductData_A000_And_A001()
    {
      // A000 Product data and A001 Protocol Capability Protocol
      GarminUSBPacket p = new GarminUSBPacket((byte)Packet_Type.Application_Layer, (ushort)L000_Packet_Id.Pid_Product_Rqst);
      SendPacket(p);

      SendUSBProgressChanged(0, 100);
      GarminUSBPacket thePacket = GetPacket();
      if (thePacket.Id == (short)L000_Packet_Id.Pid_Product_Data)
      {
        garminDevice.AddProductData(thePacket.Data);
        thePacket = GetPacket();
        while (thePacket.Id != 0)
        {
          if (thePacket.Id == (short)L000_Packet_Id.Pid_Ext_Product_Data)
          { // Skip this according to documentation...
          }
          if (thePacket.Id == (short)L000_Packet_Id.Pid_Protocol_Array)
          {
            for (int i = 0; i < thePacket.Data.Length; i = i + 3)
            {
              SendUSBProgressChanged(i + 3, thePacket.Data.Length);
              D_Protocol_Data_Type pdt = new D_Protocol_Data_Type();
              pdt.Tag = thePacket.Data[i + 0];                // 1 bytes
              pdt.Data = BitConverter.ToUInt16(thePacket.Data, i + 1);  // 2 bytes
              garminDevice.Protocols.Add(pdt);
            }
            SendUSBProgressChanged(100, 100);
            break;
          }
          thePacket = GetPacket();
        }
      }
      garminDevice.GetSupportedProtocols();

    }
    
    private void GetAlmanac()
    {
      // Retrive Almanac
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Alm);

      SendUSBProgressChanged(0, 100);
      GarminUSBPacket thePacket = GetPacket();
      UInt16 nrOfExpectedRecords = 0;
      UInt16 nrOfRecords = 0;
      while (thePacket.Id != 0 && thePacket.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        if (thePacket.Id == (short)L001_Packet_Id.Pid_Records)
        {
          nrOfExpectedRecords = BitConverter.ToUInt16(thePacket.Data, 0);
        }
        if (thePacket.Id == (short)L001_Packet_Id.Pid_Almanac_Data)
        {
          nrOfRecords++;
          SendUSBProgressChanged(nrOfRecords, nrOfExpectedRecords);
          garminDevice.AddAlmanac(thePacket.Data);
        }
        thePacket = GetPacket();
      }
      SendUSBProgressChanged(100, 100);
    }
    
    private void GetPosition()
    {
      // Retrive Position
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Posn);

      SendUSBProgressChanged(0, 100);
      GarminUSBPacket thePacket = GetPacket();
      while (thePacket.Id != 0 && thePacket.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        if (thePacket.Id == (short)L001_Packet_Id.Pid_Position_Data)
        {
          SendUSBProgressChanged(50, 100);
          garminDevice.AddPosition(thePacket.Data);
        }
        break;
      }
      SendUSBProgressChanged(100, 100);
    }
    
    private void GetLaps()
    {
      // Retrive Laps
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Laps);

      SendUSBProgressChanged(0, 100);
      GarminUSBPacket thePacket = GetPacket();
      UInt16 nrOfExpectedRecords = 0;
      UInt16 nrOfRecords = 0;
      while (thePacket.Id != 0 && thePacket.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        if (thePacket.Id == (short)L001_Packet_Id.Pid_Records)
        {
          nrOfExpectedRecords = BitConverter.ToUInt16(thePacket.Data, 0);
        }
        else if (thePacket.Id == (short)L001_Packet_Id.Pid_Lap)
        {
          nrOfRecords++;
          SendUSBProgressChanged(nrOfRecords, nrOfExpectedRecords);
          garminDevice.AddLaps(thePacket.Data);
        }
        thePacket = GetPacket();
      }
      SendUSBProgressChanged(100, 100);
    }
    
    private void GetTracks()
    {
      // Retrive Tracks
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Trk);

      SendUSBProgressChanged(0, 100);
      GarminUSBPacket thePacket = GetPacket();
      UInt16 nrOfExpectedRecords = 0;
      UInt16 nrOfRecords = 0;
      UInt16 trackKey = 0;
      bool validTrack = true;
      garminDevice.Tracks = new SortedList<int, List<D303_Trk_Point_Type>>();
      while ((nrOfRecords < nrOfExpectedRecords || nrOfExpectedRecords == 0) && thePacket.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        // would rather exit on thePacket.Id == 0, but there is something fishy with Garmin devices setting the id to 0 sometimes
        if (thePacket.Id == (short)L001_Packet_Id.Pid_Records)
        {
          nrOfExpectedRecords = BitConverter.ToUInt16(thePacket.Data, 0);
        }
        else if (thePacket.Id == (short)L001_Packet_Id.Pid_Trk_Hdr)
        {
          nrOfRecords++;
          // D311_Trk_Hdr_Type
          trackKey = BitConverter.ToUInt16(thePacket.Data, 0);
          if (!garminDevice.Tracks.ContainsKey(trackKey))
          {
            garminDevice.Tracks.Add(trackKey, new List<D303_Trk_Point_Type>());
            validTrack = true;
          }
          else
          {
            validTrack = false;
          }
        }
        else if (thePacket.Id == (short)L001_Packet_Id.Pid_Trk_Data)
        {
          nrOfRecords++;
          SendUSBProgressChanged(nrOfRecords, nrOfExpectedRecords);
          if (validTrack) garminDevice.AddTracks(thePacket.Data, trackKey);
        }

        thePacket = GetPacket();
      }
      SendUSBProgressChanged(100, 100);
    }
    
    private void GetRuns()
    {
      // Retrive Runs
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Runs);

      SendUSBProgressChanged(0, 100);
      GarminUSBPacket thePacket = GetPacket();
      UInt16 nrOfExpectedRecords = 0;
      UInt16 nrOfRecords = 0;
      while (thePacket.Id != 0 && thePacket.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        if (thePacket.Id == (short)L001_Packet_Id.Pid_Records)
        {
          nrOfExpectedRecords = BitConverter.ToUInt16(thePacket.Data, 0);
        }
        else if (thePacket.Id == (short)L001_Packet_Id.Pid_Run)
        {
          nrOfRecords++;
          SendUSBProgressChanged(nrOfRecords, nrOfExpectedRecords);
          garminDevice.AddRuns(thePacket.Data);
        }
        thePacket = GetPacket();
      }
      SendUSBProgressChanged(100, 100);
    }
    
    private void GetDateTime()
    {
      // Retrieve Time
      SendA010Command((UInt16)A010_Command_Id_Type.Cmnd_Transfer_Time);

      SendUSBProgressChanged(0, 100);
      GarminUSBPacket thePacket = GetPacket();
      while (thePacket.Id != 0 && thePacket.Id != (short)L001_Packet_Id.Pid_Xfer_Cmplt)
      {
        if (thePacket.Id == (short)L001_Packet_Id.Pid_Date_Time_Data)
        {
          garminDevice.AddDateTime(thePacket.Data);
        }
        break;
      }
      SendUSBProgressChanged(100, 100);
    }

    public GarminDevice GarminDevice
    {
      set { garminDevice = value; }
      get { return garminDevice; }
    }

    public bool ReadCompleted
    {
      get { return readCompleted; }
    }

  }
}
