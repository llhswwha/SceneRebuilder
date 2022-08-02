using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace JobHitManagers
{
    public struct CreateHitCommandsJob : IJobParallelFor
    {
        [ReadOnly] public float3 positin;

        [ReadOnly] public quaternion rotation;

        [ReadOnly] public int dirsOffsetIdx;

        [ReadOnly] public NativeArray<float3> rayDirs;

        [ReadOnly] public int mask;

        [WriteOnly] public NativeArray<RaycastCommand> rayCommands;

        public void Execute(int index)
        {
            Debug.Log($"CreateHitCommandsJob index:{index}");
            float3 direction = math.mul(rotation, rayDirs[dirsOffsetIdx + index]);
            RaycastCommand command = new RaycastCommand(positin, direction, layerMask: mask);
            rayCommands[index] = command;
        }
    }

    public struct UpdateTimerJob : IJob
    {
        public NativeArray<float> timers;

        [ReadOnly] public float deltaTime;

        public void Execute()
        {
            for (int i = 0; i < timers.Length; i++)
            {
                timers[i] += deltaTime;
            }
        }
    }

    public struct ComputeResultsJob : IJob
    {
        public NativeList<int> visibleObjects;

        public NativeList<int> hittedObjects;

        public NativeList<float> timers;

        public void Execute()
        {
            for(int i = 0; i < hittedObjects.Length; i++)
            {
                int id = hittedObjects[i];
                int index = visibleObjects.IndexOf(id);
                if(index<0)
                {
                    visibleObjects.Add(id);
                    timers.Add(0);
                }
                else
                {
                    timers[index] = 0;
                }
            }
        }
    }
}
