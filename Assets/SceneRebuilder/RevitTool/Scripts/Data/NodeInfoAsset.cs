using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class NodeInfoAsset : ScriptableObject
{
    public string SceneName;

    public NodeInfo Node;

    public static void Save(NodeInfo node,string nName, string path)
    {
        NodeInfoAsset nodeInfo = new NodeInfoAsset();
        nodeInfo.Node = node;
        nodeInfo.SceneName = nName;
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(nodeInfo, path);
        AssetDatabase.SaveAssets();
#endif
    }

    public static List<NodeInfo> nodes = new List<NodeInfo>();

    public static List<string> names = new List<string>();

    public static List<string> paths = new List<string>();
    public static void Set(NodeInfo node,string nName,string path)
    {
        nodes.Add(node);
        names.Add(nName);
        paths.Add(path);
    }

    public static void Save(NodeInfo node)
    {
        
        int id = nodes.IndexOf(node);
        Debug.LogError("NodeInfoAsset Save id:" + id+ " node:"+ node);
        if (id != -1)
        {
            var name = names[id];
            var path = paths[id];
            Save(node, name, path);
        }
    }

    public override string ToString()
    {
        return Node.ToString();
    }
}
