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
        if (prefabManager == null)
        {
            prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();
        }

        prefabManager.InitPrefabs(PrefabList);

        //generator = GameObject.FindObjectOfType<AstroidGenerator>();
        //generator.asteroidObjects.Clear();

        //prefabManager.ClearPrefabList();
        //for (int i = 0; i < PrefabList.Count; i++)
        //{
        //    float progress = (float)i / PrefabList.Count;
        //    float percents = progress * 100;

        //    if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{PrefabList.Count} {percents:F1}%", progress))
        //    {
        //        break;
        //    }
        //    GameObject item = PrefabList[i];
        //    GPUInstancerPrefab prefab = item.GetComponent<GPUInstancerPrefab>();
        //    if (prefab == null)
        //    {
        //        prefab = GPUInstancerUtility.AddComponentToPrefab<GPUInstancerPrefab>(item);
        //    }

        //    //prefabManager.prototypeList.Add(prefab);

        //    //prefabManager.AddPrefabGO(item);

        //    if (!prefabManager.prefabList.Contains(item))
        //    {
        //        prefabManager.prefabList.Add(item);
        //        prefabManager.GeneratePrototypes();
        //    }

        //    // prefabManager.prefabList.Add(item);
        //    // var prototype=GPUInstancerUtility.GeneratePrefabPrototype(item, false);
        //    // Debug.Log($"1 item:{item},prototype:{prototype},count:{prefabManager.prototypeList.Count}");
        //    // prefabManager.prototypeList.Add(prototype);
        //    // Debug.Log($"2 item:{item},prototype:{prototype},count:{prefabManager.prototypeList.Count}");

        //    //generator.asteroidObjects.Add(prefab);
        //}

        //ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("GetPrefabsInScene")]
    public void GetPrefabsInScene()
    {
        GPUInstancerPrefab[] prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();
        Debug.Log($"GetPrefabsInScene prefabs:{prefabs.Length}");

        AutomaticLOD[] lods = GameObject.FindObjectsOfType<AutomaticLOD>();
        Debug.Log("lods:" + lods.Length);
    }

    public bool IsUseGPU = false;

    public int Count = 200;

    public float PositionPower = 2;

    public float ScalePower = 0.5f;

    private void Start()
    {
        if (IsUseGPU)
        {
            //InitPrefabs();

            List<GPUInstancerPrefab> prefabs0 = new List<GPUInstancerPrefab>();
            foreach(var obj in PrefabList)
            {
                GPUInstancerPrefab prefab = obj.GetComponent<GPUInstancerPrefab>();
                if (prefab != null)
                {
                    prefabs0.Add(prefab);
                }
            }

            for (int i = 0; i < Count; i++)
            {
                foreach(var pref in prefabs0)
                {
                    GPUInstancerPrefab instancer = GameObject.Instantiate(pref);
                    instancer.transform.position = Random.insideUnitSphere* PositionPower + pref.transform.position;
                    instancer.transform.localScale= Random.insideUnitSphere* ScalePower;
                    instancer.transform.rotation = Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
                    instancer.transform.SetParent(this.transform);
                }
            }

            GPUInstancerPrefab[] allPrefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();
            if (prefabManager == null)
            {
                prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();
            }
            if (prefabManager != null && prefabManager.gameObject.activeSelf && prefabManager.enabled)
            {
                GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, allPrefabs);
                GPUInstancerAPI.InitializeGPUInstancer(prefabManager);
            }

            //AstroidGenerator.Instance.asteroidObjects = prefabs0;
            //AstroidGenerator.Instance.GenerateGos();
        }
        
    }

    [ContextMenu("ShowPrototypeList")]
    public void ShowPrototypeList()
    {
        prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();

        Debug.Log($"count:{prefabManager.prototypeList.Count}");
    }

    [ContextMenu("DisableManager")]
    public void DisableManager()
    {
        this.gameObject.SetActive(false);
    }

    [ContextMenu("EnableManager")]
    public void EnableManager()
    {
        this.gameObject.SetActive(true);
    }
}
