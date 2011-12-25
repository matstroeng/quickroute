using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Importers.GPX.GPX11;
using QuickRoute.BusinessEntities.Importers.Polar;
using QuickRoute.BusinessEntities.Importers.Polar.ProTrainer;

namespace QuickRoute.BusinessEntities.Importers.FIT
{
  public class FITImporter : IRouteFileImporter
  {
    public ImportResult ImportResult { get; set; }

    public string FileName { get; set; }

    #region IRouteImporter Members

    public event EventHandler<EventArgs> BeginWork;

    public event EventHandler<EventArgs> EndWork;

    public event EventHandler<WorkProgressEventArgs> WorkProgress;

    #endregion

    public DialogResult ShowPreImportDialogs()
    {
      return DialogResult.OK;
    }

    public void Import()
    {
      ImportResult = new ImportResult();
      if (BeginWork != null) BeginWork(this, new EventArgs());

      try
      {
        using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
        {
          using (var reader = new BinaryReader(stream))
          {
            var header = new Header(reader);
            var data = new Data(reader, header.DataSize);

            // route
            var routeSegment = new RouteSegment();
            foreach(var w in data.Waypoints)
            {
              if(routeSegment.Waypoints.Count == 0 || routeSegment.LastWaypoint.Time < w.Time)
              {
                routeSegment.Waypoints.Add(new Waypoint(w.Time, new LongLat(w.Longitude, w.Latitude), w.Altitude, w.HeartRate, null));  
              }
            }
            ImportResult.Route = new Route(new List<RouteSegment>() { routeSegment });
            
            // laps
            if (routeSegment.Waypoints.Count > 1)
            {
              ImportResult.Laps = new LapCollection();
              ImportResult.Laps.Add(new Lap(routeSegment.FirstWaypoint.Time, LapType.Start));
              foreach (var l in data.Laps)
              {
                if (l.Time > routeSegment.FirstWaypoint.Time && l.Time < routeSegment.LastWaypoint.Time)
                ImportResult.Laps.Add(new Lap(l.Time, LapType.Lap));
              }
              ImportResult.Laps.Add(new Lap(routeSegment.LastWaypoint.Time, LapType.Stop));
            }

            ImportResult.Succeeded = true;
          }
        }
      }
      catch (Exception ex)
      {
        ImportResult.Exception = ex;
      }
      if (EndWork != null) EndWork(this, new EventArgs());
    }

    private class Data
    {
      private const double positionFactor = (double)180 / 0x7FFFFFFF;
      private const Int32 invalidInt32 = 0x7FFFFFFF;
      private const UInt16 invalidUInt16 = 0xFFFF;
      private const byte invalidByte = 0xFF;

      public List<FITWaypoint> Waypoints { get; private set; }
      public List<FITLap> Laps { get; private set; }

      public Data(BinaryReader reader, UInt32 dataSize)
      {
        Waypoints = new List<FITWaypoint>();
        Laps = new List<FITLap>();
        var bytes = reader.ReadBytes((int)dataSize);
        using (var stream = new MemoryStream(bytes))
        {
          using (var dataReader = new BinaryReader(stream))
          {
            var recordHeader = new RecordHeader(dataReader);

            if (!recordHeader.IsDefinitionMessage)
            {
              throw new Exception("First record is not a definition message.");
            }
            var def = new DefinitionMessage(dataReader);
            if (def.GlobalMessageNumber != (ushort) MesgNum.file_id)
              throw new Exception("First record's global message number is not file_id.");
            recordHeader = new RecordHeader(dataReader);
            if (recordHeader.IsDefinitionMessage)
              throw new Exception("Encountered a definition message, but expected a data message.");
            var d = new DataMessage(dataReader, def);
            var fileType = d.GetByte(0);
            if (fileType != 4) throw new Exception("Not a FIT activity file.");

            var messageTypeTranslator = new Dictionary<byte, DefinitionMessage>();
            UInt32 lastTimestamp = 0;

            while (dataReader.BaseStream.Position < dataReader.BaseStream.Length)
            {
              recordHeader = new RecordHeader(dataReader);
              if (recordHeader.IsDefinitionMessage)
              {
                def = new DefinitionMessage(dataReader);
                FITUtil.AddOrReplace(messageTypeTranslator, recordHeader.LocalMessageType, def);
              }
              else
              {
                var currentDef = messageTypeTranslator[recordHeader.LocalMessageType];
                d = new DataMessage(dataReader, currentDef);

                var timestamp = d.GetUInt32(253);
                if (timestamp == null) timestamp = FITUtil.AddCompressedTimestamp(lastTimestamp, recordHeader.TimeOffset);
                var time = FITUtil.ToDateTime(timestamp.Value);

                var gmn = currentDef.GlobalMessageNumber;
                if (gmn == (byte) MesgNum.record)
                {
                  var lat = d.GetInt32(0);
                  var lng = d.GetInt32(1);
                  var alt = d.GetUInt16(2);
                  var hr = d.GetByte(3);
                  if (lng != null && lng != invalidInt32 && lat != null && lat != invalidInt32)
                  {
                    Waypoints.Add(new FITWaypoint()
                                    {
                                      Time = time,
                                      Latitude = positionFactor * lat.Value,
                                      Longitude = positionFactor * lng.Value,
                                      Altitude = alt == null || alt == invalidUInt16 ? (double?)null : (double)alt.Value / 5 - 500,
                                      HeartRate = hr == null || hr == invalidByte ? null : hr
                                    });
                  }
                }
                else if (gmn == (byte)MesgNum.lap)
                {
                  Laps.Add(new FITLap() { Time = time });
                }
                else if (gmn == 22)
                {
                    
                }
                lastTimestamp = timestamp.Value;
              }
            }
          }
        }
      }
    }

