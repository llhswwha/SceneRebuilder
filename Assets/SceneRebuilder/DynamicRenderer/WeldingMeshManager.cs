using AdvancedCullingSystem.DynamicCullingCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingMeshManager : MonoBehaviour
{
    public List<GameObject> WeldingList = new List<GameObject>();
    public MeshRendererInfoList WeldingRenderers = new MeshRendererInfoList();
    public SharedMeshInfoList WeldingSharedMeshInfos;

    [ContextMenu("GetWeldings")]
    public SharedMeshInfoList GetWeldings()
    {
        Debug.Log("GetWeldings");
        WeldingRenderers = new MeshRendererInfoList(RendererManager.FindRenderers(null,"welding"));

        //var rs = GameObject.FindObjectsOfType<MeshRenderer>(true);
        //Debug.Log($"rs:{rs.Length}");

        var weldingFilters = RendererManager.FindComponents<MeshFilter>(null, "welding");
        WeldingSharedMeshInfos = new SharedMeshInfoList(weldingFilters);
        return WeldingSharedMeshInfos;
    }

    public List<MeshRendererInfo> GetMeshInfoList()
    {
        //throw new NotImplementedException();
        return WeldingRenderers;
    }

    public void AddCollider()
    {
        WeldingRenderers.AddCollider();
    }

    public void SetStaticCulling()
    {
        
    }

    public void SetDymicCulling()
    {
#if UNITY_EDITOR
        DynamicCulling.Instance.OnEditorAddStartRenderers(WeldingRenderers.GetAllRenderers());
#endif
    }
}
