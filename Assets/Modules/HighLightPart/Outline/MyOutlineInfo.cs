using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyOutlineInfo : MonoBehaviour
{
    [ColorUsage(false,true)]
    public Color color;

    // public bool Keep=false;

    // public int KeepLayer=0;

    // void Update(){
    //     if(Keep){
    //         if(gameObject.layer!=this.KeepLayer)
    //         {
    //             MyOutlineManager.Instance.SelectGOsBuffer(gameObject);
    //         }
    //     }
    // }
}
