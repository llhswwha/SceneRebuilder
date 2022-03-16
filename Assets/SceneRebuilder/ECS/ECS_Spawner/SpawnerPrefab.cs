#if UNITY_ECS
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnerPrefab : IComponentData
{
    public Entity Prefab;
    //public float3 Pos;
    public float3[] PosList;
    //public NativeArray<quaternion> Angles;
    //public NativeArray<float3> Scales;
}
#endif