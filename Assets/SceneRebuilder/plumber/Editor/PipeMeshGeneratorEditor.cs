using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PipeMeshGenerator))]
public class PipeMeshGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		PipeMeshGenerator myTarget = (PipeMeshGenerator) target;

		if (GUILayout.Button("Generate Pipe")) {
			myTarget.RenderPipe();
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("ShowPoints"))
		{
			myTarget.ShowPoints();
		}
		if (GUILayout.Button("ShowPoints2"))
		{
			myTarget.ShowPoints2();
		}
		GUILayout.EndHorizontal();
		//GetPointsFromTransforms
		if (GUILayout.Button("GetPointsFromTransforms"))
		{
			myTarget.GetPointsFromTransforms();
		}

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