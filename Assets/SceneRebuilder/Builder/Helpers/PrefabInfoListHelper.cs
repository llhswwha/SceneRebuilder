using CommonUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public static class PrefabInfoListHelper
{
    public static SharedMeshInfo CloneSharedMeshInfo(GameObject go)
    {
        GameObject goNew = MeshHelper.CopyGO(go);
        SharedMeshInfo info = new SharedMeshInfo(goNew);
        return info;
    }

    public static T Clone<T>(T ori) where T : class, IPrefab<T>
    {
        if(ori is SharedMeshInfo)
        {
            return CloneSharedMeshInfo(ori.gameObject) as T;
        }
        return null;
    }

    public static PrefabInfoList GetPrefabInfos<T>(List<T> doors, bool align) where T : class, IPrefab<T>
    {
        List<T> list1 = new List<T>(doors);
        List<T> list2 = new List<T>(doors);

        DateTime start = DateTime.Now;

        int allCount = 0;
        for (int i = 0; i < doors.Count; i++)
        {
            allCount += doors.Count - 1 - i;
        }

        int count = 0;
        int meshAlignCount = 0;
        int posAlinCount = 0;
        int noAlignCount = 0;


        PrefabInfoList prefabsNew = new PrefabInfoList();
        List<T> instances = new List<T>();

        for (int i = 0; i < list1.Count; i++)
        {
            var item1 = list1[i];
            var p1 = new ProgressArg("GetPrefabInfos1", i, list1.Count, item1);
            if (item1.gameObject == null)
            {
                Debug.LogError($"item1.gameObject == null item1:{item1}");
                continue;
            }
            list2.Remove(item1);

            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                break;
            }

            if (instances.Contains(item1))
            {
                continue;
            }

            item1.PreparePrefab();//Split+LOD+CopyA
            if (item1.gameObject == null)
            {
                Debug.LogError($"GetPrefabInfos item1.gameObject == null item1:{item1}");
                continue;
            }

            PrefabInfo prefab = new PrefabInfo(item1.gameObject);
            prefabsNew.Add(prefab);


            T copyItem1 = Clone(item1) as T;
            if (copyItem1 == null)
            {
                Debug.LogError($"copyItem1 == null item1:{item1}");
                continue;
            }

            //copyDoor1.transform.position = door2.DoorGo.transform.position;
            bool isAligned = false;
            bool isBreak = false;
            for (int j = 0; j < list2.Count; j++)
            {
                var item2 = list2[j];
                var p2 = new ProgressArg("GetPrefabInfos2", j, list2.Count, item2);
                p1.AddSubProgress(p2);
                if (copyItem1 == null)
                {
                    copyItem1 = Clone(item1) as T;
                }
                count++;
                if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                {
                    isBreak = true;
                    break;
                }


                if (item2.gameObject == null)
                {
                    Debug.LogError($"item2Go == null item2:{item2} item1:{item1}");
                    continue;
                }
                if (copyItem1 == null)
                {
                    Debug.LogError($"copyItem1 == null item2:{item2} item1:{item1}");
                    continue;
                }

                int vertexCount12 = Math.Abs(copyItem1.GetVertexCount() - item2.GetVertexCount());
                if (vertexCount12 > 100)
                {
                    //Debug.Log($"GetPrefabInfos[{i}/{list1.Count} {j}/{list2.Count}] vertexCount12 > 100  vertexCount12:{vertexCount12} door1:{item1.ToString()} door2:{item2.ToString()} ");
                    continue;
                }

                //var copyDoor1 = MeshHelper.CopyGO(door1.DoorGo);
                var t1 = copyItem1.transform;
                var t2 = item2.transform;
                t1.parent = t2.parent;
                t1.position = t2.position;
                t1.rotation = t2.rotation;

                float distance1 = VertexHelper.GetVertexDistanceEx(t1, t2, "SetDoorShared1", false);

                if (distance1 < DistanceSetting.zeroM)
                {
                    //Debug.Log($"SetDoorShared1(Pos Aligned)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                    posAlinCount++;
                    //var copyItem2 = MeshHelper.CopyGO(copyItem1);

                    item2.gameObject.SetActive(false);
                    copyItem1.gameObject.name = item2.gameObject.name + "_New";
                    prefab.AddInstance(item2.gameObject, copyItem1.gameObject);

                    //list1.Remove(door2);
                    instances.Add(item2);

                    list2.Remove(item2);

                    j--;
                    copyItem1 = null;
                    isAligned = true;
                }
                else
                {
                    //Debug.LogWarning($"SetDoorShared2(Not)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                    if (align && copyItem1.GetVertexCount() > 0 && copyItem1.GetVertexCount() == item2.GetVertexCount())
                    {
                        MeshComparer.Instance.AcRTAlignJob(copyItem1.gameObject, item2.gameObject);

                        float distance2 = VertexHelper.GetVertexDistanceEx(copyItem1.transform, item2.gameObject.transform, "CopyDoor2", false);
                        Debug.Log($"distance1:{distance1} distance2:{distance2}");

                        if (distance2 < DistanceSetting.zeroM)
                        {
                            meshAlignCount++;

                            Debug.LogError($"SetDoorShared2(Mesh Aligned)[{i}/{list1.Count} {j}/{list2.Count}] vertexCount12:{vertexCount12} door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                        }
                        else
                        {
                            var scale = t1.localScale;
                            scale.x = -scale.x;
                            t1.localScale = scale;
                            t1.position = t2.position;

                            //float distance3 = MeshHelper.GetVertexDistanceEx(copyItem1.transform, item2.gameObject.transform, "CopyDoor2", false);


                            //Debug.LogWarning($"SetDoorShared2(Not2)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                            distance1 = VertexHelper.GetVertexDistanceEx(t1, t2, "SetDoorShared1", false);
                            if (distance1 < DistanceSetting.zeroM)
                            {
                                Debug.Log($"SetDoorShared3(Pos Aligned Reflected)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                                posAlinCount++;
                                //var copyItem2 = MeshHelper.CopyGO(copyItem1);

                                item2.gameObject.SetActive(false);
                                copyItem1.gameObject.name = item2.gameObject.name + "_New";
                                prefab.AddInstance(item2.gameObject, copyItem1.gameObject);

                                //list1.Remove(door2);
                                instances.Add(item2);

                                list2.Remove(item2);

                                j--;
                                copyItem1 = null;
                                isAligned = true;
                            }
                            else
                            {
                                //Debug.LogWarning($"SetDoorShared5(Not)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");

                                if (align && copyItem1.GetVertexCount() > 0 && copyItem1.GetVertexCount() == item2.GetVertexCount())
                                {
                                    MeshComparer.Instance.AcRTAlignJob(copyItem1.gameObject, item2.gameObject);

                                    distance2 = VertexHelper.GetVertexDistanceEx(copyItem1.transform, item2.gameObject.transform, "CopyDoor2", false);
                                    Debug.Log($"distance1:{distance1} distance2:{distance2}");

                                    if (distance2 < DistanceSetting.zeroM)
                                    {
                                        meshAlignCount++;

                                        Debug.LogError($"SetDoorShared4(Mesh Aligned  Reflected)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"SetDoorShared5(Not2  Reflected)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance:{distance1} {distance1 < DistanceSetting.zeroM}");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"SetDoorShared6(Not1  Reflected)[{i}/{list1.Count} {j}/{list2.Count}] door1:{item1.ToString()} door2:{item2.ToString()} distance1:{distance1} ги{distance1 < DistanceSetting.zeroM}) ");
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"SetDoorShared7(Not1)[{i}/{list1.Count} {j}/{list2.Count}] align:{align} vertexCount12:{vertexCount12} door1:{item1.ToString()} door2:{item2.ToString()} distance1:{distance1} ги{distance1 < DistanceSetting.zeroM}) ");
                    }
                }
            }

            if (copyItem1 != null)
            {
                GameObject.DestroyImmediate(copyItem1.gameObject);
            }

            if (isBreak)
            {
                break;
            }
        }

        prefabsNew.SortByInstanceCount();

        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SetDoorShared count:{count} posAlinCount:{posAlinCount} meshAlignCount:{meshAlignCount} noAlignCount:{noAlignCount} prefabs:{prefabsNew.Count} instances:{instances.Count} time:{DateTime.Now - start}");
        return prefabsNew;
    }
}
