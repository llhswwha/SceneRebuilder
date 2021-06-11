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
    public struct CreateRayCommandsJob : IJobParallelFor
    {
        [ReadOnly] public float3 position;
        [ReadOnly] public quaternion rotation;

        [ReadOnly] public int dirsOffsetIdx;

        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<float3> rayDirs;

        [ReadOnly] public int mask;
        [WriteOnly] public NativeArray<RaycastCommand> rayCommands;

        public void Execute(int index)
        {
            int id=dirsOffsetIdx + index;
            float3 dir=rayDirs[id];
            //IndexOutOfRangeException: Index 141820 is out of restricted IJobParallelFor range [320...383] in ReadWriteBuffer.
            //ReadWriteBuffers are restricted to only read & write the element at the job index. You can use double buffering strategies to avoid race conditions due to reading & writing in parallel to the same elements from a job.
            float3 direction = math.mul(rotation, dir);

            // float3 end = position + direction * 100;
            // Debug.DrawLine(position, end, Color.red,0.1f);//测试
            if(StaticCullingMaster.IsUseMask)
            {
                RaycastCommand command = new RaycastCommand(position, direction, layerMask : mask);
                rayCommands[index] = command;
            }
            else
            {
                RaycastCommand command = new RaycastCommand(position, direction);
                rayCommands[index] = command;
            }
        }
    }
}
