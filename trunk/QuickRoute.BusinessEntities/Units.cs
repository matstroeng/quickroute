using System;
using System.Collections.Generic;

namespace QuickRoute.BusinessEntities
{
  public abstract class IUnit : IComparable<IUnit> 
  {
    public abstract new string ToString();
    public abstract void FromString(string s);
    public abstract double ToDouble(IUnit u0, double d0, IUnit u1, double d1);
    public abstract object ValueAsObject { get; set; }
    public abstract int CompareTo(IUnit other);
  }

  public class TimeOfDay : IUnit
  {
    private readonly string[] separators = new[] { ".", ":", "," };

    public TimeOfDay()
    {
    }

    public TimeOfDay(DateTime value)
    {
      Value = value;
    }

    public DateTime Value { get; set; }

    public override string ToString()
    {
      int seconds = (int)(Value.Subtract(Value.Date).TotalSeconds);
      return string.Format("{0:d1}", seconds / 3600) +
             ":" +
             string.Format("{0:d2}", (seconds / 60) % 60) +
             ":" +
             string.Format("{0:d2}", seconds % 60);
    }

    public override void FromString(string s)
    {
      string[] atoms = s.Split(separators, StringSplitOptions.None);
      try
      {
        int hours = 0;
        int minutes = 0;
        int seconds = 0;
        switch (atoms.Length)
        {
          case 1:
            seconds = Convert.ToInt32(atoms[0]);
            break;
          case 2:
            minutes = Convert.ToInt32(atoms[0]);
            seconds = Convert.ToInt32(atoms[1]);
            break;
          case 3:
            hours = Convert.ToInt32(atoms[0]);
            minutes = Convert.ToInt32(atoms[1]);
            seconds = Convert.ToInt32(atoms[2]);
            break;
          default:
            break;
        }
        Value = new DateTime(0, 0, 0, hours, minutes, seconds);
      }
      catch (Exception)
      {
        Value = new DateTime();
      }      
    }

    public override double ToDouble(IUnit u0, double d0, IUnit u1, double d1)
    {
      TimeOfDay v0 = u0 as TimeOfDay;
      TimeOfDay v1 = u1 as TimeOfDay;
      if (v0 == null || v1 == null) throw new ArgumentException();
      if (v0.Value.Ticks == v1.Value.Ticks) return d0;
      return d0 + (double)(Value.Ticks - v0.Value.Ticks) / (v1.Value.Ticks - v0.Value.Ticks) * (d1 - d0);   
    }

    public override object ValueAsObject
    {
      get { return Value; }
      set { Value = Convert.ToDateTime(value); }
    }

    public override int CompareTo(IUnit other)
    {
      if (other is TimeOfDay)
      {
        return Value.CompareTo((other as TimeOfDay).Value);
      }
      throw new ArgumentException();
    }

    public static Scale CreateScale(DateTime minValue, DateTime maxValue, int maxNoOfMarkers, bool adjustStartAndEndValues)
    {
      Scale s = ElapsedTime.CreateScale(
        DateTimeToDouble(minValue),
        DateTimeToDouble(maxValue),
        maxNoOfMarkers,
        adjustStartAndEndValues);
      s.Start = new TimeOfDay(DoubleToDateTime(((ElapsedTime)s.Start).Value));
      s.End = new TimeOfDay(DoubleToDateTime(((ElapsedTime)s.End).Value));
      List<IUnit> newMarkers = new List<IUnit>();
      foreach(IUnit u in s.Markers)
      {
        newMarkers.Add(new TimeOfDay(DoubleToDateTime(((ElapsedTime)u).Value)));
      }
      s.Markers = newMarkers;
      return s;
    }

    private static double DateTimeToDouble(DateTime dt)
    {
      return (double)dt.Ticks / TimeSpan.TicksPerSecond;
    }

    private static DateTime DoubleToDateTime(double d)
    {
      return new DateTime((long)(d*TimeSpan.TicksPerSecond));
    }

  }

  /// <summary>
  /// Elapsed time expressed in seconds
  /// </summary>
  public class ElapsedTime : IUnit
  {
    private readonly string[] separators = new[] { ".", ":", "," };

    public ElapsedTime()
    {
    }

