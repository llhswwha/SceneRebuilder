using MeshJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabInfo:IComparable<PrefabInfo>
{
    public PrefabInfo(GameObject prefab){
        this.Prefab=prefab;
        if (prefab == null)
        {
            Debug.LogError("PrefabInfo.ctor prefab == null");
            return;
        }
        Init(new MeshPoints(prefab));
    }

    public PrefabInfo(MeshPoints mf)
    {
        Init(mf);
    }

    private void Init(MeshPoints mf)
    {
        this.MeshFilter = mf;
        this.Prefab = mf.gameObject;
        this.VertexCount = mf.vertexCount;
        this.Size = mf.size;
    }

    public string GetTitle()
    {
        if (Prefab == null)
        {
            return $"NULL({Instances.Count})";
        }
        return $"{Prefab.name}({Instances.Count})";
    }

    public override string ToString()
    {
        return $"{this.VertexCount}*{Instances.Count}={this.VertexCount* Instances.Count}|{this.MeshFilter.size}";
    }

    public GameObject Prefab;

    public GameObject PrefAsset;

    public MeshPoints MeshFilter;

    public List<GameObject> Instances=new List<GameObject>();

    public List<GameObject> InstancesNew = new List<GameObject>();

    public List<GameObject> GetInstances()
    {
        return Instances;
    }

    public void DestroyInstances()
    {
        foreach (var ins in Instances)
        {
            if (ins != null)
                GameObject.DestroyImmediate(ins);
        }
        Instances.Clear();
        foreach (var ins in InstancesNew)
        {
            if (ins != null)
                GameObject.DestroyImmediate(ins);
        }
        InstancesNew.Clear();
    }

    public int VertexCount=0;

    public float SizeVolumn
    {
      get{
        return Size.x*Size.y*Size.z;
      }
      
    }

    public Vector3 Size=Vector3.zero;

    public int _InstanceCount;

    public int InstanceCount
    {
        get
        {
            if(Instances==null)return 0;
            _InstanceCount=Instances.Count;
            return Instances.Count;
        }
    }

    public void AddInstance(GameObject instance){
        Instances.Add(instance);
        //InstanceCount++;
    }

    public void AddInstance(GameObject instance, GameObject instanceNew)
    {
        Instances.Add(instance);
        InstancesNew.Add(instanceNew);
        //InstanceCount++;
    }

    public int CompareTo(PrefabInfo other)
  {
    return other.InstanceCount.CompareTo(this.InstanceCount);
  }

    internal void ApplyReplace()
    {
        if (Instances.Count == InstancesNew.Count)
        {
            for(int i=0;i< Instances.Count;i++)
            {
                var ins = Instances[i];
                var ins2 = InstancesNew[i];
                ins2.name = ins.name;
                GameObject.DestroyImmediate(ins);
            }
        }
        else
        {
            Debug.LogError($"PrefabInfo.Repalce Instances.Count != InstancesNew.Count Prefab:{Prefab} Instances:{Instances.Count} InstancesNew:{InstancesNew.Count}");
        }
    }

    internal void RevertReplace()
    {
        if (Instances.Count == InstancesNew.Count)
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                var ins = Instances[i];
                var ins2 = InstancesNew[i];
                GameObject.DestroyImmediate(ins2);
                ins.SetActive(true);
            }
        }
        else
        {
            Debug.LogError($"PrefabInfo.Repalce Instances.Count != InstancesNew.Count Prefab:{Prefab} Instances:{Instances.Count} InstancesNew:{InstancesNew.Count}");
        }
    }

    internal void ShowOri()
    {
        if (Instances.Count == InstancesNew.Count)
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                var ins = Instances[i];
                var ins2 = InstancesNew[i];
                ins2.SetActive(false);
                ins.SetActive(true);
            }
        }
        else
        {
            Debug.LogError($"PrefabInfo.Repalce Instances.Count != InstancesNew.Count Prefab:{Prefab} Instances:{Instances.Count} InstancesNew:{InstancesNew.Count}");
        }
    }

    internal void ShowNew()
    {
        if (Instances.Count == InstancesNew.Count)
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                var ins = Instances[i];
                var ins2 = InstancesNew[i];
                ins2.SetActive(true);
                ins.SetActive(false);
            }
        }
        else
        {
            Debug.LogError($"PrefabInfo.Repalce Instances.Count != InstancesNew.Count Prefab:{Prefab} Instances:{Instances.Count} InstancesNew:{InstancesNew.Count}");
        }
    }
}

