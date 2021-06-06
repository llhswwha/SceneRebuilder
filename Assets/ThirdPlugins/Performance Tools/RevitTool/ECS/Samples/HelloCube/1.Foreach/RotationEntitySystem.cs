#if UNITY_ECS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class RotationEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithName("RotationEntitySystem22").ForEach((ref Rotation rotation, in RotationSpeedComponent speed) => {
            var normalize = math.normalize(rotation.Value);
            var angle = quaternion.AxisAngle(math.up(), speed.RadiansPerSecond * deltaTime);
            rotation.Value = math.mul(normalize, angle);
        }).ScheduleParallel();
    }
}
#endif
