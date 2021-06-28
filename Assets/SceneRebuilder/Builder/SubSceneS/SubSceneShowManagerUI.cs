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
}
