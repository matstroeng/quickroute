using System;
using System.Collections.Generic;
using QuickRoute.BusinessEntities.CzechSplitsWebService;

namespace QuickRoute.BusinessEntities.ExternalLapDataSources.CzechSplits
{
  public class CzechSplitsLapDataSource : IExternalLapDataSource
  {
    private ResultsDBService service = new ResultsDBService();

    public IEnumerable<Event> GetEvents(DateTime startDate, DateTime endDate, string country)
    {
      var response = service.GetEvents(new GetEvents() { startDate = startDate, endDate = endDate, country = country });
      var translatedEvents = new List<Event>();
      foreach (var e in response.GetEventsResult ?? new CzechSplitsWebService.Event[0])
      {
        translatedEvents.Add(TranslateEvent(e));
      }
      return translatedEvents;
    }

    public IEnumerable<Category> GetCategories(string eventId)
    {
      var response = service.GetCategories(new GetCategories() { databaseId = eventId });
      var translatedCategories = new List<Category>();
      foreach (var c in response.GetCategoriesResult ?? new CzechSplitsWebService.Category[0])
      {
        translatedCategories.Add(TranslateCategory(c));
      }
      return translatedCategories;
    }

    public IEnumerable<Runner> GetRunnersAndSplits(string eventId, int categoryIndex)
    {
      var response = service.GetRunnersAndSplits(new GetRunnersAndSplits() { databaseId = eventId, categoryIndex = categoryIndex});
      var translatedRunners = new List<Runner>();
      foreach (var r in response.GetRunnersAndSplitsResult ??  new CzechSplitsWebService.Runner[0])
      {
        translatedRunners.Add(TranslateRunner(r));
      }
      return translatedRunners;
    }

    private static Event TranslateEvent(CzechSplitsWebService.Event e)
    {
      return new Event()
               {
                 DatabaseId = e.DatabaseId.ToString(),
                 Name = e.Name,
                 Organiser = e.Organiser,
                 StartDate = e.StartDate
               };
    }

    private static Category TranslateCategory(CzechSplitsWebService.Category c)
    {
      return new Category()
               {
                 Name = c.Name
               };
    }

    private static Runner TranslateRunner(CzechSplitsWebService.Runner r)
    {
      var splits = new List<DateTime>();
      foreach(var s in r.Splits)
      {
        splits.Add(s.dateTime1);
      }
      return new Runner()
               {
                 Name = r.Name,
                 Club = r.Club,
                 Splits = splits,
                 StartTime = r.StartTime,
                 FinishTime = r.FinishTime,
               };
    }
  }
}