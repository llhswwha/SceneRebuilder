using CommonExtension;
using CommonUtils;
using GPUInstancer;
using StardardShader;
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
    public static bool IsGPUIEnabled
    {
        get
        {
            if(Instance == null || Instance.enabled == false || Instance.gameObject.activeInHierarchy == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [ContextMenu("GetIsGPUIEnabled")]
    public void GetIsGPUIEnabled()
    {
        Debug.LogError($"IsGPUIEnabled:{IsGPUIEnabled}");
    }

    public List<Material> MaterialLib = new List<Material>();

    public List<GameObject> Walls=new List<GameObject>();

#if UNITY_EDITOR
    //[ContextMenu("FindWalls")]
    public void FindWalls()
    {
        //Walls.Clear();
        Walls.RemoveAll(i=>i==null);
        Transform[] allObjs=GameObject.FindObjectsOfType<Transform>();
        for(int i=0;i<allObjs.Length;i++){
            Transform t=allObjs[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar("FindWalls",i,allObjs.Length,t)){
                break;
            }
            MeshRenderer mr=t.GetComponent<MeshRenderer>();
            if(mr)continue;
            if(t.name.ToLower().EndsWith("_wall")){
                if(!Walls.Contains(t.gameObject)){
                    Walls.Add(t.gameObject);
                }
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"FindWalls allObjs:{allObjs.Length} Walls:{Walls.Count}");
    }

    //[ContextMenu("FindWindows")]
    public void FindWindows()
    {
        Walls.RemoveAll(i=>i==null);
        Transform[] allObjs=GameObject.FindObjectsOfType<Transform>();
        for(int i=0;i<allObjs.Length;i++){
            Transform t=allObjs[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar("FindWindows",i,allObjs.Length,t)){
                break;
            }
            MeshRenderer mr=t.GetComponent<MeshRenderer>();
            if(mr)continue;
            if(t.name.ToLower().EndsWith("_窗户")){
                if(!Walls.Contains(t.gameObject)){
                    Walls.Add(t.gameObject);
                }
            }
            if(t.name.ToLower().EndsWith("_chaunghu")){
                if(!Walls.Contains(t.gameObject)){
                    Walls.Add(t.gameObject);
                }
            }
            if(t.name.ToLower().EndsWith("_chuanghu")){
                if(!Walls.Contains(t.gameObject)){
                    Walls.Add(t.gameObject);
                }
            }//chuanghu
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"FindWindows allObjs:{allObjs.Length} Walls:{Walls.Count}");
    }

    //[ContextMenu("SetdWallsGPUIRoot")]
    public void SetdWallsGPUIRoot()
    {
        Walls.RemoveAll(i=>i==null);
        foreach(var wall in Walls){
            wall.AddMissingComponent<GPUIRoot>();
        }
        Debug.LogError($"SetdWallsGPUIRoot Walls:{Walls.Count}");
    }

    //[ContextMenu("SaveToSetting")]
    public void SaveToSetting()
    {
        GPUIRootSettings.Clear();
        Walls.RemoveAll(i=>i==null);
        foreach(var wall in Walls){
            GPUIRootSettings.Add(wall.name);
        }
        Debug.LogError($"SaveToSetting GPUIRootSettings:{GPUIRootSettings.Count}");
    }

    //[ContextMenu("DeleteRootsAndLODs")]
    public void DeleteRootsAndLODs()
    {
        GetGPUIRoots();
        foreach(var root in GPUIRoots){
            if(root==null)continue;
            EditorHelper.UnpackPrefab(root.gameObject);
            GameObject.DestroyImmediate(root.gameObject);
        }

        LODGroup[] lodGroups =GameObject.FindObjectsOfType<LODGroup>(true);
        foreach(var group in lodGroups){
            if(group==null)continue;
            EditorHelper.UnpackPrefab(group.gameObject);
            GameObject.DestroyImmediate(group.gameObject);
        }
        Debug.Log($"DeleteRootsAndLODs GPUIRoots:{GPUIRoots.Count} lodGroups:{lodGroups.Length}");
    }

    //[ContextMenu("DeleteDoors")]
    public void DeleteDoors()
    {
        List<GameObject> doors=new List<GameObject>();
        Transform[] allObjs=GameObject.FindObjectsOfType<Transform>();
        for(int i=0;i<allObjs.Length;i++){
            Transform t=allObjs[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar("FindDoors",i,allObjs.Length,t)){
                break;
            }
            MeshRenderer mr=t.GetComponent<MeshRenderer>();
            if(mr)continue;
            if(t.name.ToLower().EndsWith("_doors")){
                if(!doors.Contains(t.gameObject)){
                    doors.Add(t.gameObject);
                }
            }
        }
        ProgressBarHelper.ClearProgressBar();
        foreach(var door in doors){
            EditorHelper.UnpackPrefab(door.gameObject);
            GameObject.DestroyImmediate(door);
        }
        Debug.LogError($"DeleteDoors allObjs:{allObjs.Length} Walls:{Walls.Count}");
    }

    #endif

    public void GetGPUIRoots()
    {
        GPUIRoot[] roots = GameObject.FindObjectsOfType<GPUIRoot>(true);
        GPUIRoots = roots.ToList();
    }

    public void ClearRoots()
    {
        GPUIRoot[] roots = GameObject.FindObjectsOfType<GPUIRoot>(true);
        foreach(var root in roots)
        {
            GameObject.DestroyImmediate(root);
        }
        GPUIRoots.Clear();
    }

    public List<string> GPUIRootSettings=new List<string>();

    public void SortSetting()
    {
        GPUIRootSettings.Sort();
    }

    //[ContextMenu("AddSetting")]
    public void LoadSetting()
    {
        for (int i = 0; i < GPUIRootSettings.Count; i++)
        {
            string setting = GPUIRootSettings[i];
            string clone = setting + "(Clone)";
            GameObject go1 = GameObject.Find(clone);
            if (go1 == null)
            {
                //Debug.LogError($"Not Found1 name:{clone}");
            }
            if (go1 != null)
            {
                // if (!TargetList.Contains(go1))
                // {
                //     TargetList.Add(go1);
                //     continue;
                // }
                go1.AddMissingComponent<GPUIRoot>();
                continue;
            }

            GameObject go2 = GameObject.Find(setting);
            if (go2 == null)
            {
                //Debug.LogError($"Not Found2 name:{setting}");
                //continue;
            }
            if (go2 != null)
            {
                // if (!TargetList.Contains(go2))
                // {
                //     TargetList.Add(go2);
                //     continue;
                // }
                go2.AddMissingComponent<GPUIRoot>();
                continue;
            }
            if(go1==null&& go2 == null)
            {
                Debug.LogError($"AddSetting[{i+1}/{GPUIRootSettings.Count}] Not Found name:{setting}");
            }
        }
        Debug.Log($"AddSetting SettingList:{GPUIRootSettings.Count}");
    }

    public bool IsCreatePrefabsWhenRun = false;

    public void CreatePrefabsOfRoots()
    {
        //List<MeshFilter> allMeshFilters = new List<MeshFilter>();
        //GPUIRoot[] roots = GameObject.FindObjectsOfType<GPUIRoot>(true);
        //for (int i = 0; i < roots.Length; i++)
        //{
        //    GPUIRoot root = roots[i];
        //    var prefabs = root.GetGPUIPrefabs();
        //    if (prefabs.Length == 0)
        //    {
        //        var mfs = root.GetComponentsInChildren<MeshFilter>(true);
        //        allMeshFilters.AddRange(mfs);
        //    }
        //}

        GetGPUIRoots();

        var allMeshFilters = GetMeshFilters();
        ReplaceInstances(allMeshFilters.ToArray());
        for (int i = 0; i < GPUIRoots.Count; i++)
        {
            GPUIRoot root = GPUIRoots[i];
            this.RegistGPUI(root);
        }
        ReplaceInstancesLODWhenRun();
        Debug.LogError($"CreatePrefabsOfRoots roots:{GPUIRoots.Count} allMeshFilters:{allMeshFilters.Length}");
    }

    //[ContextMenu("5.ReplaceInstancesLODWhenRun")]
    public void ReplaceInstancesLODWhenRun()
    {
        IdDictionary.InitInfos();
        int count = 0;
        int errorCount = 0;
        foreach (var obj in LODPrefabList)
        {
            foreach(var id in obj.InstanceIds)
            {
                GameObject prefabObj=IdDictionary.GetGo(id);
                if (prefabObj == null)
                {
                    Debug.LogError($"ReplaceInstancesLODWhenRun prefabObj == null id:{id} Prefab:{obj.Prefab}");
                    errorCount++;
                }
                else
                {
                    GPUInstancerPrefab prefab = prefabObj.GetComponent<GPUInstancerPrefab>();
                    if (prefab != null)
                    {
                        prefab.prefabPrototype = obj.Prefab.prefabPrototype;
                        count++;
                    }
                    else
                    {
                        Debug.LogError($"ReplaceInstancesLODWhenRun prefab == null id:{id} Prefab:{obj.Prefab}");
                        errorCount++;
                    }
                }
            }
        }
        Debug.Log($"ReplaceInstancesLODWhenRun LODPrefabList:{LODPrefabList.Count} count:{count} errorCount:{errorCount}");
    }

    public int ReplaceInstances(MeshFilter[] mfs)
    {
        int insCount = 0;
        if (mfs == null) return insCount;
        Debug.Log($"ReplaceInstances MeshFilter:{mfs.Length}");
        //MeshFilter[] mfs = MeshTarget.GetComponentsInChildren<MeshFilter>(true);
        var dict = GetPrefabMeshDict();
        int replaceCount = 0;
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            if (mf == null)
            {
                continue;
            }

#if UNITY_EDITOR
            if (ProgressBarHelper.DisplayCancelableProgressBar("Replace", i, mfs.Length))
            {
                break;
            }
#endif

            Mesh mesh = mf.sharedMesh;
            if (dict.ContainsKey(mesh))
            {
                BoundsBox bb = mf.GetComponent<BoundsBox>();
                if (bb)
                {
                    Debug.LogError($"ReplaceInstances Skip BoundsBox mf:{mf}");
                    continue;
                }

                //GameObject go = mf.gameObject;
                //Debug.Log($"ReplaceInstances[{i+1}/{mfs.Length}]({++replaceCount}) IsPartOfAnyPrefab:{PrefabUtility.IsPartOfAnyPrefab(go)} IsPartOfModelPrefab:{PrefabUtility.IsPartOfModelPrefab(go)}  IsPartOfPrefabAsset:{PrefabUtility.IsPartOfPrefabAsset(go)} IsPartOfPrefabInstance:{PrefabUtility.IsPartOfPrefabInstance(go)}");

                //if (PrefabUtility.IsPartOfAnyPrefab(go))
                //{
                //}
                //else
                //{
                //}

                GPUInstancerPrefab pre = dict[mesh];
                ReplacePrefabInstances(pre, mf.gameObject);
                GPUInstancerPrefab newPre = mf.gameObject.GetComponent<GPUInstancerPrefab>();
                initPrefabs.Add(newPre);
                insCount++;
            }
            else
            {

            }
        }
#if UNITY_EDITOR
        ProgressBarHelper.ClearProgressBar();
#endif
        return insCount;
    }


    public static void ReplacePrefabInstances(GPUInstancerPrefab pre, GameObject target)
    {
        MeshRenderer mr1=target.GetComponent<MeshRenderer>();
        MeshRenderer mr2=pre.GetComponent<MeshRenderer>();
        if(mr1 && mr2){
            mr1.sharedMaterials=mr2.sharedMaterials;
        }

//#if UNITY_EDITOR
//        EditorHelper.CopyComponent(target, pre);
//#endif
        GPUInstancerPrefab gpuiPrefab = target.AddMissingComponent<GPUInstancerPrefab>();
        gpuiPrefab.prefabPrototype = pre.prefabPrototype;
    }

    public RenderPipeline renderPipeLine = RenderPipeline.HDRP;

    public Material Transparent_Mat;//HDRPLit_Transparent_Double

    public Dictionary<GameObject, Material[]> matDict = new Dictionary<GameObject, Material[]>();

    void SetMaterialAlpha(GameObject go, float alpha)
    {
        if (StardardShaderMatHelper.Transparent_Mat == null)
        {
            StardardShaderMatHelper.Transparent_Mat = this.Transparent_Mat;
        }
        Renderer renderer = go.GetComponent<Renderer>();
        
        //if (alpha == 1)
        //{
        //    renderer.sharedMaterials = matDict[go];
        //    matDict.Remove(go);
        //    return;
        //}
        //else
        //{
        //    if (!matDict.ContainsKey(go))
        //    {
        //        matDict.Add(go, renderer.sharedMaterials);
        //    }
        //}

        int matNum = renderer.sharedMaterials.Length;
        for (int i = 0; i < matNum; i++)
        {
            ////HDRP�����£�����hdrplit���ʣ��������ڸ÷���
            //if (RoomFactory.Instance && RoomFactory.Instance.RenderPipelineType == RenderPipeline.HDRP && renderer.materials[i].shader.name != "HDRP/Lit") continue;

            Color _color = renderer.materials[i].color;
            _color.a = alpha;
            if (renderPipeLine == RenderPipeline.HDRP)
            {
                if (alpha == 1)
                {
                    StardardShaderMatHelper.SetMaterialRenderingMode(renderer.materials[i], RenderingMode.Opaque, RenderPipeline.HDRP, _color);
                }
                else
                {
                    StardardShaderMatHelper.SetMaterialRenderingMode(renderer.materials[i], RenderingMode.Transparent, RenderPipeline.HDRP, _color);
                    renderer.materials[i].SetColor("_BaseColor", _color);
                }
            }
            else
            {
                renderer.materials[i].SetColor("_Color", _color);
            }
        }
    }

    public GPUInstancerPrefab TestTargetGPUIModel;

    //[ContextMenu("HightlightOn")]
    public void HighlightOn()
    {
        HightlightModuleBase.HighlightOn(TestTargetGPUIModel.gameObject);
    }

    //[ContextMenu("HighlightOff")]
    public void HighlightOff()
    {
        HightlightModuleBase.HighlightOff(TestTargetGPUIModel.gameObject);
    }

    public void HightlightOn(GPUInstancerPrefab prefab)
    {
        HightlightModuleBase.HighlightOn(prefab.gameObject);
    }

    public void HightlightOff(GPUInstancerPrefab prefab)
    {
        HightlightModuleBase.HighlightOff(prefab.gameObject);
    }

    //[ContextMenu("HightlightOnEx")]
    public void HighlightOnEx()
    {
        HightlightOnEx(TestTargetGPUIModel);
    }

    //[ContextMenu("HighlightOffEx")]
    public void HighlightOffEx()
    {
        HighlightOffEx(TestTargetGPUIModel);
    }

    public void HightlightOnEx(GPUInstancerPrefab prefab)
    {
        GPUIOff(prefab);
        HightlightOn(prefab);
    }

    public void HighlightOffEx(GPUInstancerPrefab prefab)
    {
        HightlightOff(prefab);
        GPUIOn(prefab);
    }

    public void TransparentOnEx(GPUInstancerPrefab prefab)
    {
        GPUIOff(prefab);
        TransparentOn(prefab);
    }

    public void SetMaterialAlpha(GPUInstancerPrefab go, float alpha)
    {
        Debug.LogError($"GPUInstanceTest.SetMaterialAlpha go:{go} alpha:{alpha}");
        if (alpha == 1)
        {
            TransparentOffEx(go);
        }
        else
        {
            TransparentOnEx(go);
        }
    }

        public void TransparentOffEx(GPUInstancerPrefab prefab)
    {
        TransparentOff(prefab);
        GPUIOn(prefab);
    }

    public void TransparentOn(GPUInstancerPrefab prefab)
    {
        SetMaterialAlpha(prefab.gameObject, 0.1f);
    }

    public void TransparentOff(GPUInstancerPrefab prefab)
    {
        SetMaterialAlpha(prefab.gameObject, 1f);
    }

    public void GPUIOn(GPUInstancerPrefab prefab)
    {
        AddPrefabInstance(prefab);
    }

    public void GPUIOff(GPUInstancerPrefab prefab)
    {
        RemovePrefabInstance(prefab);
    }

    //[ContextMenu("TransparentOnEx")]
    public void TransparentOnEx()
    {
        GPUIOff();
        TransparentOn();
    }

    //[ContextMenu("TransparentOffEx")]
    public void TransparentOffEx()
    {
        TransparentOff();
        GPUIOn();
    }

    //[ContextMenu("TransparentOn")]
    public void TransparentOn()
    {
        SetMaterialAlpha(TestTargetGPUIModel.gameObject, 0.1f);
    }

    //[ContextMenu("TransparentOff")]
    public void TransparentOff()
    {
        SetMaterialAlpha(TestTargetGPUIModel.gameObject, 1f);
    }

    //[ContextMenu("GPUIOn")]
    public void GPUIOn()
    {
        AddPrefabInstance(TestTargetGPUIModel);
    }

    //[ContextMenu("GPUIOff")]
    public void GPUIOff()
    {
        RemovePrefabInstance(TestTargetGPUIModel);
    }

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

    public List<GPUInstancerPrefabList> LODPrefabList = new List<GPUInstancerPrefabList>();

    public List<GPUInstancerPrefab> PrefabList = new List<GPUInstancerPrefab>();

    public List<GameObject> ManagedPrefabList = new List<GameObject>();

    [ContextMenu("GetManagedPrefabList")]
    public void GetManagedPrefabList()
    {
        GetPrefabManager();
        ManagedPrefabList=prefabManager.prefabList;
    }

    //[ContextMenu("SortPrefabList")]
    public void SortPrefabList()
    {
        PrefabList.Sort((a, b) => a.name.CompareTo(b.name));
    }

    public GPUInstancerPrefabManager prefabManager;



    //public AstroidGenerator generator;
#if UNITY_EDITOR
    //[ContextMenu("0.OneKeySetPrefabs")]
    public void OneKeySetPrefabs()
    {
        System.DateTime start = System.DateTime.Now;
        //ClearOldPrefabs();
        GetPrefabMeshes();
        SavePrefabs();
        InitPrefabs();
        ReplaceInstances();
        Debug.Log($"OneKey time:{System.DateTime.Now-start}"); 
    }

    public void OneKeyClearPrefabs()
    {
        System.DateTime start = System.DateTime.Now;
        ClearOldPrefabs();
        DeleteOtherPrefabs();
        DeletePrototypeData();
        Debug.Log($"OneKey time:{System.DateTime.Now - start}");
    }


    //[ContextMenu("1.ClearOldPrefabs")]
    public void ClearOldPrefabs()
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
        prefabManager.ClearPrefabsAndPrototypes();
        PrefabList.Clear();
    }

    //[ContextMenu("2.GetPrefabMeshes")]
    public void GetPrefabMeshes()
    {
        //if (IsClearOldPrefabs)
        //{
        //    ClearOldPrefabs();
        //    CreatePrefabList();
        //}
        //else
        {
            GetGPUIPrefabList();
        }
    }

    //[ContextMenu("3.SaveMesh")]
    public void SaveMesh()
    {
        for (int i = 0; i < PrefabList.Count; i++)
        {
            var item = PrefabList[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("SaveMesh", i, PrefabList.Count))
            {
                break;
            }
            MeshFilter mf = item.GetComponent<MeshFilter>();
            if (UnityEditor.AssetDatabase.Contains(mf.sharedMesh)) continue;
            EditorHelper.SaveMeshAsset(mf.gameObject);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SavePrefabs PrefabList:{PrefabList.Count}");
    }

    //[ContextMenu("3.SavePrefabs")]
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
            if (goNew != null)
            {
                GameObject.DestroyImmediate(item.gameObject);
                PrefabList[i] = goNew.GetComponent<GPUInstancerPrefab>();
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SavePrefabs PrefabList:{PrefabList.Count}");
    }

    public void DeleteSamePrefabs()
    {
        DictList<GPUInstancerPrefab> list = new DictList<GPUInstancerPrefab>();
        List<Object> exceptionObjs = new List<Object>();
        int count1 = PrefabList.Count;
        for (int i = 0; i < PrefabList.Count; i++)
        {
            list.Add(PrefabList[i]);
        }
        PrefabList = list.NewList();
        int count2 = PrefabList.Count;
        Debug.Log($"DeleteSamePrefabs count1:{count1} count2:{count2}");
    }

    //[ContextMenu("DeletePrototypeData")]
    public void DeletePrototypeData()
    {
        EditorHelper.DeleteFolderFiles("Assets/ThirdPlugins/GPUInstancer/PrototypeData/Prefab");

        //List<Object> assets = new List<Object>();
        //string[] assetsList = AssetDatabase.FindAssets("", new string[] { "Assets/ThirdPlugins/GPUInstancer/PrototypeData/Prefab" });
        //for (int i = 0; i < assetsList.Length; i++)
        //{
        //    string item = assetsList[i];
        //    string path = AssetDatabase.GUIDToAssetPath(item);
        //    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
        //    assets.Add(obj);
        //    Debug.Log($"[{i}]{item} | {path} | {obj}");
        //}
        ////List<Object> assets=AssetDatabase.LoadAllAssetsAtPath("Assets/ThirdPlugins/GPUInstancer/Prefabs/").ToList();
        //int count1 = assets.Count;
        //int count2 = assets.Count;
        //for (int i = 0; i < assets.Count; i++)
        //{
        //    Object asset = assets[i];
        //    var path = AssetDatabase.GetAssetPath(asset);
        //    bool b = AssetDatabase.DeleteAsset(path);
        //    Debug.Log($"delete[{i}] asset:{asset} path:{path} b:{b}");
        //}
        //AssetDatabase.Refresh();
        //ProgressBarHelper.ClearProgressBar();
        //Debug.Log($"DeletePrototypeData PrefabList:{PrefabList.Count} count1:{count1} count2:{count2}");
    }

    //[ContextMenu("4.DeleteOtherPrefabs")]
    public void DeleteOtherPrefabs()
    {
        List<Object> exceptionObjs = new List<Object>();
        for (int i = 0; i < PrefabList.Count; i++)
        {
            exceptionObjs.Add(PrefabList[i].gameObject);
        }
        EditorHelper.DeleteFolderFiles(exceptionObjs,"Assets/ThirdPlugins/GPUInstancer/Prefabs");
    }

    public void SelectPrefabFile()
    {
        EditorHelper.SelectFolderFile("Assets/ThirdPlugins/GPUInstancer/Prefabs");
    }

    public void SelectPrototypeDataFile()
    {
        EditorHelper.SelectFolderFile("Assets/ThirdPlugins/GPUInstancer/PrototypeData/Prefab");
    }

    //[ContextMenu("5.ReplaceInstances")]
    public void ReplaceInstances()
    {
        MeshFilter[] mfs = GetMeshFilters();
        ReplaceInstances(mfs);
    }

    //[ContextMenu("5.ReplaceInstancesLODWhenEditor")]
    public void ReplaceInstancesLODWhenEditor()
    {
        //MeshFilter[] mfs = GetMeshFilters();
        //ReplaceInstances(mfs);
        List<LODGroup> groups = new List<LODGroup>();
        foreach(var root in GPUIRoots)
        {
            if(root is LODGPUIRoot)
            {
                var gs = root.GetComponentsInChildren<LODGroup>();
                groups.AddRange(gs);
            }
        }

        Dictionary<string, GPUInstancerPrefabList> prefabPathDict = new Dictionary<string, GPUInstancerPrefabList>();
        foreach(var obj in LODPrefabList)
        {
            obj.InstanceIds.Clear();
            GPUInstancerPrefab prefab = obj.Prefab.GetComponent<GPUInstancerPrefab>();
            if (prefab == null)
            {
                Debug.LogError($"ReplaceInstancesLOD prefab == null obj:{obj}");
                continue;
            }
            var prefabPath = GetPrefabAssetPath(obj.Prefab.gameObject);
            prefabPathDict.Add(prefabPath, obj);
        }

        foreach(var group in groups)
        {
            GPUInstancerPrefab prefab = group.GetComponent<GPUInstancerPrefab>();
            var prefabPath = GetPrefabAssetPath(prefab.gameObject);
            if (prefabPathDict.ContainsKey(prefabPath))
            {
                GPUInstancerPrefabList prefabAsset = prefabPathDict[prefabPath];
                prefab.prefabPrototype = prefabAsset.Prefab.prefabPrototype;
                RendererId rid = RendererId.GetRId(prefab.gameObject);
                prefabAsset.InstanceIds.Add(rid.Id);
            }
            else
            {
                Debug.LogError($"ReplaceInstancesLOD prefabPathDict.ContainsKey(prefabPath)==false group:{group} prefabPath:{prefabPath}");
            }
        }
        Debug.LogError($"ReplaceInstancesLOD groups:{groups.Count}");
    }

    /// <summary>
    /// 获取预制体资源路径。
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static string GetPrefabAssetPath(GameObject gameObject)
    {
#if UNITY_EDITOR
        // Project中的Prefab是Asset不是Instance
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            // 预制体资源就是自身
            return UnityEditor.AssetDatabase.GetAssetPath(gameObject);
        }

        // Scene中的Prefab Instance是Instance不是Asset
        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            // 获取预制体资源
            var prefabAsset = UnityEditor.PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
            return UnityEditor.AssetDatabase.GetAssetPath(prefabAsset);
        }

        // PrefabMode中的GameObject既不是Instance也不是Asset
        var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
        if (prefabStage != null)
        {
            // 预制体资源：prefabAsset = prefabStage.prefabContentsRoot
            return prefabStage.prefabAssetPath;
        }
#endif

        // 不是预制体
        return null;
    }


    public GameObject EditorSavePrefabPath(GameObject go, bool isOverride = false)
    {
        return EditorHelper.SavePrefab(go, "Assets/ThirdPlugins/GPUInstancer/Prefabs", isOverride);
    }
#endif

    private void GetPrefabManager()
    {
        if (prefabManager == null)
        {
            prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>(true);
        }
    }

    //[ContextMenu("4.InitPrefabs")]
    public void InitPrefabs()
    {
        GetPrefabManager();

        List<GPUInstancerPrefab> prefabList = new List<GPUInstancerPrefab>(PrefabList);
        foreach(var prefab in LODPrefabList)
        {
            prefabList.Add(prefab.Prefab);
        }

        prefabManager.InitPrefabs(prefabList);

        GetManagedPrefabList();
    }

#if UNITY_EDITOR

    public void InstantiatePrefabs()
    {
        for (int i1 = 0; i1 < PrefabList.Count; i1++)
        {
            GPUInstancerPrefab prefab = PrefabList[i1];
            PrefabUtility.InstantiatePrefab(prefab.gameObject);
        }
    }


    public void SetExternalMaterial()
    {
        for (int i1 = 0; i1 < PrefabList.Count; i1++)
        {
            GPUInstancerPrefab prefab = PrefabList[i1];
            InnerEditorHelper.SetExternalMaterial(prefab.gameObject, i1);
        }
        //EditorHelper.RefreshAssets();
        AssetDatabase.Refresh();
    }

    public void ReplaceMaterials()
    {
        Dictionary<string, Material> matDict = new Dictionary<string, Material>();
        foreach(var mat in MaterialLib)
        {
            if (mat == null) continue;
            matDict.Add(mat.name, mat);
        }
        for (int i1 = 0; i1 < PrefabList.Count; i1++)
        {
            GPUInstancerPrefab prefab = PrefabList[i1];
            MeshRenderer mr = prefab.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                return;
            }
            bool isChanged = false;
            Material[] newMats = mr.sharedMaterials;
            for (int i = 0; i < mr.sharedMaterials.Length; i++)
            {
                Material mat = mr.sharedMaterials[i];
                if (matDict.ContainsKey(mat.name))
                {
                    Material matNew = matDict[mat.name];
                    if (matNew != null)
                    {
                        //mr.sharedMaterials[i] = matNew;
                        newMats[i] = matNew;
                        Debug.LogWarning($"ReplaceMaterials[{i1}][{i}]1 {prefab.name}|{mat.name}");
                        isChanged = true;
                    }
                    else
                    {
                        Debug.LogError($"ReplaceMaterials[{i1}][{i}]2 {prefab.name}|{mat.name}");
                    }
                }
            }
            if (isChanged)
            {
                mr.sharedMaterials = newMats;
                InnerEditorHelper.ChangePrefabMaterials(prefab.gameObject, newMats);
                //EditorSavePrefabPath(prefab.gameObject, true);
            }
        }
    }

    public void SetMaterialTransparent()
    {
        for (int i1 = 0; i1 < PrefabList.Count; i1++)
        {
            GPUInstancerPrefab prefab = PrefabList[i1];
            InnerEditorHelper.SetExternalMaterial(prefab.gameObject, i1);

            MeshRenderer mr = prefab.GetComponent<MeshRenderer>();
            bool isChanged = false;
            Material[] newMats = mr.sharedMaterials;
            for (int i = 0; i < mr.sharedMaterials.Length; i++)
            {
                Material mat = mr.sharedMaterials[i];
                string matPath = AssetDatabase.GetAssetPath(mat);
                //Debug.Log($"SetPrefabsMaterial[{i1}][{i}] {prefab.name}|{mat.name}|{matPath}");
                if (matPath.EndsWith(".fbx"))
                {
                    
                }
                else
                {
                    Color color=MatInfo.GetColor(mat);
                }
            }
        }
        //EditorHelper.RefreshAssets();
        AssetDatabase.Refresh();
    }
