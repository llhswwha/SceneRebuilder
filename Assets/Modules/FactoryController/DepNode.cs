using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using UnityEngine;
using System;
//using HighlightingSystem;

public class DepNode : MonoBehaviour
{
    public static int index = 0;

    /// <summary>
    /// index
    /// </summary>
    public int id = 0;

    public string Tag = "";

    public string NodeKKS;
    public string NodeName;

    //private int _nodeId = 0;
    /// <summary>
    /// 对应区域ID
    /// </summary>
    public int NodeID;
    //{
    //    get
    //    {
    //        return _nodeId;
    //    }
    //    set
    //    {
    //        if (_nodeId != 0)
    //        {
               
    //        }
    //        else
    //        {
    //            _nodeId = value;
    //        }
            
    //    }
    //}

    [System.NonSerialized] private PhysicalTopology _topoNode;
    public PhysicalTopology TopoNode
    {
        get { return _topoNode; }
        //set
        //{
        //    SetTopoNode(value);
        //}
    }


    public T GetParentNode<T>() where T : DepNode
    {
        if (this is T)
        {
            return (T)this;
        }
        if (ParentNode != null)
        {
            if (ParentNode is T)
            {
                return (T)ParentNode;
            }
            else
            {
                return ParentNode.GetParentNode<T>();
            }
        }
        return null;
    }

    public void SetParentNode(DepNode node)
    {
        ParentNode = node;
        transform.SetParent(node.transform);
    }

    /// <summary>
    /// 找到相关的节点（ID相同的）
    /// </summary>
    /// <param name="focusNode"></param>
    /// <returns></returns>
    public DepNode FindRelationNode(DepNode node)
    {
        if (node == null)
        {
            //Debug.LogError("DepNode.FindRelationNode node==null ");
            return this;//漫游进入建筑，没有focusNode
        }
        DepNode rNode = null;
        if (this.NodeID == node.NodeID)
        {
            rNode = this;
        }
        else
        {
            if (ChildNodes != null)
                foreach (DepNode child in ChildNodes)
                {
                    rNode = child.FindRelationNode(node);
                    if (rNode != null)
                    {
                        return rNode;
                    }
                }
        }
        return rNode;
    }

    public void SetTopoNode(PhysicalTopology node)
    {
        if (_topoNode == node)
        {
            return;
        }
        if (_topoNode != null)//cww_20190414:发现会重复设置，并修改id，会导致重名的两个机房，后面那一个的id被修改，并找不到了。
        {
            return;
        }
        _topoNode = node;
        if (node != null)
        {
            //Debug.Log("DepNode.SetTopoNode:" + node.Name);
            NodeID = node.Id;
            NodeName = node.Name;
            NodeKKS = node.KKS;
            HaveTopNode = true;
        }
        else
        {
            HaveTopNode = false;
        }
    }

    public void SetTopoNodeEx(PhysicalTopology node)
    {
        SetTopoNode(node);
        if (node.Children != null)
        {

        }
    }

    [ContextMenu("RefreshChildrenNodes")]
    public virtual void RefreshChildrenNodes()
    {
        //var childNodes = this.gameObject.GetComponentsInChildren<DepNode>();
        //ChildNodes = new List<DepNode>();
        //ChildNodes.AddRange(childNodes);
        // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes0...");
        ChildNodes = new List<DepNode>();
        // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes0_1...");
        for (int i = 0; i < this.transform.childCount; i++)
        {
            // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes1...");
            var child = this.transform.GetChild(i);
            var depNode = child.GetComponent<DepNode>();
            // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes2...");
            if (depNode)
            {
                // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes3...");
                ChildNodes.Add(depNode);
                // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes4...");
                depNode.ParentNode = this;
            }
            else
            {
                // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes5...");
                //处理J6J11的情况
                for (int j = 0; j < child.childCount; j++)
                {
                    var subChild = child.GetChild(j);
                    var depNode2 = subChild.GetComponent<DepNode>();
                    // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes6...");
                    if (depNode2)
                    {
                        // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes7...");
                        ChildNodes.Add(depNode2);
                        // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes8...");
                        depNode2.ParentNode = this;
                    }
                }
            }
        }

        foreach (var item in ChildNodes)
        {
            // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes9..."+item.gameObject);
            item.RefreshChildrenNodes();//递归
        }
    }

