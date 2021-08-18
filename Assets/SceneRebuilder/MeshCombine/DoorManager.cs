using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MeshJobs;
using UnityEngine;

public class DoorManager : SingletonBehaviour<DoorManager>
{
    // private static DoorManager _instance;

    // public static DoorManager Instance
    // {
    //     get
    //     {
    //         if (_instance == null)
    //         {
    //             _instance = GameObject.FindObjectOfType<DoorManager>();
    //         }
    //         return _instance;
    //     }
    // }

    public GameObject LocalTarget = null;

    public bool IsOnlyActive = false;

    public bool IsOnlyCanSplit = false;

    public DoorsRootList doorRoots;

    public DoorsRootList UpdateDoors()
    {
        //MeshRenderer[] renderers = null;
        DoorsRoot[] items = null;
        if (LocalTarget != null)
        {
            //items= LocalTarget.GetComponentsInChildren<DoorsRoot>(true);
            //if(items.Length == 0)
            {
                var ts = LocalTarget.GetComponentsInChildren<Transform>(true);
                items = InitDoorsRoot(ts);
            }
            //renderers = LocalTarget.GetComponentsInChildren<MeshRenderer>(true);
        }
        else
        {
            //renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);

            //items = GameObject.FindObjectsOfType<DoorsRoot>(true);
            //if (items.Length == 0)
            {
                var ts = GameObject.FindObjectsOfType<Transform>(true);
                items = InitDoorsRoot(ts);
                //Debug.Log("UpdateDoors ");
            }
        }
        //var rendererList = renderers.Where(i => i.name.ToLower().Contains("door")).ToList();
        doorRoots = new DoorsRootList(items);
        Debug.Log($"DoorManager.UpdateDoors LocalTarget:[{LocalTarget}] doorRoots:{doorRoots.Count}");
        return doorRoots;
    }

    public PrefabInfoList prefabs;

    public void GetPrefabs()
    {
        var doors = doorRoots.GetDoors();
        prefabs = DoorHelper.SetDoorShared(doors);
    }

    public void Split()
    {
        doorRoots.Split();
    }

    public void SetLOD()
    {
        doorRoots.SetLOD();
    }

    public void CopyPart()
    {
        doorRoots.CopyPart();
    }

    public void Prepare()
    {
        doorRoots.Prepare();
    }

    public void AcRTAlignJobs()
    {
        var doors = doorRoots.GetDoors();
        var meshPoints = doors.GetFilterMeshPoints();
        PrefabInstanceBuilder.Instance.AcRTAlignJobs(meshPoints);
    }

    public void AcRTAlignJobsEx()
    {
        var doors = doorRoots.GetDoors();
        var meshPoints = doors.GetFilterMeshPoints();
        PrefabInstanceBuilder.Instance.AcRTAlignJobsEx(meshPoints);
    }

    public void ApplyReplace()
    {
        prefabs.ApplyReplace();
    }

    public void RevertReplace()
    {
        prefabs.ApplyReplace();
    }

    public void ShowOri()
    {
        prefabs.ShowOri();
    }

    public void ShowNew()
    {
        prefabs.ShowNew();
    }

    public SharedMeshInfoList GetSharedMeshList()
    {
        UpdateDoors();
        var doors = doorRoots.GetDoors();
        var filters = doors.GetMeshFilters();
        Debug.Log($"GetSharedMeshList roots:{doorRoots.Count} doors:{doors.Count} filters:{filters.Count}");
        return new SharedMeshInfoList(filters);
    }

    private DoorsRoot[] InitDoorsRoot(Transform[] ts)
    {
        List<DoorsRoot> list = new List<DoorsRoot>();
        var doorsList = ts.Where(i => i.name.ToLower().Contains("doors")).ToList();
        Debug.Log($"InitDoorsRoot ts:{ts.Length} doorsRoots:{doorsList.Count}");
        foreach (var doors in doorsList)
        {
            var root = doors.gameObject.GetComponent<DoorsRoot>();
            if (root == null)
            {
                root = doors.gameObject.AddComponent<DoorsRoot>();
            }
            root.Init();
            list.Add(root);
        }
        return list.ToArray();
    }

