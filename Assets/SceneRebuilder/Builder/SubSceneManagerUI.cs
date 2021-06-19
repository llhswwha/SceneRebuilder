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

    public SubSceneManager subSceneManager;

    public RectTransform panelSceneList;

    public GameObject subSceneUIPrefab;

    void Start()
    {
        subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        subSceneManager.ProgressChanged += SubSceneManager_ProgressChanged;
        subSceneManager.AllLoaded += SubSceneManager_AllLoaded;

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

    public void ClickGetScenes()
    {
        var subScenes = GameObject.FindObjectsOfType<SubScene>().ToList();
        subScenes.Sort((a, b) => b.vertexCount.CompareTo(a.vertexCount));
        for (int i=0;i<subScenes.Count;i++)
        {
            var subScene = subScenes[i];
            var uiItem = GameObject.Instantiate(subSceneUIPrefab);
            uiItem.SetActive(true);
            SubSceneUI ui = uiItem.GetComponent<SubSceneUI>();
            ui.SetScene(i+1,subScene);
            uiItem.transform.SetParent(panelSceneList);
        }
    }

    public void ClickLoadAll()
    {
        
        subSceneManager.IsOneCoroutine = toggleIsOneCoroutine.isOn;
        subSceneManager.LoadScenesEx();
    }

    public void ClickUnloadAll()
    {
        subSceneManager.UnLoadScenesAsync();
    }
}
