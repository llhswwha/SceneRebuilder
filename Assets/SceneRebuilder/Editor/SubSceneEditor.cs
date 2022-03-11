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

        var arg = item.GetSceneArg();
        string scenePath1 = arg.path;
        string sceneAssetPath = arg.GetSceneAssetPath();
        string sceneFilePath = arg.GetSceneFilePath();
        GUILayout.Label($"({item.contentType})[{arg.index}] {scenePath1}");
        if (GUILayout.Button("SetBuildings"))
        {
            SubSceneHelper.SetBuildings();
        }

        EditorGUILayout.BeginHorizontal();

        //sceneFilePath = sceneFilePath.Replace("/", "\\\\");
        
        bool isFileExist = System.IO.File.Exists(sceneFilePath);
        NewButton("SelectFile", buttonWidth, isFileExist, () =>
        {
            //item.GetSceneArg().path;
            Debug.Log(scenePath1);
            Debug.Log(Application.dataPath);
            Debug.Log(sceneFilePath+"|"+System.IO.File.Exists(sceneFilePath));
            SceneAsset sceneAsset=AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            Debug.Log(sceneAsset);
            //var scene=EditorSceneManager.GetSceneByPath(item.GetSceneArg().path);
            if(sceneAsset!=null)
                EditorHelper.SelectObject(sceneAsset);
        });
        NewButton("DeleteFile", buttonWidth, isFileExist && item.IsLoaded == true, () =>
        {
            System.IO.File.Delete(sceneFilePath);
            EditorHelper.RefreshAssets();
            //EditorHelper.PathToRelative
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
            NewButton("Create", buttonWidth, item.IsLoaded == true, () =>
            {
                item.EditorCreateScene(false);
                EditorHelper.RefreshAssets();
            });
            NewButton("Create(Children)", buttonWidth, item.IsLoaded == true, () =>
            {
                item.EditorCreateScene(true);
                EditorHelper.RefreshAssets();
            });
            NewButton("Load", buttonWidth, item.IsLoaded == false && isFileExist, () =>
            {
                IdDictionary.InitInfos();
                item.EditorLoadScene();
            });
            NewButton("Unload", buttonWidth, item.IsLoaded == true && isFileExist, ()=> { item.UnLoadGosM(); });

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

[CustomEditor(typeof(SubScene_Single))]
public class SubScene_SingleEditor : SubSceneEditor<SubScene_Single>
{
}

[CustomEditor(typeof(SubScene_LODs))]
public class SubScene_LODEditor : SubSceneEditor<SubScene_LODs>
{
}

[CustomEditor(typeof(SubScene_Ref))]
public class SubScene_RefEditor : BaseEditor<SubScene_Ref>
{
    public override void OnToolLayout(SubScene_Ref item)
    {
        base.OnToolLayout(item);
        GUILayout.Label($"RefId:{item.RefId} IdCount:{item.SceneIds.Count}");
        var arg = item.GetSceneArg();
        string scenePath1 = arg.path;
        string sceneAssetPath = arg.GetSceneAssetPath();
        string sceneFilePath = arg.GetSceneFilePath();
        GUILayout.Label($"({item.contentType})[{arg.index}] {scenePath1}");
        if (GUILayout.Button("SetBuildings"))
        {
            SubSceneHelper.SetBuildings();
        }
    }
}
