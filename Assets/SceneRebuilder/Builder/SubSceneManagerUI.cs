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

    public SubSceneManager subSceneManager;

    public RectTransform panelSceneList;

    public GameObject subSceneUIPrefab;

    void Start()
    {
        subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        subSceneManager.ProgressChanged += SubSceneManager_ProgressChanged;
        subSceneManager.AllLoaded += SubSceneManager_AllLoaded;

        dropdownSceneType.onValueChanged.AddListener(OnSceneTypeChanged);

        ClickGetScenes();
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
        ClickGetScenes();
    }

    private void SubSceneManager_ProgressChanged(float obj)
    {
        sliderProgress.value = obj;
    }

    public SubSceneType sceneType=SubSceneType.Single;

    public List<SubScene_Base> subScenes = new List<SubScene_Base>();

    public void ClickGetScenes()
    {
        if (sceneType== SubSceneType.In)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_In>().ToList();
            ShowSceneList(scenes);
        }
        else if (sceneType == SubSceneType.Out0)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Out0>().ToList();
            ShowSceneList(scenes);
        }
        else if (sceneType == SubSceneType.Out1)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Out1>().ToList();
            ShowSceneList(scenes);
        }
        else if (sceneType == SubSceneType.Part)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Part>().ToList();
            ShowSceneList(scenes);
        }
        else if (sceneType == SubSceneType.Single)
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Single>().ToList();
            ShowSceneList(scenes);
        }
        else
        {
            var scenes = GameObject.FindObjectsOfType<SubScene_Base>().ToList();
            ShowSceneList(scenes);
        }

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
