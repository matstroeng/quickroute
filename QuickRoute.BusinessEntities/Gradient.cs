using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using QuickRoute.BusinessEntities.GlobalizedProperties;

namespace QuickRoute.BusinessEntities
{
  /// <summary>
  /// Class representing a gradient, i e a list of colors and their locations.
  /// </summary>
  [Serializable]
  public class Gradient : GlobalizedObject, ISerializable 
  {
    private string name;
    private string fileName;
    private List<GradientColorEntry> colorEntries = new List<GradientColorEntry>();

    public Gradient() : this(Color.White, Color.Black) { }

    /// <summary>
    /// Creating a new gradient based on the specified color entries.
    /// </summary>
    /// <param name="colorEntryParamArray">First parameter: first color, second parameter: location of first color, third parameter: second color, fourth parameter: location of second color, and so forth. Locations are in the interval (0, 1), inclusive.</param>
    public Gradient(params object[] colorEntryParamArray)
    {
      if (colorEntryParamArray == null)
      {
        throw new Exception("Param array length must be greater than zero.");
      }
      else
      {
        if (colorEntryParamArray.Length % 2 != 0) throw new Exception("Param array length must be an even number.");
        for (int i = 0; i < colorEntryParamArray.Length / 2; i++)
        {
          colorEntries.Add(new GradientColorEntry((Color)colorEntryParamArray[2 * i], (double)colorEntryParamArray[2 * i + 1]));
        }
      }
      CreateSortedColorEntries();
    }

    /// <summary>
    /// Creating a new gradient based on the specified start and end color, and the specified color entries.
    /// </summary>
    /// <param name="startColor"></param>
    /// <param name="endColor"></param>
    /// <param name="colorEntryParamArray">First parameter: first middle color, second parameter: location of first middle color, third parameter: second middle color, fourth parameter: location of second middle color, and so forth. Locations are in the interval (0, 1), inclusive.</param>
    public Gradient(Color startColor, Color endColor, params object[] colorEntryParamArray)
    {
      colorEntries.Add(new GradientColorEntry(startColor, 0));
      colorEntries.Add(new GradientColorEntry(endColor, 1));
      if (colorEntryParamArray != null)
      {
        if (colorEntryParamArray.Length % 2 != 0) throw new Exception("Param array length must be an even number.");
        for (int i = 0; i < colorEntryParamArray.Length / 2; i++)
        {
          colorEntries.Add(new GradientColorEntry((Color)colorEntryParamArray[2 * i], (double)colorEntryParamArray[2 * i + 1]));
        }
      }
      CreateSortedColorEntries();
    }

    protected Gradient(SerializationInfo info, StreamingContext context)
    {
      int[] colors = (int[])(info.GetValue("colors", typeof(int[])));
      double[] locations = (double[])(info.GetValue("locations", typeof(double[])));

      for (int i = 0; i < colors.Length; i++)
      {
        colorEntries.Add(new GradientColorEntry(Color.FromArgb(colors[i]), locations[i]));
      }
      name = info.GetString("name");
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      int[] colors = new int[colorEntries.Count];
      double[] locations = new double[colorEntries.Count];

      for (int i = 0; i < colorEntries.Count; i++)
      {
        colors[i] = colorEntries[i].Color.ToArgb();
        locations[i] = colorEntries[i].Location;
      }

      info.AddValue("colors", colors, typeof(int[]));
      info.AddValue("locations", locations, typeof(double[]));
      info.AddValue("name", name);
    }

    /// <summary>
    /// Inserts a color at the given location.
    /// </summary>
    /// <param name="color">The color to insert.</param>
    /// <param name="location">Location expressed as a value in the interval (0, 1), inclusive.</param>
    public void AddColor(Color color, double location)
    {
      if (location < 0.0 || location > 1.0) throw new Exception("Location out of range, must be between 0 and 1");
      colorEntries.Add(new GradientColorEntry(color, location));
      CreateSortedColorEntries();
    }

    /// <summary>
    /// Draws a gradient on the specified graphics object, in the specified rectangle, using the specified start and end locations (in the (0, 1) interval) and the specified direction.
    /// </summary>
    /// <param name="g">The graphics object to draw on.</param>
    /// <param name="rect">The rectangle object that should be filled.</param>
    /// <param name="startLocation">The start color location.</param>
    /// <param name="endLocation">The end color location.</param>
    /// <param name="direction">The direction of the gradient, horizontal or vertical.</param>
    public void Draw(Graphics g, Rectangle rect, double startLocation, double endLocation, Direction direction)
    {
      Draw(g, rect, startLocation, endLocation, direction, 0);
    }

