using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshPrefabInstance))]
public class MeshPrefabInstanceEditor : Editor
{
    private FoldoutEditorArg<MeshPrefabInstance> ListArg = new FoldoutEditorArg<MeshPrefabInstance>();

    private List<MeshPrefabInstance> Instances = new List<MeshPrefabInstance>();

    public override void OnInspectorGUI()
    {
        MeshPrefabInstance myTarget = (MeshPrefabInstance)target;
        if (GUILayout.Button("InitInstancesDict"))
        {
            MeshPrefabInstance.InitInstancesDict();
        }
        if (GUILayout.Button("FindInstances"))
        {
            Instances=myTarget.FindInstances();
        }
        if (GUILayout.Button("LoadMesh"))
        {
            myTarget.LoadMesh();
        }

        BaseFoldoutEditorHelper.DrawObjectList("Instances", Instances, ListArg, null, null, null);

        base.OnInspectorGUI();
    }
}
