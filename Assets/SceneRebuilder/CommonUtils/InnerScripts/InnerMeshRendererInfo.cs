using CommonUtils;
//using MeshJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InnerMeshRendererInfo : MonoBehaviour, IComparable<InnerMeshRendererInfo>
{
    public static float GetSizeDistance(GameObject g1, GameObject g2)
    {
        InnerMeshRendererInfo r1 = InnerMeshRendererInfo.GetInfo(g1);
        InnerMeshRendererInfo r2 = InnerMeshRendererInfo.GetInfo(g2);
        var vs1 = r1.GetBoxCornerPonts();
        var vs2 = r2.GetBoxCornerPonts();
        return DistanceUtil.GetDistance(vs1, vs2);
    }


    //public Vector3 position;

    public Vector3 center;

    public float disToCenter;

    public float diam = 0;

    [ContextMenu("GetMeshPoints")]
    public void GetMeshPoints()
    {
        MeshPoints mps = new MeshPoints(this.gameObject);
        if (mps.sharedMesh == null)
        {
            Debug.LogError($"GetMeshPoints mps.sharedMesh == null");
        }
    }


    public float GetDiam()
    {
        if (diam == 0)
        {
            diam = Vector3.Distance(minMax[0], minMax[1]);
        }
        return diam;
    }

    public string GetMats()
    {
        string ms = "";
        var rs = GetRenderers();
        foreach (var r in rs)
        {
            if (r == null)
            {
                continue;
            }
            if (r.sharedMaterial == null)
            {
                ms += "NullMat;";
                continue;
            }
            ms += r.sharedMaterial.name + ";";
        }
        return ms;
    }

    public Vector3 size;

    public float vertexCount;

    // public Bounds bounds;

    public MeshFilter meshFilter;

    public MeshRenderer meshRenderer;

    //public string assetPath;
    //public string assetName;


    public string GetAssetPath()
    {

#if UNITY_EDITOR
        if (meshFilter == null || meshFilter.sharedMesh == null) return "";
        var assetPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
        //assetName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
        return assetPath;
#else
        return "";
#endif

    }

    public Vector3[] minMax;

    public override string ToString()
    {
        return $"meshRenderer:{meshRenderer} vertexCount:{vertexCount}";
    }

    public virtual MeshRenderer[] GetRenderers()
    {
        return new MeshRenderer[1] { meshRenderer };
    }
    public virtual MeshFilter[] GetMeshFilters()
    {
        return new MeshFilter[1] { meshFilter };
    }

    public MeshRenderer GetBoundsRenderer()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.AddComponent<BoundsBox>();
        cube.SetActive(true);
        cube.name = this.name + "_Bounds";
        cube.transform.position = this.center;
        cube.transform.localScale = this.size;
        cube.transform.SetParent(this.transform.parent);
        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
        renderer.sharedMaterials = meshRenderer.sharedMaterials;
        Collider collider = cube.GetComponent<Collider>();
        if (collider)
        {
            GameObject.DestroyImmediate(collider);
        }
        return renderer;
    }

    public Vector3[] GetBoxCornerPonts()
    {
        return new Vector3[] { minMax[0], minMax[1] };
    }

    public void AddCollider()
    {
        MeshCollider meshCollider = this.gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = this.gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.enabled = true;
    }


    //public static GameObject CreateBoundsCube(Bounds bounds, string n, Transform parent, int prefabId)
    //{
    //    // Debug.Log($"CreateBoundsCube bounds:{bounds} name:{n} parent:{parent}");
    //    InitCubePrefab();

    //    if (CubePrefabs == null || CubePrefabs.Count == 0)
    //    {
    //        var cubePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //        cubePrefab.SetActive(false);
    //    }
    //    GameObject cube = GameObject.Instantiate(CubePrefabs[prefabId]);
    //    cube.AddComponent<BoundsBox>();
    //    cube.SetActive(true);
    //    cube.name = n;
    //    cube.transform.position = bounds.center;
    //    cube.transform.localScale = bounds.size;
    //    cube.transform.SetParent(parent);
    //    return cube;
    //}

    public Mesh sharedMesh
    {
        get
        {
            if (meshFilter == null) return null;
            return meshFilter.sharedMesh;
        }

    }

    public int GetVertexCount()
    {
        int count = 0;
        var filters = GetMeshFilters();
        foreach (var filter in filters)
        {
            if (filter == null)
            {
                Debug.LogError($"InnerMeshRendererInfo.GetVertexCount filter == null this:{this.name}");
                continue;
            }
            if (filter.sharedMesh == null)
            {
                Debug.LogError($"InnerMeshRendererInfo.GetVertexCount filter:{filter}");
                continue;
            }
            count += filter.sharedMesh.vertexCount;
        }
        return count;
    }

    public Vector3[] GetVertices()
    {
        if (this.meshFilter == null) return null;
        if (this.meshFilter.sharedMesh == null)
        {
            Debug.LogError($"InnerMeshRendererInfo.GetVertices this.meshFilter.sharedMesh == null object:{this.name}");
            return new Vector3[0];
        }
        return this.meshFilter.sharedMesh.vertices;
    }

    public int GetMinLODVertexCount()
    {
        LODGroup group = this.GetComponent<LODGroup>();
        if (group == null)
        {
            return GetVertexCount();
        }
        else
        {
            var lods = group.GetLODs();
            var renderers = lods[lods.Length - 1].renderers;
            int count = 0;
            foreach (var render in renderers)
            {
                MeshFilter filter = render.GetComponent<MeshFilter>();
                count += filter.sharedMesh.vertexCount;
            }
            return count;
        }
    }

    public GameObject GetMinLODGo()
    {
        LODGroup group = this.GetComponent<LODGroup>();
        if (group == null)
        {
            return this.gameObject;
        }
        else
        {
            var lods = group.GetLODs();
            var renderers = lods[lods.Length - 1].renderers;
            return renderers[0].gameObject;
        }
    }

    //public static string GetKeyDict(GameObject go)
    //{
    //    InnerMeshRendererInfo info = GetInfo(go);
    //    List<float> args = new List<float>();
    //    args.Add(info.size.x);
    //    args.Add(info.size.x);
    //    args.Add(info.size.x);
    //    args.Add(info.size.x);
    //}

    public static Vector3[] GetMinMax(GameObject go, bool isUpdate = true)
    {
        InnerMeshRendererInfo info = go.GetComponent<InnerMeshRendererInfo>();
        if (info == null)
        {
            info = go.AddComponent<InnerMeshRendererInfo>();
            info.Init();
        }
        else
        {
            if (isUpdate)
            {
                info.Init();
            }
        }
        return info.minMax;
    }

    public static InnerMeshRendererInfo GetInfo(MeshRenderer go)
    {
        return GetInfo(go.gameObject, false, false);
    }

    public static InnerMeshRendererInfo GetInfo(GameObject go)
    {
        return GetInfo(go, false, false);
    }

    public static InnerMeshRendererInfo GetInfo(MeshRenderer go, bool isUpdateId, bool isForceUpdate = false)
    {
        return GetInfo(go.gameObject, isUpdateId, isForceUpdate);
    }

    public static Vector3 GetCenterPos(GameObject go, bool isUpdateId = false, bool isForceUpdate = false)
    {
        InnerMeshRendererInfo info = GetInfo(go, isUpdateId, isForceUpdate);
        return info.center;
    }

    public static InnerMeshRendererInfo GetInfo(GameObject go, bool isUpdateId, bool isForceUpdate = false)
    {
        //Debug.Log($"InnerMeshRendererInfo go:{go}");
        InnerMeshRendererInfo info = go.GetComponent<InnerMeshRendererInfo>();

        //Debug.Log($"info:{info} go==null:{go == null}");
        if (info == null)
        {
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                info = go.AddComponent<InnerMeshRendererInfo>();
                info.Init();
            }
            //else
            //{
            //    info = go.AddComponent<InnerMeshRendererInfoEx>();
            //    info.Init();
            //}

            //Debug.Log($"AddComponent info:{info} info==null:{info == null}");
        }
        else
        {
            if (isForceUpdate)
            {
                info.Init();
            }

        }

        //var rId = RendererId.GetRId(go);
        //rId.RefreshParentId();

        //if (isUpdateId)
        //    RendererId.UpdateId(go);
        return info;
    }

    public static InnerMeshRendererInfoList InitRenderers(GameObject go)
    {
        //MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        //InnerMeshRendererInfoList list = new InnerMeshRendererInfoList(renderers);

        InnerMeshRendererInfoList list = new InnerMeshRendererInfoList(go);
        return list;
    }

    //public static List<InnerMeshRendererInfo> GetLod0s(GameObject go)
    //{
    //    var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
    //    List<InnerMeshRendererInfo> list = GetLod0s(renderers);
    //    return list;
    //}

    //public static List<InnerMeshRendererInfo> GetLod0s(MeshRenderer[] renderers)
    //{
    //    List<InnerMeshRendererInfo> list = new List<InnerMeshRendererInfo>();
    //    foreach (var renderer in renderers)
    //    {
    //        var info = InnerMeshRendererInfo.GetInfo(renderer.gameObject);
    //        if (info.LodIds.Contains(0) || info.LodIds.Contains(-1) || info.LodIds.Count==0)
    //        {
    //            list.Add(info);
    //        }
    //    }
    //    //var infos= go.GetComponentsInChildren<InnerMeshRendererInfo>(true);
    //    //list = infos.Where(i => i.LodId == 0|| i.LodId == -1).ToList();
    //    return list;
    //}

    public static InnerMeshRendererInfoList GetLodN(GameObject go, int lv)
    {
        var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        InnerMeshRendererInfoList list = GetLodN(renderers, lv);
        return list;
    }


    public static InnerMeshRendererInfoList GetLodN(MeshRenderer[] renderers, int lv)
    {
        InnerMeshRendererInfoList list = new InnerMeshRendererInfoList();
        foreach (var renderer in renderers)
        {
            //if (renderer.transform.parent != null)
            //{
            //    InnerMeshRendererInfoEx parentInfo = renderer.transform.parent.GetComponent<InnerMeshRendererInfoEx>();
            //    if (parentInfo != null && parentInfo.IsLodN(lv))
            //    {
            //        if (!list.Contains(parentInfo))
            //            list.Add(parentInfo);
            //    }
            //}

            var info = InnerMeshRendererInfo.GetInfo(renderer.gameObject, true);
            if (info.IsLodN(lv))
            {
                list.Add(info);
            }
        }
        return list;
    }

    public static InnerMeshRendererInfoList GetLodNs(GameObject go, params int[] lvs)
    {
        var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        //InitRenderers(renderers);
        InnerMeshRendererInfoList list1 = new InnerMeshRendererInfoList(renderers);
        var rendererInfos = go.GetComponentsInChildren<InnerMeshRendererInfo>(true);
        InnerMeshRendererInfoList list2 = GetLodNs(rendererInfos, lvs);
        return list2;
    }

    //public static InnerMeshRendererInfoList GetLodNs(MeshRenderer[] renderers, params int[] lvs)
    //{
    //    InnerMeshRendererInfoList list = new InnerMeshRendererInfoList();
    //    foreach (var renderer in renderers)
    //    {
    //        var info = InnerMeshRendererInfo.GetInfo(renderer.gameObject);
    //        if (info.IsLodNs(lvs))
    //        {
    //            list.Add(info);
    //        }
    //    }
    //    return list;
    //}

    public static InnerMeshRendererInfoList GetLodNs(InnerMeshRendererInfo[] infos, params int[] lvs)
    {
        InnerMeshRendererInfoList list = new InnerMeshRendererInfoList();
        foreach (var info in infos)
        {
            if (info.IsLodNs(lvs))
            {
                list.Add(info);
            }
        }
        return list;
    }

    //public static InnerMeshRendererInfoList InitRenderers(MeshRenderer[] renderers)
    //{
    //    InnerMeshRendererInfoList list = new InnerMeshRendererInfoList(renderers);

    //    return list;
    //}

    public static List<InnerMeshRendererInfoList> SplitByLOD(MeshRenderer[] renderers)
    {
        List<InnerMeshRendererInfoList> lodList = new List<InnerMeshRendererInfoList>();
        int max = 6;
        foreach (var renderer in renderers)
        {
            var info = InnerMeshRendererInfo.GetInfo(renderer.gameObject, true);
            for (int i = 0; i < max; i++)
            {
                if (info.IsLodN(i))//0,1,2,3
                {
                    if (lodList.Count <= i)
                    {
                        lodList.Add(new InnerMeshRendererInfoList());
                    }
                    lodList[i].Add(info);
                }
            }
        }
        return lodList;
    }

    public MeshRendererType rendererType;

    public bool IsRendererType(MeshRendererType rt)
    {
        return (rendererType & rt) == rt;
    }

    public void AddType(MeshRendererType rendererType)
    {
        this.rendererType = this.rendererType | rendererType;
    }

    public List<MeshRendererType> GetRendererTypes()
    {
        List<MeshRendererType> result = new List<MeshRendererType>();
        var types = Enum.GetValues(typeof(MeshRendererType));
        foreach (MeshRendererType t in types)
        {
            //Debug.Log($"{t} {t.GetType()}");
            if (IsRendererType(t))
            {
                result.Add(t);
            }
        }
        return result;
    }

    public string GetRendererTypesS()
    {
        string result = "";
        var types = Enum.GetValues(typeof(MeshRendererType));
        foreach (MeshRendererType t in types)
        {
            //Debug.Log($"{t} {t.GetType()}");
            if (IsRendererType(t))
            {
                result += t + ";";
            }
        }
        return result;
    }

    public bool IsRendererTypes(List<MeshRendererType> rts)
    {
        foreach (var rt in rts)
        {
            if (IsRendererType(rt))
            {
                return true;
            }
        }
        return false;
    }

    public static InnerMeshRendererInfoList FindByTypes(IEnumerable<MeshRenderer> renderers, List<MeshRendererType> types)
    {
        InnerMeshRendererInfoList list = new InnerMeshRendererInfoList();
        foreach (var renderer in renderers)
        {
            var info = InnerMeshRendererInfo.GetInfo(renderer.gameObject, true);
            //if (types.Contains(info.rendererType))
            info.IsRendererTypes(types);
            {
                list.Add(info);
            }
        }
        return list;
    }

    //[ContextMenu("GetRendererType")]
    //public void GetRendererType()
    //{
    //    bool isDetail = RendererManager.Instance.IsDetail(this.gameObject);
    //    if (isDetail)
    //    {
    //        rendererType = MeshRendererType.Detail;
    //    }
    //    Debug.Log("isDetail:" + isDetail);
    //}

    internal void HideRenderers()
    {
        foreach (var renderer in GetRenderers())
        {
            //renderer.gameObject.SetActive(false);
            renderer.enabled = false;
        }
    }

    //public bool IsChanged()
    //{
    //    return position != this.transform.position;
    //}

    [ContextMenu("Init")]
    public virtual void Init()
    {
        //Debug.Log("Init");
        //position=this.transform.position;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        meshFilter = gameObject.GetComponent<MeshFilter>();

        //if(meshRenderer=null)
        //    meshRenderer = GetRenderers()[0];
        //if(meshFilter==null)
        //    meshFilter = GetMeshFilters()[0];
        InitPos();

        //GetAssetInfo();
    }

    public Vector3 GetWeightCenterPos()
    {
        if (minMax.Length < 5)
        {
            InitPos();
        }
        return minMax[4];
    }

    protected void InitPos()
    {
        if (meshFilter != null)
        {
            if (meshFilter.sharedMesh != null)
            {
                vertexCount = meshFilter.sharedMesh.vertexCount;
                //minMax = MeshHelper.GetMinMax(meshFilter);
                minMax = VertexHelper.GetMinMax(GetMeshFilters());
                SetMinMax();
            }
            else
            {
                Debug.LogError($"InnerMeshRendererInfo.Init() meshFilter.sharedMesh==null:" + this.name);
            }
        }
        else
        {
            //Debug.LogError($"InnerMeshRendererInfo.Init() meshFilter==null:" + this.name);
            var filters = gameObject.GetComponentsInChildren<MeshFilter>(true);
            minMax = VertexHelper.GetMinMax(filters);
            SetMinMax();
        }
        // if(rendererType!=MeshRendererType.Detail)
        // {
        //     if(IsStatic()){
        //         rendererType=MeshRendererType.Static;
        //     }
        //     else{
        //         rendererType=MeshRendererType.None;
        //     }
        // }

        //if (transform.parent != null)
        //{
        //    InnerMeshRendererInfoEx parentInfo = transform.parent.GetComponent<InnerMeshRendererInfoEx>();
        //    if (parentInfo != null)
        //    {
        //        //this.rendererType = MeshRendererType.CombinedPart;
        //        this.AddType(MeshRendererType.CombinedPart);
        //    }
        //}
    }

    private void SetMinMax()
    {
        if (minMax != null && minMax.Length > 3)
        {
            center = minMax[3];
            size = minMax[2];
            disToCenter = Vector3.Distance(center, this.transform.position);
            diam = Vector3.Distance(minMax[0], minMax[1]);
        }
    }

    // public bool IsStatic()
    // {
    //     List<string> detailNames=new List<string>(){
    //         //"90 Degree Direction Change"
    //         };
    //     foreach(var dn in detailNames){
    //         if(this.name.Contains(dn) || this.transform.parent.name.Contains(dn))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    [ContextMenu("ShowBounds")]
    public void ShowBounds()
    {
        Debug.Log("ShowBounds");
        CreatePoint(minMax[0], "min");
        CreatePoint(minMax[1], "max");
        CreatePoint(minMax[2], "size");
        CreatePoint(minMax[3], "center");

        GameObject centerGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        centerGo.name = this.name + "_Bounds";
        centerGo.transform.position = minMax[3];
        centerGo.transform.localScale = minMax[2];
        centerGo.transform.SetParent(this.transform);
    }

    //[ContextMenu("ShowBounds2")]
    //public void ShowBounds2()
    //{
    //    var bounds = WorldSpaceTransitions.SectionSetup.GetBounds(this.gameObject);

    //    Debug.Log("ShowBounds");
    //    CreatePoint(bounds.min, "min");
    //    CreatePoint(bounds.max, "max");
    //    //CreatePoint(minMax[2], "size");
    //    CreatePoint(bounds.center, "center");

    //    GameObject centerGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    centerGo.name = this.name + "_Bounds";
    //    centerGo.transform.position = bounds.center;
    //    centerGo.transform.localScale = bounds.size;
    //    centerGo.transform.SetParent(this.transform);
    //}



    private void CreatePoint(Vector3 pos, string name)
    {
        GameObject centerGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerGo.name = name;
        centerGo.transform.position = pos;
        centerGo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        centerGo.transform.SetParent(this.transform);
    }

    [ContextMenu("ShowCenter")]
    public void ShowCenter()
    {
        Debug.Log("ShowCenter");
        //GameObject centerGo=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //centerGo.name=this.name+"_center";
        //centerGo.transform.position=center;
        //centerGo.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
        //centerGo.transform.SetParent(this.transform);

        CreatePoint(center, this.name + "_center");
    }

    [ContextMenu("ShowWeightCenter")]
    public void ShowWeightCenter()
    {
        Debug.Log("ShowWeightCenter");
        //GameObject centerGo=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //centerGo.name=this.name+"_center";
        //centerGo.transform.position=center;
        //centerGo.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
        //centerGo.transform.SetParent(this.transform);

        CreatePoint(minMax[4], this.name + "_weight");
    }

    [ContextMenu("CenterPivot")]
    public void CenterPivot()
    {
        var oldP = this.transform.parent;

        if (oldP != null)
        {
            float disToParent = Vector3.Distance(oldP.transform.position, center);
            Debug.Log("disToParent:" + disToParent);
            if (disToParent < 0.0001)//
            {
                Debug.Log("CenterPivot 0:" + this.name);
                return;
            }
        }
        if (oldP != null && oldP.childCount == 1 /* && oldP.GetComponents<Component>().Length==1 */)
        {

            Debug.Log("CenterPivot 1:" + this.name);
            this.transform.SetParent(null);
            oldP.position = center;
            this.transform.SetParent(oldP);
        }
        else
        {
            Debug.Log("CenterPivot 2:" + this.name);
            GameObject centerGo = new GameObject();
            centerGo.name = this.name + "_center";
            centerGo.transform.position = center;

            this.transform.SetParent(centerGo.transform);
            centerGo.transform.SetParent(oldP);
        }
    }

    internal void SetVisible(bool isVisible)
    {
        meshRenderer.enabled = isVisible;
        meshRenderer.gameObject.SetActive(isVisible);
    }

    [ContextMenu("TestCenterPivot")]
    public void TestCenterPivot()
    {
        CenterPivot(0.0001f);
    }

    public bool CenterPivot(float dis)
    {
        if (disToCenter > dis)
        {
            CenterPivot();
            return true;
        }
        else
        {
            Debug.Log($"No CenterPivot dis:{disToCenter} max:{dis}");
            return false;
        }
    }

    // public void OnDisable()
    // {
    //     //Debug.Log($"OnDisable {this.name} p:{transform.parent}");
    // }
    // public void OnTransformParentChanged()
    // {
    //     //Debug.Log($"OnTransformParentChanged {this.name} p:{transform.parent}");
    // }

    public List<int> LodIds = new List<int>();

    public string GetLODIds()
    {
        string txt = "";
        for (int i = 0; i < LodIds.Count; i++)
        {
            int ld = LodIds[i];
            txt += ld.ToString();
            if (i < LodIds.Count - 1)
            {
                txt += ";";
            }
        }
        return txt;
    }

    public bool IsLodN(int lv)
    {
        //if (lv == 0)
        //{
        //    return LodIds.Count == 0 || LodIds.Contains(0);
        //}
        //else
        //{
        //    return LodIds.Contains(lv);
        //}

        //if (IsRendererType(MeshRendererType.CombinedPart))
        //{
        //    return false;
        //}

        if (lv == -1)
        {
            return LodIds.Count == 0;
        }
        else
        {
            return LodIds.Contains(lv);
        }
    }

    [ContextMenu("TestIsLod00")]
    public bool TestIsLod00()
    {
        bool r = IsLodNs(new int[] { 0, -1 });
        Debug.LogError($"TestIsLod00 r:{r}");
        return r;
    }

    [ContextMenu("TestIsLod01")]
    public bool TestIsLod01()
    {
        bool r = IsLodNs(new int[] { 0 });
        Debug.LogError($"TestIsLod00 r:{r}");
        return r;
    }

    [ContextMenu("TestIsLod02")]
    public bool TestIsLod02()
    {
        bool r = IsLodNs(new int[] { -1 });
        Debug.LogError($"TestIsLod00 r:{r}");
        return r;
    }

    public bool IsLodNs(params int[] lvs)
    {
        this.GetRenderers();
        //if (IsRendererType(MeshRendererType.CombinedPart))
        //{
        //    return false;
        //}
        foreach (int lv in lvs)
        {
            if (IsLodN(lv)) return true;
        }
        return false;
    }

    public int CompareTo(InnerMeshRendererInfo other)
    {
        return other.vertexCount.CompareTo(this.vertexCount);
        //return this.vertexCount.CompareTo(other.vertexCount);
    }
}

