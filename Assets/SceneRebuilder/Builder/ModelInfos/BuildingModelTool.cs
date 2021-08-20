using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModelTool : MonoBehaviour
{
    public List<GameObject> BoundsGos = new List<GameObject>();

    public List<GameObject> OutRoots = new List<GameObject>();

    public List<GameObject> OutGos = new List<GameObject>();

    [ContextMenu("ShowRenderers")]
    public void ShowRenderers()
    {
        var renderers = this.GetComponentsInChildren<MeshRenderer>(true);
        MeshHelper.ShowAllRenderers(renderers, 5);
    }

    public Bounds GetBounds(Vector3 scale)
    {
        ShowRenderers();

        List<Renderer> renderers = new List<Renderer>();
        foreach(var go in BoundsGos)
        {
            renderers.AddRange(go.GetComponentsInChildren<Renderer>(true));
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
        AreaTreeHelper.CreateBoundsCube(bounds, this.name + "_Bounds", transform,0);
    }

    [ContextMenu("CreateBounds95")]
    public void CreateBounds95()
    {
        Bounds bounds = GetBounds(new Vector3(0.95f, 1f, 0.95f));
        AreaTreeHelper.CreateBoundsCube(bounds, this.name + "_Bounds", transform,0);
    }

    [ContextMenu("CreateBounds90")]
    public void CreateBounds90()
    {
        Bounds bounds = GetBounds(new Vector3(0.9f, 0.9f, 0.9f));
        AreaTreeHelper.CreateBoundsCube(bounds, this.name + "_Bounds", transform,0);
    }

    public List<GameObject> inList = new List<GameObject>();
    public List<GameObject> outList = new List<GameObject>();

    public Transform InRoot = null;

    [ContextMenu("* FindDoorsInBounds95")]
    public void FindDoorsInBounds95()
    {
        SetDoorSetting();
        FindGosInBounds(0.95f);
    }

    [ContextMenu("* FindDoorsInBounds90")]
    public void FindDoorsInBounds90()
    {
        SetDoorSetting();
        FindGosInBounds(0.90f);
    }

    [ContextMenu("* FindDoorsInBounds85")]
    public void FindDoorsInBounds85()
    {
        SetDoorSetting();
        FindGosInBounds(0.85f);
    }

    [ContextMenu("* FindDoorsInBounds80")]
    public void FindDoorsInBounds80()
    {
        SetDoorSetting();
        FindGosInBounds(0.80f);
    }


    [ContextMenu("* SetDoorSetting")]
    public void SetDoorSetting()
    {
        BoundsGos.Clear();
        OutRoots.Clear();

        BuildingModelInfo info = this.GetComponent<BuildingModelInfo>();
        for(int i=0;i<info.OutPart0.transform.childCount;i++)
        {
            var child = info.OutPart0.transform.GetChild(i);
            if(child.name.Contains("Window"))
            {

            }
            if (child.name.Contains("Wall"))
            {
                BoundsGos.Add(child.gameObject);
            }
            if (child.name.Contains("Door"))
            {
                OutRoots.Add(child.gameObject);
            }
        }
        if (info.InPart == null)
        {
            info.InPart = info.InitPart("In");
            
        }

        for(int i=0;i<info.InPart.transform.childCount;i++)
        {
            var child = info.InPart.transform.GetChild(i);
            if(child.name=="Doors")
            {
                InRoot = child;
                break;
            }
        }

        if(InRoot==null)
        {
            GameObject doors = info.InitSubPart("Doors", info.InPart.transform);
            InRoot = doors.transform;
        }

        info.InitInOut(true);

    }

    [ContextMenu("* FindGosInBounds95")]
    public void FindGosInBounds95()
    {
        FindGosInBounds(0.95f);
    }

    public void FindGosInBounds(float scale)
    {
        Bounds bounds = GetBounds(new Vector3(scale, 1f, scale));

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

        BuildingModelInfo info = this.GetComponent<BuildingModelInfo>();
        info.InitInOut(true);

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
                    if (components.Length == 1)
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
        MeshHelper.RemoveEmptyObjects(this.gameObject);
    }
}
