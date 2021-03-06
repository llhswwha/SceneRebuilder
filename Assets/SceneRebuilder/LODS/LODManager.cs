using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static MeshHelper;
using CommonExtension;
using CommonUtils;
#if UNITY_EDITOR
using UnityEditor;
#endif
// using LODLevelCaculate;

public class LODManager : SingletonBehaviour<LODManager>
{
    public static List<MeshRenderer> GetLODRenderers(GameObject root)
    {
        LODManager lODManager = LODManager.Instance;
        lODManager.LocalTarget = root;
        lODManager.UpdateLODs();
        //lODManager.SetRenderersLODInfo();
        List<LODGroupDetails> lods = lODManager.lodDetails;
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach (LODGroupDetails lod in lods)
        {
            renderers.AddRange(lod.GetLODRenderers(0));
        }
        return renderers;
    }

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

    public float[] lodVertexPercents = new float[] { 0.75f, 0.5f, 0.25f, 0.1f };

    public bool isDestroy = true;
    public bool isSaveAsset = false;

    // Start is called before the first frame update
    void Start()
    {
        LocalTarget = null;
        lodDetails = null;

        StartUpdateLoadLOD0Coroutine();
    }

    public void CreateBoxLOD(GameObject go)
    {
        CreateBoxLOD(go, LODLevels_1);
    }

    public LODGroup CreateBoxLOD(GameObject go, float[] lvs)
    {
        GameObject box = TransformHelper.CreateBoundsCube(go);
        box.name = go.name + "_LODBox";
        var mrs = box.GetComponentsInChildren<MeshRenderer>(true);

        GameObject goNew = new GameObject(go.name);
        goNew.transform.position = go.transform.position;
        goNew.transform.SetParent(go.transform.parent);
        EditorHelper.UnpackPrefab(go);
        go.transform.SetParent(goNew.transform);

        box.transform.SetParent(goNew.transform);

        LODGroup lODGroup = goNew.AddMissingComponent<LODGroup>();
        MeshRendererInfoEx rendererInfoEx = go.AddMissingComponent<MeshRendererInfoEx>();



        LOD[] lods = GetLODs2(rendererInfoEx.GetRenderers(), mrs);
        lODGroup.SetLODs(lods);

        LODGroupInfo info = goNew.AddComponent<LODGroupInfo>();
        info.GetLODs();
        return lODGroup;
    }

    public void CreateAutoLOD(GameObject go)
    {
        AutomaticLODHelper.CreateAutoLOD(go, LODMaterials, LODLevels, lodVertexPercents, isDestroy, isSaveAsset);
    }

    public void CreateAutoLOD(GameObject go, Action<float> progressChanged)
    {
        AutomaticLODHelper.CreateAutoLOD(go, LODMaterials, LODLevels, lodVertexPercents, isDestroy, isSaveAsset, progressChanged);
    }

    public void CreateAutoLOD(GameObject go, float percent, Action<float> progressChanged)
    {
        AutomaticLODHelper.CreateAutoLOD(go, LODMaterials, new float[] { 0.5f }, new float[] { percent }, isDestroy, isSaveAsset, progressChanged);
    }

    public void RemoveLOD(GameObject go)
    {
        AutomaticLODHelper.ClearLODAndChildren(go);
    }

    public GameObject TestTarget;

    [ContextMenu("TestCreateLOD")]
    public void TestCreateLOD()
    {
        CreateAutoLOD(TestTarget);
    }

    public float zeroDistance = 0.0002f;

    public GameObject GroupRoot;

    public GameObject LODnRoot;

    public MeshRendererInfo CreateSimplifier(MeshRendererInfo lod0, float percent)
    {
        GameObject newObj = MeshHelper.CopyRenderer(lod0.GetMinLODGo());
        MeshRendererInfo renderer = MeshRendererInfo.GetInfo(newObj, true);
        newObj.name += "_" + percent;
        return renderer;
    }


    public List<MeshRendererInfo> list_lod0 = new List<MeshRendererInfo>();

    public LODCompareMode compareMode = LODCompareMode.NameWithCenter;

    public void ClearLod0List()
    {
        List<MeshRendererInfo> list2 = new List<MeshRendererInfo>();
        foreach (var item in list_lod0)
        {
            if (item == null) continue;
            list2.Add(item);
        }
        list_lod0 = list2;
    }

    public MinDisTarget<MeshRendererInfo> GetMinInfo(Transform t)
    {
        var min = GetMinDisTransform<MeshRendererInfo>(list_lod0, t, compareMode);
        return min;
    }

    public void CreateGroup(LODTwoRenderers twoRenderers)
    {
        var render_lod0 = twoRenderers.renderer_old;
        //var render_lod1 = twoRenderers.renderer_lod1;
        int lodLevel = 1;
        LODGroup lODGroup = render_lod0.GetComponent<LODGroup>();
        if (lODGroup != null)
        {
            lodLevel = lODGroup.GetLODs().Length;
        }
        Debug.LogError($"CreateGroup lodLevel:{lodLevel}");

        CreateGroup(twoRenderers, lodLevel);
    }

    private LODGroup CreateGroup(LODTwoRenderers twoRenderers, int lodLevel)
    {
        var render_lod0 = twoRenderers.renderer_old;
        var render_lod1 = twoRenderers.renderer_new;
        if (twoRenderers.vertexCount1 == twoRenderers.vertexCount0)
        {
#if UNITY_EDITOR
            EditorHelper.UnpackPrefab(render_lod1.gameObject);
#endif
            GameObject.DestroyImmediate(render_lod1.gameObject);

            //GameObject.DestroyImmediate(filter1.gameObject);
            render_lod1 = CreateSimplifier(render_lod0, 0.7f);

        }

        //else
        {

            render_lod1.transform.SetParent(render_lod0.transform);
            string nName = LODHelper.GetOriginalName(render_lod1.name);
            render_lod1.name = nName + "_LOD" + lodLevel;

            LODGroup group = null;
            if (lodLevel == 1)
            {
                group = AddLOD1(render_lod0, render_lod1);
            }
            else if (lodLevel == 2)
            {
                //group = render_lod0.GetComponentInParent<LODGroup>();
                group = AddLOD2(render_lod0, render_lod1);
            }
            else if (lodLevel == 3)
            {
                group = AddLOD3(render_lod0, render_lod1);
            }
            LODGroupInfo.Init(group.gameObject);
            var groupNew = LODHelper.UniformLOD0(group);
            LODHelper.SetRenderersLODInfo(groupNew, null);

            LODGroupInfo groupInfo = groupNew.GetComponent<LODGroupInfo>();
            groupInfo.SetMaterial();

            LODHelper.MoveToFloorLODsRoot(groupNew.transform);
            return group;
        }
    }
#if UNITY_EDITOR
    private void AppendLodInner(int lodLevel)
    {
        Debug.Log($"LODManager.AppendLodInner lodLevel:{lodLevel}");
        foreach (var item in twoList)
        {
            var minDis = item.dis;
            var render_lod1 = item.renderer_new;
            if (render_lod1 == null) continue;
            var render_lod0 = item.renderer_old;
            if (render_lod0 == null) continue;
            var vertexCount0 = item.vertexCount0;
            var vertexCount1 = item.vertexCount1;
            if (vertexCount0 == vertexCount1)
            {
                continue;
            }
            if (minDis <= zeroDistance)
            {
                if (DoCreateGroup)
                {
                    Debug.Log($"GetDistance1 \tLOD3:{render_lod1.name}({vertexCount1}) \tLOD0:{render_lod0.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1 / vertexCount0:P2}");
                    LODGroup group = CreateGroup(item, lodLevel);
                }
            }
            else
            {
                Debug.LogWarning($"GetDistance1 \tLOD3:{render_lod1.name}({vertexCount1}) \tLOD0:{render_lod0.name}({vertexCount0}) \tDistance:{minDis} \t{(float)vertexCount1 / vertexCount0:P2}");
            }
        }
    }

    public void DeleteFilter(string searchKey)
    {
        int count = 0;
        var list = twoList.Where(i => i.GetLODCaption().Contains(searchKey)).ToList();
        foreach (var item in list)
        {
            if (item.dis < zeroDistance)
            {
                GameObject.DestroyImmediate(item.renderer_new.gameObject);
                count++;
            }

        }
        Debug.Log($"DeleteFilter count:{count}/{list.Count}");
    }

    public void CheckLOD0()
    {
        List<MeshRendererInfo> lod0s = new List<MeshRendererInfo>();
        foreach (var item in twoList)
        {
            var minDis = item.dis;
            var render_lod1 = item.renderer_new;
            var render_lod0 = item.renderer_old;
            var vertexCount0 = item.vertexCount0;
            var vertexCount1 = item.vertexCount1;
            if (!lod0s.Contains(render_lod0))
            {
                lod0s.Add(render_lod0);
            }
            else
            {
                Debug.LogError($"CheckLOD0 lod0s.Contains(render_lod0) lod0:[{render_lod0}] two:[{item.GetLODCaption()}]");
            }

        }
    }

    public void CompareTwoRoot()
    {
        this.twoList = ModelUpdateManager.Instance.CompareModels(GroupRoot, LODnRoot);
    }

    //public void CompareTwoRoot()
    //{
    //    twoList.Clear();
    //    EditorHelper.UnpackPrefab(GroupRoot, PrefabUnpackMode.OutermostRoot);
    //    EditorHelper.UnpackPrefab(LODnRoot, PrefabUnpackMode.OutermostRoot);
    //    DateTime start = DateTime.Now;
    //    //var renderers_2 = MeshRendererInfo.InitRenderers(LODnRoot);//  LODnRoot.GetComponentsInChildren<MeshRenderer>(true);
    //    var renderers_2 = MeshRendererInfo.GetLodNs(LODnRoot, -1, 0); //  LODnRoot.GetComponentsInChildren<MeshRenderer>(true);
    //    var renderers_0 = MeshRendererInfo.GetLodNs(GroupRoot, -1, 0);

    //    twoList.LODRendererCount0 = renderers_0.Count;
    //    twoList.LODRendererCount1 = renderers_2.Length;

    //    list_lod0 = new List<MeshRendererInfo>();
    //    list_lod0.AddRange(renderers_0);
    //    //renderers_0.ToList().ForEach(i => { ts.Add(i.transform); });
    //    for (int i = 0; i < renderers_2.Length; i++)
    //    {
    //        MeshRendererInfo render_lod1 = renderers_2[i];

    //        float progress = (float)i / renderers_2.Length;
    //        ProgressBarHelper.DisplayCancelableProgressBar("CombineLOD0AndLOD1", $"{i}/{renderers_2.Length} {progress:P1} MeshRenderer:{render_lod1.name}", progress);

    //        var min = GetMinDisTransform<MeshRendererInfo>(list_lod0, render_lod1.transform, compareMode);
    //        float minDis = min.dis;
    //        MeshRendererInfo render_lod0 = min.target;

    //        //MeshFilter filter1 = render_lod1.GetComponent<MeshFilter>();
    //        //MeshFilter filter0 = render_lod0.GetComponent<MeshFilter>();
    //        int vertexCount0 = render_lod0.GetMinLODVertexCount();
    //        int vertexCount1 = render_lod1.GetMinLODVertexCount();
    //        LODTwoRenderers lODTwoRenderers = new LODTwoRenderers(render_lod0, render_lod1, minDis, min.meshDis, vertexCount0, vertexCount1);
    //        twoList.Add(lODTwoRenderers);
    //    }
    //    ProgressBarHelper.ClearProgressBar();

    //    twoList.Sort((a, b) =>
    //    {
    //        return b.dis.CompareTo(a.dis);
    //    });

    //    RemoveEmpty(LODnRoot.transform);
    //    RemoveEmpty(LODnRoot.transform);
    //    RemoveEmpty(LODnRoot.transform);

    //    //SetRenderersLODInfo();

    //    Debug.LogError($"AppendLod3ToGroup count1:{renderers_2.Length} count0:{renderers_0.Count} time:{(DateTime.Now - start)}");
    //}

    //public void TestCompareTwoRoot()
    //{
    //    twoList.Clear();
    //    EditorHelper.UnpackPrefab(GroupRoot, PrefabUnpackMode.OutermostRoot);
    //    EditorHelper.UnpackPrefab(LODnRoot, PrefabUnpackMode.OutermostRoot);
    //    DateTime start = DateTime.Now;
    //    //var renderers_2 = MeshRendererInfo.InitRenderers(LODnRoot);//  LODnRoot.GetComponentsInChildren<MeshRenderer>(true);
    //    var renderers_2 = MeshRendererInfo.GetLodNs(LODnRoot, -1, 0);
    //    var renderers_0 = MeshRendererInfo.GetLodNs(GroupRoot, -1, 0);

    //    twoList.LODRendererCount0 = renderers_0.Count;
    //    twoList.LODRendererCount1 = renderers_2.Length;

    //    list_lod0 = new List<MeshRendererInfo>();
    //    list_lod0.AddRange(renderers_0);
    //    //renderers_0.ToList().ForEach(i => { ts.Add(i.transform); });

    //    List<MeshRendererInfo> ts = list_lod0;
    //    List<MeshRendererInfo> ts0 = ts;
    //    var mode = compareMode;

    //    for (int i = 0; i < renderers_2.Length && i<10; i++)
    //    {
    //        MeshRendererInfo render_lod1 = renderers_2[i];

    //        float progress = (float)i / renderers_2.Length;
    //        ProgressBarHelper.DisplayCancelableProgressBar("CombineLOD0AndLOD1", $"{i}/{renderers_2.Length} {progress:P1} MeshRenderer:{render_lod1.name}", progress);

