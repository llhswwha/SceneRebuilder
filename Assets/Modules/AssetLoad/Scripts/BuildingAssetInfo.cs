using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAssetInfo : AssetBundleInfo
{
    void Awake()
    {
        BuildingController bc = gameObject.GetComponent<BuildingController>();
        if (bc == null) //J6J11的情况
        {
            BoxCollider collider= gameObject.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
 
}
