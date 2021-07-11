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
        Debug.Log($"InitPreview:{obj}");
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
    protected int pageCount = 16;
    protected int pageIndex = 0;

    string[] pageSizeList ={"10","16","50","100","500","1000","2000","4000"};
    int pageSizeId=1;

    protected bool setPageIndexSetting=false;
    /// <summary>
    /// 绘制页标
    /// </summary>
    protected void DrawPageIndexBlock()
    {
        //count:1000 pageCount:16 pages:62
        int count = meshElementList.Count;
        if(setPageIndexSetting){
            setPageIndexSetting=false;
            if(count<=100){
                pageCount=10;
                pageSizeId=0;
            }
            else if(count<=500){
                pageCount=16;
                pageSizeId=1;
            }
            else if(count<=1000){
                pageCount=50;
                pageSizeId=2;
            }
            else if(count<=5000){
                pageCount=100;
                pageSizeId=3;
            }
            else if(count<=10000){
                pageCount=500;
                pageSizeId=4;
            }
            else if(count<=20000){
                pageCount=1000;
                pageSizeId=5;
            }
            else if(count<=40000){
                pageCount=2000;
                pageSizeId=6;
            }
            else{
                
                pageCount=4000;
                pageSizeId=7;
            }

            Debug.LogError($"DrawPageIndexBlock count:{count} pageCount:{pageCount}");
        }

        int pages = count / pageCount;
        
        //Debug.Log($"DrawPageIndexBlock count:{count} pageCount:{pageCount} pages:{pages}");
        if (count % pageCount != 0)
            pages++;

        Rect ActualRect = MPGUIStyles.PAGEINDEX_BLOCK;
        ActualRect.x = MPGUIStyles.PAGEINDEX_BLOCK.x + MPGUIStyles.PAGEINDEX_BLOCK.width - 160 - 35 * pages-140;
        GUILayout.BeginArea(ActualRect);
        GUILayout.BeginHorizontal();
        if (meshElementList.Count != 0)
        {
            var index =EditorGUILayout.Popup(pageSizeId, pageSizeList,GUILayout.MaxWidth(70));
            pageSizeId=index;
            pageCount=int.Parse(pageSizeList[pageSizeId]);

            if (pageIndex == 0)
            {
                GUI.enabled = false;
            }

            if(count!=originList.Count){
                 if (GUILayout.Button($"{count}/{originList.Count}", MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(70)))
                {
                    
                }
            }
            else{
                if (GUILayout.Button($"{count}", MPGUIStyles.itemBtnStyles_child[0], GUILayout.MaxWidth(70)))
                {
                    
                }
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

    protected virtual void PreviewSelectedItem(T1 ele)
    {
        InitPreview(ele.rootObj);
    }

    protected virtual void DrawItem(T1 element, bool isSelect, int index)
    {

    }

    protected virtual void Sort(List<T1> list)
    {

    }

    protected virtual void SelectByConditions(List<T1> list)
    {

    }

    protected virtual int width1
    {
        get
        {
            return 66;
        }
    }
    protected virtual int width2
    {
        get
        {
            return 40;
        }
    }
    protected virtual int width3
    {
        get
        {
            return 50;
        }
    }

    protected virtual string GetValue(object v, GUIStyle style)
    {
        string t = v + "";
        if (v is int)
        {
            int i = (int)v;
            if (i == 0)
            {
                t = "-";
            }
            if (i > 50000)//5w
            {
                style.normal.textColor = new Color(1, 0, 0);
            }
            else if (i > 10000)//1w
            {
                style.normal.textColor = new Color(1, 102f / 255, 102f / 255);
            }
            else if (i > 5000)//5k
            {
                style.normal.textColor = new Color(1, 153f / 255, 102f / 255);
            }
            else if (i > 1000)//1k
            {
                style.normal.textColor = new Color(1, 204f / 255, 153f / 255);
            }
            else
            {

            }

        }
        else if (v is float)
        {
            float f = (float)v;

            if (f > 400)
            {
                style.normal.textColor = new Color(1, 0, 0);
            }
            else if (f > 200)
            {
                style.normal.textColor = new Color(1, 102f / 255, 102f / 255);
            }
            else if (f > 100)
            {
                style.normal.textColor = new Color(1, 153f / 255, 102f / 255);
            }
            else if (f > 50)
            {
                style.normal.textColor = new Color(1, 204f / 255, 153f / 255);
            }
            else
            {

            }
            if (f < 10)
            {
                t = f.ToString("F2");
            }
            else if (f < 100)
            {
                t = f.ToString("F1");
            }
            else
            {
                t = f.ToString("F0");
            }

            if (f == 0)
            {
                t = "-";
            }
        }
        else if (v is bool)
        {
            bool b = (bool)v;
            if (b == true)
            {
                style.normal.textColor = new Color(0, 1, 0);
            }
        }
        else
        {

        }
        return t;
    }

    protected List<T1> tempList = new List<T1>();


    protected string m_InputSearchText;
    /// <summary>
    /// 按照搜索结果刷新列表，不重新获取。
    /// </summary>
    protected void RefleshSearchList()
    {
        tempList.Clear();
        for (int i = 0; i < originList.Count; i++)
        {
            if (originList[i].name.Contains(m_InputSearchText))
            {
                tempList.Add(originList[i]);
            }
        }
        meshElementList.Clear();
        meshElementList.AddRange(tempList);

        SelectIndex = -1;
        pageIndex = 0;
    }

    protected int selectChildIndex = 0;//子物体选择下标
    protected virtual int SelectChildIndex
    {
        get { return selectChildIndex; }
        set
        {
            selectChildIndex = value;
            if (selectChildIndex >= 0)
                InitPreviewChild(meshElementList[SelectIndex].childList[selectChildIndex]);
        }
    }

    protected virtual void InitPreviewChild(T2 obj)
    {

    }

    protected int selectIndex = 0;//父物体选择下标
    protected int SelectIndex
    {
        get { return selectIndex; }
        set
        {
            selectIndex = value;
            if (selectIndex >= 0)
            {
                PreviewSelectedItem(meshElementList[selectIndex]);
            }

        }
    }
}