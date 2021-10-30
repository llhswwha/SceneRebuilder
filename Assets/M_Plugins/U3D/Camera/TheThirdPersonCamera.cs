using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheThirdPersonCamera : MonoBehaviour {

    public float distanceAway = 1.7f;
    public float distanceUp = 1.3f;
    public float smooth = 2f;               // how smooth the camera movement is

    private Vector3 m_TargetPosition;       // the position the camera is trying to be in)

    Transform follow;        //the position of Player

    private MCinemachineCollider mcinemachineCollider;

    void Start()
    {
        //follow = GameObject.FindWithTag("Player").transform;
        mcinemachineCollider = GetComponent<MCinemachineCollider>();
    }

    void Update()
    {
        //if (follow != null)
        //{
        //    // setting the target position to be the correct offset from the 
        //    m_TargetPosition = follow.position + Vector3.up * distanceUp - follow.forward * distanceAway;

        //    // making a smooth transition between it's current position and the position it wants to be in
        //    //transform.position = Vector3.Lerp(transform.position, m_TargetPosition, Time.deltaTime * smooth);

        //    if (CameraSceneManager.Instance.alignCamera.isCameraCollider)
        //    {
        //        SetPosition(m_TargetPosition);
        //    }
        //    else
        //    {
        //        transform.position = Vector3.Lerp(transform.position, m_TargetPosition, Time.deltaTime * smooth);
        //    }

        //    // make sure the camera is looking the right way!
        //    transform.LookAt(follow);


        //}
    }

    void LateUpdate()
    {
        if (follow != null)
        {
            // setting the target position to be the correct offset from the 
            m_TargetPosition = follow.position + Vector3.up * distanceUp - follow.forward * distanceAway;

            // making a smooth transition between it's current position and the position it wants to be in
            //transform.position = Vector3.Lerp(transform.position, m_TargetPosition, Time.deltaTime * smooth);

            //if (CameraSceneManager.Instance.alignCamera.isCameraCollider)
            //{
            //    SetPosition(m_TargetPosition);
            //}
            //else
            {
                transform.position = Vector3.Lerp(transform.position, m_TargetPosition, Time.deltaTime * smooth);
            }

            // make sure the camera is looking the right way!
            transform.LookAt(follow);


        }
    }

    /// <summary>
    /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
    /// </summary>
    public void SetPosition(Vector3 currentpos)
    {
        if (mcinemachineCollider != null)
        {
            //Vector3 adjustPos = mcinemachineCollider.GetPos(GetComponent<Camera>(), follow);
            //transform.position = adjustPos;

            Vector3 adjustPos;
            bool isbool = mcinemachineCollider.GetPos(currentpos, follow.position, out adjustPos);

            //targetDistance = Vector3.Distance(adjustPos, target.transform.position);
            //CurrentDistance = targetDistance;
            if (isbool)
            {
                Vector3 targetToCamdir = currentpos - follow.position;
                float offset = targetToCamdir.magnitude - adjustPos.magnitude;
                //adjustPos += transform.position;
                Vector3 tpos = targetToCamdir.normalized * offset + follow.position;

                Debug.LogError("currentpos:" + currentpos + "||currentpos:" + adjustPos);
                Vector3 p = Vector3.Lerp(transform.position, tpos, smooth * Time.deltaTime);
                transform.position = p;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, m_TargetPosition, Time.deltaTime * smooth);
            }
        }
    }

    /// <summary>
    /// 设置跟随对象
    /// </summary>
    public void SetFollow(Transform tran)
    {
        follow = tran;
    }


}
