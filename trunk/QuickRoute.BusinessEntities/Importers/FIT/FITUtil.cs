using System;
using System.Collections.Generic;

namespace QuickRoute.BusinessEntities.Importers.FIT
{
  public static class FITUtil
  {
    public static UInt16 ChangeEndianness(UInt16 value, byte architecture)
    {
      if (architecture == 0) return value;
      var input = BitConverter.GetBytes(value);
      var output = new byte[input.Length];
      for (var i = 0; i < input.Length; i++)
      {
        output[input.Length - 1 - i] = input[i];
      }
      return BitConverter.ToUInt16(output, 0);
    }

    public static UInt32 ChangeEndianness(UInt32 value, byte architecture)
    {
      if (architecture == 0) return value;
      var input = BitConverter.GetBytes(value);
      var output = new byte[input.Length];
      for (var i = 0; i < input.Length; i++)
      {
        output[input.Length - 1 - i] = input[i];
      }
      return BitConverter.ToUInt32(output, 0);
    }

    public static Int32 ChangeEndianness(Int32 value, byte architecture)
    {
      if (architecture == 0) return value;
      var input = BitConverter.GetBytes(value);
      var output = new byte[input.Length];
      for (var i = 0; i < input.Length; i++)
      {
        output[input.Length - 1 - i] = input[i];
      }
      return BitConverter.ToInt32(output, 0);
    }

    public static void AddOrReplace<TKey, TValue>(IDictionary<TKey, TValue> source, TKey key, TValue value)
    {
      if(source.ContainsKey(key))
      {
        source[key] = value;
      }
      else
      {
        source.Add(key, value);
      }
    }

    public static DateTime ToDateTime(UInt32 timestamp)
    {
      return new DateTime(1989, 12, 31, 00, 00, 00, DateTimeKind.Utc).AddSeconds(timestamp);
    }

    public static UInt32 AddCompressedTimestamp(UInt32 timestamp, byte offset)
    {
      if(offset > (timestamp & 0x0000001F))
      {
        return (timestamp & 0xFFFFFFE0) + offset;
      }
      return (timestamp & 0xFFFFFFE0) + offset + 0x20;
    }
  }
}