using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DistanceUtil
{
    public static float GetDistance(Unity.Collections.NativeArray<Vector3> points1, Vector3[] points2, bool showLog = false)
    {
        //Debug.Log("GetDistance:"+showLog);
        DateTime start = DateTime.Now;
        //float dis=-1;
        float disSum = 0;

        float distance = 0;
        // var points1 = vertices1;
        // var points2 = vertices2World;
        //List<float> disList = new List<float>();
        int zeroCount = 0;
        int i = 0;

        int maxCount = points1.Length;
        int zeroPMaxCount = DistanceSetting.zeroPMaxCount;
        int step = 1;
        if (maxCount > zeroPMaxCount)
        {
            step = maxCount / zeroPMaxCount;
        }

        //for (; i < points1.Length & i<points2.Length; i++)
        int count = 0;
        for (; i < points1.Length; i += step, count++)
        {
            Vector3 p1 = points1[i];
            Vector3 p2 = GetMinDistancePoint(p1, points2);
            float d = Vector3.Distance(p1, p2);
            disSum += d;//不做处理，直接累计
            if (d <= DistanceSetting.zeroP)
            {
                if (showLog) Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
                zeroCount++;
                if (zeroCount > zeroPMaxCount)//没必要计算完，大概100个都位置相同的话，就是可以的了。
                {
                    //return distance;//不能返回0哦
                    break;
                }
                else
                {

                }
                d = 0;//不考虑累计，小于zero就是0了。，比如10个E-06就E-05，100个就是E-04，1000个就是E-03了，0.001了，那我就不是很有把握是不是重合了。
            }
            else
            {
                if (showLog) Debug.Log($"GetDistance2[{i}]d2:{d}|{distance}");
            }
            //disList.Add(d);
            distance += d;

            if (distance > DistanceSetting.zeroMMaxDis)//没必要计算完，整体距离很大的话，就是已经是不行的了
            {
                break;
            }
        }
        if (showLog) Debug.Log($"GetVertexDistanceEx 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms，累计:{disSum:F7},结果:{distance:F7},序号:{i}/{points1.Length} count:{count}");
        return distance;
    }

    // public static Vector3[] GetClosedPoints(Vector3[] points1, Vector3[] points2, bool showLog = false)
    // {
    //     //points1：不变的点，vsTo,
    //     //points2：变化的点，vsFrom，将points2慢慢变成对齐points1，是算法中的目标点云P
    //     Vector3[] points3 = new Vector3[points2.Length];//结果点集,Pi<=P,是points2中的最接近points1中的点集，按顺序一一对应。
    //     //有个问题，points1获取了一个point2中的点后，是否要将该点删除？
    //     DateTime start = DateTime.Now;
    //     //float disSum = 0;
    //     int i = 0;
    //     for (; i < points1.Length & i < points2.Length; i++)
    //     {
    //         Vector3 p1 = points1[i];
    //         Vector3 p2 = GetMinDistancePoint(p1, points2);
    //         points3[i] = p2;
    //         //float d = Vector3.Distance(p1, p2);
    //         //disSum += d;//不做处理，直接累计
    //     }
    //     if (showLog) Debug.Log($"GetClosedPoints 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms");
    //     return points3;
    // }

    public static Vector3[] GetClosedPoints(Vector3[] points1, List<Vector3> points2, bool showLog = false)
    {
        //points1：不变的点，vsTo,
        //points2：变化的点，vsFrom，将points2慢慢变成对齐points1，是算法中的目标点云P
        Vector3[] points3 = new Vector3[points2.Count];//结果点集,Pi<=P,是points2中的最接近points1中的点集，按顺序一一对应。
        //有个问题，points1获取了一个point2中的点后，是否要将该点删除？
        DateTime start = DateTime.Now;
        //float disSum = 0;
        int i = 0;
        int count1 = points1.Length;
        int count2 = points2.Count;
        for (; i < count1 & i < count2; i++)
        {
            Vector3 p1 = points1[i];
            Vector3 p2 = GetMinDistancePoint(p1, points2, true);
            points3[i] = p2;
            //float d = Vector3.Distance(p1, p2);
            //disSum += d;//不做处理，直接累计
        }
        if (showLog) Debug.Log($"GetClosedPoints 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms");
        return points3;
    }

    public static float GetDistance(List<Vector3> points1, List<Vector3> points2, bool showLog = false)
    {
        //Debug.Log("GetDistance:"+showLog);
        DateTime start = DateTime.Now;
        //float dis=-1;
        float disSum = 0;

        float distance = 0;
        // var points1 = vertices1;
        // var points2 = vertices2World;
        //List<float> disList = new List<float>();
        int zeroCount = 0;
        int i = 0;

        int maxCount = points1.Count;
        int zeroPMaxCount = DistanceSetting.zeroPMaxCount;
        int step = 1;
        if (maxCount > zeroPMaxCount)
        {
            step = maxCount / zeroPMaxCount;
        }
        int count = 0;

        for (; i < points1.Count; i += step, count++)
        {
            Vector3 p1 = points1[i];
            Vector3 p2 = GetMinDistancePoint(p1, points2);
            float d = Vector3.Distance(p1, p2);
            disSum += d;//不做处理，直接累计
            if (d <= DistanceSetting.zeroP)
            {
                if (showLog)
                {
                    Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
                }
                zeroCount++;
                if (zeroCount > zeroPMaxCount)//没必要计算完，大概100个都位置相同的话，就是可以的了。
                {
                    //return distance;//不能返回0哦
                    break;
                }
                else
                {

                }
                d = 0;//不考虑累计，小于zero就是0了。，比如10个E-06就E-05，100个就是E-04，1000个就是E-03了，0.001了，那我就不是很有把握是不是重合了。
            }
            else
            {
                if (showLog)
                {
                    Debug.LogWarning($"GetDistance2[{i}]d2:{d}|{distance}");
                }
            }
            //disList.Add(d);
            distance += d;

            if (distance > DistanceSetting.zeroMMaxDis)//没必要计算完，整体距离很大的话，就是已经是不行的了
            {
                break;
            }
        }

        DisLog = $"GetVertexDistanceEx points1:{points1.Count} points2:{points2.Count} 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms，累计:{disSum:F7},结果:{distance:F7},序号:{i}/{points1.Count} count:{count}";
        if (showLog) Debug.Log(DisLog);
        return distance;
    }

    public static float GetDistance(Vector3[] points1, Vector3[] points2, bool showLog = false)
    {
        //Debug.Log("GetDistance:"+showLog);
        DateTime start = DateTime.Now;
        //float dis=-1;
        float disSum = 0;

        float distance = 0;
        // var points1 = vertices1;
        // var points2 = vertices2World;
        //List<float> disList = new List<float>();
        int zeroCount = 0;
        int i = 0;

        int maxCount = points1.Length;
        int zeroPMaxCount = DistanceSetting.zeroPMaxCount;
        int step = 1;
        if (maxCount > zeroPMaxCount)
        {
            step = maxCount / zeroPMaxCount;
        }
        int count = 0;

        for (; i < points1.Length; i += step, count++)
        {
            Vector3 p1 = points1[i];
            Vector3 p2 = GetMinDistancePoint(p1, points2);
            float d = Vector3.Distance(p1, p2);
            disSum += d;//不做处理，直接累计
            if (d <= DistanceSetting.zeroP)
            {
                if (showLog)
                {
                    Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
                }
                zeroCount++;
                if (zeroCount > zeroPMaxCount)//没必要计算完，大概100个都位置相同的话，就是可以的了。
                {
                    //return distance;//不能返回0哦
                    break;
                }
                else
                {

                }
                d = 0;//不考虑累计，小于zero就是0了。，比如10个E-06就E-05，100个就是E-04，1000个就是E-03了，0.001了，那我就不是很有把握是不是重合了。
            }
            else
            {
                if (showLog)
                {
                    Debug.LogWarning($"GetDistance2[{i}]d2:{d}|{distance} zeroP:{DistanceSetting.zeroP}");
                }
            }
            //disList.Add(d);
            distance += d;

            if (distance > DistanceSetting.zeroMMaxDis)//没必要计算完，整体距离很大的话，就是已经是不行的了
            {
                break;
            }
        }

        DisLog = $"GetVertexDistanceEx points1:{points1.Length} points2:{points2.Length} 用时:{(DateTime.Now - start).TotalMilliseconds:F2}ms，累计:{disSum:F7},结果:{distance:F7},序号:{i}/{points1.Length} count:{count} zeroPMaxCount:{zeroPMaxCount} step:{step} zeroP:{DistanceSetting.zeroP}";
        if (showLog) Debug.Log(DisLog);
        return distance;
    }

    public static string DisLog = "";

    public static Vector3 GetMinDistancePoint(Vector3 p1, Vector3[] points)
    {
        //DateTime start = DateTime.Now;

        float distance = float.MaxValue;
        Vector3 result = Vector3.zero;
        int index = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 p2 = points[i];
            float dis = Vector3.Distance(p1, p2);
            if (dis < distance)
            {
                distance = dis;
                result = p2;
                index = i;
            }
        }
        //if(distance<zero)
        //    DebugLog(string.Format("GetMinDistancePoint 用时:{0}s，距离:{1}，序号:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
        return result;
    }

    public static float GetMinDistance(Vector3 p1, Vector3[] points)
    {
        float distance = float.MaxValue;
        Vector3 result = Vector3.zero;
        int index = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 p2 = points[i];
            float dis = Vector3.Distance(p1, p2);
            if (dis < distance)
            {
                distance = dis;
                result = p2;
                index = i;
            }
        }
        return distance;
    }

    public static float GetMinDistance(Vector3 p1, List<Vector3> points)
    {
        float distance = float.MaxValue;
        Vector3 result = Vector3.zero;
        int index = 0;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p2 = points[i];
            float dis = Vector3.Distance(p1, p2);
            if (dis < distance)
            {
                distance = dis;
                result = p2;
                index = i;
            }
        }
        return distance;
    }

    public static Vector3 GetMinDistancePoint(Vector3 p1, List<Vector3> points, bool isRemove = false)
    {
        //DateTime start = DateTime.Now;

        float distance = float.MaxValue;
        Vector3 result = Vector3.zero;
        int index = 0;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p2 = points[i];
            float dis = Vector3.Distance(p1, p2);
            if (dis < distance)
            {
                distance = dis;
                result = p2;
                index = i;
            }
        }
        if (isRemove)
        {
            points.RemoveAt(index);
        }
        //if(distance<zero)
        //    DebugLog(string.Format("GetMinDistancePoint 用时:{0}s，距离:{1}，序号:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
        return result;
    }

}

