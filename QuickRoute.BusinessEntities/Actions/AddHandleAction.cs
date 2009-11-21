using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class AddHandleAction : IAction 
  {
    private Handle handle;
    private Session session;

    public AddHandleAction(Handle handle, Session session)
    {
      this.handle = handle;
      this.session = session;
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

    public void Execute()
    {
      if (!session.ContainsHandle(handle))
      {
        session.AddHandle(handle);
      }
      session.UpdateHandle(handle);
    }

    public void Undo()
    {
      DeleteHandleAction delete = new DeleteHandleAction(handle, session);
      delete.Execute();
    }

  }
}
