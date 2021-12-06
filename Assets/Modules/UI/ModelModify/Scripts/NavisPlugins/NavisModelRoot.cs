using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Base.Common;
using System.Text;

public class NavisModelRoot : MonoBehaviour
{
    public string ModelName = "";

    [NonSerialized]
    public ModelItemInfo ModelRoot;








    public List<Transform> transformList = new List<Transform>();

    public List<GameObject> RootNodes = new List<GameObject>();

    public List<BIMModelInfo> bimInfos = new List<BIMModelInfo>();

    //public Dictionary<string,List<BIMModelInfo>> bimAreas = new Dictionary<string, List<BIMModelInfo>>();

    //private Dictionary<string, BIMModelInfo> rendererId2Bim = new Dictionary<string, BIMModelInfo>();

    //private Dictionary<string, BIMModelInfo> guid2Bim = new Dictionary<string, BIMModelInfo>();

    public string TestModelName = "H¼¶Ö÷±ä1";

    [ContextMenu("TestFindModelByName")]
    public void TestFindModelByName()
    {
        var list= TransformHelper.FindSameNameList(transformList, TestModelName);
        Debug.Log($"TestFindModelByName name:{TestModelName} list:{list.Count}");
    }

    public BIMModelInfoDictionary BimDict = new BIMModelInfoDictionary();

    public List<BIMModelInfo> GetTargetBIMs()
    {
        List<BIMModelInfo> list = new List<BIMModelInfo>();
        foreach (var target in Targets)
        {
            if (target == null) continue;
            list.AddRange(target.GetComponentsInChildren<BIMModelInfo>(true));
        }
        return list;
    }

    [ContextMenu("GetBims")]
    public void GetBims(List<ModelItemInfo> models,ProgressArgEx p0)
    {
        BimDict = new BIMModelInfoDictionary(GetTargetBIMs(), p0);
        bimInfos = BimDict.bimInfos;

        //var models = file.GetAllItems();
        BimDict.CheckDict(models);

        if (p0 == null)
        {
            ProgressBarHelper.ClearProgressBar();
        }
    }

    NavisFileInfo navisFile;

    public void SetOnlySelfModel()
    {
        navisFile = InitNavisFileInfoByModel.GetNavisFileInfoEx();
        ModelRoot = navisFile.Models.Find(i => i.Name == ModelName);
        navisFile.Models.Clear();
        navisFile.Models.Add(ModelRoot);
        SaveXml();
    }

    public ModelItemInfoDictionary ModelDict = new ModelItemInfoDictionary();

    public TransformDictionary TransformDict = new TransformDictionary();

    public bool includeInactive = true;

    //public List<string> OnlyFBXFiles = new List<string>();

    //public List<string> ExceptionFBXFiles = new List<string>();

    public List<GameObject> Targets = new List<GameObject>();

    public GameObject TargetRoot = new GameObject();

    [ContextMenu("FindRelativeTargets")]
    public void FindRelativeTargets()
    {
        string key = ModelName + "_";
        var list=TransformHelper.FindGameObjects(TargetRoot.transform, key);
        foreach(var item in list)
        {
            if (!Targets.Contains(item))
            {
                Targets.Add(item);
            }
        }
    }

    [ContextMenu("HidePipes")]
    public void HidePipes()
    {
        var list = TransformHelper.FindGameObjects(this.transform,new List<string>() {"HH_","JG_","SG_","JQ_" });
        foreach (var item in list)
        {
            item.SetActive(false);
        }
    }

    [ContextMenu("ShowPipes")]
    public void ShowPipes()
    {
        var list = TransformHelper.FindGameObjects(this.transform, new List<string>() { "HH_", "JG_", "SG_", "JQ_" });
        foreach (var item in list)
        {
            item.SetActive(true);
        }
    }