    /// <summary>
    /// Draws a gradient on the specified graphics object, in the specified rectangle, using the specified start and end locations (in the (0, 1) interval) and the specified direction.
    /// </summary>
    /// <param name="g">The graphics object to draw on.</param>
    /// <param name="rect">The rectangle object that should be filled.</param>
    /// <param name="startLocation">The start color location.</param>
    /// <param name="endLocation">The end color location.</param>
    /// <param name="direction">The direction of the gradient, horizontal or vertical.</param>
    /// <param name="alphaAdjustment">Value that determines the transparency of the gradient. -1 is fully opaque, 0 is normal, 1 is fully transparent.</param>
    public void Draw(Graphics g, Rectangle rect, double startLocation, double endLocation, Direction direction, double alphaAdjustment)
    {
      switch (direction)
      {
        case Direction.Horizontal:
          for (int x = rect.Left; x <= rect.Right; x++)
          {
            double location = startLocation + (x - rect.Left) / (double)(rect.Right - rect.Left) * (endLocation - startLocation);
            Brush b = new SolidBrush(GraphicsUtil.AlphaAdjustColor(GetColor(location), alphaAdjustment));
            g.FillRectangle(b, x, rect.Top, 1, rect.Height);
            b.Dispose();
          }
          break;

        case Direction.Vertical:
          for (int y = rect.Top; y <= rect.Bottom; y++)
          {
            double location = startLocation + (y - rect.Top) / (double)(rect.Bottom - rect.Top) * (endLocation - startLocation);
            Brush b = new SolidBrush(GraphicsUtil.AlphaAdjustColor(GetColor(location), alphaAdjustment));
            g.FillRectangle(b, rect.Left, y, rect.Width, 1);
            b.Dispose();
          }
          break;

        case Direction.ReverseHorizontal:
          for (int x = rect.Left; x <= rect.Right; x++)
          {
            double location = endLocation - (x - rect.Left) / (double)(rect.Right - rect.Left) * (endLocation - startLocation);
            Brush b = new SolidBrush(GraphicsUtil.AlphaAdjustColor(GetColor(location), alphaAdjustment));
            g.FillRectangle(b, x, rect.Top, 1, rect.Height);
            b.Dispose();
          }
          break;

        case Direction.ReverseVertical:
          for (int y = rect.Top; y <= rect.Bottom; y++)
          {
            double location = endLocation - (y - rect.Top) / (double)(rect.Bottom - rect.Top) * (endLocation - startLocation);
            Brush b = new SolidBrush(GraphicsUtil.AlphaAdjustColor(GetColor(location), alphaAdjustment));
            g.FillRectangle(b, rect.Left, y, rect.Width, 1);
            b.Dispose();
          }
          break;
      }
    }

    /// <summary>
    /// Gets the color of the specified location.
    /// </summary>
    /// <param name="location">A location in the (0, 1) interval, inclusive.</param>
    /// <returns></returns>
    public Color GetColor(double location)
    {
      List<GradientColorEntry> sortedColorEntries = CreateSortedColorEntries();
      if (sortedColorEntries.Count > 0)
      {
        if (location <= sortedColorEntries[0].Location) return sortedColorEntries[0].Color;
        if (location >= sortedColorEntries[sortedColorEntries.Count - 1].Location)
          return sortedColorEntries[sortedColorEntries.Count - 1].Color;
        for (int i = 1; i < sortedColorEntries.Count; i++)
        {
          if (location <= sortedColorEntries[i].Location)
          {
            double t = (sortedColorEntries[i].Location == sortedColorEntries[i - 1].Location
                          ? 0.0
                          : (location - sortedColorEntries[i - 1].Location)/
                            (sortedColorEntries[i].Location - sortedColorEntries[i - 1].Location));
            Color c0 = sortedColorEntries[i - 1].Color;
            Color c1 = sortedColorEntries[i].Color;
            return Color.FromArgb(
              c0.A + (int) (t*(c1.A - c0.A)),
              c0.R + (int) (t*(c1.R - c0.R)),
              c0.G + (int) (t*(c1.G - c0.G)),
              c0.B + (int) (t*(c1.B - c0.B)));
          }
        }
      }
      return Color.Black;
    }

