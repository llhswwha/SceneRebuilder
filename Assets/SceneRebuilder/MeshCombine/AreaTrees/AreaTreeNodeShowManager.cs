using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AreaTreeNodeShowManager : MonoBehaviour
{
    private static AreaTreeNodeShowManager _instance;
    public static AreaTreeNodeShowManager Instance
    {
        get{
            if(_instance==null){
                _instance=GameObject.FindObjectOfType<AreaTreeNodeShowManager>(true);
            }
            return _instance;
        }
    }

    public List<Camera> cameras=new List<Camera>();
    public List<ModelAreaTree> HiddenTrees=new List<ModelAreaTree>();
    public List<float> HiddenTreesVertex = new List<float>();

    public List<ModelAreaTree> ShownTrees=new List<ModelAreaTree>();
    public List<float> ShownTreesVertex = new List<float>();

    public List<AreaTreeNode> HiddenLeafNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> ShownLeafNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> ShownNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> HiddenNodes=new List<AreaTreeNode>();

    public int AllCombinedRenderersCount=0;
    public List<MeshRenderer> AllCombinedRenderers=new List<MeshRenderer>();

    private Dictionary<MeshRenderer,MeshRenderer> dictCombined=new System.Collections.Generic.Dictionary<MeshRenderer,MeshRenderer>();

    public float HiddenTreesVertexCount = 0;

    public float ShownTreesVertexCount = 0;

    public float ShowNodeDistance=1600;//40

    public float HideNodeDistance = 3600;//60

    private void Awake()
    {
        // if (Instance != null)
        // {
        //     GameObject.Destroy(Instance);
        // }
        //Instance = this;
        Init();
    }

    void Start()
    {

        //Init();

        StartCoroutine(UpdateNodeCoroutine());
    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        StopCoroutine("UpdateNodeCoroutine");
    }



    [ContextMenu("Init")]
    public void Init()
    {
        InitCameras();
        InitTrees();
        GetLeafNodes();
    }

    [ContextMenu("DisableHiddenTrees")]
    public void DisableHiddenTrees()
    {
        foreach(ModelAreaTree t in HiddenTrees)
        {
            t.gameObject.SetActive(false);
        }
    }

    [ContextMenu("EnableHiddenTrees")]
    public void EnableHiddenTrees()
    {
        foreach(ModelAreaTree t in HiddenTrees)
        {
            t.gameObject.SetActive(true);
        }
    }

    [ContextMenu("HideSmallNodes")]
    public void HideSmallNodes()
    {
        if (HiddenTrees.Count == 0)
        {
            Init();
        }
        foreach (ModelAreaTree t in HiddenTrees)
        {
            if (t == null) continue;
            t.HideLeafNodes();
        }
    }

    [ContextMenu("ShowSmallNodes")]
    public void ShowSmallNodes()
    {
        if (HiddenTrees.Count == 0)
        {
            Init();
        }
        foreach (ModelAreaTree t in HiddenTrees)
        {
            if (t == null) continue;
            t.ShowLeafNodes();
        }
    }

    public void RegistHiddenTree(ModelAreaTree tree)
    {
        if (tree == null) return;
        if (tree.GetIsHidden() == false) return;
        if (HiddenTrees.Contains(tree))
        {
            return;
        }
        HiddenTrees.Add(tree);
        HiddenTreesVertexCount += tree.VertexCount;

        GetHiddenTreeLeafs(tree);
    }

    public void UnRegistHiddenTree(ModelAreaTree tree)
    {
        //Debug.Log("UnRegistHiddenTree:"+tree);
        if (tree == null) return;
        if (tree.GetIsHidden() == false) return;
        if (HiddenTrees.Contains(tree))
        {
            HiddenTrees.Remove(tree);
        }
        RemoveHiddenTreeLeafs(tree);
    }

    private void ShowSortedHiddenTrees()
    {
        HiddenTreesVertex.Clear();
        HiddenTrees.Sort((a, b) => b.VertexCount.CompareTo(a.VertexCount));
        foreach (var t in HiddenTrees)
        {
            HiddenTreesVertex.Add(t.VertexCount);
        }
    }

    private void ShowSortedShownTrees()
    {
        ShownTreesVertex.Clear();
        ShownTrees.Sort((a, b) => b.VertexCount.CompareTo(a.VertexCount));
        foreach (var t in ShownTrees)
        {
            ShownTreesVertex.Add(t.VertexCount);
        }
    }

    public bool IsCombinedRenderer(MeshRenderer renderer){
        return dictCombined.ContainsKey(renderer);
    }

    [ContextMenu("InitTrees")]
    private void InitTrees()
    {
        HiddenTrees.Clear();
        ShownTrees.Clear();

        HiddenTreesVertexCount = 0;
        ShownTreesVertexCount = 0;

        var ts=GameObject.FindObjectsOfType<ModelAreaTree>(true);
        foreach(ModelAreaTree t in ts)
        {
            if (t == null) continue;
            t.DestroyBoundBox();
            if (t.GetIsHidden() && !HiddenTrees.Contains(t))
            {
                HiddenTrees.Add(t);
                HiddenTreesVertexCount += t.VertexCount;
            }
            else if(t.GetIsHidden() == false && !ShownTrees.Contains(t))
            {
                ShownTrees.Add(t);
                ShownTreesVertexCount += t.VertexCount;
            }

             var leafs = t.TreeLeafs;
            foreach (var node in leafs)
            {
                if (node == null) continue;
                foreach(var r in node.CombinedRenderers)
                {
                    if(r==null)continue;

                    
                    if(!dictCombined.ContainsKey(r))
                    {
                        AllCombinedRenderers.Add(r);
                        AllCombinedRenderersCount++;
                        dictCombined.Add(r,r);
                    }
                    else{
                        Debug.LogError("dictCombined.ContainsKey(r):"+r);
                    }
                }
                
            }

            RegistHiddenTree(t);
        }

        ShowSortedHiddenTrees();

        ShowSortedShownTrees();
    }

    [ContextMenu("InitCameras")]
    private void InitCameras()
    {
        List<Camera> newCameras = new List<Camera>();
        foreach(var c in cameras)
        {
            if (c == null) continue;
            newCameras.Add(c);
        }

        var cms=GameObject.FindObjectsOfType<Camera>();
        foreach(var cm in cms)
        {
            if(!newCameras.Contains(cm))
            {
                newCameras.Add(cm);
            }
        }
        cameras = newCameras;
    }

    private void GetHiddenTreeLeafs(ModelAreaTree tree)
    {
        if (tree == null) return;
        var leafs = tree.TreeLeafs;
        foreach (var node in leafs)
        {
            if (node == null) continue;
            HiddenLeafNodes.Add(node);
            AddIdNodeDict(node);
        }
    }

    private void RemoveHiddenTreeLeafs(ModelAreaTree tree)
    {
        if (tree == null) return;
        var leafs = tree.TreeLeafs;
        foreach (var node in leafs)
        {
            if (node == null) continue;
            HiddenLeafNodes.Remove(node);
        }
    }

    [ContextMenu("GetLeafNodes")]
    public void GetLeafNodes()
    {
        Debug.Log($"AreaTreeNodeShowManager.GetLeafNodes   HiddenLeafNodes:{HiddenLeafNodes.Count} ShownLeafNodes:{ShownLeafNodes.Count} Id2NodeDict:{Id2NodeDict.Count}");

        HiddenLeafNodes.Clear();

        foreach (var tree in HiddenTrees)
        {
            GetHiddenTreeLeafs(tree);
        }

        ShownLeafNodes.Clear();
        Dictionary<AreaTreeNode, ModelAreaTree> node2Tree = new Dictionary<AreaTreeNode, ModelAreaTree>();

        Id2NodeDict.Clear();
        foreach (var tree in ShownTrees)
        {
            if (tree == null) continue;
            var leafs = tree.TreeLeafs;
            foreach (var node in leafs)
            {
                if (node == null) continue;
                if (node2Tree.ContainsKey(node))
                {
                    Debug.LogError($"GetLeafNodes node2Tree.ContainsKey(node) node1:{node} tree:{tree} [path1:{node.transform.GetPath()}] ");
                    continue;
                }
                node2Tree.Add(node,tree);
                ShownLeafNodes.Add(node);
                AddIdNodeDict(node);
            }
        }

        Debug.Log($"GetLeafNodes Id2NodeDict:{Id2NodeDict.Count}");
    }

    public void AddIdNodeDict(AreaTreeNode node)
    {
        int count = 0;
        foreach(var id in node.RenderersId)
        {
            if (string.IsNullOrEmpty(id)) continue;
            if (!Id2NodeDict.ContainsKey(id))
            {
                Id2NodeDict.Add(id, node);
            }
            else
            {
                count++;
                AreaTreeNode node2 = Id2NodeDict[id];
                if (node2 != node)
                {
                    Debug.LogError($"AddIdNodeDict Id2NodeDict.ContainsKey(id)[{count}] id:{id} node1:{node} node2:{node2} [path1:{node.transform.GetPath()}] [path2:{node2.transform.GetPath()}] ");
                }
                else
                {
                    Debug.LogWarning($"AddIdNodeDict Id2NodeDict.ContainsKey(id)[{count}] node==node2 id:{id} node1:{node} node2:{node2} [path1:{node.transform.GetPath()}] [path2:{node2.transform.GetPath()}] ");
                }
                
            }
        }
    }

    internal void MoveRenderer(RendererId rId)
    {
        if (Id2NodeDict.ContainsKey(rId.Id))
        {
            AreaTreeNode node = Id2NodeDict[rId.Id];
            node.AddRendererAfterLoadScene(rId);
            //Debug.LogWarning($"MoveRenderer  Id2NodeDict.NotContainsKey(id) rId:[{rId}] id:{rId.Id} path:{TransformHelper.GetPath(rId.transform)}");
        }
        else
        {
            GameObject go = IdDictionary.GetGo(rId.Id);
            if (go == null)
            {
                Debug.LogError($"MoveRendererToNode  Id2NodeDict.NotContainsKey(id) go == null rId:[{rId}] id:{rId.Id} path:{rId.transform.GetPath()}");
            }
            else
            {
                //Debug.LogWarning($"MoveRendererToNode  Id2NodeDict.NotContainsKey(id) rId:[{rId}] id:{rId.Id} path:{TransformHelper.GetPath(rId.transform)}");
            }
        }
    }

    public Dictionary<string, AreaTreeNode> Id2NodeDict = new Dictionary<string, AreaTreeNode>();

    public float AvgDistance=0;
    public float MinDistance=0;
    public float MaxDistance=0;

    public int ShownRenderCount=0;

    public int HiddenRenderCount=0;

    public bool IsDisToBounds = false;

    public bool IsUpdateTreeNodeByDistance = false;

    /// <summary>
    /// ????????????????????||????????   ?????????????????????????????????????? by wk
    /// </summary>
    /// <returns></returns>
    private bool IsCorssSectionMode()
    {
        //if (crossSectionSystem.Instance)
        //{
        //    return crossSectionSystem.Instance.IsModeActive;
        //}
        //else
        {
            return false;
        }       
    }

    public bool IsBusyLoading = false;

    public IEnumerator UpdateNodeCoroutine()
    {
        while (true)
        {
            if (IsUpdateTreeNodeByDistance && !IsCorssSectionMode())
            {
                if (IsBusyLoading==false)
                {
                    DateTime start = DateTime.Now;

                    ShownNodes.Clear();
                    HiddenNodes.Clear();
                    AvgDistance = 0;
                    MinDistance = float.MaxValue;
                    MaxDistance = 0;
                    ShownRenderCount = 0;
                    HiddenRenderCount = 0;
                    float sum = 0;
                    int count = HiddenLeafNodes.Count;
                    foreach (var node in HiddenLeafNodes)
                    {
                        if (node == null)
                        {
                            //continue;
                            Debug.LogError("node == null");
                            continue;
                        }
                        if (node.gameObject == null)
                        {
                            Debug.LogError("node.gameObject == null");
                            continue;
                        }
                        var bounds = node.Bounds;
                        var nodePos = node.transform.position;
                        float nodeDis1 = float.MaxValue;
                        float nodeDis2 = float.MaxValue;
                        foreach (var cam in cameras)
                        {
                            if (cam == null) continue;
                            var camPos = cam.transform.position;

                            float dis = bounds.SqrDistance(camPos);

                            if (dis < nodeDis1)
                            {
                                nodeDis1 = dis;
                            }
                        }
                        if (nodeDis1 <= ShowNodeDistance)
                        {
                            ShownNodes.Add(node);
                        }
                        //else{
                        //    HiddenNodes.Add(node);
                        //}

                        else if (nodeDis1 > HideNodeDistance)
                        {
                            HiddenNodes.Add(node);
                        }
                        else //[ShowNodeDistance,HideNodeDistance]
                        {
                            if (node.IsNodeVisible)
                            {
                                ShownNodes.Add(node);
                            }
                            else
                            {
                                HiddenNodes.Add(node);
                            }
                        }

                        node.DistanceToCamera = nodeDis1;
                        if (nodeDis1 > MaxDistance)
                        {
                            MaxDistance = nodeDis1;
                        }
                        if (nodeDis1 < MinDistance)
                        {
                            MinDistance = nodeDis1;
                        }
                        sum += nodeDis1;
                    }


                    AvgDistance = sum / count;

                    UpdateTime1 = (DateTime.Now - start).TotalMilliseconds;
                    start = DateTime.Now;
                    //Debug.Log("HideNodes:" + HiddenNodes.Count);
                    for (int i = 0; i < HiddenNodes.Count; i++)
                    {
                        AreaTreeNode node = HiddenNodes[i];
                        //node.HideRenders();
                        node.HideNodes();
                        HiddenRenderCount += node.RendererCount;
                        //yield return null;
                        //HiddenNodesWaiting.Add(node);
                    }
                    //Debug.Log("ShownNodes:" + ShownNodes.Count);

                    if (ShownNodes.Count > 0)
                    {
                        Dictionary<SubScene_Base, AreaTreeNode> scene2Nodes = new Dictionary<SubScene_Base, AreaTreeNode>();
                        List<SubScene_Base> scenes = new List<SubScene_Base>();
                        for (int i = 0; i < ShownNodes.Count; i++)
                        {
                            AreaTreeNode node = ShownNodes[i];
                            ////node.ShowRenders();
                            //

                            if (node.IsLoadingScene) continue;
                            SubScene_Base nodeScene = node.StartLoadingScene();
                            if (nodeScene==null || nodeScene.IsLoaded)
                            {
                                node.ShowNodes();//[OLD]
                            }
                            else
                            {
                                //[NEW]
                                if (nodeScene.IsLoadedOrLoading()) continue;

                                node.ShowLogInfo();
                                scenes.Add(nodeScene);
                                scene2Nodes.Add(nodeScene, node);

                                ShownRenderCount += node.RendererCount;
                            }
                            
                            //yield return null;
                            //ShownNodesWaiting.Add(node);
                        }

                        if (scenes.Count > 0)
                        {
                            IsBusyLoading = true;
                            SubSceneManager.Instance.LoadScenesEx(scenes.ToArray(), (p) =>
                            {
                                Debug.LogError($"AreaTreeNodeShowManager.LoadScene p:{p}");
                                IsBusyLoading = false;
                            }, "UpdateNodeCoroutine");
                        }

                    }


                    UpdateTime2 = (DateTime.Now - start).TotalMilliseconds;
                    //Debug.Log($"AreaTreeNodeShowManager Update usedTime:{usedTime.TotalMilliseconds}ms");

                    //if (ShownNodesWaiting.Count > 0)
                    //{
                    //    AreaTreeNode node = ShownNodesWaiting[0];
                    //    ShownNodesWaiting.RemoveAt(0);
                    //    node.ShowNodes();
                    //}

                    //if (HiddenNodesWaiting.Count > 0)
                    //{
                    //    AreaTreeNode node = HiddenNodesWaiting[0];
                    //    HiddenNodesWaiting.RemoveAt(0);
                    //    node.HideNodes();
                    //}
                }

            }
            yield return new WaitForSeconds(UpdateInterval);
        }
        yield return null;
    }

    public float UpdateInterval = 0.02f;

    //public void UpdateNode()
    //{
    //    if (IsUpdateTreeNodeByDistance && !IsCorssSectionMode())
    //    {
    //        DateTime start = DateTime.Now;

    //        ShownNodes.Clear();
    //        HiddenNodes.Clear();
    //        AvgDistance = 0;
    //        MinDistance = float.MaxValue;
    //        MaxDistance = 0;
    //        ShownRenderCount = 0;
    //        HiddenRenderCount = 0;
    //        float sum = 0;
    //        int count = HiddenLeafNodes.Count;
    //        foreach (var node in HiddenLeafNodes)
    //        {
    //            if (node == null)
    //            {
    //                //continue;
    //                Debug.LogError("node == null");
    //                continue;
    //            }
    //            if (node.gameObject == null)
    //            {
    //                Debug.LogError("node.gameObject == null");
    //                continue;
    //            }
    //            var bounds = node.Bounds;
    //            var nodePos = node.transform.position;
    //            float nodeDis1 = float.MaxValue;
    //            float nodeDis2 = float.MaxValue;
    //            foreach (var cam in cameras)
    //            {
    //                if (cam == null) continue;
    //                var camPos = cam.transform.position;

    //                float dis = bounds.SqrDistance(camPos);

    //                if (dis < nodeDis1)
    //                {
    //                    nodeDis1 = dis;
    //                }
    //            }
    //            if (nodeDis1 <= ShowNodeDistance)
    //            {
    //                ShownNodes.Add(node);
    //            }
    //            //else{
    //            //    HiddenNodes.Add(node);
    //            //}

    //            else if (nodeDis1 > HideNodeDistance)
    //            {
    //                HiddenNodes.Add(node);
    //            }
    //            else //[ShowNodeDistance,HideNodeDistance]
    //            {
    //                if (node.IsNodeVisible)
    //                {
    //                    ShownNodes.Add(node);
    //                }
    //                else
    //                {
    //                    HiddenNodes.Add(node);
    //                }
    //            }

    //            node.DistanceToCamera = nodeDis1;
    //            if (nodeDis1 > MaxDistance)
    //            {
    //                MaxDistance = nodeDis1;
    //            }
    //            if (nodeDis1 < MinDistance)
    //            {
    //                MinDistance = nodeDis1;
    //            }
    //            sum += nodeDis1;
    //        }


    //        AvgDistance = sum / count;

    //        UpdateTime1 = (DateTime.Now - start).TotalMilliseconds;
    //        start = DateTime.Now;
    //        //Debug.Log("HideNodes:" + HiddenNodes.Count);
    //        foreach (var node in HiddenNodes)
    //        {
    //            //node.HideRenders();
    //            node.HideNodes();
    //            HiddenRenderCount += node.RendererCount;

    //            //HiddenNodesWaiting.Add(node);
    //        }
    //        //Debug.Log("ShownNodes:" + ShownNodes.Count);
    //        foreach (var node in ShownNodes)
    //        {
    //            //node.ShowRenders();
    //            node.ShowNodes();
    //            ShownRenderCount += node.RendererCount;

    //            //ShownNodesWaiting.Add(node);
    //        }

    //        UpdateTime2 = (DateTime.Now - start).TotalMilliseconds;
    //        //Debug.Log($"AreaTreeNodeShowManager Update usedTime:{usedTime.TotalMilliseconds}ms");

    //        //if (ShownNodesWaiting.Count > 0)
    //        //{
    //        //    AreaTreeNode node = ShownNodesWaiting[0];
    //        //    ShownNodesWaiting.RemoveAt(0);
    //        //    node.ShowNodes();
    //        //}

    //        //if (HiddenNodesWaiting.Count > 0)
    //        //{
    //        //    AreaTreeNode node = HiddenNodesWaiting[0];
    //        //    HiddenNodesWaiting.RemoveAt(0);
    //        //    node.HideNodes();
    //        //}
    //    }
    //}

    public void Update()
    {

        //UpdateNode();->Start UpdateNodeCoroutine
    }


    //public List<AreaTreeNode> ShownNodesWaiting = new List<AreaTreeNode>();

    //public List<AreaTreeNode> HiddenNodesWaiting = new List<AreaTreeNode>();

    //public List<AreaTreeNode> ShownNodesDone = new List<AreaTreeNode>();

    //public List<AreaTreeNode> HiddenNodesDone = new List<AreaTreeNode>();

    public double UpdateTime1=0;
    public double UpdateTime2=0;
}