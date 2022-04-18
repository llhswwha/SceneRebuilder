using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInfoRoot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameObject NewGo(string name,Transform parent)
    {
        GameObject planInfoRoot = new GameObject(name);
        planInfoRoot.AddComponent<DebugInfoRoot>();
        planInfoRoot.transform.SetParent(parent);
        planInfoRoot.transform.localPosition = Vector3.zero;
        return planInfoRoot;
    }
}
