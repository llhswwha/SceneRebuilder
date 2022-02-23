using MeshJobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PipeFactory : SingletonBehaviour<PipeFactory>
{
    public GameObject Target;

    public List<Transform> PipeLines = new List<Transform>();

    public List<Transform> PipeElbows = new List<Transform>();

    public List<Transform> PipeTees = new List<Transform>();

    public List<Transform> PipeReducers = new List<Transform>();

    public List<Transform> PipeFlanges = new List<Transform>();

    public List<Transform> PipeWeldolets = new List<Transform>();

    public List<Transform> PipeWelds = new List<Transform>();

    public List<Transform> PipeOthers = new List<Transform>();

    public int TotalPipeOthersVertexCount = 0;

    public List<Transform> PipeWeldsNew = new List<Transform>();

    public List<Transform> BoxModels = new List<Transform>();

    public int TotalBoxModelsVertexCount = 0;

    private void ClearList()
    {
        PipeLines = new List<Transform>();

        PipeElbows = new List<Transform>();

        PipeTees = new List<Transform>();

        PipeReducers = new List<Transform>();

        PipeFlanges = new List<Transform>();

        PipeOthers = new List<Transform>();

        //PipeWelds = new List<Transform>();

        PipeWeldolets = new List<Transform>();

        BoxModels = new List<Transform>();

        TotalBoxModelsVertexCount = 0;

        TotalPipeOthersVertexCount = 0;
    }

    public void ClearDebugObjs()
    {
        //ClearList();
        if (Target == null)
        {
            Debug.LogError("ClearDebugObjs Target == null");
            return;
        }
        PipeModelBase[] pipeModels = Target.GetComponentsInChildren<PipeModelBase>(true);
        foreach (var pipe in pipeModels)
        {
            if (pipe == null) continue;
            //pipe.ClearChildren();
            pipe.ClearDebugInfoGos();
            GameObject.DestroyImmediate(pipe);
        }

        TransformHelper.ClearComponentGos<PipeMeshGeneratorBase>(Target);

        TransformHelper.ClearComponents<OBBCollider>(Target);

        TransformHelper.ClearComponentGos<DebugInfoRoot>(Target);
    }

    public void ClearGeneratedObjs()
    {
        if (newBuilder != null)
        {
            newBuilder.ClearGeneratedObjs();
        }

        TransformHelper.ClearComponentGos<DebugInfoRoot>(Target);
    }


    private void GetTargetInfoBefore()
    {
        MeshNode meshNode = MeshNode.InitNodes(Target);
        meshNode.GetSharedMeshList();
        TargetInfo = meshNode.GetVertexInfo();
        TargetVertexCount = meshNode.VertexCount;
    }

    public void GetResultInfoAfter()
    {
        //ResultInfo=ShowTargetInfo(Target);

        MeshNode meshNode2 = MeshNode.InitNodes(Target);
        SharedMeshInfoList sInfo = meshNode2.GetSharedMeshList();
        ResultInfo = meshNode2.GetVertexInfo();
        ResultVertexCount = meshNode2.VertexCount;
        SharedResultVertexCountCount = sInfo.sharedVertexCount;
    }

    private int OneKey_GetPipeInfos(bool isJob)
    {
        Target.SetActive(true);

        GetTargetInfoBefore();

        //this.ClearGeneratedObjs();
        this.GetPipeParts();

        int weldsCount = this.PipeWelds.Count;

        if (isJob)
        {
            this.GetPipeInfosJob();
        }
        else
        {
            this.GetPipeInfos();
        }

        return weldsCount;
    }



    private string AfterGeneratePipes(bool isMoveToBuilder,bool isPrefabGos)
    {
        if (isMoveToBuilder)
            this.MovePipes();
        string pefabLog = "";
        if (isPrefabGos)
        {
            pefabLog = GetPrefabInfoList();
        }
        else
        {
            this.CombineGeneratedWelds();
        }
        return pefabLog;
    }

    public void OneKey(bool isJob)
    {
        //DateTime start = DateTime.Now;
        //int weldsCount = OneKey_GetPipeInfos(isJob);
        //OneKeyGeneratePipes(isJob, false);//not removeMesh
        //string pefabLog = AfterGeneratePipes(true);
        //int lastWeldCount = ReplaceOldPipeMesh(true);
        //GetResultInfoAfter();
        //Debug.LogError($"OneKey target:{Target.name} arg:({generateArg}) time:{DateTime.Now - start} Models:{newBuilder.PipeModels.Count + PipeOthers.Count + weldsCount}={newBuilder.PipeModels.Count}+{PipeOthers.Count}+{weldsCount}({lastWeldCount})) {pefabLog} TargetInfo:{TargetInfo} -> ResultInfo:{ResultInfo} ({ResultVertexCount / TargetVertexCount:P2},{SharedResultVertexCountCount / TargetVertexCount:P2})");

        OneKeyCore(isJob, false, true);
    }

    public void OneKeyEx(bool isJob)
    {
        //DateTime start = DateTime.Now;
        //int weldsCount = OneKey_GetPipeInfos(isJob);
        //OneKeyGeneratePipes(isJob, true);//removeMesh
        //string pefabLog = AfterGeneratePipes(false);//not move
        //int lastWeldCount = ReplaceOldPipeMesh(false);
        //GetResultInfoAfter();
        //Debug.LogError($"OneKey target:{Target.name} arg:({generateArg}) time:{DateTime.Now - start} Models:{newBuilder.PipeModels.Count + PipeOthers.Count + weldsCount}={newBuilder.PipeModels.Count}+{PipeOthers.Count}+{weldsCount}({lastWeldCount})) {pefabLog} TargetInfo:{TargetInfo} -> ResultInfo:{ResultInfo} ({ResultVertexCount / TargetVertexCount:P2},{SharedResultVertexCountCount / TargetVertexCount:P2})");
        OneKeyCore(isJob, true, false);
    }

    public void DebugOneKey1()
    {
        DateTime start = DateTime.Now;

        EditorHelper.UnpackPrefab(Target);

        DateTime start1 = DateTime.Now;
        int weldsCount = OneKey_GetPipeInfos(true);
        TimeSpan getInfoTime = DateTime.Now - start1;

       
    }

    public void DebugOneKey2(bool isRemoveMesh)
    {
        DateTime start2 = DateTime.Now;
        OneKeyGeneratePipes(true, isRemoveMesh);//removeMesh
        TimeSpan generateTime = DateTime.Now - start2;
    }

    public void DebugOneKey3()
    {
        DateTime start3 = DateTime.Now;
        string pefabLog = AfterGeneratePipes(false, IsPrefabGos);//not move
        int lastWeldCount = ReplaceOldPipeMesh(false);
        TimeSpan prefabTime = DateTime.Now - start3;

        GetResultInfoAfter();
    }

    public void OneKeyCore(bool isJob, bool isRemoveMesh, bool isMoveToBuilder)
    {
        DateTime start = DateTime.Now;

        EditorHelper.UnpackPrefab(Target);

        DateTime start1 = DateTime.Now;
        int weldsCount = OneKey_GetPipeInfos(isJob);
        TimeSpan getInfoTime = DateTime.Now - start1;

        DateTime start2 = DateTime.Now;
        OneKeyGeneratePipes(isJob, isRemoveMesh);//removeMesh
        TimeSpan generateTime = DateTime.Now - start2;

        DateTime start3 = DateTime.Now;
        string pefabLog = AfterGeneratePipes(isMoveToBuilder, IsPrefabGos);//not move
        int lastWeldCount = ReplaceOldPipeMesh(false);
        TimeSpan prefabTime = DateTime.Now - start3;

        GetResultInfoAfter();
        Debug.LogError($"OneKey target:{Target.name} time:{(DateTime.Now - start).ToString(timeFormat)}({getInfoTime.ToString(timeFormat)}+{generateTime.ToString(timeFormat)}+{prefabTime.ToString(timeFormat)}) arg:({generateArg}) Models:{newBuilder.PipeModels.Count + PipeOthers.Count + weldsCount}={newBuilder.PipeModels.Count}+{PipeOthers.Count}+{weldsCount}({lastWeldCount})) {pefabLog} TargetInfo:{TargetInfo} -> ResultInfo:{ResultInfo} ({ResultVertexCount / TargetVertexCount:P2},{SharedResultVertexCountCount / TargetVertexCount:P2})");
    }

    public GameObject PrefabRoots = null;

    public bool IsCreatePipeByUnityPrefab = true;

    public GameObject PipeModelUnitPrefab_Line = null;

    public Mesh PipeModelUnitPrefabMesh_Line = null;

    public GameObject CreatePipeLineUnitPrefab(bool isEndCaps,int minSegs,string tag)
    {
        if (PrefabRoots == null)
        {
            PrefabRoots = new GameObject("PrefabRoots");
            PrefabRoots.transform.SetParent(this.transform);
        }

        GameObject go = new GameObject("PipePrefab_"+ tag);
        go.transform.SetParent(PrefabRoots.transform);
        PipeMeshGenerator pipe = go.AddMissingComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            Debug.LogError($"CreatePipeLineUnitPrefab.RendererModel pipe == null model:{this.name}");
            return null;
        }
        generateArg.SetArg(pipe);

        pipe.points = new List<Vector3>() { new Vector3(-0.5f, 0, 0), new Vector3(0.5f, 0, 0) };
        generateArg.SetArg(pipe);
        var radius = 0.5f;
        pipe.pipeRadius = radius;
        pipe.pipeRadius1 = radius;
        pipe.pipeRadius2 = radius;
        pipe.generateWeld = false;
        pipe.IsGenerateEndWeld = true;
        pipe.generateEndCaps = isEndCaps;
        if (minSegs>0 && pipe.pipeSegments < minSegs)
        {
            pipe.pipeSegments = minSegs;
        }
        //if (radius < 0.01)
        //{
        //    //pipe.weldRadius = 0.003f;
        //    pipe.weldPipeRadius = arg.weldRadius * 0.6f;
        //}
        pipe.RenderPipe();
        return pipe.gameObject;
    }

    public GameObject GetPipeModelUnitPrefab_Line()
    {
        if (PipeModelUnitPrefab_Line == null)
        {
            PipeModelUnitPrefab_Line = CreatePipeLineUnitPrefab(false,0,"Line");
            PipeModelUnitPrefab_Line.SetActive(false);
        }
        return PipeModelUnitPrefab_Line;
    }

    public Mesh GetPipeModelUnitPrefabMesh_Line()
    {
        if (PipeModelUnitPrefabMesh_Line == null)
        {
            PipeModelUnitPrefab_Line = CreatePipeLineUnitPrefab(false, 0, "Line");
            PipeModelUnitPrefab_Line.SetActive(false);
            PipeModelUnitPrefabMesh_Line = PipeModelUnitPrefab_Line.GetComponent<MeshFilter>().sharedMesh;
        }
        return PipeModelUnitPrefabMesh_Line;
    }

    public GameObject PipeModelUnitPrefab_Flange = null;

    public Mesh PipeModelUnitPrefabMesh_Flange = null;

    public GameObject GetPipeModelUnitPrefab_Flange()
    {
        if (PipeModelUnitPrefab_Flange == null)
        {
            PipeModelUnitPrefab_Flange = CreatePipeLineUnitPrefab(true,32, "Flange");
            PipeModelUnitPrefab_Flange.SetActive(false);
        }
        return PipeModelUnitPrefab_Flange;
    }

    public Mesh GetPipeModelUnitPrefabMesh_Flange()
    {
        if (PipeModelUnitPrefabMesh_Flange == null)
        {
            PipeModelUnitPrefab_Flange = CreatePipeLineUnitPrefab(true, 32, "Flange");
            PipeModelUnitPrefab_Flange.SetActive(false);
            PipeModelUnitPrefabMesh_Flange = PipeModelUnitPrefab_Flange.GetComponent<MeshFilter>().sharedMesh;
        }
        return PipeModelUnitPrefabMesh_Flange;
    }

    public void ClearWeldPrefabs()
    {
        PipeModelUnitPrefab_Welds_Datas.Clear();
        PipeModelUnitPrefab_Welds.Clear();
        PipeModelUnitPrefabMesh_Welds.Clear();
    }

    public List<PipeWeldData> PipeModelUnitPrefab_Welds_Datas = new List<PipeWeldData>();

    public List<GameObject> PipeModelUnitPrefab_Welds = new List<GameObject>();

    public List<Mesh> PipeModelUnitPrefabMesh_Welds = new List<Mesh>();

    public GameObject CreatePipeModelUnitPrefab_Weld(PipeWeldData data,string weldName)
    {
        //string weldName = $"WeldPrefab_{data.elbowRadius}_{data.pipeRadius}";
        GameObject go = new GameObject(weldName);
        go.transform.position = Vector3.zero;
        go.transform.localScale = new Vector3(1, 2, 1);
        //CreateWeldGo(go, WeldData.start, WeldData.direction);
        PipeMeshGenerator weldGenerator = go.AddComponent<PipeMeshGenerator>();
        PipeWeldModel.SetPipeMeshGenerator(weldGenerator, generateArg, data);
        weldGenerator.RenderTorusXZ();

        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        renderer.shadowCastingMode = ShadowCastingMode.Off;

        if (PrefabRoots == null)
        {
            PrefabRoots = new GameObject("PrefabRoots");
            PrefabRoots.transform.SetParent(this.transform);
        }

        go.transform.SetParent(PrefabRoots.transform);
        go.SetActive(false);
        return go;
    }

    public int WeldRadiusP = 4;

    public static float WeldPrefabArgPipeRadiusDistance = 0.0005f;

    public static float WeldPrefabArgElbowRadiusDistance = 0.0005f;

    public GameObject GetPipeModelUnitPrefab_Weld(PipeWeldData data)
    {
        string weldName = data.GetPrefabName(WeldRadiusP);
        GameObject result = null;
        foreach (var weld in PipeModelUnitPrefab_Welds)
        {
            if (weld.name == weldName)
            {
                result = weld; break;
            }
        }
        if (result == null)
        {
            int id = -1;
            for (int i = 0; i < PipeModelUnitPrefab_Welds_Datas.Count; i++)
            {
                var prefabData = PipeModelUnitPrefab_Welds_Datas[i];
                var disPipe = Math.Abs(data.pipeRadius - prefabData.pipeRadius);
                if(disPipe> WeldPrefabArgPipeRadiusDistance)
                {
                    continue;
                }
                var disElbow = Math.Abs(data.elbowRadius - prefabData.elbowRadius);

                if(disElbow < WeldPrefabArgElbowRadiusDistance)
                {
                    //Debug.Log($"GetPipeModelUnitPrefab_Weld[{weldName}] [elbowRadius1:{data.elbowRadius} elbowRadius2:{prefabData.elbowRadius} disElbow:{disElbow}]  [pipeRadius1:{data.pipeRadius} pipeRadius2:{prefabData.pipeRadius} disPipe:{disPipe}] WeldPrefabArgElbowRadiusDistance£º{WeldPrefabArgElbowRadiusDistance}");
                    id = i;
                    break;
                }
                else if (disElbow < WeldPrefabArgElbowRadiusDistance * 5)
                {
                    Debug.LogWarning($"GetPipeModelUnitPrefab_Weld[{weldName}] [elbowRadius1:{data.elbowRadius} elbowRadius2:{prefabData.elbowRadius} disElbow:{disElbow}]  [pipeRadius1:{data.pipeRadius} pipeRadius2:{prefabData.pipeRadius} disPipe:{disPipe}] WeldPrefabArgElbowRadiusDistance:{WeldPrefabArgElbowRadiusDistance}");
                }
            }

            if (id == -1)
            {
                result = CreatePipeModelUnitPrefab_Weld(data, weldName);
                PipeModelUnitPrefab_Welds_Datas.Add(data);
                PipeModelUnitPrefab_Welds.Add(result);
                PipeModelUnitPrefabMesh_Welds.Add(result.GetComponent<MeshFilter>().sharedMesh);
            }
            else
            {
                result = PipeModelUnitPrefab_Welds[id];
            }
        }
        return result;
    }

    public Mesh GetPipeModelUnitPrefabMesh_Weld(PipeWeldData data)
    {
        string weldName = data.GetPrefabName(WeldRadiusP);
        Mesh result = null;
        foreach (var weld in PipeModelUnitPrefabMesh_Welds)
        {
            if (weld.name == weldName)
            {
                result = weld; break;
            }
        }
        if (result == null)
        {
            var go = GetPipeModelUnitPrefab_Weld(data);
            result = go.GetComponent<MeshFilter>().sharedMesh;
        }

        //var go = GetPipeModelUnitPrefab_Weld(data);
        //var result = go.GetComponent<MeshFilter>().sharedMesh;

        //GameObject dataGo = new GameObject($"{data.elbowRadius}_{data.pipeRadius}");
        //dataGo.transform.SetParent(go.transform);

        return result;
    }

    public void OneKey_Generate(bool isJob)
    {
        int weldsCount = OneKey_GetPipeInfos(isJob);
        OneKeyGeneratePipes(isJob, false);//removeMesh
        string pefabLog = AfterGeneratePipes(true,false);//not move
    }

    private void OneKeyGeneratePipes(bool isJob,bool isRemoveMesh)
    {
        this.ClearWeldPrefabs();
        //this.ClearGeneratedObjs();
        if(isRemoveMesh)
            this.RemoveMeshes();//++
        this.RendererEachPipes();
        OneKey_CheckResults(isJob);
    }

    private void OneKey_CheckResults(bool isJob)
    {
        if (IsCheckResult)
        {
            if (isJob)
            {
                this.CheckResultsJob();
            }
            else
            {
                this.CheckResults();
            }
        }
    }

    public List<Transform> lastWeldList = new List<Transform>();

    private int ReplaceOldPipeMesh(bool isReplacePipes)
    {
        int lastWeldCount = 0;
        if (IsReplaceOld)
        {
            if(isReplacePipes)
                this.ReplacePipes();
            lastWeldList = this.ReplaceWelds();
            lastWeldCount = lastWeldList.Count;
            if (lastWeldCount > 100)
            {
                Debug.LogError("ReplaceOldPipeMesh lastWeldCount > 100");
            }
            else if (lastWeldCount > 0 && IsPrefabOldWeld)
            {
                PrefabInstanceBuilder.Instance.GetPrefabsOfList(lastWeldList, true, "Welds(Old)",IsOthersTryRT);
            }
        }
        return lastWeldCount;
    }

    private string GetPrefabInfoList()
    {
        AllPrefabs = new PrefabInfoList();

        DateTime start1 = DateTime.Now;
        PrefabInfoList pres1 = this.PrefabPipes();
        AllPrefabs.AddRange(pres1);
        TimeSpan t1 = DateTime.Now - start1;

        DateTime start2 = DateTime.Now;
        PrefabInfoList pres2 = new PrefabInfoList();
        if (IsPrefabOthers)
        {
            pres2 = this.PrefabOthers();
            AllPrefabs.AddRange(pres2);
        }
        TimeSpan t2 = DateTime.Now - start2;

        this.CombineGeneratedWelds();

        DateTime start3 = DateTime.Now;
        PrefabInfoList pres3 = this.PrefabWelds();
        AllPrefabs.AddRange(pres3);
        TimeSpan t3 = DateTime.Now - start3;
        
        return $"PrefabInfoList Prefabs:{pres1.Count + pres2.Count + pres3.Count}({pres1.Count}+{pres2.Count}+{pres3.Count}) Time:{t1.ToString(timeFormat)}+{t2.ToString(timeFormat)}+{t3.ToString(timeFormat)}";
    }

    public PrefabInfoList AllPrefabs = new PrefabInfoList();

    //[ContextMenu("ClearResult")]
    //public void ClearResult()
    //{
    //    ClearGeneratedObjs();
    //}

    [ContextMenu("GetPipeParts")]
    public void GetPipeParts()
    {
        //ClearDebugObjs();

        GetModelClass();

        ShowAll();

        InitPipeBuilder();
    }

    public void ShowPrefabs()
    {
        //GameObject allPrefabs = new GameObject("AllPrefabs");
        //foreach(PrefabInfo pres in AllPrefabs)
        //{
        //    GameObject go = GameObject.Instantiate(pres.Prefab);
        //    TransformHelper.ClearChildren(go);
        //    //EditorHelper.RemoveAllComponents(go);
        //    go.transform.SetParent(allPrefabs.transform);
        //    go.transform.position = Vector3.zero;
        //}

        ShowPrefabs(AllPrefabs, "AllPrefabs",false);
    }

    public void AlignDirectionPrefabs()
    {
        //OBBCollider.AlignDirectionList(PrefabClones);
        foreach(var pref in PrefabClones)
        {
            PipeMeshGeneratorBase generator = pref.GetComponent<PipeMeshGenerator>();
            if (generator == null) continue;
            generator.AlignDirection();
        }
    }

    public bool IsPipeTryRT = true;

    public bool IsOthersTryRT = false;

    public List<GameObject> PrefabClones = new List<GameObject>();

    public void ShowPrefabs(PrefabInfoList list,string parent,bool isAlignDir)
    {
        foreach(var item in PrefabClones)
        {
            GameObject.DestroyImmediate(item);
        }
        PrefabClones.Clear();

        GameObject allPrefabs = new GameObject(parent);
        for (int i = 0; i < list.Count; i++)
        {
            PrefabInfo pres = list[i]; 
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("ShowPrefabs", i, list.Count, pres)))
            {
                break;
            }
            if (pres == null) continue;
            if (pres.Prefab == null) continue;
            GameObject go = GameObject.Instantiate(pres.Prefab);
            MeshHelper.ClearChildren(go.transform);
            PrefabInfoComponent prefabC=go.AddComponent<PrefabInfoComponent>();
            prefabC.prefabInfo = pres;

            go.name += $"{pres.InstanceCount}";
            TransformHelper.ClearChildren(go);
            //EditorHelper.RemoveAllComponents(go);
           

            //if (isAlignDir)
            //{
            //    //OBBCollider obb = go.AddComponent<OBBCollider>();
            //    //obb.GetObb(true);
            //    //obb.AlignDirection();
            //    PipeMeshGeneratorBase generator = go.GetComponent<PipeMeshGenerator>();
            //    if (generator != null)
            //        generator.AlignDirection();
            //}

            PipeMeshGeneratorBase generator = pres.Prefab.GetComponent<PipeMeshGenerator>();
            if (generator != null)
            {
                GameObject go2 = GameObject.Instantiate(generator.Target);
                MeshHelper.ClearChildren(go2.transform);
                go2.transform.SetParent(go.transform);
                go2.transform.localPosition = Vector3.zero;
                go2.SetActive(true);

                PipeModelBase model = generator.Target.GetComponent<PipeModelBase>();
                go.name = model.GetSortKey() + go.name;
            }

            if (pres.InstanceCount > 0)
            {
                var ins = pres.Instances[0];
                if (ins == null)
                {
                    Debug.LogError($"ins==null prefab:{pres.GetTitle()}");
                }
                else
                {
                    GameObject go3 = GameObject.Instantiate(ins);
                    if (go3 == null)
                    {
                        Debug.LogError($"go3==null prefab:{pres.GetTitle()}");
                    }
                    else
                    {
                        go3.transform.SetParent(go.transform);
                        MeshHelper.ClearChildren(go3.transform);
                        go3.transform.localPosition = Vector3.zero;
                        go3.transform.localRotation = Quaternion.identity;
                        go3.SetActive(true);
                    }
                    
                }
                
            }

            go.transform.SetParent(allPrefabs.transform);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            

            PrefabClones.Add(go);
        }
        ProgressBarHelper.ClearProgressBar();
    }

    public void AddList(List<Transform> list,List<Transform> newList)
    {
        foreach (var item in newList)
        {
            if (item.GetComponent<MeshRenderer>() == null) continue;
            list.Add(item);
        }
    }

    public bool isUniformRaidus = false;

    public static bool IsElbow(string key)
    {
        return key.Contains("Degree_Direction_Change") || key.StartsWith("ELBOW") || key.StartsWith("BEND");
    }

    public static bool IsTee(string key)
    {
        return key.Contains("Tee") || key.StartsWith("TEE");
    }

    public static bool IsFlange(string key)
    {
        return key.Contains("Flange") || key.StartsWith("Ô²Öù") || key.StartsWith("CYLINDER") || key.StartsWith("FLANGE") || key.StartsWith("OLET");
    }

    public static bool IsReducer(string key)
    {
        return key.Contains("Reducer") || key.StartsWith("REDUCER") || key.StartsWith("CONE");
    }

    public static bool IsPipe(string key)
    {
        return key.StartsWith("Pipe");
    }

    public static bool? IsSmall(string key)
    {
        if (IsPipe(key)) return false;
        if (IsElbow(key)) return false;
        if (IsTee(key)) return false;
        if (IsFlange(key)) return false;
        if (IsReducer(key)) return false;
        if (IsWeldolet(key)) return true;
        return null;
    }

    public static bool IsWeldolet(string key)
    {
        return key.Contains("Weldolet");
    }

    public void GetModelClass()
    {
        if (Target == null)
        {
            Debug.LogError("GetModelClass Target == null");
            return;
        }
        ClearList();
        PipeWelds = GetWelds();

        if(newBuilder)
            PipeWeldsNew = newBuilder.GetNewWelds(Target);

        Dictionary<Transform, Transform> weldsDict = new Dictionary<Transform, Transform>();
        foreach(var weld in PipeWelds)
        {
            if (weld == null) continue;
            weldsDict.Add(weld, weld);
        }

        ModelClassDict<Transform> modelClassList = ModelMeshManager.Instance.GetPrefixNamesNoLod(Target);
        var keys = modelClassList.GetKeys();

        int maxVertexCount = PrefabInstanceBuilder.Instance.MaxVertexCount;
        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];
            ProgressArg p1 = new ProgressArg("GetModelClass", i, keys.Count, key);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                break;
            }

            var list = modelClassList.GetList(key);
            if (IsPipe(key)) 
            {
                AddList(PipeLines, list);
            }
            else if (IsElbow(key))
            {
                AddList(PipeElbows, list);
            }
            else if (IsTee(key))
            {
                AddList(PipeTees, list);
            }
            else if (IsWeldolet(key))
            {
                AddList(PipeWeldolets, list);
            }
            else if (IsReducer(key))//REDUCER CONE
            {
                AddList(PipeReducers, list);
            }
            else if (IsFlange(key))//FLANGE
            {
                AddList(PipeFlanges, list);
            }
            //ATTACHMENT
            //NOZZLE
            else
            {
                //PipeOthers.AddRange(list);
                //AddList(PipeOthers, list);
                bool isBreak = false;
                for (int i1 = 0; i1 < list.Count; i1++)
                {
                    Transform item = list[i1];
                    ProgressArg p2 = new ProgressArg("SubItems", i1, list.Count, item);
                    p1.AddSubProgress(p2);
                    if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
                    {
                        isBreak = true;
                        break;
                    }
                    if (weldsDict.ContainsKey(item)) continue;
                    MeshFilter mf = item.GetComponent<MeshFilter>();
                    if (mf == null) continue;
                    //Debug.LogError($"Other:{item.name} mf:{mf.name} mesh:{mf.sharedMesh.name} maxVertexCount:{maxVertexCount} vertexCount:{mf.sharedMesh.vertexCount}");

                    if (item.GetComponent<MeshRenderer>() == null) continue;
                    PipeWeldModel weldModel = item.GetComponent<PipeWeldModel>();
                    if (weldModel != null) continue;
                    int vertexCount = mf.sharedMesh.vertexCount;
                    if (vertexCount > maxVertexCount) continue;

                    //if (mf.name.Contains("_Combined_")) continue;
                    //if (mf.sharedMesh.name.Contains("_Combined_")) continue;

                    //Debug.LogError($"Other:{item.name} mf:{mf.name} mesh:{mf.sharedMesh.name} maxVertexCount:{maxVertexCount} vertexCount:{mf.sharedMesh.vertexCount}");

                    if (vertexCount == 24)//Box 24VertexCount
                    {
                        BoxModels.Add(item);
                        
                        TotalBoxModelsVertexCount += vertexCount;
                    }
                    else
                    {
                        PipeOthers.Add(item);
                        TotalPipeOthersVertexCount += vertexCount;
                    }

                    //PipeOthers.Add(item);
                }
                if (isBreak)
                {
                    break;
                }
            }
        }

        //MeshRendererInfoList list = new MeshRendererInfoList(PipeOthers);
        var mps = MeshPoints.GetMeshPoints(PipeOthers);
        MeshFilterListDict dict = new MeshFilterListDict(mps.ToArray(), 0);
        //dict.pr

        InitArgMat();

        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetModelClass keys:{keys.Count} maxVertexCount:{maxVertexCount} welds:{PipeWelds.Count}");
    }

    private void InitArgMat()
    {

        if (generateArg.pipeMaterial == null)
        {
            if (PipeLines.Count > 0)
            {
                generateArg.pipeMaterial = PipeLines[0].GetComponent<MeshRenderer>().sharedMaterial;
            }
            else
            {
                Debug.LogError("InitArgMat PipeLines.Count == 0");
            }
        }


        if (generateArg.weldMaterial == null)
        {
            if (PipeWelds.Count > 0)
            {
                generateArg.weldMaterial = PipeWelds[0].GetComponent<MeshRenderer>().sharedMaterial;
            }
            else
            {
                Debug.LogError("InitArgMat PipeWelds.Count == 0");
            }
        }
    }

    public void ResetGeneratorsMesh()
    {
        DateTime start = DateTime.Now;
        var gs = GetPipeMeshGenerators();
        for (int i = 0; i < gs.Count; i++)
        {
            PipeMeshGeneratorBase g = gs[i];
            g.RenderPipe();
            if (ProgressBarHelper.DisplayCancelableProgressBar("Reset", i + 1, gs.Count))
            {
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"ResetGeneratorsMesh gs:{gs.Count} time:{DateTime.Now-start}");
    }

    public void ClearGeneratorsMesh()
    {
        DateTime start = DateTime.Now;
        var gs = GetPipeMeshGenerators();
        for (int i = 0; i < gs.Count; i++)
        {
            PipeMeshGeneratorBase g = gs[i];
            EditorHelper.RemoveAllComponents(g.gameObject, typeof(PipeMeshGeneratorBase));
            if (ProgressBarHelper.DisplayCancelableProgressBar("Clear", i + 1, gs.Count))
            {
                break;
            }
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"ClearGeneratorsMesh gs:{gs.Count} time:{DateTime.Now - start}");
    }

    public List<PipeMeshGeneratorBase> GetPipeMeshGenerators()
    {
        List<PipeMeshGeneratorBase> gs = new List<PipeMeshGeneratorBase>();
        if (Target)
            gs.AddRange(Target.GetComponentsInChildren<PipeMeshGeneratorBase>(true));
        if (newBuilder)
            gs.AddRange(newBuilder.GetComponentsInChildren<PipeMeshGeneratorBase>(true));
        return gs;
    }

    public float ResultVertexCount = 0;

    public float SharedResultVertexCountCount = 0;

    public string ResultInfo = "";

    public float TargetVertexCount = 0;

    public string TargetInfo = "";

    public string GetResultInfo()
    {
        return ResultInfo;
    }

    [ContextMenu("ShowAll")]
    public void ShowAll()
    {
        SetAllVisible(true);
    }

    public GameObject WeldRootTarget;

    private void Replace(Transform w,Transform w2)
    {
        w.name = w2.name;
        //w.transform.SetParent(w2.parent);
        EditorHelper.UnpackPrefab(w2.gameObject);
        w2.gameObject.SetActive(false);
        GameObject.DestroyImmediate(w2.gameObject);
    }

    public float minWeldDis = 0.0001f;
    public float maxWeldDis = 0.05f;



    public List<Transform> ReplaceWelds()
    {
        DateTime start = DateTime.Now;
        //if (WeldRootTarget == null)
        //{
        //    WeldRootTarget = Target;
        //}
        //var welds = WeldRootTarget.GetComponentsInChildren<MeshRenderer>(true);
        //int allWeldsCount = welds.Length;
        //List<Transform> weldList = new List<Transform>();
        //foreach(var w in welds)
        //{
        //    weldList.Add(w.transform);
        //}
        //List<Transform> weldList = this.GetWelds();
        PipeWelds = GetWelds();
        List<Transform> weldList = new List<Transform>(PipeWelds);
        int allWeldsCount = weldList.Count;
        if (allWeldsCount == 0) return weldList;
        var weldsNew = newBuilder.GetNewWelds(Target);
        int newWeldsCount = weldsNew.Count;

        for(int i=0;i<weldsNew.Count;i++)
        {
            var w = weldsNew[i];
            if (w == null) continue;
            var closedW = TransformHelper.FindClosedComponentEx(weldList, w, false);
            var w2 = closedW.t;
            if (closedW.dis < minWeldDis)
            {
                //Debug.Log($"ReplaceWelds1 closedW[{i}] [w:{w.parent.name}/{w.name}] [closedW:{closedW}]");
                Replace(w, w2);
                weldList.Remove(w2);
                weldsNew.RemoveAt(i);i--;
            }
            else if (closedW.dis < maxWeldDis)
            {
                //Debug.Log($"ReplaceWelds1 closedW[{i}] [w:{w.parent.name}/{w.name}] [closedW:{closedW}]");
            }
            else
            {
                //Debug.LogError($"ReplaceWelds1 closedW[{i}] [w:{w.parent.name}/{w.name}] [closedW:{closedW}]");
            }
        }

        List<Mesh> DestroyiedMeshs = new List<Mesh>();
        int destroyCount = 0;
        for (int i = 0; i < weldsNew.Count; i++)
        {
            var w = weldsNew[i];
            if (w == null) continue;
            var closedW = TransformHelper.FindClosedComponentEx(weldList, w, false);
            var w2 = closedW.t;
            if (closedW.dis < minWeldDis)
            {
                //Debug.Log($"ReplaceWelds2 closedW[{i}] [w:{w.parent.name}/{w.name}] [closedW:{closedW}]");
                //weldList.Remove(closedW.t);
            }
            else if (closedW.dis < maxWeldDis)
            {
                //Debug.LogWarning($"ReplaceWelds2 closedW[{i}] [w:{w.parent.name}/{w.name}] [closedW:{closedW}]");
                w.position = closedW.t.position;
                Replace(w, w2);

                weldList.Remove(closedW.t);
                weldsNew.RemoveAt(i); i--;
            }
            else
            {
                MeshPrefabInstance ins = w.GetComponent<MeshPrefabInstance>();
                if (ins!=null && ins.IsPrefab)
                {
                    MeshFilter mf = w.GetComponent<MeshFilter>();
                    DestroyiedMeshs.Add(mf.sharedMesh);
                }
                //Debug.LogWarning($"ReplaceWelds2 closedW[{i}] [w:{w.parent.name}/{w.name}] [closedW:{closedW}]");
                GameObject.DestroyImmediate(w.gameObject);
                //PrefabInfoManager.Instance.DestroyPrefab(w.gameObject);
                weldsNew.RemoveAt(i); i--;

                destroyCount++;
            }
        }

        if (DestroyiedMeshs.Count > 0)
        {
            Debug.LogError($"ReplaceWelds DestroyiedMeshs:{DestroyiedMeshs.Count}");
        }

        
        if (weldList.Count > 0)
        {
            Debug.LogError($"ReplaceWelds time:{DateTime.Now - start} AllWelds:{allWeldsCount} NewWelds:{newWeldsCount} LastWelds:{weldList.Count} weld:{weldList[0]} destroyCount:{destroyCount}");
        }
        else
        {
            Debug.Log($"ReplaceWelds time:{DateTime.Now - start} AllWelds:{allWeldsCount} NewWelds:{newWeldsCount} LastWelds:{weldList.Count} destroyCount:{destroyCount}");
        }
        UpdataPrefabInstanceInfo();

        //var instances = Target.GetComponentsInChildren<MeshPrefabInstance>(true);
        //foreach(var ins in instances)
        //{

        //}
        SharedMeshInfoList sharedMeshInfos1 = new SharedMeshInfoList(Target,true);
        //Debug.LogError($"sharedMeshInfos1 :{sharedMeshInfos1.Count}");
        foreach(var mesh in DestroyiedMeshs)
        {
            SharedMeshInfo info = sharedMeshInfos1.FindByMesh(mesh);
            if (info == null)
            {
                //Debug.LogError($"sharedMeshInfos1 info==null mesh:{mesh}");
                continue;
            }
            info.AddInstanceInfo();
        }

        SharedMeshInfoList sharedMeshInfos2 = new SharedMeshInfoList(newBuilder.gameObject, true);
        //Debug.LogError($"sharedMeshInfos2 :{sharedMeshInfos2.Count}");
        foreach (var mesh in DestroyiedMeshs)
        {
            SharedMeshInfo info = sharedMeshInfos2.FindByMesh(mesh);
            if (info == null)
            {
                //Debug.LogError($"sharedMeshInfos2 info==null mesh:{mesh}");
                continue;
            }
            info.AddInstanceInfo();
        }

        return weldList;
    }

    public void UpdataPrefabInstanceInfo()
    {

    }

    public void SetAllVisible(bool isVisible)
    {
        SetListVisible(PipeLines, isVisible);
        SetListVisible(PipeElbows, isVisible);
        SetListVisible(PipeReducers, isVisible);
        SetListVisible(PipeFlanges, isVisible);
        SetListVisible(PipeTees, isVisible);
        SetListVisible(PipeOthers, isVisible);
        SetListVisible(PipeWeldolets, isVisible);
    }

    public void SetListVisible(List<Transform> renderers,bool isVisible)
    {
        foreach(var item in renderers)
        {
            if (item == null) continue;
            item.gameObject.SetActive(isVisible);
        }
    }

    public Transform OthersRoot = null;

    public void MoveOthersParent()
    {
        OthersRoot = RendererId.MoveTargetsParent(PipeOthers, OthersRoot, "PipeOther");
        OthersRoot.transform.SetParent(this.transform);
    }

    public void DestroyOthers()
    {
        EditorHelper.UnpackPrefab(Target);
        foreach(var item in PipeOthers)
        {
            EditorHelper.UnpackPrefab(item.gameObject);
            GameObject.DestroyImmediate(item.gameObject);
        }
    }

    public void RecoverOthersParent()
    {
        RendererId.RecoverTargetsParent(PipeOthers, null);
    }

    public List<Transform> GetWelds()
    {
        //var welds = WeldRootTarget.GetComponentsInChildren<MeshRenderer>(true);
        //int allWeldsCount = welds.Length;
        //List<Transform> weldList = new List<Transform>();
        //foreach (var w in welds)
        //{
        //    weldList.Add(w.transform);
        //}
        //return weldList;

        //Dictionary<GameObject, Transform> parentDict = new Dictionary<GameObject, Transform>();

        var weldRoots = TransformHelper.FindGameObjects(Target.transform, "Welding");
        List<Transform> weldList = new List<Transform>();
        for (int i = 0; i < weldRoots.Count; i++)
        {
            GameObject root = weldRoots[i];
            Transform wp = root.transform.parent;
            if (!root.name.Contains("_"))
            {

                if (wp.name == "In" || wp.name == "Out0" || wp.name == "Out1")
                {
                    root.name = wp.transform.parent.name + "_" + wp.name + "_" + root.name;
                }
                else
                {
                    root.name = wp.name + "_" + root.name;
                }
            }

            //EditorHelper.UnpackPrefab(w);
            //parentDict.Add(w, wp);
            //w.transform.SetParent(null);
            //w.transform.SetParent(WeldRootTarget.transform);

            var welds = root.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var w in welds)
            {
                weldList.Add(w.transform);
            }

            Debug.Log($"GetWelds[{i}] root:{root} welds:{welds.Length}");
        }

        Debug.Log($"GetWelds End weldList:{weldList.Count}");
        return weldList;
    }

    private Dictionary<GameObject, Transform> MoveWeldParent()
    {
        var welds = newBuilder.GetNewWelds(Target);
        Dictionary<GameObject, Transform> parentDict = new Dictionary<GameObject, Transform>();
        if (WeldRootTarget == null)
        {
            WeldRootTarget = new GameObject("WeldRootTarget");
        }
        foreach (var w in welds)
        {
            Transform wp = w.transform.parent;
            if (!w.name.Contains("_"))
            {

                if (wp.name == "In" || wp.name == "Out0" || wp.name == "Out1")
                {
                    w.name = wp.transform.parent.name + "_" + wp.name + "_" + w.name;
                }
                else
                {
                    w.name = wp.name + "_" + w.name;
                }
            }

            EditorHelper.UnpackPrefab(w.gameObject);
            parentDict.Add(w.gameObject, wp);
            w.transform.SetParent(null);
            w.transform.SetParent(WeldRootTarget.transform);
        }
        Debug.Log($"MoveWeldParent welds:{welds.Count}");
        return parentDict;
    }

    private void RecoverWeldParent(Dictionary<GameObject, Transform> dict)
    {
        foreach (var w in dict.Keys)
        {
            if (w == null) continue;
            var wp = dict[w];
            if (wp == null) continue;
            w.transform.SetParent(wp);
        }
    }

    public PrefabInfoList PrefabReducers()
    {
        AcRTAlignJobSetting.Instance.SetDefault();
        PrefabInfoList list = new PrefabInfoList();
        Dictionary<GameObject, Transform> parentDict = MoveWeldParent();
        PipeWelds = GetWelds();

        DateTime start3 = DateTime.Now;
        PrefabInfoList prefabs3 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.GetModelResult_Reducer(), true, "_Reducer_3",IsPipeTryRT);
        list.AddRange(prefabs3);
        TimeSpan t3 = DateTime.Now - start3;

        var gs = newBuilder.RefreshGenerators(Target);
        newBuilder.RefreshPipeModels(Target);
        RecoverWeldParent(parentDict);
        return list;
    }

    public PrefabInfoList PrefabFlanges()
    {
        AcRtAlignJobArg.IsDebug = true;

        AcRTAlignJobSetting.Instance.SetDefault();
        PrefabInfoList list = new PrefabInfoList();
        Dictionary<GameObject, Transform> parentDict = MoveWeldParent();
        //foreach(var obj in parentDict.Keys)
        //{
        //    if (obj == null) continue;
        //    GameObject.DestroyImmediate(obj);
        //}
        PipeWelds = GetWelds();

        DateTime start4 = DateTime.Now;
        PrefabInfoList prefabs4 = null;
        if (IsCreatePipeByUnityPrefab == false)
        {
            var list4 = newBuilder.GetModelResult_Flange();
            prefabs4 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(list4, true, "_Flange_4", IsPipeTryRT);
            //list.AddRange(prefabs1);

            Debug.LogError($"PrefabPipes Flange1 list4:{list4.Count}");
        }
        else
        {
            var list4 = newBuilder.GetModelResult_Flange(true);
            prefabs4 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(list4, true, "_Flange_4",  IsPipeTryRT);
            prefabs4.Add(new PrefabInfo());
            Debug.LogError($"PrefabPipes Flange2 list4:{list4.Count}");
        }

        TimeSpan t4 = DateTime.Now - start4;

        var gs = newBuilder.RefreshGenerators(Target);
        newBuilder.RefreshPipeModels(Target);
        RecoverWeldParent(parentDict);

        var keys = PipeFlangeModel.keys;
        var keyList = keys.Keys.ToList();
        keyList.Sort();
        for (int i = 0; i < keyList.Count; i++)
        {
            string item = keyList[i];
            Debug.Log($"key[{i}]_{item}");
        }
        AllPrefabs = prefabs4;
        ShowPrefabs(prefabs4, "Prefabs_Flange",true);


        AcRtAlignJobArg.IsDebug = false;
        return list;
    }

    

    public PrefabInfoList PrefabWeldolets()
    {
        AcRTAlignJobSetting.Instance.SetDefault();
        PrefabInfoList list = new PrefabInfoList();
        Dictionary<GameObject, Transform> parentDict = MoveWeldParent();
        PipeWelds = GetWelds();

        DateTime start6 = DateTime.Now;
        PrefabInfoList prefabs6 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.GetModelResult_Weldolet(), true, "_Weldolet_6");
        list.AddRange(prefabs6);
        TimeSpan t6 = DateTime.Now - start6;

        var gs = newBuilder.RefreshGenerators(Target);
        newBuilder.RefreshPipeModels(Target);
        RecoverWeldParent(parentDict);
        return list;
    }

    

    public PrefabInfoList PrefabTees()
    {
        AcRTAlignJobSetting.Instance.SetDefault();
        PrefabInfoList list = new PrefabInfoList();
        Dictionary<GameObject, Transform> parentDict = MoveWeldParent();
        PipeWelds = GetWelds();

        DateTime start5 = DateTime.Now;
        PrefabInfoList prefabs5 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.GetModelResult_Tee(), true, "_Tee_5", IsPipeTryRT);
        list.AddRange(prefabs5);
        TimeSpan t5 = DateTime.Now - start5;

        var gs = newBuilder.RefreshGenerators(Target);
        newBuilder.RefreshPipeModels(Target);
        RecoverWeldParent(parentDict);

        AllPrefabs = prefabs5;
        ShowPrefabs(prefabs5, "Prefabs_Tee", true);
        return list;
    }


    public PrefabInfoList PrefabElbows()
    {
        AcRTAlignJobSetting.Instance.SetDefault();
        PrefabInfoList list = new PrefabInfoList();
        Dictionary<GameObject, Transform> parentDict = MoveWeldParent();
        PipeWelds = GetWelds();

        DateTime start2 = DateTime.Now;
        var elbowGos1 = newBuilder.GetModelResult_Elbow();
        PrefabInfoList prefabs2 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(elbowGos1, true, "_Elbow_2", IsPipeTryRT);
        list.AddRange(prefabs2);
        TimeSpan t2 = DateTime.Now - start2;

        var gs = newBuilder.RefreshGenerators(Target);
        newBuilder.RefreshPipeModels(Target);
        RecoverWeldParent(parentDict);
        return list;
    }

    public PrefabInfoList PrefabPipes()
    {
        AcRTAlignJobSetting.Instance.SetDefault();

        DateTime start = DateTime.Now;

        //DateTime start1 = DateTime.Now;
        //PrefabInfoList prefabs1 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.PipeGenerators, true, "_Pipes");
        //TimeSpan t1 = DateTime.Now - start1;

        PrefabInfoList list = new PrefabInfoList();

        Dictionary<GameObject, Transform> parentDict = MoveWeldParent();

        PipeWelds = GetWelds();


        DateTime start1 = DateTime.Now;
        PrefabInfoList prefabs1 = null;
        if (IsCreatePipeByUnityPrefab==false)
        {
            prefabs1 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.GetModelResult_Line(), true, "_Line_1", IsPipeTryRT);
            list.AddRange(prefabs1);
        }
        else
        {
            prefabs1 = new PrefabInfoList(1);
        }
  
        TimeSpan t1 = DateTime.Now - start1;

        DateTime start2 = DateTime.Now;
        var elbowGos1 = newBuilder.GetModelResult_Elbow();
        PrefabInfoList prefabs2Elbow = PrefabInstanceBuilder.Instance.GetPrefabsOfList(elbowGos1, true, "_Elbow_2", IsPipeTryRT);
        list.AddRange(prefabs2Elbow);
        TimeSpan t2 = DateTime.Now - start2;

        DateTime start3 = DateTime.Now;
        PrefabInfoList prefabs3Reducer = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.GetModelResult_Reducer(), true, "_Reducer_3", IsPipeTryRT);
        list.AddRange(prefabs3Reducer);
        TimeSpan t3 = DateTime.Now - start3;

        DateTime start4 = DateTime.Now;
        PrefabInfoList prefabs4 = null;
        if (IsCreatePipeByUnityPrefab == false)
        {
            var list4 = newBuilder.GetModelResult_Flange();
            prefabs4 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(list4, true, "_Flange_4", IsPipeTryRT);
            list.AddRange(prefabs4);

            Debug.Log($"PrefabPipes Flange1 list4:{list4.Count}");
        }
        else
        {
            var list4 = newBuilder.GetModelResult_Flange(true);
            prefabs4 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(list4, true, "_Flange_4", IsPipeTryRT);
            prefabs4.Add(new PrefabInfo());
            Debug.Log($"PrefabPipes Flange2 list4:{list4.Count}");
        }

        TimeSpan t4 = DateTime.Now - start4;

        DateTime start5 = DateTime.Now;
        PrefabInfoList prefabs5 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.GetModelResult_Tee(), true, "_Tee_5", IsPipeTryRT);
        list.AddRange(prefabs5);
        TimeSpan t5 = DateTime.Now - start5;

        DateTime start6 = DateTime.Now;
        PrefabInfoList prefabs6 = PrefabInstanceBuilder.Instance.GetPrefabsOfList(newBuilder.GetModelResult_Weldolet(), true, "_Weldolet_6", IsPipeTryRT);
        list.AddRange(prefabs6);
        TimeSpan t6 = DateTime.Now - start6;

        var gs = newBuilder.RefreshGenerators(Target);
        newBuilder.RefreshPipeModels(Target);

        TimeSpan ta = DateTime.Now - start1;

        RecoverWeldParent(parentDict);

        Debug.LogError($"¡¾PipeFactory.PrefabPipes [{ta.ToString(timeFormat)}]¡¿ Pipes:{newBuilder.PipeGenerators.Count} Pipes2:{gs.Count} prefabs:{list}({prefabs1.Count}+{prefabs2Elbow.Count}+{prefabs3Reducer.Count}+{prefabs4.Count}+{prefabs5.Count}+{prefabs6.Count}) times:({t1.ToString(timeFormat)}+{t2.ToString(timeFormat)}+{t3.ToString(timeFormat)}+{t4.ToString(timeFormat)}+{t5.ToString(timeFormat)}+{t6.ToString(timeFormat)}+)");
        return list;
    }

    //private static string timeFormat = @"hh\:mm\:ss\:fff";
    private static string timeFormat = @"mm\:ss";

    public bool IsTrySameAngle = true;

    public void CombineGeneratedWelds()
    {
        newBuilder.CombineGeneratedWelds();
    }

    public PrefabInfoList PrefabWelds()
    {
        AcRTAlignJobSetting.Instance.SetDefault();
        //AcRTAlignJob.IsTrySameAngle = IsTrySameAngle;

        DateTime start = DateTime.Now;

        PrefabInfoList prefabs = null;
        if (IsCreatePipeByUnityPrefab)
        {
            prefabs = new PrefabInfoList(PipeModelUnitPrefab_Welds);
        }
        else
        {
            //newBuilder.CombineGeneratedWelds();
            var welds = newBuilder.GetNewWelds(Target);
            //return new PrefabInfoList();
            prefabs = PrefabInstanceBuilder.Instance.GetPrefabsOfList(welds, true, "_Welds(New)", IsPipeTryRT);
            Debug.LogError($"PrefabWelds time:{DateTime.Now - start} Welds:{welds.Count} prefabs:{prefabs.Count}");
        }

        //AcRTAlignJob.IsTrySameAngle = false;
        return prefabs;
    }

    public PrefabInfoList PrefabOthers()
    {
        if (PipeOthers.Count == 0) return new PrefabInfoList();
        AcRTAlignJobSetting.Instance.SetDefault();
        DateTime start = DateTime.Now;
        int count1 = PipeOthers.Count;
        PrefabInfoList prefabs =PrefabInstanceBuilder.Instance.GetPrefabsOfList(PipeOthers, true, "_Others",IsOthersTryRT);
        PipeOthers.Clear();
        PipeOthers.AddRange(prefabs.GetComponents<Transform>());
        Debug.Log($"PrefabOthers time:{DateTime.Now-start} count1:{count1} Others:{this.PipeOthers.Count} prefabs:{prefabs.Count}");
        //RecoverOthersParent();
        return prefabs;
    }

    [ContextMenu("HidAll")]
    public void HidAll()
    {
        SetAllVisible(false);
    }

    [ContextMenu("OnlyShowPipe")]
    public void OnlyShowPipe()
    {
        HidAll();
        SetListVisible(PipeLines, true);
    }

    public PipeBuilder newBuilder;

    public List<PipeModelBase> GetPipeModels()
    {
        if (newBuilder == null) return new List<PipeModelBase>();
        return newBuilder.PipeModels;
    }

    public PipeRunList GetPipeRunList()
    {
        if (newBuilder == null) return new PipeRunList();
        return newBuilder.pipeRunList;
    }

    //[ContextMenu("GetInfoAndCreateEachPipes")]
    //public void GetInfoAndCreateEachPipes()
    //{
    //    InitPipeBuilder();
    //    newBuilder.GetInfoAndCreateEachPipes();
    //}

    public void ReplacePipes()
    {
        newBuilder.ReplaceOld();
        
        if (targetNew)
        {
            GameObject.DestroyImmediate(targetNew);
        }
    }

    [ContextMenu("GetPipeInfos")]
    public void GetPipeInfos()
    {
        InitPipeBuilder();

        //newBuilder.ClearGeneratedObjs();

        newBuilder.isUniformRaidus = this.isUniformRaidus;
        newBuilder.GetPipeInfosEx();

        ProgressBarHelper.ClearProgressBar();
    }

    public void SaveSceneDataXml()
    {
        newBuilder.SaveSceneDataXml(this.Target.name,generateArg);
    }

    public void LoadSceneDataXml()
    {
        generateArg2 = null;
        newBuilder.LoadSceneDataXml(this.Target.name, this.Target.transform);
        
    }

    public void RemoveComponents()
    {
        newBuilder.RemoveComponents(this.Target);
    }

    public void RemovePipeModels()
    {
        newBuilder.RemovePipeModels(this.Target);
    }

    public void RemoveMeshes()
    {
        newBuilder.RemoveMeshes(this.Target);
    }

    public void FindPipeModels()
    {
        newBuilder.FindPipeModels(this.Target);
    }
    public void ShowPipeModels()
    {
        newBuilder.ShowPipeModels(this.Target);
    }

    [ContextMenu("GetObbInfJobs")]
    public void GetObbInfosJob()
    {
        InitPipeBuilder();
        newBuilder.GetObbInfosJob();
    }

    [ContextMenu("GetObbInfos")]
    public void GetObbInfos()
    {
        InitPipeBuilder();
        newBuilder.GetObbInfos();
    }

    [ContextMenu("GetPipeInfosJob")]
    public void GetPipeInfosJob()
    {
        InitPipeBuilder();

        //newBuilder.ClearGeneratedObjs();

        newBuilder.isUniformRaidus = this.isUniformRaidus;
        newBuilder.GetPipeInfosJob();

        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("CreatePipeRunList")]
    public void CreateRunList()
    {
        newBuilder.IsCreatePipeRuns = true;
        newBuilder.CreatePipeRunList();
    }

    public bool IsRendererOnStart = false;

    public bool IsLoadXmlOnStart = false;

    void Start()
    {
        if(IsLoadXmlOnStart)
        {
            LoadSceneDataXml();
        }
        else if(IsRendererOnStart)
        {
            RendererEachPipesEx();
        }
    }

    [ContextMenu("RendererEachPipesEx")]
    public void RendererEachPipesEx()
    {
        ClearWeldPrefabs();
        //ClearGeneratedObjs();
        RendererEachPipes();
        MovePipes();
    }

    [ContextMenu("RendererEachPipes")]
    public void RendererEachPipes()
    {
        //InitPipeBuilder();
        newBuilder.isUniformRaidus = this.isUniformRaidus;
        newBuilder.IsCreatePipeRuns = this.IsCreatePipeRuns;
        newBuilder.IsSaveMaterials = this.IsSaveMaterials;
        newBuilder.IsCopyComponents = this.IsCopyComponents;

        newBuilder.minWeldDis = this.minWeldDis;
        newBuilder.maxWeldDis = this.maxWeldDis;

    //newBuilder.CreatePipeRunList();

        newBuilder.NewObjName = NewObjName;
        newBuilder.generateArg = generateArg;

        newBuilder.RendererPipesEx();

        ProgressBarHelper.ClearProgressBar();

        ResultInfo= ShowTargetInfo(newBuilder.gameObject);
    }

    [ContextMenu("CheckResults")]
    public void CheckResults()
    {
        newBuilder.CheckResults();
        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("CheckResultsJob")]
    public void CheckResultsJob()
    {
        newBuilder.CheckResultsJob();
        ProgressBarHelper.ClearProgressBar();
    }

    private void InitPipeBuilder()
    {
        if (newBuilder == null)
        {
            GameObject builder = new GameObject("Builder");
            builder.transform.position = this.transform.position;
            builder.transform.SetParent(this.transform);
            newBuilder = builder.AddComponent<PipeBuilder>();
        }
        newBuilder.Target = this.Target;
        newBuilder.NewObjName = NewObjName;
        newBuilder.generateArg = generateArg;
        newBuilder.IsCreatePipeRuns = this.IsCreatePipeRuns;

        if (EnablePipeLine)
        {
            newBuilder.PipeLineGos = PipeLines;
        }
        else
        {
            newBuilder.PipeLineGos = new List<Transform>();
        }
        if(EnablePipeElbow)
        {
            newBuilder.PipeElbowGos = PipeElbows;
        }
        else
        {
            newBuilder.PipeElbowGos = new List<Transform>();
        }

        if (EnablePipeReducer)
        {
            newBuilder.PipeReducerGos = PipeReducers;
        }
        else
        {
            newBuilder.PipeReducerGos = new List<Transform>();
        }
        if (EnablePipeFlange)
        {
            newBuilder.PipeFlangeGos = PipeFlanges;
        }
        else
        {
            newBuilder.PipeFlangeGos = new List<Transform>();
        }
        if (EnablePipeTee)
        {
            newBuilder.PipeTeeGos = PipeTees;
        }
        else
        {
            newBuilder.PipeTeeGos = new List<Transform>();
        }

        if (EnablePipeWeldolet)
        {
            newBuilder.PipeWeldoletGos = PipeWeldolets;
        }
        else
        {
            newBuilder.PipeWeldoletGos = new List<Transform>();
        }

        if (EnableBoxModel)
        {
            newBuilder.BoxModelGos = BoxModels;
        }
        else
        {
            newBuilder.BoxModelGos = new List<Transform>();
        }
    }

    public bool EnablePipeLine = true;

    public bool EnablePipeElbow = true;

    public bool EnablePipeReducer = true;

    public bool EnablePipeTee = true;

    public bool EnablePipeFlange = true;

    public bool EnablePipeWeldolet = true;

    public bool EnableBoxModel = true;

    GameObject targetNew;

    public bool IsMoveResultToFactory = true;

    public bool IsSaveMaterials = true;

    public bool IsCopyComponents = true;

    public bool IsCreatePipeRuns = false;

    public bool IsReplaceOld = true;

    public bool IsPrefabGos = true;

    public bool IsPrefabOthers = true;

    public bool IsPrefabOldWeld = true;

    public bool IsCheckResult = false;

    public void MovePipes()
    {
        if (IsMoveResultToFactory == false) return;

        if (targetNew != null)
        {
            if (targetNew.transform.childCount == 0)
            {
                GameObject.DestroyImmediate(targetNew);
            }
        }

        targetNew = new GameObject();
        targetNew.name = Target.name + "_New";
        targetNew.transform.position = Target.transform.position;
        targetNew.transform.SetParent(Target.transform.parent);

        if (IsCreatePipeRuns)
        {
            if (newBuilder.pipeRunList != null)
            {
                var runs = newBuilder.pipeRunList.PipeRunGos;
                foreach (var run in runs)
                {
                    if (run == null) continue;
                    run.transform.SetParent(targetNew.transform);
                }
            }
            else
            {
                var newPipes = newBuilder.NewPipeList;
                foreach (var pipe in newPipes)
                {
                    if (pipe == null) continue;
                    pipe.SetParent(targetNew.transform);
                }
            }
        }
        else
        {
            var newPipes = newBuilder.NewPipeList;
            foreach (var pipe in newPipes)
            {
                if (pipe == null) continue;
                pipe.SetParent(targetNew.transform);
            }
        }


        targetNew.transform.SetParent(newBuilder.transform);

        ResultInfo=ShowTargetInfo(targetNew);
    }

    private string ShowTargetInfo(GameObject t)
    {
        MeshNode meshNode = MeshNode.InitNodes(t);
        meshNode.GetSharedMeshList();
        return meshNode.GetVertexInfo();
    }

    //public Material pipeMaterial;

    //public Material weldMaterial;

    public string NewObjName = "_New";

    public PipeGenerateArg generateArg = new PipeGenerateArg();

    public PipeGenerateArg generateArg2 = new PipeGenerateArg();

    public Material GetPipeMaterial()
    {
        return generateArg.pipeMaterial;
    }

    public Material GetWeldMaterial()
    {
        return generateArg.weldMaterial;
    }

    public PipeGenerateArg GetLoadXmlArg()
    {
        if (generateArg2 == null)
        {
            generateArg2= generateArg.Clone();
        }
        PipeGenerateArg arg = generateArg2;
        arg.generateWeld = false;
        return arg;
    }

    public void RendererModelFromXml(PipeModelBase pipeModel, MeshModelSaveData data)
    {
        pipeModel.RendererModel(GetLoadXmlArg(), NewObjName);
        //RendererWeldsFromXml(pipeModel, data);
    }

    public void RendererWeldsFromXml(List<PipeWeldSaveData> welds, Transform parent)
    {
        if (welds != null)
        {
            foreach (var weldData in welds)
            {
                if (weldData.isPrefab)
                {
                    CreateWeldGo(weldData, parent);
                }
            }
            foreach (var weldData in welds)
            {
                if (weldData.isPrefab==false)
                {
                    CopyWeldGo(weldData, parent);
                }
            }
        }
    }

    private GameObject CopyWeldGo(PipeWeldSaveData item, Transform parent)
    {
        GameObject go = new GameObject(item.Name);
        PipeWeldModel model = go.AddComponent<PipeWeldModel>();

        //PipeModels.Add(model);

        model.SetSaveData(item);
        //model.RendererModel(GetLoadXmlArg(), NewObjName);

        RendererId rId = RendererId.GetRId(go);
        rId.Id = item.Id;
        if (parent == null)
        {
            var pGo = IdDictionary.GetGo(item.PId);
            if (pGo != null)
            {
                parent = pGo.transform;
            }
            else
            {
                Debug.LogError($"CopyWeldGo pGo==null pId:{item.PId} name:{item.Name}");
            }
        }
        go.transform.SetParent(parent);

        ////item.Transform.SetTransform(go.transform);
        item.SetTransformInfo(go.transform);

        //GameObject weldNew = model.RendererModel(GetLoadXmlArg(), NewObjName);

        GameObject prefabGo = IdDictionary.GetGo(item.prefabId, item.Name, false);
        if (prefabGo == null)
        {
            Debug.LogError($"CopyWeldGo prefabGo == null Name:{item.Name} prefabId:{item.prefabId}");
            return null;
        }
        MeshFilter mf = prefabGo.GetComponent<MeshFilter>();
        if (prefabGo == null)
        {
            Debug.LogError($"CopyWeldGo MeshFilter == null Name:{item.Name} prefabId:{item.prefabId}");
            return null;
        }
        Mesh ms = mf.sharedMesh;
        if (prefabGo == null)
        {
            Debug.LogError($"CopyWeldGo sharedMesh == null Name:{item.Name} prefabId:{item.prefabId}");
            return null;
        }
        MeshFilter mf2 = go.AddMissingComponent<MeshFilter>();
        mf2.sharedMesh = ms;

        MeshRenderer mr2 = go.AddMissingComponent<MeshRenderer>();
        if (item is PipeWeldSaveData)
        {
            mr2.sharedMaterial = this.generateArg.weldMaterial;
        }
        else
        {
            mr2.sharedMaterial = this.generateArg.pipeMaterial;
        }

        go.name = item.Name;

        IdDictionary.SetId(go);
        return go;
    }

    private GameObject CreateWeldGo(PipeWeldSaveData weldData,Transform parent)
    {
        GameObject go = new GameObject(weldData.Name);
        PipeWeldModel model = go.AddComponent<PipeWeldModel>();

        //PipeModels.Add(model);

        model.SetSaveData(weldData);
        //model.RendererModel(GetLoadXmlArg(), NewObjName);

        RendererId rId = RendererId.GetRId(go);
        rId.Id = weldData.Id;
        if (parent == null)
        {
            var pGo = IdDictionary.GetGo(weldData.PId);
            if (pGo != null)
            {
                parent = pGo.transform;
            }
        }
        go.transform.SetParent(parent);

        ////item.Transform.SetTransform(go.transform);
        //item.SetTransformInfo(go.transform);

        GameObject weldNew = model.RendererModel(GetLoadXmlArg(), NewObjName);
        weldNew.name = weldData.Name;
        IdDictionary.SetId(go);
        return go;
    }

    public int MinPipeSegments = 16;

    public List<PipeModelBase> RunTestModels = new List<PipeModelBase>();

    public PipeRunList TestRunList = new PipeRunList();

    public void TestModelIsConnected()
    {
        TestRunList=newBuilder.TestModelIsConnected(RunTestModels);
    }
}
