using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key2List<T1, T2> : IComparable<Key2List<T1, T2>>
{
    public T1 Key;
    public List<T2> List;

    public int Count
    {
        get
        {
            if (List == null) return -1;
            return List.Count;
        }
    }

    public Key2List(T1 key, List<T2> list)
    {
        this.Key = key;
        this.List = list;
        //this.Count = list.Count;
    }

    public int CompareTo(Key2List<T1, T2> other)
    {
        return other.Count.CompareTo(this.Count);
    }
}