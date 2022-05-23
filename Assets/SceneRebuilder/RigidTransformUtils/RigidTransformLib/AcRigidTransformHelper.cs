// Rigid Transform implementation using explanation in http://nghiaho.com/?page_id=671
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using Accord;
using System;
//using MeshJobs;

public static class AcRigidTransformHelper
{
    public static RTResult GetRTResult(UnityEngine.Vector3[] vsFrom, UnityEngine.Vector3[] vsTo)
    {
        var r1 = AcRigidTransform.ApplyTransformationN(vsFrom, vsTo);
        var vsFromNew1 = r1.ApplyPoints(vsFrom);
        var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsTo, false);
        return r1;
    }

    public static float GetRTDistance(UnityEngine.Vector3[] vsFrom, UnityEngine.Vector3[] vsTo)
    {
        var r1 = AcRigidTransform.ApplyTransformationN(vsFrom, vsTo);
        var vsFromNew1 = r1.ApplyPoints(vsFrom);
        var dis2 = DistanceUtil.GetDistance(vsFromNew1, vsTo, false);
        return dis2;
    }


    // Calculation of covariance matrix H
    static public Matrix3x3 CovarianceMatrixStep( Accord.Math.Vector3 difSetA, Accord.Math.Vector3 difSetB )
    {
        Matrix3x3 M;
        M.V00 = difSetA.X * difSetB.X;
        M.V01 = difSetA.X * difSetB.Y;
        M.V02 = difSetA.X * difSetB.Z;

        M.V10 = difSetA.Y * difSetB.X;
        M.V11 = difSetA.Y * difSetB.Y;
        M.V12 = difSetA.Y * difSetB.Z;

        M.V20 = difSetA.Z * difSetB.X;
        M.V21 = difSetA.Z * difSetB.Y;
        M.V22 = difSetA.Z * difSetB.Z;


        return M;
    }

    static public float Distance2( Accord.Math.Vector3 a, Accord.Math.Vector3 b )
    {
        return (a.X-b.X)*(a.X-b.X)+(a.Y-b.Y)*(a.Y-b.Y)+(a.Z-b.Z)*(a.Z-b.Z);
    }

    static public float StandardDeviation( Accord.Math.Vector3 a, Accord.Math.Vector3 b )
    {
        return (a.X-b.X)*(a.X-b.X)+(a.Y-b.Y)*(a.Y-b.Y)+(a.Z-b.Z)*(a.Z-b.Z);
    }

    // Converting Unity.Vector3 to Accord.Vector3
    static public Accord.Math.Vector3 UnitytoAccord( UnityEngine.Vector3 pos )
    {
        Accord.Math.Vector3 posTransformed = new Accord.Math.Vector3();
        posTransformed.X = pos.x;
        posTransformed.Y = pos.y;
        posTransformed.Z = pos.z;

        return posTransformed;
    }
    static public Accord.Math.Vector3[] UnitytoAccord(UnityEngine.Vector3[] ps)
    {
        Accord.Math.Vector3[] ps2 = new Accord.Math.Vector3[ps.Length];
        for(int i=0;i<ps.Length;i++)
        {
            ps2[i] = UnitytoAccord(ps[i]);
        }
        return ps2;
    }
    // Converting  Accord.Vector3 to Unity.Vector3 
    static public UnityEngine.Vector3 AccordtoUnity(Accord.Math.Vector3 pos)
    {
        UnityEngine.Vector3 posTransformed = new UnityEngine.Vector3();
        posTransformed.x = pos.X;
        posTransformed.y = pos.Y;
        posTransformed.z = pos.Z;

        return posTransformed;
    }
    static  public Matrix3x3 ZeroMatrix(Matrix3x3 m)
    {
        m.V00 = 0;
        m.V01 = 0;
        m.V02 = 0;
        m.V10 = 0;
        m.V11 = 0;
        m.V12 = 0;
        m.V20 = 0;
        m.V21 = 0;
        m.V22 = 0;

        return m;
    }
    static  public Matrix3x3 NegativeMatrix(Matrix3x3 m)
    {
        m.V00 *= (-1);
        m.V01 *= (-1);
        m.V02 *= (-1);
        m.V10 *= (-1);
        m.V11 *= (-1);
        m.V12 *= (-1);
        m.V20 *= (-1);
        m.V21 *= (-1);
        m.V22 *= (-1);

        return m;
    }

    // Creating Unity Transformation matrix using 3x3 Rotation matrix and translation vector acquired from RigidTransform
    public static UnityEngine.Matrix4x4 AccordToUnityMatrix(UnityEngine.Matrix4x4 UnityM,Accord.Math.Matrix3x3 RotationM,Accord.Math.Vector3 Trans)
    {
              
        UnityM.m00 = RotationM.V00;
        UnityM.m10 = RotationM.V10;
        UnityM.m20 = RotationM.V20;
        
        UnityM.m01 = RotationM.V01;
        UnityM.m11 = RotationM.V11;
        UnityM.m21 = RotationM.V21;
        
        UnityM.m02 = RotationM.V02;
        UnityM.m12 = RotationM.V12;
        UnityM.m22 = RotationM.V22;


        UnityM.m03 = Trans.X;
        UnityM.m13 = Trans.Y;
        UnityM.m23 = Trans.Z;

        UnityM.m30 = 0;
        UnityM.m31 = 0;
        UnityM.m32 = 0;
        UnityM.m33 = 1;

        return UnityM;
    }

     public static UnityEngine.Matrix4x4 AccordToUnityMatrix(UnityEngine.Matrix4x4 UnityM,Accord.Math.Matrix3x3 RotationM,UnityEngine.Vector3 Trans)
    {
              
        UnityM.m00 = RotationM.V00;
        UnityM.m10 = RotationM.V10;
        UnityM.m20 = RotationM.V20;
        
        UnityM.m01 = RotationM.V01;
        UnityM.m11 = RotationM.V11;
        UnityM.m21 = RotationM.V21;
        
        UnityM.m02 = RotationM.V02;
        UnityM.m12 = RotationM.V12;
        UnityM.m22 = RotationM.V22;


        UnityM.m03 = Trans.x;
        UnityM.m13 = Trans.y;
        UnityM.m23 = Trans.z;

        UnityM.m30 = 0;
        UnityM.m31 = 0;
        UnityM.m32 = 0;
        UnityM.m33 = 1;

        return UnityM;
    }

        public static Accord.Math.Vector3 GetCentroid(Accord.Math.Vector3[] ps)
    {
        Accord.Math.Vector3 sum = new Accord.Math.Vector3(0, 0, 0);
        for(int i=0;i<ps.Length;i++)
        {
            sum += ps[i];
        }
        Accord.Math.Vector3 centroid = sum / ps.Length;
        return centroid;
    }

    public static Matrix3x3 GetCovarianceMatrix(Accord.Math.Vector3[] ps1, Accord.Math.Vector3 centroidA, Accord.Math.Vector3[] ps2, Accord.Math.Vector3 centroidB)
    {
        Matrix3x3 H = new Matrix3x3();
        for(int i=0;i<ps1.Length&&i<ps2.Length;i++)
        {
            H += CovarianceMatrixStep(ps1[i] - centroidA, ps2[i] - centroidB);
        }
        return H;
    }
}
