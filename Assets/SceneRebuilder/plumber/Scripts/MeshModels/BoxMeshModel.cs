using MathGeoLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMeshModel : BaseMeshModel
{
    public MeshBoxData ModelData;

    public Vector3 Position;

    public Vector3 MeshCenter;

    public Vector3 ObbCenter;

    public float DistanceOfCenters;

    public OrientedBoundingBox OBB;

    public float minNormalDis = 0.005f;//0.0005f;

    public float minAngleDis = 0.08f;//0.005f;

    public float minSamePlaneNormalDis = 0.0001f;


    public static int ErrorCount = 0;

    public void ClearModelInfo()
    {
        OBB = new OrientedBoundingBox();
    }

    public override void GetModelInfo()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;
        MeshTriangles meshTriangles = new MeshTriangles(mesh);
        //Debug.Log($"GetModelInfo mesh vertexCount:{mesh.vertexCount} triangles:{mesh.triangles.Length}");
        //meshTriangles.ShowCirclesById(this.transform, PointScale, 0, 3, minRepeatPointDistance);
        var sharedPoints1 = meshTriangles.GetSharedMeshTrianglesListByNormal(0, minSamePlaneNormalDis, this.name);
        //sharedPoints1.SortByCount();
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
                        var c1 = transform.TransformPoint(plane1.Center);
                        var c2 = transform.TransformPoint(plane2.Center);
                        var c3 = transform.TransformPoint(plane3.Center);
                        var c4 = transform.TransformPoint(plane4.Center);
                        var c5 = transform.TransformPoint(plane5.Center);
                        var c6 = transform.TransformPoint(plane6.Center);
                        Vector3 dir1 = c1 - c2;
                        Vector3 dir2 = c3 - c4;
                        Vector3 dir3 =c5 - c6;
                        float angle1 = Vector3.Angle(dir1, dir2);
                        float angle2 = Vector3.Angle(dir1, dir3);
                        float angle3 = Vector3.Angle(dir2, dir3);

                        //if (Mathf.Abs(angle1 - 90) < minAngleDis && Mathf.Abs(angle2 - 90) < minAngleDis && Mathf.Abs(angle3 - 90) < minAngleDis)
                        //{
                        //    float length1 = Vector3.Distance(c1, c2);
                        //    float length2 = Vector3.Distance(c3, c4);
                        //    float length3 = Vector3.Distance(c5, c6);
                        //    IsGetInfoSuccess = true;
                        //    //Debug.Log($"GetModelInfo length1:{length1} length2:{length2} length3:{length3}");
                        //    OBB = new OrientedBoundingBox();
                        //    //OBB.Center = this.transform.position;
                        //    OBB.Center = Vector3.zero;
                        //    OBB.Extent = new Vector3(length1 / 2, length2 / 2, length3 / 2);
                        //    OBB.Right = dir1;
                        //    OBB.Up = dir2;
                        //    OBB.Forward = dir3;
                        //}
                        //else
                        //{
                        //    IsGetInfoSuccess = false;
                        //    Debug.LogError($"GetModelInfo[{ErrorCount++}]({minAngleDis}) gameObject:{this.name} angle1:{angle1}({Mathf.Abs(angle1 - 90)}) angle2:{angle2}({Mathf.Abs(angle2 - 90)}) angle3:{angle3}({Mathf.Abs(angle3 - 90)})");
                        //}

                        if (Mathf.Abs(angle1 - 90) < minAngleDis && Mathf.Abs(angle2 - 90) < minAngleDis && Mathf.Abs(angle3 - 90) < minAngleDis)
                        {
                            float length1 = Vector3.Distance(c1, c2);
                            float length2 = Vector3.Distance(c3, c4);
                            float length3 = Vector3.Distance(c5, c6);
                            IsGetInfoSuccess = true;
                            //Debug.Log($"GetModelInfo length1:{length1} length2:{length2} length3:{length3}");
                            OBB = new OrientedBoundingBox();
                            //OBB.Center = this.transform.position;
                            OBB.Center = Vector3.zero;
                            OBB.Extent = new Vector3(length1 / 2, length2 / 2, length3 / 2);
                            OBB.Right = dir1;
                            OBB.Up = dir2;
                            OBB.Forward = dir3;
                        }
                        else
                        {
                            IsGetInfoSuccess = false;
                            Debug.LogError($"GetModelInfo[{ErrorCount++}]({minAngleDis}) gameObject:{this.name} angle1:{angle1}({Mathf.Abs(angle1 - 90)}) angle2:{angle2}({Mathf.Abs(angle2 - 90)}) angle3:{angle3}({Mathf.Abs(angle3 - 90)})");
                        }
                    }
                    else
                    {
                        plane6 = sharedPoints1.GetClosedPlaneByNormal(-plane5.Normal, minNormalDis);
                        var dis = Vector3.Distance(-plane5.Normal, plane6.Normal);
                        IsGetInfoSuccess = false;
                        Debug.LogWarning($"GetModelInfo[{ErrorCount++}] plane6 == null gameObject:{this.name} normal:{-plane5.Normal} dis:{dis} minNormalDis:{minNormalDis}");
                    }
                }
                else
                {
                    plane4 = sharedPoints1.GetClosedPlaneByNormal(-plane3.Normal, minNormalDis);
                    var dis = Vector3.Distance(-plane3.Normal, plane4.Normal);
                    IsGetInfoSuccess = false;
                    Debug.LogWarning($"GetModelInfo[{ErrorCount++}] plane4 == null gameObject:{this.name} normal:{-plane3.Normal} dis:{dis} minNormalDis:{minNormalDis}");
                }
            }
            else
            {
                plane2 = sharedPoints1.GetClosedPlaneByNormal(-plane1.Normal, minNormalDis);
                var dis = Vector3.Distance(-plane1.Normal, plane2.Normal);
                IsGetInfoSuccess = false;
                Debug.LogWarning($"GetModelInfo[{ErrorCount++}] plane2 == null gameObject:{this.name} normal:{-plane1.Normal} dis:{dis} minNormalDis:{minNormalDis}");
            }
        }
        else
        {
            IsGetInfoSuccess = false;
            Debug.LogError($"GetModelInfo[{ErrorCount++}] sharedPoints1.Count != 6 gameObject:{this.name} Count:{sharedPoints1.Count}");
        }

        meshTriangles.Dispose();
    }



    private void GetModelInfoByObb()
    {
        Position = this.transform.position;

        GetOBB();

        MeshCenter = MeshRendererInfo.GetCenterPos(gameObject, false, true);
        ObbCenter = this.transform.TransformPoint(OBB.Center);

        DistanceOfCenters = Vector3.Distance(MeshCenter, ObbCenter);

        gameObject.SetActive(false);
    }

    private OrientedBoundingBox GetOBB()
    {
        //MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        //var vs = meshFilter.sharedMesh.vertices;
        //OBB = OrientedBoundingBox.GetObb(vs, this.name, true);
        return OBB;
    }

    public override void GetModelInfo_Job()
    {
        BoxMeshInfoJob job = BoxMeshInfoJob.GetOneJob(0, gameObject);
        job.Execute();
    }

    public void SetModelData(MeshBoxData data)
    {
        this.SetModelData(data);
    }

    public override void RendererModel()
    {
        if (IsGetInfoSuccess == false)
        {
            this.gameObject.SetActive(true);
            //Debug.LogError($"BoxMeshModel.RendererModel IsGetInfoSuccess == false name:{this.name}");
            return;
        }
        ClearGo();
        this.gameObject.SetActive(false);
        GameObject go=OBBCollider.CreateObbBox(this.transform, GetOBB());
        go.transform.name = this.name + "_New";
        go.transform.transform.SetParent(this.transform.parent);
        ResultGo = go;

        //GameObject go2 = OBBCollider.ShowOBBBox(this.gameObject, false);
        //go2.transform.name = this.name + "OBBBox";
        //go2.transform.transform.SetParent(this.transform.parent);

        BoxMeshGenerator generator = go.AddMissingComponent<BoxMeshGenerator>();
        generator.Target = this.gameObject;

        CheckResult();
    }

    public void ReplaceModel()
    {
        GameObject go = OBBCollider.CreateObbBox(this.transform, OBB);
        //go.transform.SetParent(this.transform.parent);
        go.transform.name = this.name + "_ObbBox";

        MeshHelper.CopyTransformMesh(go, this.gameObject);

        GameObject.DestroyImmediate(go);
    }

    public static int CheckErrorCount = 0;

    public override void CheckResult()
    {
        //mcResult.ObbDistance = OBBCollider.GetObbDistance(ResultGo, this.gameObject);

        //mcResult.MeshDistance = MeshHelper.GetVertexDistanceEx(ResultGo, this.gameObject);
        //mcResult.SizeDistance = MeshHelper.GetSizeDistance(ResultGo, this.gameObject);
        //mcResult.RTDistance = OBBCollider.GetObbRTDistance(ResultGo, this.gameObject);

        //if (mcResult.SizeDistance > 0.1f)
        //{
        //    IsGetInfoSuccess = false;
        //}

        //TransformHelper.ClearChildren(ResultGo);

        var meshDis= MeshHelper.GetVertexDistanceEx(ResultGo, this.gameObject);
        if (meshDis > 0.00001)
        {
            //Debug.LogError($"CheckResult2 gameObject:{this.name}[{CheckErrorCount++}] Error meshDis:{meshDis}");
            IsGetInfoSuccess = false;

            MeshAlignHelper.AcRTAlign(ResultGo, this.gameObject);

            var meshDis2 = MeshHelper.GetVertexDistanceEx(ResultGo, this.gameObject);

            if (meshDis2 > 0.00001)
            {
                IsGetInfoSuccess = false;
                Debug.LogError($"CheckResult22[{IsGetInfoSuccess}] gameObject:{this.name} [{CheckErrorCount++}] Error meshDis:{meshDis} meshDis2:{meshDis2}");
            }
            else
            {
                IsGetInfoSuccess = true;
                Debug.Log($"CheckResult21[{IsGetInfoSuccess}] gameObject:{this.name} meshDis:{meshDis} meshDis2:{meshDis2} ");
            }
        }
        else
        {
            Debug.Log($"CheckResult1 gameObject:{this.name} meshDis:{meshDis} ");

        }
        
    }
}
