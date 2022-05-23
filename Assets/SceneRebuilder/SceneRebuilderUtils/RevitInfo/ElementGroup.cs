using System;
using System.Collections.Generic;

namespace RevitTools.Infos
{
    public class ElementGroup:IComparable<ElementGroup>
    {
        //public string Category { get; set; }
        //public string Family { get; set; }

        //public string Parent { get; set; }

        public int Count { get; set; }
        public int VCount { get; set; }
        public string Name { get; set; }

        public List<ElementInfo> List { get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }

        private List<ElementInfo> _list = new List<ElementInfo>();

        public ElementGroup()
        {

        }

        public ElementGroup(string name)
        {
            this.Name = name;
        }

        public void Add(ElementInfo item)
        {
            _list.Add(item);
            Count++;
        }
        public List<ElementInfo> GetList()
        {
            return _list;
        }

        //public List<ElementId> GetIds()
        //{
        //    List<ElementId> ids = new List<ElementId>();
        //    foreach (var item in _list)
        //    {
        //        ids.Add(item.GetElement().Id);
        //    }
        //    return ids;
        //}

        //public void GetMeshInfo()
        //{
        //    try
        //    {
        //        VCount = 0;
        //        Log.Info("ElementGroup.GetMeshInfo", "Count:" + Count);
        //        for (int i = 0; i < _list.Count; i++)
        //        {
        //            var item = _list[i];
        //            item.GetMeshInfo();
        //            Log.Progress2("ElementGroup.GetMeshInfo", item.ToString(), (i + 1), Count);
        //            //break;
        //            VCount += item.GetMeshInfo().NumVerts;
        //            Application.DoEvents();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Info("ElementGroup.GetMeshInfo", ex.ToString());
        //    }

        //}

        public int CompareTo(ElementGroup other)
        {
            int b1= other.VCount.CompareTo(this.VCount);
            if (b1 == 0)
            {
                b1 = other.Count.CompareTo(this.Count);
            }
            if (b1 == 0)
            {
                b1 = this.Name.CompareTo(other.Name);
            }
            return b1;
        }

        public void Remove(ElementInfo elementInfo)
        {
            _list.Remove(elementInfo);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}",Name,Count,VCount);
        }
    }
}
