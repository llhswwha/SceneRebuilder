using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using Base.Common;
using System.IO;
using System.Text;
using CommonExtension;
using CommonUtils;
using Object = UnityEngine.Object;
using System.Text.RegularExpressions;

public class MyEditorTools2
{
    #region BIM

    [MenuItem("SceneTools/BIM/GetWelds")]
    public static void BIM_GetWelds()
    {
        GameObject go = Selection.activeGameObject;
        PipeWeldModel[] welds = go.GetComponentsInChildren<PipeWeldModel>(true);
        int noBIMCount = 0;
        int BIMCount = 0;
        foreach(var weld in welds)
        {
            BIMModelInfo bim = weld.GetComponent<BIMModelInfo>();
            if (bim == null)
            {
                noBIMCount++;
                Debug.Log($"BIM_GetWelds[{noBIMCount}] path:{weld.transform.GetPath()}");
            }
            else
            {
                BIMCount++;
            }
        }
        Debug.Log($"BIM_GetWelds noBIMCount:{noBIMCount} BIMCount:{BIMCount}");
    }

    [MenuItem("SceneTools/BIM/GetPipes")]
    public static void BIM_GetPipes()
    {
        GameObject go = Selection.activeGameObject;
        PipeModelBase[] welds = go.GetComponentsInChildren<PipeModelBase>(true);
        int noBIMCount = 0;
        int BIMCount = 0;
        foreach (var weld in welds)
        {
            BIMModelInfo bim = weld.GetComponent<BIMModelInfo>();
            if (bim == null)
            {
                noBIMCount++;
                Debug.Log($"BIM_GetPipes[{noBIMCount}] path:{weld.transform.GetPath()}");
            }
            else
            {
                BIMCount++;
            }
        }
        Debug.Log($"BIM_GetPipes noBIMCount:{noBIMCount} BIMCount:{BIMCount}");
    }

    #endregion

    #region Script

    [MenuItem("SceneTools/Script/ClearMainScripts")]
    public static void ClearMainScripts()
    {
        /*
         *  TypeCount2[001]	count:69868	type:RendererId
            TypeCount2[002]	count:51014	type:MeshRendererInfo
            TypeCount2[003]	count:13875	type:MeshNode
            TypeCount2[004]	count:10070	type:MeshPrefabInstance
            TypeCount2[005]	count:9783	type:BIMModelInfo
            TypeCount2[006]	count:9122	type:DoubleClickEventTrigger_u3d
            TypeCount2[008]	count:5839	type:PipeMeshGenerator
            TypeCount2[010]	count:3206	type:PipeLineModel
            TypeCount2[012]	count:2349	type:BoundsBox
            TypeCount2[013]	count:1948	type:PipeElbowModel
         */
        ClearComponents<RendererId>();
        ClearComponents<MeshRendererInfo>();
        ClearComponents<MeshNode>();
        ClearComponents<MeshPrefabInstance>();
        ClearComponents<BIMModelInfo>();
        //ClearComponents<DoubleClickEventTrigger_u3d>();
        ClearComponents<PipeMeshGenerator>();
        ClearComponents<PipeLineModel>();
        ClearComponents<BoundsBox>();
        ClearComponents<PipeElbowModel>();
    }

    [MenuItem("SceneTools/Script/ClearOtherScripts2")]
    public static void ClearOtherScripts2()
    {
        ClearComponents<MeshRendererInfo>();
        ClearComponents<InnerMeshNode>();
        ClearComponents<MeshPrefabInstance>();
        ClearComponents<PipeMeshGenerator>();
        ClearComponents<PipeLineModel>();
        ClearComponents<BoundsBox>();
        //ClearComponents<PipeElbowModel>();
    }

    [MenuItem("SceneTools/Script/ClearOtherScripts")]
    public static void ClearOtherScripts()
    {
        /*
         *  TypeCount2[001]	count:69868	type:RendererId
            TypeCount2[002]	count:51014	type:MeshRendererInfo
            TypeCount2[003]	count:13875	type:MeshNode
            TypeCount2[004]	count:10070	type:MeshPrefabInstance
            TypeCount2[005]	count:9783	type:BIMModelInfo
            TypeCount2[006]	count:9122	type:DoubleClickEventTrigger_u3d
            TypeCount2[008]	count:5839	type:PipeMeshGenerator
            TypeCount2[010]	count:3206	type:PipeLineModel
            TypeCount2[012]	count:2349	type:BoundsBox
            TypeCount2[013]	count:1948	type:PipeElbowModel
         */
        //ClearComponents<RendererId>();
        ClearComponents<MeshRendererInfo>();
        ClearComponents<InnerMeshNode>();
        ClearComponents<MeshPrefabInstance>();
        //ClearComponents<BIMModelInfo>();
        //ClearComponents<DoubleClickEventTrigger_u3d>();
        ClearComponents<PipeMeshGenerator>();
        ClearComponents<PipeLineModel>();
        ClearComponents<BoundsBox>();
        ClearComponents<PipeElbowModel>();
    }

    [MenuItem("SceneTools/Script/ClearUIScripts")]
    public static void ClearUIScripts()
    {
        /*
TypeCount2[007]	count:7186	type:UnityEngine.UI.Image
TypeCount2[009]	count:3823	type:UnityEngine.UI.LayoutElement
TypeCount2[011]	count:2996	type:UnityEngine.UI.Text
TypeCount2[014]	count:1569	type:UnityEngine.UI.Button
TypeCount2[015]	count:1233	type:UnityEngine.UI.HorizontalLayoutGroup
TypeCount2[019]	count:761	type:UnityEngine.UI.Toggle
         */
        ClearComponents<UnityEngine.UI.Image>();
        ClearComponents<UnityEngine.UI.LayoutElement>();
        ClearComponents<UnityEngine.UI.Text>();
        ClearComponents<UnityEngine.UI.Button>();
        ClearComponents<UnityEngine.UI.HorizontalLayoutGroup>();
        ClearComponents<UnityEngine.UI.Toggle>();
    }

    [MenuItem("SceneTools/Script/GetScriptCounts")]
    public static void GetScriptCounts()
    {
        MonoBehaviour[] mbs = GameObject.FindObjectsOfType<MonoBehaviour>(true);
        Dictionary<Type, TypeCount> typeCountDict = new Dictionary<Type, TypeCount>();
        for (int i = 0; i < mbs.Length; i++)
        {
            MonoBehaviour mb = mbs[i];
            
            Type t = mb.GetType();
            if (ProgressBarHelper.DisplayCancelableProgressBar("GetScript", i, mbs.Length, mb.name+"_"+t.Name))
            {
                break;
            }
            if (!typeCountDict.ContainsKey(t))
            {
                TypeCount tc = new TypeCount(t, 1);
                typeCountDict.Add(t, tc);
            }
            else
            {
                TypeCount count1 = typeCountDict[t];
                count1.count++;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        List<TypeCount> typeCountList = typeCountDict.Values.ToList();
        typeCountList.Sort();

        List<TypeCount> typeCountList_UI = new List<TypeCount>();
        List<TypeCount> typeCountList_2 = new List<TypeCount>();
        StringBuilder sb0 = new StringBuilder();
        for (int i = 0; i < typeCountList.Count; i++)
        {
            TypeCount tc = typeCountList[i];
            sb0.AppendLine($"TypeCount1[{i + 1:000}]\tcount:{tc.count}\ttype:{tc.type} ");
            if (tc.count > 1)
            {
                if (tc.type.Name.Contains("UI"))
                {
                    typeCountList_UI.Add(tc);
                }
                else
                {
                    typeCountList_2.Add(tc);
                }
            }
        }
        StringBuilder sb1 = new StringBuilder();
        for (int i = 0; i < typeCountList_UI.Count; i++)
        {
            TypeCount tc = typeCountList_UI[i];
            sb1.AppendLine($"TypeCount_UI[{i + 1:000}]\tcount:{tc.count}\ttype:{tc.type} ");
        }

        StringBuilder sb2 = new StringBuilder();
        for (int i = 0; i < typeCountList_2.Count; i++)
        {
            TypeCount tc = typeCountList_2[i];
            sb2.AppendLine($"TypeCount2[{i + 1:000}]\tcount:{tc.count}\ttype:{tc.type} ");
        }
        Debug.LogError($"GetScriptCounts mbs:{mbs.Length} Count:{typeCountDict.Count} Count_UI:{typeCountList_UI.Count}  Count_2:{typeCountList_2.Count} \nList2:\n{sb2.ToString()} \nUI:\n{sb1.ToString()} \nList1:\n{sb0.ToString()}");
    }

    public class TypeCount:IComparable<TypeCount>
    {
        public Type type;
        public int count;

        public TypeCount(Type t,int c)
        {
            this.type = t;
            this.count = c;
        }

        public int CompareTo(TypeCount other)
        {
            return other.count.CompareTo(this.count);
        }
    }
    #endregion

    #region Building

    [MenuItem("SceneTools/Building/InitFloorModels(In)")]
    public static void InitFloorModels_In()
    {
        FloorController[] floors = GameObject.FindObjectsOfType<FloorController>(true);
        foreach(var floor in floors)
        {
            BuildingModelInfo model=floor.gameObject.AddMissingComponent<BuildingModelInfo>();
            model.InitInOut(false, false);
        }

        Debug.Log($"InitFloorModels_In floors:{floors.Length}");
    }

    [MenuItem("SceneTools/Building/HideOtherBuildings")]
    public static void HideOtherBuildings()
    {
        SetBuildingsActive(false);
        Selection.activeGameObject.SetActive(true);
    }

    private static void SetBuildingsActive(bool isActive)
    {
        var allModelAreas= GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList();
        BuildingController[] bs = GameObject.FindObjectsOfType<BuildingController>(true);
        foreach (var b in bs)
        {
            var floors = b.GetComponentsInChildren<BuildingModelInfo>(true);
            b.gameObject.SetActive(isActive);
            foreach(var floor in floors)
            {
                allModelAreas.Remove(floor);
            }
        }

        foreach (var b in allModelAreas)
        {
            b.gameObject.SetActive(isActive);
        }
    }

    [MenuItem("SceneTools/Building/ShowAllBuildings")]
    public static void ShowAllBuildings()
    {
        SetBuildingsActive(true);
    }

    [MenuItem("SceneTools/Building/MoveSelectionToBuildingIn")]
    public static void MoveSelectionToBuildingIn()
    {
        GameObject[] gos= Selection.gameObjects;
        BuildingModelInfo building = null;
        foreach(var go in gos)
        {
            BuildingModelInfo b = go.GetComponent<BuildingModelInfo>();
            if (b != null)
            {
                building = b;
            }
        }
        if (building == null)
        {
            Debug.LogError("MoveSelectionToBuildingIn building == null");
            return;
        }
        if (building.InPart == null)
        {
            Debug.LogError("MoveSelectionToBuildingIn building.InPart == null");
            return;
        }
        foreach (var go in gos)
        {
            if (go == building.gameObject) continue;
            go.transform.SetParent(building.InPart.transform);
        }

        Debug.LogError($"MoveSelectionToBuildingIn building : {building}");
    }
    [MenuItem("SceneTools/Building/MoveSelectionToBuildingOut0")]
    public static void MoveSelectionToBuildingOut0()
    {
        GameObject[] gos = Selection.gameObjects;
        BuildingModelInfo building = null;
        foreach (var go in gos)
        {
            BuildingModelInfo b = go.GetComponent<BuildingModelInfo>();
            if (b != null)
            {
                building = b;
            }
        }
        if (building == null)
        {
            Debug.LogError("MoveSelectionToBuildingOut0 building == null");
            return;
        }
        if (building.OutPart0 == null)
        {
            Debug.LogError("MoveSelectionToBuildingOut0 building.OutPart0 == null");
            return;
        }
        foreach (var go in gos)
        {
            if (go == building.gameObject) continue;
            go.transform.SetParent(building.OutPart0.transform);
        }

        Debug.LogError($"MoveSelectionToBuildingOut0 building : {building}");
    }
    [MenuItem("SceneTools/Building/MoveSelectionToBuildingLODs")]
    public static void MoveSelectionToBuildingLODs()
    {
        GameObject[] gos = Selection.gameObjects;
        BuildingModelInfo building = null;
        foreach (var go in gos)
        {
            BuildingModelInfo b = go.GetComponent<BuildingModelInfo>();
            if (b != null)
            {
                building = b;
            }
        }
        if (building == null)
        {
            Debug.LogError("MoveSelectionToBuildingLODs building == null");
            return;
        }
        if (building.LODPart == null)
        {
            Debug.LogError("MoveSelectionToBuildingLODs building.LODPart == null");
            return;
        }
        foreach (var go in gos)
        {
            if (go == building.gameObject) continue;
            go.transform.SetParent(building.LODPart.transform);
        }

        Debug.LogError($"MoveSelectionToBuildingLODs building : {building}");
    }
    #endregion

    #region Material

    [MenuItem("SceneTools/Material/Check")]
    public static void CheckMaterials()
    {
        CheckMaterialsInner(false);
    }

    [MenuItem("SceneTools/Material/Save")]
    public static void SaveMaterials()
    {
        CheckMaterialsInner(true);
    }

    [MenuItem("SceneTools/Material/Load")]
    public static void LoadMaterials()
    {
        Dictionary<string, GameObject> path2Go = new Dictionary<string, GameObject>();
        GameObject root = Selection.activeGameObject;
        MeshRenderer[] renderers = null;
        if (root == null)
        {
            renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        }
        else
        {
            renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        }
        foreach(var render in renderers)
        {
            string path = render.transform.GetPath();
            if (path2Go.ContainsKey(path))
            {
                Debug.LogError($"LoadMaterials path2Go.ContainsKey(path) path:{path} renderer:{render}");
            }
            else
            {
                path2Go.Add(path, render.gameObject);
            }
            
        }
        int nullMatCount = 0;
        int nullMatCount1 = 0;
        int nullMatCount2 = 0;
        int nullMatCount3 = 0;
        int newMatCount = 0;
        //CheckMaterialsInner(true);
        Mat2GosAsset asset = Mat2GosAsset.Load();
        for (int i = 0; i < asset.Items.Count; i++)
        {
            Mat2Gos item = asset.Items[i];
            Material mat = item.mat;
            Material mat1 = item.GetMat();
            Material mat2 = item.GetMat2();
            Material mat3 = item.GetMat3();
            if (mat == null)
            {
                

                if (mat2 != null)
                {
                    mat = mat2;
                }
                else if (mat3 != null)
                {
                    mat = mat3;
                }
                else
                {
                    mat = mat1;
                }
                Debug.LogError($"LoadMaterials[{i}] mat2==null mat:{mat} mat1:{mat1} mat2:{mat2} mat3:{mat3} matName:{item.name} mathPath:{item.file} paths:{item.pathList.Count}");
            }
            
            for (int i1 = 0; i1 < item.pathList.Count; i1++)
            {
                string path = item.pathList[i1];
                if (path2Go.ContainsKey(path))
                {
                    GameObject go = path2Go[path];
                    MeshRenderer mr = go.GetComponent<MeshRenderer>();
                    if (mr == null)
                    {
                        Debug.LogError($"LoadMaterials[{i},{i1}] mr==null path:{path} matName:{item.name} path:{path}");
                    }
                    else
                    {
                        if (mr.sharedMaterial == mat)
                        {
                            
                        }
                        else if (mr.sharedMaterial == null)
                        {
                            //var mat2 = item.GetMat2();
                            //var mat3 = item.GetMat3();
                            nullMatCount++;
                            Debug.LogWarning($"LoadMaterials[{nullMatCount}={nullMatCount1}+{nullMatCount2}+{nullMatCount3}][{i},{i1}] NullMat path:{path} matName:{item.name} path:{path}");
                            if (mat2 != null)
                            {
                                nullMatCount2++;
                                mr.sharedMaterial = mat2;
                            }
                            else if (mat3 != null)
                            {
                                nullMatCount3++;
                                mr.sharedMaterial = mat3;
                            }
                            else
                            {
                                nullMatCount1++;
                                mr.sharedMaterial = mat;
                            }
                            
                        }
                        else
                        {
                            newMatCount++;
                            Debug.LogWarning($"LoadMaterials[{newMatCount}][{i},{i1}] NewMat path:{path} matName:{item.name} path:{path}");
                            //mr.sharedMaterial = mat;
                        }
                        
                    }
                }
                else
                {
                    //Debug.LogError($"LoadMaterials[{i},{i1}] path2Go.ContainsKey(path)==false path:{path} matName:{item.name} path:{path}");
                }
            }
            Debug.Log($"LoadMaterials[{i}] mat2==null matName:{item.name} mathPath:{item.file} paths:{item.pathList.Count}");
        }
        Debug.LogError($"LoadMaterials root:{root} renderers:{renderers.Length} mats:{asset.Items.Count} nullMat:[{nullMatCount}={nullMatCount1}+{nullMatCount2}+{nullMatCount3}] newMat:{newMatCount} paths:{path2Go.Count}");
    }

    private static void CheckMaterialsInner(bool isSave)
    {
        GameObject root = Selection.activeGameObject;
        
        MeshRenderer[] renderers = null;
        if (root == null)
        {
            renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        }
        else
        {
            renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        }
        var mat2Gos = Mat2GosAsset.GetMat2GosDict(renderers);

        if (isSave)
        {
            Mat2GosAsset.Save(mat2Gos);
        }

        Debug.Log($"SaveMaterials renderers:{renderers.Length} mats:{mat2Gos.Count}");
    }

    #endregion

    #region Hierarchy
    [MenuItem("SceneTools/Hierarchy/InitIds")]
    public static void InitIds()
    {
        IdDictionary.InitInfos();
    }

    [MenuItem("SceneTools/Hierarchy/Init")]
    public static void InitHierarchy()
    {
        HierarchyHelper.InitHierarchy();
    }

    [MenuItem("SceneTools/Hierarchy/Clear")]
    public static void ClearHierarchy()
    {
        HierarchyHelper.ClearHierarchy();
    }

    [MenuItem("SceneTools/Hierarchy/Save")]
    public static void SaveHierarchy()
    {
        HierarchyHelper.SaveHierarchy();
    }

    [MenuItem("SceneTools/Hierarchy/Load")]
    public static void LoadHierarchy()
    {
        HierarchyHelper.LoadHierarchy_All(true);
    }

    [MenuItem("SceneTools/Hierarchy/Check")]
    public static void CheckHierarchy()
    {
        HierarchyHelper.CheckHierarchy();
    }


    #endregion

    #region Debug
    [MenuItem("SceneTools/Debug/Selections")]
    public static void ShowSelections()
    {
        var objs=Selection.objects;
        for (int i = 0; i < objs.Length; i++)
        {
            UnityEngine.Object obj = objs[i];
            Debug.Log($"ShowSelections[{i}] obj:{obj}");
        }
    }
    #endregion

    #region TreeNode

    [MenuItem("SceneTools/TreeNode/CheckTreeNodeRendererIds")]
    public static void CheckTreeNodeRendererIds()
    {
        AreaTreeNode[] nodes = GameObject.FindObjectsOfType<AreaTreeNode>(true);
        Dictionary<string, AreaTreeNode> Id2NodeDict = new Dictionary<string, AreaTreeNode>();
        int count = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            AreaTreeNode node = nodes[i];
            foreach (var id in node.RenderersId)
            {
                if (string.IsNullOrEmpty(id)) continue;
                if (!Id2NodeDict.ContainsKey(id))
                {
                    Id2NodeDict.Add(id, node);
                }
                else
                {
                    count++;
                    AreaTreeNode node2 = Id2NodeDict[id];
                    if (node2 != node)
                    {
                        Debug.LogError($"AddIdNodeDict Id2NodeDict.ContainsKey(id)[{count}] id:{id} node1:{node} node2:{node2} [path1:{node.transform.GetPath()}] [path2:{node2.transform.GetPath()}] ");
                    }
                    else
                    {
                        Debug.LogWarning($"AddIdNodeDict Id2NodeDict.ContainsKey(id)[{count}] node==node2 id:{id} node1:{node} node2:{node2} [path1:{node.transform.GetPath()}] [path2:{node2.transform.GetPath()}] ");
                    }

                }
            }
        }
        Debug.Log($"CheckTreeNodeRendererIds nodes:{nodes.Length}");
        //MeshRendererInfoEx
    }

    [MenuItem("SceneTools/TreeNode/ClearRenderers")]
    public static void ClearTreeNodeRenderers()
    {
        AreaTreeNode[] nodes = GameObject.FindObjectsOfType<AreaTreeNode>(true);
        foreach(var node in nodes)
        {
            node.Renderers.RemoveAll(i => i == null);
        }
        Debug.Log($"ClearTreeNodeRenderers nodes:{nodes.Length}");
        //MeshRendererInfoEx
    }
    #endregion

    #region MeshRenderer
    [MenuItem("SceneTools/MeshRenderer/ClearRenderers")]
    public static void ClearMeshRendererInfoEx()
    {
        MeshRendererInfoEx[] nodes = GameObject.FindObjectsOfType<MeshRendererInfoEx>(true);
        foreach (var node in nodes)
        {
            node.RemoveNull();
        }
        Debug.Log($"ClearTreeNodeRenderers nodes:{nodes.Length}");
        //MeshRendererInfoEx
    }

    //
    #endregion

    #region NavisModelRoot
    [MenuItem("SceneTools/NavisModelRoot/ClearModelDict")]
    public static void ClearNavisModelRoots()
    {
        NavisModelRoot[] nodes = GameObject.FindObjectsOfType<NavisModelRoot>(true);
        foreach (var node in nodes)
        {
            node.BimDict = new BIMModelInfoDictionary();
            node.ModelDict = new ModelItemInfoDictionary();
            node.model2TransformResult = new Model2TransformResult();
        }
        Debug.Log($"ClearTreeNodeRenderers nodes:{nodes.Length}");
    }

    //NavisModelRoot
    #endregion

    #region GameObject
    [MenuItem("SceneTools/GameObject/Copy")]
    public static void CopyGameObject()
    {
        GameObject go=MeshHelper.CopyGO(Selection.activeGameObject);
        go.name += "_Copy";
    }
    [MenuItem("SceneTools/GameObject/ShowAll")]
    public static void ShowAllGameObjects()
    {
        Transform[] ts = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);
        foreach(var t in ts)
        {
            t.gameObject.SetActive(true);
        }
        Debug.Log($"ShowAllGameObjects ts:{ts.Length}");
    }
    #endregion

