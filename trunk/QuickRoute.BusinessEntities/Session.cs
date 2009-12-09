using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using QuickRoute.BusinessEntities.Importers.GPX.GPX11;
using System.Drawing;
using System.Globalization;
using Wintellect.PowerCollections;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class Session
  {
    #region  Private fields

    private SessionInfo sessionInfo = new SessionInfo();
    private Route route;
    private LongLat projectionOrigin;
    private GeneralMatrix initialTransformationMatrix;
    [NonSerialized]
    private AdjustedRoute adjustedRoute;
    [NonSerialized]
    private List<AlphaAdjustmentChange> alphaAdjustmentChanges;
    private LapCollection laps;
    private HandleCollection handles = new HandleCollection();
    private SessionSettings settings = new SessionSettings();

    #endregion

    #region  Constructors

    public Session(Route route, LapCollection laps, Size mapSize, SessionSettings settings)
      : this(route, laps, mapSize, null, route.CenterLongLat(), settings) { }

    public Session(Route route, LapCollection laps, Size mapSize, GeneralMatrix initialTransformationMatrix, SessionSettings settings)
      : this(route, laps, mapSize, initialTransformationMatrix, null, settings) { }

    public Session(Route route, LapCollection laps, Size mapSize, GeneralMatrix initialTransformationMatrix, LongLat projectionOrigin, SessionSettings settings)
    {
      this.route = route;
      this.route.SuppressWaypointAttributeCalculation = true;
      this.route.SmoothingIntervals = settings.SmoothingIntervals;
      this.laps = laps;
      this.projectionOrigin = projectionOrigin ?? route.CenterLongLat();
      this.settings = settings;
      this.initialTransformationMatrix = initialTransformationMatrix ??
                                         RouteAdjustmentManager.CreateInitialTransformationMatrix(this.route, mapSize, this.projectionOrigin);
      this.route.SuppressWaypointAttributeCalculation = false;
      Initialize();
    }

    #endregion

    #region Public properties

    public SessionInfo SessionInfo
    {
      get { return sessionInfo ?? new SessionInfo(); }
      set { sessionInfo = value; }
    }

    public Route Route
    {
      get { return route; }
      set { route = value; }
    }

    public LongLat ProjectionOrigin
    {
      get { return projectionOrigin; }
      set { projectionOrigin = value; }
    }

    public AdjustedRoute AdjustedRoute
    {
      get { return adjustedRoute; }
    }

    public LapCollection Laps
    {
      get { return laps; }
      set { laps = value; }
    }

    public Handle[] Handles
    {
      get
      {
        ArrayList handleArrayList = new ArrayList();
        foreach (var h in handles)
        {
          if (h.Type == Handle.HandleType.Handle) handleArrayList.Add(h);
        }
        return handleArrayList.ToArray(typeof(Handle)) as Handle[];
      }
    }

    public Handle StartHandle
    {
      get { return (handles.Count > 0 ? handles[0] : null); }
    }

    public Handle EndHandle
    {
      get { return (handles.Count == 0 ? null : handles[handles.Count - 1]); }
    }

    public SessionSettings Settings
    {
      get { return settings; }
      set
      {
        settings = value;
        route.SmoothingIntervals = settings.SmoothingIntervals;
      }
    }

    public List<AlphaAdjustmentChange> AlphaAdjustmentChanges
    {
      get { return alphaAdjustmentChanges; }
      set
      {
        alphaAdjustmentChanges = value;
        if (alphaAdjustmentChanges != null) alphaAdjustmentChanges.Sort();
      }
    }

    public GeneralMatrix InitialTransformationMatrix
    {
      get { return initialTransformationMatrix; }
      set { initialTransformationMatrix = value; }
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
      SetLapTimesToRoute(route);
      route.Initialize();
      route.EnsureUtcTimes();
      laps.EnsureUtcTimes();
      CreateAdjustedRoute();
    }

    /// <summary>
    /// To be called when a handle is added/deleted/moved. Transformation matrices are recalculated and the adjusted route is updated.
    /// </summary>
    /// <param name="h">The handle</param>
    public void UpdateHandle(Handle h)
    {
      UpdateTransformationMatrices(h);
      CreateAdjustedRoute();
    }

    //public void CreateAdjustedRoute_old()
    //{
    //  adjustedRoute = new AdjustedRoute();
    //  for (int i = 0; i < route.Segments.Count; i++)
    //  {
    //    adjustedRoute.Segments.Add(new AdjustedRouteSegment());
    //  }

    //  if (StartHandle == null)
    //  {
    //    // no handles, use the initial transformation matrix for all adjusted waypoints
    //    for (int i = 0; i < route.Segments.Count; i++)
    //    {
    //      for (int j = 0; j < route.Segments[i].Waypoints.Count; j++)
    //      {
    //        Waypoint waypoint = route.Segments[i].Waypoints[j];
    //        PointD location = LinearAlgebraUtil.Transform(waypoint.LongLat.Project(projectionOrigin), initialTransformationMatrix);
    //        adjustedRoute.Segments[i].Waypoints.Add(new AdjustedWaypoint(location, new ParameterizedLocation(i, j)));
    //      }
    //    }
    //  }
    //  else
    //  {
    //    // there are handles, use them

    //    //1. Create parameterized location list for adjusted route. Include 
    //    //   a) handles
    //    //   b) waypoints from route
    //    List<PLWithKey> plList = new List<PLWithKey>();
    //    foreach (Handle h in handles)
    //    {
    //      PLWithKey pwk = new PLWithKey(PLKey.Handle, h.ParameterizedLocation);
    //      plList.Add(pwk);
    //    }
    //    for (int i = 0; i < route.Segments.Count; i++)
    //    {
    //      RouteSegment rs = route.Segments[i];
    //      for (int j = 0; j < rs.Waypoints.Count; j++)
    //      {
    //        // todo: if a faster route line drawing is desired, don't add all waypoint locations to the list. Use some algorithm to determine which waypoints to throw away (e g these where acceleration is nearly constant). Have a look at Garmin FR305 Smart data recording algorithm.
    //        PLWithKey pwk = new PLWithKey(PLKey.Waypoint, new ParameterizedLocation(i, j));
    //        plList.Add(pwk);
    //      }
    //    }
    //    plList.Sort();

    //    // 2. Create and add an adjusted waypoint for each location in this list 
    //    Handle previousHandle = StartHandle;
    //    Handle nextHandle = StartHandle;
    //    double previousHandleDistance = 0;
    //    double nextHandleDistance = 0;
    //    int handleIndex = -1;

    //    foreach (PLWithKey pwk in plList)
    //    {
    //      Waypoint w;
    //      PointD location0;
    //      PointD location1;
    //      double distance;
    //      if (pwk.Key == PLKey.Handle)
    //      {
    //        handleIndex++;
    //        previousHandle = handles[handleIndex];
    //        previousHandleDistance = route.GetDistanceFromParameterizedLocation(previousHandle.ParameterizedLocation);
    //        nextHandle = handleIndex < handles.Count - 1 ? handles[handleIndex + 1] : handles[handleIndex];
    //        nextHandleDistance = route.GetDistanceFromParameterizedLocation(nextHandle.ParameterizedLocation);
    //        w = route.CreateWaypointFromParameterizedLocation(pwk);
    //        location0 = LinearAlgebraUtil.Transform(w.LongLat.Project(projectionOrigin), previousHandle.TransformationMatrix);
    //        location1 = LinearAlgebraUtil.Transform(w.LongLat.Project(projectionOrigin), nextHandle.TransformationMatrix);
    //        distance = route.GetDistanceFromParameterizedLocation(pwk);
    //      }
    //      else
    //      {
    //        if (pwk == previousHandle.ParameterizedLocation)
    //        {
    //          continue;
    //        }
    //        w = route.Segments[pwk.SegmentIndex].Waypoints[(int)pwk.Value];
    //        location0 = LinearAlgebraUtil.Transform(w.LongLat.Project(projectionOrigin), previousHandle.TransformationMatrix);
    //        location1 = LinearAlgebraUtil.Transform(w.LongLat.Project(projectionOrigin), nextHandle.TransformationMatrix);
    //        distance = w.Distance;
    //      }
    //      double t;
    //      if (nextHandleDistance == previousHandleDistance)
    //      {
    //        t = 0;
    //      }
    //      else
    //      {
    //        t = (distance - previousHandleDistance) / (nextHandleDistance - previousHandleDistance);
    //      }
    //      AdjustedWaypoint aw = new AdjustedWaypoint((1 - t) * location0 + t * location1, pwk);
    //      adjustedRoute.Segments[pwk.SegmentIndex].Waypoints.Add(aw);
    //    }
    //  }
    //}

    /// <summary>
    /// Creates an adjusted route based on the handles.
    /// </summary>
    public void CreateAdjustedRoute()
    {
      adjustedRoute = new AdjustedRoute();
      for (int i = 0; i < route.Segments.Count; i++)
      {
        adjustedRoute.Segments.Add(new AdjustedRouteSegment());
      }

      if (StartHandle == null)
      {
        // no handles, use the initial transformation matrix for all adjusted waypoints
        for (int i = 0; i < route.Segments.Count; i++)
        {
          for (int j = 0; j < route.Segments[i].Waypoints.Count; j++)
          {
            Waypoint waypoint = route.Segments[i].Waypoints[j];
            PointD location = LinearAlgebraUtil.Transform(waypoint.LongLat.Project(projectionOrigin), initialTransformationMatrix);
            adjustedRoute.Segments[i].Waypoints.Add(new AdjustedWaypoint(location, new ParameterizedLocation(i, j)));
          }
        }
      }
      else
      {
        // there are handles, use them

        //1. Create parameterized location list for adjusted route. Include 
        //   a) handles
        //   b) waypoints from route
        List<PLWithKey> plList = new List<PLWithKey>();
        // handles
        foreach (Handle h in handles)
        {
          PLWithKey pwk = new PLWithKey(PLKey.Handle, h.ParameterizedLocation);
          plList.Add(pwk);
        }
        // waypoints
        for (int i = 0; i < route.Segments.Count; i++)
        {
          RouteSegment rs = route.Segments[i];
          for (int j = 0; j < rs.Waypoints.Count; j++)
          {
            // todo: if a faster route line drawing is desired, don't add all waypoint locations to the list. Use some algorithm to determine which waypoints to throw away (e g these where acceleration is nearly constant). Have a look at Garmin FR305 Smart data recording algorithm.
            PLWithKey pwk = new PLWithKey(PLKey.Waypoint, new ParameterizedLocation(i, j));
            plList.Add(pwk);
          }
        }
        plList.Sort();

        // 2. Create and add an adjusted waypoint for each location in this list 
        Handle currentHandle = StartHandle;
        int handleIndex = -1;

        foreach (PLWithKey pwk in plList)
        {
          Waypoint w;
          if (pwk.Key == PLKey.Handle)
          {
            if (handleIndex < handles.Count - 2 || handleIndex == -1) handleIndex++;
            currentHandle = handles[handleIndex];
            w = route.CreateWaypointFromParameterizedLocation(pwk);
          }
          else
          {
            w = route.Segments[pwk.SegmentIndex].Waypoints[(int)pwk.Value];
          }
          PointD location = LinearAlgebraUtil.Transform(w.LongLat.Project(projectionOrigin), currentHandle.TransformationMatrix);
          AdjustedWaypoint aw = new AdjustedWaypoint(location, pwk);
          adjustedRoute.Segments[pwk.SegmentIndex].Waypoints.Add(aw);
        }
      }
    }

    public Color GetColorFromParameterizedLocation(ParameterizedLocation pl, WaypointAttribute colorCodingAttribute)
    {
      // determining value according to current attribute: pace, heart rate or altitude
      double? value = route.GetAttributeFromParameterizedLocation(colorCodingAttribute, pl);
      if (colorCodingAttribute == WaypointAttribute.Speed) value *= 3.6;
      RouteLineSettings rls = settings.RouteLineSettingsCollection[colorCodingAttribute];
      Color color = rls.ColorRange.GetColor(value == null ? 0 : (double)value);
      return color;
    }

    public GeneralMatrix GetTransformationMatrixFromParameterizedLocation(ParameterizedLocation parameterizedLocation)
    {
      switch (handles.Count)
      {
        case 0:
          return initialTransformationMatrix;

        case 1:
          return handles[0].TransformationMatrix;

        default:
          for (int i = 0; i < handles.Count; i++)
          {
            if (parameterizedLocation <= handles[i].ParameterizedLocation) return handles[i].TransformationMatrix;
          }
          return EndHandle.TransformationMatrix;
      }
    }

    /// <summary>
    /// Calculates the real-world length of one map pixel.
    /// </summary>
    /// <returns>The length in meters.</returns>
    public double GetPixelScale()
    {
      double sum = 0;
      for (int i = 0; i < handles.Count - 1; i++)
      {
        double unitLength = LinearAlgebraUtil.GetUnitLengthFromTransformationMatrix(handles[i].TransformationMatrix.Inverse());
        ElapsedTime startTime = new ElapsedTime((i == 0
                                   ? route.FirstWaypoint.Attributes[WaypointAttribute.ElapsedTime].Value  
                                   : route.GetAttributeFromParameterizedLocation(WaypointAttribute.ElapsedTime, handles[i].ParameterizedLocation).Value));
        ElapsedTime endTime = new ElapsedTime((i == handles.Count - 2
                                 ? route.LastWaypoint.Attributes[WaypointAttribute.ElapsedTime].Value  
                                 : route.GetAttributeFromParameterizedLocation(WaypointAttribute.ElapsedTime, handles[i + 1].ParameterizedLocation).Value));
        sum += unitLength * (endTime.Value - startTime.Value);
      }
      return route.LastWaypoint.Attributes[WaypointAttribute.ElapsedTime].Value - route.FirstWaypoint.Attributes[WaypointAttribute.ElapsedTime].Value == 0
               ? 0
               : sum / (route.LastWaypoint.Attributes[WaypointAttribute.ElapsedTime].Value - route.FirstWaypoint.Attributes[WaypointAttribute.ElapsedTime].Value);
    }

    public void AddHandle(Handle h)
    {
      handles.Add(h);
    }

    public bool RemoveHandle(Handle h)
    {
      return handles.Remove(h);
    }

    public int IndexOfHandle(Handle h)
    {
      return handles.IndexOf(h);
    }

    public bool ContainsHandle(Handle h)
    {
      return handles.Contains(h);
    }

    public HandleCollection.CutHandlesData CutHandles(ParameterizedLocation parameterizedLocation, CutType cutType)
    {
      return handles.Cut(parameterizedLocation, cutType);
    }

    public void UnCutHandles(HandleCollection.CutHandlesData cutHandlesData)
    {
      handles.UnCut(cutHandlesData);
    }

    public double GetLapDirectionFromParameterizedLocation(ParameterizedLocation pl)
    {
      if (laps == null) return 0;
      int lapIndex = GetLapIndexFromParameterizedLocation(pl);
      LongLat longLat0 = route.GetLocationFromParameterizedLocation(route.GetParameterizedLocationFromTime(laps[lapIndex].Time));
      LongLat longLat1 = route.GetLocationFromParameterizedLocation(route.GetParameterizedLocationFromTime(laps[lapIndex + 1].Time));
      LongLat middle = longLat0 / 2 + longLat1 / 2;
      PointD p0 = longLat0.Project(middle);
      PointD p1 = longLat1.Project(middle);
      return LinearAlgebraUtil.AnglifyD(LinearAlgebraUtil.ToDegrees(LinearAlgebraUtil.GetVectorDirectionR(p0, p1)), AngleStyle.N360CW);
    }

    public void DrawRoute(double zoom, Graphics graphics, Document.RouteDrawingMode mode, WaypointAttribute colorCodingAttribute, SessionSettings sessionSettings)
    {
      // use this session's settings if no settings are specified
      var rls = sessionSettings == null 
        ? Settings.RouteLineSettingsCollection[colorCodingAttribute] 
        : sessionSettings.RouteLineSettingsCollection[colorCodingAttribute];

      switch (mode)
      {
        case Document.RouteDrawingMode.Simple:
          foreach (var ars in AdjustedRoute.Segments)
          {
            if (ars.Waypoints.Count > 1)
            {
              var points = new PointF[ars.Waypoints.Count];
              for (var i = 0; i < ars.Waypoints.Count; i++)
              {
                points[i] = (PointF)(zoom * ars.Waypoints[i].Location);
              }
              // use monochrome color and width
              var pen = new Pen(rls.MonochromeColor, (float)(zoom * (rls.MonochromeWidth))) { LineJoin = LineJoin.Round };
              graphics.DrawLines(pen, points);
              pen.Dispose();
            }
          }
          break;

        case Document.RouteDrawingMode.Extended:
          double lineWidth = rls.Width;
          double maskWidth = (rls.MaskVisible ? rls.MaskWidth : 0);
          List<RouteLineVertex> vertices = CreateRouteLineVertices(
            AdjustedRoute.GetFirstParameterizedLocation(),
            AdjustedRoute.GetLastParameterizedLocation(),
            colorCodingAttribute
            );

          // filter vertices
          vertices = FilterVertices(vertices);

          for (int i = 0; i < vertices.Count - 2; i++)
          {
            if (vertices[i].ParameterizedLocation.SegmentIndex == vertices[i + 1].ParameterizedLocation.SegmentIndex)
            {
              RouteLineVertex previousVertex = (i > 0 &&
                                                vertices[i - 1].ParameterizedLocation.SegmentIndex ==
                                                vertices[i].ParameterizedLocation.SegmentIndex
                                                  ? vertices[i - 1]
                                                  : null);
              RouteLineVertex nextVertex = (i < vertices.Count - 2 &&
                                            vertices[i + 2].ParameterizedLocation.SegmentIndex ==
                                            vertices[i + 1].ParameterizedLocation.SegmentIndex
                                              ? vertices[i + 2]
                                              : null);
              var line = new RouteLine(
                vertices[i],
                vertices[i + 1],
                previousVertex,
                nextVertex,
                lineWidth,
                maskWidth,
                zoom);
              Color lineColor = GraphicsUtil.AlphaAdjustColor(vertices[i].Color, vertices[i].AlphaAdjustment);
              Color maskColor = GraphicsUtil.AlphaAdjustColor(rls.MaskColor, vertices[i].AlphaAdjustment);
              Brush b = new SolidBrush(lineColor);
              graphics.FillPath(b, line.LinePath);
              b.Dispose();
              b = new SolidBrush(maskColor);
              if (line.MaskPath != null) graphics.FillPath(b, line.MaskPath);
              b.Dispose();
            }
          }
          break;

        case Document.RouteDrawingMode.None:
          // don't draw anything
          break;
      }
    }

    public List<PointD> GetAdjustedWaypointLocations(IList<int> legs)
    {
      var vertices = new List<PointD>();

      bool[] legExists = new bool[laps.Count - 1];
      for (int i = 1; i < laps.Count; i++)
      {
        legExists[i - 1] = legs.Contains(i - 1);
      }

      bool currentState = false;
      var breakpoints = new List<ParameterizedLocation>();
      for (int i = 0; i < laps.Count - 1; i++)
      {
        if (legExists[i] != currentState)
        {
          breakpoints.Add(route.GetParameterizedLocationFromTime(laps[i].Time));
          currentState = !currentState;
        }
      }
      if (currentState) breakpoints.Add(route.GetParameterizedLocationFromTime(laps[laps.Count - 1].Time));


      int currentBreakpoint = 0;
      currentState = false;
      foreach (var w in adjustedRoute.GetAllWaypoints())
      {
        if ((!currentState && w.ParameterizedLocation >= breakpoints[currentBreakpoint])
            ||
            (currentState && w.ParameterizedLocation > breakpoints[currentBreakpoint]))
        {
          currentState = !currentState;
          currentBreakpoint++;
        }
        if (currentState) vertices.Add(w.Location);
        if (currentBreakpoint > breakpoints.Count - 1) break;
      }

      return vertices;
    }

    public void AddTimeOffset(TimeSpan offset)
    {
      route.AddTimeOffset(offset);
      laps.AddTimeOffset(offset);
      SetLapTimesToRoute();
    }

    public Route CreateRouteAdaptedToSingleTransformationMatrix(GeneralMatrix transformationMatrix)
    {
      return CreateRouteAdaptedToSingleTransformationMatrix(this);
    }
    
    public Route CreateRouteAdaptedToSingleTransformationMatrix(Session baseSession)
    {
      var transformationMatrix = baseSession.CalculateAverageTransformation().TransformationMatrix;
      var transformationMatrixInverse = transformationMatrix.Inverse();
      var routeSegments = new List<RouteSegment>();
      foreach (var ars in adjustedRoute.Segments)
      {
        var routeSegment = new RouteSegment();
        foreach (var aw in ars.Waypoints)
        {
          var newWaypoint = route.CreateWaypointFromParameterizedLocation(aw.ParameterizedLocation);
          // create pixel location
          var projectedLocation = LinearAlgebraUtil.Transform(aw.Location, transformationMatrixInverse);
          newWaypoint.LongLat = LongLat.Deproject(projectedLocation, baseSession.ProjectionOrigin);
          routeSegment.Waypoints.Add(newWaypoint);
        }
        routeSegments.Add(routeSegment);
      }
      var r = new Route(routeSegments);
      SetLapTimesToRoute(r);
      r.SmoothingIntervals = route.SmoothingIntervals;
      return r;
    }

    /// <summary>
    /// Using linear least squares algorithm described at http://en.wikipedia.org/wiki/Linear_least_squares
    /// </summary>
    /// <returns></returns>
    public Transformation CalculateAverageTransformation()
    {
      return new SessionCollection { this }.CalculateAverageTransformation();
    }

    /// <summary>
    /// Used to keep internal route lap time list and laps in sync
    /// </summary>
    public void SetLapTimesToRoute()
    {
      SetLapTimesToRoute(route);
    }

    /// <summary>
    /// Used to keep internal route lap time list and laps in sync
    /// </summary>
    /// <param name="r"></param>
    private void SetLapTimesToRoute(Route r)
    {
      var lapTimes = new OrderedBag<DateTime>();
      foreach (var l in Laps)
      {
        lapTimes.Add(l.Time.ToUniversalTime());
      }
      r.LapTimes = lapTimes;
    }

    #endregion

    #region  Private methods

    /// <summary>
    /// Calculates transformation matrices based on the locations of the handles.
    /// </summary>
    /// <param name="activeHandle">The handle that has changed</param>
    private void UpdateTransformationMatrices_old(Handle activeHandle)
    {
      switch (handles.Count)
      {
        case 0:
          break;

        case 1:
          {
            // perform translation of whole route
            GeneralMatrix newTransformationMatrix = (GeneralMatrix)initialTransformationMatrix.Clone();
            PointD initialProjectedLocation = route.GetProjectedLocationFromParameterizedLocation(activeHandle.ParameterizedLocation, projectionOrigin);
            PointD initialAdjustedLocation = LinearAlgebraUtil.Transform(initialProjectedLocation, initialTransformationMatrix);
            PointD translation = activeHandle.Location - initialAdjustedLocation;
            newTransformationMatrix.SetElement(0, 2, newTransformationMatrix.GetElement(0, 2) + translation.X);
            newTransformationMatrix.SetElement(1, 2, newTransformationMatrix.GetElement(1, 2) + translation.Y);
            activeHandle.TransformationMatrix = newTransformationMatrix;
            break;
          }

        case 2:
          {
            // perform scaling of whole route based on two points
            Handle firstHandle = null;
            Handle secondHandle = activeHandle;
            foreach (Handle h in handles)
            {
              if (!h.Equals(activeHandle))
              {
                firstHandle = h;
                break;
              }
            }

            GeneralMatrix newTransformationMatrix = LinearAlgebraUtil.CalculateTransformationMatrix(
              route.GetProjectedLocationFromParameterizedLocation(firstHandle.ParameterizedLocation, projectionOrigin),
              firstHandle.Location,
              route.GetProjectedLocationFromParameterizedLocation(secondHandle.ParameterizedLocation, projectionOrigin),
              secondHandle.Location,
              firstHandle.TransformationMatrix, false);

            firstHandle.TransformationMatrix = newTransformationMatrix;
            secondHandle.TransformationMatrix = newTransformationMatrix;

            break;
          }

        case 3:
          {
            // make three-point transformation
            Handle firstHandle = null;
            Handle secondHandle = null;
            Handle thirdHandle = activeHandle;
            foreach (Handle h in handles)
            {
              if (!h.Equals(activeHandle))
              {
                if (firstHandle == null)
                {
                  firstHandle = h;
                }
                else
                {
                  secondHandle = h;
                  break;
                }
              }
            }
            GeneralMatrix newTransformationMatrix = LinearAlgebraUtil.CalculateTransformationMatrix(
              route.GetProjectedLocationFromParameterizedLocation(firstHandle.ParameterizedLocation, projectionOrigin),
              firstHandle.Location,
              route.GetProjectedLocationFromParameterizedLocation(secondHandle.ParameterizedLocation, projectionOrigin),
              secondHandle.Location,
              route.GetProjectedLocationFromParameterizedLocation(thirdHandle.ParameterizedLocation, projectionOrigin),
              thirdHandle.Location,
              firstHandle.TransformationMatrix
              );

            firstHandle.TransformationMatrix = newTransformationMatrix;
            secondHandle.TransformationMatrix = newTransformationMatrix;
            thirdHandle.TransformationMatrix = newTransformationMatrix;

            break;
          }

        default:
          {
            HandleCollection ah = GetAdjacentHandles(activeHandle);
            GeneralMatrix newTransformationMatrix = LinearAlgebraUtil.CalculateTransformationMatrix(
              route.GetProjectedLocationFromParameterizedLocation(ah[0].ParameterizedLocation, projectionOrigin),
              ah[0].Location,
              route.GetProjectedLocationFromParameterizedLocation(ah[1].ParameterizedLocation, projectionOrigin),
              ah[1].Location,
              route.GetProjectedLocationFromParameterizedLocation(ah[2].ParameterizedLocation, projectionOrigin),
              ah[2].Location,
              ah[0].TransformationMatrix);
            activeHandle.TransformationMatrix = newTransformationMatrix;
            break;
          }
      }
    }

    private void UpdateTransformationMatrices(Handle activeHandle)
    {
      switch (handles.Count)
      {
        case 0:
          break;

        case 1:
          {
            // perform translation of whole route
            GeneralMatrix newTransformationMatrix = (GeneralMatrix)initialTransformationMatrix.Clone();
            PointD initialProjectedLocation =
              route.GetProjectedLocationFromParameterizedLocation(activeHandle.ParameterizedLocation, projectionOrigin);
            PointD initialAdjustedLocation = LinearAlgebraUtil.Transform(initialProjectedLocation,
                                                                         initialTransformationMatrix);
            PointD translation = activeHandle.Location - initialAdjustedLocation;
            newTransformationMatrix.SetElement(0, 2, newTransformationMatrix.GetElement(0, 2) + translation.X);
            newTransformationMatrix.SetElement(1, 2, newTransformationMatrix.GetElement(1, 2) + translation.Y);
            activeHandle.TransformationMatrix = newTransformationMatrix;

            break;
          }

        default:
          {
            // perform scaling and rotation for the handle pairs that the active handle is part of
            int activeHandleIndex = handles.IndexOf(activeHandle);

            if (activeHandleIndex > 0)
            {
              Handle firstHandle = handles[activeHandleIndex - 1];
              Handle secondHandle = activeHandle;
              firstHandle.TransformationMatrix = LinearAlgebraUtil.CalculateTransformationMatrix(
                route.GetProjectedLocationFromParameterizedLocation(firstHandle.ParameterizedLocation, projectionOrigin),
                firstHandle.Location,
                route.GetProjectedLocationFromParameterizedLocation(secondHandle.ParameterizedLocation, projectionOrigin),
                secondHandle.Location,
                firstHandle.TransformationMatrix,
                true
                );
            }

            if (activeHandleIndex < handles.Count - 1)
            {
              Handle firstHandle = activeHandle;
              Handle secondHandle = handles[activeHandleIndex + 1];
              firstHandle.TransformationMatrix = LinearAlgebraUtil.CalculateTransformationMatrix(
                route.GetProjectedLocationFromParameterizedLocation(firstHandle.ParameterizedLocation, projectionOrigin),
                firstHandle.Location,
                route.GetProjectedLocationFromParameterizedLocation(secondHandle.ParameterizedLocation, projectionOrigin),
                secondHandle.Location,
                firstHandle.TransformationMatrix,
                true
                );
            }
          }
          break;
      }
    }

    /// <summary>
    /// Returns the two handles that are adjacent to the given handle.
    /// </summary>
    /// <param name="handle">The handle "in the middle"</param>
    /// <returns>A collection of three handles</returns>
    private HandleCollection GetAdjacentHandles(Handle handle)
    {
      HandleCollection list = new HandleCollection();
      int index = handles.IndexOf(handle);
      if (index <= 0) index = 1;
      if (index >= handles.Count - 1) index = handles.Count - 2;
      list.Add(handles[index - 1]);
      list.Add(handles[index]);
      list.Add(handles[index + 1]);
      return list;
    }

    private int GetLapIndexFromParameterizedLocation(ParameterizedLocation pl)
    {
      DateTime time = route.GetTimeFromParameterizedLocation(pl);
      if (laps == null || laps.Count == 0 || time < laps[0].Time) return 0;
      for (int i = 1; i < laps.Count - 1; i++)
      {
        if (laps[i].Time >= time) return i - 1;
      }
      return laps.Count - 2;
    }

    private static List<RouteLineVertex> FilterVertices(IList<RouteLineVertex> vertices)
    {
      var filteredVertices = new List<RouteLineVertex>();
      for (int i = 0; i < vertices.Count; i++)
      {
        if (i % 1 == 0) filteredVertices.Add(vertices[i]);
      }
      return filteredVertices;
    }

    private List<RouteLineVertex> CreateRouteLineVertices(ParameterizedLocation startPL,
                                                                 ParameterizedLocation endPL, WaypointAttribute colorCodingAttribute)
    {
      // Points to use:
      //   1. Points in adjusted route
      //   2. Start and end points
      //   3. Points where alpha adjustment changes
      // Don't insert points from category 2 if there already is a point with the same PL in category 1
      // Don't insert points from category 3 if there already is a point with the same PL in category 1 or 2

      var adjustedWaypoints = new List<AdjustedWaypoint>();

      //   1. Points in adjusted route
      for (int i = startPL.SegmentIndex; i <= endPL.SegmentIndex; i++)
      {
        AdjustedRouteSegment ars = AdjustedRoute.Segments[i];
        for (int j = 0; j < ars.Waypoints.Count; j++)
        {
          AdjustedWaypoint aw = ars.Waypoints[j];
          if (aw.ParameterizedLocation >= startPL && aw.ParameterizedLocation <= endPL)
          {
            adjustedWaypoints.Add(aw);
          }
        }
      }

      //   2. Start and end points
      AdjustedWaypoint startWaypoint = AdjustedRoute.CreateWaypointFromParameterizedLocation(startPL,
                                                                                                     AdjustedWaypoint.
                                                                                                       AdjustedWaypointType
                                                                                                       .Start);
      AdjustedWaypoint endWaypoint = AdjustedRoute.CreateWaypointFromParameterizedLocation(startPL,
                                                                                                   AdjustedWaypoint.
                                                                                                     AdjustedWaypointType
                                                                                                     .End);
      adjustedWaypoints.Add(startWaypoint);
      adjustedWaypoints.Add(endWaypoint);

      //   3. Points where alpha adjustment changes
      if (AlphaAdjustmentChanges != null)
      {
        foreach (AlphaAdjustmentChange aac in AlphaAdjustmentChanges)
        {
          AdjustedWaypoint w = AdjustedRoute.CreateWaypointFromParameterizedLocation(aac.ParameterizedLocation,
                                                                                             AdjustedWaypoint.
                                                                                               AdjustedWaypointType.
                                                                                               AlphaAdjustmentChange);
          adjustedWaypoints.Add(w);
        }
      }
      adjustedWaypoints.Sort();

      // Don't insert points from category 2 if there already is a point with the same PL in category 1
      // Don't insert points from category 3 if there already is a point with the same PL in category 1 or 2
      for (int i = adjustedWaypoints.Count - 1; i >= 1; i--)
      {
        if (adjustedWaypoints[i].ParameterizedLocation == adjustedWaypoints[i - 1].ParameterizedLocation)
          adjustedWaypoints.RemoveAt(i);
      }

      // create route line vertices
      var vertices = new List<RouteLineVertex>();

      double currentAlphaAdjustment = 0;
      int currentAlphaAdjustmentIndex = -1;
      foreach (AdjustedWaypoint aw in adjustedWaypoints)
      {
        while (AlphaAdjustmentChanges != null &&
               currentAlphaAdjustmentIndex + 1 < AlphaAdjustmentChanges.Count &&
               AlphaAdjustmentChanges[currentAlphaAdjustmentIndex + 1].ParameterizedLocation <=
               aw.ParameterizedLocation)
        {
          currentAlphaAdjustmentIndex++;
          currentAlphaAdjustment = AlphaAdjustmentChanges[currentAlphaAdjustmentIndex].AlphaAdjustment;
        }
        var vertex = new RouteLineVertex();
        vertex.Location = aw.Location;
        vertex.Color = GetColorFromParameterizedLocation(aw.ParameterizedLocation, colorCodingAttribute);
        vertex.AlphaAdjustment = currentAlphaAdjustment;
        vertex.ParameterizedLocation = aw.ParameterizedLocation;
        vertices.Add(vertex);
      }
      return vertices;
    }
    #endregion

    #region Nested type: RouteLineVertex

    private class RouteLineVertex
    {
      private double alphaAdjustment;

      public PointD Location { get; set; }
      public Color Color { get; set; }
      public ParameterizedLocation ParameterizedLocation { get; set; }

      public double AlphaAdjustment
      {
        get { return alphaAdjustment; }
        set
        {
          if (value < -1 || value > 1)
            throw new ArgumentOutOfRangeException("Alpha adjustment must be between -1 and 1, inclusive.");
          alphaAdjustment = value;
        }
      }
    }

    #endregion

    #region Nested type: RouteLine

    private class RouteLine
    {
      private readonly GraphicsPath linePath;
      private readonly GraphicsPath maskPath;

      public RouteLine(RouteLineVertex startVertex, RouteLineVertex endVertex, RouteLineVertex previousVertex,
                       RouteLineVertex nextVertex, double lineWidth, double maskWidth, double zoom)
      {
        PointD p0 = zoom * (previousVertex ?? startVertex).Location;
        PointD p1 = zoom * startVertex.Location;
        PointD p2 = zoom * endVertex.Location;
        PointD p3 = zoom * (nextVertex ?? endVertex).Location;

        PointD directionVector = LinearAlgebraUtil.Normalize(p1 - p0);
        if (p0.X == p1.X && p0.Y == p1.Y)
        {
          p0 = p1 - directionVector;
        }

        if (p3.X == p2.X && p3.Y == p2.Y)
        {
          p3 = p2 + directionVector;
        }

        linePath = CreatePath(p1, p2, p1 - p0, p3 - p2, zoom * lineWidth / 2, -zoom * lineWidth / 2, previousVertex == null,
                              nextVertex == null);
        if (zoom * maskWidth > 0)
        {
          GraphicsPath leftMaskPath = CreatePath(p1, p2, p1 - p0, p3 - p2, zoom * (lineWidth / 2 + maskWidth),
                                                 zoom * lineWidth / 2, previousVertex == null, nextVertex == null);
          GraphicsPath rightMaskPath = CreatePath(p1, p2, p1 - p0, p3 - p2, zoom * (-lineWidth / 2 - maskWidth),
                                                  zoom * -lineWidth / 2, previousVertex == null, nextVertex == null);
          maskPath = new GraphicsPath(FillMode.Winding);
          maskPath.AddPath(leftMaskPath, false);
          maskPath.AddPath(rightMaskPath, false);
        }
      }

      public GraphicsPath LinePath
      {
        get { return linePath; }
      }

      public GraphicsPath MaskPath
      {
        get { return maskPath; }
      }

      // TODO: add support for bool isStartOfLine, bool isEndOfLine
      private static GraphicsPath CreatePath(PointD p1, PointD p2, PointD d0, PointD d2, double t0, double t1,
                                             bool isStartOfLine, bool isEndOfLine)
      {
        const double EPSILON = 0.000001;
        PointD d1 = p2 - p1;
        PointD n0 = LinearAlgebraUtil.GetNormalVector(d0);
        PointD n1 = LinearAlgebraUtil.GetNormalVector(d1);
        PointD n2 = LinearAlgebraUtil.GetNormalVector(d2);
        PointD c0 = LinearAlgebraUtil.GetNormalVector(LinearAlgebraUtil.Normalize(d0) + LinearAlgebraUtil.Normalize(d1));
        PointD c1 = LinearAlgebraUtil.GetNormalVector(LinearAlgebraUtil.Normalize(d1) + LinearAlgebraUtil.Normalize(d2));
        bool parallell0 = (Math.Abs(LinearAlgebraUtil.GetAngleR(d0) - LinearAlgebraUtil.GetAngleR(d1)) < EPSILON);
        bool parallell1 = (Math.Abs(LinearAlgebraUtil.GetAngleR(d1) - LinearAlgebraUtil.GetAngleR(d2)) < EPSILON);

        PointD start;
        PointD end;
        PointD previousPoint;

        // t0
        PointD q00e = p1 + t0 * n0;
        PointD q01s = p1 + t0 * n1;
        PointD q01e = p2 + t0 * n1;
        PointD q02s = p2 + t0 * n2;

        // t1
        PointD q10e = p1 + t1 * n0;
        PointD q11s = p1 + t1 * n1;
        PointD q11e = p2 + t1 * n1;
        PointD q12s = p2 + t1 * n2;

        var path = new GraphicsPath(FillMode.Winding);

        double i00t;
        bool i00 = LinearAlgebraUtil.LineInfiniteLineIntersect(q01s, q01e, q00e, d0, out i00t);
        double i01t;
        bool i01 = LinearAlgebraUtil.LineInfiniteLineIntersect(q01s, q01e, q02s, d2, out i01t);
        double i10t;
        bool i10 = LinearAlgebraUtil.LineInfiniteLineIntersect(q11s, q11e, q10e, d0, out i10t);
        double i11t;
        bool i11 = LinearAlgebraUtil.LineInfiniteLineIntersect(q11s, q11e, q12s, d2, out i11t);

        if (i00t < 0 && Math.Abs(t0) > 0)
        {
          var startAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(t0 * c0));
          var sweepAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(c0, n1));
          path.AddArc(
            new RectangleF((float)(p1.X - Math.Abs(t0)), (float)(p1.Y - Math.Abs(t0)), (float)(2 * Math.Abs(t0)),
                           (float)(2 * Math.Abs(t0))),
            startAngle,
            sweepAngle
            );
        }

        if (parallell0) start = q01s;
        else if (i00) start = q01s + i00t * (q01e - q01s);
        //else if (i00t > 1) start = LinearAlgebraUtil.InfiniteLinesIntersectionPoint(q01s, d1, P0, t0 * n1);
        else start = q01s;

        if (parallell1) end = q01e;
        else if (i01) end = q01s + i01t * (q01e - q01s);
        //else if (i01t < 0) end = LinearAlgebraUtil.InfiniteLinesIntersectionPoint(p2, t0 * c1, p1, t0 * c0);
        else end = q01e;

        path.AddLine((PointF)start, (PointF)end);

        if (i01t > 1 && Math.Abs(t0) > 0)
        {
          var startAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(t0 * n1));
          var sweepAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(n1, c1));
          path.AddArc(
            new RectangleF((float)(p2.X - Math.Abs(t0)), (float)(p2.Y - Math.Abs(t0)), (float)(2 * Math.Abs(t0)),
                           (float)(2 * Math.Abs(t0))),
            startAngle,
            sweepAngle
            );
          previousPoint = p2 + t0 * c1;
        }
        else
        {
          previousPoint = end;
        }

        if (Math.Sign(t0) != Math.Sign(t1) && t0 != 0 && t1 != 0)
        {
          path.AddLine((PointF)previousPoint, (PointF)p2);
        }

        if (!parallell0 && i10) start = q11s + i10t * (q11e - q11s);
        else start = q11s;

        if (!parallell1 && i11) end = q11s + i11t * (q11e - q11s);
        else end = q11e;

        if (i11t > 1 && Math.Abs(t1) > 0)
        {
          var startAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(t1 * c1));
          var sweepAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(c1, n1));
          path.AddArc(
            new RectangleF((float)(p2.X - Math.Abs(t1)), (float)(p2.Y - Math.Abs(t1)), (float)(2 * Math.Abs(t1)),
                           (float)(2 * Math.Abs(t1))),
            startAngle,
            sweepAngle
            );
        }

        path.AddLine((PointF)end, (PointF)start);

        if (i10t < 0 && Math.Abs(t1) > 0)
        {
          var startAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(t1 * n1));
          var sweepAngle = (float)(180 / Math.PI * LinearAlgebraUtil.GetAngleR(n1, c0));
          path.AddArc(
            new RectangleF((float)(p1.X - Math.Abs(t1)), (float)(p1.Y - Math.Abs(t1)), (float)(2 * Math.Abs(t1)),
                           (float)(2 * Math.Abs(t1))),
            startAngle,
            sweepAngle
            );
          previousPoint = p1 + t1 * c0;
        }
        else
        {
          previousPoint = start;
        }
        if (Math.Sign(t0) != Math.Sign(t1) && t0 != 0 && t1 != 0)
        {
          path.AddLine((PointF)previousPoint, (PointF)p1);
        }

        //path.AddEllipse(new RectangleF((float)(p1.X - Math.Abs(t1 / 5)), (float)(p1.Y - Math.Abs(t1 / 5)), (float)(2 * Math.Abs(t1 / 5)), (float)(2 * Math.Abs(t1 / 5))));
        //path.AddEllipse(new RectangleF((float)(p2.X - Math.Abs(t1 / 5)), (float)(p2.Y - Math.Abs(t1 / 5)), (float)(2 * Math.Abs(t1 / 5)), (float)(2 * Math.Abs(t1 / 5))));

        return path;
      }
    }

    #endregion


    private class PLWithKey : ParameterizedLocation, IComparable<PLWithKey>
    {
      private PLKey key;
      public PLWithKey(PLKey key, ParameterizedLocation pl)
        : base(pl.SegmentIndex, pl.Value)
      {
        this.key = key;
      }

      public PLKey Key
      {
        get { return key; }
        set { key = value; }
      }

      #region IComparable<PLWithKey> Members

      public int CompareTo(PLWithKey other)
      {
        int value = base.CompareTo(other);
        if (value == 0)
        {
          return key.CompareTo(other.Key);
        }
        else
        {
          return value;
        }
      }

      #endregion
    }

    private enum PLKey
    {
      Handle,
      Waypoint
    }

    public void InsertIdleTime(DateTime time, TimeSpan duration)
    {
      if (duration.TotalSeconds < 0) throw new ArgumentException("The duration parameter must be positive.");
      if (time < route.FirstWaypoint.Time)
      {
        AddTimeOffset(duration);
      }
      else if (time < route.LastWaypoint.Time)
      {
        // add offset to waypoints
        var originalPL = route.GetParameterizedLocationFromTime(time).Ceiling();
        var pl = new ParameterizedLocation(originalPL);
        while(pl != null)
        {
          route.Segments[pl.SegmentIndex].Waypoints[(int) pl.Value].Time += duration;
          pl = route.GetNextPLNode(pl, ParameterizedLocation.Direction.Forward);
        }
        // insert "freezing" waypoint just before first moved waypoint to keep 
        var waypointIndex = (int)originalPL.Value - 1;
        if (waypointIndex > 0)
        {
          // use clone of previous waypoint as freezing waypoint
          var freezingWaypoint = route.Segments[originalPL.SegmentIndex].Waypoints[waypointIndex - 1].Clone();
          var firstMovedWaypoint = route.Segments[originalPL.SegmentIndex].Waypoints[waypointIndex];
          freezingWaypoint.Time = firstMovedWaypoint.Time.AddMilliseconds(-1); // insert one millisecond before
          route.Segments[originalPL.SegmentIndex].Waypoints.Insert(waypointIndex, freezingWaypoint);
        }

        // add offset to laps
        foreach(var lap in laps)
        {
          if (lap.Time >= time) lap.Time += duration;
        }
      }
      SetLapTimesToRoute();
    }
  }

  [Serializable]
  public class SessionInfo
  {
    private SessionPerson person = new SessionPerson();
    private string description = "";

    public SessionPerson Person
    {
      get { return person; }
      set { person = value; }
    }

    public string Description
    {
      get { return description; }
      set { description = value; }
    }
  }
  [Serializable]
  public class SessionPerson
  {
    public SessionPerson()
    {
      Name = "";
      Club = "";
    }
    public string Name { get; set; }
    public string Club { get; set; }
    public uint Id { get; set; }

    public override string ToString()
    {
      return Name + (!String.IsNullOrEmpty(Club) ? ", " + Club : "");
    }
  }
}
