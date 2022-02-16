using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;
using MeshJobs;
using System.Text;

public class AcRTAlignJobContainer 
{
    public static AcRTAlignJobContainer Last;

    MeshPoints[] meshFilters;
    public int targetCount =0;
    int jobSize=0;
    int mfCount;
    public PrefabInfoList prefabInfoList = new PrefabInfoList();
    public int totalJobCount = 0;
    int loopCount = 0;
    string jobCountDetails = "";
    string loopTimes="";
    MeshFilterListDict mfld;

    public Dictionary<Transform, Transform> parentDict = new Dictionary<Transform, Transform>();

    public AcRTAlignJobContainer(MeshPoints[] meshFilters,int size, int vertexCountOffset)
    {
        Last = this;
        //Debug.Log($"AcRTAlignJobContainer MeshPoints:{meshFilters.Length} MaxVertexCount:{AcRTAlignJobContainer.MaxVertexCount}");
        if(AcRTAlignJobContainer.MaxVertexCount>0)
        {
            List<MeshPoints> mfs=new List<MeshPoints>();
            foreach(var mf in meshFilters)
            {
                if(mf.vertexCount>AcRTAlignJobContainer.MaxVertexCount)//排除点数特别多的，不然会卡住
                {
                    Debug.LogError($"AcRTAlignJobContainer mf.vertexCount>MaxVertexCount mf:{mf.name} vertexCount:{mf.vertexCount}");
                    continue;
                }
                else{
                    mfs.Add(mf);
                }
            }

            meshFilters=mfs.ToArray();
        }

        this.meshFilters=meshFilters;
        this.jobSize=size;
        this.mfCount = meshFilters.Length;
        this.targetCount = meshFilters.Length;
        this.vertexCountOffset = vertexCountOffset;

        ResetStaticInfo();
        
    }

    int vertexCountOffset;

    public void ResetStaticInfo()
    {
        AcRTAlignJobHelper.Clear();//不能放到ResetLoop中

        //重置统计数据
        this.totalTime1 = 0;
        this.totalTime2 = 0;
        this.totalTime3 = 0;
        this.totalTime4 = 0;
        this.totalTime5 = 0;
        AcRigidTransform.totalTime1 = 0;
        AcRigidTransform.totalTime2 = 0;

        MeshHelper.TotalCopyTime = 0;
        MeshHelper.TotalCopyCount = 0;

        AcRTAlignJob.ResetTotalData();
        MeshAlignJobContainer.InitTime();

        mfld = CreateMeshFilterListDict(meshFilters, vertexCountOffset);
    }

    public void ResetLoopInfo()
    {
        
    }

    int jobId = 0;
    List<int> jobIds = new List<int>();

    private int AlignJobCount;

