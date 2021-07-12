using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Linq;

[CustomEditor(typeof(SceneRebuildManager))]
public class SceneRebuildManagerEditor : BaseEditor<SceneRebuildManager>
{
    private SerializedProperty buildingListFoldout;

    private SerializedProperty fps;
    private SerializedProperty fpsEnabled;

    public void OnEnable()
    {
        buildingListFoldout = serializedObject.FindProperty("buildingListFoldout");

    }

    public Dictionary<BuildingModelInfo, BuildingModelInfoEditorArg> editorArgs = new Dictionary<BuildingModelInfo, BuildingModelInfoEditorArg>();

    public class BuildingModelInfoEditorArg
    {
        public bool IsExpaned = false;
    }

    public override void OnToolLayout(SceneRebuildManager item)
    {
        base.OnToolLayout(item);

        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList() ;
        buildings.Sort((a, b) =>
        {
            return b.AllVertextCount.CompareTo(a.AllVertextCount);
        });

        int count = buildings.Count;
        // if (GUILayout.Button("Count:"+count, contentStyle))
        // {
        // }
        foreach(var b in buildings)
        {
            if (!editorArgs.ContainsKey(b))
            {
                editorArgs.Add(b, new BuildingModelInfoEditorArg());
            }
        }

        EditorGUILayout.BeginHorizontal();
        NewButton("1.InitModels", buttonWidth, true, item.InitBuildings);
        NewButton("ListWindow:" + count, buttonWidth, true, () =>
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
        NewButton("RemoveTrees", buttonWidth, true, item.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes", buttonWidth, true, item.SaveScenes);
        NewButton("LoadScenes", buttonWidth, true, item.LoadScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("4.SetBuildings", buttonWidth, true, item.SetBuildings);
        NewButton("ClearBuildings", buttonWidth, true, item.ClearBuildings);
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
        EditorUIUtils.SetupStyles();

        //GUIStyle contentStyle = new GUIStyle(EditorStyles.miniButton);
        //contentStyle.alignment = TextAnchor.MiddleLeft;

        if (EditorUIUtils.Foldout(buildingListFoldout, $"List({count})"))
        {
            foreach (var b in buildings)
            {
                var arg = editorArgs[b];
                //arg.IsExpaned = EditorUIUtils.Foldout(arg.IsExpaned, $"{b.name} \t[{b.AllVertextCount:F0}w][{b.AllRendererCount}]");
                //if (arg.IsExpaned)
                //{

                //}

                arg.IsExpaned = EditorUIUtils.ToggleFoldout(arg.IsExpaned, $"{b.name}", $"[{b.AllVertextCount:F0}w][{b.AllRendererCount}]", false,false,true,()=>
                {
                    EditorGUIUtility.PingObject(b.gameObject);
                });
                if (arg.IsExpaned)
                {
                    BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
        }

        //GUI.enabled = EditorUIUtils.ToggleFoldout(fpsEnabled, fps, "FPS Counter");
        ////me.fpsCounter.Enabled = fpsEnabled.boolValue;

        //if (fps.isExpanded)
        //{
        //}
        //GUI.enabled = true;
    }
}