    public ElapsedTime(double value)
    {
      Value = value;
    }

    public double Value { get; set; }

    public TimeSpan ToTimeSpan()
    {
      return new TimeSpan(Convert.ToInt64(TimeSpan.TicksPerSecond*Value));
    }

    #region IUnit Members

    public override string ToString()
    {
      int seconds = (int)Value;
      if(seconds < 3600)
      {
        return string.Format("{0:d1}", (seconds / 60) % 60) +
               ":" +
               string.Format("{0:d2}", seconds % 60);
      }
      else
      {
        return string.Format("{0:d1}", seconds / 3600) +
               ":" +
               string.Format("{0:d2}", (seconds / 60) % 60) +
               ":" +
               string.Format("{0:d2}", seconds % 60);
      }
    }

    public override void FromString(string s)
    {
      string[] atoms = s.Split(separators, StringSplitOptions.None);
      try
      {
        int hours = 0;
        int minutes = 0;
        int seconds = 0;
        switch (atoms.Length)
        {
          case 1:
            seconds = Convert.ToInt32(atoms[0]);
            break;
          case 2:
            minutes = Convert.ToInt32(atoms[0]);
            seconds = Convert.ToInt32(atoms[1]);
            break;
          case 3:
            hours = Convert.ToInt32(atoms[0]);
            minutes = Convert.ToInt32(atoms[1]);
            seconds = Convert.ToInt32(atoms[2]);
            break;
          default:
            break;
        }
        Value = new TimeSpan(hours, minutes, seconds).TotalSeconds;
      }
      catch (Exception)
      {
        Value = 0;
      }
    }

    public override double ToDouble(IUnit u0, double d0, IUnit u1, double d1)
    {
      ElapsedTime v0 = u0 as ElapsedTime;
      ElapsedTime v1 = u1 as ElapsedTime;
      if (v0 == null || v1 == null) throw new ArgumentException(); 
      if (v0.Value == v1.Value) return d0;
      return d0 + (Value - v0.Value) / (v1.Value - v0.Value) * (d1 - d0);
    }

    public override object ValueAsObject
    {
      get { return Value; }
      set { Value = Convert.ToDouble(value); }
    }

    public override int CompareTo(IUnit other)
    {
      if (other is TimeOfDay)
      {
        return Value.CompareTo((other as TimeOfDay).Value); 
      }
      throw new ArgumentException();
    }

    #endregion

    public static Scale CreateScale(double minValue, double maxValue, int maxNoOfMarkers, bool adjustStartAndEndValues)
    {
      double[] VALID_MARKER_INTERVALS = new double[] { 0.01, 0.02, 0.05, 0.1, 0.2, 0.5, 1, 2 , 5 , 15 , 30 , 1 * 60, 2 * 60, 5 * 60, 15 * 60, 30 * 60, 1 * 3600, 2 * 3600, 6 * 3600, 12 * 3600, 24 * 3600 };

      Scale scale = new Scale();
      if (maxNoOfMarkers < 2) maxNoOfMarkers = 2;
      double intervalLength = Math.Abs(maxValue - minValue);
      if (intervalLength == 0) intervalLength = double.Epsilon;

      double minMarkerInterval = intervalLength / (maxNoOfMarkers - 1);
      double firstMarkerValue;
      double lastMarkerValue;

      double markerInterval = VALID_MARKER_INTERVALS[VALID_MARKER_INTERVALS.Length - 1];
      foreach (double mi in VALID_MARKER_INTERVALS)
      {
        if (minMarkerInterval < mi)
        {
          markerInterval = mi;
          break;
        }
      }

      if (adjustStartAndEndValues)
      {
        firstMarkerValue = Math.Floor(minValue / markerInterval) * markerInterval;
        lastMarkerValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        scale.Start = new ElapsedTime(firstMarkerValue);
        scale.End = new ElapsedTime(lastMarkerValue);
      }
      else
      {
        scale.Start = new ElapsedTime(minValue);
        scale.End = new ElapsedTime(maxValue);
        firstMarkerValue = Math.Floor(minValue / markerInterval) * markerInterval;
        if (firstMarkerValue < minValue) firstMarkerValue += markerInterval;
        lastMarkerValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        if (lastMarkerValue > maxValue) lastMarkerValue -= markerInterval;
      }

      scale.Markers = new List<IUnit>();
      for (double value = firstMarkerValue; value <= lastMarkerValue; value += markerInterval)
      {
        scale.Markers.Add(new ElapsedTime(value));
      }

      return scale;
    }
  }

