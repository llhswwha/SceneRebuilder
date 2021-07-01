using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSceneShowManagerUI : MonoBehaviour
{
    public Text txtInfo;

    public SubSceneShowManager subSceneShowManager;

    public string sceneCountInfo="";
    private void Start()
    {
        subSceneShowManager = GameObject.FindObjectOfType<SubSceneShowManager>(true);
        sceneCountInfo=subSceneShowManager.GetSceneCountInfo()+"\n"+subSceneShowManager.GetShowSceneCountInfo();
    }

    public void Update()
    {
        if(subSceneShowManager)
        {
            txtInfo.text = $"{sceneCountInfo}\n{subSceneShowManager.GetDisInfo()}\n{subSceneShowManager.GetSceneInfo()}";
        }
        
    }

    public void HideAllRenderers()
    {

    }

    public void ShowAllRenderers()
    {

    }

    [ContextMenu("LoadStartScenes")]
    public void LoadStartScenes()
    {
        Debug.Log("LoadStartScenes");
        subSceneShowManager.LoadStartScens();
    }

   [ContextMenu("LoadOut0TreeScenes")]
    public void LoadOut0TreeScenes()
    {
        Debug.Log("LoadOut0TreeScenes");
        subSceneShowManager.LoadOut0TreeScenes();
    }

    [ContextMenu("LoadOut0TreeNodeScenes")]
    public void LoadOut0TreeNodeScenes()
    {
        Debug.Log("LoadOut0TreeNodeScenes");
        subSceneShowManager.LoadOut0TreeNodeScenes();
    }

    [ContextMenu("CloseSceneListUI")]
    public void CloseSceneListUI()
    {
        SubSceneManagerUI subSceneManagerUI=GameObject.FindObjectOfType<SubSceneManagerUI>(true);
        subSceneManagerUI.gameObject.SetActive(false);
    }

    [ContextMenu("OpenSceneListUI")]
    public void OpenSceneListUI()
    {
        SubSceneManagerUI subSceneManagerUI=GameObject.FindObjectOfType<SubSceneManagerUI>(true);
        subSceneManagerUI.gameObject.SetActive(true);
    }
}
