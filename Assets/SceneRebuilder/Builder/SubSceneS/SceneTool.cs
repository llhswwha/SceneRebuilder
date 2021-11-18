using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneTool : MonoBehaviour
{
#if UNITY_EDITOR
    public string ScenePath = "Scenes\\SceneModels\\1A_F级主厂房_New";

    [ContextMenu("OpenSelectedScene")]
    public void OpenSelectedScene()
    {
        var obj = Selection.activeObject;
        Debug.Log(obj);
    }
#endif
}
