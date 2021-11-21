using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelper
{
    public static void AddPreName(Transform p)
    {
        string pName = p.name + "_";
        for (int i = 0; i < p.childCount; i++)
        {
            var child = p.GetChild(i);
            if (child.name.StartsWith(pName)) continue;
            child.name = $"{pName}{child.name}";
        }
    }

    public static void AddPreNames(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            TransformHelper.AddPreName(child);
        }
    }

    public static void RemoveChildren(Transform root)
    {
        List<Transform> list1 = new List<Transform>();
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            list1.Add(child);

        }

        for (int i = 0; i < list1.Count; i++)
        {
            var child = list1[i];
            EditorHelper.UnpackPrefab(child.gameObject);

            List<Transform> list2 = new List<Transform>();
            for (int j = 0; j < child.childCount; j++)
            {
                list2.Add(child.GetChild(j));
            }
            foreach (var t in list2)
            {
                t.SetParent(root);
            }
            GameObject.DestroyImmediate(child.gameObject);
            //i--;
        }
    }

    public static void ReGroup1_After(Transform root)
    {
        string pName = root.name + "_";
        Dictionary<string, List<Transform>> beforeNames = new Dictionary<string, List<Transform>>();
        Dictionary<string, List<Transform>> afterNames = new Dictionary<string, List<Transform>>();
        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            string cName = child.name;
            if (cName.Contains("_") == false)
            {

                continue;
            }
            string[] parts = cName.Split('_');
            string n1 = parts[0];
            string n2 = parts[1];
            if (!beforeNames.ContainsKey(n1))
            {
                beforeNames.Add(n1, new List<Transform>());
            }
            beforeNames[n1].Add(child);

            if (!afterNames.ContainsKey(n2))
            {
                afterNames.Add(n2, new List<Transform>());
            }
            afterNames[n2].Add(child);
        }
        foreach (var n in afterNames.Keys)
        {
            GameObject go = new GameObject(n);
            var list = afterNames[n];
            go.transform.SetParent(root);
            foreach (var child in list)
            {
                child.SetParent(go.transform);
            }
        }
    }

    public static void ReGroupBefore(Transform root)
    {
        EditorHelper.UnpackPrefab(root.gameObject);

        string pName = root.name + "_";
        Dictionary<string, List<Transform>> beforeNames = new Dictionary<string, List<Transform>>();
        Dictionary<string, List<Transform>> afterNames = new Dictionary<string, List<Transform>>();
        GetBeforeAfterList(root, beforeNames, afterNames);

        ReGroupByDict(root, beforeNames);
    }

    public static void ReGroupByDict(Transform root,Dictionary<string, List<Transform>> dict)
    {
        List<Transform> orgChildren = new List<Transform>();
        for(int i=0;i<root.childCount;i++)
        {
            orgChildren.Add(root.GetChild(i));
        }

        foreach (var n in dict.Keys)
        {
            GameObject go = new GameObject(n);
            var list = dict[n];
            go.transform.SetParent(root);
            foreach (var child in list)
            {
                child.SetParent(go.transform);
                Debug.Log($"parent:{go.name} child:{child}");
            }
        }

        foreach(var c in orgChildren)
        {
            GameObject.DestroyImmediate(c.gameObject);
        }
    }

    public static void ReGroupAfter(Transform root)
    {
        EditorHelper.UnpackPrefab(root.gameObject);

        string pName = root.name + "_";
        Dictionary<string, List<Transform>> beforeNames = new Dictionary<string, List<Transform>>();
        Dictionary<string, List<Transform>> afterNames = new Dictionary<string, List<Transform>>();
        GetBeforeAfterList(root, beforeNames, afterNames);

        ReGroupByDict(root, afterNames);
    }

    private static void GetBeforeAfterList(Transform root, Dictionary<string, List<Transform>> beforeNames, Dictionary<string, List<Transform>> afterNames)
    {
        for (int j = 0; j < root.childCount; j++)
        {
            var child0 = root.GetChild(j);
            for (int i = 0; i < child0.childCount; i++)
            {
                var child = child0.GetChild(i);
                string cName = child.name;
                if (cName.Contains("_") == false)
                {

                    continue;
                }
                string[] parts = cName.Split('_');
                string n1 = parts[0];
                string n2 = parts[1];
                if (!beforeNames.ContainsKey(n1))
                {
                    beforeNames.Add(n1, new List<Transform>());
                }
                beforeNames[n1].Add(child);

                if (!afterNames.ContainsKey(n2))
                {
                    afterNames.Add(n2, new List<Transform>());
                }
                afterNames[n2].Add(child);
            }
        }
    }
}
