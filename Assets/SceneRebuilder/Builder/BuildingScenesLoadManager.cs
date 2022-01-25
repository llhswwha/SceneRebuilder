using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScenesLoadManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string TestTargetName = "";

    public void LoadBuildings()
    {

    }

    public void LoadFloors()
    {

    }

    [ContextMenu("LoadScenesBySetting")]
    public void LoadScenesBySetting()
    {
        DepNode[] bcs = ModelTarget.GetComponentsInChildren<DepNode>(true);
        Dictionary<string, DepNode> depDict = new Dictionary<string, DepNode>();
        foreach (var bc in bcs)
        {
            depDict.Add(bc.NodeName, bc);
        }

        foreach (var bc in Setting.Items)
        {
            if (depDict.ContainsKey(bc.Name) == false)
            {
                Debug.LogError($"LoadScenesBySetting depDict.ContainsKey(bc.Name) == false bc:{bc.Name}");
                continue;
            }

            BuildingController bc0 = depDict[bc.Name] as BuildingController;

            if (bc0==null)
            {
                Debug.LogError($"LoadScenesBySetting NotBuildingController bc:{bc.Name}");
                continue;
            }

            if (bc.Children!=null&& bc.Children.Count > 0)
            {
                foreach (var fl in bc.Children)
                {
                    if (depDict.ContainsKey(fl.Name) == false)
                    {
                        Debug.LogError($"LoadScenesBySetting depDict.ContainsKey(fl.Name) == false bc:{fl.Name}");
                        continue;
                    }

                }
            }
            else
            {
                BuildingModelInfo bmi = bc0.GetComponent<BuildingModelInfo>();
                if (bc0 == null)
                {
                    Debug.LogError($"LoadScenesBySetting NotBuildingController bc:{bc.Name}");
                    continue;
                }
            }
        }
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
        BuildingController[] bcs = ModelTarget.GetComponentsInChildren<BuildingController>(true);
        Setting = new BuildingSceneLoadSetting();
        foreach(var bc in bcs)
        {
            BuildingSceneLoadItem bItem = new BuildingSceneLoadItem();
            bItem.Name = bc.NodeName;
            Setting.Items.Add(bItem);
            FloorController[] fls = bc.GetComponentsInChildren<FloorController>(true);
            foreach(var fl in fls)
            {
                BuildingSceneLoadItem fItem = new BuildingSceneLoadItem();
                bItem.Children.Add(fItem);
            }
        }

        //string xml=XmlSerializableHelper.
    }

    public void SaveXml()
    {

    }

    public void LoadXml()
    {

    }

    public BuildingSceneLoadSetting Setting;
}

[Serializable]
public class BuildingSceneLoadSetting
{
    public List<BuildingSceneLoadItem> Items = new List<BuildingSceneLoadItem>();

    public BuildingSceneLoadItem GetItem(string name)
    {
        return null;
    }

    public List<BuildingSceneLoadItem> GetAllItems()
    {
        List<BuildingSceneLoadItem> list = new List<BuildingSceneLoadItem>();
        return list;
    }
}

[Serializable]
public class BuildingSceneLoadItem
{
    public string Name;
    public bool IsSameWithParent;

    public bool IsLoadInCombined;
    public bool IsLoadOut0Combined;
    public bool IsLoadOut1Combined;
    public bool IsStaticInRenderers;
    public bool IsStaticOut0Renderers;
    public bool IsStaticOut1Renderers;

    public BuildingSceneLoadItem()
    {
        IsLoadOut0Combined = true;
    }

    public List<BuildingSceneLoadItem> Children = new List<BuildingSceneLoadItem>();
}