    //        Transform t = render_lod1.transform;

    //        //=============
    //        var ts2 = ts.FindAll(i => i.name == t.name);
    //        Debug.Log($"t:{t.name},ts:{ts.Count} ts2:{ts2.Count}");

    //        if (ts2.Count > 0)
    //        {
    //            ts = ts2;
    //        }

    //        var min = GetMinDisTransformInner(ts, t, mode);

    //        Debug.Log($"min1:{min}");

    //        if (min.dis > 0.01f && mode != LODCompareMode.Name)
    //        {
    //            min = GetMinDisTransformInner(ts0, t, mode);
    //            Debug.Log($"min2:{min}");
    //        }
    //        //=============

    //        //var min = GetMinDisTransform<MeshRendererInfo>(list_lod0, render_lod1.transform, compareMode);
    //        float minDis = min.dis;
    //        MeshRendererInfo render_lod0 = min.target;

    //        //MeshFilter filter1 = render_lod1.GetComponent<MeshFilter>();
    //        //MeshFilter filter0 = render_lod0.GetComponent<MeshFilter>();
    //        int vertexCount0 = render_lod0.GetMinLODVertexCount();
    //        int vertexCount1 = render_lod1.GetMinLODVertexCount();
    //        LODTwoRenderers lODTwoRenderers = new LODTwoRenderers(render_lod0, render_lod1, minDis, min.meshDis, vertexCount0, vertexCount1);
    //        twoList.Add(lODTwoRenderers);
    //        //break;
    //    }
    //    ProgressBarHelper.ClearProgressBar();

    //    twoList.Sort((a, b) =>
    //    {
    //        return b.dis.CompareTo(a.dis);
    //    });

    //    RemoveEmpty(LODnRoot.transform);
    //    RemoveEmpty(LODnRoot.transform);

    //    //SetRenderersLODInfo();

    //    Debug.LogError($"AppendLod3ToGroup count1:{renderers_2.Length} count0:{renderers_0.Count} time:{(DateTime.Now - start)}");
    //}

    public int GetNewLODn()
    {
        if (LODnRoot == null)
        {
            //Debug.LogError($"GetNewLODn LODnRoot == null");
            return 0;
        }
        string name = LODnRoot.name;
        if (name.Contains("_LOD") == false) return 0;
        string[] parts = name.Split(new string[] { "_LOD" }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2)
        {
            try
            {
                string lodN = parts[1].Replace("LOD", "");
                int lod = int.Parse(lodN);
                return lod;
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetNewLODn LODnRoot:{LODnRoot.name} Exception:{ex}");
                return 0;
            }
        }
        else
        {
            Debug.LogError($"GetNewLODn LODnRoot:{LODnRoot.name} parts.Length != 2 ");
            return 0;
        }
    }

    [ContextMenu("AppendLodNToGroup")]
    public void AppendLodNToGroup()
    {
        int n = GetNewLODn();
        AppendLodInner(n);
    }