[Serializable]
public class InnerMeshRendererInfoList : List<InnerMeshRendererInfo>
{
    public string GetInfoString()
    {
        string infoT = "";
        foreach (var item in this)
        {
            infoT += $"{item.meshRenderer.name};";
        }
        return infoT;
    }

    public InnerMeshRendererInfoList()
    {

    }
    public InnerMeshRendererInfoList(List<InnerMeshRendererInfo> list)
    {
        this.AddRange(list);
    }



    public InnerMeshRendererInfoList(GameObject root, bool isForceUpdate = false)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        InitRenderers(renderers, isForceUpdate);
    }

    public InnerMeshRendererInfoList(List<MeshRenderer> renderers, bool isForceUpdate = false)
    {
        InitRenderers(renderers, isForceUpdate);
    }

    public InnerMeshRendererInfoList(MeshRenderer[] renderers, bool isForceUpdate = false)
    {
        InitRenderers(renderers, isForceUpdate);
    }



    public InnerMeshRendererAssetInfoDict GetAssetPaths()
    {
        InnerMeshRendererAssetInfoDict dic = new InnerMeshRendererAssetInfoDict();
        foreach (var render in this)
        {
            dic.AddRenderer(render);
        }
        dic.SortByPath();
        return dic;
    }

    public InnerMeshRendererInfoList(List<Renderer> renderers, bool isForceUpdate = false)
    {
        InitRenderers(renderers, isForceUpdate);
    }

    public InnerMeshRendererInfoList(List<Transform> renderers, bool isForceUpdate = false)
    {
        InitRenderers(renderers, isForceUpdate);
    }

    public InnerMeshRendererInfoList(Renderer[] renderers, bool isForceUpdate = false)
    {
        InitRenderers(renderers, isForceUpdate);
    }

    private void InitRenderers<T>(List<T> renderers, bool isForceUpdate = false) where T : Component
    {
        //Debug.Log($"InnerMeshRendererInfo.InitRenderers_List renderers:{renderers.Count}");
        for (int i = 0; i < renderers.Count; i++)
        {
            T renderer = renderers[i];
            ProgressBarHelper.DisplayCancelableProgressBar("InitRenderers_List", i, renderers.Count, renderer);
            if (renderer == null) continue;
            var info = InnerMeshRendererInfo.GetInfo(renderer.gameObject, true, isForceUpdate);
            this.Add(info);
        }
        this.Sort();
        ProgressBarHelper.ClearProgressBar();
    }

    private void InitRenderers<T>(T[] renderers, bool isForceUpdate = false) where T : Component
    {
        if (renderers.Length == 0) return;
        //Debug.Log($"InnerMeshRendererInfo.InitRenderers_Array renderers:{renderers.Length}");
        for (int i = 0; i < renderers.Length; i++)
        {
            T renderer = renderers[i];
            ProgressBarHelper.DisplayCancelableProgressBar("InitRenderers_Array", i, renderers.Length, renderer);
            if (renderer == null) continue;
            var info = InnerMeshRendererInfo.GetInfo(renderer.gameObject, true, isForceUpdate);
            this.Add(info);
        }
        this.Sort();
        ProgressBarHelper.ClearProgressBar();
    }

    public List<MeshRenderer> GetRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach (var item in this)
        {
            if (item.meshRenderer == null)
            {
                Debug.LogError("Exception.MeshRenderInfo.GetRenders.render is null.");
                continue;
            }
            renderers.Add(item.meshRenderer);
        }
        return renderers;
    }

    public List<MeshRenderer> GetAllRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach (var item in this)
        {
            var rs = item.GetComponentsInChildren<MeshRenderer>(true);
            renderers.AddRange(rs);
        }
        return renderers;
    }

    public List<Transform> GetTransforms()
    {
        List<Transform> renderers = new List<Transform>();
        foreach (var item in this)
        {
            //var rs = item.GetComponentsInChildren<Transform>(true);
            //renderers.AddRange(rs);
            var t = item.GetComponent<Transform>();
            renderers.Add(t);
        }
        return renderers;
    }

    public MeshRenderer[][] GetRenderersArray()
    {
        List<MeshRenderer[]> renderersList = new List<MeshRenderer[]>();
        foreach (var item in this)
        {
            renderersList.Add(item.GetRenderers());
        }
        return renderersList.ToArray();
    }

    public int Length
    {
        get
        {
            return this.Count;
        }
    }

    public static Bounds CaculateBounds(InnerMeshRendererInfoList renders, bool isAll = true)
    {
        //Debug.Log($"CaculateBounds renders:{renders.Count()},isAll:{isAll}");
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (var info in renders)
        {
            var child = info.meshRenderer;
            if (child == null) continue;
            if (isAll == false && !child.enabled) continue;

            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertexCount == 0)
            {
                Debug.LogWarning($"CaculateBounds1 meshFilter.sharedMesh==null || meshFilter.sharedMesh.vertexCount==0 name:{child.name} path:{GetPath(child.transform, 1000)}");
                continue;
            }

            center += child.bounds.center;
            count++;
        }

        if (count > 0)
        {
            center /= count;
        }
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (var info in renders)
        {
            var child = info.meshRenderer;
            if (isAll == false && !child.enabled) continue;

            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertexCount == 0)
            {
                Debug.LogWarning($"CaculateBounds2 meshFilter.sharedMesh==null || meshFilter.sharedMesh.vertexCount==0 name:{child.name} path:{GetPath(child.transform, 1000)}");
                continue;
            }

            // Bounds bounds1=bounds;
            bounds.Encapsulate(child.bounds);
            //Debug.Log($"CaculateBounds bounds1:{bounds},bounds2:{child.bounds} name:{child.name}");
            // if(bounds.size!=bounds1.size)
            // {
            //     Debug.Log($"CaculateBounds bounds1:{bounds},bounds2:{child.bounds} name:{child.name} path:{GetPath(child.transform,1000)}");
            //     AreaTreeHelper.CreateBoundsCube(bounds, GetPath(child.transform,2), null);
            // }

        }
        return bounds;
    }

    private static string GetPath(Transform t, int maxlevel)
    {
        if (t.parent == null || maxlevel <= 0)
        {
            return t.name;
        }
        else
        {
            return GetPath(t.parent, maxlevel - 1) + "/" + t.name;
        }
    }

    internal void SetType(MeshRendererType rendererType)
    {
        foreach (var item in this)
        {
            item.rendererType = rendererType;
        }
    }

    internal void AddType(MeshRendererType rendererType)
    {
        foreach (var item in this)
        {
            //Debug.Log($"AddType render:{item.name} old:{item.rendererType} add:{rendererType} new:{item.rendererType | rendererType}");
            item.AddType(rendererType);
        }
    }

    internal void RemoveTypes(List<MeshRendererType> list, string logTag)
    {
        if (this.Count == 0) return;
        if (list == null || list.Count == 0) return;
        int count1 = this.Count;

        //string types = "";
        //foreach(var i in list)
        //{
        //    types += i + ";";
        //}
        for (int i = 0; i < this.Count; i++)
        {
            var info = this[i];
            //Debug.Log($"info:{info} type:{info.rendererType} int:{(int)(info.rendererType)}");
            if (list.Contains(info.rendererType))
            {
                this.RemoveAt(i);
                i--;
            }
        }
        int count2 = this.Count;

        //Debug.Log($"MeshRenderInfoList.RemoveTypes[{logTag}] count1:{count1} count2:{count2} types:{list.Count}|{types}");
    }

    internal void FilterByVertexCount(float v)
    {
        int c1 = this.Count;
        for (int i = 0; i < this.Count; i++)
        {
            InnerMeshRendererInfo info = this[i];
            if (info.vertexCount > v)
            {
                this.RemoveAt(i);
                i--;
            }
        }
        int c2 = this.Count;
        if (c1 != c2)
        {
            Debug.LogError($"FilterByVertexCount vertexcount:{v} count1:{c1} count2:{c2}");
        }
    }

    internal InnerMeshRendererInfoList[] SplitListByVertexCount(float v, string tag)
    {
        InnerMeshRendererInfoList list1 = new InnerMeshRendererInfoList();
        InnerMeshRendererInfoList list2 = new InnerMeshRendererInfoList();
        for (int i = 0; i < this.Count; i++)
        {
            InnerMeshRendererInfo info = this[i];
            if (v > 0 && info.vertexCount > v)
            {
                list2.Add(info);
            }
            else
            {
                list1.Add(info);
            }
        }
        if (list2.Count > 0)
        {
            Debug.LogWarning($"SplitListByVertexCount tag:{tag} vertexcount:{v} count0:{this.Count} count1:{list1.Count} count2:{list2.Count}");
        }

        return new InnerMeshRendererInfoList[] { list1, list2 };
    }

    //internal void CloneTo(GameObject newParent)
    //{
    //    for (int i = 0; i < this.Count; i++)
    //    {
    //        InnerMeshRendererInfo info = this[i];
    //        var cloneObj = MeshHelper.CopyGO(info.gameObject);
    //        cloneObj.name = info.gameObject.name + "_CombinedClone";
    //        cloneObj.transform.SetParent(newParent.transform);
    //    }
    //    RendererId.InitIds(newParent);
    //}

    internal void SetRendererType(MeshRendererType rendererType)
    {
        for (int i = 0; i < this.Count; i++)
        {
            InnerMeshRendererInfo info = this[i];
            info.rendererType = rendererType;
        }
    }

    internal List<Renderer> GetBoundsRenderers()
    {
        List<Renderer> renderers = new List<Renderer>();
        for (int i = 0; i < this.Count; i++)
        {
            InnerMeshRendererInfo info = this[i];
            renderers.Add(info.GetBoundsRenderer());
        }
        return renderers;
    }

    internal InnerMeshRendererInfoList GetLODs(params int[] lvs)
    {
        InnerMeshRendererInfoList list = new InnerMeshRendererInfoList();
        foreach (var info in this)
        {
            if (info.IsLodNs(lvs))
            {
                list.Add(info);
            }
        }
        return list;
    }

    //public static InnerMeshRendererInfoList GetLodNs(InnerMeshRendererInfo[] infos, params int[] lvs)
    //{
    //    InnerMeshRendererInfoList list = new InnerMeshRendererInfoList();
    //    foreach (var info in infos)
    //    {
    //        if (info.IsLodNs(lvs))
    //        {
    //            list.Add(info);
    //        }
    //    }
    //    return list;
    //}

    internal void RemoveLODs()
    {
        throw new NotImplementedException();
    }

    internal InnerMeshRendererInfoList FilterRenderersByFile(List<string> filterFiles, bool isEnableFilter)
    {
        var modelFiles = this.GetAssetPaths();
        var modelFilePaths = modelFiles.Keys.ToList();
        modelFilePaths.Sort();
        InnerMeshRendererInfoList list = new InnerMeshRendererInfoList();
        foreach (var key in modelFiles.Keys)
        {
            if (isEnableFilter && IsFilter(key, filterFiles))
            {
                continue;
            }
            list.AddRange(modelFiles[key]);
        }
        return list;
    }

    private bool IsFilter(string fileName, List<string> filterKeys)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        foreach (var f in filterKeys)
        {
            if (fileName.Contains(f))
            {
                return true;
            }
        }
        return false;
    }

    private float vertexCount = 0;

    public float GetVertexCount()
    {
        if (vertexCount > 0)
        {
            return vertexCount;
        }
        foreach (var item in this)
        {
            vertexCount += (int)item.vertexCount;
        }
        return vertexCount;
    }

    public void AddCollider()
    {
        foreach (var render in this)
        {
            render.AddCollider();
        }
    }

    ///lodManager.SetRenderersLODInfo();
    ///
    public void SetLODInfo()
    {

    }

    public void AddTransform(Transform t)
    {
        InnerMeshRendererInfo mr = InnerMeshRendererInfo.GetInfo(t.gameObject);
        base.Add(mr);
    }

    public string GetCountVertex()
    {
        return $"{Count},v:{VertexHelper.GetVertexCountS(GetVertexCount())}";
    }

    public string GetCountVertex(float total)
    {
        if (Count == 0)
        {
            return "--";
        }
        return $"{Count},v:{VertexHelper.GetVertexCountS(GetVertexCount())}({GetVertexCount() / total:P1})";
    }
}

