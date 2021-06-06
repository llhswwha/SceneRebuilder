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
public static class AcRTAlignJobHelper 
{
    
    public static AcRTAlignJob NewJob(MeshFilter mfFrom, MeshFilter mfTo, int id)
        {
            AcRtAlignJobArg.SaveArg(id, mfFrom, mfTo);

            AcRTAlignJob job = new AcRTAlignJob();
            job.Id = id;

            var idFrom = MeshHelper.GetInstanceID(mfFrom);
            var idTo = MeshHelper.GetInstanceID(mfTo);
            job.vsFromId = idFrom;
            job.vsToId = idTo;

            var tpsFrom = ThreePointJobResultList.Instance.GetThreePoints(mfFrom);
            if(!tpDict.ContainsKey(idFrom))
            {
                tpDict.Add(idFrom,tpsFrom);
            }

            var tpsTo = ThreePointJobResultList.Instance.GetThreePoints(mfTo);
            if(!tpDict.ContainsKey(idTo))
            {
                tpDict.Add(idTo,tpsTo);
            }
            // job.tpsFrom = new NativeArray<ThreePoint>(tpsFrom, Allocator.Persistent);
            // job.tpsTo = new NativeArray<ThreePoint>(tpsTo, Allocator.Persistent);
           
            if (vsDictWorld.ContainsKey(idFrom))
            {
                //var vsFrom = vsDict[idFrom];
                //var vsFrom2 = new NativeArray<Vector3>(vsFrom, Allocator.Persistent);
                //job.vsFrom = vsFrom2;
            }
            else
            {
                var vsLocal=mfFrom.sharedMesh.vertices;
                vsDictLocal.Add(idFrom, vsLocal);

                var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfFrom.transform);
                vsDictWorld.Add(idFrom, vsWorld);

                posDict.Add(idFrom,mfFrom.transform.position);

                //var vsFrom = MeshHelper.GetWorldVertexes(mfFrom);

                //var vsFrom2 = new NativeArray<Vector3>(vsFrom, Allocator.Persistent);
                //job.vsFrom = vsFrom2;

                scaleDict.Add(idFrom,mfFrom.transform.localScale);
            }

            
            
            if (vsDictWorld.ContainsKey(idTo))
            {
                //var vsTo = vsDict[idTo];
                //var vsTo2 = new NativeArray<Vector3>(vsTo, Allocator.Persistent);
                //job.vsTo = vsTo2;
            }
            else
            {
                // var vsTo = MeshHelper.GetWorldVertexes(mfTo);
                // //var vsTo2 = new NativeArray<Vector3>(vsTo, Allocator.Persistent);
                // //job.vsTo = vsTo2;
                // vsDictWorld.Add(idTo, vsTo);

                var vsLocal=mfTo.sharedMesh.vertices;
                vsDictLocal.Add(idTo, vsLocal);

                var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfTo.transform);
                vsDictWorld.Add(idTo, vsWorld);

                posDict.Add(idTo,mfTo.transform.position);

                scaleDict.Add(idTo,mfTo.transform.localScale);
            }
            return job;
        }

        public static Dictionary<int, Vector3> posDict = new Dictionary<int, Vector3>();

        public static Dictionary<int, Vector3> scaleDict = new Dictionary<int, Vector3>();

        public static Dictionary<int, Vector3[]> vsDictWorld = new Dictionary<int, Vector3[]>();

        public static Dictionary<int, Vector3[]> vsDictLocal = new Dictionary<int, Vector3[]>();

        public static Dictionary<int, ThreePoint[]> tpDict = new Dictionary<int, ThreePoint[]>();

        public static void RemoveDict(int id){
            vsDictWorld.Remove(id);
            vsDictLocal.Remove(id);
            tpDict.Remove(id);
            posDict.Remove(id);
            scaleDict.Remove(id);
        }

        public static void Clear()
        {
            vsDictWorld.Clear();
            vsDictLocal.Clear();
            tpDict.Clear();
            posDict.Clear();
            scaleDict.Clear();
        }

        public static AcRTAlignJobEx NewJobEx(MeshFilter mfFrom, MeshFilter mfTo, int id,int rId)
        {
            AcRTAlignJobEx job = new AcRTAlignJobEx();
            
            job.Id = id;
            job.Rid = rId;

            var idFrom = MeshHelper.GetInstanceID(mfFrom);
            var idTo = MeshHelper.GetInstanceID(mfTo);

            job.vsFromId = idFrom;
            job.vsToId = idTo;

            var tpsFrom = ThreePointJobResultList.Instance.GetThreePoints(mfFrom);
            if(!tpDict.ContainsKey(idFrom))
            {
                tpDict.Add(idFrom,tpsFrom);
            }

            var tpsTo = ThreePointJobResultList.Instance.GetThreePoints(mfTo);
            if(!tpDict.ContainsKey(idTo))
            {
                tpDict.Add(idTo,tpsTo);
            }
            // job.tpsFrom = new NativeArray<ThreePoint>(tpsFrom, Allocator.Persistent);
            // job.tpsTo = new NativeArray<ThreePoint>(tpsTo, Allocator.Persistent);
           
            if (vsDictWorld.ContainsKey(idFrom))
            {
                //var vsFrom = vsDict[idFrom];
                //var vsFrom2 = new NativeArray<Vector3>(vsFrom, Allocator.Persistent);
                //job.vsFrom = vsFrom2;
            }
            else
            {
                var vsLocal=mfFrom.sharedMesh.vertices;
                vsDictLocal.Add(idFrom, vsLocal);

                var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfFrom.transform);
                vsDictWorld.Add(idFrom, vsWorld);

                posDict.Add(idFrom,mfFrom.transform.position);

                //var vsFrom = MeshHelper.GetWorldVertexes(mfFrom);

                //var vsFrom2 = new NativeArray<Vector3>(vsFrom, Allocator.Persistent);
                //job.vsFrom = vsFrom2;

                scaleDict.Add(idFrom,mfFrom.transform.localScale);
            }

            
            
            if (vsDictWorld.ContainsKey(idTo))
            {
                //var vsTo = vsDict[idTo];
                //var vsTo2 = new NativeArray<Vector3>(vsTo, Allocator.Persistent);
                //job.vsTo = vsTo2;
            }
            else
            {
                // var vsTo = MeshHelper.GetWorldVertexes(mfTo);
                // //var vsTo2 = new NativeArray<Vector3>(vsTo, Allocator.Persistent);
                // //job.vsTo = vsTo2;
                // vsDictWorld.Add(idTo, vsTo);

                var vsLocal=mfTo.sharedMesh.vertices;
                vsDictLocal.Add(idTo, vsLocal);

                var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfTo.transform);
                vsDictWorld.Add(idTo, vsWorld);

                posDict.Add(idTo,mfTo.transform.position);

                scaleDict.Add(idTo,mfTo.transform.localScale);
            }
            return job;
        }

}
}