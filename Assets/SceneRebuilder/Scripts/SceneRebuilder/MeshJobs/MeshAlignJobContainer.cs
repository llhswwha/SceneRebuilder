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
public class MeshAlignJobContainer
{
    public bool showLog=false;

    public GameObject go1;

    public GameObject go2;

    public MeshFilter mf1;

    public MeshFilter mf2;

    public int count;

    public int ResultId;

    // public MeshAlignJob Job;

    public TransformAccessArray tA;

    public  List<GameObject> copyList;

    public NativeArray<ThreePoint> psArray1;
    public NativeArray<ThreePoint> psArray2;

    public void Init(int count)
    {
        this.count=count;
        psArray1=new NativeArray<ThreePoint>(count,Allocator.TempJob);
        psArray2=new NativeArray<ThreePoint>(count,Allocator.TempJob);
    }

    public void SetIds(int i,ThreePoint p1,ThreePoint p2)
    {
        psArray1[i]=p1;
        psArray2[i]=p2;
    }

    public JobHandle CreateJob()
    {
        MeshAlignJob job=new MeshAlignJob();
        job.ps1=this.psArray1;
        job.ps2=this.psArray2;
        return job.Schedule(tA);
    }

    
    public MeshDistanceJobResult Result;

    public MeshAlignJobContainer(MeshFilter mf1,MeshFilter mf2,bool showLog)
    {
        this.mf1=mf1;
        this.mf2=mf2;
        this.showLog=showLog;
         //go1不动，go2匹配go1。

        // mf1=go1.GetComponent<MeshFilter>();
        // mf2=go2.GetComponent<MeshFilter>();

        this.go1=mf1.gameObject;
        this.go2=mf2.gameObject;
    }

    ThreePoint[] ps1;
    ThreePoint[] ps2;

    
    public static double TotalGetThreeeInfoTime=0;

    private void GetThreePointInfo()
    {
        DateTime start=DateTime.Now;
        // ThreePointJobResult r1=ThreePointJobResultList.Instance.GetThreePointResult(mf1);
        // r1.localToWorldMatrix=go1.transform.localToWorldMatrix;
        // ThreePointJobResult r2=ThreePointJobResultList.Instance.GetThreePointResult(mf2);
        // ps1 = r1.GetThreePoints(go1.transform.localToWorldMatrix);
        // ps2 = r2.GetThreePoints(go2.transform.localToWorldMatrix); 

        ps1=ThreePointJobResultList.Instance.GetThreePoints(mf1);
        ps2=ThreePointJobResultList.Instance.GetThreePoints(mf2);

        count=ps1.Length*ps2.Length;
        double t=(DateTime.Now-start).TotalMilliseconds;
        TotalGetThreeeInfoTime+=t;

        //if(showLog)Debug.Log($"count1:{ps1.Length},count2:{ps2.Length},countAll:{count},go1:{go1.GetInstanceID()},{r1}|go2:{go2.GetInstanceID()},r2:{r2},");

        if(showLog)Debug.Log($"count1:{ps1.Length},count2:{ps2.Length},countAll:{count},go1:{go1.GetInstanceID()}|go2:{go2.GetInstanceID()}");

        if(showLog)Debug.Log($"GetThreePointInfo Count:{count},Time:{t}ms");// 9/21
    }

    public static double TotalCopyTime=0;



    private void CopyGos()
    {
        DateTime start=DateTime.Now;
        Transform[] ts=new Transform[count];
        int i=0;
        this.copyList=new List<GameObject>();
        for(int l=0;l<ps1.Length;l++)
        {
            var id1=ps1[l];
            for(int k= 0; k<ps2.Length;k++)
            {
                var id2=ps2[k];
                this.SetIds(i,id1,id2);

                GameObject go2Copy=MeshHelper.CopyGO(go2);
                go2Copy.SetActive(false);
                string tag=$"|({id1.maxId},{id1.minId})({id2.maxId},{id2.minId})";
                go2Copy.name+=tag;

                copyList.Add(go2Copy);
                ts[i]=go2Copy.transform;
                i++;
                //id2.ShowDebugDetail();
                //break;
            }
            //break;
        }
        this.tA=new TransformAccessArray(ts);

        double t=(DateTime.Now-start).TotalMilliseconds;
        TotalCopyTime+=t;
        if(showLog)Debug.Log($"InitAlignInfo Count:{count},Time:{t}ms");// 9/21
    }

