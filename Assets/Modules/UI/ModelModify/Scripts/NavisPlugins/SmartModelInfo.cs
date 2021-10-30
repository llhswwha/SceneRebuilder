using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NavisPlugins.Infos
{
    [DataContract]
    public class SmartModelInfo
    {
        /// <summary>
        /// 数据库自增id 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        ///模型id
        /// </summary>
        [DataMember]
        public string  ModelId { get; set; }


        /// <summary>
        /// 模型名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 模型信息
        /// </summary>
        [DataMember]
        public string Categories { get; set; }        

    }
}
