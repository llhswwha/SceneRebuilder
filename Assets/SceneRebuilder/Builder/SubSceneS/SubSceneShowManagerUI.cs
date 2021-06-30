using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubSceneShowManagerUI : MonoBehaviour
{
    public Text txtInfo;

    public SubSceneShowManager subSceneShowManager;

    private void Start()
    {
        subSceneShowManager = GameObject.FindObjectOfType<SubSceneShowManager>();
    }

    public void Update()
    {
        if(subSceneShowManager)
        {
            txtInfo.text = $"{subSceneShowManager.GetDisInfo()}\n{subSceneShowManager.GetSceneInfo()}";
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
        subSceneShowManager.LoadStartScens();
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
