using System;
using System.Collections.Generic;
using QuickRoute.BusinessEntities;

namespace QuickRoute.BusinessEntities.Importers
{
  public static class RouteImporterUtil
  {
    public static LapCollection CreateLapsFromElapsedTimes(DateTime startTime, List<double> elapsedTimes, List<RouteSegment> routeSegments)
    {
      LapCollection laps = new LapCollection();
      if (routeSegments.Count == 0) return laps;

      // justify lap times with regard to paused times during the race
      int currentRouteSegmentIndex = 0;
      for (int i = 0; i < elapsedTimes.Count; i++)
      {
        while (startTime.AddSeconds(elapsedTimes[i]) > routeSegments[currentRouteSegmentIndex].LastWaypoint.Time && currentRouteSegmentIndex < routeSegments.Count - 1)
        {
          double stoppedTime = (routeSegments[currentRouteSegmentIndex + 1].FirstWaypoint.Time.Ticks - routeSegments[currentRouteSegmentIndex].LastWaypoint.Time.Ticks) / (double)TimeSpan.TicksPerSecond;
          for (int j = i; j < elapsedTimes.Count; j++)
          {
            elapsedTimes[j] += stoppedTime;
          }
          currentRouteSegmentIndex++;
        }
      }

      // add each lap
      foreach (double et in elapsedTimes)
      {
        if (et > 0) laps.Add(new Lap(startTime.AddSeconds(et), LapType.Lap));
      }

      // add start and end of each route segment as a lap 
      foreach (RouteSegment rs in routeSegments)
      {
        laps.Add(new Lap(rs.FirstWaypoint.Time, LapType.Start));
        laps.Add(new Lap(rs.LastWaypoint.Time, LapType.Stop));
      }

      return laps;
    }
  }
}
