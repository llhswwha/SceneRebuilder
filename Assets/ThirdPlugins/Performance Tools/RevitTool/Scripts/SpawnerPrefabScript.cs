using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerPrefabScript : MonoBehaviour
{
    public static SpawnerPrefabScript Instance;

    void Awake()//AddComponent时就进入了
    {
        
    }

    void Start()
    {
        blobAssetStore = new BlobAssetStore();

        SceneRebuilder rebuilder = SceneRebuilder.Instance;
        rebuilder.currentCount = 0;
        ConvertToEntity();
        print("SpawnerPrefabScript.start");
        print("Prefab:" + Prefab);
        print("AutoCreate:" + AutoCreate);
        if (AutoCreate)
        {
            int count = 0;
            List<float3> posList = new List<float3>();
            List<quaternion> rotations = new List<quaternion>();
            foreach (var node in nodes)
            {
                rebuilder.currentCount++;
                if (rebuilder.ModelMaxCount > 0 && rebuilder.currentCount > rebuilder.ModelMaxCount)
                {
                    break;
                }
                posList.Add(new float3(node.GetPos()));
                rotations.Add(node.GetRotation());
            }

            if (CreateByEntityMesh == false)
            {
                //SpawnCubeEntity spawn = new SpawnCubeEntity();
                //spawn.InstantiateCubes(entityPrefab, posList, rotations);
                //creater = spawn;

                CreateEntities(posList, rotations);
            }
            else
            {
                CreateEntitiesByRenderMesh(posList, rotations);
            }

            print("entityPrefab:" + entityPrefab);
            GameObject.Destroy(Prefab);

        }
    }

    public GameObject Prefab;
    public GameObject LODPrefab;

    public Toggle ToggleLOD;
    public List<NodeInfo> nodes;

    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;
    private Entity entityPrefab;


    private EntityMeshInfo meshInfo;

    [ContextMenu("CreateGameObjects")]
    public void CreateGameObjects(List<float3> posList,List<quaternion> rotations)
    {
        //var posList = GetPosList();
        //new SpawnCubeNormal().InstantiateCubes(posList, Prefab);
        GameObject pre = Prefab;
        if (LODPrefab != null && ToggleLOD.isOn)
        {
            pre = LODPrefab;
        }
        InstantiateCubes<GameObject, SpawnCubeNormal>(pre, posList, rotations);
    }

    public void CreateEntities(List<float3> posList, List<quaternion> rotations)
    {
        //var posList = GetPosList();
        //new SpawnCubeDOTS().InstantiateCubes(posList, EntityPrefab);
        Entity ePre = entityPrefab;
        if (LODPrefab != null && ToggleLOD.isOn)
        {
            ePre = ConvertToEntity(LODPrefab);
        }

        InstantiateCubes<Entity, SpawnCubeEntity>(ePre, posList, rotations);
    }

    public void CreateEntitiesByRenderMesh(List<float3> posList, List<quaternion> rotations)
    {
        //var posList = GetPosList();
        //new SpawnCubeDOTS().InstantiateCubes(posList, EntityPrefab);
        if (meshInfo == null)
        {
            meshInfo = GetEntityMeshInfo(Prefab);
        }
        
        InstantiateCubes<EntityMeshInfo, SpawnCubeRenderMesh>(meshInfo, posList, rotations);
    }

    private static EntityMeshInfo GetEntityMeshInfo(GameObject obj)
    {
        if (obj == null) return null;
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        EntityMeshInfo meshInfo = new EntityMeshInfo();
        meshInfo.mesh = meshFilter.sharedMesh;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        meshInfo.material = render.sharedMaterial;
        return meshInfo;
    }

    public void ConvertToEntity()
    {
        print("ConvertToEntity 1");
        if (Prefab == null)
        {
            Debug.LogError("Prefab == null");
            return;
        }
        //if (entityPrefab != null) return;
        //BlobAssetStore blobAssetStore = new BlobAssetStore();
        //entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityPrefab=ConvertToEntity(Prefab);
        //GameObject.Destroy(Prefab);


        //meshInfo.material = Prefab.GetComponent<Material>();
        print("ConvertToEntity 2");
    }

    //void Start

    private Entity ConvertToEntity(GameObject pre)
    {
       
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(pre, settings);
        //GameObjectConversionUtility.Convert(gameObjectWorld);
        //blobAssetStore.Dispose();
        return entity;
    }

    void OnDisable()
    {
        Debug.Log("OnDisable:" + this);
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
        
    } 

    void OnDestroy()
    {
        Debug.Log("OnDestroy:" + this);
        if (blobAssetStore != null)
        {
            blobAssetStore.Dispose();
            blobAssetStore = null;
        }
    }

    private List<T1> InstantiateCubes<T1, T2>(T1 prefab,List<float3> posList, List<quaternion> rotations) where T2 : IInstiantiatePrefab<T1>, new()
    {
        Clear();
        T2 t2 = new T2();
        creater = t2;
        //camera.SetTarget()
        return t2.InstantiateCubes(prefab,posList, rotations);
        //Directory.Exists()
    }

    public IDisposable creater = null;
    public bool AutoCreate = false;
    public bool CreateByEntityMesh = false;
    internal int MaxCount = 0;

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (creater != null)
        {
            creater.Dispose();
        }
    }
}

