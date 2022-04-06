using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class IdInfo
{
    [XmlAttribute]
    public string name;

    [XmlAttribute]
    public string parent;

    [XmlAttribute]
    public string Id;

    //public int insId;

    [XmlAttribute]
    public string parentId;
}

[Serializable]
public class IdInfoList
{
    public List<IdInfo> Ids = new List<IdInfo>();

    public void AddId(RendererId rId)
    {
        IdInfo id = new IdInfo();
        id.Id = rId.Id;
        id.name = rId.name;
        if(rId.transform.parent!=null)
            id.parent = rId.transform.parent.name;
        //id.insId = rId.insId;
        id.parentId = rId.parentId;
        Ids.Add(id);
    }
}
