using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
// using LODLevelCaculate;

public class LODManager : SingletonBehaviour<LODManager>
{
    //public void CheckLODPositions()
    //{
    //    var lodGroups = GameObject.FindObjectsOfType<LODGroup>(true);
    //    for (int i1 = 0; i1 < lodGroups.Length; i1++)
    //    {
    //        LODGroup group = lodGroups[i1];
    //        float progress = (float)i1 / lodGroups.Length;

    //        ProgressBarHelper.DisplayCancelableProgressBar("SetRenderersLODInfo", $"{i1}/{lodGroups.Length} {progress:P1} group:{group}", progress);

    //        LOD[] lods = group.GetLODs();
    //        for (int i = 0; i < lods.Length; i++)
    //        {
    //            LOD lod = lods[i];
    //            //LODInfo lodInfo = new LODInfo(lod);
    //            foreach (var r in lod.renderers)
    //            {
    //                if (r == null) continue;
    //                MeshRendererInfo rendererInfo = MeshRendererInfo.GetInfo(r.gameObject);
    //                //Debug.Log($"renderer:{r},parent:{r.transform.parent.name},path:{r.transform.GetPathToRoot()},rendererInfo:{rendererInfo}");
    //                rendererInfo.LodId = i;
    //            }
    //            //break;
    //        }
    //        //break;
    //    }
    //    ProgressBarHelper.ClearProgressBar();
    //}

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

    public float zeroDistance=0.0002f;

    public GameObject GroupRoot;

    public GameObject LODnRoot;

    public MeshRenderer CreateSimplifier(MeshRenderer lod0,float percent)
    {
        GameObject newObj = MeshHelper.CopyGO(lod0.gameObject);
        MeshRenderer renderer = newObj.GetComponent<MeshRenderer>();
        newObj.name += "_" + percent;
        return renderer;
    }

    private void AppendLodInner(int lodLevel)
    {
        twoList.Clear();
        EditorHelper.UnpackPrefab(GroupRoot, PrefabUnpackMode.OutermostRoot);
        EditorHelper.UnpackPrefab(LODnRoot, PrefabUnpackMode.OutermostRoot);
        DateTime start = DateTime.Now;
        var renderers_2 = LODnRoot.GetComponentsInChildren<MeshRenderer>(true);
        var renderers_0 = MeshRendererInfo.GetLod0s(GroupRoot);
        List<MeshRendererInfo> list_lod0 = new List<MeshRendererInfo>();
        list_lod0.AddRange(renderers_0);
        //renderers_0.ToList().ForEach(i => { ts.Add(i.transform); });
        for (int i = 0; i < renderers_2.Length; i++)
        {
            MeshRenderer render_lod1 = renderers_2[i];

            float progress = (float)i / renderers_2.Length;
            ProgressBarHelper.DisplayCancelableProgressBar("CombineLOD0AndLOD1", $"{i}/{renderers_2.Length} {progress:P1} MeshRenderer:{render_lod1.name}", progress);

            var min = GetMinDisTransform<MeshRendererInfo>(list_lod0, render_lod1.transform);
            float minDis = min.dis;
            MeshRenderer render_lod0 = min.target.meshRenderer;

            MeshFilter filter1 = render_lod1.GetComponent<MeshFilter>();
            MeshFilter filter0 = render_lod0.GetComponent<MeshFilter>();
            int vertexCount0 = filter0.sharedMesh.vertexCount;
            int vertexCount1 = filter1.sharedMesh.vertexCount;
            if (minDis <= zeroDistance)
            {
                if (DoCreateGroup)
                {
                    if (vertexCount1 == vertexCount0)
                    {
                        //GameObject.DestroyImmediate(filter1.gameObject);
                        render_lod1 = CreateSimplifier(render_lod0, 0.5f);
                        GameObject.DestroyImmediate(filter1.gameObject);
                    }

                    //else
                    {
                        Debug.Log($"GetDistance1 \tLOD3:{render_lod1.name}({vertexCount1}) \tLOD0:{render_lod0.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1 / vertexCount0:P2}");
                        render_lod1.transform.SetParent(render_lod0.transform);
                        render_lod1.name += "_LOD"+lodLevel;

                        if (lodLevel == 1)
                        {
                            AddLOD1(render_lod0, render_lod1);
                        }
                        else if (lodLevel == 2)
                        {
                            AddLOD2(render_lod0, render_lod1);
                        }
                        else if (lodLevel == 3)
                        {
                            AddLOD3(render_lod0, render_lod1);
                        }
                        
                    }
                }

                twoList.Add(new TwoRenderers(render_lod0, render_lod1, minDis, min.meshDis));
            }
            else
            {
                Debug.LogWarning($"GetDistance1 \tLOD3:{render_lod1.name}({vertexCount1}) \tLOD0:{render_lod0.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1 / vertexCount0:P2}");

                twoList.Add(new TwoRenderers(render_lod0, render_lod1, minDis, min.meshDis));
            }
        }
        ProgressBarHelper.ClearProgressBar();

        twoList.Sort((a, b) =>
        {
            return b.dis.CompareTo(a.dis);
        });

        RemoveEmpty(LODnRoot.transform);
        RemoveEmpty(LODnRoot.transform);

        SetRenderersLODInfo();

        Debug.LogError($"AppendLod3ToGroup count1:{renderers_2.Length} count0:{renderers_0.Count} time:{(DateTime.Now - start)}");
    }