    /// <summary>
    /// The (possible unsorted) list of color entries.
    /// </summary>
    public List<GradientColorEntry> ColorEntries
    {
      get { return colorEntries; }
      set 
      { 
        colorEntries = value;
        CreateSortedColorEntries();
      }
    }

    private List<GradientColorEntry> CreateSortedColorEntries()
    {
      List<GradientColorEntry> sortedColorEntries = new List<GradientColorEntry>();

      foreach (GradientColorEntry colorEntry in colorEntries)
      {
        sortedColorEntries.Add(colorEntry);
      }
      sortedColorEntries.Sort();
      return sortedColorEntries;
    }

    /// <summary>
    /// The name of the gradient.
    /// </summary>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <summary>
    /// The name of the file where the gradient is stored.
    /// </summary>
    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }

    /// <summary>
    /// Checks whether obj is a gradient with the same name, colors and locations as this gradient.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if (!(obj.GetType() == typeof(Gradient))) return false;

      Gradient other = obj as Gradient;
      if (other == null || name != other.Name) return false;
      for (int i = 0; i < colorEntries.Count; i++)
      {
        if (colorEntries[i].Color != other.ColorEntries[i].Color ||
           colorEntries[i].Location != other.ColorEntries[i].Location)
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public static void FillCheckerboardRectangle(Graphics g, Rectangle rect, int checkerboardBoxSize)
    {
      int x = rect.Left;
      int y = rect.Top;
      int brushIndex = 0;
      int rowIndex = 0;
      Brush[] checkerboardBrushes = new Brush[] { Brushes.LightGray, Brushes.White };
      while (y <= rect.Bottom)
      {
        while (x <= rect.Right)
        {
          g.FillRectangle(checkerboardBrushes[brushIndex], x, y, Math.Min(checkerboardBoxSize, rect.Right - x), Math.Min(checkerboardBoxSize, rect.Bottom - y));
          x += checkerboardBoxSize;
          brushIndex = (brushIndex + 1) % 2;
        }
        x = rect.Left;
        rowIndex += 1;
        brushIndex = rowIndex % 2;
        y += checkerboardBoxSize;
      }
    }
    
    public void Save(string saveAsFileName)
    {
      IFormatter formatter = new BinaryFormatter();
      try
      {
        Stream stream = new FileStream(saveAsFileName, FileMode.Create, FileAccess.Write, FileShare.None);
        try
        {
          formatter.Serialize(stream, this);
        }
        catch(Exception)
        {
        }
        stream.Close();
      }
      catch(Exception)
      {

      }
    }

    public static Gradient Load(string fileName)
    {
      IFormatter formatter = new BinaryFormatter();
      try
      {
        Stream stream = new FileStream(fileName, FileMode.Open);
        try
        {
          Gradient g = (Gradient)formatter.Deserialize(stream);
          stream.Close();
          g.FileName = fileName;
          return g;
        }
        catch (Exception)
        {
          stream.Close();
          return null;
        }
      }
      catch(Exception)
      {
        return null;
      }
    }

    /// <summary>
    /// Drawing direction of the gradient.
    /// </summary>
    public enum Direction
    {
      Horizontal,
      Vertical,
      ReverseHorizontal,
      ReverseVertical
    }
  }

  /// <summary>
  /// A color-location pair that describes an entry in a gradient color list.
  /// </summary>
  [Serializable]
  public class GradientColorEntry : IComparable<GradientColorEntry>
  {
    private int colorArgb;
    [NonSerialized]
    private Color color;
    private double location;

    public GradientColorEntry(Color color, double location)
    {
      this.color = color;
      this.colorArgb = color.ToArgb();
      this.location = location;
    }

    /// <summary>
    /// The color.
    /// </summary>
    public Color Color
    {
      get { return color; }
      set 
      { 
        color = value;
        colorArgb = color.ToArgb();
      }
    }

    /// <summary>
    /// The location, in the (0, 1) interval, inclusive.
    /// </summary>
    public double Location
    {
      get { return location; }
      set { location = value; }
    }

    /// <summary>
    /// Comprison of color entries based on their locations.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(GradientColorEntry other)
    {
      return Math.Sign(location - other.Location);
    }
  }

}
