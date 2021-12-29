using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshPoint
{
    public Vector3 Point;

    public int Id;

    public Vector3 Normal;

    public MeshPoint()
    {

    }

    public MeshPoint(Vector3 p, int i,Vector3 n)
    {
        this.Point = p;
        this.Id = i;
        this.Normal = n;
    }

    //public override string ToString()
    //{
    //    return Point.ToString();
    //}

    public override string ToString()
    {
        return Id.ToString();
    }
}

public class MeshPlane
{

}

public class MeshPointList:List<MeshPoint>
{
    public new bool Contains(MeshPoint mp)
    {
        foreach(var item in this)
        {
            if (item.Id == mp.Id)
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(Vector3 p)
    {
        foreach (var item in this)
        {
            if (item.Point==p)
            {
                return true;
            }
        }
        return false;
    }

    public int Remove(Vector3 p)
    {
        MeshPointList list = this;
        int removeCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            MeshPoint item = list[i];
            if (item.Point == p)
            {
                this.RemoveAt(i);
                i--;
                removeCount++;
            }
        }
        return removeCount;
    }

    public Vector3 GetCenter()
    {
        Vector3 center = Vector3.zero;
        foreach (var item in this)
        {
            center += item.Point;
        }
        center /= this.Count;
        return center;
    }
}
