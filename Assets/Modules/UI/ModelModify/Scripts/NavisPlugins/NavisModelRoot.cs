using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Base.Common;

public class NavisModelRoot : MonoBehaviour
{
    public string ModelName = "";

    [NonSerialized]
    public ModelItemInfo Model;

    [NonSerialized]
    public List<ModelItemInfo> allModels = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_noDrawable = new List<ModelItemInfo>();

    [NonSerialized]
    public List<ModelItemInfo> allModels_zero = new List<ModelItemInfo>();


    public List<Transform> transformList = new List<Transform>();

    public List<GameObject> RootNodes = new List<GameObject>();

    public List<BIMModelInfo> bimInfos = new List<BIMModelInfo>();

    public string TestModelName = "H¼¶Ö÷±ä1";

    [ContextMenu("TestFindModelByName")]
    public void TestFindModelByName()
    {
        var list=InitNavisFileInfoByModel.FindSameNameList(transformList, TestModelName);
        Debug.Log($"TestFindModelByName name:{TestModelName} list:{list.Count}");
    }

    [ContextMenu("GetBims")]
    public void GetBims()
    {
        bimInfos = new List<BIMModelInfo>();
        bimInfos.AddRange(this.GetComponentsInChildren<BIMModelInfo>(true));
        bimInfos.Sort();

        Debug.LogError($"GetBims infos:{bimInfos.Count}");
    }

    NavisFileInfo navisFile;

    public void SetOnlySelfModel()
    {
        navisFile = InitNavisFileInfoByModel.GetNavisFileInfoEx();
        Model = navisFile.Models.Find(i => i.Name == ModelName);
        navisFile.Models.Clear();
        navisFile.Models.Add(Model);
        SaveXml();
    }

    [ContextMenu("LoadModels")]
    public void LoadModels()
    {
        DateTime start = DateTime.Now;
        if(string.IsNullOrEmpty(ModelName))
        {
            ModelName = this.name;
        }

        ClearResult();

        transformList = this.GetComponentsInChildren<Transform>(true).ToList();

        transformList = InitNavisFileInfoByModel.Instance.FilterList(transformList);

        navisFile = InitNavisFileInfoByModel.GetNavisFileInfoEx();

        int rendererIdCount = 0;
        var all = navisFile.GetAllItems();
        Dictionary<string,ModelItemInfo> rId = new Dictionary<string, ModelItemInfo>();
        for (int i = 0; i < all.Count; i++)
        {
            ModelItemInfo item = all[i];


            ProgressArg p1 = new ProgressArg("LoadModels", i, all.Count, item.Name);
            //InitNavisFileInfoByModel.Instance.progressArg = p1;
            ProgressBarHelper.DisplayCancelableProgressBar(p1);

            if (!string.IsNullOrEmpty(item.RenderId))
            {
                rendererIdCount++; 
                
                if (!rId.ContainsKey(item.RenderId))
                {
                    rId.Add(item.RenderId, item);
                }
                else
                {
                    var old = rId[item.RenderId];
                    Debug.LogError($"old:{old.Name} new:{item.Name} rendererId:{item.RenderId} rendererName:{item.RenderName}");
                }
            }


        }

        Model = navisFile.Models.Find(i => i.Name == ModelName);
        if (Model == null)
        {
            Model = navisFile.Models.Find(i => i.Name.Contains(ModelName));
            Debug.Log($"Model == null 1 ModelName:{ModelName} Model:{Model} Models:{navisFile.Models.Count}");
            //return;
        }

        if (Model == null)
        {
            //Model = navisFile.Models.Find(i => i.Name.Contains(ModelName));
            Debug.LogError($"Model == null ModelName:{ModelName} Model:{Model} Models:{navisFile.Models.Count}");
            return;
        }

        List<ModelItemInfo> list = Model.GetAllItems();

        allModels.Clear();
        allModels_noDrawable.Clear();
        allModels_zero.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            ModelItemInfo child = list[i];
            if (child.X == 0 && child.Z == 0 && child.Y == 0)
            {
                if (child.Drawable == false)
                {
                    allModels_zero.Add(child);
                }
                else
                {
                    allModels.Add(child);
                }
            }
            else
            {
                if (child.Drawable == false)
                {
                    allModels_noDrawable.Add(child);
                }
                else
                {
                    allModels.Add(child);
                    if (child.Children != null && child.Children.Count > 0)
                    {
                        //Debug.LogError($"child.Children!=null&&child.Children.Count > 0 :{child.Name}");
                    }
                }
            }

        }

        allModels.Sort();
        allModels_noDrawable.Sort();
        allModels_zero.Sort();

        ProgressBarHelper.ClearProgressBar();

        Debug.Log($"LoadModels time:{DateTime.Now - start} rendererIdCount:{rendererIdCount}");
    }

    [ContextMenu("CreateTree")]
    public void CreateTree()
    {
        DateTime start = DateTime.Now;

        ClearResult();

        TreeNodeCount = 0;
        TreeNodeIndex = 0;
        InitCreateTree(Model);
        Debug.LogError($"CreateTree TreeNodeCount:{TreeNodeCount}");

        RootNodes =CreateTree(Model, this.transform);

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

        for (int i = 0; i < allModels_noDrawable.Count; i++)
        {
            ModelItemInfo child = allModels_noDrawable[i];
            GameObject go = child.Tag as GameObject;

            ProgressArg p1 = new ProgressArg("CreateTree3", i, allModels_noDrawable.Count, child.Name);
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
