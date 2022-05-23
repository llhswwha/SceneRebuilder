using RevitTools.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.Entities.UniversalDelegates;
using UnityEditor;
using UnityEngine;

public class ModelBuildingScript : MonoBehaviour
{
    public static ModelBuildingScript Instance;

    void Awake()
    {
        Instance = this;
    }

    public ModelBuildingInfo Info;

    public List<ModelLevelScript> Levels = new List<ModelLevelScript>();

    public Vector3 ExpandOffset = new Vector3(0, 50, 0);

    [ContextMenu("ExpandLevels")]
    public void ExpandLevels()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            ModelLevelScript item = Levels[i];
            item.gameObject.transform.position += ExpandOffset * i;
        }

        HideCategories("梁","天花板");
    }

    [ContextMenu("ResetLevels")]
    public void ResetLevels()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            ModelLevelScript item = Levels[i];
            item.ResetPos();
        }

        ShowCategories("天花板");
    }

    public void HideCategories(params string[] categoryKey)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            ModelLevelScript item = Levels[i];
            item.SetCategoriesVisible(false,categoryKey);
        }
    }

    public void ShowCategories(params string[] categoryKey)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            ModelLevelScript item = Levels[i];
            item.SetCategoriesVisible(true, categoryKey);
        }
    }

    public string HideKeys = "梁";

    [ContextMenu("HideFrameByKeys")]
    public void HideFrameByKeys()
    {
        if (HideKeys != "")
        {
            string[] keys = HideKeys.Split(',');
            HideCategories(keys);
        }
        else
        {
            HideCategories("梁");
        }
    }

    [ContextMenu("ShowAllFrame")]
    public void ShowAllFrame()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            ModelLevelScript item = Levels[i];
            item.ShowAllFrame();
        }
    }

    public ModelLevelScript GetLevel(string lvName)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            ModelLevelScript item = Levels[i];
            if (item.name == lvName)
            {
                return item;
            }
        }
        return null;
    }

    public void CombineLevel(string levelName1,string levelName2, bool isEqual)
    {
        ModelLevelScript level1 = FindLevel(levelName1, isEqual);
        print("CombineLevel level1:" + levelName1);
        ModelLevelScript level2 = FindLevel(levelName2, isEqual);
        print("CombineLevel level2:" + levelName2);
        if (level2 != null)
        {
            if (level2.Combine(level1))
            {
                Levels.Remove(level1);
                GameObject.Destroy(level1.gameObject);
            }
        }
    }

    public void CombineLevel(string sceneName)
    {
        if (sceneName == "华虹动力站建筑")
        {
            CombineLevel("室外地坪", "1F",false);
            CombineLevel("女儿墙", "RF", false);
        }
        if (sceneName == "华虹芯片生产厂房")
        {
            CombineLevel("-3.000", "B1", false);
            CombineLevel("19.2", "4F(18.750)", true);
            CombineLevel("23.7", "RF1(23.250)", true);
            CombineLevel("RF", "RF1(23.250)", true);
            CombineLevel("RF2(26.050)", "RF1(23.250)", true);
            CombineLevel("RF3(31.700)", "RF1(23.250)", true);
            CombineLevel("RF2(31.700)", "RF1(23.250)", true);
            CombineLevel("3F(12.300)", "3F(11.700)", true);
        }
    }

    public ModelLevelScript FindLevel(string lvName,bool isEqual)
    {
        foreach (var level in Levels)
        {
            if (isEqual)
            {
                if (level.Info.Name.Equals(lvName))
                {
                    return level;
                }
            }
            else
            {
                if (level.Info.Name.Contains(lvName))
                {
                    return level;
                }
            }
            
        }
        string alllevels = "";
        foreach (var level in Levels)
        {
            alllevels += level.Info.Name + ";";
        }
        Debug.LogError("未找到楼层:" + lvName + ",isEqual=" + isEqual+ "  alllevels:"+ alllevels);
        return null;
    }

    private ElementGroupList RevitElementGroupList = null;

    public void LoadRevitElementGroup()
    {
        // if (RevitElementGroupList == null || RevitElementGroupList.Count==0)
        // {
        //     var sName = "华虹动力站设备";
        //     var path = "RvtScenes/" + sName ;
        //     RevitElementGroupList = SceneRebuilder.LoadElementGroupFromResourceEx(path, null, null);
        // }

        // if (RevitElementGroupList == null)
        // {
        //     Debug.LogError("RevitElementGroup == null");
        // }
    }

    public List<string> PipeSystemList = new List<string>() { "BY_连通管", "CHR_冷冻水回水", "CHS_冷冻水供水", "CWR_冷却回水", "CWS_冷却水供", "HWR_高温热水回水", "HWS_高温热水供水", "IW_生活上水", "P_一般排水", "RCW_回收水系统", "SP_沙滤排水", "WWR_热回收回水", "WWS_热回收供水", "干式消防系统", "家用冷水", "家用热水", "其他", "其他消防系统", "湿式消防系统", "通风孔", "卫生设备", "循环供水", "循环回水", "预作用消防系统" };

    public string TestPipeSystemName = "BY_连通管";

    public List<GameObject> TestPipeSystem;

    public List<Material> PipeSystemMaterials;

    [ContextMenu("SetAllPipeSystemMaterials")]
    public void SetAllPipeSystemMaterials()
    {
        foreach (var system in PipeSystemList)
        {
            var r=SetPipeSystemMaterial(system);
            print(string.Format("{0}:{1}", system, r));
        }
    }

    [ContextMenu("TestHighlightPipeSystem")]
    public void TestHighlightPipeSystem()
    {

        HighlightPipeSystem(TestPipeSystemName);
    }

    public List<GameObject> GetPipeSystem(string systemName)
    {
        Debug.Log("GetPipeSystem:" + systemName);
        List<GameObject> pipes = new List<GameObject>();

        LoadRevitElementGroup();
        if (RevitElementGroupList == null)
        {
            return pipes;
        }
        
        NodeInfoScript[] nodes = GameObject.FindObjectsOfType<NodeInfoScript>();
        foreach (var node in nodes)
        {
            if (node.NodInfo == null)
            {
                continue;
            }
            ElementInfo info = node.EleInfo;
            if (info == null)
            {
                string id = node.GetNodeId();
                info = RevitElementGroupList.GetElementInfo(id,node.name);
                if (info == null)
                {
                    //Debug.LogError("ElementInfo == null :"+ id+","+ node.gameObject);
                }
                node.EleInfo = info;
            }

            if (info != null)
            {
                Debug.Log("systemName:" + info.SystemName);
                if (systemName == info.SystemName)
                {
                    pipes.Add(node.gameObject);
                }
            }
            
        }

        Debug.Log("pipes:"+pipes.Count);
        return pipes;
    }

    public static List<Material> GetSharedMaterials(List<GameObject> objs)
    {
        List<Material> materials = new List<Material>();
        foreach (var item in objs)
        {
            MeshRenderer meshRender = item.GetComponent<MeshRenderer>();
            var ms = meshRender.sharedMaterials;
            foreach (var m in ms)
            {
                if (m == null)
                {
                    continue;
                }
                if (!materials.Contains(m))
                {
                    materials.Add(m);
                }
            }
        }
        return materials;
    }

    public List<GameObject> HighlightPipeSystem(string systemName)
    {
        List<GameObject> pipes = GetPipeSystem(systemName);
        TestPipeSystem = pipes;
        NodeHighlightSystem.Instance.Highlight(pipes);
        return pipes;
    }

    public bool SetPipeSystemMaterial(string systemName)
    {
#if UNITY_EDITOR
        List<GameObject> pipes = GetPipeSystem(systemName);
        TestPipeSystem = pipes;

        PipeSystemMaterials = GetSharedMaterials(pipes);
        if (PipeSystemMaterials.Count == 1)
        {
            Material m = PipeSystemMaterials[0];
            if (m.name == systemName) return true;
            if(m.name!= "No Name")
            {
                return false;
            }
            var path = AssetDatabase.GetAssetPath(m);
            print("name:" + m.name);
            print("path:" + path);
            Material newMaterial = new Material(m);
            newMaterial.name = systemName;
            var pathNew = path.Replace(m.name, systemName);
            print("pathNew:" + pathNew);
            AssetDatabase.CreateAsset(newMaterial, pathNew);

            AssetDatabase.SaveAssets();

            foreach (var item in pipes)
            {
                MeshRenderer meshRenderer = item.GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = newMaterial;
            }
            return true;
        }
        else
        {
            return false;
        }
#endif
        return true;
    }
}
