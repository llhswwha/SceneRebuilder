using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class MeshFilterList:IComparable<MeshFilterList>
{
    public int vertexCount;

    public string MatId;

    public string Key;

    private List<MeshFilter> List = new List<MeshFilter>();

    private Dictionary<MeshFilter, MeshFilter> dict = new Dictionary<MeshFilter, MeshFilter>();

    public MeshFilterList()
    {

    }

    public MeshFilterList(MeshFilterList mfl)
    {
        this.vertexCount=mfl.vertexCount;
        AddRang(mfl);
    }

    public void AddRang(MeshFilterList mfl){
        var list=mfl.GetList();
        foreach(var item in list){
            this.Add(item);
        }
    }

    public void Add(MeshFilter mf)
    {
        //List.Add(mf);
        dict.Add(mf, mf);
    }

    internal void Remove(MeshFilter mf)
    {
        int count1 = dict.Count;
        
        //List.Remove(mf);
        bool r=dict.Remove(mf);
        int count2 = dict.Count;

        //Debug.Log($"MeshFilterList.Remove:{mf},R:{r},count1:{count1},count2:{count2}");
    }

    public int Count
    {
        get
        {
            return dict.Count;
        }
    }

    public List<MeshFilter> GetList()
    {
        List.Clear();
        foreach (var item in dict.Keys)
        {
            List.Add(item);
        }
        return List;
    }

//   public int Compare(MeshFilterList x, MeshFilterList y)
//   {
//     return x.vertexCount.CompareTo(x.vertexCount);
//   }

  public int CompareTo(MeshFilterList other)
  {
    return this.vertexCount.CompareTo(other.vertexCount);
  }
}