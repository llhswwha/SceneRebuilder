using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeFlangeModel))]
public class PipeFlangeModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeReducerModel targetT = target as PipeReducerModel;
        base.DrawBaseModelToolBar(targetT);
        base.OnInspectorGUI();
    }
}
