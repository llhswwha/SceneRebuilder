using MathGeoLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBuilder : MonoBehaviour
{
    public List<Transform> PipeModelGos = new List<Transform>();

    public List<PipeLineModel> PipeModels = new List<PipeLineModel>();

    public List<Transform> PipeElbowsGos = new List<Transform>();

    public List<PipeElbowModel> PipeElbows = new List<PipeElbowModel>();


    public List<Transform> NewPipeGos = new List<Transform>();

    public void ShowOBB()
    {
        OBBCollider oBB = this.gameObject.GetComponent<OBBCollider>();
        if (oBB == null)
        {
            oBB = this.gameObject.AddComponent<OBBCollider>();
        }
        if (oBB != null)
        {
            oBB.ShowObbInfo();
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

    public void CreateEachPipes()
    {
        GetPipeInfos();

        ClearOldGos();
        RendererPipes();
        RendererElbows();
    }

    public void CreateElbows()
    {
        GetPipeInfos();
    }

    public List<Vector3> points;

    public PipeMeshGenerator CreateOnePipe()
    {
        GetPipeInfos();
        ClearOldGos();
        var newPipe = RendererOnePipe();
        NewPipeGos.Add(newPipe.transform);
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

    public bool UseOnlyEndPoint = false;

    private PipeMeshGenerator RendererOnePipe()
    {
        PipeArgs = new PipeCreateArgs(PipeModels);
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
        pipe.pipeMaterial = generateArg.PipeMaterial;
        pipe.weldMaterial = generateArg.WeldMaterial;
        pipe.weldRadius = generateArg.weldRadius;

        pipe.elbowRadius = PipeArgs.elbowRadius;//Elbow
        pipe.IsGenerateElbowBeforeAfter = IsGenerateElbowBeforeAfter;
        pipe.generateWeld = generateArg.generateWeld;

        pipe.avoidStrangling = true;
        pipe.pipeRadius = PipeModels[0].PipeRadius;
        pipe.RenderPipe();
        return pipe;
    }


    public void RendererPipes()
    {
        //ClearOldGos();
        foreach (var go in PipeModels)
        {
            NewPipeGos.Add(go.RendererPipe(this.generateArg).transform);
        }
    }

    public void RendererElbows()
    {
        //ClearOldGos();

        foreach (var go in PipeElbows)
        {
            NewPipeGos.Add(go.RendererPipe(this.generateArg,false).transform);
        }
    }

    private void ClearOldGos()
    {
        foreach (var go in NewPipeGos)
        {
            if (go == null) continue;
            GameObject.DestroyImmediate(go);
        }
        NewPipeGos.Clear();
    }

    public void GetPipeInfos()
    {
        GetPipeModels();
        GetPipeElbows();
    }

    private void GetPipeElbows()
    {
        PipeElbows = new List<PipeElbowModel>();
        foreach (var p in PipeElbowsGos)
        {
            if (p == null) continue;
            //CreatePipe(p);
            PipeElbowModel pipeModel = p.GetComponent<PipeElbowModel>();
            if (pipeModel == null)
            {
                pipeModel = p.gameObject.AddComponent<PipeElbowModel>();
            }
            //pipeModel.generateWeld = this.generateWeld;
            pipeModel.GetElbowInfo();
            PipeElbows.Add(pipeModel);
            p.gameObject.SetActive(false);
        }
    }

    private void GetPipeModels()
    {
        PipeModels = new List<PipeLineModel>();
        foreach (var p in PipeModelGos)
        {
            if (p == null) continue;
            //CreatePipe(p);
            PipeLineModel pipeModel = p.GetComponent<PipeLineModel>();
            if (pipeModel == null)
            {
                pipeModel = p.gameObject.AddComponent<PipeLineModel>();
            }
            pipeModel.generateArg = this.generateArg;
            pipeModel.GetPipeInfo();
            PipeModels.Add(pipeModel);

            p.gameObject.SetActive(false);
        }
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