    public void AppendLodNToGroup(int n)
    {
        AppendLodInner(n);
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

    public void ShowRoot0()
    {
        //GroupRoot.SetActive(true);
        //LODnRoot.SetActive(false);

        foreach (var item in twoList)
        {
            item.Show0();
        }
    }

    public void ShowRoot1()
    {
        //GroupRoot.SetActive(true);
        //LODnRoot.SetActive(false);

        foreach (var item in twoList)
        {
            item.Show1();
        }
    }
    public void ShowRoot01()
    {
        //GroupRoot.SetActive(true);
        //LODnRoot.SetActive(false);

        foreach (var item in twoList)
        {
            item.Show01();
        }
    }

    public void HideRoot01()
    {
        //GroupRoot.SetActive(true);
        //LODnRoot.SetActive(false);

        foreach (var item in twoList)
        {
            item.Hide01();
        }
    }

    public void SetAppendLod3Color()
    {
        foreach (var item in twoList)
        {
            if (item.dis < zeroDistance)
            {
                item.SetColor();
            }
        }
    }



    public void DeleteSame()
    {
        int count = 0;
        foreach (var item in twoList)
        {
            if (item.dis < zeroDistance && item.isSameName && item.vertexCount0 <= item.vertexCount1)
            {
                GameObject.DestroyImmediate(item.renderer_new.gameObject);
                count++;
            }
        }
        Debug.Log($"DeleteSame count:{count}");
    }

    public void Replace()
    {
        foreach (var item in twoList)
        {
            if (item.dis < zeroDistance)
            {
                item.Replace();
            }
        }
    }

    public void ReplaceFilter(string searchKey)
    {
        int count = 0;
        var list = twoList.Where(i => i.GetLODCaption().Contains(searchKey)).ToList();
        foreach (var item in list)
        {
            if (item.dis < zeroDistance)
            {
                //GameObject.DestroyImmediate(item.renderer_lod1.gameObject);
                //count++;

                item.Replace();
            }
        }
        Debug.Log($"ReplaceFilter count:{count}/{list.Count}");
    }

    public void ReplaceLOD1()
    {
        foreach (var item in twoList)
        {
            if (item.dis < zeroDistance)
            {
                ////var minDis = item.dis;
                //var render_lod1 = item.renderer_lod1;
                //if (render_lod1 == null) continue;
                //var render_lod0 = item.renderer_lod0;
                //if (render_lod0 == null) continue;

                //var lod1 = item.renderer_lod1.meshRenderer;
                //var lod0 = item.renderer_lod0.meshRenderer;
                //lod1.sharedMaterials = lod0.sharedMaterials;

                //render_lod1.transform.SetParent(render_lod0.transform.parent);
                //GameObject.DestroyImmediate(render_lod0.gameObject);
                ////render_lod1.name += "_New";

                item.Replace();
            }
        }
    }

    public void SetName0()
    {
        Transform[] gos = GroupRoot.GetComponentsInChildren<Transform>(true);
        foreach (var go in gos)
        {
            go.name = go.name.Replace(" ", "_");
        }
    }

    public void SetName1()
    {
        Transform[] gos = LODnRoot.GetComponentsInChildren<Transform>(true);
        foreach (var go in gos)
        {
            go.name = go.name.Replace(" ", "_");
        }
    }

#endif





    private MinDisTarget<T> GetMinDisTransformInner<T>(List<T> ts, Transform t, LODCompareMode mode) where T : Component
    {
        float minDis = float.MaxValue;
        float minDisOffCenter = float.MaxValue;
        float minDisOfMesh = float.MaxValue;
        List<T> minTList = new List<T>();
        List<T> minTExList = new List<T>();
        foreach (T item in ts)
        {
            //float distance = Vector3.Distance(item.transform.position, t.position);

            //float distance = GetCenterDistance(item.gameObject, t.gameObject);

            float distance = MeshHelper.GetDistance(item, t, mode);

            //if (distance < 1)
            //{
            //    float distance2 = MeshHelper.GetAvgVertexDistanceEx(item.transform, t);
            //    if (distance2 < minDisOfMesh)
            //    {
            //        minDisOfMesh = distance2;
            //        minTExList = new List<T>() { item };
            //    }
            //    else if (distance2 == minDisOfMesh)
            //    {
            //        minTExList.Add(item);
            //    }
            //}

            //float distance = distance2;
            if (distance < minDis)
            {
                minDis = distance;
                minTList = new List<T>() { item };
                minDisOfMesh = VertexHelper.GetAvgVertexDistanceEx(item.transform, t);
            }
            else if (distance == minDisOfMesh)
            {
                minTList.Add(item);
            }
        }
        T minTEx = null;
        if (minTList.Count > 0)
        {
            T minT = minTList[0];

            if (minTExList.Count > 0)
            {
                minTEx = minTExList[0];
            }

            //float distance3 = MeshHelper.GetAvgVertexDistanceEx(minT.transform, t);
            //if (distance3 < minDisOfMesh)
            //{
            //    minDisOfMesh = distance3;
            //    minTEx = minT;
            //}

            if (minTEx == null)
            {
                minTEx = minT;
            }
        }


        return new MinDisTarget<T>(minDis, minDisOfMesh, minTEx);
    }

    private MinDisTarget<T> GetMinDisTransform<T>(List<T> ts, Transform t, LODCompareMode mode) where T : Component
    {
        //1.Find SameName
        //2.Find Closed
        List<T> ts0 = ts;
        MinDisTarget<T> min = null;
        if (mode == LODCompareMode.Name
            || mode == LODCompareMode.NameWithPos
            || mode == LODCompareMode.NameWithCenter
            || mode == LODCompareMode.NameWithMin
            || mode == LODCompareMode.NameWithMax
            || mode == LODCompareMode.NameWithMesh)
        {

            var ts2 = ts.FindAll(i => i.name == t.name);

            //string tn = t.name;
            //if(tn.Contains(" "))
            //{
            //    tn = tn.Split(' ')[0];
            //}
            //var ts2 = ts.FindAll(i => i.name.Contains(tn));

            if (ts2.Count > 0)
            {
                ts = ts2;
            }

            min = GetMinDisTransformInner(ts, t, mode);

            if (min.dis > 0.01f && mode != LODCompareMode.Name)
            {
                min = GetMinDisTransformInner(ts0, t, mode);
            }
        }
        else
        {
            min = GetMinDisTransformInner(ts0, t, mode);
        }

        if (min.dis <= 0.01f)
        {
            ts0.Remove(min.target);
        }
        return min;
    }

    //private static void RemoveEmpty(Transform root)
    //{
    //    Transform[] cs= root.GetComponentsInChildren<Transform>(true);
    //    foreach(var c in cs)
    //    {
    //        if (c.childCount > 0) continue;
    //        MeshRenderer meshRenderer = c.GetComponent<MeshRenderer>();
    //        if (meshRenderer == null)
    //        {
    //            GameObject.DestroyImmediate(c.gameObject);
    //        }
    //    }
    //}

    public LODTwoRenderersList twoList = new LODTwoRenderersList();

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

    public LODGroup AddLOD3(MeshRendererInfo lod0, MeshRendererInfo lod2)
    {
        lod2.transform.SetParent(lod0.transform);
        LODGroup group = lod0.GetComponent<LODGroup>();
        if (group != null)
        {
            //LOD[] lods = group.GetLODs();
            //LOD[] lodsNew = LODHelper.CreateLODs(LODLevels_3);
            //for (int i = 0; i < lods.Length && i < lodsNew.Length; i++)
            //{
            //    lodsNew[i].renderers = lods[i].renderers;
            //}
            //List<Renderer> renderers = new List<Renderer>();
            //if (lodsNew[lodsNew.Length - 1].renderers != null)
            //    renderers.AddRange(lodsNew[lodsNew.Length - 1].renderers);
            //renderers.AddRange(lod2.GetRenderers());
            //lodsNew[lodsNew.Length - 1].renderers = renderers.ToArray();
            //group.SetLODs(lodsNew);

            AddLOD3(group, lod2.GetRenderers(), LODLevels_3);
        }
        else
        {
            group = lod0.gameObject.AddComponent<LODGroup>();
            //LOD[] lods = new LOD[3];
            //lods[0] = new LOD(LODLevels_3[0], lod0.GetRenderers());
            //lods[1] = new LOD(LODLevels_3[1], lod0.GetRenderers());
            //lods[2] = new LOD(LODLevels_3[2], lod2.GetRenderers());
            //lods[3] = new LOD(LODLevels_3[3], lod2.GetRenderers());

            LOD[] lods = GetLODs4(lod0.GetRenderers(), lod0.GetRenderers(), lod2.GetRenderers(), lod2.GetRenderers());
            group.SetLODs(lods);
        }
        return group;
    }

    public LODGroup AddLOD2(MeshRendererInfo lod0, MeshRendererInfo lod2)
    {
        LODGroup group = null;
        if (lod0.IsRendererType(MeshRendererType.LOD))
        {
            group = lod0.GetComponentInParent<LODGroup>();
        }
        if (group == null)
        {
            group = lod0.GetComponent<LODGroup>();
        }
        if (group != null)
        {
            lod2.transform.SetParent(group.transform);
            AddLOD2(group, lod2.GetRenderers(), LODLevels_2);
        }
        else
        {
            group = lod0.gameObject.AddComponent<LODGroup>();
            lod2.transform.SetParent(group.transform);
            LOD[] lods = GetLODs3(lod0.GetRenderers(), lod0.GetRenderers(), lod2.GetRenderers());
            group.SetLODs(lods);
        }
        return group;
    }

    public void AddLOD2<T>(LODGroup group, IEnumerable<T> lod2Renderers, float[] lvs) where T : Renderer
    {
        LOD[] lods = group.GetLODs();
        LOD[] lodsNew = LODHelper.CreateLODs(lvs);
        for (int i = 0; i < lods.Length && i < lodsNew.Length; i++)
        {
            lodsNew[i].renderers = lods[i].renderers;
        }
        List<Renderer> renderers = new List<Renderer>();
        if (lodsNew[lodsNew.Length - 1].renderers != null)
            renderers.AddRange(lodsNew[lodsNew.Length - 1].renderers);
        renderers.AddRange(lod2Renderers);
        lodsNew[lodsNew.Length - 1].renderers = renderers.ToArray();
        group.SetLODs(lodsNew);
    }
    public void AddLOD3<T>(LODGroup group, IEnumerable<T> lod3Renderers, float[] lvs) where T : Renderer
    {
        LOD[] lods = group.GetLODs();
        LOD[] lodsNew = LODHelper.CreateLODs(lvs);
        for (int i = 0; i < lods.Length && i < lodsNew.Length; i++)
        {
            lodsNew[i].renderers = lods[i].renderers;
        }
        List<Renderer> renderers = new List<Renderer>();
        if (lodsNew[lodsNew.Length - 1].renderers != null)
            renderers.AddRange(lodsNew[lodsNew.Length - 1].renderers);
        renderers.AddRange(lod3Renderers);
        lodsNew[lodsNew.Length - 1].renderers = renderers.ToArray();
        group.SetLODs(lodsNew);
    }

    public float[] LODLevels_1 = new float[] { 0.5f, 0.02f };
    public float[] LODLevels_2 = new float[] { 0.7f, 0.3f, 0.02f };
    public float[] LODLevels_3 = new float[] { 0.7f, 0.3f, 0.1f, 0.02f };

    public Vector2 GetLevels1()
    {
        return new Vector2(LODLevels_1[0], LODLevels_1[1]);
    }

    public Vector3 GetLevels2()
    {
        return new Vector3(LODLevels_2[0], LODLevels_2[1], LODLevels_2[2]);
    }
    public Vector4 GetLevels3()
    {
        return new Vector4(LODLevels_3[0], LODLevels_3[1], LODLevels_3[2], LODLevels_3[3]);
    }

    public void SetLevels1(Vector2 v)
    {
        LODLevels_1 = new float[] { v.x, v.y };
    }

    public void SetLevels2(Vector3 v)
    {
        LODLevels_2 = new float[] { v.x, v.y, v.z };
    }

    public void SetLevels3(Vector4 v)
    {
        LODLevels_3 = new float[] { v.x, v.y, v.z, v.w };
    }

    public float[] DoorLODLevels_2 = new float[] { 0.6f, 0.1f, 0.01f };

    public LOD[] CreateLODS(int count)
    {
        if (count == 2)
        {
            return LODHelper.CreateLODs(LODLevels_1);
        }
        else if (count == 3)
        {
            return LODHelper.CreateLODs(LODLevels_2);
        }
        else if (count == 4)
        {
            return LODHelper.CreateLODs(LODLevels_3);
        }
        else
        {
            //return LODHelper.CreateLODs(LODLevels_3);
            return null;
        }
    }

    public LOD[] GetLODsN(params MeshRenderer[][] meshRenderersArray)
    {
        int count = meshRenderersArray.Length;
        if (count == 2)
        {
            return GetLODs2(meshRenderersArray[0], meshRenderersArray[1]);
        }
        else if (count == 3)
        {
            return GetLODs3(meshRenderersArray[0], meshRenderersArray[1], meshRenderersArray[2]);
        }
        else if (count == 4)
        {
            return GetLODs4(meshRenderersArray[0], meshRenderersArray[1], meshRenderersArray[2], meshRenderersArray[3]);
        }
        else
        {
            //return LODHelper.CreateLODs(LODLevels_3);
            Debug.LogError($"GetLODsN count error:{count}");
            return null;
        }
    }

    public LODGroup AddLOD1(GameObject lod0, GameObject lod1, bool isUniform = true)
    {
        MeshRendererInfo lod0R = MeshRendererInfo.GetInfo(lod0);
        MeshRendererInfo lod1R = MeshRendererInfo.GetInfo(lod1);
        return AddLOD1(lod0R, lod1R, isUniform);
    }

    public LODGroup AddLOD1(MeshRendererInfo lod0, MeshRendererInfo lod1, bool isUniform = true)
    {
        LODGroup lODGroup = lod0.GetComponent<LODGroup>();
        if (lODGroup == null)
        {
            lODGroup = lod0.gameObject.AddComponent<LODGroup>();
        }
        LOD[] lods = GetLODs2(lod0.GetRenderers(), lod1.GetRenderers());
        lODGroup.SetLODs(lods);
        if (isUniform)
        {
            var groupNew = LODHelper.UniformLOD0(lODGroup);
            LODGroupInfo.Init(groupNew.gameObject);
        }
        else
        {
            LODGroupInfo.Init(lODGroup.gameObject);
        }
        return lODGroup;
    }

    public LOD[] GetLODs2(MeshRenderer[] renderers0, MeshRenderer[] renderers1)
    {
        //LOD[] lods = new LOD[2];
        //lods[0] = new LOD(LODLevels_1[0], renderers0);     //LOD0 >50% 
        //                                                            //lods[1]=new LOD(0.2f,new Renderer[1]{render1});         //LOD1  > 20% - 50% 
        //                                                            // lods[2]=new LOD(0.1f,new Renderer[1]{render1});         //LOD2  > 10% - 20% 
        //lods[1] = new LOD(LODLevels_1[1], renderers1);        //LOD3  > 1% - 10% 
        //                                                               //Culled > 0% - 1%
        //return lods;

        return LODHelper.GetLODs2(LODLevels_1, renderers0, renderers1);
    }

    public LOD[] GetLODs3(MeshRenderer[] renderers0, MeshRenderer[] renderers1, MeshRenderer[] renderers2)
    {
        return LODHelper.GetLODs3(LODLevels_2, renderers0, renderers1, renderers2);
    }

    public LOD[] GetDoorLODs3(MeshRenderer[] renderers0, MeshRenderer[] renderers1, MeshRenderer[] renderers2)
    {
        return LODHelper.GetLODs3(DoorLODLevels_2, renderers0, renderers1, renderers2);
    }

    public LOD[] GetLODs4(MeshRenderer[] renderers0, MeshRenderer[] renderers1, MeshRenderer[] renderers2, MeshRenderer[] renderers3)
    {
        //LOD[] lods = new LOD[4];
        //lods[0] = new LOD(LODLevels_3[0], renderers0);
        //lods[1] = new LOD(LODLevels_3[1], renderers1);
        //lods[2] = new LOD(LODLevels_3[2], renderers2);
        //lods[3] = new LOD(LODLevels_3[3], renderers3);
        //return lods;

        return LODHelper.GetLODs4(LODLevels_3, renderers0, renderers1, renderers2, renderers3);
    }

    public void UniformLOD0()
    {
        DateTime start = DateTime.Now;
        var lodGroups = LODHelper.GetLODGroups(LocalTarget, includeInactive);
        int count = 0;
        foreach (LODGroup group in lodGroups)
        {
            var gN = LODHelper.UniformLOD0(group);
            if (gN != group)
            {
                count++;
            }
        }
        Debug.LogError($"UniformLOD0 lodGroups:{lodGroups.Length} time:{(DateTime.Now - start)} count:{count}");
    }

    public void ClearScenes()
    {
        DateTime start = DateTime.Now;
        var lodGroups = InitGroupInfos(false);
        foreach (var group in lodGroups)
        {
            group.ClearOtherScenes();
        }
        Debug.LogError($"ClearScenes lodGroups:{lodGroups.Length} time:{(DateTime.Now - start)}");
    }


    [ContextMenu("ChangeRelativeHeight")]
    public void ChangeLODsRelativeHeight()
    {
        var lodGroups = LODHelper.GetLODGroups(LocalTarget, false);
        for (int i = 0; i < lodGroups.Length; i++)
        {
            LODGroup group = lodGroups[i];
            var lods = group.GetLODs();

            if (lods.Length == 2)
            {
                lods[0].screenRelativeTransitionHeight = LODLevels_1[0];
                lods[1].screenRelativeTransitionHeight = LODLevels_1[1];
                Debug.Log($"[{i}] group:{group.name} count:{lods.Length} level1:{GetLevels1()}");
            }
            else if (lods.Length == 3)
            {
                lods[0].screenRelativeTransitionHeight = LODLevels_2[0];
                lods[1].screenRelativeTransitionHeight = LODLevels_2[1];
                lods[2].screenRelativeTransitionHeight = LODLevels_2[2];
                Debug.Log($"[{i}] group:{group.name} count:{lods.Length} level2:{GetLevels2()}");
            }
            else if (lods.Length == 4)
            {
                lods[0].screenRelativeTransitionHeight = LODLevels_3[0];
                lods[1].screenRelativeTransitionHeight = LODLevels_3[1];
                lods[2].screenRelativeTransitionHeight = LODLevels_3[2];
                lods[3].screenRelativeTransitionHeight = LODLevels_3[3];
                Debug.Log($"[{i}] group:{group.name} count:{lods.Length} level3:{GetLevels3()}");
            }
            group.SetLODs(lods);
        }
    }

    public Dictionary<Renderer, Renderer> GetLODRendererDict()
    {
        Dictionary<Renderer, Renderer> dict = new Dictionary<Renderer, Renderer>();
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>();
        foreach (LODGroup group in lodGroups)
        {
            var lods = group.GetLODs();
            for (int i = 0; i < lods.Length; i++)
            {
                var lod = lods[i];
                foreach (var renderer in lod.renderers)
                {
                    if (renderer == null) continue;
                    // renderers.Add(renderer);
                    if (!dict.ContainsKey(renderer))
                    {
                        dict.Add(renderer, renderer);
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
        //         Debug.LogError("GetLODRendererDict ?????? :"+render);
        //     }
        // }

        return dict;
    }

    [ContextMenu("SetLODMatColor")]
    public void SetLODMatColor()
    {
        DateTime start = DateTime.Now;
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>();
        foreach (LODGroup group in lodGroups)
        {
            var lods = group.GetLODs();
            for (int i = 0; i < lods.Length; i++)
            //foreach(var lod in lods)
            {
                var lod = lods[i];
                foreach (var renderer in lod.renderers)
                {
                    if (renderer == null) continue;
                    // renderer.material = LODMaterials[i];

                    // var mats=renderer.materials;
                    // for(int j=0;j<mats.Length;j++)
                    // {
                    //     mats[j]=LODMaterials[i];
                    // }
                    // renderer.materials=mats;

                    var mats = renderer.materials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        mats[j] = LODMaterials[i];
                    }
                    renderer.materials = mats;
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
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>(true);
        foreach (LODGroup group in lodGroups)
        {
            var lods = group.GetLODs();
            for (int i = 1; i < lods.Length; i++)
            //foreach(var lod in lods)
            {
                var lod = lods[i];
                foreach (var renderer in lod.renderers)
                {
                    if (renderer == null) continue;
                    renderer.enabled = isEnabled;
                }
            }
            group.enabled = isEnabled;
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

    public void DeleteLODs()
    {
        var lodGroups = GameObject.FindObjectsOfType<LODGroup>(true);
        foreach (LODGroup group in lodGroups)
        {
            GameObject.DestroyImmediate(group.gameObject);
        }
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
        List<Renderer> renderers = GameObject.FindObjectsOfType<Renderer>(true).ToList();

        for (int i = 0; i < renderers.Count; i++)
        {
            Renderer renderer = renderers[i];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("SetRenderersLODInfo1", i, renderers.Count, renderer.name));
            MeshRendererInfo rendererInfo = MeshRendererInfo.GetInfo(renderer.gameObject, true);
            rendererInfo.Init();
            rendererInfo.LodIds.Clear();
        }

        if (GroupRoot)
        {
            var rendererInfos1 = GroupRoot.GetComponentsInChildren<MeshRendererInfo>(true);
            for (int i = 0; i < rendererInfos1.Length; i++)
            {
                MeshRendererInfo renderer = rendererInfos1[i];
                ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("SetRenderersLODInfo2", i, rendererInfos1.Length, renderer.name));
                renderer.Init();
            }
        }

        for (int i1 = 0; i1 < lodGroups.Length; i1++)
        {
            LODGroup group = lodGroups[i1];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("SetRenderersLODInfo3", i1, lodGroups.Length, group.name));
            LODHelper.SetRenderersLODInfo(group, renderers);
        }
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

    public List<LODGroupDetails> lodDetails = new List<LODGroupDetails>();

    public GameObject LocalTarget;

    public bool includeInactive = false;

    public LODGroupInfo[] InitGroupInfos(bool isInactive)
    {
        LODGroup[] groups = LODHelper.GetLODGroups(LocalTarget, isInactive);
        LODGroupInfo[] infos = new LODGroupInfo[groups.Length];
        for (int i = 0; i < groups.Length; i++)
        {
            LODGroup group = groups[i];

            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("InitGroupInfos", i, groups.Length, group.name));
            var info = LODGroupInfo.Init(group.gameObject);
            infos[i] = info;
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"InitGroupInfos LocalTarget:{LocalTarget} includeInactive:{isInactive} groups:{groups.Length} infos:{infos.Length}");
        return infos;
    }

    public LODGroupInfo[] SetMats()
    {
        LODGroupInfo[] infos = InitGroupInfos(false);
        for (int i = 0; i < infos.Length; i++)
        {
            LODGroupInfo info = infos[i];
            ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("SetMats", i, infos.Length, info.name));
            info.SetMaterial();
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"SetMaterial LocalTarget:{LocalTarget} includeInactive:{includeInactive} infos:{infos.Length}");
        return infos;
    }

    public LODGroupInfo[] CheckLOD0Scenes()
    {
        LODGroupInfo[] infos = InitGroupInfos(false);
        for (int i = 0; i < infos.Length; i++)
        {
            LODGroupInfo info = infos[i];
            info.CheckLOD0Scenes();
        }
        Debug.Log($"CheckLOD0Scenes LocalTarget:{LocalTarget} includeInactive:{includeInactive} infos:{infos.Length}");
        return infos;
    }

    public int MinSaveLODVertexCount = 50000;//2w

#if UNITY_EDITOR
    public void SaveLOD0s()
    {
        //LODGroup[] groups = LODHelper.GetLODGroups(LocalTarget, includeInactive);
        //var scenes = LODHelper.SaveLOD0s(null, groups);
        //EditorHelper.ClearOtherScenes();
        //EditorHelper.RefreshAssets();

        //foreach (var group in groups)
        //{
        //    var info = LODGroupInfo.Init(group.gameObject);
        //    info.GetScene();
        //}

        LODGroupInfo[] infos = InitGroupInfos(false);
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];

            float progress = (float)i / infos.Length;
            if (ProgressBarHelper.DisplayCancelableProgressBar("LODManager.SaveLODs", $"Progress {i}/{infos.Length} {progress:P1}", progress))
            {
                break;
            }

            if (info.LodInfos[0].vertextCount > MinSaveLODVertexCount)
            {
                if (info.IsSceneCreatable() == false) continue;
                info.EditorCreateScene();
            }
        }
        EditorHelper.ClearOtherScenes();
        EditorHelper.RefreshAssets();
        ProgressBarHelper.ClearProgressBar();

        SubSceneManager.Instance.SetBuildings_All();
    }

    public void LoadLOD0s()
    {
        //LODGroup[] groups = LODHelper.GetLODGroups(LocalTarget, includeInactive);
        //var scenes = LODHelper.SaveLOD0s(null, groups);
        //EditorHelper.ClearOtherScenes();
        //EditorHelper.RefreshAssets();

        //foreach (var group in groups)
        //{
        //    var info = LODGroupInfo.Init(group.gameObject);
        //    info.GetScene();
        //}

        LODGroupInfo[] infos = InitGroupInfos(false);
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];

            float progress = (float)i / infos.Length;
            ProgressBarHelper.DisplayProgressBar("LODManager.SaveLODs", $"Progress {i}/{infos.Length} {progress:P1}", progress);

            if (info.IsSceneCreatable() == true)
            {
                Debug.LogWarning($"info.IsSceneCreatable() == false info:{info.name}");
                continue;
            }
            info.EditorLoadScene();
        }
        //EditorHelper.ClearOtherScenes();
        //EditorHelper.RefreshAssets();
        ProgressBarHelper.ClearProgressBar();

        SubSceneManager.Instance.SetBuildings_All();
    }
#endif

    public double lodInfoTime = 0;

    public bool IsThreadBusy = false;

    public bool IsUseThread = false;

    public string lodInfoText = "";

    public Camera lodCamera = null;

    private void GetLodCamera()
    {
        if (lodCamera == null)
        {
            lodCamera = Camera.main;
        }
        if (lodCamera == null)
        {
            var cs = GameObject.FindObjectsOfType<Camera>();
            foreach (var c in cs)
            {
                if (c.name.Contains("RTE"))
                {
                    continue;
                }
                lodCamera = c;
            }
        }
        if (lodCamera != null && lodCamera.gameObject.activeInHierarchy == false)
        {
            var cs = GameObject.FindObjectsOfType<Camera>();
            foreach (var c in cs)
            {
                if (c.name.Contains("RTE"))
                {
                    continue;
                }
                lodCamera = c;
            }
        }
    }

    [ContextMenu("SortLODDetails")]
    public void SortLODDetails()
    {
        lodDetails.Sort();
    }

    public void AddLODGroup(LODGroup group)
    {
        if (lodDetails == null)
        {
            lodDetails = new List<LODGroupDetails>();
        }
        LODGroupDetails detail = new LODGroupDetails(group);
        lodDetails.Add(detail);
    }

    public string GetRuntimeLODDetailSubThread(bool isForce, bool isInactive)
    {
        DateTime now = DateTime.Now;
        if (lodDetails == null || lodDetails.Count == 0 || isForce)
        {
            lodDetails = LODGroupDetails.GetSceneLodGroupInfo(LocalTarget, isInactive);
        }

        GetLodCamera();
        if (lodCamera == null) return "lodCamera==null";

        CameraData camData = new CameraData(lodCamera, LODSceneView.GameView);

        if (IsThreadBusy == false)
        {
            IsThreadBusy = true;

            foreach (var lod in lodDetails)
            {
                lod.UpdatePoint();
            }

            int[] infos = null;
            ThreadManager.Run(() =>
            {
                infos = LODGroupDetails.CaculateGroupInfo(lodDetails, LODSceneView.GameView, LODSortType.Vertex, camData);
                //Debug.Log("thread1");
            }, () =>
            {
                //Debug.Log("thread2");
                lodInfoText = GetLODGroupInfoText(infos);
                IsThreadBusy = false;

                if (IsShowLog)
                {
                    Debug.Log($"CaculateGroupInfo???????????????{lodInfoTime}ms " + lodInfoText);
                }
            }, "GetRuntimeLODDetail");
        }
        lodInfoTime = (DateTime.Now - now).TotalMilliseconds;
        string result = $"t:{lodInfoTime:F2}ms c:{lodCamera.name} {lodInfoText}";
        return result;
    }

    public string GetRuntimeLODDetailMainThread(bool isForce, bool isInactive)
    {
        DateTime now = DateTime.Now;
        if (lodDetails == null || lodDetails.Count == 0 || isForce)
        {
            lodDetails = LODGroupDetails.GetSceneLodGroupInfo(LocalTarget, isInactive);
        }
        GetLodCamera();
        if (lodCamera == null) return "lodCamera==null";
        CameraData camData = new CameraData(lodCamera, LODSceneView.GameView);
        foreach (var lod in lodDetails)
        {
            lod.UpdatePoint();
        }
        int[] infos = LODGroupDetails.CaculateGroupInfo(lodDetails, LODSceneView.GameView, LODSortType.Vertex, camData);
        lodInfoText = GetLODGroupInfoText(infos);
        lodInfoTime = (DateTime.Now - now).TotalMilliseconds;
        string result = $"t:{lodInfoTime:F2}ms c:{lodCamera.name} {lodInfoText}";
        if (IsShowLog)
        {
            Debug.Log($"GetRuntimeLODDetailMainThread result:{result}");
        }
        return result;
    }

    public bool IsEnableUpdate = true;

    public bool IsEnableLoadLod0 = true;

    public bool IsShowLog = false;

    public string GetLODGroupInfoText(int[] infos)
    {
        //DateTime now = DateTime.Now;
        List<LODGroupDetails> lod0List = new List<LODGroupDetails>();

        int[] lodCount = new int[5];
        int[] lodVertexCount = new int[5];
        float[] lodPercent = new float[5];
        float allVertex0 = 0;
        foreach (LODGroupDetails lodI in lodDetails)
        {
            if (lodI.currentChild == null) continue;
            lodCount[lodI.currentInfo.currentLevel]++;
            lodVertexCount[lodI.currentInfo.currentLevel] += lodI.currentChild.vertexCount;
            lodPercent[lodI.currentInfo.currentLevel] += lodI.currentChild.vertexPercent;

            allVertex0 += lodI.childs[0].vertexCount;
            // foreach(var child in lodI.childs){
            //     allVertex0+=child.vertexCount;
            // }

            if (lodI.currentInfo.currentLevel == 0)
            {
                if (!lod0List.Contains(lodI))
                    lod0List.Add(lodI);
            }
        }

        if (IsEnableLoadLod0)
        {
            LoadedScenes = LoadLOD0Scenes(lod0List);

            if (IsShowLog)
            {
                Debug.Log($"LoadLOD0Scenes lod0List:{lod0List.Count} LoadedScenes:{LoadedScenes.Count}");
            }
        }

        // string lodInfoTxt="";
        // for(int i=0;i<lodCount.Length;i++){
        //     lodInfoTxt+=$"LOD{i}({lodCount[i]},{lodVertexCount[i]/10000f:F1},{lodPercent[i]:P1}) ";
        // }

        string lodInfoTxt_count = "";
        string lodInfoTxt_vertex = "";
        string lodInfoTxt_percent = "";
        for (int i = 0; i < lodCount.Length; i++)
        {
            lodInfoTxt_count += $"L{i}({lodCount[i]})\t\t";
            lodInfoTxt_vertex += $"L{i}({lodVertexCount[i] / 10000f:F0})\t\t";
            lodInfoTxt_percent += $"L{i}({lodPercent[i]:P1})\t";
        }

        var allVertexCount = infos[0];
        var allMeshCount = infos[1];
        //lodInfoTime = (DateTime.Now - now).TotalMilliseconds;
        string infoText = $"LOD:{lodDetails.Count}, Vertex:{allVertexCount / 10000f:F0}/{allVertex0 / 10000f:F0}, Mesh:{allMeshCount}";
        // info+="\n"+lodInfoTxt;
        infoText += "\n" + lodInfoTxt_count + "\n" + lodInfoTxt_vertex + "\n" + lodInfoTxt_percent;

        lodDetails.Sort((a, b) =>
        {
            return b.vertexCount.CompareTo(a.vertexCount);
        });
        return infoText;
    }

    public void UpdateLODs()
    {
        includeInactive = true;
        string detail = GetRuntimeLODDetail(true, true);
        Debug.Log($"lod detail:{detail}");
        SortLODDetails();
    }

    [ContextMenu("GetRuntimeLODDetail")]
    public string GetRuntimeLODDetail(bool isForce)
    {
        return GetRuntimeLODDetail(isForce, includeInactive);
    }

    //public bool IsEnable

    public float UpdateInternal = 0.3f;

    private bool isUpdateCoroutineStarted = false;

    public void StartUpdateLoadLOD0Coroutine()
    {
        if (isUpdateCoroutineStarted) return;
        isUpdateCoroutineStarted = true;

        if (IsEnableUpdate == false) return;
        StartCoroutine(UpdateLoadLOD0Coroutine());
    }

    IEnumerator UpdateLoadLOD0Coroutine()
    {
        while (true)
        {
            if (IsEnableUpdate)
            {
                GetLODInfo();
            }
            yield return new WaitForSeconds(UpdateInternal);
        }
        //yield return null;
    }

    [ContextMenu("GetLODInfo")]
    public string GetLODInfo()
    {
        try
        {
            return GetRuntimeLODDetail(false);
        }
        catch (Exception e)
        {
            Debug.LogError("LODManager.GetLODInfo:" + e.ToString() + " " + e.StackTrace);
            return e.Message;
        }
        //Debug.Log("LODManagerUI.GetLODInfo");
    }


    [ContextMenu("GetRuntimeLODDetail")]
    public string GetRuntimeLODDetail(bool isForce, bool isInactive)
    {
        string detail = "";
        if (IsUseThread)
        {
            detail = GetRuntimeLODDetailSubThread(isForce, isInactive);
        }
        else
        {
            detail = GetRuntimeLODDetailMainThread(isForce, isInactive);
        }
        //Debug.Log(detail);
        return detail;
    }

    public List<SubScene_Base> LoadedScenes = new List<SubScene_Base>();

    private static List<SubScene_Base> LoadLOD0Scenes(List<LODGroupDetails> lod0List)
    {
        DictList<SubScene_Base> sceneList = new DictList<SubScene_Base>();
        Dictionary<SubScene_Base, LODGroupInfo> scene2Group = new Dictionary<SubScene_Base, LODGroupInfo>();
        foreach (var lod0 in lod0List)
        {
            try
            {
                if (lod0.group == null) continue;
                if (lod0.group.group == null) continue;
                LODGroupInfo ginfo = lod0.group.GetComponent<LODGroupInfo>();
                if (ginfo == null) continue;
                //ginfo.LoadScene();
                var scene = ginfo.scene;
                if (scene == null) continue;

                if (scene.IsLoading == true) continue;
                if (scene.IsLoaded == true) continue;

                sceneList.Add(scene);
                if (scene2Group.ContainsKey(scene))
                {
                    Debug.LogWarning("LoadLOD0Scenes scene2Group.ContainsKey(scene) scene:" + scene);
                }
                else
                {
                    scene2Group.Add(scene, ginfo);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadLOD0Scenes Exception:" + ex);
            }
        }

        //LoadedScenes = sceneList;

        if (sceneList.Count > 0)
        {
            //Debug.LogError($"GetRuntimeLODDetail lod0List:{lod0List.Count} sceneList:{sceneList.Count} scene2Group:{scene2Group.Count}");
            SubSceneManager.Instance.LoadScenesEx(sceneList.ToArray(), (p) =>
            {
                if (p.scene != null)
                {
                    //Debug.LogError($"GetRuntimeLODDetail scene:{p.scene.name} sceneName:scene:{p.scene.sceneName}");
                    if (scene2Group.ContainsKey(p.scene))
                    {
                        LODGroupInfo group = scene2Group[p.scene];
                        group.SetLOD0FromScene();
                    }
                    else
                    {
                        if (showLog)
                        {
                            Debug.Log($"GetRuntimeLODDetail scene2Group.ContainsKey(p.scene) == false scene:{p.scene.name} sceneParent:{p.scene.gameObject.transform.parent.name}");
                        }
                    }

                }
                else
                {
                    if (showLog)
                    {
                        Debug.Log($"GetRuntimeLODDetail p.scene == null");
                    }
                }

            }, "LoadLOD0Scenes");
        }
        else
        {
            //Debug.Log($"GetRuntimeLODDetail lod0List:{lod0List.Count}");
        }

        return sceneList.Items;
    }

    public void ClearTwoList()
    {
        //twoList = twoList.FindAll(i => i.renderer_lod0 != null || i.renderer_lod1 != null);
        twoList.ClearTwoList();
    }

    public Material LODBoxMat;
}



public static class LODHelper
{
#if UNITY_EDITOR
    [MenuItem("SceneTools/LOD/ClearLODGroupsByKey")]

    public static int ClearLODGroupsByKey(GameObject[] gos, string key)
    {
        //GameObject[] gos = Selection.gameObjects;
        int count1 = 0;
        int count2 = 0;
        for (int i = 0; i < gos.Length; i++)
        {
            GameObject go = gos[i];
            LODGroup[] groups = go.GetComponentsInChildren<LODGroup>(true);
            foreach (var group in groups)
            {
                count1++;
                if (group == null) continue;
                LODGroupInfo lODGroupInfo = group.GetComponent<LODGroupInfo>();
                if (lODGroupInfo != null)
                {
                    GameObject.DestroyImmediate(lODGroupInfo);
                }
                GameObject go0 = group.gameObject;
                MeshRenderer groupRenderer = group.GetComponent<MeshRenderer>();
                SubScene_Base scene = group.GetComponent<SubScene_Base>();
                if (scene != null)
                {
                    continue;
                }
                if (!group.name.Contains(key))
                {
                    continue;
                }
                if (groupRenderer != null)
                {
                    var lods = group.GetLODs();
                    var lod0Renderers = lods[0].renderers;
                    var lod1Renderers = lods[1].renderers;

                    for (int i1 = 1; i1 < lods.Length; i1++)
                    {
                        LOD lod = lods[i1];
                        var renderers = lod.renderers;
                        foreach (var renderer in renderers)
                        {
                            if (renderer == null) continue;
                            if (renderer.gameObject == null) continue;
                            GameObject.DestroyImmediate(renderer.gameObject);
                        }
                    }
                    List<Transform> children = new List<Transform>();
                    for (int j = 0; j < group.transform.childCount; j++)
                    {
                        var child = group.transform.GetChild(j);
                        children.Add(child);
                    }
                    foreach (var child in children)
                    {
                        GameObject.DestroyImmediate(child.gameObject);
                    }
                    GameObject.DestroyImmediate(group);
                }
                else
                {
                    var lods = group.GetLODs();
                    var lod0Renderers = lods[0].renderers.ToList();
                    var lod1Renderers = lods[1].renderers.ToList();

                    for (int i1 = 1; i1 < lods.Length; i1++)
                    {
                        LOD lod = lods[i1];
                        var renderers = lod.renderers;
                        foreach (var renderer in renderers)
                        {
                            if (renderer == null) continue;
                            if (renderer.gameObject == null) continue;
                            if (lod0Renderers.Contains(renderer)) continue;
                            GameObject.DestroyImmediate(renderer.gameObject);
                        }
                    }

                    GameObject.DestroyImmediate(group);
                    if (lod0Renderers.Count == 1)
                    {
                        GameObject go1 = lod0Renderers[0].gameObject;
                        //EditorHelper.RemoveComponents(go1);
                        TransformHelper.ClearComponents<MonoBehaviour>(go1);
                        EditorHelper.CopyAllComponents(go0, go1, true);
                    }
                    else
                    {
                        Debug.LogError($"lod0Renderers.Length !=1 :{lod0Renderers.Count}");
                    }

                    //List<Transform> children = new List<Transform>();
                    //for (int j = 0; j < group.transform.childCount; j++)
                    //{
                    //    var child = group.transform.GetChild(j);
                    //    children.Add(child);
                    //}
                    //foreach (var child in children)
                    //{
                    //    GameObject.DestroyImmediate(child.gameObject);
                    //}
                }
                count2++;
                Debug.Log($"ClearLODGroupsEx[{count2}] name:{go0.name}");

            }

            MeshRendererInfo[] rendererInfos = go.GetComponentsInChildren<MeshRendererInfo>(true);
            foreach (var info in rendererInfos)
            {
                info.rendererType = MeshRendererType.None;
            }
            Debug.LogError($"ClearLODGroupsEx count1:{count1} count2:{count2}");

        }
        return count2;
    }
#endif

#if UNITY_EDITOR
    [MenuItem("SceneTools/LOD/ClearLODGroups")]
#endif
    public static void ClearLODGroups(GameObject[] gos)
    {
        //GameObject[] gos = Selection.gameObjects;
        int count1 = 0;
        int count2 = 0;
        for (int i = 0; i < gos.Length; i++)
        {
            GameObject go = gos[i];
            LODGroup[] groups = go.GetComponentsInChildren<LODGroup>(true);
            foreach (var group in groups)
            {
                count1++;
                if (group == null) continue;
                LODGroupInfo lODGroupInfo = group.GetComponent<LODGroupInfo>();
                if (lODGroupInfo != null)
                {
                    GameObject.DestroyImmediate(lODGroupInfo);
                }
                MeshRenderer groupRenderer = group.GetComponent<MeshRenderer>();
                if (groupRenderer != null)
                {
                    var lods = group.GetLODs();
                    for (int i1 = 1; i1 < lods.Length; i1++)
                    {
                        LOD lod = lods[i1];
                        var renderers = lod.renderers;
                        foreach (var renderer in renderers)
                        {
                            if (renderer == null) continue;
                            if (renderer.gameObject == null) continue;
                            GameObject.DestroyImmediate(renderer.gameObject);
                        }
                    }
                    List<Transform> children = new List<Transform>();
                    for (int j = 0; j < group.transform.childCount; j++)
                    {
                        var child = group.transform.GetChild(j);
                        children.Add(child);
                    }
                    foreach (var child in children)
                    {
                        GameObject.DestroyImmediate(child.gameObject);
                    }
                }
                else
                {

                }
                count2++;
                GameObject.DestroyImmediate(group);
            }

            MeshRendererInfo[] rendererInfos = go.GetComponentsInChildren<MeshRendererInfo>(true);
            foreach (var info in rendererInfos)
            {
                info.rendererType = MeshRendererType.None;
            }
            Debug.LogError($"ClearLODGroups count1:{count1} count2:{count2}");
        }
    }
#if UNITY_EDITOR
    [MenuItem("SceneTools/LOD/RemoveOthers")]
#endif
    public static void RemoveLODGroupOthers()
    {
        List<MeshRenderer> renderers = GetLodGroupRenderers();
        MeshRenderer[] allRenderers = GameObject.FindObjectsOfType<MeshRenderer>(true);
        for (int i = 0; i < allRenderers.Length; i++)
        {
            MeshRenderer render = allRenderers[i];
            ProgressArg pA = new ProgressArg("RemoveLODGroupOthers", i, allRenderers.Length, render);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            if (renderers.Contains(render))
            {
                continue;
            }
            if (render == null) continue;
            if (render.gameObject == null) continue;
            EditorHelper.UnpackPrefab(render.gameObject);
            GameObject.DestroyImmediate(render.gameObject);
        }
        Debug.Log($"RemoveLODGroupOthers renderers:{renderers.Count} allRenderers:{allRenderers.Length}");
        ProgressBarHelper.ClearProgressBar();
    }

    public static List<MeshRenderer> GetLodGroupRenderers()
    {
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        LODGroup[] groups = GameObject.FindObjectsOfType<LODGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            LODGroup group = groups[i];
            ProgressArg pA = new ProgressArg("GetLodGroupRenderers", i, groups.Length, group);
            if (ProgressBarHelper.DisplayCancelableProgressBar(pA))
            {
                break;
            }
            var lodRenderers = group.GetComponentsInChildren<MeshRenderer>(true);
            renderers.AddRange(lodRenderers);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetLodGroupRenderers groups:{groups.Length} renderers:{renderers.Count}");
        return renderers;
    }

    public static void SetRenderersLODInfo(LODGroup group, List<Renderer> renderers)
    {
        LOD[] lods = group.GetLODs();
        for (int i = 0; i < lods.Length; i++)
        {
            LOD lod = lods[i];
            //LODInfo lodInfo = new LODInfo(lod);
            foreach (var r in lod.renderers)
            {
                if (r == null) continue;
                MeshRendererInfo rendererInfo = MeshRendererInfo.GetInfo(r.gameObject, true);
                //Debug.Log($"renderer:{r},parent:{r.transform.parent.name},path:{r.transform.GetPathToRoot()},rendererInfo:{rendererInfo}");
                rendererInfo.LodIds.Add(i);
                if (renderers != null)
                    renderers.Remove(r);
                rendererInfo.AddType(MeshRendererType.LOD);
            }
            //break;
        }
    }

    public static void MoveToFloorLODsRoot(Transform source)
    {
        MoveToFloorLODsRoot(source, source);
    }

    public static void MoveToFloorLODsRoot(Transform source, Transform target)
    {
        Transform lodRoot = GetFloorLODsRoot(target);
        if (lodRoot != null)
        {
            source.SetParent(lodRoot);
        }
        else
        {
            Debug.LogError($"MoveToFloorLODsRoot lodRoot==null");
        }
    }

    public static Transform GetFloorLODsRoot(Transform parent)
    {
        BuildingModelInfo floor = parent.GetComponentInParent<BuildingModelInfo>();
        if (floor)
        {
            for (int i = 0; i < floor.transform.childCount; i++)
            {
                var child = floor.transform.GetChild(i);
                if (child.name == "LODs")
                {
                    child.name = floor.name + "_LODs";
                    return child;
                }
            }
            for (int i = 0; i < floor.transform.childCount; i++)
            {
                var child = floor.transform.GetChild(i);
                if (child.name == floor.name + "_LODs")
                {
                    return child;
                }
            }
            GameObject go = new GameObject(floor.name + "_LODs");
            go.transform.position = floor.transform.position;
            go.transform.SetParent(floor.transform);
            return go.transform;
        }
        return null;
    }

    public static List<string> LODNames = new List<string>() { "_LOD0", "_LOD1", "_LOD2", "_LOD3", "_LOD4" };

    public static void SetChildrenLODName(GameObject go)
    {
        string name = go.name;
        string[] parts = name.Split('_');
        if (parts.Length == 2)
        {
            string lod = parts[1];
            if (lod.Contains("LOD"))
            {
                var meshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    var child = meshRenderers[i];
                    string cName = GetOriginalName(child.name);
                    child.name = cName + "_" + lod;
                }
            }
            else
            {
                var meshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    var child = meshRenderers[i];
                    string cName = child.name;
                    cName = cName.Replace("lod1", "_LOD1");
                    cName = cName.Replace("lod2", "_LOD2");
                    cName = cName.Replace("lod3", "_LOD3");
                    child.name = cName;
                }
            }
        }
        else
        {
            Debug.LogError($"SetChildrenLODName Not LOD go:{go.name}");
        }
    }

