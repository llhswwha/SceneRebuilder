using CommonExtension;
using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct MeshTriangle 
{
    public static Vector3[] GetTrianglePoints(MeshTriangle[] triangles)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < triangles.Length; i++)
        {
            MeshTriangle t1 = triangles[i];
            var ps = t1.GetPoints();
            foreach (var p in ps)
            {
                if (points.Contains(p.Point)) continue;
                points.Add(p.Point);
            }
        }
        return points.ToArray();
    }

    public static List<Key2List<Vector3, MeshTriangle>> FindSharedPointsByPoint(MeshTriangle[] triangles)
    {
        DictionaryList1ToN<Vector3, MeshTriangle> sharedPoints = new DictionaryList1ToN<Vector3, MeshTriangle>();
        for (int i = 0; i < triangles.Length; i++)
        {
            MeshTriangle t1 = triangles[i];
            var ps = t1.GetPoints();
            foreach (var p in ps)
            {
                sharedPoints.AddItem(p.Point, t1);
            }
        }
        List<Key2List<Vector3, MeshTriangle>> list = sharedPoints.GetListSortByCount();
        return list;
    }

    public MeshPoint p1 ;
    public MeshPoint p2 ;
    public MeshPoint p3 ;

    public Vector3 Center;

    //public string Id;

    public List<MeshPoint> GetPoints()
    {
        var Points = new List<MeshPoint>();
        Points.Add(p1);
        Points.Add(p2);
        Points.Add(p3);
        return Points;
    }


    public Vector3 GetNormal()
    {
        Vector3 normal = Vector3.zero;
        normal += p1.Normal;
        normal += p2.Normal;
        normal += p3.Normal;
        normal /= 3;
        return normal;
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

    public string GetId()
    {
        List<int> ids = new List<int>();
        ids.Add(p1.Id);
        ids.Add(p2.Id);
        ids.Add(p3.Id);
        ids.Sort();
        string Id = $"{ids[0]}_{ids[1]}_{ids[2]}";
        return Id;
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

    public bool ContainsPoint(int pointId)
    {
        foreach (var p in GetPoints())
        {
            if (p.Id == pointId)
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

    public GameObject ShowTriangle(Transform root1, Transform root2, float pointScale,int id)
    {
        var points = this.GetPoints();
        GameObject objTriangle = new GameObject($"triangle[{id+1}]({points.Count})");
        objTriangle.transform.SetParent(root2);
        objTriangle.transform.localPosition = this.Center;
        //objTriangle.transform.localPosition = TestMeshOffset;
        objTriangle.transform.localPosition = Vector3.zero;

        objTriangle.transform.rotation = root1.transform.rotation;//++;

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

            PointHelper.ShowLocalPoint(p.Point, pointScale, root1, objTriangle.transform).name = $"Point[{p.Id}]({p.Point.Vector3ToString()})({p.Normal.Vector3ToString()})";
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

    public float GetDistanceOfPoints(Vector3 p)
    {
        float min = float.MaxValue;
        foreach(var mp in GetPoints())
        {
            float dis = Vector3.Distance(mp.Point, p);
            if (dis < min)
            {
                min = dis;
            }
        }
        return min;
    }
}

[Serializable]
public class MeshTriangleList : List<MeshTriangle>
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

    public List<float> GetRadiusList(float minR, Vector3 center)
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

    public List<float> GetRadiusList(float minR, Vector3 center, Vector3 point, float minDis)
    {
        //Vector3 center2 = GetCenter();
        List<float> radiusList = new List<float>();
        foreach (MeshTriangle triangle in this)
        {
            foreach (var p in triangle.GetPoints())
            {
                float dis = Vector3.Distance(p.Point, point);
                if (dis < minDis)
                {
                    continue;
                }
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

    public float[] GetMinMaxRadius(float minR, Vector3 center, Vector3 point, float minDis)
    {
        var list = GetRadiusList(minR, center, point, minDis);
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

        var minMax = VertexHelper.GetMinMax(ps.ToArray());
        return minMax[3];

        //foreach (var p in ps)
        //{
        //    center += p;
        //    count1++;
        //}

        //center /= count1;
        //return center;
    }

    public Vector3 GetCenter(Vector3 point, float minDis)
    {
        Vector3 center = Vector3.zero;
        int count1 = 0;
        List<Vector3> ps = GetPoints();
        for (int i = 0; i < ps.Count; i++)
        {
            Vector3 p = ps[i];
            float dis = Vector3.Distance(point, p);
            if (dis < minDis)
            {
                ps.RemoveAt(i);
                i--;
            }
        }
        var minMax = VertexHelper.GetMinMax(ps.ToArray());
        return minMax[3];
    }

    PositionDictionaryList<Vector3> posDict = new PositionDictionaryList<Vector3>();

    DictionaryList1ToN<int, MeshPoint> idDict = new DictionaryList1ToN<int, MeshPoint>();

    public void AddList(List<MeshTriangle> list)
    {
        base.AddRange(list);
        //posDict = null;
        AddMeshTriangleDictEx(list);
    }

    public void AddItem(MeshTriangle item)
    {
        base.Add(item);
        //posDict = null;
        AddMeshTriangleDictEx(item);
    }

    public new void Add(MeshTriangle item)
    {
        base.Add(item);
        //posDict = null;
        AddMeshTriangleDictEx(item);
    }
    public new void AddRange(IEnumerable<MeshTriangle> collection)
    {
        base.AddRange(collection);
        //posDict = null;
        AddMeshTriangleDictEx(collection);
    }

    public List<Vector3> GetPoints(bool isForce = false)
    {
        if (isForce)
        {
            CreatePosDict();
        }

        if (InitPosDict())
        {
            var ps = GetPosList();
            //Debug.LogError($"MeshTriangles.GetPoints count:{this.Count} allPos:{this.Count * 2 + 1} ps:{ps.Count}");
            return ps;
        }
        else
        {
            return GetPosList();
        }
    }

    public List<MeshPoint> GetMeshPoints(bool isForce = false)
    {
        if (isForce)
        {
            CreateIdDict();
        }
        if (InitIdDict())
        {
            var ps = GetPosListById();
            //Debug.LogError($"MeshTriangles.GetPoints count:{this.Count} allPos:{this.Count * 2 + 1} ps:{ps.Count}");
            return ps;
        }
        else
        {
            return GetPosListById();
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

    private List<MeshPoint> GetPosListById()
    {
        List<MeshPoint> ps = new List<MeshPoint>();
        foreach (var key in idDict.Keys)
        {
            var pos = idDict[key][0];
            ps.Add(pos);
        }
        //var ps = posDict.posListDict2.Keys.ToList();
        //Debug.LogError($"MeshTriangles.GetPoints count:{this.Count} allPos:{this.Count * 2 + 1} ps:{ps.Count}");
        return ps;
    }

    private bool InitPosDict()
    {
        if (posDict == null || posDict.posListDict.Count == 0)
        {
            CreatePosDict();
            //posDict.ShowCount("MeshTriangles.InitPosDict");
            return true;
        }
        return false;
    }

    public void CreatePosDict()
    {
        posDict = new PositionDictionaryList<Vector3>();
        foreach (MeshTriangle triangle in this)
        {
            AddMeshTriangleDict_Pos(triangle);
        }
    }

    private void AddMeshTriangleDict_Pos(MeshTriangle triangle)
    {
        foreach (var p in triangle.GetPoints())
        {
            posDict.Add(p.Point, p.Point, 2);
        }
    }

    private void AddMeshTriangleDict_Id(MeshTriangle triangle)
    {
        var ps = triangle.GetPoints();
        foreach (var p in ps)
        {
            idDict.AddItem(p.Id, p);
            //Debug.Log($"InitIdDict[{i + 1}] p:{p} count:{idDict.Count}");
        }
    }

    private void AddMeshTriangleDictEx(IEnumerable<MeshTriangle> triangles)
    {
        foreach (var t in triangles)
        {
            AddMeshTriangleDictEx(t);
        }
    }

    private void AddMeshTriangleDictEx(MeshTriangle triangle)
    {
        var ps = triangle.GetPoints();
        foreach (var p in ps)
        {
            idDict.AddItem(p.Id, p);
            posDict.Add(p.Point, p.Point, 2);
        }
    }

    private bool InitIdDict()
    {
        if (idDict == null || idDict.Count == 0)
        {
            CreateIdDict();
            //posDict.ShowCount("MeshTriangles.InitPosDict");
            return true;
        }
        return false;
    }

    public void CreateIdDict()
    {
        idDict = new DictionaryList1ToN<int, MeshPoint>();
        MeshTriangleList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            MeshTriangle triangle = list[i];
            AddMeshTriangleDict_Id(triangle);
        }
    }

    public void CreateTriangleDict()
    {
        idDict = new DictionaryList1ToN<int, MeshPoint>();
        posDict = new PositionDictionaryList<Vector3>();
        MeshTriangleList list = this;
        for (int i = 0; i < list.Count; i++)
        {
            MeshTriangle triangle = list[i];
            AddMeshTriangleDictEx(triangle);
        }
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
            Debug.LogWarning($"MeshTriangleList.GetCircleCheckP radiusList.Count == 0 count:{this.Count}");
            return 0;
        }
        float min = radiusList[0];
        float max = radiusList[radiusList.Count - 1];
        float p = max / min;
        //Debug.Log($"GetCircleCheckP pointId:{pointId} min:{min} max:{max} p:{p}");
        return p;
    }

    private DictList<int> PointIds = new DictList<int>();

    public bool ContainsPoint(MeshPoint mp, int level, string tag)
    {
        //if (idDict == null || !idDict.ContainsKey(mp.Id))
        //{
        //    CreateIdDict();
        //}

        //if (idDict == null)
        //{
        //    CreateIdDict();
        //}
        //if (idDict.ContainsKey(mp.Id))
        //{
        //    return true;
        //}

        //if (posDict == null || posDict.posListDict.Count == 0 || posDict.Contains(mp.Point, level) == false)
        //{
        //    CreatePosDict();
        //}

        if (idDict == null || idDict.Count == 0 || posDict == null || posDict.posListDict.Count == 0)
        {
            CreateTriangleDict();
        }
        if (idDict.ContainsKey(mp.Id))
        {
            return true;
        }
        if (posDict.Contains(mp.Point, level))
        {
            return true;
        }
        //float minDis = float.MaxValue;
        //var ps = this.GetMeshPoints(true);
        //MeshPoint minMp = new MeshPoint();
        //string ids = "";
        //foreach(var pos in ps)
        //{
        //    float dis = Vector3.Distance(pos.Point, mp.Point);
        //    if (minDis > dis)
        //    {
        //        minDis = dis;
        //        minMp = pos;
        //    }
        //    ids += pos.Id + ";";
        //}
        //if (minDis < 1)
        //{
        //    Debug.Log($"ContainsPoint({tag}) [ps:{ps.Count} ids:{ids}] level:{level} minDis:{minDis} minMp:{minMp} mp:{mp} ");
        //}
        return false;
    }

    public bool ContainsPoint(int pointId)
    {
        //if (idDict == null)
        //{
        //    InitIdDict();
        //}
        if (idDict == null || !idDict.ContainsKey(pointId))
        {
            CreateIdDict();
        }
        if (idDict.ContainsKey(pointId))
        {
            return true;
        }
        return false;

        //if (!PointIds.Contains(pointId))
        //{

        //    PointIds = new DictList<int>();
        //    foreach(var mp in mps)
        //    {
        //        PointIds.Add(mp.)
        //    }
        //}

        //var mps = GetPoints();
        //foreach (MeshTriangle triangle in this)
        //{
        //    if (triangle.ContainsPoint(pointId))
        //    {
        //        return true;
        //    }
        //}
        //return false;
    }

    internal bool ContainsTriangleByPointId(MeshTriangle t1)
    {
        //bool result = false;
        foreach (MeshPoint p in t1.GetPoints())
        {
            if (ContainsPoint(p.Id))
            {
                return true;
            }
        }
        return false;
    }

    //internal bool ContainsTriangleByPointPos(MeshTriangle t1,string tag)
    //{
    //    //bool result = false;
    //    foreach (MeshPoint p in t1.GetPoints())
    //    {
    //        if (ContainsPoint(p,2, tag))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    internal bool ContainsTriangleByPointPos(List<MeshPoint> mps, string tag)
    {
        //bool result = false;
        foreach (MeshPoint p in mps)
        {
            if (ContainsPoint(p, 2, tag))
            {
                return true;
            }
        }
        return false;
    }

    internal GameObject CreateMeshObject(Material mat,string name)
    {
        GameObject newGo = new GameObject(name);
        Vector3 center = this.GetCenter();
        newGo.transform.position = Vector3.zero;
        MeshFilter newMf = newGo.AddMissingComponent<MeshFilter>();
        MeshRenderer newRender = newGo.AddMissingComponent<MeshRenderer>();
        newRender.sharedMaterial = mat;
        //newMf.sharedMesh = VertexHelper.CopyMesh(mf);
        //newMf.sharedMesh.name = mf.name;
        //newGo.transform.SetParent(mf.transform.parent);
        newMf.sharedMesh = this.CreateMesh(center, name);
        newMf.sharedMesh.name = name;
        newGo.transform.position = center;
        return newGo;
    }

    private Mesh CreateMesh(Vector3 center, string name)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        MeshTriangleList list = this;
        int idCount=idDict.Count();
        int pCount = 0;
        int i0 = 0;
        Dictionary<int, int> id2id = new Dictionary<int, int>();
        foreach (var pId in idDict.Keys)
        {
            var mps = idDict[pId];
            //if (mps.Count > 1)
            //{
            //    Debug.LogError($"CreateMesh[{i0}] idDict pId:{pId} mps:{mps.Count}");
            //    break;
            //}
            var mp = mps[0];
            vertices.Add(mp.Point - center);
            normals.Add(mp.Normal);
            id2id.Add(pId, i0);
            i0++;
        }

        for (int i = 0; i < list.Count; i++)
        {
            MeshTriangle triangle = (MeshTriangle)list[i];
            //AddMeshTriangleDict_Pos(triangle);
            var mps = triangle.GetPoints();
            foreach (var mp in mps)
            {
                //vertices.Add(mp.Point- center);
                //normals.Add(mp.Normal);
                int pId = id2id[mp.Id];
                triangles.Add(pId);
                pCount++;
            }
            //triangles.Add(i * 3);
            //triangles.Add(i * 3 + 1);
            //triangles.Add(i * 3 + 2);
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();

        //Debug.LogError($"CreateMesh[{name}] list:{list.Count} idCount:{idCount} mpCount:{pCount}");
        return mesh;
    }
}

