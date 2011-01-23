using System;
using System.Collections.Generic;
using System.Drawing;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class PointD
  {
    private double x;
    private double y;

    public PointD()
    {
    }

    public PointD(double x, double y)
    {
      this.x = x;
      this.y = y;
    }

    public double X
    {
      get { return x; }
      set { x = value; }
    }

    public double Y
    {
      get { return y; }
      set { y = value; }
    }

    public override string ToString()
    {
      return X + ", " + Y;
    }

    public static PointD operator -(PointD p0, PointD p1)
    {
      return new PointD(p0.X - p1.X, p0.Y - p1.Y);
    }

    public static PointD operator +(PointD p0, PointD p1)
    {
      return new PointD(p0.X + p1.X, p0.Y + p1.Y);
    }

    public static PointD operator *(double t, PointD p)
    {
      return new PointD(t * p.X, t * p.Y);
    }

    public static PointD operator *(PointD p, double t)
    {
      return new PointD(t * p.X, t * p.Y);
    }

    public static PointD operator /(PointD p, double t)
    {
      return new PointD(p.X / t, p.Y / t);
    }

    public static explicit operator PointF(PointD p)
    {
      return new PointF((float)p.X, (float)p.Y);
    }

    public static explicit operator PointD(Point p)
    {
      return new PointD(p.X, p.Y);
    }

    public static implicit operator PointD(PointF p)
    {
      return new PointD(p.X, p.Y);
    }
  }

  [Serializable]
  public class SizeD
  {
    private double width;
    private double height;

    public SizeD(double width, double height)
    {
      this.width = width;
      this.height = height;
    }

    public double Width
    {
      get { return width; }
      set { width = value; }
    }

    public double Height
    {
      get { return height; }
      set { height = value; }
    }
  }

  [Serializable]
  public class RectangleD
  {
    private double left;
    private double top;
    private double width;
    private double height;

    public RectangleD(double left, double top, double width, double height)
    {
      this.left = left;
      this.top = top;
      this.width = width;
      this.height = height;
    }

    public RectangleD(PointD location, SizeD size)
    {
      left = location.X;
      top = location.Y;
      width = size.Width;
      height = size.Height;
    }

    public double Left
    {
      get { return left; }
      set { left = value; }
    }

    public double Top
    {
      get { return top; }
      set { top = value; }
    }

    public double Right
    {
      get { return left + width; }
    }

    public double Bottom
    {
      get { return top + height; }
    }

    public double Width
    {
      get { return width; }
      set
      {
        if (value < 0)
        {
          left += value;
          width = Math.Abs(value);
        }
        else
        {
          width = value;
        }
      }
    }

    public double Height
    {
      get { return height; }
      set
      {
        if (value < 0)
        {
          top += value;
          height = Math.Abs(value);
        }
        else
        {
          height = value;
        }
      }
    }

    public SizeD Size
    {
      get { return new SizeD(width, height); }
      set
      {
        Width = value.Width;
        Height = value.Height;
      }
    }

    public PointD Center
    {
      get { return new PointD(left + width / 2, top + height / 2); }
    }

    public PointD UpperLeft
    {
      get { return new PointD(left, top); }
    }

    public PointD LowerLeft
    {
      get { return new PointD(left, top + height); }
    }

    public PointD UpperRight
    {
      get { return new PointD(left + width, top); }
    }

    public PointD LowerRight
    {
      get { return new PointD(left + width, top + height); }
    }

    public bool Contains(PointD p)
    {
      return p.X >= left &&
             p.X <= Right &&
             p.Y >= top &&
             p.Y <= Bottom;
    }
  }

  /// <summary>
  /// Class for storing longitude/latitude coordinates.
  /// </summary>
  [Serializable]
  public class LongLat
  {
    private double longitude;
    private double latitude;

    public double Longitude
    {
      get { return longitude; }
      set
      {
        longitude = ((value + 180.0) % 360.0) - 180.0;
      }
    }

    public double Latitude
    {
      get { return latitude; }
      set
      {
        latitude = ((value + 90.0) % 180.0) - 90.0;
      }
    }

    public LongLat()
    {
      longitude = 0.0;
      latitude = 0.0;
    }

    public LongLat(double longitude, double latitude)
    {
      Longitude = longitude;
      Latitude = latitude;
    }

    public static LongLat operator -(LongLat p0, LongLat p1)
    {
      return new LongLat(p0.Longitude - p1.Longitude, p0.Latitude - p1.Latitude);
    }

    public static LongLat operator +(LongLat p0, LongLat p1)
    {
      return new LongLat(p0.Longitude + p1.Longitude, p0.Latitude + p1.Latitude);
    }

    public static LongLat operator *(double t, LongLat p)
    {
      return new LongLat(t * p.Longitude, t * p.Latitude);
    }

    public static LongLat operator *(LongLat p, double t)
    {
      return new LongLat(t * p.Longitude, t * p.Latitude);
    }

    public static LongLat operator /(LongLat p, double t)
    {
      return new LongLat(p.Longitude / t, p.Latitude / t);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as LongLat);
    }

    public bool Equals(LongLat other)
    {
      if (other == null) return false;
      return (longitude == other.Longitude && latitude == other.Latitude);
    }

    /// <summary>
    /// Gets the 3D coordinates relative to the center of the earth.
    /// </summary>
    /// <returns></returns>
    public GeneralMatrix To3DPoint()
    {
      // use spherical coordinates: rho, phi, theta
      const double rho = 6378200; // earth radius in metres

      double sinPhi = Math.Sin(0.5 * Math.PI + latitude / 180.0 * Math.PI);
      double cosPhi = Math.Cos(0.5 * Math.PI + latitude / 180.0 * Math.PI);
      double sinTheta = Math.Sin(longitude / 180.0 * Math.PI);
      double cosTheta = Math.Cos(longitude / 180.0 * Math.PI);

      GeneralMatrix p = new GeneralMatrix(3, 1);
      p.SetElement(0, 0, rho * sinPhi * cosTheta);
      p.SetElement(1, 0, rho * sinPhi * sinTheta);
      p.SetElement(2, 0, rho * cosPhi);
      return p;
    }

    public override string ToString()
    {
      return ToString(new LongLatFormatter("N", "S", "E", "W", true, 0));
    }

    public string ToString(LongLatFormatter formatter)
    {
      double lon = Math.Abs(longitude);
      double lat = Math.Abs(latitude);
      string secondFormat = "00" + (formatter.SecondDecimalPlaces > 0 ? "." + new string('0', formatter.SecondDecimalPlaces) : "");
      return
        Math.Floor(lat) + "° " +
        Math.Floor(60 * (lat - Math.Floor(lat))).ToString("00") + "' " +
        (formatter.SecondsVisible ? Math.Floor((3600 * (lat - Math.Floor(lat))) % 60).ToString(secondFormat) + "\" " : "") +
        (latitude < 0 ? formatter.S : formatter.N) + "   " +
        Math.Floor(lon) + "° " +
        Math.Floor(60 * (lon - Math.Floor(lon))).ToString("00") + "' " +
        (formatter.SecondsVisible ? Math.Floor((3600 * (lon - Math.Floor(lon))) % 60).ToString(secondFormat) + "\" " : "") +
        (longitude < 0 ? formatter.W : formatter.E);
    }


    /// <summary>
    /// Applies an ortographic projection to the coordinate.
    /// http://en.wikipedia.org/wiki/Orthographic_projection_%28cartography%29
    /// </summary>
    /// <param name="projectionOrigin">The origin longitude/latitude coordinate of the projection</param>
    /// <returns>A point that gives the distance in meters from the projection origin</returns>
    public PointD Project(LongLat projectionOrigin)
    {
      const double rho = 6378200; // earth radius in metres
      double lambda0 = projectionOrigin.Longitude * Math.PI / 180.0;
      double phi0 = projectionOrigin.Latitude * Math.PI / 180.0;

      double lambda = longitude * Math.PI / 180.0;
      double phi = latitude * Math.PI / 180.0;
      return new PointD(rho * Math.Cos(phi) * Math.Sin(lambda - lambda0),
                        rho * (Math.Cos(phi0) * Math.Sin(phi) - Math.Sin(phi0) * Math.Cos(phi) * Math.Cos(lambda - lambda0)));
    }

    public static LongLat Deproject(PointD coordinate, LongLat projectionOrigin)
    {
      if (LinearAlgebraUtil.DistancePointToPoint(coordinate, new PointD(0, 0)) < 0.0000001)
        return new LongLat(projectionOrigin.Longitude, projectionOrigin.Latitude);
      const double r = 6378200; // earth radius in metres
      var longLat = new LongLat();
      var rho = Math.Sqrt(coordinate.X * coordinate.X + coordinate.Y * coordinate.Y);
      var c = Math.Asin(rho / r);
      var lambda0 = projectionOrigin.Longitude * Math.PI / 180.0;
      var phi1 = projectionOrigin.Latitude * Math.PI / 180.0;
      longLat.Latitude =
        Math.Asin(Math.Cos(c) * Math.Sin(phi1) +
                  (coordinate.Y * Math.Sin(c) * Math.Cos(phi1) / rho)) / Math.PI * 180.0;
      longLat.Longitude = (lambda0 +
                          Math.Atan(coordinate.X * Math.Sin(c) /
                                    (rho * Math.Cos(phi1) * Math.Cos(c) -
                                     coordinate.Y * Math.Sin(phi1) * Math.Sin(c)))) / Math.PI * 180.0;
      return longLat;
    }

  }

  public class LongLatFormatter
  {
    public LongLatFormatter(string n, string s, string e, string w, bool secondsVisible, int secondDecimalPlaces)
    {
      this.N = n;
      this.S = s;
      this.E = e;
      this.W = w;
      this.SecondsVisible = secondsVisible;
      this.SecondDecimalPlaces = secondDecimalPlaces;
    }

    public string N { get; set; }
    public string S { get; set; }
    public string E { get; set; }
    public string W { get; set; }
    public bool SecondsVisible { get; set; }
    public int SecondDecimalPlaces { get; set; }
  }

  /// <summary>
  /// Container class for transformation matrix and a projection origin, i e the data needed to transform from longlat space to pixel space.
  /// </summary>
  public class Transformation
  {
    public GeneralMatrix TransformationMatrix { get; set; }
    public LongLat ProjectionOrigin { get; set; }

    public Transformation()
    {
    }

    public Transformation(LongLatBox longLatBox, Size imageSize)
    {
      // calculate projection origin
      ProjectionOrigin = new LongLat((longLatBox.East + longLatBox.West) / 2,
                                     (longLatBox.North + longLatBox.South) / 2);

      // get image corners from kml file
      var imageCornerLongLats = longLatBox.GetRotatedBoxCornerLongLats();

      // project them on flat surface
      var projectedImageCorners = new Dictionary<Corner, PointD>();
      projectedImageCorners[Corner.NorthWest] = imageCornerLongLats[Corner.NorthWest].Project(ProjectionOrigin);
      projectedImageCorners[Corner.SouthEast] = imageCornerLongLats[Corner.SouthEast].Project(ProjectionOrigin);

      // calculate transformation matrix
      TransformationMatrix = LinearAlgebraUtil.CalculateTransformationMatrix(
        projectedImageCorners[Corner.NorthWest], new PointD(0, 0),
        projectedImageCorners[Corner.SouthEast], new PointD(imageSize.Width - 1, imageSize.Height - 1), null, true);
    }


    public Transformation(GeneralMatrix transformationMatrix, LongLat projectionOrigin)
    {
      TransformationMatrix = transformationMatrix;
      ProjectionOrigin = projectionOrigin;
    }
  }

  public class LongLatBox
  {
    public double North { get; set; }
    public double South { get; set; }
    public double West { get; set; }
    public double East { get; set; }
    public double Rotation { get; set; }

    public Dictionary<Corner, LongLat> GetRotatedBoxCornerLongLats()
    {
      var rotation = -Rotation;

      var corners = new Dictionary<Corner, LongLat>();
      corners[Corner.NorthEast] = new LongLat(East, North);
      corners[Corner.NorthWest] = new LongLat(West, North);
      corners[Corner.SouthWest] = new LongLat(West, South);
      corners[Corner.SouthEast] = new LongLat(East, South);

      var projectionOrigin = new LongLat((East + West) / 2, (North + South) / 2);

      var projectedMapCenter = projectionOrigin.Project(projectionOrigin);

      var projectedCorners = new Dictionary<Corner, PointD>();
      projectedCorners[Corner.NorthEast] = corners[Corner.NorthEast].Project(projectionOrigin);
      projectedCorners[Corner.NorthWest] = corners[Corner.NorthWest].Project(projectionOrigin);
      projectedCorners[Corner.SouthWest] = corners[Corner.SouthWest].Project(projectionOrigin);
      projectedCorners[Corner.SouthEast] = corners[Corner.SouthEast].Project(projectionOrigin);

      var projectedRotatedCorners = new Dictionary<Corner, PointD>();
      projectedRotatedCorners[Corner.NorthEast] = LinearAlgebraUtil.Rotate(projectedCorners[Corner.NorthEast], projectedMapCenter, rotation);
      projectedRotatedCorners[Corner.NorthWest] = LinearAlgebraUtil.Rotate(projectedCorners[Corner.NorthWest], projectedMapCenter, rotation);
      projectedRotatedCorners[Corner.SouthWest] = LinearAlgebraUtil.Rotate(projectedCorners[Corner.SouthWest], projectedMapCenter, rotation);
      projectedRotatedCorners[Corner.SouthEast] = LinearAlgebraUtil.Rotate(projectedCorners[Corner.SouthEast], projectedMapCenter, rotation);

      var rotatedCorners = new Dictionary<Corner, LongLat>();
      rotatedCorners[Corner.NorthWest] = LongLat.Deproject(projectedRotatedCorners[Corner.NorthWest], projectionOrigin);
      rotatedCorners[Corner.NorthEast] = LongLat.Deproject(projectedRotatedCorners[Corner.NorthEast], projectionOrigin);
      rotatedCorners[Corner.SouthWest] = LongLat.Deproject(projectedRotatedCorners[Corner.SouthWest], projectionOrigin);
      rotatedCorners[Corner.SouthEast] = LongLat.Deproject(projectedRotatedCorners[Corner.SouthEast], projectionOrigin);
      return rotatedCorners;
    }
  }

  public enum Corner
  {
    NorthWest,
    NorthEast,
    SouthWest,
    SouthEast
  }


  public class AlphaAdjustmentChange : IComparable<AlphaAdjustmentChange>
  {
    private ParameterizedLocation parameterizedLocation;
    private double alphaAdjustment;

    public AlphaAdjustmentChange(ParameterizedLocation parameterizedLocation, double alphaAdjustment)
    {
      this.parameterizedLocation = parameterizedLocation;
      this.alphaAdjustment = alphaAdjustment;
    }

    public ParameterizedLocation ParameterizedLocation
    {
      get { return parameterizedLocation; }
      set { parameterizedLocation = value; }
    }

    public double AlphaAdjustment
    {
      get { return alphaAdjustment; }
      set { alphaAdjustment = Math.Max(-1, Math.Min(1, value)); }
    }

    #region IComparable<AlphaAdjustmentChange> Members

    public int CompareTo(AlphaAdjustmentChange other)
    {
      return parameterizedLocation.CompareTo(other.ParameterizedLocation);
    }

    #endregion
  }

  public static class ConvertUtil
  {
    public static GeneralMatrix To3x1Matrix(PointD pointD)
    {
      GeneralMatrix m = new GeneralMatrix(3, 1);
      m.SetElement(0, 0, pointD.X);
      m.SetElement(1, 0, pointD.Y);
      m.SetElement(2, 0, 1);
      return m;
    }

    public static GeneralMatrix To3x1Matrix(LongLat longLat)
    {
      GeneralMatrix m = new GeneralMatrix(3, 1);
      m.SetElement(0, 0, longLat.Longitude);
      m.SetElement(1, 0, longLat.Latitude);
      m.SetElement(2, 0, 1);
      return m;
    }

    public static PointF ToPointF(GeneralMatrix _3x1Matrix)
    {
      return new PointF((float)_3x1Matrix.GetElement(0, 0), (float)_3x1Matrix.GetElement(1, 0));
    }

    public static PointD ToPointD(GeneralMatrix _3x1Matrix)
    {
      return new PointD(_3x1Matrix.GetElement(0, 0), _3x1Matrix.GetElement(1, 0));
    }

    /// <summary>
    /// Converts a speed (km/h) to a pace (min/km).
    /// </summary>
    /// <param name="speed">The speed in kilometres per hour.</param>
    /// <returns></returns>
    public static TimeSpan ToPace(double speed)
    {
      return speed == 0 ? new TimeSpan(0) : new TimeSpan((long)(3600.0 / speed * TimeSpan.TicksPerSecond));
    }

    /// <summary>
    /// Converts a pace (min/km) to a speed (km/h).
    /// </summary>
    /// <param name="pace">The pace in minutes per kilometre.</param>
    /// <returns></returns>
    public static double ToSpeed(TimeSpan pace)
    {
      return pace.TotalSeconds == 0 ? 0 : 3600.0 / pace.TotalSeconds;
    }
  }

  public static class LinearAlgebraUtil
  {
    public static double ClosestDistancePointToLine(PointD p, PointD l0, PointD l1, out double lineParameter)
    {
      PointD p1;
      double t;
      double tmpDist;
      double closestDistance;

      // Check endpoints
      closestDistance = DistancePointToPoint(l0, p);
      lineParameter = 0.0;

      tmpDist = DistancePointToPoint(l1, p);
      if (tmpDist < closestDistance)
      {
        closestDistance = tmpDist;
        lineParameter = 1.0;
      }

      // Check normal line
      if (l0.X == l1.X)
      {
        p1 = new PointD(p.X + 1.0, p.Y);
      }
      else if (l0.Y == l1.Y)
      {
        p1 = new PointD(p.X, p.Y + 1.0);
      }
      else
      {
        p1 = new PointD(p.X + 1.0, p.Y - (l1.X - l0.X) / (l1.Y - l0.Y));
      }

      LinesIntersect(l0, l1, p, p1, out t);

      if (t >= 0.0 && t <= 1.0)
      {
        tmpDist = DistancePointToPoint(new PointD(l0.X + t * (l1.X - l0.X), l0.Y + t * (l1.Y - l0.Y)), p);
        if (tmpDist < closestDistance)
        {
          closestDistance = tmpDist;
          lineParameter = t;
        }
      }
      return closestDistance;
    }

    /// <summary>
    /// Distance in longitude/latitude units, not meters
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <returns></returns>
    public static double DistancePointToPoint(LongLat p0, LongLat p1)
    {
      return Math.Sqrt((p1.Longitude - p0.Longitude) * (p1.Longitude - p0.Longitude) + (p1.Latitude - p0.Latitude) * (p1.Latitude - p0.Latitude));
    }

    public static double DistancePointToPoint(PointD p0, PointD p1)
    {
      return Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
    }

    public static double DistancePointToPoint(PointF p0, PointF p1)
    {
      return Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
    }

    public static double DistancePointToPoint(GeneralMatrix p0, GeneralMatrix p1)
    {
      if (p0.ColumnDimension != 1 || p1.ColumnDimension != 1 || p0.RowDimension != p1.RowDimension) throw new Exception("The matrices should have one column and equal number of rows.");
      double sum = 0.0;
      for (int i = 0; i < p0.RowDimension; i++)
        sum += (p1.GetElement(i, 0) - p0.GetElement(i, 0)) * (p1.GetElement(i, 0) - p0.GetElement(i, 0));
      return Math.Sqrt(sum);
    }

    public static bool LinesIntersect(PointD l0p0, PointD l0p1, PointD l1p0, PointD l1p1, out double parameter)
    {
      double a;
      double b;
      parameter = 0.0;

      // Check if any line has zero length
      if ((l0p0.X == l0p1.X && l0p0.Y == l0p1.Y) || (l1p0.X == l1p1.X && l1p0.Y == l1p1.Y)) return false;
      // Check if lines are parallell
      if (l0p1.X == l0p0.X && l1p1.X == l1p0.X) return false;
      if (!((l0p1.X == l0p0.X && l1p1.X != l1p0.X) || (l0p1.X != l0p0.X && l1p1.X == l1p0.X)))
      {
        if ((l0p1.Y - l0p0.Y) / (l0p1.X - l0p0.X) == (l1p1.Y - l1p0.Y) / (l1p1.X - l1p0.X)) return false;
      }

      b = ((l0p0.X - l1p0.X) * (l0p1.Y - l0p0.Y) + (l1p0.Y - l0p0.Y) * (l0p1.X - l0p0.X)) / ((l1p1.X - l1p0.X) * (l0p1.Y - l0p0.Y) - (l1p1.Y - l1p0.Y) * (l0p1.X - l0p0.X));
      if (l0p1.X == l0p0.X)
      {
        a = (l1p0.Y - l0p0.Y + b * (l1p1.Y - l1p0.Y)) / (l0p1.Y - l0p0.Y);
      }
      else
      {
        a = (l1p0.X - l0p0.X + b * (l1p1.X - l1p0.X)) / (l0p1.X - l0p0.X);
      }

      parameter = a;
      return (a >= 0.0 && a <= 1.0 && b >= 0.0 && b <= 1.0);
    }

    public static bool LineInfiniteLineIntersect(PointD l0p0, PointD l0p1, PointD l1p, PointD l1d, out double parameter)
    {
      double a;
      double b;
      parameter = 0.0;
      PointD l1p1 = l1p + Normalize(l1d);

      // Check if the first line has zero length
      if ((l0p0.X == l0p1.X && l0p0.Y == l0p1.Y)) return false;
      // Check if lines are parallell
      if (l0p1.X == l0p0.X && l1p1.X == l1p.X) return false;
      if (!((l0p1.X == l0p0.X && l1p1.X != l1p.X) || (l0p1.X != l0p0.X && l1p1.X == l1p.X)))
      {
        if ((l0p1.Y - l0p0.Y) / (l0p1.X - l0p0.X) == (l1p1.Y - l1p.Y) / (l1p1.X - l1p.X)) return false;
      }

      b = ((l0p0.X - l1p.X) * (l0p1.Y - l0p0.Y) + (l1p.Y - l0p0.Y) * (l0p1.X - l0p0.X)) / ((l1p1.X - l1p.X) * (l0p1.Y - l0p0.Y) - (l1p1.Y - l1p.Y) * (l0p1.X - l0p0.X));
      if (l0p1.X == l0p0.X)
      {
        a = (l1p.Y - l0p0.Y + b * (l1p1.Y - l1p.Y)) / (l0p1.Y - l0p0.Y);
      }
      else
      {
        a = (l1p.X - l0p0.X + b * (l1p1.X - l1p.X)) / (l0p1.X - l0p0.X);
      }

      parameter = a;
      return (a >= 0.0 && a <= 1.0);
    }

    public static PointD InfiniteLinesIntersectionPoint(PointD l0p, PointD l0d, PointD l1p, PointD l1d)
    {
      double b;
      PointD l0p1 = l0p + Normalize(l0d);
      PointD l1p1 = l1p + Normalize(l1d);

      // Check if lines are parallell
      if (l0p1.X == l0p.X && l1p1.X == l1p.X) return l0p;
      if (!((l0p1.X == l0p.X && l1p1.X != l1p.X) || (l0p1.X != l0p.X && l1p1.X == l1p.X)))
      {
        if ((l0p1.Y - l0p.Y) / (l0p1.X - l0p.X) == (l1p1.Y - l1p.Y) / (l1p1.X - l1p.X)) return l0p;
      }

      b = ((l0p.X - l1p.X) * (l0p1.Y - l0p.Y) + (l1p.Y - l0p.Y) * (l0p1.X - l0p.X)) / ((l1p1.X - l1p.X) * (l0p1.Y - l0p.Y) - (l1p1.Y - l1p.Y) * (l0p1.X - l0p.X));

      return l1p + b * l1d;
    }

    /// <summary>
    /// Speed in metres per second
    /// </summary>
    /// <param name="longLat0">longitude and latitude for first point</param>
    /// <param name="longLat1">longitude and latitude for second point</param>
    /// <param name="t0">time for first point</param>
    /// <param name="t1">time for first point</param>
    /// <returns></returns>
    public static double CalculateSpeed(LongLat longLat0, LongLat longLat1, DateTime t0, DateTime t1)
    {
      double distance = DistancePointToPointLongLat(longLat0, longLat1);
      TimeSpan timeSpan = t1.Subtract(t0);

      double speed = distance / timeSpan.TotalSeconds;
      return speed;
    }

    /// <summary>
    /// Distance in metres between two points expressed as longitude/latitude
    /// </summary>
    /// <param name="longLat0">longitude and latitude for first point</param>
    /// <param name="longLat1">longitude and latitude for second point</param>
    /// <returns></returns>
    public static double DistancePointToPointLongLat(LongLat longLat0, LongLat longLat1)
    {
      // use spherical coordinates: rho, phi, theta
      const double rho = 6378200; // earth radius in metres

      double sinPhi0 = Math.Sin(0.5 * Math.PI + longLat0.Latitude / 180.0 * Math.PI);
      double cosPhi0 = Math.Cos(0.5 * Math.PI + longLat0.Latitude / 180.0 * Math.PI);
      double sinTheta0 = Math.Sin(longLat0.Longitude / 180.0 * Math.PI);
      double cosTheta0 = Math.Cos(longLat0.Longitude / 180.0 * Math.PI);

      double sinPhi1 = Math.Sin(0.5 * Math.PI + longLat1.Latitude / 180.0 * Math.PI);
      double cosPhi1 = Math.Cos(0.5 * Math.PI + longLat1.Latitude / 180.0 * Math.PI);
      double sinTheta1 = Math.Sin(longLat1.Longitude / 180.0 * Math.PI);
      double cosTheta1 = Math.Cos(longLat1.Longitude / 180.0 * Math.PI);

      var p0 = new GeneralMatrix(3, 1);
      p0.SetElement(0, 0, rho * sinPhi0 * cosTheta0);
      p0.SetElement(1, 0, rho * sinPhi0 * sinTheta0);
      p0.SetElement(2, 0, rho * cosPhi0);

      var p1 = new GeneralMatrix(3, 1);
      p1.SetElement(0, 0, rho * sinPhi1 * cosTheta1);
      p1.SetElement(1, 0, rho * sinPhi1 * sinTheta1);
      p1.SetElement(2, 0, rho * cosPhi1);

      double distance = DistancePointToPoint(p0, p1);
      return distance;
    }

    public static PointD Transform(PointD p, GeneralMatrix transformationMatrix)
    {
      return ConvertUtil.ToPointD(transformationMatrix * ConvertUtil.To3x1Matrix(p));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p0">First point on route, in projected (metric) coordinates relative to projection origin</param>
    /// <param name="q0">First point on map, in pixels</param>
    /// <param name="p1">Second point on route, in projected (metric) coordinates relative to projection origin</param>
    /// <param name="q1">Second point on map, in pixels</param>
    /// <param name="fallbackMatrix">Matrix to use if calculation fails due to singular matrix</param>
    /// <param name="useRotation">If true, assumes orthogonal map and calculates scale and rotation. If false, calculates different scale in x and y directions and no rotation.</param>
    /// <returns></returns>
    public static GeneralMatrix CalculateTransformationMatrix(PointD p0, PointD q0, PointD p1, PointD q1, GeneralMatrix fallbackMatrix, bool useRotation)
    {
      try
      {
        if (useRotation)
        {
          // note that we need to mirror y pixel value in x axis
          double angleDifferece = GetAngleR(p1 - p0, new PointD(q1.X, -q1.Y) - new PointD(q0.X, -q0.Y));
          double lengthQ = DistancePointToPoint(q0, q1);
          double lengthP = DistancePointToPoint(p0, p1);
          double scaleFactor = lengthP == 0 ? 0 : lengthQ / lengthP;
          double cos = Math.Cos(angleDifferece);
          double sin = Math.Sin(angleDifferece);

          // translation to origo in metric space
          var a = new GeneralMatrix(3, 3);
          a.SetElement(0, 0, 1);
          a.SetElement(0, 1, 0);
          a.SetElement(0, 2, -p0.X);
          a.SetElement(1, 0, 0);
          a.SetElement(1, 1, 1);
          a.SetElement(1, 2, -p0.Y);
          a.SetElement(2, 0, 0);
          a.SetElement(2, 1, 0);
          a.SetElement(2, 2, 1);

          // rotation
          var b = new GeneralMatrix(3, 3);
          b.SetElement(0, 0, cos);
          b.SetElement(0, 1, -sin);
          b.SetElement(0, 2, 0);
          b.SetElement(1, 0, sin);
          b.SetElement(1, 1, cos);
          b.SetElement(1, 2, 0);
          b.SetElement(2, 0, 0);
          b.SetElement(2, 1, 0);
          b.SetElement(2, 2, 1);

          // scaling, note that we need to mirror y scale around x axis
          var c = new GeneralMatrix(3, 3);
          c.SetElement(0, 0, scaleFactor);
          c.SetElement(0, 1, 0);
          c.SetElement(0, 2, 0);
          c.SetElement(1, 0, 0);
          c.SetElement(1, 1, -scaleFactor);
          c.SetElement(1, 2, 0);
          c.SetElement(2, 0, 0);
          c.SetElement(2, 1, 0);
          c.SetElement(2, 2, 1);

          // translation from origo to pixel space
          var d = new GeneralMatrix(3, 3);
          d.SetElement(0, 0, 1);
          d.SetElement(0, 1, 0);
          d.SetElement(0, 2, q0.X);
          d.SetElement(1, 0, 0);
          d.SetElement(1, 1, 1);
          d.SetElement(1, 2, q0.Y);
          d.SetElement(2, 0, 0);
          d.SetElement(2, 1, 0);
          d.SetElement(2, 2, 1);

          return d * c * b * a;
        }
        else // useRotation == false
        {
          var m1 = new GeneralMatrix(2, 2);
          m1.SetElement(0, 0, p0.X);
          m1.SetElement(0, 1, 1);
          m1.SetElement(1, 0, p1.X);
          m1.SetElement(1, 1, 1);

          var v1 = new GeneralMatrix(2, 1);
          v1.SetElement(0, 0, q0.X);
          v1.SetElement(1, 0, q1.X);
          var t1 = m1.Inverse() * v1;

          var m2 = new GeneralMatrix(2, 2);
          m2.SetElement(0, 0, p0.Y);
          m2.SetElement(0, 1, 1);
          m2.SetElement(1, 0, p1.Y);
          m2.SetElement(1, 1, 1);

          var v2 = new GeneralMatrix(2, 1);
          v2.SetElement(0, 0, q0.Y);
          v2.SetElement(1, 0, q1.Y);
          var t2 = m2.Inverse() * v2;

          var t = new GeneralMatrix(3, 3);
          t.SetElement(0, 0, t1.GetElement(0, 0));
          t.SetElement(0, 1, 0);
          t.SetElement(0, 2, t1.GetElement(1, 0));
          t.SetElement(1, 0, 0);
          t.SetElement(1, 1, t2.GetElement(0, 0));
          t.SetElement(1, 2, t2.GetElement(1, 0));
          t.SetElement(2, 0, 0);
          t.SetElement(2, 1, 0);
          t.SetElement(2, 2, 1);

          return t;
        }
      }
      catch (Exception)
      {
        return (GeneralMatrix)fallbackMatrix.Clone();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p0">First point on route</param>
    /// <param name="q0">First point on map</param>
    /// <param name="p1">Second point on route</param>
    /// <param name="q1">Second point on map</param>
    /// <param name="p2">Third point on route</param>
    /// <param name="q2">Third point on map</param>
    /// <param name="fallbackMatrix">Matrix to use if calculation fails due to singular matrix</param>
    /// <returns></returns>
    public static GeneralMatrix CalculateTransformationMatrix(PointD p0, PointD q0, PointD p1, PointD q1, PointD p2, PointD q2, GeneralMatrix fallbackMatrix)
    {
      try
      {
        var m = new GeneralMatrix(3, 3);
        m.SetElement(0, 0, p0.X);
        m.SetElement(0, 1, p0.Y);
        m.SetElement(0, 2, 1.0);
        m.SetElement(1, 0, p1.X);
        m.SetElement(1, 1, p1.Y);
        m.SetElement(1, 2, 1.0);
        m.SetElement(2, 0, p2.X);
        m.SetElement(2, 1, p2.Y);
        m.SetElement(2, 2, 1.0);

        var v1 = new GeneralMatrix(3, 1);
        v1.SetElement(0, 0, q0.X);
        v1.SetElement(1, 0, q1.X);
        v1.SetElement(2, 0, q2.X);
        var t1 = m.Inverse() * v1;

        var v2 = new GeneralMatrix(3, 1);
        v2.SetElement(0, 0, q0.Y);
        v2.SetElement(1, 0, q1.Y);
        v2.SetElement(2, 0, q2.Y);
        var t2 = m.Inverse() * v2;

        var v3 = new GeneralMatrix(3, 1);
        v3.SetElement(0, 0, 1.0);
        v3.SetElement(1, 0, 1.0);
        v3.SetElement(2, 0, 1.0);
        var t3 = m.Inverse() * v3;

        var t = new GeneralMatrix(3, 3);
        t.SetElement(0, 0, t1.GetElement(0, 0));
        t.SetElement(0, 1, t1.GetElement(1, 0));
        t.SetElement(0, 2, t1.GetElement(2, 0));
        t.SetElement(1, 0, t2.GetElement(0, 0));
        t.SetElement(1, 1, t2.GetElement(1, 0));
        t.SetElement(1, 2, t2.GetElement(2, 0));
        t.SetElement(2, 0, t3.GetElement(0, 0));
        t.SetElement(2, 1, t3.GetElement(1, 0));
        t.SetElement(2, 2, t3.GetElement(2, 0));

        return t;
      }
      catch (Exception)
      {
        return (GeneralMatrix)fallbackMatrix.Clone();
      }

    }

    public static PointD Normalize(PointD vector)
    {
      if (vector.X == 0.0 && vector.Y == 0.0) return new PointD(0.0, 0.0);
      double length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
      return new PointD(vector.X / length, vector.Y / length);
    }

    public static PointD GetNormalVector(PointD vector)
    {
      if (vector.X == 0.0 && vector.Y == 0.0) return new PointD(0.0, 0.0);
      PointD normalizedVector = Normalize(vector);
      return new PointD(-normalizedVector.Y, normalizedVector.X);
    }

    /// <summary>
    /// Gets angle in radians (-PI <= a <= PI) of vector relative to x axis.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static double GetAngleR(PointD v)
    {
      PointD normalizedV = Normalize(v);
      double dp = DotProduct(normalizedV, new PointD(1, 0));
      if (dp > 1.0) dp = 1.0;
      else if (dp < -1.0) dp = -1.0;
      double angle;
      if (v.Y < 0)
        angle = 2 * Math.PI - Math.Acos(dp);
      else
        angle = Math.Acos(dp);
      if (angle > Math.PI) angle -= 2 * Math.PI;
      return angle;
    }

    /// <summary>
    /// Gets angle in radians (-PI <= a <= PI) between two vectors.
    /// </summary>
    /// <param name="v0"></param>
    /// <param name="v1"></param>
    /// <returns></returns>
    public static double GetAngleR(PointD v0, PointD v1)
    {
      double a0 = GetAngleR(v0);
      double a1 = GetAngleR(v1) + 2 * Math.PI;

      double diff = (a1 - a0) % (2 * Math.PI);
      if (diff > Math.PI) diff -= 2 * Math.PI;
      return diff;
    }

    /// <summary>
    /// Gets angle in degrees (-180 <= a <= 180) of vector relative to x axis.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static double GetAngleD(PointD v)
    {
      return ToDegrees(GetAngleR(v));
    }

    /// <summary>
    /// Gets angle in degrees (-180 <= a <= 180) between two vectors.
    /// </summary>
    /// <param name="v0"></param>
    /// <param name="v1"></param>
    /// <returns></returns>
    public static double GetAngleD(PointD v0, PointD v1)
    {
      return ToDegrees(GetAngleR(v0, v1));
    }

    public static double GetVectorDirectionR(PointD vectorStartPoint, PointD vectorEndPoint)
    {
      return ((GetAngleR(vectorEndPoint - vectorStartPoint) + Math.PI) % (2 * Math.PI)) - Math.PI;
    }

    public static double DotProduct(PointD v0, PointD v1)
    {
      return v0.X * v1.X + v0.Y * v1.Y;
    }

    public static double GetUnitLengthFromTransformationMatrix(GeneralMatrix transformationMatrix)
    {
      double v0 = transformationMatrix.GetElement(0, 0);
      double v1 = transformationMatrix.GetElement(1, 0);
      return Math.Sqrt(v0 * v0 + v1 * v1);
    }

    /// <summary>
    /// Returns an angle in degrees, E180CW style
    /// </summary>
    /// <param name="angleD">Angle in ExxxCW style</param>
    /// <returns></returns>
    public static double AnglifyD(double angleD)
    {
      return AnglifyD(angleD, AngleStyle.E180CCW);
    }

    /// <summary>
    /// Returns an angle in degrees
    /// </summary>
    /// <param name="degreeAngle">Angle in ExxxCW style</param>
    /// <param name="style"></param>
    /// <returns></returns>
    public static double AnglifyD(double degreeAngle, AngleStyle style)
    {
      switch (style)
      {
        case AngleStyle.E180CCW:
          degreeAngle = degreeAngle % 360;
          if (degreeAngle < -180)
          {
            degreeAngle += 360;
          }
          else if (degreeAngle >= 180)
          {
            degreeAngle -= 360;
          }
          break;

        default:
          degreeAngle = AnglifyD(degreeAngle, AngleStyle.E180CCW);
          degreeAngle = 90 - degreeAngle;

          if (degreeAngle < 0)
          {
            degreeAngle += 360;
          }
          break;
      }
      return degreeAngle;
    }

    /// <summary>
    /// Returns an angle in degrees
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="style"></param>
    /// <returns></returns>
    public static double AnglifyD(PointD vector, AngleStyle style)
    {
      return AnglifyD(ToDegrees(GetAngleR(vector)), style);
    }

    /// <summary>
    /// Angles in radians
    /// </summary>
    /// <param name="angleR0"></param>
    /// <param name="angleR1"></param>
    /// <returns></returns>
    public static double CombineAnglesR(double angleR0, double angleR1)
    {
      return CombineAnglesR(angleR0, 0.5, angleR1, 0.5);
    }

    /// <summary>
    /// Angles in radians
    /// </summary>
    /// <param name="angleR0"></param>
    /// <param name="weight0"></param>
    /// <param name="angleR1"></param>
    /// <param name="weight1"></param>
    /// <returns></returns>
    public static double CombineAnglesR(double angleR0, double weight0, double angleR1, double weight1)
    {
      PointD v0 = weight0 * new PointD(Math.Cos(angleR0), Math.Sin(angleR0));
      PointD v1 = weight1 * new PointD(Math.Cos(angleR1), Math.Sin(angleR1));
      return GetAngleR(v0 + v1);
    }

    /// <summary>
    /// Angles in degrees
    /// </summary>
    /// <param name="angleD0"></param>
    /// <param name="angleD1"></param>
    /// <returns></returns>
    public static double CombineAnglesD(double angleD0, double angleD1)
    {
      return CombineAnglesD(angleD0, 0.5, angleD1, 0.5);
    }

    /// <summary>
    /// Angles in radians
    /// </summary>
    /// <param name="angleD0"></param>
    /// <param name="weight0"></param>
    /// <param name="angleD1"></param>
    /// <param name="weight1"></param>
    /// <returns></returns>
    public static double CombineAnglesD(double angleD0, double weight0, double angleD1, double weight1)
    {
      PointD v0 = weight0 * new PointD(Math.Cos(ToRadians(angleD0)), Math.Sin(ToRadians(angleD0)));
      PointD v1 = weight1 * new PointD(Math.Cos(ToRadians(angleD1)), Math.Sin(ToRadians(angleD1)));
      return ToDegrees(GetAngleR(v0 + v1));
    }

    public static double GetAngleDifferenceD(double angle0, double angle1)
    {
      double difference = angle0 - angle1;
      if (difference >= 180) difference = 360 - difference;
      else if (difference < -180) difference = difference + 360;
      return difference;
    }

    public static double ToDegrees(double angleInRadians)
    {
      return angleInRadians * 180 / Math.PI;
    }

    public static double ToRadians(double angleInDegrees)
    {
      return angleInDegrees * Math.PI / 180;
    }

    public static PointD CreateNormalizedVectorFromAngleR(double angleR)
    {
      return new PointD(Math.Cos(angleR), Math.Sin(angleR));
    }

    public static PointD CreateNormalizedVectorFromAngleD(double angleD)
    {
      return CreateNormalizedVectorFromAngleR(ToRadians(angleD));
    }

    public static PointD Rotate(PointD point, PointD rotationCenter, double angleInRadians)
    {
      var rotated =
        new GeneralMatrix(new[] { 1, 0, 0, 0, 1, 0, rotationCenter.X, rotationCenter.Y, 1 }, 3) *
        new GeneralMatrix(new[] { Math.Cos(angleInRadians), -Math.Sin(angleInRadians), 0, Math.Sin(angleInRadians), Math.Cos(angleInRadians), 0, 0, 0, 1 }, 3) *
        new GeneralMatrix(new[] { 1, 0, 0, 0, 1, 0, -rotationCenter.X, -rotationCenter.Y, 1 }, 3) *
        ConvertUtil.To3x1Matrix(point);

      return new PointD(rotated.GetElement(0, 0), rotated.GetElement(1, 0));
    }

  }

  public static class StatisticsUtil
  {
    public static double GetStandardDeviation(List<double> values)
    {
      double n = values.Count;
      if (n == 0) return 0;
      double sum = 0;
      double squareSum = 0;
      foreach (var v in values)
      {
        sum += v;
        squareSum += v * v;
      }
      double squared = squareSum / n - (sum / n) * (sum / n);
      if (squared < 0) squared = 0;
      return Math.Sqrt(squared);
    }

    public static double GetStandardDeviation(List<WeightedItem> items)
    {
      if (items.Count == 0) return 0;
      double sum = 0;
      double squareSum = 0;
      double weightSum = 0;
      foreach (var item in items)
      {
        sum += item.WeightedValue;
        squareSum += item.Weight * item.Value * item.Value;
        weightSum += item.Weight;
      }
      double squared = squareSum / weightSum - (sum / weightSum) * (sum / weightSum);
      if (squared < 0) squared = 0;
      return Math.Sqrt(squared);
    }


    public class WeightedItem
    {
      public WeightedItem(double weight, double value)
      {
        Value = value;
        Weight = weight;
      }

      public double Weight { get; set; }
      public double Value { get; set; }

      public double WeightedValue
      {
        get { return Weight * Value; }
      }
    }

  }

  public static class FileFormatManager
  {
    public static List<string> GetQuickRouteFileExtensions()
    {
      return new List<string>(new[] { ".qrt", ".jpg", ".jpeg" });
    }
  }


  [Serializable]
  public enum QuickRouteFileFormat
  {
    Qrt,
    Xml,
    Jpeg,
    Unknown
  }

  public enum RouteHandleType
  {
    Adjustment = 0,
    MouseHover = 1
  }

  public enum WaypointAttribute
  {
    Pace,
    HeartRate,
    Altitude,
    Speed,
    DirectionDeviationToNextLap,
    ElapsedTime,
    Distance,
    Inclination,
    Direction,
    Longitude,
    Latitude,
    PixelX,
    PixelY,
    MapReadingState,
    MapReadingDuration,
    PreviousMapReadingEnd,
    NextMapReadingStart
  }

  public struct WaypointAttributeString
  {
    private WaypointAttribute? waypointAttribute;
    private readonly string name;
    private readonly string unit;

    public WaypointAttributeString(WaypointAttribute? waypointAttribute)
    {
      this.waypointAttribute = waypointAttribute;
      switch (waypointAttribute)
      {
        case null:
          name = Strings.NoSecondaryColorCoding;
          unit = "";
          break;
        case BusinessEntities.WaypointAttribute.Pace:
          name = Strings.Pace;
          unit = Strings.Unit_Pace;
          break;
        case BusinessEntities.WaypointAttribute.Speed:
          name = Strings.Speed;
          unit = Strings.Unit_Speed;
          break;
        case BusinessEntities.WaypointAttribute.HeartRate:
          name = Strings.HeartRate;
          unit = Strings.Unit_HeartRate;
          break;
        case BusinessEntities.WaypointAttribute.Altitude:
          name = Strings.Altitude;
          unit = Strings.Unit_Altitude;
          break;
        case BusinessEntities.WaypointAttribute.DirectionDeviationToNextLap:
          name = Strings.Direction;
          unit = Strings.Unit_Direction;
          break;
        case BusinessEntities.WaypointAttribute.MapReadingDuration:
          name = Strings.MapReadingDuration;
          unit = Strings.Unit_Time;
          break;
        default:
          name = "";
          unit = "";
          break;
      }
    }

    public WaypointAttribute? WaypointAttribute
    {
      get { return waypointAttribute; }
      set { waypointAttribute = value; }
    }

    public override string ToString()
    {
      return Name;
    }

    public string Name
    {
      get { return name; }
    }

    public string Unit
    {
      get { return unit; }
    }

  }

  public enum MarkerType
  {
    Start,
    Lap,
    Stop,
    MouseHover,
    Handle,
    ActiveHandle,
    MovingActiveHandle
  }

  public enum CutType
  {
    Before,
    After
  }

  public enum AngleStyle
  {
    N360CW,
    E180CCW
  }

}
