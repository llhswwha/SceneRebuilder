using MeshProfilerNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneManagerEditorWindow : ListManagerEditorWindow<SubSceneElement,SubSceneValues>
{
    //[MenuItem("Window/Tools/SceneRebuild")]
    [MenuItem("Window/Tools/SubSceneManager")]
    public static void ShowWindow()
    {
        SubSceneManagerEditorWindow window = (SubSceneManagerEditorWindow)EditorWindow.GetWindowWithRect(typeof(SubSceneManagerEditorWindow), new Rect(0, 0, MPGUIStyles.SCREEN_WIDTH, MPGUIStyles.SCREEN_HEIGHT), true, "BuildingModelEditorWindow");
        window.Show();
        window.Init();
    }

    // int selectIndex = 0;
    // int SelectIndex
    // {
    //     get { return selectIndex; }
    //     set
    //     {
    //         selectIndex = value;
    //         if (selectIndex >= 0)
    //         {
    //             if (meshElementList[selectIndex].isGroup)
    //             {
    //                 InitPreview(meshElementList[selectIndex].rootObj);
    //             }
    //             else
    //             {
    //                 InitPreview(meshElementList[selectIndex].rootMeshValue.mesh);
    //             }
    //         }

    //     }
    // }

    // int selectChildIndex = 0;
    // int SelectChildIndex
    // {
    //     get { return selectChildIndex; }
    //     set
    //     {
    //         selectChildIndex = value;
    //         if (selectChildIndex >= 0)
    //             InitPreview(meshElementList[SelectIndex].childList[selectChildIndex].mesh);
    //     }
    // }

    string[] tableTitle = new string[] { "Name", "ParentName","Vertex", "Renderer", "SceneType", "ContentType", "IsLoaded","--", "--" };

        // return new object[] { AllVertextCount,AllRendererCount,
        // SceneType,     ContentType,   ParentName,   IsLoaded,   Out0SmallVertextCount,
        // InRendererCount,    Out1RendererCount,  Out0RendererCount,  Out0BigRendererCount,  Out0SmallRendererCount,
        // "-","-","-","-" };

    static int minVertNum = 0;
    static int maxVertNum = 65000;
    bool isSelectConditions = false;
    string[] tableConditions = new string[] { "NoFinished", "NoneParts", "NoneTrees", "NoneScenes", "OnePart", "--", "--", "--" };
    static bool[] selectConditions = new bool[8];
    static SubSceneSortWays sortWays=SubSceneSortWays.SortByAllVertext;
    bool IsIgnoreKeywordsList = false;
    static string IgnoreKeywordList = "";
    static int[] LevelNum = new int[7] { 0, 2, 10, 50, 100,200,400 };//new int[5] { 0, 500, 1000, 1500, 2000 };

    // string m_InputSearchText;

    float max;
    float min;
    float avg;

    float sum;

    SubSceneDataClass dataChart = new SubSceneDataClass();

    void RefleshBarChart()
    {
        if (!(LevelNum[0] < LevelNum[1] && LevelNum[1] < LevelNum[2] && LevelNum[2] < LevelNum[3] && LevelNum[3] < LevelNum[4]))
        {
            LevelNum = new int[7] { 0, 5, 10, 50, 100,200,400 };
            Debug.LogError("Your parameter is wrong,bar chart parameter has been reset!");
        }
        int[] array = new int[7] { 0, 0, 0, 0, 0,0,0 };

        max = 0;
        min = float.MaxValue;
        avg = 0;
        sum = 0;
        for (int i = 0; i < originList.Count; i++)
        {
            if (originList[i].name.Contains("??????")) continue;
            var vc = originList[i].rootMeshValue.AllVertextCount;
            if (vc < LevelNum[1] && vc >= LevelNum[0])
            {
                array[0]++;
            }
            else if (vc < LevelNum[2] && vc >= LevelNum[1])
            {
                array[1]++;
            }
            else if (vc < LevelNum[3] && vc >= LevelNum[2])
            {
                array[2]++;
            }
            else if (vc < LevelNum[4] && vc >= LevelNum[3])
            {
                array[3]++;
            }
            else if (vc < LevelNum[5] && vc >= LevelNum[4])
            {
                array[4]++;
            }
            else if (vc < LevelNum[6] && vc >= LevelNum[5])
            {
                array[5]++;
            }
            else if (vc >= LevelNum[6])
            {
                array[6]++;
            }
            sum += vc;
            if(vc>max){
                max=vc;
            }
            if(vc<min){
                min=vc;
            }
        }
        avg=sum/originList.Count;

        List<int> list = new List<int>();
        list.AddRange(array);

        DataLinePainter.Init(list);
    }

    void RefleshDataChart()
    {
        dataChart.Reset();
        //dataChart.modelCount = meshElementList.Count;
        for (int i = 0; i < meshElementList.Count; i++)
        {
            if(meshElementList[i]==null)continue;
            if(string.IsNullOrEmpty(meshElementList[i].name))continue;
            if (meshElementList[i].name.Contains("??????")) continue;
            var values = meshElementList[i].rootMeshValue;
            dataChart.AllRendererCount += values.AllRendererCount;
            dataChart.AllVertextCount += values.AllVertextCount;

            dataChart.InVertextCount += values.InVertextCount;
            dataChart.Out0VertextCount += values.Out0VertextCount;
            dataChart.Out1VertextCount += values.Out1VertextCount;
            dataChart.InRendererCount += values.InRendererCount;
            dataChart.Out0RendererCount += values.Out0RendererCount;
            dataChart.Out1RendererCount += values.Out1RendererCount;
        }

    }

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

    public SubScene_Base[] scenes_All;
    public SubScene_Out0[] scenes_Out0;
    public SubScene_Out1[] scenes_Out1;
    public SubScene_In[] scenes_In;

    public List<SubScene_In> scenes_In_Part = new List<SubScene_In>();
    public List<SubScene_In> scenes_In_Tree = new List<SubScene_In>();

    public List<SubScene_Out0> scenes_Out0_Part = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_Tree = new List<SubScene_Out0>();
    //public List<SubScene_Out0> scenes_Out0_TreeNode = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Hidden = new List<SubScene_Out0>();
    public List<SubScene_Out0> scenes_Out0_TreeNode_Shown = new List<SubScene_Out0>();

    public void InitSubScenes()
    {
        //sceneManager = GameObject.FindObjectOfType<SubSceneManager>(true);
        scenes_All= GameObject.FindObjectsOfType<SubScene_Base>(true);
        scenes_In = GameObject.FindObjectsOfType<SubScene_In>(true);
        scenes_Out0 = GameObject.FindObjectsOfType<SubScene_Out0>(true);
        scenes_Out1 = GameObject.FindObjectsOfType<SubScene_Out1>(true);

        foreach(var s in scenes_Out0)
        {
            //s.gameObject.SetActive(true);
            if(s.contentType==SceneContentType.Part)
            {
                scenes_Out0_Part.Add(s);
                s.HideBoundsBox();

            }
            if (s.contentType == SceneContentType.Tree)
            {
                scenes_Out0_Tree.Add(s);
            }
            //if (s.contentType == SceneContentType.TreeNode)
            //{
            //    scenes_Out0_TreeNode.Add(s);
            //}
        }

        foreach(var s in scenes_In)
        {
            //s.gameObject.SetActive(true);
            if(s.contentType==SceneContentType.Part)
            {
                scenes_In_Part.Add(s);
                s.HideBoundsBox();

            }
            if (s.contentType == SceneContentType.Tree)
            {
                scenes_In_Tree.Add(s);
            }
            //if (s.contentType == SceneContentType.TreeNode)
            //{
            //    scenes_Out0_TreeNode.Add(s);
            //}
        }




        //cameras = GameObject.FindObjectsOfType<Camera>();

        List<ModelAreaTree> HiddenTrees = new List<ModelAreaTree>();
        List<ModelAreaTree> ShownTrees = new List<ModelAreaTree>();
        var ts = GameObject.FindObjectsOfType<ModelAreaTree>(true);

        foreach (ModelAreaTree t in ts)
        {
            if (t.GetIsHidden() && !HiddenTrees.Contains(t))
            {
                HiddenTrees.Add(t);
                //HiddenTreesVertexCount += t.VertexCount;
                var scenes = t.GetComponentsInChildren<SubScene_Out0>(true);
                scenes_Out0_TreeNode_Hidden.AddRange(scenes);
            }
            else if (t.GetIsHidden() == false && !ShownTrees.Contains(t))
            {
                ShownTrees.Add(t);
                //ShownTreesVertexCount += t.VertexCount;

                var scenes = t.GetComponentsInChildren<SubScene_Out0>(true);
                scenes_Out0_TreeNode_Shown.AddRange(scenes);
            }
        }
    }

    void RefleshList()
    {
        meshElementList.Clear();

        //meshElementList.AddRange(MeshFinder.GetMeshElementList());

        InitSubScenes();
        
        //var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);

        SubSceneElement sumEle = new SubSceneElement(null);
        sumEle.rootMeshValue.SceneName = "??????_??????";
        sumEle.rootMeshValue.ParentName = "??????_??????";
        meshElementList.Add(sumEle);

        foreach (var b in scenes_All)
        {
            if (b == null) continue;
            SubSceneElement ele = new SubSceneElement(b);
            meshElementList.Add(ele);
            sumEle.rootMeshValue.Add(ele.rootMeshValue);

            // if (b.InPart)
            // {
            //     BuildingModelValues eleValues = new BuildingModelValues();
            //     eleValues.name = "InPart";
            //     eleValues.partGo = b.InPart;
            //     eleValues.AllVertextCount = b.InVertextCount;
            //     eleValues.AllRendererCount = b.InRendererCount;
            //     ele.childList.Add(eleValues);
            // }
            // if (b.OutPart0)
            // {
            //     BuildingModelValues eleValues = new BuildingModelValues();
            //     eleValues.name = "OutPart0";
            //     eleValues.partGo = b.OutPart0;
            //     eleValues.AllVertextCount = b.Out0VertextCount;
            //     eleValues.AllRendererCount = b.Out0RendererCount;
            //     ele.childList.Add(eleValues);
            // }
            // if (b.OutPart1)
            // {
            //     BuildingModelValues eleValues = new BuildingModelValues();
            //     eleValues.name = "OutPart1";
            //     eleValues.partGo = b.OutPart1;
            //     eleValues.AllVertextCount = b.Out1VertextCount;
            //     eleValues.AllRendererCount = b.Out1RendererCount;
            //     ele.childList.Add(eleValues);
            // }

            // if(b.InPart!=null || b.OutPart0!=null || b.OutPart1 != null)
            // {
            //     ele.isGroup = true;
            // }
        }

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

        setPageIndexSetting=true;
    }

    protected override void SelectByConditions(List<SubSceneElement> list)
    {
        if (isSelectConditions)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var modelInfo = list[i].modelInfo;
                var values = list[i].rootMeshValue;
                if(sortWays==SubSceneSortWays.SortByAllRenderer || sortWays== SubSceneSortWays.SortByName)
                {
                    if (values.AllRendererCount < minVertNum || values.AllRendererCount > maxVertNum)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }
                else if (sortWays == SubSceneSortWays.SortByAllVertext)
                {
                    if (values.AllVertextCount < minVertNum || values.AllVertextCount > maxVertNum)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }
                // else if (sortWays == SubSceneSortWays.SortByVertex_In)
                // {
                //     if (values.InVertextCount < minVertNum || values.InVertextCount > maxVertNum)
                //     {
                //         list.RemoveAt(i);
                //         continue;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByVertex_Out0)
                // {
                //     if (values.Out0VertextCount < minVertNum || values.Out0VertextCount > maxVertNum)
                //     {
                //         list.RemoveAt(i);
                //         continue;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByVertex_Out1)
                // {
                //     if (values.Out1VertextCount < minVertNum || values.Out1VertextCount > maxVertNum)
                //     {
                //         list.RemoveAt(i);
                //         continue;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByRend_In)
                // {
                //     if (values.InRendererCount < minVertNum || values.InRendererCount > maxVertNum)
                //     {
                //         list.RemoveAt(i);
                //         continue;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByRend_Out0)
                // {
                //     if (values.Out0RendererCount < minVertNum || values.Out0RendererCount > maxVertNum)
                //     {
                //         list.RemoveAt(i);
                //         continue;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByRend_Out1)
                // {
                //     if (values.Out1RendererCount < minVertNum || values.Out1RendererCount > maxVertNum)
                //     {
                //         list.RemoveAt(i);
                //         continue;
                //     }
                // }

                // if (selectConditions[0] && modelInfo.IsModelSceneFinish()) //NoFinished { "NoFinished", "NoneParts", "NoneTrees", "NoneScenes", "OnePart", "--", "--", "--" };
                // {
                //     list.RemoveAt(i);
                //     continue;
                // }
                // if (selectConditions[1] && modelInfo.GetPartCount() > 0)//NoneParts
                // {
                //     list.RemoveAt(i);
                //     continue;
                // }
                // if (selectConditions[2] && modelInfo.GetTreeCount() > 0)//NoneTrees
                // {
                //     list.RemoveAt(i);
                //     continue;
                // }
                // if (selectConditions[3] && modelInfo.GetSceneCount() > 0)//NoneScenes
                // {
                //     list.RemoveAt(i);
                //     continue;
                // }
                // if (selectConditions[4] && modelInfo.GetPartCount() == 1)//OnePart
                // {
                //     list.RemoveAt(i);
                //     continue;
                // }

                //if (selectConditions[5] && !values.exist_uv[2])
                //{
                //    list.RemoveAt(i);
                //    continue;
                //}
                //if (selectConditions[6] && !values.isRead)
                //{
                //    list.RemoveAt(i);
                //    continue;
                //}
                //if (selectConditions[7] && !values.exist_uv[3])
                //{
                //    list.RemoveAt(i);
                //    continue;
                //}
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

    protected override void Sort(List<SubSceneElement> list)
    {
        for (int i = 1; i < list.Count; i++)
            for (int j = 0; j < list.Count - i; j++)
            {
                var ele1 = list[j];
                var ele2 = list[j + 1];
                var values1 = ele1.rootMeshValue;
                var values2 = ele2.rootMeshValue;
                if (sortWays == SubSceneSortWays.SortByName)
                {
                    if (values1.SceneName.CompareTo(values2.SceneName)>0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
                else if (sortWays == SubSceneSortWays.SortByParent)
                {
                    if (values1.ParentName.CompareTo(values2.ParentName)>0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
                else if (sortWays == SubSceneSortWays.SortByAllVertext)
                {
                    if (values1.AllVertextCount.CompareTo(values2.AllVertextCount)<0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
                else if (sortWays == SubSceneSortWays.SortByAllRenderer)
                {
                    if (values1.AllRendererCount.CompareTo(values2.AllRendererCount)<0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
                else if (sortWays == SubSceneSortWays.SortBySceneType)
                {
                     if (values1.SceneType.CompareTo(values2.SceneType)>0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
                else if (sortWays == SubSceneSortWays.SortByContentType)
                {
                    if (values1.ContentType.CompareTo(values2.ContentType)>0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
                else if (sortWays == SubSceneSortWays.SortByIsLoaded)
                {
                    if (values1.IsLoaded.CompareTo(values2.IsLoaded)>0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
                // else if (sortWays == SubSceneSortWays.SortByRend_In)
                // {
                //     if (values1.InRendererCount < values2.InRendererCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByRend_Out0)
                // {
                //     if (values1.Out0RendererCount < values2.Out0RendererCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByRend_Out0S)
                // {
                //     if (values1.Out0SmallRendererCount < values2.Out0SmallRendererCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByRend_Out0B)
                // {
                //     if (values1.Out0BigRendererCount < values2.Out0BigRendererCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByRend_Out1)
                // {
                //     if (values1.Out1RendererCount < values2.Out1RendererCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByVertex_In)
                // {
                //     if (values1.InVertextCount < values2.InVertextCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByVertex_Out0)
                // {
                //     if (values1.Out0VertextCount < values2.Out0VertextCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByVertex_Out0B)
                // {
                //     if (values1.Out0BigVertextCount < values2.Out0BigVertextCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByVertex_Out0S)
                // {
                //     if (values1.Out0SmallVertextCount < values2.Out0SmallVertextCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == SubSceneSortWays.SortByVertex_Out1)
                // {
                //     if (values1.Out1VertextCount < values2.Out1VertextCount)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == BuildingSortWays.SortByParts)
                // {
                //     if (ele1.modelInfo.GetPartCount() < ele2.modelInfo.GetPartCount())
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == BuildingSortWays.SortByTrees)
                // {
                //     if (ele1.modelInfo.GetTreeCount() < ele2.modelInfo.GetTreeCount())
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == BuildingSortWays.SortByScenes)
                // {
                //     if (ele1.modelInfo.GetSceneCount() < ele2.modelInfo.GetSceneCount())
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
                // else if (sortWays == BuildingSortWays.SortByIsFinished)
                // {
                //     if (ele1.modelInfo.IsModelSceneFinish().CompareTo(ele2.modelInfo.IsModelSceneFinish())<0)
                //     {
                //         var temp = list[j];
                //         list[j] = list[j + 1];
                //         list[j + 1] = temp;
                //     }
                // }
            }
    }


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

    void DrawSettingBlock()
    {
        GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.SETTING_BLOCK), "");
        int border = 10;
        GUILayout.BeginArea(MPGUIStyles.BorderArea(new Rect(MPGUIStyles.SETTING_BLOCK.x + border, MPGUIStyles.SETTING_BLOCK.y + 5, MPGUIStyles.SETTING_BLOCK.width - 2 * border, MPGUIStyles.SETTING_BLOCK.height - border)));
        GUILayout.Label("Setting Panel", MPGUIStyles.centerStyle);
        GUILayout.Space(5);
        EditorGUI.BeginChangeCheck();
        sortWays = (SubSceneSortWays)EditorGUILayout.EnumPopup("Sort Ways", sortWays);
        if (EditorGUI.EndChangeCheck())
        {
            Sort(meshElementList);
            RefleshList();
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
            //GUILayout.Label($"{i}", GUILayout.Width(100));
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

    void DrawChartBlock()
    {
        GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.CHART_BLOCK), "");
        GUILayout.BeginArea(MPGUIStyles.CHART_BLOCK);
        GUILayout.Space(5);
        // GUILayout.Label("Bar Chart Panel", MPGUIStyles.centerStyle);
        GUILayout.Label($"[Chart Panel] Min={min:F1},Max={max:F1},Avg={avg:F1}", MPGUIStyles.centerStyle);

        // GUILayout.BeginHorizontal();
        // GUILayout.Label("High>=", GUILayout.Width(70));
        // LevelNum[4] = EditorGUILayout.IntField(LevelNum[4]);
        // GUILayout.EndHorizontal();

        GUILayout.EndArea();

        DataLinePainter.Draw();

        GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.CHART_PARAS_BLOCK, 10));

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Min", GUILayout.Width(70));
        //GUILayout.Label(min.ToString("F1"));
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Max", GUILayout.Width(70));
        //GUILayout.Label(max.ToString("F1"));
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Avg", GUILayout.Width(70));
        //GUILayout.Label(avg.ToString("F1"));
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Very High=", GUILayout.Width(70));
        LevelNum[6] = EditorGUILayout.IntField(LevelNum[6]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("High=", GUILayout.Width(70));
        LevelNum[5] = EditorGUILayout.IntField(LevelNum[5]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mid High=", GUILayout.Width(70));
        LevelNum[4] = EditorGUILayout.IntField(LevelNum[4]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mid=", GUILayout.Width(70));
        LevelNum[3] = EditorGUILayout.IntField(LevelNum[3]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mid Low=", GUILayout.Width(70));
        LevelNum[2] = EditorGUILayout.IntField(LevelNum[2]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Low=", GUILayout.Width(70));
        LevelNum[1] = EditorGUILayout.IntField(LevelNum[1]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Very Low=", GUILayout.Width(70));
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

    // List<SubSceneElement> tempList = new List<SubSceneElement>();

    // void RefleshSearchList()
    // {
    //     tempList.Clear();
    //     for (int i = 0; i < originList.Count; i++)
    //     {
    //         if (originList[i].name.Contains(m_InputSearchText))
    //         {
    //             tempList.Add(originList[i]);
    //         }
    //     }
    //     meshElementList.Clear();
    //     meshElementList.AddRange(tempList);

    //     SelectIndex = -1;
    //     pageIndex = 0;
    // }

    // List<BuildingModelInfo> GetBuildingModelInfos()
    // {
    //     List<BuildingModelInfo> list=new List<BuildingModelInfo>();
    //     foreach(var ele in meshElementList)
    //     {
    //         list.Add(ele.modelInfo);
    //     }
    //     return list;
    // }

    void DrawToolBlock()
    {
        Rect toolBlockAreaLeft = MPGUIStyles.TOOL_BLOCK_LEFT;
        GUI.Box(MPGUIStyles.BorderArea(toolBlockAreaLeft), "");
        int border = 10;
        GUILayout.BeginArea(MPGUIStyles.BorderArea(new Rect(toolBlockAreaLeft.x + border, toolBlockAreaLeft.y + 5, toolBlockAreaLeft.width - 2 * border, toolBlockAreaLeft.height - 2 * border)));
        GUILayout.Label("ToolPanel(Item)", MPGUIStyles.centerStyle);
        //GUILayout.Space(15);
        int buttonHeight = 26;

        //if (GUILayout.Button("SelectObject", GUILayout.Height(buttonHeight)))
        //{
        //    if (SelectIndex < 0) return;
        //    var ele = meshElementList[SelectIndex];
        //    EditorHelper.SelectObject(ele.rootObj);
        //    Debug.Log($"SelectObject index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        //}

        // if (GUILayout.Button("InitInOut", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.InitInOut();
        //     Debug.Log($"InitInOut index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }
        // if (GUILayout.Button("GetTrees", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.GetTrees();
        //     Debug.Log($"GetTrees index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }
        // if (GUILayout.Button("ClearTrees", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.ClearTrees();
        //     ele.modelInfo.ShowRenderers();
        //     Debug.Log($"ClearTrees index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }
        // if (GUILayout.Button("CreateTrees", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.CreateTreesBSEx();
        //     Debug.Log($"CreateTrees index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }
        // if (GUILayout.Button("SaveScenes", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.EditorCreateScenes_TreeWithPart();
        //     Debug.Log($"SaveScenes index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }
        // if (GUILayout.Button("LoadScenes", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.EditorLoadScenes_TreeWithPart();
        //     Debug.Log($"LoadScenes index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }
        // if (GUILayout.Button("UnLoadScenes", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.UnLoadScenes();
        //     Debug.Log($"UnLoadScenes index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }
        // if (GUILayout.Button("OneKey", GUILayout.Height(buttonHeight)))
        // {
        //     if (SelectIndex < 0) return;
        //     var ele = meshElementList[SelectIndex];
        //     ele.modelInfo.OneKey_TreePartScene();
        //     Debug.Log($"OneKey index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        // }

        GUILayout.EndArea();

        Rect toolBlockAreaRight = MPGUIStyles.TOOL_BLOCK_RIGHT;
        GUI.Box(MPGUIStyles.BorderArea(toolBlockAreaRight), "");
        GUILayout.BeginArea(MPGUIStyles.BorderArea(new Rect(toolBlockAreaRight.x + border, toolBlockAreaRight.y + 5, toolBlockAreaRight.width - 2 * border, toolBlockAreaRight.height - 2 * border)));
        GUILayout.Label("ToolPanel(Selection)", MPGUIStyles.centerStyle);
        //GUILayout.Space(15);

        //if (GUILayout.Button("SelectObject", GUILayout.Height(buttonHeight)))
        //{
        //    if (SelectIndex < 0) return;
        //    var ele = meshElementList[SelectIndex];
        //    EditorHelper.SelectObject(ele.rootObj);
        //    Debug.Log($"SelectObject index:{SelectIndex} ele:{ele} obj:{ele.rootObj}");
        //}

        // if (GUILayout.Button("InitBuildings", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     BuildingModelManager buildingModelManager = GameObject.FindObjectOfType<BuildingModelManager>();
        //     buildingModelManager.InitBuildings(list);
        // }
        // if (GUILayout.Button("GetTrees", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     BuildingModelManager buildingModelManager = GameObject.FindObjectOfType<BuildingModelManager>();
        //     buildingModelManager.GetTrees(list);
        // }
        // if (GUILayout.Button("ClearTrees", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     BuildingModelManager buildingModelManager = GameObject.FindObjectOfType<BuildingModelManager>();
        //     buildingModelManager.ClearTrees(list);
        // }
        // if (GUILayout.Button("CreateTrees", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     BuildingModelManager buildingModelManager = GameObject.FindObjectOfType<BuildingModelManager>();
        //     //buildingModelManager.CombineAll();
        //     buildingModelManager.CombinedBuildings(list);
        // }

        // if (GUILayout.Button("SaveScenes", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //     subSceneManager.contentType = SceneContentType.TreeWithPart;
        //     subSceneManager.EditorCreateBuildingScenes(list.ToArray());
        // }
        // if (GUILayout.Button("LoadScenes", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //     subSceneManager.EditorLoadScenes(list.ToArray());
        // }
        // if (GUILayout.Button("UnLoadScenes", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //     subSceneManager.EditorUnLoadScenes(list.ToArray());
        // }
        // if (GUILayout.Button("OneKey", GUILayout.Height(buttonHeight)))
        // {
        //     var list=GetBuildingModelInfos();
        //     SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //     subSceneManager.OneKey(list.ToArray());
        // }
        // if (GUILayout.Button("SetBuildings", GUILayout.Height(buttonHeight)))
        // {
        //     SubSceneManager subSceneManager = GameObject.FindObjectOfType<SubSceneManager>();
        //     subSceneManager.SetBuildings_Parts();
        // }
        GUILayout.EndArea();

    }

    protected override int width1
    {
        get
        {
            return 100;
        }
    }
    protected override int width2
    {
        get
        {
            return 100;
        }
    }
    protected override int width3
    {
        get
        {
            return 100;
        }
    }

    void DrawListBlock()
    {

        GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.LIST_BLOCK));
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUIStyle tableHeadStyle = MPGUIStyles.itemStyle;
        //string[] tableTitle = new string[] { "Name", "Vertex", "Renderer", "Vert_In", "Vert_Out0", "Vert_Out1", "Rend_In",
        //"Rend_Out0", "Rend_Out1", "Parts", "Trees", "Scenes", "Finished", };
        for (int i = 0; i < tableTitle.Length; i++)
        {
            string tableHeaderName = tableTitle[i];
            /*
    SortByName,
    SortByAllVertext,
    SortByAllRenderer,
    SortByVertex_In,
    SortByVertex_Out0,
    SortByVertex_Out1,
    SortByRend_In,
    SortByRend_Out0,
    SortByRend_Out1,
             */
            //if (i==0 && sortWays==BuildingSortWays.SortByName)
            //{
            //    tableHeaderName += " *";
            //}
            //else if (i == 1 && sortWays == BuildingSortWays.SortByAllVertext)
            //{
            //    tableHeaderName += " *";
            //}
            //else if (i == 2 && sortWays == BuildingSortWays.SortByAllRenderer)
            //{
            //    tableHeaderName += " *";
            //}

            GUIStyle stytle = tableHeadStyle;
            if (i == (int)sortWays)
            {
                tableHeaderName += " *";
                stytle = new GUIStyle(tableHeadStyle);
                stytle.normal.textColor = new Color(0, 0, 1);
            }
            GUILayoutOption option = GUILayout.Width(width1);
            if (i == 0)
            {
                option = GUILayout.Width(220);
            }
            else if (i == 1)
            {
                option = GUILayout.Width(193);
            }
            else if (i == tableTitle.Length - 1)
            {
                option = GUILayout.Width(width3);
            }
            else if(i>=tableTitle.Length-4)
            {
                option = GUILayout.Width(width2);
            }

            GUILayout.Label(tableHeaderName, stytle, option);
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

    

    /// <param name="element"></param>
    /// <param name="isSelect"></param>
    /// <param name="index"></param>
    protected override void DrawItem(SubSceneElement element, bool isSelect, int index)
    {
        GUIStyle lineStyle = isSelect ? MPGUIStyles.itemBtnStyles[1] : MPGUIStyles.itemBtnStyles[0];
        GUILayout.BeginHorizontal();

        SubSceneValues meshValueRoot = element.rootMeshValue;

        GUIContent icon_expand_Content = element.isGroup ? MPGUIStyles.icon_down_Content : MPGUIStyles.icon_right_Content;

        GUIStyle nameStyle = new GUIStyle(lineStyle);
        if (element.rootObj!=null && element.rootObj.activeInHierarchy == false)
        {
            nameStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);
        }

        if (GUILayout.Button($"{index+1:00}", nameStyle, GUILayout.Height(30), GUILayout.Width(25)))
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

        if (GUILayout.Button(meshValueRoot.SceneName, nameStyle, GUILayout.Height(30), element.isGroup ? GUILayout.Width(180- widthOff) : GUILayout.Width(200- widthOff)))
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

        
        if (GUILayout.Button(meshValueRoot.ParentName, nameStyle, GUILayout.Height(30), element.isGroup ? GUILayout.Width(180- widthOff) : GUILayout.Width(200- widthOff)))
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

        GUILayoutOption[] btnOption =  MPGUIStyles.options_none;

        var values = meshValueRoot.GetValues();
        for(int i=0;i<values.Length;i++)
        {
            object v = values[i];
            GUIStyle style = new GUIStyle(nameStyle);
            string vt = GetValue(v,style);

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

            if (i==0){ //vertex
                    
                if (isSelect)
                {
                    btnOption = new GUILayoutOption[2] { GUILayout.Width(width1-20), GUILayout.Height(30) };
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
                    GameObjectListMeshEditorWindow.ShowWindow(element.rootObj,element.name);
                }
            }
            else{

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


        //
        //if (GUILayout.Button(meshValueRoot.AllVertextCount.ToString(), lineStyle, btnOption))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}
        //if (isSelect && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //{
        //    if (element.isGroup)
        //        isRetract = !isRetract;
        //    else
        //    {
        //        ShowDataList.AddWindow(meshValueRoot.GetVerticsStr(), "Vertices" + "-" + element.name);
        //    }
        //}
        //if (GUILayout.Button(element.refList.Count.ToString(), lineStyle, btnOption))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}
        //if (isSelect && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //{
        //    ShowRefList.AddWindow(element.refList, "RefList" + "-" + element.name);
        //}
        //if (GUILayout.Button(element.rootMeshValue.triangles.ToString(), lineStyle, btnOption))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}
        //if (isSelect && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //{
        //    if (element.isGroup)
        //        isRetract = !isRetract;
        //    else
        //    {
        //        ShowDataList.AddWindow(meshValueRoot.GetTrianglesStr(), "Triangles" + "-" + element.name);
        //    }
        //}
        //if (GUILayout.Button(meshValueRoot.exist_normals ? "??????" : "", lineStyle, (isSelect && meshValueRoot.exist_normals) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}
        //if (isSelect && meshValueRoot.exist_normals && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //{
        //    if (element.isGroup)
        //        isRetract = !isRetract;
        //    else
        //    {
        //        ShowDataList.AddWindow(meshValueRoot.GetNormalsStr(), "Normals" + "-" + element.name);
        //    }
        //}
        //if (GUILayout.Button(meshValueRoot.exist_tangents ? "??????" : "", lineStyle, (isSelect && meshValueRoot.exist_tangents) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}
        //if (isSelect && meshValueRoot.exist_tangents && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //{
        //    if (element.isGroup)
        //        isRetract = !isRetract;
        //    else
        //    {
        //        ShowDataList.AddWindow(meshValueRoot.GetTangentsStr(), "Tangents" + "-" + element.name);
        //    }
        //}
        //if (GUILayout.Button(meshValueRoot.exist_colors ? "??????" : "", lineStyle, (isSelect && meshValueRoot.exist_colors) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}
        //if (isSelect && meshValueRoot.exist_colors && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //{
        //    if (element.isGroup)
        //        isRetract = !isRetract;
        //    else
        //    {
        //        ShowDataList.AddWindow(meshValueRoot.GetColorsStr(), "Colors" + "-" + element.name);
        //    }
        //}

        //for (int m = 0; m < 4; m++)
        //{
        //    if (GUILayout.Button(meshValueRoot.exist_uv[m] ? "??????" : "", lineStyle, (isSelect && meshValueRoot.exist_uv[m]) ? MPGUIStyles.options_exist : MPGUIStyles.options_none))
        //    {
        //        if (SelectIndex != index)
        //        {
        //            isRetract = false;
        //        }
        //        SelectIndex = index;
        //        SelectChildIndex = -1;
        //    }
        //    if (isSelect && meshValueRoot.exist_uv[m] && GUILayout.Button(icon_expand_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_icon))
        //    {
        //        if (element.isGroup)
        //            isRetract = !isRetract;
        //        else
        //        {
        //            ShowDataList.AddWindow(meshValueRoot.GetUVStr(m), "UV" + (m + 1) + "-" + element.name);
        //        }
        //    }
        //}
        //string memoryStr = meshValueRoot.memory >= 1000 ? (meshValueRoot.memory / 1000.0f).ToString("F2") + "MB" : meshValueRoot.memory.ToString("F2") + "KB";

        //if (GUILayout.Button(memoryStr, lineStyle, MPGUIStyles.options_none))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}
        //if (GUILayout.Button(meshValueRoot.isRead ? "??????" : "", lineStyle, MPGUIStyles.options_none))
        //{
        //    if (SelectIndex != index)
        //    {
        //        isRetract = false;
        //    }
        //    SelectIndex = index;
        //    SelectChildIndex = -1;
        //}

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
    /// ??????????????????????????????????????
    /// </summary>
    /// <param name="meshValue"></param>
    /// <param name="index"></param>

    void DrawItemChild(SubSceneValues meshValue, int index)
    {
        bool isSelect = SelectChildIndex == index;
        GUIStyle lineStyle = isSelect ? MPGUIStyles.itemBtnStyles_child[1] : MPGUIStyles.itemBtnStyles_child[0];
        GUILayout.BeginHorizontal();
        GUILayout.Label("|------", GUILayout.Width(32));
        if (GUILayout.Button(meshValue.name, lineStyle, GUILayout.Height(25), GUILayout.Width(180)))
        {
            SelectChildIndex = index;
        }

        GUILayoutOption[] btnOption = isSelect ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none;


        if (GUILayout.Button(meshValue.AllVertextCount.ToString("F1"), lineStyle, btnOption))
        {
            SelectChildIndex = index;
        }
        if (GUILayout.Button(meshValue.AllRendererCount.ToString(), lineStyle, btnOption))
        {
            SelectChildIndex = index;
        }

        //if (isSelect && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        //{
        //    ShowDataList.AddWindow(meshValue.GetVerticsStr(), "Vertices" + "-" + meshValue.parentName + "=>" + meshValue.mesh.name);
        //}

        //if (GUILayout.Button("-", lineStyle, MPGUIStyles.options_child_none))
        //{
        //    SelectChildIndex = index;
        //}


        //if (GUILayout.Button(meshValue.triangles.ToString(), lineStyle, btnOption))
        //{
        //    SelectChildIndex = index;
        //}
        //if (isSelect && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        //{
        //    ShowDataList.AddWindow(meshValue.GetTrianglesStr(), "Triangles" + "-" + meshValue.parentName + "=>" + meshValue.mesh.name);
        //}


        //if (GUILayout.Button(meshValue.exist_normals ? "??????" : "", lineStyle, (isSelect && meshValue.exist_normals) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
        //{
        //    SelectChildIndex = index;
        //}
        //if (isSelect && meshValue.exist_normals && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        //{
        //    ShowDataList.AddWindow(meshValue.GetNormalsStr(), "Normals" + "-" + meshValue.mesh.name);
        //}
        //if (GUILayout.Button(meshValue.exist_tangents ? "??????" : "", lineStyle, (isSelect && meshValue.exist_tangents) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
        //{
        //    SelectChildIndex = index;
        //}
        //if (isSelect && meshValue.exist_tangents && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        //{
        //    ShowDataList.AddWindow(meshValue.GetTangentsStr(), "Tangents" + "-" + meshValue.mesh.name);
        //}
        //if (GUILayout.Button(meshValue.exist_colors ? "??????" : "", lineStyle, (isSelect && meshValue.exist_colors) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
        //{
        //    SelectChildIndex = index;
        //}
        //if (isSelect && meshValue.exist_colors && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        //{
        //    ShowDataList.AddWindow(meshValue.GetColorsStr(), "Colors" + "-" + meshValue.mesh.name);
        //}

        //for (int m = 0; m < 4; m++)
        //{
        //    if (GUILayout.Button(meshValue.exist_uv[m] ? "??????" : "", lineStyle, (isSelect && meshValue.exist_uv[m]) ? MPGUIStyles.options_child_exist : MPGUIStyles.options_child_none))
        //    {
        //        SelectChildIndex = index;
        //    }
        //    if (isSelect && meshValue.exist_uv[m] && GUILayout.Button(MPGUIStyles.icon_right_Content, MPGUIStyles.icon_tab_Style, MPGUIStyles.options_child_icon))
        //    {
        //        ShowDataList.AddWindow(meshValue.GetUVStr(m), "UV" + (m + 1) + "-" + meshValue.mesh.name);
        //    }
        //}
        //string memoryStr = meshValue.memory >= 1000 ? (meshValue.memory / 1000.0f).ToString("F2") + "MB" : meshValue.memory.ToString("F2") + "KB";
        //if (GUILayout.Button(memoryStr, lineStyle, MPGUIStyles.options_child_none))
        //{
        //    SelectChildIndex = index;
        //}

        //if (GUILayout.Button(meshValue.isRead ? "??????" : "", lineStyle, MPGUIStyles.options_child_none))
        //{
        //    SelectChildIndex = index;
        //}
        GUILayout.EndHorizontal();
    }

    private void DrawDataBlockItem(string title,string value)
    {
        GUILayout.Space(2);
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        GUILayout.Space(5);
        GUILayout.Label(value, MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// ????????????????????????
    /// </summary>
    void DrawDataBlock()
    {
        GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.DATA_BLOCK), "");
        //int modelCount = dataChart.modelCount;
        //if (modelCount == 0)
        //{
        //    modelCount = 1;
        //}
        GUILayout.BeginArea(MPGUIStyles.BorderArea(MPGUIStyles.DATA_BLOCK, 5));
        GUILayout.Label("Data Panel", MPGUIStyles.centerStyle);

        //// GUILayout.Space(10);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("AllVertextCount", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.Space(5);
        //GUILayout.Label(dataChart.AllVertextCount.ToString(), MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.EndHorizontal();

        DrawDataBlockItem("AllVertextCount", dataChart.AllVertextCount.ToString("F1"));
        DrawDataBlockItem("AllRendererCount", dataChart.AllRendererCount.ToString());
        DrawDataBlockItem("InVertextCount", dataChart.InVertextCount.ToString("F1"));
        DrawDataBlockItem("Out0VertextCount", dataChart.Out0VertextCount.ToString("F1"));
        DrawDataBlockItem("Out1VertextCount", dataChart.Out1VertextCount.ToString("F1"));
        DrawDataBlockItem("InRendererCount", dataChart.InRendererCount.ToString());
        DrawDataBlockItem("Out0RendererCount", dataChart.Out0RendererCount.ToString());
        DrawDataBlockItem("Out1RendererCount", dataChart.Out1RendererCount.ToString());

        //GUILayout.Space(5);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Asset Count", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.Space(5);
        //GUILayout.Label(dataChart.assetCount.ToString() + " [" + (int)((float)dataChart.assetCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.EndHorizontal();

        //GUILayout.Space(5);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Normals", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.Space(5);
        //GUILayout.Label(dataChart.normalCount.ToString() + " [" + (int)((float)dataChart.normalCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.EndHorizontal();
        //GUILayout.Space(5);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Tangents", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.Space(5);
        //GUILayout.Label(dataChart.tangentCount.ToString() + " [" + (int)((float)dataChart.tangentCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.EndHorizontal();
        //GUILayout.Space(5);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Colors", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.Space(5);
        //GUILayout.Label(dataChart.colorCount.ToString() + " [" + (int)((float)dataChart.colorCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.EndHorizontal();
        //GUILayout.Space(5);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("UV3,UV4", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.Space(5);
        //GUILayout.Label(dataChart.UVXCount.ToString() + " [" + (int)((float)dataChart.UVXCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.EndHorizontal();

        //GUILayout.Space(5);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Readable", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.Space(5);
        //GUILayout.Label(dataChart.readableCount.ToString() + " [" + (int)((float)dataChart.readableCount / modelCount * 100) + "%]", MPGUIStyles.dataAreaStyle, GUILayout.Width(295));
        //GUILayout.EndHorizontal();



        GUILayout.EndArea();


    }
}

class SubSceneDataClass
{
    public float AllVertextCount = 0;
    public int AllRendererCount = 0;

    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;

    public void Reset()
    {
        AllVertextCount = 0;
        AllRendererCount = 0;

        InVertextCount = 0;
        Out0VertextCount = 0;
        Out1VertextCount = 0;

        InRendererCount = 0;
        Out0RendererCount = 0;
        Out1RendererCount = 0;
    }
}

enum SubSceneFilterType
{
    All,In,Out0,Out1,In_Tree,Out0_Tree,Out1_Tree,Out0_TreeNode_Shown,Out0_TreeNode_Hidden
}

enum SubSceneSortWays
{
    SortByName,
    SortByParent,
    SortByAllVertext,
    SortByAllRenderer,
    SortBySceneType,
    SortByContentType,
    SortByIsLoaded
}

public class SubSceneElement: ListItemElement<SubSceneValues>
{
    public SubScene_Base modelInfo;
    public SubSceneElement(GameObject _rootObj, bool _isAsset, bool _isSkin = false) : base(_rootObj, _isAsset, _isSkin)
    {

    }
    public SubSceneElement(SubScene_Base info)
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
            rootMeshValue = new SubSceneValues(modelInfo);
            rootMeshValue.name = modelInfo.name;
        }
        else
        {
            rootMeshValue = new SubSceneValues();
        }


    }
}

public class  SubSceneValues 
    : MeshValues
    //: ListItemElementValues
{
    public string name;

    public SubScene_Base info;

    public GameObject partGo;

    public float AllVertextCount = 0;
    public int AllRendererCount = 0;

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

    public bool IsLoaded=false;

    public string SceneName="";

    public string ParentName="";

    public string SceneType="";
    public string ContentType="";

    public SubSceneValues()
    {
        name = "";
    }

    public SubSceneValues(SubScene_Base modelInfo)
    {
        SetValues(modelInfo);
    }

    public void SetValues(SubScene_Base modelInfo)
    {
        //this.name = modelInfo.name;

        this.info = modelInfo;
        this.AllVertextCount = modelInfo.vertexCount;
        this.AllRendererCount = modelInfo.rendererCount;

        this.IsLoaded=modelInfo.IsLoaded;

        this.SceneName=modelInfo.sceneName;
        if(string.IsNullOrEmpty( this.SceneName))
        {
            this.SceneName=modelInfo.name;
        }

        this.ParentName=modelInfo.sceneParent.name;
        this.ContentType=modelInfo.contentType.ToString();
        this.SceneType=modelInfo.GetType().Name;

        // this.InVertextCount = modelInfo.InVertextCount;
        // this.Out0VertextCount = modelInfo.Out0VertextCount;
        // this.Out1VertextCount = modelInfo.Out1VertextCount;
        // this.InRendererCount = modelInfo.InRendererCount;
        // this.Out0RendererCount = modelInfo.Out0RendererCount;
        // this.Out1RendererCount = modelInfo.Out1RendererCount;

        // this.Out0BigVertextCount = modelInfo.Out0BigVertextCount;
        // this.Out0SmallVertextCount = modelInfo.Out0SmallVertextCount;
        // this.Out0BigRendererCount = modelInfo.Out0BigRendererCount;
        // this.Out0SmallRendererCount = modelInfo.Out0SmallRendererCount;
    }

    internal void Add(SubSceneValues modelInfo)
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
        return new object[] { AllVertextCount,AllRendererCount,
        SceneType,     ContentType, IsLoaded, "-","-" };
        
    }

    // public string GetOut0VertexInfo()
    // {
    //     return $"{Out0VertextCount:F0}={info.Out0SmallVertextCount:F0}+{info.Out0BigVertextCount:F0}";
    // }

    // public string GetOut0RenderInfo()
    // {
    //     return $"{Out0RendererCount:F0}={info.Out0SmallRendererCount:F0}+{info.Out0BigRendererCount:F0}";
    // }


}