    public PrefabInfoList newPrefabInfoList=new PrefabInfoList();//这一轮新增加的预设
    public JobList<AcRTAlignJob> GetAlignJobs()
    {
        jobId = 0;
        jobIds = new List<int>();
        JobList<AcRTAlignJob> jobList = new JobList<AcRTAlignJob>(jobSize);
        string ids = "";
        var mfsList=mfld.GetMeshFiltersList();
        //Debug.LogError("GetAlignJobs mfsList:"+mfsList.Count);
        newPrefabInfoList.Clear();
        for (int i1 = 0; i1 < mfsList.Count; i1++)
        {
            MeshFilterList item = mfsList[i1];
            var mfList = item.GetList();
            //Debug.LogError("GetAlignJobs mfList:"+mfList.Count);
            var mfFrom = mfList[0];
            if (mfFrom == null) continue;

            
            // if(item.vertexCount>MaxVertexCount)//排除点数特别多的，不然会卡住
            // {
            //     mfsList.RemoveAt(i1);
            //     i1--;
            //     continue;
            // }

            //GameObject prefab = mfFrom.gameObject;
            //progressCount++;//一个作为预设
            PrefabInfo prefabInfo = new PrefabInfo(mfFrom);
            prefabInfoList.Add(prefabInfo);
            newPrefabInfoList.Add(prefabInfo);

            if(mfList.Count>1){
                for (int i = 1; i < mfList.Count; i++)
                {
                    var mfTo = mfList[i];
                    //Debug.LogError("GetAlignJobs mfTo:"+mfTo);
                    if (mfTo == null) continue;

                    if(mfTo.IsSameMesh(mfFrom)){//已经处理过的相同的模型
                        prefabInfo.AddInstance(mfTo.gameObject);
                        item.Remove(mfTo);
                        //i--;
                        continue;
                    }
                    if (AcRTAlignJobContainer.CompareSizeMinValue>0 && MeshHelper.CompareSize(mfFrom, mfTo) > AcRTAlignJobContainer.CompareSizeMinValue) //两者尺寸相差比较大不用比较
                    {
                        continue;
                    }

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
        AlignJobCount = jobList.Length;
        AcRTAlignJob.AlignJobCount = AlignJobCount;

        jobCountDetails += jobList.Length + ";";
        AcRTAlignJobResult.InitCount(jobId);

        //Debug.LogError($"GetAlignJobs AlignJobCount:{AlignJobCount},prefabInfoList:{prefabInfoList.Count}");
        return jobList;
    }

    public JobList<AcRTAlignJobEx> GetJobsEx()
    {
        jobId = 0;
        jobIds = new List<int>();
        JobList<AcRTAlignJobEx> jobList = new JobList<AcRTAlignJobEx>(jobSize);
        string ids = "";
        var mfsList=mfld.GetMeshFiltersList();
        newPrefabInfoList.Clear();
        for (int i1 = 0; i1 < mfsList.Count; i1++)
        {
            MeshFilterList item = mfsList[i1];
            var mfList = item.GetList();

            var mfFrom = mfList[0];
            GameObject prefab = mfFrom.gameObject;
            //progressCount++;//一个作为预设
            PrefabInfo prefabInfo = new PrefabInfo(mfFrom);
            prefabInfoList.Add(prefabInfo);
             newPrefabInfoList.Add(prefabInfo);

            if (mfList.Count > 1)
            {
                for (int i = 1; i < mfList.Count; i++)
                {
                    var mfTo = mfList[i];
                    if (mfTo == null) continue;
                    if(mfTo.IsSameMesh(mfFrom)){
                        prefabInfo.AddInstance(mfTo.gameObject);
                        item.Remove(mfTo);
                        continue;
                    }
                    //int jobId = AcRTAlignJobResult.GetId();

                    int rId= AcRTDistanceJobHelper.InitResult();

                    AcRTAlignJobPrefab.AddItem(rId, prefabInfo, item);
                    AcRtAlignJobArg.SaveArg(rId, mfFrom, mfTo);

                    var job = AcRTAlignJobHelper.NewJobEx(mfFrom, mfTo, jobId,rId);
                    jobList.Add(job);
                    jobIds.Add(jobId);
                    ids += jobId + ";";

                    totalJobCount++;
                    jobId++;
                }
            }
            else
            {
                item.Remove(mfFrom);
            }
        }
        jobCountDetails += jobList.Length + ";";
        AcRTAlignJobExResult.InitJobCount(jobId);
        AcRTAlignJobResult.InitCount(jobId);
        return jobList;
    }

    public int resultId;

    public JobList<AcRTAlignDistanceJob> GetDistanceJobs()
    {
        jobId = 0;
        resultId = 0;
        jobIds = new List<int>();
        JobList<AcRTAlignDistanceJob> jobList = new JobList<AcRTAlignDistanceJob>(jobSize);
        string ids = "";
        var mfsList=mfld.GetMeshFiltersList();
        newPrefabInfoList.Clear();
        for (int i1 = 0; i1 < mfsList.Count; i1++)//根据verticeCount分组
        {
            MeshFilterList item = mfsList[i1];
            var mfList = item.GetList();

            var mfFrom = mfList[0];
            GameObject prefab = mfFrom.gameObject;
            //progressCount++;//一个作为预设
            PrefabInfo prefabInfo = new PrefabInfo(mfFrom);
            prefabInfoList.Add(prefabInfo);
            newPrefabInfoList.Add(prefabInfo);

            if (mfList.Count > 1)
            {
                for (int i = 1; i < mfList.Count; i++)
                {
                    var mfTo = mfList[i];
                    if (mfTo == null) continue;
                    if(mfTo.IsSameMesh(mfFrom)){
                        prefabInfo.AddInstance(mfTo.gameObject);
                        item.Remove(mfTo);
                        continue;
                    }

                    var tpsFrom = ThreePointJobResultList.Instance.GetThreePoints(mfFrom);
                    var tpsTo = ThreePointJobResultList.Instance.GetThreePoints(mfTo);
                    //job.tpsFrom = new NativeArray<ThreePoint>(tpsFrom, Allocator.TempJob);
                    //job.tpsTo = new NativeArray<ThreePoint>(tpsTo, Allocator.TempJob);
                    var vsFrom = MeshHelper.GetWorldVertexes(mfFrom);
                    var vsTo = MeshHelper.GetWorldVertexes(mfTo);
                    int rId = AcRTDistanceJobHelper.InitResult();

                    AcRTAlignJobPrefab.AddItem(rId, prefabInfo, item);
                    AcRtAlignJobArg.SaveArg(rId, mfFrom, mfTo);

                    for (int l = 0; l < tpsFrom.Length; l++)
                    {
                        var tpFrom = tpsFrom[l];
                        for (int k = 0; k < tpsTo.Length; k++)
                        {
                            var tpTo = tpsTo[k]; 

                            AcRTAlignDistanceJob job = new AcRTAlignDistanceJob();
                            job.RId = rId;
                            job.Id = jobId;
                            job.tpFrom = tpFrom;
                            job.tpTo = tpTo;
                            //job.vsFrom = new NativeArray<Vector3>(vsFrom, Allocator.TempJob);
                            //job.vsTo = new NativeArray<Vector3>(vsTo, Allocator.TempJob);
                            jobList.Add(job);

                            jobId++;
                        }
                    }

                    //ids += jobId + ";";
                    totalJobCount++;
                    
                    resultId++;
                }
            }
            else
            {
                item.Remove(mfFrom);
            }
        }
        jobCountDetails += jobList.Length + ";";
        AcRTAlignJobExResult.InitJobCount(jobId);
        AcRTAlignJobResult.InitCount(jobId);
        return jobList;
    }

    int loopStartMeshFilterCount;

    private string loopInitLog = "";

    private void InitLoopData()
    {
        loopStartMeshFilterCount = mfld.GetMeshFilterCount();
        string countDetail=mfld.GetGroupCountDetails();
        int maxJobCount=mfld.GetMaxJobCount();
        loopInitLog=$"InitLoopData GroupList:{mfld.GetMeshFiltersList().Count} Mesh:{loopStartMeshFilterCount} maxJob:{maxJobCount} countDetail:{countDetail}";

        AcRTAlignJobResult.CleanResults();
        AcRtAlignJobArg.CleanArgs();
        AcRTAlignJobPrefab.Clear();
        AcRTDistanceJobHelper.Reset();
        AcRTAlignJobExResult.disJobCount = 0;
        AcRTAlignJob.ResetLoopData();

    }

    //public void DoDistanceJobResult()
    //{
    //    for (int rid = 0; rid < resultId; rid++)
    //    {
    //        var disResult = AcRTDistanceJobHelper.results[rid];
    //        var jobId = disResult.minId;
    //        //int id = jobIds[k];
    //        AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(rid);

    //        AcRTAlignJobPrefab pref = AcRTAlignJobPrefab.GetItem(rid);

    //            if (disResult.min < 0.01f)
    //            {
    //                pref.RemoveMeshFilter(arg.mfFrom);
    //                pref.RemoveMeshFilter(arg.mfTo);

    //                //Debug.Log($"对齐成功 {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
    //                GameObject newGo = MeshHelper.CopyGO(pref.PrefabInfo.Prefab);
    //                newGo.name = arg.mfTo.name + "_New";
    //                pref.AddInstance(newGo);
                    
    //                disResult.rt.ApplyMatrix(newGo.transform, arg.mfTo.transform); //变换模型

    //                GameObject.DestroyImmediate(arg.mfTo.gameObject);
    //            }
    //            else
    //            {
    //                pref.RemoveMeshFilter(arg.mfFrom);
    //                //newTargets.Add(arg.mfTo);

    //                //Debug.LogWarning($"对齐失败(距离太大) {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
    //            }
    //    }
    //    mfld.RemoveEmpty();//删除
    //}

    public void DoDistanceJobResultEx()
    {
        //AcRTDistanceJobHelper.PrintResults();
        foreach (var rid in AcRTDistanceJobHelper.results.Keys)
        {
            var disResult = AcRTDistanceJobHelper.results[rid];
            var jobId = disResult.minId;
            //int id = jobIds[k];
            AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(rid);

            AcRTAlignJobPrefab pref = AcRTAlignJobPrefab.GetItem(rid);

            if (disResult.min < 0.01f)
            {
                pref.RemoveMeshFilter(arg.mfFrom);
                pref.RemoveMeshFilter(arg.mfTo);

                //Debug.Log($"对齐成功 {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
                GameObject newGo = MeshHelper.CopyGO(pref.PrefabInfo.Prefab);
                //newGo.name = arg.mfTo.name + "_New2";
                AcRTAlignJobHelper.SetNewGoProperties(newGo, arg.mfTo);
                pref.AddInstance(newGo);
                AcRTAlignJobHelper.RemoveDict(MeshHelper.GetInstanceID(arg.mfTo));//模型都删除了，把缓存的顶点数据也删除
                
                disResult.rt.ApplyMatrix(newGo.transform, arg.mfTo.transform); //变换模型

                arg.ReplaceToObject(newGo);
            }
            else
            {
                pref.RemoveMeshFilter(arg.mfFrom);
                //newTargets.Add(arg.mfTo);

                //Debug.LogWarning($"对齐失败(距离太大) {arg.mfFrom.name} -> {arg.mfTo.name} 距离:{result.Distance}");
            }
        }
        //return;
        for (int rid = 0; rid < resultId; rid++)
        {
            
        }
         RemoveNewPrefabMeshFilter();
        mfld.RemoveEmpty();//删除
    }

   

    public static bool IsCheckResult=false;

    public static bool IsSetParent=true;

    public static int MaxVertexCount=2400;

    public static float CompareSizeMinValue = 1.5f;

    int errorCount = 0;

    List<AcRtAlignJobArg> allArg = new List<AcRtAlignJobArg>();

    public StringBuilder DoAlignJobResult()
    {
        StringBuilder sb = new StringBuilder();
        for (int k = 0; k < jobIds.Count; k++)
        {
            int id = jobIds[k];
            AcRtAlignJobArg arg = AcRtAlignJobArg.LoadArg(id);
            allArg.Add(arg);

            var result = AcRTAlignJobResult.GetResult(id);
            AcRTAlignJobPrefab pref = AcRTAlignJobPrefab.GetItem(id);

            if (result != null)
            {
                //可以改成这里创建模型，并变换，原来的模型不动。

                if (result.Distance < DistanceSetting.zeroM)
                {
                    pref.RemoveMeshFilter(arg.mfFrom);
                    pref.RemoveMeshFilter(arg.mfTo);

                    GameObject newGo = MeshHelper.CopyGO(pref.PrefabInfo.Prefab);

                    if(parentDict.ContainsKey(arg.mfTo.transform))
                    {
                        Transform oldTransformParent = parentDict[arg.mfTo.transform];
                        parentDict.Add(newGo.transform, oldTransformParent);
                    }

                    //newGo.name = arg.mfTo.name + "_New";
                    AcRTAlignJobHelper.SetNewGoProperties(newGo, arg.mfTo);
                    pref.AddInstance(newGo);
                    AcRTAlignJobHelper.RemoveDict(MeshHelper.GetInstanceID(arg.mfTo));
                    
                    result.ApplyMatrix(newGo.transform, arg.mfTo.transform); //变换模型

                    if(IsCheckResult){
                       var disNew=MeshHelper.GetVertexDistanceEx(newGo.transform,arg.mfTo.transform,"测试结果",false);
                        if(disNew>DistanceSetting.zeroM)
                        {
                            errorCount++;
                            RTResult rT = result as RTResult;

                            arg.Log = $"对齐成功 有错误[{errorCount}] zero:{DistanceSetting.zeroM:F5},disNew:{disNew}";
                            arg.AlignResult = false;

                            if (rT != null)
                            {
                                sb.AppendLine($"{arg},Mode:{rT.Mode} " + $" Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                                //Debug.LogError($"Mode:{rT.Mode},Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                            }
                            else
                            {
                                sb.AppendLine($"{arg} rT==null" );
                            }

                            newGo.name += "_Error";
                            newGo.SetActive(false);

                            //GameObject.DestroyImmediate(newGo);
                        }
                        else
                        {
                            //GameObject.DestroyImmediate(arg.mfTo.gameObject);//测试用，留下有问题的
                            //GameObject.DestroyImmediate(newGo);//测试用，留下有问题的
                            arg.AlignResult = true;
                            RTResult rT = result as RTResult;
                            if (rT != null)
                            {
                                arg.Log = $"对齐成功11 zero:{DistanceSetting.zeroM:F5},disNew:{disNew},Mode:{rT.Mode}";
                                //Debug.Log($"{arg} " + $" Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                            }
                            else
                            {
                                arg.Log = $"对齐成功12 zero:{DistanceSetting.zeroM:F5},disNew:{disNew}";
                                sb.AppendLine($"{arg} rT==null");
                            }
                            
                            arg.ReplaceToObject(newGo);
                        }
                    }
                    else
                    {
                        RTResult rT = result as RTResult;
                        arg.AlignResult = true;
                        if (rT != null)
                        {
                            arg.Log = $"对齐成功01 zero:{DistanceSetting.zeroM:F5},dis:{rT.Distance},Mode:{rT.Mode}";
                            sb.AppendLine($"{arg} " + $" Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                            //Debug.LogError($"Mode:{rT.Mode},Trans:{rT.Translation.Vector3ToString()},Matrix:\n{rT.TransformationMatrix}");
                        }
                        else
                        {
                            arg.Log = $"对齐成功02 zero:{DistanceSetting.zeroM:F5},dis:{rT.Distance}";
                            sb.AppendLine($"{arg} rT==null");
                        }
                        
                        arg.ReplaceToObject(newGo);
                    }
                    
                }
                else if (result.Distance < DistanceSetting.ICPMinDis)
                {
                    pref.RemoveMeshFilter(arg.mfFrom);
                    //newTargets.Add(arg.mfTo);
                    arg.Log = $"对齐失败(距离太大1) 距离:{result.Distance:F4}";
                    arg.AlignResult = false;
                    Debug.LogWarning($"{arg}");
                    if (IsSetParent){
                        arg.mfTo.transform.SetParent(arg.mfFrom.transform);//关联相似的模型用于测试，测试好了要关闭。
                    }
                }
                else 
                {
                    //GameObject.DestroyImmediate(arg.mfTo.gameObject);//测试用，留下有问题的

                    pref.RemoveMeshFilter(arg.mfFrom);
                    //newTargets.Add(arg.mfTo);
                    arg.Log = $"对齐失败(距离太大2) 距离:{result.Distance:F4}";
                    arg.AlignResult = false;
                    Debug.LogWarning($"{arg}");
                }
            }
            else
            {
                pref.RemoveMeshFilter(arg.mfFrom);
                //newTargets.Add(arg.mfTo);
                arg.Log = $"对齐失败(无结果数据 或者 角度不对其) id={id}";
                arg.AlignResult = false;
                //Debug.LogWarning($"{arg}");
            }
        }
        
        RemoveNewPrefabMeshFilter();
        mfld.RemoveEmpty();//删除

        string log = sb.ToString();
        if(!string.IsNullOrEmpty(log))
        {
            Debug.Log($"DoAlignJobResult \n{sb}");
        }

        return sb;
    }

    private void RemoveNewPrefabMeshFilter()
    {
        for(int k=0;k<newPrefabInfoList.Count;k++)
        {
            var prefabInfo=newPrefabInfoList[k];
            mfld.RemoveMeshFilter(prefabInfo.MeshFilter);
        }
    }

    private int lastPrafabCount = 0;

    private int lastProgressCount = 0;

    public static string CurrentLogTag = "";

    private bool ShowProgressAndLog(bool isLoopEnd, int jobCount, string tag)
    {
        bool r = false;
        if (mfld.Count == 0)
        {
            r = true;//结束
        }
        int mfCount = mfld.GetMeshFilterCount();
        int progressCount = targetCount - mfCount;

        if (isLoopEnd)
        {
            if (lastProgressCount == progressCount)
            {
                Debug.LogError("出错退出");
                r = true;//结束
            }
        }

        lastProgressCount = progressCount;
        //float progress1 = (float)progressCount / targetCount;


        double loopTime = (DateTime.Now - loopStartTime).TotalSeconds;
        string time = loopTime.ToString();
        if (loopTime < 1)
        {
            time = loopTime.ToString("F2");
        }
        else if (loopTime < 10)
        {
            time = loopTime.ToString("F1");
        }
        else
        {
            time = loopTime.ToString("F0");
        }
        if (isLoopEnd)
        {

            Debug.Log($"完成一轮[{loopCount}][{time}s]:\t{loopStartMeshFilterCount - mfCount}={loopStartMeshFilterCount}->{mfCount},Prefab:{prefabInfoList.Count}(+{prefabInfoList.Count - lastPrafabCount}), AlignJob:{AlignJobCount}, DisJob:{AcRTAlignJobExResult.disJobCount} | " + loopInitLog);
            loopTimes += loopTime + ";";

            string logInfo = $"t:{time} ,P:{prefabInfoList.Count}(+{prefabInfoList.Count - lastPrafabCount})";

            var progressArg = ProgressArg.New($"Align{tag}{CurrentLogTag}", progressCount, targetCount, $"{loopCount}|j:{jobCount} d:{mfld.Count} c:{mfCount} {logInfo}", JobHandleList.testProgressArg);
            //AcRTAlignJob.progressArg = progressArg;
            JobHandleList.SetJobProgress(progressArg);

            if (ProgressBarHelper.DisplayCancelableProgressBar(progressArg))
            {
                //isCancel = true;//取消处理
                r = true;
                IsBreak = true;
            }
        }
        else
        {
            
        }



        lastPrafabCount = prefabInfoList.Count;

        return r;
    }

    DateTime loopStartTime ;

    public double totalTime1;
    public double totalTime2;
    public double totalTime3;
    public double totalTime4;
    public double totalTime5;

    public bool IsBreak = false;

    public PrefabInfoList GetPrefabs()
    {
        DateTime start = DateTime.Now;
        this.ResetLoopInfo();

        errorCount = 0;
        int allJobCount = 0;
        for (int kk = 0; kk < mfCount; kk++)
        {
            ShowProgressAndLog(false,0, "");

            loopStartTime = DateTime.Now;
            loopCount++;

            

            DateTime  tmpT = DateTime.Now;
            //1.初始化
            InitLoopData();
            totalTime1+=(DateTime.Now-tmpT).TotalMilliseconds;

            tmpT = DateTime.Now;

            //2.获取Jobs
            JobList<AcRTAlignJob> jobList =GetAlignJobs();
            allJobCount += jobList.Length;
            totalTime2+=(DateTime.Now-tmpT).TotalMilliseconds;
            AcRTAlignJob.ProgressMax = jobList.Length;
            AcRTAlignJob.ProgressIndex = 0;

            tmpT = DateTime.Now;

            //Debug.LogError($"GetPrefabs[{loopCount}] {kk}/{mfCount} jobList:{jobList.Length}");

            //break;

            //3.执行Jos
            jobList.CompleteAllPage();
            //foreach (var job in jobList.Jobs)
            //{
            //    job.vsFrom.Dispose();
            //    job.vsTo.Dispose();
            //    job.tpsFrom.Dispose();
            //    job.tpsTo.Dispose();
            //}
            jobList.Dispose();


            totalTime3+=(DateTime.Now-tmpT).TotalMilliseconds;

            tmpT = DateTime.Now;
            //4.执行Jos
            DoAlignJobResult();//处理Jobs的结果
            totalTime4 +=(DateTime.Now-tmpT).TotalMilliseconds;

            

            if (ShowProgressAndLog(true, jobList.Length,""))
            {
                //IsBreak = true;
                break;//结束
            }

            //break;
        }

        if (allArg.Count > 0)
        {
            double allT = 0;
            allArg.Sort();

            StringBuilder sb = new StringBuilder();
            MeshComparer.Instance.SetArg(allArg[0]);
            for (int i = 0; i < allArg.Count && i < 10; i++)
            {
                AcRtAlignJobArg arg = allArg[i];
                sb.AppendLine($"GetPrefabs arg[{i}]:{arg}");
                allT += arg.Time;
            }
            Debug.Log($"GetPrefabs allArg:{allArg.Count} allT:{allT} t:{(DateTime.Now - start)} \n{sb}");
        }
        else
        {
            Debug.LogWarning($"GetPrefabs allArg.Count == 0 allArg:{allArg.Count}");
        }

        ProgressBarHelper.ClearProgressBar();

        var usedTime=DateTime.Now - start;
        Debug.Log($"【AcRTAlignJobContainer.GetPrefabs{CurrentLogTag}】 usedTime:{usedTime} Target:{targetCount},Prefab:{prefabInfoList.Count},Job:{totalJobCount}({jobCountDetails}),Loop:{loopCount},Time:{usedTime.TotalSeconds:F2}s({loopTimes})");
        string testLogItem=$"{targetCount}\t{prefabInfoList.Count}\t{totalJobCount}\t{loopCount}\t{usedTime.TotalSeconds:F1}\t{usedTime.ToString()}\tFoundCount:{AcRTAlignJob.totalFoundCount}\tNoFoundCount:{AcRTAlignJob.totalNoFoundCount}\tFoundTime:{AcRTAlignJob.totalFoundTime:F0}\tNoFoundTime:{AcRTAlignJob.totalNoFoundTime:F0}\tAngle:{AcRTAlignJob.AngleCount}\tScale:{AcRTAlignJob.ScaleCount}\tRT:{AcRTAlignJob.RTCount}\tICP:{AcRTAlignJob.ICPCount}";

        //Debug.Log($"【AcRTAlignJobContainer.GetPrefabs{CurrentLogTag}】 FoundCount:{AcRTAlignJob.totalFoundCount},NoFoundCount:{AcRTAlignJob.totalNoFoundCount},FoundTime:{AcRTAlignJob.totalFoundTime:F0},NoFoundTime:{AcRTAlignJob.totalNoFoundTime:F0}");
        //Debug.Log($"【AcRTAlignJobContainer.GetPrefabs{CurrentLogTag}】 vsDictWorld:{AcRTAlignJobHelper.vsDictWorld.Count},tpDict:{AcRTAlignJobHelper.tpDict.Count}");
        AcRTAlignJobHelper.Clear();

        MeshAlignJobContainer.PrintTime();
        prefabInfoList.Sort();

        ReportLog=testLogItem;

        Debug.Log(testLogItem+$"\t{totalTime1:F0},{totalTime2:F0},{totalTime3:F0}({AcRTAlignJob.totalTime1:F0}={AcRigidTransform.totalTime1:F0}+{AcRigidTransform.totalTime2:F0},{AcRTAlignJob.totalTime2:F0},{AcRTAlignJob.totalTime3:F0}),{totalTime4:F0}");
        //65	2377	25	3632.2	ms(14.0,84.0,3496.3,17.0)
        return prefabInfoList;
    }

    public static string ReportLog="";

    public PrefabInfoList GetPrefabsEx()
    {
        DateTime start = DateTime.Now;
        this.ResetLoopInfo();

        for (int kk = 0; kk < mfCount; kk++)
        {
            ShowProgressAndLog(false,0, "Ex");

            loopStartTime = DateTime.Now;
            loopCount++;

            DateTime tmpT = DateTime.Now;
            //1.初始化
            InitLoopData();
            totalTime1 += (DateTime.Now - tmpT).TotalMilliseconds;

            tmpT = DateTime.Now;

            //2.获取Jobs
            //var jobList = GetDistanceJobs();
            var jobList = GetJobsEx();
            totalTime2 += (DateTime.Now - tmpT).TotalMilliseconds;

            tmpT = DateTime.Now;
            //3.执行Jos
            //Debug.LogError("before Do Job:" + jobList.Length);
            jobList.CompleteAllPage();
            // foreach (var job in jobList.Jobs)
            // {
            //     job.tpsFrom.Dispose();
            //     job.tpsTo.Dispose();
            //     //Debug.LogError("vsFrom:"+job.vsFrom.Length);
            //     break;
            // }
            jobList.Dispose();
            totalTime3 += (DateTime.Now - tmpT).TotalMilliseconds;

            tmpT = DateTime.Now;

            JobList<AcRTAlignDistanceJob> disJobs = AcRTAlignJobExResult.GetJobList(jobSize);
            //Debug.LogError("disJobs:"+ disJobs.Length);
            disJobs.CompleteAllPage();
            disJobs.Dispose();

            totalTime4 += (DateTime.Now - tmpT).TotalMilliseconds;
            tmpT = DateTime.Now;
            //4.执行Jos
            DoDistanceJobResultEx();//处理Jobs的结果
            totalTime5 += (DateTime.Now - tmpT).TotalMilliseconds;
            if (ShowProgressAndLog(true, jobList.Count, "Ex"))
            {
                break;//结束
            }

            //break;
        }

        ProgressBarHelper.ClearProgressBar();
        var usedTime = DateTime.Now - start;
        Debug.Log($"AcRTAlignJobContainer.GetJobs Target:{targetCount},Prefab:{prefabInfoList.Count},Job:{totalJobCount}({jobCountDetails}),Loop:{loopCount},Time:{usedTime.TotalSeconds:F2}s({loopTimes})");
        Debug.Log($"{prefabInfoList.Count}\t{totalJobCount}\t{loopCount}\t{usedTime.TotalMilliseconds:F0}ms\t{totalTime1:F0},{totalTime2:F0},{totalTime3:F0},{totalTime4:F0},{totalTime5:F0}");
        //65	2377	25	3632.2	ms(14.0,84.0,3496.3,17.0)
        Debug.Log($"FoundCount:{AcRTAlignJob.totalFoundCount},NoFoundCount:{AcRTAlignJob.totalNoFoundCount},FoundTime:{AcRTAlignJob.totalFoundTime:F0},NoFoundTime:{AcRTAlignJob.totalNoFoundTime:F0}");
        Debug.Log($"{prefabInfoList.Count}\t{totalJobCount}\t{loopCount}\t{usedTime.TotalMilliseconds:F0}\t{AcRTAlignJob.totalFoundCount}\t{AcRTAlignJob.totalNoFoundCount}\t{AcRTAlignJob.totalFoundTime:F0}\t{AcRTAlignJob.totalNoFoundTime:F0}");
        Debug.LogError($"{AcRTAlignJobHelper.vsDictWorld.Count},{AcRTAlignJobHelper.tpDict.Count}");
        MeshAlignJobContainer.PrintTime();
        prefabInfoList.Sort();
        return prefabInfoList;
    }

    public static MeshFilterListDict CreateMeshFilterListDict(MeshPoints[] meshFilters, int vertexCountOffset)
    {
        DateTime start = DateTime.Now;
        var mfld = new MeshFilterListDict(meshFilters, vertexCountOffset);
        //Debug.Log($"CreateMeshFilterListDict meshFilters:{meshFilters.Length},Time:{(DateTime.Now - start).TotalMilliseconds:F1}ms");
        return mfld;
    }
}

        


               


