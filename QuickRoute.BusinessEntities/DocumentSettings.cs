using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using QuickRoute.BusinessEntities.GlobalizedProperties;

namespace QuickRoute.BusinessEntities
{
  /// <summary>
  /// Class containing various settings for a QuickRoute document.
  /// </summary>
  [Serializable]
  public class DocumentSettings : GlobalizedObject 
  {
    private SessionSettings defaultSessionSettings = new SessionSettings();
    private Dictionary<WaypointAttribute, ColorRangeIntervalSliderSettings> colorRangeIntervalSliderSettings =
      CreateDefaultColorRangeIntervalSliderSettings();

    private Dictionary<WaypointAttribute, LapHistogramSettings> lapHistogramSettings =
      CreateDefaultLapHistogramSettings();

    public SessionSettings DefaultSessionSettings
    {
      get { return defaultSessionSettings; }
      set { defaultSessionSettings = value; }
    }

    public Dictionary<WaypointAttribute, ColorRangeIntervalSliderSettings> ColorRangeIntervalSliderSettings
    {
      get { return colorRangeIntervalSliderSettings; }
      set { colorRangeIntervalSliderSettings = value; }
    }

    public Dictionary<WaypointAttribute, LapHistogramSettings> LapHistogramSettings
    {
      get { return lapHistogramSettings; }
      set { lapHistogramSettings = value; }
    }

    public static Dictionary<WaypointAttribute, ColorRangeIntervalSliderSettings> CreateDefaultColorRangeIntervalSliderSettings()
    {
      Dictionary<WaypointAttribute, ColorRangeIntervalSliderSettings> defaultColorRangeIntervalSliderSettings = new Dictionary<WaypointAttribute, ColorRangeIntervalSliderSettings>();
      defaultColorRangeIntervalSliderSettings.Add(
        WaypointAttribute.Pace,
        new ColorRangeIntervalSliderSettings(0.0 * 60, 10.0 * 60)
      );
      defaultColorRangeIntervalSliderSettings.Add(
        WaypointAttribute.Speed,
        new ColorRangeIntervalSliderSettings(0.0, 20.0)
      );
      defaultColorRangeIntervalSliderSettings.Add(
        WaypointAttribute.HeartRate,
        new ColorRangeIntervalSliderSettings(100, 200)
      );
      defaultColorRangeIntervalSliderSettings.Add(
        WaypointAttribute.Altitude,
        new ColorRangeIntervalSliderSettings(0, 500)
      );
      defaultColorRangeIntervalSliderSettings.Add(
        WaypointAttribute.DirectionDeviationToNextLap,
        new ColorRangeIntervalSliderSettings(0, 90)
      );
      defaultColorRangeIntervalSliderSettings.Add(
        WaypointAttribute.MapReadingDuration,
        new ColorRangeIntervalSliderSettings(0, 10)
      );

      return defaultColorRangeIntervalSliderSettings;
    }

    public static Dictionary<WaypointAttribute, LapHistogramSettings> CreateDefaultLapHistogramSettings()
    {
      Dictionary<WaypointAttribute, LapHistogramSettings> defaultLapHistogramSettings = new Dictionary<WaypointAttribute, LapHistogramSettings>();
      defaultLapHistogramSettings.Add(
        WaypointAttribute.Pace,
        new LapHistogramSettings(10)
      );
      defaultLapHistogramSettings.Add(
        WaypointAttribute.Speed,
        new LapHistogramSettings(0.5)
      );
      defaultLapHistogramSettings.Add(
        WaypointAttribute.HeartRate,
        new LapHistogramSettings(2)
      );
      defaultLapHistogramSettings.Add(
        WaypointAttribute.Altitude,
        new LapHistogramSettings(10)
      );
      defaultLapHistogramSettings.Add(
        WaypointAttribute.DirectionDeviationToNextLap,
        new LapHistogramSettings(2)
      );
      defaultLapHistogramSettings.Add(
        WaypointAttribute.MapReadingDuration,
        new LapHistogramSettings(1)
      );
      return defaultLapHistogramSettings;
    }

    public DocumentSettings Copy()
    {
      MemoryStream ms = new MemoryStream();
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(ms, this);
      ms.Flush();
      ms.Seek(0, SeekOrigin.Begin);
      return bf.Deserialize(ms) as DocumentSettings;
    }

  }

  [Serializable]
  public class ColorRangeIntervalSliderSettings : GlobalizedObject 
  {
    private double minValue;
    private double maxValue;

    public ColorRangeIntervalSliderSettings(double minValue, double maxValue)
    {
      this.minValue = minValue;
      this.maxValue = maxValue;
    }

    public double MinValue
    {
      get { return minValue; }
      set { minValue = value; }
    }

    public double MaxValue
    {
      get { return maxValue; }
      set { maxValue = value; }
    }

  }

  [Serializable]
  public class LapHistogramSettings : GlobalizedObject 
  {
    private double binWidth;

    public LapHistogramSettings(double binWidth)
    {
      this.binWidth = binWidth;
    }

    public double BinWidth
    {
      get { return binWidth; }
      set { binWidth = value; }
    }

  }

}
