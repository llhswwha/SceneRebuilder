using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text;
using OfficeOpenXml;


namespace MeshProfilerNS
{
    public class MeshProfiler : EditorWindow
    {
        [MenuItem("Window/Analysis/Mesh Profiler")]
        public static void AddWindow()
        {
            MeshProfiler window = (MeshProfiler)EditorWindow.GetWindowWithRect(typeof(MeshProfiler), new Rect(0, 0, MPGUIStyles.SCREEN_WIDTH, MPGUIStyles.SCREEN_HEIGHT), true, "Mesh Profiler 1.1");
            window.Show();
            window.Init();
        }
        enum SortWays
        {
            SortByVertNum,
            SortByRefNum,
            SortByVertsMultiplyRefCount,
            SortByTriangles,
            SortByName,
            SortByMemory
        }

        string[] tableTitle = new string[] { "name", "Vertex","RefCount", "Triangles", "Normals", "Tangents", "Colors", "UV", "UV2", "UV3", "UV4", "Memory", "Readable", };
        List<MeshElement> meshElementList = new List<MeshElement>();
        List<MeshElement> originList = new List<MeshElement>();

        //Setting辅助参数
        Editor previewEditor;
        static int minVertNum = 0;
        static int maxVertNum = 65000;
         bool isSelectConditions = false;
        string[] tableConditions = new string[] { "Normals", "UV", "Tangents", "UV2", "Colors", "UV3", "Readable", "UV4" };
        static bool[] selectConditions = new bool[8];
        static SortWays sortWays;
        bool IsIgnoreKeywordsList = false;
        static string IgnoreKeywordList = "";
        static int[] LevelNum = new int[5] { 0,500,1000,1500,2000};

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

        string m_InputSearchText;

        Vector2 scVector = new Vector2(0, 0);
        bool isRetract = false;//是否折叠
        const int pageCount = 100;
        int pageIndex = 0;


        class DataClass
        {
            public int modelCount = 0;
            public int assetCount = 0;
            public int normalCount = 0;
            public int tangentCount = 0;
            public int colorCount = 0;
            public int UVXCount = 0;
            public int readableCount = 0;
            public void Reset()
            {
                modelCount = 0;
                assetCount = 0;
                normalCount = 0;
                tangentCount = 0;
                colorCount = 0;
                UVXCount = 0;
                readableCount = 0;
            }
        }

