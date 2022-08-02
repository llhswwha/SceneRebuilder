using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_GPUI : SubScene_Part, IGPUIRoot
{
//#if UNITY_EDITOR
    public void OnDisable()
    {
        //Debug.LogError($"GPUI_OnDisable path:{this.transform.GetPath()} IsVisible:{IsVisible}");
        HideObjects();
    }

    public void OnEnable()
    {
        //Debug.LogError($"GPUI_OnEnable path:{this.transform.GetPath()}  IsVisible:{IsVisible}");
        //ShowObjects();
    }
//#endif

    protected override string BoundsName
    {
        get
        {
            return this.name + "_Bounds_In_" + contentType;
        }
    }

    public override void HideObjects()
    {
       
        if(GPUInstanceTest.IsGPUIEnabled == false)
        {
            base.HideObjects();
        }
        else
        {
            if (IsVisible == false) return;
            IsVisible = false;
            if (gos != null)
            {
                //List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
                //GPUInstanceTest.Instance.PrefabsOfHide.AddRange(prefabs);
                GPUInstanceTest.Instance.AddHideScene(this);
            }
        }
    }

    public override void ShowObjects()
    {
        if (GPUInstanceTest.IsGPUIEnabled == false)
        {
            base.ShowObjects();
        }
        else
        {
            if (IsVisible) return;
            IsVisible = true;
            if (gos != null)
            {
                List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
                foreach (var prefab in prefabs)
                {
                    prefab.gameObject.SetActive(true);
                }
                //GPUInstanceTest.Instance.PrefabsOfShow.AddRange(prefabs);

                GPUInstanceTest.Instance.AddShowScene(this);
            }
        }
    }

    public void SetObjectActive(bool isActive)
    {
        List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        foreach (var prefab in prefabs)
        {
            prefab.gameObject.SetActive(isActive);
        }
    }


    public override void GetSceneObjects()
    {
        base.GetSceneObjects();
        GPUInstanceTest.Instance.RegistGPUIScene(this);
    }

    public void StartGPUI1()
    {
        //List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        GPUInstanceTest.Instance.StartGPUInstanceEx(this.gameObject, gos);
    }

    public void StartGPUI2()
    {
        List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        GPUInstanceTest.Instance.AddShowScene(this);
    }

    public void StopGPUI1()
    {
        List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        StartCoroutine(GPUInstanceTest.Instance.RemovePrefabsInstances_Coroutine(prefabs));
    }

    public void StopGPUI2()
    {
        List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        GPUInstanceTest.Instance.AddHideScene(this);
    }

    private List<GPUInstancerPrefab> gpuiPrefabs = new List<GPUInstancerPrefab>();

    public List<GPUInstancerPrefab> GetSceneGPUIPrefabs()
    {
        if (gpuiPrefabs.Count == 0)
        {
            List<GPUInstancerPrefab> renderers = new List<GPUInstancerPrefab>();
            foreach (var go in gos)
            {
                if (go == null) continue;
                SubScene_Base scene = go.GetComponent<SubScene_Base>();
                if (scene != null)
                {
                    continue;
                }
                GPUInstancerPrefab[] subRenderers = go.GetComponentsInChildren<GPUInstancerPrefab>(true);
                //renderers.AddRange(subRenderers);
                try
                {
                    for (int i = 0; i < subRenderers.Length; i++)
                    {
                        GPUInstancerPrefab prefab = subRenderers[i];
                        if (prefab.prefabPrototype == null)
                        {
                            Debug.LogError($"GetSceneGPUIPrefabs_prefabPrototype == null[{i + 1}/{subRenderers.Length}] Path:{transform.GetPath()} prefab:{prefab} path:{prefab.transform.GetPath()}");
                        }
                        else
                        {
                            //allPrefabs.Add(prefab);
                            renderers.Add(prefab);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"GetSceneGPUIPrefabs_prefabPrototype == null[{subRenderers.Length}] Path:{transform.GetPath()} Exception:{ex}");
                }


                //go.SetActive(true);
            }
            gpuiPrefabs = renderers;
        }
        return gpuiPrefabs;
    }

    //public override void UnLoadGos()
    //{
    //    base.UnLoadGos();
    //}

    //public override int UnLoadGosM()
    //{
    //    return base.UnLoadGosM();
    //}

    //public override bool CanLoad()
    //{
    //    return base.CanLoad();
    //}

    public override bool CanUnload()
    {
        return false;
    }

    public GPUInstancerPrefab[] GetGPUIPrefabs()
    {
        return GetSceneGPUIPrefabs().ToArray();
    }
}
