using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NavisPlugins.Infos
{
    public class NavisFileInfo
    {
        [XmlAttribute]
        public string FileName = "";

        //[XmlElement]

        public List<ModelItemInfo> Models = new List<ModelItemInfo>();

        public List<ModelItemInfo> GetAllItems()
        {
            List<ModelItemInfo> list = new List<ModelItemInfo>();
            if (Models != null)
                foreach (var child in Models)
                {
                    var subList = child.GetAllItems();
                    //foreach (var item in subList)
                    //{
                    //    if (list.Contains(item))
                    //    {

                    //    }
                    //    else
                    //    {
                    //        list.Add(item);
                    //    }

                    //}
                    list.AddRange(subList);
                }
            return list;
        }

        public List<ModelItemInfo> GetAllModelInfos()
        {
            List<ModelItemInfo> models = new List<ModelItemInfo>();
            if (this.Models != null)
            {
                foreach (var item in this.Models)
                {
                    models.AddRange(GetChildItemInfo(item, item));
                }
            }
            return models;
        }

        private static List<ModelItemInfo> GetChildItemInfo(ModelItemInfo navisT, ModelItemInfo root)
        {
            List<ModelItemInfo> models = new List<ModelItemInfo>();
            if (navisT.Children != null)
            {
                models.AddRange(navisT.Children);
                foreach (var item in navisT.Children)
                {
                    item.SetParent(navisT, root);
                    models.AddRange(GetChildItemInfo(item, root));
                }
            }
            return models;
        }

        public void SetPropertiesExist(bool isSaveProperties)
        {
            foreach (var model in Models)
            {
                var items = model.GetAllItems();
                foreach (var item in items)
                {
                    if (isSaveProperties == false)
                    {
                        item.Categories = null;
                    }
                    if (item.Children != null && item.Children.Count == 0)
                    {
                        item.Children = null;
                    }
                }
            }
        }
    }
}