    public DoorInfoList GetDoors()
    {
        DoorInfoList doorsList = new DoorInfoList();
        foreach (var doorRoot in doorRoots)
        {
            if (doorRoot == null) continue;
            foreach (var door in doorRoot.Doors)
            {
                if (door == null) continue;
                doorsList.Add(door);

                doorsList.VertexCount += door.GetVertexCount();
                if (door.gameObject && door.gameObject.activeInHierarchy)
                    doorsList.VertexCount_Show += door.GetVertexCount();
            }
        }
        doorsList.Sort((a, b) =>
        {
            return b.GetVertexCount().CompareTo(a.GetVertexCount());
        });
        //Debug.Log($"DoorManager.GetDoorParts IsOnlyActive:{IsOnlyActive} IsOnlyCanSplit:{IsOnlyCanSplit} doorRoots:{doorRoots.Count} doorParts:{doorParts.Count}");
        return doorsList;
    }

    public DoorPartInfoList GetDoorParts()
    {
        DoorPartInfoList doorParts = new DoorPartInfoList();
        foreach(var doorRoot in doorRoots)
        {
            if (doorRoot == null) continue;
            foreach(var doors in doorRoot.Doors)
            {
                if (doors == null) continue;
                foreach (var door in doors.DoorParts)
                {
                    if (door == null) continue;
                    if (IsOnlyActive)
                    {
                        if (door.gameObject && door.gameObject.activeInHierarchy == false) continue;
                    }
                    if (IsOnlyCanSplit)
                    {
                        if (door.SubMeshCount <= 1) continue;
                    }
                    doorParts.Add(door);

                    doorParts.VertexCount += door.VertexCount;
                    if (door.gameObject && door.gameObject.activeInHierarchy)
                        doorParts.VertexCount_Show += door.VertexCount;
                }
            }
        }
        doorParts.Sort((a, b) =>
        {
            return b.VertexCount.CompareTo(a.VertexCount);
        });
        //Debug.Log($"DoorManager.GetDoorParts IsOnlyActive:{IsOnlyActive} IsOnlyCanSplit:{IsOnlyCanSplit} doorRoots:{doorRoots.Count} doorParts:{doorParts.Count}");
        return doorParts;
    }

    public void SplitDoors(GameObject root)
    {
        IsOnlyCanSplit = true;
        LocalTarget = root;
        UpdateDoors();
        SplitAll();
    }

    public void SplitAll()
    {
        var doorParts = GetDoorParts();
        SplitDoorParts(doorParts);
    }

    public static void SplitDoorParts(DoorPartInfoList doorParts)
    {
        for (int i = 0; i < doorParts.Count; i++)
        {
            var door = doorParts[i];
            float progress = (float)i / doorParts.Count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("CombinedBuildings", $"Progress1 {i}/{doorParts.Count} {percents:F2}%  {door.gameObject.name}", progress))
            {
                break;
            }
            GameObject result = MeshCombineHelper.SplitByMaterials(door.gameObject,false);
            MeshRendererInfo.InitRenderers(result);
        }
        ProgressBarHelper.ClearProgressBar();
    }
}

public static class DoorHelper
{
    //public static void SetDoorLOD(GameObject door)
    //{
    //    LODGroup group = door.GetComponent<LODGroup>();
    //    if (group != null)
    //    {
    //        GameObject.DestroyImmediate(group);
    //    }
    //    GameObject doorRoot = door;
    //    MeshRenderer meshRenderer = door.GetComponent<MeshRenderer>();
    //    if (meshRenderer != null)
    //    {
    //        doorRoot = MeshCombineHelper.SplitByMaterials(door);
    //    }

