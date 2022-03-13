using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubScene_List : MonoBehaviour
{
    public  static SubScene_Base[] GetBaseScenes(GameObject go)
    {
        return go.GetComponentsInChildren<SubScene_Base>(true).Where(i => i.contentType != SceneContentType.LOD0 && !(i is SubScene_Ref)).ToArray();
    }

    public int sceneCount;

    public SubScene_Base[] scenes;

    [ContextMenu("GetScenes")]
    public void GetScenes()
    {
        scenes = SubScene_List.GetBaseScenes(gameObject);
        sceneCount = scenes.Length;
    }

#if UNITY_EDITOR
    internal void AddScene(SubScene_Base scene)
    {
        GameObject subSceneGo = new GameObject(scene.GetSceneNameEx());
        subSceneGo.transform.position = this.transform.position;
        subSceneGo.transform.SetParent(this.transform);

        EditorHelper.CopyComponent(subSceneGo, scene);
        GetScenes();
    }

    public void LoadUnloadedScenes()
    {
        var scenes = this.GetUnloadedScenes();
        SubSceneHelper.EditorLoadScenes(scenes, null);
    }
#endif

    public List<MeshRenderer> GetRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach (var scene in scenes)
        {
            var rs=scene.GetSceneRenderers();
            renderers.AddRange(rs);
        }
        return renderers;
    }

    [ContextMenu("Clear")]
    internal int Clear()
    {
        //IdDictionary.InitInfos();
        var renderers = GetRenderers();
        IdDictionary.InitRenderers(renderers);

        GetScenes();
        foreach (var scene in scenes)
        {
            scene.DestroyBoundsBox();
            scene.SetRendererParent();

            // GameObject.DestroyImmediate(scene.gameObject);

            // scene.UnLoadGosM();
            //GameObject.DestroyImmediate(scene);

            scene.DestroyScene();

        }
        sceneCount = 0;
        //ClearChildren();

        var sceneList= gameObject.GetComponentsInChildren<SubScene_List>(true);
        foreach(var ss in sceneList)
        {
            GameObject.DestroyImmediate(ss);
        }
        return scenes.Length;
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

    public SubSceneBag GetUnloadedScenes()
    {
        SubSceneBag unloadscenes = new SubSceneBag();
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
