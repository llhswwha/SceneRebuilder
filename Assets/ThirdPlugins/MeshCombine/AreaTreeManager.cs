using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AreaTreeManager : MonoBehaviour
{
    public enum AreaTreeMode
    {
        Size,Count
    }

    public AreaTreeMode Mode=AreaTreeMode.Size;

    public Vector3 Size=new Vector3(10,10,10);

    public Vector3 Count=new Vector3(10,10,10);

    public List<AreaTreeNode> TreeNodes=new List<AreaTreeNode>();

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

   public GameObject Target=null;
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

   public List<AreaTree> Trees=new List<AreaTree>();

   [ContextMenu("CreateOne")]
   public void CreateOne()
   {
       DateTime start=DateTime.Now;

       ClearTrees();
       CreateTree(Target);
       Debug.LogError($"CreateOne \t{(DateTime.Now-start).ToString()}");
   }

   public void CreateTree(GameObject go)
   {
       AreaTreeHelper.CubePrefab=this.CubePrefab;

       GameObject treeGo=new GameObject(go.name+"_Tree");
       AreaTree areaTree=treeGo.AddComponent<AreaTree>();
       areaTree.MaxLevel=MaxLevel;
       areaTree.MaxRenderCount=MaxRenderCount;
       areaTree.Target=go;
       if(isCombine){
           areaTree.GenerateMesh();
       }
       else{
           areaTree.GenerateTree();
       }
       
       Trees.Add(areaTree);
   }

    public int MaxLevel=3;//3,6,9

    public int MaxRenderCount=50;

    public bool isCombine=false;

   public void ClearTrees()
   {
       foreach(var tree in Trees)
       {
           if(tree==null)continue;
           GameObject.DestroyImmediate(tree.gameObject);
       }
       Trees.Clear();
   }

  [ContextMenu("CreateChildren")]
   public void CreateChildren()
   {
        DateTime start=DateTime.Now;
       ClearTrees();
      
       for(int i=0;i<Target.transform.childCount;i++)
       {
           var child=Target.transform.GetChild(i);
           CreateTree(child.gameObject);
       }
       Debug.LogError($"CreateOne \t{(DateTime.Now-start).ToString()}");
   }
}
