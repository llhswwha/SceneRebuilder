using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRebuilderCanvas : MonoBehaviour
{
    public List<GameObject> UIElements = new List<GameObject>();

    public bool IsUIActive = true;

    private void Start()
    {
#if !UNITY_EDITOR
        bool isDebug = SystemSettingHelper.systemSetting.IsDebug;
        CanvasGroup group = gameObject.AddMissingComponent<CanvasGroup>();
        if (group)
        {
            group.enabled = true;
            group.alpha = isDebug ? 1 : 0;
            group.blocksRaycasts = isDebug ? true : false;
            group.interactable = isDebug ? true : false;
        }
        else
        {
            Debug.LogError("Exception.SceneRebuilderCanvas.canvasgroup is null...");
        }
#endif
    }

    [ContextMenu("SwitchUIActive")]
    public void SwitchUIActive()
    {
        IsUIActive = !IsUIActive;
        foreach(var ele in UIElements)
        {
            ele.gameObject.SetActive(IsUIActive);
        }
    }
}