    private class RecordHeader
    {
      public bool IsDefinitionMessage { get; set; }
      public bool IsNormalHeader { get; set; }
      public byte LocalMessageType { get; set; }
      public byte TimeOffset { get; set; }

      public RecordHeader(BinaryReader reader)
      {
        var b = reader.ReadByte();
        IsDefinitionMessage = ((b >> 6) & 1) == 1;
        IsNormalHeader = ((b >> 7) & 1) == 0;
        if (IsNormalHeader)
        {
          LocalMessageType = (byte)(b & 0xF);
        }
        else
        {
          LocalMessageType = (byte)((b >> 5) & 0x3);
          TimeOffset = (byte)(b & 0x1F);
        }
      }
    }

    private class Field
    {
      public byte FieldDefinitionNumber { get; set; }
      public byte Size { get; set; }
      public byte BaseType { get; set; }
      public int Position { get; set; }
    }

    private class DefinitionMessage
    {
      public byte Architecture { get; set; }
      public UInt16 GlobalMessageNumber { get; set; }
      public Field[] Fields { get; set; }
      public int DataLength { get; set; }

      public DefinitionMessage(BinaryReader dataReader)
      {
        dataReader.ReadByte(); // reserved
        Architecture = dataReader.ReadByte();
        GlobalMessageNumber = FITUtil.ChangeEndianness(dataReader.ReadUInt16(), Architecture);
        var numberOfFields = dataReader.ReadByte();
        Fields = new Field[numberOfFields];
        for (var i = 0; i < numberOfFields; i++)
        {
          Fields[i] = new Field()
                        {
                          FieldDefinitionNumber = dataReader.ReadByte(),
                          Size = dataReader.ReadByte(),
                          BaseType = dataReader.ReadByte(),
                          Position = DataLength
                        };
          DataLength += Fields[i].Size;
        }
      }
    }

    private class DataMessage
    {
      private readonly DefinitionMessage definitionMessage;
      private readonly byte[] data;
      public DataMessage(BinaryReader dataReader, DefinitionMessage definitionMessage)
      {
        this.definitionMessage = definitionMessage;
        data = dataReader.ReadBytes(definitionMessage.DataLength);
      }

      public byte? GetByte(int fieldDefinitionNumber)
      {
        var field = GetFieldByFieldDefinitionNumber(fieldDefinitionNumber);
        return field == null
                 ? (byte?)null
                 : data[field.Position];
      }

      public UInt16? GetUInt16(int fieldDefinitionNumber)
      {
        var field = GetFieldByFieldDefinitionNumber(fieldDefinitionNumber);
        return field == null
                 ? (UInt16?)null
                 : FITUtil.ChangeEndianness(BitConverter.ToUInt16(data, field.Position), definitionMessage.Architecture);
      }

