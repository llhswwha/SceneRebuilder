using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODManager : MonoBehaviour
{
    private static LODManager _instance;
    public static LODManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LODManager>();
            }
            return _instance;
        }
    }

    public Material[] LODMaterials;

    public float[] LODLevels = new float[] { 0.6f, 0.2f, 0.07f, 0.01f };

    public float[] lodVertexPercents = new float[] { 0.75f,0.5f,0.25f,0.1f};

    public bool isDestroy = true;
    public bool isSaveAsset = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateLOD(GameObject go)
    {
        AutomaticLODHelper.CreateLOD(go, LODMaterials, LODLevels, lodVertexPercents, isDestroy, isSaveAsset);
    }

    public void CreateLOD(GameObject go,Action<float> progressChanged)
    {
        AutomaticLODHelper.CreateLOD(go, LODMaterials, LODLevels, lodVertexPercents, isDestroy, isSaveAsset, progressChanged);
    }
}
