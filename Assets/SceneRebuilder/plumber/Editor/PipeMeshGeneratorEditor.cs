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

		GUILayout.BeginHorizontal();

		GUILayout.Label($"IsBend");
		myTarget.IsBend = EditorGUILayout.Toggle(myTarget.IsBend);
		
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Generate Pipe(Points)"))
		{
			myTarget.RenderPipe();
		}
		if (GUILayout.Button("Generate Pipe(Transform)"))
		{
			myTarget.RenderPipeFromTransforms();
		}
		GUILayout.EndHorizontal();

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
        if (GUILayout.Button("ShowPoints(T)"))
        {
            myTarget.ShowTransformPoints();
        }
        if (GUILayout.Button("ShowTriangles"))
        {
            myTarget.DebugShowTriangles();
        }
        GUILayout.EndHorizontal();

		//GUILayout.BeginHorizontal();
		//if (GUILayout.Button("ShowCircles"))
		//{
		//	myTarget.ShowCirclePoints();
		//}
		//if (GUILayout.Button("ShowCirclesAll"))
		//{
		//	myTarget.ShowCirclePointsAll();
		//}
		//GUILayout.EndHorizontal();

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
		if (GUILayout.Button("AlignDirection"))
		{
			myTarget.AlignDirection();
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

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SetRadius5"))
        {
            myTarget.SetRadiusUniform(5);
        }
        if (GUILayout.Button("SetRadius4"))
        {
            myTarget.SetRadiusUniform(4);
        }
        if (GUILayout.Button("SetRadius3"))
        {
            myTarget.SetRadiusUniform(3);
        }
        if (GUILayout.Button("SetRadius2"))
        {
            myTarget.SetRadiusUniform(2);
        }
        GUILayout.EndHorizontal();
    }

}