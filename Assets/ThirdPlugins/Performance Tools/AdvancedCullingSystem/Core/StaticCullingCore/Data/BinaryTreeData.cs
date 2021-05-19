using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BinaryTreeData
{
    public List<BinaryTreeNodeData> NodeDatas = new List<BinaryTreeNodeData>();

    public void Add(BinaryTreeNodeData data)
    {
        NodeDatas.Add(data);
    }
}