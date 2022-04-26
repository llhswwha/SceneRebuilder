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
            oBB.ShowObbInfo(null,isGetObbEx);
        }
        return oBB;
    }

    public static GameObject ShowOBBBox(GameObject obj, bool isGetObbEx)
    {
        if (obj == null)
        {
            Debug.LogError("OBBCollider.ShowOBB obj == null");
            return null;
        }
        OBBCollider oBB = GetOBB(obj);
        if (oBB != null)
        {
            return oBB.ShowObbInfoEx(null, isGetObbEx);
        }
        return null;
    }

    public static OBBCollider ShowOBBNotUpdate(GameObject obj, bool isGetObbEx)
    {
        OBBCollider oBB = obj.GetComponent<OBBCollider>();
        if (oBB == null)
        {
            oBB = obj.AddComponent<OBBCollider>();
            oBB.ShowObbInfo(null, isGetObbEx);
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
    public bool ShowObbInfo(Vector3[] vs, bool isGetObbEx)
    {


        //ClearChildren();

        if (vs == null)
        {
            ClearDebugInfoGos();
        }

        GetObb(vs,isGetObbEx);
        //if (GetObb(isGetObbEx) == null) return false ;

        ShowOBBBox();

        ShowPipePoints(vs);

        DrawWireCube();

        return true;
    }

    [ContextMenu("ShowObbInfoEx")]
    public GameObject ShowObbInfoEx(Vector3[] vs, bool isGetObbEx)
    {
        //ClearChildren();
        ClearDebugInfoGos();
        GetObb(vs,isGetObbEx);
        //if (GetObb(isGetObbEx) == null) return false ;
        return ShowOBBBox();
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

    public OrientedBoundingBox GetObb(Vector3[] vs,bool isGetObbEx)
    {
        IsBreakProgress = false;

        DateTime start = DateTime.Now;
        List<Vector3> ps1 = new List<Vector3>();
        List<Vector3S> ps2 = new List<Vector3S>();
        if (vs == null)
        {
            MeshFilter meshFilter = this.GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh == null)
            {
                Debug.LogError("OBBCollider.GetObb meshFilter.sharedMesh == null");
                IsObbError = true;
                return new OrientedBoundingBox();
            }
            vs = meshFilter.sharedMesh.vertices;
        }

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
                if (GetObbEx(vs) == false)
                {
                    return new OrientedBoundingBox();
                }
            }
            IsObbError = true;
        }
        return OBB;
    }

    public static bool IsBreakProgress = false;

    public bool GetObbEx(Vector3[] vs)
    {
        DateTime start = DateTime.Now;
        List<Vector3> ps1 = new List<Vector3>();
        List<Vector3S> ps21 = new List<Vector3S>();
        List<Vector3S> ps22 = new List<Vector3S>();
       
        if (vs == null)
        {
            MeshFilter meshFilter = this.GetComponent<MeshFilter>();
            vs = meshFilter.sharedMesh.vertices;
        }

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

    public void ShowMeshVertices(Vector3[] vs)
    {
        if (vs == null)
        {
            MeshFilter meshFilter = this.GetComponent<MeshFilter>();
            vs = meshFilter.sharedMesh.vertices;
        }
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
        var vs = meshFilter.sharedMesh.vertices;
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
        try
        {
            go0.AddComponent<DebugInfoRoot>();
            go0.transform.SetParent(this.transform);
            go0.transform.localPosition = Vector3.zero;
            //DebugInfoRootGos.Add(go0);
            return go0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"CreateDebugInfoRoot Exception:{ex}");
            return go0;
        }
    }

    public static GameObject CreateObbBox(Transform parent,OrientedBoundingBox OBB)
    {
        //GameObject go0 = CreateDebugInfoRoot("OBBCollider_OBBBox");

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        go.name = parent.name + "_ObbBox";
        ////obbGo = go;
        go.transform.SetParent(parent);
        //go.transform.SetParent(this.transform);
        //go.transform.localPosition=OBB.Center;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = OBB.Extent * 2f;

        SetRotation(go.transform, OBB, parent.name);

        go.transform.localPosition = OBB.Center;
        //go.transform.SetParent(go0.transform);
        return go;
    }

    public static float GetAngleOffset(float angle)
    {
        if (angle < 90)
        {
            return angle;
        }
        else if(angle < 180)
        {
            return 180 - angle;
        }
        else
        {
            return angle - 180;
        }
    }

    private static void SetRotation(Transform t,OrientedBoundingBox OBB,string name)
    {
        var angleRight = Vector3.Angle(t.right, OBB.Right);
        var angleUp = Vector3.Angle(t.up, OBB.Up);
        var angleForward = Vector3.Angle(t.forward, OBB.Forward);
        float sunAngle = GetAngleOffset(angleRight) + GetAngleOffset(angleUp) + GetAngleOffset(angleForward);

        //Debug.Log($"CreateObbBox1 gameObject:{name}| OBB.Up:{OBB.Up} right:{OBB.Right} Right:{angleRight} Up:{angleUp} Forward:{angleForward} sunAngle:{sunAngle}");

        Quaternion qua1 = Quaternion.FromToRotation(t.right, OBB.Right);//两条法线之间的角度变化
        t.rotation = qua1 * t.rotation;//旋转tempCenter，对齐两条法线


        //t.right = OBB.Right;
        //t.up = OBB.Up;
        //t.forward = OBB.Forward;

        angleRight = Vector3.Angle(t.right, OBB.Right);
        angleUp = Vector3.Angle(t.up, OBB.Up);
        angleForward = Vector3.Angle(t.forward, OBB.Forward);
        sunAngle = GetAngleOffset(angleRight) + GetAngleOffset(angleUp) + GetAngleOffset(angleForward);
        //Debug.Log($"CreateObbBox2 gameObject:{name}| OBB.Up:{OBB.Up} right:{OBB.Right} Right:{angleRight} Up:{angleUp} Forward:{angleForward} sunAngle:{sunAngle}");

        if (sunAngle > 0.3)
        {

            Quaternion qua2 = Quaternion.FromToRotation(t.up, OBB.Up);//两条法线之间的角度变化
            t.rotation = qua2 * t.rotation;//旋转tempCenter，对齐两条法线

            //t.Rotate(OBB.Up, -angle22, Space.World);

            angleRight = Vector3.Angle(t.right, OBB.Right);
            angleUp = Vector3.Angle(t.up, OBB.Up);
            angleForward = Vector3.Angle(t.forward, OBB.Forward);
            sunAngle = GetAngleOffset(angleRight) + GetAngleOffset(angleUp) + GetAngleOffset(angleForward);
            //Debug.Log($"CreateObbBox3 gameObject:{name}| OBB.Up:{OBB.Up} right:{OBB.Right} Right:{angleRight} Up:{angleUp} Forward:{angleForward} sunAngle:{sunAngle}");


            if (sunAngle > 0.3)
            {
                Quaternion qua3 = Quaternion.FromToRotation(t.forward, OBB.Forward);//两条法线之间的角度变化
                t.rotation = qua3 * t.rotation;//旋转tempCenter，对齐两条法线

                angleRight = Vector3.Angle(t.right, OBB.Right);
                angleUp = Vector3.Angle(t.up, OBB.Up);
                angleForward = Vector3.Angle(t.forward, OBB.Forward);
                sunAngle = GetAngleOffset(angleRight) + GetAngleOffset(angleUp) + GetAngleOffset(angleForward);
                //Debug.Log($"CreateObbBox4 gameObject:{name}| OBB.Up:{OBB.Up} right:{OBB.Right} Right:{angleRight} Up:{angleUp} Forward:{angleForward} sunAngle:{sunAngle}");
            }
        }


        //t.Rotate(t.up, angle32);
        //var angle42 = Vector3.Angle(t.up, OBB.Up);
        //Debug.Log($"CreateObbBox OBB.Up:{OBB.Up} right:{t.right} angle42:{angle42} gameObject:{name}");

        ////对齐轴方向
    }


    public GameObject ShowOBBBox()
    {
        GameObject go0 = CreateDebugInfoRoot("OBBCollider_OBBBox");
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        try
        {
            
            go.name = this.name + "_ObbBox";
            obbGo = go;
            go.transform.SetParent(this.transform);
            //go.transform.localPosition=OBB.Center;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = OBB.Extent * 2f;

            //var angle0 = Vector3.Angle(go.transform.right, OBB.Right);
            //Debug.Log($"ShowOBBBox OBB.Right:{OBB.Right} right:{go.transform.right} angle0:{angle0}");

            //go.transform.right=OBB.Right;
            //var angle1=Vector3.Angle(go.transform.up,OBB.Up);
            //Debug.Log($"ShowOBBBox OBB.Right:{OBB.Right} right:{go.transform.right} angle1:{angle1}");
            //if (angle1 < 90)
            //{
            //    go.transform.Rotate(Vector3.right, -angle1);
            //}
            //else
            //{
            //    //go.transform.Rotate(Vector3.right, angle1);
            //    go.transform.Rotate(Vector3.right, 180-angle1);
            //}

            //go.transform.Rotate(Vector3.right, -angle1);

            SetRotation(go.transform, OBB, this.name);

            //对齐轴方向

            go.transform.localPosition = OBB.Center;
            go.transform.SetParent(go0.transform);
            //var p1 = this.transform.TransformPoint(OBB.Center);
            //var center = p1;
            //CreatePoint(center, "Center", go0.transform);
            //CreatePoint(center + OBB.Right,"Right", go0.transform); ;
            //CreatePoint(center + OBB.Up,"Up", go0.transform); ;
            //CreatePoint(center + OBB.Forward,"Forward", go0.transform);

            //Vector3 v1 = CreateLine(center, center + OBB.Right, "Center-Right", go0.transform);
            //Vector3 v2 = CreateLine(center, center + OBB.Up, "Center-Up", go0.transform);
            //Vector3 v3 = CreateLine(center, center + OBB.Forward, "Center-Forward", go0.transform);

            //return go0;
            return go;
        }
        catch (Exception ex)
        {
            Debug.LogError($"ShowOBBBox Exception:{ex}");
            return go;
        }
    }

    public void ShowPipePoints(Vector3[] vs)
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
            obb.GetObb(null,true);
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
        try
        {
            var lp = p.GetVector3();
            var wp = this.transform.TransformPoint(lp);
            var pObj = CreatePoint(wp, n);
            if (pObj)
                pObj.transform.SetParent(pt);
        }
        catch (Exception ex)
        {
            Debug.LogError($"CreatePointS p:{p}");
        }
        
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
        GameObject go= MeshHelper.CreateLocalPoint(p, n, this.transform, lineSize);
        //go.AddComponent<DebugInfoRoot>();
        return go;
    }

    private GameObject CreateLocalPoint(Vector3 p, string n, Transform pT)
    {
        MeshFilter mf = pT.GetComponent<MeshFilter>();
        //if (mf)
        //{
        //    GameObject go = MeshHelper.CreateLocalPoint(p, n, pT, lineSize);
        //    //go.AddComponent<DebugInfoRoot>();
        //    return go;
        //}
        //else
        //{
        //    GameObject go = MeshHelper.CreatePoint(p, n, pT, lineSize);
        //    //go.AddComponent<DebugInfoRoot>();
        //    return go;
        //}

        GameObject go = MeshHelper.CreateLocalPoint(p, n, pT, lineSize);
        //go.AddComponent<DebugInfoRoot>();
        return go;
    }

    private GameObject CreatePoint(Vector3 p, string n, Transform pT)
    {
        GameObject go = MeshHelper.CreatePoint(p, n, pT, lineSize);
        //go.AddComponent<DebugInfoRoot>();
        return go;
    }

    private Vector3 CreateLineS(Vector3S p1,Vector3S p2,string n)
    {
        return CreateLine(this.transform, transform.TransformPoint(p1.GetVector3()),transform.TransformPoint(p2.GetVector3()),n, lineSize, null);
    }

    private Vector3 CreateLineS(Vector3S p1, Vector3S p2, string n, Transform pt)
    {
        return CreateLine(this.transform,transform.TransformPoint(p1.GetVector3()), transform.TransformPoint(p2.GetVector3()), n,lineSize, pt);
    }

    public float lineSize = 0.01f;

    public static Vector3 CreateLine(Transform root,Vector3 p1,Vector3 p2,string n,float size,Transform pt=null)
    {
        GameObject g1=GameObject.CreatePrimitive(PrimitiveType.Cube);
        //g1.transform.SetParent(this.transform);
        //g1.transform.localPosition=(p1+p2)/2;
        g1.transform.position = (p1 + p2) / 2;
        g1.transform.forward=p2-p1;
        Vector3 scale=new Vector3(size, size, Vector3.Distance(p2,p1));
        g1.transform.localScale=scale;
        g1.name=n;
        g1.transform.SetParent(root);
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
        plane.ShowPlaneInfo(i, go, v2p, lineSize, transform);

        //GameObject planeObjRoot = new GameObject($"Plane[{i}]");
        //planeObjRoot.transform.SetParent(go.transform);
        //planeObjRoot.transform.localPosition = Vector3.zero;

        //var point = plane.planePoint;
        //var normal = plane.planeNormal * 0.1f;
        //var normalPoint = (point + normal);
        //TransformHelper.ShowLocalPoint(point, lineSize, this.transform, planeObjRoot.transform).name = $"Point:{point}";
        //TransformHelper.ShowLocalPoint(normalPoint, lineSize, this.transform, planeObjRoot.transform).name = $"Normal:{normal}";
        //TransformHelper.ShowLocalPoint(plane.planeCenter, lineSize, this.transform, planeObjRoot.transform).name = $"Center:{plane.planeCenter}";

        //TransformHelper.ShowLocalPoint(plane.pointA, lineSize, this.transform, planeObjRoot.transform).name = $"pointA:{plane.pointA}";
        //TransformHelper.ShowLocalPoint(plane.pointB, lineSize, this.transform, planeObjRoot.transform).name = $"pointB:{plane.pointB}";
        //TransformHelper.ShowLocalPoint(plane.pointC, lineSize, this.transform, planeObjRoot.transform).name = $"pointC:{plane.pointC}";
        //TransformHelper.ShowLocalPoint(plane.pointD, lineSize, this.transform, planeObjRoot.transform).name = $"pointD:{plane.pointD}";

        //CreateLine(transform.TransformPoint(point), transform.TransformPoint(normalPoint), $"NormalLine:{normal}", planeObjRoot.transform);

        ////GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //GameObject planeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //string nameInfo= $"Plane[{i}] size:({plane.SizeX},{plane.SizeY})_";
        //if (v2p != null)
        //{
        //    nameInfo+= v2p.ToString();
        //}
        //planeObjRoot.name = nameInfo;

        //planeObj.name = nameInfo;
        //planeObj.transform.SetParent(this.transform);
        ////planeObj.transform.localPosition = point;
        //planeObj.transform.localPosition = plane.planeCenter;
        //planeObj.transform.forward = normal;
        ////planeObj.transform.right = plane.Edge1;
        ////planeObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.001f);

        ////planeObj.transform.localScale = new Vector3(plane.SizeX, plane.SizeY, 0.001f);
        //planeObj.transform.localScale = new Vector3(plane.Size, plane.Size, 0.001f);

        //planeObj.transform.SetParent(planeObjRoot.transform);

        //for (int i1 = 0; i1 < v2p.Plane1Points.Count; i1++)
        //{
        //    Vector3 p = v2p.Plane1Points[i1];
        //    TransformHelper.ShowLocalPoint(p, lineSize, this.transform, planeObjRoot.transform).name = $"Plane1Points[{i1}]";
        //}
        //for (int i1 = 0; i1 < v2p.Plane2Points.Count; i1++)
        //{
        //    Vector3 p = v2p.Plane2Points[i1];
        //    TransformHelper.ShowLocalPoint(p, lineSize, this.transform, planeObjRoot.transform).name = $"Plane2Points[{i1}]";
        //}
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
