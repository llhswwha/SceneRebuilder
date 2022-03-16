#if UNITY_ECS
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnerPrefabSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem bufferSystem;
    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var commandBuffer = bufferSystem.CreateCommandBuffer();
        Entities
            .WithName("SpawnerPrefabSystem")
            .WithBurst(Unity.Burst.FloatMode.Default, Unity.Burst.FloatPrecision.Standard, true)
            .ForEach((Entity entity, in SpawnerPrefab prefab) =>
        {

            for (int i = 0; i < prefab.PosList.Length; i++)
            {
                float3 pos = prefab.PosList[i];
                Entity newEntity = commandBuffer.Instantiate(prefab.Prefab);
                commandBuffer.SetComponent(newEntity, new Translation() { Value = pos });
                //if (prefab.Angles.Length > i)
                //{
                //    quaternion angle = prefab.Angles[i];
                //}
            }
            commandBuffer.DestroyEntity(entity);
        }).ScheduleParallel();
        bufferSystem.AddJobHandleForProducer(Dependency);
    }
}
#endif