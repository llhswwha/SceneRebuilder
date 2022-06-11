using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct SceneDistanceJob : IJob
{
    public NativeArray<Vector3> camPosList;
    public NativeArray<Vector3> camForwardList;

    public int sceneId;

    public Bounds sceneBounds;

    public Vector3 scenePos;


    public float disToCam;

    public float angleToCam;

    public void Execute()
    {
        //Debug.Log($"SceneDistanceJob[{sceneId}]");
    }
}