    private void GetTransformList(ProgressArgEx p0)
    {
        if (!Targets.Contains(this.gameObject))
        {
            Targets.Add(this.gameObject);
        }

        Dictionary<Transform, Transform> transformDict = new Dictionary<Transform, Transform>();
        foreach (var target in Targets)
        {
            var list = target.GetComponentsInChildren<Transform>(includeInactive).ToList();
            foreach(var item in list)
            {
                if (item.gameObject.activeInHierarchy == false && includeInactive == false) continue;
                if (!transformDict.ContainsKey(item))
                {
                    //transformList.Add(item);
                    transformDict.Add(item, item);
                }
            }
            var combinedList = TransformHelper.FindAllTransforms(target.transform, "_Combined");
            foreach(var item in combinedList)
            {
                if (transformDict.ContainsKey(item))
                {
                    transformDict.Remove(item);
                }
            }
            Debug.Log($"GetTransformList target:{target} list:{list.Count} combinedList:{combinedList.Count} transformList:{transformList.Count} includeInactive:{includeInactive}");
            //transformList.AddRange(list);
        }
        transformList = transformDict.Keys.ToList();
        //transformList = this.GetComponentsInChildren<Transform>(true).ToList();
        transformList.Remove(this.transform);

        Debug.Log($"GetTransformList transformList1:{transformList.Count}");

        transformList = InitNavisFileInfoByModel.Instance.FilterList(transformList,p0);
        transformList.Sort((a, b) => { return a.name.CompareTo(b.name); });

        Debug.Log($"GetTransformList transformList2:{transformList.Count}");

        TransformDict = new TransformDictionary(transformList);
    }

    private bool GetModelRoot()
    {
        navisFile = InitNavisFileInfoByModel.GetNavisFileInfoEx();

        ModelRoot = navisFile.Models.Find(i => i.Name == ModelName);
        if (ModelRoot == null)
        {
            ModelRoot = navisFile.Models.Find(i => i.Name.Contains(ModelName));
            Debug.Log($"Model == null 1 ModelName:{ModelName} Model:{ModelRoot} Models:{navisFile.Models.Count}");
            //return;
        }

        if (ModelRoot == null)
        {
            //Model = navisFile.Models.Find(i => i.Name.Contains(ModelName));
            Debug.LogError($"Model == null ModelName:{ModelName} Model:{ModelRoot} Models:{navisFile.Models.Count}");
            ProgressBarHelper.ClearProgressBar();
            return false;
        }
        return true;
    }

    //private void GetModelLists()
    //{
    //    allModels = ModelRoot.GetAllItems();

    //    allModels_drawable_nozero.Clear();
    //    allModels_drawable_zero.Clear();
    //    allModels_noDrawable_nozero.Clear();
    //    allModels_noDrawable_zero.Clear();

    //    //allModels.Clear();
    //    allModels_uid.Clear();
    //    allModels_noUid.Clear();

    //    for (int i = 0; i < allModels.Count; i++)
    //    {
    //        ModelItemInfo child = allModels[i];
    //        //if (child.IsZero())
    //        //{
    //        //    if (child.Drawable == false)
    //        //    {
    //        //        allModels_zero.Add(child);
    //        //    }
    //        //    else
    //        //    {
    //        //        allModels_drawable.Add(child);
    //        //    }
    //        //}
    //        //else
    //        //{
    //        //    if (child.Drawable == false)
    //        //    {
    //        //        allModels_noDrawable.Add(child);
    //        //    }
    //        //    else
    //        //    {

    //        //    }
    //        //}

    //        if (child.Drawable == true)
    //        {
    //            //allModels_drawable.Add(child);
    //            if (child.IsZero())
    //            {
    //                allModels_drawable_zero.Add(child);
    //            }
    //            else
    //            {
    //                allModels_drawable_nozero.Add(child);
    //            }
    //        }
    //        else
    //        {
    //            if (child.IsZero())
    //            {
    //                allModels_noDrawable_zero.Add(child);
    //            }
    //            else
    //            {
    //                allModels_noDrawable_nozero.Add(child);
    //            }
    //        }

    //        if (!string.IsNullOrEmpty(child.UId))
    //        {
    //            allModels_uid.Add(child);
    //        }
    //        else
    //        {
    //            allModels_noUid.Add(child);
    //        }
    //    }

    //    allModels.Sort();
    //    allModels_uid.Sort();
    //    allModels_noUid.Sort();

    //    allModels_drawable_zero.Sort();
    //    allModels_drawable_nozero.Sort();
    //    allModels_noDrawable_nozero.Sort();
    //    allModels_noDrawable_zero.Sort();
    //}

