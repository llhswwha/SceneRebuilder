using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Linq;

[CustomEditor(typeof(SceneRebuildManager))]
public class SceneRebuildManagerEditor : BaseEditor<SceneRebuildManager>
{
    //private SerializedProperty buildingListFoldout;

    private FoldoutEditorArg buildingListArg=new FoldoutEditorArg();
    private FoldoutEditorArg treeListArg=new FoldoutEditorArg();

    private FoldoutEditorArg nodeListArg=new FoldoutEditorArg();

    private FoldoutEditorArg sceneListArg=new FoldoutEditorArg();

    private FoldoutEditorArg meshListArg=new FoldoutEditorArg();

    private SceneRebuildManager manager;

    private List<MeshFilter> meshFilters = new List<MeshFilter>();

    public void OnEnable()
    {
        manager = target as SceneRebuildManager;
        UpdateList();
        Debug.LogError("SceneRebuildManagerEditor.OnEnable");
    }

    public void UpdateList()
    {
        manager.UpdateList();
        InitMeshList(meshListArg);
    }

    public Dictionary<Object, FoldoutEditorArg> editorArgs = new Dictionary<Object, FoldoutEditorArg>();

    public void InitEditorArg<T>(List<T> items) where T : Object
    {
        foreach(var item in items)
        {
            if (!editorArgs.ContainsKey(item))
            {
                editorArgs.Add(item, new FoldoutEditorArg());
            }
            else{
                if(editorArgs[item]==null){
                    editorArgs[item]=new FoldoutEditorArg();
                }
            }
        }
    }

    public bool IsShowList = true;