        DataClass dataChart = new DataClass();
        /// <summary>
        /// 刷新统计图
        /// </summary>
        void RefleshBarChart()
        {
            if (!(LevelNum[0] < LevelNum[1] && LevelNum[1] < LevelNum[2] && LevelNum[2] < LevelNum[3] && LevelNum[3] < LevelNum[4]))
            {
                LevelNum = new int[5] { 0, 500, 1000, 1500, 2000 };
                Debug.LogError("Your parameter is wrong,bar chart parameter has been reset!");
            }
            int[] array = new int[5] { 0,0,0,0,0};

            for (int i = 0; i <originList.Count; i++)
            {

                if (originList[i].rootMeshValue.vertCount < LevelNum[1]&& originList[i].rootMeshValue.vertCount>= LevelNum[0])
                {
                    array[0]++;
                }
                else if (originList[i].rootMeshValue.vertCount < LevelNum[2] && originList[i].rootMeshValue.vertCount >= LevelNum[1])
                {
                    array[1]++;
                }
                else if (originList[i].rootMeshValue.vertCount < LevelNum[3] && originList[i].rootMeshValue.vertCount >= LevelNum[2])
                {
                    array[2]++;
                }
                else if (originList[i].rootMeshValue.vertCount < LevelNum[4] && originList[i].rootMeshValue.vertCount >= LevelNum[3])
                {
                    array[3]++;
                }
                else if ( originList[i].rootMeshValue.vertCount >= LevelNum[0])
                {
                    array[4]++;
                }
            }

 

            List<int> list = new List<int>();
            list.AddRange(array);



            DataLinePainter.Init(list);
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefleshDataChart()
        {
            dataChart.Reset();
            dataChart.modelCount = meshElementList.Count;
            for (int i = 0; i < meshElementList.Count; i++)
            {
                if (meshElementList[i].isAsset)
                {
                    dataChart.assetCount++;
                }
                if (meshElementList[i].rootMeshValue.exist_normals)
                {
                    dataChart.normalCount++;
                }
                if (meshElementList[i].rootMeshValue.exist_tangents)
                {
                    dataChart.tangentCount++;
                }
                if (meshElementList[i].rootMeshValue.exist_colors)
                {
                    dataChart.colorCount++;
                }
                if (meshElementList[i].rootMeshValue.exist_uv[2]|| meshElementList[i].rootMeshValue.exist_uv[3])
                {
                    dataChart.UVXCount++;
                }
                if (meshElementList[i].rootMeshValue.isRead)
                {
                    dataChart.readableCount++;
                }
            }
            
        }

        /// <summary>
        /// 初始化UI Style和刷新列表
        /// </summary>
        public void Init()
        {
            MPGUIStyles.InitGUIStyles();
            RefleshList();
            RefleshDataChart();
            RefleshBarChart();
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
            meshElementList.AddRange(MeshFinder.GetMeshElementList());
            Sort(meshElementList, sortWays);
            SelectByConditions(meshElementList);
            originList.Clear();
            originList.AddRange(meshElementList);
            if (meshElementList.Count > 0)
                SelectIndex = 0;
            else
                SelectIndex = -1;
            pageIndex = 0;
            scVector = Vector2.zero;
            

        }
        List<MeshElement> tempList = new List<MeshElement>();
        /// <summary>
        /// 按照搜索结果刷新列表，不重新获取。
        /// </summary>
        void RefleshSearchList()
        {
            tempList.Clear();
            for (int i = 0; i < originList.Count; i++)
            {
                if (originList[i].name.Contains(m_InputSearchText))
                {
                    tempList.Add(originList[i]);
                }
            }
            meshElementList.Clear();
            meshElementList.AddRange(tempList);

            SelectIndex = -1;
            pageIndex = 0;
        }
        /// <summary>
        /// 按条件筛选
        /// </summary>
        /// <param name="list"></param>
        void SelectByConditions(List<MeshElement> list)
        {
            if (isSelectConditions)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {

                    if (list[i].rootMeshValue.vertCount < minVertNum || list[i].rootMeshValue.vertCount > maxVertNum)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[0] && !list[i].rootMeshValue.exist_normals)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[1] && !list[i].rootMeshValue.exist_uv[0])
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[2] && !list[i].rootMeshValue.exist_tangents)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[3] && !list[i].rootMeshValue.exist_uv[1])
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[4] && !list[i].rootMeshValue.exist_colors)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[5] && !list[i].rootMeshValue.exist_uv[2])
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[6] && !list[i].rootMeshValue.isRead)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                    if (selectConditions[7] && !list[i].rootMeshValue.exist_uv[3])
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }
            }
            if (IsIgnoreKeywordsList)
            {
                string[] keywords = IgnoreKeywordList.Split(',', ' ');
                if (keywords[0].Length != 0)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        for (int m = 0; m < keywords.Length; m++)
                        {
                            if (list[i].name.Contains(keywords[m]))
                            {
                                list.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
            if (list.Count == 0)
            {
                SelectIndex = -1;
            }
        }
        //按排序方式排序
        void Sort(List<MeshElement> list, SortWays sortways)
        {
            for (int i = 1; i < list.Count; i++)
                for (int j = 0; j < list.Count - i; j++)
                {
                    if (sortways == SortWays.SortByVertNum)
                    {
                        if (list[j].rootMeshValue.vertCount < list[j + 1].rootMeshValue.vertCount)
                        {
                            MeshElement temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortways == SortWays.SortByMemory)
                    {
                        if (list[j].rootMeshValue.memory < list[j + 1].rootMeshValue.memory)
                        {
                            MeshElement temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortways == SortWays.SortByTriangles)
                    {
                        if (list[j].rootMeshValue.triangles < list[j + 1].rootMeshValue.triangles)
                        {
                            MeshElement temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortways == SortWays.SortByVertsMultiplyRefCount)
                    {
                        if (list[j].AllVertsNum < list[j + 1].AllVertsNum)
                        {
                            MeshElement temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortways == SortWays.SortByRefNum)
                    {
                        if (list[j].refList.Count < list[j + 1].refList.Count)
                        {
                            MeshElement temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortways == SortWays.SortByName)
                    {
                        if (list[j].name.CompareTo(list[j + 1].name)>0)
                        {
                            MeshElement temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                }
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
            DrawSettingBlock();
            DrawPreviewBlock();
            DrawPageIndexBlock();
            DrawListBlock();
            DrawInputTextField();
            DrawToolBlock();
            DrawChartBlock();
            DrawDataBlock();
            

        }

        /// <summary>
        /// 绘制设置面板
        /// </summary>
        void DrawSettingBlock()
        {
            GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.SETTING_BLOCK), "");
            int border = 10;
            GUILayout.BeginArea(MPGUIStyles.BorderArea(new Rect(MPGUIStyles.SETTING_BLOCK.x + border, MPGUIStyles.SETTING_BLOCK.y+5, MPGUIStyles.SETTING_BLOCK.width - 2 * border, MPGUIStyles.SETTING_BLOCK.height -  border)));
            GUILayout.Label("Setting Panel", MPGUIStyles.centerStyle);
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            sortWays = (SortWays)EditorGUILayout.EnumPopup("Sort Ways", sortWays);
            if (EditorGUI.EndChangeCheck())
            {
                Sort(meshElementList, sortWays);
            }
            //GUILayout.Space(5);
            isSelectConditions = EditorGUILayout.BeginToggleGroup("Select Conditions", isSelectConditions);
            GUILayout.BeginHorizontal();
            GUILayout.Label("vertexs count:");
            minVertNum = EditorGUILayout.IntField(minVertNum);
            GUILayout.Label("-");
            maxVertNum = EditorGUILayout.IntField(maxVertNum);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            for (int i = 0; i < selectConditions.Length; i += 2)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(tableConditions[i], GUILayout.Width(100));
                selectConditions[i] = EditorGUILayout.Toggle(selectConditions[i]);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(tableConditions[i + 1], GUILayout.Width(100));
                selectConditions[i + 1] = EditorGUILayout.Toggle(selectConditions[i + 1]);
                GUILayout.EndHorizontal();

                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();
            IsIgnoreKeywordsList = EditorGUILayout.BeginToggleGroup("IgnoreKeywords(','or ' ')", IsIgnoreKeywordsList);
            IgnoreKeywordList = EditorGUILayout.TextField("Keywords", IgnoreKeywordList);
            EditorGUILayout.EndToggleGroup();
            GUILayout.Space(10);
            if (GUILayout.Button("Reflesh List", GUILayout.Height(29)))
            {
                m_InputSearchText = "";
                GUIUtility.keyboardControl = 0;
                RefleshList();
                RefleshDataChart();
                RefleshBarChart();
            }
            GUILayout.EndArea();

        }

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
        void DrawItem(MeshElement element, bool isSelect, int index)
        {
            GUIStyle lineStyle = isSelect ? MPGUIStyles.itemBtnStyles[1] : MPGUIStyles.itemBtnStyles[0];
            GUILayout.BeginHorizontal();
            MeshValues meshValueRoot = element.rootMeshValue;

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
        /// 绘制页标
        /// </summary>
        void DrawPageIndexBlock()
        {
            int count = meshElementList.Count;
            int pages = count / pageCount;
            if (count % pageCount != 0)
                pages++;

            Rect ActualRect = MPGUIStyles.PAGEINDEX_BLOCK;
            ActualRect.x = MPGUIStyles.PAGEINDEX_BLOCK.x + MPGUIStyles.PAGEINDEX_BLOCK.width - 160 - 35 * pages;
            GUILayout.BeginArea(ActualRect);
            GUILayout.BeginHorizontal();
            if (meshElementList.Count != 0)
            {
                if (pageIndex == 0)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Last Page", MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(70)))
                {
                    pageIndex--;
                    scVector = Vector2.zero;
                }
                GUI.enabled = true;

                for (int i = 0; i < pages; i++)
                {
                    bool isGoal = i == pageIndex;
                    string str = "[" + i + "]";
                    if (isGoal)
                    {
                        if (GUILayout.Button(str, MPGUIStyles.itemBtnStyles_child[1], GUILayout.MaxWidth(35)))
                        {
                            pageIndex = i;
                            scVector = Vector2.zero;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(str, MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(35)))
                        {
                            pageIndex = i;
                            scVector = Vector2.zero;
                        }
                    }
                }
                if (pageIndex == (pages - 1))
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Next Page", MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(70)))
                {
                    pageIndex++;
                    scVector = Vector2.zero;
                }
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
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
        /// <summary>
        /// 绘制预览窗口
        /// </summary>
        void DrawPreviewBlock()
        {
            GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.PREVIEW_BLOCK), "");
            

            if (meshElementList.Count > 0 && previewEditor != null)
            {
                previewEditor.DrawPreview(MPGUIStyles.PREVIEW_BLOCK_CENTER);
            }

        }
        /// <summary>
        /// 绘制工具面板
        /// </summary>
        void DrawToolBlock()
        {
            GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.TOOL_BLOCK), "");
            int border = 10;
            GUILayout.BeginArea(MPGUIStyles.BorderArea(new Rect(MPGUIStyles.TOOL_BLOCK.x + border, MPGUIStyles.TOOL_BLOCK.y + 5, MPGUIStyles.TOOL_BLOCK.width - 2 * border, MPGUIStyles.TOOL_BLOCK.height - 2 * border)));
            GUILayout.Label("Tool Panel", MPGUIStyles.centerStyle);
            //GUILayout.Space(15);
            int buttonHeight = 29;
            if (SelectIndex >= 0)
            {
                if (GUILayout.Button("Export Excel Data", GUILayout.Height(buttonHeight)))
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    string exportPath = EditorUtility.SaveFilePanel("Save SkuInfo File", "", "MeshData-"+sceneName+".xlsx", "xlsx");
                    if (string.IsNullOrEmpty(exportPath))
                    {
                        return;
                    }
                    if (File.Exists(exportPath))
                    {
                        File.Delete(exportPath);
                    }

                    using (ExcelPackage package = new ExcelPackage(new FileInfo(exportPath)))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                        worksheet.Column(1).Width = 30;
                        for (int i = 2; i <= 13; i++)
                        {
                            worksheet.Column(i).Width = 13;
                            worksheet.Column(i).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }

                        string ConStr = "SortWays:" + sortWays.ToString() + "  Select Conditions:";
                        if (!isSelectConditions)
                        {
                            ConStr += "None";
                        }
                        else
                        {
                            ConStr += "Vertex Count:" + minVertNum + "-" + maxVertNum;
                            for (int k = 0; k < tableConditions.Length; k++)
                            {
                                if (selectConditions[k] == true)
                                {
                                    ConStr += "," + tableConditions[k];
                                }
                            }
                        }
                        ConStr += "\r\n   IgnoreKeywords:";
                        if (!IsIgnoreKeywordsList)
                        {
                            ConStr += "\nNone";
                        }
                        else
                        {
                            ConStr += IgnoreKeywordList;
                        }

                        worksheet.Cells["A1:M1"].Merge = true;
                        worksheet.Cells["A2:M2"].Merge = true;
                        worksheet.Cells[1, 1].Value = "Mesh Profiler Data";
                        worksheet.Cells[2, 1].Value = ConStr;
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        worksheet.Cells[2, 1].Style.Font.Bold = true;
                        worksheet.Cells[1, 1].Style.Font.Size = 15;
                        for (int i = 0; i < tableTitle.Length; i++)
                        {
                            worksheet.Cells[3, i + 1].Value = tableTitle[i];
                            worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                            worksheet.Cells[3, i + 1].Style.Font.Size = 12;
                            //worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 0)) ;
                        }
                        for (int i = 0; i < originList.Count; i++)
                        {
                            worksheet.Cells[i + 4, 1].Value = originList[i].name;
                            worksheet.Cells[i + 4, 2].Value = originList[i].rootMeshValue.vertCount;
                            worksheet.Cells[i + 4, 3].Value = originList[i].refList.Count;
                            worksheet.Cells[i + 4, 4].Value = originList[i].rootMeshValue.triangles;
                            worksheet.Cells[i + 4, 5].Value = originList[i].rootMeshValue.exist_normals ? "√" : "";
                            worksheet.Cells[i + 4, 6].Value = originList[i].rootMeshValue.exist_tangents ? "√" : "";
                            worksheet.Cells[i + 4, 7].Value = originList[i].rootMeshValue.exist_colors ? "√" : "";
                            worksheet.Cells[i + 4, 8].Value = originList[i].rootMeshValue.exist_uv[0] ? "√" : "";
                            worksheet.Cells[i + 4, 9].Value = originList[i].rootMeshValue.exist_uv[1] ? "√" : "";
                            worksheet.Cells[i + 4, 10].Value = originList[i].rootMeshValue.exist_uv[2] ? "√" : "";
                            worksheet.Cells[i + 4, 11].Value = originList[i].rootMeshValue.exist_uv[3] ? "√" : "";
                            worksheet.Cells[i + 4, 12].Value = originList[i].rootMeshValue.memory >= 1000 ? (originList[i].rootMeshValue.memory / 1000.0f).ToString("F2") + "MB" : originList[i].rootMeshValue.memory.ToString("F2") + "KB";
                            worksheet.Cells[i + 4, 13].Value = originList[i].rootMeshValue.isRead ? "√" : "";
                        }
                        package.Save();
                        exportPath = exportPath.Replace('/', '\\');
#if UNITY_EDITOR_WIN
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = "explorer.exe";
                        p.StartInfo.Arguments = (@" /select," + exportPath);
                        p.Start();
#endif
                    }
                }

#if UNITY_EDITOR_WIN
                if (GUILayout.Button("Open Mesh Folder", GUILayout.Height(buttonHeight)))
                {
                    if (SelectIndex < 0)
                        return;

                    string meshPath = meshElementList[SelectIndex].assetPath;
                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources"))
                    {
                        string path = meshPath.Substring(0, meshPath.LastIndexOf('/'));
                        DirectoryInfo direction = new DirectoryInfo(path);
                        path = direction.GetFiles("*", SearchOption.TopDirectoryOnly)[0].FullName.ToString();
                        path = (path.Substring(0, path.IndexOf("Assets"))+ meshPath).Replace('/', '\\'); ;
                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo.FileName = "explorer.exe";
                        p.StartInfo.Arguments = (@" /select," + path);
                        p.Start();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("warning", "This mesh is not asset file！", "ok");
                    }
                }
#endif

                if (GUILayout.Button("Select Asset", GUILayout.Height(buttonHeight)))
                {
                    if (SelectIndex < 0)
                        return;

                    string meshPath = meshElementList[SelectIndex].assetPath;

                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources"))
                    {
                            UnityEngine.Object obj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(meshPath, typeof(UnityEngine.Object));
                            Selection.activeObject = obj;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("warning", "This mesh is not asset file！", "ok");
                    }
                }

                if (GUILayout.Button("Show Lost Mesh Objs", GUILayout.Height(buttonHeight)))
                {

                    MeshFilter[] filters = FindObjectsOfType<MeshFilter>();
                    List<GameObject> objList = new List<GameObject>();
                    
                    for (int i = filters.Length - 1; i >= 0; i--)
                    {
                        if (filters[i].sharedMesh == null)
                        {
                            objList.Add(filters[i].gameObject);
                        }
                    }
                    ShowRefList.AddWindow(objList, "Lost Mesh List");
                }


                if (GUILayout.Button(meshElementList[SelectIndex].rootMeshValue.exist_normals ? "Shutdown the Normals" : "Open the Normals", GUILayout.Height(buttonHeight)))
                {

                    string meshPath = meshElementList[SelectIndex].assetPath;
                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources")&& !meshPath.EndsWith(".asset"))
                    {
                        ModelImporter importer = ModelImporter.GetAtPath(meshPath) as ModelImporter;
                        importer.importTangents = ModelImporterTangents.None;
                        importer.importNormals = meshElementList[SelectIndex].rootMeshValue.exist_normals ? ModelImporterNormals.None : ModelImporterNormals.Import;
                        importer.SaveAndReimport();
                        meshElementList[SelectIndex].RefleshProps();
                        RefleshDataChart();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("warning", "This mesh is not model file！", "ok");
                    }
                }

                if (GUILayout.Button(meshElementList[SelectIndex].rootMeshValue.exist_tangents ? "Shutdown the Tangents" : "Open the Tangents", GUILayout.Height(buttonHeight)))
                {
                    string meshPath = meshElementList[SelectIndex].assetPath;
                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources")&&!meshPath.EndsWith(".asset"))
                    {
                        if (!meshElementList[SelectIndex].rootMeshValue.exist_normals)
                        {
                            EditorUtility.DisplayDialog("warning", "Must open the Normals first!", "ok");
                            return;
                        }
                        ModelImporter importer = ModelImporter.GetAtPath(meshPath) as ModelImporter;
                        importer.importTangents = meshElementList[SelectIndex].rootMeshValue.exist_tangents ? ModelImporterTangents.None : ModelImporterTangents.CalculateMikk;
                        importer.SaveAndReimport();
                        meshElementList[SelectIndex].RefleshProps();
                        RefleshDataChart();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("warning", "This mesh is not model file！", "ok");
                    }
                }
                if (GUILayout.Button(meshElementList[SelectIndex].rootMeshValue.isRead ? "Shutdown the Readable" : "Open the Readable", GUILayout.Height(buttonHeight)))
                {

                    string meshPath = meshElementList[SelectIndex].assetPath;
                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources")&& !meshPath.EndsWith(".asset"))
                    {
                        ModelImporter importer = ModelImporter.GetAtPath(meshPath) as ModelImporter;
                        importer.isReadable = meshElementList[SelectIndex].rootMeshValue.isRead ? false : true;
                        importer.SaveAndReimport();
                        meshElementList[SelectIndex].RefleshProps();
                        RefleshDataChart();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("warning", "This mesh is not model file！", "ok");
                    }
                }
            }
            if (GUILayout.Button("About", GUILayout.Height(buttonHeight)))
            {
                EditorUtility.DisplayDialog("About", "Mesh Profiler 1.1 is a tool for optimizing mesh performance, which makes it easier for developers to optimize the models in scene.\nIf you find the bug, Please send it and example demo to unseenstone@outlook.com", "ok");
            }
            GUILayout.EndArea();

        }

        /// <summary>
        /// 绘制搜索栏
        /// </summary>
        void DrawInputTextField()
        {
            GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.SEARCH_BLOCK));
            Rect position = EditorGUILayout.GetControlRect();
            GUIStyle textFieldRoundEdge = MPGUIStyles.TextFieldRoundEdge;
            GUIStyle transparentTextField = MPGUIStyles.TransparentTextField;
            GUIStyle gUIStyle = (m_InputSearchText != "") ? MPGUIStyles.TextFieldRoundEdgeCancelButton : MPGUIStyles.TextFieldRoundEdgeCancelButtonEmpty;
            position.width -= gUIStyle.fixedWidth;
            if (Event.current.type == EventType.Repaint)
            {
                GUI.contentColor = (EditorGUIUtility.isProSkin ? Color.black : new Color(0f, 0f, 0f, 0.5f));
                if (string.IsNullOrEmpty(m_InputSearchText))
                {
                    textFieldRoundEdge.Draw(position, new GUIContent(""), 0);
                }
                else
                {
                    textFieldRoundEdge.Draw(position, new GUIContent(""), 0);
                }
                GUI.contentColor = Color.white;
            }
            Rect rect = position;
            float num = textFieldRoundEdge.CalcSize(new GUIContent("")).x - 2f;
            rect.width -= num;
            rect.x += num;
            rect.y += 1f;
            EditorGUI.BeginChangeCheck();
            m_InputSearchText = EditorGUI.TextField(rect, m_InputSearchText, transparentTextField);
            if (EditorGUI.EndChangeCheck())
            {
                RefleshSearchList();
            }
            position.x += position.width;
            position.width = gUIStyle.fixedWidth;
            if (GUI.Button(position, GUIContent.none, gUIStyle) && m_InputSearchText != "")
            {
                m_InputSearchText = "";
                GUI.changed = true;
                GUIUtility.keyboardControl = 0;
                RefleshSearchList();
            }
            GUILayout.EndArea();
        }
        /// <summary>
        /// 绘制图表
        /// </summary>
        void DrawChartBlock()
        {
            GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.CHART_BLOCK), "");
            GUILayout.BeginArea(MPGUIStyles.CHART_BLOCK);
            GUILayout.Space(5);
            GUILayout.Label("Bar Chart Panel", MPGUIStyles.centerStyle);
            GUILayout.EndArea();
            DataLinePainter.Draw();
            GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.CHART_PARAS_BLOCK, 10));
            GUILayout.BeginHorizontal();
            GUILayout.Label("High>=",GUILayout.Width(70));
            LevelNum[4]= EditorGUILayout.IntField(LevelNum[4]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mid High>=", GUILayout.Width(70));
            LevelNum[3] = EditorGUILayout.IntField(LevelNum[3]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mid>=", GUILayout.Width(70));
            LevelNum[2] = EditorGUILayout.IntField(LevelNum[2]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mid Low>=", GUILayout.Width(70));
            LevelNum[1] = EditorGUILayout.IntField(LevelNum[1]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Low>=", GUILayout.Width(70));
            GUI.enabled = false;
            LevelNum[0] = EditorGUILayout.IntField(0);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("Reflesh"))
            {
                RefleshBarChart();
            }
            GUILayout.EndArea();

        }
        /// <summary>
        /// 绘制数据
        /// </summary>
        void DrawDataBlock()
        {
            GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.DATA_BLOCK), "");
            int modelCount = dataChart.modelCount;
            if (modelCount == 0)
            {
                modelCount = 1;
            }
            GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.DATA_BLOCK,5));
            GUILayout.Label("Data Panel", MPGUIStyles.centerStyle);
           // GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Model Count", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.Space(5);
            GUILayout.Label(dataChart.modelCount.ToString(), MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Asset Count", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.Space(5);
            GUILayout.Label(dataChart.assetCount.ToString()+" ["+ (int)((float)dataChart.assetCount/modelCount * 100) +"%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Normals", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.Space(5);
            GUILayout.Label(dataChart.normalCount.ToString() + " [" + (int)((float)dataChart.normalCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tangents", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.Space(5);
            GUILayout.Label(dataChart.tangentCount.ToString() + " [" + (int)((float)dataChart.tangentCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Colors", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.Space(5);
            GUILayout.Label(dataChart.colorCount.ToString() + " [" + (int)((float)dataChart.colorCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("UV3,UV4", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.Space(5);
            GUILayout.Label(dataChart.UVXCount.ToString() + " [" + (int)((float)dataChart.UVXCount/ modelCount*100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Readable", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.Space(5);
            GUILayout.Label(dataChart.readableCount.ToString() + " [" + (int)((float)dataChart.readableCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
            GUILayout.EndHorizontal();

      

            GUILayout.EndArea();


        }
    }
}

