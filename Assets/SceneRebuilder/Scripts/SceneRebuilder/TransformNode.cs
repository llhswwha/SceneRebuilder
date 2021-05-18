using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformNode : MonoBehaviour
{
    public TransformData Data;
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Init")]
    public void Init()
    {
        Data = new TransformData(this.transform);
    }


}
