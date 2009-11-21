using System;
using System.Collections.Generic;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
  public enum Sport_Type
  {
    Running = 0,
    Biking = 1,
    Other
  }
  public enum Program_Type
  {
    None = 0,
    Virtual_Partner = 1,    /* Completed with Virtual Partner */
    Workout = 2,            /* Completed as part of a workout */
    Auto_Multisport = 3     /* Completed as part of an auto MultiSport */
  };
  public enum Multisport
  {
    No = 0,                 /* Not a MultiSport run */
    Yes = 1,                /* Part of a MultiSport session */
    YesAndLastInGroup = 2   /* The last of a MultiSport session */
  };
  // time_type = uint32


  public class GarminDevice
  {
    private UInt16 productId;
    private string productDescription;
    private Int16 softwareVersion;
    private List<D_Protocol_Data_Type> protocols;
    private List<D1001_Lap_Type> laps;
    private List<D500_Almanac_Type> almanac;
    private SortedList<int, List<D303_Trk_Point_Type>> tracks;
    private D700_Position_Type position;
    private List<D1010_Run_Type> runs;
    private D600_Date_Time_Type dateTime;

    /* Number of seconds since Dec 31, 1989, 12:00 AM (UTC) */
    public const int TIME_OFFSET = 631065600;

    public GarminDevice()
    {
      protocols = new List<D_Protocol_Data_Type>();
      laps = new List<D1001_Lap_Type>();
      almanac = new List<D500_Almanac_Type>();
      tracks = new SortedList<int, List<D303_Trk_Point_Type>>();
      runs = new List<D1010_Run_Type>();
    }
    public void AddProductData(byte[] data)
    {
      productId = BitConverter.ToUInt16(data, 0);
      softwareVersion = BitConverter.ToInt16(data, 2);
      char[] c = new char[data.Length - 4];
      Array.Copy(data, 4, c, 0, data.Length - 4);
      productDescription = GetStringFromNullTerminatedCharArray(c); ;
    }
    public void AddRuns(byte[] data)
    {
      D1010_Run_Type r = new D1010_Run_Type();
      //r.TrackIndex = BitConverter.ToUInt32(data, 0);                              // 4 bytes
      //r.FirstLapIndex = BitConverter.ToUInt32(data, 4);                           // 4 bytes
      //r.LastLapIndex = BitConverter.ToUInt32(data, 8);                            // 4 bytes
      // Changed by Mats Troeng 2008-03-04 : track index, first lap index and last lap index seem to be UInt16
      r.TrackIndex = BitConverter.ToUInt16(data, 0);                              // 2 bytes
      r.FirstLapIndex = BitConverter.ToUInt16(data, 2);                           // 2 bytes
      r.LastLapIndex = BitConverter.ToUInt16(data, 4);                            // 2 bytes
      if (data[6] == 0) { r.SportType = Sport_Type.Running; }                    // 1 bytes
      else if (data[6] == 1) { r.SportType = Sport_Type.Biking; }
      else if (data[6] == 2) { r.SportType = Sport_Type.Other; }
      if (data[7] == 0) { r.ProgramType = Program_Type.None; }                   // 1 byte
      else if (data[7] == 1) { r.ProgramType = Program_Type.Virtual_Partner; }
      else if (data[7] == 2) { r.ProgramType = Program_Type.Workout; }
      else if (data[7] == 3) { r.ProgramType = Program_Type.Auto_Multisport; }
      if (data[8] == 0) { r.Multisport = Multisport.No; }                        // 1 byte
      else if (data[8] == 1) { r.Multisport = Multisport.Yes; }
      else if (data[8] == 2) { r.Multisport = Multisport.YesAndLastInGroup; }
      // 1 byte unused...
      D_Virtual_partner vp = new D_Virtual_partner();
      vp.Time = BitConverter.ToUInt32(data, 16);                                  // 4 bytes
      vp.Distance = BitConverter.ToSingle(data, 20);                              // 4 bytes
      r.VirtualPartner = vp;

      D1002_Workout_Type wt = new D1002_Workout_Type();
      wt.NumValidSteps = BitConverter.ToUInt32(data, 24);                         // 4 bytes
      // Read some more data here later if needed...

      r.Workout = wt;

      this.runs.Add(r);
    }
    public void AddLaps(byte[] data)
    {
      D1001_Lap_Type lt = new D1001_Lap_Type();
      lt.Index = BitConverter.ToUInt32(data, 0);          // 4 bytes
      lt.StartTime = BitConverter.ToUInt32(data, 4);     // 4 bytes
      lt.TotalTime = BitConverter.ToUInt32(data, 8);     // 4 bytes
      lt.TotalDist = BitConverter.ToSingle(data, 12);    // 4 bytes
      lt.MaxSpeed = BitConverter.ToSingle(data, 16);     // 4 bytes

      D_Position_Type ptB = new D_Position_Type();
      ptB.Latitude = BitConverter.ToInt32(data, 20);           // 4 bytes
      ptB.Longitude = BitConverter.ToInt32(data, 24);           // 4 bytes
      lt.Begin = ptB;

      D_Position_Type ptE = new D_Position_Type();
      ptE.Latitude = BitConverter.ToInt32(data, 28);           // 4 bytes
      ptE.Longitude = BitConverter.ToInt32(data, 32);           // 4 bytes
      lt.End = ptE;

      lt.Calories = BitConverter.ToUInt16(data, 36);      // 2 bytes
      lt.AvgHeartRate = data[38];                       // 1 bytes
      lt.MaxHeartRate = data[39];                       // 1 bytes
      lt.Intensity = data[40];                            // 1 bytes

      this.laps.Add(lt);
    }
    public void AddTracks(byte[] data, int key)
    {
      D303_Trk_Point_Type tpt = new D303_Trk_Point_Type();

      D_Position_Type p = new D_Position_Type();
      p.Latitude = BitConverter.ToInt32(data, 0);         // 4 bytes
      p.Longitude = BitConverter.ToInt32(data, 4);         // 4 bytes

      tpt.Position = p;

      tpt.Time = BitConverter.ToUInt32(data, 8);     // 4 bytes
      tpt.Altitude = BitConverter.ToSingle(data, 12);     // 4 bytes
      tpt.HeartRate = data[20];                     // 1 bytes

      this.tracks[key].Add(tpt);
    }
    public void AddAlmanac(byte[] data)
    {
      D500_Almanac_Type at = new D500_Almanac_Type();
      at.Wn = BitConverter.ToUInt16(data, 0);         // 2 bytes
      at.Toa = BitConverter.ToSingle(data, 2);        // 4 bytes
      at.Af0 = BitConverter.ToSingle(data, 6);        // 4 bytes
      at.Af1 = BitConverter.ToSingle(data, 10);        // 4 bytes
      at.E = BitConverter.ToSingle(data, 14);          // 4 bytes
      at.Sqrta = BitConverter.ToSingle(data, 18);      // 4 bytes
      at.M0 = BitConverter.ToSingle(data, 22);         // 4 bytes
      at.W = BitConverter.ToSingle(data, 26);          // 4 bytes
      at.Omg0 = BitConverter.ToSingle(data, 30);       // 4 bytes
      at.Odot = BitConverter.ToSingle(data, 34);       // 4 bytes
      at.I = BitConverter.ToSingle(data, 38);          // 4 bytes

      almanac.Add(at);
    }
    public void AddPosition(byte[] data)
    {
      D700_Position_Type pt = new D700_Position_Type();
      pt.Latitude = BitConverter.ToDouble(data, 0);       // 8 bytes
      pt.Longitude = BitConverter.ToDouble(data, 8);      // 8 bytes
      position = pt;
    }
    public void AddDateTime(byte[] data)
    {
      D600_Date_Time_Type dtt = new D600_Date_Time_Type();
      dtt.Month = data[0];                        // 1 bytes
      dtt.Day = data[1];                          // 1 bytes
      dtt.Year = BitConverter.ToUInt16(data, 2);   // 2 bytes
      dtt.Hour = BitConverter.ToUInt16(data, 4);  // 2 bytes
      dtt.Minute = data[7];                       // 1 bytes
      dtt.Second = data[8];                       // 1 bytes
      dateTime = dtt;
    }
    public string GetSupportedProtocols()
    {
      string tempString = "";
      foreach (D_Protocol_Data_Type p in protocols)
      {
        tempString += Convert.ToChar(p.Tag).ToString() + " " + p.Data + "\r\n";
      }
      return tempString;
    }
    public UInt16 ProductId
    {
      set { productId = value; }
      get { return productId; }
    }
    public string ProductDescription
    {
      set { productDescription = value; }
      get { return productDescription; }
    }
    public Int16 SoftwareVersion
    {
      set { softwareVersion = value; }
      get { return softwareVersion; }
    }
    public List<D_Protocol_Data_Type> Protocols
    {
      set { protocols = value; }
      get { return protocols; }
    }
    public List<D1001_Lap_Type> Laps
    {
      set { laps = value; }
      get { return laps; }
    }
    public List<D500_Almanac_Type> Almanac
    {
      set { almanac = value; }
      get { return almanac; }
    }
    public SortedList<int, List<D303_Trk_Point_Type>> Tracks
    {
      set { tracks = value; }
      get { return tracks; }
    }
    public D700_Position_Type Position
    {
      set { position = value; }
      get { return position; }
    }
    public D600_Date_Time_Type DateTime
    {
      set { dateTime = value; }
      get { return dateTime; }
    }
    private string GetStringFromNullTerminatedCharArray(char[] c)
    {
      string s = new string(c);
      return s.Remove(s.IndexOf("\0"));
    }
    public static DateTime GetDateTimeFromElapsedSeconds(UInt32 secElapsed)
    {
      DateTime d = new DateTime(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc);
      return d.AddSeconds(secElapsed);
    }
    public SortedList<DateTime, int> GetAllRuns()
    {
      SortedList<DateTime, int> activities = new SortedList<DateTime, int>();
      int i = 0;
      foreach (D1010_Run_Type run in runs)
      {
        // No tracks if trackIndex = 65535
        if (run.TrackIndex != 65535)
        {
          D1001_Lap_Type firstLap = GetLap((int)run.FirstLapIndex);
          if (firstLap != null && !activities.ContainsKey(firstLap.StartTimeAsDateTime)) activities.Add(firstLap.StartTimeAsDateTime, i);
        }
        i++;
      }
      return activities;
    }

    private D1001_Lap_Type GetLap(int index)
    {
      foreach (D1001_Lap_Type lap in laps)
      {
        if (lap.Index == index)
        {
          return lap;
        }
      }
      return null;
    }

    private List<D1001_Lap_Type> GetLapsForRun(int index)
    {
      return GetLapsForRun(runs[index]);
    }

    private List<D1001_Lap_Type> GetLapsForRun(D1010_Run_Type run)
    {
      List<D1001_Lap_Type> Laps = new List<D1001_Lap_Type>();
      foreach (D1001_Lap_Type lap in laps)
      {
        if (lap.Index >= run.FirstLapIndex && lap.Index <= run.LastLapIndex)
        {
          Laps.Add(lap);
        }
      }
      return Laps;
    }

    private List<D303_Trk_Point_Type> GetTrackpointsForRun(int index)
    {
      return GetTrackpointsForRun(runs[index]);
    }

    private List<D303_Trk_Point_Type> GetTrackpointsForRun(D1010_Run_Type run)
    {
      return tracks.ContainsKey((int)run.TrackIndex) ? tracks[(int)run.TrackIndex] : new List<D303_Trk_Point_Type>();
    }

    public List<GarminSession> Sessions
    {
      get
      {
        List<GarminSession> list = new List<GarminSession>();
        foreach (D1010_Run_Type run in runs)
        {
          // No tracks if trackIndex = 65535
          if (run.TrackIndex != 65535 && tracks.ContainsKey((int)run.TrackIndex))
          {
            list.Add(new GarminSession(run, GetLapsForRun(run), GetTrackpointsForRun(run)));
          }
        }
        list.Sort();
        list.Reverse();
        return list;
      }
    }

  }

  public class GarminSession : IComparable<GarminSession>, IComparable 
  {
    private D1010_Run_Type run;
    private List<D1001_Lap_Type> laps;
    private List<D303_Trk_Point_Type> trackpoints;

    public GarminSession(D1010_Run_Type run, List<D1001_Lap_Type> laps, List<D303_Trk_Point_Type> trackpoints)
    {
      this.run = run;
      this.laps = laps;
      this.trackpoints = trackpoints; 
    }

    public D1010_Run_Type Run
    {
      get { return run; }
    }

    public List<D1001_Lap_Type> Laps
    {
      get { return laps; }
    }

    public List<D303_Trk_Point_Type> Trackpoints
    {
      get { return trackpoints; }
    }

    #region IComparable<GarminSession> Members

    public int CompareTo(GarminSession other)
    {
      bool hasLaps = (laps != null && laps.Count > 0);
      bool otherHasLaps = (other.laps != null && other.laps.Count > 0);

      if (!hasLaps && !otherHasLaps) return 0;
      if (!hasLaps && otherHasLaps) return 1;
      if (hasLaps && !otherHasLaps) return -1;

      return laps[0].StartTimeAsDateTime.CompareTo(other.laps[0].StartTimeAsDateTime); 
    }

    #endregion

    #region IComparable Members

    public int CompareTo(object obj)
    {
      GarminSession other = obj as GarminSession;
      if (other != null)
      {
        return CompareTo(other);
      }
      else
      {
        return -1;
      }

    }

    #endregion

    public override string ToString()
    {
      if (laps != null && laps.Count > 0)
      {
        return laps[0].StartTimeAsDateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"); 
      }
      else
      {
        return "";
      }
    }
  }

}
