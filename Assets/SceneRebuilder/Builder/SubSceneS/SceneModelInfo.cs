using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneModelInfo : MonoBehaviour
{

    public int MaxVertexCount=0;

    public Material[] LODMaterials;

    public float[] LODLevels = new float[] { 0.6f, 0.2f, 0.07f, 0.01f };

    public GameObject BigModelRoot = null;
    public GameObject SmallModelRoot = null;

    public List<MeshFilter> Bigs = new List<MeshFilter>();
    public List<float> BigsVertexList = new List<float>();
    private List<MeshFilter> Smalls = new List<MeshFilter>();

    private void InitModelRoot()
    {
        if (BigModelRoot == null)
        {
            BigModelRoot = new GameObject("BigModelRoot");
        }
        if (SmallModelRoot == null)
        {
            SmallModelRoot = new GameObject("SmallModelRoot");
        }
    }

    [ContextMenu("SetBigLOD")]
    public void SetBigLOD()
    {
        InitModelRoot();
        GetVertexBigSmall();
        for (int i = 0; i < Bigs.Count; i++)
        {
            float progress = (float)i / Bigs.Count;
            float percent = progress * 100;
            MeshFilter item = Bigs[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar("SetBigLOD", $"item:{item.name} {percent:F1}% {i}/{Bigs.Count}", progress))
            {
                break;
            }

            item.gameObject.transform.SetParent(BigModelRoot.transform);

            CreateLOD(item.gameObject);

            MeshRenderer renderer = item.GetComponent<MeshRenderer>();
            if (renderer == null) continue;
            renderer.enabled = false;
        }
        ProgressBarHelper.ClearProgressBar();
    }

    public void CreateLOD(GameObject go)
    {
        AutomaticLODHelper.CreateAutoLOD(go, LODMaterials, LODLevels,null,false,false);
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        var meshFilters=GetMeshFilters();
        foreach (var item in meshFilters)
        {
            MeshRenderer renderer=item.GetComponent<MeshRenderer>();
            if (renderer == null) continue;
            renderer.enabled=true;
        }
    }

    [ContextMenu("ShowBigModels")]
    public void ShowBigModels()
    {
        Debug.Log("ShowBigModels");
        GetVertexBigSmall();
        foreach (var item in Bigs)
        {
            MeshRenderer renderer = item.GetComponent<MeshRenderer>();
            if (renderer == null) continue;
            renderer.enabled = true;
        }
    }

    [ContextMenu("HideSmallModels")]
    public void HideSmallModels()
    {
        Debug.Log("HideSmallModels");

        GetVertexBigSmall();
        foreach (var item in Smalls)
        {
            MeshRenderer renderer=item.GetComponent<MeshRenderer>();
            if (renderer == null) continue;
            renderer.enabled=false;
        }
    }

    [ContextMenu("HideBigModels")]
    public void HideBigModels()
    {
        Debug.Log("HideBigModels");
        GetVertexBigSmall();
        foreach (var item in Bigs)
        {
            MeshRenderer renderer=item.GetComponent<MeshRenderer>();
            if (renderer == null) continue;
            renderer.enabled=false;
        }
    }

    //[ContextMenu("TestGetVertexBigSmall")]
    //public void TestGetVertexBigSmall()
    //{
    //    GetVertexBigSmall();
    //}

    [ContextMenu("SplitBigSmall")]
    public void SplitBigSmall()
    {
        InitModelRoot();
        GetVertexBigSmall();
        for (int i = 0; i < Bigs.Count; i++)
        {
            var item = Bigs[i];
            item.transform.SetParent(BigModelRoot.transform);
        }
        for (int i = 0; i < Smalls.Count; i++)
        {
            var item = Smalls[i];
            item.transform.SetParent(SmallModelRoot.transform);
        }
    }

    public int MeshFilterCount = 0;


    [ContextMenu("GetVertexBigSmall")]
    public void GetVertexBigSmall()
    {
        // List<MeshFilter> Bigs=new List<MeshFilter>();
        // List<MeshFilter> Smalls=new List<MeshFilter>();
        //ShowRenderers();
        DateTime start=DateTime.Now;
        var meshFilters=GetMeshFilters();
        int mfCount = meshFilters.Length;
        //if (mfCount == MeshFilterCount) return;
        MeshFilterCount = mfCount;
        float sumCount=0;
        float sumBigVertex=0;
        float sumSmallVertex=0;
        Bigs.Clear();
        Smalls.Clear();
        BigsVertexList.Clear();
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf == null) continue;
            if (mf.sharedMesh == null) continue;
            var vertexC=mf.sharedMesh.vertexCount;
            sumCount+=vertexC;
            if(MaxVertexCount>0 && vertexC>MaxVertexCount){
                Bigs.Add(mf);
                sumBigVertex+=vertexC;
            }
            else{
                Smalls.Add(mf);
                sumSmallVertex+=vertexC;
            }
        }

        Bigs.Sort((a, b) =>
        {
            return a.sharedMesh.vertexCount.CompareTo(b.sharedMesh.vertexCount);
        });

        foreach (var item in Bigs)
        {
            BigsVertexList.Add(item.sharedMesh.vertexCount / 10000);
        }

        info =$"Count:{meshFilters.Length}=B:{Bigs.Count}+S:{Smalls.Count},T:{(DateTime.Now-start).TotalMilliseconds:F1}ms";
        info+=$"\nsum:{sumCount/10000f:F1}w,sumB:{sumBigVertex/ 10000f:F1}w,sumS:{sumSmallVertex/ 10000f:F1}w";
        Debug.LogWarning(info);
    }

    public string info="";

    private MeshFilter[] GetMeshFilters()
    {
        MeshFilter[]  meshFilters=GameObject.FindObjectsOfType<MeshFilter>(true);
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