    //    MeshRendererInfoList infoList = MeshRendererInfo.InitRenderers(doorRoot);
    //    if (infoList.Count == 3)
    //    {
    //        LODGroup groupNew = door.AddComponent<LODGroup>();
    //        var lods2 = LODManager.Instance.GetLODs3(infoList.GetRenderers().ToArray(), new MeshRenderer[] { infoList[0].meshRenderer, infoList[1].meshRenderer }, new MeshRenderer[] { infoList[1].GetBoundsRenderer() });
    //        groupNew.SetLODs(lods2);
    //    }
    //    else
    //    {
    //        Debug.LogError("DoorHelper.SetDoorLOD infoList.Count != 3");
    //    }
    //}

    public static PrefabInfoList SetDoorShared(DoorInfoList doors)
    {
        return PrefabInfoListHelper.GetPrefabInfos(doors,false);

        //DoorInfoList list1 = new DoorInfoList(doors);
        //DoorInfoList list2 = new DoorInfoList(doors);

        //DateTime start = DateTime.Now;

        //int allCount = 0;
        //for (int i = 0; i < doors.Count; i++)
        //{
        //    allCount += doors.Count - 1 - i;
        //}

        //int count = 0;
        //int meshAlignCount = 0;
        //int posAlinCount = 0;
        //int noAlignCount = 0;


        //PrefabInfoList prefabsNew = new PrefabInfoList();
        //DoorInfoList instances = new DoorInfoList();

        //for (int i = 0; i < list1.Count; i++)
        //{
        //    var item1 = list1[i];
        //    if (item1.gameObject == null)
        //    {
        //        Debug.LogError($"item1.gameObject == null item1:{item1}");
        //        continue;
        //    }
        //    list2.Remove(item1);

        //    if (ProgressBarHelper.DisplayCancelableProgressBar("SetDoorShared1", i, list1.Count))
        //    {
        //        break;
        //    }

        //    if (instances.Contains(item1))
        //    {
        //        continue;
        //    }

        //    item1.Prepare();//Split+LOD+CopyA

        //    PrefabInfo prefab = new PrefabInfo(item1.gameObject);
        //    prefabsNew.Add(prefab);


        //    DoorInfo copyItem1 = item1.Clone();
        //    if (copyItem1 == null)
        //    {
        //        Debug.LogError($"copyItem1 == null item1:{item1}");
        //        continue;
        //    }

        //    //copyDoor1.transform.position = door2.DoorGo.transform.position;
        //    bool isAligned = false;
        //    bool isBreak = false;
        //    for (int j = 0; j < list2.Count; j++)
        //    {
        //        if (copyItem1 == null)
        //        {
        //            copyItem1 = item1.Clone();
        //        }
        //        count++;
        //        if (ProgressBarHelper.DisplayCancelableProgressBar("SetDoorShared2", i, list1.Count, j, list2.Count))
        //        {
        //            isBreak = true;
        //            break;
        //        }

        //        var item2 = list2[j];
        //        if (item2.gameObject == null)
        //        {
        //            Debug.LogError($"item2Go == null item2:{item2} item1:{item1}");
        //            continue;
        //        }
        //        if (copyItem1 == null)
        //        {
        //            Debug.LogError($"copyItem1 == null item2:{item2} item1:{item1}");
        //            continue;
        //        }
        //        //var copyDoor1 = MeshHelper.CopyGO(door1.DoorGo);
        //        copyItem1.transform.parent = item2.gameObject.transform.parent;
        //        copyItem1.transform.position = item2.gameObject.transform.position;
        //        copyItem1.transform.rotation = item2.gameObject.transform.rotation;

        //        float distance1 = MeshHelper.GetVertexDistanceEx(copyItem1.transform, item2.gameObject.transform, "SetDoorShared1", false);

        //        if (distance1 < DistanceSetting.zeroM)
        //        {
        //            Debug.Log($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.gameObject.name} door2:{item2.gameObject.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

        //            posAlinCount++;
        //            //var copyItem2 = MeshHelper.CopyGO(copyItem1);

        //            item2.gameObject.SetActive(false);
        //            copyItem1.gameObject.name = item2.gameObject.name + "_New";
        //            prefab.AddInstance(item2.gameObject, copyItem1.gameObject);
        //            copyItem1 = null;

        //            //list1.Remove(door2);
        //            instances.Add(item2);

        //            list2.Remove(item2);
        //            j--;

        //            isAligned = true;
        //        }
        //        else
        //        {
        //            Debug.LogWarning($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.gameObject.name} door2:{item2.gameObject.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

        //        }
        //    }

        //    if (copyItem1!=null)
        //    {
        //        GameObject.DestroyImmediate(copyItem1.gameObject);
        //    }

        //    if (isBreak)
        //    {
        //        break;
        //    }
        //}

        //prefabsNew.SortByInstanceCount();

        //ProgressBarHelper.ClearProgressBar();
        //Debug.Log($"SetDoorShared count:{count} posAlinCount:{posAlinCount} meshAlignCount:{meshAlignCount} noAlignCount:{noAlignCount} prefabs:{prefabsNew.Count} instances:{instances.Count} time:{DateTime.Now - start}");
        //return prefabsNew;
    }

