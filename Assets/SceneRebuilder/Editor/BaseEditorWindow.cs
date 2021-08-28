using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BaseEditorWindow//<T> : EditorWindow where T: EditorWindow, IBaseEditorWindow
{
    public static void ShowWindow<T>(int width,int height,string title) where T : EditorWindow, IBaseEditorWindow
    {
        var window = (T)EditorWindow.GetWindowWithRect(typeof(T), new Rect(0, 0, width, height), false, title);
        window.Show();
        window.Init();
    }

    //public abstract void Init();
}

public interface IBaseEditorWindow
{
    public void Init();
}
