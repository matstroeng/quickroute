using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using QuickRoute.BusinessEntities.Exporters;

namespace QuickRoute.BusinessEntities
{
  public class QuickRouteJpegExtensionData
  {
    private static readonly byte[] quickRouteIdentifier = new byte[] { 0x51, 0x75, 0x69, 0x63, 0x6b, 0x52, 0x6f, 0x75, 0x74, 0x65 };
    private const string currentVersion = "1.0.0.0";

    public string Version { get; set; }
    public LongLat[] MapCornerPositions { get; set; }
    public LongLat[] ImageCornerPositions { get; set; }
    public Rectangle MapLocationAndSizeInPixels { get; set; }
    public SessionCollection Sessions { get; set; }
    public double PercentualSize { get; set; }

    public static QuickRouteJpegExtensionData FromQuickRouteDocument(Document document)
    {
      var data = new QuickRouteJpegExtensionData {Version = currentVersion, Sessions = document.Sessions, PercentualSize = 1};
      return data;
    }

    public static QuickRouteJpegExtensionData FromQuickRouteDocument(Document document, Rectangle imageBounds, Rectangle mapBounds, double percentualSize)
    {
      return FromQuickRouteDocument(document, document.Sessions, imageBounds, mapBounds, percentualSize);
    }

    public static QuickRouteJpegExtensionData FromQuickRouteDocument(Document document, SessionCollection sessions, Rectangle imageBounds, Rectangle mapBounds, double percentualSize)
    {
      var data = new QuickRouteJpegExtensionData
                   {
                     Version = currentVersion,
                     ImageCornerPositions = document.GetImageCornersLongLat(imageBounds, mapBounds, percentualSize),
                     MapCornerPositions = document.GetMapCornersLongLat(),
                     MapLocationAndSizeInPixels = mapBounds,
                     Sessions = sessions,
                     PercentualSize = percentualSize
                   };
      return data;
    }

    public static QuickRouteJpegExtensionData FromImageExporter(ImageExporter exporter)
    {
      var data = new QuickRouteJpegExtensionData
                   {
                     Version = currentVersion,
                     ImageCornerPositions =
                       exporter.Document.GetImageCornersLongLat(exporter.ImageBounds, exporter.MapBounds,
                                                                exporter.Properties.PercentualSize),
                     MapCornerPositions = exporter.Document.GetMapCornersLongLat(),
                     MapLocationAndSizeInPixels = exporter.MapBounds,
                     Sessions = exporter.Sessions,
                     PercentualSize = exporter.Properties.PercentualSize
                   };
      return data;
    }

    /// <summary>
    /// Creates a QuickRouteJpegExtensionData object from a stream containing a byte array with image data.
    /// </summary>
    /// <param name="stream">A stream containing the byte array with data</param>
    /// <returns></returns>
    public static QuickRouteJpegExtensionData FromStream(Stream stream)
    {
      var edStream = new MemoryStream();

      // find APP0 QuickRoute marker
      var buffer = new byte[2];
      stream.Read(buffer, 0, 2);
      if (buffer[0] == 0xff && buffer[1] == 0xd8) //SOI
      {
        // try to find QuickRoute Jpeg Extension Data block
        while (stream.Position < stream.Length)
        {
          stream.Read(buffer, 0, 2);
          if (buffer[0] != 0xff) break; // we have reached image data
          if (buffer[1] == 0xe0) // APP0
          {
            // found app0 marker, read length of block
            stream.Read(buffer, 0, 2);
            var length = buffer[1] + 256 * buffer[0]; // little-endian
            if (length >= 2 + quickRouteIdentifier.Length)
            {
              var identifier = new byte[quickRouteIdentifier.Length];
              stream.Read(identifier, 0, quickRouteIdentifier.Length);
              var equal = true;
              for (var i = 0; i < quickRouteIdentifier.Length; i++)
              {
                if (identifier[i] != quickRouteIdentifier[i])
                {
                  equal = false;
                  break;
                }
              }
              if (equal)
              {
                var block = new byte[length - 2 - quickRouteIdentifier.Length];
                stream.Read(block, 0, block.Length);
                edStream.Write(block, 0, block.Length);
              }
              else
              {
                stream.Position += length - 2 - quickRouteIdentifier.Length;
                if (edStream.Length > 0) break;
              }
            }
            else
            {
              stream.Position += length - 2;
              if (edStream.Length > 0) break;
            }
          }
          else
          {
            if (edStream.Length > 0) break;
          }
        }
      }

      return edStream.Length > 0 ? ParseStream(edStream) : null;
    }

