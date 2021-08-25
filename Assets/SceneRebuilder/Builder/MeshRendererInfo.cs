using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshRendererInfo : MonoBehaviour,IComparable<MeshRendererInfo>
{
    public Vector3 position;

    public Vector3 center;

    public float disToCenter;

    public float diam = 0;

    public float GetDiam()
    {
        if (diam==0)
        {
            diam = Vector3.Distance(minMax[0], minMax[1]);
        }
        return diam;
    }

    public Vector3 size;

    public float vertexCount;

    // public Bounds bounds;

    public MeshFilter meshFilter;

    public MeshRenderer meshRenderer;

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
        cube.AddComponent<BoundsBox>();
        cube.SetActive(true);
        cube.name = this.name+"_Bounds";
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
        foreach(var filter in filters)
        {
            count += filter.sharedMesh.vertexCount;
        }
        return count;
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
            var lods=group.GetLODs();
            var renderers=lods[lods.Length - 1].renderers;
            int count = 0;
            foreach(var render in renderers)
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

    public static Vector3[] GetMinMax(GameObject go,bool isUpdate=true)
    {
        MeshRendererInfo info = go.GetComponent<MeshRendererInfo>();
        if (info == null)
        {
            info = go.AddComponent<MeshRendererInfo>();
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

    public static MeshRendererInfo GetInfo(GameObject go)
    {
        //Debug.Log($"MeshRendererInfo go:{go}");
        MeshRendererInfo info = go.GetComponent<MeshRendererInfo>();
        //Debug.Log($"info:{info} go==null:{go == null}");
        if (info == null)
        {
            info = go.AddComponent<MeshRendererInfo>();
            info.Init();
            //Debug.Log($"AddComponent info:{info} info==null:{info == null}");
        }
        RendererId.UpdateId(go);
        return info;
    }

   public static MeshRendererInfoList InitRenderers(GameObject go)
    {
        //MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        //MeshRendererInfoList list = new MeshRendererInfoList(renderers);

        MeshRendererInfoList list = new MeshRendererInfoList(go);
        return list;
    }

    //public static List<MeshRendererInfo> GetLod0s(GameObject go)
    //{
    //    var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
    //    List<MeshRendererInfo> list = GetLod0s(renderers);
    //    return list;
    //}

    //public static List<MeshRendererInfo> GetLod0s(MeshRenderer[] renderers)
    //{
    //    List<MeshRendererInfo> list = new List<MeshRendererInfo>();
    //    foreach (var renderer in renderers)
    //    {
    //        var info = MeshRendererInfo.GetInfo(renderer.gameObject);
    //        if (info.LodIds.Contains(0) || info.LodIds.Contains(-1) || info.LodIds.Count==0)
    //        {
    //            list.Add(info);
    //        }
    //    }
    //    //var infos= go.GetComponentsInChildren<MeshRendererInfo>(true);
    //    //list = infos.Where(i => i.LodId == 0|| i.LodId == -1).ToList();
    //    return list;
    //}

    public static MeshRendererInfoList GetLodN(GameObject go, int lv)
    {
        var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        MeshRendererInfoList list = GetLodN(renderers, lv);
        return list;
    }


    public static MeshRendererInfoList GetLodN(MeshRenderer[] renderers,int lv)
    {
        MeshRendererInfoList list = new MeshRendererInfoList();
        foreach (var renderer in renderers)
        {
            if (renderer.transform.parent != null)
            {
                MeshRendererInfoEx parentInfo = renderer.transform.parent.GetComponent<MeshRendererInfoEx>();
                if (parentInfo!=null && parentInfo.IsLodN(lv))
                {
                    if(!list.Contains(parentInfo))
                        list.Add(parentInfo);
                }
            }

            var info = MeshRendererInfo.GetInfo(renderer.gameObject);
            if (info.IsLodN(lv))
            {
                list.Add(info);
            }
        }
        return list;
    }

    public static MeshRendererInfoList GetLodNs(GameObject go, params int[] lvs)
    {
        var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        //InitRenderers(renderers);
        MeshRendererInfoList list1 = new MeshRendererInfoList(renderers);
        var rendererInfos = go.GetComponentsInChildren<MeshRendererInfo>(true);
        MeshRendererInfoList list2 = GetLodNs(rendererInfos, lvs);
        return list2;
    }

    //public static MeshRendererInfoList GetLodNs(MeshRenderer[] renderers, params int[] lvs)
    //{
    //    MeshRendererInfoList list = new MeshRendererInfoList();
    //    foreach (var renderer in renderers)
    //    {
    //        var info = MeshRendererInfo.GetInfo(renderer.gameObject);
    //        if (info.IsLodNs(lvs))
    //        {
    //            list.Add(info);
    //        }
    //    }
    //    return list;
    //}

    public static MeshRendererInfoList GetLodNs(MeshRendererInfo[] infos, params int[] lvs)
    {
        MeshRendererInfoList list = new MeshRendererInfoList();
        foreach (var info in infos)
        {
            if (info.IsLodNs(lvs))
            {
                list.Add(info);
            }
        }
        return list;
    }

    //public static MeshRendererInfoList InitRenderers(MeshRenderer[] renderers)
    //{
    //    MeshRendererInfoList list = new MeshRendererInfoList(renderers);

    //    return list;
    //}

    public static List<MeshRendererInfoList> SplitByLOD(MeshRenderer[] renderers)
    {
        List<MeshRendererInfoList> lodList = new List<MeshRendererInfoList>();
        int max = 6;
        foreach (var renderer in renderers)
        {
            var info = MeshRendererInfo.GetInfo(renderer.gameObject);
            for(int i=0;i< max; i++)
            {
                if (info.IsLodN(i))//0,1,2,3
                {
                    if (lodList.Count <= i)
                    {
                        lodList.Add(new MeshRendererInfoList());
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

    public bool IsRendererTypes(List<MeshRendererType> rts)
    {
        foreach(var rt in rts)
        {
            if(IsRendererType(rt))
            {
                return true;
            }
        }
        return false;
    }

    public static MeshRendererInfoList FindByTypes(IEnumerable<MeshRenderer> renderers,List<MeshRendererType> types)
    {
        MeshRendererInfoList list = new MeshRendererInfoList();
        foreach (var renderer in renderers)
        {
            var info = MeshRendererInfo.GetInfo(renderer.gameObject);
            //if (types.Contains(info.rendererType))
            info.IsRendererTypes(types);
            {
                list.Add(info);
            }
        }
        return list;
    }

    [ContextMenu("GetRendererType")]
    public void GetRendererType()
    {
        bool isDetail=RendererManager.Instance.IsDetail(this.gameObject);
        if(isDetail)
        {
            rendererType=MeshRendererType.Detail;
        }
        Debug.Log("isDetail:"+isDetail);
    }

    internal void HideRenderers()
    {
        foreach(var renderer in GetRenderers())
        {
            //renderer.gameObject.SetActive(false);
            renderer.enabled = false;
        }
    }

    public bool IsChanged()
    {
        return position != this.transform.position;
    }

    [ContextMenu("Init")]
    public virtual void Init()
    {
        //Debug.Log("Init");
        position=this.transform.position;
        meshRenderer=gameObject.GetComponent<MeshRenderer>();

        meshFilter=gameObject.GetComponent<MeshFilter>();

        //if(meshRenderer=null)
        //    meshRenderer = GetRenderers()[0];
        //if(meshFilter==null)
        //    meshFilter = GetMeshFilters()[0];
        InitPos();
    }

    public Vector3 GetWeightCenterPos()
    {
        if(minMax.Length<4)
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
                minMax = MeshHelper.GetMinMax(GetMeshFilters());
                if (minMax != null && minMax.Length > 3)
                {
                    center = minMax[3];
                    size = minMax[2];
                    disToCenter = Vector3.Distance(center, position);
                    diam = Vector3.Distance(minMax[0], minMax[1]);
                }
            }
            else
            {
                Debug.LogError($"MeshRendererInfo.Init() meshFilter.sharedMesh==null:" + this.name);
            }
        }
        else
        {
            Debug.LogError($"MeshRendererInfo.Init() meshFilter==null:" + this.name);
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

        if (transform.parent != null)
        {
            MeshRendererInfoEx parentInfo = transform.parent.GetComponent<MeshRendererInfoEx>();
            if (parentInfo != null)
            {
                this.rendererType = MeshRendererType.CombinedPart;
            }
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
        CreatePoint(minMax[0],"min");
        CreatePoint(minMax[1],"max");
        CreatePoint(minMax[2],"size");
        CreatePoint(minMax[3],"center");

        GameObject centerGo=GameObject.CreatePrimitive(PrimitiveType.Cube);
        centerGo.name=this.name+"_Bounds";
        centerGo.transform.position=minMax[3];
        centerGo.transform.localScale=minMax[2];
        centerGo.transform.SetParent(this.transform);
    }

    private void CreatePoint(Vector3 pos,string name)
    {
        GameObject centerGo=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerGo.name=name;
        centerGo.transform.position=pos;
        centerGo.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
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
        var oldP=this.transform.parent;
        
        if(oldP!=null){
            float disToParent=Vector3.Distance(oldP.transform.position,center);
            Debug.Log("disToParent:"+disToParent);
            if(disToParent<0.0001)//
            {
                Debug.Log("CenterPivot 0:"+this.name);
                return;
            }
        }
        if(oldP!=null && oldP.childCount==1 /* && oldP.GetComponents<Component>().Length==1 */){

            Debug.Log("CenterPivot 1:"+this.name);
            this.transform.SetParent(null);
            oldP.position=center;
            this.transform.SetParent(oldP);
        }
        else{
            Debug.Log("CenterPivot 2:"+this.name);
            GameObject centerGo=new GameObject();
            centerGo.name=this.name+"_center";
            centerGo.transform.position=center;
            
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
        if(disToCenter>dis)
        {
            CenterPivot();
            return true;
        }
        else{
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
            if(i< LodIds.Count-1)
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

        if (IsRendererType(MeshRendererType.CombinedPart))
        {
            return false;
        }

        if (lv == -1)
        {
            return LodIds.Count == 0;
        }
        else
        {
            return LodIds.Contains(lv);
        }
    }

    public bool IsLodNs(int[] lvs)
    {
        this.GetRenderers();
        if (IsRendererType(MeshRendererType.CombinedPart))
        {
            return false;
        }
        foreach(int lv in lvs)
        {
            if (IsLodN(lv)) return true;
        }
        return false;
    }

    public int CompareTo(MeshRendererInfo other)
    {
        return other.vertexCount.CompareTo(this.vertexCount);
        //return this.vertexCount.CompareTo(other.vertexCount);
    }
}

[Serializable]
public class MeshRendererInfoList:List<MeshRendererInfo>
{
    public MeshRendererInfoList()
    {

    }

    public MeshRendererInfoList(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        InitRenderers(renderers);
    }

    public MeshRendererInfoList(IEnumerable<MeshRenderer> renderers)
    {
        InitRenderers(renderers);
    }

    private void InitRenderers(IEnumerable<MeshRenderer> renderers)
    {
        foreach (var renderer in renderers)
        {
            var info = MeshRendererInfo.GetInfo(renderer.gameObject);
            this.Add(info);
        }
        this.Sort();
    }

    public List<MeshRenderer> GetRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach(var item in this)
        {
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

    public static Bounds CaculateBounds(MeshRendererInfoList renders, bool isAll = true)
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
        foreach(var item in this)
        {
            item.rendererType = rendererType;
        }
    }

    internal void AddType(MeshRendererType rendererType)
    {
        foreach (var item in this)
        {
            Debug.Log($"AddType render:{item.name} old:{item.rendererType} add:{rendererType} new:{item.rendererType | rendererType}");
            item.rendererType = item.rendererType | rendererType;
            
        }
    }

    internal void RemoveTypes(List<MeshRendererType> list,string logTag)
    {
        if (this.Count == 0) return;
        if (list==null || list.Count== 0) return;
        int count1 = this.Count;
        
        for(int i = 0; i < this.Count; i++)
        {
            if(list.Contains(this[i].rendererType))
            {
                this.RemoveAt(i);
                i--;
            }
        }
        int count2 = this.Count;

        Debug.Log($"MeshRenderInfoList.RemoveTypes[{logTag}] count1:{count1} count2:{count2} types:{list.Count}");
    }

    internal void FilterByVertexCount(float v)
    {
        int c1 = this.Count;
        for (int i = 0; i < this.Count; i++)
        {
            MeshRendererInfo info = this[i];
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

    internal MeshRendererInfoList[] SplitListByVertexCount(float v,string tag)
    {
        MeshRendererInfoList list1 = new MeshRendererInfoList();
        MeshRendererInfoList list2 = new MeshRendererInfoList();
        for (int i = 0; i < this.Count; i++)
        {
            MeshRendererInfo info = this[i];
            if (v>0 && info.vertexCount > v)
            {
                list2.Add(info);
            }
            else
            {
                list1.Add(info);
            }
        }
        if(list2.Count>0)
        {
            Debug.LogWarning($"SplitListByVertexCount tag:{tag} vertexcount:{v} count0:{this.Count} count1:{list1.Count} count2:{list2.Count}");
        }
        
        return new MeshRendererInfoList[]{ list1, list2 };
    }

    internal void CloneTo(GameObject newParent)
    {
        for (int i = 0; i < this.Count; i++)
        {
            MeshRendererInfo info = this[i];
            var cloneObj = MeshHelper.CopyGO(info.gameObject);
            cloneObj.name = info.gameObject.name+"_CombinedClone";
            cloneObj.transform.SetParent(newParent.transform);
        }
        RendererId.InitIds(newParent);
    }

    internal void SetRendererType(MeshRendererType rendererType)
    {
        for (int i = 0; i < this.Count; i++)
        {
            MeshRendererInfo info = this[i];
            info.rendererType = rendererType;
        }
    }
}

public enum MeshRendererType
{
    None=1,
    Structure=2,//(Big)
    Detail=4,//(Small)
    Static=8,
    LOD=16,
    CombinedPart=32,
    CombinedRoot=64,
    Splited=128
}