    public bool IsRoom()
    {
        if (TopoNode == null)
        {
            Debug.LogError("IsRoom TopoNode == null");
            return false;
        }
        return TopoNode.Type == AreaTypes.机房 || TopoNode.Type == AreaTypes.范围;
    }

    public bool IsFloor()
    {
        if (TopoNode == null)
        {
            Debug.LogError("IsFloor TopoNode == null");
            return false;
        }
        return TopoNode.Type == AreaTypes.楼层;
    }

    /// <summary>
    /// 是否是大楼
    /// </summary>
    /// <returns></returns>
    public bool IsBuild()
    {
        if (TopoNode == null)
        {
            Debug.LogError("IsFloor TopoNode == null");
            return false;
        }
        return TopoNode.Type == AreaTypes.大楼;
    }

    [HideInInspector]
    public bool HaveTopNode;

    ///// <summary>
    ///// 区域类型
    ///// </summary>
    //public DepType depType;
    /// <summary>
    /// 是否被摄像头聚焦
    /// </summary>
    [HideInInspector]
    public bool IsFocus;
    /// <summary>
    /// 区域物体
    /// </summary>
    //[HideInInspector]
    public GameObject NodeObject;
    /// <summary>
    /// 静态设备存放处
    /// </summary>
    [HideInInspector]
    public GameObject StaticDevContainer;
    /// <summary>
    /// 静态设备(建筑)
    /// </summary>
    [HideInInspector]
    public List<FacilityDevController> StaticDevList;

    ///// <summary>
    ///// 区域范围
    ///// </summary>
    ////[HideInInspector]
    //public MonitorRangeObject monitorRangeObject;
    ///// <summary>
    ///// 父节点名称,以便找到上一级节点
    ///// </summary>
    //public string ParentName;
    /// <summary>
    /// 父节点
    /// </summary>
    [HideInInspector]
    public DepNode ParentNode;
    /// <summary>
    /// 子节点
    /// </summary>
    //[HideInInspector]
    public List<DepNode> ChildNodes;

    [ContextMenu("GetChildren")]
    public void GetChildren()
    {
        ChildNodes.Clear();
        for(int i = 0; i < transform.childCount; i++)
        {
            DepNode node = transform.GetChild(i).GetComponent<DepNode>();
            if (node == null) continue;
            ChildNodes.Add(node);
            node.ParentNode = this;

            node.GetChildren();
        }
        SortChildrenByY();
    }

    [ContextMenu("SortChildrenByY")]
    public void SortChildrenByY()
    {
        ChildNodes.Sort((a, b) =>
        {
            if (a == null) return -1;
            if (b == null) return 1;
            return a.transform.position.y.CompareTo(b.transform.position.y);
        });
    }

    [ContextMenu("Debug_GetChildNodes")]
    public void Debug_GetChildNodes()
    {
        for (int i = 0; i < ChildNodes.Count; i++)
        {
            DepNode node = ChildNodes[i];
            Debug.Log($"[{i}] {node}");
        }
    }

    /// <summary>
    /// 全部节点
    /// </summary>
    [HideInInspector]
    public List<DepNode> AllNodes;

    ///// <summary>
    ///// 地板方块，用于高层定位调整高度，设备编辑等
    ///// </summary>
    //public FloorCubeInfo floorCube;
    /// <summary>
    /// 区域设备是否创建
    /// </summary>
    [HideInInspector]
    public bool IsDevCreate;

    protected void Awake()
    {
        NodeObject = gameObject;
    }

    protected virtual void Start()
    {
        NodeObject = gameObject;
        //FactoryDepManager fdep = GetComponentInParent<FactoryDepManager>();
        //if (fdep != this)
        //{
        //    ParentNode = fdep;
        //}
        id = index++;
    }

    public bool HaveChildren()
    {
        if (ChildNodes == null) return false;
        if (ChildNodes.Count == 0) return false;
        return true;
    }

