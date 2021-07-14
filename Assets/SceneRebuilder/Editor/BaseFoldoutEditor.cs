using CodeStage.AdvancedFPSCounter.Editor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseFoldoutEditor<T> : BaseEditor<T> where T : class
{
    public Dictionary<Object, FoldoutEditorArg> editorArgs = new Dictionary<Object, FoldoutEditorArg>();

    public void InitEditorArg<T>(List<T> items) where T : Object
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

    public void DrawSceneList(FoldoutEditorArg foldoutArg,System.Func<List<SubScene_Base>> funcGetScenes)
    {
        List<SubScene_Base> scenes = new List<SubScene_Base>();
        foldoutArg.caption = $"Scenes";
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) => {
            System.DateTime start = System.DateTime.Now;
            //scenes = item.scenes.ToList();
            scenes = funcGetScenes();
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
            if (GUILayout.Button("Window", GUILayout.Width(60)))
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
}
