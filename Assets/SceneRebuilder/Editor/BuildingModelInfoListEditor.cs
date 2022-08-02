using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingModelInfoList))]
public class BuildingModelInfoListEditor : BaseFoldoutEditor<BuildingModelInfoList>
{
    private FoldoutEditorArg buildingListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        buildingListArg.isEnabled = true;
        buildingListArg.isExpanded = true;
        targetT.UpdateBuildings();
    }

    public override void OnToolLayout(BuildingModelInfoList item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        NewButton("LoadScenes", true, () =>
        {
            BuildingModelManager.Instance.OneKeyLoadScene(item.Buildings);
        });
        NewButton("SaveScenes", true, () =>
        {
            BuildingModelManager.Instance.OneKeySaveScene(item.Buildings);
        });

        EditorGUILayout.EndHorizontal();
        NewButton("SetStaticCulling", true, () =>
        {
            BuildingModelManager.Instance.SetStaticCulling(item.Buildings);
        });
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("1.InitModels", buttonWidth, true, ()=>
        {
            BuildingModelManager.Instance.InitBuildings(item.Buildings.ToList());
        });
        NewButton("ShowRenderers", buttonWidth, true, () =>
        {
            //BuildingModelManager.Instance.InitBuildings(item.Buildings.ToList());
            BuildingModelInfoList.ShowRenderers(item.Buildings.ToList());
        });
        // NewButton("ListWindow:" + count, buttonWidth, true, () =>
        //      {
        //          SceneRebuildEditorWindow.ShowWindow();
        //      });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTrees", buttonWidth, true, ()=>
        {
            BuildingModelManager.Instance.CombineBuildings(item.Buildings.ToList());
        });
        NewButton("RemoveTrees", buttonWidth, true, ()=>
        {
            BuildingModelManager.Instance.ClearTrees(item.Buildings.ToList());
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes", buttonWidth, true, ()=>
        {
            SubSceneManager.Instance.EditorCreateBuildingScenes(item.Buildings);
        });
        NewButton("LoadScenes", buttonWidth, true, ()=>
        {
            SubSceneManager.Instance.EditorLoadScenes(item.Buildings);
        });
        NewButton("UnLoadScenes", buttonWidth, true, () =>
        {
            SubSceneManager.Instance.EditorUnLoadScenes(item.Buildings);
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("SavePrefabs", buttonWidth, true, () =>
        {
            //BuildingModelManager.Instance.SavePrefabs(item.Buildings);
            BuildingModelInfoList.SavePrefabs(item.Buildings);
        });
        NewButton("LoadPrefabs", buttonWidth, true, () =>
        {
            //BuildingModelManager.Instance.LoadPrefabs(item.Buildings);
            BuildingModelInfoList.LoadPrefabs(item.Buildings);
        });
       
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("DeleteScenes", buttonWidth, true, () =>
        {
            //BuildingModelManager.Instance.LoadPrefabs(item.Buildings);
            SubSceneManager.Instance.EditorDeleteOtherRepleatScenes(item.Buildings);
        });
        NewButton("LoadLODs", buttonWidth, true, () =>
        {
            SubSceneManager.Instance.EditorLoadLODs(item.Buildings);
        });
        NewButton("CreateLODs", buttonWidth, true, () =>
        {
            SubSceneManager.Instance.EditorCreateLODs(item.Buildings);
        });
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //NewButton("4.SetBuildings", buttonWidth, true, item.SetBuildings);
        //NewButton("ClearBuildings", buttonWidth, true, item.ClearBuildings);
        //EditorGUILayout.EndHorizontal();

        EditorUIUtils.SetupStyles();
        //-------------------------------------------------------BuildingList-----------------------------------------------------------
        DrawModelList(buildingListArg, () =>
        {
            return item.Buildings.Where(b => b != null).ToList(); ;
        }, () =>
        {
            //EditorGUILayout.BeginHorizontal();
            //if (GUILayout.Button("ActiveAll"))
            //{
            //    //item.gameObject.SetActive();
            //    item.SetModelsActive(true);
            //}
            //if (GUILayout.Button("InactiveAll"))
            //{
            //    item.SetModelsActive(false);
            //}
            //EditorGUILayout.EndHorizontal();
        });
    }
}
