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
        GUILayout.Label("zM");
        item.disSetting.zeroM = EditorGUILayout.DoubleField(item.disSetting.zeroM);
        GUILayout.Label("zP");
        item.disSetting.zeroP = EditorGUILayout.DoubleField(item.disSetting.zeroP, GUILayout.Width(50));
        GUILayout.Label("pC");
        item.disSetting.zeroPMaxCount = EditorGUILayout.IntField(item.disSetting.zeroPMaxCount, GUILayout.Width(50));
        GUILayout.Label("mDis");
        item.disSetting.zeroMMaxDis = EditorGUILayout.DoubleField(item.disSetting.zeroMMaxDis,GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawUI(MeshComparer item)
    {
        if (item == null) return;
        DrawSetting(item);

        EditorGUILayout.BeginHorizontal();
        item.goFrom = BaseEditorHelper.ObjectField(">",20, item.goFrom);
        
        item.goFromCopy = BaseEditorHelper.ObjectField(">", 20, item.goFromCopy, 100);
        item.goTo = BaseEditorHelper.ObjectField(">", 20, item.goTo);
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Copy"))
        {
            item.CopyGo2();
        }
        if (GUILayout.Button("Switch"))
        {
            item.SwitchGO();
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
        

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.ShowLog = GUILayout.Toggle(item.ShowLog, "Log",GUILayout.Width(40));
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



        GUILayout.Label("Dis:" + item.distance);
        //if (GUILayout.Button("CopyTest"))
        //{
        //    item.CopyTest();
        //}
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("*Align"))
        {
            item.AcRTAlign();
            EditorHelper.SelectObject(item.goFromCopy);
        }
        if (GUILayout.Button("*AlignJob"))
        {
            item.AcRTAlignJob();
            EditorHelper.SelectObject(item.goFromCopy);
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
        if (GUILayout.Button("TryScales"))
        {
            item.TryScales();
        }
        if (GUILayout.Button("TryAngles"))
        {
            item.TryAngles();
        }
        if (GUILayout.Button("TryAnglesM1"))
        {
            item.TryAnglesMatrix(false);
        }
        if (GUILayout.Button("TryAnglesM2"))
        {
            item.TryAnglesMatrix(true);
        }
        if (GUILayout.Button("Rotate2"))
        {
            item.Rotate2();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        int x= EditorGUILayout.IntField(item.TryAngle.x, GUILayout.Width(30));
        int y = EditorGUILayout.IntField(item.TryAngle.y, GUILayout.Width(30));
        int z = EditorGUILayout.IntField(item.TryAngle.z, GUILayout.Width(30));
        item.TryAngle = new Vector3Int(x, y, z);
        if (GUILayout.Button("Try"))
        {
            var go=item.TryMatrixAngle();
            EditorHelper.SelectObject(go);
        }
        item.TryAngleId = EditorGUILayout.IntField(item.TryAngleId, GUILayout.Width(30));
        if (GUILayout.Button("TryN"))
        {
            var go=item.TryMatrixAngleN();
            EditorHelper.SelectObject(go);
        }
        if (GUILayout.Button("Clear"))
        {
            item.ClearTmpObjects();
        }
        if (GUILayout.Button("0_0_0"))
        {
            var go=item.TryMatrixAngle0_0_0();
            EditorHelper.SelectObject(go);
        }
        if (GUILayout.Button("90_0_0"))
        {
            var go=item.TryMatrixAngle90_0_0();
            EditorHelper.SelectObject(go);
        }
        if (GUILayout.Button("180_0_0"))
        {
            var go=item.TryMatrixAngle(new Vector3(180, 0, 0));
            EditorHelper.SelectObject(go);
        }
        if (GUILayout.Button("270_0_0"))
        {
            var go=item.TryMatrixAngle(new Vector3(270,0,0));
            EditorHelper.SelectObject(go);
        }
        if (GUILayout.Button("0_90_0"))
        {
            var go=item.TryMatrixAngle0_90_0();
            EditorHelper.SelectObject(go);
        }
        if (GUILayout.Button("0_0_90"))
        {
            var go=item.TryMatrixAngle0_0_90();
            EditorHelper.SelectObject(go);
        }

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("InitMeshNodeInfo"))
        {
            item.InitMeshNodeInfo();
        }
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
