using CommonExtension;
using CommonUtils;
using GPUInstancer;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GPUInstanceTest : SingletonBehaviour<GPUInstanceTest>
{
    public static void SAddPrefabInstance(MeshRenderer lastRenderer)
    {
        if (Instance.IsStartGPUInstance)
        {
            lastRenderer.enabled = false;
            //lastRenderer.transform.localScale = lastScale;
            Instance.AddPrefabInstance(lastRenderer.gameObject);
        }
    }

    public static void SRemovePrefabInstance(GameObject go)
    {
        if (Instance.IsStartGPUInstance)
        {
            //hitRenderer.transform.localScale *= 1.01f;
            Instance.RemovePrefabInstance(go);
        }
    }

    public static void SetEnable(bool isEnable)
    {
        if (isEnable)
        {
            Instance.IsStartGPUInstance = false;
            Instance.gameObject.SetActive(true);
        }
        else
        {
            Instance.gameObject.SetActive(false);
        }
    }

    public List<GPUInstancerPrefab> PrefabList = new List<GPUInstancerPrefab>();

    [ContextMenu("SortPrefabList")]
    public void SortPrefabList()
    {
        PrefabList.Sort((a, b) => a.name.CompareTo(b.name));
    }

    public GPUInstancerPrefabManager prefabManager;



    //public AstroidGenerator generator;
#if UNITY_EDITOR
    [ContextMenu("0.OneKey")]
    public void OneKey()
    {
        System.DateTime start = System.DateTime.Now;
        ClearOldPrefabs();
        GetPrefabMeshes();
        SavePrefabs();
        InitPrefabs();
        ReplaceInstances();
        Debug.Log($"OneKey time:{System.DateTime.Now-start}"); 
    }

    [ContextMenu("1.ClearOldPrefabs")]
    private void ClearOldPrefabs()
    {
        for (int i = 0; i < PrefabList.Count; i++)
        {
            GPUInstancerPrefab prefab = PrefabList[i];
            if (prefab == null) continue;
            if(PrefabUtility.IsPartOfModelPrefab(prefab.gameObject))
            {
                Debug.Log($"ClearOldPrefabs[{i}] IsPartOfModelPrefab prefab:{prefab}");
            }
            GameObject.DestroyImmediate(prefab.gameObject);
        }
        prefabManager.ClearPrefabList();
        prefabManager.NewPrototypes();
        PrefabList.Clear();
    }

    [ContextMenu("2.GetPrefabMeshes")]
    public void GetPrefabMeshes()
    {
        if (PrefabsRoot == null)
        {
            PrefabsRoot = new GameObject("PrefabsRoot");
        }

        if (IsClearOldPrefabs)
        {
            ClearOldPrefabs();

            CreatePrefabList();
        }
        else
        {
            UpdatePrefabList();
        }
    }

    [ContextMenu("3.SavePrefabs")]
    public void SavePrefabs()
    {
        for (int i = 0; i < PrefabList.Count; i++)
        {
            var item = PrefabList[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("SavePrefab", i, PrefabList.Count))
            {
                break;
            }
            GameObject goNew = EditorSavePrefabPath(item.gameObject);
            GameObject.DestroyImmediate(item.gameObject);
            PrefabList[i] = goNew.GetComponent<GPUInstancerPrefab>();
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SavePrefabs PrefabList:{PrefabList.Count}");
    }

    [ContextMenu("3.DeleteOtherPrefabs")]
    public void DeleteOtherPrefabs()
    {
        List<Object> assets = new List<Object>();
        string[] assetsList=AssetDatabase.FindAssets("", new string[] { "Assets/ThirdPlugins/GPUInstancer/Prefabs" });
        for (int i = 0; i < assetsList.Length; i++)
        {
            string item = assetsList[i];
            string path = AssetDatabase.GUIDToAssetPath(item);
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            assets.Add(obj);
            Debug.Log($"[{i}]{item} | {path} | {obj}");
        }
        //List<Object> assets=AssetDatabase.LoadAllAssetsAtPath("Assets/ThirdPlugins/GPUInstancer/Prefabs/").ToList();
        int count1 = assets.Count;
        for (int i = 0; i < PrefabList.Count; i++)
        {
            var item = PrefabList[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("SavePrefab", i, PrefabList.Count))
            {
                break;
            }
            //GameObject goNew = EditorSavePrefabPath(item.gameObject);
            //GameObject.DestroyImmediate(item.gameObject);
            //PrefabList[i] = goNew.GetComponent<GPUInstancerPrefab>();
            if (assets.Contains(item.gameObject))
            {
                assets.Remove(item.gameObject);
            }
        }
        int count2 = assets.Count;
        for (int i = 0; i < assets.Count; i++)
        {
            Object asset = assets[i];
            var path = AssetDatabase.GetAssetPath(asset);
            bool b=AssetDatabase.DeleteAsset(path);
            Debug.Log($"delete[{i}] asset:{asset} path:{path} b:{b}");
        }
        AssetDatabase.Refresh();
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"DeleteOtherPrefabs PrefabList:{PrefabList.Count} count1:{count1} count2:{count2}");
    }

    [ContextMenu("5.ReplaceInstances")]
    public void ReplaceInstances()
    {
        Debug.Log("ReplaceInstances");
        MeshFilter[] mfs = MeshTarget.GetComponentsInChildren<MeshFilter>(true);
        var dict = GetPrefabMeshDict();
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            if (mf == null)
            {
                continue;
            }
            if (ProgressBarHelper.DisplayCancelableProgressBar("Replace", i, mfs.Length))
            {
                break;
            } 
  
            Mesh mesh = mf.sharedMesh;
            if (dict.ContainsKey(mesh))
            {
                BoundsBox bb = mf.GetComponent<BoundsBox>();
                if (bb)
                {
                    Debug.LogError($"ReplaceInstances Skip BoundsBox mf:{mf}");
                    continue;
                }
                GPUInstancerPrefab pre = dict[mesh];
                GPUInstancerPrefab ins = (GPUInstancerPrefab)PrefabUtility.InstantiatePrefab(pre);
                ins.name = mf.name;
                GameObjectExtension.CopyTransfrom(mf.transform, ins.transform);
                GameObject.DestroyImmediate(mf.gameObject); 
            }
        }
        ProgressBarHelper.ClearProgressBar();
    }

    public GameObject EditorSavePrefabPath(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("SavePrefab go == null");
            return null;
        }
        //string prefabName= $"{go.name}[{go.GetInstanceID()}]";
        string prefabName = $"{go.name}";
        prefabName = prefabName.Replace("/", "=");
        EditorHelper.UnpackPrefab(go);
        string prefabPath = $"Assets/ThirdPlugins/GPUInstancer/Prefabs/{prefabName}.prefab";
        EditorHelper.makeParentDirExist(prefabPath);
        GameObject assetObj = PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.UserAction);
        Debug.Log($"SavePrefab go:{go.name} asset:{assetObj} path:{prefabPath}");
        return assetObj;
        //return prefabPath;
    }
