using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonUtils;
using MeshJobs;
using UnityEngine;

[Serializable]
public class DoorsRootList: List<DoorsRoot>
{
    //public DoorInfoList Doors = new DoorInfoList();
    public int VertexCount = 0;
    public int VertexCount_Show = 0;


    public DoorsRootList()
    {
        
    }

    //public DoorInfoList(List<MeshRenderer> renderers)
    //{
    //    GetDoors(renderers);
    //}

    public DoorsRootList(DoorsRoot[] doorsRoots, Action<ProgressArg> progressChanged)
    {
        GetDoors(doorsRoots, progressChanged);
    }

    private void GetDoors(DoorsRoot[] doorsRoots,Action<ProgressArg> progressChanged)
    {
        //Debug.Log($"GetDoors roots:{doorsRoots.Length}");
        //Doors.Clear();
        VertexCount = 0;
        VertexCount_Show = 0;
        DateTime start = DateTime.Now;
        //ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", "Start", 0);
        //var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true).Where(i => i.name.ToLower().Contains("door")).ToList();
        for (int i = 0; i < doorsRoots.Length; i++)
        {
            var doorRoot = doorsRoots[i];
            var p = new ProgressArg("GetDoors", i, doorsRoots.Length, doorRoot);
            float progress = (float)i / doorsRoots.Length;
            if (progressChanged != null)
            {
                progressChanged(p);
            }
            else
            {
                ProgressBarHelper.DisplayCancelableProgressBar(p);
            }
            
            //var parent = doorsRoots[i].transform.parent;
            ////if (parent != null && parent.name.ToLower().Contains("combined")) continue;

            //doorRoot.Init();

            this.Add(doorRoot);
            VertexCount += doorRoot.Doors.VertexCount;
            if (doorRoot.gameObject.activeInHierarchy)
            {
                VertexCount_Show += doorRoot.Doors.VertexCount;
            }
        }
        this.Sort((a, b) =>
        {
            return b.Doors.VertexCount.CompareTo(a.Doors.VertexCount);
        });

        if (progressChanged != null)
        {
            progressChanged(new ProgressArg("GetDoors", doorsRoots.Length, doorsRoots.Length, null));
        }
        else
        {
            ProgressBarHelper.ClearProgressBar();
        }

        //
        Debug.Log($"GetDoors count:{doorsRoots.Length} VertexCount:{VertexCount} time:{(DateTime.Now - start)}");
        //return Doors;
    }

    //public void GetDoors(List<MeshRenderer> renderers)
    //{
    //    //Doors.Clear();
    //    VertexCount = 0;
    //    VertexCount_Show = 0;
    //    DateTime start = DateTime.Now;
    //    ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", "Start", 0);
    //    //var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true).Where(i => i.name.ToLower().Contains("door")).ToList();
    //    for (int i = 0; i < renderers.Count; i++)
    //    {

    //        float progress = (float)i / renderers.Count;
    //        ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", $"{i}/{renderers.Count} {progress:P1}", progress);
    //        var parent = renderers[i].transform.parent;
    //        if (parent != null && parent.name.ToLower().Contains("combined")) continue;
    //        DoorInfo door = new DoorInfo(renderers[i]);
    //        this.Add(door);
    //        VertexCount += door.VertexCount;
    //        if (renderers[i].gameObject.activeInHierarchy)
    //        {
    //            VertexCount_Show += door.VertexCount;
    //        }
    //    }
    //    ProgressBarHelper.ClearProgressBar();
    //    Debug.Log($"GetDoors count:{renderers.Count} VertexCount:{VertexCount} time:{(DateTime.Now - start)}");
    //    //return Doors;
    //}

    public void ApplyReplace()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].ApplyReplace();
        }
    }

    public void RevertReplace()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].RevertReplace();
        }
    }

    public void ShowOri()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].ShowOri();
        }
    }

    public void ShowNew()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].ShowNew();
        }
    }

    public DoorInfoList GetDoors()
    {
        DoorInfoList doors = new DoorInfoList();
        Dictionary<GameObject, DoorInfo> doorDict = new Dictionary<GameObject, DoorInfo>();
        foreach(var root in this)
        {
            if (root == null) continue ;
            if (root.gameObject == null) continue;
            EditorHelper.UnpackPrefab(root.gameObject);
            //doors.AddRange(root.Doors);
            foreach(var door in root.Doors)
            {
                if (door.gameObject == null) continue;
                if(!doorDict.ContainsKey(door.gameObject))
                {
                    doors.Add(door);
                    doorDict.Add(door.gameObject, door);
                }
            }
        }
        return doors;
    }

    internal void Split()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].Split();
        }
    }

    internal void SetDoorPivot()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].SetDoorPivot();
        }
    }

    internal void SetLOD()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].SetLOD();
        }
    }

    internal void CopyPart()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].CopyPart();
        }
    }

    internal void Prepare()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].Prepare();
        }
    }

    public void SetParent(Transform p)
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].transform.SetParent(p);
        }
    }
}

