using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataComponent : ScriptableObject
{
    public string Id;

    public string Name;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("DataComponent.Start");
    }
}
