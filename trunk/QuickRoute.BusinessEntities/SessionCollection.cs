using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class SessionCollection : ICollection<Session>
  {
    private List<Session> sessions = new List<Session>();

    public Session this[int index]
    {
      get { return sessions[index]; }
      set
      {
        sessions[index] = value;
      }
    }

    /// <summary>
    /// Using linear least squares algorithm described at http://en.wikipedia.org/wiki/Linear_least_squares
    /// </summary>
    /// <returns></returns>
    public GeneralMatrix CalculateAverageTransformationMatrix()
    {
      if (Count == 0) return null;
      var n = 4;
      var XtX = new GeneralMatrix(n, n);
      var Xty = new GeneralMatrix(n, 1);
      var numberOfUnknowns = 0;

      foreach (var session in this)
      {
        var m = session.Handles.Length;
        if (m < 2) continue;
        numberOfUnknowns += m;

        var startDistance = session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, session.Route.FirstPL).Value;
        for (var i = 0; i < m; i++)
        {
          var longLat = session.Route.GetLocationFromParameterizedLocation(session.Handles[i].ParameterizedLocation);
          var p = longLat.Project(session.ProjectionOrigin); // projected point on earth (metres)
          var q = session.Handles[i].Location; // point on map image (pixels)
          var endDistance = (i != m - 1)
                              ? (session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, session.Handles[i].ParameterizedLocation).Value +
                                 session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, session.Handles[i + 1].ParameterizedLocation).Value ) / 2
                              : session.Route.GetAttributeFromParameterizedLocation(WaypointAttribute.Distance, session.Route.LastPL).Value;
          var w = endDistance - startDistance; // weight
          startDistance = endDistance;

          XtX.SetElement(0, 0, XtX.GetElement(0, 0) + w * (p.X * p.X + p.Y * p.Y));
          XtX.SetElement(0, 2, XtX.GetElement(0, 2) + w * p.X);
          XtX.SetElement(0, 3, XtX.GetElement(0, 3) - w * p.Y);
          XtX.SetElement(1, 1, XtX.GetElement(1, 1) + w * (p.X * p.X + p.Y * p.Y));
          XtX.SetElement(1, 2, XtX.GetElement(1, 2) + w * p.Y);
          XtX.SetElement(1, 3, XtX.GetElement(1, 3) + w * p.X);
          XtX.SetElement(2, 0, XtX.GetElement(2, 0) + w * p.X);
          XtX.SetElement(2, 1, XtX.GetElement(2, 1) + w * p.Y);
          XtX.SetElement(2, 2, XtX.GetElement(2, 2) + w);
          XtX.SetElement(3, 0, XtX.GetElement(3, 0) - w * p.Y);
          XtX.SetElement(3, 1, XtX.GetElement(3, 1) + w * p.X);
          XtX.SetElement(3, 3, XtX.GetElement(3, 3) + w);

          Xty.SetElement(0, 0, Xty.GetElement(0, 0) + w * (q.X * p.X - q.Y * p.Y));
          Xty.SetElement(1, 0, Xty.GetElement(1, 0) + w * (q.X * p.Y + q.Y * p.X));
          Xty.SetElement(2, 0, Xty.GetElement(2, 0) + w * q.X);
          Xty.SetElement(3, 0, Xty.GetElement(3, 0) + w * q.Y);
        }
      }

      if (numberOfUnknowns == 0) return this[0].InitialTransformationMatrix;
      if (numberOfUnknowns == 1) return this[0].Handles[0].TransformationMatrix;

      var B = XtX.QRD().Solve(Xty);

      var T = new GeneralMatrix(3, 3);

      T.SetElement(0, 0, B.GetElement(0, 0));
      T.SetElement(0, 1, B.GetElement(1, 0));
      T.SetElement(0, 2, B.GetElement(2, 0));
      T.SetElement(1, 0, B.GetElement(1, 0));
      T.SetElement(1, 1, -B.GetElement(0, 0));
      T.SetElement(1, 2, B.GetElement(3, 0));
      T.SetElement(2, 0, 0);
      T.SetElement(2, 1, 0);
      T.SetElement(2, 2, 1);
      return T;
    }

    /// <summary>
    /// Creates a shallow copy.
    /// </summary>
    /// <returns></returns>
    public SessionCollection Copy()
    {
      var copy = new SessionCollection();
      foreach (var s in sessions)
      {
        copy.Add(s);
      }
      return copy;
    }

    #region ICollection<Session> Members

    public void Add(Session item)
    {
      sessions.Add(item);
    }

    public void Clear()
    {
      sessions.Clear();
    }

    public bool Contains(Session item)
    {
      return sessions.Contains(item);
    }

    public void CopyTo(Session[] array, int arrayIndex)
    {
      sessions.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return sessions.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(Session item)
    {
      return sessions.Remove(item);
    }

    #endregion

    #region IEnumerable<Session> Members

    public IEnumerator<Session> GetEnumerator()
    {
      return sessions.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return sessions.GetEnumerator();
    }

    #endregion
  }

}
