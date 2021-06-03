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

    private List<MeshRenderer> newRenderers = new List<MeshRenderer>();

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
            count += meshFilter.mesh.vertexCount;
        }
        VertexCount = count / 10000;
        return VertexCount;
    }

    public List<MeshRenderer> GetRenderers()
    {
        return Renderers;
    }

    public void AddRenderer(MeshRenderer renderer){
        Renderers.Add(renderer);
    }

    public void AddRenderers(List<MeshRenderer> renderers)
    {
        Renderers.AddRange(renderers);
    }

    [ContextMenu("CopyRenderers")]
    public void CopyRenderers(bool isCopy)
    {
        newRenderers.Clear();
        //newRenderers.Clear();
        renderersRoot =new GameObject(this.name+"_Renderers");
        //renderersRoot.transform.SetParent(this.transform);
        foreach(var render in Renderers){
            //render.enabled=false;
            GameObject go=render.gameObject;
            if(isCopy)
            {
                go.SetActive(false);
                go =MeshHelper.CopyGO(go);
                go.SetActive(true);

                var renderNew = go.GetComponent<MeshRenderer>();
                newRenderers.Add(renderNew);
            }
            else
            {
                newRenderers.Add(render);
            }

            //newRenderers.Add

            //go.SetActive(false);
            // MeshCollider collider=copy.GetComponent<MeshCollider>();
            // if(collider){
            //     GameObject.DestroyImmediate(collider);
            // }

            go.transform.SetParent(renderersRoot.transform);
        }

        //if(isCopy)
        //    newRenderers=renderersRoot.GetComponentsInChildren<MeshRenderer>();

        //CreateColliders();
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

    public void HideRenders()
    {
        foreach (var r in newRenderers)
        {
            if(r==null)continue;
            r.enabled = false;
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
    public void CombineMesh(bool isCopy)
    {

        if(renderersRoot){
            GameObject.DestroyImmediate(renderersRoot);
        }

        DestroySelfRenderer();

        CopyRenderers(isCopy);

        CombineInner();

        renderersRoot.transform.SetParent(this.transform);

        HideRenders();
    }

    private void CombineInner()
    {
        if(combindResult){
            GameObject.DestroyImmediate(combindResult);
        }
        combindResult=MeshCombineHelper.CombineEx(this.renderersRoot,1);
        combindResult.name=this.name+"_Combined";
        combindResult.transform.SetParent(this.transform);

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
        CombineInner();
        renderersRoot.transform.SetParent(this.transform);
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
        foreach(var r in newRenderers){
            r.enabled=!isCombined;
        }

        combindResult.SetActive(isCombined);
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


    private AreaTree tree;
    public List<AreaTreeNode> CreateSubNodes(int level,int index,AreaTree tree){

        this.tree = tree;

        int minLevel = tree.nodeSetting.MinLevel;
        int maxLevel = tree.nodeSetting.MaxLevel;
        int maxRenderCount= tree.nodeSetting.MaxRenderCount;

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
            return null;
        }
        
        if(level>=minLevel){
            if(maxRenderCount>0){
                if(this.Renderers.Count<maxRenderCount){
                    IsLeaf = true;
                    return null;
                }
            }
            else{
                IsLeaf = true;
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

    public void CreateDictionary()
    {
        if(this.Nodes.Count==0)
        {
            foreach(var render in this.Renderers)
            {
                if(AreaTreeHelper.render2NodeDict.ContainsKey(render))
                {
                    Debug.LogError($"模型重复在不同的Node里:{AreaTreeHelper.render2NodeDict[render]},{this}");
                }
                else{
                    AreaTreeHelper.render2NodeDict.Add(render,this);
                }
            }
        }

        if(newRenderers!=null)
        {
            foreach(var render in this.newRenderers)
            {
                if(AreaTreeHelper.render2NodeDict.ContainsKey(render))
                {
                    //Debug.LogError($"模型重复在不同的Node里:{AreaTreeHelper.render2NodeDict[render]},{this}");
                }
                else{
                    AreaTreeHelper.render2NodeDict.Add(render,this);
                }
            }
        }

        if(combindResult!=null)
        {
            var renderers=combindResult.GetComponentsInChildren<MeshRenderer>();
            foreach (var render in renderers)
            {
                //renderer.gameObject.AddComponent<MeshCollider>();
                if(AreaTreeHelper.render2NodeDict.ContainsKey(render))
                {
                    //Debug.LogError($"模型重复在不同的Node里:{AreaTreeHelper.render2NodeDict[render]},{this}");
                }
                else{
                    AreaTreeHelper.render2NodeDict.Add(render,this);
                }
            }
        }

    }

    [ContextMenu("ShowNodes")]
    public void ShowNodes()
    {
        this.gameObject.SetActive(true);
        foreach(AreaTreeNode node in Nodes)
        {
            if(node==null)continue;
            node.ShowNodes();
        }

        foreach (var render in Renderers)
        {
            render.gameObject.SetActive(true);
        }
    }

    [ContextMenu("HideNodes")]
    public void HideNodes()
    {
        this.gameObject.SetActive(false);
        foreach(AreaTreeNode node in Nodes)
        {
            if(node==null)continue;
            node.ShowNodes();
        }

        foreach(var render in Renderers)
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
