using CodeStage.AdvancedFPSCounter.Editor.UI;
using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static BIMModelInfo;

[CustomEditor(typeof(NavisModelRoot))]
public class NavisModelRootEditor : BaseFoldoutEditor<NavisModelRoot>
{
    static FoldoutEditorArg<ModelItemInfo> targetListArg = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<ModelItemInfo> bimListArg01 = new FoldoutEditorArg<ModelItemInfo>(true,false);
    static FoldoutEditorArg<ModelItemInfo> bimListArg02 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> bimListArg03 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> bimListArg04 = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<BIMModelInfo> bimListArg0 = new FoldoutEditorArg<BIMModelInfo>(true, false);
    static FoldoutEditorArg<BIMModelInfo> bimListArg1 = new FoldoutEditorArg<BIMModelInfo>(true, false);
    static FoldoutEditorArg<BIMModelInfo> bimListArg2 = new FoldoutEditorArg<BIMModelInfo>(true, false);
    static FoldoutEditorArg<BIMModelInfo> bimListArg3 = new FoldoutEditorArg<BIMModelInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg_all = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg_uid = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg_nouid = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg_drawable_zero = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg_drawable_nozero = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg_nodrawable_nozero = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg_noDrawable_zero = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<ModelItemInfo> modelResultListArg_all = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelResultListArg_found1 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelResultListArg_found2 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelResultListArg_nofound1 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelResultListArg_nofound2 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelResultListArg_exception = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<Transform> transListArg_All = new FoldoutEditorArg<Transform>(true, false);
    static FoldoutEditorArg<Transform> transListArg = new FoldoutEditorArg<Transform>(true, false);
    static FoldoutEditorArg<Transform> transListArg2 = new FoldoutEditorArg<Transform>(true, false);

    static FoldoutEditorArg bimAreasArg = new FoldoutEditorArg(true, false);

    static FoldoutEditorArg repeatedModelsArg1 = new FoldoutEditorArg(true, false);

    static FoldoutEditorArg repeatedModelsArg2 = new FoldoutEditorArg(true, false);

    private static void DrawVueModelListEx(NavisModelRoot item)
    {
        EditorUIUtils.Separator(5);
        DrawRepeatedModels("RepeatedModels(RendererId)", repeatedModelsArg1, item, item.ModelDict.repeatModelsByRendererId);
        DrawRepeatedModels("RepeatedModels(Uid)", repeatedModelsArg2, item, item.ModelDict.repeatModelsByUId);
        
        if (item.ModelList != null)
        {
            EditorUIUtils.Separator(5);
            DrawVueModelList(item,item.ModelListAll.allModels, modelListArg_all, "Model List(All)");

            DrawVueModelList(item, item.ModelList.allModels_uid, modelListArg_uid, "Model List(UID)");
            DrawVueModelList(item, item.ModelList.allModels_noUid, modelListArg_nouid, "Model List(NoUID)");

            DrawVueModelList(item, item.ModelList.allModels_drawable_zero, modelListArg_drawable_zero, "Model List(Drawable&Zero)");
            DrawVueModelList(item, item.ModelList.allModels_drawable_nozero, modelListArg_drawable_nozero, "** Model List(Drawable&NoZero) **");
            DrawVueModelList(item, item.ModelList.allModels_noDrawable_nozero, modelListArg_nodrawable_nozero, "Model List(NoDrawable&NoZero)");
            DrawVueModelList(item, item.ModelList.allModels_noDrawable_zero, modelListArg_noDrawable_zero, "Model List(NoDrawable&Zero)");

            EditorUIUtils.Separator(5);
            DrawVueModelList(item, item.model2TransformResult.allModels_uid_all, modelResultListArg_all, "Model List(All)");
            DrawVueModelList(item, item.model2TransformResult.allModels_uid_found1, modelResultListArg_found1, "Model List(Found1)");
            DrawVueModelList(item, item.model2TransformResult.allModels_uid_found2, modelResultListArg_found2, "Model List(Found2)");
            DrawVueModelList(item, item.model2TransformResult.allModels_uid_nofound1, modelResultListArg_nofound1, "Model List(noFound1)");
            DrawVueModelList(item, item.model2TransformResult.allModels_uid_nofound2, modelResultListArg_nofound2, "Model List(noFound2)");
            DrawVueModelList(item, item.model2TransformResult.allModels_uid_exception, modelResultListArg_exception, "Model List(Exception)");
        }

        EditorUIUtils.Separator(5);

        DrawVueModelList(item, item.noFoundBimInfos01, bimListArg01, "No Found Bim List01(All)");
        DrawVueModelList(item, item.noFoundBimInfos02, bimListArg02, "No Found Bim List02(Zero)");
        DrawVueModelList(item, item.noFoundBimInfos03, bimListArg03, "No Found Bim List03(NotAero)");
        DrawVueModelList(item, item.noFoundBimInfos04, bimListArg04, "No Found Bim List04(NotDrawable)");
        EditorUIUtils.Separator(5);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        InitNavisFileInfoByModelSetting.Instance.AddDoorABFilter();
    }