    public override void OnToolLayout(SceneRebuildManager item)
    {
        Debug.Log($"------------------------------------------------------------------------");

        System.DateTime startT=System.DateTime.Now;

        base.OnToolLayout(item);

        // int count = buildings.Count;
        // if (GUILayout.Button("Count:"+count, contentStyle))
        // {
        // }
        EditorGUILayout.BeginHorizontal();
        NewButton("1.InitModels", buttonWidth, true, item.InitBuildings);
        // NewButton("ListWindow:" + count, buttonWidth, true, () =>
        //      {
        //          SceneRebuildEditorWindow.ShowWindow();
        //      });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateTrees", buttonWidth, true, item.CombineBuildings);
        NewButton("RemoveTrees", buttonWidth, true, item.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes", buttonWidth, true, item.SaveScenes);
        NewButton("LoadScenes", buttonWidth, true, item.LoadScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("4.SetBuildings", buttonWidth, true, item.SetBuildings);
        NewButton("ClearBuildings", buttonWidth, true, item.ClearBuildings);
        EditorGUILayout.EndHorizontal();

        // if (GUILayout.Button("SetBuildings", contentStyle))
        // {
        //     item.SetBuildings();
        // }
        if (GUILayout.Button("OneKey", contentStyle))
        {
            item.OneKey();
        }

        IsShowList = GUILayout.Toggle(IsShowList, "IsShowList");
        if (IsShowList == false) return;

        EditorUIUtils.SetupStyles();
        //-------------------------------------------------------BuildingList-----------------------------------------------------------
        //GUIStyle contentStyle = new GUIStyle(EditorStyles.miniButton);
        //contentStyle.alignment = TextAnchor.MiddleLeft;
        List<BuildingModelInfo> buildings=new List<BuildingModelInfo>();
        buildingListArg.caption=$"Building List";
        EditorUIUtils.ToggleFoldout(buildingListArg, 
        (arg)=>{
            System.DateTime start=System.DateTime.Now;
            float sumVertexCount=0;
            int sumRendererCount=0;

            //var ts= GameObject.FindObjectsOfType<Transform>(true);
            //Debug.Log($"Init BuildingList00 count:{ts.Length} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");

            //var bs = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
            //Debug.Log($"Init BuildingList0 count:{bs.Length} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");

            //buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList() ;

            buildings = item.GetBuildings().Where(b=>b!=null).ToList();

            //Debug.Log($"Init BuildingList1 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            buildings.Sort((a, b) =>
            {
                //return b.AllVertextCount.CompareTo(a.AllVertextCount);
                return b.Out0BigVertextCount.CompareTo(a.Out0BigVertextCount);
            });
            //Debug.Log($"Init BuildingList2 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            buildings.ForEach(b=>{
                sumVertexCount+=b.AllVertextCount;
                sumRendererCount+=b.AllRendererCount;
            });
            //Debug.Log($"Init BuildingList3 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            InitEditorArg(buildings);
            //Debug.Log($"Init BuildingList4 count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
            arg.caption= $"Building List({buildings.Count})";
            arg.info=$"[{sumVertexCount:F0}w][{sumRendererCount}]";
            Debug.Log($"Init BuildingList count:{buildings.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        },
        //"Window",
        ()=>{
            //SceneRebuildEditorWindow.ShowWindow();

            if(GUILayout.Button("Window",GUILayout.Width(60)))
			{
				SceneRebuildEditorWindow.ShowWindow();
			}
        });

        if (buildingListArg.isExpanded && buildingListArg.isEnabled)
        {
            System.DateTime start=System.DateTime.Now;
            buildingListArg.DrawPageToolbar(buildings.Count);
            int c=0;
            for(int i=buildingListArg.GetStartId();i<buildings.Count && i<buildingListArg.GetEndId();i++)
            {
                c++;
                var b=buildings[i];
                var arg = editorArgs[b];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded,
                $"[{i+1:00}] {b.name} {b.GetInfoText()}",
                //$"[{i + 1:00}] {b.name} ",
                $"[{b.AllVertextCount:F0}w][{b.AllRendererCount}]|{b.Out0BigVertextCount:F0}+{b.Out0SmallVertextCount:F0}+{b.Out1VertextCount:F0}+{b.InVertextCount:F0}",
                false, false, true, b.gameObject,
                () =>
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
                );
                if (arg.isExpanded)
                {
                    BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time=System.DateTime.Now-start;
            Debug.Log($"Show BuildingList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }

        //-------------------------------------------------------TreeList-----------------------------------------------------------
        List<ModelAreaTree> trees=new List<ModelAreaTree>();
        treeListArg.caption=$"Tree List";
        EditorUIUtils.ToggleFoldout(treeListArg, 
        (arg)=>{
            System.DateTime start=System.DateTime.Now;
            float sumVertexCount=0;
            int sumRendererCount=0;

            //trees=GameObject.FindObjectsOfType<ModelAreaTree>(true).Where(t=>t.VertexCount>0).ToList() ;
            trees = item.GetTrees().Where(t => t.VertexCount > 0).ToList();

            trees.Sort((a, b) =>
            {
                return b.VertexCount.CompareTo(a.VertexCount);
            });
            trees.ForEach(b=>{
                sumVertexCount+=b.VertexCount;
                sumRendererCount+=b.GetRendererCount();
            });
            InitEditorArg(trees);

            arg.caption= $"Tree List({trees.Count})";
            arg.info=$"[{sumVertexCount:F0}w][{sumRendererCount}]";
            var time=System.DateTime.Now-start;
            Debug.Log($"Init TreeList count:{trees.Count} time:{time.TotalMilliseconds:F1}ms ");
        },
        ()=>{
            if(GUILayout.Button("------",GUILayout.Width(60)))
			{
				//TreeNodeManagerEditorWindow.ShowWindow();
			}
        });
        if (treeListArg.isExpanded && treeListArg.isEnabled)
        {
            System.DateTime start=System.DateTime.Now;
            treeListArg.DrawPageToolbar(trees.Count);
            int c=0;
            for(int i=treeListArg.GetStartId();i<trees.Count && i<treeListArg.GetEndId();i++)
            {
                c++;
                var tree=trees[i];
                // if(tree==null)continue;
                var arg = editorArgs[tree];
                if(arg==null){
                    editorArgs[tree]=new FoldoutEditorArg();
                }
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i+1:00}] {tree.name}", $"[{tree.VertexCount}w][{tree.GetRendererCount()}][{tree.TreeLeafs.Count}]", false,false,false,tree.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time=System.DateTime.Now-start;
            Debug.Log($"Show TreeList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }

        //-------------------------------------------------------NodeList-----------------------------------------------------------
        List<AreaTreeNode> nodes=new List<AreaTreeNode>();
        nodeListArg.caption=$"Node List";
        EditorUIUtils.ToggleFoldout(nodeListArg, 
        (arg)=>{
            System.DateTime start=System.DateTime.Now;
            float sumVertexCount=0;
            int sumRendererCount=0;
            nodes = item.GetLeafNodes();

            nodes.Sort((a, b) =>
            {
                return b.VertexCount.CompareTo(a.VertexCount);
            });
            nodes.ForEach(b=>{
                sumVertexCount+=b.VertexCount;
                sumRendererCount+=b.Renderers.Count;
            });
            InitEditorArg(nodes);
            arg.caption= $"Node List({nodes.Count})";
            arg.info=$"[{sumVertexCount:F0}w][{sumRendererCount}]";
            var time=System.DateTime.Now-start;
            Debug.Log($"Init NodeList count:{nodes.Count} time:{time.TotalMilliseconds:F1}ms ");
        },
        ()=>{
            if(GUILayout.Button("Window",GUILayout.Width(60)))
			{
				TreeNodeManagerEditorWindow.ShowWindow();
			}
        });
        if (nodeListArg.isExpanded && nodeListArg.isEnabled)
        {
            System.DateTime start=System.DateTime.Now;
            nodeListArg.DrawPageToolbar(nodes.Count);
            int c=0;
            for(int i=nodeListArg.GetStartId();i<nodes.Count && i<nodeListArg.GetEndId();i++)
            {
                c++;
                var node=nodes[i];
                var arg = editorArgs[node];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i+1:00}] {node.tree.name}.{node.name}", $"[{node.VertexCount:F0}w][{node.Renderers.Count}]", false,false,false,node.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time=System.DateTime.Now-start;
            Debug.Log($"Show NodeList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }

        //-------------------------------------------------------SceneList-----------------------------------------------------------
        List<SubScene_Base> scenes=new List<SubScene_Base>();
        sceneListArg.caption=$"Scene List";
        EditorUIUtils.ToggleFoldout(sceneListArg, (arg)=>{
            System.DateTime start=System.DateTime.Now;
            float sumVertexCount=0;
            int sumRendererCount=0;
            scenes = item.GetScenes();
            scenes.Sort((a, b) =>
            {
                return b.vertexCount.CompareTo(a.vertexCount);
            });
            scenes.ForEach(b=>{
                sumVertexCount+=b.vertexCount;
                sumRendererCount+=b.rendererCount;
            });
            InitEditorArg(scenes);
            arg.caption= $"Scene List({scenes.Count})";
            arg.info=$"[{sumVertexCount:F0}w][{sumRendererCount}]";
            var time=System.DateTime.Now-start;
            Debug.Log($"Init SceneList count:{scenes.Count} time:{time.TotalMilliseconds:F1}ms ");
        },()=>{
            if(GUILayout.Button("Window",GUILayout.Width(60)))
			{
				SubSceneManagerEditorWindow.ShowWindow();
			}
        });
        if (sceneListArg.isExpanded && sceneListArg.isEnabled)
        {
            System.DateTime start=System.DateTime.Now;
            sceneListArg.DrawPageToolbar(scenes.Count);
            int c=0;
            for(int i=sceneListArg.GetStartId();i<scenes.Count && i<sceneListArg.GetEndId();i++)
            {
                c++;
                var scene=scenes[i];
                var arg = editorArgs[scene];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i+1:00}] {scene.name}", $"[{scene.vertexCount:F0}w][{scene.rendererCount}]", false,false,false,scene.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time=System.DateTime.Now-start;
            Debug.Log($"Show SceneList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }

        //-------------------------------------------------------MeshList-----------------------------------------------------------
        //List<MeshFilter> meshes=new List<MeshFilter>();
        if(string.IsNullOrEmpty(meshListArg.caption)) meshListArg.caption=$"Mesh List";
        EditorUIUtils.ToggleFoldout(meshListArg, (arg)=>{

            if (meshFilters.Count == 0)
            {
                InitMeshList(arg);
            }
        },()=>{
            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                InitMeshList(meshListArg);
            }
            if (GUILayout.Button("Window",GUILayout.Width(60)))
			{
                //SubSceneManagerEditorWindow.ShowWindow();
                MeshProfilerNS.MeshProfiler.ShowWindow();
			}
        });

        if (meshListArg.isExpanded && meshListArg.isEnabled)
        {
            System.DateTime start=System.DateTime.Now;
            meshListArg.DrawPageToolbar(meshFilters.Count);
            int c=0;
            for(int i=meshListArg.GetStartId();i<meshFilters.Count && i<meshListArg.GetEndId();i++)
            {
                c++;
                var mesh=meshFilters[i];
                if(!editorArgs.ContainsKey(mesh)){
                    editorArgs.Add(mesh,new FoldoutEditorArg());
                }
                var arg = editorArgs[mesh];
                BuildingModelInfo[] bs=mesh.GetComponentsInParent<BuildingModelInfo>(true);
                if(bs.Length==0){
                    Debug.LogError($"Show MeshList buildings.Length==0");continue;
                }
                BuildingModelInfo building=bs[0];
                if(mesh==null){
                    Debug.LogError($"Show MeshList mesh==null");continue;
                }
                if(mesh.sharedMesh==null){
                    Debug.LogError($"Show MeshList mesh.sharedMesh==null");continue;
                }
                if(arg==null){
                    Debug.LogError($"Show MeshList arg==null");continue;
                }
                if(building==null){
                    Debug.LogError($"Show MeshList building==null");continue;
                }
                if(mesh.transform.parent==null){
                    Debug.LogError($"Show MeshList mesh.transform.parent==null");continue;
                }
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i+1:00}] {building.name}>>{mesh.transform.parent.name}>{mesh.name}", $"[{mesh.sharedMesh.vertexCount/10000f:F0}w]", false,false,false,mesh.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
            var time=System.DateTime.Now-start;
            Debug.Log($"Show MeshList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }

        var timeT=System.DateTime.Now-startT;
        Debug.Log($"SceneRebuildManagerEditor time:{timeT.TotalMilliseconds:F1}ms ");
    }

    private void InitMeshList(FoldoutEditorArg arg)
    {
        System.DateTime start = System.DateTime.Now;
        float sumVertexCount = 0;
        float sumVertexCountVisible = 0;
        int sumRendererCount = 0;
        int sumRendererCountVisible = 0;
        meshFilters = GameObject.FindObjectsOfType<MeshFilter>(true).Where(m => m != null && m.sharedMesh != null && m.sharedMesh.name != "Cube").ToList();
        meshFilters.Sort((a, b) =>
        {
            return b.sharedMesh.vertexCount.CompareTo(a.sharedMesh.vertexCount);
        });
        Debug.Log($"Init MeshList1 count:{meshFilters.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        meshFilters.ForEach(b => {
            if (b == null) return;
            if (b.gameObject.activeInHierarchy == true && b.GetComponent<MeshRenderer>() != null && b.GetComponent<MeshRenderer>().enabled == true)
            {
                sumVertexCountVisible += b.sharedMesh.vertexCount;
                sumRendererCountVisible++;
            }
            sumVertexCount += b.sharedMesh.vertexCount;
            sumRendererCount++;
        });
        Debug.Log($"Init MeshList2 count:{meshFilters.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
        sumVertexCount /= 10000;
        sumVertexCountVisible /= 10000;
        //InitEditorArg(scenes);
        arg.caption = $"Mesh List({meshFilters.Count})";
        arg.info = $"[{sumVertexCountVisible:F0}/{sumVertexCount:F0}w][{sumRendererCountVisible}/{sumRendererCount}]";
        Debug.Log($"Init MeshList count:{meshFilters.Count} time:{(System.DateTime.Now - start).TotalMilliseconds:F1}ms ");
    }
}
