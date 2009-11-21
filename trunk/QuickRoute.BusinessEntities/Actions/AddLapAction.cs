using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class AddLapAction : IAction
  {
    private Lap lap;
    private Session session;

    public AddLapAction(Lap lap, Session session)
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
      session.Laps.Add(lap);
      session.SetLapTimesToRoute();
    }

    public void Undo()
    {
      session.Laps.Remove(lap);
      session.SetLapTimesToRoute();
    }

  }
}