    public static string GetOriginalName(string lodName)
    {
        string cName = lodName.Replace("_New", "");
        foreach (var lodN in LODNames)
        {
            cName = cName.Replace(lodN, "");
        }
        return cName;
    }

    public static LODGroupInfo CreateLODs(GameObject root,bool isClear=true)
    {
        if (root == null) return null;
        if (isClear)
        {
            ClearGroupInfo(root);
        }

        MeshHelper.CenterPivot(root);

        MeshRendererInfoList infoList = MeshRendererInfo.InitRenderers(root);


        MeshRenderer[][] meshRenderersArray = infoList.GetRenderersArray();
        var lods = LODManager.Instance.GetLODsN(meshRenderersArray);
        if (infoList == null)
        {
            Debug.LogError($"CreateLODs root:{root} infoList == null");
            return null;
        }
        if (meshRenderersArray == null)
        {
            Debug.LogError($"CreateLODs root:{root} meshRenderersArray == null");
            return null;
        }
        if (lods == null)
        {
            Debug.LogError($"CreateLODs root:{root} lods == null meshRenderersArray:{meshRenderersArray.Length}");
            return null;
        }
        Debug.Log($"CreateLODs root:{root} list:{infoList.Count}|{infoList.GetInfoString()},meshRenderers:{meshRenderersArray.Length} lods:{lods.Length}");
        LODGroup groupNew = root.AddComponent<LODGroup>();
        if (groupNew == null)
        {
            Debug.LogError($"CreateLODs root:{root} groupNew == null");
        }
        groupNew.SetLODs(lods);
        LODGroupInfo groupInfo = LODGroupInfo.Init(root);
        return groupInfo;
    }

