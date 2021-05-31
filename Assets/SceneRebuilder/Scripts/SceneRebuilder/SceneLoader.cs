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

    AsyncOperation async;

    public float progressValue;

    public Text progress;

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

        yield return null;
    }
}
