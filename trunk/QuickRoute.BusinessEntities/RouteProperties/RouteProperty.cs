using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public abstract class RouteProperty : IComparable
  {
    protected bool calculated;
    protected object value;

    protected RouteProperty(Session session, RouteLocations locations)
    {
      Session = session;
      Locations = locations;
    }

    /// <summary>
    /// The session to use when calculating route properties.
    /// </summary>
    protected Session Session { get; private set; }

    /// <summary>
    /// The location/span of the route property.
    /// </summary>
    public RouteLocations Locations { get; private set; }

    /// <summary>
    /// If caching is to be used, set this property to an instance of a RoutePropertyCacheManager.
    /// Caching enables to receive this and other route properties without calculation.
    /// </summary>
    public RoutePropertyCacheManager CacheManager { get; set; }

    /// <summary>
    /// Whether there exists a CacheManager.
    /// </summary>
    protected bool HasCache
    {
      get
      {
        return CacheManager != null;
      }
    }

    /// <summary>
    /// Receive this property (at this location/span) if it exists, or othrewise null.
    /// </summary>
    /// <returns></returns>
    public RouteProperty GetFromCache()
    {
      if(!HasCache) return null;
      return CacheManager.Get(GetType(), Locations);
    }

    /// <summary>
    /// Adds this route property to the cache.
    /// </summary>
    public void AddToCache()
    {
      if(HasCache) CacheManager.Add(this);
    }

    /// <summary>
    /// The value of this route property.
    /// </summary>
    public object Value
    {
      get
      {
        if (!calculated)
        {
          Calculate();
          calculated = true;
        }
        return value;
      }
    }

    /// <summary>
    /// Calculates the value of this property.
    /// </summary>
    protected abstract void Calculate();

    /// <summary>
    /// The string that has the maximum width possible for this property (e g 23:59:59 for a time of day)
    /// </summary>
    public abstract string MaxWidthString { get; }

    /// <summary>
    /// Whether the route contains this type of property. Some properties (altitude and heart rate based) can be null and then false is returned.
    /// </summary>
    public abstract bool ContainsValue { get; }

    public virtual int CompareTo(object obj)
    {
      var other = obj as RouteProperty;
      if (other == null || GetType() != obj.GetType()) return -1;
      if (Value == null && other.Value != null) return 1;
      if (Value != null && other.Value == null) return -1;
      if (Value == null && other.Value == null) return 0;
      return ((double)Value).CompareTo((double)other.Value);
    }

    public override string ToString()
    {
      return ValueToString(Value, null, null);
    }

    public string ToString(string format)
    {
      return ValueToString(Value, format, null);
    }

    public string ToString(IFormatProvider provider)
    {
      return ValueToString(Value, null, provider);
    }

    public string ToString(string format, IFormatProvider provider)
    {
      return ValueToString(Value, format, provider);
    }

    /// <summary>
    /// Internal method for converting a value to a string.
    /// </summary>
    /// <param name="v">The value to be converted.</param>
    /// <param name="format">The format string to use.</param>
    /// <param name="provider">The custom format provider, or null if no format provider.</param>
    /// <returns></returns>
    protected abstract string ValueToString(object v, string format, IFormatProvider provider);

    protected string ValueToString(object v)
    {
      return ValueToString(v, null, null);
    }

    protected string ValueToString(object v, string format)
    {
      return ValueToString(v, format, null);
    }

    protected string ValueToString(object v, IFormatProvider provider)
    {
      return ValueToString(v, null, provider);
    }
}
}
