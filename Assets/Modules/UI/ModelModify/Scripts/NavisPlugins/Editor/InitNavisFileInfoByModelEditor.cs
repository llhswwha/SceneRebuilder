using CodeStage.AdvancedFPSCounter.Editor.UI;
using NavisPlugins.Infos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InitNavisFileInfoByModel))]
public class InitNavisFileInfoByModelEditor : BaseFoldoutEditor<InitNavisFileInfoByModel>
{
    static FoldoutEditorArg<BIMModelInfo> bimListArg = new FoldoutEditorArg<BIMModelInfo>(true,false);

    static FoldoutEditorArg<ModelItemInfo> vueRootListArg = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<ModelItemInfo> vueAllListArg = new FoldoutEditorArg<ModelItemInfo>(true, false);

    static FoldoutEditorArg<Transform> transListArg = new FoldoutEditorArg<Transform>(true, false);

    static FoldoutEditorArg<GameObject> buildingListArg = new FoldoutEditorArg<GameObject>(true, false);

    // Start is called before the first frame update

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public static void DrawUI(InitNavisFileInfoByModel item)
    {
        if (item == null) return;
        //return;

        DateTime start = DateTime.Now;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("DestoryNavisInfo"))
        {
            item.DestoryNavisInfo();
        }
        if (GUILayout.Button("GetCompareList"))
        {
            item.GetCompareList();
        }
        if (GUILayout.Button("ClearRendererId"))
        {
            item.ClearRendererId();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CompareModel_Model2Vue"))
        {
            item.CompareModelVueInfo_Model2Vue();
        }
        if (GUILayout.Button("CompareModel_Vue2Model"))
        {
            item.CompareModelVueInfo_Vue2Model();
        }
        EditorGUILayout.EndHorizontal();

EditorGUILayout.BeginHorizontal();
if (GUILayout.Button("BindBimInfo"))
{
    item.BindBimInfo();
}
EditorGUILayout.EndHorizontal();
        //Debug.Log($"InitNavisFileInfoByModelEditor t1:{DateTime.Now - start}");

        //ObjectField(item.LocalTarget);
        //DrawBuildingList(item);
        BaseFoldoutEditorHelper.DrawGameObjectList("Building List", InitNavisFileInfoByModelSetting.Instance.initInfoBuildings, buildingListArg, () => 
        {
            if (GUILayout.Button("Update",GUILayout.Width(60)))
            {
                item.UpdateBuildings();
            }
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                item.ClearBuildings();
            }
            if (GUILayout.Button("InitName", GUILayout.Width(70)))
            {
                item.InitBuildingNames();
            }
        });

        //Debug.Log($"InitNavisFileInfoByModelEditor t2:{DateTime.Now - start}"); 

        BaseFoldoutEditorHelper.DrawTransformList(item.GetModels(), transListArg, ()=> { item.GetAllModelTransform(); },"Model List");
        //Debug.Log($"InitNavisFileInfoByModelEditor t3:{DateTime.Now - start}"); 

        DrawVueRootModelList(item);

        DrawVueModelList(item);
        //Debug.Log($"InitNavisFileInfoByModelEditor t4:{DateTime.Now - start}"); 

