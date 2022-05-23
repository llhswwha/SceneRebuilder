using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubMesh
{
    public MeshFilter meshFilter;
    public int meshIndex;

    public string GetName()
    {
        if (meshFilter == null) return "NULL";
        //return meshFilter.name+"_"+meshIndex;
        return $"{meshFilter.name}[{meshIndex}]";
    }

    public SubMesh(MeshFilter mf, int id)
    {
        meshFilter = mf;
        meshIndex = id;
    }

    public override string ToString()
    {
        if (meshFilter == null) return "NULL";
        return $"mesh:{meshFilter.name}, index:{meshIndex}";
    }

    public Mesh sharedMesh
    {
        get
        {
            if (meshFilter == null) return null;
            return meshFilter.sharedMesh;
        }
    }
}

public class SubMeshList : List<SubMesh>
{
    public List<MeshFilter> GetMeshFilters()
    {
        List<MeshFilter> list = new List<MeshFilter>();
        foreach (var item in this)
        {
            list.Add(item.meshFilter);
        }
        return list;
    }
}
