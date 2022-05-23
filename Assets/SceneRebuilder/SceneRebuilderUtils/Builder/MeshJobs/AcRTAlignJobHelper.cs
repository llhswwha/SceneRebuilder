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
public static class AcRTAlignJobHelper 
{
        public static void SetNewGoProperties(GameObject newGo, MeshPoints oldGo)
        {
            newGo.name = oldGo.name + "_New";

//#if UNITY_EDITOR
//            EditorHelper.CopyAllComponents(oldGo.gameObject, newGo, true, null);
//#endif

//            MeshRenderer mf1 = newGo.GetComponent<MeshRenderer>();
//            if (mf1 == null)
//            {
//                Debug.LogError("SetNewGoProperties mf1 == null");
//            }
//            MeshRenderer mf2 = oldGo.gameObject.GetComponent<MeshRenderer>();
//            if (mf2 == null)
//            {
//                Debug.LogError("SetNewGoProperties mf2 == null");
//            }
//            if (mf1 != null && mf2 != null)
//            {
//                mf1.sharedMaterials = mf2.sharedMaterials;
//            }

//            Debug.Log($"SetNewGoProperties mat1:{mf1.sharedMaterial} mat2:{mf2.sharedMaterial} mat3:{oldGo.GetMatId()}");
        }


        public static AcRTAlignJob NewJob(MeshPoints mfFrom, MeshPoints mfTo, int id)
        {
            AcRtAlignJobArg.SaveArg(id, mfFrom, mfTo);

            AcRTAlignJob job = new AcRTAlignJob();
            job.Id = id;

            var idFrom = mfFrom.GetInstanceID();
            var idTo = mfTo.GetInstanceID();
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
                var vsLocal=mfFrom.vertices;
                vsDictLocal.Add(idFrom, vsLocal);

                //var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfFrom.transform);

                var vsWorld = mfFrom.GetWorldVertexes();
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

                var vsLocal=mfTo.vertices;
                vsDictLocal.Add(idTo, vsLocal);

                //var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfTo.transform);
                var vsWorld = mfTo.GetWorldVertexes();
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

        public static AcRTAlignJobEx NewJobEx(MeshPoints mfFrom, MeshPoints mfTo, int id,int rId)
        {
            AcRTAlignJobEx job = new AcRTAlignJobEx();
            
            job.Id = id;
            job.Rid = rId;

            var idFrom = mfFrom.GetInstanceID();
            var idTo = mfTo.GetInstanceID();

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
                var vsLocal=mfFrom.vertices;
                vsDictLocal.Add(idFrom, vsLocal);

                //var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfFrom.transform);
                var vsWorld = mfFrom.GetWorldVertexes();
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

                var vsLocal=mfTo.vertices;
                vsDictLocal.Add(idTo, vsLocal);

                //var vsWorld=MeshHelper.GetWorldVertexes(vsLocal,mfTo.transform);
                var vsWorld = mfTo.GetWorldVertexes();
                vsDictWorld.Add(idTo, vsWorld);

                posDict.Add(idTo,mfTo.transform.position);

                scaleDict.Add(idTo,mfTo.transform.localScale);
            }
            return job;
        }

}
}