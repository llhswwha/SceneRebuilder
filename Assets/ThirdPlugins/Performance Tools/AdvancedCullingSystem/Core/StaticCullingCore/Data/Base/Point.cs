namespace AdvancedCullingSystem.StaticCullingCore
{
    /// <summary>
    /// （边界信息的）点信息
    /// </summary>
    public class Point
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int Index { get; set; }

        public int BoundId { get; set; }

        public virtual Bound Bound { get; set; }

        public Point()
        {

        }

        public Point(double x, double y, int index)
        {
            X = x;
            Y = y;
            Index = index;
        }

        public Point(double x, double y, double z, int index)
        {
            X = x;
            Y = y;
            Z = z;
            Index = index;
        }

        public Point(Point p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
            Index = p.Index;
        }
    }
}