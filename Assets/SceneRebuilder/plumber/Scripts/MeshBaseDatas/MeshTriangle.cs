using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct MeshTriangle 
{
    public MeshPoint p1 ;
    public MeshPoint p2 ;
    public MeshPoint p3 ;

    public Vector3 Center;

    public List<MeshPoint> GetPoints()
    {
        var Points = new List<MeshPoint>();
        Points.Add(p1);
        Points.Add(p2);
        Points.Add(p3);
        return Points;
    }

    //public MeshTriangle()
    //{
    //    p1 = new MeshPoint();
    //    p2 = new MeshPoint();
    //    p3 = new MeshPoint();

    //    Center = Vector3.zero;

    //    Points = new List<MeshPoint>();
    //}

    public override string ToString()
    {
        return $"[{p1},{p2},{p3}]";
    }

    public MeshTriangle(MeshPoint p1, MeshPoint p2, MeshPoint p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;

        Center = (p1.Point + p2.Point + p3.Point) / 3;
    }

    public MeshPoint GetPoint(int id)
    {
        foreach(var p in GetPoints())
        {
            if (p.Id == id) return p;
        }
        return new MeshPoint(); 
    }

    public bool ContainsPoint(MeshPoint mp)
    {
        foreach(var p in GetPoints())
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
        foreach (MeshPoint p1 in GetPoints())
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
        var points = this.GetPoints();
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
        if (mp.Id==0)
        {
            //Debug.LogWarning($"GetRadius mp == null pointId:{pointId}");
            return radius;
        }
        foreach(var p in GetPoints())
        {
            if (p.Id == mp.Id) continue;
            radius += Vector3.Distance(p.Point, mp.Point);
        }
        radius /= 2;
        return radius;
    }

    internal Vector3 GetCenter(int pointId)
    {
        MeshPoint mp = GetPoint(pointId);
        if (mp.Id == 0)
        {
            Debug.LogError($"GetRadius mp == null pointId:{pointId}");
            return Vector3.zero;
        }
        //int count = 0;
        Vector3 sum = Vector3.zero;
        foreach (var p in GetPoints())
        {
            if (p.Id == mp.Id) continue;
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
    public MeshTriangleList()
    {

    }

    public MeshTriangleList(List<MeshTriangle> list)
    {
        this.AddRange(list);
    }

    public float GetAvgRadius1(int pointId)
    {
        float radius = 0;
        int count = 0;
        foreach (MeshTriangle triangle in this)
        {
            float r = triangle.GetRadius(pointId);
            if (r == 0) continue;
            count++;
            radius += r;
        }
        radius /= count;
        return radius;
    }

    public List<float> GetRadiusList(float minR,Vector3 center)
    {
        //Vector3 center2 = GetCenter();
        List<float> radiusList = new List<float>();
        foreach (MeshTriangle triangle in this)
        {
            foreach (var p in triangle.GetPoints())
            {
                float r = Vector3.Distance(center, p.Point);
                if (r < minR)
                {
                    continue;
                }
                radiusList.Add(r);
            }
        }
        radiusList.Sort();
        return radiusList;
    }

    public float GetMaxRadius(float minR, Vector3 center)
    {
        return GetRadiusList(minR, center).Last();
    }

    public float GetMinRadius(float minR, Vector3 center)
    {
        return GetRadiusList(minR, center).First();
    }

    public float[] GetMinMaxRadius(float minR, Vector3 center)
    {
        var list = GetRadiusList(minR, center);
        return new float[2] { list.First(), list.Last() };
    }

    public float GetAvgRadius2(int pointId)
    {
        //Vector3 center1 = GetCenter();
        Vector3 center2 = GetCenter(pointId);
        float radius = 0;
        int count2 = 0;
        foreach (MeshTriangle triangle in this)
        {
            foreach (var p in triangle.GetPoints())
            {
                float r = Vector3.Distance(center2, p.Point);
                radius += r;
                count2++;
            }
        }
        radius /= count2;
        return radius;
    }

    public Vector3 GetCenter()
    {
        Vector3 center = Vector3.zero;
        int count1 = 0;
        //PositionDictionaryList<Vector3> posDict = new PositionDictionaryList<Vector3>();
        //List<Vector3> ps = new List<Vector3>();
        //List<int> psIds = new List<int>();
        //foreach (MeshTriangle triangle in this)
        //{
        //    foreach (var p in triangle.Points)
        //    {
        //        if (!ps.Contains(p.Point))
        //        {
        //            center += p.Point;
        //            count1++;
        //            ps.Add(p.Point);
        //        }
        //    }
        //}

       

        List<Vector3> ps = GetPoints();

        var minMax=MeshHelper.GetMinMax(ps.ToArray());
        return minMax[3];

        //foreach (var p in ps)
        //{
        //    center += p;
        //    count1++;
        //}

        //center /= count1;
        //return center;
    }

    PositionDictionaryList<Vector3> posDict = new PositionDictionaryList<Vector3>();

    public void AddList(List<MeshTriangle> list)
    {
        this.AddRange(list);
        posDict = null;
    }

    public List<Vector3> GetPoints()
    {
        if (InitPosDict())
        {
            var ps= GetPosList();
            //Debug.LogError($"MeshTriangles.GetPoints count:{this.Count} allPos:{this.Count * 2 + 1} ps:{ps.Count}");
            return ps;
        }
        else
        {
            return GetPosList();
        }
        
    }

    private List<Vector3> GetPosList()
    {
        List<Vector3> ps = new List<Vector3>();
        foreach (var key in posDict.posListDict2.Keys)
        {
            var pos = posDict.posListDict2[key][0];
            ps.Add(pos);
        }
        //var ps = posDict.posListDict2.Keys.ToList();
        //Debug.LogError($"MeshTriangles.GetPoints count:{this.Count} allPos:{this.Count * 2 + 1} ps:{ps.Count}");
        return ps;
    }

    private bool InitPosDict()
    {
        if (posDict == null || posDict.posListDict.Count==0)
        {
            posDict = new PositionDictionaryList<Vector3>();

            foreach (MeshTriangle triangle in this)
            {
                foreach (var p in triangle.GetPoints())
                {
                    posDict.Add(p.Point, p.Point,2);
                }
            }
            //posDict.ShowCount("MeshTriangles.InitPosDict");
            return true;
        }
        return false;
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
            if (r == 0) continue;
            radius += r;
            radiusList.Add(r);
        }
        radiusList.Sort();
        if (radiusList.Count == 0)
        {
            Debug.LogError($"GetCircleCheckP radiusList.Count == 0 count:{this.Count}");
            return 0;
        }
        float min = radiusList[0];
        float max = radiusList[radiusList.Count - 1];
        float p = max / min;
        //Debug.Log($"GetCircleCheckP pointId:{pointId} min:{min} max:{max} p:{p}");
        return p;
    }
}

