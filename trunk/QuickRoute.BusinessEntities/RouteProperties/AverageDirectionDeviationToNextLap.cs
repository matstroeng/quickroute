using System;
using System.Collections.Generic;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageDirectionDeviationToNextLap : RouteSpanProperty
  {
    public AverageDirectionDeviationToNextLap(Session session, ParameterizedLocation start, ParameterizedLocation end)
      : base(session, start, end)
    {
    }

    public AverageDirectionDeviationToNextLap(Session session, RouteLocations locations)
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
      var pl = new ParameterizedLocation(Start);
      var nodes = new List<Node>();
      while (pl <= End)
      {
        var directionDeviationToNextLap = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.DirectionDeviationToNextLap, pl);
        var nodeType = NodeType.Intermediate;
        if (Session.Route.IsFirstPLInSegment(pl)) nodeType = NodeType.Begin;
        if (Session.Route.IsLastPLInSegment(pl)) nodeType = NodeType.End;
        nodes.Add(new Node()
        {
          DirectionDeviationToNextLap = directionDeviationToNextLap.Value,
          Time = Session.Route.GetTimeFromParameterizedLocation(pl),
          NodeType = nodeType
        });
        if (pl >= End) break;
        pl = Session.Route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        if (pl > End) pl = new ParameterizedLocation(End);
      }

      if (nodes.Count > 0)
      {
        nodes[0].NodeType = NodeType.Begin;
        nodes[nodes.Count - 1].NodeType = NodeType.End;
      }

      var sum = 0.0;
      var totalDuration = 0.0;
      for (var i = 0; i < nodes.Count; i++)
      {
        double duration = 0;
        switch (nodes[i].NodeType)
        {
          case NodeType.Begin:
            duration = (nodes[i + 1].Time - nodes[i].Time).TotalSeconds / 2;
            break;
          case NodeType.End:
            duration = (nodes[i].Time - nodes[i - 1].Time).TotalSeconds / 2;
            break;
          case NodeType.Intermediate:
            duration = (nodes[i].Time - nodes[i - 1].Time).TotalSeconds / 2 + (nodes[i + 1].Time - nodes[i].Time).TotalSeconds / 2;
            break;
        }
        sum += duration * nodes[i].DirectionDeviationToNextLap;
        totalDuration += duration;
      }
      value = (double?)(totalDuration == 0 ? 0 : sum / totalDuration);
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageDirectionDeviationToNextLapFromStart);
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      return string.Format(provider, format, Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }

    private class Node
    {
      public double DirectionDeviationToNextLap;
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

  public class AverageDirectionDeviationToNextLapFromStart : RouteFromStartProperty
  {
    public AverageDirectionDeviationToNextLapFromStart(Session session, ParameterizedLocation location)
      : base(session, location)
    {
    }

    public AverageDirectionDeviationToNextLapFromStart(Session session, RouteLocations locations)
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
      value = (new AverageDirectionDeviationToNextLap(Session, ParameterizedLocation.Start, Location) { CacheManager = CacheManager }).Value;
      AddToCache();
    }

    protected override string ValueToString(object v, string format, IFormatProvider provider)
    {
      if (format == null) format = "{0:n0}";
      return string.Format(provider, format, Convert.ToDouble(v));
    }

    public override string MaxWidthString
    {
      get { return ValueToString((double?)999); }
    }

    public override bool ContainsValue
    {
      get { return true; }
    }
  }
}
