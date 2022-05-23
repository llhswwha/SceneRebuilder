using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtils;

public class MeshSize : MonoBehaviour
{
    public MeshFilter meshFiter;
    public Bounds bounds;

    public Vector3 size;

    public Vector3 scale;

    public float Length=0;

    public float Diam = 0;

    [ContextMenu("GetSize")]
    public void GetSize()
    {
        meshFiter=this.gameObject.GetComponent<MeshFilter>();
        if (meshFiter == null)
        {
            bounds = ColliderExtension.CaculateBounds(this.gameObject);
        }
        else
        {
            bounds = meshFiter.sharedMesh.bounds;
        }
        
        

        //Diam = Vector3.size.z * scale.z + size.z * scale.z + size.z * scale.z;

        size = bounds.size;
        scale = this.transform.lossyScale;
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.y));
        Length = size.x * scale.x;
        if (size.y * scale.y > Length)
        {
            Length = size.y * scale.y;
        }
        if (size.z * scale.z > Length)
        {
            Length = size.z * scale.z;
        }


        var minMax = VertexHelper.GetMinMax(meshFiter);
        //center = minMax[3];
        //size = minMax[2];
        Diam = Vector3.Distance(minMax[0], minMax[1]);
    }
}
