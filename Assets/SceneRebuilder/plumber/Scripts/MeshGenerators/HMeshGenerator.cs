using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMeshGenerator : MonoBehaviour
{

	public bool IsCenter = true;

	public float width = 1;
	public float height = 1.5f;
	public float length = 2;

	public float sizeY = 0.2f;
	public float sizeX = 0.25f;
	public float sizeDetail = 0.04f;


	[ContextMenu("CreateLine")]
	public void CreateLine()
	{
		LineMeshGenerator lineMesh = gameObject.AddMissingComponent<LineMeshGenerator>();
		float l2 = length / 2;
		float z = -l2;
		float offX = 0;
		if (IsCenter)
		{
			offX = width / 2;
		}
		float offY = 0;
		if (IsCenter)
		{
			offY = height / 2;
		}
		lineMesh.StartPlanes = new List<Vector3[]>();
		lineMesh.AddPlane(new Vector3[] {
			new Vector3(0-offX,0-offY, z), new Vector3(0-offX, sizeY-offY, z), new Vector3(width-offX, sizeY-offY, z), new Vector3(width-offX, 0-offY, z)
			//,new Vector3(0-off,0-off+size2-size1, z), new Vector3(0-off, size1-off+size2-size1, z), new Vector3(size2-off, size1-off+size2-size1, z), new Vector3(size2-off, 0-off+size2-size1, z)
		});
		lineMesh.AddPlane(new Vector3[] {
			new Vector3(0-offX,0-offY+height-sizeY, z), new Vector3(0-offX, sizeY-offY+height-sizeY, z), new Vector3(width-offX, sizeY-offY+height-sizeY, z), new Vector3(width-offX, 0-offY+height-sizeY, z)
		});

		Vector3 p31 = new Vector3(width / 2 - sizeX / 2 - offX, 0 - offY + sizeY, z);
		Vector3 p32 = new Vector3(width / 2 - sizeX / 2 - offX, -offY + height - sizeY, z);
		Vector3 p33 = new Vector3(width / 2 + sizeX / 2 - offX, -offY + height - sizeY, z);
		Vector3 p34 = new Vector3(width / 2 + sizeX / 2 - offX, 0 - offY + sizeY, z);
		lineMesh.AddPlane(new Vector3[] {
			p31, p32, p33, p34
		});
        if (sizeDetail > 0)
        {
			lineMesh.AddPlane(new Vector3[] {
				p31, p31-new Vector3(sizeDetail,0,0), p31+new Vector3(0,sizeDetail,0)
			});
			lineMesh.AddPlane(new Vector3[] {
				p32, p32-new Vector3(0,sizeDetail,0), p32-new Vector3(sizeDetail,0,0)
			});
			lineMesh.AddPlane(new Vector3[] {
				p33, p33+new Vector3(sizeDetail,0,0),p33-new Vector3(0,sizeDetail,0)
			});
			lineMesh.AddPlane(new Vector3[] {
				p34, p34+new Vector3(0,sizeDetail,0), p34+new Vector3(sizeDetail,0,0)
			});
		}
		//lineMesh.StartPlane = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, height, 0), new Vector3(w2, -h2, 0), new Vector3(w2, h2, 0) };
		lineMesh.Direction = new Vector3(0, 0, 1);
		lineMesh.Length = length;
		lineMesh.GenerateMesh();
	}
}