[Serializable]
public class PrefabInfoListBags
{
    public PrefabInfoList PrefabInfoList = new PrefabInfoList();
    public PrefabInfoList PrefabInfoList1 = new PrefabInfoList();
    public PrefabInfoList PrefabInfoList2 = new PrefabInfoList();
    public PrefabInfoList PrefabInfoList3 = new PrefabInfoList();
    public PrefabInfoList PrefabInfoList4 = new PrefabInfoList();
    public PrefabInfoList PrefabInfoList5 = new PrefabInfoList();
    public PrefabInfoList PrefabInfoList6 = new PrefabInfoList();

    public PrefabInfoListBags()
    {
        
    }

    public PrefabInfoListBags(PrefabInfoList PrefabInfoList,int[] InsCountList)
    {
        this.PrefabInfoList = PrefabInfoList;
        SetPrefabInfoList(PrefabInfoList, InsCountList);
    }

    public void SetPrefabInfoList(PrefabInfoList list, int[] InsCountList)
    {
        //PrefabInfoList.Clear();
        PrefabInfoList = list;

        ClearPrefabs16();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].InstanceCount < InsCountList[0])//1
            {
                PrefabInfoList1.Add(list[i]);
            }
            else if (list[i].InstanceCount < InsCountList[1])//5
            {
                PrefabInfoList2.Add(list[i]);
            }
            else if (list[i].InstanceCount < InsCountList[2])//10
            {
                PrefabInfoList3.Add(list[i]);
            }
            else if (list[i].InstanceCount < InsCountList[3])//50
            {
                PrefabInfoList4.Add(list[i]);
            }
            else if (list[i].InstanceCount < InsCountList[4])//100
            {
                PrefabInfoList5.Add(list[i]);
            }
            else
            {
                PrefabInfoList6.Add(list[i]);
            }
        }

        Debug.LogError($"SetPrefabInfoList all:{PrefabInfoList.Count}={PrefabInfoList.GetInstanceCount()}\t list1(1):{PrefabInfoList1.Count}={PrefabInfoList1.GetInstanceCount()}\t list2(2-4):{PrefabInfoList2.Count}={PrefabInfoList2.GetInstanceCount()}\t list3(5-9):{PrefabInfoList3.Count}={PrefabInfoList3.GetInstanceCount()}\t list4(10-49):{PrefabInfoList4.Count}={PrefabInfoList4.GetInstanceCount()}\t list5(50-99):{PrefabInfoList5.Count}={PrefabInfoList5.GetInstanceCount()} \tlist6(>=100):{PrefabInfoList6.Count}={PrefabInfoList6.GetInstanceCount()},");
    }

    public List<MeshRenderer> GetHiddenRenderers()
    {
        List<MeshRenderer> list = new List<MeshRenderer>();
        list.AddRange(PrefabInfoList5.GetRenderers());
        list.AddRange(PrefabInfoList6.GetRenderers());
        Debug.LogError("GetHiddenRenderers " + list.Count);
        return list;
    }

    public void RemoveInstances1()
    {
        PrefabInfoList1.RemoveInstances();
        PrefabInfoList2.RemoveInstances();
        PrefabInfoList3.RemoveInstances();
        PrefabInfoList4.RemoveInstances();
    }

    public void RemoveInstances2()
    {
        PrefabInfoList5.RemoveInstances();
        PrefabInfoList6.RemoveInstances();
    }

    public void HideInstances1()
    {
        PrefabInfoList1.HideInstances();
        PrefabInfoList2.HideInstances();
        PrefabInfoList3.HideInstances();
        PrefabInfoList4.RemoveInstances();
    }

    public void HideInstances2()
    {
        //PrefabInfoList4.RemoveInstances();
        PrefabInfoList5.HideInstances();
        PrefabInfoList6.HideInstances();
    }

    public void ShowInstances1()
    {
        PrefabInfoList1.ShowInstances();
        PrefabInfoList2.ShowInstances();
        PrefabInfoList3.ShowInstances();
        PrefabInfoList4.ShowInstances();
    }

    public void ShowInstances2()
    {
        //PrefabInfoList4.RemoveInstances();
        PrefabInfoList5.ShowInstances();
        PrefabInfoList6.ShowInstances();
    }

    public void ClearPrefabs16()
    {
        PrefabInfoList1.Clear();
        PrefabInfoList2.Clear();
        PrefabInfoList3.Clear();
        PrefabInfoList4.Clear();
        PrefabInfoList5.Clear();
        PrefabInfoList6.Clear();
    }

    public void ClearPrefabs()
    {
        PrefabInfoList.Clear();
        ClearPrefabs16();
    }

    public void ShowPrefabListCount()
    {
        DateTime start = DateTime.Now;
        Debug.Log($"all:{PrefabInfoList.Count}={PrefabInfoList.GetInstanceCount()}\t list1:{PrefabInfoList1.Count}={PrefabInfoList1.GetInstanceCount()}\t list2:{PrefabInfoList2.Count}={PrefabInfoList2.GetInstanceCount()}\t list3:{PrefabInfoList3.Count}={PrefabInfoList3.GetInstanceCount()}\t list4:{PrefabInfoList4.Count}={PrefabInfoList4.GetInstanceCount()}\t list5:{PrefabInfoList5.Count}={PrefabInfoList5.GetInstanceCount()} \tlist6:{PrefabInfoList6.Count}={PrefabInfoList6.GetInstanceCount()},");
        Debug.Log($"ShowPrefabListCount Time:{(DateTime.Now - start).ToString()}");
    }
}

