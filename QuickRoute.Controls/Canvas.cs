using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using QuickRoute.BusinessEntities;
using QuickRoute.BusinessEntities.Actions;
using QuickRoute.BusinessEntities.Numeric;
using QuickRoute.Controls.Forms;
using QuickRoute.Resources;

namespace QuickRoute.Controls
{
  public partial class Canvas : UserControl
  {
    #region Declarations

    private const double LAP_CLOSENESS_TOLERANCE = 16.0;
    private const double ROUTE_CLOSENESS_TOLERANCE = 32.0;
    private readonly MouseInfo mouseInfo = new MouseInfo();
    private Handle activeHandle;
    private PointD activeHandleOffset;
    private PointD activeHandleOldLocation;
    private Lap activeLap;
    private WaypointAttribute colorCodingAttribute = WaypointAttribute.Pace;
    private WaypointAttribute? secondaryColorCodingAttribute;
    private MouseTool currentMouseTool = MouseTool.Pointer;
    private Session currentSession;
    private Document document;
    private bool initialized;
    private bool isNewHandle;
    private double maxZoom = 2.0;
    private double minZoom = 0.25;
    private Point mouseDownScrollbarValues;
    private Handle mouseHoverHandle;
    private bool movingActiveHandleNow;
    private bool movingActiveLapNow;
    private Lap oldActiveLap;
    private Point origo;
    private bool preventRedraw;
    private bool preventScrollEvents;
    private MouseTool previousMouseTool;
    private Document.RouteDrawingMode routeDrawingMode = Document.RouteDrawingMode.Extended;
    private Graphics routeGraphics;
    private Bitmap routeImage;
    private RouteLineSettings routeLineSettings;
    private Graphics routeWithHandlesGraphics;
    private Bitmap routeWithHandlesImage;
    private SessionCollection selectedSessions = new SessionCollection();
    private double zoom = 1;
    private Bitmap zoomedMapImage;
    private bool movingMapWithMouseNow;

    #region Events

    public event EventHandler<EventArgs> DocumentChanged;
    public event EventHandler<EventArgs> BeforeZoomChanged;
    public event EventHandler<EventArgs> AfterZoomChanged;
    public event EventHandler<RouteMouseHoverEventArgs> RouteMouseHover;
    public event EventHandler<EventArgs> CurrentSessionChanged;

    /// <summary>
    /// Sends info about an action that was triggered from this class.
    /// </summary>
    public event EventHandler<ActionEventArgs> ActionPerformed;

    #endregion

    #endregion

    #region Constructor and destructor

    ~Canvas()
    {
      if (zoomedMapImage != null) zoomedMapImage.Dispose();
      if (routeImage != null) routeImage.Dispose();
      if (routeGraphics != null) routeGraphics.Dispose();
      if (routeWithHandlesImage != null) routeWithHandlesImage.Dispose();
      if (routeWithHandlesGraphics != null) routeWithHandlesGraphics.Dispose();
    }

    #endregion

    #region Public properties

    public Document Document
    {
      get { return document; }
      set
      {
        if (value == null || !value.Equals(document))
        {
          document = value;
          if (document != null)
          {
            CreateBackBuffer();
            Init();
            document.MapChanged += Document_MapChanged;
          }
          if (DocumentChanged != null) DocumentChanged(this, new EventArgs());
        }
      }
    }

    public Session CurrentSession
    {
      get { return currentSession; }
      set
      {
        if (document == null || value == currentSession) return;
        if (!document.Sessions.Contains(value))
        {
          throw new Exception("The session does not exist in the session collection.");
        }
        currentSession = value;
        if (CurrentSessionChanged != null) CurrentSessionChanged(this, new EventArgs());
      }
    }

    public double Zoom
    {
      get { return zoom; }
      set
      {
        value = Math.Min(Math.Max(value, minZoom), maxZoom);
        if (value == zoom) return;
        if (BeforeZoomChanged != null) BeforeZoomChanged(this, new EventArgs());
        double oldZoom = zoom;
        zoom = value;
        if (initialized)
        {
          // center point in unzoomed coordinates
          var centerPoint = new PointD(
            (scrX.Value + Math.Min(zoomedMapImage.Width, canvasPanel.Width) / 2.0) / oldZoom,
            (scrY.Value + Math.Min(zoomedMapImage.Height, canvasPanel.Height) / 2.0) / oldZoom);
          CreateBackBuffer();
          //CreateAdjustedRoutes();
          preventScrollEvents = true;
          UpdateScrollbarProperties();
          CenterMap(centerPoint);
          preventScrollEvents = false;
          canvasPanel.CreateGraphics().Clear(canvasPanel.BackColor);
          DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
        }
        if (AfterZoomChanged != null) AfterZoomChanged(this, new EventArgs());
      }
    }

    public double MinZoom
    {
      get { return minZoom; }
      set { minZoom = value; }
    }

    public double MaxZoom
    {
      get { return maxZoom; }
      set { maxZoom = value; }
    }

    public RouteLineSettings RouteLineSettings
    {
      get { return routeLineSettings; }
      set { routeLineSettings = value; }
    }

    public MouseTool CurrentMouseTool
    {
      get { return currentMouseTool; }
      set
      {
        Document.RouteDrawingMode previousRouteDrawingMode = routeDrawingMode;
        currentMouseTool = value;
        canvasPanel.Cursor = GetCursor(currentMouseTool);
        routeDrawingMode = (currentMouseTool == MouseTool.AdjustRoute ? Document.RouteDrawingMode.Simple : Document.RouteDrawingMode.Extended);
        MapDrawingFlags flags = MapDrawingFlags.Markers;
        if (routeDrawingMode != previousRouteDrawingMode)
        {
          flags |= MapDrawingFlags.Route;
        }
        DrawMap(flags);
      }
    }

    public WaypointAttribute ColorCodingAttribute
    {
      get { return colorCodingAttribute; }
      set
      {
        colorCodingAttribute = value;
        DrawMap(MapDrawingFlags.Markers | MapDrawingFlags.Route);
      }
    }

    public WaypointAttribute? SecondaryColorCodingAttribute
    {
      get { return secondaryColorCodingAttribute; }
      set
      {
        secondaryColorCodingAttribute = value;
        DrawMap(MapDrawingFlags.Markers | MapDrawingFlags.Route);
      }
    }

