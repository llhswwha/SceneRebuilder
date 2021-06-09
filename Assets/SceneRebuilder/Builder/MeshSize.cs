using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSize : MonoBehaviour
{
    public MeshFilter meshFiter;
    public Bounds bounds;

    public Vector3 size;

    public float Length=0;

    [ContextMenu("GetSize")]
    public void GetSize()
    {
        meshFiter=this.gameObject.GetComponent<MeshFilter>();
        bounds=meshFiter.sharedMesh.bounds;
        size=bounds.size;

        Length=size.x;
        if(Length < size.y){
            Length=size.y;
        }
        if(Length < size.z){
            Length=size.z;
        }
    }
}
