using System;
using System.Collections.Generic;
using System.Text;
using QuickRoute.Resources;

namespace QuickRoute.BusinessEntities.RouteProperties
{
  [Serializable]
  public class SelectableRoutePropertyType
  {
    private Type routePropertyType;

    public SelectableRoutePropertyType(Type routePropertyType, bool selected)
    {
      RoutePropertyType = routePropertyType;
      Selected = selected;
    }

    public Type RoutePropertyType
    {
      get
      {
        return routePropertyType;
      }
      set
      {
        if (!(value.IsSubclassOf(typeof (RouteProperty))))
          throw new Exception("The type must be a descendant of RouteProperty.");
        routePropertyType = value;
      }
    }

    public bool Selected { get; set; }

    public override string ToString()
    {
      return Strings.ResourceManager.GetString("RoutePropertyName_" + RoutePropertyType.Name) ?? RoutePropertyType.Name;
    }
  }
}
