using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
//using Assets.M_Plugins.Helpers.Utils;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;
using UnityEngine.AI;
using Unity.Modules.Context;
//using Y_UIFramework;
//using StardardShader;
using NavisPlugins.Infos;
using System.IO;
//using BestHTTP;
//using Location.ReferenceEnitiys;
//using TriLibCore;
//using Unity.RenderStreaming;
using System.Text;

public enum FactoryTypeEnum
{
    SiHui,
    BaoXin,
    ZiBo,
    ZhongShan,
    Min_PoliceStation,
    Min_Prison1,
    Min_Prison4f,
    MaGang_LengZha,
    MaGang_ReZha,
    HuaHong,
    KunShangSuo,
    ZhangJiang,
    MinHang,
    ShiDongKou
}
// public enum RenderPipeline
// {
//     Standard,
//     URP,
//     HDRP
// }
/// <summary>
/// 远程类型 （无，云渲染）
/// </summary>
public enum RemoteMode
{
    None,
    RenderStreaming,
}
public class RoomFactory : MonoBehaviour, IDevManager
{
    public FactoryTypeEnum FactoryType;
    //public RenderPipeline RenderPipelineType;//渲染管线类型
    public RemoteMode RemoteMode;//远程类型 （无，云渲染）

    public bool isVideoRecordMode;
    //private bool isInit;
    //public bool IsInit { get => isInit; set => isInit = value; }

    /// <summary>
    /// 分装对List<DevNode>的操作
    /// </summary>
    public class DevNodeList
    {
        public List<DevNode> nodes = new List<DevNode>();

        public void Add(DevNode node)
        {
            nodes.Add(node);
        }

        public void Remove(DevNode node)
        {
            nodes.Remove(node);
        }

        public int Count
        {
            get
            {
                return nodes.Count;
            }
        }

        //public DevNode Find(string devId)
        //{
        //    return nodes.Find(i => i != null && i.Info.DevID == devId);
        //}

        //public DevNode Find(int devId)
        //{
        //    return nodes.Find(i => i != null && i.Info.Id == devId);
        //}

        //private void ClearNull()
        //{
        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        var node = nodes[i];
        //        if (node == null)
        //        {
        //            nodes.RemoveAt(i);
        //            i--;
        //        }
        //    }
        //}
    }

    public class DepDevDictionary
    {
        public Dictionary<int, DevNodeList> dict = new Dictionary<int, DevNodeList>();

        //public void Add(int id, DevNodeList list)
        //{
        //    dict.Add(id, list);
        //}

        //public bool ContainsKey(int id)
        //{
        //    return dict.ContainsKey(id);
        //}

        //public void TryGetValue(int id,out DevNodeList list)
        //{
        //    dict.TryGetValue(id,out list);
        //}

        //public DevNodeList this[int id]
        //{
        //    get
        //    {
        //        return dict[id];
        //    }
        //    set
        //    {
        //        dict[id] = value;
        //    }
        //}

        //public DevNode FindDev(string devId)
        //{
        //    DevNode dev = null;
        //    foreach (DevNodeList devListTemp in dict.Values)
        //    {
        //        dev = devListTemp.Find(devId);
        //        if (dev != null)
        //        {
        //            return dev;
        //        }
        //    }
        //    return dev;
        //}

        //public DevNode FindDev(int devId)
        //{
        //    DevNode dev = null;
        //    foreach (DevNodeList devListTemp in dict.Values)
        //    {
        //        dev = devListTemp.Find(devId);
        //        if (dev != null)
        //        {
        //            return dev;
        //        }
        //    }
        //    return dev;
        //}

        //public void RemoveDev(string devId)
        //{
        //    foreach (DevNodeList devListTemp in dict.Values)
        //    {
        //        DevNode dev = devListTemp.Find(devId);
        //        if (dev)
        //        {
        //            devListTemp.Remove(dev);
        //            break;
        //        }
        //    }
        //}

        /// <summary>
        /// 获取区域下的所有设备
        /// </summary>
        /// <param name="dep">区域</param>
        /// <param name="containRoomDev">是否包含房间设备（Floor）</param>
        /// <returns></returns>
        public List<DevNode> GetDepDevs(DepNode dep, bool containRoomDev = true)
        {
            List<DevNode> depDevs = new List<DevNode>();
            DevNodeList devListTemp;
            dict.TryGetValue(dep.NodeID, out devListTemp);
            if (devListTemp == null) devListTemp = new DevNodeList();
            if (devListTemp.Count != 0) depDevs.AddRange(devListTemp.nodes);
            //楼层下，包括楼层设备+房间设备
            if (dep is FloorController && containRoomDev)
            {
                foreach (var room in dep.ChildNodes)
                {
                    DevNodeList roomDevs;
                    dict.TryGetValue(room.NodeID, out roomDevs);
                    if (roomDevs != null) depDevs.AddRange(roomDevs.nodes);
                }
            }
            return depDevs;
        }

        public void AddDev(int depId, DevNode dev)
        {
            if (!dict.ContainsKey(depId))
            {
                var devList = new DevNodeList();
                devList.Add(dev);
                dict.Add(depId, devList);
            }
            else
            {
                DevNodeList devNodes;
                dict.TryGetValue(depId, out devNodes);
                if (devNodes != null)
                {
                    devNodes.Add(dev);
                }
                else
                {
                    devNodes = new DevNodeList();
                    devNodes.Add(dev);
                    dict[depId] = devNodes;
                }
            }
        }

        /// <summary>
        /// 通过设备Id获取已经创建的设备
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="parentId"></param>
        /// <returns>返回已经创建的设备</returns>
        //public DevNode GetDev(string devId, int depId)
        //{
        //    if (dict.ContainsKey(depId))
        //    {
        //        var devList = dict[depId];//缓存已经创建的区域设备
        //        int? devIdTemp = RoomFactory.TryGetDevId(devId);
        //        if (devIdTemp == null)
        //        {
        //            var dev = devList.Find(devId);
        //            return dev;
        //        }
        //        else
        //        {
        //            var dev = devList.Find((int)devIdTemp);
        //            return dev;
        //        }
        //    }
        //    return null;
        //}
    }

    //public class DepNodeList
    //{
    //    public List<DepNode> nodes = new List<DepNode>();

    //    public Dictionary<int, DepNode> idDict = new Dictionary<int, DepNode>();

    //    public Dictionary<string, DepNode> nameDict = new Dictionary<string, DepNode>();

    //    //private void InitIdDict()
    //    //{
    //    //    if (idDict.Count != nodes.Count)
    //    //    {
    //    //        idDict.Clear();
    //    //        foreach (var node in nodes)
    //    //        {
    //    //            if (!idDict.ContainsKey(node.NodeID))
    //    //            {
    //    //                idDict.Add(node.NodeID, node);
    //    //            }
    //    //            else
    //    //            {
    //    //                Debug.LogError($"InitIdDict idDict.ContainsKey(node.NodeID) id:{node.NodeID} name:{node.NodeName} dict:{idDict.Count}");
    //    //            }
    //    //        }
    //    //    }
    //    //}

    //    public DepNode Find(int nodeId)
    //    {
    //        //InitIdDict();
    //        if (idDict.ContainsKey(nodeId)) return idDict[nodeId];
    //        return nodes.FirstOrDefault(i => i != null && i.NodeID == nodeId);
    //    }

    //    public DepNode Find(string nameT)
    //    {
    //        return nodes.FirstOrDefault(i => i != null && i.NodeName == nameT);
    //    }

    //    public DepNode Search(int nodeId)
    //    {
    //        DepNode result = null;
    //        //var nodes = GameObject.FindObjectsOfTypeAll(typeof(DepNode));

    //        var nodes = Resources.FindObjectsOfTypeAll(typeof(DepNode));
    //        List<DepNode> errorNodes = new List<DepNode>();
    //        foreach (DepNode node in nodes)
    //        {
    //            if (node == null) continue;
    //            DepNode nodeBuffer = Find(node.NodeID);
    //            if (nodeBuffer == null)
    //            {
    //                errorNodes.Add(node);
    //            }
    //            if (node.NodeID == nodeId)
    //            {
    //                result = node;
    //            }
    //        }
    //        foreach (var node in errorNodes)
    //        {
    //            //Debug.Log("FindNode : "+node.name);
    //            Add(node);
    //        }
    //        return result;
    //    }

    //    public DepNode FindByName(string nodeName)
    //    {
    //        return nodes.FirstOrDefault(i => i != null && i.NodeName == nodeName);
    //    }

    //    /// <summary>
    //    /// 通过区域ID,删除脚本
    //    /// </summary>
    //    /// <param name="physicalTopologyId"></param>
    //    /// <returns></returns>
    //    public DepNode Remove(int nodeId)
    //    {
    //        DepNode node = Find(nodeId);
    //        if (node != null)
    //        {
    //            nodes.Remove(node);
    //        }
    //        return node;
    //    }

    //    /// <summary>
    //    /// 通过区域ID,删除脚本
    //    /// </summary>
    //    /// <param name="physicalTopologyId"></param>
    //    /// <returns></returns>
    //    public DepNode Replace(DepNode newNode)
    //    {
    //        DepNode oldNode = Find(newNode.NodeID);
    //        if (oldNode != null)
    //        {
    //            nodes.Remove(oldNode);
    //        }
    //        else
    //        {
    //            Debug.LogError("RoomFactory.ReplaceNode oldNode == null :" + newNode.NodeID + "," + newNode);
    //        }
    //        nodes.Add(newNode);
    //        return oldNode;
    //    }

    //    public bool Contains(DepNode node)
    //    {
    //        return nodes.Contains(node);
    //    }

    //    public void Refresh()
    //    {
    //        Debug.Log("RoomFactory.RefreshNodes");
    //        nodes.Clear();
    //        // var findNodes = GameObject.FindObjectsOfTypeAll(typeof(DepNode));
    //        var findNodes = Resources.FindObjectsOfTypeAll(typeof(DepNode));
    //        Debug.Log("nodeCount:" + findNodes.Length);
    //        List<DepNode> errorNodes = new List<DepNode>();
    //        foreach (DepNode node in findNodes)
    //        {
    //            nodes.Add(node);
    //        }
    //    }

    //    public void Add(DepNode node)
    //    {
    //        if (node == null)
    //        {
    //            Log.Alarm("RoomFactory.AddDepNode", "node == null");
    //            return;
    //        }
    //        //if (node.NodeID == 0)
    //        //{
    //        //    Debug.LogError($"[DepNodeList.Add] node.NodeID == 0 {node.name},{node.NodeName},{node.NodeID}");
    //        //}
    //        if (!nodes.Contains(node))
    //        {
    //            //if (node.NodeName == "GIS配电装置楼")
    //            //{
    //            //    int i = 0;
    //            //}
    //            nodes.Add(node);

    //            if (!idDict.ContainsKey(node.NodeID))
    //            {
    //                idDict.Add(node.NodeID, node);
    //            }
    //            else
    //            {
    //                if (node.NodeID != 0)
    //                {
    //                    Debug.LogError($"[DepNodeList.Add] idDict.ContainsKey(node.NodeID) id:{node.NodeID} name:{node.NodeName} dict:{idDict.Count}");
    //                }
    //            }
    //        }
    //        else
    //        {
    //            //Log.Alarm("RoomFactory.AddDepNode", string.Format("存在相同Key的Node,id={0},name={1}", node.NodeID, node.NodeName));
    //            //NodeDic[key] = node;
    //        }
    //    }

    //    /// <summary>
    //    /// 区域信息列表（包含区域、建筑、机房）  区域名称(key)DepNode(value)
    //    /// 移除
    //    /// </summary>
    //    public void Remove(DepNode depNodeT)
    //    {
    //        if (nodes.Contains(depNodeT))
    //        {
    //            Destroy(depNodeT.NodeObject);
    //            nodes.Remove(depNodeT);
    //        }
    //    }

    //    public string GetNodeNames()
    //    {
    //        string names = "";
    //        foreach(var node in nodes)
    //        {
    //            names += $"({node.name},{node.NodeName},{node.NodeID})";
    //        }
    //        return names;
    //    }
    //}

    public static RoomFactory Instance;
    /// <summary>
    /// 是否正在聚焦区域
    /// </summary>
    public bool IsFocusingDep;
    /// <summary>
    /// 区域信息列表（包含区域、建筑、机房）  区域名称(key)DepNode(value)
    /// </summary>
    //public DepNodeList NodeDic = new DepNodeList();
    /// <summary>
    /// 区域下设备列表 区域ID(key) DevNode(value)
    /// </summary>
    private DepDevDictionary DepDevDic = new DepDevDictionary();
    /// <summary>
    /// 静态设备列表
    /// </summary>
    public List<DevNode> StaticDevList = new List<DevNode>();

    private List<DevInfo> staticDevInfos = new List<DevInfo>();//已经加载的静态设备信息
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DevType
    {
        DepDev,
        RoomDev,
        CabinetDev
    }

