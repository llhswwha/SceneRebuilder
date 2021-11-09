using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBoxManager : SingletonBehaviour<FloorBoxManager>
{
    public bool IsChangeParent = false;
    public bool IsIn = true;
    public List<GameObject> Floors = new List<GameObject>();
    //public List<BoxCollider> FloorBoxs = new List<BoxCollider>();
    public GameObject Sources;

    [ContextMenu("InitBoxColliders")]
    public void InitBoxColliders()
    {
        //FloorBoxs.Clear();
       for (int i=0;i<Floors.Count;i++)
        {
            BoxCollider boxCollider = Floors[i].GetBoxCollider();
            //FloorBoxs.Add(boxCollider);
        }

    }

    //public List<BoxCollider> GetBoxes(MeshRendererInfo renderer)
    //{
    //    return GetBoxes(renderer.center);
    //}

    //public List<BoxCollider> GetBoxes(Vector3 p)
    //{
    //    List<BoxCollider> boxes = new List<BoxCollider>();
    //    for (int i = 0; i < FloorBoxs.Count; i++)
    //    {
    //        if (FloorBoxs[i].bounds.Contains(p))
    //        {
    //            boxes.Add(FloorBoxs[i]);
    //        }
    //    }
    //    return boxes;
    //}

    private List<GameObject> GetBoxes(Bounds b1,Func<BoxCollider,Bounds, bool> isIntersectFun)
    {
        List<GameObject> result = new List<GameObject>();
        for(int i=0;i<Floors.Count;i++)
        {
            GameObject floor = Floors[i];
            BoxCollider[] boxes = floor.GetComponents<BoxCollider>();
            foreach(var box in boxes)
            {
                if(isIntersectFun(box,b1))
                {
                    result.Add(floor);
                    break;
                }
            }
        }
        return result;
    }

    public List<GameObject> GetBoxes(Transform t)
    {
        Bounds b0 = CaculateBounds(t.gameObject);
        List<GameObject> result = new List<GameObject>();
        result = GetBoxes(b0, (box, b1) =>{ return (box.bounds.Contains(b1.min) && box.bounds.Contains(b1.max) && box.bounds.Contains(b1.center)); });
        
        if (result.Count == 0)
        {
            result = GetBoxes(b0, (box, b1) =>{ return (box.bounds.Contains(b1.center)); });
        }        
        if (result.Count == 0)
        {
            result = GetBoxes(b0, (box, b1) =>{ return (box.bounds.Intersects(b1)); });
        }
        //if (result.Count == 0)
        //{
        //    result = GetBoxes(b0, (box, b1) =>{ return (box.bounds.Contains(b1.min) || box.bounds.Contains(b1.max) || box.bounds.Contains(b1.center)); });
        //}
        return result;
    }

    public List<TransformFloorParent> List0 = new List<TransformFloorParent>();
    public List<TransformFloorParent> List1 = new List<TransformFloorParent>();
    public List<TransformFloorParent> List2 = new List<TransformFloorParent>();

    [ContextMenu("SetToFloorRenderers")]
    public void SetToFloorRenderers()
    {
        List0.Clear();
        List1.Clear();
        List2.Clear();

        InitBoxColliders();
        MeshRenderer[] meshRenderers = Sources.GetComponentsInChildren<MeshRenderer>(true);
        MeshRendererInfoList meshRendererInfos = new MeshRendererInfoList(meshRenderers);
        foreach (var renderer in meshRendererInfos)
        {
            List<GameObject> boxes = GetBoxes(renderer.transform);
            if (boxes.Count == 1)
            {
                List1.Add(new TransformFloorParent(renderer.transform, boxes[0].transform));
            }
            else if(boxes.Count == 0)
            {
                List0.Add(new TransformFloorParent(renderer.transform));
            }
            else
            {
                List2.Add(new TransformFloorParent(renderer.transform, boxes));
            }
        }

        SetParentEx();
    }

    [ContextMenu("SetToFloorChild")]
    public void SetToFloorChild()
    {
        List0.Clear();
        List1.Clear();
        List2.Clear();

        InitBoxColliders();
        //MeshRenderer[] meshRenderers = Sources.GetComponentsInChildren<MeshRenderer>(true);
        for (int i=0;i< Sources.transform.childCount;i++)
        {
            var child = Sources.transform.GetChild(i);
            List<GameObject> floors = GetBoxes(child);
            if (floors.Count == 1)
            {
                List1.Add(new TransformFloorParent(child.transform, floors[0].transform));
            }
            else if (floors.Count == 0)
            {
                List0.Add(new TransformFloorParent(child.transform));
            }
            else
            {
                List2.Add(new TransformFloorParent(child.transform, floors));
            }
        }

        SetParentEx();
    }

    [ContextMenu("SetParent")]
    public void SetParent()
    {
        foreach (var r in List1)
        {
            r.SetParent(Sources.name,IsIn);
        }
        foreach (var r in List2)
        {
            r.SetParent(Sources.name, IsIn);
        }
    }

    [ContextMenu("SetParentEx")]
    public void SetParentEx()
    {
        if (IsChangeParent)
        {
            SetParent();
        }
    }

    public GameObject TestGo;

    public GameObject TestFloor;

    [ContextMenu("TestParent")]
    public void TestParent()
    {
        BoxCollider floor = TestFloor.GetComponent<BoxCollider>();
        Debug.Log($"TestParent1 BoxCollider:{floor} TestGo:{TestGo} TestFloor:{TestFloor}");
        Bounds b1 = CaculateBounds(TestGo);
        Debug.Log($"Bounds:{b1},min:{b1.min},max:{b1.max},center:{b1.center}");
        if (floor.bounds.Contains(b1.min) && floor.bounds.Contains(b1.max))
        {
            //boxes.Add(floor);
            Debug.Log($"TestParent2:{floor}");
        }

        if (floor.bounds.Contains(b1.center))
        {
            //boxes.Add(floor);
            Debug.Log($"TestParent3:{floor}");
        }
    }

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    /// <param name="renders"></param>
    /// <returns></returns>
    public static Bounds CaculateBounds(GameObject root)
    {
        //IEnumerable<MeshFilter> meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        ////Debug.Log($"CaculateBounds renders:{renders.Count()},isAll:{isAll}");
        //Vector3 center = Vector3.zero;
        //int count = 0;
        //foreach (MeshFilter meshFilter in meshFilters)
        //{
        //    center += meshFilter.sharedMesh.bounds.center;
        //    count++;
        //}

        //if (count > 0)
        //{
        //    center /= count;
        //}
        //Bounds bounds = new Bounds(center, Vector3.zero);
        //foreach (MeshFilter meshFilter in meshFilters)
        //{
        //    bounds.Encapsulate(meshFilter.sharedMesh.bounds);
        //}
        BoxCollider box=ColliderHelper.AddCollider(root);
        Bounds bounds = box.bounds;
        GameObject.DestroyImmediate(box);
        return bounds;
    }
}

