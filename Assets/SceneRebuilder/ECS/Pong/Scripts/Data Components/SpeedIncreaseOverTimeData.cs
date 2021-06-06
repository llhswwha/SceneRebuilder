#if UNITY_ECS
using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpeedIncreaseOverTimeData : IComponentData
{
	public float increasePerSecond;
}
#endif
