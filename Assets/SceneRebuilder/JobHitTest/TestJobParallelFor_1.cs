using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TestJobParallelFor_1 : MonoBehaviour
{
    public int dataLength = 10;

    [BurstCompile]
    public struct MyParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> a;

        [ReadOnly] public NativeArray<float> b;

        [WriteOnly]
        public NativeArray<float> result;
        public void Execute(int index)
        {
            result[index] = a[index] + b[index];
            Debug.Log($"MyParallelJob result[{index}]:{result[index]}");
        }
    }

    [ContextMenu("ScheduleParallelJobOne")]
    private void ScheduleParallelJobOne()
    {
        ScheduleParallelJob();
    }

    public bool IsUpdate = false;

    private void ScheduleParallelJob()
    {
        int count = dataLength;
        NativeArray<float> a = new NativeArray<float>(count, Allocator.TempJob);
        NativeArray<float> b = new NativeArray<float>(count, Allocator.TempJob);
        NativeArray<float> result = new NativeArray<float>(count, Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            a[i] = i * Random.Range(0f,1f);
            b[i] = i * Random.Range(0f, 1f);
        }
        MyParallelJob job = new MyParallelJob();
        job.a = a;
        job.b = b;
        job.result = result;
        JobHandle handle = job.Schedule(count, 1);
        handle.Complete();

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"ScheduleParallelJob result[{i}]:{result[i]}");
        }
        a.Dispose();
        b.Dispose();
        result.Dispose();
    }

    private void Update()
    {
        if (IsUpdate)
        {
            ScheduleParallelJob();
        }
    }
}
