using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
using System;

  public class JobList<T> where T : struct, IJob
    {
        public JobHandleList HandleList;

        public List<T> Jobs=new List<T>();

        

        public int Length{
            get{
                return Jobs.Count;
            }
        }

    public int Count
    {
        get
        {
            return Jobs.Count;
        }
    }

    public JobList(string name,int size)
        {
            HandleList=new JobHandleList(name,size);
        }

        public JobList(int size)
        {
            HandleList=new JobHandleList(typeof(T).Name,size);
        }

        public void Add(T job)
        {
            Jobs.Add(job);
            JobHandle handle=job.Schedule();
            HandleList.Add(handle);
        }


        public void CompleteAll()
        {
            HandleList.CompleteAll();
        }

        public void CompleteAllPage()
        {
            HandleList.CompleteAllPage();
        }

    public void CompletePageN(int n)
    {

    }

    public void Dispose()
    {
        foreach(var job in Jobs)
        {
            IDisposable dis = job as IDisposable;
            if (dis != null)
            {
                dis.Dispose();
            }
        }
        HandleList.Dispose();
    }
    }

    public class JobHandleList
    {
        public string name;
        public int size;

        public JobHandleList(string name,int size)
        {
            this.name=name;
            this.size=size;

            handles = new NativeList<JobHandle>(Allocator.Persistent);
            currentPage = new NativeList<JobHandle>(Allocator.Persistent);
            pages = new List<NativeList<JobHandle>>();

            pages.Add(currentPage);
        }

        NativeList<JobHandle> handles ;

        NativeList<JobHandle> currentPage ;

        List<NativeList<JobHandle>> pages ;

        public int Length{
            get{
                return handles.Length;
            }
        }

        public void Add(JobHandle handle)
        {
            handles.Add(handle);

            currentPage.Add(handle);
            if(currentPage.Length>=size){
                currentPage = new NativeList<JobHandle>(Allocator.Persistent);
                pages.Add(currentPage);
            }
        }

    public void Add(JobHandleList list)
    {
        foreach(var item in list.handles)
        {
            this.Add(item);
        }
    }

    public void CompleteAll()
        {
            JobHandle.CompleteAll(handles);
        }

    public static ProgressArg testProgressArg;

    private static IProgressArg jobProgressArg;

    public static void SetJobProgress(IProgressArg arg)
    {
        //if (testProgressArg != null)
        //{
        //    testProgressArg.AddSubProgress(arg);
        //    jobProgressArg = testProgressArg;
        //}
        //else
        //{
        //    jobProgressArg = arg;
        //}

        jobProgressArg = arg;
    }

    public void CompleteAllPage(bool isShowProgress=false)
    {
        int count = pages.Count;
        if (count == 1)
        {
            CompleteAll();
            return;
        }
        for (int i = 0; i < count; i++)
        {
#if UNITY_EDITOR
            if (isShowProgress)
            {
                //Debug.LogError($"CompleteAllPage {i}/{count}");
                var subProgress = ProgressArg.New("DoJobs...", i, count, name, jobProgressArg);
                //if (jobProgressArg != null)
                //{
                //    jobProgressArg.AddSubProgress(subProgress);
                //    if (ProgressBarHelper.DisplayCancelableProgressBar(jobProgressArg))
                //    {
                //        break;
                //    }
                //}
                //else
                //{
                //    if (ProgressBarHelper.DisplayCancelableProgressBar(subProgress))
                //    {
                //        break;
                //    }
                //}
                if (ProgressBarHelper.DisplayCancelableProgressBar(subProgress))
                {
                    break;
                }
            }
#endif

            //if (ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"{name}:{i}/{count} {percents:F1}%", progress))
            //    {
            //        break;
            //    }
            var page = pages[i];
            JobHandle.CompleteAll(page);
        }
        //ProgressBarHelper.ClearProgressBar();
    }

    public void Dispose()
    {
        int count = pages.Count;
        for (int i = 0; i < count; i++)
        {
            var page = pages[i];
            page.Dispose();
        }
        pages.Clear();
        handles.Dispose();
        //currentPage.Dispose();
    }
    }