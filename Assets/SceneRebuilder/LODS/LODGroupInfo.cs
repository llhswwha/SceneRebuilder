using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(LODGroup))]
public class LODGroupInfo : MonoBehaviour
{
    public LODGroup LODGroup;

    public List<LODInfo> LodInfos = new List<LODInfo>();

    public int lodCount;


    //public List<int> lodVertexCount=new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("GetLODs")]
    public void GetLODs()
    {
        LodInfos.Clear();
        //lodVertexCount.Clear();
        LODGroup = gameObject.GetComponent<LODGroup>();
        if (LODGroup == null)
        {
            Debug.LogError("LODGroupInfo.GetLODs LODGroup == null");
            return;
        }
        LOD[] lods = LODGroup.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            LOD lod = lods[i];
            LODInfo lodInfo = new LODInfo(lod,i);
            LodInfos.Add(lodInfo);
            int vc=0;
            foreach(var r in lod.renderers){
                MeshFilter meshFilter=r.GetComponent<MeshFilter>();
                vc+=meshFilter.sharedMesh.vertexCount;
            }
            //lodVertexCount.Add(vc);
            lodInfo.vertextCount = vc;
        }
        lodCount = LODGroup.lodCount;

        
    }

    [ContextMenu("SetLODs")]
    public void SetLODs()
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
    public void SetDefulatLOD()
    {
        LODGroup.SetLODs(LODHelper.CreateLODs(ls));
    }

    [ContextMenu("CreateLOD")]
    public void CreateLOD()
    {
        LODHelper.CreateLODs(gameObject, ls);
    }

    //public static LOD[] CreateLODs(float[] ls)
    //{
    //    LOD[] lods = new LOD[ls.Length];
    //    for(int i=0;i<ls.Length;i++)
    //    {
    //        LOD lod = new LOD();
    //        lod.screenRelativeTransitionHeight = ls[i];
    //        lod.fadeTransitionWidth = 0;
    //        lods[i] = lod;
    //    }
    //    return lods;
    //}

    //public static LODGroup CreateLODs(GameObject obj,float[] ls)
    //{
    //    LODGroup lodGroup = obj.GetComponent<LODGroup>();
    //    if (lodGroup == null)
    //    {
    //        lodGroup = obj.AddComponent<LODGroup>();
    //    }
    //    lodGroup.SetLODs(CreateLODs(ls));
    //    return lodGroup;
    //}

    public SubScene_Base scene;

    public void EditorCreateScene()
    {
        scene=LODHelper.SaveLOD0(null, this.LODGroup)[0];
        EditorHelper.ClearOtherScenes();
        EditorHelper.RefreshAssets();
    }

    public void EditorLoadScene()
    {
        IdDictionary.InitInfos();
        if(scene==null)
        {
            scene = gameObject.GetComponent<SubScene_Base>();
        }
        scene.EditorLoadScene();
        SetLOD0FromScene();
    }

    private void SetLOD0FromScene()
    {
        var lods = LODGroup.GetLODs();
        lods[0].renderers = scene.GetSceneRenderers().ToArray();
        LODGroup.SetLODs(lods);
    }

    public void LoadScene()
    {
        IdDictionary.InitInfos();
        if (scene == null)
        {
            scene = gameObject.GetComponent<SubScene_Base>();
        }
        scene.LoadSceneAsync((b, s) =>
        {
            SetLOD0FromScene();
        });
    }
}
