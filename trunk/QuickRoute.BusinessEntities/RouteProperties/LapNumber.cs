using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class LapNumber : RouteMomentaneousProperty
  {
    public LapNumber(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    public LapNumber(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
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
      // remove all start of segments (if they are not the end of another segment at the same time)
      var lapTimeCounts = new Dictionary<DateTime, int>();
      var lapTimesWithoutSegmentStarts = new List<DateTime>();
      foreach (var lapTime in Session.Route.LapTimes)
      {
        if (!lapTimeCounts.ContainsKey(lapTime)) lapTimeCounts.Add(lapTime, 0);
        lapTimeCounts[lapTime]++;
      }
      foreach (var rs in Session.Route.Segments)
      {
        lapTimeCounts[rs.FirstWaypoint.Time]--;
      }
      foreach (var item in lapTimeCounts)
      {
        for(var i=0; i<item.Value; i++)
        {
          lapTimesWithoutSegmentStarts.Add(item.Key);
        }
      }

      var time = Session.Route.GetTimeFromParameterizedLocation(Location);
      value = lapTimesWithoutSegmentStarts.Count - 1;
      for (var i = 0; i < lapTimesWithoutSegmentStarts.Count; i++)
      {
        if (time <= lapTimesWithoutSegmentStarts[i])
        {
          value = i+1;
          break;
        }
      }
      AddToCache();
    }

    public override int CompareTo(object obj)
    {
      return ((int)Value).CompareTo((int)(((RouteProperty)obj).Value));
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      return string.Format(provider, format ?? "{0}", v);
    }

    public override string MaxWidthString
    {
      get { return ValueToString(999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
