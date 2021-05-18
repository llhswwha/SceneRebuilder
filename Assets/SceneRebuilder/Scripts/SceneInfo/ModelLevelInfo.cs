using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.Entities.UniversalDelegates;
using UnityEngine;

[Serializable]
public class ModelLevelInfo:IComparable<ModelLevelInfo>
{
    public int Id;
    public string Name;
    public double Height;

    public NodeTypeInfoList TypeList;

    public ModelCategoryList CategoryList;

    public List<NodeInfo> Nodes = new List<NodeInfo>();

    public void AddNode(NodeInfo node)
    {
        Nodes.Add(node);
    }

    public ModelLevelInfo(string name,double height,int id)
    {
        this.Name = name;
        this.Height = height;
        this.Id = id;
    }

    public void InitTypeList(CategoryInfoList categoryInfos)
    {
        TypeList = new NodeTypeInfoList(Nodes);
        CategoryList = new ModelCategoryList(categoryInfos,TypeList);
    }


    public int CompareTo(ModelLevelInfo other)
    {
        var r1 = this.Height.CompareTo(other.Height);
        if (r1 == 0)
        {

        }
        return r1;
    }
}
