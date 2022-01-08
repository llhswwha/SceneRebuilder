using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedMeshTrianglesComponent : MonoBehaviour
{
    public SharedMeshTriangles sharedMeshTriangles;

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
}
