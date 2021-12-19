using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitNavisFileInfoByModelSetting : SingletonBehaviour<InitNavisFileInfoByModelSetting>
{
    public bool IsFindInCurrentModels = false;

    public List<string> CurrentModels = new List<string>();

    public List<GameObject> initInfoBuildings = new List<GameObject>();

    [ContextMenu("UpdateBuildings")]
    public void UpdateBuildings()
    {
        //initInfoBuildings.Clear();
        var bs1 = this.initInfoBuildings;
        this.initInfoBuildings.Clear();
        foreach (var b in bs1)
        {
            if (b == null) continue;
            this.initInfoBuildings.Add(b.gameObject);
        }

        BuildingController[] bs = GameObject.FindObjectsOfType<BuildingController>(true);
        foreach (var b in bs)
        {
            if (this.initInfoBuildings.Contains(b.gameObject)) continue;
            this.initInfoBuildings.Add(b.gameObject);
        }
    }

    public bool enableDistance2 = false;

    public bool enableDistance3 = false;

    public float MinDistance1 = 0.005f;

    public float MinDistance2 = 0.05f;

    public float MinDistance3 = 0.15f;

    public float MinDistance4 = 0.3f;

    public float MinDistance5 = 0.6f;

    public static List<string> FilterNames1_Default = new List<string>() { "In", "Out0", "Out1", "LOD", "LODs", "Structure" };

    public List<string> FilterNames1 = new List<string>(FilterNames1_Default);

    public static List<string> FilterNames2_Default = new List<string>() { "_F1", "_F2", "_F3", "_F4", "_F5", "_F6", "_F7",
        "LODs","LOD0","LOD1", "LOD2", "LOD3", "LOD4","_boli","_Metal032_2K_","_door","_Door1A","_Door1B",
    "_Doors","_Floors","_Others","_Pillars","_Windows","_Devs","_F111111·rames","Stairs","_Walls","_Welding"};

    public List<string> FilterNames2 = new List<string>(FilterNames2_Default);

    public static List<string> FilterNames3_Default = new List<string>() { "合成部分", "Bounds" };

    public List<string> FilterNames3 = new List<string>(FilterNames3_Default);


    public static List<string> structureNameList1_Default = new List<string>() { "MemberPartPrismatic", "PHC600AB","PHC6的·100AB", "Slab", "WallPart", "Stair", "楼梯", "转角井", "TMTHandrail", "VVAirDistribAssemAsm","CTJ01","CTJ02","CTJ03","CTJ04","CTJ05","CTJ06" };

    public List<string> structureNameList1 = new List<string>(structureNameList1_Default);

    public static List<string> structureNameList2_Default = new List<string>() {  "楼梯", "转角井" ,"PHC600AB", "PHC500AB", };

    public List<string> structureNameList2 = new List<string>(structureNameList2_Default);

    public List<string> filterModelParent = new List<string>() {"窗" };//一幢大楼的窗户被我们按楼层拆掉了。

    [ContextMenu("AddDoorABFilter")]
    public void AddDoorABFilter()
    {

        Debug.Log($"AddDoorABFilter list1:{FilterNames1_Default.Count}-{FilterNames1.Count} list2:{FilterNames2_Default.Count}-{FilterNames2.Count} list3:{FilterNames3_Default.Count}-{FilterNames3.Count}");
        foreach (var n in FilterNames1_Default)
        {
            if (!FilterNames1.Contains(n))
            {
                FilterNames1.Add(n);
            }
        }

        foreach (var n in FilterNames2_Default)
        {
            if(!FilterNames2.Contains(n))
            {
                FilterNames2.Add(n);
            }
        }

        foreach (var n in FilterNames3_Default)
        {
            if (!FilterNames3.Contains(n))
            {
                FilterNames3.Add(n);
            }
        }

        foreach (var n in structureNameList1_Default)
        {
            if (!structureNameList1.Contains(n))
            {
                structureNameList1.Add(n);
            }
        }

        foreach (var n in structureNameList2_Default)
        {
            if (!structureNameList2.Contains(n))
            {
                structureNameList2.Add(n);
            }
        }

        for (int i = 0; i < 20; i++)
        {
            if (!FilterNames2.Contains($"_Door{i + 1}A"))
            {
                FilterNames2.Add($"_Door{i + 1}A");
            }
            if (!FilterNames2.Contains($"_Door{i + 1}B"))
            {
                FilterNames2.Add($"_Door{i + 1}B");
            }
        }
    }

    public bool IsFilteredName(string n)
    {
        if (FilterNames1.Contains(n))
        {
            return true;
        }
        foreach (var f in FilterNames2)
        {
            if (n.EndsWith(f))
            {
                return true;
            }
        }
        foreach (var f in FilterNames3)
        {
            if (n.Contains(f))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsFiltered<T>(T t) where T :Component
    {
        if (t.name == "HorPumpBB1Asm-1-0002")
        {
            Debug.LogError("HorPumpBB1Asm-1-0002");
        }
        if (IsFilteredName(t.name))
        {
            return true;
        }
        MeshRendererInfo info = t.GetComponent<MeshRendererInfo>();
        if (info != null)
        {
            //if (info.IsRendererType(MeshRendererType.LOD) && !info.IsLodN(0))
            //{
            //    return true;
            //}

            //if (info.IsRendererType(MeshRendererType.LOD))
            //{
            //    if (info.GetComponent<LODGroup>() == null) return true;
            //}
        }
        if (t.transform.childCount == 0 && t.GetComponent<MeshRenderer>() == null)
        {
            return true;
        }
        //if (t.GetComponent<MeshRenderer>() == null && t.GetComponent<LODGroup>() == null) return true;
        //if (MeshHelper.IsEmptyGroup(t, false)) return true;
        //if (MeshHelper.IsSameNameGroup(t)) return true;
        if (MeshHelper.IsEmptyLODSubGroup(t.transform)) return true;
        return false;
    }

    public List<ModelItemInfo> FilterList(List<ModelItemInfo> list1, ProgressArgEx p0)
    {
        List<ModelItemInfo> all = new List<ModelItemInfo>();
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            ModelItemInfo t = list1[i1];
            var p1 = ProgressArg.New("FilterList", i1, list1.Count, t.Name, p0);
            ProgressBarHelper.DisplayCancelableProgressBar(p1);
            if(t.Name== "1B_F1_Door4")
            {

            }
            if (IsFilteredName(t.Name))
            {
                continue;
            }
            if (IsIncludeStructure == false && IsStructrue(t.Name))
            {
                continue;
            }
            all.Add(t);
        }
        if (p0 == null)
            ProgressBarHelper.ClearProgressBar();
        return all;
    }

    public string DebugFilterModelName = "#3机密封油供油装置";

    public string DebugFilterTransformName = "1B_F1_Door4";

    public List<T> FilterList<T>(List<T> list1, ProgressArgEx p0) where T :Component
    {
        List<T> all = new List<T>();
        for (int i1 = 0; i1 < list1.Count; i1++)
        {
            T t = list1[i1];
            var p1 = ProgressArg.New("FilterList", i1, list1.Count, t.name, p0);
            ProgressBarHelper.DisplayCancelableProgressBar(p1);
            if (t.name == DebugFilterTransformName)
            {

            }
            if (IsFiltered(t))
            {
                continue;
            }
            if (IsIncludeStructure==false && IsStructrue(t.name))
            {
                continue;
            }
            all.Add(t);
        }
        if (p0 == null)
            ProgressBarHelper.ClearProgressBar();
        return all;
    }

    public bool IsIncludeStructure = true;//是否包括建筑结构

    //public bool IsFilterBIM = false;


    public bool IsStructrue(string n)
    {
        foreach (var key in structureNameList1)
        {
            if (n.StartsWith(key))
            {
                return true;
            }
        }
        foreach (var key in structureNameList2)
        {
            if (n.Contains(key))
            {
                return true;
            }
        }
        return false;
    }

    public List<ModelCheckExceptionalCase> ExceptionalCases = new List<ModelCheckExceptionalCase>();

    public bool CheckExceptialCases(ModelItemInfo model1,Transform closedT,float dis)
    {
        if (ExceptionalCases.Count == 0)
        {
            ExceptionalCases.Add(new ModelCheckExceptionalCase("门;窗", "door", 0.6f));
        }
        foreach(var eCase in ExceptionalCases)
        {
            //if (model1.GetParent().Name == eCase.ModelParentName && closedT.name.ToLower().Contains(eCase.TranformName))
            if (eCase.ModelParentName.Contains(model1.GetParent().Name)  && closedT.name.ToLower().Contains(eCase.TranformName)) //门，或者窗，门可能放到窗的里面
            {
                if (dis < eCase.MinDistance)
                {
                    //AddFounded1(model1, closedT, BIMFoundType.ByClosed);
                    return true;
                }
                else
                {
                    Debug.LogError($"[CheckExceptialCases][门模型的距离太远了][{dis},{closedT}][Name:{model1.Name}][Path:{model1.GetPath()}][{model1.ShowDistance(closedT)})]");
                }
            }
        }
        return false;

    }

    [Serializable]
    public class ModelCheckExceptionalCase
    {
        public string ModelParentName;

        public string TranformName;

        public float MinDistance;

        public ModelCheckExceptionalCase(string n1,string n2,float dis)
        {
            this.ModelParentName = n1;
            this.TranformName = n2;
            this.MinDistance = dis;
        }
    }
}
