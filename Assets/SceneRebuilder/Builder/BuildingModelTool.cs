using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelTool : MonoBehaviour
{
    public List<GameObject> BoundsGos = new List<GameObject>();

    public List<GameObject> OutRoots = new List<GameObject>();

    public List<GameObject> OutGos = new List<GameObject>();

    public Bounds GetBounds(Vector3 scale)
    {
        List<Renderer> renderers = new List<Renderer>();
        foreach(var go in BoundsGos)
        {
            renderers.AddRange(go.GetComponentsInChildren<Renderer>());
        }
        Bounds bounds = ColliderHelper.CaculateBounds(renderers);
        var size = bounds.size;
        bounds.size = new Vector3(size.x * scale.x, size.y * scale.y, size.z * scale.z);
        return bounds;
    }

    [ContextMenu("CreateBounds")]
    public void CreateBounds()
    {
        Bounds bounds = GetBounds(new Vector3(1,1,1));
        AreaTreeHelper.CreateBoundsCube(bounds, this.name + "_Bounds", transform);
    }

    [ContextMenu("CreateBounds95")]
    public void CreateBounds95()
    {
        Bounds bounds = GetBounds(new Vector3(0.95f, 1f, 0.95f));
        AreaTreeHelper.CreateBoundsCube(bounds, this.name + "_Bounds", transform);
    }

    [ContextMenu("CreateBounds90")]
    public void CreateBounds90()
    {
        Bounds bounds = GetBounds(new Vector3(0.9f, 0.9f, 0.9f));
        AreaTreeHelper.CreateBoundsCube(bounds, this.name + "_Bounds", transform);
    }

    public List<GameObject> inList = new List<GameObject>();
    public List<GameObject> outList = new List<GameObject>();

    public Transform InRoot = null;

    [ContextMenu("FindGosInBounds95")]
    public void FindGosInBounds95()
    {
        Bounds bounds = GetBounds(new Vector3(0.95f, 1f, 0.95f));

        inList.Clear();
        outList.Clear();

        List<GameObject> gos = new List<GameObject>();
        gos.AddRange(OutGos);
        foreach(var root in OutRoots)
        {
            for(int i=0;i<root.transform.childCount;i++)
            {
                gos.Add(root.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < gos.Count; i++)
        {
            GameObject go = gos[i];
            var goBounds = ColliderHelper.CaculateBounds(go);
            if (bounds.Contains(goBounds.min) && bounds.Contains(goBounds.max))
            {
                inList.Add(go);

                if (InRoot)
                {
                    go.transform.SetParent(InRoot);
                }
            }
            else
            {
                outList.Add(go);
            }
        }

        Debug.Log($"FindGosInBounds95:in:{inList.Count},out:{outList.Count},gos:{gos.Count}");
    }

    [ContextMenu("RemoveGeometryGroup")]
    public void RemoveGeometryGroup()
    {
        var ts = this.GetComponentsInChildren<Transform>(true);
        List<Transform> groupList = new List<Transform>();
        for (int i = 0; i < ts.Length; i++)
        {
            var t = ts[i];
            if (t.childCount == 1)
            {
                var child = t.GetChild(0);
                if (child.childCount == 0)
                {
                    var components = t.GetComponents<Component>();
                    if (components.Length == 1)//只有一个Transform组件
                    {
                        groupList.Add(t);
                        Debug.Log($"group:{t.name}");
                    }
                }
            }
        }

        for (int i = 0; i < groupList.Count; i++)
        {
            //GameObject.DestroyImmediate(emptyList[i].gameObject);
            var group = groupList[i];
            var geometry = group.GetChild(0);
            geometry.SetParent(group.parent);
            geometry.name = group.name;
            GameObject.DestroyImmediate(group.gameObject);
        }

        Debug.Log($"empty:{groupList.Count},all:{ts.Length}");
    }

    [ContextMenu("RemoveEmptyObjects")]
    public void RemoveEmptyObjects()
    {
        var ts = this.GetComponentsInChildren<Transform>(true);
        List<Transform> emptyList = new List<Transform>();
        for (int i = 0; i < ts.Length; i++)
        {
            var t = ts[i];
            if (t.childCount == 0)
            {
                var components = t.GetComponents<Component>();
                if (components.Length == 1)//只有一个Transform组件
                {
                    emptyList.Add(t);
                    Debug.Log($"empty:{t.name}");
                }
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            GameObject.DestroyImmediate(emptyList[i].gameObject);
        }

        Debug.Log($"empty:{emptyList.Count},all:{ts.Length}");
    }
}