[Serializable]
public class EntityMeshInfo
{
    public Mesh mesh;
    public Material material;
}

public interface IInstiantiatePrefab<T> : IDisposable
{
    List<T> InstantiateCubes(T prefab,List<float3> positions,List<quaternion> rotations );

}

public class SpawnCubeEntity : IInstiantiatePrefab<Entity>
{
    EntityManager entityManager;
    List<Entity> list;

    public void Dispose()
    {
        foreach (var item in list)
        {
            entityManager.DestroyEntity(item);
        }
    }

    public List<Entity> InstantiateCubes(Entity prefab,List<float3> positions, List<quaternion> rotations)
    {
        if (prefab == null)
        {
            Debug.LogError("InstantiateCubes prefab == null");
            return new List<Entity>();
        }
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var result = positions.Select(pos => entityManager.Instantiate(prefab))
                            .Select((entity, index) =>
                            {
                                entityManager.SetComponentData(entity, new Translation { Value = positions[index] });
                                if (rotations != null)
                                {
                                    //entityManager.AddComponent(entity,typeof)
                                    entityManager.SetComponentData(entity, new Rotation { Value = rotations[index] });
                                }
                                return entity;
                            });
        list = new List<Entity>(result);

        entityManager.DestroyEntity(prefab);
        return list;
    }
}

public class SpawnCubeRenderMesh : IInstiantiatePrefab<EntityMeshInfo>
{
    EntityManager entityManager;
    List<Entity> list;
    public void Dispose()
    {
        foreach (var item in list)
        {
            entityManager.DestroyEntity(item);
        }
    }

    public List<EntityMeshInfo> InstantiateCubes(EntityMeshInfo info, List<float3> positions, List<quaternion> rotations )
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = entityManager.CreateArchetype(
            typeof(Translation)
            , typeof(RenderMesh)
            , typeof(LocalToWorld)
            , typeof(RenderBounds)
            , typeof(Rotation)
           );
        NativeArray<Entity> entities = new NativeArray<Entity>(positions.Count, Allocator.Temp);
        RenderMesh renderMesh = new RenderMesh()
        {
            mesh = info.mesh,
            material = info.material
        };
        list = new List<Entity>();
        for (int i = 0; i < positions.Count; i++)
        {
            float3 pos = positions[i];
            Entity entity = entityManager.CreateEntity(archetype);

            entityManager.SetComponentData(entity, new Translation() { Value = pos });
            if (rotations != null)
                entityManager.SetComponentData(entity, new Rotation { Value = rotations[i] });

            //entityManager.SetSharedComponentData(entity, renderMesh);
            entityManager.SetSharedComponentData(entity, new RenderMesh()
            {
                mesh = info.mesh,
                material = info.material
            });
            list.Add(entity);
        }
        entities.Dispose();

        return null;
    }
}

public class SpawnCubeNormal : IInstiantiatePrefab<GameObject>
{
    List<GameObject> list;
    public void Dispose()
    {
        foreach (var item in list)
        {
            GameObject.Destroy(item);
        }
    }

    public List<GameObject> InstantiateCubes(GameObject prefab,List<float3> positions, List<quaternion> rotations)
    {
        GameObject parent = new GameObject();
        var result = positions.Select(pos => {
            var newObj = GameObject.Instantiate(prefab, pos, Quaternion.identity);
            newObj.transform.parent = parent.transform;
            return newObj;
        }
        );
        list = new List<GameObject>(result);
        return list;
    }
}

public static class MathExtensions
{
    public static int3 ToInt3(this string txt)
    {
        string[] parts = txt.Split(',');
        int x = int.Parse(parts[0]);
        int y = int.Parse(parts[1]);
        int z = int.Parse(parts[2]);
        return new int3(x, y, z);
    }

    public static float3 ToFloat3(this string txt)
    {
        string[] parts = txt.Split(',');
        float x = float.Parse(parts[0]);
        float y = float.Parse(parts[1]);
        float z = float.Parse(parts[2]);
        return new float3(x, y, z);
    }
}

public static class PosUtils
{
    public static List<float3> GetPositins(int3 count, float3 internal3)
    {
        int amount = count.x * count.y * count.z;
        List<float3> results = new List<float3>(amount);
        float3 pos = float3.zero;
        for (int i = 1; i <= count.z; i++)
        {
            pos.y = 0;
            for (int j = 1; j <= count.y; j++)
            {
                pos.x = 0;
                for (int k = 1; k <= count.x; k++)
                {
                    results.Add(pos);
                    pos.x += internal3.x;
                }
                pos.y += internal3.y;
            }
            pos.z += internal3.z;
        }
        return results;
    }
}
