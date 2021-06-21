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
        return EditorHelper.CreateScene(path, isOverride, subSceneManager.IsOpenSubScene, children.ToArray());
        //scenePath = path;
    }

    public static SubScene_Single CreateSubScene(GameObject go, string path, bool isOverride)
    {
        UpackPrefab_One(go);

        SubScene_Single ss = go.AddComponent<SubScene_Single>();
        ss.Init();
        //string path = GetScenePath(go.name, isPart);
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