[Serializable]
public class DoorPartInfo
{
    public string Root;
    public GameObject gameObject;

    public Vector3 Pos;

    public Vector3 Center;

    public Vector3 Size;

    public Vector3[] minMax;

    public float DisToCenter;

    public Vector3 OffToCenter;

    public int MatCount;

    public string MatNames;

    public int VertexCount;

    public int SubMeshCount;

    public LODGroup lodGroup;

    public DoorPartInfo()
    {

    }

    public Vector3 localScale
    {
        get
        {
            return gameObject.transform.localScale;
        }
    }

    public Transform transform
    {
        get
        {
            return gameObject.transform;
        }
    }

    public string name
    {
        get
        {
            return gameObject.name;
        }
    }

    public string GetTitle()
    {
        if (gameObject == null) return "NULL";
        string isLOD = "LOD";
        if (lodGroup == null)
        {
            isLOD = "";
        }
        if (gameObject.transform.parent != null)
        {
            return $"{gameObject.transform.parent.name}>{gameObject.name}({transform.childCount}){isLOD}";
        }
        else
        {
            return $"{gameObject.name}({transform.childCount}){isLOD}";
            //return $"{Root}>{gameObject.name}";
        }
        //return $"{Root}>{gameObject.name}";
    }

    public DoorPartInfo(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("DoorPartInfo.ctor go == null");
        }
        BuildingModelInfo[] models = go.GetComponentsInParent<BuildingModelInfo>(true);
        if (models.Length > 0)
        {
            Root = models[0].name;
        }
        gameObject = go;
        Pos = go.transform.position;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf)
        {
            minMax = VertexHelper.GetMinMax(mf);
            Center = minMax[3];
            Size = minMax[2];
            DisToCenter = Vector3.Distance(Pos, Center);
            OffToCenter = Center - Pos;
            if (mf.sharedMesh == null)
            {
                Debug.LogError($"DoorPartInfo mf.sharedMesh == null mf2:{mf}");
            }
            else
            {
                VertexCount = mf.sharedMesh.vertexCount;
                SubMeshCount = mf.sharedMesh.subMeshCount;
            } 
        }
        else
        {
            var filters = go.GetComponentsInChildren<MeshFilter>(true);

            minMax = VertexHelper.GetMinMax(filters);
            Center = minMax[3];
            Size = minMax[2];
            DisToCenter = Vector3.Distance(Pos, Center);
            OffToCenter = Center - Pos;

            foreach (var mf2 in filters)
            {
                if (mf2 == null) continue;
                if (mf2.sharedMesh == null)
                {
                    BuildingController b = go.GetComponentInParent<BuildingController>();
                    FloorController f = go.GetComponentInParent<FloorController>();
                    Debug.LogError($"DoorPartInfo mf2.sharedMesh == null mf2:{mf2} building:{b} floor:{f}");
                    break;
                }
                VertexCount += mf2.sharedMesh.vertexCount;
            }
            SubMeshCount = filters.Length;
        }
        

        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        if (renderer)
        {
            MatCount = renderer.sharedMaterials.Length;
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat == null) continue;
                MatNames += mat.name + ";";
            }
        }
        else
        {
            //Debug.LogWarning("DoorPartInfo.ctor MeshRenderer == null:" + go);
            var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
            MatCount = renderers.Length;
        }

        var meshRenderers = MeshRendererInfo.InitRenderers(go);
        meshRenderers.AddType(MeshRendererType.Static);

        lodGroup = go.GetComponent<LODGroup>();
    }



    public override string ToString()
    {
        //return $"mat:{MatCount},mesh:{SubMeshCount},v:{VertexCount},dis:{DisToCenter:F1},off:({OffToCenter.x:F2},{OffToCenter.y:F2},{OffToCenter.z:F2})";
        return $"mat:{MatCount},mesh:{SubMeshCount},v:{VertexCount},dis:{DisToCenter:F2},size:{Size}";
    }

    internal void SetLOD()
    {
        if (this.gameObject == null)
        {
            Debug.LogError("DoorPartInfo.SetDoorLOD this.gameObject == null door:" + this);
            return;
        }
        LODHelper.SetDoorLOD(this.gameObject);
    }

    internal void SetDoorPivot(bool isForce=false)
    {
        if (DisToCenter > 0.2 && isForce==false)
        {
            Debug.LogWarning($"SetDoorPivot No1 name:{this.name} dis:{DisToCenter:F4} pos:{transform.position} center:{this.Center}");
            return;
        }
        if (transform.childCount==0)
        {
            if(MatCount > 1)
            {
                Debug.LogWarning($"SetDoorPivot Yes0 name:{this.name} dis:{DisToCenter:F4} pos:{transform.position} center:{this.Center}");
                Split();
            }
            else
            {
                Debug.LogWarning($"SetDoorPivot No3 name:{this.name} dis:{DisToCenter:F4} pos:{transform.position} center:{this.Center}");
                return;
            }
        }
        SetPivot();
    }

    private void SetPivot()
    {
        Vector3 pivotPos1 = GetPivotPosByChildren();
        if (pivotPos1 != this.transform.position)
        {
            MeshHelper.CenterPivot(this.transform, pivotPos1);
            Debug.Log($"SetDoorPivot Yes1 name:{this.name} dis:{DisToCenter:F4} pos:{transform.position} center:{this.Center}");
        }
        else
        {
            Debug.LogWarning($"SetDoorPivot Yes2 name:{this.name} dis:{DisToCenter:F4} pos:{transform.position} center:{this.Center}");
            Vector3 pivotPos2 = GetPivotPosByMeshDis();
            MeshHelper.CenterPivot(this.transform, pivotPos2);
        }
    }

    public Vector3 GetPivotPosByMeshDis()
    {
        var size = minMax[2];
        //var wVertices = MeshHelper.GetWorldVertexes(gameObject);
        MeshRendererInfoList list = new MeshRendererInfoList(gameObject);
        var maxRenderer = list[0];
        var maxRendererCenter = maxRenderer.GetWeightCenterPos();

        Vector3 p1 = Center - new Vector3(size.x / 2, 0, 0);
        Vector3 p2 = Center + new Vector3(size.x / 2, 0, 0);
        Vector3 p3 = Center - new Vector3(0, 0, size.z / 2);
        Vector3 p4 = Center + new Vector3(0, 0, size.z / 2);
        Vector3[] ps = new Vector3[] { p1, p2, p3, p4 };
        Vector3 pivot = ps[0];
        float maxDis = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            Vector3 p = ps[i];
            var dis = Vector3.Distance(p, maxRendererCenter);
            if (dis > maxDis)
            {
                maxDis = dis;
                pivot = p;
            }
            //CreatePoint(p, $"p{i + 1}:{p} dis:{dis}");
        }
        return pivot;
    }

        private void CreatePoint(Vector3 pos, string name)
    {
        GameObject centerGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerGo.name = name;
        centerGo.transform.position = pos;
        centerGo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        centerGo.transform.SetParent(this.transform);
    }

    public Vector3 GetPivotPosByChildren()
    {
        List<Transform> children = new List<Transform>();
        Vector3 pivotPos = transform.GetChild(0).position;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            children.Add(child);
            float dis = Vector3.Distance(pivotPos, child.position);//3.814697E-05
            if (dis>0.00015f)
            {
                Debug.LogError($"GetPivotPos name:{this.name} pos:{this.transform.position} pivotPos:{pivotPos} childPos:{child.position} dis:{dis}");
                pivotPos = this.transform.position;
                break;
            }
        }
        return pivotPos;
    }

    internal void Split()
    {
        this.gameObject=MeshCombineHelper.SplitByMaterials(this.gameObject,false);
    }
}

