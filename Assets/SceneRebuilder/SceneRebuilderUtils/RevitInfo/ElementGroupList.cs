
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RevitTools.Infos
{
    [Serializable]
    public class ElementGroupList:List<ElementGroup>
    {
        public int VCount = 0;
        private Dictionary<string, ElementGroup> dict = new Dictionary<string, ElementGroup>();
        private ElementInfoList _list;
        public ElementGroupType Type;

        private Dictionary<string, ElementInfo> elementIndex_name = new Dictionary<string, ElementInfo>();
        private Dictionary<string, ElementInfo> elementIndex_id = new Dictionary<string, ElementInfo>();

        public List<ElementInfo> GetElementInfos()
        {
            List<ElementInfo> result = new List<ElementInfo>();
            foreach (var item in this)
            {
                result.AddRange(item.List);
            }
            return result;
        }

        int repeatCount = 0;

        public void InitElementIndex()
        {
            if (elementIndex_name.Count == 0)
            {
                var infoList = GetElementInfos();
                foreach (var item in infoList)
                {
                    //string nodeName = string.Format("{0} {1} [{2}]", item.FamilyName, item.Name, item.Id);
                    elementIndex_name.Add(item.GetNodeName() + "", item);
                    //"Id":594564,"Name":"120mm 深度","CategoryName":"梯段","FamilyName":"整体梯段","ClassName":"StairsRun"
                    //"Id":715353,"Name":"600 x 750mm","CategoryName":"结构柱","FamilyName":"混凝土 - 矩形 - 柱","ClassName":"FamilyInstance"
                    //混凝土_-_矩形_-_柱_600_x_750mm_[715353]
                    string id = item.Id + "";
                    if (elementIndex_id.ContainsKey(id))
                    {
                        repeatCount++;
                        Debug.LogError(string.Format("重复Id[{0}] id:{1} name:{2}",repeatCount,id,item.GetNodeName()));
                    }
                    else
                    {
                        elementIndex_id.Add(id, item);
                    }
                }
            }
        }

        public ElementInfo GetElementInfo(string id,string name)
        {
            InitElementIndex();
            if (elementIndex_name.ContainsKey(name))
            {
                return elementIndex_name[name];
            }
            else
            {
                if (elementIndex_name.ContainsKey(id))
                {
                    return elementIndex_name[id];
                }
                else
                {
                    if (elementIndex_id.ContainsKey(id))
                    {
                        return elementIndex_id[id];
                    }
                    else
                    {
                        Debug.LogError("GetElementInfo null:" + id + "|" + name);
                        return null;
                    }
                }
            }
        }

        public ElementGroupList()
        {

        }

        public ElementGroupList(ElementInfoList list, ElementGroupType type)
        {
            this._list = list;
            this.Type = type;

            foreach (var item in list)
            {
                AddElementInfo(type, item);
            }

            this.Sort();
 
        }

        private void AddElementInfo(ElementGroupType type, ElementInfo item)
        {
            string key = item.Name;
            if (type == ElementGroupType.Type)
            {
                key = item.CategoryName + "|" + item.FamilyName + "|" + item.Name;
            }
            else if (type == ElementGroupType.Catagory)
            {
                key = item.CategoryName;
            }
            else if (type == ElementGroupType.Family)
            {
                key = item.FamilyName;
            }

            if (!dict.ContainsKey(key))
            {
                ElementGroup newGroup = new ElementGroup(key);
                dict[key] = newGroup;
                this.Add(newGroup);

                //newGroup.Parent = item.FamilyName;
                //newGroup.Family = item.FamilyName;
                //newGroup.Category = item.CategoryName;
            }
            ElementGroup group = dict[key];
            group.Add(item);
        }

        //internal void GetMeshInfo()
        //{
        //    VCount = 0;
        //    ElementGroupList list = this;
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        ElementGroup item = list[i];
        //        Log.Progress("ElementGroupList.GetMeshInfo", item.ToString(), (i + 1), Count);
        //        item.GetMeshInfo();
        //        VCount += item.VCount;
        //    }
        //    this.Sort();
        //}

        public void SortByName()
        {
            this.Sort(new Comparison<ElementGroup>((a, b) =>
            {
                return a.Name.CompareTo(b.Name);
            }));
        }

        public void SortByCount()
        {
            this.Sort(new Comparison<ElementGroup>((a, b) =>
            {
                return b.Count.CompareTo(a.Count);
            }));
        }

        public void SortByVCount()
        {
            this.Sort(new Comparison<ElementGroup>((a, b) =>
            {
                return b.VCount.CompareTo(a.VCount);
            }));
        }

        public string GetNameList()
        {
            string nameList = "";
            foreach (var item in dict.Keys)
            {
                nameList += item + "\n";
            }
            return nameList;
        }

        public string PrintString()
        {
            string txt = "";
            foreach (var item in this)
            {
                txt += item + "\n";
            }
            return txt;
        }

        public void AddList(ElementGroupList elementGroups2)
        {
            if (elementGroups2 != null)
            {
                var list = elementGroups2.GetElementInfos();
                foreach (var item in list)
                {
                    AddElementInfo(this.Type, item);
                }
            }

            elementIndex_name.Clear();
            elementIndex_id.Clear();
            InitElementIndex();

            this.Sort();
        }
    }
}
