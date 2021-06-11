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
    public struct UpdateTimersJob : IJob
    {
        public NativeList<float> timers;

        [ReadOnly]
        public float deltaTime;

        public void Execute()
        {
            for (int i = 0; i < timers.Length; i++)
                timers[i] += deltaTime;
        }
    }
}
