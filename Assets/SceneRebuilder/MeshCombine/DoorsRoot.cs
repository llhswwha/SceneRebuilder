using MeshJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsRoot : MonoBehaviour
{
    public string Root;
    public int VertexCount = 0;
    public int VertexCount_Show = 0;

    public DoorInfoList Doors = new DoorInfoList();

    //public bool IsAlign = false;

    //public bool IsReplace = false;

    public void Init()
    {
        Doors.Clear();
        BuildingModelInfo[] models = this.gameObject.GetComponentsInParent<BuildingModelInfo>(true);
        if (models.Length > 0)
        {
            Root = models[0].name;
        }
        VertexCount = 0;
        VertexCount_Show = 0;
        for (int i=0;i<transform.childCount;i++)
        {
            var child = transform.GetChild(i);
            DoorInfo door = new DoorInfo(child.gameObject);
            Doors.Add(door);
            VertexCount += door.vertexCount;
            if (door.gameObject && door.gameObject.activeInHierarchy)
                VertexCount_Show += door.vertexCount;
        }
        Doors.Sort((a, b) => b.vertexCount.CompareTo(a.vertexCount));
        Debug.Log($"DoorsRoot.Init name:{this.gameObject.name} children:{transform.childCount} Doors:{Doors.Count}");
    }
    public string GetTitle()
    {
        if (this.gameObject == null) return "NULL";
        return $"{Root}>{this.gameObject.name}({Doors.Count})";
    }

    public override string ToString()
    {
        return $"v:{MeshHelper.GetVertexCountS(VertexCount)}";
    }

    public PrefabInfoList prefabs = new PrefabInfoList();

    //public void SetDoorShared()
    //{
    //    SetDoorShared(false);
    //}

    public void CombinePrefab()
    {

        PrefabInfoList list1 = new PrefabInfoList(prefabs);
        PrefabInfoList list2 = new PrefabInfoList(prefabs);

        DateTime start = DateTime.Now;
        Debug.Log($"CombinePrefab root:{this.name}");
        int allCount = 0;
        for (int i = 0; i < Doors.Count; i++)
        {
            allCount += Doors.Count - 1 - i;
        }

        int count = 0;
        int meshAlignCount = 0;
        int posAlinCount = 0;
        int noAlignCount = 0;


        PrefabInfoList prefabsNew = new PrefabInfoList();
        PrefabInfoList instances = new PrefabInfoList();

        for (int i = 0; i < list1.Count; i++)
        {
            var item1 = list1[i];
            var item1Go = item1.Prefab;
            list2.Remove(item1);

            if (ProgressBarHelper.DisplayCancelableProgressBar("CombinePrefab1", i, list1.Count))
            {
                break;
            }

            if (instances.Contains(item1))
            {
                continue;
            }

            PrefabInfo prefab = new PrefabInfo(item1Go);
            prefabsNew.Add(prefab);

            var copyDoor1 = MeshHelper.CopyGO(item1Go);
            //copyDoor1.transform.position = door2.DoorGo.transform.position;

            bool isBreak = false;
            for (int j = 0; j < list2.Count; j++)
            {
                count++;
                if (ProgressBarHelper.DisplayCancelableProgressBar("CombinePrefab2", i, list1.Count, j, list2.Count))
                {
                    isBreak = true;
                    break;
                }

                var item2 = list2[j];
                var item2Go = item2.Prefab;

                //var copyDoor1 = MeshHelper.CopyGO(door1.DoorGo);
                copyDoor1.transform.position = item2Go.transform.position;

                float distance1 = MeshHelper.GetVertexDistanceEx(copyDoor1.transform, item2Go.transform, "CombinePrefab1", false);

                if (distance1 < DistanceSetting.zeroM)
                {
                    Debug.Log($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1Go.name} door2:{item2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                    posAlinCount++;
                    prefab.AddInstance(item2Go, copyDoor1);

                    //list1.Remove(door2);
                    instances.Add(item2);

                    list2.Remove(item2);
                    j--;

                }
                else
                {
                    ////if (isAlign)
                    //{
                    //    //Debug.LogWarning($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{door1Go.name} door2:{door2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                    //    MeshComparer.Instance.AcRTAlignJob(copyDoor1.gameObject, item2Go.gameObject);
                    //    isBreak = true;
                    //    break;

                    //    //float distance2 = MeshHelper.GetVertexDistanceEx(copyDoor1.transform, door2Go.transform, "SetDoorShared2", false);
                    //    //Debug.Log($"distance1:{distance1} distance2:{distance2}");

                    //    //if (distance2 < DistanceSetting.zeroM)
                    //    //{
                    //    //    //newDoor2.name = door2.name + "_New";
                    //    //    //GameObject.DestroyImmediate(door2.gameObject);
                    //    //    meshAlignCount++;
                    //    //}
                    //    //else
                    //    //{
                    //    //    noAlignCount++;
                    //    //    //Debug.LogError("¶ÔÆëÊ§°Ü");

                    //    //    Debug.LogWarning($"¶ÔÆëÊ§°Ü SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{door1Go.name} door2:{door2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                    //    //}
                    //}
                    //else
                    //{
                        
                    //}
                    Debug.LogWarning($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1Go.name} door2:{item2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                }
            }

            GameObject.DestroyImmediate(copyDoor1);

            if (isBreak)
            {
                break;
            }
        }

        prefabsNew.Sort((a, b) => b.InstanceCount.CompareTo(a.InstanceCount));

        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SetDoorShared count:{count} posAlinCount:{posAlinCount} meshAlignCount:{meshAlignCount} noAlignCount:{noAlignCount} prefabs:{prefabsNew.Count} instances:{instances.Count} time:{DateTime.Now - start}");

        prefabs = prefabsNew;
    }

    public void CopyPart()
    {
        for(int i=0;i<Doors.Count;i++)
        {
            ProgressBarHelper.DisplayCancelableProgressBar("CopyPart", i, Doors.Count);
            DoorHelper.CopyDoorA(Doors[i].gameObject, true);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    public void Split()
    {
        var parts=Doors.GetDoorParts();
        DoorManager.SplitDoorParts(parts);
    }

    public void AcRTAlignJobs(bool isCopy)
    {
        //MeshPoints[] meshPoints = Doors.GetMeshPoints();
        //prefabs=PrefabInstanceBuilder.Instance.AcRTAlignJobs(meshPoints);
        prefabs = PrefabInstanceBuilder.Instance.AcRTAlignJobs(this.gameObject, isCopy);
    }

    public void AcRTAlignJobsEx(bool isCopy)
    {
        //MeshPoints[] meshPoints = Doors.GetMeshPoints();
        //prefabs = PrefabInstanceBuilder.Instance.AcRTAlignJobsEx(meshPoints);
        prefabs = PrefabInstanceBuilder.Instance.AcRTAlignJobsEx(this.gameObject, isCopy);
    }

    public void SetDoorShared()
    {
        EditorHelper.UnpackPrefab(this.gameObject);
        Init();
        Debug.Log($"SetDoorShared root:{this.name}");
        prefabs = DoorHelper.SetDoorShared(Doors);
    }

    

    public void ApplyReplace()
    {
        prefabs.ApplyReplace();
    }

    public void DestroyInstances()
    {
        prefabs.DestroyInstances();
    }

    public void ResetPrefabs()
    {
        prefabs.ResetPrefabs();
    }

    public void SetParent()
    {
        var mfs = this.gameObject.GetComponentsInChildren<MeshFilter>(true);
        foreach(var mf in mfs)
        {
            mf.transform.SetParent(this.transform);
            mf.transform.position = mf.transform.position * 0.8f;
        }
    }
    

    public void RevertReplace()
    {
        prefabs.RevertReplace();
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
        var doors = this.Doors;
        var filters = doors.GetMeshFilters();
        return new SharedMeshInfoList(filters);
    }
}
