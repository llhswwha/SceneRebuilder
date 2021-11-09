using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class ModelLevelScript : MonoBehaviour
{
    [System.NonSerialized]
    public ModelLevelInfo Info;

    public Vector3 OrigPos;

    public List<ModelCategoryScript> Categories = new List<ModelCategoryScript>();
    

    // Start is called before the first frame update
    void Start()
    {
        OrigPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPos()
    {
        transform.position = OrigPos;
    }

    internal void SetCategoriesVisible(bool isVisible,params string[] categoryKey)
    {
        foreach (var Cate in Categories)
        {
            if (Cate.HasKey(categoryKey))
            {
                Cate.gameObject.SetActive(isVisible);
            }
        }
    }

    internal bool Combine(ModelLevelScript other)
    {
        print("Combine:"+this+"|"+other);
        if (other != null)
        {
            NodeInfoScript[] children = other.GetComponentsInChildren<NodeInfoScript>();
            print("Combine children:" + children.Length);
            foreach (var item in children)
            {
                this.AddObject(item.transform);
            }
            return true;
        }
        return false;
    }

    internal void AddObject(Transform item)
    {
        ModelCategoryScript cate = GetCategory(item);
        if (cate != null)
        {
            cate.AddObject(item);
        }
        else
        {
            item.SetParent(this.transform);
        }
    }

    internal void ShowAllFrame()
    {
        foreach (var item in Categories)
        {
            item.gameObject.SetActive(true);
        }
    }

    public ModelCategoryScript GetCategory(Transform item)
    {
        string typeName = NodeInfo.GetTypeName(item.name);
        var catogeryInfo = CategoryInfoList.Instance.GetCategory(typeName);
        if (catogeryInfo != null)
        {
            foreach (var cate in Categories)
            {
                if (cate.Cate.Info == catogeryInfo)
                {
                    //cate.AddObject(item);
                    return cate;
                }
            }

            GameObject obj = new GameObject();
            obj.name = catogeryInfo.ToString();
            obj.transform.SetParent(this.transform);

            var cateNew=obj.AddComponent<ModelCategoryScript>();
            cateNew.Cate = new ModelCategory(catogeryInfo);
            Categories.Add(cateNew);

            return cateNew;
        }
        print("ModelLevelScript.GetCategory:"+typeName+"|"+catogeryInfo);
        return null;
    }
}
