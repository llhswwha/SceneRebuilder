using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BaseFoldoutEditor<T> : BaseEditor<T> where T : class
{
    public static Dictionary<System.Object, FoldoutEditorArg> editorArgs
    {
        get
        {
            return FoldoutEditorArgBuffer.editorArgs;
        }
    }

    public static void InitEditorArg<T2>(List<T2> items)/* where T2 : System.Object*/
    {
        FoldoutEditorArgBuffer.InitEditorArg<T2>(items);
    }

    public static FoldoutEditorArg GetEditorArg<T2>(T2 item, FoldoutEditorArg newArg)/* where T2 : System.Object*/
    {
        return FoldoutEditorArgBuffer.GetEditorArg<T2>(item, newArg);
    }

    //public static Dictionary<System.Object, FoldoutEditorArg> editorArgs_global = new Dictionary<System.Object, FoldoutEditorArg>();

    public static FoldoutEditorArg GetGlobalEditorArg<T2>(T2 item, FoldoutEditorArg newArg)/* where T2 : System.Object*/
    {
        //if (newArg == null)
        //{
        //    newArg = new FoldoutEditorArg();
        //}
        //if (!editorArgs_global.ContainsKey(item))
        //{
        //    editorArgs_global.Add(item, newArg);
        //}
        //return editorArgs_global[item];

        return FoldoutEditorArgBuffer.GetGlobalEditorArg<T2>(item, newArg);
    }

    public static void RemoveEditorArg<T2>(List<T2> items)/* where T2 : System.Object*/
    {
        //foreach (var item in items)
        //{
        //    if (item == null)
        //    {

        //        continue;
        //    }
        //    if (editorArgs.ContainsKey(item))
        //    {
        //        editorArgs.Remove(item);
        //    }
        //}

        FoldoutEditorArgBuffer.RemoveEditorArg<T2>(items);
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

    public void DrawModelList(FoldoutEditorArg foldoutArg, System.Func<List<BuildingModelInfo>> funcGetList, System.Action listToolbarEvent)
    {
        List<BuildingModelInfo> buildings = new List<BuildingModelInfo>();
        foldoutArg.caption = $"Building List";
        EditorUIUtils.ToggleFoldout(foldoutArg,
        (arg) =>
        {
            System.DateTime start = System.DateTime.Now;
            float sumVertexCount = 0;
            int sumRendererCount = 0;
            float sumVertexCount_Shown = 0;
            float sumVertexCount_Hidden = 0;
            buildings = funcGetList();

            //Debug.Log($"Init BuildingList1 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            //buildings.Sort((a, b) =>
            //{
            //    //return b.AllVertextCount.CompareTo(a.AllVertextCount);
            //    return b.Out0BigVertextCount.CompareTo(a.Out0BigVertextCount);
            //});
            //Debug.Log($"Init BuildingList2 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            buildings.ForEach(b =>
            {
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
            //Debug.Log($"Init BuildingList count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        },
        //"Window",
        () =>
        {
            //SceneRebuildEditorWindow.ShowWindow();
            if (GUILayout.Button("Win", GUILayout.Width(35)))
            {
                SceneRebuildEditorWindow.ShowWindow();
            }
        });

        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            if (listToolbarEvent != null) listToolbarEvent();

            System.DateTime start = System.DateTime.Now;
            //foldoutArg.DrawPageToolbar(buildings.Count);
            int id = foldoutArg.DrawPageToolbarWithSort(buildings.Count, 100, 1, "All", "Out0B", "Out0S", "Out1", "In");

            //int id = foldoutArg.DrawSortTypeList(100, 1, "All", "Out0B", "Out0S", "Out1", "In");
            //if (id != -1)
            //{
            //    Debug.LogError($"sortType:{id}");
            //}

            if (id == 0)
            {
                buildings.Sort((a, b) =>
                {
                    return b.AllVertextCount.CompareTo(a.AllVertextCount);
                });
            }
            else if (id == 1)
            {
                buildings.Sort((a, b) =>
                {
                    return b.Out0BigVertextCount.CompareTo(a.Out0BigVertextCount);
                });
            }
            else if (id == 2)
            {
                buildings.Sort((a, b) =>
                {
                    return b.Out0SmallVertextCount.CompareTo(a.Out0SmallVertextCount);
                });
            }
            else if (id == 3)
            {
                buildings.Sort((a, b) =>
                {
                    return b.Out1VertextCount.CompareTo(a.Out1VertextCount);
                });
            }
            else if (id == 4)
            {
                buildings.Sort((a, b) =>
                {
                    return b.InVertextCount.CompareTo(a.InVertextCount);
                });
            }

            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < buildings.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var b = buildings[i];
                var arg = FoldoutEditorArgBuffer.editorArgs[b];
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
            //Debug.Log($"Show BuildingList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    public void DrawTreeList(FoldoutEditorArg foldoutArg, System.Func<List<ModelAreaTree>> funcGetList)
    {
        List<ModelAreaTree> trees = new List<ModelAreaTree>();
        foldoutArg.caption = $"Tree List";
        EditorUIUtils.ToggleFoldout(foldoutArg,
        (arg) =>
        {
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

            trees.ForEach(b =>
            {
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
            //Debug.Log($"Init TreeList count:{trees.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        },
        () =>
        {
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
                var arg = FoldoutEditorArgBuffer.editorArgs[tree];
                if (arg == null)
                {
                    FoldoutEditorArgBuffer.editorArgs[tree] = new FoldoutEditorArg();
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

    public List<AreaTreeNode> DrawNodeList(FoldoutEditorArg foldoutArg, bool showTreeName, System.Func<List<AreaTreeNode>> funcGetList)
    {
        List<AreaTreeNode> nodes = new List<AreaTreeNode>();
        foldoutArg.caption = $"Node List";
        EditorUIUtils.ToggleFoldout(foldoutArg,
        (arg) =>
        {
            System.DateTime start = System.DateTime.Now;
            float sumVertexCount = 0;
            int sumRendererCount = 0;
            //nodes = item.GetLeafNodes();
            nodes = funcGetList();
            nodes.Sort((a, b) =>
            {
                return b.VertexCount.CompareTo(a.VertexCount);
            });
            nodes.ForEach(b =>
            {
                sumVertexCount += b.VertexCount;
                sumRendererCount += b.Renderers.Count;
            });
            InitEditorArg(nodes);
            arg.caption = $"Node List({nodes.Count})";
            arg.info = $"[{sumVertexCount:F0}w][{sumRendererCount / 10000f:F0}w]";
            var time = System.DateTime.Now - start;
            //Debug.Log($"Init NodeList count:{nodes.Count} time:{time.TotalMilliseconds:F1}ms ");
        },
        () =>
        {
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
                var arg = FoldoutEditorArgBuffer.editorArgs[node];
                string title = $"[{i + 1:00}] {node.tree.name}.{node.name}";
                if (showTreeName == false)
                {
                    title = $"[{i + 1:00}] {node.name}";
                }
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, title, $"[{node.VertexCount:F0}w][{node.Renderers.Count}]", false, false, false, node.gameObject);
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

    public void DrawObjectList<T1>(FoldoutEditorArg foldoutArg, string title, System.Func<List<T1>> funcGetList, System.Action<FoldoutEditorArg, T1, int> drawItemAction, System.Action<T1> toolBarAction, System.Action<FoldoutEditorArg, T1, int> drawSubListAction)
    {
        List<T1> list = new List<T1>();
        foldoutArg.caption = title;
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) =>
        {
            System.DateTime start = System.DateTime.Now;
            //scenes = item.scenes.ToList();
            list = funcGetList();
            InitEditorArg(list);
            arg.caption = $"{title}({list.Count})";
        }, () =>
        {
        });
        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(list.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < list.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var item = list[i];
                var arg = FoldoutEditorArgBuffer.editorArgs[item];
                arg.level = 1;
                arg.caption = $"[{i:00}] {item.ToString()}";
                arg.isFoldout = false;
                arg.isEnabled = true;

                Object obj = arg.tag as Object;
                if (item is Object)
                {
                    obj = item as Object;
                    arg.caption = obj.name;
                }

                if (drawItemAction != null)
                {
                    drawItemAction(arg, item, i);
                }

                EditorUIUtils.ObjectFoldout(arg, obj, () =>
                {
                    if (toolBarAction != null)
                    {
                        toolBarAction(item);
                    }
                });
                if (arg.isEnabled && arg.isExpanded)
                {
                    if (drawSubListAction != null)
                    {
                        drawSubListAction(arg, item, i);
                    }
                }

            }
            var time = System.DateTime.Now - start;
            //Debug.Log($"Show SceneList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    public void DrawSceneList(FoldoutEditorArg foldoutArg, System.Func<List<SubScene_Base>> funcGetList)
    {
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        foldoutArg.caption = $"Scenes";
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) =>
        {
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
            //Debug.Log($"Init SceneList count:{scenes.Count} time:{time.TotalMilliseconds:F1}ms ");
        }, () =>
        {
            int filterType = foldoutArg.DrawFilterList(100, 0, "All", "Combined", "Renderers");
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
                var arg = FoldoutEditorArgBuffer.editorArgs[scene];
                BuildingModelInfo modelInfo = scene.GetComponentInParent<BuildingModelInfo>();
                if (modelInfo == null)
                {
                    arg.caption = $"[{i + 1:00}] {scene.name}";
                }
                else
                {
                    arg.caption = $"[{i + 1:00}] {modelInfo.name}>{scene.name}";
                }

                arg.info = $"[{scene.vertexCount:F0}w][{scene.rendererCount}]";
                EditorUIUtils.ObjectFoldout(arg, scene.gameObject, null);
                //if (arg.isExpanded)
                //{
                //    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                //}
            }
            var time = System.DateTime.Now - start;
            //Debug.Log($"Show SceneList count:{c} time:{time.TotalMilliseconds:F1}ms ");
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
        //Debug.Log($"Init MeshList1 count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        meshes.ForEach(b =>
        {
            if (b == null) return;
            if (b.gameObject.activeInHierarchy == true && b.GetComponent<MeshRenderer>() != null && b.GetComponent<MeshRenderer>().enabled == true)
            {
                sumVertexCountVisible += b.sharedMesh.vertexCount;
                sumRendererCountVisible++;
            }
            sumVertexCount += b.sharedMesh.vertexCount;
            sumRendererCount++;
        });
        //Debug.Log($"Init MeshList2 count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        sumVertexCount /= 10000;
        sumVertexCountVisible /= 10000;
        //InitEditorArg(scenes);
        arg.caption = $"Mesh List({meshes.Count})";
        arg.info = $"[{sumVertexCountVisible:F0}/{sumVertexCount:F0}w][{sumRendererCountVisible}/{sumRendererCount}]";
        //Debug.LogWarning($"Init MeshList count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        arg.Items = meshes;
        return meshes;
    }
    public void DrawMeshList(FoldoutEditorArg<MeshFilter> foldoutArg, System.Func<List<MeshFilter>> funcGetList)
    {
        if (string.IsNullOrEmpty(foldoutArg.caption)) foldoutArg.caption = $"Mesh List";
        List<MeshFilter> meshFilters = foldoutArg.Items;
        EditorUIUtils.SetupStyles();
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) =>
        {

            if (meshFilters.Count == 0)
            {
                meshFilters = InitMeshList(arg as FoldoutEditorArg<MeshFilter>, funcGetList);
            }
            //else
            //{
            //    meshFilters = foldoutArg.Items;
            //}

        }, () =>
        {
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
                if (!FoldoutEditorArgBuffer.editorArgs.ContainsKey(mesh))
                {
                    FoldoutEditorArgBuffer.editorArgs.Add(mesh, new FoldoutEditorArg());
                }
                var arg = FoldoutEditorArgBuffer.editorArgs[mesh];
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
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i + 1:00}] {building.name}>>{mesh.transform.parent.name}>{mesh.name}", $"[{mesh.sharedMesh.vertexCount / 10000f:F1}w]", false, false, false, mesh.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time = System.DateTime.Now - start;
            Debug.Log($"Show MeshList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    private List<MeshRendererInfo> InitMeshListEx(FoldoutEditorArg<MeshRendererInfo> arg, System.Func<List<MeshRendererInfo>> funcGetList)
    {
        System.DateTime start = System.DateTime.Now;
        float sumVertexCount = 0;
        float sumVertexCountVisible = 0;
        int sumRendererCount = 0;
        int sumRendererCountVisible = 0;
        //List<MeshFilter> meshes = GameObject.FindObjectsOfType<MeshFilter>(true).Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
        List<MeshRendererInfo> meshes = funcGetList();
        meshes.Sort((a, b) =>
        {
            return b.vertexCount.CompareTo(a.vertexCount);
        });


        List<MeshRendererInfo> filteredList = new List<MeshRendererInfo>();
        if (arg.listFilterId > 0)
        {
            int lv = arg.listFilterId - 2;
            foreach (var mf in meshes)
            {
                if (lv == -1 && mf.LodIds.Count == 0)
                {
                    filteredList.Add(mf);
                }
                else if (mf.LodIds.Contains(lv))
                {
                    filteredList.Add(mf);
                }
            }
            meshes = filteredList;
        }

        //Debug.Log($"Init MeshList1 count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        meshes.ForEach(b =>
        {
            if (b == null) return;
            if (b.gameObject.activeInHierarchy == true && b.GetComponent<MeshRenderer>() != null && b.GetComponent<MeshRenderer>().enabled == true)
            {
                sumVertexCountVisible += b.vertexCount;
                sumRendererCountVisible++;
            }
            sumVertexCount += b.vertexCount;
            sumRendererCount++;
        });
        //Debug.Log($"Init MeshList2 count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        sumVertexCount /= 10000;
        sumVertexCountVisible /= 10000;
        //InitEditorArg(scenes);
        arg.caption = $"Mesh Info List({meshes.Count})";
        arg.info = $"[{sumVertexCountVisible:F0}/{sumVertexCount:F0}w][{sumRendererCountVisible}/{sumRendererCount}]";
        //Debug.LogWarning($"Init MeshList count:{meshes.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        arg.Items = meshes;
        return meshes;
    }

    public void DrawMeshListEx(FoldoutEditorArg<MeshRendererInfo> foldoutArg, System.Func<List<MeshRendererInfo>> funcGetList)
    {
        if (string.IsNullOrEmpty(foldoutArg.caption)) foldoutArg.caption = $"Mesh Info List";
        List<MeshRendererInfo> meshFilters = foldoutArg.Items;
        EditorUIUtils.SetupStyles();
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) =>
        {

            if (meshFilters.Count == 0)
            {
                meshFilters = InitMeshListEx(arg as FoldoutEditorArg<MeshRendererInfo>, funcGetList);
            }
            //else
            //{
            //    meshFilters = foldoutArg.Items;
            //}

        }, () =>
        {
            int lodtypeId = foldoutArg.DrawFilterList(60, 0, "All", "NoLOD", "LOD0", "LOD1", "LOD2", "LOD3", "LOD4");

            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                foldoutArg.Items.Clear();
                meshFilters = InitMeshListEx(foldoutArg, funcGetList);
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
            //int lodtypeId=foldoutArg.DrawFilterList(80, "NoLOD", "LOD0", "LOD1", "LOD2", "LOD3", "LOD4");
            //int lodtypeId = foldoutArg.DrawPageToolbarWithFilter(meshFilters.Count, 80,0, "All","NoLOD", "LOD0", "LOD1", "LOD2", "LOD3", "LOD4");
            foldoutArg.DrawPageToolbar(meshFilters.Count);

            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < meshFilters.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var mesh = meshFilters[i];
                if (!FoldoutEditorArgBuffer.editorArgs.ContainsKey(mesh))
                {
                    FoldoutEditorArgBuffer.editorArgs.Add(mesh, new FoldoutEditorArg());
                }
                var arg = FoldoutEditorArgBuffer.editorArgs[mesh];
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
                //if (mesh.sharedMesh == null)
                //{
                //    Debug.LogError($"Show MeshList mesh.sharedMesh==null"); continue;
                //}
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
                arg.caption = $"[{i + 1:00}] {building.name}>>{mesh.transform.parent.name}>{mesh.name}";
                arg.info = $"[{mesh.vertexCount / 10000f:F1}w][{mesh.GetDiam():F1}](LOD[{mesh.GetLODIds()}])";
                EditorUIUtils.ObjectFoldout(arg, mesh.gameObject, null);
                //if (arg.isExpanded)
                //{
                //    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                //}
            }
            var time = System.DateTime.Now - start;
            //Debug.Log($"Show MeshList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    public void DrawMatList(GlobalMaterialManager item, FoldoutEditorArg matListArg)
    {
        //if (GUILayout.Button("InitMaterials"))
        //{
        //    item.GetSharedMaterials();
        //}

        //item.LocalTarget = EditorGUILayout.ObjectField(item.LocalTarget, typeof(GameObject)) as GameObject;

        matListArg.caption = $"Material List";
        EditorUIUtils.ToggleFoldout(matListArg, arg =>
        {
            arg.caption = $"Material List ({item.meshMaterialList.Count})";
            arg.info = $"Renderers:{item.meshMaterialList.RendererCount}";
            InitEditorArg(item.meshMaterialList);
        },
        () =>
        {
            //MeshCombineHelper.GetMatFilters
            if (GUILayout.Button("Update"))
            {
                RemoveEditorArg(item.meshMaterialList);
                item.GetSharedMaterials();
                InitEditorArg(item.meshMaterialList);
            }
        });

        if (matListArg.isEnabled && matListArg.isExpanded)
        {
            if (item.LocalTarget != null)
            {
                EditorGUILayout.BeginHorizontal();
                item.DefaultColor = EditorGUILayout.ColorField("DefaultColor", item.DefaultColor);
                if (GUILayout.Button("ResetColor"))
                {
                    item.ResetColor();
                }
                EditorGUILayout.EndHorizontal();
            }

            matListArg.DrawPageToolbar(item.meshMaterialList, (meshMat, i) =>
            {

                var matArg = FoldoutEditorArgBuffer.editorArgs[meshMat];
                matArg.background = true;
                matArg.caption = $"[{i + 1:00}] {meshMat.GetName()} ({meshMat.subMeshs.Count})";
                //matArg.info = $"count:{meshMat.subMeshs.Count}";
                EditorUIUtils.ObjectFoldout(matArg, meshMat.GetMat(), () =>
                {
                    if (meshMat != null && meshMat.matInfo != null && meshMat.matInfo.shader != null)
                    {
                        EditorGUILayout.LabelField(meshMat.matInfo.shader.name, GUILayout.Width(60));

                        var newColor = EditorGUILayout.ColorField("Color", meshMat.GetColor(), GUILayout.Width(50));
                        meshMat.SetColor(newColor);
                        bool isD = EditorGUILayout.Toggle(meshMat.matInfo.isDoubleSide, GUILayout.Width(20));
                        meshMat.matInfo.SetIsDoubleSide(isD);
                        Texture tex = EditorGUILayout.ObjectField(meshMat.matInfo.tex, typeof(Texture), GUILayout.Width(100)) as Texture;
                        meshMat.matInfo.SetTexture(tex);

                        Texture normal = EditorGUILayout.ObjectField(meshMat.matInfo.normal, typeof(Texture), GUILayout.Width(100)) as Texture;
                        meshMat.matInfo.SetNormal(normal);
                    }

                });

                if (matArg.isExpanded)
                {
                    InitEditorArg(meshMat.subMeshs);
                    matArg.DrawPageToolbar(meshMat.subMeshs, (subMesh, j) =>
                    {

                        var arg = FoldoutEditorArgBuffer.editorArgs[subMesh];
                        arg.caption = $"[{j + 1:00}] {subMesh.GetName()}";
                        //arg.info = $"count:{subMesh.subMeshs.Count}";
                        EditorUIUtils.ObjectFoldout(arg, subMesh.meshFilter.gameObject, () =>
                        {
                            //EditorGUILayout.ColorField("Color", subMesh.GetColor(), GUILayout.Width(50));
                        });
                    });
                }
            });
        }


    }

    public void DrawDoorPartList(FoldoutEditorArg doorListArg, DoorManager item)
    {
        //ObjectField(item.LocalTarget);
        doorListArg.caption = $"Door Part List";
        EditorUIUtils.ToggleFoldout(doorListArg, arg =>
        {
            var doors = item.GetDoorParts();
            arg.caption = $"Door Part List ({doors.Count})(LOD{doors.lodCount})";
            arg.info = $"{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors);
        },
        () =>
        {
            //item.IsOnlyActive = EditorGUILayout.Toggle("Active", item.IsOnlyActive);
            item.IsOnlyActive = GUILayout.Toggle(item.IsOnlyActive, "Active");
            //item.IsOnlyCanSplit = EditorGUILayout.Toggle("CanSplit", item.IsOnlyCanSplit);
            item.IsOnlyCanSplit = GUILayout.Toggle(item.IsOnlyCanSplit, "Split");
            if (GUILayout.Button("Split", GUILayout.Width(40)))
            {
                item.SplitAll();
            }
            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                RemoveEditorArg(item.GetDoorParts());
                item.UpdateDoors();
                InitEditorArg(item.GetDoorParts());
            }
        });
        if (doorListArg.isEnabled && doorListArg.isExpanded)
        {
            EditorGUILayout.BeginHorizontal();


            EditorGUILayout.EndHorizontal();
            var doors = item.GetDoorParts();
            InitEditorArg(doors);
            doorListArg.DrawPageToolbar(doors, (door, i) =>
            {
                var arg = FoldoutEditorArgBuffer.editorArgs[door];
                arg.caption = $"[{i + 1:00}] {door.GetTitle()}";
                arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, door.gameObject, () =>
                {
                    if (GUILayout.Button("Split", GUILayout.Width(50)))
                    {
                        Debug.Log($"Split:{door.GetTitle()}");
                        GameObject result = MeshCombineHelper.SplitByMaterials(door.gameObject, false);
                    }
                });
            });
        }
    }



    public void DrawPrefabInstanceList()
    {

    }

    public void DrawDoorsRootList(FoldoutEditorArg doorRootListArg, DoorManager item)
    {
        //ObjectField(item.LocalTarget);
        doorRootListArg.caption = $"DoorsRoot List";
        doorRootListArg.level = 0;
        EditorUIUtils.ToggleFoldout(doorRootListArg, arg =>
        {
            var doors = item.doorRoots;
            int count = doors.GetDoors().Count;
            arg.caption = $"DoorsRoot List ({doors.Count})";
            arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors);
        },
        () =>
        {
            if (GUILayout.Button("Update"))
            {
                RemoveEditorArg(item.doorRoots);
                InitEditorArg(item.UpdateDoors());
            }
        });
        if (doorRootListArg.isEnabled && doorRootListArg.isExpanded)
        {
            //EditorGUILayout.BeginHorizontal();
            //item.IsOnlyActive = EditorGUILayout.Toggle("Active", item.IsOnlyActive);
            //item.IsOnlyCanSplit = EditorGUILayout.Toggle("CanSplit", item.IsOnlyCanSplit);
            //if (GUILayout.Button("SplitAll", GUILayout.Width(50)))
            //{
            //    item.SplitAll();
            //}
            //EditorGUILayout.EndHorizontal();
            var rootList = item.doorRoots;
            InitEditorArg(rootList);
            doorRootListArg.DrawPageToolbar(rootList, (doorRoot, i) =>
            {
                if (doorRoot == null) return;
                var doorRootArg = FoldoutEditorArgBuffer.editorArgs[doorRoot];
                doorRootArg.level = 1;
                doorRootArg.background = true;
                doorRootArg.caption = $"[{i + 1:00}] {doorRoot.GetTitle()}";
                doorRootArg.info = doorRoot.ToString();
                //EditorUIUtils.ObjectFoldout(doorRootArg, doorRoot.gameObject, () =>
                //{

                //});

                EditorUIUtils.ObjectFoldout(doorRootArg, doorRoot.gameObject, () =>
                {
                    if (GUILayout.Button("Share", GUILayout.Width(50)))
                    {
                        doorRoot.SetDoorShared();
                    }
                });

                DrawDoorList(doorRootArg, doorRoot.Doors, true);
            });
        }
    }

    public void DrawDoorList(FoldoutEditorArg doorRootArg, DoorInfoList doors, bool isSubList)
    {
        if (isSubList == false)
        {
            doorRootArg.caption = $"Door List";
            doorRootArg.level = 0;
            EditorUIUtils.ToggleFoldout(doorRootArg, arg =>
            {
                //var doors = doorsRoot.Doors;
                arg.caption = $"Door List ({doors.Count})";
                arg.info = $"{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
                InitEditorArg(doors);
            },
            () =>
            {
                //if (GUILayout.Button("Update"))
                //{
                //    RemoveEditorArg(doorsRoot.doorRoots);
                //    InitEditorArg(item.UpdateDoors());
                //}
            });
        }


        if (doorRootArg.isExpanded)
        {
            doorRootArg.level = 1;
            //var doors = doorsRoot.Doors;
            InitEditorArg(doors);
            doorRootArg.DrawPageToolbar(doors, (door, i) =>
            {
                var arg = FoldoutEditorArgBuffer.editorArgs[door];
                arg.caption = $"[{i + 1:00}] {door.GetTitle()}";
                arg.info = door.ToString();
                arg.level = 2;
                EditorUIUtils.ObjectFoldout(arg, door.gameObject, () =>
                {

                });
            });
        }
    }

    protected static void DrawLODGroupList(FoldoutEditorArg<LODGroupDetails> lodGroupListArg, LODManager lodManager)
    {
        lodGroupListArg.caption = $"LOD List";
        EditorUIUtils.ToggleFoldout(lodGroupListArg, arg =>
        {
            var lods = lodManager.lodDetails;
            if (arg.listFilterId == 1)
            {
                lods = lods.Where(i => i.group != null && i.group.gameObject.activeInHierarchy).ToList();
            }
            else if (arg.listFilterId == 2)
            {
                lods = lods.Where(i => i.group != null && i.group.gameObject.activeInHierarchy == false).ToList();
            }
            else if (arg.listFilterId == 3)
            {
                lods = lods.Where(i => i.group!=null && i.group.gameObject.name.Contains("_Door")==false).ToList();
            }

            if (Application.isPlaying)
            {
                lods.Sort((a, b) =>
                {
                    int r1 = a.currentInfo.currentLevel.CompareTo(b.currentInfo.currentLevel);
                    if (r1 == 0)
                    {
                        r1 = b.currentInfo.currentPercentage.CompareTo(a.currentInfo.currentPercentage);
                    }
                    //if (r1 == 0)
                    //{
                    //    r1 = b.currentChild.renderers.CompareTo(a.currentInfo.currentPercentage);
                    //}
                    return r1;
                });
            }

            lodGroupListArg.Items = lods;

            float[] sumVertex = new float[4];
            for (int i = 0; i < lods.Count; i++)
            {
                LODGroupDetails lod = (LODGroupDetails)lods[i];
                //sumVertex[i] += lod.chi
                for (int j = 0; j < lod.childs.Count; j++)
                {
                    sumVertex[j] += lod.childs[j].vertexCount;
                }
            }
            arg.caption = $"LOD List ({lods.Count})";
            string txt = "";
            float v0 = 0;
            float sv = 0;
            for (int i = 0; i < sumVertex.Length; i++)
            {
                float v = sumVertex[i];
                if (i == 0)
                {
                    v0 = v;
                }
                if (i <= 1)
                {
                    txt += $"{v / 10000f:F0}({v / v0:P0})|";
                }
                else
                {
                    txt += $"{v / 10000f:F1}({v / v0:P0})|";
                }

                sv += v;
            }
            arg.info = $"({sv / 10000f:F0}){txt}";
            InitEditorArg(lods);
        },
        () =>
        {
            lodGroupListArg.DrawFilterList(100, 0, "All", "Active", "InActive","NotDoor");
        });
        if (lodGroupListArg.isEnabled && lodGroupListArg.isExpanded)
        {
            var lods = lodGroupListArg.Items;
            InitEditorArg(lods);
            lodGroupListArg.DrawPageToolbar(lods, (lodDetail, i) =>
            {
                var arg = FoldoutEditorArgBuffer.editorArgs[lodDetail];
                if (lodDetail.group == null) return;
                arg.caption = $"[{i:00}] {lodDetail.GetCaption()}";
                arg.level = 1;
                //arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, lodDetail.group.gameObject, () =>
                {
                    float lod0Vertex = 0;
                    for (int i = 0; i < lodDetail.childs.Count; i++)
                    {
                        var lodChild = lodDetail.childs[i];
                        //float vertexF = lodChild.GetVertexCountF();
                        string vertexS = MeshHelper.GetVertexCountS(lodChild.vertexCount);
                        //if (vertexF > 100)
                        //{
                        //    vertexS = vertexF.ToString("F0");
                        //}
                        //else if (vertexF > 10)
                        //{
                        //    vertexS = vertexF.ToString("F1");
                        //}
                        //else
                        //{
                        //    vertexS = vertexF.ToString("F2");
                        //}
                        if (i == 0)
                        {
                            lod0Vertex = lodChild.vertexCount;
                        }
                        else
                        {

                        }

                        string btnName = $"L{i}[{vertexS}][{lodChild.vertexCount / lod0Vertex:P0}][{lodChild.screenRelativeTransitionHeight:F1}]";

                        if (lodChild == lodDetail.currentChild)
                        {
                            btnName += "*";
                        }
                        if (GUILayout.Button(btnName, GUILayout.Width(140)))
                        {
                            //EditorHelper.SelectObject()
                            EditorHelper.SelectObjects(lodChild.renderers);
                        }
                    }
                });
            });
        }
    }

    protected bool DrawSharedMeshListEx(FoldoutEditorArg listArg, System.Func<SharedMeshInfoList> funcGetList)
    {
        bool isUpate = false;
        listArg.caption = $"SharedMesh List";
        SharedMeshInfoList list = listArg.tag as SharedMeshInfoList;
        if (list == null)
        {
            list = new SharedMeshInfoList();
        }
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {

            arg.caption = $"SharedMesh List ({list.Count})";
            arg.info = $"{MeshHelper.GetVertexCountS(list.sharedVertexCount)}/{MeshHelper.GetVertexCountS(list.totalVertexCount)}({list.sharedVertexCount / (float)list.totalVertexCount:P1})|{list.filterCount}";
            InitEditorArg(list);
        },
        () =>
        {
            var btnStyle = new GUIStyle(EditorStyles.miniButton);
            btnStyle.margin = new RectOffset(0, 0, 0, 0);
            btnStyle.padding = new RectOffset(0, 0, 0, 0);

            if (funcGetList != null)
            {
                if (GUILayout.Button("Update", btnStyle, GUILayout.Width(56)))
                {
                    listArg.tag = funcGetList();
                }
            }
            PrefabInstanceBuilder.Instance.JobSize=EditorGUILayout.IntField(PrefabInstanceBuilder.Instance.JobSize, GUILayout.Width(50));
            if (GUILayout.Button("Prefabs", btnStyle, GUILayout.Width(66)))
            {
                SharedMeshInfoList list2= funcGetList();
                //list.GetPrefabs();
                listArg.tag = list2;

                PrefabInstanceBuilder.Instance.GetPrefabInfos(list2, true);

                listArg.tag = funcGetList();
                isUpate = true;
            }
            //if (GUILayout.Button("X1", btnStyle, GUILayout.Width(25)))
            //{
            //    list.Destroy(1);
            //}
            //if (GUILayout.Button("X2", btnStyle, GUILayout.Width(25)))
            //{
            //    list.Destroy(2);
            //}
            //if (GUILayout.Button("X3", btnStyle, GUILayout.Width(25)))
            //{
            //    list.Destroy(3);
            //}
            //if (GUILayout.Button("X4", btnStyle, GUILayout.Width(25)))
            //{
            //    list.Destroy(4);
            //}
            //if (GUILayout.Button("X5", btnStyle, GUILayout.Width(25)))
            //{
            //    list.Destroy(5);
            //}
        });
        DrawSharedMeshList(listArg);
        return isUpate;
    }

    protected void DrawSharedMeshList(FoldoutEditorArg listArg)
    {
        //listArg.level = level;
        //var nodes = item.GetMeshNodes();
        var list = listArg.tag as SharedMeshInfoList;
        if (list == null)
        {
            list = new SharedMeshInfoList();
        }
        if (listArg.isEnabled && listArg.isExpanded)
        {

            InitEditorArg(list);

            int sortType = listArg.DrawPageToolbarWithSort(list.Count, 100, 0, "SharedV", "AllV", "SharedCount");
            list.SortByType(sortType);

            for (int i = listArg.GetStartId(); i < list.Count && i < listArg.GetEndId(); i++)
            {
                var node = list[i];
                if (node == null)
                {
                    var arg = new FoldoutEditorArg();
                    arg.caption = "NULL";
                    //arg.info = $"{MeshHelper.GetVertexCountS(list.vertexCount)}|{list.filterCount}";
                    EditorUIUtils.ObjectFoldout(new FoldoutEditorArg(), null, null);
                }
                else
                {
                    var arg = FoldoutEditorArgBuffer.editorArgs[node];
                    arg.level = 1;
                    arg.isFoldout = node.GetCount() > 0;
                    arg.caption = $"[{i:00}] {node.GetName()} ({node.GetCount()})";
                    arg.isEnabled = true;
                    arg.info = $"{MeshHelper.GetVertexCountS(node.vertexCount)}|{MeshHelper.GetVertexCountS(node.GetAllVertexCount())}[{node.GetAllVertexCount() / (float)list.totalVertexCount:P1}]";
                    //if (level == 0)
                    //{
                    //    arg.background = true;
                    //    arg.bold = node == item;
                    //}

                    //EditorUIUtils.ObjectFoldout(arg, node.GetMainMeshFilter().gameObject, () =>
                    //{

                    //});

                    EditorUIUtils.ObjectFoldout(arg, node.mesh, () =>
                    {
                        var btnStyle = new GUIStyle(EditorStyles.miniButton);
                        btnStyle.margin = new RectOffset(0, 0, 0, 0);
                        btnStyle.padding = new RectOffset(0, 0, 0, 0);
                        if (GUILayout.Button("S", btnStyle, GUILayout.Width(25)))
                        {
                            EditorHelper.SelectObjects(node.GetGameObjects());
                        }
                        //if (GUILayout.Button("X1", btnStyle, GUILayout.Width(25)))
                        //{
                        //    node.Destroy(1);
                        //}
                    }, () =>
                    {
                        node.Destroy();
                        list.Remove(node);
                    });

                    if (arg.isEnabled && arg.isExpanded)
                    {

                        var filters = node.meshFilters;
                        InitEditorArg(filters);
                        foreach (var mf in filters)
                        {
                            if (mf == null) continue;
                            var mfArg = FoldoutEditorArgBuffer.editorArgs[mf];
                            mfArg.isFoldout = false;
                            mfArg.level = 2;
                            if (mf.transform.parent == null)
                            {
                                mfArg.caption = "[ROOT]> " + mf.name;
                            }
                            else
                            {
                                mfArg.caption = mf.transform.parent.name + "> " + mf.name;
                            }

                            EditorUIUtils.ObjectFoldout(mfArg, mf.gameObject, () =>
                            {

                            });
                        }
                    }
                }
            }

            // listArg.DrawPageToolbar(list, (node, i) =>
            //{
            //    if (node == null)
            //    {
            //        var arg = new FoldoutEditorArg();
            //        arg.caption = "NULL";
            //        //arg.info = $"{MeshHelper.GetVertexCountS(list.vertexCount)}|{list.filterCount}";
            //        EditorUIUtils.ObjectFoldout(new FoldoutEditorArg(), null, null);
            //    }
            //    else
            //    {
            //        var arg = FoldoutEditorArgBuffer.editorArgs[node];
            //        arg.level = 1;
            //        arg.isFoldout = node.GetCount() > 0;
            //        arg.caption = $"[{i:00}] {node.GetName()} ({node.GetCount()})";
            //        arg.isEnabled = true;
            //        arg.info = $"{MeshHelper.GetVertexCountS(node.vertexCount)}|{MeshHelper.GetVertexCountS(node.GetAllVertexCount())}[{node.GetAllVertexCount() / (float)list.totalVertexCount:P1}]";
            //        //if (level == 0)
            //        //{
            //        //    arg.background = true;
            //        //    arg.bold = node == item;
            //        //}

            //        //EditorUIUtils.ObjectFoldout(arg, node.GetMainMeshFilter().gameObject, () =>
            //        //{

            //        //});

            //        EditorUIUtils.ObjectFoldout(arg, node.mesh, () =>
            //        {
            //            var btnStyle = new GUIStyle(EditorStyles.miniButton);
            //            btnStyle.margin = new RectOffset(0, 0, 0, 0);
            //            btnStyle.padding = new RectOffset(0, 0, 0, 0);
            //            if (GUILayout.Button("S", btnStyle, GUILayout.Width(25)))
            //            {
            //                EditorHelper.SelectObjects(node.GetGameObjects());
            //            }
            //            //if (GUILayout.Button("X1", btnStyle, GUILayout.Width(25)))
            //            //{
            //            //    node.Destroy(1);
            //            //}
            //        }, () =>
            //         {
            //             node.Destroy();
            //             list.Remove(node);
            //         });

            //        if (arg.isEnabled && arg.isExpanded)
            //        {

            //            var filters = node.meshFilters;
            //            InitEditorArg(filters);
            //            foreach (var mf in filters)
            //            {
            //                if (mf == null) continue;
            //                var mfArg = FoldoutEditorArgBuffer.editorArgs[mf];
            //                mfArg.isFoldout = false;
            //                mfArg.level = 2;
            //                if (mf.transform.parent == null)
            //                {
            //                    mfArg.caption = "[ROOT]> " + mf.name;
            //                }
            //                else
            //                {
            //                    mfArg.caption = mf.transform.parent.name + "> " + mf.name;
            //                }

            //                EditorUIUtils.ObjectFoldout(mfArg, mf.gameObject, () =>
            //                {

            //                });
            //            }
            //        }
            //    }
            //});
        }
    }

    public static void DrawPrefabList(FoldoutEditorArg prefabListArg, System.Func<PrefabInfoList> funcGetList)
    {
        BaseFoldoutEditorHelper.DrawPrefabList(prefabListArg, funcGetList);
    }
}
public static class FoldoutEditorArgBuffer
{
    public static Dictionary<System.Object, FoldoutEditorArg> editorArgs = new Dictionary<System.Object, FoldoutEditorArg>();

    public static void InitEditorArg<T2>(List<T2> items)/* where T2 : System.Object*/
    {
        foreach (var item in items)
        {
            if (item == null) continue;
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

    public static void InitEditorArg<T2>(List<T2> items, FoldoutEditorArg newArg)/* where T2 : System.Object*/
    {
        foreach (var item in items)
        {
            if (item == null) continue;
            if (!editorArgs.ContainsKey(item))
            {
                editorArgs.Add(item, newArg.Clone());
            }
            else
            {
                if (editorArgs[item] == null)
                {
                    editorArgs[item] = newArg.Clone();
                }
            }
        }
    }

    public static FoldoutEditorArg GetEditorArg<T2>(T2 item, FoldoutEditorArg newArg)/* where T2 : System.Object*/
    {
        if (newArg == null)
        {
            newArg = new FoldoutEditorArg();
        }
        if (!editorArgs.ContainsKey(item))
        {
            editorArgs.Add(item, newArg);
        }
        return editorArgs[item];
    }

    public static Dictionary<System.Object, FoldoutEditorArg> editorArgs_global = new Dictionary<System.Object, FoldoutEditorArg>();

    public static FoldoutEditorArg GetGlobalEditorArg<T2>(T2 item, FoldoutEditorArg newArg)/* where T2 : System.Object*/
    {
        if (newArg == null)
        {
            newArg = new FoldoutEditorArg();
        }
        if (!editorArgs_global.ContainsKey(item))
        {
            editorArgs_global.Add(item, newArg);
        }
        return editorArgs_global[item];
    }

    public static void RemoveEditorArg<T2>(List<T2> items)/* where T2 : System.Object*/
    {
        foreach (var item in items)
        {
            if (item == null)
            {

                continue;
            }
            if (editorArgs.ContainsKey(item))
            {
                editorArgs.Remove(item);
            }
        }
    }


}



public static class BaseFoldoutEditorHelper
{
    
    public static void DrawPrefabList(FoldoutEditorArg prefabListArg, System.Func<PrefabInfoList> funcGetList)
    {
        //ObjectField(item.LocalTarget);
        prefabListArg.caption = $"Prefab List";
        prefabListArg.level = 0;
        EditorUIUtils.ToggleFoldout(prefabListArg, arg =>
        {
            var prefabs = funcGetList();
            int vCount = 0;
            int vAllCount = 0;
            int rCount = 0;
            prefabs.ForEach(i =>
            {
                vCount += i.VertexCount;
                vAllCount += i.VertexCount * (i.InstanceCount + 1);
                rCount += (i.InstanceCount + 1);
            });
            arg.caption = $"Prefab List ({prefabs.Count})";
            arg.info = $"r:{rCount}|{MeshHelper.GetVertexCountS(vCount)}/{MeshHelper.GetVertexCountS(vAllCount)}({(float)vCount / vAllCount:P1})";
            FoldoutEditorArgBuffer.InitEditorArg(prefabs);
        },
        () =>
        {
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.doorRoots);
            //    InitEditorArg(item.UpdateDoors());
            //}
        });
        if (prefabListArg.isEnabled && prefabListArg.isExpanded)
        {
            var prefabList = funcGetList();
            FoldoutEditorArgBuffer.InitEditorArg(prefabList);
            prefabListArg.DrawPageToolbar(prefabList, (prefabInfo, i) =>
            {
                var prefabInfoArg = FoldoutEditorArgBuffer.editorArgs[prefabInfo];
                prefabInfoArg.level = 1;
                prefabInfoArg.background = true;
                prefabInfoArg.caption = $"[{i + 1:00}] {prefabInfo.GetTitle()}";
                prefabInfoArg.info = prefabInfo.ToString();
                EditorUIUtils.ObjectFoldout(prefabInfoArg, prefabInfo.Prefab, () =>
                {
                });

                if (prefabInfoArg.isExpanded)
                {
                    FoldoutEditorArgBuffer.InitEditorArg(prefabInfo.Instances);
                    prefabInfoArg.DrawPageToolbar(prefabInfo.Instances, (ins, j) =>
                    {
                        if (ins == null) return;
                        var insArg = FoldoutEditorArgBuffer.editorArgs[ins];
                        insArg.level = 2;
                        insArg.background = true;
                        insArg.caption = $"[{i + 1:00}] {ins.name}";
                        //insArg.info = prefabInfo.ToString();
                        EditorUIUtils.ObjectFoldout(insArg, ins, () =>
                        {
                        });
                    });
                }

                //DrawDoorList(doorRootArg, doorRoot, true);
            });
        }
    }

    public static void DrawTransformList(List<Transform> transList, FoldoutEditorArg<Transform> listArg, System.Action updateAction, string listName)
    {
        listArg.caption = $"{listName}";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var list1 = transList;
            int count = list1.Count;
            arg.caption = $"{listName} ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            //InitEditorArg(list1);
            FoldoutEditorArgBuffer.InitEditorArg(list1);
        },
        () =>
        {
            if (updateAction != null)
            {
                if (GUILayout.Button("Update"))
                {
                    //item.GetAllModelTransform();
                    updateAction();
                }
            }

            //if (GUILayout.Button("Compare"))
            //{
            //    item.CompareModelVueInfo_Model2Vue();
            //}
        });
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = transList;
            //InitEditorArg(list1);
            FoldoutEditorArgBuffer.InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var itemArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                itemArg.level = 1;
                itemArg.background = true;
                itemArg.caption = $"[{i + 1:00}] {listItem.name}";
                //doorRootArg.info = $"{listItem.Distance}|{listItem.MId}|{listItem.MName}";
                EditorUIUtils.ObjectFoldout(itemArg, listItem.gameObject, () =>
                {
                });
            });
        }
    }

    public static void DrawGameObjectList(string listName, List<GameObject> transList, FoldoutEditorArg<GameObject> listArg
        , System.Action toolbarEvent,System.Action<GameObject> itemToolbarEvent = null, System.Action<GameObject> itemdestroyAction = null)
    {
        listArg.caption = $"{listName}";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var list1 = transList;
            int count = list1.Count;
            arg.caption = $"{listName} ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            //InitEditorArg(list1);
            FoldoutEditorArgBuffer.InitEditorArg(list1);
        },
        //() =>
        //{
        //    //if (updateAction != null)
        //    //{
        //    //    if (GUILayout.Button("Update"))
        //    //    {
        //    //        //item.GetAllModelTransform();
        //    //        updateAction();
        //    //    }
        //    //}

        //    //if (GUILayout.Button("Compare"))
        //    //{
        //    //    item.CompareModelVueInfo_Model2Vue();
        //    //}
        //}

        toolbarEvent);
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = transList;
            //InitEditorArg(list1);
            FoldoutEditorArgBuffer.InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var itemArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                itemArg.level = 1;
                itemArg.background = true;
                itemArg.caption = $"[{i + 1:00}] {listItem.name}";
                //doorRootArg.info = $"{listItem.Distance}|{listItem.MId}|{listItem.MName}";
                EditorUIUtils.ObjectFoldout(itemArg, listItem, ()=>
                {
                    if (itemToolbarEvent != null)
                    {
                        itemToolbarEvent(listItem);
                    }
                },()=>
                {
                    if (itemdestroyAction != null)
                    {
                        itemdestroyAction(listItem);
                    }
                });
            });
        }
    }

    public static void DrawObjectList<T>(string listName, List<T> transList, FoldoutEditorArg<T> listArg
        , System.Action toolbarEvent, System.Action<T> itemToolbarEvent = null, System.Action<T> itemdestroyAction = null) where T:IGameObject
    {
        listArg.caption = $"{listName}";
        listArg.level = 0;
        EditorUIUtils.ToggleFoldout(listArg, arg =>
        {
            var list1 = transList;
            int count = list1.Count;
            arg.caption = $"{listName} ({list1.Count})";
            //arg.info = $"d:{count}|{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            //InitEditorArg(list1);
            FoldoutEditorArgBuffer.InitEditorArg(list1);
        },
        //() =>
        //{
        //    //if (updateAction != null)
        //    //{
        //    //    if (GUILayout.Button("Update"))
        //    //    {
        //    //        //item.GetAllModelTransform();
        //    //        updateAction();
        //    //    }
        //    //}

        //    //if (GUILayout.Button("Compare"))
        //    //{
        //    //    item.CompareModelVueInfo_Model2Vue();
        //    //}
        //}

        toolbarEvent);
        if (listArg.isEnabled && listArg.isExpanded)
        {
            var list1 = transList;
            //InitEditorArg(list1);
            FoldoutEditorArgBuffer.InitEditorArg(list1);
            listArg.DrawPageToolbar(list1, (listItem, i) =>
            {
                if (listItem == null) return;
                var itemArg = FoldoutEditorArgBuffer.editorArgs[listItem];
                itemArg.level = 1;
                itemArg.background = true;
                itemArg.caption = $"[{i + 1:00}] {listItem.GetName()}";
                //doorRootArg.info = $"{listItem.Distance}|{listItem.MId}|{listItem.MName}";
                EditorUIUtils.ObjectFoldout(itemArg, listItem.GetGameObject(), () =>
                {
                    if (itemToolbarEvent != null)
                    {
                        itemToolbarEvent(listItem);
                    }
                }, () =>
                {
                    if (itemdestroyAction != null)
                    {
                        itemdestroyAction(listItem);
                    }
                });
            });
        }
    }
}
