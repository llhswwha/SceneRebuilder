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
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("ShowObbInfo"))
        {
            targetT.ShowObbInfo();
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("GetObb"))
        {
            targetT.GetObb();
        }
        if (GUILayout.Button("ShowOBBBox"))
        {
            targetT.ClearChildren();
            targetT.ShowOBBBox();
        }
        if (GUILayout.Button("ShowPipePoints"))
        {
            targetT.ClearChildren();
            targetT.ShowPipePoints();
        }
        if (GUILayout.Button("DrawWireCube"))
        {
            targetT.ClearChildren();
            targetT.DrawWireCube();
        }
        //if (GUILayout.Button("CreatePipe"))
        //{
        //    targetT.CreatePipe();
        //}
        //if (GUILayout.Button("CreateWeld"))
        //{
        //    targetT.CreateWeld();
        //}
        GUILayout.EndHorizontal();

        if (GUILayout.Button("ShowMeshVertices"))
        {
            targetT.ClearChildren();
            targetT.ShowMeshVertices();
        }
        if (GUILayout.Button("ShowSharedMeshVertices"))
        {
            targetT.ClearChildren();
            targetT.ShowSharedMeshVertices();
        }

        base.OnInspectorGUI();
    }
}
