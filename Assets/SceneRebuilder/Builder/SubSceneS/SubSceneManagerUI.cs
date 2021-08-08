using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SubSceneManagerUI : MonoBehaviour
{
    public Text txtResult;

    public InputField inputFieldMaxCoroutineCount;

    public Slider sliderProgress;

    public Dropdown dropdownSceneType;

    public Dropdown dropdownShownType;

    public Toggle ToggleDynamicShow;

    public SubSceneManager subSceneManager;

    public SubSceneShowManager subSceneShowManager;

    public RectTransform panelSceneList;

    public RectTransform panelSceneList_Loaded;

    public GameObject panelSceneList_RootGo;

    public GameObject panelSceneList_Loaded_RootGo;

    public GameObject subSceneUIPrefab;

    void Start()
    {


        subSceneManager = GameObject.FindObjectOfType<SubSceneManager>(true);
        subSceneManager.ProgressChanged += SubSceneManager_ProgressChanged;
        subSceneManager.AllLoaded += SubSceneManager_AllLoaded;

        dropdownSceneType.onValueChanged.AddListener(OnSceneTypeChanged);

        subSceneShowManager = GameObject.FindObjectOfType<SubSceneShowManager>(true);
        ToggleDynamicShow.onValueChanged.AddListener(OnDynamicShowChanged);

        //ClickGetScenes();

        var allScenes = GameObject.FindObjectsOfType<SubScene_Base>(true);
        Debug.Log($"SubSceneManager.Start allScenes:{allScenes.Length}");
        foreach (var scene in allScenes)
        {
            scene.ProgressChanged += Scene_ProgressChanged;
            scene.UnLoadFinished += Scene_UnLoadFinished;
        }

        //toggleIsOneCoroutine.isOn=subSceneManager.IsOneCoroutine;
    }

    private void Scene_UnLoadFinished(SubScene_Base obj)
    {
        var ui = sceneUIList.Find(i => i.scene == obj);
        if (ui != null)
        {
            sceneUIList.Remove(ui);
            GameObject.DestroyImmediate(ui.gameObject);
            loadedScene.Remove(obj);
        }
    }

    private List<SubScene_Base> loadedScene = new List<SubScene_Base>();

    private List<SubSceneUI> sceneUIList = new List<SubSceneUI>();

    public int MaxListCount=10;

    private void Scene_ProgressChanged(float arg1, SubScene_Base arg2)
    {
        
        if(!loadedScene.Contains(arg2))
        {
            //Debug.Log($"SubSceneManagerUI.Scene_ProgressChanged scene:{arg2} progress:{arg1}");

            loadedScene.Add(arg2);

            //if(sceneUIList.Count<MaxListCount)
            {
                sceneUIList.Add(CreateSceneUIItem(arg2, loadedScene.Count, panelSceneList_Loaded));

                //List<SubSceneUI> sceneUIListNew = new List<SubSceneUI>();
                int j=0;
                for (int i = sceneUIList.Count-1; i >=0; i--)
                {
                    SubSceneUI ui = sceneUIList[i];
                    if (ui == null) continue;
                    ui.transform.SetParent(null);
                    ui.transform.SetParent(panelSceneList_Loaded);

                    j++;
                    if(j>MaxListCount){
                        // SubSceneUI ui = sceneUIList[i];
                        // ui.transform.SetParent(null);
                        // GameObject.Destroy(ui.gameObject);
                        // // sceneUIListNew.Add(ui);
                        // sceneUIList.RemoveAt(i);
                        // i++;

                        ui.gameObject.SetActive(false);
                    }
                    else
                    {
                        
                    }
                }
            }


        }
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

        //ClickGetScenes();
    }

    private void SubSceneManager_AllLoaded()
    {
        txtResult.text = subSceneManager.Log;
    }

    private void SubSceneManager_ProgressChanged(float obj)
    {
        //Debug.LogError($"SubSceneManager_ProgressChanged progress:{obj}");
        sliderProgress.value = obj;
        txtResult.text = subSceneManager.Log;
    }

    public SubSceneType sceneType=SubSceneType.Out0;

    public List<SubScene_Base> subScenes = new List<SubScene_Base>();

    public void ClickGetScenes()
    {
        var scenes = SubSceneManager.GetSubScenes(sceneType);
        var list = scenes.ToList();
        if (dropdownShownType.value == 0)
        {

        }
        else if (dropdownShownType.value == 1)
        {
            list=list.Where(i => subSceneShowManager.scenes_TreeNode_Shown.Contains(i)).ToList();
        }
        else if (dropdownShownType.value == 2)
        {
            list = list.Where(i => subSceneShowManager.scenes_TreeNode_Hidden.Contains(i)).ToList();
        }
        ShowSceneList(list);

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
            CreateSceneUIItem(subScene,i+1, panelSceneList);
        }
    }

    public SubSceneUI CreateSceneUIItem(SubScene_Base subScene,int id,Transform parent)
    {
        var uiItem = GameObject.Instantiate(subSceneUIPrefab);
        itemList.Add(uiItem);
        uiItem.SetActive(true);
        SubSceneUI ui = uiItem.GetComponent<SubSceneUI>();
        ui.SetScene(id, subScene);
        uiItem.transform.SetParent(parent);

        subScenes.Add(subScene);
        return ui;
    }

    public void SetLoadingSceneMaxCount()
    {
        subSceneManager.LoadingSceneMaxCount = int.Parse( inputFieldMaxCoroutineCount.text);
    }

    public void ClickLoadAll()
    {
        Debug.Log("ClickLoadAll");
        SetLoadingSceneMaxCount();
        subSceneManager.LoadScenesEx(subScenes.ToArray(),null);
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

    public void ResetScene()
    {
        Debug.Log("ResetScene");
        Scene scene = SceneManager.GetSceneAt(0);
        SceneManager.LoadScene(scene.name,LoadSceneMode.Single);
    }
}
