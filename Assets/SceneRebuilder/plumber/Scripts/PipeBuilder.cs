using MathGeoLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBuilder : MonoBehaviour
{
    public List<GameObject> PipeGos = new List<GameObject>();

    public List<PipeModel> PipeModels = new List<PipeModel>();

    public List<GameObject> NewPipeGos = new List<GameObject>();

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

    public Material PipeMaterial;

    public Material WeldMaterial;

    public int pipeSegments = 24;

    public float weldRadius = 0.005f;

    public Vector3 Offset = Vector3.zero;

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
        RendererPipes();
    }

    public List<Vector3> points;

    public void CreateOnePipe()
    {
        GetPipeInfos();

        ClearOldGos();
        NewPipeGos.Add(RendererOnePipe());

        //return pipeNew;
    }

    public bool generateWeld = false;

    private GameObject RendererOnePipe()
    {
        points = new List<Vector3>();
        
        Vector3 p11;
        Vector3 p12;
        Vector3 p21;
        Vector3 p22;

        Vector3 P1;
        Vector3 P2;
        Vector3 P3;
        Vector3 P4;
        Vector3 P5;

        float elbowRadius = 0;

        for (int i = 0; i < PipeModels.Count-1; i++)
        {
            PipeModel pm1 = PipeModels[i];
            PipeModel pm2 = PipeModels[i+1];

            p11 = pm1.GetStartPoint();
            p12 = pm1.GetEndPoint();

            p21 = pm2.GetStartPoint();
            p22 = pm2.GetEndPoint();

            Vector3 linePoint1 = Vector3.zero;
            Vector3 lineVec1 = Vector3.zero;
            

            float dis11 = Vector3.Distance(p11, p21) + Vector3.Distance(p11, p22);
            float dis12 = Vector3.Distance(p12, p21) + Vector3.Distance(p12, p22);
            if (dis11 > dis12)
            {
                //points.Add(p11);
                //points.Add(p12);
                linePoint1 = p11;
                lineVec1 = p12 - p11;

                P1 = p11;
                P2 = p12;
            }
            else
            {
                //points.Add(p12);
                //points.Add(p11);
                linePoint1 = p12;
                lineVec1 = p11 - p12;

                P1 = p12;
                P2 = p11;
            }

            if (i == 0)
            {
                points.Add(P1);
                points.Add(P2);

                //Vector3 line1 = P2 - P1;
                //Vector3 P21 = P1 + line1*1.05f;
                //P2 = P21;
                //points.Add(P21);
            }

            

            Vector3 linePoint2 = Vector3.zero;
            Vector3 lineVec2 = Vector3.zero;

            float dis21 = Vector3.Distance(p11, p21) + Vector3.Distance(p12, p21);
            float dis22 = Vector3.Distance(p11, p22) + Vector3.Distance(p12, p22);
            if (dis21 > dis22)
            {
                //points.Add(p22);
                //points.Add(p21);
                linePoint2 = p21;
                lineVec2 = p22 - p21;

                P3 = p22; ;
                P4 = p21;
            }
            else
            {
                //points.Add(p21);
                //points.Add(p22);
                linePoint2 = p22;
                lineVec2 = p21 - p22;

                P3 = p21;
                P4 = p22;
            }

            Vector3 closestPointLine1 = Vector3.zero;
            Vector3 closestPointLine2 = Vector3.zero;
            Math3D.ClosestPointsOnTwoLines(out closestPointLine1, out closestPointLine2, linePoint1, lineVec1, linePoint2, lineVec2);

            if(i==0)
                elbowRadius = Vector3.Distance(closestPointLine1, P2);

            //points.Add(closestPointLine1);
            points.Add(closestPointLine2);

            points.Add(P3);
            points.Add(P4);

            ////NewPipeGos.Add(go.RendererPipe(this.PipeMaterial, this.WeldMaterial));
            //points.Add(p11);
            //points.Add(p12);

            //sumP += p11;
            //sumP += p12;
        }


        Vector3 sumP = Vector3.zero;
        for (int i = 0; i < points.Count; i++)
        {
            sumP += points[i];
        }

        Vector3 centerP = sumP / (PipeModels.Count * 2);
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            points[i] = p - centerP;
        }

        GameObject pipeNew = new GameObject(this.name + "_NewPipe");
        //pipeNew.transform.position = this.transform.position + Offset;
        //pipeNew.transform.position = Vector3.zero;
        pipeNew.transform.position = centerP;
        pipeNew.transform.SetParent(this.transform.parent);

        PipeMeshGenerator pipe = pipeNew.GetComponent<PipeMeshGenerator>();
        if (pipe == null)
        {
            pipe = pipeNew.AddComponent<PipeMeshGenerator>();
        }
        pipe.points = points;
        pipe.pipeSegments = pipeSegments;
        pipe.pipeMaterial = PipeMaterial;
        pipe.weldMaterial = WeldMaterial;
        pipe.weldRadius = this.weldRadius;
        pipe.elbowRadius = elbowRadius;
        pipe.generateWeld = generateWeld;
        pipe.avoidStrangling = true;
        pipe.pipeRadius = PipeModels[0].PipeRadius;
        pipe.RenderPipe();

        return pipeNew;
    }

    public void RendererPipes()
    {
        ClearOldGos();
        foreach (var go in PipeModels)
        {
            NewPipeGos.Add(go.RendererPipe(this.PipeMaterial,this.WeldMaterial));
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
        PipeModels = new List<PipeModel>();
        foreach (var p in PipeGos)
        {
            if (p == null) continue;
            //CreatePipe(p);
            PipeModel pipeModel = p.GetComponent<PipeModel>();
            if (pipeModel == null)
            {
                pipeModel = p.AddComponent<PipeModel>();
            }
            pipeModel.generateWeld = this.generateWeld;
            pipeModel.GetPipeInfo();
            PipeModels.Add(pipeModel);

            p.SetActive(false);
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