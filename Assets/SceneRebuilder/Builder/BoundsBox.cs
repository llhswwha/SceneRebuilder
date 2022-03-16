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

    private void Awake()
    {
#if !UNITY_EDITOR
        if (this.transform.childCount == 0)
        {
            GameObject.DestroyImmediate(this.gameObject);
        }
        else
        {
            //Debug.LogError($"BoundsBox.Awake name:{this.name} childCount:{this.transform.childCount}");
        }
#endif

    }
}
