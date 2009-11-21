using System;

namespace QuickRoute.BusinessEntities.Numeric
{
  [Serializable]
  public abstract class ScaleCreatorBase
  {
    protected double scaleStartValue = 0.0;
    protected double scaleEndValue = 1.0;
    protected double markerInterval = 0.1;
    protected int noOfMarkers = 11;
    protected double firstMarkerValue = 0.0;
    protected double lastMarkerValue = 1.0;

    public double ScaleStartValue
    {
      get { return scaleStartValue; }
    }

    public double ScaleEndValue
    {
      get { return scaleEndValue; }
    }

    public double MarkerInterval
    {
      get { return markerInterval; }
    }

    public int NoOfMarkers
    {
      get { return noOfMarkers; }
    }

    public double FirstMarkerValue
    {
      get { return firstMarkerValue; }
    }

    public double LastMarkerValue
    {
      get { return lastMarkerValue; }
    }

    public double MarkerValue(int index)
    {
      return firstMarkerValue + index * markerInterval;
    }
  }

  [Serializable]
  public class IntScaleCreator : ScaleCreatorBase 
  {
    // use 10^n * 1, 2, or 5 as marker intervals, n>=0
    public IntScaleCreator(int minValue, int maxValue, int maxNoOfMarkers, bool adjustStartAndEndValues)
    {
      if (maxNoOfMarkers < 2) maxNoOfMarkers = 2;
      int intervalLength = Math.Abs(maxValue - minValue);
      if(intervalLength == 0) intervalLength = 1;

      double minMarkerInterval = intervalLength / (maxNoOfMarkers - 1);

      double log10 = Math.Log10(minMarkerInterval);
      int n = (int)Math.Floor(log10);
     
      double scaledMinMarkerInterval = minMarkerInterval / Math.Pow(10,n); // 1 <= scaledMinMarkerInterval < 10 
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
        scaleStartValue = Math.Floor(minValue / markerInterval) * markerInterval;
        scaleEndValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        firstMarkerValue = scaleStartValue;
        lastMarkerValue = scaleEndValue;
        noOfMarkers = (int)((lastMarkerValue - firstMarkerValue) / markerInterval) + 1;
      }
      else
      {
        scaleStartValue = minValue;
        scaleEndValue = maxValue;
        firstMarkerValue = Math.Floor(minValue / markerInterval) * markerInterval;
        if (firstMarkerValue < minValue) firstMarkerValue += markerInterval;
        lastMarkerValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        if (lastMarkerValue > maxValue) lastMarkerValue -= markerInterval;
        noOfMarkers = (int)((lastMarkerValue - firstMarkerValue) / markerInterval) + 1;
      }

    }
  }

  /// <summary>
  /// Times are represented by seconds
  /// </summary>
  [Serializable]
  public class TimeScaleCreator : ScaleCreatorBase
  {
    // marker intervals
    static readonly double[] VALID_MARKER_INTERVALS = new double[] { 0.01, 0.02, 0.05, 0.1, 0.2, 0.5, 1, 2 , 5 , 15 , 30 , 1 * 60, 2 * 60, 5 * 60, 15 * 60, 30 * 60, 1 * 3600, 2 * 3600, 6 * 3600, 12 * 3600, 24 * 3600 };

    public TimeScaleCreator(double minValue, double maxValue, int maxNoOfMarkers, bool adjustStartAndEndValues)
    {
      if (maxNoOfMarkers < 2) maxNoOfMarkers = 2;
      double intervalLength = Math.Abs(maxValue - minValue);
      if (intervalLength == 0) intervalLength = double.Epsilon;

      double minMarkerInterval = intervalLength / (maxNoOfMarkers - 1);

      markerInterval = VALID_MARKER_INTERVALS[VALID_MARKER_INTERVALS.Length - 1];
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
        scaleStartValue = Math.Floor(minValue / markerInterval) * markerInterval;
        scaleEndValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        firstMarkerValue = scaleStartValue;
        lastMarkerValue = scaleEndValue;
        noOfMarkers = (int)((lastMarkerValue - firstMarkerValue) / markerInterval) + 1;
      }
      else
      {
        scaleStartValue = minValue;
        scaleEndValue = maxValue;
        firstMarkerValue = Math.Floor(minValue / markerInterval) * markerInterval;
        if (firstMarkerValue < minValue) firstMarkerValue += markerInterval;
        lastMarkerValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        if (lastMarkerValue > maxValue) lastMarkerValue -= markerInterval;
        noOfMarkers = (int)((lastMarkerValue - firstMarkerValue) / markerInterval) + 1;
      }

    }
  }

  [Serializable]
  public class DoubleScaleCreator : ScaleCreatorBase
  {
    // use 10^n * 1, 2, or 5 as marker intervals
    public DoubleScaleCreator(double minValue, double maxValue, int maxNoOfMarkers, bool adjustStartAndEndValues)
    {
      if (maxNoOfMarkers < 2) maxNoOfMarkers = 2;
      double intervalLength = Math.Abs(maxValue - minValue);
      if (intervalLength == 0) intervalLength = double.Epsilon;

      double minMarkerInterval = intervalLength / (maxNoOfMarkers - 1);

      double log10 = Math.Log10(minMarkerInterval);
      int n = (int)Math.Floor(log10);

      double scaledMinMarkerInterval = minMarkerInterval / Math.Pow(10, n); // 1 <= scaledMinMarkerInterval < 10 
      if (scaledMinMarkerInterval <= 2){
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
        scaleStartValue = Math.Floor(minValue / markerInterval) * markerInterval;
        scaleEndValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        firstMarkerValue = scaleStartValue;
        lastMarkerValue = scaleEndValue;
        noOfMarkers = (int)((lastMarkerValue - firstMarkerValue) / markerInterval) + 1;
      }
      else
      {
        scaleStartValue = minValue;
        scaleEndValue = maxValue;
        firstMarkerValue = Math.Floor(minValue / markerInterval) * markerInterval;
        if (firstMarkerValue < minValue) firstMarkerValue += markerInterval;
        lastMarkerValue = Math.Ceiling(maxValue / markerInterval) * markerInterval;
        if (lastMarkerValue > maxValue) lastMarkerValue -= markerInterval;
        noOfMarkers = (int)((lastMarkerValue - firstMarkerValue) / markerInterval) + 1;
      }

    }
  }


}
