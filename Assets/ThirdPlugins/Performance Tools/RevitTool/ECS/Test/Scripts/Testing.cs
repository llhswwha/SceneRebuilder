using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ECS
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
public class Testing : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private int EntityCount = 10;
    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype=entityManager.CreateArchetype(
            typeof(LevelComponent)
            , typeof(Translation)
            , typeof(RenderMesh)
            , typeof(LocalToWorld)
            , typeof(RenderBounds) //->WorldRenderBounds,ChunkWorldRenderBounds
            , typeof(MoveSpeedComponent)
            );

        NativeArray<Entity> entities = new NativeArray<Entity>(EntityCount, Allocator.Temp);
        for (int i = 0; i < entities.Length; i++)
        {
            Entity entity = entityManager.CreateEntity(archetype);
            entityManager.SetComponentData(entity, new LevelComponent() { Level = Random.Range(10,20) });
            entityManager.SetComponentData(entity, new MoveSpeedComponent() { Speed = Random.Range(1f, 2f) });
            entityManager.SetComponentData(entity, new Translation() { 
                Value=new Unity.Mathematics.float3(Random.Range(-8f,8f),Random.Range(-5f,5f),Random.Range(-1f,1f)) 
            });
            entityManager.SetSharedComponentData(entity, new RenderMesh()
            {
                mesh=mesh,material=material
            });
        }
        entities.Dispose();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif