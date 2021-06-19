using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSceneManagerUI : MonoBehaviour
{
    public Text txtResult;

    public Toggle toggleIsOneCoroutine;

    public Slider sliderProgress;

    public SubSceneManager subSceneManager;
    void Start()
    {
        subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        subSceneManager.ProgressChanged += SubSceneManager_ProgressChanged;
        subSceneManager.AllLoaded += SubSceneManager_AllLoaded;
    }

    private void SubSceneManager_AllLoaded()
    {
        txtResult.text = subSceneManager.Log;
    }

    private void SubSceneManager_ProgressChanged(float obj)
    {
        sliderProgress.value = obj;
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