    private static void ClearGroupInfo(GameObject go,bool isForce=false)
    {
        if (go == null) return;
        LODGroup group = go.GetComponent<LODGroup>();
        if (group != null)
        {
            GameObject.DestroyImmediate(group);

            LODGroupInfo groupInfo = go.GetComponent<LODGroupInfo>();
            if (groupInfo != null)
            {
                GameObject.DestroyImmediate(groupInfo);
            }
            BoundsBox[] boxes = go.GetComponentsInChildren<BoundsBox>(true);
            foreach (var box in boxes)
            {
                GameObject.DestroyImmediate(box.gameObject);
            }
        }
        else
        {
            if (isForce)
            {
                LODGroupInfo groupInfo = go.GetComponent<LODGroupInfo>();
                if (groupInfo != null)
                {
                    GameObject.DestroyImmediate(groupInfo);
                }
                BoundsBox[] boxes = go.GetComponentsInChildren<BoundsBox>(true);
                foreach (var box in boxes)
                {
                    GameObject.DestroyImmediate(box.gameObject);
                }
            }
        }
    }

    public static GameObject SetDoorLOD(GameObject door)
    {

        if (door == null)
        {
            Debug.LogError("SetDoorLOD door == null");
            return null;
        }
        string name = door.name;
        ClearGroupInfo(door);

        GameObject doorRoot = door;
        if (door == null)
        {
            Debug.LogError($"SetDoorLOD door == null go:[{name}]");
            return null;
        }
        MeshRenderer meshRenderer = door.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            doorRoot = MeshCombineHelper.SplitByMaterials(door, false);
        }

