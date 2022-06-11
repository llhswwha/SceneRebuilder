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
            disSum += d;//��������ֱ���ۼ�
            if (d <= DistanceSetting.zeroP)
            {
                if (showLog) Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
                zeroCount++;
                if (zeroCount > zeroPMaxCount)//û��Ҫ�����꣬���100����λ����ͬ�Ļ������ǿ��Ե��ˡ�
                {
                    //return distance;//���ܷ���0Ŷ
                    break;
                }
                else
                {

                }
                d = 0;//�������ۼƣ�С��zero����0�ˡ�������10��E-06��E-05��100������E-04��1000������E-03�ˣ�0.001�ˣ����ҾͲ��Ǻ��а����ǲ����غ��ˡ�
            }
            else
            {
                if (showLog) Debug.Log($"GetDistance2[{i}]d2:{d}|{distance}");
            }
            //disList.Add(d);
            distance += d;

            if (distance > DistanceSetting.zeroMMaxDis)//û��Ҫ�����꣬�������ܴ�Ļ��������Ѿ��ǲ��е���
            {
                break;
            }
        }
        if (showLog) Debug.Log($"GetVertexDistanceEx ��ʱ:{(DateTime.Now - start).TotalMilliseconds:F2}ms���ۼ�:{disSum:F7},���:{distance:F7},���:{i}/{points1.Length} count:{count}");
        return distance;
    }

    // public static Vector3[] GetClosedPoints(Vector3[] points1, Vector3[] points2, bool showLog = false)
    // {
    //     //points1������ĵ㣬vsTo,
    //     //points2���仯�ĵ㣬vsFrom����points2������ɶ���points1�����㷨�е�Ŀ�����P
    //     Vector3[] points3 = new Vector3[points2.Length];//����㼯,Pi<=P,��points2�е���ӽ�points1�еĵ㼯����˳��һһ��Ӧ��
    //     //�и����⣬points1��ȡ��һ��point2�еĵ���Ƿ�Ҫ���õ�ɾ����
    //     DateTime start = DateTime.Now;
    //     //float disSum = 0;
    //     int i = 0;
    //     for (; i < points1.Length & i < points2.Length; i++)
    //     {
    //         Vector3 p1 = points1[i];
    //         Vector3 p2 = GetMinDistancePoint(p1, points2);
    //         points3[i] = p2;
    //         //float d = Vector3.Distance(p1, p2);
    //         //disSum += d;//��������ֱ���ۼ�
    //     }
    //     if (showLog) Debug.Log($"GetClosedPoints ��ʱ:{(DateTime.Now - start).TotalMilliseconds:F2}ms");
    //     return points3;
    // }

    public static Vector3[] GetClosedPoints(Vector3[] points1, List<Vector3> points2, bool showLog = false)
    {
        //points1������ĵ㣬vsTo,
        //points2���仯�ĵ㣬vsFrom����points2������ɶ���points1�����㷨�е�Ŀ�����P
        Vector3[] points3 = new Vector3[points2.Count];//����㼯,Pi<=P,��points2�е���ӽ�points1�еĵ㼯����˳��һһ��Ӧ��
        //�и����⣬points1��ȡ��һ��point2�еĵ���Ƿ�Ҫ���õ�ɾ����
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
            //disSum += d;//��������ֱ���ۼ�
        }
        if (showLog) Debug.Log($"GetClosedPoints ��ʱ:{(DateTime.Now - start).TotalMilliseconds:F2}ms");
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
            disSum += d;//��������ֱ���ۼ�
            if (d <= DistanceSetting.zeroP)
            {
                if (showLog)
                {
                    Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
                }
                zeroCount++;
                if (zeroCount > zeroPMaxCount)//û��Ҫ�����꣬���100����λ����ͬ�Ļ������ǿ��Ե��ˡ�
                {
                    //return distance;//���ܷ���0Ŷ
                    break;
                }
                else
                {

                }
                d = 0;//�������ۼƣ�С��zero����0�ˡ�������10��E-06��E-05��100������E-04��1000������E-03�ˣ�0.001�ˣ����ҾͲ��Ǻ��а����ǲ����غ��ˡ�
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

            if (distance > DistanceSetting.zeroMMaxDis)//û��Ҫ�����꣬�������ܴ�Ļ��������Ѿ��ǲ��е���
            {
                break;
            }
        }

        DisLog = $"GetVertexDistanceEx points1:{points1.Count} points2:{points2.Count} ��ʱ:{(DateTime.Now - start).TotalMilliseconds:F2}ms���ۼ�:{disSum:F7},���:{distance:F7},���:{i}/{points1.Count} count:{count}";
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
            disSum += d;//��������ֱ���ۼ�
            if (d <= DistanceSetting.zeroP)
            {
                if (showLog)
                {
                    Debug.Log($"GetDistance1[{i}]d1:{d}|{distance}");
                }
                zeroCount++;
                if (zeroCount > zeroPMaxCount)//û��Ҫ�����꣬���100����λ����ͬ�Ļ������ǿ��Ե��ˡ�
                {
                    //return distance;//���ܷ���0Ŷ
                    break;
                }
                else
                {

                }
                d = 0;//�������ۼƣ�С��zero����0�ˡ�������10��E-06��E-05��100������E-04��1000������E-03�ˣ�0.001�ˣ����ҾͲ��Ǻ��а����ǲ����غ��ˡ�
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

            if (distance > DistanceSetting.zeroMMaxDis)//û��Ҫ�����꣬�������ܴ�Ļ��������Ѿ��ǲ��е���
            {
                break;
            }
        }

        DisLog = $"GetVertexDistanceEx points1:{points1.Length} points2:{points2.Length} ��ʱ:{(DateTime.Now - start).TotalMilliseconds:F2}ms���ۼ�:{disSum:F7},���:{distance:F7},���:{i}/{points1.Length} count:{count} zeroPMaxCount:{zeroPMaxCount} step:{step} zeroP:{DistanceSetting.zeroP}";
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
        //    DebugLog(string.Format("GetMinDistancePoint ��ʱ:{0}s������:{1}�����:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
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
        //    DebugLog(string.Format("GetMinDistancePoint ��ʱ:{0}s������:{1}�����:{2}", (DateTime.Now - start).TotalSeconds, distance, index));
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
