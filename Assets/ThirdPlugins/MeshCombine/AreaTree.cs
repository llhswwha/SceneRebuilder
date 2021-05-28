using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AreaTree : MonoBehaviour
{
    public enum AreaTreeMode
    {
        Size,Count
    }

    public AreaTreeMode Mode=AreaTreeMode.Size;

    public Vector3 Size=new Vector3(10,10,10);

    public Vector3 Count=new Vector3(10,10,10);

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
        Bounds bounds=ColliderHelper.CaculateBounds(Target);
        Debug.LogError("bounds:"+bounds);

        AreaTreeHelper.CreateBoundsCube(bounds,"TargetBound",transform);
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
    

    [ContextMenu("CreateCells_Count")]
    public void CreateCells_Count()
    {
        AreaTreeHelper.CubePrefab=this.CubePrefab;
        var allCount=Count.x*Count.y*Count.z;
        DateTime start=DateTime.Now;
        ClearChildren();

        Bounds bounds=ColliderHelper.CaculateBounds(Target);
        Debug.LogError("bounds:"+bounds);
         Debug.LogError("Count:"+Count);
        var min=bounds.min;
        var xSize=bounds.size.x/Count.x;
        var ySize=bounds.size.y/Count.y;
        var zSize=bounds.size.z/Count.z;
        Vector3 size=new Vector3(xSize,ySize,zSize);
        Debug.LogError("size:"+size);
        List<Bounds> cellBoundsList=new List<Bounds>();
        TreeNodes=new List<AreaTreeNode>();
        for(int i=0;i<Count.x;i++){
            for(int j=0;j<Count.y;j++){
                for(int k=0;k<Count.z;k++){
                    var offset=new Vector3(i*xSize,j*ySize,k*zSize);
                    var center=min+offset+size/2;
                    Bounds cellBounds=new Bounds();
                    cellBounds.center=center;
                    cellBounds.size=size;
                    cellBoundsList.Add(cellBounds);

                    GameObject cube=AreaTreeHelper.CreateBoundsCube(cellBounds,$"cell[{i},{j},{k}]",transform);
                    AreaTreeNode node=cube.AddComponent<AreaTreeNode>();
                    TreeNodes.Add(node);
                    node.Bounds=cellBounds;
                }
            }
        }

        var renderers=Target.GetComponentsInChildren<MeshRenderer>();
        int count=0;
        foreach(var render in renderers)
        {
            var pos=render.transform.position;
            foreach(AreaTreeNode node in TreeNodes)
            {
                if(node.Bounds.Contains(pos))
                {
                    node.AddRenderer(render);
                }
            }
        }
        int cellCount=0;
        foreach(AreaTreeNode node in TreeNodes)
        {
            if(node.Renderers.Count==0){
                GameObject.DestroyImmediate(node.gameObject);
            }
            else{
                cellCount++;
            }
        }

        Debug.LogError($"CreateCells cellCount:{cellCount}/{allCount},\tavg:{renderers.Length/cellCount},\t{(DateTime.Now-start).ToString()}");
        //bound.Contains()
    }



    public GameObject CubePrefab=null;

    [ContextMenu("CreateCells_Size")]
    public void CreateCells_Size()
    {
        AreaTreeHelper.CubePrefab=this.CubePrefab;
        DateTime start=DateTime.Now;
        ClearChildren();

        var renderers=Target.GetComponentsInChildren<MeshRenderer>();
        Debug.LogError("renderers:"+renderers.Length);
        foreach(var render in renderers){
            render.enabled=true;
        }

        Bounds bounds=ColliderHelper.CaculateBounds(Target);
        Debug.LogError("bounds:"+bounds);
         Debug.LogError("Count:"+Count);

        var size1=Size;
        Debug.LogError("size:"+size1);
        int xCount=(int)Math.Ceiling(bounds.size.x/size1.x);
        int yCount=(int)Math.Ceiling(bounds.size.y/size1.y);
        int zCount=(int)Math.Ceiling(bounds.size.z/size1.z);


        Count=new Vector3(xCount,yCount,zCount);
        var allCount=Count.x*Count.y*Count.z;
         Debug.LogError("Count2:"+Count);

        var min=bounds.min;
        var xSize=bounds.size.x/Count.x;
        var ySize=bounds.size.y/Count.y;
        var zSize=bounds.size.z/Count.z;
        Vector3 size=new Vector3(xSize,ySize,zSize);
        Debug.LogError("size:"+size);
        List<Bounds> cellBoundsList=new List<Bounds>();
        TreeNodes=new List<AreaTreeNode>();
        for(int i=0;i<Count.x;i++){
            for(int j=0;j<Count.y;j++){
                for(int k=0;k<Count.z;k++){
                    var offset=new Vector3(i*xSize,j*ySize,k*zSize);
                    var center=min+offset+size/2;
                    Bounds cellBounds=new Bounds();
                    cellBounds.center=center;
                    cellBounds.size=size;
                    cellBoundsList.Add(cellBounds);

                    GameObject cube=AreaTreeHelper.CreateBoundsCube(cellBounds,$"cell[{i},{j},{k}]",transform);
                    AreaTreeNode node=cube.AddComponent<AreaTreeNode>();
                    TreeNodes.Add(node);
                    node.Bounds=cellBounds;
                }
            }
        }

        
        int count=0;
        foreach(var render in renderers)
        {
            var pos=render.transform.position;
            foreach(AreaTreeNode node in TreeNodes)
            {
                if(node.Bounds.Contains(pos))
                {
                    node.AddRenderer(render);
                }
            }
        }
        int cellCount=0;
        foreach(AreaTreeNode node in TreeNodes)
        {
            if(node.Renderers.Count==0){
                GameObject.DestroyImmediate(node.gameObject);
            }
            else{
                cellCount++;
                node.name+="_"+node.Renderers.Count;
            }
        }

        Debug.LogError($"CreateCells cellCount:{cellCount}/{allCount},\tavg:{renderers.Length/cellCount},\t{(DateTime.Now-start).ToString()}");
        //bound.Contains()
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
            renderCount+=node.Renderers.Count;
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
            renderCount+=node.Renderers.Count;
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
        AreaTreeHelper.CubePrefab=this.CubePrefab;
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

        GameObject cube=AreaTreeHelper.CreateBoundsCube(bounds,$"RootNode",null);
        AreaTreeNode node=cube.AddComponent<AreaTreeNode>();
        if(RootNode!=null){
            GameObject.DestroyImmediate(RootNode);
        }
        this.RootNode=node;
        this.TreeNodes.Add(node);

        node.Bounds=bounds;
        node.Renderers=renderers.ToList();
        
        node.CreateSubNodes(0,MaxLevel,0,this);

        var allCount=this.TreeNodes.Count;
        
        int cellCount=ClearNodes();
        AvgCellRendererCount=(int)(renderers.Length/cellCount);
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

    [ContextMenu("OneKey")]
    public void OneKey()
    {
         DateTime start=DateTime.Now;

        ShowRenderers();
        AddColliders();
        CreateCells_Tree();
        CombineMesh();
        CreateDictionary();
        Debug.LogError($"OneKey {(DateTime.Now-start).ToString()}");
    }

    void Start()
    {
        // if(AreaTreeHelper.render2NodeDict.Count==0)
        // {
        //     CreateDictionary();
        // }
        
    }

    public Vector3 LeafCellSize;

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

            int renderCount=tn.Renderers.Count;
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
