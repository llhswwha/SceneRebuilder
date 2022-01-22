using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeReducerModel))]
public class PipeReducerModelEditor 
    //: PipeLineModelEditor
    //:PipeElbowModelEditor
    : PipeModelBaseEditor
{
    public override void OnInspectorGUI()
    {
        PipeReducerModel targetT = target as PipeReducerModel;
        base.DrawBaseModelToolBar(targetT);

        base.OnInspectorGUI();
    }
}
