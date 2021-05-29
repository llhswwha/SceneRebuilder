using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AreaTree : MonoBehaviour
{
   public GameObject Target=null;
    
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
        DateTime start = DateTime.Now;
        Renderer[] renders = Target.GetComponentsInChildren<Renderer>();
        Bounds bounds=ColliderHelper.CaculateBounds(renders);
        AreaTreeHelper.CreateBoundsCube(bounds, Target.name+"_TargetBound",transform);

        Debug.LogError($"target:{Target.name},renders:{renders.Length},bounds:{bounds}");
        Debug.LogError($"CreateBoundes \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("CreateSubBoundes")]
    public void CreateSubBoundes()
    {
        ClearChildren();

        DateTime start = DateTime.Now;
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < Target.transform.childCount; i++)
        {
            children.Add(Target.transform.GetChild(i));
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

            Debug.LogError($"[{i}] target:{child.name},renders:{renders.Length},bounds:{bounds},boundsAll:{boundsAll}");
        }
        Debug.LogError($"CreateSubBoundes \t{(DateTime.Now - start).ToString()}");

        AreaTreeHelper.CreateBoundsCube(boundsAll, Target.name + "_TargetBoundAll", transform);
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

        Debug.LogError($"CheckRenderers renderCount:{renderCount},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("CombineMesh")]
    public void CombineMesh()
    {   
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
        var renderersNew=this.RootNode.GetComponentsInChildren<MeshRenderer>();
        foreach(var render in renderersNew){
            if(render.enabled==true)
            {
                newRenderCount++;
            }
        }

        Debug.LogError($"CombineMesh renderCount:{renderCount}->{newRenderCount},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("SwitchToCombined")]
    public void SwitchToCombined()
    {
        DateTime start=DateTime.Now;

        foreach(var node in TreeNodes)
        {
            if(node==null)continue;
            node.SwitchToCombined();
        }

        Debug.LogError($"SwitchToCombined \t{(DateTime.Now-start).ToString()}");
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

        Debug.LogError($"SwitchToRenderers \t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("CreateCells_Tree")]
    public void CreateCells_Tree()
    {
        

        //var allCount=Count.x*Count.y*Count.z;
        DateTime start=DateTime.Now;
        ClearChildren();


        var renderers=Target.GetComponentsInChildren<MeshRenderer>();
        Debug.LogError("renderers:"+renderers.Length);
        foreach(var render in renderers){
            render.enabled=true;
        }

        Bounds bounds=ColliderHelper.CaculateBounds(Target);
        Debug.LogError("size:"+bounds.size);
        Debug.LogError("size2:"+bounds.size/2);

        this.TreeNodes.Clear();

        LevelDepth = 0;
        GameObject rootCube=AreaTreeHelper.CreateBoundsCube(bounds,$"RootNode",null);
        AreaTreeNode node=rootCube.AddComponent<AreaTreeNode>();
        if(RootNode!=null){
            GameObject.DestroyImmediate(RootNode);
        }
        this.RootNode=node;
        this.TreeNodes.Add(node);

        node.Bounds=bounds;
        node.Renderers=renderers.ToList();
        
        node.CreateSubNodes(0,MinLevel,MaxLevel,0,this,MaxRenderCount);

        var allCount=this.TreeNodes.Count;
        
        int cellCount=ClearNodes();
        AvgCellRendererCount=(int)(renderers.Length/cellCount);

        rootCube.transform.SetParent(this.transform);

        Debug.LogError($"CreateCells_Tree cellCount:{cellCount}/{allCount},\tavg:{AvgCellRendererCount},\t{(DateTime.Now-start).ToString()}");
    }

    public int AvgCellRendererCount=0;

    public int MaxCellRendererCount=0;

    public int MinCellRendererCount=0;



    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        DateTime start=DateTime.Now;

        var ts=AreaTreeHelper.GetAllTransforms(Target.transform);
        foreach(var t in ts){
            t.gameObject.SetActive(true);
        }

        var renderers=GameObject.FindObjectsOfType<MeshRenderer>();
        //var renderers=Target.GetComponentsInChildren<MeshRenderer>();
        foreach(var render in renderers){
            render.enabled=true;
            render.gameObject.SetActive(true);
        }
        Debug.LogError($"ShowRenderers renderers:{renderers.Length},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("AddColliders")]
    public void AddColliders()
    {
        DateTime start=DateTime.Now;
        var renderers=Target.GetComponentsInChildren<MeshRenderer>();
        foreach(var render in renderers){
            MeshCollider collider=render.gameObject.GetComponent<MeshCollider>();
            if(collider==null){
                collider=render.gameObject.AddComponent<MeshCollider>();
            }
        }
        Debug.LogError($"AddColliders renderers:{renderers.Length},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("CreateDictionary")]
    public void CreateDictionary()
    {
        DateTime start=DateTime.Now;
        AreaTreeHelper.render2NodeDict.Clear();
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
        Debug.LogError($"CreateDictionary render2NodeDict:{AreaTreeHelper.render2NodeDict.Count},\t{(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("GenerateMesh")]
    public void GenerateMesh()
    {
         DateTime start=DateTime.Now;

        ShowRenderers();
        AddColliders();
        CreateCells_Tree();
        CombineMesh();
        CreateDictionary();
        Debug.LogError($"GenerateMesh {(DateTime.Now-start).ToString()}");
    }

    [ContextMenu("GenerateTree")]
    public void GenerateTree()
    {
         DateTime start=DateTime.Now;

        ShowRenderers();
        AddColliders();
        CreateCells_Tree();
        // CombineMesh();
        // CreateDictionary();
        Debug.LogError($"GenerateTree {(DateTime.Now-start).ToString()}");
    }

    void Start()
    {
        // if(AreaTreeHelper.render2NodeDict.Count==0)
        // {
        //     CreateDictionary();
        // }
        
    }

    public Vector3 LeafCellSize;

    public int LevelDepth = 0;

    private int ClearNodes()
    {
        TreeLeafs.Clear();
        int cellCount=0;
        LeafCellSize=Vector3.zero;
        MaxCellRendererCount=int.MinValue;
        MinCellRendererCount=int.MaxValue;
        foreach(AreaTreeNode tn in TreeNodes)
        {
            if(tn==null)continue;
            if(tn.gameObject==null)continue;

            int renderCount = tn.RendererCount;
            if(renderCount==0){
                GameObject.DestroyImmediate(tn.gameObject);
            }
            else{
                cellCount++;
                tn.name+="_"+renderCount;
            }

            if(tn==null)continue;
            if(tn.gameObject==null)continue;
            if(tn.Nodes.Count!=0){
                tn.DestroySelfRenderer();
            }
            else{
                if(renderCount>MaxCellRendererCount){
                    MaxCellRendererCount=renderCount;
                }
                if(renderCount<MinCellRendererCount){
                    MinCellRendererCount=renderCount;
                }
                TreeLeafs.Add(tn);
                if(LeafCellSize==Vector3.zero)
                    LeafCellSize=tn.Bounds.size;
            }
        }
        return cellCount;
    }

    public int MaxLevel=2;

    public int MinLevel = 2;

    public int MaxRenderCount=50;

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
}

public static class AreaTreeHelper
{
    
    public static Dictionary<MeshRenderer,AreaTreeNode> render2NodeDict=new Dictionary<MeshRenderer, AreaTreeNode>();

    public static GameObject CubePrefab;
    public static GameObject CreateBoundsCube(Bounds bounds,string n,Transform parent)
    {
        if(CubePrefab==null){
            CubePrefab=GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        GameObject cube=GameObject.Instantiate(CubePrefab);
        cube.name=n;
        cube.transform.position=bounds.center;
        cube.transform.localScale=bounds.size;
        cube.transform.SetParent(parent);
        return cube;
    }

    public static List<Transform> GetAllTransforms(this Transform transform)
    {
        List<Transform> result = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            result.Add(child);
            result.AddRange(GetAllTransforms(child));
        }
        return result;
    }
}