    public static void CopyDoorA(GameObject gameObject,bool align)
    {
        if (gameObject == null) return;
        EditorHelper.UnpackPrefab(gameObject);
        var childCount = gameObject.transform.childCount;
        if (childCount == 2)
        {
            var door1 = gameObject.transform.GetChild(0);
            var door2 = gameObject.transform.GetChild(1);

            var scale1 = door1.localScale;
            var scale2 = door2.localScale;
            //if (scale1 == Vector3.one || scale2 == Vector3.one || (scale1==new Vector3(1000,1000,1000) && scale2 == new Vector3(1000, 1000, 1000)))
            {
                if (scale2 == Vector3.one)
                {
                    var tmp = door1;
                    door1 = door2;
                    door2 = tmp;
                }

                GameObject newDoor2 = MeshHelper.CopyGO(door1.gameObject);
                newDoor2.transform.localScale = new Vector3(-door1.localScale.x, door1.localScale.y, door1.localScale.z);
                newDoor2.transform.position = door2.transform.position;
                float distance1 = MeshHelper.GetVertexDistanceEx(door2.transform, newDoor2.transform, "CopyDoor1", false);
                //door2.gameObject.SetActive(false);
                //MeshAlignHelper.AcRTAlignJob(newDoor2, door2.gameObject);

                if (distance1 > DistanceSetting.zeroM)
                {
                    if (align)
                    {
                        MeshComparer.Instance.AcRTAlignJob(newDoor2, door2.gameObject);

                        float distance2 = MeshHelper.GetVertexDistanceEx(door2.transform, newDoor2.transform, "CopyDoor2", false);
                        if (distance2 < DistanceSetting.zeroM)
                        {
                            //Debug.LogWarning($"CopyDoorA 对齐成功2 door2:{door2.gameObject} newDoor2:{newDoor2} distance1:{distance1} ");
                            newDoor2.name = door2.name + "_New";
                            GameObject.DestroyImmediate(door2.gameObject);
                        }
                        else
                        {
                            //GameObject.DestroyImmediate(newDoor2.gameObject);
                            Debug.LogError($"CopyDoorA 对齐失败1 door2:{door2.gameObject.name} newDoor2:{newDoor2.name} distance1:{distance1} distance2:{distance2}");
                            GameObject.DestroyImmediate(newDoor2);
                        }
                    }
                    else
                    {
                        Debug.LogError($"CopyDoorA 对齐失败2 door2:{door2.gameObject.name} newDoor2:{newDoor2.name} distance1:{distance1} ");
                        GameObject.DestroyImmediate(newDoor2);
                    }
                }
                else
                {
                    //Debug.LogWarning($"CopyDoorA 对齐成功1 door2:{door2.gameObject} newDoor2:{newDoor2} distance1:{distance1} ");

                    newDoor2.name = door2.name + "_New";
                    GameObject.DestroyImmediate(door2.gameObject);
                }


            }
            //else
            //{
            //    Debug.LogError($"RendererIdEditor.CopyDoorA scale1!=Vector3.one && scale2 != Vector3.one scale1:{scale1} scale2:{scale2}");
            //}
        }
        else
        {
            Debug.LogWarning("RendererIdEditor.CopyDoorA childCount =!= 2");
        }
    }

