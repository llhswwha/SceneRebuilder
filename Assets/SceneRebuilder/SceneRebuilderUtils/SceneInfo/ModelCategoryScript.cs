using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCategoryScript : MonoBehaviour
{
    public ModelCategory Cate;

    internal void AddObject(Transform item)
    {
        item.SetParent(this.transform);
    }

    public bool HasKey(string[] keys)
    {
        foreach (var key in keys)
        {
            if (Cate.Name.Contains(key))
            {
                return true;
            }
        }
        return false;
    }
}
