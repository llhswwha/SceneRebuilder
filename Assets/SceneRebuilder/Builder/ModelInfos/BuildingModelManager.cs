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
        Buildings.Clear();
        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
        InitBuildings(Buildings);
    }

    public void InitBuildings(List<BuildingModelInfo> buildings)
    {
        DateTime start = DateTime.Now;
        for (int i = 0; i < buildings.Count; i++)
        {
            float progress = (float)i / buildings.Count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("InitBuildings", $"{i}/{buildings.Count} {percents:F2}% of 100%", progress))
            {
                break;
            }
            buildings[i].InitInOut(true);
        }
        ProgressBarHelper.ClearProgressBar();

        GetCountInfo();

        SortByOut0();

        Debug.LogError($"InitBuildings Buildings:{buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
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
        Debug.Log($"CombinedBuildings count:{buildings.Count}");
        DateTime start = DateTime.Now;
        AreaTreeManager treeManager = GameObject.FindObjectOfType<AreaTreeManager>();
        if (treeManager != null) treeManager.Clear();
        List<ModelAreaTree> allTrees = new List<ModelAreaTree>();
        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingModelInfo b = buildings[i];
            if (b == null)
            {
                Debug.LogError($"CombinedBuildings b == null i={i}");
                continue;
            }

            var trees = b.CreateTreesInnerEx(IsOut0BigSmall,subProgress=>
            {
                float progress = (float)(i+ subProgress) / buildings.Count;

                //Debug.Log($"CombinedBuildings subProgress:{subProgress},progress:{progress}");

                float percents = progress * 100;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CombinedBuildings", $"Progress2 {(i + subProgress):F1}/{buildings.Count} {percents:F2}%  {b.name}", progress))
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


        if (treeManager)
        {
            treeManager.AddTrees(allTrees.ToArray());
            treeManager.GetTreeInfos();
        }

        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"CreateTrees Buildings:{buildings.Count},Trees:{allTrees.Count},Time:{(DateTime.Now - start).ToString()}");
    }


    [ContextMenu("* GetInfos")]
    public void GetInfos()
    {
        GetBuildings();

        SortByOut0();

        GetCountInfo();
    }

    private void GetBuildings()
    {
        Buildings.Clear();
        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
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
        GetBuildings();
        foreach (var b in Buildings)
        {
            b.ShowAll();
        }
    }

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        GetBuildings();
        foreach (var b in Buildings)
        {
            b.ShowRenderers();
        }
    }

    [ContextMenu("HideDetail")]
    public void HideDetail()
    {
        GetBuildings();
        foreach (var b in Buildings)
        {
            b.HideDetail();
        }
    }

    [ContextMenu("Unpack")]
    public void Unpack()
    {
        GetBuildings();
        foreach (var b in Buildings)
        {
            b.Unpack();
        }
    }

    [ContextMenu("GetTrees")]
    public void GetTrees()
    {
        GetBuildings();
        GetTrees(Buildings);
    }

    public void GetTrees(List<BuildingModelInfo> buildings)
    {
        foreach (var b in buildings)
        {
            if (b == null) continue;
            b.GetTrees();
        }
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        GetBuildings();
        ClearTrees(Buildings);
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees(List<BuildingModelInfo> buildings)
    {
        GetBuildings();
        foreach (var b in buildings)
        {
            if (b == null) continue;
            b.ClearTrees();
        }
    }

#if UNITY_EDITOR

    //[ContextMenu("OneKey")]
    //public void OneKey()
    //{
    //    GetBuildings();
    //    foreach (var b in Buildings)
    //    {
    //        if (b == null) continue;
    //        b.GetTrees();
    //    }


    //}

    [ContextMenu("EditorMoveScenes")]
    public void EditorMoveScenes()
    {
        GetBuildings();
        foreach (var b in Buildings)
        {
            b.EditorMoveScenes();
        }
    }

    [ContextMenu("CreatePrefabs")]
    public void CreatePrefabs()
    {

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

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{Buildings.Count} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }
        }


        if (treeManager) treeManager.AddTrees(allTrees.ToArray());

        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreatePrefabs Buildings:{Buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");

    }
#endif

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
            //b.EditorCreateScenes();

            // b.EditorCreateScenes_TreeWithPart((subProgress,si,c)=>
            // {
            //     Debug.Log($"CreateScenesInner subProgress:{subProgress} building:{b}");
            // });

            b.EditorCreateNodeScenes((subProgress)=>
            {
                Debug.Log($"CreateScenesInner subProgress:{subProgress} building:{b}");
            });

            scenes.Add(go.name);

            float progress = (float)i / buildings.Count;
            float percents = progress * 100;

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", $"{i}/{buildings.Count} {percents:F2}% of 100%", progress))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }

            break;
        }


        if (treeManager) treeManager.AddTrees(allTrees.ToArray());

        ProgressBarHelper.ClearProgressBar();
        Debug.LogWarning($"CreateScenesInner End Buildings:{Buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
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