[Serializable]
public class DoorPartInfoList:List<DoorPartInfo>
{
    public int VertexCount = 0;
    public int VertexCount_Show = 0;
    public int lodCount = 0;

    public void AddEx(DoorPartInfo door)
    {
        base.Add(door);
        VertexCount += door.VertexCount;
        if (door.gameObject && door.gameObject.activeInHierarchy)
            VertexCount_Show += door.VertexCount;
        if (door.lodGroup != null)
        {
            lodCount++;
        }
    }
}

[Serializable]
public class DoorInfoList : List<DoorInfo>
{
    public int VertexCount = 0;
    public int VertexCount_Show = 0;
    public DoorInfoList()
    {

    }

    public DoorInfoList(DoorInfoList list)
    {
        this.AddRange(list);
    }

    internal MeshPoints[] GetDoorMeshPoints()
    {
        MeshPoints[] meshPoints = new MeshPoints[this.Count];
        for(int i = 0; i < this.Count; i++)
        {
            meshPoints[i] = new MeshPoints(this[i].gameObject);
        }
        return meshPoints;
    }

    public List<MeshFilter> GetMeshFilters()
    {
        return GetComponents<MeshFilter>();
    }
    public List<MeshRenderer> GetMeshRenderers()
    {
        return GetComponents<MeshRenderer>();
    }

