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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowPartLines"))
        {
            targetT.ShowPartLines();
        }
        if (GUILayout.Button("RendererElbow"))
        {
            targetT.RendererElbowModel(targetT.GetArg(), "_Elbow");
        }
        if (GUILayout.Button("RendererLines"))
        {
            targetT.RendererLinesModel(targetT.GetArg(), "_Lines");
        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
