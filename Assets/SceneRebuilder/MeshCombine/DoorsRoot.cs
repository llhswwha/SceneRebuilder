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

    public bool IsAlign = false;

    public bool IsReplace = false;

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
            VertexCount += door.VertexCount;
            if (door.DoorGo && door.DoorGo.activeInHierarchy)
                VertexCount_Show += door.VertexCount;
        }
        Doors.Sort((a, b) => b.VertexCount.CompareTo(a.VertexCount));
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

    public void CombinePrefab(bool isAlign, bool isReplace)
    {

        PrefabInfoList list1 = new PrefabInfoList(prefabs);
        PrefabInfoList list2 = new PrefabInfoList(prefabs);

        DateTime start = DateTime.Now;
        Debug.Log($"SetDoorShared root:{this.name}");
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

            if (ProgressBarHelper.DisplayCancelableProgressBar("SetDoorShared1", i, list1.Count))
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
                if (ProgressBarHelper.DisplayCancelableProgressBar("SetDoorShared2", i, list1.Count, j, list2.Count))
                {
                    isBreak = true;
                    break;
                }

                var item2 = list2[j];
                var item2Go = item2.Prefab;

                //var copyDoor1 = MeshHelper.CopyGO(door1.DoorGo);
                copyDoor1.transform.position = item2Go.transform.position;

                float distance1 = MeshHelper.GetVertexDistanceEx(copyDoor1.transform, item2Go.transform, "SetDoorShared1", false);


                if (distance1 < DistanceSetting.zeroM)
                {
                    Debug.Log($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1Go.name} door2:{item2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                    posAlinCount++;
                    prefab.Add(item2Go, copyDoor1);

                    //list1.Remove(door2);
                    instances.Add(item2);

                    list2.Remove(item2);
                    j--;


                }
                else
                {
                    if (isAlign)
                    {
                        //Debug.LogWarning($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{door1Go.name} door2:{door2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                        MeshComparer.Instance.AcRTAlignJob(copyDoor1.gameObject, item2Go.gameObject);
                        isBreak = true;
                        break;

                        //float distance2 = MeshHelper.GetVertexDistanceEx(copyDoor1.transform, door2Go.transform, "SetDoorShared2", false);
                        //Debug.Log($"distance1:{distance1} distance2:{distance2}");

                        //if (distance2 < DistanceSetting.zeroM)
                        //{
                        //    //newDoor2.name = door2.name + "_New";
                        //    //GameObject.DestroyImmediate(door2.gameObject);
                        //    meshAlignCount++;
                        //}
                        //else
                        //{
                        //    noAlignCount++;
                        //    //Debug.LogError("¶ÔÆëÊ§°Ü");

                        //    Debug.LogWarning($"¶ÔÆëÊ§°Ü SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{door1Go.name} door2:{door2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                        //}
                    }
                    else
                    {
                        Debug.LogWarning($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1Go.name} door2:{item2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                    }

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

    public void AcRTAlignJobs()
    {
        MeshPoints[] meshPoints = Doors.GetMeshPoints();
        PrefabInstanceBuilder.Instance.AcRTAlignJobs(meshPoints);
    }

    public void AcRTAlignJobsEx()
    {
        MeshPoints[] meshPoints = Doors.GetMeshPoints();
        PrefabInstanceBuilder.Instance.AcRTAlignJobsEx(meshPoints);
    }

    public void SetDoorShared(bool isAlign,bool isReplace)
    {
        EditorHelper.UnpackPrefab(this.gameObject);

        Init();

        DoorInfoList list1 = new DoorInfoList(Doors);
        DoorInfoList list2 = new DoorInfoList(Doors);

        DateTime start = DateTime.Now;
        Debug.Log($"SetDoorShared root:{this.name}");
        int allCount = 0;
        for (int i = 0; i < Doors.Count; i++)
        {
            allCount += Doors.Count - 1 - i;
        }

        int count = 0;
        int meshAlignCount = 0;
        int posAlinCount = 0;
        int noAlignCount = 0;


        PrefabInfoList  prefabsNew = new PrefabInfoList();
        DoorInfoList instances = new DoorInfoList();

        for (int i = 0; i < list1.Count; i++)
        {
            var item1 = list1[i];
            var item1Go = item1.DoorGo;
            list2.Remove(item1);

            if (ProgressBarHelper.DisplayCancelableProgressBar("SetDoorShared1", i, list1.Count))
            {
                break;
            }

            if (instances.Contains(item1))
            {
                continue;
            }

            PrefabInfo prefab = new PrefabInfo(item1Go);
            prefabsNew.Add(prefab);

            var copyItem1 = MeshHelper.CopyGO(item1Go);
            //copyDoor1.transform.position = door2.DoorGo.transform.position;
            bool isAligned = false;
            bool isBreak = false;
            for (int j = 0; j < list2.Count; j++)
            {
                if (copyItem1 == null)
                {
                    copyItem1 = MeshHelper.CopyGO(item1Go);
                }
                count++;
                if (ProgressBarHelper.DisplayCancelableProgressBar("SetDoorShared2", i, list1.Count, j, list2.Count))
                {
                    isBreak = true;
                    break;
                }

                var item2 = list2[j];
                var item2Go = item2.DoorGo;

                //var copyDoor1 = MeshHelper.CopyGO(door1.DoorGo);
                copyItem1.transform.position = item2Go.transform.position;

                float distance1 = MeshHelper.GetVertexDistanceEx(copyItem1.transform, item2Go.transform, "SetDoorShared1", false);

                if (distance1 < DistanceSetting.zeroM)
                {
                    Debug.Log($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1Go.name} door2:{item2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                    posAlinCount++;
                    //var copyItem2 = MeshHelper.CopyGO(copyItem1);

                    item2Go.SetActive(false);
                    copyItem1.name = item2Go.name + "_New";
                    prefab.Add(item2Go, copyItem1);
                    copyItem1 = null;

                    //list1.Remove(door2);
                    instances.Add(item2);

                    list2.Remove(item2);
                    j--;

                    isAligned = true;
                }
                else
                {
                    if (isAlign)
                    {
                        //Debug.LogWarning($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{door1Go.name} door2:{door2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                        MeshComparer.Instance.AcRTAlignJob(copyItem1.gameObject, item2Go.gameObject);
                        isBreak = true;
                        break;

                        //float distance2 = MeshHelper.GetVertexDistanceEx(copyDoor1.transform, door2Go.transform, "SetDoorShared2", false);
                        //Debug.Log($"distance1:{distance1} distance2:{distance2}");

                        //if (distance2 < DistanceSetting.zeroM)
                        //{
                        //    //newDoor2.name = door2.name + "_New";
                        //    //GameObject.DestroyImmediate(door2.gameObject);
                        //    meshAlignCount++;
                        //}
                        //else
                        //{
                        //    noAlignCount++;
                        //    //Debug.LogError("¶ÔÆëÊ§°Ü");

                        //    Debug.LogWarning($"¶ÔÆëÊ§°Ü SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{door1Go.name} door2:{door2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                        //}
                    }
                    else
                    {
                        Debug.LogWarning($"SetDoorShared2[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1Go.name} door2:{item2Go.name} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                    }

                }
            }

            if (copyItem1)
            {
                GameObject.DestroyImmediate(copyItem1);
            }

            if (isBreak)
            {
                break;
            }
        }

        prefabsNew.SortByInstanceCount();

        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SetDoorShared count:{count} posAlinCount:{posAlinCount} meshAlignCount:{meshAlignCount} noAlignCount:{noAlignCount} prefabs:{prefabsNew.Count} instances:{instances.Count} time:{DateTime.Now - start}");

        prefabs = prefabsNew;
    }

    public void ApplyReplace()
    {
        prefabs.ApplyReplace();
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
}
