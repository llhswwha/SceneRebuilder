using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SubSceneManagerUI : MonoBehaviour
{
    public Text txtResult;

    public Toggle toggleIsOneCoroutine;

    public Slider sliderProgress;

    public Dropdown dropdownSceneType;

    public Toggle ToggleDynamicShow;

    public SubSceneManager subSceneManager;

    public SubSceneShowManager subSceneShowManager;

    public RectTransform panelSceneList;

    public GameObject subSceneUIPrefab;

    void Start()
    {
        subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        subSceneManager.ProgressChanged += SubSceneManager_ProgressChanged;
        subSceneManager.AllLoaded += SubSceneManager_AllLoaded;

        dropdownSceneType.onValueChanged.AddListener(OnSceneTypeChanged);

        subSceneShowManager = GameObject.FindObjectOfType<SubSceneShowManager>();
        ToggleDynamicShow.onValueChanged.AddListener(OnDynamicShowChanged);

        ClickGetScenes();
    }

    private void OnDynamicShowChanged(bool isOn)
    {
        Debug.Log("OnDynamicShowChanged:" + isOn);

        subSceneShowManager.gameObject.SetActive(isOn);
    }

    private void OnSceneTypeChanged(int i)
    {
        Debug.Log("OnSceneTypeChanged:"+i);
        sceneType= (SubSceneType)i;

        ClickGetScenes();
    }

    private void SubSceneManager_AllLoaded()
    {
        txtResult.text = subSceneManager.Log;
    }

    private void SubSceneManager_ProgressChanged(float obj)
    {
        sliderProgress.value = obj;
    }

    public SubSceneType sceneType=SubSceneType.Single;

    public List<SubScene_Base> subScenes = new List<SubScene_Base>();

    public void ClickGetScenes()
    {
        var scenes = SubSceneManager.GetSubScenes(sceneType);
        ShowSceneList(scenes.ToList());

        txtResult.text = $"type:{sceneType},count:{subScenes.Count}";
    }

    public List<GameObject> itemList = new List<GameObject>();

    public void ShowSceneList<T>(List<T> scenes) where T :SubScene_Base
    {
        subScenes.Clear();

        foreach(var item in itemList)
        {
            GameObject.DestroyImmediate(item);
        }
        itemList.Clear();

        scenes.Sort((a, b) => b.vertexCount.CompareTo(a.vertexCount));
        for (int i = 0; i < scenes.Count; i++)
        {
            var subScene = scenes[i];
            if (subScene == null) continue;
            var uiItem = GameObject.Instantiate(subSceneUIPrefab);
            itemList.Add(uiItem);
            uiItem.SetActive(true);
            SubSceneUI ui = uiItem.GetComponent<SubSceneUI>();
            ui.SetScene(i + 1, subScene);
            uiItem.transform.SetParent(panelSceneList);

            subScenes.Add(subScene);
        }
    }

    public void ClickLoadAll()
    {
        Debug.Log("ClickLoadAll");
        subSceneManager.IsOneCoroutine = toggleIsOneCoroutine.isOn;
        subSceneManager.LoadScenesEx(subScenes.ToArray());
    }

    public void ClickUnloadAll()
    {
        Debug.Log("ClickUnloadAll");
        subSceneManager.UnLoadScenesAsync(subScenes.ToArray());
        StartCoroutine(UnloadResources());
    }

    public IEnumerator UnloadResources()
    {
        yield return Resources.UnloadUnusedAssets();

        System.GC.Collect();
    }
}
