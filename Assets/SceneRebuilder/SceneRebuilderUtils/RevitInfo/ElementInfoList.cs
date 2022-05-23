//using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
//using System.Windows.Forms;

namespace RevitTools.Infos
{
    public class ElementInfoList : List<ElementInfo>
    {
        //public ElementInfoList(IEnumerable<Element> list, bool isFilter)
        //{
        //    Init(list, isFilter);
        //}

        //public void Init(IEnumerable<Element> list,bool isFilter)
        //{
        //    if (list == null) return;

        //    if (isFilter)
        //    {
        //        List<string> noModelCategoryList = new List<string>() { "中心线", "线", "日光路径", "尺寸标注",
        //        "文字", "文字注释", "参照平面", "材质资源",
        //        "立面", "光栅图像", "图纸", "轴网","标高", "材质", "RVT 链接",
        //        "电气负荷分类", "电气负荷分类参数图元", "分割规则", "风道末端", "导线绝缘层", "管道材质" , "电气需求系数定义" , "导线",
        //        "常规注释","电压","流体","体量墙","相机","详图项目","颜色填充方案","图例构件",
        //        "体量窗和天窗","体量洞口","体量屋顶","体量楼层","体量着色",
        //        "保护层类型","导线额定温度","工作平面网格","修订","项目信息","主等高线","剖面框","图框",
        //    "分析节点","分析独立基础","分析梁","分析柱","分析墙","<草图>"};

        //        List<string> noModelFamilyList = new List<string>() { "图纸", "明细表", "楼层平面", "剖图", "立面", "剖面" };
        //        List<string> noModelClassList = new List<string>() { "FamilySymbol", "ElementType", "WallType",
        //        "View","View3D","ViewPlan","ViewSection","Viewport",
        //        "CADLinkType","LoadCase","GroupType","Zone","BasePoint","ModelTextType","DuctType","HandRailType","ProjectLocation",
        //        "PipeType","PipeSegment","FloorType","RoofType","CeilingType","Group","AreaScheme","TopRailType","FasciaType","PointLoadType",
        //        "Phase","ImportInstance","PanelType","Dimension","ModelLine"};
        //        List<string> noModelBaseClassList = new List<string>() {
        //        "ElementType", "MEPCurveType", "HostObjAttributes","HostedSweepType","BaseArray" };
        //        foreach (var item in list)
        //        {
        //            var info = ElementInfoFactory.CreateElementInfo(item);
        //            string categoryName = info.CategoryName;
        //            string familyName = info.FamilyName;
        //            string className = info.ClassName;
        //            string baseClass = info.BaseClass;
        //            if (string.IsNullOrEmpty(categoryName) && string.IsNullOrEmpty(familyName)) continue;
        //            if (noModelBaseClassList.Contains(baseClass)) continue;
        //            if (noModelClassList.Contains(className)) continue;
        //            if (noModelCategoryList.Contains(categoryName)) continue;
        //            if (noModelFamilyList.Contains(familyName)) continue;

        //            if (categoryName != null && categoryName.Contains("明细表")) continue;
        //            if (categoryName != null && categoryName.Contains("类型设置")) continue;
        //            if (categoryName != null && categoryName.EndsWith("标记")) continue;//"风管标记", "管道标记","房间标记"
        //            if (categoryName != null && categoryName.EndsWith("系统")) continue;//"管道系统",配电系统
        //            if (categoryName != null && categoryName.EndsWith("材质")) continue;

        //            //if (categoryName == "中心线") continue;
        //            this.Add(info);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var item in list)
        //        {
        //            var info = ElementInfoFactory.CreateElementInfo(item);
        //            this.Add(info);
        //        }
        //    }
            
        //    this.Sort();
        //}

        //public void GetMeshInfo()
        //{
        //    try
        //    {
        //        Log.Info("ElementInfoList.GetMeshInfo", "Count:" + Count);
        //        for (int i = 0; i < Count; i++)
        //        {
        //            var item = this[i];
        //            item.GetMeshInfo();
        //            Log.Progress("ElementInfoList.GetMeshInfo", item.ToString(), (i + 1), Count);
        //            Application.DoEvents();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Info("ElementInfoList.GetMeshInfo", ex.ToString());
        //    }
        //}
    }
}