    public static void CopyDoorA(DoorInfo door, bool align)
    {
        if (door == null) return;
        EditorHelper.UnpackPrefab(door.gameObject);
        var childCount = door.DoorParts.Count;
        if (childCount == 2)
        {
            var door1 = door.DoorParts[0];
            var door2 = door.DoorParts[1];

            var scale1 = door1.localScale;
            var scale2 = door2.localScale;
            //if (scale1 == Vector3.one || scale2 == Vector3.one || (scale1==new Vector3(1000,1000,1000) && scale2 == new Vector3(1000, 1000, 1000)))
            {
                if (scale2 == Vector3.one)
                {
                    var tmp = door1;
                    door1 = door2;
                    door2 = tmp;
                }

                GameObject newDoor2 = MeshHelper.CopyGO(door1.gameObject);
                door1.gameObject = newDoor2;

                newDoor2.transform.localScale = new Vector3(-door1.localScale.x, door1.localScale.y, door1.localScale.z);
                newDoor2.transform.position = door2.transform.position;
                float distance1 = MeshHelper.GetVertexDistanceEx(door2.transform, newDoor2.transform, "CopyDoor1", false);
                //door2.gameObject.SetActive(false);
                //MeshAlignHelper.AcRTAlignJob(newDoor2, door2.gameObject);

                if (distance1 > DistanceSetting.zeroM && align)
                {
                    MeshComparer.Instance.AcRTAlignJob(newDoor2, door2.gameObject);

                    float distance2 = MeshHelper.GetVertexDistanceEx(door2.transform, newDoor2.transform, "CopyDoor2", false);
                    Debug.Log($"distance1:{distance1} distance2:{distance2}");

                    if (distance2 < DistanceSetting.zeroM)
                    {
                        newDoor2.name = door2.name + "_New";
                        GameObject.DestroyImmediate(door2.gameObject);
                    }
                    else
                    {
                        Debug.LogError("对齐失败");
                    }
                }
                else
                {
                    Debug.Log($"distance1:{distance1}");
                }


            }
            //else
            //{
            //    Debug.LogError($"RendererIdEditor.CopyDoorA scale1!=Vector3.one && scale2 != Vector3.one scale1:{scale1} scale2:{scale2}");
            //}
        }
        else
        {
            Debug.LogError("RendererIdEditor.CopyDoorA childCount =!= 2");
        }
    }

