using System;
using System.Collections.Generic;
using QuickRoute.BusinessEntities.WinSplitsWebService;

namespace QuickRoute.BusinessEntities.ExternalLapDataSources.WinSplits
{
  public class WinSplitsLapDataSource : IExternalLapDataSource
  {
    private readonly WebServices service = new WebServices();

    public IEnumerable<Event> GetEvents(DateTime startDate, DateTime endDate, string country)
    {
      var events = service.GetEvents(startDate, endDate, country);
      var translatedEvents = new List<Event>();
      foreach(var e in events)
      {
        translatedEvents.Add(TranslateEvent(e));
      }
      return translatedEvents;
    }

    public IEnumerable<Category> GetCategories(string eventId)
    {
      var categories = service.GetCategories(int.Parse(eventId));
      var translatedCategories = new List<Category>();
      foreach (var c in categories)
      {
        translatedCategories.Add(TranslateCategory(c));
      }
      return translatedCategories;
    }

    public IEnumerable<Runner> GetRunnersAndSplits(string eventId, int categoryIndex)
    {
      var runners = service.GetRunnersAndSplits(int.Parse(eventId), (short)categoryIndex);
      var translatedRunners = new List<Runner>();
      foreach (var r in runners)
      {
        translatedRunners.Add(TranslateRunner(r));
      }
      return translatedRunners;
    }

    private static Event TranslateEvent(WinSplitsWebService.Event e)
    {
      return new Event()
               {
                 DatabaseId = e.DatabaseId.ToString(),
                 Name = e.Name,
                 Organiser = e.Organiser,
                 StartDate = e.StartDate
               };
    }

    private static Category TranslateCategory(WinSplitsWebService.Category c)
    {
      return new Category()
               {
                 Name = c.Name
               };
    }

    private static Runner TranslateRunner(WinSplitsWebService.Runner r)
    {
      return new Runner()
               {
                 Name = r.Name,
                 Club = r.Club,
                 Splits = new List<DateTime>(r.Splits),
                 StartTime = r.StartTime,
                 FinishTime = r.FinishTime,
               };
    }
  }
}