using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[XmlType("SceneAssetInfo")]
public class SceneAssetInfo
{
    [XmlAttribute]
    public string ObjectName { get; set; }

    [XmlAttribute]
    public string SceneName { get; set; }

    public string AssetName
    {
        get
        {
            return "buildings_" + SceneName;
        }
    }

    [XmlAttribute]
    public string NodeName { get; set; }

    [XmlAttribute]
    public string ParentName { get; set; }

    public Vector3 Position { get; set; }

    public Vector3 Center { get; set; }

    public Vector3 Size { get; set; }

    [XmlAttribute]
    public float LoadDistance { get; set; }

    [XmlAttribute]
    public float UnloadDistance { get; set; }

    [XmlAttribute]
    public string ModelPath { get; set; }

    public SceneAssetInfo()
    {

    }

    public SceneAssetInfo(BuildingController building)
    {
        var obj = building.gameObject;
        ObjectName = obj.name;
        SceneName = obj.name;
        NodeName = building.NodeName;
        //AssetName = "scenes_" + SceneName;
        if (obj.transform.parent)
        {
            ParentName = obj.transform.parent.name;
        }
    }

    public SceneAssetInfo(BuildingAssetInfo building)
    {
        var obj = building.gameObject;
        ObjectName = obj.name;
        SceneName = obj.name;
        //NodeName = building.NodeName;
        //AssetName = "scenes_" + SceneName;
        if (obj.transform.parent)
        {
            ParentName = obj.transform.parent.name;
        }
    }
}

[XmlRoot("SceneAssetInfoList")]
public class SceneAssetInfoList : List<SceneAssetInfo>
{

}