public static class DistanceSetting
{
    /// <summary>
    /// zero of two vertex point
    /// </summary>
    public static double zeroP = 0.0007f;//1E-05
    public static int zeroPMaxCount = 200;//100

    /// <summary>
    /// zero of two mesh (point clouds)
    /// </summary>
    public static double zeroM = 0.007f;//1E-04

    public static double zeroMMaxDis = 2;//10

    public static int minDistance = 5;

    public static double ICPMinDis = 0.2f;//1E-04

    public static int ICPMaxCount = 20;

    public static bool IsByMat = true;

    public static bool IsShowLog = false;

    public static bool IsDebug = false;

    public static void Set(DisSetting setting)
    {
        zeroP = setting.zeroP;
        zeroPMaxCount = setting.zeroPMaxCount;
        zeroM = setting.zeroM;
        zeroMMaxDis = setting.zeroMMaxDis;
        minDistance = setting.minDistance;
        ICPMinDis = setting.ICPMinDis;
        ICPMaxCount = setting.ICPMaxCount;
        IsByMat = setting.IsByMat;
        IsShowLog = setting.IsShowLog;
    }
}

[Serializable]
public class DisSetting
{
    /// <summary>
    /// zero of two vertex point
    /// </summary>
    public double zeroP = 0.0007f;//1E-05
    public int zeroPMaxCount = 100;

    /// <summary>
    /// zero of two mesh (point clouds)
    /// </summary>
    public double zeroM = 0.007f;//1E-04

    public double zeroMMaxDis = 10;

    public int minDistance = 5;

    public double ICPMinDis = 0.2f;//1E-04

    public int ICPMaxCount = 20;

    public bool IsByMat = true;

    public bool IsShowLog = false;
}
