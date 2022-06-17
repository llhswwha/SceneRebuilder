using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtils;

[Serializable]
public class MeshPoints
{
    public int GetInstanceID()
    {
        //return mf.sharedMesh.GetInstanceID();
        return this.InstanceId;
    }

    public bool IsSameSize(MeshPoints other)
    {
        return false;
    }

    //public static List<MeshPoints> GetMeshPoints(MeshFilter[] meshFilters)
    //{
    //    List<MeshPoints> meshPoints = new List<MeshPoints>();
    //    if(meshFilters!=null)
    //        foreach (var mf in meshFilters)
    //        {
    //            meshPoints.Add(new MeshPoints(mf.gameObject));
    //        }
    //    return meshPoints;
    //}

    public static List<MeshPoints> GetMeshPoints(GameObject root)
    {
        var mfs = root.GetComponentsInChildren<MeshFilter>(true);
        return GetMeshPoints(mfs);
    }

    public static List<MeshPoints> GetMeshPointsNoLOD(GameObject root)
    {
        List<MeshPoints> list = new List<MeshPoints>();
        GetMeshPointsNoLOD(root.transform, list);
        return list;
    }

    private static void GetMeshPointsNoLOD(Transform root, List<MeshPoints> list)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            var lod = child.GetComponent<LODGroup>();
            if (lod != null)
            {
                continue;
            }
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null)
            {
                list.Add(new MeshPoints(mf));
            }

            GetMeshPointsNoLOD(child, list);
        }
    }

    public static List<MeshPoints> GetMeshPoints<T>(T[] meshFilters) where T : Component
    {
        List<MeshPoints> meshPoints = new List<MeshPoints>();
        if (meshFilters != null)
            foreach (var mf in meshFilters)
            {
                MeshPoints mps = new MeshPoints(mf.gameObject);
                if (mps.vertexCount > 0)
                {
                    meshPoints.Add(mps);
                }
            }
        return meshPoints;
    }

    public static List<MeshPoints> GetMeshPoints<T>(List<T> meshFilters) where T : Component
    {
        List<MeshPoints> meshPoints = new List<MeshPoints>();
        if (meshFilters != null)
            foreach (var mf in meshFilters)
            {
                if (mf == null) continue;
                MeshPoints mps = new MeshPoints(mf.gameObject);
                if (mps.vertexCount > 0)
                {
                    meshPoints.Add(mps);
                }
            }
        return meshPoints;
    }

    private List<float> sizeList = null;

    public List<float> GetSizeList()
    {
        if (sizeList == null)
        {
            sizeList = new List<float>() { size.x, size.y, size.z };
            sizeList.Sort();
        }
        return sizeList;
    }

    public Vector3[] vertices;
    //public Vector3[] localVertices;
    private Vector3[] worldVertices;
    public int vertexCount = 0;
    public Vector3 size;

    public int InstanceId = 0;

    public GameObject gameObject;

    public string name;

    public Transform transform;
    public MeshFilter mf;

    public Mesh sharedMesh;

    public static Dictionary<int, string> dictId2Go = new Dictionary<int, string>();

    public MeshPoints(MeshFilter mf)
    {
        InitGo(mf.gameObject);
        InitMesh(mf);
    }

    public MeshPoints(GameObject root)
    {
        InitGoEx(root);
    }

    public void InitGoEx(GameObject root)
    {
        InitGo(root);
        mf = root.GetComponent<MeshFilter>();
        InitMesh(mf);
    }

    public void InitGo(GameObject root)
    {
        if (root == null)
        {
            Debug.LogError("MeshPoints.ctor root==null");
        }
        this.name = root.name;
        this.transform = root.transform;
        this.gameObject = root;
    }

    public void InitMesh(MeshFilter mf)
    {
        if (mf != null && mf.sharedMesh != null)
        {
            sharedMesh = mf.sharedMesh;
            vertices = sharedMesh.vertices;
            //localVertices = mf.sharedMesh.vertices;
            //InstanceId = sharedMesh.GetInstanceID();
            InstanceId = this.gameObject.GetInstanceID();
            size = sharedMesh.bounds.size;
            //worldVertices = MeshHelper.GetWorldVertexes(mf);
        }
        else
        {
            vertices = VertexHelper.GetChildrenVertexes(this.gameObject);
            //worldVertices = MeshHelper.GetChildrenWorldVertexes(root);
            InstanceId = this.gameObject.GetInstanceID();
            size = VertexHelper.GetMinMax(vertices)[2];
        }

        if (!dictId2Go.ContainsKey(InstanceId))
            dictId2Go.Add(InstanceId, this.gameObject.name);

        //PipeModelBase pipeModel = gameObject.GetComponent<PipeModelBase>();
        //if (pipeModel != null)
        //{
        //    vertices = pipeModel.GetAlignPoints();
        //}

        //if (vertices == null)
        //{
        //    Debug.LogError($"MeshPoints vertices == null pipeModel:{pipeModel.GetType()} mf:{mf} gameObject:{gameObject}");
        //    return;
        //}

        vertexCount = vertices.Length;
    }

    //public string GetDictKey(string defaultKey)
    //{
    //    PipeModelBase pipeModel = gameObject.GetComponent<PipeModelBase>();
    //    if (pipeModel == null)
    //    {
    //        PipeMeshGeneratorBase generator = gameObject.GetComponent<PipeMeshGeneratorBase>();
    //        if (generator != null)
    //        {
    //            if (generator.Target == null)
    //            {
    //                Debug.LogError($"GetDictKey name:{this.name} generator.Target == null");
    //            }
    //            else
    //            {
    //                pipeModel = generator.Target.GetComponent<PipeModelBase>();
    //            }
    //        }
    //    }

    //    if (pipeModel != null)
    //    {
    //        string key2 = pipeModel.GetDictKey();
    //        if (!string.IsNullOrEmpty(key2))
    //        {
    //            defaultKey = key2;
    //        }
    //    }
    //    else
    //    {
    //        //MeshRendererInfo meshRendererInfo = MeshRendererInfo.GetMinMax(this.gameObject);
    //    }

    //    return defaultKey;
    //}

    public T GetComponent<T>() where T : Component
    {
        return gameObject.GetComponent<T>();
    }

    public Vector3[] GetWorldVertexes()
    {
        if (mf != null)
        {
            worldVertices = VertexHelper.GetWorldVertexes(mf);
        }
        else
        {
            worldVertices = VertexHelper.GetChildrenWorldVertexes(gameObject);
        }
        return worldVertices;
    }

    public bool IsSameMesh(MeshPoints other)
    {
        return (this.sharedMesh != null && other.sharedMesh != null && this.sharedMesh == other.sharedMesh);
    }

    private static string GetMatId(MeshRenderer renderer)
    {
        //if (renderer.sharedMaterials.Length > 1)
        {
            string key = "";
            foreach(var mat in renderer.sharedMaterials)
            {
                key += mat.name + ";";
            }
            return "("+key+")";
        }
        //else
        //{
        //    Color color = Color.black;
        //    try
        //    {
        //        if (renderer.sharedMaterial != null)
        //        {
        //            if (renderer.sharedMaterial.HasProperty("_Color"))
        //                color = renderer.sharedMaterial.color;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Debug.LogError($"MeshFilterListDict render:{renderer},mat:{renderer.sharedMaterial},matName:{renderer.sharedMaterial.name},ex:{ex.ToString()}");
        //    }
        //    var matId = color.ToString();
        //    return matId;
        //}
    }



    public string GetMatId()
    {
        //MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
        //return GetMatId(renderer);

        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
        string matId = "";
        List<string> ids = new List<string>();
        foreach (var renderer in renderers)
        {
            string id = GetMatId(renderer);
            ids.Add(id);
        }
        ids.Sort();
        ids.ForEach(i => matId += i + "|");
        return matId;
    }

    public override string ToString()
    {
        return $"{name}({vertexCount})";
    }
}
