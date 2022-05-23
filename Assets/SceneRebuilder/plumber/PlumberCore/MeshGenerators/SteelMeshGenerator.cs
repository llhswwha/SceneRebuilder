using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelMeshGenerator : BaseMeshGenerator
{
    public bool IsCenter = true;
    public Vector3 forward = new Vector3(0, 0, 1);
    public Vector3 up = new Vector3(0, 1, 1);
    public float angle = 0;
    public float length = 3;
    public float width = 2;
    public float height = 3;

    public virtual void CreateLine()
    {

    }

    public virtual string GetId()
    {
        return $"{width}_{height}";
    }

	[ContextMenu("ClearMeshDict")]
	public void ClearMeshDict()
	{
		Debug.Log($"ClearMeshDict meshDict:{meshDict.Count}");
		meshDict.Clear();
	}

	[ContextMenu("GetMeshDict")]
	public void GetMeshDict()
	{
		Debug.Log($"GetMeshDict meshDict:{meshDict.Count}");
	}

	[ContextMenu("ClearMeshTransform")]
	public void ClearMeshTransform()
	{
		MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
		if (mf)
		{
			mf.sharedMesh = null;
			GameObject.DestroyImmediate(mf);
		}

		MeshRenderer mr = this.GetComponent<MeshRenderer>();
		if (mr)
		{
			GameObject.DestroyImmediate(mr);
		}
		//transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	public static Dictionary<string, Mesh> meshDict = new Dictionary<string, Mesh>();

	public bool isShowLog = false;
}
