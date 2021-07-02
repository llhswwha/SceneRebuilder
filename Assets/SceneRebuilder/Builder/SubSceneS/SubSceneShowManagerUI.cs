using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSceneShowManagerUI : MonoBehaviour
{
    public SubSceneManagerUI subSceneManagerUI;

    public Text txtInfo;

    public SubSceneShowManager subSceneShowManager;

    public string sceneCountInfo="";
    private void Start()
    {
        subSceneManagerUI=GameObject.FindObjectOfType<SubSceneManagerUI>(true);

        subSceneShowManager = GameObject.FindObjectOfType<SubSceneShowManager>(true);
        sceneCountInfo=subSceneShowManager.GetSceneCountInfo()+"\n"+subSceneShowManager.GetShowSceneCountInfo();
        if(toggleListVisible1.isOn==false)
        {
            ToggleSceneUIList1(false);
        }
        if(toggleListVisible2.isOn==false)
        {
            ToggleSceneUIList2(false);
        }
        toggleListVisible1.onValueChanged.AddListener(ToggleSceneUIList1);
        toggleListVisible2.onValueChanged.AddListener(ToggleSceneUIList2);
    }

    public Toggle toggleListVisible1;

    public Toggle toggleListVisible2;

    
    public void ToggleSceneUIList1(bool isValue)
    {
        Debug.Log("ToggleSceneUIList1:"+isValue);
        subSceneManagerUI.panelSceneList_Loaded_RootGo.SetActive(isValue);
    }
    public void ToggleSceneUIList2(bool isValue)
    {
        Debug.Log("ToggleSceneUIList2:"+isValue);
        subSceneManagerUI.panelSceneList_RootGo.SetActive(isValue);
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
        subSceneManagerUI.SetIsOneCoroutine();
        subSceneShowManager.LoadStartScens();
    }

   [ContextMenu("LoadOut0TreeScenes")]
    public void LoadOut0TreeScenes()
    {
        Debug.Log("LoadOut0TreeScenes");
        subSceneManagerUI.SetIsOneCoroutine();
        subSceneShowManager.LoadOut0TreeScenes();
    }

    [ContextMenu("LoadOut0TreeNodeScenes")]
    public void LoadOut0TreeNodeScenes()
    {
        Debug.Log("LoadOut0TreeNodeScenes");
        subSceneManagerUI.SetIsOneCoroutine();
        subSceneShowManager.LoadOut0TreeNodeScenes();
    }


    [ContextMenu("LoadOut0TreeNodeScenesTopN")]
    public void LoadOut0TreeNodeScenesTopN(int index)
    {
        Debug.Log("LoadOut0TreeNodeScenesTopN:"+index);
        subSceneManagerUI.SetIsOneCoroutine();
        subSceneShowManager.LoadOut0TreeNodeSceneTopN(index);
    }

    [ContextMenu("LoadOut0TreeNodeScenesTopBigN")]
    public void LoadOut0TreeNodeScenesTopBigN(int index)
    {
        Debug.Log("LoadOut0TreeNodeScenesTopBigN:"+index);
        subSceneManagerUI.SetIsOneCoroutine();
        subSceneShowManager.LoadOut0TreeNodeSceneTopBiggerN(index);
    }

    [ContextMenu("CloseSceneListUI")]
    public void CloseSceneListUI()
    {
        subSceneManagerUI.gameObject.SetActive(false);
    }

    [ContextMenu("OpenSceneListUI")]
    public void OpenSceneListUI()
    {
        subSceneManagerUI.gameObject.SetActive(true);
    }



}
