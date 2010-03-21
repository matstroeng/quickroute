using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace QuickRoute.GPSDeviceReaders.SerialPortDeviceReader
{
    public static class SerialPortUtil
    {
        public static String[] GetLatestPortsList()
        {
            var portNames = SerialPort.GetPortNames();
            Array.Sort(portNames, new AlphanumComparator());
            return portNames;
        }
    }

    public class SerialPortSession : IDisposable
    {
        public SerialPort Port { get; private set; }
        public SerialPortSession(SerialPort serialPort)
        {
            Port = serialPort;
            if (!Port.IsOpen)
            {
                Port.Open();
            }
        }

        public void Dispose()
        {
            Port.Close();
        }
    }

    public class AlphanumComparator : IComparer<String>
    {
        public int Compare(String s1, String s2)
        {
            if (s1 == null)
            {
                return 0;
            }
            if (s2 == null)
            {
                return 0;
            }

            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                char ch1 = s1[marker1];
                char ch2 = s2[marker2];

                // Some buffers we can build up characters in for each chunk.
                var space1 = new char[len1];
                int loc1 = 0;
                var space2 = new char[len2];
                int loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                    {
                        ch1 = s1[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                    {
                        ch2 = s2[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                var str1 = new string(space1);
                var str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    int thisNumericChunk = int.Parse(str1);
                    int thatNumericChunk = int.Parse(str2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = str1.CompareTo(str2);
                }

                if (result != 0)
                {
                    return result;
                }
            }
            return len1 - len2;
        }
    }

    public static class HexUtils
    {
        public static byte[] GetBytes(Int16 value)
        {
            var valueBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes);
            }
            return valueBytes;
        }

        public static byte[] GetBytes(Int32 value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes);
            }
            return valueBytes;
        }

        public static Int16 ToInt16(byte[] buffer, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                var copy = new byte[2];
                Array.Copy(buffer, offset, copy, 0, 2);
                Array.Reverse(copy);
                return BitConverter.ToInt16(copy, 0);
            }
            return BitConverter.ToInt16(buffer, offset);
        }

        public static Int32 ToInt32(byte[] buffer, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                var copy = new byte[4];
                Array.Copy(buffer, offset, copy, 0, 4);
                Array.Reverse(copy);
                return BitConverter.ToInt32(copy, 0);
            }
            return BitConverter.ToInt32(buffer, offset);
        }

        public static Int64 ToInt64(byte[] buffer, int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                var copy = new byte[8];
                Array.Copy(buffer, offset, copy, 0, 8);
                Array.Reverse(copy);
                return BitConverter.ToInt64(copy, 0);
            }
            return BitConverter.ToInt64(buffer, offset);
        }

        public static byte CheckSum(byte[] bytes, int offset, int length)
        {
            byte checkSum = 0;
            for (var i = offset; i < offset + length; i++)
            {
                checkSum ^= bytes[i];
            }
            return checkSum;
        }

        public static byte[] GetBytesLE(Int16 value)
        {
            var valueBytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes);
            }
            return valueBytes;
        }

        public static byte[] GetBytesLE(Int32 value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes);
            }
            return valueBytes;
        }

        public static Int16 ToInt16LE(byte[] buffer, int offset)
        {
            if (!BitConverter.IsLittleEndian)
            {
                var copy = new byte[2];
                Array.Copy(buffer, offset, copy, 0, 2);
                Array.Reverse(copy);
                return BitConverter.ToInt16(copy, 0);
            }
            return BitConverter.ToInt16(buffer, offset);
        }

        public static Int32 ToInt32LE(byte[] buffer, int offset)
        {
            if (!BitConverter.IsLittleEndian)
            {
                var copy = new byte[4];
                Array.Copy(buffer, offset, copy, 0, 4);
                Array.Reverse(copy);
                return BitConverter.ToInt32(copy, 0);
            }
            return BitConverter.ToInt32(buffer, offset);
        }

        public static Int64 ToInt64LE(byte[] buffer, int offset)
        {
            if (!BitConverter.IsLittleEndian)
            {
                var copy = new byte[8];
                Array.Copy(buffer, offset, copy, 0, 8);
                Array.Reverse(copy);
                return BitConverter.ToInt64(copy, 0);
            }
            return BitConverter.ToInt64(buffer, offset);
        }
    }
}
