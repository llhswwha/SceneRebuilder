//using Autodesk.Revit.DB;
using System;

namespace RevitTools.Infos
{
    public class LocationInfo
    {
        //private Location location;

        public string TypeName = "";

        public XYZInfo Point { get; set; }

        public double Roation { get; set; }

        public XYZInfo Origin { get; set; }

        public XYZInfo Direction { get; set; }

        public double Length { get; set; }

        //public LocationInfo(Location location)
        //{
        //    this.location = location;
        //    if (location == null) return;
        //    this.TypeName = location.GetType() + "";
        //    if (location is LocationPoint)
        //    {
        //        var point = location as LocationPoint;
        //        Point = new XYZInfo(point.Point);
        //        try
        //        {
        //            Roation = point.Rotation;
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }

        //    if (location is LocationCurve)
        //    {
        //        var curve = location as LocationCurve;
        //        var line = curve.Curve as Line;
        //        if (line != null)
        //        {
        //            Origin = new XYZInfo(line.Origin);
        //            Direction = new XYZInfo(line.Direction);
        //            Length = line.Length;
        //            //还有个半径或者直径
        //        }

        //        //try
        //        //{
        //        //    Roation = point.Rotation;
        //        //}
        //        //catch (Exception ex)
        //        //{

        //        //}
        //    }
        //}

        public override string ToString()
        {
            if (Point != null)
            {
                return string.Format("Point:{0},{1:F2}", Point, Roation);
            }
            else if (Origin != null)
            {
                return string.Format("Line:{0},{1},{2:F2}", Origin,Direction,Length);
            }
            else
            {
                return string.Format("Type:{0}", TypeName);
            }
        }
    }
}
