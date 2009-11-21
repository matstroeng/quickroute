using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  [Serializable]
  public class SelectableRoutePropertyTypeCollection : Collection<SelectableRoutePropertyType>
  {
    public void AddRange(SelectableRoutePropertyTypeCollection items)
    {
      foreach (var item in items)
      {
        Add(item);
      }
    }

    public bool ContainsRoutePropertyType(Type t)
    {
      foreach (var item in this)
      {
        if(t == item.RoutePropertyType)
        {
          return true;
        }
      }
      return false;
    }
  }
}
