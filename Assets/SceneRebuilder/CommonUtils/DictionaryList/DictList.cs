using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictList<T>
    //:List<T>
{

    public DictList()
    {

    }

    public DictList(IEnumerable<T> items)
    {
        //Items.AddRange(items);
        this.AddRange(items);
    }

    public List<T> Items = new List<T>();

    private Dictionary<T, T> dict = new Dictionary<T, T>();

    public T[] ToArray()
    {
        return Items.ToArray();
    }

    public T this[int index]
    {
        get
        {
            return Items[index];
        }
        set
        {
            Items[index] = value;
        }
    }

    public int Count
    {
        get
        {
            return Items.Count;
        }
    }

    public void Clear()
    {
        Items.Clear();
        dict.Clear();
    }

    public bool Contains(T t)
    {
        return dict.ContainsKey(t);
        //return Items.Contains(t);
    }

    public void Add(T t)
    {
        if (!dict.ContainsKey(t))
        {
            dict.Add(t, t);
            Items.Add(t);
        }
        else
        {

        }
    }

    public bool Remove(T t)
    {
        if (dict.ContainsKey(t))
        {
            dict.Remove(t);
            return Items.Remove(t);
        }
        else
        {
            return false;
        }
    }

    public void RemoveAt(int id)
    {
        try
        {
            if (id >= 0 && id < Items.Count)
            {
                T t = Items[id];
                if (dict.ContainsKey(t))
                {
                    dict.Remove(t);
                }
                Items.RemoveAt(id);
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"RemoveAt Exception:{ex}");
        }

    }

    public List<T> NewList()
    {
        return new List<T>(Items);
    }

    public void AddRange(IEnumerable<T> ss)
    {
        if (ss == null) return;
        foreach(var s in ss)
        {
            if (s == null) continue;
            Add(s);
        }
    }
    //public void AddRange(T[] ss)
    //{
    //    foreach (var s in ss)
    //    {
    //        if (s == null) continue;
    //        Add(s);
    //    }
    //}
}
