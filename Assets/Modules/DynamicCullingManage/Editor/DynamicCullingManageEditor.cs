using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DynamicCullingManage))]
public class DynamicCullingManageEditor : BaseFoldoutEditor<DynamicCullingManage>
{
    public override void OnToolLayout(DynamicCullingManage cullingManange)
    {
        cullingManange.GetDynamicCulling();
        cullingManange.Target = BaseEditorHelper.ObjectFieldH("Target", cullingManange.Target);
        cullingManange.dynamicCulling = BaseEditorHelper.ObjectFieldH("DynamicCulling", cullingManange.dynamicCulling);

        GUILayout.BeginHorizontal();
        cullingManange.CullingTimeInterval = EditorGUILayout.FloatField(cullingManange.CullingTimeInterval, "Interval");
        cullingManange.ObjectsLifetime = EditorGUILayout.FloatField(cullingManange.ObjectsLifetime, "Lifetime");
        cullingManange.JobsPerFrame = EditorGUILayout.IntField(cullingManange.JobsPerFrame, "JobsPerFrame");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start"))
        {
            cullingManange.SetCullingRenders();
        }
        if (GUILayout.Button("Stop"))
        {
            cullingManange.StopCulling();
        }
        if (GUILayout.Button("Pause"))
        {
            cullingManange.PauseCulling();
        }
        if (GUILayout.Button("Continue"))
        {
            cullingManange.ContinueCulling();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ShowAll"))
        {
            cullingManange.ShowAllRenderers();
        }
        if (GUILayout.Button("HideAll"))
        {
            cullingManange.HideAllRenderers();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("AddObjectsForCulling"))
        {
            cullingManange.AddObjectsForCulling();
        }

        GUILayout.BeginHorizontal();
        cullingManange.IsDynamicCulling = GUILayout.Toggle(cullingManange.IsDynamicCulling, "IsStartCulling");
        cullingManange.IsInEditor = GUILayout.Toggle(cullingManange.IsInEditor, "InEditor");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        cullingManange.IsAutoUpdateCullingInfo = GUILayout.Toggle(cullingManange.IsAutoUpdateCullingInfo, "AutoUpdateInfo");
        if (GUILayout.Button("UpdateInfo"))
        {
            cullingManange.UpdateCullingInfo();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label(cullingManange.CullingInfo);
        //base.OnToolLayout(showManager);
        //DrawItemList<>
    }
}