public class InnerMeshRendererAssetInfo : InnerMeshRendererInfoList
{
    public string FilePath;
    public UnityEngine.Object AssetOjb;
    //public InnerMeshRendererInfoList List;

    public InnerMeshRendererAssetInfo(string path)
    {
        FilePath = path;
    }
}

public class InnerMeshRendererAssetInfoDict : Dictionary<string, InnerMeshRendererAssetInfo>
{
    public void SortByPath()
    {
        var paths = this.Keys.ToList();
        paths.Sort();
        List<InnerMeshRendererAssetInfo> list = new List<InnerMeshRendererAssetInfo>();
        foreach (var path in paths)
        {
            list.Add(this[path]);
        }
        SetList(list);
    }

    private void SetList(List<InnerMeshRendererAssetInfo> list)
    {
        this.Clear();
        foreach (var item in list)
        {
            this.Add(item.FilePath, item);
        }
    }

    public void SortByVertex()
    {
        List<InnerMeshRendererAssetInfo> list = this.Values.ToList();
        list.Sort((a, b) => { return b.GetVertexCount().CompareTo(a.GetVertexCount()); });
        SetList(list);
    }

    public void AddRenderer(InnerMeshRendererInfo render)
    {
        var path = render.GetAssetPath();
        //if (string.IsNullOrEmpty(path))
        //{
        //    Debug.LogError("path==null:" + render.name);
        //    continue;
        //}
        if (!this.ContainsKey(path))
        {
            this.Add(path, new InnerMeshRendererAssetInfo(path));
        }
        this[path].Add(render);
    }
}

public enum MeshRendererType
{
    None = 1,
    Structure = 2,//(Big)
    Detail = 4,//(Small)
    Static = 8,
    LOD = 16,
    CombinedPart = 32,
    CombinedRoot = 64,
    Splited = 128
}
