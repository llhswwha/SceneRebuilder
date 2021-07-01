using ModelEditorWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace ModelEditorWindow
{
    public class ModelPecentNum
    {
        public ModelPecentNum(List<float>values)
        {
            lodSetting=new List<float>();
            lodSetting.AddRange(values);
        }
        public List<float> lodSetting;
    }

    public class ModelDetailInfo
    {
        public string DirctoryName;
        public DirectoryInfo diretcoty;
        public LodInfo lod0;
        public LodInfo lod0_H;
        public LodInfo lod1;
        public LodInfo lod1_H;
        public LodInfo lod2;
        public LodInfo lod2_H;
        public LodInfo lod3;
        public LodInfo lod3_H;
        public LodInfo lod4;
        public LodInfo lod4_H;

        public void Dispose()
        {
            DestoryLod(lod0);
            DestoryLod(lod1);
            DestoryLod(lod2);
            DestoryLod(lod3);
            DestoryLod(lod4);
        }
        private void DestoryLod(LodInfo info)
        {
            if (info != null && info.obj != null) UnityEngine.GameObject.DestroyImmediate(info.obj);
        }
    }
    /// <summary>
    /// 模型信息
    /// </summary>
    public class LodInfo
    {
        public FileInfo fileInfo;
        public GameObject obj;
        public string assetPath;

        public float vertexCount;
        public float vertexPercent;

        public int childCount;
        public float childPercent;

        public float size;
        public float sizePercent;

        public bool IsObjectEmpty()
        {
            return obj == null;
        }
    }

    public enum ModelType
    {
        size,
        vertex,
        childCount,
    }
    /// <summary>
    /// 展示类型 （工程中模型||场景中模型）
    /// </summary>
    public enum DisplayType
    {
        Project,
        Scene
    }

    public enum LODMode
    {
        一般模式,
        合并模式,
        混合模式
    }
}


public class ModelLODWindow : EditorWindow
{
    [MenuItem("Tools/Model/LodDetailWidnow")]
    static void OpenWindow()
    {
        EditorWindow.GetWindow(typeof(ModelLODWindow));
    }

    static DisplayType displayType;
    static bool isLoadSceneModel;//是否加载场景模型
    static string assetPath = @"\Models\Plant_SuQian\Models";//\Plant_SuQian\Models  //加载模型的父文件夹

    static string loadExportDefault = @"\Models\Plant_SuQian\LodGroupExport";  //导出生成LOD物体的文件夹（重置参数时用）
    static string lodExportPath = @"\Models\Plant_SuQian\LodGroupExport";     //导出生成LOD物体的文件夹（这个参数可以在面板中修改）

    static Vector2 scrollPos = Vector2.zero;//记录滚动条位置
    static bool isSearchBtnClick = false;//是否点击了加载模型按钮
    static bool isToggleAll = false;//是否点击了选中所有物体

    static Dictionary<string, ModelDetailInfo> fileChooseDic;

    static ModelType modelType;//筛选类型 (如果增加了一个类型，需要在LodInfo这个类中增加对应字段，GetFileLODInfo方法中为其赋值，SortList方法中增加排序，GetLodInfoByType方法返回对应信息)
    static List<ModelDetailInfo> allModelInfoes;//场景或工程中，需要展示的模型信息
    static List<ModelDetailInfo> sceneModelInfoes;//场景模型信息
    static List<ModelDetailInfo> projectModelInfoes;//工程模型信息
    static string projectDirctoryPath;//项目所在路径
    static bool isSortByName;//是否按名称排序

    static bool expandLODGroupPart;//是否展开模型导出部分
    static float fileSizeLimit = 100;//大于100Mb的不加载模型信息

    static GameObject LodGroupSingle;//单个大文件，自动添加lodMesh
    static Vector2 totalScrollPos = Vector2.zero;//记录滚动条位置

