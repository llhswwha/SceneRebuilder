//using Location.IModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DbModel.Location.Pipes
{
    [Serializable]
    public class PipeType//:IId
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public int Segments { get; set; }

        [DataMember]
        public bool IsSmooth { get; set; }

        public PipeType()
        {

        }

        public PipeType(string name,string color)
        {
            this.Name = name;
            this.Color = color;
        }

        public PipeType(string name, string color,int seg,bool smooth)
        {
            this.Name = name;
            this.Color = color;
            this.Segments = seg;
            this.IsSmooth = smooth;
        }
    }
}
