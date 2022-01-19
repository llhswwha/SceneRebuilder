using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PipeBuilder : MonoBehaviour
{
    public List<Transform> PipeLineGos = new List<Transform>();

    public List<PipeLineModel> PipeLines = new List<PipeLineModel>();

    public List<Transform> PipeElbowGos = new List<Transform>();

    public List<PipeElbowModel> PipeElbows = new List<PipeElbowModel>();

    public List<Transform> PipeReducerGos = new List<Transform>();

    public List<PipeReducerModel> PipeReducers = new List<PipeReducerModel>();

    public List<Transform> PipeFlangeGos = new List<Transform>();

    public List<PipeFlangeModel> PipeFlanges = new List<PipeFlangeModel>();

    public List<Transform> PipeTeeGos = new List<Transform>();

    public List<PipeTeeModel> PipeTees = new List<PipeTeeModel>();

    public List<PipeModelBase> PipeModels = new List<PipeModelBase>();

    public List<PipeMeshGeneratorBase> PipeGenerators = new List<PipeMeshGeneratorBase>();

    public List<Transform> NewPipeList = new List<Transform>();

    public void ShowOBB()
    {
        OBBCollider oBB = this.gameObject.GetComponent<OBBCollider>();
        if (oBB == null)
        {
            oBB = this.gameObject.AddComponent<OBBCollider>();
        }
        if (oBB != null)
        {
            oBB.ShowObbInfo(true);
        }
    }

    //public Material PipeMaterial;

    //public Material WeldMaterial;

    //public int pipeSegments = 24;

    //public float weldRadius = 0.005f;

    //public Vector3 Offset = Vector3.zero;

    public PipeGenerateArg generateArg = new PipeGenerateArg();

    public PipeGenerateArg GetPipeGenerateArg()
    {
        //return generateArg.Clone();
        return generateArg;
    }

    //public Vector3 StartPoint = Vector3.zero;
    //public Vector3 EndPoint = Vector3.zero;

    //public Vector3 P1 = Vector3.zero;
    //public Vector3 P2 = Vector3.zero;
    //public Vector3 P3 = Vector3.zero;
    //public Vector3 P4 = Vector3.zero;

    //public OrientedBoundingBox OBB;

    private void CreatePoint(Vector3 p, string n)
    {
        GameObject g1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        g1.transform.SetParent(this.transform);
        g1.transform.localPosition = p;
        g1.transform.localScale = new Vector3(lineSize, lineSize, lineSize);
        g1.name = n;
    }

    public float lineSize = 0.01f;

    public List<Transform> GetInfoAndCreateEachPipes()
    {
        DateTime start = DateTime.Now;

        ClearGeneratedObjs();
        //GetPipeInfos();
        GetPipeInfosEx();

        //RendererPipes();
        RendererPipesEx();

        CheckResults();

        Debug.LogError($">CreateEachPipes time:{DateTime.Now - start}");

        ProgressBarHelper.ClearProgressBar();
        return NewPipeList;
    }

    public void CheckResults()
    {
        DateTime start = DateTime.Now;
        for (int i = 0; i < PipeModels.Count; i++)
        {
            PipeModelBase go = PipeModels[i];
            if (go == null) continue;
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("CheckResults", i, PipeModels.Count, go)))
            {
                return;
            }
            go.CheckResult();
        }
        PipeModels.Sort();
        Debug.LogError($">>CheckResults time:{DateTime.Now - start}");
    }

    public void CheckResultsJob()
    {
        DateTime start = DateTime.Now;

        PipeModelCheckJob.Result = new NativeArray<PipeModelCheckResult>(PipeModels.Count, Allocator.Persistent);

        JobList<PipeModelCheckJob> jobs = new JobList<PipeModelCheckJob>(JobSize);
        for (int i = 0; i < PipeModels.Count; i++)
        {
            PipeModelBase go = PipeModels[i];
            if (go.ResultGo == null)
            {
                Debug.LogError($"CheckResultsJob go.ResultGo == null go:{go.name}");
                return;
            }
            Vector3[] vs1 = OrientedBoundingBox.GetVertices(go.gameObject);
            Vector3[] vs2 = OrientedBoundingBox.GetVertices(go.ResultGo);
            MeshRendererInfo r1 = MeshRendererInfo.GetInfo(go.gameObject);
            MeshRendererInfo r2 = MeshRendererInfo.GetInfo(go.ResultGo);
            Vector3[] cs1 = r1.GetBoxCornerPonts();
            Vector3[] cs2 = r2.GetBoxCornerPonts();
            jobs.Add(new PipeModelCheckJob()
            {
                id = i,
                points1 = new NativeArray<Vector3>(vs1, Allocator.Persistent),
                points2 = new NativeArray<Vector3>(vs2, Allocator.Persistent),
                connerPoints1= new NativeArray<Vector3>(cs1, Allocator.Persistent),
                connerPoints2 = new NativeArray<Vector3>(cs2, Allocator.Persistent)
            });
        }
        jobs.CompleteAllPage();
        for (int i = 0; i < PipeModels.Count; i++)
        {
            PipeModelBase go = PipeModels[i];
            go.GetCheckResult(PipeModelCheckJob.Result[i]);
        }
        jobs.Dispose();
        PipeModelCheckJob.Result.Dispose();

        PipeModels.Sort();

        Debug.LogError($">>CheckResults time:{DateTime.Now - start}");
    }

    public string NewObjName = "_New";

    public bool IsCreatePipeRuns = false;

    public bool IsSaveMaterials = true;

    public bool IsCopyComponents = true;

    public void ReplaceOld()
    {
        DateTime start = DateTime.Now;
        List<PipeModelBase> newModels = new List<PipeModelBase>();
        for (int i = 0; i < PipeModels.Count; i++)
        {
            PipeModelBase model = PipeModels[i];
            if (model == null) continue;
            model.ReplaceOld();
        }

        //PipeModels.Clear();

        //PipeModels = newModels;

        Debug.LogWarning($">>ReplaceOld time:{DateTime.Now - start}");
    }

    public void RendererPipesEx()
    {
        DateTime start = DateTime.Now;
        PipeGenerators.Clear();
        for (int i = 0; i < PipeModels.Count; i++)
        {
            PipeModelBase go = PipeModels[i];
            if (go == null) continue;
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("RendererPipes", i, PipeModels.Count, go)))
            {
                return;
            }
            GameObject pipe = go.RendererModel(this.GetPipeGenerateArg(), NewObjName);
            if (pipe != null)
            {
                NewPipeList.Add(pipe.transform);
            }

            if (IsSaveMaterials)
            {
                MeshRenderer mf1 = go.GetComponent<MeshRenderer>();
                MeshRenderer mf2 = pipe.GetComponent<MeshRenderer>();
                if (mf1 != null && mf2 != null)
                {
                    mf2.sharedMaterials = mf1.sharedMaterials;
                }
            }

            if (IsCopyComponents)
            {
                EditorHelper.CopyAllComponents(go.gameObject, pipe, false, typeof(PipeModelBase));
            }

            var generator = pipe.GetComponent<PipeMeshGeneratorBase>();
            PipeGenerators.Add(generator);
        }

        if (IsCreatePipeRuns && pipeRunList != null)
        {
            pipeRunList.RenameResultBySortedId();
        }

        //CombineGeneratedWelds();
        

        Debug.LogWarning($">>RendererPipes time:{DateTime.Now - start}");
    }

    public float minWeldDis = 0.0001f;
    public float maxWeldDis = 0.05f;

    public void CombineGeneratedWelds()
    {
        if (generateArg.generateWeld == false) return;
        DateTime time = DateTime.Now;
        List<Transform> elbowWelds = GetWelds(PipeElbows);
        List<Transform> elbowWelds2 = new List<Transform>(elbowWelds);
        int count1 = elbowWelds.Count;
        CombineGeneratedWelds(elbowWelds2, elbowWelds, minWeldDis);
        int count2 = elbowWelds.Count;

        List<Transform> pipeElbows = GetWelds(PipeLines);
        int count3 = pipeElbows.Count;
        CombineGeneratedWelds(pipeElbows, elbowWelds, minWeldDis);
        int count4 = pipeElbows.Count;

        List<Transform> teeElbows = GetWelds(PipeTees);
        CombineGeneratedWelds(pipeElbows, teeElbows, minWeldDis);

        List<Transform> reducerElbows = GetWelds(PipeReducers);
        CombineGeneratedWelds(pipeElbows, reducerElbows, minWeldDis);

        List<Transform> elbowWelds4 = GetWelds(PipeElbows);

        Debug.Log($"CombineGeneratedWelds time:{DateTime.Now-time} elbowWelds1:{count1} elbowWelds2:{count2} elbowWelds3:{elbowWelds.Count} elbowWelds4:{elbowWelds4.Count} pipeElbows2:{count4}");
    }

    private void CombineGeneratedWelds(List<Transform> elbows1, List<Transform> elbows2,float minWeldDis)
    {
        
        for (int i = 0; i < elbows1.Count; i++)
        {
            var w = elbows1[i];
            if (w == null) continue;
            var closedW = TransformHelper.FindClosedComponentEx(elbows2, w, false);
            var w2 = closedW.t;
            if (closedW.dis < minWeldDis)
            {
                elbows2.Remove(w2);
                //elbows1.Remove(w2);
                //elbows1.RemoveAt(i); i--;
                
                Debug.Log($"[{i}] w:{w.name} w2:{w2.name} {w==w2} dis:{closedW.dis} min:{minWeldDis}");

                GameObject.DestroyImmediate(w2.gameObject);
            }
        }
    }

    internal List<Transform> GetWelds<T>(List<T> pipeModels) where T :PipeModelBase
    {
        List<Transform> ts = new List<Transform>();
        foreach (var model in pipeModels)
        {
            if (model == null) continue;
            if (model.ResultGo == null) continue;
            var g = model.ResultGo.GetComponent<PipeMeshGeneratorBase>();
            ts.AddRange(g.Childrens);
        }
        return ts;
    }

    internal List<Transform> GetWelds()
    {
        List<Transform> ts = new List<Transform>();
        foreach (var g in PipeGenerators)
        {
            if (g == null) continue;
            //ts.AddRange(g.Childrens);
            foreach(var c in g.Childrens)
            {
                if (c == null) continue;
                ts.Add(c);
            }
        }
        return ts;
    }

    public List<Transform> GetAllPipeGos()
    {
        List<Transform> ts = new List<Transform>();
        ts.AddRange(PipeLineGos);
        ts.AddRange(PipeElbowGos);
        ts.AddRange(PipeTeeGos);
        ts.AddRange(PipeReducerGos);
        ts.AddRange(PipeFlangeGos);
        return ts;
    }
    

    internal void GetObbInfosJob()
    {
        var ts = GetAllPipeGos();   
        ObbInfoJob.GetObbInfoJobs(ts);
    }


    internal void GetObbInfos()
    {
        var ts = GetAllPipeGos();
        GetObbInfos(ts);
    }

    private static void GetObbInfos(List<Transform> gos)
    {
        DateTime startT = DateTime.Now;
        for (int i = 0; i < gos.Count; i++)
        {
            Transform go = gos[i];
            if (go == null) continue;
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetObbInfos", i, gos.Count, go)))
            {
                return;
            }
            OBBCollider obbC=go.gameObject.AddMissingComponent<OBBCollider>();
            obbC.GetObb(false);
        }
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"GetObbInfos count:{gos.Count} time:{(DateTime.Now - startT).TotalMilliseconds.ToString("F2")}");
    }



    private void RendererPipes()
    {
        DateTime start = DateTime.Now;
        RendererPipeLines();
        RendererPipeElbows();
        Debug.LogError($">>RendererPipes time:{DateTime.Now - start}");
    }

    public void CreateElbows()
    {
        GetPipeInfos();
    }

    public List<Vector3> points;

    public PipeMeshGenerator CreateOnePipe()
    {
        GetPipeInfos();
        ClearGeneratedObjs();
        var newPipe = RendererOnePipe();
        NewPipeList.Add(newPipe.transform);
        newPipe.transform.SetParent(this.transform);
        //return pipeNew;
        return newPipe;
    }

    public void TestCreateOnePipe()
    {
        var newPipe = CreateOnePipe();
        newPipe.ShowPoints();
    }

    public float ElbowOffset = 0.1f;

    public PipeCreateArgs PipeArgs = new PipeCreateArgs();

    public bool IsGenerateElbowBeforeAfter = true;

    public bool UseOnlyEndPoint = true;

    private PipeMeshGenerator RendererOnePipe()
    {
        PipeArgs = new PipeCreateArgs(PipeLines);
        points = PipeArgs.GetLocalPoints(UseOnlyEndPoint);

        GameObject pipeNew = new GameObject(this.name + "_NewPipe");
        //pipeNew.transform.position = this.transform.position + Offset;
        //pipeNew.transform.position = Vector3.zero;
        pipeNew.transform.position = PipeArgs.centerP;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        pipe.points = points;
        pipe.pipeSegments = generateArg.pipeSegments;
        pipe.pipeMaterial = generateArg.pipeMaterial;
        pipe.weldMaterial = generateArg.weldMaterial;
        pipe.weldRadius = generateArg.weldRadius;

        pipe.elbowRadius = PipeArgs.elbowRadius;//Elbow
        pipe.IsGenerateElbowBeforeAfter = IsGenerateElbowBeforeAfter;
        pipe.generateWeld = generateArg.generateWeld;

        pipe.avoidStrangling = true;
        pipe.pipeRadius = PipeLines[0].PipeRadius;
        pipe.RenderPipe();
        return pipe;
    }

    internal void RefreshGenerators()
    {
        throw new NotImplementedException();
    }

    public void RendererPipeLines()
    {
        DateTime start = DateTime.Now;
        //ClearOldGos();
        for (int i = 0; i < PipeLines.Count; i++)
        {
            PipeLineModel go = PipeLines[i];
            GameObject pipe = go.RendererModel(this.GetPipeGenerateArg(), NewObjName);
            NewPipeList.Add(pipe.transform);
        }
        Debug.LogError($">>>RendererPipeLines time:{DateTime.Now - start}");
    }

    public void RendererPipeElbows()
    {
        //ClearOldGos();
        DateTime start = DateTime.Now;
        for (int i = 0; i < PipeElbows.Count; i++)
        {
            PipeElbowModel go = PipeElbows[i];
            GameObject pipe = go.RendererModel(this.GetPipeGenerateArg(), NewObjName);
            NewPipeList.Add(pipe.transform);
        }
        Debug.LogError($">>>RendererPipeElbows time:{DateTime.Now - start}");
    }

    [ContextMenu("ClearOldGos")]
    public void ClearGeneratedObjs()
    {
        foreach (Transform item in NewPipeList)
        {
            if (item == null) continue;
            GameObject.DestroyImmediate(item.gameObject);
        }
        NewPipeList.Clear();

        if (pipeRunList != null)
        {
            foreach (GameObject item in pipeRunList.PipeRunGos)
            {
                if (item == null) continue;
                GameObject.DestroyImmediate(item);
            }
        }
    }

    public void GetPipeInfos()
    {
        DateTime start = DateTime.Now;
        PipeModels = new List<PipeModelBase>();
        GetPipeLineInfos();
        GetPipeElbowInfos();
        PipeModels.Sort();
        Debug.LogError($">>GetPipeInfos time:{DateTime.Now- start}");
    }

    public int JobSize = 20;

    private void SetJobResultData_Line(JobList<PipeLineInfoJob> lineJobs, List<Transform> ts)
    {
        Debug.Log($"SetJobResultData_Line lineJobs:{lineJobs.Count} ts:{ts.Count}");
        if(lineJobs.Count!= ts.Count)
        {
            Debug.LogError($"SetJobResultData_Line lineJobs.Count!= ts.Count lineJobs:{lineJobs.Count} ts:{ts.Count}");
            return;
        }
        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            PipeLineModel pipeModel = GetPipeModelInfo<PipeLineModel>(t, false);
            var lineData = PipeLineInfoJob.Result[i];
            //Debug.Log($"LineModel[{i}] model:{pipeModel.name} lineData:{lineData}");
            pipeModel.SetLineData(lineData);
            PipeLines.Add(pipeModel);
            AddPipeModel(pipeModel);
        }
        lineJobs.Dispose();
        PipeLineInfoJob.Result.Dispose();
    }

    public void AddPipeModel(PipeModelBase model)
    {
        if (model == null)
        {
            Debug.LogError($"AddPipeModel model == null");
            return;
        }
        PipeModels.Add(model);
    }

    public void AddPipeModelRange<T>(List<T> models) where T : PipeModelBase
    {
        PipeModels.AddRange(models);
    }

    private void SetJobResultData_Elbow(JobList<PipeElbowInfoJob> elbowJobs, List<Transform> ts)
    {
        Debug.Log($"SetJobResultData_Elbow lineJobs:{elbowJobs.Count} ts:{ts.Count}");
        if (elbowJobs.Count != ts.Count)
        {
            Debug.LogError($"SetJobResultData_Elbow lineJobs.Count!= ts.Count lineJobs:{elbowJobs.Count} ts:{ts.Count}");
            return;
        }
        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            PipeElbowModel pipeModel = GetPipeModelInfo<PipeElbowModel>(t, false);
            var lineData = PipeElbowInfoJob.Result[i];
            //Debug.Log($"LineModel[{i}] model:{pipeModel.name} lineData:{lineData}");
            pipeModel.SetModelData(lineData);
            PipeElbows.Add(pipeModel);
            AddPipeModel(pipeModel);
        }
        elbowJobs.Dispose();
        PipeElbowInfoJob.Result.Dispose();
    }

    private void SetJobResultData_Tee(JobList<PipeTeeInfoJob> elbowJobs, List<Transform> ts)
    {
        Debug.Log($"SetJobResultData_Tee lineJobs:{elbowJobs.Count} ts:{ts.Count}");
        if (elbowJobs.Count != ts.Count)
        {
            Debug.LogError($"SetJobResultData_Tee lineJobs.Count!= ts.Count lineJobs:{elbowJobs.Count} ts:{ts.Count}");
            return;
        }
        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            PipeTeeModel pipeModel = GetPipeModelInfo<PipeTeeModel>(t, false);
            var lineData = PipeTeeInfoJob.Result[i];
            //Debug.Log($"LineModel[{i}] model:{pipeModel.name} lineData:{lineData}");
            pipeModel.SetModelData(lineData);
            PipeElbows.Add(pipeModel);
            AddPipeModel(pipeModel);
        }
        elbowJobs.Dispose();
        PipeTeeInfoJob.Result.Dispose();
    }
    private void SetJobResultData_Flange(JobList<PipeFlangeInfoJob> elbowJobs, List<Transform> ts)
    {
        Debug.Log($"SetJobResultData_Flange lineJobs:{elbowJobs.Count} ts:{ts.Count}");
        if (elbowJobs.Count != ts.Count)
        {
            Debug.LogError($"SetJobResultData_Flange lineJobs.Count!= ts.Count lineJobs:{elbowJobs.Count} ts:{ts.Count}");
            return;
        }
        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            PipeFlangeModel pipeModel = GetPipeModelInfo<PipeFlangeModel>(t, false);
            var lineData = PipeFlangeInfoJob.Result[i];
            //Debug.Log($"LineModel[{i}] model:{pipeModel.name} lineData:{lineData}");
            pipeModel.SetModelData(lineData);
            PipeFlanges.Add(pipeModel);
            AddPipeModel(pipeModel);
        }
        elbowJobs.Dispose();
        PipeFlangeInfoJob.Result.Dispose();
    }

    private void SetJobResultData_Reducer(JobList<PipeReducerInfoJob> elbowJobs, List<Transform> ts)
    {
        Debug.Log($"SetJobResultData_Reducer lineJobs:{elbowJobs.Count} ts:{ts.Count}");
        if (elbowJobs.Count != ts.Count)
        {
            Debug.LogError($"SetJobResultData_Reducer lineJobs.Count!= ts.Count lineJobs:{elbowJobs.Count} ts:{ts.Count}");
            return;
        }
        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            PipeReducerModel pipeModel = GetPipeModelInfo<PipeReducerModel>(t, false);
            var lineData = PipeReducerInfoJob.Result[i];
            //Debug.Log($"LineModel[{i}] model:{pipeModel.name} lineData:{lineData}");
            pipeModel.SetModelData(lineData);
            PipeReducers.Add(pipeModel);
            AddPipeModel(pipeModel);
        }
        elbowJobs.Dispose();
        PipeReducerInfoJob.Result.Dispose();
    }

    public void ClearModels()
    {
        PipeModels.Clear();
        PipeElbows.Clear();
        PipeLines.Clear();
        PipeTees.Clear();
        PipeReducers.Clear();
        PipeFlanges.Clear();
        PipeGenerators.Clear();
    }

    public void GetPipeInfosJob()
    {
        ClearModels();

        DateTime start = DateTime.Now;

        JobHandleList pipeJobs = new JobHandleList("PipeJobs", JobSize);

        //1.PipeLineJob
        PipeLineInfoJob.Result = new NativeArray<PipeLineData>(PipeLineGos.Count, Allocator.Persistent);
        PipeLineInfoJob.ErrorIds = new NativeList<int>(Allocator.Persistent);
        JobList<PipeLineInfoJob> lineJobs=GetPipeInfosJob_Line(PipeLineGos,0);

        //2.PipeElbowJob
        PipeElbowInfoJob.Result=new NativeArray<PipeElbowData>(PipeElbowGos.Count, Allocator.Persistent);
        JobList<PipeElbowInfoJob> elbowJobs = GetPipeInfosJob_Elbow(PipeElbowGos, 0);

        PipeTeeInfoJob.Result = new NativeArray<PipeTeeData>(PipeTeeGos.Count, Allocator.Persistent);
        JobList<PipeTeeInfoJob> teeJobs = GetPipeInfosJob_Tee(PipeTeeGos, 0);

        PipeReducerInfoJob.Result = new NativeArray<PipeReducerData>(PipeReducerGos.Count, Allocator.Persistent);
        PipeReducerInfoJob.ErrorIds = new NativeList<int>(Allocator.Persistent);
        JobList<PipeReducerInfoJob> reducerJobs = GetPipeInfosJob_Reducer(PipeReducerGos, 0);

        PipeFlangeInfoJob.Result = new NativeArray<PipeReducerData>(PipeFlangeGos.Count, Allocator.Persistent);
        PipeFlangeInfoJob.ErrorIds = new NativeList<int>(Allocator.Persistent);
        JobList<PipeFlangeInfoJob> flangeJobs = GetPipeInfosJob_Flange(PipeFlangeGos, 0);


        //lineJobs.CompleteAllPage();
        //elbowJobs.CompleteAllPage();

        pipeJobs.Add(lineJobs.HandleList);
        pipeJobs.Add(elbowJobs.HandleList);
        pipeJobs.Add(teeJobs.HandleList);
        pipeJobs.Add(reducerJobs.HandleList);
        pipeJobs.Add(flangeJobs.HandleList);
        pipeJobs.CompleteAllPage();

        SetJobResultData_Line(lineJobs, PipeLineGos);
        SetJobResultData_Elbow(elbowJobs, PipeElbowGos);
        SetJobResultData_Tee(teeJobs, PipeTeeGos);
        SetJobResultData_Reducer(reducerJobs, PipeReducerGos);
        SetJobResultData_Flange(flangeJobs, PipeFlangeGos);

        pipeJobs.Dispose();


        for(int i=0;i< PipeLineInfoJob.ErrorIds.Length; i++)
        {
            int id = PipeLineInfoJob.ErrorIds[i];
            var go = PipeLines[id];
            Debug.LogError($"GetPipeInfosJob ErrorLine id:{id} name:{go.name} v:{go.VertexCount} r:{go.IsGetInfoSuccess}");
        }

        for (int i = 0; i < PipeReducerInfoJob.ErrorIds.Length; i++)
        {
            int id = PipeReducerInfoJob.ErrorIds[i];
            var go = PipeReducers[id];
            Debug.LogError($"GetPipeInfosJob ErrorReducer id:{id} name:{go.name} v:{go.VertexCount} r:{go.IsGetInfoSuccess}");
        }

        for (int i = 0; i < PipeFlangeInfoJob.ErrorIds.Length; i++)
        {
            int id = PipeFlangeInfoJob.ErrorIds[i];
            var go = PipeFlanges[id];
            Debug.LogError($"GetPipeInfosJob ErrorFlange id:{id} name:{go.name} v:{go.VertexCount} r:{go.IsGetInfoSuccess}");
        }

        PipeLineInfoJob.ErrorIds.Dispose();
        PipeReducerInfoJob.ErrorIds.Dispose();
        PipeFlangeInfoJob.ErrorIds.Dispose();

        Debug.LogWarning($">>GetPipeInfosJob time:{DateTime.Now - start} lineJobs:{lineJobs.Count} elbowJobs:{elbowJobs.Count} teeJobs:{teeJobs.Count} reducerJobs:{reducerJobs.Count}");

        CreatePipeRunList();
    }

    public JobList<PipeLineInfoJob> GetPipeInfosJob_Line(List<Transform> ts,int offset)
    {
        int count = 0;
        JobList<PipeLineInfoJob> jobs = new JobList<PipeLineInfoJob>(JobSize);

        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            var rendererInfo = MeshRendererInfo.GetInfo(t.gameObject);
            Vector3[] vs = rendererInfo.GetVertices();
            //if (vs.Length == 0) continue;
            count++;
            PipeLineInfoJob job = new PipeLineInfoJob()
            {
                id = i+offset,
                points = new NativeArray<Vector3>(vs, Allocator.TempJob),
            };
            jobs.Add(job);
        }
        return jobs;
    }

    public JobList<PipeElbowInfoJob> GetPipeInfosJob_Elbow(List<Transform> ts, int offset)
    {
        int count = 0;
        JobList<PipeElbowInfoJob> jobs = new JobList<PipeElbowInfoJob>(JobSize);

        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            Mesh mesh = t.GetComponent<MeshFilter>().sharedMesh;
            MeshStructure meshS = new MeshStructure(mesh);
            //if (vs.Length == 0) continue;
            count++;
            PipeElbowInfoJob job = new PipeElbowInfoJob()
            {
                id = i + offset,
                mesh= meshS
            };
            jobs.Add(job);
        }
        return jobs;
    }

    public JobList<PipeFlangeInfoJob> GetPipeInfosJob_Flange(List<Transform> ts, int offset)
    {
        int count = 0;
        JobList<PipeFlangeInfoJob> jobs = new JobList<PipeFlangeInfoJob>(JobSize);

        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            Mesh mesh = t.GetComponent<MeshFilter>().sharedMesh;
            MeshStructure meshS = new MeshStructure(mesh);
            //if (vs.Length == 0) continue;
            count++;
            PipeFlangeInfoJob job = new PipeFlangeInfoJob()
            {
                id = i + offset,
                mesh = meshS
            };
            jobs.Add(job);
        }
        return jobs;
    }

    public JobList<PipeReducerInfoJob> GetPipeInfosJob_Reducer(List<Transform> ts, int offset)
    {
        int count = 0;
        JobList<PipeReducerInfoJob> jobs = new JobList<PipeReducerInfoJob>(JobSize);

        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            Mesh mesh = t.GetComponent<MeshFilter>().sharedMesh;
            MeshStructure meshS = new MeshStructure(mesh);
            //if (vs.Length == 0) continue;
            count++;
            PipeReducerInfoJob job = new PipeReducerInfoJob()
            {
                id = i + offset,
                mesh = meshS
            };
            jobs.Add(job);
        }
        return jobs;
    }

    public JobList<PipeTeeInfoJob> GetPipeInfosJob_Tee(List<Transform> ts, int offset)
    {
        int count = 0;
        JobList<PipeTeeInfoJob> jobs = new JobList<PipeTeeInfoJob>(JobSize);

        for (int i = 0; i < ts.Count; i++)
        {
            Transform t = ts[i];
            Mesh mesh = t.GetComponent<MeshFilter>().sharedMesh;
            MeshStructure meshS = new MeshStructure(mesh);
            //if (vs.Length == 0) continue;
            count++;
            PipeTeeInfoJob job = new PipeTeeInfoJob()
            {
                id = i + offset,
                mesh = meshS
            };
            jobs.Add(job);
        }
        return jobs;
    }

    public void GetPipeInfosEx()
    {
        DateTime start = DateTime.Now;

        PipeModels = new List<PipeModelBase>();
        int count = PipeLineGos.Count + PipeElbowGos.Count + PipeReducerGos.Count + PipeFlangeGos.Count+PipeTeeGos.Count;
        int id = 0;

        PipeLines = GetPipeModelInfos<PipeLineModel>(PipeLineGos, id, count, "Line");
        id += PipeLines.Count;
        AddPipeModelRange(PipeLines);

        PipeReducers = GetPipeModelInfos<PipeReducerModel>(PipeReducerGos, id, count, "Reducer");
        id += PipeReducers.Count;
        AddPipeModelRange(PipeReducers);

        PipeFlanges = GetPipeModelInfos<PipeFlangeModel>(PipeFlangeGos, id, count, "Flange");
        id += PipeFlanges.Count;
        AddPipeModelRange(PipeFlanges);

        PipeElbows = GetPipeModelInfos<PipeElbowModel>(PipeElbowGos, id, count, "Elbow");
        id += PipeElbows.Count;
        AddPipeModelRange(PipeElbows);

        PipeTees = GetPipeModelInfos<PipeTeeModel>(PipeTeeGos, id, count, "Tee");
        id += PipeTees.Count;
        AddPipeModelRange(PipeTees);

        PipeModels.Sort();

        Debug.LogWarning($">>GetPipeInfos ¡¾time:{DateTime.Now - start}¡¿ count:{PipeModels.Count}");

        CreatePipeRunList();

        
    }

    private List<T> GetPipeModelInfos<T>(List<Transform> gos,int id,int count,string tag) where T : PipeModelBase
    {
        var models = new List<T>();
        if (gos.Count == 0) return models;
        DateTime start14 = DateTime.Now;
        for (int i = 0; i < gos.Count; i++)
        {
            id++;
            Transform p = gos[i];
            if (p == null) continue;
            MeshFilter mf = p.GetComponent<MeshFilter>();
            if (mf == null) continue;
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg($"GetPipeInfos_{tag}", id, count, p)))
            {
                return models;
            }
            //CreatePipe(p);
            T pipeModel = GetPipeModelInfo<T>(p,true);
            models.Add(pipeModel);
            //AddPipeModel(pipeModel);
            //p.gameObject.SetActive(false);
        }
        Debug.Log($">>>GetPipeModelInfos time:{DateTime.Now - start14}");
        return models;
    }

    public void CreatePipeRunList()
    {
        if (IsCreatePipeRuns == false) return;
        Debug.Log($"CreatePipeRunList PipeModels:{PipeModels.Count}");
        pipeRunList = new PipeRunList(PipeModels, minConnectedDistance, false,isUniformRaidus,0.0001f);
    }

    public float minConnectedDistance = 0.0001f;

    public bool isUniformRaidus = false;

    public PipeRunList pipeRunList = new PipeRunList();

    public PipeRunList TestModelIsConnected(List<PipeModelBase> models)
    {
        PipeRunList runList = new PipeRunList(models, minConnectedDistance, true,isUniformRaidus, 0.0001f);
        return runList;
    }

    private void GetPipeElbowInfos()
    {
        DateTime start = DateTime.Now;
        PipeElbows = new List<PipeElbowModel>();
        for (int i = 0; i < PipeElbowGos.Count; i++)
        {
            Transform p = PipeElbowGos[i];
            if (p == null) continue;
            MeshFilter mf = p.GetComponent<MeshFilter>();
            if (mf == null) continue;
            PipeElbowModel pipeModel = GetPipeModelInfo<PipeElbowModel>(p,true);
            PipeElbows.Add(pipeModel);
            AddPipeModel(pipeModel);
            p.gameObject.SetActive(false);
        }
        Debug.LogError($">>>GetPipeElbowInfos time:{DateTime.Now - start}");
    }

    //private PipeElbowModel GetPipeElbowInfo(Transform elbow)
    //{
    //    PipeElbowModel pipeModel = elbow.GetComponent<PipeElbowModel>();
    //    if (pipeModel == null)
    //    {
    //        pipeModel = elbow.gameObject.AddComponent<PipeElbowModel>();
    //    }
    //    pipeModel.generateArg = this.generateArg;
    //    pipeModel.GetModelInfo();
    //    return pipeModel;
    //}

    //private PipeLineModel GetPipeLineInfo(Transform pipe)
    //{
    //    PipeLineModel pipeModel = pipe.GetComponent<PipeLineModel>();
    //    if (pipeModel == null)
    //    {
    //        pipeModel = pipe.gameObject.AddComponent<PipeLineModel>();
    //    }
    //    pipeModel.generateArg = this.generateArg;
    //    pipeModel.GetModelInfo();
    //    return pipeModel;
    //}

    private T GetPipeModelInfo<T>(Transform pipe,bool isGetInfo) where T :PipeModelBase
    {
        T pipeModel = pipe.GetComponent<T>();
        if (pipeModel == null)
        {
            pipeModel = pipe.gameObject.AddComponent<T>();
            pipeModel.VertexCount = (int)MeshRendererInfo.GetInfo(pipe.gameObject).vertexCount;
        }
        pipeModel.generateArg = this.generateArg;

        if (isGetInfo)
        {
            try
            {
                pipeModel.GetModelInfo();
            }
            catch (Exception ex)
            {
                Debug.LogError($"PipeBuilder.GetPipeModelInfo pipe:{pipe.name}  Exception:{ex.ToString()}");
            }
        }
        

        pipe.gameObject.SetActive(false);


        return pipeModel;
    }

    private void GetPipeLineInfos()
    {
        DateTime start = DateTime.Now;
        PipeLines = new List<PipeLineModel>();
        for (int i = 0; i < PipeLineGos.Count; i++)
        {
            Transform p = PipeLineGos[i];
            if (p == null) continue;
            MeshFilter mf = p.GetComponent<MeshFilter>();
            if (mf == null) continue;
            PipeLineModel pipeModel = GetPipeModelInfo<PipeLineModel>(p,true);
            PipeLines.Add(pipeModel);
            AddPipeModel(pipeModel);
            p.gameObject.SetActive(false);
        }
        Debug.LogError($">>>GetPipeModelInfos time:{DateTime.Now - start}");
    }



    //public void CreatePipe(GameObject go)
    //{
    //    ClearChildren();

    //    OBBCollider oBBCollider = go.GetComponent<OBBCollider>();
    //    if (oBBCollider == null)
    //    {
    //        oBBCollider = go.AddComponent<OBBCollider>();
    //        oBBCollider.ShowObbInfo();
    //    }
    //    OBB = oBBCollider.OBB;

    //    StartPoint = OBB.Up * OBB.Extent.y;
    //    EndPoint = -OBB.Up * OBB.Extent.y;

    //    CreatePoint(StartPoint, "StartPoint");
    //    CreatePoint(EndPoint, "EndPoint");

    //    P1 = OBB.Right * OBB.Extent.x;
    //    P2 = -OBB.Forward * OBB.Extent.z;
    //    P3 = -OBB.Right * OBB.Extent.x;
    //    P4 = OBB.Forward * OBB.Extent.z;

    //    CreatePoint(P1, "P1");
    //    CreatePoint(P2, "P2");

    //    GameObject pipeNew = new GameObject(go.name + "_NewPipe");
    //    pipeNew.transform.position = go.transform.position + Offset;
    //    pipeNew.transform.SetParent(go.transform.parent);

    //    PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
    //    if (pipe == null)
    //    {
    //        pipe = pipeNew.AddComponent<PipeMeshGenerator>();
    //    }
    //    pipe.points = new List<Vector3>() { StartPoint, EndPoint };
    //    pipe.pipeSegments = pipeSegments;
    //    pipe.pipeMaterial = this.PipeMaterial;
    //    pipe.weldMaterial = this.WeldMaterial;
    //    pipe.weldRadius = this.weldRadius;
    //    pipe.generateWeld = true;
    //    pipe.pipeRadius = OBB.Extent.x;
    //    pipe.RenderPipe();
    //}

    //public void CreateWeld()
    //{
    //    ClearChildren();

    //    OBBCollider oBBCollider = this.gameObject.GetComponent<OBBCollider>();
    //    if (oBBCollider == null)
    //    {
    //        oBBCollider = this.gameObject.AddComponent<OBBCollider>();
    //        oBBCollider.ShowObbInfo();
    //    }
    //    OBB = oBBCollider.OBB;

    //    StartPoint = OBB.Up * OBB.Extent.y;
    //    EndPoint = -OBB.Up * OBB.Extent.y;

    //    CreatePoint(StartPoint, "StartPoint");
    //    CreatePoint(EndPoint, "EndPoint");

    //    P1 = OBB.Right * OBB.Extent.x;
    //    P2 = -OBB.Forward * OBB.Extent.z;
    //    P3 = -OBB.Right * OBB.Extent.x;
    //    P4 = OBB.Forward * OBB.Extent.z;

    //    CreatePoint(P1, "P1");
    //    CreatePoint(P2, "P2");
    //    CreatePoint(P3, "P3");
    //    CreatePoint(P4, "P4");

    //    float p = 1.414213562373f;
    //    CreatePoint(P1 * p, "P11");
    //    CreatePoint(P2 * p, "P22");
    //    CreatePoint(P3 * p, "P33");
    //    CreatePoint(P4 * p, "P44");

    //    GameObject pipeNew = new GameObject(this.name + "_NewWeld");
    //    pipeNew.transform.position = this.transform.position + Offset;
    //    pipeNew.transform.SetParent(this.transform.parent);

    //    PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
    //    if (pipe == null)
    //    {
    //        pipe = pipeNew.AddComponent<PipeMeshGenerator>();
    //    }
    //    pipe.points = new List<Vector3>() { P1, P2, P3, P4 };
    //    pipe.pipeSegments = pipeSegments;
    //    pipe.pipeMaterial = this.PipeMaterial;
    //    pipe.weldMaterial = this.WeldMaterial;
    //    pipe.weldRadius = this.weldRadius;
    //    pipe.elbowRadius = Vector3.Distance(P1, P2) / 2;
    //    pipe.IsLinkEndStart = true;
    //    pipe.generateWeld = false;
    //    pipe.pipeRadius = this.weldRadius;
    //    pipe.RenderPipe();
    //}

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.gameObject == this.gameObject) continue;
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
