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
    }

    #region ICollection<Lap> Members

    public void Add(Lap item)
    {
      item.Time = item.Time.ToUniversalTime();
      laps.Add(item);
    }

    public void AddRange(IEnumerable<Lap> items)
    {
      foreach(var item in items)
      {
        item.Time = item.Time.ToUniversalTime();
        laps.Add(item);
      }
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
      // a bug in developer version caused wrong sorting of laps in some cases
      // this code handles the issue (which should be really rare)
      var utcLaps = new List<Lap>();
      foreach (var lap in this)
      {
        lap.Time = lap.Time.ToUniversalTime();
        utcLaps.Add(lap);
      }
      Clear();

      laps.AddMany(utcLaps);
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

    public bool Exists(DateTime time, int numberOfTimes = 1)
    {
      var count = 0;
      foreach (var lap in this)
      {
        if (lap.Time == time) count++;
      }
      return count >= numberOfTimes;
    }
  }
}
