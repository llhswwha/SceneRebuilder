using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VerticesToPlaneInfo : IComparable<VerticesToPlaneInfo>
{


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
        foreach(var p1 in ps1)
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

    public float DebugDistanceOfPlane12(Transform t, string tag)
    {
        Vector3 p1 = Plane1Points[0];
        Vector3 p2 = GetClosedPlanePoint1(p1, Plane1Points);
        float dis = Vector3.Distance(p1, p2);
        MeshHelper.CreateLocalPoint(p1, $"DistanceOfPlane12_{tag}_P1_{dis}", t, 0.01f);
        MeshHelper.CreateLocalPoint(p2, $"DistanceOfPlane12_{tag}_P2_{dis}", t, 0.01f);
        return dis;
    }

    public float DebugDistanceToPlane(VerticesToPlaneInfo plane2,Transform t,string tag)
    {
        Vector3 p1 = this.GetMinVector3();
        Vector3 p2 = plane2.GetClosedPlanePoint1(p1, Plane1Points);

        //var list = plane2.ClosedPoints;
        //float minD = float.MaxValue;
        //int minI = 0;
        //for (int i = 0; i < list.Count; i++)
        //{
        //    float dis0 = Vector3.Distance(list[i], p1);
        //    if (dis0 < minD)
        //    {
        //        minD = dis0;
        //        minI = i;
        //    }
        //    MeshHelper.CreateLocalPoint(list[i], $"{tag}_P2[{i}]_{dis0}", t, 0.1f);
        //}
        //Vector3 p2 = list[minI];

        float dis = Vector3.Distance(p1, p2);
        MeshHelper.CreateLocalPoint(p1, $"DistanceToPlane_{tag}_P1_{dis}", t, 0.01f);
        MeshHelper.CreateLocalPoint(p2, $"DistanceToPlane_{tag}_P2_{dis}", t, 0.01f);
        return dis;
    }

    public float GetPlaneHeight(Transform t, string tag)
    {
        Vector3 p1 = Plane1Points1[0];
        Vector3 p2 = GetClosedPlanePoint1(p1, Plane1Points2);
        float dis = Vector3.Distance(p1, p2);
        MeshHelper.CreateLocalPoint(p1, $"DistanceOfPlane12_{tag}_P1_{dis}", t, 0.01f);
        MeshHelper.CreateLocalPoint(p2, $"DistanceOfPlane12_{tag}_P2_{dis}", t, 0.01f);
        return dis;
    }

    public Vector3 GetClosedPlanePoint1(Vector3 p)
    {
        return GetClosedPlanePoint1(p, Plane1Points);
    }

    public Vector3 GetClosedPlanePoint2(Vector3 p)
    {
        return GetClosedPlanePoint1(p, Plane2Points);
    }

    public Vector3 GetClosedPlanePoint1(Vector3 p, List<Vector3> list)
    {
        //var list = Plane1Points;
        float minD = float.MaxValue;
        int minI = 0;
        for (int i = 0; i < list.Count; i++)
        {
            float dis = Vector3.Distance(list[i], p);
            if (dis < minD)
            {
                minD = dis;
                minI = i;
            }
        }
        return list[minI];
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

    public List<Vector3> Plane1Points = new List<Vector3>();

    public List<Vector3> Plane1Points1 = new List<Vector3>();

    public List<Vector3> Plane1Points2 = new List<Vector3>();

    public List<Vector3> Plane2Points = new List<Vector3>();

    public static float planeClosedMinDis = 0.00025f;

    public static int planeClosedMaxCount1 = 20;
    public static int planeClosedMaxCount2 = 100;

    public VerticesToPlaneInfo(Vector3[] vs, PlaneInfo p, bool isShowLog)
    {
       
        Plane = p;
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
            Count0 = int.MaxValue-1;
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



        foreach(var v in dict1[minDis])
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

        int j = 0;
        for (;j< planeClosedMaxCount1; j++)
        {
            float planeClosedMinDis0 = planeClosedMinDis * (j + 1);
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
            if (Plane1Points.Count > 8)
            {
                break;
            }
        }
        for (int k = j; k < planeClosedMaxCount2; k++)
        {
            float planeClosedMinDis0 = planeClosedMinDis * (k + 1);
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
            if (Plane2Points.Count > 4)
            {
                break;
            }
        }

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

            ResultInfo = $"{dict10.Count}_{dict11.Count}_{dict12.Count}_{dict13.Count}_{dict14.Count}_{dict15.Count}_{dict1.Count}_[{minDis}({dict1[minDis].Count})-{maxDis}({dict1[maxDis].Count})]({Plane1Points.Count}_{Plane2Points.Count})_{this.GetCircleInfoString()}";
        if (isShowLog)
            Debug.LogError(ResultInfo);
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

    internal bool IsCircle()
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
            info="False";
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
        while (vs3.Count < 36 && id<keys.Count)//36 是 最小的园的顶点数量
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
        return ResultInfo;
    }

    public int CompareTo(VerticesToPlaneInfo other)
    {
        int r = this.Count0.CompareTo(other.Count0);
        if (r == 0)
        {
            r = this.Count1.CompareTo(other.Count1);
        }
        if (r == 0)
        {
            r = this.Count2.CompareTo(other.Count2);
        }
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
