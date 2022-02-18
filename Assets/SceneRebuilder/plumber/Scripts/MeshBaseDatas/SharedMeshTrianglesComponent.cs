using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedMeshTrianglesComponent : MonoBehaviour
{
    public SharedMeshTriangles sharedMeshTriangles;

    //public MeshTriangleList Triangles;

    public List<MeshTriangle> AllTriangles;

    public void ShowTrianglesInfo()
    {
        //SharedMeshTriangles plane = sharedPoints1[i];
        //int pointId = plane.PointId;
        //var triangles = plane.Triangles;

        //for (int i1 = 0; i1 < triangles.Count; i1++)
        //{
        //    MeshTriangle t = triangles[i1];
        //    GameObject objTriangle = t.ShowTriangle(root, trianglesObj.transform, pointScale);
        //    objTriangle.name = $"triangle[{i1 + 1}]";
        //}
        //Vector3 point = mesh.vertices[pointId];
        //TransformHelper.ShowLocalPoint(plane.Point, pointScale * 2, root, trianglesObj.transform).name = "Point";
        //TransformHelper.ShowLocalPoint(plane.Center, pointScale * 2, root, trianglesObj.transform).name = "Center";
    }

    public void SetData(SharedMeshTriangles data)
    {
        sharedMeshTriangles = data;
        AllTriangles = data.GetAllTriangles();
    }

    [ContextMenu("GetInfo")]
    public void GetInfo()
    {
        sharedMeshTriangles.GetInfo();

        MeshTriangleList ts = new MeshTriangleList(AllTriangles);
        //Center = Triangles.GetCenter(PointId);
        var Center = ts.GetCenter();
        //Radius= Triangles.GetRadius(PointId);
        var CircleCheckP = ts.GetCircleCheckP(sharedMeshTriangles.PointId);


        //if (IsCircle)
        //{
        //    Radius = Triangles.GetAvgRadius1(PointId);
        //}
        //else
        //{
        //    Radius = Triangles.GetAvgRadius2(PointId);
        //}

        var minMaxR = ts.GetMinMaxRadius(0.00001f, Center);


        var MinRadius = minMaxR[0];
        var Radius = minMaxR[1];

        //Radius = Triangles.GetRadius2(PointId);

        var DistanceToCenter = Vector3.Distance(sharedMeshTriangles.Point, Center);

        var IsCircle = CircleCheckP <= CircleInfo.IsCircleMaxP || DistanceToCenter < CircleInfo.MinDistanceToCenter;

        Debug.Log($"IsCircle:{IsCircle} CircleCheckP:{CircleCheckP} DistanceToCenter:{DistanceToCenter}");
    }
}