    #region Pipe
    [MenuItem("SceneTools/PipeSystem/OneKeyEx(Job)")]
    public static void PipeOneKeyExJob()
    {
        PipeFactory.Instance.Target = Selection.activeGameObject;
        PipeFactory.Instance.OneKeyEx(true);
    }
    [MenuItem("SceneTools/PipeSystem/Generate")]
    public static void PipeGenerate()
    {
        PipeFactory.Instance.Target = Selection.activeGameObject;
        PipeFactory.Instance.OneKey_Generate(true);
    }
    [MenuItem("SceneTools/PipeSystem/Setting")]
    public static void PipeSetting()
    {
        EditorHelper.SelectObject(PipeFactory.Instance.gameObject);
    }

    [MenuItem("SceneTools/PipeSystem/Window")]
    public static void ShowWindow()
    {
        PipeFactoryEditorWindow.ShowWindow();
    }

    #endregion

    #region LOD 

    [MenuItem("SceneTools/LOD/CreateLODOfDevs(螺丝)")]
    public static void CreateLODOfDevs_Luosi()
    {
        GameObject go = Selection.activeGameObject;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            if (ProgressBarHelper.DisplayCancelableProgressBar("CreateLODOfDevs", i, go.transform.childCount))
            {
                break;
            }
            if (child.childCount > 0)
            {
                LODGroupInfo lODGroupInfo = child.gameObject.AddMissingComponent<LODGroupInfo>();
                lODGroupInfo.NewLODs("螺丝");
            }
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/LOD/RepairLOD0(路灯)")]
    public static void RepairLOD0Light()
    {
        GameObject root = Selection.activeGameObject;
        LODGroup[] lodGroups = root.GetComponentsInChildren<LODGroup>(true);
        int count = 0;
        for (int i = 0; i < lodGroups.Length; i++)
        {
            LODGroup group = lodGroups[i];
            LOD[] lods = group.GetLODs();
            //LOD lod0 = lods[0];
            bool isError = false;
            foreach (var renderer in lods[0].renderers)
            {
                if (renderer == null)
                {
                    isError = true;
                    break;
                }
            }
            if (isError)
            {
                MeshRenderer[] renderers = group.GetComponentsInChildren<MeshRenderer>(true);
                bool isFound = false;
                MeshRenderer mr0 = null;
                foreach(var renderer in renderers)
                {
                    if (renderer.name.Contains("LOD0"))
                    {
                        mr0 = renderer;
                        lods[0].renderers = new Renderer[] { renderer };
                        isFound = true;
                        break;
                    }
                }
                group.SetLODs(lods);
                Debug.LogError($"RepairLOD0Light[{count++}] isFound:{isFound} renderers:{renderers.Length} mr0:{mr0}");
            }
        }
        Debug.LogError($"RepairLOD0Light root:{root} lodGroups:{lodGroups.Length} count:{count}");
    }

    [MenuItem("SceneTools/LOD/SetIgnoreDynamicCulling")]
    public static void SetIgnoreDynamicCulling()
    {
        LODGroup[] lodGroups = GameObject.FindObjectsOfType<LODGroup>(true);
        LODHelper.SetIgnoreDynamicCulling(lodGroups);
    }

    [MenuItem("SceneTools/LOD/SetDoorLOD2Percent")]
    public static void SetDoorLOD2Percent()
    {
        GameObject go = Selection.activeGameObject;
        try
        {
            LODGroup[] lodGroups = go.GetComponentsInChildren<LODGroup>(true);
            int doorLODGroupCount = 0;
            foreach (var lodGroup in lodGroups)
            {
                if (lodGroup.name.ToLower().Contains("door"))
                {
                    doorLODGroupCount++;

                    LOD[] lods = lodGroup.GetLODs();
                    lods[2].screenRelativeTransitionHeight = 0.002f;
                    lodGroup.SetLODs(lods);
                }
            }
            Debug.LogError($"SetDoorLOD2Percent go:{go} lodGroups:{lodGroups.Length} doorLODGroupCount:{doorLODGroupCount}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SetDoorLOD2Percent go:{go} Exception:{ex}");
        }
    }

    [MenuItem("SceneTools/LOD/SetDoorLOD2Mat")]
    public static void SetDoorLOD2Mat()
    {
        GameObject go = Selection.activeGameObject;
        try
        {
            LODGroup[] lodGroups = go.GetComponentsInChildren<LODGroup>(true);
            int errorCount1 = 0;
            int errorCount2 = 0;
            int doorLODGroupCount = 0;
            foreach (var lodGroup in lodGroups)
            {
                if (lodGroup.name.ToLower().Contains("door"))
                {
                    doorLODGroupCount++;

                    LOD[] lods = lodGroup.GetLODs();
                    LOD lod2 = lods[2];
                    if (lod2.renderers.Length == 1)
                    {
                        MeshRenderer renderer = lod2.renderers[0] as MeshRenderer;
                        if (renderer.sharedMaterials.Length == 2)
                        {
                            if(renderer.sharedMaterials[0].name =="door")
                            {
                                renderer.sharedMaterials = new Material[] { renderer.sharedMaterials[0] };
                                errorCount1++;
                            }
                            else if (renderer.sharedMaterials[1].name == "door")
                            {
                                renderer.sharedMaterials = new Material[] { renderer.sharedMaterials[1] };
                                errorCount2++;
                            }
                        }
                    }
                }
            }
            Debug.LogError($"SetDoorLOD2Mat go:{go} lodGroups:{lodGroups.Length} doorLODGroupCount:{doorLODGroupCount} errorCount1:{errorCount1} errorCount2:{errorCount2}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SetDoorLOD2Mat go:{go} Exception:{ex}");
        }
    }

    [MenuItem("SceneTools/LOD/ClearDoorBoundBox")]
    public static void ClearDoorBoundBox()
    {
        GameObject go = Selection.activeGameObject;
        try
        {
            LODGroup[] lodGroups = go.GetComponentsInChildren<LODGroup>(true);
            int errorCount = 0;
            int doorLODGroupCount = 0;
            foreach (var lodGroup in lodGroups)
            {
                if (lodGroup.name.ToLower().Contains("door"))
                {
                    doorLODGroupCount ++;

                    List<MeshRenderer> renderers = lodGroup.GetComponentsInChildren<MeshRenderer>(true).ToList();
                    for (int i = 0; i < renderers.Count; i++)
                    {
                        MeshRenderer r = renderers[i]; 
                        if (r.name.Contains("door_Bounds")|| r.name.Contains("New_Bounds"))
                        {
                            BoundsBox bb = r.GetComponent<BoundsBox>();
                            if (bb)
                            {
                                GameObject.DestroyImmediate(bb);
                            }
                        }  
                        if(r.name.Contains("_Bounds") && !r.name.Contains("_Bounds_LOD0"))
                        {
                            BoundsBox bb = r.GetComponent<BoundsBox>();
                            if (bb)
                            {
                                GameObject.DestroyImmediate(bb);
                            }
                        }
                    }
                }
            }
            Debug.LogError($"RepairDoorLODGroup go:{go} lodGroups:{lodGroups.Length} doorLODGroupCount:{doorLODGroupCount} errorCount:{errorCount}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"RepairDoorLODGroup go:{go} Exception:{ex}");
        }
    }

    [MenuItem("SceneTools/LOD/CheckDoorLODGroup")]
    public static void CheckDoorLODGroup()
    {
        RepairDoorLODGroupInner(false,false);
    }

    [MenuItem("SceneTools/LOD/RepairDoorLODGroup")]
    public static void RepairDoorLODGroup()
    {
        RepairDoorLODGroupInner(true, false);
    }

    [MenuItem("SceneTools/LOD/ResetDoorLODGroup")]
    public static void ResetDoorLODGroup()
    {
        RepairDoorLODGroupInner(true, true);
    }

    [MenuItem("SceneTools/LOD/ChangeDoorLODGroupPercent4")]
    public static void ChangeDoorLODGroupPercent4()
    {
        ChangeDoorLODGroupPercentInner(0.4f);
    }

    [MenuItem("SceneTools/LOD/ChangeDoorLODGroupPercent5")]
    public static void ChangeDoorLODGroupPercent5()
    {
        ChangeDoorLODGroupPercentInner(0.5f);
    }

    public static void RepairDoorLODGroupInner(bool isRepair,bool isForceUpdate)
    {
        GameObject go = Selection.activeGameObject;
        try
        {
            LODGroup[] lodGroups = go.GetComponentsInChildren<LODGroup>(true);
            int errorCount = 0;
            int doorLODGroupCount = 0;
            foreach (var lodGroup in lodGroups)
            {
                if (lodGroup.name.ToLower().Contains("door"))
                {
                    doorLODGroupCount = 0;
                    LOD[] lods = lodGroup.GetLODs();
                    if (lods[0].renderers.Length == 0 || isForceUpdate)
                    {
                        Debug.LogError($"RepairDoorLODGroup1 doorLODGroup Error:{lodGroup.name} path:{lodGroup.transform.GetPath()}");
                        errorCount++;

                        if (isRepair)
                        {
                            List<MeshRenderer> renderers = lodGroup.GetComponentsInChildren<MeshRenderer>(true).ToList();
                            for (int i = 0; i < renderers.Count; i++)
                            {
                                MeshRenderer r = renderers[i];
                                if (r.name.Contains("Bounds"))
                                {
                                    renderers.Remove(r);
                                }
                            }
                            lods[0].renderers = renderers.ToArray() ;
                            lodGroup.SetLODs(lods);
                        }
                    }
                    if (lods[1].renderers.Length == 0 || isForceUpdate)
                    {
                        Debug.LogError($"RepairDoorLODGroup2 doorLODGroup Error:{lodGroup.name} path:{lodGroup.transform.GetPath()}");
                        errorCount++;

                        if (isRepair)
                        {
                            List<MeshRenderer> renderers = lodGroup.GetComponentsInChildren<MeshRenderer>(true).ToList();
                            for (int i = 0; i < renderers.Count; i++)
                            {
                                MeshRenderer r = renderers[i];
                                if (r.name.Contains("Bounds"))
                                {
                                    renderers.Remove(r);
                                    i--;
                                }
                            }
                            MeshRendererInfoList list = new MeshRendererInfoList(renderers);
                            list.RemoveAt(0);
                            lods[1].renderers = list.GetRenderers().ToArray();
                            lodGroup.SetLODs(lods);
                        }
                    }
                    if (lods[2].renderers.Length == 0 || isForceUpdate)
                    {
                        Debug.LogError($"RepairDoorLODGroup3 doorLODGroup Error:{lodGroup.name} path:{lodGroup.transform.GetPath()}");
                        errorCount++;

                        if (isRepair)
                        {
                            List<MeshRenderer> renderers = lodGroup.GetComponentsInChildren<MeshRenderer>(true).ToList();
                            for (int i = 0; i < renderers.Count; i++)
                            {
                                MeshRenderer r = renderers[i];
                                if (!r.name.Contains("door_Bounds"))
                                {
                                    renderers.Remove(r);
                                    i--;
                                }
                            }
                            lods[2].renderers = renderers.ToArray();
                            lodGroup.SetLODs(lods);
                        }
                    }
                }
            }
            Debug.LogError($"RepairDoorLODGroup go:{go} lodGroups:{lodGroups.Length} doorLODGroupCount:{doorLODGroupCount} errorCount:{errorCount}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"RepairDoorLODGroup go:{go} Exception:{ex}");
        }
    }


    public static void ChangeDoorLODGroupPercentInner(float percent)
    {
        GameObject go = Selection.activeGameObject;
        try
        {
            LODGroup[] lodGroups = go.GetComponentsInChildren<LODGroup>(true);
            int doorLODGroupCount = 0;
            foreach (var lodGroup in lodGroups)
            {
                if (lodGroup.name.ToLower().Contains("door"))
                {
                    doorLODGroupCount = 0;
                    LOD[] lods = lodGroup.GetLODs();
                    lods[0].screenRelativeTransitionHeight = percent;
                    lodGroup.SetLODs(lods);
                }
            }
            Debug.LogError($"ChangeDoorLODGroupPercent4 go:{go} lodGroups:{lodGroups.Length} doorLODGroupCount:{doorLODGroupCount}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"ChangeDoorLODGroupPercent4 go:{go} Exception:{ex}");
        }
    }

    [MenuItem("SceneTools/LOD/SaveDoorLODGroupScenes")]
    public static void SaveDoorLODGroupScenes()
    {
        GameObject go = Selection.activeGameObject;
        try
        {
            LODGroupInfo[] lodGroups = go.GetComponentsInChildren<LODGroupInfo>(true);
            int doorLODGroupCount = 0;
            List<LODGroupInfo> doorLodGroups = new List<LODGroupInfo>();
            for (int i = 0; i < lodGroups.Length; i++)
            {
                LODGroupInfo lodGroup = lodGroups[i];
                if (lodGroup.name.ToLower().Contains("door"))
                {
                    doorLODGroupCount++;
                    //lodGroup.EditorCreateScene();
                    doorLodGroups.Add(lodGroup);
                }
            }
            for (int i = 0; i < doorLodGroups.Count; i++)
            {
                LODGroupInfo lodGroup = doorLodGroups[i];
                lodGroup.EditorCreateScene();
                ProgressBarHelper.DisplayCancelableProgressBar("Save", i, lodGroups.Length);
            }
            EditorHelper.ClearOtherScenes();
            EditorHelper.RefreshAssets();
            Debug.LogError($"SaveDoorLODGroupScenes go:{go} lodGroups:{lodGroups.Length} doorLODGroupCount:{doorLODGroupCount}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveDoorLODGroupScenes go:{go} Exception:{ex}");
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/LOD/LoadDoorLODGroupScenes")]
    public static void LoadDoorLODGroupScenes()
    {
        GameObject go = Selection.activeGameObject;
        try
        {
            LODGroupInfo[] lodGroups = go.GetComponentsInChildren<LODGroupInfo>(true);
            int doorLODGroupCount = 0;
            List<LODGroupInfo> doorLodGroups = new List<LODGroupInfo>();
            for (int i = 0; i < lodGroups.Length; i++)
            {
                LODGroupInfo lodGroup = lodGroups[i];
                if (lodGroup.name.ToLower().Contains("door"))
                {
                    doorLODGroupCount++;
                    //lodGroup.EditorCreateScene();
                    doorLodGroups.Add(lodGroup);
                }
            }
            for (int i = 0; i < doorLodGroups.Count; i++)
            {
                LODGroupInfo lodGroup = doorLodGroups[i];
                lodGroup.EditorLoadScene();
                ProgressBarHelper.DisplayCancelableProgressBar("Load", i, lodGroups.Length);
            }
            EditorHelper.ClearOtherScenes();
            EditorHelper.RefreshAssets();
            Debug.LogError($"LoadDoorLODGroupScenes go:{go} lodGroups:{lodGroups.Length} doorLODGroupCount:{doorLODGroupCount}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadDoorLODGroupScenes go:{go} Exception:{ex}");
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/LOD/AddSelectionToLOD0")]
    public static void AddSelectionToLOD0()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>(true);
        LODGroup lodGroup = go.GetComponentInParent<LODGroup>();
        //LODGroup lodGroup = CommonUtils.ObjectExtension.FindComponentInParent<LODGroup>(go);
        if (renderers.Length == 0)
        {
            Debug.LogError($"renderers.Length == 0 go:{go} parent:{go.transform.parent}");
            return;
        }
        if (lodGroup==null)
        {
            Debug.LogError($"lodGroup == null go:{go} parent:{go.transform.parent}");
            return;
        }

        LOD[] lods = lodGroup.GetLODs();
        List<Renderer> lod0Renderers = lods[0].renderers.ToList();
        foreach(var renderer in renderers)
        {
            if (!lod0Renderers.Contains(renderer))
            {
                lod0Renderers.Add(renderer);
            }
        }
        lods[0].renderers = lod0Renderers.ToArray();
        lodGroup.SetLODs(lods);

        Debug.LogError($"AddSelectionToLOD0 lodGroup:{lodGroup} renderers:{renderers.Length}");
    }

    [MenuItem("SceneTools/LOD/Clear")]
    public static void ClearLODGroup2()
    {
        ClearComponents<LODGroupInfo>();
        ClearComponents<LODGroup>();
    }

    [MenuItem("SceneTools/LOD/SetSetting")]
    public static void LODSetSetting()
    {
        EditorHelper.SelectObject(LODManager.Instance.gameObject);
    }
    [MenuItem("SceneTools/LOD/SetLODDev")]
    public static void SetLODDev()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            ProgressArg pA = new ProgressArg("SetLODDev", i, Selection.gameObjects.Length, obj);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            LODManager.Instance.CreateBoxLOD(obj);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/LOD/SetLODBoxMat")]
    public static void SetLODBoxMat()
    {
        DateTime start = DateTime.Now;
        int count = 0;
        var lodMat = LODManager.Instance.LODBoxMat;
        if (lodMat == null)
        {
            Debug.LogError("SetLODBoxMat lodMat == null");
            return;
        }
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            ProgressArg pA = new ProgressArg("SetLODBoxMat", i, renderers.Length, r);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            if (r.name.Contains("_LODBox"))
            {
                r.sharedMaterial = lodMat;
                count++;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SetLODBoxMat count:{count} renderers:{renderers.Length}");
    }

    [MenuItem("SceneTools/LOD/LODManager")]
    public static void SelectLODManager()
    {
        EditorHelper.SelectObject(LODManager.Instance);
    }

    [MenuItem("SceneTools/LOD/RemoveOthers")]
    public static void RemoveLODGroupOthers()
    {
        LODHelper.RemoveLODGroupOthers();
    }


    [MenuItem("SceneTools/LOD/GetLODGroups")]
    public static void GetLODGroups()
    {
        LODGroup[] groups = GameObject.FindObjectsOfType<LODGroup>(true);
        GameObject groupRoot = new GameObject("LODGroupList");
        for (int i = 0; i < groups.Length; i++)
        {
            LODGroup group = groups[i];
            EditorHelper.UnpackPrefab(groupRoot.gameObject);
            group.transform.SetParent(groupRoot.transform);
            ProgressArg pA = new ProgressArg("GetLODGroups", i, groups.Length, group);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetLODGroups groups:{groups.Length}");
    }

    [MenuItem("SceneTools/LOD/ClearLODGroups")]
    public static void ClearLODGroups()
    {
        LODHelper.ClearLODGroups(Selection.gameObjects);
    }

    [MenuItem("SceneTools/LOD/RemoveSameNameObjects")]
    public static void RemoveSameNameObjects()
    {
        GameObject go0 = Selection.activeGameObject;
        Transform parent = go0.transform.parent;
        List<MeshRenderer> renderers2 = new List<MeshRenderer>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.gameObject == go0)
            {

            }
            else
            {
                MeshRenderer[] renderers0 = child.GetComponentsInChildren<MeshRenderer>(true);
                renderers2.AddRange(renderers0);
            }
        }
        MeshRenderer[] renderers1 = go0.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < renderers1.Length; i++)
        {
            MeshRenderer rend = renderers1[i];
            if (!rend.name.Contains("MemberPartPrismatic")) continue;
            ProgressArg p1 = new ProgressArg("Remove", i, renderers1.Length);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                break;
            }
            List<MeshRenderer> sameNameList = new List<MeshRenderer>();
            foreach(var item in renderers2)
            {
                if (item == null) continue;
                if(item.name==rend.name)
                {
                    var dis = MeshHelper.GetCenterDistance(item.gameObject, rend.gameObject);
                    if (dis < 1f)
                    {
                        sameNameList.Add(item);
                    }
                    else
                    {
                        Debug.LogError($"RemoveSameNameObjects1 dis > 1f :{sameNameList.Count} name:{rend.name} path:{item.transform.GetPath()}");
                    }
                }
            }
            if (sameNameList.Count == 0)
            {
                bool isBreak = false;
                float minDis = float.MaxValue;
                MeshRenderer minRenderer = null;
                for (int i1 = 0; i1 < renderers2.Count; i1++)
                {
                    MeshRenderer item = renderers2[i1];
                    if (item == null) continue;
                    if (!item.name.Contains("MemberPartPrismatic")) continue;
                    ProgressArg p2 = new ProgressArg("GetDis", i1, renderers2.Count);
                    p1.AddSubProgress(p2);
                    if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                    {
                        isBreak = true;
                        break;
                    }
                    var dis = MeshHelper.GetCenterDistance(item.gameObject, rend.gameObject);
                    if (dis < minDis)
                    {
                        minDis = dis;
                        minRenderer = item;
                    }
                    //if (dis < 0.0001f)
                    //{
                    //    sameNameList.Add(item);
                    //}
                    //else
                    //{
                    //    Debug.LogError($"RemoveSameNameObjects2 dis > 0.0001f dis:{dis} :{sameNameList.Count} name:{rend.name} path:{item.transform.GetPath()}");
                    //}
                }

                if (minDis < 0.0005f)
                {
                    sameNameList.Add(minRenderer);
                }
                else
                {
                    Debug.LogError($"RemoveSameNameObjects2 minRenderer > 0.0005f minDis:{minDis} minRenderer:{minRenderer} :{sameNameList.Count} name:{rend.name} path:{minRenderer.transform.GetPath()}");
                }

                if (isBreak)
                {
                    break;
                }
            }
            if (sameNameList.Count == 1)
            {
                GameObject.DestroyImmediate(sameNameList[0].gameObject);
                Debug.Log($"RemoveSameNameObjects :{rend.name}");
            }
            else
            {
                Debug.LogError($"RemoveSameNameObjects sameNameList.Count != 1 :{sameNameList.Count} name:{rend.name} ");
            }
        }
        ProgressBarHelper.ClearProgressBar();
    }


    [MenuItem("SceneTools/LOD/ClearLODGroupsByKey_MemberPartPrismatic(MemberPartPrismatic)")]
    public static void ClearLODGroupsByKey_MemberPartPrismatic()
    {
        LODHelper.ClearLODGroupsByKey(Selection.gameObjects, "MemberPartPrismatic");
        MeshHelper.DecreaseEmptyGroupEx(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/LOD/ClearRenderersByKey_MemberPartPrismatic(MemberPartPrismatic)")]
    public static void ClearRenderersByKey_MemberPartPrismatic()
    {
        MeshRenderer[] renderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
        List<MeshRenderer> keyRenderers = new List<MeshRenderer>();
        foreach(var renderer in renderers)
        {
            if (renderer.name.Contains("MemberPartPrismatic") && renderer.transform.childCount>0)
            {
                keyRenderers.Add(renderer);
            }
        }
        foreach(var renderer in keyRenderers)
        {
            renderer.gameObject.ClearChildren();
        }
        Debug.LogError($"ClearRenderersByKey_MemberPartPrismatic renderers:{renderers.Length} keyRenderers:{keyRenderers.Count}");
    }

    [MenuItem("SceneTools/LOD/AddLOD1(U)")]
    public static void AddLOD1_U()
    {
        GameObject go = Selection.activeGameObject;
        Transform t = go.transform;
        if (t.childCount == 1)
        {
            Transform child = t.GetChild(0);
            child.transform.SetParent(go.transform.parent);
            var group = LODManager.Instance.AddLOD1(go, child.gameObject,true);
            //t.transform.SetParent(go.transform);
            //var groupNew = LODHelper.UniformLOD0(group);
        }
        else
        {
            Debug.LogError($"AddLOD1 t.childCount != 1");
        }
    }

    [MenuItem("SceneTools/LOD/AddLOD1")]
    public static void AddLOD1()
    {
        GameObject go = Selection.activeGameObject;
        Transform t = go.transform;
        if (t.childCount == 1)
        {
            Transform child = t.GetChild(0);
            child.transform.SetParent(go.transform.parent);
            var group=LODManager.Instance.AddLOD1(go, child.gameObject,false);
            child.transform.SetParent(go.transform);
            //var groupNew = LODHelper.UniformLOD0(group);
        }
        else
        {
            Debug.LogError($"AddLOD1 t.childCount != 1");
        }
    }

    [MenuItem("SceneTools/LOD/AddLOD2")]
    public static void AddLOD2()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            ProgressArg pA = new ProgressArg("SetLODDev", i, Selection.gameObjects.Length, obj);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            LODManager.Instance.CreateBoxLOD(obj);
        }
        ProgressBarHelper.ClearProgressBar();

    }

    [MenuItem("SceneTools/LOD/AddLOD3")]
    public static void AddLOD3()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            ProgressArg pA = new ProgressArg("SetLODDev", i, Selection.gameObjects.Length, obj);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            LODManager.Instance.CreateBoxLOD(obj);
        }
        ProgressBarHelper.ClearProgressBar();

    }
    #endregion


    #region Mesh 

    [MenuItem("SceneTools/Mesh/MeshSplit")]
    public static void MeshSplit()
    {
        MeshSpliter.SplitGo(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Mesh/MeshSplitSelection")]
    public static void MeshSplitSelection()
    {
        DateTime start = DateTime.Now;
        GameObject go = Selection.activeGameObject;
        MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);
        GameObject newRoot = new GameObject(go.name + "_Split");
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            ProgressArg p0 = new ProgressArg("Split", i, mfs.Length);
            GameObject newGo=MeshSpliter.SplitGo(mf.gameObject,p0);
            newGo.transform.SetParent(newRoot.transform);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"MeshSplitSelection go:{go} mfs:{mfs.Length} time:{DateTime.Now-start}");
    }

    [MenuItem("SceneTools/Mesh/MeshSplitSelection(Job)")]
    public static void MeshSplitSelectionJob()
    {
        DateTime start = DateTime.Now;
        GameObject go = Selection.activeGameObject;
        MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);
        //GameObject newRoot = new GameObject(go.name + "_Split");
        JobList<MeshSplitJob> jobs = new JobList<MeshSplitJob>(100);
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            MeshStructure meshS = new MeshStructure(mf.sharedMesh);
            MeshSplitJob job = new MeshSplitJob()
            {
                id = i,
                mesh = meshS
            };
            jobs.Add(job);
        }
        jobs.CompleteAll();

        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"MeshSplitSelectionJob go:{go} mfs:{mfs.Length} time:{DateTime.Now - start}");
    }

