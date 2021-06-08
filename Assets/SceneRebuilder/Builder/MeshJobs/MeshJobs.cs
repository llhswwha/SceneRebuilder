using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;

namespace MeshJobs
{

    public static class MeshJobHelper
    {
        #region ThreePointJob
        public static JobList<ThreePointJob> CreateThreePointJobs(MeshFilter[] meshFilters, int size)
        {
            return TreePointJobHelper.CreateThreePointJobs(meshFilters, size);
        }

        public static MeshFilter[] NewThreePointJobs(GameObject[] objs, int size)
        {
            return TreePointJobHelper.NewThreePointJobs(objs, size);
        }

        public static void NewThreePointJobs(MeshFilter[] meshFilters, int size)
        {
            TreePointJobHelper.NewThreePointJobs(meshFilters, size);
        }
        #endregion

        public static GameObject CreateTempGo(ThreePointJobResult r, Transform t, string log, ThreePoint id)
        {
            GameObject tempCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //tempCenter.transform.SetParent(node2.transform.parent);
            tempCenter.transform.up = r.GetLongShortNormal(t, id);
            tempCenter.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            tempCenter.name = t.name + "_TempCenter_" + log;

            tempCenter.transform.position = r.GetCenterP(t);
            //tempCenter.transform.position=CenterGo.transform.position;
            t.SetParent(tempCenter.transform);

            return tempCenter;
        }

        #region MeshAlignJob
        public static MeshDistanceJobResult NewMeshAlignJob(GameObject go1, GameObject go2, bool showLog)
        {
            //go1不动，go2匹配go1。
            DateTime start = DateTime.Now;

            var mf1 = go1.GetComponent<MeshFilter>();
            var mf2 = go2.GetComponent<MeshFilter>();

            //1.判断顶点数据是否相同
            ThreePointJobResult r1 = ThreePointJobResultList.Instance.GetThreePointResult(mf1);
            ThreePointJobResult r2 = ThreePointJobResultList.Instance.GetThreePointResult(mf2);
            if (r1.vertexCount != r2.vertexCount)
            {
                return new MeshDistanceJobResult();
            }

            //2.Job1:对齐模型
            MeshAlignJobContainer container = new MeshAlignJobContainer(mf1, mf2, showLog);
            container.InitAlignInfo();
            // container.CreateJob().Complete();//4ms
            container.DoAlignJob();

            //3.Job2:比较距离，找出最小的变换
            container.CreateMeshDistanceJobs();
            container.DoDistanceJobs();

            //4.清理多余的模型，获取结果
            var result = container.GetResult();
            double t = (DateTime.Now - start).TotalMilliseconds;
            MeshAlignJobContainer.TotalTime += t;
            if (showLog) Debug.LogWarning($"NewMeshAlignJob Count:{container.count},Time:{t}ms");
            return result;
        }

