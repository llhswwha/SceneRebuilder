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
using MeshProfilerNS;

namespace MeshProfilerNS
{
    public class TreeNodeManagerEditorWindow : ListManagerEditorWindow<TreeNodeElement, TreeNodeValues>
    //:EditorWindow
    {
        //[MenuItem("Window/Analysis/Mesh Profiler")]
        [MenuItem("Window/Tools/TreeNode Manager")]
        public static void ShowWindow()
        {
            TreeNodeManagerEditorWindow window = (TreeNodeManagerEditorWindow)EditorWindow.GetWindowWithRect(typeof(TreeNodeManagerEditorWindow), new Rect(0, 0, MPGUIStyles.SCREEN_WIDTH, MPGUIStyles.SCREEN_HEIGHT), true, "Mesh Profiler 1.1");
            window.Show();
            window.Init();
        }

        public static void ShowWindow(GameObject rootObj, string wndName)
        {
            GameObjectListMeshEditorWindow window = (GameObjectListMeshEditorWindow)EditorWindow.GetWindowWithRect(typeof(GameObjectListMeshEditorWindow), new Rect(0, 0, MPGUIStyles.SCREEN_WIDTH, MPGUIStyles.SCREEN_HEIGHT), true, $"GameObjectList | {wndName} ");
            window.Show();
            window.Init(rootObj);
        }

        string[] tableTitle = new string[] { "name", "Vertex", "ChildCount", "Triangles", "Normals", "Tangents", "Colors", "UV", "UV2", "UV3", "UV4", "Memory", "Readable", };
        // List<MeshElement> meshElementList = new List<MeshElement>();
        // List<MeshElement> originList = new List<MeshElement>();

        //Setting��������

        static int minVertNum = 0;
        static int maxVertNum = 65000;
        bool isSelectConditions = false;
        string[] tableConditions = new string[] { "Normals", "UV", "Tangents", "UV2", "Colors", "UV3", "Readable", "UV4" };
        static bool[] selectConditions = new bool[8];
        static TreeNodeSortWays sortWays;
        bool IsIgnoreKeywordsList = false;
        static string IgnoreKeywordList = "";
        static int[] LevelNum = new int[5] { 0, 500, 1000, 1500, 2000 };

        ////UI��������
        //int selectIndex = 0;//������ѡ���±�
        //int SelectIndex
        //{
        //    get { return selectIndex; }
        //    set
        //    {
        //        selectIndex = value;
        //        if (selectIndex >= 0)
        //        {
        //            PreviewSelectedItem(meshElementList[selectIndex]);
        //        }

        //    }
        //}

        protected override void PreviewSelectedItem(TreeNodeElement ele)
        {
            if (ele.isGroup)
            {
                InitPreview(ele.rootObj);
            }
            else
            {
                InitPreview(ele.rootMeshValue.mesh);
            }
        }

        //int selectChildIndex = 0;//������ѡ���±�
        //int SelectChildIndex
        //{
        //    get { return selectChildIndex; }
        //    set
        //    {
        //        selectChildIndex = value;
        //        if (selectChildIndex >= 0)
        //            InitPreview(meshElementList[SelectIndex].childList[selectChildIndex].mesh);
        //    }
        //}


        protected override void InitPreviewChild(TreeNodeValues vs)
        {
            InitPreview(vs.mesh);
        }


        MeshDataClass dataChart = new MeshDataClass();
        /// <summary>
        /// ˢ��ͳ��ͼ
        /// </summary>
        void RefleshBarChart()
        {
            if (!(LevelNum[0] < LevelNum[1] && LevelNum[1] < LevelNum[2] && LevelNum[2] < LevelNum[3] && LevelNum[3] < LevelNum[4]))
            {
                LevelNum = new int[5] { 0, 500, 1000, 1500, 2000 };
                Debug.LogError("Your parameter is wrong,bar chart parameter has been reset!");
            }
            int[] array = new int[5] { 0, 0, 0, 0, 0 };

            for (int i = 0; i < originList.Count; i++)
            {

                if (originList[i].rootMeshValue.vertCount < LevelNum[1] && originList[i].rootMeshValue.vertCount >= LevelNum[0])
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
                else if (originList[i].rootMeshValue.vertCount >= LevelNum[0])
                {
                    array[4]++;
                }
            }



            List<int> list = new List<int>();
            list.AddRange(array);



            DataLinePainter.Init(list);
        }
        /// <summary>
        /// ˢ������
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
                if (meshElementList[i].rootMeshValue.exist_uv[2] || meshElementList[i].rootMeshValue.exist_uv[3])
                {
                    dataChart.UVXCount++;
                }
                if (meshElementList[i].rootMeshValue.isRead)
                {
                    dataChart.readableCount++;
                }
            }

        }


        public GameObject rootObj;
        public void Init(GameObject rootObj)
        {
            this.rootObj = rootObj;
            Init();
        }

        /// <summary>
        /// ��ʼ��UI Style��ˢ���б�
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
        /// ˢ���б�����������
        /// </summary>
        void RefleshList()
        {
            meshElementList.Clear();

            var nodes = GameObject.FindObjectsOfType<AreaTreeNode>(true);
            foreach(var node in nodes)
            {
                TreeNodeElement ele = new TreeNodeElement(node);
                meshElementList.Add(ele);
            }
            // meshElementList.AddRange(MeshFinder.GetMeshElementList(MeshElementType.File));
            // meshElementList.AddRange(MeshFinder.GetMeshElementList(MeshElementType.Asset));
            Debug.Log($"MeshProfiler.RefleshList meshElementList:{meshElementList.Count} nodes:{nodes.Length}");
            Sort(meshElementList);
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

        /// <summary>
        /// ������ɸѡ
        /// </summary>
        /// <param name="list"></param>
        protected override void SelectByConditions(List<TreeNodeElement> list)
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
        protected override void Sort(List<TreeNodeElement> list)
        {
            for (int i = 1; i < list.Count; i++)
                for (int j = 0; j < list.Count - i; j++)
                {
                    var ele1 = list[j];
                    var ele2 = list[j + 1];
                    var values1 = ele1.rootMeshValue;
                    var values2 = ele2.rootMeshValue;
                    if (sortWays == TreeNodeSortWays.SortByName)
                    {
                        if (values1.name.CompareTo(values2.name) > 0)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByAllRenderer)
                    {
                        if (values1.AllRendererCount < values2.AllRendererCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByAllVertext)
                    {
                        if (values1.AllVertextCount < values2.AllVertextCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByRend_In)
                    {
                        if (values1.InRendererCount < values2.InRendererCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByRend_Out0)
                    {
                        if (values1.Out0RendererCount < values2.Out0RendererCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByRend_Out0S)
                    {
                        if (values1.Out0SmallRendererCount < values2.Out0SmallRendererCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByRend_Out0B)
                    {
                        if (values1.Out0BigRendererCount < values2.Out0BigRendererCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByRend_Out1)
                    {
                        if (values1.Out1RendererCount < values2.Out1RendererCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByVertex_In)
                    {
                        if (values1.InVertextCount < values2.InVertextCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByVertex_Out0)
                    {
                        if (values1.Out0VertextCount < values2.Out0VertextCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByVertex_Out0B)
                    {
                        if (values1.Out0BigVertextCount < values2.Out0BigVertextCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByVertex_Out0S)
                    {
                        if (values1.Out0SmallVertextCount < values2.Out0SmallVertextCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    else if (sortWays == TreeNodeSortWays.SortByVertex_Out1)
                    {
                        if (values1.Out1VertextCount < values2.Out1VertextCount)
                        {
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                    //else if (sortWays == TreeNodeSortWays.SortByParts)
                    //{
                    //    if (ele1.modelInfo.GetPartCount() < ele2.modelInfo.GetPartCount())
                    //    {
                    //        var temp = list[j];
                    //        list[j] = list[j + 1];
                    //        list[j + 1] = temp;
                    //    }
                    //}
                    //else if (sortWays == TreeNodeSortWays.SortByTrees)
                    //{
                    //    if (ele1.modelInfo.GetTreeCount() < ele2.modelInfo.GetTreeCount())
                    //    {
                    //        var temp = list[j];
                    //        list[j] = list[j + 1];
                    //        list[j + 1] = temp;
                    //    }
                    //}
                    //else if (sortWays == TreeNodeSortWays.SortByScenes)
                    //{
                    //    if (ele1.modelInfo.GetSceneCount() < ele2.modelInfo.GetSceneCount())
                    //    {
                    //        var temp = list[j];
                    //        list[j] = list[j + 1];
                    //        list[j + 1] = temp;
                    //    }
                    //}
                    //else if (sortWays == TreeNodeSortWays.SortByIsFinished)
                    //{
                    //    if (ele1.modelInfo.IsModelSceneFinish().CompareTo(ele2.modelInfo.IsModelSceneFinish()) < 0)
                    //    {
                    //        var temp = list[j];
                    //        list[j] = list[j + 1];
                    //        list[j + 1] = temp;
                    //    }
                    //}
                }
        }

        /// <summary>
        /// UI������
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
        /// �����������
        /// </summary>
        void DrawSettingBlock()
        {
            GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.SETTING_BLOCK), "");
            int border = 10;
            GUILayout.BeginArea(MPGUIStyles.BorderArea(new Rect(MPGUIStyles.SETTING_BLOCK.x + border, MPGUIStyles.SETTING_BLOCK.y + 5, MPGUIStyles.SETTING_BLOCK.width - 2 * border, MPGUIStyles.SETTING_BLOCK.height - border)));
            GUILayout.Label("Setting Panel", MPGUIStyles.centerStyle);
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            sortWays = (TreeNodeSortWays)EditorGUILayout.EnumPopup("Sort Ways", sortWays);
            if (EditorGUI.EndChangeCheck())
            {
                Sort(meshElementList);
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
        /// �����б����
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

        protected override int width1
        {
            get
            {
                return 80;
            }
        }
        protected override int width2
        {
            get
            {
                return 80;
            }
        }
        protected override int width3
        {
            get
            {
                return 80;
            }
        }

        /// <summary>
        /// ������Ŀ��
        /// </summary>
        /// <param name="element"></param>
        /// <param name="isSelect"></param>
        /// <param name="index"></param>
        protected override void DrawItem(TreeNodeElement element, bool isSelect, int index)
        {
            Debug.Log($"DrawItem[{index}][{isSelect}][{element.name}]");
            GUIStyle lineStyle = isSelect ? MPGUIStyles.itemBtnStyles[1] : MPGUIStyles.itemBtnStyles[0];
            GUILayout.BeginHorizontal();

            TreeNodeValues meshValueRoot = element.rootMeshValue;

            GUIContent icon_expand_Content = element.isGroup ? MPGUIStyles.icon_down_Content : MPGUIStyles.icon_right_Content;

            GUIStyle nameStyle = new GUIStyle(lineStyle);
            if (element.rootObj != null && element.rootObj.activeInHierarchy == false)
            {
                nameStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);
            }

            if (GUILayout.Button($"{index + 1:00}", nameStyle, GUILayout.Height(30), GUILayout.Width(25)))
            {
                if (SelectIndex != index)
                {
                    isRetract = false;
                }
                SelectIndex = index;
                SelectChildIndex = -1;
            }

            if (element.isGroup &&
                GUILayout.Button(isRetract && isSelect ? MPGUIStyles.icon_retract_Contents[1] : MPGUIStyles.icon_retract_Contents[0], isSelect ? MPGUIStyles.icon_tab_Style : MPGUIStyles.icon_tab_normal_Style, MPGUIStyles.options_icon))
            {
                SelectIndex = index;
                isRetract = !isRetract;
                SelectChildIndex = -1;
            }

            int widthOff = 32;

            if (GUILayout.Button(element.name, nameStyle, GUILayout.Height(30), element.isGroup ? GUILayout.Width(180 - widthOff) : GUILayout.Width(200 - widthOff)))
            {
                if (SelectIndex != index)
                {
                    isRetract = false;
                }
                SelectIndex = index;
                SelectChildIndex = -1;
            }

            if (GUILayout.Button($"S", nameStyle, GUILayout.Height(30), GUILayout.Width(25)))
            {
                if (SelectIndex != index)
                {
                    isRetract = false;
                }
                SelectIndex = index;
                SelectChildIndex = -1;

                Debug.Log($"SelectObject index:{index} ele:{element}");
                EditorHelper.SelectObject(element.rootObj);
            }

            //GUILayoutOption[] btnOption = isSelect ? MPGUIStyles.options_exist : MPGUIStyles.options_none;

            GUILayoutOption[] btnOption = MPGUIStyles.options_none;

            var values = meshValueRoot.GetValues();
            for (int i = 0; i < values.Length; i++)
            {
                object v = values[i];
                GUIStyle style = new GUIStyle(nameStyle);
                string vt = GetValue(v, style);

                if (i == values.Length - 1)
                {
                    btnOption = new GUILayoutOption[2] { GUILayout.Width(width3), GUILayout.Height(30) };
                }
                else if (i >= values.Length - 4)
                {
                    btnOption = new GUILayoutOption[2] { GUILayout.Width(width2), GUILayout.Height(30) };
                }
                else
                {
                    btnOption = new GUILayoutOption[2] { GUILayout.Width(width1), GUILayout.Height(30) };

                }

                if (i == 0)
                { //vertex

                    if (isSelect)
                    {
                        btnOption = new GUILayoutOption[2] { GUILayout.Width(width1 - 20), GUILayout.Height(30) };
                    }

                    // GUILayoutOption[] btnOption = isSelect ? MPGUIStyles.options_exist : MPGUIStyles.options_none;
                    if (GUILayout.Button(vt, style, btnOption))
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
                        //MeshFilter[] filters = element.rootObj.GetComponentsInChildren<MeshFilter>(true);
                        GameObjectListMeshEditorWindow.ShowWindow(element.rootObj, element.name);
                    }
                }
                else
                {

                    if (GUILayout.Button(vt, style, btnOption))
                    {
                        if (SelectIndex != index)
                        {
                            isRetract = false;
                        }
                        SelectIndex = index;
                        SelectChildIndex = -1;
                    }
                }
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

        ///// <summary>
        ///// ������Ŀ��
        ///// </summary>
        ///// <param name="element"></param>
        ///// <param name="isSelect"></param>
        ///// <param name="index"></param>
        //protected override void DrawItem(TreeNodeElement element, bool isSelect, int index)
        //{
        //    GUIStyle lineStyle = isSelect ? MPGUIStyles.itemBtnStyles[1] : MPGUIStyles.itemBtnStyles[0];
        //    GUILayout.BeginHorizontal();
        //    MeshValues meshValueRoot = element.rootMeshValue;

        //    GUIContent icon_expand_Content = element.isGroup ? MPGUIStyles.icon_down_Content : MPGUIStyles.icon_right_Content;

        //    if (GUILayout.Button(isRetract && isSelect ? MPGUIStyles.icon_retract_Contents[1] : MPGUIStyles.icon_retract_Contents[0], isSelect ? MPGUIStyles.icon_tab_Style : MPGUIStyles.icon_tab_normal_Style, MPGUIStyles.options_icon))
        //    {
        //        SelectIndex = index;
        //        isRetract = !isRetract;
        //        SelectChildIndex = -1;
        //    }
        //    GUIStyle nameStyle = new GUIStyle(lineStyle);
        //    nameStyle.alignment = TextAnchor.MiddleLeft;
        //    if (GUILayout.Button(element.name, nameStyle, GUILayout.Height(30), GUILayout.Width(200)))
        //    {
        //        if (SelectIndex != index)
        //        {
        //            isRetract = false;
        //        }
        //        SelectIndex = index;
        //        SelectChildIndex = -1;
        //    }

        //    GUILayoutOption[] btnOption = isSelect ? MPGUIStyles.options_exist : MPGUIStyles.options_none;


        //    //if (GUILayout.Button(meshValueRoot.vertCount.ToString(), lineStyle, btnOption))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}

        //    //if (isSelect && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    //{
        //    //    if (element.isGroup)
        //    //        isRetract = !isRetract;
        //    //    else
        //    //    {
        //    //        ShowDataList.AddWindow(meshValueRoot.GetVerticsStr(), "Vertices" + "-" + element.name);
        //    //    }
        //    //}

        //    //if (GUILayout.Button(element.childList.Count.ToString(), lineStyle, btnOption))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}

        //    //if (isSelect && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    //{
        //    //    ShowRefList.AddWindow(element.refList, "RefList" + "-" + element.name);
        //    //}

        //    //if (GUILayout.Button(element.rootMeshValue.triangles.ToString(), lineStyle, btnOption))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}
        //    //if (isSelect && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    //{
        //    //    if (element.isGroup)
        //    //        isRetract = !isRetract;
        //    //    else
        //    //    {
        //    //        ShowDataList.AddWindow(meshValueRoot.GetTrianglesStr(), "Triangles" + "-" + element.name);
        //    //    }
        //    //}

        //    //if (GUILayout.Button(meshValueRoot.exist_normals ? "��" : "", lineStyle, (isSelect && meshValueRoot.exist_normals) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}
        //    //if (isSelect && meshValueRoot.exist_normals && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    //{
        //    //    if (element.isGroup)
        //    //        isRetract = !isRetract;
        //    //    else
        //    //    {
        //    //        ShowDataList.AddWindow(meshValueRoot.GetNormalsStr(), "Normals" + "-" + element.name);
        //    //    }
        //    //}

        //    //if (GUILayout.Button(meshValueRoot.exist_tangents ? "��" : "", lineStyle, (isSelect && meshValueRoot.exist_tangents) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}
        //    //if (isSelect && meshValueRoot.exist_tangents && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    //{
        //    //    if (element.isGroup)
        //    //        isRetract = !isRetract;
        //    //    else
        //    //    {
        //    //        ShowDataList.AddWindow(meshValueRoot.GetTangentsStr(), "Tangents" + "-" + element.name);
        //    //    }
        //    //}
        //    //if (GUILayout.Button(meshValueRoot.exist_colors ? "��" : "", lineStyle, (isSelect && meshValueRoot.exist_colors) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}
        //    //if (isSelect && meshValueRoot.exist_colors && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    //{
        //    //    if (element.isGroup)
        //    //        isRetract = !isRetract;
        //    //    else
        //    //    {
        //    //        ShowDataList.AddWindow(meshValueRoot.GetColorsStr(), "Colors" + "-" + element.name);
        //    //    }
        //    //}

        //    //for (int m = 0; m < 4; m++)
        //    //{
        //    //    if (GUILayout.Button(meshValueRoot.exist_uv[m] ? "��" : "", lineStyle, (isSelect && meshValueRoot.exist_uv[m]) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //    //    {
        //    //        if (SelectIndex != index)
        //    //        {
        //    //            isRetract = false;
        //    //        }
        //    //        SelectIndex = index;
        //    //        SelectChildIndex = -1;
        //    //    }
        //    //    if (isSelect && meshValueRoot.exist_uv[m] && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    //    {
        //    //        if (element.isGroup)
        //    //            isRetract = !isRetract;
        //    //        else
        //    //        {
        //    //            ShowDataList.AddWindow(meshValueRoot.GetUVStr(m), "UV" + (m + 1) + "-" + element.name);
        //    //        }
        //    //    }
        //    //}
        //    //string memoryStr = meshValueRoot.memory >= 1000 ? (meshValueRoot.memory / 1000.0f).ToString("F2") + "MB" : meshValueRoot.memory.ToString("F2") + "KB";


        //    //if (GUILayout.Button(memoryStr, lineStyle, MPGUIStyles.options_none))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}

        //    //if (GUILayout.Button(meshValueRoot.isRead ? "��" : "", lineStyle, MPGUIStyles.options_none))
        //    //{
        //    //    if (SelectIndex != index)
        //    //    {
        //    //        isRetract = false;
        //    //    }
        //    //    SelectIndex = index;
        //    //    SelectChildIndex = -1;
        //    //}

        //    GUILayout.EndHorizontal();
        //    if (element.isGroup && isRetract && isSelect)
        //    {
        //        GUILayout.BeginVertical(MPGUIStyles.boxStyle);
        //        for (int k = 0; k < element.childList.Count; k++)
        //        {
        //            DrawItemChild(element.childList[k], k);
        //        }
        //        GUILayout.EndVertical();
        //    }
        //}

        /// <summary>
        /// ����������Ŀ��
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


            if (GUILayout.Button(meshValue.exist_normals ? "��" : "", lineStyle, (isSelect && meshValue.exist_normals) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
            {
                SelectChildIndex = index;
            }
            if (isSelect && meshValue.exist_normals && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
            {
                ShowDataList.AddWindow(meshValue.GetNormalsStr(), "Normals" + "-" + meshValue.mesh.name);
            }
            if (GUILayout.Button(meshValue.exist_tangents ? "��" : "", lineStyle, (isSelect && meshValue.exist_tangents) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
            {
                SelectChildIndex = index;
            }
            if (isSelect && meshValue.exist_tangents && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
            {
                ShowDataList.AddWindow(meshValue.GetTangentsStr(), "Tangents" + "-" + meshValue.mesh.name);
            }
            if (GUILayout.Button(meshValue.exist_colors ? "��" : "", lineStyle, (isSelect && meshValue.exist_colors) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
            {
                SelectChildIndex = index;
            }
            if (isSelect && meshValue.exist_colors && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
            {
                ShowDataList.AddWindow(meshValue.GetColorsStr(), "Colors" + "-" + meshValue.mesh.name);
            }

            for (int m = 0; m < 4; m++)
            {
                if (GUILayout.Button(meshValue.exist_uv[m] ? "��" : "", lineStyle, (isSelect && meshValue.exist_uv[m]) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
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

            if (GUILayout.Button(meshValue.isRead ? "��" : "", lineStyle, MPGUIStyles.options_child_none))
            {
                SelectChildIndex = index;
            }
            GUILayout.EndHorizontal();
        }



        #region TOOL_BLOCK
        private bool ExportExcelData()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            string exportPath = EditorUtility.SaveFilePanel("Save SkuInfo File", "", "MeshData-" + sceneName + ".xlsx", "xlsx");
            if (string.IsNullOrEmpty(exportPath))
            {
                return false;
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
                    worksheet.Cells[i + 4, 5].Value = originList[i].rootMeshValue.exist_normals ? "��" : "";
                    worksheet.Cells[i + 4, 6].Value = originList[i].rootMeshValue.exist_tangents ? "��" : "";
                    worksheet.Cells[i + 4, 7].Value = originList[i].rootMeshValue.exist_colors ? "��" : "";
                    worksheet.Cells[i + 4, 8].Value = originList[i].rootMeshValue.exist_uv[0] ? "��" : "";
                    worksheet.Cells[i + 4, 9].Value = originList[i].rootMeshValue.exist_uv[1] ? "��" : "";
                    worksheet.Cells[i + 4, 10].Value = originList[i].rootMeshValue.exist_uv[2] ? "��" : "";
                    worksheet.Cells[i + 4, 11].Value = originList[i].rootMeshValue.exist_uv[3] ? "��" : "";
                    worksheet.Cells[i + 4, 12].Value = originList[i].rootMeshValue.memory >= 1000 ? (originList[i].rootMeshValue.memory / 1000.0f).ToString("F2") + "MB" : originList[i].rootMeshValue.memory.ToString("F2") + "KB";
                    worksheet.Cells[i + 4, 13].Value = originList[i].rootMeshValue.isRead ? "��" : "";
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

            return true;
        }

        private void OpenMeshFolder()
        {
            string meshPath = meshElementList[SelectIndex].assetPath;
            if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources"))
            {
                string path = meshPath.Substring(0, meshPath.LastIndexOf('/'));
                DirectoryInfo direction = new DirectoryInfo(path);
                path = direction.GetFiles("*", SearchOption.TopDirectoryOnly)[0].FullName.ToString();
                path = (path.Substring(0, path.IndexOf("Assets")) + meshPath).Replace('/', '\\'); ;
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "explorer.exe";
                p.StartInfo.Arguments = (@" /select," + path);
                p.Start();
            }
            else
            {
                EditorUtility.DisplayDialog("warning", "This mesh is not asset file��", "ok");
            }
        }

        private void SelectAsset()
        {
            var ele = meshElementList[SelectIndex];
            string meshPath = ele.assetPath;

            if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources"))
            {
                UnityEngine.Object obj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(meshPath, typeof(UnityEngine.Object));
                Selection.activeObject = obj;
            }
            else
            {
                //EditorHelper.SelectObject(ele.rootObj);
                //EditorUtility.DisplayDialog("warning", "This mesh is not asset file��", "ok");
                if (selectChildIndex >= 0)
                {
                    EditorHelper.SelectObject(ele.childList[selectChildIndex].obj);
                }
                else
                {
                    EditorHelper.SelectObject(ele.rootObj);
                }

            }
        }

        private void ShowLostMeshObjs()
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

        #endregion

        /// <summary>
        /// ���ƹ������
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
                    if (ExportExcelData() == false) return;
                }

#if UNITY_EDITOR_WIN
                if (GUILayout.Button("Open Mesh Folder", GUILayout.Height(buttonHeight)))
                {
                    if (SelectIndex < 0)
                        return;
                    OpenMeshFolder();
                }
#endif

                if (GUILayout.Button("Select Asset", GUILayout.Height(buttonHeight)))
                {
                    if (SelectIndex < 0)
                        return;
                    SelectAsset();
                }

                if (GUILayout.Button("Show Lost Mesh Objs", GUILayout.Height(buttonHeight)))
                {
                    ShowLostMeshObjs();
                }


                if (GUILayout.Button(meshElementList[SelectIndex].rootMeshValue.exist_normals ? "Shutdown the Normals" : "Open the Normals", GUILayout.Height(buttonHeight)))
                {

                    string meshPath = meshElementList[SelectIndex].assetPath;
                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources") && !meshPath.EndsWith(".asset"))
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
                        EditorUtility.DisplayDialog("warning", "This mesh is not model file��", "ok");
                    }
                }

                if (GUILayout.Button(meshElementList[SelectIndex].rootMeshValue.exist_tangents ? "Shutdown the Tangents" : "Open the Tangents", GUILayout.Height(buttonHeight)))
                {
                    string meshPath = meshElementList[SelectIndex].assetPath;
                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources") && !meshPath.EndsWith(".asset"))
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
                        EditorUtility.DisplayDialog("warning", "This mesh is not model file��", "ok");
                    }
                }
                if (GUILayout.Button(meshElementList[SelectIndex].rootMeshValue.isRead ? "Shutdown the Readable" : "Open the Readable", GUILayout.Height(buttonHeight)))
                {

                    string meshPath = meshElementList[SelectIndex].assetPath;
                    if (!string.IsNullOrEmpty(meshPath) && !meshPath.Contains("unity default resources") && !meshPath.EndsWith(".asset"))
                    {
                        ModelImporter importer = ModelImporter.GetAtPath(meshPath) as ModelImporter;
                        importer.isReadable = meshElementList[SelectIndex].rootMeshValue.isRead ? false : true;
                        importer.SaveAndReimport();
                        meshElementList[SelectIndex].RefleshProps();
                        RefleshDataChart();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("warning", "This mesh is not model file��", "ok");
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
        /// ����������
        /// </summary>
        void DrawInputTextField()
        {
            GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.SEARCH_BLOCK));

            GUILayout.BeginHorizontal();

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

            // if (GUI.Button(position, GUIContent.none, gUIStyle) && m_InputSearchText != "")
            // {
            //     m_InputSearchText = "";
            //     GUI.changed = true;
            //     GUIUtility.keyboardControl = 0;
            //     RefleshSearchList();
            // }

            eleType = (MeshElementType)EditorGUILayout.EnumPopup("ElementType", eleType, GUILayout.Width(250));

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        public MeshFilter[] meshFilters;
        public MeshElementType eleType = MeshElementType.GameObject;
        /// <summary>
        /// ����ͼ��
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
            GUILayout.Label("High>=", GUILayout.Width(70));
            LevelNum[4] = EditorGUILayout.IntField(LevelNum[4]);
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
        /// ��������
        /// </summary>
        void DrawDataBlock()
        {
            GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.DATA_BLOCK), "");
            int modelCount = dataChart.modelCount;
            if (modelCount == 0)
            {
                modelCount = 1;
            }
            GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.DATA_BLOCK, 5));
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
            GUILayout.Label(dataChart.assetCount.ToString() + " [" + (int)((float)dataChart.assetCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
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
            GUILayout.Label(dataChart.UVXCount.ToString() + " [" + (int)((float)dataChart.UVXCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
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
public class TreeNodeElement : ListItemElement<TreeNodeValues>
{
    public AreaTreeNode modelInfo;
    public TreeNodeElement(GameObject _rootObj, bool _isAsset, bool _isSkin = false) : base(_rootObj, _isAsset, _isSkin)
    {

    }
    public TreeNodeElement(AreaTreeNode info)
    {
        if (info != null)
        {
            this.name = info.name;
            modelInfo = info;
            rootObj = info.gameObject;
        }


        isAsset = false;
        isSkin = false;
        refList = new List<GameObject>();

        RefleshProps();
    }

    public override void RefleshProps()
    {
        base.RefleshProps();
        if (modelInfo != null)
        {
            rootMeshValue = new TreeNodeValues(modelInfo);
            rootMeshValue.name = modelInfo.name;
        }
        else
        {
            rootMeshValue = new TreeNodeValues();
        }


    }
}

public class TreeNodeValues
    : MeshValues
//: ListItemElementValues
{
    public string name;

    public AreaTreeNode info;

    public GameObject partGo;

    public float AllVertextCount = 0;
    public int AllRendererCount = 0;
    public int CombinedRendererCount = 0;
    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;

    public float Out0BigVertextCount = 0;
    public float Out0SmallVertextCount = 0;
    public int Out0BigRendererCount = 0;
    public int Out0SmallRendererCount = 0;


    public TreeNodeValues()
    {
        name = "";
    }

    public TreeNodeValues(AreaTreeNode modelInfo)
    {
        SetValues(modelInfo);
    }

    public void SetValues(AreaTreeNode modelInfo)
    {
        //this.name = modelInfo.name;

        this.info = modelInfo;
        //this.vertCount = this.AllVertextCount;
        this.AllVertextCount = modelInfo.GetVertexCount();
        this.AllRendererCount = modelInfo.RendererCount;
        this.CombinedRendererCount = modelInfo.CombinedRenderers.Length;

        //this.InVertextCount = modelInfo.InVertextCount;
        //this.Out0VertextCount = modelInfo.Out0VertextCount;
        //this.Out1VertextCount = modelInfo.Out1VertextCount;
        //this.InRendererCount = modelInfo.InRendererCount;
        //this.Out0RendererCount = modelInfo.Out0RendererCount;
        //this.Out1RendererCount = modelInfo.Out1RendererCount;

        //this.Out0BigVertextCount = modelInfo.Out0BigVertextCount;
        //this.Out0SmallVertextCount = modelInfo.Out0SmallVertextCount;
        //this.Out0BigRendererCount = modelInfo.Out0BigRendererCount;
        //this.Out0SmallRendererCount = modelInfo.Out0SmallRendererCount;
    }

    internal void Add(TreeNodeValues modelInfo)
    {
        this.AllVertextCount += modelInfo.AllVertextCount;
        this.AllRendererCount += modelInfo.AllRendererCount;
        this.InVertextCount += modelInfo.InVertextCount;
        this.Out0VertextCount += modelInfo.Out0VertextCount;
        this.Out1VertextCount += modelInfo.Out1VertextCount;
        this.InRendererCount += modelInfo.InRendererCount;
        this.Out0RendererCount += modelInfo.Out0RendererCount;
        this.Out1RendererCount += modelInfo.Out1RendererCount;

        this.Out0BigVertextCount += modelInfo.Out0BigVertextCount;
        this.Out0SmallVertextCount += modelInfo.Out0SmallVertextCount;
        this.Out0BigRendererCount += modelInfo.Out0BigRendererCount;
        this.Out0SmallRendererCount += modelInfo.Out0SmallRendererCount;
    }

    public override object[] GetValues()
    {
        //return new object[] { AllVertextCount,AllRendererCount,
        //    InVertextCount,GetOut0VertexInfo(),Out1VertextCount,
        //InRendererCount,GetOut0RenderInfo(),Out1RendererCount,
        //info.GetPartCount(),info.GetTreeCount(),info.GetSceneCount(),
        //info.IsModelSceneFinish() };

        //if (info != null)
        //{
        //    return new object[] { AllVertextCount,AllRendererCount,
        //InVertextCount,     Out1VertextCount,   Out0VertextCount,   Out0BigVertextCount,   Out0SmallVertextCount,
        //InRendererCount,    Out1RendererCount,  Out0RendererCount,  Out0BigRendererCount,  Out0SmallRendererCount,
        //info.GetPartCount(),info.GetTreeCount(),info.GetSceneCount(),        info.IsModelSceneFinish() };
        //}
        //else
        //{
        //    return new object[] { AllVertextCount,AllRendererCount,
        //InVertextCount,     Out1VertextCount,   Out0VertextCount,   Out0BigVertextCount,   Out0SmallVertextCount,
        //InRendererCount,    Out1RendererCount,  Out0RendererCount,  Out0BigRendererCount,  Out0SmallRendererCount,
        //"-","-","-","-" };
        //}

        return new object[] { AllVertextCount,AllRendererCount,
        InVertextCount,     Out1VertextCount,   Out0VertextCount,   Out0BigVertextCount,   Out0SmallVertextCount,
        InRendererCount,    Out1RendererCount,  Out0RendererCount,  Out0BigRendererCount,  Out0SmallRendererCount,
        "-","-","-","-" };

    }

    //public string GetOut0VertexInfo()
    //{
    //    return $"{Out0VertextCount:F0}={info.Out0SmallVertextCount:F0}+{info.Out0BigVertextCount:F0}";
    //}

    //public string GetOut0RenderInfo()
    //{
    //    return $"{Out0RendererCount:F0}={info.Out0SmallRendererCount:F0}+{info.Out0BigRendererCount:F0}";
    //}


}

enum TreeNodeSortWays
{
    SortByName,
    SortByAllVertext,
    SortByAllRenderer,
    SortByVertex_In,
    SortByVertex_Out1,
    SortByVertex_Out0,
    SortByVertex_Out0B,
    SortByVertex_Out0S,

    SortByRend_In,
    SortByRend_Out1,
    SortByRend_Out0,
    SortByRend_Out0B,
    SortByRend_Out0S,

    SortByParts,
    SortByTrees,
    SortByScenes,
    SortByIsFinished
}