    [MenuItem("SceneTools/Mesh/Select")]
    public static void SelectMesh()
    {
        GameObject go = Selection.activeGameObject;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;
        EditorHelper.SelectObject(mesh);
    }

    [MenuItem("SceneTools/Mesh/CombineMeshLeafs")]
    public static void CombineMeshLeafs()
    {
        Dictionary<Transform, List<Transform>> dict = new Dictionary<Transform, List<Transform>>();
        Dictionary<Transform, List<Transform>> dictError = new Dictionary<Transform, List<Transform>>();

        List<MeshFilter> mfList = new List<MeshFilter>();
        foreach (var obj in Selection.gameObjects)
        {
            EditorHelper.UnpackPrefab(obj);
            //ResetRotation(obj);
            var mfs = obj.GetComponentsInChildren<MeshFilter>(true);
            for (int i = 0; i < mfs.Length; i++)
            {
                MeshFilter mf = mfs[i];
                if (mf.transform.childCount == 0)
                {
                    mfList.Add(mf);
                    var p = mf.transform.parent;
                    if (p == null)
                    {
                        Debug.LogError($"[{i}/{mfs.Length}] p==null mf:{mf}");
                        continue;
                    }

                    if(dictError.ContainsKey(p))
                    {
                        continue;
                    }

                    
                    if (!dict.ContainsKey(p))
                    {
                        if (IsChildrenAllMesh(p))
                        {
                            dict.Add(p, new List<Transform>());
                        }
                        else
                        {
                            dictError.Add(p, new List<Transform>());
                        }
                       
                        Debug.Log($"[{i}/{mfs.Length}] p[{dict.Count}]:{p}");
                    }

                    if (dict.ContainsKey(p))
                    {
                        dict[p].Add(mf.transform);
                    }

                    
                }
            }
            //mfList.AddRange(mfs);

            Debug.Log($"selection:{obj}");
        }

        int count = 0;
        List<Transform> list = dict.Keys.ToList();

        for (int i = 0; i < list.Count; i++)
        {
            Transform p = list[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CombineMeshLeafs", i, list.Count, p)))
            {
                break;
            }
            if (p == null)
            {
                Debug.LogError($"CombineMeshLeafs p == null {i}/{list.Count}");
                continue;
            }
            count += dict[p].Count;
            if (p.gameObject == null)
            {
                Debug.LogError($"CombineMeshLeafs p.gameObject == null {i}/{list.Count}");
                continue;
            }


            MeshCombineHelper.Combine(p.gameObject);
        }

        foreach (var obj in Selection.gameObjects)
        {
            MeshHelper.DecreaseEmptyGroup(obj);
            MeshHelper.DecreaseEmptyGroup(obj);
            MeshHelper.DecreaseEmptyGroup(obj);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"CombineMeshLeafs list:{list.Count} children:{count}");


    }

    private static bool IsChildrenAllMesh(Transform p)
    {
        for(int i = 0; i < p.childCount; i++)
        {
            var t = p.GetChild(i);

            MeshFilter mf = t.GetComponent<MeshFilter>();
            MeshRenderer mr = t.GetComponent<MeshRenderer>();
            MeshCollider mc = t.GetComponent<MeshCollider>();
            if (mf == null && mr == null && mc == null)
            {
                return false;
            }
        }
        return true;
    }


    [MenuItem("SceneTools/Mesh/CheckSharedMesh")]
    public static void CheckSharedMesh()
    {
        MeshFilter[] mfs = GameObject.FindObjectsOfType<MeshFilter>(true);
        int count1 = 0;
        int count2 = 0;
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            //if (mf.mesh == null)
            //{
            //    Debug.LogError($"[{i}/{mfs.Length}][{++count2}] mf.mesh == null name:{mf.name}");
            //}
            if (mf.sharedMesh == null)
            {
                Debug.LogError($"[{i}/{mfs.Length}][{++count1}] mf.sharedMesh == null name:{mf.name}");
            }
        }
        Debug.Log($"SaveMeshAll mfs:{mfs.Length} errorCount:{count1}");
    }

