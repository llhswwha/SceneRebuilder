using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshReplace : MonoBehaviour
{
    public GameObject prefab;

    public GameObject target;

    public bool isDestoryOriginal = true;

    // Start is called before the first frame update
    void Start()
    {
        Replace();
    }

    [ContextMenu("Replace")]
    public void Replace()
    {
        //MeshHelper.ReplaceByPrefab(target, prefab);
        StartCoroutine(MeshHelper.ReplaceByPrefabEx(target, prefab,"", "",isDestoryOriginal));
    }
}
