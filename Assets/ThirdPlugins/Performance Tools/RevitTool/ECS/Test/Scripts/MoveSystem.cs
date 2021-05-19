using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

//public class MoveSystem : ComponentSystem //2w个 FPS:30->[20-25]->[15-20]
//{
//    protected override void OnUpdate()
//    {
//        Entities.ForEach((ref Translation translaton, ref MoveSpeedComponent moveSpeed) =>
//        {
//            translaton.Value.y += moveSpeed.Speed * Time.DeltaTime;
//            if (translaton.Value.y > 5)
//            {
//                moveSpeed.Speed = -math.abs(moveSpeed.Speed);
//            }
//            if (translaton.Value.y < -5)
//            {
//                moveSpeed.Speed = math.abs(moveSpeed.Speed);
//            }
//        });
//    }
//}

//public class MoveSystem : JobComponentSystem //2w个 FPS:50->[40-45]->[30-35]
//{
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        float deltaTime = Time.DeltaTime;
//        return Entities.ForEach((ref Translation translaton, ref MoveSpeedComponent moveSpeed) =>
//        {
//            translaton.Value.y += moveSpeed.Speed * deltaTime;
//            if (translaton.Value.y > 5)
//            {
//                moveSpeed.Speed = -math.abs(moveSpeed.Speed);
//            }
//            if (translaton.Value.y < -5)
//            {
//                moveSpeed.Speed = math.abs(moveSpeed.Speed);
//            }
//        }).Schedule(inputDeps);
//    }
//}

public class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Translation translaton, ref MoveSpeedComponent moveSpeed) =>
        {
            //translaton.Value.y += moveSpeed.Speed * deltaTime;
            //if (translaton.Value.y > 5)
            //{ 
            //    moveSpeed.Speed = -math.abs(moveSpeed.Speed);
            //}
            //if (translaton.Value.y < -5)
            //{
            //    moveSpeed.Speed = math.abs(moveSpeed.Speed);
            //}
            //注释掉移动代码也一样，仅仅显示模型帧率也是25-35
        })
        //.Run();//25-35
        //.Schedule();//30-35
        .ScheduleParallel();//30-35
    }
}


public struct MoveSpeedComponent : IComponentData
{
    public float Speed;
}
