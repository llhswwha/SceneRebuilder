using MeshProfilerNS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static LODManager;
using static MeshHelper;

public class ModelUpdateManager : SingletonBehaviour<ModelUpdateManager>
{
    public GameObject Model_Old;

    public MeshRenderer[] ModelRenders_Old_All;

    public MeshRendererInfoList MeshRendererInfoList_Old;

    //public MeshElement MeshElement_Old;

    public LODTwoRenderersList ModelRendersWaiting_Old = new LODTwoRenderersList("Old");

    public LODTwoRenderersList ModelRendersWaiting_Old_Door = new LODTwoRenderersList("Door");

    public LODTwoRenderersList ModelRendersWaiting_Old_LodDevs = new LODTwoRenderersList("LodDevs");

    public LODTwoRenderersList ModelRendersWaiting_Old_Walls = new LODTwoRenderersList("Walls");

    public LODTwoRenderersList ModelRendersWaiting_Old_Welding = new LODTwoRenderersList("OldWelding");

    public LODTwoRenderersList ModelRendersWaiting_Old_Piping = new LODTwoRenderersList("OldPiping");

    public LODTwoRenderersList ModelRendersWaiting_Old_MemberPart = new LODTwoRenderersList("OldMemberPart");

    public LODTwoRenderersList ModelRendersWaiting_Old_Others = new LODTwoRenderersList("Others");

    public List<string> FilterFiles = new List<string>() { "HH.fbx", "JQ.fbx", "JG.fbx", "SG.fbx" };

    public GameObject Model_New;

    public MeshRenderer[] ModelRenders_New;

    public LODTwoRenderersList ModelRendersWaiting_New = new LODTwoRenderersList("New");

    public LODTwoRenderersList ModelRendersWaiting_MemberPart = new LODTwoRenderersList("MemberPart");

    public LODTwoRenderersList ModelRendersWaiting_WallPart = new LODTwoRenderersList("WallPart");

    public LODTwoRenderersList ModelRendersWaiting_Welding = new LODTwoRenderersList("NewWelding");

    public LODTwoRenderersList ModelRendersWaiting_Piping = new LODTwoRenderersList("NewPiping");

    public LODTwoRenderersList ModelRendersWaiting_NewOthers = new LODTwoRenderersList("NotMemberPart");

    public LODTwoRenderersList twoList = new LODTwoRenderersList();

    public Dictionary<string, MeshRendererInfoList> modelFiles;

    public List<string> modelFilePaths = new List<string>();

    public bool isIncludeInactive = true;

