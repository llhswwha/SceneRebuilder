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
#if UNITY_EDITOR
    internal void AddScene(SubScene_Base scene)
    {
        GameObject subSceneGo = new GameObject(scene.GetSceneNameEx());
        subSceneGo.transform.position = this.transform.position;
        subSceneGo.transform.SetParent(this.transform);

        EditorHelper.CopyComponent(subSceneGo, scene);
        Init();
    }
#endif

    [ContextMenu("Clear")]
    internal void Clear()
    {
        IdDictionary.InitInfos();

        scenes = gameObject.GetComponentsInChildren<SubScene_Base>(true);
        foreach(var scene in scenes)
        {
            scene.DestroyBoundsBox();
            scene.SetRendererParent();

            // GameObject.DestroyImmediate(scene.gameObject);

            // scene.UnLoadGosM();
            GameObject.DestroyImmediate(scene);


        }
        sceneCount = 0;
        //ClearChildren();

        var sceneList= gameObject.GetComponentsInChildren<SubScene_List>(true);
        foreach(var ss in sceneList)
        {
            GameObject.DestroyImmediate(ss);
        }
    }

    // public void ClearChildren()
    // {
    //     List<Transform> children=new List<Transform>();
    //     for(int i=0;i<this.transform.childCount;i++)
    //     {
    //         children.Add(this.transform.GetChild(i));
    //     }
    //     foreach(var child in children){
    //         GameObject.DestroyImmediate(child.gameObject);
    //     }
    // }
}
