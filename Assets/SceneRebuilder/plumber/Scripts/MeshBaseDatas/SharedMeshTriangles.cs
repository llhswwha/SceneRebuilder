using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SharedMeshTriangles : IComparable<SharedMeshTriangles>
{
    public int PointId;

    public Vector3 Point;

    public Vector3 Center;

    public Vector3 Normal;

    public float Radius;

    public float MinRadius;

    public float DistanceToCenter;

    public bool IsCircle;

    public float CircleCheckP;

    public SharedMeshTriangles(int id, Vector3 p, Vector3 normal, List<MeshTriangle> ts)
    {
        this.Center = Vector3.zero;
        this.Radius = 0;
        this.MinRadius = 0;
        this.DistanceToCenter = 0;
        this.IsCircle = true;
        this.CircleCheckP = 0;

        this.PointId = id;
        this.Point = p;
        this.Normal = normal;
        this.Triangles = new MeshTriangleList(ts);
        this.AllTriangles = new MeshTriangleList(ts);

        GetInfo();
    }


    public SharedMeshTriangles(SharedMeshTrianglesData d)
    {
        this.PointId = d.PointId;
        this.Point = d.Point;
        this.Center = d.Center;
        this.Normal = d.Normal;
        this.Radius = d.Radius;
        this.MinRadius = d.MinRadius;
        this.DistanceToCenter = d.DistanceToCenter;
        this.IsCircle = d.IsCircle;
        this.CircleCheckP = d.CircleCheckP;
        this.Triangles = new MeshTriangleList();
        this.AllTriangles = new MeshTriangleList();
    }
    public void GetInfo()
    {
        //Center = Triangles.GetCenter(PointId);
        Center = AllTriangles.GetCenter();
        //Radius= Triangles.GetRadius(PointId);
        CircleCheckP = Triangles.GetCircleCheckP(PointId);
        IsCircle = CircleCheckP <= CircleInfo.IsCircleMaxP;

        //if (IsCircle)
        //{
        //    Radius = Triangles.GetAvgRadius1(PointId);
        //}
        //else
        //{
        //    Radius = Triangles.GetAvgRadius2(PointId);
        //}

        Radius = AllTriangles.GetMaxRadius(0.00001f, Center);
        MinRadius = AllTriangles.GetMinRadius(0.00001f, Center);

        //Radius = Triangles.GetRadius2(PointId);

        DistanceToCenter = Vector3.Distance(Point, Center);
    }

    public override string ToString()
    {
        return $"{PointId}_{Point}_{Center}_{Radius}";
    }


    public Vector3 GetCenter()
    {
        return Center;
    }

    public Vector4 GetCenter4()
    {
        Vector4 c = Center;
        c.w = Radius;
        return c;
    }
    public Vector4 GetMinCenter4()
    {
        Vector4 c = Center;
        c.w = MinRadius;
        return c;
    }

    private MeshTriangleList Triangles;

    private MeshTriangleList AllTriangles;

    public MeshTriangleList GetAllTriangles()
    {
        return AllTriangles;
    }

    public void AddOtherTriangles(List<MeshTriangle> list)
    {
        this.AllTriangles.AddList(list);
    }

    public float GetRadius()
    {
        //return Triangles.GetRadius(PointId);
        return Radius;
    }

    public int CompareTo(SharedMeshTriangles other)
    {
        return other.Radius.CompareTo(this.Radius);
    }

   

    public bool IsSamePoint(Vector3 p, float minDis)
    {
        if (this.Point == p)
        {
            return true;
        }
        else
        {
            float dis = Vector3.Distance(this.Point, p);
            if (dis < minDis)
            {
                return true;
            }
        }

        if (this.Center == p)
        {
            return true;
        }
        else
        {
            float dis = Vector3.Distance(this.Center, p);
            if (dis < minDis)
            {
                return true;
            }
        }
        return false;
    }
}

public struct SharedMeshTrianglesData
{
    public int PointId;

    public Vector3 Point;

    public Vector3 Center;

    public Vector3 Normal;

    public float Radius;

    public float MinRadius;

    public float DistanceToCenter;

    public bool IsCircle;

    public float CircleCheckP;

    public SharedMeshTrianglesData(SharedMeshTriangles d)
    {
        this.PointId = d.PointId;
        this.Point = d.Point;
        this.Center = d.Center;
        this.Normal = d.Normal;
        this.Radius = d.Radius;
        this.MinRadius = d.MinRadius;
        this.DistanceToCenter = d.DistanceToCenter;
        this.IsCircle = d.IsCircle;
        this.CircleCheckP = d.CircleCheckP;
    }

