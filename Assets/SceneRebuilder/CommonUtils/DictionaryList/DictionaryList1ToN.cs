using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryList1ToN<T1, T2> : Dictionary<T1, List<T2>> //where T2 : class
{
    public List<T2> GetList(T1 key)
    {
        if (this.ContainsKey(key))
        {
            return this[key];
        }
        else
        {
            return new List<T2>();
        }
    }

    public int GetListCount(T1 key)
    {
        if (this.ContainsKey(key))
        {
            return this[key].Count;
        }
        else
        {
            return 0;
        }
    }

    public List<Key2List<T1, T2>> Key2Lists = new List<Key2List<T1, T2>>();

    public void AddItem(T1 key, T2 item)
    {

        if (!this.ContainsKey(key))
        {
            List<T2> list0 = new List<T2>();
            this.Add(key, list0);
            Key2Lists.Add(new Key2List<T1, T2>(key, list0));
        }
        var list = this[key];
        if (!list.Contains(item))
            list.Add(item);

        //if (item.ToString().Contains("SG0000-Undefined 102") || item.ToString().Contains("FD0000-Undefined 105"))
        //{
        //    Debug.LogError($"AddItem item:{item},key:{key} count:{list.Count}");
        //}
    }

    public void RemoveItem(T1 key, T2 item)
    {
        var items = GetItems(key);
        if (items != null)
        {
            int count1 = items.Count;
            items.Remove(item);
            int count2 = items.Count;
            if (count1 == count2)
            {
                Debug.LogError($"DictionaryList1ToN RemoveItem NotContainsItem key:{key} item:{item}");
            }

            if (count2 == 0)
            {
                this.Remove(key);
            }
        }
        else
        {
            Debug.LogError($"DictionaryList1ToN RemoveItem NotContainsKey key:{key} item:{item}");
        }

        //if (item.ToString().Contains("SG0000-Undefined 102") || item.ToString().Contains("FD0000-Undefined 105"))
        //{
        //    Debug.LogError($"RemoveItem item:{item},key:{key} count:{items.Count}");
        //}
    }

    public T2 GetItem(T1 key)
    {
        if (this.ContainsKey(key))
        {
            var list = this[key];
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return default(T2);
            }
        }
        return default(T2);
    }

    public List<T2> GetItems(T1 key)
    {
        if (this.ContainsKey(key))
        {
            var list = this[key];
            return list;
        }
        return null;
    }

    public List<Key2List<T1, T2>> GetListSortByCount()
    {
        Key2Lists.Sort();
        //List<T1> keys = new List<T1>();
        //foreach(var item in Key2Lists)
        //{
        //    keys.Add(item.Key);
        //}
        //return keys;
        return Key2Lists;
    }
}

public class DictionaryList1ToN<T> : DictionaryList1ToN<string, T>// where T :class
{

}

