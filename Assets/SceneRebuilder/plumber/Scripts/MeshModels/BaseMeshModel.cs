﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeshModel : MonoBehaviour
{
    public int VertexCount = 0;

    public void ShowOBB()
    {
        OBBCollider.ShowOBB(this.gameObject, true);
    }

    public virtual void GetModelInfo()
    {

    }

    public virtual void GetModelInfo_Job()
    {

    }

    public void ClearDebugInfoGos()
    {
        DebugInfoRoot[] debugRoots = this.GetComponentsInChildren<DebugInfoRoot>(true);
        foreach (var item in debugRoots)
        {
            if (item == null) continue;
            GameObject.DestroyImmediate(item.gameObject);
        }
    }

    [ContextMenu("ClearChildren")]
    public void ClearChildren()
    {
        TransformHelper.ClearChildren(gameObject);
        //ClearCheckDistance();
    }

    public GameObject CreateDebugInfoRoot(string goName)
    {
        GameObject go0 = new GameObject(goName);
        go0.AddComponent<DebugInfoRoot>();
        go0.transform.SetParent(this.transform);
        go0.transform.localPosition = Vector3.zero;
        //DebugInfoRootGos.Add(go0);
        return go0;
    }
}
