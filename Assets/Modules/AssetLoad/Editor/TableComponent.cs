using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TableComponent  {

    public class ColumnInfo
    {
        public string Name { get; set; }

        public int Width { get; set; }
    }

    public List<ColumnInfo> Columns = new List<ColumnInfo>();

    public Dictionary<string, int> dict = new Dictionary<string, int>();

    public void Clear()
    {
        Columns.Clear();
        dict.Clear();
    }

    public void AddColumn(string name,int width)
    {
        ColumnInfo column = new ColumnInfo();
        column.Name = name;
        column.Width = width;
        Columns.Add(column);

        if (dict.ContainsKey(name))
        {
            dict[name] = width;
        }
        else
        {
            dict.Add(name, width);
        }
    }

	public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        foreach (var column in Columns)
        {
            GUILayout.Label(column.Name, GUILayout.Width(column.Width));
        }
        //GUILayout.Label("ID", GUILayout.Width(idColumnWidth));
        //GUILayout.Label("Scene", GUILayout.Width(sceneColumnWidth));
        //GUILayout.Label("Count", GUILayout.Width(countColumnWidth));
        //GUILayout.Label("Parent", GUILayout.Width(parentColumnWidth));
        //GUILayout.Label("Name", GUILayout.Width(nameColumnWidth));
        //GUILayout.Label("Node", GUILayout.Width(nodeColumnWidth));
        EditorGUILayout.EndHorizontal();
    }

    public int GetWidth(string name)
    {
        if (dict.ContainsKey(name))
        {
            return dict[name];
        }
        else
        {
            return 100;//默认100
        }
    }

    public GUILayoutOption GetGUIWidth(string name)
    {
        var width = GetWidth(name);
        return GUILayout.Width(width);
    }
}
