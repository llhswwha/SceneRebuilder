using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SharedMeshInfoList : List<SharedMeshInfo>
{
    //public List<SharedMeshInfo> Items = new List<SharedMeshInfo>();

    public int sharedVertexCount = 0;

    public int totalVertexCount = 0;

    public int filterCount = 0;

    public SharedMeshInfoList(GameObject root)
    {
        MeshFilter[] meshFilters = null;
        if (root == null)
        {
            meshFilters = GameObject.FindObjectsOfType<MeshFilter>(true);
        }
        else
        {
            meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        }

        InitMeshFilters(meshFilters);
    }

    private SharedMeshInfoList(MeshFilter[] meshFilters)
    {
        InitMeshFilters(meshFilters);
    }

    private void InitMeshFilters(MeshFilter[] meshFilters)
    {
        Dictionary<Mesh, SharedMeshInfo> meshDict = new Dictionary<Mesh, SharedMeshInfo>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            float progress = (float)i / meshFilters.Length;
            ProgressBarHelper.DisplayProgressBar("InitMeshFilters", $"Progress {i}/{meshFilters.Length} {progress:P1}", progress);
            MeshFilter mf = meshFilters[i];
            var mesh = mf.sharedMesh;
            if (mesh == null) continue;
            if (!meshDict.ContainsKey(mesh))
            {
                meshDict.Add(mesh, new SharedMeshInfo(mesh));
            }
            SharedMeshInfo sharedMeshInfo = meshDict[mesh];
            sharedMeshInfo.AddMeshFilter(mf);
        }

        Init(meshDict.Values);

        ProgressBarHelper.ClearProgressBar();
    }

    //public SharedMeshInfoList(ICollection<SharedMeshInfo> items)
    //{
    //    Init(items);
    //}

    private void Init(ICollection<SharedMeshInfo> items)
    {
        foreach (var item in items)
        {
            this.AddEx(item);
        }
        this.Sort((a, b) =>
        {
            return b.vertexCount.CompareTo(a.vertexCount);
        });
    }

    public void AddEx(SharedMeshInfo item)
    {
        base.Add(item);
        sharedVertexCount += item.vertexCount;
        filterCount += item.GetCount();
        totalVertexCount += item.GetAllVertexCount();
    }
}

[Serializable]
public class SharedMeshInfo
{
    public Mesh mesh;

    public List<MeshFilter> meshFilters = new List<MeshFilter>();

    public MeshFilter mainMeshFilter;

    public MeshFilter GetMainMeshFilter()
    {
        if (mainMeshFilter == null)
        {
            mainMeshFilter = meshFilters[0];
        }
        return mainMeshFilter;
    }

    public int vertexCount = 0;

    public SharedMeshInfo(Mesh mesh)
    {
        this.mesh = mesh;
        this.vertexCount = mesh.vertexCount;
    }

    public void AddMeshFilter(MeshFilter mf)
    {
        meshFilters.Add(mf);
        if (mf.name == mesh.name)
        {
            mainMeshFilter = mf;
        }
    }

    public string GetName()
    {
        return mesh.name;
    }

    public int GetCount()
    {
        return meshFilters.Count;
    }

    public int GetAllVertexCount()
    {
        return meshFilters.Count * vertexCount;
    }
}