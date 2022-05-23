using CommonExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMeshGenerator : MonoBehaviour
{
    public List<Vector3[]> StartPlanes=new List<Vector3[]>();

    public Vector3 Direction;

    public float Length = 2;

    //public Vector3 StartCenter;

    //public Vector3[] EndPlane;

    //public Vector3 EndCenter;

    public bool IsDrawLine = true;

    public void AddPlane(params Vector3[] plane)
    {
        StartPlanes.Add(plane);
    }

    [ContextMenu("GenerateMesh")]
    public void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        foreach(var plane in StartPlanes)
        {
            DrawPlaneLine(vertices, triangles, normals, plane);
        }
        

        Mesh m = new Mesh();
        m.name = "Line";
        m.SetVertices(vertices);
        m.SetTriangles(triangles, 0);
        m.SetNormals(normals);

        MeshFilter mf = gameObject.CreateMeshComponents();
        mf.sharedMesh = m;

        mesh = m;
    }

    public Mesh mesh;

    private void DrawPlaneLine(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, Vector3[] startPlane)
    {
        int offsetV = 0;
        if (vertices.Count > 0)
        {
            offsetV = vertices.Count;
        }
        
        Vector3 dir = Direction.normalized;
        MeshPointList mps = new MeshPointList();

        //vertices.AddRange(StartPlane);

        List<Vector3> EndPlane = new List<Vector3>();

        for (int i = 0; i < startPlane.Length; i++)
        {
            vertices.Add(startPlane[i]);
            normals.Add(-dir);

            EndPlane.Add(startPlane[i] + dir * Length);

            //Debug.LogError($"DrawPlaneLine[{i}] p:{startPlane[i]}");
        }

        for (int i = 0; i < startPlane.Length - 2; i++)
        {
            triangles.Add(i+ offsetV);
            triangles.Add(i + 1 + offsetV);
            triangles.Add(startPlane.Length - 1 + offsetV);
        }

        for (int i = 0; i < EndPlane.Count; i++)
        {
            vertices.Add(EndPlane[i]);
            normals.Add(dir);//??
        }
        int offset = startPlane.Length + offsetV;
        for (int i = 0; i < EndPlane.Count - 2; i++)
        {
            //triangles.Add(i + offset);
            //triangles.Add(i + 1 + offset);
            //triangles.Add(EndPlane.Count - 1 + offset);

            triangles.Add(EndPlane.Count - 1 + offset);
            triangles.Add(i + 1 + offset);
            triangles.Add(i + offset);
        }

        if (IsDrawLine)
        {
            DrawLineMesh(vertices, triangles, normals, startPlane, EndPlane);
        }
    }

    private void DrawLineMesh(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, Vector3[] startPlane,List<Vector3> endPlane)
    {
        for (int i = 0; i < startPlane.Length; i++)
        {
            Vector3 p01 = startPlane[i];
            Vector3 p02 = Vector3.zero;
            if (i < startPlane.Length - 1)
            {
                p02 = startPlane[i + 1];
            }
            else
            {
                p02 = startPlane[0];
            }

            Vector3 p11 = endPlane[i];
            Vector3 p12 = Vector3.zero;
            if (i < startPlane.Length - 1)
            {
                p12 = endPlane[i + 1];
            }
            else
            {
                p12 = endPlane[0];
            }

            Plane plane = new Plane(p02, p01, p11);

            vertices.Add(p01);
            int i01 = vertices.Count - 1;
            vertices.Add(p02);
            int i02 = vertices.Count - 1;
            normals.Add(plane.normal);
            normals.Add(plane.normal);

            vertices.Add(p11);
            int i11 = vertices.Count - 1;
            vertices.Add(p12);
            int i12 = vertices.Count - 1;

            normals.Add(plane.normal);
            normals.Add(plane.normal);



            //if (i < StartPlane.Length - 1)
            //{
            //    triangles.Add(i + 1);
            //    triangles.Add(i);
            //    triangles.Add(i + offset);

            //    triangles.Add(i + 1);
            //    triangles.Add(i + offset);
            //    triangles.Add(i + 1 + offset);
            //}
            //else
            //{
            //    triangles.Add(0);
            //    triangles.Add(i);
            //    triangles.Add(i + offset);

            //    triangles.Add(0);
            //    triangles.Add(i + offset);
            //    triangles.Add(0 + offset);
            //}

            triangles.Add(i02);
            triangles.Add(i01);
            triangles.Add(i11);

            triangles.Add(i02);
            triangles.Add(i11);
            triangles.Add(i12);

            //triangles.Add(i01);
            //triangles.Add(i11);
            //triangles.Add(i12);

            //Debug.LogError($"DrawLineMesh[{i}] i01:{i01} i02:{i02} i11:{i11} i12:{i12}");
        }
    }
}