        public static List<PrefabInfo> NewMeshAlignJobs(MeshFilter[] meshFilters, int size)
        {
            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            //3.创建预设，替换模型
            List<PrefabInfo> prefabInfoList = new List<PrefabInfo>();
            int progressCount = 0;
            bool isCancel = false;

            for (int j = 0; j < targetCount; j++)
            {
                //一次
                MeshFilter first = meshFilters[0];
                GameObject prefab = first.gameObject;
                progressCount++;//一个作为预设

                PrefabInfo prefabInfo = new PrefabInfo(first);
                prefabInfoList.Add(prefabInfo);

                List<MeshFilter> newTargets = new List<MeshFilter>();
                for (int i = 1; i < meshFilters.Length; i++)
                {
                    MeshFilter item = meshFilters[i];
                    GameObject model = item.gameObject;
                    var result = NewMeshAlignJob(model, prefab, false);//核心
                    if (result.IsZero)
                    {
                        GameObject.DestroyImmediate(model);//匹配成功，删除老的
                        prefabInfo.Add(result.Instance);
                        progressCount++;//一个作为实例
                    }
                    else
                    {
                        newTargets.Add(item);//新的目标
                    }

                    float progress1 = (float)progressCount / targetCount;
                    if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress1 * 100:F2}% of 100% ", progress1))
                    {
                        isCancel = true;//取消处理
                        break;
                    }
                }
                if (isCancel)
                {
                    break;
                }
                float progress = (float)progressCount / targetCount;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress * 100:F2}% of 100%", progress))
                {
                    break;
                }
                meshFilters = newTargets.ToArray();
                if (newTargets.Count == 0)
                {
                    break;//结束了
                }
            }

            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"NewMeshAlignJobs Count:{targetCount},Time:{(DateTime.Now - start).ToString()}");

            MeshAlignJobContainer.PrintTime();
            return prefabInfoList;
        }

        private static void SetParentZero(MeshFilter[] meshFilters)
        {
            int targetCount = meshFilters.Length;
            for (int j = 0; j < targetCount; j++)
            {
                MeshHelper.SetParentZero(meshFilters[j].gameObject);//要么设置为null，要么设置为zero，不然会影响到精度
            }
        }

        #endregion

        #region RTAlignJob //无用

        public static List<PrefabInfo> NewRTAlignJobs(MeshFilter[] meshFilters, int size)
        {
            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            //3.创建预设，替换模型
            List<PrefabInfo> prefabInfoList = new List<PrefabInfo>();
            int progressCount = 0;
            bool isCancel = false;

            for (int j = 0; j < targetCount; j++)
            {
                //一次
                MeshFilter first = meshFilters[0];
                GameObject prefab = first.gameObject;
                progressCount++;//一个作为预设

                PrefabInfo prefabInfo = new PrefabInfo(first);
                prefabInfoList.Add(prefabInfo);

                List<MeshFilter> newTargets = new List<MeshFilter>();
                for (int i = 1; i < meshFilters.Length; i++)
                {
                    MeshFilter item = meshFilters[i];
                    GameObject model = item.gameObject;
                    var result = NewMeshAlignJob(model, prefab, false);//核心
                    if (result.IsZero)
                    {
                        GameObject.DestroyImmediate(model);//匹配成功，删除老的
                        prefabInfo.Add(result.Instance);
                        progressCount++;//一个作为实例
                    }
                    else
                    {
                        newTargets.Add(item);//新的目标
                    }

                    float progress1 = (float)progressCount / targetCount;
                    if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress1 * 100:F2}% of 100% ", progress1))
                    {
                        isCancel = true;//取消处理
                        break;
                    }
                }
                if (isCancel)
                {
                    break;
                }
                float progress = (float)progressCount / targetCount;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress * 100:F2}% of 100%", progress))
                {
                    break;
                }
                meshFilters = newTargets.ToArray();
                if (newTargets.Count == 0)
                {
                    break;//结束了
                }
            }

            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"NewMeshAlignJobs Count:{targetCount},Time:{(DateTime.Now - start).ToString()}");

            MeshAlignJobContainer.PrintTime();
            return prefabInfoList;
        }

        public static JobList<RTAlignJob> CreateRTAlignJobs(MeshFilter mf1, MeshFilter mf2, int size)
        {
            DateTime start = DateTime.Now;
            JobList<RTAlignJob> jobList = new JobList<RTAlignJob>(size);
            Transform t1 = mf1.transform;
            Transform t2 = mf2.transform;

            ThreePointJobResult r1 = ThreePointJobResultList.Instance.GetThreePointResult(mf1);
            r1.localToWorldMatrix = t1.localToWorldMatrix;

            ThreePointJobResult r2 = ThreePointJobResultList.Instance.GetThreePointResult(mf2);
            r2.localToWorldMatrix = t2.localToWorldMatrix;

            var rts1 = r1.GetRTTransforms(t1.localToWorldMatrix);
            var rts2 = r2.GetRTTransforms(t2.localToWorldMatrix);

            Vector3[] vertices1 = mf1.sharedMesh.vertices;
            Vector3[] vertices1World = MeshHelper.GetWorldVertexes(vertices1, t1);
            NativeArray<Vector3> verticesArray1 = new NativeArray<Vector3>(vertices1World, Allocator.TempJob);

            Vector3[] vertices2 = mf2.sharedMesh.vertices;
            Vector3[] vertices2World = MeshHelper.GetWorldVertexes(vertices1, t2);
            NativeArray<Vector3> verticesArray2 = new NativeArray<Vector3>(vertices2World, Allocator.TempJob);

            int rId = MeshDistanceJobHelper.InitResult();
            int jobId = 0;
            for (int i = 0; i < rts1.Count; i++)
            {
                for (int j = 0; j < rts2.Count; j++)
                {
                    jobId++;
                    RTAlignJob job = new RTAlignJob()
                    {
                        Id = jobId,
                        RId = rId,
                        rt = RTTransform.FormTo(rts1[i], rts2[j]),
                        // Matrix1=go1.transform.localToWorldMatrix,
                        vertices1 = verticesArray1,
                        vertices2 = verticesArray2
                    };
                    jobList.Add(job);
                }
            }
            Debug.Log($"CreateRTAlignJobs Count:{jobList.Length},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
            return jobList;
        }

        public static void NewRTAlignJobs(MeshFilter mf1, MeshFilter mf2, int size)
        {
            DateTime start = DateTime.Now;
            JobList<RTAlignJob> jobList = CreateRTAlignJobs(mf1, mf2, size);
            jobList.CompleteAllPage();
            Debug.Log($"NewRTAlignJobs Count:{jobList.Length},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
        }

        // public static MeshDistanceJobResult NewMeshAlignJob(GameObject go1,GameObject go2,bool showLog)
        // {
        //     DateTime start=DateTime.Now;

        //     // MeshAlignJobContainer container=CreateMeshAlignJob(go1,go2,showLog);
        //     // if(showLog)Debug.Log($"NewMeshAlignJob32 Count:{container.count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     // return new MeshDistanceJobResult();

        //     //go1不动，go2匹配go1。
        //     ThreePointJobResult r1=ThreePointJobResultList.Instance.GetThreePoint(go1);
        //     ThreePointJobResult r2=ThreePointJobResultList.Instance.GetThreePoint(go2);
        //     if(r1.vertexCount!=r2.vertexCount){
        //         return new MeshDistanceJobResult();
        //     }

        //     // int count=ps1.Count*ps2.Count;

        //     List<ThreePoint> ps1 = r1.GetThreePoints(go1.transform.localToWorldMatrix);
        //     List<ThreePoint> ps2 = r2.GetThreePoints(go2.transform.localToWorldMatrix); 

        //     int count=ps1.Count*ps2.Count;
        //     if(showLog)Debug.Log($"NewMeshAlignJob01 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //1ms

        //     // MeshNode node1=MeshNode.CreateNew(go1);
        //     // node1.GetVertexCenterInfo(true,true,Vector3.zero);

        //     // MeshNode node2=MeshNode.CreateNew(go2);
        //     // node2.GetVertexCenterInfo(true,true,Vector3.zero);

        //     // var c2=node1.GetCenterP();
        //     // var c1=ps1[0].GetCenterP();
        //     // c1.PrintVector3("c1");
        //     // c2.PrintVector3("c2");

        //     // var n1=ps1[0].GetLongShortNormal();
        //     // var n2=node1.GetLongShortNormal();
        //     // n1.PrintVector3("n1");
        //     // n2.PrintVector3("n2");

        //     // return;

        //     if(showLog)Debug.Log($"count1:{ps1.Count},count2:{ps2.Count},countAll:{count},go1:{go1.GetInstanceID()},{r1}|go2:{go2.GetInstanceID()},r2:{r2},");

        //     MeshAlignJob alignJob=new MeshAlignJob();
        //     // job.node1=r1;
        //     // job.node2=r2;
        //     alignJob.Init(count);

        //     r1.localToWorldMatrix=go1.transform.localToWorldMatrix;

        //     Transform[] ts=new Transform[count];

        //     if(showLog)Debug.Log($"NewMeshAlignJob02 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //2ms

        //     int i=0;
        //     List<GameObject> copyList=new List<GameObject>();
        //     for(int l=0;l<ps1.Count;l++)
        //     {
        //         var id1=ps1[l];
        //         for(int k= 0; k<ps2.Count;k++)
        //         {
        //             var id2=ps2[k];
        //             alignJob.SetIds(i,id1,id2);

        //             // GameObject go1Copy=MeshHelper.CopyGO(go1);
        //             // GameObject go2Copy=MeshHelper.CopyGO(go2);
        //             // ts[i*2]=go1Copy.transform;
        //             // ts[i*2+1]=go2Copy.transform;

        //             GameObject go2Copy=MeshHelper.CopyGO(go2);
        //             string tag=$"|({id1.max},{id1.min})({id2.max},{id2.min})";
        //             go2Copy.name+=tag;

        //             copyList.Add(go2Copy);
        //             ts[i]=go2Copy.transform;

        //             // GameObject go2Temp=CreateTempGo(r2,go2Copy.transform,tag,id2);
        //             // ts[i]=go2Temp.transform;

        //             i++;
        //             //id2.ShowDebugDetail();
        //             //break;
        //         }
        //         //break;
        //     }
        //     TransformAccessArray tA=new TransformAccessArray(ts);

        //     if(showLog)Debug.Log($"NewMeshAlignJob11 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //5ms

        //     alignJob.Schedule(tA).Complete();//4ms
        //     if(showLog)Debug.Log($"NewMeshAlignJob12 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //9ms

        //     // JobHandle alignJobHandle=CreateMeshAlignJob(go1,go2,showLog);
        //     // var count=alignJobHandle.cou
        //     // alignJobHandle.Complete();//4ms
        //     // if(showLog)Debug.Log($"NewMeshAlignJob12 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");

        //     // //调试用，显示细节
        //     // foreach(var go in copyList){
        //     //     MeshNode node=MeshNode.CreateNew(go);
        //     //     node.GetVertexCenterInfo(true,true,Vector3.zero);
        //     // }

        //     //-------------------------比较距离，找出最小的
        //     MeshFilter meshFilter1=go1.GetComponent<MeshFilter>();
        //     Vector3[] vertices1=meshFilter1.sharedMesh.vertices;
        //     Vector3[] vertices1World=MeshHelper.GetWorldVertexes(vertices1,go1.transform);
        //     NativeArray<Vector3> verticesArray1=new NativeArray<Vector3>(vertices1World,Allocator.TempJob);

        //     MeshFilter meshFilter2=go2.GetComponent<MeshFilter>();
        //     Vector3[] vertices2=meshFilter2.sharedMesh.vertices;
        //     NativeArray<Vector3> verticesArray2=new NativeArray<Vector3>(vertices2,Allocator.TempJob);

        //     // MeshDistanceJob.minDis=float.MaxValue;
        //     // MeshDistanceJob.minDisId=0;

        //     int rId=MeshDistanceJobHelper.InitResult();

        //     JobList<MeshDistanceJob> jobList=new JobList<MeshDistanceJob>(10000);
        //     for (int i1 = 0; i1 < copyList.Count; i1++)
        //     {
        //         GameObject go = copyList[i1];
        //         MeshDistanceJob job1=new MeshDistanceJob{
        //             Id=i1,
        //             RId=rId,
        //             // Matrix1=go1.transform.localToWorldMatrix,
        //             Matrix2=go.transform.localToWorldMatrix,
        //             vertices1=verticesArray1,
        //             vertices2=verticesArray2
        //         };
        //         jobList.Add(job1);
        //     }

        //     if(showLog)Debug.Log($"NewMeshAlignJob21 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //     //13ms
        //     jobList.CompleteAllPage();//8ms
        //     var result=MeshDistanceJobHelper.results[rId];
        //     if(showLog)Debug.Log($"NewMeshAlignJob22 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms,Dis:{result.min},Id:{result.minId},Zero:{result.IsZero}");
        //     //21ms

        //     //清理多余的模型
        //     GameObject minGo=null;
        //     for (int i1 = 0; i1 < copyList.Count; i1++)
        //     {
        //         GameObject go = copyList[i1];
        //         if(i1!=result.minId)
        //         {
        //             GameObject.DestroyImmediate(go);
        //         }
        //         else
        //         {
        //             go.name=go1.name+"_New";
        //             minGo=go;
        //         }
        //     }
        //     if(result.IsZero){
        //         // GameObject.DestroyImmediate(go1);
        //         // GameObject.DestroyImmediate(go2);
        //         result.Instance=minGo;
        //     }
        //     else
        //     {
        //         if(result.min<0.02f)//判断是否相对很近
        //         {
        //             //if(long22<0.0001 && short22<0.0001 &&angle1<=0.09f&&angle2<=0.09f&&dis2<=0.02f)//几个条件，确保差异不大。这种情况下或许还有结合ICP算法
        //             ThreePoint node1=alignJob.ps1[result.minId];
        //             ThreePoint node2=alignJob.ps2[result.minId];

        //             var longLine1=node1.GetLongLine();
        //             var shortLine1=node1.GetShortLine();
        //             var angel1=Vector3.Angle(longLine1, shortLine1);

        //             var longLine2=node2.GetLongLine();
        //             var shortLine2=node2.GetShortLine();
        //             var angel2=Vector3.Angle(longLine2, shortLine2);

        //             var angle22 = Math.Abs(angel1 - angel2);
        //             var long22 = Math.Abs(longLine1.magnitude- longLine2.magnitude);
        //             var short22= Math.Abs(shortLine1.magnitude - shortLine2.magnitude);
        //             var rate22 = Math.Abs(longLine1.magnitude/shortLine1.magnitude - longLine2.magnitude/shortLine2.magnitude);

        //             var angle1 =Vector3.Angle(shortLine2, shortLine1);
        //             var angle2=Vector3.Angle(longLine2, longLine1);
        //             if(showLog)Debug.Log($"After ShortLine angle1:{angle1} | LongLine angle2:{angle2} | dis:{result.min}");
        //             if(showLog)Debug.Log($"node1-node2 angle:{angle22},long:{long22},short:{short22},rate:{rate22} |");
        //             if(showLog)Debug.LogError($"IsRelativeZero: {long22<0.0001}|{short22<0.0001}|{angle1<=0.09f}|{angle2<=0.09f}|{result.min<=0.02f}");

        //             //if(long22<0.0001 && short22<0.0001 &&angle1<=0.09f&&angle2<=0.09f)//几个条件，确保差异不大。这种情况下或许还有结合ICP算法
        //             if(long22<0.0001 && short22<0.0001)//长短轴相似，角度不管。就算角度不合适，重合就行
        //             {
        //                 result.IsZero=true;
        //             }
        //             else
        //             {
        //                 if(minGo!=null){
        //                     GameObject.DestroyImmediate(minGo);
        //                 }
        //             }
        //         }
        //         else{
        //             if(minGo!=null){
        //                 GameObject.DestroyImmediate(minGo);
        //             }
        //         }
        //     }

        //      if(showLog)Debug.Log($"NewMeshAlignJob32 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //      return result;
        // }

        #endregion
        #region AcRTAlign

        public static void DoAcRTAlignJob(MeshFilter mfFrom, MeshFilter mfTo, int id)
        {
            AcRTAlignJob job = AcRTAlignJobHelper.NewJob(mfFrom, mfTo, id);
            job.Schedule().Complete();//DoJob
            var result = AcRTAlignJobResult.GetResult(id);
            if (result != null)
            {

                //可以改成这里创建模型，并变换，原来的模型不动。
                result.ApplyMatrix(mfFrom.transform, mfTo.transform); //变换模型

                if (result.Distance < 0.001)
                {
                    Debug.LogError($"对齐成功 {mfFrom.name} -> {mfTo.name} 距离:{result.Distance}");
                }
                else
                {
                    Debug.LogError($"对齐失败2 {mfFrom.name} -> {mfTo.name} 距离:{result.Distance}");
                }

            }
            else
            {
                Debug.LogError($"对齐失败1 id:{id} {mfFrom.name} -> {mfTo.name}");
            }
            return;
        }

        private static MeshFilterListDict CreateMeshFilterListDict(MeshFilter[] meshFilters){
        DateTime start = DateTime.Now;
        var mfld = new MeshFilterListDict(meshFilters);
        Debug.Log($"CreateMeshFilterListDict meshFilters:{meshFilters.Length},Time:{(DateTime.Now - start).TotalMilliseconds:F1}ms");
        return mfld;
    }

        public static List<PrefabInfo> NewAcRTAlignJobsEx_OLD(MeshFilter[] meshFilters, int size)
        {
            Debug.Log("NewAcRTAlignJobsEx:"+meshFilters.Length);

            AcRtAlignJobArg.CleanArgs();
            AcRTAlignJobResult.CleanResults();

            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);


            //3.创建预设，替换模型
            List<PrefabInfo> prefabInfoList = new List<PrefabInfo>();

            int totalJobCount = 0;
            int loopCount = 0;
            string jobCountDetails = "";

            int mfCount = meshFilters.Length;
            var mfld = CreateMeshFilterListDict(meshFilters);
            string loopTimes="";

            for (int kk = 0; kk < mfCount; kk++)
            {
                DateTime loopStart = DateTime.Now;

                loopCount++;

                int count1 = mfld.GetMeshFilterCount();
                string countDetail=mfld.GetGroupCountDetails();
                var mfsList=mfld.GetMeshFiltersList();
                Debug.Log($"MeshFilterListDict List:{mfsList.Count},MeshFiter:{count1}");
                Debug.Log($"countDetail:{countDetail}");

                AcRTAlignJobResult.CleanResults();
                AcRtAlignJobArg.CleanArgs();
                AcRTAlignJobPrefab.Clear();

                int jobId = 0;
                JobList<AcRTAlignJob> jobList = new JobList<AcRTAlignJob>(size);
                List<int> jobIds = new List<int>();
                string ids = "";
                 for (int i1 = 0; i1 < mfsList.Count; i1++)
                {
                    MeshFilterList item = mfsList[i1];
                    var mfList = item.GetList();

                    MeshFilter mfFrom = mfList[0];
                    GameObject prefab = mfFrom.gameObject;
                    //progressCount++;//一个作为预设
                    PrefabInfo prefabInfo = new PrefabInfo(mfFrom);
                    prefabInfoList.Add(prefabInfo);

                    if(mfList.Count>1){
                        for (int i = 1; i < mfList.Count; i++)
                        {
                            MeshFilter mfTo = mfList[i];
                            if (mfTo == null) continue;
                            //int jobId = AcRTAlignJobResult.GetId();
                            var job = AcRTAlignJobHelper.NewJob(mfFrom, mfTo, jobId);
                            jobList.Add(job);
                            jobIds.Add(jobId);
                            AcRTAlignJobPrefab.AddItem(jobId, prefabInfo, item);
                            ids += jobId + ";";

                            totalJobCount++;
                            jobId++;
                        }
                    }
                    else{
                        item.Remove(mfFrom);
                    }
                }
                jobCountDetails += jobList.Length + ";";
                AcRTAlignJobResult.InitCount(jobId);
                jobList.CompleteAllPage();//计算

                for (int k = 0; k < jobIds.Count; k++)
                {
                    int id = jobIds[k];
                    AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(id);

                    var result = AcRTAlignJobResult.GetResult(id);
                    AcRTAlignJobPrefab pref = AcRTAlignJobPrefab.GetItem(id);
                    if (result != null)
                    {
                        //可以改成这里创建模型，并变换，原来的模型不动。

                        if (result.Distance < 0.01f)
                        {
                            pref.RemoveMeshFilter(arg.mfFrom);
                            pref.RemoveMeshFilter(arg.mfTo);

                            //Debug.Log($"对齐成功 {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                            GameObject newGo = MeshHelper.CopyGO(pref.PrefabInfo.Prefab);
                            newGo.name = arg.mfTo.name + "_New";
                            pref.AddInstance(newGo);
                            
                            result.ApplyMatrix(newGo.transform, arg.mfTo.transform); //变换模型

                            GameObject.DestroyImmediate(arg.mfTo.gameObject);
                        }
                        else
                        {
                            pref.RemoveMeshFilter(arg.mfFrom);
                            //newTargets.Add(arg.mfTo);

                            //Debug.LogWarning($"对齐失败(距离太大) {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                        }
                    }
                    else
                    {
                        pref.RemoveMeshFilter(arg.mfFrom);
                        //newTargets.Add(arg.mfTo);
                        Debug.LogError($"对齐失败(无结果数据) {arg.mfFrom.name} -> {arg.mfTo.name}");
                    }
                }

                mfld.RemoveEmpty();//删除
                if (mfld.Count == 0)
                {
                    break;
                }

                int count2 = mfld.GetMeshFilterCount();
                int progressCount=targetCount-count2;
                float progress1 = (float)progressCount / targetCount;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress1 * 100:F2}% of 100% ", progress1))
                {
                   //isCancel = true;//取消处理
                    break;
                }

                Debug.LogError($"完成一轮[{loopCount}]:{count1-count2}={count1}->{count2},PrefabCount:{prefabInfoList.Count}");
                int loopTime=(int)(DateTime.Now-loopStart).TotalMilliseconds;
                loopTimes+=loopTime+";";
            }
            

            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"NewMeshAlignJobs Target:{targetCount},Prefab:{prefabInfoList.Count},Job:{totalJobCount}({jobCountDetails}),Loop:{loopCount},Time:{(DateTime.Now - start).TotalSeconds:F2}s({loopTimes})");
            Debug.Log($"{prefabInfoList.Count}\t{totalJobCount}\t{loopCount}\t{(DateTime.Now - start).TotalSeconds:F2}s");
            MeshAlignJobContainer.PrintTime();
            prefabInfoList.Sort();
            return prefabInfoList;
        }

        public static Dictionary<Transform, Transform> GetParentDict(MeshFilter[] meshFilters)
        {
            Dictionary<Transform, Transform> parentDict = new Dictionary<Transform, Transform>();
            foreach (var meshFilter in meshFilters)
            {
                parentDict.Add(meshFilter.transform, meshFilter.transform.parent);
            }
            return parentDict;
        }

        public static void RestoreParent(Dictionary<Transform, Transform> parentDict)
        {
            List<Transform> parents = new List<Transform>();

            Dictionary<Transform, List<Transform>> parentChildren = new Dictionary<Transform, List<Transform>>();

            foreach (var child in parentDict.Keys)
            {
                if (child == null) continue;
                var parent = parentDict[child];
                if(!parentChildren.ContainsKey(parent))
                {
                    parentChildren.Add(parent, new List<Transform>());
                }
                var children = parentChildren[parent];
                children.Add(child);
            }

            foreach (var p in parentChildren.Keys)
            {
                var center = Vector3.zero;
                var list = parentChildren[p];
                for (int i=0;i< list.Count; i++)
                {
                    center += list[i].position;
                }
                center /= list.Count;

                p.position = center;

                //Debug.LogError("center:" + center);
            }
            
            //Debug.LogError("RestoreParent parentDict:" + parentDict.Count);
            foreach (var child in parentDict.Keys)
            {
                if (child == null) continue;
                var parent = parentDict[child];
                child.SetParent(parent);

                if (!parents.Contains(parent))
                {
                    parents.Add(parent);
                }
            }
        }

        public static List<PrefabInfo> NewAcRTAlignJobsEx(MeshFilter[] meshFilters, int size)
        {
            DateTime start = DateTime.Now;
            Debug.Log("NewAcRTAlignJobsEx:"+meshFilters.Length);

            var parentDict = GetParentDict(meshFilters);
            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);
            
            AcRTAlignJobContainer jobContainer=new AcRTAlignJobContainer(meshFilters, size);
            jobContainer.parentDict = parentDict;
            var preafbs=jobContainer.GetPrefabs();

            RestoreParent(parentDict);

            Debug.Log($"NewAcRTAlignJobsEx meshFilters:{meshFilters.Length},Time:{(DateTime.Now - start).TotalMilliseconds:F1}ms");
            return preafbs;
        }

        public static List<PrefabInfo> NewAcRTAlignJobsEx2(MeshFilter[] meshFilters, int size)
        {
            DateTime start = DateTime.Now;
            Debug.Log("NewAcRTAlignJobsEx:" + meshFilters.Length);

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            AcRTAlignJobContainer jobContainer = new AcRTAlignJobContainer(meshFilters, size);
            var preafbs = jobContainer.GetPrefabsEx();

            Debug.Log($"NewAcRTAlignJobsEx meshFilters:{meshFilters.Length},Time:{(DateTime.Now - start).TotalMilliseconds:F1}ms");
            return preafbs;
        }


        public static List<PrefabInfo> NewAcRTAlignJobs(MeshFilter[] meshFilters, int size)
        {
            AcRtAlignJobArg.CleanArgs();
            AcRTAlignJobResult.CleanResults();

            MeshAlignJobContainer.InitTime();
            MeshHelper.TotalCopyTime = 0;
            MeshHelper.TotalCopyCount = 0;

            DateTime start = DateTime.Now;
            int targetCount = meshFilters.Length;

            //1.设置父
            SetParentZero(meshFilters);

            //2.获取基本对应点信息
            NewThreePointJobs(meshFilters, size);

            //3.创建预设，替换模型
            List<PrefabInfo> prefabInfoList = new List<PrefabInfo>();

            //JobList<AcRTAlignJob> totalJobList = new JobList<AcRTAlignJob>(size);
            int totalJobCount = 0;
            int loopCount = 0;
            string jobCountDetails = "";

            for (int j = 0; j < targetCount; j++)
            {
                loopCount++;
                JobList<AcRTAlignJob> jobList = new JobList<AcRTAlignJob>(size);
                int jobId = 0;
                List<int> jobIds = new List<int>();
                string ids = "";

                AcRTAlignJobResult.CleanResults();
                AcRtAlignJobArg.CleanArgs();

                //一次
                MeshFilter mfFrom = meshFilters[0];
                GameObject prefab = mfFrom.gameObject;

                PrefabInfo prefabInfo = new PrefabInfo(mfFrom);
                prefabInfoList.Add(prefabInfo);

                List<MeshFilter> newTargets = new List<MeshFilter>();
                for (int i = 1; i < meshFilters.Length; i++)
                {
                    MeshFilter mfTo = meshFilters[i];
                    //int jobId = AcRTAlignJobResult.GetId();
                    var job = AcRTAlignJobHelper.NewJob(mfFrom, mfTo, jobId);
                    jobList.Add(job);
                    jobIds.Add(jobId);
                    ids += jobId + ";";

                    totalJobCount++;
                    jobId++;
                }
                jobCountDetails += jobList.Length + ";";

                //Debug.LogError("ids:" + ids);
                AcRTAlignJobResult.InitCount(jobId);
                jobList.CompleteAllPage();//计算


                for (int k = 0; k < jobIds.Count; k++)
                {
                    int id = jobIds[k];
                    AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(id);
                    var result = AcRTAlignJobResult.GetResult(id);
                    if (result!=null)
                    {
                        //可以改成这里创建模型，并变换，原来的模型不动。
                        
                        if (result.Distance < 0.01f)
                        {
                            //Debug.LogError($"对齐成功 {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                            GameObject newGo = MeshHelper.CopyGO(prefab);
                            newGo.name = arg.mfTo.name + "_New";
                            prefabInfo.Add(newGo);
                            
                            result.ApplyMatrix(newGo.transform, arg.mfTo.transform); //变换模型

                            GameObject.DestroyImmediate(arg.mfTo.gameObject);
                        }
                        else
                        {
                            newTargets.Add(arg.mfTo);
                            //Debug.LogWarning($"对齐失败(距离太大) {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                        }
                    }
                    else
                    {
                        newTargets.Add(arg.mfTo);
                        Debug.LogError($"对齐失败(无结果数据) {arg.mfFrom.name} -> {arg.mfTo.name}");
                    }
                }
                int count1 = meshFilters.Length;
                int count2 = newTargets.Count;//还剩多少模型没处理
                Debug.LogError($"完成一轮[{loopCount}]:{count1-count2}={count1}->{count2},PrefabCount:{prefabInfoList.Count}");
                meshFilters = newTargets.ToArray();//下一轮
                if (meshFilters.Length == 0)
                {
                    break;//完成了
                }

                int progressCount=targetCount-count2;
                float progress1 = (float)progressCount / targetCount;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"NewMeshAlignJobs:{progressCount}/{targetCount} {progress1 * 100:F2}% of 100% ", progress1))
                {
                   //isCancel = true;//取消处理
                    break;
                }
            }

            ProgressBarHelper.ClearProgressBar();

            Debug.Log($"NewMeshAlignJobs TargetCount:{targetCount},PrefabCount:{prefabInfoList.Count},JobCount:{totalJobCount}({jobCountDetails}),LoopCount:{loopCount},Time:{(DateTime.Now - start).TotalSeconds:F2}s");
            Debug.Log($"{prefabInfoList.Count}\t{totalJobCount}\t{loopCount}\t{(DateTime.Now - start).TotalSeconds:F2}s");
            MeshAlignJobContainer.PrintTime();
            prefabInfoList.Sort();
            return prefabInfoList;
        }
        #endregion
    }
}
