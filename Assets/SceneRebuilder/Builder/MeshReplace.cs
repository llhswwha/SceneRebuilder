using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshReplace : MonoBehaviour
{

    //public GameObject prefab;

    //public List<GameObject> targetList=new List<GameObject>();

    //public List<GameObject> targetListNew = new List<GameObject>();

    public TransfromReplaceSetting transfromReplaceSetting;

    public List<MeshReplaceItem> Items = new List<MeshReplaceItem>();

    public bool isDestoryOriginal = false;

    public bool isHiddenOriginal = false;

    // Start is called before the first frame update
    void Start()
    {
        //Replace();
    }

    [ContextMenu("Replace")]
    public void Replace()
    {
        ClearNewGos();
        foreach (var item in Items)
        {
            item.Replace(isDestoryOriginal, isHiddenOriginal, transfromReplaceSetting);
        }
        SelectNewGos();
    }

    [ContextMenu("SelectNewGos")]
    public void SelectNewGos()
    {
        List<GameObject> newGos = new List<GameObject>();
        foreach (var item in Items)
        {
            newGos.AddRange(item.targetListNew);
        }
        EditorHelper.SelectObjects(newGos);
    }

    [ContextMenu("SelectPrefabs")]
    public void SelectPrefabs()
    {
        List<GameObject> newGos = new List<GameObject>();
        foreach (var item in Items)
        {
            newGos.Add(item.prefab);
        }
        EditorHelper.SelectObjects(newGos);
    }

    [ContextMenu("SelectTargets")]
    public void SelectTargets()
    {
        List<GameObject> newGos = new List<GameObject>();
        foreach (var item in Items)
        {
            newGos.AddRange(item.targetList);
        }
        EditorHelper.SelectObjects(newGos);
    }

    [ContextMenu("ClearNewGos")]
    public void ClearNewGos()
    {
        foreach (var item in Items)
        {
            item.ClearNewGos();
        }
    }

    [ContextMenu("ApplyNewGos")]
    public void ApplyNewGos()
    {
        foreach (var item in Items)
        {
            item.ApplyNewGos();
        }
    }
}
