using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace FAFramework.Utility
{
    [Serializable]
    public class ObjectPropertyInfo
    {
        public string PropertyName { get; set; }
        public bool Observable { get; set; }
        public string Description { get; set; }
        public List<ObjectPropertyInfo> Properties { get; set; } = new List<ObjectPropertyInfo>();
        [XmlIgnore]
        public object Value { get; set; }
    }
}