        MeshRendererInfoList infoList = MeshRendererInfo.InitRenderers(doorRoot);
        if (infoList.Count == 3)
        {
            LODGroup groupNew = doorRoot.AddComponent<LODGroup>();
            //var lods2 = LODManager.Instance.GetLODs3(
            //    infoList.GetRenderers().ToArray(), 
            //    new MeshRenderer[] { infoList[0].meshRenderer, infoList[1].meshRenderer },
            //    new MeshRenderer[] { infoList[1].GetBoundsRenderer() }
            //);

            var lods2 = LODManager.Instance.GetDoorLODs3(
                infoList.GetRenderers().ToArray(),
                new MeshRenderer[] { infoList[1].meshRenderer, infoList[2].meshRenderer },
                new MeshRenderer[] { infoList[1].GetBoundsRenderer() }
            );

            groupNew.SetLODs(lods2);

            LODGroupInfo.Init(doorRoot);
            return doorRoot;
        }
        //else if (infoList.Count == 2)
        //{
        //    LODGroup groupNew = doorRoot.AddComponent<LODGroup>();
        //    //var lods2 = LODManager.Instance.GetLODs3(
        //    //    infoList.GetRenderers().ToArray(), 
        //    //    new MeshRenderer[] { infoList[0].meshRenderer, infoList[1].meshRenderer },
        //    //    new MeshRenderer[] { infoList[1].GetBoundsRenderer() }
        //    //);
        //    var lods2 = LODManager.Instance.GetLODs2(
        //        infoList.GetRenderers().ToArray(),
        //        new MeshRenderer[] { infoList[1].GetBoundsRenderer() }
        //    );
        //    groupNew.SetLODs(lods2);

        //    LODGroupInfo.Init(doorRoot);
        //    return groupNew;
        //}
        else
        {
            Debug.LogWarning($"DoorHelper.SetDoorLOD infoList.Count != 3 door:{doorRoot} infoList:{infoList.Count}");
            return doorRoot;
        }
    }

    public static LOD[] GetLODs2(float[] lvs, MeshRenderer[] renderers0, MeshRenderer[] renderers1)
    {
        LOD[] lods = new LOD[2];
        lods[0] = new LOD(lvs[0], renderers0);     //LOD0 >50% 
                                                   //lods[1]=new LOD(0.2f,new Renderer[1]{render1});         //LOD1  > 20% - 50% 
                                                   // lods[2]=new LOD(0.1f,new Renderer[1]{render1});         //LOD2  > 10% - 20% 
        lods[1] = new LOD(lvs[1], renderers1);        //LOD3  > 1% - 10% 
                                                      //Culled > 0% - 1%
        return lods;
    }

    public static LOD[] GetLODs3(float[] lvs, MeshRenderer[] renderers0, MeshRenderer[] renderers1, MeshRenderer[] renderers2)
    {
        LOD[] lods = new LOD[3];
        lods[0] = new LOD(lvs[0], renderers0);
        lods[1] = new LOD(lvs[1], renderers1);
        lods[2] = new LOD(lvs[2], renderers2);
        return lods;
    }

    public static LOD[] GetLODs4(float[] lvs, MeshRenderer[] renderers0, MeshRenderer[] renderers1, MeshRenderer[] renderers2, MeshRenderer[] renderers3)
    {
        LOD[] lods = new LOD[4];
        lods[0] = new LOD(lvs[0], renderers0);
        lods[1] = new LOD(lvs[1], renderers1);
        lods[2] = new LOD(lvs[2], renderers2);
        lods[3] = new LOD(lvs[3], renderers3);
        return lods;
    }

    public static LOD[] CreateLODs(float[] ls)
    {
        LOD[] lods = new LOD[ls.Length];
        for (int i = 0; i < ls.Length; i++)
        {
            LOD lod = new LOD();
            lod.screenRelativeTransitionHeight = ls[i];
            lod.fadeTransitionWidth = 0;
            lods[i] = lod;
        }
        return lods;
    }

    public static LODGroup CreateEmptyLODs(GameObject obj, float[] ls)
    {
        LODGroup lodGroup = obj.GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            lodGroup = obj.AddComponent<LODGroup>();
        }
        lodGroup.SetLODs(CreateLODs(ls));
        return lodGroup;
    }

    public static LODGroup[] GetLODGroups(GameObject root, bool includeinactive)
    {
        LODGroup[] groups = null;
        if (root == null)
        {
            groups = GameObject.FindObjectsOfType<LODGroup>(includeinactive);
        }
        else
        {
            groups = root.GetComponentsInChildren<LODGroup>(includeinactive);
        }
        return groups;
    }

#if UNITY_EDITOR

    public static SubScene_Base GetLOD0Scene(GameObject dirRoot, LODGroupInfo groupInfo)
    {
        LODGroup group = groupInfo.LODGroup;
        LOD[] lods = group.GetLODs();
        //if (lods[0].renderers[0] == lods[1].renderers[0])
        //{
        //    Debug.LogWarning("GetLOD0Scene lods[0].renderers[0] == lods[1].renderers[0] Group:" + group);
        //    return null;
        //}

        //lods[0].renderers = lods[1].renderers;
        //group.SetLODs(lods);
        GameObject dir = group.gameObject;
        if (dirRoot != null)
        {
            dir = dirRoot;
        }
        else
        {
            BuildingModelInfo modelInfo = group.GetComponentInParent<BuildingModelInfo>();
            if (modelInfo != null)
            {
                dir = modelInfo.gameObject;
            }
        }

        var scene = SubSceneHelper.EditorCreateScene<SubScene_Single>(group.gameObject, SceneContentType.LOD0, false, dir);


        //List<Renderer> allRenderers = new List<Renderer>();
        //for (int i = 0; i < lods.Length; i++)
        //{
        //    var renderers = lods[i].renderers;
        //    foreach(var render in renderers)
        //    {
        //        if(!allRenderers.Contains(render))
        //        {
        //            allRenderers.Add(render);
        //        }
        //    }
        //}
        List<Renderer> renderersOfLOD0 = lods[0].renderers.ToList();
        List<Renderer> renderersOfLOD123 = new List<Renderer>();

        groupInfo.LOD0Renderers = new List<Renderer>(renderersOfLOD0);
        for (int i = 1; i < lods.Length; i++)
        {
            var renderers = lods[i].renderers;
            foreach (var render in renderers)
            {
                if (!renderersOfLOD123.Contains(render))
                {
                    renderersOfLOD123.Add(render);
                }
                if (renderersOfLOD0.Contains(render))
                {
                    renderersOfLOD0.Remove(render);
                    //groupInfo.LOD0Renderers.Add(render);
                }
            }
        }

        lods[0].renderers = lods[1].renderers;
        group.SetLODs(lods);

        List<GameObject> gos = new List<GameObject>();
        for (int i = 0; i < renderersOfLOD0.Count; i++)
        {
            Renderer render = renderersOfLOD0[i];
            if (render == null)
            {
                Debug.LogError($"GetLOD0Scene render == null groupInfo:{groupInfo.name} path:{groupInfo.transform.GetPath()}");
                continue;
            }
            gos.Add(render.gameObject);
        }
        scene.SetObjects(gos);

        scene.Init();
        return scene;
    }

    public static SubScene_Base GetLOD0Scene(GameObject dirRoot, LODGroup group)
    {
        LOD[] lods = group.GetLODs();
        if (lods[0].renderers[0] == lods[1].renderers[0])
        {
            Debug.LogWarning("GetLOD0Scene lods[0].renderers[0] == lods[1].renderers[0] Group:" + group);
            return null;
        }

        //lods[0].renderers = lods[1].renderers;
        //group.SetLODs(lods);
        GameObject dir = group.gameObject;
        if (dirRoot != null)
        {
            dir = dirRoot;
        }
        else
        {
            BuildingModelInfo modelInfo = group.GetComponentInParent<BuildingModelInfo>();
            if (modelInfo != null)
            {
                dir = modelInfo.gameObject;
            }
        }

        var scene = SubSceneHelper.EditorCreateScene<SubScene_Single>(group.gameObject, SceneContentType.LOD0, false, dir);


        //List<Renderer> allRenderers = new List<Renderer>();
        //for (int i = 0; i < lods.Length; i++)
        //{
        //    var renderers = lods[i].renderers;
        //    foreach(var render in renderers)
        //    {
        //        if(!allRenderers.Contains(render))
        //        {
        //            allRenderers.Add(render);
        //        }
        //    }
        //}
        List<Renderer> renderersOfLOD0 = lods[0].renderers.ToList();
        List<Renderer> renderersOfLOD123 = new List<Renderer>();
        for (int i = 1; i < lods.Length; i++)
        {
            var renderers = lods[i].renderers;
            foreach (var render in renderers)
            {
                if (!renderersOfLOD123.Contains(render))
                {
                    renderersOfLOD123.Add(render);
                }
                renderersOfLOD0.Remove(render);
            }
        }

        lods[0].renderers = lods[1].renderers;
        group.SetLODs(lods);

        List<GameObject> gos = new List<GameObject>();
        foreach (var render in renderersOfLOD0)
        {
            gos.Add(render.gameObject);
        }
        scene.SetObjects(gos);

        scene.Init();
        return scene;
    }

    //public static SubScene_Base SaveLOD0(GameObject dirRoot,LODGroup group)
    //{
    //    group = UniformLOD0(group);
    //    var scene = GetLOD0Scene(dirRoot, group);
    //    if (scene != null)
    //    {
    //        scene.SaveScene();
    //        scene.ShowBounds();
    //    }

    //    return scene;
    //}

    public static SubScene_Base SaveLOD0(GameObject dirRoot, LODGroupInfo groupInfo)
    {
        groupInfo.LODGroup = UniformLOD0(groupInfo.LODGroup);
        var scene = GetLOD0Scene(dirRoot, groupInfo);
        if (scene != null)
        {
            scene.SaveScene();
            scene.ShowBounds();
        }

        return scene;
    }

    public static SubSceneBag SaveLOD0s(GameObject dirRoot, LODGroup[] groups)
    {
        SubSceneBag scenes = new SubSceneBag();
        for (int i = 0; i < groups.Length; i++)
        {
            var group = groups[i];
            group = UniformLOD0(group);
            var scene = GetLOD0Scene(dirRoot, group);
            if (scene != null)
            {
                scenes.Add(scene);
            }
        }
        SubSceneHelper.EditorCreateScenes(scenes, (p) =>
        {
            if (p.progress == 1)
            {
                ProgressBarHelper.ClearProgressBar();
            }
            else
            {
                ProgressBarHelper.DisplayProgressBar("SaveLOD0", p);
            }

        });
        return scenes;
    }

#endif

    public static void LOD1ToLOD0(LODGroup group)
    {
        LOD[] lods = group.GetLODs();
        lods[0].renderers = lods[1].renderers;
        group.SetLODs(lods);
    }

    //    public static LODGroup UniformLOD0(LODGroup group)
    //    {
    //        GameObject goOld = group.gameObject;
    //        MeshRenderer renderer = group.GetComponent<MeshRenderer>();
    //        if (renderer == null) return group;

    //#if UNITY_EDITOR
    //        EditorHelper.UnpackPrefab(group.gameObject);
    //#endif
    //        string origName = LODHelper.GetOriginalName(renderer.name);
    //        GameObject newGroupGo = new GameObject(origName);
    //        newGroupGo.transform.position = group.transform.position;
    //        newGroupGo.transform.parent = group.transform.parent;

    //        group.transform.SetParent(newGroupGo.transform);

    //        //List<Transform> childrens = new List<Transform>();
    //        //for(int i=0;i<group.transform.childCount;i++)
    //        //{
    //        //    Transform child = group.transform.GetChild(i);
    //        //    childrens.Add(child);
    //        //}
    //        //foreach(var child in childrens)
    //        //{
    //        //    child.SetParent(newGroupGo.transform);
    //        //}

    //        RendererId.ChangeChildrenParent(group.transform,newGroupGo.transform);

    //        renderer.name = origName+"_LOD0";
    //        LOD[] lods = group.GetLODs();
    //        //LOD[] lodsNew = new LOD[lods.Length];
    //        //for(int i=0;i<lods.Length;i++)
    //        //{
    //        //    lodsNew[i] = lods[i];
    //        //}

    //        LODGroupInfo groupInfo = group.GetComponent<LODGroupInfo>();
    //        if (groupInfo != null)
    //        {
    //            GameObject.DestroyImmediate(groupInfo);
    //        }

    //        GameObject.DestroyImmediate(group);

    //        LODGroup newGroup = newGroupGo.AddComponent<LODGroup>();
    //        newGroup.SetLODs(lods);
    //        //LODGroupInfo groupInfoNew=newGroupGo.AddComponent<LODGroupInfo>();

    //        LODGroupInfo groupInfoNew=LODGroupInfo.Init(newGroup.gameObject);


    //        //MeshRendererInfo info = MeshRendererInfo.GetInfo(group.gameObject);
    //        //info.Init();
    //        RendererId.UpdateId(goOld);



    //#if UNITY_EDITOR
    //        EditorHelper.SelectObject(newGroupGo);
    //#endif

    //#if UNITY_EDITOR

    //        BIMModelInfo bim = newGroupGo.GetComponentInChildren<BIMModelInfo>();
    //        if (bim)
    //        {
    //            EditorHelper.CopyComponent(newGroupGo, bim);
    //            EditorHelper.CopyComponent<RendererId>(newGroupGo, bim.gameObject);
    //            GameObject.DestroyImmediate(bim);
    //        }

    //        var lod1Go = groupInfoNew.LodInfos[1].GetRenderer().gameObject;
    //        EditorHelper.CopyComponent<MeshCollider>(newGroupGo, lod1Go);

    //        MeshCollider mc = newGroupGo.GetComponent<MeshCollider>();
    //        if (mc == null)
    //        {
    //            mc=newGroupGo.AddComponent<MeshCollider>();
    //            mc.sharedMesh = lod1Go.GetComponent<MeshFilter>().sharedMesh;
    //        }

    //        Collider[] colliders = newGroupGo.GetComponentsInChildren<Collider>();
    //        foreach (var c in colliders)
    //        {
    //            if (c.gameObject == newGroupGo) continue;
    //            GameObject.DestroyImmediate(c);
    //        }

    //        RendererId[] rids = newGroupGo.GetComponentsInChildren<RendererId>();
    //        foreach (var c in rids)
    //        {
    //            if (c.gameObject == newGroupGo) continue;
    //            GameObject.DestroyImmediate(c);
    //        }


    //#endif
    //        return newGroup;
    //    }

    public static LODGroup UniformLOD0(LODGroup group)
    {
        GameObject goOld = group.gameObject;
        MeshRenderer renderer = group.GetComponent<MeshRenderer>();
        if (renderer == null) return group;

#if UNITY_EDITOR
        EditorHelper.UnpackPrefab(group.gameObject);
#endif
        string origName = LODHelper.GetOriginalName(renderer.name);
        GameObject newLOD0 = new GameObject(origName);

        //TransformHelper.ClearComponents<MonoBehaviour>(newLOD0);
        //TransformHelper.ClearComponents<Collider>(newLOD0);
        //TransformHelper.ClearComponents<LODGroup>(newLOD0);

        bool r1 = GameObjectExtension.CopyTransformMesh(group.gameObject, newLOD0);
        if (r1)
        {
            MeshHelper.RemoveMeshComponents(group.gameObject, false);
            newLOD0.transform.SetParent(group.transform);

            MeshRenderer renderLod0New = newLOD0.GetComponent<MeshRenderer>();
            newLOD0.name = origName + "_LOD0";
            group.name = origName;

            LOD[] lods = group.GetLODs();
            if (lods[0].renderers.Length != 1)
            {
                Debug.LogError(@"UniformLOD0 lods[0].renderers.Length != 1 ");
            }
            lods[0].renderers = new Renderer[] { renderLod0New };
            group.SetLODs(lods);

            foreach (var lod in lods)
            {
                foreach (var r in lod.renderers)
                {
                    if (r.transform.parent == group.transform.parent)
                    {
                        r.transform.SetParent(group.transform);
                        RendererId rid = RendererId.GetRId(r);
                        rid.SetPid(RendererId.GetId(group.transform), group.transform);
                    }
                }
            }
        }
        else
        {
            GameObject.DestroyImmediate(newLOD0);
        }


        //MeshCollider meshCollider = group.GetComponent<MeshCollider>();
        //if (meshCollider != null)
        //{
        //    meshCollider.sharedMaterial=
        //}
        LODGroupInfo groupInfo = LODGroupInfo.Init(group.gameObject);
        return group;
    }
}

