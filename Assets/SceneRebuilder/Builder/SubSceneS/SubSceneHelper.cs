using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class SubSceneHelper
{

    public static void LinkScenes(SubScene_Base[] scenes)
    {
        int count = scenes.Length;
        Debug.Log("LinkScenes:" + count);
        for (int i = 0; i < count; i++)
        {
            SubScene_Base scene = scenes[i];
            if (scene.LinkedScene == null)
            {
                for (int j = i + 1; j < count; j++)
                {
                    SubScene_Base scene2 = scenes[j];
                    Debug.Log($"LinkScenes sceneType:{scene.GetType()} sceneType2:{scene2.GetType()} {scene.GetType() == scene2.GetType()}");
                    if (scene.GetType() == scene2.GetType())
                    {
                        scene.LinkedScene = scene2;
                        scene2.LinkedScene = scene;
                        break;
                    }
                }
            }
        }
    }

    public static List<GameObject> GetChildrenGos(Transform parent)
    {
        List<GameObject> rootChildren = new List<GameObject>();
        //childCount = parent.childCount;
        for (int i = 0; i < parent.childCount; i++)
        {
            rootChildren.Add(parent.GetChild(i).gameObject);
        }
        return rootChildren;
    }

#if UNITY_EDITOR

    //[ContextMenu("SaveScene")]
    public static Scene SaveChildrenToScene(string path, Transform target, bool isOverride)
    {
        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        var children = GetChildrenGos(target);
        SubSceneArg arg = new SubSceneArg(path, isOverride, subSceneManager.IsOpenSubScene, children.ToArray());
        return CreateScene(arg);
        //scenePath = path;
    }

    public static Scene CreateScene(SubSceneArg arg)
    {
        return EditorHelper.CreateScene(arg.path, arg.isOveride, arg.isOpen, arg.objs.ToArray());
        //scenePath = path;
    }

    public static T EditorCreateScene<T>(GameObject go, SceneContentType contentType,bool isSave,string dir) where T : SubScene_Base
    {
        if (go == null)
        {
            Debug.LogError("SubSceneHelper.EditorCreateScene go==null");
            return null;
        }
        string path = SubSceneManager.Instance.GetScenePath(go.name, contentType, dir);
        return EditorCreateScene<T>(go, path, SubSceneManager.Instance.IsOverride, isSave);
    }

    public static T EditorCreateScene<T>(GameObject go, string path, bool isOverride, bool isSave) where T : SubScene_Base
    {
        //UpackPrefab_One(go);

        //SubScene_Single ss = go.AddComponent<SubScene_Single>();
        ////string path = GetScenePath(go.name, isPart);
        //ss.SetPath(path);
        //ss.Init();
        ////
        //SubSceneHelper.SaveChildrenToScene(path, go.transform, isOverride);
        //ss.ShowBounds();
        //return ss;

        return EditorCreateScene<T>(go, path, isOverride, isSave,null);
    }

    public static T EditorCreateScene<T>(GameObject go, string path, bool isOverride, bool isSave, T ss) where T : SubScene_Base
    {
        UpackPrefab_One(go);

        if (ss == null)
        {
            ss = go.AddComponent<T>();
        }
        //SubScene_Single ss = go.AddComponent<SubScene_Single>();
        //string path = GetScenePath(go.name, isPart);

        SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        if(ss.sceneArg==null|| ss.sceneArg.objs==null|| ss.sceneArg.objs.Count==0)
            ss.sceneArg = new SubSceneArg(path, isOverride, subSceneManager.IsOpenSubScene, go);

        //ss.SetPath(path);
        ss.Init();

        //SubSceneHelper.SaveChildrenToScene(path, go.transform, isOverride);
        if (isSave)
        {
            ss.SaveScene();
            ss.ShowBounds();
        }


        return ss;
    }

    public static void UpackPrefab_One(GameObject go)
    {

        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }

    }
#endif

}

public enum SubSceneType
{
    Single, Part, Base, In, Out0, Out1
}

public enum SceneContentType
{
    Single,Part,Tree,TreeAndPart,TreeWithPart,TreeNode
}