    [ContextMenu("BindBimInfo")]
    public void BindBimInfo(ProgressArg p0)
    {
        DateTime start = DateTime.Now;

        var p1 = ProgressArg.New("BindBimInfo", 0, 3, "LoadModels", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p1,true);
        LoadModels(p1);

        var p2 = ProgressArg.New("BindBimInfo", 1, 3, "FindObjectByUID", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p2, true);
        FindObjectByUID();

        var p3 = ProgressArg.New("BindBimInfo", 2, 3, "FindObjectByPos", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p3, true);
        FindObjectByPos(p3);

        Debug.LogError($"[{this.name}]BindBimInfo time:{DateTime.Now-start}");

        var p4 = ProgressArg.New("BindBimInfo", 3, 3, this.name, p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p4, true);

        if (p0 == null)
        {
            ProgressBarHelper.ClearProgressBar();
        }
    }

    [ContextMenu("LoadModels")]
    public void LoadModels(IProgressArg p0)
    {
        DateTime start = DateTime.Now;

        ClearResult();

        if (string.IsNullOrEmpty(ModelName))
        {
            ModelName = this.name;
        }

        bool enableProgress = true;

        //1
        var p1 = ProgressArg.New("LoadModels", 0, 5, "GetModelRoot", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p1, enableProgress);
        if (GetModelRoot() == false) return;

        var p2 = ProgressArg.New("LoadModels", 1, 5, "GetTransformList", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p2, enableProgress);
        //2
        GetTransformList(p2);

        var p3 = ProgressArg.New("LoadModels", 2, 5, "ModelItemInfoDictionary", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p3, enableProgress);
        //3
        //var allModels = navisFile.GetAllItems();
        var allModels = navisFile.GetAllModelInfos();
        var currentModels = ModelRoot.GetChildrenModels();
        ModelDict = new ModelItemInfoDictionary(currentModels, p3);

        var p4 = ProgressArg.New("LoadModels", 3, 5, "GetBims", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p4, enableProgress);
        //4
        GetBims(allModels,p4);//2.BIMinfo

        //GetModelLists();//3.ModelInfo

        var p5 = ProgressArg.New("LoadModels", 4, 5, "ModelItemInfoListEx", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p5, enableProgress);
        //5
        ModelList = new ModelItemInfoListEx(currentModels);

        var p6 = ProgressArg.New("LoadModels", 5, 5, "ModelItemInfoListEx", p0);
        ProgressBarHelper.DisplayCancelableProgressBar(p6, enableProgress);

        if(p0==null)
            ProgressBarHelper.ClearProgressBar();

        Debug.Log($"[{this.name}][LoadModels] time:{DateTime.Now - start} rendererIdCount:{ModelDict.rendererIdCount} UidCount:{ModelDict.UidCount}");
    }

    public ModelItemInfoListEx ModelList = null;

    public string TestUIdString = "0027-20014-345476504420876800";

    [ContextMenu("TestIsUId")]
    public void TestIsUId()
    {
        //string s = "0027-20014-345476504420876800";
        string s = TestUIdString;
        bool isUid= TransformDictionary.IsUID(s);
        Debug.Log($"[{this.name}]TestIsUId s:{s} isUid:{isUid} length:{s.Length}");
    }

    [ContextMenu("FindObjectByUID")]
    public void TestFindObjectByUID()
    {
        //string s = "0027-20014-345476504420876800";
        string s = TestUIdString;
        var transform = TransformDict.FindObjectByUID(s);
        Debug.Log($"[{this.name}]TestIsUId s:{s} transform:{transform}");
    }

    public float GetMinDistance()
    {
        //if (MinDistance == 0)
        //{
        //    MinDistance = DefaultMinDinstance;
        //}
        if (MinDistance < DefaultMinDinstance)
        {
            MinDistance = DefaultMinDinstance;
        }
        return MinDistance;
    }


    public void FindObjectByUID()
    {
        GetMinDistance();
        if (ModelList == null)
        {
            Debug.LogError($"FindObjectByUID ModelList == null");
            return;
        }
        DateTime start = DateTime.Now;
        List<ModelItemInfo> allModels_uid_found = new List<ModelItemInfo>();
        List<ModelItemInfo> allModels_uid_nofound = new List<ModelItemInfo>();

        foreach (var uidModel in ModelList.allModels_uid)
        {
            var transform=TransformDict.FindObjectByUID(uidModel.UId);
            if (transform != null)
            {
                float dis = uidModel.GetDistance(transform);
                if (dis > MinDistance )
                {
                    allModels_uid_nofound.Add(uidModel);
                    
                    Debug.LogError($"[FindObjectByUID][{dis}][{MinDistance}]{uidModel.ShowDistance(transform)}");
                }
                else
                {
                    allModels_uid_found.Add(uidModel);
                    TransformDict.RemoveTransform(transform);
                    BIMModelInfo.SetModelInfo(transform, uidModel);
                }
            }
            else
            {
                allModels_uid_nofound.Add(uidModel);
            }
        }

        TransformDict.InitDict();

        Debug.LogError($"[{this.name}][FindObjectByUID] time:{DateTime.Now-start} allModels_uid:{ModelList.allModels_uid.Count},found:{allModels_uid_found.Count} nofound:{allModels_uid_nofound.Count}");

        //allModels_uid = allModels_uid_nofound;

        ModelList.SetList(allModels_uid_nofound);
    }

