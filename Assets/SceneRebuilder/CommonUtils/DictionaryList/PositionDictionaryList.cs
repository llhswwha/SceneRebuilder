using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PositionDictionaryList<T> // where T : class
{
    public DictionaryList1ToN<T> posListDict = new DictionaryList1ToN<T>();
    public DictionaryList1ToN<T> posListDict2 = new DictionaryList1ToN<T>();//6
    public DictionaryList1ToN<T> posListDict3 = new DictionaryList1ToN<T>();//5
    public DictionaryList1ToN<T> posListDict4 = new DictionaryList1ToN<T>();//4
    public DictionaryList1ToN<T> posListDict5 = new DictionaryList1ToN<T>();//3
    public DictionaryList1ToN<T> posListDict6 = new DictionaryList1ToN<T>();//2
    public DictionaryList1ToN<T> posListDict7 = new DictionaryList1ToN<T>();//2

    public T GetItem(Vector3 pos, out int listId)
    {
        var posT1 = $"({pos.x},{pos.y},{pos.z})";
        //var posT1 = $"({pos.x.ToString("F7")},{pos.y.ToString("F7")},{pos.z.ToString("F7")})";
        if (posListDict.ContainsKey(posT1))
        {
            listId = 1;
            return posListDict.GetItem(posT1);
        }

        var posT2 = pos.Vector3ToString6();
        if (posListDict2.ContainsKey(posT2))
        {
            listId = 2;
            return posListDict2.GetItem(posT2);
        }

        var posT3 = pos.Vector3ToString5();
        if (posListDict3.ContainsKey(posT3))
        {
            listId = 3;
            return posListDict3.GetItem(posT3);
        }

        var posT4 = pos.Vector3ToString4();
        if (posListDict4.ContainsKey(posT4))
        {
            listId = 4;
            return posListDict4.GetItem(posT4);
        }

        var posT5 = pos.Vector3ToString3();
        if (posListDict5.ContainsKey(posT5))
        {
            listId = 5;
            return posListDict5.GetItem(posT5);
        }

        var posT6 = $"({pos.x.ToString("F2")},{pos.y.ToString("F2")},{pos.z.ToString("F2")})";
        if (posListDict6.ContainsKey(posT6))
        {
            listId = 6;
            return posListDict6.GetItem(posT6);
        }
        var posT7 = $"({pos.x.ToString("F1")},{pos.y.ToString("F1")},{pos.z.ToString("F1")})";
        if (posListDict7.ContainsKey(posT7))
        {
            listId = 7;
            return posListDict7.GetItem(posT7);
        }
        listId = 0;
        return default(T);
    }

    public List<T> GetItems(Vector3 pos, out int listId)
    {
        var posT1 = $"({pos.x},{pos.y},{pos.z})";
        if (posListDict.ContainsKey(posT1))
        {
            listId = 1;
            return posListDict.GetItems(posT1);
        }

        var posT2 = $"({pos.x.ToString("F6")},{pos.y.ToString("F6")},{pos.z.ToString("F6")})";
        if (posListDict2.ContainsKey(posT2))
        {
            listId = 2;
            return posListDict2.GetItems(posT2);
        }

        var posT3 = $"({pos.x.ToString("F5")},{pos.y.ToString("F5")},{pos.z.ToString("F5")})";
        if (posListDict3.ContainsKey(posT3))
        {
            listId = 3;
            return posListDict3.GetItems(posT3);
        }

        var posT4 = $"({pos.x.ToString("F4")},{pos.y.ToString("F4")},{pos.z.ToString("F4")})";
        if (posListDict4.ContainsKey(posT4))
        {
            listId = 4;
            return posListDict4.GetItems(posT4);
        }

        var posT5 = $"({pos.x.ToString("F3")},{pos.y.ToString("F3")},{pos.z.ToString("F3")})";
        if (posListDict5.ContainsKey(posT5))
        {
            listId = 5;
            return posListDict5.GetItems(posT5);
        }

        var posT6 = $"({pos.x.ToString("F2")},{pos.y.ToString("F2")},{pos.z.ToString("F2")})";
        if (posListDict6.ContainsKey(posT6))
        {
            listId = 6;
            return posListDict6.GetItems(posT6);
        }
        var posT7 = $"({pos.x.ToString("F1")},{pos.y.ToString("F1")},{pos.z.ToString("F1")})";
        if (posListDict7.ContainsKey(posT7))
        {
            listId = 7;
            return posListDict7.GetItems(posT7);
        }

        listId = 0;
        return null;
    }

    public void Remove(Vector3 pos, T t)
    {
        var posT1 = $"({pos.x},{pos.y},{pos.z})";
        posListDict.RemoveItem(posT1, t);

        var posT2 = $"({pos.x.ToString("F6")},{pos.y.ToString("F6")},{pos.z.ToString("F6")})";
        posListDict2.RemoveItem(posT2, t);

        var posT3 = $"({pos.x.ToString("F5")},{pos.y.ToString("F5")},{pos.z.ToString("F5")})";
        posListDict3.RemoveItem(posT3, t);

        var posT4 = $"({pos.x.ToString("F4")},{pos.y.ToString("F4")},{pos.z.ToString("F4")})";
        posListDict4.RemoveItem(posT4, t);

        var posT5 = $"({pos.x.ToString("F3")},{pos.y.ToString("F3")},{pos.z.ToString("F3")})";
        posListDict5.RemoveItem(posT5, t);

        var posT6 = $"({pos.x.ToString("F2")},{pos.y.ToString("F2")},{pos.z.ToString("F2")})";
        posListDict6.RemoveItem(posT6, t);

        var posT7 = $"({pos.x.ToString("F1")},{pos.y.ToString("F1")},{pos.z.ToString("F1")})";
        posListDict7.RemoveItem(posT7, t);
    }

    /// <summary>
    /// 1:F6,2:F5,3:F4
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool Contains(Vector3 pos,int level=0)
    {
        var posT1 = $"({pos.x},{pos.y},{pos.z})";
        if (posListDict.ContainsKey(posT1)) //0
        {
            return true;
        }

        if (level > 0) //1
        {
            var posT2 = $"({pos.x.ToString("F6")},{pos.y.ToString("F6")},{pos.z.ToString("F6")})";
            if (posListDict2.ContainsKey(posT2))
            {
                return true;
            }
        }

        if (level > 1) //2
        {
            var posT3 = $"({pos.x.ToString("F5")},{pos.y.ToString("F5")},{pos.z.ToString("F5")})";
            if (posListDict3.ContainsKey(posT3))
            {
                return true;
            }
        }

        if (level > 2) //3
        {
            var posT4 = $"({pos.x.ToString("F4")},{pos.y.ToString("F4")},{pos.z.ToString("F4")})";
            if (posListDict4.ContainsKey(posT4))
            {
                return true;
            }
        }

        return false;
    }

    public void Add(Vector3 pos, T t, int level = 7)
    {

        var posT1 = $"({pos.x},{pos.y},{pos.z})";
        posListDict.AddItem(posT1, t);

        if (level > 1)
        {
            var posT2 = $"({pos.x.ToString("F6")},{pos.y.ToString("F6")},{pos.z.ToString("F6")})";
            posListDict2.AddItem(posT2, t);
        }

        if (level > 2)
        {
            var posT3 = $"({pos.x.ToString("F5")},{pos.y.ToString("F5")},{pos.z.ToString("F5")})";
            posListDict3.AddItem(posT3, t);
        }

        if (level > 3)
        {
            var posT4 = $"({pos.x.ToString("F4")},{pos.y.ToString("F4")},{pos.z.ToString("F4")})";
            posListDict4.AddItem(posT4, t);
        }

        if (level > 4)
        {
            var posT5 = $"({pos.x.ToString("F3")},{pos.y.ToString("F3")},{pos.z.ToString("F3")})";
            posListDict5.AddItem(posT5, t);
        }
        if (level > 5)
        {
            var posT6 = $"({pos.x.ToString("F2")},{pos.y.ToString("F2")},{pos.z.ToString("F2")})";
            posListDict6.AddItem(posT6, t);
        }
        if (level > 6)
        {
            var posT7 = $"({pos.x.ToString("F1")},{pos.y.ToString("F1")},{pos.z.ToString("F1")})";
            posListDict7.AddItem(posT7, t);
        }
    }

    public void ShowCount(string tag)
    {
        Debug.Log($"[{tag}][PositionDictionaryList] pos1:{posListDict.Count} pos2:{posListDict2.Count} pos3:{posListDict3.Count} pos4:{posListDict4.Count} pos5:{posListDict5.Count} pos6:{posListDict6.Count} pos7:{posListDict7.Count}");
    }
}