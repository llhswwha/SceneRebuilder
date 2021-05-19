using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCreater : MonoBehaviour
{
    public int a;
    public Bounds MeshBounds;
    public Vector3 boundsSize;

    public Vector3 min;

    public Vector3 max;

    public Vector3 minMaxSize;

    public Vector3 position;

    public Vector3 scale;

    public Collider newCollider;

    public GameObject newGo;

    private void GetInfo()
    {
        MeshBounds = this.gameObject.GetComponent<MeshRenderer>().bounds;
        position = this.transform.position;
        scale = this.transform.lossyScale;

        boundsSize = this.MeshBounds.size;

        min = MeshBounds.min;
        max = MeshBounds.max;
        minMaxSize = max - min;


    }

    [ContextMenu("Create1")]
    public void Create1()
    {
        GetInfo();

        newCollider = this.gameObject.AddComponent<BoxCollider>();
    }

    [ContextMenu("Create2")]
    public void Create2()
    {
        GetInfo();


        //newGo = new GameObject("Culling Collider");
        //newGo.transform.position = this.transform.position;
        //newGo.transform.rotation = this.transform.rotation;
        //newGo.transform.localScale = this.transform.lossyScale;

        //BoxCollider boxCollider = newGo.AddComponent<BoxCollider>();
        //boxCollider.center = MeshBounds.center - transform.position;
        //boxCollider.size = MeshBounds.size;
        //newGo.transform.SetParent(this.transform);
        //newCollider = boxCollider;

        BoxCollider box1 = this.gameObject.AddComponent<BoxCollider>();
        newGo = new GameObject("Culling Collider");
        newGo.transform.position = this.transform.position;
        newGo.transform.rotation = this.transform.rotation;
        newGo.transform.localScale = this.transform.lossyScale;
        BoxCollider box2 = newGo.AddComponent<BoxCollider>();
        box2.center = box1.center;
        box2.size = box1.size;
        newGo.transform.SetParent(this.transform);
        GameObject.Destroy(box1);
        newCollider = box2;
    }

    public static BoxCollider AddBoxCollider(Transform parent, Bounds bounds)
    {
        BoxCollider box1 = parent.gameObject.AddComponent<BoxCollider>();
        GameObject newGo = new GameObject("Culling Collider");
        newGo.transform.position = parent.position;
        newGo.transform.rotation = parent.rotation;
        newGo.transform.localScale = parent.lossyScale;
        BoxCollider box2 = newGo.AddComponent<BoxCollider>();
        box2.center = box1.center;
        box2.size = box1.size;
        newGo.transform.SetParent(parent);
        GameObject.Destroy(box1);
        //newCollider = box2;
        return box2;
    }
}
