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

    public void Dispose()
    {
        HandleList.Dispos();
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

        public void CompleteAll()
        {
            JobHandle.CompleteAll(handles);
        }

        public void CompleteAllPage()
        {
            int count=pages.Count;
            if(count==1){
                CompleteAll();
                return;
            }
            for(int i=0;i<count;i++)
            {
                float progress = (float)i / count;
                float percents = progress * 100;
                if(ProgressBarHelper.DisplayCancelableProgressBar("CompleteAllPage", $"{name}:{i}/{count} {percents}% of 100%", progress))
                {
                    break;
                }
                var page=pages[i];
                JobHandle.CompleteAll(page);
            }
            ProgressBarHelper.ClearProgressBar();
        }

    public void Dispos()
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