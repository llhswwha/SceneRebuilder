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

    public void OnEnable()
    {
        //buildingListFoldout = serializedObject.FindProperty("buildingListFoldout");

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

    public override void OnToolLayout(SceneRebuildManager item)
    {
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

        // if (GUILayout.Button("InitModels", contentStyle))
        // {
        //     item.InitBuildings();
        // }

        // if (GUILayout.Button("2.CreateTrees", contentStyle))
        // {
        //     item.CombineBuildings();
        // }

        // if (GUILayout.Button("CreateScenes", contentStyle))
        // {
        //     item.SaveScenes();
        // }

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
        // if (GUILayout.Button("ClearBuildings", contentStyle))
        // {
        //     item.ClearBuildings();
        // }
        EditorUIUtils.SetupStyles();

        //-------------------------------------------------------BuildingList-----------------------------------------------------------
        //GUIStyle contentStyle = new GUIStyle(EditorStyles.miniButton);
        //contentStyle.alignment = TextAnchor.MiddleLeft;
        List<BuildingModelInfo> buildings=new List<BuildingModelInfo>();
        buildingListArg.caption=$"Building List";
        EditorUIUtils.ToggleFoldout(buildingListArg, 
        (arg)=>{
            float sumVertexCount=0;
            int sumRendererCount=0;

            buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true).ToList() ;
            buildings.Sort((a, b) =>
            {
                //return b.AllVertextCount.CompareTo(a.AllVertextCount);
                return b.Out0BigVertextCount.CompareTo(a.Out0BigVertextCount);
            });
            buildings.ForEach(b=>{
                sumVertexCount+=b.AllVertextCount;
                sumRendererCount+=b.AllRendererCount;
            });
            InitEditorArg(buildings);
            arg.caption= $"Building List({buildings.Count})";
            arg.info=$"[{sumVertexCount:F0}w][{sumRendererCount}]";
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
            buildingListArg.DrawPageToolbar(buildings.Count);
            for(int i=buildingListArg.GetStartId();i<buildings.Count && i<buildingListArg.GetEndId();i++)
            // foreach (var b in buildings)
            {
                var b=buildings[i];
                var arg = editorArgs[b];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, 
                $"[{i+1:00}] {b.name} {b.GetInfoText()}", 
                $"[{b.AllVertextCount:F0}w][{b.AllRendererCount}]|{b.Out0BigVertextCount:F0}+{b.Out0SmallVertextCount:F0}+{b.Out1VertextCount:F0}+{b.InVertextCount:F0}", 
                false,false,true,b.gameObject,
                ()=>{
                    
                    int width=25;
                    var contentStyle = new GUIStyle(EditorStyles.miniButton);
                    contentStyle.margin=new RectOffset(0,0,0,0);
                    contentStyle.padding=new RectOffset(0,0,0,0);
                   var state=b.GetState();

                    if(state.CanOneKey()){
                        if(GUILayout.Button("I",contentStyle, GUILayout.Width(20)))
                        {
                            b.InitInOut();
                        }
                    }
                    else{
                        if(GUILayout.Button("-",contentStyle,GUILayout.Width(20)))
                        {
                            // b.InitInOut();
                        }
                    }

                    if(state.CanOneKey()){
                        if(GUILayout.Button("+O",contentStyle,GUILayout.Width(width)))
                        {
                            b.CreateTreesBSEx();
                        }
                    }
                    else if(state.CanReset())
                    {
                        if(GUILayout.Button("-O",contentStyle,GUILayout.Width(width)))
                        {
                            b.EditorLoadNodeScenesEx();
                            b.DestroyScenes();
                            b.ClearTrees();
                        }
                    }
                    else{
                        if(GUILayout.Button("--",contentStyle,GUILayout.Width(width)))
                        {
                            //b.CreateTreesBSEx();
                        }
                    }

                    if(state.CanCreateTrees){
                        if(GUILayout.Button("+T",contentStyle,GUILayout.Width(width)))
                        {
                            b.CreateTreesBSEx();
                        }
                    }
                    else if(state.CanRemoveTrees)
                    {
                        if(GUILayout.Button("-T",contentStyle,GUILayout.Width(width)))
                        {
                            b.ClearTrees();
                        }
                    }
                    else{
                        if(GUILayout.Button("--",contentStyle,GUILayout.Width(width)))
                        {
                            //b.CreateTreesBSEx();
                        }
                    }
                    
                    if(state.CanCreateScenes)
                    {
                        if(GUILayout.Button("+S",contentStyle,GUILayout.Width(width)))
                        {
                            b.EditorCreateNodeScenes();
                        }
                    }
                    else if(state.CanRemoveScenes)
                    {
                        if(GUILayout.Button("-S",contentStyle,GUILayout.Width(width)))
                        {
                            b.DestroyScenes();
                        }
                    }
                    else{
                        if(GUILayout.Button("--",contentStyle,GUILayout.Width(width)))
                        {
                            
                        }
                    }

                    if(state.CanLoadScenes)
                    {
                        if(GUILayout.Button("+L",contentStyle,GUILayout.Width(width)))
                        {
                            b.EditorLoadNodeScenesEx();
                        }
                    }
                    else if(state.CanUnloadScenes)
                    {
                         if(GUILayout.Button("-L",contentStyle,GUILayout.Width(width)))
                        {
                            b.UnLoadScenes();
                        }
                    }
                    else{
                        if(GUILayout.Button("--",contentStyle,GUILayout.Width(width)))
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
        }

        //-------------------------------------------------------TreeList-----------------------------------------------------------
        List<ModelAreaTree> trees=new List<ModelAreaTree>();
        treeListArg.caption=$"Tree List";
        EditorUIUtils.ToggleFoldout(treeListArg, 
        (arg)=>{
            float sumVertexCount=0;
            int sumRendererCount=0;

            trees=GameObject.FindObjectsOfType<ModelAreaTree>(true).Where(t=>t.VertexCount>0).ToList() ;
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
        },
        ()=>{
            if(GUILayout.Button("------",GUILayout.Width(60)))
			{
				//TreeNodeManagerEditorWindow.ShowWindow();
			}
        });
        if (treeListArg.isExpanded && treeListArg.isEnabled)
        {
            treeListArg.DrawPageToolbar(trees.Count);
            for(int i=treeListArg.GetStartId();i<trees.Count && i<treeListArg.GetEndId();i++)
            {
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
        }

        //-------------------------------------------------------NodeList-----------------------------------------------------------
        List<AreaTreeNode> nodes=new List<AreaTreeNode>();
        nodeListArg.caption=$"Node List";
        EditorUIUtils.ToggleFoldout(nodeListArg, 
        (arg)=>{
            float sumVertexCount=0;
            int sumRendererCount=0;
            nodes=GameObject.FindObjectsOfType<AreaTreeNode>(true).Where(n=>n.IsLeaf).ToList() ;
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
        },
        ()=>{
            if(GUILayout.Button("Window",GUILayout.Width(60)))
			{
				TreeNodeManagerEditorWindow.ShowWindow();
			}
        });
        if (nodeListArg.isExpanded && nodeListArg.isEnabled)
        {
            nodeListArg.DrawPageToolbar(nodes.Count);
            for(int i=nodeListArg.GetStartId();i<nodes.Count && i<nodeListArg.GetEndId();i++)
            {
                var node=nodes[i];
                var arg = editorArgs[node];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i+1:00}] {node.tree.name}.{node.name}", $"[{node.VertexCount:F0}w][{node.Renderers.Count}]", false,false,false,node.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
        }

        //-------------------------------------------------------SceneList-----------------------------------------------------------
        List<SubScene_Base> scenes=new List<SubScene_Base>();
        sceneListArg.caption=$"Scene List";
        EditorUIUtils.ToggleFoldout(sceneListArg, (arg)=>{
            float sumVertexCount=0;
            int sumRendererCount=0;
            scenes=GameObject.FindObjectsOfType<SubScene_Base>(true).ToList() ;
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
        },()=>{
            if(GUILayout.Button("Window",GUILayout.Width(60)))
			{
				SubSceneManagerEditorWindow.ShowWindow();
			}
        });
        if (sceneListArg.isExpanded && sceneListArg.isEnabled)
        {
            sceneListArg.DrawPageToolbar(scenes.Count);
            for(int i=sceneListArg.GetStartId();i<scenes.Count && i<sceneListArg.GetEndId();i++)
            {
                var scene=scenes[i];
                var arg = editorArgs[scene];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i+1:00}] {scene.name}", $"[{scene.vertexCount:F0}w][{scene.rendererCount}]", false,false,false,scene.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
        }

        //-------------------------------------------------------MeshList-----------------------------------------------------------
        List<MeshFilter> meshes=new List<MeshFilter>();
        meshListArg.caption=$"Mesh List";
        EditorUIUtils.ToggleFoldout(meshListArg, (arg)=>{
            System.DateTime start=System.DateTime.Now;
            float sumVertexCount=0;
            float sumVertexCountVisible=0;
            int sumRendererCount=0;
            int sumRendererCountVisible=0;
            meshes=GameObject.FindObjectsOfType<MeshFilter>(true).Where(m=>m.sharedMesh!=null && m.sharedMesh.name!="Cube").ToList() ;
            meshes.Sort((a, b) =>
            {
                return b.sharedMesh.vertexCount.CompareTo(a.sharedMesh.vertexCount);
            });
            meshes.ForEach(b=>{
                if(b.GetComponent<MeshRenderer>()!=null && b.GetComponent<MeshRenderer>().enabled == true && b.gameObject.activeInHierarchy==true){
                    sumVertexCountVisible+=b.sharedMesh.vertexCount;
                    sumRendererCountVisible++;
                }
                sumVertexCount+=b.sharedMesh.vertexCount;
                sumRendererCount++;
            });
            sumVertexCount/=10000;
            sumVertexCountVisible/=10000;
            //InitEditorArg(scenes);
            arg.caption= $"Mesh List({meshes.Count})";
            arg.info=$"[{sumVertexCountVisible:F0}/{sumVertexCount:F0}w][{sumRendererCountVisible}/{sumRendererCount}]";
            var time=System.DateTime.Now-start;
            Debug.Log($"MeshList count:{meshes.Count} time:{time.TotalMilliseconds:F1}ms ");

        },()=>{
            if(GUILayout.Button("----------",GUILayout.Width(60)))
			{
				//SubSceneManagerEditorWindow.ShowWindow();
			}
        });

        if (meshListArg.isExpanded && meshListArg.isEnabled)
        {
            meshListArg.DrawPageToolbar(meshes.Count);
            for(int i=meshListArg.GetStartId();i<meshes.Count && i<meshListArg.GetEndId();i++)
            {
                var mesh=meshes[i];
                if(!editorArgs.ContainsKey(mesh)){
                    editorArgs.Add(mesh,new FoldoutEditorArg());
                }
                var arg = editorArgs[mesh];
                BuildingModelInfo building=mesh.GetComponentInParent<BuildingModelInfo>();
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"[{i+1:00}] {building.name}>>{mesh.transform.parent.name}>{mesh.name}", $"[{mesh.sharedMesh.vertexCount/10000f:F0}w]", false,false,false,mesh.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
        }
    }
}