        DrawBimList(item, bimListArg, "BimInfo List");
        //Debug.Log($"InitNavisFileInfoByModelEditor t5:{DateTime.Now - start}"); 
    }

    private static void DrawBuildingList(InitNavisFileInfoByModel item)
    {
        buildingListArg.caption = $"Building List";
        buildingListArg.level = 0;
        EditorUIUtils.ToggleFoldout(buildingListArg, arg =>
        {
            var list1 = InitNavisFileInfoByModelSetting.Instance.initInfoBuildings;
            int count = list1.Count;
            arg.caption = $"Building List ({list1.Count})";
            InitEditorArg(list1);
        },
        () =>
        {
            if (GUILayout.Button("Update"))
            {
                item.GetAllModelTransform();
            }
        });
        if (buildingListArg.isEnabled && buildingListArg.isExpanded)
        {
            var list1 = InitNavisFileInfoByModelSetting.Instance.initInfoBuildings;
            InitEditorArg(list1);
            buildingListArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] {listItem.name}";
                //doorRootArg.info = $"{listItem.Distance}|{listItem.MId}|{listItem.MName}";
                EditorUIUtils.ObjectFoldout(doorRootArg, listItem.gameObject, () =>
                {
                });
            });
        }
    }


    private static void DrawBimList(InitNavisFileInfoByModel item, FoldoutEditorArg<BIMModelInfo> listArg, string name)
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
            if (GUILayout.Button("Compare"))
            {
                item.CompareModelVueInfo_Model2Vue();
            }
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

    private static void DrawVueRootModelList(InitNavisFileInfoByModel item)
    {
        vueRootListArg.caption = $"VueRootModel List";
        vueRootListArg.level = 0;
        EditorUIUtils.ToggleFoldout(vueRootListArg, arg =>
        {
            var list1 = item.vueRootModels;
            int count = list1.Count;
            arg.caption = $"VueRootModel List ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(list1);
        },
        () =>
        {
            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                item.GetVueModels(false);
            }
            if (GUILayout.Button("Find",GUILayout.Width(50)))
            {
                item.FindVueBuildingsByName();
            }
            if (GUILayout.Button("Sort", GUILayout.Width(50)))
            {
                item.SortVueRootModels();
            }
            if (GUILayout.Button("Save", GUILayout.Width(50)))
            {
                item.SaveNavisFile();
            }
            if (GUILayout.Button("TestSave", GUILayout.Width(90)))
            {
                item.TestSave();
            }
        });
        if (vueRootListArg.isEnabled && vueRootListArg.isExpanded)
        {
            var list1 = item.vueRootModels;
            InitEditorArg(list1);
            vueRootListArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] {listItem.Name}|{listItem.Id}";
                //doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.Id}|{listItem.UId}|{listItem.RenderId})";
                doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.X},{listItem.Y},{listItem.Z})({!string.IsNullOrEmpty(listItem.UId)}|{!string.IsNullOrEmpty(listItem.RenderId)})";
                EditorUIUtils.ObjectFoldout(doorRootArg, listItem.AreaGo, () =>
                {
                });
            });
        }
    }

    private static void DrawVueModelList(InitNavisFileInfoByModel item)
    {
        vueAllListArg.caption = $"VueAllModel List";
        vueAllListArg.level = 0;
        EditorUIUtils.ToggleFoldout(vueAllListArg, arg =>
        {
            var list1 = item.GetVueAllModels();
            int count = list1.Count;
            arg.caption = $"VueAllModel List ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(list1);
        },
        () =>
        {
            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                item.GetVueModels(false);
            }

            if (GUILayout.Button("Compare", GUILayout.Width(60)))
            {
                item.CompareModelVueInfo_Vue2Model();
            }

            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                item.GetVueModels(true);
            }

            if (GUILayout.Button("Save", GUILayout.Width(50)))
            {
                item.SaveNavisFile();
            }
        });
        if (vueAllListArg.isEnabled && vueAllListArg.isExpanded)
        {
            var list1 = item.GetVueAllModels();
            InitEditorArg(list1);
            vueAllListArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] {listItem.Name}|{listItem.Id}";
                //doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.Id}|{listItem.UId}|{listItem.RenderId})";
                doorRootArg.info = $"({listItem.AreaId}|{listItem.AreaName})({listItem.X},{listItem.Y},{listItem.Z})({!string.IsNullOrEmpty(listItem.UId)}|{!string.IsNullOrEmpty(listItem.RenderId)})";
                EditorUIUtils.ObjectFoldout(doorRootArg, null, () =>
                {
                });
            });
        }
    }

    public override void OnToolLayout(InitNavisFileInfoByModel item)
    {
        //DateTime start = DateTime.Now;
        base.OnToolLayout(item);
        //Debug.Log($"InitNavisFileInfoByModelEditor OnToolLayout t1:{DateTime.Now - start}");

        DrawUI(item);
        //Debug.Log($"InitNavisFileInfoByModelEditor OnToolLayout t2:{DateTime.Now - start}");

        if (GUILayout.Button("Window"))
        {
            InitNavisFileInfoByModelWindow.ShowWindow();
        }
        
    }

    //public override void OnInspectorGUI()
    //{
    //    DateTime start = DateTime.Now;

    //    contentStyle = new GUIStyle(EditorStyles.miniButton);
    //    contentStyle.alignment = TextAnchor.MiddleLeft;

    //    EditorUIUtils.SetupStyles();

    //    EditorUIUtils.ToggleFoldout(toolbarArg, null, null);
    //    if (toolbarArg.isExpanded && toolbarArg.isEnabled)
    //    {
    //        base.OnToolLayout(targetT);
    //    }
    //    //Debug.Log($"InitNavisFileInfoByModelEditor OnInspectorGUI t1:{DateTime.Now - start}");

    //    EditorUIUtils.ToggleFoldout(propertyArg, null, null);
    //    if (propertyArg.isExpanded)
    //    {
    //        base.OnInspectorGUI();
    //    }

    //    //Debug.Log($"InitNavisFileInfoByModelEditor OnInspectorGUI t2:{DateTime.Now - start}");
    //}
}
