#if REVIT
using Autodesk.Revit.DB;
#endif 

using System;

namespace RevitTools.Infos
{
    public class ElementInfo:IComparable<ElementInfo>
    {
#if REVIT
        private Element _ele;
#endif

        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }

        public string FamilyName { get; set; }
        public string ClassName { get; set; }
        public string BaseClass { get; set; }

        public int LevelId { get; set; }

        public string LevelName { get; set; }

        public double LevelHeight { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }

        public int SystemId { get; set; }

        public string SystemName { get; set; }

        public LocationInfo Location { get; set; }

        //public MeshInfo Mesh { get; set; }


        private string compareId = "";

        public void SetCompareId()
        {
            compareId = BaseClass + ClassName + CategoryName + LevelId + Name + SystemName;
        }


        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Id, Name, FamilyName, CategoryName);
        }

        private string _nodeName = null;

        public string GetNodeName()
        {
            if (_nodeName == null)
            {
                _nodeName=  string.Format("{0} {1} [{2}]", FamilyName, Name, Id);
                _nodeName = _nodeName.Replace(" ", "_");
                _nodeName = _nodeName.Replace("*", "_x_");
                _nodeName = _nodeName.Replace("/", "__");
                _nodeName = _nodeName.Replace("\\", "__");
                //obj.name = substitutestring obj.name " " "_"
                //obj.name = substitutestring obj.name "*" "_x_"-- * 不能作为文件名
                //obj.name = substitutestring obj.name "/" "__"-- / 不能作为文件名
                //obj.name = substitutestring obj.name "\\" "__"-- / 不能作为文件名
            }
            //"Id":715353,"Name":"600 x 750mm","CategoryName":"结构柱","FamilyName":"混凝土 - 矩形 - 柱","ClassName":"FamilyInstance"
            //混凝土_-_矩形_-_柱_600_x_750mm_[715353]
            return _nodeName;
        }

        //public ElementInfo(Element ele)
        //{
        //    this._ele = ele;
        //    this.Id = ele.Id.IntegerValue;
        //    this.Name = ele.Name;
        //    var level= ele.Document.GetElement(ele.LevelId);
        //    if (level != null)
        //    {
        //        this.LevelId = ele.LevelId.IntegerValue;
        //        this.LevelName = level.Name;
        //    }
            
        //    if(ele is MEPCurve)
        //    {
        //        var curve = ele as MEPCurve;
        //        this.LevelId = curve.ReferenceLevel.Id.IntegerValue;
        //        this.LevelName = curve.ReferenceLevel.Name;

        //        this.SystemName = curve.MEPSystem.Name;
        //        this.SystemId = curve.MEPSystem.Id.IntegerValue;
        //        var systemType= ele.Document.GetElement(curve.MEPSystem.GetTypeId());
        //        if (systemType != null)
        //        {
        //            this.SystemName = systemType.Name;
        //            this.SystemId = systemType.Id.IntegerValue;
        //        }
                
        //    }

        //    if(ele.Category != null)
        //    {
        //        this.CategoryName = ele.Category.Name;
        //    }
            
        //    this.ClassName = ele.GetType().Name;
        //    this.BaseClass = ele.GetType().BaseType.Name;

        //    var family=ele.Document.GetElement(ele.GetTypeId());
        //    if (family is FamilySymbol)
        //    {
        //        this.FamilyName = (family as FamilySymbol).FamilyName;
        //    }
        //    else if (family is ElementType)
        //    {
        //        ElementType elementType = family as ElementType;
        //        this.FamilyName = elementType.FamilyName;
        //    }
        //    else
        //    {
        //        this.FamilyName = family + "";
        //    }

        //    //compareId = LevelId+CategoryName + FamilyName + Name;
        //    compareId = BaseClass + ClassName + CategoryName + LevelId+ Name+SystemName;

        //    Location = new LocationInfo(ele.Location);

        //    //GetMeshInfo();

        //    if (ele is FamilyInstance)
        //    {
        //        var instance = ele as FamilyInstance;

        //        //if(this.FamilyName == "焊缝")
        //        //{
        //        //    var mepModel = instance.MEPModel;
        //        //    if (mepModel is MechanicalFitting)
        //        //    {
        //        //        var mf = mepModel as MechanicalFitting;
        //        //        var cm = mf.ConnectorManager;
        //        //        var cs = cm.Connectors;
        //        //        foreach (Connector ct in cs)
        //        //        {

        //        //        }
        //        //    }
                    
        //        //}

        //        var parameter = ele.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM);
        //        if (parameter != null)
        //        {
        //            this.SystemName = parameter.AsValueString();
        //            this.SystemId = parameter.AsElementId().IntegerValue;
        //        }
                

        //    }
        //}
        
        //public void GetMeshInfo()
        //{
        //    Mesh = MeshInfoBuffer.GetMeshInfo(_ele);
        //}


        //internal Element GetElement()
        //{
        //    return _ele;
        //}

        public int CompareTo(ElementInfo other)
        {
            return this.compareId.CompareTo(other.compareId);
        }

    }
}