[Serializable]
public class TransformFloorParent
{
    public Transform go;

    public Transform floor;

    public List<Transform> floors = new List<Transform>();

    public TransformFloorParent(Transform g, Transform p)
    {
        this.go = g;
        this.floor = p;
        floors.Add(p);
    }

    public TransformFloorParent(Transform g)
    {
        this.go = g;
    }
    public TransformFloorParent(Transform g, List<BoxCollider> fs)
    {
        this.go = g;
        //this.floors = fs;
        foreach(var f in fs)
        {
            this.floors.Add(f.transform);
        }
        floor = floors[0];
    }

    public TransformFloorParent(Transform g, List<GameObject> fs)
    {
        this.go = g;
        //this.floors = fs;
        foreach (var f in fs)
        {
            this.floors.Add(f.transform);
        }
        floor = floors[0];
    }

    public string GetFloors()
    {
        string fs = "";
        foreach(var f in floors)
        {
            fs += f.name + ";";
        }
        return fs;
    }

    public TransformFloorParent()
    {

    }

    public Transform p;

    public void SetParent(string pName,bool isIn)
    {
        if (p != null)
        {
            go.SetParent(p.transform);
            return;
        }
        Transform fP = floor;
        Transform inP = null;
        for(int i=0;i<fP.childCount;i++)
        {
            if(fP.GetChild(i).name=="In")
            {
                inP = fP.GetChild(i);
            }
        }
        if (isIn && inP != null)
        {
            fP = inP;
        }

        for(int i = 0; i < fP.childCount; i++)
        {
            var child = fP.GetChild(i);
            if(child.name==pName)
            {
                p = child;
                EditorHelper.UnpackPrefab(go.gameObject);
                go.SetParent(child);
                return;
            }
        }

        GameObject newP = new GameObject(pName);
        newP.transform.position = fP.transform.position;
        newP.transform.SetParent(fP);
        //p = newP.transform;

        EditorHelper.UnpackPrefab(go.gameObject);
        go.SetParent(newP.transform);
    }
}
