
namespace RevitTools.Infos
{
    public class XYZInfo
    {
        //private XYZ point;

        //public XYZInfo(XYZ point)
        //{
        //    this.point = point;
        //    this.X = point.X;
        //    this.Y = point.Y;
        //    this.Z = point.Z;
        //}

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public override string ToString()
        {
            return string.Format("({0:F3},{1:F3},{2:F3})", X, Y, Z);
        }
    }
}
