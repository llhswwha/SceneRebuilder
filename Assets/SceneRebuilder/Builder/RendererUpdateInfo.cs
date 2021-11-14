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
    OldDelete,//ɾ���ϵĲ������˵�ģ��,
    NewAdd,
    NewChanged,//
    NewSame,//MeshDis==0
    NewDelete,//�µ�ģ�ͺ��ϵ�һ����ɾ���µ�ģ��
    MatNew,
    MatOld
}