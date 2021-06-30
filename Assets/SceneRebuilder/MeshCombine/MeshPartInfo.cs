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

    public int vertexCount=0;

    public MeshPartInfo(List<MeshFilter> mfs){
        meshFilters=mfs;
    }

    public MeshPartInfo(){
        meshFilters=new List<MeshFilter>();
    }

    public void Add(MeshFilter mf)
    {
        meshFilters.Add(mf);
        vertexCount+=mf.sharedMesh.vertexCount;
    }

    public void Add(MeshFilter mf,int vc)
    {
        meshFilters.Add(mf);
        vertexCount+=vc;
    }
}