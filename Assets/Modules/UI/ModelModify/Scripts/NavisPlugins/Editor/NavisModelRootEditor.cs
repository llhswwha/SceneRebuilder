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

    static FoldoutEditorArg<ModelItemInfo> bimListArg01 = new FoldoutEditorArg<ModelItemInfo>(true,false);
    static FoldoutEditorArg<ModelItemInfo> bimListArg02 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> bimListArg03 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> bimListArg04 = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<BIMModelInfo> bimListArg0 = new FoldoutEditorArg<BIMModelInfo>(true, false);
    static FoldoutEditorArg<BIMModelInfo> bimListArg1 = new FoldoutEditorArg<BIMModelInfo>(true, false);
    static FoldoutEditorArg<BIMModelInfo> bimListArg2 = new FoldoutEditorArg<BIMModelInfo>(true, false);
    static FoldoutEditorArg<BIMModelInfo> bimListArg3 = new FoldoutEditorArg<BIMModelInfo>(true, false);

    static FoldoutEditorArg<ModelItemInfo> modelListArg1 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg2 = new FoldoutEditorArg<ModelItemInfo>(true, false);
    static FoldoutEditorArg<ModelItemInfo> modelListArg3 = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<Transform> transListArg = new FoldoutEditorArg<Transform>(true, false);


    public override void OnEnable()
    {
        base.OnEnable();
    }

    public static void DrawUI(NavisModelRoot item)
    {
        if (item == null) return;
        EditorGUILayout.BeginHorizontal();
        //item.IsSameName = GUILayout.Toggle(item.IsSameName, "SameName");
        if (GUILayout.Button("OnlySelf"))
        {
            item.SetOnlySelfModel();
        }
        if (GUILayout.Button("LoadModels"))
        {
            item.LoadModels();
        }
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
        if (GUILayout.Button("DestroyBims"))
        {
            InitNavisFileInfoByModel.DestoryNavisInfo(item.gameObject);
        }
        if (GUILayout.Button("TestFind"))
        {
            item.TestFindModelByName();
        }
        EditorGUILayout.EndHorizontal();

        DrawBimList(item, bimListArg0, "Bim List");

        BaseFoldoutEditorHelper.DrawTransformList(item.transformList, transListArg, null, "Transform List");

        DrawVueModelList(item.allModels, modelListArg1, "Model List");
        DrawVueModelList(item.allModels_noDrawable, modelListArg2, "Model(NoDrawable) List");
        DrawVueModelList(item.allModels_zero, modelListArg3, "Model(Zero) List");

        DrawVueModelList(item.noFoundBimInfos01, bimListArg01, "No Found Bim List01(All)");
        DrawVueModelList(item.noFoundBimInfos02, bimListArg02, "No Found Bim List02(Zero)");
        DrawVueModelList(item.noFoundBimInfos03, bimListArg03, "No Found Bim List03(NotAero)");
        DrawVueModelList(item.noFoundBimInfos04, bimListArg04, "No Found Bim List04(NotDrawable)");

        DrawBimList(item.foundBimInfos1, bimListArg1, $"Found Bim List1({InitNavisFileInfoByModelSetting.Instance.MinDistance1})");
        DrawBimList(item.foundBimInfos2, bimListArg2, $"Found Bim List2({InitNavisFileInfoByModelSetting.Instance.MinDistance2})");
        DrawBimList(item.foundBimInfos3, bimListArg3, $"Found Bim List3({InitNavisFileInfoByModelSetting.Instance.MinDistance3})");
    }

    private static void DrawBimList(NavisModelRoot item, FoldoutEditorArg<BIMModelInfo> listArg, string name)
    {
        listArg.caption = $"{name}";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var doors = item.bimInfos;
            int count = doors.Count;
            arg.caption = $"{name} ({doors.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors);
        },
        () =>
        {
            if (GUILayout.Button("Update"))
            {
                item.GetBims();
            }
            //if (GUILayout.Button("Compare"))
            //{
            //    item.CompareModelVueInfo_Model2Vue();
            //}
        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = item.bimInfos;
            InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] {listItem.name}";
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
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var list1 = models;
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
        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = models;
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
