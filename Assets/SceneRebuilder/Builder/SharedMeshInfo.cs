using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SharedMeshInfoList : List<SharedMeshInfo>
{
    public List<MeshFilter> meshFilters = new List<MeshFilter>();

    public List<MeshFilter> GetMeshFilters()
    {
        return meshFilters;
    }

    public int sharedVertexCount = 0;

    public int totalVertexCount = 0;

    public int filterCount = 0;

    public SharedMeshInfoList()
    {
        
    }

    public void InitAll()
    {
        InitByRoot(null);
    }

    public SharedMeshInfoList(SharedMeshInfoList root)
    {
        this.AddRange(root);
    }

    public SharedMeshInfoList(GameObject root)
    {
        InitByRoot(root);
    }

    private void InitByRoot(GameObject root)
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

    public SharedMeshInfoList(MeshFilter[] mfs)
    {
        InitMeshFilters(mfs);
    }

    public SharedMeshInfoList(List<MeshFilter> mfs)
    {
        InitMeshFilters(mfs.ToArray());
    }

    private void InitMeshFilters(MeshFilter[] mfs)
    {
        this.meshFilters.AddRange(mfs);
        Debug.Log($"SharedMeshInfo.InitMeshFilters meshFilters:{mfs.Length}");
        Dictionary<Mesh, SharedMeshInfo> meshDict = new Dictionary<Mesh, SharedMeshInfo>();
        for (int i = 0; i < mfs.Length; i++)
        {
            float progress = (float)i / mfs.Length;
            ProgressBarHelper.DisplayProgressBar("InitMeshFilters", $"Progress {i}/{mfs.Length} {progress:P1}", progress);
            MeshFilter mf = mfs[i];
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
        
        
    }

    public override string ToString()
    {
        return $"v:{MeshHelper.GetVertexCountS(sharedVertexCount)} m:{this.Count}";
    }

    public void AddEx(SharedMeshInfo item)
    {
        base.Add(item);
        sharedVertexCount += item.vertexCount;
        filterCount += item.GetCount();
        totalVertexCount += item.GetAllVertexCount();

        SortByType(0);
    }

    //public void GetPrefabs()
    //{
    //    //SharedMeshInfoList list1 = new SharedMeshInfoList(this);
    //    //SharedMeshInfoList list2 = new SharedMeshInfoList(this);

    //    PrefabInfoListHelper.GetPrefabInfos(this,true);
    //}

    public void Destroy(int v)
    {
        foreach(var item in this)
        {
            item.Destroy(v);
        }
    }

    public void SortByType(int sortType)
    {
        //"SharedV", "AllV", "SharedCount"
        if (sortType==0)
        {
            this.Sort((a, b) =>
            {
                var r1 = b.vertexCount.CompareTo(a.vertexCount);
                if (r1 == 0)
                {
                    r1 = b.meshFilters.Count.CompareTo(a.meshFilters.Count);
                }
                return r1;
            });
        }

        if (sortType == 1)
        {
            this.Sort((a, b) =>
            {
                var r1 = b.GetAllVertexCount().CompareTo(a.GetAllVertexCount());
                //if (r1 == 0)
                //{
                //    r1 = b.vertexCount.CompareTo(a.vertexCount);
                //}
                return r1;
            });
        }

        if (sortType == 2)
        {
            this.Sort((a, b) =>
            {
                var r1 = b.meshFilters.Count.CompareTo(a.meshFilters.Count);
                if (r1 == 0)
                {
                    r1 = b.vertexCount.CompareTo(a.vertexCount);
                }
                return r1;
            });
        }





        
    }
}

[Serializable]
public class SharedMeshInfo:IPrefab<SharedMeshInfo>
{
    public Mesh mesh;

    public List<MeshFilter> meshFilters = new List<MeshFilter>();

    public List<MeshFilter> GetMeshFilters()
    {
        return meshFilters;
    }

    public MeshFilter mainMeshFilter;

    public MeshFilter GetMainMeshFilter()
    {
        if (mainMeshFilter == null)
        {
            mainMeshFilter = meshFilters[0];
        }
        return mainMeshFilter;
    }

    public List<GameObject> GetGameObjects()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (var mf in meshFilters)
        {
            if (mf == null) continue;
            objs.Add(mf.gameObject);
        }
        return objs;
    }

    public void Destroy()
    {
        foreach(var mf in meshFilters)
        {
            if (mf == null) continue;
            EditorHelper.UnpackPrefab(mf.gameObject);
            GameObject.DestroyImmediate(mf.gameObject);
        }
    }

    public void Destroy(int id)
    {
        for (int i = id; i < meshFilters.Count; i++)
        {
            MeshFilter mf = meshFilters[i];
            if (mf == null)
            {
                continue;
            }
            EditorHelper.UnpackPrefab(mf.gameObject);
            GameObject.DestroyImmediate(mf.gameObject);
            
        }
    }

    public int vertexCount;

    public int GetVertexCount()
    {
        if (vertexCount == 0 && mesh!=null)
        {
            vertexCount = mesh.vertexCount;
        }
        return vertexCount;
    }

    public SharedMeshInfo(GameObject go)
    {
        this.go = go;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        
        this.mesh = mf.sharedMesh;
        this.vertexCount = mesh.vertexCount;

        meshFilters.Add(mf);
        if (mf.name == mesh.name)
        {
            mainMeshFilter = mf;
        }
    }

    public SharedMeshInfo(Mesh mesh)
    {
        this.mesh = mesh;
        this.vertexCount = mesh.vertexCount;
    }

    public void AddMeshFilter(MeshFilter mf)
    {
        if (mf == null)
        {
            Debug.LogError("SharedMeshInfo.AddMeshFilter mf == null");
            return;
        }
        meshFilters.Add(mf);
        if (mf.name == mesh.name)
        {
            mainMeshFilter = mf;
        }
        go = GetMainMeshFilter().gameObject;
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

    private GameObject go;


    public GameObject gameObject
    {
        get
        {
            return go;
        }
        set
        {
            go = value;
        }
    }

    public override string ToString()
    {
        return $"{gameObject.name}(v:{vertexCount}|c:{GetCount()})";
    }

    public Transform transform
    {
        get
        {
            return gameObject.transform;
        }
    }

    public void PreparePrefab()
    {
        
    }

    public SharedMeshInfo Clone()
    {
        
        GameObject goNew = MeshHelper.CopyGO(this.gameObject);
        SharedMeshInfo info = new SharedMeshInfo(goNew);
        return info;
        
    }
}