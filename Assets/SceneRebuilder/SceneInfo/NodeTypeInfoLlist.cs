using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTypeInfoList :List<NodeTypeInfo>
{
    private Dictionary<string, NodeTypeInfo> dict = new Dictionary<string, NodeTypeInfo>();

    public long totalVertCount;

    public long totalTriCount;

    public NodeTypeInfoList()
    {

    }

    public NodeTypeInfoList(List<NodeInfo> nodes)
    {
        foreach (var node in nodes)
        {
            NodeTypeInfo typeInfo = GetNodeTypeInfo(node.typeName);
            typeInfo.Add(node);
        }
        this.Sort();
    }

    public NodeTypeInfo GetNodeTypeInfo(string typeName)
    {
        NodeTypeInfo typeInfo = null;
        if (dict.ContainsKey(typeName) == false)
        {
            typeInfo = new NodeTypeInfo(typeName);
            dict.Add(typeName, typeInfo);
            this.Add(typeInfo);
        }
        else
        {
            typeInfo = dict[typeName];
        }
        return typeInfo;
    }

    public void SetPercent()
    {
        totalVertCount = 0;
        totalTriCount = 0;
        foreach (var typeInfo in this)
        {
            totalVertCount += typeInfo.GetTotalVertCount();
            totalTriCount += typeInfo.GetTotalTriCount();
        }
        foreach (var typeInfo in this)
        {
            typeInfo.SetPercent(totalVertCount);
        }
    }

    public string GetStatisticInfo(int startId,int typeMaxCount)
    {
        long totalVertCount2 = 0;
        long totalTriCount2 = 0;
        long modelCount = 0;
        NodeTypeInfoList list = this;
        for (int i = startId; i < list.Count; i++)
        {
            if (typeMaxCount > 0 && i >= typeMaxCount) break;
            NodeTypeInfo typeInfo = list[i];
            totalVertCount2 += typeInfo.GetTotalVertCount();
            totalTriCount2 += typeInfo.GetTotalTriCount();
            modelCount += typeInfo.Count;
        }

        if (totalVertCount == 0)
        {
            totalVertCount = totalVertCount2;
        }

        if (totalTriCount == 0)
        {
            totalTriCount = totalTriCount2;
        }

        string text = "";
        if (totalTriCount2 > 100000 || totalTriCount2 > 100000)
        {
            text = string.Format("1 tris:{0:F0}w({1:F1}%),verts:{0:F0}w({3:F1}%) count:{4}", totalTriCount2 / 10000f, (totalTriCount2 + 0.0f) / totalTriCount * 100,
                totalVertCount2 / 10000f, (totalVertCount2 + 0.0f) / totalVertCount * 100, modelCount);
        }
        else if (totalTriCount2 > 10000 || totalTriCount2 > 10000)
        {
            text = string.Format("2 tris:{0:F1}w({1:F1}%),verts:{2:F1}w({3:F1}%) count:{4}", totalTriCount2 / 10000f, (totalTriCount2 + 0.0f) / totalTriCount * 100,
                totalVertCount2 / 10000f, (totalVertCount2 + 0.0f) / totalVertCount * 100, modelCount);
        }
        else
        {
            text = string.Format("3 tris:{0}({1:F1}%),verts:{2}({3:F1}%) count:{4}", totalTriCount2 , (totalTriCount2 + 0.0f) / totalTriCount * 100,
                totalVertCount2 , (totalVertCount2 + 0.0f) / totalVertCount * 100, modelCount);
        }
        return text;
    }
}
