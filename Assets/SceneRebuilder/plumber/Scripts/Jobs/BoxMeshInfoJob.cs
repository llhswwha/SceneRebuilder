using MathGeoLib;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct BoxMeshInfoJob : IMeshInfoJob
{
    public static float minNormalDis = 0.005f;//0.0005f;

    public static float minAngleDis = 0.08f;//0.005f;

    public static float minSamePlaneNormalDis = 0.0001f;

    public static int ErrorCount;

    public int id;
    public MeshStructure mesh;

    public Matrix4x4 localToWorldMatrix;

    public static NativeArray<MeshBoxData> Result;

    public static void InitResult(int count)
    {
        ErrorCount = 0;
        Result = new NativeArray<MeshBoxData>(count, Allocator.Persistent);
    }

    public static MeshBoxData GetResult(int id)
    {
        if (Result == null)
        {
            Debug.LogError($"BoxMeshInfoJob.GetResult Result == null  ");
        }
        if (id < 0 || id >= Result.Length)
        {
            Debug.LogError($"BoxMeshInfoJob.GetResult id < 0 || id >= Result.Length id:{id} Result.Length:{Result.Length}");
            return new MeshBoxData();
        }
        return Result[id];
    }

    public static void DisposeResult()
    {
        Result.Dispose();
    }

    public static BoxMeshInfoJob GetOneJob(int i, MeshStructure m)
    {
        BoxMeshInfoJob job = new BoxMeshInfoJob(i, m);
        return job;
    }

    public static BoxMeshInfoJob GetOneJob(int i, GameObject go)
    {
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        MeshStructure meshS = new MeshStructure(mesh);
        BoxMeshInfoJob job = new BoxMeshInfoJob(i, meshS);
        return job;
    }

   

    public BoxMeshInfoJob(int i ,MeshStructure m)
    {
        this.id = i;
        this.mesh = m;
        this.data = new MeshBoxData();
        this.localToWorldMatrix = Matrix4x4.identity;
    }

    public MeshBoxData data;
    public void Execute()
    {


        MeshBoxData data = new MeshBoxData();

        //Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetModelInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        //meshTriangles.ShowCirclesById(this.transform, PointScale, 0, 3, minRepeatPointDistance);
        var sharedPoints1 = meshTriangles.GetSharedMeshTrianglesListByNormal(0, minSamePlaneNormalDis, id+"");
        //sharedPoints1.SortByCount(); 

        //Debug.Log($"BoxMeshInfoJob1[{id}] sharedPoints1.Count:{sharedPoints1.Count} localToWorldMatrix:\n{localToWorldMatrix}"); 

        if (sharedPoints1.Count == 6)
        {
            //var dir1=
            var plane1 = sharedPoints1[0];
            var plane2 = sharedPoints1.GetPlaneByNormal(-plane1.Normal, minNormalDis);
            if (plane2 != null)
            {
                sharedPoints1.Remove(plane1);
                sharedPoints1.Remove(plane2);

                var plane3 = sharedPoints1[0];
                var plane4 = sharedPoints1.GetPlaneByNormal(-plane3.Normal, minNormalDis);

                if (plane4 != null)
                {
                    sharedPoints1.Remove(plane3);
                    sharedPoints1.Remove(plane4);

                    var plane5 = sharedPoints1[0];
                    var plane6 = sharedPoints1.GetPlaneByNormal(-plane5.Normal, minNormalDis);

                    if (plane6 != null)
                    {
                        //var c1 = transform.TransformPoint(plane1.Center);
                        //var c2 = transform.TransformPoint(plane2.Center);
                        //var c3 = transform.TransformPoint(plane3.Center);
                        //var c4 = transform.TransformPoint(plane4.Center);
                        //var c5 = transform.TransformPoint(plane5.Center);
                        //var c6 = transform.TransformPoint(plane6.Center);

                        var c1 = localToWorldMatrix.MultiplyPoint(plane1.Center);
                        var c2 = localToWorldMatrix.MultiplyPoint(plane2.Center);
                        var c3 = localToWorldMatrix.MultiplyPoint(plane3.Center);
                        var c4 = localToWorldMatrix.MultiplyPoint(plane4.Center);
                        var c5 = localToWorldMatrix.MultiplyPoint(plane5.Center);
                        var c6 = localToWorldMatrix.MultiplyPoint(plane6.Center);
                        Vector3 dir1 = c1 - c2;
                        Vector3 dir2 = c3 - c4;
                        Vector3 dir3 = c5 - c6;
                        float angle1 = Vector3.Angle(dir1, dir2);
                        float angle2 = Vector3.Angle(dir1, dir3);
                        float angle3 = Vector3.Angle(dir2, dir3);

                        if (Mathf.Abs(angle1 - 90) < minAngleDis && Mathf.Abs(angle2 - 90) < minAngleDis && Mathf.Abs(angle3 - 90) < minAngleDis)
                        {
                            float length1 = Vector3.Distance(c1, c2);
                            float length2 = Vector3.Distance(c3, c4);
                            float length3 = Vector3.Distance(c5, c6);
                            data.IsGetInfoSuccess = true;
                            //Debug.Log($"GetModelInfo length1:{length1} length2:{length2} length3:{length3}");
                            var OBB = new OrientedBoundingBox();
                            //OBB.Center = this.transform.position;
                            OBB.Center = Vector3.zero;
                            OBB.Extent = new Vector3(length1 / 2, length2 / 2, length3 / 2);
                            OBB.Right = dir1;
                            OBB.Up = dir2;
                            OBB.Forward = dir3;
                            data.OBB = OBB;
                        }
                        else
                        {
                            data.IsGetInfoSuccess = false;
                            Debug.LogError($"GetModelInfo[Error_{ErrorCount++}]({minAngleDis}) gameObject:{this.id} angle1:{angle1}({Mathf.Abs(angle1 - 90)}) angle2:{angle2}({Mathf.Abs(angle2 - 90)}) angle3:{angle3}({Mathf.Abs(angle3 - 90)})");
                        }
                    }
                    else
                    {
                        plane6 = sharedPoints1.GetClosedPlaneByNormal(-plane5.Normal, minNormalDis);
                        var dis = Vector3.Distance(-plane5.Normal, plane6.Normal);
                        data.IsGetInfoSuccess = false;
                        Debug.LogWarning($"GetModelInfo[Error_{ErrorCount++}] plane6 == null gameObject:{this.id} normal:{-plane5.Normal} dis:{dis} minNormalDis:{minNormalDis}");
                    }
                }
                else
                {
                    plane4 = sharedPoints1.GetClosedPlaneByNormal(-plane3.Normal, minNormalDis);
                    var dis = Vector3.Distance(-plane3.Normal, plane4.Normal);
                    data.IsGetInfoSuccess = false;
                    Debug.LogWarning($"GetModelInfo[Error_{ErrorCount++}] plane4 == null gameObject:{this.id} normal:{-plane3.Normal} dis:{dis} minNormalDis:{minNormalDis}");
                }
            }
            else
            {
                plane2 = sharedPoints1.GetClosedPlaneByNormal(-plane1.Normal, minNormalDis);
                var dis = Vector3.Distance(-plane1.Normal, plane2.Normal);
                data.IsGetInfoSuccess = false;
                Debug.LogWarning($"GetModelInfo[Error_{ErrorCount++}] plane2 == null gameObject:{this.id} normal:{-plane1.Normal} dis:{dis} minNormalDis:{minNormalDis}");
            }
        }
        else
        {
            data.IsGetInfoSuccess = false;
            Debug.LogError($"GetModelInfo[Error_{ErrorCount++}] sharedPoints1.Count != 6 gameObject:{this.id} Count:{sharedPoints1.Count}");
        }

        if (Result.Length > id)
        {
            Result[id] = data;
        }
        else
        {
            Debug.LogWarning($"BoxMeshInfoJob2[{id}] Result.Length :{Result.Length }");
        }

        //meshTriangles.Dispose();

        //Debug.Log($"BoxMeshInfoJob2[{id}] sharedPoints1.Count:{sharedPoints1.Count} data:{data.OBB} localToWorldMatrix:\n{localToWorldMatrix}");
    }

    public void Dispose()
    {
        mesh.Dispose();
    }
}
