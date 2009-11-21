using System;
using System.Runtime.Serialization;
using System.Drawing;
using QuickRoute.BusinessEntities.GlobalizedProperties;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class ColorRange : GlobalizedObject, ISerializable 
  {
    private Gradient gradient;
    private double startValue;
    private double endValue;

    public event EventHandler StartValueChanged;
    public event EventHandler EndValueChanged;

    public ColorRange(Gradient gradient, double startValue, double endValue)
    {
      this.gradient = gradient;
      this.startValue = startValue;
      this.endValue = endValue;
    }

    protected ColorRange(SerializationInfo info, StreamingContext context)
    {
      gradient = (Gradient)info.GetValue("gradient", typeof(Gradient));
      startValue = info.GetDouble("startValue");
      endValue = info.GetDouble("endValue");
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("gradient", gradient, typeof(Gradient));
      info.AddValue("startValue", startValue);
      info.AddValue("endValue", endValue);
    }

    public Gradient Gradient
    {
      get { return gradient; }
      set { gradient = value; }
    }

    public double StartValue
    {
      get { return startValue; }
      set 
      { 
        startValue = value;
        if (StartValueChanged != null) StartValueChanged(this, new EventArgs());
      }
    }

    public double EndValue
    {
      get { return endValue; }
      set
      {
        endValue = value;
        if (EndValueChanged != null) EndValueChanged(this, new EventArgs());
      }
    }

    public Color GetColor(double value)
    {
      return gradient.GetColor(GetLocationFromValue(value));
    }

    public Color GetColor(double value, int noOfDiscreteSteps)
    {
      // get step
      var stepLength = (endValue - startValue)/noOfDiscreteSteps;
      var stepIndex = (int)Math.Floor((value - startValue)/stepLength);
      stepIndex = Math.Min(noOfDiscreteSteps - 1, stepIndex);
      stepIndex = Math.Max(0, stepIndex);
      var steppedValue = startValue + (stepIndex + 0.5)*stepLength;
      return gradient.GetColor(GetLocationFromValue(steppedValue));
    }

    private double GetLocationFromValue(double value)
    {
      if (startValue == endValue) return 0.0;
      return (value - startValue) / (endValue - startValue);
    }

  }

}
