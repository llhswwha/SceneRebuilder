using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubScene_List : MonoBehaviour
{
    public int sceneCount;

    public SubScene_Base[] scenes;

    [ContextMenu("Init")]
    public void Init()
    {
        scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        sceneCount = scenes.Length;
    }

    internal void AddScene(SubScene_Base scene)
    {
        GameObject subSceneGo = new GameObject(scene.GetSceneNameEx());
        subSceneGo.transform.position = this.transform.position;
        subSceneGo.transform.SetParent(this.transform);

        EditorHelper.CopyComponent(subSceneGo, scene);
        Init();
    }

    [ContextMenu("Clear")]
    internal void Clear()
    {
        scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        foreach(var scene in scenes)
        {
            GameObject.DestroyImmediate(scene.gameObject);
        }
    }
}
