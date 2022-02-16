using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PipeCreateArg
{
    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;
    public Vector3 P5;

    PipeLineInfo pm1;
    PipeLineInfo pm2;

    public PipeCreateArg(PipeLineInfo pm1, PipeLineInfo pm2)
    {
        this.pm1 = pm1;
        this.pm2 = pm2;
    }

    public List<Vector3> points = new List<Vector3>();

    public List<Vector4> points4 = new List<Vector4>();

    public float elbowRadius = 0;

    //public List<Vector4> GetPoints4()
    //{
    //    points4 = new List<Vector4>();

    //    Vector4 p11;
    //    Vector4 p12;
    //    Vector4 p21;
    //    Vector4 p22;

    //    p11 = pm1.GetStartPoint();
    //    p12 = pm1.GetEndPoint();

    //    p21 = pm2.GetStartPoint();
    //    p22 = pm2.GetEndPoint();

    //    Vector3 linePoint1 = Vector3.zero;
    //    Vector3 lineVec1 = Vector3.zero;


    //    float dis11 = Vector3.Distance(p11, p21) + Vector3.Distance(p11, p22);
    //    float dis12 = Vector3.Distance(p12, p21) + Vector3.Distance(p12, p22);
    //    if (dis11 > dis12)
    //    {
    //        //points.Add(p11);
    //        //points.Add(p12);
    //        linePoint1 = p11;
    //        lineVec1 = p12 - p11;

    //        P1 = p11;
    //        P2 = p12;
    //    }
    //    else
    //    {
    //        //points.Add(p12);
    //        //points.Add(p11);
    //        linePoint1 = p12;
    //        lineVec1 = p11 - p12;

    //        P1 = p12;
    //        P2 = p11;
    //    }

    //    {
    //        points4.Add(P1);
    //        points4.Add(P2);
    //    }

    //    Vector3 linePoint2 = Vector3.zero;
    //    Vector3 lineVec2 = Vector3.zero;

    //    float dis21 = Vector3.Distance(p11, p21) + Vector3.Distance(p12, p21);
    //    float dis22 = Vector3.Distance(p11, p22) + Vector3.Distance(p12, p22);
    //    if (dis21 > dis22)
    //    {
    //        linePoint2 = p21;
    //        lineVec2 = p22 - p21;

    //        P3 = p22; ;
    //        P4 = p21;
    //    }
    //    else
    //    {
    //        linePoint2 = p22;
    //        lineVec2 = p21 - p22;

    //        P3 = p21;
    //        P4 = p22;
    //    }

    //    Math3D.ClosestPointsOnTwoLines(out closestPointLine1, out closestPointLine2, linePoint1, lineVec1, linePoint2, lineVec2);
    //    distanceOfTwoClosetPoint = Vector3.Distance(closestPointLine1, closestPointLine2);
    //    Vector3 closetPoint12 = (closestPointLine1 + closestPointLine2) / 2;
    //    elbowRadius = Vector3.Distance(closetPoint12, P2);
    //    points4.Add(closetPoint12);
    //    points4.Add(P3);
    //    points4.Add(P4);
    //    return points4;
    //}

    public List<Vector3> GetPoints()
    {
        points = new List<Vector3>();

        Vector3 p11;
        Vector3 p12;
        Vector3 p21;
        Vector3 p22;

        //Vector3 P1;
        //Vector3 P2;
        //Vector3 P3;
        //Vector3 P4;
        //Vector3 P5;



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

        //if (i == 0)
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

        Math3D.ClosestPointsOnTwoLines(out closestPointLine1, out closestPointLine2, linePoint1, lineVec1, linePoint2, lineVec2);
        distanceOfTwoClosetPoint = Vector3.Distance(closestPointLine1, closestPointLine2);
        Vector3 closetPoint12 = (closestPointLine1 + closestPointLine2) / 2;

        //if (i == 0)
        elbowRadius = Vector3.Distance(closetPoint12, P2);

        //var P2_1 = P2 + (closetPoint12 - P2) * ElbowOffset;

        //points.Add(P2_1);

        //points.Add(closestPointLine1);
        points.Add(closetPoint12);

        //var P3_1 = P3 + (closetPoint12 - P3) * ElbowOffset;

        points.Add(P3);
        points.Add(P4);

        ////NewPipeGos.Add(go.RendererPipe(this.PipeMaterial, this.WeldMaterial));
        //points.Add(p11);
        //points.Add(p12);

        //sumP += p11;
        //sumP += p12;
        return points;
    }

    public List<Vector3> GetGeneratePoints(int i, int pipeCount, bool useOnlyEndPoint)
    {
        List<Vector3> points = new List<Vector3>();
        var ps = this.GetPoints();
        if (useOnlyEndPoint)
        {
            if (i == 0)
            {
                points.Add(ps[0]);
                points.Add(ps[2]);
            }
            else if (i == pipeCount - 2)
            {
                points.Add(ps[2]);
                points.Add(ps[4]);
            }
            else
            {
                points.Add(ps[2]);
            }
        }
        else
        {
            if (i == 0)
            {
                points.AddRange(ps);
            }
            //else if (i == PipeModels.Count - 2)
            //{
            //    points.Add(ps[2]);
            //    points.Add(ps[3]);
            //    points.Add(ps[4]);
            //}
            else
            {
                points.Add(ps[2]);
                points.Add(ps[3]);
                points.Add(ps[4]);
            }
        }
        return points;
    }


    public Vector3 closetPoint12 = Vector3.zero;
    public Vector3 closestPointLine1 = Vector3.zero;
    public Vector3 closestPointLine2 = Vector3.zero;
    public float distanceOfTwoClosetPoint = 0;
}