    static LODMode lodMode;//LOD选择模式
    static string fliterString;//按名称过滤
    static bool searchingLODSwitch;//是否查询LODGroup是否存在
    private void OnGUI()
    {
        //totalScrollPos = GUILayout.BeginScrollView(totalScrollPos, false, true, GUILayout.Height(800));
        GUILayout.BeginHorizontal();
        GUILayout.Label("模型展示切换:(Project:工程文件夹模型 Scene:场景中模型)",GUILayout.Width(400));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        DisplayType m1 = (DisplayType)EditorGUILayout.EnumPopup(displayType, GUILayout.Width(300));
        GUILayout.EndHorizontal();
        //bool loadScene = GUILayout.Toggle(isLoadSceneModel, "是否加载场景中模型（如加载场景中模型，请选中相关模型）", GUILayout.Width(400));
        if (m1!=displayType)//不相同，标识切换了场景||工程模式
        {
            displayType = m1;
            isLoadSceneModel = displayType==DisplayType.Project?false:true;
            //ResetValue();
            if(isLoadSceneModel)
            {
                if(allModelInfoes!=null)
                {
                    allModelInfoes.Clear();
                    if (sceneModelInfoes != null) allModelInfoes.AddRange(sceneModelInfoes);
                }                
            }
            else
            {
                if (allModelInfoes != null)
                {
                    allModelInfoes.Clear();
                    if(projectModelInfoes!=null)allModelInfoes.AddRange(projectModelInfoes);
                }
            }
        }        
        //加载场景中模型||加载文件夹中模型信息
        if (isLoadSceneModel)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("除场景选中模型外，可选中一个父物体，把子物体也加入信息列表中");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            selectParentObj = EditorGUILayout.ObjectField("模型父物体", selectParentObj, typeof(GameObject), true,GUILayout.Width(500)) as GameObject;
            GUILayout.EndHorizontal();
            LoadModelInfo();
        }
        else
        {
            //路径设置
            GUILayout.Label(@" 模型文件夹相对Assets的路径(可更改) Eg:\Models\Plant_SuQian\Models");
            GUILayout.BeginHorizontal();
            string modelPath = EditorGUILayout.TextField(assetPath, GUILayout.Width(400));
            if (modelPath != assetPath)
            {
                assetPath = modelPath;
            }
            GUILayout.EndHorizontal();
            //加载模型文件
            LoadModelInfo();
        }
        //GUILayout.EndScrollView();
    }
    private void OnDestroy()
    {
        Debug.Log("Model Lod window close...");
        ClearInfo();
    }
    #region 加载场景中的物体
    static GameObject selectParentObj;
    static bool showSelectObject;
    /// <summary>
    /// 获取场景中的模型信息
    /// </summary>
    /// <returns></returns>
    private List<ModelDetailInfo> GetSceneModleFiles()
    {
        //Dictionary<string, GameObject> objDic = GetSceneSelectObjects();
        //不加入场景中手动选中的物体，只加载父物体下模型
        Dictionary<string, GameObject> objDic = new Dictionary<string, GameObject>();
        if (selectParentObj!=null)
        {
            foreach(Transform obj in selectParentObj.transform)
            {
                if (obj == selectParentObj.transform) continue;
                if(!objDic.ContainsKey(obj.name))
                {
                    objDic.Add(obj.name,obj.gameObject);
                }
            }
        }
        List<ModelDetailInfo> details = new List<ModelDetailInfo>();
        foreach(var item in objDic.Values)
        {
            ModelDetailInfo detail = GetObjectDetail(item);
            if(detail!=null)details.Add(detail);
        }
        return details;
    }
    /// <summary>
    /// 获取场景中的物体
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string,GameObject> GetSceneSelectObjects()
    {
        GameObject[] objs = Selection.gameObjects;
        Dictionary<string, GameObject> objDic = new Dictionary<string, GameObject>();
        if(objs!=null)
        {
            foreach (var item in objs)
            {
                if (!objDic.ContainsKey(item.name)) objDic.Add(item.name, item);
            }
        }        
        return objDic;
        //return Resources.FindObjectsOfTypeAll<GameObject>().Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go)) && go.hideFlags == HideFlags.None).ToArray();
    }
    private ModelDetailInfo GetObjectDetail(GameObject obj)
    {
        ModelDetailInfo mdInfo = new ModelDetailInfo();
        mdInfo.DirctoryName = obj.name;
        //1.模型下有lod  子物体可能是多个LOD物体
        if (obj.transform.childCount > 1 && obj.transform.GetChild(0).name.ToLower().Contains("lod"))
        {                     
            foreach (Transform item in obj.transform)
            {
                string nameLower = item.name.ToLower();
                if (nameLower.Contains(Lod0)) mdInfo.lod0 = GetFileLODInfo(null,item.gameObject, FullPath(item.gameObject), null);
                else if (nameLower.Contains(Lod1)) mdInfo.lod1 = GetFileLODInfo(null, item.gameObject, FullPath(item.gameObject), mdInfo.lod0);
                else if (nameLower.Contains(Lod2)) mdInfo.lod2 = GetFileLODInfo(null, item.gameObject, FullPath(item.gameObject), mdInfo.lod0);
                else if (nameLower.Contains(Lod3)) mdInfo.lod3 = GetFileLODInfo(null, item.gameObject, FullPath(item.gameObject), mdInfo.lod0);
                else if (nameLower.Contains(Lod4)) mdInfo.lod4 = GetFileLODInfo(null, item.gameObject, FullPath(item.gameObject), mdInfo.lod0);
            }            
        }
        else
        {
            //模型下没LOD，就统计这个模型的数据
            mdInfo.lod0 = GetFileLODInfo(null, obj.gameObject, FullPath(obj.gameObject), null);           
        }
        LodInfo lodT = GetObjectByModelInfo(mdInfo);
        if (lodT != null)
        {
            if(lodT.fileInfo!=null)
            {
                mdInfo.diretcoty = lodT.fileInfo.Directory;
                mdInfo.DirctoryName = mdInfo.diretcoty.Name;
            }
            return mdInfo;
        }
        else
        {
            return null;
        }
    }
    private static string FullPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : FullPath(go.transform.parent.gameObject) + "/" + go.name;
    }

    private void InitSceneModelSetting()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("关闭选中物体", GUILayout.Width(180)))
        {
            foreach (var item in fileChooseDic.Values)
            {
                SetObjActive(item, false);
            }
        }
        if (GUILayout.Button("开启选中物体", GUILayout.Width(180)))
        {
            foreach (var item in fileChooseDic.Values)
            {
                SetObjActive(item,true);
            }
        }
        if (GUILayout.Button("删除选中物体", GUILayout.Width(180)))
        {
            foreach (var item in fileChooseDic.Values)
            {
                if(allModelInfoes!=null&&allModelInfoes.Contains(item)) allModelInfoes.Remove(item);
                if (sceneModelInfoes!=null&&sceneModelInfoes.Contains(item)) sceneModelInfoes.Remove(item);
                item.Dispose();
            }
            fileChooseDic.Clear();
        }
        GUILayout.EndHorizontal();
    }

    private void SetObjActive(ModelDetailInfo model, bool isOn)
    {
        if (model.lod0!= null) model.lod0.obj.SetActive(isOn);
        if (model.lod1!= null) model.lod1.obj.SetActive(isOn);
        if (model.lod2!= null) model.lod2.obj.SetActive(isOn);
        if (model.lod3!= null) model.lod3.obj.SetActive(isOn);
    }

    #endregion
    #region 加载模型信息

    static GameObject replaceParentObj;//选择后，可把lod_h的物体替换到场景中

    private void LoadModelInfo()
    {
        GUILayout.BeginHorizontal();
        if (!isLoadSceneModel)
            fileSizeLimit = EditorGUILayout.FloatField(string.Format("大于{0}Mb,只显示Size信息", fileSizeLimit), fileSizeLimit, GUILayout.Width(300));
        GUILayout.Space(25);
        fliterString = EditorGUILayout.TextField(fliterString, GUILayout.Width(400));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        //1.模型加载和重置部分    
        if (GUILayout.Button("加载模型信息", GUILayout.Width(180)))
        {
            isSearchBtnClick = true;            
            ClearInfo();
        }
        if (GUILayout.Button("重置所有信息", GUILayout.Width(180)))
        {
            ResetValue();
        }
        GUILayout.EndHorizontal();

        //2.类型筛选部分
        GUILayout.BeginHorizontal();
        ModelType m1 = (ModelType)EditorGUILayout.EnumPopup(modelType, GUILayout.Width(300));
        bool isRefresh = false;
        if (m1 != modelType)
        {
            modelType = m1;
            isRefresh = true;
        }

        bool sortName = GUILayout.Toggle(isSortByName,"是否按名称排序",GUILayout.Width(100));
        if(sortName!=isSortByName)
        {
            isSortByName = sortName;
            isRefresh = true;
        }       
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        bool isToggle = GUILayout.Toggle(isToggleAll, "选中", GUILayout.Width(60));
        bool isToggleAllChange = isToggle == isToggleAll ? false : true;//判断Toggle是否被手动点击
        isToggleAll = isToggle;
        GUILayout.Label("Active", GUILayout.Width(60));
        GUILayout.Label("HasLOD",GUILayout.Width(60));
        GUILayout.Label("物体", GUILayout.Width(200));
        GUILayout.Label("名称", GUILayout.Width(300));
        GUILayout.Label("LOD0", GUILayout.Width(150));
        GUILayout.Label("LOD0_H", GUILayout.Width(150));
        GUILayout.Label("LOD1", GUILayout.Width(150));
        GUILayout.Label("LOD1_H", GUILayout.Width(150));
        GUILayout.Label("LOD2", GUILayout.Width(150));
        GUILayout.Label("LOD2_H", GUILayout.Width(150));
        GUILayout.Label("LOD3", GUILayout.Width(150));
        GUILayout.Label("LOD3_H", GUILayout.Width(150));
        GUILayout.Label("LOD4", GUILayout.Width(150));
        GUILayout.Label("LOD4_H", GUILayout.Width(150));
        GUILayout.EndHorizontal();
        //3.列表展示部分
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(250));        
        if (isSearchBtnClick)
        {
            searchingLODSwitch = false;//只有第一次展示，才判断LODGroup是否已经存在

            if (allModelInfoes == null)
            {
                allModelInfoes = new List<ModelDetailInfo>();
                if(isLoadSceneModel)
                {
                    sceneModelInfoes = GetSceneModleFiles();//加载场景中的模型
                    if (sceneModelInfoes != null) allModelInfoes.AddRange(sceneModelInfoes);
                }
                else
                {
                    string directoryName = Application.dataPath + assetPath;
                    projectModelInfoes = GetModelFiles(directoryName);//获取所有模型信息，并统计各类型数量
                    if(projectModelInfoes!=null) allModelInfoes.AddRange(projectModelInfoes);
                }
                SortList();//刷新默认排序
            }
            if (allModelInfoes != null && allModelInfoes.Count > 0)
            {
                if (!lodExportPath.StartsWith(@"\")) lodExportPath = @"\" + lodExportPath;
                if (!lodExportPath.EndsWith(@"\")) lodExportPath = lodExportPath + @"\";
                string exportFullPath = Application.dataPath + lodExportPath;

                if (fileChooseDic == null) fileChooseDic = new Dictionary<string, ModelDetailInfo>();
                if (isRefresh) SortList();//后面只有更换了类型，才重新排序一次
                for(int i=0;i<allModelInfoes.Count;i++)
                {
                    ModelDetailInfo item = allModelInfoes[i]; 
                    if(!string.IsNullOrEmpty(fliterString))
                    {
                        if (!item.DirctoryName.ToLower().Contains(fliterString.ToLower())) continue;
                    }
                    GUILayout.BeginHorizontal();
                    bool isDirctoryChoose = fileChooseDic.ContainsKey(item.DirctoryName);
                    bool isOn = isToggleAllChange ? isToggleAll : fileChooseDic.ContainsKey(item.DirctoryName);//判断是否点击ToggleAll
                    //a.选中Toggle
                    int index = i + 1;
                    bool resultToggle = EditorGUILayout.ToggleLeft(index.ToString(),isOn, GUILayout.Width(60));                    
                    if (resultToggle != isDirctoryChoose) SaveToggleInfo(item, resultToggle);
                    //b.模型和是否激活
                    LodInfo actionObj = GetObjectByModelInfo(item);
                    if(actionObj != null)
                    {
                        GameObject disaplyModel = actionObj.obj;
                        bool isActive = GUILayout.Toggle(disaplyModel.activeSelf, "", GUILayout.Width(60));
                        if (isActive != disaplyModel.activeSelf) disaplyModel.SetActive(isActive);
                        bool hasLod = File.Exists(string.Format(@"{0}\{1}.prefab", exportFullPath, item.DirctoryName));
                        GUILayout.Toggle(hasLod, "", GUILayout.Width(60));
                        EditorGUILayout.ObjectField("", disaplyModel, typeof(GameObject), true, GUILayout.Width(200));
                    }
                    else
                    {
                        GUILayout.Toggle(false, "", GUILayout.Width(60));
                        if(GUILayout.Button("加载模型下Lod信息", GUILayout.Width(200)))
                        {
                            ReLoadModelInfo(ref item);
                        }
                    }                   
                    //点击名称可选中文件夹
                    if(GUILayout.Button(item.DirctoryName,GUI.skin.label, GUILayout.Width(300)))
                    {
                        SelectDirctory(item.diretcoty);
                    }
                    GUILayout.Label(GetLodInfoByType(item.lod0), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod0_H), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod1), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod1_H), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod2), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod2_H), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod3), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod3_H), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod4), GUILayout.Width(150));
                    GUILayout.Label(GetLodInfoByType(item.lod4_H), GUILayout.Width(150));
                    GUILayout.EndHorizontal();
                }
                
            }
        }
        GUILayout.EndScrollView();

        if(isLoadSceneModel)
        {
            //5.操作场景中的模型（Scene）
            InitSceneModelSetting();
        }
        else
        {
            //5.工程文件夹中，LOD参数设置及导出(Project)
            InitLodPart();
        }     
    }

    static bool isLod0Select;
    static float percent0=0.6f;

    static bool isLod1Select;
    static float percent1 = 0.3f;

    static bool isLod2Select;
    static float percent2 = 0.14f;

    static bool isLod3Select;
    static float percent3 = 0.04f;

    static bool isLod4Select;
    static float percent4 = 0.02f;

    static LODFadeMode fadeMode;  

    static bool addSceneObject=true;

    public ModelPecentNum oneModelSetting;
    public ModelPecentNum twoModelSetting;
    public ModelPecentNum threeModelSetting;
    public ModelPecentNum fourModelSetting;
    public ModelPecentNum fiveModelSetting;

    static bool replaceSceneModel = true;
    /// <summary>
    /// 生成预设部分
    /// </summary>
    private void InitLodPart()
    {
        GUILayout.Label("功能1：将LOD_H模型加载到场景中");
        GUILayout.BeginHorizontal();
        replaceParentObj = EditorGUILayout.ObjectField("Lod_H在场景中的父物体", replaceParentObj, typeof(GameObject), true, GUILayout.Width(500)) as GameObject;
        replaceSceneModel = GUILayout.Toggle(replaceSceneModel, "是否替换场景中模型，（替换规则 去掉_h后再搜索）", GUILayout.Width(300));
        GUILayout.EndHorizontal();
        if(GUILayout.Button("将勾选的LOD_H模型加入到场景中",GUILayout.Width(300)))
        {
            AddLODHToScenes();
        }


        GUILayout.Space(10);
        GUILayout.Label("功能2：生成LodGroup，加入到场景中并导出到文件夹");
        expandLODGroupPart = GUILayout.Toggle(expandLODGroupPart,"展开功能2",GUILayout.Width(150));
        if(expandLODGroupPart)
        {
            GUILayout.Label("生成Lod文件（建立一个空物体，添加勾选的lod物体）:", GUILayout.Width(300));
            GUILayout.BeginHorizontal();
            LODMode m1 = (LODMode)EditorGUILayout.EnumPopup(lodMode, GUILayout.Width(300));
            if(m1!=lodMode)
            {
                ResetLODSelect();
                lodMode = m1;
            }
            DisplayLODSetting(lodMode);

            GUILayout.EndHorizontal();
            InputLevelValue();//设置不同模型数量下，LOD百分比

            GUILayout.BeginHorizontal();
            GUILayout.TextField("LodGroup中可配置参数：", GUILayout.Width(180));
            fadeMode = (LODFadeMode)EditorGUILayout.EnumPopup("LodGroup.FadeMode:", fadeMode);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            addSceneObject = GUILayout.Toggle(addSceneObject, "是否在场景中加载LOD预设:", GUILayout.Width(250));
            GUILayout.EndHorizontal();

            lodExportPath = EditorGUILayout.TextField("生成后的路径:", lodExportPath, GUILayout.Width(400));
            bool initLodGroup = GUILayout.Button("生成选中的LodGroup");
            if (initLodGroup)
            {
                if (!isSetNessaryInfo()) return;//如未填写必要信息，弹窗提示
                if (!lodExportPath.StartsWith(@"\")) lodExportPath = @"\" + lodExportPath;
                if (!lodExportPath.EndsWith(@"\")) lodExportPath = lodExportPath + @"\";
                string exportFullPath = Application.dataPath + lodExportPath;
                string assetPath = "Assets" + lodExportPath;
                if (CreateExportDirectory(exportFullPath))
                {
                    int exportSuccessCount = 0;
                    DateTime record = DateTime.Now;
                    string logInfo = "";
                    foreach (var item in fileChooseDic.Values)
                    {
                        if (item == null) continue;
                        GameObject o = new GameObject();
                        o.name = item.DirctoryName;
                        LODGroup group = o.AddComponent<LODGroup>();
                        group.fadeMode = fadeMode;
                        List<LOD> lodInfos = InitLODInfoByType(item,o);
                        if (lodInfos != null && lodInfos.Count > 0)
                        {
                            exportSuccessCount++;
                            SetLodGroupValue(group, lodInfos.ToArray());//根据模型的个数，重新设定显示所占百分比
                            PrefabUtility.SaveAsPrefabAsset(o, assetPath + o.name + ".prefab");
                            if (!addSceneObject) DestroyImmediate(o);
                        }
                        else
                        {
                            logInfo += string.Format("Model:{0} Select LOD type is null\n",item.DirctoryName);
                        }
                    }
                    ShowNotification(new GUIContent("LOD预设生成完毕，详情见Console."));
                    Debug.Log(string.Format("Export finished,SelectCount:{0} ExportSuccessCount:{1} CostTime:{2}s", fileChooseDic.Values.Count, exportSuccessCount,
                        (DateTime.Now - record).TotalSeconds));
                    Debug.LogError("以下模型，没有选中的LOD模型：\n"+logInfo);
                }
                else
                {
                    ShowNotification(new GUIContent("导出文件夹创建失败，导出路径：" + Application.dataPath + lodExportPath));
                }
                searchingLODSwitch = true;
            }
            GUILayout.Label("功能3：场景中建立一个父物体，父物体下有LOD0、LOD1、Lod2等子物体,自动添加LODGroup，并按功能2中百分比生成LODGroup信息");
            LodGroupSingle = EditorGUILayout.ObjectField("LodGroup物体", replaceParentObj, typeof(GameObject), true, GUILayout.Width(500)) as GameObject;
            if(GUILayout.Button("生成LODGroup信息"))
            {
                InitLodGroup(LodGroupSingle);
            }
        }        
    }
    /// <summary>
    /// 根据模式不同，显示LOD勾选项
    /// </summary>
    /// <param name="mode"></param>
    private void DisplayLODSetting(LODMode mode)
    {
        if (mode == LODMode.一般模式)
        {
            isLod0Select = GUILayout.Toggle(isLod0Select, "LOD0", GUILayout.Width(100));
            isLod1Select = GUILayout.Toggle(isLod1Select, "LOD1", GUILayout.Width(100));
            isLod2Select = GUILayout.Toggle(isLod2Select, "LOD2", GUILayout.Width(100));
            isLod3Select = GUILayout.Toggle(isLod3Select, "LOD3", GUILayout.Width(100));
            isLod4Select = GUILayout.Toggle(isLod4Select, "LOD4", GUILayout.Width(100));
        }else if(mode==LODMode.合并模式)
        {
            isLod0Select = GUILayout.Toggle(isLod0Select, "LOD0_H", GUILayout.Width(100));
            isLod1Select = GUILayout.Toggle(isLod1Select, "LOD1_H", GUILayout.Width(100));
            isLod2Select = GUILayout.Toggle(isLod2Select, "LOD2_H", GUILayout.Width(100));
            isLod3Select = GUILayout.Toggle(isLod3Select, "LOD3_H", GUILayout.Width(100));
            isLod4Select = GUILayout.Toggle(isLod4Select, "LOD4_H", GUILayout.Width(100));
        }else if(mode==LODMode.混合模式)
        {
            isLod0Select = GUILayout.Toggle(isLod0Select, "LOD0", GUILayout.Width(100));
            isLod1Select = GUILayout.Toggle(isLod1Select, "LOD0_H", GUILayout.Width(100));
            isLod2Select = GUILayout.Toggle(isLod2Select, "LOD1_H", GUILayout.Width(100));
            isLod3Select = GUILayout.Toggle(isLod3Select, "LOD2_H", GUILayout.Width(100));
            isLod4Select = GUILayout.Toggle(isLod4Select, "LOD3_H", GUILayout.Width(100));
        }
    }
    private void ResetLODSelect()
    {
        isLod0Select = false;
        isLod1Select = false;
        isLod2Select = false;
        isLod3Select = false;
        isLod4Select = false;
    }
    /// <summary>
    /// 根据下拉选择的类型，生成LODInfo
    /// </summary>
    private List<LOD> InitLODInfoByType(ModelDetailInfo item,GameObject parentT)
    {
        List<LOD> lodInfos = new List<LOD>();
        parentT.transform.position = Vector3.zero;
        if (lodMode==LODMode.一般模式)
        {
            lodInfos.AddRange(GetLODInfoBySelect(parentT.transform,item.lod0,item.lod1,item.lod2,item.lod3,item.lod4));
        }
        else if(lodMode==LODMode.合并模式)
        {
            lodInfos.AddRange(GetLODInfoBySelect(parentT.transform, item.lod0_H, item.lod1_H, item.lod2_H, item.lod3_H, item.lod4_H));
        }else if(lodMode == LODMode.混合模式)
        {
            lodInfos.AddRange(GetLODInfoBySelect(parentT.transform,item.lod0, item.lod0_H, item.lod1_H, item.lod2_H, item.lod3_H));
        }
        return lodInfos;
    }
    /// <summary>
    /// 根据选中的LOD，生成对应LOD信息
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="lodFirst"></param>
    /// <param name="lodSecond"></param>
    /// <param name="lodThird"></param>
    /// <param name="lodForth"></param>
    /// <param name="lodFifth"></param>
    /// <returns></returns>
    private List<LOD>GetLODInfoBySelect(Transform parentT,LodInfo lodFirst,LodInfo lodSecond,LodInfo lodThird,LodInfo lodForth,LodInfo lodFifth)
    {
        List<LOD> lodInfos = new List<LOD>();
        if (isLod0Select && lodFirst != null && !string.IsNullOrEmpty(lodFirst.assetPath))
            lodInfos.Add(GetLODInfoByTransform(lodFirst, parentT.transform, percent0));
        if (isLod1Select && lodSecond != null && !string.IsNullOrEmpty(lodSecond.assetPath))
            lodInfos.Add(GetLODInfoByTransform(lodSecond, parentT.transform, percent0));
        if (isLod2Select && lodThird != null && !string.IsNullOrEmpty(lodThird.assetPath))
            lodInfos.Add(GetLODInfoByTransform(lodThird, parentT.transform, percent0));
        if (isLod3Select && lodForth != null && !string.IsNullOrEmpty(lodForth.assetPath))
            lodInfos.Add(GetLODInfoByTransform(lodForth, parentT.transform, percent0));
        if (isLod4Select && lodFifth != null && !string.IsNullOrEmpty(lodFifth.assetPath))
            lodInfos.Add(GetLODInfoByTransform(lodFifth, parentT.transform, percent0));
        return lodInfos;
    }

    private void InitLodGroup(GameObject obj)
    {
        if (obj == null) return;
        LODGroup group = obj.transform.GetComponent<LODGroup>();
        if (group == null) group = obj.AddComponent<LODGroup>();
        List<LOD> lodInfos = new List<LOD>();
        for (int i=0;i<obj.transform.childCount;i++)
        {
            GameObject child = obj.transform.GetChild(i).gameObject;            
            lodInfos.Add(GetLODByRenders(0.1f,child.transform));                   
        }
        if (lodInfos != null && lodInfos.Count > 0)
        {
            SetLodGroupValue(group, lodInfos.ToArray());//根据模型的个数，重新设定显示所占百分比
        }
    }

    /// <summary>
    /// 添加LOD_H模型到场景中
    /// </summary>
    private void AddLODHToScenes()
    {
        if(fileChooseDic==null||fileChooseDic.Count==0)
        {
            ShowNotification(new GUIContent("请勾选相关模型"));
        }
        else
        {
            int lodHNum = 0;
            int replaceNum = 0;
            string notFindObj = "";
            foreach(var item in fileChooseDic.Values)
            {
                if (item.lod0_H == null || item.lod0_H.obj == null) continue;
                lodHNum++;
                GameObject initObj = Instantiate(item.lod0_H.obj);
                initObj.transform.name = initObj.transform.name.Replace("(Clone)","");
                if(replaceParentObj!=null)
                {
                    Dictionary<string, GameObject> childNameDic = GetChildNameList(replaceParentObj);                    
                    var nameList = childNameDic.Keys.ToList();
                    if (replaceSceneModel)
                    {
                        string nameT = initObj.transform.name.ToLower().Replace("_h","");
                        string sceneObj = nameList.Find(i=>i.Contains(nameT));
                        if(!string.IsNullOrEmpty(sceneObj)&&childNameDic.ContainsKey(sceneObj))
                        {
                            replaceNum++;
                            GameObject obj = childNameDic[sceneObj];
                            if (obj != null) DestroyImmediate(obj);
                        }
                        else
                        {
                            notFindObj += nameT + "\n";
                        }
                    }
                    initObj.transform.parent = replaceParentObj.transform;
                }
            }
            string log = string.Format("导入场景完成，LOD_H数量：{0} 替换模型数量：{1}",lodHNum,replaceNum);
            if (!string.IsNullOrEmpty(notFindObj)) log += string.Format("未找到替换模型数量：{0} 名称列表如下：{1}",lodHNum-replaceNum,notFindObj);
            ShowNotification(new GUIContent(log));
            Debug.Log(log);
        }
    }
    /// <summary>
    /// 获取父物体下，子物体的名称
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    private Dictionary<string,GameObject>GetChildNameList(GameObject parent)
    {
        Dictionary<string, GameObject> childName = new Dictionary<string, GameObject>();
        foreach(Transform child in parent.transform)
        {
            if (child == parent.transform) continue;
            string nameLower = child.name.ToLower();
            if (!childName.ContainsKey(nameLower)) childName.Add(nameLower, child.gameObject);
        }
        return childName;
    }
    private void SetLodGroupValue(LODGroup group,LOD[] lodList)
    {
        if (lodList == null || lodList.Length == 0) return;
        int count = lodList.Length;
        ModelPecentNum percentSetting = GetModelPercentByCount(count);
        for (int i=0;i< count; i++)
        {
            lodList[i].screenRelativeTransitionHeight = percentSetting.lodSetting[i];
        }
        group.SetLODs(lodList);
    }
    private ModelPecentNum GetModelPercentByCount(int modelCount)
    {
        if (modelCount == 1) return oneModelSetting;
        else if (modelCount == 2) return twoModelSetting;
        else if(modelCount == 3) return threeModelSetting;
        else if(modelCount == 4) return fourModelSetting;
        else return fiveModelSetting;
    }
    /// <summary>
    /// 设置LOD百分比
    /// </summary>
    private void InputLevelValue()
    {
        if (oneModelSetting == null) oneModelSetting = new ModelPecentNum(new List<float> { 0.1f});  //0.2-1 都显示单个模型
        if (twoModelSetting == null) twoModelSetting = new ModelPecentNum(new List<float> { 0.9f, 0.1f });
        if (threeModelSetting == null) threeModelSetting = new ModelPecentNum(new List<float> { 0.9f, 0.7f, 0.1f });
        if (fourModelSetting == null) fourModelSetting = new ModelPecentNum(new List<float> { 0.9f, 0.7f, 0.5f, 0.1f });
        if (fiveModelSetting == null) fiveModelSetting = new ModelPecentNum(new List<float> { 0.9f, 0.7f, 0.5f, 0.3f, 0.1f });
        InitPercentLabel(1,oneModelSetting);
        InitPercentLabel(2, twoModelSetting);
        InitPercentLabel(3, threeModelSetting);
        InitPercentLabel(4, fourModelSetting);
        InitPercentLabel(5, fiveModelSetting);
    }

    private void InitPercentLabel(int num,ModelPecentNum pSetting)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(string.Format("{0}个物体的情况下，LOD百分比(1-0):",num));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        float lastPercent = 1;
        for(int i=0;i<num;i++)
        {
            if(pSetting.lodSetting.Count<=num)
            {                
                pSetting.lodSetting[i] = EditorGUILayout.FloatField(string.Format("{0}-{1}:", lastPercent,pSetting.lodSetting[i]), pSetting.lodSetting[i], GUILayout.Width(200));
                lastPercent = pSetting.lodSetting[i];
            }
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 是否填写必须信息
    /// </summary>
    /// <returns></returns>
    private bool isSetNessaryInfo()
    {
        if(!isLod0Select&&!isLod1Select&&!isLod2Select&&!isLod3Select&&!isLod4Select)
        {
            ShowNotification(new GUIContent("请选择需要生成的Lod细节（勾选Lod0||Lod1||Lod2||Lod3||Lod4）!"));
            return false;
        }else if(fileChooseDic!=null&&fileChooseDic.Values.Count==0)
        {
            ShowNotification(new GUIContent("请选择需要导出的物体!"));
            return false;
        }
        else if(string.IsNullOrEmpty(lodExportPath))
        {
            ShowNotification(new GUIContent("LODGroup生成后路径未设置!"));
            return false;
        }
        else
        {
            return true;
        }
    }

    private LOD GetLODInfoByTransform(LodInfo lodInfo,Transform parentT,float percent)
    {
        GameObject obj = lodInfo.obj==null?AssetDatabase.LoadAssetAtPath<GameObject>(lodInfo.assetPath):lodInfo.obj;
        GameObject initObj = Instantiate(obj);
        Transform child = initObj.transform;
        if(parentT.position==Vector3.zero) parentT.position = child.position;
        child.parent = parentT;
        return GetLODByRenders(percent, child);
    }
    private LOD GetLODByRenders(float percent,Transform obj)
    {
        Renderer[] meshGroup = obj.GetComponentsInChildren<Renderer>();
        LOD lod = new LOD(percent, meshGroup);
        return lod;
    }
    /// <summary>
    /// 创建预设导出文件夹
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    private bool CreateExportDirectory(string fullPath)
    {
        if (Directory.Exists(fullPath))
        {
            return true;
        }
        else
        {
            try
            {
                DirectoryInfo info = Directory.CreateDirectory(fullPath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("CreateExportDirectory.Exception:" + e.ToString());
                return false;
            }
        }
    }

    /// <summary>
    /// 清除之前的缓存信息
    /// </summary>
    private void ClearInfo()
    {
        allModelInfoes = null;
        if(fileChooseDic!=null) fileChooseDic.Clear();
        sceneModelInfoes = null;
        projectModelInfoes = null;
        expandLODGroupPart = false;
        LodGroupSingle = null;
        fliterString = "";
        searchingLODSwitch = true;
    }
    /// <summary>
    /// 重设所有值
    /// </summary>
    private void ResetValue()
    {
        isSearchBtnClick = false;
        allModelInfoes = null;
        if (fileChooseDic != null) fileChooseDic.Clear();
        isLod0Select = false;
        isLod1Select = false;
        isLod2Select = false;
        isLod3Select = false;
        isLod4Select = false;
        lodExportPath = loadExportDefault;
        expandLODGroupPart = false;
        searchingLODSwitch = true;
        fliterString = "";
        //AssetDatabase.Refresh();
        //Repaint();
    }
    /// <summary>
    /// 勾选后，保存到缓存中
    /// </summary>
    /// <param name="dirT"></param>
    /// <param name="isOn"></param>
    private void SaveToggleInfo(ModelDetailInfo dirT,bool isOn)
    {
        if(isOn)
        {
            if (!fileChooseDic.ContainsKey(dirT.DirctoryName)) fileChooseDic.Add(dirT.DirctoryName,dirT);
        }
        else
        {
            if(fileChooseDic.ContainsKey(dirT.DirctoryName)) fileChooseDic.Remove(dirT.DirctoryName);
        }
    }
    /// <summary>
    /// 根据下拉框类型，返回对应物体信息
    /// </summary>
    /// <param name="lod"></param>
    /// <returns></returns>
    private string GetLodInfoByType(LodInfo lod)
    {
        if (lod == null) return "0";
        else
        {
            if (modelType == ModelType.size)
            {
                float size = (float)Math.Round(lod.size / 1048576, 2);
                float percent = (float)Math.Round(lod.sizePercent * 100, 1);
                return string.Format("{0}MB ({1}%)", size.ToString(), percent);
            }
            else if (modelType == ModelType.vertex)
            {
                float percent = (float)Math.Round(lod.vertexPercent * 100, 1);
                return string.Format("{0}个 ({1}%)", lod.vertexCount, (int)percent);
            }
            else if (modelType == ModelType.childCount)
            {
                float percent = (float)Math.Round(lod.childPercent * 100, 1);
                return string.Format("{0}个 ({1}%)", lod.childCount, percent);
            }
            else return "0";
        }
    }
    /// <summary>
    /// 排序部分（Name、Size、Mesh、Child）
    /// </summary>
    private void SortList()
    {
        if (allModelInfoes == null || allModelInfoes.Count == 0) return;
        if(isSortByName)
        {
            allModelInfoes.Sort((a, b) =>
            {
                if (a.lod0 == null || b.lod0 == null||a.lod0.obj==null||b.lod0.obj==null) return 0;
                else return b.lod0.obj.name.CompareTo(a.lod0.obj.name);
            });
        }
        else if (modelType == ModelType.size)
        {
            allModelInfoes.Sort((a,b)=> 
            {
                if (a.lod0 == null || b.lod0 == null) return 0;
                else return b.lod0.size.CompareTo(a.lod0.size);
            });
        }
        else if (modelType == ModelType.vertex)
        {
            allModelInfoes.Sort((a, b) =>
            {
                if (a.lod0 == null || b.lod0 == null) return 0;
                else return b.lod0.vertexCount.CompareTo(a.lod0.vertexCount);
            });
        }
        else if (modelType == ModelType.childCount)
        {
            allModelInfoes.Sort((a, b) =>
            {
                if (a.lod0 == null || b.lod0 == null) return 0;
                else return b.lod0.childCount.CompareTo(a.lod0.childCount);
            });
        }
    }
    /// <summary>
    /// 获取文件下，所有子文件夹的模型信息
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <returns></returns>
    private List<ModelDetailInfo> GetModelFiles(string directoryPath)
    {
        if (string.IsNullOrEmpty(projectDirctoryPath)) projectDirctoryPath = Application.dataPath.Replace("/Assets", "").Replace("/",@"\")+@"\";
        if (Directory.Exists(directoryPath))
        {
            List<ModelDetailInfo> models = new List<ModelDetailInfo>();
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            DirectoryInfo[]childDirs = dir.GetDirectories();
            foreach(var item in childDirs)
            {
                ModelDetailInfo md = GetDirctoryModels(item);
                if (md != null) models.Add(md);
            }
            return models;
        }
        else
        {
            return null;
        }
    }

    private static string FbxExtension=".fbx";
    private static string Lod0 = "lod0";
    private static string Lod1 = "lod1";
    private static string Lod2 = "lod2";
    private static string Lod3 = "lod3";
    private static string Lod4 = "lod4";
    private static string lod0h = "0_h";
    private static string lod1h = "1_h";
    private static string lod2h = "2_h";
    private static string lod3h = "3_h";
    private static string lod4h = "4_h";
    private static float mbSize = 1048576;//大于xxMb的文件不加载模型
    /// <summary>
    /// 获取文件夹下的Lod模型文件
    /// </summary>
    /// <param name="dirInfo"></param>
    /// <returns></returns>
    private ModelDetailInfo GetDirctoryModels(DirectoryInfo dirInfo,bool isLimtSize=true)
    {
        DateTime record = DateTime.Now;
        string log = "";
        FileInfo[]childs = dirInfo.GetFiles();
        if(childs!=null&&childs.Length>0)
        {
            ModelDetailInfo mdInfo = new ModelDetailInfo();
            mdInfo.DirctoryName = dirInfo.Name;
            mdInfo.diretcoty = dirInfo;
            float maxSize = fileSizeLimit * mbSize;
            bool maxThanLimit = isLimtSize&&isFileMaxThanLimt(childs);
            foreach (var item in childs)
            {               
                if (item.Extension.ToLower().EndsWith(FbxExtension))
                {                    
                    string assetPath = item.FullName.Replace(projectDirctoryPath, "");
                    GameObject obj=null;
                    if(!maxThanLimit)
                    {
                        obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                        log += string.Format("{0} LoadAsset cost:{1}s",item.Name,(DateTime.Now-record).TotalSeconds);
                        record = DateTime.Now;
                    }
                    string nameLower = item.Name.ToLower().Replace(FbxExtension,"");
                    if (nameLower.EndsWith(Lod0)) mdInfo.lod0 = GetFileLODInfo(item, obj, assetPath);
                    else if (nameLower.EndsWith(lod0h)) mdInfo.lod0_H = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);

                    else if (nameLower.EndsWith(Lod1)) mdInfo.lod1 = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);
                    else if (nameLower.EndsWith(lod1h)) mdInfo.lod1_H = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);

                    else if (nameLower.EndsWith(Lod2)) mdInfo.lod2 = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);
                    else if (nameLower.EndsWith(lod2h)) mdInfo.lod2_H = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);

                    else if (nameLower.EndsWith(Lod3)) mdInfo.lod3 = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);
                    else if (nameLower.EndsWith(lod3h)) mdInfo.lod3_H = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);

                    else if (nameLower.EndsWith(Lod4)) mdInfo.lod4 = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);
                    else if (nameLower.EndsWith(lod4h)) mdInfo.lod4_H = GetFileLODInfo(item, obj, assetPath, mdInfo.lod0);
                    Resources.UnloadUnusedAssets();
                }
            }
            Debug.Log(log);
            if (mdInfo.lod0!=null||mdInfo.lod0_H!=null||mdInfo.lod1!=null||mdInfo.lod2!=null||mdInfo.lod3!=null||mdInfo.lod4!=null) return mdInfo;
            else return null;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 是否文件下，有超大文件
    /// </summary>
    /// <param name="childs"></param>
    /// <returns></returns>
    private bool isFileMaxThanLimt(FileInfo[] childs)
    {
        float maxSize = fileSizeLimit * mbSize;
        foreach (var item in childs)
        {
            if (item.Length > maxSize) return true;
        }
        return false;
    }

    private LodInfo GetFileLODInfo(FileInfo fileT,GameObject objT,string assetPathT,LodInfo maxLodInfo=null)
    {
        LodInfo lod = new LodInfo();
        if(fileT!=null)
        {
            lod.fileInfo = fileT;
            lod.size = fileT == null ? 0 : fileT.Length;            
        }
        else if(objT!=null)
        {
            string locationPath = Application.dataPath.Replace("Assets", "");
            string selfPath = "";
            //regular==>场景中的普通物体，不是预设  model=>预设
            if (PrefabUtility.GetPrefabAssetType(objT) == PrefabAssetType.Model)
            {                
                //PrefabUtility.GetPrefabParent
                GameObject pfbInProject = PrefabUtility.GetCorrespondingObjectFromSource(objT);               
                selfPath = AssetDatabase.GetAssetPath(pfbInProject);
               
            }else 
            {
                MeshFilter mesh = objT.GetComponentInChildren<MeshFilter>(true);//找到第一个Mesh,根据引用找到模型所在位置
                if (mesh!=null&&mesh.sharedMesh != null)
                {
                    selfPath = AssetDatabase.GetAssetPath(mesh.sharedMesh);
                }
            }

            string pfbFullPath = locationPath + selfPath;
            if (File.Exists(pfbFullPath))
            {
                lod.fileInfo = new FileInfo(pfbFullPath);
                lod.size = lod.fileInfo == null ? 0 : lod.fileInfo.Length;
            }
            else
            {
                lod.size = 0;
            }
            //EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(objT)); 获取内存中占用大小  可能和实际大小不一致            
        }
        else
        {
            lod.size = 0;
        }
        lod.sizePercent = maxLodInfo == null || maxLodInfo.size == 0 ? 1 : lod.size / maxLodInfo.size;

        lod.assetPath = assetPathT;
        lod.obj = objT;
        if(objT!=null)
        {
            MeshFilter[] mGroups = objT.GetComponentsInChildren<MeshFilter>(true);
            int vertexCount = 0;
            foreach (var item in mGroups)
            {
                //Debug.Log("Triangle:" + item.sharedMesh.triangles.Length + " Verticals" + item.sharedMesh.vertexCount);210  72
                if (item != null && item.sharedMesh != null)
                {
                    vertexCount += item.sharedMesh.vertexCount;
                }
            }
            lod.vertexCount = vertexCount;
            lod.vertexPercent = maxLodInfo == null || maxLodInfo.vertexCount == 0 ? 1 : lod.vertexCount / maxLodInfo.vertexCount;

            lod.childCount = mGroups == null ? 0 : mGroups.Length;//实际要统计的，是带有mesh的物体
            lod.childPercent = maxLodInfo == null || maxLodInfo.childCount == 0 ? 1 : (float)lod.childCount / maxLodInfo.childCount;
        }
        else
        {
            lod.vertexCount = 0;
            lod.vertexPercent = 0;
            lod.childCount = 0;
            lod.childPercent = 0;
        }    
        return lod;
    }
    #endregion
    #region HelperMethod
    /// <summary>
    /// 根据文件长度，获取文件大小信息
    /// </summary>
    /// <param name="lengthOfDocument"></param>
    /// <returns></returns>
    static string GetLength(long lengthOfDocument)
    {

        if (lengthOfDocument < 1024)
            return string.Format(lengthOfDocument.ToString() + 'B');
        else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
            return string.Format(Math.Round(lengthOfDocument / 1024.0,2).ToString() + "KB");
        else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
            return string.Format(Math.Round(lengthOfDocument / 1024.0 / 1024.0,2).ToString() + "M");
        else
            return string.Format(Math.Round(lengthOfDocument / 1024.0 / 1024.0 / 1024.0,2).ToString() + "GB");
    }
    /// <summary>
    /// 获取文件夹下，第一个模型信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private LodInfo GetObjectByModelInfo(ModelDetailInfo info)
    {
        if (info.lod0 != null && info.lod0.obj != null) return info.lod0;
        else if (info.lod0_H != null && info.lod0_H.obj != null) return info.lod0_H;

        else if (info.lod1 != null && info.lod1.obj != null) return info.lod1;
        else if (info.lod1_H != null && info.lod1_H.obj != null) return info.lod1_H;

        else if (info.lod2 != null && info.lod2.obj != null) return info.lod2;
        else if (info.lod2_H != null && info.lod2_H.obj != null) return info.lod2_H;

        else if (info.lod3 != null && info.lod3.obj != null) return info.lod3;
        else if (info.lod3_H != null && info.lod3_H.obj != null) return info.lod3_H;

        else if (info.lod4 != null && info.lod4.obj != null) return info.lod4;
        else if (info.lod4_H != null && info.lod4_H.obj != null) return info.lod4_H;
        else return null;
    }
    /// <summary>
    /// 重新加载文件夹下模型信息
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private ModelDetailInfo ReLoadModelInfo(ref ModelDetailInfo info)
    {
        info = GetDirctoryModels(info.diretcoty,false);
        return info;
    }

    public void SelectDirctory(DirectoryInfo directory)
    {
        if (directory == null) return;
        string assetPath = directory.FullName.Replace(projectDirctoryPath, "");
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (obj == null)
            return;
        EditorGUIUtility.PingObject(obj);
        Selection.activeObject = obj;
    }
    #endregion
}
