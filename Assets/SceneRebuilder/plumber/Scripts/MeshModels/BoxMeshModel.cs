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

    public override void GetModelInfo()
    {
        Position = this.transform.position;

        GetOBB();

        MeshCenter = MeshRendererInfo.GetCenterPos(gameObject,false,true);
        ObbCenter = this.transform.TransformPoint(OBB.Center);

        DistanceOfCenters = Vector3.Distance(MeshCenter, ObbCenter);

        gameObject.SetActive(false);
    }

    private OrientedBoundingBox GetOBB()
    {
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        var vs = meshFilter.sharedMesh.vertices;
        OBB = OrientedBoundingBox.GetObb(vs, this.name, true);
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

    public void RendererModel()
    {
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
            Debug.LogError($"CheckResult Error meshDis:{meshDis} gameObject:{this.name}");
        }
        else
        {
            //Debug.Log($"CheckResult meshDis:{meshDis} gameObject:{this.name}");
        }
        
    }
}
