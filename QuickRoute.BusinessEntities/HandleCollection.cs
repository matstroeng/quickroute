using System;
using System.Collections;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class HandleCollection : ICollection<Handle>
  {
    private OrderedBag<Handle> handles = new OrderedBag<Handle>();

    public Handle this[int index]
    {
      get { return handles[index]; }
      set
      {
        Handle h = handles[index];
        h = value;
      }
    }

    #region ICollection<Handle> Members

    public void Add(Handle item)
    {
      handles.Add(item);
    }

    public void Clear()
    {
      handles.Clear();
    }

    public bool Contains(Handle item)
    {
      return handles.Contains(item);
    }

    public void CopyTo(Handle[] array, int arrayIndex)
    {
      handles.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return handles.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(Handle item)
    {
      return handles.Remove(item);
    }

    #endregion

    #region IEnumerable<Handle> Members

    public IEnumerator<Handle> GetEnumerator()
    {
      return handles.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return handles.GetEnumerator();
    }

    #endregion

    public int IndexOf(Handle handle)
    {
      for (int i = 0; i < this.Count; i++)
      {
        if(this[i].Equals(handle)) return i;
      }
      return -1;
    }

    public CutHandlesData Cut(ParameterizedLocation parameterizedLocation, CutType cutType)
    {
      CutHandlesData cutHandlesData = new CutHandlesData();
      cutHandlesData.CutParamaterizedLocation = parameterizedLocation;
      cutHandlesData.CutType = cutType;
      switch (cutType)
      {
        case CutType.Before:
          foreach (Handle h in this)
          {
            if (h.ParameterizedLocation < parameterizedLocation)
            {
              cutHandlesData.CutHandles.Add(h);
            }
          }
          foreach (Handle h in cutHandlesData.CutHandles)
          {
            Remove(h);
          }
          foreach (Handle h in this)
          {
            h.ParameterizedLocation -= parameterizedLocation;
          }
          break;

        case CutType.After:
          foreach (Handle h in this)
          {
            if (h.ParameterizedLocation > parameterizedLocation)
            {
              cutHandlesData.CutHandles.Add(h);
            }
          }
          foreach (Handle h in cutHandlesData.CutHandles)
          {
            Remove(h);
          }
          break;
      }
      return cutHandlesData;
    }

    public void UnCut(CutHandlesData cutHandlesData)
    {
      switch (cutHandlesData.CutType)
      {
        case CutType.Before:
          foreach (Handle h in this)
          {
            if (h.ParameterizedLocation.SegmentIndex == 0)
            {
              h.ParameterizedLocation.Value += cutHandlesData.CutParamaterizedLocation.Value;
            }
            h.ParameterizedLocation.SegmentIndex += cutHandlesData.CutParamaterizedLocation.SegmentIndex;
          }
          foreach (Handle h in cutHandlesData.CutHandles)
          {
            Add(h);
          }
          break;

        case CutType.After:
          foreach (Handle h in cutHandlesData.CutHandles)
          {
            Add(h);
          }
          break;
      }
    }

    public class CutHandlesData
    {
      private List<Handle> cutHandles = new List<Handle>();
      private ParameterizedLocation cutParamaterizedLocation;
      private CutType cutType;

      public List<Handle> CutHandles
      {
        get { return cutHandles; }
        set { cutHandles = value; }
      }

      public ParameterizedLocation CutParamaterizedLocation
      {
        get { return cutParamaterizedLocation; }
        set { cutParamaterizedLocation = value; }
      }

      public CutType CutType
      {
        get { return cutType; }
        set { cutType = value; }
      }
    }

  }
}
