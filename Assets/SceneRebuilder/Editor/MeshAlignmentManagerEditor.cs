using System;
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

        MeshAlignmentManagerEditor.DrawUI(item);

        if (GUILayout.Button("Window"))
        {
            MeshAlignmentManagerEditorWindow.ShowWindow();
        }
    }

    internal static void DrawUI(MeshAlignmentManager item)
    {
        if (item == null) return;
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("SourceR", GUILayout.Width(60));
        //item.SourceRoot = EditorGUILayout.ObjectField(item.SourceRoot, typeof(GameObject)) as GameObject;
        //GUILayout.Label("TargetR", GUILayout.Width(60));
        //item.TargetRoot = EditorGUILayout.ObjectField(item.TargetRoot, typeof(GameObject)) as GameObject;
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("SourceR", GUILayout.Width(60));
        //item.SourceRoot = BaseEditorHelper.ObjectField(item.SourceRoot);
        //GUILayout.Label("Source", GUILayout.Width(60));
        //item.Source = BaseEditorHelper.ObjectField(item.Source);
        //GUILayout.Label("Target", GUILayout.Width(60));
        //item.Target = BaseEditorHelper.ObjectField(item.Target);
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("SourceR", GUILayout.Width(60));
        GameObject go1= BaseEditorHelper.ObjectField(item.SourceRoot);
        if (go1 != item.SourceRoot)
        {
            item.SourceRoot = go1;
            item.Init();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Source", GUILayout.Width(60));
        GameObject go2 = BaseEditorHelper.ObjectField(item.Source);
        if (go2 != item.Source)
        {
            item.Source = go2;
            item.Init();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Target", GUILayout.Width(60));
        GameObject go3 = BaseEditorHelper.ObjectField(item.Target);
        if(go3!= item.Target)
        {
            item.Target = go3;
            item.Init();
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (item.Target != null && item.Source != null)
        {
            Vector3 offset = item.Target.transform.position - item.Source.transform.position;
            GUILayout.Label($"offset:({offset.x},{offset.y},{offset.z})");
        }
        else
        {
            GUILayout.Label($"offset:(NULL)");
        }

        item.transfromReplaceSetting.SetPosX = GUILayout.Toggle(item.transfromReplaceSetting.SetPosX, "X", GUILayout.Width(40));
        item.transfromReplaceSetting.SetPosY = GUILayout.Toggle(item.transfromReplaceSetting.SetPosY, "Y", GUILayout.Width(40));
        item.transfromReplaceSetting.SetPosZ = GUILayout.Toggle(item.transfromReplaceSetting.SetPosZ, "Z", GUILayout.Width(40));
        item.transfromReplaceSetting.Align = (TransfromAlignMode)EditorGUILayout.EnumPopup(item.transfromReplaceSetting.Align, GUILayout.Width(80));
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
