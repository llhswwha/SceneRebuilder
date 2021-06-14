using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneModelInfo : MonoBehaviour
{

    public int MaxVertexCount=0;

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        var meshFilters=GetMeshFilters();
        foreach (var item in meshFilters)
        {
            MeshRenderer renderer=item.GetComponent<MeshRenderer>();
            renderer.enabled=true;
        }
    }

    [ContextMenu("HideSmallModels")]
    public void HideSmallModels()
    {
        Debug.Log("HideSmallModels");
        List<MeshFilter> Bigs=new List<MeshFilter>();
        List<MeshFilter> Smalls=new List<MeshFilter>();
        GetVertexBigSmall(Bigs,Smalls);
        foreach (var item in Smalls)
        {
            MeshRenderer renderer=item.GetComponent<MeshRenderer>();
            renderer.enabled=false;
        }
    }

    [ContextMenu("HideBigModels")]
    public void HideBigModels()
    {
        Debug.Log("HideBigModels");
        List<MeshFilter> Bigs=new List<MeshFilter>();
        List<MeshFilter> Smalls=new List<MeshFilter>();
        GetVertexBigSmall(Bigs,Smalls);
        foreach (var item in Bigs)
        {
            MeshRenderer renderer=item.GetComponent<MeshRenderer>();
            renderer.enabled=false;
        }
    }

    [ContextMenu("TestGetVertexBigSmall")]
    public void TestGetVertexBigSmall()
    {
        List<MeshFilter> Bigs=new List<MeshFilter>();
        List<MeshFilter> Smalls=new List<MeshFilter>();
        GetVertexBigSmall(Bigs,Smalls);
    }

    //[ContextMenu("GetVertexBigSmall")]
    public void GetVertexBigSmall(List<MeshFilter> Bigs,List<MeshFilter> Smalls)
    {
        // List<MeshFilter> Bigs=new List<MeshFilter>();
        // List<MeshFilter> Smalls=new List<MeshFilter>();
        //ShowRenderers();
        DateTime start=DateTime.Now;
        var meshFilters=GetMeshFilters();
        float sumCount=0;
        float sumBigVertex=0;
        float sumSmallVertex=0;
        foreach(MeshFilter mf in meshFilters)
        {
            var count=mf.sharedMesh.vertexCount;
            sumCount+=count;
            if(MaxVertexCount>0 && count>MaxVertexCount){
                Bigs.Add(mf);
                sumBigVertex+=count;
            }
            else{
                Smalls.Add(mf);
                sumSmallVertex+=count;
            }
        }
        info=$"Count:{meshFilters.Length},Big:{Bigs.Count},Small:{Smalls.Count},Time:{(DateTime.Now-start).TotalMilliseconds}ms";
        info+=$"\nsum:{sumCount/10000}w,sumBig:{sumBigVertex/10000}w,sumSmall:{sumSmallVertex/10000}w";
        Debug.LogWarning(info);
    }

    public string info="";

    private MeshFilter[] GetMeshFilters()
    {
        MeshFilter[]  meshFilters=GameObject.FindObjectsOfType<MeshFilter>();
        return meshFilters;
    }

    [ContextMenu("GetVertexCountInfo")]
    private void GetVertexCountInfo()
    {
        //ShowRenderers();
        DateTime start=DateTime.Now;
        var meshFilters=GetMeshFilters();
        float minCount=float.MaxValue;
        float maxCount=0;
        float sumCount=0;
        float avgCount=0;
        List<int> countList = new List<int>();
        foreach(MeshFilter mf in meshFilters)
        {
            var count=mf.sharedMesh.vertexCount;
            sumCount+=count;
            if(count>maxCount){
                maxCount=count;
            }
            if(count<minCount){
                minCount=count;
            }
            if(!countList.Contains(count))
                countList.Add(count);
        }
        countList.Sort();
        countList.Reverse();
        string countStr = "";
        for(int i=0;i<500&&i<countList.Count;i++)
        {
            countStr += countList[i] + "; ";
        }
        avgCount =sumCount/(float)meshFilters.Length;

        Debug.LogWarning($"GetVertexCountInfo maxCount:{maxCount},minCount:{minCount},avgCount:{avgCount},sumCount:{sumCount/10000}w,Renderers:{meshFilters.Length},Time:{(DateTime.Now-start).TotalMilliseconds}ms\ncountStr:{countStr}");
    }
}
