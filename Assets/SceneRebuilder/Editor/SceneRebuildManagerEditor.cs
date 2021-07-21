using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Linq;

[CustomEditor(typeof(SceneRebuildManager))]
public class SceneRebuildManagerEditor : BaseFoldoutEditor<SceneRebuildManager>
{
    //private SerializedProperty buildingListFoldout;

    private FoldoutEditorArg buildingListArg=new FoldoutEditorArg();
    private FoldoutEditorArg treeListArg=new FoldoutEditorArg();

    private FoldoutEditorArg nodeListArg=new FoldoutEditorArg();

    private FoldoutEditorArg sceneListArg=new FoldoutEditorArg();

    private FoldoutEditorArg<MeshFilter> meshListArg=new FoldoutEditorArg<MeshFilter>();

    private FoldoutEditorArg matListArg = new FoldoutEditorArg();

    //private SceneRebuildManager manager;

    //private List<MeshFilter> meshFilters = new List<MeshFilter>();

    public override void OnEnable()
    {
        base.OnEnable();
        //manager = target as SceneRebuildManager;
        UpdateList();
       
        Debug.LogError("SceneRebuildManagerEditor.OnEnable");

        treeListArg = new FoldoutEditorArg(true, false, true, true, false);
        nodeListArg = new FoldoutEditorArg(true, false, true, true, false);
        sceneListArg = new FoldoutEditorArg(true, false, true, true, false);

        meshListArg = new FoldoutEditorArg<MeshFilter>(true, false, true, true, false);
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
        NewButton("1.InitModels", buttonWidth, true, item.InitBuildings);
        // NewButton("ListWindow:" + count, buttonWidth, true, () =>
        //      {
        //          SceneRebuildEditorWindow.ShowWindow();
        //      });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateTrees", buttonWidth, true, item.CombineBuildings);
        NewButton("RemoveTrees", buttonWidth, true, item.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes", buttonWidth, true, item.SaveScenes);
        NewButton("LoadScenes", buttonWidth, true, item.LoadScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("4.SetBuildings", buttonWidth, true, item.SetBuildings);
        NewButton("ClearBuildings", buttonWidth, true, item.ClearBuildings);
        EditorGUILayout.EndHorizontal();

        // if (GUILayout.Button("SetBuildings", contentStyle))
        // {
        //     item.SetBuildings();
        // }
        if (GUILayout.Button("OneKey", contentStyle))
        {
            item.OneKey();
        }
    }

    public override void OnToolLayout(SceneRebuildManager item)
    {
        System.DateTime startT=System.DateTime.Now;

        base.OnToolLayout(item);
        DrawManagerToolbar(item);

        IsShowList = GUILayout.Toggle(IsShowList, "IsShowList");
        if (IsShowList == false) return;

        Debug.Log($"------------------------------------------------------------------------");
        EditorUIUtils.SetupStyles();
        //-------------------------------------------------------BuildingList-----------------------------------------------------------
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
        DrawTreeList(treeListArg, () =>
        {
            return item.GetTrees().Where(t => t.VertexCount > 0).ToList();
        });

        //-------------------------------------------------------NodeList-----------------------------------------------------------
        DrawNodeList(nodeListArg, item.GetLeafNodes);

        //-------------------------------------------------------SceneList-----------------------------------------------------------
        DrawSceneList(sceneListArg, item.GetScenes);

        //-------------------------------------------------------MeshList-----------------------------------------------------------
        DrawMeshList(meshListArg, GetMeshList);

        DrawMatList(GlobalMaterialManager.Instance, matListArg);

        var timeT=System.DateTime.Now-startT;
        Debug.Log($"SceneRebuildManagerEditor time:{timeT.TotalMilliseconds:F1}ms ");
    }

    private List<MeshFilter> GetMeshList()
    {
        return GameObject.FindObjectsOfType<MeshFilter>(true).Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
    }
}
