using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeTeeModel))]
public class PipeTeeModelEditor : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeTeeModel targetT = target as PipeTeeModel;
        
        base.DrawBaseModelToolBar(targetT);

        base.OnInspectorGUI();
    }
}
