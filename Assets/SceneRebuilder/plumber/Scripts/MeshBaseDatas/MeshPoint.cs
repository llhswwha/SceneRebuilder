using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MeshPoint
{
    public Vector3 Point;

    public int Id;

    public Vector3 Normal;

    //public MeshPoint()
    //{

    //}

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

public class MeshPlane:IComparable<MeshPlane>
{
    public string Key;
    public Vector3 Normal;
    public MeshPointList PointList;
    public Vector3 Center;

    public Vector3 GetCenter()
    {
        Center = PointList.GetCenter();
        return Center;
    }

    public int PointCount
    {
        get
        {
            if (PointList == null) return 0;
            return PointList.Count;
        }
    }

    //public void AddPoint(MeshPoint mp)
    //{
    //    PointList = new MeshPointList();
    //    PointList.Add(mp);
    //    Normal = mp.Normal;
    //}

    public void AddPoints(List<MeshPoint> mps)
    {
        if(PointList==null)
            PointList = new MeshPointList();
        PointList.AddRange(mps);
        if (PointList.Count > 0)
        {
            Normal = PointList[0].Normal;
        }
        GetCenter();
    }

    public int CompareTo(MeshPlane other)
    {
        return other.PointCount.CompareTo(this.PointCount);
    }
}

public class MeshPlaneList : List<MeshPlane>
{
    public MeshPlaneList()
    {

    }

    public MeshPlaneList(List<MeshPlane> list)
    {
        this.AddRange(list);
    }

    internal MeshPlane GetPlaneByNormal(Vector3 vector3, float minZero)
    {
        foreach (var item in this)
        {
            if (item.Normal == vector3)
            {
                return item;
            }
        }

        MeshPlane minT = null;
        foreach (var item in this)
        {
            float dis = Vector3.Distance(item.Normal, vector3);
            if (dis < minZero)
            {
                return item;
            }
        }
        return null;
    }

    internal MeshPlane GetClosedPlaneByNormal(Vector3 vector3, float minZero)
    {
        foreach (var item in this)
        {
            if (item.Normal == vector3)
            {
                return item;
            }
        }

        MeshPlane minT = null;
        float minDis = float.MaxValue;
        foreach (var item in this)
        {
            float dis = Vector3.Distance(item.Normal, vector3);
            if (dis < minDis)
            {
                minDis = dis;
                minT = item;
            }
        }
        return minT;
    }

    internal void CombineByNormal(float minDis,string name)
    {
        int count1 = this.Count;
        var list1 = new MeshPlaneList(this);
        //Debug.Log($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            var item1 = list1[i1];
            for (int i = 0; i < this.Count; i++)
            {
                var item2 = this[i];
                if (item1 == item2) continue;

                float normalDis = Vector3.Distance(item2.Normal, item1.Normal);
                if (normalDis < minDis)
                {
                    if (list1.Contains(item2))
                    {
                        list1.Remove(item2);
                    }
                    //Debug.Log($"CombineSameCenter Combine[{i1}][{i}] centerDist:{centerDist} isSamePoint:{isSamePoint} count:{this.Count} item1R:{item1.Radius} item2R:{item2.Radius} rDis:{rDis} minDis:{minDis}");
                    this.RemoveAt(i);
                    i--;
                }
            }
        }

        //foreach (var item in this)
        //{
        //    item.GetInfo();
        //}

        int count2 = this.Count;
        if (count2 != count1)
        {
            Debug.Log($"CombineByNormal count1:{count1} count2:{count2} name:{name}");
        }
        else
        {
           
        }

        if (count2 != 6)
        {
            Debug.LogError($"CombineByNormal count1:{count1} count2:{count2} minDis:{minDis} name:{name}");
        }
    }
}

public class MeshPointList:List<MeshPoint>
{
    public MeshPointList()
    {

    }

    public MeshPointList(List<MeshPoint> mps)
    {
        this.AddRange(mps);
    }

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
