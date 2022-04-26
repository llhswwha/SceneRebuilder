using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class IdInfo
{

    //[XmlAttribute]
    //public string Path;

    [XmlAttribute]
    public string name;

    [XmlIgnore]
    public string parent;

    [XmlAttribute]
    public string Id;

    //public int insId;

    [XmlIgnore]
    public string parentId;

    [XmlAttribute]
    public float PosX;

    [XmlAttribute]
    public float PosY;

    [XmlAttribute]
    public float PosZ;

    [XmlAttribute]
    public float CenterX;

    [XmlAttribute]
    public float CenterY;

    [XmlAttribute]
    public float CenterZ;

    [XmlAttribute]
    public bool HasMesh = false;

    [XmlIgnore]
    public Vector3 pos = Vector3.zero;

    [XmlIgnore]
    public Vector3 center = Vector3.zero;

    public Vector3 GetPosition()
    {
        if (pos == Vector3.zero)
        {
            pos = new Vector3(PosX, PosY, PosZ);
        }
        return pos;
    }

    public Vector3 GetCenter()
    {
        if (center == Vector3.zero)
        {
            center = new Vector3(CenterX, CenterY, CenterZ);
        }
        return pos;
    }

    [XmlElement]
    public List<IdInfo> Children;

    [XmlIgnore]
    public IdInfo pId;

    private string _path = "";

    public string GetPath()
    {
        if (string.IsNullOrEmpty(_path))
        {
            if (pId != null)
            {
                _path= pId.GetPath() + ">" + name;
            }
            else
            {
                _path= name;
            }
        }
        return _path;
    }

    public override string ToString()
    {
        return $"name:{name} pName:{parent} id:{Id} pId:{parentId} HasMesh:{HasMesh}";
    }

    public string GetFullString()
    {
        return $"¡¾name:{name} pName:{parent} id:{Id} pId:{parentId} HasMesh:{HasMesh} path:{GetPath()}¡¿";
    }

    public IdInfo()
    {

    }

    //public IdInfo(RendererId rId, bool isRecursion)
    //{
    //    Init(rId, isRecursion);
    //}

    private void Init(GameObject go,bool isRecursion)
    {
        RendererId rId = go.GetComponent<RendererId>();
        if (rId != null)
        {
            this.Id = rId.Id;
            this.parentId = rId.parentId;
        }
        

        name = go.name;
        name = name.Replace("_New3","");
        name = name.Replace("_New2", "");
        name = name.Replace("_New1", "");
        name = name.Replace("_New", ""); 

        if (go.transform.parent != null)
            this.parent = go.transform.parent.name;
        //id.insId = rId.insId;

        //this.Path = TransformHelper.GetPath(rId.transform.parent, "\\");

        if (isRecursion)
        {
            if (go.transform.childCount > 0)
            {
                Children = new List<IdInfo>();

                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var child = go.transform.GetChild(i);
                    IdInfo childId = new IdInfo(child.gameObject, isRecursion);
                    if (child.name.Contains("curve")) continue;
                    if (child.name.Contains("Geometry") && childId.HasMesh == false)
                    {
                        continue;
                    }
                    Children.Add(childId);
                }
            }
        }

        MeshRenderer renderer = go.GetComponent<MeshRenderer>();
        HasMesh = renderer != null;

        Vector3 pos = go.transform.position;
        PosX = pos.x;
        PosY = pos.y;
        PosZ = pos.z;

        if (renderer)
        {
            Vector3 center = MeshRendererInfo.GetCenterPos(go.gameObject);
            CenterX = center.x;
            CenterY = center.y;
            CenterZ = center.z;
        }
        
    }

    public IdInfo(GameObject go, bool isRecursion)
    {
        //RendererId rid = RendererId.GetRId(go);
        Init(go, isRecursion);
    }

    internal List<IdInfo> GetAllItems(int level)
    {
        List<IdInfo> items = new List<IdInfo>();
        if (level > 0)
        {
            items.Add(this);
        }
        if (Children != null)
        {
            foreach(var child in Children)
            {
                
                child.parent = this.name;
                child.parentId = this.Id;
                child.pId = this;
                items.AddRange(child.GetAllItems(level+1));
            }
        }
        return items;
    }
}

[Serializable]
public class IdInfoList
{
    public void SetRoot(GameObject rootGo, bool isRecursion)
    {
        Root = new IdInfo(rootGo, isRecursion);
    }

    public IdInfo Root = new IdInfo();

    //public List<IdInfo> Items = new List<IdInfo>();

    //public void AddId(RendererId rId)
    //{
    //    IdInfo id = new IdInfo(rId,false);
    //    Items.Add(id);
    //}

    //public void AddId(GameObject go)
    //{
    //    RendererId rid = go.GetComponent<RendererId>();
    //    AddId(rid);
    //}

    public List<IdInfo> GetAllItems()
    {
        return Root.GetAllItems(0);
    }

    [XmlIgnore]
    [HideInInspector]
    public List<IdInfo> notFoundList = new List<IdInfo>();

    [XmlIgnore]
    [HideInInspector]
    public List<IdInfo> foundList = new List<IdInfo>();
}
