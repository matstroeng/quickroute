using System;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities.ExternalLapDataSources
{
  public class Runner
  {
    public string Name { get; set; }
    public string Club { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? FinishTime { get; set; }
    public IEnumerable<DateTime> Splits { get; set; }
  }
}
