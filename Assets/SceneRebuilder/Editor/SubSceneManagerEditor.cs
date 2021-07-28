using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static SubSceneManager;

[CustomEditor(typeof(SubSceneManager))]
public class SubSceneManagerEditor : BaseFoldoutEditor<SubSceneManager>
{
    private FoldoutEditorArg sceneListArg = new FoldoutEditorArg();

    private FoldoutEditorArg<SceneFile> sceneFileListArg1 = new FoldoutEditorArg<SceneFile>();

    public override void OnEnable()
    {
        base.OnEnable();

        sceneListArg = new FoldoutEditorArg(true, false, true, true, false);

        sceneFileListArg1 = new FoldoutEditorArg<SceneFile>(true, false, true, true, false);
    }

    public override void OnToolLayout(SubSceneManager item)
    {
        base.OnToolLayout(item);

        if(GUILayout.Button("GetSceneFiles"))
        {
            sceneFileListArg1.Items = item.GetSceneFilesEx();
        }

        if (GUILayout.Button("DeleteInActiveScenes"))
        {
            item.DeleteInActiveScenes();
            sceneFileListArg1.Items = item.GetSceneFilesEx();
        }

        if (sceneFileListArg1.Items != null)
        {
            DrawObjectList(sceneFileListArg1, "Scene File List", () =>
            {
                return sceneFileListArg1.Items;
            },file=>
            {
                var btnStyle = new GUIStyle(EditorStyles.miniButton);
                btnStyle.margin = new RectOffset(0, 0, 0, 0);
                btnStyle.padding = new RectOffset(0, 0, 0, 0);
                if (GUILayout.Button(">", btnStyle, GUILayout.Width(20)))
                {
                    Debug.Log(file.scenePath1);
                    Debug.Log(Application.dataPath);
                    Debug.Log(file.sceneFilePath + "|" + System.IO.File.Exists(file.sceneFilePath));
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(file.sceneAssetPath);
                    Debug.Log(sceneAsset);
                    //var scene=EditorSceneManager.GetSceneByPath(item.GetSceneArg().path);
                    EditorHelper.SelectObject(sceneAsset);
                }
                if (GUILayout.Button("X", btnStyle, GUILayout.Width(20)))
                {
                    SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(file.sceneAssetPath);
                    AssetDatabase.DeleteAsset(file.sceneAssetPath);
                    //AssetDatabase.Refresh();
                }
            });
        }

        if (GUILayout.Button("UpdateScenes"))
        {
            item.UpdateScenes();
        }
        DrawSceneList(sceneListArg, item.GetScenes);
    }
}
