using UnityEngine;
using System.Collections;

public class TransformPosition : MonoBehaviour
{

    public Vector3 Pos;

    public Vector3 LocalPos;

    // Use this for initialization
    void Start ()
	{
        GetInfo();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    [ContextMenu("GetInfo")]
    public void GetInfo()
    {
        Pos = transform.position;
        LocalPos = transform.localPosition;
    }
}
