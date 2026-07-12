using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FAFramework.Utility
{
    public class XMLSerializer
    {
        public static object Load(string filename, Type type)
        {
            object result;
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var serializer = Manager.XmlSerializerManager.GetXmlSerializer(type);
                result = serializer.Deserialize(stream);
            }

            return result;
        }

        public static void Save(string filename, object obj)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                var ser = Manager.XmlSerializerManager.GetXmlSerializer(obj.GetType());
                ser.Serialize(stream, obj);
            }
        }
    }
}
