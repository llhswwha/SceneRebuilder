using CodeStage.AdvancedFPSCounter.Editor.UI;
using MeshJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshReplace))]
public class MeshReplaceEditor : BaseFoldoutEditor<MeshReplace>
{
    public override void OnToolLayout(MeshReplace item)
    {
        base.OnToolLayout(item);
        MeshReplaceEditor.DrawUI(item);
        if (GUILayout.Button("Window"))
        {
            MeshReplaceEditorWindow.ShowWindow();
        }
    }

    public static Dictionary<MeshReplaceItem,FoldoutEditorArg<MeshReplaceTarget>> targetListArgs = new Dictionary<MeshReplaceItem, FoldoutEditorArg<MeshReplaceTarget>>();

    public static FoldoutEditorArg<MeshReplaceTarget> GetTargetListArg(MeshReplaceItem item)
    {
        if (!targetListArgs.ContainsKey(item))
        {
            targetListArgs.Add(item, new FoldoutEditorArg<MeshReplaceTarget>(true,false));
        }
        return targetListArgs[item];
    }

    internal static void DrawUI(MeshReplace item)
    {
        if (item == null) return;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Align", GUILayout.Width(60));
        item.transfromReplaceSetting.Align = (TransfromAlignMode)EditorGUILayout.EnumPopup(item.transfromReplaceSetting.Align, GUILayout.Width(80));
        GUILayout.Label("SetPosition",GUILayout.Width(80));
        item.transfromReplaceSetting.SetPosition = EditorGUILayout.Toggle(item.transfromReplaceSetting.SetPosition, GUILayout.Width(15));
        GUILayout.Label("SetScale", GUILayout.Width(80));
        item.transfromReplaceSetting.SetScale = EditorGUILayout.Toggle(item.transfromReplaceSetting.SetScale, GUILayout.Width(15));
        GUILayout.Label("SetRotation", GUILayout.Width(80));
        item.transfromReplaceSetting.SetRotation = EditorGUILayout.Toggle(item.transfromReplaceSetting.SetRotation, GUILayout.Width(15));
        GUILayout.Label("SetMirrorX", GUILayout.Width(80));
        item.transfromReplaceSetting.SetMirrorX = EditorGUILayout.Toggle(item.transfromReplaceSetting.SetMirrorX, GUILayout.Width(15));
        GUILayout.Label("SetPosX", GUILayout.Width(60));
        item.transfromReplaceSetting.SetPosX = EditorGUILayout.Toggle(item.transfromReplaceSetting.SetPosX, GUILayout.Width(15));
        GUILayout.Label("SetPosY", GUILayout.Width(60));
        item.transfromReplaceSetting.SetPosY = EditorGUILayout.Toggle(item.transfromReplaceSetting.SetPosY, GUILayout.Width(15));
        GUILayout.Label("SetPosZ", GUILayout.Width(60));
        item.transfromReplaceSetting.SetPosZ = EditorGUILayout.Toggle(item.transfromReplaceSetting.SetPosZ, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Compare"))
        {
            item.Compare();
        }
        if (GUILayout.Button("Replace"))
        {
            item.Replace();
        }
        if (GUILayout.Button("ReplacePrefab"))
        {
            item.ReplacePrefab();
        }
        if (GUILayout.Button("ReplaceEx"))
        {
            item.ReplaceEx();
        }
        if (GUILayout.Button("Mirror"))
        {
            item.Mirror();
        }
        if (GUILayout.Button("Clear"))
        {
            item.ClearNewGos();
        }
        if (GUILayout.Button("Apply"))
        {
            item.ApplyNewGos();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectPrefabs"))
        {
            item.SelectPrefabs();
        }
        if (GUILayout.Button("SelectTargets"))
        {
            item.SelectTargets();
        }
        if (GUILayout.Button("SelectNew"))
        {
            item.SelectNewGos();
        }
        EditorGUILayout.EndHorizontal();

        EditorUIUtils.SetupStyles();

        EditorUIUtils.Separator(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"Items:({item.Items.Count})", GUILayout.Width(60));
        if (GUILayout.Button("+"))
        {
            item.Items.Add(new MeshReplaceItem());
            //MeshReplaceEditorWindow.ShowWindow();
        }
        if (GUILayout.Button("-"))
        {
            item.Items.RemoveAt(item.Items.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        foreach (var subItem in item.Items)
        {
            if (subItem == null) continue;

            EditorUIUtils.Separator(1);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Prefab", GUILayout.Width(75));
            if (GUILayout.Button("+|", GUILayout.Width(20)))
            {
                Debug.Log("+" + Selection.activeObject);
                subItem.prefab = Selection.activeObject as GameObject;
                MeshPoints ps = new MeshPoints(subItem.prefab);
                subItem.prefabVertexCount = ps.vertexCount;
            }
            if (subItem.prefab != null)
            {
                if (GUILayout.Button($"{subItem.prefab.name}[{subItem.prefabVertexCount}]"))
                {
                    EditorHelper.SelectObject(subItem.prefab);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    Debug.Log("-" + subItem.prefab);
                    subItem.prefab = null;
                }
            }

            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Label($"Target({subItem.targetList.Count})", GUILayout.Width(75));

            
            //for (int i = 0; i < subItem.targetList.Count; i++)
            //{
            //    GameObject target = subItem.targetList[i];
            //    if (target == null)
            //    {
            //        subItem.targetList.RemoveAt(i);
            //        i--;
            //        continue;
            //    }
            //    if (GUILayout.Button(target.name))
            //    {
            //        EditorHelper.SelectObject(target);
            //    }
            //    if (GUILayout.Button("-", GUILayout.Width(20)))
            //    {
            //        Debug.Log("-" + target);
            //        subItem.targetList.RemoveAt(i);
            //        i--;
            //    }
            //}
            //EditorGUILayout.EndHorizontal();

            var targetListArg = GetTargetListArg(subItem);
            BaseFoldoutEditorHelper.DrawObjectList<MeshReplaceTarget>("Target", subItem.targetList, targetListArg, ()=>
            {
                if (GUILayout.Button("+Selection", GUILayout.Width(80)))
                {
                    Debug.Log("AddSelection +activeObject:" + Selection.activeObject);
                    //subItem.targetList.Add(Selection.activeObject as GameObject);

                    foreach (GameObject obj in Selection.objects)
                    {
                        Debug.Log("+object:" + obj);
                        subItem.AddTarget(obj);
                    }
                    subItem.targetList.Sort();
                    Debug.Log("subItem.targetList:" + subItem.targetList.Count);
                }
                if (GUILayout.Button("+Children", GUILayout.Width(80)))
                {
                    Debug.Log("AddSelection Children +activeObject:" + Selection.activeObject);
                    //subItem.targetList.Add(Selection.activeObject as GameObject);

                    foreach (GameObject obj in Selection.objects)
                    {

                        for (int i = 0; i < obj.transform.childCount; i++)
                        {
                            var child = obj.transform.GetChild(i);
                            Debug.Log("+child:" + child);
                            subItem.AddTarget(child.gameObject);
                        }
                        //subItem.targetList.Add(obj);
                    }
                    subItem.targetList.Sort();
                    Debug.Log("subItem.targetList:" + subItem.targetList.Count);
                }

                if (GUILayout.Button("+AllRenderers", GUILayout.Width(100)))
                {
                    Debug.Log("AddSelection Children +activeObject:" + Selection.activeObject);
                    //subItem.targetList.Add(Selection.activeObject as GameObject);

                    foreach (GameObject obj in Selection.objects)
                    {
                        var renderers = obj.GetComponentsInChildren<MeshRenderer>(true);
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            var child = renderers[i];
                            Debug.Log("+child:" + child);
                            subItem.AddTarget(child.gameObject);
                        }
                        //subItem.targetList.Add(obj);
                    }
                    subItem.targetList.Sort();
                    Debug.Log("subItem.targetList:" + subItem.targetList.Count);
                }

                if (GUILayout.Button("Clear", GUILayout.Width(50)))
                {
                    Debug.Log("R" + subItem.targetList.Count);
                    subItem.targetList.Clear();
                }

            }, (obj)=>
            {
                GUILayout.Label(obj.dis.ToString("F5"), GUILayout.Width(100));

            },(obj)=>
            {
                Debug.Log("-" + obj);
                subItem.targetList.Remove(obj);
            });
        }
    }
}