#endif

    [ContextMenu("4.InitPrefabs")]
    public void InitPrefabs()
    {
        if (prefabManager == null)
        {
            prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>(true);
        }

        prefabManager.InitPrefabs(PrefabList, IsClearPrefabType);

        if (astroidGenerator == null)
        {
            astroidGenerator = GameObject.FindObjectOfType<AstroidGenerator>(true);
        }
        astroidGenerator.asteroidObjects = PrefabList;

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

    [ContextMenu("*.GetPrefabListInfo10-300")]
    private void GetPrefabListInfo10_300()
    {
        StringBuilder sb = new StringBuilder();
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(MeshTarget, true);
        for (int i = 2; i < 10; i += 2)
        {
            MinPrefabInstanceCount = i;
            sb.AppendLine(GetPrefabListInfoInner(i, sharedMeshInfos));
        }
        for (int i = 10; i <= 300; i += 10) 
        {
            MinPrefabInstanceCount = i;
            sb.AppendLine(GetPrefabListInfoInner(i, sharedMeshInfos));
        }
        Debug.Log(sb.ToString());
    }

    [ContextMenu("*.GetPrefabListInfo")]
    private void GetPrefabListInfo()
    {
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(MeshTarget, true);
        GetPrefabListInfoInner(MinPrefabInstanceCount, sharedMeshInfos);
    }

    private void UpdatePrefabList()
    {
        //PrefabList.Clear();
        var meshDict = GetPrefabMeshDict();
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(MeshTarget, true); 
        sharedMeshInfos.Sort();
        int totalCount = 0;
        int intanceCount = 0;
        int otherCount = 0;
        for (int i = 0; i < sharedMeshInfos.Count; i++)
        {
            SharedMeshInfo sm = sharedMeshInfos[i];
            int meshCount = sm.GetCount();
            totalCount += meshCount;
            if (meshCount < MinPrefabInstanceCount)
            {
                otherCount += meshCount;
                if (IsHideNotInPrefabs)
                {
                    sm.SetActive(false);
                }

                if (meshDict.ContainsKey(sm.mesh))
                {
                    var prefabGo = meshDict[sm.mesh];
                    PrefabList.Remove(prefabGo);
                }
                //Debug.Log($"UpdatePrefabList meshCount < MinPrefabInstanceCount meshCount:{meshCount} MinPrefabInstanceCount:{MinPrefabInstanceCount} sm:{sm.mesh.name}");
                continue;
            }
            intanceCount += meshCount;
            if (meshDict.ContainsKey(sm.mesh))
            {
                continue;
            }

            

            sm.mainMeshFilter.sharedMesh.name = sm.mainMeshFilter.sharedMesh.name.Replace("/","=");
            GameObject go = GameObjectExtension.CopyMeshObject(sm.gameObject, $"{sm.mainMeshFilter.sharedMesh.name}({sm.GetCount()})");
            go.transform.SetParent(null);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            GPUInstancerPrefab pre = go.AddMissingComponent<GPUInstancerPrefab>();
            PrefabList.Add(pre);

            go.transform.SetParent(PrefabsRoot.transform);
        }

        Debug.Log($"UpdatePrefabList MinCount:{MinPrefabInstanceCount}=> Prefabs:{PrefabList.Count} [{intanceCount}({(float)intanceCount / (float)totalCount:P1})+{otherCount}={totalCount}] sharedMeshs:{sharedMeshInfos.Count} Root:{PrefabsRoot} ");
    }

    private void CreatePrefabList()
    {
        PrefabList.Clear();
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(MeshTarget, true);
        int totalCount = 0;
        int intanceCount = 0;
        int otherCount = 0;
        foreach (SharedMeshInfo sm in sharedMeshInfos)
        {
            int meshCount = sm.GetCount();
            totalCount += meshCount;
            if (meshCount < MinPrefabInstanceCount)
            {
                otherCount += meshCount;
                if (IsHideNotInPrefabs)
                {
                    sm.SetActive(false);
                }
                continue;
            }

            intanceCount += meshCount;

            sm.mainMeshFilter.sharedMesh.name = sm.mainMeshFilter.sharedMesh.name.Replace("/", "=");
            GameObject go = GameObjectExtension.CopyMeshObject(sm.gameObject, $"{sm.mainMeshFilter.sharedMesh.name}({sm.GetCount()})");
            go.transform.SetParent(null);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            GPUInstancerPrefab pre = go.AddMissingComponent<GPUInstancerPrefab>();
            PrefabList.Add(pre);

            go.transform.SetParent(PrefabsRoot.transform);
        }

        Debug.Log($"CreatePrefabList MinCount:{MinPrefabInstanceCount}=> Prefabs:{PrefabList.Count} [{intanceCount}({(float)intanceCount / (float)totalCount:P1})+{otherCount}={totalCount}] sharedMeshs:{sharedMeshInfos.Count}  Root:{PrefabsRoot} ");
    }

    public bool IsClearPrefabType = false;

    public AstroidGenerator astroidGenerator;


    private string GetPrefabListInfoInner(int minCount,SharedMeshInfoList sharedMeshInfos)
    {
        //PrefabList.Clear();
        int totalCount = 0;
        int intanceCount = 0;
        int otherCount = 0;
        int prefabCount = 0;


        int totalVertex= 0;
        int intanceVertex = 0;
        int otherVertex = 0;
        foreach (SharedMeshInfo sm in sharedMeshInfos)
        {
            int meshCount = sm.GetCount();
            int meshVertex = sm.GetAllVertexCount();
            totalCount += meshCount;
            totalVertex += meshVertex;
            if (meshCount < minCount)
            {
                otherCount += meshCount;
                otherVertex += meshVertex;
                //if (IsHideNotInPrefabs)
                //{
                //    sm.SetActive(false);
                //}
                continue;
            }
            intanceVertex += meshVertex;
            intanceCount += meshCount;
            prefabCount++;

            //sm.mainMeshFilter.sharedMesh.name = sm.mainMeshFilter.sharedMesh.name.Replace("/", "=");
            //GameObject go = GameObjectExtension.CopyMeshObject(sm.gameObject, $"{sm.mainMeshFilter.sharedMesh.name}({sm.GetCount()})");
            //go.transform.SetParent(null);
            //go.transform.position = Vector3.zero;
            //go.transform.rotation = Quaternion.identity;
            //go.transform.localScale = Vector3.one;

            //PrefabList.Add(go);

            //go.transform.SetParent(PrefabsRoot.transform);
        }

        Debug.Log($"GetPrefabListInfo MinCount:{minCount}=> Prefabs:{prefabCount} [{intanceCount}({(float)intanceCount / (float)totalCount:P1})+{otherCount}={totalCount}] [{intanceVertex}({(float)intanceVertex / (float)totalVertex:P1})+{otherVertex}={totalVertex}] sharedMeshs:{sharedMeshInfos.Count} Root:{PrefabsRoot} ");
        return $"{minCount}\t{prefabCount}\t{intanceCount}\t{otherCount}\t{totalCount}\t{(float)intanceCount / (float)totalCount:P1}\t{intanceVertex}\t{otherVertex}\t{totalVertex}\t{(float)intanceVertex / (float)totalVertex:P1}";
    }


    [ContextMenu("GetPrefabsInScene")]
    public void GetPrefabsInScene()
    {
        GPUInstancerPrefab[] prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>();
        Debug.Log($"GetPrefabsInScene123 prefabs:{prefabs.Length}");

        AutomaticLOD[] lods = GameObject.FindObjectsOfType<AutomaticLOD>();
        Debug.Log("GetPrefabsInScene123 lods:" + lods.Length);
    }

    public GameObject MeshTarget = null;

    public int MinPrefabInstanceCount = 100;

    public bool IsStartGPUInstance = true;

    public bool IsHideNotInPrefabs = false;

    public bool IsClearOldPrefabs = true;

    //-------------------------

    public bool IsAutoHideShowRoot = true;

    public float UpdatePrefabRootInterval = 0.1f;

    public int CoroutineSize = 1000;

    public List<GameObject> GPUIRoots = new List<GameObject>();

    private Dictionary<GameObject,bool> PrefabRootsState = new Dictionary<GameObject, bool>();

    private void UpdatePrefabRootsVisible()
    {
        foreach (var root in GPUIRoots)
        {
            if (root == null) continue;
            //if(!PrefabRootsState.ContainsKey(root))
            //{
            //     bool isActive = root.activeInHierarchy;
            //    PrefabRootsState.Add(root, isActive);
            //}
            //bool state = PrefabRootsState[root];
            SetPrefabRootActive(root);
        }
    }

    private bool GetIsAllInActive()
    {
        //bool allInActive = true;
        foreach(var root in GPUIRoots)
        {
            if (root == null) continue;
            if (root.activeInHierarchy == true)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator UpdatePrefabRootsVisible_Coroutine()
    {
        Debug.Log($"UpdatePrefabRootsVisible_Coroutine Start GPUIRoots:{GPUIRoots.Count}");
        int count = 0;

        
        while (IsAutoHideShowRoot)
        {
            System.DateTime start = System.DateTime.Now;
            if (GPUIRoots.Count == 0)
            {
                Debug.LogError("UpdatePrefabRootsVisible_Coroutine GPUIRoots.Count == 0");
                IsAutoHideShowRoot = false;
                break;
            }

            //bool isActive0 = MeshTarget.activeInHierarchy;
            bool isAllInActive = GetIsAllInActive();
            //if (isAllInActive)
            //{
            //    if (prefabManager.enabled == true)
            //    {
            //        prefabManager.enabled = false;
            //    }
            //}
            //else
            //{
            //    if (prefabManager.enabled == false)
            //    {
            //        prefabManager.enabled = true;
            //    }
            foreach (var root in GPUIRoots)
                {
                    if (root == null) continue;
                    yield return SetPrefabRootActive_Coroutine(root);
                }
            //}
            //Debug.Log($"UpdatePrefabRootsVisible_Coroutine[{count}] time:{System.DateTime.Now - start} GPUIRoots:{GPUIRoots.Count} isActive0:{isAllInActive}");

            count++;
            yield return new WaitForSeconds(UpdatePrefabRootInterval);
        }
    }

    private void OnDestroy()
    {
        IsAutoHideShowRoot = false;
    }

    private void OnDisable()
    {
        Debug.Log($"OnDisable:{this.name}");
        StopCoroutine(UpdatePrefabRootsVisible_Coroutine());
    }

    private void OnEnable()
    {
        
    }

    public bool IsUseGPU = false; 

    public int NewPrefabCount = 200;

    public float PositionPower = 2;

    public float ScalePower = 0.5f;

    public bool IsInitPrefabs = true;

    public bool IsCreatePrefabs = true;

    public bool IsAstroidGenerator = true;

    public GameObject PrefabsRoot = null;

    public Material gpuColorMat = null;

    public string bufferName = "gpuiFloat4Variation";

    private List<GPUInstancerPrefab> CreatePrefabs()
    {
        List<GPUInstancerPrefab> prefabs0 = new List<GPUInstancerPrefab>();
        foreach (var obj in PrefabList)
        {
            if (obj == null) continue;
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
            for (int i = 0; i < NewPrefabCount; i++)
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

    public void RemovePrefabsOfRoot(GameObject root)
    {
        RemovePrefabsOfRootInner(root);
    }

    public void RemovePrefabsOfRootInner(GameObject root)
    {
        GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
        for (int i = 0; i < prefabs.Length; i++)
        {
            GPUInstancerPrefab prefab = prefabs[i];
            RemovePrefabInstance(prefab);
        }
    }

    public void AddPrefabsOfRoot(GameObject root)
    {
        AddPrefabsOfRootInner(root);
    }

    private void SetPrefabRootActive(GameObject root)
    {
        bool isActiveNew = root.activeInHierarchy;
        if (!PrefabRootsState.ContainsKey(root))
        {
            PrefabRootsState.Add(root, isActiveNew);

            if (isActiveNew)
            {
                AddPrefabsOfRoot(root);
            }
            else
            {
                RemovePrefabsOfRoot(root);
            }
        }
        else
        {
            bool isActiveOld = PrefabRootsState[root];
            if (isActiveOld == isActiveNew) return;
            if (isActiveNew)
            {
                AddPrefabsOfRoot(root);
            }
            else
            {
                RemovePrefabsOfRoot(root);
            }
        }
    }

    private IEnumerator SetPrefabRootActive_Coroutine(GameObject root)
    {
        bool isActiveNew = root.activeInHierarchy;
        if (!PrefabRootsState.ContainsKey(root))
        {
            PrefabRootsState.Add(root, isActiveNew);
            //if (isActiveNew)
            //{
            //    yield return AddPrefabsOfRoot_Coroutine(root);
            //}
            //else
            //{
            //    yield return RemovePrefabsOfRoot_Coroutine(root);
            //}
        }
        else
        {
            bool isActiveOld = PrefabRootsState[root];
            if (isActiveOld != isActiveNew)
            {
                System.DateTime start = System.DateTime.Now;
                Debug.Log($"SetPrefabRootActive_Coroutine isActiveOld != isActiveNew:{root.name}");
                if (isActiveNew)
                {
                    yield return AddPrefabsOfRoot_Coroutine(root);
                }
                else
                {
                    yield return RemovePrefabsOfRoot_Coroutine(root);
                } 
                PrefabRootsState[root] = isActiveNew;

                Debug.LogWarning($"UpdatePrefabRootsVisible_Coroutine({root.name}) time:{System.DateTime.Now - start} GPUIRoots:{GPUIRoots.Count}");
            }
            else
            {
                //Debug.Log($"SetPrefabRootActive_Coroutine isActive Not Changed:{root.name}");
            }
        }
        //yield return null;
    }

    public void AddPrefabsOfRootInner(GameObject root)
    {
        GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
        for (int i = 0; i < prefabs.Length; i++)
        {
            GPUInstancerPrefab prefab = prefabs[i];
            AddPrefabInstance(prefab);
        }
    }

    public IEnumerator AddPrefabsOfRoot_Coroutine(GameObject root)
    {
        GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
        int j = 0;
        for (int i = 0; i < prefabs.Length; i++)
        {
            GPUInstancerPrefab prefab = prefabs[i];
            AddPrefabInstance(prefab);
            if (j > CoroutineSize)
            {
                j = 0;
                Debug.Log($"AddPrefabsOfRoot_Coroutine[{i}/{prefabs.Length}_{j}/{CoroutineSize}] {root.name} prefab:{prefab.name}");
                yield return null;
            }
            j++;
            //yield return null;
        }
    }

    public bool isShowLog = false;

    public IEnumerator RemovePrefabsOfRoot_Coroutine(GameObject root)
    {
        GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
        Debug.Log($"RemovePrefabsOfRoot_Coroutine {root.name} prefabs:{prefabs.Length}");
        int j = 0;
        for (int i = 0; i < prefabs.Length; i++)
        {
            GPUInstancerPrefab prefab = prefabs[i];
            RemovePrefabInstance(prefab);
            if (j > CoroutineSize)
            {
                j = 0;
                if (isShowLog)
                {
                    Debug.Log($"RemovePrefabsOfRoot_Coroutine[{i}/{prefabs.Length}_{j}/{CoroutineSize}] {root.name} prefab:{prefab.name}");
                }
                yield return null;
            } 
            j++;
            //Debug.Log($"RemovePrefabsOfRoot_Coroutine[{i}/{prefabs.Length}_{j}/{CoroutineSize}] {root.name} prefab:{prefab.name}");
            //yield return null;
        }
    }

    [ContextMenu("InitAstroidGenerator")]
    private void InitAstroidGenerator()
    {
        if (AstroidGenerator.Instance == null)
        {
            this.gameObject.AddComponent<AstroidGenerator>();
        }
        AstroidGenerator.Instance.IsUseGPUOnStart = false;
    }

    private void Update()
    {
        //if (IsAutoHideShowRoot)
        //{
        //    UpdatePrefabRootsVisible();
        //}
    }

    private void Start()
    {
        InitAstroidGenerator();
        if (AstroidGenerator.Instance == null)
        {
            Debug.LogError($"Start AstroidGenerator.Instance == null "); 
            return;
        }

        if (IsInitPrefabs)
        {
            InitPrefabs(); //11
        }

        List<GPUInstancerPrefab> prefabs0 = new List<GPUInstancerPrefab>();
        if (IsCreatePrefabs)
        {
            prefabs0=CreatePrefabs();
        }

        //if (IsUseGPU)
        //{
        //    InitializeGPUInstancer();
        //}

        //if (IsAstroidGenerator)
        //{
        //    TestAstroidGenerator(prefabs0);
        //}

        if (IsStartGPUInstance)
        {
            StartGPUInstance();
        }
        else if (IsUseGPU)
        {
            InitializeGPUInstancer();
        }
        else if (IsAstroidGenerator)
        {
            TestAstroidGenerator(prefabs0);
        }

        if (IsAutoHideShowRoot)
        {
            StartUpdatePrefabRootsVisible();
        }
        InitGPUISceneLoadFinishedAction();
    }

    private void InitGPUISceneLoadFinishedAction()
    {
        foreach(var obj in GPUIRoots)
        {
            if (obj == null) continue;
            SubScene_Single scene = obj.GetComponent<SubScene_Single>();
            if (scene != null)
            {
                scene.LoadFinished += Scene_LoadFinished;
            }
        }

        StartGPUInstance();
    }

    private void Scene_LoadFinished(SubScene_Base obj)
    {
        Debug.Log($"GPUInstanceTest.Scene_LoadFinished obj:{obj}");
        StartGPUInstance(obj.gameObject);
    }

    private void TestAstroidGenerator(List<GPUInstancerPrefab> prefabs0)
    {
        InitAstroidGenerator();
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

    private Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = new Dictionary<Mesh, GPUInstancerPrefab>();

    private Dictionary<Mesh, GPUInstancerPrefab> GetMeshPrefabDict()
    {
        //Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = new Dictionary<Mesh, GPUInstancerPrefab>();
        if(meshPrefabDict.Count==0 && PrefabList.Count > 0)
        {
            for (int i = 0; i < PrefabList.Count; i++)
            {
                var prefab = PrefabList[i];
                if (prefab == null)
                {
                    continue;
                }
                MeshFilter mf = prefab.GetComponent<MeshFilter>();
                if (mf == null)
                {
                    Debug.LogError($"PrefabInstances[{i + 1}] MeshFilter == null");
                    continue;
                }
                if (mf.sharedMesh == null)
                {
                    Debug.LogError($"PrefabInstances[{i + 1}] sharedMesh == null:{mf}");
                    continue;
                }
                GPUInstancerPrefab gp = prefab.GetComponent<GPUInstancerPrefab>();
                if (gp == null)
                {
                    Debug.LogError($"PrefabInstances[{i + 1}] GPUInstancerPrefab == null");
                    continue;
                }
                meshPrefabDict.Add(mf.sharedMesh, gp);
            }
        }
        
        return meshPrefabDict;
    }

    [ContextMenu("InitPrefabInstances")]
    public void InitPrefabInstances()
    {
        Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
        var instances = InitPrefabInstances(MeshTarget, meshPrefabDict);
    }

    [ContextMenu("ClearPrefabInstances")]
    public void ClearPrefabInstances()
    {
        GPUInstancerPrefab[] gpuis = MeshTarget.GetComponentsInChildren<GPUInstancerPrefab>(true);
        foreach(var gpui in gpuis)
        {
            GameObject.DestroyImmediate(gpui);
        }
        Debug.Log($"ClearPrefabInstances gpuis:{gpuis.Length}"); 
    }

    public GameObject MoveTargetRoot = null;

    [ContextMenu("MovePrefabInstances")]
    public void MovePrefabInstances()
    {
        if (MoveTargetRoot == null)
        {
            Debug.LogError("MovePrefabInstances MoveTargetRoot == null");
            return;
        }
        GPUInstancerPrefab[] gpuis = MoveTargetRoot.GetComponentsInChildren<GPUInstancerPrefab>(true);
        GameObject gpuRoot = MoveTargetRoot.FindOrCreateChild("GPUI");
        foreach (var gpui in gpuis)
        {
            if (gpui.transform.childCount > 0)
            {
                Debug.LogError($"MovePrefabInstances gpui.transform.childCount > 0 gpui:{gpui} childCount:{gpui.transform.childCount} path:{gpui.transform.GetPath()}");
                continue;
            }
            else
            {
                TransformExtension.MoveGameObject(gpui.transform, gpuRoot.transform, MoveTargetRoot.transform);
            }
        }

        foreach (var gpui in gpuis)
        {
            if (gpui.transform.childCount > 0)
            {
                TransformExtension.MoveGameObject(gpui.transform, gpuRoot.transform, MoveTargetRoot.transform);
            }
            else
            {
                
            }
        }

        Debug.Log($"MovePrefabInstances gpuis:{gpuis.Length} MoveTargetRoot:{MoveTargetRoot}");
    }

    private List<GPUInstancerPrefab> InitPrefabInstances(List<GameObject> targets, Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict)
    {
        return SInitPrefabInstances(targets, meshPrefabDict, MinPrefabInstanceCount, IsHideNotInPrefabs);
    }
    private List<GPUInstancerPrefab> InitPrefabInstances(GameObject target, Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict)
    {
        return SInitPrefabInstances(new List<GameObject>() { target }, meshPrefabDict, MinPrefabInstanceCount, IsHideNotInPrefabs);
    }

    private static List<GPUInstancerPrefab> SInitPrefabInstances(List<GameObject> targets, Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict, int minCount, bool isHide)
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        foreach (var target in targets)
        {
            if (target == null) continue;
            meshFilters.AddRange(target.GetComponentsInChildren<MeshFilter>(true));
        }
        return SInitPrefabInstances(meshFilters, meshPrefabDict, minCount, isHide);
    }

    private static List<GPUInstancerPrefab> SInitPrefabInstances(List<MeshFilter> meshFilters, Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict,int minCount,bool isHide)
    {
        List<GPUInstancerPrefab> instances = new List<GPUInstancerPrefab>();
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(meshFilters);
        Debug.Log($"PrefabInstances sharedMeshInfos:{sharedMeshInfos.Count}");
        for (int i = 0; i < sharedMeshInfos.Count; i++)
        {
            SharedMeshInfo sm = sharedMeshInfos[i];
            if (sm.GetCount() < minCount)
            {
                if (isHide)
                {
                    sm.SetActive(false);
                }
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

#if UNITY_EDITOR
            if (GPUInstancerPrototype.IsShowLog)
            {
                Debug.Log($"PrefabInstances sharedMeshInfos[{i + 1}/{sharedMeshInfos.Count}] sm:{sm} prefab:{prefab} meshFilters:{sm.meshFilters.Count}");
            }
#endif   
        }
        return instances;
    }

    public void StartGPUInstance()
    {
        Debug.Log($"StartGPUInstance MeshTarget:{MeshTarget}");

        //if (MeshTarget == null)
        //{
        //    Debug.LogError($"StartGPUInstance MeshTarget == null");
        //}
        //else
        //{
        //    StartGPUInstance(MeshTarget);
        //}

        //if (IsAutoHideShowRoot)
        //{
        //    StartUpdatePrefabRootsVisible();
        //}
    }

    [ContextMenu("StartGPUInstanceOfMeshTarget")]
    public void StartGPUInstanceOfMeshTarget()
    {
        Debug.Log($"StartGPUInstanceOfMeshTarget MeshTarget:{MeshTarget}");
        if (MeshTarget == null)
        {
            Debug.LogError($"StartGPUInstance MeshTarget == null");
        }
        else
        {
            StartGPUInstance(MeshTarget);
        }
    }

    [ContextMenu("StartGPUInstanceOfGPUIRoots")]
    public void StartGPUInstanceOfGPUIRoots()
    {
        Debug.Log($"StartGPUInstanceOfGPUIRoots MeshTarget:{GPUIRoots.Count}");
        StartGPUInstance(GPUIRoots);
    }

    [ContextMenu("StopGPUInstanceOfGPUIRoots")]
    public void StopGPUInstanceOfGPUIRoots()
    {
        Debug.Log($"StartGPUInstanceOfGPUIRoots MeshTarget:{GPUIRoots.Count}");
        StartCoroutine(StopGPUInstanceOfGPUIRoots_Coroutine());
    }

    private IEnumerator StopGPUInstanceOfGPUIRoots_Coroutine()
    {
        Debug.Log($"StopGPUInstanceOfGPUIRoots_Coroutine Start GPUIRoots:{GPUIRoots.Count}");
        int count = 0;
        foreach (var root in GPUIRoots)
        {
            if (root == null) continue;
            yield return RemovePrefabsOfRoot_Coroutine(root);
        }
    }

    private int gpuiCount = 0;

    private void StartGPUInstance(List<GameObject> targets)
    {
        System.DateTime start = System.DateTime.Now;
        Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
        var instances = InitPrefabInstances(targets, meshPrefabDict);

        AstroidGenerator.Instance.IsUseGPUOnStart = false;
        //AstroidGenerator.Instance.asteroidObjects = prefabs0;
        //AstroidGenerator.Instance.GenerateGos();
        //Debug.Log($"StartGPUInstance[{++gpuiCount}] target:{target} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} path:{target.transform.GetPath()}");
        AstroidGenerator.Instance.StartGPUInstance(instances);
        Debug.Log($"StartGPUInstance[{++gpuiCount}] targets:{targets.Count} time:{System.DateTime.Now - start} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} ");
    }

    private void StartGPUInstance(GameObject target)
    {
        System.DateTime start = System.DateTime.Now;
        Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
        var instances = InitPrefabInstances(target, meshPrefabDict);

        AstroidGenerator.Instance.IsUseGPUOnStart = false;
        //AstroidGenerator.Instance.asteroidObjects = prefabs0;
        //AstroidGenerator.Instance.GenerateGos();
        //Debug.Log($"StartGPUInstance[{++gpuiCount}] target:{target} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} path:{target.transform.GetPath()}");
        AstroidGenerator.Instance.StartGPUInstance(instances);
        Debug.Log($"StartGPUInstance[{++gpuiCount}] target:{target} time:{System.DateTime.Now-start} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} path:{target.transform.GetPath()}");
    }

    [ContextMenu("StartUpdatePrefabRootsVisible")]
    private void StartUpdatePrefabRootsVisible()
    {
        IsAutoHideShowRoot = true;
        StartCoroutine(UpdatePrefabRootsVisible_Coroutine());
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

    public List<Mesh> GetPrefabMeshList()
    {
        List<Mesh> meshes = new List<Mesh>();
        foreach(var prefab in PrefabList)
        {
            if (prefab == null) continue;
            MeshFilter mf = prefab.GetComponent<MeshFilter>();
            meshes.Add(mf.sharedMesh);
        }
        return meshes;
    }

    public Dictionary<Mesh, GPUInstancerPrefab> GetPrefabMeshDict()
    {
        Dictionary<Mesh, GPUInstancerPrefab> meshes = new Dictionary<Mesh, GPUInstancerPrefab>();
        foreach (var prefab in PrefabList)
        {
            if (prefab == null) continue;
            MeshFilter mf = prefab.GetComponent<MeshFilter>();
            if (mf == null) continue;
            if (mf.sharedMesh == null)
            {
                Debug.LogError($"mf.sharedMesh == null mf:{mf}");
                continue; 
            }
            meshes.Add(mf.sharedMesh, prefab);
        }
        return meshes;
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        MeshFilter[] mfs = MeshTarget.GetComponentsInChildren<MeshFilter>(true);
        foreach (var mf in mfs)
        {
            mf.gameObject.SetActive(true);
        }
        Debug.Log($"ShowAll mfs:{mfs.Length}");
    }

    [ContextMenu("HideNotInPrefabs")]
    public void HideNotInPrefabs()
    {
        var meshes = GetPrefabMeshDict();
        MeshFilter[] mfs = MeshTarget.GetComponentsInChildren<MeshFilter>(true);
        int hideCount = 0;
        int showCount = 0;
        foreach(var mf in mfs)
        {
            if (meshes.ContainsKey(mf.sharedMesh))
            {
                showCount++;
                mf.gameObject.SetActive(true);
            }
            else
            {
                hideCount++;
                mf.gameObject.SetActive(false);
            }
        }
        Debug.Log($"HideNotInPrefabs meshes:{meshes.Count} mfs:{mfs.Length} hideCount:{hideCount} showCount:{showCount}");
    }

    [ContextMenu("HideInPrefabs")]
    public void HideInPrefabs()
    {
        var meshes = GetPrefabMeshDict();
        MeshFilter[] mfs = MeshTarget.GetComponentsInChildren<MeshFilter>(true);
        int hideCount = 0;
        int showCount = 0;
        foreach (var mf in mfs)
        {
            if (!meshes.ContainsKey(mf.sharedMesh))
            {
                showCount++;
                mf.gameObject.SetActive(true);
            }
            else
            {
                hideCount++;
                mf.gameObject.SetActive(false);
            }
        }
        Debug.Log($"HideNotInPrefabs meshes:{meshes.Count} mfs:{mfs.Length} hideCount:{hideCount} showCount:{showCount}");
    }

}
