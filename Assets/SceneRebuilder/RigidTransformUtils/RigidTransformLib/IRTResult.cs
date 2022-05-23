using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRTResult
{
    int id { get; set; }
    float Distance { get; set; }
    //double Time { get; set; }
    void ApplyMatrix(Transform tFrom, Transform tTo);
}
