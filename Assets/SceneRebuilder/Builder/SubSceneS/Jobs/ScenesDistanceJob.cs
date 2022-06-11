using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct ScenesDistanceJob : IJob
{
    public int Count;
    public NativeArray<Vector3> camPosList;
    public NativeArray<Vector3> camForwardList;

    public NativeArray<Bounds> sceneBounds;

    public NativeArray<Vector3> scenePos;


    public NativeArray<float> disList;

    public NativeArray<float> angleList;

    public static bool IsEnableAngleCheck = true;

    public static bool IsEnableBoundsDis = true;

    public static Vector3[] GetBoundPoints(Bounds bounds)
    {
        var min = bounds.min;
        var max = bounds.max;
        Vector3 p0 = new Vector3(min.x, min.y, min.z);
        Vector3 p1 = new Vector3(min.x, min.y, max.z);
        Vector3 p2 = new Vector3(min.x, max.y, min.z);
        Vector3 p3 = new Vector3(max.x, min.y, min.z);
        Vector3 p4 = new Vector3(min.x, max.y, max.z);
        Vector3 p5 = new Vector3(max.x, min.y, max.z);
        Vector3 p6 = new Vector3(max.x, max.y, min.z);
        Vector3 p7 = new Vector3(max.x, max.y, max.z);
        return new Vector3[9] { bounds.center, p0, p1, p2, p3, p4, p5, p6, p7 };
    }

    public float GetCameraAngle(Bounds bounds,Vector3 camPos,Vector3 camForward)
    {

        Vector3[] ps = GetBoundPoints(bounds);
        float minAngle = 360;
        float maxAngle = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            Vector3 p = ps[i];
            Vector3 dir = p - camPos;
            float angle = Vector3.Angle(camForward, dir);
            if (angle < minAngle)
            {
                minAngle = angle;
            }
            if (angle > maxAngle)
            {
                maxAngle = angle;
            }
            //Debug.Log($"GetCameraAngle[{i}] p:{p} dir:{dir} angle:{angle} minAngle:{minAngle} maxAngle:{maxAngle}  ");
        }
        //return maxAngle; 
        return minAngle;
    }

    public static float MinDisSqrtToCam = float.MaxValue;

    public static  float MaxDisSqrtToCam = 0;

    public static int MinSceneId = 0;

    public static void Reset()
    {
        MinDisSqrtToCam = float.MaxValue;

        MaxDisSqrtToCam = 0;

        MinSceneId = 0;
    }


    public void Execute()
    {
        var start = DateTime.Now;
        for (int i = 0; i < Count; i++)
        {
            float disToCams = float.MaxValue;
            float angleOfCams = 0;
            Vector3 sP = scenePos[i];
            Bounds sB = sceneBounds[i];

            for (int j=0;j< camPosList.Length;j++)
            {
                Vector3 cP = camPosList[j];
                float camAngle = 0;
                if (IsEnableAngleCheck)
                {
                    camAngle = GetCameraAngle(sB, cP, camForwardList[j]);
                }
                float dis = 0;
                
                if (IsEnableBoundsDis)
                {
                    dis = sB.SqrDistance(cP);
                }
                else
                {
                    dis = Vector3.Distance(sP, cP);
                }

                if (dis < disToCams)
                {
                    disToCams = dis;
                    angleOfCams = camAngle;
                }
            }
            disList[i] = disToCams;
            angleList[i] = angleOfCams;

            //scene.DisToCam = disToCams;
            //scene.AngleToCam = angleOfCams;
            if (disToCams > MaxDisSqrtToCam)
            {
                MaxDisSqrtToCam = disToCams;
            }
            if (disToCams < MinDisSqrtToCam)
            {
                MinDisSqrtToCam = disToCams;
                MinSceneId = i;
            }
        }

       //var TimeOfDis = (DateTime.Now - start).TotalMilliseconds;
        //Debug.Log($"ScenesDistanceJob Count:{Count} MinSceneId:{MinSceneId} TimeOfDis:{TimeOfDis:F2}ms MinDisSqrtToCam:{MinDisSqrtToCam} MaxDisSqrtToCam:{MaxDisSqrtToCam} ");
    }
}
