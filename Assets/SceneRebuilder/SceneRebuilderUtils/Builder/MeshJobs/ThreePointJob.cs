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

 


 

    public static class TreePointJobHelper
    {
        public static bool IsShowProgress = true;

        public static JobList<ThreePointJob> CreateThreePointJobs(MeshPoints[] meshFilters, int size)
        {
            //DateTime start = DateTime.Now;
            int count = meshFilters.Length;
            JobList<ThreePointJob> handles = new JobList<ThreePointJob>("ThreePointJob", size);

            ThreePointJobResultList.Instance.InitResult(count);

            for (int i = 0; i < count; i++)
            {
                //float progress = (float)i / count;
                //float percents = progress * 100;

                if (IsShowProgress) {
                    var p1 = ProgressArg.New("ThreePointJob", i, count, null, JobHandleList.testProgressArg);
                    if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                    {
                        break;
                    }
                }


                MeshPoints filter = meshFilters[i];

                ThreePointJob job1 = new ThreePointJob();
                job1.id = i;
                //job1.goId = MeshHelper.GetInstanceID(filter);//模型会复制，但是sharedMesh不会改变
                job1.goId = filter.InstanceId;
                                                              //job1.results=results;
                job1.InitVertex(filter.vertices);
                handles.Add(job1);
            }
            if(IsShowProgress)
                ProgressBarHelper.ClearProgressBar();
            //Debug.Log($"CreateThreePointJobs Time:{(DateTime.Now - start).TotalMilliseconds}ms");
            return handles;
        }

        public static MeshPoints[] NewThreePointJobs(GameObject[] objs, int size)
        {
            MeshPoints[] meshFilters = new MeshPoints[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                meshFilters[i] = new MeshPoints(objs[i]);
            }
            NewThreePointJobs(meshFilters, size);
            return meshFilters;
        }

        public static void NewThreePointJobs(MeshPoints[] meshFilters, int size)
        {
            //DateTime start = DateTime.Now;
            //Debug.Log("NewThreePointJobs:" + meshFilters.Length);
            JobList<ThreePointJob> jobList = CreateThreePointJobs(meshFilters, size);
            //handles.CompleteAll();
            jobList.CompleteAllPage();
            foreach(var job in jobList.Jobs)
            {
                job.Dispose();
            }
            jobList.Dispose();

            ThreePointJobResultList.Instance.InitDict();

            //ThreePointJobResultList.Instance.Print();//打印确认结果

            //下面的不行，ThreePointJobResult是结构体的情况下，下面的没有数据
            //    List<ThreePointJobResult> results=new List<ThreePointJobResult>();
            //     for (int i = 0; i < jobList.Jobs.Count; i++)
            //    {
            //         ThreePointJob job = jobList.Jobs[i];
            //         //results.Add(job.Result);
            //         //job.Result.Print();
            //         //job.PrintResult();
            //         //job.GetResult().Print();
            //         // job.center.PrintVector3($"[{i}]job.center");
            //         Debug.Log($"[{i}] Job id:{job.id},count:{ThreePointJob.InvokeCount},minDis:{job.minDis},maxDis:{job.maxDis},center:{job.center},center2:{job.center2}");
            //    }

            //Debug.Log($">>NewThreePointJobs Count:{jobList.Length},Time:{(DateTime.Now - start).TotalMilliseconds}ms");//15/35
        }
    }

    [BurstCompile]
    public struct ThreePointJob : IJob
    {
        public int id;

        public int goId;

        public int vertexCount;
        // public NativeArray<float3> vertices;

        [ReadOnly] public NativeArray<Vector3> vertices;

        // public ThreePointJobResult Result;

        //public NativeArray<ThreePointJobResult> results;

        //public static int InvokeCount;

        public Vector3 center;

        public float3 center2;

        public float maxDis;
        public float minDis;

        public NativeList<Vector3> maxPList;

        public int minPCount;

        public NativeList<Vector3> minPList;

        public void Dispose()
        {
            maxPList.Dispose();
            minPList.Dispose();
            vertices.Dispose();
        }

        public int maxPCount;

        public void Execute()
        {
#if UNITY_EDITOR
            //DateTime start = DateTime.Now;

            // Mesh mesh=ManagedObjectWorld.Instance.Get(MeshRef);
            // Vector3[] vertices=mesh.vertices;//UnityException: get_canAccess can only be called from the main thread.
            // vertexCount=vertices.Length;

            Vector3 sumP = Vector3.zero;
            for (int i = 0; i < vertexCount; i++)
            {
                var v = vertices[i];
                sumP += v;
            }

            //center = sumP / this.vertexCount+centerOffset;//偏移一下避免对称模型无法得到唯一的特征点

            this.center = sumP / this.vertexCount;

            // center=Result.center ;
            // center2=Result.center;

            Vector3 maxP = Vector3.zero;
            Vector3 minP = Vector3.zero;
            this.maxDis = 0;
            this.minDis = float.MaxValue;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 p = vertices[i];
                var dis = Vector3.Distance(p, this.center);
                if (dis == this.maxDis)
                {
                    if (p == maxP) continue;//重复点
                    if (!this.maxPList.Contains(p))
                    {
                        this.maxPList.Add(p);
                    }
                }
                if (dis > this.maxDis)
                {
                    this.maxPList.Clear();
                    this.maxPList.Add(p);
                    this.maxDis = dis;
                    maxP = p;
                }

                if (dis == this.minDis)
                {
                    if (p == minP) continue;//重复点
                                            //minPList.Add(p);
                    if (!this.minPList.Contains(p))
                    {
                        this.minPList.Add(p);
                    }
                }
                if (dis < this.minDis)
                {
                    this.minPList.Clear();
                    this.minPList.Add(p);
                    this.minDis = dis;
                    minP = p;
                }
            }

            //InvokeCount++;

            // Debug.LogWarning(string.Format("GetVertexCenterInfo[{9}] vertexCount:{0},time:{1}ms,center:{2},maxP:{3},minP:{4},maxDis:{5},minDis:{6},{7},{8};",
            //     this.vertexCount,(DateTime.Now-start).TotalMilliseconds,this.center,maxP,minP,this.maxDis,this.minDis,this.maxPList.Length,this.minPList.Length,InvokeCount));

            // this.id=this.id;
            // this.count=InvokeCount;
            // this.time=(DateTime.Now-start).TotalMilliseconds;

            //Result.Print();

            //if (this.maxPList.Length > 1 || this.minPList.Length > 1)
            //{
            //    Debug.LogError(string.Format("存在多个最远点和最近点! maxDis:{0},minDis:{1}", this.maxPList.Length, this.minPList.Length));
            //}

            if (maxPList.Length > 10 || minPList.Length > 10 || minPList.Length* maxPList.Length>64)
            {
                Debug.LogWarning($"[ThreePointJob] 存在大量最远点和最近点! id:{id} goId:{goId}  maxDis:{maxPList.Length},minDis:{minPList.Length}");
            }

            //Debug.Log(string.Format("ThreePointJob[{0}] vertexCount:{1}",InvokeCount,vertexCount));

            //results[id]=Result;

            //ThreePointJobResultList.Instance.Add(Result);//不能加

            // // 保存结果，后续使用
            // ThreePointJobResult result = GetResult();

            ThreePointJobResult result = new ThreePointJobResult();
            result.id = this.id;
            //result.count = InvokeCount;
            result.vertexCount = this.vertexCount;
            result.center = this.center;

            result.maxPList = this.maxPList.ToArray();
            result.maxPCount = this.maxPCount;
            result.maxDis = this.maxDis;

            result.minPList = this.minPList.ToArray();
            result.minPCount = this.minPCount;
            result.minDis = this.minDis;


            //result.time = (DateTime.Now - start).TotalMilliseconds;
            ThreePointJobResultList.Instance.SetResult(result, id, goId);
#endif
        }


        // public void PrintResult()
        // {
        //     Result.Print();
        // }

        // public ThreePointJobResult GetResult()
        // {
        //     ThreePointJobResult result = new ThreePointJobResult();
        //     result.id = this.id;
        //     //result.count = InvokeCount;
        //     result.vertexCount = this.vertexCount;
        //     result.center = this.center;

        //     result.maxPList = this.maxPList.ToArray();
        //     result.maxPCount = this.maxPCount;
        //     result.maxDis = this.maxDis;

        //     result.minPList = this.minPList.ToArray();
        //     result.minPCount = this.minPCount;
        //     result.minDis = this.minDis;
        //     return result;
        // }

        //private ManagedObjectRef<Mesh> MeshRef;

        //private ManagedObjectRef<Vector3[]> vs;

        //public void InitVertex(Mesh mesh)
        //{
        //    //MeshRef=ManagedObjectWorld.Instance.Add(mesh);//78ms

        //    //DateTime start=DateTime.Now;
        //    vertexCount = mesh.vertexCount;
        //    //vs=ManagedObjectWorld.Instance.Add(mesh.vertices);//958ms/484ms
        //    vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob);//14641个2318个点要1117ms，没有这句话要90ms
        //                                                                          //vertexCount=0;
        //                                                                          //vertices= new NativeList<Vector3>(Allocator.TempJob);
        //                                                                          //   Result=new ThreePointJobResult();
        //                                                                          //   Result.minPList= new NativeList<Vector3>(Allocator.Persistent);
        //                                                                          //   Result.maxPList= new NativeList<Vector3>(Allocator.Persistent);

        //    minPList = new NativeList<Vector3>(Allocator.TempJob);
        //    maxPList = new NativeList<Vector3>(Allocator.TempJob);

        //    //   Vector3[] vs=mesh.vertices;//发现mesh.vertices放遍历里面的成本很大，要事先取出来再用
        //    //   vertices=new NativeArray<Vector3>(vertexCount,Allocator.TempJob);
        //    //   Vector3 sum=Vector3.zero;
        //    //   for(int i=0;i<vertexCount;i++) //遍历要花费时间，2318多个点要120-140ms
        //    //   {
        //    //     //var v=mesh.vertices[i];//120-140ms
        //    //     //sum+=v;
        //    //     //vertices[i]=mesh.vertices[i];//两个的话双倍

        //    //     var v=vs[i];
        //    //   }

        //    //Debug.Log($"InitVertex Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        //}

        public void InitVertex(Vector3[] vs)
        {
            //MeshRef=ManagedObjectWorld.Instance.Add(mesh);//78ms

            //DateTime start=DateTime.Now;
            vertexCount = vs.Length;
            //vs=ManagedObjectWorld.Instance.Add(mesh.vertices);//958ms/484ms
            vertices = new NativeArray<Vector3>(vs, Allocator.TempJob);//14641个2318个点要1117ms，没有这句话要90ms
                                                                                  //vertexCount=0;
                                                                                  //vertices= new NativeList<Vector3>(Allocator.TempJob);
                                                                                  //   Result=new ThreePointJobResult();
                                                                                  //   Result.minPList= new NativeList<Vector3>(Allocator.Persistent);
                                                                                  //   Result.maxPList= new NativeList<Vector3>(Allocator.Persistent);

            minPList = new NativeList<Vector3>(Allocator.TempJob);
            maxPList = new NativeList<Vector3>(Allocator.TempJob);

            //   Vector3[] vs=mesh.vertices;//发现mesh.vertices放遍历里面的成本很大，要事先取出来再用
            //   vertices=new NativeArray<Vector3>(vertexCount,Allocator.TempJob);
            //   Vector3 sum=Vector3.zero;
            //   for(int i=0;i<vertexCount;i++) //遍历要花费时间，2318多个点要120-140ms
            //   {
            //     //var v=mesh.vertices[i];//120-140ms
            //     //sum+=v;
            //     //vertices[i]=mesh.vertices[i];//两个的话双倍

            //     var v=vs[i];
            //   }

            //Debug.Log($"InitVertex Time:{(DateTime.Now-start).TotalMilliseconds}ms");
        }
    }
}