using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OBBCollider))]
public class OBBColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        OBBCollider targetT = target as OBBCollider;
        if(GUILayout.Button("ShowObbInfo"))
        {
            targetT.ShowObbInfo();
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        //if (GUILayout.Button("CreatePipe"))
        //{
        //    targetT.CreatePipe();
        //}
        //if (GUILayout.Button("CreateWeld"))
        //{
        //    targetT.CreateWeld();
        //}
        base.OnInspectorGUI();
    }
}
