using CodeStage.AdvancedFPSCounter.Editor.UI;
using NavisPlugins.Infos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
            DrawVueModelList(item.ModelList.allModels, modelListArg_all, "Model List(All)");
            DrawVueModelList(item.ModelList.allModels_uid, modelListArg_uid, "Model List(UID)");
            DrawVueModelList(item.ModelList.allModels_noUid, modelListArg_nouid, "Model List(NoUID)");

            DrawVueModelList(item.ModelList.allModels_drawable_zero, modelListArg_drawable_zero, "Model List(Drawable&Zero)");
            DrawVueModelList(item.ModelList.allModels_drawable_nozero, modelListArg_drawable_nozero, "Model List(Drawable&NoZero)");
            DrawVueModelList(item.ModelList.allModels_noDrawable_nozero, modelListArg_nodrawable_nozero, "Model List(NoDrawable&NoZero)");
            DrawVueModelList(item.ModelList.allModels_noDrawable_zero, modelListArg_noDrawable_zero, "Model List(NoDrawable&Zero)");
        }

        EditorUIUtils.Separator(5);

        DrawVueModelList(item.noFoundBimInfos01, bimListArg01, "No Found Bim List01(All)");
        DrawVueModelList(item.noFoundBimInfos02, bimListArg02, "No Found Bim List02(Zero)");
        DrawVueModelList(item.noFoundBimInfos03, bimListArg03, "No Found Bim List03(NotAero)");
        DrawVueModelList(item.noFoundBimInfos04, bimListArg04, "No Found Bim List04(NotDrawable)");
        EditorUIUtils.Separator(5);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public static void DrawUI(NavisModelRoot item)
    {
        if (item == null) return;

        EditorGUILayout.BeginHorizontal();
        item.MinDistance = EditorGUILayout.FloatField(item.MinDistance, GUILayout.Width(70));
        if (GUILayout.Button("BindBimInfo"))
        {
            item.BindBimInfo(null);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //GUILayout
        item.ModelName = EditorGUILayout.TextField("ModelName", item.ModelName);
        item.includeInactive = EditorGUILayout.Toggle("includeInactive",item.includeInactive);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        item.TargetRoot = BaseEditorHelper.ObjectField(item.TargetRoot);
        if (GUILayout.Button("FindRelativeTargets"))
        {
            item.FindRelativeTargets();
        }
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
        if (GUILayout.Button("FindObjectByUID"))
        {
            item.FindObjectByUID();
        }
        if (GUILayout.Button("FindObjectByPos"))
        {
            item.FindObjectByPos(null);
        }

        if (GUILayout.Button("CreateTree"))
        {
            item.CreateTree();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
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
        if (GUILayout.Button("OnlySelf"))
        {
            item.SetOnlySelfModel();
        }
        EditorGUILayout.EndHorizontal();

        DrawBimList(item, bimListArg0, "Bim List");

        BaseFoldoutEditorHelper.DrawTransformList(item.transformList, transListArg, null, "Transform List(All)");
        if(item.TransformDict!=null)
            BaseFoldoutEditorHelper.DrawTransformList(item.TransformDict.dict.Keys.ToList(), transListArg2, null, "Transform List(Current)");

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
                list1 = list1.FindAll(i => i.name.Contains(listArg.searchKey)||i.MName.Contains(listArg.searchKey));
            }
            arg.caption = $"{name} ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(list1);
        },
        () =>
        {

           

            if (GUILayout.Button("Update"))
            {
                item.GetBims(null, null);
            }
            if (GUILayout.Button("RemoveNotFound"))
            {
                item.GetBims(null, null);
            }
            if (GUILayout.Button("Clear"))
            {
                InitNavisFileInfoByModel.DestoryNavisInfo(item.gameObject);
                item.GetBims(null, null);
            }
            if (GUILayout.Button("ClearAll"))
            {
                InitNavisFileInfoByModel.DestoryNavisInfo(null);
                item.GetBims(null, null);
            }
            //if (GUILayout.Button("Compare"))
            //{
            //    item.CompareModelVueInfo_Model2Vue();
            //}

            listArg.searchKey = GUILayout.TextField(listArg.searchKey, GUILayout.Width(100));
        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            //var list1 = item.bimInfos;
            InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] [{listItem.IsFound}] {listItem.name}";
                doorRootArg.info = listItem.GetItemText();
                EditorUIUtils.ObjectFoldout(doorRootArg, listItem.gameObject, () =>
                {
                });
            });
        }
    }

    private static void DrawVueModelList(List<ModelItemInfo> models, FoldoutEditorArg<ModelItemInfo> listArg,string nameList)
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

            listArg.searchKey = GUILayout.TextField(listArg.searchKey, GUILayout.Width(100));
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
                doorRootArg.caption = $"[{i + 1:00}] {listItem.Name}|{listItem.Id}|{listItem.Type}";
                //doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.Id}|{listItem.UId}|{listItem.RenderId})";
                doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.X},{listItem.Y},{listItem.Z})({!string.IsNullOrEmpty(listItem.UId)}|{!string.IsNullOrEmpty(listItem.RenderId)})";
                EditorUIUtils.ObjectFoldout(doorRootArg, listItem.Tag as GameObject, () =>
                {
                    if (GUILayout.Button("Id",GUILayout.Width(30)))
                    {
                        Debug.Log($"name:{listItem.Name}, id:{listItem.Id}, uid:{listItem.UId}, rid:{listItem.RenderId}");
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
