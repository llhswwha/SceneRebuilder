using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModelSceneLoader : MonoBehaviour
{
    public string nextSceneName;

    public List<string> sceneList = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        //progress = GetComponent<Text>();
        //slider = FindObjectOfType<Slider>();
       
    }

    public void ClickStartLoad()
    {
        Debug.Log("ClickStartLoad");
        int index = DropdownSceneList.value;
        nextSceneName = sceneList[index];
        StartCoroutine("LoadScene");
    }

    public void ClickStartUnload()
    {
        Debug.Log("ClickStartUnload");
        int index = DropdownSceneList.value;
        nextSceneName = sceneList[index];
        StartCoroutine("UnLoadScene");
    }

    public void ClickLoadAll()
    {
        Debug.Log("ClickLoadAll");
        StartCoroutine("LoadAllScene");
    }

    public int MaxCount = 2;

    IEnumerator LoadAllScene()
    {
        DateTime start = DateTime.Now;
        for (int i=0;i< DropdownSceneList.options.Count && i< MaxCount; i++)
        {
            DropdownSceneList.value = i;
            nextSceneName = sceneList[i];
            yield return LoadScene();
            yield return new WaitForSeconds(0.1f);
        }
        Debug.LogError($"LoadAllScene time:{(DateTime.Now - start).ToString()}");
        yield return null;
    }

    IEnumerator UnLoadScene()
    {
        Debug.Log("UnLoadScene:" + nextSceneName);

        async = SceneManager.UnloadSceneAsync(nextSceneName, UnloadSceneOptions.None);
        //async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress < 0.9f)
                progressValue = async.progress;
            else
                progressValue = 1.0f;

            slider.value = progressValue;
            progress.text = (int)(slider.value * 100) + " %";

            //if (progressValue >= 0.9)
            //{
            //    progress.text = "�����������";
            //    if (Input.anyKeyDown)
            //    {
            //        async.allowSceneActivation = true;
            //    }
            //}
            yield return null;
        }

        ShowModelInfo();

        yield return null;
    }

    public int lastRenderCount;

    public int lastVertextCount;

    public int lastMatCount;

    private void ShowModelInfo()
    {
        int renderCount = 0;
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>();
        int vertextCount = 0;
        List<Material> mats = new List<Material>();
        foreach (var render in renderers)
        {
            if (render.enabled == false) continue;
            if (!mats.Contains(render.sharedMaterial))
            {
                mats.Add(render.sharedMaterial);
            }
            MeshFilter meshFilter = render.GetComponent<MeshFilter>();
            if (meshFilter == null) continue;
            if (meshFilter.sharedMesh == null) continue;
            vertextCount+=meshFilter.sharedMesh.vertexCount;
            renderCount++;
        }
        int w = vertextCount / 10000;

        ModelInfoText.text = $"renders:{renderCount}(+{renderCount-lastRenderCount}),mats:{mats.Count}(+{mats.Count-lastMatCount}) \nvertext:{w}w(+{w-lastVertextCount})";

        lastRenderCount = renderCount;
        lastVertextCount = w;
        lastMatCount = mats.Count;

        AreaTreeNodeShowManager.Instance.Init();
    }

    AsyncOperation async;

    public float progressValue;

    public Text progress;

    public Text ModelInfoText;

    public Slider slider;

    public Dropdown DropdownSceneList;

    IEnumerator LoadScene()
    {
        DateTime start = DateTime.Now;
        Debug.Log("LoadScene:"+ nextSceneName);

        async = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        //async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress < 0.9f)
                progressValue = async.progress;
            else
                progressValue = 1.0f;

            slider.value = progressValue;
            progress.text = (int)(slider.value * 100) + " %";

            //if (progressValue >= 0.9)
            //{
            //    progress.text = "�����������";
            //    if (Input.anyKeyDown)
            //    {
            //        async.allowSceneActivation = true;
            //    }
            //}
            yield return null;
        }
        ShowModelInfo();
        Debug.Log($"LoadScene[{nextSceneName}] time:{(DateTime.Now - start).ToString()}");
        yield return null;
    }
}
