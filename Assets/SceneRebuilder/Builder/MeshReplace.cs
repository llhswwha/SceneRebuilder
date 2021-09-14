using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshReplace : SingletonBehaviour<MeshReplace>
{

    //public GameObject prefab;

    //public List<GameObject> targetList=new List<GameObject>();

    //public List<GameObject> targetListNew = new List<GameObject>();

    public TransfromAlignSetting transfromReplaceSetting;

    public List<MeshReplaceItem> Items = new List<MeshReplaceItem>();

    public bool isDestoryOriginal = false;

    public bool isHiddenOriginal = false;

    // Start is called before the first frame update
    void Start()
    {
        //Replace();
    }

#if UNITY_EDITOR
    [ContextMenu("Replace")]
    public void Replace()
    {
        DateTime start = DateTime.Now;
        int count = 0;
        ClearNewGos();
        for (int i = 0; i < Items.Count; i++)
        {
            MeshReplaceItem item = Items[i];
            ProgressArg p1 = new ProgressArg("Replace", i, Items.Count, item);
            if (ProgressBarHelper.DisplayCancelableProgressBar(p1))
            {
                break;
            }
            bool r=item.Replace(isDestoryOriginal, isHiddenOriginal, transfromReplaceSetting,p1);
            if (r == false)
            {
                break;
            }
            count += item.Count;
        }
        SelectNewGos();
        ProgressBarHelper.ClearProgressBar();
        Debug.Log($"Replace count:{count} time:{(DateTime.Now-start).ToString()}");
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
#endif

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
