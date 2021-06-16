using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelManager : MonoBehaviour
{
    public int selectCount = 5;
    public List<BuildingModelInfo> SelectedBuildings = new List<BuildingModelInfo>();

    public List<BuildingModelInfo> Buildings = new List<BuildingModelInfo>();

    public List<float> BuildingsOut0Vertex = new List<float>();

    public float InVertextCount = 0;
    public float Out0VertextCount = 0;
    public float Out1VertextCount = 0;

    public int InRendererCount = 0;
    public int Out0RendererCount = 0;
    public int Out1RendererCount = 0;

    public int AllRendererCount = 0;
    public float AllVertextCount = 0;

    private void ClearCount()
    {
        InVertextCount = 0;
        Out0VertextCount = 0;
        Out1VertextCount = 0;

        InRendererCount = 0;
        Out0RendererCount = 0;
        Out1RendererCount = 0;

        AllRendererCount = 0;
        AllVertextCount = 0;
    }

    [ContextMenu("* InitBuildings")]
    public void InitBuildings()
    {
        DateTime start = DateTime.Now;
        Buildings.Clear();
        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>());
        for (int i = 0; i < Buildings.Count; i++)
        {
            float progress = (float)i / Buildings.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitBuildings", $"{i}/{Buildings.Count} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            Buildings[i].InitInOut();
        }
        ProgressBarHelper.ClearProgressBar();

        GetCountInfo();

        SortByOut0();

        Debug.LogWarning($"InitBuildings Buildings:{Buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }


    [ContextMenu("* CombineAll")]
    public void CombineAll()
    {
        CombinedBuildings(Buildings);
    }

    [ContextMenu("* CombineSelection")]
    public void CombineSelection()
    {
        CombinedBuildings(SelectedBuildings);
    }

    public void CombinedBuildings(List<BuildingModelInfo> buildings)
    {
        DateTime start = DateTime.Now;
        List<ModelAreaTree> allTrees = new List<ModelAreaTree>();
        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingModelInfo b = buildings[i];
            if (b == null) continue;
            var trees = b.CreateTreesInner();
            allTrees.AddRange(trees);

            float progress = (float)i / buildings.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreateTrees", $"{i}/{buildings.Count} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
        }

        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager)
            treeManager.AddTrees(allTrees.ToArray());

        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreateTrees Buildings:{Buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }


    [ContextMenu("* GetInfos")]
    public void GetInfos()
    {
        Buildings.Clear();

        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>());

        SortByOut0();

        GetCountInfo();
    }

    private void GetCountInfo()
    {
        ClearCount();
        foreach (var b in Buildings)
        {
            InVertextCount += b.InVertextCount;
            Out0VertextCount += b.Out0VertextCount;
            Out1VertextCount += b.Out1VertextCount;

            InRendererCount += b.InRendererCount;
            Out0RendererCount += b.Out0RendererCount;
            Out1RendererCount += b.Out1RendererCount;

            AllRendererCount += b.AllRendererCount;
            AllVertextCount += b.AllVertextCount;
        }
    }

    [ContextMenu("SelectFirst")]
    public void SelectFirst()
    {
        SelectFirstN(selectCount);
    }

    [ContextMenu("SelectLast")]
    public void SelectLast()
    {
        SelectLastN(selectCount);
    }

    public void SelectFirstN(int n)
    {
        SelectedBuildings.Clear();
        for (int i = 0; i < Buildings.Count && i < n; i++)
        {
            BuildingModelInfo b = Buildings[i];
            SelectedBuildings.Add(b);
        }
    }

    public void SelectLastN(int n)
    {
        SelectedBuildings.Clear();
        for (int i = 0; i < Buildings.Count && i < n; i++)
        {
            BuildingModelInfo b = Buildings[Buildings.Count - 1 - i];
            SelectedBuildings.Add(b);
        }
    }

   

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        foreach (var b in Buildings)
        {
            b.ShowAll();
        }
    }

    [ContextMenu("HideDetail")]
    public void HideDetail()
    {
        foreach (var b in Buildings)
        {
            b.HideDetail();
        }
    }

    [ContextMenu("CreatePrefabs")]
    public void CreatePrefabs()
    {

    }

    [ContextMenu("SortByOut0")]
    public void SortByOut0()
    {
        Buildings.Sort((a, b) =>
        {
            return b.Out0VertextCount.CompareTo(a.Out0VertextCount);
        });
        BuildingsOut0Vertex.Clear();
        foreach(var b in Buildings)
        {
            BuildingsOut0Vertex.Add(b.Out0VertextCount);
        }
    }

    [ContextMenu("SortByOut1")]
    public void SortByOut1()
    {
        Buildings.Sort((a, b) =>
        {
            return b.Out1VertextCount.CompareTo(a.Out1VertextCount);
        });
    }

    [ContextMenu("SortByIn")]
    public void SortByIn()
    {
        Buildings.Sort((a, b) =>
        {
            return b.InVertextCount.CompareTo(a.InVertextCount);
        });
    }
}
