using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceArgs
{
    public Transform t1;
    public Transform t2;
    public bool isResetParent = false;
    public bool isResetPos = false;
    public bool isResetRotation = false;
    public string progress = "";

    public bool isLocal = false;//¾Ö²¿×ø±ê

    public bool showLog = false;

    public DistanceArgs(Transform t1, Transform t2, string progress, bool showLog, bool local = false)
    {
        this.t1 = t1;
        this.t2 = t2;
        this.progress = progress;
        this.showLog = showLog;
        this.isLocal = local;
    }
}