[Serializable]
public class PrefabInfoList: List<PrefabInfo>
{
    public PrefabInfoList()
    {
        
    }
    public PrefabInfoList(PrefabInfoList list)
    {
        this.AddRange(list);
    }

    //public void ShowPrefabCount()
    //{
    //    int count1 = 0;
    //    int count2 = 0;
    //    int count3 = 0;
    //    int count4 = 0;
    //    int count5 = 0;
    //    int count6_10 = 0;
    //    int count11_20 = 0;
    //    int count21_100 = 0;
    //    int count101_500 = 0;
    //    int count500B = 0;
    //    foreach (var info in this)
    //    {
    //        int count = info.InstanceCount + 1;
    //        if (count == 1)
    //        {
    //            count1++;
    //        }
    //        else if (count == 2)
    //        {
    //            count2++;
    //        }
    //        else if (count == 3)
    //        {
    //            count3++;
    //        }
    //        else if (count == 4)
    //        {
    //            count4++;
    //        }
    //        else if (count == 5)
    //        {
    //            count5++;
    //        }
    //        else if (count <= 10)
    //        {
    //            count6_10++;
    //        }
    //        else if (count <= 20)
    //        {
    //            count11_20++;
    //        }
    //        else if (count <= 100)
    //        {
    //            count21_100++;
    //        }
    //        else if (count <= 500)
    //        {
    //            count101_500++;
    //        }
    //        else
    //        {
    //            count500B++;
    //        }
    //    }

    //    Debug.Log($"1={count1};2={count2};3={count3};4={count4};5={count5};6_10={count6_10};11_20={count11_20};21_100={count21_100};101_500={count101_500};>500={count500B};");
    //    Debug.Log($"1\t{count1}\n2\t{count2}\n3\t{count3}\n4\t{count4}\n5\t{count5}\n6_10\t{count6_10}\n11_20\t{count11_20}\n21_100\t{count21_100}\n101_500\t{count101_500}\n>500\t{count500B}\n");
    //}

    public void ShowInstanceCount()
    {
        int count1 = 0;
        int count2 = 0;
        int count3 = 0;
        int count4 = 0;
        int count5 = 0;
        int count6_10 = 0;
        int count11_20 = 0;
        int count21_100 = 0;
        int count101_500 = 0;
        int count500B = 0;
        foreach (var info in this)
        {
            int count = info.InstanceCount + 1;
            if (count == 1)
            {
                count1 += count;
            }
            else if (count == 2)
            {
                count2 += count;
            }
            else if (count == 3)
            {
                count3 += count;
            }
            else if (count == 4)
            {
                count4 += count;
            }
            else if (count == 5)
            {
                count5 += count;
            }
            else if (count <= 10)
            {
                count6_10 += count;
            }
            else if (count <= 20)
            {
                count11_20 += count;
            }
            else if (count <= 100)
            {
                count21_100 += count;
            }
            else if (count <= 500)
            {
                count101_500 += count;
            }
            else
            {
                count500B += count;
            }
        }

        Debug.Log($"1={count1};2={count2};3={count3};4={count4};5={count5};6_10={count6_10};11_20={count11_20};21_100={count21_100};101_500={count101_500};>500={count500B};");
        Debug.Log($"1\t{count1}\n2\t{count2}\n3\t{count3}\n4\t{count4}\n5\t{count5}\n6_10\t{count6_10}\n11_20\t{count11_20}\n21_100\t{count21_100}\n101_500\t{count101_500}\n>500\t{count500B}\n");
    }


