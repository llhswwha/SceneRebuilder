using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeModelBase : MonoBehaviour
{
    public float PipeRadius = 0;

    public override string ToString()
    {
        return $"radius:{PipeRadius}";
    }
}