    [ContextMenu("AppendLod1ToGroup")]
    public void AppendLod1ToGroup()
    {
        AppendLodInner(1);
    }

    [ContextMenu("AppendLod2ToGroup")]
    public void AppendLod2ToGroup()
    {
        AppendLodInner(2);
    }

    [ContextMenu("AppendLod2ToGroup")]
    public void AppendLod3ToGroup()
    {
        AppendLodInner(3);
    }

    [Serializable]
    public class MinDisTarget<T> where T :Component
    {
        public float dis = float.MaxValue;
        public float meshDis = float.MaxValue;
        public T target = null;
        public MinDisTarget(float dis,float meshDis, T t)
        {
            this.dis = dis;
            target = t;
            this.meshDis = meshDis;
        }
    }

    private float GetCenterDistance(GameObject go1,GameObject go2)
    {
        MeshRendererInfo info0 = MeshRendererInfo.GetInfo(go1);
        var center0 = info0.center;
        MeshRendererInfo info1 = MeshRendererInfo.GetInfo(go2);
        var center1 = info1.center;
        float distance = Vector3.Distance(center0, center1);
        return distance;
    }

    private MinDisTarget<T> GetMinDisTransform<T>(List<T> ts,Transform t) where T : Component
    {
        float minDis = float.MaxValue;
        float minDisEx = float.MaxValue;
        T minT = null;
        T minTEx = null;
        foreach (var item in ts)
        {
            float distance = Vector3.Distance(item.transform.position, t.position);

            //float distance = GetCenterDistance(item.gameObject, t.gameObject);

            if (distance < 1)
            {
                float distance2 = MeshHelper.GetAvgVertexDistanceEx(item.transform, t);
                if (distance2 < minDisEx)
                {
                    minDisEx = distance2;
                    minTEx = item;
                }
            }

            //float distance = distance2;
            if (distance < minDis)
            {
                minDis = distance;
                minT = item;
            }
        }

        float distance3 = MeshHelper.GetAvgVertexDistanceEx(minT.transform, t);
        if (distance3 < minDisEx)
        {
            minDisEx = distance3;
            minTEx = minT;
        }

        if (minTEx == null)
        {
            minTEx = minT;
        }
        //float dis4 = GetCenterDistance(minTEx.gameObject, t.gameObject);

        //minDisEx = MeshHelper.GetAvgVertexDistanceEx(minT.transform, t);
        //if (distance3 < minDisEx)
        //{
        //    minDisEx = distance3;
        //    minTEx = minT;
        //}

        return new MinDisTarget<T>(minDis,minDisEx, minTEx);
    }

