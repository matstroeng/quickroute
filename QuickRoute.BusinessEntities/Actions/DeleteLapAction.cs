using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class DeleteLapAction : IAction
  {
    private Lap lap;
    private Session session;

    public DeleteLapAction(Lap lap, Session session)
    {
      this.lap = lap;
      this.session = session;
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
      session.Laps.Remove(lap);
      session.SetLapTimesToRoute();
    }

    public void Undo()
    {
      session.Laps.Add(lap);
      session.SetLapTimesToRoute();
    }

  }
}
