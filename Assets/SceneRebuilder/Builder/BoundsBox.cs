using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsBox : NoCombine
{
    //public void OnEnable()
    //{
    //    Debug.LogError($"BoundsBox.OnEnable:{this.name}");
    //}

    //public void OnDisable()
    //{
    //    Debug.LogError($"BoundsBox.OnDisable:{this.name}");
    //}

    public void DestroyMesh()
    {
        if (this.name.Contains("door_Bounds"))
        {
            return;
        }
        MeshHelper.RemoveMeshComponents(this.gameObject);

        AreaTreeNode node = GetComponent<AreaTreeNode>();
        if (node != null)
        {
            if (this.transform.childCount == 0)
            {
                GameObject.DestroyImmediate(this.gameObject);
            }
            else
            {
                //Debug.LogError($"BoundsBox.Awake name:{this.name} childCount:{this.transform.childCount}");
            }
            MeshHelper.RemoveMeshComponents(gameObject);
        }
        else
        {
            if (this.name.Contains("LODBox"))
            {

            }
            else
            {
                MeshHelper.RemoveMeshComponents(gameObject);
            }
        }
    }

    private void Awake()
    {
        //#if !UNITY_EDITOR

        //#endif
        if (this.name.Contains("door_Bounds"))
        {
            return;
        }
        MeshHelper.RemoveMeshComponents(this.gameObject);
    }
}
