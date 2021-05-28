using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AreaTreeNode : MonoBehaviour
{
    public Bounds Bounds;

    public List<MeshRenderer> Renderers=new List<MeshRenderer>();

    public MeshRenderer[] newRenderers;

    public List<AreaTreeNode> Nodes=new List<AreaTreeNode>();

    public void AddRenderer(MeshRenderer renderer){
        Renderers.Add(renderer);
    }

    public GameObject renderersRoot;

    [ContextMenu("CopyRenderers")]
    public void CopyRenderers()
    {
        //newRenderers.Clear();
        renderersRoot=new GameObject(this.name+"_Renderers");
        //renderersRoot.transform.SetParent(this.transform);

        foreach(var render in Renderers){
            //render.enabled=false;
            GameObject copy=MeshHelper.CopyGO(render.gameObject);
            // MeshCollider collider=copy.GetComponent<MeshCollider>();
            // if(collider){
            //     GameObject.DestroyImmediate(collider);
            // }
            copy.SetActive(true);
            copy.transform.SetParent(renderersRoot.transform);

            render.gameObject.SetActive(false);
        }

        newRenderers=renderersRoot.GetComponentsInChildren<MeshRenderer>();


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

    public GameObject combindResult;

    [ContextMenu("CombineMesh")]
    public void CombineMesh()
    {

        if(renderersRoot){
            GameObject.DestroyImmediate(renderersRoot);
        }

        DestroySelfRenderer();

        CopyRenderers();

        CombineInner();

        renderersRoot.transform.SetParent(this.transform);
    }

    private void CombineInner()
    {
        if(combindResult){
            GameObject.DestroyImmediate(combindResult);
        }
        combindResult=MeshCombineHelper.CombineEx(this.renderersRoot,1);
        combindResult.transform.SetParent(this.transform);
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
        newRenderers=renderersRoot.GetComponentsInChildren<MeshRenderer>();
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

    public List<AreaTreeNode> CreateSubNodes(int level,int maxLevel,int index,AreaTree tree,int maxRenderCount){

        //Bounds bounds=ColliderHelper.CaculateBounds(this.Renderers);
        Bounds bounds=this.Bounds;
        var bs=bounds.size;
        
        Debug.LogError("CreateSubNodes size:"+bounds.size);

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
        
        if(level>=maxLevel){
            if(maxRenderCount>0){
                if(this.Renderers.Count<maxRenderCount){
                    return null;
                }
            }
            else{
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

                    GameObject cube=AreaTreeHelper.CreateBoundsCube(cellBounds,$"Node_{id}",transform);
                    AreaTreeNode node=cube.AddComponent<AreaTreeNode>();
                    nodes.Add(node);
                    node.Bounds=cellBounds;

                    tree.TreeNodes.Add(node);

                    Nodes.Add(node);
                }
            }
        }

        var renderers=this.Renderers;
        int count=0;
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

        for (int i = 0; i < nodes.Count; i++)
        {
            AreaTreeNode node = nodes[i];
            // if(node.Renderers.Count<maxRenderCount){
            //     continue;
            // }
            node.CreateSubNodes(level+1,maxLevel,i,tree,maxRenderCount);
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
        if(this.Nodes.Count==0){
            foreach(var render in this.Renderers){
                if(AreaTreeHelper.render2NodeDict.ContainsKey(render))
                {
                    Debug.LogError($"模型重复在不同的Node里:{AreaTreeHelper.render2NodeDict[render]},{this}");
                }
                else{
                    AreaTreeHelper.render2NodeDict.Add(render,this);
                }
            }
            if(newRenderers!=null)
                foreach(var render in this.newRenderers){
                    AreaTreeHelper.render2NodeDict.Add(render,this);
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
    }
}
