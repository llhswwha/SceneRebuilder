using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using System;

//public class VelocityScript : MonoBehaviour,IConvertGameObjectToEntity
//{
//    public float Value;
//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        dstManager.AddComponent<Velocity>(entity);
//        dstManager.SetComponentData(entity,new Velocity(){ Value=Value});
//    }
//}

[GenerateAuthoringComponent]
struct Velocity:IComponentData
{
    public float Value;
}

class ApplyVelocitySystem : JobComponentSystem
{
    //struct ApplyVelocityJob : IJobForEach<Translation, Velocity>
    //{
    //    public float deltaTime;
    //    public void Execute(ref Translation translation, ref Velocity velocity)
    //    {
    //        translation.Value += velocity.Value * deltaTime;
    //    }
    //}

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //var job = new ApplyVelocityJob();
        //job.deltaTime = Time.DeltaTime;
        //return job.Schedule(this, inputDeps);

        float deltaTime = Time.DeltaTime;
        return Entities.ForEach((ref Translation translation, in Velocity velocity) =>
        {
            translation.Value += velocity.Value * deltaTime;
        }).Schedule(inputDeps);
    }
}