    private void RemoveEmpty(Transform root)
    {
        Transform[] cs= root.GetComponentsInChildren<Transform>(true);
        foreach(var c in cs)
        {
            if (c.childCount > 0) continue;
            MeshRenderer meshRenderer = c.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                GameObject.DestroyImmediate(c.gameObject);
            }
        }
    }

    [Serializable]
    public class TwoRenderers
    {
        public MeshRenderer renderer_lod0;
        public MeshRenderer renderer_lod1;
        public float dis;

        public float meshDis;

        public TwoRenderers(MeshRenderer lod0, MeshRenderer lod1,float d, float meshD)
        {
            renderer_lod0 = lod0;
            renderer_lod1 = lod1;
            dis = d;
            meshDis = meshD;
        }
    }

    public List<TwoRenderers> twoList = new List<TwoRenderers>();

    public bool DoCreateGroup = false;

    //[ContextMenu("CombineLOD0AndLOD1")]
    //public List<TwoRenderers> CombineLOD0AndLOD1()
    //{
    //    UpackPrefab_One(GoLOD1);
    //    UpackPrefab_One(GoLOD0);
    //    DateTime start = DateTime.Now;
    //    var renderers_1=GoLOD1.GetComponentsInChildren<MeshRenderer>(true);
    //    var renderers_0=GoLOD0.GetComponentsInChildren<MeshRenderer>(true);

    //    twoList.Clear();
    //    List<MeshRenderer> list_lod0 = new List<MeshRenderer>();
    //    list_lod0.AddRange(renderers_0);
    //    //renderers_0.ToList().ForEach(i => { ts.Add(i.transform); });
    //    //List<TwoRenderers> twoList = new List<TwoRenderers>();
    //    for (int i = 0; i < renderers_1.Length; i++)
    //    {
    //        MeshRenderer render_lod1 = renderers_1[i];
    //        float progress = (float)i / renderers_1.Length;
    //        ProgressBarHelper.DisplayCancelableProgressBar("CombineLOD0AndLOD1", $"{i}/{renderers_1.Length} {progress:P1} MeshRenderer:{render_lod1.name}", progress);

    //        var min = GetMinDisTransform<MeshRenderer>(list_lod0, render_lod1.transform);
    //        float minDis = min.dis;
    //        MeshRenderer render_lod0 = min.target;

    //        MeshFilter filter1 =render_lod1.GetComponent<MeshFilter>();
    //        MeshFilter filter0=render_lod0.GetComponent<MeshFilter>();
    //        int vertexCount0=filter0.sharedMesh.vertexCount;
    //        int vertexCount1=filter1.sharedMesh.vertexCount;
    //        if(minDis<=zeroDistance)
    //        {
    //            if(DoCreateGroup)
    //            {
    //                if (vertexCount1 == vertexCount0)
    //                {
    //                    //GameObject.DestroyImmediate(filter1.gameObject);
    //                    render_lod1 = CreateSimplifier(render_lod0, 0.5f);
    //                    GameObject.DestroyImmediate(filter1.gameObject);
    //                }

    //                //else
    //                {
    //                    Debug.Log($"GetDistance1 \tLOD1:{render_lod1.name}({vertexCount1}) \tLOD0:{render_lod0.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1 / vertexCount0:P2}");
    //                    render_lod1.transform.SetParent(render_lod0.transform);
    //                    render_lod1.name += "_LOD1";

    //                    CreateLODGroup_01(render_lod1, render_lod0);
    //                }
    //            }


    //            twoList.Add(new TwoRenderers(render_lod0, render_lod1, minDis, min.meshDis));
    //        }
    //        else{
    //            Debug.LogWarning($"GetDistance1 \tLOD1:{render_lod1.name}({vertexCount1}) \tLOD0:{render_lod0.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1/vertexCount0:P2}");
    //            twoList.Add(new TwoRenderers(render_lod0, render_lod1, minDis, min.meshDis));
    //        }
    //    }
    //    ProgressBarHelper.ClearProgressBar();

    //    twoList.Sort((a, b) =>
    //    {
    //        return b.dis.CompareTo(a.dis);
    //    });

    //    RemoveEmpty(GoLOD1.transform);
    //    RemoveEmpty(GoLOD1.transform);
    //    RemoveEmpty(GoLOD1.transform);

    //    Debug.LogError($"CombineLOD0AndLOD1 count1:{renderers_1.Length} count0:{renderers_0.Length} time:{(DateTime.Now - start)}");
    //    return twoList;
    //}

    public void AddLOD3(MeshRenderer lod0, MeshRenderer lod2)
    {
        lod2.transform.SetParent(lod0.transform);
        LODGroup group = lod0.GetComponent<LODGroup>();
        if (group != null)
        {
            LOD[] lods = group.GetLODs();
            LOD[] lodsNew = LODGroupInfo.CreateLODs(LODLevels_3);
            for (int i = 0; i < lods.Length && i < lodsNew.Length; i++)
            {
                lodsNew[i].renderers = lods[i].renderers;
            }
            List<Renderer> renderers = new List<Renderer>();
            if (lodsNew[lodsNew.Length - 1].renderers != null)
                renderers.AddRange(lodsNew[lodsNew.Length - 1].renderers);
            renderers.Add(lod2);
            lodsNew[lodsNew.Length - 1].renderers = renderers.ToArray();
            group.SetLODs(lodsNew);
        }
        else
        {
            group = lod0.gameObject.AddComponent<LODGroup>();
            LOD[] lods = new LOD[3];
            lods[0] = new LOD(LODLevels_3[0], new Renderer[1] { lod0 });
            lods[1] = new LOD(LODLevels_3[1], new Renderer[1] { lod0 });
            lods[2] = new LOD(LODLevels_3[2], new Renderer[1] { lod2 });
            lods[3] = new LOD(LODLevels_3[3], new Renderer[1] { lod2 });
            group.SetLODs(lods);
        }
    }

    public void AddLOD2(MeshRenderer lod0, MeshRenderer lod2)
    {
        lod2.transform.SetParent(lod0.transform);
        LODGroup group = lod0.GetComponent<LODGroup>();
        if (group != null)
        {
            LOD[] lods = group.GetLODs();
            LOD[] lodsNew = LODGroupInfo.CreateLODs(LODLevels_2);
            for (int i = 0; i < lods.Length && i < lodsNew.Length; i++)
            {
                lodsNew[i].renderers = lods[i].renderers;
            }
            List<Renderer> renderers = new List<Renderer>();
            if(lodsNew[lodsNew.Length - 1].renderers!=null)
                renderers.AddRange(lodsNew[lodsNew.Length - 1].renderers);
            renderers.Add(lod2);
            lodsNew[lodsNew.Length - 1].renderers = renderers.ToArray();
            group.SetLODs(lodsNew);
        }
        else
        {
            group = lod0.gameObject.AddComponent<LODGroup>();
            LOD[] lods = new LOD[3];
            lods[0] = new LOD(LODLevels_2[0], new Renderer[1] { lod0 });
            lods[1] = new LOD(LODLevels_2[1], new Renderer[1] { lod0 });
            lods[2] = new LOD(LODLevels_2[2], new Renderer[1] { lod2 });
            group.SetLODs(lods);
        }
    }

    public float[] LODLevels_1 = new float[] { 0.5f, 0.02f };
    public float[] LODLevels_2 = new float[] { 0.7f, 0.3f, 0.02f };
    public float[] LODLevels_3 = new float[] { 0.7f, 0.3f, 0.1f, 0.02f };

    public void AddLOD1(MeshRenderer lod0,MeshRenderer lod1)
    {
        LODGroup lODGroup = lod0.GetComponent<LODGroup>();
        if (lODGroup == null)
        {
            lODGroup = lod0.gameObject.AddComponent<LODGroup>();
        }
        LOD[] lods = new LOD[2];
        lods[0] = new LOD(LODLevels_1[0], new Renderer[1] { lod0 });     //LOD0 >50% 
                                                                      //lods[1]=new LOD(0.2f,new Renderer[1]{render1});         //LOD1  > 20% - 50% 
                                                                      // lods[2]=new LOD(0.1f,new Renderer[1]{render1});         //LOD2  > 10% - 20% 
        lods[1] = new LOD(LODLevels_1[1], new Renderer[1] { lod1 });        //LOD3  > 1% - 10% 
                                                                      //Culled > 0% - 1%
        lODGroup.SetLODs(lods);
    }

    public void UniformLOD()
    {
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>();
        foreach (LODGroup group in lodGroups)
        {
            var lods = group.GetLODs();
            Debug.Log($" group:{group.name} count:{lods.Length}");
            if (lods.Length == 3)
            {
                lods[0].screenRelativeTransitionHeight = LODLevels_3[0];
                lods[1].screenRelativeTransitionHeight = LODLevels_3[1];
                lods[2].screenRelativeTransitionHeight = LODLevels_3[2];
            }
            else if (lods.Length == 4)
            {
                lods[0].screenRelativeTransitionHeight = LODLevels_3[0];
                lods[1].screenRelativeTransitionHeight = LODLevels_3[1];
                lods[2].screenRelativeTransitionHeight = LODLevels_3[2];
                lods[3].screenRelativeTransitionHeight = LODLevels_3[3];
            }
            group.SetLODs(lods);
        }
    }

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
        var lodGroups=GameObject.FindObjectsOfType<LODGroup>(true);
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

    public void SetLODActive(bool isActive)
    {
        //DateTime start = DateTime.Now;
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>(true);
        foreach (LODGroup group in lodGroups)
        {
            group.gameObject.SetActive(isActive);
        }
        //Debug.LogError($"DisableLOD lodGroups:{lodGroups.Length} time:{(DateTime.Now - start)}");
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

    [ContextMenu("SetRenderersLODInfo")]
    public List<MeshRendererInfo> SetRenderersLODInfo()
    {
        List<MeshRendererInfo> meshRendererInfos = new List<MeshRendererInfo>();
        DateTime start = DateTime.Now;
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>(true);
        var renderers = GameObject.FindObjectsOfType<Renderer>(true).ToList();
        foreach(var renderer in renderers)
        {
            MeshRendererInfo rendererInfo = MeshRendererInfo.GetInfo(renderer.gameObject);
            rendererInfo.LodIds.Clear();
        }

        for (int i1 = 0; i1 < lodGroups.Length; i1++)
        {
            LODGroup group = lodGroups[i1];
            float progress = (float)i1 / lodGroups.Length;

            ProgressBarHelper.DisplayCancelableProgressBar("SetRenderersLODInfo", $"{i1}/{lodGroups.Length} {progress:P1} group:{group}", progress);

            LOD[] lods = group.GetLODs();
            for (int i = 0; i < lods.Length; i++)
            {
                LOD lod = lods[i];
                //LODInfo lodInfo = new LODInfo(lod);
                foreach (var r in lod.renderers)
                {
                    if (r == null) continue;
                    MeshRendererInfo rendererInfo = MeshRendererInfo.GetInfo(r.gameObject);
                    //Debug.Log($"renderer:{r},parent:{r.transform.parent.name},path:{r.transform.GetPathToRoot()},rendererInfo:{rendererInfo}");
                    rendererInfo.LodIds.Add(i);
                    renderers.Remove(r);
                }
                //break;
            }
            //break;
        }
        //foreach (var r in renderers)
        //{
        //    MeshRendererInfo rendererInfo = MeshRendererInfo.GetInfo(r.gameObject);
        //    rendererInfo.LodId = -1;
        //}
        ProgressBarHelper.ClearProgressBar();
        Debug.LogError($"SetRenderersLODInfo lods:{lodGroups.Length} count:{renderers.Count} time:{(DateTime.Now - start)}");
        return meshRendererInfos;
    }

    public List<LODGroupInfo> lODGroupInfos = new List<LODGroupInfo>();

    public List<LODGroupInfo> GetLodGroupInfos()
    {
        lODGroupInfos.Clear();
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>(true);
        for (int i1 = 0; i1 < lodGroups.Length; i1++)
        {
            LODGroup group = lodGroups[i1];
            //LODGroupInfo groupInfo=new LODGroupInfo()
            //LOD[] lods = group.GetLODs();
            //for (int i = 0; i < lods.Length; i++)
            //{
            //    LOD lod = lods[i];
            //    LODInfo lodInfo = new LODInfo(lod);
            //    foreach (var r in lod.renderers)
            //    {
            //        MeshRendererInfo rendererInfo = MeshRendererInfo.GetInfo(r.gameObject);
            //        rendererInfo.LodId = i;
            //    }
            //}
        }
        return lODGroupInfos;
    }

    public List<LODGroupDetails> lodDetails;

    public GameObject LocalTarget;

    [ContextMenu("GetRuntimeLODDetail")]
    public string GetRuntimeLODDetail(bool isForce)
    {
        DateTime now = DateTime.Now;
        if(lodDetails==null||lodDetails.Count==0 || isForce)
        {
            lodDetails=LODGroupDetails.GetSceneLodGroupInfo(LocalTarget);
        }
        
        Camera cam = GameObject.FindObjectOfType<Camera>();

        int[] infos=LODGroupDetails.CaculateGroupInfo(lodDetails,LODSceneView.GameView,LODSortType.Vertex,cam);

        int[] lodCount=new int[5];
        int[] lodVertexCount=new int[5];
        float[] lodPercent=new float[5];
        float allVertex0=0;
        foreach(LODGroupDetails lodI in lodDetails)
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
        string info=$"LOD:{lodDetails.Count}, Vertex:{allVertexCount/10000f:F0}/{allVertex0/10000f:F0}, Mesh:{allMeshCount}, t:{time:F0}ms";
        // info+="\n"+lodInfoTxt;
        info+="\n"+lodInfoTxt_count+"\n"+lodInfoTxt_vertex+"\n"+lodInfoTxt_percent;

        //Debug.Log($"CaculateGroupInfo完成，耗时{time}ms "+info);

        lodDetails.Sort((a, b) =>
        {
            return b.vertexCount.CompareTo(a.vertexCount);
        });

        return info;
    }
}