    [MenuItem("SceneTools/Mesh/Save")]
    public static void SaveMesh()
    {
        EditorHelper.SaveMeshAsset(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Mesh/SaveMeshAll_Selection")]
    public static void SaveMeshAll_Selection()
    {
        MeshFilter[] mfs = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>(true);
        //foreach(var mf  in mfs)
        //{
        //    if (UnityEditor.AssetDatabase.Contains(mf.sharedMesh)) continue;
        //}


        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(mfs);
        int count = 0;
        for (int i = 0; i < sharedMeshInfos.Count; i++)
        {
            SharedMeshInfo sharedMesh = sharedMeshInfos[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("Save", i, sharedMeshInfos.Count))
            {
                break;
            }
            if (UnityEditor.AssetDatabase.Contains(sharedMesh.mainMeshFilter.sharedMesh)) continue;
            EditorHelper.SaveMeshAsset(sharedMesh.gameObject);
            count++;
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.Log($"SaveMeshAll sharedMeshInfos:{sharedMeshInfos.Count} count:{count}");
    }

    [MenuItem("SceneTools/Mesh/SaveMeshAll")]
    public static void SaveMeshAll()
    {

        //EditorHelper.SaveMeshAsset(Selection.activeGameObject);

        MeshFilter[] mfs = GameObject.FindObjectsOfType<MeshFilter>(true);
        //foreach(var mf  in mfs)
        //{
        //    if (UnityEditor.AssetDatabase.Contains(mf.sharedMesh)) continue;
        //}


        SharedMeshInfoList sharedMeshInfos = new SharedMeshInfoList(mfs);
        int count = 0;
        for (int i = 0; i < sharedMeshInfos.Count; i++)
        {
            SharedMeshInfo sharedMesh = sharedMeshInfos[i];
            if(ProgressBarHelper.DisplayCancelableProgressBar("Save",i,sharedMeshInfos.Count))
            {
                break;
            }
            if (UnityEditor.AssetDatabase.Contains(sharedMesh.mainMeshFilter.sharedMesh)) continue;
            EditorHelper.SaveMeshAsset(sharedMesh.gameObject);
            count++;
        }

        ProgressBarHelper.ClearProgressBar();

        Debug.Log($"SaveMeshAll sharedMeshInfos:{sharedMeshInfos.Count} count:{count}");
    }

    [MenuItem("SceneTools/Mesh/NewMesh00001")]
    public static void NewMesh00001()
    {
        NewMeshInner(true, 0.00001f);
    }

    [MenuItem("SceneTools/Mesh/NewMesh001")]
    public static void NewMesh001()
    {
        NewMeshInner(true, 0.001f);
    }

    [MenuItem("SceneTools/Mesh/NewMesh005")]
    public static void NewMesh005()
    {
        NewMeshInner(true, 0.005f);
    }

    [MenuItem("SceneTools/Mesh/NewMesh010")]
    public static void NewMesh010()
    {
        NewMeshInner(true, 0.01f);
    }
    [MenuItem("SceneTools/Mesh/NewMesh100")]
    public static void NewMesh100()
    {
        NewMeshInner(true, 0.1f);
    }

    [MenuItem("SceneTools/Mesh/SetMeshName")]
    public static void SetMeshName()
    {
        GameObject go = Selection.activeGameObject;
        MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);
        //int count = 0;
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            if (mf.sharedMesh.name.Contains("Geometry"))
            {
                mf.sharedMesh.name = mf.name;
            }
            
        }
    }

    public static void NewMeshInner(bool isNew,float minDis)
    {
        //CenterPivotAll();

        DateTime startT = DateTime.Now;
        GameObject go = Selection.activeGameObject;
        MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);
        int count = 0;
        for (int i = 0; i < mfs.Length; i++)
        {
            MeshFilter mf = mfs[i];
            //mf.transform.SetParent(null);
            string meshName = mf.name;
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("NewMesh", i, mfs.Length, meshName)))
            {
                break;
            }
            Vector3 center = VertexHelper.GetCenter(mf.gameObject);
            float dis = Vector3.Distance(center, mf.transform.position);
            if (dis > minDis)
            {

                count++;
                Debug.Log($"NewMesh[{i}/{mfs.Length}][{count}] dis:{dis} name:{meshName} ");
                if (isNew)
                {
                    GameObject newGo = VertexHelper.CopyGameObjectMesh(mf);
                    //mf.gameObject.SetActive(false);

                    GameObjectExtension.CopyTransformMesh(newGo, mf.gameObject);
                    newGo.SetActive(false);
                    GameObject.DestroyImmediate(newGo);
                }

            }
            else
            {
                Debug.LogWarning($"NewMesh[{i}/{mfs.Length}] dis:{dis} name:{meshName} ");
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"NewMesh go:{go} mfs:{mfs.Length} count:{count} time:{DateTime.Now - startT}");
    }

    [MenuItem("SceneTools/Mesh/Combine")]
    public static void CombineMesh()
    {
        //MeshCombiner.Instance.CombineToOne(Selection.activeGameObject, true, true);
        MeshCombineHelper.Combine(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Mesh/Combine(Save)")]
    public static void CombineMesh_Save()
    {
        MeshCombiner.Instance.CombineToOne(Selection.activeGameObject, true, true);
        //MeshCombineHelper.Combine(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Mesh/Combine(Not Save & Destroy)")]
    public static void CombineMesh_NotSave()
    {
        MeshCombiner.Instance.CombineToOne(Selection.activeGameObject, false, false);
    }

    [MenuItem("SceneTools/Mesh/SplitByMaterials")]
    public static void SplitMesh()
    {
        MeshCombineHelper.SplitByMaterials(Selection.activeGameObject, false);
    }
    [MenuItem("SceneTools/Mesh/ShowAll")]
    public static void ShowAllMesh()
    {
        var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        SetEnbled(meshRenderers, true);
    }
    [MenuItem("SceneTools/Mesh/HideAll")]
    public static void HideAllMesh()
    {
        var meshRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        SetEnbled(meshRenderers, false);
    }
    [MenuItem("SceneTools/Mesh/ShowSelection")]
    public static void ShowSelectionMesh()
    {
        var meshRenderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
        SetEnbled(meshRenderers, true);
    }
    [MenuItem("SceneTools/Mesh/HideSelection")]
    public static void HideSelectionMesh()
    {
        var meshRenderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
        SetEnbled(meshRenderers, false);
    }

    [MenuItem("SceneTools/Mesh/HideOthers")]
    public static void HideOthersMesh()
    {
        HideAllMesh();
        ShowSelectionMesh();
    }

    private static void SetEnbled(MeshRenderer[] meshRenderers,bool isEnabled)
    {
        foreach (var mr in meshRenderers)
        {
            mr.enabled = isEnabled;
        }
    }

    #endregion

    #region Prefab

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs(Mat)")]
    public static void GetPrefabInfos_Mat()
    {
        GetPrefabInfosInner(0, true);
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs(NoMat)")]
    public static void GetPrefabInfos()
    {
        GetPrefabInfosInner(0,false);
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs(Log)")]
    public static void GetPrefabInfos_Log()
    {
        GetPrefabInfosInner(0, true, true);
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs(Debug)")]
    public static void GetPrefabInfos_Debug()
    {
        
        GetPrefabInfosInner(0, true, true,true);
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabsByGroup")]
    public static void GetPrefabsByGroup()
    {
        AcRTAlignJobSetting.Instance.SetDefault();
        GameObject obj = Selection.activeGameObject;

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform child = obj.transform.GetChild(i);
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPrefabsByGroup", i, obj.transform.childCount, child)))
            {
                break;
            }
            PrefabInstanceBuilder.Instance.GetPrefabsOfList(child);
        }
        MeshNode.InitNodes(obj);
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs0.0001")]
    public static void GetPrefabInfos0001()
    {
        GetPrefabInfosInner(0.0001);
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs0.001")]
    public static void GetPrefabInfos001()
    {
        GetPrefabInfosInner(0.001);
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs0.01")]
    public static void GetPrefabInfos01()
    {
        GetPrefabInfosInner(0.01);
    }

    [MenuItem("SceneTools/Prefab/(*) GetPrefabs0.1")]
    public static void GetPrefabInfos1()
    {
        GetPrefabInfosInner(0.1);
    }

    public static void GetPrefabInfosInner(double zeroP, bool isByMat = true, bool isShowLog = false, bool isDebug = false)
    {
        //if (zeroP > 0)
        //{
        //    PrefabInstanceBuilder.Instance.disSetting.zeroP = zeroP;
        //}
        //PrefabInstanceBuilder.Instance.disSetting.IsByMat = isByMat;
        //PrefabInstanceBuilder.Instance.disSetting.IsShowLog = isShowLog;
        //DistanceSetting.IsDebug = isDebug;

        //AcRTAlignJobSetting.Instance.SetDefault();
        
        ////for (int i = 0; i < Selection.gameObjects.Length; i++)
        ////{
        ////    GameObject obj = Selection.gameObjects[i];
        ////    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPrefabInfos", i, Selection.gameObjects.Length, obj)))
        ////    {
        ////        break;
        ////    }
        ////    PrefabInstanceBuilder.Instance.GetPrefabsOfList(obj);
        ////    MeshNode.InitNodes(obj);
        ////}
        ////ProgressBarHelper.ClearProgressBar();

        //var objs = Selection.gameObjects;
        //PrefabInstanceBuilder.Instance.GetPrefabsOfList(objs);

        //Debug.Log($"GetPrefabInfosInner objs:{objs.Length}");

        PrefabInstanceBuilder.GetSelectionPrefabInfosInner(zeroP, isByMat, isShowLog, isDebug);
    }



    [MenuItem("SceneTools/Prefab/CompareWindow(...)")]
    public static void CompareWindow()
    {
        MeshComparerEditorWindow.ShowWindow();
    }

    [MenuItem("SceneTools/Prefab/ResetPosition")]
    public static void ResetPosition()
    {
        GameObject obj = Selection.activeGameObject;
        MeshFilter[] mfs = obj.GetComponentsInChildren<MeshFilter>(true);
        foreach(var mf in mfs)
        {
            mf.transform.Reset();
        }
        Debug.LogError($"ResetPosition obj:{obj} mfs:{mfs.Length}");
    }

    [MenuItem("SceneTools/Prefab/RemoveNew")]
    public static void PrefabRemoveNew()
    {
        MeshHelper.RemoveNew(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Prefab/CleanErrors")]
    public static void PrefabCleanErrors()
    {
        MeshHelper.DestroyError(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Prefab/SetSetting")]
    public static void PrefabSetSetting()
    {
        EditorHelper.SelectObject(PrefabInstanceBuilder.Instance.gameObject);
    }
    [MenuItem("SceneTools/Prefab/InitMeshNodes")]
    public static void InitMeshNodes()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("InitMeshNodes", i, Selection.gameObjects.Length, obj)))
            {
                break;
            }
            MeshNode.InitNodes(obj);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/Prefab/InitInnerMeshNodes")]
    public static void InitInnerMeshNodes()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("InitMeshNodes", i, Selection.gameObjects.Length, obj)))
            {
                break;
            }
            InnerMeshNode.InitInnerNodes(obj);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos")]
    public static void GetTargetGos()
    {
        GetTargetGos(PrefabInstanceBuilder.Instance.vertexCountOffset);
    }

    [MenuItem("SceneTools/Prefab/InitInstancesDict")]
    public static void InitInstancesDict()
    {
        MeshPrefabInstance.InitInstancesDict();
    }

[MenuItem("SceneTools/Prefab/GetTargetGos(0)")]
    public static void GetTargetGos0()
    {
        GetTargetGos(0);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(5)")]
    public static void GetTargetGos5()
    {
        GetTargetGos(5);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(10)")]
    public static void GetTargetGos10()
    {
        GetTargetGos(10);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(50)")]
    public static void GetTargetGos50()
    {
        GetTargetGos(50);
    }

    [MenuItem("SceneTools/Prefab/GetTargetGos(100)")]
    public static void GetTargetGos100()
    {
        GetTargetGos(100);
    }

    public static void GetTargetGos(int vertexCountOffset)
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPrefabInfos", i, Selection.gameObjects.Length, obj)))
            {
                break;
            }
            var mps = PrefabInstanceBuilder.Instance.FilterMeshPoints(obj);
            var dict = AcRTAlignJobContainer.CreateMeshFilterListDict(mps, vertexCountOffset);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    

    [MenuItem("SceneTools/Prefab/RemoveNew")]
    public static void RemoveNew()
    {
        MeshHelper.RemoveNew(Selection.activeGameObject);
    }

    #endregion

    #region SubScene

    [MenuItem("SceneTools/SubScene/SetBuildings")]
    public static void SetBuildings()
    {
        SubSceneHelper.SetBuildings();
    }
    [MenuItem("SceneTools/SubScene/SetBuildingsWithNavmesh")]
    public static void SetBuildingsWithNavmesh()
    {
        SubSceneHelper.SetBuildingsWithNavmesh(true);
    }
    [MenuItem("SceneTools/SubScene/ClearBuildings")]
    public static void ClearBuildings()
    {
        SubSceneHelper.ClearBuildings();
    }

    [MenuItem("SceneTools/SubScene/GetLODsIds")]
    public static void GetLODsIds()
    {
        IdDictionary.InitInfos();
        var lodsScenes = GameObject.FindObjectsOfType<SubScene_LODs>(true);
        for (int i = 0; i < lodsScenes.Length; i++)
        {
            SubScene_LODs scene = lodsScenes[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetLODsIds", i, lodsScenes.Length, scene)))
            {
                break; 
            }
            RendererId rid = RendererId.GetRId(scene);
            rid.UpdateParent();
            scene.EditorLoadScene();
            
            ////scene.UnLoadSceneAsync();
            scene.UnLoadGosM();
            scene.ShowBounds();
            //scene.EditorCreateScene(true);
        }

        ProgressBarHelper.ClearProgressBar();
        EditorHelper.ClearOtherScenes();
    }

    [MenuItem("SceneTools/SubScene/CheckScenes")]
    public static void CheckScenes()
    {
        SubSceneHelper.CheckSceneIndex(true);
    }

    [MenuItem("SceneTools/SubScene/DeleteOtherRepleatScenes")]
    public static void EditorDeleteOtherRepleatScenes()
    {
        SubSceneManager.Instance.EditorDeleteOtherRepleatScenes(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/LoadBuildingScenes")]
    public static void OneKeyLoadScene()
    {
        BuildingModelManager.Instance.OneKeyLoadScene(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/SaveBuildingScenes")]
    public static void OneKeySaveScene()
    {
        BuildingModelManager.Instance.OneKeySaveScene(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/LoadLODsScene")]
    public static void LoadLODs()
    {
        SubSceneManager.Instance.EditorLoadLODs(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/SaveLODsScene")]
    public static void CreateLODs()
    {
        SubSceneManager.Instance.EditorCreateLODs(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
    }

    [MenuItem("SceneTools/SubScene/CreateSubScenes(LOD)")]
    public static void CreateSubScenes_LOD()
    {
        SubSceneHelper.CreateSubScenes<SubScene_LODs>(Selection.gameObjects);
    }

    [MenuItem("SceneTools/SubScene/CreateSubScenes")]
    public static void CreateSubScenes()
    {
        SubSceneHelper.CreateSubScenes<SubScene_Single>(Selection.gameObjects);
    }

    [MenuItem("SceneTools/SubScene/CreateSubScenes(Children)")]
    public static void CreateSubScenes_Children()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject go = Selection.gameObjects[i];
            for(int j = 0; j < go.transform.childCount; j++)
            {
                GameObject subGo = go.transform.GetChild(j).gameObject;
                if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CreateSubScenes", i, Selection.gameObjects.Length, subGo)))
                {
                    break;
                }
                SubScene_Single subScene = subGo.GetComponent<SubScene_Single>();
                if (subScene == null)
                {
                    subScene = subGo.AddComponent<SubScene_Single>();
                    subScene.IsLoaded = true;
                }
                subScene.EditorCreateScene(true);
            }
        }
        EditorHelper.RefreshAssets();
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/SubScene/Clear")]
    public static void ClearSubScenes()
    {
        ClearComponents<SubScene_Single>();
    }

    [MenuItem("SceneTools/SubScene/ClearSceneArg")]
    public static void ClearSceneArg()
    {
        var scenes = GameObject.FindObjectsOfType<SubScene_Base>(true);
        foreach(var scene in scenes)
        {
            scene.sceneArg.objs.Clear();
        }
        Debug.Log($"ClearSceneArg scenes:{scenes.Length}");
    }

    [MenuItem("SceneTools/SubScene/ClearOtherScenes")]
    public static void ClearOtherScenes()
    {
        EditorHelper.ClearOtherScenes();
    }

    [MenuItem("SceneTools/SubScene/LoadSubScenes(All)")]
    public static void LoadSubScenes_All()
    {
        SubScene_Single[] scenes = GameObject.FindObjectsOfType<SubScene_Single>(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            SubScene_Single scene = scenes[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("LoadSubScenes_All", i, scenes.Length, scene)))
            {
                break;
            }
            scene.EditorLoadSceneEx();
        }
        ProgressBarHelper.ClearProgressBar();
    }

    [MenuItem("SceneTools/SubScene/GetSubScenes")]
    public static void GetSubScenes()
    {
        GameObject obj = Selection.activeGameObject;
        var subScenes = obj.GetComponentsInChildren<SubScene_Base>(true).ToList();
        Debug.Log($"GetSubScenes obj:{obj.name} subScenes:{subScenes.Count} path:{obj.transform.GetPath()}");
    }
    #endregion

    #region ClearComponents

    public static void ClearComponents<T>() where T : Component
    {
        TransformHelper.ClearComponents<T>(Selection.gameObjects);
    }


    [MenuItem("SceneTools/Clear/ClearSceneNullObjs")]
    public static void ClearSceneNullObjs()
    {
        ClearSceneArg();
        ClearTreeNodeRenderers();
        ClearNavisRootComponents();
        ClearNavisModelRoots();//
        ClearMeshRendererInfoEx();
        InitNavisFileInfoByModel.Instance.vueRootModels = new List<NavisPlugins.Infos.ModelItemInfo>();
        SubSceneShowManager.Instance.scenes_Out0 = new List<SubScene_Out0>();
        SubSceneShowManager.Instance.scenes_Out1 = new List<SubScene_Out1>();
    }

    [MenuItem("SceneTools/Clear/BIMObjects")]
    public static void ClearBIMObjects()
    {
        TransformHelper.ClearComponentGos<BIMModelInfo>(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Clear/NotBIMObjects")]
    public static void ClearNotBIMObjects()
    {
        TransformHelper.ClearNotComponentGos<BIMModelInfo>(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Clear/LODGroup")]
    public static void ClearLODGroup()
    {
        ClearComponents<LODGroupInfo>();
        ClearComponents<LODGroup>();
    }

    [MenuItem("SceneTools/Clear/ClearRId")]
    public static void ClearRendererId()
    {
        ClearComponents<RendererId>();
    }

    [MenuItem("SceneTools/Clear/ClearMeshNode")]
    public static void ClearMeshNode()
    {
        ClearComponents<InnerMeshNode>();
    }

    [MenuItem("SceneTools/Clear/DestroyDoors")]
    public static void DestroyDoors()
    {
        DoorsRootList doors =DoorManager.Instance.UpdateAllDoors();
        foreach(DoorsRoot door in doors)
        {
            GameObject.DestroyImmediate(door.gameObject);
        }
    }

    [MenuItem("SceneTools/Clear/DestroyTrees")]
    public static void DestroyTrees()
    {
        //ClearComponents<MeshNode>();

        //TransformHelper.ClearComponentGos<ModelAreaTree>(Selection.activeGameObject);

        TransformHelper.ClearComponentsAllGo<ModelAreaTree>();
    }

    [MenuItem("SceneTools/Clear/ClearMeshNodeAll")]
    public static void ClearMeshNodeAll()
    {
        TransformHelper.ClearComponentsAll<MeshNode>();
    }

    [MenuItem("SceneTools/Clear/ClearRendererInfo")]
    public static void ClearRendererInfo()
    {
        ClearComponents<MeshRendererInfo>();
    }

    [MenuItem("SceneTools/Clear/ClearGenerators")]
    public static void ClearGenerators()
    {
        ClearComponents<PipeMeshGeneratorBase>();
    }

    [MenuItem("SceneTools/Clear/ClearGeneratorArgs")]
    public static void ClearGeneratorArgs()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var ids = obj.GetComponentsInChildren<PipeModelComponent>(true);
            foreach (var id in ids)
            {
                //GameObject.DestroyImmediate(id);
                id.generateArg = null;
            }
            Debug.Log($"ClearGeneratorArgs ids:{ids.Length}");
        }
    }

    [MenuItem("SceneTools/Clear/ClearPipeModels")]
    public static void ClearPipeModels()
    {
        ClearComponents<BaseMeshModel>();
    }

    [MenuItem("SceneTools/Clear/ClearOBBCollider")]
    public static void ClearOBBCollider()
    {
        ClearComponents<OBBCollider>();
    }

    [MenuItem("SceneTools/Clear/ClearMeshComponents")]
    public static void ClearMeshComponents()
    {
        ClearComponents<MeshFilter>();
        ClearComponents<MeshRenderer>();
        ClearComponents<MeshCollider>();
    }


    [MenuItem("SceneTools/Clear/ClearMesh")]
    public static void ClearMesh()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var mfs = obj.GetComponentsInChildren<MeshFilter>(true);
            foreach (var id in mfs)
            {
                id.sharedMesh = null;
            }
            var mcs = obj.GetComponentsInChildren<MeshCollider>(true);
            foreach (var id in mcs)
            {
                id.sharedMesh = null;
            }
            Debug.Log($"ClearMesh       mfs:{mfs.Length} mcs:{mcs.Length}");
        }
    }


    [MenuItem("SceneTools/Clear/ClearScripts")]
    public static void ClearScripts()
    {
        ClearComponents<MonoBehaviour>();
        //foreach (var obj in Selection.gameObjects)
        //{
        //    var ids = obj.GetComponentsInChildren<MonoBehaviour>(true);
        //    foreach (var id in ids)
        //    {
        //        GameObject.DestroyImmediate(id);
        //    }
        //    Debug.Log($"ClearScripts       ids:{ids.Length}");
        //}
    }

    [MenuItem("SceneTools/Clear/ClearNavisModelRoot")]
    public static void ClearNavisRootComponents()
    {
        ClearComponents<NavisModelRoot>();
        ClearComponents<InitNavisFileInfoByModel>();
    }

    [MenuItem("SceneTools/Clear/ClearBuildingModelInfo")]
    public static void ClearBuildingModelInfo()
    {
        ClearComponents<BuildingModelInfo>();
        ClearComponents<BuildingModelInfoList>();
    }

    [MenuItem("SceneTools/Clear/ClearSubScenesAll")]
    public static void ClearSubScenesAll()
    {
        ClearComponents<SubScene_Base>();
        ClearComponents<SubScene_List>();
    }

    #endregion

    #region Transform

    [MenuItem("SceneTools/Transform/ShowPath")]
    public static void ShowPath()
    {
        Debug.Log(Selection.activeGameObject.transform.GetPath());
    }

    //[MenuItem("SceneTools/Transform/SelectParent")]
    //public static void SelectParent()
    //{
    //    EditorHelper.SelectObject(Selection.activeGameObject.transform.parent.gameObject);
    //}

    [MenuItem("SceneTools/Transform/CheckTranformScale")]
    public static void CheckTranformScale()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
        int errorCount = 0;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MeshRenderer renderer = meshRenderers[i];
            Transform t = renderer.transform;
            if (t.lossyScale == Vector3.zero)
            {
                Debug.Log($"CheckScale[{i + 1}/{meshRenderers.Length}]({++errorCount}) Transform:{t.name} \tpath:{t.GetPath()}");
            }
        }
        Debug.LogError($"CheckTranformScale go:{go.name} meshRenderers:{meshRenderers.Length} errorCount:{errorCount}");
    }

    [MenuItem("SceneTools/Transform/GetAllChildrenRenderers")]
    public static void GetAllChildrenRenderers()
    {
        GameObject go = Selection.activeGameObject;
        MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
        foreach(var renderer in meshRenderers)
        {
            renderer.transform.SetParent(go.transform);
        }
        Debug.Log($"GetAllChildrenRenderers go:{go.name} meshRenderers:{meshRenderers.Length}");
        MeshHelper.DecreaseEmptyGroupEx(go);
    }

    [MenuItem("SceneTools/Transform/SetSelectionSamePos")]
    public static void SetSelectionSamePos()
    {

        var gos = Selection.gameObjects;
        GameObject go0 = gos[0];
        for (int i = 1; i < gos.Length; i++)
        {
            GameObject go = gos[i];
            go.transform.position = go0.transform.position;
        }
    }

    [MenuItem("SceneTools/Transform/CopySelectionToRoot")]
    public static void CopySelectionToRoot()
    {
        
        var gos = Selection.gameObjects;
        string name = $"Root[{gos.Length}]";
        foreach (var go in gos)
        {
            name += "_"+go.name ;
        }
        GameObject newRoot = new GameObject(name);
        newRoot.transform.Reset();
        foreach (var go in gos)
        {
            GameObject newGo = GameObject.Instantiate(go);
            newGo.transform.SetParent(newRoot.transform);
        }
        EditorHelper.SelectObject(newRoot);
    }

    [MenuItem("SceneTools/Transform/MoveSelectionToRoot")]
    public static void MoveSelectionToRoot()
    {
        string name = "";
        var gos = Selection.gameObjects;
        foreach (var go in gos)
        {
            name += go.name + "_";
        }
        GameObject newRoot = new GameObject(name);
        newRoot.transform.Reset();
        foreach (var go in gos)
        {
            //GameObject newGo = GameObject.Instantiate(go);
            go.transform.SetParent(newRoot.transform);
        }
        EditorHelper.SelectObject(newRoot);
    }

    [MenuItem("SceneTools/Transform/SetPivotCenter")]
    public static void SetPivotCenter()
    {
        MeshHelper.SetPivot(Selection.activeGameObject, PivotType.Center);
    }

    [MenuItem("SceneTools/Transform/SetPivotMin")]
    public static void SetPivotMin()
    {
        MeshHelper.SetPivot(Selection.activeGameObject, PivotType.Min);
    }

    [MenuItem("SceneTools/Transform/SetPivotMax")]
    public static void SetPivotMax()
    {
        MeshHelper.SetPivot(Selection.activeGameObject, PivotType.Max);
    }

    [MenuItem("SceneTools/Transform/SetPivotBottom")]
    public static void SetPivotBottom()
    {
        MeshHelper.SetPivot(Selection.activeGameObject, PivotType.Bottom);
    }

    [MenuItem("SceneTools/Transform/SetPivotForward")]
    public static void SetPivotForward()
    {
        MeshHelper.SetPivot(Selection.activeGameObject, PivotType.Forward);
    }

    [MenuItem("SceneTools/Transform/CenterPivotAll")]
    public static void CenterPivotAll()
    {
        GameObject root = Selection.activeGameObject;
        MeshHelper.CenterPivotAll(root);
    }

    [MenuItem("SceneTools/Transform/MoveToZero")]
    public static void MoveToZero()
    {
        TransformExtension.MoveToNewRoot(Selection.activeGameObject, "Zero");
    }

    [MenuItem("SceneTools/Transform/SplitParts")]
    public static void SplitParts()
    {
        GameObject newRoot = GameObject.Find("SplitParts");
        if (newRoot == null)
            newRoot = new GameObject("SplitParts");
        GameObject[] gos = Selection.gameObjects;
        foreach (GameObject go in gos)
        {
            //go.transform.SetParent(newRoot.transform);
            //var path = TransformHelper.GetAncestors(go.transform);

            //List<Transform> path = TransformHelper.GetAncestors(go.transform, null);
            //Transform newP = TransformHelper.FindOrCreatePath(newRoot.transform, path, true);
            //go.transform.SetParent(newP.transform);

            TransformExtension.MoveGameObject(go.transform, newRoot.transform);
            //break;
        }
        Debug.LogError($"SplitParts gos:{gos.Length}");
    }

    [MenuItem("SceneTools/Transform/CoypChildren100")]
    public static void CoypChildren100()
    {
        var p = Selection.activeGameObject;
        GameObject go = new GameObject(p.name+"(Copy)");
        go.transform.position = p.transform.position;
        go.transform.SetParent(p.transform.parent);


        for (int i = 0; i < 100 && i < p.transform.childCount; i++)
        {
            var chid = p.transform.GetChild(i);
            var cloned = GameObject.Instantiate(chid.gameObject);
            cloned.transform.SetParent(go.transform);
        }
    }

    [MenuItem("SceneTools/Transform/RemoveOtherBrothers")]
    public static void RemoveOtherBrothers()
    {
        List<GameObject> gos = Selection.gameObjects.ToList();
        var go = Selection.activeGameObject;
        var parent = go.transform.parent;
        EditorHelper.UnpackPrefab(parent.gameObject);
        List<Transform> childrens = new List<Transform>();
        for(int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            //if (child.gameObject == go) continue;
            if (gos.Contains(child.gameObject)) continue;
            childrens.Add(child);
        }
        foreach(var item in childrens)
        {
            GameObject.DestroyImmediate(item.gameObject);
        }
    }

    [MenuItem("SceneTools/Transform/IsEmptyEx")]
    public static void IsEmptyEx()
    {
        MeshHelper.IsEmptyObjectEx(Selection.activeGameObject.transform,true);
    }

    [MenuItem("SceneTools/Transform/IsEmpty")]
    public static void IsEmpty()
    {
        MeshHelper.IsEmptyObject(Selection.activeGameObject.transform, true);
    }

    [MenuItem("SceneTools/Transform/DcsEmptyEx")]
    public static void DcsEmptyEx()
    {
        MeshHelper.DecreaseEmptyGroupEx(Selection.activeGameObject);
    }

    [MenuItem("SceneTools/Transform/DcsEmpty")]
    public static void DcsEmpty()
    {
        MeshHelper.DecreaseEmptyGroup(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Transform/RmEmpty")]
    public static void RmEmpty()
    {
        MeshHelper.RemoveEmptyObjects(Selection.activeGameObject);
    }

    public static void ResetRotation(GameObject root)
    {
        EditorHelper.UnpackPrefab(root);
        DateTime start = DateTime.Now;
        GameObject tmp = new GameObject("TempParent");
        Dictionary<Transform,Transform> dict = new System.Collections.Generic.Dictionary<Transform, Transform>();
        var ts = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in ts)
        {
            dict.Add(t, t.parent);
            t.transform.SetParent(tmp.transform);
        }

        foreach(var t in ts)
        {
            MeshFilter mf = t.GetComponent<MeshFilter>();
            MeshRenderer mr = t.GetComponent<MeshRenderer>();
            MeshCollider mc = t.GetComponent<MeshCollider>();
            if (mf == null && mr == null && mc == null)
            {
                t.rotation = Quaternion.identity;
            }
        }

        foreach (var t in dict.Keys)
        {
            t.transform.SetParent(dict[t]);
        }

        Debug.Log($"ResetRotation root:{root} ts:{ts.Length} time:{DateTime.Now-start}");
    }

    [MenuItem("SceneTools/Transform/ClearChildren")]
    public static void ClearChildren()
    {
        foreach (var obj in Selection.gameObjects)
        {
            MeshHelper.ClearChildren(obj.transform);
        }
    }

    [MenuItem("SceneTools/Transform/SelectParent")]
    public static void SelectParent()
    {
        GameObject go = Selection.activeGameObject;
        EditorHelper.SelectObject(go.transform.parent);
    }

    [MenuItem("SceneTools/Transform/RootParent")]
    public static void RootParent()
    {
        GameObject go = Selection.activeGameObject;
        EditorHelper.UnpackPrefab(go);
        go.transform.SetParent(null);
        EditorHelper.SelectObject(go);
    }

    [MenuItem("SceneTools/Transform/UpParent")]
    public static void UpParent()
    {
        GameObject go = Selection.activeGameObject;
        EditorHelper.UnpackPrefab(go);
        go.transform.SetParent(null);
        EditorHelper.SelectObject(go);
    }

    [MenuItem("SceneTools/Transform/X10")]
    public static void TransformX10()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position *= 10;
            obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/D100Ex_P")]
    public static void TransformD100Ex_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 100;
            // obj.transform.localScale *= 10;
            List<Transform> children = new List<Transform>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                children.Add(child);
            }
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.SetParent(null);
            }
            obj.transform.localScale *= 100;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.SetParent(obj.transform);
            }
            obj.transform.localScale /= 100;
        }
    }

    [MenuItem("SceneTools/Transform/X10C")]
    public static void TransformX10C()
    {
        foreach (var obj in Selection.gameObjects)
        {
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.localScale *= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10C_S")]
    public static void TransformD10C_S()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.localScale /= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10C_PS")]
    public static void TransformD10C_PS()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for(int i=0;i<obj.transform.childCount;i++)
            {
                var child=obj.transform.GetChild(i);
                child.transform.position /= 10;
                child.transform.localScale /= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10C_P")]
    public static void TransformD10C_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            // obj.transform.position *= 10;
            // obj.transform.localScale *= 10;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                child.transform.position /= 10;
            }
        }
    }

    [MenuItem("SceneTools/Transform/D10_P")]
    public static void TransformD10_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 10;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/D100_P")]
    public static void TransformD100_P()
    {
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.position /= 100;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/APP")]
    public static void TransformAPP()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var parent = obj.transform.parent;
            obj.transform.position += parent.position;
            // obj.transform.localScale *= 10;
        }
    }

    [MenuItem("SceneTools/Transform/CSPP")]
    public static void TransformCSPP()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var p1 = obj.transform.position;

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var child = obj.transform.GetChild(i);
                child.transform.position -= p1;
            }
        }
    }

    [MenuItem("SceneTools/Transform/GetPositionOffset")]
    public static void GetPositionOffset()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var obj in Selection.gameObjects)
        {
            string name = obj.name;
            foreach(var i in allT)
            {
                if (i.name == name && i.gameObject != obj.gameObject)
                {
                    var posOff = i.position - obj.transform.position;
                    Debug.Log("posOffset:" + posOff + "|" + name);
                }
            }
        }
    }

    [MenuItem("SceneTools/Transform/SetParentNull")]
    public static void SetParentNull()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var obj in Selection.gameObjects)
        {
            obj.transform.SetParent(null);
        }
    }

    [MenuItem("SceneTools/Transform/Reset")]
    public static void Reset()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        foreach (var obj in Selection.gameObjects)
        {
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero;
        }
    }

    [MenuItem("SceneTools/Transform/LayoutX10")]
    public static void LayoutX10()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero+i*Vector3.forward*1f;
        }
    }
    [MenuItem("SceneTools/Transform/LayoutX05")]
    public static void LayoutX05()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero + i * Vector3.forward * 0.5f;
        }
    }
    [MenuItem("SceneTools/Transform/LayoutX01")]
    public static void LayoutX01()
    {
        var allT = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject obj = Selection.gameObjects[i];
            //obj.transform.SetParent(null);
            obj.transform.position = Vector3.zero + i * Vector3.forward * 0.1f;
        }
    }
    #endregion

    #region Renderers

    [MenuItem("SceneTools/Renderers/ShowSelection")]
    public static void ShowSelectionRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = true;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/HideSelection")]
    public static void HideSelectionRenders()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/ShowAll")]
    public static void ShowAllRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = true;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/HideAll")]
    public static void HideAllRenderers()
    {
        foreach (var obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var render in renderers)
            {
                render.enabled = false;
            }
        }
    }

    [MenuItem("SceneTools/Renderers/RemoveOtherIds")]
    public static void RemoveOtherIds()
    {
        var currentIds= Selection.activeGameObject.GetComponentsInChildren<RendererId>(true).ToList();
        var allIds=GameObject.FindObjectsOfType<RendererId>(true).ToList();
        foreach(var id in allIds){
            if(currentIds.Contains(id)){
                continue;
            }
            GameObject.DestroyImmediate(id);
        }
        Debug.Log($"RemoveOtherIds currentIds:{currentIds.Count} allIds:{allIds.Count}");
    }

    [MenuItem("SceneTools/Renderers/ShowIdDictionary")]
    public static void ShowIdDictionary()
    {
        var dict = IdDictionary.IdDict;
        int i = 0;
        foreach(var key in dict.Keys)
        {
            RendererId id = dict[key];
            if (id == null)
            {
                continue;
            }
            i++;
            Debug.Log($"ShowIdDictionary[{i}] id:{id.Id} pid:{id.parentId} name:{id.name} path:{id.transform.GetPath()}");
        }
        Debug.Log($"ShowIdDictionary allIds:{dict.Count}");
    }

    [MenuItem("SceneTools/Renderers/ShowTreeNodeDict")]
    public static void ShowTreeNodeDict()
    {
        var dict = AreaTreeHelper.renderId2NodeDict;
        int i = 0;
        foreach (var key in dict.Keys)
        {
            AreaTreeNode id = dict[key];
            if (id == null)
            {
                continue;
            }
            i++;
            Debug.Log($"ShowTreeNodeDict[{i}] tree:{id.tree} name:{id.name} path:{id.transform.GetPath()}");
        }
        Debug.Log($"ShowTreeNodeDict allIds:{dict.Count}");
    }

    [MenuItem("SceneTools/Renderers/InitTreeNodeDict")]
    public static void InitTreeNodeDict()
    {
        var nodes = GameObject.FindObjectsOfType<AreaTreeNode>(true).ToList();
        int i = 0;
        foreach(var node in nodes)
        {
            node.CreateDictionary();
        }
        Debug.Log($"InitTreeNodeDict nodes:{nodes.Count}");

        ShowTreeNodeDict();
    }

    [MenuItem("SceneTools/Renderers/ShowIds")]
    public static void ShowIds()
    {
        var allIds = GameObject.FindObjectsOfType<RendererId>(true).ToList();
        for (int i = 0; i < allIds.Count; i++)
        {
            RendererId id = allIds[i];
            Debug.Log($"ShowIds[{i}] id:{id.Id} pid:{id.parentId} name:{id.name} path:{id.transform.GetPath()}");
        }
        Debug.Log($"ShowIds allIds:{allIds.Count}");
    }

    [MenuItem("SceneTools/Renderers/ClearEmptyIds")]
    public static void ClearEmptyIds()
    {
        int count = 0;
        var rids = GameObject.FindObjectsOfType<RendererId>(true);
        StringBuilder sb = new StringBuilder();
       foreach(var rid in rids)
        {
            if (rid == null) continue;
            if (rid.gameObject == null) continue;
            if (string.IsNullOrEmpty(rid.Id))
            {
                count++;
                sb.AppendLine(rid.name);

                GameObject.DestroyImmediate(rid);
            }
        }
        Debug.LogError($"ClearEmptyIds rids:{rids.Length} emptyCount:{count} list:{sb.ToString()}");
    }

    [MenuItem("SceneTools/Renderers/UpdateIds(A)")]
    public static void UpdateRendererIdsA()
    {
        var rids = GameObject.FindObjectsOfType<RendererId>(true);
        UpdateRendererIds(rids, true);
    }

    [MenuItem("SceneTools/Renderers/CheckIds(A)")]
    public static void CheckRendererIdsA()
    {
        var rids = GameObject.FindObjectsOfType<RendererId>(true);
        UpdateRendererIds(rids, false);
    }

    [MenuItem("SceneTools/Renderers/UpdateIds(S)")]
    public static void UpdateRendererIdsS()
    {
        var rids = Selection.activeGameObject.GetComponentsInChildren<RendererId>(true);
        UpdateRendererIds(rids, true);
    }

    [MenuItem("SceneTools/Renderers/CheckIds(S)")]
    public static void CheckRendererIdsS()
    {
        var rids = Selection.activeGameObject.GetComponentsInChildren<RendererId>(true);
        UpdateRendererIds(rids, false);
    }


    [MenuItem("SceneTools/Renderers/UpdateChildrenIds(S)")]
    public static void UpdateRendererChildrenIdsS()
    {
        var rids = Selection.activeGameObject.GetComponentsInChildren<RendererId>(true);
        UpdateRendererIds(rids, true, true);
    }

    [MenuItem("SceneTools/Renderers/UpdateRenderersParentId")]
    public static void UpdateRenderersParentId()
    {
        var renderers = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            var rid = RendererId.GetRId(renderers[i].gameObject);
            if (ProgressBarHelper.DisplayCancelableProgressBar("UpdateParentIds", i + 1, renderers.Length))
            {
                break;
            }
            Transform parent = rid.transform.parent;
            RendererId pId = RendererId.GetRId(parent.gameObject);
            rid.SetPidEx(pId.Id);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"UpdateParentIds rids:{renderers.Length}");
    }

    public static void UpdateRendererIds(RendererId[] rids,bool isUpdateId,bool isUpdateChildrenId=false)
    {
        Dictionary<string, RendererId> ridDict = new Dictionary<string, RendererId>();
        int count = 0;
        foreach (var rid in rids)
        {
            if (isUpdateChildrenId)
            {
                rid.UpdateChildrenId(false);
            }

            if (ridDict.ContainsKey(rid.Id))
            {
                Debug.LogError($"UpdateRendererIds[{count++}] rid:{rid.Id} name:{rid.name} parent:{rid.transform.parent}");
                if (isUpdateId)
                {
                    rid.NewId();
                    ridDict.Add(rid.Id, rid);
                }
            }
            else
            {
                ridDict.Add(rid.Id, rid);
            }
        }
        Debug.LogError($"UpdateRendererIds rids:{rids.Length} UpdateId:{isUpdateId} UpdateChildren:{isUpdateChildrenId}");
    }

    [MenuItem("SceneTools/Renderers/FindRepeatedIdModels")]
    public static void FindRepeatedIdModels()
    {
        DateTime start = DateTime.Now;
        int count1 = 0;
        int count2 = 0;
        RendererId[] rendererIds = GameObject.FindObjectsOfType<RendererId>(true);
        Dictionary<string, RendererId> ridDict = new Dictionary<string, RendererId>();
        for (int i = 0; i < rendererIds.Length; i++)
        {
            RendererId rid = rendererIds[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("FindRepeatedIdModels",i+1, rendererIds.Length))
            {
                break;
            }
            if (ridDict.ContainsKey(rid.Id))
            {
                RendererId rid2 = ridDict[rid.Id];
                count1++;
                bool isSameName = rid.name == rid2.name;
                bool isSameParent = rid.transform.parent == rid2.transform.parent;
                float meshDis = MeshHelper.GetVertexDistanceEx(rid.transform, rid2.transform);
                Debug.Log($"FindRepeatedIdModels[{i+1}/{rendererIds.Length}]({count1})\t[Name:{isSameName} Parent:{isSameParent} Dis:{meshDis:F2}] [rid:{rid.name} rid2:{rid2.name}]\npath1:{rid.transform.GetPath()} bim:{rid.GetComponent<BIMModelInfo>()!=null}\npath2:{rid2.transform.GetPath()} bim:{rid2.GetComponent<BIMModelInfo>() != null}");
            }
            else
            {
                ridDict.Add(rid.Id, rid);
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"FindRepeatedIdModels rendererIds:{rendererIds.Length} count1:{count1}");
    }

    [MenuItem("SceneTools/Renderers/NewIdRepeatedIdModels")]
    public static void NewIdRepeatedIdModels()
    {
        DateTime start = DateTime.Now;
        int count1 = 0;
        int count2 = 0;
        RendererId[] rendererIds = GameObject.FindObjectsOfType<RendererId>(true);
        Dictionary<string, RendererId> ridDict = new Dictionary<string, RendererId>();
        for (int i = 0; i < rendererIds.Length; i++)
        {
            RendererId rid = rendererIds[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("FindRepeatedIdModels", i + 1, rendererIds.Length))
            {
                break;
            }
            if (ridDict.ContainsKey(rid.Id))
            {
                RendererId rid2 = ridDict[rid.Id];
                count1++;
                bool isSameName = rid.name == rid2.name;
                bool isSameParent = rid.transform.parent == rid2.transform.parent;
                float meshDis = MeshHelper.GetVertexDistanceEx(rid.transform, rid2.transform);
                Debug.Log($"NewIdRepeatedIdModels[{i + 1}/{rendererIds.Length}]({count1})\t[Name:{isSameName} Parent:{isSameParent} Dis:{meshDis:F2}] [rid:{rid.name} rid2:{rid2.name}]\npath1:{rid.transform.GetPath()} bim:{rid.GetComponent<BIMModelInfo>() != null}\npath2:{rid2.transform.GetPath()} bim:{rid2.GetComponent<BIMModelInfo>() != null}");
                if(rid.GetComponent<BIMModelInfo>() != null && rid2.GetComponent<BIMModelInfo>() == null)
                {
                    rid2.NewId();
                }
                else if(rid2.GetComponent<BIMModelInfo>() != null && rid.GetComponent<BIMModelInfo>() == null)
                {
                    rid.NewId();
                }
                else
                {
                    count2++;
                }
            }
            else
            {
                ridDict.Add(rid.Id, rid);
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"NewIdRepeatedIdModels rendererIds:{rendererIds.Length} count1:{count1} count2:{count2}");
    }

    [MenuItem("SceneTools/Renderers/DeleteRepeatedIdModels")]
    public static void DeleteRepeatedIdModels()
    {
        DateTime start = DateTime.Now;
        int count1 = 0;
        int count2 = 0;
        int count3 = 0;
        RendererId[] rendererIds = GameObject.FindObjectsOfType<RendererId>(true);
        Dictionary<string, RendererId> ridDict = new Dictionary<string, RendererId>();
        string nameList = "";
        for (int i = 0; i < rendererIds.Length; i++)
        {
            RendererId rid = rendererIds[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("FindRepeatedIdModels", i + 1, rendererIds.Length))
            {
                break;
            }
            if (ridDict.ContainsKey(rid.Id))
            {
                RendererId rid2 = ridDict[rid.Id];
                count1++;
                bool isSameName = rid.name == rid2.name;
                bool isSameParent = rid.transform.parent == rid2.transform.parent;
                float meshDis = MeshHelper.GetVertexDistanceEx(rid.transform, rid2.transform);
                string path = rid.transform.GetPath();
                Debug.Log($"FindRepeatedIdModels[{i + 1}/{rendererIds.Length}]({count1})\t[Name:{isSameName} Parent:{isSameParent} Dis:{meshDis:F2}] [rid:{rid.name} rid2:{rid2.name}] path:{path}");
                if(isSameName&& isSameParent && meshDis < 0.0001f)
                {
                    nameList += rid.name + "; ";
                    //GameObject.DestroyImmediate(rid.gameObject);
                    if(rid.transform.childCount>0 && rid2.transform.childCount == 0)
                    {
                        GameObject.DestroyImmediate(rid2.gameObject);
                        count3++;
                    }
                    else if (rid2.transform.childCount > 0 && rid.transform.childCount == 0)
                    {
                        GameObject.DestroyImmediate(rid.gameObject);
                        count3++;
                    }
                    else
                    {

                    }
                    count2++;
                }
            }
            else
            {
                ridDict.Add(rid.Id, rid);
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"DeleteRepeatedIdModels rendererIds:{rendererIds.Length} count1:{count1} count2:{count2} count3:{count3}");
    }

    [MenuItem("SceneTools/Renderers/DeleteRepeatedIdModels2")]
    public static void DeleteRepeatedIdModels2()
    {
        DateTime start = DateTime.Now;
        int count1 = 0;
        int count2 = 0;
        int count3 = 0;
        RendererId[] rendererIds = GameObject.FindObjectsOfType<RendererId>(true);
        Dictionary<string, RendererId> ridDict = new Dictionary<string, RendererId>();
        string nameList = "";
        for (int i = 0; i < rendererIds.Length; i++)
        {
            RendererId rid = rendererIds[i];
            if (ProgressBarHelper.DisplayCancelableProgressBar("FindRepeatedIdModels", i + 1, rendererIds.Length))
            {
                break;
            }
            if (ridDict.ContainsKey(rid.Id))
            {
                RendererId rid2 = ridDict[rid.Id];
                count1++;
                bool isSameName = rid.name == rid2.name;
                bool isSameParent = rid.transform.parent == rid2.transform.parent;
                float meshDis = MeshHelper.GetVertexDistanceEx(rid.transform, rid2.transform);
                string path = rid.transform.GetPath();
                Debug.Log($"FindRepeatedIdModels[{i + 1}/{rendererIds.Length}]({count1})\t[Name:{isSameName} Parent:{isSameParent} Dis:{meshDis:F2}] [rid:{rid.name} rid2:{rid2.name}] path:{path}");
                if (isSameName && isSameParent && meshDis < 0.0001f)
                {
                    nameList += rid.name + "; ";
                    //GameObject.DestroyImmediate(rid.gameObject);
                    if (rid.transform.childCount > 0 && rid2.transform.childCount == 0)
                    {
                        GameObject.DestroyImmediate(rid2.gameObject);
                        count3++;
                    }
                    else if (rid2.transform.childCount > 0 && rid.transform.childCount == 0)
                    {
                        GameObject.DestroyImmediate(rid.gameObject);
                        count3++;
                    }
                    else
                    {
                        GameObject.DestroyImmediate(rid.gameObject);
                    }
                    count2++;
                }
            }
            else
            {
                ridDict.Add(rid.Id, rid);
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"DeleteRepeatedIdModels2 rendererIds:{rendererIds.Length} count1:{count1} count2:{count2} count3:{count3}");
    }

    #endregion

    #region Collider

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/ClearCollders")]
    public static void ClearCollders()
    {
        ClearComponents<Collider>();
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/EnabledRendererColliders")]
    public static void EnabledRendererColliders()
    {
        //ClearComponents<Collider>();
        //TransformHelper.SetCollidersEnabled(Selection.gameObjects)
        int count = 0;
        var cs = Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var item in cs)
        {
            Collider[] colliders = item.GetComponents<Collider>();
            foreach(var collider in colliders)
            {
                if (collider.enabled == false)
                {
                    collider.enabled = true;
                    count++;
                }
            }
        }
        Debug.Log($"EnabledRendererColliders cs:{cs.Length} count:{count}");
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/EnabledColliders")]
    public static void EnabledColliders()
    {
        //ClearComponents<Collider>();
        //TransformHelper.SetCollidersEnabled(Selection.gameObjects)
        var cs = Selection.activeGameObject.GetComponentsInChildren<Collider>(true);
        foreach (var item in cs)
        {
            item.enabled = true;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/DisenabledColliders")]
    public static void DisenabledColliders()
    {
        //ClearComponents<Collider>();
        //TransformHelper.SetCollidersEnabled(Selection.gameObjects)
        var cs = Selection.activeGameObject.GetComponentsInChildren<Collider>(true);
        foreach (var item in cs)
        {
            item.enabled = false;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/RefreshCollderMesh")]
    public static void RefreshCollderMesh()
    {
        MeshHelper.RefreshCollderMesh();
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/AddBoxCollider")]
    public static void AddBoxCollider()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/AddBoxCollider_IsTrigger")]
    public static void AddBoxCollider_IsTrigger()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("SceneTools/Collider/AddBoxCollider_IsTrigger_NotRemoveChild")]
    public static void AddBoxCollider_IsTrigger_NotRemoveChild()
    {
        //Transform parent = Selection.activeGameObject.transform;
        //ColliderHelper.CreateBoxCollider(parent);

        foreach (GameObject O in Selection.gameObjects)
        {
            ColliderHelper.CreateBoxCollider(O.transform, false);
            BoxCollider collider = O.GetComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// 添加所有的MeshCollider
    /// </summary>
    [MenuItem("SceneTools/Collider/AddAllMeshCollider")]
    public static void AddAllMeshCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        AddAllMeshCollider(parent);
    }

    public static void AddAllMeshCollider(Transform parent)
    {
        var meshFilters = parent.GetComponentsInChildren<MeshFilter>(true);
        foreach (var meshFilter in meshFilters)
        {
            MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
        //ColliderHelper.CreateBoxCollider(parent);
    }
    #endregion

    #region EditTools
    [MenuItem("SceneTools/FocusSelection")]
    private static void FocusSelection()
    {
        EditorHelper.SelectObject(Selection.activeGameObject);
    }
    [MenuItem("SceneTools/Models/Delete")]
    private static void Delete()
    {
        Debug.Log("Delete");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            Delete(dir, false);
        }
        EditorHelper.RefreshAssets();
    }

    private static void Delete(DirectoryInfo dir, bool isTest)
    {
        var start = DateTime.Now;
        Debug.Log("Delete Dir:" + dir);
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        Regex regex = new Regex(".(3DS|3ds|fbx|FBX|prefab|Prefab)$"); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<GameObject> modelObjects = new List<GameObject>();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //Debug.Log("ChangeModel file:" + filePath);

            string relativePath = "Assets\\" + filePath.Substring(Application.dataPath.Length + 1);

            if (regex.IsMatch(relativePath))
            {
                string name = file.Name;
                string[] parts = name.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                //Debug.Log("name:" + parts.Length + "|" + name);
                if (parts.Length == 6)
                {
                    string newName = string.Format("{0}({1})({2}){3}", parts[0], parts[1], parts[2], parts[5]);
                    Debug.Log("name:" + newName + "<->" + name);
                    string assetPath = relativePath.Replace("\\", "/");
                    AssetDatabase.DeleteAsset(assetPath);//重复的
                }

                ////Debug.Log(string.Format("CreateLODGroup ({0}/{1}) file :{2}", i, files.Length, filePath));
                //GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                //if (subObj != null)
                //{

                //    modelObjects.Add(subObj);
                //}
            }
        }

        var time = DateTime.Now - start;

        Debug.Log(string.Format("Rename 用时:{0}", time));
    }

    [MenuItem("SceneTools/Models/Rename")]
    private static void Rename()
    {
        Debug.Log("Rename");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            Rename(dir, false);
        }
        EditorHelper.RefreshAssets();
    }

    private static void Rename(DirectoryInfo dir, bool isTest)
    {
        var start = DateTime.Now;
        Debug.Log("Rename Dir:" + dir);
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        Regex regex = new Regex(".(3DS|3ds|fbx|FBX|prefab|Prefab)$"); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<GameObject> modelObjects = new List<GameObject>();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //Debug.Log("ChangeModel file:" + filePath);

            string relativePath = "Assets\\" + filePath.Substring(Application.dataPath.Length + 1);

            if (regex.IsMatch(relativePath))
            {
                string name = file.Name;
                string[] parts = name.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                //Debug.Log("name:" + parts.Length + "|" + name);
                if (parts.Length == 6)
                {
                    string newName = string.Format("{0}({1})({2}){3}", parts[0], parts[1], parts[2], parts[5]);
                    Debug.Log("name:" + newName + "<->" + name);
                    //string newPath = relativePath.Replace(name, newName);
                    //Debug.Log("newPath:" + newPath + "<->" + relativePath);
                    string assetPath = relativePath.Replace("\\", "/");
                    //Debug.Log("assetPath:" + assetPath);
                    //string assetPathNew = newPath.Replace("\\", "/");
                    //Debug.Log("assetPathNew:" + assetPathNew);
                    string r = AssetDatabase.RenameAsset(assetPath, newName);
                    Debug.Log("r:" + r);
                    if (r != "")
                    {
                        if (newName.Contains("_lod"))
                        {
                            break;
                        }
                        else
                        {
                            AssetDatabase.DeleteAsset(assetPath);//重复的

                        }
                    }
                    //GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                    //Debug.Log("subObj:" + subObj);
                    //string assetpath = AssetDatabase.GetAssetPath(subObj);
                    //Debug.Log("assetpath:" + assetpath);
                    //break;
                }

                ////Debug.Log(string.Format("CreateLODGroup ({0}/{1}) file :{2}", i, files.Length, filePath));
                //GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                //if (subObj != null)
                //{

                //    modelObjects.Add(subObj);
                //}
            }
        }

        var time = DateTime.Now - start;

        Debug.Log(string.Format("Rename 用时:{0}", time));
    }

    [MenuItem("SceneTools/Models/CreatePrefab")]
    private static void CreatePrefab()
    {
        Debug.Log("CreatePrefab");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreatePrefab(dir, false);
        }
        EditorHelper.RefreshAssets();
    }

    [MenuItem("SceneTools/Models/CreatePrefab(Test)")]
    private static void CreatePrefabTest()
    {
        Debug.Log("CreatePrefab");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设

        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreatePrefab(dir, true);
        }
        EditorHelper.RefreshAssets();
    }

    private static List<GameObject> GetGameObject(DirectoryInfo dir, string filter)
    {
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        Regex regex = new Regex(filter); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<GameObject> modelObjects = new List<GameObject>();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //Debug.Log("ChangeModel file:" + filePath);

            int index = filePath.IndexOf(Application.dataPath);
            string relativePath = filePath.Substring(Application.dataPath.Length + 1);
            relativePath = "Assets\\" + relativePath;
            if (regex.IsMatch(relativePath))
            {
                //Debug.Log(string.Format("CreateLODGroup ({0}/{1}) file :{2}", i, files.Length, filePath));
                GameObject subObj = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
                if (subObj != null)
                {

                    modelObjects.Add(subObj);
                }
            }
        }
        return modelObjects;
    }

    private static void CreatePrefab(DirectoryInfo dir, bool isTest)
    {
        var start = DateTime.Now;
        Debug.Log("CreatePrefab Dir:" + dir);
        List<GameObject> modelObjects = GetGameObject(dir, ".(3DS|3ds|fbx|FBX)$");

        Dictionary<string, List<GameObject>> groups = new Dictionary<string, List<GameObject>>();
        foreach (GameObject obj in modelObjects)
        {
            string name = obj.name;
            int id = name.LastIndexOf(')');
            string groupName = name.Substring(0, id + 1);

            List<GameObject> list = null;
            if (!groups.ContainsKey(groupName))
            {
                list = new List<GameObject>();
                groups.Add(groupName, list);
            }
            else
            {
                list = groups[groupName];
            }

            list.Add(obj);
        }

        GameObject root = new GameObject();
        root.name = "PrefabRoot";

        Material[] lodMaterials = Resources.LoadAll<Material>("Materials/LODs");

        foreach (var groupKeyValue in groups)
        {
            string name = groupKeyValue.Key;
            var list = groupKeyValue.Value;

            for (int i = 0; i < list.Count; i++)
            {
                GameObject item = list[i];
                GameObject model = GameObject.Instantiate(item);
                model.transform.SetParent(root.transform);
                model.transform.position = Vector3.zero;
                model.name = item.name;

                if (isTest)
                {
                    if (item.name.ToLower().Contains("_lod1"))
                    {
                        model.name = name + "(test)_lod1";
                    }
                    else if (item.name.ToLower().Contains("_lod2"))
                    {
                        model.name = name + "(test)_lod2";
                    }
                    else if (item.name.ToLower().Contains("_lod3"))
                    {
                        model.name = name + "(test)_lod3";
                    }
                    else
                    {
                        model.name = name + "(test)";
                    }


                    //model.name += "[test]";
                    if (item.name.ToLower().Contains("_lod1"))
                    {
                        SetMaterial(model, lodMaterials[1]);
                    }
                    else if (item.name.ToLower().Contains("_lod2"))
                    {
                        SetMaterial(model, lodMaterials[2]);
                    }
                    else if (item.name.ToLower().Contains("_lod3"))
                    {
                        SetMaterial(model, lodMaterials[3]);
                    }
                    else
                    {
                        SetMaterial(model, lodMaterials[0]);
                    }
                }
                SavePrefab(model, "UnitsPrefab");
            }
        }

        var time = DateTime.Now - start;

        Debug.Log(string.Format("CreatePrefab 用时:{0}", time));
    }

    private static void SetMaterial(GameObject obj, Material newMaterial)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            //for (int k = 0; k < renderer.sharedMaterials.Length; k++)
            //{
            //    renderer.sharedMaterials[k] = newMaterial;
            //}
            //没有效果

            //for (int k = 0; k < renderer.materials.Length; k++)
            //{
            //    renderer.materials[k] = newMaterial;
            //}
            //没运行时 会报错，运行了 也没有效果

            renderer.material = newMaterial;
        }
    }

    private static void SavePrefab(GameObject obj, string dirName)
    {
        string dir = Application.dataPath + "/Resources/RvtScenes/" + currentModelSceneName + "/Models/" + dirName + "/";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        //Debug.LogError("dir:"+ dir);
        string localPath = "Assets/Resources/RvtScenes/" + currentModelSceneName + "/Models/" + dirName + "/" + obj.name + ".prefab";
        // Make sure the file name is unique, in case an existing Prefab has the same name.
        //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath); //自动重命名
        // Create the new Prefab.
        //string filePath = Application.dataPath + "/Resources/RvtScenes/滁州惠科中心/Models/" + dirName + "/" + obj.name + ".prefab";
        //Debug.Log("filePath:" + filePath);
        //if (File.Exists(filePath))
        //{
        //    File.Delete(filePath);//避免自动修改文件名
        //}

        PrefabUtility.SaveAsPrefabAssetAndConnect(obj, localPath, InteractionMode.UserAction);
    }



    [MenuItem("SceneTools/Models/CreateLODGroup")]
    private static void CreateLODGroup()
    {
        Debug.Log("CreateLODGroup");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreateLODGroup(dir, 4);

        }
        EditorHelper.RefreshAssets();
    }

    [MenuItem("SceneTools/Models/CreateLODGroup2")]
    private static void CreateLODGroup2()
    {
        Debug.Log("CreateLODGroup2");
        //1.获取全部的模型文件

        //2.根据名称分组
        //3.分组创建LODGroup
        //4.生成预设


        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            CreateLODGroup(dir, 3);
        }
        EditorHelper.RefreshAssets();
    }

    private static Renderer[] CreateAndGetRenderers(GameObject pre, GameObject parent)
    {
        Renderer[] renderers = new Renderer[1];
        GameObject model = GameObject.Instantiate(pre);
        model.transform.SetParent(parent.transform);
        model.transform.position = Vector3.zero;
        model.name = pre.name;
        Renderer render = model.GetComponent<Renderer>();
        //Debug.Log(string.Format("{0},{1}", i, render));
        renderers[0] = render;
        return renderers;
    }

    private static void CreateLODGroup(DirectoryInfo dir, int lodCount)
    {
        var start = DateTime.Now;
        Debug.Log("CreateLODGroup Dir:" + dir);
        List<GameObject> modelObjects = GetGameObject(dir, ".(3DS|3ds|fbx|FBX|prefab|Prefab)$");

        Dictionary<string, List<GameObject>> groups = new Dictionary<string, List<GameObject>>();
        foreach (GameObject obj in modelObjects)
        {
            string name = obj.name;
            //if (!name.ToLower().Contains("lod"))
            //{
            //    continue;
            //}
            int id = name.LastIndexOf(')');

            string groupName = name.Substring(0, id + 1);
            List<GameObject> list = null;
            if (!groups.ContainsKey(groupName))
            {
                list = new List<GameObject>();
                groups.Add(groupName, list);
            }
            else
            {
                list = groups[groupName];
            }
            list.Add(obj);
        }

        GameObject root = new GameObject();
        root.name = "LODRoot";

        foreach (var groupKeyValue in groups)
        {
            string name = groupKeyValue.Key;
            var list = groupKeyValue.Value;

            Debug.Log("CreateLODGroup type name:" + name);

            GameObject obj = new GameObject();
            obj.name = name + "_lod";
            obj.transform.SetParent(root.transform);
            LODGroup lodGroup = obj.AddComponent<LODGroup>();

            if (list.Count == 4)
            {
                LOD[] lods = new LOD[lodCount];

                float[] lodHeightList = new float[] { 0.1f, 0.05f, 0.01f, 0.0001f };

                if (lodCount == 3)
                {
                    lodHeightList = new float[] { 0.1f, 0.05f, 0.0001f };
                    obj.name = name + "_lod_low";

                    for (int i = 1, j = 0; i < list.Count; i++, j++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);

                        LOD lod = new LOD(lodHeightList[j], renderers);
                        lods[j] = lod;
                    }
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);
                        LOD lod = new LOD(lodHeightList[i], renderers);
                        lods[i] = lod;
                    }
                }
                lodGroup.SetLODs(lods);
            }
            if (list.Count == 3)
            {
                //lodCount--;
                int newLodCount = lodCount - 1;////4->3 3->2

                LOD[] lods = new LOD[newLodCount];

                float[] lodHeightList = new float[] { 0.1f, 0.05f, 0.0001f };

                if (newLodCount == 2)
                {
                    lodHeightList = new float[] { 0.1f, 0.0001f };
                    obj.name = name + "_lod_low";

                    for (int i = 1, j = 0; i < list.Count; i++, j++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);

                        LOD lod = new LOD(lodHeightList[j], renderers);
                        lods[j] = lod;
                    }
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var renderers = CreateAndGetRenderers(list[i], obj);
                        LOD lod = new LOD(lodHeightList[i], renderers);
                        lods[i] = lod;
                    }
                }
                lodGroup.SetLODs(lods);
            }




            lodGroup.RecalculateBounds();

            SavePrefab(obj, "UnitsLOD");
        }

        var time = DateTime.Now - start;

        Debug.Log(string.Format("CreateLODGroup 用时:{0}", time));
    }

    [MenuItem("SceneTools/Models/GetModelInfo")]//清空AssetBunldeName
    private static void GetChangeModelMenu()
    {
        Debug.Log("ChangeModelMenu");
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            FindModelInDir(dir, false, 0, false);
        }
        EditorHelper.RefreshAssets();
    }
    private static void SetChangeModelInner(bool isSet, int maxCount, bool isSetMat)
    {
        Debug.Log("ChangeModelMenu");
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            //var dir = GetDirInfo(dirObj);
            FindModelInDir(dir, isSet, maxCount, isSetMat);
        }
        EditorHelper.RefreshAssets();
    }


    [MenuItem("SceneTools/Models/ChangeModel(100)")]//清空AssetBunldeName
    private static void SetChangeModel100()
    {
        SetChangeModelInner(true, 100, true);
    }

    [MenuItem("SceneTools/Models/ChangeModel(1000)")]//清空AssetBunldeName
    private static void SetChangeModel1000()
    {
        SetChangeModelInner(true, 1000, true);
    }

    [MenuItem("SceneTools/Models/ChangeModel(All)")]
    private static void SetChangeModelAll()
    {
        SetChangeModelInner(true, 0, true);
    }

    [MenuItem("SceneTools/Models/ChangeModel(Selection)")]
    private static void SetChangeModelSelection()
    {
        SetChangeModelSelectionInner(true);
    }

    [MenuItem("SceneTools/Models/EnableReadWrite(100)")]//清空AssetBunldeName
    private static void EnableReadWrite100()
    {
        SetChangeModelInner(true, 100, false);
    }

    [MenuItem("SceneTools/Models/EnableReadWrite(1000)")]//清空AssetBunldeName
    private static void EnableReadWrite1000()
    {
        SetChangeModelInner(true, 1000, false);
    }

    [MenuItem("SceneTools/Models/EnableReadWrite(All)")]
    private static void EnableReadWriteAll()
    {
        SetChangeModelInner(true, 0, false);
    }

    [MenuItem("SceneTools/Models/EnableReadWrite(Selection)")]
    private static void EnableReadWriteSelection()
    {
        SetChangeModelSelectionInner(false);
    }

    private static void SetChangeModelSelectionInner(bool isSetMat)
    {
        var start = DateTime.Now;
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        Debug.Log("selectedAssets:" + selectedAssets.Length + "|" + selectedAssets);

        for (int i = 0; i < selectedAssets.Length; i++)
        {
            var assetObj = selectedAssets[i];
            float progress = (float)i / selectedAssets.Length;
            float percent = progress * 100;
            if (EditorUtility.DisplayCancelableProgressBar("SetChangeModelSelection", $"{i}/{selectedAssets.Length} {percent}% of 100%", progress))
            {

                break;
            }

            string sPath = AssetDatabase.GetAssetPath(assetObj);
            var atImporter = ModelImporter.GetAtPath(sPath);
            if (atImporter == null)
            {
                Debug.LogError($"atImporter == null");
            }
            else if (atImporter is ModelImporter)
            {
                ModelImporter modelImporter = (ModelImporter)atImporter;

                Debug.Log($"[{i}] {assetObj} \t{sPath} \t{modelImporter} \t{modelImporter.isReadable} \t{modelImporter.materialLocation}");

                //if (modelImporter.isReadable == false || modelImporter.materialLocation == ModelImporterMaterialLocation.InPrefab)
                //{
                //    modelImporter.isReadable = true;
                //    //importer.
                //    if (isSetMat)
                //    {
                //        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                //    }
                //    AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
                //}
                bool isChanged = false;
                if (modelImporter.isReadable == false)
                {
                    modelImporter.isReadable = true;
                    isChanged = true;
                }
                if (isSetMat)
                {
                    if (modelImporter.materialLocation == ModelImporterMaterialLocation.InPrefab)
                    {
                        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                        isChanged = true;
                    }
                }
                if (isChanged)
                {
                    AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
                }
            }
            else
            {
                Debug.LogError($"atImporter:{atImporter.GetType()} path:{sPath} obj:{assetObj}");
            }

        }
        EditorUtility.ClearProgressBar();
        Debug.Log($"SetChangeModelSelection {(DateTime.Now - start).ToString()}");
    }

    [MenuItem("SceneTools/Models/GPUI")]
    private static void SetModelGPUI()
    {
        var start = DateTime.Now;
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        Debug.Log("selectedAssets:" + selectedAssets.Length + "|" + selectedAssets);

        for (int i = 0; i < selectedAssets.Length; i++)
        {
            var assetObj = selectedAssets[i];
            float progress = (float)i / selectedAssets.Length;
            float percent = progress * 100;
            if (EditorUtility.DisplayCancelableProgressBar("SetChangeModelSelection", $"{i}/{selectedAssets.Length} {percent}% of 100%", progress))
            {

                break;
            }

            string sPath = AssetDatabase.GetAssetPath(assetObj);
            ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(sPath);

            Debug.Log($"[{i}] {assetObj} \t{sPath} \t{importer} \t{importer.isReadable} \t{importer.materialLocation}");

            if (importer.isReadable == false || importer.materialLocation == ModelImporterMaterialLocation.InPrefab)
            {
                importer.isReadable = true;
                //importer.
                importer.materialLocation = ModelImporterMaterialLocation.External;
                AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log($"SetChangeModelSelection {(DateTime.Now - start).ToString()}");
    }

    private static string currentModelSceneName = "XXXX";

    /// <summary>
    /// 获取选中文件夹
    /// </summary>
    /// <returns></returns>
    private static List<DirectoryInfo> GetDirObjs()
    {
        currentModelSceneName = "XXXX";
        Debug.Log("GetDirObjs");
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        Debug.Log("selectedAssets:" + selectedAssets.Length + "|" + selectedAssets);
        List<DirectoryInfo> dirObjs = new List<DirectoryInfo>();
        foreach (Object obj in selectedAssets)
        {
            //Debug.Log("obj:" + obj);
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //Debug.Log("sourcePath:" + sourcePath);

            if (Directory.Exists(sourcePath)) //判断是否是文件夹
            {
                //var dir = GetDirInfo(dirObj);
                dirObjs.Add(new DirectoryInfo(sourcePath));
                Debug.Log("add Dir1:" + sourcePath);
            }
        }
        if (dirObjs.Count == 0)//选中的是文件(模型)
        {
            foreach (Object obj in selectedAssets)
            {
                //Debug.Log("obj:" + obj);
                string sourcePath = AssetDatabase.GetAssetPath(obj);
                //Debug.Log("sourcePath:" + sourcePath);

                if (File.Exists(sourcePath)) //判断是否是文件夹
                {
                    FileInfo fi = new FileInfo(sourcePath);
                    if (!dirObjs.Contains(fi.Directory))
                    {
                        dirObjs.Add(fi.Directory);

                        Debug.Log("add Dir2:" + fi.Directory);
                    }

                    if (fi.Directory.Name == "Units")
                    {
                        currentModelSceneName = fi.Directory.Parent.Parent.Name;
                    }

                }
            }
        }
        return dirObjs;
    }
    /// <summary>
    /// 设置文件夹Asset名称
    /// </summary>
    /// <param name="dir"></param>
    private static void FindModelInDir(DirectoryInfo dir, bool isSet, int maxCount, bool isSetMat)
    {
        Debug.Log(string.Format("SetDirAssetName:{0}", dir.FullName));
        DirectoryInfo[] subDirs = dir.GetDirectories();
        //if (subDirs.Length == 1 && subDirs[0].Name == "Materials")
        //{
        //    ChangeModel(dir);
        //}
        //else
        //{
        ChangeModel(dir, isSet, maxCount, isSetMat);
        //foreach (DirectoryInfo subDir in subDirs)
        //{
        //    FindModelInDir(subDir);
        //}
        //}
    }
    /// <summary>
    /// 获取文件夹信息
    /// </summary>
    /// <param name="dirObj"></param>
    /// <returns></returns>
    private static DirectoryInfo GetDirInfo(Object dirObj)
    {
        string sourcePath = AssetDatabase.GetAssetPath(dirObj);
        DirectoryInfo dir = new DirectoryInfo(sourcePath); //获取文件夹
        return dir;
    }

    private static void ChangeModel(DirectoryInfo dir, bool isSetMatAndReadWrite, int maxCount, bool isSetMat)
    {
        var start = DateTime.Now;
        Debug.Log("ChangeModel Dir:" + dir);
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        List<Object> subObjects = new List<Object>();
        Regex regex = new Regex(".(3DS|3ds|fbx|FBX|prefab|Prefab)$"); //正则表达式，过滤后缀为.meta.cs.xml的文件

        List<ModelImporter> models = new List<ModelImporter>();
        List<string> pathList = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            string filePath = file.FullName;

            //Debug.Log("ChangeModel file:" + filePath);

            int index = filePath.IndexOf(Application.dataPath);
            string relativePath = filePath.Substring(Application.dataPath.Length + 1);
            relativePath = "Assets\\" + relativePath;
            if (regex.IsMatch(relativePath))
            {
                Debug.Log(string.Format("GetModelInfo ({0}/{1}) file :{2}", i, files.Length, filePath));
                Object subObj = AssetDatabase.LoadAssetAtPath<Object>(relativePath);
                string sPath = AssetDatabase.GetAssetPath(subObj);
                ModelImporter importer = ModelImporter.GetAtPath(sPath) as ModelImporter;
                if (importer != null)
                {
                    //if (importer.globalScale == 0.1f) importer.globalScale = 0.7f;
                    //else if (importer.globalScale == 0.3f) importer.globalScale = 3.3f;
                    if (importer.isReadable == false || importer.materialLocation == ModelImporterMaterialLocation.InPrefab)
                    {
                        models.Add(importer);
                        pathList.Add(sPath);

                        //importer.isReadable = true;
                        ////importer.
                        //importer.materialLocation = ModelImporterMaterialLocation.External;
                        //AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
                    }
                }
                else
                {

                }

                //MeshImporter.
            }
        }

        Debug.Log(string.Format("models:{0}", models.Count));
        if (isSetMatAndReadWrite)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (maxCount > 0 && i <= maxCount) break;
                var importer = models[i];
                var sPath = pathList[i];
                Debug.Log(string.Format("SetModelInfo({0}/{1}) file :{2}", i, files.Length, sPath));

                importer.isReadable = true;
                //importer.
                if (isSetMat)
                {
                    importer.materialLocation = ModelImporterMaterialLocation.External;
                }
                AssetDatabase.ImportAsset(sPath);  //这句不加，面板上数字会变，但是实际大小不会变
            }
        }

        var time = DateTime.Now - start;

        Debug.Log(string.Format("ChangeModel 用时:{0}", time));
    }

    [MenuItem("SceneTools/Models/LoadModels")]//清空AssetBunldeName
    private static void LoadModels()
    {
        Debug.Log("LoadModels");
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0)
        {
            Debug.Log("dirObjs.Count == 0");
            return;//选中的没有文件夹
        }
        foreach (var dir in dirObjs)
        {
            ////var dir = GetDirInfo(dirObj);
            //FindModelInDir(dir);


        }

    }
    #endregion
}
