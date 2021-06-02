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
            node.CombineMesh(this.IsCopy);
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

        CombinedCount = newRenderCount;

        Debug.LogError($"CombineMesh renderCount:{renderCount}->{newRenderCount},\t{(DateTime.Now-start).ToString()}");
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

    public MeshRenderer[] TreeRenderers;

    public MeshRenderer[] GetTreeRendererers()
    {
        if(TreeRenderers!=null&&TreeRenderers.Length>0){
            return TreeRenderers;
        }
        else if(Target!=null){
            var renderers=Target.GetComponentsInChildren<MeshRenderer>();
            return renderers;
        }
        else{
            return TreeRenderers;
        }
    }

    [ContextMenu("CreateCells_Tree")]
    public void CreateCells_Tree()
    {
        

        //var allCount=Count.x*Count.y*Count.z;
        DateTime start=DateTime.Now;
        ClearChildren();

        MeshRenderer[] renderers=GetTreeRendererers();
        
        Debug.LogError("renderers:"+renderers.Length);
        foreach(var render in renderers){
            render.enabled=true;
        }

        Bounds bounds=ColliderHelper.CaculateBounds(renderers);
        Debug.LogError("size:"+bounds.size);
        Debug.LogError("size2:"+bounds.size/2);

        this.TreeNodes.Clear();

        LevelDepth = 0;
        GameObject rootCube=AreaTreeHelper.CreateBoundsCube(bounds,$"RootNode",null);
        AreaTreeNode node=rootCube.AddComponent<AreaTreeNode>();
        DestoryNodes();
        this.RootNode=node;
        this.TreeNodes.Add(node);

        node.Bounds=bounds;
        node.AddRenderers(renderers.ToList());
        
        node.CreateSubNodes(0,MinLevel,MaxLevel,0,this,MaxRenderCount);

        var allCount=this.TreeNodes.Count;
        
        int cellCount=ClearNodes();
        CellCount = cellCount;
        AvgCellRendererCount =(int)(renderers.Length/cellCount);

        rootCube.transform.SetParent(this.transform);

        Debug.LogError($"CreateCells_Tree cellCount:{cellCount}/{allCount},\tavg:{AvgCellRendererCount},\t{(DateTime.Now-start).ToString()}");
    }

    public int CellCount = 0;

    public int AvgCellRendererCount=0;

    public int MaxCellRendererCount=0;

    public int MinCellRendererCount=0;



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

        if (TreeRenderers != null && TreeRenderers.Length > 0)
        {
            foreach (var render in TreeRenderers)
            {
                render.enabled = isVisible;
                render.gameObject.SetActive(isVisible);
            }
            Debug.LogError($"ShowRenderers renderers:{TreeRenderers.Length},\t{(DateTime.Now - start).ToString()}");
        }
        else if (Target != null)
        {
            var ts = AreaTreeHelper.GetAllTransforms(Target.transform);
            foreach (var t in ts)
            {
                t.gameObject.SetActive(true);
            }
            var renderers = Target.GetComponentsInChildren<MeshRenderer>();
            //var renderers=Target.GetComponentsInChildren<MeshRenderer>();
            foreach (var render in renderers)
            {
                render.enabled = isVisible;
                render.gameObject.SetActive(isVisible);
            }
            Debug.LogError($"ShowRenderers renderers:{renderers.Length},\t{(DateTime.Now - start).ToString()}");
        }
        else
        {

            foreach (var render in TreeRenderers)
            {
                render.enabled = isVisible;
                render.gameObject.SetActive(isVisible);
            }
            Debug.LogError($"ShowRenderers renderers:{TreeRenderers.Length},\t{(DateTime.Now - start).ToString()}");
        }
    }

    [ContextMenu("AddColliders")]
    public void AddColliders()
    {
        DateTime start=DateTime.Now;
        var renderers=GetTreeRendererers();
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
        Debug.LogError($"CreateDictionary Start");
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
        Debug.LogError($"CreateDictionary End render2NodeDict:{AreaTreeHelper.render2NodeDict.Count},\t{(DateTime.Now-start).ToString()}");
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
                    render.transform.SetParent(Target.transform);
                }
            }
            if(RootNode!=null)
                GameObject.DestroyImmediate(RootNode.gameObject);
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
        var matsEx = MeshCombineHelper.GetMatFilters(Target, out count,false);
        //foreach(var mat in mats.Keys)
        //{
        //    var list = mats[mat];
        //    foreach(var item in list)
        //    {
        //        MeshRenderer renderer = item.GetComponent<MeshRenderer>();
        //        renderer.sharedMaterial = mat;
        //    }
        //}

        //MeshCombineHelper.SetMaterials(Target);

        Debug.LogError($"GetMaterials {(DateTime.Now - start).ToString()},mats1:{mats.Count},mats2:{matsEx.Count},count:{count}");
    }


    [ContextMenu("SetMaterials")]
    public void SetMaterials()
    {
        DateTime start = DateTime.Now;

        ShowRenderers();

        int count = 0;
        var mats = MeshCombineHelper.GetMatFilters(Target, out count,true);

        //MeshCombineHelper.SetMaterials(Target);

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
            leafNode.SaveMeshes(dir);
        }
        ProgressBarHelper.ClearProgressBar();

        Debug.LogError($"SaveMeshes {(DateTime.Now - start).ToString()}");
    }

    [ContextMenu("SaveTree")]
    public void SaveTree()
    {
        DateTime start = DateTime.Now;

        //string guid = UnityEditor.AssetDatabase.CreateFolder("Assets", "My Folder");
        //Debug.LogError("guid:" + guid);
        //string newFolderPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
        //Debug.LogError("newFolderPath:" + newFolderPath);

        string parentDir = "Assets/Models/Instances/Trees";
        string guid2 = UnityEditor.AssetDatabase.CreateFolder(parentDir, Target.name+"_"+ Target.GetInstanceID());
        Debug.LogError("guid2:" + guid2);
        string newFolderPath2 = UnityEditor.AssetDatabase.GUIDToAssetPath(guid2);
        Debug.LogError("newFolderPath2:" + newFolderPath2);

        SaveMeshes(newFolderPath2);

        string prefabPath = parentDir+"/" + this.gameObject.name +"_"+ this.gameObject.GetInstanceID()+ ".prefab";
        GameObject obj=UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(this.gameObject, prefabPath,UnityEditor.InteractionMode.UserAction);

        Debug.LogError($"SaveTree {(DateTime.Now - start).ToString()}");
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
