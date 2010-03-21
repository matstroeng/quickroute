using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace QuickRoute.GPSDeviceReaders.SerialPortDeviceReader
{

    class SerialPortCommandError : Exception
    {
        public SerialPortCommandError() { }
        public SerialPortCommandError(string message) : base(message) { }
        public SerialPortCommandError(string message, Exception e) : base(message, e) { }
    }

    abstract public class SerialPortCommand
    {
        protected static readonly ASCIIEncoding ASCIIEnc = new ASCIIEncoding();

        public static int DefaultMaxBufferSize = 1024 * 1000;
        public static int DefaultSleepTime = 300;

        private readonly List<byte> _buffer = new List<byte>();
        private readonly int _maxBufferSize = DefaultMaxBufferSize;
        private readonly int _sleepTime = DefaultSleepTime;

        protected SerialPortCommand()
            : this(DefaultSleepTime, DefaultMaxBufferSize)
        {
        }

        protected SerialPortCommand(int sleepTime)
            : this(sleepTime, DefaultMaxBufferSize)
        {
        }

        protected SerialPortCommand(int sleepTime, int maxBufferSize)
        {
            _sleepTime = Math.Abs(sleepTime);
            _maxBufferSize = maxBufferSize;
        }

        public void Execute(SerialPort commport)
        {
            if (!commport.IsOpen)
            {
                throw new SerialPortCommandError("Port not opened!");
            }
            Write(commport);
            Thread.Sleep(_sleepTime);
            Read(commport);
            ProcessBuffer(_buffer.ToArray());
            _buffer.Clear();
        }

        private void Write(SerialPort commport)
        {
            byte[] commandBuffer = GetCommandBuffer();
            commport.DiscardInBuffer();
            commport.DiscardOutBuffer();
            commport.Write(commandBuffer, 0, commandBuffer.Length);
        }

        abstract protected byte[] GetCommandBuffer();

        private void Read(SerialPort commport)
        {
            while (commport.BytesToRead > 0 && _buffer.Count <= _maxBufferSize) {
                _buffer.Add((byte)commport.ReadByte());
            }
        }
    
        virtual protected void ProcessBuffer(byte[] buffer)
        {
            // Look in the byte array for useful information
        }

        public static bool CompareFirstN(byte[] buf1, byte[] buf2, int n)
        {
            int len = Math.Abs(n);
            if (buf1 == null || buf2 == null || buf1.Length < len || buf2.Length < len)
            {
                return false;
            }
            for (int i = 0; i < len; i++)
            {
                if (buf1[i] != buf2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CompareLastN(byte[] buf1, byte[] buf2, int n)
        {
            int len = Math.Abs(n);
            if (buf1 == null || buf2 == null || buf1.Length < len || buf2.Length < len)
            {
                return false;
            }
            for (int i = 0; i < len; i++)
            {
                if (buf1[buf1.Length - 1 - i] != buf2[buf2.Length - 1 - i])
                {
                    return false;
                }
            }
            return true;
        }
    }

}
