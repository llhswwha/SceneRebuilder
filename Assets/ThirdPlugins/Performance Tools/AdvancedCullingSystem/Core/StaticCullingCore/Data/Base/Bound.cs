using System.Collections.Generic;

namespace AdvancedCullingSystem.StaticCullingCore
{
    /// <summary>
    ///     边界信息 地图和区域
    /// </summary>
    
    public class Bound
    {
        
        public int Id { get; set; }

        
        public double MinX { get; set; }

        
        public double MaxX { get; set; }

        
        public double MinY { get; set; }

        
        public double MaxY { get; set; }

        
        public double MinZ { get; set; }

        
        public double MaxZ { get; set; }

        /// <summary>
        /// 是否长方形
        /// </summary>
        
        public bool IsRectangle { get; set; }

        /// <summary>
        /// 是否相对坐标
        /// </summary>
        
        public bool IsRelative { get; set; }

        ///// <summary>
        ///// 是否是普通区域
        ///// </summary>
        //
        //public bool IsOnNormalArea { get; set; }

        ///// <summary>
        ///// 是否是定位区域
        ///// </summary>
        //
        //public bool IsOnLocationArea { get; set; }

        
        public List<Point> Points { get; set; }

        public Bound()
        {
            MaxZ = 1;
            IsRelative = true;
        }

        public Bound(double x1, double y1, double x2, double y2, double bottomHeightT, double thicknessT, bool isRelative) : this()
        {
            SetInitBound(x1, y1, x2, y2, bottomHeightT, thicknessT);
            IsRectangle = true;
            IsRelative = isRelative;
        }

        public Bound(Point[] points, double bottomHeightT, double thicknessT, bool isRelative) : this()
        {
            SetInitBound(points, bottomHeightT, thicknessT);
            IsRectangle = false;
            IsRelative = isRelative;
        }

        /// <summary>
        /// 用两点(对角点)初始化区域范围
        /// </summary>
        public void SetInitBound(double x1, double y1, double x2, double y2, double bottomHeightT, double thicknessT)
        {
            MinX = double.MaxValue;
            MinY = double.MaxValue;
            MaxX = double.MinValue;
            MaxY = double.MinValue;
            MinZ = 0 + bottomHeightT;
            MaxZ = thicknessT + bottomHeightT;

            if (x1 < MinX)
            {
                MinX = x1;
            }
            if (x2 < MinX)
            {
                MinX = x2;
            }

            if (y1 < MinY)
            {
                MinY = y1;
            }
            if (y2 < MinY)
            {
                MinY = y2;
            }

            if (x1 > MaxX)
            {
                MaxX = x1;
            }
            if (x2 > MaxX)
            {
                MaxX = x2;
            }


            if (y1 > MaxY)
            {
                MaxY = y1;
            }
            if (y2 > MaxY)
            {
                MaxY = y2;
            }

            //double pX = (MinX + MaxX)/2.0;
            //double pY = (MinY + MaxY)/2.0;
            //double pZ = (MinZ + MaxZ)/2.0;
            Points = new List<Point>();
            Points.Add(new Point(MinX, MinY, 0));
            Points.Add(new Point(MaxX, MinY, 1));
            Points.Add(new Point(MaxX, MaxY, 2));
            Points.Add(new Point(MinX, MaxY, 3));
            //Points.Add(new Point(MinX - MinX, MinY - MinY, 0));
            //Points.Add(new Point(MaxX - MinX, MinY - MinY, 1));
            //Points.Add(new Point(MaxX - MinX, MaxY - MinY, 2));
            //Points.Add(new Point(MinX - MinX, MaxY - MinY, 3));
        }

        /// <summary>
        /// 用两点(对角点)初始化区域范围
        /// </summary>
        public void SetInitBound(Point[] points, double bottomHeightT, double thicknessT)
        {
            Points = new List<Point>();

            if (points.Length == 0)
            {
                MinX = 0;
                MinY = 0;
                MaxX = 1;
                MaxY = 1;
                MinZ = 0;
                MaxZ = 1;
                return;
            }

            MinX = float.MaxValue;
            MinY = float.MaxValue;
            MaxX = float.MinValue;
            MaxY = float.MinValue;
            MinZ = 0 + bottomHeightT;
            MaxZ = thicknessT + bottomHeightT;

            for (int i = 0; i < points.Length; i++)
            {
                Point point = points[i];
                point.Index = i;
                if (point.X < MinX)
                {
                    MinX = point.X;
                }
                if (point.Y < MinY)
                {
                    MinY = point.Y;
                }
                if (point.X > MaxX)
                {
                    MaxX = point.X;
                }
                if (point.Y > MaxY)
                {
                    MaxY = point.Y;
                }
            }

            for (int i = 0; i < points.Length; i++)
            {
                Point point = points[i];
                //point.X -= MinX;
                //point.Y -= MinY;
                Points.Add(new Point(point));
            }

            //double pX = (MinX + MaxX)/2.0;
            //double pY = (MinY + MaxY)/2.0;
            //double pZ = (MinZ + MaxZ)/2.0;
        }

        public void AddPoint(Point point)
        {
            if (Points == null)
            {
                Points = new List<Point>();
            }
            Points.Add(point);
        }
    }
}