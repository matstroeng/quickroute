using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  public class RoutePropertyCacheManager
  {
    private readonly Dictionary<Type, Dictionary<RouteLocations, RouteProperty>> cache = new Dictionary<Type, Dictionary<RouteLocations, RouteProperty>>();
    private readonly Dictionary<Type, List<RouteProperty>> lastAddedCache = new Dictionary<Type, List<RouteProperty>>();
    private const int itemsInLastAddedCache = 4;

    public void Add(RouteProperty routeProperty)
    {
      var type = routeProperty.GetType();
      if (!cache.ContainsKey(type))
      {
        cache[type] = new Dictionary<RouteLocations, RouteProperty>();
        lastAddedCache[type] = new List<RouteProperty>();
      }
      cache[type][routeProperty.Locations] = routeProperty;
      lastAddedCache[type].Add(routeProperty);
      if(lastAddedCache[type].Count > itemsInLastAddedCache) lastAddedCache[type].RemoveAt(0);
    }

    public RouteProperty Get(Type routePropertyType, RouteLocations locations)
    {
      if (!cache.ContainsKey(routePropertyType)) return null;
      var cacheSection = cache[routePropertyType];
      if (!cacheSection.ContainsKey(locations)) return null;
      var item = cacheSection[locations];
      return item;
    }

    public RouteProperty GetClosest(Type routePropertyType, RouteLocations locations, ParameterizedLocation.Direction direction)
    {
      if (!cache.ContainsKey(routePropertyType)) return null;
      var cacheSection = cache[routePropertyType];
      RouteProperty closestRouteProperty = null;
      ParameterizedLocation closestPL = null;
      if (locations.IsSpan)
      {
        foreach (var item in cacheSection)
        {
          if (item.Key.Start == locations.Start // need to have same start
             &&
             (closestPL == null ||
              (direction == ParameterizedLocation.Direction.Forward && item.Key.End >= locations.End && item.Key.End < closestPL) ||
              (direction == ParameterizedLocation.Direction.Backward && item.Key.End <= locations.End && item.Key.End > closestPL)
             ))
          {
            closestPL = item.Key.End;
            closestRouteProperty = item.Value;
          }
        }
      }
      else
      {
        foreach (var item in cacheSection)
        {
          if (closestPL == null ||
              (direction == ParameterizedLocation.Direction.Forward && item.Key.Location >= locations.Location && item.Key.Location < closestPL) ||
              (direction == ParameterizedLocation.Direction.Backward && item.Key.Location <= locations.Location && item.Key.Location > closestPL)
             )
          {
            closestPL = item.Key.End;
            closestRouteProperty = item.Value;
          }
        }
      }
      return closestRouteProperty;
    }

    public RouteProperty GetLastAdded(Type routePropertyType, RouteLocations desiredLocations)
    {
      if (!lastAddedCache.ContainsKey(routePropertyType)) return null;
      var items = lastAddedCache[routePropertyType];
      if (desiredLocations.IsSpan)
      {
        for(int i=items.Count-1; i>=0; i--)
        {
          if (items[i].Locations.Start == desiredLocations.Start) return items[i];
        }
        return null;
      }
      else
      {
        return items[items.Count-1];
      }
    }
  }
}