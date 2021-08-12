using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaTreeManager : SingletonBehaviour<AreaTreeManager>
{
    void Start()
    {
        CreateDictionary();
    }
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

    [ContextMenu("GetTarget")]
    public GameObject GetTarget()
    {
        if(Target==null){
            var modelRoot=GameObject.FindObjectOfType<ModelRoot>();
            if(modelRoot){
                Target=modelRoot.gameObject;
            }
        }


        if(prefabInstanceBuilder && prefabInstanceBuilder.TargetRoots==null){
            prefabInstanceBuilder.TargetRoots=Target;
        }
        return Target;
    }

    public PrefabInstanceBuilder prefabInstanceBuilder;

    [ContextMenu("CreateCells_Count")]
    public void CreateCells_Count()
    {

        AreaTreeHelper.CubePrefabs = this.CubePrefabs;
        var allCount = Count.x * Count.y * Count.z;
        DateTime start = DateTime.Now;
        ClearChildren();
        var target=GetTarget();
        Bounds bounds = ColliderHelper.CaculateBounds(target);
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

                    GameObject cube = AreaTreeHelper.CreateBoundsCube(cellBounds, $"cell[{i},{j},{k}]", transform,0);
                    AreaTreeNode node = cube.AddComponent<AreaTreeNode>();
                    TreeNodes.Add(node);
                    node.Bounds = cellBounds;
                }
            }
        }

        var renderers = target.GetComponentsInChildren<MeshRenderer>();
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



    public List<GameObject> CubePrefabs = new List<GameObject>();

    [ContextMenu("CreateCells_Size")]
    public void CreateCells_Size()
    {
        AreaTreeHelper.CubePrefabs = this.CubePrefabs;
        DateTime start = DateTime.Now;
        ClearChildren();
        var target=GetTarget();
        var renderers = target.GetComponentsInChildren<MeshRenderer>();
        Debug.LogError("renderers:" + renderers.Length);
        foreach (var render in renderers)
        {
            render.enabled = true;
        }

        Bounds bounds = ColliderHelper.CaculateBounds(target);
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

                    GameObject cube = AreaTreeHelper.CreateBoundsCube(cellBounds, $"cell[{i},{j},{k}]", transform,0);
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
    public ModelAreaTree CreateTree(GameObject go, bool isC,string suffix, MeshRenderer[] renderers, Action<float> progressChanged)
    {
        
        string treeName = "NewAreaTree" + suffix;
        if (go != null)
        {
            treeName = go.name + suffix;
        }


        if (renderers.Length == 0)
        {
            Debug.LogError($"CreateTree name:{treeName},renderers.Length == 0 ");
            return null;
        }

        if(renderers[0] == null)
        {
            Debug.LogError($"CreateTree name:{treeName},renderers:{renderers.Length} r1 is null:{renderers[0] == null}");
        }
        

        AreaTreeHelper.CubePrefabs = this.CubePrefabs;

        GameObject treeGo = new GameObject(treeName);
        ModelAreaTree areaTree = treeGo.AddComponent<ModelAreaTree>();
        areaTree.nodeSetting = this.nodeSetting;
        areaTree.IsCopy = this.IsCopy;

        areaTree.Target = go;
        areaTree.TreeRenderers = renderers;
        areaTree.ShowRenderers();

        if (isC)
        {
            if (combinerSetting)
            {
                combinerSetting.SetSetting();
            }
            areaTree.GenerateMesh(progressChanged);
        }
        else
        {
            areaTree.GenerateTree();
        }


        //areaTree.GetVertexCount();

        Trees.Add(areaTree);

        ShowTreeInfo(areaTree);
        return areaTree;
    }

    private void ShowTreeInfo(ModelAreaTree areaTree)
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
                if (render == null) continue;
                if (render.sharedMaterial == null) continue;
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

    public bool IsByLOD = false;

    public int VertexCount = 0;

    public int RendererCount = 0;

    public int CombinedCount = 0;

    public int MatCount = 0;

    public int LeafCount;


    [ContextMenu("UpdateTrees")]
    public List<ModelAreaTree> UpdateTrees()
    {
        Trees = GameObject.FindObjectsOfType<ModelAreaTree>(true).ToList();
        return Trees;
    }

    [ContextMenu("SaveTreeNodeScenes")]
    public void SaveTreeNodeScenes()
    {
        DateTime start = DateTime.Now;
        UpdateTrees();
        foreach (var tree in Trees)
        {
            tree.GetScenes();
        }
        Debug.LogError($"GetTreeNodeScenes \t{(DateTime.Now - start).ToString()}");
    }

        [ContextMenu("GetTreeNodeScenes")]
    public void GetTreeNodeScenes()
    {
        DateTime start = DateTime.Now;
        UpdateTrees();
        foreach(var tree in Trees)
        {
            tree.GetScenes();
        }
        Debug.LogError($"GetTreeNodeScenes \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("GetTreeInfos")]
    public void GetTreeInfos()
    {
        ClearCount();
        UpdateTrees();
        foreach(var tree in Trees)
        {
            ShowTreeInfo(tree);
        }
    }

    [ContextMenu("ClearTreesEx")]
    public void ClearTreesEx()
    {
        UpdateTrees();
        Clear();
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        Debug.Log($"AreaTreeManager.ClearTrees trees:{Trees.Count}");

        //AreaTreeHelper.render2NodeDict.Clear();

//        foreach (var tree in Trees)
//        {
//            try
//            {
//                if (tree == null) continue;
//#if UNITY_EDITOR
//                EditorHelper.UnpackPrefab(tree.gameObject);
//#endif
//                GameObject.DestroyImmediate(tree.gameObject);
//            }
//            catch (Exception ex)
//            {
//                Debug.LogWarning($"ClearTrees tree:{tree},Excption:{ex}");//一般是某个预设里的树不让删除
//            }
            
//        }
        Trees.Clear();
    }

     [ContextMenu("CreateHiddenOne")]
    public void CreateHiddenOne()
    {
        DateTime start = DateTime.Now;
        Clear();

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

        CreateTree(GetTarget(),false,"_HiddenTree",hiddenRenderers, p =>
        {

        });//动态显示模型的树
        
        Debug.LogError($"CreateHiddenOne \t{(DateTime.Now - start).ToString()}");
    }

    public void Clear()
    {
        ClearCount();
        ClearTrees();
    }

    public void AddTrees(ModelAreaTree[] trees)
    {
        Debug.Log("AreaTreeManager.AddTrees:"+trees.Length);

        foreach(var tree in trees)
        {
            if (tree == null) continue;
            if(!Trees.Contains(tree))
            {
                ShowTreeInfo(tree);
                Trees.Add(tree);
            }
            
        }
    }

    [ContextMenu("* TestCreateOne_BigSmall")]
    public void TestCreateOne_BigSmall()
    {
        var target = GetTarget();
        CreateOne_BigSmall(null, target,null);
    }

    //public AreaTreeNodeShowManager TreeNodeShowManager;

    public ModelAreaTree[] CreateOne_BigSmall_Core(Transform parent, GameObject target,Action<float> progressChanged)
    {
        IsCopy = false;

        DateTime start = DateTime.Now;

        prefabInstanceBuilder.IsCopyTargetRoot = this.IsCopy;
        prefabInstanceBuilder.TargetRoots = target;
        var bigSmallInfo=prefabInstanceBuilder.GetBigSmallRenderers();

        if (progressChanged!=null)
        {
            progressChanged(0);
        }
        ModelAreaTree tree2 = null;
        if (bigSmallInfo.smallModels.Count>0)
        {
            tree2 = CreateTree(target, isCombine, "_SamllTree", bigSmallInfo.smallModels.ToArray(),p=>
            {
                if (progressChanged != null)
                {
                    progressChanged((0+p)/2);
                }
            });//动态显示模型的树
            tree2.IsHidden = true;
            if (isCombine)
            {
                tree2.HideRenderers();
            }
            else
            {
                tree2.MoveRenderers();
            }
            tree2.transform.SetParent(parent);

            tree2.DestroyNodeRender();
        }

        if (progressChanged != null)
        {
            progressChanged(0.5f);
        }
        ModelAreaTree tree1 = null;
        if (bigSmallInfo.bigModels.Count>0)
        {
            tree1 = CreateTree(target, isCombine, "_BigTree", bigSmallInfo.bigModels.ToArray(), p =>
            {
                if (progressChanged != null)
                {
                    progressChanged((1f + p) / 2);
                }
            });//合并模型的树
            if (isCombine)
            {
                tree1.HideRenderers();
            }
            else
            {
                tree1.MoveRenderers();
            }
            tree1.transform.SetParent(parent);
        }


        //TreeNodeShowManager.HiddenTrees.Add(tree2);
        if (progressChanged != null)
        {
            progressChanged(1f);
        }

        ModelAreaTree[] trees = new ModelAreaTree[2] { tree1, tree2 };
        Debug.LogWarning($"CreateOne_BigSmall_Core \t{(DateTime.Now - start).TotalMilliseconds:F1}ms");
        return trees;
    }

    //[ContextMenu("CreateOne_BigSmall")]
    private ModelAreaTree[] CreateOne_BigSmall(Transform parent,GameObject target, Action<float> progressChanged)
    {
        DateTime start = DateTime.Now;
        Clear();
        var trees=CreateOne_BigSmall_Core(parent, target, progressChanged);
        Debug.LogWarning($"CreateOne_BigSmall \t{(DateTime.Now - start).ToString()}");
        return trees;
    }

    [ContextMenu("CreateOne_HiddenShown")]
    public void CreateOne_HiddenShown()
    {
        IsCopy=false;

        DateTime start = DateTime.Now;
        Clear();
        var target=GetTarget();
        MeshRenderer[] hiddenRenderers=null;
        if (prefabInstanceBuilder != null)
        {
            hiddenRenderers = prefabInstanceBuilder.GetHiddenRenderers().ToArray();
        }
        var tree2=CreateTree(target,isCombine,"_HiddenTree",hiddenRenderers, p =>
        {

        });//动态显示模型的树
        tree2.IsHidden=true;
        tree2.HideRenderers();

        MeshRenderer[] combinedRenderers=null;
        if (prefabInstanceBuilder != null)
        {
            combinedRenderers = prefabInstanceBuilder.GetCombinedRenderers().ToArray();
        }
        var tree1=CreateTree(target,isCombine,"_ShownTree",combinedRenderers, p =>
        {

        });//合并模型的树

        //TreeNodeShowManager.HiddenTrees.Add(tree2);
        tree2.DestroyNodeRender();
        Debug.LogError($"CreateCombinedOne \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("CreateOne")]
    public void CreateOne()
    {
        IsCopy=false;

        DateTime start = DateTime.Now;
        Clear();
        var target=GetTarget();
        MeshRenderer[] combinedRenderers=null;
        if (prefabInstanceBuilder != null)
        {
            combinedRenderers = prefabInstanceBuilder.GetCombinedRenderers().ToArray();
        }
        var tree1=CreateTree(target,isCombine,"_Tree",combinedRenderers, p =>
        {

        });//合并模型的树

        Debug.LogError($"CreateOne \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("CreateCombinedChildren")]
    public void CreateCombinedChildren()
    {
        DateTime start = DateTime.Now;
        Clear();
        var target=GetTarget();
        for (int i = 0; i < target.transform.childCount; i++)
        {
            var child = target.transform.GetChild(i);
            CreateTree(child.gameObject,isCombine,"_CombineTree",null, p =>
            {

            });
        }

        //AvgCount /= Target.transform.childCount;
        nodeStatics.AvgCellRendererCount /= target.transform.childCount;

        Debug.LogError($"CreateOne \t{(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("ToReanderers")]
    public void ToReanderers()
    {
         UpdateTrees();
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            tree.SwitchToRenderers();
        }
    }

    [ContextMenu("ToCombined")]
    public void ToCombined()
    {
         UpdateTrees();
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            tree.SwitchToCombined();
        }
    }

    [ContextMenu("CreateDictionary")]
    public void CreateDictionary()
    {
        DateTime start = DateTime.Now;
        UpdateTrees();

        AreaTreeHelper.ClearDict();

        //Debug.Log("AreaTreeManager.CreateDictionary:" + Trees.Count);
        int nodeCount = 0;
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            //Debug.Log("CreateDictionary tree:" + tree);
            tree.CreateDictionary();
            nodeCount += tree.TreeLeafs.Count;
        }
        Debug.LogWarning($"CreateDictionary trees:{Trees.Count}, nodes:{nodeCount} \trender2NodeDict:{AreaTreeHelper.render2NodeDict.Count},\trenderId2NodeDict:{AreaTreeHelper.renderId2NodeDict.Count}, \t{(DateTime.Now - start).TotalMilliseconds:F1}ms");
    }

    [ContextMenu("HideLeafNodes")]
    public void HideLeafNodes()
    {
         UpdateTrees();
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            tree.HideLeafNodes();
        }
    }

    [ContextMenu("ShowLeafNodes")]
    public void ShowLeafNodes()
    {
         UpdateTrees();
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            tree.ShowLeafNodes();
        }
    }

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
         UpdateTrees();
        int count = 0;
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            tree.ShowRenderers();
            count++;
        }
        Debug.Log("ShowRenderers:"+ count);
    }

    [ContextMenu("HideRenderers")]
    public void HideRenderers()
    {
         UpdateTrees();
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            tree.HideRenderers();
        }
    }

    [ContextMenu("GetMaterials")]
    public void GetMaterials()
    {
        DateTime start = DateTime.Now;

        ShowRenderers();
        var target=GetTarget();
        List<string> matKeys = new List<string>();
        List<Material> mats = new List<Material>();
        var renders = target.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var render in renders)
        {
            if (!mats.Contains(render.sharedMaterial))
            {
                mats.Add(render.sharedMaterial);
            }
        }
        var matsEx = MeshCombineHelper.GetMatFilters(target, false);
        Debug.Log($"GetMaterials {(DateTime.Now - start).ToString()},mats1:{mats.Count},mats2:{matsEx.Count},count:{renders.Length}");
    }


    [ContextMenu("SetMaterials")]
    public void SetMaterials()
    {
        DateTime start = DateTime.Now;
        var target=GetTarget();
        ShowRenderers();
        var mats = MeshCombineHelper.GetMatFilters(target, true);

        //MeshCombineHelper.SetMaterials(target);

        Debug.Log($"SetMaterials {(DateTime.Now - start).ToString()},mats:{mats.Count}");
    }

    [ContextMenu("CheckNodeName")]
    public void CheckNodeName()
    {
        DateTime start = DateTime.Now;
        int count=0;
         UpdateTrees();
        foreach (var tree in Trees)
        {
            if (tree == null) continue;
            count+=tree.CheckLeafNodeName();
        }
        Debug.Log($"CheckNodeName tree:{Trees.Count} errorCount:{count} time:{(DateTime.Now - start).ToString()}");
    }
}
