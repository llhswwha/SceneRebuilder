using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyRoot : MonoBehaviour
{
    public GameObject root;

    public IdInfoList IdInfoList = null;

    [ContextMenu("LoadXml")]
    public void LoadXml()
    {
        if (root == null)
        {
            root = this.gameObject;
        }
        IdInfoList = HierarchyHelper.LoadHierarchy(root, true);
    }

    [ContextMenu("SaveXml")]
    public void SaveXml()
    {
        if (root == null)
        {
            root = this.gameObject;
        }
        IdInfoList = HierarchyHelper.SaveHierarchy(root);
    }

    [ContextMenu("Check")]
    public void Check()
    {
        if (root == null)
        {
            root = this.gameObject;
        }
        IdInfoList = HierarchyHelper.CheckHierarchy(root);
    }
}
