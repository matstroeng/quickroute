using System;
using System.Collections.Generic;

namespace QuickRoute.BusinessEntities.ExternalLapDataSources
{
  public interface IExternalLapDataSource
  {
    IEnumerable<Event> GetEvents(DateTime startDate, DateTime endDate, string country);
    IEnumerable<Category> GetCategories(string eventId);
    IEnumerable<Runner> GetRunnersAndSplits(string eventId, string categoryIndex);
  }
}