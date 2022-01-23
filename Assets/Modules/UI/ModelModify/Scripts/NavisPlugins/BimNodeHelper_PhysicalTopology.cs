//using Assets.M_Plugins.Helpers.Utils;
using Base.Common;
using Location.WCFServiceReferences.LocationServices;
using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BimNodeHelper_PhysicalTopology
{
    public static int mCount = 10000;

    public static Dictionary<PhysicalTopology, ModelItemInfo> node2Model = new Dictionary<PhysicalTopology, ModelItemInfo>();

    //public static Dictionary<DevInfo, ModelItemInfo> dev2Model = new Dictionary<DevInfo, ModelItemInfo>();

    public static Dictionary<string, ModelItemInfo> dev2Model_id = new Dictionary<string, ModelItemInfo>();

    public static ModelItemInfo GetModel(PhysicalTopology node)
    {
        if (node2Model.ContainsKey(node))
        {
            return node2Model[node];
        }
        return null;
    }

    public static Dictionary<PhysicalTopology, int> kksDevCountOfArea = new Dictionary<PhysicalTopology, int>();

    public static int GetKKSDevCount(PhysicalTopology topoNode)
    {
        if (topoNode == null) return 0;
        if (kksDevCountOfArea.ContainsKey(topoNode))
        {
            return kksDevCountOfArea[topoNode];
        }
        int kksCount = 0;
        foreach (DevInfo child in topoNode.LeafNodes)
        {
            bool isNotKKS = IsNoKKS(child);
            if (isNotKKS == false) kksCount++;
            //if (isNotKKS)
            //{
            //    notKKSDevs.Add(new DevInfoNodeData(child, null, treeNode));
            //}
        }
        kksDevCountOfArea.Add(topoNode, kksCount);
        return kksCount;
    }
    public static string GetLeafKKSDetails(PhysicalTopology topoNode)
    {
        //if (topoNode == null) return 0;
        //if (kksDevCountOfArea.ContainsKey(topoNode))
        //{
        //    return kksDevCountOfArea[topoNode];
        //}
        //int kksCount = 0;
        string detail = "";
        foreach (DevInfo child in topoNode.LeafNodes)
        {
            //bool isNotKKS = IsNoKKS(child);
            //if (isNotKKS == false) kksCount++;
            ////if (isNotKKS)
            ////{
            ////    notKKSDevs.Add(new DevInfoNodeData(child, null, treeNode));
            ////}
            var bim = GetModel(child);
            detail += $"{child.Name}|{bim.Visible};";
        }
        //kksDevCountOfArea.Add(topoNode, kksCount);
        return detail;
    }


    public static bool IsNoKKS(DevInfo node)
    {
        var bimInfo = BimNodeHelper_PhysicalTopology.GetModel(node);
        if (bimInfo != null && bimInfo.Visible < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static ModelItemInfo GetModel(DevInfo node)
    {
        //if (dev2Model.ContainsKey(node))
        //{
        //    return dev2Model[node];
        //}

        if (node == null) return null;

        if (!string.IsNullOrEmpty(node.DevID)&&dev2Model_id.ContainsKey(node.DevID))
        {
            return dev2Model_id[node.DevID];
        }

        //Debug.LogError($"GetModel Not Found dev:{node.Name} {node.Id} {node.DevID} test:{dev2Model_id.ContainsKey(node.DevID)}");
        return null;
    }

    public static bool IsDebug = false;

    public static void InitBimModel(PhysicalTopology root)
    {
        DateTime start = DateTime.Now;
        int count = 0;
        int totalCount = 0;
        //return;
        try
        {

            Debug.LogError($"InitBimModel(PhysicalTopology) Start root:{root.Name}");
            //var file = InitNavisFileInfoByModel.GetNavisFileInfoEx();

            var file = InitNavisFileInfoByModel.GetNavisFileInfoEx_New();

            //List<ModelItemInfo> models1 = file.GetAllModelInfos();
            //List<ModelItemInfo> models = models1.FindAll(i => string.IsNullOrEmpty(i.AreaName) == false);

            List<ModelItemInfo> models = file.GetAllModelInfos();

            //List<ModelItemInfo> models1 = file.GetAllModelInfos();
            //List<ModelItemInfo> models = models1.FindAll(i => i.Children!=null && i.Children.Count>0);

            count = models.Count;
            var nodes = GetAllChildren(root);
            //AreaDevTreeHelper.CreateDevTypeNodes(nodes);
            Debug.LogError($"InitBimModel root:{root.Name} nodes:{nodes.Count} models:{models.Count} time:{(DateTime.Now - start).TotalMilliseconds}ms");

            mCount = 10000;
            path2Node.Clear();
            //dev2Model.Clear();
            dev2Model_id.Clear();
            InitNodeDict(nodes);

            StringBuilder notFoundModels = new StringBuilder();
            int notFoundCount = 0;
            int maxCount = int.MaxValue;
            if (IsDebug)
            {
                maxCount = 10;
            }

            


            for (int i = 0; i < models.Count && i < maxCount; i++)
            {
                ModelItemInfo model = models[i];
                //var mAreaName = model.GetAreaName();
                var mAreaName = model.AreaName;
                if (InitNavisFileInfoByModel.Instance.IsShowAll)
                {
                    mAreaName = model.GetAreaName();
                }
                if (string.IsNullOrEmpty(mAreaName))
                {
                    //Debug.LogError($"FindNodeByName[{i}] string.IsNullOrEmpty(mAreaName) model:{model.Name} AreaName1:{model.AreaName} AreaName2:{mAreaName}");
                    continue;
                }
                if (mAreaName == "闵行电厂")
                {
                    //Debug.LogError($"FindNodeByName[{i}] area==闵行电厂 model:{model.Name} modelPath:{model.GetPath()} AreaName1:{model.AreaName} AreaName2:{mAreaName}");

                    notFoundModels.AppendLine($"FindNodeByName[{i}] area==闵行电厂 model:{model.Name} modelPath:{model.GetPath()} AreaName1:{model.AreaName} AreaName2:{mAreaName}");
                    notFoundCount++;
                    continue;
                }

                var area = FindNodeByName(root, mAreaName);
                //if(mAreaName=="HH")
                //{
                //    Debug.LogError($"FindNodeByName[HH][{i}] model:{model.Name} area:{area.Name}");
                //}
                
                if (area != null)
                {
                    if (area.Type == AreaTypes.大楼)
                    {
                        area = GetFloorArea(model, area);
                    }

                    if (IsDebug) Debug.Log($"FindNodeByName[{i}] model:{model.Name} area:{area.Name}");
                    CreateChild(area, model);
                    totalCount++;
                }
                else
                {
                    notFoundModels.AppendLine($"model:{model.Name} AreaName1:{model.AreaName} AreaName2:{mAreaName}");
                    notFoundCount++;
                }
                //break;

                if (notFoundCount>0 && notFoundCount % 200 == 0)
                {
                    Debug.LogError($"AddBimModel not found count:【{notFoundCount}】 \n log:{notFoundModels}");
                    notFoundModels = new StringBuilder();
                }
            }

            Debug.LogError($"AddBimModel not found count:【{notFoundCount}】 \n log:{notFoundModels}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"InitBimModel Exception:{ex}");
        }
        Debug.LogError($"InitBimModel(PhysicalTopology) End models:{count} totalCount:{totalCount} root:{root.Name} time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }

    private static PhysicalTopology GetFloorArea(ModelItemInfo model, PhysicalTopology area)
    {
        //DepNode bc = RoomFactory.Instance.GetDepNodeById(area.Id);
        //if (bc != null)
        //{
        //    Vector3 pos = model.GetPositon();
        //    if (bc.ChildNodes != null && bc.ChildNodes.Count > 0)
        //    {
        //        //area = bc.ChildNodes[0].TopoNode;
        //        if (InitNavisFileInfoByModel.Instance.IsFindClosedFloor)
        //        {

        //            area = bc.GetClosedDepNode(pos).TopoNode;
        //        }
        //        else
        //        {
        //            area = bc.ChildNodes[0].TopoNode;
        //        }

        //    }
        //}
        //else
        //{
        //    if (area.Children != null && area.Children.Length > 0)
        //    {
        //        area = area.Children[0];
        //    }
        //}

        return area;
    }

    internal static void FocusDevs(PhysicalTopology topoNode)
    {
        List<ModelItemInfo> models = new List<ModelItemInfo>();
        foreach(var subNode in topoNode.Children)
        {
            var model = GetModel(subNode);
            if (model != null)
            {
                models.Add(model);
            }
            else
            {
                Debug.LogError($"FocusDevs model==null subNode:{subNode.Name}");
            }
        }
        foreach(var dev in topoNode.LeafNodes)
        {
            var model = GetModel(dev);
            if (model != null)
            {
                models.Add(model);
            }
            else
            {
                Debug.LogError($"FocusDevs model==null dev:{dev.Name}");
            }
        }

        foreach (var info in models)
        {
            MeshSelection.SelectObjectByRId(info.RenderId, modelDetail =>
            {
                //FocusModelGo(modelDetail);
                //if (callback != null)
                //{
                //    callback(modelDetail);
                //}
            });
        }

        List<BIMModelInfo> bims = new List<BIMModelInfo>();
        foreach(var info in models)
        {
            //var modelT = RoomFactory.Instance.FindBIMModelByInfo(info);
            //if(modelT!=null)
            //    bims.Add(modelT);
            //else
            //{
            //    Debug.LogError($"BimNodeHelper_PhysicalTopology.FocusDevs modelT==null info:{info}");
            //}
        }

        if (bims.Count > 0)
        {
            //bims[0].FocusOn();
            BIMModelInfo.BeginMultiFocus();
            foreach (var bim in bims)
            {
                bim.MultiFocusOn();
            }
        }
        else
        {
            //RTEManager.Instance.FocusGO(models[0].gam);
            Debug.LogError($"BimNodeHelper_PhysicalTopology.FocusDevs bims.Count==0 topoNode:{topoNode.Name} models:{models.Count}");
        }
        
    }

    public static void CreateChild(PhysicalTopology parent, ModelItemInfo model)
    {
        string path = model.GetPath();
        if (parent == null)
        {
            Debug.LogError($"CreateChild parent==null model:{model.Name} path:{path} ");
            return;
        }

        if (IsDebug) Debug.LogError($"CreateChild[{mCount}] model:{model.Name} path:{path} UId:{model.UId} Type:{model.Type}");
        PhysicalTopology pathParentNode = GetPathNodeByName1(parent, path);

        //if (model.Children == null || model.Children.Count == 0)
        //{
        if (string.IsNullOrEmpty(model.UId))
        {
            if (!string.IsNullOrEmpty(model.Type))
            {
                Debug.LogError($"CreateChild[{mCount}] string.IsNullOrEmpty(model.UId) model:{model.Name} path:{path} UId:{model.UId} Type:{model.Type}");
            }
            else
            {
                //不需要打印，都是类型节点
            }
        }
        else
        {
            DevInfo devInfo = CreateDevInfoEx(parent, model);
            //AreaDevTreeHelper.AddDevNode(pathParentNode, devInfo);
        }

        //}
        //else
        //{
        //    Debug.LogError($"CreateChild[{mCount}] model.Children == null || model.Children.Count == 0 model:{model.Name} path:{path} UId:{model.UId} Type:{model.Type}");
        //}

    }

    public static DevInfo CreateDevInfoEx(PhysicalTopology parent, ModelItemInfo model)
    {
        

        DevInfo devInfo = null;
        //if (RoomFactory.Instance.clientDevList.ContainsKey(model.UId))
        //{
        //    devInfo = RoomFactory.Instance.clientDevList[model.UId];
        //}
        //else
        //{
        //    devInfo = CreateDevInfo(parent, model);
        //    RoomFactory.Instance.AddClientDev(devInfo);
        //}
        return devInfo;
    }

    public static DevInfo CreateDevInfo(PhysicalTopology parent, ModelItemInfo model)
    {
        var devInfo = new DevInfo();
        devInfo.Name = model.Name;
        //devNode.Type = AreaTypes.设备;
        devInfo.Id = mCount++;
        //AddDevNode(parent, devNode);
        devInfo.ParentId = parent.Id;
        //devNode.DevID = model.Id;
        //devNode.DevID = model.RenderId;
        devInfo.DevID = model.UId;
        devInfo.TypeCode = 20181008;// TypeCodeHelper.StaticDevTypeCode;
        devInfo.ModelName = BIMModelName;
        devInfo.TypeName = parent.Name;
        //dev2Model.Add(devInfo, model);

        if (dev2Model_id.ContainsKey(model.UId))
        {
            string path = model.GetPath();
            //dev2Model_id.Add(model.UId, model);
            Debug.LogError($"CreateChild[{mCount}] dev2Model_id.ContainsKey(model.UId) model:{model.Name} path:{path} UId:{model.UId} Type:{model.Type}");
        }
        else
        {
            dev2Model_id.Add(model.UId, model);
        }
        return devInfo;
    }

    public static string BIMModelName = "BIMModel";
     

    public static Dictionary<string, PhysicalTopology> path2Node = new Dictionary<string, PhysicalTopology>();

    public static PhysicalTopology GetPathNodeByName1(PhysicalTopology area, string path)
    {
        if (area == null)
        {
            Debug.LogError($"CreateChild GetPathNodeByName1 area == null path:{path}");
            return null;
        }

        string fullPath = area.Name + "\\" + path;
        //Debug.LogError($"CreateChild GetPathNodeByName1 area:{area.Name} path:{path} fullPath:{fullPath}");
        if (path2Node.ContainsKey(fullPath))
        {
            return path2Node[fullPath];
        }
        int startId = 1;
        string[] parts = path.Split('\\');
        if (parts[0] == "HH" || parts[0] == "JQ" || parts[0] == "JG" || parts[0] == "SG")
        {
            startId = 0;
        }
        else
        {

        }
        PhysicalTopology pathParentNode = GetPathNodeByName2(area, parts, startId);
        path2Node.Add(fullPath, pathParentNode);
        return pathParentNode;
    }

    public static PhysicalTopology FindChild(PhysicalTopology area,string name)
    {
        PhysicalTopology node = null;
        if (area.Children != null)
            foreach (var childNode in area.Children)
            {
                if (childNode.Name == name)
                {
                    node = childNode;
                    //Debug.LogError($"CreateChild GetPathNodeByName2 FindPathNode:{childNode.Name} children:{area.Children.Length}");
                    break;
                }
            }
        return node;
    }

    private static PhysicalTopology GetPathNodeByName2(PhysicalTopology area, string[] pathParts, int index)
    {
        if (area == null)
        {
            Debug.LogError($"CreateChild GetPathNodeByName2 area == null index:{index} pathPart:{pathParts[index]} pathParts:{pathParts.Length}");
        }
        //Debug.LogError($"CreateChild GetPathNodeByName2 area:{area.Name} index:{index} pathPart:{pathParts[index]} pathParts:{pathParts.Length}");
        PhysicalTopology node = FindChild(area, pathParts[index]);
        //if (node == null)
        //{
        //    node= AreaDevTreeHelper.AddGroupNode(area, pathParts[index]);
        //}
        PhysicalTopology result = node;
        if (index < pathParts.Length - 2)
        {
            PhysicalTopology node2 = GetPathNodeByName2(node, pathParts, index + 1);
            result= node2;
        }
        return result;
    }

    private static Dictionary<string, PhysicalTopology> nodeDict = new Dictionary<string, PhysicalTopology>();

    private static List<PhysicalTopology> GetAllChildren(PhysicalTopology root)
    {
        List<PhysicalTopology> nodes = new List<PhysicalTopology>();
        GetAllChildren(root, nodes);
        return nodes;
    }


    private static void GetAllChildren(PhysicalTopology root, List<PhysicalTopology> nodes)
    {
        if (root == null) return;
        if (root.Children != null)
            foreach (var child in root.Children)
            {
                nodes.Add(child);
                GetAllChildren(child, nodes);
            }
    }

    private static void InitNodeDict(List<PhysicalTopology> children)
    {
        //if (root == null) return;
        //if (root.Children != null)
        //    foreach (var child in root.Children)
        //    {
        //        //if (nodeDict.ContainsKey(child.Name))
        //        //{
        //        //    var node = nodeDict[child.Name];
        //        //    if (node != child)
        //        //    {
        //        //        Debug.LogError($"InitNodeDict nodeDict.ContainsKey(child.Name) child:{child.Name} node:{nodeDict[child.Name].Name}");
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    nodeDict.Add(child.Name, child);
        //        //}
        //        InitNodeDict(child);
        //    }

        //var children = GetAllChildren(root);
        nodeDict.Clear();
        foreach (var child in children)
        {
            //Debug.LogError($"InitNodeDict {child.Name}");
            if (nodeDict.ContainsKey(child.Name))
            {
                var node = nodeDict[child.Name];
                if (node != child)
                {
                    Debug.LogError($"InitNodeDict nodeDict.ContainsKey(child.Name) child:{child.Name} node:{nodeDict[child.Name].Name}");
                }
            }
            else
            {
                nodeDict.Add(child.Name, child);
            }
            //break;
        }

        Debug.LogError($"InitNodeDict children:{children.Count} dict:{nodeDict.Count}");
    }

    private static PhysicalTopology FindNodeByName(PhysicalTopology parkNode, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (nodeDict.ContainsKey(name))
        {
            return nodeDict[name];
        }
        else
        {
            return null;
        }

        //if (parkNode.Children != null)
        //    foreach (var child in parkNode.Children)
        //    {
        //        if (child.Name == name)
        //        {
        //            nodeDict.Add(name, child);
        //            return child;
        //        }
        //        var result = FindNodeByName(child, name);
        //        if (result != null)
        //        {
        //            //nodeDict.Add(name, result);
        //            return result;
        //        }
        //    }
        //return null;
    }
}
