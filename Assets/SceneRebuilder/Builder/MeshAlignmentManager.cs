using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAlignmentManager : SingletonBehaviour<MeshAlignmentManager>
{
    public TransfromAlignSetting transfromReplaceSetting;

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
        tdSource = new TransformData(Source);
        tdTarget = new TransformData(Target);

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

    [ContextMenu("DoAlign")]
    public void DoAlign()
    {
        Recover();
        Init();
        MeshHelper.Align(Source, Target, transfromReplaceSetting);
        float dis = MeshHelper.GetVertexDistanceEx(Source, Target);
        Debug.Log($"DoAlign dis:{dis} Source:{Source} Target:{Target}");
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
