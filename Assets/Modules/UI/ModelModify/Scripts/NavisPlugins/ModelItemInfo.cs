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

        public List<ModelItemInfo> GetChildrenModels()
        {
            return GetChildItemInfo(this);
        }

        public List<ModelItemInfo> GetChildItemInfo(ModelItemInfo root)
        {
            List<ModelItemInfo> models = new List<ModelItemInfo>();
            if (this.Children != null)
            {
                models.AddRange(this.Children);
                foreach (var item in this.Children)
                {
                    item.SetParent(this, root);
                    models.AddRange(item.GetChildItemInfo(root));
                }
            }
            return models;
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
            //return string.Format("[{8}][{9}]id:{0}|name:{1}|{2}|({3},{4},{5})|[uid:{6}][rid:{7}]", Id, Name, count, X, Y, Z, UId, RenderId,Drawable,Type);
            return $"[{Drawable}][{Type}]id:{Id}|name:{Name}|{count}|({X},{Y},{Z})|[uid:{UId}][rid:{RenderId}]";
        }


        public override string ToString()
        {
            return GetNodeName();

            //return Name;
        }

        public string ShowDistance(Transform transform)
        {
            var p1 = this.GetPositon();
            var p2 = transform.position;
            var dis = Vector3.Distance(p1, p2);
            bool isSameName = this.IsSameName(transform);
            //Debug.Log($"ShowDistance distance:{dis} \tmodel:{this.Name}({p1}) tansform:{transform.name}({p2})");
            //return $"ShowDistance distance:{dis} isSameName:{isSameName} \tmodel:{this.Name}{p1}{this.UId} transform:{transform.name}{p2}";
            return $"dis:{dis} same:{isSameName} \tmodel:{this.Name}{p1}{this.UId} transform:{transform.name}{p2}";
        }

        public float GetDistance(Transform transform)
        {
            var p1 = this.GetPositon();
            var p2 = transform.position;
            var dis = Vector3.Distance(p1, p2);
            return dis;
        }

        public Transform FindClosedTransform(List<Transform> ts)
        {
            return FindClosedTransform(ts, this.GetPositon());
        }

        public static Transform FindClosedTransform(List<Transform> ts, Vector3 pos)
        {
            float minDis = float.MaxValue;
            Transform minModel = null;
            foreach (var t in ts)
            {
                float dis = Vector3.Distance(t.position, pos);
                if (minDis > dis)
                {
                    minDis = dis;
                    minModel = t;
                }
            }
            return minModel;
        }

        private string compareName11 = "";
        private string compareName12 = "";
        private string compareName13 = "";

        private string compareName21 = "";
        private string compareName22 = "";
        private string compareName23 = "";

        private string compareName31 = "";
        private string compareName32 = "";
        private string compareName33 = "";

        private string compareName41 = "";
        private string compareName42 = "";
        private string compareName43 = "";

        //private bool IsSameNameSelf()
        //{

        //}

        public bool IsSameName(Transform transform)
        {
            if (string.IsNullOrEmpty(compareName11))
            {
                compareName11 = this.Name.Replace(" ", "_");
                compareName12 = compareName11 + " ";
                compareName13 = compareName11 + "_";

                compareName21 = compareName11.Replace("*", "_x_");
                compareName22 = compareName21 + " ";
                compareName23 = compareName21 + "_";

                if (!string.IsNullOrEmpty(this.Description))
                {
                    compareName31 = this.Description.Replace(" ", "_");
                    compareName32 = compareName31 + " ";
                    compareName33 = compareName31 + "_";

                    compareName41 = compareName31.Replace("*", "_x_");
                    compareName42 = compareName41 + " ";
                    compareName43 = compareName41 + "_";
                }
            }

            if (transform.name == this.Name )
            {
                return true;
            }
            else if(transform.name == compareName11 || transform.name.StartsWith(compareName12) || transform.name.StartsWith(compareName13))
            {
                return true;
            }
            else if (transform.name == compareName21 || transform.name.StartsWith(compareName22) || transform.name.StartsWith(compareName23))
            {
                return true;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.Description))
                {
                    if (transform.name == this.Description)
                    {
                        return true;
                    }
                    else if (transform.name == compareName31 || transform.name.StartsWith(compareName32) || transform.name.StartsWith(compareName33))
                    {
                        return true;
                    }
                    else if (transform.name == compareName41 || transform.name.StartsWith(compareName42) || transform.name.StartsWith(compareName43))
                    {
                        return true;
                    }
                    else
                    {
                        if (_parent.Children.Count == 1)
                        {
                            return _parent.IsSameName(transform);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (_parent.Children.Count == 1)
                    {
                        return _parent.IsSameName(transform);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }

    public class ModelItemInfoListEx
    {
        [NonSerialized]
        public List<ModelItemInfo> allModels = new List<ModelItemInfo>();

        [NonSerialized]
        public List<ModelItemInfo> allModels_uid = new List<ModelItemInfo>();

        [NonSerialized]
        public List<ModelItemInfo> allModels_noUid = new List<ModelItemInfo>();

        [NonSerialized]
        public List<ModelItemInfo> allModels_drawable_nozero = new List<ModelItemInfo>();

        [NonSerialized]
        public List<ModelItemInfo> allModels_drawable_zero = new List<ModelItemInfo>();

        [NonSerialized]
        public List<ModelItemInfo> allModels_noDrawable_nozero = new List<ModelItemInfo>();

        [NonSerialized]
        public List<ModelItemInfo> allModels_noDrawable_zero = new List<ModelItemInfo>();

        public ModelItemInfoListEx()
        {

        }

        public ModelItemInfoListEx(List<ModelItemInfo> items)
        {
            SetList(items);
        }

        public void SetList(List<ModelItemInfo> items)
        {
            allModels.Clear();
            allModels.AddRange(items);
            GetModelLists();
        }

        private void GetModelLists()
        {
            //allModels = ModelRoot.GetAllItems();

            allModels_drawable_nozero.Clear();
            allModels_drawable_zero.Clear();
            allModels_noDrawable_nozero.Clear();
            allModels_noDrawable_zero.Clear();

            //allModels.Clear();
            allModels_uid.Clear();
            allModels_noUid.Clear();

            for (int i = 0; i < allModels.Count; i++)
            {
                ModelItemInfo child = allModels[i];
                //if (child.IsZero())
                //{
                //    if (child.Drawable == false)
                //    {
                //        allModels_zero.Add(child);
                //    }
                //    else
                //    {
                //        allModels_drawable.Add(child);
                //    }
                //}
                //else
                //{
                //    if (child.Drawable == false)
                //    {
                //        allModels_noDrawable.Add(child);
                //    }
                //    else
                //    {

                //    }
                //}

                if (child.Drawable == true)
                {
                    //allModels_drawable.Add(child);
                    if (child.IsZero())
                    {
                        allModels_drawable_zero.Add(child);
                    }
                    else
                    {
                        allModels_drawable_nozero.Add(child);
                    }
                }
                else
                {
                    if (child.IsZero())
                    {
                        allModels_noDrawable_zero.Add(child);
                    }
                    else
                    {
                        allModels_noDrawable_nozero.Add(child);
                    }
                }

                if (!string.IsNullOrEmpty(child.UId))
                {
                    allModels_uid.Add(child);
                }
                else
                {
                    allModels_noUid.Add(child);
                }
            }

            allModels.Sort();
            allModels_uid.Sort();
            allModels_noUid.Sort();

            allModels_drawable_zero.Sort();
            allModels_drawable_nozero.Sort();
            allModels_noDrawable_nozero.Sort();
            allModels_noDrawable_zero.Sort();
        }
    }
}
