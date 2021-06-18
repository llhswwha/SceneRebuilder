using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSize : MonoBehaviour
{
    public MeshFilter meshFiter;
    public Bounds bounds;

    public Vector3 size;

    public Vector3 scale;

    public float Length=0;

    [ContextMenu("GetSize")]
    public void GetSize()
    {
        meshFiter=this.gameObject.GetComponent<MeshFilter>();
        if (meshFiter == null)
        {
            bounds = ColliderHelper.CaculateBounds(this.gameObject);
        }
        else
        {
            bounds = meshFiter.sharedMesh.bounds;
        }
        
        size=bounds.size;
        scale = this.transform.lossyScale;
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.y));
        Length = size.x * scale.x;
        if (size.y * scale.y > Length)
        {
            Length = size.y * scale.y;
        }
        if (size.z * scale.z > Length)
        {
            Length = size.z * scale.y;
        }
    }
}
