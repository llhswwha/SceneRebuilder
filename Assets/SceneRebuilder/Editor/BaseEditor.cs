using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using System;
using CodeStage.AdvancedFPSCounter.Editor.UI;

[CanEditMultipleObjects]
public class BaseEditor<T> : Editor where T:class
{
    protected FoldoutEditorArg toolbarArg = new FoldoutEditorArg();
    protected FoldoutEditorArg propertyArg = new FoldoutEditorArg();

    protected GUIStyle contentStyle;
    protected int buttonWidth=110;

    public void NewButton(string text, int width, bool isEnable, System.Action clickEvent)
    {
        NewEnabledButton(text, width, isEnable, contentStyle, clickEvent);
    }

    public void NewButton(string text,  bool isEnable, System.Action clickEvent)
    {
        NewEnabledButton(text, isEnable, clickEvent);
    }

    public static bool NewEnabledButton(string text,int width,bool isEnable, GUIStyle style, System.Action clickEvent)
    {
        EditorGUI.BeginDisabledGroup(!isEnable || clickEvent == null);
        bool isClick = GUILayout.Button(text, style, GUILayout.Width(width));
        if (isClick)
        {
            if (clickEvent != null)
            {
                clickEvent();
            }
        }
        EditorGUI.EndDisabledGroup();
        return isClick;
    }

    public static bool NewEnabledButton(string text, bool isEnable, GUIStyle style, System.Action clickEvent)
    {
        EditorGUI.BeginDisabledGroup(!isEnable || clickEvent == null);
        bool isClick = GUILayout.Button(text, style);
        if (isClick)
        {
            if (clickEvent != null)
            {
                clickEvent();
            }
        }
        EditorGUI.EndDisabledGroup();
        return isClick;
    }

    public static bool NewEnabledButton(string text, bool isEnable, System.Action clickEvent)
    {
        EditorGUI.BeginDisabledGroup(!isEnable || clickEvent == null);
        bool isClick = GUILayout.Button(text);
        if (isClick)
        {
            if (clickEvent != null)
            {
                clickEvent();
            }
        }
        EditorGUI.EndDisabledGroup();
        return isClick;
    }

    protected T targetT ;

    public virtual void OnEnable()
    {
        targetT = target as T;

        toolbarArg.caption = "ToolBar";
        toolbarArg.isEnabled = true;
        toolbarArg.isToggle = false;
        toolbarArg.isExpanded = true;
        toolbarArg.separator = false;
        toolbarArg.background = true;
        toolbarArg.bold = true;

        propertyArg.caption = "Properties";
        propertyArg.isEnabled = true;
        propertyArg.isToggle = false;
        propertyArg.separator = true;
        propertyArg.background = true;
        propertyArg.bold = true;
    }

    public override void OnInspectorGUI()
    {
        contentStyle = new GUIStyle(EditorStyles.miniButton);
        contentStyle.alignment = TextAnchor.MiddleLeft;

        EditorUIUtils.SetupStyles();

        EditorUIUtils.ToggleFoldout(toolbarArg, null,null);
        if (toolbarArg.isExpanded && toolbarArg.isEnabled)
        {
            OnToolLayout(targetT);
        }

        EditorUIUtils.ToggleFoldout(propertyArg, null, null);
        if (propertyArg.isExpanded)
        {
            base.OnInspectorGUI();
        }
    }

    public virtual void OnToolLayout(T item)
    {

    }


    public TO ObjectField<TO>(TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }

    public static TO ObjectFieldS<TO>(TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }

    public TO ObjectField<TO>(string label,TO obj) where TO : UnityEngine.Object
    {
        //GUILayout.Label(label);
        if (GUILayout.Button(label))
        {
            EditorHelper.SelectObject(obj);
        }
        return EditorGUILayout.ObjectField(obj, typeof(TO), true) as TO;
    }

