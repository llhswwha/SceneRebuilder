using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class MeshPrefabSaveData
{
    //[XmlAttribute]
    //public string meshPath = "";

    [XmlAttribute]
    public string prefabId = "";

    public List<MeshInstanceSaveData> Instances = new List<MeshInstanceSaveData>();

    public MeshPrefabSaveData(MeshPrefabInstance instance)
    {
        //meshPath = EditorHelper.GetMeshPath(instance.PrefabGo);
        prefabId = RendererId.GetId(instance.PrefabGo);
    }

    public MeshPrefabSaveData()
    {
        
    }

    public void AddInstance(MeshPrefabInstance instance)
    {
        MeshInstanceSaveData data = new MeshInstanceSaveData();
        data.Init(instance.gameObject);
        Instances.Add(data);
    }
}

public class MeshInstanceSaveData:MeshModelSaveData
{

}