      public UInt32? GetUInt32(int fieldDefinitionNumber)
      {
        var field = GetFieldByFieldDefinitionNumber(fieldDefinitionNumber);
        return field == null
                 ? (UInt32?)null
                 : FITUtil.ChangeEndianness(BitConverter.ToUInt32(data, field.Position), definitionMessage.Architecture);
      }

      public Int32? GetInt32(int fieldDefinitionNumber)
      {
        var field = GetFieldByFieldDefinitionNumber(fieldDefinitionNumber);
        return field == null
                 ? (Int32?)null
                 : FITUtil.ChangeEndianness(BitConverter.ToInt32(data, field.Position), definitionMessage.Architecture);
      }

      private Field GetFieldByFieldDefinitionNumber(int fieldDefinitionNumber)
      {
        foreach (var f in definitionMessage.Fields)
        {
          if (f.FieldDefinitionNumber == fieldDefinitionNumber) return f;
        }
        return null;
      }
    }

    private class Header
    {
      public Header(BinaryReader reader)
      {
        Size = reader.ReadByte();
        var headerBytes = reader.ReadBytes(Size - 1);

        var crc = CalculateCrc(0, Size);
        for (var i = 0; i < 11; i++)
        {
          crc = CalculateCrc(crc, headerBytes[i]);
        }

        using (var headerStream = new MemoryStream(headerBytes))
        {
          using (var headerReader = new BinaryReader(headerStream))
          {
            ProtocolVersion = headerReader.ReadByte();
            ProfileVersion = headerReader.ReadUInt16();
            DataSize = headerReader.ReadUInt32();
            DataType = Encoding.ASCII.GetString(headerReader.ReadBytes(4));
            if (headerReader.BaseStream.Position < headerBytes.Length)
            {
              Crc = headerReader.ReadUInt16();
            }
          }
        }
        CrcIsValid = Crc == 0 /* sometimes crc is set to zero in header */ || Crc == crc;
      }

      public byte Size { get; set; }
      public byte ProtocolVersion { get; set; }
      public UInt16 ProfileVersion { get; set; }
      public UInt32 DataSize { get; set; }
      public string DataType { get; set; }
      public UInt16 Crc { get; set; }
      public bool CrcIsValid { get; private set; }

      private readonly UInt16[] crcTable = new UInt16[]
                                     {
                                       0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401,
                                       0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400
                                     };

      private UInt16 CalculateCrc(UInt16 crc, byte b)
      {
        // compute checksum of lower four bits of byte 
        var tmp = crcTable[crc & 0xF];
        crc = (UInt16)((crc >> 4) & 0x0FFF);
        crc = (UInt16)(crc ^ tmp ^ crcTable[b & 0xF]);

        // now compute checksum of upper four bits of byte 
        tmp = crcTable[crc & 0xF];
        crc = (UInt16)((crc >> 4) & 0x0FFF);
        crc = (UInt16)(crc ^ tmp ^ crcTable[(b >> 4) & 0xF]);
        return crc;
      }

    }

    private class FITWaypoint
    {
      public DateTime Time { get; set; }
      public double Latitude { get; set; }
      public double Longitude { get; set; }
      public int? HeartRate { get; set; }
      public double? Altitude { get; set; }
    }

    private class FITLap
    {
      public DateTime Time { get; set; }
    }

    private enum MesgNum : ushort 
    {
      file_id = 0,
      capabilities = 1,
      device_settings = 2,
      user_profile = 3,
      hrm_profile = 4,
      sdm_profile = 5,
      bike_profile = 6,
      zones_target = 7,
      hr_zone = 8,
      power_zone = 9,
      met_zone = 10,
      sport = 12,
      goal = 15,
      session = 18,
      lap = 19,
      record = 20,
      @event = 21,
      device_info = 23,
      workout = 26,
      workout_step = 27,
      weight_scale = 30,
      course = 31,
      course_point = 32,
      totals = 33,
      activity = 34,
      software = 35,
      file_capabilities = 37,
      mesg_capabilities = 38,
      field_capabilities = 39,
      file_creator = 49,
      blood_pressure = 51,
      hrv = 78,
      pad = 105
    }

    private enum File : byte
    {
      device = 1,
      settings = 2,
      sport = 3,
      activity = 4,
      workout = 5,
      course = 6,
      weight = 9,
      totals = 10,
      goals = 11,
      blood_pressure = 14,
      activity_summary = 20,
    }
  }
}
