using CommonExtension;
using System.Collections.Generic;
using UnityEngine;

public class LMeshGenerator : SteelMeshGenerator
{
	public float sizePlane = 0.2f;
	public float sizeDetail = 0.04f;
	public bool isMirror = false;

	public bool isSameSize = false;

	public override string GetId()
	{
		//string sx = $"{sizeX:F4}";
		//string sy = $"{sizeY:F4}";
		if (isSameSize)
		{
			return $"L_{sameSizeUnit:F4}_{sameSizeUnit:F4}_{sameSizeUnit / 10f:F4}_{sameSizeUnit / 10f:F4}";
		}
		else
		{
			return $"L_{height:F4}_{width:F4}_{sizePlane:F4}_{sizeDetail:F4}";
		}
	}

	public float GetScale()
    {
		//string sx = $"{sizeX:F4}";
		//string sy = $"{sizeY:F4}";
		if (isSameSize)
		{
			return width / sameSizeUnit;
		}
		else
		{
			return 1;
		}
	}

	public float sameSizeUnit = 0.1f;

	public void SetData(SteelModelLData data)
	{
		//this.IsCenter = data.IsCenter;
		this.forward = data.forward;
		this.angle = data.angle;
		this.length = data.length;
		this.sizePlane = data.sizePlane;
		this.width = data.width;
		this.height = data.height;
		this.sizeDetail = data.sizeDetail;
		this.isMirror = data.isMirror;
		this.isSameSize = data.isSameSize;
	}

	private Mesh GenerateMesh()
    {
		LineMeshGenerator lineMesh = gameObject.AddMissingComponent<LineMeshGenerator>();
		float l2 = 1 / 2f;
		float z = -l2;

		float sx = width;
		float sy = height;
        if (isSameSize)
        {
			sx = sameSizeUnit;
			sy = sameSizeUnit;
			sizePlane = sameSizeUnit / 10f;
			sizeDetail = sameSizeUnit / 10f;
		}

		float offX = 0;
		float offY = 0;
		if (IsCenter)
		{
			offX = sx / 2;
			offY = sy / 2;
		}
		lineMesh.StartPlanes = new List<Vector3[]>();
		lineMesh.AddPlane(new Vector3[] {
			new Vector3(0-offX,0-offY, z), new Vector3(sizePlane-offX, sizePlane-offY, z), new Vector3(sx-offX, sizePlane-offY, z), new Vector3(sx-offX, 0-offY, z),
			new Vector3(0-offX,0-offY, z), new Vector3(0-offX, sy-offY, z), new Vector3(sizePlane-offX, sy-offY, z), new Vector3(sizePlane-offX, sizePlane-offY, z)
		});
		if (sizeDetail > 0)
		{
			lineMesh.AddPlane(new Vector3[] {
			new Vector3(sizePlane-offX, sizePlane-offY, z), new Vector3(sizePlane-offX, sizePlane-offY+sizeDetail, z), new Vector3(sizePlane-offX+sizeDetail, sizePlane-offY, z)
			});
		}
		//lineMesh.StartPlane = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, height, 0), new Vector3(w2, -h2, 0), new Vector3(w2, h2, 0) };
		lineMesh.Direction = new Vector3(0, 0, 1);
		lineMesh.Length = 1;
		lineMesh.GenerateMesh();

		string id = this.GetId();
		lineMesh.mesh.name += "_" + id;

		return lineMesh.mesh;
	}

	private void SetTransform()
    {
		transform.forward = base.forward;
		transform.Rotate(base.forward, angle, Space.World);
		float scale = GetScale();
		if (isMirror)
		{
			transform.localScale = new Vector3(-scale, scale, length);
		}
		else
		{
			transform.localScale = new Vector3(scale, scale, length);
		}
	}



	//public bool isShowLog = false;

	[ContextMenu("CreateLine")]
	public override void CreateLine()
	{
		string id = this.GetId();
        if (meshDict.ContainsKey(id))
        {
            if (isShowLog)
            {
				Debug.Log($"L.CreateLine ContainsKey(id) name:{this.name} id:{id} meshDict:{meshDict.Count}");
			}
            Mesh mesh = meshDict[id];
			MeshFilter mf = this.gameObject.CreateMeshComponents();
			mf.sharedMesh = mesh;
			SetTransform();
		}
        else
        {
			if (isShowLog)
			{
				Debug.Log($"L.CreateLine GenerateMesh name:{this.name} id:{id} meshDict:{meshDict.Count}");
			}
            Mesh mesh = GenerateMesh();
			meshDict.Add(id, mesh);
			SetTransform();
		}
	}
}