    public static void Prepare(GameObject gameObject)
    {
        DoorInfo doorInfo = new DoorInfo(gameObject);
        doorInfo.PreparePrefab();
    }
}

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

    public DoorsRootList(DoorsRoot[] doorsRoots)
    {
        GetDoors(doorsRoots);
    }

    public void GetDoors(DoorsRoot[] doorsRoots)
    {
        Debug.Log($"GetDoors roots:{doorsRoots.Length}");
        //Doors.Clear();
        VertexCount = 0;
        VertexCount_Show = 0;
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", "Start", 0);
        //var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true).Where(i => i.name.ToLower().Contains("door")).ToList();
        for (int i = 0; i < doorsRoots.Length; i++)
        {

            float progress = (float)i / doorsRoots.Length;
            ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", $"{i}/{doorsRoots.Length} {progress:P1}", progress);
            //var parent = doorsRoots[i].transform.parent;
            ////if (parent != null && parent.name.ToLower().Contains("combined")) continue;
            var doorRoot = doorsRoots[i];
            doorRoot.Init();
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
        ProgressBarHelper.ClearProgressBar();
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
            EditorHelper.UnpackPrefab(root.gameObject);
            //doors.AddRange(root.Doors);
            foreach(var door in root.Doors)
            {
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
}

[Serializable]
public class DoorPartInfo
{
    public string Root;
    public GameObject gameObject;

    public Vector3 Pos;

    public Vector3 Center;

    public float DisToCenter;

    public Vector3 OffToCenter;

    public int MatCount;

    public string MatNames;

    public int VertexCount;

    public int SubMeshCount;

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
        if (gameObject.transform.parent != null)
        {
            return $"{gameObject.transform.parent.name}>{gameObject.name}";
        }
        else
        {
            return $"{gameObject.name}";
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
            var minMax = MeshHelper.GetMinMax(mf);
            Center = minMax[3];
            DisToCenter = Vector3.Distance(Pos, Center);
            OffToCenter = Center - Pos;
            VertexCount = mf.sharedMesh.vertexCount;
            SubMeshCount = mf.sharedMesh.subMeshCount;
        }
        else
        {
            var filters = go.GetComponentsInChildren<MeshFilter>();

            var minMax = MeshHelper.GetMinMax(filters);
            Center = minMax[3];
            DisToCenter = Vector3.Distance(Pos, Center);
            OffToCenter = Center - Pos;

            foreach (var mf2 in filters)
            {
                VertexCount += mf2.sharedMesh.vertexCount;
            }
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
        }

        var meshRenderers = MeshRendererInfo.InitRenderers(go);
        meshRenderers.SetType(MeshRendererType.Static);
    }



    public override string ToString()
    {
        //return $"mat:{MatCount},mesh:{SubMeshCount},v:{VertexCount},dis:{DisToCenter:F1},off:({OffToCenter.x:F2},{OffToCenter.y:F2},{OffToCenter.z:F2})";
        return $"mat:{MatCount},mesh:{SubMeshCount},v:{VertexCount},dis:{DisToCenter:F1}";
    }

    internal void SetLOD()
    {
        LODHelper.SetDoorLOD(this.gameObject);
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
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        for (int i = 0; i < this.Count; i++)
        {
            DoorInfo info = this[i];
            if (info == null) continue;
            if (info.gameObject == null) continue;
            var mfs = info.gameObject.GetComponentsInChildren<MeshFilter>(true);
            //Debug.Log($"[{i}/{this.Count}]GetMeshFilters meshFilters:{meshFilters.Count} go:{info.gameObject} mfs:{mfs.Length}");
            //meshFilters.AddRange(mfs);
            foreach(var mf in mfs)
            {
                if (!meshFilters.Contains(mf))
                {
                    meshFilters.Add(mf);
                }
            }
            
        }
        return meshFilters;
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
        if (transform.childCount == 0 || root.GetComponent<LODGroup>()!=null)
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
                DoorPartInfo doorPart = new DoorPartInfo(child.gameObject);
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
        DoorParts.Sort((a, b) => { return b.VertexCount.CompareTo(a.VertexCount); });
        //Debug.Log($"DoorInfo door:{root.name} parts:{DoorParts.Count} VertexCount:{VertexCount}");
    }

    public string GetTitle()
    {
        if (gameObject == null) return "NULL";
        return $"{Root}>{gameObject.name}({DoorParts.Count})";
    }

    public override string ToString()
    {
        return $"v:{MeshHelper.GetVertexCountS(vertexCount)}";
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
            foreach (var part in DoorParts)
            {
                part.Split();
            }
        }
        
    }

    public void SetLOD()
    {
        foreach (var part in DoorParts)
        {
            part.SetLOD();
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
