using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeMeshGenerator))]
public class PipeMeshGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		DrawToolBar();
		DrawDefaultInspector();
	}

	private void DrawToolBar()
    {
		PipeMeshGenerator myTarget = (PipeMeshGenerator)target;

		if (GUILayout.Button("Generate Pipe"))
		{
			myTarget.RenderPipe();
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("CleanChildren"))
		{
			myTarget.CleanChildren();
		}
		if (GUILayout.Button("ShowPoints"))
		{
			myTarget.ShowPoints();
		}
		if (GUILayout.Button("ShowPoints2"))
		{
			myTarget.ShowPoints2();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		//GetPointsFromTransforms
		if (GUILayout.Button("GetPointsFromTransforms"))
		{
			myTarget.GetPointsFromTransforms();
		}
		if (GUILayout.Button("TestRemoveColinearPoints"))
		{
			myTarget.TestRemoveColinearPoints();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Torus XZ"))
		{
			myTarget.RenderTorusXZ();
		}
		if (GUILayout.Button("Torus YZ"))
		{
			myTarget.RenderTorusYZ();
		}
		if (GUILayout.Button("Torus XY"))
		{
			myTarget.RenderTorusXY();
		}
		GUILayout.EndHorizontal();
	}

}