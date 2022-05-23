using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGeneratorHelper 
{
    public static CircleMeshArg GenerateCircleAtPoint(Vector3 center, Vector3 direction, int pipeSegments, float pipeRadius)
    {

        CircleMeshArg generator = new CircleMeshArg(center, direction, pipeSegments, pipeRadius);
        generator.Generate();
        return generator;
    }

    public static void OrthoNormalize(ref Vector3 direction, ref Vector3 tangent, ref Vector3 binormal, string name, int index, int psCount, int circleType)
    {
        Vector3 dir0 = direction;
        float dis = direction.magnitude;
        direction = direction * 100;
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Vector3 xAxis1 = Vector3.up;
        Vector3 yAxis1 = Vector3.right;
        if (p.GetSide(direction))
        {
            yAxis1 = Vector3.left;
        }



        // build left-hand coordinate system, with orthogonal and normalized axes
        Vector3.OrthoNormalize(ref direction, ref xAxis1, ref yAxis1);

        if (
            //circleType == 0 && 
            psCount > 1 && psCount < 5)// 2 3 4
        {
            Vector3 xAxis2 = Vector3.up;
            Vector3 yAxis2 = Vector3.right;
            if (p.GetSide(direction))
            {
                yAxis2 = Vector3.left;
            }
            Vector3.OrthoNormalize(ref direction, ref yAxis2, ref xAxis2);
            //[{Vector3.Dot(direction, xAxis)},{Vector3.Dot(direction, yAxis)},{Vector3.Dot(xAxis, yAxis)}]
            //float minDot = 0.0002f;
            float minDot = 0.001f;
            float minDot2 = 0.00001f;
            float dot1 = Mathf.Abs(Vector3.Dot(direction, xAxis1));
            float dot2 = Mathf.Abs(Vector3.Dot(direction, yAxis1));
            float dot3 = Mathf.Abs(Vector3.Dot(xAxis1, yAxis1));
            List<float> dotList1 = new List<float>() { dot1, dot2, dot3 };
            dotList1.Sort();
            if (dot1 > minDot || dot2 > minDot || dot3 > minDot)
            {
                float dot11 = Mathf.Abs(Vector3.Dot(direction, xAxis2));
                float dot22 = Mathf.Abs(Vector3.Dot(direction, yAxis2));
                float dot33 = Mathf.Abs(Vector3.Dot(xAxis2, yAxis2));

                if (dot11 > minDot || dot22 > minDot || dot33 > minDot)
                {
                    string errorLog = $"OrthoNormalize Error!(1:NotChange)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount})direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
                    errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
                    errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
                    Debug.Log(errorLog);
                }
                else
                {
                    List<float> dotList2 = new List<float>() { dot11, dot22, dot33 };
                    dotList2.Sort();

                    if (dotList1[2] > dotList2[2])
                    {
                        if (dotList2[2] < minDot2)
                        {

                            xAxis1 = yAxis2;
                            yAxis1 = xAxis2;

                            if (dotList2[2] < 0.000001f)
                            {
                                //不需要打印
                            }
                            else
                            {
                                string errorLog = $"OrthoNormalize Error! (2:Change)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
                                errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
                                errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
                                Debug.LogWarning(errorLog);
                            }
                        }
                        else
                        {
                            string errorLog = $"OrthoNormalize Error!(3:NotChange)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
                            errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
                            errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
                            Debug.Log(errorLog);
                        }
                    }
                    else
                    {
                        string errorLog = $"OrthoNormalize Error!(4:NotChange)\tindex:{index} type:{circleType}【minDot:{minDot}|{dot1},{dot2},{dot3}|{dot11},{dot22},{dot33}】gameObject:{name}({psCount}) direction:({direction.x},{direction.y},{direction.z}) dis:{dis}";
                        errorLog += $"\nxAxis:({xAxis1.x},{xAxis1.y},{xAxis1.z}) yAxis:({yAxis1.x},{yAxis1.y},{yAxis1.z})【{dot1},{dot2},{dot3}】";
                        errorLog += $"\nxAxis:({xAxis2.x},{xAxis2.y},{xAxis2.z}) yAxis:({yAxis2.x},{yAxis2.y},{yAxis2.z})【{dot11},{dot22},{dot33}】";
                        Debug.Log(errorLog);

                        //xAxis1 = yAxis2;
                        //yAxis1 = xAxis2;
                    }
                }



            }
        }


        tangent = xAxis1;
        binormal = yAxis1;
    }
}

public class CircleMeshArg
{
    public List<Vector3> vertices;//out
    public List<Vector3> normals;//out
    public Vector3 center; //in
    public Vector3 direction;//in

    public int pipeSegments;
    public float pipeRadius;

    public CircleMeshArg(Vector3 center, Vector3 direction, int pipeSegments, float pipeRadius)
    {
        this.center = center;
        this.direction = direction;
        this.pipeSegments = pipeSegments;
        this.pipeRadius = pipeRadius;
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
    }



    public List<Vector3> Generate()
    {

        List<Vector3> newVertics = new List<Vector3>();
        // 'direction' is the normal to the plane that contains the circle

        // define a couple of utility variables to build circles
        float twoPi = Mathf.PI * 2;
        float radiansPerSegment = twoPi / pipeSegments;

        // generate two axes that define the plane with normal 'direction'
        // we use a plane to determine which direction we are moving in order
        // to ensure we are always using a left-hand coordinate system
        // otherwise, the triangles will be built in the wrong order and
        // all normals will end up inverted!
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Vector3 xAxis = Vector3.up;
        Vector3 yAxis = Vector3.right;
        if (p.GetSide(direction))
        {
            yAxis = Vector3.left;
        }

        // build left-hand coordinate system, with orthogonal and normalized axes
        MeshGeneratorHelper.OrthoNormalize(ref direction, ref xAxis, ref yAxis,"",0,0,0);

        for (int i = 0; i < pipeSegments; i++)
        {
            Vector3 currentVertex =
                center +
                (pipeRadius * Mathf.Cos(radiansPerSegment * i) * xAxis) +
                (pipeRadius * Mathf.Sin(radiansPerSegment * i) * yAxis);
            vertices.Add(currentVertex);
            newVertics.Add(currentVertex);
            normals.Add((currentVertex - center).normalized);
        }
        return newVertics;
    }
}
