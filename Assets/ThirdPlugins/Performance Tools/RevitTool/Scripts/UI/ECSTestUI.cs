using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Mathematics;
using System.Linq;
using Unity.Transforms;
using UnityEngine.UIElements;
using Unity.Rendering;
using Unity.Collections;
using Random = UnityEngine.Random;
using System;
using Unity.Entities.UniversalDelegates;
// using Mogoson.CameraExtension;

[RequireComponent(typeof(SpawnerPrefabScript))]
public class ECSTestUI : MonoBehaviour
{

    public InputField inputCountInfo;
    public InputField inputInternalInfo;

    public SpawnerPrefabScript spawn;

    void Awake()
    {
        spawn = this.GetComponent<SpawnerPrefabScript>();
    }

    [ContextMenu("CreateGameObjects")]
    public void CreateGameObjects()
    {
        cameraCenter.position = GetCenterPos();
        var posList = GetPosList();
        spawn.CreateGameObjects(posList,null);
    }

    public void CreateEntities()
    {
        cameraCenter.position = GetCenterPos();
        var posList = GetPosList();
        spawn.CreateEntities(posList, null);
    }

    public void CreateEntities2()
    {
        cameraCenter.position = GetCenterPos();
        var posList = GetPosList();
        spawn.CreateEntitiesByRenderMesh(posList, null);
    }


    public Transform cameraCenter;

    private List<float3> GetPosList()
    {
        int3 count3 = inputCountInfo.text.ToInt3();
        float3 internal3 = inputInternalInfo.text.ToFloat3();
        var posList = PosUtils.GetPositins(count3, internal3);
        return posList;
    }

    private float3 GetCenterPos()
    {
        int3 count3 = inputCountInfo.text.ToInt3();
        float3 internal3 = inputInternalInfo.text.ToFloat3();
        float3 center = float3.zero;
        center.x = count3.x / 2 * internal3.x;
        center.y = count3.y / 2 * internal3.y;
        center.z = count3.z / 2 * internal3.z;
        return center;
    }
}


