using System;
using System.Runtime.Serialization;

namespace QuickRoute.BusinessEntities
{
  /// <summary>
  /// Class holding information of the handles used for adjusting the route to the map.
  /// </summary>
  [Serializable]
  public class Handle : IComparable<Handle>
  {
    private GeneralMatrix transformationMatrix;
    private ParameterizedLocation parameterizedLocation;
    private PointD location;
    private IMarkerDrawer markerDrawer;
    private HandleType type;
    /// <summary>
    /// The exact time this handle is bound to (used to prevent rounding errors when e g cutting a route) 
    /// </summary>
    [NonSerialized] private DateTime? time;

    public Handle()
    {
    }

    /// <summary>
    /// Creates a handle based on parameterized location.
    /// </summary>
    /// <param name="parameterizedLocation"></param>
    /// <param name="location"></param>
    /// <param name="transformationMatrix"></param>
    /// <param name="markerDrawer"></param>
    public Handle(ParameterizedLocation parameterizedLocation, PointD location, GeneralMatrix transformationMatrix,
                  IMarkerDrawer markerDrawer)
      : this(parameterizedLocation, location, transformationMatrix, markerDrawer, HandleType.Handle)
    {
    }

    /// <summary>
    /// Creates a handle based on parameterized location.
    /// </summary>
    /// <param name="parameterizedLocation"></param>
    /// <param name="location"></param>
    /// <param name="transformationMatrix"></param>
    /// <param name="markerDrawer"></param>
    /// <param name="type"></param>
    public Handle(ParameterizedLocation parameterizedLocation, PointD location, GeneralMatrix transformationMatrix, IMarkerDrawer markerDrawer, HandleType type)
      : this(parameterizedLocation, null, location, transformationMatrix, markerDrawer, HandleType.Handle)
    {
    }

    /// <summary>
    /// Creates a handle based on parameterized location and time (used to prevent rounding errors when e g cutting a route).
    /// </summary>
    /// <param name="parameterizedLocation"></param>
    /// <param name="time"></param>
    /// <param name="location"></param>
    /// <param name="transformationMatrix"></param>
    /// <param name="markerDrawer"></param>
    public Handle(ParameterizedLocation parameterizedLocation, DateTime? time, PointD location, GeneralMatrix transformationMatrix, IMarkerDrawer markerDrawer)
      : this(parameterizedLocation, time, location, transformationMatrix, markerDrawer, HandleType.Handle)
    {
    }

    /// <summary>
    /// Creates a handle based on parameterized location and time (used to prevent rounding errors when e g cutting a route).
    /// </summary>
    /// <param name="parameterizedLocation"></param>
    /// <param name="time"></param>
    /// <param name="location"></param>
    /// <param name="transformationMatrix"></param>
    /// <param name="markerDrawer"></param>
    /// <param name="type"></param>
    public Handle(ParameterizedLocation parameterizedLocation, DateTime? time, PointD location, GeneralMatrix transformationMatrix, IMarkerDrawer markerDrawer, HandleType type)
    {
      ParameterizedLocation = parameterizedLocation;
      Time = time;
      Location = location;
      TransformationMatrix = transformationMatrix;
      MarkerDrawer = markerDrawer;
      Type = type;
    }

    public GeneralMatrix TransformationMatrix
    {
      get { return transformationMatrix; }
      set { transformationMatrix = value; }
    }

    /// <summary>
    /// The parameterized location of this handle.
    /// </summary>
    public ParameterizedLocation ParameterizedLocation
    {
      get { return parameterizedLocation; }
      set { parameterizedLocation = value; }
    }

    /// <summary>
    /// Location of the handle in original map coordinates.
    /// </summary>
    public PointD Location
    {
      get { return location; }
      set { location = value; }
    }

    /// <summary>
    /// The marker drawer that is used when drawing the handle marker.
    /// </summary>
    public IMarkerDrawer MarkerDrawer
    {
      get { return markerDrawer; }
      set { markerDrawer = value; }
    }

    public HandleType Type
    {
      get { return type; }
      set { type = value; }
    }

    public DateTime? Time
    {
      get { return time; }
      set { time = value; }
    }

    #region IComparable<Handle> Members

    public int CompareTo(Handle other)
    {
      return ParameterizedLocation.CompareTo(other.ParameterizedLocation); 
    }

    #endregion

    public enum HandleType
    {
      StartHandle = -1,
      Handle,
      EndHandle
    }

  }

}
