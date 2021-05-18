using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MeshType :IComparable<MeshType>
{
    public static string GetTypeName(string name)
    {
        string[] parts = name.Split('[');
        string typeName = parts[0].Trim();
        typeName = typeName.Replace("*", "X");//文件中不能有*，但是模型里面可以有
        typeName = typeName.Replace("(Clone)", "");//Clone出来的对象
        return typeName;
    }

    public string TypeName = "";

    public int VertexCount = 0;

    public int ItemCount = 0;

    public int UnitVertexCount = 0;

    public float Percent = 0;

    public List<MeshNode> Items = new List<MeshNode>(); 

    public void AddItem(MeshNode node)
    {
        if (Items.Contains(node)) return;
        Items.Add(node);
        VertexCount += node.GetVertexCount();
        ItemCount++;

        UnitVertexCount = VertexCount / ItemCount;
    }

    public int CompareTo(MeshType other)
    {
        return other.VertexCount.CompareTo(this.VertexCount);
    }

    public override string ToString()
    {
        return TypeName+"|"+VertexCount;
    }
}

