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

    public Vector2 UV;

    //public MeshPoint()
    //{

    //}

    public MeshPoint(Vector3 p,  Vector3 n)
    {
        this.Point = p;
        this.Id = 0;
        this.Normal = n;
        this.UV = Vector2.zero;
    }

    public MeshPoint(Vector3 p, int i,Vector3 n)
    {
        this.Point = p;
        this.Id = i;
        this.Normal = n;
        this.UV = Vector2.zero;
    }

    public MeshPoint(Vector3 p, int i, Vector3 n, Vector2 uv)
    {
        this.Point = p;
        this.Id = i;
        this.Normal = n;
        this.UV = uv;
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

    public Vector3[] GetVertices()
    {
        Vector3[] vs = new Vector3[this.Count];
        for (int i = 0; i < this.Count; i++)
        {
            vs[i] = this[i].Point;
        }
        return vs;
    }

    public Vector3[] GetNormals()
    {
        Vector3[] vs = new Vector3[this.Count];
        for (int i = 0; i < this.Count; i++)
        {
            vs[i] = this[i].Normal;
        }
        return vs;
    }

    public Vector2[] GetUVs()
    {
        Vector2[] vs = new Vector2[this.Count];
        for (int i = 0; i < this.Count; i++)
        {
            vs[i] = this[i].UV;
        }
        return vs;
    }

    //public Mesh CreateMesh()
    //{
    //    Mesh mesh = new Mesh();
    //    mesh.vertices = this.GetVertices();
    //    mesh.normals = this.GetNormals();
    //    mesh.uv = this.GetUVs();
    //    mesh.name = "Mesh";
    //    return mesh;
    //}

    public Mesh CreateMesh()
    {
        Vector3[] vertices = new Vector3[this.Count];
        Vector3[] normals = new Vector3[this.Count];
        Vector2[] uv = new Vector2[this.Count];
        for (int i = 0; i < this.Count; i++)
        {
            vertices[i] = this[i].Point;
            normals[i] = this[i].Normal;
            uv[i] = this[i].UV;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.name = "Mesh";
        return mesh;
    }
}

public class MeshPointCountDict:List<MeshPoint>
{
    public Vector3 Point;

    public MeshPointCountDict(Vector3 p)
    {
        Point = p;
    }

    Dictionary<Vector3, int> pointCount = new Dictionary<Vector3, int>();
    Dictionary<int, List<Vector3>> countPoints = new Dictionary<int, List<Vector3>>();

    public void AddPoint(MeshPoint mp)
    {
        this.Add(mp);

        Vector3 p = mp.Point;
        if (pointCount.ContainsKey(p))
        {
            int count = pointCount[p];
            count++;
            pointCount[p] = count;
        }
        else
        {
            pointCount.Add(p, 1);
        }
    }

    public List<Vector3> GetListOfCount(int pCount)
    {
        if (countPoints.ContainsKey(pCount))
        {
            return countPoints[pCount];
        }

        List<Vector3> ps = new List<Vector3>();
        int c = 0;
        foreach (var p in pointCount.Keys)
        {
            var count = pointCount[p];
            if (count == pCount)
            {
                c++;
                //TransformHelper.ShowLocalPoint(p, pointScale * 2, root, trianglesObj2.transform).name += "_" + c;
                ps.Add(p);
            }
        }
        ps.Add(Point);
        countPoints.Add(pCount, ps);
        return ps;
    }

    public bool IsPointsCross(List<Vector3> ps1)
    {
        List<Vector3> ps2 = this.GetListOfCount(1);
        foreach (var p2 in ps2)
        {
            if (ps1.Contains(p2))
            {
                return true;
            }
        }
        return false;
    }

    public bool InsertToList(List<Vector3> ps1)
    {
        bool isCross = false;
        List<Vector3> ps2 = this.GetListOfCount(1);
        foreach (var p2 in ps2)
        {
            if (ps1.Contains(p2))
            {
                isCross= true;
                break;
            }
        }
        if (isCross)
        {
            foreach (var p2 in ps2)
            {
                if (!ps1.Contains(p2))
                {
                    ps1.Add(p2);
                }
            }
        }
        return isCross;
    }
}

public class MeshPointCountDictList :List<MeshPointCountDict>
{
    public MeshPointCountDictList()
    {

    }

    public MeshPointCountDictList(MeshPointCountDictList initList)
    {
        this.AddRange(initList);
    }

    public List<List<Vector3>> GetLines()
    {
        MeshPointCountDictList tempList = new MeshPointCountDictList(this);

        List<List<Vector3>> lines = new List<List<Vector3>>();
       
        for (int i = 0; i < tempList.Count; i++)
        {
            MeshPointCountDict dict1 = tempList[i];
            List<Vector3> ps1 = dict1.GetListOfCount(1);
            for (int j = 0; j < tempList.Count; j++)
            {
                MeshPointCountDict dict2 = tempList[j];
                if (dict2 == dict1) continue; 
                int count1 = ps1.Count;
                bool isCross = dict2.InsertToList(ps1);
                if (isCross)
                {
                    int count2 = ps1.Count;
                    tempList.RemoveAt(j);
                    j--;
                    j = 0;
                }
                else
                {

                }
            }
            lines.Add(ps1);
        }
        return lines;
    }
}