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

        //Debug.Log($"SharedMeshInfoList InitByRoot root:{root} meshFilters:{meshFilters.Length} meshCount:{this.Count}");
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
        DateTime start = DateTime.Now;
        this.meshFilters.AddRange(mfs);
        //Debug.Log($"SharedMeshInfo.InitMeshFilters meshFilters:{mfs.Length}");
        Dictionary<Mesh, SharedMeshInfo> meshDict = new Dictionary<Mesh, SharedMeshInfo>();
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            if (mf == null) continue;
            ProgressBarHelper.DisplayProgressBar(new ProgressArg("InitMeshFilters", i, mfs.Length, mf.name));
            var mesh = mf.sharedMesh;
            if (mesh == null) continue;
            if (!meshDict.ContainsKey(mesh))
            {
                meshDict.Add(mesh, new SharedMeshInfo(mesh));
            }
            SharedMeshInfo sharedMeshInfo = meshDict[mesh];
            sharedMeshInfo.AddMeshFilter(mf);
        }
        //Debug.Log($"SharedMeshInfo.InitMeshFilters time1:{(DateTime.Now-start).TotalMilliseconds}ms");

        AddList(meshDict.Values.ToList());

        //Debug.Log($"SharedMeshInfo.InitMeshFilters time2:{(DateTime.Now - start).TotalMilliseconds}ms");
        ProgressBarHelper.ClearProgressBar();
    }

    //public SharedMeshInfoList(ICollection<SharedMeshInfo> items)
    //{
    //    Init(items);
    //}

    private void AddList(ICollection<SharedMeshInfo> items)
    {
        foreach (var item in items)
        {
            this.AddEx(item);
        }
        SortByType(0);

    }

    private void AddList(List<SharedMeshInfo> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            SharedMeshInfo item = items[i];
            ProgressBarHelper.DisplayProgressBar(new ProgressArg("AddList", i, items.Count, item.GetName()));
            this.AddEx(item);
            //SortByType(0);
        }
        SortByType(0);

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

        //SortByType(0);
    }

    //public void GetPrefabs()
    //{
    //    //SharedMeshInfoList list1 = new SharedMeshInfoList(this);
    //    //SharedMeshInfoList list2 = new SharedMeshInfoList(this);

    //    PrefabInfoListHelper.GetPrefabInfos(this,true);
    //}

    public void DestroyFromStartId(int v)
    {
        foreach(var item in this)
        {
            item.DestroyFromStartId(v);
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
                if (r1 == 0)
                {
                    r1 = a.GetName().CompareTo(b.GetName());
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
                if (r1 == 0)
                {
                    r1 = a.GetName().CompareTo(b.GetName());
                }
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
                if (r1 == 0)
                {
                    r1 = a.GetName().CompareTo(b.GetName());
                }
                return r1;
            });
        }

        if (sortType == 3)
        {
            this.Sort((a, b) =>
            {
                var r1 = a.GetName().CompareTo(b.GetName());
                //var r1 = b.meshFilters.Count.CompareTo(a.meshFilters.Count);
                if (r1 == 0)
                {
                    r1 = b.vertexCount.CompareTo(a.vertexCount);
                }
                if (r1 == 0)
                {
                    r1 = b.meshFilters.Count.CompareTo(a.meshFilters.Count);
                }
                return r1;
            });
        }



    }

    public void DeleteToOne()
    {
        foreach(SharedMeshInfo item in this)
        {
            item.DeleteToOne();
        }
    }

    public void AddInstanceInfo()
    {
        SharedMeshInfoList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            SharedMeshInfo item = list[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("AddInstanceInfo", i, list.Count, item)))
            {
                break;
            }
            item.AddInstanceInfo();
        }
        ProgressBarHelper.ClearProgressBar();
    }

    public void SaveMesh()
    {
        SharedMeshInfoList list = this;
        Dictionary<string, SharedMeshInfo> dict = new Dictionary<string, SharedMeshInfo>();
        for (int i = 0; i < list.Count; i++)
        {
            SharedMeshInfo item = list[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("SaveMesh", i, list.Count, item)))
            {
                break;
            }
            string assetPath=item.SaveMesh();
            if(dict.ContainsKey(assetPath))
            {
                Debug.LogError($"SaveMesh dict.ContainsKey(assetPath)! assetPath:{assetPath} item:{item}");
            }
            else
            {
                dict.Add(assetPath, item);
            }
        }
        ProgressBarHelper.ClearProgressBar();
    }

    internal SharedMeshInfo FindByMesh(Mesh mesh)
    {
        foreach(var item in this)
        {
            if (item.mesh == mesh)
            {
                return item;
            }
        }
        return null;
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

    public void DestroyFromStartId(int startId)
    {
        for (int i = startId; i < meshFilters.Count; i++)
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
        if (mesh == null)
        {
            return "";
        }
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

    internal void DeleteToOne()
    {
        DestroyFromStartId(1);
    }

    private MeshFilter FindPrefab()
    {
        List<MeshFilter> prefabs = new List<MeshFilter>();

        if (meshFilters.Count == 1)
        {
            prefabs.Add(meshFilters[0]);
            return prefabs[0];
        }

        string meshName = mesh.name;

        if (mesh.name.Contains("_New"))
        {
            meshName = meshName.Replace("_New", "");
            meshName = meshName.Replace("_Combined_M_Combined0", "");
            for (int i = 0; i < meshFilters.Count; i++)
            {
                MeshFilter mf = meshFilters[i];
                if (mf == null)
                {
                    continue;
                }
                if (meshName == mf.name)
                {
                    //Debug.Log($"AddInfo[{mesh.name}][{i}][{mf}]");
                    prefabs.Add(mf);
                }
            }
        }
        else
        {
            for (int i = 0; i < meshFilters.Count; i++)
            {
                MeshFilter mf = meshFilters[i];
                if (mf == null)
                {
                    continue;
                }
                //if (mf.name == mesh.name)
                //{
                //    Debug.Log($"AddInfo[{mesh.name}][{i}][{mf}]");
                //}
                if (mf.name.Contains("_New"))
                {

                }
                else
                {
                    prefabs.Add(mf);
                }
            }
        }




        if (prefabs.Count == 1)
        {

        }
        else
        {
            if (prefabs.Count == 0)
            {
                Debug.LogError($"AddInfo[{mesh.name},{meshName}]prefabs:{prefabs.Count} all:{meshFilters.Count}");
                return null;
            }
            else
            {
                Debug.LogWarning($"AddInfo[{mesh.name},{meshName}]prefabs:{prefabs.Count} all:{meshFilters.Count} go:{prefabs[0].gameObject.name}");
            }
            
        }

        if (prefabs.Count == meshFilters.Count)
        {
            return meshFilters[0];
        }
        else
        {
            return prefabs[0];
        }
        
    }

    internal void AddInstanceInfo()
    {
        var mf0=FindPrefab();

        PrefabInfo prefab = new PrefabInfo(mf0.gameObject);

        //MeshPrefabInstance instance = mf0.gameObject.AddMissingComponent<MeshPrefabInstance>();
        //instance.IsPrefab = true;
        //instance.PrefabGo = mf0.gameObject;

        for (int i = 0; i < meshFilters.Count; i++)
        {
            MeshFilter mf = meshFilters[i];
            if (mf == null)
            {
                continue;
            }
            if (mf == mf0)
            {
                continue;
            }
            prefab.AddInstance(mf.gameObject);
        }
    }



    internal string SaveMesh()
    {
        var mf0 = FindPrefab();
#if UNITY_EDITOR
        string assetPath = EditorHelper.SaveMeshAssetResource(mf0);
        //EditorHelper.SaveMeshAsset();
        return assetPath;
#else
        return "";
#endif

    }

}