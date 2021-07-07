using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using LODLevelCaculate;

public class LODManager : MonoBehaviour
{
    private static LODManager _instance;
    public static LODManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LODManager>();
            }
            return _instance;
        }
    }

    public Material[] LODMaterials;

    public float[] LODLevels = new float[] { 0.6f, 0.2f, 0.07f, 0.01f };

    public float[] lodVertexPercents = new float[] { 0.75f,0.5f,0.25f,0.1f};

    public bool isDestroy = true;
    public bool isSaveAsset = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateLOD(GameObject go)
    {
        AutomaticLODHelper.CreateLOD(go, LODMaterials, LODLevels, lodVertexPercents, isDestroy, isSaveAsset);
    }

    public void CreateLOD(GameObject go,Action<float> progressChanged)
    {
        AutomaticLODHelper.CreateLOD(go, LODMaterials, LODLevels, lodVertexPercents, isDestroy, isSaveAsset, progressChanged);
    }

    public void CreateLOD(GameObject go, float percent,Action<float> progressChanged)
    {
        AutomaticLODHelper.CreateLOD(go, LODMaterials, new float[] { 0.5f}, new float[] { percent }, isDestroy, isSaveAsset, progressChanged);
    }

    public void RemoveLOD(GameObject go)
    {
        AutomaticLODHelper.ClearLODAndChildren(go);
    }

    public GameObject TestTarget;

    [ContextMenu("TestCreateLOD")]
    public void TestCreateLOD()
    {
        CreateLOD(TestTarget);
    }

    public GameObject GoLOD0;

    public GameObject GoLOD1;

    public float zeroDistance=0.0002f;

    [ContextMenu("CombineLOD0AndLOD1")]
    public void CombineLOD0AndLOD1()
    {
        DateTime start = DateTime.Now;
        var renderers_1=GoLOD1.GetComponentsInChildren<MeshRenderer>(true);
        var renderers_0=GoLOD0.GetComponentsInChildren<MeshRenderer>(true);
        List<MeshRenderer> renderersList_0=new List<MeshRenderer>();
        renderersList_0.AddRange(renderers_0);
        foreach(var render1 in renderers_1)
        {
            bool isFound=false;
            float minDis=float.MaxValue;
            MeshRenderer minRenderer=null;
            foreach(var render0 in renderersList_0)
            {
                float distance=Vector3.Distance(render0.transform.position,render1.transform.position);
                if(distance<minDis){
                    minDis=distance;
                    minRenderer=render0;
                }
            }
           MeshFilter filter1=render1.GetComponent<MeshFilter>();
            MeshFilter filter0=minRenderer.GetComponent<MeshFilter>();
            int vertexCount0=filter0.sharedMesh.vertexCount;
            int vertexCount1=filter1.sharedMesh.vertexCount;
            if(minDis<=zeroDistance)
            {
                if(vertexCount1==vertexCount0){
                    GameObject.DestroyImmediate(filter1.gameObject);
                }
                else{
                    Debug.Log($"GetDistance1 \tLOD1:{render1.name}({vertexCount1}) \tLOD0:{minRenderer.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1/vertexCount0:P2}");

                    //  LODGroupInfo lODGroup=minRenderer.GetComponent<LODGroupInfo>();
                    //  if(lODGroup==null){
                    //      lODGroup=minRenderer.AddComponent<LODGroupInfo>();
                    //  }

                    render1.transform.SetParent(minRenderer.transform);
                    render1.name+="_LOD1";

                    LODGroup lODGroup=minRenderer.GetComponent<LODGroup>();
                    if(lODGroup==null){
                        lODGroup=minRenderer.gameObject.AddComponent<LODGroup>();
                    }
                    LOD[] lods = new LOD[3];
                    lods[0]=new LOD(0.5f,new Renderer[1]{minRenderer});     //LOD0 >50% 
                    lods[1]=new LOD(0.2f,new Renderer[1]{render1});         //LOD1  > 20% - 50% 
                    // lods[2]=new LOD(0.1f,new Renderer[1]{render1});         //LOD2  > 10% - 20% 
                    lods[2]=new LOD(0.02f,new Renderer[1]{render1});        //LOD3  > 1% - 10% 
                                                                            //Culled > 0% - 1%
                    lODGroup.SetLODs(lods);
                }
            }
            else{
                Debug.LogWarning($"GetDistance1 \tLOD1:{render1.name}({vertexCount1}) \tLOD0:{minRenderer.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1/vertexCount0:P2}");
            }
        }
        Debug.LogError($"CombineLOD0AndLOD1 count1:{renderers_1.Length} count0:{renderers_0.Length} time:{(DateTime.Now - start)}");
    }

    // public List<Renderer> GetLODRenderers()
    // {
    //     List<Renderer> renderers=new List<Renderer>();
    //     DateTime start = DateTime.Now;
    //     var lodGroups=GameObject.FindObjectsOfType<LODGroup>();
    //     foreach(LODGroup group in lodGroups){
    //         var lods=group.GetLODs();
    //         for(int i=0;i<lods.Length;i++)
    //         {
    //             var lod=lods[i];
    //             foreach(var renderer in lod.renderers)
    //             {
    //                 if(renderer==null)continue;
    //                 renderers.Add(renderer);
    //             }
    //         }
    //     }
    //     Debug.LogError($"GetLODRenderers lodGroups:{lodGroups.Length} time:{(DateTime.Now - start)}");
    //     return renderers;
    // }

    public Dictionary<Renderer,Renderer> GetLODRendererDict()
    {
        Dictionary<Renderer,Renderer> dict=new Dictionary<Renderer, Renderer>();
        var lodGroups=GameObject.FindObjectsOfType<LODGroup>();
         foreach(LODGroup group in lodGroups){
            var lods=group.GetLODs();
            for(int i=0;i<lods.Length;i++)
            {
                var lod=lods[i];
                foreach(var renderer in lod.renderers)
                {
                    if(renderer==null)continue;
                    // renderers.Add(renderer);
                    if(!dict.ContainsKey(renderer))
                    {
                        dict.Add(renderer,renderer);
                    }
                }
            }
        }

        // List<Renderer> renderers=GetLODRenderers();
        // foreach(var render in renderers){
        //     if(!dict.ContainsKey(render)){
        //         dict.Add(render,render);
        //     }
        //     else{
        //         Debug.LogError("GetLODRendererDict 重复 :"+render);
        //     }
        // }

        return dict;
    }

    [ContextMenu("SetLODMatColor")]
    public void SetLODMatColor()
    {
        DateTime start = DateTime.Now;
        var lodGroups=GameObject.FindObjectsOfType<LODGroup>();
        foreach(LODGroup group in lodGroups){
            var lods=group.GetLODs();
            for(int i=0;i<lods.Length;i++)
            //foreach(var lod in lods)
            {
                var lod=lods[i];
                foreach(var renderer in lod.renderers)
                {
                    if(renderer==null)continue;
                    // renderer.material = LODMaterials[i];

                    // var mats=renderer.materials;
                    // for(int j=0;j<mats.Length;j++)
                    // {
                    //     mats[j]=LODMaterials[i];
                    // }
                    // renderer.materials=mats;

                    var mats=renderer.materials;
                    for(int j=0;j<mats.Length;j++)
                    {
                        mats[j]=LODMaterials[i];
                    }
                    renderer.materials=mats;
                }
            }
            
        }
        Debug.LogError($"SetLODMatColor lodGroups:{lodGroups.Length} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("DisableLOD")]
    public void DisableLOD()
    {
        SetLODEnabled(false);
    }

    public void SetLODEnabled(bool isEnabled)
    {
        DateTime start = DateTime.Now;
        var lodGroups=GameObject.FindObjectsOfType<LODGroup>();
        foreach(LODGroup group in lodGroups){
            var lods=group.GetLODs();
            for(int i=1;i<lods.Length;i++)
            //foreach(var lod in lods)
            {
                var lod=lods[i];
                foreach(var renderer in lod.renderers)
                {
                    if(renderer==null)continue;
                    renderer.enabled=isEnabled;
                }
            }
            group.enabled=isEnabled;
        }
        Debug.LogError($"DisableLOD lodGroups:{lodGroups.Length} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("EnableLOD")]
    public void EnableLOD()
    {
        SetLODEnabled(true);
    }

    [ContextMenu("RemoveLOD")]
    public void RemoveLOD()
    {
    }

    public List<LODGroupDetails> lodInfos;

    [ContextMenu("GetLODInfo")]
    public string GetLODInfo()
    {
        DateTime now = DateTime.Now;
        if(lodInfos==null||lodInfos.Count==0){
            lodInfos=LODGroupDetails.GetSceneLodGroupInfo();
        }
        
        Camera cam = GameObject.FindObjectOfType<Camera>();

        int[] infos=LODGroupDetails.CaculateGroupInfo(lodInfos,LODSceneView.GameView,LODSortType.Vertex,cam);

        int[] lodCount=new int[5];
        int[] lodVertexCount=new int[5];
        float[] lodPercent=new float[5];
        float allVertex0=0;
        foreach(LODGroupDetails lodI in lodInfos)
        {
            if(lodI.currentChild==null)continue;
            lodCount[lodI.currentInfo.currentLevel]++;
            lodVertexCount[lodI.currentInfo.currentLevel]+=lodI.currentChild.vertexCount;
            lodPercent[lodI.currentInfo.currentLevel]+=lodI.currentChild.vertexPercent;

            allVertex0+=lodI.childs[0].vertexCount;
            // foreach(var child in lodI.childs){
            //     allVertex0+=child.vertexCount;
            // }
        }

        // string lodInfoTxt="";
        // for(int i=0;i<lodCount.Length;i++){
        //     lodInfoTxt+=$"LOD{i}({lodCount[i]},{lodVertexCount[i]/10000f:F1},{lodPercent[i]:P1}) ";
        // }

        string lodInfoTxt_count="";
        string lodInfoTxt_vertex="";
        string lodInfoTxt_percent="";
        for(int i=0;i<lodCount.Length;i++){
            lodInfoTxt_count+=$"L{i}({lodCount[i]})\t\t";
            lodInfoTxt_vertex+=$"L{i}({lodVertexCount[i]/10000f:F0})\t\t";
            lodInfoTxt_percent+=$"L{i}({lodPercent[i]:P1})\t";
        }

        var allVertexCount=infos[0];
        var allMeshCount=infos[1];
        double time=(DateTime.Now-now).TotalMilliseconds;
        string info=$"LOD:{lodInfos.Count}, Vertex:{allVertexCount/10000f:F0}/{allVertex0/10000f:F0}, Mesh:{allMeshCount}, t:{time:F0}ms";
        // info+="\n"+lodInfoTxt;
        info+="\n"+lodInfoTxt_count+"\n"+lodInfoTxt_vertex+"\n"+lodInfoTxt_percent;

        //Debug.Log($"CaculateGroupInfo完成，耗时{time}ms "+info);
        return info;
    }
}
