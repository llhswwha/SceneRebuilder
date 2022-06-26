using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingScenesLoadManager))]
public class BuildingScenesLoadManagerEditor : BaseFoldoutEditor<BuildingScenesLoadManager>
{
    public static FoldoutEditorArg userBuildingsListfoldoutArg = new FoldoutEditorArg(true,true,true,true,true);

    public static FoldoutEditorArg settingItemsListfoldoutArg = new FoldoutEditorArg(true, true, true, true, true);

    public override void OnToolLayout(BuildingScenesLoadManager loadManager)
    {
        base.OnToolLayout(loadManager);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("UpdateXml"))
        {
            loadManager.UpdateSettingByScene();
            loadManager.SaveXml();
        }
        if (GUILayout.Button("InitXml"))
        {
            loadManager.InitSettingByScene();
            loadManager.SaveXml();
        }
        if (GUILayout.Button("SaveXml"))
        {
            loadManager.SaveXml();
        }
        if (GUILayout.Button("LoadXml"))
        {
            loadManager.LoadSettingXml();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        loadManager.Setting.IsLoadUserBuildings = GUILayout.Toggle(loadManager.Setting.IsLoadUserBuildings, "LoadUserBuildings", GUILayout.Width(135));
        loadManager.IsShowLog = GUILayout.Toggle(loadManager.IsShowLog, "ShowLog", GUILayout.Width(80));
        loadManager.IsEnableGPU = GUILayout.Toggle(loadManager.IsEnableGPU, "EnableGPU", GUILayout.Width(90));
        EditorGUILayout.LabelField($"|", GUILayout.Width(10));
        loadManager.IsLoadScene = GUILayout.Toggle(loadManager.IsLoadScene, "IsLoadScene", GUILayout.Width(100));
        loadManager.Setting.IsLoadSceneOnStart = GUILayout.Toggle(loadManager.Setting.IsLoadSceneOnStart, "LoadSceneOnStart", GUILayout.Width(135));
        EditorGUILayout.LabelField($"|", GUILayout.Width(10));
        loadManager.Setting.Enabled = GUILayout.Toggle(loadManager.Setting.Enabled, "EnableLoad", GUILayout.Width(95));
        loadManager.Setting.IsEnableUnload = GUILayout.Toggle(loadManager.Setting.IsEnableUnload, "EnableUnload", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        DrawUserBuildings(loadManager);
        DrawSettingItems(loadManager);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("LoadUserBuildings"))
        {
            loadManager.LoadUserBuildings();
        }
        if (GUILayout.Button("LoadScenesBySetting"))
        {
            loadManager.LoadScenesBySetting();
        }
        EditorGUILayout.EndHorizontal();
    }

    private static void DrawUserBuildings(BuildingScenesLoadManager loadManager)
    {
        var btnStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        //btnStyle.margin = new RectOffset(0, 0, 0, 0);
        //btnStyle.padding = new RectOffset(0, 0, 0, 0);
        btnStyle.alignment = TextAnchor.MiddleLeft;
        DrawItemList(userBuildingsListfoldoutArg, "UserBuildings", loadManager.Setting.UserBulindgs, () =>
        {
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                loadManager.Setting.UserBulindgs.Clear();
            }
            if (GUILayout.Button("TestAll", GUILayout.Width(60)))
            {
                loadManager.InitUserBuildings2();
            }
            if (GUILayout.Button("Test(F)", GUILayout.Width(60)))
            {
                loadManager.Setting.UserBulindgs.Clear();
                loadManager.Setting.UserBulindgs.AddRange(new List<string>() { "F级主厂房", "F级余热锅炉" });
            }
            if (GUILayout.Button("Test(FH)", GUILayout.Width(60)))
            {
                loadManager.Setting.UserBulindgs.Clear();
                loadManager.Setting.UserBulindgs.AddRange(new List<string>() { "F级主厂房", "F级余热锅炉", "H级余热锅炉", "H级汽机厂房", "H级燃机厂房" });
            }
        }, (arg, item, index) =>
        {
            EditorGUILayout.BeginHorizontal();
            EditorUIUtils.BeforeSpace(arg.level + 1);
            EditorGUILayout.LabelField($"[{index + 1:00}]", GUILayout.Width(30));
            if (GUILayout.Button(item, btnStyle))
            {
                GameObject obj = BuildingScenesLoadManager.FindBuildingByName(item);
                if (obj != null)
                {
                    EditorHelper.SelectObject(obj);
                }
                else
                {
                    Debug.LogError($"Not Found Name:{item}");
                }
            }
            if (GUILayout.Button("X", btnStyle, GUILayout.Width(15)))
            {
                loadManager.Setting.UserBulindgs.Remove(item);
            }
            EditorGUILayout.EndHorizontal();
        }, null, null);
    }

    private static void DrawSettingItems(BuildingScenesLoadManager loadManager)
    {
        var btnStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        //btnStyle.margin = new RectOffset(0, 0, 0, 0);
        //btnStyle.padding = new RectOffset(0, 0, 0, 0);
        btnStyle.alignment = TextAnchor.MiddleLeft;
        DrawItemList(settingItemsListfoldoutArg, "SettingItems", loadManager.Setting.Items, () =>
        {
            if (GUILayout.Button("Sort", GUILayout.Width(60)))
            {
                loadManager.Setting.Items.Sort();
            }
        }, (arg, item, index) =>
        {
            EditorGUILayout.BeginHorizontal();
            EditorUIUtils.BeforeSpace(arg.level + 1);
            item.IsEnable = EditorGUILayout.Toggle(item.IsEnable, GUILayout.Width(15));
            EditorGUILayout.LabelField($"[{index + 1:00}]", GUILayout.Width(30));
            if (GUILayout.Button(item.Name, btnStyle))
            {
                GameObject obj = BuildingScenesLoadManager.FindBuildingByName(item.Name);
                if (obj != null)
                {
                    EditorHelper.SelectObject(obj);
                }
                else
                {
                    Debug.LogError($"Not Found Name:{item.Name}");
                }
            }
            EditorGUILayout.LabelField("Test", GUILayout.Width(32));
            bool isTest = loadManager.Setting.UserBulindgs.Contains(item.Name);
            bool isTest2 = EditorGUILayout.Toggle(isTest, GUILayout.Width(15));
            if (isTest2 != isTest)
            {
                if (isTest2)
                {
                    loadManager.Setting.UserBulindgs.Add(item.Name);
                }
                else
                {
                    loadManager.Setting.UserBulindgs.Remove(item.Name);
                }
            }
            EditorGUILayout.EndHorizontal();
        }, (item) =>
        {
            if (GUILayout.Button(item.Name))
            {

            }
        }, null);
    }
}
