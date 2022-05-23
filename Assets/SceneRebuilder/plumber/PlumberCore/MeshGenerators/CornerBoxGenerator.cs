using CommonExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerBoxGenerator : SteelMeshGenerator
{
	public float cornerX = 0.1f;
	public float cornerY = 0.2f;


	public void SetData(CornerBoxMeshData data)
	{
		//this.IsCenter = data.IsCenter;
		this.forward = data.forward;
		this.up = data.up;
		this.angle = data.angle;
		this.length = data.length;
		this.width = data.width;
		this.height = data.height;
		this.cornerX = data.cornerX;
		this.cornerY = data.cornerY;
		this.angle = data.angle;
	}

	public override string GetId()
	{
        //return $"CB_{height:F4}_{width:F4}_{cornerX:F4}_{cornerY:F4}";
        //return $"CB_{height:F5}_{width:F5}_{cornerX:F5}_{cornerY:F5}";

        //return $"CB_{height:F3}_{width:F3}_{cornerX:F3}_{cornerY:F3}";

        //return $"CB_{height:F2}_{width:F2}_{cornerX:F3}_{cornerY:F3}";

        //      if (width > 1)
        //      {
        //	return $"CB_{height:F2}_{width:F1}_{cornerX:F3}_{cornerY:F3}";
        //}
        //      else
        //      {
        //	return $"CB_{height:F2}_{width:F2}_{cornerX:F3}_{cornerY:F3}";
        //}

        //return $"CB_{height:F1}_{width:F1}_{cornerX:F3}_{cornerY:F3}";

        if (width > 0.2f)
        {
            return $"CB_{height:F2}_{width:F2}_{cornerX:F3}_{cornerY:F3}";
        }
        else
        {
            return $"CB_{height:F3}_{width:F3}_{cornerX:F3}_{cornerY:F3}";
        }
    }

	private Mesh GenerateMesh()
	{
		LineMeshGenerator lineMesh = gameObject.AddMissingComponent<LineMeshGenerator>();
		float l2 = 1 / 2f;
		float z = -l2;

		float sx = width;
		float sy = height;
		//if (isSameSize)
		//{
		//	sx = sameSizeUnit;
		//	sy = sameSizeUnit;
		//	sizePlane = sameSizeUnit / 10f;
		//	sizeDetail = sameSizeUnit / 10f;
		//}
		float offX = 0;
		float offY = 0;
		if (IsCenter)
		{
			offX = sx / 2;
			offY = sy / 2;
		}
		lineMesh.StartPlanes = new List<Vector3[]>();
		lineMesh.AddPlane(new Vector3[] {
			new Vector3(0-offX,0-offY, z), new Vector3(0-offX, height-cornerY-offY, z),new Vector3(cornerX-offX, height-offY, z), new Vector3(width-cornerX-offX, height-offY, z), new Vector3(width-offX, height-cornerY-offY, z), new Vector3(width-offX, 0-offY, z)
		});
		lineMesh.Direction = new Vector3(0, 0, 1);
		lineMesh.Length = 1;
		lineMesh.GenerateMesh();

		string id = this.GetId();
		lineMesh.mesh.name += "_" + id;

		return lineMesh.mesh;
	}

 //   public override void CreateLine()
 //   {
	//	GenerateMesh();
	//	SetTransform();
	//}

	//public static Dictionary<string, Mesh> meshDict = new Dictionary<string, Mesh>();

	//public bool isShowLog = false;

	[ContextMenu("CreateLine")]
	public override void CreateLine()
	{
		string id = this.GetId();
		if (meshDict.ContainsKey(id))
		{
			if (isShowLog)
			{
				Debug.Log($"CornerBox.CreateLine ContainsKey(id) name:{this.name} id:{id} meshDict:{meshDict.Count}");
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
				Debug.Log($"CornerBox.CreateLine GenerateMesh name:{this.name} id:{id} meshDict:{meshDict.Count}");
			}
			Mesh mesh = GenerateMesh();
			meshDict.Add(id, mesh);
			SetTransform();
		}
	}

	private void SetTransform()
	{
		transform.forward = base.forward;
		//transform.up = base.up;

		//Vector3 upW = transform.TransformDirection(transform.up);
		//float angleOff = Vector3.Angle(base.up, upW);

		//      transform.Rotate(base.forward, angle + angleOff, Space.World);

		//float scale = GetScale();
		//if (isMirror)
		//{
		//	transform.localScale = new Vector3(-scale, scale, length);
		//}
		//else
		//{
		//	transform.localScale = new Vector3(scale, scale, length);
		//}

		transform.localScale = new Vector3(1, 1, length);
	}

	[ContextMenu("ShowDirection")]
	public void ShowDirection()
    {
		transform.forward = base.forward;
		Vector3 forwardW = transform.TransformDirection(transform.forward);
		Vector3 upW = transform.TransformDirection(transform.up);
		float angleOff = Vector3.Angle(base.up, upW);
		Debug.Log($"ShowDirection forward:{base.forward.Vector3ToString5()} transform.forward:{transform.forward.Vector3ToString5()} forwardW:{forwardW.Vector3ToString5()}");
		Debug.Log($"ShowDirection up:{base.up.Vector3ToString5()} transform.up:{transform.up.Vector3ToString5()} upW:{upW.Vector3ToString5()} angleOff:{angleOff}");
	}
}
