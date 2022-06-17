using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CommonUtils;
//using MeshJobs;

public class AreaTreeNode : SubSceneCreater
{
    public override string ToString()
    {
        return this.name;
    }
    public ModelAreaTree tree;

    public Bounds Bounds;

    public GameObject renderersRoot;

    public GameObject collidersRoot;

    public GameObject combindResult;

    public bool IsLeaf = false;

    public List<MeshRenderer> Renderers = new List<MeshRenderer>();

    public List<string> RenderersId = new List<string>();

    public List<MeshRendererInfo> GetRendererInfos()
    {
        List<MeshRendererInfo> list = new List<MeshRendererInfo>();
        foreach (var renderer in Renderers)
        {
            if (renderer == null) continue;
            MeshRendererInfo info = MeshRendererInfo.GetInfo(renderer.gameObject, false);
            list.Add(info);
        }
        return list;
    }

    //public int GetRendererCount()
    //{
    //    int i = 0;
    //    if (Renderers != null)
    //        foreach (var r in Renderers)
    //        {
    //            if (r != null)
    //            {
    //                i++;
    //            }
    //        }
    //    return i;
    //}

    public MeshRenderer[] CombinedRenderers;
    public List<string> CombinedRenderersId = new List<string>();

    // private List<Transform> RendererParents = new List<Transform>();

    //private List<MeshRenderer> newRenderers = new List<MeshRenderer>();
    //public List<string> newRenderersId = new List<string>();

    [ContextMenu("InitRenderersDict")]
    public void InitRenderersDict()
    {
        IdDictionary.InitInfos();
    }

    [ContextMenu("SaveRenderersId")]
    public void SaveRenderersId()
    {

        RenderersId.Clear();

        foreach (var renderer in Renderers)
        {
            if (renderer == null) continue;
            var id = RendererId.GetRId(renderer);
            RenderersId.Add(id.Id);
        }

        CombinedRenderersId.Clear();
        if (CombinedRenderers != null)
            foreach (var renderer in CombinedRenderers)
            {
                if (renderer == null) continue;
                var id = RendererId.GetRId(renderer);
                CombinedRenderersId.Add(id.Id);
            }
    }

    [ContextMenu("TestLoadRenderers")]
    public void TestLoadRenderers()
    {
        var rs1 = IdDictionary.GetRenderers(RenderersId,this.transform);
        if (rs1.Count == RenderersId.Count)//数量没变说明找到了全部的Renderers
        {
            Renderers = rs1;
        }
        else
        {
            Debug.LogError($"LoadRenderers rs1.Count != RenderersId.Count [{this.name}]");
        }

        //var rs2 = IdDictionary.GetRenderers(newRenderersId);
        //if (rs2.Count == newRenderersId.Count)//数量没变说明找到了全部的Renderers
        //{
        //    newRenderers = rs2;
        //}
        //else
        //{
        //    Debug.LogError($"LoadRenderers rs2.Count != newRenderersId.Count [{this.name}]");
        //}
        //newRenderers = rs2;
    }

    public int GetRendererCount(IEnumerable<MeshRenderer> rs)
    {
        int count = 0;
        if(rs!=null)
            foreach(var r in rs)
            {
                if (r != null) count++;
            }
        return count;
    }

    [ContextMenu("SortRenderers")]
    public void SortRenderers()
    {
        RenderersId.Sort();
        Renderers.RemoveAll(i => i == null);
        Renderers.Sort((a,b)=> { return a.name.CompareTo(b.name); });
    }

    public int LoadRenderers_Renderers()
    {
        // Debug.LogError("LoadRenderers_Renderers:"+this.name);
        int count1 = GetRendererCount(Renderers);
        if(count1!= RenderersId.Count)
        {
            var rs1 = IdDictionary.GetRenderers(RenderersId,this.transform);
            if (rs1.Count == RenderersId.Count)//数量没变说明找到了全部的Renderers
            {
                Renderers = rs1;
            }
            else
            {
                Debug.LogError($"AreaTreeNode.LoadRenderers_Renderers rs1.Count != RenderersId.Count [{this.name}][{this.tree}][ids:{RenderersId.Count}][renderers:{rs1.Count}]");
                Renderers = rs1;
            }
        }
        else
        {
            //Debug.Log($"AreaTreeNode.LoadRenderers count1 == RenderersId.Count [{this.name}][{this.tree}][renderers:{count1}]");
        }

        count1 = GetRendererCount(Renderers);
        return count1;
    }

    public int LoadRenderers_Combined()
    {
        
        // Debug.LogError("LoadRenderers_Combined:"+this.name);
        int count3 = GetRendererCount(CombinedRenderers);
        if (count3 != CombinedRenderersId.Count)
        {
            var rs3 = IdDictionary.GetRenderers(CombinedRenderersId,this.transform);
            if (rs3.Count == CombinedRenderersId.Count)//数量没变说明找到了全部的Renderers
            {
                CombinedRenderers = rs3.ToArray();
            }
            else
            {
                Debug.LogError($"LoadRenderers rs3.Count != CombinedRenderersId.Count  [{rs3.Count}][{CombinedRenderersId.Count}] [{this.name}][{this.transform.parent}][{tree.name}]");
            }
        }
        else
        {
            //Debug.Log($"AreaTreeNode.LoadRenderers count3 == CombinedRenderersId.Count [{this.name}][{this.transform.parent}][{tree.name}][{count3}]");
        }

        count3 = GetRendererCount(CombinedRenderers);
        return count3;
    }

    public int LoadRenderers()
    {
        int c1=LoadRenderers_Renderers();
        int c2=LoadRenderers_Combined();
        return c1 + c2;
    }


    public void LoadRenderers(GameObject go)
    {
        if(go==renderersRoot)
        {
            LoadRenderers_Renderers();
        }
        if(go==combindResult)
        {
            LoadRenderers_Combined();
        }
    }
    //public List<Collider> colliders = new List<Collider>();

    public List<AreaTreeNode> Nodes = new List<AreaTreeNode>();

    public int RendererCount
    {
        get
        {
            return Renderers.Count;
        }
    }

    public float VertexCount = 0;

    public float GetVertexCount()
    {
        int count = 0;
        foreach (var renderer in Renderers)
        {
            if (renderer == null) continue;
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            if (meshFilter.sharedMesh == null)
            {
                Debug.LogError("meshFilter.sharedMesh == null :"+meshFilter);
                continue;
            }
            count += meshFilter.sharedMesh.vertexCount;
        }
        VertexCount = count / 10000.0f;
        return VertexCount;
    }

