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

    public bool IsAllLoaded()
    {
        bool isLoaded=true;
        foreach(var scene in scenes)
        {
            if(scene==null)continue;
            if(scene.IsLoaded==false){
                isLoaded=false;
                break;
            }
        }
        return isLoaded;
    }

    public bool IsAllLoaded_Debug()
    {
        bool isLoaded = true;
        foreach (var scene in scenes)
        {
            if (scene == null) continue;
            if (scene.IsLoaded == false)
            {
                //isLoaded = false;
                //break;
                Debug.Log($"scene:{scene.name} isLoaded:{scene.IsLoaded}");
            }
        }
        return isLoaded;
    }

    public List<SubScene_Base> GetUnloadedScenes()
    {
        List<SubScene_Base> unloadscenes = new List<SubScene_Base>();
        foreach (var scene in scenes)
        {
            if (scene == null) continue;
            if (scene.IsLoaded == false)
            {
                unloadscenes.Add(scene);

                //Debug.Log($"unload scene:{scene.name} isLoaded:{scene.IsLoaded}");
            }
        }
        return unloadscenes;
    }

    public void LoadUnloadedScenes()
    {
        var scenes = this.GetUnloadedScenes();
        //foreach (var scene in scenes)
        //{
        //    scene.EditorLoadScene();
        //}
        SubSceneCreater.EditorLoadScenes(scenes, null);
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
