using UnityEngine;
using System.Collections;

public class GameObjectSizeInfo : MonoBehaviour {

    public Vector3 Size;
    public Vector3 ZeroSize;
    public Vector3 LocalSize;
    public Vector3 CurrentSize;
    public Vector3 OriginalSizeByCollider;
    public Vector3 OriginalSizeByMeshFilter;
    public MeshRenderer render;
	// Use this for initialization
	void Start ()
	{
	    render = gameObject.GetComponent<MeshRenderer>();
	    GetSize();
	}

    public void GetSize()
    {
        Size = gameObject.GetSize();
        LocalSize = gameObject.GetLocalSize();
        //CurrentSize = gameObject.GetCurrentSize();
        OriginalSizeByCollider = gameObject.GetOriginalSizeByCollider();
        OriginalSizeByMeshFilter = gameObject.GetOriginalSizeByMeshFilter();
        //ZeroSize = gameObject.GetZeroSize();
    }

    // Update is called once per frame
	void Update () {
        //Size = render.bounds.size;

	    GetSize();

	}
}