    public static void DrawUI(NavisModelRoot item)
    {
        if (item == null) return;

        EditorGUILayout.BeginHorizontal();
        item.MinDistanceLv1 = EditorGUILayout.FloatField(item.MinDistanceLv1, GUILayout.Width(70));
        if (GUILayout.Button("BindBimInfo"))
        {
            item.BindBimInfo(null);
        }
        if (GUILayout.Button("OneKey1"))
        {
            item.BindBimInfoEx1(null);
        }
        if (GUILayout.Button("OneKey2"))
        {
            item.BindBimInfoEx2(null);
        }
        if (GUILayout.Button("OneKey12"))
        {
            item.BindBimInfoEx12(null);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label(item.ResultLog);

        EditorGUILayout.BeginHorizontal();
        //GUILayout
        GUILayout.Label("ModelName", GUILayout.Width(80));
        item.ModelName = EditorGUILayout.TextField(item.ModelName,GUILayout.Width(120));
        GUILayout.Label("Inactive", GUILayout.Width(60));
        item.includeInactive = EditorGUILayout.Toggle(item.includeInactive, GUILayout.Width(20));
        
        GUILayout.Label("Structure", GUILayout.Width(60));
        InitNavisFileInfoByModelSetting.Instance.IsIncludeStructure = EditorGUILayout.Toggle(InitNavisFileInfoByModelSetting.Instance.IsIncludeStructure, GUILayout.Width(20));
        GUILayout.Label("FilterBIM", GUILayout.Width(60));
        item.IsFilterBIM = EditorGUILayout.Toggle(item.IsFilterBIM, GUILayout.Width(20));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.TargetRoot = BaseEditorHelper.ObjectField(item.TargetRoot);
        if (GUILayout.Button("FindRelatives"))
        {
            item.FindRelativeTargets();
        }
        if (GUILayout.Button("Move"))
        {
            item.MoveRelativeTargets();
        }
        if (GUILayout.Button("Recover"))
        {
            item.RecoverRelativeTargets();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("HidePipes"))
        {
            item.HidePipes();
        }
        if (GUILayout.Button("ShowPipes"))
        {
            item.ShowPipes();
        }
        if (GUILayout.Button("ShowAll"))
        {
            TransformHelper.ShowAll(item.gameObject);
        }
        EditorGUILayout.EndHorizontal();

        BaseFoldoutEditor<GameObject>.DrawObjectList(targetListArg, "Targets", item.Targets, null, null, null);

        EditorGUILayout.BeginHorizontal();
        //item.IsSameName = GUILayout.Toggle(item.IsSameName, "SameName");

        if (GUILayout.Button("LoadModels"))
        {
            item.LoadModels(null);
        }
        if (GUILayout.Button("FindByUID"))
        {
            item.FindObjectByUID();
        }
        GUILayout.Label("Log", GUILayout.Width(40));
        item.IsShowLog = EditorGUILayout.Toggle(item.IsShowLog, GUILayout.Width(20));
        GUILayout.Label("Level", GUILayout.Width(60));
        item.FindPosLevel = EditorGUILayout.IntField(item.FindPosLevel, GUILayout.Width(50));
        if (GUILayout.Button("FindByPos"))
        {
            //item.FindObjectByPos(true,null);
            item.FindObjectByPos(null);
        }
        //if (GUILayout.Button("FindByPos(NoLog)"))
        //{
        //    item.FindObjectByPos1234(false, null);
        //}
        if (GUILayout.Button("FindByPos2(NoNo)"))
        {
            item.FindObjectByPos_NoDrawableAndNoZero(null);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Find>Name", GUILayout.Width(80));
        item.checkResultArg.IsByNameAfterFindModel = EditorGUILayout.Toggle(item.checkResultArg.IsByNameAfterFindModel, GUILayout.Width(20));
        GUILayout.Label("NotFind>Name", GUILayout.Width(90));
        item.checkResultArg.IsByNameAfterNotFindModel = EditorGUILayout.Toggle(item.checkResultArg.IsByNameAfterNotFindModel, GUILayout.Width(20));
        GUILayout.Label("MoreDis", GUILayout.Width(65));
        item.checkResultArg.IsMoreDistance = EditorGUILayout.Toggle(item.checkResultArg.IsMoreDistance, GUILayout.Width(20));
        GUILayout.Label("OnlyName", GUILayout.Width(65));
        item.checkResultArg.IsOnlyName = EditorGUILayout.Toggle(item.checkResultArg.IsOnlyName, GUILayout.Width(20));
        GUILayout.Label("Closed1", GUILayout.Width(60));
        item.checkResultArg.IsFindClosed1 = EditorGUILayout.Toggle(item.checkResultArg.IsFindClosed1, GUILayout.Width(20));
        GUILayout.Label("Closed2", GUILayout.Width(60));
        item.checkResultArg.IsFindClosed2 = EditorGUILayout.Toggle(item.checkResultArg.IsFindClosed2, GUILayout.Width(20));
        GUILayout.Label("Found2", GUILayout.Width(60));
        item.checkResultArg.IsUseFound2 = EditorGUILayout.Toggle(item.checkResultArg.IsUseFound2, GUILayout.Width(20));
        if (GUILayout.Button("Pos1"))
        {
            item.TestFindObjectByPos(item.MinDistanceLv1);
        }
        if (GUILayout.Button("Pos2"))
        {
            item.TestFindObjectByPos(item.MinDistanceLv2);
        }
        if (GUILayout.Button("Pos3"))
        {
            item.TestFindObjectByPos(item.MinDistanceLv3);
        }
        if (GUILayout.Button("Pos4"))
        {
            item.TestFindObjectByPos(item.MinDistanceLv4);
        }
        if (GUILayout.Button("Pos5"))
        {
            item.TestFindObjectByPos(item.MinDistanceLv5);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateTree"))
        {
            item.CreateTree();
        }
        if (GUILayout.Button("ClearResult"))
        {
            item.ClearResult();
        }
        if (GUILayout.Button("SaveXml"))
        {
            item.SaveXml();
        }

        if (GUILayout.Button("TestFind"))
        {
            item.TestFindModelByName();
        }
        if (GUILayout.Button("RemoveRepeated"))
        {
            item.RemoveRepeated();
        }
        if (GUILayout.Button("ClearRendererId"))
        {
            item.ClearRendererId();
        }
        if (GUILayout.Button("OnlySelf"))
        {
            item.SetOnlySelfModel();
        }
        EditorGUILayout.EndHorizontal();

        DrawBimList(item, bimListArg0, "Bim List");
        BaseFoldoutEditorHelper.DrawTransformList(item.transformListAll, transListArg_All, null, "Transform List(All)");
        BaseFoldoutEditorHelper.DrawTransformList(item.transformList, transListArg, null, "** Transform List(Fiterd) **");
        if(item.TransformDict!=null)
            BaseFoldoutEditorHelper.DrawTransformList(item.GetCurrentTransformList(), transListArg2, null, "Transform List(Current)");

        DrawBimAreas(item);
        //DrawVueModelList(item.repeatModels, modelListArg0, "Model List(RepeatedRenderer)");

        DrawVueModelListEx(item);

        DrawBimList(item.foundBimInfos1, bimListArg1, $"Found Bim List1({InitNavisFileInfoByModelSetting.Instance.MinDistance1})");
        DrawBimList(item.foundBimInfos2, bimListArg2, $"Found Bim List2({InitNavisFileInfoByModelSetting.Instance.MinDistance2})");
        DrawBimList(item.foundBimInfos3, bimListArg3, $"Found Bim List3({InitNavisFileInfoByModelSetting.Instance.MinDistance3})");
    }



    private static void DrawRepeatedModels(string name, FoldoutEditorArg listArg,NavisModelRoot item, Dictionary<string, List<ModelItemInfo>> dictList)
    {
        //var listArg = repeatedModelsArg;
        //string name = "RepeatedModels";
        listArg.caption = $"{name}";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var doors = dictList;
            int count = doors.Count;
            arg.caption = $"{name} ({doors.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors.Keys.ToList());
        }, null);
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = dictList;
            InitEditorArg(list1.Keys.ToList());
            listArg.DrawPageToolbar(list1.Keys.ToList(), (listItem, i) =>
            {
                if (listItem == null) return;

                var list2 = list1[listItem];
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i}]{listItem}({list2.Count})";

                EditorUIUtils.ToggleFoldout(doorRootArg, null, () =>
                {
                });

                if (doorRootArg.isEnabled && doorRootArg.isExpanded)
                {
                    InitEditorArg(list2);
                    doorRootArg.DrawPageToolbar(list2, (listItem2, i2) =>
                    {
                        if (listItem2 == null) return;
                        var doorRootArg2 = FoldoutEditorArgBuffer.editorArgs[listItem2];
                        doorRootArg2.level = 2;
                        doorRootArg2.background = true;
                        doorRootArg2.caption = $"[{i2}]{listItem2.Name}|{listItem2.Id}|{listItem2.GetPath()}" ;
                        //doorRootArg2.info = listItem2.ToString();

                        EditorUIUtils.ObjectFoldout(doorRootArg2, listItem2, () =>
                        {
                        });
                    });
                }
            });
        }
    }

    private static void DrawBimAreas(NavisModelRoot item)
    {
        //bimAreasArg
        var listArg = bimAreasArg;
        string name = "BIM Areas";
        listArg.caption = $"{name}";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var doors = item.BimDict.bimAreas;
            int count = doors.Count;
            arg.caption = $"{name} ({doors.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors.Keys.ToList());
        },null);
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = item.BimDict.bimAreas;
            InitEditorArg(list1.Keys.ToList());
            listArg.DrawPageToolbar(list1.Keys.ToList(), (listItem, i) =>
            {
                if (listItem == null) return;

                var list2 = list1[listItem];
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i}]{listItem}({list2.Count})";
                
                EditorUIUtils.ToggleFoldout(doorRootArg, null, () =>
                {
                });

                if (doorRootArg.isEnabled && doorRootArg.isExpanded)
                {
                    InitEditorArg(list2);
                    listArg.DrawPageToolbar(list2, (listItem2, i2) =>
                    {
                        if (listItem2 == null) return;
                        var doorRootArg2 = FoldoutEditorArgBuffer.editorArgs[listItem2];
                        doorRootArg2.level = 2;
                        doorRootArg2.background = true;
                        doorRootArg2.caption = $"[{i2}]{listItem2.name}";
                        doorRootArg2.info = listItem2.ToString();

                        EditorUIUtils.ObjectFoldout(doorRootArg2, listItem2, () =>
                        {
                        });
                    });
                }
            });
        }
    }

    private static void DrawBimList(NavisModelRoot item, FoldoutEditorArg<BIMModelInfo> listArg, string name)
    {
        listArg.caption = $"{name}";
        listArg.level = 0;
        var list1= item.bimInfos;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            //var doors = item.bimInfos;
            int count = list1.Count;
            if (!string.IsNullOrEmpty(listArg.searchKey))
            {
                list1 = list1.FindList(listArg.searchKey);
            }
            arg.caption = $"{name} ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(list1);
        },
        () =>
        {
            if (GUILayout.Button("Update",GUILayout.Width(70)))
            {
                item.RefreshBIMList();
            }
            if (GUILayout.Button("Clear", GUILayout.Width(70)))
            {
                //InitNavisFileInfoByModel.DestoryNavisInfo(item.gameObject);
                //item.GetBims(null, null);
                item.ClearBimInfos();
            }
            if (GUILayout.Button("ClearAll", GUILayout.Width(80)))
            {
                InitNavisFileInfoByModel.DestoryNavisInfo(null);
                item.RefreshBIMList();
            }

        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            GUILayout.BeginHorizontal();

            //FilterBy BIMFoundType
            if (listArg.tag == null)
            {
                listArg.tag = BIMFoundType.None;
            }
            BIMFoundType foundType = (BIMFoundType)listArg.tag;
            foundType = (BIMFoundType)EditorGUILayout.EnumPopup(foundType);
            listArg.tag = foundType;
            if (foundType != BIMFoundType.None)
            {
                list1 = list1.FindListByType(foundType);
            }
            else
            {

            }

            if (GUILayout.Button("RemoveNotFound"))
            {
                item.RefreshBIMList();
            }
            //if (GUILayout.Button("Compare"))
            //{
            //    item.CompareModelVueInfo_Model2Vue();
            //}

            //listArg.searchKey = GUILayout.TextField(listArg.searchKey, GUILayout.Width(300));
            //if (GUILayout.Button("X", GUILayout.Width(20)))
            //{
            //    listArg.searchKey = "";
            //}

            BaseFoldoutEditorHelper.DrawSearchInput(listArg);
            GUILayout.EndHorizontal();

            //var list1 = item.bimInfos;
            InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] [{listItem.IsFound}][{listItem.FoundType}] {listItem.name}>{listItem.MName}({listItem.MId})";
                doorRootArg.info = listItem.GetItemText();
                EditorUIUtils.ObjectFoldout(doorRootArg, listItem.gameObject, () =>
                {
                });
            });
        }
    }

    private static void DrawVueModelList(NavisModelRoot root,List<ModelItemInfo> models, FoldoutEditorArg<ModelItemInfo> listArg,string nameList)
    {
        listArg.caption = $"{nameList}";
        listArg.level = 0;

        List<ModelItemInfo> list1 = models;
        if (list1 == null) return;

        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            //var list1 = models;
            if (!string.IsNullOrEmpty(listArg.searchKey))
            {
                list1 = list1.FindAll(i => i.Name.Contains(listArg.searchKey));
            }
            int count = list1.Count;
            arg.caption = $"{nameList} ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(list1);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    item.GetVueModels(false);
            //}

            //if (GUILayout.Button("Compare"))
            //{
            //    item.CompareModelVueInfo_Vue2Model();
            //}

            //if (GUILayout.Button("Clear"))
            //{
            //    item.GetVueModels(true);
            //}

            //listArg.searchKey = GUILayout.TextField(listArg.searchKey, GUILayout.Width(300));
            //if (GUILayout.Button("X", GUILayout.Width(20)))
            //{
            //    listArg.searchKey = "";
            //}

            BaseFoldoutEditorHelper.DrawSearchInput(listArg);
        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            //var list1 = models;
            InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                string itemName = listItem.Name;
                var p = listItem.GetParent();
                if (p != null)
                {
                    itemName = p.Name + "\\" + itemName;
                    if (p.GetParent() != null)
                    {
                        itemName = p.GetParent().Name + "\\" + itemName;
                    }
                }
                
                doorRootArg.caption = $"[{i + 1:00}] {itemName}|{listItem.Id}|{listItem.Type}";
                //doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.Id}|{listItem.UId}|{listItem.RenderId})";
                doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.X},{listItem.Y},{listItem.Z})({!string.IsNullOrEmpty(listItem.UId)}|{!string.IsNullOrEmpty(listItem.RenderId)})";
                EditorUIUtils.ObjectFoldout(doorRootArg, listItem.Tag as GameObject, () =>
                {
                    if (GUILayout.Button("Id",GUILayout.Width(30)))
                    {
                        Debug.Log($"name:{listItem.Name}, id:{listItem.Id}, uid:{listItem.UId}, rid:{listItem.RenderId},path:{listItem.GetPath()}");
                    }
                    if (GUILayout.Button("Pos", GUILayout.Width(30)))
                    {
                        Vector3 pos = listItem.GetPositon();
                        GameObject posObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        posObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        posObj.transform.position = pos;
                        posObj.name = listItem.Name+ "_Pos";
                    }
                    if (GUILayout.Button("Find", GUILayout.Width(50)))
                    {
                        root.TestFindObjectByModel(listItem);
                    }
                });
            });
        }
    }

    private static void DrawBimList(List<BIMModelInfo> bimInfos, FoldoutEditorArg<BIMModelInfo> listArg, string name)
    {
        listArg.caption = $"{name}";
        listArg.level = 0;
        if (bimInfos == null) return;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var doors = bimInfos;
            int count = doors.Count;
            arg.caption = $"{name} ({doors.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    item.GetBims();
            //}
            //if (GUILayout.Button("Compare"))
            //{
            //    item.CompareModelVueInfo_Model2Vue();
            //}
        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = bimInfos;
            InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] {listItem.name}[{listItem.IsFound}]";
                doorRootArg.info = listItem.GetItemText();
                EditorUIUtils.ObjectFoldout(doorRootArg, listItem.gameObject, () =>
                {
                });
            });
        }
    }

    public override void OnToolLayout(NavisModelRoot item)
    {
        base.OnToolLayout(item);

        DrawUI(item);

        //if (GUILayout.Button("Window"))
        //{
        //    InitNavisFileInfoByModelWindow.ShowWindow();
        //}
    }
}
