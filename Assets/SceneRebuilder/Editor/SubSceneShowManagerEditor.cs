using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SubSceneShowManager))]
public class SubSceneShowManagerEditor : BaseFoldoutEditor<SubSceneShowManager>
{
    private FoldoutEditorArg sceneOut0ListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneOut1ListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneInListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneLODsListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneLOD0ListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneGPUIListArg = new FoldoutEditorArg(true, false, true);

    private FoldoutEditorArg sceneOut0PartListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneOut0TreeListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneOut0HiddenNodeListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneOut0ShowNodeListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneHiddenNodeListArg = new FoldoutEditorArg(true, false, true);
    private FoldoutEditorArg sceneShowNodeListArg = new FoldoutEditorArg(true, false, true);

    private FoldoutEditorArg currentFloorListArg = new FoldoutEditorArg(true,true,true);

    public override void OnToolLayout(SubSceneShowManager showManager)
    {
        base.OnToolLayout(showManager);
        if (GUILayout.Button("InitCameras"))
        {
            showManager.InitCameras();
        }
        if (GUILayout.Button("InitScenes"))
        {
            showManager.InitScenes();
        }
        //if (GUILayout.Button("InitOnStart"))
        //{
        //    showManager.InitOnStart();
        //}
        DrawObjectList(sceneOut0ListArg, "SceneOut0", showManager.scenes_Out0, null, null, null);
        DrawObjectList(sceneOut1ListArg, "SceneOut1", showManager.scenes_Out1, null, null, null);
        DrawObjectList(sceneInListArg, "SceneIn", showManager.scenes_In, null, null, null);
        DrawObjectList(sceneLODsListArg, "SceneLODs", showManager.scenes_LODs, null, null, null);
        DrawObjectList(sceneLOD0ListArg, "SceneLOD0", showManager.scenes_LOD0, null, null, null);
        DrawObjectList(sceneGPUIListArg, "SceneGPUI", showManager.scenes_GPUI, null, null, null);

        DrawObjectList(sceneOut0PartListArg, "Scenes_Out0_Part", showManager.scenes_Out0_Part, null, null, null);
        DrawObjectList(sceneOut0TreeListArg, "scenes_Out0_Tree", showManager.scenes_Out0_Tree, null, null, null);
        DrawObjectList(sceneOut0HiddenNodeListArg, "Scenes_Out0_TreeNode_Hidden", showManager.scenes_Out0_TreeNode_Hidden, null, null, null);
        DrawObjectList(sceneOut0ShowNodeListArg, "Scenes_Out0_TreeNode_Shown", showManager.scenes_Out0_TreeNode_Shown, null, null, null);
        DrawObjectList(sceneHiddenNodeListArg, "Scenes_TreeNode_Hidden", showManager.scenes_TreeNode_Hidden, null, null, null);
        DrawObjectList(sceneShowNodeListArg, "Scenes_TreeNode_Shown", showManager.scenes_TreeNode_Shown, null, null, null);

        GUILayout.BeginHorizontal();
        GUILayout.Label("BoundsSize", GUILayout.Width(90));
        showManager.BoundsSize = EditorGUILayout.DelayedFloatField(showManager.BoundsSize, GUILayout.Width(70));
        if (GUILayout.Button("InitAllModels"))
        {
            showManager.InitAllModels();
        }
        if (GUILayout.Button("InitBuildingBounds"))
        {
            showManager.InitFloorBuildingBounds();
        }
        if (GUILayout.Button("LoadBuildingModels"))
        {
            showManager.LoadBuildingModels();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("BoundsSizeNew", GUILayout.Width(90));
        showManager.BoundsSizeNew = EditorGUILayout.DelayedFloatField(showManager.BoundsSizeNew, GUILayout.Width(70));
        if (GUILayout.Button("ChangeBoundsSize"))
        {
            showManager.ChangeBuildingBoundsSize();
        }
        if (GUILayout.Button("UpdateSelectionBounds"))
        {
            showManager.UpdateSelectionBuildingBounds();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("CalculateInWitchFloorBuildings"))
        {
            showManager.CalculateInWitchFloorBuildings();
        }
        showManager.IsCalculateBounds=GUILayout.Toggle(showManager.IsCalculateBounds, "CalculateFloorBounds");
        showManager.IsHideFloorIn = GUILayout.Toggle(showManager.IsHideFloorIn, "IsHideFloorIn");
        BuildingScenesLoadManager.Instance.IsEnableCulling = GUILayout.Toggle(BuildingScenesLoadManager.Instance.IsEnableCulling, "EnableCulling", GUILayout.Width(100));

        DrawObjectList(currentFloorListArg, "CurrentFloor", showManager.currentFloors.Items, null, null, null);
        
    }
}
