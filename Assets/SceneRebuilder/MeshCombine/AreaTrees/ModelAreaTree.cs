using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelAreaTree : SubSceneCreater
{
    public bool IsInTree()
    {
        return this.name.Contains("_InTree");
    }

    public bool IsOutTree0()
    {
        return this.name.Contains("_Out0_");
    }

    public bool IsOutTree0Big()
    {
        return this.name.Contains("_Out0_BigTree");
    }

    public bool IsOutTree0Small()
    {
        return this.name.Contains("_Out0_SmallTree");
    }

    public bool IsOutTree1()
    {
        return this.name.Contains("_OutTree1");
    }

    public bool IsGPUITree()
    {
        return this.name.Contains("_GPUI");
    }

    private void Awake()
    {
        DestroyBoundBox();
    }
    public override string ToString()
    {
        return this.name;
    }
    //public GameObject Target=null;

    // public string TargetId;

    // public GameObject TargetCopy = null;

    // public bool IsFirst = true;
    // public GameObject GetTarget()
    // {
    //     if (IsCopy && IsFirst)
    //     {
    //         IsFirst = false;
    //         Debug.Log("GetTarget Target1:"+Target);
    //         Target.SetActive(false);
    //         TargetCopy = MeshHelper.CopyGO(Target);
    //         TargetCopy.SetActive(true);
    //         Debug.Log("GetTarget Target2:"+TargetCopy);
    //     }

    //     if (!IsCopy)
    //     {
    //         TargetCopy = Target;
    //     }
    //     return TargetCopy;
    // }

    public AreaTreeNode RootNode;

    public List<AreaTreeNode> TreeNodes=new List<AreaTreeNode>();
    public List<AreaTreeNode> TreeLeafs=new List<AreaTreeNode>();

    // [ContextMenu("GetAllMeshRenders")]
    // public void GetAllMeshRenders()
    // {
    //     Targets.Clear();

    //     var renderers=GameObject.FindObjectsOfType<MeshRenderer>();
    //     foreach(var r in renderers){
    //         Targets.Add(r.gameObject);
    //     }
    // }


    [ContextMenu("CreateBoundes")]
    public void CreateBoundes()
    {
        //var target = GetTarget();
        DateTime start = DateTime.Now;
        Renderer[] renders = GetTreeRendererers();
        Bounds bounds=ColliderHelper.CaculateBounds(renders);
        AreaTreeHelper.CreateBoundsCube(bounds, this.name+"_TargetBound",transform, GetCubePrefabId());

        Debug.LogWarning($"target:{this.name},renders:{renders.Length},bounds:{bounds}");
        Debug.LogWarning($"CreateBoundes \t{(DateTime.Now - start).ToString()}");
    }

    //[ContextMenu("CreateSubBoundes")]
    //public void CreateSubBoundes()
    //{
    //    //ClearChildren();

    //    DateTime start = DateTime.Now;
    //    var target = GetTarget();
    //    List<Transform> children = new List<Transform>();
    //    for (int i = 0; i < target.transform.childCount; i++)
    //    {
    //        children.Add(target.transform.GetChild(i));
    //    }
    //    Bounds boundsAll = new Bounds();
    //    for (int i = 0; i < children.Count; i++)
    //    {
    //        Transform child = children[i];
    //        //var child = this.transform.GetChild(i);
    //        Renderer[] renders = child.GetComponentsInChildren<Renderer>();
    //        Bounds bounds = ColliderHelper.CaculateBounds(renders);
    //        AreaTreeHelper.CreateBoundsCube(bounds, child.name+"_TargetBound", transform, GetCubePrefabId());

            
    //        boundsAll.Encapsulate(bounds);

    //        Debug.LogWarning($"[{i}] target:{child.name},renders:{renders.Length},bounds:{bounds},boundsAll:{boundsAll}");
    //    }
    //    Debug.LogWarning($"CreateSubBoundes \t{(DateTime.Now - start).ToString()}");

    //    AreaTreeHelper.CreateBoundsCube(boundsAll, target.name + "_TargetBoundAll", transform, GetCubePrefabId());
    //}

    public int GetCubePrefabId()
    {
        if (GetIsHidden())
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    [ContextMenu("2.1. ClearChildren")]
    public void ClearChildren()
    {
        List<Transform> children=new List<Transform>();
        for(int i=0;i<this.transform.childCount;i++)
        {
            children.Add(this.transform.GetChild(i));
        }
        foreach(var child in children){
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
    

    [ContextMenu("CheckRenderers")]
    public void CheckRenderers()
    {   
        DateTime start=DateTime.Now;
        int renderCount=0;
        foreach(var node in TreeNodes)
        {
            if(node==null)continue;
            if(node.Nodes.Count>0)continue;
            //node.CombineMesh();
            renderCount += node.RendererCount;
        }

        Debug.LogWarning($"CheckRenderers renderCount:{renderCount},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("CheckLeafNodeName")]
    public int CheckLeafNodeName()
    {   
        int errorCount=0;
        DateTime start=DateTime.Now;
        int renderCount=0;
        List<string> names=new List<string>();
        foreach(var node in TreeLeafs)
        {
            if(names.Contains(node.name)){
                errorCount++;
                Debug.LogError($"RepeatNodeName! [{this.name}][{node.transform.parent.name}][{node.name}]");
            }
            else{
                names.Add(node.name);
            }
        }
        //Debug.LogWarning($"CheckRenderers renderCount:{renderCount},\t{(DateTime.Now-start).ToString()}");
        return errorCount;
    }

    public bool IsHidden=false;

    public bool GetIsHidden()
    {
        if (this.name.Contains("_InTree"))
        {
            return true;
        }
        if (this.name.Contains("_Out0_SmallTree"))
        {
            return true;
        }
        return IsHidden;
    }

    //[ContextMenu("CombineMesh")]
    //public void CombineMesh()
    //{
    //    CombineMesh(null);
    //}

    private void CombineMesh(Action<ProgressArg> progressChanged)
    {
        //var nodes = TreeNodes;
        var nodes = TreeLeafs;
        if (nodes.Count == 0)
        {
            Debug.LogError("Tree.CombineMesh nodes.Count == 0 :" + this.name);
            return;
        }
        MeshCombinerSetting.Instance.SetSetting();
        //IsCombined=true;
        DateTime start=DateTime.Now;
        int renderCount=0;
        for (int i = 0; i < nodes.Count; i++)
        {
            AreaTreeNode node = nodes[i];
            if (node == null) continue;
            //float progress = (float)i / TreeNodes.Count;

            var p = new ProgressArg("CombineMesh", i, nodes.Count, node);
            if (progressChanged != null)
            {
                progressChanged(p);
            }
            if (node.Nodes.Count>0)continue;
            node.CombineMesh();
            renderCount += node.RendererCount;
        }

        int newRenderCount=0;
        //if(this.RootNode==null)
        //{
        //    Debug.LogError("Tree.CombineMesh this.RootNode==null :" + this.name);
        //    return;
        //}
        //var renderersNew=this.RootNode.GetComponentsInChildren<MeshRenderer>(true);
        var renderersNew = GetTreeRendererers();
        foreach (var render in renderersNew){
            if(render.enabled==true)
            {
                newRenderCount++;
            }
        }

        CombinedCount = newRenderCount;

        Debug.LogWarning($"Tree.CombineMesh TreeRenderers:{TreeRenderers.Count} renderCount:{renderCount}->{newRenderCount},\t{(DateTime.Now-start).ToString()}");
    }

    public int CombinedCount = 0;

    [ContextMenu("SwitchToCombined")]
    public void SwitchToCombined()
    {
        DateTime start=DateTime.Now;

        int c = 0;
        foreach(var node in TreeNodes)
        {
            if(node==null)continue;
            node.SwitchToCombined();
            c++;
        }

        Debug.LogWarning($"Tree.SwitchToCombined count:{c} \t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("SwitchToRenderers")]
    public void SwitchToRenderers()
    {
        DateTime start=DateTime.Now;
        int c = 0;
        foreach (var node in TreeNodes)
        {
            if(node==null)continue;
            node.SwitchToRenderers();
            c++;
        }

        Debug.LogWarning($"Tree.SwitchToRenderers tree:{this.name} count:{c} \t{(DateTime.Now-start).ToString()}");
    }

    [SerializeField]
    private MeshRendererInfoList TreeRenderers;

    //public void SetRenderers(List<MeshRenderer> renderers)
    //{
    //    TreeRenderers = new MeshRendererInfoList(renderers);
    //    //TreeRenderers.RemoveTypes(AreaTreeManager.Instance.FilterTypes, this.name);
    //    AreaTreeManager.Instance.FilterTreeNodeRenders(TreeRenderers, this.name);
    //    Debug.Log($"SetRenderers TreeRenderers:{TreeRenderers.Count} this:{this.name}");
    //}

    public void SetRenderers(MeshRenderer[] renderers)
    {
        TreeRenderers = new MeshRendererInfoList(renderers);
        //TreeRenderers.RemoveTypes(AreaTreeManager.Instance.FilterTypes, this.name);
        AreaTreeManager.Instance.FilterTreeNodeRenders(TreeRenderers, this.name);
        //Debug.Log($"SetRenderers TreeRenderers:{TreeRenderers.Count} this:{this.name}");
    }

    public void SetRenderers(GameObject renderersRoot)
    {
        var renderers = renderersRoot.GetComponentsInChildren<MeshRenderer>(true);
        SetRenderers(renderers);
        //Debug.Log($"SetRenderers root:{renderersRoot} renderers:{renderers.Length}");

        //TreeRenderers = new MeshRendererInfoList(renderersRoot);
        //TreeRenderers.RemoveTypes(AreaTreeManager.Instance.FilterTypes, this.name);
    }

    public int GetRendererCount()
    {
        //return GetTreeRendererers().Length;
        if (TreeRenderers == null)
        {
            return 0;
        }
        return TreeRenderers.Length;
    }

    public MeshRenderer[] GetTreeRendererers()
    {
        //List<MeshRenderer> results = new List<MeshRenderer>();

        //MeshRenderer[] renders = null;
        //var target = GetTarget();
        //if(TreeRenderers!=null){
        //    renders= TreeRenderers;
        //}
        //else if(target != null){
        //    renders = target.GetComponentsInChildren<MeshRenderer>(true);
        //}
        //else{
        //    renders= TreeRenderers;
        //}
        //results = renders.ToList().FindAll(i => i != null);
        ////return results.ToArray();

        //MeshRendererInfoList infoList = new MeshRendererInfoList(results);
        //infoList.RemoveTypes(new List<MeshRendererType>() { MeshRendererType.LOD, MeshRendererType.Static },this.name);
        //return infoList.GetRenderers().ToArray();
        return TreeRenderers.GetRenderers().ToArray();
    }

    public void CreateCells_TreeEx()
    {
        if (AreaTreeManager.Instance.IsByLOD)
        {
            CreateCells_Tree_LOD();
        }
        else
        {
            CreateCells_Tree();
        }
    }

    [ContextMenu("2.CreateCells_Tree")]
    public void CreateCells_Tree()
    {
        DateTime start=DateTime.Now;
        //ClearChildren();
        MeshRenderer[] renderers=GetTreeRendererers();
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"CreateCells_Tree Start tree:{this.name} renderers:{renderers.Length} ");
            return;
        }
        else
        {
            //Debug.Log($"CreateCells_Tree Start tree:{this.name} renderers:{renderers.Length} ");
        }
        
        foreach(var render in renderers){
            if(render==null)continue;
            render.enabled=true;
        }
        Bounds bounds=ColliderHelper.CaculateBounds(renderers);
        // Debug.LogError("size:"+bounds.size);
        // Debug.LogError("size2:"+bounds.size/2);

        this.TreeNodes.Clear();

        nodeStatics.LevelDepth = 0;
        GameObject rootCube=AreaTreeHelper.CreateBoundsCube(bounds,$"RootNode",null, GetCubePrefabId());
        AreaTreeNode node=rootCube.AddComponent<AreaTreeNode>();
        //DestoryNodes();
        this.RootNode=node;
        this.TreeNodes.Add(node);        node.Bounds=bounds;
        node.AddRenderers(renderers.ToList());
        
        node.CreateSubNodes(0,0,this,GetCubePrefabId());
        var allCount=this.TreeNodes.Count;
        
        int cellCount=ClearEmptyNodes();
        nodeStatics.CellCount = cellCount;
        if(cellCount!=0)
        {
            nodeStatics.AvgCellRendererCount =(int)(renderers.Length/cellCount);
        }
        else{
            nodeStatics.AvgCellRendererCount =0;
        }
        
        if(rootCube==null){
            return;
        }
        rootCube.transform.SetParent(this.transform);

        this.GetVertexCount();

        //Debug.LogWarning($"CreateCells_Tree End tree:{this.name} cellCount:{cellCount}/{allCount},\tavg:{nodeStatics.AvgCellRendererCount},\t{(DateTime.Now-start).TotalMilliseconds:F1}ms ");
    }

    [ContextMenu("2.CreateCells_Tree_LOD")]
    public void CreateCells_Tree_LOD()
    {
        DateTime start = DateTime.Now;
        //ClearChildren();
        MeshRenderer[] renderers1 = GetTreeRendererers();
        MeshRendererInfoList renderers = MeshRendererInfo.GetLodN(renderers1, 0);
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"CreateCells_Tree_LOD Start tree:{this.name} renderers:{renderers.Length} ");
            return;
        }
        Debug.Log($"CreateCells_Tree_LOD Start tree:{this.name} renderers:{renderers.Length} ");
        foreach (var render in renderers)
        {
            if (render == null) continue;
            render.enabled = true;
        }
        Bounds bounds = MeshRendererInfoList.CaculateBounds(renderers);
        // Debug.LogError("size:"+bounds.size);
        // Debug.LogError("size2:"+bounds.size/2);

        this.TreeNodes.Clear();

        nodeStatics.LevelDepth = 0;
        GameObject rootCube = AreaTreeHelper.CreateBoundsCube(bounds, $"RootNode", null, GetCubePrefabId());
        AreaTreeNode node = rootCube.AddComponent<AreaTreeNode>();
        //DestoryNodes();
        this.RootNode = node;
        this.TreeNodes.Add(node); 
        node.Bounds = bounds;
        node.AddRenderers(renderers);

        node.CreateSubNodes(0, 0, this, GetCubePrefabId());
        var allCount = this.TreeNodes.Count;

        int cellCount = ClearEmptyNodes();
        nodeStatics.CellCount = cellCount;
        if (cellCount != 0)
        {
            nodeStatics.AvgCellRendererCount = (int)(renderers.Length / cellCount);
        }
        else
        {
            nodeStatics.AvgCellRendererCount = 0;
        }

        if (rootCube == null)
        {
            return;
        }
        rootCube.transform.SetParent(this.transform);

        this.GetVertexCount();

        Debug.LogWarning($"CreateCells_Tree_LOD End cellCount:{cellCount}/{allCount},\tavg:{nodeStatics.AvgCellRendererCount},\t{(DateTime.Now - start).TotalMilliseconds:F1}ms");
    }



    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        SetRenderersVisible(true);
    }

    [ContextMenu("HideRenderers")]
    public void HideRenderers()
    {
        SetRenderersVisible(false);
    }

    public void SetRenderersVisible(bool isVisible)
    {
        DateTime start = DateTime.Now;

        //var target = GetTarget();


        var trenderers = TreeRenderers;
        if (trenderers != null && trenderers.Length > 0)
        {
            int count = 0;
            foreach (MeshRendererInfo render in trenderers)
            {
                if (render == null) continue;
                render.SetVisible(isVisible);
                count++;
            }
            //Debug.Log($"SetRenderersVisible isVisible:{isVisible} renderers:{count}/{trenderers.Length} tree:{this.name},\t{(DateTime.Now - start).ToString()}");
        }

        //else
        //{
        //    if (TreeRenderers != null)
        //    {
        //        foreach (var render in TreeRenderers)
        //        {
        //            if (render) continue;
        //            render.enabled = isVisible;
        //            render.gameObject.SetActive(isVisible);
        //        }
        //        Debug.Log($"ShowRenderers3 renderers:{TreeRenderers.Length},\t{(DateTime.Now - start).ToString()}");
        //    }
        //    else
        //    {
        //        Debug.Log($"ShowRenderers4 renderers:0,\t{(DateTime.Now - start).ToString()}");
        //    }
        //}
    }

    //[ContextMenu("1.AddColliders")]
    public void AddColliders()
    {
        //DateTime start=DateTime.Now;
        var renderers=GetTreeRendererers();
        foreach(var render in renderers){
            if(render==null)continue;
            MeshCollider collider=render.gameObject.GetComponent<MeshCollider>();
            if(collider==null){
                collider=render.gameObject.AddComponent<MeshCollider>();
            }
        }
        //Debug.LogWarning($"AddColliders renderers:{renderers.Length},\t{(DateTime.Now-start).ToString()}");
    }

    ////[ContextMenu("ClearDictionary")]
    //public void ClearDictionary()
    //{
    //    AreaTreeHelper.render2NodeDict.Clear();
    //}

    [ContextMenu("CreateDictionary")]
    public void CreateDictionary(bool showLog=false)
    {
        //Debug.LogError($"CreateDictionary Start");
        DateTime start=DateTime.Now;
        //ClearDictionary();
        //foreach(AreaTreeNode tn in TreeNodes)
        //{
        //    if(tn==null)continue;
        //    if(tn.gameObject==null)continue;
        //    tn.CreateDictionary();
        //}
        foreach (AreaTreeNode tn in TreeLeafs)
        {
            if (tn == null) continue;
            if (tn.gameObject == null) continue;
            tn.CreateDictionary();
        }
        if(showLog)
            Debug.LogWarning($"CreateDictionary tree:{this.name}, \trender2NodeDict:{AreaTreeHelper.render2NodeDict.Count}, \t{(DateTime.Now - start).TotalMilliseconds:F1}ms");
    }
    [ContextMenu("* GenerateMesh")]
    private void GenerateMesh()
    {
        GenerateMesh(null);
    }

    [ContextMenu("DestroyBoundBox")]
    public void DestroyBoundBox()
    {
        foreach (AreaTreeNode tn in TreeNodes)
        {
            if (tn == null) continue;
            if (tn.gameObject == null) continue;
            tn.DestroyBoundBox();
        }
    }

    public void GenerateMesh(Action<ProgressArg> progressChanged)
    {
         DateTime start=DateTime.Now;

        MeshRenderer[] renderers = GetTreeRendererers();
        if (renderers.Length == 0)
        {
            //Debug.LogWarning("GenerateMesh renderers:" + renderers.Length + "|tree:" + this.name);
            return;
        }
        else
        {
            //Debug.Log("GenerateMesh renderers:" + renderers.Length + "|tree:" + this.name);
        }



        //ShowRenderers();
        AddColliders();
        CreateCells_TreeEx();


        if (AreaTreeManager.Instance.isCombine)
        {
            CombineMesh(progressChanged);
        }
        else
        {
            MoveRenderers();
        }

        CreateDictionary();
        foreach (var item in TreeNodes)
        {
            if (!item.IsLeaf)
            {
                item.Renderers = new List<MeshRenderer>();
            }
        }

        //InitRenderers();
        //MoveRenderers();

        Debug.LogWarning($"ModelAreaTree.GenerateMesh name:{this.name} renderers:{renderers.Length} time:{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("* GenerateTree")]
    public void GenerateTree()
    {
         DateTime start=DateTime.Now;

        //ShowRenderers();
        AddColliders();
        CreateCells_TreeEx();
        // CombineMesh();
        // CreateDictionary();
        int vertex = GetVertexCount();
        double ms = (DateTime.Now - start).TotalMilliseconds;
        Debug.LogWarning($"GenerateTree name:{this.name} leafNodes:{TreeLeafs.Count} vertex:{vertex} time:{ms:F1}");
    }

    void Start()
    {
        // if(AreaTreeHelper.render2NodeDict.Count==0)
        // {
        //     CreateDictionary();
        // }
        if (AreaTreeNodeShowManager.Instance)
        {
            AreaTreeNodeShowManager.Instance.RegistHiddenTree(this);
        }
        //CreateDictionary();
    }

    //public bool IsSceneLoaded()
    //{
    //    var scenes = this.GetComponentsInChildren<SubScene_Base>(true);
    //    foreach (var scene in scenes)
    //    {
    //        if (scene.IsLoaded == false) return false;
    //    }
    //    base.IsSceneLoaded
    //    return true;
    //}

    private void OnDestroy()
    {
        if (AreaTreeNodeShowManager.Instance)
        {
            AreaTreeNodeShowManager.Instance.UnRegistHiddenTree(this);
        }
    }

    public Vector3 LeafCellSize;

    [ContextMenu("DestroyNodeRender")]
    public void DestroyNodeRender()
    {
        foreach(AreaTreeNode tn in TreeNodes)
        {
            if(tn==null)continue;
            tn.DestroySelfRenderer();
        }
    }

    private int ClearEmptyNodes()
    {
        TreeLeafs.Clear();
        int cellCount=0;
        LeafCellSize=Vector3.zero;
        nodeStatics.MaxCellRendererCount=int.MinValue;
        nodeStatics.MinCellRendererCount =int.MaxValue;
        //foreach(AreaTreeNode tn in TreeNodes)
        foreach (AreaTreeNode tn in TreeNodes)
        {
            if(tn==null)continue;
            if(tn.gameObject==null)continue;

            int renderCount = tn.RendererCount;
            if(renderCount==0){
                GameObject.DestroyImmediate(tn.gameObject);
            }
            else{
                cellCount++;
                //tn.name+="_"+renderCount;
            }

            if(tn==null)continue;
            if(tn.gameObject==null)continue;
            if(tn.Nodes.Count!=0){
                tn.DestroySelfRenderer();
            }
            else{

                if (renderCount > nodeStatics.MaxCellRendererCount || nodeStatics.MaxCellRendererCount == 0)
                {
                    //Debug.Log($"MaxCellRendererCount:{renderCount},node:{tn.name}");
                    nodeStatics.MaxCellRendererCount = renderCount;
                }
                if (renderCount < nodeStatics.MinCellRendererCount || nodeStatics.MinCellRendererCount == 0)
                {
                    nodeStatics.MinCellRendererCount = renderCount;
                }

                float vertexCount = tn.VertexCount;
                if ( ( vertexCount<nodeSetting.MaxVertexCount && vertexCount > nodeStatics.MaxNodeVertexCount) || nodeStatics.MaxNodeVertexCount == 0)
                {
                    // Debug.Log($"MaxNodeVertexCount1:{vertexCount},node:{tn.name}");
                    nodeStatics.MaxNodeVertexCount = vertexCount;
                }

                if (vertexCount > nodeStatics.MaxNodeVertexCount2 || nodeStatics.MaxNodeVertexCount2 == 0)
                {
                    // Debug.Log($"MaxNodeVertexCount2:{vertexCount},node:{tn.name}");
                    nodeStatics.MaxNodeVertexCount2 = vertexCount;
                }
                if (vertexCount < nodeStatics.MinNodeVertexCount || nodeStatics.MinNodeVertexCount == 0)
                {
                    nodeStatics.MinNodeVertexCount = vertexCount;
                }

                TreeLeafs.Add(tn);
                if(LeafCellSize==Vector3.zero)
                    LeafCellSize=tn.Bounds.size;
            }
        }
        return cellCount;
    }

    public AreaTreeNodeSetting nodeSetting = new AreaTreeNodeSetting();

    public AreaTreeNodeStatics nodeStatics = new AreaTreeNodeStatics();

    //public bool IsCopy=false;

    [ContextMenu("ShowNodes")]
    public void ShowNodes()
    {
         DateTime start=DateTime.Now;
        RootNode.ShowNodes();
        Debug.LogError($"ShowNodes {(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("HideNodes")]
    public void HideNodes()
    {
        DateTime start=DateTime.Now;
        RootNode.HideNodes();
        Debug.LogError($"HideNodes {(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("ShowLeafNodes")]
    public void ShowLeafNodes()
    {
        DateTime start=DateTime.Now;
        foreach(AreaTreeNode node in TreeLeafs)
        {
            node.ShowNodes();
        }
        Debug.LogError($"ShowLeafNodes {(DateTime.Now-start).ToString()} TreeLeafs:{TreeLeafs.Count} name:{this.name}");
    }

    [ContextMenu("HideLeafNodes")]
    public void HideLeafNodes()
    {
        if (TreeLeafs.Count == 0) return;
        DateTime start=DateTime.Now;
        foreach(AreaTreeNode node in TreeLeafs)
        {
            node.HideNodes();
        }
        //Debug.Log($"HideLeafNodes {(DateTime.Now-start).ToString()} TreeLeafs:{TreeLeafs.Count} name:{this.name}");
    }

    [ContextMenu("DestoryNodes")]
    public void DestoryNodes()
    {
        Debug.LogError("DestoryNodes:"+this.name);
        ////var target = GetTarget();
        //if(IsCopy){
        //    if(RootNode!=null)
        //        GameObject.DestroyImmediate(RootNode.gameObject);
        //}
        //else
        {
            IdDictionary.InitInfos();
            foreach(var node in TreeLeafs){
                node.RecoverParent();
                //var renders = node.GetRenderers();
                //foreach (var render in renders)
                //{
                //    render.enabled=true;
                //    render.transform.SetParent(target.transform);
                //}
            }
            if(RootNode!=null)
                GameObject.DestroyImmediate(RootNode.gameObject);
        }
    }

    [ContextMenu("MoveRenderers")]
    public void MoveRenderers()
    {
        //List<RendererId> idsAll = new List<RendererId>();
         foreach (var node in TreeLeafs){
            List<RendererId> ids=node.MoveRenderers();
            //idsAll.AddRange(ids);
        }
        //IdDictionary.SaveChildrenIds(idsAll,Target);
        //return idsAll;
    }

    [ContextMenu("MoveRenderers")]
    public List<RendererId> InitRenderers()
    {
        List<RendererId> idsAll = new List<RendererId>();
        foreach (var node in TreeLeafs)
        {
            if (node == null) continue;
            List<RendererId> ids = node.InitRenderers();
            idsAll.AddRange(ids);
        }
        //IdDictionary.SaveChildrenIds(idsAll,Target);
        return idsAll;
    }

    [ContextMenu("RecoverParent")]
    public void RecoverParent(Transform parent)
    {
        bool flag = true;
         foreach(var node in TreeLeafs){
            if (node.RecoverParent(parent) == false)
            {
                flag = false;
            }
        }
        if (flag == false)
        {
            Debug.LogWarning($"AreaTreeNode.RecoverParent pGo.name.Contains(_Renderers) flag == false tree:{this.name} parent:{parent}");
        }
    }

    [ContextMenu("RecoverParentEx")]
    public void RecoverParentEx(Transform parent)
    {
        IdDictionary.InitInfos();
        RecoverParent(parent);
    }

    //[ContextMenu("RecoverParent")]
    //public List<MeshRenderer> GetCombinedRenderers()
    //{
    //    List<MeshRenderer> renderers=new List<MeshRenderer>();
    //     foreach(var node in TreeLeafs){
    //           renderers.AddRange(node.GetCombinedRenderers());
    //    }
    //    Debug.Log("GetCombinedRenderers:"+renderers);
    //    return renderers;
    //}

    public int VertexCount;

    //[ContextMenu("GetVertexCount")]
    public int GetVertexCount()
    {
        DateTime start = DateTime.Now;
        VertexCount = 0;
        var renderers = GetTreeRendererers();
        foreach(var render in renderers)
        {
            MeshFilter mf = render.GetComponent<MeshFilter>();
            if (mf == null) continue;
            if(mf.sharedMesh!=null)
                VertexCount += mf.sharedMesh.vertexCount;
        }
        VertexCount = VertexCount / 10000;
        //Debug.Log($"ModelAreaTree.GetVertexCount tree:{this.name} VertexCount:{VertexCount} time:{(DateTime.Now - start).TotalMilliseconds}ms");
        return VertexCount;
    }

    [ContextMenu("GetMaterials")]
    public void GetMaterials()
    {
        DateTime start = DateTime.Now;

        //ShowRenderers();

        List<string> matKeys = new List<string>();
        List<Material> mats = new List<Material>();
        //var target = GetTarget();
        var renders = GetTreeRendererers();
        foreach (var render in renders)
        {
            if (!mats.Contains(render.sharedMaterial))
            {
                mats.Add(render.sharedMaterial);
            }
        }

        //foreach(var mat in mats)
        //{
        //    MeshCombineHelper.MatInfo matInfo = new MeshCombineHelper.MatInfo(mat);
        //    matKeys.Add(matInfo.Key);
        //}

        //matKeys.Sort();
        //string txt = "";
        //foreach(var key in matKeys)
        //{
        //    txt += key + "\n";
        //}

        //List<string> matKeys2 = new List<string>();
        //Dictionary<string, MeshCombineHelper.MatInfo> infos = new Dictionary<string, MeshCombineHelper.MatInfo>();
        //foreach (var mat in mats)
        //{
        //    //var list = mat2Filters[mat];
        //    MeshCombineHelper.MatInfo info = new MeshCombineHelper.MatInfo(mat);
        //    if (infos.ContainsKey(info.Key))
        //    {
        //        var item = infos[info.Key];
        //        //item.AddList(list);
        //    }
        //    else
        //    {
        //        infos.Add(info.Key, info);
        //        //info.AddList(list);

        //        matKeys2.Add(info.Key);
        //    }
        //}

        //matKeys2.Sort();
        //string txt2 = "";
        //foreach (var key in matKeys2)
        //{
        //    txt2 += key + "\n";
        //}

        var matsEx = MeshCombineHelper.GetMatFilters(renders, false);
        //foreach(var mat in mats.Keys)
        //{
        //    var list = mats[mat];
        //    foreach(var item in list)
        //    {
        //        MeshRenderer renderer = item.GetComponent<MeshRenderer>();
        //        renderer.sharedMaterial = mat;
        //    }
        //}

        Debug.LogError($"GetMaterials {(DateTime.Now - start).ToString()},mats1:{mats.Count},mats2:{matsEx.Count},count:{renders.Length}");
    }


    [ContextMenu("SetMaterials")]
    public void SetMaterials()
    {
        DateTime start = DateTime.Now;

        //ShowRenderers();

        //var target = GetTarget();
        var renderers = GetTreeRendererers();
        var mats = MeshCombineHelper.GetMatFilters(renderers, true);

        Debug.LogError($"SetMaterials {(DateTime.Now - start).ToString()},mats:{mats.Count}");
    }

    //[ContextMenu("ShowModelInfo")]
    private void ShowModelInfo()
    {
        int renderCount = 0;
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>();
        int vertextCount = 0;
        List<Material> mats = new List<Material>();
        foreach (var render in renderers)
        {
            if (render.enabled == false) continue;
            if (!mats.Contains(render.sharedMaterial))
            {
                mats.Add(render.sharedMaterial);
            }
            MeshFilter meshFilter = render.GetComponent<MeshFilter>();
            vertextCount += meshFilter.sharedMesh.vertexCount;
            renderCount++;
            // Debug.Log("render:"+render.name+"|"+render.transform.parent);
        }
        int w = vertextCount / 10000;

        //ModelInfoText.text = $"renders:{renderCount}(+{renderCount - lastRenderCount}),mats:{mats.Count}(+{mats.Count - lastMatCount}) \nvertext:{w}w(+{w - lastVertextCount})";

        //lastRenderCount = renderCount;
        //lastVertextCount = w;
        //lastMatCount = mats.Count;

        Debug.LogError($"ShowModelInfo renders:{renderCount},mats:{mats.Count},vertext:{w}w");
    }

    //[ContextMenu("SaveMeshes")]
    public void SaveMeshes(string dir)
    {
#if UNITY_EDITOR
        DateTime start = DateTime.Now;

        for (int i = 0; i < TreeLeafs.Count; i++)
        {
            var leafNode = TreeLeafs[i];
            if (leafNode == null) continue;
            float progress = (float)i / TreeLeafs.Count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("SaveMeshes", $"{i}/{TreeLeafs.Count} {percents:F1}%", progress))
            {
                break;
            }
            leafNode.SaveMeshes(dir+"/Meshes/");

            string prefabPath1 = dir+"/Nodes/" + leafNode.name+ "_Combined.prefab";
            var nodeCombined=UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(leafNode.combindResult, prefabPath1,UnityEditor.InteractionMode.UserAction);
            // GameObject.DestroyImmediate(leafNode.combindResult);
            // leafNode.combindResult=nodeCombined;

            string prefabPath2 = dir+"/Nodes/" + leafNode.name+ "_Renderers.prefab";
            var nodeRenderers=UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(leafNode.renderersRoot, prefabPath2,UnityEditor.InteractionMode.UserAction);
            GameObject.DestroyImmediate(leafNode.renderersRoot);
            leafNode.renderersRoot=nodeRenderers;
        }
        ProgressBarHelper.ClearProgressBar();

        Debug.LogError($"SaveMeshes {(DateTime.Now - start).ToString()}");
#endif
    }


    [ContextMenu("SaveTree")]
    public void SaveTree()
    {
#if UNITY_EDITOR
        DateTime start = DateTime.Now;

        //string guid = UnityEditor.AssetDatabase.CreateFolder("Assets", "My Folder");
        //Debug.LogError("guid:" + guid);
        //string newFolderPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        //Debug.LogError("newFolderPath:" + newFolderPath);

        string parentDir = "Assets/Models/Instances/Trees";
        //var target = GetTarget();
        string treeName= this.name+"_"+ this.gameObject.GetInstanceID();
        string treePath = UnityEditor.AssetDatabase.GUIDToAssetPath( UnityEditor.AssetDatabase.CreateFolder(parentDir, treeName));
        Debug.LogError("treePath:" + treePath);
         string meshPath = UnityEditor.AssetDatabase.GUIDToAssetPath( UnityEditor.AssetDatabase.CreateFolder(treePath, "Meshes"));
        Debug.LogError("meshPath:" + meshPath);
        string nodePath = UnityEditor.AssetDatabase.GUIDToAssetPath( UnityEditor.AssetDatabase.CreateFolder(treePath, "Nodes"));
        Debug.LogError("nodePath:" + nodePath);

        SaveMeshes(treePath);

        string prefabPath = treePath+"/" + treeName+ ".prefab";
        GameObject obj=UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(this.gameObject, prefabPath,UnityEditor.InteractionMode.UserAction);

        Debug.LogError($"SaveTree {(DateTime.Now - start).ToString()}");
#endif
    }

    [ContextMenu("SaveRenderersId")]
    public void SaveRenderersId()
    {
        foreach (var node in TreeLeafs)
        {
            node.SaveRenderersId();
        }

        //TargetId = RendererId.GetId(Target,0);

        //Debug.Log("SaveRenderersId:"+this.name);
    }

    [ContextMenu("LoadRenderers")]
    public void LoadRenderers()
    {
        foreach (var node in TreeLeafs)
        {
            node.LoadRenderers();
        }
        //Target = IdDictionary.GetGo(TargetId);
        //Debug.Log("LoadRenderers:"+this.name);
    }

#if UNITY_EDITOR
    [ContextMenu("* EditorCreateNodeScenes")]
    public void EditorCreateNodeScenes()
    {
        EditorCreateNodeScenes(null);
    }

    public bool IsScenesFolderExists()
    {
        string dir = SubSceneManager.Instance.GetSceneDir(SceneContentType.TreeNode, this.gameObject);
        string dirPath = Application.dataPath + "/" + dir;
        return System.IO.Directory.Exists(dirPath);
    }

    public void SelectScenesFolder()
    {
        string dir=SubSceneManager.Instance.GetSceneDir(SceneContentType.TreeNode, this.gameObject);
        Debug.Log(dir);
        string folderPath = "Assets/" + dir;
        Debug.Log(folderPath);
        UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(folderPath, typeof(UnityEngine.Object));
        Debug.Log(asset);
        if (asset != null)
        {
            EditorHelper.SelectObject(asset);
        }
        string dirPath= Application.dataPath + "/" + dir;
        Debug.Log(dirPath);
        Debug.Log(System.IO.Directory.Exists(dirPath));
    }

    public UnityEngine.Object GetScenesFolder()
    {
        string dir = SubSceneManager.Instance.GetSceneDir(SceneContentType.TreeNode, this.gameObject);
        string folderPath = "Assets/" + dir;
        UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(folderPath, typeof(UnityEngine.Object));
        string dirPath = Application.dataPath + "/" + dir;
        Debug.Log($"GetScenesFolder tree:{this.name} \nfolderPath:{folderPath} asset:[{asset}] \ndirPath:{dirPath} Exists:{System.IO.Directory.Exists(dirPath)}");
        return asset;
    }

    public void DeleteScenesFolder()
    {
        DeleteScenesFolder(true);
    }

    public void DeleteScenesFolder(bool isRefresh)
    {
        string dir = SubSceneManager.Instance.GetSceneDir(SceneContentType.TreeNode, this.gameObject);
        Debug.Log(dir);
        string folderPath = "Assets/" + dir;
        Debug.Log(folderPath);
        UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath(folderPath, typeof(UnityEngine.Object));
        Debug.Log(asset);
        //if (asset != null)
        //{
        //    EditorHelper.SelectObject(asset);
        //}
        string dirPath = Application.dataPath + "/" + dir;
        Debug.Log(dirPath);
        Debug.Log(System.IO.Directory.Exists(dirPath));

        if (System.IO.Directory.Exists(dirPath))
        {
            //System.IO.Directory.Delete(dirPath, true);

            UnityEditor.AssetDatabase.DeleteAsset(folderPath);
            if(isRefresh)
                EditorHelper.RefreshAssets();

            DestroyScenes();
        }
    }

    public void EditorCreateNodeScenes(Action<ProgressArg> progressChanged)
    {
        if (TreeLeafs.Count == 0) return;
        //if (TreeLeafs.Count == 0)
        //{
        //    return;
        //}
        //if(SceneList!=null&& SceneList.sceneCount>0){
        //    Debug.LogError("ModelAreaTree.EditorCreateNodeScenes SceneList!=null&& SceneList.sceneCount>0 "+this.name);
        //    if(progressChanged!=null){
        //        progressChanged(1);
        //    }
        //    return ;
        //}

        DateTime start = DateTime.Now;

        MoveRenderers();
        
        for (int i = 0; i < TreeLeafs.Count; i++)
        {
            var leafNode = TreeLeafs[i];
            if (leafNode == null) continue;
            float progress = (float)i / TreeLeafs.Count;
            float percents = progress * 100;

            var p1 = new ProgressArg("EditorCreateNodeScenes", i, TreeLeafs.Count, leafNode);

            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                {
                    break;
                }
            }
            else
            {
                progressChanged(p1);
            }

            leafNode.EditorCreateNodeScenes(false,(p2)=>
            {
                p1.AddSubProgress(p2);
                //float progress2 = (float)(i+p) / TreeLeafs.Count;
                //float percents2 = progress2 * 100;

                if (progressChanged == null)
                {
                    if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                    {
                        
                    }
                }
                else
                {
                    progressChanged(p1);
                }
            });
        }

        GetScenes();
       
        if (progressChanged == null)
        {
            EditorHelper.RefreshAssets();
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(new ProgressArg("EditorCreateNodeScenes", TreeLeafs.Count, TreeLeafs.Count));
        }

        UpdateSceneList();

        if (progressChanged == null) Debug.LogError($"ModelAreaTree.EditorCreateNodeScenes time:{(DateTime.Now - start)}");
        //return idsAll;
    }

    [ContextMenu("* EditorLoadNodeScenesEx")]
    public void EditorLoadNodeScenesEx()
    {
        IdDictionary.InitInfos();
        EditorLoadNodeScenes(null);
    }

    [ContextMenu("* EditorLoadNodeScenes")]
    private void EditorLoadNodeScenes()
    {
        EditorLoadNodeScenes(null);
    }

    public void EditorLoadNodeScenes(Action<ProgressArg> progressChanged)
    {
        if (TreeLeafs.Count == 0)
        {
            return;
        }
        DateTime start = DateTime.Now;

        for (int i = 0; i < TreeLeafs.Count; i++)
        {
            var leafNode = TreeLeafs[i];
            if (leafNode == null) continue;
            //float progress = (float)i / TreeLeafs.Count;
            //float percents = progress * 100;

            var p1 = new ProgressArg("EditorLoadNodeScenes", i, TreeLeafs.Count, leafNode);

            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                {
                    break;
                }
            }
            else
            {
                progressChanged(p1);
            }
            leafNode.EditorLoadScenes((p) =>
            {
                p1.AddSubProgress(p);

                //float progress2 = (float)(i + p) / TreeLeafs.Count;
                //float percents2 = progress2 * 100;

                if (progressChanged == null)
                {
                    if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                    {

                    }
                }
                else
                {
                    progressChanged(p1);
                }
            });
        }

        if (progressChanged == null)
        {
            EditorHelper.RefreshAssets();
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(new ProgressArg("EditorLoadNodeScenes", TreeLeafs.Count, TreeLeafs.Count));
        }

        Debug.Log($"ModelAreaTree.EditorLoadNodeScenes time:{(DateTime.Now - start)} tree:{this.name}");
    }

    // public void UnLoadScenes()
    // {
    //     UnLoadScenes(null);
    // }

    public void UnLoadLeafScenes(Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;

        for (int i = 0; i < TreeLeafs.Count; i++)
        {
            var leafNode = TreeLeafs[i];
            if (leafNode == null) continue;
            float progress = (float)i / TreeLeafs.Count;
            float percents = progress * 100;

            if (progressChanged == null)
            {
                if (ProgressBarHelper.DisplayCancelableProgressBar("EditorLoadNodeScenes", $"Progress1 {i}/{TreeLeafs.Count} {percents:F2}%", progress))
                {
                    break;
                }
            }
            else
            {
                progressChanged(progress);
            }
            leafNode.UnLoadScenes();
        }

        if (progressChanged == null)
        {
            EditorHelper.RefreshAssets();
            ProgressBarHelper.ClearProgressBar();
        }
        else
        {
            progressChanged(1);
        }

        Debug.Log($"ModelAreaTree.EditorLoadNodeScenes time:{(DateTime.Now - start)}");
    }
#endif

    [ContextMenu("CreateLOD")]
    public void CreateLOD()
    {
        DateTime start = DateTime.Now;

        for (int i = 0; i < TreeLeafs.Count; i++)
        {
            var leafNode = TreeLeafs[i];
            if (leafNode == null) continue;
            float progress = (float)i / TreeLeafs.Count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("CreateLOD", $"P1 node:{leafNode.name} vertex:{leafNode.VertexCount:F1} {i}/{TreeLeafs.Count} {percents:F1}%", progress))
            {
                break;
            }
            leafNode.CreateLOD((p,f)=>
            {
                float progress2 = (float)(i+p) / TreeLeafs.Count;
                float percents2 = progress2 * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CreateLOD", $"P2 node:{leafNode.name} vertex:{leafNode.VertexCount:F1} {(i + p):F1}/{TreeLeafs.Count} {percents2:F1}%", progress2))
                {
                    //break;
                }
            });
        }
        ProgressBarHelper.ClearProgressBar();

        Debug.Log($"CreateLOD[{this.name}] vertexCount:{this.GetVertexCount()} {(DateTime.Now - start).ToString()}");
    }
}
