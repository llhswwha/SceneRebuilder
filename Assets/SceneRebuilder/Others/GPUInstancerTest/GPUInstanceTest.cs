using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstanceTest : SingletonBehaviour<GPUInstanceTest>
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
        Debug.Log($"GetPrefabsInScene123 prefabs:{prefabs.Length}");

        AutomaticLOD[] lods = GameObject.FindObjectsOfType<AutomaticLOD>();
        Debug.Log("GetPrefabsInScene123 lods:" + lods.Length);
    }

    public bool IsUseGPU = false;

    public int Count = 200;

    public float PositionPower = 2;

    public float ScalePower = 0.5f;

    private List<GPUInstancerPrefab> CreatePrefabs()
    {
        List<GPUInstancerPrefab> prefabs0 = new List<GPUInstancerPrefab>();
        foreach (var obj in PrefabList)
        {
            GPUInstancerPrefab prefab = obj.GetComponent<GPUInstancerPrefab>();
            if (prefab != null)
            {
                prefabs0.Add(prefab);
            }
        }

        foreach (var pref in prefabs0)
        {
            GameObject prefGo = new GameObject(pref.name);
            prefGo.transform.SetParent(this.transform);
            prefGo.transform.localPosition = Vector3.zero;
            for (int i = 0; i < Count; i++)
            {

                GPUInstancerPrefab instancer = GameObject.Instantiate(pref);
                instancer.transform.position = Random.insideUnitSphere * PositionPower + pref.transform.position;
                instancer.transform.localScale = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * ScalePower;
                instancer.transform.rotation = Random.rotationUniform;
                instancer.transform.SetParent(prefGo.transform);
            }
        }
        return prefabs0;
    }

    public void RemovePrefabInstance(GameObject go)
    {
        RemovePrefabInstance(go.GetComponent<GPUInstancerPrefab>());
    }

    public void AddPrefabInstance(GameObject go)
    {
        AddPrefabInstance(go.GetComponent<GPUInstancerPrefab>());
    }

    public void RemovePrefabInstance(GPUInstancerPrefab prefabInstance)
    {
        if (prefabInstance == null) return;
        GPUInstancerAPI.RemovePrefabInstance(prefabManager, prefabInstance);
    }

    public void AddPrefabInstance(GPUInstancerPrefab prefabInstance)
    {
        if (prefabInstance == null) return;
        GPUInstancerAPI.AddPrefabInstance(prefabManager, prefabInstance);
    }

    public bool IsInitPrefabs = true;

    public bool IsCreatePrefabs = true;

    public bool IsAstroidGenerator = true;

    public GameObject MeshTarget = null;

    public GameObject PrefabsRoot = null;

    public bool ClearOldPrefabs = true;

    public int MinPrefabInstanceCount = 100;

    [ContextMenu("GetPrefabMeshes")]
    public void GetPrefabMeshes()
    {
        if (PrefabsRoot == null)
        {
            PrefabsRoot = new GameObject("PrefabsRoot");
        }

        if (ClearOldPrefabs)
        {
            foreach (var prefab in PrefabList)
            {
                GameObject.DestroyImmediate(prefab);
            }
        }
        
        PrefabList.Clear();
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(MeshTarget, true);
        foreach(SharedMeshInfo sm in sharedMeshInfos)
        {
            if (sm.GetCount() < MinPrefabInstanceCount)
            {
                sm.SetActive(false);
                continue;
            }

            foreach(var mf in sm.meshFilters)
            {
                
            }

            GameObject go = MeshHelper.CopyMeshObject(sm.gameObject,$"{sm.mainMeshFilter.sharedMesh.name}({sm.GetCount()})");
            go.transform.SetParent(null);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            PrefabList.Add(go);

            go.transform.SetParent(PrefabsRoot.transform);
        }
    }

    private void Start()
    {
        if (IsInitPrefabs)
        {
            InitPrefabs(); //11
        }
        List<GPUInstancerPrefab> prefabs0 = new List<GPUInstancerPrefab>();
        if (IsCreatePrefabs)
        {
            prefabs0=CreatePrefabs();
        }

        if (IsUseGPU)
        {
            InitializeGPUInstancer();
        }

        if (IsAstroidGenerator)
        {
            TestAstroidGenerator(prefabs0);
        }

        if (IsPrefabTarget)
        {
            PrefabInstances();
        }
    }

    private void TestAstroidGenerator(List<GPUInstancerPrefab> prefabs0)
    {
        AstroidGenerator.Instance.IsUseGPUOnStart = false;
        AstroidGenerator.Instance.asteroidObjects = prefabs0;
        AstroidGenerator.Instance.GenerateGos();
        AstroidGenerator.Instance.StartGPUInstance();
    }

    private void InitializeGPUInstancer()
    {
        GPUInstancerPrefab[] allPrefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();
        Debug.Log($"allPrefabs:{allPrefabs.Length}");
        if (prefabManager == null)
        {
            prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();
        }
        if (prefabManager != null && prefabManager.gameObject.activeSelf && prefabManager.enabled)
        {
            GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, allPrefabs);
            GPUInstancerAPI.InitializeGPUInstancer(prefabManager);
            Debug.Log($"InitializeGPUInstancer allPrefabs:{allPrefabs.Length}");
        }
    }

    public void PrefabInstances()
    {
        Debug.LogError($"PrefabInstances1 MeshTarget:{MeshTarget}");

        if (MeshTarget == null)
        {
            Debug.LogError($"PrefabInstances MeshTarget == null");
        }
        else
        {
            Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = new Dictionary<Mesh, GPUInstancerPrefab>();
            for (int i = 0; i < PrefabList.Count; i++)
            {
                GameObject prefab = (GameObject)PrefabList[i];
                MeshFilter mf = prefab.GetComponent<MeshFilter>();
                if (mf == null)
                {
                    Debug.LogError($"PrefabInstances[{i+1}] MeshFilter == null");
                    continue;
                }
                GPUInstancerPrefab gp= prefab.GetComponent<GPUInstancerPrefab>();
                if (gp == null) 
                {
                    Debug.LogError($"PrefabInstances[{i + 1}] GPUInstancerPrefab == null");
                    continue;
                }
                meshPrefabDict.Add(mf.sharedMesh, gp);
            }

            List<GPUInstancerPrefab> instances = new List<GPUInstancerPrefab>();
            SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(MeshTarget, true);
            Debug.LogError($"PrefabInstances sharedMeshInfos:{sharedMeshInfos.Count}");
            for (int i = 0; i < sharedMeshInfos.Count; i++)
            {
                SharedMeshInfo sm = sharedMeshInfos[i];
                if (sm.GetCount() < MinPrefabInstanceCount)
                {
                    sm.SetActive(false);
                    continue;
                }

                Mesh mesh = sm.mainMeshFilter.sharedMesh;
                if (meshPrefabDict.ContainsKey(mesh) == false)
                {
                    Debug.LogError($"PrefabInstances meshPrefabDict.ContainsKey(mesh) == false mesh:{mesh}");
                    continue;
                }
                GPUInstancerPrefab prefab = meshPrefabDict[mesh];
                //GPUInstancerPrefab prefab = sm.gameObject.GetComponent<GPUInstancerPrefab>();
                //if (prefab == null)
                //{
                //    Debug.LogError($"PrefabInstances prefab == null go:{sm.gameObject}");
                //    continue;
                //}
                if (prefab.prefabPrototype == null)
                {
                    prefab.GeneratePrototype();
                }

                foreach (var mf in sm.meshFilters)
                {
                    GPUInstancerPrefab gpuPrefab = mf.gameObject.AddMissingComponent<GPUInstancerPrefab>();
                    gpuPrefab.prefabPrototype = prefab.prefabPrototype;
                    instances.Add(gpuPrefab);

                    //gpuPrefab.AddVariation(bufferName, (Vector4)Random.ColorHSV());
                    //if (gpuColorMat)
                    //{
                    //    var mr = mf.GetComponent<MeshRenderer>();
                    //    mr.sharedMaterial = gpuColorMat;
                    //}
                }

                Debug.LogError($"PrefabInstances sharedMeshInfos[{i + 1}/{sharedMeshInfos.Count}] sm:{sm} prefab:{prefab} meshFilters:{sm.meshFilters.Count}");
            }

            AstroidGenerator.Instance.IsUseGPUOnStart = false;
            //AstroidGenerator.Instance.asteroidObjects = prefabs0;
            //AstroidGenerator.Instance.GenerateGos();
            AstroidGenerator.Instance.StartGPUInstance(instances);
        }
    }

    public Material gpuColorMat = null;

    public string bufferName = "gpuiFloat4Variation";

    public bool IsPrefabTarget = true;

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
