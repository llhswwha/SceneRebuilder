using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(BuildingModelInfo))]
public class BuildingModelInfoEditor : BaseEditor<BuildingModelInfo>
{
    public override void OnInspectorGUI()
    {
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        BuildingModelInfo info = target as BuildingModelInfo;
        int sceneCount=info.GetSceneCount();
        int unloadedSceneCount = info.SceneList.GetUnloadedScenes().Count;


        bool isAllLoaded=info.IsSceneLoaded();
        int treeCount=info.GetTreeCount();
        int partCount=info.GetPartCount();

        if(isAllLoaded==false && sceneCount>0 && unloadedSceneCount>0 && unloadedSceneCount<sceneCount)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("SelectUnload", contentStyle, GUILayout.Width(100)))
            {
                //info.SceneList.IsAllLoaded_Debug();

                var scenes = info.SceneList.GetUnloadedScenes();
                Debug.Log($"scenes:{scenes.Count}");
                //EditorHelper.SelectObject(element.rootObj);
                EditorHelper.SelectObjects(scenes);
            }
            if (GUILayout.Button("LoadUnloaded", contentStyle, GUILayout.Width(100)))
            {
                IdDictionary.InitInfos();
                info.LoadUnloadedScenes();
            }
            GUILayout.Label($"loaded:{isAllLoaded},part:{partCount} tree:{treeCount},scene:{sceneCount},unloaded:{unloadedSceneCount}");
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.BeginHorizontal();
        NewButton("1.GetInfo", buttonWidth, isAllLoaded == true || sceneCount==0 || treeCount==0, info.InitInOut);
        NewButton("FindDoors",90,partCount>0 && (isAllLoaded == true || sceneCount == 0 || treeCount == 0),info.FindInDoors);

        //if(GUILayout.Button("ShowMeshes",contentStyle,GUILayout.Width(90)))
        //{
        //    //info.FindDoors();
        //    MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.gameObject);
        //}
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("2.CreateTree",buttonWidth,isAllLoaded==true && partCount>0 && treeCount==0,info.CreateTreesBSEx);
        NewButton("RemoveTrees",buttonWidth,isAllLoaded==true && treeCount>0,info.ClearTrees);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("3.CreateScenes",buttonWidth,isAllLoaded==true && treeCount>0 && sceneCount==0,info.EditorCreateNodeScenes);
        NewButton("RemoveScenes",buttonWidth,isAllLoaded==true && sceneCount>0,info.DestroyScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //int unloadedSceneCount = info.SceneList.GetUnloadedScenes().Count;
        NewButton("4.LoadScenes",buttonWidth,isAllLoaded==false && sceneCount>0,()=>
        {
            info.EditorLoadNodeScenesEx();
            //info.LoadUnloadedScenes();
        });
        NewButton("UnloadScenes",buttonWidth,isAllLoaded==true && sceneCount>0,info.UnLoadScenes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NewButton("5.OneKey",buttonWidth,treeCount==0 && sceneCount==0,()=>{
            // info.InitInOut();
            // info.CreateTreesBSEx();
            // info.EditorCreateNodeScenes();
            info.OneKey_TreeNodeScene();

        });
        NewButton("Reset",buttonWidth,sceneCount>0 && treeCount>0,()=>{
            info.EditorLoadNodeScenesEx();
            info.DestroyScenes();
            info.ClearTrees();
        });
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("All",GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.gameObject);
        }
        GUILayout.Button(info.AllVertextCount.ToString("F1")+"w", GUILayout.Width(80));
        GUILayout.Button(info.AllRendererCount.ToString(), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Out0", GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart0.gameObject);
        }
        GUILayout.Button(info.Out0VertextCount.ToString("F1")+"w", GUILayout.Width(80));
        GUILayout.Button(info.Out0RendererCount.ToString(), GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("In", GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.InPart.gameObject);
        }
        GUILayout.Button(info.InVertextCount.ToString("F1")+"w", GUILayout.Width(80));
        GUILayout.Button(info.InRendererCount.ToString(), GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Out1", GUILayout.Width(50)))
        {
            MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart1.gameObject);
        }
        GUILayout.Button(info.Out1VertextCount.ToString("F1")+"w", GUILayout.Width(80));
        GUILayout.Button(info.Out1RendererCount.ToString(), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Out0B", GUILayout.Width(50)))
        {
            //MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart1.gameObject);
        }
        GUILayout.Button(info.Out0BigVertextCount.ToString("F1")+"w", GUILayout.Width(80));
        GUILayout.Button(info.Out0BigRendererCount.ToString(), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Out0S", GUILayout.Width(50)))
        {
            //MeshProfilerNS.GameObjectListMeshEditorWindow.ShowWindow(info.OutPart1.gameObject);
        }
        GUILayout.Button(info.Out0SmallVertextCount.ToString("F1")+"w", GUILayout.Width(80));
        GUILayout.Button(info.Out0SmallRendererCount.ToString(), GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
