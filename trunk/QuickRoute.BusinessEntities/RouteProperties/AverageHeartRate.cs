using System;
using System.Collections.Generic;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageHeartRate : RouteSpanProperty
  {
    public AverageHeartRate(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
    }

    public AverageHeartRate(Session session, RouteLocations locations)
      : base(session, locations)
    {
    }

    protected override void Calculate()    
    {
      var pl = new ParameterizedLocation(Start);
      var nodes = new List<HRNode>();
      while (pl <= End)
      {
        var hr = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.HeartRate, pl);
        if(hr.HasValue)
        {
          var nodeType = NodeType.Intermediate;
          if (Session.Route.IsFirstPLInSegment(pl)) nodeType = NodeType.Begin;
          if (Session.Route.IsLastPLInSegment(pl)) nodeType = NodeType.End;
          nodes.Add(new HRNode()
                      {
                        HR = hr.Value,
                        Time = Session.Route.GetTimeFromParameterizedLocation(pl),
                        NodeType = nodeType
                      });
        }
        else
        {
          value = null;
            return;
        }
        if (pl >= End) break;
        pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        if (pl > End) pl = new ParameterizedLocation(End); 
      }

      if (nodes.Count > 0)
      {
        nodes[0].NodeType = NodeType.Begin;
        nodes[nodes.Count - 1].NodeType = NodeType.End;
      }

      var heartbeats = 0.0;
      var totalDuration = 0.0;
      for(var i=0; i<nodes.Count; i++)
      {
        double duration = 0;
        switch(nodes[i].NodeType)
        {
          case NodeType.Begin:
            duration = (nodes[i + 1].Time - nodes[i].Time).TotalSeconds / 2;
            break;
          case NodeType.End:
            duration = (nodes[i].Time - nodes[i-1].Time).TotalSeconds / 2;
            break;
          case NodeType.Intermediate:
            duration = (nodes[i].Time - nodes[i-1].Time).TotalSeconds / 2 + (nodes[i + 1].Time - nodes[i].Time).TotalSeconds / 2;
            break;
        }
        heartbeats += duration*nodes[i].HR/60;
        totalDuration += duration;
      }
      value = (double?)(totalDuration == 0 ? 0 : heartbeats / totalDuration * 60);
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageHeartRateFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if(format == null) format = "{0:n0}";
      var d = ((double?)v);
      return d.HasValue ? string.Format(provider, format, d.Value) : "-";
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.HeartRate); }
    }

    private class HRNode
    {
      public double HR;
      public DateTime Time;
      public NodeType NodeType;
    }

    private enum NodeType
    {
      Begin,
      Intermediate,
      End
    }

  }

  public class AverageHeartRateFromStart : RouteFromStartProperty
  {
    public AverageHeartRateFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    public AverageHeartRateFromStart(Session session, RouteLocations locations)
      : base(session, locations)
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
      value = (new AverageHeartRate(Session, ParameterizedLocation.Start, Location) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if(format == null) format = "{0:n0}";
      var d = ((double?)v);
      return d.HasValue ? string.Format(provider, format, d.Value) : "-";
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)999); }
    }

    public override bool ContainsValue
    {
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.HeartRate); }
    }
  }

}
