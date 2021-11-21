using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformDictionary 
{
    List<Transform> list;

    public TransformDictionary(List<Transform> list)
    {
        this.list = list;
        InitDict();
    }

    private void InitDict()
    {
        int allCount = list.Count;
        int uidCount = 0;
        foreach (var t in list)
        {
            string n = t.name;
            if (IsUID(n))
            {
                uidCount++;
            }
            if (!nameListDict.ContainsKey(n))
            {
                nameListDict.Add(n, new List<Transform>());
            }
            nameListDict[n].Add(t);
        }
        Debug.LogError($"TransformDictionary allCount:{allCount} nameCount:{nameListDict.Count} uidCount:{uidCount}");
    }

    public void CheckRepeated()
    {

    }


    public bool IsUID(string n)
    {
        //            <ItemInfo Id="H级主厂房.nwc_6_433" Name="H级精处理再生系统设备管道" UId="0027-20014-345476504420876800" X="-180.3085" Y="143.6485" Z="7.421375" Type="P3DEquipment" Drawable="true" Visible="0" AreaId="0">

        return false;
    }

    public TransformDictionary()
    {

    }

    public Dictionary<string, List<Transform>> nameListDict = new Dictionary<string, List<Transform>>();
    public Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>();
    public Dictionary<string, List<Transform>> posListDict = new Dictionary<string, List<Transform>>();

    public Transform GetTransform(string n)
    {
        return null;
    }

    public List<Transform> GetTransforms(string n)
    {
        return null;
    }
}
