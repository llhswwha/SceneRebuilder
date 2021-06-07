using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRotate : MonoBehaviour
{
    public float Interval = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("UpdateRotate", 0, Interval);

        //StartCoroutine(MeshHelper.RotateUntilMinDistance(target.transform, this.transform, "",minDistance,null));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int rx = 0;
    public int ry = 0;
    public int rz = 0;

    public GameObject target;

    public double distance = 0;

    public float minDistance = 0.01f;

    [ContextMenu("UpdateRotate")]
    public void UpdateRotate()
    {
        if (distance < minDistance)
        {
            return;
        }
        transform.localEulerAngles = new Vector3(90 * rx, 90 * ry, 90 * rz);
        rx++;
        if (rx == 4)
        {
            rx = 0;
            ry++;
            if (ry == 4)
            {
                ry = 0;
                rz++;
                if (rz == 4)
                {
                    rz = 0;
                }
            }
        }
        GetDistance();
    }

    [ContextMenu("GetDistance")]
    public void GetDistance()
    {
        if (target != null)
        {
            //distance = MeshHelper.GetVertexDistance(target.transform, this.transform);
            distance = MeshHelper.GetVertexDistanceEx(target.transform, this.transform);
        }
    }
}
