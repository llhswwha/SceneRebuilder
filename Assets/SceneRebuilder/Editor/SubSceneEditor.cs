using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(SubScene_Base))]
public class SubSceneEditor<T> : BaseEditor<T> where T : SubScene_Base
{
    public override void OnToolLayout(T item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        string scenePath1 = item.GetSceneArg().path;
        string sceneAssetPath = "Assets/" + item.GetSceneArg().path;
        string sceneFilePath = Application.dataPath + "/"+ scenePath1;
        //sceneFilePath = sceneFilePath.Replace("/", "\\\\");
        bool isFileExist = System.IO.File.Exists(sceneFilePath);
        NewButton("SelectFile", buttonWidth, true, () =>
        {
            //item.GetSceneArg().path;
            Debug.Log(scenePath1);
            Debug.Log(Application.dataPath);
            Debug.Log(sceneFilePath+"|"+System.IO.File.Exists(sceneFilePath));
            SceneAsset sceneAsset=AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            Debug.Log(sceneAsset);
            //var scene=EditorSceneManager.GetSceneByPath(item.GetSceneArg().path);
            EditorHelper.SelectObject(sceneAsset);
        });
        NewButton("DeleteFile", buttonWidth, true, () =>
        {
            //item.GetSceneArg().path;
            Debug.Log(scenePath1);
            Debug.Log(Application.dataPath);
            Debug.Log(sceneFilePath + "|" + System.IO.File.Exists(sceneFilePath));
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            Debug.Log(sceneAsset);
            //var scene=EditorSceneManager.GetSceneByPath(item.GetSceneArg().path);
            EditorHelper.SelectObject(sceneAsset);
        });
        if (Application.isPlaying)
        {
            NewButton("RunLoad", buttonWidth, item.IsLoaded == false, () =>
            {
                IdDictionary.InitInfos();
                item.LoadSceneAsync(null);
            });
            NewButton("RunUnload", buttonWidth, item.IsLoaded == true, () =>
            {
                item.UnLoadSceneAsync();
            });
        }
        else
        {
            NewButton("Load", buttonWidth, item.IsLoaded == false, () =>
            {
                IdDictionary.InitInfos();
                item.EditorLoadScene();
            });
            NewButton("Unload", buttonWidth, item.IsLoaded == true, item.UnLoadGosM);

        }
        EditorGUILayout.EndHorizontal();
    }
}

[CustomEditor(typeof(SubScene_In))]
public class SubScene_InEditor : SubSceneEditor<SubScene_In>
{
}

[CustomEditor(typeof(SubScene_Out0))]
public class SubSceneOut0Editor : SubSceneEditor<SubScene_Out0>
{
}

[CustomEditor(typeof(SubScene_Out1))]
public class SubSceneOut1Editor : SubSceneEditor<SubScene_Out1>
{
}
