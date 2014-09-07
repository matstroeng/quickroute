using System;
using System.Collections.Generic;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class AverageCadence : RouteSpanProperty
  {
    public AverageCadence(Session session, ParameterizedLocation start, ParameterizedLocation end, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, start, end, retrieveExternalProperty)
    {
    }

    public AverageCadence(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
    {
    }

    protected override void Calculate()    
    {
      var pl = new ParameterizedLocation(Start);
      var nodes = new List<CadenceNode>();
      while (pl <= End)
      {
        var cadence = Session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Cadence, pl);
        if(cadence.HasValue)
        {
          var nodeType = NodeType.Intermediate;
          if (Session.Route.IsFirstPLInSegment(pl)) nodeType = NodeType.Begin;
          if (Session.Route.IsLastPLInSegment(pl)) nodeType = NodeType.End;
          nodes.Add(new CadenceNode()
                      {
                        Cadence = cadence.Value,
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

      var revolutions = 0.0;
      var totalDuration = 0.0;
      for(var i=1; i<nodes.Count; i++)
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
        revolutions += duration*nodes[i].Cadence/60;
        totalDuration += duration;
      }
      value = (double?)(totalDuration == 0 ? 0 : revolutions / totalDuration * 60);
      AddToCache();
    }

    public override Type GetRouteFromStartPropertyType()
    {
      return typeof(AverageCadenceFromStart);
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
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.Cadence); }
    }

    private class CadenceNode
    {
      public double Cadence;
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

  public class AverageCadenceFromStart : RouteFromStartProperty
  {
    public AverageCadenceFromStart(Session session, ParameterizedLocation location, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, location, retrieveExternalProperty)
    {
    }

    public AverageCadenceFromStart(Session session, RouteLocations locations, RetrieveExternalPropertyDelegate retrieveExternalProperty)
      : base(session, locations, retrieveExternalProperty)
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
      value = (new AverageCadence(Session, ParameterizedLocation.Start, Location, RetrieveExternalProperty) { CacheManager = CacheManager }).Value;
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
      get { return Session.Route.ContainsWaypointAttribute(WaypointAttribute.Cadence); }
    }
  }

}
