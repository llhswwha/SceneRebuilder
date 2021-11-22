using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformDictionary 
{
    public List<Transform> list;

    public TransformDictionary(List<Transform> lst)
    {
        this.list = lst;
        InitDict();
        CheckUidRepeated();
    }

    private void InitDict()
    {
        int allCount = list.Count;
        int uidCount = 0;
        List<string> uids = new List<string>();
        List<string> names = new List<string>();
        foreach (var t in list)
        {
            string n = t.name;
            if(n.Contains("_New"))
            {
                n = n.Replace("_New", "");
                t.name = n;
            }

            if (!nameListDict.ContainsKey(n))
            {
                nameListDict.Add(n, new List<Transform>());
                names.Add(n);
            }
            nameListDict[n].Add(t);



            if (IsUID(n))
            {
                uidCount++;

                if (!uidListDict.ContainsKey(n))
                {
                    uidListDict.Add(n, new List<Transform>());
                }
                uidListDict[n].Add(t);

                uids.Add(n);
            }
        }

        uids.Sort();
        string txt = "";
        foreach (var uid in uids)
        {
            txt += uid + ";\t";
        }

        names.Sort();
        string txt2 = "";
        foreach (var name in names)
        {
            txt2 += name + ";\t";
        }

        Debug.LogError($"TransformDictionary allCount:{allCount} nameCount:{nameListDict.Count} uidCount:{uidCount}\n uids:{txt}\n names:{txt2}");
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
        Debug.LogError($"CheckUidRepeated repeatedCount:{repeatedCount} repeatedUid:{txt}");
    }

    public static bool IsUID(string n)
    {
        //            <ItemInfo Id="H级主厂房.nwc_6_433" Name="H级精处理再生系统设备管道" UId="0027-20014-345476504420876800" X="-180.3085" Y="143.6485" Z="7.421375" Type="P3DEquipment" Drawable="true" Visible="0" AreaId="0">

        var length = n.Length;
        bool result = false;
        if (length == 29)
        {
            result = true;
        }
        //Debug.Log($"IsUID s:{n} length:{length} result:{result}");
        return result;
    }

    public TransformDictionary()
    {

    }

    public Dictionary<string, List<Transform>> nameListDict = new Dictionary<string, List<Transform>>();
    public Dictionary<string, List<Transform>> uidListDict = new Dictionary<string, List<Transform>>();
    public Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>();
    public Dictionary<string, Transform> uidDict = new Dictionary<string, Transform>();

    public Dictionary<string, List<Transform>> posListDict = new Dictionary<string, List<Transform>>();

    public Transform GetTransform(string n)
    {
        if (nameListDict.ContainsKey(n))
        {
            if (nameListDict.Count == 1)
            {
                return nameListDict[n][0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    public List<Transform> GetTransforms(string n)
    {
        if (nameListDict.ContainsKey(n))
        {
            return nameListDict[n];
        }
        return null;
    }

    internal void FindObjectByUID(string uId)
    {
        
    }

    internal void FindObjectByPos(string uId)
    {

    }
}
