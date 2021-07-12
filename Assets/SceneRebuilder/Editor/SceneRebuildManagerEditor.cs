using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(SceneRebuildManager))]
public class SceneRebuildManagerEditor : BaseEditor<SceneRebuildManager>
{
    public override void OnToolLayout(SceneRebuildManager item)
    {
        base.OnToolLayout(item);

        var buildings=GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        int count=buildings.Length;
        // if (GUILayout.Button("Count:"+count, contentStyle))
        // {
        // }

        EditorGUILayout.BeginHorizontal();
        NewButton("1.InitModels", buttonWidth, true, item.InitBuildings);
        NewButton("Count:"+count,buttonWidth,true,()=>
        {
            SceneRebuildEditorWindow.ShowWindow();
        });
        EditorGUILayout.EndHorizontal();

        // if (GUILayout.Button("InitModels", contentStyle))
        // {
        //     item.InitBuildings();
        // }

        // if (GUILayout.Button("2.CreateTrees", contentStyle))
        // {
        //     item.CombineBuildings();
        // }

        // if (GUILayout.Button("CreateScenes", contentStyle))
        // {
        //     item.SaveScenes();
        // }

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateTrees", buttonWidth, true, item.CombineBuildings);
        NewButton("RemoveTrees",buttonWidth,true,item.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes", buttonWidth, true, item.SaveScenes);
        NewButton("LoadScenes",buttonWidth,true,item.LoadScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("4.SetBuildings", buttonWidth, true, item.SetBuildings);
        NewButton("ClearBuildings",buttonWidth,true,item.ClearBuildings);
        EditorGUILayout.EndHorizontal();

        // if (GUILayout.Button("SetBuildings", contentStyle))
        // {
        //     item.SetBuildings();
        // }
        if (GUILayout.Button("OneKey", contentStyle))
        {
            item.OneKey();
        }
        // if (GUILayout.Button("ClearBuildings", contentStyle))
        // {
        //     item.ClearBuildings();
        // }
    }
}
