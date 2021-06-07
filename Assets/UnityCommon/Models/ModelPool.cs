using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 保存已经使用过的模型列表，模型在使用前要进行简单的处理
/// </summary>
public class ModelPool
{
    public static ModelPool Instance = new ModelPool();

    /// <summary>
    /// 名称
    /// </summary>
    private string _name = "Models";

    /// <summary>
    /// 位置
    /// </summary>
    private Vector3 _position = new Vector3(1000f, 1000f, 1000f);

    public Vector3 Position { get { return _position;} }

    private ModelPool()
    {
        
    }

    public ModelPool(string name,Vector3 pos)
    {
        _name = name;
        _position = pos;
    }

    private Transform _modelsRoot;

    public Transform Root
    {
        get
        {
            if (_modelsRoot == null)
            {
                _modelsRoot = new GameObject(_name).transform;
            }
            return _modelsRoot;
        }
    }

    private readonly Dictionary<GameObject, GameObject> _models = new Dictionary<GameObject, GameObject>();

    public GameObject GetModel(GameObject model)
    {
        if (model == null) return null;
        if (!_models.ContainsKey(model))
        {
            GameObject obj = Object.Instantiate(model) as GameObject;
            if (obj == null) return null;
            obj.AddCollider(); //在这里就把碰撞体加上，不用Instantiate后再加一遍
            AddModel(obj);
            _models.Add(model, obj);
        }
        return _models[model];
    }

    public void AddPrefabs()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag(Tags.Prefab);
        AddModels(prefabs);
    }

    public void AddModels(GameObject[] prefabs)
    {
        foreach (GameObject prefab in prefabs)
        {
            AddModel(prefab);
        }
    }

    public void AddModel(GameObject obj)
    {
        obj.transform.parent = Root;
        obj.transform.localPosition = Position; //放的远远的
    }

    public void Clear()
    {
        _models.Clear();
    }
}
