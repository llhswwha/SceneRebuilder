using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingManager : MonoBehaviour
{
    public List<GameObject> WeldingList = new List<GameObject>();
    public List<MeshRenderer> WeldingRenderers = new List<MeshRenderer>();
    public SharedMeshInfoList WeldingSharedMeshInfos;

    [ContextMenu("GetWeldings")]
    public SharedMeshInfoList GetWeldings()
    {
        Debug.Log("GetWeldings");
        WeldingRenderers = RendererManager.FindRenderers(null,"welding");

        //var rs = GameObject.FindObjectsOfType<MeshRenderer>(true);
        //Debug.Log($"rs:{rs.Length}");

        var weldingFilters = RendererManager.FindComponents<MeshFilter>(null, "welding");
        WeldingSharedMeshInfos = new SharedMeshInfoList(weldingFilters);
        return WeldingSharedMeshInfos;
    }
}
