using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ModelAreaTree : MonoBehaviour
{
   public GameObject Target=null;

    public GameObject TargetCopy = null;

    public bool IsFirst = true;
    public GameObject GetTarget()
    {
        if (IsCopy && IsFirst)
        {
            IsFirst = false;
            Debug.Log("GetTarget Target1:"+Target);
            Target.SetActive(false);
            TargetCopy = MeshHelper.CopyGO(Target);
            TargetCopy.SetActive(true);
            Debug.Log("GetTarget Target2:"+TargetCopy);
        }

        if (!IsCopy)
        {
            TargetCopy = Target;
        }
        return TargetCopy;
    }

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
        var target = GetTarget();
        DateTime start = DateTime.Now;
        Renderer[] renders = target.GetComponentsInChildren<Renderer>();
        Bounds bounds=ColliderHelper.CaculateBounds(renders);
        AreaTreeHelper.CreateBoundsCube(bounds, target.name+"_TargetBound",transform);

        Debug.LogWarning($"target:{target.name},renders:{renders.Length},bounds:{bounds}");
        Debug.LogWarning($"CreateBoundes \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("CreateSubBoundes")]
    public void CreateSubBoundes()
    {
        //ClearChildren();

        DateTime start = DateTime.Now;
        var target = GetTarget();
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < target.transform.childCount; i++)
        {
            children.Add(target.transform.GetChild(i));
        }
        Bounds boundsAll = new Bounds();
        for (int i = 0; i < children.Count; i++)
        {
            Transform child = children[i];
            //var child = this.transform.GetChild(i);
            Renderer[] renders = child.GetComponentsInChildren<Renderer>();
            Bounds bounds = ColliderHelper.CaculateBounds(renders);
            AreaTreeHelper.CreateBoundsCube(bounds, child.name+"_TargetBound", transform);

            
            boundsAll.Encapsulate(bounds);

            Debug.LogWarning($"[{i}] target:{child.name},renders:{renders.Length},bounds:{bounds},boundsAll:{boundsAll}");
        }
        Debug.LogWarning($"CreateSubBoundes \t{(DateTime.Now - start).ToString()}");

        AreaTreeHelper.CreateBoundsCube(boundsAll, target.name + "_TargetBoundAll", transform);
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

    public bool IsHidden=false;

    [ContextMenu("CombineMesh")]
    public void CombineMesh()
    {   
        //IsCombined=true;
        DateTime start=DateTime.Now;
        int renderCount=0;
        foreach(AreaTreeNode node in TreeNodes)
        {
            if(node==null)continue;
            if(node.Nodes.Count>0)continue;
            node.CombineMesh();
            renderCount += node.RendererCount;
        }

        int newRenderCount=0;
        if(this.RootNode==null)
        {
            Debug.LogError("CombineMesh this.RootNode==null£¡£¡:"+this.name);
            return;
        }
        var renderersNew=this.RootNode.GetComponentsInChildren<MeshRenderer>();
        foreach(var render in renderersNew){
            if(render.enabled==true)
            {
                newRenderCount++;
            }
        }

        CombinedCount = newRenderCount;

        Debug.LogWarning($"CombineMesh renderCount:{renderCount}->{newRenderCount},\t{(DateTime.Now-start).ToString()}");
    }

    public int CombinedCount = 0;

    [ContextMenu("SwitchToCombined")]
    public void SwitchToCombined()
    {
        DateTime start=DateTime.Now;

        foreach(var node in TreeNodes)
        {
            if(node==null)continue;
            node.SwitchToCombined();
        }

        Debug.LogWarning($"SwitchToCombined \t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("SwitchToRenderers")]
    public void SwitchToRenderers()
    {
        DateTime start=DateTime.Now;

        foreach(var node in TreeNodes)
        {
            if(node==null)continue;
            node.SwitchToRenderers();
        }

        Debug.LogWarning($"SwitchToRenderers \t{(DateTime.Now-start).ToString()}");
    }

    public MeshRenderer[] TreeRenderers;

    public MeshRenderer[] GetTreeRendererers()
    {
        var target = GetTarget();
        if(TreeRenderers!=null&&TreeRenderers.Length>0){
            return TreeRenderers;
        }
        else if(target != null){
            var renderers= target.GetComponentsInChildren<MeshRenderer>();
            return renderers;
        }
        else{
            return TreeRenderers;
        }
    }

    [ContextMenu("2.CreateCells_Tree")]
    public void CreateCells_Tree()
    {
        

        //var allCount=Count.x*Count.y*Count.z;
        DateTime start=DateTime.Now;
        //ClearChildren();

        MeshRenderer[] renderers=GetTreeRendererers();
        
        Debug.Log("CreateCells_Tree renderers:"+renderers.Length);
        foreach(var render in renderers){
            if(render==null)continue;
            render.enabled=true;
        }

        Bounds bounds=ColliderHelper.CaculateBounds(renderers);
        // Debug.LogError("size:"+bounds.size);
        // Debug.LogError("size2:"+bounds.size/2);

        this.TreeNodes.Clear();

        nodeStatics.LevelDepth = 0;
        GameObject rootCube=AreaTreeHelper.CreateBoundsCube(bounds,$"RootNode",null);
        AreaTreeNode node=rootCube.AddComponent<AreaTreeNode>();
        DestoryNodes();
        this.RootNode=node;
        this.TreeNodes.Add(node);

        node.Bounds=bounds;
        node.AddRenderers(renderers.ToList());
        
        node.CreateSubNodes(0,0,this);

        var allCount=this.TreeNodes.Count;
        
        int cellCount=ClearNodes();
        nodeStatics.CellCount = cellCount;
        if(cellCount!=0)
        {
            nodeStatics.AvgCellRendererCount =(int)(renderers.Length/cellCount);
        }
        else{
            nodeStatics.AvgCellRendererCount =0;
        }
        
        if(rootCube==null){
            Debug.LogError("rootCube==null:"+this.name);
            return;
        }
        rootCube.transform.SetParent(this.transform);

        Debug.LogWarning($"CreateCells_Tree cellCount:{cellCount}/{allCount},\tavg:{nodeStatics.AvgCellRendererCount},\t{(DateTime.Now-start).ToString()}");
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

        var target = GetTarget();


        if (TreeRenderers != null && TreeRenderers.Length > 0)
        {
            int count = 0;
            foreach (var render in TreeRenderers)
            {
                if (render == null) continue;
                render.enabled = isVisible;
                render.gameObject.SetActive(isVisible);
                count++;
            }
            Debug.Log($"ShowRenderers1 renderers:{count},\t{(DateTime.Now - start).ToString()}");
        }
        else if (target != null)
        {
            var ts = AreaTreeHelper.GetAllTransforms(target.transform);
            foreach (var t in ts)
            {
                t.gameObject.SetActive(true);
            }
            var renderers = target.GetComponentsInChildren<MeshRenderer>();
            foreach (var render in renderers)
            {
                render.enabled = isVisible;
                render.gameObject.SetActive(isVisible);
            }
            TreeRenderers = renderers;
            Debug.Log($"ShowRenderers2 renderers:{renderers.Length},\t{(DateTime.Now - start).ToString()}");
        }
        else
        {
            if (TreeRenderers != null)
            {
                foreach (var render in TreeRenderers)
                {
                    if (render) continue;
                    render.enabled = isVisible;
                    render.gameObject.SetActive(isVisible);
                }
                Debug.Log($"ShowRenderers3 renderers:{TreeRenderers.Length},\t{(DateTime.Now - start).ToString()}");
            }
            else
            {
                Debug.Log($"ShowRenderers4 renderers:0,\t{(DateTime.Now - start).ToString()}");
            }
        }
    }

    [ContextMenu("1.AddColliders")]
    public void AddColliders()
    {
        DateTime start=DateTime.Now;
        var renderers=GetTreeRendererers();
        foreach(var render in renderers){
            if(render==null)continue;
            MeshCollider collider=render.gameObject.GetComponent<MeshCollider>();
            if(collider==null){
                collider=render.gameObject.AddComponent<MeshCollider>();
            }
        }
        Debug.LogWarning($"AddColliders renderers:{renderers.Length},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("ClearDictionary")]
    public void ClearDictionary()
    {
        AreaTreeHelper.render2NodeDict.Clear();
    }

    [ContextMenu("CreateDictionary")]
    public void CreateDictionary()
    {
        //Debug.LogError($"CreateDictionary Start");
        DateTime start=DateTime.Now;
        //ClearDictionary();
        foreach(AreaTreeNode tn in TreeNodes)
        {
            if(tn==null)continue;
            if(tn.gameObject==null)continue;
            // if(tn.Nodes.Count==0){
            //     foreach(var render in tn.Renderers){
            //         render2NodeDict.Add(render,tn);
            //     }
            // }
            tn.CreateDictionary();
        }
        Debug.LogWarning($"CreateDictionary tree:{this.name},render2NodeDict:{AreaTreeHelper.render2NodeDict.Count},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("* GenerateMesh")]
    public void GenerateMesh()
    {
         DateTime start=DateTime.Now;

        //ShowRenderers();
        AddColliders();
        CreateCells_Tree();
        CombineMesh();
        CreateDictionary();
        foreach (var item in TreeNodes)
        {
            if (!item.IsLeaf)
            {
                item.Renderers = null;
            }
        }
        Debug.LogWarning($"GenerateMesh {(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("* GenerateTree")]
    public void GenerateTree()
    {
         DateTime start=DateTime.Now;

        //ShowRenderers();
        AddColliders();
        CreateCells_Tree();
        // CombineMesh();
        // CreateDictionary();
        Debug.LogWarning($"GenerateTree {(DateTime.Now-start).ToString()}");
    }

    void Start()
    {
        // if(AreaTreeHelper.render2NodeDict.Count==0)
        // {
        //     CreateDictionary();
        // }
        
        CreateDictionary();
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

    private int ClearNodes()
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

                var vertexCount = tn.VertexCount;
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

    public bool IsCopy=false;

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
        Debug.LogError($"ShowLeafNodes {(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("HideLeafNodes")]
    public void HideLeafNodes()
    {
        DateTime start=DateTime.Now;
        foreach(AreaTreeNode node in TreeLeafs)
        {
            node.HideNodes();
        }
        Debug.LogError($"HideLeafNodes {(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("DestoryNodes")]
    public void DestoryNodes()
    {
        var target = GetTarget();
        if(IsCopy){
            if(RootNode!=null)
                GameObject.DestroyImmediate(RootNode.gameObject);
        }
        else{
            foreach(var node in TreeLeafs){
                var renders = node.GetRenderers();
                foreach (var render in renders)
                {
                    render.enabled=true;
                    render.transform.SetParent(target.transform);
                }
            }
            if(RootNode!=null)
                GameObject.DestroyImmediate(RootNode.gameObject);
        }
    }

    [ContextMenu("MoveRenderers")]
    public void MoveRenderers()
    {
         foreach(var node in TreeLeafs){
               node.MoveRenderers();
        }
    }

    [ContextMenu("RecoverParent")]
    public void RecoverParent()
    {
         foreach(var node in TreeLeafs){
               node.RecoverParent();
        }
    }

    [ContextMenu("RecoverParent")]
    public List<MeshRenderer> GetCombinedRenderers()
    {
        List<MeshRenderer> renderers=new List<MeshRenderer>();
         foreach(var node in TreeLeafs){
               renderers.AddRange(node.GetCombinedRenderers());
        }
        Debug.Log("GetCombinedRenderers:"+renderers);
        return renderers;
    }

    public int VertexCount;

    [ContextMenu("GetVertexCount")]
    public void GetVertexCount()
    {
        DateTime start = DateTime.Now;
        VertexCount = 0;
        var renderers = GetTreeRendererers();
        foreach(var render in renderers)
        {
            MeshFilter mf = render.GetComponent<MeshFilter>();
            VertexCount += mf.sharedMesh.vertexCount;
        }
        VertexCount = VertexCount / 10000;
        Debug.Log($"GetVertexCount VertexCount:{VertexCount} time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }

    [ContextMenu("GetMaterials")]
    public void GetMaterials()
    {
        DateTime start = DateTime.Now;

        //ShowRenderers();

        List<string> matKeys = new List<string>();
        List<Material> mats = new List<Material>();
        var target = GetTarget();
        var renders = target.GetComponentsInChildren<MeshRenderer>();
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

        int count = 0;
        var matsEx = MeshCombineHelper.GetMatFilters(target, out count,false);
        //foreach(var mat in mats.Keys)
        //{
        //    var list = mats[mat];
        //    foreach(var item in list)
        //    {
        //        MeshRenderer renderer = item.GetComponent<MeshRenderer>();
        //        renderer.sharedMaterial = mat;
        //    }
        //}

        Debug.LogError($"GetMaterials {(DateTime.Now - start).ToString()},mats1:{mats.Count},mats2:{matsEx.Count},count:{count}");
    }


    [ContextMenu("SetMaterials")]
    public void SetMaterials()
    {
        DateTime start = DateTime.Now;

        //ShowRenderers();

        var target = GetTarget();
        int count = 0;
        var mats = MeshCombineHelper.GetMatFilters(target, out count,true);

        Debug.LogError($"SetMaterials {(DateTime.Now - start).ToString()},mats:{mats.Count},count:{count}");
    }

    [ContextMenu("ShowModelInfo")]
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

    [ContextMenu("SaveMeshes")]
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
            if (ProgressBarHelper.DisplayCancelableProgressBar("SaveMeshes", $"{i}/{TreeLeafs.Count} {percents}% of 100%", progress))
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
        var target = GetTarget();
        string treeName= target.name+"_"+ target.GetInstanceID();
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
    
}
