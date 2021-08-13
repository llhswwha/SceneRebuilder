using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public DoorInfoList doorInfos;

    public DoorInfoList UpdateDoors()
    {
        MeshRenderer[] renderers = null;
        if (LocalTarget != null)
        {
            renderers = LocalTarget.GetComponentsInChildren<MeshRenderer>(true);
        }
        else
        {
            renderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        }
        var rendererList = renderers.Where(i => i.name.ToLower().Contains("door")).ToList();
        doorInfos = new DoorInfoList(rendererList);
        return doorInfos;
    }

    public DoorInfoList GetDoors()
    {
        DoorInfoList doors = new DoorInfoList();

        foreach(var door in doorInfos)
        {
            if (IsOnlyActive)
            {
                if (door.DoorGo && door.DoorGo.activeInHierarchy == false) continue;
            }
            if (IsOnlyCanSplit)
            {
                if (door.SubMeshCount <= 1) continue;
            }
            doors.Add(door);

            doors.VertexCount += door.VertexCount;
            if (door.DoorGo && door.DoorGo.activeInHierarchy)
                doors.VertexCount_Show += door.VertexCount;
        }
        doors.Sort((a, b) =>
        {
            return b.VertexCount.CompareTo(a.VertexCount);
        });
        return doors;
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
        var doors = GetDoors();
        for (int i = 0; i < doors.Count; i++)
        {
            var door = doors[i];
            float progress = (float)i / doors.Count;
            float percents = progress * 100;
            if (ProgressBarHelper.DisplayCancelableProgressBar("CombinedBuildings", $"Progress1 {i}/{doors.Count} {percents:F2}%  {door.DoorGo.name}", progress))
            {
                break;
            }
            GameObject result = MeshCombineHelper.SplitByMaterials(door.DoorGo);
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
    public static void CopyDoorA(GameObject gameObject,bool align)
    {
        EditorHelper.UnpackPrefab(gameObject);
        var childCount = gameObject.transform.childCount;
        if (childCount == 2)
        {
            var door1 = gameObject.transform.GetChild(0);
            var door2 = gameObject.transform.GetChild(1);

            var scale1 = door1.localScale;
            var scale2 = door2.localScale;
            if (scale1 == Vector3.one || scale2 == Vector3.one)
            {
                if (scale2 == Vector3.one)
                {
                    var tmp = door1;
                    door1 = door2;
                    door2 = tmp;
                }

                GameObject newDoor2 = MeshHelper.CopyGO(door1.gameObject);
                newDoor2.transform.localScale = new Vector3(-1, 1, 1);
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
                        Debug.LogError("¶ÔÆëÊ§°Ü");
                    }
                }
                else
                {
                    Debug.Log($"distance1:{distance1}");
                }


            }
            else
            {
                Debug.LogError($"RendererIdEditor.CopyDoorA scale1!=Vector3.one && scale2 != Vector3.one scale1:{scale1} scale2:{scale2}");
            }
        }
        else
        {
            Debug.LogError("RendererIdEditor.CopyDoorA childCount =!= 2");
        }
    }
}

[Serializable]
public class DoorInfoList: List<DoorInfo>
{
    //public List<DoorInfo> Doors = new List<DoorInfo>();
    public int VertexCount = 0;
    public int VertexCount_Show = 0;


    public DoorInfoList()
    {
        
    }

    public DoorInfoList(List<MeshRenderer> renderers)
    {
        GetDoors(renderers);
    }

    public void GetDoors(List<MeshRenderer> renderers)
    {
        //Doors.Clear();
        VertexCount = 0;
        VertexCount_Show = 0;
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", "Start", 0);
        //var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true).Where(i => i.name.ToLower().Contains("door")).ToList();
        for (int i = 0; i < renderers.Count; i++)
        {

            float progress = (float)i / renderers.Count;
            ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", $"{i}/{renderers.Count} {progress:P1}", progress);
            var parent = renderers[i].transform.parent;
            if (parent != null && parent.name.ToLower().Contains("combined")) continue;
            DoorInfo door = new DoorInfo(renderers[i]);
            this.Add(door);
            VertexCount += door.VertexCount;
            if (renderers[i].gameObject.activeInHierarchy)
            {
                VertexCount_Show += door.VertexCount;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetDoors count:{renderers.Count} VertexCount:{VertexCount} time:{(DateTime.Now - start)}");
        //return Doors;
    }
}

[Serializable]
public class DoorInfo
{
    public string Root;
    public GameObject DoorGo;

    public Vector3 Pos;

    public Vector3 Center;

    public float DisToCenter;

    public Vector3 OffToCenter;

    public int MatCount;

    public string MatNames;

    public int VertexCount;

    public int SubMeshCount;

    public DoorInfo()
    {

    }

    public string GetTitle()
    {
        if (DoorGo == null) return "NULL";
        return $"{Root}>{DoorGo.name}";
    }

    public DoorInfo(MeshRenderer renderer)
    {
        BuildingModelInfo[] models = renderer.gameObject.GetComponentsInParent<BuildingModelInfo>(true);
        if (models.Length > 0)
        {
            Root = models[0].name;
        }
        DoorGo = renderer.gameObject;
        Pos = renderer.transform.position;
        MeshFilter mf = renderer.GetComponent<MeshFilter>();
        var minMax = MeshHelper.GetMinMax(mf);
        Center = minMax[3];
        DisToCenter = Vector3.Distance(Pos, Center);
        OffToCenter = Center - Pos;
        MatCount = renderer.sharedMaterials.Length;
        foreach(var mat in renderer.sharedMaterials)
        {
            if (mat == null) continue;
            MatNames += mat.name + ";";
        }
        VertexCount = mf.sharedMesh.vertexCount;
        SubMeshCount = mf.sharedMesh.subMeshCount;
    }

    public override string ToString()
    {
        //return $"mat:{MatCount},mesh:{SubMeshCount},v:{VertexCount},dis:{DisToCenter:F1},off:({OffToCenter.x:F2},{OffToCenter.y:F2},{OffToCenter.z:F2})";
        return $"mat:{MatCount},mesh:{SubMeshCount},v:{VertexCount},dis:{DisToCenter:F1}";
    }
}
