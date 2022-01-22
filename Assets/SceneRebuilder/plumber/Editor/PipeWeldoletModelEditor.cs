using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeWeldoletModel))]
public class PipeWeldoletModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeWeldoletModel targetT = target as PipeWeldoletModel;
        base.DrawBaseModelToolBar(targetT);
        base.OnInspectorGUI();
    }
}
