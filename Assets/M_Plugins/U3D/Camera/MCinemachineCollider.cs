using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
/// </summary>
public class MCinemachineCollider : MonoBehaviour
{

    /// <summary>The Unity layer mask against which the collider will raycast.</summary>
    [Header("Obstacle Detection")]
    [Tooltip("The Unity layer mask against which the collider will raycast")]
    public LayerMask m_CollideAgainst = 1;

    /// <summary>Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag</summary>
    [Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
    public string m_IgnoreTag = string.Empty;

    /// <summary>Obstacles closer to the target than this will be ignored</summary>
    [Tooltip("Obstacles closer to the target than this will be ignored")]
    public float m_MinimumDistanceFromTarget = 0.1f;

    /// <summary>Useful constant for very small floats</summary>
    const float Epsilon = 0.0001f;

    /// <summary>
    /// The raycast distance to test for when checking if the line of sight to this camera's target is clear.
    /// </summary>
    [Tooltip("The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used.")]
    public float m_DistanceLimit = 0f;

    /// This must be small but greater than 0 - reduces false results due to precision
    const float PrecisionSlush = 0.001f;

    /// <summary>
    /// Camera will try to maintain this distance from any obstacle.  
    /// Increase this value if you are seeing inside obstacles due to a large 
    /// FOV on the camera.
    /// </summary>
    [Tooltip("Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera.")]
    public float m_CameraRadius = 0.1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject CameraObj;
    public GameObject TargetObj;
    public GameObject rejustobj;

    /// <summary>
    /// Test
    /// </summary>
    [ContextMenu("Test")]
    public void Test()
    {
        if (rejustobj != null)
        {
            DestroyImmediate(rejustobj);
        }
        Vector3 pos;
        PreserveLignOfSight(CameraObj.transform.position, TargetObj.transform.position, out pos);
        pos += CameraObj.transform.position;
        rejustobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rejustobj.transform.position = pos;
        rejustobj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        rejustobj.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置，并且返回是否被遮挡
    /// </summary>
    public bool GetPos(Transform oriTran, Transform targetTran, out Vector3 adjustPos)
    {
        //Vector3 pos;
        bool isbool = PreserveLignOfSight(oriTran.position, targetTran.position, out adjustPos);
        adjustPos += oriTran.position;
        return isbool;
    }

    /// <summary>
    /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置，并且返回是否被遮挡
    /// </summary>
    public bool GetPos(Vector3 cameraPos, Vector3 lookAtPos, out Vector3 adjustPos)
    {
        //Vector3 pos;
        bool isbool = PreserveLignOfSight(cameraPos, lookAtPos, out adjustPos);
        return isbool;
    }

    /// <summary>
    /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
    /// </summary>
    public Vector3 GetPos(Camera cam, Transform targetTran)
    {
        Vector3 pos = GetPos(cam.transform, targetTran);
        return pos;
    }

    /// <summary>
    /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
    /// </summary>
    public Vector3 GetPos(Transform oriTran, Transform targetTran)
    {
        Vector3 pos;
        PreserveLignOfSight(oriTran.position, targetTran.position, out pos);
        pos += oriTran.position;
        return pos;
    }

    private bool PreserveLignOfSight(Vector3 cameraPos, Vector3 lookAtPos, out Vector3 adjustPos)
    {
        bool isbool = false;
        Vector3 displacement = Vector3.zero;
        //if (state.HasLookAt)
        //{
        //Vector3 cameraPos = state.CorrectedPosition;
        //Vector3 lookAtPos = state.ReferenceLookAt;
        Vector3 pos = cameraPos;
        Vector3 dir = pos - lookAtPos;
        float targetDistance = dir.magnitude;
        float minDistanceFromTarget = Mathf.Max(m_MinimumDistanceFromTarget, Epsilon);
        if (targetDistance > minDistanceFromTarget)
        {
            dir.Normalize();
            float rayLength = targetDistance - minDistanceFromTarget;
            if (m_DistanceLimit > Epsilon)
                rayLength = Mathf.Min(m_DistanceLimit, rayLength);

            // Make a ray that looks towards the camera, to get the most distant obstruction
            Ray ray = new Ray(pos - rayLength * dir, dir);
            rayLength += PrecisionSlush;
            if (rayLength > Epsilon)
            {
                RaycastHit hitInfo;
                if (RaycastIgnoreTag(ray, out hitInfo, rayLength))
                {
                    isbool = true;
                    // Pull camera forward in front of obstacle
                    float adjustment = Mathf.Max(0, hitInfo.distance - PrecisionSlush);
                    pos = ray.GetPoint(adjustment);
                    //extra.AddPointToDebugPath(pos);
                    //if (m_Strategy != ResolutionStrategy.PullCameraForward)
                    //{
                    //    pos = PushCameraBack(
                    //        pos, dir, hitInfo, lookAtPos,
                    //        new Plane(state.ReferenceUp, cameraPos),
                    //        targetDistance, m_MaximumEffort, ref extra);
                    //}
                }
            }
        }
        if (m_CameraRadius > Epsilon)
            pos += RespectCameraRadius(pos, lookAtPos);
        //else if (mCameraColliderGameObject != null)
        //    CleanupCameraCollider();
        displacement = pos - cameraPos;
        //}
        adjustPos = displacement;
        return isbool;
    }

    private bool RaycastIgnoreTag(Ray ray, out RaycastHit hitInfo, float rayLength)
    {
        //while (Physics.Raycast(
        //    ray, out hitInfo, rayLength, m_CollideAgainst.value,
        //    QueryTriggerInteraction.Ignore))//忽略IsTirrger=true的Collier
        while (Physics.Raycast(ray, out hitInfo, rayLength, m_CollideAgainst.value, QueryTriggerInteraction.Collide))//碰撞所有Collier
        {
            if (m_IgnoreTag.Length == 0 || !hitInfo.collider.CompareTag(m_IgnoreTag))
                return true;

            // Pull ray origin forward in front of tagged obstacle
            Ray inverseRay = new Ray(ray.GetPoint(rayLength), -ray.direction);
            if (!hitInfo.collider.Raycast(inverseRay, out hitInfo, rayLength))
                break; // should never happen!
            rayLength = hitInfo.distance - PrecisionSlush;
            if (rayLength < Epsilon)
                break;
            ray.origin = inverseRay.GetPoint(rayLength);
        }
        return false;
    }

    private Collider[] mColliderBuffer = new Collider[5];
    private SphereCollider mCameraCollider;
    private GameObject mCameraColliderGameObject;
    private Vector3 RespectCameraRadius(Vector3 cameraPos, Vector3 lookAtPos)
    {
        Vector3 result = Vector3.zero;
        int numObstacles = Physics.OverlapSphereNonAlloc(
            cameraPos, m_CameraRadius, mColliderBuffer,
            m_CollideAgainst, QueryTriggerInteraction.Ignore);
        if (numObstacles > 0)
        {
            if (mCameraColliderGameObject == null)
            {
                mCameraColliderGameObject = new GameObject("Cinemachine Collider Collider");
                mCameraColliderGameObject.hideFlags = HideFlags.HideAndDontSave;
                mCameraColliderGameObject.transform.position = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                mCameraColliderGameObject.SetActive(true);
                mCameraCollider = mCameraColliderGameObject.AddComponent<SphereCollider>();
            }
            mCameraCollider.radius = m_CameraRadius;
            for (int i = 0; i < numObstacles; ++i)
            {
                Collider c = mColliderBuffer[i];
                if (m_IgnoreTag.Length > 0 && c.CompareTag(m_IgnoreTag))
                    continue;
                Vector3 dir;
                float distance;
                if (Physics.ComputePenetration(
                    mCameraCollider, cameraPos, Quaternion.identity,
                    c, c.transform.position, c.transform.rotation,
                    out dir, out distance))
                {
                    result += dir * distance;   // naive, but maybe enough
                }
            }
        }
        return result;
    }

    private void CleanupCameraCollider()
    {
        if (mCameraColliderGameObject != null)
            DestroyImmediate(mCameraColliderGameObject);
        mCameraColliderGameObject = null;
        mCameraCollider = null;
    }

    /// <summary>
    /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
    /// </summary>
    public Vector3 GetPos_Simple(Camera cam, Transform targetTran)
    {
        return GetPos_Simple(cam.transform, targetTran);
    }

    /// <summary>
    /// 通过检测，摄像机和目标物体之间是否被遮挡，获取摄像机合适的位置
    /// </summary>
    public Vector3 GetPos_Simple(Transform oriTran, Transform targetTran)
    {
        RaycastHit hit;
        bool obstacle = Physics.Raycast(oriTran.position, (oriTran.position - targetTran.position).normalized, out hit, Vector3.Distance(oriTran.position, targetTran.position));

        if (obstacle)
        {
            return hit.point;
        }

        return oriTran.position;
    }
}
