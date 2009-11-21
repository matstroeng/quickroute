using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;


namespace QuickRoute.Common
{
  public static class LogUtil
  {
    private static bool configured;
    private static decimal lastTime = -1;
    private static readonly Dictionary<object, HighPerformanceTimer> timers = new Dictionary<object, HighPerformanceTimer>();
    private static readonly HighPerformanceTimer standardTimer = new HighPerformanceTimer();

    public static void LogDebug(string message)
    {
      WriteToLog(message, LogLevel.Debug);
    }

    public static void LogInfo(string message)
    {
      WriteToLog(message, LogLevel.Info);
    }

    public static void LogWarn(string message)
    {
      WriteToLog(message, LogLevel.Warn);
    }

    public static void LogError(string message)
    {
      WriteToLog(message, LogLevel.Error);
    }

    public static void LogFatal(string message)
    {
      WriteToLog(message, LogLevel.Fatal);
    }

    private static void WriteToLog(string message, LogLevel level)
    {
      if (!configured) throw new Exception("The LogUtil is not configured.");

      var thisTime = HighPerformanceTimer.GetCurrentTime();
      var duration = (lastTime == -1 ? 0 : thisTime - lastTime);
      lastTime = thisTime;
      var caller = GetCaller();
      var log = log4net.LogManager.GetLogger(caller.DeclaringType);

      var m = String.Format("{0:0.000}", thisTime) + " " +
              String.Format("{0:0.000}", duration) + " " +
              caller.Name + ": " +
              message;
      switch (level)
      {
        case LogLevel.Debug:
          log.Debug(m);
          break;
        case LogLevel.Info:
          log.Info(m);
          break;
        case LogLevel.Warn:
          log.Warn(m);
          break;
        case LogLevel.Error:
          log.Error(m);
          break;
        case LogLevel.Fatal:
          log.Fatal(m);
          break;
      }
    }

    public static void Configure()
    {
      Configure(null);
    }

    public static void Configure(string logFileName)
    {
      XmlConfigurator.Configure();
      if (logFileName != null)
      {
        // log to the specified log file name
        var hierarchy = (log4net.Repository.Hierarchy.Hierarchy) LogManager.GetRepository();
        foreach (var a in hierarchy.Root.Appenders)
        {
          if (a is FileAppender)
          {
            var fa = (FileAppender) a;
            fa.File = logFileName;
            fa.ActivateOptions();
          }
        }
      }

      configured = true;
    }

    private static MethodBase GetCaller()
    {
      var stackFrame = new StackTrace().GetFrame(3);
      return stackFrame.GetMethod();
    }

    private static HighPerformanceTimer GetTimer()
    {
      return standardTimer;
    }

    private static HighPerformanceTimer GetTimer(object key)
    {
      if (!timers.ContainsKey(key)) timers.Add(key, new HighPerformanceTimer());
      return timers[key];
    }

    private static void DisposeTimer(object key)
    {
      if (timers.ContainsKey(key)) timers.Remove(key);
    }

  }

  public class HighPerformanceTimer
  {
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long lpFrequency);

    private long startTime, stopTime;
    private static readonly long freq;

    private bool isStarted;
    private decimal duration;

    static HighPerformanceTimer()
    {
      if (QueryPerformanceFrequency(out freq) == false)
      {
        // high-performance counter not supported
        throw new Win32Exception();
      }
    }

    // Constructor
    public HighPerformanceTimer()
      : this(false)
    {
    }

    public HighPerformanceTimer(bool startImmediately)
    {
      if (startImmediately) Start();
    }

    // Start the timer
    public decimal Start()
    {
      // lets do the waiting threads there work
      Thread.Sleep(0);

      QueryPerformanceCounter(out startTime);
      isStarted = true;
      return duration;
    }

    // Stop the timer
    public decimal Stop()
    {
      if (!isStarted) return duration;
      QueryPerformanceCounter(out stopTime);
      duration += (decimal)(stopTime - startTime) / freq;
      return duration;
    }

    // Returns the duration of the timer (in seconds)
    public decimal Duration
    {
      get
      {
        if (isStarted)
        {
          long currentTime;
          QueryPerformanceCounter(out currentTime);
          return duration + (decimal)(currentTime - startTime) / freq;
        }
        return duration;
      }
    }

    public void Reset()
    {
      isStarted = false;
      duration = 0;
    }

    public void ResetAndStart()
    {
      Reset();
      Start();
    }

    public static decimal GetCurrentTime()
    {
      long currentTime;
      QueryPerformanceCounter(out currentTime);
      return (decimal)currentTime / freq;
    }
  }

  public enum LogLevel
  {
    Debug,
    Info,
    Warn,
    Error,
    Fatal
  }

}