    public bool PreventRedraw
    {
      get { return preventRedraw; }
      set { preventRedraw = value; }
    }

    public Document.RouteDrawingMode RouteDrawingMode
    {
      get { return routeDrawingMode; }
      set { routeDrawingMode = value; }
    }

    public SessionCollection SelectedSessions
    {
      get { return selectedSessions; }
      set { selectedSessions = value; }
    }

    public SessionDrawingMode SessionsToDraw { get; set; }

    #endregion

    #region Public methods

    public Canvas()
    {
      InitializeComponent();
    }

    public void Init()
    {
      initialized = true;
      preventScrollEvents = true;
      scrX.Value = scrX.Minimum;
      scrY.Value = scrY.Minimum;
      preventScrollEvents = false;
      document.ProjectionOrigin = document.Sessions[0].ProjectionOrigin;
      CreateAdjustedRoutes();
      DrawMap(MapDrawingFlags.Markers | MapDrawingFlags.Route);
      ResizeCanvas();
    }

    public void DrawMap(MapDrawingFlags flags)
    {
      if (preventRedraw) return;
      if (document != null && document.Map != null)
      {
        if ((flags & MapDrawingFlags.Route) == MapDrawingFlags.Route)
        {
          var mode = routeDrawingMode;
          if (selectedSessions.Count > 1) mode = Document.RouteDrawingMode.Simple;
          DrawRoutes(routeGraphics, zoomedMapImage, mode, null);
        }

        if ((flags & MapDrawingFlags.Markers) == MapDrawingFlags.Markers) DrawMarkers();

        // copy map + route to canvas
        UpdateBackBuffer();
      }
    }

    /// <summary>
    /// Makes the route line for the specified lap to appear in a different alpha adjustment than the rest of the course
    /// </summary>
    /// <param name="session"></param>
    /// <param name="lap"></param>
    /// <param name="lapAlphaAdjustment"></param>
    /// <param name="courseAlphaAdjustment"></param>
    /// <param name="defaultAlphaAdjustment"></param>
    public void AlphaAdjustLap(Session session, Lap lap, double lapAlphaAdjustment, double courseAlphaAdjustment,
                               double defaultAlphaAdjustment)
    {
      Route r = session.Route;
      bool lapExists = session.Laps.Contains(lap) && lap.LapType != LapType.Start;
      int lapIndex = 0;

      if (lapExists)
      {
        // get the index of the lap
        for (int i = 0; i < session.Laps.Count; i++)
        {
          if (session.Laps[i] == lap)
          {
            lapIndex = i;
            break;
          }
        }
        Lap lastLap = session.Laps[lapIndex - 1];

        var aac = new List<AlphaAdjustmentChange>();
        if (r.FirstWaypoint.Time < lastLap.Time)
          aac.Add(new AlphaAdjustmentChange(r.GetParameterizedLocationFromTime(r.FirstWaypoint.Time),
                                            courseAlphaAdjustment));
        aac.Add(new AlphaAdjustmentChange(r.GetParameterizedLocationFromTime(lastLap.Time), lapAlphaAdjustment));
        aac.Add(new AlphaAdjustmentChange(r.GetParameterizedLocationFromTime(lap.Time), courseAlphaAdjustment));
        session.AlphaAdjustmentChanges = aac;
        DrawMap(MapDrawingFlags.Markers | MapDrawingFlags.Route);
      }
      else
      {
        var aac = new List<AlphaAdjustmentChange>();
        aac.Add(new AlphaAdjustmentChange(r.GetParameterizedLocationFromTime(r.FirstWaypoint.Time),
                                          defaultAlphaAdjustment));
        session.AlphaAdjustmentChanges = aac;
        DrawMap(MapDrawingFlags.Markers | MapDrawingFlags.Route);
      }
    }

    #endregion

    #region Private properties

    private Size CanvasSize
    {
      get
      {
        return new Size(
          canvasPanel.Width - (scrY.Visible ? scrY.Width : 0),
          canvasPanel.Height - (scrXPanel.Visible ? scrXPanel.Height : 0));
      }
    }

    #endregion

    #region Private methods

    private void UpdateBackBuffer()
    {
      UpdateBackBuffer(canvasPanel.Bounds);
    }

    private void UpdateBackBuffer(Rectangle clipRectangle)
    {
      if (routeWithHandlesImage == null) return;
      origo = new Point(
        Math.Max(0, (canvasPanel.Width - routeWithHandlesImage.Width) / 2),
        Math.Max(0, (canvasPanel.Height - routeWithHandlesImage.Height) / 2));
      var size = new Size(
        Math.Min(canvasPanel.Width, routeWithHandlesImage.Width),
        Math.Min(canvasPanel.Height, routeWithHandlesImage.Height));
      var clipSize = new Size(
        Math.Min(size.Width, clipRectangle.Width),
        Math.Min(size.Height, clipRectangle.Height));

      var destOrigo = new Point(origo.X + clipRectangle.Left, origo.Y + clipRectangle.Top);

      var destRect = new Rectangle(destOrigo, clipSize);
      var srcRect = new Rectangle(new Point(scrX.Value + clipRectangle.Left, scrY.Value + clipRectangle.Top), clipSize);
      Graphics g = canvasPanel.CreateGraphics();

      g.DrawImage(routeWithHandlesImage, destRect, srcRect, GraphicsUnit.Pixel);
      g.DrawRectangle(Pens.Black, new Rectangle(origo.X - 1, origo.Y - 1, size.Width + 1, size.Height + 1));
    }

    private void MoveMapWithMouse()
    {
      if (mouseInfo.MouseDownArgs != null)
      {
        int x = mouseDownScrollbarValues.X + (mouseInfo.MouseDownArgs.Location.X - mouseInfo.MouseMoveArgs.Location.X) -
                origo.X;
        int y = mouseDownScrollbarValues.Y + (mouseInfo.MouseDownArgs.Location.Y - mouseInfo.MouseMoveArgs.Location.Y) -
                origo.Y;
        scrX.Value = Math.Max(scrX.Minimum, Math.Min(Math.Max(x, scrX.Minimum), scrX.Maximum - scrX.LargeChange));
        scrY.Value = Math.Max(scrX.Minimum, Math.Min(Math.Max(y, scrY.Minimum), scrY.Maximum - scrY.LargeChange));
        UpdateBackBuffer();
      }
    }

