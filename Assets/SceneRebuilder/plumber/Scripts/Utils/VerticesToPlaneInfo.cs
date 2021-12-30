using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VerticesToPlaneInfo : IComparable<VerticesToPlaneInfo>
{
    public PlaneInfo Point;

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

    public VerticesToPlaneInfo(Vector3[] vs, PlaneInfo p, bool isShowLog)
    {
        Point = p;
        for (int i = 0; i < vs.Length; i++)
        {
            Vector3 v = vs[i];
            float dis = Math3D.SignedDistancePlanePoint(p.planeNormal, p.planePoint, v);
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
            Count0 = int.MaxValue;
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

        ResultInfo = $"{dict10.Count}_{dict11.Count}_{dict12.Count}_{dict13.Count}_{dict14.Count}_{dict15.Count}_{dict1.Count}";
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
        else if (Count2 == 2)
        {
            return GetCircleInfo(dict12);
        }
        else if (Count1 == 2)
        {
            return GetCircleInfo(dict11);
        }
        else if (Count0 == 2)
        {
            return GetCircleInfo(dict10);
        }
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
                else if (Count2 <= 6)
                {
                    return GetCircleInfo(dict12);
                }
                //Debug.LogError($"GetCircleInfo Error {this.ToString()}");
                //return new CircleInfo(Point, 0);
                return null;
            }

            //return GetCenter(dict15);
            //return Point;
        }
    }

    private CircleInfo GetCircleInfo(DictionaryList1ToN<string, Vector3> dict)
    {
        var keys = dict.Keys.ToList();
        keys.Sort();
        string firstKey = keys[0];
        var vs = dict[firstKey];



        Vector3 sum = Vector3.zero;
        for (int i = 0; i < vs.Count; i++)
        {
            Vector3 v = vs[i];
            sum += v;
        }
        Vector3 center = sum / vs.Count;
        float radiusSum = 0;
        for (int i = 0; i < vs.Count; i++)
        {
            Vector3 v = vs[i];
            radiusSum += Vector3.Distance(v, center);
        }
        float radius = radiusSum / vs.Count;
        return new CircleInfo(center, radius);
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
