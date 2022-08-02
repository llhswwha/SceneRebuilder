using GPUInstancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUIRoot : MonoBehaviour, IGPUIRoot
{
    public bool IsDebug = false;

    public int PrefabCount = 0;
    public bool activeInHierarchy
    {
        get
        {
            return gameObject.activeInHierarchy;
        }
    }

    private GPUInstancerPrefab[] _prefabs = null;

    public GPUInstancerPrefab[] GetGPUIPrefabs()
    {
        if (_prefabs == null || _prefabs.Length == 0)
        {
            _prefabs = this.GetComponentsInChildren<GPUInstancerPrefab>(true);
            PrefabCount = _prefabs.Length;
        }
        return _prefabs;
    }

    [ContextMenu("TestGetGPUIPrefabs")]
    public void TestGetGPUIPrefabs()
    {
        var prefabs = GetGPUIPrefabs();
        Debug.Log($"TestGetGPUIPrefabs prefabs:{prefabs.Length}");
    }

    [ContextMenu("ClearPrefabs")]
    public void ClearPrefabs()
    {

    }

    [ContextMenu("CreatePrefabs")]
    public void CreatePrefabs()
    {

    }

    private void Start()
    {
        if (IsLoadByScene == false)
        {
            GPUInstanceTest.Instance.RegistGPUI(this);
        }
    }

    private void OnDisable()
    {
        HideRenderers();
    }

    private void OnEnable()
    {
        ShowRenderers();
    }

    [ContextMenu("HideRenderers")]
    private void HideRenderers()
    {
        if (IsDebug)
        {
            Debug.LogError($"GPUIRoot[{this.name}] OnDisable");
        }
        if (IsLoadByScene == false)
        {
            GPUInstanceTest.Instance.AddToHideScene(this, IsDebug);
        }
    }

    [ContextMenu("ShowRenderers")]
    private void ShowRenderers()
    {
        if (IsDebug)
        {
            Debug.LogError($"GPUIRoot[{this.name}] OnEnable");
        }
        if (IsLoadByScene == false)
        {
            GPUInstanceTest.Instance.AddToShowScene(this, IsDebug);
        }
    }

    public bool IsLoadByScene = true;
}

public interface IGPUIRoot
{
    GPUInstancerPrefab[] GetGPUIPrefabs();
}
