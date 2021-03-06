using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Linq;
using System;

[CustomEditor(typeof(SceneRebuildManager))]
public class SceneRebuildManagerEditor : BaseFoldoutEditor<SceneRebuildManager>
{
    //private SerializedProperty buildingListFoldout;

    private FoldoutEditorArg buildingListArg=new FoldoutEditorArg();
    private FoldoutEditorArg treeListArg=new FoldoutEditorArg();

    private FoldoutEditorArg nodeListArg=new FoldoutEditorArg();

    private FoldoutEditorArg sceneListArg=new FoldoutEditorArg();


    private FoldoutEditorArg<MeshRendererInfo> meshinfoListArg = new FoldoutEditorArg<MeshRendererInfo>();

    private FoldoutEditorArg matListArg = new FoldoutEditorArg();

    //private SceneRebuildManager manager;

    //private List<MeshFilter> meshFilters = new List<MeshFilter>();

    public override void OnEnable()
    {
        base.OnEnable();
        //manager = target as SceneRebuildManager;
        UpdateList();
       
        //Debug.LogError("SceneRebuildManagerEditor.OnEnable");

        treeListArg = new FoldoutEditorArg(true, false, true, true, false);
        nodeListArg = new FoldoutEditorArg(true, false, true, true, false);
        sceneListArg = new FoldoutEditorArg(true, false, true, true, false);

        //meshListArg = new FoldoutEditorArg<MeshFilter>(true, false, true, true, false);

        meshinfoListArg = new FoldoutEditorArg<MeshRendererInfo>(true, false, true, true, false);

        matListArg = new FoldoutEditorArg(true, false, true, true, false);

        buildingListArg = new FoldoutEditorArg(true, true, true, true, false);
    }

    public override void UpdateList()
    {
        targetT.UpdateList();
        //DrawMeshList(meshListArg, GetMeshList);
    }

    

    public bool IsShowList = true;

