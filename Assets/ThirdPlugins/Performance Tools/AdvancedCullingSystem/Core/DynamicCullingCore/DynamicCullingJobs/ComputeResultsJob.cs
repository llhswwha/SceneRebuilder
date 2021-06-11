using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using System;
using AdvancedCullingSystem.StaticCullingCore;

namespace AdvancedCullingSystem.DynamicCullingCore
{
    [BurstCompile]
    public struct ComputeResultsJob : IJob
    {
        public NativeList<int> visibleObjects;

        [ReadOnly]
        public NativeList<int> hittedObjects;//碰撞检测的结果，只读

        [WriteOnly]
        public NativeList<float> timers;//只写

        public void Execute()
        {
            for (int i = 0; i < hittedObjects.Length; i++)
            {
                int id = hittedObjects[i];//renderer InstancerID
                int index = visibleObjects.IndexOf(id);

                if (index < 0)//新的,增加
                {
                    visibleObjects.Add(id);
                    timers.Add(0);
                }
                else
                    timers[index] = 0; //刷新时间
            }
        }
    }
}
