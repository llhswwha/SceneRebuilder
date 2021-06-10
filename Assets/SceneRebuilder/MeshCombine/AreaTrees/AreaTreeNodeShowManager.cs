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

    public List<AreaTreeNode> TreeLeafNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> ShownNodes=new List<AreaTreeNode>();

    public List<AreaTreeNode> HiddenNodes=new List<AreaTreeNode>();

    public float ShowNodeDistance=5;

    private void Awake()
    {
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

     [ContextMenu("InitTrees")]
    private void InitTrees()
    {
        var ts=GameObject.FindObjectsOfType<ModelAreaTree>();
        foreach(ModelAreaTree t in ts)
        {
            if(t.IsHidden && !HiddenTrees.Contains(t))
            {
                HiddenTrees.Add(t);
            }
        }
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

    [ContextMenu("GetLeafNodes")]
    public void GetLeafNodes()
    {
        TreeLeafNodes.Clear();

        foreach(var tree in HiddenTrees)
        {
            if(tree==null)continue;
            var leafs=tree.TreeLeafs;
            foreach(var node in leafs){
                if(node==null)continue;
                TreeLeafNodes.Add(node);
            }
            
        }
    }

    public float AvgDistance=0;
    public float MinDistance=0;
    public float MaxDistance=0;

    public int ShownRenderCount=0;

    public int HiddenRenderCount=0;

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
        int count=TreeLeafNodes.Count;
        foreach(var node in TreeLeafNodes)
        {
            var nodePos=node.transform.position;
            float nodeDis1=float.MaxValue;
            float nodeDis2=float.MaxValue;
            foreach(var cam in cameras)
            {
                var camPos=cam.transform.position;
                var dis=Vector3.Distance(camPos,nodePos);
                if(dis<nodeDis1)
                {
                    nodeDis1=dis;
                }
            }
            if(nodeDis1<ShowNodeDistance)
            {
                ShownNodes.Add(node);
            }
            else{
                HiddenNodes.Add(node);
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
        foreach(var node in HiddenNodes)
        {
            //node.HideRenders();
            node.HideNodes();
            HiddenRenderCount+=node.RendererCount;
        }
        foreach(var node in ShownNodes)
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