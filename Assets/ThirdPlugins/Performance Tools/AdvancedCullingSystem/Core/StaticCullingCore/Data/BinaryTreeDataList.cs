using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "BinaryTreeData", menuName = "BinaryTreeData")]
public class BinaryTreeDataList : ScriptableObject
{
    public BinaryTreeDataList()
    {

    }

    public List<BinaryTreeData> Trees = new List<BinaryTreeData>();

    public void Add(BinaryTreeData data)
    {
        Trees.Add(data);
    }

    internal void Combine(BinaryTreeDataList binaryTreeDataList)
    {
        //throw new NotImplementedException();
    }
}

[Serializable]
public class BinaryTreeDataListXml//因为BinaryTreeDataList换到另一个项目就无法正常读取，增加一个xml方式保存
{
    public BinaryTreeDataListXml()
    {

    }

    public bool IsCompressed = false;

    public List<BinaryTreeData> Trees = new List<BinaryTreeData>();

    public void Add(BinaryTreeData data)
    {
        Trees.Add(data);
    }

    public List<string> stringList = new List<string>();

    private Dictionary<int, string> int2string = new Dictionary<int, string>();

    private Dictionary<string, int> string2int = new Dictionary<string, int>();

    public void Compress()
    {
        IsCompressed = true;
        DateTime start = DateTime.Now;
        int index = 0;
        foreach (var tree in Trees)
        {
            foreach (var node in tree.NodeDatas)
            {
                foreach (var id in node.Ids)
                {
                    stringList.Add(id);
                }
            }
        }
        stringList=stringList.Distinct().ToList();
        stringList.Sort();
        for (int i = 0; i < stringList.Count; i++)
        {
            int2string.Add(i, stringList[i]);
            string2int.Add(stringList[i], i);
        }

        foreach (var tree in Trees)
        {
            foreach (var node in tree.NodeDatas)
            {
                if (node.Ids.Count == 0)
                {
                    node.Index = null;
                }
                else
                {
                    for (int i = 0; i < node.Ids.Count; i++)
                    {
                        string id = node.Ids[i];
                        node.Index.Add(string2int[id]);
                    }
                }
                
                node.Ids = null;
            }
        }

        Debug.Log("Compress:" + (DateTime.Now - start).ToString());
    }

    public void Decompress()
    {
        Debug.Log("Decompress:"+ stringList.Count);
        for (int i = 0; i < stringList.Count; i++)
        {
            int2string.Add(i, stringList[i]);
            string2int.Add(stringList[i], i);
        }

        foreach (var tree in Trees)
        {
            foreach (var node in tree.NodeDatas)
            {
                if (node.Index != null)
                {
                    for (int i = 0; i < node.Index.Count; i++)
                    {
                        int id = node.Index[i];
                        node.Ids.Add(int2string[id]);
                    }
                }
            }
        }
    }
}
