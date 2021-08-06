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
        NewButton("1.InitModels", buttonWidth, true, item.InitBuildings);
        // NewButton("ListWindow:" + count, buttonWidth, true, () =>
        //      {
        //          SceneRebuildEditorWindow.ShowWindow();
        //      });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTrees", buttonWidth, true, item.CombineBuildings);
        NewButton("RemoveTrees", buttonWidth, true, item.ClearTrees);
        AreaTreeManager.Instance.isCombine = GUILayout.Toggle(AreaTreeManager.Instance.isCombine, "Combine");
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

        DrawMeshListEx(meshinfoListArg, GetMeshInfoList);

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
