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

    public float TriangleCount = 0;

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

        TriangleCount = this.AllTriangles.Count;

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
        this.TriangleCount = d.TriangleCount;
    }

    public void SetCenterWithOutPoint(float minDis)
    {
        Center = AllTriangles.GetCenter(Point,minDis);
        var minMaxR = AllTriangles.GetMinMaxRadius(minDis, Center, Point, minDis);
        MinRadius = minMaxR[0];
        Radius = minMaxR[1];
    }

    public void GetInfo()
    {
        //Center = Triangles.GetCenter(PointId);
        Center = AllTriangles.GetCenter();
        //Radius= Triangles.GetRadius(PointId);
        CircleCheckP = Triangles.GetCircleCheckP(PointId);
        

        //if (IsCircle)
        //{
        //    Radius = Triangles.GetAvgRadius1(PointId);
        //}
        //else
        //{
        //    Radius = Triangles.GetAvgRadius2(PointId);
        //}

        var minMaxR= AllTriangles.GetMinMaxRadius(0.00001f, Center);


        MinRadius = minMaxR[0];
        Radius = minMaxR[1];

        //Radius = Triangles.GetRadius2(PointId);

        DistanceToCenter = Vector3.Distance(Point, Center);

        IsCircle = CircleCheckP <= CircleInfo.IsCircleMaxP || DistanceToCenter< CircleInfo.MinDistanceToCenter;
    }

    public List<Vector3> points;

    public List<Vector3> GetPoints()
    {
        if (points == null||points.Count==0)
        {
            points= AllTriangles.GetPoints();
        }
        return points;
    }

    public override string ToString()
    {
        return $"{PointId}_({Point.x.ToString("F3")},{Point.y.ToString("F3")},{Point.z.ToString("F3")})_({Center.x.ToString("F3")},{Center.y.ToString("F3")},{Center.z.ToString("F3")})_{Radius}";
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

    public Vector4 GetCenter4WithOff(float offset)
    {
        Vector4 c = Center+Normal.normalized*offset;
        c.w = Radius;
        return c;
    }

    public Vector4 GetCenter4WithPower(float offset)
    {
        Vector4 c = Center;
        c.w = Radius* offset;
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
        points = null;
        this.AllTriangles.AddList(list);

        TriangleCount = this.AllTriangles.Count;
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

   

    public bool IsSamePointEx(Vector3 p, float minDis)
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

    public float TriangleCount;

    public override string ToString()
    {
        return $"{PointId}_({Point.x.ToString("F3")},{Point.y.ToString("F3")},{Point.z.ToString("F3")})_({Center.x.ToString("F3")},{Center.y.ToString("F3")},{Center.z.ToString("F3")})_{Radius}";
    }

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
        this.TriangleCount = d.TriangleCount;
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
    public void SortByCount()
    {
        this.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });
    }

    public static int sharedMinCount = 36;
    public static float minRepeatPointDistance = 0.00005f;

    public static SharedMeshTrianglesList GetList(MeshStructure mesh)
    {
        var meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetSharedMeshTrianglesListById(sharedMinCount, minRepeatPointDistance);
        foreach (SharedMeshTriangles triangles in trianglesList)
        {

        }
        return trianglesList;
    }

    public static SharedMeshTrianglesList GetList(Mesh mesh)
    {
        var meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        SharedMeshTrianglesList trianglesList = meshTriangles.GetSharedMeshTrianglesListById(sharedMinCount, minRepeatPointDistance);
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
            if (item.IsSamePointEx(p, minDis))
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
        list.Sort((a, b) => { return b.Radius.CompareTo(a.Radius); });
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

    internal void RemoveSamePoints(float minDis)
    {
        var list1 = new List<SharedMeshTriangles>(this);
        //if (list1.Count < 1)
        //{
        //    Debug.LogError($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        //}
        //Debug.Log($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            SharedMeshTriangles item1 = list1[i1];
            for (int i = 0; i < this.Count; i++)
            {
                SharedMeshTriangles item2 = this[i];
                if (item1.PointId == item2.PointId) continue;
                float dis = DistanceUtil.GetDistance(item2.GetPoints(), item1.GetPoints());
                //bool isSamePoint = item1.IsSamePointEx(item2.Point, minDis);二维

                if (dis < minDis)
                {
                    //item1.AddOtherTriangles(item2.GetAllTriangles());
                    //item1.GetInfo();

                    if (list1.Contains(item2))
                    {
                        list1.Remove(item2);
                    }

                    //Debug.Log($"CombineSameCenter Combine[{i1}][{i}] dis:{dis} isSamePoint:{isSamePoint} count:{this.Count} item1R:{item1.Radius} item2R:{item2.Radius}");

                    this.RemoveAt(i);
                    i--;
                }
            }
        }

        //foreach (var item in this)
        //{
        //    item.GetInfo();
        //}
    }

    internal List<MeshTriangle> GetAllTriangles()
    {
        List<MeshTriangle> triangles = new List<MeshTriangle>();
        for (int i1 = 0; i1 < this.Count; i1++)
        {
            SharedMeshTriangles item1 = this[i1];
            var ts = item1.GetAllTriangles();
            foreach(var t in ts)
            {
                if (triangles.Contains(t)) continue;
                triangles.Add(t);
            }
        }
        return triangles;
    }

    internal void CombineSamePoint(float minDis)
    {
        var list1 = new List<SharedMeshTriangles>(this);
        //if (list1.Count < 1)
        //{
        //    Debug.LogError($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        //}
        //Debug.Log($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            SharedMeshTriangles item1 = list1[i1];
            for (int i = 0; i < this.Count; i++)
            {
                SharedMeshTriangles item2 = this[i];
                //if (item1.PointId == item2.PointId) continue;
                if (item1 == item2) continue;
                float dis = Vector3.Distance(item2.Point, item1.Point);
                bool isSamePoint = item1.IsSamePointEx(item2.Point, minDis);

                if (isSamePoint)
                {
                    item1.AddOtherTriangles(item2.GetAllTriangles());
                    //item1.GetInfo();


                    if (list1.Contains(item2))
                    {
                        list1.Remove(item2);
                    }

                    //Debug.Log($"CombineSameCenter Combine[{i1}][{i}] dis:{dis} isSamePoint:{isSamePoint} count:{this.Count} item1R:{item1.Radius} item2R:{item2.Radius}");

                    this.RemoveAt(i);
                    i--;
                }
            }
        }

        foreach (var item in this)
        {
            item.GetInfo();
        }
    }

    internal void CombineSameCenter(float minDis)
    {
        var list1 = GetCircleList();
        if (list1.Count < 1)
        {
            Debug.LogError($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        }
        //Debug.Log($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            SharedMeshTriangles item1 = list1[i1];
            for (int i = 0; i < this.Count; i++)
            {
                SharedMeshTriangles item2 = this[i];
                if (item1.PointId == item2.PointId) continue;
                float dis = Vector3.Distance(item2.Center, item1.Center);
                bool isSamePoint = item1.IsSamePointEx(item2.Center, minDis);
                
                if (isSamePoint)
                {
                    item1.AddOtherTriangles(item2.GetAllTriangles());
                    //item1.GetInfo();
                   

                    if (list1.Contains(item2))
                    {
                        list1.Remove(item2);
                    }

                    //Debug.Log($"CombineSameCenter Combine[{i1}][{i}] dis:{dis} isSamePoint:{isSamePoint} count:{this.Count} item1R:{item1.Radius} item2R:{item2.Radius}");

                    this.RemoveAt(i);
                    i--;
                }
            }
        }

        foreach (var item in this)
        {
            item.GetInfo();
        }
    }

    internal void CombineSameMesh(float minDis)
    {
        int count1 = this.Count;
        var list1 = GetCircleList();
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            SharedMeshTriangles item1 = list1[i1];
            var ps1 = item1.GetPoints();
            for (int i = 0; i < this.Count; i++)
            {
                SharedMeshTriangles item2 = this[i];
                if (item1.PointId == item2.PointId) continue;
                var ps2 = item2.GetPoints();
                if (ps1.Count != ps2.Count) continue;
                float dis = DistanceUtil.GetDistance(ps1, ps2, false);
                bool isSame = dis < minDis;
                //Debug.Log($"Combine[{i1}][{i}] dis:{dis} isSame:{isSame}");
                if (isSame)
                {
                    //item1.AddOtherTriangles(item2.GetAllTriangles());
                    //item1.GetInfo();
                    this.RemoveAt(i);
                    i--;
                    if (list1.Contains(item2))
                    {
                        list1.Remove(item2);
                    }
                }
            }
        }
        int count2 = this.Count;
        Debug.Log($"CombineSameMesh count1:{count1} count2:{count2}");
    }

    public void CombineSameCircle(float minDis)
    {
        int count1 = this.Count;
        var list1 = GetCircleList();
        if (list1.Count < 1)
        {
            Debug.LogError($"CombineSameCircle minDis:{minDis} CircleList:{list1.Count}");
        }
        //Debug.Log($"CombineSameCenter minDis:{minDis} CircleList:{list1.Count}");
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            SharedMeshTriangles item1 = list1[i1];
            for (int i = 0; i < this.Count; i++)
            {
                SharedMeshTriangles item2 = this[i];
                if (item1.PointId == item2.PointId) continue;
                float centerDist = Vector3.Distance(item2.Center, item1.Center);
                float rDis = Mathf.Abs(item2.Radius - item1.Radius);
                bool isSamePoint = item1.IsSamePointEx(item2.Center, minDis);

                if (isSamePoint && rDis < minDis * 10)
                {
                    item1.AddOtherTriangles(item2.GetAllTriangles());
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
        //Debug.Log($"CombineSameCircle count1:{count1} count2:{count2}");
    }

    public SharedMeshTriangles? FindSameDirectionPlane(SharedMeshTriangles teePlane1,object name)
    {
        SharedMeshTriangles? teePlane2 = null;

        //float minNormalAngle = 0;
        for (int i = 0; i < this.Count; i++)
        {
            SharedMeshTriangles plane = this[i];
            var normalAngle = Vector3.Dot(teePlane1.Normal, plane.Normal);
            //Debug.Log($"FindSameDirectionPlane go:{name} angle[{i}] normal1:{teePlane1.Normal} normal2:{plane.Normal} angle:{normalAngle}");
            if (Mathf.Abs(normalAngle + 1) <= 0.00001)//相反或者平行
            {
                teePlane2 = plane;
                //break;
            }
            if (Mathf.Abs(normalAngle - 1) <= 0.00001)
            {
                teePlane2 = plane;
                //break;
            }
        }

        return teePlane2;
    }
}
