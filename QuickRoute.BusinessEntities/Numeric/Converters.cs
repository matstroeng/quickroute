using System;

namespace QuickRoute.BusinessEntities.Numeric
{
  [Serializable]
  public class NumericConverter
  {
    private int noOfDecimals = 1;
    private bool forceSign;

    public int NoOfDecimals
    {
      get { return noOfDecimals; }
      set { noOfDecimals = Math.Max(0, value); }
    }

    public bool ForceSign
    {
      get { return forceSign; }
      set { forceSign = value; }
    }

    public virtual double? ToNumeric(string s)
    {
      double result;
      bool success = double.TryParse(s.Replace(" ", ""), out result);
      if (success)
      {
        return result;
      }
      return null;
    }

    public virtual string ToString(double? d)
    {
      if (d == null)
      {
        return null;
      }
      return (forceSign && d.Value > 0 ? "+" : "") + d.Value.ToString("n" + noOfDecimals);
    }

    public virtual string ToString(double d)
    {
      return ToString((double?)d);
    }
  }

  [Serializable]
  public class TimeConverter : NumericConverter  
  {
    private TimeConverterType type;

    public TimeConverter(TimeConverterType type)
    {
      this.type = type;
      NoOfDecimals = 0;
    }

    public TimeConverterType Type
    {
      get { return type; }
    }

    private readonly string[] separators = new string[] { ".", ":", "," };

    public override double? ToNumeric(string s)
    {
      string[] atoms = s.Split(separators, StringSplitOptions.None);
      try
      {
        int factor = 0;
        switch (type)
        {
          case TimeConverterType.ElapsedTime:
            factor = 1;
            break;

          case TimeConverterType.TimeOfDay:
            factor = 60;
            break;
        }

        switch (atoms.Length)
        {
          case 1:
            return factor * 60 * Convert.ToInt32(atoms[0]);
          case 2:
            return factor * 60 * Convert.ToInt32(atoms[0]) +
                   factor * Convert.ToInt32(atoms[1]);
          case 3:
            return 3600 * Convert.ToInt32(atoms[0]) +
                   60 * Convert.ToInt32(atoms[1]) +
                   Convert.ToInt32(atoms[2]);
          default:
            return null;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public override string ToString(double? d)
    {
      if (d == null) return null;
      long intValue = (long)d;
      var secondsFormat = "00" + (NoOfDecimals == 0 ? "" : "." + new string('0', NoOfDecimals));
      var resolution = Math.Pow(10, -NoOfDecimals);
      var rounded = Math.Floor(d.Value/resolution) * resolution;

      switch (type)
      {
        case TimeConverterType.ElapsedTime:
          if (intValue < 3600)
          {
            return string.Format("{0:d1}:{1:" + secondsFormat + "}",
              intValue / 60, rounded % 60);
          }
          else
          {
            return string.Format("{0:d1}:{1:d2}:{2:" + secondsFormat + "}",
              intValue / 3600, (intValue / 60) % 60, rounded % 60);
          }

        case TimeConverterType.TimeOfDay:
          return string.Format("{0:d1}:{1:d2}:{2:" + secondsFormat + "}",
            (intValue / 3600) % 24, (intValue / 60) % 60, rounded % 60);

        default:
          return null;
      }
    }

    public override string ToString(double d)
    {
      return ToString((double?)d);
    }

    public string ToString(DateTime dt)
    {
      var localTime = dt.ToLocalTime();
      return ToString((double?)localTime.Subtract(localTime.Date).TotalSeconds);
    }

    public string ToString(TimeSpan ts)
    {
      return ToString((double?)ts.TotalSeconds);
    }

    public enum TimeConverterType
    {
      ElapsedTime,
      TimeOfDay
    }
  }

  [Serializable]
  public class IntConverter : NumericConverter  
  {
    public override double? ToNumeric(string s)
    {
      int result;
      bool success = int.TryParse(s.Replace(" ", ""), out result);
      if (success)
      {
        return (double)result;
      }
      else
      {
        return null;
      }
    }

    public override string ToString(double? d)
    {
      if (d == null)
      {
        return null;
      }
      else
      {
        return ((int)d).ToString("n0");
      }
    }

    public override string ToString(double d)
    {
      return ToString((double?)d);
    }
 
  }



}
