using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshTriangles
{
    public MeshTriangleList Triangles = new MeshTriangleList();

    public int Count
    {
        get
        {
            return Triangles.Count;
        }
    }

    public MeshTriangles()
    {

    }


    public MeshTriangle GetTriangle(int id)
    {
        return Triangles[id];
    }

    public MeshStructure mesh;

    public Vector3 center = Vector3.zero;

    public MeshTriangles(Mesh mesh)
    {
        Init(new MeshStructure(mesh));
    }

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

    private MeshPoint GetMeshPoint(MeshStructure mesh, int pi1)
    {
        Vector3 p1 = mesh.vertices[pi1];
        Vector3 n1 = mesh.normals[pi1];
        MeshPoint mp1 = new MeshPoint(p1, pi1, n1);
        return mp1;

    }

    public List<Key2List<Vector3, MeshTriangle>> FindSharedPointsByPoint()
    {
        //List<Vector3> sharedPoints = new List<Vector3>();
        //for (int i = 0; i < Triangles.Count; i++)
        //{
        //    MeshTriangle t1 = Triangles[i];
        //    for(int j=1;j<Triangles.Count;j++)
        //    {
        //        MeshTriangle t2 = Triangles[j];
        //        List<Vector3> ps = t1.FindSharedPoints(t2);
        //    }
        //}

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

    public void ShowSharedPointsByPoint(Transform root, float pointScale, int minCount)
    {
        List<Key2List<Vector3, MeshTriangle>> sharedPoints2 = this.FindSharedPointsByPoint();
        Debug.LogError($"GetElbowInfo sharedPoints2:{sharedPoints2.Count}");
        GameObject sharedPoints1Obj2 = CreateSubTestObj($"sharedPoints(Point):{sharedPoints2.Count}", root);
        int id = 0;
        for (int i = 0; i < sharedPoints2.Count; i++)
        {
            var point = sharedPoints2[i].Key;
            var triangles = sharedPoints2[i].List;
            if (triangles.Count < minCount) continue;
            id++;
            Debug.Log($"GetElbowInfo sharedPoints2[{i + 1}/{sharedPoints2.Count}] point:{point} trianlges:{triangles.Count}");
            GameObject trianglesObj2 = CreateSubTestObj($"triangles[{id}]:{triangles.Count}", sharedPoints1Obj2.transform);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                GameObject objTriangle = t.ShowTriangle(root, trianglesObj2.transform, pointScale);
                objTriangle.name = $"triangle[{i1 + 1}]";
            }
            TransformHelper.ShowLocalPoint(point, pointScale * 2, root, trianglesObj2.transform);
        }
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
            TransformHelper.ShowLocalPoint(point, pointScale * 2, root, trianglesObj.transform);
        }
    }

    public void ShowSharedPointsByIdEx(Transform root, float pointScale, int minCount, float minDis)
    {
        //SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByPointEx(minCount,minDis);
        SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByIdEx(minCount, minDis);
        Debug.LogError($"GetElbowInfo sharedPoints1:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"sharedPoints(Id):{sharedPoints1.Count}", root);
        int id = 0;
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            SharedMeshTriangles plane = sharedPoints1[i];
            int pointId = plane.PointId;
            var triangles = plane.Triangles;
            if (triangles.Count < minCount) continue;
            id++;
            //Debug.Log($"GetElbowInfo sharedPoints1[{i + 1}/{sharedPoints1.Count}] point:{pointId} trianlges:{triangles.Count}");
            GameObject trianglesObj = CreateSubTestObj($"triangles[{id}][id:{pointId}]({triangles.Count})_{plane.Radius}", sharedPoints1Obj.transform);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                GameObject objTriangle = t.ShowTriangle(root, trianglesObj.transform, pointScale);
                objTriangle.name = $"triangle[{i1 + 1}]";
            }
            Vector3 point = mesh.vertices[pointId];
            TransformHelper.ShowLocalPoint(plane.Point, pointScale * 2, root, trianglesObj.transform).name="Point";
            TransformHelper.ShowLocalPoint(plane.Center, pointScale * 2, root, trianglesObj.transform).name = "Center";
        }
    }

    public void ShowSharedPointsByPointEx(Transform root, float pointScale, int minCount, float minDis)
    {
        SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByPointEx(minCount, minDis);
        //SharedMeshTrianglesList sharedPoints1 = this.GetKeyPointsByIdEx(minCount, minDis);
        Debug.LogError($"ShowSharedPointsByPointEx sharedPoints2:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"sharedPoints(Point):{sharedPoints1.Count}", root);
        int id = 0;
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            SharedMeshTriangles plane = sharedPoints1[i];
            int pointId = plane.PointId;
            var triangles = plane.GetTriangles();
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
                TransformHelper.ShowLocalPoint(p, pointScale, root, psObj.transform).name = $"Point[{i1}]({p})";
            }

            Vector3 point = mesh.vertices[pointId];
            TransformHelper.ShowLocalPoint(plane.Point, pointScale * 2, root, trianglesObj.transform).name = "Point";
            TransformHelper.ShowLocalPoint(plane.Center, pointScale * 2, root, trianglesObj.transform).name = "Center";
        }
    }

    private GameObject CreateSubTestObj(string objName, Transform parent)
    {
        GameObject objTriangles = new GameObject(objName);
        objTriangles.transform.SetParent(parent);
        objTriangles.transform.localPosition = Vector3.zero;
        return objTriangles;
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

    public SharedMeshTrianglesList GetKeyPointsByPointEx(int minCount, float minDis)
    {
        SharedMeshTrianglesList KeyPoints = new SharedMeshTrianglesList();
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        

        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Vector3 point = mesh.vertices[pointId];
            Vector3 normal = mesh.normals[pointId];



            var item = KeyPoints.FindItemByPoint(point, minDis);
            if (item == null)
            {
                KeyPoints.Add(new SharedMeshTriangles(pointId, point, normal, triangles));
            }
            else
            {
                item.AddOtherTriangles(triangles);
            }
        }

        SharedMeshTrianglesList KeyPoints2 = new SharedMeshTrianglesList();
        foreach(var item in KeyPoints)
        {
            if (item.Triangles.Count >= minCount)
            {
                item.GetInfo();
                KeyPoints2.Add(item);
            }
        }

        //Debug.Log($"GetKeyPointsByPointEx minCount:{minCount} minDis:{minDis} sharedPoints1:{sharedPoints1.Count} KeyPoints:{KeyPoints.Count} KeyPoints2:{KeyPoints2.Count}");

        if (KeyPoints2.Count < 2)
        {
            Debug.LogError($"GetKeyPoints KeyPoints:{KeyPoints2.Count}");
        }

        //KeyPoints.CombineSameCenter(minDis);

        KeyPoints2.Sort();
        return KeyPoints2;
    }

    public SharedMeshTrianglesList GetKeyPointsByIdEx(int minCount, float minDis)
    {
        SharedMeshTrianglesList KeyPoints = new SharedMeshTrianglesList();
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Vector3 point = mesh.vertices[pointId];

            if (triangles.Count >= minCount)
            {
                if (!KeyPoints.ContainsCenter(point))
                {
                    float dis = Distance2List(point, KeyPoints);
                    if (dis > minDis)
                    {
                        Vector3 normal = mesh.normals[pointId];
                        //Debug.Log($"dis:{dis} point:{point} KeyPoints:{KeyPoints.Count}");
                        KeyPoints.Add(new SharedMeshTriangles(pointId, point, normal, triangles));
                    }
                }
            }
        }
        if (KeyPoints.Count < 2)
        {
            Debug.LogError($"GetKeyPoints KeyPoints:{KeyPoints.Count}");
        }
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

    public float GetPipeRadius(int minCount)
    {
        float avgRadius = 0;
        int count = 0;
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Vector3 point = mesh.vertices[pointId];
            if (triangles.Count >= minCount)
            {
                float radius = GetTrianglesRadius(triangles, pointId);
                //Debug.Log($"point:{point} radius:{radius}");
                avgRadius += radius;
                count++;
            }
        }
        avgRadius /= count;
        //Debug.Log($"GetPipeRadius avgRadius:{avgRadius}");
        return avgRadius;
    }

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

    public void ShowKeyPointsById(Transform root, float pointScale, int minCount, float minDis)
    {
        TransformHelper.ShowLocalPoint(center, pointScale, root, null).name="MeshCenter";
        TransformHelper.ShowLocalPoint(mesh.boundCenter, pointScale, root, null).name = "BoundsCenter";

        //SharedMeshTrianglesList points = GetKeyPointsByIdEx(minCount, minDis);

        SharedMeshTrianglesList points = GetKeyPointsByPointEx(minCount, minDis);
        Debug.Log($"GetKeyPoints KeyPoints1:{points.Count}");
        if (points.Count > 6)
        {
            points.RemoveNotCircle();
        }
        //SharedMeshTrianglesList points = GetKeyPointsByIdEx(36, 0);
        Debug.Log($"GetKeyPoints KeyPoints2:{points.Count}");
        points.CombineSameCenter(minDis);
        Debug.Log($"GetKeyPoints KeyPoints3:{points.Count}");
        GameObject keyPointsObj = CreateSubTestObj($"KeyPoints:{points.Count}", root);
        var centerOfPoints = MeshHelper.GetCenterOfList(points);
        TransformHelper.ShowLocalPoint(centerOfPoints, pointScale, root, null).name = "KeysCenter";

        for (int i = 0; i < points.Count; i++)
        {
            SharedMeshTriangles point = points[i];
            TransformHelper.ShowLocalPoint(point.Normal, pointScale, root, keyPointsObj.transform).name = $"Key[{i + 1}]_Normal_{point.Normal}";
            TransformHelper.ShowLocalPoint(point.Center, pointScale, root, keyPointsObj.transform).name=$"Key[{i+1}]_Center_point.Center";
            TransformHelper.ShowLocalPoint(point.Point, pointScale, root, keyPointsObj.transform).name = $"Key[{i + 1}]_Point_{point.IsCircle}_{point.DistanceToCenter}_{point.GetRadius()}";
        }

        

        //points.Sort((a, b) => { })
    }
}