    public static float DefaultMinDinstance = 0.0002f;

    public float MinDistance = DefaultMinDinstance;

    public void FindObjectByModel(ModelItemInfo model1)
    {
        GetMinDistance();

        DateTime start = DateTime.Now;



        List<ModelItemInfo> models1 = new List<ModelItemInfo>();
        models1.AddRange(ModelList.allModels_drawable_nozero);
        //models.AddRange(ModelList.allModels_noDrawable_nozero);

        //var p01 = ProgressArg.New("FindObjectByPos", 0, 2, "ModelItemInfoDictionary", p0);
        ModelItemInfoDictionary modelDict = new ModelItemInfoDictionary(models1, null);
        //var modelDict = new ModelItemInfoDictionary(models, null);

        Model2TransformResult result = new Model2TransformResult(models1, modelDict, TransformDict, MinDistance);

        //var p02 = ProgressArg.New("FindObjectByPos", 1, 2, "FindModels", p0);
        //for (int i = 0; i < models1.Count; i++)
        {
            //ModelItemInfo model1 = models1[i];
            //var p1 = ProgressArg.New("FindModels", i, models1.Count, model1.Name, p02);
            //ProgressBarHelper.DisplayCancelableProgressBar(p1);

            //var transform = TransformDict.FindObjectByPos(uidModel);
            List<Transform> transforms1 = TransformDict.FindModelsByPosAndName(model1);
            result.CheckResult(model1, transforms1);
        }
        result.SetModelList(ModelList);
        Debug.LogError($"[{this.name}][FindObjectByPos] time:{DateTime.Now - start} allModels_uid:{ModelList.allModels_uid.Count}," + result.ToString());
        //if (p0 == null)
        {
            ProgressBarHelper.ClearProgressBar();
        }
    }

    public void FindObjectByPos(ProgressArgEx p0)
    {
        GetMinDistance();

        DateTime start = DateTime.Now;
        List<ModelItemInfo> models1 = new List<ModelItemInfo>();
        models1.AddRange(ModelList.allModels_drawable_nozero);

        //models1.AddRange(ModelList.allModels_noDrawable_nozero);

        var p01 = ProgressArg.New("FindObjectByPos", 0,2, "ModelItemInfoDictionary",p0);
        ModelItemInfoDictionary modelDict = new ModelItemInfoDictionary(models1, p01);
        //var modelDict = new ModelItemInfoDictionary(models, null);

        Model2TransformResult result = new Model2TransformResult(models1, modelDict, TransformDict, MinDistance);

        var p02 = ProgressArg.New("FindObjectByPos", 1, 2, "FindModels", p0);
        for (int i = 0; i < models1.Count; i++)
        {
            ModelItemInfo model1 = models1[i];
            var p1 = ProgressArg.New("FindModels", i, models1.Count, model1.Name, p02);
            ProgressBarHelper.DisplayCancelableProgressBar(p1);

            //var transform = TransformDict.FindObjectByPos(uidModel);
            List<Transform> transforms1 = TransformDict.FindModelsByPosAndName(model1);
            result.CheckResult(model1,transforms1);
        }
        result.SetModelList(ModelList);
        Debug.LogError($"[{this.name}][FindObjectByPos] time:{DateTime.Now - start} allModels_uid:{ModelList.allModels_uid.Count}," + result.ToString());
        if (p0 == null)
        {
            ProgressBarHelper.ClearProgressBar();
        }
    }

