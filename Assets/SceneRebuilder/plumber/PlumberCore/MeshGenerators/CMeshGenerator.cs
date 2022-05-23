using CommonExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMeshGenerator : MonoBehaviour
{
	public float size1 = 0.2f;
	public float size12 = 0.25f;
	public float size2 = 2;
	public float size3 = 0.04f;
	public float length = 3;

	public bool IsCenter = true;

	[ContextMenu("CreateLine")]
	public void CreateLine()
	{
		LineMeshGenerator lineMesh = gameObject.AddMissingComponent<LineMeshGenerator>();
		float l2 = length / 2;
		float z = -l2;
		float off = 0;
		if (IsCenter)
		{
			off = size2 / 2;
		}
		lineMesh.StartPlanes = new List<Vector3[]>();
		lineMesh.AddPlane(new Vector3[] {
			new Vector3(0-off,0-off, z), new Vector3(0-off, size1-off, z), new Vector3(size2-off, size1-off, z), new Vector3(size2-off, 0-off, z)
			//,new Vector3(0-off,0-off+size2-size1, z), new Vector3(0-off, size1-off+size2-size1, z), new Vector3(size2-off, size1-off+size2-size1, z), new Vector3(size2-off, 0-off+size2-size1, z)
		});
		lineMesh.AddPlane(new Vector3[] {
			new Vector3(0-off,0-off+size2-size1, z), new Vector3(0-off, size1-off+size2-size1, z), new Vector3(size2-off, size1-off+size2-size1, z), new Vector3(size2-off, 0-off+size2-size1, z)
		});

		Vector3 p31 = new Vector3(0 - off, 0 - off + size1, z);
		Vector3 p32 = new Vector3(0 - off, -off + size2 - size1, z);
		Vector3 p33 = new Vector3(0 + size12 - off, -off + size2 - size1, z);
		Vector3 p34 = new Vector3(0 + size12 - off, 0 - off + size1, z);
		lineMesh.AddPlane(new Vector3[] {
			p31, p32, p33, p34
		});
		if (size3 > 0)
		{
			//lineMesh.AddPlane(new Vector3[] {
			//	p31, p31-new Vector3(size3,0,0), p31+new Vector3(0,size3,0)
			//});
			//lineMesh.AddPlane(new Vector3[] {
			//	p32, p32-new Vector3(0,size3,0), p32-new Vector3(size3,0,0)
			//});
			lineMesh.AddPlane(new Vector3[] {
				p33, p33+new Vector3(size3,0,0),p33-new Vector3(0,size3,0)
			});
			lineMesh.AddPlane(new Vector3[] {
				p34, p34+new Vector3(0,size3,0), p34+new Vector3(size3,0,0)
			});
		}
		//lineMesh.StartPlane = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, height, 0), new Vector3(w2, -h2, 0), new Vector3(w2, h2, 0) };
		lineMesh.Direction = new Vector3(0, 0, 1);
		lineMesh.Length = length;
		lineMesh.GenerateMesh();
	}
}
