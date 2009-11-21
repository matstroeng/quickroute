using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using QuickRoute.BusinessEntities.RouteProperties;

namespace QuickRoute.BusinessEntities
{
  public class LapInfo : Lap
  {
    /// <summary>
    /// The order number to display for this lap.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// The index of this lap in a LapCollection.
    /// </summary>
    public int Index { get; set; }

    private readonly List<RouteProperty> properties = new List<RouteProperty>();

    public void AddProperty(RouteProperty property)
    {
      if (property == null)
      {

      }
      properties.Add(property);
    }

    public List<RouteProperty> GetProperties()
    {
      return new List<RouteProperty>(properties);
    }

    public RouteProperty this[int index]
    {
      get
      {
        return GetProperties()[index];
      }
    }

    /// <summary>
    /// Compares a property of this object with a property of another LapInfo object. If they are equal, use the LapInfo index as a comparison.
    /// </summary>
    /// <param name="other">The LapInfo object to compare with.</param>
    /// <param name="propertyIndex">The zero-based index of the property to compare.</param>
    /// <returns></returns>
    public int CompareProperty(LapInfo other, int propertyIndex)
    {
      if(propertyIndex > properties.Count-1 || propertyIndex > other.GetProperties().Count-1) return 0;
      var value = this[propertyIndex].CompareTo(other[propertyIndex]);
      if (value == 0) value = Index == -1 ? 1 : (other.Index == -1 ? -1 : Index.CompareTo(other.Index));
      return value;
    }
  }

  public class RoutePropertyType
  {
    public Type Type { get; set; }
    public ParameterizedLocation MomentaneousLocation { get; set; }
    public ParameterizedLocation SpanStart { get; set; }
    public ParameterizedLocation SpanEnd { get; set; }

    public RoutePropertyType(RouteProperty rp)
    {
      Type = rp.GetType();
      var rmp = rp as RouteMomentaneousProperty;
      if (rmp != null)
      {
        MomentaneousLocation = rmp.Location;
      }
      var rsp = rp as RouteSpanProperty;
      if (rsp != null)
      {
        SpanStart = rsp.Start;
        SpanEnd = rsp.End;
      }
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(RoutePropertyType)) return false;
      return Equals((RoutePropertyType)obj);
    }

    public bool Equals(RoutePropertyType obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return Equals(obj.Type, Type) && Equals(obj.MomentaneousLocation, MomentaneousLocation) && Equals(obj.SpanStart, SpanStart) && Equals(obj.SpanEnd, SpanEnd);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = (Type != null ? Type.GetHashCode() : 0);
        result = (result * 397) ^ (MomentaneousLocation != null ? MomentaneousLocation.GetHashCode() : 0);
        result = (result * 397) ^ (SpanStart != null ? SpanStart.GetHashCode() : 0);
        result = (result * 397) ^ (SpanEnd != null ? SpanEnd.GetHashCode() : 0);
        return result;
      }
    }
  }
}
