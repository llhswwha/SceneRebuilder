using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRebuilderCanvas : MonoBehaviour
{
    public List<GameObject> UIElements = new List<GameObject>();

    public bool IsUIActive = true;

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
