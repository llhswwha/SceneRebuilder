using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Mesh.
    }

    public Space relativeTo = Space.Self;

    public float rx;
    public float ry;
    public float rz;

    [ContextMenu("DoForword")]
    public void DoForword()
    {
        //transform.Rotate(eluers, relativeTo);
        transform.Rotate(rx,ry,rz,relativeTo);
    }

    [ContextMenu("DoBackword")]
    public void DoBackword()
    {
        //transform.Rotate(-eluers, relativeTo);
        transform.Rotate(0, 0, -rz, relativeTo);
        transform.Rotate(0, -ry, 0, relativeTo);
        transform.Rotate(-rx, 0, 0, relativeTo);
        //逆向旋转需要明确先z再y再x，因为正向是先x,y,z的，就算是 -x,-y,-z，也是转不回来的。
    }

    //public Vector3 eluers;
}