[Serializable]
public class LODTwoRenderersList : List<LODTwoRenderers>
{

    public int LODRendererCount0;
    public int LODRendererCount1;

    public int LODRendererVertexCount0;
    public int LODRendererVertexCount1;

    public void GetVertexInfos()
    {
        int v0 = 0;
        int v1 = 0;
        this.ForEach(i => { v0 += i.vertexCount0; v1 += i.vertexCount1; });
        LODRendererVertexCount0 = v0;
        LODRendererVertexCount1 = v1;
    }

    public Dictionary<MeshRenderer, LODTwoRenderers> dict = new Dictionary<MeshRenderer, LODTwoRenderers>();

    public void InitDict()
    {
        dict.Clear();
        foreach (var item in this)
        {
            if (item == null)
            {
                Debug.LogWarning($"InitDict item == null item:{item.renderer_old_name}");
                continue;
            }
            if (item.renderer_old == null)
            {
                Debug.LogWarning($"InitDict item.renderer_old item:{item.renderer_old_name}");
                continue;
            }
            if (item.renderer_old.meshRenderer == null)
            {
                Debug.LogWarning($"InitDict item.renderer_old.meshRenderer item:{item.renderer_old_name}");
                continue;
            }
            if (dict.ContainsKey(item.renderer_old.meshRenderer))
            {
                Debug.LogWarning($"InitDict dict.ContainsKey(item.renderer_lod0.meshRenderer) item:{item.renderer_old_name} oldItem:{dict[item.renderer_old.meshRenderer]}");
            }
            else
            {
                dict.Add(item.renderer_old.meshRenderer, item);
            }
        }
    }

    //public bool ContainsRenderer(MeshRenderer renderer)
    //{

    //}

    public void AddItem(LODTwoRenderers item)
    {

    }

    public LODTwoRenderersList()
    {

    }

    public LODTwoRenderersList(string name)
    {
        this.ListName = name;
    }

    public string ListName = "";

    public LODTwoRenderersList(string name, List<LODTwoRenderers> list)
    {
        this.ListName = name;
        this.AddRange(list);
        InitDict();
    }

    public LODTwoRenderersList(string name, MeshRendererInfoList list)
    {
        this.ListName = name;
        Init(list);
        InitDict();
    }

    public LODTwoRenderersList(string name, List<GameObject> list)
    {
        this.ListName = name;
        InitList(list);
    }

    public void InitList(List<GameObject> list)
    {
        Init(list);
        InitDict();
    }

    public void InitList<T>(List<T> list) where T : Component
    {
        Init(list);
        InitDict();
    }

    //public LODTwoRenderersList<T>(string name, List<T> list) where T :Component
    //{
    //    this.ListName = name;
    //    Init(list);
    //    InitDict();
    //}

    private void Init(MeshRendererInfoList list)
    {
        foreach (var item in list)
        {
            LODTwoRenderers t = new LODTwoRenderers(item);
            this.Add(t);
        }
    }

    private void Init(List<GameObject> list)
    {
        foreach (var item in list)
        {
            LODTwoRenderers t = new LODTwoRenderers(item);
            this.Add(t);
        }
    }

    private void Init<T>(List<T> list) where T : Component
    {
        foreach (var item in list)
        {
            if (item == null) continue;
            if (item.gameObject == null) continue;
            LODTwoRenderers t = new LODTwoRenderers(item.gameObject);
            this.Add(t);
        }
    }

    public LODTwoRenderersList(string name, MeshRenderer[] list)
    {
        this.ListName = name;
        MeshRendererInfoList rendererInfoList = new MeshRendererInfoList(list);
        Init(rendererInfoList);
    }
    public LODTwoRenderersList(string name, List<MeshRenderer> list)
    {
        this.ListName = name;
        MeshRendererInfoList rendererInfoList = new MeshRendererInfoList(list);
        Init(rendererInfoList);
    }


    public void ClearTwoList()
    {
        var list = this.FindAll(i => i.renderer_old != null || i.renderer_new != null);
        this.Clear();
        this.AddRange(list);
    }

    public LODTwoRenderersList FindList(string searchKey)
    {
        var list = this.Where(i => i.GetLODCaption().Contains(searchKey)).ToList();
        return new LODTwoRenderersList(this.ListName + "_" + searchKey, list);
    }

    internal List<MeshRendererInfo> GetRendererInfosOld()
    {
        List<MeshRendererInfo> list = new List<MeshRendererInfo>();
        foreach (var item in this)
        {
            list.Add(item.renderer_old);
        }
        return list;
    }

    internal List<MeshRendererInfo> GetRendererInfosNew()
    {
        List<MeshRendererInfo> list = new List<MeshRendererInfo>();
        foreach (var item in this)
        {
            list.Add(item.renderer_new);
        }
        return list;
    }

    internal void RemoveRenderers(List<MeshRenderer> doorRenderers)
    {

        for (int i = 0; i < this.Count; i++)
        {
            var item = this[i];
            if (doorRenderers.Contains(item.renderer_old.meshRenderer))
            {
                this.RemoveAt(i);
                i--;
            }
        }

        InitDict();
    }

    internal void AddRenderers(List<MeshRenderer> newRenderers)
    {
        List<LODTwoRenderers> newList = new List<LODTwoRenderers>();
        for (int i = 0; i < newRenderers.Count; i++)
        {
            var renderer = newRenderers[i];
            if (!dict.ContainsKey(renderer))
            {
                LODTwoRenderers t = new LODTwoRenderers(renderer);
                newList.Add(t);
            }
        }

        this.AddRange(newList);

        InitDict();
    }

    internal void AddRenderers(List<MeshRendererInfo> newRenderers)
    {
        List<LODTwoRenderers> newList = new List<LODTwoRenderers>();
        for (int i = 0; i < newRenderers.Count; i++)
        {
            var rendererInfo = newRenderers[i];
            if (!dict.ContainsKey(rendererInfo.meshRenderer))
            {
                LODTwoRenderers t = new LODTwoRenderers(rendererInfo);
                newList.Add(t);
            }
        }

        this.AddRange(newList);

        InitDict();
    }

    LODTwoRenderersList targetList;
    int MaxCompareCount;
    LODCompareMode compareMode;

    public string GetTargetListInfo()
    {
        if (targetList == null)
        {
            return "NULL";
        }
        else
        {
            return $"{targetList.ListName}({targetList.Count})";
        }
    }

    public void SetTargetList(LODTwoRenderersList targetList, int MaxCompareCount, LODCompareMode compareMode)
    {
        this.targetList = targetList;
        this.MaxCompareCount = MaxCompareCount;
        this.compareMode = compareMode;

        this.RemoveUpdated();
    }

    public void TestInitSharedMesh()
    {
        List<MeshRendererInfo> list_lod0 = new List<MeshRendererInfo>();
        if (targetList == null)
        {
            Debug.LogError("targetList == null:" + this.ListName);
            return;
        }
        list_lod0.AddRange(targetList.GetRendererInfosOld());
        InitSharedMesh(list_lod0);
    }

    public void InitSharedMesh(List<MeshRendererInfo> list_lod0)
    {
        sharedMeshDict.Clear();
        foreach (var t in list_lod0)
        {
            MeshFilter meshFilter = t.meshFilter;
            if (meshFilter != null)
            {
                var mesh = meshFilter.sharedMesh;
                if (mesh == null) continue;
                if (!sharedMeshDict.ContainsKey(mesh))
                {
                    // if(t.name.Contains("_New")||sharedMeshDict[mesh].name.Contains("_New")
                    // ||t.name.Contains("_Bounds")||sharedMeshDict[mesh].name.Contains("_Bounds")){

                    // }
                    // else{
                    //     //Debug.LogError($"Same SharedMesh t1:{sharedMeshDict[mesh]} t2:{meshFilter}");
                    // }
                    sharedMeshDict.Add(mesh, new List<MeshRendererInfo>());
                }
                // else{
                //     sharedMeshDict.Add(mesh,t);
                // }
                sharedMeshDict[mesh].Add(t);
            }
        }
        Debug.LogError($"InitSharedMesh list_lod0:{list_lod0.Count} sharedMeshDict:{sharedMeshDict.Count}");
    }

    internal void Compare()
    {
        //this.renderers_2 = renderers_2;
        //this.MaxCompareCount = MaxCompareCount;
        //this.compareMode = compareMode;

        DateTime start = DateTime.Now;

        List<MeshRendererInfo> list_lod0 = new List<MeshRendererInfo>();
        if (targetList == null)
        {
            Debug.LogError("targetList == null:" + this.ListName);
            return;
        }
        list_lod0.AddRange(targetList.GetRendererInfosOld());
        //renderers_0.ToList().ForEach(i => { ts.Add(i.transform); });

        if (compareMode == LODCompareMode.SharedMesh || compareMode == LODCompareMode.NameWithSharedMesh)
        {
            InitSharedMesh(list_lod0);
        }

        int maxCount = this.Count;
        if (MaxCompareCount > 0 && MaxCompareCount < maxCount)
        {
            maxCount = MaxCompareCount;
        }
        for (int i = 0; i < maxCount; i++)
        {
            LODTwoRenderers item = this[i];

            ProgressArg p1 = new ProgressArg("Compare", i, maxCount, item.renderer_old_name);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                break;
            }
            CompareOne(item, list_lod0, p1);
        }
        ProgressBarHelper.ClearProgressBar();

        this.Sort((a, b) =>
        {
            return b.dis.CompareTo(a.dis);
        });

        //RemoveEmpty(Model_New.transform);
        //RemoveEmpty(Model_New.transform);
        //RemoveEmpty(Model_New.transform);

