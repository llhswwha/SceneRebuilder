using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathGeoLib;
using System;
using Vector3=UnityEngine.Vector3;
public class OBBCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        //OrientedBoundingBox.BruteEnclosing()
    }

    //[ContextMenu("ShowVertexInfo")]
    //public void ShowVertexInfo()
    //{
    //    //MeshInfo
    //}

    public OrientedBoundingBox OBB;

    [ContextMenu("ShowObbInfo")]
    public void ShowObbInfo()
    {
        ClearChildren();

        GetObb();

        ShowOBBBox();

        DrawWireCube();
    }

    //public void CreatePipe()
    //{
    //    GameObject pipeNew = new GameObject(this.name + "_NewPipe");
    //    pipeNew.transform.position = this.transform.position;
    //    pipeNew.transform.SetParent(this.transform.parent);

    //    PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
    //    if (pipe == null)
    //    {
    //        pipe = pipeNew.AddComponent<PipeMeshGenerator>();
    //    }
    //    pipe.points = new List<Vector3>() { StartPoint, EndPoint };
    //    pipe.pipeRadius = OBB.Extent.x;
    //    pipe.RenderPipe();
    //}

    //public void CreateWeld()
    //{
    //    GameObject pipeNew = new GameObject(this.name + "_NewWeld");
    //    pipeNew.transform.position = this.transform.position;
    //    pipeNew.transform.SetParent(this.transform.parent);

    //    PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
    //    if (pipe == null)
    //    {
    //        pipe = pipeNew.AddComponent<PipeMeshGenerator>();
    //    }
    //    pipe.points = new List<Vector3>() { StartPoint, EndPoint };
    //    pipe.pipeRadius = OBB.Extent.x;
    //    pipe.RenderPipe();
    //}

    private void GetObb()
    {
        DateTime start=DateTime.Now;
        List<Vector3S> ps=new List<Vector3S>();
        MeshFilter meshFilter=this.GetComponent<MeshFilter>();
        
        var vs=meshFilter.sharedMesh.vertices;
        var count=vs.Length;
        for(int i=0;i<count;i++)
        {
            Vector3 p=vs[i];
            ps.Add(new Vector3S(p.x,p.y,p.z));
        }
        Debug.Log("ps:"+ps.Count);
        OBB=OrientedBoundingBox.BruteEnclosing(ps.ToArray());
        Debug.Log("GetObb:"+(DateTime.Now-start).TotalMilliseconds+"ms");
    }

    public GameObject obbGo;

     [ContextMenu("SetAxis1")]
    public void SetAxis1()
    {
        obbGo.transform.right=OBB.Right;
        var angle1=Vector3.Angle(obbGo.transform.up,OBB.Up);
        Debug.Log("angle1:"+Vector3.Angle(obbGo.transform.up,OBB.Up));
        Debug.Log("angle2:"+Vector3.Angle(obbGo.transform.forward,OBB.Forward));
    }

    [ContextMenu("SetAxis1Ex")]
    public void SetAxis1Ex()
    {
        obbGo.transform.right=OBB.Right;
        var angle1=Vector3.Angle(obbGo.transform.up,OBB.Up);
        var angle2=Vector3.Angle(OBB.Up,obbGo.transform.up);
        Debug.Log("angle1:"+angle1);
        Debug.Log("angle2:"+angle2);//angle1=angle2
        Debug.Log("angle up:"+Vector3.Angle(obbGo.transform.up,OBB.Up));
        Debug.Log("angle forward:"+Vector3.Angle(obbGo.transform.forward,OBB.Forward));//两个角度相同
        //obbGo.transform.Rotate(new Vector3(-angle1,0,0),Space.Self);//这里必须是负数
        //obbGo.transform.Rotate(Vector3.right,-angle1);//这个也行
        obbGo.transform.Rotate(Vector3.right,-angle2);//这个也行
    }

    // [ContextMenu("SetAxis2")]
    // public void SetAxis2()
    // {
    //     obbGo.transform.up=OBB.Up;
    //     Debug.Log("angle1:"+Vector3.Angle(obbGo.transform.right,OBB.Right));
    //     Debug.Log("angle2:"+Vector3.Angle(obbGo.transform.forward,OBB.Forward));
    // }

    // [ContextMenu("SetAxis3")]
    // public void SetAxis3()
    // {
    //     obbGo.transform.forward=OBB.Forward;
    //     Debug.Log("angle1:"+Vector3.Angle(obbGo.transform.right,OBB.Right));
    //     Debug.Log("angle2:"+Vector3.Angle(obbGo.transform.up,OBB.Up));
    // }

    [ContextMenu("ResetAngles1")]
    private void ResetAngles1()
    {
        Vector3 angles=obbGo.transform.rotation.eulerAngles;
        Debug.Log("angles:"+angles);
        Debug.Log("rotation:"+obbGo.transform.rotation);
        var inverse=Quaternion.Inverse(obbGo.transform.rotation);//求逆
        Debug.Log("inverse:"+inverse);
        //obbGo.transform.rotation=inverse*obbGo.transform.rotation;
        this.transform.rotation=inverse*this.transform.rotation;
    }

    
    [ContextMenu("ResetAngles2")]
    private void ResetAngles2()
    {
        Vector3 angles=obbGo.transform.rotation.eulerAngles;
        Debug.Log("angles:"+angles);
        Debug.Log("rotation:"+obbGo.transform.rotation);
        var inverse=Quaternion.Inverse(obbGo.transform.rotation);//求逆
        Debug.Log("inverse:"+inverse);
        obbGo.transform.rotation=inverse*obbGo.transform.rotation;
        //this.transform.rotation=inverse*this.transform.rotation;
    }

    [ContextMenu("ResetPos1")]
    private void ResetPos1()
    {
        Vector3 pos=obbGo.transform.position;
        Debug.Log("pos:"+pos);
        this.transform.position-=pos;
    }

    [ContextMenu("ResetPos2")]
    private void ResetPos2()
    {
        Vector3 pos=obbGo.transform.position;
        Debug.Log("pos:"+pos);
        obbGo.transform.position-=pos;
    }


    private void ShowOBBBox()
    {
        GameObject go=GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name=this.name+"_ObbBox";
        obbGo=go;
        go.transform.SetParent(this.transform);
        //go.transform.localPosition=OBB.Center;
        go.transform.localPosition=Vector3.zero;
        go.transform.localScale=OBB.Extent*2f;

        go.transform.right=OBB.Right;
        var angle1=Vector3.Angle(go.transform.up,OBB.Up);
        obbGo.transform.Rotate(Vector3.right,-angle1);
        //对齐轴方向

        go.transform.localPosition=OBB.Center;
        

        CreatePoint(Vector3.zero,"Zero");
        CreatePoint(OBB.Center,"Center");
        CreatePoint(OBB.Right,"Right");
        CreatePoint(OBB.Up,"Up");
        CreatePoint(OBB.Forward,"Forward");

        //Vector3 v1 = CreateLine(Vector3.zero,OBB.Right* AxixLength, "Zero-Right");
        //Vector3 v2 = CreateLine(Vector3.zero,OBB.Up* AxixLength, "Zero-Up");
        //Vector3 v3 = CreateLine(Vector3.zero,OBB.Forward* AxixLength, "Zero-Forward");

        Vector3 v1 = CreateLine(Vector3.zero, OBB.Right * OBB.Extent.x, "Zero-Right");
        Vector3 v2 = CreateLine(Vector3.zero, OBB.Up * OBB.Extent.y, "Zero-Up");
        Vector3 v3 = CreateLine(Vector3.zero, OBB.Forward * OBB.Extent.z, "Zero-Forward");


        Debug.Log("v1 dot v2:"+Vector3.Dot(v1,v2));
        Debug.Log("v1 dot v3:"+Vector3.Dot(v1,v3));
        Debug.Log("v2 dot v3:"+Vector3.Dot(v2,v3));

        ShowPipePoints();
    }

    private void ShowPipePoints()
    {
        var StartPoint = OBB.Up * OBB.Extent.y;
        var EndPoint = -OBB.Up * OBB.Extent.y;

        CreatePoint(StartPoint, "StartPoint");
        CreatePoint(EndPoint, "EndPoint");

        var P1 = OBB.Right * OBB.Extent.x;
        var P2 = -OBB.Forward * OBB.Extent.z;
        var P3 = -OBB.Right * OBB.Extent.x;
        var P4 = OBB.Forward * OBB.Extent.z;

        CreatePoint(P1, "P1");
        CreatePoint(P2, "P2");
        CreatePoint(P3, "P3");
        CreatePoint(P4, "P4");

        float p = 1.414213562373f;
        CreatePoint(P1 * p, "P11");
        CreatePoint(P2 * p, "P22");
        CreatePoint(P3 * p, "P33");
        CreatePoint(P4 * p, "P44");
    }

    //public Vector3 StartPoint = Vector3.zero;
    //public Vector3 EndPoint = Vector3.zero;

    //public Vector3 P1 = Vector3.zero;
    //public Vector3 P2 = Vector3.zero;
    //public Vector3 P3 = Vector3.zero;
    //public Vector3 P4 = Vector3.zero;

    public float AxixLength = 1;

    private void CreatePoint(Vector3S p,string n)
    {
        CreatePoint(p.GetVector3(),n);
    }

    private void CreatePoint(Vector3 p,string n)
    {
        GameObject g1=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        
        g1.transform.SetParent(this.transform);
        g1.transform.localPosition=p;
        g1.transform.localScale=new Vector3(lineSize, lineSize, lineSize);
        g1.name=n;
    }

    private Vector3 CreateLine(Vector3S p1,Vector3S p2,string n)
    {
        return CreateLine(p1.GetVector3(),p2.GetVector3(),n);
    }

    public float lineSize = 0.01f;

    private Vector3 CreateLine(Vector3 p1,Vector3 p2,string n)
    {
        GameObject g1=GameObject.CreatePrimitive(PrimitiveType.Cube);
        g1.transform.SetParent(this.transform);
        g1.transform.localPosition=(p1+p2)/2;
        g1.transform.forward=p2-p1;
        Vector3 scale=new Vector3(lineSize, lineSize, Vector3.Distance(p2,p1));
        g1.transform.localScale=scale;
        g1.name=n;
        return p2-p1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        List<Transform> children=new List<Transform>();
        for(int i=0;i<this.transform.childCount;i++)
        {
            var child=this.transform.GetChild(i);
            children.Add(child);
        }
        foreach(var child in children)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public List<string> distances = new List<string>();

    [ContextMenu("DrawWireCube")]
    public void DrawWireCube()
    {

        var points1=OBB.CornerPoints();

        for(int i=0;i<points1.Length;i++)
        {
            CreatePoint(points1[i],"p_"+i);
        }

        var points2=OBB.CornerBoxPoints();

        // Handles.DrawPolyLine(points);

        // Handles.DrawLine(points[2], points[3]);
        // Handles.DrawLine(points[6], points[7]);
        // Handles.DrawLine(points[4], points[5]);
        distances = new List<string>();
        List<Vector3S> lineCenterList = new List<Vector3S>();
        for(int i=0;i<points2.Length;i++)
        {
            if(i<points2.Length-1)
            {
                CreateLine(points2[i],points2[i+1],"line_"+i);

                Vector3S lineCenter = (points2[i] + points2[i + 1]) / 2;
                CreatePoint(lineCenter, "lineCenter_" + i);
                lineCenterList.Add(lineCenter);

                float length = points2[i].GetDistance(points2[i + 1]);
                string ls = length.ToString("F7");
                if(!distances.Contains(ls))
                    distances.Add(ls);
            }
            // else{
            //     CreateLine(points2[i],points2[0],"line_"+i);
            // }
        }

        //for (int i = 0; i < lineCenterList.Count; i++)
        //{
        //    if (i < lineCenterList.Count - 1)
        //    {
        //        Vector3S lineCenter = (lineCenterList[i] + lineCenterList[i + 1]) / 2;
        //        CreatePoint(lineCenter, "lineCenter_" + i);
        //    }
        //    // else{
        //    //     CreateLine(points2[i],points2[0],"line_"+i);
        //    // }
        //}

        CreateLine(points1[2], points1[3],"line_23");
        CreateLine(points1[6], points1[7],"line_67");
        CreateLine(points1[4], points1[5],"line_45");
    }

    private void OnSceneGUI()
    {
        Debug.Log("OnSceneGUI");
        //OBB.DrawWireCube();
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Debug.Log("OnDrawGizmosSelected");
    // }
    // private void OnDrawGizmos()
    // {
    //     Debug.Log("OnDrawGizmos");
    // }
    
}