    public List<GameObject> GetGameObjects()
    {
        List<GameObject> gos = new List<GameObject>();
        foreach (var door in this)
        {
            gos.Add(door.gameObject);
        }
        return gos;
    }

    public List<T> GetComponents<T>() where T :Component
    {
        List<T> components = new List<T>();
        for (int i = 0; i < this.Count; i++)
        {
            DoorInfo info = this[i];
            if (info == null) continue;
            if (info.gameObject == null) continue;
            var mfs = info.gameObject.GetComponentsInChildren<T>(true);
            //Debug.Log($"[{i}/{this.Count}]GetMeshFilters meshFilters:{meshFilters.Count} go:{info.gameObject} mfs:{mfs.Length}");
            //meshFilters.AddRange(mfs);
            foreach(var mf in mfs)
            {
                if (!components.Contains(mf))
                {
                    components.Add(mf);
                }
            }
            
        }
        return components;
    }

    public MeshPoints[] GetFilterMeshPoints()
    {
        List<MeshPoints> meshPoints = new List<MeshPoints>();
        List<MeshFilter> meshFilters = GetMeshFilters();
        foreach (var mf in meshFilters)
        {
            meshPoints.Add(new MeshPoints(mf.gameObject));
        }
        return meshPoints.ToArray();
    }

    public DoorPartInfoList GetDoorParts()
    {
        DoorPartInfoList list = new DoorPartInfoList();
        for (int i = 0; i < this.Count; i++)
        {
            list.AddRange(this[i].DoorParts);
        }
        return list;
    }

    internal void Prepare()
    {
        DateTime start = DateTime.Now;
        for (int i = 0; i < this.Count; i++)
        {
            ProgressBarHelper.DisplayCancelableProgressBar("Prepare", i, this.Count);
            this[i].PreparePrefab();
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"DoorInfoList.Prepare count:{ this.Count} time:{(DateTime.Now-start).TotalMilliseconds}ms");
    }

    internal void SetLOD()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].SetLOD();
        }
        Debug.Log($"DoorInfoList.SetLOD count:{ this.Count}");
    }

    internal void CopyPart()
    {
        for (int i = 0; i < this.Count; i++)
        {
            ProgressBarHelper.DisplayCancelableProgressBar("CopyPart", i, this.Count);
            //DoorHelper.CopyDoorA(this[i].gameObject, true);
            this[i].CopyPart1();
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"DoorInfoList.CopyPart count:{ this.Count}");
    }

    internal void Split()
    {
        //var parts = this.GetDoorParts();
        //DoorManager.SplitDoorParts(parts);

        for (int i = 0; i < this.Count; i++)
        {
            this[i].Split();
        }
        Debug.Log($"DoorInfoList.Split count:{ this.Count}");
    }

    internal void SetDoorPivot()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].SetDoorPartPivot();
        }
    }
}

[Serializable]
public class DoorInfo: IPrefab<DoorInfo>
{
    public string Root;
    private GameObject go;
    public GameObject gameObject
    {
        get
        {
            return go;
        }
        set
        {
            go = value;
        }
    }
    public int vertexCount;
    public int GetVertexCount()
    {
        if (vertexCount == 0)
        {
            foreach(var part in DoorParts)
            {
                vertexCount += part.VertexCount;
            }
        }
        return vertexCount;
    }
    public List<DoorPartInfo> DoorParts = new List<DoorPartInfo>();
    public string name;

    public Transform transform
    {
        get
        {
            return gameObject.transform;
        }
    }

    public List<MeshFilter> meshFilters = new List<MeshFilter>();

