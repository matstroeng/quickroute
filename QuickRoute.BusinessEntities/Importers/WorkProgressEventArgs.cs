using System;

namespace QuickRoute.BusinessEntities.Importers
{
  public class WorkProgressEventArgs : EventArgs
  {
    private double percentCompleted;

    public WorkProgressEventArgs(double percentCompleted)
    {
      this.percentCompleted = percentCompleted;
    }

    public double PercentCompleted
    {
      get { return percentCompleted; }
      set { percentCompleted = value; }
    }
  }

}
