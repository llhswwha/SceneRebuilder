using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AreaTreeManager : MonoBehaviour
{
    public enum AreaTreeMode
    {
        Size, Count
    }

    public AreaTreeMode Mode = AreaTreeMode.Size;

    public Vector3 Size = new Vector3(10, 10, 10);

    public Vector3 Count = new Vector3(10, 10, 10);

    public List<AreaTreeNode> TreeNodes = new List<AreaTreeNode>();

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
    }

    public GameObject Target = null;

    public PrefabInstanceBuilder prefabInstanceBuilder;

    [ContextMenu("CreateCells_Count")]
    public void CreateCells_Count()
    {

        AreaTreeHelper.CubePrefab = this.CubePrefab;
        var allCount = Count.x * Count.y * Count.z;
        DateTime start = DateTime.Now;
        ClearChildren();

        Bounds bounds = ColliderHelper.CaculateBounds(Target);
        Debug.LogError("bounds:" + bounds);
        Debug.LogError("Count:" + Count);
        var min = bounds.min;
        var xSize = bounds.size.x / Count.x;
        var ySize = bounds.size.y / Count.y;
        var zSize = bounds.size.z / Count.z;
        Vector3 size = new Vector3(xSize, ySize, zSize);
        Debug.LogError("size:" + size);
        List<Bounds> cellBoundsList = new List<Bounds>();
        TreeNodes = new List<AreaTreeNode>();
        for (int i = 0; i < Count.x; i++)
        {
            for (int j = 0; j < Count.y; j++)
            {
                for (int k = 0; k < Count.z; k++)
                {
                    var offset = new Vector3(i * xSize, j * ySize, k * zSize);
                    var center = min + offset + size / 2;
                    Bounds cellBounds = new Bounds();
                    cellBounds.center = center;
                    cellBounds.size = size;
                    cellBoundsList.Add(cellBounds);

                    GameObject cube = AreaTreeHelper.CreateBoundsCube(cellBounds, $"cell[{i},{j},{k}]", transform);
                    AreaTreeNode node = cube.AddComponent<AreaTreeNode>();
                    TreeNodes.Add(node);
                    node.Bounds = cellBounds;
                }
            }
        }

        var renderers = Target.GetComponentsInChildren<MeshRenderer>();
        int count = 0;
        foreach (var render in renderers)
        {
            var pos = render.transform.position;
            foreach (AreaTreeNode node in TreeNodes)
            {
                if (node.Bounds.Contains(pos))
                {
                    node.AddRenderer(render);
                }
            }
        }
        int cellCount = 0;
        foreach (AreaTreeNode node in TreeNodes)
        {
            if (node.RendererCount == 0)
            {
                GameObject.DestroyImmediate(node.gameObject);
            }
            else
            {
                cellCount++;
            }
        }

        Debug.LogError($"CreateCells cellCount:{cellCount}/{allCount},\tavg:{renderers.Length / cellCount},\t{(DateTime.Now - start).ToString()}");
        //bound.Contains()
    }



    public GameObject CubePrefab = null;

    [ContextMenu("CreateCells_Size")]
    public void CreateCells_Size()
    {
        AreaTreeHelper.CubePrefab = this.CubePrefab;
        DateTime start = DateTime.Now;
        ClearChildren();

        var renderers = Target.GetComponentsInChildren<MeshRenderer>();
        Debug.LogError("renderers:" + renderers.Length);
        foreach (var render in renderers)
        {
            render.enabled = true;
        }

        Bounds bounds = ColliderHelper.CaculateBounds(Target);
        Debug.LogError("bounds:" + bounds);
        Debug.LogError("Count:" + Count);

        var size1 = Size;
        Debug.LogError("size:" + size1);
        int xCount = (int)Math.Ceiling(bounds.size.x / size1.x);
        int yCount = (int)Math.Ceiling(bounds.size.y / size1.y);
        int zCount = (int)Math.Ceiling(bounds.size.z / size1.z);


        Count = new Vector3(xCount, yCount, zCount);
        var allCount = Count.x * Count.y * Count.z;
        Debug.LogError("Count2:" + Count);

        var min = bounds.min;
        var xSize = bounds.size.x / Count.x;
        var ySize = bounds.size.y / Count.y;
        var zSize = bounds.size.z / Count.z;
        Vector3 size = new Vector3(xSize, ySize, zSize);
        Debug.LogError("size:" + size);
        List<Bounds> cellBoundsList = new List<Bounds>();
        TreeNodes = new List<AreaTreeNode>();
        for (int i = 0; i < Count.x; i++)
        {
            for (int j = 0; j < Count.y; j++)
            {
                for (int k = 0; k < Count.z; k++)
                {
                    var offset = new Vector3(i * xSize, j * ySize, k * zSize);
                    var center = min + offset + size / 2;
                    Bounds cellBounds = new Bounds();
                    cellBounds.center = center;
                    cellBounds.size = size;
                    cellBoundsList.Add(cellBounds);

                    GameObject cube = AreaTreeHelper.CreateBoundsCube(cellBounds, $"cell[{i},{j},{k}]", transform);
                    AreaTreeNode node = cube.AddComponent<AreaTreeNode>();
                    TreeNodes.Add(node);
                    node.Bounds = cellBounds;
                }
            }
        }


        int count = 0;
        foreach (var render in renderers)
        {
            var pos = render.transform.position;
            foreach (AreaTreeNode node in TreeNodes)
            {
                if (node.Bounds.Contains(pos))
                {
                    node.AddRenderer(render);
                }
            }
        }
        int cellCount = 0;
        foreach (AreaTreeNode node in TreeNodes)
        {
            if (node.RendererCount == 0)
            {
                GameObject.DestroyImmediate(node.gameObject);
            }
            else
            {
                cellCount++;
                node.name += "_" + node.RendererCount;
            }
        }

        Debug.LogError($"CreateCells cellCount:{cellCount}/{allCount},\tavg:{renderers.Length / cellCount},\t{(DateTime.Now - start).ToString()}");
        //bound.Contains()
    }

    public List<ModelAreaTree> Trees = new List<ModelAreaTree>();

    List<Material> matList = new List<Material>();
    public ModelAreaTree CreateTree(GameObject go, bool isC,string suffix, MeshRenderer[] renderers)
    {
        string treeName = "NewAreaTree" + suffix;
        if (go != null)
        {
            treeName = go.name + suffix;
        }
        AreaTreeHelper.CubePrefab = this.CubePrefab;

        GameObject treeGo = new GameObject(treeName);
        ModelAreaTree areaTree = treeGo.AddComponent<ModelAreaTree>();
        areaTree.nodeSetting = this.nodeSetting;
        areaTree.IsCopy = this.IsCopy;

        areaTree.Target = go;
        areaTree.TreeRenderers = renderers;

        if (isC)
        {
            if(combinerSetting)
            {
                combinerSetting.SetSetting();
            }
            areaTree.GenerateMesh();
        }
        else
        {
            areaTree.GenerateTree();
        }
        Trees.Add(areaTree);

        ShowModelInfo(areaTree);
        return areaTree;
    }

    private void ShowModelInfo(ModelAreaTree areaTree)
    {
        LeafCount += areaTree.TreeLeafs.Count;
        //nodeStatics.AvgCellRendererCount += areaTree.nodeStatics.AvgCellRendererCount;
        //if (areaTree.nodeStatics.MaxCellRendererCount > nodeStatics.MaxCellRendererCount)
        //    nodeStatics.MaxCellRendererCount = areaTree.nodeStatics.MaxCellRendererCount;
        //if (areaTree.nodeStatics.LevelDepth > nodeStatics.Depth)
        //    nodeStatics.Depth = areaTree.nodeStatics.LevelDepth;
        nodeStatics.SetInfo(areaTree.nodeStatics);

        //RendererCount += areaTree.RootNode.RendererCount;

        var renders = areaTree.GetTreeRendererers();
        int vertexCount = 0;
        if (renders != null)
        {
            RendererCount += renders.Length;
            foreach (var render in renders)
            {
                if (!matList.Contains(render.sharedMaterial))
                {
                    matList.Add(render.sharedMaterial);
                }
                MeshFilter meshFilter = render.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    vertexCount += meshFilter.sharedMesh.vertexCount;
                }
            }
        }

        VertexCount += vertexCount/10000;
        MatCount = matList.Count;

        CombinedCount += areaTree.CombinedCount;
        nodeStatics.CellCount += areaTree.nodeStatics.CellCount;
    }

    public void ClearCount()
    {
        matList.Clear();
        LeafCount = 0;
        //AvgCount = 0;
        //MaxCount = 0;
        //Depth = 0;
        nodeStatics.Clear();
        VertexCount = 0;
        RendererCount = 0;
        MatCount = 0;
        CombinedCount = 0;
        //nodeStatics.CellCount = 0;
    }


    //public int MinLevel = 3;//3,6,9

    //public int MaxLevel = 15;//3,6,9

    //public int MaxRenderCount = 50;

    public AreaTreeNodeSetting nodeSetting;

    public AreaTreeNodeStatics nodeStatics;

    public bool isCombine = false;

    public MeshCombinerSetting combinerSetting;

    public bool IsCopy = true;

    public int VertexCount = 0;

    public int RendererCount = 0;

    public int CombinedCount = 0;

    public int MatCount = 0;

    public int LeafCount;

    //public float MaxModelLength=0.6f;

    //public int CellCount = 0;

    //public int AvgCount;

    //public int MaxCount;

    //public int Depth = 0;

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        AreaTreeHelper.render2NodeDict.Clear();
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            GameObject.DestroyImmediate(tree.gameObject);
        }
        Trees.Clear();
    }

     [ContextMenu("CreateHiddenOne")]
    public void CreateHiddenOne()
    {
        DateTime start = DateTime.Now;
        ClearCount();
        ClearTrees();

        // MeshRenderer[] combinedRenderers=null;
        // if (prefabInstanceBuilder != null)
        // {
        //     combinedRenderers = prefabInstanceBuilder.GetCombinedRenderers().ToArray();
        // }
        // CreateTree(Target,isCombine,"_CombineTree",combinedRenderers);//合并模型的树

        MeshRenderer[] hiddenRenderers=null;
        if (prefabInstanceBuilder != null)
        {
            hiddenRenderers = prefabInstanceBuilder.GetHiddenRenderers().ToArray();

            Target=prefabInstanceBuilder.GetTarget();
        }

        CreateTree(Target,false,"_HiddenTree",hiddenRenderers);//动态显示模型的树
        
        Debug.LogError($"CreateHiddenOne \t{(DateTime.Now - start).ToString()}");
    }

    //public AreaTreeNodeShowManager TreeNodeShowManager;

    [ContextMenu("CreateOne_BigSmall")]
    public void CreateOne_BigSmall()
    {
        IsCopy=false;

        DateTime start = DateTime.Now;
        ClearCount();
        ClearTrees();

        List<MeshRenderer> bigModels=new List<MeshRenderer>();
        List<MeshRenderer> smallModels=new List<MeshRenderer>();
        prefabInstanceBuilder.GetBigSmallRenderers(bigModels,smallModels);


        var tree2=CreateTree(Target,isCombine,"_SamllTree",smallModels.ToArray());//动态显示模型的树
        tree2.IsHidden=true;
        tree2.HideRenderers();

        var tree1=CreateTree(Target,isCombine,"_BigTree",bigModels.ToArray());//合并模型的树

        //TreeNodeShowManager.HiddenTrees.Add(tree2);
        tree2.DestroyNodeRender();
        Debug.LogError($"CreateOne_BigSmall \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("CreateOne_HiddenShown")]
    public void CreateOne_HiddenShown()
    {
        IsCopy=false;

        DateTime start = DateTime.Now;
        ClearCount();
        ClearTrees();

        MeshRenderer[] hiddenRenderers=null;
        if (prefabInstanceBuilder != null)
        {
            hiddenRenderers = prefabInstanceBuilder.GetHiddenRenderers().ToArray();
        }
        var tree2=CreateTree(Target,isCombine,"_HiddenTree",hiddenRenderers);//动态显示模型的树
        tree2.IsHidden=true;
        tree2.HideRenderers();

        MeshRenderer[] combinedRenderers=null;
        if (prefabInstanceBuilder != null)
        {
            combinedRenderers = prefabInstanceBuilder.GetCombinedRenderers().ToArray();
        }
        var tree1=CreateTree(Target,isCombine,"_ShownTree",combinedRenderers);//合并模型的树

        //TreeNodeShowManager.HiddenTrees.Add(tree2);
        tree2.DestroyNodeRender();
        Debug.LogError($"CreateCombinedOne \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("CreateOne")]
    public void CreateOne()
    {
        IsCopy=false;

        DateTime start = DateTime.Now;
        ClearCount();
        ClearTrees();

        MeshRenderer[] combinedRenderers=null;
        if (prefabInstanceBuilder != null)
        {
            combinedRenderers = prefabInstanceBuilder.GetCombinedRenderers().ToArray();
        }
        var tree1=CreateTree(Target,isCombine,"_Tree",combinedRenderers);//合并模型的树

        Debug.LogError($"CreateOne \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("CreateCombinedChildren")]
    public void CreateCombinedChildren()
    {
        DateTime start = DateTime.Now;
        ClearTrees();

        for (int i = 0; i < Target.transform.childCount; i++)
        {
            var child = Target.transform.GetChild(i);
            CreateTree(child.gameObject,isCombine,"_CombineTree",null);
        }

        //AvgCount /= Target.transform.childCount;
        nodeStatics.AvgCellRendererCount /= Target.transform.childCount;

        Debug.LogError($"CreateOne \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("ToReanderers")]
    public void ToReanderers()
    {
        foreach (var tree in Trees)
        {
            tree.SwitchToRenderers();
        }
    }

    [ContextMenu("ToCombined")]
    public void ToCombined()
    {
        foreach (var tree in Trees)
        {
            tree.SwitchToCombined();
        }
    }

    [ContextMenu("CreateDictionary")]
    public void CreateDictionary()
    {
        Debug.Log("AreaTreeManager.CreateDictionary:" + Trees.Count);
        foreach (var tree in Trees)
        {
            Debug.Log("tree:" + tree);
            tree.CreateDictionary();
        }
    }

    [ContextMenu("HideLeafNodes")]
    public void HideLeafNodes()
    {
        foreach (var tree in Trees)
        {
            tree.HideLeafNodes();
        }
    }

    [ContextMenu("ShowLeafNodes")]
    public void ShowLeafNodes()
    {
        foreach (var tree in Trees)
        {
            tree.ShowLeafNodes();
        }
    }

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        foreach (var tree in Trees)
        {
            tree.ShowRenderers();
        }
    }

    [ContextMenu("HideRenderers")]
    public void HideRenderers()
    {
        foreach (var tree in Trees)
        {
            tree.HideRenderers();
        }
    }

    [ContextMenu("GetMaterials")]
    public void GetMaterials()
    {
        DateTime start = DateTime.Now;

        ShowRenderers();

        List<string> matKeys = new List<string>();
        List<Material> mats = new List<Material>();
        var renders = Target.GetComponentsInChildren<MeshRenderer>();
        foreach (var render in renders)
        {
            if (!mats.Contains(render.sharedMaterial))
            {
                mats.Add(render.sharedMaterial);
            }
        }
        int count = 0;
        var matsEx = MeshCombineHelper.GetMatFilters(Target, out count, false);
        Debug.LogError($"GetMaterials {(DateTime.Now - start).ToString()},mats1:{mats.Count},mats2:{matsEx.Count},count:{count}");
    }


    [ContextMenu("SetMaterials")]
    public void SetMaterials()
    {
        DateTime start = DateTime.Now;

        ShowRenderers();

        int count = 0;
        var mats = MeshCombineHelper.GetMatFilters(Target, out count, true);

        //MeshCombineHelper.SetMaterials(Target);

        Debug.LogError($"SetMaterials {(DateTime.Now - start).ToString()},mats:{mats.Count},count:{count}");
    }
}
