using MeshProfilerNS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MeshManagerEditorWindow : EditorWindow
{
    [MenuItem("Window/Tools/ MeshManager")]
public static void AddWindow()
{
    SceneRebuildEditorWindow window = (SceneRebuildEditorWindow)EditorWindow.GetWindowWithRect(typeof(SceneRebuildEditorWindow), new Rect(0, 0, MPGUIStyles.SCREEN_WIDTH * 0.8f, MPGUIStyles.SCREEN_HEIGHT * 0.8f), true, "MeshManagerEditorWindow");
    window.Show();
    window.Init();
}

public void Init()
{
    MPGUIStyles.InitGUIStyles();
    RefleshList();
    //RefleshDataChart();
    //RefleshBarChart();
    EditorSceneManager.sceneOpened -= OnOpenedScene;
    EditorSceneManager.sceneOpened += OnOpenedScene;

}
public void OnDestroy()
{
    EditorSceneManager.sceneOpened -= OnOpenedScene;
}

void OnOpenedScene(Scene sce, OpenSceneMode mode)
{
    MPGUIStyles.InitGUIStyles();
    RefleshList();
}

/// <summary>
/// 刷新列表，重新搜索
/// </summary>
void RefleshList()
{
    //meshElementList.Clear();
    //meshElementList.AddRange(MeshFinder.GetMeshElementList());
    //Sort(meshElementList, sortWays);
    //SelectByConditions(meshElementList);
    //originList.Clear();
    //originList.AddRange(meshElementList);
    //if (meshElementList.Count > 0)
    //    SelectIndex = 0;
    //else
    //    SelectIndex = -1;
    //pageIndex = 0;
    //scVector = Vector2.zero;
}

/// <summary>
/// UI主函数
/// </summary>
private void OnGUI()
{
    if (!MPGUIStyles.IsDirty)
    {
        Init();
    }
    //DrawSettingBlock();
    //DrawPreviewBlock();
    //DrawPageIndexBlock();
    //DrawListBlock();
    //DrawInputTextField();
    //DrawToolBlock();
    //DrawChartBlock();
    //DrawDataBlock();
}
}