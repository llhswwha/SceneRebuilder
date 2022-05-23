using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension 
{
    public static Dictionary<Transform, string> PathDict = new Dictionary<Transform, string>();

    public static string GetPath<T>(T t, Transform root = null, string split = ">") where T : MonoBehaviour
    {
        return t.transform.GetPath(root, split);
    }


    public static void MoveToNewRoot(GameObject target, string newRootName)
    {
        GameObject newRoot = GameObject.Find(newRootName);
        if (newRoot == null)
            newRoot = new GameObject(newRootName);
        MeshFilter[] gos = target.GetComponentsInChildren<MeshFilter>(true);
        foreach (MeshFilter go in gos)
        {
            MoveGameObject(go.transform, newRoot.transform);
        }
        Debug.LogError($"MoveToZero gos:{gos.Length}");
    }

    public static void MoveGameObject(Transform target, Transform newRoot,Transform pathRoot=null)
    {
        List<Transform> path = target.GetAncestors(pathRoot);
        Transform newP = FindOrCreatePath(newRoot, path, false);
        target.SetParent(newP.transform);
    }

    public static Transform FindOrCreatePath(Transform root, List<Transform> path, bool isDebug)
    {
        Transform parent = root;
        for (int j = 0; j < path.Count - 1; j++)
        {
            Transform node = path[j];
            Transform child = FindOrCreate(parent, node.name);
            if (isDebug)
            {
                Debug.LogError($"FindOrCreatePath[{j}/{path.Count}] root:{root.name} node:{node.name} parent:{parent.name} ");
            }
            parent = child;
        }
        return parent;
    }

    public static Transform FindOrCreate(Transform root, string name)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == name)
            {
                return child;
            }
        }
        GameObject go = new GameObject(name);
        go.transform.SetParent(root);
        return go.transform;
    }
    public static string GetPathWithActive<T>(T t) where T : MonoBehaviour
    {
        if (t == null) return "NULL";
        return GetPathWithActive(t.transform);
    }

    public static string GetPathWithActive(Transform t)
    {
        string path = "";
        while (t != null)
        {
            path = $"{t.name}({t.gameObject.activeSelf})>{path}";
            t = t.parent;
        }
        return path;
    }

    public static List<Transform> GetAncestors(this Transform t, Transform root = null)
    {
        List<Transform> ancestors = new List<Transform>();
        ancestors.Add(t);
        string path = t.name;
        t = t.parent;
        while (t != null)
        {
            if (t == root)
            {
                break;
            }
            ancestors.Add(t);

            t = t.parent;
        }
        ancestors.Reverse();
        return ancestors;
    }

    public static string GetPath(this Transform t, Transform root = null, string split = ">")
    {
        //string path = t.name;
        //t = t.parent;
        //while (t != null)
        //{
        //    path = t.name + split + path;
        //    if (t == root)
        //    {
        //        break;
        //    }
        //    t = t.parent;
        //}
        //return path;

        List<Transform> ancestors = GetAncestors(t, root);

        string path = "";
        for (int i = 0; i < ancestors.Count; i++)
        {
            Transform a = ancestors[i];
            path += a.name;
            if (i < ancestors.Count - 1)
            {
                path += split;
            }
        }
        return path;
    }
}
