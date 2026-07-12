using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FAFramework.Manager
{
    public static class XmlSerializerManager
    {
        private static Dictionary<Type, XmlSerializer> _xmlSerializers = new Dictionary<Type, XmlSerializer>();

        public static XmlSerializer GetXmlSerializer(Type type)
        {
            if (!_xmlSerializers.ContainsKey(type))
                _xmlSerializers.Add(type, new XmlSerializer(type));

            return _xmlSerializers[type];
        }
    }
}
