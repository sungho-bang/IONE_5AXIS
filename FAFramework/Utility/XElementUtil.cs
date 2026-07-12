using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace FAFramework.Utility
{
    public static class XElementUtil
    {
        public static XElement ToXElement(this object obj)
        {
            if (obj == null) return null;

            var xmlSerializer = FALibrary.Utility.FAUtility.GetXmlSerializer(obj.GetType());
            XDocument doc = new XDocument();
            using (XmlWriter xw = doc.CreateWriter())
            {
                xmlSerializer.Serialize(xw, obj);
                xw.Close();
            }

            return doc.Root;
        }

        public static XElement ToXElement<T>(this object obj)
        {
            if (obj == null) return null;

            var xmlSerializer = FALibrary.Utility.FAUtility.GetXmlSerializer(typeof(T));
            XDocument doc = new XDocument();
            using (XmlWriter xw = doc.CreateWriter())
            {
                xmlSerializer.Serialize(xw, obj);
                xw.Close();
            }

            return doc.Root;
        }

        public static T ToObject<T>(this XElement xElement)
        {
            var xmlSerializer = FALibrary.Utility.FAUtility.GetXmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }
    }
}
