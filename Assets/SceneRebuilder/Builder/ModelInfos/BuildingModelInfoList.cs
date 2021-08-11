using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelInfoList : MonoBehaviour
{
    public int AllRendererCount = 0;
    public float AllVertextCount = 0;
    public float ShowVertextCount = 0;
    public BuildingModelInfo[] Buildings;

    [ContextMenu("UpdateBuildings")]
    public void UpdateBuildings()
    {
        Buildings = gameObject.GetComponentsInChildren<BuildingModelInfo>(true);
        AllRendererCount = 0;
        AllVertextCount = 0;
        ShowVertextCount = 0;
        foreach (var b in Buildings)
        {
            AllRendererCount += b.AllRendererCount;
            AllVertextCount += b.AllVertextCount;

            ShowVertextCount += b.Out0BigVertextCount;
        }
    }

    public static void InitBuildings(List<BuildingModelInfo> buildings)
    {
        //DateTime start = DateTime.Now;
        //for (int i = 0; i < buildings.Count; i++)
        //{
        //    float progress = (float)i / buildings.Count;
        //    float percents = progress * 100;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar("InitBuildings", $"{i}/{buildings.Count} {percents:F2}% of 100%", progress))
        //    {
        //        break;
        //    }
        //    buildings[i].InitInOut(true);
        //}
        //ProgressBarHelper.ClearProgressBar();
        //Debug.LogError($"InitBuildings Buildings:{buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
        buildings.ForEachEx("InitBuildings", i => i.InitInOut(true));
    }

    public static void SavePrefabs(List<BuildingModelInfo> buildings)
    {
        buildings.ForEachEx("SavePrefabs", i => i.EditorLoadPrefab());
    }

    public static void LoadPrefabs(List<BuildingModelInfo> buildings)
    {
        buildings.ForEachEx("LoadPrefabs", i => i.EditorLoadPrefab());
    }
    

    public static void CombinedBuildings(List<BuildingModelInfo> buildings,bool isOut0BigSmall)
    {
        Debug.Log($"CombinedBuildings count:{buildings.Count}");
        DateTime start = DateTime.Now;
        var treeManager = AreaTreeManager.Instance;
        treeManager.Clear();
        List<ModelAreaTree> allTrees = new List<ModelAreaTree>();
        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingModelInfo b = buildings[i];
            if (b == null)
            {
                Debug.LogError($"CombinedBuildings b == null i={i}");
                continue;
            }

            var trees = b.CreateTreesInnerEx(isOut0BigSmall, subProgress =>
            {
                float progress2 = (float)(i + subProgress) / buildings.Count;

                //Debug.Log($"CombinedBuildings subProgress:{subProgress},progress:{progress}");

                float percents2 = progress2 * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CombinedBuildings", $"Progress2 {(i + subProgress):F1}/{buildings.Count} {percents2:F2}%  {b.name}", progress2))
                {
                    return;
                }
            });

            if (trees != null)
            {
                allTrees.AddRange(trees);
            }

            float progress = (float)i / buildings.Count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("CombinedBuildings", $"Progress1 {i}/{buildings.Count} {percents:F2}%  {b.name}", progress))
            {
                break;
            }
        }

        treeManager.AddTrees(allTrees.ToArray());
        treeManager.GetTreeInfos();

        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"CreateTrees Buildings:{buildings.Count},Trees:{allTrees.Count},Time:{(DateTime.Now - start).ToString()}");
    }
}
