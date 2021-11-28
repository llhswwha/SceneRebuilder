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

public static class BimNodeHelper_AreaNode
{
    public static void InitBimModel(AreaNode root)
    {
        //return;
        DateTime start = DateTime.Now;
        int count = 0;
        try
        {
            mCount = 10000;

            var file = InitNavisFileInfoByModel.GetNavisFileInfoEx();
            var models = file.GetAllModelInfos();
            count = models.Count;
            Debug.LogError($"InitBimModel(AreaNode) Start root:{root.Name} models:{models.Count}");
            InitNodeDict(root);

            StringBuilder notFoundModels = new StringBuilder();
            int notFoundCount = 0;

            for (int i = 0; i < models.Count; i++)
            {
                ModelItemInfo model = models[i];
                if (string.IsNullOrEmpty(model.AreaName))
                {
                    //Debug.LogError($"AddBimModel model:{model.Name} model.AreaName = null !!");
                    continue;
                }

                //var bim = RoomFactory.Instance.FindBIMModelByInfo(model);
                //if (bim == null) continue;

                var area = FindNodeByName(root, model.AreaName);
                if (area != null)
                {
                    CreateChild(area, model);
                }
                else
                {
                    notFoundModels.AppendLine($"model:{model.Name} AreaName:{model.AreaName}");
                    notFoundCount++;
                }
            }
            Debug.LogError($"InitBimModel not found count:{notFoundCount} \n log:{notFoundModels}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"InitBimModel Exception:{ex}");
        }
        Debug.LogError($"InitBimModel(AreaNode) End count:{count} root:{root.Name} time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }

    private static Dictionary<string, AreaNode> nodeDict2 = new Dictionary<string, AreaNode>();

    private static void InitNodeDict(AreaNode root)
    {
        if (root.Children != null)
            foreach (var child in root.Children)
            {
                if (nodeDict2.ContainsKey(child.Name))
                {
                    var node = nodeDict2[child.Name];
                    if (node != child)
                    {
                        Debug.LogError($"InitNodeDict nodeDict2.ContainsKey(child.Name) child:{child.Name} node:{nodeDict2[child.Name].Name}");
                    }
                }
                else
                {
                    nodeDict2.Add(child.Name, child);
                }

                InitNodeDict(child);
            }
    }

    private static AreaNode FindNodeByName(AreaNode parkNode, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (nodeDict2.ContainsKey(name))
        {
            return nodeDict2[name];
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
        //            nodeDict2.Add(name, child);
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


    public static Dictionary<AreaNode, ModelItemInfo> node2Model = new Dictionary<AreaNode, ModelItemInfo>();

    public static ModelItemInfo GetModel(AreaNode node)
    {
        if (node2Model.ContainsKey(node))
        {
            return node2Model[node];
        }
        return null;
    }

    public static int mCount = 10000;

    public static void CreateChild(AreaNode parent, ModelItemInfo model)
    {
        AreaNode node = new AreaNode();
        node.Name = model.Name;
        node.Type = AreaTypes.设备;
        node.Id = mCount++;
        //node.ta
        //area.Children.
        node2Model.Add(node, model);

        string path = model.GetPath();
        AreaNode pathParentNode = GetPathNodeByName(parent, path);
        if (mCount < 10)
        {
            Debug.LogError($"CreateChild model:{model.Name} path:{path} pathParentNode:{pathParentNode.Name}");
        }
        AddChild(pathParentNode, node);
    }

    public static void AddChild(AreaNode parent, AreaNode child)
    {
        //Debug.Log($"AddBimModel_AddChild parent:{parent.Name} child:{child.Name}");
        var children = parent.Children;
        if (children == null)
        {
            parent.Children = new AreaNode[1];
            parent.Children[0] = child;
        }
        else
        {
            parent.Children = new AreaNode[children.Length + 1];
            for (int i = 0; i < children.Length; i++)
            {
                parent.Children[i] = children[i];
            }
            parent.Children[children.Length] = child;
        }

    }

    public static Dictionary<string, AreaNode> path2Node = new Dictionary<string, AreaNode>();

    public static AreaNode GetPathNodeByName(AreaNode area, string path)
    {
        string fullPath = area.Name + "\\" + path;
        if (path2Node.ContainsKey(fullPath))
        {
            return path2Node[fullPath];
        }
        int startId = 1;
        string[] parts = path.Split('\\');
        //if (parts[0] == "HH" || parts[0] == "JQ" || parts[0] == "JG" || parts[0] == "SG")
        //{
        //    startId = 0;
        //}
        //else
        //{

        //}
        AreaNode pathParentNode = GetPathNodeByName(area, parts, startId);
        path2Node.Add(fullPath, pathParentNode);
        return pathParentNode;
    }

    private static AreaNode GetPathNodeByName(AreaNode area, string[] pathParts, int index)
    {
        AreaNode node = null;
        if (area.Children != null)
            foreach (var childNode in area.Children)
            {
                if (childNode.Name == pathParts[index])
                {
                    node = childNode;
                    break;
                }
            }

        if (node == null)
        {
            node = new AreaNode();
            node.Name = pathParts[index];
            node.Type = AreaTypes.分组;
            node.Id = mCount++;
            AddChild(area, node);
        }
        if (index < pathParts.Length - 2)
        {
            AreaNode node2 = GetPathNodeByName(node, pathParts, index + 1);
            return node2;
        }
        else
        {
            return node;
        }
    }
}