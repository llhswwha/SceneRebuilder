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
        NewButton("1.InitModels", buttonWidth, true, ()=>
        {
            BuildingModelManager.Instance.InitBuildings(item.Buildings.ToList());
        });
        // NewButton("ListWindow:" + count, buttonWidth, true, () =>
        //      {
        //          SceneRebuildEditorWindow.ShowWindow();
        //      });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateTrees", buttonWidth, true, ()=>
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
