using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ObjectDictionary
{
    Dictionary<string, GameObject> _dict = new Dictionary<string, GameObject>();

    public List<string> list1 = new List<string>();
    public List<GameObject> list2 = new List<GameObject>();

    public int Count
    {
        get
        {
            //if (Base.Interfaces.AppContext.IsWebGL)
            //{
            //    return list1.Count;
            //}
            //else
            //{
                return _dict.Count;
            //}
        }
    }

    public void Add(string key,GameObject value)
    {
        
        //if (Base.Interfaces.AppContext.IsWebGL)
        //{
        //    list1.Add(key);
        //    list2.Add(value);
        //}
        //else
        //{
            if (_dict.ContainsKey(key))
            {
                _dict[key] = value;
            }
            else
            {
                _dict.Add(key, value);
            }
        //}
    }

    public bool ContainsKey(string key)
    {
        //if (Base.Interfaces.AppContext.IsWebGL)
        //{
        //    return list1.Contains(key);
        //}
        //else
        //{
            return _dict.ContainsKey(key);
        //}
    }

    public GameObject Get(string key)
    {
        //if (Base.Interfaces.AppContext.IsWebGL)
        //{
        //    int id = list1.IndexOf(key);
        //    if (id != -1)
        //    {
        //        return list2[id];
        //    }
        //}
        //else
        //{
            if (_dict.ContainsKey(key))
                return _dict[key];
        //}

        return null;
    }

    public void Clear()
    {
        _dict.Clear();
        list1.Clear();
        list2.Clear();
    }
}
