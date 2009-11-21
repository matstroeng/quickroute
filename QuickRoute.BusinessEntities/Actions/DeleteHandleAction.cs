using System;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class DeleteHandleAction : IAction
  {
    private Handle handle;
    private Session session;

    public DeleteHandleAction(Handle handle, Session session)
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

    #region IAction Members

    public void Execute()
    {
      int handleIndex = Math.Max(0, session.IndexOfHandle(handle) - 1);
      session.RemoveHandle(handle);
      Handle h = (session.Handles.Length == 0 ? null : session.Handles[handleIndex]);
      session.UpdateHandle(h);
    }

    public void Undo()
    {
      session.AddHandle(handle);
      session.UpdateHandle(handle);
    }

    #endregion
  }
}
