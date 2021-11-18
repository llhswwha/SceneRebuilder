using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace NavisPlugins.Infos
{
    [Serializable]
    [XmlType("ItemInfo")]
    public class ModelItemInfo:IComparable<ModelItemInfo>
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string UId { get; set; }

        [XmlAttribute]
        public float X { get; set; }

        [XmlAttribute]
        public float Y { get; set; }

        [XmlAttribute]
        public float Z { get; set; }

        public Vector3 GetPositon()
        {
            return new Vector3(X, Z, Y);
        }

        public bool IsZero()
        {
            return X == 0 && Y == 0 && Z == 0;
        }

        // [XmlAttribute]
        // public string ClassName { get; set; }

        // [XmlAttribute]
        // public string ClassDisplayName { get; set; }

        // [XmlAttribute]
        // public string SP3DName { get; set; }

 
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public bool Drawable { get; set; }

        [XmlAttribute]
        public int Visible { get; set; }

        [XmlAttribute]
        public int AreaId { get; set; }

        [XmlAttribute]
        public string AreaName { get; set; }

        [XmlAttribute]
        public string RenderId { get; set; }

        [XmlAttribute]
        public string RenderName { get; set; }

        public void SetVisible(int v)
        {
            Visible = v;
            if (_parent != null)
            {
                _parent.SetVisible(v);
            }
        }

        [XmlIgnore]
        public GameObject AreaGo
        {
            get
            {
                if (AreaGos == null || AreaGos.Count == 0) return null;
                return AreaGos[0];
            }
        }

        [XmlIgnore]
        public List<GameObject> AreaGos = new List<GameObject>();

        public BIMDevDetail Detail { get; set; }

        public List<PropertyCategoryInfo> Categories = new List<PropertyCategoryInfo>();

        [XmlIgnore]
        public object Tag { get; set; }

        //[XmlElement]
        public List<ModelItemInfo> Children = new List<ModelItemInfo>();

        // [XmlAttribute]
        // public string ModelName { get; set; }

        //[XmlIgnore]
        //public ModelItem Model;

        [XmlIgnore]
        public List<string> Models=new List<string>();

        [XmlIgnore]
        public GameObject CurrentModel=null;

         private ModelItemInfo _parent;

        public void SetParent(ModelItemInfo p, ModelItemInfo root)
        {
            _parent = p;
            this.root = root;
            if (root == null)
            {
                Debug.LogError("SetParent root == null");
            }
        }

        private string _path = null;

        public string GetPath()
        {
            if (!string.IsNullOrEmpty(_path))
            {
                return _path;
            }
            if(string.IsNullOrEmpty(_path))
            {
                if (_parent != null)
                {
                    _path = _parent.GetPath() + "\\"+this.Name;
                }
                else
                {
                    _path= this.Name;
                }
            }
            return _path;
        }

        public string GetAreaName()
        {
            if (!string.IsNullOrEmpty(AreaName))
            {
                return AreaName;
            }
            if (root != null)
            {
                return root.AreaName;
            }
            return AreaName;
        }

        private ModelItemInfo root;

        //public ModelItemInfo GetRoot()
        //{
        //    if (root != null)
        //    {
        //        GetPath
        //    }
        //}

        public ModelItemInfo GetParent(){
             return _parent;
         }

        internal List<ModelItemInfo> GetAllItems()
        {
            List<ModelItemInfo> list = new List<ModelItemInfo>();
            list.Add(this);
            if(Children!=null)
                foreach (var child in Children)
                {
                    var subList = child.GetAllItems();
                    list.AddRange(subList);
                }
            return list;
        }

        internal List<ModelItemInfo> GetAllChildren()
        {
            List<ModelItemInfo> list = new List<ModelItemInfo>();
            //list.Add(this);
            if (Children != null)
                foreach (var child in Children)
                {
                    var subList = child.GetAllItems();
                    list.AddRange(subList);
                }
            return list;
        }

        public int CompareTo(ModelItemInfo other)
        {
            return this.Name.CompareTo(other.Name);
        }

        public string GetNodeName()
        {
            //return ModelName;
            //return ModelName + "|" + Children.Count;
            //GetXYZ();

            int count = 0;
            if (Children != null)
            {
                count = Children.Count;
            }
            return string.Format("[{8}][{9}]id:{0}|name:{1}|{2}|({3},{4},{5})|[uid:{6}][rid:{7}]", Id, Name, count, X, Y, Z, UId, RenderId,Drawable,Type);
        }


        public override string ToString()
        {
            return GetNodeName();

            //return Name;
        }
    }
}
