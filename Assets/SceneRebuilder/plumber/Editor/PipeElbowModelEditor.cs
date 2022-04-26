using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeElbowModel))]
public class PipeElbowModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeElbowModel targetT = target as PipeElbowModel;
        base.DrawBaseModelToolBar(targetT);


        base.OnInspectorGUI();
    }
}
