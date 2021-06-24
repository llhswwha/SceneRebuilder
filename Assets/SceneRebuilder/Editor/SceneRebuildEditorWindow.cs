using MeshProfilerNS;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRebuildEditorWindow : EditorWindow
{
    [MenuItem("Window/Tools/SceneRebuild")]
    public static void AddWindow()
    {
        SceneRebuildEditorWindow window = (SceneRebuildEditorWindow)EditorWindow.GetWindowWithRect(typeof(SceneRebuildEditorWindow), new Rect(0, 0, MPGUIStyles.SCREEN_WIDTH*0.8f, MPGUIStyles.SCREEN_HEIGHT*0.8f), true, "SceneRebuildEditorWindow");
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
        meshElementList.Clear();

        //meshElementList.AddRange(MeshFinder.GetMeshElementList());
        
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>();
        foreach(var b in buildings)
        {
            BuildingModelElement ele = new BuildingModelElement(b.gameObject,false,false);
            meshElementList.Add(ele);
            ele.rootMeshValue = new BuildingModelValues();

            if (b.InPart)
            {
                BuildingModelValues eleValues = new BuildingModelValues();
                ele.childList.Add(eleValues);
            }
            if (b.OutPart0)
            {
                BuildingModelValues eleValues = new BuildingModelValues();
                ele.childList.Add(eleValues);
            }
            if (b.OutPart1)
            {
                BuildingModelValues eleValues = new BuildingModelValues();
                ele.childList.Add(eleValues);
            }
        }

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
        DrawListBlock();
        //DrawInputTextField();
        DrawToolBlock();
        //DrawChartBlock();
        //DrawDataBlock();
    }

    void DrawToolBlock()
    {
        GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.TOOL_BLOCK), "");
        int border = 10;
        GUILayout.BeginArea(MPGUIStyles.BorderArea(new Rect(MPGUIStyles.TOOL_BLOCK.x + border, MPGUIStyles.TOOL_BLOCK.y + 5, MPGUIStyles.TOOL_BLOCK.width - 2 * border, MPGUIStyles.TOOL_BLOCK.height - 2 * border)));
        GUILayout.Label("Tool Panel", MPGUIStyles.centerStyle);
        //GUILayout.Space(15);
        int buttonHeight = 29;

        if (GUILayout.Button("Export Excel Data", GUILayout.Height(buttonHeight)))
        {
            //ExportExcelData();
        }
        GUILayout.EndArea();

    }

    //UI辅助参数
    int selectIndex = 0;//父物体选择下标
    int SelectIndex
    {
        get { return selectIndex; }
        set
        {
            selectIndex = value;
            if (selectIndex >= 0)
            {
                if (meshElementList[selectIndex].isGroup)
                {
                    InitPreview(meshElementList[selectIndex].rootObj);
                }
                else
                {
                    InitPreview(meshElementList[selectIndex].rootMeshValue.mesh);
                }
            }

        }
    }

    int selectChildIndex = 0;//子物体选择下标
    int SelectChildIndex
    {
        get { return selectChildIndex; }
        set
        {
            selectChildIndex = value;
            if (selectChildIndex >= 0)
                InitPreview(meshElementList[SelectIndex].childList[selectChildIndex].mesh);
        }
    }

    string[] tableTitle = new string[] { "name", "Vertex", "RefCount", "Triangles", "Normals", "Tangents", "Colors", "UV", "UV2", "UV3", "UV4", "Memory", "Readable", };
    List<BuildingModelElement> meshElementList = new List<BuildingModelElement>();
    List<BuildingModelElement> originList = new List<BuildingModelElement>();

    //Setting辅助参数
    Editor previewEditor;

    Vector2 scVector = new Vector2(0, 0);
    bool isRetract = false;//是否折叠
    const int pageCount = 100;
    int pageIndex = 0;
    /// <summary>
    /// 绘制列表面板
    /// </summary>
    void DrawListBlock()
    {

        GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.LIST_BLOCK));
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < tableTitle.Length; i++)
        {
            GUILayout.Label(tableTitle[i], MPGUIStyles.itemStyle, i == 0 ? GUILayout.Width(220) : GUILayout.Width(80));
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        scVector = GUILayout.BeginScrollView(scVector);
        for (int i = pageIndex * pageCount; i < (pageIndex + 1) * pageCount; i++)
        {
            if (i < meshElementList.Count)
            {
                bool isSelect = SelectIndex == i;
                DrawItem(meshElementList[i], isSelect, i);
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    /// <summary>
    /// 绘制条目框
    /// </summary>
    /// <param name="element"></param>
    /// <param name="isSelect"></param>
    /// <param name="index"></param>
    void DrawItem(BuildingModelElement element, bool isSelect, int index)
    {
        GUIStyle lineStyle = isSelect ? MPGUIStyles.itemBtnStyles[1] : MPGUIStyles.itemBtnStyles[0];
        GUILayout.BeginHorizontal();
        BuildingModelValues meshValueRoot = element.rootMeshValue;

        GUIContent icon_expand_Content = element.isGroup ? MPGUIStyles.icon_down_Content : MPGUIStyles.icon_right_Content;

        if (element.isGroup && GUILayout.Button(isRetract && isSelect ? MPGUIStyles.icon_retract_Contents[1] : MPGUIStyles.icon_retract_Contents[0], isSelect ? MPGUIStyles.icon_tab_Style : MPGUIStyles.icon_tab_normal_Style, MPGUIStyles.options_icon))
        {
            SelectIndex = index;
            isRetract = !isRetract;
            SelectChildIndex = -1;
        }
        if (GUILayout.Button(element.name, lineStyle, GUILayout.Height(30), element.isGroup ? GUILayout.Width(200) : GUILayout.Width(220)))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }

        GUILayoutOption[] btnOption = isSelect ? MPGUIStyles.options_exist : MPGUIStyles.options_none;


        if (GUILayout.Button(meshValueRoot.vertCount.ToString(), lineStyle, btnOption))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }

        if (isSelect && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        {
            if (element.isGroup)
                isRetract = !isRetract;
            else
            {
                ShowDataList.AddWindow(meshValueRoot.GetVerticsStr(), "Vertices" + "-" + element.name);
            }
        }

        if (GUILayout.Button(element.refList.Count.ToString(), lineStyle, btnOption))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }

        if (isSelect && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        {
            ShowRefList.AddWindow(element.refList, "RefList" + "-" + element.name);
        }

        if (GUILayout.Button(element.rootMeshValue.triangles.ToString(), lineStyle, btnOption))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }
        if (isSelect && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        {
            if (element.isGroup)
                isRetract = !isRetract;
            else
            {
                ShowDataList.AddWindow(meshValueRoot.GetTrianglesStr(), "Triangles" + "-" + element.name);
            }
        }

        if (GUILayout.Button(meshValueRoot.exist_normals ? "√" : "", lineStyle, (isSelect && meshValueRoot.exist_normals) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }
        if (isSelect && meshValueRoot.exist_normals && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        {
            if (element.isGroup)
                isRetract = !isRetract;
            else
            {
                ShowDataList.AddWindow(meshValueRoot.GetNormalsStr(), "Normals" + "-" + element.name);
            }
        }

        if (GUILayout.Button(meshValueRoot.exist_tangents ? "√" : "", lineStyle, (isSelect && meshValueRoot.exist_tangents) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }
        if (isSelect && meshValueRoot.exist_tangents && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        {
            if (element.isGroup)
                isRetract = !isRetract;
            else
            {
                ShowDataList.AddWindow(meshValueRoot.GetTangentsStr(), "Tangents" + "-" + element.name);
            }
        }
        if (GUILayout.Button(meshValueRoot.exist_colors ? "√" : "", lineStyle, (isSelect && meshValueRoot.exist_colors) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }
        if (isSelect && meshValueRoot.exist_colors && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        {
            if (element.isGroup)
                isRetract = !isRetract;
            else
            {
                ShowDataList.AddWindow(meshValueRoot.GetColorsStr(), "Colors" + "-" + element.name);
            }
        }

        for (int m = 0; m < 4; m++)
        {
            if (GUILayout.Button(meshValueRoot.exist_uv[m] ? "√" : "", lineStyle, (isSelect && meshValueRoot.exist_uv[m]) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
            {
                if (SelectIndex != index)
                {
                    isRetract = false;
                }
                SelectIndex = index;
                SelectChildIndex = -1;
            }
            if (isSelect && meshValueRoot.exist_uv[m] && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
            {
                if (element.isGroup)
                    isRetract = !isRetract;
                else
                {
                    ShowDataList.AddWindow(meshValueRoot.GetUVStr(m), "UV" + (m + 1) + "-" + element.name);
                }
            }
        }
        string memoryStr = meshValueRoot.memory >= 1000 ? (meshValueRoot.memory / 1000.0f).ToString("F2") + "MB" : meshValueRoot.memory.ToString("F2") + "KB";


        if (GUILayout.Button(memoryStr, lineStyle, MPGUIStyles.options_none))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }

        if (GUILayout.Button(meshValueRoot.isRead ? "√" : "", lineStyle, MPGUIStyles.options_none))
        {
            if (SelectIndex != index)
            {
                isRetract = false;
            }
            SelectIndex = index;
            SelectChildIndex = -1;
        }
        GUILayout.EndHorizontal();
        if (element.isGroup && isRetract && isSelect)
        {
            GUILayout.BeginVertical(MPGUIStyles.boxStyle);
            for (int k = 0; k < element.childList.Count; k++)
            {
                DrawItemChild(element.childList[k], k);
            }
            GUILayout.EndVertical();
        }
    }

    /// <summary>
    /// 绘制子项条目框
    /// </summary>
    /// <param name="meshValue"></param>
    /// <param name="index"></param>

    void DrawItemChild(MeshValues meshValue, int index)
    {
        bool isSelect = SelectChildIndex == index;
        GUIStyle lineStyle = isSelect ? MPGUIStyles.itemBtnStyles_child[1] : MPGUIStyles.itemBtnStyles_child[0];
        GUILayout.BeginHorizontal();
        GUILayout.Label("|------", GUILayout.Width(32));
        if (GUILayout.Button(meshValue.mesh.name, lineStyle, GUILayout.Height(25), GUILayout.Width(180)))
        {
            SelectChildIndex = index;
        }

        GUILayoutOption[] btnOption = isSelect ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none;


        if (GUILayout.Button(meshValue.vertCount.ToString(), lineStyle, btnOption))
        {
            SelectChildIndex = index;
        }

        if (isSelect && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        {
            ShowDataList.AddWindow(meshValue.GetVerticsStr(), "Vertices" + "-" + meshValue.parentName + "=>" + meshValue.mesh.name);
        }

        if (GUILayout.Button("-", lineStyle, MPGUIStyles.options_child_none))
        {
            SelectChildIndex = index;
        }


        if (GUILayout.Button(meshValue.triangles.ToString(), lineStyle, btnOption))
        {
            SelectChildIndex = index;
        }
        if (isSelect && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        {
            ShowDataList.AddWindow(meshValue.GetTrianglesStr(), "Triangles" + "-" + meshValue.parentName + "=>" + meshValue.mesh.name);
        }


        if (GUILayout.Button(meshValue.exist_normals ? "√" : "", lineStyle, (isSelect && meshValue.exist_normals) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
        {
            SelectChildIndex = index;
        }
        if (isSelect && meshValue.exist_normals && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        {
            ShowDataList.AddWindow(meshValue.GetNormalsStr(), "Normals" + "-" + meshValue.mesh.name);
        }
        if (GUILayout.Button(meshValue.exist_tangents ? "√" : "", lineStyle, (isSelect && meshValue.exist_tangents) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
        {
            SelectChildIndex = index;
        }
        if (isSelect && meshValue.exist_tangents && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        {
            ShowDataList.AddWindow(meshValue.GetTangentsStr(), "Tangents" + "-" + meshValue.mesh.name);
        }
        if (GUILayout.Button(meshValue.exist_colors ? "√" : "", lineStyle, (isSelect && meshValue.exist_colors) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
        {
            SelectChildIndex = index;
        }
        if (isSelect && meshValue.exist_colors && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        {
            ShowDataList.AddWindow(meshValue.GetColorsStr(), "Colors" + "-" + meshValue.mesh.name);
        }

        for (int m = 0; m < 4; m++)
        {
            if (GUILayout.Button(meshValue.exist_uv[m] ? "√" : "", lineStyle, (isSelect && meshValue.exist_uv[m]) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
            {
                SelectChildIndex = index;
            }
            if (isSelect && meshValue.exist_uv[m] && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
            {
                ShowDataList.AddWindow(meshValue.GetUVStr(m), "UV" + (m + 1) + "-" + meshValue.mesh.name);
            }
        }
        string memoryStr = meshValue.memory >= 1000 ? (meshValue.memory / 1000.0f).ToString("F2") + "MB" : meshValue.memory.ToString("F2") + "KB";
        if (GUILayout.Button(memoryStr, lineStyle, MPGUIStyles.options_child_none))
        {
            SelectChildIndex = index;
        }

        if (GUILayout.Button(meshValue.isRead ? "√" : "", lineStyle, MPGUIStyles.options_child_none))
        {
            SelectChildIndex = index;
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 初始化预览物体
    /// </summary>
    /// <param name="obj"></param>
    void InitPreview(UnityEngine.Object obj)
    {
        if (previewEditor != null)
        {
            DestroyImmediate(previewEditor);
        }

        previewEditor = Editor.CreateEditor(obj);

    }
}

public class BuildingModelElement: ListItemElement<BuildingModelValues>
{
    public BuildingModelElement(GameObject _rootObj, bool _isAsset, bool _isSkin = false) : base(_rootObj, _isAsset, _isSkin)
    {

    }

    public override void RefleshProps()
    {
        base.RefleshProps();
    }
}

public class BuildingModelValues 
    : MeshValues
    //: ListItemElementValues
{
}