  public class DoubleUnitBase : IUnit
  {
    public DoubleUnitBase()
    {
    }

    public DoubleUnitBase(double value)
    {
      Value = value;
    }

    public double Value { get; set; }

    #region IUnit Members

    public override string ToString()
    {
      return Value.ToString();
    }

    public override void FromString(string s)
    {
      double result;
      if (double.TryParse(s, out result))
      {
        Value = result;
      }
    }

    public override double ToDouble(IUnit u0, double d0, IUnit u1, double d1)
    {
      Distance v0 = u0 as Distance;
      Distance v1 = u1 as Distance;
      if (v0 == null || v1 == null) throw new ArgumentException();
      if (v0.Value == v1.Value) return d0;
      return d0 + (Value - v0.Value) / (v1.Value - v0.Value) * (d1 - d0);
    }

    public override object ValueAsObject
    {
      get { return Value; }
      set { Value = Convert.ToDouble(value); }
    }

    public override int CompareTo(IUnit other)
    {
      if (other is Distance)
      {
        return Value.CompareTo((other as Distance).Value);
      }
      throw new ArgumentException();
    }

    #endregion

    // use 10^n * 1, 2, or 5 as marker intervals
    public static Scale CreateScale(double minValue, double maxValue, int maxNoOfMarkers, bool adjustStartAndEndValues)
    {
      Scale scale = new Scale();
      if (maxNoOfMarkers < 2) maxNoOfMarkers = 2;
      double intervalLength = Math.Abs(maxValue - minValue);
      if (intervalLength == 0) intervalLength = double.Epsilon;

      double minMarkerInterval = intervalLength / (maxNoOfMarkers - 1);
      double firstMarkerValue;
      double lastMarkerValue;

      double log10 = Math.Log10(minMarkerInterval);
      int n = (int)Math.Floor(log10);

      double scaledMinMarkerInterval = minMarkerInterval / Math.Pow(10, n); // 1 <= scaledMinMarkerInterval < 10 
      double markerInterval;
      if (scaledMinMarkerInterval <= 2)
      {
        markerInterval = 2 * Math.Pow(10, n);
      }
      else if (scaledMinMarkerInterval <= 5)
      {
        markerInterval = 5 * Math.Pow(10, n);
      }
      else
      {
        markerInterval = 10 * Math.Pow(10, n);
      }

      if (adjustStartAndEndValues)
      {
        firstMarkerValue = Math.Floor(minValue / markerInterval) * markerInterval;
        lastMarkerValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        scale.Start = new Distance(firstMarkerValue);
        scale.End = new Distance(lastMarkerValue);
      }
      else
      {
        scale.Start = new Distance(minValue);
        scale.End = new Distance(maxValue);
        firstMarkerValue = Math.Floor(minValue / markerInterval) * markerInterval;
        if (firstMarkerValue < minValue) firstMarkerValue += markerInterval;
        lastMarkerValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        if (lastMarkerValue > maxValue) lastMarkerValue -= markerInterval;
      }

      scale.Markers = new List<IUnit>();
      for (double value = firstMarkerValue; value <= lastMarkerValue; value += markerInterval)
      {
        scale.Markers.Add(new Distance(value));
      }

      return scale;
    }
  }

  public class Distance : DoubleUnitBase
  {
    public Distance()
    {
    }

    public Distance(double value)
      : base(value)
    {
    }
  }

  public class HeartRate : DoubleUnitBase
  {
    public HeartRate()
    {
    }

    public HeartRate(double value)
      : base(value)
    {
    }

  }

  public class Altitude : DoubleUnitBase
  {
    public Altitude()
    {
    }

    public Altitude(double value)
      : base(value)
    {
    }
  }

  public class Scale
  {
    public IUnit Start { get; set; }
    public IUnit End { get; set; }
    public List<IUnit> Markers { get; set; }
  }
}
