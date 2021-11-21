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



    [NonSerialized]
    public List<ModelItemInfo> allModels = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_uid = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_noUid = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_drawable_nozero = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_drawable_zero = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_noDrawable_nozero = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_noDrawable_zero = new List<ModelItemInfo>();


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
        var list=InitNavisFileInfoByModel.FindSameNameList(transformList, TestModelName);
        Debug.Log($"TestFindModelByName name:{TestModelName} list:{list.Count}");
    }

    public BIMModelInfoDictionary BimDict = new BIMModelInfoDictionary();

    [ContextMenu("GetBims")]
    public void GetBims(NavisFileInfo file)
    {
        BimDict = new BIMModelInfoDictionary(this.GetComponentsInChildren<BIMModelInfo>(true));
        bimInfos = BimDict.bimInfos;

        var models = file.GetAllItems();
        BimDict.CheckDict(models);
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

    public List<BIMModelInfo> errorBIMs = new List<BIMModelInfo>();

    public ModelItemInfoDictionary ModelDict = new ModelItemInfoDictionary();

    public TransformDictionary TransformDict = new TransformDictionary();

    [ContextMenu("LoadModels")]
    public void LoadModels()
    {
        DateTime start = DateTime.Now;
        if(string.IsNullOrEmpty(ModelName))
        {
            ModelName = this.name;
        }

        errorBIMs.Clear();


        ClearResult();

        transformList = this.GetComponentsInChildren<Transform>(true).ToList();

        transformList = InitNavisFileInfoByModel.Instance.FilterList(transformList);

        TransformDict = new TransformDictionary(transformList);

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
            return;
        }

        

        
        var all = navisFile.GetAllItems();
        ModelDict = new ModelItemInfoDictionary(all);

        GetBims(navisFile);
        //repeatModelsByRendererId = new Dictionary<string, List<ModelItemInfo>>();
        //foreach (var r in renderId2Model.Keys)
        //{
        //    var ms = renderId2Model[r];
        //    if (ms.Count > 1)
        //    {
        //        if (rendererId2Bim.ContainsKey(r))
        //        {
        //            var bim = rendererId2Bim[r];
        //            Debug.LogError($"repeatModels rendererId:{r} list:{ms.Count} bim:{bim}");
        //        }
        //        else
        //        {
        //            Debug.LogError($"repeatModels rendererId:{r} list:{ms.Count} bim:NULL");
        //        }
        //        //repeatModels.AddRange(ms);
        //        repeatModelsByRendererId.Add(r, ms);
        //    }
        //}

        //repeatModelsByUId = new Dictionary<string, List<ModelItemInfo>>();
        //foreach (var r in uid2Model.Keys)
        //{
        //    var ms = uid2Model[r];
        //    if (ms.Count > 1)
        //    {
        //        if (guid2Bim.ContainsKey(r))
        //        {
        //            var bim = guid2Bim[r];
        //            Debug.LogError($"repeatModels UID:{r} list:{ms.Count} bim:{bim}");
        //        }
        //        else
        //        {
        //            Debug.LogError($"repeatModels UID:{r} list:{ms.Count} bim:NULL");
        //        }
        //        //repeatModels.AddRange(ms);
        //        repeatModelsByUId.Add(r, ms);
        //    }
        //}

        allModels = ModelRoot.GetAllItems();

        allModels_drawable_nozero.Clear();
        allModels_drawable_zero.Clear();
        allModels_noDrawable_nozero.Clear();
        allModels_noDrawable_zero.Clear();

        //allModels.Clear();
        allModels_uid.Clear();
        allModels_noUid.Clear();

        for (int i = 0; i < allModels.Count; i++)
        {
            ModelItemInfo child = allModels[i];
            //if (child.IsZero())
            //{
            //    if (child.Drawable == false)
            //    {
            //        allModels_zero.Add(child);
            //    }
            //    else
            //    {
            //        allModels_drawable.Add(child);
            //    }
            //}
            //else
            //{
            //    if (child.Drawable == false)
            //    {
            //        allModels_noDrawable.Add(child);
            //    }
            //    else
            //    {
                    
            //    }
            //}

            if (child.Drawable == true)
            {
                //allModels_drawable.Add(child);
                if (child.IsZero())
                {
                    allModels_drawable_zero.Add(child);
                }
                else
                {
                    allModels_drawable_nozero.Add(child);
                }
            }
            else
            {
                if (child.IsZero())
                {
                    allModels_noDrawable_zero.Add(child);
                }
                else
                {
                    allModels_noDrawable_nozero.Add(child);
                }
            }

            if (!string.IsNullOrEmpty(child.UId))
            {
                allModels_uid.Add(child);
            }
            else
            {
                allModels_noUid.Add(child);
            }
        }

        allModels.Sort();
        allModels_uid.Sort();
        allModels_noUid.Sort();

        allModels_drawable_zero.Sort();
        allModels_drawable_nozero.Sort();
        allModels_noDrawable_nozero.Sort();
        allModels_noDrawable_zero.Sort();

        ProgressBarHelper.ClearProgressBar();

        Debug.Log($"LoadModels time:{DateTime.Now - start} rendererIdCount:{ModelDict.rendererIdCount} UidCount:{ModelDict.UidCount}");
    }

    public void RemoveRepeated()
    {
        this.ModelDict.RemoveRepeatedModelInfo(this.BimDict);
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

        for (int i = 0; i < allModels_noDrawable_nozero.Count; i++)
        {
            ModelItemInfo child = allModels_noDrawable_nozero[i];
            GameObject go = child.Tag as GameObject;

            ProgressArg p1 = new ProgressArg("CreateTree3", i, allModels_noDrawable_nozero.Count, child.Name);
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
        noFoundBimInfos01.Clear();
        noFoundBimInfos02.Clear();
        noFoundBimInfos03.Clear();
        noFoundBimInfos04.Clear();

        foundBimInfos1.Clear();
        foundBimInfos2.Clear();
        foundBimInfos3.Clear();
        //bimInfos0_name.Clear();
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

                        var list = InitNavisFileInfoByModel.FindSameNameList(transformList, child.Name);
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