    //public override string ToString()
    //{
    //    return string.Format("name:{0},nodeId:{1},nodeName:{2},haveChildren:{3},topoNode:{4},depType:{5}", name, NodeID, NodeName,
    //        HaveChildren(), TopoNode != null, depType);
    //}

    ///// <summary>
    ///// 设置该节点下的区域范围
    ///// </summary>
    //public virtual void SetMonitorRangeObject(MonitorRangeObject oT)
    //{
    //    monitorRangeObject = oT;
    //}
    /// <summary>
    /// 打开并聚焦区域
    /// </summary>
    public virtual void OpenDep(Action onComplete = null, bool isFocusT = true)
    {

    }
    /// <summary>
    /// 关闭区域，返回上一层
    /// </summary>
    public virtual void HideDep(Action onComplete = null)
    {

    }
    /// <summary>
    /// 聚焦区域
    /// </summary>
    /// <param name="onComplete"></param>
    public virtual void FocusOn(Action onComplete = null)
    {

    }
    /// <summary>
    /// 取消聚焦
    /// </summary>
    /// <param name="onComplete"></param>
    public virtual void FocusOff(Action onComplete = null)
    {

    }

    public virtual void Unload()
    {

    }

    #region 区域高亮
    /// <summary>
    /// 高亮设备
    /// </summary>
    /// <param name="isHighLightLastOff">是否关闭上一个物体的高亮</param>
    public virtual void HighlightOn(bool isHighLightLastOff = true)
    {
        //HightlightModuleBase h = gameObject.AddMissingComponent<HightlightModuleBase>();
        //Color colorConstant = Color.green;
        ////SetOcculuderState(false);
        //h.ConstantOnImmediate(colorConstant);
        //HighlightManage manager = HighlightManage.Instance;
        //if (manager && isHighLightLastOff)
        //{
        //    manager.SetHightLightDep(this);
        //}
    }
    /// <summary>
    /// 设置遮挡
    /// </summary>
    /// <param name="isOn"></param>
    private void SetOcculuderState(bool isOn)
    {
        //if (isOn)
        //{
        //    HighlighterOccluder[] occulders = transform.GetComponentsInChildren<HighlighterOccluder>(false);
        //    foreach (HighlighterOccluder item in occulders)
        //    {
        //        Highlighter highLight = item.GetComponent<Highlighter>();
        //        if (highLight) highLight.OccluderOn();
        //    }
        //}
        //else
        //{
        //    HighlighterOccluder[] occulders = transform.GetComponentsInChildren<HighlighterOccluder>(false);
        //    foreach (HighlighterOccluder item in occulders)
        //    {
        //        Highlighter highLight = item.GetComponent<Highlighter>();
        //        if (highLight)
        //        {
        //            Debug.LogError("Occulder Off...");
        //            highLight.OccluderOff();
        //            highLight.ReinitMaterials();
        //        }
        //    }
        //}
    }
    /// <summary>
    /// 取消高亮
    /// </summary>
    public virtual void HighLightOff()
    {
        //HightlightModuleBase h = gameObject.AddMissingComponent<HightlightModuleBase>();
        ////SetOcculuderState(true);
        //h.ConstantOffImmediate();
    }
    #endregion
    #region DoorPart
    ///// <summary>
    ///// 区域下，所有门的管理
    ///// </summary>
    //[HideInInspector]
    //public DepDoors Doors;
    //public void InitDoor(DepDoors door)
    //{
    //    door.DoorDep = this;
    //    Doors = door;
    //}
    [HideInInspector]
    public bool IsUnload = false;

    public void SetIsUnload()
    {
        IsUnload = true;
        if (ChildNodes != null)
            foreach (var item in ChildNodes)
            {
                item.SetIsUnload();
            }
    }

    [HideInInspector]
    public bool IsLoaded = false;

    public void SetIsLoaded()
    {
        IsLoaded = true;
        if (ChildNodes != null)
            foreach (var item in ChildNodes)
            {
                item.SetIsLoaded();
            }
    }

    #endregion

    private bool isInitBounds = false;

    private Bounds bounds;

    public virtual bool IsInBounds(Transform t)
    {
        if (isInitBounds == false)
        {
            bounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
            isInitBounds = true;
        }

        return bounds.Contains(t.position);
    }


}
