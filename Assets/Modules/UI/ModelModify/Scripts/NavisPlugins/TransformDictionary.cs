using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TransformDictionary 
{
    public List<Transform> list = new List<Transform>();

    public TransformDictionary(List<Transform> lst)
    {
        this.list.AddRange(lst);
        InitDict();
        GetTransformNames();
        CheckUidRepeated();
    }

    public void InitDict()
    {
        nameListDict = new DictionaryList1ToN<Transform>();
        uidListDict = new DictionaryList1ToN<Transform>();
        nameDict = new Dictionary<string, Transform>();
        uidDict = new Dictionary<string, Transform>();

        positionDictionaryList = new PositionDictionaryList<Transform>();
        InitNameDict();
        InitPosDict();
    }

    private void GetTransformNames()
    {
        List<string> uids = new List<string>();
        List<string> otherNames = new List<string>();
        List<string> kks = new List<string>();

        Dictionary<string, List<Transform>> nameDict = new Dictionary<string, List<Transform>>();

        foreach (var t in list)
        {
            string n = t.name;
            if (n.Contains("_New"))
            {
                n = n.Replace("_New", "");
                t.name = n;
            }

            if (!nameDict.ContainsKey(n))
            {
                nameDict.Add(n, new List<Transform>());
                //names.Add(n);

                if (IsUID(n))
                {
                    uids.Add(n);
                }
                else if (IsKKS(n))
                {
                    kks.Add(n);
                }
                else
                {
                    if (!n.Contains("Degree_Direction_Change") &&
                        !n.Contains("Concentric_Reducer") &&
                        !n.Contains("Flange-") &&
                        !n.Contains("Undefined") &&
                        !n.Contains("MemberPartPrismatic") &&
                        !n.Contains("Undefined"))
                        otherNames.Add(n);
                }
            }
            nameDict[n].Add(t);

        }

        uids.Sort();
        string txt = "";
        foreach (var uid in uids)
        {
            txt += uid + ";\t";
        }

        otherNames.Sort();
        string txt2 = "";
        foreach (var name in otherNames)
        {
            txt2 += name + ";\t";
        }

        kks.Sort();
        string txt3 = "";
        foreach (var name in kks)
        {
            txt3 += name + ";\t";
        }
        Debug.Log($"uids({uids.Count}):\n{txt}");
        Debug.Log($"kks({kks.Count}):\n{txt3}");
        Debug.Log($"otherNames({otherNames.Count}):\n{txt2}");
    }

    private void InitPosDict()
    {
        foreach (var t in list)
        {
            var pos = t.position;
            positionDictionaryList.Add(pos, t);
        }
        positionDictionaryList.ShowCount("TransformDictionary");
    }

    private void InitNameDict()
    {
        int allCount = list.Count;
        int uidCount = 0;
        foreach (var t in list)
        {
            string n = t.name;
            if(n.Contains("_New"))
            {
                n = n.Replace("_New", "");
                t.name = n;
            }

            nameListDict.AddItem(n, t);

            if (IsUID(n))
            {
                uidCount++;
                uidListDict.AddItem(n, t);
            }
        }
        Debug.Log($"TransformDictionary allCount:{allCount} nameCount:{nameListDict.Count} uidCount:{uidCount}");
    }

    public void CheckUidRepeated()
    {
        int repeatedCount = 0;
        List<string> repeatedUid = new List<string>();
        foreach(var uid in uidListDict.Keys)
        {
            var list = uidListDict[uid];
            if (list.Count > 1)
            {
                repeatedCount++;
                repeatedUid.Add(uid);
            }
            else
            {
                uidDict.Add(uid, list[0]);
            }
        }
        repeatedUid.Sort();
        string txt = "";
        foreach(var uid in repeatedUid)
        {
            txt += uid + ";\t";
        }
        if(repeatedCount>0)
            Debug.LogError($"[TransformDictionary.CheckUidRepeated] repeatedCount:{repeatedCount} repeatedUid:{txt}");
    }


    public static bool IsKKS(string n)
    {
        //J0QFF10AA401
        //HH-2-0LDB-11 12
        //20LDB10BR001 12
        //20LDB10BR 9

        //if(n.Contains("_"))
        int l = n.Length;
        if (l < 9 || l > 12)
        {
            return false;
        }
        if (IsNumAndUpperEnCh(n))
        {
            return true;
        }
        else
        {
            return false;
        }

        //return false;
    }

    public static bool IsEnCh(string input)
    {
        string pattern = @"^[A-Za-z]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(input);
    }

    public static bool IsNum(string input)
    {
        string pattern = @"^[0-9]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(input);
    }

    /// <summary>
    /// �ж�������ַ����Ƿ�ֻ�������ֺ�Ӣ����ĸ
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsNumAndUpperEnCh(string input)
    {
        string pattern = @"^[A-Z0-9]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(input);
    }

    public static bool IsLetterOrDigit(string s)
    {
        s = s.Replace("-", "");
        return IsNumAndUpperEnCh(s);
    }

    public static bool IsUID(string n)
    {
        //            <ItemInfo Id="H��������.nwc_6_433" Name="H������������ϵͳ�豸�ܵ�" UId="0028-140039-306069491919619191" X="-180.3085" Y="143.6485" Z="7.421375" Type="P3DEquipment" Drawable="true" Visible="0" AreaId="0">

        var length = n.Length;
        bool result = false;
        if (length == 30)
        {
            string[] parts = n.Split('-');
            if (parts.Length == 3)
            {
                if (parts[0].Length == 4 && parts[1].Length == 6 && parts[2].Length == 18)
                {
                    result = true;
                }
                else
                {
                    Debug.LogError($"IsUID s:{n} length:{length} {parts[0].Length},{parts[1].Length},{parts[2].Length}");
                }
            }
            else
            {
                Debug.LogError($"IsUID s:{n} length:{length} parts.Length != 3");
            }
        }
        return result;
    }

    public TransformDictionary()
    {

    }

    public DictionaryList1ToN<Transform> nameListDict = new DictionaryList1ToN<Transform>();
    public DictionaryList1ToN<Transform> uidListDict = new DictionaryList1ToN<Transform>();
    public Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>();
    public Dictionary<string, Transform> uidDict = new Dictionary<string, Transform>();

    public PositionDictionaryList<Transform> positionDictionaryList = new PositionDictionaryList<Transform>();

    public Transform GetTransformByName(string n)
    {
        if (nameListDict.ContainsKey(n))
        {
            var list = nameListDict[n];
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    public Transform GetTransformByUid(string n)
    {
        if (uidListDict.ContainsKey(n))
        {
            var list = uidListDict[n];
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    public Transform GetTransformByName_Debug(string n)
    {
        Debug.Log($"GetTransformByName_Debug n:{n}");
        if (nameListDict.ContainsKey(n))
        {
            var list = nameListDict[n];
            Debug.Log($"GetTransformByName_Debug n:{n}");
            if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    internal void RemoveTransform(Transform transform)
    {
        list.Remove(transform);
        //string n = transform.name;

    }

    public List<Transform> GetTransformsByName(string n)
    {
        if (nameListDict.ContainsKey(n))
        {
            return nameListDict[n];
        }
        return null;
    }

    internal Transform FindObjectByUID(string uId)
    {
        return GetTransformByName(uId);
    }

    internal Transform FindObjectByPos(ModelItemInfo model)
    {
        Vector3 pos = model.GetPositon();
        int listId = 0;
        var t=positionDictionaryList.GetItem(pos, out listId);

        //if (t != null)
        //{
        //    if (listId == 0)
        //    {

        //    }
        //    else if (listId == 1 || listId == 2)
        //    {
        //        Debug.Log($"[FindObjectByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //    else if (listId == 3 || listId == 4 || listId == 5)
        //    {
        //        Debug.LogWarning($"[FindObjectByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //    else
        //    {
        //        Debug.LogError($"[FindObjectByPos][{listId}] {model.ShowDistance(t)}");
        //    }
        //}

        return t;
    }
}

public class DictionaryList1ToN<T>: Dictionary<string, List<T>> where T :class
{
    public void AddItem(string key, T item)
    {
        if (!this.ContainsKey(key))
        {
            this.Add(key, new List<T>());
        }
        this[key].Add(item);
    }

    public T GetItem(string key)
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
                return null;
            }
        }
        return null;
    }

    public List<T> GetItems(string key)
    {
        if (this.ContainsKey(key))
        {
            var list = this[key];
            return list;
        }
        return null;
    }
}

public class PositionDictionaryList<T> where T : class
{
    public DictionaryList1ToN<T> posListDict = new DictionaryList1ToN<T>();
    public DictionaryList1ToN<T> posListDict2 = new DictionaryList1ToN<T>();//6
    public DictionaryList1ToN<T> posListDict3 = new DictionaryList1ToN<T>();//5
    public DictionaryList1ToN<T> posListDict4 = new DictionaryList1ToN<T>();//4
    public DictionaryList1ToN<T> posListDict5 = new DictionaryList1ToN<T>();//3
    public DictionaryList1ToN<T> posListDict6 = new DictionaryList1ToN<T>();//2
    public DictionaryList1ToN<T> posListDict7 = new DictionaryList1ToN<T>();//2

    public T GetItem(Vector3 pos,out int listId)
    {
        var posT1 = $"({pos.x},{pos.y},{pos.z})";
        if (posListDict.ContainsKey(posT1))
        {
            listId = 1;
            return posListDict.GetItem(posT1);
        }

        var posT2 = $"({pos.x.ToString("F6")},{pos.y.ToString("F6")},{pos.z.ToString("F6")})";
        if (posListDict2.ContainsKey(posT2))
        {
            listId = 2;
            return posListDict2.GetItem(posT2);
        }

        var posT3 = $"({pos.x.ToString("F5")},{pos.y.ToString("F5")},{pos.z.ToString("F5")})";
        if (posListDict3.ContainsKey(posT3))
        {
            listId = 3;
            return posListDict3.GetItem(posT3);
        }

        var posT4 = $"({pos.x.ToString("F4")},{pos.y.ToString("F4")},{pos.z.ToString("F4")})";
        if (posListDict4.ContainsKey(posT4))
        {
            listId = 4;
            return posListDict4.GetItem(posT4);
        }

        var posT5 = $"({pos.x.ToString("F3")},{pos.y.ToString("F3")},{pos.z.ToString("F3")})";
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
        return null;
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

    internal void Add(Vector3 pos, T t)
    {
        var posT1 = $"({pos.x},{pos.y},{pos.z})";
        posListDict.AddItem(posT1, t);

        var posT2 = $"({pos.x.ToString("F6")},{pos.y.ToString("F6")},{pos.z.ToString("F6")})";
        posListDict2.AddItem(posT2, t);

        var posT3 = $"({pos.x.ToString("F5")},{pos.y.ToString("F5")},{pos.z.ToString("F5")})";
        posListDict3.AddItem(posT3, t);

        var posT4 = $"({pos.x.ToString("F4")},{pos.y.ToString("F4")},{pos.z.ToString("F4")})";
        posListDict4.AddItem(posT4, t);

        var posT5 = $"({pos.x.ToString("F3")},{pos.y.ToString("F3")},{pos.z.ToString("F3")})";
        posListDict5.AddItem(posT5, t);

        var posT6 = $"({pos.x.ToString("F2")},{pos.y.ToString("F2")},{pos.z.ToString("F2")})";
        posListDict6.AddItem(posT6, t);

        var posT7 = $"({pos.x.ToString("F1")},{pos.y.ToString("F1")},{pos.z.ToString("F1")})";
        posListDict7.AddItem(posT7, t);
    }

    public void ShowCount(string tag)
    {
        Debug.Log($"[{tag}][PositionDictionaryList] pos1:{posListDict.Count} pos2:{posListDict2.Count} pos3:{posListDict3.Count} pos4:{posListDict4.Count} pos5:{posListDict5.Count} pos6:{posListDict6.Count} pos7:{posListDict7.Count}");
    }
}