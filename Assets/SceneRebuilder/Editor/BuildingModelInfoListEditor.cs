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
