using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(BuildingModelInfo))]
public class BuildingModelInfoEditor : BaseEditor<BuildingModelInfo>
{
    public override void OnInspectorGUI()
    {
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        BuildingModelInfo info = target as BuildingModelInfo;
        int sceneCount=info.GetSceneCount();
        bool isAllLoaded=info.IsSceneLoaded();
        int treeCount=info.GetTreeCount();
        int partCount=info.GetPartCount();

        EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("Debug", contentStyle, GUILayout.Width(80)))
        //{
        //    info.SceneList.IsAllLoaded_Debug();
        //}
        if (GUILayout.Button("SetUnloadScenes", contentStyle, GUILayout.Width(150)))
        {
            //info.SceneList.IsAllLoaded_Debug();

            var scenes=info.SceneList.GetUnloadedScenes();
            Debug.Log($"scenes:{scenes.Count}");
            //EditorHelper.SelectObject(element.rootObj);
            EditorHelper.SelectObjects(scenes);
        }
        GUILayout.Label($"loaded:{isAllLoaded},part:{partCount} tree:{treeCount},scene:{sceneCount}");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("1.GetInfo",contentStyle,GUILayout.Width(buttonWidth)))
        {
            info.InitInOut();
        }

        // if(GUILayout.Button("FindDoors",contentStyle,GUILayout.Width(90)))
        // {
        //     //info.FindDoors();
        // }
        NewButton("FindDoors",90,partCount>0,info.FindInDoors);

        if(GUILayout.Button("ShowMeshes",contentStyle,GUILayout.Width(90)))
        {
            //info.FindDoors();
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTree",buttonWidth,isAllLoaded==true && partCount>0,info.CreateTreesBSEx);
        NewButton("RemoveTrees",buttonWidth,isAllLoaded==true && treeCount>0,info.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes",buttonWidth,isAllLoaded==true && treeCount>0,info.EditorCreateNodeScenes);
        NewButton("RemoveScenes",buttonWidth,isAllLoaded==true && sceneCount>0,info.DestroyScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("4.LoadScenes",buttonWidth,isAllLoaded==false && sceneCount>0,info.EditorLoadNodeScenesEx);
        NewButton("UnloadScenes",buttonWidth,isAllLoaded==true && sceneCount>0,info.UnLoadScenes);
        EditorGUILayout.EndHorizontal();

         base.OnInspectorGUI();
    }
}
