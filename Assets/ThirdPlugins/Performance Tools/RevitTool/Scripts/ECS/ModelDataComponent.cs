#if UNITY_ECS
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ModelDataComponent : IComponentData
{
    public int Id;
    // public NativeString64 Name;

    // public override string ToString()
    // {
    //     return string.Format("Id:{0},Name:{1}", Id, Name);
    // }
}
#endif