    public List<MeshFilter> GetMeshFilters()
    {
        meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true).ToList();
        return meshFilters;
    }

    public Vector3 localScale
    {
        get
        {
            return gameObject.transform.localScale;
        }
    }

    public DoorInfo(GameObject root)
    {
        this.name = root.name;
        this.go = root.gameObject;

        BuildingModelInfo[] models = root.gameObject.GetComponentsInParent<BuildingModelInfo>(true);
        if (models.Length > 0)
        {
            Root = models[0].name;
        }

        InitParts(root);
    }

    private void InitParts(GameObject root)
    {
        if ((transform.childCount != 2 && transform.childCount != 1) || root.GetComponent<LODGroup>() != null)
        {
            DoorPartInfo doorPart = new DoorPartInfo(root);
            DoorParts.Add(doorPart);
            vertexCount += doorPart.VertexCount;
        }
        else
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                var child = root.transform.GetChild(i);
                MeshRendererInfo info = child.GetComponent<MeshRendererInfo>();
                if (info != null && info.IsRendererType(MeshRendererType.Splited)) continue;
                DoorPartInfo doorPart = new DoorPartInfo(child.gameObject);
                DoorParts.Add(doorPart);
                vertexCount += doorPart.VertexCount;
            }
            if (DoorParts.Count == 0)
            {
                DoorPartInfo doorPart = new DoorPartInfo(root);
                DoorParts.Add(doorPart);
                vertexCount += doorPart.VertexCount;
            }
        }

        //MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        //foreach(var renderer in renderers)
        //{
        //    DoorPartInfo doorPart = new DoorPartInfo(renderer);
        //    DoorParts.Add(doorPart);
        //    VertexCount += doorPart.VertexCount;
        //}
        DoorParts.Sort(
            (a, b) =>
            { return b.VertexCount.CompareTo(a.VertexCount); }
            );
        //Debug.Log($"DoorInfo door:{root.name} parts:{DoorParts.Count} VertexCount:{VertexCount}");
    }

    public string GetTitle()
    {
        if (gameObject == null) return "NULL";
        return $"{Root}>{gameObject.name}({DoorParts.Count})";
    }

    public override string ToString()
    {
        //return $"{this.name}(v:{MeshHelper.GetVertexCountS(vertexCount)})";
        return $"{this.name}(v:{vertexCount})";
    }

    public void Split()
    {
        if (DoorParts.Count == 1 && DoorParts[0].gameObject==this.gameObject)
        {
            DoorParts[0].Split();
            this.gameObject = DoorParts[0].gameObject;
        }
        else
        {
            for (int i = 0; i < DoorParts.Count; i++)
            {
                DoorPartInfo part = DoorParts[i];
                if (part.gameObject == null)
                {
                    Debug.LogError($"DoorInfo.Split part.gameObject == null door:{this.name} part:{part} id:{i}");
                    continue;
                }
                part.Split();
            }
        }
        
    }

    public void SetLOD()
    {
        for (int i = 0; i < DoorParts.Count; i++)
        {
            DoorPartInfo part = DoorParts[i];
            if (part.gameObject == null)
            {
                Debug.LogError($"DoorInfo.SetDoorLOD part.gameObject == null door:{this.name} part:{part} id:{i}");
                continue;
            }
            part.SetLOD();
        }
    }

    internal void SetDoorPartPivot(bool isForce = false)
    {
        foreach (var part in DoorParts)
        {
            part.SetDoorPivot(isForce);
        }
    }

    public void CopyPart1()
    {
        DoorHelper.CopyDoorA(this.gameObject, true);
    }

    public void PreparePrefab()
    {
        Split();
        if (this.gameObject == null)
        {
            Debug.LogError("DoorInfo.PreparePrefab1 this.gameObject==null:" + this.name);
        }
        SetLOD();
        if (this.gameObject == null)
        {
            Debug.LogError("DoorInfo.PreparePrefab2 this.gameObject==null:" + this.name);
        }
        CopyPart1();
        if (this.gameObject == null)
        {
            Debug.LogError("DoorInfo.PreparePrefab3 this.gameObject==null:" + this.name);
        }
    }

    public DoorInfo Clone()
    {
        if (this.gameObject == null)
        {
            Debug.LogError("DoorInfo.Clone this.gameObject==null:" + this.name);
            return null;
        }
        GameObject doorNew = MeshHelper.CopyGO(this.gameObject);
        return new DoorInfo(doorNew);
    }
}