#endif

    //[ContextMenu("*.GetPrefabListInfo10-300")]
    public void GetPrefabListInfo10_300()
    {
        StringBuilder sb = new StringBuilder();
        SharedMeshInfoList sharedMeshInfos = GetSharedMeshInfoList();
        for (int i = 2; i < 10; i += 1)
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

    //[ContextMenu("*.GetPrefabListInfo")]
    public void GetPrefabListInfo()
    {
        SharedMeshInfoList sharedMeshInfos = GetSharedMeshInfoList();
        GetPrefabListInfoInner(MinPrefabInstanceCount, sharedMeshInfos);
    }

    private void GetGPUIPrefabList()
    {
        //PrefabList.Clear();
        SharedMeshInfoList sharedMeshInfos = GetSharedMeshInfoList();
        GetGPUIPrefabList(sharedMeshInfos);
    }

    private SharedMeshInfoList GetSharedMeshInfoList()
    {
        //SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(MeshTarget, true);

        MeshFilter[] meshFilters = GetMeshFilters();
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(meshFilters);
        return sharedMeshInfos;
    }

    private MeshFilter[] GetMeshFilters()
    {
        //MeshFilter[] meshFilters = MeshTarget.GetComponentsInChildren<MeshFilter>(true);
        MeshFilter[] meshFilters = GetTargetsComponentsEx<MeshFilter>(false).ToArray();
        return meshFilters;
    }

    private List<GameObject> GetTargetList(bool isAll)
    {
        List<GameObject> targetList = new List<GameObject>();
        foreach (var root in GPUIRoots)
        {
            if (isAll == false)
            {
                if(root is LODGPUIRoot)
                {
                    continue;
                }

            }
            targetList.Add(root.gameObject);
        }
        if (MeshTarget)
        {
            targetList.Add(MeshTarget);
        }
        return targetList;
    }

    private GPUInstancerPrefab[] GetGPUInstancerPrefabs()
    {
        //GPUInstancerPrefab[] prefabs = MeshTarget.GetComponentsInChildren<GPUInstancerPrefab>(true);
        GPUInstancerPrefab[] prefabs = GetGPUIPrefabListOfTarget().ToArray();
        return prefabs;
    }

    private GameObject GetPrefabsRoot()
    {
        if (PrefabsRoot == null)
        {
            PrefabsRoot = GameObject.Find("GPUIPrefabsRoot");
        }
        if (PrefabsRoot == null)
        {
            PrefabsRoot = new GameObject("GPUIPrefabsRoot");
        }
        return PrefabsRoot;
    }

    private void GetGPUIPrefabList(SharedMeshInfoList sharedMeshInfos)
    {
        GameObject prefabRoot = GetPrefabsRoot();
        prefabRoot.ClearChildren();
        PrefabList.Clear();

        sharedMeshInfos.Sort();
        var prefabMeshDict = GetPrefabMeshDict();
        var managedPrefabMeshDict = GetManagedPrefabMeshDict();
        int totalCount = 0;
        int intanceCount = 0;
        int otherCount = 0;

        int containsCount = 0;
        int containsCount2 = 0;
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

                if (prefabMeshDict.ContainsKey(sm.mesh))
                {
                    var prefabGo = prefabMeshDict[sm.mesh];
                    PrefabList.Remove(prefabGo);
                }
                //Debug.Log($"UpdatePrefabList meshCount < MinPrefabInstanceCount meshCount:{meshCount} MinPrefabInstanceCount:{MinPrefabInstanceCount} sm:{sm.mesh.name}");
                continue;
            }
            intanceCount += meshCount;
            if (prefabMeshDict.ContainsKey(sm.mesh))
            {
                Debug.Log($"GetGPUIPrefabList[{i+1}/{sharedMeshInfos.Count}]({++containsCount}) meshDict.ContainsKey(sm.mesh) mesh:{sm.mesh} meshCount:{meshCount} MinPrefabInstanceCount:{MinPrefabInstanceCount}");
                continue;
            }

            if (managedPrefabMeshDict.ContainsKey(sm.mesh))
            {
                var pre = managedPrefabMeshDict[sm.mesh];
                PrefabList.Add(pre);
                Debug.Log($"GetGPUIPrefabList[{i + 1}/{sharedMeshInfos.Count}]({++containsCount2}) managedPrefabMeshDict.ContainsKey(sm.mesh) mesh:{sm.mesh} meshCount:{meshCount} MinPrefabInstanceCount:{MinPrefabInstanceCount}");
            }
            else
            {
                sm.mainMeshFilter.sharedMesh.name = sm.mainMeshFilter.sharedMesh.name.Replace("/", "=");
                GameObject go = GameObjectExtension.CopyMeshObject(sm.gameObject, $"{sm.mainMeshFilter.sharedMesh.name}({sm.GetCount()})");
                go.transform.SetParent(null);
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                GPUInstancerPrefab pre = go.AddMissingComponent<GPUInstancerPrefab>();
                PrefabList.Add(pre);
                go.transform.SetParent(prefabRoot.transform);
            }
        }

        if (PrefabList.Count == 0)
        {
            Debug.LogError($"UpdatePrefabList(PrefabList.Count == 0!!) MinCount:{MinPrefabInstanceCount}=> Prefabs:{PrefabList.Count} [{intanceCount}({(float)intanceCount / (float)totalCount:P1})+{otherCount}={totalCount}] sharedMeshs:{sharedMeshInfos.Count} Root:{prefabRoot} ");
        }
        else
        {
            Debug.Log($"UpdatePrefabList MinCount:{MinPrefabInstanceCount}=> Prefabs:{PrefabList.Count} [{intanceCount}({(float)intanceCount / (float)totalCount:P1})+{otherCount}={totalCount}] sharedMeshs:{sharedMeshInfos.Count} Root:{prefabRoot} ");
        }
    }

    private void CreatePrefabList()
    {
        PrefabList.Clear();
        SharedMeshInfoList sharedMeshInfos = GetSharedMeshInfoList();
        int totalCount = 0;
        int intanceCount = 0;
        int otherCount = 0;
        GameObject prefabRoot = GetPrefabsRoot();
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

            go.transform.SetParent(prefabRoot.transform);
        }

        Debug.Log($"CreatePrefabList MinCount:{MinPrefabInstanceCount}=> Prefabs:{PrefabList.Count} [{intanceCount}({(float)intanceCount / (float)totalCount:P1})+{otherCount}={totalCount}] sharedMeshs:{sharedMeshInfos.Count}  Root:{prefabRoot} ");
    }

    //public bool IsClearPrefabType = false;

    //public AstroidGenerator astroidGenerator;


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

        Debug.Log($"GetPrefabListInfo MinCount:{minCount}=> Prefabs:{prefabCount} [{intanceCount}({(float)intanceCount / (float)totalCount:P1})+{otherCount}={totalCount}] [{intanceVertex}({(float)intanceVertex / (float)totalVertex:P1})+{otherVertex}={totalVertex}] sharedMeshs:{sharedMeshInfos.Count}");
        return $"{minCount}\t{prefabCount}\t{intanceCount}\t{otherCount}\t{totalCount}\t{(float)intanceCount / (float)totalCount:P1}\t{intanceVertex}\t{otherVertex}\t{totalVertex}\t{(float)intanceVertex / (float)totalVertex:P1}";
    }


    //[ContextMenu("GetPrefabsInScene")]
    public void GetPrefabsInScene()
    {
        //GPUInstancerPrefab[] prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>(true);
        GPUInstancerPrefab[] prefabs = GetGPUInstancerPrefabs();
        Debug.Log($"GetPrefabsInScene prefabs:{prefabs.Length}");
        int count = 0;
        int count2 = 0;
        for (int i = 0; i < prefabs.Length; i++)
        {
            var prefab = prefabs[i];
            //if (prefab.gameObject.activeInHierarchy == true)
            //{
            //    Debug.LogError($"GetPrefabsInScene[{i + 1}/{prefabs.Length}] activeInHierarchy == true[{++count2}] prefab:{prefab} path:{prefab.transform.GetPath()}");
            //}
            if (prefab.prefabPrototype == null)
            {
                Debug.LogError($"GetPrefabsInScene[{i+1}/{prefabs.Length}] prefabPrototype == null[{++count}] prefab:{prefab} path:{prefab.transform.GetPath()}");
            }
            if (insListOfStarted.Count > 0)
            {
                if (insListOfStarted.Contains(prefab))
                {

                }
                else
                {
                    Debug.LogError($"GetPrefabsInScene[{i + 1}/{prefabs.Length}] prefabPrototype == null[{++count}] prefab:{prefab} path:{prefab.transform.GetPath()}");
                }
            }
        }

        AutomaticLOD[] lods = GameObject.FindObjectsOfType<AutomaticLOD>();
        Debug.Log("GetPrefabsInScene lods:" + lods.Length);
    }

    public GameObject MeshTarget = null;

    public int MinPrefabInstanceCount = 100;

    public bool IsStartGPUInstance = true;

    public bool IsAutoGPUI = false;

    public bool IsHideNotInPrefabs = false;

    //public bool IsClearOldPrefabs = true;

    //-------------------------

    public bool IsAutoHideShowRoot = true;

    public bool IsAsync = false;

    public bool runInThreads = true;

    public float UpdatePrefabRootInterval = 0.1f;

    public int CoroutineSize = 500;

    public List<GPUIRoot> GPUIRoots = new List<GPUIRoot>();

    private Dictionary<GameObject,bool> PrefabRootsState = new Dictionary<GameObject, bool>();

    public void CheckList()
    {
        GPUIRoots.RemoveAll(i => i == null);
        PrefabList.RemoveAll(i => i == null);
    }

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
            SetPrefabRootActive(root.gameObject);
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

    private IEnumerator AutoUpdatePrefabRootsVisible_Coroutine()
    {
        Debug.Log($"UpdatePrefabRootsVisible_Coroutine Start GPUIRoots:{GPUIRoots.Count}");
        int count = 0;

        
        while (IsAutoHideShowRoot)
        {
            System.DateTime start = System.DateTime.Now;
            //if (GPUIRoots.Count == 0)
            //{
            //    Debug.LogError("UpdatePrefabRootsVisible_Coroutine GPUIRoots.Count == 0");
            //    IsAutoHideShowRoot = false;
            //    break;
            //}

            //bool isActive0 = MeshTarget.activeInHierarchy;
            bool isAllInActive = GetIsAllInActive();
            if (isAllInActive)
            {
                if (prefabManager.IsEnableUpdateBuffers == true)
                {
                    prefabManager.IsEnableUpdateBuffers = false;
                    Debug.Log($"UpdatePrefabRootsVisible_Coroutine[{++count}] time:{System.DateTime.Now - start} GPUIRoots:{GPUIRoots.Count} isActive0:{isAllInActive}");
                }
            }
            else
            {
                if (prefabManager.IsEnableUpdateBuffers == false)
                {
                    prefabManager.IsEnableUpdateBuffers = true;
                    Debug.Log($"UpdatePrefabRootsVisible_Coroutine[{++count}] time:{System.DateTime.Now - start} GPUIRoots:{GPUIRoots.Count} isActive0:{isAllInActive}");
                }
                //foreach (var root in GPUIRoots)
                //{
                //    if (root == null) continue;
                //    yield return SetPrefabRootActive_Coroutine(root);
                //}
            }
            
            yield return new WaitForSeconds(UpdatePrefabRootInterval);
        }
    }

    private DictList<IGPUIRoot> RootsOfHide = new DictList<IGPUIRoot>();
    private DictList<IGPUIRoot> RootsOfShow = new DictList<IGPUIRoot>();


    private DictList<SubScene_GPUI> ScenesOfHide = new DictList<SubScene_GPUI>();
    private DictList<SubScene_GPUI> ScenesOfShow = new DictList<SubScene_GPUI>();

    private SubScene_GPUI[] allGPUIScenes;

    private Dictionary<IGPUIRoot,bool> GPUIStateDict = new Dictionary<IGPUIRoot, bool>();

    //private DictList<SubScene_GPUI> ScenesOfHide_Done = new DictList<SubScene_GPUI>();
    //private DictList<SubScene_GPUI> ScenesOfShow_Done = new DictList<SubScene_GPUI>();

    private DictList<GPUInstancerPrefab> initPrefabs = new DictList<GPUInstancerPrefab>();

    public int PrefabCount
    {
        get
        {
            return initPrefabs.Count;
        }
    }

    private int sceneCount = 0;
    public void RegistGPUIScene(SubScene_GPUI scene)
    {
        if (scene == null) return;
        
        GameObject keyGo = scene.gameObject;
        if (instancesDict.ContainsKey(keyGo))
        {
            Debug.LogError($"RegistGPUIScene[{++sceneCount}] instancesDict.ContainsKey(keyGo) scene:{scene.name} initPrefabs:{initPrefabs.Count} path:{scene.transform.GetPath()}");
        }
        else
        {
            List<GPUInstancerPrefab> prefabs = scene.GetSceneGPUIPrefabs();
            initPrefabs.AddRange(prefabs);
            instancesDict.Add(keyGo, prefabs);
            Debug.Log($"RegistGPUIScene[{++sceneCount}] scene:{scene.name} prefabs:{prefabs.Count} initPrefabs:{initPrefabs.Count}");
        }
    }

    //public void RegistGPUI<T>(T scene) where T : MonoBehaviour, IGPUIRoot
    //{
    //    if (scene == null) return;
    //    GameObject keyGo = scene.gameObject;
    //    //if (instancesDict.ContainsKey(keyGo))
    //    //{
    //    //    Debug.LogError($"RegistGPUIScene[{++sceneCount}] instancesDict.ContainsKey(keyGo) scene:{scene.name} initPrefabs:{initPrefabs.Count} path:{scene.transform.GetPath()}");
    //    //}
    //    //else
    //    {
    //        var prefabs = scene.GetGPUIPrefabs();
    //        initPrefabs.AddRange(prefabs);
    //        //instancesDict.Add(keyGo, prefabs);
    //        Debug.Log($"RegistGPUIScene[{++sceneCount}] scene:{scene.name} prefabs:{prefabs.Length} initPrefabs:{initPrefabs.Count}");
    //    }
    //}

    public GPUInstancerPrefab[] RegistGPUI(GPUIRoot root)
    {
        //Debug.Log($"RegistGPUI root:{root} path:{root.transform.GetPath()}");
        GPUInstancerPrefab[] prefabs = null;
        if (root == null) return prefabs;
        //if (GPUIRoots.Contains(scene))
        //{
        //    return prefabs;
        //}

        //GameObject keyGo = scene.gameObject;

        //if (instancesDict.ContainsKey(keyGo))
        //{
        //    Debug.LogError($"RegistGPUIScene[{++sceneCount}] instancesDict.ContainsKey(keyGo) scene:{scene.name} initPrefabs:{initPrefabs.Count} path:{scene.transform.GetPath()}");
        //}
        //else
        {
            prefabs = root.GetGPUIPrefabs();
            initPrefabs.AddRange(prefabs);
            //instancesDict.Add(keyGo, prefabs);
            //Debug.Log($"RegistGPUIScene[{++sceneCount}] scene:{root.name} prefabs:{prefabs.Length} initPrefabs:{initPrefabs.Count}");
        }
        AddGPUIRoot(root);
        return prefabs;
    }

    private void AddGPUIRoot(GPUIRoot root)
    {
        if (!GPUIRoots.Contains(root))
        {
            //Debug.Log($"AddGPUIRoot root:{root} path:{root.transform.GetPath()}");
            GPUIRoots.Add(root);
        }
    }

    public void AddShowScene(SubScene_GPUI scene)
    {
        //Debug.LogError($"AddShowScene Start scene:{scene} ScenesOfShow:{ScenesOfShow.Count} ScenesOfHide:{ScenesOfHide.Count}");
        ScenesOfShow.Add(scene);
        ScenesOfHide.Remove(scene);
        //Debug.LogError($"AddShowScene End scene:{scene} ScenesOfShow:{ScenesOfShow.Count} ScenesOfHide:{ScenesOfHide.Count}");

        //GPUIStateList.Add(new GPUIRootState(scene, true));
        if (scene.gameObject.activeInHierarchy == false)
        {
            Debug.LogError($"GPUInstanceTest.AddShowScene scene.gameObject.activeInHierarchy == false scene:{scene} ScenesOfShow:{ScenesOfShow.Count} ScenesOfHide:{ScenesOfHide.Count} path:{scene.transform.GetPath()}");
        }
        SetGPUIState(scene, true);
    }

    private void SetGPUIState(IGPUIRoot root,bool visible)
    {
        if (GPUIStateDict.ContainsKey(root))
        {
            //GPUIStateDict[root] = visible;
            if (GPUIStateDict[root] == visible)
            {

            }
            else
            {
                GPUIStateDict.Remove(root);
            }
            //GPUIStateDict[root] = visible;
        }
        else
        {
            GPUIStateDict.Add(root, visible);
        }
    }

    public void AddHideScene(SubScene_GPUI scene)
    {
        //Debug.LogError($"AddHideScene Start scene:{scene} ScenesOfShow:{ScenesOfShow.Count} ScenesOfHide:{ScenesOfHide.Count}");
        ScenesOfHide.Add(scene);
        ScenesOfShow.Remove(scene);
        //Debug.LogError($"AddHideScene End scene:{scene} ScenesOfShow:{ScenesOfShow.Count} ScenesOfHide:{ScenesOfHide.Count}");

        SetGPUIState(scene, false);
    }

    public void AddToShowScene(GPUIRoot scene,bool isDebug)
    {
        isShowLog = isDebug;
        if (isDebug)
        {
            Debug.LogError($"AddToShowScene Start scene:{scene} RootsOfShow:{RootsOfShow.Count} RootsOfHide:{RootsOfHide.Count}");
        }
        RootsOfShow.Add(scene);
        RootsOfHide.Remove(scene);
        if (isDebug)
        {
            Debug.LogError($"AddToShowScene End scene:{scene} RootsOfShow:{RootsOfShow.Count} RootsOfHide:{RootsOfHide.Count}");
        }

        SetGPUIState(scene, true);
    }

    public void AddToHideScene(GPUIRoot scene, bool isDebug)
    {
        isShowLog = isDebug;
        if (isDebug)
        {
            Debug.LogError($"AddToHideScene Start scene:{scene} RootsOfShow:{RootsOfShow.Count} RootsOfHide:{RootsOfHide.Count}");
        }
        RootsOfHide.Add(scene);
        RootsOfShow.Remove(scene);
        if (isDebug)
        {
            Debug.LogError($"AddToHideScene End scene:{scene} RootsOfShow:{RootsOfShow.Count} RootsOfHide:{RootsOfHide.Count}");
        }

        SetGPUIState(scene, false);
    }

    private void ClearShowHideList()
    {
        RootsOfHide.Clear();
        RootsOfShow.Clear();
        ScenesOfHide.Clear();
        ScenesOfShow.Clear();
    }

    public bool IsBusyHideShow = false;

    private IEnumerator UpdateGPUIScenesVisible_Coroutine()
    {
        allGPUIScenes = GameObject.FindObjectsOfType<SubScene_GPUI>(true);

        Debug.Log($"UpdatePrefabRootsVisible_Coroutine Start GPUIRoots:{GPUIRoots.Count} allGPUIScenes:{allGPUIScenes.Length}");
        int count = 0;

       

        while (IsAutoHideShowRoot)
        {
            System.DateTime start = System.DateTime.Now;
            //if (GPUIRoots.Count == 0)
            //{
            //    Debug.LogError("UpdatePrefabRootsVisible_Coroutine GPUIRoots.Count == 0");
            //    IsAutoHideShowRoot = false;
            //    break;
            //}

            ScenesOfHide.Clear();
            ScenesOfShow.Clear();
            RootsOfHide.Clear();
            RootsOfShow.Clear();
            if (GPUIStateDict.Count > 0)
            {
                foreach(var scene in GPUIStateDict.Keys)
                {
                    bool visible = GPUIStateDict[scene];
                    if(scene is SubScene_GPUI)
                    {
                        SubScene_GPUI gpuiScene = scene as SubScene_GPUI;
                        if(visible!=gpuiScene.gameObject.activeInHierarchy)
                        {
                            Debug.LogError($"UpdatePrefabRootsVisible_Coroutine_Error0 visible!=gpuiScene.gameObject.activeInHierarchy visible:{visible} allGPUIScenes:{allGPUIScenes.Length} scene:{gpuiScene.name} path:{gpuiScene.transform.GetPath()}");
                        }
                    }
                    if (visible)
                    {
                        RootsOfShow.Add(scene);
                    }
                    else
                    {
                        RootsOfHide.Add(scene);
                    }
                }
                GPUIStateDict.Clear();
            }

            int inactiveCount = 0;
            foreach(var scene in allGPUIScenes)
            {
                if (scene.gameObject.activeInHierarchy == false)
                {
                    inactiveCount++;

                    if (RootsOfHide.Contains(scene))
                    {

                    }
                    else
                    {
                        //Debug.LogError($"UpdatePrefabRootsVisible_Coroutine_Error1 RootsOfHide.Contains(scene)==false allGPUIScenes:{allGPUIScenes.Length} scene:{scene.name} path:{scene.transform.GetPath()}");
                    }
                    if (RootsOfShow.Contains(scene))
                    {
                        Debug.LogError($"UpdatePrefabRootsVisible_Coroutine_Error2 RootsOfShow.Contains(scene)==true allGPUIScenes:{allGPUIScenes.Length} scene:{scene.name} path:{scene.transform.GetPath()}");
                        RootsOfShow.Remove(scene);
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    //if (RootsOfShow.Contains(scene))
                    //{

                    //}
                    //else
                    //{
                    //    Debug.LogError($"UpdatePrefabRootsVisible_Coroutine_Error3 RootsOfShow.Contains(scene)==false allGPUIScenes:{allGPUIScenes.Length} scene:{scene.name} path:{scene.transform.GetPath()}");
                    //}
                    //if (RootsOfHide.Contains(scene))
                    //{
                    //    Debug.LogError($"UpdatePrefabRootsVisible_Coroutine_Error4 RootsOfHide.Contains(scene)==true allGPUIScenes:{allGPUIScenes.Length} scene:{scene.name} path:{scene.transform.GetPath()}");
                    //}
                    //else
                    //{

                    //}
                }
            }

            int count1 = ScenesOfHide.Count;
            int count2 = ScenesOfShow.Count;
            int count3 = RootsOfHide.Count;
            int count4 = RootsOfShow.Count;

            if (EnableAutoHide)
            {
                DictList<IGPUIRoot> scenesDict = new DictList<IGPUIRoot>();
                List<SubScene_GPUI> ScenesOfHideTemp = ScenesOfHide.NewList();
                if (ScenesOfHideTemp.Count > 0)
                {
                    //List<SubScene_GPUI> listOfHide = ScenesOfHide.NewList();
                    //ScenesOfHide_Done.AddRange(listOfHide);
                    //ScenesOfHide.Clear();
                    //yield return RemovePrefabInstancesAsyncEx(ScenesOfHide, start);

                    foreach (var item in ScenesOfHideTemp)
                    {
                        scenesDict.Add(item);
                    }
                    ScenesOfHide.Clear();
                }

                List<IGPUIRoot> RootsOfHideTemp = RootsOfHide.NewList();
                if (RootsOfHideTemp.Count > 0)
                {
                    //yield return RemovePrefabInstancesAsyncEx(RootsOfHide, start);
                    foreach (var item in RootsOfHideTemp)
                    {
                        scenesDict.Add(item);
                    }
                    RootsOfHide.Clear();
                }

                if (scenesDict.Count > 0)
                {
                    List<IGPUIRoot> listOfHide = scenesDict.NewList();
                    System.DateTime start00 = System.DateTime.Now;
                    List<GPUInstancerPrefab> allPrefabs = GetScenesGPUIPrefabs(listOfHide);
                    if (allPrefabs.Count > 0)
                    {
                        if (IsAsync)
                        {
                            int waitCount = 0;
                            while (RemovePrefabInstancesAsync(allPrefabs) == false)
                            {
                                yield return new WaitForSeconds(UpdatePrefabRootInterval);
                                if (isShowLog)
                                {
                                    Debug.LogWarning($"UpdatePrefabRootsVisible_RemovePrefabInstancesAsyncEx WaitThread[{++waitCount}][time:{System.DateTime.Now - start}] Hide:{ScenesOfHide.Count}|{RootsOfHide.Count} Show1:{ScenesOfShow.Count}|{RootsOfShow.Count}");
                                }
                            }
                            scenesDict.Clear();
                        }
                        else
                        {
                            scenesDict.Clear();
                            yield return RemovePrefabsInstances_Coroutine(allPrefabs);
                        }
                    }
                    //yield return RemovePrefabsInstances_Coroutine(listOfHide);
                    if (isShowLog)
                    {
                        Debug.Log($"UpdatePrefabRootsVisible_RemovePrefabInstancesAsyncEx Finished[time:{System.DateTime.Now - start00}] listOfHide:{listOfHide.Count} allPrefabs:{allPrefabs.Count} meshPrefabDict:{GetMeshPrefabDict().Count}  ");
                    }
                }
            }


            //List<SubScene_GPUI> listOfShowFirst = new List<SubScene_GPUI>();
            //List<SubScene_GPUI> listOfShowAdd = new List<SubScene_GPUI>();

            if (EnableAutoShow)
            {
                if (ScenesOfShow.Count > 0)
                {
                    yield return AddGPUIPrefabInstancesAsyncEx(ScenesOfShow, start);
                }
                if (RootsOfShow.Count > 0)
                {
                    yield return AddGPUIPrefabInstancesAsyncEx(RootsOfShow, start);
                }
            }
            //if (isShowLog)
            {
                if (count1 > 0 || count2 > 0 || count3 > 0 || count4 > 0 || isShowLog)
                {
                    //Debug.LogError($"UpdatePrefabRootsVisible_4_ALL [time:{System.DateTime.Now - start}] Hide1:{count1} Show1:{count2} Hide2:{count3} Show2:{count4}");
                    Debug.Log($"UpdatePrefabRootsVisible_4_ALL [time:{System.DateTime.Now - start}] Hide:{count1}|{count3} Show1:{count2}|{count4}");
                }
            }
            count++;
            yield return new WaitForSeconds(UpdatePrefabRootInterval);
        }
    }

    public bool EnableAutoHide = true;

    public bool EnableAutoShow = true;

    private IEnumerator AddGPUIPrefabInstancesAsyncEx<T>(DictList<T> scenesDict, System.DateTime start) where T : IGPUIRoot
    {
        Debug.LogError($"AddGPUIPrefabInstancesAsyncEx scenesDict:{scenesDict.Count}");
        //List<GPUInstancerPrefab> listOfShow = new List<GPUInstancerPrefab>(ScenesOfShow);
        //ScenesOfShow.Clear();
        //yield return AddGPUIPrefabInstances_Coroutine(listOfShow, "UpdatePrefabRootsVisible_Coroutine");
        List<T> listOfShow = scenesDict.NewList();
        scenesDict.Clear();

        //foreach (var scene in listOfShow)
        //{
        //    List<GameObject> targets = scene.GetObjects();
        //    GetInstances(scene.gameObject, targets);

        //    ScenesOfShow_Done.Add(scene);

        //    ////StartGPUInstanceEx(scene.gameObject, scene.GetObjects());
        //    ////yield return null;
        //    //if (instancesDict.ContainsKey(scene.gameObject))
        //    //{
        //    //    listOfShowAdd.Add(scene);
        //    //}
        //    //else
        //    //{
        //    //    listOfShowFirst.Add(scene);
        //    //}
        //    listOfShowAdd.Add(scene);
        //}

        //if (listOfShowFirst.Count > 0)
        //{
        //    System.DateTime start1 = System.DateTime.Now;
        //    List<GPUInstancerPrefab> instancesAll = new List<GPUInstancerPrefab>();
        //    //List<GameObject> allTarget = new List<GameObject>();
        //    foreach (var scene in listOfShowFirst)
        //    {
        //        //List<GameObject> targets = scene.GetObjects();
        //        //allTarget.AddRange(targets);
        //        //StartGPUInstanceEx(scene.gameObject, targets);
        //        //yield return null;
        //        List<GameObject> targets = scene.GetObjects();
        //        instancesAll.AddRange(GetInstances(scene.gameObject, targets));
        //    }

        //    //StartGPUInstance(instancesAll);

        //    if (isShowLog)
        //    {
        //        Debug.Log($"UpdatePrefabRootsVisible_2_StartGPUInstanceEx[time:{System.DateTime.Now - start1}] listOfShowFirst:{listOfShowFirst.Count} instancesAll:{instancesAll.Count} meshPrefabDict:{meshPrefabDict.Count}");
        //    }
        //}

        if (listOfShow.Count > 0)
        {
            System.DateTime start2 = System.DateTime.Now;
            List<GPUInstancerPrefab> instances = new List<GPUInstancerPrefab>();
            foreach (var scene in listOfShow)
            {
                //List<GameObject> targets = scene.GetObjects();
                //instances.AddRange(GetInstances(scene.gameObject, targets));
                instances.AddRange(scene.GetGPUIPrefabs());
            }

            if (IsAsync)
            {
                //if (AddGPUIPrefabInstancesAsync(instances))
                //{

                //}
                //else
                //{
                //    ScenesOfShow.AddRange(listOfShowAdd);
                //}

                while (AddGPUIPrefabInstancesAsync(instances) == false)
                {
                    yield return new WaitForSeconds(UpdatePrefabRootInterval);
                    if (isShowLog)
                    {
                        Debug.LogWarning($"UpdatePrefabRootsVisible_3_AddGPUIPrefabInstances_Coroutine WaitThread[time:{System.DateTime.Now - start}]");
                    }
                }
                //ScenesOfHide.Clear();
            }
            else
            {
                yield return AddGPUIPrefabInstances_Coroutine(instances, "UpdatePrefabRootsVisible");
            }

            if (isShowLog)
            {
                Debug.Log($"AddGPUIPrefabInstancesAsyncEx [time:{System.DateTime.Now - start2}] instances:{instances.Count} meshPrefabDict:{GetMeshPrefabDict().Count}  ");
            }
        }
    }

    private IEnumerator RemovePrefabInstancesAsyncEx<T>(DictList<T> scenesDict, System.DateTime start) where T : IGPUIRoot
    {
        List<T> listOfHide = scenesDict.NewList();
        System.DateTime start00 = System.DateTime.Now;
        List<GPUInstancerPrefab> allPrefabs = GetScenesGPUIPrefabs(listOfHide);
        if (IsAsync)
        {
            while (RemovePrefabInstancesAsync(allPrefabs) == false)
            {
                yield return new WaitForSeconds(UpdatePrefabRootInterval);
                if (isShowLog)
                {
                    Debug.LogWarning($"RemovePrefabInstancesAsyncEx WaitThread[time:{System.DateTime.Now - start}]");
                }
            }
            scenesDict.Clear();
        }
        else
        {
            scenesDict.Clear();
            yield return RemovePrefabsInstances_Coroutine(allPrefabs);
        }
        //yield return RemovePrefabsInstances_Coroutine(listOfHide);
        if (isShowLog)
        {
            Debug.Log($"RemovePrefabInstancesAsyncEx Finished[time:{System.DateTime.Now - start00}] listOfHide:{listOfHide.Count} allPrefabs:{allPrefabs.Count} meshPrefabDict:{GetMeshPrefabDict().Count}  ");
        }
    }

    private static List<GPUInstancerPrefab> GetScenesGPUIPrefabs<T>(List<T> listOfHide) where T : IGPUIRoot
    {
        List<GPUInstancerPrefab> allPrefabs = new List<GPUInstancerPrefab>();
        foreach (var scene in listOfHide)
        {
            //ScenesOfHide_Done.Add(scene);
            var gpuiPrefabs = scene.GetGPUIPrefabs();
            foreach (var prefab in gpuiPrefabs)
            {
                if (prefab.prefabPrototype == null)
                {
                    Debug.LogError($"GetScenesGPUIPrefabs prefab.prefabPrototype == null prefab:{prefab} path:{prefab.transform.GetPath()}");
                }
                else
                {
                    allPrefabs.Add(prefab);
                }
            }

            //allPrefabs.AddRange(gpuiPrefabs);
            ////yield return RemovePrefabsInstances_Coroutine(scene.GetSceneGPUIPrefabs());
            //RemovePrefabsInstances(gpuiPrefabs);
            //yield return null;
        }

        return allPrefabs;
    }


    private void OnDestroy()
    {
        IsAutoHideShowRoot = false;
    }

    private void OnDisable()
    {
        Debug.Log($"OnDisable:{this.name}");
        StopCoroutine("UpdateGPUIScenesVisible_Coroutine");
        StopCoroutine("AutoUpdatePrefabRootsVisible_Coroutine");
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

    //public bool IsAstroidGenerator = true;

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
        GPUInstancerAPI.AddPrefabInstance(prefabManager, prefabInstance,true);
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
                GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
                if (isActiveNew)
                {
                    yield return AddGPUIPrefabInstances_Coroutine(prefabs, "SetPrefabRootActive_Coroutine");
                }
                else
                {
                    yield return RemovePrefabsInstances_Coroutine(prefabs);
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

    public IEnumerator AddGPUIPrefabInstances_Coroutine(IEnumerable<GPUInstancerPrefab> prefabs,string tag)
    {
        Debug.Log($"AddGPUIPrefabInstances_Coroutine[{tag}] prefabs:{prefabs.Count()}");
        //GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
        int j = 0;
        //for (int i = 0; i < prefabs.Length; i++)
        foreach(var prefab in prefabs)
        {
            //GPUInstancerPrefab prefab = prefabs[i];
            AddPrefabInstance(prefab);
            if (j > CoroutineSize)
            {
                j = 0;
                //Debug.Log($"AddPrefabsOfRoot_Coroutine[{i}/{prefabs.Length}_{j}/{CoroutineSize}] {root.name} prefab:{prefab.name}");
                yield return null;
            }
            j++;
            //yield return null;
        }
    }

    public void AddGPUIPrefabInstances(IEnumerable<GPUInstancerPrefab> prefabs, string tag)
    {
        Debug.Log($"AddGPUIPrefabInstances[{tag}] prefabs:{prefabs.Count()}");
        //GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
        int j = 0;
        //for (int i = 0; i < prefabs.Length; i++)
        foreach (var prefab in prefabs)
        {
            AddPrefabInstance(prefab);
        }
    }

    public bool isShowLog = false;

    public bool AlwaysGUPI = false;

    public IEnumerator RemovePrefabsInstances_Coroutine(IEnumerable<GPUInstancerPrefab> prefabs)
    {
        if (prefabs != null)
        {
            //GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
            //Debug.Log($"RemovePrefabsOfRoot_Coroutine {root.name} prefabs:{prefabs.Length}");
            int j = 0;
            //for (int i = 0; i < prefabs.Length; i++)
            foreach (var prefab in prefabs)
            {
                //GPUInstancerPrefab prefab = prefabs[i];
                RemovePrefabInstance(prefab);
                if (j > CoroutineSize)
                {
                    j = 0;
                    if (isShowLog)
                    {
                        //Debug.Log($"RemovePrefabsOfRoot_Coroutine[{i}/{prefabs.Length}_{j}/{CoroutineSize}] {root.name} prefab:{prefab.name}");
                    }
                    yield return null;
                }
                j++;
                //Debug.Log($"RemovePrefabsOfRoot_Coroutine[{i}/{prefabs.Length}_{j}/{CoroutineSize}] {root.name} prefab:{prefab.name}");
                //yield return null;
            }
        }
        else
        {
            yield return null;
        }
    }

    public void RemovePrefabsInstances(IEnumerable<GPUInstancerPrefab> prefabs)
    {
        foreach (var prefab in prefabs)
        {
            RemovePrefabInstance(prefab);
        }
    }

    //[ContextMenu("InitAstroidGenerator")]
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

    private void Awake()
    {
        if (IsCreatePrefabsWhenRun)
        {
            CreatePrefabsOfRoots();
        }
    }

    private void Start()
    {
        //MinPrefabInstanceCount = 0;

        InitAstroidGenerator();
        //if (AstroidGenerator.Instance == null)
        //{
        //    Debug.LogError($"Start AstroidGenerator.Instance == null "); 
        //    return;
        //}

        if (IsInitPrefabs)
        {
            InitPrefabs(); //11
        }

        //List<GPUInstancerPrefab> prefabs0 = new List<GPUInstancerPrefab>();
        //if (IsCreatePrefabs)
        //{
        //    prefabs0=CreatePrefabs();
        //}

        //if (IsUseGPU)
        //{
        //    InitializeGPUInstancer();
        //}

        //if (IsAstroidGenerator)
        //{
        //    TestAstroidGenerator(prefabs0);
        //}

        //if (IsStartGPUInstance)
        //{
        //    StartGPUInstance();
        //}
        //else if (IsUseGPU)
        //{
        //    InitializeGPUInstancer();
        //}
        //else if (IsAstroidGenerator)
        //{
        //    TestAstroidGenerator(prefabs0);
        //}

        //if (IsAutoHideShowRoot)
        //{
        //    StartUpdatePrefabRootsVisible();
        //}

        //InitGPUISceneLoadFinishedAction();
        //if (IsAutoGPUI)
        //{
        //    StartGPUInstanceOfTargets();
        //}
    }

    //private void InitGPUISceneLoadFinishedAction()
    //{
    //    foreach(var obj in GPUIRoots)
    //    {
    //        if (obj == null) continue;
    //        SubScene_Single scene = obj.GetComponent<SubScene_Single>();
    //        if (scene != null)
    //        {
    //            scene.LoadFinished += Scene_LoadFinished;
    //        }
    //    }

    //    StartGPUInstance("InitGPUISceneLoadFinishedAction");
    //}

    //private void Scene_LoadFinished(SubScene_Base obj)
    //{
    //    Debug.Log($"GPUInstanceTest.Scene_LoadFinished obj:{obj}");
    //    StartGPUInstance(obj.gameObject);
    //}

    //private void TestAstroidGenerator(List<GPUInstancerPrefab> prefabs0)
    //{
    //    InitAstroidGenerator();
    //    AstroidGenerator.Instance.IsUseGPUOnStart = false;
    //    AstroidGenerator.Instance.asteroidObjects = prefabs0;
    //    AstroidGenerator.Instance.GenerateGos();
    //    AstroidGenerator.Instance.StartGPUInstance();
    //}

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
        Debug.LogError($"GetMeshPrefabDict meshPrefabDict:{meshPrefabDict.Count}");
        return meshPrefabDict;
    }

    //[ContextMenu("InitPrefabInstances")]
    public void InitPrefabInstances()
    {
        //Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
        //var instances = InitPrefabInstances(MeshTarget, meshPrefabDict);

        var targetList = GetTargetList(false);
        var instances = InitPrefabInstances(targetList, GetMeshPrefabDict());
    }

    //[ContextMenu("ClearPrefabInstances")]
    public void ClearPrefabInstances()
    {
        GPUInstancerPrefab[] gpuis = GetGPUInstancerPrefabs();
        foreach (var gpui in gpuis)
        {
            GameObject.DestroyImmediate(gpui);
        }
        Debug.Log($"ClearPrefabInstances gpuis:{gpuis.Length}"); 
    }

    public GameObject MoveTargetRoot = null;

    //[ContextMenu("MovePrefabInstances")]
    public void MovePrefabInstances()
    {
        //if (MoveTargetRoot == null)
        //{
        //    MoveTargetRoot = MeshTarget;
        //    Debug.LogError($"MovePrefabInstances MoveTargetRoot == null MoveTargetRoot:{MoveTargetRoot}");
        //}
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
        List<MeshFilter> meshFilters = GetMeshFilterList(targets);
        return SInitPrefabInstances(meshFilters, meshPrefabDict, minCount, isHide);
    }

    public static List<MeshFilter> GetMeshFilterList(List<GameObject> targets)
    {
        DictList<MeshFilter> meshFilters = new DictList<MeshFilter>();
        foreach (var target in targets)
        {
            if (target == null) continue;
            var mfs = target.GetComponentsInChildren<MeshFilter>(true);
            foreach(var mf in mfs)
            {
                meshFilters.Add(mf);
            }
        }
        return meshFilters.NewList();
    }

    public List<GPUInstancerPrefab> GetGPUIPrefabList(List<GameObject> targets)
    {
        return GetTargetsComponents<GPUInstancerPrefab>(targets);
    }

    private List<T> GetTargetsComponentsEx<T>(bool isAll) where T : Component
    {
        List<GameObject> targetList = GetTargetList(isAll);
        List<T> prefabs = GetTargetsComponents<T>(targetList);
        Debug.Log($"GetTargetsComponentsEx targetList:{targetList.Count} prefabs:{prefabs.Count}");
        return prefabs;
    }

    public static List<T> GetTargetsComponents<T>(List<GameObject> targets) where T : Component
    {
        DictList<T> meshFilters = new DictList<T>();
        foreach (var target in targets)
        {
            if (target == null) continue;
            var mfs = target.GetComponentsInChildren<T>(true);
            foreach (var mf in mfs)
            {
                meshFilters.Add(mf);
            }
        }
        //Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
        //List<GPUInstancerPrefab> instances = InitPrefabInstances(targets, meshPrefabDict);
        return meshFilters.NewList();
    }

    private static List<GPUInstancerPrefab> SInitPrefabInstances(List<MeshFilter> meshFilters, Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict,int minCount,bool isHide)
    {
        List<GPUInstancerPrefab> instances = new List<GPUInstancerPrefab>();
        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(meshFilters);
        //Debug.Log($"SInitPrefabInstances sharedMeshInfos:{sharedMeshInfos.Count}");
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

    bool isStarted = false;

    public void StartGPUInstance(string tag)
    {
        //Debug.Log($"StartGPUInstance({tag}) MeshTarget:{MeshTarget}");

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
        GetGPUIRoots();

        List<GPUInstancerPrefab> prefabs= StartGPUInstanceEx();
        if (IsAutoHideShowRoot)
        {
            RemovePrefabInstancesAsync(prefabs);
        }
    }

    //[ContextMenu("StartGPUInstanceOfMeshTarget")]
    //public void StartGPUInstanceOfMeshTarget()
    //{
    //    Debug.Log($"StartGPUInstanceOfMeshTarget MeshTarget:{MeshTarget}");
    //    if (MeshTarget == null)
    //    {
    //        Debug.LogError($"StartGPUInstance MeshTarget == null");
    //    }
    //    else
    //    {
    //        StartGPUInstance(MeshTarget);
    //    }
    //}

    public GPUInstancerPrefabListRuntimeHandler runtimeHandler;

    //[ContextMenu("RemovePrefabInstancesAsync")]
    public bool RemovePrefabInstancesAsync(List<GPUInstancerPrefab> prefabs)
    {
        if (runtimeHandler == null)
        {
            runtimeHandler = this.gameObject.AddComponent<GPUInstancerPrefabListRuntimeHandler>();
        }
        runtimeHandler.prefabManager = this.prefabManager;
        runtimeHandler.runInThreads = runInThreads;

        if (prefabs==null)
            prefabs = GetGPUIPrefabListOfTarget();
        for (int i = 0; i < prefabs.Count; i++)
        {
            GPUInstancerPrefab prefab = prefabs[i];
            if (insListOfStarted.Contains(prefab))
            {

            }
            else
            {
                Debug.LogError($"GPUInstanceTest.RemovePrefabInstancesAsync[{i + 1}/{prefabs.Count}] insListOfStarted.Contains(prefab) == false prefab:{prefab} path:{prefab.transform.GetPath()}");
            }
        }
        Debug.Log($"GPUInstanceTest.RemovePrefabInstancesAsync[{prefabs.Count}]");
        return runtimeHandler.RemovePrefabInstancesAsync(prefabs);
    }

    //[ContextMenu("AddGPUIPrefabInstancesAsync")]
    public bool AddGPUIPrefabInstancesAsync(List<GPUInstancerPrefab> prefabs)
    {
        if (runtimeHandler == null)
        {
            runtimeHandler = this.gameObject.AddComponent<GPUInstancerPrefabListRuntimeHandler>();
        }
        runtimeHandler.prefabManager = this.prefabManager;
        runtimeHandler.runInThreads = runInThreads;

        

        if (prefabs == null)
        {
            Debug.LogError($"AddGPUIPrefabInstancesAsync prefabs == null");
            prefabs = GetGPUIPrefabListOfTarget();
        }
        List<GPUInstancerPrefab> prefabs2 = new List<GPUInstancerPrefab>();
        for (int i = 0; i < prefabs.Count; i++)
        {
            GPUInstancerPrefab prefab = prefabs[i];
            if (insListOfStarted.Contains(prefab))
            {
                prefabs2.Add(prefab);
            }
            else
            {
                Debug.LogError($"AddGPUIPrefabInstancesAsync[{i + 1}/{prefabs.Count}] insListOfStarted.Contains(prefab) == false prefab:{prefab} path:{prefab.transform.GetPath()}");
            }
        }
        Debug.Log($"AddGPUIPrefabInstancesAsync[{prefabs.Count}][{prefabs2.Count}]");
        prefabs = prefabs2;

        Debug.LogWarning($"AddGPUIPrefabInstancesAsync prefabs:{prefabs.Count}");
        return runtimeHandler.AddPrefabInstancesAsync(prefabs);
    }

    //[ContextMenu("StartGPUInstanceOfTargets")]
    public void StartGPUInstanceOfTargets()
    {
        List<GameObject> targetList = GetTargetList(true);
        Debug.Log($"StartGPUInstanceOfTargets targetList:{targetList.Count}");
        StartGPUInstance(targetList);
    }

    //[ContextMenu("RemovePrefabsInstancesCoroutine")]
    public void RemovePrefabsInstancesCoroutine()
    {
        List<GPUInstancerPrefab> prefabs = GetGPUIPrefabListOfTarget();
        StartCoroutine(RemovePrefabsInstances_Coroutine(prefabs));
    }

    //[ContextMenu("AddPrefabsInstancesCoroutine")]
    public void AddPrefabsInstancesCoroutine()
    {
        List<GPUInstancerPrefab> prefabs = GetGPUIPrefabListOfTarget();
        StartCoroutine(AddGPUIPrefabInstances_Coroutine(prefabs, "AddPrefabsInstancesCoroutine"));
    }

    private List<GPUInstancerPrefab> GetGPUIPrefabListOfTarget()
    {
        //List<GameObject> targetList = GetTargetList();
        //List<GPUInstancerPrefab> prefabs = GetGPUIPrefabList(targetList);
        //Debug.Log($"GetGPUIPrefabListOfTarget targetList:{targetList.Count} prefabs:{prefabs.Count}");
        //return prefabs;
        return GetTargetsComponentsEx<GPUInstancerPrefab>(false);
    }

    //[ContextMenu("StartGPUInstanceOfGPUIRoots")]
    public List<GPUInstancerPrefab> StartGPUInstanceOfGPUIRoots()
    {
        Debug.Log($"StartGPUInstanceOfGPUIRoots GPUIRoots:{GPUIRoots.Count}");
        List<GameObject> targetList = new List<GameObject>();
        foreach (var root in GPUIRoots)
        {
            targetList.Add(root.gameObject);
        }
        return StartGPUInstance(targetList);
    }

    //[ContextMenu("StopGPUInstanceOfGPUIRoots")]
    public void StopGPUInstanceOfGPUIRoots()
    {
        isStarted = false;
        Debug.Log($"StartGPUInstanceOfGPUIRoots GPUIRoots:{GPUIRoots.Count}");
        StartCoroutine(StopGPUInstanceOfGPUIRoots_Coroutine());
    }

    public IEnumerator StopGPUInstanceOfGPUIRoots_Coroutine()
    {
        Debug.Log($"StopGPUInstanceOfGPUIRoots_Coroutine Start GPUIRoots:{GPUIRoots.Count}");
        int count = 0;
        foreach (var root in GPUIRoots)
        {
            if (root == null) continue;
            GPUInstancerPrefab[] prefabs = root.GetComponentsInChildren<GPUInstancerPrefab>(true);
            yield return RemovePrefabsInstances_Coroutine(prefabs);
        }
    }

    public void RemoveOneInstance()
    {
        Debug.Log($"RemoveOneInstance count:{initPrefabs.Count}");
        if (initPrefabs.Count > 0)
        {
            GPUInstancerPrefab prefab = initPrefabs[0];
            initPrefabs.RemoveAt(0);
            RemovePrefabInstance(prefab);
        }
    }

    public void RemoveOnePrefab()
    {
        if (PrefabList.Count > 0)
        {
            var dict = GetPrefabMeshDict();

            var prefabOne = PrefabList[0];
            PrefabList.RemoveAt(0);
            var mfs=GetMeshFilters();

            //Debug.Log($"RemoveOnePrefab MeshFilter:{mfs.Length}");
            //MeshFilter[] mfs = MeshTarget.GetComponentsInChildren<MeshFilter>(true);
            int replaceCount = 0;
            List<GPUInstancerPrefab> prefabs = new List<GPUInstancerPrefab>();
            for (int i = 0; i < mfs.Length; i++)
            {
                MeshFilter mf = mfs[i];
                if (mf == null)
                {
                    continue;
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
                    if(pre== prefabOne)
                    {
                        GPUInstancerPrefab newPre = mf.gameObject.GetComponent<GPUInstancerPrefab>();
                        prefabs.Add(newPre);
                    }
                    //ReplacePrefabInstances(pre, mf.gameObject);
                    //GPUInstancerPrefab newPre = mf.gameObject.GetComponent<GPUInstancerPrefab>();
                    //initPrefabs.Add(newPre);
                    //insCount++;
                }
                else
                {
                    //Debug.LogError($"dict.ContainsKey(mesh)==false mf:{mf}");
                }
            }

            StartCoroutine(RemovePrefabsInstances_Coroutine(prefabs));
            Debug.Log($"RemoveOnePrefab Prefab:{prefabOne} count:{prefabs.Count} MeshFilter:{mfs.Length} dict:{dict.Count}");
        }
    }

    public void RemoveOnePrefabLOD()
    {
        if (LODPrefabList.Count > 0)
        {
            var prefabOne = LODPrefabList[0];
            LODPrefabList.RemoveAt(0);
            //IdDictionary.InitInfos();
            int count = 0;
            int errorCount = 0;
            List<GPUInstancerPrefab> prefabs = new List<GPUInstancerPrefab>();
            foreach (var id in prefabOne.InstanceIds)
            {
                GameObject prefabObj = IdDictionary.GetGo(id);
                if (prefabObj == null)
                {
                    Debug.LogError($"RemoveOnePrefabLOD prefabObj == null id:{id} Prefab:{prefabOne.Prefab}");
                    errorCount++;
                }
                else
                {
                    GPUInstancerPrefab prefab = prefabObj.GetComponent<GPUInstancerPrefab>();
                    if (prefab != null)
                    {
                        prefabs.Add(prefab);
                        count++;
                    }
                    else
                    {
                        Debug.LogError($"RemoveOnePrefabLOD prefab == null id:{id} Prefab:{prefabOne.Prefab}");
                        errorCount++;
                    }
                }
            }
            StartCoroutine(RemovePrefabsInstances_Coroutine(prefabs));
            Debug.Log($"RemoveOnePrefabLOD Prefab:{prefabOne.Prefab} count:{count} errorCount:{errorCount}");
        }
    }

    private int gpuiCount = 0;

    //public List<GPUInstancerPrefab> StartGPUInstance(List<GameObject> targets)
    //{
    //    System.DateTime start = System.DateTime.Now;
    //    Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
    //    List<GPUInstancerPrefab> instances = GetGPUIPrefabList(targets); 

    //    //List<GPUInstancerPrefab> instances = InitPrefabInstances(targets, meshPrefabDict);
    //    //if (instances.Count == 0)
    //    //{
    //    //    Debug.LogError($"StartGPUInstance[{++gpuiCount}] instances.Count == 0 1 targets:{targets.Count} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} ");
    //    //}

    //    if (instances.Count == 0)
    //    {
    //        Debug.LogError($"StartGPUInstance[{++gpuiCount}] instances.Count == 0 2 targets:{targets.Count} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} ");
    //        return instances;
    //    }

    //    if (IsAutoHideShowRoot)
    //    {
    //        StartUpdatePrefabRootsVisible();
    //    }

    //    if (isStarted == true)
    //    {
    //        Debug.LogError($"StartGPUInstance isStarted == true targets:{targets.Count}");
    //        return null;
    //    }
    //    isStarted = true;

    //    AstroidGenerator.Instance.IsUseGPUOnStart = false;
    //    //AstroidGenerator.Instance.asteroidObjects = prefabs0;
    //    //AstroidGenerator.Instance.GenerateGos();
    //    //Debug.Log($"StartGPUInstance[{++gpuiCount}] target:{target} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} path:{target.transform.GetPath()}");
    //    this.StartGPUInstance(instances);
    //    Debug.Log($"StartGPUInstance[{++gpuiCount}] targets:{targets.Count} time:{System.DateTime.Now - start} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} ");
    //    StartedCount = instances.Count;
    //    return instances;
    //}

    //public List<GPUInstancerPrefab> StartGPUInstance(List<GameObject> targets)
    //{
    //    string targetsName = "";
    //    foreach(var target in targets)
    //    {
    //        targetsName += target.name + ";";
    //    }
    //    System.DateTime start = System.DateTime.Now;
    //    Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
    //    List<GPUInstancerPrefab> instances = GetGPUIPrefabList(targets);
    //    if (instances.Count == 0)
    //    {
    //        Debug.LogError($"StartGPUInstance[{++gpuiCount}] instances.Count == 0 2 targets:{targets.Count} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} ");
    //        return instances;
    //    }
    //    if (IsAutoHideShowRoot)
    //    {
    //        StartUpdatePrefabRootsVisible();
    //    }
    //    if (isStarted == true)
    //    {
    //        Debug.LogError($"StartGPUInstance isStarted == true targets:{targets.Count}");
    //        return null;
    //    }
    //    isStarted = true;
    //    AstroidGenerator.Instance.IsUseGPUOnStart = false;
    //    this.StartGPUInstance(instances);
    //    Debug.Log($"StartGPUInstance[{++gpuiCount}] targets:{targets.Count} time:{System.DateTime.Now - start} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} targetsName:{targetsName}");
    //    StartedCount = instances.Count;
    //    return instances;
    //}

    public List<GPUInstancerPrefab> StartGPUInstance(List<GameObject> targets)
    {
        string targetsName = "";
        foreach (var target in targets)
        {
            targetsName += target.name + ";";
        }
        System.DateTime start = System.DateTime.Now;
        if (IsAutoHideShowRoot)
        {
            StartUpdatePrefabRootsVisible();
        }
        if (isStarted == true)
        {
            Debug.LogError($"StartGPUInstance isStarted == true targets:{targets.Count}");
            return null;
        }
        isStarted = true;
        AstroidGenerator.Instance.IsUseGPUOnStart = false;
        var instances = initPrefabs.NewList();
        this.StartGPUInstance(instances);
        Debug.Log($"StartGPUInstance[{++gpuiCount}] targets:{targets.Count} time:{System.DateTime.Now - start} meshPrefabDict:{GetMeshPrefabDict().Count} instances:{instances.Count} targetsName:{targetsName}");
        StartedCount = instances.Count;
        return instances;
    }

    public List<GPUInstancerPrefab> StartGPUInstanceEx()
    {
        System.DateTime start = System.DateTime.Now;
        if (IsAutoHideShowRoot)
        {
            StartUpdatePrefabRootsVisible();
        }
        if (isStarted == true)
        {
            Debug.LogError($"StartGPUInstance isStarted == true");
            return null;
        }
        isStarted = true;
        AstroidGenerator.Instance.IsUseGPUOnStart = false;
        var instances = initPrefabs.NewList();
        this.StartGPUInstance(instances);
        Debug.Log($"StartGPUInstanceEx[{++gpuiCount}] time:{System.DateTime.Now - start} meshPrefabDict:{GetMeshPrefabDict().Count} instances:{instances.Count}");
        StartedCount = instances.Count;
        return instances;
    }

    public int StartedCount = 0;

    private Dictionary<GameObject, List<GPUInstancerPrefab>> instancesDict = new Dictionary<GameObject, List<GPUInstancerPrefab>>();

    public void StartGPUInstanceEx(GameObject keyGo,List<GameObject> targets)
    {
        if (targets.Count == 0) return;
        System.DateTime start = System.DateTime.Now;
        List<GPUInstancerPrefab> instances = null;
        if (instancesDict.ContainsKey(keyGo))
        {
            instances = instancesDict[keyGo];
            AddGPUIPrefabInstances(instances, "StartGPUInstanceEx");
            //Debug.Log($"StartGPUInstanceEx[{++gpuiCount}] AddGPUIPrefabInstances targets:{targets.Count} time:{System.DateTime.Now - start} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} ");
        }
        else
        {
            Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
            instances = InitPrefabInstances(targets, meshPrefabDict);
            if (instances.Count == 0)
            {
                return;
            }
            //AstroidGenerator.Instance.IsUseGPUOnStart = false;
            //AstroidGenerator.Instance.asteroidObjects = prefabs0;
            //AstroidGenerator.Instance.GenerateGos();
            //Debug.Log($"StartGPUInstance[{++gpuiCount}] target:{target} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} path:{target.transform.GetPath()}");
            //AstroidGenerator.Instance.StartGPUInstance(instances);
            this.StartGPUInstance(instances);
            //Debug.Log($"StartGPUInstanceEx[{++gpuiCount}] StartGPUInstance targets:{targets.Count} time:{System.DateTime.Now - start} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} ");
            instancesDict.Add(keyGo, instances);
        }
    }

    public List<GPUInstancerPrefab> GetInstances(GameObject keyGo, List<GameObject> targets)
    {
        List<GPUInstancerPrefab> instances = null;
        if (instancesDict.ContainsKey(keyGo))
        {
            instances = instancesDict[keyGo];
        }
        else
        {
            Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
            instances = InitPrefabInstances(targets, meshPrefabDict);
            instancesDict.Add(keyGo, instances);
        }
        return instances;
    }

    private DictList<GPUInstancerPrefab> insListOfStarted = new DictList<GPUInstancerPrefab>();

    private void InitGPUIPrefabInsListOfStarted(List<GPUInstancerPrefab> instances)
    {
        int c = 0;
        int e = 0;
        foreach (GPUInstancerPrefab prefabInstance in instances)
        {
            c++;
            if (prefabInstance == null)
            {
                Debug.LogError($"StartGPUInstance[{c}] prefabInstance == null");
                continue;
            }
            if (prefabInstance.prefabPrototype == null)
            {
                e++;
                Debug.LogError($"StartGPUInstance[{c}]({e}) prefabInstance.prefabPrototype==null prefabInstance:{prefabInstance} path:{prefabInstance.transform.GetPath()}");
                continue;
            }
            insListOfStarted.Add(prefabInstance);
        }

        Debug.LogError($"InitGPUIPrefabInsListOfStarted insListOfStarted:{insListOfStarted.Count} instances:{instances.Count}");
    }

    public void StartGPUInstance(List<GPUInstancerPrefab> instances)
    {

        //AstroidGenerator.Instance.StartGPUInstance(instances);

        if (prefabManager == null)
        {
            prefabManager = GPUInstancerPrefabManager.Instance;
        }
        if (prefabManager != null && prefabManager.gameObject.activeSelf && prefabManager.enabled)
        {
            //if (instances == null || instances.Count == 0)
            //{
            //    instances = GameObject.FindObjectsOfType<GPUInstancerPrefab>().ToList();
            //}

            prefabManager.GetPrefabInstances(instances, true, "StartGPUInstance");

            InitGPUIPrefabInsListOfStarted(instances);

            if (IsAsync)
            {
                foreach (GPUInstancerPrefab pi in instances)
                {
                    // save transform data before threading
                    pi.GetLocalToWorldMatrix(true);
                }
            }

            GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, instances);

            GPUInstancerAPI.InitializeGPUInstancer(prefabManager);

            //var prefabs = GameObject.FindObjectsOfType<GPUInstancerPrefab>(true);
            Debug.Log($"GUIInstanceTest.StartGPUInstance instances:{instances.Count} insListOfStarted:{insListOfStarted.Count}");
        }
        else
        {
            Debug.LogError($"GUIInstanceTest.StartGPUInstance prefabManager:{prefabManager}");
        }
    }

    //public void StartGPUInstance(GameObject target)
    //{
    //    System.DateTime start = System.DateTime.Now;
    //    Dictionary<Mesh, GPUInstancerPrefab> meshPrefabDict = GetMeshPrefabDict();
    //    var instances = InitPrefabInstances(target, meshPrefabDict);

    //    AstroidGenerator.Instance.IsUseGPUOnStart = false;
    //    //AstroidGenerator.Instance.asteroidObjects = prefabs0;
    //    //AstroidGenerator.Instance.GenerateGos();
    //    //Debug.Log($"StartGPUInstance[{++gpuiCount}] target:{target} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} path:{target.transform.GetPath()}");
    //    this.StartGPUInstance(instances);
    //    Debug.Log($"StartGPUInstance[{++gpuiCount}] target:{target} time:{System.DateTime.Now-start} meshPrefabDict:{meshPrefabDict.Count} instances:{instances.Count} path:{target.transform.GetPath()}");
    //}

    [ContextMenu("StartUpdatePrefabRootsVisible")]
    public void StartUpdatePrefabRootsVisible()
    {
        ClearShowHideList();
        IsAutoHideShowRoot = true;
        StartCoroutine(UpdateGPUIScenesVisible_Coroutine());
        StartCoroutine(AutoUpdatePrefabRootsVisible_Coroutine());
    }

    //[ContextMenu("ShowPrototypeList")]
    public void ShowPrototypeList()
    {
        prefabManager = GameObject.FindObjectOfType<GPUInstancerPrefabManager>();

        Debug.Log($"count:{prefabManager.prototypeList.Count}");
    }

    //[ContextMenu("DisableManager")]
    public void DisableManager()
    {
        this.gameObject.SetActive(false);
    }

    //[ContextMenu("EnableManager")]
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
        for (int i = 0; i < PrefabList.Count; i++)
        {
            GPUInstancerPrefab prefab = PrefabList[i];
            if (prefab == null) continue;
            MeshFilter mf = prefab.GetComponent<MeshFilter>();
            if (mf == null) continue;
            if (mf.sharedMesh == null)
            {
                Debug.LogError($"GetPrefabMeshDict[{i}/{PrefabList.Count}] mf.sharedMesh == null mf:{mf}");
                continue; 
            }
            if (meshes.ContainsKey(mf.sharedMesh))
            {
                Debug.LogError($"GetPrefabMeshDict[{i}/{PrefabList.Count}] meshes.ContainsKey(mf.sharedMesh) mf:{mf}");
            }
            else
            {
                meshes.Add(mf.sharedMesh, prefab);
            }
            
        }
        return meshes;
    }

    public Dictionary<Mesh, GPUInstancerPrefab> GetManagedPrefabMeshDict()
    {
        Dictionary<Mesh, GPUInstancerPrefab> meshes = new Dictionary<Mesh, GPUInstancerPrefab>();
        int count = ManagedPrefabList.Count;
        for (int i = 0; i < ManagedPrefabList.Count; i++)
        {
            GameObject obj = ManagedPrefabList[i];
            if (obj == null) continue;
            GPUInstancerPrefab prefab = obj.GetComponent<GPUInstancerPrefab>();
            if (prefab == null) continue;
            MeshFilter mf = prefab.GetComponent<MeshFilter>();
            if (mf == null) continue;
            if (mf.sharedMesh == null)
            {
                Debug.LogError($"GetPrefabMeshDict[{i}/{count}] mf.sharedMesh == null mf:{mf}");
                continue;
            }
            if (meshes.ContainsKey(mf.sharedMesh))
            {
                Debug.LogError($"GetPrefabMeshDict[{i}/{count}] meshes.ContainsKey(mf.sharedMesh) mf:{mf}");
            }
            else
            {
                meshes.Add(mf.sharedMesh, prefab);
            }

        }
        return meshes;
    }

    public Dictionary<Mesh, GPUInstancerPrefab> GetPrefabRootMeshDict()
    {
        GameObject prefabRoot = GetPrefabsRoot();
        Dictionary<Mesh, GPUInstancerPrefab> meshes = new Dictionary<Mesh, GPUInstancerPrefab>();
        int count = prefabRoot.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform obj = prefabRoot.transform.GetChild(i);
            if (obj == null) continue;
            GPUInstancerPrefab prefab = obj.GetComponent<GPUInstancerPrefab>();
            if (prefab == null) continue;
            MeshFilter mf = prefab.GetComponent<MeshFilter>();
            if (mf == null) continue;
            if (mf.sharedMesh == null)
            {
                Debug.LogError($"GetPrefabRootMeshDict[{i}/{count}] mf.sharedMesh == null mf:{mf}");
                continue;
            }
            if (meshes.ContainsKey(mf.sharedMesh))
            {
                Debug.LogError($"GetPrefabRootMeshDict[{i}/{count}] meshes.ContainsKey(mf.sharedMesh) mf:{mf}");
            }
            else
            {
                meshes.Add(mf.sharedMesh, prefab);
            }

        }
        return meshes;
    }

    //[ContextMenu("ShowAll")]
    public void ShowAll()
    {
        MeshFilter[] mfs = GetMeshFilters();
        foreach (var mf in mfs)
        {
            mf.gameObject.SetActive(true);
        }
        Debug.Log($"ShowAll mfs:{mfs.Length}");
    }

    //[ContextMenu("UnpackAll")]
    public void UnpackAll()
    {
        MeshFilter[] mfs = GetMeshFilters();
        foreach (var mf in mfs)
        {
            EditorHelper.UnpackPrefab(mf.gameObject);
        }
        Debug.Log($"UnpackAll mfs:{mfs.Length}");
    }

    //[ContextMenu("ClearPrefabScrips")]
    public void ClearPrefabScrips()
    {
        GPUInstancerPrefab[] mfs = GetGPUInstancerPrefabs();
        foreach (var mf in mfs)
        {
           GameObject.DestroyImmediate(mf);
        }
        Debug.Log($"ClearPrefabScrips GPUInstancerPrefab:{mfs.Length}");
    }

    //[ContextMenu("HideNotInPrefabs")]
    public void HideNotInPrefabs()
    {
        var meshes = GetPrefabMeshDict();
        MeshFilter[] mfs = GetMeshFilters();
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

    //[ContextMenu("HideInPrefabs")]
    public void HideInPrefabs()
    {
        var meshes = GetPrefabMeshDict();
        MeshFilter[] mfs = GetMeshFilters();
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

    //[ContextMenu("DistroyInactive")]
    public void DistroyInactive()
    {
        MeshFilter[] mfs = GetMeshFilters();
        int count = 0;
        foreach (var mf in mfs)
        {
            if (mf == null) continue;
            if (mf.gameObject.activeInHierarchy == false)
            {
                EditorHelper.UnpackPrefab(mf.gameObject);
                GameObject.DestroyImmediate(mf.gameObject);
                count++;
            }
        }
        Debug.Log($"DistroyInactive count:{count} mfs:{mfs.Length}");
    }

    //[ContextMenu("DistroyInBuild")]
    public void DistroyInBuild()
    {
        Transform[] mfs = MeshTarget.GetComponentsInChildren<Transform>(true);
        List<BuildingController> bs=new List<BuildingController>();
        int count = 0;
        foreach (var mf in mfs)
        {
            if (mf == null) continue;
            if (mf.gameObject.tag=="LoadInBuildObj")
            {
                BuildingController b=mf.gameObject.GetComponentInParent<BuildingController>();
                if(b!=null){
                    if(!bs.Contains(b)){
                        bs.Add(b);
                    }
                }
                
                count++;
                Debug.Log($"Distroy[{count}] name:{mf.name} path:{mf.transform.GetPath()}");

                EditorHelper.UnpackPrefab(mf.gameObject);
                GameObject.DestroyImmediate(mf.gameObject);
            }
        }
        Debug.Log($"DistroyInBuild count:{count} mfs:{mfs.Length} bs:{bs.Count}");
    }

    public class GPUIRootState
    {
        public IGPUIRoot Root;

        public bool Visible;

        public GPUIRootState(IGPUIRoot root, bool visible)
        {
            Root = root;
            Visible = visible;
        }
    }
}
