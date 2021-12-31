using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshTriangle 
{
    public MeshPoint p1 = new MeshPoint();
    public MeshPoint p2 = new MeshPoint();
    public MeshPoint p3 = new MeshPoint();

    public Vector3 Center = Vector3.zero;

    public List<MeshPoint> Points = new List<MeshPoint>();



    public MeshTriangle()
    {

    }

    public override string ToString()
    {
        return $"[{p1},{p2},{p3}]";
    }

    public MeshTriangle(MeshPoint p1, MeshPoint p2, MeshPoint p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;

        Points.Add(p1);
        Points.Add(p2);
        Points.Add(p3);

        Center = (p1.Point + p2.Point + p3.Point) / 3;
    }

    public MeshPoint GetPoint(int id)
    {
        foreach(var p in Points)
        {
            if (p.Id == id) return p;
        }
        return null; 
    }

    public bool ContainsPoint(MeshPoint mp)
    {
        foreach(var p in Points)
        {
            if (p.Id == mp.Id)
            {
                return true;
            }
            if (p.Point == mp.Point)
            {
                return true;
            }
        }
        return false;
    }

    public List<MeshPoint> FindSharedPoints(MeshTriangle other)
    {
        List<MeshPoint> ps = new List<MeshPoint>();
        foreach (MeshPoint p1 in Points)
        {
            if (other.ContainsPoint(p1))
            {
                ps.Add(p1);
            }
        }
        return ps;
    }

    internal Vector3[] GetMeshVertices()
    {
        return new Vector3[]{p1.Point,p2.Point,p3.Point };
    }

    internal int[] GetMeshTriangles()
    {
        return new int[] { 0,1,2 };
    }

    public Mesh GetTriangleMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = $"Triangle_{this.ToString()}";
        mesh.vertices = GetMeshVertices();
        mesh.triangles = GetMeshTriangles();
        return mesh;
    }

    public GameObject ShowTriangle(Transform root1, Transform root2, float pointScale)
    {
        var points = this.Points;
        GameObject objTriangle = new GameObject($"triangle");
        objTriangle.transform.SetParent(root2);
        objTriangle.transform.localPosition = this.Center;
        //objTriangle.transform.localPosition = TestMeshOffset;
        objTriangle.transform.localPosition = Vector3.zero;

        MeshFilter mf = objTriangle.AddComponent<MeshFilter>();
        MeshRenderer mr = objTriangle.AddComponent<MeshRenderer>();
        mf.sharedMesh = this.GetTriangleMesh();

        for (int j = 0; j < points.Count; j++)
        {
            MeshPoint p = points[j];

            //GameObject objPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ////objPoint.name = $"Point[{i + 1}][{j + 1}]({p.Point})";
            ////objPoint.name = $"Point[{j + 1}]({p.Point})";
            //objPoint.name = $"Point[{p.Id}]({p.Point})";
            //objPoint.transform.SetParent(root1);
            //objPoint.transform.localPosition = p.Point;
            //objPoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);

            //objPoint.transform.SetParent(objTriangle.transform);

            TransformHelper.ShowLocalPoint(p.Point, pointScale, root1, objTriangle.transform).name = $"Point[{p.Id}]({p.Point})";
        }

        return objTriangle;
    }

    internal float GetRadius(int pointId)
    {
        float radius = 0;
        MeshPoint mp=GetPoint(pointId);
        if (mp == null)
        {
            Debug.LogError($"GetRadius mp == null pointId:{pointId}");
            return radius;
        }
        foreach(var p in Points)
        {
            if (p == mp) continue;
            radius += Vector3.Distance(p.Point, mp.Point);
        }
        radius /= 2;
        return radius;
    }

    internal Vector3 GetCenter(int pointId)
    {
        MeshPoint mp = GetPoint(pointId);
        if (mp == null)
        {
            Debug.LogError($"GetRadius mp == null pointId:{pointId}");
            return Vector3.zero;
        }
        //int count = 0;
        Vector3 sum = Vector3.zero;
        foreach (var p in Points)
        {
            if (p == mp) continue;
            sum += p.Point;
        }
        Vector3 center=sum / 2;
        return center;
    }

    //internal float GetCenter()
    //{
    //    Vector3 sum = Vector3.zero;
    //    foreach (var p in Points)
    //    {
    //        sum += p.Point;
    //    }
    //    Vector3 center = sum / 2;
    //    return center;
    //}
}

public class MeshTriangleList:List< MeshTriangle >
{
    public float GetRadius(int pointId)
    {
        float radius = 0;
        foreach (MeshTriangle triangle in this)
        {
            float r = triangle.GetRadius(pointId);
            radius += r;
        }
        radius /= this.Count;
        return radius;
    }

    public Vector3 GetCenter(int pointId)
    {
        Vector3 center = Vector3.zero;
        foreach (MeshTriangle triangle in this)
        {
            Vector3 r = triangle.GetCenter(pointId);
            center += r;
        }
        center /= this.Count;
        return center;
    }

    //internal bool GetIsCircle(int pointId,float maxP)
    //{
    //    List<float> radiusList = new List<float>();
    //    float radius = 0;
    //    foreach (MeshTriangle triangle in this)
    //    {
    //        float r = triangle.GetRadius(pointId);
    //        radius += r;
    //        radiusList.Add(r);
    //    }
    //    radiusList.Sort();
    //    float min = radiusList[0];
    //    float max = radiusList[radiusList.Count - 1];
    //    float p = max / min;
    //    Debug.Log($"GetIsCircle pointId:{pointId} min:{min} max:{max} p:{p} maxP:{maxP} result{p <= maxP}");
    //    return p <= maxP;
    //}

    internal float GetCircleCheckP(int pointId)
    {
        List<float> radiusList = new List<float>();
        float radius = 0;
        foreach (MeshTriangle triangle in this)
        {
            float r = triangle.GetRadius(pointId);
            radius += r;
            radiusList.Add(r);
        }
        radiusList.Sort();
        float min = radiusList[0];
        float max = radiusList[radiusList.Count - 1];
        float p = max / min;
        Debug.Log($"GetCircleCheckP pointId:{pointId} min:{min} max:{max} p:{p}");
        return p;
    }
}


public class SharedMeshTriangles
{
    public int PointId;

    public Vector3 Point;

    public Vector3 Center;

    public float DistanceToCenter;

    public bool IsCircle = true;

    public float CircleCheckP = 0;


    public Vector3 GetCenter()
    {
        return Center;
    }

    public MeshTriangleList Triangles = new MeshTriangleList();

    public float GetRadius()
    {
        return Triangles.GetRadius(PointId);
    }

    public SharedMeshTriangles(int id, Vector3 p, List<MeshTriangle> ts)
    {
        this.PointId = id;
        this.Point = p;
        this.Triangles.AddRange(ts);
        Center = Triangles.GetCenter(PointId);
        CircleCheckP = Triangles.GetCircleCheckP(PointId);
        IsCircle = CircleCheckP <= CircleInfo.IsCircleMaxP;
        DistanceToCenter = Vector3.Distance(Point, Center);
    }
}

public class SharedMeshTrianglesList : List<SharedMeshTriangles>
{
    public bool Contains(Vector3 p)
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
}