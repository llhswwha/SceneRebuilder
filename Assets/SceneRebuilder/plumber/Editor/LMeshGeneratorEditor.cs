using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LMeshGenerator))]
public class LMeshGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawToolBar();
		DrawDefaultInspector();
	}

	private void DrawToolBar()
	{
		LMeshGenerator targetT = (LMeshGenerator)target;

		GUILayout.Label(targetT.GetId());
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("CreateLine"))
		{
			targetT.CreateLine();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("ClearDict"))
		{
			targetT.ClearMeshDict();
		}
		if (GUILayout.Button("GetDict"))
		{
			targetT.GetMeshDict();
		}
		if (GUILayout.Button("ClearMesh"))
		{
			targetT.ClearMeshTransform();
		}
		GUILayout.EndHorizontal();
	}
}
