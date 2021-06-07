using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 保存使用过的模型的索引
/// </summary>
public class ModelIndex  {

    public int Count
    {
        get { return _indexes.Count; }
    }

    public static ModelIndex Instance=new ModelIndex();

    private ModelIndex()
    {
        
    }

    private ObjectDictionary _indexes = new ObjectDictionary();

    /// <summary>
    /// 是否包含
    /// </summary>
    public bool Contains(string name)
    {
        return Get(name) != null;
    }

    //public void Add(GameObject model)
    //{
    //    Add(model, model.name);
    //}
    /// <summary>
    /// 添加到索引中（小写字母）
    /// </summary>
    public void Add(GameObject model,string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        key = key.ToLower();//小写字母
        //LogInfo.Info("ModelIndex.Add","添加模型到Index:" + key + "|" + model);
        if (_indexes.ContainsKey(key))
        {
            Debug.LogWarning("存在多个同名模型:" + key+"|"+Count);
        }
        else
        {
            _indexes.Add(key, model);
            //Debug.LogWarning("记录模型模型:" + key + "|" + Count);
        }
    }

    /// <summary>
    /// 根据名称获取模型
    /// </summary>
    public GameObject Get(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        name = name.ToLower();//小写字母
        return _indexes.Get(name);
    }

    public void Clear()
    {
        _indexes.Clear();
    }
}