    /// <summary>
    /// Creates a QuickRouteJpegExtensionData object from a jpeg file
    /// </summary>
    /// <param name="fileName">The file name of the jpeg file</param>
    /// <returns></returns>
    public static QuickRouteJpegExtensionData FromJpegFile(string fileName)
    {
      var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      var ed = FromStream(stream);
      stream.Close();
      stream.Dispose();
      return ed;
    }

    private static QuickRouteJpegExtensionData ParseStream(Stream stream)
    {
      var data = new QuickRouteJpegExtensionData {PercentualSize = 1};
      stream.Position = 0;
      var edReader = new BinaryReader(stream);

      while (stream.Position < stream.Length)
      {
        var tag = (Tags)edReader.ReadByte();
        var tagLength = Convert.ToInt32(edReader.ReadUInt32());
        switch (tag)
        {
          case Tags.Version:
            data.Version =
              edReader.ReadByte() + "." +
              edReader.ReadByte() + "." +
              edReader.ReadByte() + "." +
              edReader.ReadByte();
            break;

          case Tags.MapCornerPositions:
            data.MapCornerPositions = new LongLat[4];
            data.MapCornerPositions[0] = ReadLongLat(edReader);
            data.MapCornerPositions[1] = ReadLongLat(edReader);
            data.MapCornerPositions[2] = ReadLongLat(edReader);
            data.MapCornerPositions[3] = ReadLongLat(edReader);
            break;

          case Tags.ImageCornerPositions:
            data.ImageCornerPositions = new LongLat[4];
            data.ImageCornerPositions[0] = ReadLongLat(edReader);
            data.ImageCornerPositions[1] = ReadLongLat(edReader);
            data.ImageCornerPositions[2] = ReadLongLat(edReader);
            data.ImageCornerPositions[3] = ReadLongLat(edReader);
            break;

          case Tags.MapLocationAndSizeInPixels:
            var rect = new Rectangle
                         {
                           X = edReader.ReadUInt16(),
                           Y = edReader.ReadUInt16(),
                           Width = edReader.ReadUInt16(),
                           Height = edReader.ReadUInt16()
                         };
            data.MapLocationAndSizeInPixels = rect;
            break;

          case Tags.Sessions:
            data.Sessions = ReadSessions(edReader);
            break;

          default:
            edReader.BaseStream.Position += tagLength;
            break;
        }
      }
      return data;
    }
    