    public static double TotalInitAlignInfoTime=0;

    public void InitAlignInfo()
    {
        DateTime start=DateTime.Now;
        GetThreePointInfo();

        this.Init(count);
        CopyGos();
        double t=(DateTime.Now-start).TotalMilliseconds;
        TotalInitAlignInfoTime+=t;
        if(showLog)Debug.Log($"InitAlignInfo Count:{count},Time:{t}ms");// 9/21
        //return alignJob.Schedule(tA);
    }

    public static double TotalAlignJobTime=0;

    public void DoAlignJob()
    {
        DateTime start=DateTime.Now;
        this.CreateJob().Complete();//4/21ms
        double t=(DateTime.Now-start).TotalMilliseconds;
        TotalAlignJobTime+=t;
        if(showLog)Debug.Log($"DoAlignJob Count:{count},Time:{t}ms");
    }

    public static double TotalDistanceJobTime=0;

    public void DoDistanceJobs()
    {
        DateTime start=DateTime.Now;
        distanceJobs.CompleteAllPage();//13/21ms
        double t=(DateTime.Now-start).TotalMilliseconds;
        TotalDistanceJobTime+=t;
        if(showLog)Debug.Log($"DoDistanceJobs Count:{distanceJobs.Length},Time:{t}ms");
    }

    public static double TotalTime=0;

    public static void InitTime(){
        TotalAlignJobTime=0;
        TotalDistanceJobTime=0;
        TotalCreateMeshDistanceJobsTime=0;
        TotalInitAlignInfoTime=0;
        TotalTime=0;
        TotalGetThreeeInfoTime=0;
        TotalCopyTime=0;
    }

    public static void PrintTime()
    {
        Debug.Log($"[PrintTime] InitAlignInfo:{TotalInitAlignInfoTime:F1}({TotalGetThreeeInfoTime:F1}+{TotalCopyTime:F1}),AlignJob:{TotalAlignJobTime:F1},InitDistance:{TotalCreateMeshDistanceJobsTime:F1},DistanceJob:{TotalDistanceJobTime:F1},TotalTime:{TotalTime:F1}|CopyTime:{MeshHelper.TotalCopyTime:F1},CopyCount:{MeshHelper.TotalCopyCount}");
    }

    JobList<MeshDistanceJob> distanceJobs;

    public static double TotalCreateMeshDistanceJobsTime=0;

    public JobList<MeshDistanceJob> CreateMeshDistanceJobs()
    {
        DateTime start=DateTime.Now;

        // //调试用，显示细节
        // foreach(var go in copyList){
        //     MeshNode node=MeshNode.CreateNew(go);
        //     node.GetVertexCenterInfo(true,true,Vector3.zero);
        // }

        //3.比较距离，找出最小的


        Vector3[] vertices1=mf1.sharedMesh.vertices;
        Vector3[] vertices1World=MeshHelper.GetWorldVertexes(vertices1,go1.transform);
        NativeArray<Vector3> verticesArray1=new NativeArray<Vector3>(vertices1World,Allocator.TempJob);

        
        Vector3[] vertices2=mf2.sharedMesh.vertices;
        NativeArray<Vector3> verticesArray2=new NativeArray<Vector3>(vertices2,Allocator.TempJob);

        // MeshDistanceJob.minDis=float.MaxValue;
        // MeshDistanceJob.minDisId=0;

        int rId=MeshDistanceJobHelper.InitResult();

        JobList<MeshDistanceJob> jobList=new JobList<MeshDistanceJob>(10000);
        for (int i1 = 0; i1 < copyList.Count; i1++)
        {
            GameObject go = copyList[i1];
            MeshDistanceJob job1=new MeshDistanceJob{
                Id=i1,
                RId=rId,
                // Matrix1=go1.transform.localToWorldMatrix,
                Matrix2=go.transform.localToWorldMatrix,
                vertices1=verticesArray1,
                vertices2=verticesArray2
            };
            jobList.Add(job1);
        }

        this.ResultId=rId;

        double t=(DateTime.Now-start).TotalMilliseconds;
        TotalCreateMeshDistanceJobsTime+=t;

        if(showLog)Debug.Log($"CreateMeshDistanceJobs Count:{count},Time:{t}ms");//4/21

        distanceJobs=jobList;
        return jobList;
    }


