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

        if (GUILayout.Button("InitModels", contentStyle))
        {
            item.InitBuildings();
        }
        if (GUILayout.Button("CombineModels", contentStyle))
        {
            item.CombineBuildings();
        }
        if (GUILayout.Button("SaveScenes", contentStyle))
        {
            item.SaveScenes();
        }
        if (GUILayout.Button("SetBuildings", contentStyle))
        {
            item.SetBuildings();
        }
        if (GUILayout.Button("OneKey", contentStyle))
        {
            item.OneKey();
        }
        if (GUILayout.Button("ClearBuildings", contentStyle))
        {
            item.ClearBuildings();
        }
    }
}
