using System;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class CutRouteAction : IAction
  {
    private Session session;
    private ParameterizedLocation parameterizedLocation;
    private CutType cutType;
    private Route.CutRouteData cutRouteData;
    private LapCollection.CutLapsData cutLapsData;
    private HandleCollection.CutHandlesData cutHandlesData;
    private DateTime? time;

    /// <summary>
    /// Use this constructor when cutting should be based on time (to avoid rounding errors).
    /// </summary>
    /// <param name="session"></param>
    /// <param name="parameterizedLocation"></param>
    /// <param name="cutType"></param>
    public CutRouteAction(Session session, ParameterizedLocation parameterizedLocation, CutType cutType) 
      : this(session, parameterizedLocation, null, cutType)
    {
    }

    /// <summary>
    /// Use this constructor when cutting should be based on parameterized location.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="time"></param>
    /// <param name="cutType"></param>
    public CutRouteAction(Session session, DateTime time, CutType cutType)
      : this(session, session.Route.GetParameterizedLocationFromTime(time), time, cutType)
    {
    }

    private CutRouteAction(Session session, ParameterizedLocation parameterizedLocation, DateTime? time, CutType cutType)
    {
      this.session = session;
      this.parameterizedLocation = parameterizedLocation;
      this.cutType = cutType;
      this.time = time;
    }

    public Session Session
    {
      get { return session; }
      set { session = value; }
    }

    public ParameterizedLocation ParameterizedLocation
    {
      get { return parameterizedLocation; }
      set { parameterizedLocation = value; }
    }

    public CutType CutType
    {
      get { return cutType; }
      set { cutType = value; }
    }

    public DateTime? Time
    {
      get { return time; }
      set { time = value; }
    }

    public void Execute()
    {
      var cutTime = time.HasValue ? time.Value : session.Route.GetTimeFromParameterizedLocation(parameterizedLocation);
      cutRouteData = session.Route.Cut(parameterizedLocation, cutType);
      cutHandlesData = session.CutHandles(parameterizedLocation, cutType);
      cutLapsData = session.Laps.Cut(cutTime, cutType);
      session.SetLapTimesToRoute();
      session.CreateAdjustedRoute();
    }

    public void Undo()
    {
      session.Route.UnCut(cutRouteData);
      session.UnCutHandles(cutHandlesData);
      session.Laps.UnCut(cutLapsData);
      session.SetLapTimesToRoute();
      session.CreateAdjustedRoute();
    }

  }
}
