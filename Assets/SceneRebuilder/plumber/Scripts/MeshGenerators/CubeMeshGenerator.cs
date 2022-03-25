using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMeshGenerator : BaseMeshGenerator
{
	[ContextMenu("CreateLine")]
	public void CreateLine()
    {
		LineMeshGenerator lineMesh = gameObject.AddMissingComponent<LineMeshGenerator>();
		float w2 = width / 2;
		float h2 = height / 2;
		float l2 = length / 2;
        lineMesh.AddPlane(new Vector3[] { new Vector3(-w2, -h2, -l2), new Vector3(-w2, h2, -l2), new Vector3(w2, h2, -l2), new Vector3(w2, -h2, -l2) });
        //lineMesh.StartPlane = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, height, 0), new Vector3(w2, -h2, 0), new Vector3(w2, h2, 0) };
		lineMesh.Direction = new Vector3(0, 0, 1);
		lineMesh.Length = length;
		lineMesh.GenerateMesh();
	}

	[ContextMenu("ClearMesh")]
	public void ClearMesh()
	{
		MeshHelper.RemoveMeshComponents(gameObject);
	}

	[ContextMenu("CreateCube")]
	public void CreateCube()
	{
		MeshFilter mf = MeshHelper.CreateMeshComponents(gameObject);
		mf.sharedMesh = CreateCubeMesh();
	}

	private Mesh CreateCubeMesh()
	{
		Vector3[] vertices = {
			new Vector3 (0, 0, 0),
			new Vector3 (1, 0, 0),
			new Vector3 (1, 1, 0),
			new Vector3 (0, 1, 0),
			new Vector3 (0, 1, 1),
			new Vector3 (1, 1, 1),
			new Vector3 (1, 0, 1),
			new Vector3 (0, 0, 1),
		};

		int[] triangles = {
			0, 2, 1, //face front
			0, 3, 2,
			2, 3, 4, //face top
			2, 4, 5,
			1, 2, 5, //face right
			1, 5, 6,
			0, 7, 4, //face left
			0, 4, 3,
			5, 4, 7, //face back
			5, 7, 6,
			0, 6, 7, //face bottom
			0, 1, 6
		};

		//Mesh mesh = GetComponent<MeshFilter>().mesh;
		//mesh.Clear();
		Mesh mesh = new Mesh();
		mesh.name = "Cube";
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.Optimize();
		mesh.RecalculateNormals();
		return mesh;
	}

	[ContextMenu("CreateQuad")]
	private void CreateQuad()
	{
		MeshFilter mf = MeshHelper.CreateMeshComponents(gameObject);
		mf.sharedMesh = CreateQuadMesh();
	}

	[ContextMenu("CreateQuad2")]
	private void CreateQuad2()
	{
		MeshFilter mf = MeshHelper.CreateMeshComponents(gameObject);
		mf.sharedMesh = CreateQuadMesh2();
	}

	public float width = 1;
	public float height = 2;
	public float length = 3;

	public bool IsCenter = true;

	public Mesh CreateQuadMesh2()
	{
		float w2 = width / 2;
		float h2 = height / 2;
		MeshPointList mps = new MeshPointList();

		if (IsCenter)
		{
			mps.Add(new MeshPoint(new Vector3(-w2, -h2, 0), 0, -Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(w2, -h2, 0), 0, -Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(-w2, h2, 0), 0, -Vector3.forward, new Vector2(0, 1)));
			mps.Add(new MeshPoint(new Vector3(w2, h2, 0), 0, -Vector3.forward, new Vector2(1, 1)));
		}
		else
		{
			mps.Add(new MeshPoint(new Vector3(0, 0, 0), 0, -Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(width, 0, 0), 0, -Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(0, height, 0), 0, -Vector3.forward, new Vector2(0, 1)));
			mps.Add(new MeshPoint(new Vector3(width, height, 0), 0, -Vector3.forward, new Vector2(1, 1)));
		}
		Mesh mesh = mps.CreateMesh();
		int[] tris = new int[6]
		{
			// lower left triangle
			0, 2, 1,
			// upper right triangle
			2, 3, 1
		};
		mesh.name = "Quad2";
		mesh.triangles = tris;
		return mesh;
	}

	public Mesh CreateQuadMesh()
	{
		Mesh mesh = new Mesh();
		Vector3[] vertices = null;
		if (IsCenter)
		{
			float w2 = width / 2;
			float h2 = height / 2;
			vertices = new Vector3[4]
			{
				new Vector3(-w2, -h2, 0),
				new Vector3(w2, -h2, 0),
				new Vector3(-w2, h2, 0),
				new Vector3(w2, h2, 0)
			};
		}
		else
		{
			vertices = new Vector3[4]
			{
				new Vector3(0, 0, 0),
				new Vector3(width, 0, 0),
				new Vector3(0, height, 0),
				new Vector3(width, height, 0)
			};
		}
		mesh.vertices = vertices;
		SetQuadMesh(mesh);
		return mesh;
	}

	private void SetQuadMesh(Mesh mesh)
	{
		int[] tris = new int[6]
		{
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
		};
		mesh.triangles = tris;
		Vector3[] normals = new Vector3[4]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.normals = normals;

		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(1, 1)
		};
		mesh.uv = uv;
		mesh.name = "Quad";
	}

	[ContextMenu("CreateTriangle")]
	public void CreateTriangle()
	{
		MeshFilter mf = MeshHelper.CreateMeshComponents(gameObject);
		mf.sharedMesh = CreateTriangleMesh();
	}

	[ContextMenu("CreateTriangle2")]
	public void CreateTriangle2()
	{
		MeshFilter mf = MeshHelper.CreateMeshComponents(gameObject);
		mf.sharedMesh = CreateTriangleMesh2();
	}

	public Mesh CreateTriangleMesh2()
	{
		float w2 = width / 2;
		float h2 = height / 2;
		MeshPointList mps = new MeshPointList();

		if (IsCenter)
		{
			mps.Add(new MeshPoint(new Vector3(-w2, -h2, 0), 0, -Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(w2, -h2, 0), 0, -Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(-w2, h2, 0), 0, -Vector3.forward, new Vector2(0, 1)));
		}
		else
		{
			mps.Add(new MeshPoint(new Vector3(0, 0, 0), 0, -Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(width, 0, 0), 0, -Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(0, height, 0), 0, -Vector3.forward, new Vector2(0, 1)));
		}
		Mesh mesh = mps.CreateMesh();
		int[] tris = new int[3]
		{
			// lower left triangle
			0, 2, 1
		};
		mesh.name = "Triangle2";
		mesh.triangles = tris;
		return mesh;
	}

	public Mesh CreateTriangleMesh()
	{
		Mesh mesh = new Mesh();
		Vector3[] vertices = null;
		if (IsCenter)
		{
			float w2 = width / 2;
			float h2 = height / 2;
			vertices = new Vector3[3]
			{
				new Vector3(-w2, -h2, 0),
				new Vector3(w2, -h2, 0),
				new Vector3(-w2, h2, 0)
			};
		}
		else
		{
			vertices = new Vector3[3]
			{
				new Vector3(0, 0, 0),
				new Vector3(width, 0, 0),
				new Vector3(0, height, 0)
			};
		}
		mesh.vertices = vertices;

		int[] tris = new int[3]
		{
            // lower left triangle
            0, 2, 1
		};
		mesh.triangles = tris;

		Vector3[] normals = new Vector3[3]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		mesh.normals = normals;

		Vector2[] uv = new Vector2[3]
		{
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1)
		};
		mesh.uv = uv;
		mesh.name = "Triangle";
		return mesh;
	}

	[ContextMenu("CreateTriangleEx")]
	public void CreateTriangleEx()
	{
		MeshFilter mf = MeshHelper.CreateMeshComponents(gameObject);
		mf.sharedMesh = CreateTriangleMeshEx();
	}

	public Mesh CreateTriangleMeshEx()
	{
		float w2 = width / 2;
		float h2 = height / 2;
		MeshPointList mps = new MeshPointList();

		if (IsCenter)
		{
			mps.Add(new MeshPoint(new Vector3(-w2, -h2, 0), 0, -Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(w2, -h2, 0), 1, -Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(-w2, h2, 0), 2, -Vector3.forward, new Vector2(0, 1)));

			mps.Add(new MeshPoint(new Vector3(-w2, -h2, length), 3, Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(w2, -h2, length), 4, Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(-w2, h2, length), 5, Vector3.forward, new Vector2(0, 1)));
		}
		else
		{
			mps.Add(new MeshPoint(new Vector3(0, 0, 0), 0, -Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(width, 0, 0), 1, -Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(0, height, 0), 2, -Vector3.forward, new Vector2(0, 1)));

			mps.Add(new MeshPoint(new Vector3(0, 0, length), 3, Vector3.forward, new Vector2(0, 0)));
			mps.Add(new MeshPoint(new Vector3(width, 0, length), 4, Vector3.forward, new Vector2(1, 0)));
			mps.Add(new MeshPoint(new Vector3(0, height, length), 5, Vector3.forward, new Vector2(0, 1)));
		}

		Mesh mesh = mps.CreateMesh();
		int[] tris = new int[12]
		{
			// lower left triangle
			0, 2, 1,
			4,5,3,
			0,3,1,
			1,3,4
		};
		mesh.name = "TriangleEx";
		mesh.triangles = tris;
		return mesh;
	}

	//[ContextMenu("CreateTriangleEx2")]
	//public void CreateTriangleEx2()
	//{
	//	MeshFilter mf = CreateMeshComponents();
	//	mf.sharedMesh = CreateTriangleMesh2();
	//}
}
