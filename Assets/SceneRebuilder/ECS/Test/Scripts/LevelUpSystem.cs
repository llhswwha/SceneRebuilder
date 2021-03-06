#if UNITY_ECS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public class LevelUpSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref LevelComponent levelComponent) =>
        {
            levelComponent.Level += 1f * Time.DeltaTime;
        });
    }
}

public struct LevelComponent : IComponentData
{
    public float Level;
}

#endif