using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LODGroupInfo))]
public class LODGroupInfoEditor : BaseFoldoutEditor<LODGroupInfo>
{
    FoldoutEditorArg lodListArg = new FoldoutEditorArg();

    public override void OnEnable()
    {
        base.OnEnable();
        targetT.GetLODs();
        targetT.CheckLOD0Scenes();

        lodListArg = new FoldoutEditorArg(true,true,true,true,true);
    }

    public override void OnToolLayout(LODGroupInfo item)
    {
        base.OnToolLayout(item);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GetLODs"))
        {
            item.GetLODs();
        }
        if (GUILayout.Button("SetLODs"))
        {
            item.SetLODs();
        }
        if (GUILayout.Button("AddLODN"))
        {
            item.AddLODN();
        }
        if (GUILayout.Button("AddBox"))
        {
            item.AddBox();
        }
        if (GUILayout.Button("SetMats"))
        {
            item.SetMaterial();
        }
        if (GUILayout.Button("Rename"))
        {
            item.Rename();
        }
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.ObjectField(item.scene, typeof(SubScene_Base));
        if (item.scene == null)
        {
            GUILayout.Label($"Scene:NULL");
        }
        else
        {
            GUILayout.Label($"Scene:{item.scene.sceneName}");
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Uniform"))
        {
            //item.GetLODs();
            LODHelper.UniformLOD0(item.LODGroup);
        }
        //if (GUILayout.Button("LOD1->LOD0"))
        //{
        //    LODHelper.LOD1ToLOD0(item.LODGroup);
        //}


        if (Application.isPlaying)
        {
            if (GUILayout.Button("LoadScene"))
            {
                item.LoadScene();
            }
            if (GUILayout.Button("UnloadScene"))
            {

            }
        }
        else
        {
            //if (GUILayout.Button("CreateScene"))
            //{
            //    item.EditorCreateScene();
            //}
            //if (GUILayout.Button("LoadScene"))
            //{
            //    item.EditorLoadScene();
            //}
            //if (GUILayout.Button("UnloadScene"))
            //{
            //    item.UnloadScene();
            //}
            NewEnabledButton("CreateScene", buttonWidth, item.IsSceneCreatable(), contentStyle, ()=>
            {
                item.EditorCreateScene();
                EditorHelper.ClearOtherScenes();
                EditorHelper.RefreshAssets();
            });
            if (item == null)
            {
                return;
            }
            NewEnabledButton("LoadScene", buttonWidth, item.IsSceneCreatable() == false, contentStyle, item.EditorLoadScene);
            NewEnabledButton("UnloadScene", buttonWidth, item.IsSceneCreatable(), contentStyle, item.UnloadScene);
        }
 
        if (GUILayout.Button("SelectScene"))
        {
            item.SetLODs();
        }
        EditorGUILayout.EndHorizontal();

        lodListArg.caption = $"LOD List";
        EditorUIUtils.ToggleFoldout(lodListArg, arg =>
        {
            var lods = item.LodInfos;
            arg.caption = $"LOD List ({lods.Count})";
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
        if (lodListArg.isEnabled && lodListArg.isExpanded)
        {
            var lods = item.LodInfos;
            InitEditorArg(lods);

            //lodListArg.DrawPageToolbar(lods, (lod, i) =>
            //{
            //    var arg = editorArgs[lod];
            //    arg.caption = $"[{i:00}] {lod.GetName()}";
            //    arg.info = $"{lod.vertextCount}";
            //    var renderer = lod.GetRenderer();
            //    EditorUIUtils.ObjectFoldout(arg, renderer, () =>
            //    {
            //        lod.screenRelativeTransitionHeight = EditorGUILayout.FloatField(lod.screenRelativeTransitionHeight, GUILayout.Width(40));
            //    });
            //});

            for (int i = 0; i < lods.Count; i++)
            {
                LODInfo lod = lods[i];
                var arg = editorArgs[lod];
                arg.caption = $"[LOD_{i:00}] {lod.GetName()}";
                arg.info = $"{MeshHelper.GetVertexCountS(lod.vertextCount)}";
                var renderer = lod.GetRenderer();
                EditorUIUtils.ObjectFoldout(arg, renderer, () =>
                {
                    lod.screenRelativeTransitionHeight = EditorGUILayout.FloatField(lod.screenRelativeTransitionHeight, GUILayout.Width(40));
                });
            }

            
        }
    }
}