    private GameObject DestroyCopyGos()
    {
        Result=MeshDistanceJobHelper.results[ResultId];
        //if(showLog)Debug.Log($"NewMeshAlignJob22 Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms,Dis:{result.min},Id:{result.minId},Zero:{result.IsZero}");
        //21ms

        //4.清理多余的模型
        GameObject minGo=null;
        for (int i1 = 0; i1 < copyList.Count; i1++)
        {
            GameObject go = copyList[i1];
            if(i1!=Result.minId)
            {
                GameObject.DestroyImmediate(go);
            }
            else
            {
                go.SetActive(true);
                go.name=go1.name+"_New";
                minGo=go;
            }
        }
        return minGo;
    }

    public MeshDistanceJobResult GetResult()
    {
        DateTime start=DateTime.Now;
        var minGo=this.DestroyCopyGos();
        var result=this.Result;

        if(result.IsZero){
            // GameObject.DestroyImmediate(go1);
            // GameObject.DestroyImmediate(go2);
            result.Instance=minGo;
        }
        else
        {
            if(result.min<0.02f)//判断是否相对很近
            {
                //if(long22<0.0001 && short22<0.0001 &&angle1<=0.09f&&angle2<=0.09f&&dis2<=0.02f)//几个条件，确保差异不大。这种情况下或许还有结合ICP算法
                ThreePoint node1=this.psArray1[result.minId];
                ThreePoint node2=this.psArray2[result.minId];
                bool isRelativeZero=IsActiveZero(node1,node2,this.showLog);
                if(isRelativeZero)//长短轴相似，角度不管。就算角度不合适，重合就行
                {
                    result.IsZero=true;
                    result.Instance=minGo;
                }
                else
                {
                    if(minGo!=null){
                        GameObject.DestroyImmediate(minGo);
                    }
                }
            }
            else{
                if(minGo!=null){
                    GameObject.DestroyImmediate(minGo);
                }
            }
        }
        if(showLog)Debug.Log($"GetResult Count:{count},Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        return result;
    }

    private static bool IsActiveZero(ThreePoint node1,ThreePoint node2,bool showLog)
    {
        var longLine1=node1.GetLongLine();
        var shortLine1=node1.GetShortLine();
        var angel1=Vector3.Angle(longLine1, shortLine1);

        var longLine2=node2.GetLongLine();
        var shortLine2=node2.GetShortLine();
        var angel2=Vector3.Angle(longLine2, shortLine2);

        var angle22 = Math.Abs(angel1 - angel2);
        var long22 = Math.Abs(longLine1.magnitude- longLine2.magnitude);
        var short22= Math.Abs(shortLine1.magnitude - shortLine2.magnitude);
        var rate22 = Math.Abs(longLine1.magnitude/shortLine1.magnitude - longLine2.magnitude/shortLine2.magnitude);

        var angle1 =Vector3.Angle(shortLine2, shortLine1);
        var angle2=Vector3.Angle(longLine2, longLine1);
        if(showLog)Debug.Log($"After ShortLine angle1:{angle1} | LongLine angle2:{angle2} ");
        if(showLog)Debug.Log($"node1-node2 angle:{angle22},long:{long22},short:{short22},rate:{rate22} |");
        if(showLog)Debug.LogError($"IsRelativeZero: {long22<0.0001}|{short22<0.0001}|{angle1<=0.09f}|{angle2<=0.09f}");

        //if(long22<0.0001 && short22<0.0001 &&angle1<=0.09f&&angle2<=0.09f)//几个条件，确保差异不大。这种情况下或许还有结合ICP算法
        if(long22<0.0001 && short22<0.0001)//长短轴相似，角度不管。就算角度不合适，重合就行
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
}
