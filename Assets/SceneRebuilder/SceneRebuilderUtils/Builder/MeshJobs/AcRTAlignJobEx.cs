using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;
//using static MeshJobs.MeshJobHelper;
using System.Linq;

namespace MeshJobs
{
public struct AcRTAlignJobEx : IJob
    {
        public int Id;
        public int Rid;
        // [ReadOnly] public NativeArray<ThreePoint> tpsFrom;

        // [ReadOnly] public NativeArray<ThreePoint> tpsTo;

        public int vsFromId;

        //[ReadOnly] public NativeArray<Vector3> vsFrom;

        public int vsToId;

        //[ReadOnly] public NativeArray<Vector3> vsTo;

        //public NativeList<AcRTAlignJobCore> CoreJobs;

        public static double totalTime1 = 0;
        public static double totalTime2 = 0;
        public static double totalTime3 = 0;

        public static int totalFoundCount = 0;
        public static int totalNoFoundCount = 0;

        public static double totalFoundTime = 0;
        public static double totalNoFoundTime = 0;

        

        public static void ResetTotalData()
        {
            totalTime1 = 0;
            totalTime2 = 0;
            totalTime3 = 0;

            totalFoundCount = 0;
            totalNoFoundCount = 0;

            totalFoundTime = 0;
            totalNoFoundTime = 0;
        }

        public void Execute()
        {
            //Debug.Log(string.Format("AcRTAlignJobEx[{0}]",Id));
            //DateTime start = DateTime.Now;

            int count = 0;

            float minDis = float.MaxValue;
            RTResult minRT = new RTResult();
            bool isFoundZero = false;
            List<AcRTAlignDistanceJob> jobs = new List<AcRTAlignDistanceJob>();
            
             var tpsFrom=AcRTAlignJobHelper.tpDict[vsFromId];
            var tpsTo=AcRTAlignJobHelper.tpDict[vsToId];

            for (int l = 0; l < tpsFrom.Length; l++)
            {
                var tpFrom = tpsFrom[l];
                for (int k = 0; k < tpsTo.Length; k++)
                {
                    var tpTo = tpsTo[k];

                    var angle1 = tpFrom.GetLongShortAngle();
                    var angle2 = tpTo.GetLongShortAngle();
                    var angle22 = math.abs(angle1 - angle2);

                    if (angle22 > AcRTAlignJob.MaxAngle)
                    {
                        //AcRTAlignDistanceJob disJob = new AcRTAlignDistanceJob();
                        //disJob.Id = -1;//排除用
                        //jobs[count] = disJob;
                        continue;//下一个了，角度都不对其
                    }
                    else
                    {
                        AcRTAlignDistanceJob disJob = new AcRTAlignDistanceJob();
                        disJob.Id = this.Id;
                        disJob.RId = this.Rid;
                        disJob.tpTo = tpTo;
                        disJob.tpFrom = tpFrom;
                        disJob.vsToId = this.vsToId;
                        disJob.vsFromId = this.vsFromId;
                        //disJob.vsTo = this.vsTo;
                        jobs.Add(disJob);
                    }

                    

                    count++;
                    //var matrix=GetRTMatrix(tpFrom,tpTo);//核心,基于脚本的
                }
            }

            AcRTAlignJobExResult.SetJobs(this.Id, jobs.ToArray());
        }
    }
}