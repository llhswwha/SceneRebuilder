using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

namespace NGS.AdvancedRenderSystem
{
    [BurstCompile]
    public struct UpdateBillboardDataJob : IJobParallelFor
    {
        public NativeArray<BillboardData> billboardDatas;
        public float3 cameraPosition;
        public float deltaTime;

        public void Execute(int idx)
        {
            billboardDatas[idx] = billboardDatas[idx].UpdateData(cameraPosition, deltaTime);
        }
    }

    [BurstCompile]
    public struct QuickSortJob<T, C> : IJob
        where T : struct
        where C : struct, IComarator<T>
    {
        [ReadOnly]
        public NativeArray<T> sourceArray;
        public NativeArray<T> sortedArray;
        public C comparer;

        public void Execute()
        {
            sortedArray.CopyFrom(sourceArray);

            QuickSort(0, sourceArray.Length - 1);
        }

        private void QuickSort(int a, int b)
        {
            if (a >= b)
                return;

            int c = Partition(a, b);

            QuickSort(a, c - 1);
            QuickSort(c + 1, b);
        }

        private int Partition(int a, int b)
        {
            int i = a;

            for (int j = a; j <= b; j++)
            {
                if (comparer.Compare(sortedArray[j], sortedArray[b]) <= 0)
                {
                    T t = sortedArray[i];

                    sortedArray[i] = sortedArray[j];
                    sortedArray[j] = t;

                    i++;
                }
            }

            return i - 1;
        }
    }


    public interface IComarator<T>
    {
        int Compare(T obj1, T obj2);
    }

    public struct BillboardDataDistanceComarator : IComarator<BillboardData>
    {
        public int Compare(BillboardData obj1, BillboardData obj2)
        {
            if (obj1.distance > obj2.distance)
                return 1;

            //if (obj1.distance == obj2.distance)
            //    return 0;

            return -1;
        }
    }

    public struct BillboardDataTimeComarator : IComarator<BillboardData>
    {
        public int Compare(BillboardData obj1, BillboardData obj2)
        {
            if (obj1.lastUpdateTime < obj2.lastUpdateTime)
                return 1;

            //if (obj1.distance == obj2.distance)
            //    return 0;

            return -1;
        }
    }
}
