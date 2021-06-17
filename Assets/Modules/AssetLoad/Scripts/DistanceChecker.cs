using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DistanceChecker : MonoBehaviour
{
    public Bounds bounds;

    public Bounds roamBounds;

    public Bounds roamBoundsOut;

    public float roamDistancePower = 1.4f;//漫游时的包围盒放大倍数

    public float roamDistanceOutPower = 1.6f;//漫游时的包围盒放大倍数

    public bool isInside = false;

    public bool isOutside = true;

    public Bounds freeBounds;

    public float freeDistancePower = 2f;//俯视时的包围盒放大倍数

    public float freeRadius = 0;

    public float roamRadius = 0;

    public float roamRadiusOut = 0;

    public float distanceIn = 0;

    public bool IsInBounds(Transform t)
    {
        var result = bounds.Contains(t.position);
        return result;
    }

    public bool IsInDistance(Transform t, bool isRoam,bool isRadius=false)
    {
        //CreateDistanceCheckBoxCollider();
        //IsIn = checkDistanceCollider.bounds.Contains(t.position)；不能之间用BoxCollider判断，BoxCollider不激活时判断有问题。
        bool result = false;
        if (isRoam)
        {
            if (isRadius)
            {
                distanceIn = Vector3.Distance(t.position, transform.position);
                result = distanceIn <= roamRadius;
            }
            else
            {
                result = roamBounds.Contains(t.position);
            }
            
        }
        else
        {
            distanceIn = Vector3.Distance(t.position, transform.position);
            result = distanceIn <= freeRadius;
        }
        isInside = result;
        return isInside;
    }

    public float distanceOut = 0;

    public bool IsOutDistance(Transform t, bool isRoam, bool isRadius = false)
    {
        //CreateDistanceCheckBoxCollider();
        //IsIn = checkDistanceCollider.bounds.Contains(t.position)；不能之间用BoxCollider判断，BoxCollider不激活时判断有问题。
        bool result = true;
        if (isRoam)
        {
            if (isRadius)
            {
                distanceOut = Vector3.Distance(t.position, transform.position);
                result = distanceOut > roamRadiusOut;
            }
            else
            {
                result = roamBoundsOut.Contains(t.position);
            }
        }
        else
        {
            distanceOut = Vector3.Distance(t.position, transform.position);
            //isInside = freeBounds.Contains(t.position);
            result = distanceOut > freeRadius * 3;
        }
        isOutside = result;
        return isOutside;
    }


    //[ContextMenu("CreateDistanceCheckBoxCollider")]
    //public void CreateDistanceCheckBoxCollider()
    //{
    //    if (haveRoamBounds == false)
    //    {
    //        haveRoamBounds = true;
    //        //if (ShowCollider)
    //        //{
    //        //    BoxCollider checkDistanceCollider = CreateBoxCollider(gameObject.transform);
    //        //    checkDistanceCollider.size *= DistancePower;
    //        //    bounds = checkDistanceCollider.bounds;
    //        //    checkDistanceCollider.enabled = false;
    //        //}
    //        roamBounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
    //        roamBounds.size *= roamDistancePower;
    //        //bounds= bounds2
    //    }
    //}
    //private bool ShowCollider = true;

    [ContextMenu("InitDistanceBounds")]
    public void InitDistanceBounds()
    {
        bounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了

        roamBounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
        roamBounds.size *= roamDistancePower;

        roamBoundsOut = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
        roamBoundsOut.size *= roamDistanceOutPower;

        freeBounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
        freeBounds.size *= freeDistancePower;

        freeRadius = ColliderHelper.GetRadius(freeBounds.size);
        freeRadius /= transform.lossyScale.x;//模型本身可能有比例参数

        roamRadius = ColliderHelper.GetRadius(roamBounds.size);
        roamRadius /= transform.lossyScale.x;//模型本身可能有比例参数

        roamRadiusOut = ColliderHelper.GetRadius(roamBoundsOut.size);
        roamRadiusOut /= transform.lossyScale.x;//模型本身可能有比例参数

        //#if UNITY_EDITOR
        //ShowDistanceCollider();
        //#endif
    }

    [ContextMenu("ShowDistanceCollider")]
    private void ShowDistanceCollider()
    {
        BoxCollider collider1 = ColliderHelper.CreateBoxCollider(gameObject.transform, false, true);
        collider1.size *= roamDistancePower;
        collider1.enabled = false;

        BoxCollider collider11 = ColliderHelper.CreateBoxCollider(gameObject.transform, false, true);
        collider11.size *= roamDistancePower*3;
        collider11.enabled = false;

        BoxCollider collider2 = ColliderHelper.CreateBoxCollider(gameObject.transform, false, true);
        collider2.size *= freeDistancePower;
        collider2.enabled = false;

        SphereCollider collider3 = ColliderHelper.CreateSphereCollider(gameObject.transform, false, true);
        collider3.radius *= freeDistancePower;
        //collider3.radius = radius;
        collider3.enabled = false;
    }

    [ContextMenu("TestCheckPos")]
    private void TestCheckPos()
    {
        foreach (var item in tmp)
        {
            GameObject.DestroyImmediate(item);
        }
        Vector3 pos = Vector3.zero;
        pos = roamBounds.size;

        pos = pos / 2;
        var result = true;

        CheckPos(transform.position + pos + Vector3.one);
        CheckPos(transform.position + pos);
        CheckPos(transform.position + pos - Vector3.one);

        for (int i = 0; i < 1000 && result == true; i++)
        {
            result = CheckPos(transform.position + Vector3.one * i * 0.1f);
        }
    }

    public List<GameObject> tmp = new List<GameObject>();

    private bool CheckPos(Vector3 pos)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.position = pos;
        obj.name = pos + "";
        obj.transform.parent = transform;
        tmp.Add(obj);
        //var result = checkDistanceCollider.bounds.Contains(pos);
        var result = roamBounds.Contains(pos);
        Debug.Log("pos:" + pos + " result:" + result);
        return result;
    }
}
