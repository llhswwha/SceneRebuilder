using MeshProfilerNS;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ListManagerEditorWindow<T1,T2> : EditorWindow where T1 : ListItemElement<T2> where T2:ListItemElementValues,new()
{
    protected List<T1> meshElementList = new List<T1>();
    protected List<T1> originList = new List<T1>();

    protected Editor previewEditor;

    /// <summary>
    /// 初始化预览物体
    /// </summary>
    /// <param name="obj"></param>
    protected void InitPreview(UnityEngine.Object obj)
    {
        if (previewEditor != null)
        {
            DestroyImmediate(previewEditor);
        }

        previewEditor = Editor.CreateEditor(obj);
    }

    /// <summary>
    /// 绘制预览窗口
    /// </summary>
    protected void DrawPreviewBlock()
    {
        GUI.Box(MPGUIStyles.BorderArea(MPGUIStyles.PREVIEW_BLOCK), "");
        

        if (meshElementList.Count > 0 && previewEditor != null)
        {
            previewEditor.DrawPreview(MPGUIStyles.PREVIEW_BLOCK_CENTER);
        }

    }

    protected Vector2 scVector = new Vector2(0, 0);
    protected bool isRetract = false;//是否折叠
    protected const int pageCount = 100;
    protected int pageIndex = 0;

    /// <summary>
    /// 绘制页标
    /// </summary>
    protected void DrawPageIndexBlock()
    {
        int count = meshElementList.Count;
        int pages = count / pageCount;
        if (count % pageCount != 0)
            pages++;

        Rect ActualRect = MPGUIStyles.PAGEINDEX_BLOCK;
        ActualRect.x = MPGUIStyles.PAGEINDEX_BLOCK.x + MPGUIStyles.PAGEINDEX_BLOCK.width - 160 - 35 * pages;
        GUILayout.BeginArea(ActualRect);
        GUILayout.BeginHorizontal();
        if (meshElementList.Count != 0)
        {
            if (pageIndex == 0)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Last Page", MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(70)))
            {
                pageIndex--;
                scVector = Vector2.zero;
            }
            GUI.enabled = true;

            for (int i = 0; i < pages; i++)
            {
                bool isGoal = i == pageIndex;
                string str = "[" + i + "]";
                if (isGoal)
                {
                    if (GUILayout.Button(str, MPGUIStyles.itemBtnStyles_child[1], GUILayout.MaxWidth(35)))
                    {
                        pageIndex = i;
                        scVector = Vector2.zero;
                    }
                }
                else
                {
                    if (GUILayout.Button(str, MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(35)))
                    {
                        pageIndex = i;
                        scVector = Vector2.zero;
                    }
                }
            }
            if (pageIndex == (pages - 1))
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Next Page", MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(70)))
            {
                pageIndex++;
                scVector = Vector2.zero;
            }
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}