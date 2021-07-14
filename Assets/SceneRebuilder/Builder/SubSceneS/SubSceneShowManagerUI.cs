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
        sceneCountInfo=subSceneShowManager.GetSceneCountInfoEx();
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

    public Toggle toggleEnableShow;
    public Toggle toggleEnableLoad;
    public Toggle toggleEnableHide;
    public Toggle toggleEnableUnload;

    public InputField InputDisOfShow;
    public InputField InputDisOfLoad;
    public InputField InputDisOfHide;
    public InputField InputDisOfUnload;

    public void SetShowSetting()
    {
        Debug.Log("SetShowSetting");
        subSceneShowManager.IsEnableHide = toggleEnableHide.isOn;
        subSceneShowManager.IsEnableLoad = toggleEnableLoad.isOn;
        subSceneShowManager.IsEnableShow = toggleEnableShow.isOn;
        subSceneShowManager.IsEnableUnload = toggleEnableUnload.isOn;
        subSceneShowManager.DisOfHidden = int.Parse(InputDisOfHide.text)* int.Parse(InputDisOfHide.text);
        subSceneShowManager.DisOfLoad = int.Parse(InputDisOfLoad.text) * int.Parse(InputDisOfLoad.text);
        subSceneShowManager.DisOfUnLoad = int.Parse(InputDisOfUnload.text) * int.Parse(InputDisOfUnload.text);
        subSceneShowManager.DisOfVisible = int.Parse(InputDisOfShow.text) * int.Parse(InputDisOfShow.text);
    }

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

    [ContextMenu("LoadAllScenes")]
    public void LoadAllScenes()
    {
        Debug.Log("LoadAllScenes");
        subSceneManagerUI.SetLoadingSceneMaxCount();
        subSceneShowManager.LoadStartScens_All();
    }

    [ContextMenu("LoadStartScenes")]
    public void LoadStartScenes()
    {
        Debug.Log("LoadStartScenes");
        subSceneManagerUI.SetLoadingSceneMaxCount();
        subSceneShowManager.LoadStartScens();
    }

   //[ContextMenu("LoadOut0BuildingScenes")]
   // public void LoadOut0BuildingScenes()
   // {
   //     Debug.Log("LoadOut0BuildingScenes");
   //     subSceneManagerUI.SetLoadingSceneMaxCount();
   //     subSceneShowManager.LoadOut0BuildingScenes();
   // }

   // [ContextMenu("LoadShownTreeNodes")]
   // public void LoadShownTreeNodes()
   // {
   //     Debug.Log("LoadShownTreeNodes");
   //     subSceneManagerUI.SetLoadingSceneMaxCount();
   //     subSceneShowManager.LoadShownTreeNodes();
   // }

    [ContextMenu("LoadHiddenTreeNodes")]
    public void LoadHiddenTreeNodes()
    {
        Debug.Log("LoadHiddenTreeNodes");
        subSceneManagerUI.SetLoadingSceneMaxCount();
        subSceneShowManager.LoadHiddenTreeNodes();
    }


    [ContextMenu("LoadOut0TreeNodeScenesTopN")]
    public void LoadOut0TreeNodeScenesTopN(int index)
    {
        Debug.Log("LoadOut0TreeNodeScenesTopN:"+index);
        subSceneManagerUI.SetLoadingSceneMaxCount();
        subSceneShowManager.LoadOut0TreeNodeSceneTopN(index);
    }

    [ContextMenu("LoadOut0TreeNodeScenesTopBigN")]
    public void LoadOut0TreeNodeScenesTopBigN(int index)
    {
        Debug.Log("LoadOut0TreeNodeScenesTopBigN:"+index);
        subSceneManagerUI.SetLoadingSceneMaxCount();
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
