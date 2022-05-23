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
            targetT.RendererElbowModel(PipeFactory.Instance.GetArg(targetT), "_Elbow");
        }
        if (GUILayout.Button("RendererLines"))
        {
            targetT.RendererLinesModel(PipeFactory.Instance.GetArg(targetT), "_Lines");
        }
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
