using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool result = false;

    public List<Transform> allList = new List<Transform>();
    public List<Transform> errorList = new List<Transform>();
    [ContextMenu("Check")]
    public void Check()
    {
        errorList.Clear();
        allList.Clear();
        allList = GetAllTransforms(this.transform);
        allList.Add(this.transform);
        this.result = true;
        foreach (Transform item in allList)
        {
            Vector3 eulerAngles = item.rotation.eulerAngles;
            bool isZero = eulerAngles.Equals( Vector3.zero);
            isZero = IsZero(eulerAngles);
            float distance = Vector3.Distance(eulerAngles, Vector3.zero);
            //distance = GetDistance(eulerAngles, Vector3.zero);
            if (distance>0.0001f)
            {
                this.result = false;
                errorList.Add(item);
                //break;

               
            }
            else
            {

            }
            Debug.Log(string.Format("eulerAngles:({0},{1},{2}),{3},{4},{5}", eulerAngles.x, eulerAngles.y, eulerAngles.z, item, (isZero), distance));
        }
        //result = true;


        Debug.Log("Check:"+result);

    }

    public static bool IsZero(Vector3 v)
    {
        float min = 0.0001f;
        return Mathf.Abs(v.x) < min && Mathf.Abs(v.y) < min && Mathf.Abs(v.z) < min;
    }

    public static float GetDistance(Vector3 p1,Vector3 p2)
    {
        float x = Mathf.Pow(p1.x * p1.x + p2.x * p2.x, 0.5f);
        float y = Mathf.Pow(p1.y * p1.y + p2.y * p2.y, 0.5f);
        float z = Mathf.Pow(p1.z * p1.z + p2.z * p2.z, 0.5f);
        float d = Mathf.Pow(x * x + y*y+z*z, 0.5f);
        Debug.Log(string.Format("Distance:({0},{1},{2}),{3}", x,y,z,d));
        return d;
    }

    public List<Transform> GetAllTransforms(Transform parentT)
    {
        List<Transform> list = new List<Transform>();
        for(int i = 0; i < parentT.childCount; i++)
        {
            Transform t = parentT.GetChild(i);
            list.Add(t);

            List<Transform> subList = GetAllTransforms(t);
            list.AddRange(subList);
        }
        return list;
    }
}
