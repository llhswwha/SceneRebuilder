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
            targetT.ShowObbInfo(true);
        }
        if (GUILayout.Button("ClearChildren"))
        {
            targetT.ClearChildren();
        }
        if (GUILayout.Button("GetObb"))
        {
            targetT.GetObb(true);
        }
        if (GUILayout.Button("GetObb(Job)"))
        {
            targetT.GetObb(true);
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

        //GUILayout.BeginHorizontal();

        //GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
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
        if (GUILayout.Button("ShowPlanes"))
        {
            targetT.ShowPlanes();
        }
        if (GUILayout.Button("DrawWireCube"))
        {
            targetT.ClearChildren();
            targetT.DrawWireCube();
        }
        if (GUILayout.Button("AlignDirection"))
        {
            targetT.AlignDirection();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
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

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
