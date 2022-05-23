using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VerticesToPlaneInfo : IComparable<VerticesToPlaneInfo>
{
    public static List<VerticesToPlaneInfo> SortList(List<VerticesToPlaneInfo> verticesToPlaneInfos,List<VerticesToPlaneInfo> verticesToPlaneInfos_All)
    {
        verticesToPlaneInfos.Sort();

        List<VerticesToPlaneInfo> verticesToPlaneInfos2 = new List<VerticesToPlaneInfo>();
        VerticesToPlaneInfo before1 = verticesToPlaneInfos[0];
        VerticesToPlaneInfo before2 = VerticesToPlaneInfo.GetEndPlane(before1, verticesToPlaneInfos_All);
        verticesToPlaneInfos2.Add(before1);
        verticesToPlaneInfos.Remove(before1);
        verticesToPlaneInfos2.Add(before2);
        verticesToPlaneInfos.Remove(before2);

        VerticesToPlaneInfo top1 = verticesToPlaneInfos[0];
        VerticesToPlaneInfo top2 = VerticesToPlaneInfo.GetEndPlane(top1, verticesToPlaneInfos_All);
        verticesToPlaneInfos2.Add(top1);
        verticesToPlaneInfos.Remove(top1);
        verticesToPlaneInfos2.Add(top2);
        verticesToPlaneInfos.Remove(top2);

        VerticesToPlaneInfo left1 = verticesToPlaneInfos[0];
        VerticesToPlaneInfo left2 = VerticesToPlaneInfo.GetEndPlane(left1, verticesToPlaneInfos_All);
        verticesToPlaneInfos2.Add(left1);
        verticesToPlaneInfos.Remove(left1);
        verticesToPlaneInfos2.Add(left2);
        verticesToPlaneInfos.Remove(left2);

        return verticesToPlaneInfos2;
    }

    public static VerticesToPlaneInfo GetEndPlane(VerticesToPlaneInfo startPlane, List<VerticesToPlaneInfo> verticesToPlaneInfos_All)
    {
        VerticesToPlaneInfo endPlane = null;
        if (startPlane == verticesToPlaneInfos_All[0])
        {
            endPlane = verticesToPlaneInfos_All[1];
        }
        if (startPlane == verticesToPlaneInfos_All[1])
        {
            endPlane = verticesToPlaneInfos_All[0];
        }
        if (startPlane == verticesToPlaneInfos_All[2])
        {
            endPlane = verticesToPlaneInfos_All[3];
        }
        if (startPlane == verticesToPlaneInfos_All[3])
        {
            endPlane = verticesToPlaneInfos_All[2];
        }
        if (startPlane == verticesToPlaneInfos_All[4])
        {
            endPlane = verticesToPlaneInfos_All[5];
        }
        if (startPlane == verticesToPlaneInfos_All[5])
        {
            endPlane = verticesToPlaneInfos_All[4];
        }
        return endPlane;
    }

    public static VerticesToPlaneInfo GetMiddlePlane(VerticesToPlaneInfo plane1, VerticesToPlaneInfo plane2)
    {
        var ps1 = plane1.Plane1Points;
        var ps2 = plane2.Plane2Points;
        if (ps1.Count != ps2.Count)
        {
            Debug.LogError($"VerticesToPlaneInfo.GetMiddlePlane ps1.Count != ps2.Count ps1:{ps1.Count},ps2:{ps2.Count}");
            return null;
        }
        List<Vector3> ps3 = new List<Vector3>();
        foreach (var p1 in ps1)
        {
            Vector3 p2 = plane2.GetClosedPlanePoint1(p1);
            float dis = Vector3.Distance(p1, p2);
            Vector3 p3 = (p1 + p2) / 2;
            ps3.Add(p3);
        }
        PlaneInfo plane30 = new PlaneInfo(ps3[0], ps3[1], ps3[2], ps3[3]);
        VerticesToPlaneInfo plane31 = new VerticesToPlaneInfo(ps3.ToArray(), plane30, true);
        return plane31;
    }

    public float DistanceToPlane(VerticesToPlaneInfo plane2)
    {
        Vector3 p1 = this.GetMinVector3();
        Vector3 p2 = plane2.GetClosedPlanePoint1(p1);
        float dis = Vector3.Distance(p1, p2);
        return dis;
    }

    public float DistanceOfPlane12()
    {
        Vector3 p1 = Plane1Points[0];
        Vector3 p2 = GetClosedPlanePoint2(p1);
        float dis = Vector3.Distance(p1, p2);
        return dis;
    }

    public float DistanceOfPlane12Ex(Transform t, string tag, bool isShowPoint = false,float pointScale=0.01f)
    {
        if (Plane2PointCount == 0)
        {
            Debug.LogError($"DistanceOfPlane12Ex t:{t} tag:{tag}");
            return 0;
        }
        Vector3 p1 = this.GetMinVector3();
        //Vector3 normal = this.Plane.planeNormal;
        PlaneInfo plane1 = this.NewPlane1();
        Vector3 normal = plane1.planeNormal;
        float dis1 = Math3D.SignedDistancePlanePoint(plane1.planeNormal, plane1.planePoint, Plane2Points[0]);

        //Vector3 p2 = VertexHelper.GetClosedPointToLine(p1, normal, Plane2Points);


        Vector3 p2 = VertexHelper.GetClosedPointToPlane(plane1.planeNormal, plane1.planePoint, Plane2Points, false,null,tag);
        Vector3 p3 = Math3D.ProjectPointOnLine(p1, normal, p2);

        float dis12 = Vector3.Distance(p1, p2);
        float dis23 = Vector3.Distance(p3, p2);
        //float dis34 = Vector3.Distance(p12, p22);
        //Vector3 p1 = Plane1Points[0];
        //Vector3 p2 = GetClosedPlanePoint2(p1);
        float dis = Vector3.Distance(p1, p3);
        if (isShowPoint)
        {
            PointHelper.CreateLocalPoint(p1, $"DisOfPlane12Ex_{tag}_P1_({p1.Vector3ToString3()})_{dis12}/{dis}_{dis1}", t, pointScale);
            PointHelper.CreateLocalPoint(p2, $"DisOfPlane12Ex_{tag}_P2_({p2.Vector3ToString3()})_{dis12}/{dis}_{dis1}", t, pointScale);
            PointHelper.CreateLocalPoint(p3, $"DisOfPlane12Ex_{tag}_P3_({p3.Vector3ToString3()})_{dis23}/{dis}_{dis1}", t, pointScale);
            //PointHelper.CreateLocalPoint(p22, $"DisToPlane3_{tag}_P22_({p22.Vector3ToString3()})_{dis34}/{dis}", t, pointScale);
        }
        return dis;
    }

    public float DebugDistanceOfPlane12(Transform t, string tag)
    {
        if (Plane2Points.Count == 0)
        {
            Debug.LogError($"DebugDistanceOfPlane12 Plane2Points.Count == 0 t:{t.name} tag:{tag}");
            return 0;
        }

        Vector3 p1 = Plane1Points[0];
        Vector3 p2 = VertexHelper.GetClosedPoint(p1, this.Plane2Points);
        float dis = Vector3.Distance(p1, p2);
        PointHelper.CreateLocalPoint(p1, $"DistanceOfPlane12_{tag}_P1_{dis}", t, 0.01f);
        PointHelper.CreateLocalPoint(p2, $"DistanceOfPlane12_{tag}_P2_{dis}", t, 0.01f);
        return dis;
    }

    public Vector3 DirectionToPlane(VerticesToPlaneInfo plane2, Transform t, string tag)
    {
        Vector3 p1 = this.GetMinVector3();
        Vector3 p2 = VertexHelper.GetClosedPoint(p1, plane2.Plane1Points);
        return p2 - p1;
    }

    public void ShowPlaneInfo(object tag,GameObject parentGo, float lineSize, Transform rootT)
    {
        var plane = Plane;
        PlaneHelper.ShowPlaneInfo(plane, tag, parentGo, this, lineSize, rootT);
    }


    public float DistanceToPlane(VerticesToPlaneInfo plane2, Transform t, string tag,bool isShowPoint=false)
    {
        Vector3 p1 = this.GetMinVector3();
        Vector3 p2 = VertexHelper.GetClosedPoint(p1, plane2.Plane1Points);
        float dis12 = Vector3.Distance(p1, p2);
        float dis = dis12;
        if (isShowPoint)
        {
            PointHelper.CreateLocalPoint(p1, $"DistanceToPlane2_{tag}_P1_({p1.Vector3ToString()})_{dis12}/{dis}", t, 0.01f);
            PointHelper.CreateLocalPoint(p2, $"DistanceToPlane2_{tag}_P2_({p2.Vector3ToString()})_{dis12}/{dis}", t, 0.01f);
        }
        return dis;
    }

    public Vector2 GetSize()
    {
        var minMax = VertexHelper.GetMinMax(this.Plane1Points.ToArray());
        var size = minMax[2];
        List<float> size123 = new List<float>();
        size123.Add(size.x);
        size123.Add(size.y);
        size123.Add(size.z);
        size123.Sort();
        return new Vector2(size123[1], size123[2]);
    }

    public float DistanceToPlaneEx(VerticesToPlaneInfo plane2, Transform t, string tag, bool isShowPoint = false,float pointScale=0.01f)
    {
        var ps1 = new List<Vector3>(this.Plane1Points);
        var ps2 = new List<Vector3>(plane2.Plane1Points);
        foreach (var p in ps1)
        {
            plane2.Plane1Points.Remove(p);
            plane2.Plane2Points.Add(p);
        }
        foreach (var p in ps2)
        {
            this.Plane1Points.Remove(p);
            this.Plane2Points.Add(p);
        }

        Vector3 p1 = this.GetMinVector3();
        //Vector3 normal1 = this.Plane.planeNormal;
        Vector3 normal1 = this.NewPlane1().planeNormal;
        //Vector3 p2 = VertexHelper.GetClosedPoint(p1, plane2.Plane1Points);
        //Vector3 p3 = VertexHelper.GetClosedPoint(p2, this.Plane1Points);

        Vector3 p2 = VertexHelper.GetClosedPointToLine(p1, normal1, plane2.Plane1Points, false, t);
        //Vector3 normal2 = plane2.Plane.planeNormal;
        Vector3 normal2 = normal1;
        Vector3 p3 = VertexHelper.GetClosedPointToLine(p2, normal2, this.Plane1Points, false, t);
        Vector3 p4 = VertexHelper.GetClosedPointToLine(p3, normal1, plane2.Plane1Points, false, t);

        float dis12 = Vector3.Distance(p1, p2);
        float dot12 = Vector3.Angle(p2 - p1, normal1);
        float dis23 = Vector3.Distance(p3, p2);
        float dot23 = Vector3.Angle(p3 - p2, normal1);
        float dis34 = Vector3.Distance(p3, p4);
        float dot34=Vector3.Angle(p4 - p3, normal1);

        float dis = dis34;

        Vector3 lineP = Math3D.ProjectPointOnLine(p3, normal1, p4);

        float dis2 = Vector3.Distance(lineP, p3);
        if (lineP == p3 || dis2==0)
        {
            if (isShowLog)
            {
                Debug.LogError($"DistanceToPlaneEx_{tag}[{t}] lineP == p3 dis:{dis} dis2:{dis2}");
            }
        }
        else
        {
            dis = dis2;
        }
       
        float dis5 = Vector3.Distance(lineP, p4);

        if (dis5 == dis34)
        {
            if (isShowLog)
            {
                Debug.LogError($"DistanceToPlaneEx_{tag}[{t}] dis5 == dis34 dis5:{dis}");
            }
        }
        float dot5 = Vector3.Angle(lineP - p3, normal1);

        //if (dot34 < 1 || 180 - dot34 < 1)
        //{
        //}
        //else
        //{
        //    Debug.LogError($"DistanceToPlaneEx Error :{dot34} name:{t.name} tag:{tag}");
        //}

        
        //if (dis23 < dis)
        //{
        //    dis = dis23;
        //}

        if (isShowPoint)
        {
            PointHelper.CreateLocalPoint(p1, $"DisToPlane3_{tag}_P1_({p1.Vector3ToString3()})_{dis12}/{dis}", t, pointScale);
            PointHelper.CreateLocalPoint(p2, $"DisToPlane3_{tag}_P2_({p2.Vector3ToString3()})_{dis23}/{dis}_{dot12}", t, pointScale);
            PointHelper.CreateLocalPoint(p3, $"DisToPlane3_{tag}_P3_({p3.Vector3ToString3()})_{dis34}/{dis}_{dot23}", t, pointScale);
            PointHelper.CreateLocalPoint(p4, $"DisToPlane3_{tag}_P4_({p4.Vector3ToString3()})_{dis34}/{dis}_{dot34}", t, pointScale);
            PointHelper.CreateLocalPoint(lineP, $"DisToPlane3_{tag}_P5_({lineP.Vector3ToString3()})_{dis5}/{dis}_{dot5}", t, pointScale);
        }
        return dis;
    }

    public float GetPlaneHeight(Transform t, string tag)
    {
        Vector3 p1 = Plane1Points1[0];
        Vector3 p2 = VertexHelper.GetClosedPoint(p1, Plane1Points2);
        float dis = Vector3.Distance(p1, p2);
        PointHelper.CreateLocalPoint(p1, $"DistanceOfPlane12_{tag}_P1_{dis}", t, 0.01f);
        PointHelper.CreateLocalPoint(p2, $"DistanceOfPlane12_{tag}_P2_{dis}", t, 0.01f);
        return dis;
    }

    public Vector3 GetClosedPlanePoint1(Vector3 p)
    {
        return VertexHelper.GetClosedPoint(p, Plane1Points);
    }

    public Vector3 GetClosedPlanePoint2(Vector3 p)
    {
        return VertexHelper.GetClosedPoint(p, Plane2Points);
    }



    //public Vector3 GetClosedPlanePoint2(Vector3 p)
    //{
    //    var list = Plane2Points;
    //    float minD = float.MaxValue;
    //    int minI = 0;
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        float dis = Vector3.Distance(list[i], p);
    //        if (dis < minD)
    //        {
    //            minD = dis;
    //            minI = i;
    //        }
    //    }
    //    return list[minI];
    //}

    public Vector3 GetMinVector3()
    {
        return dict1[minDis][0];
    }

    public Vector3 GetMaxVector3()
    {
        return dict1[maxDis][0];
    }

    public PlaneInfo Plane;

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

    public float maxDis = 0;
    public float minDis = float.MaxValue;

    //public Vector3 minPoint;
    //public Vector3 maxPoint;

    public List<Vector3> Plane1Points = new List<Vector3>();

    public PlaneInfo NewPlane1()
    {
        if (Plane1Points.Count >= 3)
        {
            return new PlaneInfo(Plane1Points);
        }
        else
        {
            return new PlaneInfo();
        }
    }

    public List<Vector3> Plane1Points1 = new List<Vector3>();

    public List<Vector3> Plane1Points2 = new List<Vector3>();

    public List<Vector3> Plane2Points = new List<Vector3>();

    public int Plane1PointCount
    {
        get
        {
            return Plane1Points.Count;
        }
    }

    public int Plane2PointCount
    {
        get
        {
            return Plane2Points.Count;
        }
    }

    public bool IsCount(int c1,int c2)
    {
        return Plane1PointCount == c1 && Plane2PointCount == c2;
    }

    public float planeClosedMinDis = 0.00025f;

    public int planeClosedMaxCount1 = 20;
    public int planeClosedMaxCount2 = 100;

    private void SetCount()
    {
        Count0 = dict10.Count;
        Count1 = dict11.Count;
        Count2 = dict12.Count;
        Count3 = dict13.Count;
        Count4 = dict14.Count;
        Count5 = dict15.Count;

        if (Count0 == 0)
        {
            Count0 = int.MaxValue;
        }
        if (Count1 == 0)
        {
            Count1 = int.MaxValue;
        }
        if (Count2 == 0)
        {
            Count2 = int.MaxValue;
        }
        if (Count3 == 0)
        {
            Count3 = int.MaxValue;
        }
        if (Count4 == 0)
        {
            Count4 = int.MaxValue;
        }
        if (Count5 == 0)
        {
            Count5 = int.MaxValue;
        }

        if (Count0 == 1)
        {
            Count0 = int.MaxValue - 1;
        }
        if (Count1 == 1)
        {
            Count1 = int.MaxValue - 1;
        }
        if (Count2 == 1)
        {
            Count2 = int.MaxValue - 1;
        }
        if (Count3 == 1)
        {
            Count3 = int.MaxValue - 1;
        }
        if (Count4 == 1)
        {
            Count4 = int.MaxValue - 1;
        }
        if (Count5 == 1)
        {
            Count5 = int.MaxValue - 1;
        }
    }

    public void SplitToTwoPlane(float percent = 0.6f)
    {
        float middelDis = (minDis + maxDis) / 2 * percent;

        Plane1Points.Clear();
        Plane1Points1.Clear();
        Plane1Points2.Clear();
        Plane2Points.Clear();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 v = points[i];
            float dis1 = Math3D.SignedDistancePlanePoint(Plane.planeNormal, Plane.planePoint, v);
            float dis = Math.Abs(dis1);
            dict1.AddItem(dis, v);

            if (dis < middelDis)
            {
                if (!Plane1Points.Contains(v))
                {
                    Plane1Points.Add(v);
                }
            }
            else
            {
                if (!Plane2Points.Contains(v))
                {
                    Plane2Points.Add(v);
                }
            }
        }
    }

    public void AddPlane2PointsEx(float percent = 0.1f,float percent2=0.5f)
    {
        float mDis = (minDis + maxDis) * percent2;
        float middelDis2 = mDis;
        float middelDis1 = mDis * percent;

        //Plane1Points.Clear();
        //Plane1Points1.Clear();
        //Plane1Points2.Clear();
        Plane2Points.Clear();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 v = points[i];
            float dis1 = Math3D.SignedDistancePlanePoint(Plane.planeNormal, Plane.planePoint, v);
            float dis = Math.Abs(dis1);
            dict1.AddItem(dis, v);

            //if (dis < middelDis1)
            //{
            //    if (!Plane1Points.Contains(v))
            //    {
            //        Plane1Points.Add(v);
            //    }
            //}
            //else if (dis < middelDis2)
            //{
            //    if (!Plane2Points.Contains(v))
            //    {
            //        Plane2Points.Add(v);
            //    }
            //}

            if (dis < middelDis2)
            {
                if (Plane1Points.Contains(v))
                {
                    continue;
                }
                if (!Plane2Points.Contains(v))
                {
                    Plane2Points.Add(v);
                }
            }
        }

        SetResultInfo();
    }

    public static string logTag = "";

    private void GetPlanePoints(List<Vector3> list, PlaneInfo p, int plane1MaxCount = 8, int plane2MaxCount = 4)
    {
        List<Vector3> allPs = new List<Vector3>(list);

        if (dict1.ContainsKey(minDis))
        {
            foreach (Vector3 v in dict1[minDis])
            {
                float dis1 = Math3D.SignedDistancePlanePoint(p.planeNormal, p.planePoint, v);
                //float dis = Math.Abs(dis1);

                if (!Plane1Points.Contains(v))
                {
                    Plane1Points.Add(v);
                    if (dis1 > 0)
                    {
                        Plane1Points1.Add(v);
                    }
                    else
                    {
                        Plane1Points2.Add(v);
                    }
                }
                allPs.Remove(v);
            }
        }
        else
        {
            var keys = dict1.Keys.ToList();
            float key0 = keys[0];
            Debug.LogError($"GetPlanePoints[{logTag}] dict1.ContainsKey(minDis) = false list:{list.Count} dict1:{dict1.Count}  minDis:{minDis} keys:{keys.Count} key0:{key0} plane:{Plane}");
        }

        float maxDis2 = maxDis / 2f;
        int j = 0;
        for (; j < planeClosedMaxCount1; j++)
        {
            float planeClosedMinDis0 = planeClosedMinDis * (j + 1);
            if (planeClosedMinDis0 > maxDis2) break;

            for (int i = 0; i < allPs.Count; i++)
            {
                Vector3 v = allPs[i];
                float dis1 = Math3D.SignedDistancePlanePoint(p.planeNormal, p.planePoint, v);
                float dis = Math.Abs(dis1);
                dict1.AddItem(dis, v);

                if (dis < planeClosedMinDis0)
                {
                    if (!Plane1Points.Contains(v))
                    {
                        Plane1Points.Add(v);
                        if (dis1 > 0)
                        {
                            Plane1Points1.Add(v);
                        }
                        else
                        {
                            Plane1Points2.Add(v);
                        }
                    }
                    allPs.Remove(v);
                }
            }
            if (plane1MaxCount > 0)
            {
                if (Plane1Points.Count > plane1MaxCount)
                {
                    break;
                }
            }
        }
        for (int k = j; k < planeClosedMaxCount2; k++)
        {
            float planeClosedMinDis0 = planeClosedMinDis * (k + 1);
            if (planeClosedMinDis0 > maxDis2) break;
            for (int i = 0; i < allPs.Count; i++)
            {
                Vector3 v = allPs[i];
                float dis = Math.Abs(Math3D.SignedDistancePlanePoint(p.planeNormal, p.planePoint, v));
                dict1.AddItem(dis, v);

                if (dis < planeClosedMinDis0)
                {
                    if (!Plane2Points.Contains(v))
                    {
                        Plane2Points.Add(v);
                    }
                    allPs.Remove(v);
                }
            }
            if (plane2MaxCount > 0)
            {
                if (Plane2Points.Count > plane2MaxCount)
                {
                    break;
                }
            }

        }
    }

    private List<Vector3> points = new List<Vector3>();

    private bool isShowLog;

    public VerticesToPlaneInfo(Vector3[] vs, PlaneInfo p, bool isShowLog, float planeClosedMinDis = 0.00025f, int planeClosedMaxCount1 = 20, int planeClosedMaxCount2 = 100)
    {
        this.planeClosedMinDis = planeClosedMinDis;
        this.planeClosedMaxCount1 = planeClosedMaxCount1;
        this.planeClosedMaxCount2 = planeClosedMaxCount2;

        Plane = p;
        this.isShowLog = isShowLog;

        InitInfo(vs);
    }

    protected void InitInfo(Vector3[] vs)
    {
        Plane1Points = new List<Vector3>();
        Plane1Points1 = new List<Vector3>();
        Plane1Points2 = new List<Vector3>();
        Plane2Points = new List<Vector3>();

        PlaneInfo p = Plane;
        List<Vector3> allPs = new List<Vector3>();
        for (int i = 0; i < vs.Length; i++)
        {
            Vector3 v = vs[i];
            float dis1 = Math3D.SignedDistancePlanePoint(p.planeNormal, p.planePoint, v);
            float dis = Math.Abs(dis1);
            dict1.AddItem(dis, v);

            if (dis < planeClosedMinDis)
            {
                if (!Plane1Points.Contains(v))
                {
                    Plane1Points.Add(v);

                    if (dis1 > 0)
                    {
                        Plane1Points1.Add(v);
                    }
                    else
                    {
                        Plane1Points2.Add(v);
                    }
                }

            }

            if (!allPs.Contains(v))
            {
                allPs.Add(v);
            }

            if (dis > maxDis)
            {
                maxDis = dis;

            }
            if (dis < minDis)
            {
                minDis = dis;
                //minPoint = v;
            }
            //float disToMax=
            int dis10 = (int)dis * 10;
            int dis100 = (int)dis * 100;
            int dis1000 = (int)dis * 1000;

            string sDis0 = dis.ToString("F0");
            //if (sDis0 != "0")
            {
                dict10.AddItem(sDis0, v);
            }

            string sDis1 = dis.ToString("F1");
            //if(sDis1!="0.0")
            {
                dict11.AddItem(sDis1, v);
            }

            string sDis2 = dis.ToString("F2");
            //if (sDis2 != "0.00")
            {
                dict12.AddItem(sDis2, v);
            }

            string sDis3 = dis.ToString("F3");
            //if (sDis3 != "0.000")
            {
                dict13.AddItem(sDis3, v);
            }

            string sDis4 = dis.ToString("F4");
            //if (sDis4 != "0.0000")
            {
                dict14.AddItem(sDis4, v);
            }

            string sDis5 = dis.ToString("F5");
            //if (sDis5 != "0.0000")
            {
                dict15.AddItem(sDis5, v);
            }

            if (isShowLog)
            {
                Debug.Log($"Point2Vertices[{i + 1}] \tp:{p} \tdis:{dis} \tsDis0:{sDis0} \tsDis1:{sDis1} \tsDis2:{sDis2} \tsDis3:{sDis3} \tsDis4:{sDis4} \tsDis5:{sDis5} \tv:{v} ");
            }
        }

        points = new List<Vector3>(allPs);

        SetCount();

        GetPlanePoints(allPs, p, 0, 0);

        //float middleDist = (minDis + maxDis) / 2;
        //for (int i = 0; i < vs.Length; i++)
        //{
        //    Vector3 v = vs[i];
        //    float dis = Math.Abs(Math3D.SignedDistancePlanePoint(p.planeNormal, p.planePoint, v));
        //    if (dis < middleDist)
        //    {
        //        ClosedPoints.Add(v);
        //    }
        //}

        SetResultInfo();
    }

    private void SetResultInfo()
    {
        ResultInfo = $"{dict10.Count}_{dict11.Count}_{dict12.Count}_{dict13.Count}_{dict14.Count}_{dict15.Count}_{dict1.Count}_[{minDis:F5}({dict1.GetListCount(minDis)})-{maxDis:F5}({dict1.GetListCount(maxDis)})]({Plane1Points.Count}_{Plane2Points.Count})_{this.GetCircleInfoString()}";
        if (isShowLog)
            Debug.LogError(ResultInfo);
    }


    public void AddPlane1Points(bool isShowLog)
    {
        var r1 = ResultInfo;

        var p1 = Plane1Points[0];
        var p2 = Plane2Points[0];
        Plane1Points.Add(p2);
        Plane2Points.Remove(p2);

        //SetResultInfo();

        Plane = NewPlane1();
        InitInfo(points.ToArray());

        var r2 = ResultInfo;

        if (isShowLog)
        {
            Debug.Log($"AddPlane1Points r2:{r2}\nr1:{r1}");
        }
        
    }

    public CircleInfo GetPlane1Circle()
    {
        return new CircleInfo(Plane1Points);
    }

    public CircleInfo GetCircleInfo()
    {
        if (Count5 == 2)
        {
            return GetCircleInfo(dict15);
        }
        else if (Count4 == 2)
        {
            return GetCircleInfo(dict14);
        }
        else if (Count3 == 2)
        {
            return GetCircleInfo(dict13);
        }
        //else if (Count2 == 2)
        //{
        //    return GetCircleInfo(dict12);
        //}
        //else if (Count1 == 2)
        //{
        //    return GetCircleInfo(dict11);
        //}
        else if (maxDis > 1f && Count2 == 2)
        {
            return GetCircleInfo(dict12);
        }
        else if (maxDis > 1f && Count1 == 2)
        {
            return GetCircleInfo(dict11);
        }
        //else if (Count0 == 2)
        //{
        //    return GetCircleInfo(dict10);
        //}
        else
        {
            //Debug.LogWarning($"GetCircleInfo Warning {this.ToString()}");
            //return new CircleInfo(Point.planeCenter, 0);

            if (Count5 == 3)
            {
                return GetCircleInfo(dict15);
            }
            else if (Count4 == 3)
            {
                return GetCircleInfo(dict14);
            }
            else if (Count3 == 3)
            {
                return GetCircleInfo(dict13);
            }
            else if (Count2 == 3)
            {
                return GetCircleInfo(dict12);
            }
            //else if (Count1 == 3)
            //{
            //    return GetCircleInfo(dict11);
            //}
            //else if (Count0 == 3)
            //{
            //    return GetCircleInfo(dict10);
            //}
            else
            {
                if (Count5 <= 6)
                {
                    return GetCircleInfo(dict15);
                }
                else if (Count4 <= 6)
                {
                    return GetCircleInfo(dict14);
                }
                else if (Count3 <= 6)
                {
                    return GetCircleInfo(dict13);
                }
                //else if (Count2 <= 6)
                //{
                //    return GetCircleInfo(dict12);
                //}
                //Debug.LogError($"GetCircleInfo Error {this.ToString()}");
                //return new CircleInfo(Point, 0);
                return null;
            }

            //return GetCenter(dict15);
            //return Point;
        }
    }

    public bool IsCircle()
    {
        CircleInfo circle = GetCircleInfo();
        if (circle == null) return false;
        return circle.IsCircle;
    }

    public string GetCircleInfoString()
    {
        string info = "";
        CircleInfo circle = GetCircleInfo();
        if (circle == null)
        {
            info = "False";
        }
        else
        {
            info = $"{circle.IsCircle}({circle.CheckCircleP})";
        }
        return info;
    }

    private CircleInfo GetCircleInfo(DictionaryList1ToN<string, Vector3> dict)
    {
        var keys = dict.Keys.ToList();
        keys.Sort();
        int id = 0;
        string firstKey = keys[id];
        var vs = dict[firstKey];
        var vs3 = new List<Vector3>();
        vs3.AddRange(vs);
        while (vs3.Count < 36 && id < keys.Count)//36 是 最小的园的顶点数量
        {
            id++;
            if (id >= keys.Count)
            {
                break;
            }
            string secondKey = keys[id];
            var vs2 = dict[secondKey];
            vs3.AddRange(vs2);
        }
        return new CircleInfo(vs3);

        //Vector3 sum = Vector3.zero;
        //for (int i = 0; i < vs.Count; i++)
        //{
        //    Vector3 v = vs[i];
        //    sum += v;
        //}
        //Vector3 center = sum / vs.Count;
        //float radiusSum = 0;
        //for (int i = 0; i < vs.Count; i++)
        //{
        //    Vector3 v = vs[i];
        //    radiusSum += Vector3.Distance(v, center);
        //}
        //float radius = radiusSum / vs.Count;

    }

    public override string ToString()
    {
        return $"[{ResultInfo}]";
    }

    public int CompareTo(VerticesToPlaneInfo other)
    {
        //int r = this.Count0.CompareTo(other.Count0);//F0
        //if (r == 0)
        //{
        //    r = this.Count1.CompareTo(other.Count1);//F1
        //}
        //if (r == 0)
        //{
        //    r = this.Count2.CompareTo(other.Count2);//F2
        //}

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
