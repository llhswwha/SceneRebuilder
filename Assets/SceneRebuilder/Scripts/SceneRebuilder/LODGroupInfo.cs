using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(LODGroup))]
public class LODGroupInfo : MonoBehaviour
{
    public LODGroup LODGroup;

    public List<LODInfo> LodInfos = new List<LODInfo>();

    public int lodCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("GetLODs")]
    void GetLODs()
    {
        LodInfos.Clear();
        LODGroup = gameObject.GetComponent<LODGroup>();
        LOD[] lods = LODGroup.GetLODs();
        foreach (var lod in lods)
        {
            LODInfo lodInfo = new LODInfo(lod);
            LodInfos.Add(lodInfo);
        }
        lodCount = LODGroup.lodCount;
    }

    [ContextMenu("SetLODs")]
    void SetLODs()
    {
        LODGroup = gameObject.GetComponent<LODGroup>();
        List<LOD> lods = new List<LOD>();
        for (int i = 0; i < LodInfos.Count; i++)
        {
            LODInfo lodInfo = LodInfos[i];
            //lodInfo.screenRelativeTransitionHeight = ls[i];
            LOD lod = lodInfo.GetLOD();
            lods.Add(lod);
        }
        LODGroup.SetLODs(lods.ToArray());
    }

    public float[] ls = new float[] { 0.6f, 0.2f,0.1f,0.01f};

    [ContextMenu("SetDefulatLOD")]
    void SetDefulatLOD()
    {
        LODGroup.SetLODs(CreateLODs(ls));
    }

    [ContextMenu("CreateLOD")]
    void CreateLOD()
    {
        CreateLODs(gameObject, ls);
    }

    public static LOD[] CreateLODs(float[] ls)
    {
        LOD[] lods = new LOD[ls.Length];
        for(int i=0;i<ls.Length;i++)
        {
            LOD lod = new LOD();
            lod.screenRelativeTransitionHeight = ls[i];
            lod.fadeTransitionWidth = 0;
            lods[i] = lod;
        }
        return lods;
    }

    public static LODGroup CreateLODs(GameObject obj,float[] ls)
    {
        LODGroup lodGroup = obj.GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            lodGroup = obj.AddComponent<LODGroup>();
        }
        lodGroup.SetLODs(CreateLODs(ls));
        return lodGroup;
    }
}
