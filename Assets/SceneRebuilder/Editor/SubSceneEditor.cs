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

        buttonWidth = 80;

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
            ////item.GetSceneArg().path;
            //Debug.Log(scenePath1);
            //Debug.Log(Application.dataPath);
            //Debug.Log(sceneFilePath+"|"+System.IO.File.Exists(sceneFilePath));
            //SceneAsset sceneAsset=AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
            //Debug.Log(sceneAsset);
            ////var scene=EditorSceneManager.GetSceneByPath(item.GetSceneArg().path);
            //if(sceneAsset!=null)
            //    EditorHelper.SelectObject(sceneAsset);
            SubSceneHelper.SelectSceneFile(item);
        });
        NewButton("DeleteFile", buttonWidth, isFileExist && item.IsLoaded == true, () =>
        {
            System.IO.File.Delete(sceneFilePath);
            EditorHelper.RefreshAssets();
            //EditorHelper.PathToRelative
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
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
            NewButton("Create(C)", buttonWidth, item.IsLoaded == true, () =>
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

        EditorGUILayout.BeginHorizontal();
        NewButton("Bounds", buttonWidth, true, () => { item.ShowBounds(); });
        NewButton("SelectObjects", buttonWidth, true, () => { item.EditorSelectObjects(); });
        NewButton("ShowObjects", buttonWidth, true, () => { item.ShowObjects(); });
        NewButton("HideObjects", buttonWidth, true, () => { item.HideObjects(); });
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
public class SubScene_LODsEditor : SubSceneEditor<SubScene_LODs>
{
    public override void OnToolLayout(SubScene_LODs item)
    {
        GUILayout.Label($"rendererIds:{item.RenderersId.Count}");

        base.OnToolLayout(item);
    }
}

[CustomEditor(typeof(SubScene_GPUI))]
public class SubScene_GPUIEditor : SubSceneEditor<SubScene_GPUI>
{
    public override void OnToolLayout(SubScene_GPUI item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Show"))
        {
            item.SetObjectActive(true);
        }
        if (GUILayout.Button("Hide"))
        {
            item.SetObjectActive(false);
        }
        if (GUILayout.Button("StartGPUI1"))
        {
            item.StartGPUI1();
        }
        if (GUILayout.Button("StartGPUI2"))
        {
            item.StartGPUI2();
        }
        if (GUILayout.Button("StopGPUI1"))
        {
            item.StopGPUI1();
        }
        if (GUILayout.Button("StopGPUI2"))
        {
            item.StopGPUI2();
        }
        EditorGUILayout.EndHorizontal();
    }
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
