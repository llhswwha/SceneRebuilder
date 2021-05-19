using AdvancedCullingSystem.StaticCullingCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Collections;
using UnityEngine;

[Serializable]
[XmlType("Node")]
public class BinaryTreeNodeData
{
    [XmlAttribute]
    public string Id;

    [XmlAttribute]
    public string ParentId;

    [XmlAttribute]
    public bool isLeft = true;

    public V3 center = new V3();

    public V3 size = new V3();

    public List<string> Ids = new List<string>();

    public List<int> Index = new List<int>();

    [NonSerialized]
    public BinaryTreeNodeData left = null;

    [NonSerialized]
    public BinaryTreeNodeData right = null;


    public BinaryTreeNodeData()
    {
        
    }

    public BinaryTreeNodeData(string id)
    {
        Id = id;
    }

    //public void AddRendererIds(NativeList<int> ids)
    //{
    //    //visibleRenderers.AddRange(ids);
    //    NativeList<int> ids2 = new NativeList<int>();
    //    ids2.AddRange(ids);
    //    foreach (int id in ids2)
    //    {
    //        AddId(id);
    //    }
    //}

    public void AddId(string id)
    {
        if (!Ids.Contains(id))
        {
            Ids.Add(id);
        }
    }
}
