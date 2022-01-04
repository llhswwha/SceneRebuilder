using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBuilder : MonoBehaviour
{
    public List<Transform> PipeLineGos = new List<Transform>();

    public List<PipeLineModel> PipeLines = new List<PipeLineModel>();

    public List<Transform> PipeElbowsGos = new List<Transform>();

    public List<PipeElbowModel> PipeElbows = new List<PipeElbowModel>();

    public List<Transform> PipeReducerGos = new List<Transform>();

    public List<PipeReducerModel> PipeReducers = new List<PipeReducerModel>();

    public List<Transform> PipeFlangeGos = new List<Transform>();

    public List<PipeFlangeModel> PipeFlanges = new List<PipeFlangeModel>();

    public List<Transform> PipeTeeGos = new List<Transform>();

    public List<PipeTeeModel> PipeTees = new List<PipeTeeModel>();

    public List<PipeModelBase> PipeModels = new List<PipeModelBase>();

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

        Debug.LogError($">>CheckResults time:{DateTime.Now - start}");
    }

    public string NewObjName = "_New";

    public void RendererPipesEx()
    {
        DateTime start = DateTime.Now;
        //int count = PipeLines.Count + PipeElbows.Count + PipeReducers.Count + PipeFlanges.Count;
        //int id = 0;
        //DateTime start1 = DateTime.Now;
        //for (int i = 0; i < PipeLines.Count; i++)
        //{
        //    id++;
        //    PipeLineModel go = PipeLines[i];
        //    if (go == null) continue;
        //    if(ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("RendererPipesEx1",id,count,go)))
        //    {
        //        return;
        //    }
        //    GameObject pipe = go.RendererModel(this.generateArg, NewObjName);
        //    NewPipeList.Add(pipe.transform);
        //}
        //Debug.LogError($">>>RendererPipeLines time:{DateTime.Now - start1}");

        //DateTime start12 = DateTime.Now;
        //for (int i = 0; i < PipeReducers.Count; i++)
        //{
        //    id++;
        //    PipeReducerModel go = PipeReducers[i];
        //    if (go == null) continue;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("RendererPipesEx2", id, count, go)))
        //    {
        //        return;
        //    }
        //    GameObject pipe = go.RendererModel(this.generateArg, NewObjName);
        //    NewPipeList.Add(pipe.transform);
        //}
        //Debug.LogError($">>>RendererPipeReducers time:{DateTime.Now - start12}");

        //DateTime start13 = DateTime.Now;
        //for (int i = 0; i < PipeFlanges.Count; i++)
        //{
        //    id++;
        //    PipeFlangeModel go = PipeFlanges[i];
        //    if (go == null) continue;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("RendererPipesEx3", id, count, go)))
        //    {
        //        return;
        //    }
        //    GameObject pipe = go.RendererModel(this.generateArg, NewObjName);
        //    NewPipeList.Add(pipe.transform);
        //}
        //Debug.LogError($">>>RendererPipeFlanges time:{DateTime.Now - start13}");

        //DateTime start2 = DateTime.Now;
        //for (int i = 0; i < PipeElbows.Count; i++)
        //{
        //    id++;
        //    PipeElbowModel go = PipeElbows[i];
        //    if (go == null) continue;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("RendererPipesEx4", id, count, go)))
        //    {
        //        return;
        //    }
        //    GameObject pipe = go.RendererModel(this.generateArg, NewObjName);
        //    NewPipeList.Add(pipe.transform);
        //}
        //Debug.LogError($">>>RendererPipeElbows time:{DateTime.Now - start2}");

        for (int i = 0; i < PipeModels.Count; i++)
        {
            PipeModelBase go = PipeModels[i];
            if (go == null) continue;
            if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("RendererPipes", i, PipeModels.Count, go)))
            {
                return;
            }
            GameObject pipe = go.RendererModel(this.generateArg, NewObjName);
            NewPipeList.Add(pipe.transform);
        }

        if (pipeRunList != null)
        {
            pipeRunList.RenameResultBySortedId();
        }

        Debug.LogError($">>RendererPipes time:{DateTime.Now - start}");
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


    public void RendererPipeLines()
    {
        DateTime start = DateTime.Now;
        //ClearOldGos();
        for (int i = 0; i < PipeLines.Count; i++)
        {
            PipeLineModel go = PipeLines[i];
            GameObject pipe = go.RendererModel(this.generateArg, NewObjName);
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
            GameObject pipe = go.RendererModel(this.generateArg,NewObjName);
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

    public void GetPipeInfosEx()
    {
        DateTime start = DateTime.Now;

        PipeModels = new List<PipeModelBase>();
        int count = PipeLineGos.Count + PipeElbowsGos.Count + PipeReducerGos.Count + PipeFlangeGos.Count+PipeTeeGos.Count;
        int id = 0;

        //DateTime start1 = DateTime.Now;
        //PipeLines = new List<PipeLineModel>();
        //for (int i = 0; i < PipeLineGos.Count; i++)
        //{
        //    id++;
        //    Transform p = PipeLineGos[i];
        //    if (p == null) continue;
        //    MeshFilter mf = p.GetComponent<MeshFilter>();
        //    if (mf == null) continue;
        //    if(ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPipeInfosEx1",id,count,p)))
        //    {
        //        return;
        //    }
        //    //CreatePipe(p);
        //    PipeLineModel pipeModel = GetPipeModelInfo<PipeLineModel>(p);
        //    PipeLines.Add(pipeModel);
        //    PipeModels.Add(pipeModel);
        //    p.gameObject.SetActive(false);
        //}
        //Debug.LogError($">>>GetPipeLineModelInfos time:{DateTime.Now - start1}");

        PipeLines = GetPipeModelInfos<PipeLineModel>(PipeLineGos, id, count, "Line");
        id += PipeLines.Count;
        PipeModels.AddRange(PipeLines);

        //DateTime start12 = DateTime.Now;
        //PipeReducers = new List<PipeReducerModel>();
        //for (int i = 0; i < PipeReducerGos.Count; i++)
        //{
        //    id++;
        //    Transform p = PipeReducerGos[i];
        //    if (p == null) continue;
        //    MeshFilter mf = p.GetComponent<MeshFilter>();
        //    if (mf == null) continue;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPipeInfosEx2", id, count, p)))
        //    {
        //        return;
        //    }
        //    //CreatePipe(p);
        //    PipeReducerModel pipeModel = GetPipeModelInfo<PipeReducerModel>(p);
        //    PipeReducers.Add(pipeModel);
        //    PipeModels.Add(pipeModel);
        //    p.gameObject.SetActive(false);
        //}
        //Debug.LogError($">>>GetPipeReducerModelInfos time:{DateTime.Now - start12}");

        PipeReducers = GetPipeModelInfos<PipeReducerModel>(PipeReducerGos, id, count, "Reducer");
        id += PipeReducers.Count;
        PipeModels.AddRange(PipeReducers);

        //DateTime start13 = DateTime.Now;
        //PipeFlanges = new List<PipeFlangeModel>();
        //for (int i = 0; i < PipeFlangeGos.Count; i++)
        //{
        //    id++;
        //    Transform p = PipeFlangeGos[i];
        //    if (p == null) continue;
        //    MeshFilter mf = p.GetComponent<MeshFilter>();
        //    if (mf == null) continue;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPipeInfosEx3", id, count, p)))
        //    {
        //        return;
        //    }
        //    //CreatePipe(p);
        //    PipeFlangeModel pipeModel = GetPipeModelInfo<PipeFlangeModel>(p);
        //    PipeFlanges.Add(pipeModel);
        //    PipeModels.Add(pipeModel);
        //    p.gameObject.SetActive(false);
        //}
        //Debug.LogError($">>>GetPipeFlangeModelInfos time:{DateTime.Now - start13}");

        PipeFlanges = GetPipeModelInfos<PipeFlangeModel>(PipeFlangeGos, id, count, "Flange");
        id += PipeFlanges.Count;
        PipeModels.AddRange(PipeFlanges);

        //DateTime start2 = DateTime.Now;
        //PipeElbows = new List<PipeElbowModel>();
        //for (int i = 0; i < PipeElbowsGos.Count; i++)
        //{
        //    id++;
        //    Transform p = PipeElbowsGos[i];
        //    if (p == null) continue;
        //    MeshFilter mf = p.GetComponent<MeshFilter>();
        //    if (mf == null) continue;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPipeInfosEx4", id, count, p)))
        //    {
        //        return;
        //    }
        //    //CreatePipe(p);
        //    PipeElbowModel pipeModel = GetPipeModelInfo<PipeElbowModel>(p);
        //    PipeElbows.Add(pipeModel);
        //    PipeModels.Add(pipeModel);
        //    p.gameObject.SetActive(false);
        //}
        //Debug.LogError($">>>GetPipeElbowInfos time:{DateTime.Now - start2}");

        PipeElbows = GetPipeModelInfos<PipeElbowModel>(PipeElbowsGos, id, count, "Elbow");
        id += PipeElbows.Count;
        PipeModels.AddRange(PipeElbows);

        //DateTime start14 = DateTime.Now;
        //PipeTees = new List<PipeTeeModel>();
        //for (int i = 0; i < PipeTeeGos.Count; i++)
        //{
        //    id++;
        //    Transform p = PipeTeeGos[i];
        //    if (p == null) continue;
        //    MeshFilter mf = p.GetComponent<MeshFilter>();
        //    if (mf == null) continue;
        //    if (ProgressBarHelper.DisplayCancelableProgressBar(new ProgressArg("GetPipeInfosEx3", id, count, p)))
        //    {
        //        return;
        //    }
        //    //CreatePipe(p);
        //    PipeTeeModel pipeModel = GetPipeModelInfo<PipeTeeModel>(p);
        //    PipeTees.Add(pipeModel);
        //    PipeModels.Add(pipeModel);
        //    p.gameObject.SetActive(false);
        //}
        //Debug.LogError($">>>GetPipeTeeModelInfos time:{DateTime.Now - start14}");

        PipeTees = GetPipeModelInfos<PipeTeeModel>(PipeTeeGos, id, count, "Tee");
        id += PipeTees.Count;
        PipeModels.AddRange(PipeTees);

        PipeModels.Sort();

        CreatePipeRunList();

        Debug.LogError($">>GetPipeInfos time:{DateTime.Now - start}");
    }

    private List<T> GetPipeModelInfos<T>(List<Transform> gos,int id,int count,string tag) where T : PipeModelBase
    {
        DateTime start14 = DateTime.Now;
        var models = new List<T>();
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
            T pipeModel = GetPipeModelInfo<T>(p);
            models.Add(pipeModel);
            //PipeModels.Add(pipeModel);
            //p.gameObject.SetActive(false);
        }
        Debug.LogError($">>>GetPipeTeeModelInfos time:{DateTime.Now - start14}");
        return models;
    }

    public void CreatePipeRunList()
    {
        pipeRunList = new PipeRunList(PipeModels, minConnectedDistance, isUniformRaidus);
    }

    public float minConnectedDistance = 0.0001f;

    public bool isUniformRaidus = false;

    public PipeRunList pipeRunList = new PipeRunList();

    private void GetPipeElbowInfos()
    {
        DateTime start = DateTime.Now;
        PipeElbows = new List<PipeElbowModel>();
        for (int i = 0; i < PipeElbowsGos.Count; i++)
        {
            Transform p = PipeElbowsGos[i];
            if (p == null) continue;
            MeshFilter mf = p.GetComponent<MeshFilter>();
            if (mf == null) continue;
            PipeElbowModel pipeModel = GetPipeModelInfo<PipeElbowModel>(p);
            PipeElbows.Add(pipeModel);
            PipeModels.Add(pipeModel);
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

    private T GetPipeModelInfo<T>(Transform pipe) where T :PipeModelBase
    {
        T pipeModel = pipe.GetComponent<T>();
        if (pipeModel == null)
        {
            pipeModel = pipe.gameObject.AddComponent<T>();
        }
        pipeModel.generateArg = this.generateArg;
        try
        {
            pipeModel.GetModelInfo();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
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
            PipeLineModel pipeModel = GetPipeModelInfo<PipeLineModel>(p);
            PipeLines.Add(pipeModel);
            PipeModels.Add(pipeModel);
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
