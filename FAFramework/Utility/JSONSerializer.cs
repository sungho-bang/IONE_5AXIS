using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FAFramework.Utility
{
    public class JSONSerializer
    {
        public static object Load(string filename, Type type)
        {
            object result;
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var serializer = Manager.JSONSerializeManager.GetSerializer(type);
                result = serializer.ReadObject(stream);
            }

            return result;
        }

        public static void Save(string filename, object obj)
        {
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                var ser = Manager.JSONSerializeManager.GetSerializer(obj.GetType());
                ser.WriteObject(stream, obj);
            }
        }
    }
}
