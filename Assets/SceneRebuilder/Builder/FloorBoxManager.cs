using CommonUtils;
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

    [ContextMenu("AddFloors")]
    public void AddFloors()
    {
        var buildings = GameObject.FindObjectsOfType<BuildingController>();
        foreach (var b in buildings)
        {
            var floors = b.GetComponentsInChildren<FloorController>();
            foreach (var floor in floors)
            {
                Floors.Add(floor.gameObject);
            }
        }
    }

    [ContextMenu("GetFloors")]
    public void GetFloors()
    {
        Floors.Clear();
        var buildings = GameObject.FindObjectsOfType<BuildingController>();
        foreach(var b in buildings)
        {
            var floors = b.GetComponentsInChildren<FloorController>();
            foreach(var floor in floors)
            {
                Floors.Add(floor.gameObject);
            }
        }

        if (buildings.Length == 0)
        {
            var floors = GameObject.FindObjectsOfType<FloorController>();
            foreach (var floor in floors)
            {
                Floors.Add(floor.gameObject);
            }
            Debug.Log($"GetFloors floors:{floors.Length}");
        }
        else
        {
            Debug.Log($"GetFloors buildings:{buildings.Length}");
        }
    }

    [ContextMenu("InitBoxColliders")]
    public void InitBoxColliders()
    {
        //FloorBoxs.Clear();
       for (int i=0;i<Floors.Count;i++)
        {
            if (Floors[i] == null) continue;
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

    private List<GameObject> GetIntersectBoxes(Bounds b1,Func<BoxCollider,Bounds, bool> isIntersectFun)
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

    private List<IntersectInfo> GetIntersectBoxesEx(Transform t)
    {
        Bounds b0 = CaculateBounds(t.gameObject);
        return GetIntersectBoxesEx(b0);
    }

    private List<IntersectInfo> GetIntersectBoxesEx(Bounds b1)
    {
        List<IntersectInfo> result = new List<IntersectInfo>();
        for (int i = 0; i < Floors.Count; i++)
        {
            GameObject floor = Floors[i];
            float pSum = 0;
            BoxCollider[] boxes = floor.GetComponents<BoxCollider>();
            foreach (var box in boxes)
            {
                float p = ColliderExtension.BoundsContainedPercentage(b1, box.bounds);
                pSum += p;
            }
            if (pSum > 0)
            {
                result.Add(new IntersectInfo(floor.transform, pSum));
            }
        }
        result.Sort();
        return result;
    }

    public List<GameObject> GetIntersectBoxes(Transform t)
    {
        Bounds b0 = CaculateBounds(t.gameObject);
        List<GameObject> result = new List<GameObject>();
        result = GetIntersectBoxes(b0, (box, b1) =>{ return (box.bounds.Contains(b1.min) && box.bounds.Contains(b1.max) && box.bounds.Contains(b1.center)); });
        
        if (result.Count == 0)
        {
            result = GetIntersectBoxes(b0, (box, b1) =>{ return (box.bounds.Contains(b1.center)); });
        }        
        if (result.Count == 0)
        {
            result = GetIntersectBoxes(b0, (box, b1) =>{ return (box.bounds.Intersects(b1)); });
        }
        //if (result.Count == 0)
        //{
        //    result = GetBoxes(b0, (box, b1) =>{ return (box.bounds.Contains(b1.min) || box.bounds.Contains(b1.max) || box.bounds.Contains(b1.center)); });
        //}
        return result;
    }

    public List<TransformFloorParent> List00 = new List<TransformFloorParent>();
    public List<TransformFloorParent> List01 = new List<TransformFloorParent>();
    public List<TransformFloorParent> List1 = new List<TransformFloorParent>();
    public List<TransformFloorParent> List2 = new List<TransformFloorParent>();

    public float MinIntersectPercent = 0.5f;

    [ContextMenu("SetToFloorRenderers")]
    public void SetToFloorRenderers()
    {
        DateTime start = DateTime.Now;
        ClearList();
        InitBoxColliders();
        MeshRenderer[] meshRenderers = Sources.GetComponentsInChildren<MeshRenderer>(true);
        MeshRendererInfoList meshRendererInfos = new MeshRendererInfoList(meshRenderers);
        foreach (var renderer in meshRendererInfos)
        {
            Transform t = renderer.transform;
            AddToIntersectList(t);
        }
        SetParentEx();
        Debug.Log($"SetToFloorRenderers time:{DateTime.Now - start} meshRenderers:{meshRenderers.Length}");
    }

    private void ClearList()
    {
        List00.Clear();
        List01.Clear();
        List1.Clear();
        List2.Clear();
    }

    private void AddToIntersectList(Transform t)
    {
        List<IntersectInfo> boxes = GetIntersectBoxesEx(t);
        List<IntersectInfo> boxesIntersect = new List<IntersectInfo>();
        foreach (var b in boxes)
        {
            if (b.percent > MinIntersectPercent)
            {
                boxesIntersect.Add(b);
            }
        }
        if (boxesIntersect.Count == 0)
        {
            if (boxes.Count > 0)
            {
                List01.Add(new TransformFloorParent(t, boxes));
            }
            else
            {
                List00.Add(new TransformFloorParent(t, boxes));
            }
            
        }
        else if (boxesIntersect.Count == 1)
        {
            List1.Add(new TransformFloorParent(t, boxes));
        }
        else
        {
            List2.Add(new TransformFloorParent(t, boxes));
        }

        List01.Sort();
        List1.Sort();
        List2.Sort();
    }

    [ContextMenu("SetToFloorChild")]
    public void SetToFloorChild()
    {
        DateTime start = DateTime.Now;
        ClearList();
        InitBoxColliders();
        //MeshRenderer[] meshRenderers = Sources.GetComponentsInChildren<MeshRenderer>(true);
        for (int i=0;i< Sources.transform.childCount;i++)
        {
            var child = Sources.transform.GetChild(i);
            AddToIntersectList(child.transform);
        }
        SetParentEx();
        Debug.Log($"SetToFloorChild time:{DateTime.Now-start} childCount:{Sources.transform.childCount}");
    }

    public bool IsDebug = false;

    [ContextMenu("SetParent")]
    public void SetParent()
    {
        if (OnlySetOneFloor)
        {
            foreach (TransformFloorParent r in List1)
            {
                r.SetParent(Sources, IsIn, IsDebug);
                if (IsDebug)
                {
                    break;
                }
            }
        }
        else
        {
            foreach (TransformFloorParent r in List1)
            {
                r.SetParent(Sources, IsIn, IsDebug);
                if (IsDebug)
                {
                    break;
                }
            }
            foreach (TransformFloorParent r in List2)
            {
                r.SetParent(Sources, IsIn, IsDebug);
                if (IsDebug)
                {
                    break;
                }
            }
        }
    }

    public bool OnlySetOneFloor = false;

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
public class IntersectInfo:IComparable<IntersectInfo>
{
    public Transform parent
    {
        get
        {
            if (Container == null) return null;
            return Container.parent;
        }
    }

    public string name
    {
        get
        {
            if (Container == null) return "";
            return Container.name;
        }
    }

    public Transform Container;

    public float percent;

    public IntersectInfo(Transform go,float p)
    {
        Container = go;
        percent = p;
    }

    public IntersectInfo(Transform go)
    {
        Container = go;
        percent = 0;
    }

    public override string ToString()
    {
        return $"{name}({parent:P1})";
    }

    public int CompareTo(IntersectInfo other)
    {
        return other.percent.CompareTo(this.percent);
    }
}

[Serializable]
public class TransformFloorParent:IComparable<TransformFloorParent>
{
    public Transform go;

    public IntersectInfo floor;

    public List<IntersectInfo> floors = new List<IntersectInfo>();

    public IntersectInfo GetDownFloor()
    {
        IntersectInfo downFloor = null;
        float height = float.MaxValue;
        foreach(var f in floors)
        {
            if (f.Container.position.y < height)
            {
                height = f.Container.position.y;
                downFloor = f;
            }
        }
        return downFloor;
    }

    public float percent
    {
        get
        {
            if (floor != null)
            {
                return floor.percent;
            }
            else
            {
                return 0;
            }
        }
    }

    public TransformFloorParent(Transform g, IntersectInfo p)
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
        foreach (var f in fs)
        {
            this.floors.Add(new IntersectInfo(f.transform));
        }
        floor = floors[0];
    }

    public TransformFloorParent(Transform g, List<IntersectInfo> fs)
    {
        this.go = g;
        //this.floors = fs;
        foreach (var f in fs)
        {
            this.floors.Add(f);
        }
        if (floors.Count > 0)
        {
            floor = floors[0];
        }
    }

    public string GetFloors()
    {
        string fs = "";
        if (floors.Count == 1)
        {
            IntersectInfo floor = floors[0];
            if (floor != null)
            {
                if (floor.parent != null)
                {
                    fs = $"{floor.parent.name} > {floor.name}({floor.percent:P1})";
                }
                else
                {
                    fs = $"{floor.name}({floor.percent:P1})";
                }
            }
        }
        else
        {
            for (int i = 0; i < floors.Count; i++)
            {
                IntersectInfo f = floors[i];
                if (f == null) continue;
                fs += $"{f.name}({f.percent:P1})";
                if (i < floors.Count - 1)
                {
                    fs += ";";
                }
            }
        }
        return fs;
    }

    public TransformFloorParent()
    {

    }

    public bool IsSameBuilding()
    {
        if (floors.Count <= 1) return true;
        var b1 = floors[0].parent;
        for (int i = 1; i < floors.Count; i++)
        {
            var b2 = floors[i].parent;
            if (b1 != b2)
            {
                return false;
            }
        }
        return true;
    }

    public Transform p;

    public void SetParent(GameObject root, bool isIn, bool isDebug)
    {
        string pName = root.name;
        if (p != null)
        {
            go.SetParent(p.transform);
            return;
        }

        if (IsSameBuilding() == false)
        {
            Debug.LogError("SetParent IsSameBuilding() == false go:{go}");
            return;
        }

        //Transform fP = floor.Container;
        Transform fP = GetDownFloor().Container;
        Transform inP = null;
        for (int i = 0; i < fP.childCount; i++)
        {
            if (fP.GetChild(i).name == "In")
            {
                inP = fP.GetChild(i);
            }
        }
        if (isIn && inP != null)
        {
            fP = inP;
        }

        //for (int i = 0; i < fP.childCount; i++)
        //{
        //    var child = fP.GetChild(i);
        //    if (child.name == pName)
        //    {
        //        p = child;
        //        EditorHelper.UnpackPrefab(go.gameObject);
        //        go.SetParent(child);
        //        return;
        //    }
        //}

        //GameObject newP = new GameObject(pName);
        //newP.transform.position = fP.transform.position;
        //newP.transform.SetParent(fP);
        ////p = newP.transform;
        //EditorHelper.UnpackPrefab(go.gameObject);
        //go.SetParent(newP.transform);

        List<Transform> path = go.transform.GetAncestors(root.transform.parent);
        if (isDebug)
        {
            Debug.LogError($"SetParent go:{go} root:{root} paths:{path.Count} path:{go.transform.GetPath(root.transform)}");
        }

        Transform newP = TransformHelper.FindOrCreatePath(fP, path, isDebug);
        go.SetParent(newP.transform);
    }

    public int CompareTo(TransformFloorParent other)
    {
        return other.percent.CompareTo(this.percent);
    }
}
