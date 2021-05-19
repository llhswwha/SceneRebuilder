using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct RotationSpeedComponent : IComponentData
{
    public float RadiansPerSecond;
}