    /// <summary>
    /// Places this object's data into a jpeg stream.
    /// </summary>
    /// <param name="sourceImageStream">The original stream where the image resides</param>
    /// <param name="targetImageStream">The stream to be returned, including the embedded data</param>
    public void EmbedDataInImage(Stream sourceImageStream, Stream targetImageStream)
    {
      var buffer = new byte[2];
      sourceImageStream.Position = 0;
      sourceImageStream.Read(buffer, 0, 2);
      if (buffer[0] == 0xff && buffer[1] == 0xd8) //SOI
      {
        sourceImageStream.Read(buffer, 0, 2);
        if (buffer[0] == 0xff && buffer[1] == 0xe0) // APP0
        {
          // read first APP0 block and write it to the target stream
          sourceImageStream.Read(buffer, 0, 2);
          var length = buffer[1] + 256 * buffer[0]; // little-endian
          sourceImageStream.Position += length - 2;
          int pos = Convert.ToInt32(sourceImageStream.Position);
          var tmpBuffer = new byte[pos];
          sourceImageStream.Position = 0;
          sourceImageStream.Read(tmpBuffer, 0, pos);
          targetImageStream.Write(tmpBuffer, 0, pos);

          // write the QuickRoute Jpeg Extension Data to the target stream
          var data = CreateQuickRouteJpegExtensionData();
          var maxBlockLength = 65523;
          length = data.Length;
          var lengthLeft = length;
          do // we may need to use several blocks
          {
            var blockLength = Math.Min(lengthLeft, maxBlockLength);
            targetImageStream.Write(new byte[] { 0xff, 0xe0 }, 0, 2);
            targetImageStream.Write(new byte[] { Convert.ToByte((blockLength + 12) / 256), Convert.ToByte((blockLength + 12) % 256) }, 0, 2);
            targetImageStream.Write(quickRouteIdentifier, 0, quickRouteIdentifier.Length);
            targetImageStream.Write(data, length - lengthLeft, blockLength);
            lengthLeft -= blockLength;
          } while (lengthLeft > 0);

          // write remaining blocks, but exclude any QuickRoute Jpeg Extension Data blocks
          while (sourceImageStream.Position < sourceImageStream.Length)
          {
            // TODO: fetch APP1 and other blocks that have a specified length as well
            sourceImageStream.Read(buffer, 0, 2);
            if (!(buffer[0] == 0xff && buffer[1] == 0xe0))
            {
              // we have reached start of image data, just write it to target stream
              sourceImageStream.Position -= 2;
              tmpBuffer = new byte[sourceImageStream.Length - sourceImageStream.Position];
              sourceImageStream.Read(tmpBuffer, 0, tmpBuffer.Length);
              targetImageStream.Write(tmpBuffer, 0, tmpBuffer.Length);
              break;
            }
            else
            {
              // found app0 marker, read length of block
              var isQuickRouteBlock = true;
              sourceImageStream.Read(buffer, 0, 2);
              length = buffer[1] + 256 * buffer[0]; // little-endian
              if (length >= 2 + quickRouteIdentifier.Length)
              {
                var identifier = new byte[quickRouteIdentifier.Length];
                sourceImageStream.Read(identifier, 0, quickRouteIdentifier.Length);
                for (var i = 0; i < quickRouteIdentifier.Length; i++)
                {
                  if (identifier[i] != quickRouteIdentifier[i])
                  {
                    isQuickRouteBlock = false;
                    break;
                  }
                }
                if (isQuickRouteBlock)
                {
                  sourceImageStream.Position += length - 2 - quickRouteIdentifier.Length;
                }
                else
                {
                  sourceImageStream.Position -= 2 + 2 + quickRouteIdentifier.Length;
                }
              }
              else
              {
                sourceImageStream.Position -= 4;
              }

              if (!isQuickRouteBlock)
              {
                tmpBuffer = new byte[length + 2];
                sourceImageStream.Read(tmpBuffer, 0, tmpBuffer.Length);
                targetImageStream.Write(tmpBuffer, 0, tmpBuffer.Length);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Exports the QuickRouteJpegExtensionData object to a GPX file.
    /// </summary>
    /// <param name="stream">The stream to export the gpx file to.backbuffer</param>
    public void ToGpx(Stream stream)
    {
      var gpxExporter = new GpxExporter(Sessions[0], stream);
      gpxExporter.Export();
    }

    private static SessionCollection ReadSessions(BinaryReader reader)
    {
      var sessions = new SessionCollection();
      var sessionCount = reader.ReadUInt32();
      for (int i = 0; i < sessionCount; i++)
      {
        var tag = (Tags)reader.ReadByte();
        var tagLength = Convert.ToInt32(reader.ReadUInt32());
        switch (tag)
        {
          case Tags.Session:
            sessions.Add(ReadSession(reader, tagLength));
            break;

          default:
            reader.BaseStream.Position += tagLength;
            break;
        }
      }
      return sessions;
    }

    private static Session ReadSession(BinaryReader reader, int length)
    {
      List<DateTime> mapReadingList = null;
      Route route = null;
      HandleCollection handles = null;
      LongLat projectionOrigin = null;
      LapCollection laps = null;
      var startPos = reader.BaseStream.Position;
      SessionInfo sessionInfo = null;
      DateTime lastTime;
      while (reader.BaseStream.Position < startPos + length)
      {
        var tag = (Tags)reader.ReadByte();
        var tagLength = Convert.ToInt32(reader.ReadUInt32());
        switch (tag)
        {
          case Tags.Route:
            var attributes = reader.ReadUInt16();
            var extraWaypointAttributesLength = reader.ReadUInt16();
            var routeSegments = new List<RouteSegment>();
            var segmentCount = reader.ReadUInt32();
            lastTime = DateTime.MinValue;
            for (var i = 0; i < segmentCount; i++)
            {
              var rs = new RouteSegment();
              var waypointCount = reader.ReadUInt32();
              for (var j = 0; j < waypointCount; j++)
              {
                var w = new Waypoint();
                w.LongLat = ReadLongLat(reader);
                w.Time = ReadTime(lastTime, reader);
                lastTime = w.Time;
                if ((attributes & (UInt16)WaypointAttribute.HeartRate) == (UInt16)WaypointAttribute.HeartRate)
                {
                  w.HeartRate = reader.ReadByte();
                }
                if ((attributes & (UInt16)WaypointAttribute.Altitude) == (UInt16)WaypointAttribute.Altitude)
                {
                  w.Altitude = reader.ReadInt16();
                }
                reader.BaseStream.Position += extraWaypointAttributesLength; // for forward compatibility
                rs.Waypoints.Add(w);
              }
              routeSegments.Add(rs);
            }
            route = new Route(routeSegments);
            break;

          case Tags.Handles:
            handles = new HandleCollection();
            var handleCount = reader.ReadUInt32();
            var handleMarkerDrawer = SessionSettings.CreateDefaultMarkerDrawers()[MarkerType.Handle];
            for (var i = 0; i < handleCount; i++)
            {
              var handle = new Handle();
              // transformation matrix
              handle.TransformationMatrix = new GeneralMatrix(3, 3);
              for (var j = 0; j < 9; j++)
              {
                handle.TransformationMatrix.SetElement(j / 3, j % 3, reader.ReadDouble());
              }
              // parameterized location
              var segmentIndex = Convert.ToInt32(reader.ReadUInt32());
              var value = reader.ReadDouble();
              handle.ParameterizedLocation = new ParameterizedLocation(segmentIndex, value);

              // pixel location
              handle.Location = new PointD(reader.ReadDouble(), reader.ReadDouble());
              // type
              handle.Type = (Handle.HandleType)reader.ReadInt16();
              // use default marker drawer
              handle.MarkerDrawer = handleMarkerDrawer;

              handles.Add(handle);
            }
            break;

          case Tags.ProjectionOrigin:
            projectionOrigin = ReadLongLat(reader);
            break;

          case Tags.Laps:
            laps = new LapCollection();
            var lapCount = reader.ReadUInt32();
            for (var i = 0; i < lapCount; i++)
            {
              var lap = new Lap();
              lap.Time = DateTime.FromBinary(reader.ReadInt64());
              lap.LapType = (LapType)reader.ReadByte();
              laps.Add(lap);
            }
            break;

          case Tags.SessionInfo:
            sessionInfo = new SessionInfo();
            sessionInfo.Person = new SessionPerson();
            sessionInfo.Person.Name = ReadString(reader);
            sessionInfo.Person.Club = ReadString(reader);
            sessionInfo.Person.Id = reader.ReadUInt32();
            sessionInfo.Description = ReadString(reader);
            // when more fields are added, check so that tagLength is not passed
            break;

          case Tags.MapReadingInfo:
            mapReadingList = new List<DateTime>();
            lastTime = DateTime.MinValue;
            var startPosition = reader.BaseStream.Position;
            while (reader.BaseStream.Position - startPosition < tagLength)
            {
              var time = ReadTime(lastTime, reader);
              mapReadingList.Add(time);
              lastTime = time;
            }
            break;

          default:
            reader.BaseStream.Position += tagLength;
            break;
        }
      }

      if(mapReadingList != null && route != null) route = new Route(Route.AddMapReadingWaypoints(route.Segments, mapReadingList));
      var session = new Session(
        route, 
        laps,
        new Size(0, 0), 
        handles != null && handles.Count > 0 ? handles[0].TransformationMatrix : null, 
        projectionOrigin, 
        new SessionSettings());
      if (handles != null)
      {
        foreach (var h in handles)
        {
          session.AddHandle(h);
        }
      }
      if (sessionInfo != null) session.SessionInfo = sessionInfo;

      return session;
    }

    private static DateTime ReadTime(DateTime lastTime, BinaryReader reader)
    {
      var timeType = (TimeType)reader.ReadByte();
      switch (timeType)
      {
        case TimeType.Full:
          return DateTime.FromBinary(reader.ReadInt64());
        default: //case TimeType.Delta:
          return lastTime.AddSeconds((double)reader.ReadUInt16() / 1000);
      }
    }

    private static LongLat ReadLongLat(BinaryReader br)
    {
      var longLat = new LongLat();
      longLat.Longitude = (double)br.ReadInt32() / 3600000;
      longLat.Latitude = (double)br.ReadInt32() / 3600000;
      return longLat;
    }

    private static void WriteLongLat(LongLat longLat, BinaryWriter bw)
    {
      bw.Write(Convert.ToInt32(longLat.Longitude * 3600000));
      bw.Write(Convert.ToInt32(longLat.Latitude * 3600000));
    }

    private byte[] CreateQuickRouteJpegExtensionData()
    {
      var stream = new MemoryStream();
      var writer = new BinaryWriter(stream);

      // version, 4 bytes
      writer.Write((byte)Tags.Version);
      writer.Write((UInt32)4);
      writer.Write(new byte[] { 1, 0, 0, 0 });

      // image corners
      writer.Write((byte)Tags.ImageCornerPositions);
      writer.Write((UInt32)32);
      foreach (var corner in ImageCornerPositions)
      {
        WriteLongLat(corner, writer);
      }

      // map corners
      writer.Write((byte)Tags.MapCornerPositions);
      writer.Write((UInt32)32);
      foreach (var corner in MapCornerPositions)
      {
        WriteLongLat(corner, writer);
      }

      // bounds of the map portion of the image
      writer.Write((byte)Tags.MapLocationAndSizeInPixels);
      writer.Write((UInt32)8);
      writer.Write((UInt16)MapLocationAndSizeInPixels.X);
      writer.Write((UInt16)MapLocationAndSizeInPixels.Y);
      writer.Write((UInt16)MapLocationAndSizeInPixels.Width);
      writer.Write((UInt16)MapLocationAndSizeInPixels.Height);

      // sessions
      var sessionsStream = new MemoryStream();
      var sessionsWriter = new BinaryWriter(sessionsStream);
      sessionsWriter.Write((UInt32)Sessions.Count);
      foreach (var session in Sessions)
      {
        var sessionData = CreateSessionData(session);
        sessionsWriter.Write((byte)Tags.Session);
        sessionsWriter.Write((UInt32)sessionData.Length);
        sessionsWriter.Write(sessionData);
      }
      writer.Write((byte)Tags.Sessions);
      writer.Write((UInt32)sessionsStream.Length);
      writer.Write(sessionsStream.ToArray());
      sessionsWriter.Close();
      sessionsStream.Close();
      sessionsStream.Dispose();

      var data = stream.ToArray();
      writer.Close();
      stream.Close();
      stream.Dispose();
      return data;
    }

    private byte[] CreateSessionData(Session session)
    {
      var sessionStream = new MemoryStream();
      var sessionWriter = new BinaryWriter(sessionStream);
      // route
      var routeStream = new MemoryStream();
      var routeWriter = new BinaryWriter(routeStream);
      
      // which attributes to include for each waypoint
      var attributes = WaypointAttribute.Position | WaypointAttribute.Time;
      if (session.Route.ContainsWaypointAttribute(BusinessEntities.WaypointAttribute.HeartRate))
        attributes |= WaypointAttribute.HeartRate;
      if (session.Route.ContainsWaypointAttribute(BusinessEntities.WaypointAttribute.Altitude))
        attributes |= WaypointAttribute.Altitude;
      routeWriter.Write((UInt16)attributes);
      // any extra length in bytes for future elements for each waypoint
      routeWriter.Write((UInt16)0);
      // number of route segments in this route
      routeWriter.Write((UInt32)session.Route.Segments.Count);

      foreach (var routeSegment in session.Route.Segments)
      {
        // number of waypoints in this route segment
        routeWriter.Write((UInt32)routeSegment.Waypoints.Count);
        var lastTime = DateTime.MinValue;
        foreach (var waypoint in routeSegment.Waypoints)
        {
          // position: 8 bytes
          WriteLongLat(waypoint.LongLat, routeWriter);
          // time and tome type: 1 + 2-8 bytes
          WriteTimeTypeAndTime(waypoint.Time, lastTime, routeWriter);
          lastTime = waypoint.Time;
          // heart rate: 1 byte
          if ((((UInt16)attributes) & ((UInt16)WaypointAttribute.HeartRate)) == (UInt16)WaypointAttribute.HeartRate)
          {
            routeWriter.Write((byte)(waypoint.HeartRate.HasValue ? Math.Min(Math.Max(waypoint.HeartRate.Value, byte.MinValue), byte.MaxValue) : byte.MinValue));
          }
          // altitude: 2 bytes
          if ((((UInt16)attributes) & ((UInt16)WaypointAttribute.Altitude)) == (UInt16)WaypointAttribute.Altitude)
          {
            routeWriter.Write((Int16)(waypoint.Altitude.HasValue ? Math.Min(Math.Max(waypoint.Altitude.Value, Int16.MinValue), Int16.MaxValue) : 0));
          }
        }
      }
      sessionWriter.Write((byte)Tags.Route);
      sessionWriter.Write((UInt32)routeStream.Length);
      sessionWriter.Write(routeStream.ToArray());
      routeWriter.Close();
      routeStream.Close();
      routeStream.Dispose();

      // handles
      // TODO: adjust for zoom
      var handleStream = new MemoryStream();
      var handleWriter = new BinaryWriter(handleStream);
      handleWriter.Write((UInt32)session.Handles.Length);
      foreach (var handle in session.Handles)
      {
        // transformation matrix
        var scaleMatrix = new GeneralMatrix(new[] {PercentualSize, 0, 0, 0, PercentualSize, 0, 0, 0, 1}, 3);
        var scaledTM = scaleMatrix*handle.TransformationMatrix; 
        for (var i = 0; i < 3; i++)
        {
          for (var j = 0; j < 3; j++)
          {
            handleWriter.Write(scaledTM.GetElement(i, j));
          }
        }
        // parameterized location
        handleWriter.Write((UInt32)handle.ParameterizedLocation.SegmentIndex);
        handleWriter.Write(handle.ParameterizedLocation.Value);
        // pixel location
        handleWriter.Write(PercentualSize * handle.Location.X);
        handleWriter.Write(PercentualSize * handle.Location.Y);
        // type
        handleWriter.Write((Int16)handle.Type);
      }
      sessionWriter.Write((byte)Tags.Handles);
      sessionWriter.Write((UInt32)handleStream.Length);
      sessionWriter.Write(handleStream.ToArray());
      handleWriter.Close();
      handleStream.Close();
      handleStream.Dispose();

      // projection origin
      sessionWriter.Write((byte)Tags.ProjectionOrigin);
      sessionWriter.Write((UInt32)8);
      WriteLongLat(session.ProjectionOrigin, sessionWriter);

      // laps
      var lapStream = new MemoryStream();
      var lapWriter = new BinaryWriter(lapStream);
      lapWriter.Write((UInt32)session.Laps.Count);
      foreach (var lap in session.Laps)
      {
        // time
        lapWriter.Write(lap.Time.ToUniversalTime().ToBinary());
        // type
        lapWriter.Write((byte)lap.LapType);
      }
      sessionWriter.Write((byte)Tags.Laps);
      sessionWriter.Write((UInt32)lapStream.Length);
      sessionWriter.Write(lapStream.ToArray());
      lapWriter.Close();
      lapStream.Close();
      lapStream.Dispose();

      // session info
      var sessionInfoStream = new MemoryStream();
      var sessionInfoWriter = new BinaryWriter(sessionInfoStream);
      // never change the order or remove field that has already been added!
      WriteString(session.SessionInfo.Person.Name, sessionInfoWriter);
      WriteString(session.SessionInfo.Person.Club, sessionInfoWriter);
      sessionInfoWriter.Write(session.SessionInfo.Person.Id);
      WriteString(session.SessionInfo.Description, sessionInfoWriter);
      
      sessionWriter.Write((byte)Tags.SessionInfo);
      sessionWriter.Write((UInt32)sessionInfoStream.Length);
      sessionWriter.Write(sessionInfoStream.ToArray());
      sessionInfoWriter.Close();
      sessionInfoStream.Close();
      sessionInfoStream.Dispose();

      // map reading info
      var mapReadingsList = session.Route.GetMapReadingsList();
      if(mapReadingsList != null)
      {
        var mapReadingInfoStream = new MemoryStream();
        var mapReadingInfoWriter = new BinaryWriter(mapReadingInfoStream);
        var lastTime = DateTime.MinValue;
        foreach (var mapReading in mapReadingsList)
        {
          WriteTimeTypeAndTime(mapReading, lastTime, mapReadingInfoWriter);
        }
        sessionWriter.Write((byte)Tags.MapReadingInfo);
        sessionWriter.Write((UInt32)mapReadingInfoStream.Length);
        sessionWriter.Write(mapReadingInfoStream.ToArray());
        mapReadingInfoWriter.Close();
        mapReadingInfoStream.Close();
        mapReadingInfoStream.Dispose();
      }

      var data = sessionStream.ToArray();
      sessionWriter.Close();
      sessionStream.Close();
      sessionStream.Dispose();
      return data;
    }

    private static void WriteTimeTypeAndTime(DateTime time, DateTime lastTime, BinaryWriter writer)
    {
      var diff = time.Subtract(lastTime).Ticks / 10000; // milliseconds
      var type = (diff > UInt16.MaxValue ? TimeType.Full : TimeType.Delta);
      writer.Write((byte)type);
      // time: 2-8 bytes
      switch (type)
      {
        case TimeType.Full:
          writer.Write(time.ToUniversalTime().ToBinary());
          break;
        case TimeType.Delta:
          writer.Write((UInt16)diff);
          break;
      }
    }

    private static void WriteString(string s, BinaryWriter writer)
    {
      var bytes = new UTF8Encoding().GetBytes(s);
      var length = Math.Min(bytes.Length, 65535);
      var bytesToWrite = new byte[length];

      Array.Copy(bytes, bytesToWrite, length);
      writer.Write((UInt16)length);
      writer.Write(bytesToWrite);
    }

    private static byte[] GetStringBytes(string s)
    {
      var bytes = new UTF8Encoding().GetBytes(s);
      var length = Math.Min(bytes.Length, 65535);
      var bytesToWrite = new byte[length];

      Array.Copy(bytes, bytesToWrite, length);
      return bytesToWrite;
    }
    
    private static string ReadString(BinaryReader reader)
    {
      var length = reader.ReadUInt16();
      return new UTF8Encoding().GetString(reader.ReadBytes(length));
    }

    private enum Tags : byte
    {
      Version = 1,
      /// <summary>
      /// The longitude/latitude coordinates of the corners of the map image (excluding borders and header)
      /// </summary>
      MapCornerPositions = 2,
      /// <summary>
      /// The longitude/latitude coordinates of the corners of the map image (including borders and header)
      /// </summary>
      ImageCornerPositions = 3,
      /// <summary>
      /// The pixel location and size of the the map image (excluding borders and header) 
      /// </summary>
      MapLocationAndSizeInPixels = 4,
      Sessions = 5,
      Session = 6,
      Route = 7,
      Handles = 8,
      ProjectionOrigin = 9,
      Laps = 10,
      SessionInfo = 11,
      MapReadingInfo = 12
    }

    private enum TimeType : byte
    {
      Full = 0,
      Delta = 1
    }

    [Flags]
    private enum WaypointAttribute
    {
      Position = 1,
      Time = 2,
      HeartRate = 4,
      Altitude = 8
    }
  }
}