    public void FindObjectByPos2(ProgressArgEx p0)
    {
        GetMinDistance();

        DateTime start = DateTime.Now;
        List<ModelItemInfo> models1 = new List<ModelItemInfo>();
        //models1.AddRange(ModelList.allModels_drawable_nozero);

        models1.AddRange(ModelList.allModels_noDrawable_nozero);

        var p01 = ProgressArg.New("FindObjectByPos", 0, 2, "ModelItemInfoDictionary", p0);
        ModelItemInfoDictionary modelDict = new ModelItemInfoDictionary(models1, p01);
        //var modelDict = new ModelItemInfoDictionary(models, null);

        Model2TransformResult result = new Model2TransformResult(models1, modelDict, TransformDict, MinDistance);

        var p02 = ProgressArg.New("FindObjectByPos", 1, 2, "FindModels", p0);
        for (int i = 0; i < models1.Count; i++)
        {
            ModelItemInfo model1 = models1[i];
            var p1 = ProgressArg.New("FindModels", i, models1.Count, model1.Name, p02);
            ProgressBarHelper.DisplayCancelableProgressBar(p1);

            //var transform = TransformDict.FindObjectByPos(uidModel);
            List<Transform> transforms1 = TransformDict.FindModelsByPosAndName(model1);
            result.CheckResult(model1, transforms1);
        }
        result.SetModelList(ModelList);
        Debug.LogError($"[{this.name}][FindObjectByPos] time:{DateTime.Now - start} allModels_uid:{ModelList.allModels_uid.Count}," + result.ToString());
        if (p0 == null)
        {
            ProgressBarHelper.ClearProgressBar();
        }
    }

    public void RemoveRepeated()
    {
        this.ModelDict.RemoveRepeatedModelInfo(this.BimDict);
    }


    public void ClearRendererId()
    {
        //this.ModelDict.ClearRendererId();

        int count=navisFile.ClearRendererId();
        Debug.LogError($"ClearRendererId count:{count}");
    }

