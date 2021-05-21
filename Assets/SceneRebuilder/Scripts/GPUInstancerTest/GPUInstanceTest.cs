using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstanceTest : MonoBehaviour
{
    public List<GameObject> PrefabList = new List<GameObject>();

    public GPUInstancerPrefabManager prefabManager;

    //public AstroidGenerator generator;

    [ContextMenu("InitPrefabs")]
    public void InitPrefabs()
    {
        prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();
        prefabManager.InitPrefabs(PrefabList);

        // //generator = GameObject.FindObjectOfType<AstroidGenerator>();
        // //generator.asteroidObjects.Clear();
        // prefabManager.ClearPrefabList();
        // for (int i = 0; i < PrefabList.Count; i++)
        // {
        //     float progress = (float)i / PrefabList.Count;
        //     float percents = progress * 100;

        //     if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{PrefabList.Count} {percents}% of 100%", progress))
        //     {
        //         break;
        //     }
        //     GameObject item = PrefabList[i];
        //     GPUInstancerPrefab prefab = item.GetComponent<GPUInstancerPrefab>();
        //     if (prefab == null)
        //     {
        //         prefab=GPUInstancerUtility.AddComponentToPrefab<GPUInstancerPrefab>(item);
        //     }

        //     //prefabManager.prototypeList.Add(prefab);

        //     prefabManager.DefineGameObjectAsPrefabPrototype(item);

        //     // prefabManager.prefabList.Add(item);
        //     // var prototype=GPUInstancerUtility.GeneratePrefabPrototype(item, false);
        //     // Debug.Log($"1 item:{item},prototype:{prototype},count:{prefabManager.prototypeList.Count}");
        //     // prefabManager.prototypeList.Add(prototype);
        //     // Debug.Log($"2 item:{item},prototype:{prototype},count:{prefabManager.prototypeList.Count}");

        //     //generator.asteroidObjects.Add(prefab);
        // }

        // ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("GetPrefabsInScene")]
    public void GetPrefabsInScene()
    {
        GPUInstancerPrefab[] prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();
        Debug.Log("prefabs:"+ prefabs.Length);

        AutomaticLOD[] lods = GameObject.FindObjectsOfType<AutomaticLOD>();
        Debug.Log("lods:" + lods.Length);
    }

    public bool IsUseGPU = false;

    private void Start()
    {
        if (IsUseGPU)
        {
            GPUInstancerPrefab[] prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();
            prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();
            if (prefabManager != null && prefabManager.gameObject.activeSelf && prefabManager.enabled)
            {
                GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, prefabs);
                GPUInstancerAPI.InitializeGPUInstancer(prefabManager);
            }
        }
        
    }

    [ContextMenu("ShowPrototypeList")]
    public void ShowPrototypeList()
    {
        prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();

        Debug.Log($"count:{prefabManager.prototypeList.Count}");
    }
}