    private void BeginMoveMapWithMouse()
    {
      movingMapWithMouseNow = true;
      previousMouseTool = currentMouseTool;
      currentMouseTool = MouseTool.MoveMap;
      canvasPanel.Cursor = GetCursor(currentMouseTool);
      preventScrollEvents = true;
      mouseDownScrollbarValues = new Point(scrX.Value, scrY.Value);
    }

    private void EndMoveMapWithMouse()
    {
      currentMouseTool = previousMouseTool;
      canvasPanel.Cursor = GetCursor(currentMouseTool);
      movingMapWithMouseNow = false;
    }

    private void CheckMouseHovering()
    {
      // location of mouse pointer in original map coordinates
      PointD mouseLocation =
        new PointD((mouseInfo.MouseMoveArgs.X + scrX.Value - origo.X),
                   (mouseInfo.MouseMoveArgs.Y + scrY.Value - origo.Y)) / zoom;
      double distanceToRoute;
      ParameterizedLocation closestPL = CurrentSession.AdjustedRoute.GetClosestParameterizedLocation(mouseLocation,
                                                                                                     out distanceToRoute);
      if (!movingActiveHandleNow && !movingActiveLapNow)
      {
        if (closestPL == null)
        {
          // not close to route
          if(mouseHoverHandle != null)
          {
            ResetMouseHoverHandle();
            DrawMap(MapDrawingFlags.Markers);
          }
          return; 
        }

        // check if close to existing handle
        double closestHandleDistance = 0;
        double closestLapDistance = 0;
        Handle closestHandle = null;
        Lap closestLap = null;

        // Shift key prevents check if close to handle or lap
        bool shiftDown = (ModifierKeys & Keys.Shift) == Keys.Shift;

        PointD routeLocation = CurrentSession.AdjustedRoute.GetLocationFromParameterizedLocation(closestPL);

        if (!shiftDown && currentMouseTool == MouseTool.AdjustRoute)
          closestHandle = GetClosestHandle(routeLocation, out closestHandleDistance);
        if (!shiftDown) closestLap = GetClosestLap(routeLocation, out closestLapDistance);

        if (distanceToRoute >= ROUTE_CLOSENESS_TOLERANCE)
        {
          // not close to route
          ResetMouseHoverHandle();
          if (RouteMouseHover != null) RouteMouseHover(this, new RouteMouseHoverEventArgs(closestPL, false));
        }
        else if (closestHandle != null && closestHandleDistance < ROUTE_CLOSENESS_TOLERANCE &&
                 !(closestLap != null && closestLapDistance * 1.00001 < closestHandleDistance))
        {
          // close to existing handle, use it as active handle
          if (activeHandle != null)
          {
            // reset previous handle drawer
            activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.Handle];
          }
          activeHandle = closestHandle;
          activeHandleOldLocation = activeHandle.Location;
          activeHandleOffset = mouseLocation - activeHandle.Location;
          activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.ActiveHandle];
          mouseHoverHandle = null;
          activeLap = null;
          if (RouteMouseHover != null)
            RouteMouseHover(this, new RouteMouseHoverEventArgs(activeHandle.ParameterizedLocation, true));
        }
        else if (closestLap != null && closestLapDistance < LAP_CLOSENESS_TOLERANCE)
        {
          // close to existing lap, create a mouse hover over this lap
          ParameterizedLocation pl = CurrentSession.Route.GetParameterizedLocationFromTime(closestLap.Time);
          PointD lapLocation = CurrentSession.AdjustedRoute.GetLocationFromParameterizedLocation(pl);
          GeneralMatrix transformationMatrix = CurrentSession.GetTransformationMatrixFromParameterizedLocation(pl);
          mouseHoverHandle = new Handle(pl, closestLap.Time, lapLocation, transformationMatrix,
                                        CurrentSession.Settings.MarkerDrawers[MarkerType.MouseHover]);
          if (RouteMouseHover != null) RouteMouseHover(this, new RouteMouseHoverEventArgs(pl, true));
          activeLap = closestLap;
        }
        else
        {
          // mouse hover over route
          if (activeHandle != null)
          {
            // reset handle drawer to standard handle drawer
            activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.Handle];
          }
          activeHandle = null;
          activeLap = null;
          GeneralMatrix transformationMatrix =
            CurrentSession.GetTransformationMatrixFromParameterizedLocation(closestPL);
          mouseHoverHandle = new Handle(closestPL, routeLocation, transformationMatrix,
                                        CurrentSession.Settings.MarkerDrawers[MarkerType.MouseHover]);
          if (RouteMouseHover != null) RouteMouseHover(this, new RouteMouseHoverEventArgs(closestPL, true));
        }
        DrawMap(MapDrawingFlags.Markers);
      }
      else if (movingActiveHandleNow)
      {
        activeHandle.Location = mouseLocation - activeHandleOffset;
        CurrentSession.UpdateHandle(activeHandle);
        DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
      }
      else if (movingActiveLapNow)
      {
        if (closestPL == null) return; // not close to the route
        activeLap.Time = CurrentSession.Route.GetTimeFromParameterizedLocation(closestPL);
        DrawMap(MapDrawingFlags.Markers);
      }
    }

    private void ResetMouseHoverHandle()
    {
      mouseHoverHandle = null;
      activeLap = null;
      if (activeHandle != null)
      {
        // reset handle drawer to standard handle drawer
        activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.Handle];
      }
      activeHandle = null;
    }

    public void DrawActiveHandle(ParameterizedLocation pl)
    {
      // mouse hover over route
      if (activeHandle != null)
      {
        // reset handle drawer to standard handle drawer
        activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.Handle];
      }
      activeHandle = null;
      activeLap = null;
      PointD routeLocation = CurrentSession.AdjustedRoute.GetLocationFromParameterizedLocation(pl);
      if (pl != null)
      {
        GeneralMatrix transformationMatrix = CurrentSession.GetTransformationMatrixFromParameterizedLocation(pl);
        mouseHoverHandle = new Handle(pl, routeLocation, transformationMatrix,
                                      CurrentSession.Settings.MarkerDrawers[MarkerType.MouseHover]);
      }
      else
      {
        mouseHoverHandle = null;
      }
      DrawMap(MapDrawingFlags.Markers);
    }


    /// <summary>
    /// Returns the closest handle to the given point.
    /// </summary>
    /// <param name="location">Location in original map coordinates</param>
    /// <param name="distance">Distance in original map distance units</param>
    /// <returns></returns>
    private Handle GetClosestHandle(PointD location, out double distance)
    {
      Handle closestHandle = null;
      double closestDistance = 0.0;
      foreach (Handle h in CurrentSession.Handles)
      {
        double tmpDistance = LinearAlgebraUtil.DistancePointToPoint(location, h.Location);
        if (closestHandle == null || tmpDistance < closestDistance)
        {
          closestHandle = h;
          closestDistance = tmpDistance;
        }
      }
      distance = closestDistance;
      return closestHandle;
    }

    /// <summary>
    /// Returns the closest lap to the given point.
    /// </summary>
    /// <param name="location">Location in original map coordinates</param>
    /// <param name="distance">Distance in original map distance units</param>
    /// <returns></returns>
    private Lap GetClosestLap(PointD location, out double distance)
    {
      Lap closestLap = null;
      double closestDistance = 0.0;
      foreach (Lap lap in CurrentSession.Laps)
      {
        ParameterizedLocation pl = CurrentSession.Route.GetParameterizedLocationFromTime(lap.Time);
        PointD lapLocation = CurrentSession.AdjustedRoute.GetLocationFromParameterizedLocation(pl);
        double tmpDistance = LinearAlgebraUtil.DistancePointToPoint(location, lapLocation);
        if (closestLap == null || tmpDistance < closestDistance)
        {
          closestLap = lap;
          closestDistance = tmpDistance;
        }
      }
      distance = closestDistance;
      return closestLap;
    }

    private void CreateAdjustedRoutes()
    {
      foreach (Session s in document.Sessions)
      {
        s.CreateAdjustedRoute();
      }
    }

    private void DrawRoutes(Graphics graphics, Image mapImage, Document.RouteDrawingMode mode, SessionSettings sessionSettings)
    {
      SessionCollection sessionList;
      // which sessions to draw?
      if (currentMouseTool == MouseTool.AdjustRoute)
      {
        sessionList = GetSessions(SessionDrawingMode.Current);
      }
      else
      {
        sessionList = GetSessions(SessionsToDraw);
      }
      document.DrawRoutes(sessionList, zoom, graphics, mapImage, mode, colorCodingAttribute, secondaryColorCodingAttribute, sessionSettings);
    }

    private SessionCollection GetSessions(SessionDrawingMode mode)
    {
      var coll = new SessionCollection();
      switch (mode)
      {
        case SessionDrawingMode.None:
          coll = new SessionCollection();
          break;
        case SessionDrawingMode.Current:
          coll.Add(CurrentSession);
          break;
        case SessionDrawingMode.Selected:
          coll = SelectedSessions;
          break;
        case SessionDrawingMode.All:
          coll = document.Sessions;
          break;
      }
      return coll;
    }

    private void DrawMarkers()
    {
      // copy route graphics as a background to route with handles
      routeWithHandlesGraphics.DrawImage(routeImage, new Point(0, 0));
      //BitBltWrapper(mRouteImage, mRouteWithHandlesGraphics);

      // draw the handles
      if (currentMouseTool == MouseTool.AdjustRoute)
      {
        foreach (Handle h in CurrentSession.Handles)
        {
          if (h.MarkerDrawer != null) h.MarkerDrawer.Draw(routeWithHandlesGraphics, zoom * h.Location, zoom);
        }
      }

      if (mouseHoverHandle != null && mouseHoverHandle.MarkerDrawer != null)
      {
        mouseHoverHandle.MarkerDrawer.Draw(routeWithHandlesGraphics, zoom * mouseHoverHandle.Location, zoom);
      }

      // laps
      foreach (Lap lap in CurrentSession.Laps)
      {
        if (lap.LapType == LapType.Lap || (lap.LapType == LapType.Stop && CurrentSession.Route.Segments.Count > 1))
        {
          ParameterizedLocation pl = CurrentSession.Route.GetParameterizedLocationFromTime(lap.Time);
          PointD p = CurrentSession.AdjustedRoute.GetLocationFromParameterizedLocation(pl);
          CurrentSession.Settings.MarkerDrawers[MarkerType.Lap].Draw(routeWithHandlesGraphics, zoom * p, zoom);
        }
      }
    }

    private void ResizeCanvas()
    {
      if (document != null && document.Map != null)
      {
        Console.WriteLine("ResizeCanvas");
        UpdateScrollbarProperties();
        if (!scrXPanel.Visible) scrX.Value = scrX.Minimum;
        if (!scrY.Visible) scrY.Value = scrY.Minimum;
        scrX.Value = Math.Min(scrX.Value, scrX.Maximum - scrX.LargeChange + 1);
        scrY.Value = Math.Min(scrY.Value, scrY.Maximum - scrY.LargeChange + 1);
        canvasPanel.CreateGraphics().Clear(canvasPanel.BackColor);
        UpdateBackBuffer(canvasPanel.Bounds);
      }
    }

    private void UpdateScrollbarProperties()
    {
      if (zoomedMapImage == null) return;
      scrXPanel.Visible = Width < zoomedMapImage.Size.Width;
      scrY.Visible = Height - (scrXPanel.Visible ? scrXPanel.Height : 0) < zoomedMapImage.Size.Height;
      scrXPanel.Visible = Width - (scrY.Visible ? scrY.Width : 0) < zoomedMapImage.Size.Width;
      scrY.Visible = Height - (scrXPanel.Visible ? scrXPanel.Height : 0) < zoomedMapImage.Size.Height;
      bottomRightPanel.Visible = (scrXPanel.Visible && scrY.Visible);
      canvasPanel.Size = new Size(Width - (scrY.Visible ? scrY.Width : 0),
                                  Height - (scrXPanel.Visible ? scrXPanel.Height : 0));
      scrX.Maximum = zoomedMapImage.Size.Width;
      scrX.LargeChange = canvasPanel.Width;
      scrY.Maximum = zoomedMapImage.Size.Height;
      scrY.LargeChange = canvasPanel.Height;
    }

    /// <summary>
    /// Centers map around point p (non-zoomed coordinates)
    /// </summary>
    /// <param name="p"></param>
    public void CenterMap(PointD p)
    {
      PointD zoomedPoint = zoom * p;
      if (scrXPanel.Visible)
      {
        var value = (int)(zoomedPoint.X - (CanvasSize.Width) / 2.0);
        scrX.Value = Math.Min(Math.Max(value, scrX.Minimum), scrX.Maximum - scrX.LargeChange);
      }
      else
      {
        scrX.Value = 0;
      }

      if (scrY.Visible)
      {
        var value = (int)(zoomedPoint.Y - (CanvasSize.Height) / 2.0);
        scrY.Value = Math.Min(Math.Max(value, scrY.Minimum), scrY.Maximum - scrY.LargeChange);
      }
      else
      {
        scrY.Value = 0;
      }
    }

    private RectangleD GetViewport()
    {
      return new RectangleD(
        scrX.Value / zoom,
        scrY.Value / zoom,
        (CanvasSize.Width) / zoom,
        (CanvasSize.Height) / zoom);
    }

    public void EnsureLegIsVisible(RectangleD rect, bool allowZoomChange)
    {
      RectangleD viewport = GetViewport();

      // inner viewport is 90% of viewport
      double side = Math.Min(viewport.Width, viewport.Height);
      var innerViewport = new RectangleD(
        viewport.Left + 0.05 * side,
        viewport.Top + 0.05 * side,
        viewport.Width - 0.10 * side,
        viewport.Height - 0.10 * side);

      var center = new PointD();

      if (rect.Width > innerViewport.Width)
      {
        center.X = rect.Left + rect.Width / 2;
      }
      else if (rect.Right < innerViewport.Left || rect.Left > innerViewport.Right)
      {
        center.X = rect.Center.X;
      }
      else if (rect.Left < innerViewport.Left)
      {
        center.X = innerViewport.Center.X + rect.Left - innerViewport.Left;
      }
      else if (rect.Right > innerViewport.Right)
      {
        center.X = innerViewport.Center.X + rect.Right - innerViewport.Right;
      }
      else
      {
        center.X = innerViewport.Center.X;
      }

      if (rect.Height > innerViewport.Height)
      {
        center.Y = rect.Top + rect.Height / 2;
      }
      else if (rect.Bottom < innerViewport.Top || rect.Top > innerViewport.Bottom)
      {
        center.Y = rect.Center.Y;
      }
      else if (rect.Top < innerViewport.Top)
      {
        center.Y = innerViewport.Center.Y + rect.Top - innerViewport.Top;
      }
      else if (rect.Bottom > innerViewport.Bottom)
      {
        center.Y = innerViewport.Center.Y + rect.Bottom - innerViewport.Bottom;
      }
      else
      {
        center.Y = innerViewport.Center.Y;
      }

      if (!(center.X == innerViewport.Center.X && center.Y == innerViewport.Center.Y)) CenterMap(center);
      if (allowZoomChange && (rect.Width > innerViewport.Width || rect.Height > innerViewport.Height))
      {
        // rectangle larger than innerViewport: change zoom
        double newZoom = Math.Min(innerViewport.Width / rect.Width, innerViewport.Height / rect.Height) * zoom;
        Zoom = newZoom;
      }
    }

    /// <summary>
    /// Makes sure that the line starting at p0 and ending at p1 is fully visible. If the line does not fit, place center of line at center of screen.
    /// </summary>
    /// <param name="p0">Start of line (in unzoomed map coordinates)</param>
    /// <param name="p1">End of line (in unzoomed map coordinates)</param>
    public void EnsureLegIsVisible(PointD p0, PointD p1)
    {
      RectangleD viewport = GetViewport();

      // inner viewport is 90% of viewport
      double side = Math.Min(viewport.Width, viewport.Height);
      var innerViewport = new RectangleD(
        viewport.Left + 0.05 * side,
        viewport.Top + 0.05 * side,
        viewport.Width - 0.10 * side,
        viewport.Height - 0.10 * side);

      var center = new PointD();

      if (Math.Abs(p1.X - p0.X) > innerViewport.Width)
      {
        center.X = (p0.X + p1.X) / 2;
      }
      else if (Math.Min(p0.X, p1.X) < innerViewport.Left)
      {
        center.X = innerViewport.Center.X - (innerViewport.Left - Math.Min(p0.X, p1.X));
      }
      else if (Math.Max(p0.X, p1.X) > innerViewport.Right)
      {
        center.X = innerViewport.Center.X + (Math.Max(p0.X, p1.X) - innerViewport.Right);
      }
      else
      {
        center.X = viewport.Center.X;
      }

      if (Math.Abs(p1.Y - p0.Y) > innerViewport.Width)
      {
        center.Y = (p0.Y + p1.Y) / 2;
      }
      else if (Math.Min(p0.Y, p1.Y) < innerViewport.Top)
      {
        center.Y = innerViewport.Center.Y - (innerViewport.Top - Math.Min(p0.Y, p1.Y));
      }
      else if (Math.Max(p0.Y, p1.Y) > innerViewport.Bottom)
      {
        center.Y = innerViewport.Center.Y + (Math.Max(p0.Y, p1.Y) - innerViewport.Bottom);
      }
      else
      {
        center.Y = viewport.Center.Y;
      }

      if (!(center.X == viewport.Center.X && center.Y == viewport.Center.Y)) CenterMap(center);
    }

    private void CreateBackBuffer()
    {
      if (document != null && document.Map != null)
      {
        // Dispose previus images and graphics objects
        if (zoomedMapImage != null) zoomedMapImage.Dispose();
        if (routeImage != null) routeImage.Dispose();
        if (routeGraphics != null) routeGraphics.Dispose();
        if (routeWithHandlesImage != null) routeWithHandlesImage.Dispose();
        if (routeWithHandlesGraphics != null) routeWithHandlesGraphics.Dispose();

        // create new images and graphics
        zoomedMapImage = document.CreateMapImage(zoom);

        routeImage = new Bitmap(zoomedMapImage.Width, zoomedMapImage.Height, PixelFormat.Format32bppPArgb);
        routeGraphics = Graphics.FromImage(routeImage);
        routeGraphics.SmoothingMode = SmoothingMode.AntiAlias;

        routeWithHandlesImage = new Bitmap(zoomedMapImage.Width, zoomedMapImage.Height, PixelFormat.Format32bppPArgb);
        routeWithHandlesGraphics = Graphics.FromImage(routeWithHandlesImage);
        routeWithHandlesGraphics.SmoothingMode = SmoothingMode.AntiAlias;
      }
    }

    public void AddLap(ParameterizedLocation parameterizedLocation, bool showLapTimeForm)
    {
      var time = CurrentSession.Route.GetTimeFromParameterizedLocation(parameterizedLocation);
      time = time.AddTicks(-time.Ticks % TimeSpan.TicksPerSecond); // truncate to nearest second

      if (showLapTimeForm)
      {
        using (var form = new LapTimeForm { InitialTime = time, Session = CurrentSession })
        {
          if (form.ShowDialog() != DialogResult.OK) return;
          time = form.Time;
        }
      }
      if (!CurrentSession.Laps.Exists(time))
      {
        ExecuteAction(new AddLapAction(new Lap(time, LapType.Lap), CurrentSession));
        DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
      }
    }

    public void DeleteLap(Lap lap)
    {
      ExecuteAction(new DeleteLapAction(lap, CurrentSession));
      DrawMap(MapDrawingFlags.Markers);
    }

    private void ExecuteAction(IAction action)
    {
      action.Execute();
      if (ActionPerformed != null) ActionPerformed(this, new ActionEventArgs(action));
    }

    public void Cut(ParameterizedLocation parameterizedLocation)
    {
      Cut(parameterizedLocation, null);
    }

    public void Cut(ParameterizedLocation parameterizedLocation, DateTime? time)
    {
      if (!time.HasValue) time = CurrentSession.Route.GetTimeFromParameterizedLocation(parameterizedLocation);

      using (var form = new RouteCutForm { InitialTime = time.Value })
      {
        double distance =
          CurrentSession.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, parameterizedLocation).
            Value;
        double totalDistance = CurrentSession.Route.LastWaypoint.Attributes[WaypointAttribute.Distance].Value;
        form.CutType = (distance < totalDistance/2 ? CutType.Before : CutType.After);

        if (form.ShowDialog() == DialogResult.OK)
        {
          mouseHoverHandle = null;
          var action = new CutRouteAction(CurrentSession, form.Time,
                                          form.CutType);
          ExecuteAction(action);
          DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
        }
      }
    }

    private void DeleteActiveHandle()
    {
      ExecuteAction(new DeleteHandleAction(activeHandle, CurrentSession));
      DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
    }

    private void BeginActiveHandleMoving()
    {
      PointD mouseLocation =
        new PointD((mouseInfo.MouseDownArgs.X - origo.X + scrX.Value),
                   (mouseInfo.MouseDownArgs.Y - origo.Y + scrY.Value)) / zoom;
      activeHandle.Location = mouseLocation - activeHandleOffset;
      activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.MovingActiveHandle];
      isNewHandle = !CurrentSession.ContainsHandle(activeHandle);
      if (isNewHandle)
      {
        CurrentSession.AddHandle(activeHandle);
        isNewHandle = true;
      }
      movingActiveHandleNow = true;
      CurrentSession.UpdateHandle(activeHandle);
      DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
    }

    private void EndActiveHandleMoving()
    {
      movingActiveHandleNow = false;
      if (isNewHandle)
      {
        ExecuteAction(new AddHandleAction(activeHandle, CurrentSession));
      }
      else
      {
        ExecuteAction(new MoveHandleAction(activeHandle, CurrentSession, activeHandleOldLocation));
      }
      DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
    }

    private void EndActiveLapMoving()
    {
      movingActiveLapNow = false;
      if (!CurrentSession.Laps.Exists(activeLap.Time, 2))
      {
        ExecuteAction(new EditLapAction(activeLap, oldActiveLap, CurrentSession));
        DrawMap(MapDrawingFlags.Route | MapDrawingFlags.Markers);
      }
      else
      {
        activeLap.Time = oldActiveLap.Time;
      }
    }

    public static Cursor GetCursor(MouseTool mouseTool)
    {
      // Cursors should be saved with 1 bit per pixel. 
      // First make png image in Photoshop, then open it in GIF Movie Gear and select Frame > Edit Pixels. 
      // This reduces the bpp number. Save as .cur file.
      var resources = new ComponentResourceManager(typeof(Canvas));

      switch (mouseTool)
      {
        case MouseTool.ZoomIn:
          return new Cursor(new MemoryStream((byte[])resources.GetObject("ZoomIn")));
        case MouseTool.ZoomOut:
          return new Cursor(new MemoryStream((byte[])resources.GetObject("ZoomOut")));
        case MouseTool.Cut:
          return new Cursor(new MemoryStream((byte[])resources.GetObject("Cut")));
        case MouseTool.MoveMap:
          return new Cursor(new MemoryStream((byte[])resources.GetObject("MoveMap")));
        case MouseTool.AdjustRoute:
          return new Cursor(new MemoryStream((byte[])resources.GetObject("AdjustRoute")));
        case MouseTool.Lap:
          return new Cursor(new MemoryStream((byte[])resources.GetObject("Lap")));
        case MouseTool.Pointer:
          return Cursors.Default;
        default:
          return Cursors.Default;
      }
    }

    private void ZoomIn()
    {
      var zoomRectangle = new Rectangle(
        Math.Min(mouseInfo.MouseDownArgs.X, mouseInfo.MouseUpArgs.X) - origo.X,
        Math.Min(mouseInfo.MouseDownArgs.Y, mouseInfo.MouseUpArgs.Y) - origo.Y,
        Math.Abs(mouseInfo.MouseUpArgs.X - mouseInfo.MouseDownArgs.X) - origo.X,
        Math.Abs(mouseInfo.MouseUpArgs.Y - mouseInfo.MouseDownArgs.Y) - origo.Y);
      PointD centerPointBeforeZoom = new PointD(
                                       scrX.Value + (double)zoomRectangle.Left + (double)zoomRectangle.Width / 2,
                                       scrY.Value + (double)zoomRectangle.Top + (double)zoomRectangle.Height / 2) / zoom;
      double zoomFactor;

      if (zoomRectangle.Width > 0 && zoomRectangle.Height > 0)
      {
        zoomFactor = Math.Min(canvasPanel.Width / (double)zoomRectangle.Width,
                              canvasPanel.Height / (double)zoomRectangle.Height);
      }
      else
      {
        zoomFactor = 4.0 / 3;
      }
      Zoom *= zoomFactor;
      CenterMap(new PointD(centerPointBeforeZoom.X, centerPointBeforeZoom.Y));
    }

    private void ZoomOut()
    {
      PointD centerPointBeforeZoom = new PointD(
                                       scrX.Value + (double)mouseInfo.MouseDownArgs.X - origo.X,
                                       scrY.Value + (double)mouseInfo.MouseDownArgs.Y - origo.Y) / zoom;
      Zoom *= 0.75;
      CenterMap(new PointD(centerPointBeforeZoom.X, centerPointBeforeZoom.Y));
    }

    #endregion

    #region Event handlers

    private void CanvasPanel_Resize(object sender, EventArgs e)
    {
      ResizeCanvas();
    }

    private void Scrollbar_Scroll(object sender, ScrollEventArgs e)
    {
      if (!preventScrollEvents) UpdateBackBuffer();
    }

    private void Scrollbar_ValueChanged(object sender, EventArgs e)
    {
      if (!preventScrollEvents) UpdateBackBuffer();
    }

    private void CanvasPanel_MouseDown(object sender, MouseEventArgs e)
    {
      mouseInfo.MouseDownArgs = e;

      if (e.Button == MouseButtons.Right && (scrXPanel.Visible || scrY.Visible))
      {
        // move map
        BeginMoveMapWithMouse();
      }
      else
      {
        switch (currentMouseTool)
        {
          case MouseTool.AdjustRoute:

            #region AdjustRoute

            // is there a mouse hover handle?
            if (mouseHoverHandle != null)
            {
              // transform mouse hover handle to adjustment handle
              activeHandle = mouseHoverHandle;
              activeHandleOldLocation = activeHandle.Location;
              PointD mouseLocation =
                new PointD((mouseInfo.MouseDownArgs.X + scrX.Value - origo.X),
                           (mouseInfo.MouseDownArgs.Y + scrY.Value - origo.Y)) / zoom;
              activeHandleOffset = mouseLocation - mouseHoverHandle.Location;
              BeginActiveHandleMoving();
            }
            else if (activeHandle != null)
            {
              if ((ModifierKeys & Keys.Control) == Keys.Control)
              {
                // delete active handle
                DeleteActiveHandle();
              }
              else
              {
                BeginActiveHandleMoving();
                activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.MovingActiveHandle];
              }
            }
            else
            {
              BeginMoveMapWithMouse();
            }
            break;

            #endregion

          case MouseTool.ZoomIn:
            break;

          case MouseTool.ZoomOut:
            ZoomOut();
            break;

          case MouseTool.Cut:

            #region Cut

            // cut route
            // determine the handle where we should cut
            Handle cutHandle = null;
            if (activeHandle != null)
            {
              cutHandle = activeHandle;
            }
            else if (mouseHoverHandle != null)
            {
              cutHandle = mouseHoverHandle;
            }
            else
            {
              BeginMoveMapWithMouse();
            }

            if (cutHandle != null)
            {
              // if not at a lap, round time to nearest second
              DateTime cutTime;
              if (cutHandle.Time.HasValue)
              {
                cutTime = cutHandle.Time.Value;
              }
              else
              {
                cutTime = CurrentSession.Route.GetTimeFromParameterizedLocation(cutHandle.ParameterizedLocation);
                cutTime = new DateTime(TimeSpan.TicksPerSecond * (long)Math.Round((double)cutTime.Ticks / TimeSpan.TicksPerSecond), cutTime.Kind);  // round to nearest second
              }
              Cut(cutHandle.ParameterizedLocation, cutTime);
            }
            break;

            #endregion

          case MouseTool.Lap:

            #region Lap

            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
              // Ctrl pressed: delete lap
              if (activeLap != null && activeLap.LapType == LapType.Lap)
              {
                // delete lap
                ExecuteAction(new DeleteLapAction(activeLap, CurrentSession));
                DrawMap(MapDrawingFlags.Markers);
              }
              else if(activeLap == null)
              {
                BeginMoveMapWithMouse();
              }
            }
            else
            {
              if (activeLap != null && activeLap.LapType == LapType.Lap)
              {
                // Close to lap: begin move lap
                movingActiveLapNow = true;
                oldActiveLap = (Lap)activeLap.Clone();
              }
              else if(activeLap == null)
              {
                // add lap
                Handle addLapHandle = null;
                if (activeHandle != null)
                {
                  addLapHandle = activeHandle;
                }
                else if (mouseHoverHandle != null)
                {
                  addLapHandle = mouseHoverHandle;
                }
                else
                {
                  BeginMoveMapWithMouse();
                }

                if (addLapHandle != null)
                {
                  bool showLapTimeForm = ((ModifierKeys & Keys.Shift) == Keys.Shift);
                  AddLap(addLapHandle.ParameterizedLocation, showLapTimeForm);
                }
              }
            }
            break;

          default:
            BeginMoveMapWithMouse();
            break;

            #endregion
        }
      }
    }

    private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
    {
      MouseEventArgs oldMouseMoveArgs = mouseInfo.MouseMoveArgs;
      mouseInfo.MouseMoveArgs = e;

      if (movingMapWithMouseNow)
      {
        MoveMapWithMouse();
      }
      else
      {
        switch (currentMouseTool)
        {
          case MouseTool.Pointer:
          case MouseTool.AdjustRoute:
          case MouseTool.Cut:
          case MouseTool.Lap:
            CheckMouseHovering();
            break;

          case MouseTool.ZoomIn:
            if (mouseInfo.MouseDownArgs != null)
            {
              if (oldMouseMoveArgs != null)
                RubberBandRectangle.DrawXORRectangle(canvasPanel.CreateGraphics(), mouseInfo.MouseDownArgs.X,
                                                     mouseInfo.MouseDownArgs.Y, oldMouseMoveArgs.X, oldMouseMoveArgs.Y,
                                                     PenStyles.PS_DOT);
              RubberBandRectangle.DrawXORRectangle(canvasPanel.CreateGraphics(), mouseInfo.MouseDownArgs.X,
                                                   mouseInfo.MouseDownArgs.Y, mouseInfo.MouseMoveArgs.X,
                                                   mouseInfo.MouseMoveArgs.Y, PenStyles.PS_DOT);
            }
            break;
        }
      }
    }

    private void CanvasPanel_MouseUp(object sender, MouseEventArgs e)
    {
      mouseInfo.MouseUpArgs = e;
      preventScrollEvents = false;

      if (movingMapWithMouseNow)
      {
        EndMoveMapWithMouse();
      }
      else
      {
        switch (currentMouseTool)
        {
          case MouseTool.AdjustRoute:

            if (movingActiveHandleNow)
            {
              EndActiveHandleMoving();
            }
            if (activeHandle != null)
            {
              activeHandle.MarkerDrawer = CurrentSession.Settings.MarkerDrawers[MarkerType.Handle];
              activeHandle = null;
            }
            break;

          case MouseTool.ZoomIn:
            if (mouseInfo.MouseMoveArgs != null)
              RubberBandRectangle.DrawXORRectangle(canvasPanel.CreateGraphics(), mouseInfo.MouseDownArgs.X,
                                                   mouseInfo.MouseDownArgs.Y, mouseInfo.MouseMoveArgs.X,
                                                   mouseInfo.MouseMoveArgs.Y, PenStyles.PS_DOT);
            ZoomIn();
            break;

          case MouseTool.Lap:
            if (movingActiveLapNow)
            {
              EndActiveLapMoving();
            }
            break;
        }
      }
      mouseInfo.Reset();
    }

    private void CanvasPanel_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((ModifierKeys & Keys.Control) == Keys.Control)
      {
        Zoom += 0.1 * Math.Sign(e.Delta);
      }
      else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
      {
        scrX.Value = Math.Max(scrX.Minimum,
                              Math.Min(scrX.Maximum - scrX.LargeChange, scrX.Value - Math.Sign(e.Delta) * scrX.SmallChange));
      }
      else
      {
        scrY.Value = Math.Max(scrY.Minimum,
                              Math.Min(scrY.Maximum - scrY.LargeChange, scrY.Value - Math.Sign(e.Delta) * scrY.SmallChange));
      }
    }

    private void CanvasPanel_Paint(object sender, PaintEventArgs e)
    {
      UpdateBackBuffer(e.ClipRectangle);
    }

    private void Document_MapChanged(object sender, EventArgs e)
    {
      CreateBackBuffer();
    }

    private void Document_DocumentChanged(object sender, EventArgs e)
    {
      if (DocumentChanged != null) DocumentChanged(this, new EventArgs());
    }

    private void Canvas_KeyDown(object sender, KeyEventArgs e)
    {
      // Ctrl++
      if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
      {
        Zoom *= 4.0 / 3;
      }

      // Ctrl+-
      if (e.Modifiers == Keys.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
      {
        Zoom *= 0.75;
      }
    }

    #endregion

    #region Public classes

    #region Nested type: ActionEventArgs

    public class ActionEventArgs : EventArgs
    {
      public ActionEventArgs(IAction action)
      {
        Action = action;
      }

      public IAction Action { get; set; }
    }

    #endregion

    #region Nested type: RouteMouseHoverEventArgs

    public class RouteMouseHoverEventArgs : EventArgs
    {
      public RouteMouseHoverEventArgs(ParameterizedLocation parameterizedLocation, bool isClose)
      {
        ParameterizedLocation = parameterizedLocation;
        IsClose = isClose;
      }

      public ParameterizedLocation ParameterizedLocation { get; set; }

      public bool IsClose { get; set; }
    }

    #endregion

    #region Nested type: SessionParameterizedLocationEventArgs

    public class SessionParameterizedLocationEventArgs : EventArgs
    {
      public SessionParameterizedLocationEventArgs(Session session, ParameterizedLocation parameterizedLocation)
      {
        Session = session;
        ParameterizedLocation = parameterizedLocation;
      }

      public Session Session { get; set; }

      public ParameterizedLocation ParameterizedLocation { get; set; }
    }

    #endregion

    #endregion

    #region Private classes

    #region Nested type: MouseInfo

    private class MouseInfo
    {
      private MouseEventArgs mouseDownArgs;
      private MouseEventArgs mouseMoveArgs;
      private MouseEventArgs mouseUpArgs;

      public MouseEventArgs MouseDownArgs
      {
        get { return mouseDownArgs; }
        set { mouseDownArgs = value; }
      }

      public MouseEventArgs MouseMoveArgs
      {
        get { return mouseMoveArgs; }
        set { mouseMoveArgs = value; }
      }

      public MouseEventArgs MouseUpArgs
      {
        get { return mouseUpArgs; }
        set { mouseUpArgs = value; }
      }

      public void Reset()
      {
        mouseDownArgs = null;
        mouseMoveArgs = null;
        mouseUpArgs = null;
      }
    }

    #endregion

    #endregion

    #region Enums

    #region MapDrawingFlags enum

    [Flags]
    public enum MapDrawingFlags
    {
      Route = 1,
      Markers = 2
    }

    #endregion

    #region MouseTool enum

    public enum MouseTool
    {
      Pointer,
      AdjustRoute,
      ZoomIn,
      ZoomOut,
      Cut,
      Lap,
      MoveMap
    }

    #endregion

    #region SessionDrawingMode enum

    public enum SessionDrawingMode
    {
      Selected,
      Current,
      None,
      All
    }

    #endregion

    #endregion
  }
}