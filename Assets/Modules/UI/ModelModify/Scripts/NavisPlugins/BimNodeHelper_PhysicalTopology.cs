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

    public static Dictionary<DevInfo, ModelItemInfo> dev2Model = new Dictionary<DevInfo, ModelItemInfo>();

    public static ModelItemInfo GetModel(PhysicalTopology node)
    {
        if (node2Model.ContainsKey(node))
        {
            return node2Model[node];
        }
        return null;
    }

    public static ModelItemInfo GetModel(DevInfo node)
    {
        if (dev2Model.ContainsKey(node))
        {
            return dev2Model[node];
        }
        return null;
    }

    public static bool IsDebug = false;

    public static void InitBimModel(PhysicalTopology root)
    {
        DateTime start = DateTime.Now;
        int count = 0;
        try
        {

            Debug.LogError($"InitBimModel(PhysicalTopology) Start root:{root.Name}");
            var file = InitNavisFileInfoByModel.GetNavisFileInfoEx();

            //List<ModelItemInfo> models1 = file.GetAllModelInfos();
            //List<ModelItemInfo> models = models1.FindAll(i => string.IsNullOrEmpty(i.AreaName) == false);

            List<ModelItemInfo> models = file.GetAllModelInfos();

            count = models.Count;
            var nodes = GetAllChildren(root);
            //AreaDevTreeHelper.CreateDevTypeNodes(nodes);
            Debug.LogError($"InitBimModel root:{root.Name} nodes:{nodes.Count} models:{models.Count} time:{(DateTime.Now - start).TotalMilliseconds}ms");

            mCount = 10000;
            path2Node.Clear();
            dev2Model.Clear();
            InitNodeDict(nodes);

            StringBuilder notFoundModels = new StringBuilder();
            int notFoundCount = 0;
            int maxCount = int.MaxValue;
            if (IsDebug)
            {
                maxCount = 10;
            }
            for (int i = 0; i < models.Count && i< maxCount; i++)
            {
                ModelItemInfo model = models[i];
                var mAreaName = model.GetAreaName();
                if (string.IsNullOrEmpty(mAreaName))
                {
                    Debug.LogError($"FindNodeByName[{i}] string.IsNullOrEmpty(mAreaName) model:{model.Name} AreaName1:{model.AreaName} AreaName2:{mAreaName}");
                    continue;
                }
                if (mAreaName == "闵行电厂")
                {
                    Debug.LogError($"FindNodeByName[{i}] area==闵行电厂 model:{model.Name} AreaName1:{model.AreaName} AreaName2:{mAreaName}");
                    continue;
                }

                var area = FindNodeByName(root, mAreaName);
                if (area != null)
                {

                    if (IsDebug) Debug.Log($"FindNodeByName[{i}] model:{model.Name} area:{area.Name}");
                    CreateChild(area, model);
                }
                else
                {
                    Debug.LogError($"FindNodeByName[{i}] area==null model:{model.Name} AreaName1:{model.AreaName} AreaName2:{mAreaName}");

                    notFoundModels.AppendLine($"model:{model.Name} AreaName1:{model.AreaName} AreaName2:{mAreaName}");
                    notFoundCount++;
                }
                //break;
            }

            //Debug.LogError($"AddBimModel not found count:{notFoundCount} \n log:{notFoundModels}");

        }
        catch (Exception ex)
        {
            Debug.LogError($"InitBimModel Exception:{ex}");
        }
        Debug.LogError($"InitBimModel(PhysicalTopology) End models:{count} root:{root.Name} time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }

    public static void CreateChild(PhysicalTopology parent, ModelItemInfo model)
    {
        string path = model.GetPath();
        if (IsDebug) Debug.LogError($"CreateChild[{mCount}] model:{model.Name} path:{path} ");
        PhysicalTopology pathParentNode = GetPathNodeByName1(parent, path);
        DevInfo devInfo = null;
        //if (RoomFactory.Instance.clientDevList.ContainsKey(model.UId))
        //{
        //    devInfo = RoomFactory.Instance.clientDevList[model.UId];
        //}
        //else
        {
            devInfo = new DevInfo();
            devInfo.Name = model.Name;
            //devNode.Type = AreaTypes.设备;
            devInfo.Id = mCount++;
            //AddDevNode(parent, devNode);
            devInfo.ParentId = parent.Id;
            //devNode.DevID = model.Id;
            //devNode.DevID = model.RenderId;
            devInfo.DevID = model.UId;

            dev2Model.Add(devInfo, model);
            //RoomFactory.Instance.AddClientDev(devInfo);
        }

        //AreaDevTreeHelper.AddDevNode(pathParentNode, devInfo);
    }
     

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



    private static PhysicalTopology GetPathNodeByName2(PhysicalTopology area, string[] pathParts, int index)
    {
        if (area == null)
        {
            Debug.LogError($"CreateChild GetPathNodeByName2 area == null index:{index} pathPart:{pathParts[index]} pathParts:{pathParts.Length}");
        }
        //Debug.LogError($"CreateChild GetPathNodeByName2 area:{area.Name} index:{index} pathPart:{pathParts[index]} pathParts:{pathParts.Length}");
        PhysicalTopology node = null;
        if (area.Children != null)
            foreach (var childNode in area.Children)
            {
                if (childNode.Name == pathParts[index])
                {
                    node = childNode;
                    //Debug.LogError($"CreateChild GetPathNodeByName2 FindPathNode:{childNode.Name} children:{area.Children.Length}");
                    break;
                }
            }

        if (node == null)
        {
            //node = new PhysicalTopology();
            //node.Name = pathParts[index];
            ////node.Type = AreaTypes.分组;
            //node.Type = AreaTypes.分组;
            //node.Id = mCount++;
            ////node2Model.Add(node);
            //AddChildNode(area, node);
            //node= AreaDevTreeHelper.AddGroupNode(area, pathParts[index]);
        }
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