    public static List<MeshRenderer> FindRenderers(GameObject go, string key)
    {
        List<Transform> findList = new List<Transform>();
        Transform[] ts = go.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.name.ToLower().Contains(key))
            {
                findList.Add(t);
            }
        }
        List<MeshRenderer> renderers = new List<MeshRenderer>();
        foreach (var t in findList)
        {
            MeshRenderer[] rs = t.GetComponentsInChildren<MeshRenderer>(true);
            foreach(var r in rs)
            {
                if (r.name.Contains("Bounds")) continue;
                if(!renderers.Contains(r))
                    renderers.Add(r);
            }
            //renderers.AddRange(rs);
        }
        return renderers;
    }

    public MeshRendererInfoList GetFilteredList(List<MeshRenderer> lodRenderers2)
    {
        MeshRendererInfoList lodRenderersInfoList2 = new MeshRendererInfoList(lodRenderers2);
        var lodFiltedList2 = lodRenderersInfoList2.FilterRenderersByFile(FilterFiles);
        lodFiltedList2 = lodFiltedList2.GetLODs(0, -1);
        lodFiltedList2.Sort();
        return lodFiltedList2;
    }

    public void ClearUpdates()
    {
        var infos = Model_Old.GetComponentsInChildren<RendererUpdateInfo>(true);
        foreach(var info in infos)
        {
            GameObject.DestroyImmediate(info);
        }
        Debug.LogError($"ClearUpdates infos:{infos.Length}");
    }

    public void GetModelRenders()
    {
        ModelRenders_Old_All = Model_Old.GetComponentsInChildren<MeshRenderer>(isIncludeInactive);
        MeshRendererInfoList_Old = new MeshRendererInfoList(ModelRenders_Old_All, true);
        modelFiles = MeshRendererInfoList_Old.GetAssetPaths();
        modelFilePaths = modelFiles.Keys.ToList();
        modelFilePaths.Sort();


        var oldFiltedList = MeshRendererInfoList_Old.FilterRenderersByFile(FilterFiles);
        oldFiltedList = oldFiltedList.GetLODs(0, -1);
        oldFiltedList.Sort();

        ModelRendersWaiting_Old = new LODTwoRenderersList("OldAll",oldFiltedList);

        ModelRendersWaiting_Old_Others = new LODTwoRenderersList("Others",oldFiltedList);
        ModelRendersWaiting_Old_Others.RemoveRenderersByKey("_Bounds_");

        DoorsRootList doorRoots = DoorManager.UpdateDoors(Model_Old, null);
        DoorInfoList doors = doorRoots.GetDoors();
        //DoorPartInfoList doorParts = doors.GetDoorParts();
        var doorRenderers = doors.GetMeshRenderers();
        var doorGos = doors.GetGameObjects();
        Debug.LogError($"doors:{doors.Count} doorRenderers:{doorRenderers.Count}");
        ModelRendersWaiting_Old_Door = new LODTwoRenderersList("Doors",doorGos);
        ModelRendersWaiting_Old_Door.zeroDistance = 0.9f;
        ModelRendersWaiting_Old_Others.RemoveRenderers(doorRenderers);

        List<MeshRenderer> lodRenderers1 = LODManager.GetLODRenderers(Model_Old);

        MeshRendererInfoList lodRenderersInfoList1 = new MeshRendererInfoList(lodRenderers1);
        var lodFiltedList1 = lodRenderersInfoList1.FilterRenderersByFile(FilterFiles);
        lodFiltedList1.Sort();

        ModelRendersWaiting_Old_LodDevs = new LODTwoRenderersList("LodDevs", lodFiltedList1);
        ModelRendersWaiting_Old_LodDevs.RemoveRenderers(doorRenderers);
        ModelRendersWaiting_Old_Others.RemoveRenderers(lodRenderers1);

        List<MeshRenderer> lodRenderers2 = FindRenderers(Model_Old, "lod");
        var lodFiltedList2 = GetFilteredList(lodRenderers2);
        ModelRendersWaiting_Old_LodDevs.AddRenderers(lodFiltedList2);
        ModelRendersWaiting_Old_Others.RemoveRenderers(lodRenderers2);

        List<MeshRenderer> weldingRenderers = FindRenderers(Model_Old, "welding");
        var weldingFiltedList = GetFilteredList(weldingRenderers);
        ModelRendersWaiting_Old_Welding=new LODTwoRenderersList("OldWelding", weldingFiltedList);
        ModelRendersWaiting_Old_Others.RemoveRenderers(weldingRenderers);

        List<MeshRenderer> pipingRenderers = FindRenderers(Model_Old, "piping");
        var pipingFiltedList = GetFilteredList(pipingRenderers);
        ModelRendersWaiting_Old_Piping = new LODTwoRenderersList("OldPiping", pipingFiltedList);
        ModelRendersWaiting_Old_Others.RemoveRenderers(pipingRenderers);

        var wallRenderers = FindRenderers(Model_Old, "wall");
        ModelRendersWaiting_Old_Walls= new LODTwoRenderersList("Walls", wallRenderers);
        ModelRendersWaiting_Old_Others.RemoveRenderers(wallRenderers);

        //ModelRendersWaiting_Old_MemberPart

        var findList11 = ModelRendersWaiting_Old_Others.FindRenderers("MemberPart");
        ModelRendersWaiting_Old_MemberPart = findList11[0];
        
        ModelRendersWaiting_Old_Others = findList11[1];


        MeshElement.IsShowProgress = true;
        // MeshElement_Old = new MeshElement(Model_Old, false, false);

        //MeshRendererInfoList lods = ModelRendersWaiting_Old.GetLODs();
        //ModelRendersWaiting_Old.RemoveLODs();

        ModelRenders_New = Model_New.GetComponentsInChildren<MeshRenderer>(isIncludeInactive);
        var list2 = new MeshRendererInfoList(ModelRenders_New);
        list2.Sort();
        ModelRendersWaiting_New = new LODTwoRenderersList("New",list2);

        //1.�ҵ�����ģ���е� �ţ��豸(LOD)��ǽ�ڣ�����
        //2.�ҵ���Щģ�Ͷ�Ӧ����ģ��
        //3.ɾ����ģ���е��š��豸(LOD)��ǽ�ڣ���Ӧ��ģ�ͣ�ǽ���е����⣬�����Ƿֲ��˵ġ�
        //4.�ҵ�����ģ�Ͷ�Ӧ����ģ�ͣ��ж϶�Ӧ��ģ���Ƿ����˱仯��ɾ��û�з����仯�ģ��滻�����˱仯�ġ�
        //ʣ������ ��ģ�ͣ�����Ҫ�����ģ�������ģ��Ҫ�ֲ㣬Ҫ�ҵ����ڵ�¥�㡣
        //û���ҵ��� ��ģ�ͣ�����Ҫɾ���ġ�

        var findList2= ModelRendersWaiting_New.FindRenderers("MemberPart");
        ModelRendersWaiting_MemberPart = findList2[0];
        ModelRendersWaiting_NewOthers = findList2[1];

        var findList3 = ModelRendersWaiting_NewOthers.FindRenderers("WallPart");
        ModelRendersWaiting_WallPart = findList3[0];
        ModelRendersWaiting_NewOthers = findList3[1];

        //var findList4 = ModelRendersWaiting_NewOthers.FindRenderers("Welding");
        //ModelRendersWaiting_Welding = findList4[0];
        //ModelRendersWaiting_NewOthers = findList4[1];

        //var findList5 = ModelRendersWaiting_NewOthers.FindRenderers("Piping");
        //ModelRendersWaiting_Piping = findList5[0];
        //ModelRendersWaiting_NewOthers = findList5[1];

        List<MeshRenderer> weldingRenderers2 = FindRenderers(Model_New, "welding");
        var weldingFiltedList2 = GetFilteredList(weldingRenderers2);
        ModelRendersWaiting_Welding = new LODTwoRenderersList("NewWelding", weldingFiltedList2);
        ModelRendersWaiting_NewOthers.RemoveRenderers(weldingRenderers2);

        List<MeshRenderer> pipingRenderers2 = FindRenderers(Model_New, "piping");
        var pipingFiltedList2 = GetFilteredList(pipingRenderers2);
        ModelRendersWaiting_Piping = new LODTwoRenderersList("NewPiping", pipingFiltedList2);
        ModelRendersWaiting_NewOthers.RemoveRenderers(pipingRenderers2);


        

        ModelRendersWaiting_Old.SetTargetList(ModelRendersWaiting_NewOthers, 0, compareMode);
        ModelRendersWaiting_Old_Door.SetTargetList(ModelRendersWaiting_NewOthers, 0, compareMode);
        ModelRendersWaiting_Old_LodDevs.SetTargetList(ModelRendersWaiting_NewOthers, 0, compareMode);
        ModelRendersWaiting_Old_Walls.SetTargetList(ModelRendersWaiting_WallPart, 0, compareMode);
        ModelRendersWaiting_Old_Welding.SetTargetList(ModelRendersWaiting_Welding, 0, compareMode);
        ModelRendersWaiting_Old_Piping.SetTargetList(ModelRendersWaiting_Piping, 0, compareMode);
        ModelRendersWaiting_Old_Others.SetTargetList(ModelRendersWaiting_NewOthers, 0, compareMode);

        ModelRendersWaiting_MemberPart.AddRenderers(ModelRendersWaiting_NewOthers.GetRendererInfosOld());

        ModelRendersWaiting_Old_MemberPart.SetTargetList(ModelRendersWaiting_MemberPart, 0, compareMode);

        ModelRendersWaiting_Welding.SetTargetList(ModelRendersWaiting_Old_Welding, 0, compareMode);
        ModelRendersWaiting_NewOthers.SetTargetList(ModelRendersWaiting_Old_Others, 0, compareMode);
    }

    public List<MeshRendererInfo> list_lod0 = new List<MeshRendererInfo>();

    public void ReplaceOld()
    {
        throw new NotImplementedException();
    }

    public LODCompareMode compareMode = LODCompareMode.NameWithCenter;

    public void DeleteNew()
    {
        int count = 0;
        foreach (var t in twoList)
        {
            if (t.dis < MinDistance && t.meshDis < MinDistance)
            {
                count++;
                GameObject.DestroyImmediate(t.renderer_new.gameObject);
            }
        }
        Debug.LogError($"DeleteNew count:{count}");
    }

    public List<GameObject> NewAdded = new List<GameObject>();//����
    public List<GameObject> OldDeleted = new List<GameObject>();//�޸�
    public List<GameObject> NotChanged = new List<GameObject>();//ɾ��

    public float zeroDistance = 0.0002f;

    public LODTwoRenderersList CompareModels(GameObject goOld, GameObject goNew)
    {
        Model_New = goNew;
        Model_Old = goOld;
        return CompareModels();
    }

    public MinDisTarget<MeshRendererInfo> GetMinInfo(Transform t)
    {
        var min = LODTwoRenderersList.GetMinDisTransform<MeshRendererInfo>(list_lod0, t, compareMode, null);
        return min;
    }

    public int MaxCompareCount = 0;

    //public void CompareModels_Doors()
    //{
    //    Debug.LogError("CompareModels_Doors");
    //    ModelRendersType1_Door.SetTargetList(ModelRendersWaiting_New, 0, compareMode);
    //    ModelRendersType1_Door.Compare();
    //}

    public LODTwoRenderersList CompareModels()
    {
        Debug.LogError("CompareModels");

        twoList.Clear();
        #if UNITY_EDITOR
        EditorHelper.UnpackPrefab(Model_Old, PrefabUnpackMode.OutermostRoot);
        EditorHelper.UnpackPrefab(Model_New, PrefabUnpackMode.OutermostRoot);
        #endif

        MeshHelper.RemoveNew(Model_Old);

        ModelRendersWaiting_Old.SetTargetList(ModelRendersWaiting_New, MaxCompareCount, compareMode);
        ModelRendersWaiting_Old.Compare();

        //DateTime start = DateTime.Now;
        ////var renderers_2 = MeshRendererInfo.InitRenderers(LODnRoot);//  LODnRoot.GetComponentsInChildren<MeshRenderer>(true);
        ////var renderers_2 = MeshRendererInfo.GetLodNs(Model_New, -1, 0); //  LODnRoot.GetComponentsInChildren<MeshRenderer>(true);
        ////var renderers_0 = MeshRendererInfo.GetLodNs(Model_Old, -1, 0);
        //var renderers_2 = ModelRendersWaiting_New;
        //var renderers_0 = ModelRendersWaiting_Old;
        ////LODRendererCount0 = renderers_0.Count;
        ////LODRendererCount1 = renderers_2.Length;

        //list_lod0 = new List<MeshRendererInfo>();
        //list_lod0.AddRange(renderers_0.GetRendererInfos0());
        ////renderers_0.ToList().ForEach(i => { ts.Add(i.transform); });

        //int maxCount = renderers_2.Count;
        //if(MaxCompareCount>0 && MaxCompareCount<maxCount)
        //{
        //    maxCount = MaxCompareCount;
        //}
        //for (int i = 0; i < maxCount; i++)
        //{
        //    MeshRendererInfo render_lod1 = renderers_2[i].renderer_lod0;
        //    if (render_lod1 == null) continue;
        //    ProgressArg p1 = new ProgressArg("Compare", i, maxCount, render_lod1.name);
        //    if(ProgressBarHelper.DisplayCancelableProgressBar(p1))
        //    {
        //        break;
        //    }

        //    MinDisTarget<MeshRendererInfo> min = GetMinDisTransform<MeshRendererInfo>(list_lod0, render_lod1.transform, compareMode,p1);
        //    float minDis = min.dis;
        //    MeshRendererInfo render_lod0 = min.target;

        //    //MeshFilter filter1 = render_lod1.GetComponent<MeshFilter>();
        //    //MeshFilter filter0 = render_lod0.GetComponent<MeshFilter>();
        //    int vertexCount0 = render_lod0.GetMinLODVertexCount();
        //    int vertexCount1 = render_lod1.GetMinLODVertexCount();
        //    LODTwoRenderers lODTwoRenderers = new LODTwoRenderers(render_lod0, render_lod1, minDis, min.meshDis, vertexCount0, vertexCount1);
        //    twoList.Add(lODTwoRenderers);
        //}
        //ProgressBarHelper.ClearProgressBar();

        //twoList.Sort((a, b) =>
        //{
        //    return b.dis.CompareTo(a.dis);
        //});

        LODTwoRenderersList.RemoveEmpty(Model_New.transform);
        LODTwoRenderersList.RemoveEmpty(Model_New.transform);
        LODTwoRenderersList.RemoveEmpty(Model_New.transform);

        //SetRenderersLODInfo();
        //Debug.LogError($"AppendLod3ToGroup count1:{renderers_2.Count} count0:{renderers_0.Count} time:{(DateTime.Now - start)}");

        return twoList;
    }



    /// <summary>
    /// ɾ��New����Ĳ���Old���ᷢ���仯��ģ��
    /// </summary>
    public void RemoveNewRepeatedModels()
    {
        //LOD��Ӧ��ģ��
        //�Ŷ�Ӧ��ģ��
        //ǽ�ڶ�Ӧ��ģ��
    }

    public void UpdateModel()
    {

        //2.�޸�
        //1.���� 4.�ֲ�
        //3.ɾ��
        //
    }

    public float MinDistance = 0.0002f;
}
