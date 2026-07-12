using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace FALibrary.Utility
{
    public class FAUtility
    {
        private static Dictionary<Type, XmlSerializer> _xmlSerializers = new Dictionary<Type, XmlSerializer>();

        private static byte[] BYTE_BIT_MASK = { 1, 2, 4, 8, 16, 32, 64, 128 };       

        public static bool CheckBit(ushort num, int index)
        {
            if (index < 0 || index >= 32) return false;

            if (((num >> index) & 1) == 0) return false;
            else return true;
        }

        public static byte SetBit(byte num, int index, bool value)
        {
            if (index < 0 || index > 7) return num;

            if (value == true)
                return (byte)(num | BYTE_BIT_MASK[index]);
            else
                return (byte)(num & ~BYTE_BIT_MASK[index]);
        }

        public static XElement GetElement(XElement xml, string key, string value)
        {
            if (xml == null) return null;

            foreach (XElement item in xml.Elements())
            {
                if (item.Element(key) != null)
                {
                    if (item.Element(key).Value.Trim() == value)
                        return item;
                }
            }

            return null;
        }

        public static object Deserialize(XDocument doc, Type type)
        {
            XmlSerializer xmlSerializer = GetXmlSerializer(type);

            using (var reader = doc.Root.CreateReader())
            {
                return xmlSerializer.Deserialize(reader);
            }
        }

        public static XDocument Serialize(object value)
        {
            XmlSerializer xmlSerializer = GetXmlSerializer(value.GetType());

            XDocument doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.IndentChars = "  ";
                setting.NewLineOnAttributes = true;
                setting.OmitXmlDeclaration = true;

                XmlWriter xw = XmlWriter.Create(writer, setting);
                xmlSerializer.Serialize(xw, value);
            }

            return doc;
        }

        public static XmlSerializer GetXmlSerializer(Type type)
        {
            if (!_xmlSerializers.ContainsKey(type))
                _xmlSerializers.Add(type, new XmlSerializer(type));

            return _xmlSerializers[type];
        }

        /// <summary>
        /// 시간 안에 동작을 완료하는지 확인한다.
        /// </summary>
        /// <param name="compare">비교</param>
        /// <param name="timeout">millisecond. 이 시간을 초과하면 false를 return</param>
        /// <returns>timeout을 초과하면 false. timeout 시간 안에 compare() == true이면 true</returns>
        public static bool Compare(Func<bool> compare, Action actionWhenSuccess, int timeout)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                if (compare())
                {
                    if (actionWhenSuccess != null)
                        actionWhenSuccess();
                    stopwatch.Stop();
                    return true;
                }
            }

            stopwatch.Stop();
            return false;
        }

        public static void Wait(int timeout)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < timeout)
            {
            }
        }
    }
}
