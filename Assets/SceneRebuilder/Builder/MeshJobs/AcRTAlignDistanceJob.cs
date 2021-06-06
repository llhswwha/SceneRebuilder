using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;
using static MeshJobs.MeshJobHelper;
using System.Linq;

namespace MeshJobs
{
public class AcRTDistanceJobResult
    {
        public float min = float.MaxValue;

        public int minId = -1;

        public bool IsZero = false;

        public RTResult rt;

        //public GameObject Instance;

        public override string ToString()
        {
            return string.Format("(Dis:{0},Id:{1},RT:{2})",min,minId,rt);
        }
    }

    public static class AcRTDistanceJobHelper
    {

        public const float zero = 1E-08f;

        public static int ID = 0;

        public static Dictionary<int, AcRTDistanceJobResult> results = new Dictionary<int, AcRTDistanceJobResult>();

        public static void PrintResults()
        {
            foreach (var key in results.Keys)
            {
                var r = results[key];
                Debug.Log(string.Format("PrintResults key:{0},value:{1}", key,r));
            }
        }

        public static void Reset()
        {
            ID = 0;
            results.Clear();
        }

        public static int InitResult()
        {
            AcRTDistanceJobResult result = new AcRTDistanceJobResult();
            int id = ID++;
            results.Add(id, result);
            return id;
        }

        public static void SetResult(int rId, int id, float dis, RTResult rt)
        {
            AcRTDistanceJobResult result = results[rId];
            if (dis < result.min)
            {
                result.min = dis;
                result.minId = id;
                result.rt = rt;

                if (dis <= zero)
                {
                    result.IsZero = true;
                }
            }

        }
    }

    public struct AcRTAlignDistanceJob : IJob
    {
        public int Id;

        public int RId;

        public ThreePoint tpFrom;

        public ThreePoint tpTo;

        public int vsFromId;

        //[ReadOnly] public NativeArray<Vector3> vsFrom;

        public int vsToId;

        //[ReadOnly] public NativeArray<Vector3> vsTo;

        public static double totalTime1 = 0;
        public static double totalTime2 = 0;
        public static double totalTime3 = 0;

        public void Execute()
        {
            //DateTime tmpT = DateTime.Now;
            var rt = AcRigidTransform.GetRTMatrixS(tpFrom, tpTo);//核心，静态函数的
            //totalTime1 += (DateTime.Now - tmpT).TotalMilliseconds;

            //tmpT = DateTime.Now;
            if (AcRTAlignJobHelper.vsDictWorld.ContainsKey(vsFromId) == false)
            {
                Debug.LogError("AcRTAlignJob.vsDict.ContainsKey(vsFromId) == false:"+ vsFromId);
                return;
            }
            var vsNew = rt.ApplyPoints(AcRTAlignJobHelper.vsDictWorld[vsFromId]);
            //totalTime2 += (DateTime.Now - tmpT).TotalMilliseconds;

            //tmpT = DateTime.Now;
            if (AcRTAlignJobHelper.vsDictWorld.ContainsKey(vsToId) == false)
            {
                Debug.LogError("AcRTAlignJob.vsDict.ContainsKey(vsToId) == false:" + vsToId);
                return;
            }
            var dis = DistanceUtil.GetDistance(AcRTAlignJobHelper.vsDictWorld[vsToId], vsNew, false);
            //var dis = 0;
            //totalTime3 += (DateTime.Now - tmpT).TotalMilliseconds;

            AcRTDistanceJobHelper.SetResult(RId, Id, dis, rt);
        }
    }

    public class AcRTAlignJobExResult
    {
        public static AcRTAlignDistanceJob[][] DisJobs;

        public static int disJobCount = 0;

        public static JobList<AcRTAlignDistanceJob> GetJobList(int jobSize)
        {
            disJobCount = 0;
            JobList<AcRTAlignDistanceJob> disJobs = new JobList<AcRTAlignDistanceJob>(jobSize);
            for (int i = 0; i < AcRTAlignJobExResult.DisJobs.Length; i++)
            {
                var jobs = AcRTAlignJobExResult.DisJobs[i];
                for (int j = 0; j < jobs.Length; j++)
                {
                    var job = jobs[j];
                    if (job.Id == -1) continue;
                    disJobCount++;
                    disJobs.Add(job);
                }
            }
            return disJobs;
        }

        public static void InitJobCount(int count)
        {
            DisJobs = new AcRTAlignDistanceJob[count][];
        }

        public static void SetJobs(int id, AcRTAlignDistanceJob[] jobs)
        {
            DisJobs[id] = jobs;
        }
    }
}