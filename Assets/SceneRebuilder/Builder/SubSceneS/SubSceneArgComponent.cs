using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSceneArgComponent : MonoBehaviour
{
    public SubSceneArg sceneArg;

    public SceneLoadArg GetSceneArg()
    {
        SceneLoadArg arg = new SceneLoadArg();
        arg.name = GetSceneName();
        arg.path = sceneArg.path;
        arg.index = sceneArg.index;
        return arg;
    }

    public string sceneName = "";

    public string GetSceneName()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            try
            {
                if (sceneArg == null) return "";
                if (string.IsNullOrEmpty(sceneArg.path)) return "";
                string[] parts = sceneArg.path.Split(new char[] { '.', '\\', '/' });
                if (parts.Length < 2) return "";
                sceneName = parts[parts.Length - 2];
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetSceneName  obj:{this},path:{sceneArg.path},Exception:{ex}");
            }

        }
        return sceneName;
    }

    public SceneContentType contentType;
}
