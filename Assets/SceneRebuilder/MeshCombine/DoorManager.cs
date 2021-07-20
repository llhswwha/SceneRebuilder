using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public List<DoorInfo> Doors = new List<DoorInfo>();
    public int VertexCount = 0;
    public int VertexCount_Show = 0;

    public void GetDoors()
    {
        Doors.Clear();
        VertexCount = 0;
        VertexCount_Show = 0;
        DateTime start = DateTime.Now;
        ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", "Start", 0);
        var renderers = GameObject.FindObjectsOfType<MeshRenderer>(true).Where(i => i.name.ToLower().Contains("door")).ToList() ;
        for(int i = 0; i < renderers.Count; i++)
        {

            float progress = (float)i / renderers.Count;
            ProgressBarHelper.DisplayCancelableProgressBar("GetDoors", $"{i}/{renderers.Count} {progress:P1}", progress);
            var parent = renderers[i].transform.parent;
            if (parent.name.ToLower().Contains("combined")) continue;
            DoorInfo door = new DoorInfo(renderers[i]);
            Doors.Add(door);
            VertexCount += door.VertexCount;
            if (renderers[i].gameObject.activeInHierarchy)
            {
                VertexCount_Show += door.VertexCount;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetDoors count:{renderers.Count} VertexCount:{VertexCount} time:{(DateTime.Now-start)}");
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
        return $"mat:{MatCount},mesh:{SubMeshCount},v:{VertexCount},dis:{DisToCenter:F1},off:({OffToCenter.x:F2},{OffToCenter.y:F2},{OffToCenter.z:F2})";
    }
}
