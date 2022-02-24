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

        BoxMeshGenerator generator = go.AddMissingComponent<BoxMeshGenerator>();
        generator.Target = this.gameObject;
    }

    public void ReplaceModel()
    {
        GameObject go = OBBCollider.CreateObbBox(this.transform, OBB);
        //go.transform.SetParent(this.transform.parent);
        go.transform.name = this.name + "_ObbBox";

        MeshHelper.CopyTransformMesh(go, this.gameObject);

        GameObject.DestroyImmediate(go);
    }
}