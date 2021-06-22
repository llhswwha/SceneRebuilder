using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public class BuildingModelManager : MonoBehaviour
{
    public int selectCount = 5;
    public List<BuildingModelInfo> SelectedBuildings = new List<BuildingModelInfo>();

    public bool IsOut0BigSmall = false;

    public List<BuildingModelInfo> Buildings = new List<BuildingModelInfo>();

    public List<float> BuildingsOut0Vertex = new List<float>();

    public List<BuildingModelInfo> Buildings10 = new List<BuildingModelInfo>();

    public List<float> BuildingsOut0Vertex10 = new List<float>();

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
        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
        for (int i = 0; i < Buildings.Count; i++)
        {
            float progress = (float)i / Buildings.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("InitBuildings", $"{i}/{Buildings.Count} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
            Buildings[i].InitInOut(true);
        }
        ProgressBarHelper.ClearProgressBar();

        GetCountInfo();

        SortByOut0();

        Debug.LogWarning($"InitBuildings Buildings:{Buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }


    [ContextMenu("* CombineAll")]
    public void CombineAll()
    {
        GetInfos();
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
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager != null) treeManager.Clear();
        List<ModelAreaTree> allTrees = new List<ModelAreaTree>();
        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingModelInfo b = buildings[i];
            if (b == null)
            {
                Debug.LogWarning($"CombinedBuildings b == null i={i}");
                continue;
            }

            var trees = b.CreateTreesInnerEx(IsOut0BigSmall);

            if (trees != null)
            {
                allTrees.AddRange(trees);
            }

            float progress = (float)i / buildings.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreateTrees", $"{i}/{buildings.Count} {percents}% of 100%", progress))
            {
                break;
            }
        }


        if (treeManager)
        {
            treeManager.AddTrees(allTrees.ToArray());
            treeManager.GetTreeInfos();
        }

        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreateTrees Buildings:{Buildings.Count},Trees:{allTrees.Count},Time:{(DateTime.Now - start).ToString()}");
    }


    [ContextMenu("* GetInfos")]
    public void GetInfos()
    {
        Buildings.Clear();

        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>(true));

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

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        foreach (var b in Buildings)
        {
            b.ShowRenderers();
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

    [ContextMenu("Unpack")]
    public void Unpack()
    {
        foreach (var b in Buildings)
        {
            b.Unpack();
        }
    }

    [ContextMenu("EditorMoveScenes")]
    public void EditorMoveScenes()
    {
        foreach (var b in Buildings)
        {
            b.EditorMoveScenes();
        }
    }

    [ContextMenu("CreatePrefabs")]
    public void CreatePrefabs()
    {
#if UNITY_EDITOR
        Unpack();
        DateTime start = DateTime.Now;
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager != null) treeManager.Clear();
        List<ModelAreaTree> allTrees = new List<ModelAreaTree>();
        for (int i = 0; i < Buildings.Count; i++)
        {
            BuildingModelInfo b = Buildings[i];
            if (b == null) continue;
            GameObject go = b.gameObject;
            //string path = "Assets/Models/Instances/Buildings/" + go.name +"_"+ go.GetInstanceID() + ".prefab";
            string path = "Assets/Models/Instances/Buildings/" + go.name + ".prefab";
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.UserAction);

            float progress = (float)i / Buildings.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{Buildings.Count} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
        }


        if (treeManager) treeManager.AddTrees(allTrees.ToArray());

        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreatePrefabs Buildings:{Buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
#endif
    }

    private string GetScenePath(string sceneName)
    {
        return Application.dataPath + "/Models/Instances/Buildings/" + sceneName + ".unity";
    }

    public List<string> scenes = new List<string>();

    public SceneContentType contentType;

    //[ContextMenu("CreateScenes")]
    public void CreateScenesInner(List<BuildingModelInfo> buildings)
    {
#if UNITY_EDITOR

        //Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        //var rootObjs = scene.GetRootGameObjects();
        //Debug.Log("rootObjs:"+ rootObjs.Length);

        DateTime start = DateTime.Now;
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager != null) treeManager.Clear();
        List<ModelAreaTree> allTrees = new List<ModelAreaTree>();
        scenes.Clear();
        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingModelInfo b = buildings[i];
            if (b == null) continue;
            GameObject go = b.gameObject;

            //var path = GetScenePath(go.name);
            //Scene scene = EditorHelper.CreateScene(path,true,go);
            b.contentType = this.contentType;
            b.EditorCreateScenes();

            scenes.Add(go.name);

            float progress = (float)i / buildings.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{buildings.Count} {percents}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
        }


        if (treeManager) treeManager.AddTrees(allTrees.ToArray());

        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreatePrefabs Buildings:{Buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
#endif
    }

    [ContextMenu("CreateScenes_All")]
    public void CreateScenes_All()
    {
        CreateScenesInner(Buildings);
    }

    [ContextMenu("CreateScenes_Selection")]
    public void CreateScenes_Selection()
    {
        CreateScenesInner(SelectedBuildings);
    }

    [ContextMenu("SortByOut0")]
    public void SortByOut0()
    {
        Buildings.Sort((a, b) =>
        {
            return b.Out0VertextCount.CompareTo(a.Out0VertextCount);
        });
        BuildingsOut0Vertex.Clear();
        Buildings10.Clear();
        BuildingsOut0Vertex10.Clear();

        for (int i = 0; i < Buildings.Count; i++)
        {
            BuildingModelInfo b = Buildings[i];
            BuildingsOut0Vertex.Add(b.Out0VertextCount);
            if (i < 10)
            {
                Buildings10.Add(b);
                BuildingsOut0Vertex10.Add(b.Out0VertextCount);
            }
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