    public TO ObjectFieldH<TO>(string label, TO obj) where TO : UnityEngine.Object
    {
        GUILayout.BeginHorizontal();
        //GUILayout.Label(label);
        if (GUILayout.Button(label))
        {
            EditorHelper.SelectObject(obj);
        }
        TO go= EditorGUILayout.ObjectField(obj, typeof(TO), true) as TO;
        GUILayout.EndHorizontal();
        return go;
    }

    public static TO ObjectFieldS<TO>(string label, TO obj) where TO : UnityEngine.Object
    {
        //GUILayout.Label(label);
        if (GUILayout.Button(label))
        {
            EditorHelper.SelectObject(obj);
        }
        return EditorGUILayout.ObjectField(obj, typeof(TO), true) as TO;
    }
}

public static class BaseEditorHelper
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

    public static void InitEditorArgEx<T2, TA>(List<T2> items) where TA : FoldoutEditorArg, new()
    {
        FoldoutEditorArgBuffer.InitEditorArg<T2, TA>(items);
    }

    public static FoldoutEditorArg GetEditorArg<T2, TA>(T2 item, TA newArg) where TA : FoldoutEditorArg, new()
    {
        return FoldoutEditorArgBuffer.GetEditorArg<T2, TA>(item, newArg);
    }

    //public static Dictionary<System.Object, FoldoutEditorArg> editorArgs_global = new Dictionary<System.Object, FoldoutEditorArg>();

    public static FoldoutEditorArg GetGlobalEditorArg<T2, TA>(T2 item, TA newArg) where TA : FoldoutEditorArg, new()
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

        return FoldoutEditorArgBuffer.GetGlobalEditorArg<T2, TA>(item, newArg);
    }

    public static void DrawObjectList<T1>(FoldoutEditorArg foldoutArg, string title, List<T1> list,
    System.Action<FoldoutEditorArg, T1, int> drawItemAction, System.Action<T1> itemToolBarAction, System.Action<FoldoutEditorArg, T1, int> drawSubListAction)
    {
        if (list == null) return;
        //if (list.Count == 0) return;
        //List<T1> list = new List<T1>();
        foldoutArg.caption = title;
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) =>
        {
            System.DateTime start = System.DateTime.Now;
            //scenes = item.scenes.ToList();
            //list = funcGetList();
            InitEditorArg(list);
            arg.caption = $"{title}({list.Count})";
        }, () =>
        {
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                list.Clear();
            }
            if (GUILayout.Button("DestroyGo", GUILayout.Width(80)))
            {
                foreach (var item in list)
                {
                    if (item == null) continue;
                    if (item is GameObject)
                    {
                        GameObject.DestroyImmediate(item as GameObject);
                    }
                    else if (item is Component)
                    {
                        Component c = (item as Component);
                        if (c == null) continue;
                        GameObject go = c.gameObject;
                        if (go == null) continue;
                        EditorHelper.UnpackPrefab(go);
                        GameObject.DestroyImmediate(go);
                    }
                }
                list.Clear();
            }
        });
        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(list.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < list.Count && i < foldoutArg.GetEndId(); i++)
            {
                try
                {
                    c++;
                    var item = list[i];
                    if (item == null) return;
                    FoldoutEditorArg arg = FoldoutEditorArgBuffer.editorArgs[item];
                    arg.level = 1;
                    arg.caption = $"[{i:00}] {item.ToString()}";
                    arg.isFoldout = false;
                    arg.isEnabled = true;

                    Object obj = arg.tag as Object;
                    if (item is Object)
                    {
                        obj = item as Object;
                        if (obj != null)
                        {
                            arg.caption = obj.name;
                        }
                        else
                        {
                            arg.caption = "NULL";
                        }
                    }

                    if (drawItemAction != null)
                    {
                        drawItemAction(arg, item, i);
                    }
                    else
                    {
                        EditorUIUtils.ObjectFoldout(arg, obj, () =>
                        {
                            if (itemToolBarAction != null)
                            {
                                itemToolBarAction(item);
                            }
                        });
                    }
                    if (arg.isEnabled && arg.isExpanded)
                    {
                        if (drawSubListAction != null)
                        {
                            drawSubListAction(arg, item, i);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"DrawObjectList Exception:{ex} ");
                }
            }
            var time = System.DateTime.Now - start;
            //Debug.Log($"Show SceneList count:{c} time:{time.TotalMilliseconds:F1}ms ");
        }
    }

    public static void DrawItemList<T1>(FoldoutEditorArg foldoutArg, string title, List<T1> list, System.Action toolBarAction,
        System.Action<FoldoutEditorArg, T1, int> drawItemAction, System.Action<T1> itemToolBarAction, System.Action<FoldoutEditorArg, T1, int> drawSubListAction)
    {
        if (list == null) return;
        //List<T1> list = new List<T1>();
        foldoutArg.caption = title;
        EditorUIUtils.ToggleFoldout(foldoutArg, (arg) =>
        {
            System.DateTime start = System.DateTime.Now;
            //scenes = item.scenes.ToList();
            //list = funcGetList();
            InitEditorArg(list);
            arg.caption = $"{title}({list.Count})";
        }, toolBarAction);
        if (foldoutArg.isExpanded && foldoutArg.isEnabled)
        {
            System.DateTime start = System.DateTime.Now;
            foldoutArg.DrawPageToolbar(list.Count);
            int c = 0;
            for (int i = foldoutArg.GetStartId(); i < list.Count && i < foldoutArg.GetEndId(); i++)
            {
                c++;
                var item = list[i];
                FoldoutEditorArg arg = FoldoutEditorArgBuffer.editorArgs[item];
                arg.level = 1;
                arg.caption = $"[{i:00}] {item.ToString()}";
                arg.isFoldout = false;
                arg.isEnabled = true;

                Object obj = arg.tag as Object;
                if (item is Object)
                {
                    obj = item as Object;
                    if (obj != null)
                    {
                        arg.caption = obj.name;
                    }
                    else
                    {
                        arg.caption = "NULL";
                    }
                }

                if (drawItemAction != null)
                {
                    drawItemAction(arg, item, i);
                }
                else
                {
                    EditorUIUtils.ObjectFoldout(arg, obj, () =>
                    {
                        if (itemToolBarAction != null)
                        {
                            itemToolBarAction(item);
                        }
                    });
                }
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

    public static void DrawObjectList<T1>(FoldoutEditorArg foldoutArg, string title, System.Func<List<T1>> funcGetList,
        System.Action<FoldoutEditorArg, T1, int> drawItemAction, System.Action<T1> toolBarAction, System.Action<FoldoutEditorArg, T1, int> drawSubListAction)
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

    public static TO ObjectFieldH<TO>(string label, TO obj) where TO : UnityEngine.Object
    {
        GUILayout.BeginHorizontal();
        //GUILayout.Label(label);
        if (GUILayout.Button(label))
        {
            EditorHelper.SelectObject(obj);
        }
        TO go = EditorGUILayout.ObjectField(obj, typeof(TO), true) as TO;
        GUILayout.EndHorizontal();
        return go;
    }

    public static TO ObjectField<TO>(TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }
    public static TO ObjectField<TO>(string label, TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        if (GUILayout.Button(label))
        {
            EditorHelper.SelectObject(obj);
        }
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }

    public static TO ObjectField<TO>(string label, float width,TO obj, params GUILayoutOption[] options) where TO : UnityEngine.Object
    {
        if (GUILayout.Button(label,GUILayout.Width(width)))
        {
            EditorHelper.SelectObject(obj);
        }
        return EditorGUILayout.ObjectField(obj, typeof(TO), true, options) as TO;
    }

    public static TO ObjectField<TO>(string label,int width1, TO obj, int width2=0) where TO : UnityEngine.Object
    {
        if (GUILayout.Button(label,GUILayout.Width(width1)))
        {
            EditorHelper.SelectObject(obj);
        }
        if (width2 == 0)
        {
            return EditorGUILayout.ObjectField(obj, typeof(TO), true) as TO;
        }
        else
        {
            return EditorGUILayout.ObjectField(obj, typeof(TO), true, GUILayout.Width(width2)) as TO;
        }
        
    }
}
