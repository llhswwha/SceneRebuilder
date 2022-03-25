using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LMeshGenerator : MonoBehaviour
{
	public float size1 = 0.2f;
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
			new Vector3(0-off,0-off, z), new Vector3(size1-off, size1-off, z), new Vector3(size2-off, size1-off, z), new Vector3(size2-off, 0-off, z),
			new Vector3(0-off,0-off, z), new Vector3(0-off, size2-off, z), new Vector3(size1-off, size2-off, z), new Vector3(size1-off, size1-off, z)
		});
        if (size3 > 0)
        {
			lineMesh.AddPlane(new Vector3[] {
			new Vector3(size1-off, size1-off, z), new Vector3(size1-off, size1-off+size3, z), new Vector3(size1-off+size3, size1-off, z)
			});
        }
		//lineMesh.StartPlane = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, height, 0), new Vector3(w2, -h2, 0), new Vector3(w2, h2, 0) };
		lineMesh.Direction = new Vector3(0, 0, 1);
		lineMesh.Length = length;
		lineMesh.GenerateMesh();
	}
}
