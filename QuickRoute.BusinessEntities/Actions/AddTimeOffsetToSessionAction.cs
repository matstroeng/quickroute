using System;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Actions
{
  public class AddTimeOffsetToSessionAction : IAction 
  {
    private Session session;
    private TimeSpan timeOffset;

    public AddTimeOffsetToSessionAction(Session session, TimeSpan timeOffset)
    {
      this.session = session;
      this.timeOffset = timeOffset;
    }

    public TimeSpan TimeOffset
    {
      get { return timeOffset; }
      set { timeOffset = value; }
    }

    public Session Session
    {
      get { return session; }
      set { session = value; }
    }

    public void Execute()
    {
      session.AddTimeOffset(timeOffset);
    }

    public void Undo()
    {
      session.AddTimeOffset(-timeOffset);
    }

  }
}
