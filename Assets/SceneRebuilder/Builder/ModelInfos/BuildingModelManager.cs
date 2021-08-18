using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public class BuildingModelManager : SingletonBehaviour<BuildingModelManager>
{
    // private static BuildingModelManager _instance;
    // public static BuildingModelManager Instance{
    //     get{
    //         if(_instance==null){
    //             _instance=GameObject.FindObjectOfType<BuildingModelManager>();
    //         }
    //         return _instance;
    //     }
    // }
    public int selectCount = 5;
    public List<BuildingModelInfo> SelectedBuildings = new List<BuildingModelInfo>();

    public bool IsOut0BigSmall = false;

    //public List<BuildingModelInfo> Buildings = new List<BuildingModelInfo>();

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
        UpdateBuildings();
        InitBuildings(Buildings);
    }

    public void OneKey_Save()
    {
        DateTime start = DateTime.Now;
        UpdateBuildings();
        for (int i = 0; i < Buildings.Count; i++)
        {
            BuildingModelInfo building = Buildings[i];
            building.OneKey_TreeNodeScene();
        }
        Debug.LogError($"OneKey_Save Buildings:{Buildings.Count} Time:{(DateTime.Now - start).ToString()}");
    }

    public void OneKey_Reset()
    {
        DateTime start = DateTime.Now;
        UpdateBuildings();
        for (int i = 0; i < Buildings.Count; i++)
        {
            BuildingModelInfo building = Buildings[i];
            building.ResetModel();
        }
        Debug.LogError($"OneKey_Reset Buildings:{Buildings.Count} Time:{(DateTime.Now - start).ToString()}");
    }

    public void OneKey_Resave()
    {
        DateTime start = DateTime.Now;
        UpdateBuildings();
        for (int i = 0; i < Buildings.Count; i++)
        {
            BuildingModelInfo building = Buildings[i];
            building.ResaveScenes();
        }
        Debug.LogError($"OneKey_Resave Buildings:{Buildings.Count} Time:{(DateTime.Now - start).ToString()}");
    }

    public void InitBuildings(List<BuildingModelInfo> buildings)
    {
        BuildingModelInfoList.InitBuildings(buildings);

        GetCountInfo();

        SortByOut0();

        //Debug.LogError($"InitBuildings Buildings:{buildings.Count},Time:{(DateTime.Now - start).TotalMilliseconds}ms");
    }


    [ContextMenu("* CombineAll")]
    public void CombineAll()
    {
        GetInfos();
        CombineBuildings(Buildings);
    }

    [ContextMenu("* CombineSelection")]
    public void CombineSelection()
    {
        CombineBuildings(SelectedBuildings);
    }

    public void CombineBuildings(List<BuildingModelInfo> buildings)
    {
        BuildingModelInfoList.CombinedBuildings(buildings,IsOut0BigSmall);
    }

    //public void SavePrefabs(BuildingModelInfo[] buildings)
    //{
    //    BuildingModelInfoList.SavePrefabs(buildings);
    //}

    //public void LoadPrefabs(BuildingModelInfo[] buildings)
    //{
    //    BuildingModelInfoList.LoadPrefabs(buildings);
    //}

    internal void SetModelsActive(bool v)
    {
        foreach(var b in Buildings)
        {
            b.gameObject.SetActive(v);
        }

        if (v)
        {
            var bList = GameObject.FindObjectsOfType<BuildingModelInfoList>(true);
            foreach (var l in bList)
            {
                l.gameObject.SetActive(v);
            }
        }
    }

    [ContextMenu("* GetInfos")]
    public void GetInfos()
    {
        UpdateBuildings();

        SortByOut0();

        GetCountInfo();
    }

    public void UpdateBuildings()
    {
        Buildings.Clear();
        Buildings.AddRange(GameObject.FindObjectsOfType<BuildingModelInfo>(true));
        //Debug.Log("");
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
        UpdateBuildings();
        foreach (var b in Buildings)
        {
            b.ShowAll();
        }
    }

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        UpdateBuildings();
        foreach (var b in Buildings)
        {
            b.ShowRenderers();
        }
    }

    [ContextMenu("HideDetail")]
    public void HideDetail()
    {
        UpdateBuildings();
        foreach (var b in Buildings)
        {
            b.HideDetail();
        }
    }

    [ContextMenu("ShowDetail")]
    public void ShowDetail()
    {
        UpdateBuildings();
        foreach (var b in Buildings)
        {
            b.ShowDetail();
        }
    }

    [ContextMenu("Unpack")]
    public void Unpack()
    {
        UpdateBuildings();
        foreach (var b in Buildings)
        {
            b.Unpack();
        }
    }

    [ContextMenu("UpdateTrees")]
    public void UpdateTrees()
    {
        UpdateBuildings();
        UpdateTrees(Buildings);
    }

    public List<ModelAreaTree> GetTrees()
    {
        List<ModelAreaTree> trees = new List<ModelAreaTree>();

        foreach (var b in Buildings)
        {
            if (b == null) continue;
            //trees.AddRange(b.trees);
            foreach(var t in b.trees)
            {
                if (t != null)
                {
                    trees.Add(t);
                }
            }
        }

        return trees;
    }

    public void UpdateTrees(List<BuildingModelInfo> buildings)
    {
        foreach (var b in buildings)
        {
            if (b == null) continue;
            b.UpdateTrees();
        }
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees()
    {
        UpdateBuildings();
        ClearTrees(Buildings);
    }

    [ContextMenu("ClearTrees")]
    public void ClearTrees(List<BuildingModelInfo> buildings)
    {
        UpdateBuildings();
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
        UpdateBuildings();
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
        
        var treeManager = AreaTreeManager.Instance;
        treeManager.Clear();

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

        treeManager.AddTrees(allTrees.ToArray());

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
        Debug.Log($"CreateScenesInner building:{buildings.Count}");

        DateTime start = DateTime.Now;
        var treeManager = AreaTreeManager.Instance;
        treeManager.Clear();

        List<ModelAreaTree> allTrees = new List<ModelAreaTree>();
        scenes.Clear();


        for (int i = 0; i < buildings.Count; i++)
        {
            BuildingModelInfo b = buildings[i];
            //Debug.Log($"CreateScenesInner [{i}] building:{b}");
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

                       
            //float progress = (float)i / buildings.Count;
            //float percents = progress * 100;

            var p1 = new ProgressArg(i, buildings.Count, b);

            if (ProgressBarHelper.DisplayCancelableProgressBar("CreatePrefabs", p1))
            {
                //ProgressBarHelper.ClearProgressBar();
                break;
            }

            b.EditorCreateNodeScenes((subProgress)=>
            {
                p1.AddSubProgress(subProgress);
                //Debug.Log($"CreateScenesInner subProgress:{subProgress} building:{b}");
                //float progress2 = (float)(i+subProgress) / buildings.Count;
                //float percents2 = progress2 * 100;
                ProgressBarHelper.DisplayCancelableProgressBar("BuildingModelInfo.EditorCreateNodeScenes", p1);
            });

            scenes.Add(go.name);



            //break;
        }

        treeManager.AddTrees(allTrees.ToArray());

        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"CreateScenesInner End Buildings:{Buildings.Count},Time:{(DateTime.Now - start)}");
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