    public void DestroyInstances()
    {
        foreach (var info in this)
        {
            info.DestroyInstances();
        }
    }

    public void ApplyReplace()
    {
        for (int i = 0; i < this.Count; i++)
        {
            this[i].ApplyReplace();
        }
    }

    public void ResetPrefabs()
    {
        for (int i = 0; i < this.Count; i++)
        {
            //this[i].ApplyReplace();
            var t = this[i].Prefab.transform;
            
            
            t.localPosition = Vector3.one;
            t.rotation = Quaternion.identity;
            //t.localScale = Vector3.one;
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

    public void SortByInstanceCount()
    {
        this.Sort((a, b) => b.InstanceCount.CompareTo(a.InstanceCount));
    }
}

public interface IPrefab<T>
{
    public GameObject gameObject { get; set; }

    public Transform transform { get; }

    public void PreparePrefab();

    public T Clone();

    public int GetVertexCount();

    public List<MeshFilter> GetMeshFilters();
}

[Serializable]
public static class PrefabInfoListHelper
{
    public static int GetInstanceCount(this PrefabInfoList list)
    {
        int count = 0;
        for(int i=0;i< list.Count;i++)
        {
            count += list[i].InstanceCount+1;
        }
        return count;
    }

    public static List<MeshRenderer> GetRenderers(this PrefabInfoList list)
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        for (int i = 0; i < list.Count; i++)
        {
            var prefabInfo=list[i];
            if(prefabInfo==null)
            {
                Debug.LogError($"[{i}] prefabInfo==null");
                continue;
            }
            if(prefabInfo.Prefab==null)
            {
                Debug.LogError($"[{i}] prefabInfo.Prefab==null {prefabInfo}");
                continue;
            }
            var prefabRenderer=prefabInfo.Prefab.GetComponent<MeshRenderer>();
            renderers.Add(prefabRenderer);
            var insList = prefabInfo.GetInstances();
            if(insList==null)continue;
            for (int j=0;j< insList.Count; j++)
            {
                if(insList[j]==null)continue;
                MeshRenderer renderer=insList[j].GetComponent<MeshRenderer>();
                if(renderer==null)continue;
                renderers.Add(renderer);
            }
        }
        return renderers;
    }

    public static int RemoveInstances(this PrefabInfoList list)
    {
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var insList = list[i].GetInstances();
            for (int j=0;j< insList.Count; j++)
            {
                GameObject.DestroyImmediate(insList[j]);
            }
            GameObject.DestroyImmediate(list[i].Prefab);
        }
        return count;
    }

    public static void HideInstances(this PrefabInfoList list)
    {
        SetInstancesVisible(list,false);
    }

    public static void ShowInstances(this PrefabInfoList list)
    {
        SetInstancesVisible(list,true);
    }

    public static void SetInstancesVisible(this PrefabInfoList list,bool isVisible)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var insList = list[i].GetInstances();
            for (int j=0;j< insList.Count; j++)
            {
                insList[j].SetActive(isVisible);
            }
            list[i].Prefab.SetActive(isVisible);
        }
    }

    public static PrefabInfoList GetPrefabInfos<T>(List<T> doors,bool align) where T: class,IPrefab<T>
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


            T copyItem1 = item1.Clone() as T;
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
                    copyItem1 = item1.Clone() as T;
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

                float distance1 = MeshHelper.GetVertexDistanceEx(t1, t2, "SetDoorShared1", false);

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

                        float distance2 = MeshHelper.GetVertexDistanceEx(copyItem1.transform, item2.gameObject.transform, "CopyDoor2", false);
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
                            distance1 = MeshHelper.GetVertexDistanceEx(t1, t2, "SetDoorShared1", false);
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

                                    distance2 = MeshHelper.GetVertexDistanceEx(copyItem1.transform, item2.gameObject.transform, "CopyDoor2", false);
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