    [ContextMenu("CreateTree")]
    public void CreateTree()
    {
        DateTime start = DateTime.Now;

        ClearResult();

        TreeNodeCount = 0;
        TreeNodeIndex = 0;
        InitCreateTree(ModelRoot);
        Debug.LogError($"CreateTree TreeNodeCount:{TreeNodeCount}");

        RootNodes =CreateTree(ModelRoot, this.transform);

        //List<ModelItemInfo> bimInfos02 = new List<ModelItemInfo>();
        for (int i = 0; i < noFoundBimInfos01.Count; i++)
        {
            ModelItemInfo child = noFoundBimInfos01[i];
            GameObject go = child.Tag as GameObject;

            ProgressArg p1 = new ProgressArg("CreateTree2", i, noFoundBimInfos01.Count, child.Name);
            InitNavisFileInfoByModel.Instance.progressArg = p1;

            if (InitNavisFileInfoByModel.Instance.IsProgressBreak)
            {
                return;
                //break;
            }
            if (FindModelGameObjectEx(child, go, false) == false)
            {
                if (child.X == 0 && child.Z == 0 && child.Y == 0)
                {
                    go.name = "[*Model*31] " + child.Name;
                    noFoundBimInfos02.Add(child);
                    //Debug.LogError($"[Model Not Found 3] {child.ToString()}");
                }
                else
                {
                    go.name = "[*Model*32] " + child.Name;
                    noFoundBimInfos03.Add(child);
                    //Debug.LogError($"[Model Not Found 3] {child.ToString()}");
                }
                
            }
            else
            {
                //noFoundBimInfos02.Add(child);
                noFoundBimInfos01.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < ModelList.allModels_noDrawable_nozero.Count; i++)
        {
            ModelItemInfo child = ModelList.allModels_noDrawable_nozero[i];
            GameObject go = child.Tag as GameObject;

            ProgressArg p1 = new ProgressArg("CreateTree3", i, ModelList.allModels_noDrawable_nozero.Count, child.Name);
            InitNavisFileInfoByModel.Instance.progressArg = p1;

            if (go == null) continue;
            if (FindModelGameObjectEx(child, go, true) == false)
            {
                go.name = "[*Model*4] " + child.Name;
                noFoundBimInfos04.Add(child);
                //Debug.LogError($"[Model Not Found 4] {child.ToString()}");
            }
        }

        //bimInfos0.Sort();
        foundBimInfos1.Sort();
        foundBimInfos2.Sort();
        foundBimInfos3.Sort();
        //bimInfos0_name.Sort();

        ProgressBarHelper.ClearProgressBar();

        SaveXml();

        Debug.Log($"CreateTree time:{DateTime.Now-start}");
    }

    public void SaveXml()
    {
        SaveNavisFile(navisFile);
    }

    public void SaveNavisFile(NavisFileInfo navisFiles)
    {
        if (navisFiles == null)
        {
            Debug.LogError("SaveNavisFile navisFiles == null");
            return;
        }
        string newModelInfo = "\\..\\NavisFileInfo.xml";
        string pathNew = Application.dataPath + newModelInfo;
        navisFiles.SetPropertiesExist(false);
        SerializeHelper.Save(navisFiles, pathNew);
        Debug.Log($"SaveNavisFile path:{pathNew}");
    }

    [ContextMenu("ClearRootNodes")]
    public void ClearRootNodes()
    {
        foreach (var node in RootNodes)
        {
            if (node == null) continue;
            EditorHelper.UnpackPrefab(node);
            GameObject.DestroyImmediate(node);
        }
        RootNodes.Clear();
    }

    //public void RemoveRepeated()
    //{
    //    StringBuilder sb = new StringBuilder();
    //    foreach(var rendererId in repeatModelsByRendererId.Keys)
    //    {
    //        var models = repeatModelsByRendererId[rendererId];
    //        var bim = GetBIMModelByRendererId(rendererId);
    //        if (bim != null)
    //        {
    //            var closedModel = bim.FindClosedModel(models);
    //            bim.SetModelInfo(closedModel);
    //            models.Remove(closedModel);
    //        }
    //        for (int i = 0; i < models.Count; i++)
    //        {
    //            ModelItemInfo model = models[i];
    //            model.RenderId = "";
    //            model.RenderName = "";
    //            model.AreaName = "";
    //            sb.AppendLine($"id:{rendererId} model:{model.Name} uid:{model.UId}");

    //            if (i > 0 && i % 100 == 0)
    //            {
    //                Debug.LogError($"RemoveRepeated: "+ sb);
    //                sb = new StringBuilder();
    //            }
    //        }
    //    }
    //    SaveXml();
    //}

    [ContextMenu("ClearResult")]
    public void ClearResult()
    {
        ClearRootNodes();
        //noFoundBimInfos01.Clear();
        //noFoundBimInfos02.Clear();
        //noFoundBimInfos03.Clear();
        //noFoundBimInfos04.Clear();

        //foundBimInfos1.Clear();
        //foundBimInfos2.Clear();
        //foundBimInfos3.Clear();

        noFoundBimInfos01 = new List<ModelItemInfo>();
        noFoundBimInfos02 = new List<ModelItemInfo>();
        noFoundBimInfos03 = new List<ModelItemInfo>();
        noFoundBimInfos04 = new List<ModelItemInfo>();
        foundBimInfos1 = new List<BIMModelInfo>();
        foundBimInfos2 = new List<BIMModelInfo>();
        foundBimInfos3 = new List<BIMModelInfo>();

        transformList.Clear();
    }

    //public List<GameObject> bimModels = new List<GameObject>();
    //public List<GameObject> bimInfos0_name = new List<GameObject>();
    [NonSerialized]
    public List<ModelItemInfo> noFoundBimInfos01 = new List<ModelItemInfo>();
    [NonSerialized]
    public List<ModelItemInfo> noFoundBimInfos02 = new List<ModelItemInfo>();
    [NonSerialized]
    public List<ModelItemInfo> noFoundBimInfos03 = new List<ModelItemInfo>();
    [NonSerialized]
    public List<ModelItemInfo> noFoundBimInfos04 = new List<ModelItemInfo>();
    [NonSerialized]
    public List<BIMModelInfo> foundBimInfos1 = new List<BIMModelInfo>();
    [NonSerialized]
    public List<BIMModelInfo> foundBimInfos2 = new List<BIMModelInfo>();
    [NonSerialized]
    public List<BIMModelInfo> foundBimInfos3 = new List<BIMModelInfo>();

    private int TreeNodeCount = 0;

    private int TreeNodeIndex = 0;

    private void InitCreateTree(ModelItemInfo rootModel)
    {
        if (rootModel == null)
        {
            Debug.LogError("InitCreateTree rootModel == null");
            return;
        }
        if(rootModel.Children!=null)
        foreach (ModelItemInfo child in rootModel.Children)
        {
            TreeNodeCount++;
            InitCreateTree(child);
        }
    }

    public List<GameObject> CreateTree(ModelItemInfo rootModel,Transform parent)
    {
        List<GameObject> goList = new List<GameObject>();
        if(rootModel!=null && rootModel.Children!=null)
        for (int i = 0; i < rootModel.Children.Count; i++)
        {
            ModelItemInfo child = (ModelItemInfo)rootModel.Children[i];
            child.Name = child.Name.Replace(" ", "_");
            TreeNodeIndex++;

            ProgressArg p1 = new ProgressArg("CreateTree", TreeNodeIndex, TreeNodeCount, child.Name);
            InitNavisFileInfoByModel.Instance.progressArg = p1;
                //if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                //{
                //    break;
                //}

                GameObject go = new GameObject($"{child.Name}");
            var pos = new Vector3(child.X, child.Z, child.Y);
            go.transform.position = pos;
            go.transform.SetParent(parent);
            goList.Add(go);

            CreateTree(child, go.transform);

            if (child.Drawable == false)
            {
                go.name = "[Group1] " + child.Name;
            }
            else if (child.X==0 && child.Z==0 && child.Y == 0)
            {
                if (child.Drawable)
                {
                    go.name = "[Model] " + child.Name;
                    if (FindModelGameObjectEx(child, go, true) == false)
                    {
                        go.name = "[*Model*2] " + child.Name;
                        noFoundBimInfos01.Add(child);
                        //Debug.LogError($"[Model Not Found 2] {child.ToString()}");

                        var list = TransformHelper.FindSameNameList(transformList, child.Name);
                        //Debug.LogError($"TestFindModelByName name:{child.Name} list:{list.Count}");
                    }
                }
                else
                {
                    go.name = "[Group2] " + child.Name;
                }
            }
            else
            {
                go.name = "[Model] " + child.Name;
                if(FindModelGameObjectEx(child, go, true) == false)
                {
                    go.name = "[*Model*1] " + child.Name;
                    noFoundBimInfos01.Add(child);
                    //Debug.LogError($"[Model Not Found 1] {child.ToString()}");
                }
            }

            child.Tag = go;
        }

        return goList;
    }

    private bool FindModelGameObjectEx(ModelItemInfo child, GameObject go, bool isSameName)
    {
        BIMModelInfo bim1 = FindModelGameObject1(child, isSameName);
        if (bim1 == null)
        {
            //go.name = "[*Model*] " + child.Name;
            //bimInfos0.Add(child);
            //bimInfos0_name.Add(go);
            //Debug.LogError($"{child.ToString()}");
            return false;
        }
        else if (bim1.IsFound)
        {
            foundBimInfos1.Add(bim1);
        }
        else
        {
            BIMModelInfo bim2 = FindModelGameObject2(child, isSameName);
            if (bim2.IsFound)
            {
                foundBimInfos2.Add(bim2);
            }
            else
            {
                BIMModelInfo bim3 = FindModelGameObject3(child, isSameName);
                if (bim3.IsFound)
                {
                    foundBimInfos3.Add(bim3);
                }
                else
                {
                    //go.name = "[*Model*] " + child.Name;
                    //bimInfos0.Add(child);
                    //bimInfos0_name.Add(go);
                    //Debug.LogError($"{child.ToString()}|bim:{bim3.name}[{bim3.IsFound}]");
                    return false;
                }
            }
        }
        return true;
    }

    //public bool IsSameName = true;

    private BIMModelInfo FindModelGameObject1(ModelItemInfo child,bool isSameName)
    {
        return InitNavisFileInfoByModel.Instance.InitBIMModelInfoByPos_Vue2Model(child, transformList, InitNavisFileInfoByModelSetting.Instance.MinDistance1, isSameName);
    }
    private BIMModelInfo FindModelGameObject2(ModelItemInfo child, bool isSameName)
    {
        return InitNavisFileInfoByModel.Instance.InitBIMModelInfoByPos_Vue2Model(child, transformList, InitNavisFileInfoByModelSetting.Instance.MinDistance2, isSameName);
    }
    private BIMModelInfo FindModelGameObject3(ModelItemInfo child, bool isSameName)
    {
        return InitNavisFileInfoByModel.Instance.InitBIMModelInfoByPos_Vue2Model(child, transformList, InitNavisFileInfoByModelSetting.Instance.MinDistance3, isSameName);
    }
    private BIMModelInfo FindModelGameObject4(ModelItemInfo child, bool isSameName)
    {
        return InitNavisFileInfoByModel.Instance.InitBIMModelInfoByPos_Vue2Model(child, transformList, InitNavisFileInfoByModelSetting.Instance.MinDistance4, isSameName);
    }
}
