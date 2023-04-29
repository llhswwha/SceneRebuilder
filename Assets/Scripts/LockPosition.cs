using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPosition : MonoBehaviour
{
    public bool Enabled = true;

    public float Interval = 1;

    public float MinY = 0;

    public float LimitMinY = -0.02f;

    void Start()
    {
        if(Enabled)
            StartLock();
    }

    [ContextMenu("StartLock")]
    public void StartLock()
    {
        CancelInvoke("LockY");
        InvokeRepeating("LockY",0, Interval);
    }

    private void LockY()
    {
        //Debug.Log("LockY");
        var p = transform.localPosition;
        if(LimitMinY>p.y)
            transform.localPosition = new Vector3(0, 0, 0);
    }

	// Update is called once per frame
	void FixedUpdate ()
	{
	    if (MinY > transform.localPosition.y)
	    {
	        MinY = transform.localPosition.y;

	    }
	}
}
