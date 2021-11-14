using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererUpdateInfo : MonoBehaviour
{
    public LODTwoRenderers twoRenderers;

    public UpdateChangedMode changedMode;

    public bool IsNew = false;
}


public enum UpdateChangedMode
{
    None,
    OldDelete,//删除老的不存在了的模型,
    NewAdd,
    NewChanged,//
    NewSame,//MeshDis==0
    NewDelete,//新的模型和老的一样，删除新的模型
    MatNew,
    MatOld
}