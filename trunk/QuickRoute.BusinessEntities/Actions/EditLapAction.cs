using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class EditLapAction : IAction
  {
    private Lap lap;
    private Session session;
    private Lap oldLap;
    private Lap newLap;

    public EditLapAction(Lap lap, Lap oldLap, Session session)
    {
      this.lap = lap;
      this.session = session;
      this.oldLap = oldLap;
      this.newLap = (Lap)lap.Clone();
    }

    public Lap Lap
    {
      get { return lap; }
      set { lap = value; }
    }

    public Session Session
    {
      get { return session; }
      set { session = value; }
    }

    public void Execute()
    {
      lap.Time = newLap.Time;
      lap.LapType = newLap.LapType;
      session.SetLapTimesToRoute();
    }

    public void Undo()
    {
      lap.Time = oldLap.Time;
      lap.LapType = oldLap.LapType;
      session.SetLapTimesToRoute();
    }

  }
}
