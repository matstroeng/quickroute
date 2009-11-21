using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace QuickRoute.BusinessEntities
{
  public static class RouteAdjustmentManager
  {

    public static GeneralMatrix CreateInitialTransformationMatrix(Route route, Size mapSize, LongLat projectionOrigin)
    {
      // create initial adjustment: route should fit in the 75% inner rectangle of the map
      RectangleD routeRectangle = route.BoundingProjectedRectangle(projectionOrigin);
      RectangleD mapRectangle = new RectangleD(1.0 / 8.0 * mapSize.Width, 1.0 / 8.0 * mapSize.Height, 3.0 / 4.0 * mapSize.Width, 3.0 / 4.0 * mapSize.Height);
 
      // check width/height ratio for each of the rectangles, and adjust the map rectangle to have the same ratio as the route rectangle
      double routeRatio = routeRectangle.Width / routeRectangle.Height;
      double mapRatio = mapRectangle.Width / mapRectangle.Height;
      if (mapRatio < routeRatio)
      {
        // too narrow
        mapRectangle = new RectangleD(mapRectangle.Left, mapRectangle.Center.Y - mapRectangle.Width / routeRatio / 2.0, mapRectangle.Width, mapRectangle.Width / routeRatio);
      }
      else
      {
        // too wide
        mapRectangle = new RectangleD(mapRectangle.Center.X - mapRectangle.Height * routeRatio / 2.0, mapRectangle.Top, mapRectangle.Height * routeRatio, mapRectangle.Height);
      }

      GeneralMatrix t = LinearAlgebraUtil.CalculateTransformationMatrix(routeRectangle.LowerLeft, mapRectangle.UpperLeft, routeRectangle.UpperRight, mapRectangle.LowerRight, null);

      return t;
    }

  }
}
