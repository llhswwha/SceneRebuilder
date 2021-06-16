using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AreaTreeNode : MonoBehaviour
{
    public Bounds Bounds;

    public GameObject renderersRoot;

    public GameObject collidersRoot;

    public GameObject combindResult;

    public bool IsLeaf = false;

    public List<MeshRenderer> Renderers=new List<MeshRenderer>();

    public MeshRenderer[] CombinedRenderers;

    private List<Transform> RendererParents = new List<Transform>();

    private List<MeshRenderer> newRenderers = new List<MeshRenderer>();

    public List<Collider> colliders = new List<Collider>();

    public List<AreaTreeNode> Nodes = new List<AreaTreeNode>();

    public int RendererCount
    {
        get
        {
            return Renderers.Count;
        }
    }

    public int VertexCount = 0;

    public int GetVertexCount()
    {
        int count = 0;
        foreach(var renderer in Renderers)
        {
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            count += meshFilter.sharedMesh.vertexCount;
        }
        VertexCount = count / 10000;
        return VertexCount;
    }

    public List<MeshRenderer> GetRenderers()
    {
        return Renderers;
    }

    public void AddRenderer(MeshRenderer renderer){
        if (!Renderers.Contains(renderer))
        {
            Renderers.Add(renderer);
        }
        else
        {
            Debug.LogError("AddRenderer Renderers.Contains(renderer):"+renderer);
        }
        
    }

    public void AddRenderers(List<MeshRenderer> renderers)
    {
        Renderers.AddRange(renderers);
    }

    //public static bool TestDebug = true;

    //private bool IsCopyed = false;

    [ContextMenu("CopyRenderers")]
    public void CopyRenderers()
    {
        //if (IsCopyed)
        //{
        //    Debug.LogError("TestDebug IsCopyed==true");
        //    return;
        //}
        //IsCopyed = true;
        newRenderers.Clear();
        RendererParents.Clear();
        colliders.Clear();

        //newRenderers.Clear();
        if(renderersRoot==null)
            renderersRoot =new GameObject(this.name+"_Renderers");

        //renderersRoot.transform.SetParent(this.transform);
        foreach(var render in Renderers){
            if(render==null)continue;
            GameObject go=render.gameObject;
            //TransformParent tp = go.GetComponent<TransformParent>();
            //if(tp!=null)
            //{
            //    if (TestDebug)
            //    {
            //        TestDebug = false;
            //        Debug.LogError("TestDebug tp!=null");
            //    }
                
            //}
            //else
            //{
            //    tp= go.AddComponent<TransformParent>();
            //}
            //tp.Parent = go.transform.parent;

            newRenderers.Add(render);
            if(go.transform.parent!= renderersRoot.transform)
            {
                RendererParents.Add(go.transform.parent);
                go.transform.SetParent(renderersRoot.transform);
            }

            MeshCollider collider = go.GetComponent<MeshCollider>();
            if(collider)
                collider.enabled = false;
            colliders.Add(collider);
        }
    }

    [ContextMenu("MoveRenderers")]
    public void MoveRenderers()
    {
        newRenderers.Clear();
        RendererParents.Clear();

        //newRenderers.Clear();
        if(renderersRoot==null)
            renderersRoot =new GameObject(this.name+"_Renderers");
        foreach(var render in Renderers){
            if(render==null){
                Debug.LogError("render==null:"+this);
                continue;
            }
            GameObject go=render.gameObject;
            newRenderers.Add(render);
            if(go.transform.parent!= renderersRoot.transform)
            {
                RendererParents.Add(go.transform.parent);
                go.transform.SetParent(renderersRoot.transform);
            }
        }

        renderersRoot.transform.SetParent(this.transform);
    }

    private void CreateColliders()
    {
        collidersRoot=MeshHelper.CopyGO(renderersRoot);
        collidersRoot.name=this.name+"_Colliders";
        var renderers=collidersRoot.GetComponentsInChildren<Renderer>();
        foreach(var renderer in renderers)
        {
            GameObject.DestroyImmediate(renderer);
        }
        collidersRoot.transform.SetParent(this.transform);

        var colliders=renderersRoot.GetComponentsInChildren<Collider>();
        foreach(var collider in colliders)
        {
            GameObject.DestroyImmediate(collider);
        }
    }

    public float DistanceToCamera=0;

    public bool IsRendererVisible=true;

    [ContextMenu("HideRenders")]
    public void HideRenders()
    {
        IsRendererVisible=false;
        foreach (var r in Renderers)
        {
            if(r==null)continue;
            r.enabled = false;
        }
    }

     [ContextMenu("ShowRenders")]
    public void ShowRenders()
    {
        IsRendererVisible=true;
        foreach (var r in Renderers)
        {
            if(r==null)continue;
            r.enabled = true;
        }
    }

    [ContextMenu("DestroySelfRenderer")]
    public void DestroySelfRenderer()
    {
        MeshRenderer r=this.GetComponent<MeshRenderer>();
        if(r!=null){
            GameObject.DestroyImmediate(r);
        }
    }

    public int GetNodeCount()
    {
        int count=0;
        foreach(var node in Nodes){
            if(node!=null){
                count++;
            }
        }
        return count;
    }

    [ContextMenu("CombineMesh")]
    public void CombineMesh()
    {

        if(renderersRoot){
            GameObject.DestroyImmediate(renderersRoot);
        }

        DestroySelfRenderer();

        CopyRenderers();

        CombineInner();

        HideRenders();

        renderersRoot.transform.SetParent(this.transform);

        RecoverParent();
    }

    public void RecoverParent()
    {
        for (int i = 0; i < Renderers.Count; i++)
        {
            MeshRenderer render = Renderers[i];
            if(render==null)continue;
            Transform parent = RendererParents[i];
            render.transform.SetParent(parent);
        }
    }

    private void CombineInner()
    {
        if(combindResult){
            GameObject.DestroyImmediate(combindResult);
        }
        combindResult=MeshCombineHelper.CombineEx(this.renderersRoot,1);
        combindResult.name=this.name+"_Combined";
        combindResult.transform.SetParent(this.transform);

        CombinedRenderers= combindResult.GetComponentsInChildren<MeshRenderer>();

        var meshFilters=combindResult.GetComponentsInChildren<MeshFilter>();
        foreach (var mf in meshFilters)
        {
            mf.gameObject.AddComponent<MeshCollider>();
        }
        this.renderersRoot.SetActive(false);
    }

    [ContextMenu("UpdateCombined")]
    public void UpdateCombined()
    {
         DateTime start=DateTime.Now;
        renderersRoot.transform.SetParent(null);

        // CopyRenderers();

        // CombineInner();

        // HideRenders();

        // CreateDictionary();

        // renderersRoot.transform.SetParent(this.transform);

        CombineMesh();

        CreateDictionary();

        Debug.LogError($"UpdateCombined renderCount:{Renderers.Count},\t{(DateTime.Now-start).ToString()}");
    }

    private void SwitchModel(bool isCombined)
    {
        if(this.Nodes.Count>0){
            return;
        }
        if(renderersRoot==null){
            return;
        }
        //newRenderers=renderersRoot.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < Renderers.Count; i++)
        {
            MeshRenderer r = Renderers[i];
            r.gameObject.SetActive(!isCombined);
            r.enabled=!isCombined;
            // var collider = colliders[i];
            // if (collider)
            // {
            //     collider.enabled = !isCombined;
            // }
            MeshCollider collider = r.GetComponent<MeshCollider>();
            collider.enabled = !isCombined;
        }

        combindResult.SetActive(isCombined);
        renderersRoot.SetActive(!isCombined);
    }

    [ContextMenu("SwitchToCombined")]
    public void SwitchToCombined()
    {
        SwitchModel(true);
    }

    [ContextMenu("SwitchToRenderers")]
    public void SwitchToRenderers()
    {
        SwitchModel(false);
    }

    [ContextMenu("ClearChildren")]
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
        
        GameObject.DestroyImmediate(combindResult);
        GameObject.DestroyImmediate(renderersRoot);
    }


    private ModelAreaTree tree;
    public List<AreaTreeNode> CreateSubNodes(int level,int index,ModelAreaTree tree){

        this.tree = tree;

        var nodeSetting = tree.nodeSetting;
        int minLevel = nodeSetting.MinLevel;
        int maxLevel = nodeSetting.MaxLevel;
        //int maxRenderCount= nodeSetting.MaxRenderCount;

        //Bounds bounds=ColliderHelper.CaculateBounds(this.Renderers);
        Bounds bounds=this.Bounds;
        var bs=bounds.size;
        
        //Debug.LogError("CreateSubNodes size:"+bounds.size);

        Vector3 cellCount=new Vector3(1,1,2);

        // if(level%3==0){
        //     cellCount=new Vector3(1,1,2);
        // }
        // else if(level%3==1){
        //     cellCount=new Vector3(1,2,1);
        // }
        // else if(level%3==2){
        //     cellCount=new Vector3(2,1,1);
        // }

        if(bs.x>=bs.y&&bs.x>=bs.z){
            cellCount=new Vector3(2,1,1);
        }
        else if(bs.y>=bs.x&&bs.y>=bs.z){
            cellCount=new Vector3(1,2,1);
        }
        else if(bs.z>=bs.x&&bs.z>=bs.y){
            cellCount=new Vector3(1,1,2);
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

        if (level>=minLevel){
            // Debug.Log($"[{this.name}]nodeSetting.MaxRenderCount:{nodeSetting.MaxRenderCount}");
            if(nodeSetting.MaxRenderCount > 0){
                if(this.Renderers.Count< nodeSetting.MaxRenderCount)
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
            else{
                IsLeaf = true;
                // Debug.Log($"LeafNode4 level:{level}");
                return null;
            }
        }
        var allCount=cellCount.x*cellCount.y*cellCount.z;
        //DateTime start=DateTime.Now;
        this.ClearChildren();



        // Debug.LogError("bounds:"+bounds);
        // Debug.LogError("cellCount:"+cellCount);
        var min=bounds.min;
        var xSize=bounds.size.x/cellCount.x;
        var ySize=bounds.size.y/cellCount.y;
        var zSize=bounds.size.z/cellCount.z;
        Vector3 size=new Vector3(xSize,ySize,zSize);
        // Debug.LogError("size:"+size);
        List<Bounds> cellBoundsList=new List<Bounds>();
        List<AreaTreeNode> nodes=new List<AreaTreeNode>();
        int id=0;
        for(int i=0;i<cellCount.x;i++){
            for(int j=0;j<cellCount.y;j++){
                for(int k=0;k<cellCount.z;k++){
                    id++;
                    var offset=new Vector3(i*xSize,j*ySize,k*zSize);
                    var center=min+offset+size/2;
                    Bounds cellBounds=new Bounds();
                    cellBounds.center=center;
                    cellBounds.size=size;
                    cellBoundsList.Add(cellBounds);

                    GameObject cube=AreaTreeHelper.CreateBoundsCube(cellBounds,$"Node_{level}_{id}",transform);
                    AreaTreeNode node=cube.AddComponent<AreaTreeNode>();
                    nodes.Add(node);
                    node.Bounds=cellBounds;

                    tree.TreeNodes.Add(node);

                    Nodes.Add(node);
                }
            }
        }

        var renderers=this.Renderers;
         foreach(var render in renderers)
        {
            var pos=render.transform.position;
            foreach(AreaTreeNode node in nodes)
            {
                if(node.Bounds.Contains(pos))
                {
                    node.AddRenderer(render);
                }
            }
        }

        //RendererCount = renderers.Count;
        //renderers.Clear();

        for (int i = 0; i < nodes.Count; i++)
        {
            AreaTreeNode node = nodes[i];
            // if(node.Renderers.Count<maxRenderCount){
            //     continue;
            // }
            
            node.name += "_" + node.RendererCount+"_"+ node.GetVertexCount()+"w";
            node.CreateSubNodes(level+1,i,tree);

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

    [ContextMenu("ClearDictionary")]
    public void ClearDictionary()
    {
        Debug.Log("ClearDictionary Start:"+AreaTreeHelper.render2NodeDict.Count);
        AreaTreeHelper.render2NodeDict.Clear();
        Debug.Log("ClearDictionary End:"+AreaTreeHelper.render2NodeDict.Count);
    }

    [ContextMenu("CreateDictionary")]
    public void CreateDictionary()
    {
        //Debug.Log("CreateDictionary Start:"+AreaTreeHelper.render2NodeDict.Count);
        if(this.Nodes.Count==0)
        {
            //Renders
            foreach(var render in this.Renderers)
            {
                if(AreaTreeHelper.render2NodeDict.ContainsKey(render))
                {
                    var node=AreaTreeHelper.render2NodeDict[render];
                    if(node==this)
                    {

                    }
                    else{
                        AreaTreeHelper.render2NodeDict[render]=this;
                        Debug.LogError($"模型重复在不同的Node里:{AreaTreeHelper.render2NodeDict[render]},{this}");
                    }
                }
                else{
                    AreaTreeHelper.render2NodeDict.Add(render,this);
                }
            }
        }
        //Debug.Log("CreateDictionary 1:"+AreaTreeHelper.render2NodeDict.Count);

        //CopyRenders
        if(newRenderers!=null)
        {
            AreaTreeHelper.AddNodeDictItem_Renderers(newRenderers, this);
        }

        //CombinedRenders
        //Debug.Log("CreateDictionary 2:"+AreaTreeHelper.render2NodeDict.Count);
        if(combindResult!=null)
        {
            var renderers=combindResult.GetComponentsInChildren<MeshRenderer>();
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

    [ContextMenu("ShowNodes")]
    public void ShowNodes()
    {
        if(IsNodeVisible==true)return;
        Debug.Log("ShowNodes:"+this.name);
        IsNodeVisible=true;
        this.gameObject.SetActive(true);
        foreach(AreaTreeNode node in Nodes)
        {
            if(node==null)continue;
            node.ShowNodes();
        }

        foreach (var render in CombinedRenderers)
        {
            render.gameObject.SetActive(true);
        }
    }

    public bool IsNodeVisible=true;

    [ContextMenu("HideNodes")]
    public void HideNodes()
    {
        if(IsNodeVisible==false)return;
        Debug.Log("HideNodes:" + this.name);
        IsNodeVisible =false;
        this.gameObject.SetActive(false);
        foreach(AreaTreeNode node in Nodes)
        {
            if(node==null)continue;
            node.ShowNodes();
        }

        foreach(var render in CombinedRenderers)
        {
            render.gameObject.SetActive(false);
        }
    }

    Mesh meshAsset = null;

    [ContextMenu("SaveMeshes")]
    public void SaveMeshes(string dir)
    {
#if UNITY_EDITOR
        if (meshAsset != null) return;

        MeshFilter[] meshFilters = combindResult.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            MeshFilter meshFilter = meshFilters[i];
            if(UnityEditor.AssetDatabase.Contains(meshFilter.sharedMesh))
            {
                return;
            }
        }

        meshAsset = new Mesh();
        //string assetName = tree.Target.name+"_"+gameObject.name + gameObject.GetInstanceID();
        string assetName = gameObject.name + gameObject.GetInstanceID();

        string meshPath = dir+"/" + assetName + ".asset";
        AutomaticLODHelper.SaveAsset(meshAsset, meshPath, false);
        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            MeshFilter meshFilter = meshFilters[i];
            Debug.LogError("meshFilter.sharedMesh :"+ meshFilter.sharedMesh);
            if (meshFilter.sharedMesh == null)
            {
                Debug.LogError("meshFilter.sharedMesh == null");
                continue;
            }
            Mesh mesh = meshFilter.sharedMesh;
            AutomaticLODHelper.SaveAsset(mesh, meshPath, true );
        }
#endif
    }
}
