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

    public Mesh mesh;

    public Vector3 center = Vector3.zero;

    public MeshTriangles(Mesh mesh)
    {
        this.mesh = mesh;
        int[] triangles = mesh.triangles;
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

        for(int i = 0; i < mesh.vertexCount; i++)
        {
            center += mesh.vertices[i];
        }
        center /= mesh.vertexCount;
    }

    private MeshPoint GetMeshPoint(Mesh mesh, int pi1)
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
            var ps = t1.Points;
            foreach (var p in ps)
            {
                sharedPoints.AddItem(p.Point, t1);
            }
        }
        List<Key2List<Vector3, MeshTriangle>> list = sharedPoints.GetListSortByCount();
        return list;
    }

    public void ShowSharedPointsByPoint(Transform root, float pointScale)
    {
        List<Key2List<Vector3, MeshTriangle>> sharedPoints2 = this.FindSharedPointsByPoint();
        Debug.LogError($"GetElbowInfo sharedPoints2:{sharedPoints2.Count}");
        GameObject sharedPoints1Obj2 = CreateSubTestObj($"sharedPoints1:{sharedPoints2.Count}", root);
        for (int i = 0; i < sharedPoints2.Count; i++)
        {
            var point = sharedPoints2[i].Key;
            var triangles = sharedPoints2[i].List;
            Debug.Log($"GetElbowInfo sharedPoints2[{i + 1}/{sharedPoints2.Count}] point:{point} trianlges:{triangles.Count}");
            GameObject trianglesObj2 = CreateSubTestObj($"triangles:{triangles.Count}", sharedPoints1Obj2.transform);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                GameObject objTriangle = t.ShowTriangle(root, trianglesObj2.transform, pointScale);
                objTriangle.name = $"triangle[{i1 + 1}]";
            }
            TransformHelper.ShowLocalPoint(point, pointScale * 10, root, trianglesObj2.transform);
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
                var ps = t1.Points;
                foreach (var p in ps)
                {
                    sharedPoints.AddItem(p.Id, t1);
                }
            }
            sharedPointsById = sharedPoints.GetListSortByCount();
        }

        return sharedPointsById;
    }

    public void ShowSharedPointsById(Transform root, float pointScale)
    {
        List<Key2List<int, MeshTriangle>> sharedPoints1 = this.FindSharedPointsById();
        Debug.LogError($"GetElbowInfo sharedPoints1:{sharedPoints1.Count}");
        GameObject sharedPoints1Obj = CreateSubTestObj($"sharedPoints1:{sharedPoints1.Count}", root);

        for (int i = 0; i < sharedPoints1.Count; i++)
        {
            int pointId = sharedPoints1[i].Key;
            var triangles = sharedPoints1[i].List;
            Debug.Log($"GetElbowInfo sharedPoints1[{i + 1}/{sharedPoints1.Count}] point:{pointId} trianlges:{triangles.Count}");
            GameObject trianglesObj = CreateSubTestObj($"triangles[{i + 1}][id:{pointId}]({triangles.Count})", sharedPoints1Obj.transform);
            for (int i1 = 0; i1 < triangles.Count; i1++)
            {
                MeshTriangle t = triangles[i1];
                GameObject objTriangle = t.ShowTriangle(root, trianglesObj.transform, pointScale);
                objTriangle.name = $"triangle[{i1 + 1}]";
            }
            Vector3 point = mesh.vertices[pointId];
            TransformHelper.ShowLocalPoint(point, pointScale * 10, root, trianglesObj.transform);
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
                if (!KeyPoints.Contains(point))
                {
                    float dis = Distance2List(point, KeyPoints);
                    if (dis > minDis)
                    {
                        Vector3 normal= mesh.normals[pointId];
                        //Debug.Log($"dis:{dis} point:{point} KeyPoints:{KeyPoints.Count}");
                        KeyPoints.Add(new SharedMeshTriangles(pointId,point,triangles));
                    }
                }
            }
        }
        if (KeyPoints.Count < 2)
        {
            Debug.LogError($"GetKeyPoints KeyPoints:{KeyPoints.Count}");
        }
        return KeyPoints;
    }

    private float GetTrianglesRadius(List<MeshTriangle> triangles,int pointId)
    {
        float radius = 0;
        foreach (MeshTriangle triangle in triangles)
        {
            float r = triangle.GetRadius(pointId);
            radius += r;
        }
        radius /= triangles.Count;
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

    public void ShowKeyPointsById(Transform root, float pointScale)
    {
        TransformHelper.ShowLocalPoint(center, pointScale * 5, root, null).name="MeshCenter";
        TransformHelper.ShowLocalPoint(mesh.bounds.center, pointScale * 5, root, null).name = "BoundsCenter";

        List<Vector3> points = GetKeyPointsById(36,0);
        Debug.Log($"GetKeyPoints KeyPoints:{points.Count}");
        GameObject keyPointsObj = CreateSubTestObj($"KeyPoints:{points.Count}", root);
        var centerOfPoints = MeshHelper.GetCenterOfList(points);
        TransformHelper.ShowLocalPoint(centerOfPoints, pointScale * 5, root, null).name = "KeysCenter";

        foreach (var point in points)
        {
            TransformHelper.ShowLocalPoint(point, pointScale * 5, root, keyPointsObj.transform);
        }

        

        //points.Sort((a, b) => { })
    }
}