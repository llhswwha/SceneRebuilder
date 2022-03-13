using Base.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class BuildingScenesLoadManager : MonoBehaviour
{
    public bool IsLoadXmlOnStart = true;
    public bool IsLoadSceneOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        if (IsLoadXmlOnStart)
        {
            LoadXml();
        }
        if (IsLoadSceneOnStart)
        {
            LoadScenesBySetting();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string TestTargetName = "";

    public void LoadDepNodes(List<DepNode> depNodes)
    {

    }

    public void LoadBuildings(List<DepNode> depNodes)
    {

    }

    public void LoadFloors()
    {

    }

    [ContextMenu("CreateScenes")]
    public void CreateScenes()
    {
        var trees = ModelTarget.GetComponentsInChildren<ModelAreaTree>();
        for(int i=0;i<trees.Length;i++)
        {
            //trees[i].EditorCreateScenes();
        }
    }

    [ContextMenu("LoadScenes")]
    public void LoadScenes()
    {
        if (ModelTarget == null)
        {
            Debug.LogError($"BuildingScenesLoadManager.LoadScenes ModelTarget == null");
            return;
        }
        var trees = ModelTarget.GetComponentsInChildren<ModelAreaTree>();
        for (int i = 0; i < trees.Length; i++)
        {
            //trees[i].EditorCreateScenes();
        }
    }

    [ContextMenu("LoadScenesBySetting")]
    public void LoadScenesBySetting()
    {
        if (ModelTarget == null)
        {
            Debug.LogError($"BuildingScenesLoadManager.LoadScenesBySetting ModelTarget == null");
            return;
        }
        DepNode[] bcs = ModelTarget.GetComponentsInChildren<DepNode>(true);
        Dictionary<string, DepNode> depDict = new Dictionary<string, DepNode>();
        foreach (var bc in bcs)
        {
            if(string.IsNullOrEmpty(bc.NodeName))
            {
                bc.NodeName = bc.name;
            }
            depDict.Add(bc.NodeName, bc);
        }

        BuildingModelInfo[] models = ModelTarget.GetComponentsInChildren<BuildingModelInfo>(true);
        Dictionary<string, BuildingModelInfo> modelDict = new Dictionary<string, BuildingModelInfo>();
        foreach (var bc in models)
        {
            modelDict.Add(bc.name, bc);
        }

        Debug.Log($"LoadScenesBySetting depDict:{depDict.Count}");


        SubSceneBagList allScenes = new SubSceneBagList();
        for (int i = 0; i < Setting.Items.Count; i++)
        {
            BuildingSceneLoadItemCollection buildingloadItem = Setting.Items[i];
            //Debug.LogError($"LoadScenesBySetting building[{i+1}]:{buildingloadItem}");
            if (depDict.ContainsKey(buildingloadItem.Name) == false)
            {
                Debug.LogError($"LoadScenesBySetting depDict.ContainsKey(buildingloadSetting.Name) == false building:{buildingloadItem}");
                continue;
            }

            DepNode buildingDep = depDict[buildingloadItem.Name];
            BuildingController buildingController = buildingDep as BuildingController;
            if (buildingController==null)
            {
                Debug.LogError($"LoadScenesBySetting buildingController==null buildingDep:{buildingDep}");
                continue;
            }

            if (buildingloadItem.IsEnable == false)
            {
                GameObject.DestroyImmediate(buildingController.gameObject);
                continue;
            }

            if (buildingloadItem.Children!=null&& buildingloadItem.Children.Count > 0)
            {
                for (int i1 = 0; i1 < buildingloadItem.Children.Count; i1++)
                {
                    BuildingSceneLoadItem floorLoadItem = buildingloadItem.Children[i1];
                    //Debug.LogError($"LoadScenesBySetting building[{i + 1}]:{buildingloadItem.Name} floor[{i1+1}]:{floorLoadItem}");

                    if (modelDict.ContainsKey(floorLoadItem.Name) == false)
                    {
                        Debug.LogError($"LoadScenesBySetting depDict.ContainsKey(floorLoadSetting.Name) == false floor:{floorLoadItem}");
                        continue;
                    }

                    BuildingModelInfo floorModelInfo = modelDict[floorLoadItem.Name];
                    //FloorController floorController = floorDep as FloorController;
                    //if (floorController == null)
                    //{
                    //    Debug.LogError($"LoadScenesBySetting floorController == null floorDep:{floorDep}");
                    //    continue;
                    //}

                    //BuildingModelInfo floorModelInfo = floorController.GetComponent<BuildingModelInfo>();
                    if (floorModelInfo == null)
                    {
                        Debug.LogError($"LoadScenesBySetting floorModelInfo == null building:{floorLoadItem.Name}");
                        continue;
                    }
                    else
                    {
                        if (floorLoadItem.IsEnable == false)
                        {
                            GameObject.DestroyImmediate(floorModelInfo.gameObject);
                        }
                        else
                        {
                            //LoadFloorScenes
                            //LoadSubScenes(floorController)
                            SubSceneBag subScenes = floorModelInfo.GetSubScenes(floorLoadItem);
                            allScenes.Add(subScenes);
                        }
                        
                    }
                }
            }
            else
            {
                Debug.LogError($"LoadScenesBySetting buildingloadItem.Children is Empty buildingDep:{buildingDep}");

                //BuildingModelInfo floorModelInfo = buildingController.GetComponent<BuildingModelInfo>();
                //if (floorModelInfo == null)
                //{
                //    Debug.LogError($"LoadScenesBySetting floorModelInfo == null buildingController:{buildingController}");
                //    continue;
                //}

                //SubSceneBag subScenes = floorModelInfo.GetSubScenes(buildingloadItem);
                //allScenes.Add(subScenes);
            }
        
            
        }

        var ss = allScenes.GetAllScenesArray();

        Debug.Log($"LoadScenesBySetting depDict:{depDict.Count} bags:{allScenes.Count} scenes:{ss.Length}");


        SubSceneManager.Instance.LoadScenesAsyncEx(ss, (p)=>
        {

        });
    }

    public void LoadSubScenes(BuildingModelInfo root, BuildingSceneLoadItem loadSetting)
    {

    }

    [ContextMenu("TestLoadTarget")]
    public void TestLoadTarget()
    {
        DepNode[] bcs = ModelTarget.GetComponentsInChildren<DepNode>(true);
        Dictionary<string, DepNode> depDict = new Dictionary<string, DepNode>();
        foreach(var bc in bcs)
        {
            depDict.Add(bc.NodeName, bc);
        }


    }

    public GameObject ModelTarget;

    [ContextMenu("InitSettingByScene")]
    public void InitSettingByScene()
    {
        if (ModelTarget == null)
        {
            Debug.LogError($"BuildingScenesLoadManager.InitSettingByScene ModelTarget == null");
            return;
        }
        BuildingController[] bcs = ModelTarget.GetComponentsInChildren<BuildingController>(true);
        Setting = new BuildingSceneLoadSetting();
        foreach(var bc in bcs)
        {
            BuildingSceneLoadItemCollection bItem = new BuildingSceneLoadItemCollection();
            bItem.Name = bc.NodeName;
            if (string.IsNullOrEmpty(bItem.Name))
            {
                bItem.Name = bc.name;
            }
            Setting.Items.Add(bItem);
            BuildingModelInfo[] fls = bc.GetComponentsInChildren<BuildingModelInfo>(true);
            foreach(var fl in fls)
            {
                BuildingSceneLoadItem fItem = new BuildingSceneLoadItem();
                //fItem.Name = fl.NodeName;
                //if (string.IsNullOrEmpty(fItem.Name))
                //{
                    fItem.Name = fl.name;
                //}
                bItem.Children.Add(fItem);
            }
        }

        //string xml=XmlSerializableHelper.
    }

    //public string XmlFilePath = "d:\\BuildingSceneLoadSetting.xml";

#if UNITY_WEBGL
        public static string XmlFilePath = "\\BuildingSceneLoadSetting.XML";
#else
    public static string XmlFilePath = "\\..\\BuildingSceneLoadSetting.XML";
#endif

    public string GetXmlFilePath()
    {
        string path = Application.dataPath + XmlFilePath;
        return path;
    }

    [ContextMenu("SaveXml")]
    public void SaveXml()
    {
        string xml = SerializeHelper.GetXmlText(Setting);
        Debug.Log($"SaveXml xml:{xml}");
        File.WriteAllText(GetXmlFilePath(), xml);
    }

    [ContextMenu("LoadXml")]
    public void LoadXml()
    {
        string path = GetXmlFilePath();
        if (File.Exists(path) == false)
        {
            Debug.LogError($"LoadXml FileNotFound path:{path}");
            return;
        }
        string xml=File.ReadAllText(path);
        Debug.Log($"LoadXml xml:{xml}");
        BuildingSceneLoadSetting setting = SerializeHelper.LoadFromText<BuildingSceneLoadSetting>(xml);
        Setting = setting;
        //Setting.SetAllEnable(true);
        Debug.Log($"LoadXml setting:{setting} xml:{xml}");
    }

    [ContextMenu("SetAllEnable")]
    public void SetAllEnable()
    {
        if (Setting!=null)
        {
            Setting.SetAllEnable(true);
        }
    }

    public BuildingSceneLoadSetting Setting;
}

[Serializable]
public class BuildingSceneLoadSetting
{
    public List<BuildingSceneLoadItemCollection> Items = new List<BuildingSceneLoadItemCollection>();

    //public BuildingSceneLoadItem GetItem(string name)
    //{
    //    return null;
    //}

    //public List<BuildingSceneLoadItem> GetAllItems()
    //{
    //    List<BuildingSceneLoadItem> list = new List<BuildingSceneLoadItem>();
    //    return list;
    //}

    public override string ToString()
    {
        return $"BuildingSceneLoadSetting({Items.Count})";
    }

    public void SetAllEnable(bool e)
    {
        foreach(var item in Items)
        {
            item.SetAllEnable(e);
        }
    }
}

[Serializable]
[XmlType("BuildingSceneLoadItemCollection")]
public class BuildingSceneLoadItemCollection
{
    [XmlAttribute]
    public string Name;
    [XmlAttribute]
    public bool IsEnable;

    [XmlElement("BuildingSceneLoadItem")]
    public List<BuildingSceneLoadItem> Children = new List<BuildingSceneLoadItem>();

    public void SetAllEnable(bool e)
    {
        foreach (var item in Children)
        {
            item.SetAllEnable(e);
        }
    }
    public override string ToString()
    {
        //return $"[Name:{Name} InCombined:{InCombined} Out0Combined:{Out0Combined} Out1Combined:{Out1Combined} InRenderers:{InRenderers} Out0Renderers:{Out0Renderers} Out1Renderers:{Out1Renderers}]";
        return $"[Name:{Name} IsEnable:{IsEnable}]";
    }
}

    [Serializable]
[XmlType("BuildingSceneLoadItem")]
public class BuildingSceneLoadItem
{
    public void SetAllEnable(bool e)
    {
        IsEnable = e;
        //InCombined = e;
        //Out0Combined = e;
        //Out1Combined = e;
        InRenderers = e;
        Out0RenderersB = e;
        Out0RenderersS = e;
        Out1Renderers = e;
        LODs = e;

        foreach (var item in Children)
        {
            item.SetAllEnable(e);
        }
    }

    [XmlAttribute]
    public string Name;
    [XmlAttribute]
    public bool IsEnable;
    //[XmlAttribute]
    //public bool InCombined;
    [XmlAttribute]
    public bool InRenderers;
    //[XmlAttribute]
    //public bool Out0Combined;
    [XmlAttribute]
    public bool Out0RenderersB;
    [XmlAttribute]
    public bool Out0RenderersS;
    //[XmlAttribute]
    //public bool Out1Combined;
    [XmlAttribute]
    public bool Out1Renderers;
    [XmlAttribute]
    public bool LODs;

    public BuildingSceneLoadItem()
    {
        IsEnable = true;
        //Out0Combined = true;
        Out0RenderersB = true;
        Out0RenderersS = true;
        //SetAllEnable(true);
    }

    [XmlElement("BuildingSceneLoadItem")]
    public List<BuildingSceneLoadItem> Children = new List<BuildingSceneLoadItem>();

    public override string ToString()
    {
        //return $"[Name:{Name} InCombined:{InCombined} Out0Combined:{Out0Combined} Out1Combined:{Out1Combined} InRenderers:{InRenderers} Out0Renderers:{Out0Renderers} Out1Renderers:{Out1Renderers}]";
        return $"[Name:{Name} In:{InRenderers} Out0B:{Out0RenderersB} Out0S:{Out0RenderersS} Out1:{Out1Renderers}]";
    }
}