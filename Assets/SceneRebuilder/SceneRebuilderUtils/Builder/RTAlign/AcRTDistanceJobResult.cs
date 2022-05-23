using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcRTDistanceJobResult
{
    public float min = float.MaxValue;

    public int minId = -1;

    public bool IsZero = false;

    public RTResult rt;

    //public GameObject Instance;

    public override string ToString()
    {
        return string.Format("(Dis:{0},Id:{1},RT:{2})", min, minId, rt);
    }
}
