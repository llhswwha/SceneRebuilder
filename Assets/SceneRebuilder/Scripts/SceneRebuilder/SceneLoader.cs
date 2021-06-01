using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
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
        int index = DropdownSceneList.value;
        nextSceneName = sceneList[index];
        StartCoroutine("LoadScene");
    }

    public void ClickStartUnload()
    {
        int index = DropdownSceneList.value;
        nextSceneName = sceneList[index];
        StartCoroutine("UnLoadScene");
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
            //    progress.text = "按任意键继续";
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
            vertextCount+=meshFilter.sharedMesh.vertexCount;
            renderCount++;
        }
        int w = vertextCount / 10000;

        ModelInfoText.text = $"renders:{renderCount}(+{renderCount-lastRenderCount}),mats:{mats.Count}(+{mats.Count-lastMatCount}) \nvertext:{w}w(+{w-lastVertextCount})";

        lastRenderCount = renderCount;
        lastVertextCount = w;
        lastMatCount = mats.Count;
    }

    AsyncOperation async;

    public float progressValue;

    public Text progress;

    public Text ModelInfoText;

    public Slider slider;

    public Dropdown DropdownSceneList;

    IEnumerator LoadScene()
    {
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
            //    progress.text = "按任意键继续";
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
}
