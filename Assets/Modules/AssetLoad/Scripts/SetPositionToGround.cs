using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionToGround : MonoBehaviour {

    public Vector3 localPosition;

    public Vector3 position;

    public Vector3 size;

    // Use this for initialization
    void Start () {
        GetPosition();
    }

    [ContextMenu("GetPosition")]
    void GetPosition()
    {
        localPosition = transform.localPosition;
        position = transform.position;
        size = gameObject.GetSize();
    }

    public bool ShowSphere = false;

    public List<GameObjectPosition> targetList = new List<GameObjectPosition>();
	
    [ContextMenu("AjustSelf")]
	void AjustSelf()
    {
        AjustModel(gameObject);
    }

    [ContextMenu("AjustChildren")]
    void AjustChildren()
    {
        targetList.Clear();
        var meshRenderers = gameObject.FindComponentsInChildren<MeshRenderer>();
        foreach (var item in meshRenderers)
        {
            targetList.Add(new GameObjectPosition(item.gameObject));
        }
        foreach (var target in targetList)
        {
            bool r=AjustModel(target);
            if (r == false)
            {
                Other.Add(target.target);
            }
        }
        targetList.Sort();
    }

    [ContextMenu("Recovery")]
    void Recovery()
    {
        foreach (var target in targetList)
        {
            AjustModel(target);
        }
    }

    bool AjustModel(GameObjectPosition gp)
    {
        bool r=AjustModel(gp.target);
        if (r)
        {
            gp.SetNewPos();
        }
        return r;
    }

    public float MaxMoveDistance = 2;


    bool AjustModel(GameObject target)
    {
        var size = target.GetSize();
        Transform t = target.transform;
        RaycastHit hitInfo;
        if (Physics.Raycast(t.position, -t.up, out hitInfo))
        {
            CreateSphere("hit", hitInfo.point);
            Vector3 bottomPoint = t.position - new Vector3(0, size.y / 2f, 0);
            CreateSphere("bottom", bottomPoint);
            float moveDistance = bottomPoint.y - hitInfo.point.y;
            Debug.Log("moveDistance:" + moveDistance);
            if (Math.Abs(moveDistance) > MaxMoveDistance)
            {
                Debug.LogError("moveDistance > MaxMoveDistance:" + target);
                return false;
            }
            else
            {
                t.Translate(0, -moveDistance, 0);
                return true;
            }
            
        }
        else
        {
            Debug.LogError("Raycast 未碰撞:" + target);
            return false;
        }
    }

    public List<GameObject> Other = new List<GameObject>();

    private GameObject CreateSphere(string sName)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.name = sName;
        return sphere;
    }

    private GameObject CreateSphere(string sName,Vector3 position)
    {
        if (ShowSphere == false) return null;
        GameObject sphere = CreateSphere(sName);
        sphere.transform.position = position;
        sphere.transform.parent = transform;
        return sphere;
    }

    [ContextMenu("CreateTestSpheres")]
    void CreateTestSpheres()
    {
        GameObject sphere0 = CreateSphere("center", transform.position);

        GameObject sphere1 = CreateSphere("down", transform.position - transform.up);

        GameObject sphere2 = CreateSphere("forward", transform.position + transform.forward);
    }

    [Serializable]
    public class GameObjectPosition:IComparable<GameObjectPosition>
    {
        public string name;

        public GameObject target;

        public Vector3 position1;

        public Vector3 position2;

        public float distance = 0;

        public void SetNewPos()
        {
            position2 = target.transform.position;
            distance = Vector3.Distance(position1, position2);
        }

        public GameObjectPosition(GameObject t)
        {
            name = t.name;
            target = t;
            position1 = t.transform.position;
        }

        public void Recovery()
        {
            target.transform.position = position1;
        }

        public int CompareTo(GameObjectPosition other)
        {
            int r = other.distance.CompareTo(this.distance);
            if (r == 0)
            {
                return other.name.CompareTo(this.name);
            }
            else
            {
                return r;
            }
        }
    }
}