    // Use this for initialization
    void Start()
    {

        //Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Action onInitComplete = null)
    {
        //DateTime start = DateTime.Now;
        //isTopoInited = false;//开始 
        //Debug.Log("Roomfatory.Init,store dep info start...");
        //StoreDepInfo();//将DepNode存到NodeDic里面
        //Debug.Log("Roomfatory.Init,store dep info end...");
        //BindingModelIDByNodeName(() => //获取AreaTree并关联DepNode
        //{
        //    Debug.Log("Roomfatory.Start save static dev info...");
        //    SaveStaticDevInfo();
        //    if (onInitComplete != null)
        //    {
        //        Debug.Log("Roomfatory.OnInitComplet!=null,init complete...");
        //        onInitComplete();
        //    }

        //    Debug.Log($"Roomfatory.Init AllEnd time:{DateTime.Now-start}");
        //});
        ////SaveStaticDevInfo();
    }

    void Awake()
    {
        Unity.Modules.Context.AppContext.DevManager = this;
        Instance = this;
        //SceneEvents.TopoNodeChanged += SceneEvents_TopoNodeChanged;
    }

    private void SceneEvents_TopoNodeChanged(PhysicalTopology arg1, PhysicalTopology arg2)
    {
        //FocusNode(arg2);
    }

    void OnDestroy()
    {
        //SceneEvents.TopoNodeChanged -= SceneEvents_TopoNodeChanged;
    }
    [ContextMenu("CreateChildRoom")]
    public void CreateChildRoom()
    {
        //StoreDepInfo();

    }
    /// <summary>
    /// 获取当前区域节点下的所有子节点区域Id（建筑节点Id），包括当前节点Id
    /// </summary>
    /// <returns></returns>
    public List<int> GetCurrentDepNodeChildNodeIds(DepNode node)
    {
        List<int> nodeIds = new List<int>();
        if (node == null)
        {
            int i = 0;
        }
        PhysicalTopology topoNode = node.TopoNode;
        if (topoNode == null)
        {
            return nodeIds;
        }
        if (topoNode.Name.Contains("集控楼"))
        {
            int i = 0;
        }
        if (topoNode.Transfrom != null && topoNode.Transfrom.IsOnLocationArea)
        {
            int nodeid = topoNode.Id;
            nodeIds.Add(nodeid);
        }
        if (node.ChildNodes != null)
        {
            foreach (DepNode nodeT in node.ChildNodes)
            {
                if (nodeT == null) continue;
                List<int> nodeIdsT = GetCurrentDepNodeChildNodeIds(nodeT);
                nodeIds.AddRange(nodeIdsT);
            }
        }
        return nodeIds;

    }
    ///// <summary>
    ///// 通过区域ID,获取区域管理脚本
    ///// </summary>
    ///// <param name="nodeId"></param>
    ///// <returns></returns>
    //public DepNode GetDepNodeById(int nodeId, bool toFind = false)
    //{
    //    DepNode node = NodeDic.Find(nodeId);
    //    if (node == null && toFind)
    //    {
    //        //缓存中找不到则到全部里面找
    //        Debug.Log("RoomFactory.GetDepNodeById node == null id:" + nodeId);
    //        Debug.Log("FindDepNodeById");
    //        node = NodeDic.Search(nodeId);
    //        if (node == null)
    //        {
    //            Debug.Log("node == null");
    //        }
    //        else
    //        {
    //            Debug.Log("node:" + node.NodeName);
    //        }
    //    }
    //    if (node == null)
    //    {
    //        Debug.LogError($"GetDepNodeById node == null nodeId:{nodeId} nodes:{NodeDic.GetNodeNames()}");
    //    }
    //    return node;
    //}


    ///// <summary>
    ///// 通过区域名称,获取区域管理脚本
    ///// </summary>
    ///// <param name="nodeId"></param>
    ///// <returns></returns>
    //public DepNode GetDepNodeByName(string nameT)
    //{
    //    DepNode node = NodeDic.Find(nameT);
    //    return node;
    //}

    //public DepNode GetDepNodeByTopo(PhysicalTopology topoNode, bool toFind = false)
    //{
    //    if (topoNode == null) return null;
    //    DepNode node = NodeDic.Find(topoNode.Id);
    //    if (node == null && toFind)
    //    {
    //        //缓存中找不到则到全部里面找
    //        Debug.Log("RoomFactory.GetDepNodeByTopo node == null id:" + topoNode.Id + ",name:" + topoNode.Name);
    //        Debug.Log("FindDepNodeById");
    //        node = NodeDic.Search(topoNode.Id);
    //    }
    //    return node;
    //}

    //[ContextMenu("RefreshNodes")]
    //public void RefreshNodes()
    //{
    //    NodeDic.Refresh();
    //}

    ///// <summary>
    ///// 通过区域ID,删除脚本
    ///// </summary>
    ///// <param name="physicalTopologyId"></param>
    ///// <returns></returns>
    //public DepNode RemoveDepNodeById(int nodeId)
    //{
    //    return NodeDic.Remove(nodeId);
    //}

    ///// <summary>
    ///// 通过区域ID,删除脚本
    ///// </summary>
    ///// <param name="physicalTopologyId"></param>
    ///// <returns></returns>
    //public DepNode ReplaceNode(DepNode newNode)
    //{
    //    return NodeDic.Replace(newNode);
    //}
    //public bool Contains(DepNode node)
    //{
    //    return NodeDic.Contains(node);
    //}

    ///// <summary>
    ///// 根据名称，找到对应区域
    ///// </summary>
    ///// <param name="key"></param>
    ///// <returns></returns>
    //public DepNode GetDepNode(string key)
    //{
    //    DepNode node = NodeDic.FindByName(key);
    //    if (node == null)
    //    {
    //        //Debug.LogError("RoomFactory.GetDepNode node == null:" + key);
    //    }
    //    return node;
    //}


    ///// <summary>
    ///// 根据名称，找到对应区域,如果类型是Types.区域，找不到就创建
    ///// </summary>
    ///// <param name="key"></param>
    ///// <returns></returns>
    //public DepNode GetDepNode(PhysicalTopology pnode)
    //{
    //    DepNode node = GetDepNode(pnode.Name);

    //    if (node == null)
    //    {
    //        if (pnode.Type == Types.区域)//|| pnode.Type == Types.分组
    //        {

    //            GameObject o = new GameObject(pnode.Name+"_区域");
    //            o.transform.SetParent(FactoryDepManager.Instance.Facotory.transform);
    //            DepController depController = o.AddComponent<DepController>();
    //            NodeDic.Add(depController);
    //            depController.depType = DepType.Dep;
    //            depController.NodeID = pnode.Id;
    //            depController.NodeKKS = pnode.KKS;
    //            depController.NodeName = pnode.Name;
    //            FactoryDepManager fdep = o.GetComponentInParent<FactoryDepManager>();
    //            depController.ParentNode = fdep;
    //            node = depController;
    //            if (node == null)
    //            {
    //                Debug.LogError("创建RoomFactory.GetDepNode node:" + pnode.Name + "失败！");
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError($"创建RoomFactory.GetDepNode 失败 id:{pnode.Id} name:{pnode.Name} type:{pnode.Type}");
    //        }
    //    }

    //    return node;
    //}

    ///// <summary>
    ///// 设置区域聚焦状态（是否正在聚焦过程）
    ///// </summary>
    //public void SetDepFoucusingState(bool value)
    //{
    //    IsFocusingDep = value;
    //}
    //public void AddDepNode(DepNode node)
    //{
    //    NodeDic.Add(node);
    //}

    ///// <summary>
    ///// 添加静态设备信息
    ///// </summary>
    ///// <param name="devController"></param>
    //public void SaveStaticDevInfo(DevNode devController)
    //{
    //    //保存新加载的静态设备，完善设备信息
    //    if (devController.Info == null)
    //    {
    //        DevInfo info = staticDevInfos.Find(i => i != null && i.ModelName == devController.gameObject.name);
    //        if (info == null) info = staticDevInfos.Find(i=>i.ModelName==devController.devName);
    //        if (info != null)
    //        {
    //            DepNode parentNode = GetDepNodeById((int)info.ParentId);
    //            if (parentNode != null)
    //            {
    //                SaveDepDevInfo(parentNode, devController, info);
    //                devController.CreateFollowUI();
    //            }
    //        }
    //    }
    //    if (!StaticDevList.Contains(devController))
    //    {
    //        StaticDevList.Add(devController);
    //    }
    //}
    ///// <summary>
    ///// 保存静态设备信息
    ///// </summary>
    ///// <param name="staticDev"></param>
    //private void SaveStaticDevInfo()
    //{
    //    FactoryDepManager manager = FactoryDepManager.Instance;
    //    if (manager)
    //    {
    //        FacilityDevController[] staticDevs = manager.transform.GetComponentsInChildren<FacilityDevController>(true);
    //        StaticDevList.AddRange(staticDevs);
    //    }
    //}
    //#region 建筑ID初始化
    ///// <summary>
    ///// 保存所有区域信息
    ///// </summary>
    //public void StoreDepInfo()
    //{
    //    try
    //    {
    //        FactoryDepManager depManager = FactoryDepManager.Instance;
    //        if (depManager)
    //        {
    //            AddDepNode(depManager);
    //            Debug.Log("Roomfatory.StoreDepInfo,AddDepNode...");
    //            depManager.AllNodes = depManager.transform.GetComponentsInChildren<DepNode>(false).ToList();
    //            List<string> allLogs = new List<string>();
    //            foreach (DepNode item in depManager.ChildNodes)
    //            {
    //                // Debug.Log("Roomfatory.depManager.ChildNodes,foreach..."+item.NodeName);
    //                if (item == null) continue;
    //                item.RefreshChildrenNodes();//刷新子节点，避免手动设置和模型替换是节点丢失
    //                // Debug.Log("Roomfatory.depManager.ChildNodes,RefreshChildrenNodes...");
    //                AddDepNode(item);
    //                if (item.HaveChildren())
    //                {
    //                    var logs=StoreChildInfo(item);
    //                    allLogs.AddRange(logs);
    //                }
    //            }
    //            Debug.Log("Roomfatory.depManager.ChildNodes,End...");
    //            if (allLogs.Count > 0)
    //            {
    //                StringBuilder s = new StringBuilder();
    //                foreach(var log in allLogs)
    //                {
    //                    s.AppendLine(log);
    //                }
    //                Debug.LogError(s.ToString());
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Error:RoomFactory.StoreDepInfo->" + e.ToString());
    //    }

    //}
    ///// <summary>
    ///// 保存所有子区域信息
    ///// </summary>
    ///// <param name="parent"></param>
    //private List<string> StoreChildInfo(DepNode parent)
    //{
    //    List<string> sb = new List<string>();
    //    for (int i = 0; i < parent.ChildNodes.Count; i++)
    //    {
    //        DepNode child = parent.ChildNodes[i];
    //        if (child == null)
    //        {
    //            //if(node!=null)
    //            //{
    //            //    Debug.LogError(string.Format("{0} child is null..", node.NodeName));
    //            //}
    //            continue;
    //        }

    //        if (child.NodeID == 0)
    //        {
    //            //Debug.LogError($"[StoreChildInfo][{parent.name}_{i}] node.NodeID == 0 [{parent.name},{parent.NodeName},{parent.NodeID}] -> [{child.name},{child.NodeName},{child.NodeID}]");

    //            sb.Add($"[StoreChildInfo][{parent.name}_{i}] node.NodeID == 0 [{parent.name},{parent.NodeName},{parent.NodeID}] -> [{child.name},{child.NodeName},{child.NodeID}]");
    //        }

    //        AddDepNode(child);
    //        if (child.HaveChildren())
    //        {
    //            var subLog=StoreChildInfo(child);
    //            sb.AddRange(subLog);
    //        }
    //    }

    //    //if (!string.IsNullOrEmpty(sb.ToString()))
    //    //{
    //    //    Debug.LogError(sb.ToString());
    //    //}
    //    return sb;
    //}
    ///// <summary>
    ///// 绑定建筑ID
    ///// </summary>
    //public void BindingModelIDByNodeName(Action callback)
    //{
    //    Debug.Log("RoomFactory->BindingModelIDByNodeName");
    //    //PhysicalTopology topoRoot = CommunicationObject.Instance.GetTopoTree();
    //    //StartBindingTopolgy(topoRoot);
    //    CommunicationObject.Instance.GetTopoTree((topoRoot) =>
    //    {
    //        Debug.Log("RoomFactory->GetTopoTree success,start binding...");
    //        StartBindingTopolgy(topoRoot);
    //        Debug.Log("RoomFactory->Building bind topoTree complete...");
    //        isTopoInited = true;
    //        if (callback != null)
    //        {
    //            callback();
    //        }            
    //    }
    //    );
    //}

    //public bool isTopoInited = false;

    //public void StartBindingTopolgy()
    //{
    //    StartBindingTopolgy(FactoryDepManager.Instance.TopoNode);
    //}

    //private void StartBindingTopolgy(PhysicalTopology toplogy)
    //{
    //    try
    //    {
    //        Log.Info("RoomFactory->StartBindingTopolgy Start !!!!!!!!!!!!!!!11");
    //        if (toplogy == null || toplogy.Children == null || toplogy.Children.Length == 0)
    //        {
    //            Debug.LogError("RoomFactory->PhysicalTopology is null!");
    //            return;
    //        }
    //        Log.Info("RoomFactory->NodeName:" + toplogy.Name);
    //        var factoryTopo = toplogy;//这里传进来就是园区节点（“四会热电厂”)
    //                                  //var topologies = toplogy.Children.ToList();
    //                                  //foreach (var factoryTopo in topologies)
    //        {
    //            //if (factoryTopo.Name == "四会热电厂" || factoryTopo.Name == "高新软件园")
    //            {
    //                var rangesT = new List<PhysicalTopology>();
    //                if (factoryTopo.Children != null)
    //                {
    //                    var factoryTopologies = factoryTopo.Children.ToList();
    //                    for (int i = 0; i < factoryTopologies.Count; i++)
    //                    {
    //                        PhysicalTopology topoNode = factoryTopologies[i];
    //                        if (topoNode == null)
    //                        {
    //                            Debug.LogError($"StartBindingTopolgy[{i}] topoNode == null [factoryTopo:{factoryTopo.Name}]");
    //                            continue;
    //                        }

    //                        //var node = GetDepNode(topoNode.Name);
    //                        var node = GetDepNode(topoNode);

    //                        //Debug.Log($"StartBindingTopolgy[{i}] name:{topoNode.Name} id:{topoNode.Id} type:{topoNode.Type} node:{node}");
    //                        if (node != null)
    //                        {
    //                            node.SetTopoNode(topoNode);
    //                            if (topoNode.Children != null)
    //                            {
    //                                BindingChild(node, topoNode.Children.ToList());
    //                            }
    //                        }
    //                        else
    //                        {
    //                            //if (topoNode.Type == Types.范围)
    //                            //{
    //                            //    rangesT.Add(topoNode);
    //                            //}
    //                            //else
    //                            //{
    //                            //    Log.Alarm("RoomFactory->StartBindingTopolgy", $"未找到DepNode:{topoNode.Name} type:{topoNode.Type}");
    //                            //}

    //                            Debug.LogError($"StartBindingTopolgy[{i}] node==null [topoNode:{topoNode.Name}]");

    //                            rangesT.Add(topoNode);
    //                        }
    //                    }
    //                }

    //                //DepNode toplogyNode = GetDepNode(factoryTopo.Name);
    //                DepNode toplogyNode = GetDepNode(factoryTopo);
    //                if (toplogyNode != null)
    //                {
    //                    toplogyNode.SetTopoNode(factoryTopo);
    //                    AddRanges(toplogyNode, rangesT);
    //                }
    //            }
    //        }
    //        SetParkNode(factoryTopo);
    //        Log.Info("RoomFactory->StartBindingTopolgy End !!!!!!!!!!!!!!!11");
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Error:RoomFactory->StartBindingTopolgy:" + e.ToString());
    //    }
    //}

    //private void SetParkNode(PhysicalTopology toplogy)
    //{
    //    var parks = GameObject.FindObjectsOfType<FactoryDepManager>().ToList();
    //    //var topologies = toplogy.Children.ToList();
    //    //foreach (var factoryTopo in topologies)
    //    //{
    //    //    FactoryDepManager park = parks.Find(i => i.NodeName == factoryTopo.Name);
    //    //    if (park != null)
    //    //    {
    //    //        park.SetTopoNode(factoryTopo);
    //    //    }
    //    //    else
    //    //    {
    //    //        Log.Alarm("StartBindingTopolgy", "未找到园区:" + factoryTopo.Name);
    //    //    }
    //    //}
    //    {
    //        FactoryDepManager park = parks.Find(i => i.NodeName == toplogy.Name);
    //        if (park != null)
    //        {
    //            park.SetTopoNode(toplogy);
    //        }
    //        else
    //        {
    //            Log.Alarm("StartBindingTopolgy", "未找到园区:" + toplogy.Name);
    //        }
    //    }
    //}

    //public void SetTopoNode(DepNode depNode, PhysicalTopology topoNode, bool isReplaceNode)
    //{
    //    if (depNode == null)
    //    {
    //        Debug.LogError("RoomFactory.SetTopoNode depNode==null : " + topoNode);
    //        return;
    //    }
    //    depNode.SetTopoNode(topoNode);
    //    if (isReplaceNode)
    //    {
    //        ReplaceNode(depNode);
    //    }
    //}

    //public void BindingChild(DepNode node, List<PhysicalTopology> topologies, bool isReplaceNode = false)
    //{
    //    if (node == null)
    //    {
    //        Debug.LogError("RoomFactory.BindingChild node == null");
    //        return;
    //    }
    //    List<PhysicalTopology> rangesT = new List<PhysicalTopology>();
    //    if (node.ChildNodes != null)
    //        foreach (var item in node.ChildNodes)
    //        {
    //            if (item == null) continue;
    //            try
    //            {
    //                if (!string.IsNullOrEmpty(item.NodeName))
    //                {
    //                    PhysicalTopology topology = topologies.Find(topo => topo.Name == item.NodeName);
    //                    if (topology != null)
    //                    {
    //                        //item.SetTopoNode(topology);
    //                        SetTopoNode(item, topology, isReplaceNode);
    //                        if (topology.Children == null || topology.Children.Length == 0) continue;
    //                        if (item as FloorController)
    //                        {
    //                            FloorController floor = item as FloorController;
    //                            AddRoomInFloor(floor, topology.Children.ToList(), isReplaceNode);
    //                        }
    //                        BindingChild(item, topology.Children.ToList());
    //                    }
    //                    else
    //                    {
    //                        Log.Alarm("BindingChild", "未找到Topo节点:" + item.NodeName);
    //                    }
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.LogError("RoomFactory.BindingChild:" + item.NodeName + "|" + ex);
    //            }
    //        }
    //    AddRanges(node, topologies);
    //}

    ///// <summary>
    ///// 在厂区中添加范围
    ///// </summary>
    ///// <param name="depNodeT"></param>
    //public void AddRanges(DepNode depNodeT, List<PhysicalTopology> roomTopo)
    //{
    //    //Log.Info("AddRanges", "depNodeT:" + depNodeT.NodeName);
    //    foreach (var topo in roomTopo)
    //    {
    //        AddRange(depNodeT, topo);
    //    }
    //}

    //public RangeController AddRange(DepNode depNodeT, PhysicalTopology topo)
    //{
    //    if (depNodeT == null) return null;
    //    if (topo.Type != Types.范围) return null;
    //    PhysicalTopology topoNode = depNodeT.TopoNode;

    //    if (topoNode == null)
    //    {
    //        Debug.Log("TopoNode is null...");
    //        return null;
    //    }

    //    Transform rangesTran = depNodeT.transform.Find("Ranges");
    //    if (rangesTran == null)
    //    {
    //        rangesTran = CreateGameObject("Ranges", depNodeT.transform).transform;
    //    }
    //    RangeController rangeController = FindRangeController(rangesTran, topo);//已经创建了
    //    if (rangeController != null) return null;//已经创建了
    //    if (GetDepNodeByTopo(topo) != null) return null;
    //    rangeController = CreateRangeController(topo, rangesTran);
    //    rangeController.ParentNode = depNodeT;
    //    if (depNodeT.ChildNodes == null)
    //    {
    //        depNodeT.ChildNodes = new List<DepNode>();
    //    }
    //    depNodeT.ChildNodes.Add(rangeController);
    //    //if (topo.Name == "集控楼4.5m层测试范围1")
    //    //{
    //    //    int i = 0;
    //    //}
    //    NodeDic.Add(rangeController);
    //    return rangeController;
    //}

    //private RangeController FindRangeController(Transform rangeRoot, PhysicalTopology topo)
    //{
    //    var rangeControllers = rangeRoot.FindComponentsInChildren<RangeController>();
    //    foreach (var item in rangeControllers)
    //    {
    //        if (item.NodeID == topo.Id)
    //        {
    //            return item;
    //        }
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 区域信息列表（包含区域、建筑、机房）  区域名称(key)DepNode(value)
    ///// 移除
    ///// </summary>
    //public void NodeDic_Remove(DepNode depNodeT)
    //{
    //    NodeDic.Remove(depNodeT);
    //}

    ///// <summary>
    ///// 在楼层中添加机房
    ///// </summary>
    ///// <param name="floor"></param>
    //private void AddRoomInFloor(FloorController floor, List<PhysicalTopology> roomTopo, bool isReplaceNode)
    //{
    //    PhysicalTopology topoNode = floor.TopoNode;
    //    if (floor == null || floor.ParentNode == null) return;
    //    PhysicalTopology buildingNode = floor.ParentNode.TopoNode;
    //    if (topoNode == null || buildingNode == null)
    //    {
    //        Debug.Log("TopoNode is null...");
    //        return;
    //    }
    //    Transform roomsRootObj = floor.transform.FindChildByName("Rooms");
    //    if (roomsRootObj == null)//判断一下 避免重复创建
    //    {
    //        roomsRootObj = CreateGameObject("Rooms", floor.transform).transform;
    //        foreach (var topo in roomTopo)
    //        {
    //            var node = GetDepNodeByTopo(topo);
    //            if (node != null)
    //            {
    //                if (isReplaceNode)
    //                {
    //                    node = RemoveDepNodeById(topo.Id);
    //                }
    //                else
    //                {
    //                    Debug.LogWarning("AddRoomInFloor. node != null");
    //                    continue;
    //                }
    //            }
    //            if (topo.Type == Types.范围) continue;
    //            AddRoomInFloorOP(floor, roomsRootObj, topo);
    //        }
    //    }
    //    else
    //    {
    //        if (isReplaceNode)//已经有了的情况下，也要替换回去
    //        {
    //            foreach (DepNode item in floor.ChildNodes)
    //            {
    //                ReplaceNode(item);
    //            }
    //        }
    //    }
    //}

    ///// <summary>
    ///// 如果FloorController下已经有RoomController物体了，就不再创建Cube房间
    ///// </summary>
    ///// <param name="floor"></param>
    ///// <param name="rooms"></param>
    ///// <param name="topo"></param>
    //private void AddRoomInFloorOP(FloorController floor, Transform rooms, PhysicalTopology topo)
    //{
    //    if(floor.ChildNodes!=null)
    //    {
    //        DepNode node = floor.ChildNodes.Find(i=>i.NodeName==topo.Name);
    //        if(node!=null)
    //        {
    //            node.SetTopoNode(topo);
    //            node.NodeObject = node.gameObject;
    //            node.ParentNode = floor;
    //            node.transform.parent = rooms;
    //        }
    //        else
    //        {
    //            var roomController = CreateRoomRoomController(topo, rooms);
    //            roomController.ParentNode = floor;
    //            floor.ChildNodes.Add(roomController);
    //            NodeDic.Add(roomController);
    //        }
    //    }
    //    else
    //    {
    //        var roomController = CreateRoomRoomController(topo, rooms);
    //        roomController.ParentNode = floor;
    //        floor.ChildNodes.Add(roomController);
    //        NodeDic.Add(roomController);
    //    }        
    //}

    //private static GameObject CreateGameObject(string objName, Transform parent)
    //{
    //    GameObject obj = new GameObject(objName);
    //    obj.transform.parent = parent;
    //    obj.transform.localEulerAngles = Vector3.zero;
    //    //obj.transform.position = Vector3.zero;
    //    obj.transform.position = parent.parent.transform.position;//设置Ranges的父物体的位置，为默认位置
    //    obj.transform.localScale = Vector3.one;
    //    return obj;
    //}

    //private static RoomController CreateRoomRoomController(PhysicalTopology topo, Transform parent)
    //{
    //    GameObject obj = CreateGameObject(topo.Name, parent);
    //    RoomController roomController = obj.AddComponent<RoomController>();
    //    roomController.SetTopoNode(topo);
    //    roomController.NodeObject = obj;
    //    roomController.angleFocus = new Vector2(60, 0);
    //    roomController.camDistance = 15;
    //    roomController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
    //    roomController.disRange = new Mogoson.CameraExtension.Range(2, 15);
    //    roomController.AreaSize = new Vector2(5, 5);
    //    return roomController;
    //}

    //private static RangeController CreateRangeController(PhysicalTopology topo, Transform parent)
    //{
    //    GameObject obj = CreateGameObject(topo.Name, parent);
    //    RangeController rangeController = obj.AddComponent<RangeController>();
    //    rangeController.SetTopoNode(topo);
    //    rangeController.NodeObject = obj;
    //    rangeController.angleFocus = new Vector2(60, 0);
    //    //是否根据厂区/楼层内，定不同的参数?
    //    rangeController.camDistance = 20;
    //    rangeController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
    //    rangeController.disRange = new Mogoson.CameraExtension.Range(2, 25);
    //    rangeController.AreaSize = new Vector2(5, 5);
    //    return rangeController;
    //}

    //private static T CreateDepNode<T>(PhysicalTopology topo, GameObject parent) where T : DepNode
    //{
    //    GameObject rangeT = CreateGameObject(topo.Name, parent.transform);
    //    T rangeController = rangeT.AddComponent<T>();
    //    rangeController.SetTopoNode(topo);
    //    rangeController.NodeObject = rangeT;
    //    //rangeController.angleFocus = new Vector2(60, 0);
    //    //rangeController.camDistance = 15;
    //    //rangeController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
    //    //rangeController.disRange = new Mogoson.CameraExtension.Range(2, 15);
    //    //rangeController.AreaSize = new Vector2(5, 5);
    //    return rangeController;
    //}

    ///// <summary>
    ///// 获取设备存放处的缩放值
    ///// </summary>
    ///// <param name="ParentLossyScale"></param>
    ///// <returns></returns>
    //private Vector3 GetContainerScale(Vector3 ParentLossyScale)
    //{
    //    float x = ParentLossyScale.x;
    //    float y = ParentLossyScale.y;
    //    float z = ParentLossyScale.z;
    //    if (x != 0) x = 1 / x;
    //    if (y != 0) y = 1 / y;
    //    if (z != 0) z = 1 / z;
    //    return new Vector3(x, y, z);
    //}
    //#endregion
    //#region 设备拓扑树点击响应部分
    //public void FocusNode(PhysicalTopology topoNode)
    //{
    //    if (topoNode == null)
    //    {
    //        Log.Alarm("FocusNode", "topoNode == null");
    //        return;
    //    }
    //    Log.Info("FocusNode:" + topoNode.Name);
    //    DepNode node = GetDepNodeByTopo(topoNode);
    //    if (node != null)
    //    {
    //        FocusNode(node);
    //    }
    //    else
    //    {
    //        Log.Alarm("未找到节点区域");
    //    }
    //}

    //private BuildingBox GetBuildingBox(DepNode node)
    //{
    //    if (node == null)
    //    {
    //        Debug.LogError("RoomFactory.GetBuildingBox node == null");
    //        return null;
    //    }
    //    BuildingController building = node.GetParentNode<BuildingController>();
    //    if (building != null)
    //    {
    //        BuildingBox box = building.gameObject.GetComponent<BuildingBox>();
    //        return box;
    //    }
    //    return null;
    //}


    //public void FocusNode(DepNode node, Action onDevCreateFinish = null, bool isSetSelectNode = true)
    //{
    //    DepNode currentNode = FactoryDepManager.currentDep;
    //    if (node == null)
    //    {
    //        Debug.LogError($"RoomFactory.FocusNode0 node == null!!!!! currentNode:{currentNode}");
    //        return;
    //    }

    //    if (currentNode != null)
    //    {
    //        if (currentNode.ParentNode != null && node.ParentNode != null)
    //        {
    //            Debug.LogError($"RoomFactory.FocusNode1 [currentNode:{currentNode.ParentNode.name}>{currentNode.name}] [newNode:{node.ParentNode.name}>{node.name}]");
    //        }
    //        else
    //        {
    //            Debug.LogError($"RoomFactory.FocusNode2 [currentNode:{currentNode.ParentNode}>{currentNode.name}] [newNode:{node.ParentNode}>{node.name}]");
    //        }
    //    }
    //    else
    //    {
    //        if (node.ParentNode != null)
    //        {
    //            Debug.LogError($"RoomFactory.FocusNode3 [currentNode:NULL] [newNode:{node.ParentNode.name}>{node.name}]");
    //        }
    //        else
    //        {
    //            Debug.LogError($"RoomFactory.FocusNode4 [currentNode:NULL] [newNode:NULL>{node.name}]");
    //        }
    //    }


    //    //if (!(FactoryDepManager.currentDep == node && IsFocusingDep))
    //    if (!(currentNode == node))
    //    {
    //        Debug.LogError($"RoomFactory.FocusNode_1 currentDep:{FactoryDepManager.currentDep} node:{node}");
    //        //if(currentNode.ParentNode == node.ParentNode)
    //        //{ 
    //        //}

    //        if (node is FactoryDepManager)
    //        {
    //            if(currentNode is BuildingController)
    //            {
    //                BuildingController building = currentNode as BuildingController;
    //                building.SetBuildingCollider(true);
    //                building.ShowFloors();
    //            }
    //            if(currentNode is FloorController)
    //            {
    //                FloorController floor = currentNode as FloorController;
    //                BuildingController building = floor.ParentNode as BuildingController;
    //                building.SetBuildingCollider(true);
    //                building.ShowFloors();
    //            }
    //        }
    //        else if (node is BuildingController && currentNode is FloorController)
    //        {
    //            BuildingController building = node as BuildingController;
    //            building.SetBuildingCollider(true);

    //            //FloorController floor = currentNode as FloorController;
    //            //floor.SetBuildingCollider(true);
    //        }

    //        if (currentNode is BuildingController)
    //        {
    //            BuildingController building = currentNode as BuildingController;

    //            if (FactoryDepManager.Instance.FactoryRoomContainer.transform == building.transform.parent)
    //            {
    //                RendererId rendererId = RendererId.GetRId(building);
    //                rendererId.RecoverParent();//之前的建筑放回到Factory里面
    //                Debug.LogError($"RoomFactory.FocusNode_2 currentDep:{FactoryDepManager.currentDep} node:{node}");
    //            }
    //            else
    //            {
    //                Debug.LogError($"RoomFactory.FocusNode_3 currentDep:{FactoryDepManager.currentDep} node:{node}");
    //            }
    //        }

    //        if(node is BuildingController) //cww_220608:不加上这个的话，切换楼层时会出现isActive=false > Factory.SetActive(true) > Factory.SetActive(false)，大量游戏物体显示后隐藏，会导致卡一下。
    //        {
    //            BuildingController building = node as BuildingController;
    //            building.ShowFloors();
    //            if (FactoryDepManager.Instance.FactoryRoomContainer.transform == building.transform.parent)
    //            {
    //                RendererId rendererId = RendererId.GetRId(building);
    //                rendererId.RecoverParent();
    //                Debug.LogError($"RoomFactory.FocusNode_4 currentDep:{FactoryDepManager.currentDep} node:{node}");
    //            }
    //            else
    //            {
    //                Debug.LogError($"RoomFactory.FocusNode_5 currentDep:{FactoryDepManager.currentDep} node:{node}");
    //            }

    //            FactoryDepManager depManager = FactoryDepManager.Instance;
    //            depManager.ShowFactory("Factory.FocusNode");
    //        }
    //        else
    //        {
    //            Debug.LogError($"RoomFactory.FocusNode_6 currentDep:{FactoryDepManager.currentDep} node:{node}");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError($"RoomFactory.FocusNode_7 FactoryDepManager.currentDep == node node:{node}");
    //    }

    //    BuildingController buildingControllerT = FactoryDepManager.Instance.GetParentBuildingController(node);
    //    InBuildingObjs inBuildingObjs = null;
    //    if (buildingControllerT)
    //    {
    //        //inBuildingObjs = node.GetComponentInParent<InBuildingObjs>();//这个获取组件方法在游戏对象Active为false时，无法获取父对象的组件
    //        inBuildingObjs = buildingControllerT.GetComponent<InBuildingObjs>();
    //        if(inBuildingObjs)inBuildingObjs.gameObject.SetActive(true);//不激活，无法执行携程
    //    }
    //    //InBuildingObjs inBuildingObjs = node.GetComponentInParent<InBuildingObjs>();

    //    if (inBuildingObjs)
    //    {
    //        StartCoroutine(inBuildingObjs.LoadInBuildingObjs(() =>
    //        {
    //            FocusNodeOP(node, onDevCreateFinish, isSetSelectNode);
    //        }));
    //    }
    //    else
    //    {
    //        FocusNodeOP(node, onDevCreateFinish, isSetSelectNode);
    //    }

    //    //FocusNodeOP(node, () =>
    //    //{
    //    //    LoadInBuildingObjs(node, onDevCreateFinish);
    //    //}, isSetSelectNode);
    //}

    //public void LoadInBuildingObjs(DepNode node, Action onDevCreateFinish)
    //{
    //    BuildingController buildingControllerT = FactoryDepManager.Instance.GetParentBuildingController(node);
    //    InBuildingObjs inBuildingObjs = null;
    //    if (buildingControllerT)
    //    {
    //        //inBuildingObjs = node.GetComponentInParent<InBuildingObjs>();//这个获取组件方法在游戏对象Active为false时，无法获取父对象的组件
    //        inBuildingObjs = buildingControllerT.GetComponent<InBuildingObjs>();
    //        inBuildingObjs.gameObject.SetActive(true);//不激活，无法执行携程
    //    }

    //    //InBuildingObjs inBuildingObjs = node.GetComponentInParent<InBuildingObjs>();//这个获取组件方法在游戏对象Active为false时，无法获取父对象的组件
    //    if (inBuildingObjs)
    //    {
    //        StartCoroutine(inBuildingObjs.LoadInBuildingObjs(() =>
    //        {
    //            if (onDevCreateFinish != null)
    //            {
    //                onDevCreateFinish();
    //            }
    //        }));
    //    }
    //}

    //private void FocusNodeOP(DepNode node, Action onDevCreateFinish, bool isSetSelectNode)
    //{
    //    try
    //    {
    //        if (node == null || node.TopoNode == null)
    //        {
    //            Debug.LogError("RoomFactory.FocusNode node == null");
    //            if (onDevCreateFinish != null)
    //            {
    //                onDevCreateFinish();//2019_05_14_cww:继续进行下去
    //            }
    //            return;
    //        }

    //        Log.Info("RoomFactory.FocusNode", string.Format("nodeId:{0},nodeName:{1}", node.NodeID, node.NodeName));

    //        BuildingBox box = GetBuildingBox(node);
    //        if (box)
    //        {
    //            Log.Info("RoomFactory.FocusNode", string.Format("box.LoadBuilding AssetName:{0},SceneName:{1}", box.AssetName, box.SceneName));
    //            box.LoadBuilding((nNode) =>
    //            {
    //                FocusNode(nNode, onDevCreateFinish, isSetSelectNode);//加载完建筑模型后继续原来的对焦工作
    //            }, true, node);
    //            return;
    //        }

    //        //if (node.TopoNode != null && node.TopoNode.Type == AreaTypes.范围) return;
    //        if (FactoryDepManager.currentDep == node && IsFocusingDep)
    //        {
    //            //处理拓扑树,快速单击两次的问题
    //            Debug.Log(string.Format("{0} is Focusing...", node.NodeName));
    //            return;
    //        }

    //        //if (LocationManager.Instance)
    //        //{
    //        //    LocationManager.Instance.HideCurrentPersonInfoUI();
    //        //}

    //        bool isFocusBreak = false;
    //        if (IsFocusingDep) isFocusBreak = true;
    //        IsFocusingDep = true;
    //        if (DevNode.CurrentFocusDev != null) DevNode.CurrentFocusDev.FocusOff(false);
    //        if (BIMModelInfo.currentFocusModel != null) BIMModelInfo.currentFocusModel.FocusOff();
    //        Log.Info(string.Format("FocusNode ID:{0},Name:{1},Type:{2}", node.NodeID, node.NodeName, node.GetType()));
    //        DepNode lastNodep = FactoryDepManager.currentDep;
    //        SceneEvents.OnDepNodeChangeStart(lastNodep, node);
    //        if (FactoryDepManager.currentDep == node)
    //        {
    //            node.FocusOn(() =>
    //            {
    //                IsFocusingDep = false;
    //                if (onDevCreateFinish != null) onDevCreateFinish();
    //            });
    //            //IsFocusingDep = false;
    //            //if (onDevCreateFinish != null) onDevCreateFinish();
    //            if (isFocusBreak) IsFocusingDep = true;
    //        }
    //        else
    //        {
    //            //FactoryDepManager manager = FactoryDepManager.Instance;
    //            DeselctLast(FactoryDepManager.currentDep, node);
    //            node.OpenDep(() =>
    //            {
    //                IsFocusingDep = false;
    //                if (onDevCreateFinish != null) onDevCreateFinish();
    //                SceneEvents.OnDepCreateCompleted(node);
    //            });
    //            if (isFocusBreak) IsFocusingDep = true;
    //        }

    //        if (isSetSelectNode)
    //        {
    //            //if (AreaDevTreePanel.Instance) AreaDevTreePanel.Instance.SetSelectNode(lastNodep, node);
    //            MessageCenter.SendMsgParams(MsgType.AreaDevTreePanelMsg.TypeName, MsgType.AreaDevTreePanelMsg.SetSelectNode, lastNodep, node);
    //            //if (PersonnelTreePanel.Instance) PersonnelTreePanel.Instance.areaDivideTree.Tree.AreaSelectNodeByType(node.NodeID);
    //            MessageCenter.SendMsg(MsgType.PersonnelTreePanelMsg.TypeName, MsgType.PersonnelTreePanelMsg.AreaSelectNodeById, node.NodeID);
    //            //if (node != FactoryDepManager.Instance) PersonnelTreeManage.Instance.areaDivideTree.Tree.AreaSelectNodeByType(node.NodeID);
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        Log.Error("RoomFactory.FocusNode", ex.ToString());
    //    }
    //}

    ///// <summary>
    ///// 聚焦区域节点，这里是用在聚焦人员时，切换到人员所在的区域节点时用的
    ///// </summary>
    ///// <param name="node"></param>
    ///// <param name="onDevCreateFinish"></param>
    //public void FocusNodeForFocusPerson(DepNode node, Action onDevCreateFinish = null, bool isSetSelectNode = true)
    //{
    //    if (node == null)
    //    {
    //        Debug.LogError("RoomFactory.FocusNodeForFocusPerson node == null");
    //        if (onDevCreateFinish != null)
    //        {
    //            onDevCreateFinish();
    //        }
    //        return;
    //    }
    //    BuildingBox box = GetBuildingBox(node);
    //    if (box)
    //    {
    //        box.LoadBuilding((nNode) =>
    //        {
    //            FocusNodeForFocusPerson(nNode, onDevCreateFinish, isSetSelectNode);//加载完建筑模型后继续原来的对焦工作
    //        }, true, node);
    //        return;
    //    }

    //    ////if (node.TopoNode != null && node.TopoNode.Type == AreaTypes.范围) return;
    //    //if (FactoryDepManager.currentDep == node && IsFocusingDep)
    //    //{
    //    //    //处理拓扑树,快速单击两次的问题
    //    //    Debug.Log(string.Format("{0} is Focusing...", node.NodeName));
    //    //    return;
    //    //}
    //    bool isFocusBreak = false;
    //    if (IsFocusingDep) isFocusBreak = true;
    //    IsFocusingDep = true;
    //    if (DevNode.CurrentFocusDev != null) DevNode.CurrentFocusDev.FocusOff(false);
    //    Log.Info(string.Format("FocusNode ID:{0},Name:{1},Type:{2}", node.NodeID, node.NodeName, node.GetType()));
    //    DepNode lastNodep = FactoryDepManager.currentDep;
    //    SceneEvents.OnDepNodeChangeStart(lastNodep, node);
    //    if (FactoryDepManager.currentDep == node)
    //    {
    //        //node.FocusOn(() =>
    //        //{
    //        //    IsFocusingDep = false;
    //        //    if (onDevCreateFinish != null) onDevCreateFinish();
    //        //});
    //        IsFocusingDep = false;
    //        if (onDevCreateFinish != null) onDevCreateFinish();
    //        if (isFocusBreak) IsFocusingDep = true;
    //    }
    //    else
    //    {
    //        //FactoryDepManager manager = FactoryDepManager.Instance;
    //        DeselctLast(FactoryDepManager.currentDep, node);
    //        node.OpenDep(() =>
    //        {
    //            IsFocusingDep = false;
    //            if (onDevCreateFinish != null) onDevCreateFinish();
    //            SceneEvents.OnDepCreateCompleted(node);
    //        }, false);
    //        if (isFocusBreak) IsFocusingDep = true;
    //    }

    //    //if (isSetSelectNode)
    //    //{
    //    //    if (TopoTreeManager.Instance) TopoTreeManager.Instance.SetSelectNode(lastNodep, node);
    //    //}
    //}

    ///// <summary>
    ///// 取消上一个区域的选中,无视角转换
    ///// </summary>
    ///// <param name="lastNode"></param>
    ///// <param name="currentNode"></param>
    //public void DeselctLast(DepNode lastNode, DepNode currentNode)
    //{
    //    if (lastNode == null) return;
    //    HighlightManage highlight = HighlightManage.Instance;
    //    if (highlight)
    //    {
    //        highlight.CancelHighLight();//取消当前区域,设备的高亮
    //    }
    //    lastNode.IsFocus = false;
    //    if (lastNode.NodeID != currentNode.NodeID)
    //    {
    //        lastNode.HideDep();
    //    }
    //}

    ///// <summary>
    ///// 无动画切换区域
    ///// </summary>
    //public void ChangeDepNodeNoTween()
    //{

    //    //FactoryDepManager.Instance.ShowOtherBuilding();
    //    DepNode lastDep = FactoryDepManager.currentDep;
    //    //lastDep.IsFocus = false;
    //    FactoryDepManager.currentDep = FactoryDepManager.Instance;
    //    RoomFactory.Instance.DeselctLast(lastDep, FactoryDepManager.Instance);
    //    SceneEvents.OnDepNodeChanged(lastDep, FactoryDepManager.Instance);
    //    FactoryDepManager.Instance.ShowOtherBuilding();
    //}
    //#endregion
    //    #region 创建设备部分

    //    //private Dictionary<int?, List<DevInfo>> DevCreateDic = new Dictionary<int?, List<DevInfo>>();
    //    /// <summary>
    //    /// 创建前，存储区域下所有设备
    //    /// </summary>
    //    private Dictionary<DepNode, List<DevInfo>> DepDevCreateDic = new Dictionary<DepNode, List<DevInfo>>();
    //    /// <summary>
    //    /// 区域下所有门禁信息
    //    /// </summary>
    //    private List<Dev_DoorAccess> DoorAccessList = new List<Dev_DoorAccess>();
    //    /// <summary>
    //    /// 设备创建完成回调
    //    /// </summary>
    //    private Action OnDevCreateAction;
    //    /// <summary>
    //    /// 当前创建设备的建筑（服务端获取数据的过程，切换区域）
    //    /// </summary>
    //    private DepNode currentFocusDep;
    //    private DateTime devStartTime;
    //    private DateTime recordTime;
    //    public void CreateDepDev(DepNode dep, Action onComplete = null)
    //    {
    //        //CreateDepDev(dep, DevSubsystemManage.IsRoamState, onComplete);
    //    }
    //    /// <summary>
    //    /// 创建设备
    //    /// </summary>
    //    /// <param name="dep">区域</param>
    //    /// <param name="isRoam">是否漫游模式</param>
    //    /// <param name="onComplete">设备创建完回调</param>
    //    public void CreateDepDev(DepNode dep, bool isRoam, Action onComplete = null)
    //    {
    //        //TimeTest.Start("CreateDepDev");
    //        Debug.Log("RoomFactory.CreateDepDev dep=" + dep);
    //        if (currentFocusDep == dep) return;
    //        currentFocusDep = dep;
    //        ResultText = "";
    //        OnDevCreateAction = onComplete;

    //        //Debug.LogError(string.Format("StartCreateDev {0}",dep));
    //        //ThreadManager.Run(() =>
    //        //{
    //        //    GetDevs(dep);
    //        //}, () =>
    //        //{
    //        //    CreateDevs(isRoam);
    //        //}, "LoadDevInfo...");

    //        GetDevs(dep, devs =>
    //         {
    //             CreateDevs(isRoam);
    //         });

    //    }

    //    private void GetDevs(DepNode dep, Action<List<DevInfo>> callback)
    //    {
    //        recordTime = DateTime.Now;
    //        List<DepNode> depList = new List<DepNode>();
    //        //CommunicationObject service = CommunicationObject.Instance;
    //        DevCount = 0;
    //        depList.Add(dep);
    //        //dep is BuildingController || 
    //        if (dep is FloorController)
    //        {
    //            List<DepNode> childList = GetChildNodes(dep);
    //            //去除VUE和设备信息
    //            childList.RemoveAll(i => i.TopoNode != null && (i.TopoNode.Type == Types.设备 || i.TopoNode.Type == Types.VuePath));
    //            if (childList != null && childList.Count != 0) depList.AddRange(childList);
    //        }
    //        GetDevInfo(depList, devs =>
    //         {
    //             ResultText += string.Format("GetDevs cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
    //             if (callback != null)
    //             {
    //                 callback(devs);
    //             }
    //         });//从服务端获取设备

    //    }

    //    private void CreateDevs(bool isRoam)
    //    {
    //        DepNode factoryDep = FactoryDepManager.currentDep;
    //        bool isRoomState = factoryDep is RoomController && factoryDep.ParentNode == currentFocusDep;
    //        if (isRoam || isRoomState || currentFocusDep == factoryDep)
    //        {
    //            //recordTime = DateTime.Now;
    //            CreateDepDev();//里面是携程
    //            //ResultText += string.Format("CreateDepDev cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
    //            //Debug.Log(ResultText);
    //        }
    //    }

    //    private string ResultText = "";
    //    /// <summary>
    //    /// 获取建筑下所有楼层
    //    /// </summary>
    //    /// <param name="building"></param>
    //    /// <returns></returns>
    //    private List<DepNode> GetChildNodes(DepNode building)
    //    {
    //        if (building == null || building.ChildNodes == null)
    //        {
    //            return null;
    //        }
    //        List<DepNode> depTempList = new List<DepNode>();
    //        foreach (DepNode child in building.ChildNodes)
    //        {
    //            if (child.IsDevCreate) continue;
    //            child.IsDevCreate = true;
    //            depTempList.Add(child);
    //            List<DepNode> childList = GetChildNodes(child);
    //            if (childList != null && childList.Count != 0) depTempList.AddRange(childList);
    //        }
    //        return depTempList;
    //    }
    //    /// <summary>
    //    /// 保存获取的设备信息
    //    /// </summary>
    //    /// <param name="parentID"></param>
    //    private void GetDevInfo(List<DepNode> deps, Action<List<DevInfo>> callback)
    //    {
    //        try
    //        {
    //            CommunicationObject service = CommunicationObject.Instance;
    //            if (service)
    //            {
    //                RecordTime = DateTime.Now;
    //                List<int> pidList = GetPidList(deps);
    //                service.GetDevInfoByParentAsync(pidList, devInfoList =>
    //                {
    //                    List<DevInfo> devInfos = RemoveRepeateDev(devInfoList);
    //                    int count = devInfos == null ? 0 : devInfos.Count;
    //                    ResultText += string.Format("Get dep info, length:{0} cost :{1}ms\n", count, (DateTime.Now - RecordTime).TotalMilliseconds);
    //                    recordTime = DateTime.Now;
    //                    SaveDepDevInfoInCreating(deps, devInfos);
    //                    //GetDoorAccessInfo(pidList);
    //                    ResultText += string.Format("Get DoorAccessInfo cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
    //                    //if (callback != null)
    //                    //{
    //                    //    callback(devInfos);
    //                    //}
    //                    GetDoorAccessInfo(pidList,()=>
    //                    {
    //                        if (callback != null)
    //                        {
    //                            callback(devInfos);
    //                        }
    //                    });
    //                });
    //            }
    //            else
    //            {
    //                Debug.LogError("RoomFactory.GetDevInfo CommunicationObject.Instance==null");
    //                if (callback != null)
    //                {
    //                    callback(null);
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Log.Error("RoomFactory.GetDevInfo.Exception:" + e.ToString());
    //            if (callback != null)
    //            {
    //                callback(null);
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 移除重复数据
    //    /// </summary>
    //    /// <param name="devListTemp"></param>
    //    /// <returns></returns>
    //    private List<DevInfo> RemoveRepeateDev(List<DevInfo> devListTemp)
    //    {
    //        if (devListTemp == null) return null;
    //        int repeatDevCount = 0;
    //        Dictionary<string, DevInfo> devDicNoRepeat = new Dictionary<string, DevInfo>();
    //        List<DevInfo> devList = new List<DevInfo>();
    //        foreach (var item in devListTemp)
    //        {
    //            if (TypeCodeHelper.IsKKSMonitor(item.TypeCode.ToString()) && string.IsNullOrEmpty(item.KKSCode)) continue;//未填充kks的测点，不创建
    //            if (string.IsNullOrEmpty(item.DevID)) devList.Add(item);
    //            else if (!devDicNoRepeat.ContainsKey(item.DevID)) devDicNoRepeat.Add(item.DevID, item);
    //            else
    //            {
    //                if (devDicNoRepeat[item.DevID].Id > item.Id) devDicNoRepeat[item.DevID] = item;
    //                repeatDevCount++;
    //            }
    //        }
    //        Debug.LogError("RemoveRepeatDev,RepeatDevCount:" + repeatDevCount);
    //        List<DevInfo> listT = devDicNoRepeat.Values.ToList();
    //        devList.AddRange(listT);
    //        return devList;
    //    }
    //    private void GetDoorAccessInfo(List<int> pidList,Action action=null)
    //    {
    //        CommunicationObject service = CommunicationObject.Instance;
    //        if (service)
    //        {
    //            //List<Dev_DoorAccess> doorAccesses = service.GetDoorAccessInfoByParent(pidList);
    //            //SaveDoorAccessInfo(doorAccesses);

    //            service.GetDoorAccessInfoByParentAsync(pidList,(doorAccesses)=>
    //            {
    //                SaveDoorAccessInfo(doorAccesses);
    //                if(action != null) action();
    //            });
    //        }
    //    }
    //    /// <summary>
    //    /// 获取Pid(设备所属区域)列表
    //    /// </summary>
    //    /// <param name="deps"></param>
    //    /// <returns></returns>
    //    private List<int> GetPidList(List<DepNode> deps)
    //    {
    //        List<int> pidList = new List<int>();
    //        foreach (var dep in deps)
    //        {
    //            if (!pidList.Contains(dep.NodeID)) pidList.Add(dep.NodeID);
    //        }
    //        return pidList;
    //    }
    //    /// <summary>
    //    /// 保存区域下门禁信息
    //    /// </summary>
    //    /// <param name="doorAccess"></param>
    //    private void SaveDoorAccessInfo(List<Dev_DoorAccess> doorAccess)
    //    {
    //        DoorAccessList.Clear();
    //        if (doorAccess != null && doorAccess.Count != 0)
    //        {
    //            DoorAccessList.AddRange(doorAccess);
    //        }
    //    }
    //    /// <summary>
    //    /// 保存区域下设备信息
    //    /// </summary>
    //    /// <param name="dep"></param>
    //    /// <param name="devInfos"></param>
    //    private void SaveDepDevInfoInCreating(List<DepNode> depList, List<DevInfo> devInfos)
    //    {
    //        DepDevCreateDic.Clear();
    //        if (devInfos != null && devInfos.Count != 0)
    //        {
    //            foreach (var dep in depList)
    //            {
    //                List<DevInfo> devs = devInfos.FindAll(i => i.ParentId == dep.NodeID);
    //                if (devs != null && devs.Count != 0)
    //                {
    //                    DepDevCreateDic.Add(dep, devs);
    //                    DevCount += devs.Count;
    //                }
    //            }

    //        }
    //    }
    //    /// <summary>
    //    /// 创建区域下设备
    //    /// </summary>
    //    private void CreateDepDev()
    //    {
    //        if (DepDevCreateDic != null && DepDevCreateDic.Count != 0)
    //        {
    //            devStartTime = DateTime.Now;//开始创建设备
    //            CurrentCreateIndex = 0;
    //            foreach (var item in DepDevCreateDic.Keys)
    //            {
    //                int id = item.NodeID;
    //                DevType devType = DevType.DepDev;
    //                GameObject devContainer = GetDepDevContainer(item, ref devType);
    //                //StartCoroutine(LoadDevsCorutine(item, devContainer, devType));//开始携程,一个区域开始一个携程？，递归方式
    //                StartCoroutine(LoadDevsCorutine2(item, devContainer, devType));//开始携程,一个区域开始一个携程？，循环方式
    //            }
    //        }
    //        else
    //        {
    //            Log.Error("RoomFactory.CreateDpeDev 区域下没有设备");
    //            FireDevCreateAction();
    //        }
    //    }

    //    private void FireDevCreateAction()
    //    {
    //        if (OnDevCreateAction != null)
    //        {
    //            OnDevCreateAction();
    //        }

    //        //TimeTest.Stop("CreateDepDev", "FireDevCreateAction");
    //    }

    //    /// <summary>
    //    /// 获取存放设备的物体
    //    /// </summary>
    //    /// <param name="depNode"></param>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    private GameObject GetDepDevContainer(DepNode depNode, ref DevType type)
    //    {
    //        if (depNode as FloorController)
    //        {
    //            FloorController floor = depNode as FloorController;
    //            type = DevType.RoomDev;
    //            return floor.RoomDevContainer;
    //        }
    //        else if (depNode as RoomController)
    //        {
    //            RoomController room = depNode as RoomController;
    //            type = DevType.RoomDev;
    //            return room.RoomDevContainer;
    //        }else if(depNode is BuildingController)
    //        {
    //            BuildingController building = depNode as BuildingController;
    //            GameObject container = building.DevContainer;
    //            type = DevType.RoomDev;
    //            return container;
    //        }
    //        else
    //        {
    //            type = DevType.DepDev;
    //            return FactoryDepManager.Instance.FactoryDevContainer;
    //        }
    //    }
    //    /// <summary>
    //    /// 设备数量
    //    /// </summary>
    //    private float DevCount;
    //    /// <summary>
    //    /// 当前创建设备下标
    //    /// </summary>
    //    private float CurrentCreateIndex;

    //    private DateTime RecordTime;



    //    //private WaitForSeconds waitTime = new WaitForSeconds(0.1f);
    //    IEnumerator LoadDevsCorutine(DepNode dep, GameObject container, DevType type)
    //    {
    //        //yield return null;
    //        if (SystemSettingHelper.deviceSetting.NotLoadAllDev == false)
    //        {
    //            List<DevInfo> devList;
    //            DepDevCreateDic.TryGetValue(dep, out devList);
    //            if (devList != null && devList.Count != 0)
    //            {
    //                //float percent =1- devList.Count / DevCount;  
    //                CurrentCreateIndex++;
    //                float percent = CurrentCreateIndex / DevCount;
    //                //Debug.Log(string.Format("当前创建设备Index:{0}  总设备数:{1}  Percent:{2}",CurrentCreateIndex,DevCount,percent));
    //                ProgressbarLoad.Instance.Show(percent);
    //                DevInfo dev = devList[devList.Count - 1];
    //                //Debug.Log(string.Format("创建设备:{0} 剩余设备:{1}",dev.Name,devList.Count));
    //                devList.Remove(dev);
    //                //StartCoroutine(LoadSingleDevCorutine(dev, container, type, dep, obj =>
    //                //{
    //                //    StartCoroutine(LoadDevsCorutine(dep, container, type));
    //                //}));
    //                //yield return LoadSingleDevCorutine(dev, container, type, dep, obj =>
    //                //{
    //                //    StartCoroutine(LoadDevsCorutine(dep, container, type));
    //                //});
    //                yield return LoadSingleDevCorutine(dev, container, type, dep, null);//创建设备
    //                yield return LoadDevsCorutine(dep, container, type);//递推调用，继续区域内下一个设备。
    //            }
    //            else
    //            {
    //                DepDevCreateDic.Remove(dep);
    //                if (DepDevCreateDic.Count == 0)
    //                {
    //                    FireDevCreateAction();
    //                    ProgressbarLoad.Instance.Hide();
    //                    // SystemModeTweenManage.Instance.StartTweener();

    //                    ResultText += string.Format("CreateDepDev All cost:{0}ms count:{1} \n", (DateTime.Now - devStartTime).TotalMilliseconds, DevCount);
    //                    Debug.Log(ResultText);//结束创建
    //                }
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 图片路径和流的类
    //    /// </summary>
    //    public class UrlAndStream
    //    {
    //        public string name;//文件名称
    //        public string url;//图片url
    //        public Stream stream;//流
    //        public List<byte> byteList;//字节
    //        public GameObject obj;//模型
    //        public DevInfo devInfo;//设备信息
    //        public GameObject container;//
    //        public DevType devType;//
    //        public DepNode depNode;//
    //    }
    //    private HTTPRequest request;
    //    public List<UrlAndStream> textureStreamList;//贴图文件路径列表
    //    public List<UrlAndStream> modelStreamList;//模型文件路径列表
    //    public List<string> fileNameList;//模型文件名称列表

    //    private Dictionary<int, DevTypeModel> typeDic;
    //    private List<int> devIdGettingList;//正在获取的设备信息
    //    IEnumerator LoadDevsCorutine2(DepNode dep, GameObject container, DevType type)
    //    {
    //        if (SystemSettingHelper.deviceSetting.NotLoadAllDev == false)
    //        {
    //            List<DevInfo> devList=new List<DevInfo>();
    //            List<DevInfo> devDic;
    //            DepDevCreateDic.TryGetValue(dep, out devDic);
    //            //直接用字典中的value,会导致value信息被修改，取出后传给其他list做修改
    //            if (devDic != null) devList.AddRange(devDic);

    //            if (devList != null && devList.Count != 0)
    //            {
    //                //改动方式1.整理成循环方式，逻辑上更加清晰，效果和性能不变
    //                //for (int i = 0; i < devList.Count; i++)
    //                //{
    //                //    float percent = (i + 1) / DevCount;
    //                //    ProgressbarLoad.Instance.Show(percent);
    //                //    var dev = devList[i];
    //                //    yield return LoadSingleDevCorutine(dev, container, type, dep, null);//创建一个设备
    //                //}

    //                //改动方式2.先加载模型，在创建设备。模型加载进度成为进度条进度，设备创建基本不耗时。
    //                recordTime = DateTime.Now;
    //                List<string> models = new List<string>();
    //                for (int i = 0; i < devList.Count; i++)//循环创建设备
    //                {
    //                    var dev = devList[i];
    //                    if (string.IsNullOrEmpty(dev.ModelName)) continue;
    //                    if (!models.Contains(dev.ModelName) && !TypeCodeHelper.IsStaticDev(dev.TypeCode.ToString())
    //                        && dev.TypeCode != TypeCodeHelper.KKSMonitorTypeCode)
    //                    {
    //                        models.Add(dev.ModelName);
    //                    }
    //                }

    //                yield return AssetbundleGet.LoadCommonModels(models, true);
    //                ResultText += string.Format("[RoomFactory.LoadDevsCorutine2] GetModels cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);

    //                recordTime = DateTime.Now;
    //                //Debug.LogError("[RoomFactory.LoadDevsCorutine2] DevListCount:" + devList.Count);
    //                textureStreamList = new List<UrlAndStream>();
    //                modelStreamList = new List<UrlAndStream>();
    //                fileNameList = new List<string>();
    //                isLoadModels = true;
    //                loadModelCount = 0;
    //                if (typeDic == null) typeDic = new Dictionary<int, DevTypeModel>();
    //                if (devIdGettingList == null) devIdGettingList = new List<int>();
    //                for (int i = 0; i < devList.Count; i++)//循环创建设备
    //                {
    //                    DevInfo devInfoT = devList[i];
    //                    if (devInfoT == null) continue;
    //                    try
    //                    {
    //                        //float percent = (i + 1) / devList.Count;
    //                        //ProgressbarLoad.Instance.Show(percent);//就是加了进度条也不会出来
    //                        //Debug.LogErrorFormat("[RoomFactory.LoadDevsCorutine2] Index:{0} DevName:{1}", i, devList[i].Name);                       
    //                        if (TypeCodeHelper.IsStaticDev(devInfoT.TypeCode.ToString()))
    //                        {
    //                            //Debug.LogError("[RoomFactory.LoadDevsCorutine2] CreateStaticDev:" + dev.ModelName);
    //                            CreateStaticDev(devInfoT, dep, null);
    //                        }
    //                        else
    //                        {
    //                            if (TypeCodeHelper.IsFireFightDevType(devInfoT.TypeCode.ToString()))
    //                            {
    //                                continue;
    //                            }

    //                            if (typeDic.ContainsKey(devInfoT.TypeCode))
    //                            {
    //                                LoadModel(typeDic[devInfoT.TypeCode], devInfoT, dep, container, type);
    //                                DepDevCreateDic[dep].Remove(devInfoT);
    //                            }
    //                            else
    //                            {
    //                                devIdGettingList.Add(devInfoT.Id);
    //                                CommunicationObject.Instance.GetModelInfoByTypeCodeAsync(long.Parse(devInfoT.TypeCode.ToString()), result =>
    //                                {
    //                                    if (!typeDic.ContainsKey(devInfoT.TypeCode))
    //                                    {
    //                                        typeDic.Add(devInfoT.TypeCode, result);
    //                                    }
    //                                    devIdGettingList.Remove(devInfoT.Id);
    //                                    LoadModel(result, devInfoT, dep, container, type);

    //                                    if (devIdGettingList.Count == 0)
    //                                    {
    //                                        CompleteDevCreate();
    //                                    }
    //                                });
    //                            }
    //                        }
    //                        //yield return null;//这个加上的话，进度条才会出来，但是没必要。见后面总结
    //                    }catch(Exception e)
    //                    {
    //                        if (devInfoT != null && devIdGettingList.Contains(devInfoT.Id)) devIdGettingList.Remove(devInfoT.Id);
    //                        Debug.LogError("Roomfactory.CreateDev.Exception:"+e.ToString());
    //                    }
    //                } 
    //                ResultText += string.Format("[RoomFactory.LoadDevsCorutine2] CreatDevs cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
    //                /*
    //加载厂区内的设备的耗时：基本都是加载设备asset占用时间的，可以通过提前加载模型，和把常用模型放到resource中优化。
    //Get dep info, length:142 cost :129.1248ms
    //Get DoorAccessInfo cost:35.5331ms 
    //GetDevs cost:35.5331ms 
    //GetModels cost:2655.0736ms  //加载了两个设备Asset模型 摄像头和基站
    //CreatDevs cost:71.5566ms  //创建了142个设备模型
    //CreateDepDev All cost:2729.1469ms count:142 
    //                 */
    //            }
    //            DepDevCreateDic.Remove(dep);//该区域创建好了
    //            CompleteDevCreate();
    //        }
    //    }

    //    private void CompleteDevCreate()
    //    {
    //        //当区域设备都循环完成，还会有部分设备回调未触发。只有当区域循环完成，且设备信息也获取完成，才触发创建完成回调
    //        if (DepDevCreateDic.Count == 0&&(devIdGettingList==null||devIdGettingList.Count==0))//全部区域的全部设备都创建好了
    //        {
    //            FireDevCreateAction();
    //            ProgressbarLoad.Instance.Hide();
    //            ResultText += string.Format("[RoomFactory.LoadDevsCorutine2] CreateDepDev All cost:{0}ms count:{1} \n", (DateTime.Now - devStartTime).TotalMilliseconds, DevCount);
    //            Debug.Log(ResultText);//结束创建
    //        }
    //    }


    //    private void LoadModel(DevTypeModel result, DevInfo dev, DepNode dep, GameObject container, DevType type)
    //    {
    //        if (result != null && result.flag == "1")
    //        {
    //            Load3DModel(result, dev, container, type, dep);
    //        }
    //        else
    //        {
    //            if (dev.TypeCode == TypeCodeHelper.KKSMonitorTypeCode)
    //            {
    //                GameObject modelT = GameObject.CreatePrimitive((PrimitiveType)Enum.Parse(typeof(PrimitiveType), dev.ModelName));
    //                CreateKKSModel(dev, container, type, dep, modelT);//测点无需再创建一次物体
    //            }
    //            else
    //            {
    //                GameObject modelT = ModelIndex.Instance.Get(dev.ModelName);
    //                CreateDevObject(dev, container, type, dep, modelT);//创建设备的函数
    //            }
    //        }
    //    }

    //    public void Load3DModel(DevTypeModel resultT, DevInfo devT, GameObject containerT, DevType typeT, DepNode depT)
    //    {
    //        if (RenderStreaming.Instance && RenderStreaming.Instance.IsConnectServer() == false)
    //        {
    //            //Debug.LogError("RenderStreaming.Instance.IsConnected():" + RenderStreaming.Instance.IsConnectServer());
    //            Debug.LogError("未连接云渲染服务器，无法获取模型文件");
    //            return;
    //        }
    //        string modelPath = resultT.itemName + "/" + resultT.className + "/" + resultT.typeName;
    //        StringArg stringArg = new StringArg(modelPath);
    //        CommunicationObject.Instance.GetModelInfoByModelNameAsync(stringArg, modelInfo =>
    //        {
    //            if (modelInfo != null)
    //            {
    //                if (modelInfo.fileInfoList.Count != 0)
    //                {
    //                    foreach (var fileInfo in modelInfo.fileInfoList)
    //                    {
    //                        if (fileInfo.FileName == (resultT.typeName + ".fbx") || fileInfo.FileName == (resultT.typeName + ".FBX") || fileInfo.ExtensionName == ".jpg" || fileInfo.ExtensionName == ".png" || fileInfo.ExtensionName == ".jpeg")
    //                        {
    //                            //string fileUrl = "http://127.0.0.1:8019/Exe/upload/动环设备/KVM设备/墙/墙.FBX";
    //                            string fileName = fileNameList.Find(list => list == fileInfo.FileName);
    //                            if (string.IsNullOrEmpty(fileName) || fileName.Contains(".fbx") || fileName.Contains(".FBX"))
    //                            {
    //                                string name = fileInfo.FileName;
    //                                fileNameList.Add(name);
    //                                request = new HTTPRequest(new System.Uri(@fileInfo.FileDownloadDirctoryURL), (reqT, respT) =>
    //                                {
    //                                    try
    //                                    {
    //                                        string respResultT = reqT != null ? string.Format("{0}{1}", respT.IsSuccess, respT.IsStreamingFinished) : "null";
    //                                        //当流传输未完全结束，也会执行回调.(满足两个条件，才算完成)
    //                                        if (respT.IsSuccess)
    //                                        {
    //                                            if (respT.IsStreamingFinished)
    //                                            {
    //                                                List<byte[]> fragmentsT = respT.GetStreamedFragments();
    //                                                if (fragmentsT != null)
    //                                                {
    //                                                    List<byte> mlistT = new List<byte>();
    //                                                    foreach (var item in fragmentsT)
    //                                                    {
    //                                                        mlistT.AddRange(item);
    //                                                    }
    //                                                    Stream stream = new MemoryStream(mlistT.ToArray());
    //                                                    UrlAndStream urlStreamT = new UrlAndStream();
    //                                                    urlStreamT.name = fileInfo.FileName;
    //                                                    urlStreamT.url = fileInfo.FileDownloadDirctoryURL;
    //                                                    urlStreamT.stream = stream;
    //                                                    urlStreamT.byteList = mlistT;
    //                                                    urlStreamT.obj = null;
    //                                                    urlStreamT.devInfo = devT;
    //                                                    urlStreamT.container = containerT;
    //                                                    urlStreamT.devType = typeT;
    //                                                    urlStreamT.depNode = depT;
    //                                                    if (name.Contains(".fbx") || name.Contains(".FBX"))
    //                                                    {
    //                                                        //Debug.LogError("模型stream:" + fileInfo.FileName + "+" + urlStreamT.stream.Length);
    //                                                        modelStreamList.Add(urlStreamT);
    //                                                    }
    //                                                    else
    //                                                    {
    //                                                        //Debug.LogError("贴图stream:" + fileInfo.FileName + ":" + urlStreamT.stream.Length);
    //                                                        textureStreamList.Add(urlStreamT);
    //                                                    }
    //                                                }
    //                                            }
    //                                        }
    //                                        else
    //                                        {
    //                                            var status = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", respT.StatusCode, respT.Message, respT.DataAsText);
    //                                            Debug.LogError(status);
    //                                            request = null;
    //                                        }
    //                                    }
    //                                    catch (Exception e)
    //                                    {
    //                                        string error = e.ToString();
    //                                        Log.Error("Download Error:" + error);
    //                                    }
    //                                });
    //                                request.UseStreaming = true;
    //                                request.StreamFragmentSize = 1 * 1024 * 1024;// 1 megabyte 
    //                                request.DisableCache = true;// already saving to a file, so turn off caching 
    //                                request.OnProgress = OnLoadProgress;
    //                                request.EnableTimoutForStreaming = true;
    //                                request.Send();
    //                            }
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    Debug.LogError("模型文件不存在！");
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogError("模型文件不存在！");
    //            }
    //        });
    //    }
    //    void OnLoadProgress(HTTPRequest originalRequest, long downloaded, long downloadLength)
    //    {
    //        float progressPercent = (downloaded / (float)downloadLength) * 100.0f;
    //        Log.Info(request.Uri + " Download: " + progressPercent.ToString("F2") + "%");
    //    }
    //    public bool isLoadModels;//是否加载模型
    //    public int loadModelCount = 0;//加载模型计数
    //    void Update()
    //    {
    //        if (isLoadModels && fileNameList.Count != 0 && (fileNameList.Count == (modelStreamList.Count + textureStreamList.Count)))
    //        {
    //            StartCreateObjInfo(loadModelCount);
    //            isLoadModels = false;
    //        }
    //    }
    //    /// <summary>
    //    /// 新加载模型方式的模型信息
    //    /// </summary>
    //    public void StartCreateObjInfo(int num)
    //    {
    //        if (modelStreamList != null && modelStreamList.Count != 0)
    //        {
    //            loadModelCount++;
    //            var objInfo = modelStreamList[num];
    //            Debug.LogError("objInfo.name:" + objInfo.name);
    //            AssetLoader.LoadModelFromStream(objInfo.stream, objInfo.url, null, onLoadComplete =>
    //            {
    //                if (onLoadComplete.RootGameObject != null)
    //                {
    //                    CreateObj(objInfo.devInfo, objInfo.container, objInfo.devType, objInfo.depNode, onLoadComplete.RootGameObject);
    //                    //Debug.LogError("onLoadComplete:" + onLoadComplete.RootGameObject.name);
    //                }
    //            }, materialLoad =>
    //            {
    //                if (materialLoad.LoadedMaterials != null)
    //                {
    //                    foreach (var item in materialLoad.LoadedMaterials.Values)
    //                    {
    //                        var data = textureStreamList.Find(infoT => infoT.url.Contains(item.name));
    //                        //Debug.LogError("item.name:" + item.name + " item.url:" + data.url);
    //                        if (data != null)
    //                        {
    //                            Texture2D tex = new Texture2D(100, 100);
    //                            tex.LoadImage(data.byteList.ToArray());
    //                            item.mainTexture = tex;
    //                        }
    //                    }
    //                }
    //            });
    //            //AssetLoader.LoadModelFromStream(stream, "E:/LocationSystem/bin/Exe/40_消防水池/40_消防水池外壳.fbx");
    //            //AssetLoaderZip.LoadModelFromZipStream(stream,null,null,null);    
    //        }
    //    }
    //    /// <summary>
    //    /// 新加载模型方式的模型创建
    //    /// </summary>
    //    private GameObject CreateObj(DevInfo dev, GameObject container, DevType type, DepNode dep, GameObject modelT)
    //    {
    //        if (modelT == null)
    //        {
    //            Debug.LogError(string.Format("{0} info is null,modelName:{1}", dev.Name, dev.ModelName));
    //            return null;
    //        }
    //        GameObject o = modelT;
    //        o.transform.parent = container.transform;
    //        o.transform.name = dev.Name;
    //        o.AddCollider();
    //        AddDevController(o, dev, type, dep);
    //        SetDevPos(o, dev.Pos);
    //        o.SetActive(true);
    //        o.layer = LayerMask.NameToLayer("DepDevice");
    //        return o;
    //    }
    //    IEnumerator LoadSingleDevCorutine(DevInfo dev, GameObject container, DevType type, DepNode dep, Action<GameObject> onComplete)
    //    {
    //        DevNode devCreate = GetCreateDevById(dev.DevID, dep.NodeID);
    //        if (string.IsNullOrEmpty(dev.ModelName) || devCreate != null)
    //        {
    //            Debug.Log(string.Format("设备：{0} 模型名称不存在,model:{1}", dev.Name, dev.ModelName));
    //            if (onComplete != null) onComplete(null);
    //        }
    //        else
    //        {
    //            if (TypeCodeHelper.IsStaticDev(dev.TypeCode.ToString()))
    //            {
    //                CreateStaticDev(dev, dep, onComplete);
    //            }
    //            else
    //            {
    //                GameObject modelT = ModelIndex.Instance.Get(dev.ModelName);
    //                if (modelT != null)
    //                {
    //                    GameObject objInit = CreateDevObject(dev, container, type, dep, modelT);//提取创建设备的函数
    //                    if (onComplete != null) onComplete(objInit);
    //                }
    //                else
    //                {
    //                    yield return AssetBundleHelper.LoadAssetObject("Devices", dev.ModelName, AssetbundleGetSuffixalName.prefab, obj =>
    //                    {
    //                        if (obj == null)
    //                        {
    //                            Debug.LogError("获取不到模型:" + dev.ModelName);
    //                            //StartCoroutine(LoadDevsCorutine(dep, container, type));//这里不需要
    //                            if (onComplete != null) onComplete(null);
    //                            return;
    //                        }
    //                        else
    //                        {
    //                            GameObject g = obj as GameObject;
    //                            ModelIndex.Instance.Add(g, dev.ModelName); //添加到缓存中
    //                            CreateDevObject(dev, container, type, dep, g);//提取创建设备的函数
    //                        }
    //                    }); //内部也是LoadAssetObject
    //                }

    //                //yield return AssetbundleGet.Instance.GetObjFromCatch(dev.ModelName, AssetbundleGetSuffixalName.prefab, obj =>
    //                //{
    //                //    CreateDevObjectEx(dev, container, type, dep, obj, onComplete);//提取创建设备的函数
    //                //});
    //            }
    //        }
    //    }

    //    private void CreateDevObjectEx(DevInfo dev, GameObject container, DevType type, DepNode dep, UnityEngine.Object obj, Action<GameObject> onComplete)
    //    {
    //        if (obj == null)
    //        {
    //            Debug.LogError("拖动获取不到模型:" + dev.ModelName);
    //            //StartCoroutine(LoadDevsCorutine(dep, container, type));//这里不需要
    //            if (onComplete != null) onComplete(null);
    //            return;
    //        }
    //        else
    //        {
    //            GameObject g = obj as GameObject;
    //            ModelIndex.Instance.Add(g, dev.ModelName); //添加到缓存中
    //            GameObject objInit = CreateDevObject(dev, container, type, dep, g);//提取创建设备的函数
    //            if (onComplete != null) onComplete(objInit);
    //        }
    //    }

    //    private GameObject CreateKKSModel(DevInfo dev, GameObject container, DevType type, DepNode dep, GameObject o)
    //    {        
    //        o.transform.parent = container.transform;
    //        o.transform.name = dev.Name;
    //        o.AddCollider();
    //        AddDevController(o, dev, type, dep);
    //        SetDevPos(o, dev.Pos);
    //        o.SetActive(true);
    //        o.layer = LayerMask.NameToLayer("DepDevice");
    //        return o;
    //    }

    //    private GameObject CreateDevObject(DevInfo dev, GameObject container, DevType type, DepNode dep, GameObject modelT)
    //    {
    //        if (modelT == null)
    //        {
    //            Debug.LogError(string.Format("{0} info is null,modelName:{1}", dev.Name, dev.ModelName));
    //            return null;
    //        }
    //        GameObject o = Instantiate(modelT);
    //        o.transform.parent = container.transform;
    //        o.transform.name = dev.Name;
    //        o.AddCollider();
    //        AddDevController(o, dev, type, dep);
    //        SetDevPos(o, dev.Pos);
    //        o.SetActive(true);
    //        o.layer = LayerMask.NameToLayer("DepDevice");
    //        return o;
    //    }

    //    /// <summary>
    //    /// 创建静态设备
    //    /// </summary>
    //    /// <param name="dev"></param>
    //    /// <param name="parnetDep"></param>
    //    /// <param name="onComplete"></param>
    //    private void CreateStaticDev(DevInfo dev, DepNode parnetDep, Action<GameObject> onComplete)
    //    {
    //        Debug.LogError($"CreateStaticDev [dev id:{dev.Id} devId:{dev.DevID} name:{dev.Name} model:{dev.ModelName} ]");

    //        if (dev.ModelName == BimNodeHelper_PhysicalTopology.BIMModelName)
    //        {
    //            ModelSystemTreePanel.FocusBimModel(dev, onComplete);
    //            return;
    //        }

    //        if (staticDevInfos.Find(devT => devT != null && devT.Id == dev.Id) == null)//改用Dic,主键换成int?
    //        {
    //            staticDevInfos.Add(dev);
    //        }
    //        DevNode staticDevT = StaticDevList.Find(i => i.devName.ToLower() == dev.ModelName.ToLower());
    //        if (staticDevT != null)
    //        {
    //            SaveDepDevInfo(parnetDep, staticDevT, dev);
    //            staticDevT.CreateFollowUI();
    //            if (onComplete != null) onComplete(staticDevT.gameObject);
    //        }
    //        else
    //        {
    //            if (onComplete != null) onComplete(null);
    //        }
    //    }
    //    /// <summary>
    //    /// 通过设备信息，创建单个设备
    //    /// </summary>
    //    /// <param name="devInfo"></param>
    //    /// <param name="OnSingleDevCreate"></param>
    //    private void CreateDevByDevId(DevInfo devInfo, Action<DevNode> OnSingleDevCreate)
    //    {
    //        if (devInfo == null)
    //        {
    //            OnSingleDevCreate(null);
    //            Debug.LogError("ID为[" + devInfo.DevID + "]的设备找不到");
    //            return;
    //        }
    //        DepNode dep = GetDepNodeById((int)devInfo.ParentId);
    //        if (dep == null)
    //        {
    //            Debug.LogError("DevParentId not find:" + devInfo.ParentId);
    //            //if (OnDevCreateAction != null) OnSingleDevCreate(null);

    //            Debug.LogError($"RoomFactory.CreateDevByDevId_2 dep==null【devInfo id:{devInfo.Id} pid:{devInfo.ParentId} name:{devInfo.Name} model:{devInfo.ModelName} DevID:{devInfo.DevID}】");
    //            if (devInfo.ModelName == BimNodeHelper_PhysicalTopology.BIMModelName)
    //            {
    //                ModelSystemTreePanel.FocusBimModel(devInfo, (obj) =>
    //                {
    //                    DevNode devNode = null;
    //                    if (obj != null)
    //                    {
    //                        devNode = obj.AddMissingComponent<DevNode>();
    //                        devNode.devName = devInfo.Name;
    //                        devNode.DevId = devInfo.DevID;
    //                        devNode.Info = devInfo;
    //                        devNode.Id = devInfo.Id;
    //                    }
    //                    if (OnSingleDevCreate != null) OnSingleDevCreate(devNode);
    //                });
    //                return;
    //            }
    //            if (OnSingleDevCreate != null) OnSingleDevCreate(null);
    //            return;//从这里返回，设备定位就相当与没有反应了
    //        }
    //        if (dep && !AutoFoam.Instance.isAutoFoam)
    //        {
    //            Debug.LogError($"RoomFactory.CreateDevByDevId_1 【devInfo id:{devInfo.Id} pid:{devInfo.ParentId} name:{devInfo.Name} model:{devInfo.ModelName} DevID:{devInfo.DevID}】 dep:{dep.NodeName}");
    //            if (devInfo.ModelName == BimNodeHelper_PhysicalTopology.BIMModelName)
    //            {
    //                ModelSystemTreePanel.FocusBimModel(devInfo, (obj)=>
    //                {
    //                    DevNode devNode = null;
    //                    if (obj != null)
    //                    {
    //                        devNode = obj.AddMissingComponent<DevNode>();
    //                        devNode.devName = devInfo.Name;
    //                        devNode.DevId = devInfo.DevID;
    //                        devNode.Info = devInfo;
    //                        devNode.Id = devInfo.Id;
    //                    }
    //                    if (OnSingleDevCreate != null) OnSingleDevCreate(devNode);
    //                });
    //                return;
    //            }
    //            if (OnSingleDevCreate != null) OnSingleDevCreate(null);
    //            return;//从这里返回，设备定位就相当与没有反应了
    //        }

    //        Debug.Log($"RoomFactory.CreateDevByDevId devInfo id:{devInfo.Id} pid:{devInfo.ParentId} name:{devInfo.Name} DevID:{devInfo.DevID} dep:{dep.NodeName}");

    //        List<int> pidList = new List<int>() { dep.NodeID };
    //        GetDoorAccessInfo(pidList);
    //        DevType devType = DevType.DepDev;
    //        GameObject devContainer = GetDepDevContainer(dep, ref devType);
    //        StartCoroutine(LoadSingleDevCorutine(devInfo, devContainer, devType, dep, obj =>
    //            {
    //                if (obj != null)
    //                {
    //                    Debug.LogError("LoadSingleDevCorutine obj==null");
    //                    DevNode dev = obj.GetComponent<DevNode>();
    //                    if (OnSingleDevCreate != null) OnSingleDevCreate(dev);
    //                }
    //                else
    //                {
    //                    if (OnSingleDevCreate != null) OnSingleDevCreate(null);
    //                }

    //            }));
    //    }
    //    /// <summary>
    //    /// 设置设备位置
    //    /// </summary>
    //    /// <param name="obj"></param>
    //    /// <param name="pos"></param>
    //    private void SetDevPos(GameObject obj, DevPos pos)
    //    {
    //        if (pos == null)
    //        {
    //            SetErrorDevPos(obj);
    //            return;
    //        }
    //        DevNode devNode = obj.GetComponent<DevNode>();
    //        bool isLocalPos = !(devNode.ParentDepNode == FactoryDepManager.Instance);
    //        Vector3 cadPos = new Vector3(pos.PosX, pos.PosY, pos.PosZ);
    //        Vector3 unityPos = LocationManager.CadToUnityPos(cadPos, isLocalPos);
    //        if (isLocalPos)
    //        {
    //            obj.transform.localPosition = new Vector3(unityPos.x, unityPos.y, unityPos.z);
    //        }
    //        else
    //        {
    //            obj.transform.position = new Vector3(unityPos.x, unityPos.y, unityPos.z);
    //        }
    //        obj.transform.eulerAngles = new Vector3(pos.RotationX, pos.RotationY, pos.RotationZ);
    //        obj.transform.localScale = new Vector3(pos.ScaleX, pos.ScaleY, pos.ScaleZ);
    //    }

    //    private void SetErrorDevPos(GameObject obj)
    //    {
    //        Debug.Log("Error dev name:" + obj.transform.name);
    //        obj.transform.position = Vector3.zero;
    //        obj.transform.eulerAngles = Vector3.zero;
    //        obj.transform.localScale = Vector3.one;
    //        //obj.Reset();
    //    }
    //    /// <summary>
    //    /// 给设备添加脚本
    //    /// </summary>
    //    /// <param name="dev"></param>
    //    /// <param name="info"></param>
    //    /// <param name="type"></param>
    //    private void AddDevController(GameObject dev, DevInfo info, DevType type, DepNode depNode)
    //    {
    //        try
    //        {
    //            if (TypeCodeHelper.IsDoorAccess(info.TypeCode.ToString()))
    //            {
    //                DoorAccessDevController doorController = dev.AddComponent<DoorAccessDevController>();
    //                Dev_DoorAccess doorAccess = DoorAccessList.Find(i => i.DevID == info.DevID);
    //                if (doorAccess == null)
    //                {
    //                    Debug.LogError("DoorAccess not find:" + info.DevID);
    //                    return;
    //                }
    //                doorAccess.DevInfo = info;
    //                doorController.DoorAccessInfo = doorAccess;
    //                SaveDepDevInfo(depNode, doorController, info);
    //                DoorAccessItem doorItem = DoorAccessModelAdd.GetDoorAccessByDoorName(doorAccess.DoorId);
    //                if(doorItem)
    //                {
    //                    doorItem.AddDoorAccess(doorController);
    //                    doorController.DoorItem = doorItem;
    //                }
    //                else
    //                {
    //                    Log.Error(string.Format("Door not find,DoorName:{0} DepName:{1}", doorAccess.DoorId,depNode.NodeName));
    //                }
    //                //if (depNode.Doors != null)
    //                //{
    //                //    DoorAccessItem doorItem = depNode.Doors.GetDoorItem(doorAccess.DoorId);
    //                //    doorItem.AddDoorAccess(doorController);
    //                //    doorController.DoorItem = doorItem;
    //                //}
    //                //else
    //                //{
    //                //    Log.Error(string.Format("RoomFactory.AddDevController:{0} ，Doors is null", depNode.NodeName));
    //                //}
    //            }
    //            else if (TypeCodeHelper.IsBorderAlarmDev(info.TypeCode.ToString()))
    //            {
    //                BorderDevController depDev = dev.AddComponent<BorderDevController>();
    //                SaveDepDevInfo(depNode, depDev, info);
    //            }
    //            else if (TypeCodeHelper.IsCamera(info.TypeCode.ToString()))
    //            {
    //                CameraDevController depDev = dev.AddComponent<CameraDevController>();
    //                SaveDepDevInfo(depNode, depDev, info);
    //            }
    //            else
    //            {
    //                //NavMeshObstacle obstacle = dev.gameObject.AddMissingComponent<NavMeshObstacle>();
    //                //obstacle.carving = true; //参考知识：https://www.jianshu.com/p/eae6c84793ac

    //                switch (type)
    //                {
    //                    case DevType.DepDev:
    //                        DepDevController depDev = dev.AddComponent<DepDevController>();
    //                        SaveDepDevInfo(depNode, depDev, info);
    //                        break;
    //                    case DevType.RoomDev:
    //                        RoomDevController roomDev = dev.AddComponent<RoomDevController>();
    //                        SaveDepDevInfo(depNode, roomDev, info);
    //                        break;
    //                    default:
    //                        Debug.Log("DevType not find:" + type);
    //                        break;
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            string devId = info == null ? "null" : info.DevID;
    //            string depId = depNode == null ? "null" : depNode.NodeID.ToString();
    //            Log.Error(string.Format("RoomFactory.AddDevController,DevInfo.local_Devid:{0} depId:{1}\n Exception:{2}", devId, depId, e.ToString()));
    //        }
    //    }
    //    #endregion
    //    #region 设备定位模块

    //    /// <summary>
    //    /// 保存设备信息
    //    /// </summary>
    //    /// <param name="depId"></param>
    //    /// <param name="dev"></param>
    //    public void SaveDepDevInfo(DepNode dep, DevNode dev, DevInfo devInfo)
    //    {
    //        dev.Info = devInfo;
    //        dev.ParentDepNode = dep;
    //        int depId = dep.NodeID;
    //        DepDevDic.AddDev(depId, dev);
    //    }
    //    /// <summary>
    //    /// 获取区域下的所有设备
    //    /// </summary>
    //    /// <param name="dep">区域</param>
    //    /// <param name="containRoomDev">是否包含房间设备（Floor）</param>
    //    /// <returns></returns>
    //    public List<DevNode> GetDepDevs(DepNode dep, bool containRoomDev = true)
    //    {
    //        return DepDevDic.GetDepDevs(dep, containRoomDev);
    //    }
    //    /// <summary>
    //    /// 通过设备Id获取已经创建的设备
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    /// <param name="parentId"></param>
    //    /// <returns>返回已经创建的设备</returns>
    //    public DevNode GetCreateDevById(string devId, int parentId)
    //    {
    //        return DepDevDic.GetDev(devId, parentId);
    //    }
    //    /// <summary>
    //    /// 看是否int类型的DevID 
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    /// <returns></returns>
    //    public static int? TryGetDevId(string devId)
    //    {
    //        try
    //        {
    //            int value = int.Parse(devId);
    //            return value;
    //        }
    //        catch (Exception e)
    //        {
    //            return null;
    //        }
    //    }
    //    /// <summary>
    //    /// 通过设备Id,获取设备
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    /// <returns></returns>
    //    public void GetDevById(string devId, Action<DevNode> onDevFind)
    //    {
    //        DevNode dev = DepDevDic.FindDev(devId);
    //        if (dev == null)
    //        {
    //            GetDevInfoByDevId(devId,info=> 
    //            {
    //                if (info == null)
    //                {
    //                    Debug.LogError("ID为[" + devId + "]的设备找不到");
    //                    if(onDevFind!=null)onDevFind(null);
    //                    return;
    //                }
    //                CreateDevByDevId(info, obj => { if (onDevFind != null) onDevFind(obj); });
    //            });            
    //        }
    //        else
    //        {
    //            if (onDevFind != null) onDevFind(dev);
    //        }
    //    }

    //    /// <summary>
    //    /// 通过设备Id(不是字符串DevId),获取设备
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <returns></returns>
    //    public void GetDevByid(int id, Action<DevNode> onDevFind)
    //    {
    //        DevNode dev = DepDevDic.FindDev(id);
    //        if (dev == null)
    //        {
    //            GetDevInfoByid(id,(devinfo)=>
    //            {
    //                if (devinfo == null || (devinfo != null && TypeCodeHelper.IsFireFightDevType(devinfo.TypeCode.ToString())))
    //                {
    //                    if (devinfo == null) Debug.LogError("ID为[" + id + "]的设备找不到");
    //                    onDevFind(null);
    //                    return;
    //                }
    //                CreateDevByDevId(devinfo, obj => { if (onDevFind != null) onDevFind(obj); });
    //            });

    //            //DevInfo info = GetDevInfoByid(id);
    //            //if (info == null || (info != null && TypeCodeHelper.IsFireFightDevType(info.TypeCode.ToString())))
    //            //{
    //            //    if (info == null) Debug.LogError("ID为[" + id + "]的设备找不到");
    //            //    onDevFind(null);
    //            //    return;
    //            //}
    //            //CreateDevByDevId(info, obj => { if (onDevFind != null) onDevFind(obj); });
    //        }
    //        else
    //        {
    //            if (onDevFind != null) onDevFind(dev);
    //        }
    //    }

    //    public Dictionary<string, DevInfo> clientDevList = new Dictionary<string, DevInfo>();

    //    //public DevInfo GetClientDev(string id)
    //    //{
    //    //    if(clientDevList.ContainsKey(id))
    //    //    {
    //    //        return clientDevList[id];
    //    //    }
    //    //    Debug.LogError($"GetClientDev Not Found id:{id}");
    //    //    return null;
    //    //}

    //    //public bool IsHideNoDeviceNode = false;

    //    public void AddClientDev(DevInfo dev)
    //    {
    //        if(clientDevList.ContainsKey(dev.DevID))
    //        {
    //            Debug.LogError($"clientDevList.ContainsKey(dev.DevID) dev:{dev.Name}");
    //            return;
    //        }
    //        clientDevList.Add(dev.DevID, dev);
    //    }

    //    /// <summary>
    //    /// 通过设备Id,获取设备信息
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    /// <returns></returns>
    //    private void GetDevInfoByDevId(string devId,Action<DevInfo>callback=null)
    //    {       
    //        Debug.LogError($"RoomFactory.GetDevInfoByDevId devId:{devId} clientDev:{clientDevList.ContainsKey(devId)} clientDevList:{clientDevList.Count}");
    //        if (clientDevList.ContainsKey(devId))
    //        {
    //            DevInfo devInfo= clientDevList[devId];
    //            var model = BimNodeHelper_PhysicalTopology.GetModel(devInfo);
    //            if (callback != null) callback(devInfo);
    //            return;
    //        }
    //        CommunicationObject service = CommunicationObject.Instance;
    //        if (service)
    //        {
    //            //service.GetDevByDevId(devId);
    //            service.GetDevByDevIdAsync(devId, callback);
    //        }
    //    }

    //    /// <summary>
    //    /// 通过设备Id,获取设备信息
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    /// <returns></returns>
    //    private DevInfo GetDevInfoByid(int id)
    //    {
    //        CommunicationObject service = CommunicationObject.Instance;
    //        if (service)
    //        {
    //            return service.GetDevByid(id);
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    /// 通过设备Id,获取设备信息
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    /// <returns></returns>
    //    private void GetDevInfoByid(int id, Action<DevInfo> callback)
    //    {
    //        CommunicationObject service = CommunicationObject.Instance;
    //        if (service)
    //        {
    //            service.GetDevByidAsync(id, callback);
    //        }
    //        //return null;
    //    }


    //    /// <summary>
    //    /// 删除设备信息
    //    /// </summary>
    //    /// <param name="dev"></param>
    //    public void RemoveDevInfo(DevNode dev)
    //    {
    //        if (dev.Info == null) return;
    //        RemoveDevInfo(dev.Info.DevID);
    //    }
    //    /// <summary>
    //    /// 删除设备信息
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    public void RemoveDevInfo(string devId)
    //    {
    //        try
    //        {
    //            DepDevDic.RemoveDev(devId);
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError("RoomFactory.RemoveDevinfo :" + e.ToString());
    //        }
    //    }

    /// <summary>
    /// 聚焦设备(去掉int类型，统一用string,方法内部区分int还是Guid的devId)
    /// </summary>
    public void FocusDev(string devId, int depId, Action<bool> onFocusComplete = null)
    {
        //Debug.LogFormat($"RoomFactory.FocusDev depId:{depId},devId:{devId}");
        ////2019_04_30_cww_处理问题:处理打开集控楼，打开一楼，创建设备，打开其他建筑，卸载集控楼，打开设备搜索界面，定位集控楼1楼设备，没有反应...       
        //DevNode devNode = DepDevDic.GetDev(devId, depId);
        //Debug.LogFormat($"RoomFactory.FocusDev devNode != null:{devNode != null}");
        //if (devNode != null)
        //{
        //    int? devIdTemp = TryGetDevId(devId);//区分int还是Guid的devId
        //    Debug.LogFormat($"RoomFactory.FocusDev devIdTemp :{devIdTemp}");
        //    if (devIdTemp == null)
        //    {
        //        FocusDevInner(devId, onFocusComplete);//创建并定位设备
        //    }
        //    else
        //    {
        //        FocusDevInner((int)devIdTemp, onFocusComplete);//创建并定位设备
        //    }
        //}
        //else
        //{
        //    FocusDepAndDev(devId, depId, onFocusComplete);//先定位区域，在定位设备
        //}
    }

    //    public DepNode CurrentFocusDep = null;

    //    private void FocusDepAndDev(string devId, int depId, Action<bool> onFocusComplete = null)
    //    {
    //        DepNode dep = GetDepNodeById(depId);
    //        try
    //        {
    //            if (dep != null && CurrentFocusDep == dep)
    //            {
    //                Debug.LogError($"RoomFactory.FocusDepAndDev CurrentFocusDep == dep dep:{dep.name}");
    //                int? devIdTemp = TryGetDevId(devId);
    //                if (devIdTemp == null)
    //                {
    //                    FocusDevInner(devId, onFocusComplete);
    //                }
    //                else
    //                {
    //                    FocusDevInner((int)devIdTemp, onFocusComplete);
    //                }
    //                return;
    //            }
    //            CurrentFocusDep = dep;
    //            if (dep)
    //            {
    //                if (dep.HaveTopNode)
    //                {
    //                    FocusNode(dep, () =>
    //                    {
    //                        int? devIdTemp = TryGetDevId(devId);
    //                        if (devIdTemp == null)
    //                        {
    //                            FocusDevInner(devId, onFocusComplete);
    //                        }
    //                        else
    //                        {
    //                            FocusDevInner((int)devIdTemp, onFocusComplete);
    //                        }
    //                    },false);
    //                }
    //                else
    //                {

    //                    Debug.LogError("RoomFactory.FocusDepAndDev,dep.HaveTopNode ==false:" + depId);
    //                    if (onFocusComplete != null) onFocusComplete(false);
    //                }
    //            }
    //            else
    //            {
    //                int? devIdTemp = TryGetDevId(devId);//在厂区内的设备模型
    //                if (devIdTemp == null)
    //                {
    //                    FocusDevInner(devId, onFocusComplete);
    //                }
    //                else
    //                {
    //                    FocusDevInner((int)devIdTemp, onFocusComplete);
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError("Error: Roomfactory.FoucusDepAndDev:" + e.ToString());
    //            if (onFocusComplete != null) onFocusComplete(false);
    //        }
    //    }

    //    /// <summary>
    //    /// 聚焦设备 int类型ID
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    private void FocusDevInner(int devId, Action<bool> onFocusComplete = null)
    //    {
    //        Log.Info("RoomFactory.FocusDevInner 2", string.Format("devId :{0}", devId));
    //        GetDevByid(devId, dev =>
    //        {
    //            if (dev)
    //            {
    //                dev.FocusOn();
    //                if (onFocusComplete != null) onFocusComplete(true);
    //            }
    //            else
    //            {
    //                DevInfo info = GetDevInfoByid(devId);
    //                if (info != null && TypeCodeHelper.IsFireFightDevType(info.TypeCode.ToString()))
    //                {
    //                    if (onFocusComplete != null) onFocusComplete(true);
    //                }
    //                else
    //                {
    //                    Debug.LogError("RoomFactory.FoucusDev,Dev is null :" + devId);
    //                    if (onFocusComplete != null) onFocusComplete(false);
    //                }
    //            }
    //        });
    //    }
    //    /// <summary>
    //    /// 聚焦设备s
    //    /// </summary>
    //    /// <param name="devId"></param>
    //    private void FocusDevInner(string devId, Action<bool> onFocusComplete = null)
    //    {
    //        Debug.Log($"RoomFactory.FocusDevInner 1 Before devId :{devId}");
    //        GetDevById(devId, dev =>
    //        {
    //            Debug.Log($"RoomFactory.FocusDevInner 1 After devId :{devId} dev:{dev}");
    //            if (dev)
    //            {
    //                dev.FocusOn();
    //                if (onFocusComplete != null) onFocusComplete(true);
    //            }
    //            else
    //            {
    //                Debug.LogError("RoomFactory.FoucusDev,Dev is null :" + devId);
    //                if (onFocusComplete != null) onFocusComplete(false);
    //            }
    //        });
    //    }
    //    #endregion
    //    #region BIM模型定位
    //    private Dictionary<string, BIMModelInfo> bimModelDic;

    //    public List<string> BimModels=new List<string>();

    //    ///// <summary>
    //    ///// 添加BIM模型信息
    //    ///// </summary>
    //    ///// <param name="guidT"></param>
    //    ///// <param name="objT"></param>
    //    //public void AddBIMModel(string guidT,GameObject objT)
    //    //{
    //    //    if (bimModelDic == null) bimModelDic = new Dictionary<string, GameObject>();
    //    //    if (string.IsNullOrEmpty(guidT)) return;
    //    //    if(!bimModelDic.ContainsKey(guidT))
    //    //    {
    //    //        bimModelDic.Add(guidT,objT);

    //    //        BimModels.Add(guidT);
    //    //    }
    //    //}

    //    public void AddBIMModel(BIMModelInfo bim)
    //    {
    //        if (bimModelDic == null) bimModelDic = new Dictionary<string, BIMModelInfo>();
    //        if (string.IsNullOrEmpty(bim.Guid)) return;
    //        if (!bimModelDic.ContainsKey(bim.Guid))
    //        {
    //            bimModelDic.Add(bim.Guid, bim);

    //            BimModels.Add(bim.Guid);
    //        }
    //    }

    //    /// <summary>
    //    /// 物体被删除时，移除BIM信息
    //    /// </summary>
    //    /// <param name="guidT"></param>
    //    public void RemoveBIMmodel(string guidT)
    //    {
    //        if (bimModelDic == null||string.IsNullOrEmpty(guidT)) return;
    //        if(bimModelDic.ContainsKey(guidT))
    //        {
    //            bimModelDic.Remove(guidT);
    //        }
    //    }
    //    /// <summary>
    //    /// 通过设备Guid，找到Bim模型
    //    /// </summary>
    //    /// <param name="modelId"></param>
    //    /// <returns></returns>
    //    public BIMModelInfo FindBIMModelByGuid(string modelId)
    //    {
    //        if(bimModelDic!=null&&bimModelDic.ContainsKey(modelId))
    //        {
    //            return bimModelDic[modelId];
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }

    //    public BIMModelInfo FindBIMModelByInfo(ModelItemInfo info)
    //    {
    //        if(bimModelDic!=null&&bimModelDic.ContainsKey(info.UId))
    //        {
    //            return bimModelDic[info.UId];
    //        }
    //        else
    //        {
    //            //string n=info.Name.Replace(".nwc","");
    //            //GameObject go=GameObject.Find(n);
    //            //if(go==null){
    //            //    Debug.Log("FindBIMModelByInfo未找到模型1:"+n);
    //            //    n+="_LOD0";
    //            //    go=GameObject.Find(n);
    //            //}
    //            //if(go==null){
    //            //    Debug.LogError("FindBIMModelByInfo未找到模型2:"+n);
    //            //}
    //            //return go;
    //            return null;
    //        }
    //    }

    //    #endregion
}
