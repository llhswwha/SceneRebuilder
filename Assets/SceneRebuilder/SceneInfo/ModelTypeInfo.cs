using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MaxModelTypeInfo 
{
    public string id;
    public string name;
    public int count;
    public int numverts;
    public long totalNumVerts;
    public float percent;
    public string className;//class
    public string category;
}


public struct MaxModelTypeInfoList
{
    public List<MaxModelTypeInfo> items;
    public List<string> typeList;
    public long totalVerts;
}