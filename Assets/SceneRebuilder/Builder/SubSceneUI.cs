using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSceneUI : MonoBehaviour
{
    public Toggle toggleLoadUnload;

    public Text txtName;

    public Toggle toggleIsLoaded;

    public Text txtLog;

    public Slider sliderProgresss;

    // Start is called before the first frame update
    void Start()
    {
        toggleLoadUnload.onValueChanged.AddListener(OnLoadUnLoad);
    }

    void OnLoadUnLoad(bool isOn)
    {
        Debug.Log("OnLoadUnLoad:"+ isOn);
        if (isOn)
        {
            scene.LoadSceneAsync(null);
        }
        else
        {
            scene.UnLoadSceneAsync();
            toggleIsLoaded.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SubScene_Base scene;

    internal void SetScene(int id,SubScene_Base subScene)
    {
        txtName.text = $"[{id:000}]{subScene.GetSceneInfo()}";
        scene = subScene;
        scene.ProgressChanged += Scene_ProgressChanged;
        scene.AllLoaded += Scene_AllLoaded;

        toggleIsLoaded.isOn = scene.IsLoaded;
        txtLog.text = scene.Log;
        sliderProgresss.value = scene.loadProgress;
    }

    private void OnDestroy()
    {
        scene.ProgressChanged -= Scene_ProgressChanged;
        scene.AllLoaded -= Scene_AllLoaded;
    }

    private void Scene_AllLoaded()
    {
        Debug.Log("Scene_AllLoaded:" + scene.Log+"|"+scene.loadProgress);
        txtLog.text = scene.Log;
        toggleIsLoaded.isOn = true;
    }

    private void Scene_ProgressChanged(float obj)
    {
        //Debug.Log("Scene_ProgressChanged:"+obj);
        sliderProgresss.value = obj;
    }
}
