using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColumnSortComponent  {

    public bool IsSortByVertextCount = true;
    public bool IsSortByParent = false;
    public bool IsSortByName = false;

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("SortBy:", GUILayout.Width(50));
        IsSortByVertextCount = GUILayout.Toggle(IsSortByVertextCount, "VertextCount", GUILayout.Width(100));
        if (IsSortByVertextCount)
        {
            IsSortByParent = false;
            IsSortByName = false;
        }
        IsSortByParent = GUILayout.Toggle(IsSortByParent, "Parent", GUILayout.Width(100));
        if (IsSortByParent)
        {
            IsSortByVertextCount = false;
            IsSortByName = false;
        }
        IsSortByName = GUILayout.Toggle(IsSortByName, "Name", GUILayout.Width(100));
        if (IsSortByName)
        {
            IsSortByVertextCount = false;
            IsSortByParent = false;
        }
        EditorGUILayout.EndHorizontal();
    }

    private Dictionary<Component, int> vertexCountDic = new Dictionary<Component, int>();

    private int GetVertexCount(Component b)
    {
        if (vertexCountDic.ContainsKey(b))
        {
            return vertexCountDic[b];
        }
        int count = EditorHelper.GetVertexs(b.gameObject);
        vertexCountDic[b] = count;
        return count;
    }

    public int SortMethod(Component a, Component b)
    {
        var result = 0;
        if (IsSortByVertextCount)
        {
            var count1 = GetVertexCount(a);
            var count2 = GetVertexCount(b);
            result = count2.CompareTo(count1);
        }
        else if (IsSortByName)
        {
            result = a.gameObject.name.CompareTo(b.gameObject.name);
        }
        else
        {
            result = SortByParentName(a.gameObject, b.gameObject);
        }
        return result;
    }

    private int SortByParentName(GameObject a,GameObject b)
    {
        var result = 0;
        var parent1 = a.transform.parent;
        var parent2 = b.transform.parent;
        if (parent1 != null && parent2 != null)
        {
            var result1 = parent1.name.CompareTo(parent2.name);
            if (result1 == 0)
            {
                result = a.name.CompareTo(b.name);
            }
            else
            {
                result = result1;
            }
        }
        else
        {
            result = a.name.CompareTo(b.name);
        }
        return result;
    }
}