    public List<MeshRenderer> GetRenderers()
    {
        return Renderers;
    }

    public void AddRendererAfterLoadScene(RendererId rId)
    {
        var root = GetRenderersRoot();
        rId.transform.SetParent(root.transform);
    }

    public void AddRenderer(MeshRenderer renderer)
    {
        if (!Renderers.Contains(renderer))
        {
            Renderers.Add(renderer);
        }
        else
        {
            Debug.LogError("AddRenderer Renderers.Contains(renderer):" + renderer);
        }

    }

    public void AddRenderers(List<MeshRenderer> renderers)
    {
        Renderers.AddRange(renderers);
    }

    public void AddRenderers(MeshRendererInfoList rendererInfos)
    {
        var renderers = rendererInfos.GetAllRenderers();
        Renderers.AddRange(renderers);
    }

    //public static bool TestDebug = true;

    //private bool IsCopyed = false;

    public GameObject GetRenderersRoot()
    {
        try
        {
            if (this == null)
            {
                Debug.LogError($"AreaTreeNode.GetRenderersRoot this == null");
                if (renderersRoot == null)
                    renderersRoot = new GameObject("NULLError_Renderers");
                return renderersRoot;
            }

            if (this.gameObject==null)
            {
                Debug.LogError($"AreaTreeNode.GetRenderersRoot gameObject == null");
                if (renderersRoot == null)
                    renderersRoot = new GameObject("NULLError_Renderers");
                return renderersRoot;
            }

            if (renderersRoot == null)
            {
                renderersRoot = new GameObject(this.name + "_Renderers");
                //if (tree.name.Contains("_GPUI_SmallTree"))
                //{
                //    EnableDebug debug=renderersRoot.AddComponent<EnableDebug>();
                //    debug.DebugDestroy = false;
                //    debug.DebugEnable = false;
                //}
            }
            return renderersRoot;
        }
        catch (Exception ex)
        {
            Debug.LogError($"AreaTreeNode.GetRenderersRoot Exception:{ex}");
            if (renderersRoot == null)
                renderersRoot = new GameObject("NULLError_Renderers");
            return renderersRoot;
        }
        
    }

    [ContextMenu("InitRenderers")]
    public List<RendererId> InitRenderers()
    {
        GetRenderersRoot();
        List<RendererId> ids = new List<RendererId>();
        //renderersRoot.transform.SetParent(this.transform);
        foreach (var render in Renderers)
        {
            if (render == null) continue;
            GameObject go = render.gameObject;
            //newRenderers.Add(render);
            var rId=RendererId.GetRId(render);
            MeshCollider collider = go.GetComponent<MeshCollider>();
            //if (collider)
            //    collider.enabled = false;
            //colliders.Add(collider);
            ids.Add(rId);
        }
        return ids;
    }

    public List<RendererId> MoveRenderers()
    {
        //Debug.Log($"AreaTreeNode.MoveRenderers tree:{tree.name} node:{this.name}");

        List<RendererId> ids=InitRenderers();

        //newRenderers.Clear();
        // RendererParents.Clear();

        //newRenderers.Clear();
        GetRenderersRoot();
        foreach (var render in Renderers)
        {
            if (render == null)
            {
                Debug.LogError("MoveRenderers render==null:" + this);
                continue;
            }
            GameObject go = render.gameObject;
            //newRenderers.Add(render);
            if (go.transform.parent != renderersRoot.transform)
            {
                // RendererParents.Add(go.transform.parent);
                EditorHelper.UnpackPrefab(go);
                go.transform.SetParent(renderersRoot.transform);
            }
        }

        EditorHelper.UnpackPrefab(renderersRoot);
        renderersRoot.transform.SetParent(this.transform);

        return ids;
    }

    private void CreateColliders()
    {
        collidersRoot = MeshHelper.CopyGO(renderersRoot);
        collidersRoot.name = this.name + "_Colliders";
        var renderers = collidersRoot.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            GameObject.DestroyImmediate(renderer);
        }
        collidersRoot.transform.SetParent(this.transform);