    public Vector4 GetCenter4()
    {
        Vector4 c = Center;
        c.w = Radius;
        return c;
    }
    //public Vector4 GetMinCenter4()
    //{
    //    Vector4 c = Center;
    //    c.w = MinRadius;
    //    return c;
    //}
}

public class SharedMeshTrianglesList : List<SharedMeshTriangles>
{
    public static int sharedMinCount = 36;
    public static float minRepeatPointDistance = 0.00005f;

    public static SharedMeshTrianglesList GetList(MeshStructure mesh)
    {
        var meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }
        return trianglesList;
    }

    public static SharedMeshTrianglesList GetList(Mesh mesh)
    {
        var meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetKeyPointsByIdEx(sharedMinCount, minRepeatPointDistance);
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }
        return trianglesList;
    }

    public SharedMeshTrianglesList()
    {

    }

    public SharedMeshTrianglesList(List<SharedMeshTriangles> list)
    {
        this.AddRange(list);
    }

    public bool ContainsCenter(Vector3 p)
    {
        foreach (var item in this)
        {
            if (item.GetCenter() == p)
            {
                return true;
            }
        }
        return false;
    }
    public bool ContainsPoint(Vector3 p)
    {
        foreach (var item in this)
        {
            if (item.Center == p)
            {
                return true;
            }
        }
        return false;
    }

    public SharedMeshTriangles? FindItemByPoint(Vector3 p, float minDis)
    {
        foreach (var item in this)
        {
            if (item.IsSamePoint(p, minDis))
            {
                return item;
            }
        }
        return null;
    }



    public int Remove(Vector3 p)
    {
        int removeCount = 0;
        for (int i = 0; i < this.Count; i++)
        {
            SharedMeshTriangles item = this[i];
            if (item.GetCenter() == p)
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
        foreach (SharedMeshTriangles item in this)
        {
            //center += item.Point;
            center += item.Center;
        }
        center /= this.Count;
        return center;
    }

    internal void RemoveNotCircle()
    {
        for (int i = 0; i < this.Count; i++)
        {
            SharedMeshTriangles item = this[i];
            if (item.IsCircle == false)
            {
                this.RemoveAt(i);
                i--;
            }
        }
    }

    public List<SharedMeshTriangles> GetCircleList()
    {
        List<SharedMeshTriangles> list = new List<SharedMeshTriangles>();
        for (int i = 0; i < this.Count; i++)
        {
            SharedMeshTriangles item = this[i];
            if (item.IsCircle == true)
            {
                list.Add(item);
            }
        }
        return list;
    }

    public List<PlanePointDistance> GetPlanePointDistanceList()
    {
        var centerOfPoints = MeshHelper.GetCenterOfList(this);
        var distanceList = new List<PlanePointDistance>();
        foreach (var p in this)
        {
            distanceList.Add(new PlanePointDistance(p, centerOfPoints));
        }
        distanceList.Sort();
        return distanceList;
    }

    internal void CombineSameCenter(float minDis)
    {
        Debug.Log($"CombineSameCenter count:{this.Count} minDis:{minDis}");
        //for (int i = 0; i < this.Count-1; i++)
        //{
        //    SharedMeshTriangles item1 = this[i];
        //    for(int j=i+1;j<this.Count;j++)
        //    {
        //        SharedMeshTriangles item2 = this[j];
        //        float dis = Vector3.Distance(item2.Center, item1.Center);
        //        bool isSamePoint = item1.IsSamePoint(item2.Center, minDis);
        //        Debug.Log($"Combine[{i},{j}] dis:{dis} isSamePoint:{isSamePoint}");
        //        if (isSamePoint)
        //        {
        //            item1.Triangles.AddRange(item2.Triangles);
        //            item1.GetInfo();
        //            this.RemoveAt(j);
        //            j--;
        //        }
        //    }
        //}

        var list1 = GetCircleList();
        foreach (var item1 in list1)
        {
            for (int i = 0; i < this.Count; i++)
            {
                SharedMeshTriangles item2 = this[i];
                if (item1.PointId == item2.PointId) continue;
                float dis = Vector3.Distance(item2.Center, item1.Center);
                bool isSamePoint = item1.IsSamePoint(item2.Center, minDis);
                Debug.Log($"Combine[{i}] dis:{dis} isSamePoint:{isSamePoint}");
                //if (isSamePoint)
                //{
                //    item1.Triangles.AddRange(item2.Triangles);
                //    //item1.GetInfo();
                //    this.RemoveAt(i);
                //    i--;
                //}
            }
        }
    }
}
