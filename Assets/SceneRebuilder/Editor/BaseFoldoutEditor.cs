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

    public List<AreaTreeNode> DrawNodeList(FoldoutEditorArg foldoutArg, bool showTreeName,System.Func<List<AreaTreeNode>> funcGetList)
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
                var arg = editorArgs[node];
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

    public void DrawObjectList<T1>(FoldoutEditorArg foldoutArg,string title, System.Func<List<T1>> funcGetList,System.Action<T1> toolBarAction)
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
                var arg = editorArgs[item];
                arg.level = 1;
                arg.caption = $"[{i:00}] {item.ToString()}";
                arg.isFoldout = false;
                EditorUIUtils.ObjectFoldout(arg, null, ()=>
                {
                    if (toolBarAction != null)
                    {
                        toolBarAction(item);
                    }
                });
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
            int filterType = foldoutArg.DrawFilterList(100, 0,"All", "Combined", "Renderers");
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
                BuildingModelInfo modelInfo = scene.GetComponentInParent<BuildingModelInfo>();
                arg.caption = $"[{i + 1:00}] {modelInfo.name}>{scene.name}";
                arg.info = $"[{scene.vertexCount:F0}w][{scene.rendererCount}]";
                EditorUIUtils.ObjectFoldout(arg, scene.gameObject,null);
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
                arg.info = $"[{mesh.vertexCount / 10000f:F1}w][{mesh.size}](LOD[{mesh.GetLODIds()}])";
               EditorUIUtils.ObjectFoldout(arg, mesh.gameObject,null);
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

                var matArg = editorArgs[meshMat];
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

                        var arg = editorArgs[subMesh];
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

    public void DrawDoorList(FoldoutEditorArg doorListArg, DoorManager item)
    {
        doorListArg.caption = $"Door List";
        EditorUIUtils.ToggleFoldout(doorListArg, arg =>
        {
            var doors = item.GetDoors();
            arg.caption = $"Door List ({doors.Count})";
            arg.info = $"{doors.VertexCount_Show / 10000f:F0}/{doors.VertexCount / 10000f:F0}";
            InitEditorArg(doors);
        },
        () =>
        {
            if (GUILayout.Button("Update"))
            {
                RemoveEditorArg(item.GetDoors());
                InitEditorArg(item.UpdateDoors());
            }
        });
        if (doorListArg.isEnabled && doorListArg.isExpanded)
        {
            EditorGUILayout.BeginHorizontal();
            item.IsOnlyActive = EditorGUILayout.Toggle("Active", item.IsOnlyActive);
            item.IsOnlyCanSplit = EditorGUILayout.Toggle("CanSplit", item.IsOnlyCanSplit);
            if (GUILayout.Button("SplitAll", GUILayout.Width(50)))
            {
                item.SplitAll();
            }
            EditorGUILayout.EndHorizontal();
            var doors = item.GetDoors();
            InitEditorArg(doors);
            doorListArg.DrawPageToolbar(doors, (door, i) =>
            {
                var arg = editorArgs[door];
                arg.caption = $"[{i + 1:00}] {door.GetTitle()}";
                arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, door.DoorGo, () =>
                {
                    if (GUILayout.Button("Split", GUILayout.Width(50)))
                    {
                        Debug.Log($"Split:{door.GetTitle()}");
                        GameObject result = MeshCombineHelper.SplitByMaterials(door.DoorGo);
                    }
                });
            });
        }
    }

    public void DrawLODGroupList(FoldoutEditorArg lodGroupListArg, LODManager item)
    {
        lodGroupListArg.caption = $"LOD List";
        EditorUIUtils.ToggleFoldout(lodGroupListArg, arg =>
        {
            var lods = item.lodDetails;
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
            //if (GUILayout.Button("Update"))
            //{
            //    RemoveEditorArg(item.GetDoors());
            //    InitEditorArg(item.UpdateDoors());
            //}
        });
        if (lodGroupListArg.isEnabled && lodGroupListArg.isExpanded)
        {
            var lods = item.lodDetails;
            InitEditorArg(lods);
            lodGroupListArg.DrawPageToolbar(lods, (lodDetail, i) =>
            {
                var arg = editorArgs[lodDetail];
                if (lodDetail.group == null) return;
                arg.caption = $"[{i:00}] {lodDetail.group.name}";
                //arg.info = door.ToString();
                EditorUIUtils.ObjectFoldout(arg, lodDetail.group, () =>
                {
                    float lod0Vertex = 0;
                    for (int i = 0; i < lodDetail.childs.Count; i++)
                    {
                        var lodChild = lodDetail.childs[i];
                        float vertexF = lodChild.GetVertexCountF();
                        string vertexS = "";
                        if (vertexF > 100)
                        {
                            vertexS = vertexF.ToString("F0");
                        }
                        else if (vertexF > 10)
                        {
                            vertexS = vertexF.ToString("F1");
                        }
                        else
                        {
                            vertexS = vertexF.ToString("F2");
                        }
                        if (i == 0)
                        {
                            lod0Vertex = lodChild.vertexCount;
                        }
                        else
                        {

                        }

                        if (GUILayout.Button($"L{i}[{vertexS}][{lodChild.vertexCount / lod0Vertex:P0}][{lodChild.screenRelativeTransitionHeight}]", GUILayout.Width(140)))
                        {
                            //EditorHelper.SelectObject()
                            EditorHelper.SelectObjects(lodChild.renderers);
                        }
                    }
                });
            });
        }
    }
}
