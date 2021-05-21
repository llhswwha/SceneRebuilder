using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODGenerate : MonoBehaviour
{

    public float[] Lods=new float[] { 0.6f, 0.2f, 0.01f };

    public float[] simplifiers=new float[] { 1f,0.7f,0.2f };

    [ContextMenu("Create")]
    public void Create()
    {
        AutomaticLODHelper.CreateLOD(this.gameObject, null, Lods,simplifiers,false);
    }
}
