using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAlignmentManager : SingletonBehaviour<MeshAlignmentManager>
{
    public TransfromAlignSetting transfromReplaceSetting = new TransfromAlignSetting();

    public GameObject Source;

    public GameObject Target;

    public TransformData tdSource;

    public TransformData tdTarget;

    public GameObject SourceRoot;

    //public GameObject TargetRoot;

    public TransformData tdSourceRoot;

    //public TransformData tdTargetRoot;

    [ContextMenu("Init")]
    public void Init()
    {
        if(Source!=null)
            tdSource = new TransformData(Source);
        if(Target!=null)
            tdTarget = new TransformData(Target);
        if(SourceRoot!=null)
            tdSourceRoot = new TransformData(SourceRoot);
        //tdTargetRoot = new TransformData(TargetRoot);
    }

    public void DoAlign(GameObject source,GameObject target)
    {
        this.Source = source;
        this.Target = target;
        Init();
        DoAlign();
    }

    public float Zero = 0.00005f;

    public bool IsDestroy = false;

    [ContextMenu("CopyAndAlign")]
    public void CopyAndAlign()
    {
        if (Source == null)
        {
            Debug.LogError("CopyAndAlign Source == null");
            return;
        }
        GameObject goNew=MeshHelper.CopyGO(Source);
        Source = goNew;
        float dis = DoAlign();

        Target.SetActive(false);
        Source.name = Target.name;

        if (IsDestroy && dis < Zero)
        {
            EditorHelper.UnpackPrefab(Target);
            GameObject.DestroyImmediate(Target);
        }
    }

    [ContextMenu("DoAlign")]
    public float DoAlign()
    {
        Recover();
        Init();
        MeshHelper.Align(Source, Target, transfromReplaceSetting);
        float dis = MeshHelper.GetVertexDistanceEx(Source, Target);
        Debug.Log($"DoAlign dis:{dis} Source:{Source} Target:{Target}");
        return dis;
    }

    [ContextMenu("DoAlignRoot")]
    public void DoAlignRoot()
    {
        Recover();
        Init();
        MeshHelper.AlignRoot(Source, Target, SourceRoot, transfromReplaceSetting);
    }

    [ContextMenu("Recover")]
    public void Recover()
    {
        tdSourceRoot.Recover();
        //tdTargetRoot.Recover();

        tdSource.Recover();
        tdTarget.Recover();

    }
}