        Debug.LogError($"LODTwoRenderersList.Compare count1:{targetList.Count} count0:{this.Count} time:{(DateTime.Now - start)}");
    }

    public static Dictionary<Mesh, List<MeshRendererInfo>> sharedMeshDict = new Dictionary<Mesh, List<MeshRendererInfo>>();

    public MinDisTarget<MeshRendererInfo> CompareOne(LODTwoRenderers item, List<MeshRendererInfo> list_lod0, ProgressArg p1)
    {
        MeshRendererInfo oldRenderer = item.renderer_old;
        if (oldRenderer == null) return null;

        if (compareMode == LODCompareMode.SharedMesh || compareMode == LODCompareMode.NameWithSharedMesh)
        {
            var mf = oldRenderer.meshFilter;
            if (mf == null)
            {

            }
            var mesh = mf.sharedMesh;
            if (sharedMeshDict.ContainsKey(mesh))
            {
                List<MeshRendererInfo> list = sharedMeshDict[mesh];
                if (list.Count == 1)
                {
                    var newInfo = list[0] as MeshRendererInfo;
                    MinDisTarget<MeshRendererInfo> min2 = new MinDisTarget<MeshRendererInfo>(0, 0, newInfo);
                    item.SetMinDisTarget(min2);
                    return min2;
                }
                else
                {
                    MinDisTarget<MeshRendererInfo> min2 = GetMinDisTransform<MeshRendererInfo>(list, oldRenderer.transform, LODCompareMode.NameWithCenter, p1);
                    item.SetMinDisTarget(min2);
                    return min2;
                }
            }
            else
            {
                MinDisTarget<MeshRendererInfo> min2 = new MinDisTarget<MeshRendererInfo>(50, 50, null);
                item.SetMinDisTarget(min2);
                return min2;
            }

        }
        MinDisTarget<MeshRendererInfo> min = GetMinDisTransform<MeshRendererInfo>(list_lod0, oldRenderer.transform, compareMode, p1);

        // float minDis = min.dis;
        // MeshRendererInfo newRenderer = min.target;
        // if (newRenderer == null)
        // {
        //     Debug.LogError("render_lod0 == null");
        //     return null; ;
        // }
        // //MeshFilter filter1 = render_lod1.GetComponent<MeshFilter>();
        // //MeshFilter filter0 = render_lod0.GetComponent<MeshFilter>();
        // int vertexCount0 = newRenderer.GetMinLODVertexCount();
        // int vertexCount1 = oldRenderer.GetMinLODVertexCount();
        // //LODTwoRenderers lODTwoRenderers = new LODTwoRenderers(render_lod0, render_lod1, minDis, min.meshDis, vertexCount0, vertexCount1);
        // //this.Add(lODTwoRenderers);

        // item.SetLOD1(newRenderer, minDis, min.meshDis, vertexCount0, vertexCount1);

        item.SetMinDisTarget(min);
        return min;
    }

    public static void RemoveEmpty(Transform root)
    {
        Transform[] cs = root.GetComponentsInChildren<Transform>(true);
        foreach (var c in cs)
        {
            if (c.childCount > 0) continue;
            MeshRenderer meshRenderer = c.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                GameObject.DestroyImmediate(c.gameObject);
            }
        }
    }



    public static MinDisTarget<T> GetMinDisTransform<T>(List<T> ts, Transform t, LODCompareMode mode, ProgressArg p1) where T : Component
    {
        //1.Find SameName
        //2.Find Closed
        List<T> ts0 = ts;
        MinDisTarget<T> min = null;
        if (mode == LODCompareMode.Name
            || mode == LODCompareMode.NameWithPos
            || mode == LODCompareMode.NameWithCenter
            || mode == LODCompareMode.NameWithMin
            || mode == LODCompareMode.NameWithMax
            || mode == LODCompareMode.NameWithMesh
            || mode == LODCompareMode.NameWithBounds
            || mode == LODCompareMode.NameWithSharedMesh)
        {

            var ts2 = ts.FindAll(i => i != null && i.name == t.name);

            // Debug.LogError($"GetMinDisTransform mode={mode} t:{t.name} ts:{ts.Count} ts2:{ts2.Count}");

            //string tn = t.name;
            //if(tn.Contains(" "))
            //{
            //    tn = tn.Split(' ')[0];
            //}
            //var ts2 = ts.FindAll(i => i.name.Contains(tn));

            if (ts2.Count > 0)
            {
                ts = ts2;
            }

            min = GetMinDisTransformInner(ts, t, mode, p1);

            if (min.dis > 0.01f && mode != LODCompareMode.Name)
            {
                min = GetMinDisTransformInner(ts0, t, mode, p1);
            }
        }
        else
        {
            min = GetMinDisTransformInner(ts0, t, mode, p1);
        }

        if (min.dis <= 0.01f)
        {
            ts0.Remove(min.target);
        }
        return min;
    }

    public static float MaxVertexCountPower = 10;

    private static MinDisTarget<T> GetMinDisTransformInner<T>(List<T> ts, Transform t, LODCompareMode mode, ProgressArg p1) where T : Component
    {
        float minDis = float.MaxValue;
        float minDisOffCenter = float.MaxValue;
        float minDisOfMesh = float.MaxValue;
        List<T> minTList = new List<T>();
        List<T> minTExList = new List<T>();

        //Debug.LogError($"GetMinDisTransformInner mode={mode} t:{t.name} ts:{ts.Count}");



        for (int i = 0; i < ts.Count; i++)
        {
            T item = ts[i];
            if (item == null)
            {
                Debug.LogError($"GetMinDisTransformInner item == null item:{item}");
                continue;
            }

            //MeshRendererInfo info0 = MeshRendererInfo.GetInfo(t.gameObject, false);
            //MeshRendererInfo info1 = MeshRendererInfo.GetInfo(item.gameObject, false);
            //float v0 = info0.GetVertexCount();
            //float v1 = info1.GetVertexCount();
            //float vp = v0 / v1;
            ////if (v0 > 100 && (vp > MaxVertexCountPower || vp < 1f / MaxVertexCountPower)) 
            ////{
            ////    float distance1 = GetDistance(item, t, mode);
            ////    Debug.LogError($"GetMinDisTransformInner[{i}] mode={mode} t:{t.name} item:{item.name} distance:{distance1} v0:{v0} v1:{v1} vp:{vp}");
            ////    continue;
            ////}

            ////float distance = Vector3.Distance(item.transform.position, t.position);

            ////float distance = GetCenterDistance(item.gameObject, t.gameObject);

            float distance = GetDistance(item, t, mode);

            ////Debug.Log($"GetMinDisTransformInner[{i}] mode={mode} t:{t.name} item:{item.name} distance:{distance} v0:{v0} v1:{v1} vp:{vp}");

            ////if (distance < 1)
            ////{
            ////    float distance2 = MeshHelper.GetAvgVertexDistanceEx(item.transform, t);
            ////    if (distance2 < minDisOfMesh)
            ////    {
            ////        minDisOfMesh = distance2;
            ////        minTExList = new List<T>() { item };
            ////    }
            ////    else if (distance2 == minDisOfMesh)
            ////    {
            ////        minTExList.Add(item);
            ////    }
            ////}

            ////if (p1 != null)
            ////{
            ////    ProgressArg p2 = new ProgressArg("GetDistance", i, ts.Count, item.name);
            ////    p1.AddSubProgress(p2);
            ////    ProgressBarHelper.DisplayCancelableProgressBar(p1);
            ////}

            ////float distance = distance2;
            if (distance < minDis)
            {
                minDis = distance;
                minTList = new List<T>() { item };
                ////minDisOfMesh = MeshHelper.GetAvgVertexDistanceEx(item.transform, t);
                minDisOfMesh = MeshHelper.GetVertexDistanceEx(item.transform, t);
            }
            else if (distance == minDisOfMesh)
            {
                minTList.Add(item);
            }
        }
        T minTEx = null;
        if (minTList.Count > 0)
        {
            T minT = minTList[0];

            if (minTExList.Count > 0)
            {
                minTEx = minTExList[0];
            }

            ////float distance3 = MeshHelper.GetAvgVertexDistanceEx(minT.transform, t);
            ////if (distance3 < minDisOfMesh)
            ////{
            ////    minDisOfMesh = distance3;
            ////    minTEx = minT;
            ////}

            if (minTEx == null)
            {
                minTEx = minT;
            }
        }


        return new MinDisTarget<T>(minDis, minDisOfMesh, minTEx);
    }

    public void ClearNew()
    {
        foreach (var item in this)
        {
            item.ClearNew();
        }
    }

    public float zeroDistance = 0.002f;

    public string searchKey { get; set; }

    public string searchKeyInput { get; set; }

    public bool isShowSize = true;

    public bool isMaterial = true;

    public void DeleteNewOld()
    {
        foreach (var item in this)
        {
            if (item.dis < zeroDistance)
            {
                item.SetUpdateState(UpdateChangedMode.NewDelete);//
            }
            else
            {
                item.SetUpdateState(UpdateChangedMode.OldDelete);
            }
        }
    }

    internal void RemoveUpdated()
    {
        for (int i = 0; i < this.Count; i++)
        {
            var item = this[i];
            var upInfo = item.renderer_old.gameObject.GetComponent<RendererUpdateInfo>();
            if (upInfo != null)
            {
                this.RemoveAt(i);
                i--;
            }
        }
    }

    public void DeleteNew()
    {
        //foreach (var item in this)
        //{
        //    if (item.dis < zeroDistance)
        //    {
        //        item.SetUpdateState(UpdateChangedMode.NewDelete);//
        //    }
        //    //else
        //    //{
        //    //    item.SetUpdateState(UpdateChangedMode.OldDelete);
        //    //}
        //}

        SetUpdateState(UpdateChangedMode.NewDelete);
    }

    public void DeleteOld()
    {
        //foreach (var item in this)
        //{
        //    if (item.dis < zeroDistance)
        //    {
        //        item.SetUpdateState(UpdateChangedMode.OldDelete);//
        //    }
        //}
        SetUpdateState(UpdateChangedMode.OldDelete);
    }

    public void SetUpdateState(UpdateChangedMode updateMode)
    {
        foreach (var item in this)
        {
            if (item.dis < zeroDistance)
            {
                item.SetUpdateState(updateMode);//
            }
        }
    }

    public void DeleteSame()
    {
        //foreach (var item in this)
        //{
        //    if (item.meshDis == 0)
        //    {
        //        item.SetUpdateState(UpdateChangedMode.NewSame);
        //    }
        //}
        SetUpdateState(UpdateChangedMode.NewSame);
    }

    public void RenameNew()
    {
        foreach (var item in this)
        {
            if (item.dis < zeroDistance)
            {
                item.Rename();
            }
        }
    }

    public void ReplaceMaterialNew()
    {
        foreach (var item in this)
        {
            if (item.dis < zeroDistance)
            {
                item.SetUpdateState(UpdateChangedMode.MatNew);
                item.ReplaceMaterialNew();
            }
        }
    }

    public void ReplaceMaterialOld()
    {
        foreach (var item in this)
        {
            if (item.dis < zeroDistance)
            {
                item.SetUpdateState(UpdateChangedMode.MatOld);
                item.ReplaceMaterialOld();
            }
        }
    }

    public void ResetUpdateState()
    {
        foreach (var item in this)
        {
            //if (item.dis < zeroDistance)
            {
                item.SetUpdateState(UpdateChangedMode.None);
                item.ReplaceMaterialOld();
            }
        }
    }


    public void ReplaceOld()
    {
        foreach (var item in this)
        {
            if (item.meshDis == 0)
            {
                //item.SetUpdateState(UpdateChangedMode.NewDelete);
                item.SetUpdateState(UpdateChangedMode.NewChanged);//
            }
            else if (item.dis < zeroDistance)
            {
                item.SetUpdateState(UpdateChangedMode.NewChanged);//
            }
            //else
            //{
            //    item.SetUpdateState(UpdateChangedMode.OldDelete);
            //}
            //else
            //{
            //    item.SetUpdateState(UpdateChangedMode.OldDelete);
            //}
        }
    }

    public void ReplaceNew()
    {
        foreach (var item in this)
        {
            if (item.meshDis == 0)
            {
                //item.SetUpdateState(UpdateChangedMode.NewDelete);
                item.SetUpdateState(UpdateChangedMode.NewChanged);//
            }
            else if (item.dis < zeroDistance)
            {
                item.SetUpdateState(UpdateChangedMode.NewChanged);//
            }
            else
            {
                item.SetUpdateState(UpdateChangedMode.OldDelete);
            }
            //else
            //{
            //    item.SetUpdateState(UpdateChangedMode.OldDelete);
            //}
        }
    }

    public void AlignOld()
    {
        throw new NotImplementedException();
    }



    public void CompareList(int maxCompareCount, LODCompareMode compareMode)
    {
        this.MaxCompareCount = maxCompareCount;
        this.compareMode = compareMode;
        Compare();
    }

    public void DoUpdate()
    {
        foreach (var item in this)
        {
            item.DoUpdate();
        }
        Debug.LogError($"DoUpdate:{this.Count}");
    }

    internal LODTwoRenderersList[] FindRenderers(string v)
    {
        LODTwoRenderersList list1 = new LODTwoRenderersList(this.ListName + "_1");
        LODTwoRenderersList list2 = new LODTwoRenderersList(this.ListName + "_2");
        foreach (var item in this)
        {
            if (item == null)
            {
                list2.Add(item);
                continue;
            }
            if (item.renderer_old_name.Contains(v))
            {
                list1.Add(item);
            }
            else
            {
                list2.Add(item);
            }
        }
        return new LODTwoRenderersList[] { list1, list2 };
    }


    internal void RemoveRenderersByKey(string v)
    {
        LODTwoRenderersList list = FindRenderers(v)[0];
        foreach (var item in list)
        {
            this.Remove(item);
        }
    }

    public MinDisTarget<MeshRendererInfo> CompareOne(LODTwoRenderers item, LODCompareMode compareMode)
    {
        this.compareMode = compareMode;

        List<MeshRendererInfo> list_lod0 = new List<MeshRendererInfo>();
        if (targetList == null)
        {
            Debug.LogError("targetList == null:" + this.ListName);
            return null;
        }
        list_lod0.AddRange(targetList.GetRendererInfosOld());
        return CompareOne(item, list_lod0, null);
    }

    public MinDisTarget<MeshRendererInfo> GetMinInfo(Transform t, LODCompareMode compareMode)
    {
        this.compareMode = compareMode;

        List<MeshRendererInfo> list_lod0 = new List<MeshRendererInfo>();
        if (targetList == null)
        {
            Debug.LogError("targetList == null:" + this.ListName);
            return null;
        }
        var min = LODTwoRenderersList.GetMinDisTransform<MeshRendererInfo>(list_lod0, t, compareMode, null);
        return min;
    }

}


