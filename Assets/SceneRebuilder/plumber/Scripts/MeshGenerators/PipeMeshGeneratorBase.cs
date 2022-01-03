using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMeshGeneratorBase : MonoBehaviour
{
    public GameObject Target;

    public float elbowRadius = 0.5f;
    [Range(3, 32)]
    public int pipeSegments = 8;
    [Range(3, 32)]
    public int elbowSegments = 6;
    public Material pipeMaterial;
    public bool flatShading;
    public bool avoidStrangling;
    public bool generateEndCaps;
    public bool generateElbows = true;
    public bool generateOnStart;
    public bool makeDoubleSided;
    //public float colinearThreshold = 0.001f;
    public float colinearThreshold = 0.002f;

    protected void GenerateEndCaps(List<Vector3> points, List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
    {
        // create the circular cap on each end of the pipe
        int firstCircleOffset = 0;
        int secondCircleOffset = (points.Count - 1) * pipeSegments * 2 - pipeSegments;

        vertices.Add(points[0]); // center of first segment cap
        int firstCircleCenter = vertices.Count - 1;
        var firstCircleNormal = points[0] - points[1];
        normals.Add(firstCircleNormal);

        //vertices.Add(points[points.Count - 1]); // center of end segment cap
        //int secondCircleCenter = vertices.Count - 1;
        //normals.Add(points[points.Count - 1] - points[points.Count - 2]);

        //for (int i = 0; i < pipeSegments; i++) {
        //    triangles.Add(firstCircleCenter);
        //    triangles.Add(firstCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(firstCircleOffset + i);

        //    triangles.Add(secondCircleOffset + i);
        //    triangles.Add(secondCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(secondCircleCenter);
        //}
        GameObject root = new GameObject("CapInfos");
        root.transform.SetParent(this.transform);
        root.transform.localPosition = Vector3.zero;

        for (int i = 0; i < pipeSegments ; i++)
        {
            //triangles.Add(firstCircleCenter);
            //triangles.Add(firstCircleOffset + (i + 1) % pipeSegments);
            //triangles.Add(firstCircleOffset + i);

            Vector3 p1 = vertices[firstCircleOffset + (i + 1) % pipeSegments];
            Vector3 p2 = vertices[firstCircleOffset + i];

            vertices.Add(p1);
            normals.Add(firstCircleNormal);

            vertices.Add(p2);
            normals.Add(firstCircleNormal);

            triangles.Add(firstCircleCenter);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            //ShowPoint(p1, $"point[{i + 1}][{firstCircleOffset + i}]", root.transform);
            //ShowPoint(p1, $"point[{i + 2}][{firstCircleOffset + (i + 1) % pipeSegments}]", root.transform);


        }

        vertices.Add(points[points.Count - 1]); // center of end segment cap
        int secondCircleCenter = vertices.Count - 1;
        Vector3 sencondCircleNormal = points[points.Count - 1] - points[points.Count - 2];
        normals.Add(sencondCircleNormal);

        for (int i = 0; i < pipeSegments; i++)
        {
            //triangles.Add(secondCircleOffset + i);
            //triangles.Add(secondCircleOffset + (i + 1) % pipeSegments);
            //triangles.Add(secondCircleCenter);

            Vector3 p1 = vertices[secondCircleOffset + i];
            Vector3 p2 = vertices[secondCircleOffset + (i + 1) % pipeSegments];

            vertices.Add(p1);
            normals.Add(sencondCircleNormal);

            vertices.Add(p2);
            normals.Add(sencondCircleNormal);

            triangles.Add(secondCircleCenter);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            //ShowPoint(p1, $"point[{i + 1}][{secondCircleOffset + i}]", root.transform);
            //ShowPoint(p1, $"point[{i + 2}][{secondCircleOffset + (i + 1) % pipeSegments}]", root.transform);
        }
    }

    protected void GenerateEndCaps(List<Vector4> points4, List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
    {
        Debug.Log($"GenerateEndCaps 4 points:{points4.Count} vertices:{vertices.Count} triangles:{triangles.Count}" );
        //// create the circular cap on each end of the pipe
        //int firstCircleOffset = 0;
        //int secondCircleOffset = (points.Count - 1) * pipeSegments * 2 - pipeSegments;

        //vertices.Add(points[0]); // center of first segment cap
        //int firstCircleCenter = vertices.Count - 1;
        //normals.Add(points[0] - points[1]);

        //vertices.Add(points[points.Count - 1]); // center of end segment cap
        //int secondCircleCenter = vertices.Count - 1;
        //normals.Add(points[points.Count - 1] - points[points.Count - 2]);

        //for (int i = 0; i < pipeSegments; i++)
        //{
        //    triangles.Add(firstCircleCenter);
        //    triangles.Add(firstCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(firstCircleOffset + i);

        //    triangles.Add(secondCircleOffset + i);
        //    triangles.Add(secondCircleOffset + (i + 1) % pipeSegments);
        //    triangles.Add(secondCircleCenter);
        //}

        List<Vector3> points3 = new List<Vector3>();
        foreach (var p in points4)
        {
            points3.Add(p);
        }
        GenerateEndCaps(points3, vertices, triangles, normals);
    }

    public float PointScale = 0.01f;

    public List<GameObject> ShowPoints(List<Vector3> ps)
    {
        var psObjs = PointHelper.ShowPoints(ps, new Vector3(PointScale, PointScale, PointScale), this.transform);
        return psObjs;
    }

    public GameObject ShowPoint(Vector3 p, string pName, Transform parent)
    {
        GameObject pObj = PointHelper.ShowPoint(p, new Vector3(PointScale, PointScale, PointScale), this.transform);
        pObj.name = pName;
        pObj.transform.SetParent(parent);
        return pObj;
    }

    public List<GameObject> ShowPoints(List<Vector3> ps, string n)
    {
        GameObject go = new GameObject(n);
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        var psObjs = PointHelper.ShowPoints(ps, new Vector3(PointScale, PointScale, PointScale), go.transform);
        return psObjs;
    }
}
