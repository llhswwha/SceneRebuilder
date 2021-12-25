using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PipeLineModel
/// </summary>
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
        //return StartPoint + this.transform.position;
        //return StartPoint;
        return this.transform.TransformPoint(StartPoint);
    }

    public Vector3 GetEndPoint()
    {
        //return EndPoint + this.transform.position;
        //return EndPoint;
        return this.transform.TransformPoint(EndPoint);
    }

    public float PipeRadius = 0;
    public float PipeLength = 0;

    public float SizeX = 0;
    public float SizeY = 0;
    public float SizeZ = 0;

    public Vector3 P1 = Vector3.zero;
    public Vector3 P2 = Vector3.zero;
    public Vector3 P3 = Vector3.zero;
    public Vector3 P4 = Vector3.zero;
    public Vector3 P5 = Vector3.zero;
    public Vector3 P6 = Vector3.zero;

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

        //CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        //CreateLocalPoint(EndPoint, "EndPoint1", go.transform);

        P1 = OBB.Right * OBB.Extent.x;
        P2 = -OBB.Forward * OBB.Extent.z;
        P3 = -OBB.Right * OBB.Extent.x;
        P4 = OBB.Forward * OBB.Extent.z;
        P5 = OBB.Up * OBB.Extent.y;
        P6 = -OBB.Up * OBB.Extent.y;

        //CreateLocalPoint(P1, "P1", go.transform);
        //CreateLocalPoint(P2, "P2", go.transform);
        //CreateLocalPoint(P3, "P3", go.transform);
        //CreateLocalPoint(P4, "P4", go.transform);
      

        var rendererInfo = MeshRendererInfo.GetInfo(this.gameObject);
        Vector3[] vs = rendererInfo.GetVertices();

        planeCenterPointInfos = new List<PlaneCenterPointInfo>();

        CreateLocalPoint(P1, $"P1_{GetPoint2VerticesInfo(vs,P1,false)}",go.transform);
        CreateLocalPoint(P2, $"P2_{GetPoint2VerticesInfo(vs,P2, false)}", go.transform);
        CreateLocalPoint(P3, $"P3_{GetPoint2VerticesInfo(vs,P3, false)}", go.transform);
        CreateLocalPoint(P4, $"P4_{GetPoint2VerticesInfo(vs,P4, false)}", go.transform);
        CreateLocalPoint(P5, $"P5_{GetPoint2VerticesInfo(vs, P5, true)}", go.transform);
        CreateLocalPoint(P6, $"P6_{GetPoint2VerticesInfo(vs, P6, false)}", go.transform);

        planeCenterPointInfos.Sort();

        StartPoint = planeCenterPointInfos[0].GetCenter();
        EndPoint = planeCenterPointInfos[1].GetCenter();

        EndPoints = new List<Vector3>() { StartPoint, EndPoint };

        CreateLocalPoint(StartPoint, "StartPoint1", go.transform);
        CreateLocalPoint(EndPoint, "EndPoint1", go.transform);

        PipeRadius = OBB.Extent.x;

        if(StartPoint== P1 || EndPoint==P1)
        {
            PipeRadius = (OBB.Extent.z + OBB.Extent.y) / 2;
        }
        if (StartPoint == P2 || EndPoint == P2)
        {
            PipeRadius = (OBB.Extent.x + OBB.Extent.y) / 2;
        }
        if (StartPoint == P5 || EndPoint == P5)
        {
            PipeRadius = (OBB.Extent.x + OBB.Extent.z) / 2;
        }

        PipeLength = Vector3.Distance(StartPoint, EndPoint);
    }

    public List<Vector3> EndPoints = new List<Vector3>();

    public class PlaneCenterPointInfo:IComparable<PlaneCenterPointInfo>
    {
        public Vector3 Point;

        DictionaryList1ToN<float, Vector3> dict1 = new DictionaryList1ToN<float, Vector3>();
        DictionaryList1ToN<string, Vector3> dict10 = new DictionaryList1ToN<string, Vector3>();
        DictionaryList1ToN<string, Vector3> dict11 = new DictionaryList1ToN<string, Vector3>();
        DictionaryList1ToN<string, Vector3> dict12 = new DictionaryList1ToN<string, Vector3>();
        DictionaryList1ToN<string, Vector3> dict13 = new DictionaryList1ToN<string, Vector3>();
        DictionaryList1ToN<string, Vector3> dict14 = new DictionaryList1ToN<string, Vector3>();
        DictionaryList1ToN<string, Vector3> dict15 = new DictionaryList1ToN<string, Vector3>();

        public string ResultInfo = "";

        public int Count0;
        public int Count1;
        public int Count2;
        public int Count3;
        public int Count4;
        public int Count5;

        public PlaneCenterPointInfo(Vector3[] vs, Vector3 p,bool isShowLog)
        {
            Point = p;
            for (int i = 0; i < vs.Length; i++)
            {
                Vector3 v = vs[i];
                float dis = Vector3.Distance(v, p);
                string sDis0 = dis.ToString("F0");
                string sDis1 = dis.ToString("F1");
                string sDis2 = dis.ToString("F2");
                string sDis3 = dis.ToString("F3");
                string sDis4 = dis.ToString("F4");
                string sDis5 = dis.ToString("F5");
                dict1.AddItem(dis, v);
                if (isShowLog)
                {
                    Debug.Log($"Point2Vertices[{i + 1}] \tp:{p} \tdis:{dis} \tsDis0:{sDis0} \tsDis1:{sDis1} \tsDis2:{sDis2} \tsDis3:{sDis3} \tsDis4:{sDis4} \tsDis5:{sDis5} \tv:{v} ");
                }
                
                dict14.AddItem(sDis4, v);
                dict15.AddItem(sDis5, v);
                dict13.AddItem(sDis3, v);
                dict12.AddItem(sDis2, v);
                dict11.AddItem(sDis1, v);
                dict10.AddItem(sDis0, v);
            }
            Count0 = dict10.Count;
            Count1 = dict11.Count;
            Count2 = dict12.Count;
            Count3 = dict13.Count;
            Count4 = dict14.Count;
            Count5 = dict15.Count;

            if (Count0 == 1)
            {
                Count1 = int.MaxValue;
            }
            if (Count1 == 1)
            {
                Count1 = int.MaxValue;
            }
            if (Count2 == 1)
            {
                Count2 = int.MaxValue;
            }
            if (Count3 == 1)
            {
                Count3 = int.MaxValue;
            }
            if (Count4 == 1)
            {
                Count4 = int.MaxValue;
            }
            if (Count5 == 1)
            {
                Count5 = int.MaxValue;
            }

            ResultInfo =$"{dict10.Count}_{dict11.Count}_{dict12.Count}_{dict13.Count}_{dict14.Count}_{dict15.Count}_{dict1.Count}";
            if (isShowLog)
                Debug.LogError(ResultInfo);
        }

        private Vector3 GetCenter(DictionaryList1ToN<string, Vector3>  dict)
        {
            var keys = dict.Keys.ToList();
            keys.Sort();
            string firstKey = keys[0];
            var vs = dict[firstKey];
            Vector3 sum = Vector3.zero;
            foreach (var v in vs)
            {
                sum += v;
            }
            Vector3 center = sum / vs.Count;
            return center;
        }

        public Vector3 GetCenter()
        {

            if (Count5 == 2)
            {
                return GetCenter(dict15);
            }
            else if (Count4 == 2)
            {
                return GetCenter(dict14);
            }
            else if (Count3 == 2)
            {
                return GetCenter(dict13);
            }
            else if (Count2 == 2)
            {
                return GetCenter(dict12);
            }
            else if (Count1 == 2)
            {
                return GetCenter(dict11);
            }
            else if (Count0 == 2)
            {
                return GetCenter(dict10);
            }
            else
            {
                //return GetCenter(dict15);
                return Point;
            }
        }

        public override string ToString()
        {
            return ResultInfo;
        }

        public int CompareTo(PlaneCenterPointInfo other)
        {
            int r = this.Count2.CompareTo(other.Count2);
            if (r == 0)
            {
                r = this.Count3.CompareTo(other.Count3);
            }
            if (r == 0)
            {
                r = this.Count4.CompareTo(other.Count4);
            }
            if (r == 0)
            {
                r = this.Count5.CompareTo(other.Count5);
            }
            return r;
        }
    }

    public List<PlaneCenterPointInfo> planeCenterPointInfos = new List<PlaneCenterPointInfo>();

    private PlaneCenterPointInfo GetPoint2VerticesInfo(Vector3[] vs,Vector3 p,bool isShowLog)
    {
        //DictionaryList1ToN<float, Vector3> dict1 = new DictionaryList1ToN<float, Vector3>();
        //DictionaryList1ToN<string, Vector3> dict13 = new DictionaryList1ToN<string, Vector3>();
        //DictionaryList1ToN<string, Vector3> dict14 = new DictionaryList1ToN<string, Vector3>();
        //DictionaryList1ToN<string, Vector3> dict15 = new DictionaryList1ToN<string, Vector3>();
        //for (int i = 0; i < vs.Length; i++)
        //{
        //    Vector3 v = vs[i];
        //    float dis = Vector3.Distance(v, p);
        //    string sDis3 = dis.ToString("F3");
        //    string sDis4 = dis.ToString("F4");
        //    string sDis5 = dis.ToString("F5");
        //    dict1.AddItem(dis, v);
        //    Debug.Log($"GetPoint2VerticesInfo p:{p} dis[{i + 1}]:{dis} sDis3:{sDis3} sDis4:{sDis4} sDis5:{sDis5} v:{v} ");
        //    dict14.AddItem(sDis4, v);
        //    dict15.AddItem(sDis5, v);
        //}
        PlaneCenterPointInfo planeCenterPointInfo = new PlaneCenterPointInfo(vs, p, isShowLog);
        planeCenterPointInfos.Add(planeCenterPointInfo);
        return planeCenterPointInfo;
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
