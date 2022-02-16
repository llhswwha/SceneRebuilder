using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODGenerate : MonoBehaviour
{

    //public float[] Lods=new float[] { 0.6f, 0.2f, 0.01f };

    //public float[] simplifiers=new float[] { 1f,0.7f,0.2f };

    [ContextMenu("CreateLOD")]
    public void CreateLOD()
    {
        //AutomaticLODHelper.CreateLOD(this.gameObject, null, Lods,simplifiers,false);

        LODManager.Instance.CreateAutoLOD(this.gameObject,p=>
        {
            ProgressBarHelper.DisplayProgressBar("CreateLOD", $"{p:P}", p);
        });
        ProgressBarHelper.ClearProgressBar();
    }

    [ContextMenu("RemoveLOD")]
    public void RemoveLOD()
    {
        //AutomaticLODHelper.CreateLOD(this.gameObject, null, Lods,simplifiers,false);

        LODManager.Instance.RemoveLOD(this.gameObject);
    }

    [ContextMenu("CreateLOD1")]
    public void CreateLOD1()
    {
        CreateLODN(0.75f);
    }
    [ContextMenu("CreateLOD2")]
    public void CreateLOD2()
    {
        CreateLODN(0.5f);
    }
    [ContextMenu("CreateLOD3")]
    public void CreateLOD3()
    {
        CreateLODN(0.25f);
    }
    [ContextMenu("CreateLOD4")]
    public void CreateLOD4()
    {
        CreateLODN(0.1f);
    }

    private void CreateLODN(float percent)
    {
        LODManager.Instance.CreateAutoLOD(this.gameObject, percent, p =>
        {
            ProgressBarHelper.DisplayProgressBar($"CreateLODN({percent})", $"{p:P}", p);
        });
        ProgressBarHelper.ClearProgressBar();
    }
}
