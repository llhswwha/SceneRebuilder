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
        return EditorHelper.CreateScene(arg.path, arg.isOveride, arg.isOpen, arg.objs);
        //scenePath = path;
    }

    public static SubScene_Single EditorCreateScene(GameObject go)
    {

        //UpackPrefab_One(go);
        //SubSceneManager subSceneManager = SubSceneManager.Instance;
        //SubScene_Single ss = go.AddComponent<SubScene_Single>();
        //ss.Init();
        //string path = subSceneManager.GetScenePath(go.name, SubSceneDir.Single);
        //SubSceneHelper.SaveChildrenToScene(path, go.transform, subSceneManager.IsOverride);
        //ss.ShowBounds();

        SubSceneManager subSceneManager = SubSceneManager.Instance;
        string path = subSceneManager.GetScenePath(go.name, SceneContentType.Single);
        return EditorCreateScene<SubScene_Single>(go, path, subSceneManager.IsOverride);
    }

    public static T EditorCreateScene<T>(GameObject go, string path, bool isOverride) where T : SubScene_Base
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

        return EditorCreateScene<T>(go, path, isOverride, null);
    }

    public static T EditorCreateScene<T>(GameObject go, string path, bool isOverride, T ss) where T : SubScene_Base
    {
        UpackPrefab_One(go);

        if (ss == null)
        {
            ss = go.AddComponent<T>();
        }
        //SubScene_Single ss = go.AddComponent<SubScene_Single>();
        //string path = GetScenePath(go.name, isPart);
        ss.SetPath(path);
        ss.Init();
        //
        SubSceneHelper.SaveChildrenToScene(path, go.transform, isOverride);
        ss.ShowBounds();
        return ss;
    }

    public static void UpackPrefab_One(GameObject go)
    {
#if UNITY_EDITOR
        GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
        if (root != null)
        {
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        }
#endif
    }
}

public enum SubSceneType
{
    Single, Part, Base, In, Out0, Out1
}

public enum SceneContentType
{
    Single,Part,Tree,TreePart
}
