using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.Actions
{
  public class TrimRouteAndAddLapsAction : IAction 
  {
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<Lap> Laps { get; set; }
    public Session Session { get; set; }
    private CutRouteAction startTimeCutAction;
    private CutRouteAction endTimeCutAction;
    private List<AddLapAction> addLapActions;
    private readonly TimeSpan offset;

    public TrimRouteAndAddLapsAction()
    {
      
    }

    public TrimRouteAndAddLapsAction(DateTime? startTime, DateTime? endTime, List<Lap> laps, Session session)
    {
      StartTime = startTime;
      EndTime = endTime;
      Laps = laps;
      Session = session;

      if (StartTime.HasValue && EndTime.HasValue)
      {
        if (EndTime.Value < Session.Route.FirstWaypoint.Time || StartTime.Value > Session.Route.LastWaypoint.Time)
        {
          offset = Session.Route.FirstWaypoint.Time - StartTime.Value;
        }
      }
    }

    public void Execute()
    {
      if (StartTime.HasValue)
      {
        startTimeCutAction = new CutRouteAction(Session, StartTime.Value + offset,
                                                CutType.Before);
        startTimeCutAction.Execute();
      }
      if (EndTime.HasValue)
      {
        endTimeCutAction = new CutRouteAction(Session, EndTime.Value + offset,
                                                CutType.After);
        endTimeCutAction.Execute();
      }
      addLapActions = new List<AddLapAction>();
      foreach(Lap lap in Laps)
      {
        lap.Time += offset;
        if (lap.Time >= Session.Route.FirstWaypoint.Time &&
            lap.Time <= Session.Route.LastWaypoint.Time)
        {
          var a = new AddLapAction(lap, Session);
          addLapActions.Add(a);
          a.Execute();
        }
      }
    }

    public void Undo()
    {
      foreach (AddLapAction a in addLapActions)
      {
        a.Undo();
      }
      if (endTimeCutAction != null) endTimeCutAction.Undo();
      if (startTimeCutAction != null) startTimeCutAction.Undo();
    }
  }
}
