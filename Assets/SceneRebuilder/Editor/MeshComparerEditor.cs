using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshComparer))]
public class MeshComparerEditor : BaseEditor<MeshComparer>
{
    public static void DrawSetting(MeshComparer item)
    {
        EditorGUILayout.BeginHorizontal();
        item.step = (AlignDebugStep)EditorGUILayout.EnumPopup(item.step);
        item.mode = (AlignRotateMode)EditorGUILayout.EnumPopup(item.mode);
        GUILayout.Label("zeroM:");
        item.disSetting.zeroM = EditorGUILayout.DoubleField(item.disSetting.zeroM);
        GUILayout.Label("zeroP:");
        item.disSetting.zeroP = EditorGUILayout.DoubleField(item.disSetting.zeroP);
        GUILayout.Label("PCount:");
        item.disSetting.zeroPMaxCount = EditorGUILayout.IntField(item.disSetting.zeroPMaxCount);
        GUILayout.Label("MaxDis:");
        item.disSetting.zeroMMaxDis = EditorGUILayout.DoubleField(item.disSetting.zeroMMaxDis);
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawUI(MeshComparer item)
    {
        DrawSetting(item);

        EditorGUILayout.BeginHorizontal();
        item.goFrom = BaseEditorHelper.ObjectField("From", item.goFrom);
        if (GUILayout.Button("Copy"))
        {
            item.CopyGo2();
        }
        item.goFromCopy = BaseEditorHelper.ObjectField("FromC", item.goFromCopy);
        item.goTo = BaseEditorHelper.ObjectField("To", item.goTo);
        if (GUILayout.Button("Switch"))
        {
            item.SwitchGO();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.ShowLog = GUILayout.Toggle(item.ShowLog, "Log");
        if (GUILayout.Button("Dis12"))
        {
            item.GetDistance12();
        }
        if (GUILayout.Button("Dis12C"))
        {
            item.GetDistance12C();
        }
        if (GUILayout.Button("Dis22C"))
        {
            item.GetDistance22C();
        }

        if (GUILayout.Button("Show0"))
        {
            item.goFrom.SetActive(true);
            item.goTo.SetActive(false);
            Selection.activeObject = item.goFrom;
            //EditorHelper.SelectObject(item.goFrom);
        }
        if (GUILayout.Button("Show1"))
        {
            item.goFrom.SetActive(false);
            item.goTo.SetActive(true);
            Selection.activeObject = item.goTo;
            //EditorHelper.SelectObject(item.goTo);
        }
        if (GUILayout.Button("Show01"))
        {
            item.goFrom.SetActive(true);
            item.goTo.SetActive(true);

            //EditorHelper.SelectObjects(new List<GameObject>() { item.goFrom,item.goTo });
        }
        if (GUILayout.Button("Hide01"))
        {
            item.goFrom.SetActive(false);
            item.goTo.SetActive(false);
        }

        GUILayout.Label("Dis:" + item.distance);
        //if (GUILayout.Button("CopyTest"))
        //{
        //    item.CopyTest();
        //}
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Align"))
        {
            item.AcRTAlign();
        }
        if (GUILayout.Button("AlignJob"))
        {
            item.AcRTAlignJob();
        }
        if (GUILayout.Button("TestRTAlignJob"))
        {
            item.TestRTAlignJob();
        }
        if (GUILayout.Button("TestMeshAlignJob"))
        {
            item.TestMeshAlignJob();
        }
        if (GUILayout.Button("AlignModels"))
        {
            item.AlignModels();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.MaxICPCount = EditorGUILayout.IntField(item.MaxICPCount);
        if (GUILayout.Button("TestICP"))
        {
            item.TestICP();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("InitMeshNodeInfo"))
        {
            item.InitMeshNodeInfo();
        }
        if (GUILayout.Button("TryScales"))
        {
            item.TryScales();
        }
        if (GUILayout.Button("TryAngles"))
        {
            item.TryAngles();
        }
        if (GUILayout.Button("TryAnglesMatrix"))
        {
            item.TryAnglesMatrix();
        }
        if (GUILayout.Button("Rotate2"))
        {
            item.Rotate2();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("TryMA_0_0_0"))
        {
            item.TryMatrixAngle0_0_0();
        }
        if (GUILayout.Button("TryMA_90_0_0"))
        {
            item.TryMatrixAngle90_0_0();
        }
        if (GUILayout.Button("TryMA_180_0_0"))
        {
            item.TryMatrixAngle180_0_0();
        }
        if (GUILayout.Button("TryMA_0_90_0"))
        {
            item.TryMatrixAngle0_90_0();
        }
        if (GUILayout.Button("TryMA_0_0_90"))
        {
            item.TryMatrixAngle0_0_90();
        }

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CreatePlaneByThreePoint"))
        {
            item.CreatePlaneByThreePoint();
        }
        if (GUILayout.Button("ReplacePrefab"))
        {
            item.ReplacePrefab();
        }
        EditorGUILayout.EndHorizontal();
    }

    public override void OnToolLayout(MeshComparer item)
    {
        base.OnToolLayout(item);

        MeshComparerEditor.DrawUI(item);

        if (GUILayout.Button("Window"))
        {
            MeshComparerEditorWindow.ShowWindow();
        }
    }
}
