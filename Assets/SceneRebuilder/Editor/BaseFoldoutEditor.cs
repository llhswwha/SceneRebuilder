using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BaseFoldoutEditor<T> : BaseEditor<T> where T : class
{
    public Dictionary<System.Object, FoldoutEditorArg> editorArgs = new Dictionary<System.Object, FoldoutEditorArg>();

    public void InitEditorArg<T2>(List<T2> items)/* where T2 : System.Object*/
    {
        foreach (var item in items)
        {
            if (!editorArgs.ContainsKey(item))
            {
                editorArgs.Add(item, new FoldoutEditorArg());
            }
            else
            {
                if (editorArgs[item] == null)
                {
                    editorArgs[item] = new FoldoutEditorArg();
                }
            }
        }
    }

    public void RemoveEditorArg<T2>(List<T2> items)/* where T2 : System.Object*/
    {
        foreach (var item in items)
        {
            if (editorArgs.ContainsKey(item))
            {
                editorArgs.Remove(item);
            }
        }
    }

    public virtual void UpdateList()
    {
        //manager.UpdateList();
        //InitMeshList(meshListArg);
    }

    protected void DrawModelItemToolbar(BuildingModelInfo b)
    {
        int width = 25;
        var contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.margin = new RectOffset(0, 0, 0, 0);
        contentStyle.padding = new RectOffset(0, 0, 0, 0);
        var state = b.GetState();
        if (state.CanOneKey())
        {
            if (GUILayout.Button("I", contentStyle, GUILayout.Width(20)))
            {
                b.InitInOut();
            }
        }
        else
        {
            if (GUILayout.Button("-", contentStyle, GUILayout.Width(20)))
            {

            }
        }
        if (state.CanOneKey())
        {
            if (GUILayout.Button("+O", contentStyle, GUILayout.Width(width)))
            {
                b.CreateTreesBSEx();
                UpdateList();
            }
        }
        else if (state.CanReset())
        {
            if (GUILayout.Button("-O", contentStyle, GUILayout.Width(width)))
            {
                b.EditorLoadNodeScenesEx();
                b.DestroyScenes();
                b.ClearTrees();
                UpdateList();
            }
        }
        else
        {
            if (GUILayout.Button("--", contentStyle, GUILayout.Width(width)))
            {

            }
        }

        if (state.CanCreateTrees)
        {
            if (GUILayout.Button("+T", contentStyle, GUILayout.Width(width)))
            {
                b.CreateTreesBSEx();
                UpdateList();
            }
        }
        else if (state.CanRemoveTrees)
        {
            if (GUILayout.Button("-T", contentStyle, GUILayout.Width(width)))
            {
                b.ClearTrees();
                UpdateList();
            }
        }
        else
        {
            if (GUILayout.Button("--", contentStyle, GUILayout.Width(width)))
            {

            }
        }

        if (state.CanCreateScenes)
        {
            if (GUILayout.Button("+S", contentStyle, GUILayout.Width(width)))
            {
                b.EditorCreateNodeScenes();
                UpdateList();
            }
        }
        else if (state.CanRemoveScenes)
        {
            if (GUILayout.Button("-S", contentStyle, GUILayout.Width(width)))
            {
                b.DestroyScenes();
                UpdateList();
            }
        }
        else
        {
            if (GUILayout.Button("--", contentStyle, GUILayout.Width(width)))
            {

            }
        }

        if (state.CanLoadScenes)
        {
            if (GUILayout.Button("+L", contentStyle, GUILayout.Width(width)))
            {
                b.EditorLoadNodeScenesEx();
                UpdateList();
            }
        }
        else if (state.CanUnloadScenes)
        {
            if (GUILayout.Button("-L", contentStyle, GUILayout.Width(width)))
            {
                b.UnLoadScenes();
                UpdateList();
            }
        }
        else
        {
            if (GUILayout.Button("--", contentStyle, GUILayout.Width(width)))
            {

            }
        }
    }

    public void DrawModelList(FoldoutEditorArg foldoutArg, System.Func<List<BuildingModelInfo>> funcGetList,System.Action listToolbarEvent)
    {
        List<BuildingModelInfo> buildings = new List<BuildingModelInfo>();
        foldoutArg.caption = $"Building List";
        EditorUIUtils.ToggleFoldout(foldoutArg,
        (arg) => {
            System.DateTime start = System.DateTime.Now;
            float sumVertexCount = 0;
            int sumRendererCount = 0;
            float sumVertexCount_Shown = 0;
            float sumVertexCount_Hidden = 0;
            //var ts= GameObject.FindObjectsOfType<Transform>(true);
            //Debug.Log($"Init BuildingList00 count:{ts.Length} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");

            //var bs = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
            //Debug.Log($"Init BuildingList0 count:{bs.Length} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");

            //buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList() ;

            //buildings = item.GetBuildings().Where(b => b != null).ToList();
            buildings = funcGetList();

            //Debug.Log($"Init BuildingList1 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            buildings.Sort((a, b) =>
            {
                //return b.AllVertextCount.CompareTo(a.AllVertextCount);
                return b.Out0BigVertextCount.CompareTo(a.Out0BigVertextCount);
            });
            //Debug.Log($"Init BuildingList2 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            buildings.ForEach(b => {
                sumVertexCount += b.AllVertextCount;
                sumRendererCount += b.AllRendererCount;

                sumVertexCount_Shown += b.Out0BigVertextCount;
                sumVertexCount_Hidden += b.Out0SmallVertextCount + b.InVertextCount + b.Out1VertextCount;
            });
            //Debug.Log($"Init BuildingList3 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            InitEditorArg(buildings);
            //Debug.Log($"Init BuildingList4 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            arg.caption = $"Building List({buildings.Count})";
            arg.info = $"[{sumVertexCount:F0}w={sumVertexCount_Shown:F0}+{sumVertexCount_Hidden:F0}+({(sumVertexCount - sumVertexCount_Shown - sumVertexCount_Hidden):F0})][{sumRendererCount / 10000f:F0}w]";
            Debug.Log($"Init BuildingList count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        },
        //"Window",
        () => {
            //SceneRebuildEditorWindow.ShowWindow();

            if (GUILayout.Button("Win", GUILayout.Width(35)))
            {
                SceneRebuildEditorWindow.ShowWindow();
            }
        });

        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
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
            if (listToolbarEvent != null) listToolbarEvent();

            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(buildings.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < buildings.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var b = buildings[i];
                var arg = editorArgs[b];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded,
                $"[{i + 1:00}] {b.name} {b.GetInfoText()}",
                //$"[{i + 1:00}] {b.name} ",
                $"[{b.Out0BigVertextCount:F0}+{b.Out0SmallVertextCount:F0}+{b.Out1VertextCount:F0}+{b.InVertextCount:F0}={b.AllVertextCount:F0}][{b.AllRendererCount}]",
                false, false, true, b.gameObject,
                () =>
                {
                    DrawModelItemToolbar(b);
                }
                );
                if (arg.isExpanded)
                {
                    BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time = System.DateTime.Now - start;
            Debug.Log($"Show BuildingList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    public void DrawTreeList(FoldoutEditorArg foldoutArg, System.Func<List<ModelAreaTree>> funcGetList)
    {
        List<ModelAreaTree> trees = new List<ModelAreaTree>();
        foldoutArg.caption = $"Tree List";
        EditorUIUtils.ToggleFoldout(foldoutArg,
        (arg) => {
            System.DateTime start = System.DateTime.Now;
            float sumVertexCount = 0;
            int sumRendererCount = 0;

            //trees=GameObject.FindObjectsOfType<ModelAreaTree>(true).Where(t=>t.VertexCount>0).ToList() ;
            //trees = item.GetTrees().Where(t => t.VertexCount > 0).ToList();
            trees = funcGetList();
            //Debug.Log($"Init TreeList1 count:{trees.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            trees.Sort((a, b) =>
            {
                if (b == null) return -1;
                if (a == null) return 1;
                return b.VertexCount.CompareTo(a.VertexCount);
            });
            //Debug.Log($"Init TreeList2 count:{trees.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");

            trees.ForEach(b => {
                if (b != null)
                {
                    sumVertexCount += b.VertexCount;
                    sumRendererCount += b.GetRendererCount();
                }
            });
            //Debug.Log($"Init TreeList3 count:{trees.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            InitEditorArg(trees);

            arg.caption = $"Tree List({trees.Count})";
            arg.info = $"[{sumVertexCount:F0}w][{sumRendererCount / 10000f:F0}w]";
            Debug.Log($"Init TreeList count:{trees.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        },
        () => {
            if (GUILayout.Button("------", GUILayout.Width(60)))
            {
                //TreeNodeManagerEditorWindow.ShowWindow();
            }
        });
        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(trees.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < trees.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var tree = trees[i];
                // if(tree==null)continue;
                var arg = editorArgs[tree];
                if (arg == null)
                {
                    editorArgs[tree] = new FoldoutEditorArg();
                }
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i + 1:00}] {tree.name}", $"[{tree.VertexCount}w][{tree.GetRendererCount()}][{tree.TreeLeafs.Count}]", false, false, false, tree.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time = System.DateTime.Now - start;
            Debug.Log($"Show TreeList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    public List<AreaTreeNode> DrawNodeList(FoldoutEditorArg foldoutArg, System.Func<List<AreaTreeNode>> funcGetList)
    {
        List<AreaTreeNode> nodes = new List<AreaTreeNode>();
        foldoutArg.caption = $"Node List";
        EditorUIUtils.ToggleFoldout(foldoutArg,
        (arg) => {
            System.DateTime start = System.DateTime.Now;
            float sumVertexCount = 0;
            int sumRendererCount = 0;
            //nodes = item.GetLeafNodes();
            nodes = funcGetList();
            nodes.Sort((a, b) =>
            {
                return b.VertexCount.CompareTo(a.VertexCount);
            });
            nodes.ForEach(b => {
                sumVertexCount += b.VertexCount;
                sumRendererCount += b.Renderers.Count;
            });
            InitEditorArg(nodes);
            arg.caption = $"Node List({nodes.Count})";
            arg.info = $"[{sumVertexCount:F0}w][{sumRendererCount / 10000f:F0}w]";
            var time = System.DateTime.Now - start;
            Debug.Log($"Init NodeList count:{nodes.Count} time:{time.TotalMilliseconds:F1}ms ");
        },
        () => {
            if (GUILayout.Button("Win", GUILayout.Width(35)))
            {
                TreeNodeManagerEditorWindow.ShowWindow();
            }
        });
        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(nodes.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < nodes.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var node = nodes[i];
                var arg = editorArgs[node];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i + 1:00}] {node.tree.name}.{node.name}", $"[{node.VertexCount:F0}w][{node.Renderers.Count}]", false, false, false, node.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time = System.DateTime.Now - start;
            Debug.Log($"Show NodeList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
        return nodes;
    }

    public void DrawSceneList(FoldoutEditorArg foldoutArg,System.Func<List<SubScene_Base>> funcGetList)
    {
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        foldoutArg.caption = $"Scenes";
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) => {
            System.DateTime start = System.DateTime.Now;
            //scenes = item.scenes.ToList();
            scenes = funcGetList();
            if (foldoutArg.listFilterId == 1)
            {
                scenes = scenes.Where(i => { return i.name.Contains("Combined"); }).ToList();
            }
            else if (foldoutArg.listFilterId == 2)
            {
                scenes = scenes.Where(i => { return i.name.Contains("Renderers"); }).ToList();
            }
            else
            {

            }
            float sumVertexCount = 0;
            int sumRendererCount = 0;
            scenes.Sort((a, b) =>
            {
                return b.vertexCount.CompareTo(a.vertexCount);
            });
            scenes.ForEach(b =>
            {
                sumVertexCount += b.vertexCount;
                sumRendererCount += b.rendererCount;
            });
            InitEditorArg(scenes);
            arg.caption = $"Scenes({scenes.Count})";
            arg.info = $"[{sumVertexCount:F0}w][{sumRendererCount / 10000f:F0}w]";
            var time = System.DateTime.Now - start;
            Debug.Log($"Init SceneList count:{scenes.Count} time:{time.TotalMilliseconds:F1}ms ");
        }, () => {
            int filterType = foldoutArg.DrawFilterList(100,"All","Combined","Renderers");
            if (GUILayout.Button("Win", GUILayout.Width(35)))
            {
                SubSceneManagerEditorWindow.ShowWindow();
            }
            
        });
        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(scenes.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < scenes.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var scene = scenes[i];
                var arg = editorArgs[scene];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i + 1:00}] {scene.name}", $"[{scene.vertexCount:F0}w][{scene.rendererCount}]", false, false, false, scene.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time = System.DateTime.Now - start;
            Debug.Log($"Show SceneList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    private List<MeshFilter> InitMeshList(FoldoutEditorArg<MeshFilter> arg, System.Func<List<MeshFilter>> funcGetList)
    {
        System.DateTime start = System.DateTime.Now;
        float sumVertexCount = 0;
        float sumVertexCountVisible = 0;
        int sumRendererCount = 0;
        int sumRendererCountVisible = 0;
        //List<MeshFilter> meshes = GameObject.FindObjectsOfType<MeshFilter>(true).Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
        List<MeshFilter> meshes = funcGetList();
        meshes.Sort((a, b) =>
        {
            return b.sharedMesh.vertexCount.CompareTo(a.sharedMesh.vertexCount);
        });
        Debug.Log($"Init MeshList1 count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        meshes.ForEach(b => {
            if (b == null) return;
            if (b.gameObject.activeInHierarchy == true && b.GetComponent<MeshRenderer>() != null && b.GetComponent<MeshRenderer>().enabled == true)
            {
                sumVertexCountVisible += b.sharedMesh.vertexCount;
                sumRendererCountVisible++;
            }
            sumVertexCount += b.sharedMesh.vertexCount;
            sumRendererCount++;
        });
        Debug.Log($"Init MeshList2 count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        sumVertexCount /= 10000;
        sumVertexCountVisible /= 10000;
        //InitEditorArg(scenes);
        arg.caption = $"Mesh List({meshes.Count})";
        arg.info = $"[{sumVertexCountVisible:F0}/{sumVertexCount:F0}w][{sumRendererCountVisible}/{sumRendererCount}]";
        Debug.LogWarning($"Init MeshList count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        arg.Items = meshes;
        return meshes;
    }
    public void DrawMeshList(FoldoutEditorArg<MeshFilter> foldoutArg, System.Func<List<MeshFilter>> funcGetList)
    {
        if (string.IsNullOrEmpty(foldoutArg.caption)) foldoutArg.caption = $"Mesh List";
        List<MeshFilter> meshFilters = foldoutArg.Items;
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) => {

            if (meshFilters.Count == 0)
            {
                meshFilters = InitMeshList(arg as FoldoutEditorArg<MeshFilter>, funcGetList);
            }
            //else
            //{
            //    meshFilters = foldoutArg.Items;
            //}

        }, () => {
            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                foldoutArg.Items.Clear();
                meshFilters = InitMeshList(foldoutArg, funcGetList);
            }
            if (GUILayout.Button("Win", GUILayout.Width(35)))
            {
                //SubSceneManagerEditorWindow.ShowWindow();
                MeshProfilerNS.MeshProfiler.ShowWindow();
            }
        });

        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(meshFilters.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < meshFilters.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var mesh = meshFilters[i];
                if (!editorArgs.ContainsKey(mesh))
                {
                    editorArgs.Add(mesh, new FoldoutEditorArg());
                }
                var arg = editorArgs[mesh];
                BuildingModelInfo[] bs = mesh.GetComponentsInParent<BuildingModelInfo>(true);
                if (bs.Length == 0)
                {
                    Debug.LogError($"Show MeshList buildings.Length==0"); continue;
                }
                BuildingModelInfo building = bs[0];
                if (mesh == null)
                {
                    Debug.LogError($"Show MeshList mesh==null"); continue;
                }
                if (mesh.sharedMesh == null)
                {
                    Debug.LogError($"Show MeshList mesh.sharedMesh==null"); continue;
                }
                if (arg == null)
                {
                    Debug.LogError($"Show MeshList arg==null"); continue;
                }
                if (building == null)
                {
                    Debug.LogError($"Show MeshList building==null"); continue;
                }
                if (mesh.transform.parent == null)
                {
                    Debug.LogError($"Show MeshList mesh.transform.parent==null"); continue;
                }
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i + 1:00}] {building.name}>>{mesh.transform.parent.name}>{mesh.name}", $"[{mesh.sharedMesh.vertexCount / 10000f:F0}w]", false, false, false, mesh.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time = System.DateTime.Now - start;
            Debug.Log($"Show MeshList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }
}
