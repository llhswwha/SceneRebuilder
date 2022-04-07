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
    public GameObject OldTarget = null;

    public GameObject LocalTarget = null;

    public void RecoverDoors()
    {
        EditorHelper.UnpackPrefab(OldTarget);
        EditorHelper.UnpackPrefab(LocalTarget);
        var doorRoots0 = UpdateDoors(OldTarget, null);
        var doorRoots1 = UpdateDoors(LocalTarget, null);
        foreach(var door0 in doorRoots0)
        {
            var door1 = doorRoots1.Find(i => i!=null && i.name == door0.name);
            if (door1)
            {
                door0.transform.SetParent(door1.transform.parent);
                GameObject.DestroyImmediate(door1.gameObject);
            }
            else
            {
                Debug.LogError($"RecoverDoors door1==null door0:{door0.name}");
            }
            
        }
    }

    public bool IsOnlyActive = false;

    public bool IsOnlyCanSplit = false;

    public DoorsRootList doorRoots;

    public void CombineDoors()
    {
        //buildings->CombineDoors
        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        for (int i = 0; i < buildings.Length; i++)
        {
            var b = buildings[i];
            var p = new ProgressArg("CombineDoors", i, buildings.Length, b);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p))
            {
                break;
            }
            b.CombineDoors();
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log("CombineDoors");
    }

    public void DeleteOthersOfDoor()
    {
        //var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        //foreach (var b in buildings)
        //{
        //    b.DeleteOthersOfDoor();
        //}
        //Debug.Log("DeleteOthersOfDoor");

        var buildings = GameObject.FindObjectsOfType<BuildingModelInfo>(true);
        for (int i = 0; i < buildings.Length; i++)
        {
            var b = buildings[i];
            var p = new ProgressArg("DeleteOthersOfDoor", i, buildings.Length, b);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p))
            {
                break;
            }
            b.DeleteOthersOfDoor();
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log("DeleteOthersOfDoor");
    }

    public static Transform[] GetTransforms(GameObject go)
    {
        Transform[] ts = null;
        if (go == null)
        {
            ts = GameObject.FindObjectsOfType<Transform>(true);
        }
        else
        {
            ts = go.GetComponentsInChildren<Transform>(true);
        }
        return ts;
    }

    public DoorsRootList UpdateDoors(Action<ProgressArg> progressChanged=null)
    {
        doorsList = null;
        doorParts = null;
        doorRoots = UpdateDoors(LocalTarget, progressChanged);
        GetDoors();
        GetDoorParts();
        return doorRoots;
    }

    public DoorsRootList UpdateAllDoors()
    {
        doorsList = null;
        doorParts = null;
        doorRoots = UpdateDoors(null, null);
        GetDoors();
        GetDoorParts();
        return doorRoots;
    }

    public static DoorsRootList UpdateDoors(GameObject go, Action<ProgressArg> progressChanged)
    {
        var ts = GetTransforms(go);
        DoorsRoot[] items = InitDoorsRoot(ts, progressChanged);
        DoorsRootList doorRootList = new DoorsRootList(items, progressChanged);
        Debug.Log($"DoorManager.UpdateDoors LocalTarget:[{go}] doorRoots:{doorRootList.Count}");
        return doorRootList;
    }

    public PrefabInfoList prefabs;

    public bool isAlignDoor = false;

    public void GetPrefabs()
    {
        var doors = doorRoots.GetDoors();
        prefabs = DoorHelper.SetDoorShared(doors, isAlignDoor);
    }

    public void Split()
    {
        doorRoots.Split();
    }

    public void SetDoorPivot()
    {
        doorRoots.SetDoorPivot();
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

    public static DoorsRoot[] InitDoorsRoot(Transform[] ts, Action<ProgressArg> progressChanged)
    {
        List<DoorsRoot> list = new List<DoorsRoot>();
        var doorsList = ts.Where(i => i.name.ToLower().EndsWith("doors")).ToList();
        //Debug.Log($"InitDoorsRoot ts:{ts.Length} doorsRoots:{doorsList.Count}");
        for (int i = 0; i < doorsList.Count; i++)
        {
            Transform doors = doorsList[i];
            var p = new ProgressArg("InitDoorsRoot", i, doorsList.Count, doors);
            if (progressChanged != null) progressChanged(p);
             var root = doors.gameObject.GetComponent<DoorsRoot>();
            if (root == null)
            {
                root = doors.gameObject.AddComponent<DoorsRoot>();
            }
            root.Init();
            list.Add(root);
        }
        if (progressChanged != null) progressChanged(new ProgressArg("InitDoorsRoot", doorsList.Count, doorsList.Count, "End"));
        return list.ToArray();
    }

    DoorInfoList doorsList;

    public DoorInfoList GetDoors()
    {
        if (doorsList != null) return doorsList;
        doorsList = new DoorInfoList();
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

    public DoorPartInfoList doorParts;

    public DoorPartInfoList GetDoorParts()
    {
        if (doorParts != null)
        {
            return doorParts;
        }
        doorParts = new DoorPartInfoList();
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
                    //if (IsOnlyCanSplit)
                    //{
                    //    if (door.SubMeshCount <= 1) continue;
                    //}
                    if (IsOnlyCanSplit)
                    {
                        if (door.MatCount != 1) continue;
                    }
                    doorParts.AddEx(door);

                   
                }
            }
        }
        doorParts.Sort((a, b) =>
        {
            var r1 = a.DisToCenter.ToString("F2").CompareTo(b.DisToCenter.ToString("F2"));
            if (r1 == 0)
            {
                r1 = a.GetTitle().CompareTo(b.GetTitle());
            }
            if (r1 == 0)
            {
                r1 = b.VertexCount.CompareTo(a.VertexCount);
            }
            return r1;
        });
        Debug.Log($"DoorManager.GetDoorParts IsOnlyActive:{IsOnlyActive} IsOnlyCanSplit:{IsOnlyCanSplit} doorRoots:{doorRoots.Count} doorParts:{doorParts.Count}");
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

    public static PrefabInfoList SetDoorShared(DoorInfoList doors,bool align)
    {
        return PrefabInfoListHelper.GetPrefabInfos(doors, align);
    }

    public static void CopyDoorA(GameObject gameObject,bool align,bool isDestroy=true)
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
                            if(isDestroy)
                                GameObject.DestroyImmediate(newDoor2);
                        }
                    }
                    else
                    {
                        Debug.LogError($"CopyDoorA 对齐失败2 door2:{door2.gameObject.name} newDoor2:{newDoor2.name} distance1:{distance1} ");
                        if (isDestroy)
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
            Debug.LogWarning("RendererIdEditor.CopyDoorA childCount =!= 2 :"+ gameObject);
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

    public static void SetDoorPartPivot(GameObject gameObject, bool isForce = false)
    {
        DoorInfo doorInfo = new DoorInfo(gameObject);
        doorInfo.SetDoorPartPivot(isForce);
    }
}

