using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NavisPlugins.Infos
{
    public class PropertyCategoryInfo
    {
        [XmlAttribute]
        public string DisplayName { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        public List<DataPropertyInfo> Properties = new List<DataPropertyInfo>();
    }
}
