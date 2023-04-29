using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DbModel.Location.Pipes
{
    [Serializable]
    public class PipeSystem
    {
        private string guid="";

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int PId { get; set; }//ParentId

        [DataMember]
        public string Name { get; set; }

        private string key="";
        public string GetKey()
        {
            if(string.IsNullOrEmpty(guid)){
                guid=Guid.NewGuid().ToString();
            }
            if(string.IsNullOrEmpty(key)){
                if(Id==0){
                    //key=Name;
                    key=guid;
                }
                else{
                    key=Id+"";
                }
            }
            return key;
        }

        [DataMember]
        public float SizeX { get; set; }

        [DataMember]
        public float SizeY { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Code { get; set; }

        //[NotMapped]
        [DataMember]
        public List<PipePoint> Points { get; set; }

        public PipeSystem Clone(double offX,double offY)
        {
            PipeSystem clone=new PipeSystem();
            clone.Id=this.Id;
            clone.PId=this.PId;
            clone.Name=this.Name;
            clone.key=this.key;
            clone.SizeX=this.SizeX;
            clone.SizeY=this.SizeY;
            clone.Type=this.Type;
            clone.Code=this.Code;
            clone.PosX=this.PosX;
            clone.PosY=this.PosY;
            clone.PosZ=this.PosZ;
            clone.ScaleX=this.ScaleX;
            clone.ScaleY=this.ScaleY;
            clone.ScaleZ=this.ScaleZ;
            clone.RotationX=this.RotationX;
            clone.RotationY=this.RotationY;
            clone.RotationZ=this.RotationZ;
            clone.Points=new List<PipePoint>();
            if(this.Points!=null)
                foreach(var p in this.Points)
                {
                    clone.Points.Add(p.Clone(offX,offY));
                }
            return clone;
        }

        public void SortPoints()
        {
            Points.Sort((a,b)=>{
                int r1=a.Num.CompareTo(b.Num);
                if(r1==0){
                    r1=a.Id.CompareTo(b.Id);
                }
                return r1;
            });
            ResetNums();
        }

        public int PointCount
        {
            get{
                if(Points==null)return 0;
                return Points.Count;
            }
        }

        private Dictionary<string,PipePoint> pointsDict=new Dictionary<string, PipePoint>();
        public void ResetNums()
        {
            for(int i=0;i<Points.Count;i++)
            {
                Points[i].Num=i+1;
            }
        }
        internal int InsertPoint(PipePoint newPoint,PipePoint currentPoint)
        {
            int id=-1;
            PipePoint pointOld=GetPoint(currentPoint.GetKey());
            if(pointOld==null)return id;
            UnityEngine.Debug.Log($"InsertPoint_1 Points:{Points.Count}");
            if(pointOld!=null){
                //Points.Remove(pointsDict[currentPoint.Id]);
                var point=pointOld;
                id=Points.IndexOf(point);
                Points.Insert(id+1,newPoint);
            }
            ResetNums();
            UnityEngine.Debug.Log($"InsertPoint_2 Points:{Points.Count}");
            return id;
        }

        internal int DeletePoint(PipePoint point)
        {
            UnityEngine.Debug.Log($"DeletePoint_1 Points:{Points.Count}");
            // PipePoint pointOld=GetPoint(point.Id);
            // if(pointOld!=null){
            //     Points.Remove(pointOld);
            // }
            int id=GetPointIndex(point);
            if(id!=-1){
                Points.RemoveAt(id);
            }
            UnityEngine.Debug.Log($"DeletePoint_2 Points:{Points.Count}");
            return id;
        }

        internal int GetPointIndex(PipePoint point)
        {

            PipePoint pointOld=GetPoint(point.GetKey());
            if(pointOld==null)return -1;
            int id=Points.IndexOf(pointOld);
            return id;
        }

        internal PipePoint GetPoint(string pointKey)
        {
            if(Points==null)return null;
            if(pointsDict.Count==0){
                for (int i = 0; i < Points.Count; i++)
                {
                    pointsDict.Add(Points[i].GetKey(),Points[i]);
                }
            }
            //UnityEngine.Debug.Log($"GetPoint_1 Points:{Points.Count}");
            if(pointsDict.ContainsKey(pointKey)){
                return pointsDict[pointKey];
            }
            UnityEngine.Debug.Log($"GetPoint NotFoundPoint pointId:{pointKey} Points:{Points.Count}");
            return null;
        }

        public override string ToString()
        {
            if(Points!=null){
                return $"{Id}|{PId}|{Name}|{SizeX},{SizeY}|{Type}|{Code}|{Points.Count}";
            }
            else{
                return $"{Id}|{PId}|{Name}|{SizeX},{SizeY}|{Type}|{Code}";
            }
            
        }

        #region DevPos信息
        /// <summary>
        /// PosX
        /// </summary>
        [DataMember]
        //[Display(Name = "位置信息X")]
        public float PosX { get; set; }

        /// <summary>
        /// PosY
        /// </summary>
        [DataMember]
        //[Display(Name = "位置信息Y")]
        public float PosY { get; set; }

        /// <summary>
        /// PosZ
        /// </summary>
        [DataMember]
        //[Display(Name = "位置信息Z")]
        public float PosZ { get; set; }

        /// <summary>
        /// RotationX
        /// </summary>
        [DataMember]
        //[Display(Name = "角度信息X")]
        public float RotationX { get; set; }

        /// <summary>
        /// RotationY
        /// </summary>
        [DataMember]
        //[Display(Name = "角度信息Y")]
        public float RotationY { get; set; }

        /// <summary>
        /// RotationZ
        /// </summary>
        [DataMember]
        //[Display(Name = "角度信息Z")]
        public float RotationZ { get; set; }

        /// <summary>
        /// ScaleX
        /// </summary>
        [DataMember]
        //[Display(Name = "比例信息X")]
        public float ScaleX { get; set; }

        /// <summary>
        /// ScaleY
        /// </summary>
        [DataMember]
        //[Display(Name = "比例信息Y")]
        public float ScaleY { get; set; }

        /// <summary>
        /// ScaleZ
        /// </summary>
        [DataMember]
        //[Display(Name = "比例信息Z")]
        public float ScaleZ { get; set; }

        #endregion

        public Vector3 GetPos(){
            return new Vector3(PosX,PosY,PosZ);
        }
    }
}
