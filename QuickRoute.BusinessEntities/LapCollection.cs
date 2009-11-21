using System;
using System.Collections;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace QuickRoute.BusinessEntities
{
  [Serializable]
  public class LapCollection : ICollection<Lap>
  {
    private readonly OrderedBag<Lap> laps = new OrderedBag<Lap>();

    public Lap this[int index]
    {
      get { return laps[index]; }
      set 
      { 
        var lap = laps[index];
        lap = value; 
      }
    }

    #region ICollection<Lap> Members

    public void Add(Lap item)
    {
      laps.Add(item);
    }

    public void AddRange(IEnumerable<Lap> items)
    {
      laps.AddMany(items);
    }

    public void Clear()
    {
      laps.Clear();
    }

    public bool Contains(Lap item)
    {
      return laps.Contains(item);
    }

    public void CopyTo(Lap[] array, int arrayIndex)
    {
      laps.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return laps.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(Lap item)
    {
      return laps.Remove(item);
    }

    #endregion

    #region IEnumerable<Lap> Members

    public IEnumerator<Lap> GetEnumerator()
    {
      return laps.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return laps.GetEnumerator();
    }

    #endregion

    public void EnsureUtcTimes()
    {
      foreach (var lap in this)
      {
        lap.Time = lap.Time.ToUniversalTime();
      }
    }

    public CutLapsData Cut(DateTime time, CutType cutType)
    {
      var cutLapsData = new CutLapsData();

      switch (cutType)
      {
        case CutType.Before:
          foreach (var l in this)
          {
            if (l.Time <= time)
            {
              cutLapsData.CutLaps.Add(l);
            }
          }
          foreach (var l in cutLapsData.CutLaps)
          {
            Remove(l);
          }
          if (cutLapsData.CutLaps.Count > 0)
          {
            var newLap = new Lap(time, LapType.Start);
            Add(newLap);
            cutLapsData.AddedLaps.Add(newLap);
          }
          break;

        case CutType.After:
          foreach (var l in this)
          {
            if (l.Time >= time)
            {
              cutLapsData.CutLaps.Add(l);
            }
          }
          foreach (var l in cutLapsData.CutLaps)
          {
            Remove(l);
          }
          if (cutLapsData.CutLaps.Count > 0)
          {
            var newLap = new Lap(time, LapType.Stop);
            Add(newLap);
            cutLapsData.AddedLaps.Add(newLap);
          }
          break;
      }
      return cutLapsData;
    }

    public void UnCut(CutLapsData cutLapsData)
    {
      foreach (Lap l in cutLapsData.CutLaps)
      {
        Add(l);
      }
      foreach (Lap l in cutLapsData.AddedLaps)
      {
        Remove(l);
      }
    }

    public void AddTimeOffset(TimeSpan offset)
    {
      foreach (var lap in this)
      {
        lap.Time = lap.Time.Add(offset);
      }
    }

    public class CutLapsData
    {
      private List<Lap> cutLaps = new List<Lap>();
      private List<Lap> addedLaps = new List<Lap>();

      public List<Lap> CutLaps
      {
        get { return cutLaps; }
        set { cutLaps = value; }
      }

      public List<Lap> AddedLaps
      {
        get { return addedLaps; }
        set { addedLaps = value; }
      }

    }
  }
}
