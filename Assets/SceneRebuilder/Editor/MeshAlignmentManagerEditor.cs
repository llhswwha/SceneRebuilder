using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshAlignmentManager))]
public class MeshAlignmentManagerEditor : BaseEditor<MeshAlignmentManager>
{
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnToolLayout(MeshAlignmentManager item)
    {
        base.OnToolLayout(item);

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("SourceR", GUILayout.Width(60));
        //item.SourceRoot = EditorGUILayout.ObjectField(item.SourceRoot, typeof(GameObject)) as GameObject;
        //GUILayout.Label("TargetR", GUILayout.Width(60));
        //item.TargetRoot = EditorGUILayout.ObjectField(item.TargetRoot, typeof(GameObject)) as GameObject;
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("SourceR", GUILayout.Width(60));
        item.SourceRoot = EditorGUILayout.ObjectField(item.SourceRoot, typeof(GameObject)) as GameObject;
        GUILayout.Label("Source", GUILayout.Width(60));
        item.Source = EditorGUILayout.ObjectField(item.Source, typeof(GameObject)) as GameObject;
        GUILayout.Label("Target", GUILayout.Width(60));
        item.Target = EditorGUILayout.ObjectField(item.Target, typeof(GameObject)) as GameObject;
        GUILayout.EndHorizontal();

       


        GUILayout.BeginHorizontal();
        Vector3 offset = item.Target.transform.position - item.Source.transform.position;
        GUILayout.Label($"offset:({offset.x},{offset.y},{offset.z})");
        item.transfromReplaceSetting.SetPosX = GUILayout.Toggle(item.transfromReplaceSetting.SetPosX, "X", GUILayout.Width(40));
        item.transfromReplaceSetting.SetPosY = GUILayout.Toggle(item.transfromReplaceSetting.SetPosY, "Y", GUILayout.Width(40));
        item.transfromReplaceSetting.SetPosZ = GUILayout.Toggle(item.transfromReplaceSetting.SetPosZ, "Z", GUILayout.Width(40));
        item.transfromReplaceSetting.Align = (TransfromAlignMode)EditorGUILayout.EnumPopup(item.transfromReplaceSetting.Align,GUILayout.Width(80));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Init"))
        {
            item.Init();
        }
        if (GUILayout.Button("Recover"))
        {
            item.Recover();
        }
        if (GUILayout.Button("Align"))
        {
            item.DoAlign();
        }
        if (GUILayout.Button("AlignRoot"))
        {
            item.DoAlignRoot();
        }
        GUILayout.EndHorizontal();
    }
}
