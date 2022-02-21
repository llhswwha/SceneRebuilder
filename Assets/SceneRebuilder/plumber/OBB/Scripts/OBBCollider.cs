using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathGeoLib;
using System;
using Vector3=UnityEngine.Vector3;
using System.Linq;
using System.Text;

public class OBBCollider : MonoBehaviour
{
    public static OBBCollider GetOBB(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("OBBCollider.GetOBB obj == null");
            return null;
        }
        OBBCollider oBB = obj.GetComponent<OBBCollider>();
        if (oBB == null)
        {
            oBB = obj.AddComponent<OBBCollider>();
        }
        return oBB;
    }

    public static OBBCollider ShowOBB(GameObject obj, bool isGetObbEx)
    {
        if (obj == null)
        {
            Debug.LogError("OBBCollider.ShowOBB obj == null");
            return null;
        }
        OBBCollider oBB = GetOBB(obj);
        if (oBB != null)
        {
            oBB.ShowObbInfo(isGetObbEx);
        }
        return oBB;
    }

    public static OBBCollider ShowOBBNotUpdate(GameObject obj, bool isGetObbEx)
    {
        OBBCollider oBB = obj.GetComponent<OBBCollider>();
        if (oBB == null)
        {
            oBB = obj.AddComponent<OBBCollider>();
            oBB.ShowObbInfo(isGetObbEx);
        }
        return oBB;
    }

    public static float GetObbDistance(GameObject result, GameObject original)
    {
        if (result == null)
        {
            Debug.LogError("GetObbDistance result == null");
            return 10000;
        }
        if (original == null)
        {
            Debug.LogError("GetObbDistance original == null");
            return 10000;
        }
        OBBCollider oBBCollider1 = ShowOBB(result,false);
        OBBCollider oBBCollider2 = ShowOBBNotUpdate(original, false);
        OrientedBoundingBox obb1 = oBBCollider1.OBB; 
        OrientedBoundingBox obb2 = oBBCollider2.OBB;
        //if (obb1 == null || obb2 == null) return 11;
        if (oBBCollider1.IsObbError || oBBCollider2.IsObbError) return 22;
        return GetObbDistance(obb1, obb2);
    }

    public static float GetObbDistance(OrientedBoundingBox obb1, OrientedBoundingBox obb2)
    {
        var vs1 = obb1.CornerPointsVector3();
        var vs2 = obb2.CornerPointsVector3();
        //MeshHelper.GetVertexDistanceEx(vs1, vs2,)
        var dis = DistanceUtil.GetDistance(vs1, vs2);
        return dis;
    }

    public static float GetObbRTDistance(GameObject result, GameObject original)
    {
        OBBCollider oBBCollider1 = ShowOBB(result, false);
        OBBCollider oBBCollider2 = ShowOBBNotUpdate(original, false);
        OrientedBoundingBox obb1 = oBBCollider1.OBB;
        OrientedBoundingBox obb2 = oBBCollider2.OBB;
        //if (obb1 == null || obb2 == null) return 11;
        if (oBBCollider1.IsObbError || oBBCollider2.IsObbError) return 22;
        var vs1 = obb1.CornerPointsVector3();
        var vs2 = obb2.CornerPointsVector3();
        //MeshHelper.GetVertexDistanceEx(vs1, vs2,)
        var dis = MeshHelper.GetRTDistance(vs1, vs2);
        return dis;
    }

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

    public void ClearDebugInfoGos()
    {
        DebugInfoRoot[] debugRoots = this.GetComponentsInChildren<DebugInfoRoot>(true);
        foreach(var item in debugRoots)
        {
            if (item == null) continue;
            GameObject.DestroyImmediate(item.gameObject);
        }
    }

    [ContextMenu("ShowObbInfo")]
    public bool ShowObbInfo(bool isGetObbEx)
    {


        //ClearChildren();
        ClearDebugInfoGos();
        GetObb(isGetObbEx);
        //if (GetObb(isGetObbEx) == null) return false ;

        ShowOBBBox();

        ShowPipePoints();

        DrawWireCube();

        return true;
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

    public void TestGetObb()
    {
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        var vs = meshFilter.sharedMesh.vertices;
        OBB = OrientedBoundingBox.GetObb(vs, this.name,true);
    }


    public int TestObbPointCount = int.MaxValue;

    public ObbInfoJob GetObbJob(bool isGetObbEx)
    {
        ObbInfoJob job = new ObbInfoJob();
        return job;
    }

    public OrientedBoundingBox GetObb(bool isGetObbEx)
    {
        IsBreakProgress = false;

        DateTime start = DateTime.Now;
        List<Vector3> ps1 = new List<Vector3>();
        List<Vector3S> ps2 = new List<Vector3S>();
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
        {
            Debug.LogError("OBBCollider.GetObb meshFilter.sharedMesh == null");
            IsObbError = true;
            return new OrientedBoundingBox();
        }
        var vs = meshFilter.sharedMesh.vertices;
        var count = vs.Length;
        for (int i = 0; i < count; i++)
        {
            Vector3 p = vs[i];
            ps2.Add(new Vector3S(p.x, p.y, p.z));
            ps1.Add(p);
        }
        //Debug.Log("ps:"+ps.Count);
        OBB = OrientedBoundingBox.BruteEnclosing(ps2.ToArray());
        //Debug.Log($"GetObb ps:{ps2.Count} go:{gameObject.name} time:{(DateTime.Now - start).TotalMilliseconds}ms OBB:{OBB} Center:{OBB.Center} Extent:{OBB.Extent}");
        if (OBB.IsInfinity())
        {
            Debug.LogError($"GetObb Error gameObject:{this.name} Extent:{OBB.Extent} ps_Last:{ps2.Last()}");
            var errorP = ps1.Last();
            CreateLocalPoint(errorP, $"ObbErrorPoint({errorP.x},{errorP.y},{errorP.z})");
            //OBB = null;
            if (isGetObbEx)
            {
                if (GetObbEx() == false)
                {
                    return new OrientedBoundingBox();
                }
            }
            IsObbError = true;
        }
        return OBB;
    }

    public static bool IsBreakProgress = false;

    public bool GetObbEx()
    {
        DateTime start = DateTime.Now;
        List<Vector3> ps1 = new List<Vector3>();
        List<Vector3S> ps21 = new List<Vector3S>();
        List<Vector3S> ps22 = new List<Vector3S>();
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();

        var vs = meshFilter.sharedMesh.vertices;
        var count = vs.Length;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < count && i < TestObbPointCount; i++)
        {
            Vector3 p = vs[i];

            if(ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetObbEx", i, count, p)))
            {
                Debug.LogError("GetObbEx BreakProgress");
                IsBreakProgress = true;
                ProgressBarHelper.ClearProgressBar();
                return false;
            }

            ps21.Add(new Vector3S(p.x, p.y, p.z));

            if (i > 2)
            {
                OBB = OrientedBoundingBox.BruteEnclosing(ps21.ToArray());
                if (float.IsInfinity(OBB.Extent.x))
                {
                    //Debug.LogWarning($"GetObb Error[{i}/{count},{TestObbPointCount}] ps22:{ps22.Count}  ps21:{ps21.Count} Extent:{OBB.Extent} ps_Last:{ps21.Last()}");
                    sb.AppendLine($"GetObb go:{gameObject.name} Error[{i}/{count},{TestObbPointCount}] ps22:{ps22.Count}  ps21:{ps21.Count} Extent:{OBB.Extent} ps_Last:{ps21.Last()}");
                    ps21 = new List<Vector3S>(ps22);
                }
                else
                {
                    ps22.Add(new Vector3S(p.x, p.y, p.z));
                }
                ps1.Add(p);
            }
            else
            {
                ps22.Add(new Vector3S(p.x, p.y, p.z));
            }

        }
        //Debug.Log("ps:"+ps.Count);
        OBB = OrientedBoundingBox.BruteEnclosing(ps22.ToArray());
        if (sb.Length > 0)
        {
            Debug.LogWarning(sb.ToString());
        }
        Debug.Log($"GetObbEx go:{gameObject.name} ps:{ps22.Count}/{vs.Length}  time:{(DateTime.Now - start).TotalMilliseconds}ms OBB:{OBB} Center:{OBB.Center} Extent:{OBB.Extent}");
        if (OBB.IsInfinity())
        {
            Debug.LogError($"GetObbEx Error Extent:{OBB.Extent} ps_Last:{ps22.Last()}");
            var errorP = ps1.Last();
            CreateLocalPoint(errorP, $"ObbErrorPoint({errorP.x},{errorP.y},{errorP.z})");
        }
        ProgressBarHelper.ClearProgressBar();
        return true;
    }

    public bool IsObbError = false;

    public void ShowMeshVertices()
    {
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        var vs = meshFilter.sharedMesh.vertices;
        var count = vs.Length;
        Debug.Log($"ShowMeshVertices vs:{count}");

        List<Vector3> list = new List<Vector3>(vs);
        list.Sort((a,b)=> { 
            int r = a.x.CompareTo(b.x);
            if (r == 0)
            {
                r = a.y.CompareTo(b.y);
            }
            if (r == 0)
            {
                r = a.z.CompareTo(b.z);
            }
            return r;
        });

        DictionaryList1ToN<Vector3, Vector3> pointDict = new DictionaryList1ToN<Vector3, Vector3>();

        for (int i = 0; i < list.Count; i++)
        {
            Vector3 p = list[i];
            //GameObject obj=CreateLocalPoint(p, p.ToString());
            //obj.name = $"[{i + 1}_{count}]_({p.x},{p.y},{p.z})";

            pointDict.AddItem(p,p);
        }

        Debug.Log($"ShowMeshVertices pointDict:{pointDict.Count}");
        int k = 0;
        int count2 = pointDict.Keys.Count;
        foreach (var p in pointDict.Keys)
        {
            k++;
            GameObject obj = CreateLocalPoint(p, p.ToString());
            obj.name = $"[{k}_{count2}]_({p.x},{p.y},{p.z})";
        }

    }

    public void ShowSharedMeshVertices()
    {
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        var vs = meshFilter.mesh.vertices;
        var count = vs.Length;
        for (int i = 0; i < count; i++)
        {
            Vector3 p = vs[i];
            CreatePoint(p, p.ToString());
        }
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

    //public List<GameObject> DebugInfoRootGos = new List<GameObject>();

    public GameObject CreateDebugInfoRoot(string goName)
    {
        GameObject go0 = new GameObject(goName);
        go0.AddComponent<DebugInfoRoot>();
        go0.transform.SetParent(this.transform);
        go0.transform.localPosition = Vector3.zero;
        //DebugInfoRootGos.Add(go0);
        return go0;
    }

    public static GameObject CreateObbBox(Transform parent,OrientedBoundingBox OBB)
    {
        //GameObject go0 = CreateDebugInfoRoot("OBBCollider_OBBBox");

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        go.name = parent.name + "_ObbBox";
        ////obbGo = go;
        go.transform.SetParent(parent);
        //go.transform.localPosition=OBB.Center;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = OBB.Extent * 2f;

        go.transform.right = OBB.Right;
        var angle1 = Vector3.Angle(go.transform.up, OBB.Up);
        go.transform.Rotate(Vector3.right, -angle1);
        //对齐轴方向

        go.transform.localPosition = OBB.Center;
        //go.transform.SetParent(go0.transform);
        return go;
    }


    public void ShowOBBBox()
    {
        GameObject go0 = CreateDebugInfoRoot("OBBCollider_OBBBox");

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
        go.transform.SetParent(go0.transform);


        var p1 = this.transform.TransformPoint(OBB.Center);

        //CreatePoint(Vector3.zero,"Zero", go0.transform);
        //var center = OBB.Center;
        var center = p1;
        CreatePoint(center, "Center", go0.transform);
        CreatePoint(center + OBB.Right,"Right", go0.transform); ;
        CreatePoint(center + OBB.Up,"Up", go0.transform); ;
        CreatePoint(center + OBB.Forward,"Forward", go0.transform);

        //Vector3 v1 = CreateLine(Vector3.zero,OBB.Right* AxixLength, "Zero-Right");
        //Vector3 v2 = CreateLine(Vector3.zero,OBB.Up* AxixLength, "Zero-Up");
        //Vector3 v3 = CreateLine(Vector3.zero,OBB.Forward* AxixLength, "Zero-Forward");

        //Vector3 v1 = CreateLine(Vector3.zero, OBB.Right * OBB.Extent.x, "Zero-Right");
        //Vector3 v2 = CreateLine(Vector3.zero, OBB.Up * OBB.Extent.y, "Zero-Up");
        //Vector3 v3 = CreateLine(Vector3.zero, OBB.Forward * OBB.Extent.z, "Zero-Forward");

        Vector3 v1 = CreateLine(center, center + OBB.Right, "Center-Right", go0.transform);
        Vector3 v2 = CreateLine(center, center + OBB.Up, "Center-Up", go0.transform);
        Vector3 v3 = CreateLine(center, center + OBB.Forward, "Center-Forward", go0.transform);

        //Debug.Log("v1 dot v2:"+Vector3.Dot(v1,v2));
        //Debug.Log("v1 dot v3:"+Vector3.Dot(v1,v3));
        //Debug.Log("v2 dot v3:"+Vector3.Dot(v2,v3));

        //ShowPipePoints();
    }

    public void ShowPipePoints()
    {
        GameObject go = CreateDebugInfoRoot("OBBCollider_PipePoints");

        var StartPoint = OBB.Up * OBB.Extent.y;
        var EndPoint = -OBB.Up * OBB.Extent.y;

        CreateLocalPoint(StartPoint, "StartPoint",go.transform);
        CreateLocalPoint(EndPoint, "EndPoint", go.transform);

        var P1 = OBB.Right * OBB.Extent.x;
        var P2 = -OBB.Forward * OBB.Extent.z;
        var P3 = -OBB.Right * OBB.Extent.x;
        var P4 = OBB.Forward * OBB.Extent.z;

        CreateLocalPoint(P1, "P1", go.transform);
        CreateLocalPoint(P2, "P2", go.transform);
        CreateLocalPoint(P3, "P3", go.transform);
        CreateLocalPoint(P4, "P4", go.transform);

        float p = 1.414213562373f;
        CreateLocalPoint(P1 * p, "P11", go.transform);
        CreateLocalPoint(P2 * p, "P22", go.transform);
        CreateLocalPoint(P3 * p, "P33", go.transform);
        CreateLocalPoint(P4 * p, "P44", go.transform);
    }

    public void AlignDirection()
    {
        var StartPoint = OBB.Up * OBB.Extent.y;
        var EndPoint = -OBB.Up * OBB.Extent.y;
        var P1 = OBB.Right * OBB.Extent.x;
        var P2 = -OBB.Forward * OBB.Extent.z;
        var P3 = -OBB.Right * OBB.Extent.x;
        var P4 = OBB.Forward * OBB.Extent.z;

        this.transform.up = P1 - P3;
    }

    public static void AlignDirectionList(List<GameObject> objs)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            GameObject obj = objs[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("AlignDirection", i, objs.Count, obj)))
            {
                break;
            }
            OBBCollider obb = obj.AddMissingComponent<OBBCollider>();
            obb.GetObb(true);
            obb.AlignDirection();
        }
        ProgressBarHelper.ClearProgressBar();
    }

    //public Vector3 StartPoint = Vector3.zero;
    //public Vector3 EndPoint = Vector3.zero;

    //public Vector3 P1 = Vector3.zero;
    //public Vector3 P2 = Vector3.zero;
    //public Vector3 P3 = Vector3.zero;
    //public Vector3 P4 = Vector3.zero;

    public float AxixLength = 1;

    private void CreatePointS(Vector3S p,string n)
    {
        var lp = p.GetVector3();
        var wp = this.transform.TransformPoint(lp);
        CreatePoint(wp,n);
    }

    private void CreatePointS(Vector3S p, string n,Transform pt)
    {
        var lp = p.GetVector3();
        var wp = this.transform.TransformPoint(lp);
        var pObj=CreatePoint(wp, n);
        if(pObj)
            pObj.transform.SetParent(pt);
    }

    private GameObject CreatePoint(Vector3 p,string n)
    {
        if (float.IsNaN(p.x)) return null;
        GameObject g1=GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //g1.transform.SetParent(this.transform);
        //g1.transform.localPosition=p;
        g1.transform.position = p;
        g1.transform.localScale=new Vector3(lineSize, lineSize, lineSize);
        g1.name=n;

        g1.transform.SetParent(this.transform);
        return g1;
    }

    private GameObject CreateLocalPoint(Vector3 p, string n)
    {
        GameObject go= CreateLocalPoint(p, n, this.transform, lineSize);
        //go.AddComponent<DebugInfoRoot>();
        return go;
    }

    private GameObject CreateLocalPoint(Vector3 p, string n, Transform pT)
    {
        GameObject go = CreateLocalPoint(p, n, pT, lineSize);
        //go.AddComponent<DebugInfoRoot>();
        return go;
    }

    public static GameObject CreateLocalPoint(Vector3 p, string n, Transform pT,float scale)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        g1.transform.SetParent(pT);
        g1.transform.localPosition = p;
        //g1.transform.position = p;
        g1.transform.localScale = new Vector3(scale, scale, scale);
        g1.name = n;

        //g1.transform.SetParent(this.transform);

        g1.AddComponent<DebugInfoRoot>();
        return g1;
    }

    private void CreatePoint(Vector3 p, string n,Transform pT)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        //g1.transform.SetParent(this.transform);
        //g1.transform.localPosition=p;
        g1.transform.position = p;
        g1.transform.localScale = new Vector3(lineSize, lineSize, lineSize);
        g1.name = n;

        g1.transform.SetParent(pT);
    }

    private Vector3 CreateLineS(Vector3S p1,Vector3S p2,string n)
    {
        return CreateLine(transform.TransformPoint(p1.GetVector3()),transform.TransformPoint(p2.GetVector3()),n, null);
    }

    private Vector3 CreateLineS(Vector3S p1, Vector3S p2, string n, Transform pt)
    {
        return CreateLine(transform.TransformPoint(p1.GetVector3()), transform.TransformPoint(p2.GetVector3()), n, pt);
    }

    public float lineSize = 0.01f;

    public Vector3 CreateLine(Vector3 p1,Vector3 p2,string n,Transform pt=null)
    {
        GameObject g1=GameObject.CreatePrimitive(PrimitiveType.Cube);
        //g1.transform.SetParent(this.transform);
        //g1.transform.localPosition=(p1+p2)/2;
        g1.transform.position = (p1 + p2) / 2;
        g1.transform.forward=p2-p1;
        Vector3 scale=new Vector3(lineSize, lineSize, Vector3.Distance(p2,p1));
        g1.transform.localScale=scale;
        g1.name=n;
        g1.transform.SetParent(this.transform);
        if(pt!=null)
            g1.transform.SetParent(pt);
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

    [ContextMenu("ShowPlanes")]
    public void ShowPlanes()
    {
        PlaneInfo[] planes=OBB.GetPlaneInfos();
        GameObject go = new GameObject("Planes");
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;

        for(int i=0;i<planes.Length;i++)
        {
            var plane = planes[i];
            ShowPlaneInfo(plane, i, go,null);
        }
    }

    public void ShowPlaneInfo(PlaneInfo plane,int i,GameObject go,VerticesToPlaneInfo v2p)
    {
        GameObject planeObjRoot = new GameObject($"Plane[{i}]");
        planeObjRoot.transform.SetParent(go.transform);
        planeObjRoot.transform.localPosition = Vector3.zero;

        var point = plane.planePoint;
        var normal = plane.planeNormal * 0.1f;
        var normalPoint = (point + normal);
        TransformHelper.ShowLocalPoint(point, lineSize, this.transform, planeObjRoot.transform).name = $"Point:{point}";
        TransformHelper.ShowLocalPoint(normalPoint, lineSize, this.transform, planeObjRoot.transform).name = $"Normal:{normal}";
        TransformHelper.ShowLocalPoint(plane.planeCenter, lineSize, this.transform, planeObjRoot.transform).name = $"Center:{plane.planeCenter}";

        TransformHelper.ShowLocalPoint(plane.pointA, lineSize, this.transform, planeObjRoot.transform).name = $"pointA:{plane.pointA}";
        TransformHelper.ShowLocalPoint(plane.pointB, lineSize, this.transform, planeObjRoot.transform).name = $"pointB:{plane.pointB}";
        TransformHelper.ShowLocalPoint(plane.pointC, lineSize, this.transform, planeObjRoot.transform).name = $"pointC:{plane.pointC}";
        TransformHelper.ShowLocalPoint(plane.pointD, lineSize, this.transform, planeObjRoot.transform).name = $"pointD:{plane.pointD}";

        CreateLine(transform.TransformPoint(point), transform.TransformPoint(normalPoint), $"NormalLine:{normal}", planeObjRoot.transform);

        //GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        string nameInfo= $"Plane[{i}] size:({plane.SizeX},{plane.SizeY})_";
        if (v2p != null)
        {
            nameInfo+= v2p.ToString();
        }
        planeObjRoot.name = nameInfo;

        planeObj.name = nameInfo;
        planeObj.transform.SetParent(this.transform);
        //planeObj.transform.localPosition = point;
        planeObj.transform.localPosition = plane.planeCenter;
        planeObj.transform.forward = normal;
        //planeObj.transform.right = plane.Edge1;
        //planeObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.001f);

        //planeObj.transform.localScale = new Vector3(plane.SizeX, plane.SizeY, 0.001f);
        planeObj.transform.localScale = new Vector3(plane.Size, plane.Size, 0.001f);

        planeObj.transform.SetParent(planeObjRoot.transform);

    }

    public List<string> distances = new List<string>();

    [ContextMenu("DrawWireCube")]
    public void DrawWireCube()
    {

        GameObject go = CreateDebugInfoRoot("OBBCollider_WireCube");

        var points1=OBB.CornerPoints();

        for(int i=0;i<points1.Length;i++)
        {

            CreatePointS(points1[i],"p_"+i, go.transform);
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
                CreateLineS(points2[i],points2[i+1],"line_"+i, go.transform);

                Vector3S lineCenter = (points2[i] + points2[i + 1]) / 2;
                CreatePointS(lineCenter, "lineCenter_" + i, go.transform);
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

        CreateLineS(points1[2], points1[3],"line_23", go.transform);
        CreateLineS(points1[6], points1[7],"line_67", go.transform);
        CreateLineS(points1[4], points1[5],"line_45", go.transform);
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
