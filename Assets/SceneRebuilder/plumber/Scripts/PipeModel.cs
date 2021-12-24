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

    private GameObject CreateLocalPoint(Vector3 p, string n)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        g1.transform.SetParent(this.transform);
        g1.transform.localPosition = p;
        g1.transform.localScale = new Vector3(lineSize, lineSize, lineSize);
        g1.name = n;
        return g1;
    }

    private GameObject CreateLocalPoint(Vector3 p, string n,Transform pT)
    {
        GameObject g1 = CreateLocalPoint(p, n);

        g1.transform.SetParent(pT);
        return g1;
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
            oBBCollider = this.gameObject.AddComponent<OBBCollider>();

            //var mf = this.gameObject.GetComponent<MeshFilter>();
            //for(int i=0;i<mf.sharedMesh.vertices.Length;i++)
            //{
            //    var v = mf.sharedMesh.vertices[i];
            //    PointHelper.ShowPoint(v, new Vector3(0.01f, 0.01f, 0.01f), this.transform);
            //    var m = mf.sharedMesh.normals[i];
            //    //if (!sharedNormals.Contains(m))
            //    //{
            //    //    sharedNormals.Add(m);
            //    //}
            //}

        }
        oBBCollider.ShowObbInfo();
        OBB = oBBCollider.OBB;

        StartPoint = OBB.Up * OBB.Extent.y;
        EndPoint = -OBB.Up * OBB.Extent.y;

        GameObject go = new GameObject("PipeModel_PipeInfo");
        go.transform.SetParent(this.transform);
        go.transform.position = Vector3.zero;

        CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        CreateLocalPoint(EndPoint, "EndPoint1", go.transform);

        P1 = OBB.Right * OBB.Extent.x;
        P2 = -OBB.Forward * OBB.Extent.z;
        P3 = -OBB.Right * OBB.Extent.x;
        P4 = OBB.Forward * OBB.Extent.z;

        //CreateLocalPoint(P1, "P1", go.transform);
        //CreateLocalPoint(P2, "P2", go.transform);
        //CreateLocalPoint(P3, "P3", go.transform);
        //CreateLocalPoint(P4, "P4", go.transform);

        PipeRadius = OBB.Extent.x;

        var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();
        CreateLocalPoint(P1, $"P1_{GetPoint2VerticesInfo(vs,P1)}",go.transform);
        CreateLocalPoint(P2, $"P2_{GetPoint2VerticesInfo(vs,P2)}", go.transform);
        CreateLocalPoint(P3, $"P3_{GetPoint2VerticesInfo(vs,P3)}", go.transform);
        CreateLocalPoint(P4, $"P4_{GetPoint2VerticesInfo(vs,P4)}", go.transform);
    }

    private string GetPoint2VerticesInfo(Vector3[] vs,Vector3 p)
    {
        DictionaryList1ToN<float, Vector3> dict1 = new DictionaryList1ToN<float, Vector3>();
        DictionaryList1ToN<string, Vector3> dict14 = new DictionaryList1ToN<string, Vector3>();
        DictionaryList1ToN<string, Vector3> dict15 = new DictionaryList1ToN<string, Vector3>();
        for (int i = 0; i < vs.Length; i++)
        {
            Vector3 v = vs[i];
            float dis = Vector3.Distance(v, p);
            string sDis4 = dis.ToString("F4");
            string sDis5 = dis.ToString("F5");
            dict1.AddItem(dis, v);
            Debug.Log($"GetPoint2VerticesInfo p:{p} dis[{i + 1}]:{dis} sDis4:{sDis4} sDis5:{sDis5} v:{v} ");
            dict14.AddItem(sDis4, v);
            dict15.AddItem(sDis5, v);
        }
        return $"{dict1.Count}_{dict14.Count}_{dict15.Count}";
    }

    public void CreatePipe()
    {
        GetPipeInfo();

        //RendererPipe();
    }

    public bool generateWeld = false;

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
        pipe.generateWeld = generateWeld;
        pipe.pipeRadius = PipeRadius;
        pipe.RenderPipe();
        return pipeNew;
    }

    public void CreateWeld()
    {
        GameObject go = new GameObject("WeldPoints");
        go.transform.SetParent(this.transform);
        go.transform.position = Vector3.zero;

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

        CreateLocalPoint(StartPoint, "StartPoint2", go.transform);
        CreateLocalPoint(EndPoint, "EndPoint2", go.transform);

        P1 = OBB.Right * OBB.Extent.x ;
        P2 = -OBB.Forward * OBB.Extent.z;
        P3 = -OBB.Right * OBB.Extent.x;
        P4 = OBB.Forward * OBB.Extent.z;

        CreateLocalPoint(P1, "P1", go.transform);
        CreateLocalPoint(P2, "P2", go.transform);
        CreateLocalPoint(P3, "P3", go.transform);
        CreateLocalPoint(P4, "P4", go.transform);

        float p = 1.414213562373f;
        CreateLocalPoint(P1 * p, "P11", go.transform);
        CreateLocalPoint(P2 * p, "P22", go.transform);
        CreateLocalPoint(P3 * p, "P33", go.transform);
        CreateLocalPoint(P4 * p, "P44", go.transform);

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
            if (child == null) continue;
            if (child.gameObject == this.gameObject) continue;
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
