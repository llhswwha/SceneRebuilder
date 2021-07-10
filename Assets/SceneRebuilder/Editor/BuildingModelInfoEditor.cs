using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(BuildingModelInfo))]
public class BuildingModelInfoEditor : Editor
{
    
    GUIStyle contentStyle;
    int buttonWidth=110;

    void OnEnable () {
		
	}

    private void NewButton(string text,bool isEnable,Action clickEvent)
    {
        EditorGUI.BeginDisabledGroup(!isEnable);
        if (GUILayout.Button(text,contentStyle,GUILayout.Width(buttonWidth)))
        {
            if(clickEvent!=null){
                clickEvent();
            }
        }
        EditorGUI.EndDisabledGroup();

        // if (GUILayout.Button(text))
        // {
        //     if(clickEvent!=null){
        //         clickEvent();
        //     }
        // }
    }

    public override void OnInspectorGUI()
    {
        BuildingModelInfo info = target as BuildingModelInfo;
        int sceneCount=info.GetSceneCount();
        bool isAllLoaded=info.IsSceneLoaded();
        int treeCount=info.GetTreeCount();
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("1.GetInfo",contentStyle,GUILayout.Width(buttonWidth)))
        {
            info.InitInOut();
        }

        if(GUILayout.Button("FindDoors",contentStyle,GUILayout.Width(buttonWidth)))
        {
            //info.FindDoors();
        }

        if(GUILayout.Button("ShowMeshes",contentStyle,GUILayout.Width(buttonWidth)))
        {
            //info.FindDoors();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTree",true,info.CreateTreesBSEx);
        NewButton("RemoveTrees",treeCount>0,info.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes",true,info.EditorCreateNodeScenes);
        NewButton("RemoveScenes",isAllLoaded && sceneCount>0,info.DestroyScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("4.LoadScenes",isAllLoaded==false && sceneCount>0,info.EditorLoadNodeScenes);
        NewButton("UnloadScenes",isAllLoaded==true && sceneCount>0,info.UnLoadScenes);
        EditorGUILayout.EndHorizontal();

         base.OnInspectorGUI();
    }
}