    private void DrawManagerToolbar(SceneRebuildManager item)
    {
        EditorGUILayout.BeginHorizontal();
        NewButton("LoadScenes", true, () =>
        {
            BuildingModelManager.Instance.OneKeyLoadScene(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
        });
        NewButton("SaveScenes", true, () =>
        {
            BuildingModelManager.Instance.OneKeySaveScene(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
        });
        NewButton("LoadScenes(Active)", true, () =>
        {
            BuildingModelManager.Instance.OneKeyLoadScene(GameObject.FindObjectsOfType<BuildingModelInfo>(false));
        });
        NewButton("SaveScenes(Active)", true, () =>
        {
            BuildingModelManager.Instance.OneKeySaveScene(GameObject.FindObjectsOfType<BuildingModelInfo>(false));
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("1.InitModels", buttonWidth, true, item.InitBuildings);
        // NewButton("ListWindow:" + count, buttonWidth, true, () =>
        //      {
        //          SceneRebuildEditorWindow.ShowWindow();
        //      });
        NewButton("MoveRenderers", buttonWidth, true, item.MoveRenderers);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTrees", buttonWidth, true, ()=> {
            AreaTreeManager.Instance.isCombine = false;
            item.CombineBuildings();
            });
        NewButton("2.CreateTrees(C)", buttonWidth, true, () => {
            AreaTreeManager.Instance.isCombine = true;
            item.CombineBuildings();
        });
        NewButton("RemoveTrees", buttonWidth, true, item.ClearTrees);
        NewButton("DestroyBox", buttonWidth, true, item.DestroyBox);

        //AreaTreeManager.Instance.isCombine = GUILayout.Toggle(AreaTreeManager.Instance.isCombine, "Combine");

        var setting = AreaTreeManager.Instance.nodeSetting;
        setting.MinLevel = EditorGUILayout.IntField("", setting.MinLevel, GUILayout.Width(35));
        //setting.MaxLevel = EditorGUILayout.IntField("MaxL", setting.MaxLevel);
        setting.MaxRenderCount = EditorGUILayout.IntField("", setting.MaxRenderCount, GUILayout.Width(40));
        setting.MinRenderCount = EditorGUILayout.IntField("", setting.MinRenderCount, GUILayout.Width(40));
        setting.MaxVertexCount = EditorGUILayout.IntField("", setting.MaxVertexCount, GUILayout.Width(40));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes", buttonWidth, true, item.SaveScenes);
        NewButton("LoadScenes", buttonWidth, true, item.LoadScenes);
        NewButton("UnLoadScenes", buttonWidth, true, item.UnLoadScenes);
        NewButton("RemoveScenes", buttonWidth, true, item.RemoveScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("4.SavePrefabs", buttonWidth, true, item.SavePrefabs);
        NewButton("LoadPrefabs", buttonWidth, true, item.LoadPrefabs);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("5.SetBuildings", buttonWidth, true, item.SetBuildings);
        NewButton("ClearBuildings", buttonWidth, true, item.ClearBuildings);
        NewButton("ShowBuildings", buttonWidth, true, item.ShowBuildings);
        NewButton("CheckSceneIndex", buttonWidth, true, SubSceneManager.Instance.CheckSceneIndex);
        //SubSceneManager.Instance.includeInactive = GUILayout.Toggle(SubSceneManager.Instance.includeInactive, "includeInactive");
        EditorGUILayout.EndHorizontal();

        // if (GUILayout.Button("SetBuildings", contentStyle))
        // {
        //     item.SetBuildings();
        // }
        EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("OneKey_Save", contentStyle))
        //{
        //    item.OneKey();
        //}
        //if (GUILayout.Button("OneKey_Reset", contentStyle))
        //{
        //    item.OneKey();
        //}

        NewButton("OneKey_Save", buttonWidth, true, item.OneKey_Save);
        NewButton("OneKey_Reset", buttonWidth, true, item.OneKey_Reset);
        NewButton("OneKey_Resave", buttonWidth, true, item.OneKey_Resave);
        EditorGUILayout.EndHorizontal();
    }

    public override void OnToolLayout(SceneRebuildManager item)
    {
        System.DateTime startT=System.DateTime.Now;

        base.OnToolLayout(item);
        DrawManagerToolbar(item);

        IsShowList = GUILayout.Toggle(IsShowList, "IsShowList");
        if (IsShowList == false) return;

        //Debug.Log($"------------------------------------------------------------------------");
        EditorUIUtils.SetupStyles();
        //-------------------------------------------------------BuildingList-----------------------------------------------------------
        //Debug.Log($"SceneRebuildManagerEditor6 time:{(System.DateTime.Now - startT).TotalMilliseconds:F1}ms ");
        DrawModelList(buildingListArg, () =>
        {
            return item.GetBuildings().Where(b => b != null).ToList(); ;
        }, () =>
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("ActiveAll"))
            {
                //item.gameObject.SetActive();
                item.SetModelsActive(true);
            }
            if (GUILayout.Button("InactiveAll"))
            {
                item.SetModelsActive(false);
            }
            EditorGUILayout.EndHorizontal();
        });

        //-------------------------------------------------------TreeList-----------------------------------------------------------
        //Debug.Log($"SceneRebuildManagerEditor5 time:{(System.DateTime.Now - startT).TotalMilliseconds:F1}ms ");
        DrawTreeList(treeListArg, () =>
        {
            return item.GetTrees().Where(t => t!=null && t.VertexCount > 0).ToList();
        });

        //-------------------------------------------------------NodeList-----------------------------------------------------------
        //Debug.Log($"SceneRebuildManagerEditor4 time:{(System.DateTime.Now - startT).TotalMilliseconds:F1}ms ");
        DrawNodeList(nodeListArg,true, item.GetLeafNodes);

        //-------------------------------------------------------SceneList-----------------------------------------------------------
        //Debug.Log($"SceneRebuildManagerEditor3 time:{(System.DateTime.Now - startT).TotalMilliseconds:F1}ms ");
        DrawSceneList(sceneListArg, item.GetScenes);

        //-------------------------------------------------------MeshList-----------------------------------------------------------
        //Debug.Log($"SceneRebuildManagerEditor2 time:{(System.DateTime.Now - startT).TotalMilliseconds:F1}ms ");
        //DrawMeshList(meshListArg, GetMeshList);

        DrawMeshRendererInfoListEx(meshinfoListArg, GetMeshInfoList(),GetMeshInfoList);

        //Debug.Log($"SceneRebuildManagerEditor1 time:{(System.DateTime.Now - startT).TotalMilliseconds:F1}ms ");
        DrawMatList(GlobalMaterialManager.Instance, matListArg);

        //Debug.Log($"SceneRebuildManagerEditor time:{(System.DateTime.Now - startT).TotalMilliseconds:F1}ms ");
    }

    private List<MeshFilter> GetMeshList()
    {
        return GameObject.FindObjectsOfType<MeshFilter>(true).Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
    }

    private List<MeshRendererInfo> GetMeshInfoList()
    {
        return GameObject.FindObjectsOfType<MeshRendererInfo>(true).Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
    }
}
