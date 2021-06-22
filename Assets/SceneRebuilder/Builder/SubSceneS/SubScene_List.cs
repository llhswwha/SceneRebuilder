using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_List : MonoBehaviour
{
    public int sceneCount;

    [ContextMenu("Init")]
    public void Init()
    {
        var scenes = gameObject.GetComponentsInChildren<SubScene_Base>();
        sceneCount = scenes.Length;
    }

    internal void AddScene(SubScene_Base scene)
    {
        GameObject subSceneGo = new GameObject(scene.GetSceneNameEx());
        subSceneGo.transform.position = this.transform.position;
        subSceneGo.transform.SetParent(this.transform);

        EditorHelper.CopyComponent(subSceneGo, scene);
        sceneCount++;
    }
}
