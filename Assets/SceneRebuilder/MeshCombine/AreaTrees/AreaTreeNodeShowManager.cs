using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AreaTreeNodeShowManager : MonoBehaviour
{
    public static AreaTreeNodeShowManager Instance;

    public List<Camera> cameras=new List<Camera>();
    public List<ModelAreaTree> HiddenTrees=new List<ModelAreaTree>();
    public List<float> HiddenTreesVertex = new List<float>();

    public List<ModelAreaTree> ShownTrees=new List<ModelAreaTree>();
    public List<float> ShownTreesVertex = new List<float>();

    public List<AreaTreeNode> HiddenLeafNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> ShownLeafNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> ShownNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> HiddenNodes=new List<AreaTreeNode>();

    public float HiddenTreesVertexCount = 0;

    public float ShownTreesVertexCount = 0;

    public float ShowNodeDistance=1600;//40

    public float HideNodeDistance = 3600;//60

    private void Awake()
    {
        if (Instance != null)
        {
            GameObject.Destroy(Instance);
        }
        Instance = this;
    }

    void Start()
    {
        InitCameras();
        Init();
    }

    public void Init()
    {
        InitTrees();
        GetLeafNodes();
    }

    public void RegistHiddenTree(ModelAreaTree tree)
    {
        if (tree == null) return;
        if (tree.IsHidden == false) return;
        if (HiddenTrees.Contains(tree))
        {
            return;
        }
        HiddenTrees.Add(tree);
        HiddenTreesVertexCount += tree.VertexCount;

        GetHiddenTreeLeafs(tree);
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

    [ContextMenu("InitTrees")]
    private void InitTrees()
    {
        HiddenTrees.Clear();
        ShownTrees.Clear();

        HiddenTreesVertexCount = 0;
        ShownTreesVertexCount = 0;

        var ts=GameObject.FindObjectsOfType<ModelAreaTree>();
        foreach(ModelAreaTree t in ts)
        {
            if(t.IsHidden && !HiddenTrees.Contains(t))
            {
                HiddenTrees.Add(t);
                HiddenTreesVertexCount += t.VertexCount;
            }
            else if(t.IsHidden==false && !ShownTrees.Contains(t))
            {
                ShownTrees.Add(t);
                ShownTreesVertexCount += t.VertexCount;
            }
        }

        ShowSortedHiddenTrees();

        ShowSortedShownTrees();
    }

    [ContextMenu("InitCameras")]
    private void InitCameras()
    {
        var cms=GameObject.FindObjectsOfType<Camera>();
        foreach(var cm in cms)
        {
            if(!cameras.Contains(cm))
            {
                cameras.Add(cm);
            }
        }
    }

    private void GetHiddenTreeLeafs(ModelAreaTree tree)
    {
        if (tree == null) return;
        var leafs = tree.TreeLeafs;
        foreach (var node in leafs)
        {
            if (node == null) continue;
            HiddenLeafNodes.Add(node);
        }
    }

    [ContextMenu("GetLeafNodes")]
    public void GetLeafNodes()
    {
        HiddenLeafNodes.Clear();

        foreach(var tree in HiddenTrees)
        {
            GetHiddenTreeLeafs(tree);
        }

        ShownLeafNodes.Clear();
        foreach(var tree in ShownTrees)
        {
            if(tree==null)continue;
            var leafs=tree.TreeLeafs;
            foreach(var node in leafs){
                if(node==null)continue;
                ShownLeafNodes.Add(node);
            }
        }
    }

    public float AvgDistance=0;
    public float MinDistance=0;
    public float MaxDistance=0;

    public int ShownRenderCount=0;

    public int HiddenRenderCount=0;

    public bool IsDisToBounds = false;

    public void Update()
    {
        DateTime start=DateTime.Now;

        ShownNodes.Clear();
        HiddenNodes.Clear();
        AvgDistance=0;
        MinDistance=float.MaxValue;
        MaxDistance=0;
        ShownRenderCount=0;
        HiddenRenderCount=0;
        float sum=0;
        int count=HiddenLeafNodes.Count;
        foreach(var node in HiddenLeafNodes)
        {
            var bounds = node.Bounds;
            var nodePos=node.transform.position;
            float nodeDis1=float.MaxValue;
            float nodeDis2=float.MaxValue;
            foreach(var cam in cameras)
            {
                if(cam==null)continue;
                var camPos=cam.transform.position;

                //float dis = 0;
                //if(IsDisToBounds)
                //{
                //    dis = bounds.SqrDistance(camPos);
                //}
                //else
                //{
                //    //dis = Vector3.SqrMagnitude(camPos - nodePos);
                //    dis = Vector3.Distance(camPos,nodePos);
                //}

                float dis = bounds.SqrDistance(camPos);

                if (dis < nodeDis1)
                {
                    nodeDis1 = dis;
                }
            }
            if(nodeDis1<=ShowNodeDistance)
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

            node.DistanceToCamera=nodeDis1;
            if(nodeDis1>MaxDistance)
            {
                MaxDistance=nodeDis1;
            }
            if(nodeDis1<MinDistance)
            {
                MinDistance=nodeDis1;
            }
            sum+=nodeDis1;
        }
        

        AvgDistance=sum/count;

        UpdateTime1=(DateTime.Now-start).TotalMilliseconds;
        start=DateTime.Now;
        //Debug.Log("HideNodes:" + HiddenNodes.Count);
        foreach (var node in HiddenNodes)
        {
            //node.HideRenders();
            node.HideNodes();
            HiddenRenderCount+=node.RendererCount;
        }
        //Debug.Log("ShownNodes:" + ShownNodes.Count);
        foreach (var node in ShownNodes)
        {
            //node.ShowRenders();
            node.ShowNodes();
            ShownRenderCount+=node.RendererCount;
        }

        UpdateTime2=(DateTime.Now-start).TotalMilliseconds;
        //Debug.Log($"AreaTreeNodeShowManager Update usedTime:{usedTime.TotalMilliseconds}ms");
    }

    public double UpdateTime1=0;
    public double UpdateTime2=0;
}