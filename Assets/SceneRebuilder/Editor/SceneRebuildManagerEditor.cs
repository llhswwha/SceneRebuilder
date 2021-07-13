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

    private FoldoutEditorArg buildingListFoldoutEditorArg=new FoldoutEditorArg();
    private FoldoutEditorArg treeListFoldoutEditorArg=new FoldoutEditorArg();

    private bool isNodeListExpanded=false;

    private bool isSceneListExpanded=false;

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
        buildingListFoldoutEditorArg.caption=$"Building List";
        EditorUIUtils.ToggleFoldout(buildingListFoldoutEditorArg, 
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

        if (buildingListFoldoutEditorArg.isExpanded)
        {
            foreach (var b in buildings)
            {
                var arg = editorArgs[b];
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, 
                $"{b.name} {b.GetInfoText()}", 
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


        treeListFoldoutEditorArg.caption=$"Tree List";
        EditorUIUtils.ToggleFoldout(treeListFoldoutEditorArg, 
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
            
        });
        if (treeListFoldoutEditorArg.isExpanded&&treeListFoldoutEditorArg.isEnabled)
        {
            foreach (var tree in trees)
            {
                if(tree==null)continue;
                var arg = editorArgs[tree];
                if(arg==null){
                    editorArgs[tree]=new FoldoutEditorArg();
                }
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"{tree.name}", $"[{tree.VertexCount}w][{tree.GetRendererCount()}][{tree.TreeLeafs.Count}]", false,false,false,tree.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
        }

        //-------------------------------------------------------NodeList-----------------------------------------------------------
        var nodes=GameObject.FindObjectsOfType<AreaTreeNode>(true).Where(n=>n.IsLeaf).ToList() ;
        nodes.Sort((a, b) =>
        {
            return b.VertexCount.CompareTo(a.VertexCount);
        });
        InitEditorArg(nodes);
        isNodeListExpanded=EditorUIUtils.ButtonFoldout(isNodeListExpanded, $"Node List({nodes.Count})","",true,true,true,"Window",()=>{
            TreeNodeManagerEditorWindow.ShowWindow();
        });
        if (isNodeListExpanded)
        {
            foreach (var node in nodes)
            {
                var arg = editorArgs[node];
                //arg.isExpanded = EditorUIUtils.Foldout(arg.isExpanded, $"{b.name} \t[{b.AllVertextCount:F0}w][{b.AllRendererCount}]");
                //if (arg.isExpanded)
                //{

                //}
                // var vCount=node.GetVertexCount();
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"{node.name}", $"[{node.VertexCount:F0}w][{node.Renderers.Count}]", false,false,false,node.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
        }

        //-------------------------------------------------------SceneList-----------------------------------------------------------
        var scenes=GameObject.FindObjectsOfType<SubScene_Base>(true).ToList() ;
        scenes.Sort((a, b) =>
        {
            return b.vertexCount.CompareTo(a.vertexCount);
        });
        InitEditorArg(scenes);
        isSceneListExpanded=EditorUIUtils.ButtonFoldout(isSceneListExpanded, $"Scene List({scenes.Count})","",true,true,true,"Window",()=>{
            SubSceneManagerEditorWindow.ShowWindow();
        });
        if (isSceneListExpanded)
        {
            foreach (var scene in scenes)
            {
                var arg = editorArgs[scene];
                //arg.isExpanded = EditorUIUtils.Foldout(arg.isExpanded, $"{b.name} \t[{b.AllVertextCount:F0}w][{b.AllRendererCount}]");
                //if (arg.isExpanded)
                //{

                //}
                // var vCount=node.GetVertexCount();
                arg.isExpanded = EditorUIUtils.ObjectFoldout(arg.isExpanded, $"{scene.name}", $"[{scene.vertexCount:F0}w][{scene.rendererCount}]", false,false,false,scene.gameObject);
                if (arg.isExpanded)
                {
                    //BuildingModelInfoEditor.DrawToolbar(b, contentStyle, buttonWidth);
                }
            }
        }
    }
}
