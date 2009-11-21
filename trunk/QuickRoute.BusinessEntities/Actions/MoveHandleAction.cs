using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class MoveHandleAction : IAction 
  {
    private Handle handle;
    private Session session;
    private PointD oldLocation;
    private PointD newLocation;

    public MoveHandleAction(Handle handle, Session session, PointD oldLocation)
    {
      this.handle = handle;
      this.session = session;
      this.oldLocation = oldLocation;
      this.newLocation = handle.Location; 
    }

    public Handle Handle
    {
      get { return handle; }
      set { handle = value; }
    }

    public Session Session
    {
      get { return session; }
      set { session = value; }
    }

    public PointD OldLocation
    {
      get { return oldLocation; }
      set { oldLocation = value; }
    }

    public PointD NewLocation
    {
      get { return newLocation; }
      set { newLocation = value; }
    }

    public void Execute()
    {
      handle.Location = newLocation;
      session.UpdateHandle(handle);
    }

    public void Undo()
    {
      handle.Location = oldLocation;
      session.UpdateHandle(handle);
    }

  }
}