        var colliders = renderersRoot.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            GameObject.DestroyImmediate(collider);
        }
    }

    public float DistanceToCamera = 0;

    public bool IsRendererVisible = true;

    [ContextMenu("HideRenders")]
    public void HideRenders(MeshRendererInfoList renderers)
    {
        IsRendererVisible = false;
        foreach (MeshRendererInfo r in renderers)
        {
            if (r == null) continue;
            //r.enabled = false;
            //r.HideRenderers();
            r.SetVisible(false);
        }
    }

    [ContextMenu("ShowRenders")]
    public void ShowRenders()
    {
        IsRendererVisible = true;
        foreach (var r in Renderers)
        {
            if (r == null) continue;
            r.enabled = true;
        }
    }

    [ContextMenu("DestroySelfRenderer")]
    public void DestroySelfRenderer()
    {
        MeshRenderer r = this.GetComponent<MeshRenderer>();
        if (r != null)
        {
            GameObject.DestroyImmediate(r);
        }
    }

    public int GetNodeCount()
    {
        int count = 0;
        foreach (var node in Nodes)
        {
            if (node != null)
            {
                count++;
            }
        }
        return count;
    }

    [ContextMenu("CombineMesh")]
    public void CombineMesh()
    {

        //if (renderersRoot)
        //{
        //    GameObject.DestroyImmediate(renderersRoot);
        //}

        DestroySelfRenderer();

        InitRenderers();

        var renderers=CombineInner();

        HideRenders(renderers);

        renderersRoot.transform.SetParent(this.transform);

        //RecoverParent();

        //MoveRenderers();
    }

    [ContextMenu("RecoverParent")]
    public bool RecoverParent(Transform parent = null)
    {

        // if(Renderers.Count!= RendererParents.Count)
        // {
        //     Debug.LogError($"AreaTreeNode.RecoverParent [{this.name}][Renderers.Count && i<RendererParents.Count][{Renderers.Count}][{RendererParents.Count}]");
        //     return;
        // }
        // for (int i = 0; i < Renderers.Count && i<RendererParents.Count; i++)
        // {
        //     MeshRenderer render = Renderers[i];
        //     if (render == null) continue;
        //     Transform parent = RendererParents[i];
        //     render.transform.SetParent(parent);
        // }
        bool flag = true;
        //int count = 0;
        
        for (int i = 0; i < Renderers.Count; i++)
        {
            MeshRenderer render = Renderers[i];
            if (render == null) continue;
            RendererId rId=render.GetComponent<RendererId>();
            if(rId!=null){
                GameObject pGo=rId.SetParent();
                if (pGo == null || pGo.name.Contains("_Renderers"))//
                {
                    //Debug.LogWarning($"AreaTreeNode.RecoverParent pGo.name.Contains(_Renderers) pGo:{pGo} tree:{tree.name}");
                    if(parent!=null)
                        render.transform.SetParent(parent);
                    flag = false;
                }
                else
                {

                }
            }
            else{
                //Transform parent = tree.Target.transform;
                if (parent != null)
                    render.transform.SetParent(parent);
                else 
                    render.transform.SetParent(tree.transform.parent);
            }
        }

        if (renderersRoot != null)
        {
            List<Transform> children = new List<Transform>();
            for(int i = 0; i < renderersRoot.transform.childCount; i++)
            {
                Transform child = renderersRoot.transform.GetChild(i);
                children.Add(child);
            }
            if (children.Count > 0)
            {
                if (parent == null)
                {
                    parent = tree.transform.parent;
                }
                foreach(var child in children)
                {
                    Debug.LogWarning($"AreaTreeNode.RecoverParent Error node:{this.name} tree:{tree.name} renderersChild:{child.name} children:{children.Count}");
                    LODGroup lodGroup = child.GetComponent<LODGroup>();
                    if(lodGroup!=null)
                    {
                        var lods= parent.gameObject.FindOrCreateChild("LODs");
                        lodGroup.transform.SetParent(lods.transform);
                    }
                    else
                    {
                        child.SetParent(parent);
                    }
                }
            }
        }
        return flag;
    }

    public void RecoverParentEx()
    {
        IdDictionary.InitInfos();

        RecoverParent();
    }

    private MeshRendererInfoList CombineInner()
    {
        if(AreaTreeManager.Instance.IsByLOD)
        {
            return CombineInnerLOD();
        }
        else
        {
            if (combindResult)
            {
                GameObject.DestroyImmediate(combindResult);
            }
            //MeshRendererInfoList list=MeshRendererInfo.FindByTypes(Renderers, new List<MeshRendererType>() { MeshRendererType.None, MeshRendererType.Structure, MeshRendererType.Detail });

            var rendererInfos = new MeshRendererInfoList(Renderers);
            //rendererInfos.FilterByVertexCount(AreaTreeManager.Instance.MaxMeshVertexCount);
            var list12 = rendererInfos.SplitListByVertexCount(AreaTreeManager.Instance.MaxMeshVertexCount,this.name);

            combindResult = CombineRenderers(list12[0].GetAllRenderers().ToArray(), this.transform, this.name + "_Combined");

            list12[1].CloneTo(combindResult);

            //MeshCombineHelper.CombineEx(new MeshCombineArg(this.renderersRoot, Renderers.ToArray()), 1);
            ////combindResult = MeshCombineHelper.CombineEx(new MeshCombineArg(this.renderersRoot, Renderers.ToArray()), 0);
            //combindResult.name = this.name + "_Combined";
            //combindResult.transform.SetParent(this.transform);

            CombinedRenderers = combindResult.GetComponentsInChildren<MeshRenderer>(true);
            //AddMeshCollider(combindResult);
            SetRenderersRootActive(false);
            return rendererInfos;
        }
    }

    private MeshRendererInfoList CombineInnerLOD()
    {
        try
        {
            if (combindResult)
            {
                GameObject.DestroyImmediate(combindResult);
            }

            var infoLodList = MeshRendererInfo.SplitByLOD(Renderers.ToArray());

            combindResult = new GameObject(this.name + "_Combined_LOD");
            combindResult.transform.position = this.transform.position;
            combindResult.transform.SetParent(this.transform);

            LODGroup group = combindResult.AddComponent<LODGroup>();
            LOD[] lods = LODManager.Instance.CreateLODS(infoLodList.Count);
            for (int i = 0; i < infoLodList.Count; i++)
            {
                var list = infoLodList[i];
                var renderers = list.GetRenderers();
                var combinedLOD = CombineRenderers(renderers.ToArray(), combindResult.transform, this.name + "_Combined_LOD" + i);

                //foreach(var renderer in renderers)
                //{
                //    renderer.transform.SetParent(combinedLOD.transform);
                //}
                var combinedRenderers = combinedLOD.GetComponentsInChildren<MeshRenderer>(true);

                lods[i].renderers = combinedRenderers;
            }
            group.SetLODs(lods);

            CombinedRenderers = combindResult.GetComponentsInChildren<MeshRenderer>(true);
            SetRenderersRootActive(false);
        }
        catch(Exception ex)
        {
            Debug.LogError($"CombineInnerLOD name:{this.name} Exception:{ex}");
        }
        return null;
    }

    private GameObject CombineRenderers(MeshRenderer[] renderers,Transform parent,string name)
    {
        GameObject cResult = MeshCombineHelper.CombineEx(new MeshCombineArg(this.renderersRoot, renderers), MeshCombineMode.MultiByMat);
        //combindResult = MeshCombineHelper.CombineEx(new MeshCombineArg(this.renderersRoot, Renderers.ToArray()), 0);
        cResult.name = name;
        cResult.transform.SetParent(parent);

        //CombinedRenderers = cResult.GetComponentsInChildren<MeshRenderer>(true);

        AddMeshCollider(cResult);
        return cResult;
    }

    private void AddMeshCollider(GameObject root)
    {
        var meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        foreach (var mf in meshFilters)
        {
            mf.gameObject.AddComponent<MeshCollider>();
        }
    }

    public void DestroyBoundBox()
    {
        MeshHelper.RemoveMeshComponents(this.gameObject);
    }

    [ContextMenu("UpdateCombined")]
    public void UpdateCombined()
    {
        DateTime start = DateTime.Now;
        renderersRoot.transform.SetParent(null);

        // CopyRenderers();

        // CombineInner();

        // HideRenders();

        // CreateDictionary();

        // renderersRoot.transform.SetParent(this.transform);

        CombineMesh();

        CreateDictionary();

        Debug.LogError($"UpdateCombined renderCount:{Renderers.Count},\t{(DateTime.Now - start).ToString()}");
    }

    private void SwitchModel(bool isCombined)
    {
        if (this.Nodes.Count > 0)
        {
            return;
        }
        if (renderersRoot == null)
        {
            return;
        }
        if (combindResult == null)
        {
            return;
        }
        //newRenderers=renderersRoot.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < Renderers.Count; i++)
        {
            try
            {
                MeshRenderer r = Renderers[i];
                if (r == null)
                {
                    Debug.LogError($"SwitchModel r == null node:{this.name}");
                    continue;
                }
                if (r.gameObject == null)
                {
                    Debug.LogError($"SwitchModel r.gameObject == null node:{this.name}");
                    continue;
                }
                r.gameObject.SetActive(!isCombined);
                r.enabled = !isCombined;
                // var collider = colliders[i];
                // if (collider)
                // {
                //     collider.enabled = !isCombined;
                // }

                //MeshCollider collider = r.GetComponent<MeshCollider>();
                //if (collider != null)
                //{
                //    collider.enabled = !isCombined;
                //}
                //else
                //{
                //    Debug.LogError("SwitchModel collider==null:" + r);
                //}
            }
            catch (Exception ex)
            {
                Debug.LogError($"SwitchModel Exception {ex}");
            }
        }
        if(combindResult)
            combindResult.SetActive(isCombined);
        SetRenderersRootActive(!isCombined);
    }

    private void SetRenderersRootActive(bool isActive)
    {
        //renderersRoot.SetActive(isActive);
    }

    public void SwitchToCombined()
    {
        SwitchModel(true);
    }

    public void SwitchToRenderers()
    {
        SwitchModel(false);
    }

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            children.Add(this.transform.GetChild(i));
        }
        foreach (var child in children)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        if(combindResult)
            GameObject.DestroyImmediate(combindResult);
        if(renderersRoot)
            GameObject.DestroyImmediate(renderersRoot);
    }


    //private ModelAreaTree tree;
    public List<AreaTreeNode> CreateSubNodes(int level, int index, ModelAreaTree tree,int prefabId)
    {

        this.tree = tree;

        var nodeSetting = tree.nodeSetting;
        int minLevel = nodeSetting.MinLevel;
        int maxLevel = nodeSetting.MaxLevel;
        //int maxRenderCount= nodeSetting.MaxRenderCount;

        //Bounds bounds=ColliderHelper.CaculateBounds(this.Renderers);
        Bounds bounds = this.Bounds;
        var bs = bounds.size;

        //Debug.Log($"CreateSubNodes tree:{tree.name} level:{level} index:{index} size:{bounds.size} count:{this.Renderers.Count}");

        Vector3 cellCount = new Vector3(1, 1, 2);

        // if(level%3==0){
        //     cellCount=new Vector3(1,1,2);
        // }
        // else if(level%3==1){
        //     cellCount=new Vector3(1,2,1);
        // }
        // else if(level%3==2){
        //     cellCount=new Vector3(2,1,1);
        // }

        if (bs.x >= bs.y && bs.x >= bs.z)
        {
            cellCount = new Vector3(2, 1, 1);
        }
        else if (bs.y >= bs.x && bs.y >= bs.z)
        {
            cellCount = new Vector3(1, 2, 1);
        }
        else if (bs.z >= bs.x && bs.z >= bs.y)
        {
            cellCount = new Vector3(1, 1, 2);
        }

        if (level > tree.nodeStatics.LevelDepth)
        {
            tree.nodeStatics.LevelDepth = level;
        }

        if (level > maxLevel)
        {
            IsLeaf = true;
            // Debug.Log($"LeafNode0 level:{level}");
            return null;
        }

        if (this.Renderers.Count <= nodeSetting.MinRenderCount)
        {
            IsLeaf = true;
            // Debug.Log($"LeafNode1 level:{level}");
            return null;
        }

        if (level >= minLevel)
        {
            // Debug.Log($"[{this.name}]nodeSetting.MaxRenderCount:{nodeSetting.MaxRenderCount}");
            if (nodeSetting.MaxRenderCount > 0)
            {
                if (this.Renderers.Count < nodeSetting.MaxRenderCount)
                {
                    //IsLeaf = true;
                    //Debug.Log($"LeafNode2 level:{level}");
                    //return null;

                    if (this.VertexCount < nodeSetting.MaxVertexCount)
                    {
                        IsLeaf = true;
                        // Debug.Log($"LeafNode3 level:{level}");
                        return null;
                    }
                    else
                    {
                        //继续创建子物体
                    }
                }
                else
                {

                    //if (this.VertexCount < nodeSetting.MaxVertexCount)
                    //{
                    //    IsLeaf = true;
                    //    Debug.Log($"LeafNode3 level:{level}");
                    //    return null;
                    //}
                    //else
                    //{
                    //    //继续创建子物体
                    //}
                }
            }
            else
            {
                IsLeaf = true;
                // Debug.Log($"LeafNode4 level:{level}");
                return null;
            }
        }
        var allCount = cellCount.x * cellCount.y * cellCount.z;
        //DateTime start=DateTime.Now;
        this.ClearChildren();



        // Debug.LogError("bounds:"+bounds);
        // Debug.LogError("cellCount:"+cellCount);
        var min = bounds.min;
        var xSize = bounds.size.x / cellCount.x;
        var ySize = bounds.size.y / cellCount.y;
        var zSize = bounds.size.z / cellCount.z;
        Vector3 size = new Vector3(xSize, ySize, zSize);
        // Debug.LogError("size:"+size);
        List<Bounds> cellBoundsList = new List<Bounds>();
        List<AreaTreeNode> nodes = new List<AreaTreeNode>();
        int id = 0;
        for (int i = 0; i < cellCount.x; i++)
        {
            for (int j = 0; j < cellCount.y; j++)
            {
                for (int k = 0; k < cellCount.z; k++)
                {
                    id++;
                    var offset = new Vector3(i * xSize, j * ySize, k * zSize);
                    var center = min + offset + size / 2;
                    Bounds cellBounds = new Bounds();
                    cellBounds.center = center;
                    cellBounds.size = size;
                    cellBoundsList.Add(cellBounds);

                    GameObject cube = AreaTreeHelper.CreateBoundsCube(cellBounds, $"Node_{level}_{tree.TreeNodes.Count}", transform, prefabId);
                    AreaTreeNode node = cube.AddComponent<AreaTreeNode>();
                    nodes.Add(node);
                    node.Bounds = cellBounds;

                    tree.TreeNodes.Add(node);

                    Nodes.Add(node);
                }
            }
        }

        var renderers = this.Renderers;
        foreach (var render in renderers)
        {
            //var pos=render.transform.position;
            MeshRendererInfo renderInfo = render.GetComponent<MeshRendererInfo>();
            if (renderInfo == null)
            {
                renderInfo = render.gameObject.AddComponent<MeshRendererInfo>();
                renderInfo.Init();
            }
            //var pos = renderInfo.center;  //position not center

            GetNodesByCenterPos(nodes, renderInfo);

            //foreach (AreaTreeNode node in nodes)
            //{
            //    if (node.Bounds.Contains(pos))
            //    {
            //        node.AddRenderer(render);
            //    }
            //}
        }

        //RendererCount = renderers.Count;
        //renderers.Clear();

        for (int i = 0; i < nodes.Count; i++)
        {
            AreaTreeNode node = nodes[i];
            // if(node.Renderers.Count<maxRenderCount){
            //     continue;
            // }

            node.name += $"_{node.RendererCount}_{node.GetVertexCount():F0}w";
            //node.TreeName=tree.name;
            node.CreateSubNodes(level + 1, i, tree, prefabId);

            //if (node.VertexCount > tree.nodeStatics.MaxNodeVertexCount || tree.nodeStatics.MaxNodeVertexCount == 0)
            //{
            //    tree.nodeStatics.MaxNodeVertexCount = node.VertexCount;
            //}

            //if (node.VertexCount < tree.nodeStatics.MinNodeVertexCount || tree.nodeStatics.MinNodeVertexCount==0)
            //{
            //    tree.nodeStatics.MinNodeVertexCount = node.VertexCount;
            //}
        }

        // int visibleCellCount=0;
        // foreach(AreaTreeNode node in nodes)
        // {
        //     if(node.Renderers.Count==0){
        //         GameObject.DestroyImmediate(node.gameObject);
        //     }
        //     else{
        //         visibleCellCount++;
        //     }
        // }

        //Debug.LogError($"CreateCells cellCount:{visibleCellCount}/{allCount},\tavg:{renderers.Count/visibleCellCount},\t{(DateTime.Now-start).ToString()}");
        //bound.Contains()

        return nodes;
    }

    public static List<AreaTreeNode> GetNodesByCenterPos(List<AreaTreeNode> nodes, MeshRendererInfo renderer)
    {
        return GetNodesByCenterPos(nodes, renderer, Vector3.zero, 0);
    }

    public static List<AreaTreeNode> GetNodesByCenterPos(List<AreaTreeNode> nodes, MeshRendererInfo renderer,Vector3 offset,int count)
    {
        List<AreaTreeNode> newNodes = new List<AreaTreeNode>();
        foreach (AreaTreeNode node in nodes)
        {
            if (node.Bounds.Contains(renderer.center+ offset))
            {
                newNodes.Add(node);
            }
        }
        if (newNodes.Count == 1)
        {
            //Debug.LogError($"GetNodesByCenterPos newNodes.Count == 1:" + renderer);
            newNodes[0].AddRenderer(renderer.meshRenderer);
        }
        else if (newNodes.Count == 0)
        {
            Debug.LogWarning($"GetNodesByCenterPos1[{count}][{offset}] newNodes.Count == 0 renderer:{renderer} center:{renderer.center}");
            if (count > 2)
            {
                return newNodes;
            }
            GetNodesByCenterPos(nodes, renderer, new Vector3(0.01f, 0.01f, 0.01f), count + 1);
        }
        else //if(newNodes.Count > 1)
        {
            Debug.LogWarning($"GetNodesByCenterPos2[{count}][{offset}] newNodes.Count > 1 renderer:{renderer} center:{renderer.center} count:{newNodes.Count} node1:{newNodes[0]}({newNodes[0].Bounds}) node2:{newNodes[1]}({newNodes[1].Bounds})");
            if (count > 2)
            {
                newNodes[0].AddRenderer(renderer.meshRenderer);
            }
            else
            {
                GetNodesByCenterPos(nodes, renderer, new Vector3(0.01f, 0.01f, 0.01f), count + 1);
            }
        }

        return newNodes;
    }

    //[ContextMenu("ClearDictionary")]
    //public void ClearDictionary()
    //{
    //    Debug.Log("ClearDictionary Start:" + AreaTreeHelper.render2NodeDict.Count);
    //    AreaTreeHelper.render2NodeDict.Clear();
    //    Debug.Log("ClearDictionary End:" + AreaTreeHelper.render2NodeDict.Count);
    //}

    [ContextMenu("CreateDictionary")]
    public void CreateDictionary()
    {
        if (RenderersId.Count==0)
        {
            SaveRenderersId();
        }
        // Debug.Log($"CreateDictionary StartCount:{AreaTreeHelper.render2NodeDict.Count},Renderers:{Renderers.Count} Count:{GetRendererCount()}");
        if (this.Nodes.Count == 0)
        {
            //if (GetRendererCount() == 0)
            //{
            //    Renderers = renderersRoot.GetComponentsInChildren<MeshRenderer>(true).ToList();
            //    // Debug.Log($"CreateDictionary FindRenderers Renderers:{Renderers.Count} Count:{GetRendererCount()}");
            //}

            for (int i = 0; i < Renderers.Count; i++)
            {
                var render = this.Renderers[i];
                if (render == null)
                {
                    //Debug.LogError($"AreaTreeNode.CreateDictionary1 render == null id:{i}");
                    continue;
                }
                AreaTreeHelper.RegisterRenderer(render, this);
            }

            for (int i = 0; i < RenderersId.Count; i++)
            {
                var renderId = this.RenderersId[i];
                if (renderId == null)
                {
                    Debug.LogError($"AreaTreeNode.CreateDictionary2 renderId == null id:{i}");
                    continue;
                }

                bool r=AreaTreeHelper.RegisterRendererId(renderId, i, this);
                //if (r == false)
                //{
                //    if(GetRendererCount()== RenderersId.Count)
                //    {

                //    }
                //    else
                //    {
                //        Debug.LogError($"AreaTreeNode.CreateDictionary3 GetRendererCount()!= RenderersId.Count tree:{tree.name} node:{this.name} id:{i} count1:{GetRendererCount()} count2:{RenderersId.Count}");
                //    }
                    
                //}
            }
        }
        //Debug.Log("CreateDictionary 1:"+AreaTreeHelper.render2NodeDict.Count);

        ////CopyRenders
        //if (newRenderers != null)
        //{
        //    AreaTreeHelper.AddNodeDictItem_Renderers(newRenderers, this);
        //}

        //CombinedRenders
        //Debug.Log("CreateDictionary 2:"+AreaTreeHelper.render2NodeDict.Count);
        if (combindResult != null)
        {
            var renderers = combindResult.GetComponentsInChildren<MeshRenderer>(true);
            AreaTreeHelper.AddNodeDictItem_Renderers(renderers, this);
            AreaTreeHelper.AddNodeDictItem_Combined(renderers, this);
        }
        //Debug.Log("CreateDictionary 3:"+AreaTreeHelper.render2NodeDict.Count);

        //Debug.Log("CreateDictionary End:"+AreaTreeHelper.render2NodeDict.Count);
    }

    public MeshRenderer[] GetCombinedRenderers()
    {
        return combindResult.GetComponentsInChildren<MeshRenderer>();
    }

    private SubScene_Base RenderersScene;

    public SubScene_Base GetRenderersScene()
    {
        if (RenderersScene == null)
        {
            RenderersScene = this.renderersRoot.GetComponent<SubScene_Base>();
        }
        return RenderersScene;
    }

    public bool IsLoadingScene = false;

    public SubScene_Base StartLoadingScene()
    {
        if (RenderersScene == null)
        {
            RenderersScene = this.renderersRoot.GetComponent<SubScene_Base>();
        }
        IsLoadingScene = true;
        return RenderersScene;
    }

    public void ShowLogInfo()
    {
        GetRenderersScene();
        Debug.Log($"Node {this.name} Renderers:{Renderers.Count} RenderersId:{RenderersId.Count} IsLoaded:{RenderersScene.IsLoaded} Path:{this.transform.GetPath()}");
    }

    [ContextMenu("ShowNodes")]
    public void ShowNodes()
    {
        try
        {
            if (IsNodeVisible == true) return;

            GetRenderersScene();

#if UNITY_EDITOR
            SubScene_Base[] scenes = SubScene_List.GetBaseScenes(this.gameObject);
            if (RenderersScene != null)
            {
                Debug.Log($"ShowNodes {this.name} Renderers:{Renderers.Count} RenderersId:{RenderersId.Count} Scenes:{scenes.Length} IsLoaded:{RenderersScene.IsLoaded} Path:{this.transform.GetPath()}");
            }
            else
            {
                Debug.Log($"ShowNodes {this.name} RenderersScene==null Renderers:{Renderers.Count} RenderersId:{RenderersId.Count} Scenes:{scenes.Length} Path:{this.transform.GetPath()}");
            }
            
#endif
            IsNodeVisible = true;
            if(this.gameObject==null)
            {
                Debug.LogError("AreaTreeNode.ShowNodes this.gameObject==null ：" + this.name + "|" + tree);
                return;
            }
            this.gameObject.SetActive(true);
            foreach (AreaTreeNode node in Nodes)
            {
                if (node == null) continue;
                node.ShowNodes();
            }

            ShowRenderersRoot();

            if (CombinedRenderers == null)
            {
                Debug.LogError("AreaTreeNode.ShowNodes CombinedRenderers==null ：" + this.name + "|" + tree);
            }
            else
            {
                if (combindResult != null)
                {
                    if (scene_combined == null)
                    {
                        scene_combined = combindResult.GetComponent<SubScene_Base>();
                    }
                    if (scene_combined != null && scene_combined.IsLoaded)
                    {
                        foreach (var render in CombinedRenderers)
                        {
                            if (render == null)
                            {
                                Debug.LogError("AreaTreeNode.ShowNodes CombinedRenderers render==null ：" + this.name + "|" + tree);
                                continue;
                            }
                            if (render.gameObject == null)
                            {
                                Debug.LogError("AreaTreeNode.ShowNodes render.gameObject == null ：" + this.name + "|" + tree);
                                continue;
                            }
                            render.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {

            Debug.LogError($"AreaTreeNode.ShowNodes gameObject:{this.name} Exception:{ex}");
        }
        
    }

    private void ShowRenderersRoot()
    {
        if (renderersRoot)
        {
            renderersRoot.SetActive(true);

            //DynamicCullingManage.Instance.AddObjectsForCulling(Renderers.ToArray());
        }

        Debug.LogError($"AreaTreeNode.ShowRenderersRoot :{this.name} path:{transform.GetPath()}");
    }

    private void HideRenderersRoot()
    {
        if (renderersRoot)
        {
            renderersRoot.SetActive(false);

            //DynamicCullingManage.Instance.AddObjectsForCulling(Renderers.ToArray());
        }

        Debug.LogError($"AreaTreeNode.HideRenderersRoot :{this.name} path:{transform.GetPath()}");
    }

    public bool IsNodeVisible = true;

    SubScene_Base scene_combined;

    public SubScene_Base GetCombinedScene()
    {
        if (combindResult == null)
        {
            return scene_combined;
        }
        if(scene_combined==null){
            scene_combined=combindResult.GetComponent<SubScene_Base>();
        }
        return scene_combined;
    }

    public SubScene_Base GetCombinedLinkedScene()
    {
        if (combindResult == null)
        {
            return scene_combined.LinkedScene;
        }
        if (scene_combined==null){
            scene_combined=combindResult.GetComponent<SubScene_Base>();
        }
        return scene_combined.LinkedScene;
    }

    /// <summary>
    /// 是否处于楼层模式
    /// </summary>
    /// <returns></returns>
    private bool IsFloorMode()
    {
        if (FactoryDepManager.currentDep == null) return false;
        if (FactoryDepManager.currentDep is FloorController || FactoryDepManager.currentDep is RoomController) return true;
        return false;
    }

    [ContextMenu("HideNodes")]
    public void HideNodes()
    {
        try
        {
            //处于楼层模式时，只加载细节，不卸载细节 add by wk 220314
            if (IsNodeVisible == false|| IsFloorMode()) return;
            
            //Debug.Log("HideNodes:" + this.name);
            IsNodeVisible = false;

            // this.gameObject.SetActive(false);

            for (int i = 0; i < Nodes.Count; i++)
            {
                AreaTreeNode node = Nodes[i];
                //if (node == null) continue;
                if (node == null)
                {
                    Debug.LogError($"HideNodes[{i}] node == null:" + this.name + "|" + tree);
                    continue;
                }
                node.HideNodes();
            }
            var scene1 = GetCombinedScene();
            if (scene1 != null && scene1.IsLoaded)
            {
                for (int i = 0; i < CombinedRenderers.Length; i++)
                {
                    MeshRenderer render = CombinedRenderers[i];
                    if (render == null)
                    {
                        Debug.LogError($"HideNodes[{i}] render == null:" + this.name + "|" + tree);
                        continue;
                    }
                    render.gameObject.SetActive(false);
                }
            }
            //if (renderersRoot)
            //{
            //    renderersRoot.SetActive(false);

            //    //DynamicCullingManage.Instance.RemoveObjects(Renderers.ToArray());
            //}

            HideRenderersRoot();

            //MeshRenderer mr = this.GetComponent<MeshRenderer>();
            //if(mr!=null)
            //    mr.enabled = false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"AreaTreeNode.HideNode Exception gameObject:{this.name} ex:{ex}");
        }
        
    }

    Mesh meshAsset = null;

    //[ContextMenu("SaveMeshes")]
    public void SaveMeshes(string dir)
    {
#if UNITY_EDITOR
        if (meshAsset != null) return;

        MeshFilter[] meshFilters = combindResult.GetComponentsInChildren<MeshFilter>();
        //for (int i = 0; i < meshFilters.Length; i++)
        //{
        //    MeshFilter meshFilter = meshFilters[i];
        //    if (UnityEditor.AssetDatabase.Contains(meshFilter.sharedMesh))
        //    {
        //        return;
        //    }
        //}

        //meshAsset = new Mesh();
        ////string assetName = tree.Target.name+"_"+gameObject.name + gameObject.GetInstanceID();
        //string assetName = gameObject.name + gameObject.GetInstanceID();

        //string meshPath = dir + "/" + assetName + ".asset";
        //EditorHelper.SaveAsset(meshAsset, meshPath, false);

        //for (int i = 0; i < meshFilters.Length; i++)
        //{
        //    MeshFilter meshFilter = meshFilters[i];
        //    Debug.LogError("meshFilter.sharedMesh :" + meshFilter.sharedMesh);
        //    if (meshFilter.sharedMesh == null)
        //    {
        //        Debug.LogError("meshFilter.sharedMesh == null");
        //        continue;
        //    }
        //    Mesh mesh = meshFilter.sharedMesh;
        //    EditorHelper.SaveAsset(mesh, meshPath, true);
        //}

        EditorHelper.SaveMeshAsset(dir, gameObject, meshFilters);
#endif
    }

    public override void SaveTreeRendersId()
    {
        this.SaveRenderersId();
    }

#if UNITY_EDITOR
    public void EditorCreateNodeScenes()
    {
        EditorCreateNodeScenes(false,null);
    }

    public void EditorCreateNodeScenes(bool isSingle,Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;

        SaveTreeRendersId();

        DestroyScenes();

        //InitInOut(false);
        //var scenes = CreatePartScene(contentType);
        //EditorCreateScenes(scenes, null);

        var scenes = new SubSceneBag();

        if(tree==null){
            
            var trees=this.GetComponentsInParent<ModelAreaTree>(true);
            if(trees.Length>0){
                tree=trees[0];
                Debug.LogWarning("AreaTreeNode.EditorCreateNodeScenes tree==null 1 :"+this.name+" tree:"+tree.name);
            }
            
        }

        if(tree==null){
            Debug.LogError("AreaTreeNode.EditorCreateNodeScenes tree==null 2 !!!!!!!!!!!!!!!:"+this.name);
            return ;
        }

        SubScene_Out0 scene1 = null;
        if (combindResult)
        {
            scene1 = SubSceneHelper.EditorCreateScene<SubScene_Out0>(combindResult, SceneContentType.TreeNode, false, tree.gameObject);
            scene1.cubePrefabId = tree.GetCubePrefabId();
            //scene1.contentType = SceneContentType.TreeNode;
            //scene1.gos = SubSceneHelper.GetChildrenGos(combindResult.transform);
            scene1.SetObjects(SubSceneHelper.GetChildrenGos(combindResult.transform));
            scene1.Init();
            scenes.Add(scene1);
        }
        else
        {
            //Debug.LogError("AreaTreeNode.EditorCreateScenes combindResult==null:" + this.name);
        }

        SubScene_Base scene2 = null;
        if (renderersRoot)
        {
            if(isSingle)
                MoveRenderers();

            if (tree.IsInTree())
            {
                scene2 = SubSceneHelper.EditorCreateScene<SubScene_In>(renderersRoot, SceneContentType.TreeNode, false, tree.gameObject);
            }
            else if (tree.IsOutTree0())
            {
                scene2 = SubSceneHelper.EditorCreateScene<SubScene_Out0>(renderersRoot, SceneContentType.TreeNode, false, tree.gameObject);
            }
            else if (tree.IsOutTree1())
            {
                scene2 = SubSceneHelper.EditorCreateScene<SubScene_Out1>(renderersRoot, SceneContentType.TreeNode, false, tree.gameObject);
            }
            else if (tree.IsGPUITree())
            {
                scene2 = SubSceneHelper.EditorCreateScene<SubScene_GPUI>(renderersRoot, SceneContentType.TreeNode, false, tree.gameObject);
            }
            else
            {
                scene2 = SubSceneHelper.EditorCreateScene<SubScene_In>(renderersRoot, SceneContentType.TreeNode, false, tree.gameObject);
            }

            //scene2.gos = SubSceneHelper.GetChildrenGos(renderersRoot.transform);
            scene2.cubePrefabId = tree.GetCubePrefabId();
            //scene2.contentType = SceneContentType.TreeNode;
            scene2.SetObjects(SubSceneHelper.GetChildrenGos(renderersRoot.transform));
            scene2.Init();
            scenes.Add(scene2);
        }
        else
        {
            //Debug.LogError("AreaTreeNode.EditorCreateScenes renderersRoot==null:" + this.name);
        }

        if (scene2 != null)
            scene2.LinkedScene = scene1;

        if (scene1 != null)
            scene1.LinkedScene = scene2;

        EditorCreateScenes(scenes, progressChanged);

        //SubSceneManager.Instance.ClearOtherScenes();
        //EditorMoveScenes();

        if (progressChanged == null) Debug.LogError($"AreaTreeNode.EditorCreateScenes time:{(DateTime.Now - start)}");
    }

    [ContextMenu("* EditorLoadScenesEx")]
    public void EditorLoadScenesEx()
    {
        IdDictionary.InitInfos();
        EditorLoadScenes(null);
    }

    public override void EditorLoadScenes(Action<ProgressArg> progressChanged)
    {
        DateTime start = DateTime.Now;
        // var scenes = GetSubScenesOfTypes(new List<SceneContentType>() { SceneContentType.TreeNode });
        // base.EditorLoadScenes(scenes.ToArray(), progressChanged);

        base.EditorLoadScenes(progressChanged);

        //this.InitInOut(false);
        //LoadTreeRenderers();

        int count = this.LoadRenderers();
        var time=DateTime.Now - start;
        double ms = time.TotalMilliseconds;
        MoveRenderers();
        Debug.LogWarning($"AreaTreeNode.EditorLoadScenes tree:{tree.name} node:{this.name} time:{ms:F1}ms vertex:{GetVertexCount()} ");
    }

    [ContextMenu("* UnLoadRenderers")]
    public void UnLoadRenderers()
    {
        SubScene_Base rendererScene = renderersRoot.GetComponent<SubScene_Base>();
        if (rendererScene)
        {
            rendererScene.UnLoadGosM();
        }
        else
        {
            Debug.LogError("EditorUnLoadRenderers rendererScene==null :" + this.name);
        }
    }

    [ContextMenu("* EditorLoadRenderers")]
    private void EditorLoadRenderers()
    {
        IdDictionary.InitInfos();
        SubScene_Base rendererScene = renderersRoot.GetComponent<SubScene_Base>();
        if (rendererScene)
        {
            rendererScene.EditorLoadSceneEx();
            LoadRenderers_Renderers();
            //SwitchToCombined();
        }
        else
        {
            Debug.LogError("EditorUnLoadRenderers rendererScene==null :" + this.name);
        }
    }

    [ContextMenu("* EditorLoadRenderersEx")]
    private void EditorLoadRenderersEx()
    {
        IdDictionary.InitInfos();
        SubScene_Base rendererScene = renderersRoot.GetComponent<SubScene_Base>();
        if (rendererScene)
        {
            rendererScene.EditorLoadSceneEx();
            LoadRenderers_Renderers();
            SwitchToCombined();
        }
        else
        {
            Debug.LogError("EditorUnLoadRenderers rendererScene==null :" + this.name);
        }
    }

#endif

    public SubScene_Base GetScene()
    {
        var scene1 = this.GetComponentInChildren<SubScene_LODs>(true);
        if (scene1 != null)
        {
            return scene1;
        }
        var scene2 = this.GetComponentInChildren<SubScene_Base>(true);
        return scene2;
    }


    public void LoadAndSwitchToRenderers(Action<bool> callback)
    {
        this.gameObject.SetActive(true);
        SubScene_Base scene = GetScene();

        //Debug.LogError($"LoadAndSwitchToRenderers 1 node:{this} tree:{tree} scene:{scene}");
        
        if (scene != null && scene.IsLoaded==false)
        {
            //Debug.LogError($"LoadAndSwitchToRenderers 21 node:{this} tree:{tree} scene:{scene}");
            scene.LoadSceneAsync((b, s) =>
            {
                //Debug.LogError($"LoadAndSwitchToRenderers 3 node:{this} tree:{tree} scene:{scene}");
                LoadAndSwitchToRenderers_Inner(callback);
            });
        }
        else
        {
            //Debug.LogError($"LoadAndSwitchToRenderers 22 node:{this} tree:{tree} scene:{scene}");
            LoadAndSwitchToRenderers_Inner(callback);
        }
    }

    public void SwitchToRenderersEx()
    {
        this.LoadRenderers();
        SwitchToRenderers();
    }

    private void LoadAndSwitchToRenderers_Inner(Action<bool> callback)
    {
        if (renderersRoot != null)
        {
            var sceneIn = renderersRoot.GetComponentInChildren<SubScene_In>(true);
            if (sceneIn == null)
            {
                Debug.Log($"LoadAndSwitchToRenderers[{this.name}] scene == null!!!");
                SwitchToRenderersEx();

                if (callback != null)
                {
                    callback(false);
                }
            }
            else
            {
                Debug.Log($"LoadAndSwitchToRenderers[{this.name}] scene:{sceneIn}");
                sceneIn.LoadSceneAsync((b, s) =>
                {
                    SwitchToRenderersEx();

                    if (callback != null)
                    {
                        callback(b);
                    }
                });
            }
        }
        else
        {
            if (callback != null)
            {
                callback(false);
            }
        }
        
    }

    private void OnDisable()
    {
        //Debug.LogError($"AreaTreeNode.OnDisable {this.name}");
    }

#if UNITY_EDITOR
    //private void OnDestroy()
    //{
    //    Debug.LogError($"AreaTreeNode.OnDestroy {this.name} path:{TransformHelper.GetPath(this.transform)}");
    //}
#endif

    private float lastP = 0;

    [ContextMenu("CreateLOD")]
    public void CreateLOD()
    {
        CreateLOD((p,f)=>
        {
            if (f)
            {
                ProgressBarHelper.ClearProgressBar();
            }
            else
            {
                if (p < lastP)
                {
                    Debug.LogError($"p < lastP {p}->{lastP}");
                }
                lastP = p;
                ProgressBarHelper.DisplayProgressBar("CreateLOD", $"P1 {p:P1}%", p);
                System.Threading.Thread.Sleep(100);
            }
        });
    }

    public void CreateLOD(Action<float,bool> progressChanged)
    {
        var renderers = CombinedRenderers;
        for (int i = 0; i < renderers.Length; i++)
        {
            float progress = (float)i / renderers.Length;
            if (progressChanged != null)
            {
                progressChanged(progress,false);
            }
            MeshRenderer renderer = renderers[i];
            LODManager.Instance.CreateAutoLOD(renderer.gameObject,p=> {
                float progress2 = (float)(i+p) / renderers.Length;
                if (progressChanged != null)
                {
                    progressChanged(progress2,false);
                }
            });
        }
        if (progressChanged != null)
        {
            progressChanged(1,true);
        }
    }
}
