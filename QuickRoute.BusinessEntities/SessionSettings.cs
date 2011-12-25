using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using QuickRoute.BusinessEntities.GlobalizedProperties;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class SessionSettings : GlobalizedObject 
  {
    // for backward compatibility only, do not use in new code
    private Interval speedSmoothingInterval = new Interval(0, 0);
    private Dictionary<WaypointAttribute, Interval> smoothingIntervals = CreateDefaultSmoothingIntervals();
    private Dictionary<WaypointAttribute, RouteLineSettings> routeLineSettingsCollection = CreateDefaultRouteLineSettingsCollection();
    private Dictionary<MarkerType, IMarkerDrawer> markerDrawers = CreateDefaultMarkerDrawers();
    private double circleTimeRadius = 45;

    public Dictionary<WaypointAttribute, Interval> SmoothingIntervals
    {
      get
      {
        if(smoothingIntervals == null)
        {
          smoothingIntervals = CreateDefaultSmoothingIntervals();
          smoothingIntervals[WaypointAttribute.Speed] = speedSmoothingInterval;
          smoothingIntervals[WaypointAttribute.Pace] = speedSmoothingInterval;
        }
        return smoothingIntervals;
      }
      set { smoothingIntervals = value; }
    }

    public Dictionary<WaypointAttribute, RouteLineSettings> RouteLineSettingsCollection
    {
      get { return routeLineSettingsCollection; }
      set { routeLineSettingsCollection = value; }
    }

    public Dictionary<MarkerType, IMarkerDrawer> MarkerDrawers
    {
      get { return markerDrawers; }
      set { markerDrawers = value; }
    }

    public double CircleTimeRadius
    {
      get { return circleTimeRadius; }
      set { circleTimeRadius = value; }
    }

    public static Dictionary<WaypointAttribute, Interval> CreateDefaultSmoothingIntervals()
    {
      var smoothingIntervals = new Dictionary<WaypointAttribute, Interval>();
      smoothingIntervals.Add(WaypointAttribute.Speed, new Interval(0, 0));
      smoothingIntervals.Add(WaypointAttribute.Pace, new Interval(0, 0));
      smoothingIntervals.Add(WaypointAttribute.HeartRate, new Interval(0, 0));
      smoothingIntervals.Add(WaypointAttribute.Altitude, new Interval(-10, 10));
      smoothingIntervals.Add(WaypointAttribute.DirectionDeviationToNextLap, new Interval(-2.5, 2.5));
      smoothingIntervals.Add(WaypointAttribute.MapReadingDuration, new Interval(0, 0));
      return smoothingIntervals;
    }

    public static Dictionary<WaypointAttribute, RouteLineSettings> CreateDefaultRouteLineSettingsCollection()
    {
      var coll = new Dictionary<WaypointAttribute, RouteLineSettings>();
      RouteLineSettings rls;

      // default pace line settings
      rls = new RouteLineSettings(
        new ColorRange(
          new Gradient(Color.FromArgb(128, Color.Green), 0.0,
                       Color.FromArgb(128, Color.Yellow), 0.5,
                       Color.FromArgb(128, Color.Red), 1.0),
          3.5 * 60, 10 * 60),
        3,
        Color.FromArgb(160, Color.Black),
        1.5,
        true,
        0);
      // TODO: localize
      rls.ColorRange.Gradient.Name = WaypointAttribute.Pace.ToString();
      rls.MonochromeColor = rls.ColorRange.Gradient.GetColor(1);
      rls.MonochromeWidth = rls.Width + 2 * rls.MaskWidth; 
      coll.Add(WaypointAttribute.Pace, rls);

      // default speed line settings
      rls = new RouteLineSettings(
        new ColorRange(
          new Gradient(Color.FromArgb(128, Color.Red), 0.0,
                       Color.FromArgb(128, Color.Yellow), 0.5,
                       Color.FromArgb(128, Color.Green), 1.0),
          5, 20),
        3,
        Color.FromArgb(160, Color.Blue),
        1.5,
        true,
        0);
      // TODO: localize
      rls.ColorRange.Gradient.Name = WaypointAttribute.Speed.ToString();
      rls.MonochromeColor = rls.ColorRange.Gradient.GetColor(1);
      rls.MonochromeWidth = rls.Width + 2 * rls.MaskWidth;
      coll.Add(WaypointAttribute.Speed, rls);

      // default heart rate line settings
      rls = new RouteLineSettings(
        new ColorRange(
          new Gradient(Color.FromArgb(128, Color.Green), 0.0,
                       Color.FromArgb(128, Color.Yellow), 0.5,
                       Color.FromArgb(128, Color.Red), 1.0),
          120, 190),
        3,
        Color.FromArgb(160, Color.DarkRed),
        1.5,
        true,
        0);
      // TODO: localize
      rls.ColorRange.Gradient.Name = WaypointAttribute.HeartRate.ToString();
      rls.MonochromeColor = rls.ColorRange.Gradient.GetColor(1);
      rls.MonochromeWidth = rls.Width + 2 * rls.MaskWidth;
      coll.Add(WaypointAttribute.HeartRate, rls);

      // default altitude line settings
      rls = new RouteLineSettings(
        new ColorRange(
          new Gradient(Color.FromArgb(128, Color.Green), 0.0,
                       Color.FromArgb(128, Color.Yellow), 1.0 / 3,
                       Color.FromArgb(128, Color.Red), 2.0 / 3,
                       Color.FromArgb(128, Color.Blue), 1.0),
          0, 500),
        3,
        Color.FromArgb(160, Color.Brown),
        1.5,
        true,
        0);
      // TODO: localize
      rls.ColorRange.Gradient.Name = WaypointAttribute.Altitude.ToString();
      rls.MonochromeColor = rls.ColorRange.Gradient.GetColor(1);
      rls.MonochromeWidth = rls.Width + 2 * rls.MaskWidth;
      coll.Add(WaypointAttribute.Altitude, rls);

      // default direction line settings
      rls = new RouteLineSettings(
        new ColorRange(
          new Gradient(Color.FromArgb(0, Color.White), 0.0,
                       Color.FromArgb(128, Color.Yellow), 0.5,
                       Color.FromArgb(128, Color.Red), 1.0),
          0, 45),
        3,
        Color.FromArgb(160, Color.Black),
        2,
        true,
        0);
      // TODO: localize
      rls.ColorRange.Gradient.Name = WaypointAttribute.DirectionDeviationToNextLap.ToString();
      rls.MonochromeColor = rls.ColorRange.Gradient.GetColor(1);
      rls.MonochromeWidth = rls.Width + 2 * rls.MaskWidth;
      coll.Add(WaypointAttribute.DirectionDeviationToNextLap, rls);

      // default map reading duration line settings
      rls = new RouteLineSettings(
        new ColorRange(
          new Gradient(Color.FromArgb(0, Color.White), 0.0,
                       Color.FromArgb(128, 255, 128, 128), 0.0001,
                       Color.FromArgb(128, 255, 0, 0), 0.5,
                       Color.FromArgb(128, 128, 0, 0), 1.0),
          0, 10),
        3,
        Color.FromArgb(160, Color.Black),
        2,
        true,
        0);
      // TODO: localize
      rls.ColorRange.Gradient.Name = WaypointAttribute.MapReadingDuration.ToString();
      rls.MonochromeColor = rls.ColorRange.Gradient.GetColor(1);
      rls.MonochromeWidth = rls.Width + 2 * rls.MaskWidth;
      coll.Add(WaypointAttribute.MapReadingDuration, rls);


      return coll; 
    }

    public static Dictionary<MarkerType, IMarkerDrawer> CreateDefaultMarkerDrawers()
    {
      Dictionary<MarkerType, IMarkerDrawer> markerDrawers = new Dictionary<MarkerType, IMarkerDrawer>();
      markerDrawers.Add(
        MarkerType.Start,
        new CircleDrawer(Color.FromArgb(192, 255, 0, 64), 12, 3)
      );
      markerDrawers.Add(
        MarkerType.Lap,
        new CircleDrawer(Color.FromArgb(192, 255, 0, 64), 12, 3)
      );
      markerDrawers.Add(
        MarkerType.Stop,
        new CircleDrawer(Color.FromArgb(192, 255, 0, 64), 12, 3)
      );
      markerDrawers.Add(
        MarkerType.MouseHover,
        new DiscAndCircleDrawer(Color.FromArgb(192, Color.Red), 4, Color.FromArgb(192,Color.Black), 6, 2)
      );
      markerDrawers.Add(
        MarkerType.Handle,
        new DiscDrawer(Color.FromArgb(192, Color.Blue), 6)
      );
      markerDrawers.Add(
        MarkerType.ActiveHandle,
        new DiscAndCircleDrawer(Color.FromArgb(192, Color.Blue), 6, Color.FromArgb(192, Color.Blue), 12, 3)
      );
      markerDrawers.Add(
        MarkerType.MovingActiveHandle,
        new CircleDrawer(Color.FromArgb(192, Color.Blue), 12, 3)
      );

      return markerDrawers;
    }

    public SessionSettings Copy()
    {
      MemoryStream ms = new MemoryStream();
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(ms, this);
      ms.Flush();
      ms.Seek(0, SeekOrigin.Begin);
      return bf.Deserialize(ms) as SessionSettings;
    }
  }
}
