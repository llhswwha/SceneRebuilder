using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorBoxManager : SingletonBehaviour<FloorBoxManager>
{
    public GameObject FactoryFrom;//New

    public GameObject FactoryTo;//Old;

    [ContextMenu("CopyFloorCollider")]
    public void CopyFloorCollider()
    {
        Dictionary<string, DepNode> dictOld = new Dictionary<string, DepNode>();
        DepNode[] floorsTo = FactoryTo.GetComponentsInChildren<DepNode>(true);
        DepNode[] floorsFrom = FactoryFrom.GetComponentsInChildren<DepNode>(true);
        foreach(var floor in floorsTo)
        {
            string floorName = floor.name;
            if (floor is FloorController)
            {
                floorName = floor.transform.parent.name + ">" + floor.name;
            }
            if (dictOld.ContainsKey(floorName))
            {
                Debug.LogError($"SetDict dictOld.ContainsKey(floor.name) name:{floorName} path1:{floor.transform.GetPath()} path2:{dictOld[floorName].transform.GetPath()}");
            }
            else
            {
                dictOld.Add(floorName, floor);
            }
        }

        foreach (var floor in floorsFrom)
        {
            string floorName = floor.name;
            if(floor is FloorController)
            {
                floorName = floor.transform.parent.name + ">" + floor.name;
            }
            
            if (dictOld.ContainsKey(floorName))
            {
                DepNode floorTo = dictOld[floorName];
                CopyFloorCollider(floor, floorTo);
            }
            else
            {
                Debug.LogError($"SetCollider !dictOld.ContainsKey(floor.name) name:{floorName} path1:{floor.transform.GetPath()}");
            }
        }

        Debug.Log($"CopyFloorCollider floorsFrom:{floorsFrom.Length} floorsTo:{floorsTo.Length}");
    }

    private void CopyFloorCollider(DepNode fFrom, DepNode fTo)
    {
#if UNITY_EDITOR
        InnerEditorHelper.ClearAndCopyComponents<BoxCollider>(fFrom.gameObject, fTo.gameObject);
#endif

    }


    public bool IsChangeParent = false;
    public bool IsIn = true;
    public List<ModelContainerBox> Floors = new List<ModelContainerBox>();
    public List<ModelContainerBox> Buildings = new List<ModelContainerBox>();

    private void SortFloors()
    {
        foreach(var floor in Floors)
        {
            floor.SortItems();
        }
        foreach (var floor in Buildings)
        {
            floor.SortItems();
        }
    }

    private void ClearFloors()
    {
        foreach (var floor in Floors)
        {
            floor.Clear();
        }
        foreach (var floor in Buildings)
        {
            floor.Clear();
        }
    }

    //public List<BoxCollider> FloorBoxs = new List<BoxCollider>();
    public GameObject Sources;

    ////[ContextMenu("AddFloors")]
    //public void AddFloors()
    //{
    //    var buildings = GameObject.FindObjectsOfType<BuildingController>();
    //    foreach (var b in buildings)
    //    {
    //        Buildings.Add(new FloorBox(b.gameObject));
    //        var floors = b.GetComponentsInChildren<FloorController>();
    //        foreach (var floor in floors)
    //        {
    //            Floors.Add(new FloorBox(floor.gameObject));
    //        }
    //    }
    //}

    //[ContextMenu("ShowNotInFloorsInBuildings")]
    public void ShowNotInFloorsInBuildings()
    {
        foreach (var b in Buildings)
        {
            for(int i = 0; i < b.Items.Count; i++)
            {
                IntersectInfo item = b.Items[i];
                if (b.IsInSubBoxes(item))
                {
                    b.Items.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    //[ContextMenu("GetFloors")]
    public void GetFloors()
    {
        Floors.Clear();
        Buildings.Clear();
        var buildings = GameObject.FindObjectsOfType<BuildingController>();
        foreach(var b in buildings)
        {
            ModelContainerBox buildingBox = new ModelContainerBox(b.gameObject);
            Buildings.Add(buildingBox);
            var floors = b.GetComponentsInChildren<FloorController>();
            foreach(var floor in floors)
            {
                ModelContainerBox floorBox = new ModelContainerBox(floor.gameObject);
                Floors.Add(floorBox);
                buildingBox.SubBoxes.Add(floorBox);
            }
        }

        if (buildings.Length == 0)
        {
            var floors = GameObject.FindObjectsOfType<FloorController>();
            foreach (var floor in floors)
            {
                Floors.Add(new ModelContainerBox(floor.gameObject));
            }
            Debug.Log($"GetFloors floors:{floors.Length}");
        }
        else
        {
            Debug.Log($"GetFloors buildings:{buildings.Length}");
        }
    }

    //[ContextMenu("InitBoxColliders")]
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

    private List<ModelContainerBox> GetIntersectBoxes(Bounds b1,Func<BoxCollider,Bounds, bool> isIntersectFun)
    {
        List<ModelContainerBox> result = new List<ModelContainerBox>();
        for(int i=0;i<Floors.Count;i++)
        {
            ModelContainerBox floor = Floors[i];
            BoxCollider[] boxes = floor.GetBoxColliders();
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

    //private List<IntersectInfo> GetIntersectBoxesEx(Transform t)
    //{
    //    Bounds b0 = CaculateBounds(t.gameObject);
    //    return GetIntersectBoxesEx(b0);
    //}

    private List<IntersectInfo> GetIntersectBoxesEx(Transform t)
    {
        Bounds b1 = CaculateBounds(t.gameObject);
        List<IntersectInfo> result = new List<IntersectInfo>();
        for (int i = 0; i < Floors.Count; i++)
        {
            ModelContainerBox floor = Floors[i];
            float pSum = 0;
            BoxCollider[] boxes = floor.GetBoxColliders();
            foreach (var box in boxes)
            {
                float p = ColliderExtension.BoundsContainedPercentage(b1, box.bounds);
                pSum += p;
            }
            if (pSum > 0)
            {
                result.Add(new IntersectInfo(floor.transform, pSum));
                floor.AddModel(new IntersectInfo(t, pSum));
            }
        }

        for (int i = 0; i < Buildings.Count; i++)
        {
            ModelContainerBox building = Buildings[i];
            float pSum = 0;
            BoxCollider[] boxes = building.GetBoxColliders();
            foreach (var box in boxes)
            {
                float p = ColliderExtension.BoundsContainedPercentage(b1, box.bounds);
                pSum += p;
            }
            if (pSum > 0)
            {
                //result.Add(new IntersectInfo(floor.transform, pSum));
                building.AddModel(new IntersectInfo(t, pSum));
            }
        }

        result.Sort();
        return result;
    }

    public List<ModelContainerBox> GetIntersectBoxes(Transform t)
    {
        Bounds b0 = CaculateBounds(t.gameObject);
        List<ModelContainerBox> result = new List<ModelContainerBox>();
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

    //[ContextMenu("SetToFloorRenderers")]
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
        SortFloors();
        Debug.Log($"SetToFloorRenderers time:{DateTime.Now - start} meshRenderers:{meshRenderers.Length}");
    }

    private void ClearList()
    {
        ClearFloors();
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

    //[ContextMenu("SetToFloorChild")]
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
        SortFloors();
        Debug.Log($"SetToFloorChild time:{DateTime.Now-start} childCount:{Sources.transform.childCount}");
    }

    public bool IsDebug = false;

    //[ContextMenu("DeleteInFloors")]
    public void DeleteInFloors()
    {
        //foreach (TransformFloorParent r in List01)
        //{
        //    GameObject.DestroyImmediate(r.go);
        //}
        foreach (TransformFloorParent r in List1)
        {
            GameObject.DestroyImmediate(r.go.gameObject);
        }
        foreach (TransformFloorParent r in List2)
        {
            GameObject.DestroyImmediate(r.go.gameObject);
        }

        Debug.Log($"DeleteInFloors List1:{List1.Count} List2:{List2.Count} List01:{List01.Count}");
    }

    public void SetParent(TransformFloorParent r)
    {
        r.SetParent(Sources, IsIn, IsDebug);
    }

    private void GetParentPath()
    {
        foreach (TransformFloorParent r in List1)
        {
            r.GetParentPath(Sources);
        }
        foreach (TransformFloorParent r in List2)
        {
            r.GetParentPath(Sources);
        }
    }

    //[ContextMenu("SetParent")]
    public void SetParent()
    {
        GetParentPath();

        if (OnlySetOneFloor)
        {
            foreach (TransformFloorParent r in List1)
            {
                SetParent(r);
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
                SetParent(r);
                if (IsDebug)
                {
                    break;
                }
            }
            foreach (TransformFloorParent r in List2)
            {
                SetParent(r);
                if (IsDebug)
                {
                    break;
                }
            }
        }
    }

    public bool OnlySetOneFloor = false;

    //[ContextMenu("SetParentEx")]
    public void SetParentEx()
    {
        if (IsChangeParent)
        {
            SetParent();
        }
    }

    public GameObject TestGo;

    public GameObject TestFloor;

    //[ContextMenu("TestParent")]
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
            if (IntersectGo == null) return null;
            return IntersectGo.parent;
        }
    }

    public string name
    {
        get
        {
            if (IntersectGo == null) return "";
            return IntersectGo.name;
        }
    }

    public Transform IntersectGo;

    public float percent;

    public IntersectInfo(Transform go,float p)
    {
        IntersectGo = go;
        percent = p;
    }

    public IntersectInfo(Transform go)
    {
        IntersectGo = go;
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
public class ModelContainerBox
{
    public GameObject go;

    public List<IntersectInfo> Items = new List<IntersectInfo>();

    public List<ModelContainerBox> SubBoxes = new List<ModelContainerBox>();

    private Dictionary<Transform, IntersectInfo> Dict = new Dictionary<Transform, IntersectInfo>();

    public void Clear()
    {
        Items = new List<IntersectInfo>();
        //SubBoxes = new List<ModelContainerBox>();
        Dict = new Dictionary<Transform, IntersectInfo>();
    }

    public bool IsInSubBoxes(IntersectInfo item)
    {
        foreach (var f in SubBoxes)
        {
            if (f.Contains(item))
            {
                return true;
            }
        }
        return false;
    }


    public List<IntersectInfo> GetItemList(float percent)
    {
        if (Items == null) return new List<IntersectInfo>();
        return Items.FindAll(i => i.percent > percent);
    }

    public int GetCount(float percent)
    {
        if (Items == null) return 0;
        else
        {
            return Items.FindAll(i => i.percent > percent).Count;
        }
    }

    public void AddModel(IntersectInfo item)
    {
        if (Items == null)
        {
            Items = new List<IntersectInfo>();
        }
        
        if (Dict.ContainsKey(item.IntersectGo))
        {
            Debug.LogError($"AddModel Dict.ContainsKey(item.IntersectGo) Box:{this.name} item:{item.IntersectGo.name}");
        }
        else
        {
            Dict.Add(item.IntersectGo, item);
            Items.Add(item);
        }
        
    }

    public void RemoveModel(IntersectInfo item)
    {
        if (Dict.ContainsKey(item.IntersectGo))
        {
            IntersectInfo item2 = Dict[item.IntersectGo];
            Items.Remove(item2);
            Dict.Remove(item.IntersectGo);
        }
    }

    public bool Contains(IntersectInfo item)
    {
        return Dict.ContainsKey(item.IntersectGo);
    }

    public ModelContainerBox(GameObject go)
    {
        this.go = go;
    }

    public BoxCollider GetBoxCollider()
    {
        return go.GetBoxCollider();
    }

    public BoxCollider[] GetBoxColliders()
    {
        return go.GetComponents<BoxCollider>();
    }

    internal void SortItems()
    {
        if (Items != null)
        {
            Items.Sort();
        }
    }

    public Transform transform
    {
        get
        {
            if (go == null) return null;
            return go.transform;
        }
    }

    public string name
    {
        get
        {
            if (go == null) return "";
            return go.name;
        }
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
            if (f.IntersectGo.position.y < height)
            {
                height = f.IntersectGo.position.y;
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

    List<string> parentPath = new List<string>();

    public void GetParentPath(GameObject root)
    {
        parentPath= go.transform.GetAncestorNames(root.transform.parent);
    }

    public void SetParent(GameObject root, bool isIn, bool isDebug)
    {
        Transform parent = go.transform.parent;
        if (parent != null)
        {
            MeshRenderer mr = parent.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                return;
            }
        }
        string pName = root.name;
        if (p != null)
        {
            go.SetParent(p.transform);
            return;
        }
        Transform fP = floor.IntersectGo;
        //if (IsSameBuilding() == false)
        //{
        //    Debug.LogError("SetParent IsSameBuilding() == false go:{go}");
        //    return;
        //}
        if (IsSameBuilding())
        {
            fP = GetDownFloor().IntersectGo;
        }

        //Transform fP = floor.Container;

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

        //List<string> path = go.transform.GetAncestorNames(root.transform.parent);
        List<string> path = parentPath;
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
