using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
 
 [Serializable]
public class MeshPartInfo{
    public Mesh mesh;

    public Material[] mats;

    public List<MeshFilter> meshFilters;

    public List<int> meshIndexes;

    public int vertexCount=0;

    public Vector3 offset = Vector3.zero;

    public int GetMeshCount()
    {
        if (meshFilters == null)
        {
            return 0;
        }
        return meshFilters.Count;
    }

    public int GetMeshIndex(int id)
    {
        if (meshIndexes == null)
        {
            return 0;
        }
        if(meshIndexes.Count-1>=id)
        {
            return meshIndexes[id];
        }
        else
        {
            return 0;
        }
    }

    public MeshPartInfo(List<MeshFilter> mfs, List<int> indexes)
    {
        meshFilters=mfs;
        meshIndexes = indexes;
    }

    public MeshPartInfo(){
        meshFilters=new List<MeshFilter>();
        meshIndexes = new List<int>();
    }

    //public void Add(MeshFilter mf,int id)
    //{
    //    meshFilters.Add(mf);
    //    meshIndexes.Add(id);
    //    //vertexCount+=mf.sharedMesh.vertexCount;
    //    vertexCount += (int)(mf.sharedMesh.GetIndexCount(id));
    //}

    public void Add(MeshFilter mf, int id, int vc)
    {
        meshFilters.Add(mf);
        meshIndexes.Add(id);
        vertexCount += vc;
    }
}