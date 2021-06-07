using UnityEngine;
using System.Collections;

public class GameObjectInfo : MonoBehaviour
{

    public Vector3 Size ;

    public Vector3 Position;

    public Vector3 LocalPosition;

    public Vector3 Scale;

    public Vector3 LocalScale;

    public Vector3 Angle;

    public Vector3 LocalAngle;

    public Quaternion Rotation;

    public Quaternion LocalQuaternion;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        GetInfo();
    }

    [ContextMenu("GetInfo")]
    public void GetInfo()
    {
        Size = gameObject.GetSize();

        Position = transform.position;
        LocalPosition = transform.localPosition;

        Scale = transform.lossyScale;
        LocalScale = transform.localScale;

        Rotation = transform.rotation;
        LocalQuaternion = transform.localRotation;

        Angle = transform.eulerAngles;
        LocalAngle = transform.localEulerAngles;
    }
}
