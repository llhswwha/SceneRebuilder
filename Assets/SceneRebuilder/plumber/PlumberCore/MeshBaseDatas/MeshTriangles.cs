using CommonUtils;
using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class MeshTriangles
{
    public bool isSubT = false;
    public MeshTriangleList Triangles = new MeshTriangleList();

    public int Count
    {
        get
        {
            return Triangles.Count;
        }
    }

    public void Dispose()
    {
        if (isSubT) return;
        mesh.Dispose();
    }

    public MeshTriangles()
    {

    }


    public MeshTriangle GetTriangle(int id)
    {
        return Triangles[id];
    }

    public void AddTriangle(MeshTriangle t)
    {
        Triangles.Add(t);
    }

    public MeshStructure mesh;

    public void GenerateNewMesh()
    {
        mesh = new MeshStructure(Triangles);
    }

    public Vector3 center = Vector3.zero;

    public MeshTriangles(Mesh mesh)
    {
        Init(new MeshStructure(mesh));
    }

    public void ShowTriangles(Transform transform,float pointScale,string tag="")
    {
        GameObject tsRoot = CreateSubTestObj($"Triangles{tag}({this.Count}_{mesh.vertexCount})", transform);
        MeshTriangles meshTriangles = this;
        Debug.Log($"ShowTriangles trialges:{meshTriangles.Count}_{mesh.vertexCount}");
        for (int i = 0; i < meshTriangles.Count; i++)
        {
            MeshTriangle t = meshTriangles.GetTriangle(i);

            //Debug.Log($"ShowTriangles[{i + 1}/{meshTriangles.Count}] trialge:{t}");
            GameObject sharedPoints1Obj = CreateSubTestObj($"trialge[{i+1}]:{t}", tsRoot.transform);
            t.ShowTriangle(tsRoot.transform, sharedPoints1Obj.transform, pointScale);
        }
    }

    //public PipeLineInfo ShowTrianglesWithObb(Transform t, float pointScale, string tag, bool isShowDebugObj)
    //{
    //    PipeLineModel pipeLine = t.gameObject.AddMissingComponent<PipeLineModel>();
    //    pipeLine.lineSize = pointScale;
    //    PipeLineInfo pipeLineInfo = pipeLine.ShowLinePartModelInfo(this.GetPoints().ToArray(), t, isShowDebugObj, 0.001f, 20, 100);
    //    if (pipeLineInfo == null)
    //    {
    //        List<List<Vector3>> pointLines = GetLinePoints(t, pointScale, 0);
    //        if (pointLines.Count == 2)
    //        {
    //            pipeLineInfo = new PipeLineInfo(pointLines);
    //        }
    //        else
    //        {
    //            Debug.LogError($"ShowTrianglesWithObb[{tag}] pointLines.Count != 2 transform:{t.name}");
    //        }
    //    }
    //    return pipeLineInfo;
    //}

    public List<MeshTriangles> Split(int count)
    {
        var meshTriangles = this;
        List<MeshTriangles> list = new List<MeshTriangles>();
        MeshTriangles ts = null;
        for (int i = 0; i < meshTriangles.Count; i++)//0-39,40-119
        {
            if (i % count == 0)
            {
                ts = new MeshTriangles();
                ts.isSubT = true;
                list.Add(ts);
            }
            MeshTriangle t = meshTriangles.GetTriangle(i);
            ts.AddTriangle(t);
        }

        //foreach(MeshTriangles item in list)
        //{
        //    item.GenerateNewMesh();
        //}
        return list; 
    }

    public static void DebugShowTriangles(GameObject target,float PointScale)
    {
        Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        Debug.Log($"GetElbowInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        meshTriangles.ShowTriangles(target.transform, PointScale);
        meshTriangles.Dispose();
    }

    //public static PipeLineInfoList ShowPartLines(GameObject target, float PointScale, int count, bool isShowDebugObj)
    //{
    //    Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;
    //    MeshTriangles meshTriangles = new MeshTriangles(mesh);

    //    Debug.Log($"ShowPartLines mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
    //    //meshTriangles.ShowTriangles(target.transform, PointScale);
    //    List<MeshTriangles> subTriangles = meshTriangles.Split(count);
    //    PipeLineInfoList lineInfoList = new PipeLineInfoList();
    //    for (int i = 0; i < subTriangles.Count; i++)
    //    {
    //        GameObject tsRoot = CreateSubTestObj($"Parts {i + 1}({mesh.vertexCount})", target.transform);

    //        MeshTriangles subT = subTriangles[i];
    //        LogTag.logTag = tsRoot.name;
    //        PipeLineInfo lineInfo = subT.ShowTrianglesWithObb(tsRoot.transform, PointScale, $"[{i + 1}]", isShowDebugObj);
    //        lineInfoList.Add(lineInfo);
    //    }
    //    foreach (var subT in subTriangles)
    //    {
    //        subT.Dispose();
    //    }
    //    meshTriangles.Dispose();
    //    return lineInfoList;
    //}

    //public static PipeLineInfoList GetPartLinesInfo_Job(GameObject target, float PointScale, int count, bool isShowDebugObj)
    //{
    //    Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;
    //    MeshTriangles meshTriangles = new MeshTriangles(mesh);


    //    //meshTriangles.ShowTriangles(target.transform, PointScale);
    //    List<MeshTriangles> subTriangles = meshTriangles.Split(count);

    //    //Debug.Log($"GetPartLinesInfo_Job mesh vertexCount:{mesh.vertexCount} MeshTriangles:{mesh.triangles.Length} meshTriangles:{meshTriangles.Count} count:{count} subTriangles:{subTriangles.Count}");

    //    PipeBendPartInfoJob.logTag = target.name;
    //    PipeBendPartInfoJob.Result = new NativeArray<PipeLineData>(subTriangles.Count, Allocator.Persistent);
    //    PipeBendPartInfoJob.ErrorIds = new NativeList<int>(Allocator.Persistent);
    //    JobList<PipeBendPartInfoJob> jobs = new JobList<PipeBendPartInfoJob>(100);
    //    for (int i = 0; i < subTriangles.Count; i++)
    //    {
    //        MeshTriangles subT = subTriangles[i];
    //        Vector3[] vs = subT.GetPoints().ToArray();

    //        PipeBendPartInfoJob job = new PipeBendPartInfoJob()
    //        {
    //            id = i,
    //            meshTriangles = new Unity.Collections.NativeArray<MeshTriangle>(subT.Triangles.ToArray(), Unity.Collections.Allocator.Persistent)
    //        };
    //        jobs.Add(job);
    //    }
    //    jobs.CompleteAll();

    //    PipeLineInfoList lineInfoList = new PipeLineInfoList();
    //    for (int i = 0; i < subTriangles.Count; i++)
    //    {
    //        PipeLineData lineData = PipeBendPartInfoJob.Result[i];
    //        PipeLineInfo lineInfo = new PipeLineInfo(lineData, null);
    //        lineInfoList.Add(lineInfo);
    //    }

    //    PipeBendPartInfoJob.Result.Dispose();
    //    PipeBendPartInfoJob.ErrorIds.Dispose();

    //    foreach (var subT in subTriangles)
    //    {
    //        //subT.Dispose(); 
    //    }
    //    meshTriangles.Dispose();
    //    jobs.Dispose();

    //    return lineInfoList;
    //}

    private void Init(MeshStructure mesh)
    {
        this.mesh = mesh;
        var triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int pi1 = triangles[i];
            int pi2 = triangles[i + 1];
            int pi3 = triangles[i + 2];
            MeshPoint mp1 = GetMeshPoint(mesh, pi1);
            MeshPoint mp2 = GetMeshPoint(mesh, pi2);
            MeshPoint mp3 = GetMeshPoint(mesh, pi3);

            MeshTriangle triangle = new MeshTriangle(mp1, mp2, mp3);
            Triangles.Add(triangle);
        }

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            center += mesh.vertices[i];
        }
        center /= mesh.vertexCount;
    }

        public MeshTriangles(MeshStructure mesh)
    {
        Init(mesh);
    }

    public MeshTriangles(List<MeshTriangle> triangles)
    {
        foreach(var tri in triangles)
        {
            if (Triangles.Contains(tri)) continue;
            Triangles.Add(tri);
        }
        
    }

    private MeshPoint GetMeshPoint(MeshStructure mesh, int pi1)
    {
        Vector3 p1 = mesh.vertices[pi1];
        Vector3 n1 = mesh.normals[pi1];
        MeshPoint mp1 = new MeshPoint(p1, pi1, n1);
        return mp1;

    }

    public List<Key2List<Vector3, MeshTriangle>> FindSharedPointsByPoint()
    {
        DictionaryList1ToN<Vector3, MeshTriangle> sharedPoints = new DictionaryList1ToN<Vector3, MeshTriangle>();
        for (int i = 0; i < Triangles.Count; i++)
        {
            MeshTriangle t1 = Triangles[i];
            var ps = t1.GetPoints();
            foreach (var p in ps)
            {
                sharedPoints.AddItem(p.Point, t1);
            }
        }
        List<Key2List<Vector3, MeshTriangle>> list = sharedPoints.GetListSortByCount();
        return list;
    }



    public List<Vector3> GetPoints()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < Triangles.Count; i++)
        {
            MeshTriangle t1 = Triangles[i];
            var ps = t1.GetPoints();
            foreach (var p in ps)
            {
                if (points.Contains(p.Point)) continue;
                points.Add(p.Point);
            }
        }
        return points;
    }

    public List<Vector3> GetPoints(float minDis)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < Triangles.Count; i++)
        {
            if (i == 0)
            {
                MeshTriangle t1 = Triangles[i];
                var ps = t1.GetPoints();
                foreach (var p in ps)
                {
                    //if (points.Contains(p.Point)) continue;
                    points.Add(p.Point);
                }
            }
            else
            {
                MeshTriangle t1 = Triangles[i];
                var ps = t1.GetPoints();
                foreach (var p in ps)
                {
                    float dis = DistanceUtil.GetMinDistance(p.Point, points);
                    if (dis >= minDis)
                    {
                        points.Add(p.Point);
                    }
                    //
                }
            }
        }
        return points;
    }

    public List<Key2List<Vector3, MeshTriangle>> FindSharedPointsByPoint(float minDis)
    {
        DictionaryList1ToN<Vector3, MeshTriangle> sharedPoints = new DictionaryList1ToN<Vector3, MeshTriangle>();

        var points = this.GetPoints(minDis);
        for (int i1 = 0; i1 < points.Count; i1++)
        {
            Vector3 point = points[i1];
            for (int i = 0; i < Triangles.Count; i++)
            {
                MeshTriangle t1 = Triangles[i];
                //var ps = t1.GetPoints();
                //foreach (var p in ps)
                //{
                //    float dis = Vector3.Distance(p.Point, point);
                //    if (dis < minDis)
                //    {
                //        sharedPoints.AddItem(p.Point, t1);
                //        break;
                //    }
                //}
                float dis = t1.GetDistanceOfPoints(point);
                //Debug.Log($"【{dis<minDis}】FindSharedPointsByPoint p[{i1}][{i}] t1:{t1.Id} dis:{dis} minDis:{minDis}");
                if(dis < minDis)
                {
                    sharedPoints.AddItem(point, t1);
                }
                
            }
            //break;
        }

        List<Key2List<Vector3, MeshTriangle>> list = sharedPoints.GetListSortByCount();
        return list;
    }

    public List<Key2List<string, MeshTriangle>> FindSharedPointsByPoint(int pt)
    {
        PositionDictionaryList<MeshTriangle> posDict = new PositionDictionaryList<MeshTriangle>();

        DictionaryList1ToN<Vector3, MeshTriangle> sharedPoints = new DictionaryList1ToN<Vector3, MeshTriangle>();
        for (int i = 0; i < Triangles.Count; i++)
        {
            MeshTriangle t1 = Triangles[i];
            var ps = t1.GetPoints();
            foreach (var p in ps)
            {
                posDict.Add(p.Point, t1);
                sharedPoints.AddItem(p.Point, t1);
            }
        }
        List<Key2List<string, MeshTriangle>> list1 = null;
        if (pt == 6)
        {
            list1 = posDict.posListDict2.GetListSortByCount();
        }
        else if (pt == 5)
        {
            list1 = posDict.posListDict3.GetListSortByCount();
        }
        else if (pt == 4)
        {
            list1 = posDict.posListDict4.GetListSortByCount();
        }
        else if (pt == 3)
        {
            list1 = posDict.posListDict5.GetListSortByCount();
        }
        else
        {
            list1 = posDict.posListDict.GetListSortByCount();
        }

        //List<Key2List<Vector3, MeshTriangle>> list = sharedPoints.GetListSortByCount();
        return list1;
    }

    public void ShowSharedPointsByPoint(Transform root, float pointScale, int minCount)
    {
        List<Key2List<Vector3, MeshTriangle>> sharedPoints2 = this.FindSharedPointsByPoint();
        Debug.LogError($"GetElbowInfo sharedPoints2:{sharedPoints2.Count}");
        GameObject sharedPoints1Obj2 = CreateSubTestObj($"sharedPoints(Point):{sharedPoints2.Count}", root);
        int id = 0;
        for (int i = 0; i < sharedPoints2.Count; i++)
        {
            Vector3 point = sharedPoints2[i].Key;
            List<MeshTriangle> triangles = sharedPoints2[i].List;
            if (triangles.Count < minCount) continue;
            id++;

            Dictionary<Vector3, int> pointCount = new Dictionary<Vector3, int>();

            Debug.Log($"GetElbowInfo sharedPoints2[{i + 1}/{sharedPoints2.Count}] point:{point} trianlges:{triangles.Count}");
            GameObject trianglesObj2 = CreateSubTestObj($"triangles[{id}]:{triangles.Count}", sharedPoints1Obj2.transform);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                GameObject objTriangle = t.ShowTriangle(root, trianglesObj2.transform, pointScale);
                objTriangle.name = $"triangle[{i1 + 1}]";
            }

            PointHelper.ShowLocalPoint(point, pointScale * 2, root, trianglesObj2.transform);
        }

        List<Key2List<string, MeshTriangle>> sharedPointsEx = this.FindSharedPointsByPoint(4);
    }

    public List<List<Vector3>> GetLinePoints(Transform root, float pointScale, int minCount)
    {
        MeshPointCountDictList lineCountList = new MeshPointCountDictList();
        List<Key2List<Vector3, MeshTriangle>> sharedPoints2 = this.FindSharedPointsByPoint();
        //Debug.LogError($"GetLinePoints sharedPoints2:{sharedPoints2.Count}");
        //GameObject sharedPoints1Obj2 = CreateSubTestObj($"sharedPoints(Point):{sharedPoints2.Count}", root);
        int id = 0;
        for (int i = 0; i < sharedPoints2.Count; i++)
        {
            Vector3 point = sharedPoints2[i].Key;
            List<MeshTriangle> triangles = sharedPoints2[i].List;
            if (triangles.Count < minCount) continue;
            id++;

            MeshPointCountDict pointCount = new MeshPointCountDict(point);
            lineCountList.Add(pointCount);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                List<MeshPoint> mps = t.GetPoints();
                foreach (MeshPoint mp in mps)
                {
                    pointCount.AddPoint(mp);
                }
            }
        } 
        List<List<Vector3>> pointLines = lineCountList.GetLines();
        GameObject linesObj = CreateSubTestObj($"PointLines:{pointLines.Count}", root);
        for (int i = 0; i < pointLines.Count; i++)
        {
            List<Vector3> ps = pointLines[i];
            GameObject lineObj = CreateSubTestObj($"PointLine[{i}]:{ps.Count}", linesObj.transform);
            for (int j = 0; j < ps.Count; j++)
            {
                Vector3 p = ps[j];
                PointHelper.ShowLocalPoint(p, pointScale * 2, root, lineObj.transform);
            } 
        }
        return pointLines;
    }

    public void ShowSharedPointsByPointExEx(Transform root, float pointScale, int minCount,float minDis)
    {
        Debug.Log($"ShowSharedPointsByPointExEx minDis:{minDis}");
        //SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByPointEx(minCount,minDis);
        SharedMeshTrianglesList sharedPoints1 = this.GetSharedMeshTrianglesListById(0, minDis, "ShowSharedPointsByPointExEx");
        sharedPoints1.SortByCount();

        sharedPoints1.RemoveSamePoints(minDis);

        var circle1 = sharedPoints1[0];
        sharedPoints1.RemoveAt(0);
        var circle2 = sharedPoints1[0];
        sharedPoints1.RemoveAt(0);

        sharedPoints1.CombineSamePoint(minDis);

        var ts=sharedPoints1.GetAllTriangles();
        MeshTriangles meshTriangles2 = new MeshTriangles(ts);
        meshTriangles2.ShowTriangles(root, pointScale);

        SharedMeshTrianglesList sharedPoints2 = meshTriangles2.GetSharedMeshTrianglesListByPoint(minCount, minDis);
        //sharedPoints2.CombineSamePoint(minDis);
        if (sharedPoints2.Count != 1)
        {
            Debug.LogError($"ShowSharedPointsByPointExEx sharedPoints2.Count != 1 :{sharedPoints2.Count}");
        }

        if(sharedPoints2.Count>0)
        {
            sharedPoints2[0].SetCenterWithOutPoint(minDis);
        }

        sharedPoints2.Add(circle1);
        sharedPoints2.Add(circle2);
        ShowSharedMeshTrianglesList(root, pointScale, 0,int.MaxValue, sharedPoints2, true);
    }

    public SharedMeshTrianglesList GetWeldoletKeyPoints(int minCount, float minDis)
    {
        
        //SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByPointEx(minCount,minDis);
        SharedMeshTrianglesList sharedPoints1 = this.GetSharedMeshTrianglesListById(0, minDis, "GetWeldoletKeyPoints");
        sharedPoints1.SortByCount();

        sharedPoints1.RemoveSamePoints(minDis);

        var circle1 = sharedPoints1[0];
        sharedPoints1.RemoveAt(0);
        var circle2 = sharedPoints1[0];
        sharedPoints1.RemoveAt(0);

        sharedPoints1.CombineSamePoint(minDis);

        var ts = sharedPoints1.GetAllTriangles();
        MeshTriangles meshTriangles2 = new MeshTriangles(ts);
        //meshTriangles2.ShowTriangles(root, pointScale);

        SharedMeshTrianglesList sharedPoints2 = meshTriangles2.GetSharedMeshTrianglesListByPoint(minCount, minDis);
        //sharedPoints2.CombineSamePoint(minDis);
        if (sharedPoints2.Count > 2)
        {
            Debug.LogError($"GetWeldoletKeyPoints sharedPoints2.Count > 2 :{sharedPoints2.Count}");
        }
        if (circle1.Radius > circle2.Radius)
        {
            sharedPoints2.Add(circle1);
            sharedPoints2.Add(circle2);
        }
        else
        {
            sharedPoints2.Add(circle2);
            sharedPoints2.Add(circle1);
        }
        
        //ShowSharedMeshTrianglesList(root, pointScale, 0, sharedPoints2, false);

        Debug.Log($"GetWeldoletKeyPoints minDis:{minDis} sharedPoints1:{sharedPoints1.Count} sharedPoints2:{sharedPoints2.Count}");
        return sharedPoints2;
    }

    public List<Key2List<int, MeshTriangle>> sharedPointsById = null;

    public List<Key2List<int, MeshTriangle>> FindSharedPointsById()
    {
        if (sharedPointsById == null)
        {
            DictionaryList1ToN<int, MeshTriangle> sharedPoints = new DictionaryList1ToN<int, MeshTriangle>();
            for (int i = 0; i < Triangles.Count; i++)
            {
                MeshTriangle t1 = Triangles[i];
                var ps = t1.GetPoints();
                foreach (var p in ps)
                {
                    sharedPoints.AddItem(p.Id, t1);
                }
            }
            sharedPointsById = sharedPoints.GetListSortByCount();
        }

        return sharedPointsById;
    }

    public List<Key2List<string, MeshTriangle>> sharedPointsByNormal2 = null;

    public List<Key2List<string, MeshTriangle>> FindSharedPointsByNormal2()
    {
        if (sharedPointsByNormal2 == null)
        {
            DictionaryList1ToN<string, MeshTriangle> sharedPoints = new DictionaryList1ToN<string, MeshTriangle>();
            for (int i = 0; i < Triangles.Count; i++)
            {
                MeshTriangle t1 = Triangles[i];
                var normal = t1.GetNormal();
                string n = normal.Vector3ToString5();
                sharedPoints.AddItem(n, t1);
            }
            sharedPointsByNormal2 = sharedPoints.GetListSortByCount();
        }

        return sharedPointsByNormal2;
    }

    public List<Key2List<string, MeshPoint>> sharedPointsByNormal = null;

    public List<Key2List<string, MeshPoint>> FindSharedPointsByNormal()
    {
        if (sharedPointsByNormal == null)
        {
            DictionaryList1ToN<string, MeshPoint> sharedPoints = new DictionaryList1ToN<string, MeshPoint>();
            for (int i = 0; i < Triangles.Count; i++)
            {
                MeshTriangle t1 = Triangles[i];
                var ps = t1.GetPoints();
                foreach (var p in ps)
                {
                    string n = p.Normal.Vector3ToString5();
                    sharedPoints.AddItem(n, p);
                }
            }
            sharedPointsByNormal = sharedPoints.GetListSortByCount();
        }

        return sharedPointsByNormal;
    }

    public void ShowSharedPointsById(Transform root, float pointScale,int minCount)
    {
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        Debug.LogError($"GetElbowInfo sharedPoints1:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"sharedPoints(Id):{sharedPoints1.Count}", root);
        int id = 0;
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            if (triangles.Count < minCount) continue;
            id++;
            Debug.Log($"GetElbowInfo sharedPoints1[{i + 1}/{sharedPoints1.Count}] point:{pointId} trianlges:{triangles.Count}");
            GameObject trianglesObj = CreateSubTestObj($"triangles[{id}][id:{pointId}]({triangles.Count})", sharedPoints1Obj.transform);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                GameObject objTriangle = t.ShowTriangle(root, trianglesObj.transform, pointScale);
                objTriangle.name = $"triangle[{i1 + 1}]";
            }
            Vector3 point = mesh.vertices[pointId];
            PointHelper.ShowLocalPoint(point, pointScale * 2, root, trianglesObj.transform);
        }
    }

    //    public void ShowPointGroups(Transform root, float pointScale, int minCount, int maxCount, float minDis)
    //    {
    //        /*
    //         * 1 找到空间中某点p10，有kdTree找到离他最近的n个点，判断这n个点到p的距离。将距离小于阈值r的点p12,p13,p14....放在类Q里
    //2 在 Q(p10) 里找到一点p12,重复1

    //3 在 Q(p10,p12) 找到一点，重复1，找到p22,p23,p24....全部放进Q里

    //4 当 Q 再也不能有新点加入了，则完成搜索了
    //————————————————
    //版权声明：本文为CSDN博主「hxxjxw」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
    //原文链接：https://blog.csdn.net/hxxjxw/article/details/112689489
    //         */

    //    }

    public void ShowNormalPoints(Transform root, float pointScale, int minCount, int maxCount, float minDis)
    {
        var planes = this.GetPlaneListByNormal(minCount, minDis, root.name);
        planes.Sort((a, b) => { return b.PointList.Count.CompareTo(a.PointList.Count); });
        GameObject sharedPoints1Obj = CreateSubTestObj($"NormalPoints_({minCount}_{maxCount}):{planes.Count}", root);
        int count = 0;
        //sharedPoints1.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });
        for (int i = 0; i < planes.Count; i++)
        {
            var plane = planes[i];
            count++;
            ShowNormalPoints(root, pointScale, sharedPoints1Obj, count, plane, true);
        }
        sharedPoints1Obj.name = $"sharedPoints(Id)_({minCount}_{maxCount}):{count}";
    }

    private void ShowNormalPoints(Transform root, float pointScale, GameObject sharedPoints1Obj, int id, MeshPlane plane, bool isShowCenter)
    {
        var pointId = plane.Normal;
        //Debug.Log($"GetElbowInfo sharedPoints1[{i + 1}/{sharedPoints1.Count}] point:{pointId} trianlges:{triangles.Count}");
        GameObject trianglesObj = CreateSubTestObj($"triangles[{id}]({plane.PointCount})_{plane.Key}_{plane.Normal.Vector3ToString()}", sharedPoints1Obj.transform);

        //SharedMeshTrianglesComponent triangleComponent = trianglesObj.AddComponent<SharedMeshTrianglesComponent>();

        //for (int i1 = 0; i1 < plane.PointCount; i1++)
        //{
        //    var t = plane.PointList[i1];
        //    GameObject objTriangle = t.ShowTriangle(root, trianglesObj.transform, pointScale);
        //    objTriangle.name = $"triangle[{i1 + 1}]({t.Id})";
        //}

        for (int j = 0; j < plane.PointCount; j++)
        {
            MeshPoint p = plane.PointList[j];

            PointHelper.ShowLocalPoint(p.Point, pointScale, root, trianglesObj.transform).name = $"Point[{p.Id}]({p.Point.Vector3ToString()})({p.Normal.Vector3ToString()})";
        }

        //Vector3 point = mesh.vertices[pointId];
        //TransformHelper.ShowLocalPoint(plane.Point, pointScale * 2, root, trianglesObj.transform).name = "Point";
        if (isShowCenter)
            PointHelper.ShowLocalPoint(plane.Center, pointScale * 2, root, trianglesObj.transform).name = "Center";
    }

    public void ShowNormalPlanes(Transform root, float pointScale, int minCount, int maxCount, float minDis)
    {
        SharedMeshTrianglesList sharedPoints1 = this.GetSharedMeshTrianglesListByNormal(minCount, minDis, root.name);
        sharedPoints1.SortByCount();
        //Debug.Log($"ShowSharedMeshTrianglesList sharedPoints1:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"NormalPlanes(Id)_({minCount}_{maxCount}):{sharedPoints1.Count}", root);
        int count = 0;
        //sharedPoints1.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            SharedMeshTriangles plane = sharedPoints1[i];
            var triangles = plane.GetAllTriangles();
            //plane.GetLines();
            if (triangles.Count < minCount) continue;
            count++;
            ShowSharedMeshTrianglesList(root, pointScale, sharedPoints1Obj, count, plane, triangles, true);
        }
        sharedPoints1Obj.name = $"sharedPoints(Id)_({minCount}_{maxCount}):{count}";
    }

    public void ShowCirclesById(Transform root, float pointScale, int minCount, int maxCount, float minDis)
    {
        SharedMeshTrianglesList sharedPoints1 = this.GetSharedMeshTrianglesListById(minCount, minDis, "ShowCirclesById");
        sharedPoints1.SortByCount();
        //Debug.Log($"ShowSharedMeshTrianglesList sharedPoints1:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"sharedPoints(Id)_({minCount}_{maxCount}):{sharedPoints1.Count}", root);
        int count = 0;
        //sharedPoints1.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            SharedMeshTriangles plane = sharedPoints1[i];
            var triangles = plane.GetAllTriangles();
            //plane.GetLines();
            if (triangles.Count < minCount) continue;
            count++;
            ShowSharedMeshTrianglesList(root, pointScale, sharedPoints1Obj, count, plane, triangles, true);
        }
        sharedPoints1Obj.name = $"sharedPoints(Id)_({minCount}_{maxCount}):{count}";
    }

    public void ShowSharedPointsByIdEx(Transform root, float pointScale, int minCount, int maxCount, float minDis)
    {
        //SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByPointEx(minCount,minDis);
        SharedMeshTrianglesList sharedPoints1 = this.GetSharedMeshTrianglesListById(minCount, minDis, "ShowSharedPointsByIdEx");
        ShowSharedMeshTrianglesList(root, pointScale, minCount, maxCount, sharedPoints1,true);
    }

    public void ShowSharedMeshTrianglesList(Transform root, float pointScale, int minCount,int maxCount, SharedMeshTrianglesList sharedPoints1, bool isShowCenter)
    {
        sharedPoints1.SortByCount();
        //Debug.Log($"ShowSharedMeshTrianglesList sharedPoints1:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"sharedPoints(Id)_({minCount}_{maxCount}):{sharedPoints1.Count}", root);
        int count = 0;
        //sharedPoints1.Sort((a, b) => { return b.TriangleCount.CompareTo(a.TriangleCount); });
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            SharedMeshTriangles plane = sharedPoints1[i];
            var triangles = plane.GetAllTriangles();
            if (triangles.Count < minCount || triangles.Count>maxCount) continue;

            count++;
            ShowSharedMeshTrianglesList(root, pointScale, sharedPoints1Obj, count, plane, triangles, isShowCenter);
        }
        sharedPoints1Obj.name = $"sharedPoints(Id)_({minCount}_{maxCount}):{count}";
    }

    private void ShowSharedMeshTrianglesList(Transform root, float pointScale, GameObject sharedPoints1Obj, int id, SharedMeshTriangles plane, MeshTriangleList triangles,bool isShowCenter)
    {
        int pointId = plane.PointId;
        //Debug.Log($"GetElbowInfo sharedPoints1[{i + 1}/{sharedPoints1.Count}] point:{pointId} trianlges:{triangles.Count}");
        GameObject trianglesObj = CreateSubTestObj($"triangles[{id}][id:{pointId}]({triangles.Count})_[{plane.MinRadius},{plane.Radius}]_{plane.IsCircle}_{plane.CircleCheckP}_{plane.Normal.Vector3ToString()}", sharedPoints1Obj.transform);
        SharedMeshTrianglesComponent triangleComponent=trianglesObj.AddComponent<SharedMeshTrianglesComponent>();
        triangleComponent.SetData(plane);

        for (int i1 = 0; i1 < triangles.Count; i1++)
        {
            MeshTriangle t = triangles[i1];
            GameObject objTriangle = t.ShowTriangle(root, trianglesObj.transform, pointScale);
            objTriangle.name = $"triangle[{i1 + 1}]({t.GetId()})";
        }
        Vector3 point = mesh.vertices[pointId];
        PointHelper.ShowLocalPoint(plane.Point, pointScale * 2, root, trianglesObj.transform).name = "Point";
        if(isShowCenter)
            PointHelper.ShowLocalPoint(plane.Center, pointScale * 2, root, trianglesObj.transform).name = "Center";
    }

    public void ShowSharedPointsByPointEx(Transform root, float pointScale, int minCount, float minDis,string tag)
    {
        SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByPointEx(minCount, minDis, tag);
        //SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByIdEx(minCount, minDis);
        Debug.LogError($"ShowSharedPointsByPointEx sharedPoints2:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"sharedPoints(Point):{sharedPoints1.Count}", root);
        int id = 0;
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            SharedMeshTriangles plane = sharedPoints1[i];
            int pointId = plane.PointId;
            var triangles = plane.GetAllTriangles();
            if (triangles.Count < minCount) continue;
            id++;
            Debug.Log($"GetElbowInfo sharedPoints1[{i + 1}/{sharedPoints1.Count}] point:{pointId} trianlges:{triangles.Count}");
            GameObject trianglesObj = CreateSubTestObj($"triangles[{id}][id:{pointId}]({triangles.Count})_({plane.MinRadius}-{plane.Radius})", sharedPoints1Obj.transform);

            SharedMeshTrianglesComponent component = trianglesObj.AddComponent<SharedMeshTrianglesComponent>();
            component.sharedMeshTriangles = plane;

            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                GameObject objTriangle = t.ShowTriangle(root, trianglesObj.transform, pointScale);
                objTriangle.name = $"triangle[{i1 + 1}]";
            }

            var ps = triangles.GetPoints();
            GameObject psObj = CreateSubTestObj($"ps[{i + 1}]({ps.Count})", sharedPoints1Obj.transform);
            for (int i1 = 0; i1 < ps.Count; i1++)
            {
                var p = ps[i1];
                PointHelper.ShowLocalPoint(p, pointScale, root, psObj.transform).name = $"Point[{i1}]({p})";
            }

            Vector3 point = mesh.vertices[pointId];
            PointHelper.ShowLocalPoint(plane.Point, pointScale * 2, root, trianglesObj.transform).name = "Point";
            PointHelper.ShowLocalPoint(plane.Center, pointScale * 2, root, trianglesObj.transform).name = "Center";
        }
    }

    private static GameObject CreateSubTestObj(string objName, Transform parent)
    {
        return DebugInfoRoot.CreateSubTestObj(objName, parent);
    }

    public List<MeshTriangle> GetTriangles(int pointId)
    {
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        var triangles = sharedPoints1[pointId].List;
        return triangles;
    }

    public float GetRadius(int pointId)
    {
        var triangles = GetTriangles(pointId);
        float radius = GetTrianglesRadius(triangles, pointId);
        return radius;
    }

    public List<Vector3> GetKeyPointsById(int minCount,float minDis)
    {
        List<Vector3> KeyPoints = new List<Vector3>();
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Vector3 point = mesh.vertices[pointId];
            if (triangles.Count >= minCount)
            {
                if (!KeyPoints.Contains(point))
                {
                    float dis = Distance2List(point, KeyPoints);
                    if(dis> minDis)
                    {
                        //Debug.Log($"dis:{dis} point:{point} KeyPoints:{KeyPoints.Count}");
                        KeyPoints.Add(point);
                    }
                }
            }
        }
        if (KeyPoints.Count <2)
        {
            Debug.LogError($"GetKeyPoints KeyPoints:{KeyPoints.Count}");
        }
        return KeyPoints;
    }

    public SharedMeshTrianglesList GetKeyPointsByPointEx(int minCount, float minDis,string tag)
    {
        SharedMeshTrianglesList KeyPoints = new SharedMeshTrianglesList();
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        

        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Vector3 point = mesh.vertices[pointId];
            Vector3 normal = mesh.normals[pointId];



            SharedMeshTriangles? item = KeyPoints.FindItemByPoint(point, minDis);
            if (item == null)
            {
                KeyPoints.Add(new SharedMeshTriangles(pointId, point, normal, triangles));
            }
            else
            {
                ((SharedMeshTriangles)item).AddOtherTriangles(triangles);
            }
        }

        SharedMeshTrianglesList KeyPoints2 = new SharedMeshTrianglesList();
        foreach(var item in KeyPoints)
        {
            if (item.GetAllTriangles().Count >= minCount)
            {
                item.GetInfo();
                KeyPoints2.Add(item);
            }
        }

        //Debug.Log($"GetKeyPointsByPointEx minCount:{minCount} minDis:{minDis} sharedPoints1:{sharedPoints1.Count} KeyPoints:{KeyPoints.Count} KeyPoints2:{KeyPoints2.Count}");

        if (KeyPoints2.Count < 2)
        {
            Debug.LogError($"GetKeyPoints({tag}) KeyPoints:{KeyPoints2.Count}");
        }

        //KeyPoints.CombineSameCenter(minDis);

        KeyPoints2.Sort();
        return KeyPoints2;
    }

    public static int sharedMinCount = 32;
    public static float minRepeatPointDistance = 0.00005f;
    public static float PipeLineOffset = 0.05f;

    public SharedMeshTrianglesList GetSharedMeshTrianglesListById(string tag)
    {
        return GetSharedMeshTrianglesListById(sharedMinCount, minRepeatPointDistance, tag);
    }
    public MeshPlaneList GetPlaneListByNormal(int minCount, float minDis,string name)
    {
        MeshPlaneList trianglesList = new MeshPlaneList();
        List<Key2List<string, MeshPoint>> sharedPoints1 = this.FindSharedPointsByNormal();
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            string normal = sharedPoints1[i].Key;
            var mpList = sharedPoints1[i].List;
            //Vector3 point = mesh.vertices[pointId];
            //Vector3 normal = mesh.normals[pointId];
            if (mpList.Count >= minCount)
            {
                MeshPlane plane = new MeshPlane();
                plane.Key = normal;
                //plane.Normal = normal;
                plane.AddPoints(mpList);
                trianglesList.Add(plane);
            }
            //KeyPoints.Add(plane);
        }

        trianglesList.CombineByNormal(minDis, name);
        //trianglesList.CombineSameCircle(minDis);

        //if (trianglesList.Count < 2)
        //{
        //    Debug.LogError($"GetKeyPoints KeyPoints:{trianglesList.Count}");
        //}

        trianglesList.Sort();
        return trianglesList;
    }

    public SharedMeshTrianglesList GetSharedMeshTrianglesListByNormal(int minCount, float minDis,string name)
    {
        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList();
        List<Key2List<string, MeshTriangle>> sharedPoints1 = this.FindSharedPointsByNormal2();
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            string key = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Vector3 point = Vector3.zero;
            Vector3 normal = triangles[0].GetNormal();
            if (triangles.Count >= minCount)
            {
                SharedMeshTriangles plane = new SharedMeshTriangles(0, point, normal, triangles,false);
                trianglesList.Add(plane);
            }
            //KeyPoints.Add(plane);
        }

        trianglesList.CombineSameNormal(minDis, name);
        //trianglesList.CombineSameCircle(minDis);

        if (trianglesList.Count < 2)
        {
            Debug.LogError($"GetKeyPoints KeyPoints:{trianglesList.Count}");
        }

        trianglesList.Sort();
        return trianglesList;
    }

    public SharedMeshTrianglesList GetSharedMeshTrianglesListById(int minCount, float minDis,string tag)
    {
        SharedMeshTrianglesList trianglesList = new SharedMeshTrianglesList();
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Vector3 point = mesh.vertices[pointId];
            Vector3 normal = mesh.normals[pointId];
            if (triangles.Count >= minCount)
            {
                SharedMeshTriangles plane = new SharedMeshTriangles(pointId, point, normal, triangles);
                trianglesList.Add(plane);
            }
            //KeyPoints.Add(plane);
        }

        trianglesList.CombineSameMesh(minDis);
        trianglesList.CombineSameCircle(minDis); 

        //Debug.Log($"GetSharedMeshTrianglesListById({tag}) sharedPoints1:{sharedPoints1.Count} KeyPoints:{trianglesList.Count} minCount:{minCount} minDis:{minDis}");
        if (trianglesList.Count < 2)
        {
            Debug.LogError($"GetSharedMeshTrianglesListById({tag}) sharedPoints1:{sharedPoints1.Count} KeyPoints:{trianglesList.Count} minCount:{minCount} minDis:{minDis}");
        }

        trianglesList.Sort();
        return trianglesList;
    }

    public SharedMeshTrianglesList GetSharedMeshTrianglesListByPoint(int minCount, float minDis)
    {
        SharedMeshTrianglesList KeyPoints = new SharedMeshTrianglesList();
        List<Key2List<Vector3, MeshTriangle>> sharedPoints1 = this.FindSharedPointsByPoint(minDis);
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            Vector3 point = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            //Vector3 point = mesh.vertices[pointId];
            Vector3 normal = triangles[0].p1.Normal;
            int pointId = triangles[0].p1.Id;
            if (triangles.Count >= minCount)
            {
                SharedMeshTriangles plane = new SharedMeshTriangles(pointId, point, normal, triangles);
                KeyPoints.Add(plane);
            }
            //KeyPoints.Add(plane);
        }
        Debug.Log($"GetSharedMeshTrianglesListByPoint minCount:{minCount} minDis:{minDis} sharedPoints1：{sharedPoints1.Count} KeyPoints:{KeyPoints.Count}");
        KeyPoints.CombineSameMesh(minDis);
        KeyPoints.CombineSameCircle(minDis);

        //if (KeyPoints.Count < 2)
        //{
        //    Debug.LogError($"GetSharedMeshTrianglesListByPoint KeyPoints.Count < 2 KeyPoints:{KeyPoints.Count}");
        //}

        KeyPoints.Sort();
        return KeyPoints;
    }

    private float GetTrianglesRadius(List<MeshTriangle> triangles,int pointId)
    {
        float radius = 0;
        int count = 0;
        foreach (MeshTriangle triangle in triangles)
        {
            float r = triangle.GetRadius(pointId);
            if (r == 0) continue;
            count++;
            radius += r;
        }
        radius /= count;
        return radius;
    }

    //public float GetPipeRadius(int minCount)
    //{
    //    float avgRadius = 0;
    //    int count = 0;
    //    List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
    //    for (int i = 0; i < sharedPoints1.Count; i++)
    //    {
    //        int pointId = sharedPoints1[i].Key;
    //        var triangles = sharedPoints1[i].List;
    //        Vector3 point = mesh.vertices[pointId];
    //        if (triangles.Count >= minCount)
    //        {
    //            float radius = GetTrianglesRadius(triangles, pointId);
    //            //Debug.Log($"point:{point} radius:{radius}");
    //            avgRadius += radius;
    //            count++;
    //        }
    //    }
    //    avgRadius /= count;
    //    //Debug.Log($"GetPipeRadius avgRadius:{avgRadius}");
    //    return avgRadius;
    //}

    public float Distance2List(Vector3 p,List<Vector3> list)
    {
        float minDis = float.MaxValue;
        foreach(var item in list)
        {
            float dis = Vector3.Distance(item, p);
            if (dis < minDis)
            {
                minDis = dis;
            }
        }
        return minDis;
    }

    public float Distance2List(Vector3 p, SharedMeshTrianglesList list)
    {
        float minDis = float.MaxValue;
        foreach (var item in list)
        {
            //float dis = Vector3.Distance(item.GetCenter(), p);
            float dis = Vector3.Distance(item.Point, p);
            if (dis < minDis)
            {
                minDis = dis;
            }
        }
        return minDis;
    }

    public float Distance2List(Vector3 p, MeshPointList list)
    {
        float minDis = float.MaxValue;
        foreach (var item in list)
        {
            float dis = Vector3.Distance(item.Point, p);
            if (dis < minDis)
            {
                minDis = dis;
            }
        }
        return minDis;
    }

    public void ShowKeyPointsById(Transform root, float pointScale, int minCount, float minDis,bool isCombineSameCenter)
    {
        PointHelper.ShowLocalPoint(center, pointScale, root, null).name="MeshCenter";
        PointHelper.ShowLocalPoint(mesh.boundCenter, pointScale, root, null).name = "BoundsCenter";

        SharedMeshTrianglesList points = GetSharedMeshTrianglesListById(minCount, minDis, "ShowKeyPointsById");
        //SharedMeshTrianglesList points = GetKeyPointsByPointEx(minCount, minDis);
        Debug.Log($"GetKeyPoints KeyPoints1:{points.Count}");
        if (points.Count > 6)
        {
            points.RemoveNotCircle();
        }
        //SharedMeshTrianglesList points = GetKeyPointsByIdEx(36, 0);
        Debug.Log($"GetKeyPoints KeyPoints2:{points.Count} minCount:{minCount} minDis:{minDis}");
        if(isCombineSameCenter)
            points.CombineSameCenter(minDis);
        Debug.Log($"GetKeyPoints KeyPoints3:{points.Count}");
        GameObject keyPointsObj = CreateSubTestObj($"KeyPoints:{points.Count}", root);

        if (points.Count > 0)
        {
            //var centerOfPoints = VertexHelper.GetCenterOfList(points);
            var centerOfPoints = points.GetCenter();
            PointHelper.ShowLocalPoint(centerOfPoints, pointScale, root, null).name = "KeysCenter";

            for (int i = 0; i < points.Count; i++)
            {
                SharedMeshTriangles point = points[i];
                PointHelper.ShowLocalPoint(point.Normal, pointScale, root, keyPointsObj.transform).name = $"Key[{i + 1}]_Normal_{point.Normal}";
                PointHelper.ShowLocalPoint(point.Center, pointScale, root, keyPointsObj.transform).name = $"Key[{i + 1}]_Center_point.Center";
                PointHelper.ShowLocalPoint(point.Point, pointScale, root, keyPointsObj.transform).name = $"Key[{i + 1}]_Point_{point.IsCircle}_{point.DistanceToCenter}_{point.GetRadius()}";
            }
        }
        else
        {
            Debug.LogError($"GetKeyPoints  Count == 0 KeyPoints:{points.Count} root:{root}");
        }
        //points.Sort((a, b) => { })
    }
}
