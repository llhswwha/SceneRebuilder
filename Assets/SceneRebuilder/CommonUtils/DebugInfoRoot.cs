using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInfoRoot : MonoBehaviour
{
    public static void ClearDebugInfoGos(Transform target)
    {
        DebugInfoRoot[] debugRoots = target.GetComponentsInChildren<DebugInfoRoot>(true);
        foreach (var item in debugRoots)
        {
            if (item == null) continue;
            GameObject.DestroyImmediate(item.gameObject);
        }
    }

    public static GameObject NewGo(string name,Transform parent)
    {
        GameObject planInfoRoot = new GameObject(name);
        planInfoRoot.AddComponent<DebugInfoRoot>();
        planInfoRoot.transform.SetParent(parent);
        planInfoRoot.transform.localPosition = Vector3.zero;
        return planInfoRoot;
    }

    public static GameObject CreateSubTestObj(string objName, Transform parent)
    {
        GameObject objTriangles = new GameObject(objName);
        objTriangles.AddComponent<DebugInfoRoot>();
        objTriangles.transform.SetParent(parent);
        objTriangles.transform.localPosition = Vector3.zero;
        objTriangles.transform.localRotation = Quaternion.identity;
        return objTriangles;
    }
}
