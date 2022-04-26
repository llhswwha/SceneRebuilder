using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeBendModel))]
public class PipeBendModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeBendModel targetT = target as PipeBendModel;
        base.DrawBaseModelToolBar(targetT);

        if (GUILayout.Button("ShowPartLines"))
        {
            targetT.ShowPartLines();
        }

        base.OnInspectorGUI();
    }
}
