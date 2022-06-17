using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_GPUI : SubScene_Part
{
    protected override string BoundsName
    {
        get
        {
            return this.name + "_Bounds_In_" + contentType;
        }
    }

    public override void HideObjects()
    {
        //base.HideObjects();
        if (IsVisible == false) return;
        IsVisible = false;
        if (gos != null)
        {
            //List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
            //GPUInstanceTest.Instance.PrefabsOfHide.AddRange(prefabs);
            GPUInstanceTest.Instance.ScenesOfHide.Add(this);
        }
    }

    public override void ShowObjects()
    {
        //base.ShowObjects();
        if (IsVisible) return;
        IsVisible = true;
        if (gos != null)
        {
            //List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
            //foreach(var prefab in prefabs)
            //{
            //    prefab.gameObject.SetActive(true);
            //}
            //GPUInstanceTest.Instance.PrefabsOfShow.AddRange(prefabs);

            GPUInstanceTest.Instance.ScenesOfShow.Add(this);
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

    public void StartGPUI1()
    {
        //List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        GPUInstanceTest.Instance.StartGPUInstanceEx(this.gameObject, gos);
    }

    public void StartGPUI2()
    {
        List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        GPUInstanceTest.Instance.ScenesOfShow.Add(this);
    }

    public void StopGPUI1()
    {
        List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        StartCoroutine(GPUInstanceTest.Instance.RemovePrefabsInstances_Coroutine(prefabs));
    }

    public void StopGPUI2()
    {
        List<GPUInstancerPrefab> prefabs = GetSceneGPUIPrefabs();
        GPUInstanceTest.Instance.ScenesOfHide.Add(this);
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
                renderers.AddRange(subRenderers);
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
}
