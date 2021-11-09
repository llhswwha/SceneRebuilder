using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModel : MonoBehaviour
{
    //public Vector3 startP;

    //public Vector3 endP;

    //public float radius;

    public void ShowOBB()
    {
        OBBCollider oBB = this.gameObject.GetComponent<OBBCollider>();
        if (oBB == null)
        {
            oBB = this.gameObject.AddComponent<OBBCollider>();
        }
        if (oBB != null)
        {
            oBB.ShowObbInfo();
        }
    }

    //public Material PipeMaterial;

    //public Material WeldMaterial;

    public int pipeSegments = 24;

    public float weldRadius = 0.005f;

    public Vector3 Offset = Vector3.zero;

    //private void ShowPipePoints()
    //{
    //    StartPoint = OBB.Up * OBB.Extent.y;
    //    EndPoint = -OBB.Up * OBB.Extent.y;

    //    CreatePoint(StartPoint, "StartPoint");
    //    CreatePoint(EndPoint, "EndPoint");

    //    P1 = OBB.Right * OBB.Extent.x;
    //    P2 = -OBB.Forward * OBB.Extent.z;
    //    P3 = -OBB.Right * OBB.Extent.x;
    //    P4 = OBB.Forward * OBB.Extent.z;
    //    CreatePoint(P1, "P1");
    //    CreatePoint(P2, "P2");
    //}

    public Vector3 StartPoint = Vector3.zero;
    public Vector3 EndPoint = Vector3.zero;

    public Vector3 GetStartPoint()
    {
        return StartPoint + this.transform.position;
    }

    public Vector3 GetEndPoint()
    {
        return EndPoint + this.transform.position;
    }

    public float PipeRadius = 0;

    public Vector3 P1 = Vector3.zero;
    public Vector3 P2 = Vector3.zero;
    public Vector3 P3 = Vector3.zero;
    public Vector3 P4 = Vector3.zero;

    public OrientedBoundingBox OBB;

    private void CreatePoint(Vector3 p, string n)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        g1.transform.SetParent(this.transform);
        g1.transform.localPosition = p;
        g1.transform.localScale = new Vector3(lineSize, lineSize, lineSize);
        g1.name = n;
    }

    //private Vector3 CreateLine(Vector3S p1, Vector3S p2, string n)
    //{
    //    return CreateLine(p1.GetVector3(), p2.GetVector3(), n);
    //}

    public float lineSize = 0.01f;

    public void GetPipeInfo()
    {
        ClearChildren();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            var v = mf.sharedMesh.vertices[i];
            PointHelper.ShowPoint(v, new Vector3(0.01f, 0.01f, 0.01f), this.transform);
            var m = mf.sharedMesh.normals[i];
            if(!sharedNormals.Contains(m))
            {
                sharedNormals.Add(m);
            }
        }
        oBBCollider.ShowObbInfo();
        OBB = oBBCollider.OBB;

        StartPoint = OBB.Up * OBB.Extent.y;
        EndPoint = -OBB.Up * OBB.Extent.y;

        CreatePoint(StartPoint, "StartPoint");
        CreatePoint(EndPoint, "EndPoint");

        P1 = OBB.Right * OBB.Extent.x;
        P2 = -OBB.Forward * OBB.Extent.z;
        P3 = -OBB.Right * OBB.Extent.x;
        P4 = OBB.Forward * OBB.Extent.z;

        CreatePoint(P1, "P1");
        CreatePoint(P2, "P2");

        PipeRadius= OBB.Extent.x;
    }

    public void CreatePipe()
    {
        GetPipeInfo();

        //RendererPipe();
    }

    public GameObject RendererPipe(Material pMat,Material wMat)
    {
        GameObject pipeNew = new GameObject(this.name + "_NewPipe");
        pipeNew.transform.position = this.transform.position + Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        pipe.points = new List<Vector3>() { StartPoint, EndPoint };
        pipe.pipeSegments = pipeSegments;
        pipe.pipeMaterial = pMat;
        pipe.weldMaterial = wMat;
        pipe.weldRadius = this.weldRadius;
        pipe.generateWeld = true;
        pipe.pipeRadius = PipeRadius;
        pipe.RenderPipe();
        return pipeNew;
    }

    public void CreateWeld()
    {
        ClearChildren();

        OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
        if (oBBCollider == null)
        {
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();
            oBBCollider.ShowObbInfo();
        }
        OBB = oBBCollider.OBB;

        StartPoint = OBB.Up * OBB.Extent.y;
        EndPoint = -OBB.Up * OBB.Extent.y;

        CreatePoint(StartPoint, "StartPoint");
        CreatePoint(EndPoint, "EndPoint");

        P1 = OBB.Right * OBB.Extent.x ;
        P2 = -OBB.Forward * OBB.Extent.z;
        P3 = -OBB.Right * OBB.Extent.x;
        P4 = OBB.Forward * OBB.Extent.z;

        CreatePoint(P1, "P1");
        CreatePoint(P2, "P2");
        CreatePoint(P3, "P3");
        CreatePoint(P4, "P4");

        float p = 1.414213562373f;
        CreatePoint(P1 * p, "P11");
        CreatePoint(P2 * p, "P22");
        CreatePoint(P3 * p, "P33");
        CreatePoint(P4 * p, "P44");

        GameObject pipeNew = new GameObject(this.name + "_NewWeld");
        pipeNew.transform.position = this.transform.position + Offset;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        pipe.points = new List<Vector3>() { P1, P2, P3, P4 };
        pipe.pipeSegments = pipeSegments;
        //pipe.pipeMaterial = this.PipeMaterial;
        //pipe.weldMaterial = this.WeldMaterial;
        pipe.weldRadius = this.weldRadius;
        pipe.elbowRadius = Vector3.Distance(P1, P2)/2;
        pipe.IsLinkEndStart = true;
        pipe.generateWeld = false;
        pipe.pipeRadius = this.weldRadius;
        pipe.RenderPipe();
    }

    //public Vector3[] vertices;
    //public Vector3[] normals;
    //public Vector4[] tangents;
    //public int[] triangles;
    //public List<Vector3> sharedNormals;
    //[ContextMenu("ShowVertex")]
    //public void ShowVertex()
    //{
    //    //Dictionary<Vector3,List<Vector3>> 
    //    MeshFilter mf = this.GetComponent<MeshFilter>();
    //    vertices = mf.sharedMesh.vertices;
    //    normals = mf.sharedMesh.normals;
    //    tangents = mf.sharedMesh.tangents;
    //    triangles= mf.sharedMesh.triangles;
    //    sharedNormals = new List<Vector3>();
    //    ClearChildren();
    //    for (int i=0;i<mf.sharedMesh.vertices.Length;i++)
    //    {
    //        var v = mf.sharedMesh.vertices[i];
    //        VertexHelper.ShowPoint(v, new Vector3(0.01f, 0.01f, 0.01f), this.transform);
    //        var m = mf.sharedMesh.normals[i];
    //        if(!sharedNormals.Contains(m))
    //        {
    //            sharedNormals.Add(m);
    //        }
    //    }
    //}

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach(var child in children)
        {
            if (child.gameObject == this.gameObject) continue;
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
