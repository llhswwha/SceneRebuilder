using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshRotateBuffer
{
    public List<MeshRotateInfo> Items = new List<MeshRotateInfo>();

    public MeshRotateInfo Get(string k1,string k2, Vector3 v1)
    {
        MeshRotateInfo item = null;
        item = Items.Find(i => i.key1 == k1 && i.V1 == v1 && i.key2 == k2);
        return item;
    }

    public MeshRotateInfo Set(string k1,string k2, Vector3 v1,Vector3 v2)
    {
        MeshRotateInfo item = new MeshRotateInfo(k1,k2,v1,v2);
        Items.Add(item);
        return item;
    }
}

public class MeshRotateInfo
{
    public string key1;

    public string key2;

    public Vector3 V1;

    public Vector3 V2;

    public MeshRotateInfo()
    {

    }

    public MeshRotateInfo(string k,Vector3 v1,Vector3 v2)
    {
        key1 = k;
        V1 = v1;
        V2 = v2;
    }

    public MeshRotateInfo(string k1,string k2, Vector3 v1, Vector3 v2)
    {
        key1 = k1;
        key2 = k2;
        V1 = v1;
        V2 = v2;
    }

    public override string ToString()
    {
        return string.Format("{0},{1},{2},{3}", key1,key2, V1, V2);
    }
}
