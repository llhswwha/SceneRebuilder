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
        return $"Name:{Name} Id:{Id} PId:{PId}";
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
        Name = go.name;
        Id = RendererId.GetId(go);
        //data.Path = GetPath();
        PId = RendererId.GetId(go.transform.parent);
        //data.Transform = new TransformInfo(this.transform);
        GetTransformInfo(go.transform);
    }
}
