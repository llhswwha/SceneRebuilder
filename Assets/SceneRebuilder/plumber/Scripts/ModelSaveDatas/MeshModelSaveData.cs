using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class MeshModelSaveData
{
    [XmlAttribute]
    public string Name;

    [XmlAttribute]
    public string Id;

    [XmlAttribute]
    public string PId;

    //public List<string> Path;

    //public TransformInfo Transform;

    public Vector3 pos;

    public Vector3 rotation;

    public Vector3 scale;

    public List<PipeWeldSaveData> PipeWelds = new List<PipeWeldSaveData>();

    [XmlAttribute]
    public string prefabId;

    [XmlAttribute]
    public bool isPrefab;

    public virtual bool IsSuccess()
    {
        return true;
    }

    public void GetTransformInfo(Transform t)
    {
        pos = t.position;
        rotation = t.rotation.eulerAngles;
        scale = t.localScale;
    }

    public void SetTransformInfo(Transform transform)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(rotation);
        transform.localScale = scale;
    }

    public override string ToString()
    {
        return $"Name:{Name} Id:{Id} PId:{PId} prefabId:{prefabId} isPrefab:{isPrefab}";
    }

    public MeshModelSaveData()
    {

    }

    public MeshModelSaveData(GameObject go)
    {
        Init(go);
    }

    public void Init(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError($"MeshModelSaveData.Init go==null");
        }
        Name = go.name;
        Id = RendererId.GetId(go);
        //data.Path = GetPath();
        PId = RendererId.GetId(go.transform.parent);
        //data.Transform = new TransformInfo(this.transform);
        GetTransformInfo(go.transform);

        
    }

    public void InitPrefabInfo(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError($"MeshModelSaveData.InitPrefabInfo go==null Name:{this.Name}");
            return;
        }
        MeshPrefabInstance ins = go.GetComponent<MeshPrefabInstance>();
        if (ins == null)
        {
            Debug.LogError($"MeshModelSaveData.InitPrefabInfo ins==null go:{go}");
        }
        else
        {
            if (ins.PrefabGo==null)
            {
                Debug.LogError($"MeshModelSaveData.InitPrefabInfo ins.PrefabGo==null go:{go}");
                return;
            }
            prefabId = RendererId.GetId(ins.PrefabGo);
            if (string.IsNullOrEmpty(prefabId))
            {
                Debug.LogError($"MeshModelSaveData.InitPrefabInfo string.IsNullOrEmpty(prefabId) go:{go}");
                return;
            }
            isPrefab = ins.IsPrefab;
        }
    }
}
