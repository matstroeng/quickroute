using System;

namespace QuickRoute.BusinessEntities.ExternalLapDataSources
{
  public class Event
  {
    public string DatabaseId { get; set; }
    public string Name { get; set; }
    public string Organiser { get; set; }
    public DateTime StartDate { get; set; }
  }
}