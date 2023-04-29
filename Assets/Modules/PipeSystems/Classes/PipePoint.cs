using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Location.Pipes
{
    [Serializable]
    public class PipePoint
    {
        private string key="";
        public string GetKey()
        {
            if(string.IsNullOrEmpty(key)){
                key=$"{PId}_{Id}_{Num}";
            }
            return key;
        }

        [DataMember]
        public int Id { get; set; }
        
        [DataMember]
        public int PId { get; set; }//PipeSystemId

        [DataMember]
        public int Num { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public double Z { get; set; }

        public override string ToString()
        {
            return $"P{Num}_({X:F3},{Y:F3},{Z:F3})";
        }

        public static double ppOffsetX=4960000;
        public static double ppOffsetY=430000;

        public static double ppOffsetXMin=4900000;
        public static double ppOffsetYMin=400000;

        public PipePoint Clone(double offX,double offY){
            PipePoint p=new PipePoint();
            p.Id=this.Id;
            p.PId=this.PId;
            p.Num=this.Num;

            // p.X=this.X+offX;
            // p.Y=this.Y+offY;

            //兼容旧数据
            if(offX>0){
                if(this.X<ppOffsetXMin){
                    p.X=this.X+offX;
                }
                else{
                    p.X=this.X;
                }
            }
            else{
                if(this.X>ppOffsetXMin){
                    p.X=this.X+offX;
                }
                else{
                    p.X=this.X;
                }
            }

            if(offY>0){
                if(this.Y<ppOffsetYMin){
                    p.Y=this.Y+offY;
                }
                else{
                    p.Y=this.Y;
                }
            }
            else{
                if(this.Y>ppOffsetYMin){
                    p.Y=this.Y+offY;
                }
                else{
                    p.Y=this.Y;
                }
            }
            
            p.Z=this.Z;
            p.key=this.key;
            //UnityEngine.Debug.Log($"PipePoint.Clone ({this.X} |{p.X}| {ppOffsetXMin}) ({this.Y} |{p.Y}| {ppOffsetYMin})");
            return p;
        }
    }
}
