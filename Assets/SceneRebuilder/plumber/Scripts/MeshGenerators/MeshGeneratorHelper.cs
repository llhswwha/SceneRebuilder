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
        PipeMeshGeneratorBase.OrthoNormalize(ref direction, ref xAxis, ref yAxis,"");

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
