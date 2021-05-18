using RevitTools.Infos;
using System.Collections;
using System.Collections.Generic;
// using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class NodeInfoScript : MonoBehaviour
{
    public static NodeInfoScript Current;

    public NodeInfo NodInfo;

    public string GetNodeId()
    {
        string id = NodInfo.GetId();
        if (string.IsNullOrEmpty(id))
        {
            id = NodeInfo.GetId(this.name);
            NodInfo.Id = id;
        }
        return id;
    }

    public ElementInfo EleInfo;

    public void OnMouseEnter()
    {
        //print("MouseEnter:" + this);
        Current = this;
    }

    public void OnMouseExit()
    {
        //print("MouseExit:" + this);
        if (Current = this)
        {
            Current = null;
        }
    }

    public void OnMouseDown()
    {
        print("OnMouseDown:" + this);
    }
}
