using System;
using QuickRoute.BusinessEntities.Numeric;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class Time : RouteMomentaneousProperty
  {
    public Time(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    public Time(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    protected override void Calculate()
    {
      var cachedProperty = GetFromCache();
      if (cachedProperty != null)
      {
        value = cachedProperty.Value;
        return;
      }
      value = Session.Route.GetTimeFromParameterizedLocation(Location);
      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return (Convert.ToDateTime(Value).CompareTo(Convert.ToDateTime(((RouteProperty)obj).Value)));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if(provider == null)
      {
        var tc = new TimeConverter(TimeConverter.TimeConverterType.TimeOfDay);
        return tc.ToString(Convert.ToDateTime(v));
      }
      return string.Format(provider, format ?? "{0}", Convert.ToDateTime(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString(new DateTime(2000, 01, 01, 23, 59, 59, 0, DateTimeKind.Local)); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}